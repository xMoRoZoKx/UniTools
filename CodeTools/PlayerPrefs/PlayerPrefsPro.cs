using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UniTools.PlayerPrefs
{
    public enum SaveLayer
    {
        Default,
        Layer1,
        Layer2,
        Layer3,
        Layer4,
        SystemLayer
    }
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
        public static string Patch(string key, SaveLayer layer = SaveLayer.Default)
        {
            return Application.persistentDataPath + "/" + layer.ToString() + ByteStorage.EncryptString(key + "v2").Replace('/', 'f').Replace('\\', 'f').Replace(':', 'f') + "6"; ;
        }

        #region Seters
        public static void Set<T>(string key, T obj, SaveLayer layer = SaveLayer.Default) => Set(key, layer, obj, true);
        private static void Set<T>(string key, SaveLayer layer, T obj, bool addToKeysList)
        {
            SetBytes(ByteSerializer.ToByteArray(obj), key, addToKeysList, layer);

        }
        public static void SetBytes(this byte[] bytes, string key, SaveLayer layer = SaveLayer.Default) => SetBytes(bytes, key, true, layer);
        private static void SetBytes(this byte[] bytes, string key, bool addToKeysList, SaveLayer layer = SaveLayer.Default)
        {
            if (bytes == null) return;

            File.WriteAllBytes(Patch(key, layer),ByteStorage.EncryptBytes(bytes));

            if (addToKeysList) AddNewKey(key, layer);
        }
        public static void SetSprite(string key, Sprite sprite, SaveLayer layer = SaveLayer.Default) => SetTexture(key, sprite.texture, layer); 
        public static void SetTexture(string key, Texture2D texture, SaveLayer layer = SaveLayer.Default) => SetBytes(texture.EncodeToPNG(), key, layer);
        public static void SetFloat(string key, float value, SaveLayer layer = SaveLayer.Default) => Set(key, value, layer);
        public static void SetInt(string key, int value, SaveLayer layer = SaveLayer.Default) => Set(key, value, layer);
        public static void SetString(string key, string value, SaveLayer layer = SaveLayer.Default) => Set(key, value, layer);
        #endregion

        #region Geters
        public static T Get<T>(string key, SaveLayer layer = SaveLayer.Default)
        {
            return ByteSerializer.ByteArrayTo<T>(GetBytes(key, layer));
        }
        public static byte[] GetBytes(string key, SaveLayer layer = SaveLayer.Default)
        {
            if (!HasKey(key, layer)) return null;
            return ByteStorage.DecryptBytes(File.ReadAllBytes(Patch(key, layer)));
        }
        public static string GetPatch(string key, SaveLayer layer = SaveLayer.Default)
        {
            return Patch(key, layer);
        }
        public static Sprite GetSprite(string key, SaveLayer layer = SaveLayer.Default)
        {
            var texture = GetTexture(key, layer);

            if (texture == null) return null;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
        }
        public static Texture2D GetTexture(string key, SaveLayer layer = SaveLayer.Default)
        {
            if (!HasKey(key, layer)) return null;

            Texture2D texture = new(500, 500);//Texture2D.normalTexture;

            texture.LoadImage(File.ReadAllBytes(Patch(key, layer)));
            return texture;
        }
        public static float GetFloat(string key, SaveLayer layer = SaveLayer.Default) => Get<float>(key, layer);
        public static int GetInt(string key, SaveLayer layer = SaveLayer.Default) => Get<int>(key, layer);
        public static string GetString(string key, SaveLayer layer = SaveLayer.Default) => Get<string>(key, layer);
        #endregion
        #region Layers
        public static List<string> GetAllKeys(SaveLayer layer = SaveLayer.Default)
        {
            var keys = Get<List<string>>(layer + ALL_KEYS, SaveLayer.SystemLayer);
            return keys ?? new List<string>();
        }
        public static List<string> GetAllLayers()
        {
            return Get<List<string>>(ALL_LAYERS, SaveLayer.SystemLayer);
        }

        private static void AddNewKey(string newKey, SaveLayer layer)
        {
            var keys = GetAllKeys(layer);
            if (keys == null) keys = new List<string>();

            if (keys.Contains(newKey)) return;

            keys.Add(newKey);

            Set(layer + ALL_KEYS, SaveLayer.SystemLayer, keys, false);
        }
        #endregion
        public static bool HasKey(string key, SaveLayer layer = SaveLayer.Default) => File.Exists(Patch(key, layer));

        #region ClearSaves
        public static void DeleteSave(string key, SaveLayer layer = SaveLayer.Default)
        {
            if (!HasKey(key, layer)) return;

            var keys = GetAllKeys(layer);

            File.Delete(Patch(key, layer));
            keys.RemoveAll(k => k == key);

            Set(layer + ALL_KEYS, SaveLayer.SystemLayer, keys, false);

        }
#if UNITY_EDITOR
        [MenuItem("PlayerPrefsPro/Clear Default Layer")]
#endif
        public static void DeleteDefaulSaves()
        {
            DeleteAllSaves(SaveLayer.Default);
        }
        public static void DeleteAllSaves(SaveLayer layer = SaveLayer.Default)
        {
            GetAllKeys(layer)?.ForEach(key => DeleteSave(key, layer));
            Set(layer + ALL_KEYS, layer, new List<string>(), false);
        }
#if UNITY_EDITOR
        [MenuItem("PlayerPrefsPro/Clear Layer1")]
#endif
        public static void DeleteLayer1Saves()
        {
            DeleteAllSaves(SaveLayer.Layer1);
        }
#if UNITY_EDITOR
        [MenuItem("PlayerPrefsPro/Clear Layer2")]
#endif
        public static void DeleteLayer2Saves()
        {
            DeleteAllSaves(SaveLayer.Layer2);
        }
#if UNITY_EDITOR
        [MenuItem("PlayerPrefsPro/Clear Layer3")]
#endif
        public static void DeleteLayer3Saves()
        {
            DeleteAllSaves(SaveLayer.Layer3);
        }
#if UNITY_EDITOR
        [MenuItem("PlayerPrefsPro/Clear Layer4")]
#endif
        public static void DeleteLayer4Saves()
        {
            DeleteAllSaves(SaveLayer.Layer4);
        }
#if UNITY_EDITOR
        [MenuItem("PlayerPrefsPro/Clear All Layers")]
#endif
        public static void DeleteAllLayers()
        {
            EnumTools.GetValues<SaveLayer>().ForEach(layer => DeleteAllSaves(layer));
        }
        #endregion
    }
    public static class ByteStorage
    {
        private const string BASE_LAYER = "DefaultLayer";
        private const string EncryptionKey = "YourSecureKey123"; // need 16, 24 or 32 bytes for AES

       public static string EncryptString(string text)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = new byte[16]; 

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(text);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static string DecryptString(string encryptedText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = new byte[16];

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        public static byte[] EncryptBytes(byte[] data)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = new byte[16];

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return encryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }

        public static byte[] DecryptBytes(byte[] encryptedData)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = new byte[16];

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                }
            }
        }
    }
}