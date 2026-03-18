using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.GameManager
{
    public class GameManagerService : IGameManagerService, IStartable, ITickable, IFixedTickable, IDisposable
    {
        private EGameState _gameState;
        private readonly List<IGameListener> _gameListeners = new();
        private readonly List<IGameUpdateListener> _gameUpdateListeners = new();
        private readonly List<IGameFixedUpdateListener> _gameFixedUpdateListeners = new();

        [Inject]
        private GameManagerService()
        {
            _gameState = EGameState.Off;

            IGameListener.onRegister += AddListener;
            IGameListener.onUnregister += RemoveListener;
        }

        public void Start()
        {
            _gameState = EGameState.Play;
        }

        public void Dispose()
        {
            _gameState = EGameState.Finish;

            IGameListener.onRegister -= AddListener;
            IGameListener.onUnregister -= RemoveListener;

            _gameListeners.Clear();
            _gameUpdateListeners.Clear();
            _gameFixedUpdateListeners.Clear();
        }

        public void Tick()
        {
            if (_gameState != EGameState.Play)
                return;

            var deltaTime = Time.deltaTime;
            foreach (var listener in _gameUpdateListeners)
            {
                listener.OnUpdate(deltaTime);
            }
        }

        public void FixedTick()
        {
            if (_gameState != EGameState.Play)
                return;

            var deltaTime = Time.deltaTime;
            foreach (var listener in _gameFixedUpdateListeners)
            {
                listener.OnFixedUpdate(deltaTime);
            }
        }

        private void AddListener(IGameListener gameListener)
        {
            _gameListeners.Add(gameListener);

            if (gameListener is IGameUpdateListener gameUpdateListener)
                _gameUpdateListeners.Add(gameUpdateListener);

            if (gameListener is IGameFixedUpdateListener gameFixedUpdateListener)
                _gameFixedUpdateListeners.Add(gameFixedUpdateListener);
        }

        private void RemoveListener(IGameListener gameListener)
        {
            _gameListeners.Remove(gameListener);

            if (gameListener is IGameUpdateListener gameUpdateListener)
                _gameUpdateListeners.Remove(gameUpdateListener);

            if (gameListener is IGameFixedUpdateListener gameFixedUpdateListener)
                _gameFixedUpdateListeners.Remove(gameFixedUpdateListener);
        }

        public void StartGame()
        {
            foreach (var gameListener in _gameListeners)
            {
                if (gameListener is IGameStartListener gameStartListener)
                {
                    gameStartListener.OnStartGame();
                }
            }

            _gameState = EGameState.Play;
            Time.timeScale = 1f;
        }

        public void FinishGame()
        {
            foreach (var gameListener in _gameListeners)
            {
                if (gameListener is IGameFinishListener gameFinishListener)
                {
                    gameFinishListener.OnFinishGame();
                }
            }

            Time.timeScale = 0f;
            _gameState = EGameState.Finish;
        }

    }
}
