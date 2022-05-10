using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace SeagullsSmartWatch
{
    /// <summary>
    /// NumberTextbox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NumberTextbox : UserControl
    {

        private static readonly Regex _regex = new Regex("[0-9]"); //regex that matches disallowed text

        public int MaxNumber { get; set; } = 59;

        public TextBox TextBox
        {
            get
            {
                return textbox;
            }
        }

        public string Text
        {
            get
            {
                return textbox.Text;
            }
            set
            {
                textbox.Text = value;
            }
        }

        public NumberTextbox()
        {
            InitializeComponent();
        }
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void CheckNumberic(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = e.Source as TextBox;
            int time = int.Parse(textBox.Text);

            if (time > MaxNumber)
                textBox.Text = MaxNumber.ToString("D2");
            else if(time < 0)
                textBox.Text = 0.ToString("D2");
            else
                textBox.Text = time.ToString("D2");

        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            int cur = int.Parse(TextBox.Text);
            cur++;
            TextBox.Text = cur.ToString("D2");
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            int cur = int.Parse(TextBox.Text);
            cur--;
            TextBox.Text = cur.ToString("D2");
        }
    }
}
