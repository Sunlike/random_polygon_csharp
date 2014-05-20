using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Random_Polygon;
using System.Windows.Media;
using System.Diagnostics;


namespace Random_Polygon
{

    public class RectangleContainer
    {
        private Rectangle m_rectange = Rectangle.Empty;
        private double m_blankArea = 0.0;
        private double m_area = 0.0;
        private int listSize = 0;
        private List<ExtendedPolygon>[] polygonInside = new List<ExtendedPolygon>[maxCount];
        private static int maxCount = 5;
        public RectangleContainer(int x, int y, int width, int height)
        {
            m_rectange = new Rectangle(x, y, width, height);
            this.m_area = this.m_blankArea = width * height;
            for (int i = 0; i < maxCount; ++i)
            {
                polygonInside[i] = new List<ExtendedPolygon>();

            }
        }

        public int Width
        {
            get { return this.m_rectange.Width; }
        }
        public int Height
        {
            get { return this.m_rectange.Height; }
        }
        public int X
        {
            get { return this.m_rectange.X; }
        }
        public int Y
        {
            get { return this.m_rectange.Y; }
        }

        public string LogInfo
        {
            get;
            set;
        }

        public Rectangle getRectangle()
        {
            return new Rectangle(X, Y, Width, Height);
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

        private int calculateQuadrant(ExtendedPolygon polygon, int x, int y, int width, int height)
        {
            Point center = new Point(x + width / 2, y + height / 2);
            RectangleContainer section1 = new RectangleContainer(x, y, width / 2, height / 2);
            RectangleContainer section2 = new RectangleContainer(center.X, y, width / 2, height / 2);
            RectangleContainer section3 = new RectangleContainer(x, center.Y, width / 2, height / 2);
            RectangleContainer section4 = new RectangleContainer(center.X, center.Y, width / 2, height / 2);

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

        public bool contains(ExtendedPolygon polygon)
        {
            if (null != polygon.Points)
            {

                List<Point> pts = polygon.getPoints();
                for (int i = 0; i < pts.Count; i++)
                {
                    if (!this.m_rectange.Contains((int)pts[i].X, (int)pts[i].Y))
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        public int getQuadrant(ExtendedPolygon polygon)
        {
            return calculateQuadrant(polygon, this.m_rectange.X, this.m_rectange.Y, this.m_rectange.Width, this.m_rectange.Height);
        }

        public double getBlankArea()
        {
            return this.m_blankArea;
        }

        public double getArea()
        {
            return this.m_area;
        }

        public double getCoverageRatio()
        {
            return 1 - this.m_blankArea / this.m_area;
        }

        public int getListSize()
        {
            return listSize;
        }

        public List<ExtendedPolygon>[] getPolygonsInside()
        {
            return this.polygonInside;
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
    }
}
