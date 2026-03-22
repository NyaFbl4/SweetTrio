using System;
using MessagePipe;
using Project.Scripts.GameManager;
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

        private static string BuildCompletionText(LevelConfig levelConfig, int score, int starsCount, int totalStarsCount)
        {
            if (starsCount <= 0)
            {
                var passScore = ResolveThreshold(levelConfig, 1);
                var missing = Mathf.Max(0, passScore - score);
                return missing > 0
                    ? $"Не хватило {missing} очков до прохождения"
                    : "Уровень не пройден";
            }

            if (starsCount >= totalStarsCount)
                return $"Максимальный результат: {totalStarsCount}/{totalStarsCount} звезд";

            var nextStarNumber = starsCount + 1;
            var nextStarScore = ResolveNextStarScore(levelConfig, score);
            if (nextStarScore <= score)
                return $"Пройдено на {starsCount}/{totalStarsCount} звезд";

            var missingForNextStar = nextStarScore - score;
            return $"Пройдено на {starsCount}/{totalStarsCount}. До {nextStarNumber}-й звезды: {missingForNextStar}";
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
    }
}


