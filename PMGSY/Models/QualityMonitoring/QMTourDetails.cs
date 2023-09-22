using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class QMTourDetails
    {

        //public ProposalFilterViewModel()
        //{
        //    BLOCKS = new List<SelectListItem>();
        //    Years = new List<SelectListItem>();
        //    BATCHS = new List<SelectListItem>();
        //    PROPOSAL_TYPES = new List<SelectListItem>();
        //    DISTRICTS = new List<SelectListItem>();
        //    STATES = new List<SelectListItem>();
        //    MORD_STATUS = new List<SelectListItem>();
        //    COLLABORATIONS = new List<SelectListItem>();
        //    SCHEMELIST = new List<SelectListItem>();
        //    SCHEMELIST.Insert(0, new SelectListItem { Value = "1" ,Text = "PMGSY-I"});
        //    SCHEMELIST.Insert(1, new SelectListItem { Value = "2", Text = "PMGSY-II" });
        //}

        public int UserLevelID { get; set; }
        public int RoleID { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please select Year.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid Year")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "Month")]
        [Required(ErrorMessage = "Please select Month.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid Month")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { set; get; }

        [Display(Name = "Monitor")]
        [Required(ErrorMessage = "Please select Monitor.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid Monitor")]
        public int Monitor { get; set; }
        public List<SelectListItem> MonitorList { get; set; }

        //[Display(Name = "Proposal Type")]
        //public string IMS_PROPOSAL_TYPE { get; set; }
        //public List<SelectListItem> PROPOSAL_TYPES { get; set; }

        ////[Display(Name = "Technology Proposed")]        
        ////public Nullable<int> IMS_STREAMS { get; set; }
        ////public List<SelectListItem> STREAMS { get; set; }

        //[Display(Name = "District")]
        //public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        //public List<SelectListItem> DISTRICTS { get; set; }

        //[Display(Name = "Proposal Status")]
        //public string IMS_PROPOSAL_STATUS { get; set; }
        //public List<SelectListItem> PROPOSAL_STATUS { get; set; }

        //[Display(Name = "State Name")]
        //public Nullable<int> MAST_STATE_CODE { get; set; }
        //public List<SelectListItem> STATES { get; set; }

        //public List<SelectListItem> MORD_STATUS { get; set; }

        //[Display(Name = "Funding Agency")]
        //public Nullable<int> IMS_COLLABORATION { get; set; }
        //public List<SelectListItem> COLLABORATIONS { get; set; }

        //[Display(Name = "Agency")]
        //public Nullable<int> IMS_AGENCY { get; set; }
        //public List<SelectListItem> AGENCIES { get; set; }

        //[Display(Name = "New / Upgradation")]
        //public string IMS_UPGRADE_COONECT { get; set; }
        //public List<SelectListItem> CONNECTIVITYLIST { get; set; }

        //[Display(Name = "Scheme")]
        //public int PMGSY_SCHEME { get; set; }
        //public List<SelectListItem> SCHEMELIST { get; set; }
    }
}