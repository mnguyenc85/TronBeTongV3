using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db26_ThamSo : PlcDb
    {
        #region Tham số đặt
        #region Empty Level
        public PlcTag[] EmptyLevelCLs { get; private set; } = new PlcTag[5];
        public PlcTag[] EmptyLevelXMs { get; private set; } = new PlcTag[2];
        public PlcTag[] EmptyLevelNuoc { get; private set; } = new PlcTag[2];
        public PlcTag[] EmptyLevelPGs { get; private set; } = new PlcTag[2];
        #endregion

        #region CutOff Level
        public PlcTag[] CutOffLevelCLs { get; private set; } = new PlcTag[5];
        public PlcTag[] CutOffLevelXMs { get; private set; } = new PlcTag[4];
        public PlcTag[] CutOffLevelNuoc { get; private set; } = new PlcTag[2];
        public PlcTag[] CutOffLevelPGs { get; private set; } = new PlcTag[2];

        public PlcTag[] EnableAutoCutOffCLs { get; private set; } = new PlcTag[5];
        public PlcTag[] EnableAutoCutOffXMs { get; private set; } = new PlcTag[4];
        public PlcTag[] EnableAutoCutOffNuoc { get; private set; } = new PlcTag[2];
        public PlcTag[] EnableAutoCutOffPGs { get; private set; } = new PlcTag[2];
        #endregion

        #region CoarsFine
        public PlcTag[] CoarsFineCLs { get; private set; } = new PlcTag[5];
        public PlcTag[] CoarsFineXMs { get; private set; } = new PlcTag[4];
        public PlcTag[] CoarsFineNuoc { get; private set; } = new PlcTag[2];
        public PlcTag[] CoarsFinePGs { get; private set; } = new PlcTag[2];
        #endregion

        #region Pause Time
        public PlcTag[] PauseTimeCLs { get; private set; } = new PlcTag[5];
        public PlcTag[] PauseTimeXMs { get; private set; } = new PlcTag[2];
        public PlcTag[] PauseTimeNuoc { get; private set; } = new PlcTag[2];
        public PlcTag[] PauseTimePGs { get; private set; } = new PlcTag[2];
        #endregion

        #region FineFactor
        public PlcTag[] FineFactorCLs { get; private set; } = new PlcTag[5];
        public PlcTag[] FineFactorXMs { get; private set; } = new PlcTag[2];
        public PlcTag[] FineFactorNuoc { get; private set; } = new PlcTag[2];
        public PlcTag[] FineFactorPGs { get; private set; } = new PlcTag[2];
        #endregion

        #region EnablePulse
        public PlcTag[] EnablePulseCLs { get; private set; } = new PlcTag[5];

        public PlcTag[] KL0RungXaCanCLs { get; private set; } = new PlcTag[5];
        public PlcTag[] KL0RungXaCanXMs { get; private set; } = new PlcTag[2];
        #endregion

        #region Mức cân nháy
        public PlcTag[] MucCanNhayCLs { get; private set; } = new PlcTag[5];
        //public PlcTag[] MucCanNhayXMs { get; private set; }
        //public PlcTag[] MucCanNhayNuoc { get; private set; }
        //public PlcTag[] MucCanNhayPGs { get; private set; }
        #endregion

        #region Rung & Sục khí
        public PlcTag[] EnableSucKhiSiloes { get; private set; } = new PlcTag[4];
        public PlcTag[] EnableDamRungCLs { get; private set; } = new PlcTag[5];
        #endregion
        #endregion

        #region Calibrations
        public PlcTag[] CalibCLAIs { get; private set; } = new PlcTag[5];
        public PlcTag[] CalibCLZeroes { get; private set; } = new PlcTag[5];
        public PlcTag[] CalibCLSpans { get; private set; } = new PlcTag[5];

        public PlcTag[] CalibXMAIs { get; private set; } = new PlcTag[2];
        public PlcTag[] CalibXMZeroes { get; private set; } = new PlcTag[5];
        public PlcTag[] CalibXMSpans { get; private set; } = new PlcTag[5];

        public PlcTag[] CalibNuocAIs { get; private set; } = new PlcTag[2];
        public PlcTag[] CalibNuocZeroes { get; private set; } = new PlcTag[2];
        public PlcTag[] CalibNuocSpans { get; private set; } = new PlcTag[2];

        public PlcTag[] CalibPGAIs { get; private set; } = new PlcTag[2];
        public PlcTag[] CalibPGZeroes { get; private set; } = new PlcTag[2];
        public PlcTag[] CalibPGSpans { get; private set; } = new PlcTag[2];
        #endregion

        public Db26_ThamSo() : base(26, 342, 0) {
            #region Tham số
            #region Empty Level
            for (int i = 0; i < 5; i++)
                EmptyLevelCLs[i] = new PlcTag(TagTypes.Real, i * 4);
            
            for (int i = 0; i < 2; i++)
                EmptyLevelXMs[i] = new PlcTag(TagTypes.Real, 20 + i * 4);

            for (int i = 0; i < 2; i++)
                EmptyLevelNuoc[i] = new PlcTag(TagTypes.Real, 28 + i * 4);

            for (int i = 0; i < 2; i++)
                EmptyLevelPGs[i] = new PlcTag(TagTypes.Real, 36 + i * 4);
            #endregion

            #region CutOff Level
            for (int i = 0; i < 5; i++)
                CutOffLevelCLs[i] = new PlcTag(TagTypes.Real, 44 + i * 4);
            for (int i = 0; i < 4; i++)
                CutOffLevelXMs[i] = new PlcTag(TagTypes.Real, 64 + i * 4);
            for (int i = 0; i < 2; i++)
                CutOffLevelNuoc[i] = new PlcTag(TagTypes.Real, 80 + i * 4);
            for (int i = 0; i < 2; i++)
                CutOffLevelPGs[i] = new PlcTag(TagTypes.Real, 88 + i * 4);

            for (int i = 0; i < 5; i++)
                EnableAutoCutOffCLs[i] = new PlcTag(TagTypes.Bool, 222, i);
            for (int i = 0; i < 4; i++)
                EnableAutoCutOffXMs[i] = new PlcTag(TagTypes.Bool, 224, i);
            for (int i = 0; i < 2; i++)
                EnableAutoCutOffNuoc[i] = new PlcTag(TagTypes.Bool, 226, i);
            for (int i = 0; i < 2; i++)
                EnableAutoCutOffPGs[i] = new PlcTag(TagTypes.Bool, 228, i);
            #endregion

            #region CoarsFine
            for (int i = 0; i < 5; i++)
                CoarsFineCLs[i] = new PlcTag(TagTypes.Real, 96 + i * 4);
            for (int i = 0; i < 4; i++)
                CoarsFineXMs[i] = new PlcTag(TagTypes.Real, 116 + i * 4);
            for (int i = 0; i < 2; i++)
                CoarsFineNuoc[i] = new PlcTag(TagTypes.Real, 132 + i * 4);
            for (int i = 0; i < 2; i++)
                CoarsFinePGs[i] = new PlcTag(TagTypes.Real, 140 + i * 4);
            #endregion

            #region Pause Time
            for (int i = 0; i < 5; i++)
                PauseTimeCLs[i] = new PlcTag(TagTypes.Int16, 148 + i * 2);
            for (int i = 0; i < 2; i++)
                PauseTimeXMs[i] = new PlcTag(TagTypes.Int16, 158 + i * 2);
            for (int i = 0; i < 2; i++)
                PauseTimeNuoc[i] = new PlcTag(TagTypes.Int16, 162 + i * 2);
            for (int i = 0; i < 2; i++)
                PauseTimePGs[i] = new PlcTag(TagTypes.Int16, 166 + i * 2);
            #endregion

            #region FineFactor
            for (int i = 0; i < 5; i++)
                FineFactorCLs[i] = new PlcTag(TagTypes.Int16, 170 + i * 2);
            for (int i = 0; i < 2; i++)
                FineFactorXMs[i] = new PlcTag(TagTypes.Int16, 180 + i * 2);
            for (int i = 0; i < 2; i++)
                FineFactorNuoc[i] = new PlcTag(TagTypes.Int16, 184 + i * 2);
            for (int i = 0; i < 2; i++)
                FineFactorPGs[i] = new PlcTag(TagTypes.Int16, 188 + i * 2);
            #endregion

            #region Enable Pulse
            for (int i = 0; i < 5; i++)
                EnablePulseCLs[i] = new PlcTag(TagTypes.Bool, 192, i);
            #endregion

            #region Rung xả cân
            for (int i = 0; i < 5; i++)
                KL0RungXaCanCLs[i] = new PlcTag(TagTypes.Real, 194 + i * 4);
            for (int i = 0; i < 2; i++)
                KL0RungXaCanXMs[i] = new PlcTag(TagTypes.Real, 214 + i * 4);
            #endregion

            #region Mức cân nháy
            for (int i = 0; i < 5; i++)
                MucCanNhayCLs[i] = new PlcTag(TagTypes.Int16, 230 + i * 4);
            #endregion

            #region Rung & sục khí
            for (int i = 0; i < 4; i++)
                EnableSucKhiSiloes[i] = new PlcTag(TagTypes.Bool, 250, i);
            for (int i = 0; i < 5; i++)
                EnableDamRungCLs[i] = new PlcTag(TagTypes.Bool, 252, i);
            #endregion
            #endregion

            #region Hiệu chuẩn
            for (int i = 0; i < 5; i++)
            {
                CalibCLAIs[i] = new PlcTag(TagTypes.Int16, 254 + i * 2);
                CalibCLZeroes[i] = new PlcTag(TagTypes.Int16, 264 + i * 2);
                CalibCLSpans[i] = new PlcTag(TagTypes.Real, 274 + i * 4);

            }
            for (int i = 0; i < 2; i++)
            {
                CalibXMAIs[i] = new PlcTag(TagTypes.Int16, 294 + i * 2);
                CalibXMZeroes[i] = new PlcTag(TagTypes.Int16, 298 + i * 2);
                CalibXMSpans[i] = new PlcTag(TagTypes.Real, 302 + i * 4);

                CalibNuocAIs[i] = new PlcTag(TagTypes.Int16, 310 + i * 2);
                CalibNuocZeroes[i] = new PlcTag(TagTypes.Int16, 314 + i * 2);
                CalibNuocSpans[i] = new PlcTag(TagTypes.Real, 318 + i * 4);

                CalibPGAIs[i] = new PlcTag(TagTypes.Int16, 326 + i * 2);
                CalibPGZeroes[i] = new PlcTag(TagTypes.Int16, 330 + i * 2);
                CalibPGSpans[i] = new PlcTag(TagTypes.Real, 334 + i * 4);
            }
            #endregion

            Cycle = -1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            #region Empty Level
            for (int i = 0; i < 5; i++)
                EmptyLevelCLs[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                EmptyLevelXMs[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                EmptyLevelNuoc[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                EmptyLevelPGs[i].ParseDb(_buf, StartByteAddr);
            #endregion

            #region CutOff Level
            for (int i = 0; i < 5; i++)
                CutOffLevelCLs[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 4; i++)
                CutOffLevelXMs[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                CutOffLevelNuoc[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                CutOffLevelPGs[i].ParseDb(_buf, StartByteAddr);

            for (int i = 0; i < 5; i++)
                EnableAutoCutOffCLs[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 4; i++)
                EnableAutoCutOffXMs[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                EnableAutoCutOffNuoc[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                EnableAutoCutOffPGs[i].ParseDb(_buf, StartByteAddr);
            #endregion

            #region CoarsFine
            for (int i = 0; i < 5; i++)
                CoarsFineCLs[i].ParseDb(_buf);
            for (int i = 0; i < 4; i++)
                CoarsFineXMs[i].ParseDb(_buf);
            for (int i = 0; i < 2; i++)
                CoarsFineNuoc[i].ParseDb(_buf);
            for (int i = 0; i < 2; i++)
                CoarsFinePGs[i].ParseDb(_buf, StartByteAddr);
            #endregion

            #region Pause Time
            for (int i = 0; i < 5; i++)
                PauseTimeCLs[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                PauseTimeXMs[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                PauseTimeNuoc[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                PauseTimePGs[i].ParseDb(_buf, StartByteAddr);
            #endregion

            #region FineFactor
            for (int i = 0; i < 5; i++)
                FineFactorCLs[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                FineFactorXMs[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                FineFactorNuoc[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                FineFactorPGs[i].ParseDb(_buf, StartByteAddr);
            #endregion

            #region Enable Pulse
            for (int i = 0; i < 5; i++)
                EnablePulseCLs[i].ParseDb( _buf, StartByteAddr);
            #endregion

            #region KL 0 rung xả cân
            for (int i = 0; i < 5; i++)
                KL0RungXaCanCLs[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 2; i++)
                KL0RungXaCanXMs[i].ParseDb(_buf, StartByteAddr);
            #endregion

            #region Mức cân nháy
            for (int i = 0; i < 5; i++)
                MucCanNhayCLs[i].ParseDb(_buf, StartByteAddr);
            #endregion

            #region Rung & sục khí
            for (int i = 0; i < 4; i++)
                EnableSucKhiSiloes[i].ParseDb(_buf, StartByteAddr);
            for (int i = 0; i < 5; i++)
                EnableDamRungCLs[i].ParseDb(_buf, StartByteAddr);
            #endregion

            #region Hiệu chuẩn
            for (int i = 0; i < 5; i++)
            {
                CalibCLAIs[i].ParseDb(_buf, StartByteAddr);
                CalibCLZeroes[i].ParseDb(_buf, StartByteAddr);
                CalibCLSpans[i].ParseDb(_buf, StartByteAddr);

            }
            for (int i = 0; i < 2; i++)
            {
                CalibXMAIs[i].ParseDb(_buf, StartByteAddr);
                CalibXMZeroes[i].ParseDb(_buf, StartByteAddr);
                CalibXMSpans[i].ParseDb(_buf, StartByteAddr);

                CalibNuocAIs[i].ParseDb(_buf, StartByteAddr);
                CalibNuocZeroes[i].ParseDb(_buf, StartByteAddr);
                CalibNuocSpans[i].ParseDb(_buf, StartByteAddr);

                CalibPGAIs[i].ParseDb(_buf, StartByteAddr);
                CalibPGZeroes[i].ParseDb(_buf, StartByteAddr);
                CalibPGSpans[i].ParseDb(_buf, StartByteAddr);
            }
            #endregion

            T = DateTime.Now.Ticks;
            IsParsingData = false;
        }
    }
}
