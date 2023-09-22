using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.MABQMS
{
    public class MABQMSModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RoleId { get; set; }
        public string Password { get; set; }
        public string AdminQmCode { get; set; }
        public string QmType { get; set; }
        public string Status { get; set; }
        public string MonitorName { get; set; }
    }

    public class MABQMSConfigParamsModel
    {
        public string MABQMS_FIXED_URL { get; set; }
        public string MABQMS_APP_URL { get; set; }
        public string MABQMS_APK_URL { get; set; }
        public string MABQMS_VERSION_NO { get; set; }
        public string NQM_MAX_IMAGE_CNT { get; set; }
        public string SQM_MAX_IMAGE_CNT { get; set; }
        public string MABQMS_HELPLINE_NO { get; set; }
        public string MABQMS_CRASH_REPORT_URL { get; set; }
        public string MABQMS_CRASH_EMAIL { get; set; }
        public string MABQMS_APP_MODE { get; set; }
        public string MABQMS_TRAINING_BATCH { get; set; }
        public string MABQMS_NOTIFICATION_MESSAGE { get; set; } 
    }

}