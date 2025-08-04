namespace RpiMon.Models;

public class StaticSystemInfo
{
    public string OperatingSystem { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string RamSize { get; set; } = string.Empty;
    public string CpuInfo { get; set; } = string.Empty;
    public int CpuCoreCount { get; set; }
}