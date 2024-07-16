using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity
{
    public static class TimeSpanExtensions
    {
        public static string ToMusicString(this TimeSpan timeSpan)
        {

            if (timeSpan.TotalHours < 1)
            {
                return timeSpan.ToString(@"mm\:ss");
            }
            else
            {
                return timeSpan.ToString(@"hh\:mm\:ss");
            }
        }
    }
}
