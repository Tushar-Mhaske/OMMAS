using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.OnlineFundRequest
{
    public class ConditionReplyViewModel
    {
        public int CONDITION_IMPOSED_ID { get; set; }
        public int REQUEST_ID { get; set; }
        public int CONDITION_ID { get; set; }
        public Nullable<int> CONDITION_IMPOSED_BY { get; set; }
        public Nullable<System.DateTime> CONDITION_IMPOSED_DATE { get; set; }
        public string CONDITION_STATUS { get; set; }
        
        [Display(Name="Reply")]
        [Required(ErrorMessage="Please Enter Reply")]
        public string CONDITION_REPLY { get; set; }
    }
}