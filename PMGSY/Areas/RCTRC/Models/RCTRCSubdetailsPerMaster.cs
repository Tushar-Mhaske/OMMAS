using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.Models;
namespace PMGSY.Areas.RCTRC.Models
{
    public class RCTRCSubdetailsPerMaster
    {

        [UIHint("Hidden")]
        public string EncryptedSubdetailsPerMasterCode { get; set; }

        public List<RCTRC_MASTER_KEY_AREA> KeyAreaList { get; set; }

        public string masterAreaName { get; set; }
        public bool isAbilityToWork { get; set; }

        [Display(Name = "Contact Person")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Contact Person.")]
        public int ContactPersonID { get; set; }
        public List<SelectListItem> ContactPerson_List { get; set; }



        //[Display(Name = "Master Area Work")]
        //[Range(1, int.MaxValue, ErrorMessage = "Select Master Area Work.")]
        public int masterWorkAreaCode { get; set; }
        //public List<SelectListItem> masterWorkAreaList { get; set; }




        [Display(Name = "Contact Person")]
        [Range(0, int.MaxValue, ErrorMessage = "Select Contact Person.")]
        public int ContactPersonIDSearch { get; set; }

    }
}