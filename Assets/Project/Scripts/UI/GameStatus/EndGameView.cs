using System;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.EndGame
{
    public class EndGameView : LayoutViewBase, IEndGameView
    {
        private const string TitleLabelName = "end-game-title-label";
        private const string ScoreLabelName = "end-game-score-label";
        private const string ExitToMenuButtonName = "end-game-menu-button";

        private Label _titleLabel;
        private Label _scoreLabel;
        private Button _exitToMenuButton;

        public event Action ExitToMenuClicked;

        public override void Awake()
        {
            base.Awake();
            _titleLabel = _root.Q<Label>(TitleLabelName);
            _scoreLabel = _root.Q<Label>(ScoreLabelName);
            _exitToMenuButton = _root.Q<Button>(ExitToMenuButtonName);

            if (_titleLabel == null)
            {
                Debug.LogError($"EndGameView: Label '{TitleLabelName}' not found in UXML.");
            }

            if (_scoreLabel == null)
            {
                Debug.LogError($"EndGameView: Label '{ScoreLabelName}' not found in UXML.");
            }

            if (_exitToMenuButton == null)
            {
                Debug.LogError($"EndGameView: Button '{ExitToMenuButtonName}' not found in UXML.");
            }
            else
            {
                _exitToMenuButton.clicked += HandleExitToMenuClicked;
            }
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

        private void HandleExitToMenuClicked()
        {
            ExitToMenuClicked?.Invoke();
        }
    }
}
