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
using System.Windows.Shapes;

namespace UI_Helper
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Boolean _value_button;

        public static Boolean Show(string defaultWidth, string defaultHeight, out string outWidth, out string outHeight)
        {
            Window2 dialog = new Window2();
            dialog.Title = "输入自定义画布的宽度和高度";
            dialog.tbInputWidth.Text = defaultWidth;
            dialog.tbInputHeight.Text = defaultHeight;

            dialog.ShowDialog();
            Boolean resultat = dialog.value_button;
            outWidth = dialog.tbInputWidth.Text;
            outHeight = dialog.tbInputHeight.Text;

            return resultat;
        }

        //public static Boolean Show(string text, string title, string defaultInput, out string userInput)
        //{
        //    Window2 dialog = new Window2();
        //    dialog.lbText.Content = text;
        //    dialog.Title = title;
        //    dialog.tbInput.Text = defaultInput;

        //    dialog.ShowDialog();
        //    Boolean resultat = dialog.value_button;
        //    userInput = dialog.tbInput.Text;

        //    return resultat;
        //}

        public Boolean value_button
        {
            get { return _value_button; }
            set { _value_button = value; }
        }

        public Window2()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbInputWidth.Focus();
            tbInputWidth.SelectAll();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.value_button = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.value_button = false;
            this.Close();
        }
    }
}
