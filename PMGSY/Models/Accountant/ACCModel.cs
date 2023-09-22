using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.Accountant.Models
{ 
    //added by Pradip Patil 4-1-2017 
     
    public class ACCModel
    {    
        [Display(Name="Role")]
       // [Required(ErrorMessage="Please Select Role")]
       // [Range(1,int.MaxValue,ErrorMessage="Please Select Role")]
        public int RoleCode { get; set; }
        public List<SelectListItem> RoleList { get; set; }
      
        [Display(Name = "SRRDA")]
        public int SRRDA { get; set; }
        public List<SelectListItem> SRRDAList { get; set; }

        [Display(Name = "DPIU")]
        //[Range(1,int.MaxValue,ErrorMessage="Please Select DPIU")]
        public int DPIU { get; set; }
        public List<SelectListItem> DPIUList { get; set; }
    }

 
}