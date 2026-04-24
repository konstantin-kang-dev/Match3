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
        public int PlayerLevel { get; private set; } = 1;
        public float PlayerExp { get; private set; } = 0f;
        public float RequiredExpToLvlUp { get; private set; } = 0f;

        readonly Subject<int> _onLevelUp = new();
        public Observable<int> OnLevelUp => _onLevelUp.AsObservable();

        readonly Subject<(float current, float required)> _onExpChanged = new();
        public Observable<(float current, float required)> OnExpChanged => _onExpChanged.AsObservable();

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
            SetExp(0);
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
                SetExp(PlayerExp + exp);
            }
        }

        void SetExp(float exp)
        {
            PlayerExp = exp;

            _onExpChanged.OnNext((PlayerExp, RequiredExpToLvlUp));
        }

        bool IsLevelUpAvailable()
        {
            return PlayerExp >= RequiredExpToLvlUp;
        }

        void LevelUp()
        {
            SetExp(PlayerExp - RequiredExpToLvlUp);
            SetLevel(PlayerLevel + 1);
        }

        void SetLevel(int level)
        {
            PlayerLevel = level;
            RequiredExpToLvlUp = _progressionConfig.GetRequiredExp(PlayerLevel + 1);
            _onLevelUp.OnNext(PlayerLevel);
        }
    }
}
