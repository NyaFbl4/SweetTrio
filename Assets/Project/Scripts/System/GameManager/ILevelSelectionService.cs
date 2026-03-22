using System.Collections.Generic;

namespace Project.Scripts.GameManager
{
    public interface ILevelSelectionService
    {
        IReadOnlyList<LevelConfig> AvailableLevels { get; }
        LevelConfig CurrentLevel { get; }
        bool HasAnyLevel { get; }
        void SelectLevel(LevelConfig levelConfig);
    }

    public interface ILevelProgressService
    {
        int GetBestStars(LevelConfig levelConfig);
        void SaveBestStars(LevelConfig levelConfig, int starsCount);
    }
}
