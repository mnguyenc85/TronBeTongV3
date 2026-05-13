namespace TronBeTongV3.CSDL
{
    public enum SettingValueTypes { String = 0, Number = 1, Boolean = 2 }

    public class SettingValueDO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SettingValueTypes ValType { get; set; }
        private string? _val;
        public string? Value { get { return _val; } set { if (_val != value) { _val = value; Changed = true; } } }

        public bool Changed { get; set; }

        public SettingValueDO(string n) { Name = n; }
    }
}
