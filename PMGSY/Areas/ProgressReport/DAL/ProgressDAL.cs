using PMGSY.Areas.ProgressReport.Models;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.ProgressReport.DAL
{
    public class ProgressDAL
    {
        public PMGSYEntities dbContext = null;

        #region Execution
        public Dictionary<string, string> GetFileDetails(int proposalCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                Dictionary<string, string> lstPaths = new Dictionary<string, string>();
                var lstPathDetails = (from item in dbContext.EXEC_FILES
                                      where item.IMS_PR_ROAD_CODE == proposalCode
                                      select
                                      new
                                      {
                                          item.EXEC_FILE_NAME,
                                          item.EXEC_FILE_DESC,
                                          item.EXEC_LATITUDE,
                                          item.EXEC_LONGITUDE,
                                          item.EXEC_UPLOAD_DATE
                                      }).ToList();

                foreach (var item in lstPathDetails)
                {
                    lstPaths.Add(Path.Combine(ConfigurationManager.AppSettings["EXECUTION_PRGRESS_FILE_UPLOAD_VIRTUAL_DIR_PATH"], "thumbnails/" + HttpUtility.UrlEncode(item.EXEC_FILE_NAME.Trim())
                            .Replace(@"\\", @"//").Replace(@"\", @"/") +
                            "$$$" + (item.EXEC_LATITUDE == null ? "0" : item.EXEC_LATITUDE.ToString()) + "$$" +
                            (item.EXEC_LONGITUDE == null ? "0" : item.EXEC_LONGITUDE.ToString()) +
                            "$$$" + (Convert.ToDateTime(item.EXEC_UPLOAD_DATE).ToString("dd/MM/yyyy hh:MM:ss"))
                            ), item.EXEC_FILE_DESC);
                }
                return lstPaths;
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoringDAL.GetFileDetails()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool IsLatLongAvailable(int proposalCode)
        {
            dbContext = new PMGSYEntities();
            bool isLatLongAvailable = false;
            try
            {
                var lstPathDetails = (from item in dbContext.EXEC_FILES
                                      where item.IMS_PR_ROAD_CODE == proposalCode
                                      select
                                      new
                                      {
                                          item.EXEC_LATITUDE,
                                          item.EXEC_LONGITUDE
                                      }).ToList();

                if (lstPathDetails.Any(c => c.EXEC_LATITUDE != null && c.EXEC_LONGITUDE != null)) // && c.EXEC_LATITUDE > 0 && c.EXEC_LONGITUDE > 0
                {
                    isLatLongAvailable = true;
                }

                return isLatLongAvailable;
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoringDAL.IsLatLongAvailable()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                return isLatLongAvailable;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public string GetMarkerDetails(int proposalCode)
        {
            dbContext = new PMGSYEntities();
            decimal value = (Decimal)0.00;
            try
            {
                if (dbContext.EXEC_FILES.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && (m.EXEC_LONGITUDE != value) && m.EXEC_LATITUDE != value))
                {
                    return "Y";
                }
                else
                {
                    return "N";
                }
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoringDAL.GetMarkerDetails()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}
                return "N";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ExecutionMonitoringDetails GetProposalDetails(int proposalCode)
        {
            dbContext = new PMGSYEntities();
            ExecutionMonitoringDetails model = new ExecutionMonitoringDetails();
            try
            {
                var proposalDetails = dbContext.IMS_SANCTIONED_PROJECTS.Find(proposalCode);

                model.State = proposalDetails.MASTER_STATE.MAST_STATE_NAME;
                model.District = proposalDetails.MASTER_DISTRICT.MAST_DISTRICT_NAME;
                model.Package = proposalDetails.IMS_PACKAGE_ID;
                model.SanctionYear = proposalDetails.IMS_YEAR + "-" + (proposalDetails.IMS_YEAR + 1);
                model.RoadName = proposalDetails.IMS_ROAD_NAME;
                model.BridgeName = proposalDetails.IMS_BRIDGE_NAME;
                model.Type = proposalDetails.IMS_PROPOSAL_TYPE.Equals("P") ? "Road" : "LSB";
                if (proposalDetails.IMS_ISCOMPLETED == "P")
                {
                    var vProList = dbContext.USP_SLR_PROPOSAL_STATUS(proposalDetails.IMS_PROPOSAL_TYPE, proposalCode).ToList();
                    foreach (var item in vProList)
                    {
                        model.RoadStatus = Convert.ToString(item);
                        break;
                    }
                }
                else
                {
                    model.RoadStatus = proposalDetails.IMS_ISCOMPLETED == "G" ? "Agreement" : proposalDetails.IMS_ISCOMPLETED == "C" ? "Completed" : proposalDetails.IMS_ISCOMPLETED == "X" ? "Maintenance" : proposalDetails.IMS_ISCOMPLETED == "A" ? "Pending-Land Equation" : proposalDetails.IMS_ISCOMPLETED == "L" ? "Pending Legal Cases" : proposalDetails.IMS_ISCOMPLETED == "F" ? "Pending Forest Clearance" : "";
                }
                model.RoadLength = proposalDetails.IMS_PAV_LENGTH.ToString();
                model.BridgeLength = proposalDetails.IMS_BRIDGE_LENGTH.ToString();
                return model;
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoringDAL.GetProposalDetails()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetLatLongDAL(int proposalId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                string latLongs = null;
                var fileDetails = (from qqif in dbContext.EXEC_FILES
                                   where qqif.IMS_PR_ROAD_CODE == proposalId
                                   select qqif).ToList();


                foreach (var item in fileDetails)
                {
                    latLongs = latLongs + item.EXEC_LATITUDE + "@" + item.EXEC_LONGITUDE + "$$";
                }

                return latLongs;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoringDAL.GetLatLongDAL()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// get the start end latitude and longitude according to the observatio id
        /// </summary>
        /// <param name="obsId"></param>
        /// <returns></returns>
        public string GetStartEndLatLong(int obsId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var schDetails = (from qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER
                                  where qqom.QM_OBSERVATION_ID == obsId
                                  select qqom).FirstOrDefault();


                return schDetails.QM_START_LATITUDE + "@" + schDetails.QM_START_LONGITUDE + "$$" + schDetails.QM_END_LATITUDE + "@" + schDetails.QM_END_LONGITUDE;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Lab
        public Dictionary<string, string> GetFileDetailsLAB(int labCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                Dictionary<string, string> lstPaths = new Dictionary<string, string>();
                var lstPathDetails = (from item in dbContext.QUALITY_QM_LAB_DETAILS
                                      where item.QM_LAB_ID == labCode
                                      select
                                      new
                                      {
                                          item.QM_LAB_FILE_NAME,
                                          item.QM_LAB_FILE_DESC,
                                          item.QM_LAB_FILE_LATITUDE,
                                          item.QM_LAB_FILE_LONGITUDE,
                                          item.QM_LAB_FILE_UPLOAD_DATE
                                      }).ToList();

                foreach (var item in lstPathDetails)
                {
                    lstPaths.Add(Path.Combine(ConfigurationManager.AppSettings["QUALITY_LAB_FILE_UPLOAD_VIRTUAL_DIR_PATH"], "thumbnails/" + HttpUtility.UrlEncode(item.QM_LAB_FILE_NAME.Trim())
                            .Replace(@"\\", @"//").Replace(@"\", @"/") +
                            "$$$" + (item.QM_LAB_FILE_LATITUDE == null ? "0" : item.QM_LAB_FILE_LATITUDE.ToString()) + "$$" +
                            (item.QM_LAB_FILE_LONGITUDE == null ? "0" : item.QM_LAB_FILE_LONGITUDE.ToString()) +
                            "$$$" + (Convert.ToDateTime(item.QM_LAB_FILE_UPLOAD_DATE).ToString("dd/MM/yyyy hh:MM:ss"))
                            ), item.QM_LAB_FILE_DESC);
                }
                return lstPaths;
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoringDAL.GetFileDetailsLAB()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool IsLatLongAvailableLAB(int labCode)
        {
            dbContext = new PMGSYEntities();
            bool isLatLongAvailable = false;
            try
            {
                var lstPathDetails = (from item in dbContext.QUALITY_QM_LAB_DETAILS
                                      where item.QM_LAB_ID == labCode
                                      select
                                      new
                                      {
                                          item.QM_LAB_FILE_LATITUDE,
                                          item.QM_LAB_FILE_LONGITUDE
                                      }).ToList();

                if (lstPathDetails.Any(c => c.QM_LAB_FILE_LATITUDE != null && c.QM_LAB_FILE_LONGITUDE != null)) // && c.EXEC_LATITUDE > 0 && c.EXEC_LONGITUDE > 0
                {
                    isLatLongAvailable = true;
                }

                return isLatLongAvailable;
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoringDAL.IsLatLongAvailableLAB()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                return isLatLongAvailable;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public string GetMarkerDetailsLAB(int labCode)
        {
            dbContext = new PMGSYEntities();
            decimal value = (Decimal)0.00;
            try
            {
                if (dbContext.QUALITY_QM_LAB_DETAILS.Any(m => m.QM_LAB_ID == labCode && (m.QM_LAB_FILE_LONGITUDE != value) && m.QM_LAB_FILE_LATITUDE != value))
                {
                    return "Y";
                }
                else
                {
                    return "N";
                }
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoringDAL.GetMarkerDetailsLAB()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}
                return "N";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ExecutionMonitoringDetails GetLABDetails(int labCode)
        {
            dbContext = new PMGSYEntities();
            ExecutionMonitoringDetails model = new ExecutionMonitoringDetails();
            try
            {
                var labDetails = dbContext.QUALITY_QM_LAB_MASTER.Where(x=>x.QM_LAB_ID == labCode).FirstOrDefault();

                model.Package = labDetails.IMS_PACKAGE_ID;
                int year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PACKAGE_ID == model.Package).Select(x=>x.IMS_YEAR).FirstOrDefault();
                model.SanctionYear = (year) + "-" + (year + 1);
                model.labEstablishDate = labDetails.QM_LAB_ESTABLISHMENT_DATE.ToString("dd/MM/yyyy");                

                return model;
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoringDAL.GetLABDetails()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetLatLongLABDAL(int labId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                string latLongs = null;
                var fileDetails = (from qqif in dbContext.QUALITY_QM_LAB_DETAILS
                                   where qqif.QM_LAB_ID == labId
                                   select qqif).ToList();


                foreach (var item in fileDetails)
                {
                    latLongs = latLongs + item.QM_LAB_FILE_LATITUDE + "@" + item.QM_LAB_FILE_LONGITUDE + "$$";
                }

                return latLongs;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoringDAL.GetLatLongLABDAL()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion
    }
}