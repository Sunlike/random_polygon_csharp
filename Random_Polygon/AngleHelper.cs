using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Random_Polygon
{
    public class AngleHelper
    {
        /**
   * calc the the angle of <BAC by using vector method
   * @param ptA
   * @param ptB
   * @param ptC
   * @return
   */
        public static double getAngle(Point ptA, Point ptB, Point ptC)
        {
            double angle = 0;
            // vector AB
            double Vab_x = ptB.X - ptA.X;
            double Vab_y = ptB.Y - ptA.Y;

            // vector AC
            double Vac_x = ptC.X - ptA.X;
            double Vac_y = ptC.Y - ptA.Y;

            double productValue = Vab_x * Vac_x + Vab_y * Vac_y;
            double Vab_value = Math.Sqrt(Math.Pow(Vab_x, 2.0) + Math.Pow(Vab_y, 2.0));
            double Vac_value = Math.Sqrt(Math.Pow(Vac_x, 2.0) + Math.Pow(Vac_y, 2.0));

            double cosValue = productValue / (Vab_value * Vac_value);
            //casValue [-1,1]
            if (cosValue < -1 && cosValue > -2)
            {
                cosValue = -1;
            }
            else if (cosValue > 1 && cosValue < 2)
            {
                cosValue = 1;
            }

            angle = Math.Acos(cosValue) * 180 / Math.PI;
            return angle;
        }

        /**
         * points are clockwise
         * @param points
         * @return
         */
        public static List<Double> getAngleFromPoints(List<Point> points)
        {
            List<Double> angleList = new List<Double>();
            List<Point> tmpPoints = new List<Point>(points);
            tmpPoints.Insert(0, points[points.Count - 1]);
            tmpPoints.Add(points[0]);
            int size = tmpPoints.Count;
            for (int i = 1; i < size - 1; ++i)
            {
                double angle = getAngle(tmpPoints[i], tmpPoints[i-1], tmpPoints[i+1]);
                angleList.Add(angle);
            }
            return angleList;
        }
    }
}
