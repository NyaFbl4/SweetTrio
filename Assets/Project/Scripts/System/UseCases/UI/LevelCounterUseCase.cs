using VContainer.Unity;
using Project.Scripts.UI.LevelUI;

namespace Project.Scripts.UI.UseCases
{
    public class LevelCounterUseCase : ILevelCounterUseCase, IInitializable
    {
        private readonly ILevelUIPresenter _levelUIPresenter;
        private int _value;

        public int CurrentValue => _value;

        public LevelCounterUseCase(ILevelUIPresenter levelUIPresenter)
        {
            _levelUIPresenter = levelUIPresenter;
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
            _levelUIPresenter.SetCounterText($"Score: {_value}");
        }
    }
}
