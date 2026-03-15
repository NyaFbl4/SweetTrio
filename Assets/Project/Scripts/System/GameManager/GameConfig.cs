using UnityEngine;

namespace Project.Scripts.GameManager
{
    [CreateAssetMenu(menuName = "Configs/Game Config", fileName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float _spawnDessertScale = 2.0f;

        public float SpawnDessertScale => _spawnDessertScale;
    }
}
