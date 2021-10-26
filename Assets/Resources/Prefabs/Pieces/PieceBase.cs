using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StatsUpdated();

public class PieceBase : MonoBehaviour
{
    [SerializeField]
    public PieceStats _pieceStats = new PieceStats();
    protected FieldManager _fm;


    [Header("Piece Team")]
    [SerializeField]
    public Team allegiance;

    // Target and movement variables
    protected PieceBase _target = null;
    protected bool _actionLock = false;
    protected List<Vector3Int> _cellPath;
    public bool _canBeAttacked = true;
    public bool _canBeHit = true;

    [Header("Piece Status")]
    public ePieceStatus _status = ePieceStatus.Idle;

    [Header("Position and Movement")]
    public Vector2 previousPosition;
    public Vector2 targetPosition;

    [Header("Action Timer")]
    public float _actionTimer = 0;
    public float _actionSpeedMultiplier = 1;

    // Animation State Variables
    private int _currentAnimState;
    private Animator _animator;
    private SpriteRenderer _sr;
    private PieceDisplayHandler _pieceDisplay;

    [Header("Discarded Card On Death")]
    public CardData discardCard = null;

    #region Enums
    public enum Team
    {
        independant,
        enemy,
        ally,
        yellow,
        green,
        red,
        blue,
        purple
    }
    public enum ePieceStatus
    {
        Idle,
        Moving,
        Action,
        Casting
    }

    public enum ePieceBaseAnim
    {
        ERROR,
        FlyingEye,
        Mushroom,
        Skeleton

    }

    public enum ePieceAction
    {
        Idle,
        Move,
        Attack,
        Hit
    }
    #endregion Enums

    [Header("Animator State Constants")]
    public ePieceBaseAnim baseAnimType;
    public float numberOfAttacks = 1;

    #region Constructors

    public PieceBase(PieceStats stats)
    {
        _pieceStats = stats;
    }

    public PieceBase()
    {
    }

    #endregion Constructors

    #region Properties

    public PieceStats Stats
    {
        get { return _pieceStats; }
    }

    #endregion Properties

    /// <summary>
    /// Called before Start
    /// </summary>
    public virtual void Awake()
    {
        _fm = GameObject.Find("Grid").GetComponent<FieldManager>();
        _sr = this.GetComponent<SpriteRenderer>();
        _animator = this.GetComponent<Animator>();
        _actionTimer = Stats.attackSpeed / 2;
        _pieceDisplay = this.transform.GetChild(0)?.GetComponent<PieceDisplayHandler>();
        GameAssets.Instance.AddToPool("Pieces", this.gameObject);
    }

    /// <summary>
    /// Called when script start
    /// </summary>
    public virtual void Start()
    {
        _fm.AddPiece(this);
        targetPosition = this.transform.position;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!_actionLock)
        {
            UpdateTarget();
            if (GameManager.Instance.gameSpeed > 0)
                HandleActions();
            HandleSpriteDirection();
        }
        if (_animator != null)
        {
            _animator.speed = GameManager.Instance.gameSpeed * _actionSpeedMultiplier;
        }
    }


    /// <summary>
    /// Returns true if another piece can start an attack on this piece
    /// </summary>
    /// <returns></returns>
    public virtual bool CanBeAttacked()
    {
        return _canBeAttacked;
    }

    /// <summary>
    /// Returns true if the piece can be hit by damage
    /// </summary>
    /// <returns></returns>
    public virtual bool CanBeHit()
    {
        return _canBeHit;
    }

    /// <summary>
    /// Handles piece action logic
    /// </summary>
    public virtual void HandleActions()
    {
    }

    public virtual void UpdateTarget()
    {
        if (_target == null || _fm.GetPath(this, _target) == null)
            _target = _fm.GetClosestApponent(this);
    }

    public virtual void HandleSpriteDirection()
    {
        if (targetPosition != (Vector2)this.transform.position)
        {
            _sr.flipX = targetPosition.x < this.transform.position.x;
        }
        else if (_target != null)
        {
            _sr.flipX = _target.transform.position.x < this.transform.position.x;
        }
    }

    /// <summary>
    /// Sets the next targeted cell position for this piece to travel to
    /// </summary>
    /// <returns> The next cell towards the target </returns>
    public virtual Vector2 GetStepTowardsTarget()
    {
        Vector2 retres = targetPosition;
        if (_target != null)
        {
            _cellPath = _fm.GetPath(this, _target);

            if (this.transform.position == _fm.fieldTilemap.GetCellCenterWorld(_cellPath[0]))
            {
                _cellPath.RemoveAt(0);
            }

            if (_cellPath.Count > 1)
            {
                retres = _fm.fieldTilemap.GetCellCenterWorld(_cellPath[0]);
            }
        }
        return retres;
    }

    /// <summary>
    /// Applys damage to self
    /// </summary>
    /// <param name="dmg"></param>
    public void DamageSelf(float dmg, float xAwayVector)
    {
        if (CanBeHit())
        {
            float damageTotal = 0;
            damageTotal = dmg - this.Stats.armour;
            if (this.Stats.armour > 0)
            {
                this.Stats.armour -= dmg;
                DamagePopup.Create(this.transform.position, (int)dmg, xAwayVector);
            }

            if (damageTotal > 0)
            {
                this.Stats.currentHealth -= dmg;
                DamagePopup.Create(this.transform.position, (int)dmg, xAwayVector);

                if (this.Stats.currentHealth <= 0)
                {
                    _fm.RemovePiece(this);
                    SoundManager.PlaySound(SoundManager.GetNextSound(SoundManager.eSound.SwordHeavyContact));
                    GameManager.Instance.GetHandManager().DiscardCard(discardCard);
                    Destroy(this.gameObject);
                }
            }
        }

        OnStatsUpdated();
    }

    /// <summary>
    /// Applys damage to self
    /// </summary>
    /// <param name="dmg"></param>
    public void DamageSelf(float dmg)
    {
        if (this.CanBeHit())
            DamageSelf(dmg, 0);
    }

    /// <summary>
    /// Damages the _target object
    /// </summary>
    protected void DamageTarget()
    {
        if (_target != null && _target.gameObject != null)
        {
            _target.DamageSelf(this.Stats.attackDamage, (_target.transform.position - this.transform.position).normalized.x * 3);
            AddMana(this.Stats.attackManaGain);
        }
        OnStatsUpdated();
    }


    protected void AddMana(float amount)
    {
        if ((this.Stats.currentManaPool + amount) > this.Stats.maxManaPool)
        {
            this.Stats.currentManaPool = this.Stats.maxManaPool;
        }
        else
        {
            this.Stats.currentManaPool += amount;
        }
        OnStatsUpdated();
    }

    #region Sound Equipment Conversion

    protected void PlayAttackSound()
    {
        SoundManager.PlaySound(SoundManager.GetNextSound(SoundManager.eSound.SwordHeavySwipe));
    }

    protected void PlayStepSound()
    {
        SoundManager.PlaySound(SoundManager.GetNextSound(SoundManager.eSound.LightArmourGrassStep));
    }


    #endregion Sound Equipment Conversion

    #region Animation Routines

    public virtual void ChangeAnimationState(ePieceAction action)
    {
        _animator.speed = 1;
        if (_currentAnimState != (int)action && !_actionLock)
        {
            if (_animator != null)
            {
                string animName = ConvertAnimationActionToString(action);
                bool loop = true;
                _currentAnimState = (int)action;
                if (action == ePieceAction.Attack)
                {
                    _actionSpeedMultiplier = 1; // TODO - fix speed of attack if attack speed high
                    animName += (int)Random.Range(1, numberOfAttacks + 1);
                    _actionLock = true;
                    loop = false;
                }
                else if (action == ePieceAction.Move)
                {
                    _actionSpeedMultiplier = Stats.moveSpeed;
                }
                else
                {
                    _actionSpeedMultiplier = 1;
                }
                _animator.Play(animName);
                //StartCoroutine(AnimationPlay(animName, loop));
            }
        }
    }

    public void UnlockActions()
    {
        _actionLock = false;
    }

    private string ConvertAnimationActionToString(ePieceAction action)
    {
        if (this.baseAnimType == ePieceBaseAnim.ERROR)
            Debug.LogError("BASE ANIMATION NOT SET");

        return this.Stats.name.Replace(' ', '\0') + action.ToString();
    }

    /*
    private IEnumerator AnimationPlay(string animation, bool loop)
    {
        _animator.Play(animation);
        if (!loop)
        {
            // Wait for animation to play out
            _actionLock = true;
            float time = 0;
            RuntimeAnimatorController ac = _animator.runtimeAnimatorController;    //Get Animator controller
            for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
            {
                if (ac.animationClips[i].name == animation)        //If it has the same name as your clip
                {
                    time = ac.animationClips[i].length;
                    break;
                }
            }
            // unlock animation if not looped
        }

        yield return null;
    }
    */
    private IEnumerator DelayActionTime(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    private void OnAttackEnd()
    {
        UnlockActions();
    }

    #endregion Animation Ruotines

    #region Events

    public event StatsUpdated StatsUpdated;

    #endregion Events



    #region Invokers

    protected virtual void OnStatsUpdated() //protected virtual method
    {
        //if ProcessCompleted is not null then call delegate
        StatsUpdated?.Invoke();
    }

    #endregion Invokers

    public void SetHighlight(bool val)
    {
        this.GetComponent<SpriteOutline>().OutlineColor = val ? Color.white : Color.clear;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(targetPosition, new Vector3(0.1f, 0.1f, 0.1f));
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(previousPosition, new Vector3(0.1f, 0.1f, 0.1f));
    }
}


[System.Serializable]
public class PieceStats
{
    [Header("Name of Sprite")]
    public string name;

    [Header("Higher Means Faster")]
    public float moveSpeed;

    [Header("Lower Means Faster")]
    public float attackSpeed;

    [Header("Health Stats")]
    public float currentHealth;
    public float maxHealth;

    [Header("Mana Stats")]
    public float currentManaPool;
    public float maxManaPool;

    [Header("Damage Stats")]
    public float attackDamage;
    public float attackManaGain;

    [Header("Armour Stats")]
    public float armour;

    public PieceStats(float moveSpeed, float attackSpeed, float currentHealth, float maxHealth, float currentManaPool, float maxManaPool, float attackDamage, float attackManaGain, float armour)
    {
        this.moveSpeed = moveSpeed;
        this.attackSpeed = attackSpeed;
        this.currentHealth = currentHealth;
        this.maxHealth = maxHealth;
        this.currentManaPool = currentManaPool;
        this.maxManaPool = maxManaPool;
        this.attackDamage = attackDamage;
        this.attackManaGain = attackManaGain;
        this.armour = armour;

    }

    public PieceStats()
    {
        this.moveSpeed = 0.05f;
        this.armour = 0f;
        this.attackSpeed = 1;
        this.currentHealth = 5;
        this.maxHealth = 5;
        this.currentManaPool = 10;
        this.maxManaPool = 10;
        this.attackDamage = 1;
        this.attackManaGain = 1;
    }
}