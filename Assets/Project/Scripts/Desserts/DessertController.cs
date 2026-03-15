using UnityEngine;

namespace Assets.Project.Scripts.Desserts
{
    public class DessertController : MonoBehaviour
    {
        [SerializeField] private EDessertType _dessertType;
        private bool _isInteractable = true;
        private bool _isInActionBar;

        public EDessertType DessertType => _dessertType;
        public bool IsInActionBar => _isInActionBar;

        public void MoveToActionBar(Transform newPosition)
        {
            _isInteractable = false;
            _isInActionBar = true;

            transform.SetParent(newPosition, worldPositionStays: false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * 0.85f;
            Destroy(GetComponent<Rigidbody2D>());
        }

        public void SetInteractable(bool isInteractable)
        {
            _isInteractable = isInteractable;
        }

        private void OnMouseDown()
        {
            Debug.Log("Figure clicked: " + _dessertType);
            if (!_isInteractable)
                return;

            // Clicked?.Invoke(this);
        }
    }
}
