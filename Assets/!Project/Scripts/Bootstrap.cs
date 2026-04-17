using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zenject;

public class Bootstrap: MonoBehaviour
{
    [Inject]
    GameManager _gameManager;

    private void Start()
    {
        Boot();
    }

    void Boot()
    {
        _gameManager.Init();
    }
}
