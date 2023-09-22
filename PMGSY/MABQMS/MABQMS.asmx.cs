#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MABQMS.asmx.cs        
        * Description   :   Web service for Mobile Application Based Quality Monitoring System.
        *               :   Web Methods for Login, InserLog, DownLoadSchedule, InsertObservationDetails, UploadAndInsertImageDetails, VerifyUnplannedSchedule etc.
        * Author        :   Shyam Yadav 
        * Creation Date :   30/Sep/2013
 **/
#endregion


using PMGSY.BAL.Proposal;
using PMGSY.Common;
using PMGSY.DAL.MABQMS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace PMGSY.MABQMS
{
    /// <summary>
    /// Summary description for MABQMS
    /// </summary>
    [WebService(Namespace = "https://online.omms.nic.in/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MABQMS : System.Web.Services.WebService
    {
        MABQMSDAL objMABQMSDAL = null;
        RijndaelCrypt cryptObj = null;

        //[WebMethod]
        //public string Login(string userName, string imei)  //Commented on 01-02-2022
        //{
        //    objMABQMSDAL = new MABQMSDAL();
        //    return objMABQMSDAL.LoginDetails(userName, imei);
        //}


        //[WebMethod]
        //public string InsertLog(string monitorCode, string mobileNo, string imeiNo, string osVersion, string modelName, string nwProvider, string appVersion, string loginMode, string ipAddress)  //Commented on 01-02-2022
        //{
        //    objMABQMSDAL = new MABQMSDAL();
        //    return objMABQMSDAL.InsertLog(monitorCode, mobileNo, imeiNo, osVersion, modelName, nwProvider, appVersion, loginMode, ipAddress);
        //}


        //[WebMethod]
        //public string DownloadSchedule(string monitorCode)  //Commented on 01-02-2022
        //{
        //    objMABQMSDAL = new MABQMSDAL();
        //    return objMABQMSDAL.DownloadSchedule(monitorCode);
        //}

        //[WebMethod]
        //public string InsertObservationDetails(string scheduleCode, string prRoadCode, string inspDate, string startChainage, string endChainage, string roadStatus, string startLatitude, string startLongitude, string endLatitude, string endLongitude, string gradeString, string imgToBeUploadedCnt, string bearValueProp, string remark)  //01-02-2022
        //{
        //    objMABQMSDAL = new MABQMSDAL();
        //    return objMABQMSDAL.InsertObservationDetails(scheduleCode, prRoadCode, inspDate, startChainage, endChainage, roadStatus,
        //                                                 startLatitude, startLongitude, endLatitude, endLongitude, gradeString, imgToBeUploadedCnt, bearValueProp, remark);
        //}



        //[WebMethod]
        //public string InsertObservationDetails(string scheduleCode, string prRoadCode, string inspDate, string startChainage, string endChainage, string roadStatus, string startLatitude, string startLongitude, string endLatitude, string endLongitude, string gradeString, string imgToBeUploadedCnt)
        //{
        //    objMABQMSDAL = new MABQMSDAL();
        //    return objMABQMSDAL.InsertObservationDetails(scheduleCode, prRoadCode, inspDate, startChainage, endChainage, roadStatus,
        //                                                 startLatitude, startLongitude, endLatitude, endLongitude, gradeString, imgToBeUploadedCnt);
        //}

        
        //[WebMethod]
        //public string UploadAndInsertImageDetails(byte[] imgData, string scheduleCode, string prRoadCode, string obsId, string fileName, string fileDesc, string latitude, string longitude, string deviceFlag)  // Commented on 01-02-2022
        //{
        //    objMABQMSDAL = new MABQMSDAL();
        //    return objMABQMSDAL.UploadAndInsertImageDetails(imgData, scheduleCode, prRoadCode, obsId, fileName, fileDesc, latitude, longitude, deviceFlag);
        //}


        ///  /// Inspection Date added to the service. 
        ///  ///After 18 Aug 2014 means 2 months after this release, remove function UploadAndInsertImageDetails();
        //[WebMethod]
        //public string UploadAndInsertImageDetailsWithDate(byte[] imgData, string scheduleCode, string prRoadCode, string obsId, string fileName, string fileDesc, string latitude, string longitude, string deviceFlag, string inspDate) //Commented on 01-02-2022
        //{
        //    objMABQMSDAL = new MABQMSDAL();
        //    return objMABQMSDAL.UploadAndInsertImageDetailsWithDate(imgData, scheduleCode, prRoadCode, obsId, fileName, fileDesc, latitude, longitude, deviceFlag, inspDate);
        //}

        //[WebMethod]
        //public string DownloadUnplannedSchedule(string qmCode, string packageId, string inspMonth, string inspYear)  //Commented on 01-02-2022
        //{
        //    objMABQMSDAL = new MABQMSDAL();
        //    //byte[] arr = { };
        //    //return objMABQMSDAL.UploadAndInsertImageDetails(arr, "1", "1", "1", "1", "1", "1", "1", "1");
        //    return objMABQMSDAL.DownloadUnplannedSchedule(qmCode, packageId, inspMonth, inspYear);
        //}

        //[WebMethod]
        //public string AssignUnplannedSchedule(string scheduleCode, string prRoadCode, string monitorCode, string ipAddr)  //Commented on 01-02-2022
        //{
        //    objMABQMSDAL = new MABQMSDAL();
        //    return objMABQMSDAL.AssignUnplannedSchedule(scheduleCode, prRoadCode, monitorCode, ipAddr);
        //}


        //[WebMethod]
        //public string DownloadNotifications(string userName)  //Commented on 01-02-2022
        //{
        //    objMABQMSDAL = new MABQMSDAL();
        //    return objMABQMSDAL.DownloadNotifications(userName);
        //}


        //#region New Service 11 Feb 2021  //Commented on 01-02-2022

        //[WebMethod]
        //public string UploadAndInsertImageDetailsWithDateNew(byte[] imgData, string scheduleCode, string prRoadCode, string obsId, string fileName, string fileDesc, string latitude, string longitude, string deviceFlag, string inspDate)
        //{
        //    objMABQMSDAL = new MABQMSDAL();
        //    return objMABQMSDAL.UploadAndInsertImageDetailsWithDateNew(imgData, scheduleCode, prRoadCode, obsId, fileName, fileDesc, latitude, longitude, deviceFlag, inspDate);
        //    // return objMABQMSDAL.UploadAndInsertImageDetailsWithDateNew1( scheduleCode, prRoadCode, obsId, fileName, fileDesc, latitude, longitude, deviceFlag, inspDate);
        //}


        //[WebMethod]
        //public string GetVersionNumber()
        //{
        //    objMABQMSDAL = new MABQMSDAL();
        //    return objMABQMSDAL.GetVersionNumberDAL();
        //}

        //#endregion

    }
}
