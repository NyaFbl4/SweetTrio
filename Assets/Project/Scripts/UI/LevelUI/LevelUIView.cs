using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.LevelUI
{
    public class LevelUIView : LayoutViewBase, ILevelUIView
    {
        private const string CounterLabelName = "level-counter-label";
        private Label _counterLabel;

        public override void Awake()
        {
            base.Awake();
            _counterLabel = _root.Q<Label>(CounterLabelName);

            if (_counterLabel == null)
            {
                Debug.LogError($"LevelUIView: Label '{CounterLabelName}' not found in UXML.");
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
    }
}
