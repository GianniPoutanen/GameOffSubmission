using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager _i;

    public static EffectManager Instance
    {
        get
        {
            if (_i == null)
            {
                _i = Instantiate(Resources.Load<EffectManager>("EffectManager"));
                GameAssets.Instance.AddToPool("Managers", _i.gameObject);
            }

            return _i;
        }
    }

    public void CreateFireball(float damage)
    {

    }
}
