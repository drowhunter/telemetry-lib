# TelemetryLib

A .NET library for sending and receiving telemetry data over multiple transports: **Memory Mapped Files (MMF)**, **UDP**, and **TCP**. Data serialization is handled by pluggable byte converters.

- **Namespace**: `Drowhunter.TelemetryLib`
- **Target Framework**: .NET 10.0

---

## Transports

| Class | Config | Description |
|---|---|---|
| `MmfTelemetry<TData>` | `MmfTelemetryConfig` | Windows shared memory via Memory Mapped Files. `TData` must be a `struct`. |
| `UdpTelemetry<TData>` | `UdpTelemetryConfig` | UDP send/receive. |
| `TcpTelemetry<TData>` | `TcpTelemetryConfig` | TCP caller/listener with automatic reconnect. |

All transports implement `ITelemetry<TData, TConfig>` and extend `TelemetryBase<TData, TConfig>`.

---

## Byte Converters

A converter (`IByteConverter<T>`) handles serialization between `TData` and `byte[]`.

| Class | Description |
|---|---|
| `MarshalByteConverter<T>` | Marshal/unmanaged struct layout. `T` must be a `struct`. |
| `JsonByteConverter<T>` | UTF-8 JSON via `System.Text.Json`. |
| `StringByteConverter` | String with configurable `Encoding`. |
| `RawBytesConverter` | Pass-through for `byte[]`. |

---

## Usage

### Memory Mapped File

```csharp
// Creator (writer)
var writer = new MmfTelemetry<MyStruct>(
    new MmfTelemetryConfig("MyMap", isCreator: true),
    new MarshalByteConverter<MyStruct>());

writer.Send(new MyStruct { Value = 42 });

// Reader
var reader = new MmfTelemetry<MyStruct>(
    new MmfTelemetryConfig("MyMap"),
    new MarshalByteConverter<MyStruct>());

MyStruct data = reader.Receive();
```

### UDP

```csharp
var sender = new UdpTelemetry<string>(
    new UdpTelemetryConfig(sendAddress: "127.0.0.1:9000"),
    new StringByteConverter(Encoding.UTF8));

sender.Send("hello");

var receiver = new UdpTelemetry<string>(
    new UdpTelemetryConfig(receiveAddress: "0.0.0.0:9000"),
    new StringByteConverter(Encoding.UTF8));

string msg = receiver.Receive();
```

### TCP

```csharp
// Caller (connects to listener)
var caller = new TcpTelemetry<MyStruct>(
    new TcpTelemetryConfig("127.0.0.1:5000") { IsCaller = true },
    new MarshalByteConverter<MyStruct>());

// Listener (waits for incoming connection)
var listener = new TcpTelemetry<MyStruct>(
    new TcpTelemetryConfig("0.0.0.0:5000") { IsCaller = false },
    new MarshalByteConverter<MyStruct>());
```

### Async receive loop

```csharp
using var cts = new CancellationTokenSource();
await telemetry.BeginAsync(cts.Token);
```

### Factories

Use the factory classes to share instances across consumers (keyed by consumer type):

```csharp
var factory = new UdpTelemetryFactory();
var udp = factory.GetOrCreate<MyConsumer, MyStruct>(
    new MarshalByteConverter<MyStruct>(),
    cfg => { cfg.SendAddress = new IPEndPoint(IPAddress.Loopback, 9000); });
```

Factories exist for all three transports: `MmfTelemetryFactory`, `UdpTelemetryFactory`, `TcpTelemetryFactory`.

---

## Events

```csharp
telemetry.OnLog += (sender, message) => Console.WriteLine(message);
```

---

## Notes

- `MmfTelemetry` is Windows-only (uses `advapi32` for security descriptor management).
- `MmfTelemetry` supports an optional named mutex (`MmfTelemetryConfig.MutexName`) for cross-process synchronization.
- `TcpTelemetry` runs reconnect logic on a background thread and supports multiple simultaneous clients.
