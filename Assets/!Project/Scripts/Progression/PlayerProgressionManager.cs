using R3;
using UnityEngine;

namespace Game
{
    public class PlayerProgressionManager
    {
        private readonly PlayfieldManager _playfieldManager;
        private MatchShapesConfig _matchShapesConfig;

        private PlayerProgressionConfig _progressionConfig;

        public PlayerProgressionManager(PlayfieldManager playfieldManager)
        {
            _playfieldManager = playfieldManager;
        }

        public ReactiveProperty<int> PlayerLevel { get; } = new(1);
        public ReactiveProperty<(float current, float required)> PlayerExp { get; } = new((0f, 0f));

        public void Init()
        {
            _progressionConfig =
                Resources.Load<PlayerProgressionConfig>("PlayerProgressionConfigs/PlayerProgressionConfig");
            _matchShapesConfig = Resources.Load<MatchShapesConfig>("MatchShapesConfigs/MatchShapesConfig");
            _matchShapesConfig.CacheData();

            _playfieldManager.OnMatchResolved.Subscribe(HandleMatchResolved);

            SetLevel(1);
            SetCurrentExp(0);
        }

        private void HandleMatchResolved(MatchResolvedEvent matchEvent)
        {
            var exp = _matchShapesConfig.GetExpRewardByShapeType(matchEvent.Shape);
            AddExp(exp);
        }

        private void AddExp(float exp)
        {
            if (IsLevelUpAvailable())
                LevelUp();
            else
                SetCurrentExp(PlayerExp.Value.current + exp);
        }

        private void SetCurrentExp(float exp)
        {
            PlayerExp.Value = (exp, PlayerExp.Value.required);
        }

        private bool IsLevelUpAvailable()
        {
            return PlayerExp.Value.current >= PlayerExp.Value.required;
        }

        private void LevelUp()
        {
            SetCurrentExp(PlayerExp.Value.current - PlayerExp.Value.required);
            SetLevel(PlayerLevel.Value + 1);
        }

        private void SetLevel(int level)
        {
            PlayerLevel.Value = level;

            PlayerExp.Value = (PlayerExp.Value.current, _progressionConfig.GetRequiredExp(PlayerLevel.Value + 1));
        }
    }
}