using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
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
using OrganizerDB;
using WPFEx;

namespace Document_Organizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Database.SetInitializer<OrganizerContext>(new OrganizerContextInitializer());

            SetPDFPath();
            SetupUI();
            UpdateDbList();
        }

        private void SetPDFPath()
        {
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.Path))
            {
                // If the user has set a default path in the past, use it
                mainPath = Properties.Settings.Default.Path;
            }
            else
            {
                // Otherwise, ask the user for a path

                PathSelectionDialog path = new PathSelectionDialog();
                path.Title = "Please select the root PDF directory";
                path.ShowDialog();
                if (path.OkClicked)
                {
                    mainPath = path.Path;

                    Properties.Settings.Default.Path = mainPath;
                    Properties.Settings.Default.Save();
                }
                else
                    Close();
            }
        }

        private void SetupUI()
        {
            //string[] dirs = Directory.GetDirectories(mainPath);
            //foreach (string dir in dirs)
            //{
            //    Files.Items.Add(dir);
            //}

            string[] files = Directory.GetFiles(mainPath, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Files.Items.Add(System.IO.Path.GetFileName(file));
            }
        }

        private void UpdateDbList()
        {
            ORM.Items.Clear();

            Database.SetInitializer<OrganizerContext>(new OrganizerContextInitializer());
            using (var context = new OrganizerContext())
            {
                //foreach (var folder in context.Folders)
                //{
                foreach (var pdf in context.PDFs)
                {
                    ORM.Items.Add(pdf.FileName);
                }
                //}
            }
        }

        string mainPath;

        private void SaveEntry(object sender, RoutedEventArgs e)
        {
            using (var context = new OrganizerContext())
            {
                bool fileSelected = true;
                if (Files.SelectedIndex == -1)
                    fileSelected = false;
                string desiredFileName = (fileSelected ? (string)(Files.SelectedValue) : (string)(ORM.SelectedValue));

                var thisPdf = (from p in context.PDFs
                               where p.FileName == desiredFileName
                               select p).FirstOrDefault();

                bool newPDF = false;
                if (thisPdf == null)
                {
                    thisPdf = new PDF();
                    newPDF = true;
                }

                if ((string)((ListBoxItem)Folder.SelectedValue).Content == "Add folder...")
                    Folder.SelectedValue = AddFolder();

                thisPdf.Folder = (from f in context.Folders
                                  where f.Name == (string)Folder.SelectedValue
                                  select f).FirstOrDefault();

                if (thisPdf.Folder == null)
                    thisPdf.Folder = (from f in context.Folders
                                      where f.Name == "Default"
                                      select f).First();

                thisPdf.FriendlyName = FriendlyName.Text;

                if (newPDF)
                {
                    thisPdf.FileName = (string)Files.SelectedValue;

                    context.PDFs.Add(thisPdf);
                }

                int changes = context.SaveChanges();

                UpdateDbList();
            }
        }

        private void ShowSelectionFromFiles(object sender, SelectionChangedEventArgs e)
        {
            ORM.SelectedIndex = -1;

            using (var context = new OrganizerContext())
            {
                var thisPdf = (from p in context.PDFs
                               where p.FileName == (string)(((ListBox)sender).SelectedValue)
                               select p).FirstOrDefault();

                if (thisPdf == null)
                {
                    FriendlyName.Text = "";
                    SelectedFile.Content = (string)(((ListBox)sender).SelectedValue);
                    Folder.SelectedIndex = -1;
                    return;
                }

                FriendlyName.Text = thisPdf.FriendlyName;
                SelectedFile.Content = thisPdf.FileName;
                Folder.SelectedIndex = -1;
            }
        }

        private void ShowSelectionFromDB(object sender, SelectionChangedEventArgs e)
        {
            Files.SelectedIndex = -1;

            using (var context = new OrganizerContext())
            {
                var thisPdf = (from p in context.PDFs
                               where p.FileName == (string)(((ListBox)sender).SelectedValue)
                               select p).FirstOrDefault();

                if (thisPdf == null)
                    return;

                FriendlyName.Text = thisPdf.FriendlyName;
                SelectedFile.Content = thisPdf.FileName;
                Folder.SelectedValue = thisPdf.Folder.Name;
            }
        }

        private void UpdateFoldersCombobox(object sender, EventArgs e)
        {
            Folder.Items.Clear();

            using (var context = new OrganizerContext())
            {
                var folders = context.Folders;

                foreach (Folder folder in folders)
                {
                    Folder.Items.Add(folder.Name);
                }

                ListBoxItem addFolder = new ListBoxItem();
                addFolder.Content = "Add folder...";
                Folder.Items.Add(addFolder);
            }
        }

        private string AddFolder()
        {
            TextInputDialog folderName = new TextInputDialog();
            folderName.Title = "Please enter a folder name";
            folderName.ShowDialog();
            if (folderName.OkClicked)
                return folderName.Text;
            else
                return null;
        }

        private void DeleteEntry(object sender, RoutedEventArgs e)
        {
            if (ORM.SelectedIndex == -1)
                return;

            using (var context = new OrganizerContext())
            {
                var thisPdf = (from p in context.PDFs
                               where p.FileName == (string)(ORM.SelectedValue)
                               select p).FirstOrDefault();

                if (thisPdf == null)
                    return;

                context.PDFs.Remove(thisPdf);

                int changes = context.SaveChanges();

                UpdateDbList();
            }
        }
    }
}