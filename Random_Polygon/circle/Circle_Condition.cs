﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Random_Polygon.circle;

namespace Random_Polygon
{
    public class Circle_Condition :INotifyPropertyChanged
    {
        public Circle_Condition()
        {
            MaxEdges = 5;
        }
        private int m_MaxEdges = 5;
        public int MaxEdges
        {
            get { return m_MaxEdges; }
            set 
            {
                m_MaxEdges = value; 
                SubscribePropertyChanged("MaxEdges");
                this.ratioControlList.Initialize(m_MaxEdges);
            }
        }
        private int m_MinRadius = 5;

        public int MinRadius
        {
            get { return m_MinRadius; }
            set { m_MinRadius = value; }
        }
        private int m_MaxRadius = 60;
        public int MaxRadius
        {
            get { return m_MaxRadius; }
            set { m_MaxRadius = value; }
        }
        private int m_MinAngle = 10;
        public int MinAngle
        {
            get { return m_MinAngle; }
            set { m_MinAngle = value; }
        }
        private int m_MaxAngle = 179;
        public int MaxAngle
        {
            get { return m_MaxAngle; }
            set { m_MaxAngle = value; }
        }
        private int m_MinCoverRadio = 50;
        public int MinCoverRadio
        {
            get { return m_MinCoverRadio; }
            set { m_MinCoverRadio = value; }
        }
        private int m_IterCount = 25000;
        public int IterCount
        {
            get { return m_IterCount; }
            set { m_IterCount = value; }
        }


        private int m_radius = 250;
        public int Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }

        private int m_stepX = -1;
        public int StepX
        {
            get { return m_stepX; }
            set { m_stepX = value; }
        }
        private int m_stepY = 1;
        public int StepY
        {
            get { return m_stepY; }
            set { m_stepY = value; }
        }

        private int m_expandStep = 1;
        public int ExpandStep
        {
            get { return m_expandStep; }
            set { m_expandStep = value; }
        }

        private RatioControlList ratioControlList = new RatioControlList();
        public Random_Polygon.circle.RatioControlList RatioControlList
        {
            get { return ratioControlList; }
            set { ratioControlList = value; SubscribePropertyChanged("RatioControlList"); }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void SubscribePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
