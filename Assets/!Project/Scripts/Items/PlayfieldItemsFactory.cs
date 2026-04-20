using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VContainer;

namespace Game
{
    public class PlayfieldItemsFactory
    {
        readonly IObjectResolver _resolver;
        readonly PlayfieldItemView _playfieldItemVisuals;
        readonly PlayfieldItemsDB _playfieldItemsDB;

        public PlayfieldItemsFactory(IObjectResolver resolver, PlayfieldItemView visualsPrefab, PlayfieldItemsDB itemsDB)
        {
            _resolver = resolver;
            _playfieldItemVisuals = visualsPrefab;
            _playfieldItemsDB = itemsDB;
        }

        public PlayfieldItemPresenter SpawnItem(PlayfieldItemType itemType, Transform parent)
        {
            PlayfieldItemConfig config = _playfieldItemsDB.GetConfigByType(itemType);
            PlayfieldItemView visuals = GameObject.Instantiate(_playfieldItemVisuals, parent);

            var presenter = _resolver.Resolve<PlayfieldItemPresenter>();
            presenter.Init(config, visuals);
            return presenter;
        }
    }
}