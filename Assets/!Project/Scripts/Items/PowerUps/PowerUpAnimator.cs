using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class PowerUpAnimator
    {
        private readonly GridManager _gridManager;
        public PowerUpAnimator(GridManager gridManager)
        {
            _gridManager = gridManager;
        }
        public async UniTask PlayMergeAnimation(List<PlayfieldItem> items, Vector2 targetPos, float duration = 0.2f)
        {
            var tasks = new List<UniTask>();
            foreach (var item in items)
                tasks.Add(AnimateOneToTarget(item, targetPos, duration));

            await UniTask.WhenAll(tasks);
        }

        async UniTask AnimateOneToTarget(PlayfieldItem item, Vector2 targetPos, float duration)
        {
            var rt = item.RectTransform;

            var seq = DOTween.Sequence();
            seq.Append(rt.DOAnchorPos(targetPos, duration).SetEase(Ease.InQuad));
            seq.Join(rt.DOScale(0.3f, duration).SetEase(Ease.InQuad));

            await seq.AsyncWaitForCompletion().AsUniTask();
        }
        
        public UniTask PlayRocketSpawn(PlayfieldItem item)
        {
            var rt = item.RectTransform;
            var seq = DOTween.Sequence();

            seq.Append(rt.DOScale(new Vector3(1.3f, 1.3f, 1f), 0.15f).SetEase(Ease.OutBack));
            seq.Join(rt.DORotate(new Vector3(0f, 0f, 15f), 0.15f).SetEase(Ease.OutBack));
            
            seq.Append(rt.DOScale(new Vector3(1f, 1f, 1f), 0.15f).SetEase(Ease.OutBack));
            seq.Join(rt.DORotate(new Vector3(0f, 0f, 0f), 0.15f).SetEase(Ease.OutBack));

            return seq.AsyncWaitForCompletion().AsUniTask();
        }
        
        public UniTask PlayRocketActivation(PlayfieldItem item)
        {
            var rt = item.RectTransform;
            var seq = DOTween.Sequence();

            seq.Append(rt.DOScale(new Vector3(0f, 0f, 0f), 0.01f).SetEase(Ease.InBack));

            return seq.AsyncWaitForCompletion().AsUniTask();
        }
        
        public UniTask PlayBombSpawn(PlayfieldItem item)
        {
            var rt = item.RectTransform;
            var seq = DOTween.Sequence();

            seq.Append(rt.DOScale(new Vector3(1.3f, 1.3f, 1f), 0.15f).SetEase(Ease.OutBack));
            seq.Join(rt.DORotate(new Vector3(0f, 0f, 15f), 0.15f).SetEase(Ease.OutBack));
            
            seq.Append(rt.DOScale(new Vector3(1f, 1f, 1f), 0.15f).SetEase(Ease.OutBack));
            seq.Join(rt.DORotate(new Vector3(0f, 0f, 0f), 0.15f).SetEase(Ease.OutBack));

            return seq.AsyncWaitForCompletion().AsUniTask();
        }
        
        public UniTask PlayBombActivation(PlayfieldItem item)
        {
            var rt = item.RectTransform;
            var seq = DOTween.Sequence();

            seq.Append(rt.DOScale(new Vector3(0f, 0f, 1f), 0.01f).SetEase(Ease.InBack));

            return seq.AsyncWaitForCompletion().AsUniTask();
        }
        
        public UniTask PlayBalloonSpawn(PlayfieldItem item)
        {
            var rt = item.RectTransform;
            var seq = DOTween.Sequence();
            
            seq.Append(rt.DOScale(new Vector3(1.3f, 1.3f, 1f), 0.15f).SetEase(Ease.OutBack));
            
            seq.Append(rt.DOScale(new Vector3(1f, 1f, 1f), 0.15f).SetEase(Ease.OutBack));

            return seq.AsyncWaitForCompletion().AsUniTask();
        }

        public UniTask PlayBalloonActivation(PlayfieldItem item, Vector2Int targetCell)
        {
            var rt = item.RectTransform;
            var seq = DOTween.Sequence();
            Vector2 targetPos = _gridManager.GetPositionForCell(targetCell);
            Vector2 startPos = rt.anchoredPosition;

            seq.Append(rt.DOAnchorPos(targetPos, 0.6f).SetEase(Ease.InOutQuad));

            seq.Join(DOTween.To(
                () => 0f,
                t => rt.localScale = Vector3.LerpUnclamped(Vector3.one, new Vector3(2f, 2f, 2f), Mathf.Sin(t * Mathf.PI)),
                1f,
                0.6f
            ).SetEase(Ease.Linear));

            return seq.AsyncWaitForCompletion().AsUniTask();
        }
        
        public UniTask PlayDiscoSpawn(PlayfieldItem item)
        {
            var rt = item.RectTransform;
            var seq = DOTween.Sequence();

            seq.Append(rt.DOScale(new Vector3(1.3f, 1.3f, 1f), 0.15f).SetEase(Ease.OutBack));
            seq.Join(rt.DORotate(new Vector3(0f, 0f, 15f), 0.15f).SetEase(Ease.OutBack));
            
            seq.Append(rt.DOScale(new Vector3(1f, 1f, 1f), 0.15f).SetEase(Ease.OutBack));
            seq.Join(rt.DORotate(new Vector3(0f, 0f, 0f), 0.15f).SetEase(Ease.OutBack));

            return seq.AsyncWaitForCompletion().AsUniTask();
        }

        public UniTask PlayDiscoActivation(PlayfieldItem item)
        {
            var rt = item.RectTransform;
            var startY = rt.anchoredPosition.y;
            var seq = DOTween.Sequence();

            seq.Append(rt.DOAnchorPosY(startY + 40f, 0.2f).SetEase(Ease.OutCubic));
            seq.Join(rt.DOScale(new Vector3(0.85f, 1.25f, 1f), 0.2f));
            seq.AppendInterval(0.1f);

            return seq.AsyncWaitForCompletion().AsUniTask();
        }
    }
}