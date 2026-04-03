using System;
using Project.Scripts.System.Localization;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Project.Scripts.UI.SettingsUI
{
    public class SettingsUIView : AnimatedPopupViewBase, ISettingsUIView
    {
        private const string ToggleOnPath = "UI/Settings Panel/on";
        private const string ToggleOffPath = "UI/Settings Panel/off";
        private const float VolumeStep = 0.05f;

        [Inject] private readonly ILocalizationService _localizationService;

        private Label _titleLabel;
        private Label _musicLabel;
        private Label _soundLabel;
        private Button _closeButton;
        private Button _musicToggleButton;
        private Button _soundToggleButton;
        private Button _musicVolumeMinusButton;
        private Button _musicVolumePlusButton;
        private Button _soundVolumeMinusButton;
        private Button _soundVolumePlusButton;
        private VisualElement _musicVolumeFill;
        private VisualElement _soundVolumeFill;
        private Texture2D _toggleOnTexture;
        private Texture2D _toggleOffTexture;
        private float _musicVolume = 1f;
        private float _soundVolume = 1f;

        protected override string OverlayElementName => "settings-overlay";
        protected override string PanelElementName => "settings-panel";

        public event Action CloseClicked;
        public event Action MusicToggleClicked;
        public event Action SoundToggleClicked;
        public event Action<float> MusicVolumeChanged;
        public event Action<float> SoundVolumeChanged;

        public override void Awake()
        {
            base.Awake();

            _titleLabel = _root.Q<Label>("setting-title-label");
            _musicLabel = _root.Q<Label>("settings-music-label");
            _soundLabel = _root.Q<Label>("settings-sound-label");
            _closeButton = _root.Q<Button>("settings-close-button");
            _musicToggleButton = _root.Q<Button>("settings-music-toggle-button");
            _soundToggleButton = _root.Q<Button>("settings-sound-toggle-button");

            _musicVolumeMinusButton = _root.Q<Button>("settings-music-volume-minus-button");
            _musicVolumePlusButton = _root.Q<Button>("settings-music-volume-plus-button");
            _soundVolumeMinusButton = _root.Q<Button>("settings-sound-volume-minus-button");
            _soundVolumePlusButton = _root.Q<Button>("settings-sound-volume-plus-button");
            _musicVolumeFill = _root.Q<VisualElement>("settings-music-volume-fill");
            _soundVolumeFill = _root.Q<VisualElement>("settings-sound-volume-fill");

            if (_closeButton == null)
                Debug.LogError("SettingsUIView: Button 'settings-close-button' not found in UXML.");
            else
            {
                UIButtonAnimationUtility.EnableDefault(_closeButton);
                _closeButton.clicked += HandleCloseClicked;
            }

            if (_musicToggleButton == null)
                Debug.LogError("SettingsUIView: Button 'settings-music-toggle-button' not found in UXML.");
            else
            {
                UIButtonAnimationUtility.EnableDefault(_musicToggleButton);
                _musicToggleButton.clicked += HandleMusicToggleClicked;
            }

            if (_soundToggleButton == null)
                Debug.LogError("SettingsUIView: Button 'settings-sound-toggle-button' not found in UXML.");
            else
            {
                UIButtonAnimationUtility.EnableDefault(_soundToggleButton);
                _soundToggleButton.clicked += HandleSoundToggleClicked;
            }

            if (_musicVolumeMinusButton == null || _musicVolumePlusButton == null || _musicVolumeFill == null)
            {
                Debug.LogError("SettingsUIView: music volume controls are not found in UXML.");
            }
            else
            {
                UIButtonAnimationUtility.EnableDefault(_musicVolumeMinusButton);
                UIButtonAnimationUtility.EnableDefault(_musicVolumePlusButton);
                _musicVolumeMinusButton.clicked += HandleMusicVolumeMinusClicked;
                _musicVolumePlusButton.clicked += HandleMusicVolumePlusClicked;
            }

            if (_soundVolumeMinusButton == null || _soundVolumePlusButton == null || _soundVolumeFill == null)
            {
                Debug.LogError("SettingsUIView: sound volume controls are not found in UXML.");
            }
            else
            {
                UIButtonAnimationUtility.EnableDefault(_soundVolumeMinusButton);
                UIButtonAnimationUtility.EnableDefault(_soundVolumePlusButton);
                _soundVolumeMinusButton.clicked += HandleSoundVolumeMinusClicked;
                _soundVolumePlusButton.clicked += HandleSoundVolumePlusClicked;
            }

            _toggleOnTexture = Resources.Load<Texture2D>(ToggleOnPath);
            _toggleOffTexture = Resources.Load<Texture2D>(ToggleOffPath);

            if (_toggleOnTexture == null || _toggleOffTexture == null)
                Debug.LogWarning("SettingsUIView: ON/OFF textures not found in Resources/UI/Settings Panel.");

            if (_titleLabel != null)
                _titleLabel.text = GetLocalizedText(LocalizationKeys.SettingsTitle, _titleLabel.text);

            if (_musicLabel != null)
                _musicLabel.text = GetLocalizedText(LocalizationKeys.SettingsMusicLabel, _musicLabel.text);

            if (_soundLabel != null)
                _soundLabel.text = GetLocalizedText(LocalizationKeys.SettingsSoundLabel, _soundLabel.text);
        }

        private void OnDestroy()
        {
            if (_closeButton != null)
                _closeButton.clicked -= HandleCloseClicked;

            if (_musicToggleButton != null)
                _musicToggleButton.clicked -= HandleMusicToggleClicked;

            if (_soundToggleButton != null)
                _soundToggleButton.clicked -= HandleSoundToggleClicked;

            if (_musicVolumeMinusButton != null)
                _musicVolumeMinusButton.clicked -= HandleMusicVolumeMinusClicked;

            if (_musicVolumePlusButton != null)
                _musicVolumePlusButton.clicked -= HandleMusicVolumePlusClicked;

            if (_soundVolumeMinusButton != null)
                _soundVolumeMinusButton.clicked -= HandleSoundVolumeMinusClicked;

            if (_soundVolumePlusButton != null)
                _soundVolumePlusButton.clicked -= HandleSoundVolumePlusClicked;
        }

        public void SetMusicEnabled(bool isEnabled)
        {
            ApplyToggleVisual(_musicToggleButton, isEnabled);
        }

        public void SetSoundEnabled(bool isEnabled)
        {
            ApplyToggleVisual(_soundToggleButton, isEnabled);
        }

        public void SetMusicVolume(float value)
        {
            _musicVolume = Mathf.Clamp01(value);
            ApplyVolumeFill(_musicVolumeFill, _musicVolume);
        }

        public void SetSoundVolume(float value)
        {
            _soundVolume = Mathf.Clamp01(value);
            ApplyVolumeFill(_soundVolumeFill, _soundVolume);
        }

        private void HandleCloseClicked()
        {
            CloseClicked?.Invoke();
        }

        private void HandleMusicToggleClicked()
        {
            MusicToggleClicked?.Invoke();
        }

        private void HandleSoundToggleClicked()
        {
            SoundToggleClicked?.Invoke();
        }

        private void HandleMusicVolumeMinusClicked()
        {
            ChangeMusicVolume(-VolumeStep);
        }

        private void HandleMusicVolumePlusClicked()
        {
            ChangeMusicVolume(VolumeStep);
        }

        private void HandleSoundVolumeMinusClicked()
        {
            ChangeSoundVolume(-VolumeStep);
        }

        private void HandleSoundVolumePlusClicked()
        {
            ChangeSoundVolume(VolumeStep);
        }

        private void ChangeMusicVolume(float delta)
        {
            var newValue = Mathf.Clamp01(_musicVolume + delta);
            if (Mathf.Approximately(newValue, _musicVolume))
                return;

            SetMusicVolume(newValue);
            MusicVolumeChanged?.Invoke(newValue);
        }

        private void ChangeSoundVolume(float delta)
        {
            var newValue = Mathf.Clamp01(_soundVolume + delta);
            if (Mathf.Approximately(newValue, _soundVolume))
                return;

            SetSoundVolume(newValue);
            SoundVolumeChanged?.Invoke(newValue);
        }

        private static void ApplyVolumeFill(VisualElement fill, float value)
        {
            if (fill == null)
                return;

            fill.style.width = Length.Percent(Mathf.Clamp01(value) * 100f);
        }

        private void ApplyToggleVisual(Button button, bool isEnabled)
        {
            if (button == null)
                return;

            var texture = isEnabled ? _toggleOnTexture : _toggleOffTexture;
            if (texture != null)
            {
                button.style.backgroundImage = new StyleBackground(texture);
                button.text = string.Empty;
                return;
            }

            button.text = isEnabled ? "ON" : "OFF";
        }

        private string GetLocalizedText(string key, string fallback)
        {
            if (_localizationService == null)
                return fallback;

            var text = _localizationService.Get(key);
            return string.IsNullOrWhiteSpace(text) ? fallback : text;
        }
    }
}
