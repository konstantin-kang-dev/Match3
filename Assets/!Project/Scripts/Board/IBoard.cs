using UnityEngine;

namespace Game
{
    public interface IBoard
    {
        Vector2Int Size { get; }
        PlayfieldItem Get(Vector2Int pos);
        PlayfieldItemType? GetType(Vector2Int pos);
        bool IsInBounds(Vector2Int pos);
    }
}