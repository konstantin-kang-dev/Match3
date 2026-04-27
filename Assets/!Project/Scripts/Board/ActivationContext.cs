using UnityEngine;

namespace Game
{
    public readonly struct ActivationContext
    {
        public readonly Vector2Int Origin;
        public readonly PlayfieldItemColorType? SwappedColor;

        public ActivationContext(Vector2Int origin, PlayfieldItemColorType? swappedColor = null)
        {
            Origin = origin;
            SwappedColor = swappedColor;
        }
    }
}