﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RODEC.Model
{
    public class Item
    {
        public decimal BarCode { get; set; }
        public string TaxSituation { get; set; }
        public string FiscalClassification { get; set; }
        public string Description { get; set; }
        public string SingleLabel { get; set; }
        public string CompanyCode { get; set; }
        public decimal Percentage { get; set; }
    }
}
