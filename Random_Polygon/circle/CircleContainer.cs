using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Random_Polygon;
using System.Diagnostics;

namespace Random_Polygon.circle
{
    public class CircleContainer
    {
        private RCircle m_circle = new RCircle();
        private int listSize = 0;
        private double m_blankArea = 0.0;
        public double BlankArea
        {
            get { return m_blankArea; }
        }
        private double m_area = 0.0;
        public double Area
        {
            get { return m_area; }
        }
        private List<ExtendedPolygon>[] polygonInside = new List<ExtendedPolygon>[maxCount];
        public List<ExtendedPolygon>[] PolygonInside
        {
            get { return polygonInside; }
        }
        public string LogInfo
        {
            get;
            set;
        }
        private static int maxCount = 5;

        public CircleContainer(int x, int y, int radius)
        {
            m_circle = new RCircle(x, y, radius);
            this.m_area = this.m_blankArea = m_circle.Area();;
            for (int i = 0; i < maxCount; ++i)
            {
                polygonInside[i] = new List<ExtendedPolygon>();

            }
        }

        public double Radius
        {
            get { return m_circle.Radius;}
        }

        public Point Center
        {
            get{return m_circle.Center;}
        }


        public List<ExtendedPolygon> getAllPolygons()
        {
            List<ExtendedPolygon> list = new List<ExtendedPolygon>();
            for (int i = 0; i < maxCount; ++i)
            {
                List<ExtendedPolygon> tmpList = polygonInside[i];
                list.AddRange(tmpList);
            }

            return list;
        }

        public bool contains(ExtendedPolygon polygon)
        {
            if (null != polygon.Points)
            {

                List<Point> pts = polygon.getPoints();
                for (int i = 0; i < pts.Count; i++)
                {
                    if (!this.m_circle.Contains(pts[i].X, pts[i].Y))
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        public double getCoverageRatio()
        {
            return 1 - this.BlankArea / this.Area;
        }

        public void put(ExtendedPolygon polygon)
        {
            this.polygonInside[polygon.Quadrant].Add(polygon);
            this.m_blankArea -= polygon.getArea();
            listSize = 0;
            foreach (List<ExtendedPolygon> temp in this.polygonInside)
            {
                listSize += temp.Count;
            }

            string str = "" + listSize + ": " + polygon.Points.Count + "-edges   " + getCoverageRatio() * 100 + "%     " + polygon.getArea() + " pix^2\n";
            LogInfo += str;
            Debug.WriteLine(str);
        }

        private int getQuadrant(ExtendedPolygon polygon)
        {
            return calculateQuadrant(polygon,0,0,m_circle.Radius);
        }

        public bool canSafePut(ExtendedPolygon polygon)
        {
            if (!this.contains(polygon))
            {
                return false;
            }

            int section = getQuadrant(polygon);
            polygon.Quadrant = section;

            foreach (ExtendedPolygon p in polygonInside[0])
            {
                if (polygon.intersects(p))
                {
                    return false;
                }
            }

            if (section == 0)
            {
                for (int i = 1; i < maxCount; i++)
                {
                    foreach (ExtendedPolygon pg in this.polygonInside[i])
                    {
                        if (polygon.intersects(pg))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                foreach (ExtendedPolygon pg in this.polygonInside[section])
                {
                    if (polygon.intersects(pg))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // x,y center
        private int calculateQuadrant(ExtendedPolygon polygon, int x, int y, double radius)
        {
            Point center = new Point(x,y);
            // 外界元
            int interRadius = 0 - 1 + (int)Math.Round(radius + 0.5);//(int)Math.Round(0.5+Math.Sqrt(2 * Math.Pow(radius,2.0))) -1;
            RectangleContainer section1 = new RectangleContainer(x, y, interRadius, interRadius);
            RectangleContainer section2 = new RectangleContainer(x - interRadius, y, interRadius, interRadius);
            RectangleContainer section3 = new RectangleContainer(x, y - interRadius, interRadius, interRadius);
            RectangleContainer section4 = new RectangleContainer(x - interRadius, y - interRadius, interRadius, interRadius);

            if (section1.contains(polygon))
            {
                return 1;
            }
            else if (section2.contains(polygon))
            {
                return 2;
            }
            else if (section3.contains(polygon))
            {
                return 3;
            }
            else if (section4.contains(polygon))
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }

        public RectangleContainer GetBoundBox()
        {
            int x = (int)(Center.X - Radius);
            int y = (int)(Center.Y - Radius) ;
            int width = 0,height = 0;
            width = height =(int)(2*Radius);
            return new RectangleContainer(x, y, width, height); 
        }


    }
}
