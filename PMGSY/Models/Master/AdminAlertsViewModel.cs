using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Master
{
    public class AdminAlertsViewModel
    {
        [UIHint("hidden")]
        public string EncryptedAlertId { get; set; }
        
        public int ALERT_ID { get; set; }
        
        [Display(Name="Heading")]
        //[Required(ErrorMessage = "Please enter heading.")]
        [RegularExpression(@"^([a-zA-Z0-9 ()]+)$", ErrorMessage = "Only Alpanumeric Characters are allowed.")]                  
        public string ALERT_HEADING { get; set; }

        [Display(Name = "Subject")]
        [Required(ErrorMessage = "Please enter subject.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._':,\r\n&()-/]+)$", ErrorMessage = "Subject is not in valid format.")]          
        public string ALERT_TEXT { get; set; }
        
        public System.DateTime INSERTION_DATE { get; set; }
        
        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Please enter start date.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format")]
        public String DISPLAY_START_DATE { get; set; }
        
        [Display(Name = "End Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End Date is not in valid format")]
        [DateValidationVST("DISPLAY_START_DATE", ErrorMessage = "End Date must be greater than or equal to Start Date.")]        
        public String DISPLAY_END_DATE { get; set; }

        [Display(Name = "Status")]        
        public string ALERT_STATUS { get; set; }
    }
}