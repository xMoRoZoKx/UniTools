using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Tools.PlayerPrefs;
using System.IO;
using TMPro;
using Unity.VisualScripting;

namespace Tools
{
    public static class WebRequestTools
    {
        public static UnityWebRequest SetPostRequest<T>(string url, T sendData)
        {
            WWWForm form = new WWWForm();
            string json = JsonUtility.ToJson(sendData);
            UnityWebRequest request = UnityWebRequest.Post(url, form);
            byte[] postBytes = Encoding.UTF8.GetBytes(json);
            UploadHandler uploadHandler = new UploadHandlerRaw(postBytes);
            request.uploadHandler = uploadHandler;
            return request;
        }

        public static UnityWebRequest SetPutRequest<T>(string url, T sendData)
        {
            string json = JsonUtility.ToJson(sendData);
            UnityWebRequest request = UnityWebRequest.Put(url, json);

            byte[] putBytes = Encoding.UTF8.GetBytes(json);
            UploadHandler uploadHandler = new UploadHandlerRaw(putBytes);
            request.uploadHandler = uploadHandler;
            return request;
        }

        public static UnityWebRequest SetPatchRequest<T>(string url, T sendData)
        {
            UnityWebRequest request = SetPutRequest<T>(url, sendData);
            request.method = "PATCH";
            return request;
        }
        public static void DestroyDefender(this UnityWebRequest request, bool validateCertificate)
        {
            request.certificateHandler = new BypassCertificate();
        }
    }
    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
    public static class WebLoader
    {
        public static IEnumerator LoadTexture2D(string url, Action<Texture2D> getTextureEvent, bool createCash = true, bool needValidateCertificate = true)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                if (createCash && PlayerPrefsPro.HasKey(url))
                {
                    getTextureEvent.Invoke(PlayerPrefsPro.GetTexture(url));
                    yield break;
                }
                //Need set headers
                if (!needValidateCertificate) request.certificateHandler = new BypassCertificate();
                yield return request.SendWebRequest();
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                getTextureEvent.Invoke(texture);
                if (createCash) PlayerPrefsPro.SetTexture(url, texture);
            }
        }
        public static IEnumerator LoadSprite(string url, Action<Sprite> getSpriteEvent, bool createCash = true, bool needValidateCertificate = true)
        {
            yield return LoadTexture2D(url, texture =>
            {
                getSpriteEvent.Invoke(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100));
            }, createCash, needValidateCertificate);
        }
        public static IEnumerator LoadAssetBundle(string url, Action<AssetBundle> getBundleEvent, bool createCash = true, bool needValidateCertificate = true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                //need set headers
                if (createCash && PlayerPrefsPro.HasKey(url))
                {
                    AssetBundle bundle = AssetBundle.LoadFromMemory(PlayerPrefsPro.GetBytes(url));
                    getBundleEvent.Invoke(bundle);
                    yield break;
                }
                if (!needValidateCertificate) request.certificateHandler = new BypassCertificate();
                request.SendWebRequest();
                while (!request.isDone)
                {
                    yield return null;
                }
                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    if (createCash) PlayerPrefsPro.SetBytes(request.downloadHandler.data, url);
                    AssetBundle bundle = AssetBundle.LoadFromMemory(request.downloadHandler.data);
                    getBundleEvent.Invoke(bundle);
                }
            }
        }
    }
}