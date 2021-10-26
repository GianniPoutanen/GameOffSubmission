using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SelectionMap : MonoBehaviour
{
    List<Vector3Int> previousMarkerPostiions = new List<Vector3Int>();
    Tilemap map;

    FieldManager fm;
    public TileBase markerTile;

    public bool canSelect = false;


    // Start is called before the first frame update
    void Start()
    {
        map = this.GetComponent<Tilemap>();
        fm = this.transform.parent.GetComponent<FieldManager>();
    }

    public void SetMarker(Vector2 initPos)
    {
        ClearMarkers();

        previousMarkerPostiions.Clear();
        previousMarkerPostiions.Add(map.WorldToCell(initPos));

        foreach (Vector3Int cellPos in previousMarkerPostiions)
        {
            DrawTile(cellPos, markerTile, Color.white);
        }
    }

    public void SetMarkers(List<Vector2> positions)
    {
        ClearMarkers();
        foreach (Vector2 pos in positions)
        {
            previousMarkerPostiions.Clear();
            previousMarkerPostiions.Add(map.WorldToCell(pos));
        }
        foreach (Vector3Int cellPos in previousMarkerPostiions)
        {
            DrawTile(cellPos, markerTile, Color.white);
        }
    }

    public void SetMarkers(List<Vector3Int> positions)
    {
        ClearMarkers();
        previousMarkerPostiions.Clear();
        foreach (Vector3Int pos in positions)
        {
            previousMarkerPostiions.Add(pos);
        }
        foreach (Vector3Int cellPos in previousMarkerPostiions)
        {
            DrawTile(cellPos, markerTile, Color.white);
        }
    }

    public void ClearMarkers()
    {
        foreach (Vector3Int cellPos in previousMarkerPostiions)
        {
            map.SetTile(cellPos, null);
        }
    }


    public void DrawTile(Vector3Int coord, TileBase tile, Color color)
    {
        map.SetTile(new Vector3Int(coord.x, coord.y, 0), tile);
        map.SetTileFlags(new Vector3Int(coord.x, coord.y, 0), TileFlags.None);
        map.SetColor(new Vector3Int(coord.x, coord.y, 0), color == null ? Color.white : color);
        map.RefreshTile(new Vector3Int(coord.x, coord.y, 0));
    }
}
