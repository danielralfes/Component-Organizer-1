using System;
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

        /// <summary>
        /// Handles user cancelling input
        /// </summary>
        private void Cancel()
        {
            _OkClicked = false;
            _Text = UserText.Text;
            Close();
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        /// <summary>
        /// Handles dragging on the chromeless window
        /// </summary>
        private void DragHandler(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public string _Text;

        /// <summary>
        /// Gets the contents of the user input textbox
        /// </summary>
        public string Text
        {
            get
            {
                return _Text;
            }
        }

        private bool _OkClicked = false;

        /// <summary>
        /// Returns the user's button selection
        /// </summary>
        /// <returns>OK or Cancel</returns>
        public new MessageBoxResult DialogResult
        {
            get
            {
                if (_OkClicked)
                    return MessageBoxResult.OK;
                return MessageBoxResult.Cancel;
            }
        }
    }
}