using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.PFMS
{
    public class UpdateBeneficiaryIFSCViewModel
    {
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "No Special Characters Allowed.")]
        [Required(ErrorMessage = "Please Enter PAN Number")]
        public string contractorPAN { set; get; }

    }
}