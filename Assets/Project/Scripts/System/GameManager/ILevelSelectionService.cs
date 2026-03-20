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
}
