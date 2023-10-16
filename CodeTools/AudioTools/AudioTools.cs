using UnityEngine;

namespace UniTools
{
    public static class AudioTools
    {
        //TODO simple, for fast codding
        public static AudioSource PlayAudio(this Component component, AudioClip clip, float volume = 1, bool loop = false)
        {
            if (clip == null || component == null) return null;
            var c = component.GetOrAddComponent<AudioSource>();
            c.enabled = true;
            c.volume = volume;
            c.clip = clip;
            c.loop = loop;
            c.Play();
            return c;
        }
    }
}
