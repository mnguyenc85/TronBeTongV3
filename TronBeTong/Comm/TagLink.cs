using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm
{
    /// <summary>
    /// Lưu thông tin kết nối đến lớp truyền thông (S7-1200, KEPServer...)
    /// </summary>
    public class TagLink
    {
        public ModelTag Tag { get; set; }

        public PlcDb? S71200Db { get; set; }
        public PlcTag? S71200Tag { get; set; }

        public string? S7200Addr { get; set; }

        public TagLink(ModelTag mt, PlcDb? plcdb, PlcTag? plctag)
        {
            Tag = mt;
            S71200Db = plcdb; S71200Tag = plctag;
        }
        public TagLink(ModelTag mt, string s7200Addr)
        {
            Tag = mt;
            S7200Addr = s7200Addr;
        }

        public void WriteToS71200(double v, double delay)
        {
            if (S71200Db != null && S71200Tag != null)
            {
                DataType? dbtype = S71200Db.DbType switch
                {
                    PlcDbTypes.DB => DataType.DataBlock,
                    PlcDbTypes.M => DataType.Memory,
                    _ => null
                };
                if (dbtype != null)
                {
                    WriteBytesCmd cmd = new() { DbType = dbtype.Value, Delay = delay };
                    cmd.AddTag(S71200Tag, v);
                    S71200Db.AddWriteBytesCmd(cmd);
                }
            }

        }
    }
}
