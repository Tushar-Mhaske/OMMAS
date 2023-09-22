using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.AnalysisSSRSReport.Models
{
    public class FinStaAnalysisProposalModel
    {
        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
      
        public string FundingAgencyName { get; set; }
        public string StatusName { get; set; }
        public string BatchName { get; set; }

        public int Mast_State_Code { get; set; }
        public int Mast_District_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int PhaseYear { get; set; }
        public int Batch { get; set; }
        public int Agency { get; set; }
        public int FundingAgency { get; set; }
        public int Population { get; set; }

        public string Proposal { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Drop { get; set; }
        public string ReportType { get; set; }
        public string Report { get; set; }
        public string Sanctioned { get; set; }
        public string Progress { get; set; }
       
    }
}