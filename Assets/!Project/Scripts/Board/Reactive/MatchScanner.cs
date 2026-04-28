using R3;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class MatchScanner : IDisposable
    {
        readonly BoardState _board;
        readonly MatchDetector _detector;
        readonly MatchResolver _resolver;
        readonly CompositeDisposable _disposables = new();

        public MatchScanner(BoardState board, MatchDetector detector, MatchResolver resolver)
        {
            _board = board;
            _detector = detector;
            _resolver = resolver;

            foreach (var slot in board.AllCells())
            {
                slot.OnStateChanged
                    .Subscribe(OnSlotStateChanged)
                    .AddTo(_disposables);
            }
        }

        void OnSlotStateChanged(CellSlot slot)
        {
            if (slot.State != CellState.Occupied) return;

            // Сканируем матчи через эту клетку
            ScanAndResolve(slot.Position).Forget();
        }

        async UniTask ScanAndResolve(Vector2Int pos)
        {
            var groups = _detector.FindMatches(new[] { pos });
            if (groups.Count == 0) return;

            await _resolver.Resolve(groups, swapCell: null, cascade: 0);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}