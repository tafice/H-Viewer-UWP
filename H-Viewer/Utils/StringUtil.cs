using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HViewer.Utils
{
    public static class StringUtil
    {
        public static string CharAt(this string s, int index)
        {
            if ((index >= s.Length) || (index < 0))
                return "";
            return s.Substring(index, 1);
        }

        public static bool isNumeric(this string str)
        {
            if (str == null || str.Length == 0)
                return false;

            int l = str.Length;
            for (int i = 0; i < l; i++)
            {
                if (!char.IsDigit(str.ElementAt(i)))
                    return false;
            }
            return true;
        }
    }
}
