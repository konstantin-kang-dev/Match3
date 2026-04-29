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

        readonly Subject<int> _onNeedsRefill = new();
        public Observable<int> OnNeedsRefill => _onNeedsRefill.AsObservable();

        public ColumnSimulator(int columnIndex, List<CellSlot> slots, BoardActivityTracker tracker)
        {
            _columnIndex = columnIndex;
            _slots = slots;
            _tracker = tracker;

            // Подписываемся на изменения каждого слота своей колонки
            foreach (var slot in _slots)
            {
                slot.OnStateChanged
                    .Subscribe(OnSlotChanged)
                    .AddTo(_disposables);
            }
        }

        void OnSlotChanged(CellSlot changedSlot)
        {
            // Реагируем только на освобождение клетки
            if (changedSlot.State != CellState.Empty) return;

            // Доска заморожена — новые падения не запускаем.
            // Существующие Falling доедут до своих ToCell естественно.
            // На Unfreeze координатор вызовет ResumeIfNeeded и колонка перепроверится.
            if (_tracker.IsFrozen) return;

            TryStartFall();
        }

        // Принудительно перепроверить колонку. Вызывается координатором после Unfreeze,
        // когда Empty-события случились под freeze и были проигнорированы.
        public void ResumeIfNeeded()
        {
            if (_tracker.IsFrozen) return;
            TryStartFall();
        }

        void TryStartFall()
        {
            // Идём снизу вверх. Для каждой Empty клетки — ищем выше первую Occupied и спускаем её.
            for (int y = 0; y < _slots.Count; y++)
            {
                var slot = _slots[y];
                if (slot.State != CellState.Empty) continue;

                // Ищем выше первую Occupied клетку
                int sourceY = FindNearestOccupiedAbove(y);
                if (sourceY == -1)
                {
                    // Нечем заполнить из колонки — нужен спавн сверху
                    if (NeedsSpawnAt(y))
                        _onNeedsRefill.OnNext(_columnIndex);
                    continue;
                }

                // Запускаем падение из sourceY → y
                StartFall(sourceY, y);
            }
        }

        int FindNearestOccupiedAbove(int fromY)
        {
            for (int y = fromY + 1; y < _slots.Count; y++)
            {
                if (_slots[y].State == CellState.Occupied)
                    return y;
            }
            return -1;
        }

        bool NeedsSpawnAt(int y)
        {
            // Спавн нужен, если выше этой клетки нет Occupied И нет Falling.
            for (int upper = y + 1; upper < _slots.Count; upper++)
            {
                if (_slots[upper].State == CellState.Occupied) return false;
                if (_slots[upper].State == CellState.Falling) return false;
            }
            return true;
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
            _onNeedsRefill.Dispose();
        }
    }
}