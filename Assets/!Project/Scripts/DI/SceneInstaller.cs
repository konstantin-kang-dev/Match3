using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] PlayfieldItemsContentManager _playfieldItemsContentManager;
    [SerializeField] GameManager _gameManager;
    [SerializeField] GridManager _gridManager;
    [SerializeField] PlayfieldManager _playfieldManager; 

    public override void InstallBindings()
    {
        Container.Bind<PlayfieldItemsContentManager>().FromInstance(_playfieldItemsContentManager).AsSingle();
        Container.Bind<GameManager>().FromInstance(_gameManager).AsSingle();
        Container.Bind<GridManager>().FromInstance(_gridManager).AsSingle();
        Container.Bind<PlayfieldManager>().FromInstance(_playfieldManager).AsSingle();

        Container.Bind<GameProgressionManager>().AsSingle();

    }
}
