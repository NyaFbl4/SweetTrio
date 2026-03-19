using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.MainScreen;
using VContainer;

namespace Project.Scripts.UI.EndGame
{
    public class EndGamePresenter : LayoutPresenterBase<IEndGameView>, IEndGamePresenter, IGameStartListener
    {
        private const string WinMessage = "Победа";
        private const string LoseMessage = "Поражение";

        [Inject] private readonly IGameManagerService _gameManagerService;
        [Inject] private readonly IPublisher<ShowPopupDto> _showPopUpPublisher;
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;

        public override void Initialize()
        {
            base.Initialize();
            _layoutView.ExitToMenuClicked += HandleExitToMenuClicked;
            IGameListener.Register(this);
        }

        public override void Dispose()
        {
            _layoutView.ExitToMenuClicked -= HandleExitToMenuClicked;
            IGameListener.Unregister(this);
            base.Dispose();
        }

        public void ShowWin(int score)
        {
            _layoutView.SetTitle(WinMessage);
            _layoutView.SetScoreText($"Очки: {score}");
            _layoutView.SetScoreVisible(true);
            _showPopUpPublisher.Publish(new ShowPopupDto { TargetPopUpType = typeof(IEndGamePresenter) });
        }

        public void ShowLose()
        {
            _layoutView.SetTitle(LoseMessage);
            _layoutView.SetScoreVisible(false);
            _showPopUpPublisher.Publish(new ShowPopupDto { TargetPopUpType = typeof(IEndGamePresenter) });
        }

        public void OnStartGame()
        {
            _hidePopUpPublisher.Publish(new HidePopupDto { TargetPopUpType = typeof(IEndGamePresenter) });
        }

        private void HandleExitToMenuClicked()
        {
            _gameManagerService.FinishGame();

            _hidePopUpPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(IEndGamePresenter)
            });

            _showPopUpPublisher.Publish(new ShowPopupDto
            {
                TargetPopUpType = typeof(IMainMenuPresenter)
            });
        }
    }
}
