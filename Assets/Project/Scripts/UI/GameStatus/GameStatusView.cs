using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.GameStatus
{
    public class GameStatusView : LayoutViewBase, IGameStatusView
    {
        private const string StatusLabelName = "game-status-label";
        private Label _messageLabel;

        public override void Awake()
        {
            base.Awake();
            _messageLabel = _root.Q<Label>(StatusLabelName);

            if (_messageLabel == null)
            {
                Debug.LogError($"GameStatusView: Label '{StatusLabelName}' not found in UXML.");
            }
        }

        public void SetMessage(string message)
        {
            if (_messageLabel == null)
                return;

            _messageLabel.text = message;
        }
    }
}
