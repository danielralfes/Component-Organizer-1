using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrganizerDB
{
    public class Part : INotifyPropertyChanged
    {
        // Primary key
        [Key]
        public int PartID { get; set; }

        public string PartName { get; set; }

        public string Description { get; set; }

        public string DistributorURL { get; set; }

        public string ManufacturerURL { get; set; }

        public int QuantityAvailable { get; set; }

        public int? DatasheetID { get; set; }

        public int? ManufacturerID { get; set; }

        public virtual Datasheet Datasheet { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Datasheet : INotifyPropertyChanged
    {
        // Primary key
        [Key]
        public int DatasheetID { get; set; }

        public string FileName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Manufacturer : INotifyPropertyChanged
    {
        // Primary key
        [Key]
        public int ManufacturerID { get; set; }

        public string Name { get; set; }

        // Navigation property
        public virtual ICollection<Part> Parts { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}