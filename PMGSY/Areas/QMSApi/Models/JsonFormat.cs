using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.QMSApi.Models
{
    public class JsonFormat
    {

        public bool status { get; set; }
        public List<PMGSY.Models.USP_MABQMS_DOWNLOAD_SCHEDULE_Result> Result { get; set; }
    }
}