using System;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.PauseUI
{
    public class PauseUIView : LayoutViewBase, IPauseUIView
    {
        private Button _playButton;
        private Button _settingsButton;
        private Button _menuButton;

        public event Action PlayClicked;
        public event Action SettingsClicked;
        public event Action MenuClicked;

        public override void Awake()
        {
            base.Awake();

            _playButton = _root.Q<Button>("pause-play-button");
            _settingsButton = _root.Q<Button>("pause-settings-button");
            _menuButton = _root.Q<Button>("pause-menu-button");

            if (_playButton == null)
                Debug.LogError("PauseUIView: Button 'pause-play-button' not found in UXML.");
            else
                _playButton.clicked += HandlePlayClicked;

            if (_settingsButton == null)
                Debug.LogError("PauseUIView: Button 'pause-settings-button' not found in UXML.");
            else
                _settingsButton.clicked += HandleSettingsClicked;

            if (_menuButton == null)
                Debug.LogError("PauseUIView: Button 'pause-menu-button' not found in UXML.");
            else
                _menuButton.clicked += HandleMenuClicked;
        }

        private void OnDestroy()
        {
            if (_playButton != null)
                _playButton.clicked -= HandlePlayClicked;

            if (_settingsButton != null)
                _settingsButton.clicked -= HandleSettingsClicked;

            if (_menuButton != null)
                _menuButton.clicked -= HandleMenuClicked;
        }

        private void HandlePlayClicked()
        {
            PlayClicked?.Invoke();
        }

        private void HandleSettingsClicked()
        {
            SettingsClicked?.Invoke();
        }

        private void HandleMenuClicked()
        {
            MenuClicked?.Invoke();
        }
    }
}
