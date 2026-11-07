using UnityEngine;

namespace Common.UI
{
    /// <summary>
    /// ScrollViewのContents生成クラスのベースクラス
    /// </summary>
    public abstract class ScrollViewBase : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;

        private void Awake()
        {
            if (_content == null)
            {
                Debug.LogError($"[ScrollViewBase] Contentが設定されていません。 GameObject:{name}", this);
                enabled = false;
            }
        }

        /// <summary>
        /// 子オブジェクトをすべて削除する
        /// </summary>
        protected void DeleteChildren()
        {
            for (int i = _content.childCount - 1; i >= 0; i--)
            {
                Destroy(_content.GetChild(i).gameObject);
            }

            OnDeletedChildren();
        }

        protected virtual void OnDeletedChildren() { }

        /// <summary>
        /// contentの子オブジェクトに生成
        /// </summary>
        protected T InstantiateContent<T>(T prefab) where T : MonoBehaviour
        {
            return Instantiate(prefab, _content);
        }
    }
}
