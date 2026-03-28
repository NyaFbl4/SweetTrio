using System;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.RulesUI
{
    public class RulesUIView : AnimatedPopupViewBase, IRulesUIView
    {
        private const string LocalizedRulesTitle = "\u041F\u0420\u0410\u0412\u0418\u041B\u0410";

        private const string RulesTextTemplate =
            "\u0421\u043E\u0431\u0438\u0440\u0430\u0439 \u043E\u0434\u0438\u043D\u0430\u043A\u043E\u0432\u044B\u0435 \u0434\u0435\u0441\u0435\u0440\u0442\u044B \u0432 \u0440\u044F\u0434 \u043F\u043E 3 \u0438 \u0431\u043E\u043B\u044C\u0448\u0435, \u0447\u0442\u043E\u0431\u044B \u043F\u043E\u043B\u0443\u0447\u0430\u0442\u044C \u043E\u0447\u043A\u0438.\n\n" +
            "\u0427\u0435\u043C \u0434\u043B\u0438\u043D\u043D\u0435\u0435 \u043A\u043E\u043C\u0431\u0438\u043D\u0430\u0446\u0438\u044F, \u0442\u0435\u043C \u0431\u043E\u043B\u044C\u0448\u0435 \u043D\u0430\u0433\u0440\u0430\u0434\u0430.\n\n" +
            "\u0421\u043E\u0431\u0438\u0440\u0430\u0439 \u0434\u0435\u0441\u0435\u0440\u0442\u044B \u0431\u044B\u0441\u0442\u0440\u043E \u0438 \u0434\u0435\u043B\u0430\u0439 \u043A\u043E\u043C\u0431\u043E \u043F\u043E\u0434\u0440\u044F\u0434, \u0447\u0442\u043E\u0431\u044B \u043F\u043E\u043B\u0443\u0447\u0438\u0442\u044C \u0431\u043E\u043D\u0443\u0441\u043D\u044B\u0435 \u043E\u0447\u043A\u0438.\n\n" +
            "\u041E\u0441\u0442\u0430\u0432\u0448\u0435\u0435\u0441\u044F \u0432 \u043A\u043E\u043D\u0446\u0435 \u0443\u0440\u043E\u0432\u043D\u044F \u0432\u0440\u0435\u043C\u044F \u043F\u0440\u0435\u0432\u0440\u0430\u0449\u0430\u0435\u0442\u0441\u044F \u0432 \u0434\u043E\u043F\u043E\u043B\u043D\u0438\u0442\u0435\u043B\u044C\u043D\u044B\u0435 \u043E\u0447\u043A\u0438.\n\n" +
            "\u0417\u0432\u0435\u0437\u0434\u044B \u0437\u0430 \u0440\u0435\u0437\u0443\u043B\u044C\u0442\u0430\u0442:\n" +
            "1 \u0437\u0432\u0435\u0437\u0434\u0430 \u2014 \u043E\u0442 {0} \u043E\u0447\u043A\u043E\u0432\n" +
            "2 \u0437\u0432\u0435\u0437\u0434\u044B \u2014 \u043E\u0442 {1} \u043E\u0447\u043A\u043E\u0432\n" +
            "3 \u0437\u0432\u0435\u0437\u0434\u044B \u2014 \u043E\u0442 {2} \u043E\u0447\u043A\u043E\u0432";

        private Label _titleLabel;
        private Label _mainTextLabel;
        private Button _closeButton;

        protected override string OverlayElementName => "rules-overlay";
        protected override string PanelElementName => "rules-panel";

        public event Action CloseClicked;

        public override void Awake()
        {
            base.Awake();

            _titleLabel = _root.Q<Label>("rules-title-label");
            _mainTextLabel = _root.Q<Label>("rules-main-text-label");
            _closeButton = _root.Q<Button>("rules-close-button");

            if (_titleLabel != null)
                _titleLabel.text = LocalizedRulesTitle;

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

            _mainTextLabel.text = string.Format(
                RulesTextTemplate,
                oneStar,
                twoStars,
                threeStars);
        }

        private void HandleCloseClicked()
        {
            CloseClicked?.Invoke();
        }
    }
}
