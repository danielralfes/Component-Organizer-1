using System;
using System.Windows;
using System.Windows.Input;

namespace WPFEx
{
    public partial class PathSelectionDialog : Window
    {
        public PathSelectionDialog()
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
            _Path = UserText.Text;
            Close();
        }

        private void BrowseClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                UserText.Text = folder.SelectedPath;
            }
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            _Path = UserText.Text;
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

        public string _Path;

        public string Path
        {
            get
            {
                return _Path;
            }
        }

        private bool _OkClicked = false;

        [Obsolete("TODO: Use a standard enum")]
        public bool OkClicked
        {
            get
            {
                return _OkClicked;
            }
        }
    }
}