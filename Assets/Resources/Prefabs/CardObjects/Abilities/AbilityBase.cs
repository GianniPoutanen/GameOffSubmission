using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Card", menuName = "Ability")]
public class AbilityBase : ScriptableObject
{
    public enum TargetingType
    {
        Single,
        Tile,
        Area,
        Chain,
        None
    }

    [Header("The Cost of this ability")]
    public int cost = 0;

    [Header("The Targeting Type")]
    public TargetingType targetingType;

    [SerializeField]
    public Color abilityColour = Color.white;

    public virtual bool CanCast()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Cancel()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Cast()
    {
        throw new System.NotImplementedException();
    }

    public virtual List<PieceBase> GetTargets()
    {
        return new List<PieceBase>();
    }

    public virtual List<Vector3Int> GetTileTargets()
    {
        return new List<Vector3Int>();
    }
}
