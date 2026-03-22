using Assets.Project.Scripts.Desserts;
using UnityEngine;

namespace Project.Scripts.GameManager
{
    [CreateAssetMenu(menuName = "Configs/Level Config", fileName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public const int TotalStarsCount = 3;

        [Header("Gameplay")]
        [SerializeField] private DessertPool _dessertPool;
        [SerializeField, Min(1)] private int _copiesPerDessert = 3;
        [SerializeField, Min(0.05f)] private float _spawnDelaySeconds = 0.5f;
        [SerializeField, Min(1f)] private float _roundDurationSeconds = 120f;
        [SerializeField, Min(0f)] private float _actionBarOverflowPenaltySeconds = 10f;
        [SerializeField] private DessertPoints _dessertPointsConfig;

        [Header("Score Criteria")]
        [SerializeField, Min(0)] private int _oneStarScore = 1000;
        [SerializeField, Min(0)] private int _twoStarsScore = 2500;
        [SerializeField, Min(0)] private int _threeStarsScore = 5000;

        public DessertPool DessertPool => _dessertPool;
        public int CopiesPerDessert => _copiesPerDessert;
        public float SpawnDelaySeconds => _spawnDelaySeconds;
        public float RoundDurationSeconds => _roundDurationSeconds;
        public float ActionBarOverflowPenaltySeconds => _actionBarOverflowPenaltySeconds;
        public DessertPoints DessertPointsConfig => _dessertPointsConfig;
        public int OneStarScore => GetNormalizedThresholds().oneStarScore;
        public int TwoStarsScore => GetNormalizedThresholds().twoStarsScore;
        public int ThreeStarsScore => GetNormalizedThresholds().threeStarsScore;

        public int GetPointsForDessert(EDessertType dessertType, int defaultPoints)
        {
            return _dessertPointsConfig != null
                ? _dessertPointsConfig.GetPointsForDessert(dessertType, defaultPoints)
                : Mathf.Max(0, defaultPoints);
        }

        public int GetStarsByScore(int score)
        {
            var safeScore = Mathf.Max(0, score);
            var thresholds = GetNormalizedThresholds();

            if (safeScore >= thresholds.threeStarsScore)
                return 3;

            if (safeScore >= thresholds.twoStarsScore)
                return 2;

            if (safeScore >= thresholds.oneStarScore)
                return 1;

            return 0;
        }

        public int GetNextStarScore(int score)
        {
            var safeScore = Mathf.Max(0, score);
            var thresholds = GetNormalizedThresholds();

            if (safeScore < thresholds.oneStarScore)
                return thresholds.oneStarScore;

            if (safeScore < thresholds.twoStarsScore)
                return thresholds.twoStarsScore;

            if (safeScore < thresholds.threeStarsScore)
                return thresholds.threeStarsScore;

            return -1;
        }

        private (int oneStarScore, int twoStarsScore, int threeStarsScore) GetNormalizedThresholds()
        {
            var oneStar = Mathf.Max(0, _oneStarScore);
            var twoStars = Mathf.Max(oneStar, _twoStarsScore);
            var threeStars = Mathf.Max(twoStars, _threeStarsScore);
            return (oneStar, twoStars, threeStars);
        }

        private void OnValidate()
        {
            _oneStarScore = Mathf.Max(0, _oneStarScore);
            _twoStarsScore = Mathf.Max(_oneStarScore, _twoStarsScore);
            _threeStarsScore = Mathf.Max(_twoStarsScore, _threeStarsScore);
        }
    }
}
