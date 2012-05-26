using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
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
        private OrganizerContext context;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += WindowLoaded;
            this.Closing += WindowClosing;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.context.Dispose();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Database.SetInitializer<OrganizerContext>(new OrganizerContextInitializer());
            context = new OrganizerContext();
            context.Manufacturers.Load();
            //context.Datasheets.Load();
            //context.Parts.Load();
            this.DataContext = context.Manufacturers.Local;
            ORM.ItemsSource = context.Manufacturers.Local;

            SetPDFPath();
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

        string mainPath;

        private void UpdateManufacturersCombobox(object sender, EventArgs e)
        {
            Manufacturer.Items.Clear();

            var manufacturers = context.Manufacturers;

            foreach (Manufacturer manufacturer in manufacturers)
            {
                Manufacturer.Items.Add(manufacturer);
            }

            Manufacturer.Items.Add("Add manufacturer...");
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

        private void SaveEntry(object sender, RoutedEventArgs e)
        {
            if (Manufacturer.Text == "Add manufacturer...")
            {
                Manufacturer man = new Manufacturer() { Name = AddManufacturer() };
                Manufacturer.SelectedItem = man;
            }

            if (ORM.SelectedItem is Part)
            {
                context.SaveChanges();
            }
            else if (ORM.SelectedItem is Manufacturer)
            {
                // TODO Something here...
            }
        }

        private void DeleteEntry(object sender, RoutedEventArgs e)
        {
            if (ORM.SelectedItem is Part)
            {
                Part selected = (Part)ORM.SelectedItem;
                Manufacturer man = selected.Manufacturer;
                man.Parts.Remove(selected);
            }
            else if (ORM.SelectedItem is Manufacturer)
            {
                // TODO: Make sure the user wants to delete all entries for this manufacturer
                Manufacturer selected = (Manufacturer)ORM.SelectedItem;
                context.Manufacturers.Remove(selected);
            }

            context.SaveChanges();
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