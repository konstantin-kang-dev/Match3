using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(menuName = "GameData/Items/Balloon")]
    public class BalloonItemConfig : PlayfieldItemConfig
    {
        [SerializeField] private float _flySpeed = 8f;
        [SerializeField] private int _targetsCount = 1;

        public float FlySpeed => _flySpeed;
        public int TargetsCount => _targetsCount;
    }
}