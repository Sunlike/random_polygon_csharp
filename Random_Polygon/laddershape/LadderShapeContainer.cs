using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Random_Polygon;
using System.Diagnostics;

namespace Random_Polygon.laddershape
{
    public class LadderShapeContainer
    {
        private LadderShape m_laddershape = null;
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

        public LadderShapeContainer( int upLayer,int downLayer,int height)
        {
            m_laddershape = new LadderShape(upLayer, downLayer, height);
            this.m_area = this.m_blankArea = m_laddershape.Area(); ;
            for (int i = 0; i < maxCount; ++i)
            {
                polygonInside[i] = new List<ExtendedPolygon>();

            }
        }

        public double Height
        {
            get { return m_laddershape.Height; }
        }

        public double DownLayer
        {
            get { return m_laddershape.Down_layer; }
        }

        public double UpLayer
        {
            get { return m_laddershape.Up_layer; }
        }

        public List<Point> Points
        {
            get { return m_laddershape.Points; }
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

                List<PointF> pts = polygon.getPoints();
                for (int i = 0; i < pts.Count; i++)
                {
                    if (!this.m_laddershape.Contains(pts[i].X, pts[i].Y))
                    {
                        return false;
                    }
                }

                // 精度计算
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
            return calculateQuadrant(polygon,0,0);
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
        private int calculateQuadrant(ExtendedPolygon polygon, int x, int y)
        {
             
            int height = (int)m_laddershape.Height;
            int width = (int)m_laddershape.Down_layer;




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

        public RectangleContainer GetBoundBox()
        {
           // return new RectangleContainer((int)(m_laddershape.Down_layer-m_laddershape.Up_layer)/2, 0, (int)(m_laddershape.Up_layer), (int)m_laddershape.Height);
           return new RectangleContainer(0, 0,(int)m_laddershape.Down_layer, (int)m_laddershape.Height); 
        }


    }
}
