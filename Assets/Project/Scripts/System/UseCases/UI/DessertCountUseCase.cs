using System;
using Assets.Project.Scripts.System.DessertCreator;
using Project.Scripts.GameManager;
using Project.Scripts.UI.LevelUI;
using Project.System;
using VContainer.Unity;

namespace Project.Scripts.UI.UseCases
{
    public class DessertCountUseCase : IDessertCountUseCase, IInitializable, ITickable, IDisposable, IGameStartListener, IGameFinishListener
    {
        private readonly ILevelUIPresenter _levelUIPresenter;
        private readonly IDessertSpawner _dessertSpawner;
        private int _value;
        private bool _isRoundActive;

        public int CurrentValue => _value;

        public DessertCountUseCase(ILevelUIPresenter levelUIPresenter, IDessertSpawner dessertSpawner)
        {
            _levelUIPresenter = levelUIPresenter;
            _dessertSpawner = dessertSpawner;
        }

        public void Initialize()
        {
            IGameListener.Register(this);
            Refresh();
        }

        public void Dispose()
        {
            IGameListener.Unregister(this);
        }

        public void Tick()
        {
            if (_isRoundActive)
            {
                Refresh();
            }
        }

        public void OnStartGame()
        {
            _isRoundActive = true;
            Refresh();
        }

        public void OnFinishGame()
        {
            _isRoundActive = false;
            Refresh();
        }

        public void Refresh()
        {
            _value = _dessertSpawner?.TotalDessertsCount ?? 0;
            _levelUIPresenter.SetTotalDessertsText($"Desserts: {_value}");
        }

        public void Reset()
        {
            _value = 0;
            _levelUIPresenter.SetTotalDessertsText($"Desserts: {_value}");
        }
    }
}
