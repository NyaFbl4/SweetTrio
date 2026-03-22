using Project.Scripts.System.Localization;
using Project.Scripts.UI.LevelUI;
using VContainer.Unity;

namespace Project.Scripts.UI.UseCases
{
    public class LevelCounterUseCase : ILevelCounterUseCase, IInitializable
    {
        private readonly ILevelUIPresenter _levelUIPresenter;
        private readonly ILocalizationService _localizationService;
        private int _value;

        public int CurrentValue => _value;

        public LevelCounterUseCase(
            ILevelUIPresenter levelUIPresenter,
            ILocalizationService localizationService)
        {
            _levelUIPresenter = levelUIPresenter;
            _localizationService = localizationService;
        }

        public void Initialize()
        {
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
            var text = _localizationService != null
                ? _localizationService.Format(LocalizationKeys.HudScoreFormat, _value)
                : $"Score: {_value}";

            _levelUIPresenter.SetCounterText(text);
        }
    }
}
