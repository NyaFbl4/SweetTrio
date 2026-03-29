using Cysharp.Threading.Tasks;
using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.System.Ads;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using UnityEngine;
using VContainer;

namespace Project.Scripts.UI.RulesUI
{
    public class RulesUIController : LayoutPresenterBase<IRulesUIView>, IRulesUIController, IGameStartListener, IGameFinishListener
    {
        [Inject] private readonly IGameManagerService _gameManagerService;
        [Inject] private readonly ILevelSelectionService _levelSelectionService;
        [Inject] private readonly ILevelStartAdService _levelStartAdService;
        [Inject] private readonly IPublisher<ShowPopupDto> _showPopupPublisher;
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopupPublisher;

        private bool _isGameActive;
        private bool _startGameOnClose;
        private bool _startGameRequestedAfterHide;

        public override void Initialize()
        {
            base.Initialize();
            IGameListener.Register(this);
            _layoutView.CloseClicked += HandleCloseClicked;
        }

        public override void Dispose()
        {
            _layoutView.CloseClicked -= HandleCloseClicked;
            IGameListener.Unregister(this);
            base.Dispose();
        }

        public override async UniTask ActivateAsync()
        {
            Time.timeScale = 0f;
            await base.ActivateAsync();
        }

        public override async UniTask DeactivateAsync()
        {
            await base.DeactivateAsync();

            if (_startGameRequestedAfterHide)
            {
                _startGameRequestedAfterHide = false;
                _startGameOnClose = false;
                _gameManagerService.StartGame();
                return;
            }

            if (_isGameActive)
                Time.timeScale = 1f;
        }

        public void ShowBeforeLevelStart()
        {
            ApplyCurrentLevelRulesThresholds();
            _levelStartAdService?.ShowLevelStartAd();
            _startGameOnClose = true;
            _startGameRequestedAfterHide = false;
            ShowRulesPopup();
        }

        public void OnStartGame()
        {
            _isGameActive = true;
        }

        public void OnFinishGame()
        {
            _isGameActive = false;
            _startGameOnClose = false;
            _startGameRequestedAfterHide = false;
            HideRulesPopup();
        }

        private void HandleCloseClicked()
        {
            if (_startGameOnClose)
            {
                _startGameRequestedAfterHide = true;
            }

            HideRulesPopup();
        }

        private void ApplyCurrentLevelRulesThresholds()
        {
            var levelConfig = _levelSelectionService?.CurrentLevel;
            if (levelConfig == null)
            {
                _layoutView.SetRulesScoreThresholds(1000, 2500, 5000);
                return;
            }

            _layoutView.SetRulesScoreThresholds(
                levelConfig.OneStarScore,
                levelConfig.TwoStarsScore,
                levelConfig.ThreeStarsScore);
        }

        private void ShowRulesPopup()
        {
            _showPopupPublisher.Publish(new ShowPopupDto
            {
                TargetPopUpType = typeof(IRulesUIController)
            });
        }

        private void HideRulesPopup()
        {
            _hidePopupPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(IRulesUIController)
            });
        }
    }
}
