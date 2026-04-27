using Game;
using UnityEngine;
using VContainer;

public class DebugTools : MonoBehaviour
{
    [SerializeField] private float _initialGameSpeed = 1f;
    private GameManager _gameManager;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
        _gameManager.SetSpeed(_initialGameSpeed);
    }

    [ContextMenu("Set 1x speed")]
    public void Set1Speed()
    {
        _gameManager.SetSpeed(1f);
    }

    [ContextMenu("Set 0.5x speed")]
    public void Set05Speed()
    {
        _gameManager.SetSpeed(0.5f);
    }

    [ContextMenu("Set 0.3x speed")]
    public void Set03Speed()
    {
        _gameManager.SetSpeed(0.3f);
    }

    [ContextMenu("Set 0.1x speed")]
    public void Set01Speed()
    {
        _gameManager.SetSpeed(0.1f);
    }

    [ContextMenu("Set 0.05x speed")]
    public void Set005Speed()
    {
        _gameManager.SetSpeed(0.05f);
    }
}