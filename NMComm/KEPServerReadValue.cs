namespace NMComm
{
    public class KEPServerReadValue
    {
        public double Value { get; set; }
        public bool IsGood { get; set; }

        public KEPServerReadValue() { }

        public KEPServerReadValue(object v) { SetValue(v); }

        public void SetValue(object v)
        {
            var cv = ConvertValue(v);
            if (cv != null)
            {
                Value = cv.Value;
                IsGood = true;
            }
            else
            {
                IsGood = false;
            }
        }

        private double? ConvertValue(object value)
        {
            if (value == null) return null;

            switch (value)
            {
                case bool b:
                    return b ? 1.0 : 0.0;

                case sbyte sb:
                    return (double)sb;
                case byte by:
                    return (double)by;
                case short sh:
                    return (double)sh;
                case ushort ush:
                    return (double)ush;
                case int i:
                    return (double)i;
                case uint ui:
                    return (double)ui;
                case long l:
                    return (double)l;
                case ulong ul:
                    return (double)ul;
                case float f:
                    return (double)f;
                case double d:
                    return d;
                case decimal dec:
                    return (double)dec;

                default:
                    // Nếu là string, array hoặc type khác → bỏ qua
                    return null;
            }
        }
    }
}
