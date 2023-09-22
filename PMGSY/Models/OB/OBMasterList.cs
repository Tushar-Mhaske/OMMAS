using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.OB
{
    public class OBMasterList
    {

        public long BILL_ID { get; set; }
        public string BILL_NO { get; set; }
        public short BILL_MONTH { get; set; }
        public short BILL_YEAR { get; set; }
        public System.DateTime BILL_DATE { get; set; }
        public string TXN_DESC { get; set; }
        public decimal GROSS_AMOUNT { get; set; }
        public decimal? DETAILS_AMOUNT { get; set; }        
        public string BILL_FINALIZED { get; set; }
        public string ACTION_REQUIRED { get; set; }
    }
}