using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Feedback
{
    public class FeedbackReply
    {

        public string Feed_Reply { get; set; }

        [Required(ErrorMessage="Please enter comments for Reply")]
        [StringLength(8000, ErrorMessage = "Reply cannot be greater than 8000 characters.")]
        [RegularExpression(@"^([-0-9a-zA-Z\u0900-\u097F0-9,.@()/ ]+)$", ErrorMessage = "Reply has some invalid characters.")]
        public string Rep_Comments { get; set; }

        public string hdnFBId { get; set; }
        public int hdnRepId { get; set; }
        public string hdnRepStatus { get; set; }
        
        public int hdnRepStateCode { get; set; }
        public int hdnRepDistrictCode { get; set; }
        public string hdnDBOpr { get; set; }

        public string hdnStateType { get; set; }

        public int hdnRole { get; set; }


        [Display(Name = "ETA (Tentative Timeline to resolve the complaint)")]
        [Required(ErrorMessage = "Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Maintenance End Date must be in dd/mm/yyyy format.")]
        //  [DateValidationVST("CurrentDate", ErrorMessage = "Tender Form Issue Start Date must be greater than or equal to current date.")]
        public string TIMELINE_DATE { get; set; }


        public string Is_Action_Taken { get; set; }




    }
}