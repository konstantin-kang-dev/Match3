using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(menuName = "GameData/Items/Plane")]
    public class PlaneItemConfig : PlayfieldItemConfig
    {
        [SerializeField] private float _flySpeed = 8f;
        [SerializeField] private int _targetsCount = 1;

        public float FlySpeed => _flySpeed;
        public int TargetsCount => _targetsCount;
    }
}