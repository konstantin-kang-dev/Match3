using UnityEngine;

namespace Game
{
    public interface ISwapRequester
    {
        void TrySwap(Vector2Int from, Vector2Int direction);
    }
}