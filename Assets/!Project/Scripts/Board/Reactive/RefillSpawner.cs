using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
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
        readonly BoardActivityTracker _tracker;

        
        
        
        const float TICK_INTERVAL = 0.08f;

        CancellationTokenSource _cts;

        public RefillSpawner(
            BoardState board,
            PlayfieldItemsFactory factory,
            GridManager gridManager,
            BoardActivityTracker tracker)
        {
            _board = board;
            _factory = factory;
            _gridManager = gridManager;
            _tracker = tracker;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            TickLoop(_cts.Token).Forget();
        }

        async UniTaskVoid TickLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(TICK_INTERVAL), cancellationToken: token);

                if (_tracker.IsFrozen) continue;

                for (int x = 0; x < _board.Size.x; x++)
                    TrySpawnTopOfColumn(x);
            }
        }

        void TrySpawnTopOfColumn(int columnIndex)
        {
            int topY = _board.Size.y - 1;
            var topSlot = _board.Get(new Vector2Int(columnIndex, topY));

            
            
            if (topSlot.State != CellState.Empty) return;

            var color = GetTypeWithoutMatch(topSlot.Position);
            var item = _factory.SpawnColored(color, _gridManager.PlayfieldItemsContainer);

            Vector2Int virtualSourceCell = new Vector2Int(columnIndex, _board.Size.y);
            Vector2 startWorldPos = _gridManager.GetPositionForCell(virtualSourceCell);
            item.MoveTo(startWorldPos, MoveAnimationType.None);
            item.PlaySpawnAnimation();

            topSlot.SetFalling(item, virtualSourceCell);
        }

        PlayfieldItemColorType GetTypeWithoutMatch(Vector2Int pos)
        {
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
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}