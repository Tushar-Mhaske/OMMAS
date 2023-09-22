#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ReportsFilterViewModel.cs
        * Description   :   This View Model is Used in Filtering Different Reports Data - ReportsFilters.cshtml
        * Author        :   Shyam Yadav   
        * Creation Date :   28/August/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.ReportsLayout
{
    public class ReportsFiltersViewModel
    {
        public ReportsFiltersViewModel()
        {
            BLOCKS = new List<SelectListItem>();
            Years = new List<SelectListItem>();
            BATCHS = new List<SelectListItem>();
            PROPOSAL_TYPES = new List<SelectListItem>();
            DISTRICTS = new List<SelectListItem>();
            STATES = new List<SelectListItem>();
            COLLABORATIONS = new List<SelectListItem>();
        }

        [Display(Name="State")]
        public Nullable<int> MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        [Display(Name="District")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS { get; set; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS { get; set; }

        [Display(Name = "Year")]        
        public int YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "Month")]        
        public int MONTH { get; set; }
        public List<SelectListItem> Months { set; get; }

        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }
        public List<SelectListItem> BATCHS { set; get; }

        [Display(Name="Proposal Type")]
        public string IMS_PROPOSAL_TYPE { get; set; }
        public List<SelectListItem> PROPOSAL_TYPES { get; set; }

        [Display(Name="Proposal Status")]
        public string IMS_PROPOSAL_STATUS { get; set;}
        public List<SelectListItem> PROPOSAL_STATUS { get; set; }

        [Display(Name = "Funding Agency")]
        public Nullable<int> IMS_COLLABORATION { get; set; }
        public List<SelectListItem> COLLABORATIONS { get; set; }
    
    }


    public class ReportMenuMasterModel
    {
        //public List<ReportMenuMasterModel> ReportMenuList { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public int ParentId { get; set; }
        public short VerticalLevel { get; set; }
        public short Sequence { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }

    public class ReportMenuListModel
    {
        public List<ReportMenuMasterModel> ReportParentMenusList { get; set; }
        public List<ReportMenuMasterModel> ReportChildMenusList { get; set; }
        public int CountOfChildren { get; set; }
    }
}