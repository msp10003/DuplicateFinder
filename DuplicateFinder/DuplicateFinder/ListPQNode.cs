using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    class ListPQNode<Item>
    {
        public ListPQNode<Item> next;
        public ListPQNode<Item> prev;
        private Item item;

        public ListPQNode(Item i)
        {
            item = i;
        }

        public Item getValue()
        {
            return item;
        }
    }
}
