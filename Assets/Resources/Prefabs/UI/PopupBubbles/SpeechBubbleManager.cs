using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubbleManager : MonoBehaviour
{
    private bool InDialog { get; set; } = false;


    #region Constructor
    private static GameManager _i;
    public static GameManager Instance
    {
        get
        {
            if (_i == null)
            {
                _i = Instantiate(Resources.Load<GameManager>("GameManager"));
                GameAssets.Instance.AddToPool("Managers", _i.gameObject);
            }

            return _i;
        }
    }
    #endregion Constructor


    public void SpawnBubble()
    {
        
    }
}
