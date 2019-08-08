using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using System.Text;
#endif

namespace LegendaryTools
{
    public class Serialization
    {
        public static string SaveXML<T>(T data)
        {
            using (var writer = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, data);
                return writer.ToString();
            }
        }

        public static T LoadXML<T>(string data)
        {
            using (var reader = new StringReader(data))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }

        public static byte[] SaveBinary<T>(T data)
        {
            using (var stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
                return stream.ToArray();
            }
        }

        public static T LoadBinary<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
        }

#if !UNITY_WEBPLAYER

        public static void SaveBinaryToFile<T>(T data, string fileName)
        {
            File.WriteAllBytes(fileName, SaveBinary<T>(data));
        }

        public static T LoadBinaryFromFile<T>(string fileName)
        {
            return LoadBinary<T>(File.ReadAllBytes(fileName));
        }

        public static void SaveXMLToFile<T>(T data, string FileName)
        {
            using (var writer = new StreamWriter(FileName))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, data);
                writer.Flush();
            }
        }

        public static T LoadXMLFromFile<T>(string FileName)
        {
            using (var stream = File.OpenRead(FileName))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stream);
            }
        }

#endif

#if ODIN_INSPECTOR

        public static byte[] SaveOdinBinary<T>(T data)
        {
            return SerializationUtility.SerializeValue<T>(data, DataFormat.Binary);
        }

        public static T LoadOdinBinary<T>(byte[] data)
        {
            return SerializationUtility.DeserializeValue<T>(data, DataFormat.Binary);
        }

        public static string SaveOdinJson<T>(T data)
        {
            return Encoding.UTF8.GetString(SerializationUtility.SerializeValue<T>(data, DataFormat.JSON));
        }

        public static T LoadOdinJson<T>(string jsonString)
        {
            return SerializationUtility.DeserializeValue<T>(System.Text.Encoding.UTF8.GetBytes(jsonString), DataFormat.JSON);
        }

        public static void SaveOdinBinaryToFile<T>(T data, string fileName)
        {
            File.WriteAllBytes(fileName, SaveOdinBinary<T>(data));
        }

        public static T LoadOdinBinaryFromFile<T>(string fileName)
        {
            return LoadOdinBinary<T>(File.ReadAllBytes(fileName));
        }

        public static void SaveOdinJsonToFile<T>(T data, string fileName)
        {
            File.WriteAllBytes(fileName, SerializationUtility.SerializeValue<T>(data, DataFormat.JSON));
        }

        public static T LoadOdinJsonFromFile<T>(string fileName)
        {
            return SerializationUtility.DeserializeValue<T>(File.ReadAllBytes(fileName), DataFormat.JSON);
        }
#endif
    }
}