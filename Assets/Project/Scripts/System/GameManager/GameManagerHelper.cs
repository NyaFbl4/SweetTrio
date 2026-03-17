using UnityEngine;
using VContainer;
using Sirenix.OdinInspector;
using MessagePipe;
using Assets.Project.Scripts.System.DessertCreator;
using Project.Scripts.Systems.UI.Dtos;

namespace Project.Scripts.GameManager
{
    public class GameManagerHelper : MonoBehaviour
    {
        private IGameManagerService _gameManagerService;
        private IPublisher<GameStatusCommandDto> _gameStatusPublisher;
        private IDessertSpawner _dessertSpawner;
        private IGameBootstrapControl _gameBootstrapControl;

        [Inject]
        public void Construct(
            IGameManagerService gameManagerService,
            IPublisher<GameStatusCommandDto> gameStatusPublisher,
            IDessertSpawner dessertSpawner,
            IGameBootstrapControl gameBootstrapControl)
        {
            _gameManagerService = gameManagerService;
            _gameStatusPublisher = gameStatusPublisher;
            _dessertSpawner = dessertSpawner;
            _gameBootstrapControl = gameBootstrapControl;
        }

        [Button]
        public void StartGame()
        {
            if (_gameManagerService == null)
            {
                Debug.LogError("GameManagerService is null. Ensure GameManager is registered in GameLifetimeScope.");
                return;
            }

            _gameManagerService.StartGame();
        }

        [Button]
        public void FinishGame()
        {
            if (_gameManagerService == null)
            {
                Debug.LogError("GameManagerService is null. Ensure GameManager is registered in GameLifetimeScope.");
                return;
            }

            _gameManagerService.FinishGame();
        }

        [Button]
        public void PauseGame()
        {
            if (_gameManagerService == null)
            {
                Debug.LogError("GameManagerService is null. Ensure GameManager is registered in GameLifetimeScope.");
                return;
            }

            _gameManagerService.PauseGame();
            _gameStatusPublisher.Publish(new GameStatusCommandDto
            {
                Command = EGameStatusCommand.ShowPaused
            });
        }

        [Button]
        public void ResumeGame()
        {
            if (_gameManagerService == null)
            {
                Debug.LogError("GameManagerService is null. Ensure GameManager is registered in GameLifetimeScope.");
                return;
            }

            _gameManagerService.ResumeGame();
            _gameStatusPublisher.Publish(new GameStatusCommandDto
            {
                Command = EGameStatusCommand.HideStatus
            });
        }

        [Button]
        public void SpawnDessert()
        {
            if (_dessertSpawner == null)
            {
                Debug.LogError("DessertSpawner is null. Ensure it is registered in GameLifetimeScope.");
                return;
            }

            _dessertSpawner.SpawnNext();
        }

        [Button]
        public void PrepareDesserts()
        {
            if (_dessertSpawner == null)
            {
                Debug.LogError("DessertSpawner is null. Ensure it is registered in GameLifetimeScope.");
                return;
            }

            _dessertSpawner.PrepareDeck();
        }

        [Button]
        public void RespawnFieldWithShuffle()
        {
            if (_dessertSpawner == null)
            {
                Debug.LogError("DessertSpawner is null. Ensure it is registered in GameLifetimeScope.");
                return;
            }

            if (_gameBootstrapControl == null)
            {
                Debug.LogError("GameBootstrapControl is null. Ensure GameBootstrap is registered in GameLifetimeScope.");
                return;
            }

            _dessertSpawner.RespawnFieldWithShuffle();
            _gameBootstrapControl.RestartInitialSpawn();
        }
    }
}
