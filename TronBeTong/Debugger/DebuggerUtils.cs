using System.Text;
using NMComm.S71200;

namespace TronBeTongV3.Debugger
{
    public class DebuggerUtils
    {
        private StringBuilder _sb = new();

        public string GetS71200Addr(PlcDb? db, PlcTag? tag)
        {
            if (tag == null) return "";

            _sb.Clear();
            if (db != null) {
                switch (db.DbType) {
                    case PlcDbTypes.DB:
                        _sb.Append($"DB{db.DbNo}.");
                        switch (tag.TagType)
                        {
                            case TagTypes.Bool:
                                _sb.Append($"DBX{tag.ByteAddr:0000}.{tag.Bit}");
                                break;
                            case TagTypes.Int8:
                                _sb.Append($"DBB{tag.ByteAddr:0000}");
                                break;
                            case TagTypes.Int16:
                                _sb.Append($"DBW{tag.ByteAddr:0000}");
                                break;
                            case TagTypes.Real:
                            case TagTypes.Int32:
                                _sb.Append($"DBD{tag.ByteAddr:0000}");
                                break;
                        }
                        break;
                    case PlcDbTypes.M:
                        switch (tag.TagType)
                        {
                            case TagTypes.Bool:
                                _sb.Append($"M{tag.ByteAddr:0000}.{tag.Bit}");
                                break;
                            case TagTypes.Int8:
                                _sb.Append($"MB{tag.ByteAddr:0000}");
                                break;
                            case TagTypes.Int16:
                                _sb.Append($"MW{tag.ByteAddr:0000}");
                                break;
                            case TagTypes.Real:
                            case TagTypes.Int32:
                                _sb.Append($"MD{tag.ByteAddr:0000}");
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                _sb.Append("?.");
                switch (tag.TagType)
                {
                    case TagTypes.Bool:
                        _sb.Append($"X{tag.ByteAddr:0000}{tag.Bit}");
                        break;
                    case TagTypes.Int8:
                        _sb.Append($"B{tag.ByteAddr:0000}");
                        break;
                    case TagTypes.Int16:
                        _sb.Append($"W{tag.ByteAddr:0000}");
                        break;
                    case TagTypes.Real:
                    case TagTypes.Int32:
                        _sb.Append($"D{tag.ByteAddr:0000}");
                        break;
                }
            }

            return _sb.ToString();
        }
    }
}
