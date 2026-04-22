using UnityEngine;

namespace Game
{
    public class PlayfieldBoard : IBoard
    {
        readonly PlayfieldItemPresenter[,] _items;
        public Vector2Int Size { get; }

        public PlayfieldBoard(Vector2Int size)
        {
            Size = size;
            _items = new PlayfieldItemPresenter[size.x, size.y];
        }

        public PlayfieldItemPresenter Get(Vector2Int pos) => _items[pos.x, pos.y];

        public PlayfieldItemType? GetType(Vector2Int pos)
        {
            var item = _items[pos.x, pos.y];
            return item?.Model.Type;
        }

        public void Set(Vector2Int pos, PlayfieldItemPresenter item) => _items[pos.x, pos.y] = item;

        public void Clear(Vector2Int pos) => _items[pos.x, pos.y] = null;

        public bool IsInBounds(Vector2Int pos) =>
            pos.x >= 0 && pos.x < Size.x && pos.y >= 0 && pos.y < Size.y;

        public void Swap(Vector2Int a, Vector2Int b)
        {
            (_items[a.x, a.y], _items[b.x, b.y]) = (_items[b.x, b.y], _items[a.x, a.y]);
        }
    }
}