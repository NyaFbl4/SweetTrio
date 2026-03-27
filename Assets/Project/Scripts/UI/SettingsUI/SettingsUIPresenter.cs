using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using VContainer;

namespace Project.Scripts.UI.SettingsUI
{
    public class SettingsUIPresenter : LayoutPresenterBase<ISettingsUIView>, ISettingsUIPresenter, IGameStartListener, IGameFinishListener
    {
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;

        private bool _isMusicEnabled = true;
        private bool _isSoundEnabled = true;

        public override void Initialize()
        {
            base.Initialize();
            IGameListener.Register(this);
            _layoutView.CloseClicked += HandleCloseClicked;
            _layoutView.MusicToggleClicked += HandleMusicToggleClicked;
            _layoutView.SoundToggleClicked += HandleSoundToggleClicked;
            _layoutView.SetMusicEnabled(_isMusicEnabled);
            _layoutView.SetSoundEnabled(_isSoundEnabled);
        }

        public override void Dispose()
        {
            _layoutView.CloseClicked -= HandleCloseClicked;
            _layoutView.MusicToggleClicked -= HandleMusicToggleClicked;
            _layoutView.SoundToggleClicked -= HandleSoundToggleClicked;
            IGameListener.Unregister(this);
            base.Dispose();
        }

        public void OnStartGame()
        {
            HideSettingsPopup();
        }

        public void OnFinishGame()
        {
            HideSettingsPopup();
        }

        private void HandleCloseClicked()
        {
            HideSettingsPopup();
        }

        private void HandleMusicToggleClicked()
        {
            _isMusicEnabled = !_isMusicEnabled;
            _layoutView.SetMusicEnabled(_isMusicEnabled);
        }

        private void HandleSoundToggleClicked()
        {
            _isSoundEnabled = !_isSoundEnabled;
            _layoutView.SetSoundEnabled(_isSoundEnabled);
        }

        private void HideSettingsPopup()
        {
            _hidePopUpPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(ISettingsUIPresenter)
            });
        }
    }
}
