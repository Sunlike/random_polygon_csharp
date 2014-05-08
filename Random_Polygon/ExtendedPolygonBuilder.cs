using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Random_Polygon;

namespace Random_Polygon
{


    /**
     * Created with IntelliJ IDEA.
     * User: Rye
     * Date: 3/4/14
     * Time: 3:15 PM
     */
    public class ExtendedPolygonBuilder
    {

        private RectangleContainer container = null;

        public ExtendedPolygonBuilder()
        {

        }

        public ExtendedPolygonBuilder(RectangleContainer container)
        {
            this.container = container;

        }

        public ExtendedPolygon buildPolygon(int edgeNum, int minRadius, int maxRadius)
        {
            return this.randAnyPolygon(container, edgeNum, minRadius, maxRadius);
        }

        public ExtendedPolygon buildPolygon(Rectangle box, int edgeNum)
        {
            return this.randAnyPolygonWithinBox(box, edgeNum);
        }

        public ExtendedPolygon buildPolygon(int edgeNum, int minRadius, int maxRadius, double minAngle, double maxAngle)
        {
            return this.randAnyPolygon(container, edgeNum, minRadius, maxRadius, minAngle, maxAngle);
        }

        public ExtendedPolygon buildPolygon(Rectangle box, int edgeNum, double minAngle, double maxAngle)
        {
            return this.randAnyPolygonWithinBox(box, edgeNum, minAngle, maxAngle);
        }

        private ExtendedPolygon randAnyPolygonWithinBox(Rectangle box, int edgeNum)
        {
            Random rand = new Random(DateTime.Now.Millisecond);

            Point center = new Point(box.X + box.Width / 2, box.Y + box.Height / 2);

            int radius = box.Width / 2;
            double minTriangleArea = 0.3 * Math.PI * radius * radius;

            List<Point> generatedPoints = new List<Point>();

            ExtendedPolygon triangle = new ExtendedPolygon();
            for (int i = 0; i < edgeNum; i++)
            {
                Point p = new Point();
                do
                {
                    // TODO(Rye): use general formula to be able to limit x & y
                    double param_t = rand.NextDouble() * 2 * Math.PI;
                    p = new Point((int)(radius * Math.Cos(param_t) + center.X), (int)(radius * Math.Sin(param_t) + center.Y));
                    if (!generatedPoints.Contains(p))
                    {
                        generatedPoints.Add(p);
                        break;
                    }
                }
                while (true);

                if (i == 2)
                {
                    // Limit the smallest area of triangle to 1/4 of the circle
                    if (triangle.getArea() < minTriangleArea)
                    {
                        i = 0;
                        generatedPoints.RemoveAt(generatedPoints.Count - 1);
                        generatedPoints.RemoveAt(generatedPoints.Count - 1);
                        triangle = new ExtendedPolygon();
                        Point tmp = generatedPoints[generatedPoints.Count() - 1];
                        triangle.addPoint(tmp);
                        continue;
                    }
                }
            }

            CollectionComparator vComparator = new CollectionComparator(center);
            generatedPoints.Sort(vComparator);

            ExtendedPolygon polygon = new ExtendedPolygon();
            polygon.CircleCenter = new System.Windows.Point(center.X, center.Y);
            polygon.Radius = radius;

            foreach (Point p in generatedPoints)
            {

                polygon.addPoint(p);
            }

            return polygon;
        }

        private ExtendedPolygon randAnyPolygon(RectangleContainer box, int edgeNum, int minRadius, int maxRadius)
        {
            Random rand = new Random(DateTime.Now.Millisecond);

            Point center = new Point();
            center.X = box.X + rand.Next(box.Width);
            center.Y = box.Y + rand.Next(box.Height);
            int radius = minRadius + rand.Next(maxRadius - minRadius + 1);
            double minTriangleArea = 0.05 * Math.PI * radius * radius;

            List<Point> generatedPoints = new List<Point>();

            ExtendedPolygon triangle = new ExtendedPolygon();
            for (int i = 0; i < edgeNum; i++)
            {
                Point p = new Point();
                do
                {
                    // TODO(Rye): use general formula to be able to limit x & y
                    double param_t = rand.NextDouble() * 2 * Math.PI;
                    p.X = (int)(radius * Math.Cos(param_t) + center.X);
                    p.Y = (int)(radius * Math.Sin(param_t) + center.Y);
                    if (!generatedPoints.Contains(p))
                    {
                        generatedPoints.Add(p);
                        triangle.addPoint(p);
                        break;
                    }
                } while (true);

                if (i == 2)
                {
                    // Limit the smallest area of triangle to 1/4 of the circle
                    if (triangle.getArea() < minTriangleArea)
                    {
                        i = 0;
                        generatedPoints.RemoveAt(generatedPoints.Count - 1);
                        generatedPoints.RemoveAt(generatedPoints.Count - 1);
                        triangle = new ExtendedPolygon();
                        Point tmp = generatedPoints[generatedPoints.Count - 1];
                        triangle.addPoint(tmp);
                        continue;
                    }
                }
            }


            CollectionComparator vComparator = new CollectionComparator(center);
            generatedPoints.Sort(vComparator);


            ExtendedPolygon polygon = new ExtendedPolygon();
            polygon.CircleCenter = new System.Windows.Point(center.X, center.Y);
            polygon.Radius = radius;

            foreach (Point p in generatedPoints)
            {

                polygon.addPoint(p);
            }

            return polygon;
        }

        private ExtendedPolygon randAnyPolygonWithinBox(Rectangle box, int edgeNum, double minAngle, double maxAngle)
        {
            Random rand = new Random(DateTime.Now.Millisecond);

            Point center = new Point(box.X + box.Width / 2, box.Y + box.Height / 2);

            int radius = box.Width / 2;
            double minTriangleArea = 0.3 * Math.PI * radius * radius;

            List<Point> generatedPoints = new List<Point>();
            do
            {

                ExtendedPolygon triangle = new ExtendedPolygon();
                generatedPoints.Clear();
                for (int i = 0; i < edgeNum; i++)
                {
                    Point p = new Point();
                    do
                    {
                        // TODO(Rye): use general formula to be able to limit x & y
                        double param_t = rand.NextDouble() * 2 * Math.PI;
                        p = new Point((int)(radius * Math.Cos(param_t) + center.X), (int)(radius * Math.Sin(param_t) + center.Y));
                        if (!generatedPoints.Contains(p))
                        {
                            generatedPoints.Add(p);
                            triangle.addPoint(p);
                            break;
                        }
                    }
                    while (true);

                    if (i == 2)
                    {
                        // Limit the smallest area of triangle to 1/4 of the circle
                        if (triangle.getArea() < minTriangleArea)
                        {
                            i = 0;
                            generatedPoints.RemoveAt(generatedPoints.Count - 1);
                            generatedPoints.RemoveAt(generatedPoints.Count - 1);
                            triangle = new ExtendedPolygon();
                            Point tmp = generatedPoints[generatedPoints.Count() - 1];
                            triangle.addPoint(tmp);
                            continue;
                        }
                    }
                }

                CollectionComparator vComparator = new CollectionComparator(center);
                generatedPoints.Sort(vComparator);

               
                //whether the angle of each edge is under control
                List<Double> angles = AngleHelper.getAngleFromPoints(generatedPoints);
                Boolean isAllOK = true;
                foreach (Double angle in angles)
                {
                    if (angle < minAngle || angle > maxAngle)
                    {
                        isAllOK = false;
                        break;
                    }
                }

                if (isAllOK)
                {
                    break;
                }

            } while (true);

            ExtendedPolygon polygon = new ExtendedPolygon();
            polygon.CircleCenter = new System.Windows.Point(center.X, center.Y);
            polygon.Radius = radius;

            foreach (Point p in generatedPoints)
            {

                polygon.addPoint(p);
            }

            return polygon;
        }

        private ExtendedPolygon randAnyPolygon(RectangleContainer box, int edgeNum, int minRadius, int maxRadius, double minAngle, double maxAngle)
        {
            Random rand = new Random(DateTime.Now.Millisecond);

            Point center = new Point();
            center.X = box.X + rand.Next(box.Width);
            center.Y = box.Y + rand.Next(box.Height);
            int radius = minRadius + rand.Next(maxRadius - minRadius + 1);
            double minTriangleArea = 0.05 * Math.PI * radius * radius;

            List<Point> generatedPoints = new List<Point>();
            do
            {
                ExtendedPolygon triangle = new ExtendedPolygon();
                for (int i = 0; i < edgeNum; i++)
                {
                    Point p = new Point();
                    do
                    {
                        // TODO(Rye): use general formula to be able to limit x & y
                        double param_t = rand.NextDouble() * 2 * Math.PI;
                        p.X = (int)(radius * Math.Cos(param_t) + center.X);
                        p.Y = (int)(radius * Math.Sin(param_t) + center.Y);
                        if (!generatedPoints.Contains(p))
                        {
                            generatedPoints.Add(p);
                            triangle.addPoint(p);
                            break;
                        }
                    } while (true);

                    if (i == 2)
                    {
                        // Limit the smallest area of triangle to 1/4 of the circle
                        if (triangle.getArea() < minTriangleArea)
                        {
                            i = 0;
                            generatedPoints.RemoveAt(generatedPoints.Count - 1);
                            generatedPoints.RemoveAt(generatedPoints.Count - 1);
                            triangle = new ExtendedPolygon();
                            Point tmp = generatedPoints[generatedPoints.Count - 1];
                            triangle.addPoint(tmp);
                            continue;
                        }
                    }
                }



                CollectionComparator vComparator = new CollectionComparator(center);
                generatedPoints.Sort(vComparator);

                

                //whether the angle of each edge is under control
                List<Double> angles = AngleHelper.getAngleFromPoints(generatedPoints);
                Boolean isAllOK = true;
                foreach (Double angle in angles)
                {
                    if (angle < minAngle || angle > maxAngle)
                    {
                        isAllOK = false;
                        break;
                    }
                }

                if (isAllOK)
                {
                    break;
                }

            } while (true);

            ExtendedPolygon polygon = new ExtendedPolygon();
            polygon.CircleCenter = new System.Windows.Point(center.X, center.Y);
            polygon.Radius = radius;

            foreach (Point p in generatedPoints)
            {

                polygon.addPoint(p);
            }
            return polygon;
        }
    }

}
