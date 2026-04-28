using R3;
using System;
using System.Collections.Generic;

namespace Game
{
    public class ColumnsCoordinator : IDisposable
    {
        readonly List<ColumnSimulator> _columns = new();
        readonly CompositeDisposable _disposables = new();

        readonly Subject<int> _onColumnNeedsRefill = new();
        public Observable<int> OnColumnNeedsRefill => _onColumnNeedsRefill.AsObservable();

        public ColumnsCoordinator(BoardState board)
        {
            for (int x = 0; x < board.Size.x; x++)
            {
                var slots = new List<CellSlot>();
                foreach (var slot in board.GetColumn(x))
                    slots.Add(slot);

                var sim = new ColumnSimulator(x, slots);
                sim.OnNeedsRefill
                    .Subscribe(idx => _onColumnNeedsRefill.OnNext(idx))
                    .AddTo(_disposables);

                _columns.Add(sim);
            }
        }

        public void Dispose()
        {
            foreach (var col in _columns) col.Dispose();
            _disposables.Dispose();
            _onColumnNeedsRefill.Dispose();
        }
    }
}