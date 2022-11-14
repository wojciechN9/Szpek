using System.Collections.Generic;

namespace Szpek.Core.Models
{
    public class Sensor
    {
        public long Id { get; private set; }

        public string Code { get; private set; }

        public string PublicKey { get; private set; } // Secret - 32 bytes that are base64 encoded

        public bool IsPrivate { get; private set; }

        public long SensorOwnerId { get; private set; }

        public SensorOwner SensorOwner { get; private set; }

        public List<Location> Locations { get; private set; }

        public int? BoardId { get; set; }
        public Board Board { get; set; }

        public int? RecommendedFirmwareId { get; set; }
        public Firmware RecommendedFirmware { get; set; }

        public int? InstalledFirmwareId { get; set; }
        public Firmware InstalledFirmware { get; set; }

        public List<SensorLog> Logs { get; set; } = new List<SensorLog>();

        public static Sensor Create(string code, long ownerId, string publicKey, bool isPrivate)
        {
            return new Sensor(code, ownerId, publicKey, isPrivate);
        }

        //public for test try to change it to protected
        public Sensor() { }
        public Sensor(string code, List<Location> locations) 
        {
            this.Code = code;
            this.Locations = locations;
        }
        public Sensor(string publicKey) 
        {
            this.PublicKey = publicKey;
        }

        private Sensor(string code, long ownerId, string publicKey, bool isPrivate)
        {
            this.Code = code;
            this.SensorOwnerId = ownerId;
            this.PublicKey = publicKey;
            this.IsPrivate = isPrivate;
        }


        public void SetPrivacy(bool isPrivate)
        {
            IsPrivate = isPrivate;
        }
    }
}
