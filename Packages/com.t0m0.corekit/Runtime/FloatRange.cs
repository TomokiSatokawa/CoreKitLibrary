using UnityEngine;

namespace CoreKit
{
    /// <summary>
    /// 範囲値
    /// </summary>
    [System.Serializable]
    public struct FloatRange
    {
        [SerializeField] private float _min;
        [SerializeField] private float _max;

        public float Min => _min;
        public float Max => _max;

        /// <summary>
        /// 値が範囲内か判定する（最小値を含み、最大値を含まない）
        /// </summary>
        public bool IsRange(float value)
        {
            return value >= _min && value < _max;
        }

        /// <summary>
        /// 値を範囲内に収める（両端を含む）
        /// </summary>
        public float Clamp(float value)
        {
            return Mathf.Clamp(value, _min, _max);
        }

        /// <summary>
        /// 範囲内のランダムな値を取得する
        /// </summary>
        public float GetRandom()
        {
            return Random.Range(_min, _max);
        }
    }

}
