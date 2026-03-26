using Cysharp.Threading.Tasks;
using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.MainScreen;
using UnityEngine;
using VContainer;

namespace Project.Scripts.UI.PauseUI
{
    public class PauseUIPresenter : LayoutPresenterBase<IPauseUIView>, IPauseUIPresenter, IGameStartListener, IGameFinishListener
    {
        [Inject] private readonly IGameManagerService _gameManagerService;
        [Inject] private readonly IPublisher<ShowPopupDto> _showPopUpPublisher;
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;
        [Inject] private readonly IPublisher<OpenPauseSettingsDto> _openPauseSettingsPublisher;

        public override void Initialize()
        {
            base.Initialize();
            IGameListener.Register(this);
            _layoutView.PlayClicked += HandlePlayClicked;
            _layoutView.SettingsClicked += HandleSettingsClicked;
            _layoutView.MenuClicked += HandleMenuClicked;
        }

        public override void Dispose()
        {
            _layoutView.PlayClicked -= HandlePlayClicked;
            _layoutView.SettingsClicked -= HandleSettingsClicked;
            _layoutView.MenuClicked -= HandleMenuClicked;
            IGameListener.Unregister(this);
            base.Dispose();
        }

        public override async UniTask ActivateAsync()
        {
            Time.timeScale = 0f;
            await base.ActivateAsync();
        }

        public override async UniTask DeactivateAsync()
        {
            await base.DeactivateAsync();
            Time.timeScale = 1f;
        }

        public void OnStartGame()
        {
            _hidePopUpPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(IPauseUIPresenter)
            });
        }

        public void OnFinishGame()
        {
            _hidePopUpPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(IPauseUIPresenter)
            });
        }

        private void HandlePlayClicked()
        {
            _hidePopUpPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(IPauseUIPresenter)
            });
        }

        private void HandleSettingsClicked()
        {
            _openPauseSettingsPublisher.Publish(new OpenPauseSettingsDto());
        }

        private void HandleMenuClicked()
        {
            _gameManagerService.FinishGame();

            _showPopUpPublisher.Publish(new ShowPopupDto
            {
                TargetPopUpType = typeof(IMainMenuPresenter)
            });
        }
    }
}
