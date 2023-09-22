using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Feedback
{
    public class FBRepStatusNew
    {

        public int RoleCode { get; set; }

 //select * from omms.UM_Role_Master
 //8 SQC
 //22 PIU 

    //    public string isFinalReplyByPIU { get; set; }
        public string isFinalReplyByPIU { get; set; }

        public string AddButtonShow { get; set; }

        [Display(Name = "To")]
        public string ToAdd { get; set; }

        [Display(Name = "Date")]
        public string Date { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Reply status")]
        public string RepStat { get; set; }


        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Please Enter Reply")]
        [Display(Name = "Reply")]
        public string Reply { get; set; }

        [Required(ErrorMessage = "Please Enter Interim Reply")]
        [Display(Name = "Interim Reply")]
        public string Interim1 { get; set; }

        [Display(Name = "Interim Reply")]
        public string Interim2 { get; set; }

        [Display(Name = "Final Reply")]
        public string Final { get; set; }

        public string hdnfeedId { get; set; }

        public string hdnappr { get; set; }

        public string hdnstate { get; set; }
        public string hdndist { get; set; }
        public string hdnblock { get; set; }

        public List<String> interimList { get; set; }


        public int? FBState { get; set; }

        public string fbapprDate { get; set; }

        public int hdRole { get; set; }
        public string hdRepStatus { get; set; }

        public bool IS_PMGSY_ROAD { get; set; }

        public int hdnStateCode { get; set; }
        public int hdnDistrictCode { get; set; }
        public int hdnBlockCode { get; set; }





    }
}