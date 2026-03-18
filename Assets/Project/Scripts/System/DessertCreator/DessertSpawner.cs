using System.Collections.Generic;
using Assets.Project.Scripts.Desserts;
using Assets.Project.Scripts.System.DessertCreator.Dtos;
using MessagePipe;
using Project.Scripts.GameManager;
using Project.System;
using UnityEngine;

namespace Assets.Project.Scripts.System.DessertCreator
{
    public class DessertSpawner : IDessertSpawner
    {
        private readonly LevelConfig _levelConfig;
        private readonly GameConfig _gameConfig;
        private readonly TransformController _transformController;
        private readonly IPublisher<DessertCountsDto> _dessertCountsPublisher;
        private readonly Queue<DessertController> _preparedDessertsQueue = new();
        private readonly List<DessertController> _preparedDesserts = new();
        private readonly List<DessertController> _fieldDesserts = new();

        public int TotalDessertsCount
        {
            get
            {
                var count = 0;
                for (var i = 0; i < _preparedDesserts.Count; i++)
                {
                    if (_preparedDesserts[i] != null)
                    {
                        count++;
                    }
                }

                return count;
            }
        }
        public int RemainingDessertsCount => _preparedDessertsQueue.Count;
        public int FieldDessertsCount
        {
            get
            {
                CleanupFieldDesserts();
                return _fieldDesserts.Count;
            }
        }
        public int ActiveDessertsCount
        {
            get
            {
                var count = 0;
                for (var i = 0; i < _preparedDesserts.Count; i++)
                {
                    if (_preparedDesserts[i] != null)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public DessertSpawner(
            LevelConfig levelConfig,
            GameConfig gameConfig,
            TransformController transformController,
            IPublisher<DessertCountsDto> dessertCountsPublisher)
        {
            _levelConfig = levelConfig;
            _gameConfig = gameConfig;
            _transformController = transformController;
            _dessertCountsPublisher = dessertCountsPublisher;
        }

        public void PrepareDeck()
        {
            if (_levelConfig == null)
            {
                Debug.LogError("LevelConfig is not assigned.");
                return;
            }

            if (_levelConfig.DessertPool == null || _levelConfig.DessertPool.DessertPrefabs == null)
            {
                Debug.LogError("DessertPool is not assigned.");
                return;
            }

            if (_transformController == null || _transformController.DessertdContainer == null)
            {
                Debug.LogError("DessertContainer is not assigned in TransformController.");
                return;
            }

            if (_levelConfig.CopiesPerDessert <= 0)
            {
                Debug.LogError("copiesPerDessert must be greater than 0.");
                return;
            }

            ClearPreparedDesserts(notify: false);
            _fieldDesserts.Clear();

            for (var i = 0; i < _levelConfig.DessertPool.DessertPrefabs.Count; i++)
            {
                var prefab = _levelConfig.DessertPool.DessertPrefabs[i];
                if (prefab == null)
                {
                    Debug.LogWarning($"Dessert prefab at index {i} is null.");
                    continue;
                }

                for (var copy = 0; copy < _levelConfig.CopiesPerDessert; copy++)
                {
                    var instance = UnityEngine.Object.Instantiate(prefab, _transformController.DessertdContainer, false);
                    instance.gameObject.SetActive(false);
                    _preparedDesserts.Add(instance);
                }
            }

            ShufflePreparedDesserts();

            for (var i = 0; i < _preparedDesserts.Count; i++)
            {
                _preparedDessertsQueue.Enqueue(_preparedDesserts[i]);
            }

            NotifyCountsChanged();
        }

        public DessertController SpawnNext()
        {
            if (_transformController == null || _transformController.SpawnPoint == null)
            {
                Debug.LogError("SpawnPoint is not assigned in TransformController.");
                return null;
            }

            if (_gameConfig == null)
            {
                Debug.LogError("GameConfig is not assigned.");
                return null;
            }

            if (_preparedDessertsQueue.Count == 0)
            {
                Debug.LogWarning("Prepared deck is empty. Call PrepareDeck() first.");
                return null;
            }

            CleanupFieldDesserts();
            if (_fieldDesserts.Count >= _gameConfig.MaxDessertsOnField)
            {
                return null;
            }

            var dessert = _preparedDessertsQueue.Dequeue();
            dessert.transform.SetParent(_transformController.SpawnPoint, false);
            dessert.transform.localPosition = Vector3.zero;
            dessert.transform.localRotation = Quaternion.identity;
            dessert.transform.localScale = Vector3.one * _gameConfig.SpawnDessertScale;
            dessert.gameObject.SetActive(true);
            dessert.PrepareForField();
            _fieldDesserts.Add(dessert);
            NotifyCountsChanged();

            return dessert;
        }

        public void ClearDeck()
        {
            ClearPreparedDesserts();
        }

        public void RespawnFieldWithShuffle()
        {
            if (_transformController == null || _transformController.DessertdContainer == null || _transformController.SpawnPoint == null)
            {
                Debug.LogError("TransformController references are not assigned.");
                return;
            }

            if (_gameConfig == null)
            {
                Debug.LogError("GameConfig is not assigned.");
                return;
            }

            CleanupFieldDesserts();

            var dessertsToShuffle = new List<DessertController>(_preparedDessertsQueue.Count + _fieldDesserts.Count);
            while (_preparedDessertsQueue.Count > 0)
            {
                var queuedDessert = _preparedDessertsQueue.Dequeue();
                if (queuedDessert == null)
                    continue;

                queuedDessert.gameObject.SetActive(false);
                queuedDessert.transform.SetParent(_transformController.DessertdContainer, false);
                dessertsToShuffle.Add(queuedDessert);
            }

            for (var i = 0; i < _fieldDesserts.Count; i++)
            {
                var fieldDessert = _fieldDesserts[i];
                if (fieldDessert == null)
                    continue;

                fieldDessert.SetInteractable(false);
                fieldDessert.gameObject.SetActive(false);
                fieldDessert.transform.SetParent(_transformController.DessertdContainer, false);
                dessertsToShuffle.Add(fieldDessert);
            }

            _fieldDesserts.Clear();
            ShuffleDesserts(dessertsToShuffle);

            for (var i = 0; i < dessertsToShuffle.Count; i++)
            {
                _preparedDessertsQueue.Enqueue(dessertsToShuffle[i]);
            }

            NotifyCountsChanged();
        }

        public void ReturnDessertsToPool(IReadOnlyList<DessertController> desserts)
        {
            if (desserts == null || desserts.Count == 0)
                return;

            if (_transformController == null || _transformController.DessertdContainer == null)
            {
                Debug.LogError("DessertContainer is not assigned in TransformController.");
                return;
            }

            var hasChanges = false;
            for (var i = 0; i < desserts.Count; i++)
            {
                var dessert = desserts[i];
                if (dessert == null || !dessert.IsInActionBar)
                    continue;

                dessert.ReturnToPool(_transformController.DessertdContainer);
                _preparedDessertsQueue.Enqueue(dessert);
                hasChanges = true;
            }

            if (hasChanges)
            {
                NotifyCountsChanged();
            }
        }

        private void ClearPreparedDesserts(bool notify = true)
        {
            while (_preparedDessertsQueue.Count > 0)
            {
                _preparedDessertsQueue.Dequeue();
            }

            for (var i = 0; i < _preparedDesserts.Count; i++)
            {
                if (_preparedDesserts[i] != null)
                {
                    UnityEngine.Object.Destroy(_preparedDesserts[i].gameObject);
                }
            }

            _preparedDesserts.Clear();
            _fieldDesserts.Clear();

            if (notify)
            {
                NotifyCountsChanged();
            }
        }

        private void CleanupFieldDesserts()
        {
            for (var i = _fieldDesserts.Count - 1; i >= 0; i--)
            {
                var dessert = _fieldDesserts[i];
                if (dessert == null || !dessert.gameObject.activeInHierarchy || dessert.IsInActionBar)
                {
                    _fieldDesserts.RemoveAt(i);
                }
            }
        }

        private void ShufflePreparedDesserts()
        {
            ShuffleDesserts(_preparedDesserts);
        }

        private static void ShuffleDesserts(IList<DessertController> desserts)
        {
            for (var i = desserts.Count - 1; i > 0; i--)
            {
                var j = UnityEngine.Random.Range(0, i + 1);
                (desserts[i], desserts[j]) = (desserts[j], desserts[i]);
            }
        }

        private void NotifyCountsChanged()
        {
            _dessertCountsPublisher.Publish(new DessertCountsDto(
                TotalDessertsCount,
                RemainingDessertsCount,
                ActiveDessertsCount,
                FieldDessertsCount));
        }
    }
}
