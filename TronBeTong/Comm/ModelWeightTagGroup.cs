using NMComm.S71200;

namespace TronBeTongV3.Comm
{
    public class ModelWeightTagGroup
    {
        public string Name { get; set; }

        public ModelTag TrangThai { get; set; }
        public ModelTag MeHT { get; set; }
        public ModelTag KL { get; set; }

        public ModelTag OutputValves { get; set; }

        public ModelWeightTagGroup(string name)
        {
            Name = name;
            TrangThai = new ModelTag($"{name}.TT");
            MeHT = new ModelTag($"{name}.Me");
            KL = new ModelTag($"{name}.KL");
            OutputValves = new ModelTag($"{name}.Van");
        }

        public void CreateLink(Dictionary<string, TagLink> links, 
            PlcDb dbTT, PlcTag tagTT, 
            PlcDb dbMe, PlcTag tagMe, 
            PlcDb dbKL, PlcTag tagKL,
            PlcDb dbValves, PlcTag tagValves)
        {
            links.Add(TrangThai.Name, new TagLink(TrangThai, dbTT, tagTT));
            links.Add(MeHT.Name, new TagLink(MeHT, dbMe, tagMe));
            links.Add(KL.Name, new TagLink(KL, dbKL, tagKL));
            links.Add(OutputValves.Name, new TagLink(OutputValves, dbValves, tagValves));
        }
    }
}
