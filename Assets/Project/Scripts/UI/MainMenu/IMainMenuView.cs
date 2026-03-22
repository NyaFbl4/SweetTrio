using System;
using System.Collections.Generic;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;

namespace Project.Scripts.UI.MainScreen
{
    public interface IMainMenuView : ILayoutView
    {
        event Action ChooseLevelClicked;
        event Action<LevelConfig> LevelSelected;

        void SetLevels(IReadOnlyList<LevelConfig> levels, LevelConfig selectedLevel, IReadOnlyList<int> savedStars);
        void SetLevelsTabVisible(bool visible);
        void SetChooseLevelButtonText(string text);
        void SetSelectedLevel(LevelConfig selectedLevel);
    }
}
