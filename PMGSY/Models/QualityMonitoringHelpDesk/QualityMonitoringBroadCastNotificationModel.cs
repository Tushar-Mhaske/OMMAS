using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoringHelpDesk
{
    public class QualityMonitoringBroadCastNotificationModel
    {
        public QualityMonitoringBroadCastNotificationModel()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            State_LIST = objCommonFunctions.PopulateStates(false);
            State_LIST.Insert(0, (new SelectListItem { Text = "All State", Value = "0", Selected = true }));
        }
        [Display(Name = "Message Description")]
        [Required(ErrorMessage = "Message Description  is required.")]
        [StringLength(1000, ErrorMessage = "Message Description is ust be less than 1000 characters.")]
        // [RegularExpression(@"^(?![0-9]*$)[a-zA-Z0-9 ._!,-]+$", ErrorMessage = "Message Description is not in valid format.")]  //working
        public string MESSAGE_TEXT { get; set; }


        public bool IS_DOWNLOAD { get; set; }

        [Display(Name = "State")]
        [Range(0, 2147483647, ErrorMessage = "Please select State.")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> State_LIST { get; set; }

        public string QM_Type { get; set; }
        public int Message_Id { get; set; }
        public System.DateTime TIME_STAMP { get; set; }
        public int BroadCast_Id { get; set; }


    }
    public class USP_QM_HELPDESK_BROADCAST_NOTIFICATION_DETAILS_Result
    {

        public int Message_Id { get; set; }
        public string Message_Text { get; set; }
        public string Is_Download { get; set; }
        public string Created_Date { get; set; }
        public Nullable<int> BroadCast_Id { get; set; }
        public DateTime Time_Stamp { get; set; }

    }
    public class USP_QM_HELPDESK_GET_IMEI_DETAILS_Data_Result
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ImeiNo { get; set; }
        public int ADMIN_QM_CODE { get; set; }
        public string ApplicationMode { get; set; }
        public string MonitorName { get; set; }
        public string ADMIN_QM_TYPE { get; set; }

        //added by Hrishikesh to Provide "reset IMEI and ResetIMEI Count" Functionality to CQCAdmin Role
        public int ResetIMEICount { get; set; }
    }

    public class QM_HELPDESK_IMEI_MODEL_DETAILS
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ImeiNo { get; set; }
        public int ADMIN_QM_CODE { get; set; }
        public string ApplicationMode { get; set; }
        public string MonitorName { get; set; }
        public string ADMIN_QM_TYPE { get; set; }
    }

}