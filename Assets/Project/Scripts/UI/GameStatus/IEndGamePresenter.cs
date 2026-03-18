using Project.Scripts.Systems.UI;

namespace Project.Scripts.UI.EndGame
{
    public interface IEndGamePresenter : ILayoutPresenter
    {
        void ShowWin(int score);
        void ShowLose();
    }
}

