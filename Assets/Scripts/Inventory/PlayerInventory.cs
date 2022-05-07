namespace Tracks.Inventory
{
    public class PlayerInventory : Inventory
    {
        private PlayerManager _player;
        public IInventory _accessing;

        public PlayerManager OwnedBy
        {
            get
            {
                return _player;
            }
        }

        public PlayerInventory(int size) : base(size)
        {

        }
    }
}
