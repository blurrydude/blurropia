using System;

namespace Server.Customs
{
    public static class MathHelper
    {
        public static Point3D GetCartesian3D(int i, int w)
        {
            var p = GetCartesian(i, w);
            return new Point3D(p.X, p.Y, 0);
        }
        public static Point2D GetCartesian(int i, int w)
        {
            var y = (int)Math.Floor((double)i/w);
            var x = i - (y*w);
            return new Point2D(x, y);
        }

        public static int GetIndex(Point2D p, int w)
        {
            return GetIndex(p.X, p.Y, w);
        }

        public static int GetIndex(int x, int y, int w)
        {
            return y * w + x;
        }
    }
}