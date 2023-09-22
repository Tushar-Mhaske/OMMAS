using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Report.Account
{
    

    public class ContractorLedgerHeader
    {
        public string FormNumber { get; set; }
        public string FundName { get; set; }
        public string ReportName { get; set; }
        public string ParagraphName { get; set; }
        public string PiuName { get; set; }
        public string SrrdaName { get; set; }
        public string ContractorName { get; set; }
        public string AgreementNumber { get; set; }

    }
    public class ContractorLedgerFooter
    {
        public double AdvancePayment { get; set; }
        public double SecuredAdvanced { get; set; }
        public double MobilisationAdvance { get; set; }
        public double MachineryAdvanced { get; set; }
        public double MaterialIssued { get; set; }
        public double GrossDebits { get; set; }
        public double GrossCredits { get; set; }
        public double TotalValue { get; set; }
    }

    public class ContractorLedger
    {
        [Display(Name = "DPIU")]
        [Range(1, short.MaxValue, ErrorMessage = "Please Select DPIU")]
        public short Dpiu { get; set; }
        public List<SelectListItem> PiuList { get; set; }
        [Display(Name = "Contractor")]
        [Range(1, short.MaxValue, ErrorMessage = "Please Select DPIU")]
        public short contractor { get; set; }
        public List<SelectListItem> ContractorList { get; set; }
        [Display(Name = "Agreement")]
        [Range(1, short.MaxValue, ErrorMessage = "Please Select DPIU")]
        public short agreement { get; set; }
        public List<SelectListItem> AgreementList { get; set; }
        public string FundType { get; set; }
        public int PiuNdCode { get; set; }
        public int SrrdaNdCode { get; set; }
        public int LevelId { get; set; }


    }

    public class ContractorLedgerList
    {
        public ContractorLedgerHeader header;
        public ContractorLedgerFooter footer;
        public List<SP_ACC_RPT_LEDGER_CONTRACTOR_WORK_Result> RecordList { get; set; }
       
    }
}