using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using VContainer;

namespace Project.Scripts.UI.MainScreen
{
    public class MainMenuPresenter : LayoutPresenterBase<IMainMenuView>, IMainMenuPresenter
    {
        [Inject] private readonly IGameManagerService _gameManagerService;
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;

        public override void Initialize()
        {
            base.Initialize();
            _layoutView.StartLevelClicked += HandleStartLevelClicked;
            _layoutView.Show();
        }

        public override void Dispose()
        {
            _layoutView.StartLevelClicked -= HandleStartLevelClicked;
            base.Dispose();
        }

        private void HandleStartLevelClicked()
        {
            _hidePopUpPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(IMainMenuPresenter)
            });

            _gameManagerService.StartGame();
        }
    }
}
