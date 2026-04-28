using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public interface IVfxService
    {
        void Init(Transform container);
        void Play(PlayfieldVfxType type, Vector2 position, Transform overrideParent = null);
        void PlayAtCell(PlayfieldVfxType type, Vector2Int cell);
    }
}