using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardData : ScriptableObject
{
    public enum CardType
    {
        Spell,
        Summon,
    }

    [Header("Card Info")]
    public string cardName;
    public CardType cardType;

    [Header("Card Abilities")]
    [SerializeField]
    public AbilityBase[] cardAbilities;

}
