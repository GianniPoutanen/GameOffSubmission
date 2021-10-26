using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDescription : MonoBehaviour
{
    [Header("The Description Object Parts")]
    public TMP_Text typeText;
    public Image icon;
    public TMP_Text amountText;

    private int amountVal;

    [SerializeField]
    public TypeSpritePair[] typeSpritePairs;

    #region Enum Types

    public enum AbilityType
    {
        Damage,
        Poison
    }

    #endregion Enum Types

    public void SetUp(AbilityType type, int amount, Color typeColour)
    {
        ChangeType(type);
        ChangeAmount(amount);
        ChangeTypeColour(typeColour);
    }

    public void ChangeAmount(int amount)
    {
        amountVal = amount;
        amountText.text = amount.ToString();
    }

    public void ChangeTypeColour(Color colour)
    {
        amountText.color = colour;
    }

    public void ChangeType(AbilityType type)
    {
        typeText.text = type.ToString();
        icon.sprite = GetIcon(type);
    }

    private Sprite GetIcon(AbilityType type)
    {
        foreach (TypeSpritePair pair in typeSpritePairs)
        {
            if (pair.type == type)
                return pair.icon;
        }
        return null;
    }
}

[System.Serializable]
public class TypeSpritePair
{
    [SerializeField]
    public AbilityDescription.AbilityType type;
    [SerializeField]
    public Sprite icon;
}