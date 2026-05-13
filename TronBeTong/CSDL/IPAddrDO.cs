namespace TronBeTongV3.CSDL
{
    public class IPAddrDO
    {
        public string? Addr { get; set; }
        public int Port { get; set; }

        public bool FromStr(string s)
        {
            string[] ss = s.Split(':');
            bool isIPv4 = true;
            if (ss.Length > 0)
            {
                string[] addrs = ss[0].Split(".");
                if (addrs.Length == 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (int.TryParse(addrs[i], out int b))
                        {
                            if (b < 0 || b > 255) { isIPv4 = false; break; }
                        }
                        else { isIPv4 = false; break; }
                    }
                }
                else isIPv4 = false;
            }
            if (ss.Length > 1)
            {
                if (int.TryParse(ss[1], out int b)) { Port = b; }
                else { isIPv4 = false; }
            }
            else Port = 102;

            if (isIPv4) Addr = ss[0];
            else Addr = null;

            return isIPv4;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Addr, Port);
        }
    }
}
