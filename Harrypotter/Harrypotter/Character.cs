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

        public List<Child> Children { get; set; }
        public List<Spell> KnownSpells { get; set; }

        public Character()
        {
            Children = new List<Child>();
            KnownSpells = new List<Spell>();
        }

    }
}
