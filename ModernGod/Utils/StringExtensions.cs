using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.Utils
{
    public static class StringExtensions
    {
        public static string Form(this string s, params object[] args)
        {
            return string.Format(s, args);
        }
    }
}
