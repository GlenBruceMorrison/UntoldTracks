using UntoldTracks.Models;

namespace UntoldTracks.InventorySystem
{
    public class ItemQuery
    {
        public ItemModel item;
        public int count;
        public int preferredIndex;

        public ItemQuery(ItemModel item, int count = 1, int preferredIndex = -1)
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
