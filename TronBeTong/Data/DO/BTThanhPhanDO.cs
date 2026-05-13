using MySqlConnector;
using S7.Net.Types;
using System.Reflection;

namespace TronBeTongV3.Data.DO
{
    public class BTThanhPhanDO
    {
        private const string _tbl = "ct_thanhphan";

        public int Id { get; set; }
        public int CTId { get; set; }
        public int NLId { get; set; }
        public double KL { get; set; }

        public const string SelectFields = "id,ct_id,nl_id,kl";
        public static BTThanhPhanDO FromDataReader(MySqlDataReader rd)
        {
            return new BTThanhPhanDO()
            {
                Id = rd.GetInt32(0),
                CTId = rd.IsDBNull(1) ? 0 : rd.GetInt32(1),
                NLId = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                KL = rd.IsDBNull(3) ? 0 : rd.GetDouble(3),
            };
        }

        public MySqlCommand CreateUpdate(MySqlConnection conn)
        {
            string query =
                $@"UPDATE {_tbl} SET 
                ct_id=@ctid,nl_id=@nlid,kl=@kl                
                WHERE id={Id}";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@ctid", CTId);
            command.Parameters.AddWithValue("@nlid", NLId);
            command.Parameters.AddWithValue("@kl", KL);
            return command;
        }

        public MySqlCommand CreateInsert(MySqlConnection conn)
        {
            string query = $@"INSERT INTO {_tbl}(
                ct_id,nl_id,kl) 
                VALUES (@ctid,@nlid,@kl);
                SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@ctid", CTId);
            command.Parameters.AddWithValue("@nlid", NLId);
            command.Parameters.AddWithValue("@kl", KL);
            return command;
        }
    }
}
