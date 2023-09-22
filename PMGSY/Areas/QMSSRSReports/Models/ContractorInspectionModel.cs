using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class ContractorInspectionModel
    {
        public int RoleID { get; set; }
        public int MAST_STATE_CODE { get; set; }
        
        
        [Display(Name = "Sanction Year")]
        public int SANCTION_YEAR { get; set; }
        public SelectList SANCTION_YEAR_LIST { set; get; }

        
        [Display(Name = "Road Status")]
        public string ROAD_STATUS { get; set; }
        public List<SelectListItem> ROAD_STATUS_LIST { get; set; }

        

    }
}