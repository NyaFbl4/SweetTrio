using System;
using System.Collections.Generic;
using Assets.Project.Scripts.Desserts;
using Assets.Project.Scripts.System.DessertCreator;
using MessagePipe;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.UseCases;
using Project.System;
using VContainer.Unity;

namespace Project.Scripts.GameManager
{
    public class GameRulesManager : IInitializable, ITickable, IDisposable, IGameStartListener, IGameFinishListener
    {
        private const int LoseDessertsCount = 7;
        private const int MatchCount = 3;
        private const int PointsPerDessert = 100;

        private readonly IActionBar _actionBar;
        private readonly IDessertSpawner _dessertSpawner;
        private readonly ILevelCounterUseCase _levelCounterUseCase;
        private readonly IPublisher<GameStatusCommandDto> _gameStatusPublisher;

        private bool _isGameFinished;
        private bool _isRoundActive;

        public GameRulesManager(
            IActionBar actionBar,
            IDessertSpawner dessertSpawner,
            ILevelCounterUseCase levelCounterUseCase,
            IPublisher<GameStatusCommandDto> gameStatusPublisher)
        {
            _actionBar = actionBar;
            _dessertSpawner = dessertSpawner;
            _levelCounterUseCase = levelCounterUseCase;
            _gameStatusPublisher = gameStatusPublisher;
        }

        public void Initialize()
        {
            _actionBar.DessertAdded += HandleDessertAdded;
            IGameListener.Register(this);
            _isGameFinished = false;
            _isRoundActive = false;
        }

        public void Dispose()
        {
            _actionBar.DessertAdded -= HandleDessertAdded;
            IGameListener.Unregister(this);
        }

        public void OnStartGame()
        {
            _isGameFinished = false;
            _isRoundActive = true;
            _levelCounterUseCase.Reset();
        }

        public void OnFinishGame()
        {
            _isGameFinished = true;
            _isRoundActive = false;
        }

        private void HandleDessertAdded(DessertController _)
        {
            if (_isGameFinished || !_isRoundActive)
                return;

            RemoveMatches();
            EvaluateGameState();
        }

        public void Tick()
        {
            if (_isGameFinished || !_isRoundActive)
                return;

            EvaluateGameState();
        }

        private void EvaluateGameState()
        {
            if (_actionBar.CurrentCount >= LoseDessertsCount)
            {
                _gameStatusPublisher.Publish(new GameStatusCommandDto
                {
                    Command = EGameStatusCommand.ShowLoseAndFinish
                });
                _isGameFinished = true;
                return;
            }

            if (_dessertSpawner.RemainingDessertsCount == 0 && _dessertSpawner.ActiveDessertsCount == 0)
            {
                _gameStatusPublisher.Publish(new GameStatusCommandDto
                {
                    Command = EGameStatusCommand.ShowWinAndFinish
                });
                _isGameFinished = true;
            }
        }

        private void RemoveMatches()
        {
            while (TryGetThreeOfKind(out var matchedDesserts))
            {
                _actionBar.RemoveDesserts(matchedDesserts);
                _levelCounterUseCase.Increment(matchedDesserts.Count * PointsPerDessert);
            }
        }

        private bool TryGetThreeOfKind(out List<DessertController> matchedDesserts)
        {
            matchedDesserts = null;
            var desserts = _actionBar.GetDesserts();

            var dessertsByType = new Dictionary<EDessertType, List<DessertController>>();
            for (var i = 0; i < desserts.Count; i++)
            {
                var dessert = desserts[i];
                if (dessert == null)
                    continue;

                var type = dessert.DessertType;
                if (!dessertsByType.TryGetValue(type, out var bucket))
                {
                    bucket = new List<DessertController>();
                    dessertsByType[type] = bucket;
                }

                bucket.Add(dessert);
                if (bucket.Count >= MatchCount)
                {
                    matchedDesserts = bucket.GetRange(0, MatchCount);
                    return true;
                }
            }

            return false;
        }
    }
}
