using System;
using Assets.Project.Scripts.Desserts;
using UnityEngine;

namespace Project.Scripts.GameManager
{
    [CreateAssetMenu(menuName = "Configs/Level Config", fileName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Menu")]
        [SerializeField] private string _levelTitle = "New Level";
        [SerializeField, TextArea(2, 4)] private string _levelDescription = "Level description";
        [SerializeField] private int _menuOrder;

        [Header("Gameplay")]
        [SerializeField] private DessertPool _dessertPool;
        [SerializeField, Min(1)] private int _copiesPerDessert = 3;
        [SerializeField, Min(0.05f)] private float _spawnDelaySeconds = 0.5f;
        [SerializeField, Min(1f)] private float _roundDurationSeconds = 120f;
        [SerializeField, Min(0f)] private float _actionBarOverflowPenaltySeconds = 10f;
        [SerializeField] private DessertPoints _dessertPointsConfig;

        public string LevelTitle => string.IsNullOrWhiteSpace(_levelTitle) ? name : _levelTitle;
        public string LevelDescription => string.IsNullOrWhiteSpace(_levelDescription)
            ? $"Time: {Mathf.RoundToInt(_roundDurationSeconds)}s | Copies: {_copiesPerDessert} | Penalty: {Mathf.RoundToInt(_actionBarOverflowPenaltySeconds)}s"
            : _levelDescription;
        public int MenuOrder => _menuOrder;
        public DessertPool DessertPool => _dessertPool;
        public int CopiesPerDessert => _copiesPerDessert;
        public float SpawnDelaySeconds => _spawnDelaySeconds;
        public float RoundDurationSeconds => _roundDurationSeconds;
        public float ActionBarOverflowPenaltySeconds => _actionBarOverflowPenaltySeconds;
        public DessertPoints DessertPointsConfig => _dessertPointsConfig;

        public int GetPointsForDessert(EDessertType dessertType, int defaultPoints)
        {
            return _dessertPointsConfig != null
                ? _dessertPointsConfig.GetPointsForDessert(dessertType, defaultPoints)
                : Mathf.Max(0, defaultPoints);
        }
    }
}
