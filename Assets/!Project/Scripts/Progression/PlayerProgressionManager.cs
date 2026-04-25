using R3;
using UnityEngine;
using static UnityEngine.Audio.GeneratorInstance;

namespace Game
{
    public class PlayerProgressionManager
    {
        readonly PlayfieldManager _playfieldManager;

        PlayerProgressionConfig _progressionConfig;
        MatchShapesConfig _matchShapesConfig;
        public ReactiveProperty<int> PlayerLevel { get; private set; } = new(1);
        public ReactiveProperty<(float current, float required)> PlayerExp { get; private set; } = new((0f, 0f));
        public PlayerProgressionManager(PlayfieldManager playfieldManager)
        {
            _playfieldManager = playfieldManager;
        }

        public void Init()
        {
            _progressionConfig = Resources.Load<PlayerProgressionConfig>("PlayerProgressionConfigs/PlayerProgressionConfig");
            _matchShapesConfig = Resources.Load<MatchShapesConfig>("MatchShapesConfigs/MatchShapesConfig");
            _matchShapesConfig.CacheData();

            _playfieldManager.OnMatchResolved.Subscribe(HandleMatchResolved);

            SetLevel(1);
            SetCurrentExp(0);
        }

        void HandleMatchResolved(MatchResolvedEvent matchEvent)
        {
            float exp = _matchShapesConfig.GetExpRewardByShapeType(matchEvent.Shape);
            AddExp(exp);
        }

        void AddExp(float exp)
        {
            if (IsLevelUpAvailable())
            {
                LevelUp();
            }
            else
            {
                SetCurrentExp(PlayerExp.Value.current + exp);
            }
        }

        void SetCurrentExp(float exp)
        {
            PlayerExp.Value = (exp, PlayerExp.Value.required);
        }

        bool IsLevelUpAvailable()
        {
            return PlayerExp.Value.current >= PlayerExp.Value.required;
        }

        void LevelUp()
        {
            SetCurrentExp(PlayerExp.Value.current - PlayerExp.Value.required);
            SetLevel(PlayerLevel.Value + 1);
        }

        void SetLevel(int level)
        {
            PlayerLevel.Value = level;

            PlayerExp.Value = (PlayerExp.Value.current, _progressionConfig.GetRequiredExp(PlayerLevel.Value + 1));
        }
    }
}
