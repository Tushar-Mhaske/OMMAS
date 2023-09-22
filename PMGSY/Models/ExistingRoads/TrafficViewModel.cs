using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PMGSY.Controllers;

namespace PMGSY.Models.ExistingRoads
{
    public class TrafficViewModel
    {
        // U- Update , A-Add
        public string Operation { get; set; }

        [UIHint("hidden")]
        public string EncryptedErRoadCode { get; set; }

        public int MAST_ER_ROAD_CODE { get; set; }

        [Display(Name = "Year")]
        [Required]
        [Range(1950, 2099, ErrorMessage = "Please Select Year.")]      
        public int MAST_TI_YEAR { get; set; }

        [Display(Name = "Total Motarised Traffic/day")]
        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Total Motarised Traffic/day,Can only contains Numeric values")]
        [Range(1, 99999999, ErrorMessage = "Total Motarised Traffic/day must be a positive number.")]
        [CompareValidation("MAST_COMM_TI", ErrorMessage = "Total Motarised Traffic/Day should be greater than Commercial Vehicle Traffic/Day.")]
        public int MAST_TOTAL_TI { get; set; }

        [Display(Name = "Commercial Vehicle Traffic/day ")]
        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Commercial Vehicle Traffic/day,Can only contains Numeric values")]
        [Range(1, 99999999, ErrorMessage = "Commercial Vehicle Traffic/day must be a positive number.")]
        public int MAST_COMM_TI { get; set; }

        [Display(Name = "Road Number")]
        public string RoadNumber { get; set; }

        [Display(Name = "Road Name")]
        public string RoadName { get; set; }

        [Display(Name = "Block Name")]
        public string BlockName { get; set; }


        [Display(Name = "Traffic Catagory")]
        public string CurveType { get; set; }


        public virtual MASTER_EXISTING_ROADS MASTER_EXISTING_ROADS { get; set; }

    }
}