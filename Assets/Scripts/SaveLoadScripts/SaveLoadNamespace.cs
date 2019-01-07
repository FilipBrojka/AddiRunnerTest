
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SaveLoadNamespace
{
    public static class BinarySerializer
    {
        public static void Save<T>(T data) where T : class
        {
            string path = Application.persistentDataPath + "/" + typeof(T).Name + ".sav";

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Create(path);

            binaryFormatter.Serialize(fileStream, data);
            fileStream.Close();

            Debug.Log("Data saved: " + path);
        }

        public static T Load<T>() where T : class
        {
            string path = Application.persistentDataPath + "/" + typeof(T).Name + ".sav";

            if (File.Exists(path))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(path, FileMode.Open);

                T data = (T)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();

                Debug.Log("Data loaded: " + path);

                return data;
            }

            Debug.LogWarning("No data file found at: " + path);
            return null;
        }

        public static void Delete<T>() where T : class
        {
            string path = Application.persistentDataPath + "/" + typeof(T).Name + ".sav";

            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("Data deleted: " + path);
            }
            else
            {
                Debug.LogWarning("No data file found at: " + path);
            }
        }
    }
}