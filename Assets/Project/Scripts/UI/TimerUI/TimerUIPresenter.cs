using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using VContainer;

namespace Project.Scripts.UI.TimerUI
{
    public class TimerUIPresenter : LayoutPresenterBase<ITimerUIView>, ITimerUIPresenter, IGameStartListener, IGameFinishListener
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

        public void SetTimerText(string text)
        {
            _layoutView.SetTimerText(text);
        }

        public void SetProgress(float value01)
        {
            _layoutView.SetProgress(value01);
        }

        public void OnStartGame()
        {
            _showPopUpPublisher.Publish(new ShowPopupDto
            {
                TargetPopUpType = typeof(ITimerUIPresenter)
            });
        }

        public void OnFinishGame()
        {
            _hidePopUpPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(ITimerUIPresenter)
            });
        }
    }
}
