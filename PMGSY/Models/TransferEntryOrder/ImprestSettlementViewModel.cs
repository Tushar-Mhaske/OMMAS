using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.TransferEntryOrder
{
    public class ImprestSettlementViewModel
    {
        public long MAP_ID { get; set; }
        public long P_BILL_ID { get; set; }
        public Nullable<short> P_TXN_ID { get; set; }
        public long S_BIll_ID { get; set; }
        public Nullable<short> S_TXN_ID { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }

        public short Month { get; set; }
        public short Year { get; set; }
    }
}