using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace com.drowhunter.TelemetryLib
{

    public class UdpTelemetry<TData> : TelemetryBase<TData, UdpTelemetryConfig>
       // where TData : struct
    {

        public override bool IsConnected =>
            udpClient != null &&
            udpClient.Client != null &&
            udpClient.Client.IsBound;

        private static UdpClient udpClient;

        public event Action<IPEndPoint, TData> OnReceiveAsync;

        public UdpTelemetry(UdpTelemetryConfig config, IByteConverter<TData> converter) : base(config, converter)
        {
        }

        protected override void Configure(UdpTelemetryConfig config)
        {
            if (config.ReceiveAddress != null)
            {
                Log($"Create UdpClient: Receiving @ {config.ReceiveAddress.Address}: {config.ReceiveAddress.Port} with timeout of {Config.ReceiveTimeout} ms");
                udpClient = new UdpClient(config.ReceiveAddress);
            }
            else
            {
                Log("Create UdpClient");
                udpClient = new UdpClient();
            }

            if (config.SendAddress != null)
            {
                Log($"Create Send Adress {config.SendAddress.Address}: {config.SendAddress.Port} with timeout of {Config.ReceiveTimeout} ms");
            }

            udpClient.Client.ReceiveTimeout = Config.ReceiveTimeout;
        }

        public override TData Receive()
        {
            IPEndPoint remoteEp = null;
            var bytes = udpClient.Receive(ref remoteEp);

            var data = Converter.FromBytes(bytes);

            OnReceiveAsync?.Invoke(remoteEp, data);

            return data;

        }

        public override int Send(TData data)
        {
            var bytes = Converter.ToBytes(data);
            if (Config.SendAddress != null)
            {
                return udpClient.Send(bytes, bytes.Length, Config.SendAddress);
            }

            return 0;
        }

        public override void Dispose()
        {
            udpClient.Close();
        }

        public override Task<TData> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return Receive();
            }, cancellationToken);
        }

    }
}
