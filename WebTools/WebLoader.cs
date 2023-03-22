using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Tools.PlayerPrefs;

namespace Tools
{
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