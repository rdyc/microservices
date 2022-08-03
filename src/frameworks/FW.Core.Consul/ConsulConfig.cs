namespace FW.Core.Consul;

public class ConsulConfig
{
    public string Address { get; set; } = "http://localhost:8500";
    public string ServiceName { get; set; } = "my-service";
    public string ServiceId { get; set; } = "my-service-1";
    public bool EnableCheck { get; set; } = false;
    public string[] Tags { get; set; } = default!;
}