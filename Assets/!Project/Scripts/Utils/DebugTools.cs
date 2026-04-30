using Game;
using UnityEngine;
using VContainer;

public class DebugTools : MonoBehaviour
{
    [Header("Game speed")]
    [SerializeField] private float _initialGameSpeed = 1f;

    [Header("Gizmos")]
    [SerializeField] private bool _drawCellStates = true;
    [SerializeField] private bool _drawCellLabels = true;
    [SerializeField] private bool _drawItemKind = true;
    [SerializeField, Range(0f, 1f)] private float _gizmoAlpha = 0.35f;

    private GameManager _gameManager;
    private BoardMutator _boardMutator;
    private IBoardContext _boardContext;
    private BoardState _board;
    private GridManager _gridManager;
    private BoardActivityTracker _tracker;

    [Inject]
    public void Construct(
        GameManager gameManager,
        BoardMutator boardMutator,
        IBoardContext boardContext,
        BoardState board,
        GridManager gridManager,
        BoardActivityTracker tracker)
    {
        _gameManager = gameManager;
        _boardMutator = boardMutator;
        _boardContext = boardContext;
        _board = board;
        _gridManager = gridManager;
        _tracker = tracker;

        _gameManager.SetSpeed(_initialGameSpeed);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (_board == null || _gridManager == null) return;
        if (!_drawCellStates) return;

        var container = _gridManager.PlayfieldItemsContainer.GetComponent<RectTransform>();
        if (container == null) return;

        Vector2 cellSize = _gridManager.CellSize;

        foreach (var slot in _board.AllCells())
        {
            Vector2 anchoredPos = _gridManager.GetPositionForCell(slot.Position);

            var rectSize = container.rect.size;
            var pivot = container.pivot;
            Vector3 localPos = new Vector3(
                anchoredPos.x - rectSize.x * pivot.x,
                anchoredPos.y - rectSize.y * pivot.y,
                0f);
            Vector3 worldPos = container.TransformPoint(localPos);

            Color c = GetColorForState(slot.State);
            c.a = _gizmoAlpha;
            Gizmos.color = c;

            Vector3 size = new Vector3(
                cellSize.x * container.lossyScale.x,
                cellSize.y * container.lossyScale.y,
                0.01f);

            Gizmos.DrawCube(worldPos, size * 0.95f);

            Gizmos.color = new Color(c.r, c.g, c.b, 1f);
            Gizmos.DrawWireCube(worldPos, size * 0.95f);

#if UNITY_EDITOR
            if (_drawCellLabels)
            {
                string label = $"{slot.Position}\n{slot.State}";
                if (_drawItemKind && slot.Item != null)
                {
                    string activating = slot.Item.IsActivating ? "*" : "";
                    label += $"\n{slot.Item.Kind}{activating}";
                }
                UnityEditor.Handles.Label(worldPos, label);
            }
#endif
        }

#if UNITY_EDITOR
        var trackerLabel = $"Idle: {_tracker.IsIdle} | Frozen: {_tracker.IsFrozen}";
        var corner = _gridManager.GetPositionForCell(new Vector2Int(0, _board.Size.y));
        Vector3 worldCorner = container.TransformPoint(corner);
        UnityEditor.Handles.Label(worldCorner + Vector3.up * 0.5f, trackerLabel);
#endif
    }

    static Color GetColorForState(CellState state) => state switch
    {
        CellState.Empty      => Color.gray,
        CellState.Occupied   => Color.green,
        CellState.Falling    => Color.yellow,
        CellState.Destroying => Color.red,
        _ => Color.magenta
    };

    [ContextMenu("Set 1x speed")]
    public void Set1Speed() => _gameManager.SetSpeed(1f);

    [ContextMenu("Set 0.1x speed")]
    public void Set01Speed() => _gameManager.SetSpeed(0.1f);

    [ContextMenu("Spawn Horizontal Rocket")]
    public void SpawnHorizontalRocket()
    {
        var cell = _boardContext.FindRandomColoredCell();
        if (cell.HasValue) _boardMutator.SpawnRocketAt(cell.Value, RocketOrientation.Horizontal);
    }

    [ContextMenu("Spawn Vertical Rocket")]
    public void SpawnVerticalRocket()
    {
        var cell = _boardContext.FindRandomColoredCell();
        if (cell.HasValue) _boardMutator.SpawnRocketAt(cell.Value, RocketOrientation.Vertical);
    }

    [ContextMenu("Spawn Bomb")]
    public void SpawnBomb()
    {
        var cell = _boardContext.FindRandomColoredCell();
        if (cell.HasValue) _boardMutator.SpawnBombAt(cell.Value);
    }

    [ContextMenu("Spawn Plane")]
    public void SpawnPlane()
    {
        var cell = _boardContext.FindRandomColoredCell();
        if (cell.HasValue) _boardMutator.SpawnPlaneAt(cell.Value);
    }

    [ContextMenu("Spawn Disco")]
    public void SpawnDisco()
    {
        var cell = _boardContext.FindRandomColoredCell();
        if (cell.HasValue) _boardMutator.SpawnDiscoAt(cell.Value);
    }
}