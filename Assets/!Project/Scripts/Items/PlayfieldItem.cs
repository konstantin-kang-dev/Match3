using Game.Configs;
using R3;
using System;
using UnityEngine;

namespace Game
{
    public class PlayfieldItem : IDisposable
    {
        public PlayfieldItemKind Kind { get; private set; }
        public PlayfieldItemColorType? Color { get; private set; }
        public IPowerUpBehaviour PowerUp { get; private set; }
        public Vector2Int OccupiedCell { get; private set; }

        public bool IsPowerUp => PowerUp != null;

        PlayfieldItemView _view;

        readonly PlayfieldManager _playfieldManager;
        readonly GridManager _gridManager;

        public PlayfieldItem(PlayfieldManager playfieldManager, GridManager gridManager)
        {
            _playfieldManager = playfieldManager;
            _gridManager = gridManager;
        }

        public void Init(PlayfieldItemConfig config, PlayfieldItemView view, IPowerUpBehaviour powerUp)
        {
            Kind = config.Kind;
            Color = config is ColoredItemConfig colored ? colored.Color : null;
            PowerUp = powerUp;

            _view = view;
            _view.Init(config);
            _view.SetSize(_gridManager.CellSize);

            _view.OnSwapRequest.Subscribe(HandleSwapRequest);
            _view.OnDestroyed.Subscribe(_ => Dispose());
        }

        void HandleSwapRequest(Vector2Int direction)
        {
            if (_playfieldManager.IsMatching) return;
            _playfieldManager.TrySwap(OccupiedCell, direction);
        }

        public void OccupyCell(Vector2Int cell) => OccupiedCell = cell;

        public void MoveTo(Vector2 targetPos, MoveAnimationType anim = MoveAnimationType.Move)
            => _view.MoveTo(targetPos, anim);

        public void SetVisibility(bool visible) => _view.SetVisibility(visible);

        public void DestroyItem() => _view.AnimateDestroy();

        public void Dispose() { }
    }
}