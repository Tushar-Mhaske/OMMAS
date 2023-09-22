using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.TransferEntryOrder
{
    public class ListImprest
    {
        public long BILL_ID { get; set; }
        public string BILL_NO { get; set; }
        public System.DateTime BILL_DATE { get; set; }
        public string CHQ_NO { get; set; }
        public Nullable<System.DateTime> CHQ_DATE { get; set; }
        public decimal GROSS_AMOUNT { get; set; }
        public string PAYEE_NAME { get; set; }
        public Nullable<short> P_TXN_ID { get; set; }
        public long S_BIll_ID { get; set; }
        public Nullable<Decimal> MASTER_SETTLED_AMOUNT { get; set; }
        public Nullable<decimal> C_SETTLED_AMOUNT { get; set; }
        public Nullable<decimal> D_SETTLED_AMOUNT { get; set; }
        public string IS_FINALIZE { get; set; }
        public String BILL_TYPE { get; set; }
        public bool AddNewMaster { get; set; }
        public decimal Amount_Settled_By_TEO { get; set; }
       

    }
}