using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
// using Newtonsoft.Json;

namespace UniTools.PlayerPrefs
{
    public static class PlayerPrefsPro
    {
        [System.Serializable]
        private class Json<T>
        {
            public Json(T value)
            {
                this.value = value;
            }
            public T value;
        }
        public const string BASE_LAYER = "BASE", ALL_KEYS = "_ALL_KEYS", ALL_LAYERS = "_ALL_LAYERS", CONSTANT_LAYER = "CONST_LAYER";
        public static string Patch(string key, string layerName = BASE_LAYER)
        {
            return Application.persistentDataPath + "/" + layerName + key.Replace('/', 'f').Replace('\\', 'f').Replace(':', 'f') + "6"; ;
        }
        // private static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        #region Seters
        public static void Set<T>(string key, T obj, string layerName = BASE_LAYER) => Set<T>(key, layerName, obj, true, true);
        private static void Set<T>(string key, string layerName, T obj, bool needAddToKeysList, bool needAddToLayersList)
        {
            // SetBytes(System.Text.Encoding.Default.GetBytes(JsonUtility.ToJson(new Json<T>(obj))), key, needAddToKeysList, needAddToLayersList, layerName);
            
            SetBytes(ByteSerializer.ToByteArray(obj), key, needAddToKeysList, needAddToLayersList, layerName);

            // SetBytes(System.Text.Encoding.Default.GetBytes(JsonConvert.SerializeObject(new Json<T>(obj), settings)), key, needAddToKeysList, needAddToLayersList, layerName);
        }
        public static void SetBytes(this byte[] bytes, string key, string layerName = BASE_LAYER) => SetBytes(bytes, key, true, true, layerName);
        private static void SetBytes(this byte[] bytes, string key, bool needAddToKeysList, bool needAddToLayersList, string layerName = BASE_LAYER)
        {
            if (bytes == null) return;

            File.WriteAllBytes(Patch(key, layerName), bytes);

            if (needAddToLayersList) AddNewLayer(layerName);
            if (needAddToKeysList) AddNewKey(key, layerName);
        }
        public static void SetSprite(string key, Sprite sprite, string layerName = BASE_LAYER) => SetTexture(key, sprite.texture, layerName); //File.WriteAllBytes(Patch(layerName) + GetKey(key), sprite.texture.EncodeToPNG());
        public static void SetTexture(string key, Texture2D texture, string layerName = BASE_LAYER) => SetBytes(texture.EncodeToPNG(), key, layerName);//=> File.WriteAllBytes(Patch(layerName) + GetKey(key), texture.EncodeToPNG());
        public static void SetFloat(string key, float value, string layerName = BASE_LAYER) => Set(key, value, layerName);
        public static void SetInt(string key, int value, string layerName = BASE_LAYER) => Set(key, value, layerName);
        public static void SetString(string key, string value, string layerName = BASE_LAYER) => Set(key, value, layerName);
        #endregion

        #region Geters
        public static T Get<T>(string key, string layerName = BASE_LAYER)
        {
            // var bytes = GetBytes(key, layerName);

            // string json = bytes == null ? "" : System.Text.Encoding.Default.GetString(bytes);
            // if (String.IsNullOrEmpty(json)) return default;
            // return JsonUtility.FromJson<Json<T>>(json).value;

            return ByteSerializer.ByteArrayTo<T>(GetBytes(key, layerName));

            // var bytes = GetBytes(key, layerName);

            // string json = bytes == null ? "" : System.Text.Encoding.Default.GetString(bytes);
            // if (String.IsNullOrEmpty(json)) return default;
            // return JsonConvert.DeserializeObject<Json<T>>(json, settings).value;
        }
        public static byte[] GetBytes(string key, string layerName = BASE_LAYER)
        {
            if (!HasKey(key, layerName)) return null;
            return File.ReadAllBytes(Patch(key, layerName));
        }
        public static string GetPatch(string key, string layerName = BASE_LAYER)
        {
            return Patch(key, layerName);
        }
        public static Sprite GetSprite(string key, string layerName = BASE_LAYER)
        {
            var texture = GetTexture(key, layerName);

            if (texture == null) return null;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
        }
        public static Texture2D GetTexture(string key, string layerName = BASE_LAYER)
        {
            if (!HasKey(key, layerName)) return null;

            Texture2D texture = new(500, 500);//Texture2D.normalTexture;

            texture.LoadImage(File.ReadAllBytes(Patch(key, layerName)));
            return texture;
        }
        public static float GetFloat(string key, string layerName = BASE_LAYER) => Get<float>(key, layerName);
        public static int GetInt(string key, string layerName = BASE_LAYER) => Get<int>(key, layerName);
        public static string GetString(string key, string layerName = BASE_LAYER) => Get<string>(key, layerName);
        #endregion
        #region Layers
        public static List<string> GetAllKeys(string layerName = BASE_LAYER)
        {
            var keys = Get<List<string>>(layerName + ALL_KEYS, CONSTANT_LAYER);
            return keys ?? new List<string>();
        }
        public static List<string> GetAllLayers()
        {
            return Get<List<string>>(ALL_LAYERS, CONSTANT_LAYER);
        }

        private static void AddNewKey(string newKey, string layerName)
        {
            var keys = GetAllKeys(layerName);
            if (keys == null) keys = new List<string>();

            if (keys.Contains(newKey)) return;

            keys.Add(newKey);

            Set(layerName + ALL_KEYS, CONSTANT_LAYER, keys, false, false);
        }
        private static void AddNewLayer(string newLayer)
        {
            var layers = GetAllLayers();
            if (layers == null) layers = new List<string>();

            if (layers.Contains(newLayer)) return;

            layers.Add(newLayer);

            Set(ALL_LAYERS, CONSTANT_LAYER, layers, false, false);
        }
        #endregion
        public static bool HasKey(string key, string layerName = BASE_LAYER) => File.Exists(Patch(key, layerName));

        #region ClearSaves
        public static void DeleteSave(string key, string layerName = BASE_LAYER)
        {
            if (!HasKey(key, layerName)) return;

            var keys = GetAllKeys(layerName);

            File.Delete(Patch(key, layerName));
            keys.RemoveAll(k => k == key);

            Set(layerName + ALL_KEYS, CONSTANT_LAYER, keys, false, false);

        }
#if UNITY_EDITOR
        [MenuItem("PlayerPrefsPro/Clear Default Layer")]
#endif
        public static void DeleteDefaulSaves()
        {
            GetAllKeys(BASE_LAYER)?.ForEach(key => DeleteSave(key, BASE_LAYER));
            Set(BASE_LAYER + ALL_KEYS, BASE_LAYER, new List<string>(), false, false);
        }
        public static void DeleteAllSaves(string layerName = BASE_LAYER)
        {
            GetAllKeys(layerName)?.ForEach(key => DeleteSave(key, layerName));
            Set(layerName + ALL_KEYS, layerName, new List<string>(), false, false);
        }
#if UNITY_EDITOR
        [MenuItem("PlayerPrefsPro/Clear All Layers")]
#endif
        public static void DeleteAllLayers()
        {
            GetAllLayers()?.ForEach(layer => DeleteAllSaves(layer));
            Set(ALL_LAYERS, CONSTANT_LAYER, new List<string>(), false, false);
        }
        #endregion
    }
}