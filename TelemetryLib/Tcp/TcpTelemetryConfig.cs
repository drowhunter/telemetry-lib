using System.Net;

namespace com.drowhunter.TelemetryLib
{
    public class TcpTelemetryConfig
    {
        /// <summary>
        /// Set to true if you are the caller otherwise set to false if you are the receiver.
        /// </summary>
        public bool IsCaller { get; set; }

        public IPEndPoint IpAddress { get; set; }

        public int ReceiveTimeout { get; set; } = 0;

        public TcpTelemetryConfig()
        {

        }

        /// <summary>
        /// Configure the TCP plugin with send and receive addresses and ports.
        /// </summary>
        /// <param name="sendAddress">ipaddress:port</param>
        /// <param name="receiveAddress">ipaddress:port</param>      
        public TcpTelemetryConfig(string sendAddress = null)
        {
            IpAddress = ParseAddressAndPort(sendAddress);
        }

        /// <summary>
        /// Configure the TCP plugin with send and receive addresses and ports.
        /// </summary>
        /// <param name="sendAddress">send address</param>
        /// <param name="receiveAddress">receive address</param>      
        public TcpTelemetryConfig(IPEndPoint sendAddress = null)
        {
            IpAddress = sendAddress;
        }

        private IPEndPoint ParseAddressAndPort(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
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
