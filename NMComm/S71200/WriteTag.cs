namespace NMComm.S71200
{
    public class WriteTag
    {
        public PlcTag? ReadTag { get; private set; }
        public TagTypes TagType { get; set; }
        public double Value { get; set; }
        public int ByteAddr { get; set; }
        public int Bit { get; set; }

        public WriteTag(PlcTag rTag)
        {
            ReadTag = rTag;
        }

        public void ToDb(byte[] buf, int i0 = 0)
        {
            int i = ByteAddr - i0;
            int v = (int)Value;
            switch (TagType)
            {
                case TagTypes.Bool:
                    if (v == 0) buf[i] &= (byte)~(1 << Bit);
                    else buf[i] |= (byte)(1 << Bit);
                    break;
                case TagTypes.Int8:
                    buf[i] = (byte)Value;
                    break;
                case TagTypes.Int16:
                    buf[i] = (byte)(v >> 8);
                    buf[i + 1] = (byte)(v);
                    break;
                case TagTypes.Int32:
                    buf[i] = (byte)(v >> 24);
                    buf[i + 1] = (byte)(v >> 16);
                    buf[i + 2] = (byte)(v >> 8);
                    buf[i + 3] = (byte)v;
                    break;
                case TagTypes.Real:
                    Float2Buf(buf, (float)Value, i);
                    break;
                case TagTypes.LReal:
                    LFloat2Buf(buf, Value, i);
                    break;
            }
        }

        public static void Float2Buf(byte[] buf, float v, int offset)
        {
            byte[] data = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian) Array.Reverse(data);
            Array.Copy(data, 0, buf, offset, data.Length);
        }

        public static void LFloat2Buf(byte[] buf, double v, int offset)
        {
            byte[] data = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian) Array.Reverse(data);
            Array.Copy(data, 0, buf, offset, data.Length);
        }

    }
}
