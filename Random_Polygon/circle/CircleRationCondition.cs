using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Random_Polygon.circle
{
    [Serializable]
    public class CircleRatioCondition : INotifyPropertyChanged
    {

        private Circle_Condition m_condition = new Circle_Condition();
        public Random_Polygon.Circle_Condition Condition
        {
            get { return m_condition; }
            set { m_condition = value; SubscribePropertyChanged("Condition"); }
        }
        private RatioControl m_controlRatio = new RatioControl();
        public Random_Polygon.circle.RatioControl ControlRatio
        {
            get { return m_controlRatio; }
            set { m_controlRatio = value; SubscribePropertyChanged("ControlRatio"); }
        }

        public CircleRatioCondition Clone()
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(objectStream) as CircleRatioCondition;
            }  
        }
        
        #region INotifyPropertyChanged Members
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void SubscribePropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }

    public class CircleRatioConditionList : INotifyPropertyChanged
    {
        private int m_MinCoverRadio = 50;
        public int MinCoverRadio
        {
            get { return m_MinCoverRadio; }
            set { m_MinCoverRadio = value; SubscribePropertyChanged("MinCoverRadio"); }
        }
        private int m_IterCount = 25000;
        public int IterCount
        {
            get { return m_IterCount; }
            set { m_IterCount = value; SubscribePropertyChanged("IterCount"); }
        }


        private int m_radius = 250;
        public int Radius
        {
            get { return m_radius; }
            set { m_radius = value; SubscribePropertyChanged("Radius"); }
        }

        private ObservableCollection<CircleRatioCondition> m_RatioConditionList= new ObservableCollection<CircleRatioCondition>();
        public ObservableCollection<CircleRatioCondition> RatioConditionList
        {
            get { return m_RatioConditionList; }
            set { m_RatioConditionList = value; }
        }

        public CircleRatioCondition getMiniRatioControl()
        {
            CircleRatioCondition ratioConditon = (from ratio in m_RatioConditionList orderby ratio.ControlRatio.Diff descending select ratio).First();
            return ratioConditon;
        }

        public void ClearGeneraterInfo()
        {
            foreach (CircleRatioCondition ratio in this.m_RatioConditionList)
            {
                ratio.ControlRatio.ClearGenteraterInfo();
            }
        }

        public void UpdateTotalCount()
        {
            foreach (CircleRatioCondition ratio in this.m_RatioConditionList)
            {
                ++ratio.ControlRatio.TotalCount;
            }
        }

        public int IsTargetOutLimited(double current)
        {
            double total = current + this.m_RatioConditionList.Sum(x => x.ControlRatio.TargetRatio);
            if (total > 1.0) 
                return 1;
            else if (total < 1.0) 
                return -1;
            else  
                return 0;

             
        }

        #region INotifyPropertyChanged Members
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void SubscribePropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion


    }
}
