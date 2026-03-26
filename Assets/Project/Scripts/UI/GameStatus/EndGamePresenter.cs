using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.System.Localization;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.MainScreen;
using VContainer;

namespace Project.Scripts.UI.EndGame
{
    public class EndGamePresenter : LayoutPresenterBase<IEndGameView>, IEndGamePresenter, IGameStartListener
    {
        [Inject] private readonly IGameManagerService _gameManagerService;
        [Inject] private readonly ILocalizationService _localizationService;
        [Inject] private readonly IPublisher<ShowPopupDto> _showPopUpPublisher;
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;

        public override void Initialize()
        {
            base.Initialize();
            _layoutView.PrimaryButtonClicked += HandlePrimaryButtonClicked;
            _layoutView.SecondaryButtonClicked += HandleSecondaryButtonClicked;
            IGameListener.Register(this);
        }

        public override void Dispose()
        {
            _layoutView.PrimaryButtonClicked -= HandlePrimaryButtonClicked;
            _layoutView.SecondaryButtonClicked -= HandleSecondaryButtonClicked;
            IGameListener.Unregister(this);
            base.Dispose();
        }

        public void ShowResult(bool isPassed, int score, int starsCount, int totalStarsCount, string completionText)
        {
            var titleKey = isPassed ? LocalizationKeys.EndGameTitleWin : LocalizationKeys.EndGameTitleLose;
            var title = _localizationService != null
                ? _localizationService.Get(titleKey)
                : (isPassed ? "Victory" : "Defeat");

            var scoreText = _localizationService != null
                ? _localizationService.Format(LocalizationKeys.EndGameScoreFormat, score)
                : $"Score: {score}";

            _layoutView.SetTitle(title);
            _layoutView.SetScoreText(scoreText);
            _layoutView.SetScoreVisible(true);

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

        private void HandlePrimaryButtonClicked()
        {
            _gameManagerService.StartGame();
        }

        private void HandleSecondaryButtonClicked()
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
