namespace TronBeTongV3.CSDL.Server
{
    public class TableInfo(string tblname)
    {
        public int Index { get; set; }
        public string TableName { get; set; } = tblname;
        public List<ColumnInfo> Columns { get; private set; } = [];

        public string? SelectFields { get; private set; }
        public void Build()
        {
            List<string> colnames = [];
            foreach (var col in Columns)
            {
                colnames.Add(col.ColName);
                if (col.ColName == "id") col.TargetCol = "local_id";
                else if (col.ColName == "updated_at") col.TargetCol = "local_updated_at";
                else if (col.ColName == "created_at") col.TargetCol = "local_created_at";
                else col.TargetCol = col.ColName;
            }
            SelectFields = string.Join(",", colnames);
        }
    }

    public class ColumnInfo(string colName, string colType, string nullable, string defVal, string extra)
    {
        public string ColName { get; set; } = colName;
        public string ColType { get; set; } = colType;
        public string Nullable { get; set; } = nullable;
        public string DefaultValue { get; set; } = defVal;
        public string Extra { get; set; } = extra;

        public string? TargetCol { get; set; }
    }
}
