using System;
using Project.Scripts.GameManager;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.UI.UseCases
{
    public class TimerPointsUseCase : ITimerPointsUseCase, IInitializable, IDisposable, IGameStartListener, IGameFinishListener
    {
        private readonly ITimerCountdownUseCase _timerCountdownUseCase;
        private readonly ILevelCounterUseCase _levelCounterUseCase;
        private bool _bonusApplied;

        public TimerPointsUseCase(
            ITimerCountdownUseCase timerCountdownUseCase,
            ILevelCounterUseCase levelCounterUseCase)
        {
            _timerCountdownUseCase = timerCountdownUseCase;
            _levelCounterUseCase = levelCounterUseCase;
        }

        public void Initialize()
        {
            IGameListener.Register(this);
            _bonusApplied = false;
        }

        public void Dispose()
        {
            IGameListener.Unregister(this);
        }

        public void OnStartGame()
        {
            _bonusApplied = false;
        }

        public void OnFinishGame()
        {
            _bonusApplied = true;
        }

        public int ApplyWinBonus()
        {
            if (_bonusApplied)
                return 0;

            var wholeSeconds = Mathf.Max(0, Mathf.FloorToInt(_timerCountdownUseCase.RemainingSeconds));
            var bonusPoints = CalculateTriangularPoints(wholeSeconds);
            if (bonusPoints > 0)
            {
                _levelCounterUseCase.Increment(bonusPoints);
            }

            _bonusApplied = true;
            return bonusPoints;
        }

        private static int CalculateTriangularPoints(int seconds)
        {
            var safeSeconds = Mathf.Max(0, seconds);
            var result = (long)safeSeconds * (safeSeconds + 1) / 2;
            return result > int.MaxValue ? int.MaxValue : (int)result;
        }
    }
}
