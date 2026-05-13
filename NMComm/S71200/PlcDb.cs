using S7.Net;

namespace NMComm.S71200
{
    public enum PlcDbTypes { I, Q, IQ, M, DB }

    public abstract class PlcDb
    {
        protected int _dbNo = 0;
        public int DbNo { get { return _dbNo; } }
        protected int StartByteAddr = 0;
        protected byte[] _buf;
        protected List<WriteBytesCmd> WriteCmds = new();
        private bool _isWriting = false;

        /// <summary>
        /// Thời điểm đọc được dữ liệu (ticks)
        /// </summary>
        public long T { get; set; }
        /// <summary>
        /// Đang tách dữ liệu từ bộ đệm
        /// </summary>
        public bool IsParsingData { get; protected set; }
        /// <summary>
        /// Chỉ đọc trước khi ghi lệnh
        /// </summary>
        public bool ReadBeforeWriteOnly { get; set; }
        /// <summary>
        /// Ép đọc
        /// </summary>
        public bool ForceRead { get; set; } = true;
        /// <summary>
        /// Chu kỳ đọc (s). Không đọc nếu ≤ 0
        /// </summary>
        public double Cycle { get; set; } = 0.1;
        /// <summary>
        /// Giá trị trễ hiện tại. Dùng với Cycle
        /// </summary>
        protected double _delay;

        /// <summary>
        /// Thời điểm update view (mốc từ lúc bắt đầu giao tiếp). Cập nhật từ view (MainWindow)
        /// </summary>
        public long UpdateViewT { get; set; }

        public Dictionary<string, PlcTag> Tags { get; private set; } = new();

        public event EventHandler<WriteBytesCmd>? CommandWrote;

        public int NoWriteCms { get { return WriteCmds.Count; } }

        /// <summary>
        /// Chỉ để tạo địa chỉ tag
        /// </summary>
        public PlcDbTypes DbType { get; set; } = PlcDbTypes.DB;

        /// <summary>
        /// </summary>
        /// <param name="db">Số thứ tự db</param>
        /// <param name="buf_sz">Kích thước bộ đệm (số byte đọc tối đa)</param>
        /// <param name="startAddr">Địa chỉ trong db từ đó bắt đầu đọc</param>
        public PlcDb(int db, int buf_sz, int startAddr) { _dbNo = db; _buf = new byte[buf_sz]; StartByteAddr = startAddr; }

        /// <summary>
        /// Kiểm tra xem có cần đọc dữ liệu ko?
        /// </summary>
        /// <param name="delta">Khoảng thời gian từ lần giao tiếp trước (s)</param>
        /// <returns>Trả về true nếu có thể đọc</returns>
        public bool NeedRead(double delta)
        {
            if (ReadBeforeWriteOnly)
            {
                if (WriteCmds.Count > 0) return true;
            }
            else
            {
                if (Cycle > 0)
                {
                    _delay -= delta;
                    if (_delay <= 0)
                    {
                        _delay += Cycle;
                        return true;
                    }
                }
            }
            if (ForceRead) { ForceRead = false; return true; }
            return false;
        }

        /// <summary>
        /// Đọc giá trị
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="delta">Khoảng thời gian từ lần giao tiếp trước (s)</param>
        /// <returns></returns>
        public abstract Task ReadAsync(Plc plc, double delta);

        /// <summary>
        /// Ghi giá trị
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="delta">Khoảng thời gian từ lần giao tiếp trước (s)</param>
        /// <returns></returns>
        public virtual async Task<int> WriteAsync(Plc plc, double delta)
        {
            if (_isWriting) return 0;

            _isWriting = true;
            int i = 0;
            int n = 0;
            while (i < WriteCmds.Count)
            {
                var cmd = WriteCmds[i];
                cmd.Delay -= delta;
                if (cmd.Delay <= 0)
                {
                    await cmd.WriteAsync(plc, _dbNo, _buf, StartByteAddr);
                    WriteCmds.RemoveAt(i);
                    CommandWrote?.Invoke(this, cmd);
                    n++;
                }
                else i++;
            }
            _isWriting = false;
            return n;
        }

        public void AddTag(string name, TagTypes t, int byteAddr, int bitAddr)
        {
            PlcTag tag = new(t, byteAddr, bitAddr) { Name = name };
            Tags.Add(name, tag);
        }

        public bool GetBool(string n)
        {
            return Tags.ContainsKey(n) ? Tags[n].Value > 0 : false;
        }

        /// <summary>
        /// Maybe generate BUGS
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public double GetDouble(string n)
        {
            return Tags.ContainsKey(n) ? Tags[n].Value : 0;
        }

        public virtual void AddWriteBytesCmd(WriteBytesCmd writeCmd) { 
            WriteCmds.Add(writeCmd); 
        }
        public void ClearWriteCmds() { WriteCmds.Clear(); }

        public override string ToString()
        {
            return $"{_dbNo}, {StartByteAddr}, {_buf.Length}";
        }
    }
}
