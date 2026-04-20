using Game;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SceneInstaller: LifetimeScope
{
    [SerializeField] PlayfieldItemsDB _playfieldItemsContentManager;
    [SerializeField] GridManager _gridManager;
    [SerializeField] PlayfieldItemView _playfieldItemVisualsPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_gridManager);
        builder.RegisterInstance(_playfieldItemsContentManager);
        builder.RegisterInstance(_playfieldItemVisualsPrefab);
        builder.Register<GameManager>(Lifetime.Singleton);
        builder.Register<PlayfieldManager>(Lifetime.Singleton);
        builder.Register<PlayfieldItemsFactory>(Lifetime.Singleton);
        builder.Register<PlayfieldItemPresenter>(Lifetime.Transient);
    }
}
