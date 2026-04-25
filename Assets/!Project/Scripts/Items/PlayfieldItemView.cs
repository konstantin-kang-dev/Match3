using DG.Tweening;
using Game.Configs;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using static UnityEngine.GraphicsBuffer;

public enum MoveAnimationType
{
    None = 0,
    Move = 1,
    Bounce = 2,
}

public class PlayfieldItemView: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform _rectTransform;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] Image _icon;
    [SerializeField] ParticleSystem _collapseVFX;

    Vector2 _dragStartPos = Vector2.zero;
    bool _isDragged = false;
    bool _isVisible = false;

    Sequence _spawnAnim;

    public Subject<Vector2Int> OnSwapRequest = new Subject<Vector2Int>();
    public Subject<bool> OnDestroyed = new Subject<bool>();

    public void Init(PlayfieldItemConfig config)
    {
        _rectTransform = GetComponent<RectTransform>();
        SetVisibility(false, true);
    }

    public void SetVisibility(bool visible, bool force = false)
    {
        if (_isVisible == visible && !force) return;

        if (visible)
        {
            if(_spawnAnim != null)
            {
                _spawnAnim.Kill();
            }

            _spawnAnim = DOTween.Sequence();
            
            Tween fadeAnim = _canvasGroup.DOFade(1f, ProjectConstants.ITEM_SPAWN_ANIM_DURATION).From(0f).SetEase(Ease.InQuad);
            _spawnAnim.Append(fadeAnim);
            Tween scaleAnim = _rectTransform.DOScale(1f, ProjectConstants.ITEM_SPAWN_ANIM_DURATION).From(0.5f).SetEase(Ease.InQuad);
            _spawnAnim.Join(scaleAnim);
        }
        else
        {
            _canvasGroup.alpha = 0f;
        }

        _isVisible = visible;
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

        float speed = 2200f;
        float distance = Vector2.Distance(_rectTransform.anchoredPosition, targetPos);
        float duration = distance / speed;

        switch (animType)
        {
            case MoveAnimationType.Move:
                _rectTransform.DOAnchorPos(targetPos, duration);
                break;
            case MoveAnimationType.Bounce:
                //_rectTransform.DOAnchorPos(targetPos, duration).SetEase(Ease.InQuad);
                
                if(_spawnAnim != null)
                {
                    _spawnAnim.Kill();
                }
                
                Sequence sequence = DOTween.Sequence();

                Tween moveAnim = _rectTransform.DOAnchorPos(targetPos, duration).SetEase(Ease.InSine);
                sequence.Append(moveAnim);

                Vector3 squashInScale = new Vector3(1.05f, 0.95f, 1f);
                Tween bounceInAnim = _rectTransform.DOScale(squashInScale, 0.1f);
                sequence.Append(bounceInAnim);

                float overshootYPos = targetPos.y - 10f;
                Tween overshootInAnim = _rectTransform.DOAnchorPosY(overshootYPos, 0.1f);
                sequence.Join(overshootInAnim);

                Vector3 squashOutScale = new Vector3(1f, 1f, 1f);
                Tween bounceOutAnim = _rectTransform.DOScale(squashOutScale, 0.1f);
                sequence.Append(bounceOutAnim);

                Tween overshootOutAnim = _rectTransform.DOAnchorPosY(targetPos.y, 0.1f);
                sequence.Join(overshootOutAnim);
                break;
            default:
                break;
        }
    }

    public void AnimateDestroy()
    {
        _rectTransform
            .DOScale(Vector3.zero, ProjectConstants.ITEM_DESTROY_ANIM_DURATION)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _collapseVFX.Play();
                OnDestroyed.OnNext(true);
                Destroy(gameObject, 2f);
            });
    }

    void OnDestroy()
    {
        OnSwapRequest.Dispose();
        OnDestroyed.Dispose();
    }

    void HandleDrag(Vector2 dragPos)
    {
        
        Vector2 delta = dragPos - _dragStartPos;
        Vector2Int direction = ProjectUtils.GetSwipeDirection(delta);

        OnSwapRequest.OnNext(direction);
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
        float dragDistance = (_dragStartPos - eventData.position).magnitude;
        if (dragDistance < 50f) return;

        _isDragged = true;
        HandleDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //_dragStartPos = eventData.position;
        //HandleDrag(eventData.position);
    }
}
