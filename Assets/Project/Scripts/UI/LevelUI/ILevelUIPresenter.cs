using Project.Scripts.Systems.UI;

namespace Project.Scripts.UI.LevelUI
{
    public interface ILevelUIPresenter : ILayoutPresenter
    {
        void SetCounter(int value);
        void SetCounterText(string text);
        void SetTotalDessertsText(string text);
    }
}
