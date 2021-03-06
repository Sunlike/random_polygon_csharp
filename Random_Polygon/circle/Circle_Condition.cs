﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Random_Polygon.circle;
using System.Collections.ObjectModel;

namespace Random_Polygon
{
    [Serializable]
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
            }
        }
        private int m_MinRadius = 5;

        public int MinRadius
        {
            get { return m_MinRadius; }
            set { m_MinRadius = value; SubscribePropertyChanged("MinRadius"); }
        }
        private int m_MaxRadius = 60;
        public int MaxRadius
        {
            get { return m_MaxRadius; }
            set { m_MaxRadius = value; SubscribePropertyChanged("MaxRadius"); }
        }
        private int m_MinAngle = 10;
        public int MinAngle
        {
            get { return m_MinAngle; }
            set { m_MinAngle = value; SubscribePropertyChanged("MinAngle"); }
        }
        private int m_MaxAngle = 179;
        public int MaxAngle
        {
            get { return m_MaxAngle; }
            set { m_MaxAngle = value; SubscribePropertyChanged("MaxAngle"); }
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
        [field: NonSerialized]  
        public event PropertyChangedEventHandler PropertyChanged;

        private void SubscribePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion


        public override string ToString()
        {
            string format = "物料边数:[3,{0}]\n\r" +
                            "物料外接圆半径大小:[{1},{2}]\n\r" +
                            "物料角度大小:[{3},{4}]\n\r";
            
            return string.Format(format,this.m_MaxEdges,this.m_MinRadius,this.MaxRadius,this.MinAngle,this.MaxAngle);
        }
      
    }

    
}
