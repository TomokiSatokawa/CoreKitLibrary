using System.Collections.Generic;
using CoreKit.DynamicEnum;
using UnityEngine;
using UnityEngine.Audio;

namespace CoreKit.Sound
{
    /// <summary>
    /// サウンドデータを管理するデータベース
    /// </summary>
    [CreateAssetMenu(fileName = "SoundDatabase", menuName = "CoreKit/Sound/SoundDatabase")]
    public class SoundDatabase : ScriptableObject
    {
        [SerializeReference,SubclassSelector] private ClipData[] _clipDatas;

        private Dictionary<DynamicEnumValue, ClipData> _clipDataDictionary;

        [System.Serializable]
        public abstract class ClipData
        {
            [SerializeField] protected DynamicEnumValue _type;
            [SerializeField, Range(0f, 1f)] protected float _volume = 1f;

            public DynamicEnumValue Type => _type;
            public float Volume => _volume;
            public abstract void GetClip(out AudioClip clip, out float volume);
        }

        [System.Serializable]
        public class SingleClipData:ClipData
        {
            [SerializeField] private AudioClip _clip;
            public AudioClip Clip => _clip;

            public override void GetClip(out AudioClip clip, out float volume)
            {
                clip = _clip;
                volume = _volume;
            }
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
        }

        public ClipData GetClipData(DynamicEnumValue type)
        {
            if (_clipDataDictionary.TryGetValue(type, out var clipData))
                return clipData;

            Debug.LogError($"指定されたSoundTypeが登録されていません。 Type: {type}", this);
            return null;
        }
    }
}