using UnityEngine;

public class GridManager : MonoBehaviour
{
    [field: SerializeField] public Vector2 CellSize {  get; private set; }
    [field: SerializeField] public Vector2Int GridSize { get; private set; } 
}
