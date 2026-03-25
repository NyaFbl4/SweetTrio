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

        public override void Initialize()
        {
            base.Initialize();
            IGameListener.Register(this);
            _layoutView.ShuffleButtonClicked += HandleShuffleButtonClicked;
            _layoutView.ExitToMenuClicked += HandleExitToMenuClicked;
            _layoutView.PauseButtonClicked += HandlePauseButtonClicked;
        }

        public override void Dispose()
        {
            _layoutView.ShuffleButtonClicked -= HandleShuffleButtonClicked;
            _layoutView.ExitToMenuClicked -= HandleExitToMenuClicked;
            _layoutView.PauseButtonClicked -= HandlePauseButtonClicked;
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

        private void HandleShuffleButtonClicked()
        {
            _shuffleFieldPublisher.Publish(new ShuffleFieldCommandDto());
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
