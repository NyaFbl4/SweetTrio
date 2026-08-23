using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.System.Localization;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.MainScreen;
using Project.Scripts.UI.RulesUI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Project.Scripts.UI.EndGame
{
    public class EndGamePresenter : LayoutPresenterBase<IEndGameView>, IEndGamePresenter, IGameStartListener
    {
        private const int ScoreAnimationStartDelayMilliseconds = 240;

        [Inject] private readonly IGameManagerService _gameManagerService;
        [Inject] private readonly ILocalizationService _localizationService;
        [Inject] private readonly IPublisher<ShowPopupDto> _showPopUpPublisher;
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;
        [Inject] private readonly IRulesUIController _rulesUIController;
        [Inject] private readonly ILevelSelectionService _levelSelectionService;

        private int _scoreAnimationVersion;

        public override void Initialize()
        {
            base.Initialize();
            _layoutView.RestartButtonClicked += HandleRestartButtonClicked;
            _layoutView.NextLevelButtonClicked += HandleNextLevelButtonClicked;
            _layoutView.MenuButtonClicked += HandleMenuButtonClicked;
            IGameListener.Register(this);
        }

        public override void Dispose()
        {
            _scoreAnimationVersion++;
            _layoutView.RestartButtonClicked -= HandleRestartButtonClicked;
            _layoutView.NextLevelButtonClicked -= HandleNextLevelButtonClicked;
            _layoutView.MenuButtonClicked -= HandleMenuButtonClicked;
            IGameListener.Unregister(this);
            base.Dispose();
        }

        public void ShowResult(bool isPassed, int score, int starsCount, 
            int totalStarsCount, string completionText, int timeBonusPoints = 0 )
        {
            var titleKey = isPassed ? LocalizationKeys.EndGameTitleWin : LocalizationKeys.EndGameTitleLose;
            var title = _localizationService != null
                ? _localizationService.Get(titleKey)
                : (isPassed ? "Victory" : "Defeat");

            var finalScore = Mathf.Max(0, score);
            var safeTimeBonusPoints = Mathf.Clamp(timeBonusPoints, 0, finalScore);
            var scoreBeforeTimeBonus = finalScore - safeTimeBonusPoints;

            _scoreAnimationVersion++;

            _layoutView.SetTitle(title);
            _layoutView.SetScoreText(FormatScore(scoreBeforeTimeBonus));
            _layoutView.SetScoreVisible(true);

            _layoutView.SetCompletionText(completionText);
            _layoutView.SetCompletionVisible(!string.IsNullOrWhiteSpace(completionText));

            _layoutView.SetStarsVisible(true);
            _layoutView.SetStars(starsCount, totalStarsCount > 0 ? totalStarsCount : LevelConfig.TotalStarsCount);
            _layoutView.SetNextLevelButtonVisible(isPassed && _levelSelectionService.HasNextLevel);

            _showPopUpPublisher.Publish(new ShowPopupDto { TargetPopUpType = typeof(IEndGamePresenter) });

            if (!isPassed || safeTimeBonusPoints <= 0)
            {
                _layoutView.SetScoreText(FormatScore(finalScore));
                return;
            }

            AnimateTimeBonusScoreAsync(scoreBeforeTimeBonus, finalScore, _scoreAnimationVersion).Forget();
        }

        public void OnStartGame()
        {
            _scoreAnimationVersion++;
            _layoutView.SetNextLevelButtonVisible(false);
            _hidePopUpPublisher.Publish(new HidePopupDto { TargetPopUpType = typeof(IEndGamePresenter) });
        }

        private void HandleRestartButtonClicked()
        {
            ShowRulesForSelectedLevel();
        }
        
        private void ShowRulesForSelectedLevel()
        {
            _scoreAnimationVersion++;

            _hidePopUpPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(IEndGamePresenter)
            });

            _rulesUIController.ShowBeforeLevelStart();
        }
        
        private void HandleNextLevelButtonClicked()
        {
            if (_levelSelectionService.TrySelectNextLevel())
                ShowRulesForSelectedLevel();
        }

        private void HandleMenuButtonClicked()
        {
            _scoreAnimationVersion++;
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

        private async UniTaskVoid AnimateTimeBonusScoreAsync(int fromScore, int toScore, int animationVersion)
        {
            if (toScore <= fromScore)
            {
                _layoutView.SetScoreText(FormatScore(toScore));
                return;
            }

            await UniTask.Delay(
                ScoreAnimationStartDelayMilliseconds,
                DelayType.UnscaledDeltaTime,
                PlayerLoopTiming.Update);

            if (animationVersion != _scoreAnimationVersion)
                return;

            var delta = toScore - fromScore;
            var durationSeconds = Mathf.Clamp(0.35f + delta * 0.0018f, 0.35f, 1.35f);
            var elapsed = 0f;
            var currentScore = fromScore;

            while (elapsed < durationSeconds)
            {
                if (animationVersion != _scoreAnimationVersion)
                    return;

                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / durationSeconds);
                var eased = 1f - Mathf.Pow(1f - t, 3f);
                var nextScore = Mathf.RoundToInt(Mathf.Lerp(fromScore, toScore, eased));

                if (nextScore != currentScore)
                {
                    currentScore = nextScore;
                    _layoutView.SetScoreText(FormatScore(currentScore));
                }

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            if (animationVersion != _scoreAnimationVersion)
                return;

            _layoutView.SetScoreText(FormatScore(toScore));
        }

        private string FormatScore(int score)
        {
            var safeScore = Mathf.Max(0, score);
            return _localizationService != null
                ? _localizationService.Format(LocalizationKeys.EndGameScoreFormat, safeScore)
                : $"Score: {safeScore}";
        }
    }
}

