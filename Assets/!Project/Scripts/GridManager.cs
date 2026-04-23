using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [field: SerializeField] public Vector2 CellSize {  get; private set; }
    [field: SerializeField] public Vector2Int GridSize { get; private set; }
    [field: SerializeField] public Canvas Canvas { get; private set; }

    [Header("Grid cells")]
    [SerializeField] Transform _gridCellsContainer;
    [SerializeField] GridCell _cellPrefab;
    List<GridCell> _gridCells = new List<GridCell>();

    [Header("Other")]
    [field: SerializeField] public Transform PlayfieldItemsContainer {  get; private set; }

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

        SpawnCells();
    }

    [ContextMenu("Regenerate grid cells")]
    void SpawnCells()
    {
        foreach (var gridCell in _gridCells)
        {
            Destroy(gridCell.gameObject);
        }
        _gridCells.Clear();
        List<Vector2Int> cells = _validCells.ToList();
        for (int i = 0; i < cells.Count; i++)
        {
            Vector2Int cell = cells[i];
            Vector2 pos = GetPositionForCell(cell);

            GridCell gridCell = Instantiate(_cellPrefab, _gridCellsContainer);
            gridCell.transform.localPosition = pos;
            gridCell.SetSize(CellSize);
            _gridCells.Add(gridCell);
        }
    }

    public Vector2 GetPositionForCell(Vector2Int cell)
    {
        return new Vector2(cell.x * CellSize.x + (CellSize.x / 2f), cell.y * CellSize.y + (CellSize.y / 2f));
    }
    public bool IsValidCell(Vector2Int cell) => _validCells.Contains(cell);
}
