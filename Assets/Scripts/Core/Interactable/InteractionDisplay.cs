namespace UntoldTracks
{
    [System.Serializable]
    public class InteractionDisplay
    {
        public InteractionInput input;
        public string text;

        public InteractionDisplay(InteractionInput input, string text)
        {
            this.input = input;
            this.text = text;
        }
    }
}