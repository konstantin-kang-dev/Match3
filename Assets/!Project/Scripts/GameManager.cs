using UnityEngine;

namespace Game
{
    public class GameManager
    {
        readonly PlayfieldManager _playfieldManager;
        readonly PlayfieldItemsDB _playfieldItemsDB;
        public GameManager(PlayfieldManager playfieldManager, PlayfieldItemsDB itemsDB)
        {
            _playfieldManager = playfieldManager;
            _playfieldItemsDB = itemsDB;
        }

        public void Init()
        {
            _playfieldItemsDB.Init();
            _playfieldManager.Init();
            Debug.Log($"[GameManager] Initialized.");
        }
    }
}
