using System;
using Cysharp.Threading.Tasks;
using Game.Configs;
using R3;
using UnityEngine;

namespace Game
{
    public class PlayfieldItem : IDisposable
    {
        private readonly GridManager _gridManager;

        private readonly ISwapRequester _swapRequester;

        private PlayfieldItemView _view;

        public PlayfieldItem(ISwapRequester swapRequester, GridManager gridManager)
        {
            _swapRequester = swapRequester;
            _gridManager = gridManager;
        }
        public RectTransform RectTransform => _view.RectTransform;
        public PlayfieldItemKind Kind { get; private set; }
        public PlayfieldItemColorType? Color { get; private set; }
        public IPowerUpBehaviour PowerUp { get; private set; }
        public Vector2Int OccupiedCell { get; private set; }
        
        private Subject<bool> _onDestroyed = new();
        public Observable<bool> OnDestroyed => _onDestroyed;

        public bool IsPowerUp => PowerUp != null;

        public void Dispose()
        {
            _onDestroyed.OnNext(true);
            _onDestroyed.Dispose();
        }

        public void Init(PlayfieldItemConfig config, PlayfieldItemView view, IPowerUpBehaviour powerUp)
        {
            Kind = config.Kind;
            Color = config is ColoredItemConfig colored ? colored.Color : null;
            PowerUp = powerUp;

            _view = view;
            _view.Init(config);
            _view.SetSize(_gridManager.CellSize);

            if (Kind == PlayfieldItemKind.Rocket)
            {
                RocketBehaviour rocketBehaviour = powerUp as RocketBehaviour;
                if (rocketBehaviour != null && rocketBehaviour.Orientation == RocketOrientation.Vertical)
                {
                    _view.SetAlternativeSprite(true);
                }
            }
            
            _view.OnSwapRequest.Subscribe(HandleSwapRequest);
            _view.OnDestroyed.Subscribe(_ => Dispose());
        }

        private void HandleSwapRequest(Vector2Int direction)
        {
            _swapRequester.TrySwap(OccupiedCell, direction);
        }

        public void OccupyCell(Vector2Int cell)
        {
            OccupiedCell = cell;
        }

        public void MoveTo(Vector2 targetPos, MoveAnimationType anim = MoveAnimationType.Move)
        {
            _view.MoveTo(targetPos, anim);
        }

        public void Hide() => _view.Hide();

        public UniTask PlaySpawnAnimation() => _view.PlaySpawnAnimation();

        public void DestroyItem(DestroyMode mode)
        {
            _view.Destroy(mode);
        }
    }
}