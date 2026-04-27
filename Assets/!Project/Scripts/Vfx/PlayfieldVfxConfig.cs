using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(menuName = "GameData/PlayfieldVfx", fileName = "vfx_config")]
    public class PlayfieldVfxConfig : SerializedScriptableObject
    {
        [SerializeField] PlayfieldVfxType _type;
        [SerializeField] GameObject _prefab;
        [SerializeField] float _duration = 1f;

        public PlayfieldVfxType Type => _type;
        public GameObject Prefab => _prefab;
        public float Duration => _duration;
    }
}