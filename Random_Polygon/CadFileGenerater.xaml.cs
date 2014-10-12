using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.ComponentModel;
using System.Threading;
using Random_Polygon.circle;
using System.Xml.Serialization;
using Random_Polygon.laddershape;
using Random_Polygon.rectangle;

namespace Random_Polygon
{
    /// <summary>
    /// Interaction logic for CadFileGenerater.xaml
    /// </summary>
    public partial class CadFileGenerater : UserControl, INotifyPropertyChanged
    {
        public CadFileGenerater()
        {
            InitializeComponent();

        }
        private Autodesk.AutoCAD.Interop.AcadApplication AcadApp = null;

        private ObservableCollection<CadFileInfo> m_CadFileInfoList = new ObservableCollection<CadFileInfo>();
        public ObservableCollection<CadFileInfo> CadFileInfoList
        {
            get { return m_CadFileInfoList; }
            set { m_CadFileInfoList = value; SubscribePropertyChanged("CadFileInfoList"); }
        }
        public void Initialize()
        {
            CadFileInfoList.Clear();
            ui_fileInfoView.DataContext = null;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            InitializeFileList(path + "rectangle", CadShapeType.CadShapeType_Rectangle);
            InitializeFileList(path + "laddershape", CadShapeType.CadShapeType_LadderShape);
            InitializeFileList(path + "circle", CadShapeType.CadShapeType_Circle);
            ui_fileInfoView.DataContext = CadFileInfoList;
            if (m_CadFileInfoList.Count > 0)
            {
                ui_fileInfoView.SelectedIndex = 0;
            }
        }

        private void InitializeFileList(string path, CadShapeType shapeType)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] xmlFileList = Directory.GetFiles(path, "*.xml");
            string[] satFileList = Directory.GetFiles(path, "*.sat");
            foreach (string file in xmlFileList)
            {
                CadFileInfo fileInfo = new CadFileInfo();
                fileInfo.CadType = shapeType;
                fileInfo.FileFullPath = file;
                fileInfo.FileName = System.IO.Path.GetFileNameWithoutExtension(file);
                fileInfo.IsGenerater = satFileList.Contains(file.Remove(file.Length - 4) + ".sat");

                CadFileInfoList.Add(fileInfo);
            }

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

        private void ui_fileInfoView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void LunchCad_Click(object sender, RoutedEventArgs e)
        {
            AcadApp = new Autodesk.AutoCAD.Interop.AcadApplication();
            AcadApp.Application.Visible = true;
            Thread.Sleep(2000);
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "CadHelper.dll";
            string strCommand = "netload\r" + filePath + "\r";
            AcadApp.ActiveDocument.SendCommand("filedia\r0\r");
            AcadApp.ActiveDocument.SendCommand(strCommand);
        }



        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            
            //

            CadFileInfo cadFileInfo = ui_fileInfoView.SelectedValue as CadFileInfo;
            if (cadFileInfo == null)
            {
                MessageBox.Show("请先选中要生成的文件", "错误", MessageBoxButton.OK);
                return;
            }

            try
            {
                string strCommand = "";
                AcadApp.Application.Documents.Add("acad.dwt");


                switch (cadFileInfo.CadType)
                {
                    case CadShapeType.CadShapeType_Circle:
                        strCommand = "RunCircle\r\"" + cadFileInfo.FileFullPath + "\"\r";
                        AcadApp.ActiveDocument.SendCommand(strCommand);
                        break;
                    case CadShapeType.CadShapeType_LadderShape:
                        strCommand = "RunLadderShape\r\"" + cadFileInfo.FileFullPath + "\"\r";
                        AcadApp.ActiveDocument.SendCommand(strCommand);
                        break;
                    case CadShapeType.CadShapeType_Rectangle:
                        strCommand = "RunRectangle\r\"" + cadFileInfo.FileFullPath + "\"\r";
                        AcadApp.ActiveDocument.SendCommand(strCommand);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("AutoCad没有启动或者已经关闭，\n请点击左侧“启动Cad” 的按钮进行启动AutoCad.", "错误", MessageBoxButton.OK);
            } 

        }

        public void Close()
        {
            try
            {
                AcadApp.Visible = false;
                AcadApp.Application.Documents.Close();
                AcadApp.Quit();
            }
            catch (System.Exception ex)
            {

            }

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            CadFileInfo cadFileInfo = ui_fileInfoView.SelectedValue as CadFileInfo;
            if (cadFileInfo == null)
            {
                MessageBox.Show("请先选中要生成的文件", "错误", MessageBoxButton.OK);
                return;
            }

            try
            {
                int nThickness = 0;
                switch (cadFileInfo.CadType)
                {
                    case CadShapeType.CadShapeType_Circle: 
                        CircleRatioConditionList Circlelist = GetCircleInfo(cadFileInfo.FileFullPath);
                        nThickness = Circlelist.Thickness;
                        break;
                    case CadShapeType.CadShapeType_LadderShape: 
                        LadderShapeRationConditionList LadderShapelist = GetLadderShapeInfo(cadFileInfo.FileFullPath);
                        nThickness = LadderShapelist.Thickness; 
                        break;
                    case CadShapeType.CadShapeType_Rectangle:
                        RectRationConditionList RectShapelist = GetRectangleInfo(cadFileInfo.FileFullPath);
                        nThickness = RectShapelist.Thickness;
                        break;
                }


                string satName = cadFileInfo.FileFullPath.Replace(".xml", ".sat");

                string strCommand = "";
                strCommand = "region\r" + "select\r" + "all\r\r";
                AcadApp.ActiveDocument.SendCommand(strCommand);
                Thread.Sleep(2000);
                if (nThickness > 0)
                {
                    strCommand = "extrude\r" + "select\r" + "all\r\r" + nThickness.ToString() + "\r";
                    AcadApp.ActiveDocument.SendCommand(strCommand);
                    Thread.Sleep(2000); 
                }
                
                strCommand = "acisout\r" + "select\r" + "all\r\r" + satName +"\r";
                AcadApp.ActiveDocument.SendCommand(strCommand);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("命令参数不正确，请注意先region命令，转化为面\n\r在用extrude命令转化为3D实体\n最后通过命令export或acisout输出sat文件", "错误", MessageBoxButton.OK);
            }


            Thread.Sleep(1000);
            Initialize();

        }

        /// <summary>
        /// 获取圆形边界生成的信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private CircleRatioConditionList GetCircleInfo(string path)
        {
            CircleRatioConditionList result = null;
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(typeof(CircleRatioConditionList));
                result = (CircleRatioConditionList)xs.Deserialize(stream);
            }

            return result;
        }

        /// <summary>
        /// 获取梯形形边界生成的信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private  LadderShapeRationConditionList GetLadderShapeInfo(string path)
        {
            LadderShapeRationConditionList result = null;
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(typeof(LadderShapeRationConditionList));
                result = (LadderShapeRationConditionList)xs.Deserialize(stream);
            }

            return result;
        }

        /// <summary>
        /// 获取矩形形边界生成的信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private  RectRationConditionList GetRectangleInfo(string path)
        {
            RectRationConditionList result = null;
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(typeof(RectRationConditionList));
                result = (RectRationConditionList)xs.Deserialize(stream);
            }

            return result;
        } 

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Cancel == MessageBox.Show("确认删除该行吗？", "提示", MessageBoxButton.OKCancel))
            {
                return;
            }
            Button btn = sender as Button;
            if (btn != null)
            {
                try
                {
                    string tag = btn.Tag.ToString();                     
                    string satPath = tag.Replace(".xml", ".sat");
                    File.Delete(satPath);
                }
                catch (System.Exception ex)
                {
                    string error = "删除失败，失败原因如下:\n\r" + ex.ToString();
                    MessageBox.Show(error, "提示", MessageBoxButton.OK);
                }
                finally
                {
                    Initialize();
                }

            }
            

        }

        private void ButtonDeleteALL_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Cancel == MessageBox.Show("确认删除该行吗？", "提示", MessageBoxButton.OKCancel))
            {
                return;
            }
            Button btn = sender as Button;
            if (btn != null)
            {
                try
                {
                    string tag = btn.Tag.ToString();
                    string xmlPath = tag;
                    string satPath = tag.Replace(".xml", ".sat");
                    File.Delete(xmlPath);
                    File.Delete(satPath);
                }
                catch (System.Exception ex)
                {
                    string error = "删除失败，失败原因如下:\n\r" + ex.ToString();
                    MessageBox.Show(error, "提示", MessageBoxButton.OK);
                }
                finally
                {
                    Initialize();
                }

            }
        }

    }
}
