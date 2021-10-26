using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Ability", menuName = "Damage Ability")]
public class DamageAbility : AbilityBase
{
    [Header("Ability Effect")]
    public float DamageAmount = 2;

    [Header("Ability Target Tuning")]
    public float targetDistance = 2f;
    public PieceBase target;

    public override bool CanCast()
    {
        target = GameObject.Find("Grid").GetComponent<FieldManager>().GetClosestPiece(Camera.main.ScreenToWorldPoint(Input.mousePosition), targetDistance);
        return target != null;
    }

    public override List<PieceBase> GetTargets()
    {
        List<PieceBase> targets = new List<PieceBase>();
        target = GameObject.Find("Grid").GetComponent<FieldManager>().GetClosestPiece(Camera.main.ScreenToWorldPoint(Input.mousePosition), targetDistance);
        if (target != null)
            targets.Add(target);
        return targets;
    }

    public override void Cancel()
    {
        target = null;
    }

    public override void Cast()
    {
        target.DamageSelf(DamageAmount);
    }
}
