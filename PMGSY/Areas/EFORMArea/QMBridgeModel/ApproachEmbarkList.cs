using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
     
        public class ApproachEmbarkList
        {
            public ApproachEmbarkList()
            {
              this.EmbarkList = new List<ApproachEmbarkModelDetails>();
              ApproachEmbarkModelDetails EmbarkModelDetails = new ApproachEmbarkModelDetails();
              EmbarkModelDetails.RowCount = 3;
              this.EmbarkList.Add(EmbarkModelDetails);

            }

            public List<ApproachEmbarkModelDetails> EmbarkList { get; set; }
        }

        public class ApproachEmbarkModelDetails
        {
           
            public int RowCount { get; set; }
        }




    public class ApproachSubBaseList
    {
        public ApproachSubBaseList()
        {
            this.SubBaseList = new List<ApproachSubBaseModelDetails>();
            ApproachSubBaseModelDetails SubBaseModelDetails = new ApproachSubBaseModelDetails();
            SubBaseModelDetails.RowCount = 3;
            this.SubBaseList.Add(SubBaseModelDetails);

        }

        public List<ApproachSubBaseModelDetails> SubBaseList { get; set; }
    }

    public class ApproachSubBaseModelDetails
    {

        public int RowCount { get; set; }
    }


    public class ApproachBaseCourseList
    {
        public ApproachBaseCourseList()
        {
            this.BaseCourseList = new List<ApproachBaseCourseModelDetails>();
            ApproachBaseCourseModelDetails BaseCourseModelDetails = new ApproachBaseCourseModelDetails();
            BaseCourseModelDetails.RowCount = 3;
            this.BaseCourseList.Add(BaseCourseModelDetails);

        }

        public List<ApproachBaseCourseModelDetails> BaseCourseList { get; set; }
    }

    public class ApproachBaseCourseModelDetails
    {

        public int RowCount { get; set; }
    }


    public class ApproachWearingCourseList
    {
        public ApproachWearingCourseList()
        {
            this.WearingCourseList = new List<ApproachWearingCourseModelDetails>();
            ApproachWearingCourseModelDetails WearingCourseModelDetails = new ApproachWearingCourseModelDetails();
            WearingCourseModelDetails.RowCount = 4;
            this.WearingCourseList.Add(WearingCourseModelDetails);

        }

        public List<ApproachWearingCourseModelDetails> WearingCourseList { get; set; }
    }

    public class ApproachWearingCourseModelDetails
    {

        public int RowCount { get; set; }
    }


    public class ApproachProtQOMList
    {
        public ApproachProtQOMList()
        {
            this.ProtQOMList = new List<ApproachProtQOMModelDetails>();
            ApproachProtQOMModelDetails ProtQOMModelDetails = new ApproachProtQOMModelDetails();
            ProtQOMModelDetails.RowCount = 4;
            this.ProtQOMList.Add(ProtQOMModelDetails);

        }

        public List<ApproachProtQOMModelDetails> ProtQOMList { get; set; }
    }

    public class ApproachProtQOMModelDetails
    {

        public int RowCount { get; set; }
    }

    public class ApproachProtWorkmanshipList
    {
        public ApproachProtWorkmanshipList()
        {
            this.ProtWorkmanshipList = new List<ApproachProtWorkmanshipModelDetails>();
            ApproachProtWorkmanshipModelDetails ProtWorkmanshipModelDetails = new ApproachProtWorkmanshipModelDetails();
            ProtWorkmanshipModelDetails.RowCount = 4;
            this.ProtWorkmanshipList.Add(ProtWorkmanshipModelDetails);

        }

        public List<ApproachProtWorkmanshipModelDetails> ProtWorkmanshipList { get; set; }
    }

    public class ApproachProtWorkmanshipModelDetails
    {

        public int RowCount { get; set; }
    }





    public class VerificationBridgeModelList
    {
        public VerificationBridgeModelList()
        {
            this.VerificationList = new List<VerificationModelDetails>();


            VerificationModelDetails VerificationListDetails = new VerificationModelDetails();
            VerificationListDetails.VerificationType = "V";
            VerificationListDetails.RowCount = 12;
            this.VerificationList.Add(VerificationListDetails);

        }

        public List<VerificationModelDetails> VerificationList { get; set; }
    }

    public class VerificationModelDetails
    {
        public string VerificationType { get; set; }
        public int RowCount { get; set; }
    }
     
      public class OnQomFoundationList
    {
        public OnQomFoundationList()
        {
            this.OnQomFoundationDetailsList = new List<OnQomFoundationModelDetails>();


            OnQomFoundationModelDetails OnQomFoundationDetails = new OnQomFoundationModelDetails();

            OnQomFoundationDetails.RowCount = 5;
            this.OnQomFoundationDetailsList.Add(OnQomFoundationDetails);

        }

        public List<OnQomFoundationModelDetails> OnQomFoundationDetailsList { get; set; }
    }

    public class OnQomFoundationModelDetails
    {
        
        public int RowCount { get; set; }
    }
    public class OnWorkmanshipFoundationList
    {
        public OnWorkmanshipFoundationList()
        {
            this.OnWorkmanshipDetailsList = new List<OnWorkmanshipModelDetails>();


            OnWorkmanshipModelDetails OnWorkmanshipDetails = new OnWorkmanshipModelDetails();

            OnWorkmanshipDetails.RowCount = 5;
            this.OnWorkmanshipDetailsList.Add(OnWorkmanshipDetails);

        }

        public List<OnWorkmanshipModelDetails> OnWorkmanshipDetailsList { get; set; }
    }

    public class OnWorkmanshipModelDetails
    {

        public int RowCount { get; set; }
    }



    public class CompletedQomFoundationList
    {
        public CompletedQomFoundationList()
        {
            this.CompletedQomFoundationDetailsList = new List<CompletedQomFoundationModelDetails>();


            CompletedQomFoundationModelDetails CompletedQomFoundationDetails = new CompletedQomFoundationModelDetails();

            CompletedQomFoundationDetails.RowCount = 6;
            this.CompletedQomFoundationDetailsList.Add(CompletedQomFoundationDetails);

        }

        public List<CompletedQomFoundationModelDetails> CompletedQomFoundationDetailsList { get; set; }
    }

    public class CompletedQomFoundationModelDetails
    {

        public int RowCount { get; set; }
    }


    #region Bhushan
    public class ActionTakenByPIUModelDetails
    {
        public ActionTakenByPIUModelDetails()
        {
            RowCount = 4;
        }
        public int RowCount { get; set; }
    }
    #endregion
}