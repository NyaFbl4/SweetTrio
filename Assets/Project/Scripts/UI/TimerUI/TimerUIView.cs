using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.TimerUI
{
    public class TimerUIView : LayoutViewBase, ITimerUIView
    {
        private Label _countdownLabel;
        private VisualElement _fillElement;

        public override void Awake()
        {
            base.Awake();
            _countdownLabel = _root.Q<Label>("timer-countdown-label");
            _fillElement = _root.Q<VisualElement>("timer-progress-fill");

            if (_countdownLabel == null)
            {
                Debug.LogError("TimerUIView: Label 'timer-countdown-label' not found in UXML.");
            }

            if (_fillElement == null)
            {
                Debug.LogError("TimerUIView: VisualElement 'timer-progress-fill' not found in UXML.");
            }
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
    }
}
