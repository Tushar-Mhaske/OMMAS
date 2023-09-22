using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.Models;

namespace PMGSY.Areas.RCTRC.Models
{
    public class RCTRCSuggestion
    {
        [UIHint("Hidden")]
        public string EncryptedSuggestionCode { get; set; }


        [Display(Name = "Contact Person")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Contact Person.")]
        public int ContactPersonID { get; set; }
        public List<SelectListItem> ContactPerson_List { get; set; }


        public List<RCTRC_MASTER_SUGGESTION> KeyAreaList { get; set; }


        public bool isStatus { get; set; }



        [Display(Name = "Contact Person")]
        [Range(0, int.MaxValue, ErrorMessage = "Select Contact Person.")]
        public int ContactPersonIDSearch { get; set; }

    
    }
}