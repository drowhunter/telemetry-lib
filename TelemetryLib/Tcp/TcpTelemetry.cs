using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace com.drowhunter.TelemetryLib
{

    public class TcpTelemetry<TData> : TelemetryBase<TData, TcpTelemetryConfig> //where TData : struct
    {
        

        private static readonly System.Collections.Concurrent.ConcurrentBag<TcpClient> tcpClients = new();

        public event Action<object,TData> OnReceiveAsync;

        public override bool IsConnected => tcpClients.Any(c => c?.Connected ?? false);


        CancellationTokenSource cts;

        public TcpTelemetry(TcpTelemetryConfig config, IByteConverter<TData> converter) : base(config, converter)
        {
        }

        protected override void Configure(TcpTelemetryConfig config)
        {
            
            
            cts = new CancellationTokenSource();


            if (config.IpAddress != null)
            {
                Log($"Create TcpClient");
               
                Log($"Create Send Adress {config.IpAddress.Address}: {config.IpAddress.Port} with timeout of {Config.ReceiveTimeout} ms");
               
                //_ = Task.Factory.StartNew(async () =>
                new Thread(async () =>                
                {
                    while (!cts.IsCancellationRequested)
                    {
                        if (config.IsCaller)
                        {
                            tcpClients.Add(await WaitForConnectAsync(cts.Token));
                        }
                        else
                        {
                            tcpClients.Add(await WaitForCallAsync(cts.Token));
                        }
                    }
                })//, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                    .Start();
                
                
            }
            
        }

        public async Task<TcpClient> WaitForConnectAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(Config.IpAddress.Address, Config.IpAddress.Port);
            cancellationToken.ThrowIfCancellationRequested();

            return tcpClient;
        }

        
        public async Task<TcpClient> WaitForCallAsync(CancellationToken cancellationToken = default)
        {
            TcpClient client = null;
            var tcpListener = new TcpListener(Config.IpAddress);
            try 
            {
                

                tcpListener.Start();

                Log($"TCP Listener started on {tcpListener.LocalEndpoint}");

                client = await tcpListener.AcceptTcpClientAsync().WithCancellation(cancellationToken);

                Log($"TCP Client connected from {client.Client.RemoteEndPoint}");

                return client;

            }
            catch(OperationCanceledException)
            {
                Log("TCP Listener operation was canceled."); 
            }
            catch(SocketException ex)
            {
                Log($"Error starting TCP Listener: {ex.Message}");
            }
            finally
            {
                try
                {
                    tcpListener.Stop();
                }
                catch(Exception ex)
                {
                    Log($"Error stopping TCP Listener: {ex.Message}");
                }
            }

            return null;
        }

        private bool IsStruct<T>()
        {
            return typeof(T).IsValueType && !typeof(T).IsPrimitive && !typeof(T).IsEnum;
        }

        public new bool IsValueType<T>()
        {
            return typeof(T).IsValueType && typeof(T) != typeof(string) && !typeof(T).IsEnum;
        }

        public override TData Receive()
        {
            TData retval = default;

            if (!IsConnected)
            { 
                Thread.Sleep(1000);
                return default;
            }

            //IPEndPoint remoteEp = null;

            int size = GetBufferSizeOrDefault<TData>(1024);
            
            var buffer = new byte[size];


            RemoveDisconnectedClients();
            if (tcpClients.Count == 0)
            {
                Thread.Sleep(1000);
                return retval;
            }
            foreach (var tcpClient in tcpClients)
            {
                //var tcpClient = tcpClients.FirstOrDefault(c => c?.Client?.Connected ?? false);
                
                tcpClient.Client.Receive(buffer, SocketFlags.None);

                var data = Converter.FromBytes(buffer);

                OnReceiveAsync?.Invoke(tcpClient, data);

                retval = data;
                //break;
            }

            return retval;
        }

        private int RemoveDisconnectedClients()
        {
            List<TcpClient> toDelete = tcpClients.Where(c => !(c?.Client.Connected ?? false)).ToList();
            
            foreach (var tcpClient in toDelete)
            {
                try
                {
                    if (tcpClients.TryTake(out var removed))
                        removed?.Close();
                }
                catch (Exception ex)
                {
                    Log($"Error removing TcpClient: {ex.Message}");
                }
            }

            return toDelete.Count;
        }

        public override int Send(TData data)
        {
            int retval = 0;
            try
            {
                if (IsConnected && tcpClients.Count > 0)
                {
                    var bytes = Converter.ToBytes(data);

                    foreach (var tcpClient in tcpClients.Where(c => c?.Connected ?? false))
                    {
                       retval += tcpClient.Client.Send(bytes);
                    }         
                }
                else
                {
                    Thread.Sleep(1000);
                }

            }
            catch (SocketException)
            {

            }
            return retval;
        }

        public override void Dispose()
        {
            foreach (var tcpClient in tcpClients)
            {
                try
                {
                    tcpClient.Close();
                }
                catch (Exception ex)
                {
                    Log($"Error disposing TcpClient: {ex.Message}");
                }                
            }

            while (tcpClients.TryTake(out var removed))
            {
                try
                {
                    removed?.Close();
                }
                catch (Exception ex)
                {
                    Log($"Error disposing TcpClient: {ex.Message}");
                }
            }

            cts?.Cancel();
            cts?.Dispose();
        }

        public override async Task<int> SendAsync(TData data, CancellationToken cancellationToken = default)
        {
            int retval = 0;

            if (!IsConnected || tcpClients.Count == 0)
            {
                await Task.Delay(1000, cancellationToken);
                return retval;
            }
            foreach (var tcpClient in tcpClients.Where(c => c?.Connected ?? false))
            {
                var bytes = Converter.ToBytes(data);
                var stream = tcpClient.GetStream();
                await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
                retval += bytes.Length;
            }
            

            return retval;
        }


        public override async Task<TData> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            int size = GetBufferSizeOrDefault<TData>(1024);

            var buffer = new byte[size];
            TData retval = default;

            var connectedTCPClients = tcpClients.Where(c => c?.Connected ?? false).ToList();
            if (connectedTCPClients.Count == 0)
            {
                await Task.Delay(1000, cancellationToken);
                return retval;
            }

            foreach (var tcpClient in connectedTCPClients)
            {
                try
                {
                    var stream = tcpClient.GetStream();
                    var result = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (result > 0)
                    {
                        var trimmed = new byte[result];
                        Array.Copy(buffer, trimmed, result);
                        retval = Converter.FromBytes(trimmed);

                        if (retval != null)
                            OnReceiveAsync?.Invoke(tcpClient, retval);
                    }
                    break;
                }
                catch (OperationCanceledException)
                {
                    Log("ReceiveAsync operation was canceled.");
                }
                catch (Exception ex)
                {
                    Log($"An error occurred during ReceiveAsync: {ex.Message}");
                }
            }

            return retval;

        }

        private int GetBufferSizeOrDefault<T>(int defaultSize)
        {
            if (IsValueType<T>())
                return Marshal.SizeOf<T>();
            else
                return defaultSize;
        }
    }
}
