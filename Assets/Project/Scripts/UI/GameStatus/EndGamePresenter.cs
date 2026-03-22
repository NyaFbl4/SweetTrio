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
        private const string PassedTitle = "Победа";
        private const string FailedTitle = "Поражение";

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

        public void ShowResult(bool isPassed, int score, int starsCount, int totalStarsCount, string completionText)
        {
            _layoutView.SetTitle(isPassed ? PassedTitle : FailedTitle);
            _layoutView.SetScoreText($"Очки: {score}");
            _layoutView.SetScoreVisible(true);

            // Current EndGame design: title + score + stars + menu button.
            _layoutView.SetCompletionText(string.Empty);
            _layoutView.SetCompletionVisible(false);

            _layoutView.SetStarsVisible(true);
            _layoutView.SetStars(starsCount, totalStarsCount > 0 ? totalStarsCount : LevelConfig.TotalStarsCount);

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
