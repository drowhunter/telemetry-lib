namespace Drowhunter.TelemetryLib
{ 
    public class RawBytesConverter : IByteConverter<byte[]>
    {
        public byte[] FromBytes(byte[] data) => data ?? Array.Empty<byte>();

        public byte[] ToBytes(byte[] data) => data ?? Array.Empty<byte>();
    }
}
