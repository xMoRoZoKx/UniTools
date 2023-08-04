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
        public const string BASE_LAYER = "BASE", ALL_KEYS = "_ALL_KEYS", ALL_LAYERS = "_ALL_LAYERS", CONSTANT_LAYER = "CONST_LAYER";
        public static string Patch(string layerName = BASE_LAYER)
        {
            return Application.persistentDataPath + "/" + layerName;
        }
        public static string GetKey(string key) => key.Replace('/', 'f').Replace('\\', 'f').Replace(':', 'f');
        public static void Set<T>(string key, T obj, string layerName = BASE_LAYER) => Set<T>(key, layerName, obj, true, true);
        private static void Set<T>(string key, string layerName, T obj, bool needAddToKeysList, bool needAddToLayersList)
        {
            SetBytes(System.Text.Encoding.Default.GetBytes(JsonUtility.ToJson(new Json<T>(obj))), key, layerName);
        }
        public static T Get<T>(string key, string layerName = BASE_LAYER)
        {
            key = GetKey(key);
            var bytes = GetBytes(key, layerName);

            string json = bytes == null ? "" : System.Text.Encoding.Default.GetString(bytes);
            if (String.IsNullOrEmpty(json)) return default;
            return JsonUtility.FromJson<Json<T>>(json).value;
        }
        public static void SetBytes(this byte[] bytes, string key, string layerName = BASE_LAYER) => SetBytes(bytes, key, true, true, layerName);
        private static void SetBytes(this byte[] bytes, string key, bool needAddToKeysList, bool needAddToLayersList, string layerName = BASE_LAYER)
        {
            key = GetKey(key);
            File.WriteAllBytes(Patch(layerName) + key, bytes);
            if (needAddToLayersList) TryAddNewLayer(layerName);
            if (needAddToKeysList) TryAddNewKey(key, layerName);
        }
        public static byte[] GetBytes(string key, string layerName = BASE_LAYER)
        {
            key = GetKey(key);
            if (!HasKey(key, layerName)) return null;
            return File.ReadAllBytes(Patch(layerName) + key);
        }
        public static string GetPatch(string key, string layerName = BASE_LAYER)
        {
            key = GetKey(key);
            return Patch(layerName) + key;
        }

        public static void SetSprite(string key, Sprite sprite, string layerName = BASE_LAYER) => File.WriteAllBytes(Patch(layerName) + GetKey(key), sprite.texture.EncodeToPNG());
        public static void SetTexture(string key, Texture2D texture, string layerName = BASE_LAYER) => File.WriteAllBytes(Patch(layerName) + GetKey(key), texture.EncodeToPNG());
        public static void SetFloat(string key, float value, string layerName = BASE_LAYER) => Set(GetKey(key), value, layerName);
        public static void SetInt(string key, int value, string layerName = BASE_LAYER) => Set(GetKey(key), value, layerName);
        public static void SetString(string key, string value, string layerName = BASE_LAYER) => Set(GetKey(key), value, layerName);

        public static Sprite GetSprite(string key, string layerName = BASE_LAYER)
        {
            key = GetKey(key);
            var texture = GetTexture(key, layerName);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100);
        }
        public static Texture2D GetTexture(string key, string layerName = BASE_LAYER)
        {
            key = GetKey(key);
            string patch = Patch(layerName) + key;
            if (!File.Exists(patch)) return null;
            Texture2D texture = Texture2D.normalTexture;

            texture.LoadImage(File.ReadAllBytes(patch));
            return texture;
        }
        public static float GetFloat(string key, string layerName = BASE_LAYER) => Get<float>(GetKey(key), layerName);
        public static int GetInt(string key, string layerName = BASE_LAYER) => Get<int>(GetKey(key), layerName);
        public static string GetString(string key, string layerName = BASE_LAYER) => Get<string>(GetKey(key), layerName);

        public static bool HasKey(string key, string layerName = BASE_LAYER) => File.Exists(Patch(layerName) + GetKey(key));

        private static List<string> GetAllKeys(string layerName)
        {
            return Get<List<string>>(layerName + ALL_KEYS, CONSTANT_LAYER);
        }
        private static void TryAddNewKey(string newKey, string layerName)
        {
            var keys = GetAllKeys(layerName);
            if (keys == null) keys = new List<string>();
            if (keys.Contains(newKey)) return;
            keys.Add(newKey);
            Set<List<string>>(layerName + ALL_KEYS, CONSTANT_LAYER, keys, false, false);
        }
        private static List<string> GetAllLayers()
        {
            return Get<List<string>>(ALL_LAYERS, CONSTANT_LAYER);
        }
        private static void TryAddNewLayer(string newLayer)
        {
            var layers = GetAllLayers();
            if (layers == null) layers = new List<string>();
            if (layers.Contains(newLayer)) return;
            layers.Add(newLayer);
            Set<List<string>>(ALL_LAYERS, CONSTANT_LAYER, layers, false, false);
        }
        public static void DeleteSave(string key, string layerName = BASE_LAYER)
        {
            key = GetKey(key);
            if (!HasKey(key, layerName)) return;
            var keys = GetAllKeys(layerName);
            File.Delete(Patch(layerName) + keys.Find(k => k == key));
            Set<List<string>>(layerName + ALL_KEYS, layerName, keys, false, false);
        }
#if UNITY_EDITOR
        [MenuItem("PlayerPrefsPro/Clear Default Layer")]
#endif
        public static void DeleteDefaulSaves()
        {
            GetAllKeys(BASE_LAYER)?.ForEach(key => File.Delete(Patch(BASE_LAYER) + key));
            Set<List<string>>(BASE_LAYER + ALL_KEYS, BASE_LAYER, new List<string>(), false, false);
        }
        public static void DeleteAllSaves(string layerName = BASE_LAYER)
        {
            GetAllKeys(layerName)?.ForEach(key => File.Delete(Patch(layerName) + key));
            Set<List<string>>(layerName + ALL_KEYS, layerName, new List<string>(), false, false);
        }
#if UNITY_EDITOR
        [MenuItem("PlayerPrefsPro/Clear All Layers")]
#endif
        public static void DeleteAllLayers()
        {
            GetAllLayers()?.ForEach(layer => DeleteAllSaves(layer));
            Set<List<string>>(ALL_LAYERS, CONSTANT_LAYER, new List<string>(), false, false);
        }
    }
}