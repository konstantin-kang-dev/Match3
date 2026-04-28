namespace Game
{
    public enum PlayfieldVfxType
    {
        // Match (0-99)
        MatchDestroy = 0,
        
        // Rocket (100-199)
        RocketSpawn = 100,
        RocketActivateHorizontal = 101,
        RocketActivateVertical = 102,
        RocketBeam = 103,

        // Bomb (200-299)
        BombSpawn = 200,
        BombActivate = 201,

        // Plane (300-399)
        PlaneSpawn = 300,
        PlaneTakeoff = 301,
        PlaneFly = 302,
        PlaneImpact = 303,

        // Disco (400-499)
        DiscoSpawn = 400,
        DiscoActivate = 401,
        DiscoBeam = 402,

        // Combos (500-599)
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
        
        // Others (600-699)
        PowerUpSpawn = 600,
    }
}