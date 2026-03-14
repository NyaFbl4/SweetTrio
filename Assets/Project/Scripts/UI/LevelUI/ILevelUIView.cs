using Project.Scripts.Systems.UI;

namespace Project.Scripts.UI.LevelUI
{
    public interface ILevelUIView : ILayoutView
    {
        void SetCounter(int value);
        void SetCounterText(string text);
    }
}
