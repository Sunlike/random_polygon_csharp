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
                        strCommand = "RunCircle\r" + cadFileInfo.FileFullPath + "\r";
                        AcadApp.ActiveDocument.SendCommand(strCommand);
                        break;
                    case CadShapeType.CadShapeType_LadderShape:
                        strCommand = "RunLadderShape\r" + cadFileInfo.FileFullPath + "\r";
                        AcadApp.ActiveDocument.SendCommand(strCommand);
                        break;
                    case CadShapeType.CadShapeType_Rectangle:
                        strCommand = "RunRectangle\r" + cadFileInfo.FileFullPath + "\r";
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

    }
}
