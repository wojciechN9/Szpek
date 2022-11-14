namespace Szpek.Application.Sensor
{
    public class OwnerRead
    {
        public OwnerRead(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; set; }

        public string Name { get; set; }
    }
}
