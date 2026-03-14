using System.Collections.Generic;
using Assets.Project.Scripts.Desserts;
using Project.Scripts.GameManager;
using Project.System;
using UnityEngine;

namespace Assets.Project.Scripts.System.DessertCreator
{
    public class DessertSpawner : IDessertSpawner
    {
        private readonly LevelConfig _levelConfig;
        private readonly TransformController _transformController;
        private readonly Queue<DessertController> _preparedDessertsQueue = new();
        private readonly List<DessertController> _preparedDesserts = new();

        public DessertSpawner(LevelConfig levelConfig, TransformController transformController)
        {
            _levelConfig = levelConfig;
            _transformController = transformController;
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

            ClearPreparedDesserts();

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
                    var instance = Object.Instantiate(prefab, _transformController.DessertdContainer, false);
                    instance.gameObject.SetActive(false);
                    _preparedDesserts.Add(instance);
                }
            }

            ShufflePreparedDesserts();

            for (var i = 0; i < _preparedDesserts.Count; i++)
            {
                _preparedDessertsQueue.Enqueue(_preparedDesserts[i]);
            }
        }

        public DessertController SpawnNext()
        {
            if (_transformController == null || _transformController.SpawnPoint == null)
            {
                Debug.LogError("SpawnPoint is not assigned in TransformController.");
                return null;
            }

            if (_preparedDessertsQueue.Count == 0)
            {
                Debug.LogWarning("Prepared deck is empty. Call PrepareDeck() first.");
                return null;
            }

            var dessert = _preparedDessertsQueue.Dequeue();
            dessert.transform.SetParent(_transformController.SpawnPoint, false);
            dessert.transform.localPosition = Vector3.zero;
            dessert.transform.localRotation = Quaternion.identity;
            dessert.SetInteractable(true);
            dessert.gameObject.SetActive(true);

            return dessert;
        }

        public void ClearDeck()
        {
            ClearPreparedDesserts();
        }

        private void ClearPreparedDesserts()
        {
            while (_preparedDessertsQueue.Count > 0)
            {
                _preparedDessertsQueue.Dequeue();
            }

            for (var i = 0; i < _preparedDesserts.Count; i++)
            {
                if (_preparedDesserts[i] != null)
                {
                    Object.Destroy(_preparedDesserts[i].gameObject);
                }
            }

            _preparedDesserts.Clear();
        }

        private void ShufflePreparedDesserts()
        {
            for (var i = _preparedDesserts.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (_preparedDesserts[i], _preparedDesserts[j]) = (_preparedDesserts[j], _preparedDesserts[i]);
            }
        }
    }
}
