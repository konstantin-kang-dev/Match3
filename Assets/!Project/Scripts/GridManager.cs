using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [field: SerializeField] public Vector2 CellSize { get; private set; }
    [field: SerializeField] public Vector2 Offset { get; private set; }
    [field: SerializeField] public Vector2Int GridSize { get; private set; }
    [field: SerializeField] public Canvas Canvas { get; private set; }

    [Header("Grid cells")] [field: SerializeField]
    public Transform GridCellsContainer { get; private set; }

    [SerializeField] private GridCell _cellPrefab;

    [Header("Other")]
    [field: SerializeField]
    public Transform PlayfieldItemsContainer { get; private set; }

    private readonly List<GridCell> _gridCells = new();

    private readonly HashSet<Vector2Int> _validCells = new();

    public void Init()
    {
        for (var x = 0; x < GridSize.x; x++)
        for (var y = 0; y < GridSize.y; y++)
            _validCells.Add(new Vector2Int(x, y));

        SpawnCells();
    }

    [ContextMenu("Regenerate grid cells")]
    private void SpawnCells()
    {
        foreach (var gridCell in _gridCells) Destroy(gridCell.gameObject);
        _gridCells.Clear();
        var cells = _validCells.ToList();
        for (var i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            var pos = GetPositionForCell(cell);

            var gridCell = Instantiate(_cellPrefab, GridCellsContainer);
            gridCell.transform.localPosition = pos;
            gridCell.SetSize(CellSize);
            _gridCells.Add(gridCell);
        }
    }

    public Vector2 GetPositionForCell(Vector2Int cell)
    {
        return new Vector2(cell.x * CellSize.x + CellSize.x / 2f, cell.y * CellSize.y + CellSize.y / 2f) + Offset;
    }

    public bool IsValidCell(Vector2Int cell)
    {
        return _validCells.Contains(cell);
    }
}