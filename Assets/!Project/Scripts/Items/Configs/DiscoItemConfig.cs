using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(menuName = "GameData/Items/Disco")]
    public class DiscoItemConfig : PlayfieldItemConfig
    {
        [SerializeField] private float _activationDuration = 12f;

        public float ActivationDuration => _activationDuration;
    }
}