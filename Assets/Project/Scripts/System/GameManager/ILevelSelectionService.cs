using System.Collections.Generic;

namespace Project.Scripts.GameManager
{
    public interface ILevelSelectionService
    {
        IReadOnlyList<LevelConfig> AvailableLevels { get; }
        LevelConfig CurrentLevel { get; }
        bool HasAnyLevel { get; }
        void SelectLevel(LevelConfig levelConfig);
        bool HasNextLevel { get; }
        bool TrySelectNextLevel();
    }

    public interface ILevelProgressService
    {
        int GetBestStars(LevelConfig levelConfig);
        void SaveBestStars(LevelConfig levelConfig, int starsCount);
        int GetMaxUnlockedLevelIndex(IReadOnlyList<LevelConfig> levels);
        bool IsLevelUnlocked(IReadOnlyList<LevelConfig> levels, LevelConfig levelConfig);
        void UnlockNextLevel(IReadOnlyList<LevelConfig> levels, LevelConfig completedLevel);
    }
}
