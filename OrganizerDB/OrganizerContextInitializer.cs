using System.Collections.Generic;
using System.Data.Entity;

namespace OrganizerDB
{
    public class OrganizerContextInitializer : CreateDatabaseIfNotExists<OrganizerContext>
    {
        protected override void Seed(OrganizerContext context)
        {
            var defaultFolder = new Folder { FolderID = 0, Name = "Default", Path = "" };
            context.Folders.Add(defaultFolder);

            //var TI = new Folder { FolderID = 1, Name = "Texas Instruments", Path = @"K:\PDF\Texas Instruments" };
            //context.Folders.Add(TI);

            //var ST = new Folder { FolderID = 2, Name = "ST", Path = @"K:\PDF\ST" };
            //context.Folders.Add(ST);

            //var pdfs = new List<PDF>
            //{
            //    new PDF { PDFID = 1, FileName = "lm3429.pdf", FriendlyName = "LM3429Q1 N-Channel Controller for Constant Current LED Drivers", Folder = TI},
            //    new PDF { PDFID = 2, FileName = "snvs649d.pdf", FriendlyName = "LMZ14201 1A SIMPLE SWITCHER® Power Module with 42V Maximum Input Voltage", Folder = TI },
            //    new PDF { PDFID = 3, FileName = "CD00001887.pdf", FriendlyName = "TDA7293 120-volt, 100-watt, DMOS audio amplifier with mute and standby", Folder = ST },
            //};

            //pdfs.ForEach(c => context.PDFs.Add(c));

            context.SaveChanges();
        }
    }
}