namespace com.drowhunter.TelemetryLib
{
    public interface IByteConverter<T>// where T : struct
    {
        T FromBytes(byte[] data);
        byte[] ToBytes(T data);
    }

}
