using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.ChequeBook
{
    public class FilterModel
    {
        public short MAST_MONTH_CODE { get; set; }
        public string MAST_MONTH_FULL_NAME { get; set; }
        public short MAST_YEAR_CODE { get; set; }
        public string MAST_YEAR_TEXT { get; set; }
    }
}