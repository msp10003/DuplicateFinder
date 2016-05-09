
//TODO make this a private class
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
