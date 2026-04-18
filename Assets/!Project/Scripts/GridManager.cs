using UnityEngine;

public class GridManager : MonoBehaviour
{
    [field: SerializeField] public Vector2 CellSize {  get; private set; }
    [field: SerializeField] public Vector2Int GridSize { get; private set; }
    [field: SerializeField] public Canvas Canvas { get; private set; }

    public Vector2 GetPositionForCell(Vector2Int cell)
    {
        return new Vector2(cell.x * CellSize.x + (CellSize.x / 2f), cell.y * CellSize.y + (CellSize.y / 2f));
    }
}
