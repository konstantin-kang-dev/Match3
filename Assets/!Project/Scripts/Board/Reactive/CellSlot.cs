using R3;
using UnityEngine;

namespace Game
{
    public class CellSlot
    {
        public Vector2Int Position { get; }
        public CellState State { get; private set; }
        public IBoardItem Item { get; private set; }

        readonly Subject<CellSlot> _onStateChanged = new();
        public Observable<CellSlot> OnStateChanged => _onStateChanged.AsObservable();
        readonly Subject<FallStartedEvent> _onFallStarted = new();
        public Observable<FallStartedEvent> OnFallStarted => _onFallStarted.AsObservable();

        public CellSlot(Vector2Int position)
        {
            Position = position;
            State = CellState.Empty;
        }

        public void SetFalling(IBoardItem item, Vector2Int sourceCell)
        {
            Item = item;
            if (State != CellState.Falling)
            {
                State = CellState.Falling;
                _onFallStarted.OnNext(new FallStartedEvent(item, sourceCell, Position));
                _onStateChanged.OnNext(this);
            }
            else
            {
                _onFallStarted.OnNext(new FallStartedEvent(item, sourceCell, Position));
            }
        }

        public void SetOccupied(IBoardItem item)
        {
            Item = item;
            ChangeState(CellState.Occupied);
        }

        public void SetDestroying()
        {
            ChangeState(CellState.Destroying);
        }

        public void SetEmpty()
        {
            Item = null;
            ChangeState(CellState.Empty);
        }

        internal void ClearItem()
        {
            Item = null;
        }

        internal void SwapItemSilently(CellSlot other)
        {
            var tmp = Item;
            Item = other.Item;
            other.Item = tmp;
        }

        void ChangeState(CellState newState)
        {
            if (State == newState) return;
            State = newState;
            _onStateChanged.OnNext(this);
        }
    }
}