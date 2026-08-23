using System;
using System.Collections.Generic;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.EndGame
{
    public class EndGameView : AnimatedPopupViewBase, IEndGameView
    {
        private static readonly string[] StarsSpriteByActiveCount =
        {
            "UI/EndGamePanel/star_4",
            "UI/EndGamePanel/star_3",
            "UI/EndGamePanel/star_2",
            "UI/EndGamePanel/star_1"
        };

        private readonly Dictionary<string, Sprite> _spriteCache = new();

        private Label _titleLabel;
        private Label _scoreLabel;
        private Label _completionLabel;
        private VisualElement _starsContainer;
        private Button _restartButton;
        private Button _nextLevelButton;
        private Button _menuButton;

        protected override string OverlayElementName => "end-game-overlay";
        protected override string PanelElementName => "end-game-panel";

        public event Action RestartButtonClicked;
        public event Action NextLevelButtonClicked;
        public event Action MenuButtonClicked;

        public override void Awake()
        {
            base.Awake();

            QueryElements();
            EnsureRequiredElements();

            if (_restartButton != null)
            {
                UIButtonAnimationUtility.EnableDefault(_restartButton);
                _restartButton.clicked += HandleRestartActionClicked;
            }

            if (_nextLevelButton != null)
            {
                UIButtonAnimationUtility.EnableDefault(_nextLevelButton);
                _nextLevelButton.clicked += HandleNextLevelButtonClicked;
            }
            
            if (_menuButton != null)
            {
                UIButtonAnimationUtility.EnableDefault(_menuButton);
                _menuButton.clicked += HandleMenuActionClicked;
            }

            SetCompletionVisible(false);
            SetStars(activeStarsCount: 0, totalStarsCount: 3);
        }

        private void OnDestroy()
        {
            if (_restartButton != null)
            {
                UIButtonAnimationUtility.EnableDefault(_restartButton);
                _restartButton.clicked -= HandleRestartActionClicked;
            }

            if (_nextLevelButton != null)
            {
                UIButtonAnimationUtility.EnableDefault(_nextLevelButton);
                _nextLevelButton.clicked -= HandleNextLevelButtonClicked;
            }
            
            if (_menuButton != null)
            {
                UIButtonAnimationUtility.EnableDefault(_menuButton);
                _menuButton.clicked -= HandleMenuActionClicked;
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
            var normalizedActiveStars = NormalizeStarsCount(active, total);
            var starsSpritePath = StarsSpriteByActiveCount[normalizedActiveStars];
            ApplyBackgroundSprite(_starsContainer, starsSpritePath);
        }

        public void SetStarsVisible(bool isVisible)
        {
            if (_starsContainer == null)
                return;

            _starsContainer.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void QueryElements()
        {
            _titleLabel = _root.Q<Label>("end-game-title-label");
            _scoreLabel = _root.Q<Label>("end-game-score-label");
            _completionLabel = _root.Q<Label>("end-game-completion-label");
            _starsContainer = _root.Q<VisualElement>("end-game-stars-container");
            _restartButton = _root.Q<Button>("end-game-restart-button") ?? _root.Q<Button>("end-game-primary-button");
            _nextLevelButton = _root.Q<Button>("end-game-next-level-button");
            _menuButton = _root.Q<Button>("end-game-menu-button") ?? _root.Q<Button>("end-game-secondary-button");
        }

        public void SetNextLevelButtonVisible(bool isVisible)
        {
            if (_nextLevelButton == null)
                return;

            _nextLevelButton.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            _nextLevelButton.SetEnabled(isVisible);
        }
        
        private void EnsureRequiredElements()
        {
            if (_titleLabel == null ||
                _scoreLabel == null ||
                _starsContainer == null)
            {
                Debug.LogError("EndGameView: Required elements not found in EndGameView.uxml.");
            }
        }

        private static int NormalizeStarsCount(int activeStarsCount, int totalStarsCount)
        {
            if (totalStarsCount <= 0)
                return 0;

            var maxVisualStars = StarsSpriteByActiveCount.Length - 1;
            var normalized = Mathf.RoundToInt((float)activeStarsCount / totalStarsCount * maxVisualStars);
            return Mathf.Clamp(normalized, 0, maxVisualStars);
        }

        private void ApplyBackgroundSprite(VisualElement element, string spritePath)
        {
            if (element == null)
                return;

            var sprite = GetSprite(spritePath);
            element.style.backgroundImage = sprite != null
                ? new StyleBackground(sprite)
                : new StyleBackground();
        }

        private Sprite GetSprite(string resourcePath)
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
                return null;

            if (_spriteCache.TryGetValue(resourcePath, out var cachedSprite))
                return cachedSprite;

            var sprite = Resources.Load<Sprite>(resourcePath);
            _spriteCache[resourcePath] = sprite;
            return sprite;
        }

        private void HandleNextLevelButtonClicked()
        {
            NextLevelButtonClicked?.Invoke();
        }
        
        private void HandleRestartActionClicked()
        {
            RestartButtonClicked?.Invoke();
        }

        private void HandleMenuActionClicked()
        {
            MenuButtonClicked?.Invoke();
        }
    }
}

