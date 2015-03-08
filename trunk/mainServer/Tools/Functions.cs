using System;

namespace gameServer.Tools
{
    class Functions
    {
        public static float GetDistanceBetweenPoints(int x1, int y1, int x2, int y2)
        {
            return (float)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
    }
}
