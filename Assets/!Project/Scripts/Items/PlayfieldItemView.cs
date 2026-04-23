using DG.Tweening;
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
    [SerializeField] Image _icon;
    [SerializeField] Image _shadow;
    [SerializeField] ParticleSystem _collapseVFX;

    Vector2 _dragStartPos = Vector2.zero;
    bool isDragged = false;

    public Subject<Vector2Int> OnSwapRequest = new Subject<Vector2Int>();
    public Subject<bool> OnDestroyed = new Subject<bool>();

    public void Init(PlayfieldItemConfig config)
    {
        _rectTransform = GetComponent<RectTransform>();
        SetVisibility(true);
    }

    public void SetVisibility(bool visible)
    {
        _icon.gameObject.SetActive(visible);
        _shadow.gameObject.SetActive(visible);

        if (visible)
        {
            _icon.DOFade(1f, ProjectConstants.ITEM_SPAWN_ANIM_DURATION).From(0f).SetEase(Ease.InQuad);
            _shadow.DOFade(1f, ProjectConstants.ITEM_SPAWN_ANIM_DURATION).From(0f).SetEase(Ease.InQuad);
            _rectTransform.DOScale(1f, ProjectConstants.ITEM_SPAWN_ANIM_DURATION).From(0.5f).SetEase(Ease.InQuad);
        }
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

        float speed = 1600f;
        float distance = Vector2.Distance(_rectTransform.anchoredPosition, targetPos);
        float duration = distance / speed;

        switch (animType)
        {
            case MoveAnimationType.Move:
                _rectTransform.DOAnchorPos(targetPos, duration);
                break;
            case MoveAnimationType.Bounce:
                //_rectTransform.DOAnchorPos(targetPos, duration).SetEase(Ease.InQuad);
                _rectTransform.DOAnchorPos(targetPos, duration).SetEase(Ease.Linear);
                break;
            default:
                break;
        }
    }

    public void AnimateDestroy()
    {
        _rectTransform
            .DOScale(Vector3.zero, ProjectConstants.ITEM_DESTROY_ANIM_DURATION)
            .SetEase(Ease.InBack)
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
        isDragged = false;
        _dragStartPos = eventData.position;
        //HandleDrag(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragged) return;
        float dragDistance = (_dragStartPos - eventData.position).magnitude;
        if (dragDistance < 50f) return;

        isDragged = true;
        HandleDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //_dragStartPos = eventData.position;
        //HandleDrag(eventData.position);
    }
}
