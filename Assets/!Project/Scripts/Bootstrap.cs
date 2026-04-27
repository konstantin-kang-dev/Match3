using Game;
using UnityEngine;
using VContainer;

public class Bootstrap : MonoBehaviour
{
    private GameManager _gameManager;

    private void Start()
    {
        Boot();
    }

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    private void Boot()
    {
        Debug.Log("[Bootstrap] Booted successfully!");
        _gameManager.Init();
    }
}