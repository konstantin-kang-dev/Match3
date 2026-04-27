using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(menuName = "GameData/Items/Colored")]
    public class ColoredItemConfig : PlayfieldItemConfig
    {
        [SerializeField] private PlayfieldItemColorType _color;
        public PlayfieldItemColorType Color => _color;
    }
}