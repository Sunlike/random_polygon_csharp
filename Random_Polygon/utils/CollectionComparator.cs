using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Random_Polygon
{
    public class CollectionComparator : IComparer<PointF>
    { 
        private PointF m_center;
        public CollectionComparator(PointF center)
        {
            this.m_center = center;
        }

        public int Compare(PointF p1, PointF p2)
        {

            PointF vector1 = new PointF(), vector2 = new PointF();

            PointF axis = new PointF(), axisVertex = new PointF();


            axis.X = -m_center.X;
            axis.Y = 0;


            axisVertex.X = 0;
            axisVertex.Y = m_center.Y;


            vector1.X = p1.X - m_center.X;
            vector1.Y = p1.Y - m_center.Y;


            vector2.X = p2.X - m_center.X;
            vector2.Y = p2.Y - m_center.Y; ;

            double cos1 = getAngleCos(axis, vector1);
            double cos2 = getAngleCos(axis, vector2);

            bool on_right_av1 = onTheRightSide(axis, vector1);
            bool on_right_av2 = onTheRightSide(axis, vector2);
            bool on_left_av1 = onTheLeftSide(axis, vector1);
            bool on_left_av2 = onTheLeftSide(axis, vector2);
            bool on_same_av1 = onTheSameLine(axis, vector1);
            bool on_same_av2 = onTheSameLine(axis, vector2);

            bool on_right_v1v2 = onTheRightSide(vector1, vector2);
            bool on_same_v1v2 = onTheSameLine(vector1, vector2);

            double dotpro_av1 = doProduct(axis, vector1);
            double dotpro_av2 = doProduct(axis, vector2);

            double magpro_av1 = getMagnitude(axis) * getMagnitude(vector1);
            double magpro_av2 = getMagnitude(axis) * getMagnitude(vector2);

            if (on_right_av1 && on_right_av2)
            {
                if (on_right_v1v2)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (on_left_av1 && on_left_av2)
            {
                if (on_right_v1v2)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (on_right_av1 && on_left_av2)
            {
                return -1;
            }
            else if (on_left_av1 && on_right_av2)
            {
                return 1;
            }
            else if (on_same_v1v2)
            {
                // on_same_av1 && on_same_av2
                if (dotpro_av1 == magpro_av1)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else if (on_same_av1 && on_left_av2)
            {
                return -1;
            }
            else if (on_same_av1 && on_right_av2)
            {
                if (dotpro_av1 == magpro_av1)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (on_same_av2 && on_left_av1)
            {
                return 1;
            }
            else
            {
                //                } else if(on_same_av2 && on_right_av1) {
                if (dotpro_av2 == magpro_av2)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            } 
        }


        // 获得该平面向量的值
        private double getMagnitude(PointF point)
        {
            return Math.Sqrt(Math.Pow(point.X, 2.0) + Math.Pow(point.Y, 2.0));
        }

        private double doProduct(PointF p1, PointF p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y;
        }

        //获得叉积
        private double getAngleCos(PointF p1, PointF p2)
        {
            return doProduct(p1, p2) / (getMagnitude(p1) * getMagnitude(p2));
        }

        // the clock wise circle	
        private bool onTheRightSide(PointF p1, PointF p2)
        {
            return p2.Y * p1.X - p2.X * p1.Y < 0;
        }

        private bool onTheLeftSide(PointF p1, PointF p2)
        {
            return p2.Y * p1.X - p2.X * p1.Y > 0;
        }

        private bool onTheSameLine(PointF p1, PointF p2)
        {
            return p2.Y * p1.X - p2.X * p1.Y == 0;
        }

    }

}
