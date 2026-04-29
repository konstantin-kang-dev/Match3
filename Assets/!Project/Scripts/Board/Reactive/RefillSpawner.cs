using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;
using VContainer.Unity;

namespace Game
{
    public class RefillSpawner : IStartable, IDisposable
    {
        readonly BoardState _board;
        readonly PlayfieldItemsFactory _factory;
        readonly GridManager _gridManager;
        readonly ColumnsCoordinator _coordinator;
        readonly BoardActivityTracker _tracker;
        readonly CompositeDisposable _disposables = new();

        public RefillSpawner(
            BoardState board,
            PlayfieldItemsFactory factory,
            GridManager gridManager,
            ColumnsCoordinator coordinator,
            BoardActivityTracker tracker)
        {
            _board = board;
            _factory = factory;
            _gridManager = gridManager;
            _coordinator = coordinator;
            _tracker = tracker;
        }

        public void Start()
        {
            _coordinator.OnColumnNeedsRefill
                .Subscribe(SpawnInColumn)
                .AddTo(_disposables);
        }

        void SpawnInColumn(int columnIndex)
        {
            if (_tracker.IsFrozen) 
            {
                return;
            }
            var topEmpty = FindTopEmpty(columnIndex);
            if (topEmpty == null) 
            {
                return;
            }

            // Спавним фишку
            var color = GetTypeWithoutMatch(topEmpty.Position);
            var item = _factory.SpawnColored(color, _gridManager.PlayfieldItemsContainer);
            item.MarkRefillFalling(true);
            
            // Стартовая позиция — над доской
            Vector2Int virtualSourceCell = new Vector2Int(columnIndex, _board.Size.y);
            Vector2 startWorldPos = _gridManager.GetPositionForCell(virtualSourceCell);
            item.MoveTo(startWorldPos, MoveAnimationType.None);
            item.PlaySpawnAnimation();

            // Запускаем падение через слот
            topEmpty.SetFalling(item, virtualSourceCell);
            Debug.Log($"[RefillSpawner] Spawned cell in y={virtualSourceCell}");
        }

        CellSlot FindTopEmpty(int columnIndex)
        {
            // Ищем самую высокую Empty клетку, выше которой нет Falling/Occupied
            for (int y = _board.Size.y - 1; y >= 0; y--)
            {
                var slot = _board.Get(new Vector2Int(columnIndex, y));
                if (slot.State == CellState.Empty)
                    return slot;
            }
            return null;
        }

        PlayfieldItemColorType GetTypeWithoutMatch(Vector2Int pos)
        {
            // Старая логика из BoardFiller — учёт двух соседей слева и снизу
            var forbidden = new HashSet<PlayfieldItemColorType>();
            int x = pos.x;
            int y = pos.y;

            if (x >= 2)
            {
                var left1 = _board.Get(new Vector2Int(x - 1, y)).Item;
                var left2 = _board.Get(new Vector2Int(x - 2, y)).Item;
                if (HaveSameColor(left1, left2))
                    forbidden.Add(left1.Color.Value);
            }

            if (y >= 2)
            {
                var down1 = _board.Get(new Vector2Int(x, y - 1)).Item;
                var down2 = _board.Get(new Vector2Int(x, y - 2)).Item;
                if (HaveSameColor(down1, down2))
                    forbidden.Add(down1.Color.Value);
            }

            // Защита от квадрата 2×2
            if (x >= 1 && y >= 1)
            {
                var left = _board.Get(new Vector2Int(x - 1, y)).Item;
                var down = _board.Get(new Vector2Int(x, y - 1)).Item;
                var diag = _board.Get(new Vector2Int(x - 1, y - 1)).Item;
                if (HaveSameColor(left, down) && HaveSameColor(left, diag))
                    forbidden.Add(left.Color.Value);
            }

            return ProjectUtils.GetRandomPlayfieldItemColorTypeExcluding(forbidden);
        }

        bool HaveSameColor(PlayfieldItem a, PlayfieldItem b)
        {
            return a != null && b != null
                && a.Color.HasValue && b.Color.HasValue
                && a.Color.Value == b.Color.Value;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}