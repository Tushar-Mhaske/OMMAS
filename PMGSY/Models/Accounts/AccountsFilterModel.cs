using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Accounts
{
    public class AccountsFilterModel
    {
        public Int32 page { get; set; }
        public Int32 rows { get; set; }
        public String sidx { get; set; }
        public String sord { get; set; }
        public String FilterMode { get; set; }
        public String FundType { get; set; }
        public Int32 AdminNdCode { get; set; }
        public Int16 LevelId { get; set; }
        public String BillType { get; set; }
        public string AssetOrliability { get; set; }
        public Int16 month { get; set; }
        public Int16 year { get; set; }
        public Int16 lowercode { get; set; }
        public string ownLower { get; set; }
        public Int16 rptId { get; set; }
        public string selectioncode { get; set; }
        public Int16 masterHead { get; set; }
    }
}