using System;
using System.Collections.Generic;
using Assets.Project.Scripts.System.DessertCreator;
using MessagePipe;
using Project.System;
using VContainer.Unity;

namespace Project.Scripts.GameManager
{
    public class GameBootstrap : ITickable, IDisposable, IGameStartListener, IGameFinishListener, IGameBootstrapControl
    {
        private readonly IDessertSpawner _dessertSpawner;
        private readonly IActionBar _actionBar;
        private readonly LevelConfig _levelConfig;
        private readonly GameConfig _gameConfig;

        private bool _isAutoSpawnActive;
        private bool _isInitialSpawnInProgress;
        private float _spawnTimer;
        private readonly Queue<int> _spawnRequestQueue = new();
        private readonly IDisposable _shuffleFieldSubscription;

        public GameBootstrap(
            IDessertSpawner dessertSpawner,
            IActionBar actionBar,
            LevelConfig levelConfig,
            GameConfig gameConfig,
            ISubscriber<ShuffleFieldCommandDto> shuffleFieldSubscriber)
        {
            _dessertSpawner = dessertSpawner;
            _actionBar = actionBar;
            _levelConfig = levelConfig;
            _gameConfig = gameConfig;

            IGameListener.Register(this);
            _actionBar.DessertAdded += HandleDessertAdded;
            _shuffleFieldSubscription = shuffleFieldSubscriber.Subscribe(HandleShuffleRequested);
        }

        public void Tick()
        {
            if (!_isAutoSpawnActive)
                return;

            _spawnTimer += UnityEngine.Time.deltaTime;
            if (_spawnTimer < _levelConfig.SpawnDelaySeconds)
                return;

            _spawnTimer = 0f;

            if (_isInitialSpawnInProgress)
            {
                if (_dessertSpawner.FieldDessertsCount >= _gameConfig.MaxDessertsOnField)
                {
                    _isInitialSpawnInProgress = false;
                }
                else
                {
                    var initialSpawned = _dessertSpawner.SpawnNext();
                    if (initialSpawned == null)
                    {
                        _isInitialSpawnInProgress = false;
                    }
                }

                return;
            }

            if (_spawnRequestQueue.Count == 0)
                return;

            if (_dessertSpawner.FieldDessertsCount >= _gameConfig.MaxDessertsOnField)
                return;

            _spawnRequestQueue.Dequeue();
            var queuedSpawned = _dessertSpawner.SpawnNext();
            if (queuedSpawned == null)
            {
                _isAutoSpawnActive = false;
            }
        }

        public void OnStartGame()
        {
            _actionBar.ClearField();
            _dessertSpawner.PrepareDeck();
            _spawnRequestQueue.Clear();

            _spawnTimer = 0f;
            _isAutoSpawnActive = true;
            _isInitialSpawnInProgress = true;
        }

        public void OnFinishGame()
        {
            _isAutoSpawnActive = false;
            _isInitialSpawnInProgress = false;
            _spawnTimer = 0f;
            _spawnRequestQueue.Clear();

            _dessertSpawner.ClearDeck();
            _actionBar.ClearField();
        }

        public void RestartInitialSpawn()
        {
            if (!_isAutoSpawnActive)
                return;

            _spawnTimer = 0f;
            _spawnRequestQueue.Clear();
            _isInitialSpawnInProgress = true;
        }

        private void HandleDessertAdded(Assets.Project.Scripts.Desserts.DessertController _)
        {
            if (!_isAutoSpawnActive)
                return;

            // Queue spawn requests and process them in Tick.
            _spawnRequestQueue.Enqueue(1);
        }

        private void HandleShuffleRequested(ShuffleFieldCommandDto _)
        {
            if (!_isAutoSpawnActive)
                return;

            _dessertSpawner.RespawnFieldWithShuffle();
            RestartInitialSpawn();
        }

        public void Dispose()
        {
            _shuffleFieldSubscription.Dispose();
            _actionBar.DessertAdded -= HandleDessertAdded;
            IGameListener.Unregister(this);
        }
    }
}
