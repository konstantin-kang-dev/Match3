using R3;
using System;
using System.Collections.Generic;
using VContainer.Unity;

namespace Game
{
    public class ColumnsCoordinator : IStartable, IDisposable
    {
        readonly BoardState _board;
        readonly BoardActivityTracker _tracker;
        readonly List<ColumnSimulator> _columns = new();
        readonly CompositeDisposable _disposables = new();

        public ColumnsCoordinator(BoardState board, BoardActivityTracker tracker)
        {
            _board = board;
            _tracker = tracker;
        }

        public void Start()
        {
            for (int x = 0; x < _board.Size.x; x++)
            {
                var slots = new List<CellSlot>();
                foreach (var slot in _board.GetColumn(x))
                    slots.Add(slot);

                var sim = new ColumnSimulator(x, slots, _tracker);
                _columns.Add(sim);
            }

            _tracker.OnUnfrozen
                .Subscribe(_ => ResumeAllColumns())
                .AddTo(_disposables);
        }

        void ResumeAllColumns()
        {
            foreach (var col in _columns)
                col.ResumeIfNeeded();
        }

        public void Dispose()
        {
            foreach (var col in _columns) col.Dispose();
            _disposables.Dispose();
        }
    }
}