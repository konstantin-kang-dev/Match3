using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(menuName = "GameData/Items/Bomb")]
    public class BombItemConfig : PlayfieldItemConfig
    {
        [SerializeField] private int _explosionRadius = 2;
        public int ExplosionRadius => _explosionRadius;
    }
}