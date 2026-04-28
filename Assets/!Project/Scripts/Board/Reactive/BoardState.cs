using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BoardState
    {
        readonly CellSlot[,] _cells;
        public Vector2Int Size { get; }

        public BoardState(Vector2Int size)
        {
            Size = size;
            _cells = new CellSlot[size.x, size.y];
            for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                _cells[x, y] = new CellSlot(new Vector2Int(x, y));
        }

        public CellSlot Get(Vector2Int pos) => _cells[pos.x, pos.y];

        public bool IsInBounds(Vector2Int pos)
            => pos.x >= 0 && pos.x < Size.x && pos.y >= 0 && pos.y < Size.y;

        public IEnumerable<CellSlot> GetColumn(int x)
        {
            for (int y = 0; y < Size.y; y++)
                yield return _cells[x, y];
        }

        public IEnumerable<CellSlot> GetRow(int y)
        {
            for (int x = 0; x < Size.x; x++)
                yield return _cells[x, y];
        }

        public IEnumerable<CellSlot> AllCells()
        {
            for (int x = 0; x < Size.x; x++)
            for (int y = 0; y < Size.y; y++)
                yield return _cells[x, y];
        }
    }
}