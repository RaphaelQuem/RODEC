using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RODEC.Model
{
    public class Item
    {
        public string BarCode { get; set; }
        public int TaxSituation { get; set; }
        public string FiscalClassification { get; set; }
        public string Description { get; set; }
        public string SingleLabel { get; set; }
        public string CompanyCode { get; set; }
    }
}
