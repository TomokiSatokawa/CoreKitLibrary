using UnityEngine;
using UnityEngine.Events;

namespace Common.UI
{
    /// <summary>
    /// UIWindowの表示/非表示
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class PanelControl : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private UnityEvent _activeAction = new();
        [SerializeField] private UnityEvent _hiddenAction = new();

        public bool IsActive { get; private set; }
        public RectTransform Rect => _rectTransform;

        private void Awake()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();

            SetVisible(false);
        }

        public virtual void OnActive(float duration = 0)
        {
            _activeAction?.Invoke();

            SetVisible(true);
        }
        public virtual void OnHidden(float duration = 0)
        {
            _hiddenAction?.Invoke();

            SetVisible(false);
        }

        /// <summary>
        /// UIの表示・非表示を切り替える
        /// </summary>
        private void SetVisible(bool visible)
        {
            float alpha = visible ? 1f : 0f;

            _canvasGroup.alpha = alpha;

            _canvasGroup.blocksRaycasts = visible;
            IsActive = visible;
        }
    }
}