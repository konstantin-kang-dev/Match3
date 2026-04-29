using R3;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class ColumnSimulator : IDisposable
    {
        readonly int _columnIndex;
        readonly List<CellSlot> _slots;
        readonly BoardActivityTracker _tracker;
        readonly CompositeDisposable _disposables = new();

        readonly Subject<int> _onNeedsRefill = new();
        public Observable<int> OnNeedsRefill => _onNeedsRefill.AsObservable();

        // Re-entry guard: TryStartFall вызывает мутации (SetEmpty/SetFalling),
        // которые синхронно эмитят OnStateChanged → OnSlotChanged → TryStartFall.
        // Без guard'а получим вложенные циклы, работающие на одном и том же
        // состоянии колонки, что приводит к двойным записям в один слот
        // (наслоение фишек) и потере Item'ов.
        //
        // Паттерн: пока идёт внешний цикл, вложенные триггеры только взводят
        // _pending. После внешнего — крутим повторы, пока pending не сбросится.
        // Гарантия: каждое изменение либо обработано в текущем проходе,
        // либо вызовет немедленный повтор.
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
                if (sourceY == -1)
                {
                    if (NeedsSpawnAt(y))
                    {
                        _onNeedsRefill.OnNext(_columnIndex); //TODO: Надо сделать, чтобы новые фишки падали очередно друг за другом, сейчас они спавнятся в кучке и падают одновременно в свои места
                    }
                    continue;
                }
                StartFall(sourceY, y);
            }
        }

        int FindNearestValidCellAbove(int fromY)
        {
            for (int y = fromY + 1; y < _slots.Count; y++)
            {
                if (_slots[y].State == CellState.Occupied || _slots[y].State == CellState.Falling)
                    return y;
            }
            return -1;
        }

        bool NeedsSpawnAt(int y)
        {
            for (int upper = y + 1; upper < _slots.Count; upper++)
            {
                var s = _slots[upper];
                if (s.State == CellState.Occupied) return false;

                if (s.State == CellState.Falling && s.Item != null && s.Item.IsRefillFalling) return false;
            }
            return true;
        }

        void StartFall(int fromY, int toY)
        {
            var sourceSlot = _slots[fromY];
            var targetSlot = _slots[toY];
            var item = sourceSlot.Item;
            if (item != null && item.IsRefillFalling)
                item.MarkRefillFalling(false);
            
            sourceSlot.SetEmpty();
            targetSlot.SetFalling(item, new Vector2Int(_columnIndex, fromY));
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _onNeedsRefill.Dispose();
        }
    }
}