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
        //public Db16_CanCl1_TT Db16CanCL1TT { get; private set; } = new();
        //public Db28_CanCl1_Me Db28CanCL1Me { get; private set; } = new();
        //public Db19_CanCl2_TT Db19CanCL2TT { get; private set; } = new();
        //public Db30_CanCl2_Me Db30CanCL2Me { get; private set; } = new();
        //public Db21_CanCl3_TT Db21CanCL3TT { get; private set; } = new();
        //public Db31_CanCl3_Me Db27CanCL3Me { get; private set; } = new();
        //public Db36_CanXM1_TT Db36CanXM1TT { get; private set; } = new();
        //public Db34_CanXM1_Me Db34CanXM1Me { get; private set; } = new();
        //public Db90_CanXM2_TT Db90CanXM2TT { get; private set; } = new();
        //public Db68_CanXM2_Me Db68CanXM2Me { get; private set; } = new();
        //public Db24_CanNuoc_TT Db24CanNuocTT { get; private set; } = new();
        //public Db33_CanNuoc_Me Db33CanNuocMe { get; private set; } = new();
        //public DB35_CanPG_TT Db35CanPGTT { get; private set; } = new();
        //public Db23_CanPG_Me Db23CanPGMe { get; private set; } = new();
        
        public Db26_WIs Db26WIs { get; private set; } = new();
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
                //if (Db16CanCL1TT.NeedRead(delta)) await Db16CanCL1TT.ReadAsync(plc, delta);
                //if (Db28CanCL1Me.NeedRead(delta)) await Db28CanCL1Me.ReadAsync(plc, delta);

                //if (Db19CanCL2TT.NeedRead(delta)) await Db19CanCL2TT.ReadAsync(plc, delta);
                //if (Db30CanCL2Me.NeedRead(delta)) await Db30CanCL2Me.ReadAsync(plc, delta);

                //if (Db21CanCL3TT.NeedRead(delta)) await Db21CanCL3TT.ReadAsync(plc, delta);
                //if (Db27CanCL3Me.NeedRead(delta)) await Db27CanCL3Me.ReadAsync(plc, delta);

                //if (Db36CanXM1TT.NeedRead(delta)) await Db36CanXM1TT.ReadAsync(plc, delta);
                //if (Db34CanXM1Me.NeedRead(delta)) await Db34CanXM1Me.ReadAsync(plc, delta);

                //if (Db90CanXM2TT.NeedRead(delta)) await Db90CanXM2TT.ReadAsync(plc, delta);
                //if (Db68CanXM2Me.NeedRead(delta)) await Db68CanXM2Me.ReadAsync(plc, delta);

                //if (Db24CanNuocTT.NeedRead(delta)) await Db24CanNuocTT.ReadAsync(plc, delta);
                //if (Db33CanNuocMe.NeedRead(delta)) await Db33CanNuocMe.ReadAsync(plc, delta);

                //if (Db35CanPGTT.NeedRead(delta)) await Db35CanPGTT.ReadAsync(plc, delta);
                //if (Db23CanPGMe.NeedRead(delta)) await Db23CanPGMe.ReadAsync(plc, delta);
                if (Db26WIs.NeedRead(delta)) await Db26WIs.ReadAsync(plc, delta);
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
                    await Db26ThamSo.WriteAsync(plc, delta);
                }
                if (Db29ThamSo.NeedRead(delta))
                {
                    await Db29ThamSo.ReadAsync(plc, delta);
                    await Db29ThamSo.WriteAsync(plc, delta);
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
        }

        public void Reset()
        {
            Db09MeDat.UpdateViewT = 0;
            Db09MeDat.ForceRead = true;
            Db43CP.UpdateViewT = 0;
            Db43CP.ForceRead = true;
            Db43KLMe.UpdateViewT = 0;
            Db43KLMe.ForceRead = true;

            //Db16CanCL1TT.UpdateViewT = 0;
            //Db28CanCL1Me.UpdateViewT = 0;
            //Db19CanCL2TT.UpdateViewT = 0;
            //Db30CanCL2Me.UpdateViewT = 0;
            //Db21CanCL3TT.UpdateViewT = 0;
            //Db27CanCL3Me.UpdateViewT = 0;
            //Db36CanXM1TT.UpdateViewT = 0;
            //Db34CanXM1Me.UpdateViewT = 0;
            //Db90CanXM2TT.UpdateViewT = 0;
            //Db68CanXM2Me.UpdateViewT = 0;
            //Db24CanNuocTT.UpdateViewT = 0;
            //Db33CanNuocMe.UpdateViewT = 0;
            //Db35CanPGTT.UpdateViewT = 0;
            //Db23CanPGMe.UpdateViewT = 0;

            //Db16CanCL1TT.ForceRead = true;
            //Db28CanCL1Me.ForceRead = true;
            //Db19CanCL2TT.ForceRead = true;
            //Db30CanCL2Me.ForceRead = true;
            //Db21CanCL3TT.ForceRead = true;   
            //Db27CanCL3Me.ForceRead = true;
            //Db36CanXM1TT.ForceRead = true;
            //Db34CanXM1Me.ForceRead = true;
            //Db90CanXM2TT.ForceRead = true;
            //Db68CanXM2Me.ForceRead = true;
            //Db24CanNuocTT.ForceRead = true;
            //Db33CanNuocMe.ForceRead = true;
            //Db35CanPGTT.ForceRead = true;
            //Db23CanPGMe.ForceRead = true;

            Db26WIs.UpdateViewT = 0;
            Db26WIs.ForceRead = true;

            Db29ThamSo.UpdateViewT = 0;
            Db29ThamSo.ForceRead = true;
            Db26ThamSo.UpdateViewT = 0;
            Db26ThamSo.ForceRead = true;
        }

        public void MarkUpdateView()
        {
            Db43CP.UpdateViewT = Db43CP.T;
            Db43KLMe.UpdateViewT = Db43KLMe.T;
            Db09MeDat.UpdateViewT = Db09MeDat.T;

            //Db16CanCL1TT.UpdateViewT = Db16CanCL1TT.T;
            //Db28CanCL1Me.UpdateViewT = Db28CanCL1Me.T;
            //Db19CanCL2TT.UpdateViewT = Db19CanCL2TT.T;
            //Db30CanCL2Me.UpdateViewT = Db30CanCL2Me.T;
            //Db21CanCL3TT.UpdateViewT = Db21CanCL3TT.T;
            //Db27CanCL3Me.UpdateViewT = Db27CanCL3Me.T;
            //Db36CanXM1TT.UpdateViewT = Db36CanXM1TT.T;
            //Db34CanXM1Me.UpdateViewT = Db34CanXM1Me.T;
            //Db90CanXM2TT.UpdateViewT = Db90CanXM2TT.T;
            //Db68CanXM2Me.UpdateViewT = Db68CanXM2Me.T;
            //Db24CanNuocTT.UpdateViewT = Db24CanNuocTT.T;
            //Db33CanNuocMe.UpdateViewT = Db33CanNuocMe.T;
            //Db35CanPGTT.UpdateViewT = Db35CanPGTT.T;
            //Db23CanPGMe.UpdateViewT = Db23CanPGMe.T;
            Db26WIs.UpdateViewT = Db26WIs.T;

            Db29ThamSo.UpdateViewT = Db29ThamSo.T;
            Db26ThamSo.UpdateViewT = Db26ThamSo.T;
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
