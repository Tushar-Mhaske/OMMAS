using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PMGSY.Models.Accounts
{
    public class ObservationModel
    {
        [Display(Name = "Observation Subject")]
        [Required(ErrorMessage = "Observation subject is required.")]
        [StringLength(2000, ErrorMessage = "subject can be upto 2000 character only.")]
        [RegularExpression(@"^([a-zA-Z0-9 .,/()-]+)$", ErrorMessage = "Subject is not in valid format.")]
        public String Subject { get; set; }

        [Display(Name = "Observation")]
        [Required(ErrorMessage = "Observation is required.")]
        [RegularExpression(@"^([a-zA-Z0-9 .,/()-]+)$", ErrorMessage = "Observation is not in valid format.")]
        [StringLength(4000, ErrorMessage = "Observation can be upto 4000 character only.")]
        public String Observation { get; set; }

        [Display(Name = "Non-conformance")]
        public String NonConforfance { get; set; }

        public int ParentId { get; set; }
        public String hdnMasterObId { get; set; }

    }
}