using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tools.PlayerPrefs
{
    public static class PlayerPrefsPro
    {
        private class Json<T>
        {
            public Json(T value)
            {
                this.value = value;
            }
            public T value;
        }
        private const string BASE_SAVE_KEY = "MJN3S";
        public static string Patch => Application.persistentDataPath + "/" + BASE_SAVE_KEY;
        public static string GetKey(string key) => key.Trim('/');
        public static void Set<T>(string key, T obj) => Set<T>(key, obj, true);
        private static void Set<T>(string key, T obj, bool saveKey)
        {
            SetBytes(System.Text.Encoding.Default.GetBytes(JsonUtility.ToJson(new Json<T>(obj))), key);
            if (saveKey) TryAddNewKey(key);
        }
        public static T Get<T>(string key)
        {
            key = GetKey(key);
            var bytes = GetBytes(key);

            string json = bytes == null ? "" : System.Text.Encoding.Default.GetString(bytes);
            if (String.IsNullOrEmpty(json)) return default;
            // if(json[json.Length - 1] != '}') json += '}';
            return JsonUtility.FromJson<Json<T>>(json).value;
        }
        public static void SetBytes(this byte[] bytes, string key)
        {
            key = GetKey(key);
            File.WriteAllBytes(Patch + key, bytes);
        }
        public static byte[] GetBytes(string key)
        {
            key = GetKey(key);
            if (!HasKey(key)) return null;
            return File.ReadAllBytes(Patch + key);
        }
        public static string GetPatch(string key)
        {
            key = GetKey(key);
            return Patch + key;
        }

        public static void SetSprite(string key, Sprite sprite) => File.WriteAllBytes(Patch + GetKey(key), sprite.texture.EncodeToPNG());
        public static void SetTexture(string key, Texture2D texture) => File.WriteAllBytes(Patch + GetKey(key), texture.EncodeToPNG());
        public static void SetFloat(string key, float value) => Set(GetKey(key), value);
        public static void SetInt(string key, int value) => Set(GetKey(key), value);
        public static void SetString(string key, string value) => Set(GetKey(key), value);

        public static Sprite GetSprite(string key)
        {
            key = GetKey(key);
            var texture = GetTexture(key);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100);
        }
        public static Texture2D GetTexture(string key)
        {
            key = GetKey(key);
            string patch = Patch + key;
            if (!File.Exists(patch)) return null;
            Texture2D texture = Texture2D.normalTexture;

            texture.LoadImage(File.ReadAllBytes(patch));
            return texture;
        }
        public static float GetFloat(string key) => Get<float>(GetKey(key));
        public static int GetInt(string key) => Get<int>(GetKey(key));
        public static string GetString(string key) => Get<string>(GetKey(key));

        public static bool HasKey(string key) => File.Exists(Patch + GetKey(key));

        private static List<string> GetAllKeys()
        {
            return Get<List<string>>(BASE_SAVE_KEY + "_ALL_KEYS");
        }
        private static void TryAddNewKey(string newKey)
        {
            var keys = GetAllKeys();
            if (keys == null) keys = new List<string>();
            if (keys.Contains(newKey)) return;
            keys.Add(newKey);
            Set<List<string>>(BASE_SAVE_KEY + "_ALL_KEYS", keys, false);
        }
        public static void DeleteKey(string key)
        {
            key = GetKey(key);
            if (!HasKey(key)) return;
            var keys = GetAllKeys();
            File.Delete(Patch + keys.Find(k => k == key));
            Set<List<string>>(BASE_SAVE_KEY + "_ALL_KEYS", keys, false);
        }
#if UNITY_EDITOR
        [MenuItem("PlayerPrefsPro/Clear")]
#endif
        public static void DeleteAllKeys()
        {
            GetAllKeys()?.ForEach(key => File.Delete(Patch + key));
            Set<List<string>>(BASE_SAVE_KEY + "_ALL_KEYS", new List<string>(), false);
        }
    }
}