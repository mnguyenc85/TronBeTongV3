using MySqlConnector;

namespace TronBeTongV3.Data.DO.Business
{
    public class StringDO
    {
        public int Id { get; set; }
        public string? VanBan { get; set; }
        public int PhanLoai { get; set; }
        public int SuDung { get; set; }

        public const string SelectFields = "id,vanban,phanloai,sudung";
        public static StringDO FromDataReader(MySqlDataReader rd)
        {
            return new StringDO()
            {
                Id = rd.GetInt32(0),
                VanBan = rd.IsDBNull(1) ? null : rd.GetString(1),
                PhanLoai = rd.IsDBNull(2) ? 0: rd.GetInt32(2),
                SuDung = rd.IsDBNull(3) ? 0 : rd.GetInt32(3),
            };
        }
    }
}
