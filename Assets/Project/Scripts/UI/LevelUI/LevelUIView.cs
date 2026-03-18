using System;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.LevelUI
{
    public class LevelUIView : LayoutViewBase, ILevelUIView
    {
        private const string CounterLabelName = "level-counter-label";
        private const string TotalDessertsLabelName = "total-desserts-label";
        private const string CountdownLabelName = "timer-countdown-label";
        private const string FillName = "timer-progress-fill";
        private const string ShuffleButtonName = "shuffle-button";

        private Label _counterLabel;
        private Label _totalDessertsLabel;
        private Label _countdownLabel;
        private VisualElement _fillElement;
        private Button _shuffleButton;

        public event Action ShuffleButtonClicked;

        public override void Awake()
        {
            base.Awake();
            _counterLabel = _root.Q<Label>(CounterLabelName);
            _totalDessertsLabel = _root.Q<Label>(TotalDessertsLabelName);
            _countdownLabel = _root.Q<Label>(CountdownLabelName);
            _fillElement = _root.Q<VisualElement>(FillName);
            _shuffleButton = _root.Q<Button>(ShuffleButtonName);

            if (_counterLabel == null)
            {
                Debug.LogError($"LevelUIView: Label '{CounterLabelName}' not found in UXML.");
            }

            if (_totalDessertsLabel == null)
            {
                Debug.LogError($"LevelUIView: Label '{TotalDessertsLabelName}' not found in UXML.");
            }

            if (_countdownLabel == null)
            {
                Debug.LogError($"LevelUIView: Label '{CountdownLabelName}' not found in UXML.");
            }

            if (_fillElement == null)
            {
                Debug.LogError($"LevelUIView: VisualElement '{FillName}' not found in UXML.");
            }

            if (_shuffleButton == null)
            {
                Debug.LogError($"LevelUIView: Button '{ShuffleButtonName}' not found in UXML.");
            }
            else
            {
                _shuffleButton.clicked += OnShuffleButtonClicked;
            }
        }

        private void OnDestroy()
        {
            if (_shuffleButton != null)
            {
                _shuffleButton.clicked -= OnShuffleButtonClicked;
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

        private void OnShuffleButtonClicked()
        {
            ShuffleButtonClicked?.Invoke();
        }
    }
}

