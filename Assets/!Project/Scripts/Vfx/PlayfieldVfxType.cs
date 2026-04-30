namespace Game
{
    public enum PlayfieldVfxType
    {
        
        MatchDestroy = 0,
        
        
        RocketSpawn = 100,
        RocketActivateHorizontal = 101,
        RocketActivateVertical = 102,
        RocketBeam = 103,

        
        BombSpawn = 200,
        BombActivate = 201,

        
        PlaneSpawn = 300,
        PlaneTakeoff = 301,
        PlaneFly = 302,
        PlaneImpact = 303,

        
        DiscoSpawn = 400,
        DiscoActivate = 401,
        DiscoBeam = 402,

        
        ComboRocketRocket = 500,
        ComboRocketBomb = 501,
        ComboRocketPlane = 502,
        ComboRocketDisco = 503,
        ComboBombBomb = 504,
        ComboBombPlane = 505,
        ComboBombDisco = 506,
        ComboPlanePlane = 507,
        ComboPlaneDisco = 508,
        ComboDiscoDisco = 509,
        
        
        PowerUpSpawn = 600,
    }
}