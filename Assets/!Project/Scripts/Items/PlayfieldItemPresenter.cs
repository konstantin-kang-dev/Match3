using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Game
{
    public class PlayfieldItemPresenter
    {
        public PlayfieldItemModel Model { get; private set;  }
        public PlayfieldItemVisuals Visuals { get; private set; }

        public PlayfieldItemPresenter()
        {

        }

        public void Init(PlayfieldItemConfig config, PlayfieldItemVisuals visuals)
        {
            Model = new PlayfieldItemModel();
            Visuals = visuals;

            Model.Init(config);
            Visuals.Init(config);
        }
    }
}
