using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class PlayfieldAnimator
    {
        private const float FallInterval = 0.1f;
        private readonly GridManager _gridManager;

        public PlayfieldAnimator(GridManager gridManager)
        {
            _gridManager = gridManager;
        }

        public async UniTask AnimateFall(List<CellMovement> movements)
        {
            var byColumn = movements
                .GroupBy(m => m.ToCell.x)
                .ToDictionary(g => g.Key, g => g.OrderBy(m => m.ToCell.y).ToList());

            foreach (var movementData in movements)
            {
                if (movementData.IsNew)
                {
                }

                var startPos = _gridManager.GetPositionForCell(movementData.FromCell);
                movementData.Item.MoveTo(startPos, MoveAnimationType.None);
            }

            var tasks = byColumn.Values.Select(AnimateColumn);
            await UniTask.WhenAll(tasks);
        }

        private async UniTask AnimateColumn(List<CellMovement> columnMoves)
        {
            foreach (var movementData in columnMoves)
            {
                var targetPos = _gridManager.GetPositionForCell(movementData.ToCell);
                movementData.Item.MoveTo(targetPos, MoveAnimationType.Bounce);
                if(movementData.IsNew) movementData.Item.SetVisibility(true);

                await UniTask.Delay(TimeSpan.FromSeconds(FallInterval));
            }
        }

        public void MoveItems(List<CellMovement> movements)
        {
            foreach (var movementData in movements)
            {
                var targetPos = _gridManager.GetPositionForCell(movementData.ToCell);
                movementData.Item.MoveTo(targetPos);
            }
        }
    }
}