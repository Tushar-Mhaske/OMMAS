using PMGSY.NERAGA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;


namespace PMGSY.GepnicAndNrega
{
    public class OMMASGePNICIntegration
    {
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["GepnicNregaErrorLogPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private static string ErrorLogFilePathForGIntegration = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";	

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
        public static string PostSanctionDataToGepnic(int state, int sanctionYear, int batch, int collaboration, string propType, int scheme)
        {
            try
            {
                var refToGepnicService = new GePNICWebReference.PullTenderService();
                string uniqueReferenceNo = string.Empty;
                //string xmlStrWithoutCDATA = string.Empty;
                string xmlString = string.Empty;
                string result = string.Empty;


                int cntTotalRecords = 0;

                OmmasIntegrationDAL objDAL = new OmmasIntegrationDAL();
                xmlString = objDAL.GetProposalData("G", state.ToString(), sanctionYear.ToString(), batch.ToString(), collaboration.ToString(), propType, scheme.ToString(), out uniqueReferenceNo, out   cntTotalRecords); // cntTotalRecords new parameter added

                string successMsg = "Error";

                // --- Call to Gepnic WebMethod
                var successMsgGepnic = refToGepnicService.getPreTenderXML(uniqueReferenceNo, xmlString);
                if (successMsgGepnic.Equals("Success"))
                {
                    result = objDAL.UpdateRecordCount(uniqueReferenceNo, "G"); //G for Gepnic
                    successMsg = "Gepnic Message : " + "Success";
                }
                else
                {
                    successMsg = "Gepnic Message : " + successMsgGepnic;
                }

                return successMsg;
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePathForGIntegration, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "OMMASGePNICIntegration.cs - PostSanctionDataToNrega()");
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
    }
}