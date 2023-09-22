using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.ExistingRoadsReports
{
    public class ExistingRoadsReportsViewModel
   
    {
        public ExistingRoadsReportsViewModel()
        {
            STATE_NAME = PMGSYSession.Current.StateCode == 0 ? string.Empty : PMGSYSession.Current.StateName;
            DISTRICT_NAME = PMGSYSession.Current.DistrictCode == 0 ? string.Empty : PMGSYSession.Current.DistrictName;
            BLOCK_NAME = string.Empty;
            CONST_NAME = string.Empty;
        }

        public string STATE_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string BLOCK_NAME { get; set; }
        public string CONST_NAME { get; set; }

        [Display(Name = "State")]
        public Nullable<int> MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        [Display(Name = "District")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS { get; set; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS { get; set; }

        public int MAST_CONSTITUENCY_CODE { get; set; }

        [Display(Name = "Constituency Type")]
        public string MAST_CONSTITUENCY_TYPE { get; set; }
        public List<SelectListItem> CONSTITUENCY_TYPES { get; set; }
    }
}