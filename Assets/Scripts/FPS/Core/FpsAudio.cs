using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public static class FpsAudio
    {
        private static readonly Dictionary<string, AudioClip> _cache = new();

        public static void PlayAt(string assetPath, Vector3 position, float volume = 1f)
        {
            AudioClip clip = GetClip(assetPath);
            if (clip == null) return;

            AudioSource.PlayClipAtPoint(clip, position, volume);
        }

        public static AudioClip GetClip(string assetPath)
        {
            if (_cache.TryGetValue(assetPath, out AudioClip cached) && cached != null)
            {
                return cached;
            }

            AudioClip clip = ProjectAsset.Load<AudioClip>(assetPath);
            if (clip != null)
            {
                _cache[assetPath] = clip;
            }

            return clip;
        }
    }
}
