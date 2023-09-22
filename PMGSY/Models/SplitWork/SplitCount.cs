using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PMGSY.Models.SplitWork
{
    public class SplitCount
    {

        [UIHint("Hidden")]
        public string EncryptedIMSPRRoadCode { get; set; }

        [Display(Name = "Total Split")]
        [Required(ErrorMessage = "Total Split is required.")]
        [Range(2, 20, ErrorMessage = "Total Split must be greater than 1 upto 20 works.")]
        public int? IMS_TOTAL_SPLIT { get; set; }
        

        public string IMS_SPLIT_STATUS { get; set; }

    }
}