using System;
using Project.Scripts.System.Localization;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Project.Scripts.UI.LevelUI
{
    public class LevelUIView : LayoutViewBase, ILevelUIView
    {
        [Inject] private readonly ILocalizationService _localizationService;

        private Label _counterLabel;
        private Label _totalDessertsLabel;
        private Label _countdownLabel;
        private Label _bonusMultiplierLabel;
        private VisualElement _fillElement;
        private VisualElement _bonusDessertImage;
        private Button _shuffleButton;
        private Button _exitToMenuButton;

        public event Action ShuffleButtonClicked;
        public event Action ExitToMenuClicked;

        public override void Awake()
        {
            base.Awake();
            _counterLabel = _root.Q<Label>("level-counter-label");
            _totalDessertsLabel = _root.Q<Label>("total-desserts-label");
            _countdownLabel = _root.Q<Label>("timer-countdown-label");
            _bonusMultiplierLabel = _root.Q<Label>("bonus-multiplier-label");
            _fillElement = _root.Q<VisualElement>("timer-progress-fill");
            _bonusDessertImage = _root.Q<VisualElement>("bonus-dessert-image");
            _shuffleButton = _root.Q<Button>("shuffle-button");
            _exitToMenuButton = _root.Q<Button>("gameplay-menu-button");

            if (_counterLabel == null)
            {
                Debug.LogError("LevelUIView: Label 'level-counter-label' not found in UXML.");
            }

            if (_totalDessertsLabel == null)
            {
                Debug.LogError("LevelUIView: Label 'total-desserts-label' not found in UXML.");
            }

            if (_countdownLabel == null)
            {
                Debug.LogError("LevelUIView: Label 'timer-countdown-label' not found in UXML.");
            }

            if (_bonusMultiplierLabel == null)
            {
                Debug.LogError("LevelUIView: Label 'bonus-multiplier-label' not found in UXML.");
            }

            if (_fillElement == null)
            {
                Debug.LogError("LevelUIView: VisualElement 'timer-progress-fill' not found in UXML.");
            }

            if (_bonusDessertImage == null)
            {
                Debug.LogError("LevelUIView: VisualElement 'bonus-dessert-image' not found in UXML.");
            }

            if (_shuffleButton == null)
            {
                Debug.LogError("LevelUIView: Button 'shuffle-button' not found in UXML.");
            }
            else
            {
                _shuffleButton.text = GetLocalizedText(LocalizationKeys.HudShuffleButton, _shuffleButton.text);
                _shuffleButton.clicked += OnShuffleButtonClicked;
            }

            if (_exitToMenuButton == null)
            {
                Debug.LogError("LevelUIView: Button 'gameplay-menu-button' not found in UXML.");
            }
            else
            {
                _exitToMenuButton.text = GetLocalizedText(LocalizationKeys.HudMenuButton, _exitToMenuButton.text);
                _exitToMenuButton.clicked += OnExitToMenuButtonClicked;
            }
        }

        private void OnDestroy()
        {
            if (_shuffleButton != null)
            {
                _shuffleButton.clicked -= OnShuffleButtonClicked;
            }

            if (_exitToMenuButton != null)
            {
                _exitToMenuButton.clicked -= OnExitToMenuButtonClicked;
            }
        }

        public void SetCounter(int value)
        {
            SetCounterText(value.ToString());
        }

        public void SetCounterText(string text)
        {
            if (_counterLabel == null)
                return;

            _counterLabel.text = text;
        }

        public void SetTotalDessertsText(string text)
        {
            if (_totalDessertsLabel == null)
                return;

            _totalDessertsLabel.text = text;
        }

        public void SetTimerText(string text)
        {
            if (_countdownLabel == null)
                return;

            _countdownLabel.text = text;
        }

        public void SetProgress(float value01)
        {
            if (_fillElement == null)
                return;

            var clampedValue = Mathf.Clamp01(value01);
            _fillElement.style.width = Length.Percent(clampedValue * 100f);
        }

        public void SetBonusDessertSprite(Sprite sprite)
        {
            if (_bonusDessertImage == null)
                return;

            if (sprite == null)
            {
                _bonusDessertImage.style.backgroundImage = StyleKeyword.None;
                return;
            }

            _bonusDessertImage.style.backgroundImage = new StyleBackground(sprite);
        }

        public void SetBonusMultiplierText(string text)
        {
            if (_bonusMultiplierLabel == null)
                return;

            _bonusMultiplierLabel.text = text;
        }

        private string GetLocalizedText(string key, string fallback)
        {
            if (_localizationService == null)
                return fallback;

            var text = _localizationService.Get(key);
            return string.IsNullOrWhiteSpace(text) ? fallback : text;
        }

        private void OnShuffleButtonClicked()
        {
            ShuffleButtonClicked?.Invoke();
        }

        private void OnExitToMenuButtonClicked()
        {
            ExitToMenuClicked?.Invoke();
        }
    }
}
