using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.RESTImplementationForABAProgress.Models
{
    public class JsonFormat
    {
        public bool status { get; set; }
        public List<PMGSY.Models.GET_DATA_FROM_ABA_MIS_DATA_SERVICE_Result> Result{get;set;}
    }
}