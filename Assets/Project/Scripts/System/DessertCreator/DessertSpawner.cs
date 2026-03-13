using System.Collections.Generic;
using Assets.Project.Scripts.Desserts;
using Project.System;
using UnityEngine;

namespace Assets.Project.Scripts.System.DessertCreator
{
    public class DessertSpawner : IDessertSpawner
    {
        private readonly DessertPool _dessertPool;
        private readonly TransformController _transformController;
        private readonly Dictionary<int, Queue<DessertController>> _preparedDesserts = new();

        public DessertSpawner(DessertPool dessertPool, TransformController transformController)
        {
            _dessertPool = dessertPool;
            _transformController = transformController;
        }

        public void PreparePool(int copiesPerDessert)
        {
            if (_dessertPool == null || _dessertPool.DessertPrefabs == null)
            {
                Debug.LogError("DessertPool is not assigned.");
                return;
            }

            if (_transformController == null || _transformController.DessertdContainer == null)
            {
                Debug.LogError("DessertContainer is not assigned in TransformController.");
                return;
            }

            if (copiesPerDessert <= 0)
            {
                Debug.LogError("copiesPerDessert must be greater than 0.");
                return;
            }

            _preparedDesserts.Clear();

            for (var i = 0; i < _dessertPool.DessertPrefabs.Count; i++)
            {
                var prefab = _dessertPool.DessertPrefabs[i];
                if (prefab == null)
                {
                    Debug.LogWarning($"Dessert prefab at index {i} is null.");
                    continue;
                }

                var queue = new Queue<DessertController>(copiesPerDessert);
                for (var copy = 0; copy < copiesPerDessert; copy++)
                {
                    var instance = Object.Instantiate(prefab, _transformController.DessertdContainer, false);
                    instance.gameObject.SetActive(false);
                    queue.Enqueue(instance);
                }

                _preparedDesserts[i] = queue;
            }
        }

        public DessertController SpawnByIndex(int index)
        {
            if (_transformController == null || _transformController.SpawnPoint == null)
            {
                Debug.LogError("SpawnPoint is not assigned in TransformController.");
                return null;
            }

            if (!_preparedDesserts.TryGetValue(index, out var queue))
            {
                Debug.LogError($"Dessert index {index} is not prepared. Call PreparePool(n) first.");
                return null;
            }

            if (queue.Count == 0)
            {
                Debug.LogWarning($"No prepared desserts left for index {index}.");
                return null;
            }

            var dessert = queue.Dequeue();
            dessert.transform.SetParent(_transformController.SpawnPoint, false);
            dessert.transform.localPosition = Vector3.zero;
            dessert.transform.localRotation = Quaternion.identity;
            dessert.SetInteractable(true);
            dessert.gameObject.SetActive(true);

            return dessert;
        }
    }
}
