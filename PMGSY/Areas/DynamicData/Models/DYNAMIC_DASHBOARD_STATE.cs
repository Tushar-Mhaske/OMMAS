﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.DynamicData.Models
{
    public class DYNAMIC_DASHBOARD_STATE
    {

        //public int ID { get; set; }
        //public int SCODE { get; set; }
        public string STATE_NAME { get; set; }

        //public int DCODE { get; set; }
        public string DISTRICT_NAME { get; set; }

        public string BLOCK_NAME { get; set; }

        public string AWARD_STATUS { get; set; }

        public int SCHEME { get; set; }
        public string WTYPE { get; set; }

        //public int WCODE { get; set; }
        public decimal RLENGTH { get; set; }
        public decimal BLENGTH { get; set; }
        public string CONNECTTYPE { get; set; }
        //public string SANCTION { get; set; }
        public decimal SCOST { get; set; }
        public int SYEAR { get; set; }
        public Nullable<System.DateTime> SDDATE { get; set; }
        public string STAGED { get; set; }
        public string SPHASE { get; set; }
        public string PACKAGE { get; set; }

        //public Nullable<decimal> RPREPATORY { get; set; }
        //public Nullable<decimal> REARTHWORK { get; set; }
        //public Nullable<decimal> RBASECOURSE { get; set; }
        //public Nullable<decimal> RSUBBASE { get; set; }
        //public Nullable<decimal> RSURFACE { get; set; }
        //public Nullable<decimal> RCLENGTH { get; set; }
        public string RISCOMPLETED { get; set; }
        public Nullable<decimal> RBALANCE { get; set; }
        public Nullable<int> RPYEAR { get; set; }
        public Nullable<int> RPMONTH { get; set; }
        public Nullable<System.DateTime> RCOMPLETIONDATE { get; set; }

        //public Nullable<int> RLPROGRESS { get; set; }
        public Nullable<int> BPMONTH { get; set; }
        public Nullable<int> BPYEAR { get; set; }
        public Nullable<System.DateTime> BCOMPLETIONDATE { get; set; }

        //public Nullable<int> BLPROGRESS { get; set; }
        public Nullable<int> H1 { get; set; }
        public Nullable<int> H2 { get; set; }
        public Nullable<int> H3 { get; set; }
        public Nullable<int> H4 { get; set; }
        public Nullable<int> CH1 { get; set; }
        public Nullable<int> CH2 { get; set; }
        public Nullable<int> CH3 { get; set; }
        public Nullable<int> CH4 { get; set; }
        public Nullable<decimal> EXPENDITURE { get; set; }
        public Nullable<System.DateTime> FBILLDATE { get; set; }
        public Nullable<int> FBILLSTATUS { get; set; }
        public string WORK_NAME { get; set; }
        public Nullable<System.DateTime> GENERATED_DATE { get; set; }
    }
}