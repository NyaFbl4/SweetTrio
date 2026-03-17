using System;
using Project.Scripts.GameManager;
using MessagePipe;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.TimerUI;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.UI.UseCases
{
    public class TimerCountdownUseCase : ITimerCountdownUseCase, IInitializable, ITickable, IDisposable, IGameStartListener, IGameFinishListener
    {
        private const float DefaultRoundDurationSeconds = 120f;

        private readonly ITimerUIPresenter _timerUIPresenter;
        private readonly IPublisher<GameStatusCommandDto> _gameStatusPublisher;
        private readonly float _initialDurationSeconds;

        private float _durationSeconds;
        private float _remainingSeconds;
        private int _lastShownSeconds = -1;
        private bool _isActive;
        private bool _timeoutTriggered;

        public float RemainingSeconds => _remainingSeconds;

        public TimerCountdownUseCase(
            ITimerUIPresenter timerUIPresenter,
            IPublisher<GameStatusCommandDto> gameStatusPublisher,
            LevelConfig levelConfig)
        {
            _timerUIPresenter = timerUIPresenter;
            _gameStatusPublisher = gameStatusPublisher;
            _initialDurationSeconds = levelConfig != null && levelConfig.RoundDurationSeconds > 0f
                ? levelConfig.RoundDurationSeconds
                : DefaultRoundDurationSeconds;

            _durationSeconds = Mathf.Max(0.01f, _initialDurationSeconds);
            _remainingSeconds = _initialDurationSeconds;
        }

        public void Initialize()
        {
            IGameListener.Register(this);
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
            Reset(_initialDurationSeconds);
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
            _timerUIPresenter.SetProgress(normalized);

            var roundedSeconds = Mathf.CeilToInt(_remainingSeconds);
            if (!forceTextUpdate && roundedSeconds == _lastShownSeconds)
                return;

            _lastShownSeconds = roundedSeconds;
            _timerUIPresenter.SetTimerText(FormatSeconds(roundedSeconds));
        }

        private static string FormatSeconds(int totalSeconds)
        {
            totalSeconds = Mathf.Max(0, totalSeconds);
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }
    }
}
