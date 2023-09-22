using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Master
{
    public class MasterCarriageViewModel
    {           
        [UIHint("hidden")]
        public string EncryptedCarriageCode { get; set; }

        public int MAST_CARRIAGE_CODE { get; set; }

        [Display(Name = "Width")]        
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Width,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0.001, 9999.999, ErrorMessage = "Invalid Width.")]
        [Required(ErrorMessage = "Width is required.")]
        public decimal MAST_CARRIAGE_WIDTH { get; set; }

        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Invalid Status.")]
        public string MAST_CARRIAGE_STATUS { get; set; }

        public virtual ICollection<IMS_SANCTIONED_PROJECTS> IMS_SANCTIONED_PROJECTS { get; set; }

    }
}