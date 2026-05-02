using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game
{   
    public class DiscoBehaviour : IPowerUpBehaviour
    {
        private readonly float _activationDuration;
        private readonly IVfxService _vfxService;
        private readonly PowerUpAnimator _animator;

        public DiscoBehaviour(float activationDuration, IVfxService vfxService, PowerUpAnimator animator)
        {
            _activationDuration = activationDuration; 
            _vfxService = vfxService;
            _animator = animator;
        }

        public async UniTask Activate(ActivationContext activationContext, IBoardContext context)
        {
            await _animator.PlayDiscoActivation(((PlayfieldItem)activationContext.Self).View);
            
            var color = activationContext.SwappedColor ?? context.GetDominantColor();
            if (color == null) return;
    
            var cells = context.GetCellsByColor(color.Value).ToList();

            float markInterval = _activationDuration / cells.Count;
            
            foreach (var cell in cells)
            {
                await UniTask.WaitForSeconds(markInterval);
                await context.DestroyCells(new []{cell}, DestroyMode.Instant);
            }
            
        }
    }
}