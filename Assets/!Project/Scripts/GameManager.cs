using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class GameManager
    {
        readonly PlayfieldManager _playfieldManager;
        readonly PlayfieldItemsDB _playfieldItemsDB;
        readonly GridManager _gridManager;
        readonly PlayerProgressionManager _playerProgressionManager;

        public GameManager(
            PlayfieldManager playfieldManager,
            PlayfieldItemsDB itemsDB,
            GridManager gridManager,
            PlayerProgressionManager playerProgressionManager
            )
        {
            _playfieldManager = playfieldManager;
            _playfieldItemsDB = itemsDB;
            _gridManager = gridManager;
            _playerProgressionManager = playerProgressionManager;
        }

        public void Init()
        {
            _playerProgressionManager.Init();
            _playfieldItemsDB.Init();
            _gridManager.Init();
            _playfieldManager.Init().Forget();
            Debug.Log($"[GameManager] Initialized.");
        }

        public void SetSpeed(float speed)
        {
            Time.timeScale = speed;
        }
    }
}
