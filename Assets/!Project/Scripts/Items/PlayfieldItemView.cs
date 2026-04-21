using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class PlayfieldItemView: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform _rectTransform;
    [SerializeField] Image _icon;

    Vector2 _dragStartPos = Vector2.zero;
    bool isDragged = false;

    public Subject<Vector2Int> OnSwapRequest = new Subject<Vector2Int>();
    public Subject<bool> OnDestroyed = new Subject<bool>();

    public void Init(PlayfieldItemConfig config)
    {
        _rectTransform = GetComponent<RectTransform>();
        _icon.sprite = config.Icon;
    }

    public void MoveTo(Vector2 targetPos, float time = 0.5f, bool doInstantly = false)
    {
        if (doInstantly)
        {
            _rectTransform.anchoredPosition = targetPos;
            return;
        }

        _rectTransform.DOAnchorPos(targetPos, time);
    }

    public void AnimateDestroy()
    {
        _rectTransform
            .DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                OnDestroyed.OnNext(true);
                Destroy(gameObject);
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
