using UnityEngine;
using VContainer;
using Sirenix.OdinInspector;
using MessagePipe;
using Project.Scripts.Systems.UI.Dtos;
using Project.System;
using Assets.Project.Scripts.System.DessertCreator;
using Assets.Project.Scripts.Desserts;

namespace Project.Scripts.GameManager
{
    public class GameManagerHelper : MonoBehaviour
    {
        private IGameManagerService _gameManagerService;
        private IPublisher<ShowPopupDto> _showPopupDto;
        private IPublisher<HidePopupDto> _hidePopupDto;
        private TransformController _transformController;
        private IDessertCreator _dessertCreator;
        private DessertPool _dessertsPool;

        [Inject]
        public void Construct(
            IGameManagerService gameManagerService,
            IPublisher<ShowPopupDto> showPopupDto,
            IPublisher<HidePopupDto> hidePopupDto,
            TransformController transformController,
            IDessertCreator dessertCreator,
            DessertPool dessertsPool)
        {
            _gameManagerService = gameManagerService;
            _showPopupDto = showPopupDto;
            _hidePopupDto = hidePopupDto;
            _transformController = transformController;
            _dessertCreator = dessertCreator;
            _dessertsPool = dessertsPool;
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
        public void Spawn(int n)
        {
            _dessertCreator.SpawnDessert(_transformController.SpawnPoint, _dessertsPool.DessertPrefabs[n]);
        }
    }
}
