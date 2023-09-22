using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Menu.ChartOfAccounts
{
    public class ChartOfAccountsModel
    {

        public int HEAD_ID { get; set; }
        public string HEAD_CODE { get; set; }
        public string HEAD_CODE_REF { get; set; }
        public string HEAD_NAME { get; set; }
        public Nullable<int> PARENT_HEAD_ID { get; set; }
        public String PARENT_HEAD_Name { get; set; }
        public string FUND_TYPE { get; set; }
        public string CREDIT_DEBIT { get; set; }
        public Nullable<byte> OP_LVL_ID { get; set; }
        public bool IS_OPERATIONAL { get; set; }
        public string EntryToBeMadeBy { get; set; }
    }
}