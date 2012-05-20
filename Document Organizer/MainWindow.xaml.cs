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

/*
 * TODO:
 * 1: Make it do inventory tracking
 * 2: Add more fields
 * 3: Allow files to be added by drag and drop
 * 4: Take advantage of WPF's data binding
 * 5: Hide tracked files in filesystem view
 * 6: Add searching
 * 7: Treeview should be MVVM
 * 8: ...
 *
 * n-1: Some SoC would be REALLY nice
 * n: Work on GUI & UX
 *
 */

namespace Document_Organizer
{
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
                path.Title = "Please select the root Datasheet directory";
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

            // TODO: Need to be able to update this list without clearing the items
            //! ^^ AKA USE DATABINDING

            Database.SetInitializer<OrganizerContext>(new OrganizerContextInitializer());
            using (var context = new OrganizerContext())
            {
                foreach (var manufacturer in context.Manufacturers)
                {
                    TreeViewItem folderItem = new TreeViewItem();
                    folderItem.Header = manufacturer.Name;
                    foreach (var part in manufacturer.Parts)
                    {
                        TreeViewItem partItem = new TreeViewItem();
                        partItem.Header = part.PartName;
                        folderItem.Items.Add(partItem);
                    }
                    ORM.Items.Add(folderItem);
                }
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

                var thisPart = (from p in context.Parts
                                where p.Datasheet.FileName == desiredFileName
                                select p).FirstOrDefault();

                bool newPDF = false;
                if (thisPart == null)
                {
                    thisPart = new Part();
                    newPDF = true;
                }

                if ((string)(Manufacturer.SelectedItem) == "Add manufacturer...")
                {
                    string folderName = AddManufacturer();
                    if (folderName == null)
                        return;

                    // Create a new folder
                    Manufacturer f = new Manufacturer();
                    f.Name = folderName;
                    context.Manufacturers.Add(f);

                    // Add this PDF to the new folder
                    thisPart.Manufacturer = f;

                    // Select new folder in combobox
                    Manufacturer.SelectedIndex = Manufacturer.Items.IndexOf(f.Name);
                }
                else
                    thisPart.Manufacturer = (from f in context.Manufacturers
                                             where f.Name == (string)Manufacturer.SelectedValue
                                             select f).FirstOrDefault();

                if (thisPart.Manufacturer == null)
                    thisPart.Manufacturer = (from f in context.Manufacturers
                                             where f.Name == "Default"
                                             select f).First();

                thisPart.PartName = PartName.Text;

                if (newPDF)
                {
                    thisPart.Datasheet = new Datasheet();
                    thisPart.Datasheet.FileName = (string)Files.SelectedValue;

                    context.Parts.Add(thisPart);
                }

                int changes = context.SaveChanges();

                UpdateDbList();
            }
        }

        private void ShowSelectionFromFiles(object sender, SelectionChangedEventArgs e)
        {
            using (var context = new OrganizerContext())
            {
                var thisPdf = (from p in context.Parts
                               where p.Datasheet.FileName == (string)(((ListBox)sender).SelectedValue)
                               select p).FirstOrDefault();

                if (thisPdf == null)
                {
                    PartName.Text = "";
                    Manufacturer.SelectedIndex = -1;
                    return;
                }

                PartName.Text = thisPdf.PartName;
                Manufacturer.SelectedIndex = -1;
            }
        }

        private void ShowSelectionFromDB(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Files.SelectedIndex = -1;

            if (sender == null)
                return;

            TreeView _sender = sender as TreeView;
            if (_sender == null)
                return;

            string selectedName = null;

            if (_sender.SelectedItem is string)
                selectedName = (string)_sender.SelectedItem;
            else if (_sender.SelectedItem is TreeViewItem)
                selectedName = (string)((TreeViewItem)_sender.SelectedItem).Header;

            using (var context = new OrganizerContext())
            {
                var thisPdf = (from p in context.Parts
                               where p.PartName == selectedName
                               select p).FirstOrDefault();

                if (thisPdf == null)
                    return;

                PartName.Text = thisPdf.PartName;
                Manufacturer.SelectedValue = thisPdf.Manufacturer.Name;
            }
        }

        private void UpdateManufacturersCombobox(object sender, EventArgs e)
        {
            Manufacturer.Items.Clear();

            using (var context = new OrganizerContext())
            {
                var manufacturers = context.Manufacturers;

                foreach (Manufacturer manufacturer in manufacturers)
                {
                    Manufacturer.Items.Add(manufacturer.Name);
                }

                Manufacturer.Items.Add("Add manufacturer...");
            }
        }

        private string AddManufacturer()
        {
            TextInputDialog manufacturerName = new TextInputDialog();
            manufacturerName.Title = "Please enter a manufacturer name";
            manufacturerName.ShowDialog();
            if (manufacturerName.OkClicked)
                return manufacturerName.Text;
            else
                return null;
        }

        private void DeleteEntry(object sender, RoutedEventArgs e)
        {
            //if (ORM.SelectedIndex == -1)
            //    return;

            using (var context = new OrganizerContext())
            {
                var thisPdf = (from p in context.Datasheets
                               where p.FileName == (string)(ORM.SelectedValue)
                               select p).FirstOrDefault();

                if (thisPdf == null)
                    return;

                context.Datasheets.Remove(thisPdf);

                int changes = context.SaveChanges();

                UpdateDbList();
            }
        }
    }
}