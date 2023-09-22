using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EformQCOffViewModel
    {
        public string OFFICIAL_TYPE { get; set; }
        public string OFFICIAL_NAME { get; set; }
        public string PAN { get; set; }
        public string IDENTITY_NUMBER { get; set; }
        public string MOBILE_NO { get; set; }
        public string EMAIL_ID { get; set; }
        public Nullable<System.DateTime> FROM_DATE { get; set; }
        public Nullable<System.DateTime> TO_DATE { get; set; }
    }
}