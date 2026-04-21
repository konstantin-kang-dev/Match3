using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    [SerializeField] Image _icon;

    public void SetColor(Color color)
    {
        _icon.color = color;
    }
}
