using UnityEngine;

namespace Project.System
{
    public class TransformController : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _dessertsContainer;
        public Transform SpawnPoint => _spawnPoint;
        public Transform DessertdContainer => _dessertsContainer;
    }
}