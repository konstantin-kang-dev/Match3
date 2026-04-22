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
        readonly PlayfieldItemsDB _playfieldItemsDB;

        public PlayfieldItemsFactory(IObjectResolver resolver, PlayfieldItemsDB itemsDB)
        {
            _resolver = resolver;
            _playfieldItemsDB = itemsDB;
        }

        public PlayfieldItemPresenter SpawnItem(PlayfieldItemType itemType, Transform parent)
        {
            PlayfieldItemConfig config = _playfieldItemsDB.GetConfigByType(itemType);
            PlayfieldItemView view = GameObject.Instantiate(config.Prefab, parent).GetComponent<PlayfieldItemView>();

            var presenter = _resolver.Resolve<PlayfieldItemPresenter>();
            presenter.Init(config, view);
            return presenter;
        }
    }
}