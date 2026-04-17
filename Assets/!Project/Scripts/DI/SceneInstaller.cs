using Game;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SceneInstaller: LifetimeScope
{
    [SerializeField] PlayfieldItemsContentManager _playfieldItemsContentManager;
    [SerializeField] GridManager _gridManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_gridManager);
        builder.RegisterInstance(_playfieldItemsContentManager);
        builder.Register<GameManager>(Lifetime.Singleton);
        builder.Register<PlayfieldManager>(Lifetime.Singleton);
    }
}
