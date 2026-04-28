using UnityEngine;

namespace Game
{
    public readonly struct ActivationContext
    {
        public readonly Vector2Int Origin;
        public readonly PlayfieldItem Self;
        public readonly PlayfieldItemColorType? SwappedColor;

        public ActivationContext(Vector2Int origin, PlayfieldItem self, PlayfieldItemColorType? swappedColor = null)
        {
            Origin = origin;
            Self = self;
            SwappedColor = swappedColor;
        }
    }
}