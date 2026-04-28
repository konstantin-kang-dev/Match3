using UnityEngine;

namespace Game
{
    public readonly struct CellMovement
    {
        public readonly PlayfieldItem Item;
        public readonly Vector2Int FromCell;
        public readonly Vector2Int ToCell;
        public readonly bool IsNew;

        public CellMovement(PlayfieldItem item, Vector2Int fromCell, Vector2Int toCell, bool isNew)
        {
            Item = item;
            FromCell = fromCell;
            ToCell = toCell;
            IsNew = isNew;
        }
    }

    public readonly struct ExplosionReaction
    {
        public readonly PlayfieldItem Item;
        public readonly Vector2 Direction;
        public readonly float Force;

        public ExplosionReaction(PlayfieldItem item, Vector2 direction, float force)
        {
            Item = item;
            Direction = direction;
            Force = force;
        }
    }
    public readonly struct FallStartedEvent
    {
        public readonly PlayfieldItem Item;
        public readonly Vector2Int FromCell;
        public readonly Vector2Int ToCell;

        public FallStartedEvent(PlayfieldItem item, Vector2Int fromCell, Vector2Int toCell)
        {
            Item = item;
            FromCell = fromCell;
            ToCell = toCell;
        }
    }
}