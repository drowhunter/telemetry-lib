namespace com.drowhunter.TelemetryLib
{
    /// <summary>
    /// Configuration settings for a Memory-Mapped File (MMF) telemetry channel.
    /// </summary>
    public class MmfTelemetryConfig
    {
        /// <summary>
        /// Gets or sets the name of the memory-mapped file.
        /// </summary>
        /// <value>Defaults to <c>"MmfTelemetry"</c>.</value>
        public string Name { get; set; } = "MmfTelemetry";

        /// <summary>
        /// Gets or sets a value indicating whether this instance should create the memory-mapped file.
        /// When <c>false</c>, the instance will attempt to open an existing file.
        /// </summary>
        /// <value>Defaults to <c>false</c>.</value>
        public bool Create { get; set; } = false;

        /// <summary>
        /// Gets or sets the name of the mutex used to synchronise access to the memory-mapped file.
        /// When <c>null</c>, no mutex synchronisation is used.
        /// </summary>
        /// <value>Defaults to <c>null</c>.</value>
        public string MutexName { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether the memory-mapped file should be created
        /// in the global namespace, making it accessible across user sessions (e.g. services).
        /// </summary>
        /// <value>Defaults to <c>false</c>.</value>
        public bool IsGlobal { get; set; } = false;

        /// <summary>
        /// Initialises a new instance of <see cref="MmfTelemetryConfig"/> with default values.
        /// </summary>
        public MmfTelemetryConfig() { }

        /// <summary>
        /// Initialises a new instance of <see cref="MmfTelemetryConfig"/> with the specified settings.
        /// </summary>
        /// <param name="name">The name of the memory-mapped file.</param>
        /// <param name="isCreator">
        /// <c>true</c> if this instance should create the memory-mapped file;
        /// <c>false</c> to open an existing one. Defaults to <c>false</c>.
        /// </param>
        /// <param name="isGlobal">
        /// <c>true</c> to create the file in the global namespace; otherwise <c>false</c>.
        /// Defaults to <c>false</c>.
        /// </param>
        public MmfTelemetryConfig(string name, bool isCreator = false, bool isGlobal = false)
        {
            Name = name; 
            Create = isCreator;
            IsGlobal = isGlobal;
        }
    }
    
}
