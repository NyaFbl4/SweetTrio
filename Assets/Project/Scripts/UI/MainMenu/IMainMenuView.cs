using System;
using Project.Scripts.Systems.UI;

namespace Project.Scripts.UI.MainScreen
{
    public interface IMainMenuView : ILayoutView
    {
        event Action StartLevelClicked;
    }
}
