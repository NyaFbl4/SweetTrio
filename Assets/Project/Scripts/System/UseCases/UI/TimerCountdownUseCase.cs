using System;
using Project.Scripts.GameManager;
using MessagePipe;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.LevelUI;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.UI.UseCases
{
    public class TimerCountdownUseCase : ITimerCountdownUseCase, IInitializable, ITickable, IDisposable, IGameStartListener, IGameFinishListener
    {
        private const float DefaultRoundDurationSeconds = 120f;

        private readonly ILevelUIPresenter _levelUIPresenter;
        private readonly IPublisher<GameStatusCommandDto> _gameStatusPublisher;
        private readonly ILevelSelectionService _levelSelectionService;

        private float _durationSeconds;
        private float _remainingSeconds;
        private int _lastShownSeconds = -1;
        private bool _isActive;
        private bool _timeoutTriggered;

        public float RemainingSeconds => _remainingSeconds;

        public TimerCountdownUseCase(
            ILevelUIPresenter levelUIPresenter,
            IPublisher<GameStatusCommandDto> gameStatusPublisher,
            ILevelSelectionService levelSelectionService)
        {
            _levelUIPresenter = levelUIPresenter;
            _gameStatusPublisher = gameStatusPublisher;
            _levelSelectionService = levelSelectionService;

            _durationSeconds = Mathf.Max(0.01f, DefaultRoundDurationSeconds);
            _remainingSeconds = DefaultRoundDurationSeconds;
        }

        public void Initialize()
        {
            IGameListener.Register(this);
            Reset(ResolveRoundDurationSeconds());
            NotifyPresenter(forceTextUpdate: true);
        }

        public void Dispose()
        {
            IGameListener.Unregister(this);
        }

        public void Tick()
        {
            if (!_isActive)
                return;

            _remainingSeconds -= Time.deltaTime;
            if (_remainingSeconds <= 0f)
            {
                _remainingSeconds = 0f;
                _isActive = false;
                NotifyPresenter(forceTextUpdate: true);
                TriggerTimeoutLose();
                return;
            }

            NotifyPresenter(forceTextUpdate: false);
        }

        public void OnStartGame()
        {
            _timeoutTriggered = false;
            Reset(ResolveRoundDurationSeconds());
            _isActive = true;
        }

        public void OnFinishGame()
        {
            _isActive = false;
            NotifyPresenter(forceTextUpdate: true);
        }

        public void Reset(float seconds)
        {
            _durationSeconds = Mathf.Max(0.01f, seconds);
            _remainingSeconds = Mathf.Max(0f, seconds);
            _lastShownSeconds = -1;
            NotifyPresenter(forceTextUpdate: true);
        }

        public void SubtractSeconds(float seconds)
        {
            if (!_isActive || seconds <= 0f)
                return;

            _remainingSeconds = Mathf.Max(0f, _remainingSeconds - seconds);
            if (_remainingSeconds <= 0f)
            {
                _remainingSeconds = 0f;
                _isActive = false;
                NotifyPresenter(forceTextUpdate: true);
                TriggerTimeoutLose();
                return;
            }

            NotifyPresenter(forceTextUpdate: true);
        }

        private void TriggerTimeoutLose()
        {
            if (_timeoutTriggered)
                return;

            _timeoutTriggered = true;
            _gameStatusPublisher.Publish(new GameStatusCommandDto
            {
                Command = EGameStatusCommand.ShowLoseAndFinish
            });
        }

        private void NotifyPresenter(bool forceTextUpdate)
        {
            var normalized = Mathf.Clamp01(_remainingSeconds / _durationSeconds);
            _levelUIPresenter.SetProgress(normalized);

            var roundedSeconds = Mathf.CeilToInt(_remainingSeconds);
            if (!forceTextUpdate && roundedSeconds == _lastShownSeconds)
                return;

            _lastShownSeconds = roundedSeconds;
            _levelUIPresenter.SetTimerText(FormatSeconds(roundedSeconds));
        }

        private static string FormatSeconds(int totalSeconds)
        {
            totalSeconds = Mathf.Max(0, totalSeconds);
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }

        private float ResolveRoundDurationSeconds()
        {
            var levelConfig = _levelSelectionService.CurrentLevel;
            if (levelConfig != null && levelConfig.RoundDurationSeconds > 0f)
            {
                return levelConfig.RoundDurationSeconds;
            }

            return DefaultRoundDurationSeconds;
        }
    }
}
