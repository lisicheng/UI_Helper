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
    /// Window3.xaml 的交互逻辑
    /// </summary>
    public partial class Window3 : Window
    {
        public Boolean _value_button;
        static int fontIndex = 0;
        static int sizeIndex = 24;

        public Window3()
        {
            InitializeComponent();

            initFontName();
            this.fontName_ComboBox.SelectedIndex = Window3.fontIndex;

            initFontSize();
            this.fontSize_ComboBox.SelectedIndex = Window3.sizeIndex;
        }

        public Boolean value_button
        {
            get { return this._value_button; }
            set { this._value_button = value; }
        }

        private void initFontName()
        {
            ICollection<FontFamily> fonts = Fonts.SystemFontFamilies;
            foreach (FontFamily font in fonts)
            {
                this.fontName_ComboBox.Items.Add(font);
            }
        }

        private void initFontSize()
        {
            for (int i = 1; i < 200; i++)
            {
                this.fontSize_ComboBox.Items.Add(i);
            }
        }

        public static Boolean Show(out FontFamily ff, out double fs, out string textentered)
        {
            Window3 fp = new Window3();
            fp.Title = "选择字体 大小 并输入字本内容";
            fp.ShowDialog();
            Boolean resultat = fp.value_button;
            ff = fp.fontName_ComboBox.SelectedItem as FontFamily;
            fs = fp.fontSize_ComboBox.SelectedIndex + 1;
            if (fs < 1)
                fs = 1;
            textentered = fp.textentered_TextBox.Text;

            return resultat;
        }

        private void font_Click(object sender, RoutedEventArgs e)
        {
            this.value_button = true;
            this.Close();
        }

        private void Make_default_Click(object sender, RoutedEventArgs e)
        {
            Window3.fontIndex = this.fontName_ComboBox.SelectedIndex;
            Window3.sizeIndex = this.fontSize_ComboBox.SelectedIndex;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.value_button = false;
            this.Close();
        }
    }
}
