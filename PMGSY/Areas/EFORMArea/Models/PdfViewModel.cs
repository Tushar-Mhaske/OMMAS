using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class PdfViewModel
    {
        public int id { get; set; }
        public string dateOfInsp { get; set; }
        public string qmName { get; set; }
        public string nqmOrsqm { get; set; }
        public string qmCode { get; set; }
        public string state { get; set; }
        public string District { get; set; }
       
        public Nullable<System.DateTime> date_of_inspection { get; set; }
        public string name_qualitymonitor { get; set; }
        public string nqm { get; set; }
        public string nqmcode { get; set; }
        public string sqm { get; set; }
        public string sqmcode { get; set; }
    
        public string district { get; set; }
        public string block { get; set; }
        public string name_of_road { get; set; }
        public string package_no { get; set; }
        public string sanctioned_length { get; set; }
        public string flexible_pavement { get; set; }
        public string rs_pavement { get; set; }
        public string executed_length { get; set; }
        public string eflexible_pavement { get; set; }
        public string ers_pavement { get; set; }
        public string reasons { get; set; }
        public string tech { get; set; }
        public string rd_from { get; set; }
        public string rd_to { get; set; }
        public string estimated_cost { get; set; }
        public string tech_cost { get; set; }
        public string awarded_cost { get; set; }
        public string expenditure_done { get; set; }
        public string bills_pending { get; set; }
        public string total_expenditure { get; set; }
        public string completion_cost { get; set; }
        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }

    }
}