using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class VerificationModelList
    {
        public VerificationModelList()
        {
            this.VerificationList = new List<VerificationModelDetails>();


            VerificationModelDetails VerificationListDetails = new VerificationModelDetails();
            VerificationListDetails.VerificationType = "V";
            VerificationListDetails.RowCount = 4;
            this.VerificationList.Add(VerificationListDetails);

        }

        public List<VerificationModelDetails> VerificationList { get; set; }
    }
   
    public class VerificationModelDetails
    {
        public string VerificationType { get; set; }
        public int RowCount { get; set; }
    }


    //add at 28-07-2022

    public class VerificationModelListTempV2_0
    {
        public VerificationModelListTempV2_0()
        {
            this.VerificationList = new List<VerificationModelDetailsTemp2_0>();


            VerificationModelDetailsTemp2_0 VerificationListDetails = new VerificationModelDetailsTemp2_0();
            VerificationListDetails.VerificationType = "V";
            VerificationListDetails.RowCount = 12;
            this.VerificationList.Add(VerificationListDetails);

        }

        public List<VerificationModelDetailsTemp2_0> VerificationList { get; set; }
    }

    public class VerificationModelDetailsTemp2_0
    {
        public string VerificationType { get; set; }
        public int RowCount { get; set; }
    }
    //end add at 28-07-2022
}