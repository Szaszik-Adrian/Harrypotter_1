using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrypotter
{
    public class Child
    {
        public string Name { get; set; }

        public Child(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
