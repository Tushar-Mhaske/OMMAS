using PMGSY.GepnicAndNrega;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace PMGSY.NERAGA
{
    public class OmmasIntegrationDAL
    {
        PMGSYEntities dbContext = null;
        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["GepnicNregaErrorLogPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";

        /// <summary>
        /// StringWriterWithEncoding class that inherits from StringWriter but 
        /// allows you to set the encoding in the constructor
        /// </summary>
        public class StringWriterUtf8 : StringWriter
        {
            public StringWriterUtf8()
                : base()
            { }

            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        /// <summary>
        /// Converts Objects in XML Form
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public string GetXMLFromObject(object obj)
        {
            XmlSerializer XmlS = new XmlSerializer(obj.GetType());

            StringWriter sw = new StringWriterUtf8();
            XmlTextWriter tw = new XmlTextWriter(sw);
            tw.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlS.Serialize(tw, obj, ns);
            return sw.ToString();
        }

       

        /// <summary>
        /// Get Proposal Data in correspondence with provided values
        /// </summary>
        /// <param name="state"></param>
        /// <param name="sanctionYear"></param>
        /// <param name="batch"></param>
        /// <param name="collaboration"></param>
        /// <param name="propType"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public string GetProposalData(string agencyFlag, string state, string sanctionYear, string batch, string collaboration, string propType, string scheme, out string uniqueReferenceNo, out int cntTotalRecords)
        {
            dbContext = new PMGSYEntities();
            cntTotalRecords = 0;
            try
            {
                //Insert into SP & get Reference ID Back
                ObjectParameter outParam = new ObjectParameter("refNo", typeof(Int64?));

                dbContext.USP_PROP_NERAGA_INTEGRATION_INSERT_DATA(Convert.ToInt32(state), Convert.ToInt32(sanctionYear), Convert.ToInt32(batch),
                                                                              Convert.ToInt32(collaboration), propType, Convert.ToInt32(scheme), "", outParam, "N", 0);
                long? refNo = Convert.ToInt64(outParam.Value);
                uniqueReferenceNo = refNo.ToString();


                List<USP_PROP_NERAGA_INTEGRATION_DATA_Result> itemList = new List<USP_PROP_NERAGA_INTEGRATION_DATA_Result>();

                itemList = dbContext.USP_PROP_NERAGA_INTEGRATION_DATA(refNo, agencyFlag).ToList<USP_PROP_NERAGA_INTEGRATION_DATA_Result>();
                cntTotalRecords = itemList.Count;
                if (agencyFlag.Equals("G"))
                {
                    foreach (var item in itemList)
                    {
                        item.CARRIAGED_WIDTH = Convert.ToInt32(item.CARRIAGED_WIDTH);
                    }
                }

                if (itemList.Count > 0)
                {
                    return GetXMLFromObject(itemList).Replace("&amp;", " and ").Replace(System.Environment.NewLine, " ");
                }
                else
                    return string.Empty;

            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "OmmasIntegrationDAL.cs - GetProposalData()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                uniqueReferenceNo = "0";
                return GetXMLFromObject("Error occurred while fetching data. Please try after some time.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        /// <summary>
        /// Update Pushed Records Count
        /// </summary>
        /// <returns></returns>
        public string UpdateRecordCount(string uniqueRefNo, string flag, int state, int sanctionYear, int cntTotalRecords, string successMsgNrega, bool responseType)
        {
            dbContext = new PMGSYEntities();
            try
            {

                OMMAS_GEPNIC_INTEGRATION_MASTER master = new OMMAS_GEPNIC_INTEGRATION_MASTER();

                if (dbContext.OMMAS_GEPNIC_INTEGRATION_MASTER.Any())
                {
                    master.MASTER_ID = dbContext.OMMAS_GEPNIC_INTEGRATION_MASTER.Max(m => m.MASTER_ID) + 1;
                }
                else
                {
                    master.MASTER_ID = 1;
                }

                master.REF_NO = Convert.ToInt64(uniqueRefNo);
                master.MAST_STATE_CODE = state;
                master.MAST_YEAR = sanctionYear;
                master.DATE_OF_INSERTION = DateTime.Now;
                master.NO_OF_RECORDS_INSERTED = cntTotalRecords;
                master.DATE_OF_RESPONSE = DateTime.Now;
                master.RESPONSE_MESSAGE = successMsgNrega;
                master.RESPONSE_TYPE = responseType;
                master.GEPNIC_NREGA = flag;
                dbContext.OMMAS_GEPNIC_INTEGRATION_MASTER.Add(master);
                dbContext.SaveChanges();

                int count = 0;
                if (responseType == true)
                {
                    count = dbContext.USP_PROP_NERAGA_UPDATE_RECORD_COUNT(Convert.ToInt64(uniqueRefNo), flag);
                }

                if (count > 0)
                {
                    return "Success";
                }
                else if (responseType == false)
                {
                    return "Error Occurred While getting response from NREGA";
                }
                else
                {
                    return "Data is successfully pushed . Error Occurred While Updating Pushed Records Count.";
                }
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "OmmasIntegrationDAL.cs - UpdateRecordCount()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return "Data is successfully pushed to GePNIC System. Error Occurred While Updating Pushed Records Count.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public string UpdateRecordCount(string uniqueRefNo, string flag)
        {
            dbContext = new PMGSYEntities();
            try
            {



                int count = dbContext.USP_PROP_NERAGA_UPDATE_RECORD_COUNT(Convert.ToInt64(uniqueRefNo), flag);
                if (count > 0)
                {
                    return "Success";
                }
                else
                {
                    return "Data is successfully pushed to GePNIC System. Error Occurred While Updating Pushed Records Count.";
                }
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "OmmasIntegrationDAL.cs - UpdateRecordCount()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return "Data is successfully pushed to GePNIC System. Error Occurred While Updating Pushed Records Count.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Get Census Code Of state
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public string GetCensusStateCode(Int32 stateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                Int32 censusCode = dbContext.MASTER_STATE.Where(c => c.MAST_STATE_CODE == stateCode).Select(c => c.MAST_NIC_STATE_CODE).FirstOrDefault();
                return censusCode == null ? stateCode.ToString() : censusCode.ToString();
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "OmmasIntegrationDAL.cs - GetCensusStateCode()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return "Error Occurred While Fetching Census Code of State.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}