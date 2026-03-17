using System;
using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.GameStatus;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.UI.UseCases
{
    public class GameStatusUseCase : IInitializable, IDisposable
    {
        [Inject] private readonly ISubscriber<GameStatusCommandDto> _gameStatusSubscriber;
        [Inject] private readonly IGameStatusPresenter _gameStatusPresenter;
        [Inject] private readonly IGameManagerService _gameManagerService;

        private IDisposable _subscription = DisposableBag.Empty;

        public void Initialize()
        {
            _subscription = _gameStatusSubscriber.Subscribe(Handle);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        private void Handle(GameStatusCommandDto message)
        {
            if (message == null)
                return;

            switch (message.Command)
            {
                case EGameStatusCommand.ShowPaused:
                    _gameStatusPresenter.ShowPaused();
                    break;
                case EGameStatusCommand.ShowWinAndFinish:
                    _gameStatusPresenter.ShowWin();
                    _gameManagerService.FinishGame();
                    break;
                case EGameStatusCommand.ShowLoseAndFinish:
                    _gameStatusPresenter.ShowLose();
                    _gameManagerService.FinishGame();
                    break;
                case EGameStatusCommand.HideStatus:
                    _gameStatusPresenter.HideStatus();
                    break;
            }
        }
    }
}
