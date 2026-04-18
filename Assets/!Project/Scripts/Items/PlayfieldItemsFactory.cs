using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game
{
    public class PlayfieldItemsFactory
    {
        readonly PlayfieldItemVisuals _playfieldItemVisuals;
        readonly PlayfieldItemsDB _playfieldItemsDB;
        public PlayfieldItemsFactory(PlayfieldItemVisuals visualsPrefab, PlayfieldItemsDB itemsDB)
        {
            _playfieldItemVisuals = visualsPrefab;
            _playfieldItemsDB = itemsDB;
        }

        public PlayfieldItemPresenter SpawnItem(PlayfieldItemType itemType, Transform parent)
        {
            PlayfieldItemConfig config = _playfieldItemsDB.GetConfigByType(itemType);

            PlayfieldItemPresenter presenter = new PlayfieldItemPresenter();
            PlayfieldItemVisuals visuals = GameObject.Instantiate(_playfieldItemVisuals, parent);
            presenter.Init(config, visuals);

            return presenter;
        }
    }
}