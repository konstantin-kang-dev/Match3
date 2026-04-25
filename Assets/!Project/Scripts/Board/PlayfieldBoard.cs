using UnityEngine;

namespace Game
{
    public class PlayfieldBoard : IBoard
    {
        readonly PlayfieldItem[,] _items;
        public Vector2Int Size { get; }

        public PlayfieldBoard(Vector2Int size)
        {
            Size = size;
            _items = new PlayfieldItem[size.x, size.y];
        }

        public PlayfieldItem Get(Vector2Int cell) => _items[cell.x, cell.y];

        public PlayfieldItemColorType? GetType(Vector2Int cell)
        {
            var item = _items[cell.x, cell.y];
            return item?.Color;
        }

        public void Set(Vector2Int cell, PlayfieldItem item) => _items[cell.x, cell.y] = item;

        public void Clear(Vector2Int cell) => _items[cell.x, cell.y] = null;

        public bool IsInBounds(Vector2Int cell) =>
            cell.x >= 0 && cell.x < Size.x && cell.y >= 0 && cell.y < Size.y;

        public void Swap(Vector2Int cellA, Vector2Int cellB)
        {
            (_items[cellA.x, cellA.y], _items[cellB.x, cellB.y]) = (_items[cellB.x, cellB.y], _items[cellA.x, cellA.y]);
        }
    }
}