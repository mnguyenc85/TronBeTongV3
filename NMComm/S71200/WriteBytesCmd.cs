using S7.Net;

namespace NMComm.S71200
{
    public class WriteBytesCmd
    {
        private readonly List<WriteTag> _tags = new();
        private int _addr0 = -1;
        private int _addr1 = -1;
        private int _n;
        public DataType DbType { get; set; } = DataType.DataBlock;
        /// <summary>
        /// Thời gian trễ (s)
        /// </summary>
        public double Delay { get; set; }

        public WriteBytesCmd() { }

        public void AddTag(PlcTag tag, double val)
        {
            if (_addr0 < 0 || _addr0 > tag.ByteAddr) _addr0 = tag.ByteAddr;
            if (_addr1 < 0 || _addr1 < tag.EndByteAddr) _addr1 = tag.EndByteAddr;
            _n = _addr1 - _addr0 + 1;

            _tags.Add(new WriteTag(tag) { TagType = tag.TagType, ByteAddr = tag.ByteAddr, Bit = tag.Bit, Value = val });
        }

        /// <summary>
        /// Write cmd to PLC
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="db">Db number</param>
        /// <param name="readBuf">Read Buffer</param>
        /// <param name="startByteAddr">Start byte address of read buffer</param>
        /// <param name="autoWriteToReadTag">Write value to Read Tag</param>
        public async Task WriteAsync(Plc plc, int db, byte[] readBuf, int startByteAddr, bool autoWriteToReadTag = true)
        {
            int i0 = _addr0 - startByteAddr;
            byte[] _buf = new byte[_n];         // [_addr0 .. _addr1]

            Array.Copy(readBuf, i0, _buf, 0, _n);

            foreach (var tag in _tags)
            {
                tag.ToDb(_buf, _addr0);
            }

            System.Diagnostics.Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Try write to Db{db} type={DbType} start={_addr0}, len={_n}");

            await plc.WriteBytesAsync(DbType, db, _addr0, _buf);
        }

        public string Debug()
        {
            return $"Start={_addr0}, Len={_n}";
        }
    }
}
