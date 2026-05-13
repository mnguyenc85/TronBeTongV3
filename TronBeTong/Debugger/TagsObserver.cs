using System.Text;
using System.Windows;
using TronBeTongV3.Comm;

namespace TronBeTongV3.Debugger
{
    public class TagsObserver
    {
        private long _t0;
        private readonly StringBuilder _sb = new();

        private ModelHeThong _ht;
        public ModelHeThong HeThong { get { return _ht; } }

        public Dictionary<string, TagLine> Lines { get; private set; } = [];

        public TagsObserver(ModelHeThong ht) { _ht = ht; }

        public bool CanObserve { get; set; }

        public bool AddLine(string name)
        {
            if (Lines.Count < 32)
            {
                if (Lines.ContainsKey(name)) return false;
                Lines.Add(name, new TagLine(name));
                return true;
            }
            return false;
        }

        public bool RemoveLine(string name)
        {
            return Lines.Remove(name);
        }

        public void CollectData() {
            if (!CanObserve) return;
            foreach (var (k,line) in Lines)
            {
                if (_ht.AllLinks.TryGetValue(k, out TagLink? l))
                {
                    line.AddPoint((l.Tag.LastUpdated - _t0) / 10000000d, l.Tag.Value);
                }
            }
        }

        public void Start()
        {
            _t0 = DateTime.Now.Ticks;
            foreach (var (k, line) in Lines)
            {
                line.Reset();
            }
            CanObserve = true;
        }

        public void Stop()
        {
            CanObserve = false;
        }

        public void ExportText()
        {
            _sb.Clear();

            foreach (var (k,line) in Lines)
            {
                line.ExportText(_sb);
            }

            Clipboard.SetText(_sb.ToString());
        }
    }   
}
