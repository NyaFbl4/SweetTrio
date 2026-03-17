using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.TimerUI
{
    public class TimerUIView : LayoutViewBase, ITimerUIView
    {
        private const string CountdownLabelName = "timer-countdown-label";
        private const string FillName = "timer-progress-fill";

        private Label _countdownLabel;
        private VisualElement _fillElement;

        public override void Awake()
        {
            base.Awake();
            _countdownLabel = _root.Q<Label>(CountdownLabelName);
            _fillElement = _root.Q<VisualElement>(FillName);

            if (_countdownLabel == null)
            {
                Debug.LogError($"TimerUIView: Label '{CountdownLabelName}' not found in UXML.");
            }

            if (_fillElement == null)
            {
                Debug.LogError($"TimerUIView: VisualElement '{FillName}' not found in UXML.");
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
