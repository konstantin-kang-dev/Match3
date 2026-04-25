
using UnityEngine;

namespace Game.Configs
{


    [CreateAssetMenu(menuName = "GameData/Items/Plane")]
    public class PlaneItemConfig : PlayfieldItemConfig
    {
        [SerializeField] float _flySpeed = 8f;
        [SerializeField] int _targetsCount = 1;

        public float FlySpeed => _flySpeed;
        public int TargetsCount => _targetsCount;
    }
}