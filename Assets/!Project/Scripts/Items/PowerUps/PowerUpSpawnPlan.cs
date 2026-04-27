using UnityEngine;

namespace Game
{
    public struct PowerUpSpawnPlan
    {
        public Vector2Int Cell;
        public PlayfieldItemKind Kind;
        public MatchShape SourceShape;
    }
}