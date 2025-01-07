using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task4_2
{
    public class Buyers: General
    {
        
        public string address { get; }
        public string tel { get; set; }
        public string person { get; }

        public Buyers(int id, string name, string address, string tel, string person): base(id, name)
        {
            this.id = id; this.name = name; this.address = address; this.tel = tel; this.person = person;
        }

        public override string ToString()
        {
            return $"{id}: {name}";
        }
    }
}
