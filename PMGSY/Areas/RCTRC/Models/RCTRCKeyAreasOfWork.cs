using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.Models;
namespace PMGSY.Areas.RCTRC.Models
{
    public class RCTRCKeyAreasOfWork
    {

        [UIHint("Hidden")]
        public string EncryptedKeyAreaCode { get; set; }

     //   public List<USP_GET_RCTRC_MASTER_APPLICATIONS_Result> MASTER_APPLICATIONS_LIST { get; set; }

        [Display(Name = "Contact Person")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Contact Person.")]
        public int ContactPersonID { get; set; }
        public List<SelectListItem> ContactPerson_List { get; set; }


        //[Display(Name = "Graduation")]
        //[Range(1, int.MaxValue, ErrorMessage = "Select Graduation.")]
    //     public int KeyAreaCode { get; set; }
        public List<RCTRC_MASTER_WORK_AREA> KeyAreaList { get; set; }


        public bool isPastExp { get; set; }
        public bool isPresentWork { get; set; }
        public bool isPlanfor5Years { get; set; }

        [Display(Name = "Contact Person")]
        [Range(0, int.MaxValue, ErrorMessage = "Select Contact Person.")]
        public int ContactPersonIDSearch { get; set; }
    }
}