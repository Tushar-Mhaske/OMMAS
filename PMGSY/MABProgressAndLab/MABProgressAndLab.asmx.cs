#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MABProgressAndLab.asmx.cs        
        * Description   :   Web service for Mobile Application Based Progress And Lab Photo Uploads
        *               :   Web Methods for Login, InserLog, DownLoadWorks...etc
        * Author        :   Shyam Yadav 
        * Creation Date :   21/Dec/2015
 **/
#endregion

using PMGSY.DAL.MABProgressAndLab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace PMGSY.MABProgressAndLab
{
    /// <summary>
    /// Summary description for MABProgressAndLab
    /// </summary>
    [WebService(Namespace = "https://online.omms.nic.in/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MABProgressAndLab : System.Web.Services.WebService
    {
        MABProgressAndLabDAL objMABProgressDAL = null;

        #region Before Version 2.0.3

        [WebMethod]
        public string Login(string userName, string password, string imei)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.LoginDetails(userName, password, imei);
        }


        [WebMethod]
        public string Register(string userName, string fName, string lName, string mobNo, string email, string imei)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.Register(userName, fName, lName, mobNo, email, imei);
        }


        [WebMethod]
        public string InsertLog(string userName, string mobileNo, string imeiNo, string osVersion, string modelName, string nwProvider, string appVersion, string ipAddress)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.InsertLog(userName, mobileNo, imeiNo, osVersion, modelName, nwProvider, appVersion, ipAddress);
        }


        [WebMethod]
        public string DownloadWorks(string moduleFlag, string packageId)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.DownloadWorks(moduleFlag, packageId);
        }


        [WebMethod]
        public string InsertLabDetails(string userName, string agreementCode, string packageId, string labEshtablishedDate, string ipAddress)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.InsertLabDetails(userName, agreementCode, packageId, labEshtablishedDate, ipAddress);
        }


        [WebMethod]
        public string UploadImageAndDetailsForLab(byte[] imgData, string labId, string fileDesc,
                                               string latitude, string longitude, string userName, string ipAddress)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.UploadImageAndDetailsForLab(imgData, labId, fileDesc, latitude, longitude, userName, ipAddress);
        }

        [WebMethod]
        public string UploadImageAndDetailsForProgress(byte[] imgData, string prRoadCode, string stageOfProgress, string fileDesc, string latitude, string longitude)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.UploadImageAndDetailsForProgress(imgData, prRoadCode, stageOfProgress, fileDesc, latitude, longitude);
        }

        [WebMethod]
        public string DownloadHabs(string prRoadCode)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.DownloadHabs(prRoadCode);
        }


        [WebMethod]
        public string InsertCumulativeProgressDetails(string prRoadCode, string prepWork, string earthWork, string subBase, string baseCourse,
                                                      string surfaceCourse, string signStones, string cdWorks, string miscelaneous, string lenCompleted, string isCompleted,
                                                      string completionDate, string userName)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.InsertCumulativeProgressDetails(prRoadCode, prepWork, earthWork, subBase, baseCourse,
                                                      surfaceCourse, signStones, cdWorks, miscelaneous, lenCompleted, isCompleted,
                                                      completionDate, userName);
        }


        [WebMethod]
        public string InsertHabitationsConnected(string prRoadCode, string habCodeString)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.InsertHabitationsConnected(prRoadCode, habCodeString);
        }


        [WebMethod]
        public string GetReportData(string userName, string progressStage)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetReportData(userName, progressStage);
        }

        #endregion


        #region Version 2.0.3 ---- Allow multiple Devices against same Username

        [WebMethod]
        public string RegisterV203(string userName, string fName, string lName, string mobNo, string email, string imei)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.RegisterV203(userName, fName, lName, mobNo, email, imei);
        }


        [WebMethod]
        public string InsertLabDetailsV203(string userName, string agreementCode, string packageId, string labEshtablishedDate, string ipAddress, string regId)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.InsertLabDetailsV203(userName, agreementCode, packageId, labEshtablishedDate, ipAddress, regId);
        }

        [WebMethod]
        public string UploadImageAndDetailsForLabV203(byte[] imgData, string labId, string fileDesc,
                                               string latitude, string longitude, string userName, string ipAddress, string regId)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.UploadImageAndDetailsForLabV203(imgData, labId, fileDesc, latitude, longitude, userName, ipAddress, regId);
        }

        [WebMethod]
        public string UploadImageAndDetailsForProgressV203(byte[] imgData, string prRoadCode, string stageOfProgress, string fileDesc, string latitude, string longitude, string regId)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.UploadImageAndDetailsForProgressV203(imgData, prRoadCode, stageOfProgress, fileDesc, latitude, longitude, regId);
        }

        [WebMethod]
        public string InsertCumulativeProgressDetailsV203(string prRoadCode, string prepWork, string earthWork, string subBase, string baseCourse,
                                                      string surfaceCourse, string signStones, string cdWorks, string miscelaneous, string lenCompleted, string isCompleted,
                                                      string completionDate, string userName, string regId)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.InsertCumulativeProgressDetailsV203(prRoadCode, prepWork, earthWork, subBase, baseCourse,
                                                      surfaceCourse, signStones, cdWorks, miscelaneous, lenCompleted, isCompleted,
                                                      completionDate, userName, regId);
        }


        [WebMethod]
        public string InsertHabitationsConnectedV203(string prRoadCode, string habCodeString, string regId)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.InsertHabitationsConnectedV203(prRoadCode, habCodeString, regId);
        }
        #endregion
      
 
        #region RnD section

        [WebMethod]
        public string UploadImageAndDetailsForRnDV4(byte[] imgData, string prRoadCode, string stageOfProgress, string fileDesc, string latitude, string longitude, string regId)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.UploadImageAndDetailsForRnDV4(imgData, prRoadCode, stageOfProgress, fileDesc, latitude, longitude, regId);
        }

        [WebMethod]
        public string GetImageDescSpinnerItems()
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetImageDescSpinnerItems();
        }
        #endregion

        #region Feedback section Created on 28-12-2017[version 4.0]

        [WebMethod]
        public string LoginV4(string userName, string password, string imei)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.LoginDetailsV4(userName, password, imei);
        }

        [WebMethod]
        public String DownloadFeedback(string roleCode, string stateCode, string districtCode, string maxFeedId, string downloadDate)
        {
            MABProgressAndLabDAL objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.DownloadFeedbacks(roleCode, stateCode, districtCode, maxFeedId, downloadDate);
        }

        [WebMethod]
        public String DownloadreplyDetails(string roleCode, string stateCode, string districtCode, string maxReplyId, string downloadDatetime)
        {
            MABProgressAndLabDAL objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.DownloadreplyDetails(roleCode, stateCode, districtCode, maxReplyId, downloadDatetime);
        }

        [WebMethod]
        public string SubmitReply(String feedbackServerID, String replyComment, String replyStatus, String replyByUserID)
        {

            MABProgressAndLabDAL objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.SubmitReplyDAL(feedbackServerID, replyComment, replyStatus, replyByUserID); ;
        }

        [WebMethod]
        public string UploadReplyImage( byte[] imgdata ,string feedbackServerCode, string replyServerCode, string latitude, string longitude)
        {
            MABProgressAndLabDAL objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.UploadReplyImage(imgdata, feedbackServerCode, replyServerCode, latitude, longitude);
        }

        #endregion

        #region Avinash-2   27_07_2018  Work List
        [WebMethod]
        public string GetWorkDetailsForCaptureLocation(string userName, string adminQMCode, string adminBlockCode, string check)
        {
            MABProgressAndLabDAL objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetWorkDetailsForCaptureLocation(userName, adminQMCode, adminBlockCode, check);
        }
        #endregion

        #region Avinash-3  27_07_2018 Insert_Location
        [WebMethod]
        public string InsertDetailsForCaptureLocation(string userID, string prRoadCode, string propType, string startPoint, string cdworkpoint, string midPoint, string endPoint, string captureDate)
        {
            MABProgressAndLabDAL objMABProgressDAL = new MABProgressAndLabDAL();

            return objMABProgressDAL.InsertDetailsForCaptureLocation(userID, prRoadCode, propType, startPoint, cdworkpoint, midPoint, endPoint, captureDate);
        }
        #endregion

        #region Avinash-4 26_09_2018 Getting States

        [WebMethod]
        public string GetStateList()
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetStateList();
        }

        #endregion

        #region Avinash-5 27_09_2018 Getting District

        [WebMethod]
        public string GetDistrictList(string StateCode)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetDistrictList(StateCode);
        }
        #endregion

        #region Avinash-6 26_10_2018 Getting Blocks
        [WebMethod]
        public string GetBlockList(string DistrictCode)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetBlockList(DistrictCode);
        }
        #endregion

        #region Avinash-7 08_01_2019 Facility Development

        //Download Facility Based on District Code And Block code
        [WebMethod]
        public string GetFacilityDetailsForCaptureLocation(string districtCode, string blockCode)
        {
            MABProgressAndLabDAL objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetFacilityDetailsForCaptureLocation(districtCode, blockCode);
        }

        [WebMethod]
        public string GetDistrictFromUserIDForFacility(string userID)
        {
            MABProgressAndLabDAL objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetDistrictFromUserIDForFacility(userID);
        }


        [WebMethod]
        public string GetFacilityCategoryList()
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetFacilityCategoryList();
        }


        [WebMethod]
        public string GetFacilityTypeList(string FacilityCategoryCode)
        {
            //FacilityCategoryCode=FacilityCategoryID
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetFacilityTypeList(FacilityCategoryCode);
        }


        [WebMethod]
        public string GetFacilityHabitationList(string FacilityBlockCode)
        {
            //FacilityCategoryCode=FacilityCategoryID
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetFacilityHabitationList(FacilityBlockCode);
        }


        [WebMethod]
        public string InsertFacilityDetailsForCaptureLocation(string mastfacilityID, string factilityCategoryCode, string factilityTypeCode, string factilityDistrictCode, string factilityBlockCode, string factilityLat, string factilityLong, string factilityName, string factilityAddress, string factilityHabitationCode, string userID, string factilitybase64String, string factilityPinCode)
        {

            objMABProgressDAL = new MABProgressAndLabDAL();

            //            factilityCategoryCode = "+gCNrcJAaSZwvfY/WGVH5A==";
            //factilityTypeCode = "Ymg0ji2UYNInCKbatvRKzA==";
            //factilityDistrictCode = "glLCcFzzgR+oVSNEL1sdOA==";
            //factilityBlockCode = "FKC1QmTVvV3qefkrnWEy3Q==";
            //factilityLat = "XwD+R4aF6DnFm8HMTzamnw==";
            //factilityLong = "D6V3Dwe+nKHkRm68C11i8DTB98iWLcCDhFMdNqr7Bz8=";
            //factilityName = "VcCiE0dP8P4c/bHLeJk35A==";
            //factilityAddress = "1nW0Ir8XgDa81YijFbHbZw==";
            //factilityHabitationCode = "YqCqoVtEXIY2Gb3YoI3ZbQ==";
            //userID = "lzHv58mVxR+YErVJfZnwVw==";
            //factilitybase64String ='B@54739f7';




            return objMABProgressDAL.InsertFacilityDetailsForCaptureLocation(mastfacilityID, factilityCategoryCode, factilityTypeCode, factilityDistrictCode, factilityBlockCode, factilityLat, factilityLong, factilityName, factilityAddress, factilityHabitationCode, userID, factilitybase64String, factilityPinCode);
        }


        [WebMethod]
        public string GetBlockName(string BlockCode)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetBlockName(BlockCode);
        }

        [WebMethod]
        public string GetStateName(string StateCode)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetStateName(StateCode);
        }


        [WebMethod]
        public string GetDistrictName(string DistrictCode)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetDistrictName(DistrictCode);
        }

        //New Added Function
        [WebMethod]
        public string GetStateNameBasedOnUserID(string userID)
        {
            objMABProgressDAL = new MABProgressAndLabDAL();
            return objMABProgressDAL.GetStateNameBasedOnUserID(userID);
        }

        #endregion
    }
}
