using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Poison Ability")]
public class PoisonAbility : AbilityBase
{
    [SerializeField]
    [Header("Ability Effect")]
    public int poisonAmount = 2;

    [Header("Ability Target Tuning")]
    public float targetDistance = 1f;
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
        PoisonStatus status = target.gameObject.AddComponent<PoisonStatus>();
        status.amount = poisonAmount;
    }
}
