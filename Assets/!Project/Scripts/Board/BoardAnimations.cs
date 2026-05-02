using UnityEngine;

namespace Game
{
    public readonly struct CellMovement
    {
        public readonly IBoardItem Item;
        public readonly Vector2Int FromCell;
        public readonly Vector2Int ToCell;
        public readonly bool IsNew;

        public CellMovement(IBoardItem item, Vector2Int fromCell, Vector2Int toCell, bool isNew)
        {
            Item = item;
            FromCell = fromCell;
            ToCell = toCell;
            IsNew = isNew;
        }
    }

    public readonly struct ExplosionReaction
    {
        public readonly IBoardItem Item;
        public readonly Vector2 Direction;
        public readonly float Force;

        public ExplosionReaction(IBoardItem item, Vector2 direction, float force)
        {
            Item = item;
            Direction = direction;
            Force = force;
        }
    }

    public readonly struct FallStartedEvent
    {
        public readonly IBoardItem Item;
        public readonly Vector2Int FromCell;
        public readonly Vector2Int ToCell;

        public FallStartedEvent(IBoardItem item, Vector2Int fromCell, Vector2Int toCell)
        {
            Item = item;
            FromCell = fromCell;
            ToCell = toCell;
        }
    }
}