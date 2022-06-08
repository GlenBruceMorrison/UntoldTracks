using UntoldTracks.Models;

namespace UntoldTracks.InventorySystem
{
    public class ItemQueryResult
    {
        public ItemModel item;
        public int amountAdded;
        public float durability;

        public ItemQueryResult(ItemModel item, int count = 1, float durability=-1)
        {
            this.item = item;
            this.amountAdded = count;
            this.durability = durability;
        }
    }
}
