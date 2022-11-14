using System;
using System.Collections.Generic;
using System.Text;

namespace Szpek.Core.Models
{
    public class Firmware
    {
        public int Id { get; set; }

        public string Name { get; private set; }

        public byte[] Content { get; private set; }

        public DateTime ReleaseDateTimeUtc { get; private set; }

        public Firmware(string name, byte[] content, DateTime releaseDateTimeUtc)
        {
            Name = name;
            Content = content;
            ReleaseDateTimeUtc = releaseDateTimeUtc;
        }
    }
}
