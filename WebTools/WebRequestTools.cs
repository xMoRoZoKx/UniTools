using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace UniTools
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
        public static void RemoveCertificateValidation(this UnityWebRequest request)
        {
            request.certificateHandler = new BypassCertificate();
        }
        public static bool HasError(this UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError ||
                request.result == UnityWebRequest.Result.DataProcessingError)
            {
                return true;
            }
            return false;
        }
    }
    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}