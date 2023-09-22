using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Models
{
    public class AddXmlFileUser
    {

        public int UserId { get; set; }

        public int MAST_STATE_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string QM_TYPE { get; set; }

        public int schMonth { get; set; }

        public int ToschMonth { get; set; }

        public int schYear { get; set; }
        public int ToMonth { get; set; }
        public int ToYear { get; set; }
        public int ToDate { get; set; }
    }
}