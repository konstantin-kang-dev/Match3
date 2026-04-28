using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game
{   
    public class DiscoBehaviour : IPowerUpBehaviour
    {
        private readonly float _beamSpeed;
        private readonly IVfxService _vfxService;
        private readonly PowerUpAnimator _animator;

        public DiscoBehaviour(float beamSpeed, IVfxService vfxService, PowerUpAnimator animator)
        {
            _beamSpeed = beamSpeed; 
            _vfxService = vfxService;
            _animator = animator;
        }

        public async UniTask Activate(ActivationContext activationContext, IBoardContext context)
        {
            await _animator.PlayDiscoActivation(activationContext.Self);
            
            var color = activationContext.SwappedColor ?? context.GetDominantColor();
            if (color == null) return;
    
            var cells = context.GetCellsByColor(color.Value);
            await context.DestroyCells(cells, DestroyMode.Instant);
        }
    }
}