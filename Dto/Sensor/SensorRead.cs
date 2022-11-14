namespace Szpek.Application.Sensor
{
    public class SensorRead
    {
        public SensorRead(long id, string code, bool isPrivate, long? activeLocationId, OwnerRead owner)
        {
            Id = id;
            Code = code;
            IsPrivate = isPrivate;
            ActiveLocationId = activeLocationId;
            Owner = owner;            
        }

        public long Id { get; }

        public string Code { get; }

        public bool IsPrivate { get; set; }

        public long? ActiveLocationId { get; set; }

        public OwnerRead Owner { get; set; }
    }
}
