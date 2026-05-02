using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Game.Utils;
using UnityEngine;

namespace Game
{    
    public enum RocketOrientation
    {
        Horizontal,
        Vertical,
    }
    
    public class RocketBehaviour : IPowerUpBehaviour
    {
        public readonly RocketOrientation Orientation;
        private readonly IVfxService _vfxService;
        private readonly PowerUpAnimator _animator;
        
        public RocketBehaviour(RocketOrientation orientation, IVfxService vfxService, PowerUpAnimator animator)
        {
            Orientation = orientation;
            _vfxService = vfxService;
            _animator = animator;
        }
        
        public async UniTask Activate(ActivationContext activationContext, IBoardContext context)
        {
            await _animator.PlayRocketActivation(((PlayfieldItem)activationContext.Self).View);
            
            var origin = activationContext.Origin;
    
            var vfxType = Orientation == RocketOrientation.Horizontal
                ? PlayfieldVfxType.RocketActivateHorizontal
                : PlayfieldVfxType.RocketActivateVertical;

            _vfxService.PlayAtCell(vfxType, origin);

            if (Orientation == RocketOrientation.Horizontal)
                await DestroyHorizontalWaves(origin, context);
            else
                await DestroyVerticalWaves(origin, context);
        }
        
        async UniTask DestroyHorizontalWaves(Vector2Int origin, IBoardContext context)
        {
            int maxDist = Mathf.Max(origin.x, context.Size.x - origin.x - 1);
            var waveTasks = new List<UniTask>();
    
            for (int dist = 0; dist <= maxDist; dist++)
            {
                var cells = new List<Vector2Int>();
                var left = new Vector2Int(origin.x - dist, origin.y);
                var right = new Vector2Int(origin.x + dist, origin.y);
                if (context.IsValidCell(left)) cells.Add(left);
                if (dist != 0 && context.IsValidCell(right)) cells.Add(right);

                waveTasks.Add(FireDestroyWithDelayAsync(cells, context, dist * 0.05f));
            }

            await UniTask.WhenAll(waveTasks);
        }
        
        async UniTask DestroyVerticalWaves(Vector2Int origin, IBoardContext context)
        {
            int maxDist = Mathf.Max(origin.y, context.Size.y - origin.y - 1);
            var waveTasks = new List<UniTask>();

            for (int dist = 0; dist <= maxDist; dist++)
            {
                var cells = new List<Vector2Int>();
                var down = new Vector2Int(origin.x, origin.y - dist);
                var up = new Vector2Int(origin.x, origin.y + dist);
                if (context.IsValidCell(down)) cells.Add(down);
                if (dist != 0 && context.IsValidCell(up)) cells.Add(up);

                waveTasks.Add(FireDestroyWithDelayAsync(cells, context, dist * 0.05f));
            }

            await UniTask.WhenAll(waveTasks);
        }

        async UniTask FireDestroyWithDelayAsync(List<Vector2Int> cells, IBoardContext context, float delay)
        {
            if (delay > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
            await context.DestroyCells(cells, DestroyMode.Instant);
        }
    }
}