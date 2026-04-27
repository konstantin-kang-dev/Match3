using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(menuName = "GameData/Items/Rocket")]
    public class RocketItemConfig : PlayfieldItemConfig
    {
        [SerializeField] private float _flySpeed = 10f;
        public float FlySpeed => _flySpeed;
    }
}