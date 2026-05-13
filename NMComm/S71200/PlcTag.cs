namespace NMComm.S71200
{
    public enum TagTypes { Bool, Int8, Int16, Int32, Real, LReal }

    public class PlcTag: IComparable<PlcTag>
    {
        public string? Name { get; set; }
        public TagTypes TagType { get; protected set; }
        private double _val0;
        private double _val;
        public double Value { get { return _val; } set { _val = value; } }
        public int ByteAddr { get; set; }
        public int EndByteAddr { get; protected set; }
        public int Bit { get; set; }

        public PlcTag(string n) { Name = n; }

        /// <summary>
        /// </summary>
        /// <param name="t">Kiểu dữ liệu của Tag</param>
        /// <param name="b">Địa chỉ byte (theo PLC)</param>
        /// <param name="bit">Địa chỉ bit</param>
        public PlcTag(TagTypes t, int b, int bit = 0) { TagType = t; ByteAddr = b; Bit = bit; SetEndByteAddr(); }

        /// <param name="buf"></param>
        /// <param name="i0">Địa chỉ bắt đầu của bộ đệm buf trong DB</param>
        public void ParseDb(byte[] buf, int i0 = 0)
        {
            int i = ByteAddr - i0;
            switch (TagType)
            {
                case TagTypes.Bool:
                    Value = ((buf[i] >> Bit) & 1) == 1 ? 1 : 0;
                    break;
                case TagTypes.Int8:
                    Value = buf[i];
                    break;
                case TagTypes.Int16:
                    Value = buf[i] << 8 | buf[i + 1];
                    break;
                case TagTypes.Int32:
                    Value = buf[i] << 24 | buf[i + 1] << 16 | buf[i + 2] << 8 | buf[i + 3];
                    break;
                case TagTypes.Real:
                    Value = Buf2Float(buf, i);
                    break;
                case TagTypes.LReal:
                    Value = Buf2LFloat(buf, i);
                    break;
            }
        }

        public void ParseByte(byte b)
        {
            Value = ((b >> Bit) & 1) == 1 ? 1 : 0;
        }

        private void SetEndByteAddr()
        {
            switch (TagType)
            {
                case TagTypes.Bool:
                case TagTypes.Int8:
                    EndByteAddr = ByteAddr;
                    break;
                case TagTypes.Int16:
                    EndByteAddr = ByteAddr + 1;
                    break;
                case TagTypes.Int32:
                case TagTypes.Real:
                    EndByteAddr = ByteAddr + 3;
                    break;
                case TagTypes.LReal:
                    EndByteAddr = ByteAddr + 7;
                    break;
            }
        }

        public bool IsChanged() { return _val0 != _val; }
        public bool IsUpEdge() { return _val0 < 0.1 && _val > 0.9; }
        public bool IsDownEdge() { return _val0 > 0.9 && _val < 0.1; }
        public void MarkChanged() { _val0 = _val; }

        public static float Buf2Float(byte[] buf, int offset)
        {
            byte[] data = new byte[4];
            Array.Copy(buf, offset, data, 0, 4);
            if (BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        public static double Buf2LFloat(byte[] buf, int offset)
        {
            byte[] data = new byte[8];
            Array.Copy(buf, offset, data, 0, 8);
            if (BitConverter.IsLittleEndian) Array.Reverse(data);
            return BitConverter.ToDouble(data, 0);
        }

        public int CompareTo(PlcTag? other)
        {
            if (other == null) return -1;
            if (ByteAddr == other.ByteAddr)
            {
                return Bit.CompareTo(other.Bit);
            }
            return ByteAddr.CompareTo(other.ByteAddr);
        }
    }
}
