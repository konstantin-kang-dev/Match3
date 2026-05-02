using R3;
using UnityEngine;

namespace Game
{
    public interface IBoardItemColor
    {
        PlayfieldItemColorType? Color { get; }
    }

    public interface IBoardItem : IBoardItemColor
    {
        PlayfieldItemKind Kind { get; }
        bool IsPowerUp { get; }
        bool IsDisposed { get; }
        bool IsActivating { get; }
        IPowerUpBehaviour PowerUp { get; }
        Vector2Int OccupiedCell { get; }
        Observable<bool> OnDestroyed { get; }
        void DestroyItem(DestroyMode mode);
        void SetActivating(bool value);
    }
}