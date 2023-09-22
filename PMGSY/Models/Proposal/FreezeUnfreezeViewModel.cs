using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class FreezeUnfreezeViewModel
    {
        [Display(Name = "State Name")]
        public Nullable<int> MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        public string StateName { get; set; }

        [Display(Name = "District")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS { get; set; }

        public string DistrictName { get; set; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS { get; set; }

        public string BlockName { get; set; }

        [Display(Name = "Year")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        public string Year { get; set; }

        public int IMS_ROAD_CODE { get; set; }

        [Display(Name = "Road Name")]
        public string Road_Name { get; set; }

        [Display(Name = "LSB Name")]
        public string LSB_Name { get; set; }

        [Display(Name = "Length")]
        public decimal Length { get; set; }

        [Display(Name = "Award Status")]
        public string Award_Status { get; set; }

        [Display(Name = "Physical Progress")]
        public decimal Physical_Progress { get; set; }

        [Display(Name = "Financial Progress")]
        public Nullable<decimal> Financial_Progress { get; set; }

        public int RoleID { get; set; }
        public int UserLevelID { get; set; }

        public string Package_ID { get; set; }

    }
}