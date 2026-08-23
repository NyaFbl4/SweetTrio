using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.GameManager
{
    public class LevelSelectionService : ILevelSelectionService, IInitializable
    {
        private readonly LevelsCatalogConfig _levelsCatalogConfig;
        private readonly LevelConfig _fallbackLevelConfig;
        private readonly List<LevelConfig> _availableLevels = new();

        public IReadOnlyList<LevelConfig> AvailableLevels => _availableLevels;
        public LevelConfig CurrentLevel { get; private set; }
        public bool HasAnyLevel => _availableLevels.Count > 0;
        
        public bool HasNextLevel
        {
            get
            {
                var currentLevelIndex = GetCurrentLevelIndex();
                return currentLevelIndex >= 0 && currentLevelIndex < _availableLevels.Count - 1;
            }
        }

        public LevelSelectionService(LevelsCatalogConfig levelsCatalogConfig, LevelConfig fallbackLevelConfig)
        {
            _levelsCatalogConfig = levelsCatalogConfig;
            _fallbackLevelConfig = fallbackLevelConfig;
        }

        public void Initialize()
        {
            _availableLevels.Clear();

            if (_levelsCatalogConfig != null && _levelsCatalogConfig.Levels != null)
            {
                for (var i = 0; i < _levelsCatalogConfig.Levels.Count; i++)
                {
                    var levelConfig = _levelsCatalogConfig.Levels[i];
                    if (levelConfig == null || _availableLevels.Contains(levelConfig))
                        continue;

                    _availableLevels.Add(levelConfig);
                }
            }

            if (_fallbackLevelConfig != null && !_availableLevels.Contains(_fallbackLevelConfig))
            {
                _availableLevels.Add(_fallbackLevelConfig);
            }

            if (_availableLevels.Count == 0)
            {
                CurrentLevel = null;
                Debug.LogError("LevelSelectionService: levels list is empty. Assign LevelsCatalogConfig or fallback LevelConfig.");
                return;
            }

            var defaultLevel = _levelsCatalogConfig != null ? _levelsCatalogConfig.DefaultLevel : null;
            if (defaultLevel != null && _availableLevels.Contains(defaultLevel))
            {
                CurrentLevel = defaultLevel;
                return;
            }

            CurrentLevel = _availableLevels[0];
        }

        public void SelectLevel(LevelConfig levelConfig)
        {
            if (levelConfig == null)
            {
                Debug.LogWarning("LevelSelectionService: trying to select null level.");
                return;
            }

            if (!_availableLevels.Contains(levelConfig))
            {
                Debug.LogWarning($"LevelSelectionService: level '{levelConfig.name}' is not present in catalog.");
                return;
            }

            CurrentLevel = levelConfig;
        }
        
        public bool TrySelectNextLevel()
        {
            var currentLevelIndex = GetCurrentLevelIndex();
            if (currentLevelIndex < 0 || currentLevelIndex >= _availableLevels.Count - 1)
                return false;

            CurrentLevel = _availableLevels[currentLevelIndex + 1];
            return true;
        }

        private int GetCurrentLevelIndex()
        {
            return CurrentLevel != null ? _availableLevels.IndexOf(CurrentLevel) : -1;
        }

    }

    public class LevelProgressService : ILevelProgressService
    {
        private const string StarsKeyPrefix = "level_stars_";
        private const string MaxUnlockedLevelIndexKey = "max_unlocked_level_index";

        public int GetBestStars(LevelConfig levelConfig)
        {
            if (levelConfig == null)
                return 0;

            var value = PlayerPrefs.GetInt(GetLevelStarsKey(levelConfig), 0);
            return Mathf.Clamp(value, 0, LevelConfig.TotalStarsCount);
        }

        public void SaveBestStars(LevelConfig levelConfig, int starsCount)
        {
            if (levelConfig == null)
                return;

            var clampedStars = Mathf.Clamp(starsCount, 0, LevelConfig.TotalStarsCount);
            var key = GetLevelStarsKey(levelConfig);
            var currentBest = PlayerPrefs.GetInt(key, 0);
            if (clampedStars <= currentBest)
                return;

            PlayerPrefs.SetInt(key, clampedStars);
            PlayerPrefs.Save();
        }
        
        public int GetMaxUnlockedLevelIndex(IReadOnlyList<LevelConfig> levels)
        {
            if (levels == null || levels.Count == 0)
                return 0;

            var savedIndex = PlayerPrefs.GetInt(MaxUnlockedLevelIndexKey, 0);

            for (var i = 0; i < levels.Count; i++)
            {
                if (GetBestStars(levels[i]) > 0)
                    savedIndex = Mathf.Max(savedIndex, i + 1);
            }

            return Mathf.Clamp(savedIndex, 0, levels.Count - 1);
        }

        public bool IsLevelUnlocked(IReadOnlyList<LevelConfig> levels, LevelConfig levelConfig)
        {
            if (levels == null || levelConfig == null)
                return false;

            var maxUnlockedIndex = GetMaxUnlockedLevelIndex(levels);

            for (var i = 0; i < levels.Count; i++)
            {
                if (levels[i] == levelConfig)
                    return i <= maxUnlockedIndex;
            }

            return false;
        }

        public void UnlockNextLevel(IReadOnlyList<LevelConfig> levels, LevelConfig completedLevel)
        {
            if (levels == null || completedLevel == null)
                return;

            for (var i = 0; i < levels.Count; i++)
            {
                if (levels[i] != completedLevel)
                    continue;

                var nextIndex = Mathf.Clamp(i + 1, 0, levels.Count - 1);
                var savedIndex = PlayerPrefs.GetInt(MaxUnlockedLevelIndexKey, 0);

                if (nextIndex > savedIndex)
                {
                    PlayerPrefs.SetInt(MaxUnlockedLevelIndexKey, nextIndex);
                    PlayerPrefs.Save();
                }

                return;
            }
        }

        private static string GetLevelStarsKey(LevelConfig levelConfig)
        {
            return $"{StarsKeyPrefix}{levelConfig.name}";
        }
    }
}
