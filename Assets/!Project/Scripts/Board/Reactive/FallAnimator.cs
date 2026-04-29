using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System;
using UnityEngine;
using VContainer.Unity;

namespace Game
{
    public class FallAnimator : IStartable, IDisposable
    {
        readonly BoardState _board;
        readonly GridManager _gridManager;
        readonly BoardActivityTracker _tracker;
        readonly CompositeDisposable _disposables = new();

        public FallAnimator(
            BoardState board,
            GridManager gridManager,
            BoardActivityTracker tracker)
        {
            _board = board;
            _gridManager = gridManager;
            _tracker = tracker;
        }

        public void Start()
        {
            foreach (var slot in _board.AllCells())
            {
                slot.OnFallStarted
                    .Subscribe(OnFallStarted)
                    .AddTo(_disposables);
            }
        }

        void OnFallStarted(FallStartedEvent evt)
        {
            AnimateFall(evt).Forget();
        }

        async UniTask AnimateFall(FallStartedEvent evt)
        {
            using (_tracker.BeginActivity())
            {
                var item = evt.Item;
                var rt = item.RectTransform;
                Vector2 targetPos = _gridManager.GetPositionForCell(evt.ToCell);

                float distance = Vector2.Distance(rt.anchoredPosition, targetPos);
                float duration = distance / 2200f;

                await rt.DOAnchorPos(targetPos, duration)
                    .SetEase(Ease.InQuad)
                    .AsyncWaitForCompletion()
                    .AsUniTask();

                // После приземления — переводим в Occupied
                var targetSlot = _board.Get(evt.ToCell);
                item.OccupyCell(evt.ToCell);
                targetSlot.SetOccupied(item);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}