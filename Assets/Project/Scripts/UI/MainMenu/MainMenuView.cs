using System;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.MainScreen
{
    public class MainMenuView : LayoutViewBase, IMainMenuView
    {
        private const string StartButtonName = "main-menu-start-button";

        private Button _startButton;

        public event Action StartLevelClicked;

        public override void Awake()
        {
            base.Awake();

            _startButton = _root.Q<Button>(StartButtonName);
            if (_startButton == null)
            {
                Debug.LogError($"MainMenuView: Button '{StartButtonName}' not found in UXML.");
                return;
            }

            _startButton.clicked += HandleStartButtonClicked;
        }

        private void OnDestroy()
        {
            if (_startButton != null)
            {
                _startButton.clicked -= HandleStartButtonClicked;
            }
        }

        private void HandleStartButtonClicked()
        {
            StartLevelClicked?.Invoke();
        }
    }
}
