using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.LevelUI
{
    public class LevelUIView : LayoutViewBase, ILevelUIView
    {
        private const string CounterLabelName = "level-counter-label";
        private const string TotalDessertsLabelName = "total-desserts-label";
        private Label _counterLabel;
        private Label _totalDessertsLabel;

        public override void Awake()
        {
            base.Awake();
            _counterLabel = _root.Q<Label>(CounterLabelName);
            _totalDessertsLabel = _root.Q<Label>(TotalDessertsLabelName);

            if (_counterLabel == null)
            {
                Debug.LogError($"LevelUIView: Label '{CounterLabelName}' not found in UXML.");
            }

            if (_totalDessertsLabel == null)
            {
                Debug.LogError($"LevelUIView: Label '{TotalDessertsLabelName}' not found in UXML.");
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
    }
}
