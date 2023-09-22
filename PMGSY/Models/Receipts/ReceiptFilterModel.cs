using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Receipts
{
    public class ReceiptFilterModel
    {
        public Int16 Month { get; set; }
        public Int16 Year { get; set; }
        public String FromDate { get; set; }
        public String ToDate { get; set; }
        public Int16 TransId { get; set; }
        public Int32 page { get; set; }
        public Int32 rows { get; set; }
        public String sidx { get; set; }
        public String sord { get; set; }

        public String FilterMode { get; set; }

        public String FundType { get; set; }
        public Int32 AdminNdCode { get; set; }
        public Int16 LevelId { get; set; }
        public String BillType { get; set; }

        public Int64 BillId { get; set; }

        public Int64 AssetBillId { get; set; }
        public Int64 LibBillId { get; set; }

        //Added got OB Imprest Settlement 10/07/2013
        public Int16 HeadId { get; set; }
    }
}