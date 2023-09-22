using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.AccountsReports
{
    public class ReportHeader
    {
        public string FormNo { get; set; }

        public string ReportParaGraphName { get; set; }

        public string ReportName { get; set; }

        public string CPWDNo { get; set; }

        public List<SP_ACC_Get_Report_Header_Information_Result> lstReportHeader { get; set; }
    }
}