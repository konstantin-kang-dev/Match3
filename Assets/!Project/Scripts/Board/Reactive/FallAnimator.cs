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
                rt.DOKill();
                
                Vector2 targetPos = _gridManager.GetPositionForCell(evt.ToCell);

                float distance = Vector2.Distance(rt.anchoredPosition, targetPos);
                float duration = distance / 2200f;
                
                Tween moveAnim = rt.DOAnchorPos(targetPos, duration).SetEase(Ease.Linear);
                
                await moveAnim.AsyncWaitForCompletion().AsUniTask();
                
                var targetSlot = _board.Get(evt.ToCell);
                if (targetSlot.State != CellState.Falling || targetSlot.Item != item)
                    return;
                
                item.OccupyCell(evt.ToCell);
                targetSlot.SetOccupied(item);
                if (targetSlot.State != CellState.Occupied)
                    return;
                
                var bounceSeq = DOTween.Sequence().SetTarget(rt);
                var squashInScale = new Vector3(1.05f, 0.95f, 1f);
                Tween bounceInAnim = rt.DOScale(squashInScale, 0.1f);
                bounceSeq.Append(bounceInAnim);

                var overshootYPos = targetPos.y - 10f;
                Tween overshootInAnim = rt.DOAnchorPosY(overshootYPos, 0.1f);
                bounceSeq.Join(overshootInAnim);
                
                var squashOutScale = new Vector3(1f, 1f, 1f);
                Tween bounceOutAnim = rt.DOScale(squashOutScale, 0.1f);
                bounceSeq.Append(bounceOutAnim);
                
                Tween overshootOutAnim = rt.DOAnchorPosY(targetPos.y, 0.1f);
                bounceSeq.Join(overshootOutAnim);
                
                
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}