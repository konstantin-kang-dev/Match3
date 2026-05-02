using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Configs;
using R3;
using UnityEngine;

namespace Game
{
    public class PlayfieldItem : IBoardItem, IDisposable
    {
        private readonly GridManager _gridManager;
        private readonly ISwapRequester _swapRequester;

        private readonly CompositeDisposable _disposables = new();
        private readonly Subject<bool> _onDestroyed = new();
        public Observable<bool> OnDestroyed => _onDestroyed.AsObservable();

        private bool _disposed;
        public bool IsDisposed => _disposed;
        public bool IsActivating { get; private set; }

        public void SetActivating(bool value)
        {
            IsActivating = value;
        }

        public PlayfieldItem(ISwapRequester swapRequester, GridManager gridManager)
        {
            _swapRequester = swapRequester;
            _gridManager = gridManager;
        }

        public PlayfieldItemView View { get; private set; }
        public PlayfieldItemKind Kind { get; private set; }
        public PlayfieldItemColorType? Color { get; private set; }
        public IPowerUpBehaviour PowerUp { get; private set; }
        public Vector2Int OccupiedCell { get; private set; }

        public bool IsPowerUp => PowerUp != null;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _onDestroyed.OnNext(true);
            _onDestroyed.Dispose();
            _disposables.Dispose();
        }

        public void Init(PlayfieldItemConfig config, PlayfieldItemView view, IPowerUpBehaviour powerUp)
        {
            Kind = config.Kind;
            Color = config is ColoredItemConfig colored ? colored.Color : null;
            PowerUp = powerUp;

            View = view;
            View.Init(config);
            View.SetSize(_gridManager.CellSize);

            if (Kind == PlayfieldItemKind.Rocket)
            {
                RocketBehaviour rocketBehaviour = powerUp as RocketBehaviour;
                if (rocketBehaviour != null && rocketBehaviour.Orientation == RocketOrientation.Vertical)
                {
                    View.SetAlternativeSprite(true);
                }
            }

            View.OnSwapRequest
                .Subscribe(HandleSwapRequest)
                .AddTo(_disposables);

            View.OnDestroyed
                .Subscribe(_ => Dispose())
                .AddTo(_disposables);
        }

        private void HandleSwapRequest(Vector2Int direction)
        {
            _swapRequester.TrySwap(OccupiedCell, direction);
        }

        public void OccupyCell(Vector2Int cell)
        {
            OccupiedCell = cell;
        }

        public void DestroyItem(DestroyMode mode)
        {
            View.RectTransform.DOKill();
            View.Destroy(mode);
        }
    }
}