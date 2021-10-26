using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class CardBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private HandManager hm;
    private FieldManager _fm;
    private Image cardImage;
    private Outline outline;

    [Header("Card Data")]
    public CardData cardData;

    [Header("Positioning Variables")]
    public Vector2 targetPosition;
    public RectTransform rect;
    private bool _mouseOver = false;

    [Header("Size Variables")]
    [Range(1f, 2f)]
    public float heighlightSizeIncrease = 1;
    private Vector2 _origSize;

    [Header("Descriptions Holder")]
    public GameObject abilityDescriptionBase;
    public GameObject descriptionsPanel;

    [Header("UI Holders")]
    public TMP_Text cardNameText;
    public TMP_Text cardTypeText;
    public TMP_Text costText;
    public TMP_Text summonHealthText;
    public TMP_Text summonAttackText;

    [Header("Targets")]
    public List<Vector3Int> tileTargets;
    public List<PieceBase> pieceTargets;

    // Start is called before the first frame update
    void Awake()
    {
        hm = this.transform.parent.gameObject.GetComponent<HandManager>();
        _fm = GameObject.Find("Grid").GetComponent<FieldManager>();
        rect = this.GetComponent<RectTransform>();
        cardImage = this.GetComponent<Image>();
        outline = this.GetComponent<Outline>();
        _origSize = rect.sizeDelta;
        hm.AddCard(this);
    }

    private void Start()
    {
        //Setup Look
        SetupDescriptions();
        HandleManaCostChange();

        cardTypeText.text = cardData.cardType.ToString();
        cardNameText.text = cardData.cardName;
        costText.text = CardCost.ToString();

        //Handle if this is a summon card
        SetupSummonStatCircles();
    }

    public void Update()
    {
        if (targetPosition != null && rect.anchoredPosition != targetPosition)
        {
            rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, (Vector2)targetPosition, Vector2.Distance(rect.anchoredPosition, (Vector2)targetPosition) / 3);
        }
    }

    public void HandleCardPlayed()
    {
        Debug.LogError("No card effect played.");
        throw new System.NotImplementedException();
    }

    public void AddAbility()
    {
        //TODO
    }

    public void RemoveAvility()
    {
        // TODO
    }

    #region Card Look Setup

    /// <summary>
    /// Change look based on mana available
    /// </summary>
    public void HandleManaCostChange()
    {
        if (this.CardCost > HUDManager.Instance.currentMana)
        {
            this.costText.color = Color.red;
        }
        else
        {
            this.costText.color = Color.white;
        }
    }

    /// <summary>
    /// Sets up the descriptions for this card
    /// </summary>
    public void SetupDescriptions()
    {
        for (int i = 0; i < cardData.cardAbilities.Length; i++)
        {
            AbilityBase ability = cardData.cardAbilities[i];
            GameObject descriptionObj = null;

            if (ability is DamageAbility)
            {
                descriptionObj = Instantiate(abilityDescriptionBase, descriptionsPanel.transform);
                AbilityDescription desc = descriptionObj.GetComponent<AbilityDescription>();
                desc.SetUp(AbilityDescription.AbilityType.Damage, (int)(ability as DamageAbility).DamageAmount, ability.abilityColour);
            }
            else if (ability is PoisonAbility)
            {
                descriptionObj = Instantiate(abilityDescriptionBase, descriptionsPanel.transform);
                AbilityDescription desc = descriptionObj.GetComponent<AbilityDescription>();
                desc.SetUp(AbilityDescription.AbilityType.Poison, (int)(ability as PoisonAbility).poisonAmount, ability.abilityColour);
            }

            if (descriptionObj != null)
            {
                descriptionObj.transform.SetParent(descriptionsPanel.transform);
                RectTransform rect = descriptionObj.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0, -((rect.sizeDelta.y * ((1 + cardData.cardAbilities.Length) % 2)) / 2) -
                    ((rect.sizeDelta.y * ((cardData.cardAbilities.Length) % 2))) +
                    rect.sizeDelta.y * i);
            }
        }
    }

    /// <summary>
    /// Setup summon stat circles for summon cards
    /// </summary>
    private void SetupSummonStatCircles()
    {
        if (this.cardData.cardType == CardData.CardType.Summon)
        {
            foreach (AbilityBase ability in this.cardData.cardAbilities)
            {
                if (ability is SummonAbilityBase)
                {
                    SummonAbilityBase summon = ability as SummonAbilityBase;

                    summonHealthText.text = summon.Stats.maxHealth.ToString();
                    summonAttackText.text = summon.Stats.attackDamage.ToString();
                    return;
                }
            }
        }
        else
        {
            summonHealthText.transform.parent.gameObject.SetActive(false);
            summonAttackText.transform.parent.gameObject.SetActive(false);
        }
    }

    #endregion Card Look Setup

    public int CardCost
    {
        get
        {
            int retRes = 0;

            foreach (AbilityBase ability in cardData.cardAbilities)
            {
                retRes += ability.cost;
            }

            return retRes;
        }
    }

    #region Pointer Event Handlers
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hm.selectedCard == null)
        {
            _mouseOver = true;
            this.rect.eulerAngles = Vector3.zero;
            this.rect.sizeDelta = _origSize * heighlightSizeIncrease;
            cardImage.raycastTarget = true;
            hm.HandleHeighlightedCard(this);
            this.transform.SetAsLastSibling();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.rect.sizeDelta = _origSize;
        cardImage.raycastTarget = false;
        _mouseOver = false;
        hm.ResetCardPositions();
    }
    #endregion Pointer Event Handlers

    #region Drag Event Handlers
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (HUDManager.Instance.currentMana < this.CardCost)
        {
            eventData.pointerDrag = null;
            hm.ResetCardPositions();
        }
        else
        {
            hm.selectedCard = this;
            this.rect.sizeDelta = _origSize;
            hm.ResetCardPositions();
            this.SetOutline(true);
            if (hm.hasPlayedCard)
                GameManager.Instance.SetGameSpeed(0.2f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (HUDManager.Instance.currentMana < this.CardCost)
        {
            eventData.pointerDrag = null;
        }
        else
        {
            //Change the card look
            hm.ChangeManaCardLook();

            //Clear Highlights
            _fm.ClearHighlight();
            foreach (PieceBase piece in pieceTargets)
            {
                piece.SetHighlight(false);
            }

            pieceTargets.Clear();
            tileTargets.Clear();
            foreach (AbilityBase ab in cardData.cardAbilities)
            {
                pieceTargets.AddRange(ab.GetTargets());
                tileTargets.AddRange(ab.GetTileTargets());
            }

            _fm.UpdateHighlight(tileTargets);
            foreach (PieceBase piece in pieceTargets)
            {
                piece.SetHighlight(true);
            }
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {

        if (HUDManager.Instance.currentMana < this.CardCost)
        {
            eventData.pointerDrag = null;
        }
        else
        {
            // Reset card and field as card has either been cast or cancelled
            hm.ResetCardPositions();
            foreach (PieceBase piece in pieceTargets)
            {
                piece.SetHighlight(false);
            }
            _fm.ClearHighlight();
            hm.selectedCard = null;
            this.SetOutline(false);

            // Check if card can be cast

            // Spell cast handling
            if (CheckCanCast())
            {
                hm.hasPlayedCard = true;
                HUDManager.Instance.RemoveManaCost(CardCost);
                foreach (AbilityBase ab in cardData.cardAbilities)
                {
                    ab.Cast();
                }
                hm.DiscardCard(this);
                Destroy(this.gameObject);
            }
        }
        if (hm.hasPlayedCard)
            GameManager.Instance.SetGameSpeed(1f);
    }
    #endregion Drag Event Handlers

    public bool CheckCanCast()
    {
        bool canCast = true;
        foreach (AbilityBase ab in cardData.cardAbilities)
        {
            if (!ab.CanCast())
                canCast = false;
        }
        return canCast;
    }

    public void SetOutline(bool val)
    {
        this.outline.enabled = val;
    }
}
