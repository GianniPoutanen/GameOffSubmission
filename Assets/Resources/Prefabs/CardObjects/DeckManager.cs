using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{

    [Space(10)]
    public List<CardData> drawPile = new List<CardData>();
    public List<CardData> discardPile = new List<CardData>();

    [Header("Starting Champ and Deck")]
    public CardData championCard;
    public CardData[] startingDeck;

    public int currentHash = 0;


    private void Awake()
    {
        LoadStartingDeck();
        HUDManager.Instance.SetDrawValue(drawPile.Count);
        HUDManager.Instance.SetDiscardValue(discardPile.Count);
    }

    /// <summary>
    /// Loads the given starting deck into the deck
    /// </summary>
    public void LoadStartingDeck()
    {
        foreach (CardData card in startingDeck)
        {
            drawPile.Add(card);
        }
    }

    /// <summary>
    /// Returns a card from the deck
    /// </summary>
    /// <param name="hash"></param>
    /// <returns></returns>
    public CardData DrawCard(int hash)
    {
        if (drawPile.Count == 0)
            Reshuffle();

        if (drawPile.Count > 0)
        {
            CardData card = drawPile[hash % drawPile.Count];
            drawPile.RemoveAt(hash % drawPile.Count);
            return card;
        }
        HUDManager.Instance.SetDrawValue(drawPile.Count);
        return null;
    }

    /// <summary>
    /// Adds a card
    /// </summary>
    /// <param name="card"></param>
    public void AddToDiscard(CardData card)
    {
        discardPile.Add(card);
        HUDManager.Instance.SetDiscardValue(discardPile.Count);
    }


    /// <summary>
    /// Reshuffled discard into deck
    /// </summary>
    public void Reshuffle()
    {
        if (discardPile.Count > 0)
        {
            drawPile.AddRange(discardPile);
            discardPile.Clear();
        }
        else
        {
            // TODO alert draw empty
            Debug.Log("Deck Empty");
        }
        HUDManager.Instance.SetDrawValue(drawPile.Count);
        HUDManager.Instance.SetDiscardValue(discardPile.Count);
    }


}
