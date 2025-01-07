using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task4_2
{
    public class Orders: General
    {
        public Buyers buyer { get; }
        public Items item { get; }
        public int item_quantity { get; }
        public int total_value { get; set; }
        public DateTime date { get; }
        public Orders(int id, Buyers buyer, Items item, int item_quantity, DateTime date):base(id, null) {
            this.id = id; this.buyer = buyer; this.item = item; this.item_quantity = item_quantity; this.total_value = item.value * item_quantity;  this.date = date;
        }
    }
}
