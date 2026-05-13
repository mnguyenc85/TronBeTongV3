using System.Text;
using TronBeTongV3.Core;

namespace TronBeTongV3.Debugger
{
    public class TagLine
    {
        private double _x1;
        public string? Name { get; set; }
        public List<Vector2d> DataPoints { get; private set; } = [];

        public TagLine(string name) { Name = name; }

        public void Reset()
        {
            DataPoints.Clear();
            _x1 = 0;
        }

        public void AddPoint(double x, double y)
        {
            if (x > _x1)
            {
                DataPoints.Add(new Vector2d(x, y));
                _x1 = x;
            }
        }

        public void ExportText(StringBuilder sb)
        {
            sb.AppendFormat("# {0}\r\n", Name);
            foreach (Vector2d p in DataPoints)
            {
                sb.AppendLine($"{p.X}, {p.Y}");
            }
            sb.AppendLine();
        }
    }
}
