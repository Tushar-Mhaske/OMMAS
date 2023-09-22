using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class QMFilterViewModel
    {

        public QMFilterViewModel()
        {
            QM_TYPES = new List<SelectListItem>();
            STATES = new List<SelectListItem>();
            MONITORS = new List<SelectListItem>();
        }

        public int UserLevelID { get; set; }
        public int RoleID { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessage = "Please select QM Type.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please select QM Type.")]
        public string QM_TYPE_CODE { set; get; }
        public List<SelectListItem> QM_TYPES { set; get; }

        [Display(Name = "State")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        public int MAST_DISTRICT_CODE { get; set; }

        [Display(Name = "Monitor")]
        public int ADMIN_QM_CODE { get; set; }
        public List<SelectListItem> MONITORS { set; get; }

        [Display(Name = "From Month")]
        public int FROM_MONTH { get; set; }
        public List<SelectListItem> FROM_MONTHS_LIST { set; get; }

        [Display(Name = "To Month")]
        public int TO_MONTH { get; set; }
        public List<SelectListItem> TO_MONTHS_LIST { set; get; }

        [Display(Name = "From Year")]
        public int FROM_YEAR { get; set; }
        public List<SelectListItem> FROM_YEARS_LIST { set; get; }

        [Display(Name = "To Year")]
        public int TO_YEAR { get; set; }
        public List<SelectListItem> TO_YEARS_LIST { set; get; }

        [Display(Name = "ATR Status")]
        public string ATR_STATUS { get; set; }
        public List<SelectListItem> ATR_STATUS_LIST { set; get; }

        [Display(Name = "Road Status")]
        public string ROAD_STATUS { get; set; }
        public List<SelectListItem> ROAD_STATUS_LIST { get; set; }

        [Display(Name = "Duration")]
        public int ATR_SUBMIT_DURATION { get; set; }
        public List<SelectListItem> ATR_SUBMIT_DURATION_LIST { get; set; }

        [Display(Name = "Empanelled")]
        public string IsEmpanelled { get; set; }
        public List<SelectListItem> EmpanelledList { get; set; }

        //added by Ajinkya 
        public int schemeType { get; set; }
        public List<SelectListItem> schemeList { get; set; }

        public string imsSanctioned { get; set; }
        public List<SelectListItem> imsSanctionedList { get; set; }


    }
}