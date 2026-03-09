using UnityEngine;

namespace Project.System
{
    public class TransformController : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        public Transform SpawnPoint => _spawnPoint;
    }
}