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

            _availableLevels.Sort(CompareLevels);

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

        private static int CompareLevels(LevelConfig left, LevelConfig right)
        {
            if (left == null && right == null)
                return 0;

            if (left == null)
                return 1;

            if (right == null)
                return -1;

            var orderCompare = left.MenuOrder.CompareTo(right.MenuOrder);
            if (orderCompare != 0)
                return orderCompare;

            return string.Compare(left.LevelTitle, right.LevelTitle, global::System.StringComparison.Ordinal);
        }
    }
}

