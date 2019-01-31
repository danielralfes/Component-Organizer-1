using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using OctoPart;
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
 * 5: Implement searching
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
        private OctoPartFetcherV2 lookup;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += WindowLoaded;
            this.Closing += WindowClosing;
            lookup = new OctoPartFetcherV2();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Database.SetInitializer<OrganizerContext>(new OrganizerContextInitializer());
            context = new OrganizerContext();
            context.Manufacturers.Load();
            this.DataContext = context.Manufacturers.Local;
            ORM.ItemsSource = context.Manufacturers.Local;
            datagrid.ItemsSource = context.Parts.Local;

            // TODO: Rewrite this as LINQ
            for(int i = 0; i < datagrid.Columns.Count; i++)
            {
                var header = datagrid.Columns[i].Header;
                if(header is string)
                {
                    if(((string)header).Contains("ID"))
                    {
                        datagrid.Columns[i].Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.context.Dispose();
        }

        private string AddManufacturer()
        {
            TextInputDialog manufacturerName = new TextInputDialog();
            manufacturerName.Title = "Please enter a manufacturer name";
            manufacturerName.ShowDialog();
            if (manufacturerName.DialogResult == MessageBoxResult.OK)
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

                    if (mr == MessageBoxResult.Yes)
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

        private void BrowseDatasheet_Click_1(object sender, RoutedEventArgs e)
        {
            FileSelectionDialog datasheetFilebroser = new FileSelectionDialog();
            datasheetFilebroser.Title = "Please select the datasheet";
            datasheetFilebroser.CheckFileExists = true;
            datasheetFilebroser.ShowDialog();
            if (datasheetFilebroser.DialogResult == MessageBoxResult.OK)
            {
                if (ORM.SelectedItem is Part)
                {
                    Part sel = (Part)ORM.SelectedItem;
                    Datasheet ds = sel.Datasheet;
                    if (ds == null)
                    {
                        ds = new Datasheet();
                        context.Datasheets.Add(ds);
                        sel.Datasheet = ds;
                    }

                    sel.Datasheet.FileName = datasheetFilebroser.FileName;
                }
            }
        }

        private void HandlePartLookup(object sender, RoutedEventArgs e)
        {
            if (!(ORM.SelectedItem is Part))
                return;

            AppBusy.IsBusy = true;

            Part ourPart = (Part)ORM.SelectedItem;
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() => PartLookup(ourPart))
                .ContinueWith(w => AppBusy.IsBusy = false, new CancellationToken(), TaskContinuationOptions.None, scheduler);
        }

        private void PartLookup(Part part)
        {

            // TODO: Create a OctoPartFetcher.FillInInfo(Part p) method

            part.Description = lookup.GetDescription(part.PartName);
            part.Pins = lookup.GetPinCount(part.PartName);
            part.Package = lookup.GetPackage(part.PartName);
            part.Price = lookup.GetAveragePrice(part.PartName);
            part.ManufacturerURL = lookup.GetManufacturer(part.PartName);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TestV3();
        }

        private void TestV3()
        {
            OctoPartFetcher octo    = new OctoPartFetcher();
            OctoPartFetcherV3 teste = new OctoPartFetcherV3();

            teste._ParametricSearch(octo.APIKEY,"tip122g");
            teste._ParametricSearch(octo.APIKEY,"IRFZ44NPBF-ND");
            teste._ParametricSearch(octo.APIKEY, "solid state relay");
            teste._BOMMatchingTeste(octo.APIKEY);

        }
    }
}