using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class QMJIViewModel
    {
        public QMJIViewModel()
        {
            
            PublicRepresentativeList = new List<SelectListItem>();
            StateList = new List<SelectListItem>();
        }

        public int UserLevelID { get; set; }
        public int RoleID { get; set; }

        
        [Display(Name = "State")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> StateList { get; set; }

        
        [Display(Name = "Public Representative")]
        public string PublicRepresentativeCode { get; set; } //A-All, P - MP, L-MLA, G-GPR, O-Other
        public List<SelectListItem> PublicRepresentativeList { set; get; }

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

        

    }
}