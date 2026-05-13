namespace DebuggerViewer.Data
{
    public class DataLine
    {
        public int Type { get; set; }

        public List<Vector2d> DataPoints { get; set; } = [];
    }
}
