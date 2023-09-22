using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Execution
{
    public class ExecutionAdditionalRoadDetails
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public int IMS_TRANSACTION_CODE { get; set; }

        [UIHint("Hidden")]
        public string EncryptedTransactionRoadCode { get; set; }

        [UIHint("Hidden")]
        public string EncryptedRoadCode { get; set; }

        [Display(Name = "Work Completion Date")]
        public Nullable<System.DateTime> WORK_COMPLETION_DATE { get; set; }

        [Display(Name = "Execution Approved Date")]
        public System.DateTime EXEC_APPROVED_DATE { get; set; }

        [Display(Name = "File")]
        [Required(ErrorMessage = "Please select File to upload")]
        public string FILE_NAME { get; set; }

        public string FILE_TYPE { get; set; }

        [Display(Name = "Year")]
        public int IMS_YEAR { get; set; }

        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }

        [Display(Name = "Package Number")]
        public string IMS_PACKAGE_ID { get; set; }

        [Display(Name = "Road Name")]
        public string IMS_ROAD_NAME { get; set; }

        [Display(Name = "Pavement Length")]
        public Nullable<decimal> IMS_PAV_LENGTH { get; set; }

        [Display(Name = "State Amount")]
        public Nullable<decimal> IMS_STATE_AMOUNT_TEXT { get; set; }

        [Display(Name = "Mord Amount")]
        public Nullable<decimal> IMS_MORD_AMOUNT_TEXT { get; set; }

    }
}
