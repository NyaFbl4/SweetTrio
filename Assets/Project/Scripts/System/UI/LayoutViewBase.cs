using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.Systems.UI
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class LayoutViewBase : MonoBehaviour, ILayoutView
    {
        protected UIDocument _uiDocument;
        protected VisualElement _root;

        public virtual void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;

            Hide();
        }

        public bool Visible
        {
            get { return _root.style.display == DisplayStyle.Flex; }
            set { _root.style.display = value ? DisplayStyle.Flex : DisplayStyle.None; }
        }

        public virtual async UniTask ShowAsync()
        {
            Show();
            await UniTask.Yield();
        }

        public virtual async UniTask HideAsync()
        {
            Hide();
            await UniTask.Yield();
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
        }
    }
}
