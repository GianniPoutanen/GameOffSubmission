using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Draw Ability")]
public class DrawAbility : AbilityBase
{
    public int drawNumber;

    public override bool CanCast()
    {
        return true;
    }

    public override void Cast()
    {
        HandManager hm = GameManager.Instance.GetHandManager();
        for (int i = 0; i < drawNumber; i++)
            hm.DrawCard();
    }
}
