using Game;
using UnityEngine;
using VContainer;

public class DebugTools : MonoBehaviour
{
    [SerializeField] private float _initialGameSpeed = 1f;
    private GameManager _gameManager;
    private BoardMutator _boardMutator;
    private IBoardContext _boardContext;
    [Inject]
    public void Construct(GameManager gameManager, BoardMutator boardMutator, IBoardContext boardContext)
    {
        _gameManager = gameManager;
        _boardMutator = boardMutator;
        _boardContext = boardContext;
        
        _gameManager.SetSpeed(_initialGameSpeed);
    }

    [ContextMenu("Set 1x speed")]
    public void Set1Speed()
    {
        _gameManager.SetSpeed(1f);
    }

    [ContextMenu("Set 0.1x speed")]
    public void Set01Speed()
    {
        _gameManager.SetSpeed(0.1f);
    }

    [ContextMenu("Spawn Horizontal Rocket")]
    public void SpawnHorizontalRocket()
    {
        Vector2Int randomCell = _boardContext.FindRandomColoredCell().Value;
        _boardMutator.SpawnRocketAt(randomCell, RocketOrientation.Horizontal);
    }
    [ContextMenu("Spawn Vertical Rocket")]
    public void SpawnVerticalRocket()
    {
        Vector2Int randomCell = _boardContext.FindRandomColoredCell().Value;
        _boardMutator.SpawnRocketAt(randomCell, RocketOrientation.Vertical);
    }
    [ContextMenu("Spawn Bomb")]
    public void SpawnBomb()
    {
        Vector2Int randomCell = _boardContext.FindRandomColoredCell().Value;
        _boardMutator.SpawnBombAt(randomCell);
    }
    [ContextMenu("Spawn Plane")]
    public void SpawnPlane()
    {
        Vector2Int randomCell = _boardContext.FindRandomColoredCell().Value;
        _boardMutator.SpawnPlaneAt(randomCell);
    }
    [ContextMenu("Spawn Disco")]
    public void SpawnDisco()
    {
        Vector2Int randomCell = _boardContext.FindRandomColoredCell().Value;
        _boardMutator.SpawnDiscoAt(randomCell);
    }
}