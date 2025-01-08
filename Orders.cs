using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public class Orders : General
    {
        public Buyers buyer { get; }
        public List<(Items Item, int Quantity)> items { get; set; }
        public int items_quantity { get; }
        public int total_value { get; set; }
        public DateTime date { get; }
        public Orders(int id, Buyers buyer, List<(Items Item, int Quantity)> items, DateTime date) : base(id, null)
        {
            this.id = id; this.buyer = buyer; this.items = items;
            int sum = 0;
            foreach (var item in items)
            {
                sum += item.Item.value * item.Quantity;
                items_quantity += item.Quantity;
            }
            this.total_value = sum; this.date = date;
        }
    }
}
