using Project.Scripts.Systems.UI;

namespace Project.Scripts.UI.TimerUI
{
    public interface ITimerUIPresenter : ILayoutPresenter
    {
        void SetTimerText(string text);
        void SetProgress(float value01);
    }
}
