using NMComm.S71200;

namespace TronBeTongV3.Comm
{
    public enum ModelTagTypes { None, Bool, Byte, Word, Short, Long, DWord, Float, Double }

    public class ModelTag
    {
        public TagLink? Link { get; set; }

        public string Name { get; set; }

        public ModelTagTypes ValueType { get; set; } = ModelTagTypes.None;

        /// <summary>
        /// Giá trị hiện tại
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Giá trị do người sử dụng nhập
        /// </summary>
        public double WriteValue { get; set; }

        public long LastUpdated { get; set; }

        public bool IsChanged { get; set; } = false;

        public ModelTag(string name) { Name = name; }

        public bool GetBool() { return Value > 0; }

        public bool SetValue(double v)
        {
            IsChanged = v != Value;
            Value = v;
            LastUpdated = DateTime.Now.Ticks;
            return IsChanged;
        }
        public bool SetValue(double v, long t)
        {
            IsChanged = v != Value;
            Value = v;
            LastUpdated = t;
            return IsChanged;
        }

        public void ConvertType(TagTypes t)
        {
            switch (t)
            {
                case TagTypes.Bool:
                    ValueType = ModelTagTypes.Bool;
                    break;
                case TagTypes.Int8:
                    ValueType = ModelTagTypes.Byte;
                    break;
                case TagTypes.Int16:
                    ValueType = ModelTagTypes.Word;
                    break;
                case TagTypes.Int32:
                    ValueType = ModelTagTypes.DWord;
                    break;
                case TagTypes.Real:
                    ValueType = ModelTagTypes.Float;
                    break;
                case TagTypes.LReal:
                    ValueType = ModelTagTypes.Double;
                    break;
                default:
                    break;
            }
        }

        public object ConvertValue(double v)
        {
            return ValueType switch
            {
                ModelTagTypes.Bool => v > 0,
                ModelTagTypes.Byte => (byte)v,
                ModelTagTypes.Word => (ushort)v,
                ModelTagTypes.DWord => (ulong)v,
                ModelTagTypes.Short => (short)v,
                ModelTagTypes.Long => (long)v,
                ModelTagTypes.Float => (float)v,
                _ => v,
            };
        }
    }
}
