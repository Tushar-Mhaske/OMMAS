using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.REAT.Models
{
    public class FundReceiptModel
    {
        public Int16 Month { get; set; }
        public Int16 Year { get; set; }
        public String FundType { get; set; }
        public Int32 AdminNdCode { get; set; }
        public Int16 LevelId { get; set; }
        public String BillType { get; set; }
        public Int64 BillId { get; set; }
        public Int16 TransId { get; set; }
        public Int32 page { get; set; }
        public Int32 rows { get; set; }
        public String sidx { get; set; }
        public String sord { get; set; }
    }
}