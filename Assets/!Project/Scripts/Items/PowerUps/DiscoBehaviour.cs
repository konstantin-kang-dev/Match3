using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game
{   
    public class DiscoBehaviour : IPowerUpBehaviour
    {
        private readonly float _beamSpeed;
        private readonly IVfxService _vfxService;

        public DiscoBehaviour(float beamSpeed, IVfxService vfxService)
        {
            _beamSpeed = beamSpeed; 
            _vfxService = vfxService;
        }

        public async UniTask Activate(ActivationContext activationContext, IBoardContext context)
        {
            var color = activationContext.SwappedColor ?? context.GetDominantColor();
            if (color == null) return;
    
            var cells = context.GetCellsByColor(color.Value);
            await context.DestroyCells(cells);
        }
    }
}