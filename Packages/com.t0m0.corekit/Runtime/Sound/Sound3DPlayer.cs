//AI
using CoreKit.DynamicEnum;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace CoreKit
{
    /// <summary>
    /// 3D空間上の位置でSEを再生する責務を持つ。AudioSourceはプールして再利用する。
    /// </summary>
    public class Sound3DPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSourcePrefab;
        [SerializeField] private SoundDatabase _database;
        [SerializeField] private int _defaultCapacity = 16;
        [SerializeField] private int _maxSize = 64;

        private ObjectPool<AudioSource> _audioSourcePool;

        private void Awake()
        {
            _audioSourcePool = new ObjectPool<AudioSource>(
                createFunc: OnCreate,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroyItem,
                collectionCheck: false, // Release時の重複チェックはコストがあるため無効化
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize);
        }

        private void OnDestroy()
        {
            _audioSourcePool?.Dispose();
        }

        public void Play3D(DynamicEnumValue type, Vector3 position, Transform parent = null)
        {
            var clipData = _database.GetClipData(type);
            Play3D(clipData.Clip, position, clipData.Volume, parent);
        }

        public void Play3D(AudioClip clip, Vector3 position, float volume = 1f, Transform parent = null)
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
            audioSource.Play();

            ReleaseAfterPlayAsync(audioSource, clip.length).Forget();
        }

        private async UniTaskVoid ReleaseAfterPlayAsync(AudioSource audioSource, float duration)
        {
            // 再生はTime.timeScaleの影響を受けないため、ignoreTimeScaleで待つ
            var isCanceled = await UniTask
                .WaitForSeconds(duration, ignoreTimeScale: true, cancellationToken: destroyCancellationToken)
                .SuppressCancellationThrow();

            // 破棄時はプールごと破棄されるためReleaseしない
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
}