using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject]
    PlayfieldManager _playfieldManager;

    public void Init()
    {
        _playfieldManager.Init();
    }
}
