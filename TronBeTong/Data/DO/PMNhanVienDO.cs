using MySqlConnector;

namespace TronBeTongV3.Data.DO
{
    public enum NVRoles { None = 0, Operator = 1, Supervisor = 2, Admin = 4, Tester = 128 }

    public class PMNhanVienDO
    {
        private const string _tbl = "pm_nhanien";

        public int Id { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? HoTen { get; set; }
        public string? Mobile { get; set; }

        public int Flags { get; set; }
        public int LoginCount { get; set; }

        public const string SelectFields = "id,username,pw,ten,mobile,role_flags,login";

        public void Reset()
        {
            Id = 0; HoTen = null; Mobile = null; Flags = 0; LoginCount = 0;
        }

        public void CopyFrom(PMNhanVienDO nv)
        {
            Id = nv.Id;
            UserName = nv.UserName;
            Password = nv.Password;
            HoTen = nv.HoTen;
            Mobile = nv.Mobile;
            Flags = nv.Flags;
            LoginCount = nv.LoginCount;
        }

        public MySqlCommand CreateUpdate(MySqlConnection conn)
        {
            string query = 
                $@"UPDATE {_tbl} SET 
                username=@user,pw=@pw,ten=@ten,
                mobile=@mobile,role_flags=@flags,login=@login
                WHERE id={Id}";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@user", UserName);
            command.Parameters.AddWithValue("@pw", Password);
            command.Parameters.AddWithValue("@ten", HoTen);
            command.Parameters.AddWithValue("@mobile", Mobile);
            command.Parameters.AddWithValue("@flags", Flags);
            command.Parameters.AddWithValue("@login", LoginCount);
            return command;
        }

        public MySqlCommand CreateInsert(MySqlConnection conn)
        {
            string query = $@"INSERT INTO {_tbl}(
                username,pw,ten,mobile,role_flags,login) 
                VALUES (@user,@pw,@ten,@mobile,@flags,@login);
                SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@user", UserName);
            command.Parameters.AddWithValue("@pw", Password);
            command.Parameters.AddWithValue("@ten", HoTen);
            command.Parameters.AddWithValue("@mobile", Mobile);
            command.Parameters.AddWithValue("@flags", Flags);
            command.Parameters.AddWithValue("@login", LoginCount);
            return command;
        }

        public static PMNhanVienDO FromDataReader(MySqlDataReader rd)
        {
            return new PMNhanVienDO()
            {
                Id = rd.GetInt32(0),
                UserName = rd.IsDBNull(1) ? null : rd.GetString(1),
                //Password = rd.IsDBNull(2) ? null : rd.GetString(2),
                HoTen = rd.IsDBNull(3) ? null : rd.GetString(3),
                Mobile = rd.IsDBNull(4) ? null : rd.GetString(4),
                Flags = rd.IsDBNull(5) ? 0 : rd.GetInt32(5),
                LoginCount = rd.IsDBNull(6) ? 0 : rd.GetInt32(6),
            };
        }
    }
}
