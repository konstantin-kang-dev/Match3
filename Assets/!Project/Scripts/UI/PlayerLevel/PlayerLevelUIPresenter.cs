using System;
using R3;
using VContainer.Unity;

namespace Game
{
    public class PlayerLevelUIPresenter : IStartable, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly PlayerProgressionManager _playerProgressionManager;
        private readonly PlayerLevelUIView _view;

        public PlayerLevelUIPresenter(PlayerProgressionManager playerProgressionManager, PlayerLevelUIView view)
        {
            _playerProgressionManager = playerProgressionManager;
            _view = view;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void Start()
        {
            _playerProgressionManager.PlayerLevel.Subscribe(HandlePlayerLevelChange).AddTo(_disposables);
            _playerProgressionManager.PlayerExp.Subscribe(HandlePlayerExpChange).AddTo(_disposables);
        }

        private void HandlePlayerLevelChange(int level)
        {
            _view.SetLevel(level);
        }

        private void HandlePlayerExpChange((float current, float required) kvp)
        {
            _view.SetExp(kvp.current, kvp.required);
        }
    }
}