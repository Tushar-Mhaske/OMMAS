using PMGSY.BAL.DigSign;
using PMGSY.BAL.Payment;
using PMGSY.Common;
using PMGSY.DAL.PFMS;
using PMGSY.Extensions;
using PMGSY.Models.DigSign;
using PMGSY.Models.PFMS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;



namespace PMGSY.Controllers
{
    public class PFMS1Controller : Controller
    {
        //
        // GET: /PFMS1/
        PFMSDAL1 objDAL = null;
        public ActionResult PFMSPaymentLayout()
        {
            PFMSDownloadPaymentXMLViewModel model = new PFMSDownloadPaymentXMLViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.lstState = new List<SelectListItem>();
                    model.lstState.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName.Trim(), Value = Convert.ToString(PMGSYSession.Current.StateCode).Trim() });
                    model.lstAgency = comm.PopulateAgencies(PMGSYSession.Current.StateCode, false);
                }
                else
                {
                    model.lstState = comm.PopulateStates(true);
                    model.lstAgency = new List<SelectListItem>();
                    model.lstAgency.Insert(0, new SelectListItem { Text = "Select Agency", Value = "-1" });
                }
                model.FileType = "N"; //new file
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.PFMSPaymentLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult PopulateAgencybyStateCode(int stateCode)
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                List<SelectListItem> list = objCommonFunctions.PopulateAgencies(stateCode, false);
                //list.Find(x => x.Value == "-1").Value = "0";
                return Json(list);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.PopulateAgencybyStateCode()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult PopulateDistrictsbyStateCode(int stateCode)
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                //int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                List<SelectListItem> lstDist = new List<SelectListItem>();
                lstDist = objCommonFunctions.PopulateDistrict(stateCode, true);
                lstDist.RemoveAt(0);
                lstDist.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
                return Json(lstDist);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        [HttpGet]
        public ActionResult GeneratePaymentXML(string param)
        {
            string xml = string.Empty;
            PFMSDownloadPaymentXMLViewModel model;
            string[] arrParams = null;
            try
            {
                arrParams = param.Split('$');
                model = new PFMSDownloadPaymentXMLViewModel();

                model.stateCode = Convert.ToInt32(arrParams[0]);
                model.agencyCode = Convert.ToInt32(arrParams[1]);
                model.generationDate = arrParams[2];
                model.FileType = arrParams[3];



                if (ModelState.IsValid)
                {
                    objDAL = new PFMSDAL1();
                    string fileName = string.Empty;
                    int runningCnt = 0;
                    xml = objDAL.GeneratePaymentXMLDAL(model, out fileName, out runningCnt);

                    if (xml == "0") //already generated
                    {
                        return Json(new { success = false, message = "Payment request file is already generated for selected state and agency for date: " + model.generationDate }, JsonRequestBehavior.AllowGet);
                    }
                    if (xml == "1") //Reject file already generated
                    {
                        return Json(new { success = false, message = "Payment request file(Rejection Type) is already generated for selected state and agency for date: " + model.generationDate }, JsonRequestBehavior.AllowGet);
                    }
                    byte[] bytes = Encoding.ASCII.GetBytes(xml);

                    if (!string.IsNullOrEmpty(xml))
                    {
                        Response.Clear();
                        var cd = new System.Net.Mime.ContentDisposition
                        {

                            FileName = fileName + ".xml",

                            // always prompt the user for downloading, set to true if you want 
                            // the browser to try to show the file inline
                            Inline = false,
                        };
                        Response.AppendHeader("Content-Disposition", cd.ToString());
                        return File(bytes, "application/octet-stream");
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error in PFMS Payment XML generation" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Error in PFMS Payment XML generation" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GenerateXML");
                return null;
            }
        }

        #region Payment Acknowledge
        [HttpGet]
        public ViewResult GetPaymentXMLValueLayout()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GetPaymentXMLValueLayout()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SavePaymentAcknowledgementXmlData()
        {
            CommonFunctions comm = new CommonFunctions();
            string filePath = string.Empty;
            //int i = 0;
            HttpPostedFileBase file = null;
            bool status = false;
            try
            {
                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "Please select a xml file" });
                }

                filePath = ConfigurationManager.AppSettings["PFMS_VALIDATE_XML"];
                objDAL = new PFMSDAL1();

                //if (!(objDAL.IsValidXMLFile(filePath, Request)))
                //{
                //    return Json(new { success = false, message = "Please select a valid xml file" });
                //}

                #region CPSMSID Code for Reference
                //XmlNodeList elemList = doc.GetElementsByTagName("CPSMSID");
                //for (int i = 0; i < elemList.Count; i++)
                //{
                //    if (i == 0)
                //    {
                //        arrXML = new string[elemList.Count];
                //    }
                //    arrXML[i] = elemList[i].InnerXml;
                //    //Console.WriteLine(elemList[i].InnerXml);
                //}

                /*
                ContractorMapping model = null;
                List<ContractorMapping> lstmodel = new List<ContractorMapping>();
                XmlNodeList nodes = doc.GetElementsByTagName("Cstmr");
                foreach (XmlNode node in nodes)
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.Name == "PrTry")
                        {
                            foreach (XmlNode child1 in child.ChildNodes)
                            {
                                if (child1.Name == "Id")
                                {
                                    model = new ContractorMapping();
                                    model.contractorID = string.IsNullOrEmpty(child1.InnerXml) ? 0 : Convert.ToInt32(child1.InnerXml);
                                    break;
                                }
                            }
                        }
                        if (child.Name == "CPSMSID")
                        {
                            model.cpsmsID = string.IsNullOrEmpty(child.InnerXml) ? 0 : Convert.ToInt32(child.InnerXml);
                            //break;
                        }
                        //if (child.Name == "Nm")
                        //{
                        //    model.contractorName = child.InnerXml;
                        //}
                        if (child.Name == "PstlAdr")
                        {
                            foreach (XmlNode child1 in child.ChildNodes)
                            {
                                if (child1.Name == "Prtry")
                                {
                                    foreach (XmlNode child2 in child1.ChildNodes)
                                    {
                                        if (child2.Name == "DstCd")
                                        {
                                            model.lgdDistrictCode = string.IsNullOrEmpty(child2.InnerXml) ? 0 : Convert.ToInt32(child2.InnerXml);
                                        }
                                        if (child2.Name == "PrvcCd")
                                        {
                                            model.lgdStateCode = string.IsNullOrEmpty(child2.InnerXml) ? 0 : Convert.ToInt32(child2.InnerXml);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (child.Name == "CstmrAcct")
                        {
                            foreach (XmlNode child1 in child.ChildNodes)
                            {
                                if (child1.Name == "CstmrAgt")
                                {
                                    foreach (XmlNode child2 in child1.ChildNodes)
                                    {
                                        if (child2.Name == "FinInstnId")
                                        {
                                            foreach (XmlNode child3 in child2.ChildNodes)
                                            {
                                                if (child3.Name == "BICFI")
                                                {
                                                    model.bankName = child3.InnerXml;
                                                }
                                                if (child3.Name == "BrnchId")
                                                {
                                                    model.branchName = child3.InnerXml;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (child1.Name == "AcctId")
                                {
                                    foreach (XmlNode child2 in child1.ChildNodes)
                                    {
                                        if (child2.Name == "Othr")
                                        {
                                            foreach (XmlNode child3 in child2.ChildNodes)
                                            {
                                                if (child3.Name == "BBAN")
                                                {
                                                    model.accountNumber = child3.InnerXml;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    lstmodel.Add(model);
                    i++;
                }
                 * */
                #endregion

                #region Single File
                //file = Request.Files[0];

                //XElement doc = XElement.Load(file.InputStream);
                //XNamespace ns = doc.GetDefaultNamespace();//"http://cpsms.nic.in/Beneficiary/ContractorPaymentAck";

                //if (objDAL.EditPFMSPaymentDetails(doc, file.FileName))
                //{
                //    return Json(new { success = true, message = "Payment data mapped successfully" });
                //}
                //else
                //{
                //    return Json(new { success = false, message = "Error in PFMS mapping payment data" });
                //}
                #endregion

                #region Multiple File
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        file = Request.Files[i];

                        XElement doc = XElement.Load(file.InputStream);
                        XNamespace ns = doc.GetDefaultNamespace();//"http://cpsms.nic.in/Beneficiary/ContractorPaymentAck";

                        status = objDAL.EditPFMSPaymentDetails(doc, file.FileName);
                        if (status == false)
                        {
                            break;
                        }
                    }
                    if (status == false)
                    {
                        return Json(new { success = false, message = "Error in PFMS mapping payment data for file : " + file.FileName });
                    }
                    return Json(new { success = true, message = "Payment data mapped successfully" });
                }
                return Json(new { success = true, message = "Payment data mapped successfully" });
                #endregion
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.SavePaymentAcknowledgementXmlData()");
                return Json(new { success = false, message = "Error occured while mapping payment data" });
            }
        }
        #endregion

        #region Bank Acknowledge
        [HttpGet]
        public ViewResult GetBankPaymentAckLayout()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GetPaymentXMLValueLayout()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveBankAcknowledgementXmlData()
        {
            CommonFunctions comm = new CommonFunctions();
            string filePath = string.Empty;
            bool isRecordExists = false;
            //int i = 0;
            HttpPostedFileBase file = null;
            bool status = false;
            try
            {
                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "Please select a xml file" });
                }

                filePath = ConfigurationManager.AppSettings["PFMS_VALIDATE_XML"];
                objDAL = new PFMSDAL1();

                //if (!(objDAL.IsValidXMLFile(filePath, Request)))
                //{
                //    return Json(new { success = false, message = "Please select a valid xml file" });
                //}

                #region Single File
                //file = Request.Files[0];

                //XElement doc = XElement.Load(file.InputStream);
                //XNamespace ns = doc.GetDefaultNamespace(); //http://cpsms.nic.in/Beneficiary/ContractorPaymentStatus

                //if (objDAL.EditPFMSBankAcknowlegemntDetails(doc, file.FileName)) //original 
                ////if (objDAL.SaveDscEnrollmentAcknowlegement(doc, file.FileName, out isRecordExists)) //tesing of new function dsc acknowlegement file
                //{
                //    return Json(new { success = true, message = "Bank acknowledgement data mapped successfully" });
                //}
                //else
                //{
                //    if (isRecordExists)
                //    {
                //        return Json(new { success = false, message = "Bank acknowledgement data already mapped" });
                //    }
                //    else
                //    {
                //        return Json(new { success = false, message = "Error in PFMS mapping bank acknowledgement data" });
                //    }
                //}
                #endregion

                #region Multiple File
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        file = Request.Files[i];

                        XElement doc = XElement.Load(file.InputStream);
                        XNamespace ns = doc.GetDefaultNamespace(); //http://cpsms.nic.in/Beneficiary/ContractorPaymentStatus

                        status = objDAL.EditPFMSBankAcknowlegemntDetails(doc, file.FileName); //original 
                        if (status == false)
                        {
                            break;
                        }
                    }
                    if (status == false)
                    {
                        return Json(new { success = false, message = "Error in PFMS mapping bank acknowledgement data for file : " + file.FileName });
                    }
                }
                return Json(new { success = true, message = "Bank acknowledgement data mapped successfully" });
                #endregion
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.SaveBankAcknowledgementXmlData()");
                return Json(new { success = false, message = "Error occured while mapping bank acknowledgement data" });
            }
        }
        #endregion

        #region DSC XML Sign
        [HttpPost]
        [Audit]
        public ActionResult SignEpaymentDSCXml()
        {
            try
            {
                PFMSDAL1 pfmsdal = new PFMSDAL1();
                DSCPFMSModel model = null;

                if (Request.Params["operation"] == "D")
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { Request.Params["officerCode"].Split('/')[0], Request.Params["officerCode"].Split('/')[1], Request.Params["officerCode"].Split('/')[2] });

                    model = pfmsdal.ValidateDscPFMSDetailsforDelete(Convert.ToInt32(urlParams[0].Split('$')[0]));
                    model.operation = Request.Params["operation"];
                    model.adminNdName = urlParams[0].Split('$')[1].Trim();
                }
                else
                {
                    model = pfmsdal.ValidateDscPFMSDetails();
                    model.operation = "A";
                }

                return View("SignEpaymentDSCXml", model);
            }
            catch (Exception ex)
            {
                return Json(string.Empty);
            }
        }

        [HttpPost]
        public ActionResult GetPdf()
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
            {
                sw.WriteLine(" in GetpDF :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                sw.Close();
            }


            Int64 bill_ID;
            DigSignBAL objBAL = new DigSignBAL();
            try
            {
                RegistrationData model = new RegistrationData();
                model.PdfKey = Request.Params["pdfKey"];
                model.CertificateBase64 = string.IsNullOrEmpty(Request.Params["certificate"]) ? string.Empty : Request.Params["certificate"].Trim();
                model.CertificateChainBase64 = string.IsNullOrEmpty(Request.Params["certificateChain"]) ? string.Empty : Request.Params["certificateChain"].Trim();

                if (string.IsNullOrEmpty(model.CertificateBase64) || string.IsNullOrEmpty(model.CertificateChainBase64))
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("Certificate details from client to server are null or empty." + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                        sw.Close();
                    }
                    PMGSYSession.Current.AppletErrMessage = "Certificate details from client to server are null or empty.";
                    return Json(new { message = "Certificate details from client to server are null or empty." });
                }

                RegisterDSCModel regModel = new RegisterDSCModel();
                regModel = objBAL.GetDetailsToRegisterDSC();

                //verify certificate equality with Authorized Signatory's registered Certificate
                string verificationOfRegCertResult = objBAL.VerifyRegisteredCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);

                if (!verificationOfRegCertResult.Equals(string.Empty))
                {

                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("verificationOfRegCertResult is NULL or empty" + DateTime.Now.ToString() + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                        sw.Close();
                    }
                    PMGSYSession.Current.AppletErrMessage = verificationOfRegCertResult;
                    return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }

                // verify Certificate, if suucessful, then only store it in DB else flag error message
                string verificationResult = objBAL.VerifyCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NameAsPerCertificate);

                if (verificationResult.Equals(string.Empty))
                {

                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("verificationResult success " + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                        sw.Close();
                    }

                    Int32 AdminNdCode = Convert.ToInt32(model.PdfKey.Split('$')[0]);
                    string fileName = string.Empty;
                    string Path = string.Empty;
                    String xmlString = String.Empty;

                    string strFlag = "1";
                    string passwords = string.Empty;



                    if (strFlag.Equals("1"))
                    {
                        //Path = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;

                        PFMSDAL1 pfmsDal = new PFMSDAL1();

                        xmlString = pfmsDal.GenerateDSCXml(AdminNdCode, out fileName, "A", regModel.NameAsPerCertificate);
                        TempData["FileName"] = fileName;
                        //Path = @"C:\AuthPdfFile\newpayrequest.xml";
                    }
                    else
                    {
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                        {
                            sw.WriteLine("verificationResult is  NULL or empty" + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                            sw.Close();
                        }

                        return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                            JsonRequestBehavior.AllowGet);
                    }
                    //if (System.IO.File.Exists(Path))
                    if (!String.IsNullOrEmpty(xmlString))
                    {
                        //var fileByteArr = System.IO.File.ReadAllBytes(Path);
                        var fileByteArr = System.Text.Encoding.ASCII.GetBytes(xmlString);
                        string fileBase64Str = Convert.ToBase64String(fileByteArr);

                        passwords = "abc";
                        string encParam = PMGSY.Common.Cryptography.AESEncrypt(passwords);
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                        {
                            sw.WriteLine("encParam" + encParam + "  " + DateTime.Now.ToString() + " UserName -" +
                           PMGSYSession.Current.UserName);
                            sw.Close();
                        }

                        return Json(new
                        {
                            status = "success",
                            password = encParam,
                            message = string.Empty,
                            fileBase64String = fileBase64Str,
                            encHashOfBase64String =
                                string.Empty
                        }, JsonRequestBehavior.AllowGet);

                        // return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        //JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        PMGSYSession.Current.AppletErrMessage = verificationResult;
                        return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                            JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    PMGSYSession.Current.AppletErrMessage = verificationResult;
                    return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + "Get Pdf()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Json(new { status = "error", message = "Error occurred while accessing pdf file.", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                    JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult GetPdfforDelete()
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
            {
                sw.WriteLine(" in GetpDF :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                sw.Close();
            }


            Int64 bill_ID;
            DigSignBAL objBAL = new DigSignBAL();
            try
            {
                RegistrationData model = new RegistrationData();
                model.PdfKey = Request.Params["pdfKey"];
                model.CertificateBase64 = string.IsNullOrEmpty(Request.Params["certificate"]) ? string.Empty : Request.Params["certificate"].Trim();
                model.CertificateChainBase64 = string.IsNullOrEmpty(Request.Params["certificateChain"]) ? string.Empty : Request.Params["certificateChain"].Trim();

                if (string.IsNullOrEmpty(model.CertificateBase64) || string.IsNullOrEmpty(model.CertificateChainBase64))
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("Certificate details from client to server are null or empty." + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                        sw.Close();
                    }
                    PMGSYSession.Current.AppletErrMessage = "Certificate details from client to server are null or empty.";
                    return Json(new { message = "Certificate details from client to server are null or empty." });
                }

                RegisterDSCModel regModel = new RegisterDSCModel();
                regModel = objBAL.GetDetailsToRegisterDSC();

                regModel.NameAsPerCertificate = model.PdfKey.Split('$')[2];

                //verify certificate equality with Authorized Signatory's registered Certificate
                string verificationOfRegActiveCertResult = objBAL.VerifyRegisteredActiveCertificateforDelete(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);

                if (!verificationOfRegActiveCertResult.Equals(string.Empty))
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("verificationOfRegActiveCertResult is NULL or empty" + DateTime.Now.ToString() + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                        sw.Close();
                    }
                    PMGSYSession.Current.AppletErrMessage = verificationOfRegActiveCertResult;
                    return Json(new { status = "error", message = verificationOfRegActiveCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }

                //verify certificate equality with Authorized Signatory's registered Certificate
                string verificationOfRegCertResult = objBAL.VerifyRegisteredCertificateforDelete(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);

                if (!verificationOfRegCertResult.Equals(string.Empty))
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("verificationOfRegCertResult is NULL or empty" + DateTime.Now.ToString() + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                        sw.Close();
                    }
                    PMGSYSession.Current.AppletErrMessage = verificationOfRegCertResult;
                    return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }

                // verify Certificate, if suucessful, then only store it in DB else flag error message
                string verificationResult = objBAL.VerifyCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NameAsPerCertificate);

                if (verificationResult.Equals(string.Empty))
                {

                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("verificationResult success " + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                        sw.Close();
                    }

                    Int32 AdminNdCode = Convert.ToInt32(model.PdfKey.Split('$')[0]);
                    string fileName = string.Empty;
                    string Path = string.Empty;
                    String xmlString = String.Empty;

                    string strFlag = "1";
                    string passwords = string.Empty;



                    if (strFlag.Equals("1"))
                    {
                        //Path = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;

                        PFMSDAL1 pfmsDal = new PFMSDAL1();

                        xmlString = pfmsDal.GenerateDSCXml(AdminNdCode, out fileName, model.PdfKey.Split('$')[1], regModel.NameAsPerCertificate);
                        TempData["FileName"] = fileName;
                        //Path = @"C:\AuthPdfFile\newpayrequest.xml";
                    }
                    else
                    {
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                        {
                            sw.WriteLine("verificationResult is  NULL or empty" + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                            sw.Close();
                        }
                        PMGSYSession.Current.AppletErrMessage = verificationResult;
                        return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                            JsonRequestBehavior.AllowGet);
                    }
                    //if (System.IO.File.Exists(Path))
                    if (!String.IsNullOrEmpty(xmlString))
                    {
                        //var fileByteArr = System.IO.File.ReadAllBytes(Path);
                        var fileByteArr = System.Text.Encoding.ASCII.GetBytes(xmlString);
                        string fileBase64Str = Convert.ToBase64String(fileByteArr);

                        passwords = "abc";
                        string encParam = PMGSY.Common.Cryptography.AESEncrypt(passwords);
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                        {
                            sw.WriteLine("encParam" + encParam + "  " + DateTime.Now.ToString() + " UserName -" +
                           PMGSYSession.Current.UserName);
                            sw.Close();
                        }

                        return Json(new
                        {
                            status = "success",
                            password = encParam,
                            message = string.Empty,
                            fileBase64String = fileBase64Str,
                            encHashOfBase64String =
                                string.Empty
                        }, JsonRequestBehavior.AllowGet);

                        // return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        //JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        PMGSYSession.Current.AppletErrMessage = verificationResult;
                        return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                            JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    PMGSYSession.Current.AppletErrMessage = verificationResult;
                    return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + "GetPdfforDelete()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Json(new { status = "error", message = "Error occurred while accessing pdf file.", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                    JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult SavePdf()
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
            {
                sw.WriteLine(" in SavePdf :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                sw.Close();
            }


            DigSignBAL objBAL = new DigSignBAL();
            RegistrationData model = new RegistrationData();
            model.PdfKey = Request.Params["pdfKey"];

            PFMSDAL1 pfmsdal = new PFMSDAL1();
            // string fileName = pfmsdal.GetDSCFileName(Convert.ToInt32(model.PdfKey));//"NewFile.xml";
            Boolean isXmlFlagSet = false;
            if (model.PdfKey.Split('$')[1] == "A")
            {
                isXmlFlagSet = pfmsdal.ValidXmlGenerateSetFlag();
            }
            else
            {
                isXmlFlagSet = true;
            }
            try
            {
                // <add key="XmlFilePath" value="C:\AuthXmlFile\" />
                String XmlFilePath = ConfigurationManager.AppSettings["PFMSDSCFilePath"].ToString();
                if (!Directory.Exists(XmlFilePath))
                    Directory.CreateDirectory(XmlFilePath);


                #region commented
                //if (!Directory.Exists(XmlFilePath))
                //{
                //    Directory.CreateDirectory(XmlFilePath);
                //}

                //String ShortStateCode = XmlFilePath + "" + pfmsdal.GetStateshortCode();

                //if (!Directory.Exists(ShortStateCode))
                //{
                //    Directory.CreateDirectory(ShortStateCode);
                //}

                //String CurrentFinYer = "";
                //if (DateTime.Now.Month < 3)
                //{
                //    CurrentFinYer = DateTime.Now.Year - 1 + "-" + DateTime.Now.Year;
                //}
                //else
                //{
                //    CurrentFinYer = DateTime.Now.Year + "-" + (DateTime.Now.Year + 1);
                //}

                //String FinYear = ShortStateCode + "\\" + CurrentFinYer;

                //if (!Directory.Exists(FinYear))
                //{
                //    Directory.CreateDirectory(FinYear);
                //}

                //Request.Files[0].SaveAs(Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName));
                #endregion
                string fileName1 = (String)TempData["FileName"];
                Request.Files[0].SaveAs(Path.Combine(XmlFilePath, fileName1));
                if (isXmlFlagSet)
                {
                    return Json(new { status = "success", message = "Document signed successfully." });
                }
                else
                {
                    System.IO.File.Delete(Path.Combine(XmlFilePath, fileName1));
                    return Json(new { status = "error", message = "Error occured while processing your request." });
                }
            }
            catch (Exception ex)
            {

                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + "SavePdf()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Json(new { message = "Error occurred while saving signed Document." });
            }
        }
        #endregion

        #region DSC Acknowledgement
        [HttpGet]
        public ViewResult GetDSCAckLayout()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GetDSCAckLayout()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveDSCAcknowledgementXmlData()
        {
            CommonFunctions comm = new CommonFunctions();
            string filePath = string.Empty;
            bool isRecordExists = false;
            //int i = 0;
            HttpPostedFileBase file = null;
            bool status = false;
            try
            {
                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "Please select a xml file" });
                }

                filePath = ConfigurationManager.AppSettings["PFMS_VALIDATE_XML"];
                objDAL = new PFMSDAL1();

                //if (!(objDAL.IsValidXMLFile(filePath, Request)))
                //{
                //    return Json(new { success = false, message = "Please select a valid xml file" });
                //}
                #region Single File
                //HttpPostedFileBase file = Request.Files[0];

                //XElement doc = XElement.Load(file.InputStream);
                //XNamespace ns = doc.GetDefaultNamespace(); //http://cpsms.nic.in/Beneficiary/ContractorPaymentStatus

                ///// if (objDAL.EditPFMSBankAcknowlegemntDetails(doc, file.FileName)) //original 
                //if (objDAL.SaveDscEnrollmentAcknowlegement(doc, file.FileName, out isRecordExists)) //tesing of new function dsc acknowlegement file
                //{
                //    return Json(new { success = true, message = "DSC acknowledgement data mapped successfully" });
                //}
                //else
                //{
                //    if (isRecordExists)
                //    {
                //        return Json(new { success = false, message = "DSC acknowledgement data already mapped" });
                //    }
                //    else
                //    {
                //        return Json(new { success = false, message = "Error in PFMS mapping DSC acknowledgement data" });
                //    }
                //}
                #endregion

                #region Multiple File
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        file = Request.Files[i];

                        XElement doc = XElement.Load(file.InputStream);
                        XNamespace ns = doc.GetDefaultNamespace(); //http://cpsms.nic.in/Beneficiary/ContractorPaymentStatus

                        status = objDAL.SaveDscEnrollmentAcknowlegement(doc, file.FileName, out isRecordExists);
                        if (status == false && isRecordExists == false)
                        {
                            break;
                        }
                    }
                    //if (isRecordExists)
                    //{
                    //    return Json(new { success = false, message = "DSC acknowledgement data already mapped for file : " + file.FileName });
                    //}
                    //else 
                    if (status == false && isRecordExists == false)
                    {
                        return Json(new { success = false, message = "Error in PFMS mapping DSC acknowledgement data for file : " + file.FileName });
                    }
                }
                return Json(new { success = true, message = "DSC acknowledgement data mapped successfully" });
                #endregion
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.SaveDSCAcknowledgementXmlData()");
                return Json(new { success = false, message = "Error occured while mapping DSC acknowledgement data" });
            }
        }
        #endregion

        #region Temporary for Testing Sign XML on LIVE

        public ActionResult TestSignXml()
        {
            string xml = string.Empty;
            string xmlFileName = string.Empty;

            string[] arrParams = null;
            try
            {
                PFMSDAL1 objDAL = new PFMSDAL1();
                //xml = objDAL.GeneratePayXmlDAL();
                xml = objDAL.GeneratePayXmlDAL(out xml);

                byte[] bytes = Encoding.ASCII.GetBytes(xml);

                if (!string.IsNullOrEmpty(xml))
                {
                    //return Json(new { success = true, message = "PFMS XML generated successfully" }, JsonRequestBehavior.AllowGet);

                    Response.Clear();
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        // for example foo.bak
                        FileName = xmlFileName + ".xml",

                        // always prompt the user for downloading, set to true if you want 
                        // the browser to try to show the file inline
                        Inline = false,
                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());
                    return File(bytes, "application/octet-stream");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult SignEpaymentXmlTest()
        {
            try
            {
                //PFMSDAL1 pfmsdal = new PFMSDAL1();

                //DSCPFMSModel model = pfmsdal.ValidateDscPFMSDetails();
                return View();
            }
            catch (Exception ex)
            {
                return Json(string.Empty);
            }
        }


        [HttpPost]
        public ActionResult GetXmlTemp()
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
            {
                sw.WriteLine(" in GetpDF :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                sw.Close();
            }

            Int64 bill_ID;
            DigSignBAL objBAL = new DigSignBAL();
            try
            {
                RegistrationData model = new RegistrationData();
                model.PdfKey = Request.Params["pdfKey"];
                model.CertificateBase64 = string.IsNullOrEmpty(Request.Params["certificate"]) ? string.Empty : Request.Params["certificate"].Trim();
                model.CertificateChainBase64 = string.IsNullOrEmpty(Request.Params["certificateChain"]) ? string.Empty : Request.Params["certificateChain"].Trim();

                if (string.IsNullOrEmpty(model.CertificateBase64) || string.IsNullOrEmpty(model.CertificateChainBase64))
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("Certificate details from client to server are null or empty." + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                        sw.Close();
                    }
                    return Json(new { message = "Certificate details from client to server are null or empty." });
                }

                RegisterDSCModel regModel = new RegisterDSCModel();
                regModel = objBAL.GetDetailsToRegisterDSC();

                //verify certificate equality with Authorized Signatory's registered Certificate
                string verificationOfRegCertResult = objBAL.VerifyRegisteredCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);

                if (!verificationOfRegCertResult.Equals(string.Empty))
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("verificationOfRegCertResult is NULL or empty" + DateTime.Now.ToString() + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                        sw.Close();
                    }

                    return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }

                // verify Certificate, if suucessful, then only store it in DB else flag error message
                string verificationResult = objBAL.VerifyCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NameAsPerCertificate);

                if (verificationResult.Equals(string.Empty))
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("verificationResult success " + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                        sw.Close();
                    }

                    Int32 AdminNdCode = Convert.ToInt32(model.PdfKey);
                    string fileName = string.Empty;
                    string Path = string.Empty;
                    String xmlString = String.Empty;

                    string strFlag = "1";
                    string passwords = string.Empty;

                    if (strFlag.Equals("1"))
                    {
                        //Path = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;
                        PFMSDAL1 pfmsDal = new PFMSDAL1();

                        //xmlString = pfmsDal.GenerateDSCXml(AdminNdCode, out fileName);
                        xmlString = pfmsDal.GeneratePayXmlDAL(out fileName);
                        TempData["FileName"] = fileName;
                        //Path = @"C:\AuthPdfFile\newpayrequest.xml";
                    }
                    else
                    {
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                        {
                            sw.WriteLine("verificationResult is  NULL or empty" + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                            sw.Close();
                        }
                        return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                            JsonRequestBehavior.AllowGet);
                    }
                    //if (System.IO.File.Exists(Path))
                    if (!String.IsNullOrEmpty(xmlString))
                    {
                        //var fileByteArr = System.IO.File.ReadAllBytes(Path);
                        var fileByteArr = System.Text.Encoding.ASCII.GetBytes(xmlString);
                        string fileBase64Str = Convert.ToBase64String(fileByteArr);

                        passwords = "abc";
                        string encParam = PMGSY.Common.Cryptography.AESEncrypt(passwords);
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                        {
                            sw.WriteLine("encParam" + encParam + "  " + DateTime.Now.ToString() + " UserName -" +
                           PMGSYSession.Current.UserName);
                            sw.Close();
                        }

                        return Json(new
                        {
                            status = "success",
                            password = encParam,
                            message = string.Empty,
                            fileBase64String = fileBase64Str,
                            encHashOfBase64String =
                                string.Empty
                        }, JsonRequestBehavior.AllowGet);

                        // return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        //JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                            JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + "GetXmlTemp()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Json(new { status = "error", message = "Error occurred while accessing pdf file.", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveXmlTemp()
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
            {
                sw.WriteLine(" in SavePdf :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                sw.Close();
            }

            DigSignBAL objBAL = new DigSignBAL();
            RegistrationData model = new RegistrationData();
            model.PdfKey = Request.Params["pdfKey"];

            PFMSDAL1 pfmsdal = new PFMSDAL1();
            // string fileName = pfmsdal.GetDSCFileName(Convert.ToInt32(model.PdfKey));//"NewFile.xml";
            //Boolean isXmlFlagSet = pfmsdal.ValidXmlGenerateSetFlag();
            try
            {
                // <add key="XmlFilePath" value="C:\AuthXmlFile\" />
                String XmlFilePath = ConfigurationManager.AppSettings["PFMSXmlSignFilePath"].ToString();
                if (!Directory.Exists(XmlFilePath))
                    Directory.CreateDirectory(XmlFilePath);

                //string fileName1 = (PMGSYSession.Current.AdminNdCode == 451) ? "0037DBTPAYREQ" + DateTime.Now.Day.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Year.ToString() + "1" 
                //                 : (PMGSYSession.Current.AdminNdCode == 453) ? "0037DBTPAYREQ" + DateTime.Now.Day.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Year.ToString() + "2" //(String)TempData["FileName"];
                //                                                             : "0037DBTPAYREQ" + DateTime.Now.Day.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Year.ToString() + "3";

                string fileName1 = (String)TempData["FileName"];
                Request.Files[0].SaveAs(Path.Combine(XmlFilePath, fileName1 + ".xml"));
                return Json(new { status = "success", message = "Document signed successfully." });

                //if (isXmlFlagSet)
                //{
                //    return Json(new { status = "success", message = "Document signed successfully." });
                //}
                //else
                //{
                //    System.IO.File.Delete(Path.Combine(XmlFilePath, fileName1));
                //    return Json(new { status = "error", message = "Error occured while processing your request." });
                //}
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + "SaveXmlTemp()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Json(new { message = "Error occurred while saving signed Document." });
            }
        }
        #endregion

        #region Download XML Beneficiary
        //
        // GET: /PFMS/
        [HttpGet]
        public ActionResult DownloadXMLLayout()
        {
            PFMSDownloadXMLViewModel model = new PFMSDownloadXMLViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.lstState = new List<SelectListItem>();
                    model.lstState.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName.Trim(), Value = Convert.ToString(PMGSYSession.Current.StateCode).Trim() });
                    model.lstAgency = comm.PopulateAgencies(PMGSYSession.Current.StateCode, false);

                    model.lstDistrict = comm.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                    model.lstDistrict.Find(x => x.Value == "0").Text = "All Districts";
                }
                else
                {
                    model.lstState = comm.PopulateStates(true);
                    model.lstAgency = new List<SelectListItem>();
                    model.lstAgency.Insert(0, new SelectListItem { Text = "Select Agency", Value = "-1" });

                    model.lstDistrict = new List<SelectListItem>();
                    model.lstDistrict.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.DownloadXMLLayout()");
                return null;
            }
        }

        //[HttpGet]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult GenerateXML(/*string param*/ string[] Contractors)
        {
            string xml = string.Empty;
            string xmlFileName = string.Empty;
            int recordCount = 0;
            //PFMSDownloadXMLViewModel model;
            string[] arrParams = null;
            string stateShortCode = string.Empty;
            PFMSDownloadXMLViewModel model = null;
            try
            {


                model = new PFMSDownloadXMLViewModel();

                model.stateCode = string.IsNullOrEmpty(Request.Params["stateCode"]) ? 0 : Convert.ToInt32(Request.Params["stateCode"]);
                model.districtCode = string.IsNullOrEmpty(Request.Params["districtCode"]) ? 0 : Convert.ToInt32(Request.Params["districtCode"]);
                model.agencyCode = string.IsNullOrEmpty(Request.Params["agencyCode"]) ? 0 : Convert.ToInt32(Request.Params["agencyCode"]);

                model.Level = string.IsNullOrEmpty(Request.Params["level"]) ? 0 : Convert.ToInt32(Request.Params["level"]);

                if ((Contractors == null && model.Level == 1))
                {
                    return Json(new { message = "No contractors selected, please select a contractor" }, JsonRequestBehavior.AllowGet);
                }

                if (Contractors != null)
                {
                    model.mastContractorIds = new string[Contractors.Length];
                    Contractors.CopyTo(model.mastContractorIds, 0);
                }
                //arrParams = param.Split('$');
                //model = new PFMSDownloadXMLViewModel();

                //model.stateCode = Convert.ToInt32(arrParams[0]);
                //model.agencyCode = Convert.ToInt32(arrParams[1]);

                objDAL = new PFMSDAL1();
                xml = objDAL.GenerateXMLDAL(model, out xmlFileName, out recordCount);

                byte[] bytes = Encoding.ASCII.GetBytes(xml);

                if (!string.IsNullOrEmpty(xml))
                {
                    //System.IO.File.WriteAllBytes(Path.Combine(ConfigurationManager.AppSettings["PFMSBeneficiaryRequest"] + "\\" + PMGSYSession.Current.StateShortCode.Trim() + "\\" + xmlFileName + ".xml"), bytes); // Requires System.IO

                    stateShortCode = objDAL.GetStateShortName(model.stateCode);
                    string filePath = Path.Combine(ConfigurationManager.AppSettings["PFMSBeneficiaryRequest"] + "\\" + stateShortCode);
                    
                    if(!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    
                    System.IO.File.WriteAllBytes(Path.Combine(ConfigurationManager.AppSettings["PFMSBeneficiaryRequest"] + "\\" + stateShortCode + "\\" + xmlFileName + ".xml"), bytes); // Requires System.IO
                    return Json(new { success = true, message = "PFMS XML generated successfully" }, JsonRequestBehavior.AllowGet);

                    //Response.Clear();
                    //var cd = new System.Net.Mime.ContentDisposition
                    //{
                    //    // for example foo.bak
                    //    FileName = xmlFileName + ".xml",

                    //    // always prompt the user for downloading, set to true if you want 
                    //    // the browser to try to show the file inline
                    //    Inline = false,
                    //};
                    //Response.AppendHeader("Content-Disposition", cd.ToString());
                    //return File(bytes, "application/octet-stream");
                }
                else
                {
                    if (recordCount == 0)
                    {
                        return Json(new { message = "No Records to generate XML" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { message = "Error in PFMS XML generation" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GenerateXML");
                //return null;
                return Json(new { message = "Error occured while PFMS XML generation" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetBeneficiaryDetails(FormCollection formCollection)
        {
            int stateCode = 0, districtCode = 0, agencyCode = 0;
            try
            {
                objDAL = new PFMSDAL1();

                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                agencyCode = Convert.ToInt32(Request.Params["agencyCode"]);
                int totalRecords;

                var jsonData = new
                {
                    rows = objDAL.GetBeneficiaryDetailsDAL(stateCode, districtCode, agencyCode, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GetBeneficiaryDetails()");
                return null;
            }
        }

        #endregion

        #region Map Contractor for PFMS
        [HttpGet]
        public ActionResult GetXMLValueLayout()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GetXMLValueLayout()");
                return null;
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult GetXMLValue()
        {
            CommonFunctions comm = new CommonFunctions();
            string[] arrXML = null;
            string filePath = string.Empty;
            //int i = 0;
            HttpPostedFileBase file = null;
            bool status = false;
            try
            {
                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "Please select a xml file" });
                }

                filePath = ConfigurationManager.AppSettings["PFMS_VALIDATE_XML"];
                objDAL = new PFMSDAL1();

                //if (!(objDAL.IsValidXMLFile(filePath, Request)))
                //{
                //    return Json(new { success = false, message = "Please select a valid xml file" });
                //}



                #region Code for Reference
                //IEnumerable<System.Xml.Linq.XElement> direclty = infodoc.Elements("Settings").Elements("directory");
                //var rosterUserIds = direclty.Select(r => r.Attribute("value").Value);

                //XmlDocument doc = new XmlDocument();
                //doc.Load(file.InputStream);

                //Display all the book titles.
                #region CPSMSID
                //XmlNodeList elemList = doc.GetElementsByTagName("CPSMSID");
                //for (int i = 0; i < elemList.Count; i++)
                //{
                //    if (i == 0)
                //    {
                //        arrXML = new string[elemList.Count];
                //    }
                //    arrXML[i] = elemList[i].InnerXml;
                //    //Console.WriteLine(elemList[i].InnerXml);
                //}
                #endregion

                #region XML Read
                //XmlNodeList nodes = doc.GetElementsByTagName("Cstmr");
                //foreach (XmlNode node in nodes)
                //{
                //    foreach (XmlNode child in node.ChildNodes)
                //    {
                //        if (child.Name == "PrTry")
                //        {
                //            foreach (XmlNode child1 in child.ChildNodes)
                //            {
                //                if (child1.Name == "Id")
                //                {
                //                    model = new ContractorMapping();
                //                    model.contractorID = string.IsNullOrEmpty(child1.InnerXml) ? 0 : Convert.ToInt32(child1.InnerXml);
                //                    break;
                //                }
                //            }
                //        }
                //        if (child.Name == "CPSMSId")
                //        {
                //            //model.cpsmsID = string.IsNullOrEmpty(child.InnerXml) ? 0 : Convert.ToInt32(child.InnerXml);
                //            model.cpsmsID = string.IsNullOrEmpty(child.InnerXml) ? "--" : child.InnerXml;
                //            //break;
                //        }
                //        //if (child.Name == "Nm")
                //        //{
                //        //    model.contractorName = child.InnerXml;
                //        //}
                //        if (child.Name == "PstlAdr")
                //        {
                //            foreach (XmlNode child1 in child.ChildNodes)
                //            {
                //                if (child1.Name == "Prtry")
                //                {
                //                    foreach (XmlNode child2 in child1.ChildNodes)
                //                    {
                //                        if (child2.Name == "DstCd")
                //                        {
                //                            model.lgdDistrictCode = string.IsNullOrEmpty(child2.InnerXml) ? 0 : Convert.ToInt32(child2.InnerXml);
                //                        }
                //                        if (child2.Name == "PrvcCd")
                //                        {
                //                            model.lgdStateCode = string.IsNullOrEmpty(child2.InnerXml) ? 0 : Convert.ToInt32(child2.InnerXml);
                //                            break;
                //                        }
                //                    }
                //                }
                //            }
                //        }

                //        if (child.Name == "CstmrAcct")
                //        {
                //            foreach (XmlNode child1 in child.ChildNodes)
                //            {
                //                if (child1.Name == "CstmrAgt")
                //                {
                //                    foreach (XmlNode child2 in child1.ChildNodes)
                //                    {
                //                        if (child2.Name == "FinInstnId")
                //                        {
                //                            foreach (XmlNode child3 in child2.ChildNodes)
                //                            {
                //                                if (child3.Name == "BICFI")
                //                                {
                //                                    model.bankName = child3.InnerXml;
                //                                }
                //                                if (child3.Name == "BrnchId")
                //                                {
                //                                    model.branchName = child3.InnerXml;
                //                                    break;
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //                if (child1.Name == "AcctId")
                //                {
                //                    foreach (XmlNode child2 in child1.ChildNodes)
                //                    {
                //                        if (child2.Name == "Othr")
                //                        {
                //                            foreach (XmlNode child3 in child2.ChildNodes)
                //                            {
                //                                if (child3.Name == "BBAN")
                //                                {
                //                                    model.accountNumber = child3.InnerXml;
                //                                    break;
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    lstmodel.Add(model);
                //    i++;
                //}
                #endregion
                #endregion

                #region Single File
                //ContractorMapping model = null;
                //List<ContractorMapping> lstmodel = new List<ContractorMapping>();

                //file = Request.Files[0];

                //XElement doc = XElement.Load(file.InputStream);

                //XNamespace ns = doc.GetDefaultNamespace();//"http://cpsms.nic.in/Beneficiary/BeneficiaryDataResponse";

                //lstmodel = (from ack in doc.Element(ns + "CstmrDtls").Element(ns + "CstmrInf").Elements(ns + "CstmrTxInf")
                //            select new ContractorMapping()
                //            {
                //                contractorID = Convert.ToInt32(ack.Element(ns + "Cstmr").Element(ns + "PrTry").Element(ns + "Id").Value),
                //                cpsmsID = ack.Element(ns + "Cstmr").Descendants(ns + "CPSMSId").Count() > 0 ? ack.Element(ns + "Cstmr").Element(ns + "CPSMSId").Value : null,

                //                //cpsmsID = ack.Element(ns + "Cstmr").Element(ns + "CPSMSId").HasElements ? ack.Element(ns + "Cstmr").Element(ns + "CPSMSId").Value : null,
                //                bankName = ack.Element(ns + "Cstmr").Element(ns + "CstmrAcct").Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BICFI").Value,
                //                branchName = ack.Element(ns + "Cstmr").Element(ns + "CstmrAcct").Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BrnchId").Value,
                //                accountNumber = ack.Element(ns + "Cstmr").Element(ns + "CstmrAcct").Element(ns + "AcctId").Element(ns + "Othr").Element(ns + "BBAN").Value,
                //                acceptStatus = ack.Element(ns + "Cstmr").Element(ns + "CstmrSts").Value,
                //                //lstRejectCode = ack.Element(ns + "Cstmr").Descendants(ns + "StsRsnInf").Count() > 0 ? ack.Element(ns + "Cstmr").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null,

                //                lstRejectCode = ((ack.Element(ns + "Cstmr").Descendants(ns + "StsRsnInf").Count() > 0) ?
                //                                 (from cd in ack.Element(ns + "Cstmr").Element(ns + "StsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
                //                                  select cd.Element(ns + "Cd").Value).ToList() : null)

                //                //FileName = FileName,
                //                //BatchId = ack.Element(ns + "OrgnlPmtInfId").Value,
                //                //BillId = ack.Element(ns + "TxInfAndSts").Element(ns + "OrgnlEndToEndId").Value,
                //                //Status = ack.Element(ns + "TxInfAndSts").Element(ns + "TxSts").Value,
                //                //RejectionCode = ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value,
                //                //RejectionReason = ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value
                //            }).ToList();

                //if (objDAL.EditPFMSContractorDetails(lstmodel, file.FileName))
                //{
                //    return Json(new { success = true, message = "PFMS Contractor mapped successfully" }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json(new { success = false, message = "Error in PFMS Contractor mapping" }, JsonRequestBehavior.AllowGet);
                //}
                #endregion

                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        file = Request.Files[i];

                        ContractorMapping model = null;
                        List<ContractorMapping> lstmodel = new List<ContractorMapping>();

                        XElement doc = XElement.Load(file.InputStream);
                        XNamespace ns = doc.GetDefaultNamespace();//"http://cpsms.nic.in/Beneficiary/BeneficiaryDataResponse";

                        lstmodel = (from ack in doc.Element(ns + "CstmrDtls").Element(ns + "CstmrInf").Elements(ns + "CstmrTxInf")
                                    select new ContractorMapping()
                                    {
                                        contractorID = Convert.ToInt32(ack.Element(ns + "Cstmr").Element(ns + "PrTry").Element(ns + "Id").Value),
                                        cpsmsID = ack.Element(ns + "Cstmr").Descendants(ns + "CPSMSId").Count() > 0 ? ack.Element(ns + "Cstmr").Element(ns + "CPSMSId").Value : null,

                                        //cpsmsID = ack.Element(ns + "Cstmr").Element(ns + "CPSMSId").HasElements ? ack.Element(ns + "Cstmr").Element(ns + "CPSMSId").Value : null,
                                        bankName = ack.Element(ns + "Cstmr").Element(ns + "CstmrAcct").Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BICFI").Value,
                                        branchName = ack.Element(ns + "Cstmr").Element(ns + "CstmrAcct").Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BrnchId").Value,
                                        accountNumber = ack.Element(ns + "Cstmr").Element(ns + "CstmrAcct").Element(ns + "AcctId").Element(ns + "Othr").Element(ns + "BBAN").Value,
                                        acceptStatus = ack.Element(ns + "Cstmr").Element(ns + "CstmrSts").Value,
                                        //lstRejectCode = ack.Element(ns + "Cstmr").Descendants(ns + "StsRsnInf").Count() > 0 ? ack.Element(ns + "Cstmr").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null,

                                        lstRejectCode = ((ack.Element(ns + "Cstmr").Descendants(ns + "StsRsnInf").Count() > 0) ?
                                                         (from cd in ack.Element(ns + "Cstmr").Element(ns + "StsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
                                                          select cd.Element(ns + "Cd").Value).ToList() : null),

                                        batchId = (from cd in doc.Element(ns + "CstmrDtls").Elements(ns + "CstmrInf") select cd.Element(ns + "CstmrInfId").Value).FirstOrDefault(),

                                        pfmsConName = ack.Element(ns + "Cstmr").Descendants(ns + "CstmrAcct").Descendants(ns + "CstmrAcctFmly").Descendants(ns + "Nm").Count() > 0 ? ack.Element(ns + "Cstmr").Element(ns + "CstmrAcct").Element(ns + "CstmrAcctFmly").Element(ns + "Nm").Value : null,
                                        pfmsStateCode = (from cd in doc.Element(ns + "CstmrDtls").Elements(ns + "GrpHdr").Elements(ns + "OrglInitgPty").Elements(ns + "PrTry") select cd.Element(ns + "Id").Value).FirstOrDefault(),
                                        pfmsResponseDate = (from cd in doc.Element(ns + "CstmrDtls").Elements(ns + "GrpHdr") select cd.Element(ns + "CreDtTm").Value).FirstOrDefault(),

                                        //(doc.Element("CstmrDtls").Element(ns + "Cstmr").Element(ns + "CstmrInfId").Value)
                                        //FileName = FileName,
                                        //BatchId = ack.Element(ns + "OrgnlPmtInfId").Value,
                                        //BillId = ack.Element(ns + "TxInfAndSts").Element(ns + "OrgnlEndToEndId").Value,
                                        //Status = ack.Element(ns + "TxInfAndSts").Element(ns + "TxSts").Value,
                                        //RejectionCode = ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value,
                                        //RejectionReason = ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value
                                    }).ToList();

                        status = objDAL.EditPFMSContractorDetails(lstmodel, file.FileName);
                        if (status == false)
                        {
                            break;
                        }
                    }
                    if (status == false)
                    {
                        return Json(new { success = false, message = "Error in PFMS Contractor mapping for file : " + file.FileName }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = true, message = "PFMS Contractor mapped successfully" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = "PFMS Contractor mapped successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GetXMLValue()");
                return Json(new { success = false, message = "Error occured while mapping contractor" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Payment Sign XML
        [HttpPost]
        [Audit]
        public ActionResult SignPaymentXml()
        {
            int MastConId = 0, ConAccountId = 0;
            try
            {
                PFMSDAL1 pfmsdal = new PFMSDAL1();
                DSCPFMSModel model = pfmsdal.ValidateDscPFMSDetails();

                string encrBillId = Request.Params["encrBillId"].Trim();
                String[] encryptedParameters = encrBillId.Split('/');
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    model.billId = Convert.ToInt64(urlSplitParams[0]);
                    model.mastConId = Convert.ToInt32(urlSplitParams[4]);
                    model.conAccountId = string.IsNullOrEmpty(urlSplitParams[5]) ? 0 : Convert.ToInt32(urlSplitParams[5]);
                }
                model.IsValidContractor = pfmsdal.ValidatePFMSContractor(model.mastConId, model.conAccountId);

                return View("SignPaymentXml", model);
            }
            catch (Exception ex)
            {
                return Json(string.Empty);
            }
        }

        [HttpPost]
        public ActionResult GetXml()
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
            {
                sw.WriteLine(" in GetpDF :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                sw.Close();
            }

            PFMSDAL1 pfmsDal = new PFMSDAL1();
            Int64 bill_ID;
            DigSignBAL objBAL = new DigSignBAL();
            try
            {

                RegistrationData model = new RegistrationData();
                model.PdfKey = Request.Params["pdfKey"];
                model.CertificateBase64 = string.IsNullOrEmpty(Request.Params["certificate"]) ? string.Empty : Request.Params["certificate"].Trim();
                model.CertificateChainBase64 = string.IsNullOrEmpty(Request.Params["certificateChain"]) ? string.Empty : Request.Params["certificateChain"].Trim();

                #region Payment Validation
                bill_ID = Convert.ToInt64(model.PdfKey.Split('$')[1]);
                if (pfmsDal.IsPaymentExists(bill_ID))
                {
                    PMGSYSession.Current.AppletErrMessage = "Cannot make duplicate payment";
                    return Json(new { status = "error", message = "Cannot make duplicate payment", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                   JsonRequestBehavior.AllowGet);
                }
                #endregion

                if (string.IsNullOrEmpty(model.CertificateBase64) || string.IsNullOrEmpty(model.CertificateChainBase64))
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("Certificate details from client to server are null or empty." + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                        sw.Close();
                    }
                    PMGSYSession.Current.AppletErrMessage = "Certificate details from client to server are null or empty.";
                    return Json(new { message = "Certificate details from client to server are null or empty." });
                }

                RegisterDSCModel regModel = new RegisterDSCModel();
                regModel = objBAL.GetDetailsToRegisterDSC();

                //verify certificate equality with Authorized Signatory's registered Certificate
                string verificationOfRegCertResult = objBAL.VerifyRegisteredCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);

                if (!verificationOfRegCertResult.Equals(string.Empty))
                {

                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("verificationOfRegCertResult is NULL or empty" + DateTime.Now.ToString() + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                        sw.Close();
                    }
                    PMGSYSession.Current.AppletErrMessage = verificationOfRegCertResult;
                    return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }

                // verify Certificate, if suucessful, then only store it in DB else flag error message
                string verificationResult = objBAL.VerifyCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NameAsPerCertificate);

                if (verificationResult.Equals(string.Empty))
                {

                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("verificationResult success " + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                        sw.Close();
                    }

                    Int32 AdminNdCode = Convert.ToInt32(model.PdfKey.Split('$')[0]);
                    string fileName = string.Empty;
                    int runningCnt = 0;
                    string Path = string.Empty;
                    String xmlString = String.Empty;

                    string strFlag = "1";
                    string passwords = string.Empty;



                    if (strFlag.Equals("1"))
                    {
                        //Path = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;

                        //xmlString = pfmsDal.GenerateDSCXml(AdminNdCode, out fileName);

                        PFMSDownloadPaymentXMLViewModel payModel = new PFMSDownloadPaymentXMLViewModel();

                        payModel.stateCode = PMGSYSession.Current.StateCode;
                        //payModel.agencyCode = PMGSYSession.Current.AdminNdCode;
                        payModel.generationDate = DateTime.Now.ToString();
                        payModel.FileType = "N";
                        payModel.billId = Convert.ToInt64(model.PdfKey.Split('$')[1]);

                        xmlString = pfmsDal.GeneratePaymentXMLDAL(payModel, out fileName, out runningCnt);
                        TempData["FileName"] = fileName;
                        TempData["RunningCount"] = runningCnt;
                        //Path = @"C:\AuthPdfFile\newpayrequest.xml";
                    }
                    else
                    {
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                        {
                            sw.WriteLine("verificationResult is  NULL or empty" + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                            sw.Close();
                        }
                        PMGSYSession.Current.AppletErrMessage = verificationResult;
                        return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                            JsonRequestBehavior.AllowGet);
                    }
                    //if (System.IO.File.Exists(Path))
                    if (!String.IsNullOrEmpty(xmlString))
                    {
                        //var fileByteArr = System.IO.File.ReadAllBytes(Path);
                        var fileByteArr = System.Text.Encoding.ASCII.GetBytes(xmlString);
                        string fileBase64Str = Convert.ToBase64String(fileByteArr);

                        passwords = "abc";
                        string encParam = PMGSY.Common.Cryptography.AESEncrypt(passwords);
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                        {
                            sw.WriteLine("encParam" + encParam + "  " + DateTime.Now.ToString() + " UserName -" +
                           PMGSYSession.Current.UserName);
                            sw.Close();
                        }

                        return Json(new
                        {
                            status = "success",
                            password = encParam,
                            message = string.Empty,
                            fileBase64String = fileBase64Str,
                            encHashOfBase64String =
                                string.Empty
                        }, JsonRequestBehavior.AllowGet);

                        // return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        //JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        PMGSYSession.Current.AppletErrMessage = verificationResult;
                        return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                            JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    PMGSYSession.Current.AppletErrMessage = verificationResult;
                    return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + "Get Xml()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Json(new { status = "error", message = "Error occurred while accessing pdf file.", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                    JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult SaveXml()
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
            {
                sw.WriteLine(" in SavePdf :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                sw.Close();
            }

            PaymentBAL objPaymentBAL = new PaymentBAL();

            DigSignBAL objBAL = new DigSignBAL();
            RegistrationData model = new RegistrationData();
            model.PdfKey = Request.Params["pdfKey"];

            PFMSDAL1 pfmsdal = new PFMSDAL1();
            // string fileName = pfmsdal.GetDSCFileName(Convert.ToInt32(model.PdfKey));//"NewFile.xml";
            //Boolean isXmlFlagSet = pfmsdal.ValidXmlGenerateSetFlag();
            try
            {
                // <add key="XmlFilePath" value="C:\AuthXmlFile\" />
                //String XmlFilePath = ConfigurationManager.AppSettings["PFMSDSCFilePath"].ToString();
                String XmlFilePath = ConfigurationManager.AppSettings["PFMSXmlSignFilePath"].ToString();
                if (!Directory.Exists(XmlFilePath))
                    Directory.CreateDirectory(XmlFilePath);

                #region State Short Code
                String StateShortCode = pfmsdal.GetStateShortName(PMGSYSession.Current.StateCode);
                if (!Directory.Exists(XmlFilePath + "\\" + StateShortCode))
                    Directory.CreateDirectory(XmlFilePath + "\\" + StateShortCode);

                string finalPath = XmlFilePath + "\\" + StateShortCode;// +"\\" + PMGSYSession.Current.AdminNdCode;
                #endregion

                if (!Directory.Exists(finalPath))
                    Directory.CreateDirectory(finalPath);

                #region commented
                //if (!Directory.Exists(XmlFilePath))
                //{
                //    Directory.CreateDirectory(XmlFilePath);
                //}

                //String ShortStateCode = XmlFilePath + "" + pfmsdal.GetStateshortCode();

                //if (!Directory.Exists(ShortStateCode))
                //{
                //    Directory.CreateDirectory(ShortStateCode);
                //}

                //String CurrentFinYer = "";
                //if (DateTime.Now.Month < 3)
                //{
                //    CurrentFinYer = DateTime.Now.Year - 1 + "-" + DateTime.Now.Year;
                //}
                //else
                //{
                //    CurrentFinYer = DateTime.Now.Year + "-" + (DateTime.Now.Year + 1);
                //}

                //String FinYear = ShortStateCode + "\\" + CurrentFinYer;

                //if (!Directory.Exists(FinYear))
                //{
                //    Directory.CreateDirectory(FinYear);
                //}

                //Request.Files[0].SaveAs(Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName));
                #endregion
                string fileName1 = (String)TempData["FileName"];
                int runningCount = Convert.ToInt32(TempData["RunningCount"]);

                //string result = objPaymentBAL.InsertEpaymentMailDetails(bill_ID, "S_" + fileName);
                string result = string.Empty;
                if (!string.IsNullOrEmpty(fileName1) && runningCount > 0)
                {
                    result = pfmsdal.InsertEpaymentMailDetailsPFMS(Convert.ToInt64(model.PdfKey.Split('$')[1]), fileName1 + ".xml", Convert.ToInt32(model.PdfKey.Split('$')[2]), Convert.ToInt32(model.PdfKey.Split('$')[3]), runningCount);
                }
                if (result.Equals("1"))
                {
                    #region Mail
                    //if (result.Equals("1"))
                    //{

                    //    try
                    //    {

                    //        string strHeaderPath = "";
                    //        string ErrorMessage = string.Empty;
                    //        EpaymentOrderModel epaymodel = new EpaymentOrderModel();
                    //        epaymodel = objPaymentBAL.GetEpaymentDetails(bill_ID);

                    //        MvcMailMessage ms = objPaymentBAL.EpayOrderMail(epaymodel, Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName), strHeaderPath, ref ErrorMessage);
                    //        // To uncomment file on the live environment 
                    //        ms.Send();

                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        string mailDeleteResult = objPaymentBAL.DeleteMailDetails(bill_ID);
                    //        System.IO.File.Delete(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName);
                    //        return Json(new { status = "error", message = "Error while sending E-Mail ." });

                    //    }
                    //}
                    //System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName));
                    #endregion

                    Request.Files[0].SaveAs(Path.Combine(finalPath, fileName1 + ".xml"));
                    return Json(new { status = "success", message = "Document signed successfully." });
                }
                else if (result.Equals("-1"))
                {
                    //System.IO.File.Delete(Path.Combine(finalPath, fileName1 + ".xml"));
                    return Json(new { status = "error", message = "Isssue in payment entry, please try again." });
                }
                else
                {
                    //System.IO.File.Delete(Path.Combine(finalPath, fileName1 + ".xml"));
                    return Json(new { status = "error", message = "Error occured while processing your request." });
                }
            }
            catch (Exception ex)
            {

                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + "SaveXml()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Json(new { message = "Error occurred while saving signed Document." });
            }
        }
        #endregion

        #region  DSC Deregister
        [HttpGet]
        public ActionResult GetDSCDetailsForDeregister()
        {
            PFMSDownloadXMLViewModel model = new PFMSDownloadXMLViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.lstState = new List<SelectListItem>();
                    model.lstState.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName.Trim(), Value = Convert.ToString(PMGSYSession.Current.StateCode).Trim() });
                    model.lstAgency = comm.PopulateAgencies(PMGSYSession.Current.StateCode, false);

                    model.lstDistrict = comm.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                }
                else
                {
                    model.lstState = comm.PopulateStates(true);
                    model.lstAgency = new List<SelectListItem>();
                    model.lstAgency.Insert(0, new SelectListItem { Text = "Select Agency", Value = "-1" });

                    model.lstDistrict = new List<SelectListItem>();
                    model.lstDistrict.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.DownloadXMLLayout()");
                return null;
            }
        }

        public ActionResult GetDSCDetailsListForDeregister(FormCollection formCollection)
        {
            try
            {
                objDAL = new PFMSDAL1();

                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                int totalRecords;

                var jsonData = new
                {
                    rows = objDAL.GetDSCDetailsForDeregisterDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GetBeneficiaryDetails()");
                return null;
            }
        }
        #endregion

        #region Map Contractor for PFMS Temp
        [HttpGet]
        public ActionResult GetXMLValueLayoutTemp()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GetXMLValueLayoutTemp()");
                return null;
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult GetXMLValueTemp()
        {
            CommonFunctions comm = new CommonFunctions();
            string[] arrXML = null;
            string filePath = string.Empty;
            //int i = 0;
            HttpPostedFileBase file = null;
            bool status = false;
            try
            {
                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "Please select a xml file" });
                }

                filePath = ConfigurationManager.AppSettings["PFMS_VALIDATE_XML"];
                objDAL = new PFMSDAL1();

                //if (!(objDAL.IsValidXMLFile(filePath, Request)))
                //{
                //    return Json(new { success = false, message = "Please select a valid xml file" });
                //}

                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        file = Request.Files[i];

                        ContractorMapping model = null;
                        List<ContractorMapping> lstmodel = new List<ContractorMapping>();

                        XElement doc = XElement.Load(file.InputStream);
                        XNamespace ns = doc.GetDefaultNamespace();//"http://cpsms.nic.in/Beneficiary/BeneficiaryDataResponse";

                        lstmodel = (from ack in doc.Element(ns + "CstmrDtls").Element(ns + "CstmrInf").Elements(ns + "CstmrTxInf")
                                    select new ContractorMapping()
                                    {
                                        contractorID = Convert.ToInt32(ack.Element(ns + "Cstmr").Element(ns + "PrTry").Element(ns + "Id").Value),
                                        cpsmsID = ack.Element(ns + "Cstmr").Descendants(ns + "CPSMSId").Count() > 0 ? ack.Element(ns + "Cstmr").Element(ns + "CPSMSId").Value : null,

                                        //cpsmsID = ack.Element(ns + "Cstmr").Element(ns + "CPSMSId").HasElements ? ack.Element(ns + "Cstmr").Element(ns + "CPSMSId").Value : null,
                                        bankName = ack.Element(ns + "Cstmr").Element(ns + "CstmrAcct").Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BICFI").Value,
                                        branchName = ack.Element(ns + "Cstmr").Element(ns + "CstmrAcct").Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BrnchId").Value,
                                        accountNumber = ack.Element(ns + "Cstmr").Element(ns + "CstmrAcct").Element(ns + "AcctId").Element(ns + "Othr").Element(ns + "BBAN").Value,
                                        acceptStatus = ack.Element(ns + "Cstmr").Element(ns + "CstmrSts").Value,
                                        //lstRejectCode = ack.Element(ns + "Cstmr").Descendants(ns + "StsRsnInf").Count() > 0 ? ack.Element(ns + "Cstmr").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null,

                                        lstRejectCode = ((ack.Element(ns + "Cstmr").Descendants(ns + "StsRsnInf").Count() > 0) ?
                                                         (from cd in ack.Element(ns + "Cstmr").Element(ns + "StsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
                                                          select cd.Element(ns + "Cd").Value).ToList() : null),

                                        batchId = (from cd in doc.Element(ns + "CstmrDtls").Elements(ns + "CstmrInf") select cd.Element(ns + "CstmrInfId").Value).FirstOrDefault(),
                                        pfmsConName = ack.Element(ns + "Cstmr").Descendants(ns + "CstmrAcct").Descendants(ns + "CstmrAcctFmly").Descendants(ns + "Nm").Count() > 0 ? ack.Element(ns + "Cstmr").Element(ns + "CstmrAcct").Element(ns + "CstmrAcctFmly").Element(ns + "Nm").Value : null,

                                        pfmsStateCode = (from cd in doc.Element(ns + "CstmrDtls").Elements(ns + "GrpHdr").Elements(ns + "OrglInitgPty").Elements(ns + "PrTry") select cd.Element(ns + "Id").Value).FirstOrDefault(),
                                        pfmsResponseDate = (from cd in doc.Element(ns + "CstmrDtls").Elements(ns + "GrpHdr") select cd.Element(ns + "CreDtTm").Value).FirstOrDefault(),

                                        //(doc.Element("CstmrDtls").Element(ns + "Cstmr").Element(ns + "CstmrInfId").Value)
                                        //FileName = FileName,
                                        //BatchId = ack.Element(ns + "OrgnlPmtInfId").Value,
                                        //BillId = ack.Element(ns + "TxInfAndSts").Element(ns + "OrgnlEndToEndId").Value,
                                        //Status = ack.Element(ns + "TxInfAndSts").Element(ns + "TxSts").Value,
                                        //RejectionCode = ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value,
                                        //RejectionReason = ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value
                                    }).ToList();

                        status = objDAL.EditPFMSContractorDetailsTemp(lstmodel, file.FileName);
                        if (status == false)
                        {
                            break;
                        }
                    }
                    if (status == false)
                    {
                        return Json(new { success = false, message = "Error in PFMS Contractor mapping for file : " + file.FileName }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = true, message = "PFMS Contractor mapped successfully" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = "PFMS Contractor mapped successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GetXMLValueTemp()");
                return Json(new { success = false, message = "Error occured while mapping contractor" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        //added by abhinav pathak on 08-12-2018
        #region Deactivate Contractor Account
        /// <summary>
        /// This Method Returns the view to deactivate/activate contractor details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetContractorDetailsLayout()
        {
            DeactivateAccountDetailsModel model = new DeactivateAccountDetailsModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.lstState = new List<SelectListItem>();
                    model.lstState.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName.Trim(), Value = Convert.ToString(PMGSYSession.Current.StateCode).Trim() });
                }
                else
                {
                    model.lstState = comm.PopulateStates(true);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS1.GetContractorDetailsLayout()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult DeactivateContractorAccount(FormCollection formCollection)
        {
            int stateCode = 0;
            string panNumber = null;
            string message = string.Empty;
            string accountStatus = string.Empty;
            try
            {
                objDAL = new PFMSDAL1();

                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                panNumber = (Request.Params["panNumber"]);
                accountStatus = Request.Params["status"];
                var jsonData = new
                {
                    rows = objDAL.GetContractorsListDAL(stateCode, panNumber, accountStatus, ref message),
                    page = Convert.ToInt32(formCollection["page"]),
                    usermessage = message,
                    //records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS1.GetBeneficiaryDetails()");
                return null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateContractorAccount()
        {
            try
            {
                bool status = false;
                string message = string.Empty;
                int pocmID = Convert.ToInt32(Request.Params["pocmid"]);
                objDAL = new PFMSDAL1();

                if (objDAL.DeactivateContactorDetailsDAL(pocmID))
                {
                    message = message == string.Empty ? "Account details updated successfully." : message;
                    status = true;
                }
                else
                {
                    message = message == string.Empty ? "An Error Occured, Action Could not be performed" : message;
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS1.UpdateContractorAccount()");
                return null;
            }
        }


        #endregion

        #region Update Beneficiary IFSC
        [HttpGet]
        public ActionResult UpdateBeneficiaryIFSCLayout()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.UpdateBeneficiaryIFSCLayout()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetBeneficiaryDetailsForUpdate(int? page, int? rows, string sidx, string sord)
        {
            string panNo = string.Empty;
            try
            {
                objDAL = new PFMSDAL1();

                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                panNo = Convert.ToString(Request.Params["panNo"]);

                int totalRecords;

                var jsonData = new
                {
                    rows = objDAL.GetBeneficiaryDetailsForUpdateDAL(panNo, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GetBeneficiaryDetailsForUpdate()");
                return null;
            }
        }

        public ActionResult GenerateUpdateXML(FormCollection frmCollection)
        {
            string xml = string.Empty;
            string xmlFileName = string.Empty;
            int recordCount = 0;
            //PFMSDownloadXMLViewModel model;
            string[] arrParams = null;
            string stateShortCode = string.Empty;
            PFMSDownloadXMLViewModel model = null;
            int pocmId = 0;
            bool flag = false;
            string message = string.Empty;
            try
            {
                Regex regex = new Regex(@"^[a-zA-Z0-9]+$");
                if (!(regex.IsMatch(Convert.ToString(Request.Params["IFSCCode"]).Trim())))
                {
                    return Json("No Special Characters Allowed.");
                }
                if (Convert.ToString(Request.Params["IFSCCode"]).Trim().Length > 11)
                {
                    return Json("PAN No. length exceeds 11 characters.");
                }
                objDAL = new PFMSDAL1();

                pocmId = Convert.ToInt32(Request.Params["id"]);
                model = objDAL.GetAccountDetailsDAL(pocmId);

                model.operation = "U";
                model.Level = 1;

                flag = objDAL.UpdateIFSCforBeneficiaryDAL(Convert.ToInt32(model.mastContractorIds[0].Split('$')[0]), Convert.ToInt32(model.mastContractorIds[0].Split('$')[1]), Convert.ToString(Request.Params["IFSCCode"]).Trim(), ref message);

                if (flag == true)
                {
                    xml = objDAL.GenerateXMLUpdateDAL(model, out xmlFileName, out recordCount);

                    byte[] bytes = Encoding.ASCII.GetBytes(xml);

                    if (!string.IsNullOrEmpty(xml))
                    {
                        //System.IO.File.WriteAllBytes(Path.Combine(ConfigurationManager.AppSettings["PFMSBeneficiaryRequest"] + "\\" + PMGSYSession.Current.StateShortCode.Trim() + "\\" + xmlFileName + ".xml"), bytes); // Requires System.IO

                        stateShortCode = objDAL.GetStateShortName(model.stateCode);
                        System.IO.File.WriteAllBytes(Path.Combine(ConfigurationManager.AppSettings["PFMSBeneficiaryRequest"] + "\\" + stateShortCode + "\\" + xmlFileName + ".xml"), bytes); // Requires System.IO
                        return Json(new { message = "PFMS XML generated successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (recordCount == 0)
                        {
                            return Json(new { message = "No Records to generate XML" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { message = "Error in PFMS XML generation" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
                //else
                //{
                //    if (recordCount == 0)
                //    {
                //        return Json(new { message = "No Records to generate XML" }, JsonRequestBehavior.AllowGet);
                //    }
                //    else
                //    {
                //        return Json(new { message = "Error in PFMS XML generation" }, JsonRequestBehavior.AllowGet);
                //    }
                //}
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.GenerateXML");
                //return null;
                return Json(new { message = "Error occured while PFMS XML generation" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
