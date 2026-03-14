using Assets.Project.Scripts.Desserts;
using UnityEngine;

namespace Project.Scripts.GameManager
{
    [CreateAssetMenu(menuName = "Configs/Level Config", fileName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private DessertPool _dessertPool;
        [SerializeField, Min(1)] private int _copiesPerDessert = 3;
        [SerializeField, Min(0.05f)] private float _spawnDelaySeconds = 0.5f;
        [SerializeField, Min(0.1f)] private float _spawnDessertScale = 2.0f;

        public DessertPool DessertPool => _dessertPool;
        public int CopiesPerDessert => _copiesPerDessert;
        public float SpawnDelaySeconds => _spawnDelaySeconds;
        public float SpawnDessertScale => _spawnDessertScale;
    }
}
