using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Receipts
{
    public class MasterTransViewModel
    {
        public MasterTransViewModel()
        {
            this.ACC_AUTH_REQUEST_MASTER = new HashSet<ACC_AUTH_REQUEST_MASTER>();
            this.ACC_BILL_MASTER = new HashSet<ACC_BILL_MASTER>();
            this.ACC_MASTER_TXN1 = new HashSet<ACC_MASTER_TXN>();
            this.ACC_TXN_HEAD_MAPPING = new HashSet<ACC_TXN_HEAD_MAPPING>();
        }
    
        public string TXN_ID { get; set; }
        public Nullable<short> TXN_PARENT_ID { get; set; }
        public string CASH_CHQ { get; set; }
        public string BILL_TYPE { get; set; }
        public string TXN_DESC { get; set; }
        public string TXN_NARRATION { get; set; }
        public string FUND_TYPE { get; set; }
        public bool IS_OPERATIONAL { get; set; }
        public byte TXN_ORDER { get; set; }
        public byte OP_LVL_ID { get; set; }
    
        public virtual ICollection<ACC_AUTH_REQUEST_MASTER> ACC_AUTH_REQUEST_MASTER { get; set; }
        public virtual ICollection<ACC_BILL_MASTER> ACC_BILL_MASTER { get; set; }
        public virtual ACC_MASTER_BILL_TYPE ACC_MASTER_BILL_TYPE { get; set; }
        public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        public virtual ICollection<ACC_MASTER_TXN> ACC_MASTER_TXN1 { get; set; }
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN2 { get; set; }
        public virtual ACC_SCREEN_DESIGN_PARAM_DETAILS ACC_SCREEN_DESIGN_PARAM_DETAILS { get; set; }
        public virtual ACC_SCREEN_DESIGN_PARAM_MASTER ACC_SCREEN_DESIGN_PARAM_MASTER { get; set; }
        public virtual ICollection<ACC_TXN_HEAD_MAPPING> ACC_TXN_HEAD_MAPPING { get; set; }


    }
}