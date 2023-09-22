using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace PMGSY.Areas.RCTRC.Models
{
    public class RCTRCStateUserMapping
    {
        [UIHint("Hidden")]
        public string EncryptedRCTRCCode { get; set; }

        
        // [Display(Name = "State")]
      //  [Required(ErrorMessage = "Please select State. ")]
    //    [Range(1, int.MaxValue, ErrorMessage = "Please select State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        public bool isStatus { get; set; }
       
        [Display(Name = "User")]

        [Range(1, int.MaxValue, ErrorMessage = "Please select User.")]
        public int UserID { get; set; }
        public List<SelectListItem> UserList { get; set; }



    }
}