using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using VContainer;

namespace Project.Scripts.UI.LevelUI
{
    public class LevelUIPresenter : LayoutPresenterBase<ILevelUIView>, ILevelUIPresenter, IGameStartListener, IGameFinishListener
    {
        [Inject] private readonly IPublisher<ShowPopupDto> _showPopUpPublisher;
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;

        public override void Initialize()
        {
            base.Initialize();
            IGameListener.Register(this);
        }

        public override void Dispose()
        {
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
                TargetPopUpType = typeof(ILevelUIPresenter)
            });
        }
    }
}
