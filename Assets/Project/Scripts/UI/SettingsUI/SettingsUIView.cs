using System;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.SettingsUI
{
    public class SettingsUIView : LayoutViewBase, ISettingsUIView
    {
        private const string ToggleOnPath = "UI/Settings Panel/on";
        private const string ToggleOffPath = "UI/Settings Panel/off";

        private Button _closeButton;
        private Button _musicToggleButton;
        private Button _soundToggleButton;
        private Texture2D _toggleOnTexture;
        private Texture2D _toggleOffTexture;

        public event Action CloseClicked;
        public event Action MusicToggleClicked;
        public event Action SoundToggleClicked;

        public override void Awake()
        {
            base.Awake();

            _closeButton = _root.Q<Button>("settings-close-button");
            _musicToggleButton = _root.Q<Button>("settings-music-toggle-button");
            _soundToggleButton = _root.Q<Button>("settings-sound-toggle-button");

            if (_closeButton == null)
                Debug.LogError("SettingsUIView: Button 'settings-close-button' not found in UXML.");
            else
                _closeButton.clicked += HandleCloseClicked;

            if (_musicToggleButton == null)
                Debug.LogError("SettingsUIView: Button 'settings-music-toggle-button' not found in UXML.");
            else
                _musicToggleButton.clicked += HandleMusicToggleClicked;

            if (_soundToggleButton == null)
                Debug.LogError("SettingsUIView: Button 'settings-sound-toggle-button' not found in UXML.");
            else
                _soundToggleButton.clicked += HandleSoundToggleClicked;

            _toggleOnTexture = Resources.Load<Texture2D>(ToggleOnPath);
            _toggleOffTexture = Resources.Load<Texture2D>(ToggleOffPath);

            if (_toggleOnTexture == null || _toggleOffTexture == null)
                Debug.LogWarning("SettingsUIView: ON/OFF textures not found in Resources/UI/Settings Panel.");
        }

        private void OnDestroy()
        {
            if (_closeButton != null)
                _closeButton.clicked -= HandleCloseClicked;

            if (_musicToggleButton != null)
                _musicToggleButton.clicked -= HandleMusicToggleClicked;

            if (_soundToggleButton != null)
                _soundToggleButton.clicked -= HandleSoundToggleClicked;
        }

        public void SetMusicEnabled(bool isEnabled)
        {
            ApplyToggleVisual(_musicToggleButton, isEnabled);
        }

        public void SetSoundEnabled(bool isEnabled)
        {
            ApplyToggleVisual(_soundToggleButton, isEnabled);
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
    }
}
