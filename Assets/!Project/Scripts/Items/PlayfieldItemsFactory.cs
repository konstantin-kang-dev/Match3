using System;
using Game.Configs;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Game
{
    public class PlayfieldItemsFactory
    {
        private readonly PlayfieldItemsDB _db;
        private readonly IObjectResolver _resolver;

        readonly PowerUpBehaviourFactory _behaviourFactory;
        
        public PlayfieldItemsFactory(IObjectResolver resolver, PlayfieldItemsDB itemsDB, PowerUpBehaviourFactory behaviourFactory)
        {
            _resolver = resolver;
            _db = itemsDB;
            _behaviourFactory = behaviourFactory;
        }

        public PlayfieldItem SpawnColored(PlayfieldItemColorType color, Transform parent)
        {
            var config = _db.GetColored(color);
            return SpawnFromConfig(config, null, parent);
        }

        public PlayfieldItem SpawnRocket(RocketOrientation orientation, Transform parent)
        {
            var config = _db.Get<RocketItemConfig>(PlayfieldItemKind.Rocket);
            var behaviour = _behaviourFactory.CreateRocket(orientation);
            return SpawnFromConfig(config, behaviour, parent);
        }

        public PlayfieldItem SpawnBomb(Transform parent)
        {
            var config = _db.Get<BombItemConfig>(PlayfieldItemKind.Bomb);
            var behaviour = _behaviourFactory.CreateBomb(config);
            return SpawnFromConfig(config, behaviour, parent);
        }

        public PlayfieldItem SpawnPlane(Transform parent)
        {
            var config = _db.Get<BalloonItemConfig>(PlayfieldItemKind.Balloon);
            var behaviour = _behaviourFactory.CreatePlane(config);
            return SpawnFromConfig(config, behaviour, parent);
        }

        public PlayfieldItem SpawnDisco(Transform parent)
        {
            var config = _db.Get<DiscoItemConfig>(PlayfieldItemKind.Disco);
            var behaviour = _behaviourFactory.CreateDisco(config);
            return SpawnFromConfig(config, behaviour, parent);
        }

        PlayfieldItem SpawnFromConfig(PlayfieldItemConfig config, IPowerUpBehaviour behaviour, Transform parent)
        {
            var view = Object.Instantiate(config.Prefab, parent).GetComponent<PlayfieldItemView>();
            var item = _resolver.Resolve<PlayfieldItem>();
            item.Init(config, view, behaviour);
            return item;
        }
    }
}