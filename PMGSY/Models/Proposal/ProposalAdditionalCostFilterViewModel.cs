using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class ProposalAdditionalCostFilterViewModel
    {
        public ProposalAdditionalCostFilterViewModel()
        {
            BLOCKS = new List<SelectListItem>();
            Districts = new List<SelectListItem>();
            States = new List<SelectListItem>();
            Years = new List<SelectListItem>();
            BATCHS = new List<SelectListItem>();
            PROPOSAL_TYPES = new List<SelectListItem>();
            PACKAGES = new List<SelectListItem>();
            lstBatchs = new List<SelectListItem>();
            lstCollaborations = new List<SelectListItem>();
            lstUpgradations = new List<SelectListItem>();
            //DISTRICTS = new List<SelectListItem>();
            //STATES = new List<SelectListItem>();
            //MORD_STATUS = new List<SelectListItem>();
        }

        //public int UserLevelID { get; set; }
        //public int RoleID { get; set; }


        
        [Display(Name = "Financial Year")]        
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }
        public List<SelectListItem> BATCHS { set; get; }

        [Display(Name = "State")]
        public int MAST_State_CODE { get; set; }
        public List<SelectListItem> States { get; set; }

        [Display(Name = "District")]
        public int MAST_District_CODE { get; set; }
        public List<SelectListItem> Districts { get; set; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS { get; set; }

        [Display(Name="Proposal Type")]
        public string IMS_PROPOSAL_TYPE { get; set; }
        public List<SelectListItem> PROPOSAL_TYPES { get; set; }

        [Display(Name = "Stream")]        
        public Nullable<int> IMS_STREAMS { get; set; }
        public List<SelectListItem> STREAMS { get; set; }

        public string filterParameters { get; set; }

        [Display(Name="Package")]
        public string IMS_PACKAGE { get; set; }
        public List<SelectListItem> PACKAGES { get; set; }

        public string Urlparameter { get; set; }

        //new filters added by Vikram 

        [Display(Name="Batch")]
        public int Batch { get; set; }
        public List<SelectListItem> lstBatchs { get; set; }

        [Display(Name="Funding Agency")]
        public int Collaboration { get; set; }
        public List<SelectListItem> lstCollaborations { get; set; }

        [Display(Name="New / Upgrade")]
        public string UpgradeConnect { get; set; }
        public List<SelectListItem> lstUpgradations { get; set; }

        //end of change




        //[Display(Name="District")]
        //public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        //public List<SelectListItem> DISTRICTS { get; set; }

        //[Display(Name="Proposal Status")]
        //public string IMS_PROPOSAL_STATUS { get; set;}
        //public List<SelectListItem> PROPOSAL_STATUS { get; set; }

        //[Display(Name="State Name")]
        //public Nullable<int> MAST_STATE_CODE { get; set; }
        //public List<SelectListItem> STATES { get; set; }

        //public List<SelectListItem> MORD_STATUS { get; set; }
    }
}