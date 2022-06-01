namespace UntoldTracks
{
    [System.Serializable]
    public class InteractionDisplay
    {
        public ItemList validItems;
        public InteractionInput input;
        public string text;

        public InteractionDisplay(InteractionInput input, string text, ItemList validItems=null)
        {
            this.input = input;
            this.text = text;
            this.validItems = validItems;
        }
    }
}