using UnityEngine;

namespace Game
{
    public readonly struct ActivationContext
    {
        public readonly Vector2Int Origin;
        public readonly IBoardItem Self;
        public readonly PlayfieldItemColorType? SwappedColor;

        public ActivationContext(Vector2Int origin, IBoardItem self, PlayfieldItemColorType? swappedColor = null)
        {
            Origin = origin;
            Self = self;
            SwappedColor = swappedColor;
        }
    }
}