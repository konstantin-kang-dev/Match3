using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System;
using UnityEngine;

namespace Game
{
    public class FallAnimator : IDisposable
    {
        readonly BoardState _board;
        readonly GridManager _gridManager;
        readonly CompositeDisposable _disposables = new();

        public FallAnimator(BoardState board, GridManager gridManager)
        {
            _board = board;
            _gridManager = gridManager;

            foreach (var slot in board.AllCells())
            {
                slot.OnFallStarted
                    .Subscribe(OnFallStarted)
                    .AddTo(_disposables);
            }
        }

        public void Dispose()
        {
            
        }

        void OnFallStarted(FallStartedEvent evt)
        {
            AnimateFall(evt).Forget();
        }

        async UniTask AnimateFall(FallStartedEvent evt)
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
}