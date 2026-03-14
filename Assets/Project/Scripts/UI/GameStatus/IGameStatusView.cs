using Project.Scripts.Systems.UI;

namespace Project.Scripts.UI.GameStatus
{
    public interface IGameStatusView : ILayoutView
    {
        void SetMessage(string message);
    }
}
