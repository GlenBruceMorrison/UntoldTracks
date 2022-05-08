namespace Tracks.Resource
{
    [System.Serializable]
    public class ResourceContainer
    {
        public Item item;
        public float rarity = 1;
        public int baseAmount = 1;
        public bool canProductMultiple = false;
        public int maxPossible = 1;
        public float multipleChance = 1;
    }
}