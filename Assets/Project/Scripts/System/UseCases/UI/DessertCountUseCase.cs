using System;
using Assets.Project.Scripts.System.DessertCreator;
using Assets.Project.Scripts.System.DessertCreator.Dtos;
using MessagePipe;
using Project.Scripts.System.Localization;
using Project.Scripts.UI.LevelUI;
using VContainer.Unity;

namespace Project.Scripts.UI.UseCases
{
    public class DessertCountUseCase : IDessertCountUseCase, IInitializable, IDisposable
    {
        private readonly ILevelUIPresenter _levelUIPresenter;
        private readonly IDessertSpawner _dessertSpawner;
        private readonly ISubscriber<DessertCountsDto> _dessertCountsSubscriber;
        private readonly ILocalizationService _localizationService;

        private IDisposable _subscription = DisposableBag.Empty;
        private int _value;

        public int CurrentValue => _value;

        public DessertCountUseCase(
            ILevelUIPresenter levelUIPresenter,
            IDessertSpawner dessertSpawner,
            ISubscriber<DessertCountsDto> dessertCountsSubscriber,
            ILocalizationService localizationService)
        {
            _levelUIPresenter = levelUIPresenter;
            _dessertSpawner = dessertSpawner;
            _dessertCountsSubscriber = dessertCountsSubscriber;
            _localizationService = localizationService;
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
            var text = _localizationService != null
                ? _localizationService.Format(LocalizationKeys.HudDessertsFormat, _value)
                : $"Desserts: {_value}";

            _levelUIPresenter.SetTotalDessertsText(text);
        }
    }
}
