using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Report
{
    public class ReportFormModel
    {
        [DisplayName("Select Month")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        public short Month { get; set; }
        public List<SelectListItem> ListMonth { get; set; }

        [DisplayName("Select Year")]
        [Range(2000, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }

        [DisplayName("Select PIU")]
        public int Piu { get; set; }
        public List<SelectListItem> ListPiu { get; set; }

        [DisplayName("Select Agency")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> ListAgency { get; set; }

        public int LevelId { get; set; }
        public int ReportType { get; set; }
        public int ReportLevel { get; set; }

    }

    public class IncomeAndExpenditureModel
    {
        //[DisplayName("Select State")]
        //public int State { get; set; }
        //public List<SelectListItem> ListState { get; set; }

        //[DisplayName("Funding Agency")]
        //public int FundingAgency { get; set; }
        //public List<MASTER_FUNDING_AGENCY> lstFundingAgency { get; set; }

        //[DisplayName("Head")]
        //[Range(1, Int16.MaxValue, ErrorMessage = "Please Select Head")]
        //public int HeadCode { get; set; }
        //public List<ACC_MASTER_HEAD> lstHead { get; set; }

        public int LevelId { get; set; }        
    }

}