using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public bool hasPlayedCard;

    [Header("Card Display Object")]
    public CardBase cardDisplay;

    [Header("Hand Variables")]
    public int handSizeLimit = 5;
    public List<CardBase> hand = new List<CardBase>();

    [Range(0, 180)]
    public float cardRotation;
    public float gapBetweenCards;
    public float gapHandLimiter;
    public float gapHighlightIncrease;
    public float cardDropOff;

    public CardBase selectedCard;

    [Header("Cast Line")]
    public SpellPathCastIndicator spellPathIndicator;
    public List<PieceBase> highlightedTargets = new List<PieceBase>();

    // Private references
    private FieldManager fm;
    private DeckManager dm;

    // List of all Cards
    List<CardData> cardList = new List<CardData>();

    private void Awake()
    {
        fm = GameObject.Find("Grid").GetComponent<FieldManager>();
        dm = this.GetComponent<DeckManager>();
        cardList.AddRange(Resources.LoadAll<CardData>("Prefabs/CardObjects/Cards/"));
        DrawChampion();
        DrawCard();
        GameManager.Instance.gameSpeed = 0;
    }

    private void Update()
    {
        HandleCastLine();
    }

    #region Enable/Disabled

    private void OnEnable()
    {
        HUDManager.Instance.ManaChangedEvent += CheckManaCards_ManaChanged;
        HUDManager.Instance.TimerLoopedEvent += HandleDrawingCards_TimerLooped;
    }

    private void OnDisable()
    {
        HUDManager.Instance.ManaChangedEvent -= CheckManaCards_ManaChanged;
        HUDManager.Instance.TimerLoopedEvent -= HandleDrawingCards_TimerLooped;
    }

    #endregion Enable/Disabled

    #region Card Methods 

    public void HandleDrawingCards_TimerLooped()
    {
        for (int i = 0; i < 2; i++)
            if (hand.Count < handSizeLimit)
                DrawCard();
    }

    public void DrawChampion()
    {
        GameObject newCard = Instantiate(cardDisplay.gameObject, this.transform);
        newCard.name = dm.championCard.cardName;
        newCard.GetComponent<CardBase>().cardData = dm.championCard;
        newCard.transform.localScale = Vector3.one;
        newCard.GetComponent<RectTransform>().anchoredPosition = HUDManager.Instance.DrawDeckPosition();
    }

    public void DrawCard()
    {
        CardData cardData = dm.DrawCard(0);
        if (cardData != null)
        {
            GameObject newCard = Instantiate(cardDisplay.gameObject, this.transform);
            newCard.name = cardData.cardName;
            newCard.GetComponent<CardBase>().cardData = cardData;
            newCard.transform.localScale = Vector3.one;
            newCard.GetComponent<RectTransform>().anchoredPosition = HUDManager.Instance.DrawDeckPosition();
        }
    }

    public void AddCard(CardBase card)
    {
        hand.Add(card);
        ResetCardPositions();
    }
    public void DiscardCard(CardBase discardCard)
    {
        hand.Remove(discardCard);
        foreach (CardData card in cardList)
        {
            if (card.cardName == discardCard.cardData.cardName)
                dm.AddToDiscard(card);
        }
        ResetCardPositions();
    }
    public void DiscardCard(CardData discardCard)
    {
        dm.AddToDiscard(discardCard);
        ResetCardPositions();
    }

    public int GetIndexOfCard(CardBase card)
    {
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i] == card)
            {
                return i;
            }
        }
        return -1;
    }
    #endregion Card Methods 

    #region Styling and Positioning

    public void ChangeManaCardLook()
    {
        foreach (CardBase card in this.hand)
        {
            card.HandleManaCostChange();
        }
    }

    /// <summary>
    /// Handles pointing the cast line indicator
    /// </summary>
    public void HandleCastLine()
    {
        if (selectedCard != null &&
            (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - Camera.main.ScreenToWorldPoint((selectedCard.rect.position + new Vector3(0, selectedCard.rect.sizeDelta.y / 2))).y) > 0)
        {
            if (selectedCard.pieceTargets.Count > 0 && selectedCard.CheckCanCast())
            {
                spellPathIndicator.ChangePoints(Camera.main.ScreenToWorldPoint(selectedCard.rect.position + new Vector3(0, selectedCard.rect.sizeDelta.y / 2)),
                    (Vector2)selectedCard.pieceTargets[0].transform.position);
            }
            else if (selectedCard.tileTargets.Count > 0 && selectedCard.CheckCanCast())
            {
                spellPathIndicator.ChangePoints(Camera.main.ScreenToWorldPoint(selectedCard.rect.position + new Vector3(0, selectedCard.rect.sizeDelta.y / 2)),
                    fm.fieldTilemap.GetCellCenterWorld(selectedCard.tileTargets[0]));
            }
            else
            {
                spellPathIndicator.ChangePoints(Camera.main.ScreenToWorldPoint(selectedCard.rect.position + new Vector3(0, selectedCard.rect.sizeDelta.y / 2)),
                    Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            // Check if mouse is above card to show line
            if (!spellPathIndicator.gameObject.activeSelf)
            {
                spellPathIndicator.gameObject.SetActive(true);
            }
        }
        else if (spellPathIndicator.gameObject.activeSelf)
        {
            spellPathIndicator.gameObject.SetActive(false);
        }
    }

    public void HandleHeighlightedCard(CardBase card)
    {
        float midPoint = ((float)hand.Count) / 2f;
        int highlightedCardIndex = GetIndexOfCard(card);

        for (int i = 0; i < hand.Count; i++)
        {
            if (i != highlightedCardIndex)
            {
                float xPosOffset = ((50f / -(highlightedCardIndex - i)) * 1.2f) + 10f;
                hand[i].targetPosition = new Vector2(xPosOffset + ((gapBetweenCards + gapHighlightIncrease - ((gapHandLimiter / (handSizeLimit - (hand.Count - 1))))) *
                    (-midPoint + (float)i + (0.5f * ((1 + hand.Count) % 2)))) + ((gapBetweenCards / 2) * ((1 + hand.Count) % 2)),
                    -(Mathf.Abs((hand.Count / 2) - i) * cardDropOff));
                hand[i].transform.SetAsLastSibling();
                hand[i].rect.eulerAngles = new Vector3(0, 0, -(((cardRotation / (float)hand.Count) * ((hand.Count) % 2)) / 2) +
                    (cardRotation / 2f) - ((cardRotation / (float)hand.Count) * ((float)i + ((0.5f * ((1 + hand.Count) % 2))))));
            }
            else
            {
                hand[i].targetPosition = new Vector2(((gapBetweenCards + gapHighlightIncrease - ((gapHandLimiter / (handSizeLimit - (hand.Count - 1))))) * (-midPoint + (float)i + (0.5f * ((1 + hand.Count) % 2)))) + ((gapBetweenCards / 2) * ((1 + hand.Count) % 2)),
                    hand[i].rect.sizeDelta.y / 2f);
                hand[i].transform.SetAsLastSibling();
                hand[i].rect.eulerAngles = Vector2.zero;
            }
        }
    }

    public void ResetCardPositions()
    {
        float midPoint = ((float)hand.Count) / 2f;
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].targetPosition = new Vector2(((gapBetweenCards - ((gapHandLimiter / (handSizeLimit - (hand.Count - 1))))) *
                (-midPoint + (float)i + (0.5f * ((1 + hand.Count) % 2)))) + ((gapBetweenCards / 2) *
                ((1 + hand.Count) % 2)),
                -(Mathf.Abs((hand.Count / 2) - i) * cardDropOff));
            hand[i].transform.SetAsLastSibling();
            hand[i].GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, -(((cardRotation / (float)hand.Count) * ((hand.Count) % 2)) / 2) +
                (cardRotation / 2f) - ((cardRotation / (float)hand.Count) * ((float)i + ((0.5f * ((1 + hand.Count) % 2))))));
        }
    }

    #endregion Styling and Positioning

    #region Event Agrigators

    private void CheckManaCards_ManaChanged()
    {
        ChangeManaCardLook();
    }

    #endregion Event Agrigators

}
