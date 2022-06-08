using UntoldTracks.Models;

namespace UntoldTracks
{
    [System.Serializable]
    public class InteractionDisplay
    {
        public ItemListModel validItems;
        public InteractionInput input;
        public string text;

        public InteractionDisplay(InteractionInput input, string text, ItemListModel validItems=null)
        {
            this.input = input;
            this.text = text;
            this.validItems = validItems;
        }
    }
}