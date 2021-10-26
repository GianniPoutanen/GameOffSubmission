using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldManager : MonoBehaviour
{
    [Header("Field Tiles")]
    public TileBase fieldBaseTile;
    public TileBase fieldSpawnableTile;
    public TileBase enemySpawnIndicatorTile;
    [Header("Field Maps")]
    public Tilemap fieldTilemap;
    public Tilemap indicatorMap;
    public SelectionMap selectionMap;

    private List<Vector3Int> _indicatedTiles = new List<Vector3Int>();
    private List<Vector3Int> _previousHoverTiles = new List<Vector3Int>();
    private List<Vector3Int> tileCenterPosGizmos = new List<Vector3Int>();

    // List of game pieces
    private List<PieceBase> _pieceList = new List<PieceBase>();

    // Size of the field
    private BoundsInt _fieldSize = new BoundsInt(new Vector3Int(10, 10, 0), new Vector3Int(20, 20, 1));

    // A* testing variables - Delete later
    private Vector3Int? _startPathTest;
    private Vector3Int? _endPathTest;
    private List<Vector3Int> _pathTest = new List<Vector3Int>();

    private void Start()
    {
        _pathTest = new List<Vector3Int>();
    }

    private void Awake()
    {
        _pathTest = new List<Vector3Int>();
    }

    public float GetTilesBetween(PieceBase pieceBase, PieceBase target)
    {
        return Mathf.Round(Vector3.Distance((Vector2)pieceBase.transform.position, target.transform.position) / this.fieldTilemap.cellSize.x);
    }

    public void UpdateHighlight(List<Vector3Int> tilePositions)
    {
        selectionMap.SetMarkers(tilePositions);
    }

    public void ClearHighlight()
    {
        selectionMap.ClearMarkers();
    }


    #region Piece Management

    /// <summary>
    /// Returns a list of pieces on the given team
    /// </summary>
    /// <param name="team"></param>
    /// <returns></returns>
    public List<PieceBase> GetPiecesOnTeam(PieceBase.Team team)
    {
        List<PieceBase> pieces = new List<PieceBase>();

        foreach (PieceBase piece in Pieces)
        {
            pieces.Add(piece);
        }

        return pieces;
    }

    /// <summary>
    /// Get closest piece from pos
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="maxDist"></param>
    /// <returns></returns>
    public PieceBase GetClosestPiece(Vector3 pos, float maxDist)
    {
        float tempDist = float.MaxValue;
        PieceBase retRes = null;

        foreach (PieceBase piece in _pieceList)
        {
            float curPieceDist = Vector2.Distance(piece.transform.position, pos);
            if (tempDist > curPieceDist && curPieceDist < maxDist)
            {
                retRes = piece;
            }
        }

        return retRes;
    }

    /// <summary>
    /// Get a path from one piece to another
    /// </summary>
    /// <param name="challanger"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public List<Vector3Int> GetPath(PieceBase challanger, PieceBase target)
    {
        List<Vector3Int> blockedPositions = new List<Vector3Int>();
        foreach (PieceBase piece in _pieceList)
        {
            if (piece != challanger && piece != target)
            {
                blockedPositions.Add(fieldTilemap.WorldToCell(piece.targetPosition));
            }
        }
        return AStarGridSearch.GetPath(fieldTilemap, fieldTilemap.WorldToCell(challanger.transform.position), fieldTilemap.WorldToCell(target.targetPosition), blockedPositions);
    }

    public bool CheckSpaceFree(Vector3Int cell)
    {
        foreach (PieceBase piece in _pieceList)
        {
            if (piece.targetPosition == (Vector2)fieldTilemap.GetCellCenterWorld(cell))
            {
                return false;
            }
        }
        return true;
    }


    public bool CheckInField(Vector3Int cell)
    {
        return fieldTilemap.GetTile(cell) != null;
    }


    /// <summary>
    /// Registry of pieces on field
    /// </summary>
    public List<PieceBase> Pieces
    {
        get { return _pieceList; }
    }

    /// <summary>
    /// Adds a piece to the field managers registry of pieces, piece also snaps to grid position
    /// </summary>
    /// <param name="piece">The piece to be added</param>
    public void AddPiece(PieceBase piece)
    {
        Vector3Int cellPos = fieldTilemap.WorldToCell(piece.transform.position);
        if (!_pieceList.Contains(piece))
            _pieceList.Add(piece);
        piece.transform.position = fieldTilemap.GetCellCenterWorld(cellPos);
    }

    /// <summary>
    /// Removes a piece to the field managers registry of pieces, piece also snaps to grid position
    /// </summary>
    /// <param name="piece">The piece to be added</param>
    public void RemovePiece(PieceBase piece)
    {
        if (_pieceList.Contains(piece))
            _pieceList.Remove(piece);
    }


    /// <summary>
    /// Returns a new piece for a challanging piece
    /// </summary>
    /// <param name="challanger"></param>
    /// <returns></returns>
    public PieceBase GetClosestApponent(PieceBase challanger)
    {
        PieceBase retres = null;
        int pathDist = int.MaxValue;
        foreach (PieceBase piece in _pieceList)
        {
            if (piece != challanger)
            {
                List<Vector3Int> tempPath = this.GetPath(challanger, piece);
                if (tempPath != null)
                {
                    int newPathDist = this.GetPath(challanger, piece).Count;
                    if (newPathDist < pathDist)
                    {
                        if (piece.allegiance != challanger.allegiance || piece.allegiance == PieceBase.Team.independant)
                        {
                            retres = piece;
                            pathDist = newPathDist;
                        }
                    }
                }
            }
        }
        return retres;
    }

    #endregion Piece Management

    #region Tile Based Methods

    /// <summary>
    /// Returns the number of steps to a target location
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public int GetNumStepsToTarget(Vector3Int orig, Vector3Int target)
    {
        List<Vector3Int> path = AStarGridSearch.GetPath(this.fieldTilemap, orig, target);
        return path.Count;
    }


    /// <summary>
    /// Returns the number of steps to a target location
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public int GetNumStepsToTarget(Vector3 orig, Vector3 target)
    {
        return GetNumStepsToTarget(fieldTilemap.WorldToCell(orig), fieldTilemap.WorldToCell(target));
    }




    /// <summary>
    /// Gets the neighbouring tiles of given till cell
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public List<Vector3Int> GetNeighbours(Vector3Int cell)
    {
        int[] x = new int[] { -1, 0, 1, 0 };
        int[] y = new int[] { 0, 1, 0, -1 };
        List<Vector3Int> neighouringCells = new List<Vector3Int>();
        for (int i = 0; i < 4; i++)
        {
            if (fieldTilemap.GetTile(cell + new Vector3Int(x[i], y[i], 0))) ;
            neighouringCells.Add(cell + new Vector3Int(x[i], y[i], 0));
        }

        return neighouringCells;
    }


    /// <summary>
    /// Returns true if the tile under the mouse is the given tile
    /// </summary>
    /// <returns></returns>
    public bool IsTileUnderMouse(TileBase tile)
    {
        return fieldTilemap.GetTile(fieldTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition))) == tile;
    }
    /// <summary>
    /// Returns true if a tile is under the mouse
    /// </summary>
    /// <returns></returns>
    public bool IsTileUnderMouse()
    {
        return !(fieldTilemap.GetTile(fieldTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition))) == null);
    }

    #endregion Tile Based Methods


    #region Indication Map Methods

    /// <summary>
    /// Sets the indicators for where enemies will spawn
    /// </summary>
    /// <param name="positions"></param>
    public void IndicateAtTilePositions(List<Vector3Int> positions)
    {
        foreach (Vector3Int previousPos in _indicatedTiles)
        {
            indicatorMap.SetTile(previousPos, null);
        }

        _indicatedTiles.Clear();

        foreach (Vector3Int previousPos in positions)
        {
            indicatorMap.SetTile(previousPos, enemySpawnIndicatorTile);
        }

        _indicatedTiles = positions;
    }

    #endregion Indication Map Methods

    /// <summary>
    /// Draw markers for scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_fieldSize.position, _fieldSize.size);
        foreach (Vector3Int pos in tileCenterPosGizmos)
        {
            Gizmos.DrawSphere(fieldTilemap.GetCellCenterLocal(pos), 0.5f);
        }
    }
}

public static class AStarGridSearch
{
    /// <summary>
    /// Get a path on given map from start cell to end cell, concidering white list tiles
    /// </summary>
    /// <param name="map"> A tile map with tile positions</param>
    /// <param name="startCell">Start position</param>
    /// <param name="endCell">End position</param>
    /// <param name="whiteListTiles">A list of tiles the search only allows</param>
    /// <returns></returns>
    public static List<Vector3Int> GetPath(Tilemap map, Vector3Int startCell, Vector3Int endCell, List<TileBase> whiteListTiles, List<Vector3Int> blockedPositions)
    {
        Heap<CellValuePairNode> openList = new Heap<CellValuePairNode>(map.size.sqrMagnitude);
        List<Vector3Int> searchedPositions = new List<Vector3Int>();

        openList.Add(new CellValuePairNode(startCell));

        bool destinationReached = false;


        while (!destinationReached)
        {
            if (openList.Count == 0)
            {
                return null;
            }
            CellValuePairNode current = openList.RemoveFirst();

            searchedPositions.Add(current.Cell);

            if (current.Cell == endCell)
            {
                return ReconstructPath(current);
            }

            foreach (CellValuePairNode neighbour in GetIsometricNeighbours(current.Cell, map))
            {
                if (searchedPositions.Contains(neighbour.Cell) || blockedPositions.Contains(neighbour.Cell))
                {
                    continue;
                }

                float newMoveCost = current.GCost + EuclideanDistance(current.Cell, neighbour.Cell);
                if (newMoveCost < neighbour.GCost || !openList.Contains(neighbour))
                {
                    neighbour.GCost = newMoveCost;
                    neighbour.HCost = EuclideanDistance(neighbour.Cell, endCell);
                    neighbour.Parent = current;
                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Get a path on given map from start cell to end cell
    /// </summary>
    /// <param name="map"> A tile map with tile positions</param>
    /// <param name="startCell">Start position</param>
    /// <param name="endCell">End position</param>
    /// <returns></returns>
    public static List<Vector3Int> GetPath(Tilemap map, Vector3Int startCell, Vector3Int endCell)
    {
        return GetPath(map, startCell, endCell, new List<TileBase>(), new List<Vector3Int>());
    }
    /// <summary>
    /// Get a path on given map from start cell to end cell
    /// </summary>
    /// <param name="map"> A tile map with tile positions</param>
    /// <param name="startCell">Start position</param>
    /// <param name="endCell">End position</param>
    /// <returns></returns>
    public static List<Vector3Int> GetPath(Tilemap map, Vector3Int startCell, Vector3Int endCell, List<Vector3Int> blockedPositions)
    {
        return GetPath(map, startCell, endCell, new List<TileBase>(), blockedPositions);
    }


    /// <summary>
    /// Reconstucts the path found from end node to start node
    /// </summary>
    /// <param name="endNode">The final node in A* search query</param>
    /// <returns></returns>
    public static List<Vector3Int> ReconstructPath(CellValuePairNode endNode)
    {
        LinkedList<Vector3Int> path = new LinkedList<Vector3Int>();
        CellValuePairNode currentNode = endNode;
        while (currentNode.Parent != null)
        {
            path.AddFirst(currentNode.Cell);
            currentNode = currentNode.Parent;
        }

        return path.ToList();
    }


    /// <summary>
    /// Gets the neighbouring tiles of given tile cell in hex map
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public static List<CellValuePairNode> GetHexNeighbours(Vector3Int cell, Tilemap map)
    {
        int[] x, y;
        if (cell.y % 2 == 1)
        {
            x = new int[] { 0, 1, -1, 1, 0, 1 };
            y = new int[] { -1, -1, 0, 0, 1, 1 };
        }
        else
        {
            x = new int[] { 0, -1, -1, 1, 0, -1 };
            y = new int[] { -1, -1, 0, 0, 1, 1 };
        }


        List<CellValuePairNode> neighouringCells = new List<CellValuePairNode>();
        for (int i = 0; i < 6; i++)
        {
            if (map.GetTile(cell + new Vector3Int(x[i], y[i], 0)) != null)
                neighouringCells.Add(new CellValuePairNode(cell + new Vector3Int(x[i], y[i], 0)));
        }

        return neighouringCells;
    }


    /// <summary>
    /// Gets the neighbouring tiles of given tile cell in isometric map
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public static List<CellValuePairNode> GetIsometricNeighbours(Vector3Int cell, Tilemap map)
    {
        int[] x = new int[] { -1, 0, 1, 0 };
        int[] y = new int[] { 0, 1, 0, -1 };

        List<CellValuePairNode> neighouringCells = new List<CellValuePairNode>();
        for (int i = 0; i < 4; i++)
        {
            if (map.GetTile(cell + new Vector3Int(x[i], y[i], 0)) != null)
                neighouringCells.Add(new CellValuePairNode(cell + new Vector3Int(x[i], y[i], 0)));
        }

        return neighouringCells;
    }


    /// <summary>
    /// implementation for floating-point  Manhattan Distance
    /// </summary>
    /// <param name="cell1"></param>
    /// <param name="cell2"></param>
    /// <returns></returns>
    public static float ManhattanDistance(Vector3Int cell1, Vector3Int cell2)
    {
        return Mathf.Abs(cell1.x - cell2.x) + Mathf.Abs(cell1.y - cell2.y);
    }

    /// <summary>
    /// Gets the EuclideanDistance between two points
    /// </summary>
    /// <param name="cell1">first point</param>
    /// <param name="cell2">second point</param>
    /// <returns></returns>
    public static float EuclideanDistance(Vector3Int cell1, Vector3Int cell2)
    {
        float square = (cell1.x - cell2.x) * (cell1.x - cell2.x) + (cell1.y - cell2.y) * (cell1.y - cell2.y);
        return Mathf.Sqrt(square);
    }

    /// <summary>
    /// implementation for floating-point Chebyshev Distance
    /// </summary>
    /// <param name="cell1"></param>
    /// <param name="cell2"></param>
    /// <returns></returns>
    public static float ChebyshevDistance(Vector3Int cell1, Vector3Int cell2)
    {
        // not quite sure if the math is correct here
        return Mathf.Max(Mathf.Abs(cell2.x - cell1.x), Mathf.Abs(cell2.y - cell1.y));
    }
}


/// <summary>
/// Cell Value pair for A* algorithym
/// </summary>
public class CellValuePairNode : IHeapItem<CellValuePairNode>
{
    private CellValuePairNode _parent;
    private Vector3Int _cell;
    private float _gCost;
    private float _hCost;

    private int _heapIndex;

    #region Properties
    public CellValuePairNode(Vector3Int cell)
    {
        _cell = cell;
    }

    public CellValuePairNode Parent
    {
        get { return _parent; }
        set { _parent = value; }
    }
    public float FCost
    {
        get { return _gCost + _hCost; }
    }

    public float GCost
    {
        get { return _gCost; }
        set { _gCost = value; }
    }
    public float HCost
    {
        get { return _hCost; }
        set { _hCost = value; }
    }

    public Vector3Int Cell
    {
        get { return _cell; }
    }

    public int HeapIndex
    {
        get { return _heapIndex; }
        set { _heapIndex = value; }
    }

    #endregion Properties

    #region Comparator Methods
    public int CompareTo(CellValuePairNode nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(nodeToCompare.HCost);
        }
        return -compare;
    }

    public new bool Equals(object x, object y)
    {
        return (GetHashCode(x) == GetHashCode(y));
    }

    public int GetHashCode(object obj)
    {
        CellValuePairNode pair = (CellValuePairNode)obj;
        return (int)((pair.Cell.x * 100 * 13) + (pair.Cell.y * 100 * 17) + (pair.FCost * 41));
    }
    #endregion Comparator Methods
}