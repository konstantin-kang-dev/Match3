using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IBoardContext
    {
        Vector2Int Size { get; }
        bool IsValidCell(Vector2Int cell);
        UniTask DestroyCells(IEnumerable<Vector2Int> cells, DestroyMode mode = DestroyMode.Animated, bool playVfx = true);
        IEnumerable<Vector2Int> GetCellsInRadius(Vector2Int center, int radius);
        Vector2Int? FindRandomColoredCell();
        IEnumerable<Vector2Int> GetCellsByColor(PlayfieldItemColorType color);
        PlayfieldItemColorType? GetDominantColor();
        Vector2 GetWorldPosition(Vector2Int cell);
    }
}