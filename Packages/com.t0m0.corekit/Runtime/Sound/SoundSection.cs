using CoreKit.DynamicEnum;
using UnityEngine;

namespace CoreKit.Sound
{
    public interface IReadOnlySoundSection
    {
        public void Play(DynamicEnumValue type);
        public void Play(AudioClip clip, float volume);
        public void PlayOneShot(DynamicEnumValue type);
        public void PlayOneShot(AudioClip clip, float volume);
        public void ChangeVolume(float volume, float duration);
    }

    [System.Serializable]
    public class SoundSection : IReadOnlySoundSection
    {
        [SerializeField] protected SoundDatabase _database;
        [SerializeField] protected AudioSource _audioScore;

        private VolumeController _volumeController = new VolumeController();

        public void Update()
        {
            _audioScore.volume = _volumeController.Volume;
            _volumeController.Update(Time.deltaTime);
        }

        public void Play(DynamicEnumValue type)
        {
            var clipData = _database.GetClipData(type);

            clipData.GetClip(out var clip, out var volume);
            Play(clip, volume);
        }

        public void Play(AudioClip clip, float volume)
        {
            _audioScore.volume = volume;
            _volumeController.Set(volume);
            _audioScore.clip = clip;
            _audioScore.Play();
        }

        public void PlayOneShot(DynamicEnumValue type)
        {
            var clipData = _database.GetClipData(type);

            clipData.GetClip(out var clip, out var volume);
            PlayOneShot(clip, volume);
        }

        public void PlayOneShot(AudioClip clip, float volume)
        {
            _audioScore.PlayOneShot(clip, volume);
        }

        public void ChangeVolume(float volume, float duration)
        {
            _volumeController.Change(volume, duration);
        }
    }

}
