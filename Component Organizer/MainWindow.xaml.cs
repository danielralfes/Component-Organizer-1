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
 * n: Work on GUI & UX
 *
 */

namespace Component_Organizer
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
            if (ORM.SelectedItem is Part)
            {
                Part selected = (Part)ORM.SelectedItem;
                context.Parts.Remove(selected);
                //Manufacturer man = selected.Manufacturer;
                //man.Parts.Remove(selected);
            }
            else if (ORM.SelectedItem is Manufacturer)
            {
                // TODO: Make sure the user wants to delete all entries for this manufacturer
                Manufacturer selected = (Manufacturer)ORM.SelectedItem;
                if (selected.Parts.Count > 0)
                {
                    MessageBoxResult mr = MessageBox.Show("Are you sure you want to delete this manufacturer and ALL of this manufacturer's parts?",
                        "Delete manufacturer", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);

                    if(mr == MessageBoxResult.Yes)
                        context.Manufacturers.Remove(selected);
                }
                else
                {
                    context.Manufacturers.Remove(selected);
                }
            }

            context.SaveChanges();
        }

        private void SaveEntry(object sender, RoutedEventArgs e)
        {
            if (Manufacturer.Text == "Add manufacturer...")
            {
                Manufacturer man = new Manufacturer() { Name = AddManufacturer() };
                context.Manufacturers.Add(man);
                Manufacturer.SelectedItem = man;

                if (ORM.SelectedItem is Part)
                {
                    Part selected = (Part)ORM.SelectedItem;
                    selected.Manufacturer = man;
                }
            }

            if (ORM.SelectedItem is Manufacturer)
            {
                Manufacturer man = (Manufacturer)ORM.SelectedItem;
                Part newPart = new Part();
                newPart.Manufacturer = man;
                context.Parts.Add(newPart);

                // TODO: Add part data
            }

            context.SaveChanges();
        }

        private void TextBox_GotFocus_1(object sender, RoutedEventArgs e)
        {
            //BrowseDatasheet.Visibility = System.Windows.Visibility.Visible;
        }

        private void TextBox_LostFocus_1(object sender, RoutedEventArgs e)
        {
            //BrowseDatasheet.Visibility = System.Windows.Visibility.Collapsed;
        }

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
        }

        private void BrowseDatasheet_Click_1(object sender, RoutedEventArgs e)
        {
            FileSelectionDialog datasheetFilebroser = new FileSelectionDialog();
            datasheetFilebroser.Title = "Please select the datasheet";
            datasheetFilebroser.CheckFileExists = true;
            datasheetFilebroser.ShowDialog();
            if (datasheetFilebroser.OkClicked)
            {
                if(ORM.SelectedItem is Part)
                {
                    Part sel = (Part)ORM.SelectedItem;
                    Datasheet ds = sel.Datasheet;
                    if(ds == null)
                    {
                        ds = new Datasheet();
                        context.Datasheets.Add(ds);
                        sel.Datasheet = ds;
                    }

                    sel.Datasheet.FileName = datasheetFilebroser.FileName;
                }
            }
        }
    }
}