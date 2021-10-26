using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameAssets : MonoBehaviour
{

    #region Constructor
    private static GameAssets _i;

    public static GameAssets Instance
    {
        get
        {
            if (_i == null)
            {
                _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
                _i.AddToPool("Managers", _i.gameObject);
            }

            return _i;
        }
    }
    #endregion Constructor

    [Header("Pop Up Objects")]
    public GameObject pfDamagePopup;

    [SerializeField]
    [Header("Sounds")]
    public SoundAudioClip[] soundAudioClips;


    #region Pooling 
    //Pooling System Variable
    private Dictionary<string, GameObject> _pools = new Dictionary<string, GameObject>();

    /// <summary>
    /// Object pooling
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public GameObject GetObject(GameObject obj)
    {
        GameObject retres = null;

        if (_pools.ContainsKey(obj.name + 's'))
        {
            foreach (Transform child in _pools[obj.name + 's'].transform)
            {
                if (!child.gameObject.activeSelf)
                {
                    retres = child.gameObject;
                }
            }
        }
        else
        {
            _pools.Add(obj.name + 's', new GameObject(obj.name));
        }

        if (retres == null)
        {
            retres = Instantiate(obj);
            retres.transform.SetParent(_pools[obj.name + 's'].transform);
        }

        return retres;
    }

    /// <summary>
    /// Object pooling
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public GameObject GetObject(GameObject obj, Transform parent)
    {
        GameObject retres = null;

        foreach (Transform child in parent)
        {
            if (!child.gameObject.activeSelf)
            {
                retres = child.gameObject;
            }
        }

        if (retres == null)
        {
            retres = Instantiate(obj);
            retres.transform.SetParent(parent);
        }
        else
        {
            retres.SetActive(true);
        }

        return retres;
    }



    public GameObject GetObject(string name)
    {
        GameObject retres = null;

        if (_pools.ContainsKey(name + 's'))
        {
            foreach (Transform children in _pools[name + 's'].transform)
            {
                if (!children.gameObject.activeSelf)
                {
                    retres = children.gameObject;
                }
            }
        }
        else
        {
            _pools.Add(name + 's', new GameObject(name));
        }

        if (retres == null)
        {
            retres = new GameObject(name);
            retres.transform.SetParent(_pools[name + 's'].transform);
        }

        return retres;
    }

    public void AddToPool(string poolName, GameObject obj)
    {
        if (!_pools.ContainsKey(poolName))
        {
            _pools.Add(poolName, new GameObject(poolName));
        }
        obj.transform.SetParent(_pools[poolName].transform);
    }

    #endregion Pooling 

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.eSound sound;
        public string resourceLocations;
        private AudioClip[] _clips;

        public AudioClip Clip(int n)
        {
            if (_clips == null)
            {
                _clips = Resources.LoadAll<AudioClip>(resourceLocations);
            }

            return _clips[n];
        }

        /// <summary>
        /// Returns the number of clips in this object
        /// </summary>
        public int Count
        {
            get
            {
                if (_clips == null)
                {
                    _clips = Resources.LoadAll<AudioClip>(resourceLocations);
                }
                return _clips.Length;
            }
        }

        /// <summary>
        /// Gets a random clip of this type
        /// </summary>
        public AudioClip RandomClip
        {
            get
            {
                int randInt = Random.Range(0, 10000000);
                return Clip(randInt % _clips.Length);
            }
        }
    }

}
