using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Report
{
    public class CBReceiptModel
    {
        public Nullable<Decimal> TotalRecCash { get; set; }
        public Nullable<Decimal> TotalRecBank { get; set; }
        public UDF_ACC_GEN_GET_BA_CASH_Opening_Balances_Result OpeningBalace { get; set; }
        public List<SP_ACC_CASHBOOK_RECEIPT_SIDE_Result> ListReceiptCB { get; set; }        
    }
}