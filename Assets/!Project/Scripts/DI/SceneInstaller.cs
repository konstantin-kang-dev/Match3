using Game;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SceneInstaller : LifetimeScope
{
    [SerializeField] private PlayfieldItemsDB _playfieldItemsDB;
    [SerializeField] private PlayfieldVfxDB _playfieldVfxDB;
    [SerializeField] private GridManager _gridManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_gridManager);
        builder.RegisterInstance(_playfieldItemsDB);
        builder.RegisterInstance(_playfieldVfxDB);
        builder.Register<GameManager>(Lifetime.Singleton);
        
        builder.Register<PowerUpBehaviourFactory>(Lifetime.Singleton);
        builder.Register<PlayfieldItemsFactory>(Lifetime.Singleton);
        
        builder.Register<PlayerProgressionManager>(Lifetime.Singleton);
        builder.Register<PlayfieldItem>(Lifetime.Transient);

        builder.Register<PlayfieldManager>(Lifetime.Singleton)
            .AsSelf()
            .As<ISwapRequester>();
        
        builder.Register(resolver =>
                new PlayfieldBoard(resolver.Resolve<GridManager>().GridSize),
            Lifetime.Singleton).As<IBoard>();

        builder.Register<MatchDetector>(Lifetime.Singleton);
        builder.Register<BoardMutator>(Lifetime.Singleton);
        builder.Register<BoardContext>(Lifetime.Singleton).As<IBoardContext>();
        builder.Register<MatchResolver>(Lifetime.Singleton);
        builder.Register<BoardCollapser>(Lifetime.Singleton);
        builder.Register<BoardFiller>(Lifetime.Singleton);
        builder.Register<PlayfieldAnimator>(Lifetime.Singleton);
        builder.Register<PlayfieldVfxService>(Lifetime.Singleton).As<IVfxService>();
        builder.Register<PowerUpAnimator>(Lifetime.Singleton);
        
        builder.RegisterComponentInHierarchy<HUDRoot>();
        builder.Register(resolver => resolver.Resolve<HUDRoot>().PlayerLevelUIView, Lifetime.Singleton);
        builder.RegisterEntryPoint<PlayerLevelUIPresenter>();
    }
}