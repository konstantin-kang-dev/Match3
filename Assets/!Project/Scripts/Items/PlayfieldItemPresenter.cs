using R3;
using System;
using UnityEngine;

namespace Game
{
    public class PlayfieldItemPresenter : IDisposable
    {
        public PlayfieldItemModel Model { get; private set;  }
        public PlayfieldItemView View { get; private set; }

        PlayfieldManager _playfieldManager;
        GridManager _gridManager;
        public PlayfieldItemPresenter(PlayfieldManager playfieldManager, GridManager gridManager)
        {
            _playfieldManager = playfieldManager;
            _gridManager = gridManager;
        }

        public void Init(PlayfieldItemConfig config, PlayfieldItemView visuals)
        {
            Model = new PlayfieldItemModel();
            View = visuals;

            Model.Init(config);
            View.Init(config);

            View.OnSwapRequest.Subscribe((Vector2Int direction) =>
            {
                HandleSwapRequest(direction);
            });

            View.OnDestroyed.Subscribe(isDestroyed =>
            {
                Dispose();
            });
        }

        void HandleSwapRequest(Vector2Int direction)
        {
            _playfieldManager.TrySwap(Model.OccupiedCell, direction);
        }

        public void OccupyCell(Vector2Int nextCell, bool animate = false)
        {
            Model.OccupyCell(nextCell);

            if(animate)
            {
                Vector2 targetPos = _gridManager.GetPositionForCell(nextCell);
                View.MoveTo(targetPos, 0.25f);
            }

        }

        public void DestroyItem()
        {
            View.AnimateDestroy();
        }

        public void Dispose()
        {

        }
    }
}
