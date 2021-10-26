using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PieceDisplayHandler : MonoBehaviour
{
    [Header("Display Bars")]
    public Bar healthBar;
    public Bar manaBar;
    public TMP_Text shieldText;

    private PieceBase piece;
    private void Awake()
    {
        piece = this.transform.parent.GetComponent<PieceBase>();
        if (piece.Stats.maxManaPool == 0)
        {
            manaBar.gameObject.SetActive(false);
        }
        piece.StatsUpdated += StatsUpdated;
        StatsUpdated();
    }

    private void OnDestroy()
    {
        piece.StatsUpdated -= StatsUpdated;
    }

    private void StatsUpdated()
    {
        healthBar.SetFillAmount(piece.Stats.currentHealth / piece.Stats.maxHealth);
        manaBar.SetFillAmount(piece.Stats.currentManaPool / piece.Stats.maxManaPool);
        shieldText.text = piece.Stats.armour.ToString();
        shieldText.transform.parent.gameObject.SetActive(piece.Stats.armour > 0);
    }
}
