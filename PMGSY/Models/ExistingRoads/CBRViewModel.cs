using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.ExistingRoads
{
    public class CBRViewModel
    {
        [UIHint("hidden")]
        public string EncryptedCBRCode{ get; set; }

        public string Operation { get; set; }

        public int MAST_ER_ROAD_CODE { get; set; }

        [Display(Name = "Segment Number")]
        public int MAST_SEGMENT_NO { get; set; }

        [Display(Name = "Start Chainage(in Kms.)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Start Chainage ,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0, 9999.999, ErrorMessage = "Invalid Start Chainage.")]
        public decimal MAST_STR_CHAIN { get; set; }

        [Display(Name = "End Chainage(in Kms.)")]
        [Required]
        [Range(0, 9999.999, ErrorMessage = "Invalid End Chainage.")]
        [CompareValidation("MAST_STR_CHAIN",ErrorMessage="End Chainage must be greater than Start Chainage.")]
        [CompareEndChainageValidation("MAST_STR_CHAIN", "TotalAvailableRoadLength", "EncryptedCBRCode", ErrorMessage = "Segment Length exceeds the Remaining Length.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid End Chainage ,Can only contains Numeric values and 3 digits after decimal place")]
        public decimal MAST_END_CHAIN { get; set; }


        [Display(Name = "CBR Value")]
        [Required]
        [RegularExpression("[0-9]{1,2}", ErrorMessage = "Invalid CBR value,Can only contains Numeric values.")]      
        [Range(1,30, ErrorMessage = "CBR Value must be between 1 to 30")]
        public int MAST_CBR_VALUE { get; set; }

        [Display(Name = "Segment Length")]
        public decimal? Segment_Length { get; set; }

        [Display(Name = "Road Number")]
        public string RoadID { get; set; }

        [Display(Name = "Road Name")]
        public string RoadName { get; set; }

        public decimal RoadLength { get; set; }

        public decimal EndChainage { get; set; }

        [Display(Name = "Remaining Length: ")]
        public decimal? TotalAvailableRoadLength { get; set; }

        [Display(Name = "Total Entered Segment Length: ")]
        public decimal EnteredSegmentLength { get; set; }
       
        public virtual MASTER_EXISTING_ROADS MASTER_EXISTING_ROADS { get; set; }

    }
}