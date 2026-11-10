using CoreKit.DynamicEnum;
using UnityEngine;

namespace CoreKit
{
    [System.Serializable]
    public class SoundSection
    {
        [SerializeField] private SoundDatabase _database;
        [SerializeField] private AudioSource _audioScore;

        public void Play(DynamicEnumValue type)
        {
            var clipData = _database.GetClipData(type);
            Play(clipData.Clip, clipData.Volume);
        }

        public void Play(AudioClip clip,float volume)
        {
            _audioScore.volume = volume;
            _audioScore.clip = clip;
            _audioScore.Play();
        }

        public void PlayOneShot(DynamicEnumValue type)
        {
            var clipData = _database.GetClipData(type);
            PlayOneShot(clipData.Clip, clipData.Volume);
        }

        public void PlayOneShot(AudioClip clip, float volume)
        {
            _audioScore.PlayOneShot(clip,volume);
        }
    }

}
