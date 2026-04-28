using Cysharp.Threading.Tasks;
using Game.Configs;
using UnityEngine;

namespace Game
{
    public class PlayfieldVfxService : IVfxService
    {
        private readonly PlayfieldVfxDB _db;
        private readonly GridManager _gridManager;
        private Transform _container;

        public PlayfieldVfxService(PlayfieldVfxDB db, GridManager gridManager)
        {
            _db = db;
            _gridManager = gridManager;
        }

        public void Init(Transform container)
        {
            _container = container;
        }

        public void Play(PlayfieldVfxType type, Vector2 position, Transform overrideParent = null)
        {
            if (!_db.TryGet(type, out var config))
            {
                Debug.LogWarning($"[PlayfieldVfxService] No vfx for type {type}");
                return;
            }
            
            Transform parent = overrideParent != null ?  overrideParent : _container; 
            var instance = Object.Instantiate(config.Prefab, parent);
            instance.transform.position = position;

            if (instance.TryGetComponent<ParticleSystem>(out ParticleSystem vfx))
            {
                vfx.Play();
            }

            Object.Destroy(instance, config.Duration);
        }

        public void PlayAtCell(PlayfieldVfxType type, Vector2Int cell)
        {
            if (!_db.TryGet(type, out var config))
            {
                Debug.LogWarning($"[PlayfieldVfxService] No vfx for type {type}");
                return;
            }

            Vector2 position = _gridManager.GetPositionForCell(cell);
            
            var instance = Object.Instantiate(config.Prefab, _container);
            instance.transform.localPosition = position;

            if (instance.TryGetComponent<ParticleSystem>(out ParticleSystem vfx))
            {
                vfx.Play();
            }

            Object.Destroy(instance, config.Duration);
        }
    }
}