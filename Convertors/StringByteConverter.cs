using System.Text;

namespace Drowhunter.TelemetryLib
{
    //public struct StringData
    //{
    //    public string Value;

    //    public override string ToString()
    //    {
    //        return Value;
    //    }
    //}

    public class StringByteConverter : IByteConverter<string>
    {
        private readonly Encoding encoding;

        public StringByteConverter(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public string FromBytes(byte[] data)
        {
            return encoding.GetString(data);
        }

        public byte[] ToBytes(string data)
        {
            return data != null ? encoding.GetBytes(data) : [];
        }
    }

}
