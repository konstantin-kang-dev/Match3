using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game
{   
    public class PlaneBehaviour : IPowerUpBehaviour
    {
        private readonly float _flySpeed;
        private readonly int _targetsCount;
        private readonly IVfxService _vfxService;
        private readonly PowerUpAnimator _animator;
        
        public PlaneBehaviour(float flySpeed, int targetsCount, IVfxService vfxService, PowerUpAnimator animator)
        {
            _flySpeed = flySpeed;
            _targetsCount = targetsCount;
            _vfxService = vfxService;
            _animator = animator;
        }

        public async UniTask Activate(ActivationContext activationContext, IBoardContext context)
        {
            Vector2Int origin = activationContext.Origin;
            Vector2Int? target = context.FindRandomColoredCell();
            if (target.HasValue)
            {
                await _animator.PlayPlaneActivation(activationContext.Self, target.Value);
                await context.DestroyCells(new[] { target.Value }, DestroyMode.Instant);
            }
            
            /*
            for (int i = 0; i < _targetsCount; i++)
            {
                var target = context.FindRandomColoredCell();
                if (!target.HasValue) break;

                await context.DestroyCells(new[] { target.Value }, DestroyMode.Instant);
            }
            */
        }
    }
}