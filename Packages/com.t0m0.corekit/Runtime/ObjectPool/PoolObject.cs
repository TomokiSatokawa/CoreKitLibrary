using UnityEngine;
using UnityEngine.Events;

namespace CoreKit.ObjectPool
{
    /// <summary>
    /// プールされるオブジェクトの有効化・返却と、時間経過による自動返却を担当する
    /// </summary>
    public sealed class PoolObject : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onActivate;
        [SerializeField] private UnityEvent _onRelease;
        [SerializeField] private bool _isAutoRelease;
        [SerializeField] private float _autoReleaseDuration;

        private float _autoReleaseTime;
        private bool _isActive = true;

        public bool IsActive => _isActive;

        private void Awake()
        {
            // 自動返却しないオブジェクトはUpdateを回さない
            enabled = false;
        }

        private void Update()
        {
            if (Time.time >= _autoReleaseTime)
                Release();
        }

        public void Activate()
        {
            _isActive = true;
            gameObject.SetActive(true);

            if (_isAutoRelease)
            {
                _autoReleaseTime = Time.time + _autoReleaseDuration;
                enabled = true;
            }

            _onActivate.Invoke();
        }

        public void Release()
        {
            if (!_isActive)
                return;

            _isActive = false;
            enabled = false;

            _onRelease.Invoke();
            gameObject.SetActive(false);
        }
    }
}