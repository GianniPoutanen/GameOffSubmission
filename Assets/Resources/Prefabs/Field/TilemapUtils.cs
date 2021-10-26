using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapUtils
{

    /// <summary>
    /// Gets the Cell in the map under the mouse
    /// </summary>
    /// <param name="map"></param>
    /// <returns>null if no cell exists</returns>
    public static Vector3Int? CellUnderMouseInMap(Tilemap map)
    {
        Vector3Int? retres = null;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = map.WorldToCell(mousePos);
        if (map.GetTile(cellPos) != null)
        {
            retres = cellPos;
        }
        return retres;
    }
}
