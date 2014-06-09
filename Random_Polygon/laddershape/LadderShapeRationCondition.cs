using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Random_Polygon.circle;
using System.Collections.ObjectModel;


namespace Random_Polygon.laddershape
{
    [Serializable]
    public class LadderShapeRationCondition : INotifyPropertyChanged
    {

        private LadderShap_Condition m_condition = new LadderShap_Condition();
        public Random_Polygon.LadderShap_Condition Condition
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


        public LadderShapeRationCondition Clone()
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(objectStream) as LadderShapeRationCondition;
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


    public class LadderShapeRationConditionList : INotifyPropertyChanged
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


        private int m_upLayer = 200;
        public int UpLayer
        {
            get { return m_upLayer; }
            set { m_upLayer = value; SubscribePropertyChanged("UpLayer"); }
        }
        private int m_downLayer = 400;
        public int DownLayer
        {
            get { return m_downLayer; }
            set { m_downLayer = value; SubscribePropertyChanged("DownLayer"); }
        }

        private int height = 200;
        public int Height
        {
            get { return height; }
            set { height = value; SubscribePropertyChanged("Height"); }
        }

        private ObservableCollection<LadderShapeRationCondition> m_RatioConditionList = new ObservableCollection<LadderShapeRationCondition>();
        public ObservableCollection<LadderShapeRationCondition> RatioConditionList
        {
            get { return m_RatioConditionList; }
            set { m_RatioConditionList = value; }
        }

        public LadderShapeRationCondition getMiniRatioControl()
        {
            LadderShapeRationCondition ratioConditon = (from ratio in m_RatioConditionList orderby ratio.ControlRatio.Diff descending select ratio).First();
            return ratioConditon;
        }

        public void ClearGeneraterInfo()
        {
            foreach (LadderShapeRationCondition ratio in this.m_RatioConditionList)
            {
                ratio.ControlRatio.ClearGenteraterInfo();
            }
        }

        public void UpdateTotalCount()
        {
            foreach (LadderShapeRationCondition ratio in this.m_RatioConditionList)
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
