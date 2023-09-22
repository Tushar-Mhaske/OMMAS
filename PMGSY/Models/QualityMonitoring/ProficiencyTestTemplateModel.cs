using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class ProficiencyTestTemplateModel
    {
        public int ID { get; set; }
        public string MONITOR_NAME { get; set; }
        public int ADMIN_QM_CODE { get; set; }
        public string MOBILE_NUMBER { get; set; }
        public string EMAIL { get; set; }
        public string MONITOR_STATUS { get; set; }
        public int? MARKS { get; set; }


    }
}