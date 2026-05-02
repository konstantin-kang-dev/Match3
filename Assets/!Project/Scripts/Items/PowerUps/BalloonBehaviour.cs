using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class BalloonBehaviour : IPowerUpBehaviour
    {
        private readonly float _flySpeed;
        private readonly int _targetsCount;
        private readonly IVfxService _vfxService;
        private readonly PowerUpAnimator _animator;
        private readonly BoardActivityTracker _tracker;

        public BalloonBehaviour(
            float flySpeed,
            int targetsCount,
            IVfxService vfxService,
            PowerUpAnimator animator,
            BoardActivityTracker tracker)
        {
            _flySpeed = flySpeed;
            _targetsCount = targetsCount;
            _vfxService = vfxService;
            _animator = animator;
            _tracker = tracker;
        }

        public bool SelfDestroys => true;

        public UniTask Activate(ActivationContext activationContext, IBoardContext context)
        {
            Vector2Int origin = activationContext.Origin;

            var crossCells = new[]
            {
                origin + Vector2Int.up,
                origin + Vector2Int.down,
                origin + Vector2Int.left,
                origin + Vector2Int.right,
            }.Where(context.IsValidCell).ToArray();

            FlyAndExplode(((PlayfieldItem)activationContext.Self), crossCells, context).Forget();

            return UniTask.CompletedTask;
        }

        async UniTaskVoid FlyAndExplode(PlayfieldItem balloon, Vector2Int[] crossCells, IBoardContext context)
        {
            using (_tracker.BeginActivity())
            {
                await context.DestroyCells(crossCells, DestroyMode.Instant);

                Vector2Int target = context.GetRandomCell();
                await _animator.PlayBalloonActivation(balloon.View, target);

                _vfxService.PlayAtCell(PlayfieldVfxType.BalloonImpact, target);
                await context.DestroyCells(GetRingCells(target, 1, context), DestroyMode.Instant);
                
                balloon.DestroyItem(DestroyMode.Instant);
            }
        }

        List<Vector2Int> GetRingCells(Vector2Int center, int ring, IBoardContext context)
        {
            var cells = new List<Vector2Int>();
            if (ring == 0) { cells.Add(center); return cells; }

            for (int dx = -ring; dx <= ring; dx++)
            for (int dy = -ring; dy <= ring; dy++)
            {
                if (Mathf.Abs(dx) != ring && Mathf.Abs(dy) != ring) continue;

                var cell = new Vector2Int(center.x + dx, center.y + dy);
                if (context.IsValidCell(cell))
                    cells.Add(cell);
            }
            return cells;
        }
    }
}