using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class InterStateMapping
    {
        public string StateName { get; set; }
        
        [Display(Name = "Type")]
        public string QM_TYPE_CODE { set; get; }
        public List<SelectListItem> QM_TYPES { set; get; }

        [Display(Name="State")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        public InterStateMapping()
        {
            QM_TYPES = new List<SelectListItem>();
            STATES = new List<SelectListItem>();
        }

        [Required(ErrorMessage = "Please select Monitor.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Monitor.")]

        [Display(Name = "Monitor")]
        public int[] ADMIN_QM_CODE { get; set; }

        public MultiSelectList MONITORSList
        {
            get
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                List<SelectListItem> MONITORS = new List<SelectListItem>();
                MONITORS = objCommonFunctions.PopulateAllMonitorsForInterstate("false", "S", PMGSYSession.Current.StateCode);
                return new SelectList(MONITORS, "Value", "Text", this.ADMIN_QM_CODE);
            }
        }

    }
}