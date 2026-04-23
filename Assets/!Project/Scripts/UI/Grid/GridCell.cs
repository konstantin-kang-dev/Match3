using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    RectTransform _rectTransform;
    public void SetSize(Vector2 size)
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.sizeDelta = size;
    }
}
