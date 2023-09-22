#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MABQMSDAL.asmx.cs        
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
using PMGSY.Models.MABQMS;
using System.Transactions;
using System.Data.Entity;
using System.Configuration;
using PMGSY.Common;
using PMGSY.MABQMS;
using System.Text;
using System.Data.Entity.Core;

namespace PMGSY.DAL.MABQMS
{
    public class MABQMSDAL
    {
        PMGSYEntities dbContext = null;
        public static string key = "^%V{T%&]@08_01-";
        RijndaelCrypt cryptObj = new RijndaelCrypt(key);

        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["MABQMSErrorLogPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" +   "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//MABQMSErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";	

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


        /// <summary>
        /// Method returns all Master Configuration Parameters
        /// Called from MABQMSConfigParams.asmx web service page
        /// </summary>
        /// <param name="imeiNo"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetConfigParameters(string userName,string imei)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (!Directory.Exists(ErrorLogBasePath))
                {
                    Directory.CreateDirectory(ErrorLogBasePath);
                }
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }

                List<ADMIN_MODULE_CONFIGURATION> itemList = (from amc in dbContext.ADMIN_MODULE_CONFIGURATION
                                                             select amc).ToList<ADMIN_MODULE_CONFIGURATION>();
                
                
                if(itemList.Count == 0)
                        return "0";

                // Check for userName - if null then return only configuration data 
                // else return Batch, Message etc.
                if (userName != null && !userName.Trim().Equals("") )
                {
                    int userId = Convert.ToInt32((from uum in dbContext.UM_User_Master
                                                  where uum.UserName == userName.Trim()
                                                  select uum.UserID).First());

                    var userDetails = (from qmin in dbContext.QUALITY_MOB_IMEI_NO
                                       where qmin.UserId == userId 
                                       select qmin).FirstOrDefault();

                    //Byte maxConfigId = dbContext.ADMIN_MODULE_CONFIGURATION.Select(m => m.Id).Max();

                    itemList.Add(new ADMIN_MODULE_CONFIGURATION()
                    {
                        Id = Convert.ToByte(14),
                        Parameter = "MABQMS_APP_MODE",
                        Value = userDetails.ApplicationMode
                    });

                    itemList.Add(new ADMIN_MODULE_CONFIGURATION()
                    {
                        Id = Convert.ToByte(15),
                        Parameter = "MABQMS_TRAINING_BATCH",
                        Value = userDetails.TrainingBatch.ToString()
                    });

                    itemList.Add(new ADMIN_MODULE_CONFIGURATION()
                    {
                        Id = Convert.ToByte(16),
                        Parameter = "MABQMS_NOTIFICATION_MESSAGE",
                        Value = userDetails.NotificationMessage == String.Empty ? "NoMessage" : userDetails.NotificationMessage
                    });
                }
                
                   
                return GetXMLFromObject(itemList);
                
                
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetConfigParameters()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return "-1";
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
           
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetConfigParameters()");
                    sw.WriteLine("Exception : " + ex.Message);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return "-1";
            }
            finally
            {
                dbContext.Dispose();
            }   
        }


        /// <summary>
        /// Method returns image descriptions
        /// </summary>
        /// <param name="imeiNo"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetImageDescription()
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<QUALITY_QM_IMAGE_DESCRIPTION> itemList = (from qqid in dbContext.QUALITY_QM_IMAGE_DESCRIPTION
                                                               select qqid).ToList<QUALITY_QM_IMAGE_DESCRIPTION>();

                if (itemList.Count == 0)
                    return "0";

                return GetXMLFromObject(itemList);
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetImageDescription()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return "-1";
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetImageDescription()");
                    sw.WriteLine("Exception : " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Database Exception : " + ex.InnerException.StackTrace.ToString());
                    }
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return "-1";
            }
            finally
            {
                dbContext.Dispose();
            }
        }


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
        public string LoginDetails(string encUserName, string encImei)
        {
            MABQMSModel objMABQMSModel = new MABQMSModel();
            dbContext = new PMGSYEntities();
            string status = "-1";
            string userName = null;
            string imei = null;
            try
            {
                userName = (encUserName == null || encUserName.Trim().Equals("")) ? null : cryptObj.Decrypt(encUserName);
                imei = cryptObj.Decrypt(encImei);
                
                // Check for Valid User, if valid then proceed, else return
                var userDetails = dbContext.USP_MABQMS_VERIFY_LOGIN(userName).FirstOrDefault();

                if (userDetails == null)
                {
                    return cryptObj.Encrypt("0");
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("MABQMSDAL.LoginDetails : step1");
                    sw.WriteLine("UserID : " + userDetails.UserID.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                #region IMEI Validation
                //Only one device is allow for a user to login
                String IMEI = string.Empty;
                if (dbContext.QUALITY_MOB_IMEI_NO.Where(c => c.UserId == userDetails.UserID).Any())
                {
                    var imeiDetails = (from qmin in dbContext.QUALITY_MOB_IMEI_NO
                                       where qmin.UserId == userDetails.UserID
                                       select qmin).First();

                    if (imeiDetails.ImeiNo == null || imeiDetails.ImeiNo == "")
                    {
                        QUALITY_MOB_IMEI_NO objQUALITY_MOB_IMEI_NO = new QUALITY_MOB_IMEI_NO();
                        objQUALITY_MOB_IMEI_NO = dbContext.QUALITY_MOB_IMEI_NO.Where(c => c.UserId == userDetails.UserID).FirstOrDefault();
                        objQUALITY_MOB_IMEI_NO.ImeiNo = imei;
                        dbContext.Entry(objQUALITY_MOB_IMEI_NO).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("MABQMSDAL.LoginDetails : step2");
                            sw.WriteLine("update imei : " + imei);
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                    else
                    {
                        string StrIMEI = imeiDetails.ImeiNo.Trim().ToUpper();
                        string StrIMEI1 = imei.Trim().ToUpper();

                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("MABQMSDAL.LoginDetails : step3");
                            sw.WriteLine("StrIMEI : " + StrIMEI);
                            sw.WriteLine("StrIMEI1 : " + StrIMEI1);
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }

                        if (StrIMEI.Equals(StrIMEI1))
                        {
                            IMEI = imeiDetails.ImeiNo;
                        }
                        else
                        {
                            using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                            {
                                sw.WriteLine("MABQMSDAL.LoginDetails : step4");
                                sw.WriteLine("return -5 ");
                                sw.WriteLine("____________________________________________________");
                                sw.Close();
                            }
                            return cryptObj.Encrypt("-5");
                        }
                    }
                }
                #endregion

                // Check existance of IMEI No.
                // For old user imei compared to existing IMEI_NO 
                if (dbContext.QUALITY_MOB_IMEI_NO.Where(c => c.UserId == userDetails.UserID).Any())
                {
                    var imeiDetails = (from qmin in dbContext.QUALITY_MOB_IMEI_NO
                                      where qmin.UserId == userDetails.UserID
                                      select qmin).First();

                    if (imeiDetails.ImeiNo.Equals(imei) && imeiDetails.UserId == userDetails.UserID)        //User with valid IMEI exists
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("MABQMSDAL.LoginDetails : step5");
                            sw.WriteLine("Compare old imei with existing imei ");
                            sw.WriteLine("return 1 ");
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                        status = "1";
                    }
                    else
                    {
                        if (dbContext.QUALITY_MOB_IMEI_NO.Where(c => c.ImeiNo == imei).Any())
                        {
                            using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                            {
                                sw.WriteLine("MABQMSDAL.LoginDetails : step6");
                                sw.WriteLine("Compare old imei with existing imei ");
                                sw.WriteLine("return -2 ");
                                sw.WriteLine("____________________________________________________");
                                sw.Close();
                            }
                            return cryptObj.Encrypt("-2"); 
                        }
                        else
                        {
                            if (imeiDetails.ImeiNo.Length == 0)         //Reset IMEI case
                            {
                                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                                {
                                    sw.WriteLine("MABQMSDAL.LoginDetails : step7");
                                    sw.WriteLine("reset imei ");
                                    sw.WriteLine("____________________________________________________");
                                    sw.Close();
                                }
                                //For reset IMEI option, update IMEI
                                QUALITY_MOB_IMEI_NO quality_mob_imei_no = dbContext.QUALITY_MOB_IMEI_NO.Find(userDetails.UserID);
                                quality_mob_imei_no.ImeiNo = imei;
                                dbContext.Entry(quality_mob_imei_no).State = System.Data.Entity.EntityState.Modified;
                                int updateCnt = dbContext.SaveChanges();

                                if (updateCnt > 0)
                                {
                                    status = "1";                           //User with valid IMEI exists
                                }
                                else
                                {
                                    using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                                    {
                                        sw.WriteLine("MABQMSDAL.LoginDetails : step8");
                                        sw.WriteLine("user with invalid imei");
                                        sw.WriteLine("return -2");
                                        sw.WriteLine("____________________________________________________");
                                        sw.Close();
                                    }
                                    return cryptObj.Encrypt("-2");          //User with invalid IMEI exists
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (dbContext.QUALITY_MOB_IMEI_NO.Where(c => c.ImeiNo == imei).Any())   //Imei exists
                    {
                        var imeiDetails = (from qmin in dbContext.QUALITY_MOB_IMEI_NO
                                           where qmin.ImeiNo == imei
                                           select qmin).First();

                        if (imeiDetails.UserId == userDetails.UserID)   //User with valid IMEI exists
                        {
                            using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                            {
                                sw.WriteLine("MABQMSDAL.LoginDetails : step9");
                                sw.WriteLine("user with imei");
                                sw.WriteLine("return 1");
                                sw.WriteLine("____________________________________________________");
                                sw.Close();
                            }
                            status = "1";
                        }
                        else
                        {
                            using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                            {
                                sw.WriteLine("MABQMSDAL.LoginDetails : step10");
                                sw.WriteLine("user with invalid imei");
                                sw.WriteLine("return -2");
                                sw.WriteLine("____________________________________________________");
                                sw.Close();
                            }
                            return cryptObj.Encrypt("-2");              //User with invalid IMEI exists
                        }
                    }
                    else
                    {
                        //For New User, Insert IMEI, if inserted successfully then set IsValidUser = true;
                        QUALITY_MOB_IMEI_NO quality_mob_imei_no = new QUALITY_MOB_IMEI_NO();
                        quality_mob_imei_no.UserId = userDetails.UserID;
                        quality_mob_imei_no.ImeiNo = imei;
                        quality_mob_imei_no.ApplicationMode = "D";
                        quality_mob_imei_no.TrainingBatch = 0;
                        quality_mob_imei_no.NotificationMessage = "";

                        dbContext.QUALITY_MOB_IMEI_NO.Add(quality_mob_imei_no);
                        int inserCnt = dbContext.SaveChanges();

                        if (inserCnt > 0)
                        {
                            using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                            {
                                sw.WriteLine("MABQMSDAL.LoginDetails : step11");
                                sw.WriteLine("insert");
                                sw.WriteLine("return 1");
                                sw.WriteLine("____________________________________________________");
                                sw.Close();
                            }
                            status = "1";
                        }
                        else
                        {
                            using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                            {
                                sw.WriteLine("MABQMSDAL.LoginDetails : step12");
                                sw.WriteLine("insert");
                                sw.WriteLine("return -2");
                                sw.WriteLine("____________________________________________________");
                                sw.Close();
                            }
                            return cryptObj.Encrypt("-2");
                        }
                    }
                }

                objMABQMSModel.UserId = userDetails.UserID.ToString();
                objMABQMSModel.UserName = userDetails.UserName;
                objMABQMSModel.RoleId = userDetails.DefaultRoleID.ToString();
                objMABQMSModel.Password = userDetails.Password.ToString();
                objMABQMSModel.AdminQmCode = userDetails.AdminQmCode.ToString();
                objMABQMSModel.QmType = userDetails.QmType.ToString();
                objMABQMSModel.Status = status;
                objMABQMSModel.MonitorName = userDetails.MONITOR_NAME;

                //encrypt return value with user's "encryptionKey"
                return cryptObj.Encrypt(Convert.ToString(GetXMLFromObject(objMABQMSModel))); 

             }
            catch (DbEntityValidationException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "MABQMSDAL.LoginDetails.DbEntityValidationException");

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
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "MABQMSDAL.LoginDetails.DbUpdateException");
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "LoginDetails()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "MABQMSDAL.LoginDetails");
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "LoginDetails()");
                    sw.WriteLine("Exception : " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine("Database Exception : " + ex.InnerException.StackTrace.ToString());
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
        public string InsertLog(string monitorCode, string mobileNo, string imeiNo, string osVersion, string modelName, string nwProvider, string appVersion, string loginMode, string ipAddress)
        {
            dbContext = new PMGSYEntities();
            try
            {
                int decMonitorCode = Convert.ToInt32(cryptObj.Decrypt(monitorCode));
                string decMobileNo = cryptObj.Decrypt(mobileNo);
                string decImei = cryptObj.Decrypt(imeiNo);
                string decOsVersion = cryptObj.Decrypt(osVersion);
                string decModelName = cryptObj.Decrypt(modelName);
                string decNwProvider = cryptObj.Decrypt(nwProvider);
                string decAppVersion = cryptObj.Decrypt(appVersion);
                string decLoginMode = cryptObj.Decrypt(loginMode);
                string decIpAddress = cryptObj.Decrypt(ipAddress);

                int insertCount = dbContext.USP_MABQMS_INSERT_LOG(decMonitorCode, decMobileNo, decImei, decOsVersion, decModelName, decNwProvider, decAppVersion, decLoginMode, decIpAddress);
                return cryptObj.Encrypt(Convert.ToString(insertCount));
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        /// DownLoad Current Month/Year Schedule Details
        /// If No Record Found return 0 else return result set
        /// </summary>
        /// <param name="monitorCode"></param>
        /// <returns></returns>
        public string DownloadSchedule(string monitorCode)
        {
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            dbContext = new PMGSYEntities();
            try
            {
                int decMonitorCode = Convert.ToInt32(cryptObj.Decrypt(monitorCode));

                List<USP_MABQMS_DOWNLOAD_SCHEDULE_Result> itemList = new List<USP_MABQMS_DOWNLOAD_SCHEDULE_Result>();
                itemList = dbContext.USP_MABQMS_DOWNLOAD_SCHEDULE(decMonitorCode, month, year).ToList<USP_MABQMS_DOWNLOAD_SCHEDULE_Result>();
                
                if (itemList.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }


                return cryptObj.Encrypt(GetXMLFromObject(itemList).Replace("&amp;", " and "));
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DownloadSchedule()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DownloadSchedule()");
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
        /// Insert Observations of inspected road
        /// gradeArr consists of , seperated string for Observation Grades
        /// </summary>
        /// <param name="scheduleCode"></param>
        /// <param name="prRoadCode"></param>
        /// <param name="inspDate"></param>
        /// <param name="startChainage"></param>
        /// <param name="endChainage"></param>
        /// <param name="roadStatus"></param>
        /// <param name="startLatitude"></param>
        /// <param name="startLongitude"></param>
        /// <param name="endLatitude"></param>
        /// <param name="endLongitude"></param>
        /// <param name="gradeArr"></param>
        /// <returns></returns>
        public string InsertObservationDetails(string scheduleCode, string prRoadCode, string inspDate, string startChainage, string endChainage,
                                               string roadStatus, string startLatitude, string startLongitude, string endLatitude, string endLongitude, string gradeString,
                                               string imgToBeUploadedCnt, string bearValueProp, string remark)
        {
            dbContext = new PMGSYEntities();
            int insertCount = 0;
            int obsId = 0;
            int overallGrade = 0;
            try
            {




                int decScheduleCode = Convert.ToInt32(cryptObj.Decrypt(scheduleCode));
                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
                string decInspDate = cryptObj.Decrypt(inspDate);
                decimal decStartChainage = Convert.ToDecimal(cryptObj.Decrypt(startChainage));
                decimal decEndChainage = Convert.ToDecimal(cryptObj.Decrypt(endChainage));
                string decRoadStatus = cryptObj.Decrypt(roadStatus);
                decimal decStartLatitude = Convert.ToDecimal(cryptObj.Decrypt(startLatitude) == "" ? "0.0" : cryptObj.Decrypt(startLatitude));
                decimal decStartLongitude = Convert.ToDecimal(cryptObj.Decrypt(startLongitude) == "" ? "0.0" : cryptObj.Decrypt(startLongitude));
                decimal decEndLatitude = Convert.ToDecimal(cryptObj.Decrypt(endLatitude) == "" ? "0.0" : cryptObj.Decrypt(endLatitude));
                decimal decEndLongitude = Convert.ToDecimal(cryptObj.Decrypt(endLongitude) == "" ? "0.0" : cryptObj.Decrypt(endLongitude));
                string decGradeString = cryptObj.Decrypt(gradeString);
                int decImgToBeUploadedCnt = Convert.ToInt32(cryptObj.Decrypt(imgToBeUploadedCnt));
                int? decbearValueProp = (int?)Convert.ToInt32(cryptObj.Decrypt(bearValueProp));
                string deremark = Convert.ToString((cryptObj.Decrypt(remark)));

                //  startLatitude = "NcOKWPNlKDCrMEhzaFmaZA==" ;
                //  startLongitude ="NcOKWPNlKDCrMEhzaFmaZA==";
                //  endLatitude = "NcOKWPNlKDCrMEhzaFmaZA==" ;
                //  endLongitude = "NcOKWPNlKDCrMEhzaFmaZA==" ;



                //Find Record from Schedule Details table
                var scheduleDetails = dbContext.QUALITY_QM_SCHEDULE_DETAILS.Where(c => c.ADMIN_SCHEDULE_CODE == decScheduleCode && c.IMS_PR_ROAD_CODE == decPrRoadCode).First();

                //Set Total Image Count
                scheduleDetails.TOTAL_IMAGE_COUNT = decImgToBeUploadedCnt;
                dbContext.Entry(scheduleDetails).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                // Is Inspection Completed or Ended?
                if (scheduleDetails.INSP_STATUS_FLAG.Equals("C"))
                {
                    return "C";
                }
                else if (scheduleDetails.INSP_STATUS_FLAG.Equals("E"))
                {
                    return "E";
                }

                string[] deremarkArr = deremark.Split(',');
                string[] obsGradeArr = decGradeString.Split(',');
                int[] obsGradeCodeArr = new int[(obsGradeArr.Length - 1)];
                int index = 0;
                foreach (var grade in obsGradeArr)
                {
                    if (!(grade.Trim().Equals("")))
                    {
                        if (grade.Equals("S"))
                        {
                            obsGradeCodeArr[index] = 1;

                            if (index == (obsGradeCodeArr.Length - 1))
                                overallGrade = 1;
                        }
                        else if (grade.Equals("SRI"))
                        {
                            obsGradeCodeArr[index] = 2;

                            if (index == (obsGradeCodeArr.Length - 1))
                                overallGrade = 2;
                        }
                        else if (grade.Equals("U"))
                        {
                            obsGradeCodeArr[index] = 3;

                            if (index == (obsGradeCodeArr.Length - 1))
                                overallGrade = 3;
                        }
                        else if (grade.Equals("NA"))
                        {
                            obsGradeCodeArr[index] = 4;

                            if (index == (obsGradeCodeArr.Length - 1))
                                overallGrade = 4;
                        }

                        index++;
                    }
                }

                using (var scope = new TransactionScope())
                {
                    //In case of Bridge, decRoadStatus can be LC or LP i.e LSB(Completed) or LSB(Progress), so as per length assign status
                    string roadStatusToInsert = decRoadStatus;
                    if (decRoadStatus.Length > 1)
                    {
                        roadStatusToInsert = Convert.ToString(decRoadStatus[1]);
                        decRoadStatus = "L";
                    }

                    insertCount = dbContext.USP_MABQMS_INSERT_OBSERVATIONS(decScheduleCode, decPrRoadCode, Convert.ToDateTime(decInspDate), decStartChainage,
                                                                           decEndChainage, overallGrade, roadStatusToInsert, decStartLatitude, decStartLongitude,
                                                                           decEndLatitude, decEndLongitude);

                    // insertCount = dbContext.USP_MABQMS_INSERT_OBSERVATIONS(decScheduleCode, decPrRoadCode,Convert.ToDateTime(inspDate) , decStartChainage,
                    //                                                        decEndChainage, overallGrade, roadStatusToInsert, decStartLatitude, decStartLongitude,
                    //                                                        decEndLatitude, decEndLongitude);


                    if (insertCount > 0)
                    {
                        obsId = dbContext.QUALITY_QM_OBSERVATION_MASTER.Select(c => c.QM_OBSERVATION_ID).Max();
                        string qmType = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                         join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                         where qqs.ADMIN_SCHEDULE_CODE == decScheduleCode
                                         select aqm.ADMIN_QM_TYPE).First().ToString();

                        // get itemList for appropriate monitor type & road Status
                        // and as per items, save grade provided in obsGradeCodeArr
                        List<qm_monitor_master_grade_items_Result> itemList = new List<qm_monitor_master_grade_items_Result>();


                        itemList = dbContext.qm_monitor_master_grade_items(decRoadStatus, qmType).ToList<qm_monitor_master_grade_items_Result>();

                        if (decRoadStatus.Equals("L"))              //For Bridge - (Completed), One Item is less.
                        {
                            if (roadStatusToInsert.Equals("C"))
                                itemList.RemoveAt(0);
                            else if (roadStatusToInsert.Equals("M"))
                                itemList.RemoveAt(0);
                            // else if (roadStatusToInsert.Equals("P"))
                            // itemList.RemoveAt(0);
                        }

                        for (int i = 0; i < itemList.Count; i++)
                        {
                            QUALITY_QM_OBSERVATION_DETAIL quality_qm_observation_detail = new QUALITY_QM_OBSERVATION_DETAIL();
                            quality_qm_observation_detail.QM_OBSERVATION_ID = obsId;
                            quality_qm_observation_detail.MAST_ITEM_NO = itemList[i].MAST_ITEM_NO;
                            // for bearing type add bearing code
                            if ((itemList[i].MAST_ITEM_NO == 122 || itemList[i].MAST_ITEM_NO == 151))
                            {
                                quality_qm_observation_detail.MAST_BEARING_CODE = decbearValueProp;

                            }
                            else
                            { quality_qm_observation_detail.MAST_BEARING_CODE = null; }

                            if ((deremarkArr[i + 1] != "NA"))
                            {
                                quality_qm_observation_detail.REMARKS = deremarkArr[i + 1];

                            }
                            else
                            { quality_qm_observation_detail.REMARKS = null; }

                            quality_qm_observation_detail.MAST_GRADE_CODE = Convert.ToInt32(obsGradeCodeArr[i]);
                            quality_qm_observation_detail.MAST_GRADE_CODE_UPGRADE = null;
                            dbContext.QUALITY_QM_OBSERVATION_DETAIL.Add(quality_qm_observation_detail);
                        }

                        dbContext.SaveChanges();
                    }

                    scope.Complete();
                    return cryptObj.Encrypt("1_" + obsId.ToString());
                }
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertObservationDetails()");
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("Cunccurrency Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertObservationDetails()");
                    sw.WriteLine("update : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
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
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "InsertObservationDetails()");
                    sw.WriteLine("Exception : " + ex.StackTrace);

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
        /// Insert Observations of inspected road
        /// gradeArr consists of , seperated string for Observation Grades
        /// </summary>
        /// <param name="scheduleCode"></param>
        /// <param name="prRoadCode"></param>
        /// <param name="inspDate"></param>
        /// <param name="startChainage"></param>
        /// <param name="endChainage"></param>
        /// <param name="roadStatus"></param>
        /// <param name="startLatitude"></param>
        /// <param name="startLongitude"></param>
        /// <param name="endLatitude"></param>
        /// <param name="endLongitude"></param>
        /// <param name="gradeArr"></param>
        /// <returns></returns>
        //public string InsertObservationDetails(string scheduleCode, string prRoadCode, string inspDate, string startChainage, string endChainage,
        //                                       string roadStatus, string startLatitude, string startLongitude, string endLatitude, string endLongitude, string gradeString,
        //                                       string imgToBeUploadedCnt)
        //{
        //    dbContext = new PMGSYEntities();
        //    int insertCount = 0;
        //    int obsId = 0;
        //    int overallGrade = 0;
        //    try
        //    {

        //        int decScheduleCode = Convert.ToInt32(cryptObj.Decrypt(scheduleCode));
        //        int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
        //        string decInspDate = cryptObj.Decrypt(inspDate);
        //        decimal decStartChainage = Convert.ToDecimal(cryptObj.Decrypt(startChainage));
        //        decimal decEndChainage = Convert.ToDecimal(cryptObj.Decrypt(endChainage));
        //        string decRoadStatus = cryptObj.Decrypt(roadStatus);
        //        decimal decStartLatitude = Convert.ToDecimal(cryptObj.Decrypt(startLatitude) == "" ? "0.0" : cryptObj.Decrypt(startLatitude));
        //        decimal decStartLongitude = Convert.ToDecimal(cryptObj.Decrypt(startLongitude) == "" ? "0.0" : cryptObj.Decrypt(startLongitude));
        //        decimal decEndLatitude = Convert.ToDecimal(cryptObj.Decrypt(endLatitude) == "" ? "0.0" : cryptObj.Decrypt(endLatitude));
        //        decimal decEndLongitude = Convert.ToDecimal(cryptObj.Decrypt(endLongitude) == "" ? "0.0" : cryptObj.Decrypt(endLongitude));
        //        string decGradeString = cryptObj.Decrypt(gradeString);
        //        int decImgToBeUploadedCnt = Convert.ToInt32(cryptObj.Decrypt(imgToBeUploadedCnt));

        //        //int decScheduleCode =Convert.ToInt32( scheduleCode);
        //        //int decPrRoadCode =Convert.ToInt32( prRoadCode);
        //        //string decInspDate = inspDate;
        //        //decimal decStartChainage = Convert.ToDecimal(startChainage);
        //        //decimal decEndChainage = Convert.ToDecimal(endChainage);
        //        //string decRoadStatus = (roadStatus);
        //        //decimal decStartLatitude = Convert.ToDecimal(startLatitude == "" ? "0.0" : startLatitude);
        //        //decimal decStartLongitude = Convert.ToDecimal(startLongitude == "" ? "0.0" : startLongitude);
        //        //decimal decEndLatitude = Convert.ToDecimal(endLatitude == "" ? "0.0" : endLatitude);
        //        //decimal decEndLongitude = Convert.ToDecimal(endLongitude == "" ? "0.0" :endLongitude);
        //        //string decGradeString = gradeString;
        //        //int decImgToBeUploadedCnt = Convert.ToInt32(imgToBeUploadedCnt);


        //        //Find Record from Schedule Details table
        //        var scheduleDetails = dbContext.QUALITY_QM_SCHEDULE_DETAILS.Where(c => c.ADMIN_SCHEDULE_CODE == decScheduleCode && c.IMS_PR_ROAD_CODE == decPrRoadCode).First();

        //        //Set Total Image Count
        //        scheduleDetails.TOTAL_IMAGE_COUNT = decImgToBeUploadedCnt;
        //        dbContext.Entry(scheduleDetails).State = System.Data.Entity.EntityState.Modified;
        //        dbContext.SaveChanges();

        //        // Is Inspection Completed or Ended?
        //        if (scheduleDetails.INSP_STATUS_FLAG.Equals("C"))
        //        {
        //            return "C";
        //        }
        //        else if (scheduleDetails.INSP_STATUS_FLAG.Equals("E"))
        //        {
        //            return "E";
        //        }


        //        string[] obsGradeArr = decGradeString.Split(',');
        //        int[] obsGradeCodeArr = new int[(obsGradeArr.Length - 1)];
        //        int index = 0;
        //        foreach (var grade in obsGradeArr)
        //        {
        //            if (!(grade.Trim().Equals("")))
        //            {
        //                if (grade.Equals("S"))
        //                {
        //                    obsGradeCodeArr[index] = 1;

        //                    if (index == (obsGradeCodeArr.Length - 1))
        //                        overallGrade = 1;
        //                }
        //                else if (grade.Equals("SRI"))
        //                {
        //                    obsGradeCodeArr[index] = 2;

        //                    if (index == (obsGradeCodeArr.Length - 1))
        //                        overallGrade = 2;
        //                }
        //                else if (grade.Equals("U"))
        //                {
        //                    obsGradeCodeArr[index] = 3;

        //                    if (index == (obsGradeCodeArr.Length - 1))
        //                        overallGrade = 3;
        //                }
        //                else if (grade.Equals("NA"))
        //                {
        //                    obsGradeCodeArr[index] = 4;

        //                    if (index == (obsGradeCodeArr.Length - 1))
        //                        overallGrade = 4;
        //                }

        //                index++;
        //            }
        //        }

        //        using (var scope = new TransactionScope())
        //        {
        //            //In case of Bridge, decRoadStatus can be LC or LP i.e LSB(Completed) or LSB(Progress), so as per length assign status
        //            string roadStatusToInsert = decRoadStatus;
        //            if (decRoadStatus.Length > 1)
        //            {
        //                roadStatusToInsert = Convert.ToString(decRoadStatus[1]);
        //                decRoadStatus = "L";
        //            }

        //            insertCount = dbContext.USP_MABQMS_INSERT_OBSERVATIONS(decScheduleCode, decPrRoadCode, Convert.ToDateTime(decInspDate), decStartChainage,
        //                                                                   decEndChainage, overallGrade, roadStatusToInsert, decStartLatitude, decStartLongitude,
        //                                                                   decEndLatitude, decEndLongitude);

        //           // insertCount = dbContext.USP_MABQMS_INSERT_OBSERVATIONS(decScheduleCode, decPrRoadCode,Convert.ToDateTime(inspDate) , decStartChainage,
        //           //                                                        decEndChainage, overallGrade, roadStatusToInsert, decStartLatitude, decStartLongitude,
        //           //                                                        decEndLatitude, decEndLongitude);


        //            if (insertCount > 0)
        //            {
        //                obsId = dbContext.QUALITY_QM_OBSERVATION_MASTER.Select(c => c.QM_OBSERVATION_ID).Max();
        //                string qmType = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
        //                                 join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
        //                                 where qqs.ADMIN_SCHEDULE_CODE == decScheduleCode
        //                                 select aqm.ADMIN_QM_TYPE).First().ToString();

        //                // get itemList for appropriate monitor type & road Status
        //                // and as per items, save grade provided in obsGradeCodeArr
        //                List<qm_monitor_master_grade_items_Result> itemList = new List<qm_monitor_master_grade_items_Result>();


        //                itemList = dbContext.qm_monitor_master_grade_items(decRoadStatus, qmType).ToList<qm_monitor_master_grade_items_Result>();

                       
        //                // change made so that 1st item can be set 'NA' for maintenance,completed work  on 24sep 2020  by sachin solanki
        //                //if (decRoadStatus.Equals("L"))              //For Bridge - (Completed), One Item is less.
        //                //{
        //                //    if (roadStatusToInsert.Equals("C"))
        //                //        itemList.RemoveAt(0);
        //                //    else if (roadStatusToInsert.Equals("M"))
        //                //        itemList.RemoveAt(0);
        //                //    else if (roadStatusToInsert.Equals("P"))
        //                //        itemList.RemoveAt(0);
        //                //}

        //                for (int i = 0; i < itemList.Count; i++)
        //                {
        //                    QUALITY_QM_OBSERVATION_DETAIL quality_qm_observation_detail = new QUALITY_QM_OBSERVATION_DETAIL();
        //                    quality_qm_observation_detail.QM_OBSERVATION_ID = obsId;
        //                    quality_qm_observation_detail.MAST_ITEM_NO = itemList[i].MAST_ITEM_NO;
        //                    quality_qm_observation_detail.MAST_GRADE_CODE = Convert.ToInt32(obsGradeCodeArr[i]);
        //                    quality_qm_observation_detail.MAST_GRADE_CODE_UPGRADE = null;
        //                    dbContext.QUALITY_QM_OBSERVATION_DETAIL.Add(quality_qm_observation_detail);
        //                }

        //                dbContext.SaveChanges();
        //            }

        //            scope.Complete();
        //            return cryptObj.Encrypt("1_" + obsId.ToString());
        //        }
        //    }
        //    catch (OptimisticConcurrencyException ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //        {
        //            sw.WriteLine("Date :" + DateTime.Now.ToString());
        //            sw.WriteLine("Method : " + "InsertObservationDetails()");
        //            sw.WriteLine("Cunccurrency Exception : " + ex.Message);
        //            sw.WriteLine("____________________________________________________");
        //            sw.Close();
        //        }

        //        return cryptObj.Encrypt("-1");
        //    }
        //    catch (UpdateException ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //        {
        //            sw.WriteLine("Date :" + DateTime.Now.ToString());
        //            sw.WriteLine("Method : " + "InsertObservationDetails()");
        //            sw.WriteLine("Database Exception : " + ex.Message);
        //            sw.WriteLine("____________________________________________________");
        //            sw.Close();
        //        }

        //        return cryptObj.Encrypt("-1");
        //    }
        //    catch (Exception ex)
        //    {
        //        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //        {
        //            Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //            sw.WriteLine("Date :" + DateTime.Now.ToString());
        //            sw.WriteLine("Method : " + "InsertObservationDetails()");
        //            sw.WriteLine("Exception : " + ex.Message);
        //            if (ex.InnerException != null)
        //                sw.WriteLine("Inner Exception : " + ex.InnerException.Message);
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


        /// <summary>
        /// Insert Image Details & Update Device Flag
        /// </summary>
        /// <param name="scheduleCode"></param>
        /// <param name="prRoadCode"></param>
        /// <param name="obsId"></param>
        /// <param name="fileName"></param>
        /// <param name="fileDesc"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="deviceFlag"></param>
        /// <returns></returns>
        public string UploadAndInsertImageDetails(byte[] imgData, string scheduleCode, string prRoadCode, string obsId, string fileName, string fileDesc,
                                               string latitude, string longitude, string deviceFlag)
        {
            dbContext = new PMGSYEntities();
            int insertCount = 0;
            bool isTransComplete = false;
            int updateCount = 0;
            int maxFileID = 0;
            string storageRoot = string.Empty;
            int currentImgNumber = 0;
            int totalImgCnt = 0;
            //latitude = "NcOKWPNlKDCrMEhzaFmaZA==";        //Value for Empty String.
            try
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("scheduleCode : " + scheduleCode);
                    sw.WriteLine("prRoadCode : " + prRoadCode);
                    sw.WriteLine("obsId : " + obsId);
                    sw.WriteLine("fileName : " + fileName);
                    sw.WriteLine("fileDesc : " + fileDesc);
                    sw.WriteLine("latitude : " + latitude);
                    sw.WriteLine("longitude : " + longitude);
                    sw.WriteLine("deviceFlag : " + deviceFlag);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                int decScheduleCode = Convert.ToInt32(cryptObj.Decrypt(scheduleCode));
                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
                int decObsId = Convert.ToInt32(cryptObj.Decrypt(obsId));
                string decFileName = cryptObj.Decrypt(fileName);
                string decFileDesc = cryptObj.Decrypt(fileDesc);

                string decLat = cryptObj.Decrypt(latitude);
                string decLong = cryptObj.Decrypt(longitude);

                if (decLat.Equals("") && decLong.Equals(""))
                {
                    decLat = "0";
                    decLong = "0";
                }

                decimal decLatitude = Convert.ToDecimal(decLat);
                decimal decLongitude = Convert.ToDecimal(decLong);

                string decDeviceFlag = cryptObj.Decrypt(deviceFlag);

                // First Take Max File ID to follow the naming convention for Files
                // As per qmType, set storagePath
                // countOfFilesUploaded taken to modify File Name with combination of (MaxFileId + 1) appended with "_" and countOfFilesUploaded
                Int32 countOfFilesUploaded = dbContext.QUALITY_QM_INSPECTION_FILE.Where(c => c.ADMIN_SCHEDULE_CODE == decScheduleCode && c.IMS_PR_ROAD_CODE == decPrRoadCode).Select(c => c.QM_FILE_ID).Count();
                currentImgNumber = countOfFilesUploaded;
                string qmType = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                 join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                 where qqs.ADMIN_SCHEDULE_CODE == decScheduleCode
                                 select aqm.ADMIN_QM_TYPE).First().ToString();
                if (countOfFilesUploaded == 0)
                {
                    countOfFilesUploaded = 1;
                }
                else
                {
                    countOfFilesUploaded = countOfFilesUploaded + 1;
                }

                if (dbContext.QUALITY_QM_INSPECTION_FILE.Count() == 0)
                    maxFileID = 0;
                else
                    maxFileID = (from c in dbContext.QUALITY_QM_INSPECTION_FILE select (Int32?)c.QM_FILE_ID ?? 0).Max();

                var fileId = maxFileID + 1;

                decFileName = fileId + "_" + countOfFilesUploaded + ".jpeg";

                if (qmType.Equals("I"))       //NQM
                {
                    storageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM"];
                }
                else if (qmType.Equals("S"))   //SQM 
                {
                    storageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM"];
                }

                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("MABQMSDAL.UploadAndInsertImageDetails : step1" );
                    sw.WriteLine("storageRoot : " + storageRoot);
                    sw.Close();
                }

                //------------------- Insert Image Details In Databse  ---------------------------- //
                using (var scope = new TransactionScope())
                {
                    totalImgCnt = Convert.ToInt32(dbContext.QUALITY_QM_SCHEDULE_DETAILS.Where(c => c.ADMIN_SCHEDULE_CODE == decScheduleCode && c.IMS_PR_ROAD_CODE == decPrRoadCode).Select(c => c.TOTAL_IMAGE_COUNT).First());

                    using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("MABQMSDAL.UploadAndInsertImageDetails : step2");
                        sw.WriteLine("currentImgNumber : " + currentImgNumber.ToString());
                        sw.WriteLine("totalImgCnt : " + totalImgCnt.ToString());
                        sw.Close();
                    }

                    // This condition is to check, is Total number of images already uploaded
                    if (currentImgNumber >= totalImgCnt)
                    {
                        return cryptObj.Encrypt("2");
                    }

                    insertCount = dbContext.USP_MABQMS_INSERT_IMAGE_DETAILS_CURRENT_DATE(decScheduleCode, decPrRoadCode, decObsId, decFileName, decFileDesc, decLatitude, decLongitude,"M");

                    if (insertCount > 0)
                    {
                        QUALITY_QM_SCHEDULE_DETAILS qmScheduleDetails = dbContext.QUALITY_QM_SCHEDULE_DETAILS.Find(decScheduleCode, decPrRoadCode);
                        qmScheduleDetails.DEVICE_FLAG = decDeviceFlag;
                        dbContext.Entry(qmScheduleDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    
                    scope.Complete();
                    isTransComplete = true;

                    using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("MABQMSDAL.UploadAndInsertImageDetails : step3");
                        sw.WriteLine("insertCount : " + insertCount.ToString());
                        sw.WriteLine("isTransComplete : " + isTransComplete.ToString());
                        sw.Close();
                    }
                }

                //------------------- Save Image to Disk ---------------------------- //

                if (isTransComplete)
                {
                    //string deviceFlg = dbContext.QUALITY_QM_SCHEDULE_DETAILS.Where(c => c.ADMIN_SCHEDULE_CODE == decScheduleCode && c.IMS_PR_ROAD_CODE == decPrRoadCode).Select(c => c.DEVICE_FLAG).First();
                    //if (deviceFlg.Equals("N"))
                     updateCount = dbContext.USP_MABQMS_UPDATE_DEVICE_FLAG(decScheduleCode, decPrRoadCode, decDeviceFlag);

                    // Take totalImgCnt to compare is uploading completed as number of images exceeds the limit set by the user.
                    // If reached to its totalImgCnt, update Device Flag to Completed Inspection.
                    //if (totalImgCnt == (currentImgNumber + 1))  //insertions is successful so, increment by 1
                    //{
                    //    updateCount = dbContext.USP_MABQMS_UPDATE_DEVICE_FLAG(decScheduleCode, decPrRoadCode, decDeviceFlag);
                    //}

                    // isTransComplete is true then only save Image to Disk
                    var fullPath = Path.Combine(storageRoot, decFileName);
                    var thumbnailPath = Path.Combine(storageRoot, "thumbnails");
                    var fullThumbnailPath = Path.Combine(thumbnailPath, decFileName);
                    int saveImgCnt = new CommonFunctions().SaveImage(imgData, storageRoot, fullPath, fullThumbnailPath);

                    using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("MABQMSDAL.UploadAndInsertImageDetails : step4");
                        sw.WriteLine("saveImgCnt : " + saveImgCnt.ToString());
                        sw.WriteLine("updateCount : " + updateCount.ToString());
                        sw.Close();
                    }
                    
                    if (saveImgCnt == 1 && updateCount > 0)
                        return cryptObj.Encrypt("2");           //for last image
                    else if (saveImgCnt == 1)
                        return cryptObj.Encrypt("1");           //for each image before last image
                    else
                    {
                        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("MABQMSDAL.UploadAndInsertImageDetails : step5");
                            sw.WriteLine("error in saving image, but other details uploaded : ");
                            sw.Close();
                        }

                        //for error in saving image, but other details uploaded
                        //Delete last saved record based on current file Name
                        QUALITY_QM_INSPECTION_FILE quality_qm_inspection_file = dbContext.QUALITY_QM_INSPECTION_FILE.Find(decObsId, fileId);
                        dbContext.QUALITY_QM_INSPECTION_FILE.Remove(quality_qm_inspection_file);
                        dbContext.SaveChanges();
                        return cryptObj.Encrypt("-2"); 
                    }
                }
                else
                    return cryptObj.Encrypt("-1");

                //--------------------------------------------------------------------- //
            }
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "MABQMSDAL.UploadAndInsertImageDetails().DbUpdateException");
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadAndInsertImageDetails()");
                    sw.WriteLine("Inner Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MABQMSDAL.UploadAndInsertImageDetails()");
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadAndInsertImageDetails()");
                    sw.WriteLine("Exception : " + ex.Message);
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
        /// Inspection Date added to the service. 
        /// After 18 Aug 2014 means 2 months after this release, remove function UploadAndInsertImageDetails();
        /// </summary>
        /// <param name="imgData"></param>
        /// <param name="scheduleCode"></param>
        /// <param name="prRoadCode"></param>
        /// <param name="obsId"></param>
        /// <param name="fileName"></param>
        /// <param name="fileDesc"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="deviceFlag"></param>
        /// <param name="inspDate"></param>
        /// <returns></returns>
        public string UploadAndInsertImageDetailsWithDate(byte[] imgData, string scheduleCode, string prRoadCode, string obsId, string fileName, string fileDesc,
                                               string latitude, string longitude, string deviceFlag, string inspDate)
        {
            dbContext = new PMGSYEntities();
            int insertCount = 0;
            bool isTransComplete = false;
            int updateCount = 0;
            int maxFileID = 0;
            string storageRoot = string.Empty;
            int currentImgNumber = 0;
            int totalImgCnt = 0;
            //latitude = "NcOKWPNlKDCrMEhzaFmaZA==";        //Value for Empty String.
            try
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("scheduleCode : " + scheduleCode);
                    sw.WriteLine("prRoadCode : " + prRoadCode);
                    sw.WriteLine("obsId : " + obsId);
                    sw.WriteLine("fileName : " + fileName);
                    sw.WriteLine("fileDesc : " + fileDesc);
                    sw.WriteLine("latitude : " + latitude);
                    sw.WriteLine("longitude : " + longitude);
                    sw.WriteLine("deviceFlag : " + deviceFlag);
                    sw.WriteLine("inspDate : " + inspDate);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                int decScheduleCode = Convert.ToInt32(cryptObj.Decrypt(scheduleCode));
                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
                int decObsId = Convert.ToInt32(cryptObj.Decrypt(obsId));
                string decFileName = cryptObj.Decrypt(fileName);
                string decFileDesc = cryptObj.Decrypt(fileDesc);

                string decLat = cryptObj.Decrypt(latitude);
                string decLong = cryptObj.Decrypt(longitude);

                if (decLat.Equals("") && decLong.Equals(""))
                {
                    decLat = "0";
                    decLong = "0";
                }

                decimal decLatitude = Convert.ToDecimal(decLat);
                decimal decLongitude = Convert.ToDecimal(decLong);

                string decDeviceFlag = cryptObj.Decrypt(deviceFlag);
                string decInspDate = cryptObj.Decrypt(inspDate);


                // First Take Max File ID to follow the naming convention for Files
                // As per qmType, set storagePath
                // countOfFilesUploaded taken to modify File Name with combination of (MaxFileId + 1) appended with "_" and countOfFilesUploaded
                Int32 countOfFilesUploaded = dbContext.QUALITY_QM_INSPECTION_FILE.Where(c => c.ADMIN_SCHEDULE_CODE == decScheduleCode && c.IMS_PR_ROAD_CODE == decPrRoadCode).Select(c => c.QM_FILE_ID).Count();
                currentImgNumber = countOfFilesUploaded;
                string qmType = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                 join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                 where qqs.ADMIN_SCHEDULE_CODE == decScheduleCode
                                 select aqm.ADMIN_QM_TYPE).First().ToString();
                if (countOfFilesUploaded == 0)
                {
                    countOfFilesUploaded = 1;
                }
                else
                {
                    countOfFilesUploaded = countOfFilesUploaded + 1;
                }

                if (dbContext.QUALITY_QM_INSPECTION_FILE.Count() == 0)
                    maxFileID = 0;
                else
                    maxFileID = (from c in dbContext.QUALITY_QM_INSPECTION_FILE select (Int32?)c.QM_FILE_ID ?? 0).Max();

                var fileId = maxFileID + 1;

                decFileName = fileId + "_" + countOfFilesUploaded + ".jpeg";

                if (qmType.Equals("I"))       //NQM
                {
                    storageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM"];
                }
                else if (qmType.Equals("S"))   //SQM 
                {
                    storageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM"];
                }



                //------------------- Insert Image Details In Databse  ---------------------------- //
                using (var scope = new TransactionScope())
                {
                    totalImgCnt = Convert.ToInt32(dbContext.QUALITY_QM_SCHEDULE_DETAILS.Where(c => c.ADMIN_SCHEDULE_CODE == decScheduleCode && c.IMS_PR_ROAD_CODE == decPrRoadCode).Select(c => c.TOTAL_IMAGE_COUNT).First());

                    // This condition is to check, is Total number of images already uploaded
                    if (currentImgNumber >= totalImgCnt)
                    {
                        //return cryptObj.Encrypt("2");
                        return cryptObj.Encrypt("-1");
                    }

                    insertCount = dbContext.USP_MABQMS_INSERT_IMAGE_DETAILS(decScheduleCode, decPrRoadCode, decObsId, decFileName, decFileDesc, decLatitude, decLongitude, decInspDate,"M");
                   
                    if (insertCount > 0)
                    {
                        QUALITY_QM_SCHEDULE_DETAILS qmScheduleDetails = dbContext.QUALITY_QM_SCHEDULE_DETAILS.Find(decScheduleCode, decPrRoadCode);
                        qmScheduleDetails.DEVICE_FLAG = decDeviceFlag;
                        dbContext.Entry(qmScheduleDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    scope.Complete();
                    isTransComplete = true;
                }

                //------------------- Save Image to Disk ---------------------------- //

                if (isTransComplete)
                {
                    //Update Flag on each image insertion
                    //string deviceFlg = dbContext.QUALITY_QM_SCHEDULE_DETAILS.Where(c => c.ADMIN_SCHEDULE_CODE == decScheduleCode && c.IMS_PR_ROAD_CODE == decPrRoadCode).Select(c => c.DEVICE_FLAG).First();
                    //if (deviceFlg.Equals("N"))
                    updateCount = dbContext.USP_MABQMS_UPDATE_DEVICE_FLAG(decScheduleCode, decPrRoadCode, decDeviceFlag);

                    // Take totalImgCnt to compare is uploading completed as number of images exceeds the limit set by the user.
                    // If reached to its totalImgCnt, update Device Flag to Completed Inspection.
                    //if (totalImgCnt == (currentImgNumber + 1))  //insertions is successful so, increment by 1
                    //{
                    //    updateCount = dbContext.USP_MABQMS_UPDATE_DEVICE_FLAG(decScheduleCode, decPrRoadCode, decDeviceFlag);
                    //}

                    // isTransComplete is true then only save Image to Disk
                    var fullPath = Path.Combine(storageRoot, decFileName);
                    var thumbnailPath = Path.Combine(storageRoot, "thumbnails");
                    var fullThumbnailPath = Path.Combine(thumbnailPath, decFileName);
                    int saveImgCnt = new CommonFunctions().SaveImage(imgData, storageRoot, fullPath, fullThumbnailPath);

                    if (saveImgCnt == 1 && updateCount > 0)
                        return cryptObj.Encrypt("2");           //for last image
                    else if (saveImgCnt == 1)
                        return cryptObj.Encrypt("1");           //for each image before last image
                    else
                    {
                        //for error in saving image, but other details uploaded
                        //Delete last saved record based on current file Name
                        QUALITY_QM_INSPECTION_FILE quality_qm_inspection_file = dbContext.QUALITY_QM_INSPECTION_FILE.Find(decObsId, fileId);
                        dbContext.QUALITY_QM_INSPECTION_FILE.Remove(quality_qm_inspection_file);
                        dbContext.SaveChanges();
                        return cryptObj.Encrypt("-2");
                    }
                }
                else
                    return cryptObj.Encrypt("-1");

                //--------------------------------------------------------------------- //
            }
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "MABQMSDAL.UploadAndInsertImageDetailsWithDate().DbUpdateException");
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadAndInsertImageDetails()");
                    sw.WriteLine("Inner Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MABQMSDAL.UploadAndInsertImageDetailsWithDate()");
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadAndInsertImageDetails()");
                    sw.WriteLine("Exception : " + ex.Message);
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
        /// Download Unplanned schedules which are not yet uploaded for last month & current month
        /// return all details with Month of scheudle
        /// </summary>
        /// <param name="scheduleCode"></param>
        /// <param name="prRoadCode"></param>
        /// <param name="monitorCode"></param>
        /// <returns></returns>
        public string DownloadUnplannedSchedule(string qmCode, string packageId, string inspMonth, string inspYear)
        {
            dbContext = new PMGSYEntities();
            try
            {
                int decQmCode = Convert.ToInt32(cryptObj.Decrypt(qmCode));
                string decPackageId = cryptObj.Decrypt(packageId);
                int decInspMonth = Convert.ToInt32(cryptObj.Decrypt(inspMonth));
                int decInspYear = Convert.ToInt32(cryptObj.Decrypt(inspYear));

                List<USP_MABQMS_DOWNLOAD_UNPLANNED_SCHEDULE_Result> itemList = new List<USP_MABQMS_DOWNLOAD_UNPLANNED_SCHEDULE_Result>();
                itemList = dbContext.USP_MABQMS_DOWNLOAD_UNPLANNED_SCHEDULE(decQmCode, decPackageId, decInspMonth, decInspYear).ToList<USP_MABQMS_DOWNLOAD_UNPLANNED_SCHEDULE_Result>();

                if (itemList.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                return cryptObj.Encrypt(GetXMLFromObject(itemList).Replace("&amp;", " and "));
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DownloadUnplannedSchedule()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DownloadUnplannedSchedule()");
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
        /// Assign Unplanned schedule
        /// </summary>
        /// <param name="prRoadCode"></param>
        /// <param name="monitorCode"></param>
        /// <param name="inspMonth"></param>
        /// <param name="inspYear"></param>
        /// <returns></returns>
        public string AssignUnplannedSchedule(string scheduleCode, string prRoadCode, string monitorCode, string ipAddr)
        {
            dbContext = new PMGSYEntities();
            try
            {
                int decScheduleCode = Convert.ToInt32(cryptObj.Decrypt(scheduleCode));
                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
                int decMonitorCode = Convert.ToInt32(cryptObj.Decrypt(monitorCode));
                string decIpAddress = cryptObj.Decrypt(ipAddr);
              
                var qmDetails = dbContext.ADMIN_QUALITY_MONITORS.Find(decMonitorCode);
                List<ADMIN_MODULE_CONFIGURATION> itemList = (from amc in dbContext.ADMIN_MODULE_CONFIGURATION
                                                             select amc).ToList<ADMIN_MODULE_CONFIGURATION>();
               
                QUALITY_QM_SCHEDULE_DETAILS qmAssignRoadsModel = new QUALITY_QM_SCHEDULE_DETAILS();
                qmAssignRoadsModel.ADMIN_SCHEDULE_CODE = decScheduleCode;
                qmAssignRoadsModel.IMS_PR_ROAD_CODE = decPrRoadCode;
                qmAssignRoadsModel.DEVICE_FLAG = "N";
                qmAssignRoadsModel.FINALIZE_FLAG = qmDetails.ADMIN_QM_TYPE.Equals("I") ? "NQM" : "SQM";
                qmAssignRoadsModel.SCHEDULE_ASSIGNED = qmDetails.ADMIN_QM_TYPE.Equals("I") ? "N" : "M";
                qmAssignRoadsModel.INSP_STATUS_FLAG = "UPGF";
                qmAssignRoadsModel.ADMIN_IS_ENQUIRY = "N";
                qmAssignRoadsModel.CQC_FORWARD_FLAG = "N";
                qmAssignRoadsModel.IS_SCHEDULE_DOWNLOAD = "N";
                qmAssignRoadsModel.TOTAL_IMAGE_COUNT = qmDetails.ADMIN_QM_TYPE.Equals("I") 
                                                        ? Convert.ToInt32(itemList.Where(c => c.Parameter.Equals("NQM_MIN_IMAGE_CNT")).Select(c => c.Value).First())
                                                        : Convert.ToInt32(itemList.Where(c => c.Parameter.Equals("SQM_MIN_IMAGE_CNT")).Select(c => c.Value).First());

                qmAssignRoadsModel.USERID = qmDetails.ADMIN_USER_ID;
                qmAssignRoadsModel.IPADD = decIpAddress;

                dbContext.QUALITY_QM_SCHEDULE_DETAILS.Add(qmAssignRoadsModel);
                int insertVal = dbContext.SaveChanges();

                return cryptObj.Encrypt(insertVal.ToString());
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "AssignUnplannedSchedule()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "AssignUnplannedSchedule()");
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
        /// Get Encryption Key of User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //public string GetEncryptionKey(int userId)
        //{
        //    dbContext = new PMGSYEntities();
        //    try
        //    {
        //        return Convert.ToString(dbContext.QUALITY_MOB_IMEI_NO.Where(m => m.UserId == userId).Select(m => m.EncryptionKey).FirstOrDefault());
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
        //        {
        //            sw.WriteLine("Date :" + DateTime.Now.ToString());
        //            sw.WriteLine("Method : " + "GetEncryptionKey()");
        //            sw.WriteLine("Database Exception : " + ex.Message);
        //            sw.WriteLine("____________________________________________________");
        //            sw.Close();
        //        }

        //        return "-1";
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}


        /// <summary>
        /// Download Notifiucation Message to User
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string DownloadNotifications(string userName)
        {
            dbContext = new PMGSYEntities();
            try
            {
                string decUserName = cryptObj.Decrypt(userName);
                var userId = dbContext.UM_User_Master.Where(c => c.UserName == decUserName).Select(c => c.UserID).First();

                var itemList = (from qqn in dbContext.QUALITY_QM_NOTIFICATIONS
                                where qqn.USER_ID == userId &&
                                qqn.IS_DOWNLOAD == false
                                select qqn).ToList<QUALITY_QM_NOTIFICATIONS>();

                if (itemList.Count == 0)
                {
                    return cryptObj.Encrypt("0");
                }

                //update Is Download Flag
                foreach (var item in itemList)
                {
                    var updateRecord = dbContext.QUALITY_QM_NOTIFICATIONS.Where(c => c.USER_ID == item.USER_ID && c.MESSAGE_ID == item.MESSAGE_ID && c.IS_DOWNLOAD == false).First();
                    updateRecord.IS_DOWNLOAD = true;
                    dbContext.Entry(updateRecord).State = System.Data.Entity.EntityState.Modified;
                }

                if (dbContext.SaveChanges() > 0)
                    return cryptObj.Encrypt(GetXMLFromObject(itemList).Replace("&amp;", " and "));
                else
                    return cryptObj.Encrypt("-1");
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DownloadNotifications()");
                    sw.WriteLine("Database Exception : " + ex.Message);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DownloadNotifications()");
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



        #region New Service 11 Feb 2021

       
        public string UploadAndInsertImageDetailsWithDateNew(byte[] imgData, string scheduleCode, string prRoadCode, string obsId, string fileName, string fileDesc,
                                       string latitude, string longitude, string deviceFlag, string inspDate)
        {
            dbContext = new PMGSYEntities();

            string storageRoot = string.Empty;


            try
            {
                #region Parameters Decrypting
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("scheduleCode : " + scheduleCode);
                    sw.WriteLine("prRoadCode : " + prRoadCode);
                    sw.WriteLine("obsId : " + obsId);
                    sw.WriteLine("fileName : " + fileName);
                    sw.WriteLine("fileDesc : " + fileDesc);
                    sw.WriteLine("latitude : " + latitude);
                    sw.WriteLine("longitude : " + longitude);
                    sw.WriteLine("deviceFlag : " + deviceFlag);
                    sw.WriteLine("inspDate : " + inspDate);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                int decScheduleCode = Convert.ToInt32(cryptObj.Decrypt(scheduleCode));
                int decPrRoadCode = Convert.ToInt32(cryptObj.Decrypt(prRoadCode));
                int decObsId = Convert.ToInt32(cryptObj.Decrypt(obsId));
                string decFileName = cryptObj.Decrypt(fileName);
                string decFileDesc = cryptObj.Decrypt(fileDesc);

                string decLat = cryptObj.Decrypt(latitude);
                string decLong = cryptObj.Decrypt(longitude);


                if (decLat.Equals("") && decLong.Equals(""))
                {
                    decLat = "0";
                    decLong = "0";
                }

                decimal decLatitude = Convert.ToDecimal(decLat);
                decimal decLongitude = Convert.ToDecimal(decLong);

                string decDeviceFlag = cryptObj.Decrypt(deviceFlag);
                string decInspDate = cryptObj.Decrypt(inspDate);

                string qmType = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                 join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                 where qqs.ADMIN_SCHEDULE_CODE == decScheduleCode
                                 select aqm.ADMIN_QM_TYPE).First().ToString();


                if (qmType.Equals("I"))       //NQM
                {
                    storageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM"];
                }
                else if (qmType.Equals("S"))   //SQM 
                {
                    storageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM"];
                }

                #endregion

                #region Database and Disk Saving

                using (var scope = new TransactionScope())
                {
                    bool isFilePresentOnDBorDisk = isFilePresent(storageRoot, decFileName, decScheduleCode, decPrRoadCode);

                    if (isFilePresentOnDBorDisk)
                    {// File is already present on Disk and Database
                        return cryptObj.Encrypt("-9$" + decFileName);
                    }
                    else
                    {// File is not present on Disk and Database

                        bool isfilesavedonDatabase = saveFileOnDatabase(decScheduleCode, decPrRoadCode, decObsId, decFileName, decFileDesc, decLatitude, decLongitude, decInspDate, "M");

                        if (isfilesavedonDatabase)
                        {
                            // imgData = null;

                            string result = saveFileOnDisk(decScheduleCode, decPrRoadCode, decDeviceFlag, storageRoot, decFileName, imgData, decObsId);
                            // here result will be 2  if last image and image is uploaded successfully , 1 if image is uploadeded succesfully but not last image, -2 in case of failure  and -1 in case of exception any where
                            scope.Complete();
                            return result;
                        }
                        else
                        {
                            // Imgae will not be saved in database. (Error while saving in database)
                            return cryptObj.Encrypt("-1$" + decFileName);
                        }

                    }
                }

                #endregion
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "MABQMSDAL.UploadAndInsertImageDetailsWithDateNew().DbUpdateException");
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadAndInsertImageDetailsWithDateNew().DbUpdateException");
                    sw.WriteLine("Inner Exception : " + ex.Message);
                    sw.WriteLine("Database Exception : " + ex.StackTrace.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return cryptObj.Encrypt("-1");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MABQMSDAL.UploadAndInsertImageDetailsWithDateNew().Exception");
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "UploadAndInsertImageDetailsWithDateNew().Exception");
                    sw.WriteLine("Exception : " + ex.Message);
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


        public bool saveFileOnDatabase(int decScheduleCode, int decPrRoadCode, int decObsId, string decFileName, string decFileDesc, decimal decLatitude, decimal decLongitude, string decInspDate, string deviceType)
        {

            int insertCount = 0;
            try
            {
                insertCount = dbContext.USP_MABQMS_INSERT_IMAGE_DETAILS(decScheduleCode, decPrRoadCode, decObsId, decFileName, decFileDesc, decLatitude, decLongitude, decInspDate, deviceType);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

        public string saveFileOnDisk(int decScheduleCode, int decPrRoadCode, string decDeviceFlag, string storageRoot, string decFileName, byte[] imgData, int decObsId)
        {
            try
            {
                int updateCount = 0;

                var fullPath = Path.Combine(storageRoot, decFileName);
                var thumbnailPath = Path.Combine(storageRoot, "thumbnails");
                var fullThumbnailPath = Path.Combine(thumbnailPath, decFileName);

                //using (StreamWriter sw = System.IO.File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "Before SaveImage() on Disk");

                //    sw.WriteLine("decFileName : " + decFileName);
                // //   sw.WriteLine("saveImgCnt : " + saveImgCnt);
                //    sw.WriteLine("updateCount : " + updateCount);
                // //   sw.WriteLine("fileId : " + fileId);

                //    sw.WriteLine("____________________________________________________");
                //    sw.Close();
                //}


                int saveImgCnt = new CommonFunctions().SaveImage(imgData, storageRoot, fullPath, fullThumbnailPath);

                //using (StreamWriter sw = System.IO.File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "After SaveImage() on Disk");

                //    sw.WriteLine("decFileName : " + decFileName);
                //    sw.WriteLine("saveImgCnt : " + saveImgCnt);
                //    sw.WriteLine("updateCount : " + updateCount);
                // //   sw.WriteLine("fileId : " + fileId);

                //    sw.WriteLine("____________________________________________________");
                //    sw.Close();
                //}

                if (saveImgCnt == 1 && updateCount > 0)
                    return cryptObj.Encrypt("2$" + decFileName);  // LAST IMAGE
                else if (saveImgCnt == 1)
                    return cryptObj.Encrypt("1$" + decFileName);     //for each image before last image
                else
                {
                    //    QUALITY_QM_INSPECTION_FILE quality_qm_inspection_file = dbContext.QUALITY_QM_INSPECTION_FILE.Find(decObsId, fileId);

                    QUALITY_QM_INSPECTION_FILE quality_qm_inspection_file = dbContext.QUALITY_QM_INSPECTION_FILE.Where(m => m.QM_OBSERVATION_ID == decObsId && m.QM_FILE_NAME == decFileName && m.IMS_PR_ROAD_CODE == decPrRoadCode && m.ADMIN_SCHEDULE_CODE == decScheduleCode).FirstOrDefault();
                    if (quality_qm_inspection_file != null)
                    {
                        dbContext.QUALITY_QM_INSPECTION_FILE.Remove(quality_qm_inspection_file);
                        dbContext.SaveChanges();
                    }
                    return cryptObj.Encrypt("-2$" + decFileName);
                }

            }
            catch (Exception ex)
            {
                using (StreamWriter sw = System.IO.File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "Incatch SaveImage() on Disk");

                    sw.WriteLine("decFileName : " + decFileName);
                    //   sw.WriteLine("saveImgCnt : " + saveImgCnt);
                    sw.WriteLine("ex.Message : " + ex.Message);


                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                QUALITY_QM_INSPECTION_FILE quality_qm_inspection_file = dbContext.QUALITY_QM_INSPECTION_FILE.Where(m => m.QM_OBSERVATION_ID == decObsId && m.QM_FILE_NAME == decFileName && m.IMS_PR_ROAD_CODE == decPrRoadCode && m.ADMIN_SCHEDULE_CODE == decScheduleCode).FirstOrDefault();
                if (quality_qm_inspection_file != null)
                {
                    dbContext.QUALITY_QM_INSPECTION_FILE.Remove(quality_qm_inspection_file);
                    dbContext.SaveChanges();
                }

                return cryptObj.Encrypt("-1$" + decFileName);

            }

        }


        public bool isFilePresent(string storageRoot, string decFileName, int decScheduleCode, int decPrRoadCode)
        {
            #region Check Image is available on Disk or Not

            //Schedule Code =====85736
            //Road Code =======93386
            //Observation Id======343824
            //File Name======85736_93386_3.jpg

            var fullPath = Path.Combine(storageRoot, decFileName);
            var thumbnailPath = Path.Combine(storageRoot, "thumbnails");
            var fullThumbnailPath = Path.Combine(thumbnailPath, decFileName);
            bool isFileOnDisk = false;
            bool isFileOnDatabase = false;

            if (System.IO.File.Exists(fullPath) && System.IO.File.Exists(fullThumbnailPath))
            {
                isFileOnDisk = true;
            }
            #endregion

            #region  Check Image is available in Database or Not

            if (dbContext.QUALITY_QM_INSPECTION_FILE.Where(c => c.QM_FILE_NAME == decFileName && c.ADMIN_SCHEDULE_CODE == decScheduleCode && c.IMS_PR_ROAD_CODE == decPrRoadCode).Any())
            {
                isFileOnDatabase = true;
            }
            #endregion

            if (isFileOnDisk == true && isFileOnDatabase == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }





        public string GetVersionNumberDAL()
        {
            try
            {
                dbContext = new PMGSYEntities();

                string VersionNumber = dbContext.ADMIN_MODULE_CONFIGURATION.Where(m => m.Id == 4).Select(m => m.Value).FirstOrDefault();
                return VersionNumber;
            }
            catch (Exception ex)
            {
                return "NA";

            }
        }



        #endregion


    }
}