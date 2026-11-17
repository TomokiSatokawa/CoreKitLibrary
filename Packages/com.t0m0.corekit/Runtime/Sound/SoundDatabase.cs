using System.Collections.Generic;
using CoreKit.DynamicEnum;
using UnityEngine;
using UnityEngine.Audio;

namespace CoreKit
{
    /// <summary>
    /// サウンドデータを管理するデータベース
    /// </summary>
    [CreateAssetMenu(fileName = "SoundDatabase", menuName = "CoreKit/Sound/SoundDatabase")]
    public class SoundDatabase : ScriptableObject
    {
        [SerializeField] private ClipData[] _clipDatas;
        [SerializeField] private RandomClipData[] _randomClipData;

        private Dictionary<DynamicEnumValue, ClipData> _clipDataDictionary;
        private Dictionary<DynamicEnumValue, RandomClipData> _randomCipDataDictionary;

        [System.Serializable]
        public class ClipData
        {
            [SerializeField] private DynamicEnumValue _type;
            [SerializeField] private AudioClip _clip;
            [SerializeField, Range(0f, 1f)] private float _volume;

            public DynamicEnumValue Type => _type;
            public AudioClip Clip => _clip;
            public float Volume => _volume;
        }

        [System.Serializable]
        public class RandomClipData
        {
            [SerializeField] private DynamicEnumValue _type;
            [SerializeField] private AudioResource _audioResource;
            [SerializeField, Range(0f, 1f)] private float _volume;

            public DynamicEnumValue Type => _type;
            public AudioResource AudioResource => _audioResource;
            public float Volume => _volume;
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void Initialize()
        {
            _clipDataDictionary = new Dictionary<DynamicEnumValue, ClipData>(_clipDatas.Length);

            foreach (var clipData in _clipDatas)
            {
                if (!_clipDataDictionary.TryAdd(clipData.Type, clipData))
                {
                    Debug.LogError($"SoundTypeが重複しています: {clipData.Type}", this);
                }
            }

            _randomCipDataDictionary = new Dictionary<DynamicEnumValue, RandomClipData>(_clipDatas.Length);

            foreach (var clipData in _randomClipData)
            {
                if (!_randomCipDataDictionary.TryAdd(clipData.Type, clipData))
                {
                    Debug.LogError($"SoundTypeが重複しています: {clipData.Type}", this);
                }
            }
        }

        public ClipData GetClipData(DynamicEnumValue type)
        {
            if (_clipDataDictionary.TryGetValue(type, out var clipData))
                return clipData;

            Debug.LogError($"指定されたSoundTypeが登録されていません。 Type: {type}", this);
            return null;
        }

        public RandomClipData GetRandomClipData(DynamicEnumValue type)
        {
            if (_randomCipDataDictionary.TryGetValue(type, out var clipData))
                return clipData;

            Debug.LogError($"指定されたSoundTypeが登録されていません。 Type: {type}", this);
            return null;
        }
    }
}