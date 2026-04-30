using Game;
using Game.Configs;

public class PowerUpBehaviourFactory
{
    private readonly IVfxService _vfxService;
    private readonly PowerUpAnimator _animator;
    private readonly BoardActivityTracker _activityTracker;
    public PowerUpBehaviourFactory(IVfxService vfxService, PowerUpAnimator animator, BoardActivityTracker activityTracker)
    {
        _vfxService = vfxService;
        _animator = animator;
        _activityTracker = activityTracker;
    }

    public IPowerUpBehaviour CreateRocket(RocketOrientation orientation)
        => new RocketBehaviour(orientation, _vfxService, _animator);

    public IPowerUpBehaviour CreateBomb(BombItemConfig config)
        => new BombBehaviour(config.ExplosionRadius, _vfxService, _animator);

    public IPowerUpBehaviour CreatePlane(BalloonItemConfig config)
        => new BalloonBehaviour(
            config.FlySpeed,
            config.TargetsCount,
            _vfxService,
            _animator,
            _activityTracker);

    public IPowerUpBehaviour CreateDisco(DiscoItemConfig config)
        => new DiscoBehaviour(config.BeamSpeed ,_vfxService, _animator);
}