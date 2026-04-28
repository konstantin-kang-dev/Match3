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

            seq.Append(rt.DOScale(new Vector3(1.3f, 1.3f, 1f), 0.12f).SetEase(Ease.OutBack));
            seq.Append(rt.DOScale(new Vector3(1f, 1f, 1f), 0.12f).SetEase(Ease.OutBack));

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

            seq.Append(rt.DOScale(new Vector3(1.4f, 1.4f, 1f), 0.15f).SetEase(Ease.OutQuad));
            seq.Append(rt.DOScale(new Vector3(1.2f, 0.8f, 1f), 0.05f));

            return seq.AsyncWaitForCompletion().AsUniTask();
        }
        
        public UniTask PlayPlaneSpawn(PlayfieldItem item)
        {
            var rt = item.RectTransform;
            var seq = DOTween.Sequence();

            seq.Append(rt.DOScale(new Vector3(1.3f, 1.3f, 1f), 0.15f).SetEase(Ease.OutBack));
            seq.Join(rt.DORotate(new Vector3(0f, 0f, 15f), 0.15f).SetEase(Ease.OutBack));
            
            seq.Append(rt.DOScale(new Vector3(1f, 1f, 1f), 0.15f).SetEase(Ease.OutBack));
            seq.Join(rt.DORotate(new Vector3(0f, 0f, 0f), 0.15f).SetEase(Ease.OutBack));

            return seq.AsyncWaitForCompletion().AsUniTask();
        }

        public UniTask PlayPlaneActivation(PlayfieldItem item, Vector2Int targetCell)
        {
            var rt = item.RectTransform;
            var seq = DOTween.Sequence();
            Vector2 targetPos = _gridManager.GetPositionForCell(targetCell);
            Vector2 startPos = rt.anchoredPosition;

            Vector2 direction = targetPos - startPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            float targetAngle = angle - 90f;

            seq.Append(rt.DOScale(new Vector3(0.9f, 1.2f, 1f), 0.1f));
            seq.Append(rt.DOAnchorPosY(startPos.y + 30f, 0.15f).SetEase(Ease.OutCubic));

            seq.Append(rt.DOLocalRotate(new Vector3(0, 0, targetAngle), 0.3f).SetEase(Ease.OutQuad));
            seq.Join(rt.DOScale(Vector3.one, 0.2f));
            
            seq.Append(rt.DOAnchorPos(targetPos, 0.6f).SetEase(Ease.InSine));

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