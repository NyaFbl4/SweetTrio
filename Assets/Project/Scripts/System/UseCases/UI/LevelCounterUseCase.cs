using Project.Scripts.GameManager;
using Project.Scripts.UI.LevelUI;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.UI.UseCases
{
    public class LevelCounterUseCase : ILevelCounterUseCase, IInitializable
    {
        private readonly ILevelUIPresenter _levelUIPresenter;
        private readonly ILevelSelectionService _levelSelectionService;
        private LevelConfig _cachedLevelConfig;
        private int _value;
        private int _targetScore;

        public int CurrentValue => _value;

        public LevelCounterUseCase(
            ILevelUIPresenter levelUIPresenter,
            ILevelSelectionService levelSelectionService)
        {
            _levelUIPresenter = levelUIPresenter;
            _levelSelectionService = levelSelectionService;
        }

        public void Initialize()
        {
            RefreshTargetScore();
            NotifyPresenter();
        }

        public void SetValue(int value)
        {
            _value = value;
            NotifyPresenter();
        }

        public void Increment(int amount = 1)
        {
            if (amount <= 0)
                return;

            _value += amount;
            NotifyPresenter();
        }

        public void Decrement(int amount = 1)
        {
            if (amount <= 0)
                return;

            _value -= amount;
            if (_value < 0)
            {
                _value = 0;
            }

            NotifyPresenter();
        }

        public void Reset()
        {
            _value = 0;
            NotifyPresenter();
        }

        private void NotifyPresenter()
        {
            RefreshTargetScore();
            _levelUIPresenter.SetCounter(_value);

            var normalized = _targetScore > 0
                ? Mathf.Clamp01((float)_value / _targetScore)
                : 0f;
            _levelUIPresenter.SetProgress(normalized);
        }

        private int ResolveTargetScore()
        {
            var levelConfig = _cachedLevelConfig;
            if (levelConfig == null)
                return 0;

            return Mathf.Max(0, levelConfig.ThreeStarsScore);
        }

        private void RefreshTargetScore()
        {
            var currentLevel = _levelSelectionService.CurrentLevel;
            if (ReferenceEquals(currentLevel, _cachedLevelConfig) && _targetScore > 0)
                return;

            _cachedLevelConfig = currentLevel;
            _targetScore = ResolveTargetScore();
        }
    }
}
