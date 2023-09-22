using PMGSY.NERAGA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Configuration;
using System.Xml;

namespace PMGSY.GepnicAndNrega
{
    public class OMMASNregaIntegration
    {
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["GepnicNregaErrorLogPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private static string ErrorLogFilePathForIntegration = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";	

        /// <summary>
        /// Static Method to Push Data to Gepnic Application.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="sanctionYear"></param>
        /// <param name="batch"></param>
        /// <param name="collaboration"></param>
        /// <param name="propType"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static string PostSanctionDataToNrega(int state, int sanctionYear, int batch, int collaboration, string propType, int scheme)
        {
            try
            {
                var refToNregaService = new NREGAWebReference.pmgsyplantation();
                string uniqueReferenceNo = string.Empty;
                string xmlString = string.Empty;
                string result = string.Empty;
                int cntTotalRecords = 0;

                OmmasIntegrationDAL objDAL = new OmmasIntegrationDAL();

                
                    xmlString = objDAL.GetProposalData("N", state.ToString(), sanctionYear.ToString(), batch.ToString(), collaboration.ToString(), propType, scheme.ToString(), out uniqueReferenceNo, out cntTotalRecords);

                    string successMsg = "Error";

                    // --- Call to Nrega WebMethod
                    string StateCensusCode2011 = state.ToString();
                    //As per discussion with Mr.Anil kumar, input to the function state is considered as Ommas State code and not Nic state code                  
        
                    //objDAL.GetCensusStateCode(state);
                    string filex = xmlString;

                    var successMsgNrega = string.Empty;

                    if ((filex.Equals(string.Empty)) || (cntTotalRecords ==0) )
                    {
                        successMsgNrega = "No data Found";
                        return successMsgNrega;
                    }
                    else
                    {
                        try
                        {
                            XmlDocument xdoc = new XmlDocument();
                            xdoc.LoadXml(filex);
                            xdoc.Save(ErrorLogBasePath + "\\" + DateTime.Now.ToString("dd/MM/yyyy").Replace("/","-") +"_"+ state.ToString() + "_" + uniqueReferenceNo + ".xml");

                            //Uncomment below line
                            successMsgNrega = refToNregaService.uploadPMGSYDataWithOMMSCode(StateCensusCode2011, filex);
                            bool responseType = true;
                            result = objDAL.UpdateRecordCount(uniqueReferenceNo, "N", state, sanctionYear, cntTotalRecords, successMsgNrega, responseType);
                            successMsgNrega = "Data inserted Successfully";
                            return successMsgNrega;
                        }
                        catch (Exception ex)
                        {
                            if (!Directory.Exists(ErrorLogDirectory))
                            {
                                Directory.CreateDirectory(ErrorLogDirectory);
                            }

                            using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePathForIntegration, "")))
                            {
                                sw.WriteLine(" in  PostSanctionDataToNrega:" + DateTime.Now.ToString() + ex.Message.ToString());
                                sw.WriteLine("-----------------" + DateTime.Now.ToString() + ex.Message.ToString());
                                sw.Close();
                            }
                            bool responseType = false;

                            if ( ex.Message.ToString().Length>=200)
                            {
                                successMsgNrega = ex.Message.ToString().Substring(0, 199);
                            }
                            else
                            {
                                successMsgNrega = ex.Message.ToString();
                            }

                            result = objDAL.UpdateRecordCount(uniqueReferenceNo, "N", state, sanctionYear, cntTotalRecords, successMsgNrega, responseType);
                            successMsgNrega = "Error while uploading data";
                            return successMsgNrega;
                        }

                    }
                }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePathForIntegration, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "OMMASNregaIntegration.cs - PostSanctionDataToNrega()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return "Error Occurred While Pushing Data. Please Try Again.";
            }
        }


//        public void PostSanctionDataToNrega(int state, int sanctionYear, int batch, int collaboration, string propType, int scheme)
//        {
//            try
//            {
//                var refToNregaService = new NREGAWebReference.pmgsyplantation();
//                string uniqueReferenceNo = string.Empty;
//                string xmlString = string.Empty;
//                string result = string.Empty;

//                OmmasIntegrationDAL objDAL = new OmmasIntegrationDAL();
//                xmlString = objDAL.GetProposalData("N", state.ToString(), sanctionYear.ToString(), batch.ToString(), collaboration.ToString(), propType, scheme.ToString(), out uniqueReferenceNo);

//                string successMsg = "Error";

//                 --- Call to Nrega WebMethod
//                string StateCensusCode2011 = objDAL.GetCensusStateCode(state);
//                string filex = xmlString;

//                HttpContext.Current.Response.Clear();
//                HttpContext.Current.Response.ContentType = "text/xml";
//                HttpContext.Current.Response.Write(filex);
//                HttpContext.Current.Response.End();

//                 return filex;

//                var successMsgNrega = refToNregaService.uploadPMGSYData("35", filex);
//                var successMsgNrega = refToNregaService.uploadPMGSYDataWithOMMSCode(StateCensusCode2011, filex);
//                var successMsgNrega = string.Empty;
//                if (successMsgNrega.Equals("Success"))
//                {
//                    result = objDAL.UpdateRecordCount(uniqueReferenceNo, "N"); //N for Nrega
//                    successMsg = "Nrega Message : " + "Success";
//                }
//                else
//                {
//                    successMsg = "Nrega Message : " + successMsgNrega;
//                }

//                return successMsg;
//            }
//            catch (Exception ex)
//            {
//                if (!Directory.Exists(ErrorLogDirectory))
//                {
//                    Directory.CreateDirectory(ErrorLogDirectory);
//                }
//                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePathForIntegration, "")))
//                {
//                    sw.WriteLine("Date :" + DateTime.Now.ToString());
//                    sw.WriteLine("Method : " + "OMMASNregaIntegration.cs - PostSanctionDataToNrega()");
//                    sw.WriteLine("Exception : " + ex.Message);
//                    sw.WriteLine("Exception : " + ex.StackTrace);
//                    if (ex.InnerException != null)
//                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
//                    sw.WriteLine("____________________________________________________");
//                    sw.Close();
//                }
//return "Error Occurred While Pushing Data. Please Try Again.";
//            }
//        }
    }
}