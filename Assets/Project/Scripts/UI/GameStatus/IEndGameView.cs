using System;
using Project.Scripts.Systems.UI;

namespace Project.Scripts.UI.EndGame
{
    public interface IEndGameView : ILayoutView
    {
        event Action ExitToMenuClicked;

        void SetTitle(string message);
        void SetScoreText(string text);
        void SetScoreVisible(bool isVisible);
    }
}
