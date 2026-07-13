using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace com.drowhunter.TelemetryLib
{
    /// <summary>
    /// Converter json to byte array and vice versa.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonByteConverter<T> : IByteConverter<T> //where T : struct
    {
        public byte[] ToBytes(T data)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, data);
                return stream.ToArray();
            }
        }

        public T FromBytes(byte[] data)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new MemoryStream(data ?? Array.Empty<byte>()))
            {
                return (T)serializer.ReadObject(stream);
            }
        }
    }

}
