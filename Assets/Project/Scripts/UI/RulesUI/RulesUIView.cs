using System;
using Project.Scripts.System.Localization;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Project.Scripts.UI.RulesUI
{
    public class RulesUIView : AnimatedPopupViewBase, IRulesUIView
    {
        private const string FallbackTitle = "ПРАВИЛА";
        private const string FallbackLevelTitle = "Правила уровня";
        private const string FallbackRulesTextTemplate =
            "Собирай одинаковые десерты в ряд по 3 и больше, чтобы получать очки.\n\n" +
            "Чем длиннее комбинация, тем больше награда.\n\n" +
            "Собирай десерты быстро и делай комбо подряд, чтобы получить бонусные очки.\n\n" +
            "Оставшееся в конце уровня время превращается в дополнительные очки.\n\n" +
            "Звезды за результат:\n" +
            "1 звезда — от {0} очков\n" +
            "2 звезды — от {1} очков\n" +
            "3 звезды — от {2} очков";

        [Inject] private readonly ILocalizationService _localizationService;

        private Label _titleLabel;
        private Label _mainTitleLabel;
        private Label _mainTextLabel;
        private Button _closeButton;

        protected override string OverlayElementName => "rules-overlay";
        protected override string PanelElementName => "rules-panel";

        public event Action CloseClicked;

        public override void Awake()
        {
            base.Awake();

            _titleLabel = _root.Q<Label>("rules-title-label");
            _mainTitleLabel = _root.Q<Label>("rules-main-title-label");
            _mainTextLabel = _root.Q<Label>("rules-main-text-label");
            _closeButton = _root.Q<Button>("rules-close-button");

            if (_titleLabel != null)
                _titleLabel.text = GetLocalizedText(LocalizationKeys.RulesTitle, FallbackTitle);

            if (_mainTitleLabel != null)
                _mainTitleLabel.text = GetLocalizedText(LocalizationKeys.RulesLevelTitle, FallbackLevelTitle);

            SetRulesScoreThresholds(1000, 2500, 5000);

            if (_closeButton == null)
            {
                Debug.LogError("RulesUIView: Button 'rules-close-button' not found in UXML.");
                return;
            }

            UIButtonAnimationUtility.EnableDefault(_closeButton);
            _closeButton.clicked += HandleCloseClicked;
        }

        private void OnDestroy()
        {
            if (_closeButton != null)
                _closeButton.clicked -= HandleCloseClicked;
        }

        public void SetRulesScoreThresholds(int oneStarScore, int twoStarsScore, int threeStarsScore)
        {
            if (_mainTextLabel == null)
                return;

            var oneStar = Mathf.Max(0, oneStarScore);
            var twoStars = Mathf.Max(oneStar, twoStarsScore);
            var threeStars = Mathf.Max(twoStars, threeStarsScore);

            var template = GetLocalizedText(LocalizationKeys.RulesTextTemplate, FallbackRulesTextTemplate);
            _mainTextLabel.text = string.Format(template, oneStar, twoStars, threeStars);
        }

        private string GetLocalizedText(string key, string fallback)
        {
            if (_localizationService == null)
                return fallback;

            var text = _localizationService.Get(key);
            return string.IsNullOrWhiteSpace(text) ? fallback : text;
        }

        private void HandleCloseClicked()
        {
            CloseClicked?.Invoke();
        }
    }
}
