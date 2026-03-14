using Project.Scripts.Systems.UI;

namespace Project.Scripts.UI.GameStatus
{
    public interface IGameStatusPresenter : ILayoutPresenter
    {
        void ShowPaused();
        void ShowWin();
        void ShowLose();
        void HideStatus();
    }
}
