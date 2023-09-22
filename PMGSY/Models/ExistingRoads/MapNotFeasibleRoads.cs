using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.ExistingRoads
{
    public class MapNotFeasibleRoads
    {
        public MapNotFeasibleRoads()
        {
            lstBlocks = new List<SelectListItem>();
            ReasonList = new List<SelectListItem>();
        }
        public int CUPL_PMGSY3_ID { get; set; }

        public string WorkName { get; set; }

        public string PackageName { get; set; }

        public int PLAN_CN_ROAD_CODE { get; set; }

        public int District { get; set; }

        public int Year { get; set; }
        public int Batch { get; set; }


        [Display(Name = "Block")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please select Block.")]
        [Required(ErrorMessage = "Please select Block.")]
        public int Block { get; set; }
        public List<SelectListItem> lstBlocks { get; set; }

        [Display(Name = "Reason")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Reason.")]
        public int ReasonCode { get; set; }
        public List<SelectListItem> ReasonList { get; set; }




        [Display(Name = "Request Remarks")]
        [Required(ErrorMessage = "Request Remarks can not be empty.")]
        [RegularExpression(@"^[a-zA-Z0-9 ._()&/]+$", ErrorMessage = "Request Remarks are not in valid format.")]
        [StringLength(200, ErrorMessage = "Request Remarks must be less than 200 characters.")]
        public string REQUEST_REMARKS_EXEMPTION { get; set; }


    }
}