namespace NMWPFControls.Core
{
    public class CalendarUtils
    {
        public static bool IsLeapYear(int yy)
        {
            bool leap = yy % 4 == 0;
            if (yy < 1583) return leap;
            return (yy % 400 == 0) || (yy % 100 != 0 && leap);
        }

        public static int GetDaysInMonth(int m, int y)
        {
            switch (m)
            {
                case 4:
                case 6:
                case 9:
                case 11:
                    return 30;
                case 2: if (IsLeapYear(y)) return 29; else return 28;
                default: return 31;
            }
        }
    }
}
