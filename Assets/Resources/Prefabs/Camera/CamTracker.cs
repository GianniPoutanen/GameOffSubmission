using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTracker : MonoBehaviour
{
public int ID;

    // Start is called before the first frame update
    void Start()
    {
        GameAssets.Instance.AddToPool("Cameras", this.gameObject);
        ID = GameAssets.Instance.GetPoolItemCount("Cameras");
    }

}
