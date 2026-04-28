using Game;
using Game.Configs;

public class PowerUpBehaviourFactory
{
    readonly IVfxService _vfxService;
    readonly PowerUpAnimator _animator;
    
    public PowerUpBehaviourFactory(IVfxService vfxService, PowerUpAnimator animator)
    {
        _vfxService = vfxService;
        _animator = animator;
    }

    public IPowerUpBehaviour CreateRocket(RocketOrientation orientation)
        => new RocketBehaviour(orientation, _vfxService, _animator);

    public IPowerUpBehaviour CreateBomb(BombItemConfig config)
        => new BombBehaviour(config.ExplosionRadius, _vfxService, _animator);

    public IPowerUpBehaviour CreatePlane(PlaneItemConfig config)
        => new PlaneBehaviour(config.FlySpeed, config.TargetsCount, _vfxService, _animator);

    public IPowerUpBehaviour CreateDisco(DiscoItemConfig config)
        => new DiscoBehaviour(config.BeamSpeed ,_vfxService, _animator);
}