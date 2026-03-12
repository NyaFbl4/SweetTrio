using Assets.Project.Scripts.Desserts;
using UnityEngine;

namespace Project.System
{
    public class ActionBar : MonoBehaviour, IActionBar
    {
        [SerializeField] private Transform _actionBarContainer;
        [SerializeField] private int _maxCount = 7;

        public bool TryAddDessert(DessertController dessert)
         {
            if (dessert == null || _actionBarContainer == null)
                return false;

            if (_actionBarContainer.childCount >= _maxCount)
                return false;

            Transform slot = _actionBarContainer.childCount < _actionBarContainer.childCount
                ? _actionBarContainer.GetChild(_actionBarContainer.childCount)
                : _actionBarContainer;

            dessert.MoveToActionBar(slot);
            return true;
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
