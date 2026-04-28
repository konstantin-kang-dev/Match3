using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.Utils;

namespace Game
{   
    public class BombBehaviour : IPowerUpBehaviour
    {
        private readonly int _explosionRadius;
        private readonly IVfxService _vfxService;
        private readonly PowerUpAnimator _animator;
        public BombBehaviour(int explosionRadius, IVfxService vfxService, PowerUpAnimator animator)
        {
            _explosionRadius = explosionRadius;
            _vfxService = vfxService;
            _animator = animator;
        }

        public async UniTask Activate(ActivationContext activationContext, IBoardContext context)
        {
            await _animator.PlayBombActivation(activationContext.Self);
            
            var origin = activationContext.Origin;
            
            _vfxService.PlayAtCell(PlayfieldVfxType.BombActivate, origin);

            var waveTasks = new List<UniTask>();
            for (int ring = 0; ring <= _explosionRadius; ring++)
            {
                var cells = GetRingCells(activationContext.Origin, ring, context);
                waveTasks.Add(FireDestroyWithDelayAsync(cells, context, ring * 0.07f));
            }

            await UniTask.WhenAll(waveTasks);
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
        


        async UniTask FireDestroyWithDelayAsync(List<Vector2Int> cells, IBoardContext context, float delay)
        {
            if (delay > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
            await context.DestroyCells(cells, DestroyMode.Instant);
        }
    }
}