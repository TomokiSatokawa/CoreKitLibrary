using System;
using UnityEngine;

namespace CoreKit.Demo
{
    /// <summary>
    /// 現在値を目標値まで、指定した変化速度とカーブで滑らかに変化させる汎用クラス。
    /// 目標値が途中で変わった場合は、その時点の値から新しい目標値へ再スタートする。
    /// </summary>
    public class SmoothValue
    {
        private float _startValue;
        private float _targetValue;
        private float _speed;
        private float _duration;
        private float _elapsed;
        private AnimationCurve _curve;

        public float Current { get; private set; }
        public float Target => _targetValue;
        public bool IsMoving => _elapsed < _duration;

        public event Action<float> OnValueChanged;

        public SmoothValue(float initialValue, float speed, AnimationCurve curve)
        {
            Current = initialValue;
            _startValue = initialValue;
            _targetValue = initialValue;
            _speed = speed;
            _curve = curve ?? AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }

        /// <summary>
        /// 目標値を設定する。速度とカーブはコンストラクタで指定した値を使う。
        /// </summary>
        public void SetTarget(float target)
        {
            SetTarget(target, _speed, _curve);
        }

        /// <summary>
        /// 目標値・変化速度(単位/秒)・カーブを指定して設定する。
        /// カーブは横軸0〜1(進行度)、縦軸0〜1(変化率)を想定。
        /// </summary>
        public void SetTarget(float target, float speed, AnimationCurve curve)
        {
            if (speed <= 0f)
            {
                Debug.LogError($"[SmoothValue] 変化速度は0より大きい値を指定してください。speed={speed}");
                Snap(target);
                return;
            }

            _speed = speed;
            _curve = curve ?? AnimationCurve.Linear(0f, 0f, 1f, 1f);
            _startValue = Current;
            _targetValue = target;
            _duration = Mathf.Abs(_targetValue - _startValue) / _speed;
            _elapsed = 0f;
        }

        /// <summary>
        /// 毎フレーム呼び出して値を更新する。
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (!IsMoving)
            {
                return;
            }

            _elapsed = Mathf.Min(_elapsed + deltaTime, _duration);

            var progress = _duration > 0f ? _elapsed / _duration : 1f;
            var rate = _curve.Evaluate(progress);
            SetCurrent(Mathf.LerpUnclamped(_startValue, _targetValue, rate));
        }

        /// <summary>
        /// 補間せず、即座に値を設定する。
        /// </summary>
        public void Snap(float value)
        {
            _startValue = value;
            _targetValue = value;
            _duration = 0f;
            _elapsed = 0f;
            SetCurrent(value);
        }

        private void SetCurrent(float value)
        {
            if (Mathf.Approximately(Current, value))
            {
                return;
            }

            Current = value;
            OnValueChanged?.Invoke(Current);
        }
    }
}