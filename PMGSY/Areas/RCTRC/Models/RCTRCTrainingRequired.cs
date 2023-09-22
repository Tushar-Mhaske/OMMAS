using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.Models;
namespace PMGSY.Areas.RCTRC.Models
{
    public class RCTRCTrainingRequired
    {
        [UIHint("Hidden")]
        public string EncryptedTrainingRequiredCode { get; set; }

        [Display(Name = "Contact Person")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Contact Person.")]
        public int ContactPersonID { get; set; }
        public List<SelectListItem> ContactPerson_List { get; set; }

        public List<RCTRC_MASTER_TRAINING> KeyAreaList { get; set; }


        public bool isRPD { get; set; }
        public bool isEYC { get; set; }
        public bool isFAI { get; set; }

        [Display(Name = "Contact Person")]
        [Range(0, int.MaxValue, ErrorMessage = "Select Contact Person.")]
        public int ContactPersonIDSearch { get; set; }
    }
}