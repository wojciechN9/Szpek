using System;
using System.Collections.Generic;
using System.Text;

namespace Szpek.Core.Models
{
    public class Board
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Board(string name)
        {
            Name = name;
        }
    }
}
