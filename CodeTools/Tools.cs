using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Tools
{
    public static class CodeTools
    {
        public static void InvokWithChance(Action action, int chance)
        {
            if (UnityEngine.Random.Range(0, 100) <= chance) action?.Invoke();
        }
    }
    public class TaskTools
    {
        public static Task WaitForSeconds(int value)
        {
            return Task.Delay(TimeSpan.FromSeconds(value));
        }
    }
    public static class SoundTools
    {
        //TODO simple, for fast codding
        public static AudioSource PlayAudio(this Component component, AudioClip clip, float volume = 1, bool loop = false)
        {
            if (clip == null || component == null) return null;
            var c = component.GetOrAddCommponent<AudioSource>();
            c.enabled = true;
            c.volume = volume;
            c.clip = clip;
            c.loop = loop;
            c.Play();
            return c;
        }
    }
    public static class EnumTools
    {
        public static T[] GetValues<T>() where T : Enum => (T[])Enum.GetValues(typeof(T));
        public static T ToEnum<T>(this string str) where T : Enum
        {
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                if (e.ToString() == str) return (T)e;
            }
            return default;
        }
        public static bool TryConvertToEnum<T>(this string str) where T : Enum
        {
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                if (e.ToString() == str) return true;
            }
            return false;
        }
        public static T GetRandomEnum<T>() where T : Enum
        {
            var enums = Enum.GetValues(typeof(T));
            return (T)enums.GetValue(UnityEngine.Random.Range(0, enums.Length));
        }
    }
    public static class GeometryTools
    {
        public static Vector3 GetDirection(this Vector3 from, Vector3 to) => (to - from).normalized; 
        public static Vector2 GetDirection(this Vector2 from, Vector2 to) => (to - from).normalized;
    }
}
public static class MonobehaviorTools
{
    public static void SetActive(this Component component, bool value)
    {
        component?.gameObject?.SetActive(value);
    }
    public static T GetOrAddCommponent<T>(this Component component) where T : Component
    {
        var c = component.gameObject.GetComponent<T>();
        if (c == null) c = component.gameObject.AddComponent<T>();
        return c;
    }
    public static void OnClick(this Button button, Action onClick)
    {
        button?.onClick.RemoveAllListeners();
        button?.onClick.AddListener(() => onClick?.Invoke());
    }
}
namespace Tools.PlayerPrefs
{
    public static class PlayerPrefsPro
    {
        private const string SAVE_KEY = "MJN3S";
        private static string GetSimpleKey<T>(string key) => (key);
        private static string GetCryptoKey<T>(string key) => GetSimpleKey<T>(key);//.Encrypt(SAVE_KEY);
        public static string Patch => Application.persistentDataPath + "/" + SAVE_KEY;
        public static void Set<T>(string key, T obj)
        {
            var secretKey = GetCryptoKey<T>(key);
            SetBytes(System.Text.Encoding.Default.GetBytes(JsonUtility.ToJson(new Json<T>(obj))), secretKey);
            // UnityEngine.PlayerPrefs.SetString(secretKey, JsonUtility.ToJson(new Json<T>(obj)));//.Encrypt(secretKey));
        }
        public static T Get<T>(string key)
        {
            var secretKey = GetCryptoKey<T>(key);
            var bytes = GetBytes(secretKey);

            string json = bytes == null ? "" : System.Text.Encoding.Default.GetString(bytes);//UnityEngine.PlayerPrefs.GetString(secretKey);//.Decrypt(secretKey);
            if (String.IsNullOrEmpty(json)) return default;
            // if(json[json.Length - 1] != '}') json += '}';
            return JsonUtility.FromJson<Json<T>>(json).value;
        }
        public static void SetBytes(this byte[] bytes, string key)
        {
            File.WriteAllBytes(Patch + key, bytes);
        }
        public static byte[] GetBytes(string key)
        {
            if (!HasKey(key)) return null;
            return File.ReadAllBytes(Patch + key);
        }
        private class Json<T>
        {
            public Json(T value)
            {
                this.value = value;
            }
            public T value;
        }

        public static void SetSprite(string key, Sprite sprite) => File.WriteAllBytes(Application.persistentDataPath + SAVE_KEY + key, sprite.texture.EncodeToPNG());
        public static void SetFloat(string key, float value) => Set(key, value);
        public static void SetInt(string key, int value) => Set(key, value);
        public static void SetString(string key, string value) => Set(key, value);

        public static Sprite GetSprite(string key)
        {
            string patch = Patch + key;
            if (!File.Exists(patch)) return null;
            Texture2D texture = Texture2D.normalTexture;

            texture.LoadImage(File.ReadAllBytes(patch));
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100);
        }
        public static float GetFloat(string key) => Get<float>(key);
        public static int GetInt(string key) => Get<int>(key);
        public static string GetString(string key) => Get<string>(key);


        public static void DeleteKey(string key) => File.Delete(Patch + key);//UnityEngine.PlayerPrefs.DeleteKey(key);
        public static void DeleteAll() => File.Delete(Patch);//UnityEngine.PlayerPrefs.DeleteAll();
        public static bool HasKey(string key) => File.Exists(Patch + key);//UnityEngine.PlayerPrefs.HasKey(key);
    }
    public static class XORCipher
    {
        private static string GetRepeatKey(string s, int n)
        {
            var r = s;
            while (r.Length < n)
            {
                r += r;
            }

            return r.Substring(0, n);
        }

        private static string Cipher(string text, string secretKey)
        {
            if (String.IsNullOrEmpty(text)) return text;
            var currentKey = GetRepeatKey(secretKey, text.Length);
            var res = string.Empty;
            for (var i = 0; i < text.Length; i++)
            {
                res += ((char)(text[i] ^ currentKey[i])).ToString();
            }

            return res;
        }
        public static string Encrypt(this string plainText, string password)
            => Cipher(plainText, password);
        public static string Decrypt(this string encryptedText, string password)
            => Cipher(encryptedText, password);
    }
}
