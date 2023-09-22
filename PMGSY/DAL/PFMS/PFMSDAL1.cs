using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;
using PMGSY.Models.PFMS;
using PMGSY.Common;
using System.Data.Entity.Core.Objects;
using System.Xml.Linq;
using System.Transactions;
using System.Data.Entity;
using System.Security.Cryptography.X509Certificates;
using PMGSY.Extensions;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using iTextSharp.text;
using System.Data;
using System.Data.Entity.Core;
namespace PMGSY.DAL.PFMS
{
    public class PFMSDAL1
    {
        PMGSYEntities dbContext = null;

        #region Sammed Sir code

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

        public string GetXMLAsString(XmlDocument myxml)
        {
            return myxml.OuterXml;
        }

        static string GetXmlString(string strFile)
        {
            try
            {
                // Load the xml file into XmlDocument object.
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    //xmlDoc.Load(strFile);
                    xmlDoc.Load("$<root>{xmlString}</root>");
                }
                catch (XmlException e)
                {
                    Console.WriteLine(e.Message);
                }
                // Now create StringWriter object to get data from xml document.
                StringWriter sw = new StringWriter();
                XmlTextWriter xw = new XmlTextWriter(sw);
                xmlDoc.WriteTo(xw);
                return sw.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        #endregion

        #region Payment and DSC code by Pradip Patil

        public bool IsPaymentExists(long billId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return (dbContext.PFMS_OMMAS_PAYMENT_MAPPING.Any(x => x.BILL_ID == billId));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.IsPaymentExists()");
                return false;
            }
        }

        public string GeneratePaymentXMLDAL(PFMSDownloadPaymentXMLViewModel model, out string fileName, out int runningCnt)
        {
            string xmlString = string.Empty;
            string xmlHeader = string.Empty;
            string xmlBody = string.Empty;
            // GenerateDSCXml();
            //  fileName = "";
            // return "";
            string genDate = string.Empty;
            int runningCount = 0;

            CommonFunctions comm = new CommonFunctions();
            try
            {
                dbContext = new PMGSYEntities();

                String xmlFileName = String.Empty;

                //int AdminNdCode = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.MAST_STATE_CODE == model.stateCode && s.MAST_AGENCY_CODE == model.agencyCode && s.MAST_ND_TYPE == "S").ADMIN_ND_CODE;

                int AdminNdCode = PMGSYSession.Current.AdminNdCode;
                DateTime GenrationBillDate = Convert.ToDateTime(model.generationDate);
                var PFMSdataList = dbContext.PFMS_DATA_SEND_DETAILS.Where(x => x.ADMIN_ND_CODE == AdminNdCode).ToList();

                model.agencyCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x => x.MAST_AGENCY_CODE).FirstOrDefault();
                model.generationDate = comm.GetDateTimeToString(dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == model.billId).Select(x => x.BILL_DATE).FirstOrDefault());

                //if (PFMSdataList.Any(s => s.BILL_GENERATION_DATE.Value.Date == GenrationBillDate.Date) && model.FileType == "N") // && model.FileType == "N"
                //{
                //    fileName = "";
                //    return "0";
                //}

                //if (model.FileType == "R")
                //{
                //    String FileNameGenerated = PFMSdataList.Where(s => s.BILL_GENERATION_DATE.Value.Date == GenrationBillDate.Date).LastOrDefault().GENERATED_FILE_NAME;
                //    Boolean IsRejceFileExists = dbContext.PFMS_OMMAS_PAYMENT_MAPPING.Any(s => s.PAYMENT_REQ_FILENAME == FileNameGenerated && s.ACK_RECEVIED_FILENAME == null && s.ACK_BILL_STATUS == null);
                //    if (IsRejceFileExists)
                //    {
                //        fileName = "";
                //        return "1"; //rejeted file already genearted
                //    }
                //}

                //int BillId = 2990803;  //this billid for which payrequest is generated 
                Int64 BillId = model.billId;
                ObjectParameter outfilename = new System.Data.Entity.Core.Objects.ObjectParameter("XmlFileName", xmlFileName);
                ObjectParameter outParam = new System.Data.Entity.Core.Objects.ObjectParameter("RunningCount", runningCount);
                var hdr = dbContext.PFMS_PAYMENT_HEADER_XML(model.stateCode, model.agencyCode, Convert.ToDateTime(model.generationDate), model.FileType, BillId, outfilename, outParam).ToList();
                var bdy = dbContext.PFMS_GENERATE_PAYEMENT_XML(model.stateCode, model.agencyCode, Convert.ToDateTime(model.generationDate), BillId, model.FileType, Convert.ToString(outfilename.Value), Convert.ToInt32(outParam.Value)).ToList();


                xmlHeader = string.Join("", hdr);
                xmlBody = string.Join("", bdy);

                xmlString = "<DbtPayment xmlns=\"http://cpsms.nic.in/PaymentRequest\"><CstmrCdtTrfInitn>";

                xmlString = xmlString + xmlHeader.Trim() + xmlBody.Trim();

                xmlString = xmlString + "</CstmrCdtTrfInitn></DbtPayment>";

                XmlSerializer XmlS = new XmlSerializer(xmlString.GetType());

                StringWriter sw = new StringWriterUtf8();
                XmlTextWriter tw = new XmlTextWriter(sw);
                tw.WriteProcessingInstruction("xml", "version=\"1.0\"");

                XmlS.Serialize(tw, xmlString);

                xmlString = sw.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("<string>", "").Replace("</string>", "");
                fileName = outfilename.Value.ToString();
                runningCnt = Convert.ToInt32(outParam.Value);

                //if (bdy[0] != null)
                //{
                //    PFMS_DATA_SEND_DETAILS pfmsmodel = new PFMS_DATA_SEND_DETAILS();
                //    pfmsmodel.ID = dbContext.PFMS_DATA_SEND_DETAILS.Any() ? dbContext.PFMS_DATA_SEND_DETAILS.Max(s => s.ID) + 1 : 1;
                //    pfmsmodel.FUND_TYPE = "P";
                //    //pfmsmodel.ADMIN_ND_CODE = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.MAST_STATE_CODE == model.stateCode && s.MAST_AGENCY_CODE == model.agencyCode && s.MAST_ND_TYPE == "S").ADMIN_ND_CODE;
                //    pfmsmodel.ADMIN_ND_CODE = AdminNdCode;
                //    pfmsmodel.DATA_SEND_DATE = DateTime.Now;
                //    pfmsmodel.BILL_GENERATION_DATE = GenrationBillDate.Date;
                //    pfmsmodel.DATA_TYPE = "P"; //payment
                //    pfmsmodel.GENERATED_FILE_NAME = fileName + ".xml";
                //    dbContext.PFMS_DATA_SEND_DETAILS.Add(pfmsmodel);
                //    dbContext.SaveChanges();
                //    return xmlString;
                //}
                //return "";

                return xmlString;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GeneratePaymentXMLDAL()");
                fileName = "";
                runningCnt = 0;
                return string.Empty;
            }
        }

        public bool EditPFMSPaymentDetails(XElement doc, string FileName)
        {
            dbContext = new PMGSYEntities();
            string ackRecDate = string.Empty;
            string[] strArr = null;
            string rjCode = "";
            try
            {
                //adminNdCode = dbContext.ADMIN_DEPARTMENT.Where(x=>x.MAST_STATE_CODE == );
                // to access the element in xml file having namespace ,plz provide name along with tag name
                XNamespace ns = doc.GetDefaultNamespace();//"http://cpsms.nic.in/Beneficiary/ContractorPaymentAck";

                var xmlfiledata = (from ack in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "OrgnlPmtInfAndSts")
                                   select new
                                   {
                                       FileName = FileName,
                                       AckReceivedDate = (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr") select cd.Element(ns + "CreDtTm").Value),
                                       BatchId = ack.Element(ns + "OrgnlPmtInfId").Value,
                                       BillId = ack.Element(ns + "TxInfAndSts").Element(ns + "OrgnlEndToEndId").Value,
                                       Status = ack.Element(ns + "TxInfAndSts").Element(ns + "TxSts").Value,
                                       //RejectionCode = ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").HasElements == true ? ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null,
                                       //RejectionReason = ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").HasElements == true ? ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value : null,

                                       RejectionCode = ((ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ?
                                                (from cd in ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
                                                 select cd.Element(ns + "Cd").Value).ToList() : null),

                                       RejectionReason = ((ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
                                                  select cd.Element(ns + "AddtlInf").Value).ToList() : null),

                                       GrpStatus = (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "OrgnlGrpInfAndSts") select cd.Element(ns + "GrpSts").Value).FirstOrDefault(),
                                       GrpRejectionCode = ((doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "OrgnlGrpInfAndSts").Elements(ns + "GrpStsRsnInf").Elements(ns + "Rsn").Descendants(ns + "Cd").Count() > 0)
                                                           ? (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "OrgnlGrpInfAndSts").Elements(ns + "GrpStsRsnInf").Elements(ns + "Rsn") select cd.Element(ns + "Cd").Value).ToList()
                                                           : null),
                                       GrpRejectionReason = ((doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "OrgnlGrpInfAndSts").Elements(ns + "GrpStsRsnInf").Elements(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0)
                                                           ? (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "OrgnlGrpInfAndSts").Elements(ns + "GrpStsRsnInf").Elements(ns + "Rsn") select cd.Element(ns + "AddtlInf").Value).ToList()
                                                           : null),
                                   }).ToList();

                //using (TransactionScope ts = new TransactionScope())
                //{
                PFMS_OMMAS_PAYMENT_MAPPING paymentobj = null;
                foreach (var itm in xmlfiledata)
                {

                    long BillId = Convert.ToInt64(itm.BillId);
                    paymentobj = dbContext.PFMS_OMMAS_PAYMENT_MAPPING.Where(x => x.BATCH_ID == itm.BatchId && x.BILL_ID == BillId).FirstOrDefault();
                    if (paymentobj != null)
                    {
                        paymentobj.ACK_RECEVIED_FILENAME = itm.FileName;
                        //paymentobj.ACK_BILL_STATUS = (itm.Status == "ACCP" || itm.Status == "ACPT") ? "A" : "R";

                        paymentobj.ACK_BILL_STATUS = (itm.GrpStatus == "ACCP" || itm.GrpStatus == "ACPT") ? "A" : "R";

                        //paymentobj.REJECTION_CODE = itm.RejectionCode == "" ? null : itm.RejectionCode;
                        //paymentobj.REJECTION_NARRATION = itm.RejectionReason == "" ? null : itm.RejectionReason;

                        if (paymentobj.ACK_BILL_STATUS == "R")
                        {
                            paymentobj.REJECTION_CODE = itm.GrpRejectionCode == null ? null : string.Join(",", itm.GrpRejectionCode);
                            paymentobj.REJECTION_CODE += itm.RejectionCode == null ? null : (!string.IsNullOrEmpty(paymentobj.REJECTION_CODE)) ? "," + string.Join(",", itm.RejectionCode) : string.Join(",", itm.RejectionCode);

                            paymentobj.REJECTION_NARRATION = itm.GrpRejectionReason == null ? null : string.Join(",", itm.GrpRejectionReason);
                            paymentobj.REJECTION_NARRATION += itm.RejectionReason == null ? null : (!string.IsNullOrEmpty(paymentobj.REJECTION_NARRATION)) ? "," + string.Join(",", itm.RejectionReason) : string.Join(",", itm.RejectionReason);
                        }
                        else
                        {
                            paymentobj.REJECTION_CODE = null;
                            paymentobj.REJECTION_NARRATION = null;
                        }
                        strArr = itm.AckReceivedDate.ElementAt(0).Substring(0, 10).Split('-');
                        ackRecDate = strArr[2] + "/" + strArr[1] + "/" + strArr[0];
                        paymentobj.ACK_RECEIVED_DATE = (new CommonFunctions().GetStringToDateTime(ackRecDate.Trim()));
                        //(new System.Linq.SystemCore_EnumerableDebugView<string>(itm.AckReceivedDate as System.Linq.Enumerable.WhereSelectEnumerableIterator<System.Xml.Linq.XElement, string>)).Items[0];//

                        dbContext.Entry(paymentobj).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                //PFMS_DATA_SEND_DETAILS pfms_data_send_details = dbContext.PFMS_DATA_SEND_DETAILS.Where(x => x.GENERATED_FILE_NAME == genFileName).FirstOrDefault();
                //if (pfms_data_send_details != null)
                //{
                //    pfms_data_send_details.RECEIVED_FILE_NAME = FileName;
                //    dbContext.Entry(pfms_data_send_details).State = System.Data.Entity.EntityState.Modified;
                //}
                if (paymentobj != null)
                {
                    dbContext.SaveChanges();
                    PFMSLog("Payment", ConfigurationManager.AppSettings["PFMSPaymentLog"].ToString(), "Payment acknowledgement successful", FileName);
                }
                else
                {
                    PFMSLog("Payment", ConfigurationManager.AppSettings["PFMSPaymentLog"].ToString(), "No Records to update for payment acknowledgement", FileName);
                }
                //   ts.Complete();
                // }
                return true;
            }
            catch (Exception ex)
            {
                PFMSLog("Payment", ConfigurationManager.AppSettings["PFMSPaymentLog"].ToString(), "Error in payment acknowledgement", FileName);
                ErrorLog.LogError(ex, "PFMSDAL.EditPFMSPaymentDetails()");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        //
        public bool EditPFMSBankAcknowlegemntDetails(XElement doc, string FileName)
        {
            dbContext = new PMGSYEntities();
            string ackRecDate = string.Empty;
            string[] strArr = null;
            try
            {

                XNamespace ns = doc.GetDefaultNamespace();//"";

                var xmlfiledata = (from ack in doc.Element(ns + "CstmrCdtTrfInitn").Elements(ns + "PmtInf")
                                   select new
                                   {
                                       FileName = FileName,
                                       AckReceivedDate = (from cd in doc.Element(ns + "CstmrCdtTrfInitn").Elements(ns + "GrpHdr") select cd.Element(ns + "CreDtTm").Value),
                                       BatchId = ack.Element(ns + "PmtInfId").Value,
                                       BillId = ack.Element(ns + "CdtTrfTxInf").Element(ns + "PmtId").Element(ns + "EndToEndId").Value,
                                       Status = ack.Element(ns + "CdtTrfTxInf").Element(ns + "TxSts").Value,
                                       //RejectionCode = ack.Element(ns + "CdtTrfTxInf").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value,
                                       //RejectionReason = ack.Element(ns + "CdtTrfTxInf").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value

                                       InstrId = ack.Element(ns + "CdtTrfTxInf").Element(ns + "PmtId").Element(ns + "InstrId").Value,
                                       CreTxnId = ack.Element(ns + "CdtTrfTxInf").Element(ns + "PmtId").Element(ns + "CrTxID").Value,
                                       TxnDate = ack.Element(ns + "CdtTrfTxInf").Element(ns + "TxDt").Value,

                                       RejectionCode = ((ack.Element(ns + "CdtTrfTxInf").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "CdtTrfTxInf").Element(ns + "StsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
                                                  select cd.Element(ns + "Cd").Value).ToList() : null),

                                       RejectionReason = ((ack.Element(ns + "CdtTrfTxInf").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "CdtTrfTxInf").Element(ns + "StsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
                                                  select cd.Element(ns + "AddtlInf").Value).ToList() : null),
                                   }).ToList();

                //  using (TransactionScope ts = new TransactionScope())
                // {
                PFMS_OMMAS_PAYMENT_MAPPING paymentobj = null;
                foreach (var itm in xmlfiledata)
                {
                    long BillId = Convert.ToInt64(itm.BillId);
                    paymentobj = dbContext.PFMS_OMMAS_PAYMENT_MAPPING.Where(x => x.BATCH_ID == itm.BatchId && x.BILL_ID == BillId).FirstOrDefault();
                    if (paymentobj != null)
                    {
                        paymentobj.BANK_ACK_RECEVIED_FILENAME = itm.FileName;
                        paymentobj.BANK_ACK_BILL_STATUS = (itm.Status == "ACCP" || itm.Status == "ACPT") ? "A" : "R";
                        //paymentobj.BANK_ACK_REJECTION_CODE = itm.RejectionCode == "" ? null : itm.RejectionCode;
                        //paymentobj.BANK_ACK_REJECTION_NARRATION = itm.RejectionReason == "" ? null : itm.RejectionReason;

                        paymentobj.BANK_ACK_REJECTION_CODE = itm.RejectionCode == null ? null : string.Join(",", itm.RejectionCode);
                        paymentobj.BANK_ACK_REJECTION_NARRATION = itm.RejectionReason == null ? null : string.Join(",", itm.RejectionReason);

                        strArr = itm.AckReceivedDate.ElementAt(0).Substring(0, 10).Split('-');
                        ackRecDate = strArr[2] + "/" + strArr[1] + "/" + strArr[0];
                        paymentobj.BANK_ACK_RECEIVED_DATE = (new CommonFunctions().GetStringToDateTime(ackRecDate.Trim()));

                        paymentobj.BANK_ACK_INSTRID = itm.InstrId.Trim();
                        paymentobj.BANK_ACK_CRETXNID = itm.CreTxnId.Trim();

                        strArr = itm.TxnDate.Substring(0, 10).Split('-');
                        ackRecDate = strArr[2] + "/" + strArr[1] + "/" + strArr[0];
                        paymentobj.BANK_ACK_TXNDATE = new CommonFunctions().GetStringToDateTime(ackRecDate);

                        dbContext.Entry(paymentobj).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                //PFMS_DATA_SEND_DETAILS pfms_data_send_details = dbContext.PFMS_DATA_SEND_DETAILS.Where(x => x.GENERATED_FILE_NAME == genFileName).FirstOrDefault();
                //if (pfms_data_send_details != null)
                //{
                //    pfms_data_send_details.RECEIVED_FILE_NAME = FileName;
                //    dbContext.Entry(pfms_data_send_details).State = System.Data.Entity.EntityState.Modified;
                //}
                if (paymentobj != null)
                {
                    dbContext.SaveChanges();
                    PFMSLog("Bank", ConfigurationManager.AppSettings["PFMSBankLog"].ToString(), "Bank acknowledgement successful", FileName);
                }
                else
                {
                    PFMSLog("Bank", ConfigurationManager.AppSettings["PFMSBankLog"].ToString(), "No Records to update for Bank acknowledgement", FileName);
                }
                //  ts.Complete();
                // }
                return true;
            }
            catch (Exception ex)
            {
                PFMSLog("Bank", ConfigurationManager.AppSettings["PFMSBankLog"].ToString(), "Error in bank acknowledgement", FileName);
                ErrorLog.LogError(ex, "PFMSDAL.EditPFMSBankAcknowlegemntDetails()");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region XML File Validation
        /// <summary>
        /// Validate Is File type as Jpeg/png/gif
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// 
        public bool IsValidXMLFile(string basePath, HttpRequestBase request)
        {
            //byte[] EXE_SIGNATURE = { 77, 90 };
            byte[] XML_BYTES = new byte[] { 0x3c, 0x3f, 0x78, 0x6d, 0x6c, 0x20 };
            basePath = Path.Combine(basePath, "TemporaryFile");

            try
            {

                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);
                HttpPostedFileBase tempFile = request.Files[0];
                tempFile.SaveAs(Path.Combine(basePath, tempFile.FileName));

                var bytes = new byte[6];
                using (var fileStream = File.Open((Path.Combine(basePath, tempFile.FileName)), FileMode.Open))
                {
                    fileStream.Read(bytes, 0, 6);
                }

                if (bytes.SequenceEqual(XML_BYTES))
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return true;
                }
                else
                {
                    System.IO.File.Delete(Path.Combine(basePath, tempFile.FileName));
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.IsValidXMLFile");
                return false;
            }
        }
        #endregion

        #region DSC XML
        public string GenerateDSCXml(int adminNdCode, out string fileName, string operation, string AdminNdName)
        {
            try
            {
                dbContext = new PMGSYEntities();

                // ACC_BILL_MASTER bill = dbContext.ACC_BILL_MASTER.FirstOrDefault(s => s.BILL_ID == 2990803);

                ADMIN_NODAL_OFFICERS officer = dbContext.ADMIN_NODAL_OFFICERS.FirstOrDefault(x => x.ADMIN_MODULE == "A" && x.ADMIN_NO_DESIGNATION == 26 && x.ADMIN_ACTIVE_STATUS == "Y" && x.ADMIN_ND_CODE == adminNdCode);  //bill admin code used b4

                ACC_CERTIFICATE_DETAILS cert = dbContext.ACC_CERTIFICATE_DETAILS.FirstOrDefault(s => s.ADMIN_NO_OFFICER_CODE == officer.ADMIN_NO_OFFICER_CODE);

                ADMIN_DEPARTMENT AdminDeptPIU = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.ADMIN_ND_CODE == adminNdCode); //bill

                //ADMIN_DEPARTMENT AdminState = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.MAST_STATE_CODE == AdminDeptPIU.MAST_STATE_CODE && s.MAST_ND_TYPE == "S");

                //ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminState.ADMIN_ND_CODE && s.BANK_ACC_STATUS == true);
                ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");

                X509Certificate2 certifiate = new X509Certificate2(Convert.FromBase64String(cert.CERTIFICATE));

                String FileName = string.Empty;

                //using (StreamWriter sw = File.AppendText(@"F:\Cert.txt"))
                //using (StreamWriter sw = File.AppendText(ConfigurationManager.AppSettings["DSCXmlCertificateFilePath"]))
                //{
                //    sw.WriteLine("Issuer :" + certifiate.Issuer);
                //    sw.WriteLine("IssuerName :" + certifiate.IssuerName);
                //    sw.WriteLine("Thumbprint :" + certifiate.Thumbprint);
                //    sw.WriteLine("Issuer :" + certifiate.GetEffectiveDateString());
                //    sw.WriteLine("Issuer :" + certifiate.GetExpirationDateString());
                //    sw.Flush();
                //    sw.Close();
                //};

                int runningCount = dbContext.PFMS_DATA_SEND_DETAILS.Any(s => s.DATA_TYPE == "D" && s.FUND_TYPE == "P") //bill {{&& s.ADMIN_ND_CODE == adminNdCode}}
                                   ? (dbContext.PFMS_DATA_SEND_DETAILS.Where(s => s.DATA_TYPE == "D" && s.FUND_TYPE == "P" && EntityFunctions.TruncateTime(s.DATA_SEND_DATE) == DateTime.Today).Count()) + 1 //{{&& s.ADMIN_ND_CODE == adminNdCode}}
                                   : 1;
                FileName = "0037DSCENRREQ" + DateTime.Now.ToString("ddMMyyyy") + "" + runningCount.ToString("D3");

                StringBuilder startstring = new StringBuilder("<DscEnrolmentRequest xmlns=\"http://pfms.nic.in/EnrolmentRequest\"><AcctMndtMntncReq>");
                // string EndString = "</AcctMndtMntncReq></DscEnrolmentRequest>";



                PFMS_INITIATING_PARTY_MASTER initparty = dbContext.PFMS_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == /*AdminState.ADMIN_ND_CODE*/ AdminDeptPIU.MAST_PARENT_ND_CODE);
                XDocument doc = new XDocument(new XElement("GrpHdr",
                                                     new XElement("MsgID", "0037DSCENRREQ" + DateTime.Now.ToString("ddMMyyyy") + "" + runningCount.ToString("D3")),
                                                     new XElement("CreDtTm", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                                                     new XElement("OrgId",
                                                           new XElement("SchmeCd", "9179")
                                                                 ),
                                                      new XElement("InitgPty",
                                                              new XElement("Nm", initparty.PFMS_INIT_PARTYNAME),
                                                              new XElement("Id", initparty.PFMS_INIT_PARTY_UNIQUE_CODE)
                                                                  ),
                                                      new XElement("PrcId", "DSC"),
                                                      operation == "A"
                                                      ? new XElement("Prtry",
                                                                  new XElement("TxTp", "ADDN")
                                                                )
                                                      : new XElement("Prtry",
                                                                  new XElement("TxTp", "DEL")
                                                                )
                                                     ));
                string xmlstring = doc.ToString();

                string middleString = "<RptAcct><Accts>";
                XDocument middle_1 = new XDocument(new XElement("Acct",
                                                         new XElement("AcctId",
                                                              new XElement("Id",
                                                                   new XElement("BBAN", BankDetails.BANK_ACC_NO)),
                                                             new XElement("Nm", initparty.PFMS_INIT_PARTYNAME),
                                                             new XElement("Tp", "SVGS")))); //saving

                XDocument middle_2 = new XDocument(new XElement("AcctSvcrId",
                                                             new XElement("FinInstnNm", BankDetails.BANK_NAME),
                                                             new XElement("BrnchId", BankDetails.MAST_IFSC_CODE) //to be changed
                                                             )
                                                   );

                var endstring = "<Mndt><ID>" + officer.ADMIN_NO_OFFICER_CODE + "</ID><SgntrOrdrInd>" + 1 + "</SgntrOrdrInd><MndtHldr>";


                //bewlo part repetative

                var stateLgd = dbContext.OMMAS_LDG_STATE_MAPPING.FirstOrDefault(s => s.MAST_STATE_CODE == AdminDeptPIU.MAST_STATE_CODE);
                var districtLgd = dbContext.OMMAS_LDG_DISTRICT_MAPPING.FirstOrDefault(s => s.MAST_DISTRICT_CODE == AdminDeptPIU.MAST_DISTRICT_CODE);


                //Added on 16-11-2021 
                var MAST_DISTRICT_LDG_CODE = PMGSYSession.Current.LevelId == 4 ? "" : districtLgd.MAST_DISTRICT_LDG_CODE.ToString();

                XDocument LastCert = new XDocument(new XElement("DgtlSgntr",
                                                              new XElement("StartDt", Convert.ToDateTime(certifiate.GetEffectiveDateString()).ToString("yyyy-MM-dd")),
                                                              new XElement("EndDt", Convert.ToDateTime(certifiate.GetExpirationDateString()).ToString("yyyy-MM-dd")),
                                                    new XElement("Pty",
                                                             new XElement("NM", officer.ADMIN_NO_FNAME + " " + (officer.ADMIN_NO_MNAME ?? "") + " " + officer.ADMIN_NO_LNAME),
                                                             new XElement("Cert", certifiate.SerialNumber),
                                                             new XElement("ThumbPrint", certifiate.Thumbprint),
                                                             new XElement("Issr", certifiate.IssuerName.Name),
                                                             new XElement("ID",
                                                                   new XElement("PrvtId",
                    //  new XElement("SOSE",officer.ADMIN_AADHAR_NO)
                    //  new XElement("PAN","")
                                                                     "")
                                                                     ),
                                                             new XElement("Prtry",
                                                                     new XElement("Nm", officer.MASTER_DESIGNATION.MAST_DESIG_NAME)
                                                                     ),
                                                                  new XElement("LglAdr",
                                                                      new XElement("Dept", "Rural Department"),
                                                                      new XElement("SubDept", "Rural Department"),
                    //new XElement("PstCd","?"),
                                                                      //new XElement("DstCd", districtLgd.MAST_DISTRICT_LDG_CODE),
                                                                      new XElement("DstCd", MAST_DISTRICT_LDG_CODE),
                                                                      new XElement("PrvcCd", stateLgd.MAST_STATE_LDG_CODE)
                                                                              ),
                                                             new XElement("CtctDtls",
                    //new XElement("MobNb",""),
                                                                 new XElement("EmailAdr", officer.ADMIN_NO_EMAIL))
                                                                ),
                                                     new XElement("Authstn",
                    // new XElement("MinAmtPerTx", "0"),
                                                                  new XElement("MaxAmtPerTx", "10000000")
                                                                 )
                                                                )
                                                              );

                string EndStringRepeatSection = "</MndtHldr>";


                String DscDeclaretion = "I " + officer.ADMIN_NO_FNAME + " " + (officer.ADMIN_NO_MNAME ?? "") + " " + officer.ADMIN_NO_LNAME + " from Department, confirm that account mentioned in this message is operated by me and I shall be using this digital signature for payment signing as appended below with this message within the amount mentioned above through PFMS.";
                XDocument Declartion = new XDocument(new XElement("BkOpr",
                                                              new XElement("Domn",
                                                              new XElement("Cd", "PMNT")
                                                              )
                                                              )
                    //new XElement("MemFld",DscDeclaretion)//+I officer.ADMIN_NO_FNAME +" " +officer.ADMIN_NO_MNAME?""+officer.ADMIN_NO_LNAME +" from Department, confirm that account mentioned in this message is operated by me and I shall be using this digital signature for payment signing as appended below with this message within the amount mentioned above through PFMS.",
                                                        );

                String declartionendString = "<MemFld>" + DscDeclaretion + "</MemFld></Mndt></Accts></RptAcct></AcctMndtMntncReq></DscEnrolmentRequest>";


                startstring.Append(doc.ToString());
                startstring.Append(middleString);
                startstring.Append(middle_1.ToString());
                startstring.Append(middle_2.ToString());
                startstring.Append(endstring);
                startstring.Append(LastCert);
                startstring.Append(EndStringRepeatSection);
                startstring.Append(Declartion.ToString());
                startstring.Append(declartionendString);


                XDocument finaldoc = XDocument.Parse(startstring.ToString());

                //finaldoc.Save(@"d:\LINQ.XML");
                //finaldoc.Save(ConfigurationManager.AppSettings["FinalDSCXmlFilePath"]);

                PFMS_DATA_SEND_DETAILS Datasenddetails = new PFMS_DATA_SEND_DETAILS();
                Datasenddetails.ID = dbContext.PFMS_DATA_SEND_DETAILS.Any() ? dbContext.PFMS_DATA_SEND_DETAILS.Max(s => s.ID) + 1 : 1;
                Datasenddetails.ADMIN_ND_CODE = adminNdCode;
                Datasenddetails.FUND_TYPE = "P";
                Datasenddetails.BILL_GENERATION_DATE = null;
                Datasenddetails.DATA_SEND_DATE = DateTime.Now;
                Datasenddetails.DATA_TYPE = "D";
                Datasenddetails.RESPONSE_RECEIVED_DATE = null;
                Datasenddetails.RECEIVED_RESPONSE = null;
                Datasenddetails.GENERATED_FILE_NAME = FileName + ".xml";
                Datasenddetails.RECEIVED_FILE_NAME = null;

                Datasenddetails.OP_TYPE = operation;
                Datasenddetails.AUTH_NAME = AdminNdName.Trim();

                dbContext.PFMS_DATA_SEND_DETAILS.Add(Datasenddetails);
                dbContext.SaveChanges();

                fileName = FileName + ".xml";
                return startstring.ToString();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.GenerateDSCXml()");
                fileName = "";
                return String.Empty;
            }
        }

        public bool SaveDscEnrollmentAcknowlegement(XElement doc, string FileName, out bool isRecordExists)
        {
            isRecordExists = false;
            dbContext = new PMGSYEntities();
            try
            {
                XNamespace ns = doc.GetDefaultNamespace();//"";

                var xmlfiledata = (from ack in doc.Element(ns + "DSCEnrolmentAckRpt").Elements(ns + "OrgnlGrpInfAndSts")
                                   select new
                                   {
                                       FileName = FileName,
                                       MesageID = ack.Element(ns + "OrgnlMsgId").Value,
                                       FileStatus = ack.Element(ns + "GrpSts").Value,
                                       //FileStatusReasonCode = ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value,
                                       //FileStatusReason = ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value,

                                       FileStatusReasonCode = ((ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ? ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null),
                                       FileStatusReason = ((ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ? ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value : null),

                                       RecordStatus = ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "TxSts").Value,

                                       #region
                                       //RejectionCode = ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value,
                                       //RejectionReason = ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value

                                       //RejectionCode = ((ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ?
                                       //                     ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null),

                                       //RejectionCode = ((ack.Element(ns + "GrpStsRsnInf").Descendants(ns + "Rsn").Count() > 0) ?
                                       //                     ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null),
                                       //RejectionReason = ((ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ?
                                       //                     ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value : null)
                                       #endregion
                                       RejectionCode = ((ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "GrpStsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
                                                  select cd.Element(ns + "Cd").Value).ToList() : null),

                                       RejectionReason = ((ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "GrpStsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
                                                  select cd.Element(ns + "AddtlInf").Value).ToList() : null)
                                   }).ToList();

                PFMS_OMMAS_DSC_MAPPING DscMapping = null;


                for (int i = 0; i < xmlfiledata.Count; i++)
                {
                    String originalFile = xmlfiledata[i].MesageID + ".xml";
                    PFMS_DATA_SEND_DETAILS OrigMsg = dbContext.PFMS_DATA_SEND_DETAILS.SingleOrDefault(s => s.GENERATED_FILE_NAME == originalFile);

                    OrigMsg.RECEIVED_FILE_NAME = FileName;
                    OrigMsg.RESPONSE_RECEIVED_DATE = DateTime.Now;

                    //if (dbContext.PFMS_OMMAS_DSC_MAPPING.Any(x => x.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && x.DSC_REQ_FILENAME == DscMapping.DSC_REQ_FILENAME))
                    //{
                    //    isRecordExists = true;
                    //    return false;
                    //    PFMSLog("DSC", ConfigurationManager.AppSettings["PFMSDSCLog"].ToString(), "DSC already acknowledged", FileName);
                    //}

                    if (OrigMsg != null)
                    {
                        if (dbContext.PFMS_OMMAS_DSC_MAPPING.Any(x => x.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && x.DSC_REQ_FILENAME == originalFile))
                        {
                            ///If Previous status is rejected and current status is accepted then change status to accept else do nothing
                            DscMapping = dbContext.PFMS_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && x.DSC_REQ_FILENAME == originalFile).FirstOrDefault();
                            if (DscMapping.ACK_DSC_STATUS.Trim() == "RJCT" && (xmlfiledata[i].FileStatus == "ACPT" || xmlfiledata[i].FileStatus == "ACCP"))
                            {
                                DscMapping.ACK_DSC_STATUS = xmlfiledata[i].FileStatus.Trim();
                                DscMapping.REJECTION_CODE = null;
                                DscMapping.REJECTION_NARRATION = null;
                                DscMapping.IS_ACTIVE = true;
                                dbContext.Entry(DscMapping).State = System.Data.Entity.EntityState.Modified;

                                //if (DscMapping.IS_ACTIVE == true)
                                {
                                    var nodalOfficerDetails = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).FirstOrDefault();
                                    if (nodalOfficerDetails != null)
                                    {
                                        nodalOfficerDetails.IS_VALID_XML = true;
                                        nodalOfficerDetails.XML_FINALIZATION_DATE = DateTime.Now;
                                        dbContext.Entry(nodalOfficerDetails).State = System.Data.Entity.EntityState.Modified;
                                    }
                                }
                            }
                            else
                            {
                                isRecordExists = true;
                                PFMSLog("DSC", ConfigurationManager.AppSettings["PFMSDSCLog"].ToString(), "DSC already acknowledged", FileName);
                                return false;
                            }
                        }
                        else
                        {
                            DscMapping = new PFMS_OMMAS_DSC_MAPPING();

                            DscMapping.ID = dbContext.PFMS_OMMAS_DSC_MAPPING.Any() ? dbContext.PFMS_OMMAS_DSC_MAPPING.Max(s => s.ID) + 1 : 1;
                            DscMapping.ACK_RECEVIED_FILENAME = FileName.Trim();
                            DscMapping.DSC_REQ_FILENAME = xmlfiledata[i].MesageID + ".xml";
                            DscMapping.ACK_RECEIVED_DATE = DateTime.Now;
                            DscMapping.ACK_DSC_STATUS = xmlfiledata[i].FileStatus.Trim();
                            //DscMapping.REJECTION_CODE = String.IsNullOrEmpty(xmlfiledata[i].RejectionCode) ? null : string.Join(",", xmlfiledata[i].RejectionCode);//xmlfiledata[i].FileStatusReasonCode;
                            //DscMapping.REJECTION_NARRATION = String.IsNullOrEmpty(xmlfiledata[i].RejectionReason) ? null : string.Join(",", xmlfiledata[i].RejectionReason);//xmlfiledata[i].FileStatusReason;

                            DscMapping.REJECTION_CODE = xmlfiledata[i].RejectionCode == null ? null : string.Join(",", xmlfiledata[i].RejectionCode).Trim();//xmlfiledata[i].FileStatusReasonCode;
                            DscMapping.REJECTION_NARRATION = xmlfiledata[i].RejectionReason == null ? null : string.Join(",", xmlfiledata[i].RejectionReason).Trim();//xmlfiledata[i].FileStatusReason;

                            DscMapping.IS_ACTIVE = (DscMapping.ACK_DSC_STATUS.Trim() == "RJCT") ? false : true;

                            DscMapping.ADMIN_ND_CODE = OrigMsg.ADMIN_ND_CODE;
                            DscMapping.FILE_PROCESS_DATE = DateTime.Now;
                            dbContext.PFMS_OMMAS_DSC_MAPPING.Add(DscMapping);

                            #region on Rejection of DSC
                            if (DscMapping.IS_ACTIVE == false)
                            {
                                var nodalOfficerDetails = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).FirstOrDefault();
                                if (nodalOfficerDetails != null)
                                {
                                    nodalOfficerDetails.IS_VALID_XML = false;
                                    nodalOfficerDetails.XML_FINALIZATION_DATE = null;
                                    dbContext.Entry(nodalOfficerDetails).State = System.Data.Entity.EntityState.Modified;
                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        PFMSLog("DSC", ConfigurationManager.AppSettings["PFMSDSCLog"].ToString(), "No Records to update for DSC acknowledgement", FileName);
                        return false;
                    }
                }
                if (DscMapping != null)
                {
                    dbContext.SaveChanges();
                    PFMSLog("DSC", ConfigurationManager.AppSettings["PFMSDSCLog"].ToString(), "DSC acknowledged successful", FileName);
                }
                return true;
            }
            catch (Exception ex)
            {
                PFMSLog("DSC", ConfigurationManager.AppSettings["PFMSDSCLog"].ToString(), "Error on DSC acknowledgement", FileName);
                ErrorLog.LogError(ex, "PFMSDAL.SaveDscEnrollmentAcknowlegement()");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        public Boolean ValidXmlGenerateSetFlag()
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).Any())
                {
                    var nodalOfficerDetails = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).First();
                    dbContext.ADMIN_NODAL_OFFICERS.Attach(nodalOfficerDetails);
                    nodalOfficerDetails.IS_VALID_XML = true;
                    nodalOfficerDetails.XML_FINALIZATION_DATE = DateTime.Now;
                    dbContext.Entry<ADMIN_NODAL_OFFICERS>(nodalOfficerDetails).State = System.Data.Entity.EntityState.Modified;

                    PFMS_OMMAS_DSC_MAPPING DscMapping = dbContext.PFMS_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).OrderByDescending(c => c.FILE_PROCESS_DATE).FirstOrDefault();
                    if (DscMapping != null)
                    {
                        DscMapping.IS_ACTIVE = true;
                        dbContext.Entry(DscMapping).State = System.Data.Entity.EntityState.Modified;
                    }
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.ValidXmlGenerateSetFlag()");
                return false;
            }
        }

        public DSCPFMSModel ValidateDscPFMSDetails()
        {
            dbContext = new PMGSYEntities();
            try
            {

                ADMIN_NODAL_OFFICERS officer = dbContext.ADMIN_NODAL_OFFICERS.FirstOrDefault(x => x.ADMIN_MODULE == "A" && x.ADMIN_NO_DESIGNATION == 26 && x.ADMIN_ACTIVE_STATUS == "Y" && x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);  //bill admin code used b4

                ADMIN_DEPARTMENT AdminDeptPIU = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode); //bill

                //ADMIN_DEPARTMENT AdminState = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.MAST_STATE_CODE == AdminDeptPIU.MAST_STATE_CODE && s.MAST_ND_TYPE == "S");
                //ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminState.ADMIN_ND_CODE && s.BANK_ACC_STATUS == true);
                //PFMS_INITIATING_PARTY_MASTER initparty = dbContext.PFMS_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminState.ADMIN_ND_CODE);

                ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");
                PFMS_INITIATING_PARTY_MASTER initparty = dbContext.PFMS_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE);

                DSCPFMSModel model = new DSCPFMSModel();
                model.IsEmailAvailable = (officer == null) ? false : !String.IsNullOrEmpty(officer.ADMIN_NO_EMAIL.Trim());
                model.IsAccountNumberAvailable = (BankDetails == null) ? false : !String.IsNullOrEmpty(BankDetails.BANK_ACC_NO);
                model.IsIFSCAvailable = (BankDetails == null) ? false : !String.IsNullOrEmpty(BankDetails.MAST_IFSC_CODE);
                if (initparty != null)
                    model.IsInitPartyAvailable = true;
                else
                    model.IsInitPartyAvailable = false;

                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.ValidateDscPFMSDetails()");
                return null;
            }
        }

        public DSCPFMSModel ValidateDscPFMSDetailsforDelete(int officerCode)
        {
            dbContext = new PMGSYEntities();
            try
            {

                ADMIN_NODAL_OFFICERS officer = dbContext.ADMIN_NODAL_OFFICERS.FirstOrDefault(x => x.ADMIN_MODULE == "A" && x.ADMIN_NO_DESIGNATION == 26 && x.ADMIN_ACTIVE_STATUS == "Y" && x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == officerCode);  //bill admin code used b4

                ADMIN_DEPARTMENT AdminDeptPIU = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode); //bill

                //ADMIN_DEPARTMENT AdminState = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.MAST_STATE_CODE == AdminDeptPIU.MAST_STATE_CODE && s.MAST_ND_TYPE == "S");
                //ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminState.ADMIN_ND_CODE && s.BANK_ACC_STATUS == true);
                //PFMS_INITIATING_PARTY_MASTER initparty = dbContext.PFMS_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminState.ADMIN_ND_CODE);

                ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");
                PFMS_INITIATING_PARTY_MASTER initparty = dbContext.PFMS_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE);

                DSCPFMSModel model = new DSCPFMSModel();
                model.IsEmailAvailable = (officer == null) ? false : !String.IsNullOrEmpty(officer.ADMIN_NO_EMAIL.Trim());
                model.IsAccountNumberAvailable = (BankDetails == null) ? false : !String.IsNullOrEmpty(BankDetails.BANK_ACC_NO);
                model.IsIFSCAvailable = (BankDetails == null) ? false : !String.IsNullOrEmpty(BankDetails.MAST_IFSC_CODE);
                if (initparty != null)
                    model.IsInitPartyAvailable = true;
                else
                    model.IsInitPartyAvailable = false;

                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.ValidateDscPFMSDetails()");
                return null;
            }
        }

        public bool ValidatePFMSContractor(int mastConId, int conAccountId)
        {
            try
            {
                int lgdStateCode = dbContext.OMMAS_LDG_STATE_MAPPING.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(x => x.MAST_STATE_LDG_CODE).FirstOrDefault();
                return (dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(x => x.MAST_CON_ID == mastConId && x.MAST_ACCOUNT_ID == conAccountId && x.PFMS_CON_ID != null && x.STATUS == "A" && x.MAST_LGD_STATE_CODE == lgdStateCode && x.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode
                    && x.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Where(z => z.MAST_CON_ID == mastConId && z.MAST_ACCOUNT_ID == conAccountId && z.MAST_LOCK_STATUS == "Y").Select(a => a.MAST_ACCOUNT_STATUS).FirstOrDefault() == "A"));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.ValidatePFMSContractor");
                return false;
            }
        }

        public string GetDSCFileName(int adminNdCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.PFMS_DATA_SEND_DETAILS.FirstOrDefault(s => s.ADMIN_ND_CODE == adminNdCode && s.DATA_TYPE == "D").GENERATED_FILE_NAME;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.GetDSCFileName()");
                return String.Empty;
            }
        }

        public string GetStateShortName(int StateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.MASTER_STATE.FirstOrDefault(s => s.MAST_STATE_CODE == StateCode).MAST_STATE_SHORT_CODE;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.GetStateShortName");
                return String.Empty;
            }
        }

        #region Temporary Code to Test Sign Xml on LIVE
        public string GeneratePayXmlDAL(out string xmlFName)
        {
            int stateCode = 0, agencyCode = 0;
            string xmlString = string.Empty;
            string xmlHeader = string.Empty;
            string xmlBody = string.Empty;
            string xmlFileName = string.Empty;

            int runningCount = 0;

            var outParam = new System.Data.Entity.Core.Objects.ObjectParameter("XmlFileName", xmlFileName);
            var outParam1 = new System.Data.Entity.Core.Objects.ObjectParameter("RunningCount", runningCount);
            try
            {
                dbContext = new PMGSYEntities();

                agencyCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x => x.MAST_AGENCY_CODE).FirstOrDefault();

                var hdr = dbContext.PFMS_PAYMENT_HEADER_XML_TEMP(PMGSYSession.Current.StateCode, agencyCode, PMGSYSession.Current.AdminNdCode, outParam, outParam1).ToList();
                var bdy = dbContext.PFMS_GENERATE_PAYEMENT_XML_TEMP(PMGSYSession.Current.StateCode, agencyCode, PMGSYSession.Current.AdminNdCode, Convert.ToString(outParam.Value), Convert.ToInt32(outParam1.Value)).ToList();

                xmlFName = Convert.ToString(outParam.Value);

                xmlHeader = string.Join("", hdr);
                xmlBody = string.Join("", bdy);
                xmlString = xmlHeader.Trim() + xmlBody.Trim();

                XmlSerializer XmlS = new XmlSerializer(xmlString.GetType());

                StringWriter sw = new StringWriterUtf8();
                XmlTextWriter tw = new XmlTextWriter(sw);
                tw.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");

                XmlS.Serialize(tw, xmlString);

                xmlString = sw.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("<DbtBeneficiaries>", "").Replace("</DbtBeneficiaries>", "").Replace("<string>", @"<DbtPayment xmlns=""http://cpsms.nic.in/PaymentRequest""><CstmrCdtTrfInitn>").Replace("</string>", "</CstmrCdtTrfInitn></DbtPayment>");

                return xmlString;
            }
            catch (Exception ex)
            {
                xmlFName = "";
                ErrorLog.LogError(ex, "GeneratePayXmlDAL()");
                //fileName = "";
                return String.Empty;
            }
        }

        public bool ValidateSamplePayment()
        {
            try
            {
                dbContext = new PMGSYEntities();
                return (dbContext.PFMS_PAYMENT_CONFIGURATION_TABLE.Any(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.IS_ENABLE == true && (x.IS_XML_GENERATED == false || x.IS_XML_GENERATED == null)));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL1.ValidateSamplePayment()");
                return false;
            }
        }
        #endregion

        #region Generate XML
        public string GenerateXMLDAL(PFMSDownloadXMLViewModel model, out string xmlFName, out int recCount)
        {
            string xmlString = string.Empty;
            string xmlHeader = string.Empty;
            string xmlBody = string.Empty;
            string xmlFileName = string.Empty;
            int recordCount = 0;

            var outParam = new System.Data.Entity.Core.Objects.ObjectParameter("XmlFileName", xmlFileName);
            var outPrmCount = new System.Data.Entity.Core.Objects.ObjectParameter("RecordCount", recordCount);
            #region SqlParameter (for reference)
            //var outParam = new SqlParameter();
            //outParam.ParameterName = "TotalRows";
            //outParam.SqlDbType = SqlDbType.Int;
            //outParam.Direction = ParameterDirection.Output;
            #endregion
            try
            {
                dbContext = new PMGSYEntities();
                #region Call SP (for reference)
                //Object[] parameters = { model.stateCode, model.agencyCode };
                //var h = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_Header_XML", parameters).ToList();
                //var b = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_XML", parameters).ToList();
                #endregion

                //Create object of data table.                             
                DataTable ContractorIds = new DataTable();
                //Create Column
                ContractorIds.Columns.Add("MAST_CON_ID", typeof(int));
                ContractorIds.Columns.Add("MAST_ACCOUNT_ID", typeof(int));
                if (model.mastContractorIds != null)
                {
                    foreach (string conId in model.mastContractorIds)
                    {
                        if (conId.Split('$').Length > 2)
                        {
                            if (conId.Split('$')[2].ToUpper() == "TRUE")
                            {
                                ContractorIds.Rows.Add(new object[] { Convert.ToInt32(conId.Split('$')[0]), Convert.ToInt32(conId.Split('$')[1]) });
                            }
                        }
                    }
                }

                //var hdr = dbContext.PFMS_Generate_Conctractor_Header_XML(model.stateCode, model.agencyCode, model.districtCode, outParam, outPrmCount, ContractorIds).ToList();
                //var bdy = dbContext.PFMS_Generate_Conctractor_XML(model.stateCode, model.agencyCode, model.districtCode, ContractorIds).ToList();

                //xmlFName = Convert.ToString(outParam.Value);
                //recCount = Convert.ToInt32(outPrmCount.Value);

                Object[] parameters = { model.stateCode, model.agencyCode, model.districtCode, outParam, outPrmCount, ContractorIds };
                //var hdr = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_Header_XML", parameters).ToList();



                Object[] bparameters = { model.stateCode, model.agencyCode, model.districtCode, ContractorIds };
                //var bdy = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_XML", bparameters).ToList();

                //var hdr = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_Header_XML", new SqlParameter("stateCode", model.stateCode), new SqlParameter("agencyCode", model.agencyCode), new SqlParameter("DistrictC", model.districtCode), new SqlParameter("XmlFileName", outParam), new SqlParameter("RecordCount", outPrmCount), new SqlParameter("Contractors", ContractorIds)).ToList();

                //var bdy = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_XML", new SqlParameter("stateCode", model.stateCode), new SqlParameter("agencyCode", model.agencyCode), new SqlParameter("DistrictC", model.districtCode), new SqlParameter("Contractors", ContractorIds)).ToList();

                var levelParam = new SqlParameter("@Level", SqlDbType.Int);
                levelParam.Value = model.Level;

                var stateParam = new SqlParameter("@stateCode", SqlDbType.Int);
                stateParam.Value = model.stateCode;
                //stateParam.TypeName = "int";

                var agencyParam = new SqlParameter("@agencyCode", SqlDbType.Int);
                agencyParam.Value = model.agencyCode;
                //agencyParam.TypeName = "int";

                var distParam = new SqlParameter("@DistrictC", SqlDbType.Int);
                distParam.Value = model.districtCode;
                //distParam.TypeName = "int";

                //var xmlParam = new SqlParameter {
                //                   ParameterName = "@XmlFileName",
                //                   Value = 0,
                //                   Direction = ParameterDirection.Output };

                //var xmlParam = new SqlParameter("@XmlFileName", SqlDbType.VarChar);
                //xmlParam.Value = "";
                ////xmlParam.TypeName = "varchar";
                //xmlParam.Direction = ParameterDirection.Output;

                var xmlParam = new SqlParameter
                {
                    ParameterName = "XmlFileName",
                    DbType = System.Data.DbType.String,
                    Size = 30,
                    Direction = System.Data.ParameterDirection.Output
                };

                var recParam = new SqlParameter("@RecordCount", SqlDbType.Int);
                recParam.Value = 0;
                //recParam.TypeName = "int";
                recParam.Direction = ParameterDirection.Output;

                var conParam = new SqlParameter("@Contractors", SqlDbType.Structured);
                conParam.Value = ContractorIds;
                conParam.TypeName = "omms.ContractorList";

                var levelParam1 = new SqlParameter("@Level", SqlDbType.Int);
                levelParam1.Value = model.Level;

                var stateParam1 = new SqlParameter("@stateCode", SqlDbType.Int);
                stateParam1.Value = model.stateCode;
                //stateParam.TypeName = "int";

                var agencyParam1 = new SqlParameter("@agencyCode", SqlDbType.Int);
                agencyParam1.Value = model.agencyCode;
                //agencyParam.TypeName = "int";

                var distParam1 = new SqlParameter("@DistrictC", SqlDbType.Int);
                distParam1.Value = model.districtCode;

                var conParam1 = new SqlParameter("@Contractors", SqlDbType.Structured);
                conParam1.Value = ContractorIds;
                conParam1.TypeName = "omms.ContractorList";

                //var results = context.Database.SqlQuery<Person>("GetPersonAndVoteCount @id, @voteCount out", idParam,votesParam);
                var hdr = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_Header_XML @Level,@stateCode,@agencyCode,@DistrictC,@XmlFileName out,@RecordCount out,@Contractors", levelParam, stateParam, agencyParam, distParam, xmlParam, recParam, conParam).ToList();
                var bdy = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_XML @Level,@stateCode,@agencyCode,@DistrictC,@Contractors", levelParam1, stateParam1, agencyParam1, distParam1, conParam1).ToList();

                xmlFName = Convert.ToString(xmlParam.Value);
                recCount = Convert.ToInt32(recParam.Value);

                if (recCount == 0)
                {
                    return "";
                }

                xmlHeader = string.Join("", hdr);
                xmlBody = string.Join("", bdy);

                xmlString = xmlHeader.Trim() + xmlBody.Trim();

                XmlSerializer XmlS = new XmlSerializer(xmlString.GetType());

                StringWriter sw = new StringWriterUtf8();
                XmlTextWriter tw = new XmlTextWriter(sw);
                tw.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");

                XmlS.Serialize(tw, xmlString);

                xmlString = sw.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("<DbtBeneficiaries>", "").Replace("</DbtBeneficiaries>", "").Replace("<string>", @"<DbtBeneficiaries xmlns=""http://cpsms.nic.in/BeneficiaryDataRequest""><CstmrDtls>").Replace("</string>", "</CstmrDtls></DbtBeneficiaries>");



                dbContext = new PMGSYEntities();

                PFMS_DATA_SEND_DETAILS pfms_data_send_details = new PFMS_DATA_SEND_DETAILS();
                pfms_data_send_details.ID = !(dbContext.PFMS_DATA_SEND_DETAILS.Any()) ? 1 : (dbContext.PFMS_DATA_SEND_DETAILS.Max(x => x.ID) + 1);
                pfms_data_send_details.ADMIN_ND_CODE = dbContext.ADMIN_DEPARTMENT.Where(x => x.MAST_STATE_CODE == model.stateCode && x.MAST_AGENCY_CODE == model.agencyCode && x.MAST_ND_TYPE == "S").Select(x => x.ADMIN_ND_CODE).FirstOrDefault();
                pfms_data_send_details.FUND_TYPE = null;
                pfms_data_send_details.DATA_SEND_DATE = DateTime.Now;
                pfms_data_send_details.DATA_TYPE = "C";
                pfms_data_send_details.RESPONSE_RECEIVED_DATE = null;
                pfms_data_send_details.RECEIVED_RESPONSE = null;
                pfms_data_send_details.GENERATED_FILE_NAME = xmlFName.Trim();
                pfms_data_send_details.RECEIVED_FILE_NAME = null;

                dbContext.PFMS_DATA_SEND_DETAILS.Add(pfms_data_send_details);
                dbContext.SaveChanges();

                return xmlString;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.GenerateXMLDAL()");
                xmlFName = string.Empty;
                recCount = -1;
                return string.Empty;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetBeneficiaryDetailsDAL(int stateCode, int districtCode, int agencyCode, int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            string date = string.Empty;
            try
            {
                //int[] contractorIds = { 153, 2767, 5724, 1756, 7032, 22078 };

                dbContext = new Models.PMGSYEntities();
                var contractorDetails = (from con in dbContext.MASTER_CONTRACTOR
                                         join reg in dbContext.MASTER_CONTRACTOR_BANK on con.MAST_CON_ID equals reg.MAST_CON_ID
                                         join CON_REG in dbContext.MASTER_CONTRACTOR_REGISTRATION on con.MAST_CON_ID equals CON_REG.MAST_CON_ID
                                         join MS in dbContext.MASTER_STATE on CON_REG.MAST_REG_STATE equals MS.MAST_STATE_CODE
                                         join MD in dbContext.MASTER_DISTRICT on reg.MAST_DISTRICT_CODE equals MD.MAST_DISTRICT_CODE
                                         join LST in dbContext.OMMAS_LDG_STATE_MAPPING on MS.MAST_STATE_CODE equals LST.MAST_STATE_CODE
                                         join LDT in dbContext.OMMAS_LDG_DISTRICT_MAPPING on MD.MAST_DISTRICT_CODE equals LDT.MAST_DISTRICT_CODE
                                         //join pfmsCon in dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING on new { x1 = con.MAST_CON_ID, x2 = reg.MAST_ACCOUNT_ID, x3 = LST.MAST_STATE_LDG_CODE, x4 = agencyCode } equals new { x1 = pfmsCon.MAST_CON_ID, x2 = pfmsCon.MAST_ACCOUNT_ID, x3 = pfmsCon.MAST_LGD_STATE_CODE, x4 = pfmsCon.MAST_AGENCY_CODE.Value }
                                         where
                                         CON_REG.MAST_REG_STATUS == "A"
                                         && reg.MAST_ACCOUNT_STATUS == "A"
                                         && (reg.MAST_IFSC_CODE).Length > 10
                                         && !string.IsNullOrEmpty(reg.MAST_IFSC_CODE)
                                             //&& contractorIds.Contains(con.MAST_CON_ID)
                                         && !(dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(x => x.MAST_CON_ID == con.MAST_CON_ID && x.MAST_ACCOUNT_ID == reg.MAST_ACCOUNT_ID && x.PFMS_CON_ID != null && x.STATUS == "A" && x.MAST_LGD_STATE_CODE == LST.MAST_STATE_LDG_CODE && (x.MAST_AGENCY_CODE == agencyCode || x.MAST_AGENCY_CODE == null)))
                                         && MS.MAST_STATE_CODE == stateCode//PMGSYSession.Current.StateCode
                                         && MD.MAST_DISTRICT_CODE == (districtCode == 0 ? MD.MAST_DISTRICT_CODE : districtCode)
                                         && reg.MAST_LOCK_STATUS == "Y"
                                         select new
                                         {
                                             con.MAST_CON_ID,
                                             con.MAST_CON_PAN,
                                             reg.MAST_ACCOUNT_ID,
                                             reg.MASTER_DISTRICT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE,
                                             reg.MASTER_DISTRICT.OMMAS_LDG_DISTRICT_MAPPING.MAST_DISTRICT_CODE,
                                             con.MAST_CON_COMPANY_NAME,
                                             reg.MAST_IFSC_CODE,
                                             reg.MAST_ACCOUNT_NUMBER,
                                             //CON_NAME = Convert.ToString(con.MAST_CON_FNAME).Trim() + Convert.ToString(con.MAST_CON_FNAME).Trim() + Convert.ToString(con.MAST_CON_FNAME).Trim(),
                                             con.MAST_CON_FNAME,
                                             con.MAST_CON_MNAME,
                                             con.MAST_CON_LNAME,
                                             BANK_NAME = reg.MAST_BANK_NAME,//.Trim().Replace("\n", "").Replace("\r", ""),
                                             BATCH_ID = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(z => z.MAST_CON_ID == con.MAST_CON_ID && z.MAST_ACCOUNT_ID == reg.MAST_ACCOUNT_ID && z.MAST_LGD_STATE_CODE == LST.MAST_STATE_LDG_CODE && z.MAST_AGENCY_CODE == agencyCode).OrderByDescending(a => a.POCM_ID).Select(v => v.BATCH_ID).FirstOrDefault(),
                                             status = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(z => z.MAST_CON_ID == con.MAST_CON_ID && z.MAST_ACCOUNT_ID == reg.MAST_ACCOUNT_ID && z.MAST_LGD_STATE_CODE == LST.MAST_STATE_LDG_CODE && (z.MAST_AGENCY_CODE == agencyCode || z.MAST_AGENCY_CODE == null) && z.STATUS == "A" && z.PFMS_CON_ID != null) ? "Accepted"
                                                    : dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(z => z.MAST_CON_ID == con.MAST_CON_ID && z.MAST_ACCOUNT_ID == reg.MAST_ACCOUNT_ID && z.MAST_LGD_STATE_CODE == LST.MAST_STATE_LDG_CODE && (z.MAST_AGENCY_CODE == agencyCode || z.MAST_AGENCY_CODE == null) && z.STATUS == "R" && z.PFMS_CON_ID == null) ? "Rejected"
                                                    : "Processing at PFMS",
                                         }).Distinct().ToList();
                totalRecords = contractorDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                contractorDetails = contractorDetails.OrderBy(x => x.MAST_CON_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                contractorDetails = contractorDetails.OrderByDescending(x => x.MAST_CON_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    contractorDetails = contractorDetails.OrderByDescending(x => x.MAST_CON_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = contractorDetails.Select(conDetails => new
                {
                    conDetails.MAST_CON_FNAME,
                    conDetails.MAST_CON_MNAME,
                    conDetails.MAST_CON_LNAME,
                    conDetails.MAST_CON_PAN,
                    conDetails.MAST_CON_ID,
                    conDetails.MAST_CON_COMPANY_NAME,
                    conDetails.BANK_NAME,
                    conDetails.MAST_ACCOUNT_ID,
                    conDetails.MAST_IFSC_CODE,
                    conDetails.MAST_ACCOUNT_NUMBER,
                    conDetails.status,
                    flag = checkDate(conDetails.BATCH_ID, out date),
                    date
                    //flag = executionDetails.EXEC_COMPLETION_DATE.HasValue ? (executionDetails.EXEC_COMPLETION_DATE.Value >= stDate && executionDetails.EXEC_COMPLETION_DATE.Value <= endDate) : false,
                }).ToArray();

                return result.Select(lstcontractorDetails => new
                {
                    id = lstcontractorDetails.MAST_CON_ID.ToString(),
                    cell = new[] {      
                                    lstcontractorDetails.flag == true ? "<input id='cbx_'"+ lstcontractorDetails.MAST_CON_ID +" class='cbxCon' type='checkbox' title='Proposal is dropped.' name='cbContractor' value='" + lstcontractorDetails.MAST_CON_ID.ToString() + "$" + lstcontractorDetails.MAST_ACCOUNT_ID.ToString() + "$" + lstcontractorDetails.flag
                                    //+URLEncrypt.EncryptParameters1(new string[] {"MastConId="+lstcontractorDetails.MAST_CON_ID.ToString()})
                                    +"'>"
                                    : "<input id='cbx_'"+ lstcontractorDetails.MAST_CON_ID +" class='cbxCon' type='checkbox' title='Proposal is dropped.' name='cbContractor' disabled=disabled value='" + lstcontractorDetails.MAST_CON_ID.ToString()  + "$" + lstcontractorDetails.MAST_ACCOUNT_ID.ToString() + "'>",
                                    //"<input id='cbx_'"+ lstcontractorDetails.MAST_CON_ID +" class='cbxCon' type='checkbox' title='Proposal is dropped.' name='cbContractor' value='" + lstcontractorDetails.MAST_CON_ID.ToString() + "'>",
                                    //lstcontractorDetails.MAST_CON_ID.ToString(),
                                    Convert.ToString(lstcontractorDetails.MAST_CON_FNAME).Trim() + " " + Convert.ToString(lstcontractorDetails.MAST_CON_MNAME) + " " + Convert.ToString(lstcontractorDetails.MAST_CON_LNAME) + " (" + lstcontractorDetails.MAST_CON_ID.ToString() + ")",
                                    string.IsNullOrEmpty(lstcontractorDetails.MAST_CON_PAN) ? "-" : lstcontractorDetails.MAST_CON_PAN,
                                    lstcontractorDetails.MAST_CON_COMPANY_NAME,
                                    lstcontractorDetails.BANK_NAME.Trim().Replace("\n", "").Replace("\r", ""),
                                    lstcontractorDetails.MAST_ACCOUNT_ID.ToString(),
                                    lstcontractorDetails.MAST_IFSC_CODE,
                                    lstcontractorDetails.MAST_ACCOUNT_NUMBER.ToString(),

                                    lstcontractorDetails.status.Trim(),
                                    lstcontractorDetails.date
                                    //lstcontractorDetails.MAST_STATE_LDG_CODE.ToString(),
                                    //lstcontractorDetails.MAST_DISTRICT_CODE.ToString(),
                    }
                }).ToArray();
                //return contractorDetails.ToArray();
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                ErrorLog.LogError(ex, "PFMSDAL.GetBeneficiaryDetailsDAL()");
                return null;
            }
        }

        public bool checkDate(string genDate, out string date)
        {
            date = "";
            if (string.IsNullOrEmpty(genDate))
            {
                return true;
            }
            PMGSYEntities dbContext1 = new PMGSYEntities();
            DateTime currDate = DateTime.Now;
            string[] strArr = new string[3];
            try
            {
                strArr[0] = genDate.Substring(6, 2);
                strArr[1] = genDate.Substring(8, 2);
                strArr[2] = "20" + genDate.Substring(10, 2);

                date = strArr[0] + "/" + strArr[1] + "/" + strArr[2];

                DateTime dt = new DateTime(Convert.ToInt32(strArr[2]), Convert.ToInt32(strArr[1]), Convert.ToInt32(strArr[0]));

                if ((currDate - dt).TotalDays <= 2)
                {
                    return false;
                }
                else
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                date = "";
                ErrorLog.LogError(ex, "PFMSDAL.checkDate()");
                return false;
            }
            finally
            {
                dbContext1.Dispose();
            }
        }
        #endregion

        #region Map Contractor for PFMS

        public bool EditPFMSContractorDetails(List<ContractorMapping> lstModel, string xmlFileName)
        {
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            int lgdStateCode = 0;
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                {
                    PFMS_OMMAS_CONTRACTOR_MAPPING pfms_ommas_contractor_mapping = null;
                    string genFileName = string.Empty;
                    genFileName = xmlFileName.Trim().Contains('_') ? xmlFileName.Trim().Substring(0, xmlFileName.IndexOf('_')) : xmlFileName.Trim();
                    genFileName = genFileName.Replace('S', 'Q');
                    PFMS_DATA_SEND_DETAILS pfms_data_send_details = dbContext.PFMS_DATA_SEND_DETAILS.Where(x => x.GENERATED_FILE_NAME == genFileName).FirstOrDefault();
                    //if (pfms_data_send_details != null)
                    //{
                    //    lgdStateCode = pfms_data_send_details.ADMIN_DEPARTMENT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE;
                    //}
                    foreach (var itm in lstModel)
                    {
                        if (lgdStateCode == 0)
                        {
                            ///Get LGD State Code from file
                            lgdStateCode = dbContext.PFMS_INITIATING_PARTY_MASTER.Where(q => q.PFMS_INIT_PARTY_UNIQUE_CODE == itm.pfmsStateCode).Select(w => w.ADMIN_DEPARTMENT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE).FirstOrDefault();
                        }

                        if (lgdStateCode == 0)
                        {
                            ///Get LGD State Code from PFMS Data Send Details if unique code is changed
                            lgdStateCode = pfms_data_send_details.ADMIN_DEPARTMENT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE;
                        }


                        //PFMS_OMMAS_CONTRACTOR_MAPPING pfms_ommas_contractor_mapping = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(x => x.MAST_CON_ID == itm.contractorID && x.MAST_LGD_STATE_CODE == itm.lgdStateCode && x.MAST_LGD_DISTRICT_CODE == itm.lgdDistrictCode && x.MAST_ACCOUNT_NUMBER == itm.accountNumber.Trim() && x.MAST_BANK_NAME == itm.bankName.Trim() && x.MAST_IFSC_CODE == itm.branchName.Trim()).FirstOrDefault();
                        pfms_ommas_contractor_mapping = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(x => x.MAST_CON_ID == itm.contractorID && x.MAST_ACCOUNT_NUMBER.Contains(itm.accountNumber.Trim())
                            && x.MAST_BANK_NAME.Contains(itm.bankName.Trim().Replace("\n", "").Replace("\r", ""))
                            //&& x.MAST_IFSC_CODE == itm.branchName.Trim() 
                            && x.MAST_LGD_STATE_CODE == lgdStateCode && x.PFMS_CON_ID == null
                            && x.BATCH_ID == itm.batchId).FirstOrDefault();
                        if (pfms_ommas_contractor_mapping != null)
                        {
                            if (itm.cpsmsID != null)
                            {
                                pfms_ommas_contractor_mapping.PFMS_CON_ID = itm.cpsmsID;
                                pfms_ommas_contractor_mapping.STATUS = (itm.acceptStatus.Trim() == "ACCP" || itm.acceptStatus.Trim() == "ACPT") ? "A" : "R";
                            }
                            if (itm.lstRejectCode != null)
                            {
                                pfms_ommas_contractor_mapping.STATUS = (itm.acceptStatus.Trim() == "ACCP" || itm.acceptStatus.Trim() == "ACPT") ? "A" : "R";
                                pfms_ommas_contractor_mapping.REJECTION_CODE = string.Join(",", itm.lstRejectCode);
                                //dbContext.Entry(pfms_ommas_contractor_mapping).State = System.Data.Entity.EntityState.Modified;
                            }

                            pfms_ommas_contractor_mapping.ACK_FILE_NAME = xmlFileName.Trim().Substring(0, xmlFileName.IndexOf('.'));

                            string responseDate = itm.pfmsResponseDate.Substring(0, 10).Split('-')[2] + "/" + itm.pfmsResponseDate.Substring(0, 10).Split('-')[1] + "/" + itm.pfmsResponseDate.Substring(0, 10).Split('-')[0];
                            pfms_ommas_contractor_mapping.ACK_RECV_DATE = new CommonFunctions().GetStringToDateTime(responseDate.Trim());//DateTime.Now;

                            pfms_ommas_contractor_mapping.PFMS_CON_NAME = itm.pfmsConName;
                            pfms_ommas_contractor_mapping.PFMS_IFSC_CODE = itm.branchName;
                            pfms_ommas_contractor_mapping.PFMS_STATUS = (itm.acceptStatus.Trim() == "ACCP" || itm.acceptStatus.Trim() == "ACPT") ? "A" : "R";

                            dbContext.Entry(pfms_ommas_contractor_mapping).State = System.Data.Entity.EntityState.Modified;

                            #region Definalize for Contractor correction after rejection from PFMS commented
                            //if (pfms_ommas_contractor_mapping.STATUS == "R")
                            //{
                            //    MASTER_CONTRACTOR_BANK bank = dbContext.MASTER_CONTRACTOR_BANK.Where(x => x.MAST_CON_ID == itm.contractorID && x.MAST_ACCOUNT_ID == pfms_ommas_contractor_mapping.MAST_ACCOUNT_ID && x.MASTER_DISTRICT.OMMAS_LDG_DISTRICT_MAPPING.MAST_DISTRICT_LDG_CODE == pfms_ommas_contractor_mapping.MAST_LGD_DISTRICT_CODE).FirstOrDefault();
                            //    if (bank != null)
                            //    {
                            //        bank.MAST_LOCK_STATUS = "N";
                            //        dbContext.Entry(bank).State = System.Data.Entity.EntityState.Modified;
                            //    }
                            //}
                            #endregion
                        }
                    }

                    if (pfms_data_send_details != null)
                    {
                        pfms_data_send_details.RECEIVED_FILE_NAME = xmlFileName.Trim().Substring(0, xmlFileName.IndexOf('.'));
                        dbContext.Entry(pfms_data_send_details).State = System.Data.Entity.EntityState.Modified;
                    }
                    if (pfms_ommas_contractor_mapping != null || pfms_ommas_contractor_mapping != null)
                    {
                        //dbContext.SaveChanges();
                        PFMSLog("Beneficiary", ConfigurationManager.AppSettings["PFMSBeneficiaryLog"].ToString(), "Beneficiary acknowledgement successful", xmlFileName);
                    }
                    else
                    {
                        PFMSLog("Beneficiary", ConfigurationManager.AppSettings["PFMSBeneficiaryLog"].ToString(), "No Records updated for Beneficiary acknowledgement", xmlFileName);
                    }
                    //ts.Complete();
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (DbEntityValidationException e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ErrorLog.LogError(e, "SaveRoadProposalDAL(DbEntityValidationException ex).DAL");
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }
                ErrorLog.LogError(e, "PFMSDAL.EditPFMSContractorDetails()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("PFMSDAL.EditPFMSContractorDetails() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                //return new CommonFunctions().FormatErrorMessage(modelstate);
                return false;
            }

            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "PFMSDAL.EditPFMSContractorDetails()");
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "PFMSDAL.EditPFMSContractorDetails()");
                return false;
            }
            catch (Exception ex)
            {
                PFMSLog("Beneficiary", ConfigurationManager.AppSettings["PFMSBeneficiaryLog"].ToString(), "Error on Beneficiary acknowledgement", xmlFileName);
                ErrorLog.LogError(ex, "PFMSDAL.EditPFMSContractorDetails()");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool EditPFMSContractorDetailsTemp(List<ContractorMapping> lstModel, string xmlFileName)
        {
            dbContext = new PMGSYEntities();
            int lgdStateCode = 0;
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                {
                    List<PFMS_OMMAS_CONTRACTOR_MAPPING_TEMP> pfms_ommas_contractor_mapping_temp = null;
                    string genFileName = string.Empty;
                    genFileName = xmlFileName.Trim().Contains('_') ? xmlFileName.Trim().Substring(0, xmlFileName.IndexOf('_')) : xmlFileName.Trim();
                    genFileName = genFileName.Replace('S', 'Q');



                    PFMS_DATA_SEND_DETAILS_TEMP pfms_data_send_details_temp = dbContext.PFMS_DATA_SEND_DETAILS_TEMP.Where(x => x.GENERATED_FILE_NAME == genFileName && x.DATA_TYPE == "C").FirstOrDefault();
                    //if (pfms_data_send_details_temp != null)
                    //{
                    //    lgdStateCode = pfms_data_send_details_temp.ADMIN_DEPARTMENT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE;
                    //}

                    foreach (var itm in lstModel)
                    {
                        if (lgdStateCode == 0)
                        {
                            ///Get LGD State Code from file
                            lgdStateCode = dbContext.PFMS_INITIATING_PARTY_MASTER.Where(q => q.PFMS_INIT_PARTY_UNIQUE_CODE == itm.pfmsStateCode).Select(w => w.ADMIN_DEPARTMENT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE).FirstOrDefault();
                            //PFMSLog("BeneficiaryTemp", ConfigurationManager.AppSettings["PFMSBeneficiaryTempLog"].ToString(), "lgdStateCode not found", xmlFileName);
                        }
                        if (lgdStateCode == 0)
                        {
                            ///Get LGD State Code from PFMS Data Send Details if unique code is changed
                            lgdStateCode = pfms_data_send_details_temp.ADMIN_DEPARTMENT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE;
                        }

                        pfms_ommas_contractor_mapping_temp = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING_TEMP.Where(x => x.MAST_CON_ID == itm.contractorID
                            && x.MAST_ACCOUNT_NUMBER.Contains(itm.accountNumber.Trim())
                            && x.MAST_BANK_NAME.Contains(itm.bankName.Trim().Replace("\n", "").Replace("\r", ""))
                            && x.MAST_LGD_STATE_CODE == lgdStateCode
                            ).OrderBy(c => c.MAST_CON_ID).ThenBy(v => v.MAST_ACCOUNT_ID).ToList();
                        if (pfms_ommas_contractor_mapping_temp != null && pfms_ommas_contractor_mapping_temp.Count() > 0)
                        {
                            var lstBatch = pfms_ommas_contractor_mapping_temp.Select(a => a.BATCH_ID).ToList();
                            if (lstBatch.Contains(itm.batchId))
                            {
                                pfms_ommas_contractor_mapping_temp = pfms_ommas_contractor_mapping_temp.Where(b => b.BATCH_ID == itm.batchId).ToList();
                            }

                            foreach (PFMS_OMMAS_CONTRACTOR_MAPPING_TEMP dbitem in pfms_ommas_contractor_mapping_temp)
                            {
                                if (dbitem.STATUS != "A")
                                {
                                    dbitem.PFMS_CON_ID = string.IsNullOrEmpty(itm.cpsmsID) ? null : itm.cpsmsID;
                                    dbitem.STATUS = (itm.acceptStatus.Trim() == "ACCP" || itm.acceptStatus.Trim() == "ACPT") ? "A" : "R";

                                    if (dbitem.STATUS == "A")
                                    {
                                        dbitem.REJECTION_CODE = null;
                                    }

                                    if (itm.lstRejectCode != null)
                                    {
                                        dbitem.REJECTION_CODE = string.Join(",", itm.lstRejectCode);
                                        //dbContext.Entry(pfms_ommas_contractor_mapping).State = System.Data.Entity.EntityState.Modified;
                                    }

                                    dbitem.ACK_FILE_NAME = xmlFileName.Trim().Substring(0, xmlFileName.IndexOf('.'));

                                    string responseDate = itm.pfmsResponseDate.Substring(0, 10).Split('-')[2] + "/" + itm.pfmsResponseDate.Substring(0, 10).Split('-')[1] + "/" + itm.pfmsResponseDate.Substring(0, 10).Split('-')[0];
                                    dbitem.ACK_RECV_DATE = new CommonFunctions().GetStringToDateTime(responseDate.Trim());//DateTime.Now;

                                    dbitem.PFMS_CON_NAME = itm.pfmsConName;
                                    dbitem.PFMS_IFSC_CODE = itm.branchName;
                                    dbitem.PFMS_STATUS = (itm.acceptStatus.Trim() == "ACCP" || itm.acceptStatus.Trim() == "ACPT") ? "A" : "R";
                                    dbContext.Entry(dbitem).State = System.Data.Entity.EntityState.Modified;
                                }
                            }

                        }
                        else
                        {
                            PFMSLog("BeneficiaryTemp", ConfigurationManager.AppSettings["PFMSBeneficiaryTempLog"].ToString(), "No records in pfms_ommas_contractor_mapping_temp for conId=" + itm.contractorID + "     MAST_ACCOUNT_NUMBER=" + itm.accountNumber + "     MAST_BANK_NAME=[" + itm.bankName + "]     IFSC=[" + itm.branchName + "]     lgdStateCode=" + lgdStateCode + "     Batchd=[" + itm.batchId + "]", xmlFileName);
                        }
                    }

                    if (pfms_data_send_details_temp != null)
                    {
                        pfms_data_send_details_temp.RECEIVED_FILE_NAME = xmlFileName.Trim().Substring(0, xmlFileName.IndexOf('.'));
                        dbContext.Entry(pfms_data_send_details_temp).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        PFMSLog("BeneficiaryTemp", ConfigurationManager.AppSettings["PFMSBeneficiaryTempLog"].ToString(), "No recorlds in pfms_data_send_details_temp ", xmlFileName);
                    }

                    if (pfms_ommas_contractor_mapping_temp != null || pfms_data_send_details_temp != null)
                    {
                        //dbContext.SaveChanges();
                        PFMSLog("BeneficiaryTemp", ConfigurationManager.AppSettings["PFMSBeneficiaryTempLog"].ToString(), "Beneficiary acknowledgement successful", xmlFileName);
                    }
                    else
                    {
                        PFMSLog("BeneficiaryTemp", ConfigurationManager.AppSettings["PFMSBeneficiaryTempLog"].ToString(), "No Records updated for Beneficiary acknowledgement", xmlFileName);
                    }
                    //ts.Complete();
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                PFMSLog("BeneficiaryTemp", ConfigurationManager.AppSettings["PFMSBeneficiaryTempLog"].ToString(), "Error on Beneficiary acknowledgement", xmlFileName);
                ErrorLog.LogError(ex, "PFMSDAL.EditPFMSContractorDetailsTemp()");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region PFMS Log
        public void PFMSLog(string module, string logPath, string message, string fileName)
        {
            try
            {
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(logPath + "\\PFMS" + module + "Log_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date : " + DateTime.Now.ToString());
                    sw.WriteLine("FileName : " + fileName);
                    sw.WriteLine("status : " + message);
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.PFMSLog");
            }
        }
        #endregion

        #region Deregister DSC for PFMS
        public Array GetDSCDetailsForDeregisterDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                #region
                //var query = (from NOLog in dbContext.ADMIN_NODAL_OFFICERS_SHADOW
                //             join crt in dbContext.ACC_CERTIFICATE_DETAILS_SHADOW on NOLog.ADMIN_NO_OFFICER_CODE equals crt.ADMIN_NO_OFFICER_CODE
                //             select new
                //             {
                //                 NOLog.ADMIN_NO_OFFICER_CODE,
                //                 NOLog.AuditDate,
                //                 OfficerName = NOLog.ADMIN_NO_FNAME + NOLog.ADMIN_NO_MNAME + NOLog.ADMIN_NO_LNAME,
                //                 NOLog.ADMIN_ND_CODE
                //             }).ToList();

                //        //var dscDetails = query.OrderByDescending(x => x.AuditDate)
                //        //                .Where(v => v.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && )
                //        //                .GroupBy(c => c.ADMIN_ND_CODE)
                //        //                .Select(group => new { Group = group, Count = group.Count() })
                //        //                .SelectMany(groupWithCount => groupWithCount.Group.Select(b => b)
                //        //                .Zip(
                //        //                        Enumerable.Range(1, groupWithCount.Count),
                //        //                        (j, i) => new { j.OfficerName, RowNumber = i }
                //        //                    )
                //        //                    .Take
                //        //                );
                #endregion
                var dscDetails = dbContext.PFMS_GET_DSC_TO_DEREGISTER(PMGSYSession.Current.AdminNdCode).ToList();
                totalRecords = dscDetails.Count();

                return dscDetails.Select(lstDSCDetails => new
                {
                    //id = propDetails.IMS_PR_ROAD_CODE.ToString(),
                    cell = new[] {      
                                    //lstcontractorDetails.MAST_CON_ID.ToString(),
                                    lstDSCDetails.ADMIN_ND_NAME.Trim(),
                                    lstDSCDetails.Name.Trim(),
                                    (lstDSCDetails.ADMIN_ACTIVE_START_DATE.ToString()),
                                    lstDSCDetails.ADMIN_NO_MOBILE.ToString().Trim(),
                                    lstDSCDetails.ADMIN_NO_EMAIL.Trim(),
                                    lstDSCDetails.IsRegistered.Trim(),
                                    //lstcontractorDetails.IsRegistered.Trim().ToUpper().ToString() == "REGISTERED" ? "<a href='#' class='ui-icon ui-icon-refresh ui-align-center' onclick='DeRegister(\"" + URLEncrypt.EncryptParameters(new string[]{lstcontractorDetails.ADMIN_ND_CODE.ToString()})+"\");return false;'>Un -Register</a>":"",
                                    lstDSCDetails.IsDeRegistered == "Yes" ? "-" : "<a href='#' class='ui-icon ui-icon-refresh ui-align-center' onclick='DeRegister(\"" + URLEncrypt.EncryptParameters(new string[]{lstDSCDetails.ADMIN_NO_OFFICER_CODE.ToString() + "$" + lstDSCDetails.Name.Trim()})+"\");return false;'>Un -Register</a>"
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                ErrorLog.LogError(ex, "PFMSDAL.GetDSCDetailsForDeregisterDAL()");
                return null;
            }
        }
        #endregion

        #region Insert Epayment Details

        public string InsertHoldingAccountEpaymentMailDetailsPFMS(Int64 bill_ID, String FileName, int mastConId, int conAccountId, int runningCount)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            dbContext = new PMGSYEntities();
            string moduleType = "R";
            try
            {
                using (var scope = new TransactionScope())
                {

                    ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();

                    acc_bill_master = dbContext.ACC_BILL_MASTER.Find(bill_ID);

                    #region Mail Details Entry
                    int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                    //get authorized bank details
                    ACC_BANK_DETAILS bankDetails = new ACC_BANK_DETAILS();

                    if (parentNdVode.HasValue)
                    {
                        bankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.ACCOUNT_TYPE == "S" && f.BANK_ACC_STATUS == true).Any() ? dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.ACCOUNT_TYPE == "S" && f.BANK_ACC_STATUS == true).First() : bankDetails;
                    }

                    //get state /district information
                    MASTER_DISTRICT district = new MASTER_DISTRICT();

                    //Below line Added on 22-11-2021 to get district for DPIU and SRRDA
                    district = PMGSYSession.Current.LevelId == 4 ? null : dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).First();

                    //Below line Added on 22-11-2021 to get state for DPIU and SRRDA

                    int stateCode = PMGSYSession.Current.LevelId == 4 ? PMGSYSession.Current.StateCode : district.MAST_STATE_CODE;

                    string StateName = dbContext.MASTER_STATE.Where(v => v.MAST_STATE_CODE == stateCode).Select(c => c.MAST_STATE_NAME).First();
                    string PiuName = dbContext.ADMIN_DEPARTMENT.Where(v => v.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(c => c.ADMIN_ND_NAME).First();

                    ACC_EPAY_MAIL_MASTER EpayModel = new ACC_EPAY_MAIL_MASTER();

                    long maxEPAY_ID = 0;

                    if (dbContext.ACC_EPAY_MAIL_MASTER.Any())
                    {
                        maxEPAY_ID = dbContext.ACC_EPAY_MAIL_MASTER.Max(c => c.EPAY_ID);
                    }

                    maxEPAY_ID = maxEPAY_ID + 1;

                    EpayModel.EPAY_ID = maxEPAY_ID;

                    EpayModel.BILL_ID = bill_ID;

                    EpayModel.EPAY_NO = acc_bill_master.CHQ_NO;

                    EpayModel.EPAY_MONTH = (byte)acc_bill_master.BILL_MONTH;

                    EpayModel.EPAY_YEAR = acc_bill_master.BILL_YEAR;

                    if (acc_bill_master.CHQ_DATE.HasValue)
                    {
                        EpayModel.EPAY_DATE = acc_bill_master.CHQ_DATE.Value;
                    }
                    else
                    {
                        EpayModel.EPAY_DATE = acc_bill_master.BILL_DATE;
                    }

                    EpayModel.EMAIL_FROM = "omms.pmgsy@nic.in";

                    //Below Code added on 22-03-2023
                    //EpayModel.EMAIL_TO = bankDetails.BANK_EMAIL;
                    ACC_BANK_DETAILS holdAccBankDetails = new ACC_BANK_DETAILS();
                    if (acc_bill_master.TXN_ID == 3171)
                    {
                        holdAccBankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true && f.MAST_STATE_CODE == PMGSYSession.Current.StateCode && f.ACCOUNT_TYPE == "H").FirstOrDefault();
                    }
                    //Below Code added on 22-03-2023
                    if (acc_bill_master.TXN_ID == 3171)
                    {
                        EpayModel.EMAIL_TO = holdAccBankDetails.BANK_EMAIL != null ? holdAccBankDetails.BANK_EMAIL : null;
                    }
                    else
                    {
                        EpayModel.EMAIL_TO = bankDetails.BANK_EMAIL;

                    }

                    EpayModel.EMAIL_SUBJECT = PMGSYSession.Current.LevelId == 4 ? " An Epayment transaction is made by SRRDA of " + StateName + "on https://omms.nic.in,Epayment No: " + acc_bill_master.CHQ_NO : " An Epayment transaction is made by DPIU of " + PiuName + " ( " + district.MAST_DISTRICT_NAME + " ) of " + StateName + "on www.omms.nic.in,Epayment No: " + acc_bill_master.CHQ_NO;

                    //Below Code Added on 22-03-2023
                    //EpayModel.EMAIL_CC = "";

                    //Below Code Added on 22-03-2023
                    if (acc_bill_master.TXN_ID == 3171)
                    {

                        string SRRDAMailId = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode).Select(x => x.ADMIN_ND_EMAIL).FirstOrDefault();
                        if (SRRDAMailId != null && SRRDAMailId != String.Empty)
                        {
                            EpayModel.EMAIL_CC = bankDetails.BANK_EMAIL + "," + SRRDAMailId;

                        }
                        else
                        {
                            EpayModel.EMAIL_CC = bankDetails.BANK_EMAIL;

                        }
                    }
                    else
                    {
                        EpayModel.EMAIL_CC = "";
                    }

                    //EpayModel.EMAIL_CC = dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.MAST_ND_TYPE == "S").Select(m=>m.ADMIN_ND_EMAIL).FirstOrDefault();

                    EpayModel.EMAIL_BCC = "omms.pmgsy@nic.in";

                    EpayModel.EMAIL_SENT_DATE = DateTime.Now;

                    EpayModel.IS_EPAY_VALID = true;

                    EpayModel.REQUEST_IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    EpayModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                    EpayModel.LVL_ID = (byte)PMGSYSession.Current.LevelId;

                    EpayModel.EPAY_EREMITTANCE = "E";

                    EpayModel.DEPT_BANK_ACC_NO = String.Empty;

                    EpayModel.DPIU_TAN_NO = String.Empty;

                    //File Name Added By Abhishek kamble for Resend/Reject mail Details 25 Sep 2014
                    EpayModel.FILE_NAME = FileName;

                    //Added By Abhishek kamble 29-nov-2013
                    EpayModel.USERID = PMGSYSession.Current.UserId;
                    EpayModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_EPAY_MAIL_MASTER.Add(EpayModel);

                    //insert the details into [ACC_EPAY_MAIL_DETAILS]

                    long maxDetail_ID = 0;

                    if (dbContext.ACC_EPAY_MAIL_MASTER.Any())
                    {
                        maxDetail_ID = dbContext.ACC_EPAY_MAIL_DETAILS.Max(c => c.DETAIL_ID);
                    }

                    ACC_EPAY_MAIL_DETAILS EpayDetailsModel = new ACC_EPAY_MAIL_DETAILS();

                    maxDetail_ID = maxDetail_ID + 1;

                    EpayDetailsModel.DETAIL_ID = maxDetail_ID;

                    EpayDetailsModel.EPAY_ID = maxEPAY_ID;

                    EpayDetailsModel.BANK_ACC_NO = bankDetails.BANK_ACC_NO;

                    EpayDetailsModel.BILL_NO = acc_bill_master.BILL_NO;

                    EpayDetailsModel.BILL_DATE = acc_bill_master.BILL_DATE;//EpayDetailsModel.BILL_DATE;//acc_bill_master.BILL_DATE;
                    //EpayDetailsModel.BILL_DATE = DateTime.Now;//EpayDetailsModel.BILL_DATE;//acc_bill_master.BILL_DATE;

                    EpayDetailsModel.AGREEMENT_NO = null;

                    EpayDetailsModel.PKG_NO = null;

                    EpayDetailsModel.CON_NAME = acc_bill_master.PAYEE_NAME;


                    //modified by abhishek kamble
                    //EpayDetailsModel.CON_ACC_NO = con.MAST_ACCOUNT_NUMBER;

                    //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                    //{
                    //    EpayDetailsModel.CON_ACC_NO = (admNoBank == null ? "" : admNoBank.MAST_ACCOUNT_NUMBER);
                    //    EpayDetailsModel.CON_BANK_NAME = (admNoBank == null ? "" : admNoBank.MAST_BANK_NAME);
                    //    EpayDetailsModel.CON_BANK_IFS_CODE = (admNoBank == null ? "" : admNoBank.MAST_IFSC_CODE);
                    //}
                    //else
                    //{
                    EpayDetailsModel.CON_ACC_NO = (holdAccBankDetails == null ? "" : holdAccBankDetails.BANK_ACC_NO);
                    EpayDetailsModel.CON_BANK_NAME = (holdAccBankDetails == null ? "" : holdAccBankDetails.BANK_NAME);
                    EpayDetailsModel.CON_BANK_IFS_CODE = (holdAccBankDetails == null ? "" : holdAccBankDetails.MAST_IFSC_CODE);
                    //}
                    //EpayDetailsModel.EPAY_AMOUNT = masterDetails.GROSS_AMOUNT;

                    //Modified By Abhishek kamble 15-July-2014 Change to Cheque Amount from Gross Amount
                    EpayDetailsModel.EPAY_AMOUNT = acc_bill_master.CHQ_AMOUNT;


                    EpayDetailsModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                    EpayDetailsModel.LVL_ID = (byte)PMGSYSession.Current.LevelId;

                    EpayDetailsModel.CON_PAN_NO = null;

                    //Added By Abhishek kamble 29-nov-2013
                    EpayDetailsModel.USERID = PMGSYSession.Current.UserId;
                    EpayDetailsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_EPAY_MAIL_DETAILS.Add(EpayDetailsModel);

                    //set finalization status as "Y" in bill master

                    if (acc_bill_master.CHQ_EPAY == "E")
                    {
                        //new change done by Vikram on 10-08-2013
                        acc_bill_master.BILL_FINALIZED = "Y";
                        //end of change
                        //acc_bill_master.BILL_FINALIZED = "E";
                    }
                    else
                    {
                        acc_bill_master.BILL_FINALIZED = "Y";
                    }
                    acc_bill_master.ACTION_REQUIRED = "N";

                    //(Flag Modified) Added By Abhishek kamble 29-nov-2013
                    acc_bill_master.USERID = PMGSYSession.Current.UserId;
                    acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;

                    #endregion

                    ///Check for duplicate File Name
                    string FName = FileName.Trim().Contains(".xml") ? FileName.Trim().Substring(0, FileName.Trim().IndexOf('.')) : FileName.Trim();
                    if (moduleType.Equals("D"))
                    {
                        if (dbContext.REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS.Any(z => z.REQ_FILENAME.Trim() == FName.Trim() || z.REQ_FILENAME.Trim() == FName.Trim() + ".xml"))
                        {
                            return "-1";
                        }
                    }

                    if (moduleType.Equals("R"))
                    {
                        if (dbContext.REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS.Any(z => z.REQ_FILENAME.Trim() == FName.Trim() || z.REQ_FILENAME.Trim() == FName.Trim() + ".xml"))
                        {
                            return "-1";
                        }
                    }

                    #region Insert PFMS Details


                    if (moduleType.Equals("D"))
                    {
                        REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS reat_ommas_holding_account_payments = new REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS();
                        reat_ommas_holding_account_payments.HOLDING_ID = dbContext.REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS.Any() ? dbContext.REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS.Max(m => m.HOLDING_ID) + 1 : 1;
                        reat_ommas_holding_account_payments.BILL_ID = bill_ID;
                        reat_ommas_holding_account_payments.BATCH_ID = "CH0037" + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + runningCount.ToString("D4");
                        //   reat_ommas_holding_account_payments.BILL_DATE = acc_bill_master.BILL_DATE;
                        reat_ommas_holding_account_payments.REQ_FILENAME = FileName.Trim();
                        reat_ommas_holding_account_payments.REQ_FILE_GEN_DATE = DateTime.Now;
                        dbContext.REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS.Add(reat_ommas_holding_account_payments);
                    }

                    if (moduleType.Equals("R"))
                    {
                        REAT_DATA_SEND_DETAILS reat_data_send_details = new REAT_DATA_SEND_DETAILS();
                        Int64 maxFileID = dbContext.REAT_DATA_SEND_DETAILS.Any() ? dbContext.REAT_DATA_SEND_DETAILS.Max(m => m.FILE_ID) + 1 : 1;
                        reat_data_send_details.FILE_ID = maxFileID;
                        reat_data_send_details.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                        reat_data_send_details.FUND_TYPE = "P";
                        reat_data_send_details.GENERATED_FILE_NAME = FileName.Trim();
                        reat_data_send_details.FILE_GENERATION_DATE = DateTime.Now;
                        reat_data_send_details.GENERATED_FILE_PATH = FileName.Trim();
                        reat_data_send_details.FILE_TYPE = "H";
                        reat_data_send_details.RESPONSE_RECEIVED_DATE = DateTime.Now;
                        reat_data_send_details.RECEIVED_FILE_NAME = null;
                        reat_data_send_details.ERR_RECEIVED_RESPONSE = null;
                        reat_data_send_details.USER_ID = PMGSYSession.Current.UserId;
                        reat_data_send_details.IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.REAT_DATA_SEND_DETAILS.Add(reat_data_send_details);






                        REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS reat_ommas_holding_account_payments = new REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS();
                        Int64 fileID = maxFileID; //dbContext.REAT_DATA_SEND_DETAILS.Any() ? dbContext.REAT_DATA_SEND_DETAILS.Max(m => m.FILE_ID) + 1 : 1; //dbContext.REAT_DATA_SEND_DETAILS.Where(c => c.GENERATED_FILE_NAME == FileName.Trim() && c.FILE_TYPE  == "P" ).First().FILE_ID;
                        reat_ommas_holding_account_payments.HOLDING_ID = dbContext.REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS.Any() ? dbContext.REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS.Max(m => m.HOLDING_ID) + 1 : 1;
                        reat_ommas_holding_account_payments.BILL_ID = bill_ID;
                        reat_ommas_holding_account_payments.FILE_ID = fileID;
                        reat_ommas_holding_account_payments.BATCH_ID = "H0037" + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString() + runningCount.ToString();
                        //  reat_ommas_holding_account_payments.BILL_DATE = acc_bill_master.BILL_DATE;
                        reat_ommas_holding_account_payments.REQ_FILENAME = FileName.Trim();
                        reat_ommas_holding_account_payments.REQ_FILE_GEN_DATE = DateTime.Now;
                        dbContext.REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS.Add(reat_ommas_holding_account_payments);


                    }

                    #endregion

                    dbContext.SaveChanges();

                    scope.Complete();

                    return "1";

                }
            }
            catch (DbEntityValidationException e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                //ErrorLog.LogError(e, "PFMSDAL.InsertEpaymentMailDetailsPFMS(DbEntityValidationException ex)");
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        //modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                        using (StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                            sw.WriteLine("Method : " + "PFMSDAL1.InsertHoldingAccountEpaymentMailDetailsPFMS(DbEntityValidationException ex)");
                            sw.WriteLine("Exception : " + e.ToString());
                            sw.WriteLine("Exception Message : " + ve.ErrorMessage);
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                //return new CommonFunctions().FormatErrorMessage(modelstate);



                throw new Exception("Error while Finalizing Epayment");
            }
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "PFMSDAL1.InsertHoldingAccountEpaymentMailDetailsPFMS(DbUpdateException ex)");

                throw new Exception("Error while Finalizing Epayment");
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "PFMSDAL1.InsertHoldingAccountEpaymentMailDetailsPFMS(OptimisticConcurrencyException ex)");
                throw new Exception("Error while Finalizing Epayment");
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "PFMSDAL1.InsertHoldingAccountEpaymentMailDetailsPFMS()");
                throw new Exception("Error while Finalizing Epayment");
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public string InsertEpaymentMailDetailsPFMS(Int64 bill_ID, String FileName, int mastConId, int conAccountId, int runningCount)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            dbContext = new PMGSYEntities();


            string moduleType = "R";
            //string moduleType = "D";
            //REAT_OMMAS_PAYMENT_SUCCESS objModuleType = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);
            //if (objModuleType != null)
            //{
            //    moduleType = "R";
            //}

            try
            {
                using (var scope = new TransactionScope())
                {
                    int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                    //get payment master details
                    ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();

                    masterDetails = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).First();

                    //get package details
                    List<Int32> lstBillDetails = new List<Int32>();

                    lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_ID && c.CASH_CHQ == "Q" && c.CREDIT_DEBIT == "D" && c.IMS_PR_ROAD_CODE != null).Select(f => f.IMS_PR_ROAD_CODE.Value).Distinct().ToList<Int32>();

                    String Packages = String.Empty;

                    if (lstBillDetails != null && lstBillDetails.Count() != 0)
                    {
                        foreach (int item in lstBillDetails)
                        {
                            Packages = Packages + "," + dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == item).Select(x => x.IMS_PACKAGE_ID).First();
                        }

                        if (Packages != string.Empty)
                        {
                            if (Packages[0] == ',')
                            {
                                Packages = Packages.Substring(1);
                            }

                        }
                    }
                    //get the agreement details 
                    int? agreementCode = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_ID && c.CREDIT_DEBIT == "D" && c.CASH_CHQ == "Q").First().IMS_AGREEMENT_CODE;

                    // String AgreementNo = dbContext.TEND_AGREEMENT_MASTER.Where(d => d.TEND_AGREEMENT_CODE == agreementCode.Value).Select(t => t.TEND_AGREEMENT_NUMBER).First();

                    //old
                    //String AgreementNo = masterDetails.FUND_TYPE == "M" ? (dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_PR_CONTRACT_CODE == (agreementCode.HasValue ? agreementCode.Value : -1)).Select(x => x.MANE_AGREEMENT_NUMBER).FirstOrDefault()) : (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (agreementCode.HasValue ? agreementCode.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault());
                    //Modified by Abhishek kamble to get Agreement No Using Mane_CONTRACTOR_ID 17Nov2014
                    String AgreementNo = masterDetails.FUND_TYPE == "M" ? (dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_CONTRACT_ID == (agreementCode.HasValue ? agreementCode.Value : -1)).Select(x => x.MANE_AGREEMENT_NUMBER).FirstOrDefault()) : (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (agreementCode.HasValue ? agreementCode.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault());

                    //get contractor details
                    MASTER_CONTRACTOR_BANK con = new MASTER_CONTRACTOR_BANK();
                    //PFMS_OMMAS_CONTRACTOR_MAPPING con = null;
                    ADMIN_NO_BANK admNoBank = new ADMIN_NO_BANK();
                    //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                    //{
                    //    admNoBank = dbContext.ADMIN_NO_BANK.Where(v => v.ADMIN_NO_OFFICER_CODE == masterDetails.ADMIN_NO_OFFICER_CODE && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                    //}
                    //else
                    //{
                        //if (PMGSYSession.Current.FundType == "P")
                        //{
                            int lgdStateCode = dbContext.OMMAS_LDG_STATE_MAPPING.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(x => x.MAST_STATE_LDG_CODE).FirstOrDefault();

                            if (moduleType.Equals("D"))
                            {
                                PFMS_OMMAS_CONTRACTOR_MAPPING con1 = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.PFMS_CON_ID != null && v.MAST_ACCOUNT_ID == conAccountId && v.MAST_LGD_STATE_CODE == lgdStateCode && v.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode && v.STATUS == "A"
                                          && v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Where(z => z.MAST_CON_ID == masterDetails.MAST_CON_ID && z.MAST_ACCOUNT_ID == conAccountId && z.MAST_LOCK_STATUS == "Y").Select(b => b.MAST_ACCOUNT_STATUS).FirstOrDefault() == "A").FirstOrDefault();
                                if (con1 != null)
                                {
                                    con.MAST_ACCOUNT_NUMBER = con1.MAST_ACCOUNT_NUMBER;
                                    con.MAST_ACCOUNT_ID = con1.MAST_ACCOUNT_ID;
                                    con.MAST_IFSC_CODE = con1.MAST_IFSC_CODE;
                                    con.MAST_BANK_NAME = con1.MAST_BANK_NAME;
                                }
                            }

                            if (moduleType.Equals("R"))
                            {
                                REAT_CONTRACTOR_DETAILS con1 = dbContext.REAT_CONTRACTOR_DETAILS.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.REAT_CON_ID != null && v.MAST_ACCOUNT_ID == masterDetails.CON_ACCOUNT_ID && v.reat_STATUS == "A").FirstOrDefault();
                                if (con1 != null)
                                {
                                    con.MAST_ACCOUNT_NUMBER = con1.MAST_ACCOUNT_NUMBER;
                                    con.MAST_ACCOUNT_ID = con1.MAST_ACCOUNT_ID;
                                    con.MAST_IFSC_CODE = con1.MAST_IFSC_CODE;
                                    con.MAST_BANK_NAME = con1.MAST_BANK_NAME;
                                }
                            }
                        //}
                    //}

                    //get authorized bank details
                    ACC_BANK_DETAILS bankDetails = new ACC_BANK_DETAILS();

                    if (parentNdVode.HasValue)
                    {
                        bankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE == "S").First();
                    }

                    //get state /district information
                    MASTER_DISTRICT district = new MASTER_DISTRICT();

                    //Below line Commented on 22-11-2021 to get district for DPIU and SRRDA 
                    //district = dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).First();

                    //Below line Added on 22-11-2021 to get district for DPIU and SRRDA
                    district = PMGSYSession.Current.LevelId == 4 ? null : dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).First();

                    //Below line Commented on 22-11-2021 to get state for DPIU and SRRDA

                    //int stateCode = district.MAST_STATE_CODE;

                    //Below line Added on 22-11-2021 to get state for DPIU and SRRDA

                    int stateCode = PMGSYSession.Current.LevelId == 4 ? PMGSYSession.Current.StateCode : district.MAST_STATE_CODE;

                    string StateName = dbContext.MASTER_STATE.Where(v => v.MAST_STATE_CODE == stateCode).Select(c => c.MAST_STATE_NAME).First();
                    string PiuName = dbContext.ADMIN_DEPARTMENT.Where(v => v.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(c => c.ADMIN_ND_NAME).First();

                    ACC_EPAY_MAIL_MASTER EpayModel = new ACC_EPAY_MAIL_MASTER();

                    long maxEPAY_ID = 0;

                    if (dbContext.ACC_EPAY_MAIL_MASTER.Any())
                    {
                        maxEPAY_ID = dbContext.ACC_EPAY_MAIL_MASTER.Max(c => c.EPAY_ID);
                    }

                    maxEPAY_ID = maxEPAY_ID + 1;

                    EpayModel.EPAY_ID = maxEPAY_ID;

                    EpayModel.BILL_ID = bill_ID;

                    EpayModel.EPAY_NO = masterDetails.CHQ_NO;

                    EpayModel.EPAY_MONTH = (byte)masterDetails.BILL_MONTH;

                    EpayModel.EPAY_YEAR = masterDetails.BILL_YEAR;

                    if (masterDetails.CHQ_DATE.HasValue)
                    {
                        EpayModel.EPAY_DATE = masterDetails.CHQ_DATE.Value;
                    }

                    EpayModel.EMAIL_FROM = "omms.pmgsy@nic.in";

                    EpayModel.EMAIL_TO = bankDetails.BANK_EMAIL;

                    EpayModel.EMAIL_SUBJECT = PMGSYSession.Current.LevelId==4 ? " An Epayment transaction is made by SRRDA of " + StateName + "on https://omms.nic.in,Epayment No: " + masterDetails.CHQ_NO : " An Epayment transaction is made by DPIU of " + PiuName + " ( " + district.MAST_DISTRICT_NAME + " ) of " + StateName + "on www.omms.nic.in,Epayment No: " + masterDetails.CHQ_NO;

                    EpayModel.EMAIL_CC = "";

                    //EpayModel.EMAIL_CC = dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.MAST_ND_TYPE == "S").Select(m=>m.ADMIN_ND_EMAIL).FirstOrDefault();

                    EpayModel.EMAIL_BCC = "omms.pmgsy@nic.in";

                    EpayModel.EMAIL_SENT_DATE = DateTime.Now;

                    EpayModel.IS_EPAY_VALID = true;

                    EpayModel.REQUEST_IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    EpayModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                    EpayModel.LVL_ID = (byte)PMGSYSession.Current.LevelId;

                    EpayModel.EPAY_EREMITTANCE = "E";

                    EpayModel.DEPT_BANK_ACC_NO = String.Empty;

                    EpayModel.DPIU_TAN_NO = String.Empty;

                    //File Name Added By Abhishek kamble for Resend/Reject mail Details 25 Sep 2014
                    EpayModel.FILE_NAME = FileName;

                    //Added By Abhishek kamble 29-nov-2013
                    EpayModel.USERID = PMGSYSession.Current.UserId;
                    EpayModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_EPAY_MAIL_MASTER.Add(EpayModel);

                    //insert the details into [ACC_EPAY_MAIL_DETAILS]

                    long maxDetail_ID = 0;

                    if (dbContext.ACC_EPAY_MAIL_MASTER.Any())
                    {
                        maxDetail_ID = dbContext.ACC_EPAY_MAIL_DETAILS.Max(c => c.DETAIL_ID);
                    }

                    ACC_EPAY_MAIL_DETAILS EpayDetailsModel = new ACC_EPAY_MAIL_DETAILS();

                    maxDetail_ID = maxDetail_ID + 1;

                    EpayDetailsModel.DETAIL_ID = maxDetail_ID;

                    EpayDetailsModel.EPAY_ID = maxEPAY_ID;

                    EpayDetailsModel.BANK_ACC_NO = bankDetails.BANK_ACC_NO;

                    EpayDetailsModel.BILL_NO = masterDetails.BILL_NO;

                    EpayDetailsModel.BILL_DATE = masterDetails.BILL_DATE;

                    EpayDetailsModel.AGREEMENT_NO = AgreementNo;

                    EpayDetailsModel.PKG_NO = Packages;

                    EpayDetailsModel.CON_NAME = masterDetails.PAYEE_NAME;


                    //modified by abhishek kamble
                    //EpayDetailsModel.CON_ACC_NO = con.MAST_ACCOUNT_NUMBER;

                    //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                    //{
                    //    EpayDetailsModel.CON_ACC_NO = (admNoBank == null ? "" : admNoBank.MAST_ACCOUNT_NUMBER);
                    //    EpayDetailsModel.CON_BANK_NAME = (admNoBank == null ? "" : admNoBank.MAST_BANK_NAME);
                    //    EpayDetailsModel.CON_BANK_IFS_CODE = (admNoBank == null ? "" : admNoBank.MAST_IFSC_CODE);
                    //}
                    //else
                    //{
                        EpayDetailsModel.CON_ACC_NO = (con == null ? "" : con.MAST_ACCOUNT_NUMBER);
                        EpayDetailsModel.CON_BANK_NAME = (con == null ? "" : con.MAST_BANK_NAME);
                        EpayDetailsModel.CON_BANK_IFS_CODE = (con == null ? "" : con.MAST_IFSC_CODE);
                    //}
                    //EpayDetailsModel.EPAY_AMOUNT = masterDetails.GROSS_AMOUNT;

                    //Modified By Abhishek kamble 15-July-2014 Change to Cheque Amount from Gross Amount
                    EpayDetailsModel.EPAY_AMOUNT = masterDetails.CHQ_AMOUNT;


                    EpayDetailsModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                    EpayDetailsModel.LVL_ID = (byte)PMGSYSession.Current.LevelId;

                    EpayDetailsModel.CON_PAN_NO = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == masterDetails.MAST_CON_ID).Select(d => d.MAST_CON_PAN).FirstOrDefault();

                    //Added By Abhishek kamble 29-nov-2013
                    EpayDetailsModel.USERID = PMGSYSession.Current.UserId;
                    EpayDetailsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_EPAY_MAIL_DETAILS.Add(EpayDetailsModel);

                    //set finalization status as "Y" in bill master
                    ACC_BILL_MASTER acc_bill_master = dbContext.ACC_BILL_MASTER.Find(bill_ID);

                    if (acc_bill_master.CHQ_EPAY == "E")
                    {
                        //new change done by Vikram on 10-08-2013
                        acc_bill_master.BILL_FINALIZED = "Y";
                        //end of change
                        //acc_bill_master.BILL_FINALIZED = "E";
                    }
                    else
                    {
                        acc_bill_master.BILL_FINALIZED = "Y";
                    }
                    acc_bill_master.ACTION_REQUIRED = "N";

                    //(Flag Modified) Added By Abhishek kamble 29-nov-2013
                    acc_bill_master.USERID = PMGSYSession.Current.UserId;
                    acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;

                    ///Check for duplicate File Name
                    string FName = FileName.Trim().Contains(".xml") ? FileName.Trim().Substring(0, FileName.Trim().IndexOf('.')) : FileName.Trim();
                    if (moduleType.Equals("D"))
                    {
                        if (dbContext.PFMS_OMMAS_PAYMENT_MAPPING.Any(z => z.PAYMENT_REQ_FILENAME.Trim() == FName.Trim() || z.PAYMENT_REQ_FILENAME.Trim() == FName.Trim() + ".xml"))
                        {
                            return "-1";
                        }
                    }

                    if (moduleType.Equals("R"))
                    {
                        if (dbContext.REAT_OMMAS_PAYMENT_MAPPING.Any(z => z.PAYMENT_REQ_FILENAME.Trim() == FName.Trim() || z.PAYMENT_REQ_FILENAME.Trim() == FName.Trim() + ".xml"))
                        {
                            return "-1";
                        }
                    }


                    #region Insert PFMS Details
                    //PFMS_DATA_SEND_DETAILS pfms_data_send_details = new PFMS_DATA_SEND_DETAILS();

                    //pfms_data_send_details.ID = dbContext.PFMS_DATA_SEND_DETAILS.Any() ? dbContext.PFMS_DATA_SEND_DETAILS.Max(m => m.ID) + 1 : 1;
                    //pfms_data_send_details.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    //pfms_data_send_details.FUND_TYPE = "P";
                    //pfms_data_send_details.BILL_GENERATION_DATE = acc_bill_master.BILL_DATE;
                    //pfms_data_send_details.DATA_SEND_DATE = DateTime.Now;
                    //pfms_data_send_details.DATA_TYPE = "P";
                    //pfms_data_send_details.RESPONSE_RECEIVED_DATE = null;
                    //pfms_data_send_details.RECEIVED_RESPONSE = null;
                    //pfms_data_send_details.GENERATED_FILE_NAME = FileName.Trim();
                    //pfms_data_send_details.RECEIVED_FILE_NAME = null;

                    //dbContext.PFMS_DATA_SEND_DETAILS.Add(pfms_data_send_details);

                    if (moduleType.Equals("D"))
                    {




                        PFMS_OMMAS_PAYMENT_MAPPING pfms_ommas_payment_mapping = new PFMS_OMMAS_PAYMENT_MAPPING();

                        pfms_ommas_payment_mapping.BILL_ID = bill_ID;
                        pfms_ommas_payment_mapping.BATCH_ID = "CP0037" + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + runningCount.ToString("D4");
                        pfms_ommas_payment_mapping.BILL_DATE = acc_bill_master.BILL_DATE;
                        pfms_ommas_payment_mapping.PAYMENT_REQ_FILENAME = FileName.Trim();
                        pfms_ommas_payment_mapping.PAYMENT_REQ_FILE_GEN_DATE = DateTime.Now;

                        dbContext.PFMS_OMMAS_PAYMENT_MAPPING.Add(pfms_ommas_payment_mapping);


                    }

                    if (moduleType.Equals("R"))
                    {


                        //  runningCount= dbContext.REAT_DATA_SEND_DETAILS.Any() ? dbContext.REAT_DATA_SEND_DETAILS.Count(m => m.FILE_GENERATION_DATE.Year  == DateTime.Now.Year && m.FILE_GENERATION_DATE.Month == DateTime.Now.Month && m.FILE_GENERATION_DATE.Day == DateTime.Now.Day  && m.FILE_TYPE == "P" ) + 1 : 1;


                        //     FileName = " 0037EATPAYREQ" + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString() + runningCount.ToString() + ".xml";

                        REAT_DATA_SEND_DETAILS reat_data_send_details = new REAT_DATA_SEND_DETAILS();

                        Int64 maxFileID = dbContext.REAT_DATA_SEND_DETAILS.Any() ? dbContext.REAT_DATA_SEND_DETAILS.Max(m => m.FILE_ID) + 1 : 1;
                        reat_data_send_details.FILE_ID = maxFileID;
                        reat_data_send_details.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                        reat_data_send_details.FUND_TYPE = "P";
                        reat_data_send_details.GENERATED_FILE_NAME = FileName.Trim();
                        reat_data_send_details.FILE_GENERATION_DATE = DateTime.Now;
                        reat_data_send_details.GENERATED_FILE_PATH = FileName.Trim();

                        reat_data_send_details.FILE_TYPE = "P";
                        reat_data_send_details.RESPONSE_RECEIVED_DATE = null;
                        reat_data_send_details.RECEIVED_FILE_NAME = null;

                        reat_data_send_details.ERR_RECEIVED_RESPONSE = null;

                        reat_data_send_details.USER_ID = PMGSYSession.Current.UserId;
                        reat_data_send_details.IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.REAT_DATA_SEND_DETAILS.Add(reat_data_send_details);


                        REAT_OMMAS_PAYMENT_MAPPING reat_ommas_payment_mapping = new REAT_OMMAS_PAYMENT_MAPPING();
                        Int64 fileID = maxFileID; //dbContext.REAT_DATA_SEND_DETAILS.Any() ? dbContext.REAT_DATA_SEND_DETAILS.Max(m => m.FILE_ID) + 1 : 1; //dbContext.REAT_DATA_SEND_DETAILS.Where(c => c.GENERATED_FILE_NAME == FileName.Trim() && c.FILE_TYPE  == "P" ).First().FILE_ID;


                        reat_ommas_payment_mapping.BILL_ID = bill_ID;
                        reat_ommas_payment_mapping.FILE_ID = fileID;
                        reat_ommas_payment_mapping.BATCH_ID = "P0037" + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString() + runningCount.ToString();
                        reat_ommas_payment_mapping.BILL_DATE = acc_bill_master.BILL_DATE;
                        reat_ommas_payment_mapping.PAYMENT_REQ_FILENAME = FileName.Trim();
                        reat_ommas_payment_mapping.PAYMENT_REQ_FILE_GEN_DATE = DateTime.Now;

                        dbContext.REAT_OMMAS_PAYMENT_MAPPING.Add(reat_ommas_payment_mapping);


                    }

                    #endregion

                    dbContext.SaveChanges();

                    scope.Complete();

                    return "1";

                }
            }
            catch (DbEntityValidationException e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                //ErrorLog.LogError(e, "PFMSDAL.InsertEpaymentMailDetailsPFMS(DbEntityValidationException ex)");
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        //modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                        using (StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                            sw.WriteLine("Method : " + "PFMSDAL.InsertEpaymentMailDetailsPFMS(DbEntityValidationException ex)");
                            sw.WriteLine("Exception : " + e.ToString());
                            sw.WriteLine("Exception Message : " + ve.ErrorMessage);
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                //return new CommonFunctions().FormatErrorMessage(modelstate);



                throw new Exception("Error while Finalizing Epayment");
            }
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "PFMSDAL.InsertEpaymentMailDetailsPFMS(DbUpdateException ex)");

                throw new Exception("Error while Finalizing Epayment");
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "PFMSDAL.InsertEpaymentMailDetailsPFMS(OptimisticConcurrencyException ex)");
                throw new Exception("Error while Finalizing Epayment");
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "PFMSDAL.InsertEpaymentMailDetailsPFMS()");
                throw new Exception("Error while Finalizing Epayment");
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        #endregion

        //added by abhinav pathak on 08-12-2018
        #region Deactivate/Activate Contractor Details
        /// <summary>
        /// returns the list of contractor details
        /// </summary>
        public Array GetContractorsListDAL(int stateCode, string panNumber, string accountStatus, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                var code = (from sc in dbContext.OMMAS_LDG_STATE_MAPPING
                            where sc.MAST_STATE_CODE == (stateCode == 0 ? sc.MAST_STATE_CODE : stateCode)
                            select sc.MAST_STATE_LDG_CODE).ToList();

                int lgdStateCode = Convert.ToInt32(code.ElementAt(0));

                var statename = (from item in dbContext.MASTER_STATE
                                 where item.MAST_STATE_CODE == stateCode
                                 select item.MAST_STATE_NAME).ToList();

                var result = (from item in dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING
                              join con in dbContext.MASTER_CONTRACTOR on item.MAST_CON_ID equals con.MAST_CON_ID
                              join dist in dbContext.OMMAS_LDG_DISTRICT_MAPPING on item.MAST_LGD_DISTRICT_CODE equals dist.MAST_DISTRICT_LDG_CODE
                              join mastBank in dbContext.MASTER_CONTRACTOR_BANK on new { item.MAST_CON_ID, item.MAST_ACCOUNT_ID } equals new { mastBank.MAST_CON_ID, mastBank.MAST_ACCOUNT_ID }
                              join agencyName in dbContext.ADMIN_DEPARTMENT on item.MAST_AGENCY_CODE equals agencyName.MAST_AGENCY_CODE
                              where item.MAST_LGD_STATE_CODE == lgdStateCode//(stateCode == 0 ? item.MAST_LGD_STATE_CODE : lgdStateCode) 
                              && con.MAST_CON_PAN == panNumber
                              && item.STATUS == (accountStatus == "A" ? accountStatus : "R") //"A" for default
                              && item.PFMS_CON_ID != null
                              && (accountStatus == "A" ? mastBank.MAST_ACCOUNT_STATUS == "A" : true) && (accountStatus == "A" ? mastBank.MAST_LOCK_STATUS == "Y" : true)
                              && agencyName.MAST_ND_TYPE == "S"
                              && agencyName.MAST_STATE_CODE == stateCode //(stateCode == 0 ? agencyName.MAST_STATE_CODE : stateCode)
                                  //Agency filter for ITNO login 16JUL2019
                              && item.MAST_AGENCY_CODE == (PMGSYSession.Current.MastAgencyCode > 0 ? PMGSYSession.Current.MastAgencyCode : item.MAST_AGENCY_CODE)
                              && mastBank.MAST_ACCOUNT_STATUS == "A" && mastBank.MAST_LOCK_STATUS == "Y"
                              select new
                              {
                                  item.POCM_ID,
                                  item.MAST_CON_ID,
                                  con.MAST_CON_FNAME,
                                  con.MAST_CON_MNAME,
                                  con.MAST_CON_LNAME,
                                  dist.MAST_DISTRICT_CODE,
                                  item.MAST_LGD_DISTRICT_CODE,
                                  dist.MASTER_DISTRICT.MAST_DISTRICT_NAME,
                                  item.MAST_CON_COMPANY_NAME,
                                  item.MAST_BANK_NAME,
                                  item.MAST_IFSC_CODE,
                                  item.MAST_ACCOUNT_NUMBER,
                                  item.MAST_ACCOUNT_ID,
                                  item.MAST_AGENCY_CODE,
                                  agencyName.ADMIN_ND_NAME
                              }).ToList().Distinct();
                //result = result.GroupBy(x => x.POCM_ID).ToList();

                var contractor = result.Select(conDetails => new
                {
                    conDetails.POCM_ID,
                    conDetails.MAST_CON_ID,
                    conDetails.MAST_ACCOUNT_ID,
                    conDetails.MAST_DISTRICT_CODE,
                    conDetails.MAST_LGD_DISTRICT_CODE,
                    conDetails.MAST_DISTRICT_NAME,
                    conDetails.MAST_CON_FNAME,
                    conDetails.MAST_CON_MNAME,
                    conDetails.MAST_CON_LNAME,
                    conDetails.MAST_BANK_NAME,
                    conDetails.MAST_IFSC_CODE,
                    conDetails.MAST_ACCOUNT_NUMBER,
                    conDetails.MAST_AGENCY_CODE,
                    conDetails.ADMIN_ND_NAME
                }).ToArray();


                return contractor.Select(contractorDetails => new
                {
                    contractorid = contractorDetails.MAST_CON_ID,
                    accountid = Convert.ToString(contractorDetails.MAST_ACCOUNT_ID),
                    cell = new[]
                {
                 
                    Convert.ToString(contractorDetails.POCM_ID),
                    Convert.ToString(contractorDetails.MAST_CON_ID),
                    Convert.ToString(contractorDetails.MAST_ACCOUNT_ID),
                    Convert.ToString(contractorDetails.MAST_LGD_DISTRICT_CODE),
                    Convert.ToString(contractorDetails.MAST_AGENCY_CODE),
                    Convert.ToString(contractorDetails.MAST_CON_FNAME)+" "+Convert.ToString(contractorDetails.MAST_CON_MNAME)+" "+Convert.ToString(contractorDetails.MAST_CON_LNAME)+" "+"("+contractorDetails.MAST_CON_ID+")",
                    Convert.ToString(statename.ElementAt(0)),
                    Convert.ToString(contractorDetails.MAST_DISTRICT_NAME),
                    Convert.ToString(contractorDetails.ADMIN_ND_NAME),
                    Convert.ToString(contractorDetails.MAST_BANK_NAME),
                    Convert.ToString(contractorDetails.MAST_IFSC_CODE),
                    Convert.ToString(contractorDetails.MAST_ACCOUNT_NUMBER),

                }
                }).ToArray();
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.GetContractorsListDAL()");
                message = "An Error Occured while processing the Request";
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public bool DeactivateContactorDetailsDAL(int pocmid)
        {
            try
            {
                dbContext = new PMGSYEntities();
                var contractorInfo = (from item in dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING
                                      where item.POCM_ID == pocmid
                                      select item).FirstOrDefault();

                if (contractorInfo.STATUS == "A")
                {
                    contractorInfo.STATUS = "R";
                    dbContext.SaveChanges();
                    return true;
                }
                if (contractorInfo.STATUS == "R")
                {
                    contractorInfo.STATUS = "A";
                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL1.DeactivateContactorDetailsDAL()");
                return false;
            }

            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region Update Beneficiary IFSC
        public Array GetBeneficiaryDetailsForUpdateDAL(string panNo, int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            string date = string.Empty;
            int agencyCount = 0, stateCount = 0;
            try
            {
                dbContext = new Models.PMGSYEntities();

                var agencies = (dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(x => x.MASTER_CONTRACTOR.MAST_CON_PAN == panNo.Trim()).Select(c => c.MAST_AGENCY_CODE).ToList().Distinct());

                var contractorDetails = (from con in dbContext.MASTER_CONTRACTOR
                                         join bank in dbContext.MASTER_CONTRACTOR_BANK on con.MAST_CON_ID equals bank.MAST_CON_ID
                                         join pfms in dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING on new { con.MAST_CON_ID, bank.MAST_ACCOUNT_ID } equals new { pfms.MAST_CON_ID, pfms.MAST_ACCOUNT_ID }
                                         join reg in dbContext.MASTER_CONTRACTOR_REGISTRATION on con.MAST_CON_ID equals reg.MAST_CON_ID
                                         join lgdState in dbContext.OMMAS_LDG_STATE_MAPPING on pfms.MAST_LGD_STATE_CODE equals lgdState.MAST_STATE_LDG_CODE
                                         where
                                         bank.MAST_ACCOUNT_STATUS == "A"
                                         && (bank.MAST_IFSC_CODE).Length > 10
                                         && !string.IsNullOrEmpty(bank.MAST_IFSC_CODE)
                                         && bank.MAST_IFSC_CODE != null
                                         && bank.MAST_LOCK_STATUS == "Y"
                                         && pfms.STATUS == "A"
                                         && pfms.PFMS_CON_ID != null
                                             //&& pfms.MAST_IFSC_CODE.Trim() == bank.MAST_IFSC_CODE.Trim()

                                         &&
                                         (
                                            (!(dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(x => x.MAST_CON_ID == con.MAST_CON_ID && x.MAST_ACCOUNT_ID == pfms.MAST_ACCOUNT_ID && x.PFMS_CON_ID == null && (x.STATUS == null || x.STATUS.Trim() == "R")
                                             && x.MAST_LGD_STATE_CODE == lgdState.MAST_STATE_LDG_CODE && x.MAST_IFSC_CODE == bank.MAST_IFSC_CODE
                                             && (x.MAST_AGENCY_CODE == pfms.MAST_AGENCY_CODE /*agencies.Contains(x.MAST_AGENCY_CODE) */ || x.MAST_AGENCY_CODE == null)))
                                             && pfms.MAST_IFSC_CODE.Trim() != bank.MAST_IFSC_CODE.Trim()
                                             )
                                             ||
                                             (pfms.MAST_IFSC_CODE.Trim() == bank.MAST_IFSC_CODE.Trim())
                                         )

                                         //&& !(dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(x => x.MAST_CON_ID == con.MAST_CON_ID && x.MAST_ACCOUNT_ID == pfms.MAST_ACCOUNT_ID && x.STATUS == "R"
                                             //    && x.MAST_LGD_STATE_CODE == lgdState.MAST_STATE_LDG_CODE && x.MAST_IFSC_CODE == pfms.MAST_IFSC_CODE && (x.MAST_AGENCY_CODE == pfms.MAST_AGENCY_CODE || x.MAST_AGENCY_CODE == null)))

                                         && con.MAST_CON_PAN == panNo.Trim()
                                         select new
                                         {
                                             pfms.POCM_ID,
                                             con.MAST_CON_ID,
                                             con.MAST_CON_PAN,
                                             bank.MAST_ACCOUNT_ID,
                                             //bank.MASTER_DISTRICT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE,
                                             lgdState.MAST_STATE_LDG_CODE,
                                             lgdState.MASTER_STATE.MAST_STATE_NAME,
                                             bank.MASTER_DISTRICT.OMMAS_LDG_DISTRICT_MAPPING.MAST_DISTRICT_CODE,
                                             con.MAST_CON_COMPANY_NAME,
                                             Bank_Ifsc_Code = bank.MAST_IFSC_CODE,
                                             Pfms_Ifsc_Code = pfms.MAST_IFSC_CODE,
                                             bank.MAST_ACCOUNT_NUMBER,
                                             con.MAST_CON_FNAME,
                                             con.MAST_CON_MNAME,
                                             con.MAST_CON_LNAME,
                                             BANK_NAME = bank.MAST_BANK_NAME,
                                             pfms.MAST_AGENCY_CODE,

                                             status = (pfms.STATUS == "A" && pfms.PFMS_CON_ID != null) ? "Accepted" : (pfms.STATUS == "R") ? "Rejected" : "Processing at PFMS",
                                             agency = (dbContext.ADMIN_DEPARTMENT.Where(c => c.MAST_STATE_CODE == lgdState.MAST_STATE_CODE && c.MAST_ND_TYPE == "S" && c.MAST_AGENCY_CODE == pfms.MAST_AGENCY_CODE).Select(m => m.ADMIN_ND_NAME).FirstOrDefault())
                                         }).Distinct().ToList();
                ///For contractor accepted multiple times
                //if (contractorDetails.GroupBy(m => new { m.MAST_CON_ID, m.MAST_CON_PAN, m.MAST_ACCOUNT_ID, m.MAST_STATE_LDG_CODE, m.MAST_AGENCY_CODE, m.MAST_DISTRICT_CODE, m.MAST_CON_COMPANY_NAME, m.MAST_IFSC_CODE, m.MAST_ACCOUNT_NUMBER, m.BANK_NAME, }).Count() == 1)
                //{
                //    contractorDetails = contractorDetails.Take(1).ToList();
                //}

                //var accpConDetails = contractorDetails.Select(b => b.MAST_CON_ID).ToList();
                //totalRecords = contractorDetails.Count();

                //agencyCount = contractorDetails.GroupBy(x => new { x.MAST_CON_ID, x.agency }).Count();
                //stateCount = contractorDetails.GroupBy(x => new { x.MAST_CON_ID, x.MAST_STATE_LDG_CODE }).Count();

                #region In Progress Records
                //var contractorDetails2 = (from con in dbContext.MASTER_CONTRACTOR
                //                          join bank in dbContext.MASTER_CONTRACTOR_BANK on con.MAST_CON_ID equals bank.MAST_CON_ID
                //                          join pfms in dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING on new { con.MAST_CON_ID, bank.MAST_ACCOUNT_ID } equals new { pfms.MAST_CON_ID, pfms.MAST_ACCOUNT_ID }
                //                          join reg in dbContext.MASTER_CONTRACTOR_REGISTRATION on con.MAST_CON_ID equals reg.MAST_CON_ID
                //                          join lgdState in dbContext.OMMAS_LDG_STATE_MAPPING on pfms.MAST_LGD_STATE_CODE equals lgdState.MAST_STATE_LDG_CODE
                //                          where
                //                          bank.MAST_ACCOUNT_STATUS == "A"
                //                          && (bank.MAST_IFSC_CODE).Length > 10
                //                          && !string.IsNullOrEmpty(bank.MAST_IFSC_CODE)
                //                          && bank.MAST_IFSC_CODE != null
                //                          && bank.MAST_LOCK_STATUS == "Y"
                //                          && pfms.MAST_IFSC_CODE.Trim() == bank.MAST_IFSC_CODE.Trim()

                //                          //&& pfms.STATUS == "A"
                //                              //&& pfms.PFMS_CON_ID != null
                //                              //&& !(dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(x => x.MAST_CON_ID == con.MAST_CON_ID && x.MAST_ACCOUNT_ID == pfms.MAST_ACCOUNT_ID && x.PFMS_CON_ID == null && x.STATUS == null
                //                              //    && x.MAST_LGD_STATE_CODE == lgdState.MAST_STATE_LDG_CODE && x.MAST_IFSC_CODE == pfms.MAST_IFSC_CODE && (x.MAST_AGENCY_CODE == pfms.MAST_AGENCY_CODE || x.MAST_AGENCY_CODE == null)))

                //                          && !(dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(x => x.MAST_CON_ID == con.MAST_CON_ID && x.MAST_ACCOUNT_ID == pfms.MAST_ACCOUNT_ID && (x.STATUS == "R" /*|| x.STATUS == "A"*/)
                //                                  && x.MAST_LGD_STATE_CODE == lgdState.MAST_STATE_LDG_CODE && x.MAST_IFSC_CODE == pfms.MAST_IFSC_CODE && (x.MAST_AGENCY_CODE == pfms.MAST_AGENCY_CODE || x.MAST_AGENCY_CODE == null)))

                //                          && con.MAST_CON_PAN == panNo.Trim()
                //                          //&& !(accpConDetails.Contains(con.MAST_CON_ID))
                //                          select new
                //                          {
                //                              pfms.POCM_ID,
                //                              con.MAST_CON_ID,
                //                              con.MAST_CON_PAN,
                //                              bank.MAST_ACCOUNT_ID,
                //                              //bank.MASTER_DISTRICT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE,
                //                              lgdState.MAST_STATE_LDG_CODE,
                //                              lgdState.MASTER_STATE.MAST_STATE_NAME,
                //                              bank.MASTER_DISTRICT.OMMAS_LDG_DISTRICT_MAPPING.MAST_DISTRICT_CODE,
                //                              con.MAST_CON_COMPANY_NAME,
                //                              bank.MAST_IFSC_CODE,
                //                              bank.MAST_ACCOUNT_NUMBER,
                //                              con.MAST_CON_FNAME,
                //                              con.MAST_CON_MNAME,
                //                              con.MAST_CON_LNAME,
                //                              BANK_NAME = bank.MAST_BANK_NAME,
                //                              pfms.MAST_AGENCY_CODE,

                //                              status = (pfms.STATUS == "A" && pfms.PFMS_CON_ID != null) ? "Accepted" : (pfms.STATUS == "R") ? "Rejected" : "Processing at PFMS",
                //                              agency = (dbContext.ADMIN_DEPARTMENT.Where(c => c.MAST_STATE_CODE == lgdState.MAST_STATE_CODE && c.MAST_ND_TYPE == "S" && c.MAST_AGENCY_CODE == pfms.MAST_AGENCY_CODE).Select(m => m.ADMIN_ND_NAME).FirstOrDefault())
                //                          }).Distinct().ToList();

                #endregion

                #region reference
                /*
                 var accConDetails = contractorDetails2.Where(a => a.status.Trim() == "Accepted").Select(b => b.MAST_CON_ID).ToList();
                 var progConDetails = contractorDetails2.Where(a => a.status.Trim() == "Processing at PFMS").Select(b => b.MAST_CON_ID).ToList();
                 
                var result = contractorDetails.Select(conDetails => new
                {
                    conDetails.POCM_ID,
                    conDetails.MAST_CON_FNAME,
                    conDetails.MAST_CON_MNAME,
                    conDetails.MAST_CON_LNAME,
                    conDetails.MAST_STATE_NAME,
                    conDetails.MAST_CON_PAN,
                    conDetails.MAST_CON_ID,
                    conDetails.MAST_CON_COMPANY_NAME,
                    conDetails.BANK_NAME,
                    conDetails.MAST_ACCOUNT_ID,
                    conDetails.MAST_IFSC_CODE,
                    conDetails.MAST_ACCOUNT_NUMBER,
                    conDetails.status,
                    conDetails.agency,
                    //flag = checkDate(conDetails.BATCH_ID, out date),
                    //date
                    edit = ((agencyCount > 1) || (stateCount > 1) 
                            //|| (accConDetails.Contains(conDetails.MAST_CON_ID) && progConDetails.Contains(conDetails.MAST_CON_ID)) && !(contractorDetails2.Select(c=>c.MAST_CON_ID).Contains(conDetails.MAST_CON_ID))
                            ) ? "-" : "<a href='#' title='Click here to edit Physical Road Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditRoadProgressDetails('" + conDetails.POCM_ID.ToString().Trim() + "'); return false;'>Edit Progress</a>",
                }).ToList();

                agencyCount = contractorDetails2.GroupBy(x => new { x.MAST_CON_ID, x.agency }).Count();
                stateCount = contractorDetails2.GroupBy(x => new { x.MAST_CON_ID, x.MAST_STATE_LDG_CODE }).Count();


                var result2 = contractorDetails2.Select(conDetails => new
                {
                    conDetails.POCM_ID,
                    conDetails.MAST_CON_FNAME,
                    conDetails.MAST_CON_MNAME,
                    conDetails.MAST_CON_LNAME,
                    conDetails.MAST_STATE_NAME,
                    conDetails.MAST_CON_PAN,
                    conDetails.MAST_CON_ID,
                    conDetails.MAST_CON_COMPANY_NAME,
                    conDetails.BANK_NAME,
                    conDetails.MAST_ACCOUNT_ID,
                    conDetails.MAST_IFSC_CODE,
                    conDetails.MAST_ACCOUNT_NUMBER,
                    conDetails.status,
                    conDetails.agency,
                    //flag = checkDate(conDetails.BATCH_ID, out date),
                    //date
                    edit = ((agencyCount > 1) || (stateCount > 1) || contractorDetails.Select(a => a.MAST_CON_ID).Contains(conDetails.MAST_CON_ID)) ? "-" : "<a href='#' title='Click here to edit Physical Road Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditRoadProgressDetails('" + conDetails.POCM_ID.ToString().Trim() + "'); return false;'>Edit Progress</a>",
                }).ToList();

                result = result.Union(result2).ToList();

                result = result.GroupBy(x => new { x.POCM_ID, x.MAST_CON_FNAME, x.MAST_CON_MNAME, x.MAST_CON_LNAME, x.MAST_STATE_NAME, x.MAST_CON_PAN, x.MAST_CON_ID, x.MAST_CON_COMPANY_NAME, x.BANK_NAME, x.MAST_ACCOUNT_ID, x.MAST_IFSC_CODE, x.MAST_ACCOUNT_NUMBER, x.status, x.agency }).Select(g=>g.First()).ToList();
                */
                #endregion

                //contractorDetails = contractorDetails.Union(contractorDetails2).GroupBy(x => new { x.POCM_ID, x.MAST_CON_FNAME, x.MAST_CON_MNAME, x.MAST_CON_LNAME, x.MAST_STATE_NAME, x.MAST_CON_PAN, x.MAST_CON_ID, x.MAST_CON_COMPANY_NAME, x.BANK_NAME, x.MAST_ACCOUNT_ID, x.MAST_IFSC_CODE, x.MAST_ACCOUNT_NUMBER, x.status, x.agency }).Select(g => g.First()).ToList();

                agencyCount = contractorDetails.GroupBy(x => new { x.MAST_CON_ID, x.agency }).Count();
                stateCount = contractorDetails.GroupBy(x => new { x.MAST_CON_ID, x.MAST_STATE_LDG_CODE }).Count();

                var result = contractorDetails.Select(conDetails => new
                {
                    conDetails.POCM_ID,
                    conDetails.MAST_CON_FNAME,
                    conDetails.MAST_CON_MNAME,
                    conDetails.MAST_CON_LNAME,
                    conDetails.MAST_STATE_NAME,
                    conDetails.MAST_CON_PAN,
                    conDetails.MAST_CON_ID,
                    conDetails.MAST_CON_COMPANY_NAME,
                    conDetails.BANK_NAME,
                    conDetails.MAST_ACCOUNT_ID,
                    conDetails.Bank_Ifsc_Code,
                    conDetails.Pfms_Ifsc_Code,
                    conDetails.MAST_ACCOUNT_NUMBER,
                    conDetails.status,
                    conDetails.agency,
                    //edit = ((contractorDetails.Where(x => x.MAST_CON_ID == conDetails.MAST_CON_ID).GroupBy(x => new { x.MAST_CON_ID, x.agency, x.MAST_STATE_LDG_CODE }).Count() == 1) 
                    //        //|| (contractorDetails.Where(x => x.MAST_CON_ID == conDetails.MAST_CON_ID).GroupBy(x => new { x.MAST_CON_ID, x.MAST_STATE_LDG_CODE }).Count() > 1)
                    //    ///(agencyCount > 1) || (stateCount > 1)
                    //    //|| (accConDetails.Contains(conDetails.MAST_CON_ID) && progConDetails.Contains(conDetails.MAST_CON_ID)) && !(contractorDetails2.Select(c=>c.MAST_CON_ID).Contains(conDetails.MAST_CON_ID))
                    //        ) ? "-" : "<a href='#' title='Click here to edit Physical Road Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditRoadProgressDetails('" + conDetails.POCM_ID.ToString().Trim() + "'); return false;'>Edit Progress</a>",
                    edit = "<a href='#' title='Click here to edit Physical Road Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditRoadProgressDetails('" + conDetails.POCM_ID.ToString().Trim() + "'); return false;'>Edit Progress</a>"
                }).ToList();

                totalRecords = result.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                result = result.OrderBy(x => x.MAST_CON_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                result = result.OrderByDescending(x => x.MAST_CON_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    result = result.OrderByDescending(x => x.MAST_CON_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return result.Select(lstcontractorDetails => new
                {
                    id = lstcontractorDetails.POCM_ID.ToString(),
                    cell = new[] {      

                                    lstcontractorDetails.MAST_CON_ID.ToString(),
                                    Convert.ToString(lstcontractorDetails.MAST_CON_FNAME).Trim() + " " + Convert.ToString(lstcontractorDetails.MAST_CON_MNAME) + " " + Convert.ToString(lstcontractorDetails.MAST_CON_LNAME) + " (" + lstcontractorDetails.MAST_CON_ID.ToString() + ")",
                                    
                                    lstcontractorDetails.MAST_STATE_NAME,
                                    lstcontractorDetails.agency,                                    
                                    
                                    string.IsNullOrEmpty(lstcontractorDetails.MAST_CON_PAN) ? "-" : lstcontractorDetails.MAST_CON_PAN,
                                    lstcontractorDetails.MAST_CON_COMPANY_NAME,
                                    lstcontractorDetails.BANK_NAME.Trim().Replace("\n", "").Replace("\r", ""),
                                    lstcontractorDetails.MAST_ACCOUNT_ID.ToString(),
                                    lstcontractorDetails.Bank_Ifsc_Code,
                                    lstcontractorDetails.Pfms_Ifsc_Code,
                                    lstcontractorDetails.MAST_ACCOUNT_NUMBER.ToString(),
                                    lstcontractorDetails.status.Trim(),
                                    lstcontractorDetails.edit.Trim(),
                                    "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave' title='Click here to Save the File Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SaveRoadProgressDetails('" + lstcontractorDetails.POCM_ID.ToString().Trim() +"');></a><a href='#' style='float:right' id='btnCancel' title='Click here to Cancel the File Edit' class='ui-icon ui-icon-closethick ui-align-                                                    center' onClick= CancelRoadProgressDetails('" +  lstcontractorDetails.POCM_ID.ToString().Trim() + "');></a></td></tr></table></center>",
                    }
                }).ToArray();
                //return contractorDetails.ToArray();
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                ErrorLog.LogError(ex, "PFMSDAL.GetBeneficiaryDetailsForUpdateDAL()");
                return null;
            }
        }


        public PFMSDownloadXMLViewModel GetAccountDetailsDAL(int pocmId)
        {
            PFMSDownloadXMLViewModel model = null;
            dbContext = new PMGSYEntities();
            try
            {
                var conDetails = (from pfms in dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING
                                  join ldgState in dbContext.OMMAS_LDG_STATE_MAPPING on pfms.MAST_LGD_STATE_CODE equals ldgState.MAST_STATE_LDG_CODE
                                  join ldgDistrict in dbContext.OMMAS_LDG_DISTRICT_MAPPING on pfms.MAST_LGD_DISTRICT_CODE equals ldgDistrict.MAST_DISTRICT_LDG_CODE
                                  where pfms.POCM_ID == pocmId
                                  select new
                                  {
                                      pfms.MAST_CON_ID,
                                      pfms.MAST_ACCOUNT_ID,
                                      pfms.MAST_ACCOUNT_NUMBER,
                                      pfms.MAST_AGENCY_CODE,
                                      ldgState.MAST_STATE_CODE,
                                      ldgDistrict.MAST_DISTRICT_CODE
                                  }).FirstOrDefault();
                if (conDetails != null)
                {
                    model = new PFMSDownloadXMLViewModel();
                    model.stateCode = conDetails.MAST_STATE_CODE;
                    model.districtCode = conDetails.MAST_DISTRICT_CODE;
                    model.agencyCode = conDetails.MAST_AGENCY_CODE.Value;
                    model.mastContractorIds = new string[1];
                    model.mastContractorIds[0] = conDetails.MAST_CON_ID + "$" + conDetails.MAST_ACCOUNT_ID;
                }
                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetAccountDetailsDAL");
                return null;
            }
        }

        public bool UpdateIFSCforBeneficiaryDAL(int conId, int accountId, string ifsc, ref string message)
        {
            dbContext = new PMGSYEntities();
            try
            {
                MASTER_CONTRACTOR_BANK master_contractor_bank = dbContext.MASTER_CONTRACTOR_BANK.Where(x => x.MAST_CON_ID == conId && x.MAST_ACCOUNT_ID == accountId).FirstOrDefault();

                if (master_contractor_bank.MAST_IFSC_CODE.Trim() == ifsc.Trim())
                {
                    //message = "Please enter new IFSC to update";
                    //return false;
                    return true;
                }

                master_contractor_bank.MAST_IFSC_CODE = ifsc.Trim();
                master_contractor_bank.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                master_contractor_bank.USERID = PMGSYSession.Current.UserId;

                dbContext.Entry(master_contractor_bank).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                ErrorLog.LogError(ex, "PFMS1DAL.UpdateIFSCforBeneficiaryDAL");
                return false;
            }
        }

        public string GenerateXMLUpdateDAL(PFMSDownloadXMLViewModel model, out string xmlFName, out int recCount)
        {
            string xmlString = string.Empty;
            string xmlHeader = string.Empty;
            string xmlBody = string.Empty;
            string xmlFileName = string.Empty;
            int recordCount = 0;

            var outParam = new System.Data.Entity.Core.Objects.ObjectParameter("XmlFileName", xmlFileName);
            var outPrmCount = new System.Data.Entity.Core.Objects.ObjectParameter("RecordCount", recordCount);
            #region SqlParameter (for reference)
            //var outParam = new SqlParameter();
            //outParam.ParameterName = "TotalRows";
            //outParam.SqlDbType = SqlDbType.Int;
            //outParam.Direction = ParameterDirection.Output;
            #endregion
            try
            {
                dbContext = new PMGSYEntities();
                #region Call SP (for reference)
                //Object[] parameters = { model.stateCode, model.agencyCode };
                //var h = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_Header_XML", parameters).ToList();
                //var b = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_XML", parameters).ToList();
                #endregion

                //Create object of data table.                             
                DataTable ContractorIds = new DataTable();
                //Create Column
                ContractorIds.Columns.Add("MAST_CON_ID", typeof(int));
                ContractorIds.Columns.Add("MAST_ACCOUNT_ID", typeof(int));
                if (model.mastContractorIds != null)
                {
                    foreach (string conId in model.mastContractorIds)
                    {
                        if (model.operation == "A")///Changes for Update
                        {
                            if (conId.Split('$').Length > 2)
                            {
                                if (conId.Split('$')[2].ToUpper() == "TRUE")
                                {
                                    ContractorIds.Rows.Add(new object[] { Convert.ToInt32(conId.Split('$')[0]), Convert.ToInt32(conId.Split('$')[1]) });
                                }
                            }
                        }
                        else if (model.operation == "U")///Changes for Update
                        {
                            if (conId.Split('$').Length == 2)
                            {
                                ContractorIds.Rows.Add(new object[] { Convert.ToInt32(conId.Split('$')[0]), Convert.ToInt32(conId.Split('$')[1]) });
                            }
                        }
                    }
                }

                Object[] parameters = { model.stateCode, model.agencyCode, model.districtCode, outParam, outPrmCount, ContractorIds };
                Object[] bparameters = { model.stateCode, model.agencyCode, model.districtCode, ContractorIds };

                var levelParam = new SqlParameter("@Level", SqlDbType.Int);
                levelParam.Value = model.Level;

                var stateParam = new SqlParameter("@stateCode", SqlDbType.Int);
                stateParam.Value = model.stateCode;

                var agencyParam = new SqlParameter("@agencyCode", SqlDbType.Int);
                agencyParam.Value = model.agencyCode;

                var distParam = new SqlParameter("@DistrictC", SqlDbType.Int);
                distParam.Value = model.districtCode;

                var xmlParam = new SqlParameter
                {
                    ParameterName = "XmlFileName",
                    DbType = System.Data.DbType.String,
                    Size = 30,
                    Direction = System.Data.ParameterDirection.Output
                };

                var recParam = new SqlParameter("@RecordCount", SqlDbType.Int);
                recParam.Value = 0;
                recParam.Direction = ParameterDirection.Output;

                var conParam = new SqlParameter("@Contractors", SqlDbType.Structured);
                conParam.Value = ContractorIds;
                conParam.TypeName = "omms.ContractorList";

                ///Changes for Update
                var oprParam = new SqlParameter("@Operation", SqlDbType.Char);
                oprParam.Value = model.operation;

                var levelParam1 = new SqlParameter("@Level", SqlDbType.Int);
                levelParam1.Value = model.Level;

                var stateParam1 = new SqlParameter("@stateCode", SqlDbType.Int);
                stateParam1.Value = model.stateCode;

                var agencyParam1 = new SqlParameter("@agencyCode", SqlDbType.Int);
                agencyParam1.Value = model.agencyCode;

                var distParam1 = new SqlParameter("@DistrictC", SqlDbType.Int);
                distParam1.Value = model.districtCode;

                var conParam1 = new SqlParameter("@Contractors", SqlDbType.Structured);
                conParam1.Value = ContractorIds;
                conParam1.TypeName = "omms.ContractorList";

                ///Changes for Update
                var oprParam1 = new SqlParameter("@Operation", SqlDbType.Char);
                oprParam1.Value = model.operation;

                var hdr = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_Header_XML_U @Level,@stateCode,@agencyCode,@DistrictC,@XmlFileName out,@RecordCount out,@Contractors,@Operation", levelParam, stateParam, agencyParam, distParam, xmlParam, recParam, conParam, oprParam).ToList();
                var bdy = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_XML_U @Level,@stateCode,@agencyCode,@DistrictC,@Contractors,@Operation", levelParam1, stateParam1, agencyParam1, distParam1, conParam1, oprParam1).ToList();

                xmlFName = Convert.ToString(xmlParam.Value);
                recCount = Convert.ToInt32(recParam.Value);

                ///Changes for Update
                //if(model.operation == "A")
                if (recCount == 0)
                {
                    return "";
                }

                xmlHeader = string.Join("", hdr);
                xmlBody = string.Join("", bdy);

                xmlString = xmlHeader.Trim() + xmlBody.Trim();

                XmlSerializer XmlS = new XmlSerializer(xmlString.GetType());

                StringWriter sw = new StringWriterUtf8();
                XmlTextWriter tw = new XmlTextWriter(sw);
                tw.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");

                XmlS.Serialize(tw, xmlString);

                xmlString = sw.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("<DbtBeneficiaries>", "").Replace("</DbtBeneficiaries>", "").Replace("<string>", @"<DbtBeneficiaries xmlns=""http://cpsms.nic.in/BeneficiaryDataRequest""><CstmrDtls>").Replace("</string>", "</CstmrDtls></DbtBeneficiaries>");


                #region Insert into PFMS_DATA_SEND_DETAILS commented
                //dbContext = new PMGSYEntities();

                //PFMS_DATA_SEND_DETAILS pfms_data_send_details = new PFMS_DATA_SEND_DETAILS();
                //pfms_data_send_details.ID = !(dbContext.PFMS_DATA_SEND_DETAILS.Any()) ? 1 : (dbContext.PFMS_DATA_SEND_DETAILS.Max(x => x.ID) + 1);
                //pfms_data_send_details.ADMIN_ND_CODE = dbContext.ADMIN_DEPARTMENT.Where(x => x.MAST_STATE_CODE == model.stateCode && x.MAST_AGENCY_CODE == model.agencyCode && x.MAST_ND_TYPE == "S").Select(x => x.ADMIN_ND_CODE).FirstOrDefault();
                //pfms_data_send_details.FUND_TYPE = null;
                //pfms_data_send_details.DATA_SEND_DATE = DateTime.Now;
                //pfms_data_send_details.DATA_TYPE = "C";
                //pfms_data_send_details.RESPONSE_RECEIVED_DATE = null;
                //pfms_data_send_details.RECEIVED_RESPONSE = null;
                //pfms_data_send_details.GENERATED_FILE_NAME = xmlFName.Trim();
                //pfms_data_send_details.RECEIVED_FILE_NAME = null;

                /////Changes for Update
                //pfms_data_send_details.OP_TYPE = model.operation;

                //dbContext.PFMS_DATA_SEND_DETAILS.Add(pfms_data_send_details);
                //dbContext.SaveChanges();
                #endregion

                return xmlString;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.GenerateXMLUpdateDAL()");
                xmlFName = string.Empty;
                recCount = -1;
                return string.Empty;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion
    }
}
