using UnityEngine;

namespace Assets.Project.Scripts.Desserts
{
    public class DessertController : MonoBehaviour
    {
        [SerializeField] private EDessertType _dessertType;
        private bool _isInteractable = true;
        private bool _isInActionBar;
        private Rigidbody2D _rigidbody2D;
        private Quaternion _fieldLocalRotation;
        private bool _hasFieldLocalRotation;

        public EDessertType DessertType => _dessertType;
        public bool IsInActionBar => _isInActionBar;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            CacheFieldLocalRotation();
        }

        public void MoveToActionBar(Transform newPosition)
        {
            _isInteractable = false;
            _isInActionBar = true;

            transform.SetParent(newPosition, worldPositionStays: false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * 0.85f;

            SetRigidBodySimulated(false);
        }

        public void PrepareForField()
        {
            CacheFieldLocalRotation();
            _isInteractable = true;
            _isInActionBar = false;
            transform.localRotation = _fieldLocalRotation;
            SetRigidBodySimulated(true);
        }

        public void ReturnToPool(Transform newParent)
        {
            _isInteractable = false;
            _isInActionBar = false;

            transform.SetParent(newParent, worldPositionStays: false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            SetRigidBodySimulated(false);
            gameObject.SetActive(false);
        }

        public void SetInteractable(bool isInteractable)
        {
            _isInteractable = isInteractable;
        }
        
        public void BeginMoveToActionBar()
        {
            _isInteractable = false;
            _isInActionBar = true;
            SetRigidBodySimulated(false);
        }

        private void OnMouseDown()
        {
            // Debug.Log("Figure clicked: " + _dessertType);
            if (!_isInteractable)
                return;

            // Clicked?.Invoke(this);
        }

        private void SetRigidBodySimulated(bool isSimulated)
        {
            if (_rigidbody2D == null)
            {
                _rigidbody2D = GetComponent<Rigidbody2D>();
            }

            if (_rigidbody2D == null)
                return;

            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;
            _rigidbody2D.simulated = isSimulated;
        }

        private void CacheFieldLocalRotation()
        {
            if (_hasFieldLocalRotation)
                return;

            _fieldLocalRotation = transform.localRotation;
            _hasFieldLocalRotation = true;
        }
    }
}
