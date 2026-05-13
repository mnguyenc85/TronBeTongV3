namespace NMWPFControls.Controls
{
    public class MessageDO
    {
        public DateTime T { get; set; }
        public string? Text {  get; set; }

        public MessageDO(string text) { 
            Text = text;
            T = DateTime.Now;
        }
    }
}
