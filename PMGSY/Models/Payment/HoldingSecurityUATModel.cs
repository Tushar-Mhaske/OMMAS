using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Payment
{
    public class HoldingSecurityUATModel
    {
        public Int32 StateCode { get; set; }
        public Int32 AdminNdCode { get; set; }
    }
    public class SDOBAllowTxnModel
    {

        public Int32 StateCode { get; set; }
        public Int32 AdminNdCode { get; set; }

        public Int32 maxAllowCount { get; set; }
    }

    public class HoldingTranfserInitDateModel
    {

        public Int32 StateCode { get; set; }
        public Int32 ParentNdCode { get; set; }

        public string startDate { get; set; }
    }
}