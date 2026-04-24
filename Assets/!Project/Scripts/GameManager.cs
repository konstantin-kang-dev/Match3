using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class GameManager
    {
        readonly PlayfieldManager _playfieldManager;
        readonly PlayfieldItemsDB _playfieldItemsDB;
        readonly GridManager _gridManager;
        public GameManager(PlayfieldManager playfieldManager, PlayfieldItemsDB itemsDB, GridManager gridManager)
        {
            _playfieldManager = playfieldManager;
            _playfieldItemsDB = itemsDB;
            _gridManager = gridManager;
        }

        public void Init()
        {
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
