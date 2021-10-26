using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonStatus : MonoBehaviour
{
    [Header("Poison Damage Per Loop")]
    public int amount;

    private PieceBase piece;

    private void Awake()
    {
        HUDManager.Instance.TimerLoopedEvent += PoisonTick_OnTimerLooped;
        piece = this.GetComponent<PieceBase>();
    }

    public void PoisonTick_OnTimerLooped()
    {
        piece.DamageSelf(amount);
        amount--;
        if (amount <= 0)
            Destroy(this);
    }

    private void OnDestroy()
    {
        HUDManager.Instance.TimerLoopedEvent -= PoisonTick_OnTimerLooped;
    }
}
