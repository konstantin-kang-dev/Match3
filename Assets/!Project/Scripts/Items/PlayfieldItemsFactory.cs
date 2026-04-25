using Game.Configs;
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

        public PlayfieldItem SpawnColored(PlayfieldItemColorType color, Transform parent)
        {
            var config = _playfieldItemsDB.GetColored(color);
            return SpawnFromConfig(config, parent);
        }

        public PlayfieldItem SpawnPowerUp(PlayfieldItemKind kind, Transform parent)
        {
            if (kind == PlayfieldItemKind.Colored)
                throw new System.ArgumentException(
                    "Use SpawnColored for colored items.", nameof(kind));

            var config = _playfieldItemsDB.Get<PlayfieldItemConfig>(kind);
            return SpawnFromConfig(config, parent);
        }

        PlayfieldItem SpawnFromConfig(PlayfieldItemConfig config, Transform parent)
        {
            var view = GameObject.Instantiate(config.Prefab, parent).GetComponent<PlayfieldItemView>();
            var item = _resolver.Resolve<PlayfieldItem>();

            var behaviour = CreatePowerUpBehaviour(config);
            item.Init(config, view, behaviour);

            return item;
        }

        IPowerUpBehaviour CreatePowerUpBehaviour(PlayfieldItemConfig config) => config switch
        {
            /*
            RocketItemConfig rocket => new RocketBehaviour(rocket.FlySpeed),
            BombItemConfig bomb => new BombBehaviour(bomb.ExplosionRadius),
            PaperPlaneItemConfig plane => new PaperPlaneBehaviour(plane.FlySpeed, plane.TargetsCount),
            RainbowBallItemConfig ball => new RainbowBallBehaviour(ball.BeamSpeed),
            ColoredItemConfig => null,
            */
            _ => null
        };
    }
}