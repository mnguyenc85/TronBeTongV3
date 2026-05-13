using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db26_ThamSo : PlcDb
    {
        #region Empty Level
        public PlcTag EmptyLevelCL1 { get; private set; }
        public PlcTag EmptyLevelCL2 { get; private set; }
        public PlcTag EmptyLevelCL3 { get; private set; }
        public PlcTag EmptyLevelCL4 { get; private set; }
        public PlcTag EmptyLevelXM1 { get; private set; }
        public PlcTag EmptyLevelXM2 { get; private set; }
        public PlcTag EmptyLevelNuoc { get; private set; }
        public PlcTag EmptyLevelPG1 { get; private set; }
        #endregion

        #region CutOff Level
        public PlcTag CutOffLevelCL1 { get; private set; }
        public PlcTag CutOffLevelCL2 { get; private set; }
        public PlcTag CutOffLevelCL3 { get; private set; }
        public PlcTag CutOffLevelCL4 { get; private set; }
        public PlcTag CutOffLevelCL5 { get; private set; }
        public PlcTag CutOffLevelCL6 { get; private set; }
        public PlcTag CutOffLevelXM1 { get; private set; }
        public PlcTag CutOffLevelXM2 { get; private set; }
        public PlcTag CutOffLevelXM3 { get; private set; }
        public PlcTag CutOffLevelXM4 { get; private set; }
        public PlcTag CutOffLevelNuoc { get; private set; }
        public PlcTag CutOffLevelPG1 { get; private set; }
        public PlcTag CutOffLevelPG2 { get; private set; }

        public PlcTag EnableCutOffLevelCL1 { get; private set; }
        public PlcTag EnableCutOffLevelCL2 { get; private set; }
        public PlcTag EnableCutOffLevelCL3 { get; private set; }
        public PlcTag EnableCutOffLevelCL4 { get; private set; }
        public PlcTag EnableCutOffLevelCL5 { get; private set; }
        public PlcTag EnableCutOffLevelCL6 { get; private set; }
        public PlcTag EnableCutOffLevelXM1 { get; private set; }
        public PlcTag EnableCutOffLevelXM2 { get; private set; }
        public PlcTag EnableCutOffLevelXM3 { get; private set; }
        public PlcTag EnableCutOffLevelXM4 { get; private set; }
        public PlcTag EnableCutOffLevelNuoc { get; private set; }
        public PlcTag EnableCutOffLevelPG1 { get; private set; }
        public PlcTag EnableCutOffLevelPG2 { get; private set; }
        #endregion

        #region CoarsFine
        public PlcTag CoarsFineCL1 { get; private set; }
        public PlcTag CoarsFineCL2 { get; private set; }
        public PlcTag CoarsFineCL3 { get; private set; }
        public PlcTag CoarsFineCL4 { get; private set; }
        public PlcTag CoarsFineXM1 { get; private set; }
        public PlcTag CoarsFineXM2 { get; private set; }
        public PlcTag CoarsFineXM3 { get; private set; }
        public PlcTag CoarsFineXM4 { get; private set; }
        public PlcTag CoarsFineNuoc { get; private set; }
        public PlcTag CoarsFinePG1 { get; private set; }
        #endregion

        #region Pause Time
        public PlcTag PauseTimeCL1 { get; private set; }
        public PlcTag PauseTimeCL2 { get; private set; }
        public PlcTag PauseTimeCL3 { get; private set; }
        public PlcTag PauseTimeCL4 { get; private set; }
        public PlcTag PauseTimeXM1 { get; private set; }
        public PlcTag PauseTimeXM2 { get; private set; }
        public PlcTag PauseTimeNuoc { get; private set; }
        public PlcTag PauseTimePG1 { get; private set; }
        #endregion

        #region FineFactor
        public PlcTag FineFactorCL1 { get; private set; }
        public PlcTag FineFactorCL2 { get; private set; }
        public PlcTag FineFactorCL3 { get; private set; }
        public PlcTag FineFactorCL4 { get; private set; }
        public PlcTag FineFactorXM1 { get; private set; }
        public PlcTag FineFactorXM2 { get; private set; }
        public PlcTag FineFactorNuoc { get; private set; }
        public PlcTag FineFactorPG1 { get; private set; }
        #endregion

        #region EnablePulse
        public PlcTag EnablePulseCL1 { get; private set; }
        public PlcTag EnablePulseCL2 { get; private set; }
        public PlcTag EnablePulseCL3 { get; private set; }
        public PlcTag EnablePulseCL4 { get; private set; }
        public PlcTag EnablePulseCL5 { get; private set; }
        public PlcTag EnablePulseCL6 { get; private set; }
        
        public PlcTag KL0RungXaCanCL1 { get; private set; }
        public PlcTag KL0RungXaCanCL2 { get; private set; }
        public PlcTag KL0RungXaCanCL3 { get; private set; }
        public PlcTag KL0RungXaCanCL4 { get; private set; }
        public PlcTag KL0RungXaCanCL5 { get; private set; }
        public PlcTag KL0RungXaCanCL6 { get; private set; }
        public PlcTag KL0RungXaCanXM1 { get; private set; }
        public PlcTag KL0RungXaCanXM2 { get; private set; }
        #endregion

        #region Mức cân nháy
        public PlcTag MucCanNhayCL1 { get; private set; }
        public PlcTag MucCanNhayCL2 { get; private set; }
        public PlcTag MucCanNhayCL3 { get; private set; }
        public PlcTag MucCanNhayCL4 { get; private set; }
        public PlcTag MucCanNhayCL5 { get; private set; }
        public PlcTag MucCanNhayCL6 { get; private set; }
        public PlcTag MucCanNhayXM1 { get; private set; }
        public PlcTag MucCanNhayXM2 { get; private set; }
        public PlcTag MucCanNhayNuoc { get; private set; }
        public PlcTag MucCanNhayPG1 { get; private set; }
        public PlcTag MucCanNhayPG2 { get; private set; }
        #endregion

        public Db26_ThamSo() : base(26, 238, 0) {
            #region Empty Level
            EmptyLevelCL1 = new PlcTag(TagTypes.Real, 0);
            EmptyLevelCL2 = new PlcTag(TagTypes.Real, 4);
            EmptyLevelCL3 = new PlcTag(TagTypes.Real, 8);
            EmptyLevelCL4 = new PlcTag(TagTypes.Real, 12);
            EmptyLevelXM1 = new PlcTag(TagTypes.Real, 24);
            EmptyLevelXM2 = new PlcTag(TagTypes.Real, 28);
            EmptyLevelNuoc = new PlcTag(TagTypes.Real, 32);
            EmptyLevelPG1 = new PlcTag(TagTypes.Real, 36);
            #endregion

            #region CutOff Level
            CutOffLevelCL1 = new PlcTag(TagTypes.Real, 40);
            CutOffLevelCL2 = new PlcTag(TagTypes.Real, 44);
            CutOffLevelCL3 = new PlcTag(TagTypes.Real, 48);
            CutOffLevelCL4 = new PlcTag(TagTypes.Real, 52);
            CutOffLevelCL5 = new PlcTag(TagTypes.Real, 56);
            CutOffLevelCL6 = new PlcTag(TagTypes.Real, 60);
            CutOffLevelXM1 = new PlcTag(TagTypes.Real, 64);
            CutOffLevelXM2 = new PlcTag(TagTypes.Real, 68);
            CutOffLevelXM3 = new PlcTag(TagTypes.Real, 72);
            CutOffLevelXM4 = new PlcTag(TagTypes.Real, 76);
            CutOffLevelNuoc = new PlcTag(TagTypes.Real, 80);
            CutOffLevelPG1 = new PlcTag(TagTypes.Real, 84);
            CutOffLevelPG2 = new PlcTag(TagTypes.Real, 88);

            EnableCutOffLevelCL1 = new PlcTag(TagTypes.Bool, 214, 0);
            EnableCutOffLevelCL2 = new PlcTag(TagTypes.Bool, 214, 1);
            EnableCutOffLevelCL3 = new PlcTag(TagTypes.Bool, 214, 2);
            EnableCutOffLevelCL4 = new PlcTag(TagTypes.Bool, 214, 3);
            EnableCutOffLevelCL5 = new PlcTag(TagTypes.Bool, 214, 4);
            EnableCutOffLevelCL6 = new PlcTag(TagTypes.Bool, 214, 5);
            EnableCutOffLevelXM1 = new PlcTag(TagTypes.Bool, 214, 6);
            EnableCutOffLevelXM2 = new PlcTag(TagTypes.Bool, 214, 7);
            EnableCutOffLevelXM3 = new PlcTag(TagTypes.Bool, 215, 0);
            EnableCutOffLevelXM4 = new PlcTag(TagTypes.Bool, 215, 1);
            EnableCutOffLevelNuoc = new PlcTag(TagTypes.Bool, 215, 2);
            EnableCutOffLevelPG1 = new PlcTag(TagTypes.Bool, 215, 3);
            EnableCutOffLevelPG2 = new PlcTag(TagTypes.Bool, 215, 4);
            #endregion

            #region CoarsFine
            CoarsFineCL1 = new PlcTag(TagTypes.Real, 92);
            CoarsFineCL2 = new PlcTag(TagTypes.Real, 96);
            CoarsFineCL3 = new PlcTag(TagTypes.Real, 100);
            CoarsFineCL4 = new PlcTag(TagTypes.Real, 104);
            CoarsFineXM1 = new PlcTag(TagTypes.Real, 116);
            CoarsFineXM2 = new PlcTag(TagTypes.Real, 120);
            CoarsFineXM3 = new PlcTag(TagTypes.Real, 124);
            CoarsFineXM4 = new PlcTag(TagTypes.Real, 128);
            CoarsFineNuoc = new PlcTag(TagTypes.Real, 132);
            CoarsFinePG1 = new PlcTag(TagTypes.Real, 136);
            #endregion

            #region Pause Time
            PauseTimeCL1 = new PlcTag(TagTypes.Int16, 140);
            PauseTimeCL2 = new PlcTag(TagTypes.Int16, 142);
            PauseTimeCL3 = new PlcTag(TagTypes.Int16, 144);
            PauseTimeCL4 = new PlcTag(TagTypes.Int16, 146);
            PauseTimeXM1 = new PlcTag(TagTypes.Int16, 152);
            PauseTimeXM2 = new PlcTag(TagTypes.Int16, 154);
            PauseTimeNuoc = new PlcTag(TagTypes.Int16, 156);
            PauseTimePG1 = new PlcTag(TagTypes.Int16, 158);
            #endregion

            #region FineFactor
            FineFactorCL1 = new PlcTag(TagTypes.Int16, 160);
            FineFactorCL2 = new PlcTag(TagTypes.Int16, 162);
            FineFactorCL3 = new PlcTag(TagTypes.Int16, 164);
            FineFactorCL4 = new PlcTag(TagTypes.Int16, 166);
            FineFactorXM1 = new PlcTag(TagTypes.Int16, 172);
            FineFactorXM2 = new PlcTag(TagTypes.Int16, 174);
            FineFactorNuoc = new PlcTag(TagTypes.Int16, 176);
            FineFactorPG1 = new PlcTag(TagTypes.Int16, 178);
            #endregion

            #region Enable Pulse
            EnablePulseCL1 = new PlcTag(TagTypes.Bool, 180, 2);
            EnablePulseCL2 = new PlcTag(TagTypes.Bool, 180, 3);
            EnablePulseCL3 = new PlcTag(TagTypes.Bool, 180, 4);
            EnablePulseCL4 = new PlcTag(TagTypes.Bool, 180, 5);
            EnablePulseCL5 = new PlcTag(TagTypes.Bool, 180, 6);
            EnablePulseCL6 = new PlcTag(TagTypes.Bool, 180, 7);
            #endregion

            #region Rung xả cân
            KL0RungXaCanCL1 = new PlcTag(TagTypes.Real, 182);
            KL0RungXaCanCL2 = new PlcTag(TagTypes.Real, 186);
            KL0RungXaCanCL3 = new PlcTag(TagTypes.Real, 190);
            KL0RungXaCanCL4 = new PlcTag(TagTypes.Real, 194);
            KL0RungXaCanCL5 = new PlcTag(TagTypes.Real, 198);
            KL0RungXaCanCL6 = new PlcTag(TagTypes.Real, 202);
            KL0RungXaCanXM1 = new PlcTag(TagTypes.Real, 206);
            KL0RungXaCanXM2 = new PlcTag(TagTypes.Real, 210);
            #endregion

            #region Mức cân nháy
            MucCanNhayCL1 = new PlcTag(TagTypes.Int16, 216);
            MucCanNhayCL2 = new PlcTag(TagTypes.Int16, 218);
            MucCanNhayCL3 = new PlcTag(TagTypes.Int16, 220);
            MucCanNhayCL4 = new PlcTag(TagTypes.Int16, 222);
            MucCanNhayCL5 = new PlcTag(TagTypes.Int16, 224);
            MucCanNhayCL6 = new PlcTag(TagTypes.Int16, 226);
            MucCanNhayXM1 = new PlcTag(TagTypes.Int16, 228);
            MucCanNhayXM2 = new PlcTag(TagTypes.Int16, 230);
            MucCanNhayNuoc = new PlcTag(TagTypes.Int16, 232);
            MucCanNhayPG1 = new PlcTag(TagTypes.Int16, 234);
            MucCanNhayPG2 = new PlcTag(TagTypes.Int16, 236);
            #endregion

            Cycle = -1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            #region Empty Level
            EmptyLevelCL1.ParseDb(_buf, StartByteAddr);
            EmptyLevelCL2.ParseDb(_buf, StartByteAddr);
            EmptyLevelCL3.ParseDb(_buf, StartByteAddr);
            EmptyLevelCL4.ParseDb(_buf, StartByteAddr);
            EmptyLevelXM1.ParseDb(_buf, StartByteAddr);
            EmptyLevelXM2.ParseDb(_buf, StartByteAddr);
            EmptyLevelNuoc.ParseDb(_buf, StartByteAddr);
            EmptyLevelPG1.ParseDb(_buf, StartByteAddr);
            #endregion

            #region CutOff Level
            CutOffLevelCL1.ParseDb(_buf, StartByteAddr);
            CutOffLevelCL2.ParseDb(_buf, StartByteAddr);
            CutOffLevelCL3.ParseDb(_buf, StartByteAddr);
            CutOffLevelCL4.ParseDb(_buf, StartByteAddr);
            //CutOffLevelCL5.ParseDb(_buf, StartByteAddr);
            //CutOffLevelCL6.ParseDb(_buf, StartByteAddr);
            CutOffLevelXM1.ParseDb(_buf, StartByteAddr);
            CutOffLevelXM2.ParseDb(_buf, StartByteAddr);
            CutOffLevelXM3.ParseDb(_buf, StartByteAddr);
            CutOffLevelXM4.ParseDb(_buf, StartByteAddr);
            CutOffLevelNuoc.ParseDb(_buf, StartByteAddr);
            CutOffLevelPG1.ParseDb(_buf, StartByteAddr);
            CutOffLevelPG2.ParseDb(_buf, StartByteAddr);

            EnableCutOffLevelCL1.ParseDb(_buf, StartByteAddr);
            EnableCutOffLevelCL2.ParseDb(_buf, StartByteAddr);
            EnableCutOffLevelCL3.ParseDb(_buf, StartByteAddr);
            EnableCutOffLevelCL4.ParseDb(_buf, StartByteAddr);
            EnableCutOffLevelCL5.ParseDb(_buf, StartByteAddr);
            EnableCutOffLevelCL6.ParseDb(_buf, StartByteAddr);
            EnableCutOffLevelXM1.ParseDb(_buf, StartByteAddr);
            EnableCutOffLevelXM2.ParseDb(_buf, StartByteAddr);
            EnableCutOffLevelXM3.ParseDb(_buf, StartByteAddr);
            EnableCutOffLevelXM4.ParseDb(_buf, StartByteAddr);
            EnableCutOffLevelNuoc.ParseDb(_buf, StartByteAddr);
            EnableCutOffLevelPG1.ParseDb(_buf, StartByteAddr);
            EnableCutOffLevelPG2.ParseDb(_buf, StartByteAddr);
            #endregion

            #region CoarsFine
            CoarsFineCL1.ParseDb(_buf);
            CoarsFineCL2.ParseDb(_buf);
            CoarsFineCL3.ParseDb(_buf);
            CoarsFineCL4.ParseDb(_buf);
            CoarsFineXM1.ParseDb(_buf);
            CoarsFineXM2.ParseDb(_buf);
            CoarsFineXM3.ParseDb(_buf);
            CoarsFineXM4.ParseDb(_buf);
            CoarsFineNuoc.ParseDb(_buf);
            CoarsFinePG1.ParseDb(_buf, StartByteAddr);
            #endregion

            #region Pause Time
            PauseTimeCL1.ParseDb(_buf, StartByteAddr);
            PauseTimeCL2.ParseDb(_buf, StartByteAddr);
            PauseTimeCL3.ParseDb(_buf, StartByteAddr);
            PauseTimeCL4.ParseDb(_buf, StartByteAddr);
            PauseTimeXM1.ParseDb(_buf, StartByteAddr);
            PauseTimeXM2.ParseDb(_buf, StartByteAddr);
            PauseTimeNuoc.ParseDb(_buf, StartByteAddr);
            PauseTimePG1.ParseDb(_buf, StartByteAddr);
            #endregion

            #region FineFactor
            FineFactorCL1.ParseDb(_buf, StartByteAddr);
            FineFactorCL2.ParseDb(_buf, StartByteAddr);
            FineFactorCL3.ParseDb(_buf, StartByteAddr);
            FineFactorCL4.ParseDb(_buf, StartByteAddr);
            FineFactorXM1.ParseDb(_buf, StartByteAddr);
            FineFactorXM2.ParseDb(_buf, StartByteAddr);
            FineFactorNuoc.ParseDb(_buf, StartByteAddr);
            FineFactorPG1.ParseDb(_buf, StartByteAddr);
            #endregion

            #region Enable Pulse
            EnablePulseCL1.ParseDb( _buf, StartByteAddr);
            EnablePulseCL2.ParseDb(_buf, StartByteAddr);
            EnablePulseCL3.ParseDb(_buf, StartByteAddr);
            EnablePulseCL4.ParseDb(_buf, StartByteAddr);
            #endregion

            #region KL 0 rung xả cân
            KL0RungXaCanCL1.ParseDb(_buf, StartByteAddr);
            KL0RungXaCanCL2.ParseDb(_buf, StartByteAddr);
            KL0RungXaCanCL3.ParseDb(_buf, StartByteAddr);
            KL0RungXaCanCL4.ParseDb(_buf, StartByteAddr);
            KL0RungXaCanCL5.ParseDb(_buf, StartByteAddr);
            KL0RungXaCanCL6.ParseDb(_buf, StartByteAddr);
            KL0RungXaCanXM1.ParseDb(_buf, StartByteAddr);
            KL0RungXaCanXM2.ParseDb(_buf, StartByteAddr);
            #endregion

            #region Mức cân nháy
            MucCanNhayCL1.ParseDb(_buf, StartByteAddr);
            MucCanNhayCL2.ParseDb(_buf, StartByteAddr);
            MucCanNhayCL3.ParseDb(_buf, StartByteAddr);
            MucCanNhayCL4.ParseDb(_buf, StartByteAddr);
            MucCanNhayCL5.ParseDb(_buf, StartByteAddr);
            MucCanNhayCL6.ParseDb(_buf, StartByteAddr);
            MucCanNhayXM1.ParseDb(_buf, StartByteAddr);
            MucCanNhayXM2.ParseDb(_buf, StartByteAddr);
            MucCanNhayNuoc.ParseDb(_buf, StartByteAddr);
            MucCanNhayPG1.ParseDb(_buf, StartByteAddr);
            MucCanNhayPG2.ParseDb(_buf, StartByteAddr);
            #endregion

            T = DateTime.Now.Ticks;
            IsParsingData = false;
        }
    }
}
