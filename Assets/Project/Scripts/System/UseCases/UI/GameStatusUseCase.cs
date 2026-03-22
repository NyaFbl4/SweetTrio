using System;
using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.System.Localization;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.EndGame;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.UI.UseCases
{
    public class GameStatusUseCase : IInitializable, IDisposable
    {
        private const int FallbackOneStarScore = 1000;
        private const int FallbackTwoStarsScore = 2500;
        private const int FallbackThreeStarsScore = 5000;

        [Inject] private readonly ISubscriber<GameStatusCommandDto> _gameStatusSubscriber;
        [Inject] private readonly IEndGamePresenter _endGamePresenter;
        [Inject] private readonly IGameManagerService _gameManagerService;
        [Inject] private readonly ILevelCounterUseCase _levelCounterUseCase;
        [Inject] private readonly ITimerPointsUseCase _timerPointsUseCase;
        [Inject] private readonly ILevelSelectionService _levelSelectionService;
        [Inject] private readonly ILevelProgressService _levelProgressService;
        [Inject] private readonly ILocalizationService _localizationService;

        private IDisposable _subscription = DisposableBag.Empty;

        public void Initialize()
        {
            _subscription = _gameStatusSubscriber.Subscribe(Handle);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        private void Handle(GameStatusCommandDto message)
        {
            if (message == null)
                return;

            switch (message.Command)
            {
                case EGameStatusCommand.ShowWinAndFinish:
                    ShowCompletionResult(applyWinBonus: true);
                    break;
                case EGameStatusCommand.ShowLoseAndFinish:
                    ShowCompletionResult(applyWinBonus: false);
                    break;
            }
        }

        private void ShowCompletionResult(bool applyWinBonus)
        {
            if (applyWinBonus)
            {
                _timerPointsUseCase.ApplyWinBonus();
            }

            var score = Mathf.Max(0, _levelCounterUseCase.CurrentValue);
            var levelConfig = _levelSelectionService.CurrentLevel;
            var totalStarsCount = LevelConfig.TotalStarsCount;
            var starsCount = ResolveStarsCount(levelConfig, score);
            var isPassed = starsCount > 0;
            var completionText = BuildCompletionText(levelConfig, score, starsCount, totalStarsCount);

            _levelProgressService.SaveBestStars(levelConfig, starsCount);
            _endGamePresenter.ShowResult(isPassed, score, starsCount, totalStarsCount, completionText);
            _gameManagerService.FinishGame();
        }

        private string BuildCompletionText(LevelConfig levelConfig, int score, int starsCount, int totalStarsCount)
        {
            if (starsCount <= 0)
            {
                var passScore = ResolveThreshold(levelConfig, 1);
                var missing = Mathf.Max(0, passScore - score);
                return missing > 0
                    ? FormatLocalizedText(LocalizationKeys.GameStatusNotEnoughToPassFormat, "Need {0} more points to pass", missing)
                    : GetLocalizedText(LocalizationKeys.GameStatusLevelNotPassed, "Level failed");
            }

            if (starsCount >= totalStarsCount)
            {
                return FormatLocalizedText(
                    LocalizationKeys.GameStatusMaxResultFormat,
                    "Best result: {0}/{1} stars",
                    totalStarsCount,
                    totalStarsCount);
            }

            var nextStarNumber = starsCount + 1;
            var nextStarScore = ResolveNextStarScore(levelConfig, score);
            if (nextStarScore <= score)
            {
                return FormatLocalizedText(
                    LocalizationKeys.GameStatusPassedStarsFormat,
                    "Completed with {0}/{1} stars",
                    starsCount,
                    totalStarsCount);
            }

            var missingForNextStar = nextStarScore - score;
            return FormatLocalizedText(
                LocalizationKeys.GameStatusToNextStarFormat,
                "Completed with {0}/{1}. To star {2}: {3}",
                starsCount,
                totalStarsCount,
                nextStarNumber,
                missingForNextStar);
        }

        private static int ResolveStarsCount(LevelConfig levelConfig, int score)
        {
            if (levelConfig != null)
                return levelConfig.GetStarsByScore(score);

            var safeScore = Mathf.Max(0, score);
            if (safeScore >= FallbackThreeStarsScore)
                return 3;

            if (safeScore >= FallbackTwoStarsScore)
                return 2;

            if (safeScore >= FallbackOneStarScore)
                return 1;

            return 0;
        }

        private static int ResolveNextStarScore(LevelConfig levelConfig, int score)
        {
            if (levelConfig != null)
                return levelConfig.GetNextStarScore(score);

            var safeScore = Mathf.Max(0, score);
            if (safeScore < FallbackOneStarScore)
                return FallbackOneStarScore;

            if (safeScore < FallbackTwoStarsScore)
                return FallbackTwoStarsScore;

            if (safeScore < FallbackThreeStarsScore)
                return FallbackThreeStarsScore;

            return -1;
        }

        private static int ResolveThreshold(LevelConfig levelConfig, int starNumber)
        {
            if (levelConfig != null)
            {
                return starNumber switch
                {
                    1 => levelConfig.OneStarScore,
                    2 => levelConfig.TwoStarsScore,
                    3 => levelConfig.ThreeStarsScore,
                    _ => levelConfig.OneStarScore
                };
            }

            return starNumber switch
            {
                1 => FallbackOneStarScore,
                2 => FallbackTwoStarsScore,
                3 => FallbackThreeStarsScore,
                _ => FallbackOneStarScore
            };
        }

        private string GetLocalizedText(string key, string fallback)
        {
            if (_localizationService == null)
                return fallback;

            var text = _localizationService.Get(key);
            return string.IsNullOrWhiteSpace(text) ? fallback : text;
        }

        private string FormatLocalizedText(string key, string fallbackFormat, params object[] args)
        {
            if (_localizationService != null)
            {
                return _localizationService.Format(key, args);
            }

            try
            {
                return string.Format(fallbackFormat, args);
            }
            catch (FormatException)
            {
                return fallbackFormat;
            }
        }
    }
}
