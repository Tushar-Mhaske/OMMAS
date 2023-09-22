using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Report
{
    public class CBPaymentModel
    {
        public Nullable<Decimal> TotalPayCash { get; set; }
        public Nullable<Decimal> TotalPayBank { get; set; }
        public UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result ClosingBalace { get; set; }
        public List<SP_ACC_CASHBOOK_PAYMENT_SIDE_Result> ListPaymentCB { get; set; }      
    }
}