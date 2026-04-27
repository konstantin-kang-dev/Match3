using UnityEngine;

namespace Game
{
    public interface IBoard
    {
        Vector2Int Size { get; }
        PlayfieldItem Get(Vector2Int pos);
        PlayfieldItemColorType? GetColor(Vector2Int pos);
        bool IsInBounds(Vector2Int pos);
    }
}