using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox.Missile
{
    public static class MathUtils
    {
        private const double Precision = 0.0001;

        public static bool Compare(double a, double b)
        {
            return Math.Abs(a - b) < Precision;
        }

        public static bool Compare(Vector a, Vector b)
        {
            return Compare(a.X, b.X) && Compare(a.Y, b.Y);
        }

        public static bool Compare(Size a, Size b)
        {
            return Compare(a.Width, b.Width) && Compare(a.Height, b.Height);
        }

        public static bool Compare(Rect a, Rect b)
        {
            return Compare(a.X, b.X) && Compare(a.Y, b.Y) &&
                   Compare(a.Width, b.Width) && Compare(a.Height, b.Height);
        }

        public static bool IsNaN(double a)
        {
            return Double.IsNaN(a);
        }

        public static bool IsNaN(Vector a)
        {
            return IsNaN(a.X) && IsNaN(a.Y);
        }

        public static bool IsNaN(Size a)
        {
            return IsNaN(a.Width) && IsNaN(a.Height);
        }

        public static bool IsNaN(Rect a)
        {
            return IsNaN(a.X) && IsNaN(a.Y) &&
                   IsNaN(a.Width) && IsNaN(a.Height);
        }
    }
}
