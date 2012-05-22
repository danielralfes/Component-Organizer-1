using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OrganizerDB;
using WPFEx;

/*
 * TODO:
 * TOP Priority:
 * 1: Write viewmodel
 * 2: Bind to viewmodel
 * 3: Blah blah, abstraction MVVM all that
 * n-1: Some SoC would be REALLY nice
 *
 * 4: Allow files to be added by drag and drop
 * 5: Add searching
 * 6: ...
 *
 * > Manufacturer
 *    > IC Type
 *       > IC
 *
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
                string desiredFileName = (string)(ORM.SelectedValue);

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
                    thisPart.Datasheet.FileName = (string)DatasheetTextbox.Text;

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

        private void TextBox_LostFocus_1(object sender, RoutedEventArgs e)
        {
            BrowseDatasheet.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void TextBox_GotFocus_1(object sender, RoutedEventArgs e)
        {
            BrowseDatasheet.Visibility = System.Windows.Visibility.Visible;
        }
    }
}