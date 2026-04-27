using Game;
using Game.Configs;

public class PowerUpBehaviourFactory
{
    readonly IVfxService _vfxService;

    public PowerUpBehaviourFactory(IVfxService vfxService)
    {
        _vfxService = vfxService;
    }

    public IPowerUpBehaviour CreateRocket(RocketOrientation orientation)
        => new RocketBehaviour(orientation, _vfxService);

    public IPowerUpBehaviour CreateBomb(BombItemConfig config)
        => new BombBehaviour(config.ExplosionRadius, _vfxService);

    public IPowerUpBehaviour CreatePlane(PlaneItemConfig config)
        => new PlaneBehaviour(config.FlySpeed, config.TargetsCount, _vfxService);

    public IPowerUpBehaviour CreateDisco(DiscoItemConfig config)
        => new DiscoBehaviour(config.BeamSpeed ,_vfxService);
}