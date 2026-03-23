using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrypotter
{
    public class Spell
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Use { get; set; }

        public Spell(int id, string name, string use)
        {
            Id = id;
            Name = name;
            Use = use;
        }

        public override string ToString()
        {
            return Name + " - " + Use;
        }
    }
}
