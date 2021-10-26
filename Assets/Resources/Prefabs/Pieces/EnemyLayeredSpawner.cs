using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class EnemyLayeredSpawner : MonoBehaviour
{
    // Private References
    private FieldManager _fm;
    private int currentSpawnIndex;


    // Edited by custom editor
    [HideInInspector]
    public int spawnIndexIndicator;
    [HideInInspector]
    public List<SpawnLayer> spawnLayers = new List<SpawnLayer>();

    private void Awake()
    {
        currentSpawnIndex = 0;
        _fm = GameObject.Find("Grid").GetComponent<FieldManager>();
    }

    private void Start()
    {
        SpawnLayer(0);
    }


    public void HandleSpawnIndicators()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        foreach (SpawnLayer layer in spawnLayers)
        {
            if (layer.loopInterval == currentSpawnIndex + 1)
            {
                foreach (PiecePositionPair pair in layer.spawns)
                {
                    if (pair.piece != null)
                    {
                        GameObject obj = Instantiate(pair.piece.gameObject);
                        obj.transform.position = _fm.fieldTilemap.GetCellCenterWorld(pair.pos);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Spawn layer
    /// </summary>
    public void SpawnCheck_TimerLooped()
    {
        currentSpawnIndex++;
        SpawnLayer(currentSpawnIndex);
    }

    /// <summary>
    /// Spawns the given layer index in the spawn layers
    /// </summary>
    /// <param name="index"></param>
    public void SpawnLayer(int index)
    {
        foreach (SpawnLayer layer in spawnLayers)
        {
            if (layer.loopInterval == index)
            {
                foreach (PiecePositionPair pair in layer.spawns)
                {
                    if (pair.piece != null)
                    {
                        GameObject obj = Instantiate(pair.piece.gameObject);
                        obj.transform.position = _fm.fieldTilemap.GetCellCenterWorld(pair.pos);
                    }
                }
            }
        }
    }

    #region Enable/Disable

    private void OnEnable()
    {
        HUDManager.Instance.TimerLoopedEvent += SpawnCheck_TimerLooped;
    }
    private void OnDisable()
    {
        HUDManager.Instance.TimerLoopedEvent -= SpawnCheck_TimerLooped;
    }

    #endregion Enable/Disable

    #region Editor Functions

    public void AddSpawnLayer()
    {
        spawnLayers.Add(new SpawnLayer());
    }

    public void DeleteLast()
    {
        spawnLayers.RemoveAt(spawnLayers.Count - 1);
    }
    public void RemoveLayerAt(int index)
    {
        spawnLayers.RemoveAt(index);
    }

    #endregion Editor Functions

    private void OnDrawGizmosSelected()
    {
        _fm = GameObject.Find("Grid").GetComponent<FieldManager>();
        foreach (SpawnLayer layer in spawnLayers)
        {
            if (layer.loopInterval == spawnIndexIndicator)
            {
                foreach (PiecePositionPair pair in layer.spawns)
                {
                    Gizmos.color = pair.colour;
                    Gizmos.DrawCube(_fm.fieldTilemap.GetCellCenterWorld(pair.pos), Vector2.one / 4);
                }
            }
        }
    }
}

[System.Serializable]
public class SpawnLayer
{
    public int loopInterval = 0;
    public List<PiecePositionPair> spawns = new List<PiecePositionPair>();
    public void AddSpawn()
    {
        spawns.Add(new PiecePositionPair());
    }

    public void DeleteLast()
    {
        spawns.RemoveAt(spawns.Count - 1);
    }
    public void RemoveSpawnAt(int index)
    {
        spawns.RemoveAt(index);
    }

    //Inpspector flag
    public bool inspectorShow = true;
}

[System.Serializable]
public class PiecePositionPair
{
    public PieceBase piece;
    public Vector3Int pos;
    public Color colour = Color.red;

    //Inpspector flag
    public bool inspectorShow = true;
}