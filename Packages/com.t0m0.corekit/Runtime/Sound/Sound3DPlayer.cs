using CoreKit.DynamicEnum;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace CoreKit.Sound
{
    /// <summary>
    /// 3D空間上の位置でSEを再生する責務を持つ。AudioSourceはプールして再利用する。
    /// シーン上に1つ配置し、staticメソッド経由で再生する。
    /// </summary>
    public class Sound3DPlayer : MonoBehaviour
    {
        private static Sound3DPlayer _instance;

        [SerializeField] private AudioSource _audioSourcePrefab;
        [SerializeField] private SoundDatabase _database;
        [SerializeField] private int _defaultCapacity = 16;
        [SerializeField] private int _maxSize = 64;
        [SerializeField] private Sound3DSetting _defaultSetting;

        private ObjectPool<AudioSource> _audioSourcePool;

        // Enter Play Modeの「Reload Domain」を無効にしている場合に前回のInstanceが残るのを防ぐ
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatic()
        {
            _instance = null;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogError($"[Sound3DPlayer] 既にInstanceが存在します。重複した {name} を破棄します");
                Destroy(gameObject);
                return;
            }

            _instance = this;

            _audioSourcePool = new ObjectPool<AudioSource>(
                createFunc: OnCreate,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroyItem,
                collectionCheck: false,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize);
        }

        private void OnDestroy()
        {
            if (_instance != this)
            {
                return;
            }

            _instance = null;
            _audioSourcePool?.Dispose();
        }

        public static void Play3D(DynamicEnumValue type, Vector3 position, Transform parent = null)
        {
            Play3D(type, position, _instance._defaultSetting, parent);
        }

        public static void Play3D(DynamicEnumValue type, Vector3 position, Sound3DSetting setting, Transform parent = null)
        {
            if (!TryGetInstance(out var instance))
            {
                return;
            }

            var clipData = instance._database.GetClipData(type);
            clipData.GetClip(out var clip, out var volume);
            instance.PlayInternal(clip, position, volume, setting, parent);
        }

        public static void Play3D(AudioClip clip, Vector3 position, float volume = 1f, Transform parent = null)
        {
            Play3D(clip, position, _instance._defaultSetting, volume, parent);
        }

        public static void Play3D(AudioClip clip, Vector3 position, Sound3DSetting setting, float volume = 1f, Transform parent = null)
        {
            if (!TryGetInstance(out var instance))
            {
                return;
            }

            instance.PlayInternal(clip, position, volume, setting, parent);
        }

        private static bool TryGetInstance(out Sound3DPlayer instance)
        {
            instance = _instance;
            if (instance != null)
            {
                return true;
            }

            Debug.LogError("[Sound3DPlayer] シーン上にInstanceが存在しません");
            return false;
        }

        private void PlayInternal(AudioClip clip, Vector3 position, float volume, Sound3DSetting setting, Transform parent)
        {
            if (clip == null)
            {
                Debug.LogError("[Sound3DPlayer] AudioClipがnullです");
                return;
            }

            var audioSource = _audioSourcePool.Get();

            var sourceTransform = audioSource.transform;
            if (parent != null)
            {
                sourceTransform.SetParent(parent, worldPositionStays: true);
            }
            sourceTransform.position = position;

            audioSource.clip = clip;
            audioSource.volume = volume;
            setting.SetSetting(audioSource);
            audioSource.Play();

            ReleaseAfterPlayAsync(audioSource, clip.length).Forget();
        }

        private async UniTaskVoid ReleaseAfterPlayAsync(AudioSource audioSource, float duration)
        {
            var isCanceled = await UniTask
                .WaitForSeconds(duration, ignoreTimeScale: true, cancellationToken: destroyCancellationToken)
                .SuppressCancellationThrow();

            if (isCanceled || audioSource == null)
            {
                return;
            }

            _audioSourcePool.Release(audioSource);
        }

        private AudioSource OnCreate()
        {
            var audioSource = Instantiate(_audioSourcePrefab, transform);
            audioSource.playOnAwake = false;
            return audioSource;
        }

        private void OnGet(AudioSource audioSource)
        {
            audioSource.gameObject.SetActive(true);
        }

        private void OnRelease(AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.transform.SetParent(transform, worldPositionStays: false);
            audioSource.gameObject.SetActive(false);
        }

        private void OnDestroyItem(AudioSource audioSource)
        {
            if (audioSource != null)
            {
                Destroy(audioSource.gameObject);
            }
        }
    }

    [System.Serializable]
    public class Sound3DSetting
    {
        [Range(0f, 1f)] public float SpatialBlend = 1f;
        [Range(0f, 5f)] public float DopplerLevel = 0f;
        [Range(0f, 360f)] public float Spread = 0f;
        public AudioRolloffMode RolloffMode = AudioRolloffMode.Linear;
        [Min(0f)] public float MinDistance = 1f;
        [Min(0f)] public float MaxDistance = 50f;

        public void SetSetting(AudioSource source)
        {
            source.spatialBlend = SpatialBlend;
            source.dopplerLevel = DopplerLevel;
            source.spread = Spread;
            source.rolloffMode = RolloffMode;
            source.minDistance = MinDistance;
            source.maxDistance = MaxDistance;
        }
    }
}