using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    internal class Items: General
    {
        public int value { get;}
        public string description { get;}
        public bool check { get; }

        public Items(int id, string name, int value, string description, bool check) : base (id, name)
        {
            this.id = id; this.name = name; this.value = value; this.description = description; this.check = check;
        }
    }
}
