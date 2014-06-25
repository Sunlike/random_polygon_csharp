using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;

namespace Random_Polygon
{
    public enum CadShapeType
    {
        CadShapeType_Circle,
        CadShapeType_LadderShape,
        CadShapeType_Rectangle
        
    }

    public class CadTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            CadShapeType cadType = (CadShapeType)Enum.Parse(typeof(CadShapeType),value.ToString(),true);
            switch (cadType)
            {
                case CadShapeType.CadShapeType_Circle :
                    return "圆形边界";
                case CadShapeType.CadShapeType_LadderShape:
                    return "梯形边界";
                case CadShapeType.CadShapeType_Rectangle:
                    return "矩形边界";
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value || false == (bool)value)
            {
                return "否";
            }
            return "是";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CadFileInfo :INotifyPropertyChanged
    {
        public CadFileInfo()
        {

        }

        private string fileFullPath;
        public string FileFullPath
        {
            get { return fileFullPath; }
            set { fileFullPath = value; SubscribePropertyChanged("FileFullPath"); }
        }
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; SubscribePropertyChanged("FileName"); }
        }
        private CadShapeType m_cadType;
        public Random_Polygon.CadShapeType CadType
        {
            get { return m_cadType; }
            set { m_cadType = value; SubscribePropertyChanged("CadType"); }
        }

        private bool m_isGenerater = false;
        public bool IsGenerater
        {
            get { return m_isGenerater; }
            set { m_isGenerater = value; SubscribePropertyChanged("IsGenerater"); }
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
    }
}
