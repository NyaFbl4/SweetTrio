using System.Collections.Generic;
using Assets.Project.Scripts.Desserts;
using UnityEngine;

namespace Assets.Project.Scripts.System.DessertCreator
{
    public class DessertCreator : IDessertCreator
    {
        // private readonly DessertPool _dessertsPool;
        private readonly List<DessertController> _spawnedDesserts = new();

        public DessertCreator(DessertPool dessertsPool)
        {
            // _dessertsPool = dessertsPool;
        }

        public void SpawnDessert(Transform parent, DessertController gamePbject)
        {
            // _spawnedDesserts.Clear();

            // if (parent == null || _dessertsPool == null || _dessertsPool.DessertPrefabs == null)
            //     // return _spawnedDesserts;

            // foreach (var dessertPrefab in _dessertsPool.DessertPrefabs)
            // {
            //     if (dessertPrefab == null)
            //         continue;

            //     var dessertInstance = Object.Instantiate(dessertPrefab, parent, false);
            //     _spawnedDesserts.Add(dessertInstance);
            // }

            var dessertInstance = Object.Instantiate(gamePbject, parent, false);

            // return _spawnedDesserts;
        }
    }
}
