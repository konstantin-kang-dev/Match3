using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(menuName = "GameData/Items/Disco")]
    public class DiscoItemConfig : PlayfieldItemConfig
    {
        [SerializeField] private float _beamSpeed = 12f;

        public float BeamSpeed => _beamSpeed;
    }
}