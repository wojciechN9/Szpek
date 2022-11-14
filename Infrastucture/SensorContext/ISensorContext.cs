namespace Szpek.Infrastructure.SensorContext
{
    /// <summary>
    /// Identifies device (sensor) that makes a request
    /// </summary>
    public interface ISensorContext
    {
        long SensorId { get; }
        
        string SensorCode { get; }
    }
}
