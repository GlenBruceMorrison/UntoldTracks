public interface IInventoryController
{
    public Inventory Inventory { get; }
    public InventoryUI UI { get; }
    public PlayerInventoryController AccessedBy { get; }
    public IInventoryController Accessing { get; }
    public void Hide();
    public void Show();
}
