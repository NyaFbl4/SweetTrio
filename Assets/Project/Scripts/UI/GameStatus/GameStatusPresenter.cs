using Cysharp.Threading.Tasks;
using MessagePipe;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using VContainer;

namespace Project.Scripts.UI.GameStatus
{
    public class GameStatusPresenter : LayoutPresenterBase<IGameStatusView>, IGameStatusPresenter
    {
        private const string PauseMessage = "Игра на паузе";
        private const string WinMessage = "Победили";
        private const string LoseMessage = "Проиграли";

        [Inject] private readonly IPublisher<ShowPopupDto> _showPopUpPublisher;
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;

        public void ShowPaused()
        {
            ShowWithMessage(PauseMessage);
        }

        public void ShowWin()
        {
            ShowWithMessage(WinMessage);
        }

        public void ShowLose()
        {
            ShowWithMessage(LoseMessage);
        }

        public void HideStatus()
        {
            _hidePopUpPublisher.Publish(new HidePopupDto { TargetPopUpType = typeof(IGameStatusPresenter) });
        }

        public override async UniTask ActivateAsync()
        {
            _layoutView.SetMessage(PauseMessage);
            await base.ActivateAsync();
        }

        private void ShowWithMessage(string message)
        {
            _layoutView.SetMessage(message);
            _showPopUpPublisher.Publish(new ShowPopupDto { TargetPopUpType = typeof(IGameStatusPresenter) });
        }
    }
}
