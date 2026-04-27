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

        public PlaneBehaviour(float flySpeed, int targetsCount, IVfxService vfxService)
        {
            _flySpeed = flySpeed;
            _targetsCount = targetsCount;
            _vfxService = vfxService;
        }

        public async UniTask Activate(ActivationContext activationContext, IBoardContext context)
        {
            Vector2Int origin = activationContext.Origin;
            for (int i = 0; i < _targetsCount; i++)
            {
                var target = context.FindRandomColoredCell();
                if (!target.HasValue) break;

                await context.DestroyCells(new[] { target.Value });
            }
        }
    }
}