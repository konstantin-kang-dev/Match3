using DG.Tweening;
using Game;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class PlayfieldItemVisuals: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform _rectTransform;
    [SerializeField] Image _icon;

    Vector2 _dragStartPos = Vector2.zero;
    public void Init(PlayfieldItemConfig config)
    {
        _rectTransform = GetComponent<RectTransform>();
        _icon.sprite = config.Icon;
    }

    public void MoveTo(Vector2 targetPos, bool doInstantly = false)
    {
        if (doInstantly)
        {
            _rectTransform.anchoredPosition = targetPos;
            return;
        }

        _rectTransform.DOLocalMove(targetPos, 1f);
    }


    void HandleDrag(Vector2 dragPos)
    {
        /*
        Vector2 delta = dragPos - _dragStartPos;
        Vector2Int direction = ProjectUtils.GetSwipeDirection(delta);
        Vector2Int targetCell = OccupiedCell + direction;
        */
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragStartPos = eventData.position;
        HandleDrag(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        HandleDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
