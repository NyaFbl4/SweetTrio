using Assets.Project.Scripts.Desserts;
using UnityEngine;

namespace Project.System
{
    public class ActionBar : MonoBehaviour, IActionBar
    {
        [SerializeField] private Transform _actionBarContainer;

        public void AddDessert(DessertController dessert)
        {
            
        }

        public void ClearField()
        {
            foreach (Transform child in _actionBarContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
