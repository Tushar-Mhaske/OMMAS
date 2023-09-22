using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
namespace PMGSY.Models.Master
{
    public class MasterPMGSY2ViewModel
    {
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please Select State.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "State is not in valid format.")]  
        public int MAST_STATE_CODE { get; set; }

        [Display(Name = "Status")]        
        [RegularExpression("[YN]", ErrorMessage = "Status is not in valid format.")]
        public string MAST_PMGSY2_ACTIVE { get; set; }

        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}