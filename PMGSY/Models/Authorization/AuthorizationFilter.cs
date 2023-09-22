using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Authorization
{
    public class AuthorizationFilter
    {
        public Int32 AdminNdCode { get; set; }
        public Int16 Month { get; set; }
        public Int16 Year { get; set; }
        public Int32 page { get; set; }
        public Int32 rows { get; set; }
        public String sidx { get; set; }
        public String sord { get; set; }
        public String FundType { get; set; }
        public Int16 LevelId { get; set; }
    }
}