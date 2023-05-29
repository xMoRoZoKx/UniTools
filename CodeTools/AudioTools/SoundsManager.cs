using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools.PlayerPrefs;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public static class Sounds
{
    public const string SimpleClick = "Audio/Click";
}
public enum AudioType
{
    Music,
    SFX
}
public static class SoundsManager
{
    private class SourceAndType
    {
        public AudioSource source;
        public AudioType type;
    }
    private static List<SourceAndType> sourcesAndTypes = new List<SourceAndType>();
    public static float musicVolume
    {
        get => GetVolume(nameof(musicVolume));
        private set => SetVolume(nameof(musicVolume), value);
    }
    public static float sfxVolume
    {
        get => GetVolume(nameof(sfxVolume));
        private set => SetVolume(nameof(sfxVolume), value);
    }
    private static void SetVolume(string key, float value) => PlayerPrefsPro.SetFloat(key, value == 0 ? 0.01f : value);
    private static float GetVolume(string key)
    {
        var val = PlayerPrefsPro.GetFloat(key);
        return val == 0 ? 0.01f : val;
    }
    public static AudioSource PlayAudio(string clicpPatch, float volume = 1, bool loop = false, AudioType type = AudioType.SFX)
        => PlayAudio(LoadAudio(clicpPatch), volume, loop, type);
    public static AudioSource PlayAudio(AudioClip clip, float volume = 1, bool loop = false, AudioType type = AudioType.SFX)
    {
        sourcesAndTypes.RemoveAll(s => s.source == null);
        var sourceAndType = sourcesAndTypes.Find(s => !s.source.isPlaying);
        if (sourceAndType == null)
        {
            sourceAndType = new SourceAndType() { source = new GameObject("SoundObject").AddComponent<AudioSource>(), type = type };
            sourcesAndTypes.Add(sourceAndType);
        }
        sourceAndType.type = type;

        float maxVol = type == AudioType.Music ? musicVolume : sfxVolume;

        sourceAndType.source.clip = clip;
        sourceAndType.source.loop = loop;
        sourceAndType.source.volume = volume * maxVol;
        sourceAndType.source.Play();
        return sourceAndType.source;
    }
    public static void SetGlobalMaxVolume(AudioType type, float vol)
    {
        if (vol == 0) vol = 0.0001f;
        sourcesAndTypes.FindAll(st => st.type == type).ForEach(st => st.source.volume = st.source.volume / (type == AudioType.Music ? musicVolume : sfxVolume) * vol);
        if (type == AudioType.Music) musicVolume = vol;
        else if (type == AudioType.SFX) sfxVolume = vol;
    }
    public static AudioClip LoadAudio(string clipPatch) => Resources.Load<AudioClip>(clipPatch.ToString());
    public static void OnClickWithSound(this Button btn, Action reaction, string clipPatch = Sounds.SimpleClick)
    {
        btn.OnClick(() =>
        {
            PlayAudio(clipPatch);
            reaction?.Invoke();
        });
    }
}