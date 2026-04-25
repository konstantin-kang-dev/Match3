using System;
using System.Collections.Generic;
using System.Text;
using VContainer.Unity;
using R3;

namespace Game
{
    public class PlayerLevelUIPresenter : IStartable, IDisposable
    {
        readonly PlayerProgressionManager _playerProgressionManager;
        readonly PlayerLevelUIView _view;

        private readonly CompositeDisposable _disposables = new();

        public PlayerLevelUIPresenter(PlayerProgressionManager playerProgressionManager, PlayerLevelUIView view)
        {
            _playerProgressionManager = playerProgressionManager;
            _view = view;
        }
        public void Start()
        {
            _playerProgressionManager.PlayerLevel.Subscribe(HandlePlayerLevelChange).AddTo(_disposables);
            _playerProgressionManager.PlayerExp.Subscribe(HandlePlayerExpChange).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        void HandlePlayerLevelChange(int level)
        {
            _view.SetLevel(level);
        }

        void HandlePlayerExpChange((float current, float required) kvp)
        {
            _view.SetExp(kvp.current, kvp.required);
        }

    }
}
