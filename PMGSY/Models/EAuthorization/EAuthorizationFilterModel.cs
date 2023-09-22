using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.EAuthorization
{
    public class EAuthorizationFilterModel
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
        public Int32 StatusID { get; set; }
        public Int32 ParentNdCode { get; set; }
        public Int16 LevelId { get; set; }
        public Int64 BillId { get; set; }
        public String Bill_type { get; set; }
        public String TransMode { get; set; }
        public String Deduction_Payment { get; set; }
        public String ChequeEpayNumber { get; set; }
        public String EAuthorizationStatus { get; set; }

        public String LoadStr { get; set; }
        //AckUnackFlag Added by Abhishek to identify Ack/Unack Operation
        public String AckUnackFlag { get; set; }
        public Int32 AuthID { get; set; }
        public int SrNo { get; set; }
        public Int16 StateCode { get; set; }
    }
}