#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ProposalFilterViewModel.cs
        * Description   :   This View Model is Used in CBR Views ListProposal.cshtml
        * Author        :   Vikram Nandanwar        
        * Creation Date :   19/June/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;


namespace PMGSY.Models.Execution
{
    public class ProposalFilterViewModel
    {
        public ProposalFilterViewModel()
        {
            BLOCKS = new List<SelectListItem>();
            Years = new List<SelectListItem>();
            BATCHS = new List<SelectListItem>();
            PROPOSAL_TYPES = new List<SelectListItem>();
            PACKAGES = new List<SelectListItem>();
            lstBatchs = new List<SelectListItem>();
            lstCollaborations = new List<SelectListItem>();
            lstUpgradations = new List<SelectListItem>();


            DISTRICTS = new List<SelectListItem>();
            STATES = new List<SelectListItem>();
        }

        public string RoleName { get; set; }

        [Display(Name = "District")]
        public int MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS { get; set; }


        [Display(Name = "State")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }



        [Display(Name = "Financial Year")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }
        public List<SelectListItem> BATCHS { set; get; }

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

    public class ProposalFilterForITNOViewModel
    {
        public ProposalFilterForITNOViewModel()
        {
            lstDistricts = new List<SelectListItem>();
            BLOCKS = new List<SelectListItem>();
            Years = new List<SelectListItem>();
            BATCHS = new List<SelectListItem>();
            PROPOSAL_TYPES = new List<SelectListItem>();
            PACKAGES = new List<SelectListItem>();
            lstBatchs = new List<SelectListItem>();
            lstCollaborations = new List<SelectListItem>();
            lstUpgradations = new List<SelectListItem>();
        }

        [Display(Name = "District")]
        [Range(-1, Int32.MaxValue, ErrorMessage = "Please select District.")]
        public int District { get; set; }
        public List<SelectListItem> lstDistricts { get; set; }

        [Display(Name = "Financial Year")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }
        public List<SelectListItem> BATCHS { set; get; }

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

        public int ProposalCode { get; set; }
        public decimal EXEC_BASE_COURSE { get; set; }
        public decimal EXEC_CD_WORKS { get; set; }
        public decimal EXEC_COMPLETED { get; set; }
        public decimal EXEC_EARTHWORK_SUBGRADE { get; set; }
        public string EXEC_ISCOMPLETED { get; set; }
        public decimal EXEC_LSB_WORKS { get; set; }
        public decimal EXEC_MISCELANEOUS { get; set; }
        public decimal EXEC_PREPARATORY_WORK { get; set; }
        public int EXEC_PROG_MONTH { get; set; }
        public int EXEC_PROG_YEAR { get; set; }
        public decimal EXEC_SIGNS_STONES { get; set; }
        public decimal EXEC_SUBBASE_PREPRATION { get; set; }
        public decimal EXEC_SURFACE_COURSE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }

        public int MONTH { get; set; }
        public int YEAR { get; set; }
    }

}