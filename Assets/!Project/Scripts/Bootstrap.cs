using Game;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VContainer;

public class Bootstrap: MonoBehaviour
{
    GameManager _gameManager;
    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    private void Start()
    {
        Boot();
    }

    void Boot()
    {
        Debug.Log($"[Bootstrap] Booted successfully!");
        _gameManager.Init();
    }
}
