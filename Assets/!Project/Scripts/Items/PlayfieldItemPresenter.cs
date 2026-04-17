using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Game
{
    public class PlayfieldItemPresenter
    {
        public PlayfieldItemModel Model { get; private set;  }

        public PlayfieldItemPresenter()
        {

        }

        public void Init(PlayfieldItemConfig config)
        {
            Model.Init(config);
        }

    }
}
