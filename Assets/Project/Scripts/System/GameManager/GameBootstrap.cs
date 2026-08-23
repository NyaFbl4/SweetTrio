using System;
using System.Collections.Generic;
using Assets.Project.Scripts.System.DessertCreator;
using MessagePipe;
using Project.Scripts.System.Audio;
using Project.System;
using VContainer.Unity;

namespace Project.Scripts.GameManager
{
    public class GameBootstrap : ITickable, IDisposable, IGameStartListener, IGameFinishListener, IGameBootstrapControl
    {
        private readonly IDessertSpawner _dessertSpawner;
        private readonly IActionBar _actionBar;
        private readonly ILevelSelectionService _levelSelectionService;
        private readonly GameConfig _gameConfig;
        private readonly ISoundManager _soundManager;
        private readonly IDisposable _clearActionBarSubscription;
        
        private bool _isAutoSpawnActive;
        private bool _isInitialSpawnInProgress;
        private float _spawnTimer;
        private readonly Queue<int> _spawnRequestQueue = new();
        private readonly IDisposable _shuffleFieldSubscription;

        public GameBootstrap(
            IDessertSpawner dessertSpawner,
            IActionBar actionBar,
            ILevelSelectionService levelSelectionService,
            GameConfig gameConfig,
            ISoundManager soundManager,
            ISubscriber<ShuffleFieldCommandDto> shuffleFieldSubscriber,
            ISubscriber<ClearActionBarCommandDto> clearActionBarSubscriber)
        {
            _dessertSpawner = dessertSpawner;
            _actionBar = actionBar;
            _levelSelectionService = levelSelectionService;
            _gameConfig = gameConfig;
            _soundManager = soundManager;

            IGameListener.Register(this);
            _actionBar.DessertAdded += HandleDessertAdded;
            _shuffleFieldSubscription = shuffleFieldSubscriber.Subscribe(HandleShuffleRequested);
            _clearActionBarSubscription = clearActionBarSubscriber.Subscribe(HandleClearActionBarRequested);
        }

        public void Tick()
        {
            if (!_isAutoSpawnActive)
                return;

            var levelConfig = _levelSelectionService.CurrentLevel;
            if (levelConfig == null)
                return;

            _spawnTimer += UnityEngine.Time.deltaTime;
            if (_spawnTimer < levelConfig.SpawnDelaySeconds)
                return;

            _spawnTimer = 0f;

            if (_isInitialSpawnInProgress)
            {
                if (_dessertSpawner.RemainingDessertsCount <= 0)
                {
                    _isInitialSpawnInProgress = false;
                    return;
                }

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
                    else
                    {
                        _soundManager.PlayDessertSpawn();
                    }
                }

                return;
            }

            if (_spawnRequestQueue.Count == 0)
            {
                // Recovery path: if field has room but queue is empty (e.g. objects left field
                // without click), restart initial fill to avoid permanent spawn stall.
                if (_dessertSpawner.RemainingDessertsCount > 0 &&
                    _dessertSpawner.FieldDessertsCount < _gameConfig.MaxDessertsOnField)
                {
                    _isInitialSpawnInProgress = true;
                }

                return;
            }

            if (_dessertSpawner.RemainingDessertsCount <= 0)
            {
                _spawnRequestQueue.Clear();
                _isAutoSpawnActive = false;
                return;
            }

            if (_dessertSpawner.FieldDessertsCount >= _gameConfig.MaxDessertsOnField)
                return;

            _spawnRequestQueue.Dequeue();
            var queuedSpawned = _dessertSpawner.SpawnNext();
            if (queuedSpawned == null)
            {
                _isInitialSpawnInProgress = _dessertSpawner.RemainingDessertsCount > 0;
            }
            else
            {
                _soundManager.PlayDessertSpawn();
            }
        }

        public void OnStartGame()
        {
            if (_levelSelectionService.CurrentLevel == null)
            {
                _isAutoSpawnActive = false;
                _isInitialSpawnInProgress = false;
                _spawnRequestQueue.Clear();
                _spawnTimer = 0f;
                return;
            }
            
            _actionBar.ClearField();
            _dessertSpawner.PrepareDeck();
            _spawnRequestQueue.Clear();

            _spawnTimer = 0f;
            _isAutoSpawnActive = _dessertSpawner.RemainingDessertsCount > 0;
            _isInitialSpawnInProgress = _isAutoSpawnActive;
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
            if (_dessertSpawner.RemainingDessertsCount <= 0)
            {
                _isAutoSpawnActive = false;
                _isInitialSpawnInProgress = false;
                return;
            }

            _isAutoSpawnActive = true;
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
        
        private void HandleClearActionBarRequested(ClearActionBarCommandDto _)
        {
            if (_actionBar.TryReturnDessertsToPool())
            {
                RestartInitialSpawn();
            }
        }

        public void Dispose()
        {
            _shuffleFieldSubscription.Dispose();
            _actionBar.DessertAdded -= HandleDessertAdded;
            _clearActionBarSubscription.Dispose();
            IGameListener.Unregister(this);
        }
    }
}
