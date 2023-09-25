using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class ByteSerializer
{
    public static byte[] ToByteArray<T>(T obj)
    {
        if (obj == null)
            return null;

        byte[] bytes;
        using (MemoryStream memStream = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(memStream, obj);
            bytes = memStream.ToArray();
        }

        return bytes;
    }
    public static T ByteArrayTo<T>(byte[] arrBytes)
    {
        if (arrBytes == null || arrBytes.Length == 0) return default;

        T obj;
        using (MemoryStream memStream = new MemoryStream())
        {
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            obj = (T)binForm.Deserialize(memStream);
        };

        return obj;
    }
}
