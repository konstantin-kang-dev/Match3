using UnityEngine;

public class GridCell : MonoBehaviour
{
    private RectTransform _rectTransform;

    public void SetSize(Vector2 size)
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.sizeDelta = size;
    }
}