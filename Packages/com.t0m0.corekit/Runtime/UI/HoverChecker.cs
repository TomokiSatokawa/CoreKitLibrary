//AI
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoreKit.UI
{
    /// <summary>
    /// UI上のポインター(マウス/タッチ)のEnter・Exitを検知し、
    /// 現在のホバー状態・イベント通知・当該フレームでのEnter/Exit判定を提供する
    /// </summary>
    public class HoverChecker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private int _enterFrame = -1;
        private int _exitFrame = -1;

        /// <summary>現在ポインターが乗っているか</summary>
        public bool IsHovering { get; private set; }

        /// <summary>このフレームでEnterしたか</summary>
        public bool IsEnteredThisFrame => _enterFrame == Time.frameCount;

        /// <summary>このフレームでExitしたか</summary>
        public bool IsExitedThisFrame => _exitFrame == Time.frameCount;

        public event Action OnEnter;
        public event Action OnExit;

        private void OnDisable()
        {
            // 非アクティブ化されるとExitが飛んでこないため、ここで状態を解消する
            if (IsHovering)
            {
                SetHover(false);
            }
        }

        private void SetHover(bool isHovering)
        {
            if (IsHovering == isHovering)
            {
                return;
            }

            IsHovering = isHovering;

            if (isHovering)
            {
                _enterFrame = Time.frameCount;
                OnEnter?.Invoke();
            }
            else
            {
                _exitFrame = Time.frameCount;
                OnExit?.Invoke();
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            SetHover(true);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            SetHover(false);
        }
    }
}