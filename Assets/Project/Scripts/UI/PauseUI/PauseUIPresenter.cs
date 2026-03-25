using Cysharp.Threading.Tasks;
using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using UnityEngine;
using VContainer;

namespace Project.Scripts.UI.PauseUI
{
    public class PauseUIPresenter : LayoutPresenterBase<IPauseUIView>, IPauseUIPresenter, IGameStartListener, IGameFinishListener
    {
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;

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

        private static void HandleSettingsClicked()
        {
        }

        private static void HandleMenuClicked()
        {
        }
    }
}
