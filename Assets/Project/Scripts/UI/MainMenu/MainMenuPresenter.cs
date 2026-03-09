using System;
using MessagePipe;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.UI.MainScreen
{
    public class MainMenuPresenter : LayoutPresenterBase<IMainMenuView>, IMainMenuPresenter, IInitializable, IDisposable
    {
        [Inject] private readonly IPublisher<ShowPopupDto> _showPopUpPublisher;
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;

        public void Initialize()
        {
            base.Initialize();
        }

        public void Dispose()
        {
            base.Dispose();
        }
    }
}
