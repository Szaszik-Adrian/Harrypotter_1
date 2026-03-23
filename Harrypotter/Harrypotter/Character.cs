using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrypotter
{
    public class Character
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string HogwartsHouse { get; set; }
        public string InterpretedBy { get; set; }
        public string Image { get; set; }
        public DateTime Birthdate { get; set; }

        public List<string> Children { get; set; }
        public List<string> KnownSpells { get; set; }

        public Character()
        {
            Children = new List<string>();
            KnownSpells = new List<string>();
        }

    }
}
