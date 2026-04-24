using R3;
using System;
using UnityEngine;

namespace Game
{
    public class PlayfieldItem : IDisposable
    {
        public PlayfieldItemType Type { get; private set; }
        public Vector2Int OccupiedCell { get; private set; }

        PlayfieldItemView _view;

        readonly PlayfieldManager _playfieldManager;
        readonly GridManager _gridManager;
        public PlayfieldItem(PlayfieldManager playfieldManager, GridManager gridManager)
        {
            _playfieldManager = playfieldManager;
            _gridManager = gridManager;
        }

        public void Init(PlayfieldItemConfig config, PlayfieldItemView visuals)
        {
            Type = config.ItemType;
            _view = visuals;

            _view.Init(config);
            _view.SetSize(_gridManager.CellSize);

            _view.OnSwapRequest.Subscribe((Vector2Int direction) =>
            {
                HandleSwapRequest(direction);
            });

            _view.OnDestroyed.Subscribe(isDestroyed =>
            {
                Dispose();
            });
        }

        void HandleSwapRequest(Vector2Int direction)
        {
            if (_playfieldManager.IsMatching) return;
            _playfieldManager.TrySwap(OccupiedCell, direction);
        }

        public void OccupyCell(Vector2Int cell)
        {
            OccupiedCell = cell;
        }

        public void MoveTo(Vector2 targetPos, MoveAnimationType moveAnimationType = MoveAnimationType.Move)
        {
            _view.MoveTo(targetPos, moveAnimationType);
        }

        public void SetVisibility(bool visible)
        {
            _view.SetVisibility(visible);
        }

        public void DestroyItem()
        {
            _view.AnimateDestroy();
        }

        public void Dispose()
        {

        }
    }
}
