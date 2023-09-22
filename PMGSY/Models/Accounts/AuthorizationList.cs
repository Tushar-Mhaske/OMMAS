using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Accounts
{
    public class AuthorizationList
    {
        public long AUTH_ID { get; set; }
        public string AUTH_NO { get; set; }
        public System.DateTime AUTH_DATE { get; set; }
        public decimal CHQ_AMOUNT { get; set; }
        public decimal CASH_AMOUNT { get; set; }
        public decimal GROSS_AMOUNT { get; set; }
        public string CON_NAME { get; set; }
        public string AGG_NAME { get; set; }
        public string PKG_NAME { get; set; }
        public string ROAD_NAME { get; set; }
        public string FUND_TYPE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public string CURRENT_AUTH_STATUS { get; set; }
        public string REJECTION_REMARKS { get; set; }
   //     public System.DateTime AUTH_REJECTION_DATE { get; set; }

        public string AUTH_REJECTION_DATE { get; set; }
        public Nullable<Int64> BILL_ID { get; set; }

        public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        public virtual ACC_MASTER_LEVEL ACC_MASTER_LEVEL { get; set; }
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN { get; set; }
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual ACC_AUTH_REQUEST_TRACKING ACC_AUTH_REQUEST_TRACKING { get; set; }
    }
}