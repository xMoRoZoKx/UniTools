using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        // public static void DeleteAll() => File.Delete(Patch);//UnityEngine.PlayerPrefs.DeleteAll();
        public static bool HasKey(string key) => File.Exists(Patch + key);//UnityEngine.PlayerPrefs.HasKey(key);
    }
    //TODO LOST DATAS. problem with float value, maybe problem in json
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

