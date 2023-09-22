using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Common
{
    public class SendMailCustomFuncModel
    {
        public String EmailRecepient { get; set; }
        public String EmailCC { get; set; } 
        public String EmailBCC { get; set; }
        public String EmailSubject { get; set; }
        public String EmailDate { get; set; }
        public String RecepientName { get; set; }
        public String RecepientDesignation { get; set; }
        public String RecepientAddress { get; set; }
        public String RecepientState { get; set; }

        public String AttachedFilePath { get; set; }

        public int RoleCode { get; set; }

        public bool IsFeedBackMail { get; set; }
        public List<SP_FEEDBACK_WEEKLY_REPORT_Result> MobileWeeklyReportList { get; set; }
        public List<SP_FEEDBACK_WEEKLY_REPORT_Result> WebWeeklyReportList { get; set; }
    }
}