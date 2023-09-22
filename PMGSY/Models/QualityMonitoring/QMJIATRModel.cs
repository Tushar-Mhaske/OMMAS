using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class QMJIATRModel
    {
        
        public int jiATRFileCode { get; set; }

        [Required]
        public int jiCode { get; set; }

        [Required(ErrorMessage = "Action Taken Date is required")]
        [Display(Name = "Action Taken Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Action Taken Date is not in valid format")]
        public string ActionTakendDate { get; set; }

        [Required(ErrorMessage = "ATR Status is required")]
        [Display(Name = "Select ATR Status (Provisional/Final)")]
        public string ATRStatus { get; set; }

        [Required(ErrorMessage = "Action Taken File is required")]
        [Display(Name="Select Action Taken File")]
        public string ATRFileName { get; set; }

        [Required(ErrorMessage = "ATR Remarks required")]
        [RegularExpression(@"^[a-zA-Z0-9 ,_().]+$", ErrorMessage = "Remarks contain Invalid characters")]
        public string remarks { get; set; }

        public string qmATRStatus { get; set; }

        public string prevActionTakendDate { get; set; }
    }
}