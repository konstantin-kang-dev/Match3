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
        // Scene refs
        builder.RegisterInstance(_gridManager);
        builder.RegisterInstance(_playfieldItemsDB);
        builder.RegisterInstance(_playfieldVfxDB);

        // Top-level managers
        builder.Register<GameManager>(Lifetime.Singleton);
        builder.Register<PlayerProgressionManager>(Lifetime.Singleton);

        // Item factories
        builder.Register<PowerUpBehaviourFactory>(Lifetime.Singleton);
        builder.Register<PlayfieldItemsFactory>(Lifetime.Singleton);
        builder.Register<PlayfieldItem>(Lifetime.Transient);

        // Playfield orchestrator
        builder.Register<PlayfieldManager>(Lifetime.Singleton)
            .AsSelf()
            .As<ISwapRequester>();

        // Reactive board state — фабрика, т.к. зависит от runtime GridSize
        builder.Register(resolver =>
                new BoardState(resolver.Resolve<GridManager>().GridSize),
            Lifetime.Singleton);

        // Activity / freeze tracker — единая точка состояния доски
        builder.Register<BoardActivityTracker>(Lifetime.Singleton);

        // Board services
        builder.Register<MatchDetector>(Lifetime.Singleton);
        builder.Register<BoardMutator>(Lifetime.Singleton);
        builder.Register<BoardContext>(Lifetime.Singleton).As<IBoardContext>();
        builder.Register<MatchResolver>(Lifetime.Singleton);

        // Reactive simulation entry points — порядок важен:
        // ColumnsCoordinator подписывается первым (реакция на Empty),
        // MatchScanner — последним (реакция на Occupied), чтобы матчи
        // обрабатывались после решений колонок.
        builder.RegisterEntryPoint<ColumnsCoordinator>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<FallAnimator>(Lifetime.Singleton);
        builder.RegisterEntryPoint<RefillSpawner>(Lifetime.Singleton);
        builder.RegisterEntryPoint<MatchScanner>(Lifetime.Singleton);

        // Vfx & PowerUp animation
        builder.Register<PlayfieldVfxService>(Lifetime.Singleton).As<IVfxService>();
        builder.Register<PowerUpAnimator>(Lifetime.Singleton);

        // UI
        builder.RegisterComponentInHierarchy<HUDRoot>();
        builder.Register(resolver => resolver.Resolve<HUDRoot>().PlayerLevelUIView, Lifetime.Singleton);
        builder.RegisterEntryPoint<PlayerLevelUIPresenter>();
    }
}