using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Sound Volumes")]
    [Range(0f, 1f)]
    public float pieceSoundEffectsVolume;
    [Range(0f, 1f)]
    public float pieceStepsEffectsVolume;
    [Range(0f, 1f)]
    public float musicVolume;

    [Header("Game Speed")]
    [Range(0f, 2f)]
    public float gameSpeed;

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

    #region Time Methosd

    // TODO smooth transition
    public void SetGameSpeed(float speed)
    {
        gameSpeed = speed;
    }

    #endregion Time Methosd

}
