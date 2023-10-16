using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UniTools.PlayerPrefs;

namespace UniTools
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
                if (!needValidateCertificate) request.RemoveCertificateValidation();
                yield return request.SendWebRequest();
                if (request.HasError())
                {
                    Debug.LogError(request.error);
                }
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
        public static IEnumerator LoadAssetBundle(string url, Action<AssetBundle> getBundleEvent, Action<float> progress, bool createCash = true, bool needValidateCertificate = true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                //need set headers
                AssetBundle bundle = null;
                if (createCash && PlayerPrefsPro.HasKey(url))
                {
                    bundle = AssetBundle.LoadFromMemory(PlayerPrefsPro.GetBytes(url));
                    getBundleEvent.Invoke(bundle);
                    yield break;
                }

                if (!needValidateCertificate) request.RemoveCertificateValidation();

                request.SendWebRequest();
                while (!request.isDone)
                {
                    yield return null;
                }

                if (request.HasError())
                {
                    Debug.LogError(request.error);
                    yield break;
                }

                if (createCash) PlayerPrefsPro.SetBytes(request.downloadHandler.data, url);

                bundle = AssetBundle.LoadFromMemory(request.downloadHandler.data);
                getBundleEvent.Invoke(bundle);

            }
        }
        public static IEnumerator LoadText(string url, Action<string> getTextEvent, Action<UnityWebRequest> onCreateRequest = null)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                onCreateRequest?.Invoke(request);
                yield return request.SendWebRequest();
                if (request.HasError())
                {
                    Debug.LogError(request.error);
                    yield break;
                }
                getTextEvent(request.downloadHandler.text);
            }
        }
        public static IEnumerator LoadGoogleTable(string sheetId, string gid, Action<GoogleTable> getTableEvent)
        {
            string text = "";//ttps://docs.google.com/spreadsheets/d/KEY/export?format=csv&gid=SHEET_ID
            //https://docs.google.com/spreadsheets/d/{key}{sheet_name}
            yield return LoadText("https://docs.google.com/spreadsheets/d/" + sheetId + "/gviz/tq?tqx=out:csv&sheet=" + gid, txt => text = txt);
            getTableEvent(new GoogleTable(text));
        }
    }
}