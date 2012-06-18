using System;
using System.Windows;
using System.Windows.Input;

namespace WPFEx
{
    public partial class FileSelectionDialog : Window
    {
        private System.Windows.Forms.OpenFileDialog file;

        public FileSelectionDialog()
        {
            InitializeComponent();
            file = new System.Windows.Forms.OpenFileDialog();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            TitleLbl.Content = Title;
        }

        private void OkBtnClicked(object sender, RoutedEventArgs e)
        {
            _OkClicked = true;
            _FileName = UserText.Text;
            Close();
        }

        private void BrowseClicked(object sender, RoutedEventArgs e)
        {
            file.Title = this.Title;
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                UserText.Text = file.FileName;
            }
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            _FileName = UserText.Text;
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

        public string _FileName;

        public string FileName
        {
            get
            {
                return _FileName;
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

        public string DefaultExtension
        {
            get { return file.DefaultExt; }
            set { file.DefaultExt = value; }
        }

        public bool CheckFileExists
        {
            get { return file.CheckFileExists; }
            set { file.CheckFileExists = value; }
        }

        public string DefaultPath
        {
            set { file.FileName = value; }
        }
    }
}