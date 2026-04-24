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

        public PlayfieldItem SpawnItem(PlayfieldItemType itemType, Transform parent)
        {
            PlayfieldItemConfig config = _playfieldItemsDB.GetConfigByType(itemType);
            PlayfieldItemView view = GameObject.Instantiate(config.Prefab, parent).GetComponent<PlayfieldItemView>();

            var presenter = _resolver.Resolve<PlayfieldItem>();
            presenter.Init(config, view);
            return presenter;
        }
    }
}