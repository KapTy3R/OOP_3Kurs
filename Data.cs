using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    internal abstract class Data
    {
        public abstract void Read(ref List<Buyers> buyers, ref List<Items> items, ref List<Orders> orders);
        public abstract void Write(List<Buyers> buyers, List<Items> items, List<Orders> orders);
    }
}
