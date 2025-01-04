using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    internal class General
    {
        public int id { get; set; }
        public string name { get; set; }

        public General(int id, string name)
        {
            this.id = id; this.name = name;
        }

    }
}
