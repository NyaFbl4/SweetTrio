using System;
using System.Collections.Generic;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;

namespace Project.Scripts.UI.MainScreen
{
    public interface IMainMenuView : ILayoutView
    {
        event Action<LevelConfig> LevelSelected;
        event Action SettingsClicked;
        
        void SetLevels(IReadOnlyList<LevelConfig> levels, LevelConfig selectedLevel,
            IReadOnlyList<int> savedStars, IReadOnlyList<bool> unlockedLevels);
        void SetLevelsTabVisible(bool visible);
        void SetSelectedLevel(LevelConfig selectedLevel);
    }
}
