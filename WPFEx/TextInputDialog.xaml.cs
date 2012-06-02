using System.Windows;
using System.Windows.Input;

namespace WPFEx
{
    public partial class TextInputDialog : Window
    {
        public TextInputDialog()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            TitleLbl.Content = Title;
        }

        private void OkBtnClicked(object sender, RoutedEventArgs e)
        {
            _OkClicked = true;
            _Text = UserText.Text;
            Close();
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            _Text = UserText.Text;
            Close();
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DragHandler(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public string _Text;

        public string Text
        {
            get
            {
                return _Text;
            }
        }

        private bool _OkClicked = false;

        public bool OkClicked
        {
            get
            {
                return _OkClicked;
            }
        }
    }
}