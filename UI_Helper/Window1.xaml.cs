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
using System.IO;

namespace UI_Helper
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private UIElement elementForContextMenu;
        string MyStrEntered;

        string UserInputWidth;
        string UserInputHeight;

        public Window1()
        {
            InitializeComponent();
            this.PreviewMouseRightButtonDown += Window1_PreviewMouseRightButtonDown;
        }

        void Window1_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.dragCanvas.ElementBeingDragged != null)
                this.elementForContextMenu = this.dragCanvas.ElementBeingDragged;
            else
                this.elementForContextMenu =
                    this.dragCanvas.FindCanvasChild(e.Source as DependencyObject);
        }

        private void MenuItem_Click_OpenPictureFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            //openFileDlg.DefaultExt = ".png";
            //openFileDlg.Filter = "*.png | *.jpg";
            Nullable<bool> showDlg = openFileDlg.ShowDialog();
            if (showDlg == true)
            {
                string fileName = openFileDlg.FileName;
                if (fileName != null)
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fileName, UriKind.Relative);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    if (bitmap != null)
                    {
                        Image img = new Image();
                        img.Source = bitmap;
                        WPF.JoshSmith.Controls.DragCanvas.SetLocationPath(img, fileName);
                        this.dragCanvas.Children.Add(img);
                        img.Width = bitmap.PixelWidth;
                        img.Height = bitmap.PixelHeight;
                        Canvas.SetBottom(img, 0.0);
                        Canvas.SetLeft(img, 0.0);      
                        this.ResetZOrder();
                    }
                }
            }
        }

        private void ResetZOrder()
        {
            // Set the z-index of every visible child in the Canvas.
            int index = 0;
            for (int i = 0; i < this.dragCanvas.Children.Count; ++i)
                if (this.dragCanvas.Children[i].Visibility == Visibility.Visible)
                    Canvas.SetZIndex(this.dragCanvas.Children[i], index++);
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (this.elementForContextMenu != null)
                this.menuItemCanBeDragged.IsChecked = WPF.JoshSmith.Controls.DragCanvas.GetCanBeDragged(this.elementForContextMenu);
        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            if (this.elementForContextMenu == null)
                return;

       

            if (e.Source == this.menuItemBringToFront ||
                e.Source == this.menuItemSendToBack)
            {
                bool bringToFront = e.Source == this.menuItemBringToFront;

                if (bringToFront)
                    this.dragCanvas.BringToFront(this.elementForContextMenu);
                else
                    this.dragCanvas.SendToBack(this.elementForContextMenu);
            }
            else
            {
                bool canBeDragged = WPF.JoshSmith.Controls.DragCanvas.GetCanBeDragged(this.elementForContextMenu);
                WPF.JoshSmith.Controls.DragCanvas.SetCanBeDragged(this.elementForContextMenu, !canBeDragged);
                (e.Source as MenuItem).IsChecked = !canBeDragged;
            }
        }

        private void MenuItem_Click_Quit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Click_export_xml(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
            Nullable<bool> showDlg = saveFileDlg.ShowDialog();
            if (showDlg == true)
            {
                string fileName = saveFileDlg.FileName;
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);

                sw.BaseStream.Seek(0, SeekOrigin.Begin);

                Window mainWindow = Application.Current.MainWindow;
                PresentationSource currentPS = PresentationSource.FromVisual(mainWindow);
                Matrix m = currentPS.CompositionTarget.TransformFromDevice;

                double dpiWidthFactor = m.M11;
                double dpiHeightFactor = m.M22;
                
                sw.WriteLine("<Bundles>");
                for (int i = 0; i < this.dragCanvas.Children.Count; ++i)
                {
                    string path = WPF.JoshSmith.Controls.DragCanvas.GetLocationPath(this.dragCanvas.Children[i]);
                    double posX = Canvas.GetLeft(this.dragCanvas.Children[i]) * dpiWidthFactor;
                    double posY = Canvas.GetBottom(this.dragCanvas.Children[i]) * dpiHeightFactor;
                    sw.WriteLine("\t<picture src=\"{0}\" posX=\"{1}\" posY=\"{2}\" anchorX=\"0\" anchorY=\"0\"/>", 
                        path,
                        posX,
                        posY);
                }
                sw.WriteLine("</Bundles>");
               
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void MenuItem_Click_screen_resolution(object sender, RoutedEventArgs e)
        {
            if (e.Source == this.iPhone_resolution)
            {
                this.dragCanvas.Width = 480;
                this.dragCanvas.Height = 320;
            }
            else if (e.Source == this.iPhone_Retina_resolution)
            {
                this.dragCanvas.Width = 960;
                this.dragCanvas.Height = 640;
            }
            else if (e.Source == this.iPad_resolution)
            {
                this.dragCanvas.Width = 1024;
                this.dragCanvas.Height = 768;
            }
            else if (e.Source == this.Android_resolution)
            {
                this.dragCanvas.Width = 800;
                this.dragCanvas.Height = 480;
            }
            else if (e.Source == this.self_defined_resolution)
            {
                if (Window2.Show("480", "320", out this.UserInputWidth, out this.UserInputHeight))
                {
                    int width;
                    int height;
                    int.TryParse(UserInputWidth, out width);
                    int.TryParse(UserInputHeight, out height);
                    this.dragCanvas.Width = width;
                    this.dragCanvas.Height = height;
                }
            }
        }

        private void MenuItem_Click_Help(object sender, RoutedEventArgs e)
        {
            string helpString = "1.在\"设置\"中选择对应屏幕分辨率.\n2.在\"文件\"中点击\"导入图片\".\n3.用鼠标拖动图片到适当位置.\n4.重复步骤2和3. \n\n5.点击鼠标右键将对应图片移到到最顶层或最低层.\n\n6.点击\"导出\"菜单，导出描述图片位置的xml文件.";
            string caption = "帮助";
            MessageBoxButton mbb = MessageBoxButton.OK;
            MessageBox.Show(helpString, caption, mbb);
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            string aboutString = "UI编辑助手V1.0\n\n有问题发邮件到 lisicheng@pwrd.com";
            string caption = "关于";
            MessageBoxButton mbb = MessageBoxButton.OK;
            MessageBox.Show(aboutString, caption, mbb);
        }
    }
}
