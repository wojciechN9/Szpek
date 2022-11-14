namespace Szpek.Application.Meassurement
{
    public class SensorMeassurementDataModel
    {
        public string Payload { get; set; } // base64 encoded
        public string Signature { get; set; } // base64 encoded
    }
}
