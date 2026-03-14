using System;
using Assets.Project.Scripts.System.DessertCreator;
using Project.System;
using VContainer.Unity;

namespace Project.Scripts.GameManager
{
    public class GameBootstrap : ITickable, IDisposable, IGameStartListener, IGamePauseListener, IGameResumeListener, IGameFinishListener
    {
        private readonly IDessertSpawner _dessertSpawner;
        private readonly IActionBar _actionBar;

        private bool _isAutoSpawnActive;
        private float _spawnTimer;
        private const float SpawnDelay = 0.5f;

        public GameBootstrap(IDessertSpawner dessertSpawner, IActionBar actionBar)
        {
            _dessertSpawner = dessertSpawner;
            _actionBar = actionBar;

            IGameListener.Register(this);
        }

        public void Tick()
        {
            if (!_isAutoSpawnActive)
                return;

            _spawnTimer += UnityEngine.Time.deltaTime;
            if (_spawnTimer < SpawnDelay)
                return;

            _spawnTimer = 0f;

            var spawned = _dessertSpawner.SpawnNext();
            if (spawned == null)
            {
                _isAutoSpawnActive = false;
            }
        }

        public void OnStartGame()
        {
            _actionBar.ClearField();
            _dessertSpawner.PrepareDeck();

            _spawnTimer = 0f;
            _isAutoSpawnActive = true;
        }

        public void OnPauseGame()
        {
            _isAutoSpawnActive = false;
        }

        public void OnResumeGame()
        {
            _isAutoSpawnActive = true;
        }

        public void OnFinishGame()
        {
            _isAutoSpawnActive = false;
            _spawnTimer = 0f;

            _dessertSpawner.ClearDeck();
            _actionBar.ClearField();
        }

        public void Dispose()
        {
            IGameListener.Unregister(this);
        }
    }
}
