namespace com.drowhunter.TelemetryLib
{
    public class MmfTelemetryConfig
    {
        public string Name { get; set; } = "MmfTelemetry";

        public bool Create { get; set; } = false;

        public string MutexName { get; set; } = null;

        public bool IsGlobal { get; set; } = false;

        public MmfTelemetryConfig() { }

        public MmfTelemetryConfig(string name, bool isCreator = false, bool isGlobal = false)
        {
            Name = name; 
            Create = isCreator;
            IsGlobal = IsGlobal;
        }
    }
}
