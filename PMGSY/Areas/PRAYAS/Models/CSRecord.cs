using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CsRecord
/// </summary>
namespace CSModels
{

    // This file contains all the required Classes and their properties.


    public class InputParam
    {
        public int Instance_Code { get; set; }
        public int Sec_Code { get; set; }
        public int Ministry_Code { get; set; }
        public int Dept_Code { get; set; }
        public long Project_Code { get; set; }
        
    }





    // =====================KML============================
    public class KPIData
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
    

    public class Masterdata
    {
        public int Instance_Code { get; set; }
        public int Sec_Code { get; set; }
        public int Ministry_Code { get; set; }
        public int Dept_Code { get; set; }
        public long Project_Code { get; set; }
        public int Frequency_Id { get; set; }
        public int atmpt { get; set; }
        public IList<KPIDetails> ListKpidata { get; set; }
    }
    public class KPIDetails
    {
        public int Group_Id { get; set; }
        public string datadate { get; set; }
        public string KValue { get; set; }
        public string LValue { get; set; }


    }

    public class ProjrctKpiDetails
    {
        public InputParam IP { get; set; }
        public string EncyptedData { get; set; }
    }


}
