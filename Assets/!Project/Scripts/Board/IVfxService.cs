using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public interface IVfxService
    {
        void Init(Transform container);
        void Play(PlayfieldVfxType type, Vector2 position, Transform overrideParent = null);
    }
}