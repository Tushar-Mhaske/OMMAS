using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class ExecutionProgressMRD
    {
        public ExecutionProgressMRD()
        {
            STATES = new List<SelectListItem>();
            DISTRICTS = new List<SelectListItem>();
            BLOCKS = new List<SelectListItem>();
            Years = new List<SelectListItem>();
            BATCHS = new List<SelectListItem>();
            PROPOSAL_TYPES = new List<SelectListItem>();
            PACKAGES = new List<SelectListItem>();
            lstBatchs = new List<SelectListItem>();
            lstCollaborations = new List<SelectListItem>();
            lstUpgradations = new List<SelectListItem>();
        }

        [Display(Name = "Financial Year")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }
        public List<SelectListItem> BATCHS { set; get; }

        [Display(Name = "State")]
        [Range(0, 50, ErrorMessage = "Please select a valid State")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }


        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid District")]
        public int MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS { get; set; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS { get; set; }

        [Display(Name = "Proposal Type")]
        public string IMS_PROPOSAL_TYPE { get; set; }
        public List<SelectListItem> PROPOSAL_TYPES { get; set; }

        [Display(Name = "Stream")]
        public Nullable<int> IMS_STREAMS { get; set; }
        public List<SelectListItem> STREAMS { get; set; }

        public string filterParameters { get; set; }

        [Display(Name = "Package")]
        public string IMS_PACKAGE { get; set; }
        public List<SelectListItem> PACKAGES { get; set; }

        public string Urlparameter { get; set; }

        [Display(Name = "Batch")]
        public int Batch { get; set; }
        public List<SelectListItem> lstBatchs { get; set; }

        [Display(Name = "Funding Agency")]
        public int Collaboration { get; set; }
        public List<SelectListItem> lstCollaborations { get; set; }

        [Display(Name = "New / Upgrade")]
        public string UpgradeConnect { get; set; }
        public List<SelectListItem> lstUpgradations { get; set; }
    }
}