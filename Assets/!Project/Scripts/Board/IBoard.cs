using UnityEngine;

namespace Game
{
    public interface IBoard
    {
        Vector2Int Size { get; }
        PlayfieldItem Get(Vector2Int pos);
        PlayfieldItemColorType? GetColor(Vector2Int pos);
        bool IsInBounds(Vector2Int pos);
        void Swap(Vector2Int cellA, Vector2Int cellB);
        void Set(Vector2Int cell, PlayfieldItem item);
        void Clear(Vector2Int cell);
    }
}