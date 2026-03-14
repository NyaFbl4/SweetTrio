using UnityEngine;
using VContainer;
using Sirenix.OdinInspector;
using MessagePipe;
using Project.Scripts.Systems.UI.Dtos;
using Assets.Project.Scripts.System.DessertCreator;

namespace Project.Scripts.GameManager
{
    public class GameManagerHelper : MonoBehaviour
    {
        private IGameManagerService _gameManagerService;
        private IPublisher<ShowPopupDto> _showPopupDto;
        private IPublisher<HidePopupDto> _hidePopupDto;
        private IDessertSpawner _dessertSpawner;

        [Inject]
        public void Construct(
            IGameManagerService gameManagerService,
            IPublisher<ShowPopupDto> showPopupDto,
            IPublisher<HidePopupDto> hidePopupDto,
            IDessertSpawner dessertSpawner)
        {
            _gameManagerService = gameManagerService;
            _showPopupDto = showPopupDto;
            _hidePopupDto = hidePopupDto;
            _dessertSpawner = dessertSpawner;
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
    }
}
