using System;
using Assets.Project.Scripts.System.DessertCreator;
using Assets.Project.Scripts.System.DessertCreator.Dtos;
using MessagePipe;
using Project.Scripts.UI.LevelUI;
using VContainer.Unity;

namespace Project.Scripts.UI.UseCases
{
    public class DessertCountUseCase : IDessertCountUseCase, IInitializable, IDisposable
    {
        private readonly ILevelUIPresenter _levelUIPresenter;
        private readonly IDessertSpawner _dessertSpawner;
        private readonly ISubscriber<DessertCountsDto> _dessertCountsSubscriber;

        private IDisposable _subscription = DisposableBag.Empty;
        private int _value;

        public int CurrentValue => _value;

        public DessertCountUseCase(
            ILevelUIPresenter levelUIPresenter,
            IDessertSpawner dessertSpawner,
            ISubscriber<DessertCountsDto> dessertCountsSubscriber)
        {
            _levelUIPresenter = levelUIPresenter;
            _dessertSpawner = dessertSpawner;
            _dessertCountsSubscriber = dessertCountsSubscriber;
        }

        public void Initialize()
        {
            _subscription = _dessertCountsSubscriber.Subscribe(HandleCountsChanged);
            Refresh();
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        public void Refresh()
        {
            _value = _dessertSpawner?.ActiveDessertsCount ?? 0;
            NotifyPresenter();
        }

        public void Reset()
        {
            _value = 0;
            NotifyPresenter();
        }

        private void HandleCountsChanged(DessertCountsDto counts)
        {
            _value = counts.ActiveDessertsCount;
            NotifyPresenter();
        }

        private void NotifyPresenter()
        {
            _levelUIPresenter.SetTotalDessertsText(_value.ToString());
        }
    }
}
