using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public interface IPowerUpBehaviour
    {
        UniTask Activate(ActivationContext activationContext, IBoardContext board);
    }
}