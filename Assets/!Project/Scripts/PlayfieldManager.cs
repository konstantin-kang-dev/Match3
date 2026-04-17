using UnityEngine;
using Zenject;

public class PlayfieldManager : MonoBehaviour
{
    [SerializeField] PlayfieldItem ItemPrefab;
    PlayfieldItem[,] _playfieldItems;

    [Inject]
    GridManager _gridManager;
    public void Init()
    {
        Vector2 gridSize = _gridManager.GridSize;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                SpawnItem(new Vector2Int(x, y));
            }
        }
    }

    void SpawnItem(Vector2Int targetCell)
    {
        Vector2 cellSize = _gridManager.CellSize;
        PlayfieldItem item = Instantiate(ItemPrefab);
        RectTransform rectTransform = item.GetComponent<RectTransform>();
        rectTransform.SetParent(transform);
        rectTransform.anchoredPosition = targetCell * cellSize + (cellSize / 2f);
    }
}
