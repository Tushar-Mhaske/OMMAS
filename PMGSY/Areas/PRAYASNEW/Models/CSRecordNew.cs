using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PMGSY.Areas.PRAYASNEW.Models
{
    public class CSRecordNew
    {


        public class InputParamNew
        {
            public int Instance_Code { get; set; }
            public int Sec_Code { get; set; }
            public int Ministry_Code { get; set; }
            public int Dept_Code { get; set; }
            public long Project_Code { get; set; }

        }





        // =====================KML============================
        public class KPIDataNew
        {
            public int Instance_Code { get; set; }
            public int Sec_Code { get; set; }
            public int Ministry_Code { get; set; }
            public int Dept_Code { get; set; }
            public long Project_Code { get; set; }
            public int Frequency_Id { get; set; }
            public int atmpt { get; set; }
            public string KValue { get; set; }
            public string LValue { get; set; }
            public int Group_Id { get; set; }
            public string datadate { get; set; }
        }


        public class MasterdataNew
        {
            public int Instance_Code { get; set; }
            public int Sec_Code { get; set; }
            public int Ministry_Code { get; set; }
            public int Dept_Code { get; set; }
            public long Project_Code { get; set; }
            public int Frequency_Id { get; set; }
            public int atmpt { get; set; }
            public IList<KPIDetailsNew> ListKpidata { get; set; }
        }
        public class KPIDetailsNew
        {
            public int Group_Id { get; set; }
            public string datadate { get; set; }
            public string KValue { get; set; }
            public string LValue { get; set; }


        }

        public class ProjrctKpiDetailsNew
        {
            public InputParamNew IP { get; set; }
            public string EncyptedData { get; set; }
        }


    }
}