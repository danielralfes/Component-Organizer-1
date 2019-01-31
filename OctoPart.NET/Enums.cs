using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoPart
{
    public class Enums
    {
        public enum TypeSearch
        {
            [Description("parts/search")]
            SearchPartName,
            [Description("parts/match")]
            SearchBoom,
        };

        public enum FieldSearch
        {
            mpn,
            mpn_or_sku,
            brand
        }
    }
}
