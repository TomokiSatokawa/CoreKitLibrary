using UnityEngine;

/// <summary>
/// 音量の制御を行う構造体
/// </summary>
[System.Serializable]
public struct VolumeController
{
    [SerializeField, Range(0f, 1f)]
    private float _volume;

    private float _startVolume;
    private float _targetVolume;
    private float _elapsedTime;
    private float _duration;

    public float Volume => _volume;

    public bool IsChanging => _elapsedTime < _duration;

    /// <summary>
    /// 音量変更を開始する
    /// </summary>
    public void Change(float targetVolume, float duration)
    {
        _startVolume = _volume;
        _targetVolume = Mathf.Clamp01(targetVolume);
        _duration = Mathf.Max(0f, duration);
        _elapsedTime = 0f;

        if (_duration <= 0f)
            _volume = _targetVolume;
    }

    /// <summary>
    /// 音量を更新する
    /// </summary>
    public void Update(float deltaTime)
    {
        if (!IsChanging)
            return;

        _elapsedTime += deltaTime;

        float progress = Mathf.Clamp01(_elapsedTime / _duration);
        _volume = Mathf.Lerp(_startVolume, _targetVolume, progress);
    }

    /// <summary>
    /// 音量を即座に設定する
    /// </summary>
    public void Set(float volume)
    {
        _volume = Mathf.Clamp01(volume);
        _startVolume = _volume;
        _targetVolume = _volume;
        _elapsedTime = 0f;
        _duration = 0f;
    }
}