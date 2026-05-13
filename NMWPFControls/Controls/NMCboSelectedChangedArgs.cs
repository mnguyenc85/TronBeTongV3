namespace NMWPFControls.Controls
{
    public class NMCboSelectedChangedArgs: EventArgs
    {
        public object? Selected {  get; set; }
        /// <summary>
        /// Ấn Enter hoặc Left Mouse
        /// </summary>
        public bool IsDecided { get; set; }
    }
}
