using UnityEngine;
using static CoreKit.Sound.SoundDatabase;

namespace CoreKit.Sound
{
    [System.Serializable]
    public class RandomClipData : ClipData
    {
        [SerializeField,Inline] private RandomClipEntry[] _clipEntry;

        [System.Serializable]
        public class RandomClipEntry
        {
            [SerializeField] private AudioClip _clip;
            [SerializeField, Range(0f, 1f)] public float _volume = 1f;
            [SerializeField] private float _weight;

            public AudioClip Clip => _clip;
            public float Volume => _volume;
            public float Weight => _weight;
        }

        /// <summary>
        /// d‚İ•t‚«ƒ‰ƒ“ƒ_ƒ€‚ÅClip‚ğæ“¾
        /// </summary>
        public override void GetClip(out AudioClip clip, out float volume)
        {
            clip = null;
            volume = 0f;

            if (_clipEntry == null || _clipEntry.Length == 0)
                return;

            float totalWeight = 0f;

            foreach (var entry in _clipEntry)
            {
                if (entry == null || entry.Clip == null || entry.Weight <= 0f)
                    continue;

                totalWeight += entry.Weight;
            }

            if (totalWeight <= 0f)
                return;

            float random = Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            RandomClipEntry lastEntry = null;

            foreach (var entry in _clipEntry)
            {
                if (entry == null || entry.Clip == null || entry.Weight <= 0f)
                    continue;

                currentWeight += entry.Weight;
                lastEntry = entry;

                if (random < currentWeight)
                {
                    clip = entry.Clip;
                    volume = entry.Volume * _volume;
                    return;
                }
            }

            clip = lastEntry.Clip;
            volume = lastEntry.Volume * _volume;

            return;
        }
    }
}
