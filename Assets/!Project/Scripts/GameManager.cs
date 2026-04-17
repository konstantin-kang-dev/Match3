using UnityEngine;

namespace Game
{
    public partial class GameManager
    {
        readonly PlayfieldManager _playfieldManager;

        public GameManager(PlayfieldManager playfieldManager)
        {
            _playfieldManager = playfieldManager;
        }

        public void Init()
        {
            _playfieldManager.Init();
        }
    }
}
