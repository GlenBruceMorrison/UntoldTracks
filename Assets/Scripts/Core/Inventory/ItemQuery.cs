namespace UntoldTracks.Inventory
{
    public class ItemQuery
    {
        public Item item;
        public int count;
        public int preferredIndex;

        public ItemQuery(Item item, int count = 1, int preferredIndex = -1)
        {
            this.item = item;
            this.count = count;
            this.preferredIndex = preferredIndex;
        }

        public ItemQuery(ItemContainer container)
        {
            this.item = container.Item;
            this.count = container.Count;
        }
    }
}
