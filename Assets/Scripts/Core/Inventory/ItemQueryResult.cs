namespace UntoldTracks.InventorySystem
{
    public class ItemQueryResult
    {
        public Item item;
        public int amountAdded;
        public float durability;

        public ItemQueryResult(Item item, int count = 1, float durability=-1)
        {
            this.item = item;
            this.amountAdded = count;
            this.durability = durability;
        }
    }
}
