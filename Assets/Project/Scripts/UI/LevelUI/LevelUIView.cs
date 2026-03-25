using System;
using Project.Scripts.GameManager;
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
        [Inject] private readonly ILevelSelectionService _levelSelectionService;

        private Label _counterLabel;
        private Label _totalDessertsLabel;
        private Label _countdownLabel;
        private Label _bonusMultiplierLabel;
        private VisualElement _levelProgressBar;
        private VisualElement _levelProgressFill;
        private VisualElement _progressStar1;
        private VisualElement _progressStar2;
        private VisualElement _progressStar3;
        private VisualElement _bonusDessertImage;
        private Button _shuffleButton;
        private Button _exitToMenuButton;
        private Button _pauseButton;

        public event Action ShuffleButtonClicked;
        public event Action ExitToMenuClicked;
        public event Action PauseButtonClicked;

        public override void Awake()
        {
            base.Awake();
            var scorePanel = _root.Q<VisualElement>("score-panel");
            _counterLabel = _root.Q<Label>("score-counter-label")
                            ?? _root.Q<Label>("level-counter-label")
                            ?? scorePanel?.Q<Label>();
            _totalDessertsLabel = _root.Q<Label>("total-desserts-label");
            _countdownLabel = _root.Q<Label>("timer-countdown-label");
            _bonusMultiplierLabel = _root.Q<Label>("bonus-multiplier-label");
            _levelProgressBar = _root.Q<VisualElement>("level-progress-bar");
            _levelProgressFill = _root.Q<VisualElement>("level-progress-fill");
            _progressStar1 = _root.Q<VisualElement>("progress-star-1");
            _progressStar2 = _root.Q<VisualElement>("progress-star-2");
            _progressStar3 = _root.Q<VisualElement>("progress-star-3");
            _bonusDessertImage = _root.Q<VisualElement>("bonus-dessert-image");
            _shuffleButton = _root.Q<Button>("shuffle-button");
            _exitToMenuButton = _root.Q<Button>("gameplay-menu-button");
            _pauseButton = _root.Q<Button>("pause-button");

            if (_counterLabel == null)
            {
                Debug.LogError("LevelUIView: Score label not found. Expected 'score-counter-label' in 'score-panel'.");
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

            ConfigureProgressBar();

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

            if (_pauseButton == null)
            {
                Debug.LogError("LevelUIView: Button 'pause-button' not found in UXML.");
            }
            else
            {
                _pauseButton.clicked += OnPauseButtonClicked;
            }

            ConfigureProgressStars();
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

            if (_pauseButton != null)
            {
                _pauseButton.clicked -= OnPauseButtonClicked;
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
            if (_levelProgressFill == null)
                return;

            _levelProgressFill.style.width = Length.Percent(Mathf.Clamp01(value01) * 100f);
            _levelProgressFill.MarkDirtyRepaint();
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

        private void ConfigureProgressBar()
        {
            if (_levelProgressBar == null)
            {
                Debug.LogError("LevelUIView: VisualElement 'level-progress-bar' not found in UXML.");
                return;
            }

            if (_levelProgressFill == null)
            {
                Debug.LogError("LevelUIView: VisualElement 'level-progress-fill' not found in UXML.");
                return;
            }

            var backgroundSprite = Resources.Load<Sprite>("UI/Level UI/bgload");
            if (backgroundSprite == null)
            {
                Debug.LogError("LevelUIView: sprite 'UI/Level UI/bgload' not found.");
            }

            _levelProgressBar.style.backgroundImage = backgroundSprite != null
                ? new StyleBackground(backgroundSprite)
                : StyleKeyword.None;
            _levelProgressBar.style.backgroundColor = Color.clear;
            _levelProgressBar.style.unityBackgroundImageTintColor = Color.white;
            _levelProgressBar.style.overflow = Overflow.Hidden;

            var fillSprite = Resources.Load<Sprite>("UI/Level UI/load");
            if (fillSprite == null)
            {
                Debug.LogError("LevelUIView: sprite 'UI/Level UI/load' not found.");
            }

            _levelProgressFill.style.backgroundImage = fillSprite != null
                ? new StyleBackground(fillSprite)
                : StyleKeyword.None;
            _levelProgressFill.style.backgroundColor = Color.clear;
            _levelProgressFill.style.unityBackgroundImageTintColor = Color.white;
            _levelProgressFill.style.left = 0f;
            _levelProgressFill.style.top = 0f;
            _levelProgressFill.style.bottom = 0f;
            _levelProgressFill.style.width = Length.Percent(0f);
        }

        private void ConfigureProgressStars()
        {
            var levelConfig = _levelSelectionService?.CurrentLevel;
            if (levelConfig == null)
                return;

            var threeStarsScore = Mathf.Max(1, levelConfig.ThreeStarsScore);
            var oneRatio = Mathf.Clamp01((float)levelConfig.OneStarScore / threeStarsScore);
            var twoRatio = Mathf.Clamp01((float)levelConfig.TwoStarsScore / threeStarsScore);
            var threeRatio = Mathf.Clamp01((float)levelConfig.ThreeStarsScore / threeStarsScore);

            SetProgressStarPosition(_progressStar1, oneRatio, pinToEnd: false);
            SetProgressStarPosition(_progressStar2, twoRatio, pinToEnd: false);
            SetProgressStarPosition(_progressStar3, threeRatio, pinToEnd: true);
        }

        private static void SetProgressStarPosition(VisualElement starMarker, float ratio, bool pinToEnd)
        {
            if (starMarker == null)
                return;

            if (pinToEnd)
            {
                starMarker.style.left = StyleKeyword.Auto;
                starMarker.style.right = 0f;
                return;
            }

            starMarker.style.right = StyleKeyword.Auto;
            starMarker.style.left = Length.Percent(Mathf.Clamp01(ratio) * 100f);
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

        private void OnPauseButtonClicked()
        {
            PauseButtonClicked?.Invoke();
        }
    }
}
