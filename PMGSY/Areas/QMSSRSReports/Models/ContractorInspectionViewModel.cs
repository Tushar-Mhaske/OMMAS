using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class ContractorInspectionViewModel
    {
        public int RoleID { get; set; }
        public int MAST_STATE_CODE { get; set; }

        [Range(2000, 2099, ErrorMessage = "Please select valid From Year.")]
        public int fromYear { get; set; }
        public List<SelectListItem> lstFromYear { set; get; }

        [Range(2000, 2099, ErrorMessage = "Please select valid To Year.")]
        public int toYear { get; set; }
        public List<SelectListItem> lstToYear { set; get; }
    }
}