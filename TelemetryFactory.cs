using System.Collections.Concurrent;

namespace Drowhunter.TelemetryLib
{
    public interface IMmfTelemetryFactory
    {
        MmfTelemetry<TData> GetOrCreate<TConsumer, TData>(IByteConverter<TData> converter, Action<MmfTelemetryConfig> setup = null)
            where TConsumer : class
            where TData : struct;
    }

    public class MmfTelemetryFactory : IMmfTelemetryFactory
    {
        private readonly ConcurrentDictionary<string, object> _telemetryInstances = new();

        public MmfTelemetry<TData> GetOrCreate<TConsumer, TData>(IByteConverter<TData> converter, Action<MmfTelemetryConfig> setup = null)
            where TConsumer : class
            where TData : struct
        {
            var key = typeof(TConsumer).FullName ?? typeof(TConsumer).Name;
            return (MmfTelemetry<TData>)_telemetryInstances.GetOrAdd(key, _ =>
            {
                var config = new MmfTelemetryConfig();
                setup?.Invoke(config);
                return new MmfTelemetry<TData>(config, converter);
            });
        }
    }

    public interface IUdpTelemetryFactory
    {
        UdpTelemetry<TData> GetOrCreate<TConsumer, TData>(IByteConverter<TData> converter, Action<UdpTelemetryConfig> setup = null)
            where TConsumer : class;
    }

    public class UdpTelemetryFactory : IUdpTelemetryFactory
    {
        private readonly ConcurrentDictionary<string, object> _telemetryInstances = new();

        public UdpTelemetry<TData> GetOrCreate<TConsumer, TData>(IByteConverter<TData> converter, Action<UdpTelemetryConfig> setup = null)
            where TConsumer : class            
        {
            var key = typeof(TConsumer).FullName ?? typeof(TConsumer).Name;
            return (UdpTelemetry<TData>)_telemetryInstances.GetOrAdd(key, _ =>
            {
                var config = new UdpTelemetryConfig();
                setup?.Invoke(config);
                return new UdpTelemetry<TData>(config, converter);
            });
        }
    }

    public interface ITcpTelemetryFactory
    {
        TcpTelemetry<TData> GetOrCreate<TConsumer, TData>(IByteConverter<TData> converter, Action<TcpTelemetryConfig> setup = null)
            where TConsumer : class;
    }

    public class TcpTelemetryFactory : ITcpTelemetryFactory
    {
        private readonly ConcurrentDictionary<string, object> _telemetryInstances = new();

        public TcpTelemetry<TData> GetOrCreate<TConsumer, TData>(IByteConverter<TData> converter, Action<TcpTelemetryConfig> setup = null)
            where TConsumer : class
        {
            var key = typeof(TConsumer).FullName ?? typeof(TConsumer).Name;
            return (TcpTelemetry<TData>)_telemetryInstances.GetOrAdd(key, _ =>
            {
                var config = new TcpTelemetryConfig();
                setup?.Invoke(config);
                return new TcpTelemetry<TData>(config, converter);
            });
        }
    }




}
