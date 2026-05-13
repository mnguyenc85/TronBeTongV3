namespace TronBeTongV3.View
{
    public class GlobalUIEvent
    {
        private GlobalUIEvent() { }
        private static GlobalUIEvent? _instance;

        public event EventHandler<GlobalUIEventArgs>? UIEventRaised;

        public static GlobalUIEvent Instance => _instance ??= new GlobalUIEvent();

        public void RaiseEvent(object? sender, GlobalUIEventKinds k, int id)
        {
            UIEventRaised?.Invoke(sender, new GlobalUIEventArgs() { Kind = k, ObjectId = id });
        }
        public void RaiseEvent(object? sender, GlobalUIEventKinds k, string s)
        {
            UIEventRaised?.Invoke(sender, new GlobalUIEventArgs() { Kind = k, Text = s });
        }
    }

    public enum GlobalUIEventKinds { CauHinhCotLieu, CauHinhPhuGia, CauHinhXiMang, CauHinhNuoc, DebugMsg }
    public class GlobalUIEventArgs: EventArgs
    {
        public GlobalUIEventKinds Kind { get; set; }
        public int ObjectId;
        public string? Text { get; set; }
    }
}
