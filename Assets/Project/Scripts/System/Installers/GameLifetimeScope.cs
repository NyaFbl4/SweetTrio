using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;
using Project.Scripts.UI.MainScreen;
using Project.Scripts.UI.UseCases;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.System.Installers
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameManagerHelper _gameManagerHelper;
        [SerializeField] private LayoutsRepository _layoutsRepository;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterSystems(builder);
            RegisterHelpers(builder);
            RegisterUseCases(builder);
            RegisterViews(builder);
            RegisterPresenters(builder);
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
            builder.RegisterEntryPoint<UIController>().As<IUIController>();
            builder.RegisterEntryPoint<GameManagerService>().As<IGameManagerService>();
        }

        private void RegisterHelpers(IContainerBuilder builder)
        {
            if (_gameManagerHelper != null)
            {
                builder.RegisterComponent(_gameManagerHelper).AsSelf();
            }
            else
            {
                builder.RegisterComponentInHierarchy<GameManagerHelper>();
            }
        }

        private void RegisterUseCases(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<HidePopUpUseCase>(Lifetime.Singleton);
            builder.RegisterEntryPoint<ShowPopUpUseCase>(Lifetime.Singleton);
        }

        private void RegisterViews(IContainerBuilder builder)
        {
            if (_layoutsRepository == null || _layoutsRepository.Views == null)
                return;

            foreach (var prefab in _layoutsRepository.Views)
            {
                if (prefab == null)
                    continue;

                builder.RegisterComponentInNewPrefab(prefab, Lifetime.Scoped)
                    .AsSelf()
                    .AsImplementedInterfaces();
            }
        }

        private void RegisterPresenters(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainMenuPresenter>(Lifetime.Scoped);
        }
    }
}
