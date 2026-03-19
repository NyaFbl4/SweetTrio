using System;
using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.EndGame;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.UI.UseCases
{
    public class GameStatusUseCase : IInitializable, IDisposable
    {
        [Inject] private readonly ISubscriber<GameStatusCommandDto> _gameStatusSubscriber;
        [Inject] private readonly IEndGamePresenter _endGamePresenter;
        [Inject] private readonly IGameManagerService _gameManagerService;
        [Inject] private readonly ILevelCounterUseCase _levelCounterUseCase;
        [Inject] private readonly ITimerPointsUseCase _timerPointsUseCase;

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
                case EGameStatusCommand.ShowWinAndFinish:
                    _timerPointsUseCase.ApplyWinBonus();
                    _endGamePresenter.ShowWin(_levelCounterUseCase.CurrentValue);
                    _gameManagerService.FinishGame();
                    break;
                case EGameStatusCommand.ShowLoseAndFinish:
                    _endGamePresenter.ShowLose();
                    _gameManagerService.FinishGame();
                    break;
            }
        }
    }
}

