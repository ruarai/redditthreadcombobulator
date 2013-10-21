using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentCombobulator
{
    public static class Utilities
    {
        public static int MaxAt(this int input, int maxlength)
        {
            if (input > maxlength)
            {
                return maxlength;
            }
            return input;
        }
    }
}
