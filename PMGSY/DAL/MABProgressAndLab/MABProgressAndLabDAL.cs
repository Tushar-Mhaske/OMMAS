#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MABProgressAndLabDAL.asmx.cs        
        * Description   :   Datamethods for Web service for Mobile Application Based Quality Monitoring System.
        *               :   Datamethods as Login, InserLog, DownLoadSchedule, InsertObservationDetails, UploadAndInsertImageDetails, VerifyUnplannedSchedule etc.
        * Author        :   Shyam Yadav 
        * Creation Date :   30/Sep/2013
 **/
#endregion

using PMGSY.DAL;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Transactions;
using System.Data.Entity;
using System.Configuration;
using PMGSY.Common;
using System.Text;
using PMGSY.MABProgressAndLab;
using PMGSY.Extensions;
using System.Data.Entity.Core;

namespace PMGSY.DAL.MABProgressAndLab
{
    public class MABProgressAndLabDAL
    {
        PMGSYEntities dbContext = null;
        public static string key = "^$S{%T&]@W8_01-";
        PMGSY.MABProgressAndLab.RijndaelCrypt cryptObj = new PMGSY.MABProgressAndLab.RijndaelCrypt(key);

        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["MABProgressErrorLogPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//MABProgressErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";


        /// <summary>
        /// Converts Objects in XML Form
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public string GetXMLFromObject(object obj)
        {
            XmlSerializer XmlS = new XmlSerializer(obj.GetType());

            StringWriter sw = new StringWriter();
            XmlTextWriter tw = new XmlTextWriter(sw);

            XmlS.Serialize(tw, obj);
            return sw.ToString();
        }

        #region Before Version 2.0.3

        /// <summary>
        /// Verify Login, IMEI and return user details
        /// Returns Status  0 -  User Not Exists
        ///         Status -2 -  User Exists with Invalid IMEI
        ///         Status -1 -  Exception
        ///         Status  1 -  User Exists with valid IMEI
        ///         
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        public string LoginDetails(string encUserName, string encPassword, string encImei)
        {
            dbContext = new PMGSYEntities();
            string status = "-1";
            string userName = null;
            //string imei = null;
            string password = null;
            try
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }


                userName = (encUserName == null || encUserName.Trim().Equals("")) ? null : cryptObj.Decrypt(encUserName);
                //imei = cryptObj.Decrypt(encImei);
                password = cryptObj.Decrypt(encPassword);

                string passwordHash = new Login().EncodePassword(password);
                // Check for Valid User, if valid then proceed, else return
                var userDetails = dbContext.UM_User_Master.Where(c => c.UserName.Equals(userName) && c.Password.Equals(passwordHash)).FirstOrDefault();

                if (userDetails == null)
                {
                    return cryptObj.Encrypt("0");
                }

                switch (userDetails.DefaultRoleID)
                {
                    case 2:
                        status = "1_SRRDA";
                        break;
                    case 22:
                        status = "1_PIU";
                        break;
                    case 25:
                        status = "1_MORD";
                        break;
                    case 37:
                        status = "1_SRRDAOA";
                        break;
                    case 38:
                        status = "1_PIUOA";
                        break;
                    default:
                        status = "0";
                        break;
                }


                return cryptObj.Encrypt(status);
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "LoginDetails()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "LoginDetails()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Exception StackTrace: " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Register new User
        /// Restriction of Only one Device Per UserName for Registration
        /// </summary>
        /// <param name="encUserName"></param>
        /// <param name="encFName"></param>
        /// <param name="encLName"></param>
        /// <param name="encMobNo"></param>
        /// <param name="encEmail"></param>
        /// <param name="encImei"></param>
        /// <returns></returns>
        public string Register(string encUserName, string encFName, string encLName, string encMobNo, string encEmail, string encImei)
        {
            dbContext = new PMGSYEntities();
            try
            {
                string userName = cryptObj.Decrypt(encUserName);
                string fName = cryptObj.Decrypt(encFName);
                string lName = cryptObj.Decrypt(encLName);
                string mobNo = cryptObj.Decrypt(encMobNo);
                string email = cryptObj.Decrypt(encEmail);
                string imei = cryptObj.Decrypt(encImei);

                //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "Register()");
                //    sw.WriteLine("encUserName : " + encUserName);
                //    sw.WriteLine("encFName : " + encFName);
                //    sw.WriteLine("encLName : " + encLName);
                //    sw.WriteLine("encMobNo : " + encMobNo);
                //    sw.WriteLine("encEmail : " + encEmail);
                //    sw.WriteLine("encImei : " + encImei);
                //    sw.WriteLine("____________________________________________________");
                //    sw.Close();
                //}

                // Check for Valid User, if valid then proceed, else return
                var userDetails = dbContext.MABProgressUserDetails.Where(c => c.UserName.Equals(userName)).FirstOrDefault();

                if (userDetails != null && userDetails.UserName.Equals(userName))
                {
                    if (userDetails.IMEINo == null) // If user exists with null IMEI
                    {
                        var progressDetails = dbContext.MABProgressUserDetails.Where(c => c.UserName.Equals(userName)).First();
                        progressDetails.FirstName = fName;
                        progressDetails.LastName = lName;
                        progressDetails.MobileNo = mobNo;
                        progressDetails.EmailId = email;
                        progressDetails.IMEINo = imei;
                        progressDetails.RegistrationDate = DateTime.Now;
                        dbContext.MABProgressUserDetails.Add(progressDetails);
                        dbContext.Entry(progressDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        return cryptObj.Encrypt("1");
                    }
                    else if (userDetails.IMEINo.Equals(imei)) // User with same device - can update details
                    {
                        var progressDetails = dbContext.MABProgressUserDetails.Where(c => c.UserName.Equals(userName)).First();
                        progressDetails.FirstName = fName;
                        progressDetails.LastName = lName;
                        progressDetails.MobileNo = mobNo;
                        progressDetails.EmailId = email;
                        progressDetails.RegistrationDate = DateTime.Now;
                        dbContext.MABProgressUserDetails.Add(progressDetails);
                        dbContext.Entry(progressDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        return cryptObj.Encrypt("1");
                    }
                    else
                    {
                        return cryptObj.Encrypt("2"); //User already registered
                    }
                }
                else
                {
                    if (userDetails != null && userDetails.IMEINo.Equals(imei))
                    {
                        return cryptObj.Encrypt("3"); //IMEI already exists
                    }

                    MABProgressUserDetails objProgressDetail = new MABProgressUserDetails();
                    Int32 maxCode = 0;
                    if (dbContext.MABProgressUserDetails.Any())
                    {
                        maxCode = dbContext.MABProgressUserDetails.Select(c => c.ID).Max();
                    }

                    objProgressDetail.ID = maxCode + 1;
                    objProgressDetail.UserName = userName;
                    objProgressDetail.FirstName = fName;
                    objProgressDetail.LastName = lName;
                    objProgressDetail.MobileNo = mobNo;
                    objProgressDetail.EmailId = email;
                    objProgressDetail.IMEINo = imei;

                    dbContext.MABProgressUserDetails.Add(objProgressDetail);
                    dbContext.SaveChanges();

                    return cryptObj.Encrypt("1");
                }

            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "Register()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "Register()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "Register()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Exception StackTrace: " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Insert Log Details to Log Table
        /// </summary>
        /// <param name="monitorCode"></param>
        /// <param name="mobileNo"></param>
        /// <param name="imeiNo"></param>
        /// <param name="osVersion"></param>
        /// <param name="modelName"></param>
        /// <param name="nwProvider"></param>
        /// <param name="appVersion"></param>
        /// <param name="loginMode"></param>
        /// <returns></returns>
        public string InsertLog(string userName, string mobileNo, string imeiNo, string osVersion, string modelName, string nwProvider, string appVersion, string ipAddress)
        {
            dbContext = new PMGSYEntities();
            try
            {
                string decUserName = cryptObj.Decrypt(userName);
                string decMobileNo = cryptObj.Decrypt(mobileNo);
                string decImei = cryptObj.Decrypt(imeiNo);
                string decOsVersion = cryptObj.Decrypt(osVersion);
                string decModelName = cryptObj.Decrypt(modelName);
                string decNwProvider = cryptObj.Decrypt(nwProvider);
                string decAppVersion = cryptObj.Decrypt(appVersion);
                string decIpAddress = cryptObj.Decrypt(ipAddress);

                int insertCount = dbContext.USP_MABProgress_Insert_Log(decUserName, decMobileNo, decImei, decOsVersion, decModelName, decNwProvider, decAppVersion, decIpAddress);
                //int insertCount = dbContext.USP_MABProgress_Insert_Log(userName, mobileNo, imeiNo, osVersion, modelName, nwProvider, appVersion, ipAddress);
                return cryptObj.Encrypt("1");
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertLog()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertLog()");
                    sw.WriteLine("Exception : " + ex.Message);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Download Unplanned schedules which are not yet uploaded for last month & current month
        /// return all details with Month of scheudle
        /// </summary>
        /// <param name="scheduleCode"></param>
        /// <param name="prRoadCode"></param>
        /// <param name="monitorCode"></param>
        /// <returns></returns>
        public string DownloadWorks(string moduleFlag, string packageId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                string decModuleFlag = cryptObj.Decrypt(moduleFlag);
                string decPackageId = cryptObj.Decrypt(packageId);

                //string decModuleFlag = moduleFlag;
                //string decPackageId = packageId;

                List<USP_MABProgress_Download_works_Result> itemList = new List<USP_MABProgress_Download_works_Result>();
                itemList = dbContext.USP_MABProgress_Download_works(decModuleFlag, decPackageId).ToList<USP_MABProgress_Download_works_Result>();

                if (itemList.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                return cryptObj.Encrypt(GetXMLFromObject(itemList).Replace("&amp;", " and "));
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DownloadWorks()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DownloadWorks()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Insert Lab Details
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="agreementCode"></param>
        /// <param name="packageId"></param>
        /// <param name="labEshtablishedDate"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public string InsertLabDetails(string userName, string agreementCode, string packageId, string labEshtablishedDate, string ipAddress)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                string decUserName = cryptObj.Decrypt(userName);
                int decAgreementCode = Convert.ToInt32(cryptObj.Decrypt(agreementCode));
                string decPackageId = cryptObj.Decrypt(packageId);
                string decLabEshtablishedDate = cryptObj.Decrypt(labEshtablishedDate);
                string decIPAddress = cryptObj.Decrypt(ipAddress);

                if (dbContext.QUALITY_QM_LAB_MASTER.Where(c => c.TEND_AGREEMENT_CODE == decAgreementCode).Any())
                {
                    return cryptObj.Encrypt("3"); //Lab details against agreement are already entered
                }

                DateTime labDate = Convert.ToDateTime(decLabEshtablishedDate);
                int labId = 0;
                if (dbContext.QUALITY_QM_LAB_MASTER.ToList().Any())
                {
                    labId = dbContext.QUALITY_QM_LAB_MASTER.Select(lab => lab.QM_LAB_ID).Max();
                }

                QUALITY_QM_LAB_MASTER labToadd = new QUALITY_QM_LAB_MASTER
                {
                    QM_LAB_ID = labId + 1,
                    TEND_AGREEMENT_CODE = decAgreementCode,
                    IMS_PACKAGE_ID = decPackageId,
                    QM_LAB_ESTABLISHMENT_DATE = labDate,
                    QM_SQC_APPROVAL = "N",
                    QM_LAB_CLOSURE_STATUS = "I",
                    QM_LOCK_STATUS = "N",
                    USERID = dbContext.UM_User_Master.Where(c => c.UserName == decUserName).Select(c => c.UserID).First(),
                    IPADD = (string.IsNullOrEmpty(decIPAddress) ? "" : (decIPAddress.Length > 15 ? decIPAddress.Substring(0, 15) : decIPAddress))
                };

                dbContext.QUALITY_QM_LAB_MASTER.Add(labToadd);
                dbContext.SaveChanges();

                Int32 savedLabId = labId + 1;
                return cryptObj.Encrypt("1_" + savedLabId.ToString());
            }
            catch (UpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertLabDetails()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "InsertLabDetails()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertLabDetails()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Upload and save image details for Lab
        /// </summary>
        /// <param name="imgData"></param>
        /// <param name="labId"></param>
        /// <param name="fileDesc"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="userName"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public string UploadImageAndDetailsForLab(byte[] imgData, string labId, string fileDesc,
                                               string latitude, string longitude, string userName, string ipAddress)
        {
            dbContext = new PMGSYEntities();
            int insertCount = 0;
            bool isTransComplete = false;
            int maxFileID = 0;
            string storageRoot = string.Empty;
            int currentImgNumber = 0;
            try
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("labId :" + labId);
                    sw.WriteLine("fileDesc : " + fileDesc);
                    sw.WriteLine("latitude : " + latitude);
                    sw.WriteLine("longitude : " + longitude);
                    sw.WriteLine("userName : " + userName);
                    sw.WriteLine("ipAddress : " + ipAddress);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                int decLabId = Convert.ToInt32(cryptObj.Decrypt(labId));
                string decFileDesc = cryptObj.Decrypt(fileDesc);
                string decLat = cryptObj.Decrypt(latitude);
                string decLong = cryptObj.Decrypt(longitude);
                string decUserName = cryptObj.Decrypt(userName);
                string decIPAddress = cryptObj.Decrypt(ipAddress);


                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("labId :" + decLabId);
                    sw.WriteLine("fileDesc : " + decFileDesc);
                    sw.WriteLine("latitude : " + decLat);
                    sw.WriteLine("longitude : " + decLong);
                    sw.WriteLine("userName : " + decUserName);
                    sw.WriteLine("ipAddress : " + decIPAddress);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                if (decLat.Equals("") && decLong.Equals(""))
                {
                    decLat = "0";
                    decLong = "0";
                }

                decimal decLatitude = Convert.ToDecimal(decLat);
                decimal decLongitude = Convert.ToDecimal(decLong);

                // First Take Max File ID to follow the naming convention for Files
                // countOfFilesUploaded taken to modify File Name with combination of (MaxFileId + 1) appended with "_" and countOfFilesUploaded
                Int32 countOfFilesUploaded = dbContext.QUALITY_QM_LAB_DETAILS.Where(c => c.QM_LAB_ID == decLabId).Select(c => c.QM_LAB_FILE_ID).Count();
                currentImgNumber = countOfFilesUploaded;
                if (countOfFilesUploaded == 0)
                {
                    countOfFilesUploaded = 1;
                }
                else
                {
                    countOfFilesUploaded = countOfFilesUploaded + 1;
                }

                if (dbContext.QUALITY_QM_LAB_DETAILS.Count() == 0)
                    maxFileID = 0;
                else
                    maxFileID = (from c in dbContext.QUALITY_QM_LAB_DETAILS select (Int32?)c.QM_LAB_FILE_ID ?? 0).Max();

                var fileId = maxFileID + 1;

                string fileName = fileId + "_" + countOfFilesUploaded + ".jpeg";

                storageRoot = ConfigurationManager.AppSettings["QUALITY_LAB_FILE_UPLOAD"];

                //------------------- Insert Image Details In Databse  ---------------------------- //
                using (var scope = new TransactionScope())
                {
                    // This condition is to check, is Total number of images already uploaded
                    if (currentImgNumber > 1)
                    {
                        return cryptObj.Encrypt("3"); //Number of images exceeded
                    }

                    QUALITY_QM_LAB_DETAILS qualityQMLabDetails = new QUALITY_QM_LAB_DETAILS();
                    qualityQMLabDetails.QM_LAB_FILE_ID = fileId;
                    qualityQMLabDetails.QM_LAB_ID = decLabId;
                    qualityQMLabDetails.QM_LAB_FILE_NAME = fileName;
                    qualityQMLabDetails.QM_LAB_FILE_DESC = decFileDesc;
                    qualityQMLabDetails.QM_LAB_FILE_UPLOAD_DATE = DateTime.Now;
                    qualityQMLabDetails.QM_LAB_FILE_LATITUDE = decLatitude;
                    qualityQMLabDetails.QM_LAB_FILE_LONGITUDE = decLongitude;
                    qualityQMLabDetails.QM_LAB_IMAGE_FIRST = "Y";
                    qualityQMLabDetails.USERID = dbContext.UM_User_Master.Where(c => c.UserName == decUserName).Select(c => c.UserID).First();
                    qualityQMLabDetails.IPADD = (string.IsNullOrEmpty(decIPAddress) ? "" : (decIPAddress.Length > 15 ? decIPAddress.Substring(0, 15) : decIPAddress));

                    dbContext.QUALITY_QM_LAB_DETAILS.Add(qualityQMLabDetails);
                    insertCount = dbContext.SaveChanges();

                    scope.Complete();
                    isTransComplete = true;
                }

                //------------------- Save Image to Disk ---------------------------- //

                if (isTransComplete)
                {
                    var fullPath = Path.Combine(storageRoot, fileName);
                    var thumbnailPath = Path.Combine(storageRoot, "thumbnails");
                    var fullThumbnailPath = Path.Combine(thumbnailPath, fileName);
                    int saveImgCnt = new CommonFunctions().SaveImage(imgData, storageRoot, fullPath, fullThumbnailPath);

                    if (saveImgCnt == 1 && insertCount > 0)
                        return cryptObj.Encrypt("1");           //for last image
                    else if (saveImgCnt == 1 && insertCount <= 0)
                    {
                        //for error in saving image, but other details uploaded
                        //Delete last saved record based on current file Name
                        QUALITY_QM_LAB_DETAILS qualityLabDetails = dbContext.QUALITY_QM_LAB_DETAILS.Find(fileId);
                        dbContext.QUALITY_QM_LAB_DETAILS.Remove(qualityLabDetails);
                        dbContext.SaveChanges();
                        return cryptObj.Encrypt("-2");
                    }
                    else
                        return cryptObj.Encrypt("-1");
                }
                else
                    return cryptObj.Encrypt("-1");

                //--------------------------------------------------------------------- //
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadImageAndDetailsForLab()");
                    sw.WriteLine("Inner Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "UploadImageAndDetailsForLab()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadImageAndDetailsForLab()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Save Image details for Progress
        /// </summary>
        /// <param name="imgData"></param>
        /// <param name="prRoadCode"></param>
        /// <param name="fileDesc"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public string UploadImageAndDetailsForProgress(byte[] imgData, string prRoadCode, string stageOfProgress, string fileDesc, string latitude, string longitude)
        {
            dbContext = new PMGSYEntities();
            int insertCount = 0;
            bool isTransComplete = false;
            int maxFileID = 0;
            string storageRoot = string.Empty;
            int currentImgNumber = 0;
            try
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("prRoadCode :" + prRoadCode);
                    sw.WriteLine("stageOfProgress : " + stageOfProgress);
                    sw.WriteLine("fileDesc : " + fileDesc);
                    sw.WriteLine("latitude : " + latitude);
                    sw.WriteLine("longitude : " + longitude);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
                string decStageOfProgress = cryptObj.Decrypt(stageOfProgress);
                string decFileDesc = cryptObj.Decrypt(fileDesc);
                string decLat = cryptObj.Decrypt(latitude);
                string decLong = cryptObj.Decrypt(longitude);

                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("decPrRoadCode :" + decPrRoadCode);
                    sw.WriteLine("decStageOfProgress : " + decStageOfProgress);
                    sw.WriteLine("decFileDesc : " + decFileDesc);
                    sw.WriteLine("decLat : " + decLat);
                    sw.WriteLine("decLong : " + decLong);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                if (decLat.Equals("") && decLong.Equals(""))
                {
                    decLat = "0";
                    decLong = "0";
                }

                decimal decLatitude = Convert.ToDecimal(decLat);
                decimal decLongitude = Convert.ToDecimal(decLong);

                // First Take Max File ID to follow the naming convention for Files
                // countOfFilesUploaded taken to modify File Name with combination of (MaxFileId + 1) appended with "_" and countOfFilesUploaded
                Int32 countOfFilesUploaded = dbContext.EXEC_FILES.Where(c => c.IMS_PR_ROAD_CODE == decPrRoadCode).Select(c => c.EXEC_FILE_ID).Count();
                currentImgNumber = countOfFilesUploaded;
                // This condition is to check, is Total number of images already uploaded
                if (currentImgNumber >= 20)
                {
                    return cryptObj.Encrypt("3"); //Number of images exceeded
                }

                if (countOfFilesUploaded == 0)
                {
                    countOfFilesUploaded = 1;
                }
                else
                {
                    countOfFilesUploaded = countOfFilesUploaded + 1;
                }

                if (dbContext.EXEC_FILES.Where(s => s.IMS_PR_ROAD_CODE == decPrRoadCode).Any())
                {
                    maxFileID = (from c in dbContext.EXEC_FILES.Where(s => s.IMS_PR_ROAD_CODE == decPrRoadCode) select (Int32?)c.EXEC_FILE_ID ?? 0).Max();
                }
                else
                {
                    maxFileID = 0;
                }

                var fileId = maxFileID + 1;
                string fileName = decPrRoadCode + "-" + countOfFilesUploaded + ".jpeg";
                storageRoot = ConfigurationManager.AppSettings["EXECUTION_PRGRESS_FILE_UPLOAD"];

                //------------------- Insert Image Details In Databse  ---------------------------- //
                using (var scope = new TransactionScope())
                {
                    EXEC_FILES execFiles = new EXEC_FILES();
                    execFiles.EXEC_FILE_ID = fileId;
                    execFiles.IMS_PR_ROAD_CODE = decPrRoadCode;
                    execFiles.EXEC_UPLOAD_DATE = DateTime.Now;
                    execFiles.EXEC_FILE_NAME = fileName;
                    execFiles.EXEC_FILE_DESC = decFileDesc;
                    execFiles.EXEC_LATITUDE = decLatitude;
                    execFiles.EXEC_LONGITUDE = decLongitude;
                    execFiles.EXEC_FILE_TYPE = 0;
                    execFiles.EXEC_STAGE = Convert.ToInt32(decStageOfProgress);
                    execFiles.EXEC_STATUS = "P";

                    dbContext.EXEC_FILES.Add(execFiles);
                    insertCount = dbContext.SaveChanges();

                    scope.Complete();
                    isTransComplete = true;
                }

                //------------------- Save Image to Disk ---------------------------- //

                if (isTransComplete)
                {
                    var fullPath = Path.Combine(storageRoot, fileName);
                    var thumbnailPath = Path.Combine(storageRoot, "thumbnails");
                    var fullThumbnailPath = Path.Combine(thumbnailPath, fileName);
                    int saveImgCnt = new CommonFunctions().SaveImage(imgData, storageRoot, fullPath, fullThumbnailPath);

                    if (saveImgCnt == 1 && insertCount > 0)
                    {
                        Int32 noOfImagesUploaded = dbContext.EXEC_FILES.Where(c => c.IMS_PR_ROAD_CODE == decPrRoadCode).Select(c => c.EXEC_FILE_ID).Count();
                        return cryptObj.Encrypt("1_" + noOfImagesUploaded);           //for last image
                    }
                    else if (saveImgCnt == 1 && insertCount <= 0)
                    {
                        //for error in saving image, but other details uploaded
                        //Delete last saved record based on current file Name
                        EXEC_FILES fileDetails = dbContext.EXEC_FILES.Find(decPrRoadCode, fileId);
                        dbContext.EXEC_FILES.Remove(fileDetails);
                        dbContext.SaveChanges();
                        return cryptObj.Encrypt("-2");
                    }
                    else
                        return cryptObj.Encrypt("-1");
                }
                else
                    return cryptObj.Encrypt("-1");

                //--------------------------------------------------------------------- //
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadImageAndDetailsForProgress()");
                    sw.WriteLine("Inner Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadImageAndDetailsForProgress()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Download Habitation mapped with Road id
        /// </summary>
        /// <param name="prRoadCode"></param>
        /// <returns></returns>
        public string DownloadHabs(string prRoadCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                Int32 decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
                List<Int32> habConnected = new List<Int32>();
                List<EXEC_HABITATIONS_CONNECTED> connectedHabDetails = dbContext.EXEC_HABITATIONS_CONNECTED.Where(c => c.IMS_PR_ROAD_CODE == decPrRoadCode).ToList();
                Int32 maxOrder = 0;
                if (connectedHabDetails.Count > 0)
                {
                    habConnected = connectedHabDetails.Select(c => c.MAST_HAB_CODE).ToList();
                    maxOrder = connectedHabDetails.Select(c => c.EXEC_HAB_CONNECTED_ORDER).Max();
                }

                var sanctionRoadDetails = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == decPrRoadCode).First();

                if (sanctionRoadDetails.IMS_UPGRADE_CONNECT.Equals("N"))
                {
                    List<HabConnectionModel> itemList = new List<HabConnectionModel>();
                    var habDetails = (from ibh in dbContext.IMS_BENEFITED_HABS
                                      join mh in dbContext.MASTER_HABITATIONS on ibh.MAST_HAB_CODE equals mh.MAST_HAB_CODE
                                      join mhd in dbContext.MASTER_HABITATIONS_DETAILS on ibh.MAST_HAB_CODE equals mhd.MAST_HAB_CODE
                                      join isp in dbContext.IMS_SANCTIONED_PROJECTS on ibh.IMS_PR_ROAD_CODE equals isp.IMS_PR_ROAD_CODE
                                      where ibh.IMS_PR_ROAD_CODE == decPrRoadCode
                                      && ibh.HAB_INCLUDED.Equals("Y")
                                      && !(habConnected.Contains(ibh.MAST_HAB_CODE))
                                      && (mhd.MAST_YEAR == ((isp.MAST_PMGSY_SCHEME == 1) ? 2001 : 2011))
                                      select new
                                      {
                                          habCode = ibh.MAST_HAB_CODE,
                                          habName = mh.MAST_HAB_NAME,
                                          population = mhd.MAST_HAB_TOT_POP,
                                          habMaxOrder = maxOrder
                                      }).Distinct().ToList();

                    foreach (var item in habDetails)
                    {
                        HabConnectionModel model = new HabConnectionModel();
                        model.HabCode = item.habCode.ToString();
                        model.HabName = item.habName;
                        model.HabPopulation = item.population.ToString();
                        model.HabMaxOrder = item.habMaxOrder.ToString();
                        itemList.Add(model);
                    }

                    if (itemList.Count == 0)
                    {
                        return cryptObj.Encrypt("0");
                    }

                    return cryptObj.Encrypt(GetXMLFromObject(itemList).Replace("&amp;", " and "));
                }
                else
                    return cryptObj.Encrypt("0");

            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DownloadHabs()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DownloadHabs()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Insert Progress Details
        /// </summary>
        /// <param name="prRoadCode"></param>
        /// <param name="progressDate"></param>
        /// <param name="prepWork"></param>
        /// <param name="earthWork"></param>
        /// <param name="subBase"></param>
        /// <param name="baseCourse"></param>
        /// <param name="surfaceCourse"></param>
        /// <param name="signStones"></param>
        /// <param name="cdWorks"></param>
        /// <param name="miscelaneous"></param>
        /// <param name="lenCompleted"></param>
        /// <param name="isCompleted"></param>
        /// <param name="completionDate"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string InsertCumulativeProgressDetails(string prRoadCode, string prepWork, string earthWork, string subBase, string baseCourse,
                                                      string surfaceCourse, string signStones, string cdWorks, string miscelaneous, string lenCompleted, string isCompleted,
                                                      string completionDate, string userName)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
                DateTime decProgressDate = DateTime.Now;
                decimal decPrepWork = Convert.ToDecimal(cryptObj.Decrypt(prepWork));
                decimal decEarthWork = Convert.ToDecimal(cryptObj.Decrypt(earthWork));
                decimal decSubBase = Convert.ToDecimal(cryptObj.Decrypt(subBase));
                decimal decBaseCourse = Convert.ToDecimal(cryptObj.Decrypt(baseCourse));
                decimal decSurfaceCourse = Convert.ToDecimal(cryptObj.Decrypt(surfaceCourse));
                decimal decSignStones = Convert.ToDecimal(cryptObj.Decrypt(signStones));
                decimal decCdWorks = Convert.ToDecimal(cryptObj.Decrypt(cdWorks));
                decimal decMiscelaneous = Convert.ToDecimal(cryptObj.Decrypt(miscelaneous));
                decimal decLenCompleted = Convert.ToDecimal(cryptObj.Decrypt(lenCompleted));
                string decIsCompleted = cryptObj.Decrypt(isCompleted);

                string decCompDate = cryptObj.Decrypt(completionDate);

                string decUserName = cryptObj.Decrypt(userName);

                if (dbContext.EXEC_ROADS_WEEKLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == decPrRoadCode && c.EXEC_PROGRESS_DATE == decProgressDate).Any())
                {
                    return cryptObj.Encrypt("3"); //Progress details already entered
                }

                if (!string.IsNullOrEmpty(decCompDate))
                {
                    EXEC_ROADS_WEEKLY_STATUS progressToadd = new EXEC_ROADS_WEEKLY_STATUS
                    {
                        IMS_PR_ROAD_CODE = decPrRoadCode,
                        EXEC_PROGRESS_DATE = decProgressDate,
                        EXEC_PREPARATORY_WORK = decPrepWork,
                        EXEC_EARTHWORK_SUBGRADE = decEarthWork,
                        EXEC_SUBBASE_PREPRATION = decSubBase,
                        EXEC_BASE_COURSE = decBaseCourse,
                        EXEC_SURFACE_COURSE = decSurfaceCourse,
                        EXEC_SIGNS_STONES = decSignStones,
                        EXEC_CD_WORKS = decCdWorks,
                        EXEC_MISCELANEOUS = decMiscelaneous,
                        EXEC_COMPLETED = decLenCompleted,
                        EXEC_ISCOMPLETED = decIsCompleted,
                        EXEC_COMPLETION_DATE = Convert.ToDateTime(decCompDate),
                        USERID = dbContext.UM_User_Master.Where(c => c.UserName == decUserName).Select(c => c.UserID).First(),
                    };
                    dbContext.EXEC_ROADS_WEEKLY_STATUS.Add(progressToadd);
                    dbContext.SaveChanges();
                }
                else
                {
                    EXEC_ROADS_WEEKLY_STATUS progressToadd = new EXEC_ROADS_WEEKLY_STATUS
                    {
                        IMS_PR_ROAD_CODE = decPrRoadCode,
                        EXEC_PROGRESS_DATE = decProgressDate,
                        EXEC_PREPARATORY_WORK = decPrepWork,
                        EXEC_EARTHWORK_SUBGRADE = decEarthWork,
                        EXEC_SUBBASE_PREPRATION = decSubBase,
                        EXEC_BASE_COURSE = decBaseCourse,
                        EXEC_SURFACE_COURSE = decSurfaceCourse,
                        EXEC_SIGNS_STONES = decSignStones,
                        EXEC_CD_WORKS = decCdWorks,
                        EXEC_MISCELANEOUS = decMiscelaneous,
                        EXEC_COMPLETED = decLenCompleted,
                        EXEC_ISCOMPLETED = decIsCompleted,
                        USERID = dbContext.UM_User_Master.Where(c => c.UserName == decUserName).Select(c => c.UserID).First(),
                    };
                    dbContext.EXEC_ROADS_WEEKLY_STATUS.Add(progressToadd);
                    dbContext.SaveChanges();
                }


                return cryptObj.Encrypt("1");
            }
            catch (UpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertCumulativeProgressDetails()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "InsertCumulativeProgressDetails()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertCumulativeProgressDetails()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Inser Connected Habitations with their order
        /// </summary>
        /// <param name="prRoadCode"></param>
        /// <param name="progressDate"></param>
        /// <param name="habCode"></param>
        /// <param name="habConnectedOrder"></param>
        /// <returns></returns>
        public string InsertHabitationsConnected(string prRoadCode, string habCodeString)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertHabitationsConnected()");
                    sw.WriteLine("habCodeString : " + habCodeString);
                    sw.WriteLine("prRoadCode : " + decPrRoadCode);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                string[] stringSeparators = new string[] { "$$" };
                string[] habCodeArr = habCodeString.Split(stringSeparators, StringSplitOptions.None);
                string[] decHabCodeArr = new string[habCodeArr.Length];

                for (int i = 0; i < habCodeArr.Length; i++)
                {
                    decHabCodeArr[i] = cryptObj.Decrypt(habCodeArr[i]);

                    using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Method : " + "InsertHabitationsConnected()");
                        sw.WriteLine("decHabCodeArr[i] : " + decHabCodeArr[i]);
                        sw.WriteLine("____________________________________________________");
                        sw.Close();
                    }
                }

                bool isTransComplete = false;
                using (var scope = new TransactionScope())
                {
                    for (int i = 0; i < decHabCodeArr.Length; i++)
                    {
                        string[] habArr = decHabCodeArr[i].Split('@');
                        EXEC_HABITATIONS_CONNECTED habToadd = new EXEC_HABITATIONS_CONNECTED
                        {
                            IMS_PR_ROAD_CODE = decPrRoadCode,
                            EXEC_PROGRESS_DATE = DateTime.Now,
                            MAST_HAB_CODE = Convert.ToInt32(habArr[0]),
                            EXEC_HAB_CONNECTED_ORDER = Convert.ToInt32(habArr[1])
                        };

                        dbContext.EXEC_HABITATIONS_CONNECTED.Add(habToadd);
                        dbContext.SaveChanges();
                    }

                    scope.Complete();
                    isTransComplete = true;

                }

                if (isTransComplete)
                    return cryptObj.Encrypt("1");
                else
                    return cryptObj.Encrypt("-1");
            }
            catch (UpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertHabitationsConnected()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "InsertHabitationsConnected()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertHabitationsConnected()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Get Report Data
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="progressStage"></param>
        /// <returns></returns>
        public string GetReportData(string userName, string progressStage)
        {
            dbContext = new PMGSYEntities();
            try
            {
                string decUserName = cryptObj.Decrypt(userName);
                Int32? decProgressStage = Convert.ToInt32(cryptObj.Decrypt(progressStage));

                var userDetails = dbContext.UM_User_Master.Where(c => c.UserName.Equals(decUserName.Trim())).FirstOrDefault();
                Int32? stateCode = 0;
                if (userDetails.Mast_State_Code != null)
                    stateCode = userDetails.Mast_State_Code;

                Int32? level = 0;

                if (userDetails.DefaultRoleID == 22 || userDetails.DefaultRoleID == 38) // 22 - PIU and 38 - PIUOA
                {
                    level = 1;
                }
                else if (userDetails.DefaultRoleID == 2 || userDetails.DefaultRoleID == 37)//2 - SRRDA and 37 - SRRDAOA
                {
                    level = 2;
                }
                else if (userDetails.DefaultRoleID == 25)   //MORD
                {
                    level = 3;
                }

                List<USP_WEEKLY_PROGRESS_Result> itemList = new List<USP_WEEKLY_PROGRESS_Result>();
                itemList = dbContext.USP_WEEKLY_PROGRESS(decUserName.Trim(), level, decProgressStage, stateCode).ToList<USP_WEEKLY_PROGRESS_Result>();

                if (itemList.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                string str = GetXMLFromObject(itemList).Replace("&amp;", " and ");
                return cryptObj.Encrypt(GetXMLFromObject(itemList).Replace("&amp;", " and "));
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetReportData()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetReportData()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion



        #region Version 2.0.3

        /// <summary>
        /// method to Register User details 
        /// Provision of Multiple Users against same User Name can be registered is given in this method Version
        /// Date : 23/02/2016
        /// Version of Application is V203
        /// </summary>
        /// <param name="encUserName"></param>
        /// <param name="encFName"></param>
        /// <param name="encLName"></param>
        /// <param name="encMobNo"></param>
        /// <param name="encEmail"></param>
        /// <param name="encImei"></param>
        /// <returns></returns>
        public string RegisterV203(string encUserName, string encFName, string encLName, string encMobNo, string encEmail, string encImei)
        {
            dbContext = new PMGSYEntities();
            try
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "Register()");
                    sw.WriteLine("encUserName : " + encUserName);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                string userName = cryptObj.Decrypt(encUserName);
                string fName = cryptObj.Decrypt(encFName);
                string lName = cryptObj.Decrypt(encLName);
                string mobNo = cryptObj.Decrypt(encMobNo);
                string email = cryptObj.Decrypt(encEmail);
                string imei = cryptObj.Decrypt(encImei);
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "Register()");
                    sw.WriteLine("userName : " + userName);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                // Check for Valid User, if valid then proceed, else return
                var userDetails = dbContext.MABProgressUserDetails.Where(c => c.IMEINo.Equals(imei)).FirstOrDefault();

                if (userDetails != null)
                {
                    userDetails.FirstName = fName;
                    userDetails.LastName = lName;
                    userDetails.MobileNo = mobNo;
                    userDetails.EmailId = email;
                    userDetails.IMEINo = imei;
                    userDetails.RegistrationDate = DateTime.Now;
                    dbContext.Entry(userDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    return cryptObj.Encrypt("1_" + userDetails.ID);
                }
                else
                {
                    MABProgressUserDetails objProgressDetail = new MABProgressUserDetails();
                    Int32 maxCode = 0;
                    if (dbContext.MABProgressUserDetails.Any())
                    {
                        maxCode = dbContext.MABProgressUserDetails.Select(c => c.ID).Max();
                    }

                    objProgressDetail.ID = maxCode + 1;
                    objProgressDetail.UserName = userName;
                    objProgressDetail.FirstName = fName;
                    objProgressDetail.LastName = lName;
                    objProgressDetail.MobileNo = mobNo;
                    objProgressDetail.EmailId = email;
                    objProgressDetail.IMEINo = imei;

                    dbContext.MABProgressUserDetails.Add(objProgressDetail);
                    dbContext.SaveChanges();

                    return cryptObj.Encrypt("1_" + objProgressDetail.ID);
                }

            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "RegisterV203()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "RegisterV203()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "RegisterV203()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Exception StackTrace: " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Insert Lab Details
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="agreementCode"></param>
        /// <param name="packageId"></param>
        /// <param name="labEshtablishedDate"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public string InsertLabDetailsV203(string userName, string agreementCode, string packageId, string labEshtablishedDate, string ipAddress, string regId)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                string decUserName = cryptObj.Decrypt(userName);
                int decAgreementCode = Convert.ToInt32(cryptObj.Decrypt(agreementCode));
                string decPackageId = cryptObj.Decrypt(packageId);
                string decLabEshtablishedDate = cryptObj.Decrypt(labEshtablishedDate);
                string decIPAddress = cryptObj.Decrypt(ipAddress);
                Int32 decRegistrationId = Convert.ToInt32(cryptObj.Decrypt(regId));

                if (dbContext.QUALITY_QM_LAB_MASTER.Where(c => c.TEND_AGREEMENT_CODE == decAgreementCode).Any())
                {
                    return cryptObj.Encrypt("3"); //Lab details against agreement are already entered
                }

                DateTime labDate = Convert.ToDateTime(decLabEshtablishedDate);
                int labId = 0;
                if (dbContext.QUALITY_QM_LAB_MASTER.ToList().Any())
                {
                    labId = dbContext.QUALITY_QM_LAB_MASTER.Select(lab => lab.QM_LAB_ID).Max();
                }

                QUALITY_QM_LAB_MASTER labToadd = new QUALITY_QM_LAB_MASTER
                {
                    QM_LAB_ID = labId + 1,
                    TEND_AGREEMENT_CODE = decAgreementCode,
                    IMS_PACKAGE_ID = decPackageId,
                    QM_LAB_ESTABLISHMENT_DATE = labDate,
                    QM_SQC_APPROVAL = "N",
                    QM_LAB_CLOSURE_STATUS = "I",
                    QM_LOCK_STATUS = "N",
                    USERID = dbContext.UM_User_Master.Where(c => c.UserName == decUserName).Select(c => c.UserID).First(),
                    IPADD = (string.IsNullOrEmpty(decIPAddress) ? "" : (decIPAddress.Length > 15 ? decIPAddress.Substring(0, 15) : decIPAddress)),
                    REGISTERD_USER = decRegistrationId
                };

                dbContext.QUALITY_QM_LAB_MASTER.Add(labToadd);
                dbContext.SaveChanges();

                Int32 savedLabId = labId + 1;
                return cryptObj.Encrypt("1_" + savedLabId.ToString());
            }
            catch (UpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertLabDetails()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "InsertLabDetails()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertLabDetails()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Upload and save image details for Lab
        /// </summary>
        /// <param name="imgData"></param>
        /// <param name="labId"></param>
        /// <param name="fileDesc"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="userName"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public string UploadImageAndDetailsForLabV203(byte[] imgData, string labId, string fileDesc,
                                               string latitude, string longitude, string userName, string ipAddress, string regId)
        {
            dbContext = new PMGSYEntities();
            int insertCount = 0;
            bool isTransComplete = false;
            int maxFileID = 0;
            string storageRoot = string.Empty;
            int currentImgNumber = 0;
            try
            {
                //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                //{
                //    sw.WriteLine("labId :" + labId);
                //    sw.WriteLine("fileDesc : " + fileDesc);
                //    sw.WriteLine("latitude : " + latitude);
                //    sw.WriteLine("longitude : " + longitude);
                //    sw.WriteLine("userName : " + userName);
                //    sw.WriteLine("ipAddress : " + ipAddress);
                //    sw.WriteLine("____________________________________________________");
                //    sw.Close();
                //}

                int decLabId = Convert.ToInt32(cryptObj.Decrypt(labId));
                string decFileDesc = cryptObj.Decrypt(fileDesc);
                string decLat = cryptObj.Decrypt(latitude);
                string decLong = cryptObj.Decrypt(longitude);
                string decUserName = cryptObj.Decrypt(userName);
                string decIPAddress = cryptObj.Decrypt(ipAddress);
                Int32 decRegistrationId = Convert.ToInt32(cryptObj.Decrypt(regId));

                //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                //{
                //    sw.WriteLine("labId :" + decLabId);
                //    sw.WriteLine("fileDesc : " + decFileDesc);
                //    sw.WriteLine("latitude : " + decLat);
                //    sw.WriteLine("longitude : " + decLong);
                //    sw.WriteLine("userName : " + decUserName);
                //    sw.WriteLine("ipAddress : " + decIPAddress);
                //    sw.WriteLine("____________________________________________________");
                //    sw.Close();
                //}

                if (decLat.Equals("") && decLong.Equals(""))
                {
                    decLat = "0";
                    decLong = "0";
                }

                decimal decLatitude = Convert.ToDecimal(decLat);
                decimal decLongitude = Convert.ToDecimal(decLong);

                // First Take Max File ID to follow the naming convention for Files
                // countOfFilesUploaded taken to modify File Name with combination of (MaxFileId + 1) appended with "_" and countOfFilesUploaded
                Int32 countOfFilesUploaded = dbContext.QUALITY_QM_LAB_DETAILS.Where(c => c.QM_LAB_ID == decLabId).Select(c => c.QM_LAB_FILE_ID).Count();
                currentImgNumber = countOfFilesUploaded;
                if (countOfFilesUploaded == 0)
                {
                    countOfFilesUploaded = 1;
                }
                else
                {
                    countOfFilesUploaded = countOfFilesUploaded + 1;
                }

                if (dbContext.QUALITY_QM_LAB_DETAILS.Count() == 0)
                    maxFileID = 0;
                else
                    maxFileID = (from c in dbContext.QUALITY_QM_LAB_DETAILS select (Int32?)c.QM_LAB_FILE_ID ?? 0).Max();

                var fileId = maxFileID + 1;

                string fileName = fileId + "_" + countOfFilesUploaded + ".jpeg";

                storageRoot = ConfigurationManager.AppSettings["QUALITY_LAB_FILE_UPLOAD"];

                //------------------- Insert Image Details In Databse  ---------------------------- //
                using (var scope = new TransactionScope())
                {
                    // This condition is to check, is Total number of images already uploaded
                    if (currentImgNumber > 1)
                    {
                        return cryptObj.Encrypt("3"); //Number of images exceeded
                    }

                    QUALITY_QM_LAB_DETAILS qualityQMLabDetails = new QUALITY_QM_LAB_DETAILS();
                    qualityQMLabDetails.QM_LAB_FILE_ID = fileId;
                    qualityQMLabDetails.QM_LAB_ID = decLabId;
                    qualityQMLabDetails.QM_LAB_FILE_NAME = fileName;
                    qualityQMLabDetails.QM_LAB_FILE_DESC = decFileDesc;
                    qualityQMLabDetails.QM_LAB_FILE_UPLOAD_DATE = DateTime.Now;
                    qualityQMLabDetails.QM_LAB_FILE_LATITUDE = decLatitude;
                    qualityQMLabDetails.QM_LAB_FILE_LONGITUDE = decLongitude;
                    qualityQMLabDetails.QM_LAB_IMAGE_FIRST = "Y";
                    qualityQMLabDetails.USERID = dbContext.UM_User_Master.Where(c => c.UserName == decUserName).Select(c => c.UserID).First();
                    //qualityQMLabDetails.IPADD = decIPAddress.Substring(0, 15);
                    qualityQMLabDetails.IPADD = (string.IsNullOrEmpty(decIPAddress) ? "" : (decIPAddress.Length > 15 ? decIPAddress.Substring(0, 15) : decIPAddress));
                    qualityQMLabDetails.REGISTERD_USER = decRegistrationId;

                    dbContext.QUALITY_QM_LAB_DETAILS.Add(qualityQMLabDetails);
                    insertCount = dbContext.SaveChanges();

                    scope.Complete();
                    isTransComplete = true;
                }

                //------------------- Save Image to Disk ---------------------------- //

                if (isTransComplete)
                {
                    var fullPath = Path.Combine(storageRoot, fileName);
                    var thumbnailPath = Path.Combine(storageRoot, "thumbnails");
                    var fullThumbnailPath = Path.Combine(thumbnailPath, fileName);
                    int saveImgCnt = new CommonFunctions().SaveImage(imgData, storageRoot, fullPath, fullThumbnailPath);

                    if (saveImgCnt == 1 && insertCount > 0)
                        return cryptObj.Encrypt("1");           //for last image
                    else if (saveImgCnt == 1 && insertCount <= 0)
                    {
                        //for error in saving image, but other details uploaded
                        //Delete last saved record based on current file Name
                        QUALITY_QM_LAB_DETAILS qualityLabDetails = dbContext.QUALITY_QM_LAB_DETAILS.Find(fileId);
                        dbContext.QUALITY_QM_LAB_DETAILS.Remove(qualityLabDetails);
                        dbContext.SaveChanges();
                        return cryptObj.Encrypt("-2");
                    }
                    else
                        return cryptObj.Encrypt("-1");
                }
                else
                    return cryptObj.Encrypt("-1");

                //--------------------------------------------------------------------- //
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadImageAndDetailsForLab()");
                    sw.WriteLine("Inner Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "UploadImageAndDetailsForLab()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadImageAndDetailsForLab()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Save Image details for Progress
        /// </summary>
        /// <param name="imgData"></param>
        /// <param name="prRoadCode"></param>
        /// <param name="fileDesc"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public string UploadImageAndDetailsForProgressV203(byte[] imgData, string prRoadCode, string stageOfProgress, string fileDesc, string latitude, string longitude, string regId)
        {
            dbContext = new PMGSYEntities();
            int insertCount = 0;
            bool isTransComplete = false;
            int maxFileID = 0;
            string storageRoot = string.Empty;
            int currentImgNumber = 0;
            try
            {
                //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                //{
                //    sw.WriteLine("prRoadCode :" + prRoadCode);
                //    sw.WriteLine("stageOfProgress : " + stageOfProgress);
                //    sw.WriteLine("fileDesc : " + fileDesc);
                //    sw.WriteLine("latitude : " + latitude);
                //    sw.WriteLine("longitude : " + longitude);
                //    sw.WriteLine("____________________________________________________");
                //    sw.Close();
                //}

                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
                string decStageOfProgress = cryptObj.Decrypt(stageOfProgress);
                string decFileDesc = cryptObj.Decrypt(fileDesc);
                string decLat = cryptObj.Decrypt(latitude);
                string decLong = cryptObj.Decrypt(longitude);
                Int32 decRegistrationId = Convert.ToInt32(cryptObj.Decrypt(regId));

                //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                //{
                //    sw.WriteLine("decPrRoadCode :" + decPrRoadCode);
                //    sw.WriteLine("decStageOfProgress : " + decStageOfProgress);
                //    sw.WriteLine("decFileDesc : " + decFileDesc);
                //    sw.WriteLine("decLat : " + decLat);
                //    sw.WriteLine("decLong : " + decLong);
                //    sw.WriteLine("____________________________________________________");
                //    sw.Close();
                //}

                if (decLat.Equals("") && decLong.Equals(""))
                {
                    decLat = "0";
                    decLong = "0";
                }

                decimal decLatitude = Convert.ToDecimal(decLat);
                decimal decLongitude = Convert.ToDecimal(decLong);

                // First Take Max File ID to follow the naming convention for Files
                // countOfFilesUploaded taken to modify File Name with combination of (MaxFileId + 1) appended with "_" and countOfFilesUploaded
                Int32 countOfFilesUploaded = dbContext.EXEC_FILES.Where(c => c.IMS_PR_ROAD_CODE == decPrRoadCode).Select(c => c.EXEC_FILE_ID).Count();
                currentImgNumber = countOfFilesUploaded;
                // This condition is to check, is Total number of images already uploaded
                if (currentImgNumber >= 20)
                {
                    return cryptObj.Encrypt("3"); //Number of images exceeded
                }

                if (countOfFilesUploaded == 0)
                {
                    countOfFilesUploaded = 1;
                }
                else
                {
                    countOfFilesUploaded = countOfFilesUploaded + 1;
                }

                if (dbContext.EXEC_FILES.Where(s => s.IMS_PR_ROAD_CODE == decPrRoadCode).Any())
                {
                    maxFileID = (from c in dbContext.EXEC_FILES.Where(s => s.IMS_PR_ROAD_CODE == decPrRoadCode) select (Int32?)c.EXEC_FILE_ID ?? 0).Max();
                }
                else
                {
                    maxFileID = 0;
                }

                var fileId = maxFileID + 1;
                string fileName = decPrRoadCode + "-" + countOfFilesUploaded + ".jpeg";
                storageRoot = ConfigurationManager.AppSettings["EXECUTION_PRGRESS_FILE_UPLOAD"];

                //------------------- Insert Image Details In Databse  ---------------------------- //
                using (var scope = new TransactionScope())
                {
                    EXEC_FILES execFiles = new EXEC_FILES();
                    execFiles.EXEC_FILE_ID = fileId;
                    execFiles.IMS_PR_ROAD_CODE = decPrRoadCode;
                    execFiles.EXEC_UPLOAD_DATE = DateTime.Now;
                    execFiles.EXEC_FILE_NAME = fileName;
                    execFiles.EXEC_FILE_DESC = decFileDesc;
                    execFiles.EXEC_LATITUDE = decLatitude;
                    execFiles.EXEC_LONGITUDE = decLongitude;
                    execFiles.EXEC_FILE_TYPE = 0;
                    execFiles.EXEC_STAGE = Convert.ToInt32(decStageOfProgress);
                    execFiles.EXEC_STATUS = "P";
                    execFiles.REGISTERD_USER = decRegistrationId;

                    dbContext.EXEC_FILES.Add(execFiles);
                    insertCount = dbContext.SaveChanges();

                    scope.Complete();
                    isTransComplete = true;
                }

                //------------------- Save Image to Disk ---------------------------- //

                if (isTransComplete)
                {
                    var fullPath = Path.Combine(storageRoot, fileName);
                    var thumbnailPath = Path.Combine(storageRoot, "thumbnails");
                    var fullThumbnailPath = Path.Combine(thumbnailPath, fileName);
                    int saveImgCnt = new CommonFunctions().SaveImage(imgData, storageRoot, fullPath, fullThumbnailPath);

                    if (saveImgCnt == 1 && insertCount > 0)
                    {
                        Int32 noOfImagesUploaded = dbContext.EXEC_FILES.Where(c => c.IMS_PR_ROAD_CODE == decPrRoadCode).Select(c => c.EXEC_FILE_ID).Count();
                        return cryptObj.Encrypt("1_" + noOfImagesUploaded);           //for last image
                    }
                    else if (saveImgCnt == 1 && insertCount <= 0)
                    {
                        //for error in saving image, but other details uploaded
                        //Delete last saved record based on current file Name
                        EXEC_FILES fileDetails = dbContext.EXEC_FILES.Find(decPrRoadCode, fileId);
                        dbContext.EXEC_FILES.Remove(fileDetails);
                        dbContext.SaveChanges();
                        return cryptObj.Encrypt("-2");
                    }
                    else
                        return cryptObj.Encrypt("-1");
                }
                else
                    return cryptObj.Encrypt("-1");

                //--------------------------------------------------------------------- //
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadImageAndDetailsForProgress()");
                    sw.WriteLine("Inner Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadImageAndDetailsForProgress()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Insert Progress Details
        /// </summary>
        /// <param name="prRoadCode"></param>
        /// <param name="progressDate"></param>
        /// <param name="prepWork"></param>
        /// <param name="earthWork"></param>
        /// <param name="subBase"></param>
        /// <param name="baseCourse"></param>
        /// <param name="surfaceCourse"></param>
        /// <param name="signStones"></param>
        /// <param name="cdWorks"></param>
        /// <param name="miscelaneous"></param>
        /// <param name="lenCompleted"></param>
        /// <param name="isCompleted"></param>
        /// <param name="completionDate"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string InsertCumulativeProgressDetailsV203(string prRoadCode, string prepWork, string earthWork, string subBase, string baseCourse,
                                                      string surfaceCourse, string signStones, string cdWorks, string miscelaneous, string lenCompleted, string isCompleted,
                                                      string completionDate, string userName, string regId)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                //Avinash Stop Physical Progress Entry for April Month
                string AprilMonthDay = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH_DAY"];
                int AprilMonthDayValue = Convert.ToInt16(AprilMonthDay);
                string AprilMonth = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH"];
                int AprilMonthValue = Convert.ToInt16(AprilMonth);
                int CurrentDay = DateTime.Now.Day;
                int currMonth = DateTime.Now.Month;
                if (currMonth == AprilMonthValue)
                {
                    return cryptObj.Encrypt("-1");

                }

                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
                DateTime decProgressDate = DateTime.Now;
                decimal decPrepWork = Convert.ToDecimal(cryptObj.Decrypt(prepWork));
                decimal decEarthWork = Convert.ToDecimal(cryptObj.Decrypt(earthWork));
                decimal decSubBase = Convert.ToDecimal(cryptObj.Decrypt(subBase));
                decimal decBaseCourse = Convert.ToDecimal(cryptObj.Decrypt(baseCourse));
                decimal decSurfaceCourse = Convert.ToDecimal(cryptObj.Decrypt(surfaceCourse));
                decimal decSignStones = Convert.ToDecimal(cryptObj.Decrypt(signStones));
                decimal decCdWorks = Convert.ToDecimal(cryptObj.Decrypt(cdWorks));
                decimal decMiscelaneous = Convert.ToDecimal(cryptObj.Decrypt(miscelaneous));
                decimal decLenCompleted = Convert.ToDecimal(cryptObj.Decrypt(lenCompleted));
                string decIsCompleted = cryptObj.Decrypt(isCompleted);

                string decCompDate = cryptObj.Decrypt(completionDate);

                string decUserName = cryptObj.Decrypt(userName);
                Int32 decRegistrationId = Convert.ToInt32(cryptObj.Decrypt(regId));

                if (dbContext.EXEC_ROADS_WEEKLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == decPrRoadCode && c.EXEC_PROGRESS_DATE == decProgressDate).Any())
                {
                    return cryptObj.Encrypt("3"); //Progress details already entered
                }

                if (!string.IsNullOrEmpty(decCompDate))
                {
                    EXEC_ROADS_WEEKLY_STATUS progressToadd = new EXEC_ROADS_WEEKLY_STATUS
                    {
                        IMS_PR_ROAD_CODE = decPrRoadCode,
                        EXEC_PROGRESS_DATE = decProgressDate,
                        EXEC_PREPARATORY_WORK = decPrepWork,
                        EXEC_EARTHWORK_SUBGRADE = decEarthWork,
                        EXEC_SUBBASE_PREPRATION = decSubBase,
                        EXEC_BASE_COURSE = decBaseCourse,
                        EXEC_SURFACE_COURSE = decSurfaceCourse,
                        EXEC_SIGNS_STONES = decSignStones,
                        EXEC_CD_WORKS = decCdWorks,
                        EXEC_MISCELANEOUS = decMiscelaneous,
                        EXEC_COMPLETED = decLenCompleted,
                        EXEC_ISCOMPLETED = decIsCompleted,
                        EXEC_COMPLETION_DATE = Convert.ToDateTime(decCompDate),
                        USERID = dbContext.UM_User_Master.Where(c => c.UserName == decUserName).Select(c => c.UserID).First(),
                        REGISTERD_USER = decRegistrationId
                    };
                    dbContext.EXEC_ROADS_WEEKLY_STATUS.Add(progressToadd);
                    dbContext.SaveChanges();
                }
                else
                {
                    EXEC_ROADS_WEEKLY_STATUS progressToadd = new EXEC_ROADS_WEEKLY_STATUS
                    {
                        IMS_PR_ROAD_CODE = decPrRoadCode,
                        EXEC_PROGRESS_DATE = decProgressDate,
                        EXEC_PREPARATORY_WORK = decPrepWork,
                        EXEC_EARTHWORK_SUBGRADE = decEarthWork,
                        EXEC_SUBBASE_PREPRATION = decSubBase,
                        EXEC_BASE_COURSE = decBaseCourse,
                        EXEC_SURFACE_COURSE = decSurfaceCourse,
                        EXEC_SIGNS_STONES = decSignStones,
                        EXEC_CD_WORKS = decCdWorks,
                        EXEC_MISCELANEOUS = decMiscelaneous,
                        EXEC_COMPLETED = decLenCompleted,
                        EXEC_ISCOMPLETED = decIsCompleted,
                        USERID = dbContext.UM_User_Master.Where(c => c.UserName == decUserName).Select(c => c.UserID).First(),
                    };
                    dbContext.EXEC_ROADS_WEEKLY_STATUS.Add(progressToadd);
                    dbContext.SaveChanges();
                }


                return cryptObj.Encrypt("1");
            }
            catch (UpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertCumulativeProgressDetails()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "InsertCumulativeProgressDetails()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertCumulativeProgressDetails()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Inser Connected Habitations with their order
        /// </summary>
        /// <param name="prRoadCode"></param>
        /// <param name="progressDate"></param>
        /// <param name="habCode"></param>
        /// <param name="habConnectedOrder"></param>
        /// <returns></returns>
        public string InsertHabitationsConnectedV203(string prRoadCode, string habCodeString, string regId)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                //Avinash Stop Habitation Entry for April Month
                string AprilMonthDay = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH_DAY"];
                int AprilMonthDayValue = Convert.ToInt16(AprilMonthDay);
                string AprilMonth = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH"];
                int AprilMonthValue = Convert.ToInt16(AprilMonth);
                int CurrentDay = DateTime.Now.Day;
                int currMonth = DateTime.Now.Month;
                if (currMonth == AprilMonthValue)
                {
                    return cryptObj.Encrypt("-1");

                }

                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));

                string[] stringSeparators = new string[] { "$$" };
                string[] habCodeArr = habCodeString.Split(stringSeparators, StringSplitOptions.None);
                string[] decHabCodeArr = new string[habCodeArr.Length];
                Int32 decRegistrationId = Convert.ToInt32(cryptObj.Decrypt(regId));

                for (int i = 0; i < habCodeArr.Length; i++)
                {
                    decHabCodeArr[i] = cryptObj.Decrypt(habCodeArr[i]);

                    //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                    //{
                    //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    //    sw.WriteLine("Method : " + "InsertHabitationsConnected()");
                    //    sw.WriteLine("decHabCodeArr[i] : " + decHabCodeArr[i]);
                    //    sw.WriteLine("____________________________________________________");
                    //    sw.Close();
                    //}
                }

                bool isTransComplete = false;
                using (var scope = new TransactionScope())
                {
                    for (int i = 0; i < decHabCodeArr.Length; i++)
                    {
                        string[] habArr = decHabCodeArr[i].Split('@');
                        EXEC_HABITATIONS_CONNECTED habToadd = new EXEC_HABITATIONS_CONNECTED
                        {
                            IMS_PR_ROAD_CODE = decPrRoadCode,
                            EXEC_PROGRESS_DATE = DateTime.Now,
                            MAST_HAB_CODE = Convert.ToInt32(habArr[0]),
                            EXEC_HAB_CONNECTED_ORDER = Convert.ToInt32(habArr[1]),
                            REGISTERD_USER = decRegistrationId
                        };

                        dbContext.EXEC_HABITATIONS_CONNECTED.Add(habToadd);
                        dbContext.SaveChanges();
                    }

                    scope.Complete();
                    isTransComplete = true;

                }

                if (isTransComplete)
                    return cryptObj.Encrypt("1");
                else
                    return cryptObj.Encrypt("-1");
            }
            catch (UpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertHabitationsConnected()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "InsertHabitationsConnected()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertHabitationsConnected()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region version 4.0

        /// <summary>
        /// Save Image details for Progress
        /// </summary>
        /// <param name="imgData"></param>
        /// <param name="prRoadCode"></param>
        /// <param name="fileDesc"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public string UploadImageAndDetailsForRnDV4(byte[] imgData, string prRoadCode, string stageOfProgress, string fileDesc, string latitude, string longitude, string regId)
        {
            dbContext = new PMGSYEntities();
            int insertCount = 1;
            bool isTransComplete = false;
            int maxFileID = 0;
            string storageRoot = string.Empty;
            int currentImgNumber = 0;
            try
            {
                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
                string decStageOfProgress = cryptObj.Decrypt(stageOfProgress);
                string decFileDesc = cryptObj.Decrypt(fileDesc);
                string decLat = cryptObj.Decrypt(latitude);
                string decLong = cryptObj.Decrypt(longitude);
                Int32 decRegistrationId = Convert.ToInt32(cryptObj.Decrypt(regId));


                if (decLat.Equals("") && decLong.Equals(""))
                {
                    decLat = "0";
                    decLong = "0";
                }

                decimal decLatitude = Convert.ToDecimal(decLat);
                decimal decLongitude = Convert.ToDecimal(decLong);

                // First Take Max File ID to follow the naming convention for Files
                // countOfFilesUploaded taken to modify File Name with combination of (MaxFileId + 1) appended with "_" and countOfFilesUploaded


                Int32 countOfFilesUploaded = dbContext.EXEC_TECH_FILES.Where(c => c.IMS_PR_ROAD_CODE == decPrRoadCode).Select(c => c.EXEC_FILE_ID).Count();
                currentImgNumber = countOfFilesUploaded;
                // This condition is to check, is Total number of images already uploaded


                if (currentImgNumber >= 20)
                {
                    return cryptObj.Encrypt("3"); //Number of images exceeded
                }

                if (countOfFilesUploaded == 0)
                {
                    countOfFilesUploaded = 1;
                }
                else
                {
                    countOfFilesUploaded = countOfFilesUploaded + 1;
                }

                if (dbContext.EXEC_TECH_FILES.Where(s => s.IMS_PR_ROAD_CODE == decPrRoadCode).Any())
                {
                    maxFileID = dbContext.EXEC_TECH_FILES.Where(z => z.IMS_PR_ROAD_CODE == decPrRoadCode).Max(x => x.EXEC_FILE_ID);
                }
                else
                {
                    maxFileID = 0;
                }

                var fileId = maxFileID + 1;
                string fileName = decPrRoadCode + "-" + countOfFilesUploaded + ".jpeg";


                storageRoot = ConfigurationManager.AppSettings["EXEC_TECH_PROGRESS_FILE_UPLOAD"];

                //------------------- Insert Image Details In Databse  ---------------------------- //
                using (var scope = new TransactionScope())
                {
                    EXEC_TECH_FILES execFiles = new EXEC_TECH_FILES();
                    execFiles.EXEC_FILE_ID = fileId;
                    execFiles.IMS_PR_ROAD_CODE = decPrRoadCode;
                    execFiles.EXEC_UPLOAD_DATE = DateTime.Now;
                    execFiles.EXEC_FILE_NAME = fileName;
                    execFiles.EXEC_FILE_DESC = decFileDesc;
                    execFiles.EXEC_LATITUDE = decLatitude;
                    execFiles.EXEC_LONGITUDE = decLongitude;
                    execFiles.EXEC_STAGE = Convert.ToInt32(decStageOfProgress);
                    execFiles.EXEC_STATUS = dbContext.IMS_SANCTIONED_PROJECTS.FirstOrDefault(p => p.IMS_PR_ROAD_CODE == decPrRoadCode).IMS_ISCOMPLETED;
                    execFiles.REGISTERD_USER = decRegistrationId;
                    dbContext.EXEC_TECH_FILES.Add(execFiles);


                    insertCount = dbContext.SaveChanges();

                    scope.Complete();
                    isTransComplete = true;
                }

                //------------------- Save Image to Disk ---------------------------- //

                if (isTransComplete)
                {
                    var fullPath = Path.Combine(storageRoot, fileName);
                    var thumbnailPath = Path.Combine(storageRoot, "thumbnails");
                    var fullThumbnailPath = Path.Combine(thumbnailPath, fileName);
                    int saveImgCnt = new CommonFunctions().SaveImage(imgData, storageRoot, fullPath, fullThumbnailPath);

                    if (saveImgCnt == 1 && insertCount > 0)
                    {
                        Int32 noOfImagesUploaded = dbContext.EXEC_TECH_FILES.Where(c => c.IMS_PR_ROAD_CODE == decPrRoadCode).Select(c => c.EXEC_FILE_ID).Count();
                        return cryptObj.Encrypt("1_" + noOfImagesUploaded);           //for last image
                    }
                    else if (saveImgCnt == 1 && insertCount <= 0)
                    {
                        //for error in saving image, but other details uploaded
                        //Delete last saved record based on current file Name
                        EXEC_TECH_FILES fileDetails = dbContext.EXEC_TECH_FILES.Find(decPrRoadCode, fileId);
                        dbContext.EXEC_TECH_FILES.Remove(fileDetails);
                        dbContext.SaveChanges();
                        return cryptObj.Encrypt("-2");
                    }
                    else
                        return cryptObj.Encrypt("-1");
                }
                else
                    return cryptObj.Encrypt("-1");

                //--------------------------------------------------------------------- //
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadImageAndDetailsForRnDV4()");
                    sw.WriteLine("Inner Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadImageAndDetailsForRnDV4()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetImageDescSpinnerItems()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ImageDecscrptionModel> imageDescLst = new List<ImageDecscrptionModel>();

                List<MASTER_EXECUTION_ITEM> execItenLst = dbContext.MASTER_EXECUTION_ITEM.ToList();

                foreach (MASTER_EXECUTION_ITEM item in execItenLst)
                {
                    ImageDecscrptionModel model = new ImageDecscrptionModel();
                    model.HeadCode = item.MAST_HEAD_CODE.ToString();
                    model.HeadSHDesc = item.MAST_HEAD_SH_DESC;
                    model.HeadType = item.MAST_HEAD_TYPE;
                    imageDescLst.Add(model);
                }
                return cryptObj.Encrypt(GetXMLFromObject(imageDescLst));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetImageDescSpinnerItems()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Version 4.0 --Feedbacks

        /// <summary>
        /// Verify Login, IMEI and return user details
        /// Returns Status  0 -  User Not Exists
        ///         Status -2 -  User Exists with Invalid IMEI
        ///         Status -1 -  Exception
        ///         Status  1 -  User Exists with valid IMEI
        ///         
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        public string LoginDetailsV4(string encUserName, string encPassword, string encImei)
        {
            dbContext = new PMGSYEntities();
            string status = "-1";
            string userName = null;
            //string imei = null;
            string password = null;
            try
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }


                userName = (encUserName == null || encUserName.Trim().Equals("")) ? null : cryptObj.Decrypt(encUserName);
                //imei = cryptObj.Decrypt(encImei);
                password = cryptObj.Decrypt(encPassword);

                string passwordHash = new Login().EncodePassword(password);
                // Check for Valid User, if valid then proceed, else return
                var userDetails = dbContext.UM_User_Master.Where(c => c.UserName.Equals(userName) && c.Password.Equals(passwordHash)).FirstOrDefault();


                if (userDetails == null)
                {
                    return cryptObj.Encrypt("0");
                }

                int? stateCode = userDetails.Mast_State_Code == null ? 0 : userDetails.Mast_State_Code;
                int? districtCode = userDetails.Mast_District_Code == null ? 0 : userDetails.Mast_District_Code;

                switch (userDetails.DefaultRoleID)
                {


                    case 2:
                        status = "1_SRRDA_" + stateCode + "_" + districtCode + "_" + userDetails.DefaultRoleID + "_" + userDetails.UserID;
                        break;
                    case 22:
                        status = "1_PIU_" + stateCode + "_" + districtCode + "_" + userDetails.DefaultRoleID + "_" + userDetails.UserID;
                        //1_PIU_21_328_22
                        break;
                    case 25:
                        status = "1_MORD_" + stateCode + "_" + districtCode + "_" + userDetails.DefaultRoleID + "_" + userDetails.UserID;
                        break;
                    case 37:
                        status = "1_SRRDAOA_" + stateCode + "_" + districtCode + "_" + userDetails.DefaultRoleID + "_" + userDetails.UserID;
                        break;
                    case 38:
                        status = "1_PIUOA_" + stateCode + "_" + districtCode + "_" + userDetails.DefaultRoleID + "_" + userDetails.UserID;
                        break;
                    case 53:
                        status = "1_MORD_" + stateCode + "_" + districtCode + "_" + userDetails.DefaultRoleID + "_" + userDetails.UserID;
                        break;
                    case 54:
                        status = "1_RCPLWE_" + stateCode + "_" + districtCode + "_" + userDetails.DefaultRoleID + "_" + userDetails.UserID;
                        break;
                    default:
                        status = "0";
                        break;
                }


                return cryptObj.Encrypt(status);
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "LoginDetails()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "LoginDetails()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Exception StackTrace: " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        ///
        public String DownloadFeedbacks(string roleCode, string stateCode, string districtCode, string maxFeedId, string downloadDate)
        {
            int State = Convert.ToInt32(cryptObj.Decrypt(stateCode));
            int District = Convert.ToInt32(cryptObj.Decrypt(districtCode));
            int Role = Convert.ToInt32(cryptObj.Decrypt(roleCode));
            int MaxfbID = Convert.ToInt32(cryptObj.Decrypt(maxFeedId));
            if (downloadDate != null || cryptObj.Decrypt(downloadDate) != "0")
            {
                DateTime DownloadDate = Convert.ToDateTime(cryptObj.Decrypt(downloadDate));
            }
            try
            {
                dbContext = new PMGSYEntities();
                List<USP_PM_GET_FEEDBACK_Result> pmfbList = dbContext.USP_PM_GET_FEEDBACK(Role, State, District, MaxfbID).ToList();
                return cryptObj.Encrypt(GetXMLFromObject(pmfbList));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "MABProgressAndLab.DownloadFeedbacks()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
        }

        public String DownloadreplyDetails(string roleCode, string stateCode, string districtCode, string maxReplyId, string downloadDatetime)
        {
            int State = Convert.ToInt32(cryptObj.Decrypt(stateCode));
            int District = Convert.ToInt32(cryptObj.Decrypt(districtCode));
            int Role = Convert.ToInt32(cryptObj.Decrypt(roleCode));
            int MaxRepID = Convert.ToInt32(cryptObj.Decrypt(maxReplyId));
            DateTime DownloadDate = cryptObj.Decrypt(downloadDatetime) == "0" ? Convert.ToDateTime("1990-01-01") : Convert.ToDateTime(cryptObj.Decrypt(downloadDatetime));

            try
            {
                dbContext = new PMGSYEntities();
                List<USP_PM_DOWNLOAD_REPLY_Result> pmfbList = dbContext.USP_PM_DOWNLOAD_REPLY(Role, State, District, MaxRepID, DownloadDate).ToList();
                return cryptObj.Encrypt(GetXMLFromObject(pmfbList));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "NABProgressAndLAB.DownloadreplyDetails()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
        }


        public string SubmitReplyDAL(string feedbackServerID, string replyComment, string replyStatus, string replyByUserID)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                string decFeedbackServerID = cryptObj.Decrypt(feedbackServerID);
                string decReplyComment = cryptObj.Decrypt(replyComment);
                string decReplyStatus = cryptObj.Decrypt(replyStatus);
                string decReplyByUserID = cryptObj.Decrypt(replyByUserID);

                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "SubmitReplyDAL()");
                    sw.WriteLine("feedbackServerID : " + decFeedbackServerID);
                    sw.WriteLine("replyComment : " + decReplyComment);
                    sw.WriteLine("replyStatus : " + decReplyStatus);
                    sw.WriteLine("replyByUserID : " + decReplyByUserID);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                ADMIN_FEEDBACK_REPLY model = new ADMIN_FEEDBACK_REPLY();
                model.FEED_ID = Convert.ToInt32(decFeedbackServerID);
                model.REP_ID = dbContext.ADMIN_FEEDBACK_REPLY.Any(x => x.FEED_ID == model.FEED_ID) ? dbContext.ADMIN_FEEDBACK_REPLY.Where(c => c.FEED_ID == model.FEED_ID).Max(x => x.REP_ID) + 1 : 1;
                if (dbContext.ADMIN_FEEDBACK_REPLY.Any(x => x.FEED_ID == model.FEED_ID))
                {
                    string existinFeedStatus = dbContext.ADMIN_FEEDBACK_REPLY.AsEnumerable().LastOrDefault(x => x.FEED_ID == model.FEED_ID).REP_STATUS;
                    if (existinFeedStatus == "F" && decReplyStatus == "I")
                    {
                        return cryptObj.Encrypt("5");// After final reply Intrim reply is not allowed
                    }

                    if (dbContext.ADMIN_FEEDBACK_REPLY.AsEnumerable().LastOrDefault(x => x.FEED_ID == model.FEED_ID).REP_STATUS == "F")
                    {
                        model.REP_STATUS = "F";
                    }
                    else
                    {
                        model.REP_STATUS = decReplyStatus;
                    }
                }
                else
                {
                    model.REP_STATUS = decReplyStatus;
                }
                model.REP_DATE = DateTime.Now;
                model.REP_USER_ID = Convert.ToInt32(decReplyByUserID);
                model.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] ?? "0.0";
                model.REP_COMMENT = decReplyComment;
                dbContext.ADMIN_FEEDBACK_REPLY.Add(model);

                ADMIN_FEEDBACK fb = dbContext.ADMIN_FEEDBACK.FirstOrDefault(x => x.FEED_ID == model.FEED_ID);

                if (fb.FEED_STATUS.Equals("N", StringComparison.OrdinalIgnoreCase))
                {
                    fb.FEED_STATUS = decReplyStatus;
                }
                if (fb.FEED_STATUS == "F")
                {
                    fb.FEED_STATUS = "F";
                }

                dbContext.Entry<ADMIN_FEEDBACK>(fb).State = System.Data.Entity.EntityState.Modified;
                int rowAffected = dbContext.SaveChanges();
                return cryptObj.Encrypt(rowAffected > 0 ? "_1_1_" + model.REP_ID + "_" + DateTime.Now : "0");

            }
            catch (UpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "SubmitReplyDAL()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "SubmitReplyDAL()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "SubmitReplyDAL()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public String UploadReplyImage(byte[] imgData, string feedbackServerCode, string replyServerCode, string latitude, string longitude)
        {
            dbContext = new PMGSYEntities();
            int insertCount = 0;
            bool isTransComplete = false;
            int maxFileID = 0;
            string storageRoot = string.Empty;
            int currentImgNumber = 0;
            try
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("feedbackServerCode : " + feedbackServerCode);
                    sw.WriteLine("replyServerCode : " + replyServerCode);
                    sw.WriteLine("latitude : " + latitude);
                    sw.WriteLine("longitude : " + longitude);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }


                string decFeedbackServerCode = cryptObj.Decrypt(feedbackServerCode);
                string decReplyServerCode = cryptObj.Decrypt(replyServerCode);
                string decLatitude = cryptObj.Decrypt(latitude);
                string decLongitude = cryptObj.Decrypt(longitude);


                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("decFeedbackServerCode : " + decFeedbackServerCode);
                    sw.WriteLine("decReplyServerCode : " + decReplyServerCode);
                    sw.WriteLine("decLatitude : " + decLatitude);
                    sw.WriteLine("decLongitude : " + decLongitude);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                if (decLatitude.Equals("") && decLongitude.Equals(""))
                {
                    decLatitude = "0";
                    decLongitude = "0";
                }

                decimal _decLatitude = Convert.ToDecimal(decLatitude);
                decimal _decLongitude = Convert.ToDecimal(decLongitude);

                if (dbContext.ADMIN_FEEDBACK_REPLY_FILES.Count() == 0)
                    maxFileID = 0;
                else
                    maxFileID = (from c in dbContext.ADMIN_FEEDBACK_REPLY_FILES select (Int32?)c.FILE_ID ?? 0).Max();

                var fileId = maxFileID + 1;

                string fileName = fileId + "_" + decFeedbackServerCode + "_" + decReplyServerCode + ".jpeg";

                storageRoot = ConfigurationManager.AppSettings["FEEDBACK_REPLY_FILE_UPLOAD"];
                if (!Directory.Exists(storageRoot))
                    Directory.CreateDirectory(storageRoot);
                //------------------- Insert Image Details In Databse  ---------------------------- //
                using (var scope = new TransactionScope())
                {
                    // This condition is to check, is Total number of images already uploaded


                    ADMIN_FEEDBACK_REPLY_FILES afrf = new ADMIN_FEEDBACK_REPLY_FILES();
                    afrf.FILE_ID = fileId;
                    afrf.FEED_ID = Int32.Parse(decFeedbackServerCode);
                    afrf.REP_ID = Int32.Parse(decReplyServerCode);
                    afrf.FILE_TYPE = "I";
                    afrf.FILE_NAME = fileName;
                    afrf.FILE_DESC = "NA";
                    afrf.FILE_UPLOAD_DATE = DateTime.Now;
                    afrf.FILE_LAT = Decimal.Parse(decLatitude);
                    afrf.FILE_LONG = Decimal.Parse(decLongitude);

                    dbContext.ADMIN_FEEDBACK_REPLY_FILES.Add(afrf);
                    insertCount = dbContext.SaveChanges();

                    scope.Complete();
                    isTransComplete = true;
                }

                //------------------- Save Image to Disk ---------------------------- //

                if (insertCount > 0 && isTransComplete == true)
                {
                    var fullPath = Path.Combine(storageRoot, fileName);
                    var thumbnailPath = Path.Combine(storageRoot, "thumbnails");
                    var fullThumbnailPath = Path.Combine(thumbnailPath, fileName);
                    int saveImgCnt = new CommonFunctions().SaveImage(imgData, storageRoot, fullPath, fullThumbnailPath);

                    if (saveImgCnt > 0)
                    {
                        return cryptObj.Encrypt("_1$" + insertCount + "$" + saveImgCnt + "$" + decReplyServerCode + "$" + fileId);
                    }
                    else
                    {
                        return cryptObj.Encrypt("-1");
                    }
                }
                else
                {
                    return cryptObj.Encrypt("-1");
                }

                //--------------------------------------------------------------------- //
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadReplyImage()");
                    sw.WriteLine("Inner Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Method : " + "UploadReplyImage()");
                            sw.WriteLine("Database Exception : " + ve.ErrorMessage.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadReplyImage()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region Avinash-2   27_07_2018   Work List
        public string GetWorkDetailsForCaptureLocation(string userName, string adminQMCode, string adminBlockCode, string check)
        {
            dbContext = new PMGSYEntities();
            int mord_AdminNDcode = 0;
            int adminNDCode = 0;
            Int32 BlockCode = 0;
            try
            {

                String userNameDec = cryptObj.Decrypt(userName).ToString();
                String cmord = cryptObj.Decrypt(check).ToString();



                //String adminQMCodeDec = cryptObj.Decrypt(adminQMCode).ToString();

                if (cmord == "mord")
                {
                    String adminQMCodeDec = cryptObj.Decrypt(adminQMCode).ToString();

                    Int32 DistrictCode = Convert.ToInt32(adminQMCodeDec);

                    String adminBlockCodeDec = cryptObj.Decrypt(adminBlockCode).ToString();
                    BlockCode = Convert.ToInt32(adminBlockCodeDec);


                    ADMIN_DEPARTMENT obj = dbContext.ADMIN_DEPARTMENT.Where(x => x.MAST_DISTRICT_CODE == DistrictCode).FirstOrDefault();
                    if (obj == null)
                    {
                        return cryptObj.Encrypt("0");
                    }
                    mord_AdminNDcode = Convert.ToInt32(obj.ADMIN_ND_CODE);
                    adminNDCode = mord_AdminNDcode;


                }
                else
                {

                    String block = cryptObj.Decrypt(adminBlockCode).ToString();
                    if (block == "B")
                    {
                        BlockCode = 0;
                    }
                    adminNDCode = Convert.ToInt16(dbContext.UM_User_Master.Where(c => c.UserName == userNameDec).Select(c => c.Admin_ND_Code).First());



                }



                //int userIDDecNew = Convert.ToInt16(userIDDec);
                //int adminNDCode = Convert.ToInt16(dbContext.UM_User_Master.Where(c => c.UserID == userIDDecNew).Select(c => c.Admin_ND_Code).First());


                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("adminNDCode :" + adminNDCode);
                    sw.Close();
                }

                List<USP_GET_ROAD_LIST_FOR_CAPTURE_LCOATION_Result> itemList = new List<USP_GET_ROAD_LIST_FOR_CAPTURE_LCOATION_Result>();

                itemList = dbContext.USP_GET_ROAD_LIST_FOR_CAPTURE_LCOATION(adminNDCode, BlockCode).ToList<USP_GET_ROAD_LIST_FOR_CAPTURE_LCOATION_Result>();

                if (itemList.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }


                return cryptObj.Encrypt(GetXMLFromObject(itemList).Replace("&amp;", " and "));

            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetWorkDetailsForCaptureLocation()");
                    sw.WriteLine("Database Exception : " + ex.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetWorkDetailsForCaptureLocation()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Avinash-3   27_07_2018 Insert_Location
        public string InsertDetailsForCaptureLocation(string userID, string prRoadeCode, string propType, string startPoint, string cdworkpoint, string midPoint, string endPoint, string captureDate)
        {
            dbContext = new PMGSYEntities();

            string pointFlag = "S";

            string startPointLatitude;
            string startPointLongitude;


            string CDPointLatitude;
            string CDPointLongitude;


            propType = string.Empty;


            string endPointLatitude;
            string endPointLongitude;

            Int32 maxCode = 0;
            try
            {
                int devUserID = Convert.ToInt32(cryptObj.Decrypt(userID));
                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadeCode));
                string decStartPoint = cryptObj.Decrypt(startPoint);
                //Avinash
                string deccdwork = cryptObj.Decrypt(cdworkpoint);


                string decMidPoint = cryptObj.Decrypt(midPoint);
                string decEndPoint = cryptObj.Decrypt(endPoint);
                string decCaptureDate = cryptObj.Decrypt(captureDate);

                string[] startPointArr = decStartPoint.Split(',');
                string[] CDWorkPointArr = deccdwork.Split(',');
                string[] endPointArr = decEndPoint.Split(',');


                //Start 
                if (startPointArr.Length == 2)
                {
                    startPointLatitude = startPointArr[0].Trim();
                    startPointLongitude = startPointArr[1].Trim();
                }
                else
                {
                    startPointLatitude = "0.0";
                    startPointLongitude = "0.0";
                }




                //CD Work Single CD Work  Avinash

                //--------------------Single CD--------------------------------------------
                /*
                if (CDWorkPointArr.Length == 2)
                {
                    CDPointLatitude = CDWorkPointArr[0].Trim();
                    CDPointLongitude = CDWorkPointArr[1].Trim();
                }
                else
                {
                    CDPointLatitude = "0.0";
                    CDPointLongitude = "0.0";
                }
                 */
                //--------------------Single CD--------------------------------------------





                //End Work

                if (endPointArr.Length == 2)
                {
                    endPointLatitude = endPointArr[0];
                    endPointLongitude = endPointArr[1];
                }
                else
                {
                    endPointLatitude = "0.0";
                    endPointLongitude = "0.0";
                }

                string[] midPointAr = decMidPoint.Split('#');

                string[] cdworkAr = deccdwork.Split('#');



                //decimal decStartLatitude = Convert.ToDecimal(cryptObj.Decrypt(startLatitude) == "" ? "0.0" : cryptObj.Decrypt(startLatitude));
                //decimal decStartLongitude = Convert.ToDecimal(cryptObj.Decrypt(startLongitude) == "" ? "0.0" : cryptObj.Decrypt(startLongitude));
                //decimal decEndLatitude = Convert.ToDecimal(cryptObj.Decrypt(endLatitude) == "" ? "0.0" : cryptObj.Decrypt(endLatitude));
                //decimal decEndLongitude = Convert.ToDecimal(cryptObj.Decrypt(endLongitude) == "" ? "0.0" : cryptObj.Decrypt(endLongitude));



                if (dbContext.EXEC_LOCATION_DETAILS.Any())
                {
                    maxCode = dbContext.EXEC_LOCATION_DETAILS.Select(c => c.EXEC_LOC_ID).Max();
                }


                if (pointFlag == "S")
                {
                    EXEC_LOCATION_DETAILS locationDetails = new EXEC_LOCATION_DETAILS();
                    locationDetails.EXEC_LOC_ID = maxCode + 1;
                    locationDetails.IMS_PR_ROAD_CODE = decPrRoadCode;
                    locationDetails.EXEC_LOC_FLAG = "S";
                    locationDetails.EXEC_LOC_LONG = startPointLongitude;
                    locationDetails.EXEC_LOC_LAT = startPointLatitude;
                    locationDetails.EXEC_UPLOAD_DATETIME = Convert.ToDateTime(decCaptureDate);
                    locationDetails.USERID = devUserID;
                    dbContext.EXEC_LOCATION_DETAILS.Add(locationDetails);
                    dbContext.SaveChanges();
                    pointFlag = "C";

                }


                //----------------------------Single CD-------------------------------------------------------
                /*
                if (pointFlag == "C")
                {
                    EXEC_LOCATION_DETAILS locationDetails = new EXEC_LOCATION_DETAILS();
                    locationDetails.EXEC_LOC_ID = dbContext.EXEC_LOCATION_DETAILS.Select(c => c.EXEC_LOC_ID).Max() +1;
                    locationDetails.IMS_PR_ROAD_CODE = decPrRoadCode;
                    locationDetails.EXEC_LOC_FLAG = "C";
                    locationDetails.EXEC_LOC_LONG = CDPointLongitude;
                    locationDetails.EXEC_LOC_LAT = CDPointLatitude;
                    locationDetails.EXEC_UPLOAD_DATETIME = Convert.ToDateTime(decCaptureDate);
                    locationDetails.USERID = devUserID;
                    dbContext.EXEC_LOCATION_DETAILS.Add(locationDetails);
                    dbContext.SaveChanges();
                    pointFlag = "M";

                }
                 */

                //----------------------------Single CD-------------------------------------------------------





                if (pointFlag == "C")
                {
                    maxCode = dbContext.EXEC_LOCATION_DETAILS.Select(c => c.EXEC_LOC_ID).Max();
                    int maxCount = 1;
                    for (int i = 0; i < cdworkAr.Length; i++)
                    {
                        EXEC_LOCATION_DETAILS locationDetails = new EXEC_LOCATION_DETAILS();
                        string[] midPointLatLongArr1 = cdworkAr[i].Split(',');

                        locationDetails.EXEC_LOC_ID = maxCode + maxCount;
                        locationDetails.IMS_PR_ROAD_CODE = decPrRoadCode;
                        locationDetails.EXEC_LOC_FLAG = "C";

                        if (midPointLatLongArr1.Length == 2)
                        {
                            locationDetails.EXEC_LOC_LONG = midPointLatLongArr1[1];
                            locationDetails.EXEC_LOC_LAT = midPointLatLongArr1[0];
                        }
                        else
                        {
                            locationDetails.EXEC_LOC_LONG = "0.0";
                            locationDetails.EXEC_LOC_LAT = "0.0";
                        }
                        locationDetails.EXEC_UPLOAD_DATETIME = Convert.ToDateTime(decCaptureDate);
                        locationDetails.USERID = devUserID;
                        dbContext.EXEC_LOCATION_DETAILS.Add(locationDetails);
                        dbContext.SaveChanges();

                        maxCount++;
                    }
                    pointFlag = "M";

                }

                if (pointFlag == "M")
                {
                    maxCode = dbContext.EXEC_LOCATION_DETAILS.Select(c => c.EXEC_LOC_ID).Max();
                    int maxCount = 1;
                    for (int i = 0; i < midPointAr.Length; i++)
                    {
                        EXEC_LOCATION_DETAILS locationDetails = new EXEC_LOCATION_DETAILS();
                        string[] midPointLatLongArr = midPointAr[i].Split(',');

                        locationDetails.EXEC_LOC_ID = maxCode + maxCount;
                        locationDetails.IMS_PR_ROAD_CODE = decPrRoadCode;
                        locationDetails.EXEC_LOC_FLAG = "M";

                        if (midPointLatLongArr.Length == 2)
                        {
                            locationDetails.EXEC_LOC_LONG = midPointLatLongArr[1];
                            locationDetails.EXEC_LOC_LAT = midPointLatLongArr[0];
                        }
                        else
                        {
                            locationDetails.EXEC_LOC_LONG = "0.0";
                            locationDetails.EXEC_LOC_LAT = "0.0";
                        }
                        locationDetails.EXEC_UPLOAD_DATETIME = Convert.ToDateTime(decCaptureDate);
                        locationDetails.USERID = devUserID;
                        dbContext.EXEC_LOCATION_DETAILS.Add(locationDetails);
                        dbContext.SaveChanges();

                        maxCount++;
                    }
                    pointFlag = "E";

                }



                if (pointFlag == "E")
                {
                    EXEC_LOCATION_DETAILS locationDetails = new EXEC_LOCATION_DETAILS();

                    maxCode = dbContext.EXEC_LOCATION_DETAILS.Select(c => c.EXEC_LOC_ID).Max();

                    locationDetails.EXEC_LOC_ID = maxCode + 1;
                    locationDetails.IMS_PR_ROAD_CODE = decPrRoadCode;
                    locationDetails.EXEC_LOC_FLAG = "E";
                    locationDetails.EXEC_LOC_LONG = endPointLongitude;
                    locationDetails.EXEC_LOC_LAT = endPointLatitude;
                    locationDetails.EXEC_UPLOAD_DATETIME = Convert.ToDateTime(decCaptureDate);
                    locationDetails.USERID = devUserID;
                    dbContext.EXEC_LOCATION_DETAILS.Add(locationDetails);
                    dbContext.SaveChanges();
                    pointFlag = "F";
                }

                if (pointFlag == "F")
                {
                    return cryptObj.Encrypt("1");
                }

                return cryptObj.Encrypt("0");
            }
            catch (OptimisticConcurrencyException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertDetailsForCaptureLocation()");
                    sw.WriteLine("Cunccurrency Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (UpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertDetailsForCaptureLocation()");
                    sw.WriteLine("Database Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertDetailsForCaptureLocation()");
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Avinash-4 26_09_2018 Getting States
        public string GetStateList()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<StateDetailModel> objStateDetailModel = new List<StateDetailModel>();

                List<MASTER_STATE> execItenLst = dbContext.MASTER_STATE.ToList();

                if (execItenLst.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }


                foreach (MASTER_STATE item in execItenLst)
                {
                    StateDetailModel model = new StateDetailModel();
                    model.MAST_STATE_CODE = item.MAST_STATE_CODE.ToString();
                    model.MAST_STATE_NAME = item.MAST_STATE_NAME;
                    objStateDetailModel.Add(model);
                }
                return cryptObj.Encrypt(GetXMLFromObject(objStateDetailModel));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetStateList()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Avinash-4 26_09_2018 Getting District
        public string GetDistrictList(string StateCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<DistrictDetailModel> objDistrictDetailModel = new List<DistrictDetailModel>();

                Int32 stateCode = Convert.ToInt32(cryptObj.Decrypt(StateCode));

                List<MASTER_DISTRICT> execItenLst = dbContext.MASTER_DISTRICT.Where(x => x.MAST_STATE_CODE == stateCode && x.MAST_DISTRICT_ACTIVE == "Y").ToList();


                if (execItenLst.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                foreach (MASTER_DISTRICT item in execItenLst)
                {
                    DistrictDetailModel model = new DistrictDetailModel();
                    model.MAST_DISTRICT_CODE = item.MAST_DISTRICT_CODE.ToString();
                    model.MAST_DISTRICT_NAME = item.MAST_DISTRICT_NAME;
                    objDistrictDetailModel.Add(model);
                }
                return cryptObj.Encrypt(GetXMLFromObject(objDistrictDetailModel));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetStateList()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Avinash-6 26_10_2018 Getting Blocks
        public string GetBlockList(string DistrictCode)
        {

            try
            {
                dbContext = new PMGSYEntities();
                List<BlockDetailModel> objBlockDetailModel = new List<BlockDetailModel>();

                Int32 districtCode = Convert.ToInt32(cryptObj.Decrypt(DistrictCode));

                List<MASTER_BLOCK> execItenLst = dbContext.MASTER_BLOCK.Where(x => x.MAST_DISTRICT_CODE == districtCode && x.MAST_BLOCK_ACTIVE == "Y").ToList();


                if (execItenLst.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                foreach (MASTER_BLOCK item in execItenLst)
                {
                    BlockDetailModel model = new BlockDetailModel();
                    model.MAST_BLOCK_CODE = item.MAST_BLOCK_CODE.ToString();
                    model.MAST_BLOCK_NAME = item.MAST_BLOCK_NAME;
                    objBlockDetailModel.Add(model);
                }
                return cryptObj.Encrypt(GetXMLFromObject(objBlockDetailModel));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetStateList()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }



        }
        #endregion

        #region Avinash-7 08_01_2019 Facility Development
        public string GetFacilityDetailsForCaptureLocation(string districtCode, string blockCode)
        {
            dbContext = new PMGSYEntities();
            int adminNDCode = 0;
            Int32 BlockCode = 0;
            Int32 DistrictCode = 0;

            try
            {

                //String userNameDec = cryptObj.Decrypt(userName).ToString();
                // String cmord = cryptObj.Decrypt(check).ToString();
                //adminNDCode = Convert.ToInt16(dbContext.UM_User_Master.Where(c => c.UserName == userNameDec).Select(c => c.Admin_ND_Code).First());
                if (!String.IsNullOrEmpty(blockCode))
                {
                    String DistrictCodeDec = cryptObj.Decrypt(districtCode).ToString();
                    DistrictCode = Convert.ToInt32(DistrictCodeDec);

                    String BlockCodeDec = cryptObj.Decrypt(blockCode).ToString();
                    BlockCode = Convert.ToInt32(BlockCodeDec);
                }

                if (string.IsNullOrEmpty(districtCode) || string.IsNullOrEmpty(blockCode) || DistrictCode == 0 || BlockCode == 0)
                {
                    return cryptObj.Encrypt("-1");
                }

                #region Getoffline Facility Details

                ////GETTING ALL FACILITY CATEGORY WHERE PARENT ID IS NULL
                //List<FacilityOfflineDetailModel> lstMASTER_FACILITY_CATEGORYStorage = new List<FacilityOfflineDetailModel>();
                //List<MASTER_FACILITY_CATEGORY> lstMASTER_FACILITY_CATEGORY = new List<MASTER_FACILITY_CATEGORY>();

                //lstMASTER_FACILITY_CATEGORY = dbContext.MASTER_FACILITY_CATEGORY.Where(x => x.MASTER_FACILITY_PARENT_ID == null).ToList();
                //foreach (var item in lstMASTER_FACILITY_CATEGORY)
                //{
                //    FacilityOfflineDetailModel objFacilityOfflineDetailModel1 = new FacilityOfflineDetailModel();
                //    objFacilityOfflineDetailModel1.MAST_FACILITY_CATEGORY_ID = Convert.ToString(item.MASTER_FACILITY_CATEGORT_ID);
                //    objFacilityOfflineDetailModel1.MAST_FACILITY_CATEGORY_NAME = item.MASTER_FACILITY_CATEGORY_NAME;
                //    lstMASTER_FACILITY_CATEGORYStorage.Add(objFacilityOfflineDetailModel1);
                //}
                //string XMlFacilityCategory = GetXMLFromObject(lstMASTER_FACILITY_CATEGORYStorage).Replace("&amp;", " and ");




                ////GETTING ALL FACILITY TYPE WHERE PARENT ID IS NOT NULL

                //List<FacilityOfflineDetailModel> lstMASTER_FACILITY_TYPEStorage = new List<FacilityOfflineDetailModel>();
                //List<MASTER_FACILITY_CATEGORY> lstMASTER_FACILITY_TYPE = new List<MASTER_FACILITY_CATEGORY>();

                //lstMASTER_FACILITY_TYPE = dbContext.MASTER_FACILITY_CATEGORY.Where(x => x.MASTER_FACILITY_PARENT_ID != null).ToList();
                //foreach (var item in lstMASTER_FACILITY_TYPE)
                //{
                //    FacilityOfflineDetailModel objFacilityOfflineDetailModel2 = new FacilityOfflineDetailModel();
                //    objFacilityOfflineDetailModel2.MAST_FACILITY_CATEGORY_ID1 = Convert.ToString(item.MASTER_FACILITY_CATEGORT_ID);
                //    objFacilityOfflineDetailModel2.MAST_FACILITY_PARENT_ID = Convert.ToString(item.MASTER_FACILITY_PARENT_ID);
                //    objFacilityOfflineDetailModel2.MAST_FACILITY_CATEGORY_NAME1 = item.MASTER_FACILITY_CATEGORY_NAME;
                //    lstMASTER_FACILITY_TYPEStorage.Add(objFacilityOfflineDetailModel2);
                //}
                //string XMlFacilityType = GetXMLFromObject(lstMASTER_FACILITY_TYPEStorage).Replace("&amp;", " and ");

                ////GETTING DISTRICT NAME BASED ON DISTRICT CODE
                //FacilityOfflineDetailModel objFacilityOfflineDetailModel3 = new FacilityOfflineDetailModel();
                //MASTER_DISTRICT objMASTER_DISTRICT = new MASTER_DISTRICT();
                //objMASTER_DISTRICT = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == DistrictCode).FirstOrDefault();
                //objFacilityOfflineDetailModel3.MAST_DISTRICT_CODE = Convert.ToString(objMASTER_DISTRICT.MAST_DISTRICT_CODE);
                //objFacilityOfflineDetailModel3.MAST_DISTRICT_NAME = objMASTER_DISTRICT.MAST_DISTRICT_NAME;
                //string XMlFacilityDistrict = GetXMLFromObject(objFacilityOfflineDetailModel3).Replace("&amp;", " and ");





                ////GETTING BLOCK NAME BASED ON BLOCK CODE
                //FacilityOfflineDetailModel objFacilityOfflineDetailModel4 = new FacilityOfflineDetailModel();
                //MASTER_BLOCK objMASTER_BLOCK = new MASTER_BLOCK();
                //objMASTER_BLOCK = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == BlockCode).FirstOrDefault();
                //objFacilityOfflineDetailModel4.MAST_BLOCK_CODE = Convert.ToString(objMASTER_BLOCK.MAST_BLOCK_CODE);
                //objFacilityOfflineDetailModel4.MAST_DISTRICT_CODE1 = Convert.ToString(objMASTER_BLOCK.MAST_DISTRICT_CODE);
                //objFacilityOfflineDetailModel4.MAST_BLOCK_NAME = objMASTER_BLOCK.MAST_BLOCK_NAME;
                //string XMlFacilityBlock = GetXMLFromObject(objFacilityOfflineDetailModel4).Replace("&amp;", " and ");

                ////GETTING HABITATION BASED ON BLOCK CODE
                //var villageList = dbContext.MASTER_VILLAGE.Where(v => v.MAST_BLOCK_CODE == BlockCode && v.MAST_VILLAGE_ACTIVE == "Y").OrderBy(v => v.MAST_VILLAGE_NAME).Select(s => s.MAST_VILLAGE_CODE).ToList();
                //var habList = (from hab in dbContext.MASTER_HABITATIONS
                //               where villageList.Contains(hab.MAST_VILLAGE_CODE)
                //               select new
                //               {
                //                   hab.MAST_HAB_NAME,
                //                   hab.MAST_HAB_CODE,

                //               }).ToList();

                //if (habList.Count == 0)
                //{
                //    return cryptObj.Encrypt("0");
                //}


                //List<FacilityOfflineDetailModel> lstMASTER_FACILITY_HABITATIONStorage = new List<FacilityOfflineDetailModel>();

                //foreach (var item in habList)
                //{
                //    FacilityOfflineDetailModel objFacilityOfflineDetailModel5 = new FacilityOfflineDetailModel();
                //    objFacilityOfflineDetailModel5.MAST_HAB_CODE = item.MAST_HAB_CODE.ToString();
                //    objFacilityOfflineDetailModel5.MAST_HAB_NAME = item.MAST_HAB_NAME;

                //    lstMASTER_FACILITY_HABITATIONStorage.Add(objFacilityOfflineDetailModel5);
                //}
                //string XMlFacilityHabitation = GetXMLFromObject(lstMASTER_FACILITY_HABITATIONStorage).Replace("&amp;", " and ");
                //string formValueString = XMlFacilityCategory + XMlFacilityType + XMlFacilityDistrict + XMlFacilityBlock + XMlFacilityHabitation;

                #endregion



                List<USP_HABITATION_FACILITY_Result> itemList = new List<USP_HABITATION_FACILITY_Result>();

                itemList = dbContext.USP_HABITATION_FACILITY(DistrictCode, BlockCode).ToList<USP_HABITATION_FACILITY_Result>();
                itemList = itemList.OrderBy(obj => obj.MAST_HAB_NAME).ToList<USP_HABITATION_FACILITY_Result>(); //added by abhinav pathak on 23-08-2019

                if (itemList.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                //string spStringValue = GetXMLFromObject(itemList).Replace("&amp;", " and ");
                //string CombineValue = spStringValue + "~"  + formValueString;
                //return cryptObj.Encrypt(CombineValue);

                return cryptObj.Encrypt(GetXMLFromObject(itemList).Replace("&amp;", " and "));


            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetFacilityDetailsForCaptureLocation()");
                    sw.WriteLine("Database Exception : " + ex.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetFacilityDetailsForCaptureLocation()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetDistrictFromUserIDForFacility(string UserID)
        {
            UM_User_Master userMaster = null;
            try
            {
                dbContext = new PMGSYEntities();
                userMaster = new UM_User_Master();
                List<DistrictDetailModel> objDistrictDetailModel = new List<DistrictDetailModel>();
                Int32 userID = Convert.ToInt32(cryptObj.Decrypt(UserID));

                userMaster = dbContext.UM_User_Master.Where(x => x.UserID == userID).FirstOrDefault();
                if (userMaster == null)
                {
                    return cryptObj.Encrypt("0");
                }

                List<MASTER_DISTRICT> execItenLst = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == userMaster.Mast_District_Code && x.MAST_DISTRICT_ACTIVE == "Y").ToList();



                if (execItenLst.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                foreach (MASTER_DISTRICT item in execItenLst)
                {
                    DistrictDetailModel model = new DistrictDetailModel();
                    model.MAST_DISTRICT_CODE = item.MAST_DISTRICT_CODE.ToString();
                    model.MAST_DISTRICT_NAME = item.MAST_DISTRICT_NAME;
                    objDistrictDetailModel.Add(model);
                }
                return cryptObj.Encrypt(GetXMLFromObject(objDistrictDetailModel));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetDistrictFromUserIDForFacility()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetFacilityCategoryList()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<FacilityCategoryDetailModel> objFacilityCategoryDetailModel = new List<FacilityCategoryDetailModel>();

                List<MASTER_FACILITY_CATEGORY> execItenLst = dbContext.MASTER_FACILITY_CATEGORY.Where(x => x.MASTER_FACILITY_PARENT_ID == null).ToList();

                if (execItenLst.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }


                foreach (MASTER_FACILITY_CATEGORY item in execItenLst)
                {
                    FacilityCategoryDetailModel model = new FacilityCategoryDetailModel();
                    model.FACILITY_CATEGORY_CODE = item.MASTER_FACILITY_CATEGORT_ID.ToString();
                    model.FACILITY_CATEGORY_NAME = item.MASTER_FACILITY_CATEGORY_NAME;
                    objFacilityCategoryDetailModel.Add(model);
                }
                objFacilityCategoryDetailModel = objFacilityCategoryDetailModel.OrderBy(z => z.FACILITY_CATEGORY_NAME).ToList();
                return cryptObj.Encrypt(GetXMLFromObject(objFacilityCategoryDetailModel));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetFacilityCategoryList()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetFacilityTypeList(string FacilityCategoryID)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<FacilityTypeDetailModel> objFacilityTypeDetailModel = new List<FacilityTypeDetailModel>();

                //Int32 facilityCategoryID = Convert.ToInt32(cryptObj.Decrypt(FacilityCategoryID));

                //if (facilityCategoryID == 0)
                //{
                //    return cryptObj.Encrypt("0");
                //}
                //if (string.IsNullOrEmpty(FacilityCategoryID))
                //{
                //    return cryptObj.Encrypt("0");
                //}

                List<MASTER_FACILITY_CATEGORY> execItenLst = dbContext.MASTER_FACILITY_CATEGORY.Where(x => x.MASTER_FACILITY_PARENT_ID != null).ToList();

                if (execItenLst.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }


                foreach (MASTER_FACILITY_CATEGORY item in execItenLst)
                {
                    FacilityTypeDetailModel model = new FacilityTypeDetailModel();
                    model.FACILITY_TYPE_CODE = item.MASTER_FACILITY_CATEGORT_ID.ToString();
                    model.FACILITY_TYPE_NAME = item.MASTER_FACILITY_CATEGORY_NAME;
                    model.FACILITY_PARENT_ID = item.MASTER_FACILITY_PARENT_ID.ToString();
                    objFacilityTypeDetailModel.Add(model);
                }
                objFacilityTypeDetailModel = objFacilityTypeDetailModel.OrderBy(z=>z.FACILITY_TYPE_NAME).ToList();
                return cryptObj.Encrypt(GetXMLFromObject(objFacilityTypeDetailModel));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetFacilityTypeList()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetFacilityHabitationList(string FacilityBlockCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<FacilityHabitationDetailModel> objFacilityHabitationDetailModel = new List<FacilityHabitationDetailModel>();

                Int32 facilityBlockCode = Convert.ToInt32(cryptObj.Decrypt(FacilityBlockCode));

                if (facilityBlockCode == 0)
                {
                    return cryptObj.Encrypt("0");
                }
                if (string.IsNullOrEmpty(FacilityBlockCode))
                {
                    return cryptObj.Encrypt("0");
                }

                var villageList = dbContext.MASTER_VILLAGE.Where(v => v.MAST_BLOCK_CODE == facilityBlockCode && v.MAST_VILLAGE_ACTIVE == "Y").OrderBy(v => v.MAST_VILLAGE_NAME).Select(s => s.MAST_VILLAGE_CODE).ToList();

                // As per suggestion of Srinivasa Sir on 03 Aug 2020, Active habitations of year 2011 with population more than 0 will be populated
                var habList = (from hab in dbContext.MASTER_HABITATIONS
                               join details in dbContext.MASTER_HABITATIONS_DETAILS on hab.MAST_HAB_CODE equals details.MAST_HAB_CODE
                               where villageList.Contains(hab.MAST_VILLAGE_CODE) && hab.MAST_HABITATION_ACTIVE=="Y" && details.MAST_YEAR==2011 && details.MAST_HAB_TOT_POP>0
                               select new
                               {
                                   hab.MAST_HAB_NAME,
                                   hab.MAST_HAB_CODE,

                               }).ToList();

                // Below is Commented on 03 Aug 2020
                //var habList = (from hab in dbContext.MASTER_HABITATIONS
                //               where villageList.Contains(hab.MAST_VILLAGE_CODE)
                //               select new
                //               {
                //                   hab.MAST_HAB_NAME,
                //                   hab.MAST_HAB_CODE,

                //               }).ToList();

                if (habList.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                foreach (var item in habList)
                {
                    FacilityHabitationDetailModel model = new FacilityHabitationDetailModel();
                    model.FACILITY_HABITATION_CODE = item.MAST_HAB_CODE.ToString();
                    model.FACILITY_HABITATION_NAME = item.MAST_HAB_NAME;
                    objFacilityHabitationDetailModel.Add(model);
                }
                objFacilityHabitationDetailModel = objFacilityHabitationDetailModel.OrderBy(z=>z.FACILITY_HABITATION_NAME).ToList();
                return cryptObj.Encrypt(GetXMLFromObject(objFacilityHabitationDetailModel));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetFacilityHabitationList()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }


        }

        //public string InsertFacilityDetailsForCaptureLocation(string mastfacilityID, string factilityCategoryCode, string factilityTypeCode, string factilityDistrictCode, string factilityBlockCode, string factilityLat, string factilityLong, string factilityName, string factilityAddress, string factilityHabitationCode, string userID, string factilitybase64String, string factilityPinCode)
        //{

        //    MASTER_FACILITY objMastFacility = null;
        //    FACILITY_HABITATION_MAPPING objMapping = null;

        //    Int32 FacilityCategoryCode = 0;
        //    Int32 FacilityTypeCode = 0;
        //    Int32 FacilityDistrictCode = 0;
        //    Int32 FacilityBlockCode = 0;
        //    decimal FacilityLatitude = 0;
        //    decimal FacilityLongitude = 0;
        //    string FacilityName = string.Empty;
        //    string FacilityAddress = string.Empty;
        //    Int32 FacilityHabitationCode = 0;
        //    Int32 UserID = 0;
        //    Int32 filenameId = 0;
        //    Int32 Pincode = 0;

        //     UserID = Convert.ToInt32(cryptObj.Decrypt(userID));
        //    //UserID = 6;

        //    //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //    //{
        //    //    sw.WriteLine("____________________________________________________");
        //    //    sw.WriteLine("step1 " + UserID.ToString());
        //    //    sw.WriteLine("____________________________________________________");
        //    //    sw.Close();
        //    //}

        //    try
        //    {
        //        //using (TransactionScope ts = new TransactionScope())
        //        //{
        //            dbContext = new PMGSYEntities();
        //            objMastFacility = new MASTER_FACILITY();
        //            objMapping = new FACILITY_HABITATION_MAPPING();

        //            if (String.IsNullOrEmpty(factilityCategoryCode) || String.IsNullOrEmpty(factilityTypeCode) || String.IsNullOrEmpty(factilityDistrictCode) || String.IsNullOrEmpty(factilityBlockCode) || String.IsNullOrEmpty(factilityLat) || String.IsNullOrEmpty(factilityLong) || String.IsNullOrEmpty(factilityName) || String.IsNullOrEmpty(factilityAddress) || String.IsNullOrEmpty(userID) || String.IsNullOrEmpty(factilityHabitationCode) || String.IsNullOrEmpty(factilityPinCode))
        //            {
        //                return cryptObj.Encrypt("-1");
        //            }

        //             Int32 mastFacilityId = Convert.ToInt32(cryptObj.Decrypt(mastfacilityID));

        //            // Int32 mastFacilityId = 0; 

        //            if (mastFacilityId == 0)  //new Add
        //            {
        //                //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                //{
        //                //    sw.WriteLine("____________________________________________________");
        //                //    sw.WriteLine("step2 " + UserID.ToString());
        //                //    sw.WriteLine("____________________________________________________");
        //                //    sw.Close();
        //                //}


        //                FacilityCategoryCode = Convert.ToInt32(cryptObj.Decrypt(factilityCategoryCode));
        //                FacilityTypeCode = Convert.ToInt32(cryptObj.Decrypt(factilityTypeCode));
        //                FacilityDistrictCode = Convert.ToInt32(cryptObj.Decrypt(factilityDistrictCode));
        //                FacilityBlockCode = Convert.ToInt32(cryptObj.Decrypt(factilityBlockCode));
        //                FacilityLatitude = Convert.ToDecimal(cryptObj.Decrypt(factilityLat));
        //                FacilityLongitude = Convert.ToDecimal(cryptObj.Decrypt(factilityLong));
        //                FacilityName = Convert.ToString(cryptObj.Decrypt(factilityName));
        //                FacilityAddress = Convert.ToString(cryptObj.Decrypt(factilityAddress));
        //                FacilityHabitationCode = Convert.ToInt32(cryptObj.Decrypt(factilityHabitationCode));
        //                //UserID = Convert.ToInt32(cryptObj.Decrypt(userID));
        //                Pincode = Convert.ToInt32(cryptObj.Decrypt(factilityPinCode));





        //                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                {
        //                    sw.WriteLine("____________________________________________________");
        //                    sw.WriteLine("FacilityHabitationCode " + FacilityHabitationCode.ToString());
        //                    sw.WriteLine("FacilityCategoryCode " + FacilityCategoryCode.ToString());
        //                    sw.WriteLine("FacilityTypeCode " + FacilityTypeCode.ToString());
        //                    sw.WriteLine("FacilityDistrictCode " + FacilityDistrictCode.ToString());
        //                    sw.WriteLine("FacilityLatitude " + FacilityLatitude.ToString());
        //                    sw.WriteLine("FacilityLongitude " + FacilityLatitude.ToString());
        //                    sw.WriteLine("FacilityName " + FacilityName.ToString());
        //                    sw.WriteLine("FacilityAddress " + FacilityAddress.ToString());
        //                    sw.WriteLine("UserID " + UserID.ToString());
        //                    sw.WriteLine("Pincode " + Pincode.ToString());
        //                    sw.WriteLine("FacilityBlockCode " + FacilityBlockCode.ToString());
        //                    sw.WriteLine("____________________________________________________");
        //                    sw.Close();
        //                }



        //                #region Validation

        //                List<FACILITY_HABITATION_MAPPING> lstMapping = new List<FACILITY_HABITATION_MAPPING>();
        //                MASTER_FACILITY master = new MASTER_FACILITY();

        //                //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                //{
        //                //    sw.WriteLine("____________________________________________________");
        //                //    sw.WriteLine("step3 A ");
        //                //    sw.WriteLine("habCode" + dbContext.FACILITY_HABITATION_MAPPING.Where(x => x.MASTER_DISTRICT_CODE == FacilityDistrictCode && x.MASTER_BLOCK_CODE == FacilityBlockCode && x.MASTER_HAB_CODE == FacilityHabitationCode).Select(c => c.MASTER_HAB_CODE));
        //                //    //sw.WriteLine(dbContext.FACILITY_HABITATION_MAPPING.Where(x => x.MASTER_DISTRICT_CODE == FacilityDistrictCode && x.MASTER_BLOCK_CODE == FacilityBlockCode && x.MASTER_HAB_CODE == FacilityHabitationCode).Any().ToString());

        //                //    sw.WriteLine("____________________________________________________");
        //                //    sw.Close();
        //                //}

        //                if (dbContext.FACILITY_HABITATION_MAPPING.Where(x => x.MASTER_DISTRICT_CODE == FacilityDistrictCode && x.MASTER_BLOCK_CODE == FacilityBlockCode && x.MASTER_HAB_CODE == FacilityHabitationCode).Any())
        //                {
        //                    //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                    //{
        //                    //    sw.WriteLine("____________________________________________________");
        //                    //    sw.WriteLine("step4  " + UserID.ToString());
        //                    //    sw.WriteLine("____________________________________________________");
        //                    //    sw.Close();
        //                    //}

        //                    lstMapping = dbContext.FACILITY_HABITATION_MAPPING.Where(x => x.MASTER_DISTRICT_CODE == FacilityDistrictCode && x.MASTER_BLOCK_CODE == FacilityBlockCode && x.MASTER_HAB_CODE == FacilityHabitationCode).ToList();

        //                    //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                    //{
        //                    //    sw.WriteLine("____________________________________________________");
        //                    //    sw.WriteLine("step5 " + UserID.ToString());
        //                    //    sw.WriteLine("____________________________________________________");
        //                    //    sw.Close();
        //                    //}
        //                    foreach (var item in lstMapping)
        //                    {
        //                        master = dbContext.MASTER_FACILITY.Where(x => x.MASTER_FACILITY_ID == item.MASTER_FACILITY_ID && x.MASTER_FACILITY_CATEGORY_ID == FacilityCategoryCode && x.MASTER_FACILITY_SUB_CATEGORY_ID == FacilityTypeCode).FirstOrDefault();

        //                        if (master != null)
        //                        {
        //                            //if (master.MASTER_FACILITY_DESC == FacilityName.ToUpper() || master.MASTER_FACILITY_DESC == FacilityName.ToLower())
        //                            //{
        //                            //    return cryptObj.Encrypt("-2");   //Facility Name already entered for selected Category,Type,District,Block and Habitation   FacilityNameAlreadyPresent(R.string.E615),

        //                            //}
        //                            string strUpper = FacilityName.ToUpper();
        //                            string strLower = FacilityName.ToLower();



        //                            if (string.Equals(master.MASTER_FACILITY_DESC, FacilityName, StringComparison.CurrentCultureIgnoreCase))
        //                            {
        //                                return cryptObj.Encrypt("-2");   //Facility Name already entered for selected Category,Type,District,Block and Habitation   FacilityNameAlreadyPresent(R.string.E615),

        //                            }


        //                        }


        //                    }
        //                }


        //                if (FacilityName.Length > 200)
        //                {
        //                    return cryptObj.Encrypt("-3");  //Only 200 characters allowed for Facility Name  FacilityNameLengthExceed(R.string.E616),
        //                }


        //                if (FacilityAddress.Length > 255)
        //                {
        //                    return cryptObj.Encrypt("-4");  //Only 255 characters allowed for Address  FacilityAddressLengthExceed(R.string.E617),

        //                }

        //                var values = new[] { "~", "`", "!", "@", "#", "$", "%", "^", "*", "(", ")", "-", "_", "+", "=", "{", "}", "[", "]", "|", ";", ":", "'", "<", ">", ",", "/", "?" };
        //                if (values.Any(FacilityName.Contains))
        //                {
        //                    //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                    //{
        //                    //    sw.WriteLine("____________________________________________________");
        //                    //    sw.WriteLine("step6 " + UserID.ToString());
        //                    //    sw.WriteLine("____________________________________________________");
        //                    //    sw.Close();
        //                    //}
        //                    return cryptObj.Encrypt("-5");  //Special symbols are not allowed in Facility Name    FacilityNameNoSpecialSymbol(R.string.E618),
        //                }


        //                var values1 = new[] { "~", "`", "!", "@", "#", "$", "%", "^", "*", "(", ")", "_", "+", "=", "{", "}", "[", "]", "|", ";", "'", "<", ">", "?", "." };
        //                if (values1.Any(FacilityAddress.Contains))
        //                {

        //                    //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                    //{
        //                    //    sw.WriteLine("____________________________________________________");
        //                    //    sw.WriteLine("step7 " + UserID.ToString());
        //                    //    sw.WriteLine("____________________________________________________");
        //                    //    sw.Close();
        //                    //}
        //                    return cryptObj.Encrypt("-6");  //Special symbols are not allowed in Address
        //                }
        //                #endregion
        //            }
        //            else //Update
        //            {
        //                FacilityLatitude = Convert.ToDecimal(cryptObj.Decrypt(factilityLat));
        //                FacilityLongitude = Convert.ToDecimal(cryptObj.Decrypt(factilityLong));
        //                UserID = Convert.ToInt32(cryptObj.Decrypt(userID));


        //            }
        //            //if (FacilityCategoryCode == 0 || FacilityTypeCode == 0 || FacilityDistrictCode == 0 || FacilityBlockCode == 0 || FacilityLatitude == 0 || FacilityLongitude == 0 || UserID == 0 || FacilityHabitationCode == 0)
        //            //{
        //            //    return cryptObj.Encrypt("-1");
        //            //}

        //            #region Master Entry
        //            objMastFacility.MASTER_FACILITY_ID = (dbContext.MASTER_FACILITY.Any() ? dbContext.MASTER_FACILITY.Max(X => X.MASTER_FACILITY_ID) : 0) + 1;
        //            if (mastFacilityId == 0)  //new...MAX
        //            {

        //                filenameId = objMastFacility.MASTER_FACILITY_ID;
        //            }
        //            else //Update
        //            {
        //                filenameId = mastFacilityId;
        //            }

        //            //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //            //{
        //            //    sw.WriteLine("____________________________________________________");
        //            //    sw.WriteLine("step8 " + filenameId);
        //            //    sw.WriteLine("userid8 " + UserID.ToString());
        //            //    sw.WriteLine("____________________________________________________");
        //            //    sw.Close();
        //            //}

        //            string fileName = filenameId.ToString() + ".jpeg";
        //            string storageRoot = ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"];
        //            //if (!Directory.Exists(storageRoot))
        //            //{
        //            //    Directory.CreateDirectory(storageRoot);
        //            //}

        //            //Save Image on DISK
        //            var fullPath = Path.Combine(storageRoot, fileName);
        //            var thumbnailPath = Path.Combine(storageRoot, "thumbnails");
        //            var fullThumbnailPath = Path.Combine(thumbnailPath, fileName);


        //            byte[] encodedDataAsBytes = System.Convert.FromBase64String(factilitybase64String);


        //            int saveImgCnt = new CommonFunctions().SaveImage(encodedDataAsBytes, storageRoot, fullPath, fullThumbnailPath);


        //            //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //            //{
        //            //    sw.WriteLine("____________________________________________________");
        //            //    sw.WriteLine("step9  " + saveImgCnt.ToString());
        //            //    sw.WriteLine("userid9 " + UserID.ToString());
        //            //    sw.WriteLine("____________________________________________________");
        //            //    sw.Close();
        //            //}

        //            if (saveImgCnt == 1)
        //            {

        //                if (mastFacilityId == 0)  //Adding new Entry...pass "0" Hardcoded)
        //                {
        //                    //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                    //{
        //                    //    sw.WriteLine("____________________________________________________");
        //                    //    sw.WriteLine("step10  - b4 saving new entry " );
        //                    //    sw.WriteLine("____________________________________________________");
        //                    //    sw.Close();
        //                    //}

        //                    //if (UserID == 3740)
        //                    //{
        //                    //    using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                    //    {
        //                    //        sw.WriteLine("____________________________________________________");
        //                    //        sw.WriteLine("step11  - Params ");
        //                    //        sw.WriteLine("FacilityCategoryCode " + FacilityCategoryCode.ToString());

        //                    //        sw.WriteLine("FacilityTypeCode " + FacilityTypeCode.ToString());
        //                    //        sw.WriteLine("FacilityName " + FacilityName);
        //                    //        sw.WriteLine("FacilityAddress " + FacilityAddress);
        //                    //        sw.WriteLine("FacilityLatitude " + FacilityLatitude.ToString());
        //                    //        sw.WriteLine("FacilityLongitude " + FacilityLongitude.ToString());
        //                    //        sw.WriteLine("UserID " + UserID.ToString());

        //                    //        sw.WriteLine("Pincode " + Pincode.ToString());
        //                    //        sw.WriteLine("FacilityDistrictCode " + FacilityDistrictCode.ToString());
        //                    //        sw.WriteLine("FacilityBlockCode " + FacilityBlockCode.ToString());
        //                    //        sw.WriteLine("FacilityHabitationCode " + FacilityHabitationCode.ToString());

        //                    //        sw.WriteLine("____________________________________________________");
        //                    //        sw.Close();
        //                    //    }
        //                    //}

        //                    using (TransactionScope ts = new TransactionScope())
        //                    {

        //                        objMastFacility.MASTER_FACILITY_CATEGORY_ID = FacilityCategoryCode;
        //                        objMastFacility.MASTER_FACILITY_SUB_CATEGORY_ID = FacilityTypeCode;
        //                        objMastFacility.MASTER_FACILITY_DESC = FacilityName;
        //                        objMastFacility.ADDRESS = FacilityAddress;
        //                        objMastFacility.LATITUDE = FacilityLatitude;
        //                        objMastFacility.LONGITUDE = FacilityLongitude;
        //                        objMastFacility.USERID = UserID;
        //                        objMastFacility.FILE_UPLOAD_DATE = DateTime.Now;
        //                        objMastFacility.FILE_NAME = fileName;
        //                        objMastFacility.PINCODE = Pincode;
        //                        dbContext.MASTER_FACILITY.Add(objMastFacility);
        //                        dbContext.SaveChanges();


        //                        objMapping.ID = (dbContext.FACILITY_HABITATION_MAPPING.Any() ? dbContext.FACILITY_HABITATION_MAPPING.Max(X => X.ID) : 0) + 1;
        //                        objMapping.MASTER_DISTRICT_CODE = FacilityDistrictCode;
        //                        objMapping.MASTER_BLOCK_CODE = FacilityBlockCode;
        //                        objMapping.MASTER_HAB_CODE = FacilityHabitationCode;
        //                        objMapping.MASTER_FACILITY_ID = objMastFacility.MASTER_FACILITY_ID;
        //                        objMapping.USERID = objMastFacility.USERID;
        //                        dbContext.FACILITY_HABITATION_MAPPING.Add(objMapping);
        //                        dbContext.SaveChanges();

        //                        //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                        //{
        //                        //    sw.WriteLine("____________________________________________________");
        //                        //    sw.WriteLine("step11  - after saving new entry ");
        //                        //    sw.WriteLine("____________________________________________________");
        //                        //    sw.Close();
        //                        //}

        //                        ts.Complete();
        //                        return cryptObj.Encrypt("1");
        //                    }

        //                }
        //                else //Update
        //                {

        //                    //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                    //{
        //                    //    sw.WriteLine("____________________________________________________");
        //                    //    sw.WriteLine("step12  - update ntry ");
        //                    //    sw.WriteLine("____________________________________________________");
        //                    //    sw.Close();
        //                    //}

        //                    MASTER_FACILITY master = new MASTER_FACILITY();
        //                    master = dbContext.MASTER_FACILITY.Where(x => x.MASTER_FACILITY_ID == mastFacilityId).FirstOrDefault();
        //                    if (master == null)
        //                    {

        //                        return cryptObj.Encrypt("-1");
        //                    }
        //                    else
        //                    {
        //                        //using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                        //{
        //                        //    sw.WriteLine("____________________________________________________");
        //                        //    sw.WriteLine("step13  - b4 update ntry ");
        //                        //    sw.WriteLine("____________________________________________________");
        //                        //    sw.Close();
        //                        //}

        //                        master.FILE_UPLOAD_DATE = DateTime.Now;
        //                        master.FILE_NAME = fileName;
        //                        master.LATITUDE = FacilityLatitude;
        //                        master.LONGITUDE = FacilityLongitude;
        //                        dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
        //                        dbContext.SaveChanges();

        //                        return cryptObj.Encrypt("1");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //               // ts.Complete();
        //                return cryptObj.Encrypt("-1");
        //            }
        //            #endregion
        //       //  } // end of using block
        //    }
        //    catch (System.Data.Entity.Validation.DbEntityValidationException e)
        //    {
        //        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //        {
        //            sw.WriteLine("____________________________________________________");
        //            sw.WriteLine("user " + UserID.ToString());
        //            sw.WriteLine("DbEntityValidationException message" + e.Message);
        //            sw.WriteLine("DbEntityValidationException " + e.ToString());
        //            sw.WriteLine("____________________________________________________");
        //            sw.Close();
        //        }
        //        string rs = "";
        //        foreach (var eve in e.EntityValidationErrors)
        //        {
        //            rs = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
        //            Console.WriteLine(rs);

        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                rs += "<br />" + string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
        //                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //                {
        //                    sw.WriteLine("____________________________________________________");
        //                    sw.WriteLine("user " + UserID.ToString());
        //                    sw.WriteLine("DbEntityValidationException valex " + rs.ToString());
        //                    sw.WriteLine("____________________________________________________");
        //                    sw.Close();
        //                }
        //            }
        //        }
        //        //throw new Exception(rs);
        //        return cryptObj.Encrypt("-1");
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //        {
        //            sw.WriteLine("____________________________________________________");
        //            sw.WriteLine("user " + UserID.ToString());
        //            sw.WriteLine("DbUpdateException valex " + ex.ToString());
        //            sw.WriteLine("____________________________________________________");
        //            sw.Close();
        //        }
        //        return cryptObj.Encrypt("-1");
        //    }
        //    catch (OptimisticConcurrencyException ex)
        //    {
        //        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //        {
        //            sw.WriteLine("____________________________________________________");
        //            sw.WriteLine("user " + UserID.ToString());
        //            sw.WriteLine("OptimisticConcurrencyException valex " + ex.ToString());
        //            sw.WriteLine("____________________________________________________");
        //            sw.Close();
        //        }
        //        return cryptObj.Encrypt("-1");
        //    }
        //    catch (Exception ex)
        //    {
        //        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //        {
        //            sw.WriteLine("Date :" + DateTime.Now.ToString());
        //            sw.WriteLine("Method : " + "InsertFacilityDetailsForCaptureLocation()");
        //            sw.WriteLine("Exception : " + ex.Message);
        //            sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
        //            if (ex.InnerException != null)
        //            {
        //                sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
        //                sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
        //            }
        //            sw.WriteLine("____________________________________________________");
        //            sw.Close();
        //        }
        //        return cryptObj.Encrypt("-1");
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }

        //}

        public string InsertFacilityDetailsForCaptureLocation(string mastfacilityID, string factilityCategoryCode, string factilityTypeCode, string factilityDistrictCode, string factilityBlockCode, string factilityLat, string factilityLong, string factilityName, string factilityAddress, string factilityHabitationCode, string userID, string factilitybase64String, string factilityPinCode)
        {

            MASTER_FACILITY objMastFacility = null;
            FACILITY_HABITATION_MAPPING objMapping = null;

            Int32 FacilityCategoryCode = 0;
            Int32 FacilityTypeCode = 0;
            Int32 FacilityDistrictCode = 0;
            Int32 FacilityBlockCode = 0;
            decimal FacilityLatitude = 0;
            decimal FacilityLongitude = 0;
            string FacilityName = string.Empty;
            string FacilityAddress = string.Empty;
            Int32 FacilityHabitationCode = 0;
            Int32 UserID = 0;
            Int32 filenameId = 0;
            Int32 Pincode = 0;


            UserID = Convert.ToInt32(cryptObj.Decrypt(userID));
            //UserID = Convert.ToInt32(userID);

            try
            {
                dbContext = new PMGSYEntities();
                objMastFacility = new MASTER_FACILITY();
                objMapping = new FACILITY_HABITATION_MAPPING();

                if (String.IsNullOrEmpty(factilityCategoryCode) || String.IsNullOrEmpty(factilityTypeCode) || String.IsNullOrEmpty(factilityDistrictCode) || String.IsNullOrEmpty(factilityBlockCode) || String.IsNullOrEmpty(factilityLat) || String.IsNullOrEmpty(factilityLong) || String.IsNullOrEmpty(factilityName) || String.IsNullOrEmpty(factilityAddress) || String.IsNullOrEmpty(userID) || String.IsNullOrEmpty(factilityHabitationCode) || String.IsNullOrEmpty(factilityPinCode))
                {
                    return cryptObj.Encrypt("-1");
                }

                Int32 mastFacilityId = Convert.ToInt32(cryptObj.Decrypt(mastfacilityID));
                FacilityHabitationCode = Convert.ToInt32(cryptObj.Decrypt(factilityHabitationCode));
                //Int32 mastFacilityId = Convert.ToInt32(mastfacilityID);

                if (mastFacilityId == 0)  //new Add
                {

                    FacilityCategoryCode = Convert.ToInt32(cryptObj.Decrypt(factilityCategoryCode));
                    FacilityTypeCode = Convert.ToInt32(cryptObj.Decrypt(factilityTypeCode));
                    FacilityDistrictCode = Convert.ToInt32(cryptObj.Decrypt(factilityDistrictCode));
                    FacilityBlockCode = Convert.ToInt32(cryptObj.Decrypt(factilityBlockCode));
                    FacilityLatitude = Convert.ToDecimal(cryptObj.Decrypt(factilityLat));
                    FacilityLongitude = Convert.ToDecimal(cryptObj.Decrypt(factilityLong));
                    FacilityName = Convert.ToString(cryptObj.Decrypt(factilityName));
                    FacilityAddress = Convert.ToString(cryptObj.Decrypt(factilityAddress));
                    FacilityHabitationCode = Convert.ToInt32(cryptObj.Decrypt(factilityHabitationCode));
                    //UserID = Convert.ToInt32(cryptObj.Decrypt(userID));
                    Pincode = Convert.ToInt32(cryptObj.Decrypt(factilityPinCode));


                    #region Validation

                    if (FacilityName.Length > 200)
                    {
                        return cryptObj.Encrypt("-3");  //Only 200 characters allowed for Facility Name  FacilityNameLengthExceed(R.string.E616),
                    }


                    if (FacilityAddress.Length > 255)
                    {
                        return cryptObj.Encrypt("-4");  //Only 255 characters allowed for Address  FacilityAddressLengthExceed(R.string.E617),

                    }

                    //var values = new[] { "~", "`", "!", "@", "#", "$", "%", "^", "*", "(", ")", "-", "_", "+", "=", "{", "}", "[", "]", "|", ";", ":", "'", "<", ">", ",", "/", "?" };
                    //if (values.Any(FacilityName.Contains))
                    //{

                    //    return cryptObj.Encrypt("-5");  //Special symbols are not allowed in Facility Name    FacilityNameNoSpecialSymbol(R.string.E618),
                    //}

                    //var values1 = new[] { "~", "`", "!", "@", "#", "$", "%", "^", "*", "(", ")", "_", "+", "=", "{", "}", "[", "]", "|", ";", "'", "<", ">", "?", "." };
                    //if (values1.Any(FacilityAddress.Contains))
                    //{
                    //    return cryptObj.Encrypt("-6");  //Special symbols are not allowed in Address
                    //}

                    // Following modifications are done on 12 May 2020 as per suggestion by Anita Mam
                    var values = new[] { "<", ">" };
                    if (values.Any(FacilityName.Contains))
                    {

                        return cryptObj.Encrypt("-5");  //Special symbols are not allowed in Facility Name    FacilityNameNoSpecialSymbol(R.string.E618),
                    }

                    var values1 = new[] { "<", ">" };
                    if (values1.Any(FacilityAddress.Contains))
                    {
                        return cryptObj.Encrypt("-6");  //Special symbols are not allowed in Address
                    }


                    if (dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE.Where(m => m.MAST_DISTRICT_CODE == FacilityDistrictCode && m.IS_FINALIZED == "Y").Any())
                    {
                        return cryptObj.Encrypt("-8"); // Facility Details can not be added as District is Finalized.
                    }

                    if (dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(m => m.MAST_BLOCK_CODE == FacilityBlockCode && m.IS_FINALIZED == "Y").Any())
                    {
                        return cryptObj.Encrypt("-7"); // Facility Details can not be added as Block is Finalized.
                    }

                    if (dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == FacilityHabitationCode).Any())
                    {
                    }
                    else
                    {
                        return cryptObj.Encrypt("-9");// Hab Code is not present in Habitation Master Table.
                    }



                

                    //if (dbContext.MASTER_FACILITY_CATEGORY.Where(m => m.MASTER_FACILITY_CATEGORT_ID == FacilityCategoryCode).Any())
                    //{
                    //    if (dbContext.MASTER_FACILITY_CATEGORY.Where(m => m.MASTER_FACILITY_CATEGORT_ID == FacilityTypeCode).Any())
                    //    {
                    //        Int32? ParentofSubcategory = dbContext.MASTER_FACILITY_CATEGORY.Where(m => m.MASTER_FACILITY_CATEGORT_ID == FacilityTypeCode).Select(m => m.MASTER_FACILITY_PARENT_ID).FirstOrDefault();
                    //        if (FacilityCategoryCode != ParentofSubcategory)
                    //        {
                    //            return cryptObj.Encrypt("-10");// Subcategory details are not available under Main Category.
                    //        }
                    //    }
                    //    else 
                    //    {
                    //        return cryptObj.Encrypt("-11");// Main Category and its Subcategory details are Invalid.
                    //    }
                    //}
                    //else
                    //{
                    //    return cryptObj.Encrypt("-11");// Main Category and its Subcategory details are Invalid.
                    //}




                    #endregion
                }
                else //Update
                {
                    FacilityLatitude = Convert.ToDecimal(cryptObj.Decrypt(factilityLat));
                    FacilityLongitude = Convert.ToDecimal(cryptObj.Decrypt(factilityLong));
                    UserID = Convert.ToInt32(cryptObj.Decrypt(userID));

                }


                #region Master Entry
                objMastFacility.MASTER_FACILITY_ID = (dbContext.MASTER_FACILITY.Any() ? dbContext.MASTER_FACILITY.Max(X => X.MASTER_FACILITY_ID) : 0) + 1;

                var timestampArray = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff").Replace('.', '_').Split(' ');
                string timestamp = timestampArray[0].ToString() + "_" + timestampArray[1].ToString();
                string fileName = "IMG_" + timestamp.Replace('/', '_').Replace(':', '_') + "_" + FacilityHabitationCode.ToString() + ".jpeg";
                string storageRoot = ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"];
                //if (!Directory.Exists(storageRoot))
                //{
                //    Directory.CreateDirectory(storageRoot);
                //}

                //Save Image on DISK
                var fullPath = Path.Combine(storageRoot, fileName);
                var thumbnailPath = Path.Combine(storageRoot, "thumbnails");
                var fullThumbnailPath = Path.Combine(thumbnailPath, fileName);

                byte[] encodedDataAsBytes = System.Convert.FromBase64String(factilitybase64String);

                //int saveImgCnt = new CommonFunctions().SaveImage(encodedDataAsBytes, storageRoot, fullPath, fullThumbnailPath);

                if (mastFacilityId == 0)  //Adding new Entry...pass "0" Hardcoded)
                {
                    int? result = dbContext.USP_facility_insert_facility_details(FacilityCategoryCode, FacilityTypeCode, FacilityName,
                            FacilityAddress, Pincode, UserID, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], FacilityDistrictCode, FacilityBlockCode,
                            FacilityHabitationCode, fileName, FacilityLatitude, FacilityLongitude).FirstOrDefault();

                    if (result == 1)
                    {
                        int saveImgCnt = new CommonFunctions().SaveImage(encodedDataAsBytes, storageRoot, fullPath, fullThumbnailPath);
                        return cryptObj.Encrypt("1"); ;
                    }
                    if (result == -1)
                    {
                        return cryptObj.Encrypt("-2"); ;
                    }
                    if (result == 0)
                    {
                        return cryptObj.Encrypt("-1"); ;
                    }

                }
                else //Update
                {
                    MASTER_FACILITY master = new MASTER_FACILITY();
                    master = dbContext.MASTER_FACILITY.Where(x => x.MASTER_FACILITY_ID == mastFacilityId).FirstOrDefault();
                    if (master == null)
                    {
                        return cryptObj.Encrypt("-1");
                    }
                    else
                    {

                        master.FILE_UPLOAD_DATE = DateTime.Now;
                        master.FILE_NAME = fileName;
                        master.LATITUDE = FacilityLatitude;
                        master.LONGITUDE = FacilityLongitude;
                        dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                        int saveImgCnt = new CommonFunctions().SaveImage(encodedDataAsBytes, storageRoot, fullPath, fullThumbnailPath);//save file in case of update operation

                        dbContext.SaveChanges();

                        return cryptObj.Encrypt("1");
                    }
                }

                //else
                //{
                //    // ts.Complete();
                //    return cryptObj.Encrypt("-1");
                //}
                return cryptObj.Encrypt("1");
                #endregion
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("____________________________________________________");
                    sw.WriteLine("user " + UserID.ToString());
                    sw.WriteLine("DbEntityValidationException message" + e.Message);
                    sw.WriteLine("DbEntityValidationException " + e.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                string rs = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    rs = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine(rs);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        rs += "<br />" + string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("____________________________________________________");
                            sw.WriteLine("user " + UserID.ToString());
                            sw.WriteLine("DbEntityValidationException valex " + rs.ToString());
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }
                //throw new Exception(rs);
                return cryptObj.Encrypt("-1");
            }
            catch (DbUpdateException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("____________________________________________________");
                    sw.WriteLine("user " + UserID.ToString());
                    sw.WriteLine("DbUpdateException valex " + ex.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            catch (OptimisticConcurrencyException ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("____________________________________________________");
                    sw.WriteLine("user " + UserID.ToString());
                    sw.WriteLine("OptimisticConcurrencyException valex " + ex.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertFacilityDetailsForCaptureLocation()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        public static byte[] DecodeUrlBase64(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/').PadRight(4 * ((s.Length + 3) / 4), '=');
            return Convert.FromBase64String(s);
        }

        public string GetBlockName(string BlockCode)
        {

            try
            {
                dbContext = new PMGSYEntities();
                List<BlockDetailModel> objBlockDetailModel = new List<BlockDetailModel>();

                Int32 blockCode = Convert.ToInt32(cryptObj.Decrypt(BlockCode));

                List<MASTER_BLOCK> execItenLst = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode && x.MAST_BLOCK_ACTIVE == "Y").ToList();


                if (execItenLst.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                foreach (MASTER_BLOCK item in execItenLst)
                {
                    BlockDetailModel model = new BlockDetailModel();
                    model.MAST_BLOCK_CODE = item.MAST_BLOCK_CODE.ToString();
                    model.MAST_BLOCK_NAME = item.MAST_BLOCK_NAME;
                    objBlockDetailModel.Add(model);
                }
                return cryptObj.Encrypt(GetXMLFromObject(objBlockDetailModel));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetStateList()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }



        }

        public string GetStateName(string StateCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<StateDetailModel> objStateDetailModel = new List<StateDetailModel>();
                Int32 MastStateCode = Convert.ToInt32(cryptObj.Decrypt(StateCode));


                List<MASTER_STATE> execItenLst = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == MastStateCode && x.MAST_STATE_ACTIVE == "Y").ToList();

                if (execItenLst.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }


                foreach (MASTER_STATE item in execItenLst)
                {
                    StateDetailModel model = new StateDetailModel();
                    model.MAST_STATE_CODE = item.MAST_STATE_CODE.ToString();
                    model.MAST_STATE_NAME = item.MAST_STATE_NAME;
                    objStateDetailModel.Add(model);
                }
                return cryptObj.Encrypt(GetXMLFromObject(objStateDetailModel));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetStateList()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetDistrictName(string DistrictCode)
        {

            try
            {
                dbContext = new PMGSYEntities();

                List<DistrictDetailModel> objDistrictDetailModel = new List<DistrictDetailModel>();
                Int32 MastDistrictCode = Convert.ToInt32(cryptObj.Decrypt(DistrictCode));

                List<MASTER_DISTRICT> execItenLst = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == MastDistrictCode && x.MAST_DISTRICT_ACTIVE == "Y").ToList();


                if (execItenLst.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                foreach (MASTER_DISTRICT item in execItenLst)
                {
                    DistrictDetailModel model = new DistrictDetailModel();
                    model.MAST_DISTRICT_CODE = item.MAST_DISTRICT_CODE.ToString();
                    model.MAST_DISTRICT_NAME = item.MAST_DISTRICT_NAME;
                    objDistrictDetailModel.Add(model);
                }
                return cryptObj.Encrypt(GetXMLFromObject(objDistrictDetailModel));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetDistrictFromUserIDForFacility()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //New Added Function
        public string GetStateNameBasedOnUserID(string userID)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<StateDetailModel> objStateDetailModel = new List<StateDetailModel>();



                if (String.IsNullOrEmpty(userID))
                {
                    return cryptObj.Encrypt("0");

                }
                Int32 UserID = Convert.ToInt32(cryptObj.Decrypt(userID));

                if (UserID == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                Int32? MastStateCode = dbContext.UM_User_Master.Where(x => x.UserID == UserID).Select(x => x.Mast_State_Code).FirstOrDefault();
                Int32 MastterStateCode = Convert.ToInt32(MastStateCode);

                if (MastterStateCode == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                List<MASTER_STATE> execItenLst = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == MastterStateCode && x.MAST_STATE_ACTIVE == "Y").ToList();

                if (execItenLst.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                foreach (MASTER_STATE item in execItenLst)
                {
                    StateDetailModel model = new StateDetailModel();
                    model.MAST_STATE_CODE = item.MAST_STATE_CODE.ToString();
                    model.MAST_STATE_NAME = item.MAST_STATE_NAME;
                    objStateDetailModel.Add(model);
                }
                return cryptObj.Encrypt(GetXMLFromObject(objStateDetailModel));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetStateNameBasedOnUserID()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception StackTrace: " + ex.StackTrace.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return cryptObj.Encrypt("-1");
            }
            finally
            {
                dbContext.Dispose();
            }


        }

        #endregion
    }
}