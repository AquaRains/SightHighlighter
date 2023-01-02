using System.Windows.Controls;
using System.Windows.Input;

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
