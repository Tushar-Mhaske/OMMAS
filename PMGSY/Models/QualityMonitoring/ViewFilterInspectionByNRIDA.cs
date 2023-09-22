using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class ViewFilterInspectionByNRIDA
    {
        public ViewFilterInspectionByNRIDA()
        {
            STATES = new List<SelectListItem>();
            DISTRICTS = new List<SelectListItem>();
            BLOCKS = new List<SelectListItem>();
            Years = new List<SelectListItem>();
            BATCHS = new List<SelectListItem>();

            SCHEMELIST = new List<SelectListItem>();
            SCHEMELIST.Insert(0, new SelectListItem { Value = "0", Text = "All Schemes" });
            SCHEMELIST.Insert(1, new SelectListItem { Value = "1", Text = "PMGSY-I" });
            SCHEMELIST.Insert(2, new SelectListItem { Value = "2", Text = "PMGSY-II" });
            SCHEMELIST.Insert(3, new SelectListItem { Value = "3", Text = "RCPLWE" });
            SCHEMELIST.Insert(4, new SelectListItem { Value = "4", Text = "PMGSY-III" });
            PROPOSAL_TYPES = new List<SelectListItem>();
            PROPOSAL_TYPES.Insert(0, new SelectListItem { Value = "A", Text = "All" });
            PROPOSAL_TYPES.Insert(1, new SelectListItem { Value = "P", Text = "Road" });
            PROPOSAL_TYPES.Insert(2, new SelectListItem { Value = "L", Text = "Bridges" });
            PROPOSAL_TYPES.Insert(3, new SelectListItem { Value = "B", Text = "Building" });
        }

        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid state")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        [Display(Name = "State")]
        public string MAST_STATE_NAME { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid district")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS { get; set; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS { get; set; }

        [Display(Name = "Year")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }
        public List<SelectListItem> BATCHS { set; get; }

        [Display(Name = "Proposal Type")]
        public string IMS_PROPOSAL_TYPE { get; set; }
        public List<SelectListItem> PROPOSAL_TYPES { get; set; }

        [Display(Name = "Scheme")]
        public int PMGSY_SCHEME { get; set; }
        public List<SelectListItem> SCHEMELIST { get; set; }

        public int ROLE_CODE { get; set; }
    }
}