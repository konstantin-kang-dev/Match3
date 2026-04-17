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
    PlayfieldItemPresenter _presenter;
    [SerializeField] Image _icon;

    Vector2 _dragStartPos = Vector2.zero;
    public void Init(PlayfieldItemPresenter playfieldItemPresenter, PlayfieldItemConfig config)
    {
        _presenter = playfieldItemPresenter;
        _icon.sprite = config.Icon;
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
