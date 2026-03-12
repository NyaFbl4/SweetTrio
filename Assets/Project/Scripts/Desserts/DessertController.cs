using UnityEngine;

namespace Assets.Project.Scripts.Desserts
{
    public class DessertController : MonoBehaviour
    {
        [SerializeField] private EDessertType _dessertType;
        private bool _isInteractable = true;

        public EDessertType DessertType => _dessertType;


        public void MoveToActionBar(Transform newPosition)
        {
            _isInteractable = false;

            transform.SetParent(newPosition, worldPositionStays: false);
            transform.localPosition = Vector3.zero;
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
