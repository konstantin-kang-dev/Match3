using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [field: SerializeField] public Vector2 CellSize {  get; private set; }
    [field: SerializeField] public Vector2Int GridSize { get; private set; }
    [field: SerializeField] public Canvas Canvas { get; private set; }

    HashSet<Vector2Int> _validCells = new HashSet<Vector2Int>();

    public void Init()
    {
        for (int x = 0; x < GridSize.x; x++)
        {
            for(int y = 0; y < GridSize.y; y++)
            {
                _validCells.Add(new Vector2Int(x, y));
            }
        }
    }

    public Vector2 GetPositionForCell(Vector2Int cell)
    {
        return new Vector2(cell.x * CellSize.x + (CellSize.x / 2f), cell.y * CellSize.y + (CellSize.y / 2f));
    }
    public bool IsValidCell(Vector2Int cell) => _validCells.Contains(cell);
}
