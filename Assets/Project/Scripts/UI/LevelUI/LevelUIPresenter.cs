using System;
using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.PauseUI;
using Project.Scripts.UI.MainScreen;
using UnityEngine;
using VContainer;

namespace Project.Scripts.UI.LevelUI
{
    public class LevelUIPresenter : LayoutPresenterBase<ILevelUIView>, ILevelUIPresenter, IGameStartListener, IGameFinishListener
    {
        [Inject] private readonly IGameManagerService _gameManagerService;
        [Inject] private readonly IPublisher<ShuffleFieldCommandDto> _shuffleFieldPublisher;
        [Inject] private readonly IPublisher<ShowPopupDto> _showPopUpPublisher;
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;
        [Inject] private readonly IPublisher<ClearActionBarCommandDto> _clearActionBarPublisher;
        [Inject] private readonly IRewardedAdService _rewardedAdService;
        [Inject] private readonly ISubscriber<ActionBarStateDto> _actionBarStateSubscriber;
        
        private IDisposable _actionBarStateSubscription;
        private int _actionBarCurrentCount;
        private int _freeBoosterPressesPerLevel = 1;
        private string _shuffleRewardId = "shuffle_field";
        private string _clearActionBarRewardId = "clear_action_bar";

        private int _shuffleFreePresses;
        private int _clearActionBarFreePresses;
        private bool _isWaitingReward;

        public override void Initialize()
        {
            base.Initialize();
            IGameListener.Register(this);
            _layoutView.ShuffleButtonClicked += HandleShuffleButtonClicked;
            _layoutView.ExitToMenuClicked += HandleExitToMenuClicked;
            _layoutView.PauseButtonClicked += HandlePauseButtonClicked;
            _layoutView.ClearActionBarButtonClicked += HandleClearActionBarButtonClicked;
            _actionBarStateSubscription = _actionBarStateSubscriber.Subscribe(HandleActionBarStateChanged);
        }

        public override void Dispose()
        {
            _layoutView.ShuffleButtonClicked -= HandleShuffleButtonClicked;
            _layoutView.ExitToMenuClicked -= HandleExitToMenuClicked;
            _layoutView.PauseButtonClicked -= HandlePauseButtonClicked;
            _layoutView.ClearActionBarButtonClicked -= HandleClearActionBarButtonClicked;
            _actionBarStateSubscription.Dispose();
            IGameListener.Unregister(this);
            base.Dispose();
        }

        public void SetCounter(int value)
        {
            _layoutView.SetCounter(value);
        }

        public void SetCounterText(string text)
        {
            _layoutView.SetCounterText(text);
        }

        public void SetTotalDessertsText(string text)
        {
            _layoutView.SetTotalDessertsText(text);
        }

        public void SetTimerText(string text)
        {
            _layoutView.SetTimerText(text);
        }

        public void SetProgress(float value01)
        {
            _layoutView.SetProgress(value01);
        }

        public void SetBonusDessertSprite(Sprite sprite)
        {
            _layoutView.SetBonusDessertSprite(sprite);
        }

        public void SetBonusMultiplierText(string text)
        {
            _layoutView.SetBonusMultiplierText(text);
        }

        public void OnStartGame()
        {
            _shuffleFreePresses = _freeBoosterPressesPerLevel;
            _clearActionBarFreePresses = _freeBoosterPressesPerLevel;

            _layoutView.SetShufflePressCount(_shuffleFreePresses);
            _layoutView.SetClearActionBarPressCount(_clearActionBarFreePresses);
            _layoutView.SetBoosterButtonsEnabled(true);
            
            _showPopUpPublisher.Publish(new ShowPopupDto
            {
                TargetPopUpType = typeof(ILevelUIPresenter)
            });
        }

        public void OnFinishGame()
        {
            _hidePopUpPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(IPauseUIPresenter)
            });

            _hidePopUpPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(ILevelUIPresenter)
            });
        }
        
        private void HandleActionBarStateChanged(ActionBarStateDto dto)
        {
            _actionBarCurrentCount = dto.CurrentCount;
        }

        private void HandleShuffleButtonClicked()
        {
            TryUseBooster(
                ref _shuffleFreePresses,
                _layoutView.SetShufflePressCount,
                _shuffleRewardId,
                () => _shuffleFieldPublisher.Publish(new ShuffleFieldCommandDto()));
        }

        private void HandleClearActionBarButtonClicked()
        {
            if (_actionBarCurrentCount  <= 0)
                return;

            TryUseBooster(
                ref _clearActionBarFreePresses,
                _layoutView.SetClearActionBarPressCount,
                _clearActionBarRewardId,
                () => _clearActionBarPublisher.Publish(new ClearActionBarCommandDto()));
        }
        
        private void TryUseBooster(
            ref int freePresses,
            Action<int> updatePressCount,
            string rewardId,
            Action applyAction)
        {
            if (_isWaitingReward)
                return;

            if (freePresses > 0)
            {
                freePresses--;
                updatePressCount(freePresses);
                applyAction();
                return;
            }

            _isWaitingReward = true;
            _layoutView.SetBoosterButtonsEnabled(false);

            _rewardedAdService.Show(
                rewardId,
                () =>
                {
                    _isWaitingReward = false;
                    _layoutView.SetBoosterButtonsEnabled(true);
                    applyAction();
                },
                () =>
                {
                    _isWaitingReward = false;
                    _layoutView.SetBoosterButtonsEnabled(true);
                });
        }

        private void HandleExitToMenuClicked()
        {
            _gameManagerService.FinishGame();

            _showPopUpPublisher.Publish(new ShowPopupDto
            {
                TargetPopUpType = typeof(IMainMenuPresenter)
            });
        }

        private void HandlePauseButtonClicked()
        {
            _showPopUpPublisher.Publish(new ShowPopupDto
            {
                TargetPopUpType = typeof(IPauseUIPresenter)
            });
        }
    }
}
