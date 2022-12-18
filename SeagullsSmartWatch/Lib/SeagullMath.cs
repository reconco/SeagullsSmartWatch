using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SeagullsSmartWatch
{
    public static class SeagullMath
    {
        public static double ToRadian(double degree)
        {
            return degree * (double)Math.PI / 180.0;
        }

        public static double ToDegree(double radian) 
        {
            return radian * 180.0f / (double)Math.PI;
        }
        public static Point Normalize(Point point)
        {
            double val = 1.0f / Math.Sqrt((point.X * point.X) + (point.Y * point.Y));

            point.X *= val;
            point.Y *= val;

            return point;
        }

        public static Point GetRotatedPointFromOrigin(Point point, Point origin, double angle)
        {
            double radianAngle = ToRadian(angle);
            point = new Point(point.X - origin.X, point.Y - origin.Y);

            double x = (point.X * Math.Cos(radianAngle)) - (point.Y * Math.Sin(radianAngle));
            double y = (point.X * Math.Sin(radianAngle)) + (point.Y * Math.Cos(radianAngle));

            return new Point(x + origin.X, y + origin.Y);
        }
    }
}
