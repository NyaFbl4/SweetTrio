using Assets.Project.Scripts.Desserts;
using UnityEngine;

namespace Project.Scripts.GameManager
{
    [CreateAssetMenu(menuName = "Configs/Level Config", fileName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private DessertPool _dessertPool;
        [SerializeField, Min(1)] private int _copiesPerDessert = 3;

        public DessertPool DessertPool => _dessertPool;
        public int CopiesPerDessert => _copiesPerDessert;
    }
}
