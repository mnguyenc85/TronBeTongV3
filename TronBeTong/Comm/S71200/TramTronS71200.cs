using NMComm.S71200;
using S7.Net;
using System.Reflection;
using System.Text;

namespace TronBeTongV3.Comm.S71200
{
    public class TramTronS71200: S71200Communicator
    {
        #region Cấp phối
        public Db09_MeDat Db09MeDat { get; private set; } = new Db09_MeDat();
        public Db43_RTVAR Db43CP { get; private set; } = new Db43_RTVAR();

        public Db43_KLMeThuc Db43KLMe { get; private set; } = new Db43_KLMeThuc();
        #endregion

        #region Thùng cân
        public Db26_WIs Db26WIs { get; private set; } = new();
        public Db26_WIs2 Db26WIs2 { get; private set; } = new();
        #endregion

        #region TG trộn
        public Db29_SetMixerTime Db29SetMixerTime { get; private set; } = new();

        /// <summary>
        /// Thời gian cốt liệu trung gian
        /// </summary>
        //public Db42_ReadTG Db42ReadTG { get; private set; } = new();
        #endregion

        #region M
        public DbMemory11 M11 { get; private set; } = new();
        public DbMemory100 M100 { get; private set; } = new();
        public DbMemory200 M200 { get; private set; } = new();
        public DbMemory1000 M1000 { get; private set; } = new();
        #endregion

        #region Tham số
        public Db26_ThamSo Db26ThamSo { get; private set; } = new();
        public Db29_ThamSo Db29ThamSo { get; private set; } = new();        
        public Db26_Calib Db26Cablib { get; private set; } = new();
        
        public Db26_ReadTG Db26ReadTG { get; private set; } = new();
        #endregion

        public TramTronS71200()
        {
            IPAddress = "127.0.0.1";
            IPPort = 5102;
            CycleTime = 20 * 10000;
        }

        protected override async Task Comm(Plc plc, double delta, CancellationToken token)
        {
            if (plc == null || !plc.IsConnected) return;

            try
            {
                #region Cấp phối
                if (Db43CP.NeedRead(delta))
                {
                    if (IsRunning && plc.IsConnected) await Db43CP.ReadAsync(plc, delta);
                    if (IsRunning && plc.IsConnected) await Db43CP.WriteAsync(plc, delta);
                }
                if (Db09MeDat.NeedRead(delta))
                {
                    if (IsRunning && plc.IsConnected) await Db09MeDat.ReadAsync(plc, delta);
                    if (Db09MeDat.NoWriteCms > 0) 
                        await Db09MeDat.WriteAsync(plc, delta);
                }
                if (Db43KLMe.NeedRead(delta))
                {
                    await Db43KLMe.ReadAsync(plc, delta);
                }
                #endregion

                #region Cân
                if (Db26WIs.NeedRead(delta)) await Db26WIs.ReadAsync(plc, delta);
                if (Db26WIs2.NeedRead(delta)) await Db26WIs2.ReadAsync(plc, delta);
                if (Db26ReadTG.NeedRead(delta)) await Db26ReadTG.ReadAsync(plc, delta);
                #endregion

                #region TG Trộn
                try
                {
                    if (Db29SetMixerTime.NeedRead(delta))
                    {
                        await Db29SetMixerTime.ReadAsync(plc, delta);
                        await Db29SetMixerTime.WriteAsync(plc, delta);
                    }
                    //if (Db42ReadTG.NeedRead(delta))
                    //{
                    //    await Db42ReadTG.ReadAsync(plc, delta);
                    //}
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Lỗi đọc Db thời gian trộn: {ex.Message}");
                }
                #endregion

                #region Memory
                if (M11.NeedRead(delta))
                {
                    await M11.ReadAsync(plc, delta);
                    await M11.WriteAsync(plc, delta);
                }
                if (M100.NeedRead(delta))
                {
                    await M100.ReadAsync(plc, delta);
                    await M100.WriteAsync(plc, delta);
                }
                if (M200.NeedRead(delta))
                {
                    await M200.ReadAsync(plc, delta);
                }
                if (M1000.NeedRead(delta))
                {
                    await M1000.ReadAsync(plc, delta);
                    await M1000.WriteAsync(plc, delta);
                }
                #endregion

                #region Tham số
                if (Db26ThamSo.NeedRead(delta))
                {
                    await Db26ThamSo.ReadAsync(plc, delta);
                    int n = await Db26ThamSo.WriteAsync(plc, delta);
                    if (n > 0) 
                        Db26ThamSo.ForceRead = true;
                }
                if (Db29ThamSo.NeedRead(delta))
                {
                    await Db29ThamSo.ReadAsync(plc, delta);
                    await Db29ThamSo.WriteAsync(plc, delta);
                }

                if (Db26Cablib.NeedRead(delta))
                {
                    await Db26Cablib.ReadAsync(plc, delta);
                    int n = await Db26Cablib.WriteAsync(plc, delta);
                    if (n > 0)
                        Db26Cablib.ForceRead = true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                //Messages.Add(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.Message);

            }
        }

        public void ClearWriteCmds()
        {
            Db43CP.ClearWriteCmds();
            Db09MeDat.ClearWriteCmds();
            Db29ThamSo.ClearWriteCmds();
            Db26ThamSo.ClearWriteCmds();
            Db26Cablib.ClearWriteCmds();
            // Db26ReadTG.ClearWriteCmds(); // Không ghi
        }

        public void Reset()
        {
            Db09MeDat.UpdateViewT = 0;
            Db09MeDat.ForceRead = true;
            Db43CP.UpdateViewT = 0;
            Db43CP.ForceRead = true;
            Db43KLMe.UpdateViewT = 0;
            Db43KLMe.ForceRead = true;

            Db29ThamSo.UpdateViewT = 0;
            Db29ThamSo.ForceRead = true;

            Db26WIs.UpdateViewT = 0;
            Db26WIs.ForceRead = true;
            Db26WIs2.UpdateViewT = 0;
            Db26WIs2.ForceRead = true;

            Db26ThamSo.UpdateViewT = 0;
            Db26ThamSo.ForceRead = true;
            Db26Cablib.UpdateViewT = 0;
            Db26Cablib.ForceRead = true;

            Db26ReadTG.UpdateViewT = 0;
            Db26ReadTG.ForceRead = true;
        }

        public void MarkUpdateView()
        {
            Db43CP.UpdateViewT = Db43CP.T;
            Db43KLMe.UpdateViewT = Db43KLMe.T;
            Db09MeDat.UpdateViewT = Db09MeDat.T;

            Db29ThamSo.UpdateViewT = Db29ThamSo.T;

            Db26WIs.UpdateViewT = Db26WIs.T;
            Db26WIs2.UpdateViewT = Db26WIs2.T;

            Db26ThamSo.UpdateViewT = Db26ThamSo.T;
            Db26Cablib.UpdateViewT = Db26Cablib.T;

            Db26ReadTG.UpdateViewT = Db26ReadTG.T;
        }

        #region Export Tags
        public string GetListTags()
        {
            StringBuilder sb = new();

            // Get all properties of type PlcDb
            var dbs = typeof(TramTronS71200)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => typeof(PlcDb).IsAssignableFrom(p.PropertyType)).ToList();

            foreach (var i in dbs)
            {
                if (i != null)
                {
                    PlcDb? db = i.GetValue(this) as PlcDb;
                    if (db != null)
                    {
                        List<PlcTag> tags = [];

                        var ptags = db.GetType()
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.PropertyType == typeof(PlcTag)).ToList();
                        foreach (var t in ptags)
                        {
                            PlcTag? tag = t.GetValue(db) as PlcTag;
                            if (tag != null)
                            {
                                tag.Name = t.Name;
                                tags.Add(tag);
                            }
                        }
                        var ptagarrs = db.GetType()
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.PropertyType == typeof(PlcTag[])).ToList();
                        foreach (var arr in ptagarrs)
                        {
                            PlcTag[]? tagarr = arr.GetValue(db) as PlcTag[];
                            if (tagarr != null)
                            {
                                int k = 0;
                                foreach (var t in tagarr)
                                {
                                    t.Name = $"{arr.Name}[{k++}]";
                                    tags.Add(t);
                                }
                            }
                        }
                        tags.Sort();

                        sb.AppendLine($"{i.Name}, {db}, {tags.Count}");
                        foreach (var t in tags)
                        {
                            sb.AppendLine($"   {t.Name}, {t.TagType}, {t.ByteAddr}, {t.Bit}, {GetS71200Addr(db, t)}");
                        }
                    }
                }
            }

            return sb.ToString();
        }
        private string GetS71200Addr(PlcDb db, PlcTag tag)
        {
            if (db.DbType == PlcDbTypes.IQ)
            {
                switch (tag.TagType)
                {
                    case TagTypes.Bool:
                        return $"IQ{tag.ByteAddr}.{tag.Bit}";
                }
            }
            if (db.DbType == PlcDbTypes.M)
            {
                switch (tag.TagType)
                {
                    case TagTypes.Bool:
                        return $"M{tag.ByteAddr}.{tag.Bit}";
                }
            }
            else if (db.DbType == PlcDbTypes.DB)
            {
                switch (tag.TagType)
                {
                    case TagTypes.Bool:
                        return $"DB{db.DbNo}.DBX{tag.ByteAddr}.{tag.Bit}";
                    case TagTypes.Int8:
                        return $"DB{db.DbNo}.DBB{tag.ByteAddr}";
                    case TagTypes.Int16:
                        return $"DB{db.DbNo}.DBW{tag.ByteAddr}";
                    case TagTypes.Int32:
                    case TagTypes.Real:
                        return $"DB{db.DbNo}.DBD{tag.ByteAddr}";
                }
            }
            return "";
        }
        #endregion
    }
}
