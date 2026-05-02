using R3;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace Game
{
    public class MatchScanner : IStartable, IDisposable
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
        }

        public void Start()
        {
            foreach (var slot in _board.AllCells())
            {
                slot.OnStateChanged
                    .Subscribe(OnSlotStateChanged)
                    .AddTo(_disposables);
            }
        }

        void OnSlotStateChanged(CellSlot slot)
        {
            if (slot.State != CellState.Occupied) return;

            ScanAndResolve(slot.Position).Forget();
        }

        async UniTask ScanAndResolve(Vector2Int pos)
        {
            var components = _detector.FindMatches(new[] { pos });
            if (components.Count == 0) return;

            var groups = BuildGroups(components);
            if (groups.Count == 0) return;

            await _resolver.Resolve(groups, swapCell: null, cascade: 0);
        }

        List<MatchGroup> BuildGroups(List<MatchComponent> components)
        {
            var groups = new List<MatchGroup>(components.Count);
            foreach (var component in components)
            {
                var recognized = MatchShapeRecognizer.Recognize(component.Cells);
                if (!recognized.HasValue) continue;

                groups.Add(new MatchGroup(
                    cells: component.Cells,
                    shapeCells: recognized.Value.ShapeCells,
                    color: component.Color,
                    shape: recognized.Value.Shape));
            }
            return groups;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}