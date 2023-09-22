using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace PMGSY.Models.AccountsReports
{
    public class ContractorLedgerModel
    {

        public Int32 SRRDACode { get; set; }

        public Int32 PIUCode { get; set; }

        public Int32 ContractorId { get; set; }

        public Int32 AggrementId { get; set; }

        public int LevelId { get; set; }
        public  List<SP_ACC_RPT_LEDGER_CONTRACTOR_WORK_Result> lstContractorLedger { get; set; }

        public List<SelectListItem> ddlPIU { get; set; }

        public List<SelectListItem> ddlContractor { get; set; }

        public List<SelectListItem> ddlAggrement { get; set; }

        public string ReportNumber { get; set; }

        public string ReportName { get; set; }

        public string ReportPara { get; set; }

        public string FundType { get; set; }

        public string PIUName { get; set; }

        public string ContCmpName { get; set; }

        public string AggrementNo { get; set; }

        public string FundName { get; set; }

        
    }
}