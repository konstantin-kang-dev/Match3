using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class GameManager
    {
        private readonly GridManager _gridManager;
        private readonly PlayerProgressionManager _playerProgressionManager;
        private readonly PlayfieldItemsDB _playfieldItemsDB;
        private readonly PlayfieldManager _playfieldManager;
        private readonly PlayfieldVfxDB _playfieldVfxDB;
        private readonly IVfxService _vfxService;

        public GameManager(
            PlayfieldManager playfieldManager,
            PlayfieldItemsDB itemsDB,
            GridManager gridManager,
            PlayerProgressionManager playerProgressionManager,
            PlayfieldVfxDB playfieldVfxDB,
            IVfxService vfxService
        )
        {
            _playfieldManager = playfieldManager;
            _playfieldItemsDB = itemsDB;
            _gridManager = gridManager;
            _playerProgressionManager = playerProgressionManager;
            _playfieldVfxDB =  playfieldVfxDB;
            _vfxService = vfxService;
        }

        public void Init()
        {
            _playfieldItemsDB.Init();
            _playfieldVfxDB.Init();
            _gridManager.Init();
            _playfieldManager.Init().Forget();
            _playerProgressionManager.Init();
            _vfxService.Init(_gridManager.GridCellsContainer);
            Debug.Log("[GameManager] Initialized.");
        }

        public void SetSpeed(float speed)
        {
            Time.timeScale = speed;
        }
    }
}