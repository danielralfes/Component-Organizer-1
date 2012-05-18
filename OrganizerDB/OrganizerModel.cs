using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizerDB
{
    public class PDF
    {
        // Primary key
        public int PDFID { get; set; }

        public string FileName { get; set; }

        public string FriendlyName { get; set; }

        // Foreign key
        public int FolderID { get; set; }

        // Foreign key
        public int? PublisherID { get; set; }

        public virtual Folder Folder { get; set; }

        public virtual Publisher Publisher { get; set; }
    }

    public class Folder
    {
        // Primary key
        public int FolderID { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        // Navigation property
        public virtual ICollection<PDF> PDFs { get; private set; }
    }

    public class Publisher
    {
        // Primary key
        public int PublisherID { get; set; }

        public string Name { get; set; }

        // Navigation property
        public virtual ICollection<PDF> PDFs { get; private set; }
    }
}