using System;
using System.Collections.Generic;
using Project.Scripts.System.Localization;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Project.Scripts.UI.EndGame
{
    public class EndGameView : LayoutViewBase, IEndGameView
    {
        private static readonly Color ActiveStarColor = new(1f, 0.85f, 0.2f, 1f);
        private static readonly Color InactiveStarColor = new(0.76f, 0.78f, 0.83f, 1f);

        [Inject] private readonly ILocalizationService _localizationService;

        private Label _titleLabel;
        private Label _scoreLabel;
        private Label _completionLabel;
        private VisualElement _starsContainer;
        private Button _exitToMenuButton;
        private readonly List<VisualElement> _starDots = new();
        private bool _isInitialized;

        public event Action ExitToMenuClicked;

        public override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            InitializeIfNeeded();
        }

        private void OnEnable()
        {
            if (_isInitialized || _root == null)
                return;

            _root.schedule.Execute(InitializeIfNeeded);
        }

        private void OnDestroy()
        {
            if (_exitToMenuButton != null)
            {
                _exitToMenuButton.clicked -= HandleExitToMenuClicked;
            }
        }

        public void SetTitle(string message)
        {
            if (_titleLabel == null)
                return;

            _titleLabel.text = message;
        }

        public void SetScoreText(string text)
        {
            if (_scoreLabel == null)
                return;

            _scoreLabel.text = text;
        }

        public void SetScoreVisible(bool isVisible)
        {
            if (_scoreLabel == null)
                return;

            _scoreLabel.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetCompletionText(string text)
        {
            if (_completionLabel == null)
                return;

            _completionLabel.text = text;
        }

        public void SetCompletionVisible(bool isVisible)
        {
            if (_completionLabel == null)
                return;

            _completionLabel.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetStars(int activeStarsCount, int totalStarsCount = 3)
        {
            if (_starsContainer == null)
                return;

            var total = Mathf.Max(0, totalStarsCount);
            var active = Mathf.Clamp(activeStarsCount, 0, total);

            EnsureStarsCount(total);
            for (var i = 0; i < _starDots.Count; i++)
            {
                _starDots[i].style.backgroundColor = i < active ? ActiveStarColor : InactiveStarColor;
            }
        }

        public void SetStarsVisible(bool isVisible)
        {
            if (_starsContainer == null)
                return;

            _starsContainer.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void InitializeIfNeeded()
        {
            if (_isInitialized || _root == null)
                return;

            QueryElements();

            var hasAllCriticalElements = _titleLabel != null && _scoreLabel != null && _starsContainer != null && _exitToMenuButton != null;
            if (!hasAllCriticalElements)
            {
                BuildFallbackCenteredLayout();
                QueryElements();
            }

            if (_titleLabel == null || _scoreLabel == null || _starsContainer == null || _exitToMenuButton == null)
            {
                Debug.LogError("EndGameView: failed to initialize required UI elements.");
                return;
            }

            _exitToMenuButton.text = GetLocalizedText(LocalizationKeys.EndGameMenuButton, "Menu");
            _exitToMenuButton.clicked -= HandleExitToMenuClicked;
            _exitToMenuButton.clicked += HandleExitToMenuClicked;

            SetStars(activeStarsCount: 0, totalStarsCount: 3);
            SetCompletionVisible(false);

            _isInitialized = true;
        }

        private void QueryElements()
        {
            _titleLabel = _root.Q<Label>("end-game-title-label");
            _scoreLabel = _root.Q<Label>("end-game-score-label");
            _completionLabel = _root.Q<Label>("end-game-completion-label");
            _starsContainer = _root.Q<VisualElement>("end-game-stars-container");
            _exitToMenuButton = _root.Q<Button>("end-game-menu-button");
        }

        private void BuildFallbackCenteredLayout()
        {
            if (_root == null)
                return;

            _root.Clear();

            var overlay = new VisualElement { name = "end-game-overlay" };
            overlay.style.position = Position.Absolute;
            overlay.style.left = 0f;
            overlay.style.right = 0f;
            overlay.style.top = 0f;
            overlay.style.bottom = 0f;
            overlay.style.backgroundColor = new Color(0f, 0f, 0f, 0.55f);
            overlay.style.alignItems = Align.Center;
            overlay.style.justifyContent = Justify.Center;
            _root.Add(overlay);

            var panel = new VisualElement { name = "end-game-panel" };
            panel.style.width = 420f;
            panel.style.minHeight = 260f;
            panel.style.maxWidth = 520f;
            panel.style.backgroundColor = new Color(1f, 1f, 1f, 0.95f);
            panel.style.borderTopLeftRadius = 14f;
            panel.style.borderTopRightRadius = 14f;
            panel.style.borderBottomLeftRadius = 14f;
            panel.style.borderBottomRightRadius = 14f;
            panel.style.alignItems = Align.Center;
            panel.style.justifyContent = Justify.Center;
            panel.style.paddingTop = 20f;
            panel.style.paddingBottom = 20f;
            overlay.Add(panel);

            _titleLabel = new Label
            {
                name = "end-game-title-label"
            };
            _titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            _titleLabel.style.fontSize = 36f;
            _titleLabel.style.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            panel.Add(_titleLabel);

            _scoreLabel = new Label
            {
                name = "end-game-score-label"
            };
            _scoreLabel.style.marginTop = 12f;
            _scoreLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _scoreLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            _scoreLabel.style.fontSize = 26f;
            _scoreLabel.style.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            panel.Add(_scoreLabel);

            _completionLabel = new Label
            {
                name = "end-game-completion-label"
            };
            _completionLabel.style.display = DisplayStyle.None;
            panel.Add(_completionLabel);

            _starsContainer = new VisualElement
            {
                name = "end-game-stars-container"
            };
            _starsContainer.style.marginTop = 16f;
            _starsContainer.style.flexDirection = FlexDirection.Row;
            _starsContainer.style.alignItems = Align.Center;
            _starsContainer.style.justifyContent = Justify.Center;
            _starsContainer.style.minHeight = 28f;
            panel.Add(_starsContainer);

            _exitToMenuButton = new Button
            {
                name = "end-game-menu-button",
                text = GetLocalizedText(LocalizationKeys.EndGameMenuButton, "Menu")
            };
            _exitToMenuButton.style.marginTop = 20f;
            _exitToMenuButton.style.width = 180f;
            _exitToMenuButton.style.height = 52f;
            _exitToMenuButton.style.unityFontStyleAndWeight = FontStyle.Bold;
            _exitToMenuButton.style.fontSize = 22f;
            panel.Add(_exitToMenuButton);
        }

        private void EnsureStarsCount(int totalStarsCount)
        {
            if (_starsContainer == null)
                return;

            _starDots.Clear();
            _starsContainer.Clear();

            for (var i = 0; i < totalStarsCount; i++)
            {
                var starDot = new VisualElement();
                starDot.style.width = 24f;
                starDot.style.height = 24f;
                starDot.style.marginLeft = 6f;
                starDot.style.marginRight = 6f;
                starDot.style.borderTopLeftRadius = 12f;
                starDot.style.borderTopRightRadius = 12f;
                starDot.style.borderBottomLeftRadius = 12f;
                starDot.style.borderBottomRightRadius = 12f;
                starDot.style.backgroundColor = InactiveStarColor;

                _starsContainer.Add(starDot);
                _starDots.Add(starDot);
            }
        }

        private string GetLocalizedText(string key, string fallback)
        {
            if (_localizationService == null)
                return fallback;

            var text = _localizationService.Get(key);
            return string.IsNullOrWhiteSpace(text) ? fallback : text;
        }

        private void HandleExitToMenuClicked()
        {
            ExitToMenuClicked?.Invoke();
        }
    }
}
