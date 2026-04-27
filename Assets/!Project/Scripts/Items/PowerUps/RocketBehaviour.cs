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
        
        public RocketBehaviour(RocketOrientation orientation, IVfxService vfxService)
        {
            Orientation = orientation;
            _vfxService = vfxService;
        }
        
        public async UniTask Activate(ActivationContext activationContext, IBoardContext context)
        {
            var origin = activationContext.Origin;
    
            var vfxType = Orientation == RocketOrientation.Horizontal
                ? PlayfieldVfxType.RocketActivateHorizontal
                : PlayfieldVfxType.RocketActivateVertical;

            Vector2 pos = context.GetWorldPosition(origin);
            _vfxService.Play(vfxType, pos);

            if (Orientation == RocketOrientation.Horizontal)
                await DestroyHorizontalWaves(origin, context);
            else
                await DestroyVerticalWaves(origin, context);
        }
        
        async UniTask DestroyHorizontalWaves(Vector2Int origin, IBoardContext context)
        {
            int maxDist = Mathf.Max(origin.x, context.Size.x - origin.x - 1);
            for (int dist = 0; dist <= maxDist; dist++)
            {
                var cells = new List<Vector2Int>();
                var left = new Vector2Int(origin.x - dist, origin.y);
                var right = new Vector2Int(origin.x + dist, origin.y);
                if (context.IsValidCell(left)) cells.Add(left);
                if (dist != 0 && context.IsValidCell(right)) cells.Add(right);
                
                FireDestroyAsync(cells, context).Forget();
        
                await UniTask.Delay(TimeSpan.FromSeconds(0.04f));
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(ProjectConstants.ITEM_DESTROY_ANIM_DURATION));
        }
        
        async UniTask DestroyVerticalWaves(Vector2Int origin, IBoardContext context)
        {
            int maxDist = Mathf.Max(origin.y, context.Size.y - origin.y - 1);
            for (int dist = 0; dist <= maxDist; dist++)
            {
                var cells = new List<Vector2Int>();
                var left = new Vector2Int(origin.x, origin.y - dist);
                var right = new Vector2Int(origin.x, origin.y + dist);
                if (context.IsValidCell(left)) cells.Add(left);
                if (dist != 0 && context.IsValidCell(right)) cells.Add(right);
                
                FireDestroyAsync(cells, context).Forget();
        
                await UniTask.Delay(TimeSpan.FromSeconds(0.04f));
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(ProjectConstants.ITEM_DESTROY_ANIM_DURATION));
        }
        
        UniTask FireDestroyAsync(List<Vector2Int> cells, IBoardContext context)
            => context.DestroyCells(cells);
    }
}