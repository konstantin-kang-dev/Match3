namespace Game
{
    public enum PlayfieldVfxType
    {
        
        MatchDestroy = 0,
        MatchRedDestroy = 1,
        MatchGreenDestroy = 2,
        MatchYellowDestroy = 3,
        MatchPinkDestroy = 4,
        
        
        RocketSpawn = 100,
        RocketActivateHorizontal = 101,
        RocketActivateVertical = 102,
        RocketBeam = 103,

        
        BombSpawn = 200,
        BombActivate = 201,

        
        BalloonSpawn = 300,
        BalloonTakeoff = 301,
        BalloonFly = 302,
        BalloonImpact = 303,

        
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