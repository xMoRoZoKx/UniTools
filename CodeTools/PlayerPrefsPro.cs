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
        private const string SAVE_KEY = "MJN3S";
        private static string GetKey(string key) => key;
        public static string Patch => Application.persistentDataPath + "/" + SAVE_KEY;
        public static void Set<T>(string key, T obj) => Set<T>(key, obj, true);
        private static void Set<T>(string key, T obj, bool saveKey)
        {
            var secretKey = GetKey(key);
            SetBytes(System.Text.Encoding.Default.GetBytes(JsonUtility.ToJson(new Json<T>(obj))), secretKey);
            if (saveKey) SetNewKey(secretKey);
        }
        public static T Get<T>(string key)
        {
            var secretKey = GetKey(key);
            var bytes = GetBytes(secretKey);

            string json = bytes == null ? "" : System.Text.Encoding.Default.GetString(bytes);
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

        public static void SetSprite(string key, Sprite sprite) => File.WriteAllBytes(Patch + key, sprite.texture.EncodeToPNG());
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

        public static bool HasKey(string key) => File.Exists(Patch + key);

        private static List<string> GetAllKeys()
        {
            return Get<List<string>>(SAVE_KEY + "_ALL_KEYS");
        }
        private static void SetNewKey(string newKey)
        {
            var keys = GetAllKeys();
            if (keys == null) keys = new List<string>();
            if (keys.Contains(newKey)) return;
            keys.Add(newKey);
            Set<List<string>>(SAVE_KEY + "_ALL_KEYS", keys, false);
        }
        public static void DeleteKey(string key)
        {
            if (!HasKey(key)) return;
            var keys = GetAllKeys();
            File.Delete(Patch + keys.Find(k => k == GetKey(key)));
            Set<List<string>>(SAVE_KEY + "_ALL_KEYS", keys, false);
        }
        [MenuItem("PlayerPrefsPro/Clear")]
        public static void DeleteAllKeys()
        {
            GetAllKeys()?.ForEach(key => File.Delete(Patch + GetKey(key)));
            Set<List<string>>(SAVE_KEY + "_ALL_KEYS", new List<string>(), false);
        }
    }
}