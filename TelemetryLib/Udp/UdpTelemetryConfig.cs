using System.Net;

namespace com.drowhunter.TelemetryLib
{
    public class UdpTelemetryConfig
    {
        public IPEndPoint SendAddress { get; set; }

        public IPEndPoint ReceiveAddress { get; set; }

        public int ReceiveTimeout { get; set; } = 0;

        public UdpTelemetryConfig()
        {

        }

        /// <summary>
        /// Configure the UDP plugin with send and receive addresses and ports.
        /// </summary>
        /// <param name="sendAddress">ipaddress:port</param>
        /// <param name="receiveAddress">ipaddress:port</param>
        public UdpTelemetryConfig(string sendAddress = null, string receiveAddress = null)
        {
            SendAddress = ParseAddressAndPort(sendAddress);
            ReceiveAddress = ParseAddressAndPort(receiveAddress);
        }

        /// <summary>
        /// Configure the UDP plugin with send and receive addresses and ports.
        /// </summary>
        /// <param name="sendAddress">send address</param>
        /// <param name="receiveAddress">receive address</param>
        public UdpTelemetryConfig(IPEndPoint sendAddress = null, IPEndPoint receiveAddress = null)
        {
            SendAddress = sendAddress;
            ReceiveAddress = receiveAddress;
        }

        private IPEndPoint ParseAddressAndPort(string address)
        {
            if (string.IsNullOrEmpty(address) || address.Trim().Length == 0)
                return null;

            var parts = address.Split(':');
            if (parts.Length != 2)
                throw new ArgumentException("Invalid address format. Expected format: ipaddress:port");

            var ip = IPAddress.Parse(parts[0]);
            var port = int.Parse(parts[1]);
            return new IPEndPoint(ip, port);
        }
    }
}
