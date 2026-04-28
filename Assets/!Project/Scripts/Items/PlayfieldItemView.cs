using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game;
using Game.Configs;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Game.Utils;

public enum MoveAnimationType
{
    None = 0,
    Move = 1,
    Bounce = 2
}

public class PlayfieldItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _icon;
    [SerializeField] private Sprite _alternativeSprite;
    private Sprite _initialSprite;
    
    [SerializeField] private RectTransform _rectTransform;
    public RectTransform RectTransform => _rectTransform;
    
    private Vector2 _dragStartPos = Vector2.zero;
    private bool _isDragged;
    
    private Sequence _spawnAnim;
    private Subject<bool> _onDestroyed = new();
    public Observable<bool> OnDestroyed => _onDestroyed;
    
    private Subject<Vector2Int> _onSwapRequest = new();
    public Observable<Vector2Int> OnSwapRequest => _onSwapRequest;
    private void OnDestroy()
    {
        _onSwapRequest.Dispose();
        _onDestroyed.Dispose();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragged = false;
        _dragStartPos = eventData.position;
        //HandleDrag(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragged) return;
        var dragDistance = (_dragStartPos - eventData.position).magnitude;
        if (dragDistance < 50f) return;

        _isDragged = true;
        HandleDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //_dragStartPos = eventData.position;
        //HandleDrag(eventData.position);
    }

    public void Init(PlayfieldItemConfig config)
    {
        _initialSprite = _icon.sprite;
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0f;
    }
    public UniTask PlaySpawnAnimation()
    {
        if (_spawnAnim != null) _spawnAnim.Kill();

        _spawnAnim = DOTween.Sequence();

        Tween fadeAnim = _canvasGroup.DOFade(1f, ProjectConstants.ITEM_SPAWN_ANIM_DURATION).From(0f)
            .SetEase(Ease.InQuad);
        _spawnAnim.Append(fadeAnim);
        Tween scaleAnim = _rectTransform.DOScale(1f, ProjectConstants.ITEM_SPAWN_ANIM_DURATION).From(0.5f)
            .SetEase(Ease.InQuad);
        _spawnAnim.Join(scaleAnim);

        return UniTask.WaitForSeconds(ProjectConstants.ITEM_SPAWN_ANIM_DURATION);
    }

    public void SetAlternativeSprite(bool setAlternative)
    {
        _icon.sprite = setAlternative ? _alternativeSprite : _initialSprite;
    }
    
    public void SetSize(Vector2 size)
    {
        _rectTransform.sizeDelta = size;
    }

    public void MoveTo(Vector2 targetPos, MoveAnimationType animType = MoveAnimationType.Move)
    {
        if (animType == MoveAnimationType.None)
        {
            _rectTransform.anchoredPosition = targetPos;
            return;
        }

        var speed = 2200f;
        var distance = Vector2.Distance(_rectTransform.anchoredPosition, targetPos);
        var duration = distance / speed;

        switch (animType)
        {
            case MoveAnimationType.Move:
                _rectTransform.DOAnchorPos(targetPos, duration);
                break;
            case MoveAnimationType.Bounce:
                //_rectTransform.DOAnchorPos(targetPos, duration).SetEase(Ease.InQuad);

                if (_spawnAnim != null) _spawnAnim.Kill();

                var sequence = DOTween.Sequence();

                Tween moveAnim = _rectTransform.DOAnchorPos(targetPos, duration).SetEase(Ease.InSine);
                sequence.Append(moveAnim);

                var squashInScale = new Vector3(1.05f, 0.95f, 1f);
                Tween bounceInAnim = _rectTransform.DOScale(squashInScale, 0.1f);
                sequence.Append(bounceInAnim);

                var overshootYPos = targetPos.y - 10f;
                Tween overshootInAnim = _rectTransform.DOAnchorPosY(overshootYPos, 0.1f);
                sequence.Join(overshootInAnim);

                var squashOutScale = new Vector3(1f, 1f, 1f);
                Tween bounceOutAnim = _rectTransform.DOScale(squashOutScale, 0.1f);
                sequence.Append(bounceOutAnim);

                Tween overshootOutAnim = _rectTransform.DOAnchorPosY(targetPos.y, 0.1f);
                sequence.Join(overshootOutAnim);
                break;
        }
    }

    public void Destroy(DestroyMode mode)
    {
        Tween destroyAnim = _rectTransform
            .DOScale(Vector3.zero, ProjectConstants.ITEM_DESTROY_ANIM_DURATION)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _onDestroyed.OnNext(true);
                Destroy(gameObject, 2f);
            });

        if (mode == DestroyMode.Instant)
        {
            destroyAnim.Complete();
        }
    }

    private void HandleDrag(Vector2 dragPos)
    {
        var delta = dragPos - _dragStartPos;
        var direction = ProjectUtils.GetSwipeDirection(delta);

        _onSwapRequest.OnNext(direction);
    }
}