using PathCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// delegates
public delegate void TimerLooped();
public delegate void HealthChanged();
public delegate void ManaChanged();

public class HUDManager : MonoBehaviour
{
    [Header("Health Values")]
    public float maxHealth;
    public float currentHealth;

    [Header("Mana Values")]
    public float maxMana;
    public float currentMana;
    public float manaIncreaseAmount = 2;

    [Header("HUD Bars")]
    public Bar _healthBar;
    public Bar _manaBar;
    public Bar _damageTimer;

    [Header("Hand Variables")]
    public HandManager _hand;

    [Header("Timer Options")]
    public bool timerGoing;
    public float loopTime;
    private float loopTimer;

    [Header("Cast Line")]
    public PathCreator castLine;

    [Header("Deck References")]
    public TMP_Text drawDeckNumber;
    public TMP_Text discardDeckNumber;


    // Private references
    private FieldManager fm;

    #region Constructor
    private static HUDManager _i;

    public static HUDManager Instance
    {
        get
        {
            if (_i == null)
            {
                _i = GameObject.Find("UI").GetComponent<HUDManager>();
            }

            return _i;
        }
    }
    #endregion Constructor

    private void Awake()
    {
        loopTimer = loopTime;
        currentHealth = maxHealth;
        fm = GameObject.Find("Grid").GetComponent<FieldManager>();
    }

    private void Update()
    {
        HandleTimer();
    }

    /// <summary>
    /// Handle the damage timer
    /// </summary>
    private void HandleTimer()
    {
        if (timerGoing)
        {
            if (loopTimer <= 0)
            {
                loopTimer = loopTime;
                float damageAmount = 0;
                if (fm.GetPiecesOnTeam(PieceBase.Team.ally).Count <= 0)
                    damageAmount = fm.GetPiecesOnTeam(PieceBase.Team.enemy).Count;

                OnTimerLooped();
                ChangeHealthBarValue(-1);
                ChangeManaBarValue(manaIncreaseAmount);
            }
            _damageTimer.SetFillAmount(loopTimer / loopTime);
            loopTimer -= Time.deltaTime * GameManager.Instance.gameSpeed;
        }
    }

    /// <summary>
    /// Changes Health Bar fill value
    /// </summary>
    /// <param name="ChangeAmount"></param>
    private void ChangeHealthBarValue(float ChangeAmount)
    {
        currentHealth += ChangeAmount;
        _healthBar.SetFillAmount(currentHealth / maxHealth);
        HealthChangedEvent?.Invoke();
    }

    /// <summary>
    /// Changes Mana Bar fill value
    /// </summary>
    /// <param name="ChangeAmount"></param>
    private void ChangeManaBarValue(float ChangeAmount)
    {
        currentMana += ChangeAmount;
        _manaBar.SetFillAmount(currentMana / maxMana);
        ManaChangedEvent?.Invoke();
    }

    /// <summary>
    /// Remeoves the mana cost
    /// </summary>
    /// <param name="cost"></param>
    public void RemoveManaCost(int cost)
    {
        ChangeManaBarValue(-cost);
    }

    public void SetDrawValue(int number)
    {
        drawDeckNumber.text = number.ToString();
    }

    public void SetDiscardValue(int number)
    {
        discardDeckNumber.text = number.ToString();
    }

    public Vector2 DrawDeckPosition()
    {
        return drawDeckNumber.GetComponent<RectTransform>().anchoredPosition;
    }

    public Vector2 DiscardDeckPosition()
    {
        return discardDeckNumber.GetComponent<RectTransform>().anchoredPosition;
    }

    #region Events

    public event TimerLooped TimerLoopedEvent;
    public event HealthChanged HealthChangedEvent;
    public event ManaChanged ManaChangedEvent;

    #endregion Events


    #region Invokers

    protected virtual void OnTimerLooped() //protected virtual method
    {
        //if ProcessCompleted is not null then call delegate
        TimerLoopedEvent?.Invoke();
    }

    #endregion Invokers
}