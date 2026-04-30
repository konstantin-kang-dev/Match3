using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ColumnSimulator : IDisposable
    {
        readonly int _columnIndex;
        readonly List<CellSlot> _slots;
        readonly BoardActivityTracker _tracker;
        readonly CompositeDisposable _disposables = new();

        bool _running;
        bool _pending;

        public ColumnSimulator(int columnIndex, List<CellSlot> slots, BoardActivityTracker tracker)
        {
            _columnIndex = columnIndex;
            _slots = slots;
            _tracker = tracker;

            foreach (var slot in _slots)
            {
                slot.OnStateChanged
                    .Subscribe(OnSlotChanged)
                    .AddTo(_disposables);
            }
        }

        void OnSlotChanged(CellSlot _)
        {
            if (_tracker.IsFrozen) return;
            RunOrSchedule();
        }

        public void ResumeIfNeeded()
        {
            if (_tracker.IsFrozen) return;
            RunOrSchedule();
        }

        void RunOrSchedule()
        {
            if (_running)
            {
                _pending = true;
                return;
            }

            _running = true;
            try
            {
                do
                {
                    _pending = false;
                    TryStartFall();
                } while (_pending);
            }
            finally
            {
                _running = false;
            }
        }

        void TryStartFall()
        {
            for (int y = 0; y < _slots.Count; y++)
            {
                var slot = _slots[y];
                if (slot.State != CellState.Empty) continue;

                int sourceY = FindNearestValidCellAbove(y);
                if (sourceY == -1) continue;

                StartFall(sourceY, y);
            }
        }

        int FindNearestValidCellAbove(int fromY)
        {
            for (int y = fromY + 1; y < _slots.Count; y++)
            {
                var s = _slots[y];
                if (s.State == CellState.Occupied || s.State == CellState.Falling)
                    return y;
            }
            return -1;
        }

        void StartFall(int fromY, int toY)
        {
            var sourceSlot = _slots[fromY];
            var targetSlot = _slots[toY];
            var item = sourceSlot.Item;
            sourceSlot.SetEmpty();
            targetSlot.SetFalling(item, new Vector2Int(_columnIndex, fromY));
            
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}