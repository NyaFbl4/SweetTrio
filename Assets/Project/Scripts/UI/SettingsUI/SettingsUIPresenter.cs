using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.System.Audio;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using VContainer;

namespace Project.Scripts.UI.SettingsUI
{
    public class SettingsUIPresenter : LayoutPresenterBase<ISettingsUIView>, ISettingsUIPresenter, IGameStartListener, IGameFinishListener
    {
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;
        [Inject] private readonly ISoundManager _soundManager;

        public override void Initialize()
        {
            base.Initialize();
            IGameListener.Register(this);
            _layoutView.CloseClicked += HandleCloseClicked;
            _layoutView.MusicToggleClicked += HandleMusicToggleClicked;
            _layoutView.SoundToggleClicked += HandleSoundToggleClicked;
            _layoutView.MusicVolumeChanged += HandleMusicVolumeChanged;
            _layoutView.SoundVolumeChanged += HandleSoundVolumeChanged;

            _layoutView.SetMusicEnabled(_soundManager.IsMusicEnabled);
            _layoutView.SetSoundEnabled(_soundManager.IsSoundEnabled);
            _layoutView.SetMusicVolume(_soundManager.MusicVolume);
            _layoutView.SetSoundVolume(_soundManager.SoundVolume);
        }

        public override void Dispose()
        {
            _layoutView.CloseClicked -= HandleCloseClicked;
            _layoutView.MusicToggleClicked -= HandleMusicToggleClicked;
            _layoutView.SoundToggleClicked -= HandleSoundToggleClicked;
            _layoutView.MusicVolumeChanged -= HandleMusicVolumeChanged;
            _layoutView.SoundVolumeChanged -= HandleSoundVolumeChanged;
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
            var newValue = !_soundManager.IsMusicEnabled;
            _soundManager.SetMusicEnabled(newValue);
            _layoutView.SetMusicEnabled(newValue);
        }

        private void HandleSoundToggleClicked()
        {
            var newValue = !_soundManager.IsSoundEnabled;
            _soundManager.SetSoundEnabled(newValue);
            _layoutView.SetSoundEnabled(newValue);
        }

        private void HandleMusicVolumeChanged(float value)
        {
            _soundManager.SetMusicVolume(value);
        }

        private void HandleSoundVolumeChanged(float value)
        {
            _soundManager.SetSoundVolume(value);
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