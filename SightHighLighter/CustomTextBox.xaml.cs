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

namespace SightHighlighter
{
    /// <summary>
    /// CustomTextBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CustomTextBox : TextBox
    {
        public CustomTextBox()
        {
            InitializeComponent();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Return)
            {
                this.GetBindingExpression(CustomTextBox.TextProperty).UpdateSource();
                Keyboard.ClearFocus();
            }
                
        }
    }
}
