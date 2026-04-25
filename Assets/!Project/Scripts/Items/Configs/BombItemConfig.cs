using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game.Configs
{

    [CreateAssetMenu(menuName = "GameData/Items/Bomb")]
    public class BombItemConfig : PlayfieldItemConfig
    {
        [SerializeField] int _explosionRadius = 2;
        public int ExplosionRadius => _explosionRadius;
    }
}