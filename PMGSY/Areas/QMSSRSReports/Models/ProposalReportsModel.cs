using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class ProposalReportsModel
    {
        public ProposalReportsModel()
        {
            STATE_NAME = PMGSYSession.Current.StateCode == 0 ? string.Empty : PMGSYSession.Current.StateName;
            DISTRICT_NAME = PMGSYSession.Current.DistrictCode == 0 ? string.Empty : PMGSYSession.Current.DistrictName;
            BLOCK_NAME = string.Empty;

        }

        public string STATE_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string BLOCK_NAME { get; set; }


        [Display(Name = "State")]
        public Nullable<int> MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        [Display(Name = "District")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS { get; set; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS { get; set; }
    }




    /// <summary>
    /// Added by SAMMED PATIL 14 JUNE 2014
    /// </summary>
    public class MRDProposalModel
    {
        public int State_Code { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid  State")]
        public int StateCode { get; set; }
        public string StateName { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid  District")]
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid  Block")]
        public int BlockCode { get; set; }
        public string BlockName { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Sanctioned Year")]
        [Required(ErrorMessage = "Please select Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid  Year")]
        public int Year { get; set; }
        public SelectList YearList { get; set; }
        public string YearName { get; set; }

        [Display(Name = "Batch")]
        [Required(ErrorMessage = "Please select Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid  Batch")]
        public int BatchCode { get; set; }
        public string BatchName { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Collaboration")]
        [Required(ErrorMessage = "Please select Collaboration")]
        [Range(-1, int.MaxValue, ErrorMessage = "Invalid  Collaboration")]
        public int CollabCode { get; set; }
        public string CollabName { get; set; }
        public List<SelectListItem> CollabList { get; set; }

      



        [Display(Name = "Agency")]
        [Required(ErrorMessage = "Please select Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Agency")]
        //[RegularExpression(@"^([0NU]+)$", ErrorMessage = "Invalid Status selected")]
        public int Agency { get; set; }
        public string AgencyName { get; set; }
        public List<SelectListItem> AgencyList { get; set; }

        public int Level { get; set; }
       
        public int PMGSY { get; set; }
        public string PTAStatus { get; set; }
        public string MRDStatus { get; set; }

     
    }

   
}
