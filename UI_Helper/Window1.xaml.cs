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
        private UIElement backgroundElement;
        string UserInputWidth;
        string UserInputHeight;
        int mapWidth = 960;
        int mapHeight = 640;

        public Window1()
        {
            InitializeComponent();
            this.PreviewMouseRightButtonDown += Window1_PreviewMouseRightButtonDown;
            this.dragCanvas.PreviewMouseLeftButtonDown += Window1_PreviewMouseLeftButtonDown;

            UpdateInfo();
        }

        private void Scale_XY_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (this.elementForContextMenu != null)
                {
                    TransformGroup group = (TransformGroup)this.elementForContextMenu.RenderTransform;
                    ScaleTransform trans = group.Children[1] as ScaleTransform;
                    double scalex;
                    double scaley;
                    double.TryParse(this.scaleX_textbox.Text, out scalex);
                    double.TryParse(this.scaleY_textbox.Text, out scaley);
                    trans.ScaleX = scalex;
                    trans.ScaleY = scaley;
                }
            }
            
        }

        private void Window1_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.dragCanvas.ElementBeingDragged != null)
                this.elementForContextMenu = this.dragCanvas.ElementBeingDragged;
            else
            {
                this.elementForContextMenu =
                    this.dragCanvas.FindCanvasChild(e.Source as DependencyObject);
                
            }
        }

        private void Window1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.dragCanvas.ElementBeingDragged != null)
                this.elementForContextMenu = this.dragCanvas.ElementBeingDragged;
            else
            {
                this.elementForContextMenu =
                    this.dragCanvas.FindCanvasChild(e.Source as DependencyObject);
            }
            e.Handled = false;

            UpdateInfo();
        }

        private void UpdateInfo()
        {
            string info_detail;
            if (this.elementForContextMenu != null)
            {
                string path = WPF.JoshSmith.Controls.DragCanvas.GetLocationPath(this.elementForContextMenu);
                info_detail = "当前操作:" + path;
                if (this.backgroundElement != null)
                {
                    info_detail += "\t当前背景:" + WPF.JoshSmith.Controls.DragCanvas.GetLocationPath(this.backgroundElement);
                }
                else
                {
                    info_detail += "\t当前没有设置背景图，点击\"背景\"进行设置";
                }
                this.info_label.Content = info_detail;

                TransformGroup group = (TransformGroup)this.elementForContextMenu.RenderTransform;
                RotateTransform roTrans = group.Children[0] as RotateTransform;
                ScaleTransform scTrans = group.Children[1] as ScaleTransform;

                if (this.elementForContextMenu is Image)
                {
                    Image img = this.elementForContextMenu as Image;
                    this.rotate_slider.Value = roTrans.Angle;
                    this.rotate_textbox.Text = roTrans.Angle.ToString();

                    this.anchorX_textbox.Text = (roTrans.CenterX / img.Width).ToString();
                    this.anchorY_textbox.Text = ((img.Height - roTrans.CenterY) / img.Height).ToString();

                    this.scaleX_textbox.Text = scTrans.ScaleX.ToString();
                    this.scaleY_textbox.Text = scTrans.ScaleY.ToString();
                }
                else if (this.elementForContextMenu is TextBlock)
                {
                    this.rotate_slider.Value = roTrans.Angle;
                    this.rotate_textbox.Text = roTrans.Angle.ToString();

                    this.anchorX_textbox.Text = "N/A";
                    this.anchorY_textbox.Text = "N/A";

                    this.scaleX_textbox.Text = "N/A";
                    this.scaleY_textbox.Text = "N/A";
                }

            }
            else
            {
                info_detail = "当前无操作对象，点击\"文件\"导入图片或文字";
                if (this.backgroundElement != null)
                {
                    info_detail += "\t当前背景:" + WPF.JoshSmith.Controls.DragCanvas.GetLocationPath(this.backgroundElement);
                }
                else
                {
                    info_detail += "\t当前没有设置背景图，点击\"背景\"进行设置";
                }
                this.info_label.Content = info_detail;

                this.rotate_slider.Value = 0;
                this.rotate_textbox.Text = "0";

                this.anchorX_textbox.Text = "N/A";
                this.anchorY_textbox.Text = "N/A";

                this.scaleX_textbox.Text = "N/A";
                this.scaleY_textbox.Text = "N/A";
            }
            
        }

        private void MenuItem_Click_OpenPictureFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            //openFileDlg.DefaultExt = ".png | *.jpg";
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

                        RotateTransform rotateTransform = new RotateTransform(0);
                        ScaleTransform scaleTransform = new ScaleTransform();
                        TranslateTransform translateTransform = new TranslateTransform();

                        TransformGroup transformGroup = new TransformGroup();
                        transformGroup.Children.Add(rotateTransform);
                        transformGroup.Children.Add(scaleTransform);
                        transformGroup.Children.Add(translateTransform);
                       
                        img.Source = bitmap;
                        img.RenderTransform = transformGroup;

                        WPF.JoshSmith.Controls.DragCanvas.SetLocationPath(img, fileName);
                        this.dragCanvas.Children.Add(img);
                        img.Width = bitmap.PixelWidth;
                        img.Height = bitmap.PixelHeight;
                        Canvas.SetBottom(img, (this.mapHeight-img.Height)/2);
                        Canvas.SetTop(img, (this.mapHeight-img.Height)/2);
                        Canvas.SetLeft(img, (this.mapWidth-img.Width)/2);      
                        this.ResetZOrder();

                        rotateTransform.CenterX = img.Width * 0.5;
                        rotateTransform.CenterY = img.Height * 0.5;

                        scaleTransform.CenterX = img.Width * 0.5;
                        scaleTransform.CenterY = img.Height * 0.5;

                        this.elementForContextMenu = img as UIElement;

                        UpdateInfo();
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
            string messageBoxText = "是否保存并导出xml文件？";
            string caption = "退出";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    ExportXMLFile();
                    Application.Current.Shutdown();
                    break;
                case MessageBoxResult.No:
                    Application.Current.Shutdown();
                    break;
                case MessageBoxResult.Cancel:
                    break;
                
            }
           
        }

        private void MenuItem_Click_export_xml(object sender, RoutedEventArgs e)
        {
            ExportXMLFile();
        }

        private void ExportXMLFile()
        {
            if (this.backgroundElement == null)
                MessageBox.Show("未设置背景图片");

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

                sw.Write("<Bundles ");

                double angle_back = 0;
                double anchorX_back = 0;
                double anchorY_back = 0;
                double scaleX_back = 1.0;
                double scaleY_back = 1.0;
                double posX_back = 0;
                double posY_back = 0;

                if (this.backgroundElement == null)
                {
                    sw.Write(">\r\n");
                }
                else
                {
                    string path = WPF.JoshSmith.Controls.DragCanvas.GetLocationPath(this.backgroundElement);
                    TransformGroup group = (TransformGroup)this.backgroundElement.RenderTransform;
                    RotateTransform roTrans = group.Children[0] as RotateTransform;
                    ScaleTransform scTrans = group.Children[1] as ScaleTransform;

                    Image img = this.backgroundElement as Image;
                    angle_back = roTrans.Angle;
                    anchorX_back = roTrans.CenterX / img.Width;
                    anchorY_back = (img.Height - roTrans.CenterY) / img.Height;
                    scaleX_back = scTrans.ScaleX;
                    scaleY_back = scTrans.ScaleY;
                    posX_back = Canvas.GetLeft(this.backgroundElement) * dpiWidthFactor + roTrans.CenterX;
                    posY_back = Canvas.GetBottom(this.backgroundElement) * dpiHeightFactor + img.Height - roTrans.CenterY;

                    sw.WriteLine("Name=\"{0}\" Type=\"Pic\" angle=\"{1}\" anchorX=\"{2}\" anchorY=\"{3}\" scaleX=\"{4}\" scaleY=\"{5}\" posX=\"{6}\" posY=\"{7}\" >\r\n",
                        path, angle_back, anchorX_back, anchorY_back, scaleX_back, scaleY_back, posX_back, posY_back);
                    posX_back -= roTrans.CenterX;
                    posY_back = posY_back - img.Height + roTrans.CenterY; 
                }

                for (int i = 0; i < this.dragCanvas.Children.Count; ++i)
                {
                    if (this.dragCanvas.Children[i] == this.backgroundElement)
                        continue;

                    string path = WPF.JoshSmith.Controls.DragCanvas.GetLocationPath(this.dragCanvas.Children[i]);
                    TransformGroup group = (TransformGroup)this.dragCanvas.Children[i].RenderTransform;
                    RotateTransform roTrans = group.Children[0] as RotateTransform;
                    ScaleTransform scTrans = group.Children[1] as ScaleTransform;

                    double angle = 0.0;
                    double anchorX = 0.5;
                    double anchorY = 0.5;
                    double scaleX = 1.0;
                    double scaleY = 1.0;
                    double posX = 0.0;
                    double posY = 0.0;

                    if (this.dragCanvas.Children[i] is Image)
                    {
                        Image img = this.dragCanvas.Children[i] as Image;
                        angle = roTrans.Angle;
                        anchorX = roTrans.CenterX / img.Width;
                        anchorY = (img.Height - roTrans.CenterY) / img.Height;
                        scaleX = scTrans.ScaleX;
                        scaleY = scTrans.ScaleY;
                        posX = Canvas.GetLeft(this.dragCanvas.Children[i]) * dpiWidthFactor + roTrans.CenterX;
                        posY = Canvas.GetBottom(this.dragCanvas.Children[i]) * dpiHeightFactor + img.Height - roTrans.CenterY;

                        posX -= posX_back;
                        posY -= posY_back;

                        sw.WriteLine("\t<Picture Name=\"{0}\" Type=\"Pic\" angle=\"{1}\" anchorX=\"{2}\" anchorY=\"{3}\" scaleX=\"{4}\" scaleY=\"{5}\" posX=\"{6}\" posY=\"{7}\" />",
                            path, angle, anchorX, anchorY, scaleX, scaleY, posX, posY);
                    }
                    else if (this.dragCanvas.Children[i] is TextBlock)
                    {
                        TextBlock tblock = this.dragCanvas.Children[i] as TextBlock;
                        angle = roTrans.Angle;
                        posX = Canvas.GetLeft(tblock) * dpiWidthFactor;
                        posY = Canvas.GetBottom(tblock) * dpiHeightFactor;

                        posX -= posX_back;
                        posY -= posY_back;

                        string font = tblock.FontFamily.ToString();
                        double fontsize = tblock.FontSize;

                        sw.WriteLine("\t<Text Name=\"{0}\" Type=\"Text\" angle=\"{1}\" posX=\"{2}\" posY=\"{3}\" font=\"{4}\" fontSize=\"{5}\"/>",
                            path, angle, posX, posY, font, fontsize);
                    }
                    
                }
                sw.WriteLine("</Bundles>");

                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void MenuItem_Click_export_ios_xml(object sender, RoutedEventArgs e)
        {
            ExportXMLFileForIOS();
        }

        private void ExportXMLFileForIOS()
        {
            if (this.backgroundElement == null)
                MessageBox.Show("未设置背景图片");

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

                sw.Write("<Bundles target=\"ios\" ");

                double angle_back = 0;
                double anchorX_back = 0;
                double anchorY_back = 0;
                double scaleX_back = 1.0;
                double scaleY_back = 1.0;
                double posX_back = 0;
                double posY_back = 0;

                if (this.backgroundElement == null)
                {
                    sw.Write(">\r\n");
                }
                else
                {
                    string path = WPF.JoshSmith.Controls.DragCanvas.GetLocationPath(this.backgroundElement);
                    TransformGroup group = (TransformGroup)this.backgroundElement.RenderTransform;
                    RotateTransform roTrans = group.Children[0] as RotateTransform;
                    ScaleTransform scTrans = group.Children[1] as ScaleTransform;

                    Image img = this.backgroundElement as Image;
                    angle_back = roTrans.Angle;
                    anchorX_back = roTrans.CenterX / img.Width;
                    anchorY_back = 1 - roTrans.CenterY / img.Height;
                    scaleX_back = scTrans.ScaleX;
                    scaleY_back = scTrans.ScaleY;
                    posX_back = Canvas.GetLeft(this.backgroundElement) * dpiWidthFactor + roTrans.CenterX;
                    posY_back = Canvas.GetTop(this.backgroundElement) * dpiHeightFactor + img.Height - roTrans.CenterY;

                    sw.WriteLine("Name=\"{0}\" Type=\"Pic\" angle=\"{1}\" anchorX=\"{2}\" anchorY=\"{3}\" scaleX=\"{4}\" scaleY=\"{5}\" posX=\"{6}\" posY=\"{7}\" width=\"{8}\" height=\"{9}\" >\r\n",
                        path, angle_back, anchorX_back, anchorY_back, scaleX_back, scaleY_back, posX_back/2, posY_back/2, img.Width/2, img.Height/2);
                    posX_back -= roTrans.CenterX;
                    posY_back -= img.Height - roTrans.CenterY;
                }

                for (int i = 0; i < this.dragCanvas.Children.Count; ++i)
                {
                    if (this.dragCanvas.Children[i] == this.backgroundElement)
                        continue;

                    string path = WPF.JoshSmith.Controls.DragCanvas.GetLocationPath(this.dragCanvas.Children[i]);
                    TransformGroup group = (TransformGroup)this.dragCanvas.Children[i].RenderTransform;
                    RotateTransform roTrans = group.Children[0] as RotateTransform;
                    ScaleTransform scTrans = group.Children[1] as ScaleTransform;

                    double angle = 0.0;
                    double anchorX = 0.5;
                    double anchorY = 0.5;
                    double scaleX = 1.0;
                    double scaleY = 1.0;
                    double posX = 0.0;
                    double posY = 0.0;

                    if (this.dragCanvas.Children[i] is Image)
                    {
                        Image img = this.dragCanvas.Children[i] as Image;
                        angle = roTrans.Angle;
                        anchorX = roTrans.CenterX / img.Width;
                        anchorY = 1 - roTrans.CenterY / img.Height;
                        scaleX = scTrans.ScaleX;
                        scaleY = scTrans.ScaleY;
                        posX = Canvas.GetLeft(this.dragCanvas.Children[i]) * dpiWidthFactor + roTrans.CenterX;
                        posY = Canvas.GetTop(this.dragCanvas.Children[i]) * dpiHeightFactor + img.Height - roTrans.CenterY;

                        posX -= posX_back;
                        posY -= posY_back;

                        sw.WriteLine("\t<Picture Name=\"{0}\" Type=\"Pic\" angle=\"{1}\" anchorX=\"{2}\" anchorY=\"{3}\" scaleX=\"{4}\" scaleY=\"{5}\" posX=\"{6}\" posY=\"{7}\" width=\"{8}\" height=\"{9}\" />",
                            path, angle, anchorX, anchorY, scaleX, scaleY, posX/2, posY/2, img.Width/2, img.Height/2);
                    }
                    else if (this.dragCanvas.Children[i] is TextBlock)
                    {
                        TextBlock tblock = this.dragCanvas.Children[i] as TextBlock;
                        angle = roTrans.Angle;
                        posX = Canvas.GetLeft(tblock) * dpiWidthFactor;
                        posY = Canvas.GetTop(tblock) * dpiHeightFactor;

                        posX -= posX_back;
                        posY -= posY_back;

                        string font = tblock.FontFamily.ToString();
                        double fontsize = tblock.FontSize;

                        sw.WriteLine("\t<Text Name=\"{0}\" Type=\"Text\" angle=\"{1}\" posX=\"{2}\" posY=\"{3}\" font=\"{4}\" fontSize=\"{5}\"/>",
                            path, angle, posX, posY, font, fontsize);
                    }

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
                this.mapWidth = 480;
                this.mapHeight = 320;
                this.dragCanvas.Width = 480;
                this.dragCanvas.Height = 320;
            }
            else if (e.Source == this.iPhone_Retina_resolution)
            {
                this.mapWidth = 960;
                this.mapHeight = 640;
                this.dragCanvas.Width = 960;
                this.dragCanvas.Height = 640;
            }
            else if (e.Source == this.iPhone_Retina_portrait_resolution)
            {
                this.mapWidth = 640;
                this.mapHeight = 960;
                this.dragCanvas.Width = 640;
                this.dragCanvas.Height = 960;
            }
            else if (e.Source == this.iPad_resolution)
            {
                this.mapWidth = 1024;
                this.mapHeight = 768;
                this.dragCanvas.Width = 1024;
                this.dragCanvas.Height = 768;
            }
            else if (e.Source == this.Android_resolution)
            {
                this.mapWidth = 800;
                this.mapHeight = 480;
                this.dragCanvas.Width = 800;
                this.dragCanvas.Height = 480;
            }
            else if (e.Source == this.self_defined_resolution)
            {
                if (Window2.Show("960", "640", out this.UserInputWidth, out this.UserInputHeight))
                {
                    int width;
                    int height;
                    int.TryParse(UserInputWidth, out width);
                    int.TryParse(UserInputHeight, out height);
                    this.dragCanvas.Width = width;
                    this.dragCanvas.Height = height;
                    this.mapWidth = width;
                    this.mapHeight = height;
                }
            }
        }

        private void MenuItem_Click_Help(object sender, RoutedEventArgs e)
        {
            string helpString = 
                "1.在\"设置\"中选择对应屏幕分辨率.\n" +
                "2.在\"文件\"中点击\"导入图片\"或\"插入文字\".\n" +
                "3.点击元件设置为当前操作对象.\n" +
                "4.设置当前选中元件的属性.\n" +
                "5.用鼠标拖动图片到适当位置.\n" +
                "6.重复步骤2到5. \n" +
                "\n" +
                "7.点击鼠标右键将对应图片移到到最顶层或最低层.\n" +
                "\n" +
                "8.点击\"导出\"菜单，导出描述图片位置的xml文件.";
            string caption = "帮助";
            MessageBoxButton mbb = MessageBoxButton.OK;
            MessageBox.Show(helpString, caption, mbb);
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            string aboutString = 
                "UI编辑助手V2.2\n" +
                "\n" +
                "有问题发邮件到 lisicheng@pwrd.com";
            string caption = "关于";
            MessageBoxButton mbb = MessageBoxButton.OK;
            MessageBox.Show(aboutString, caption, mbb);
        }

        private void rotation_value_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.elementForContextMenu != null)
            {
                TransformGroup group = (TransformGroup)this.elementForContextMenu.RenderTransform;
                RotateTransform trans = group.Children[0] as RotateTransform;
                trans.Angle = this.rotate_slider.Value;
                this.rotate_textbox.Text = this.rotate_slider.Value.ToString();
            }
        }

        private void anchorX_textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.elementForContextMenu != null)
            {
                TransformGroup group = (TransformGroup)this.elementForContextMenu.RenderTransform;
                double anchorx;
                double anchory;
                double.TryParse(this.anchorX_textbox.Text, out anchorx);
                double.TryParse(this.anchorY_textbox.Text, out anchory);

                if (this.elementForContextMenu is Image)
                {
                    Image img = this.elementForContextMenu as Image;
                    anchorx = anchorx * img.Width;
                    anchory = (1 - anchory) * img.Height;

                    RotateTransform rotateTrans = group.Children[0] as RotateTransform;
                    rotateTrans.CenterX = anchorx;
                    rotateTrans.CenterY = anchory;

                    ScaleTransform scaleTrans = group.Children[1] as ScaleTransform;
                    scaleTrans.CenterX = anchorx;
                    scaleTrans.CenterY = anchory;
                }
            }
        }

        private void MenuItem_Click_Insert_Text(object sender, RoutedEventArgs e)
        {
            FontFamily ff;
            double fs;
            string te;
            if (Window3.Show(out ff, out fs, out te))
            {
                TextBlock textBlock = new TextBlock();

                textBlock.FontFamily = ff;
                textBlock.FontSize = fs;
                textBlock.Text = te;

                if (te.Equals(""))
                    return;

                RotateTransform rotateTransform = new RotateTransform(0);
                ScaleTransform scaleTransform = new ScaleTransform();
                TranslateTransform translateTransform = new TranslateTransform();

                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(rotateTransform);
                transformGroup.Children.Add(scaleTransform);
                transformGroup.Children.Add(translateTransform);

                textBlock.RenderTransform = transformGroup;

                WPF.JoshSmith.Controls.DragCanvas.SetLocationPath(textBlock, te);
                this.dragCanvas.Children.Add(textBlock);


                Canvas.SetBottom(textBlock, (this.mapHeight) / 2);
                Canvas.SetLeft(textBlock, (this.mapWidth) / 2);
                this.ResetZOrder();

                this.elementForContextMenu = textBlock as UIElement;

                UpdateInfo();
            }
        }

        private void rotate_textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (this.elementForContextMenu != null)
                {
                    TransformGroup group = (TransformGroup)this.elementForContextMenu.RenderTransform;
                    RotateTransform trans = group.Children[0] as RotateTransform;
                    double rotateAngle = 0;
                    double.TryParse(this.rotate_textbox.Text, out rotateAngle);
                    if (rotateAngle > 360)
                    {
                        rotateAngle = 360;
                        this.rotate_textbox.Text = rotateAngle.ToString();
                    }
                    trans.Angle = rotateAngle;
                    this.rotate_slider.Value = rotateAngle;
                }
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (this.elementForContextMenu != null)
            {
                string path = WPF.JoshSmith.Controls.DragCanvas.GetLocationPath(this.elementForContextMenu);
                string messageBoxText = "删除元件 " + path + "？";
                string caption = "删除元件";
                MessageBoxButton button = MessageBoxButton.OKCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;

                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        if (this.backgroundElement == this.elementForContextMenu)
                            this.backgroundElement = null;
                        this.dragCanvas.Children.Remove(this.elementForContextMenu);
                        this.elementForContextMenu = null;
                        UpdateInfo();
                        break;
                    case MessageBoxResult.Cancel:
                        break;

                }
                
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string messageBoxText = "是否保存并导出xml文件？";
            string caption = "退出";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    ExportXMLFile();
                    //Application.Current.Shutdown();
                    break;
                case MessageBoxResult.No:
                    //Application.Current.Shutdown();
                    break;
                case MessageBoxResult.Cancel:
                    break;

            }
        }

        private void set_background_Click(object sender, RoutedEventArgs e)
        {
            if (this.elementForContextMenu != null && this.elementForContextMenu is Image)
            {
                this.backgroundElement = this.elementForContextMenu;
                UpdateInfo();
            }
        }
    }
}
