using PMGSY.Areas.REAT.DAL;
using PMGSY.Areas.REAT.Models;
using PMGSY.BAL.DigSign;
using PMGSY.BAL.Payment;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.DAL.PFMS;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.DigSign;
using PMGSY.Models.PFMS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace PMGSY.Areas.REAT.Controllers
{
    public class ReatController : Controller
    {
        ReatDAL objDAL = null;

        #region REAT Vendor Registration
        [HttpGet]
        public ActionResult DownloadXMLLayout()
        {
            REATDownloadXMLViewModel model = new REATDownloadXMLViewModel();
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
                ErrorLog.LogError(ex, "Reat.DownloadXMLLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetBeneficiaryDetails(FormCollection formCollection)
        {
            int stateCode = 0, districtCode = 0, agencyCode = 0;
            try
            {
                objDAL = new ReatDAL();

                //using (CommonFunctions commonFunction = new CommonFunctions())
                //{
                //    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                //    {
                //        return null;
                //    }
                //}

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
                ErrorLog.LogError(ex, "REAT.GetBeneficiaryDetails()");
                return null;
            }
        }

        [HttpPost]
        //public ActionResult GenerateXML(string[] Contractors)
        //{
        //    string xml = string.Empty;
        //    string xmlFileName = string.Empty;
        //    string message = string.Empty;
        //    int recordCount = 0;
        //    string stateShortCode = string.Empty;
            
        //    REATDownloadXMLViewModel model = null;
        //    try
        //    {

        //        model = new REATDownloadXMLViewModel();

        //        model.stateCode = string.IsNullOrEmpty(Request.Params["stateCode"]) ? 0 : Convert.ToInt32(Request.Params["stateCode"]);
        //        model.districtCode = string.IsNullOrEmpty(Request.Params["districtCode"]) ? 0 : Convert.ToInt32(Request.Params["districtCode"]);
        //        model.agencyCode = string.IsNullOrEmpty(Request.Params["agencyCode"]) ? 0 : Convert.ToInt32(Request.Params["agencyCode"]);

        //        model.Level = string.IsNullOrEmpty(Request.Params["level"]) ? 0 : Convert.ToInt32(Request.Params["level"]);

        //        if ((Contractors == null && model.Level == 1))
        //        {
        //            return Json(new { message = "No contractors selected, please select a contractor" }, JsonRequestBehavior.AllowGet);
        //        }

        //        if (Contractors != null)
        //        {
        //            model.mastContractorIds = new string[Contractors.Length];
        //            Contractors.CopyTo(model.mastContractorIds, 0);
        //        }

        //        objDAL = new ReatDAL();
        //        //xml = objDAL.GenerateXMLDAL(model, out xmlFileName, out recordCount);

        //        bool result = objDAL.GenerateXMLDAL(model, ref message);

        //        return Json(new { message = message.Trim() }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "REAT.GenerateXML");
        //        return Json(new { message = "Error occured while Reat XML generation" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult GenerateXML(string[] Contractors)
        {
            string xml = string.Empty;
            string xmlFileName = string.Empty;
            string message = string.Empty;
            int recordCount = 0;
            string stateShortCode = string.Empty;

            REATDownloadXMLViewModel model = null;
            try
            {

                model = new REATDownloadXMLViewModel();

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
                    string ContractorID = null;
                    List<string> IdList = new List<string>();
                    foreach (string Id in Contractors)
                    {
                        ContractorID = Id.Split('$')[0];
                        IdList.Add(ContractorID);
                    }
                    String[] IdArr = IdList.ToArray();
                    if (IdArr.Distinct().Count() != IdArr.Count())
                    {
                        return Json(new { message = "Error : Duplicate contractor selection is not allowed" }, JsonRequestBehavior.AllowGet);
                    }
                    model.mastContractorIds = new string[Contractors.Length];
                    Contractors.CopyTo(model.mastContractorIds, 0);
                }

                objDAL = new ReatDAL();
                //xml = objDAL.GenerateXMLDAL(model, out xmlFileName, out recordCount);

                bool result = objDAL.GenerateXMLDAL(model, ref message);

                return Json(new { message = message.Trim() }, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException ex)
            {
                ErrorLog.LogError(ex, "REAT.GenerateXML");
                return Json(new { message = "Error occured while Reat XML generation" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GenerateXML");
                return Json(new { message = "Error occured while Reat XML generation" }, JsonRequestBehavior.AllowGet);
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
                objDAL = new ReatDAL();

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
                        List<ContractorMappingREAT> lstmodel = new List<ContractorMappingREAT>();

                        XElement doc = XElement.Load(file.InputStream);
                        XNamespace ns = doc.GetDefaultNamespace();//"http://cpsms.nic.in/VendorDataResponse";

                        #region Not Working for multiple customer
                        //lstmodel = (from ack in doc.Element(ns + "CstmrDtls").Element(ns + "CstmrInf").Elements(ns + "CstmrTxInf")
                        //            select new ContractorMappingREAT()
                        //            {
                        //                contractorID = Convert.ToInt32(ack.Element(ns + "Cstmr").Element(ns + "Prtry").Element(ns + "Id").Value),
                        //                cpsmsID = ack.Element(ns + "Cstmr").Descendants(ns + "PFMSId").Count() > 0 ? ack.Element(ns + "Cstmr").Element(ns + "PFMSId").Value : null,

                        //                bankName = ack.Element(ns + "Cstmr").Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BICFI").Value,
                        //                branchName = ack.Element(ns + "Cstmr").Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BrnchId").Value,
                        //                accountNumber = ack.Element(ns + "Cstmr").Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BBAN").Value,
                        //                acceptStatus = ack.Element(ns + "Cstmr").Element(ns + "CstmrSts").Value,


                        //                lstRejectCode = ((ack.Element(ns + "Cstmr").Descendants(ns + "StsRsnInf").Count() > 0) ?
                        //                                 (from cd in ack.Element(ns + "Cstmr").Element(ns + "StsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
                        //                                  select cd.Element(ns + "Cd").Value).ToList() : null),

                        //                batchId = (from cd in doc.Element(ns + "CstmrDtls").Elements(ns + "CstmrInf") select cd.Element(ns + "CstmrInfId").Value).FirstOrDefault(),

                        //                pfmsConName = ack.Element(ns + "Cstmr").Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BBNm").Value,
                        //                pfmsResponseDate = (from cd in doc.Element(ns + "CstmrDtls").Elements(ns + "GrpHdr") select cd.Element(ns + "CreDtTm").Value).FirstOrDefault(),
                        //                /**/

                        //                //cpsmsID = ack.Element(ns + "Cstmr").Element(ns + "CPSMSId").HasElements ? ack.Element(ns + "Cstmr").Element(ns + "CPSMSId").Value : null,
                        //                //lstRejectCode = ack.Element(ns + "Cstmr").Descendants(ns + "StsRsnInf").Count() > 0 ? ack.Element(ns + "Cstmr").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null,

                        //                //ack.Element(ns + "Cstmr").Descendants(ns + "CstmrAcct").Descendants(ns + "CstmrAcctFmly").Descendants(ns + "Nm").Count() > 0 ? ack.Element(ns + "Cstmr").Element(ns + "CstmrAcct").Element(ns + "CstmrAcctFmly").Element(ns + "Nm").Value : null,
                        //                //pfmsStateCode = (from cd in doc.Element(ns + "CstmrDtls").Elements(ns + "GrpHdr").Elements(ns + "OrglInitgPty").Elements(ns + "PrTry") select cd.Element(ns + "Id").Value).FirstOrDefault(),


                        //                //(doc.Element("CstmrDtls").Element(ns + "Cstmr").Element(ns + "CstmrInfId").Value)
                        //                //FileName = FileName,
                        //                //BatchId = ack.Element(ns + "OrgnlPmtInfId").Value,
                        //                //BillId = ack.Element(ns + "TxInfAndSts").Element(ns + "OrgnlEndToEndId").Value,
                        //                //Status = ack.Element(ns + "TxInfAndSts").Element(ns + "TxSts").Value,
                        //                //RejectionCode = ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value,
                        //                //RejectionReason = ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value
                        //      }).ToList();
                        #endregion
                        lstmodel = (from ack in doc.Element(ns + "CstmrDtls").Element(ns + "CstmrInf").Element(ns + "CstmrTxInf").Elements(ns + "Cstmr")
                                    select new ContractorMappingREAT()
                                    {
                                        contractorID = Convert.ToInt32(ack.Element(ns + "Prtry").Element(ns + "Id").Value),
                                        cpsmsID = ack.Descendants(ns + "PFMSId").Count() > 0 ? ack.Element(ns + "PFMSId").Value : null,
                                        bankName = ((ack.Descendants(ns + "BICFI").Count() > 0) ? ack.Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BICFI").Value : null),
                                        branchName = ((ack.Descendants(ns + "BrnchId").Count() > 0) ? ack.Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BrnchId").Value : null),
                                        accountNumber = ((ack.Descendants(ns + "BBAN").Count() > 0) ? ack.Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BBAN").Value : null),

                                        acceptStatus = ack.Element(ns + "CstmrSts").Value,
                                        lstRejectCode = ((ack.Descendants(ns + "StsRsnInf").Count() > 0) ?
                                                         (from cd in ack.Element(ns + "StsRsnInf").Elements(ns + "Rsn")
                                                          select cd.Element(ns + "Cd").Value).ToList() : null),
                                        batchId = (from cd in doc.Element(ns + "CstmrDtls").Elements(ns + "CstmrInf") select cd.Element(ns + "CstmrInfId").Value).FirstOrDefault(),
                                        pfmsConName = ((ack.Descendants(ns + "BBNm").Count() > 0) ? ack.Element(ns + "CstmrAgt").Element(ns + "FinInstnId").Element(ns + "BBNm").Value : null),
                                        pfmsResponseDate = (from cd in doc.Element(ns + "CstmrDtls").Elements(ns + "GrpHdr") select cd.Element(ns + "CreDtTm").Value).FirstOrDefault(),
                                    }).ToList();
                        status = objDAL.EditREATContractorDetails(lstmodel, file.FileName);
                        if (status == false)
                        {
                            break;
                        }
                    }
                    if (status == false)
                    {
                        return Json(new { success = false, message = "Error in REAT Contractor mapping for file : " + file.FileName }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = true, message = "REAT Contractor mapped successfully" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = "REAT Contractor mapped successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetXMLValue()");
                return Json(new { success = false, message = "Error occured while mapping contractor" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region REAT DSC Register
        [HttpPost]
        [Audit]
        public ActionResult SignEpaymentDSCXmlREAT()
        {
            ReatDAL reatDAL = new ReatDAL();
            try
            {
                objDAL = new ReatDAL();
                DSCREATModel model = null;

                //if (Request.Params["operation"] == "D")
                //{
                //    //String[] urlParams = URLEncrypt.DecryptParameters(new String[] { Request.Params["officerCode"].Split('/')[0], Request.Params["officerCode"].Split('/')[1], Request.Params["officerCode"].Split('/')[2] });

                //    //model = reatDAL.ValidateDscREATDetailsforDelete(Convert.ToInt32(urlParams[0].Split('$')[0]));
                //    //model.operation = Request.Params["operation"];
                //    //model.adminNdName = urlParams[0].Split('$')[1].Trim();
                //}
                //else
                //{
                    model = reatDAL.ValidateDscREATDetails();
                    model.operation = "A";

                //}

                    if (model.IsInitPartyAvailable == true)
                    {
                        return View("SignEpaymentDSCXmlREAT", model);
                    }
                    else
                    {
                       // return Json(new { status = "error", message = "Verify Initiating Party Details - Unique Agency Code and scheme code but be available before registration of DSC. " });
                        return Json(new { Success = false, error = "Verify Initiating Party Details - Unique Agency Code and scheme code but be available before registration of DSC. " });
                    }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Reat.SignEpaymentDSCXmlREAT()");
                return Json(string.Empty);
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult DeRegisterDSCXmlREAT()
        {
            ReatDAL reatDAL = new ReatDAL();
            try
            {
                objDAL = new ReatDAL();
                DSCREATModel model = null;

                //if (Request.Params["operation"] == "D")
                //{
                //    //String[] urlParams = URLEncrypt.DecryptParameters(new String[] { Request.Params["officerCode"].Split('/')[0], Request.Params["officerCode"].Split('/')[1], Request.Params["officerCode"].Split('/')[2] });

                //    //model = reatDAL.ValidateDscREATDetailsforDelete(Convert.ToInt32(urlParams[0].Split('$')[0]));
                //    //model.operation = Request.Params["operation"];
                //    //model.adminNdName = urlParams[0].Split('$')[1].Trim();
                //}
                //else
                //{
                model = reatDAL.ValidateDscREATDetails();
                model.operation = "D";

                //}

                if (model.IsInitPartyAvailable == true)
                {
                   // GetDSCXmlforDelete();
                    return View("SignEpaymentDSCXmlREAT", model);
                }
                else
                {
                    // return Json(new { status = "error", message = "Verify Initiating Party Details - Unique Agency Code and scheme code but be available before registration of DSC. " });
                    return Json(new { Success = false, error = "Verify Initiating Party Details - Unique Agency Code and scheme code but be available before registration of DSC. " });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Reat.DeRegisterDSCXmlREAT()");
                return Json(string.Empty);
            }
        }


        [HttpPost]
        public ActionResult GetDSCXml()
        {
          

  
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
                  
                    PMGSYSession.Current.AppletErrMessage = "Certificate details from client to server are null or empty.";
                    return Json(new { message = "Certificate details from client to server are null or empty." });
                }

                RegisterDSCModel regModel = new RegisterDSCModel();
                regModel = objBAL.GetDetailsToRegisterDSC();

                //verify certificate equality with Authorized Signatory's registered Certificate
                string verificationOfRegCertResult = objBAL.VerifyRegisteredCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);

                if (!verificationOfRegCertResult.Equals(string.Empty))
                {

                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["REATDigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
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

                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["REATDigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("verificationResult success " + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                        sw.Close();
                    }

                    Int32 AdminNdCode = Convert.ToInt32(model.PdfKey.Split('$')[0]);
                    string fileName = string.Empty;
                    string Path = string.Empty;
                    String xmlString = String.Empty;

                   // string strFlag = "1";
                    string passwords = string.Empty;

                  //  if (strFlag.Equals("1"))
                   // {
                        //Path = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;
                        ReatDAL reatDAL = new ReatDAL();

                        xmlString = reatDAL.GenerateDSCXml(AdminNdCode, out fileName, "A", regModel.NameAsPerCertificate);
                        TempData["FileName"] = fileName;
                        //Path = @"C:\AuthPdfFile\newpayrequest.xml";
                   // }
                  //  else
                   // {
                     //   using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["REATDigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                       // {
                         //   sw.WriteLine("verificationResult is  NULL or empty" + DateTime.Now.ToString() + " UserName -" +
                           // PMGSYSession.Current.UserName);
                           // sw.Close();
                        //}

                       // return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        //    JsonRequestBehavior.AllowGet);
                    //}
                    //if (System.IO.File.Exists(Path))
                    if (!String.IsNullOrEmpty(xmlString))
                    {
                        //var fileByteArr = System.IO.File.ReadAllBytes(Path);
                        var fileByteArr = System.Text.Encoding.ASCII.GetBytes(xmlString);
                        string fileBase64Str = Convert.ToBase64String(fileByteArr);

                        passwords = "abc";
                        string encParam = PMGSY.Common.Cryptography.AESEncrypt(passwords);
                       
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
           
                return Json(new { status = "error", message = "Error occurred while accessing pdf file.", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                    JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult GetDSCXmlforDelete()
        {
            

            Int64 bill_ID;
            DigSignBAL objBAL = new DigSignBAL();
            try
            {
                RegistrationData model = new RegistrationData();
               
                model.PdfKey = Request.Params["pdfKey"];   // uncomment
                
                model.CertificateBase64 = string.IsNullOrEmpty(Request.Params["certificate"]) ? string.Empty : Request.Params["certificate"].Trim();
                model.CertificateChainBase64 = string.IsNullOrEmpty(Request.Params["certificateChain"]) ? string.Empty : Request.Params["certificateChain"].Trim();

                if (string.IsNullOrEmpty(model.CertificateBase64) || string.IsNullOrEmpty(model.CertificateChainBase64))
                {
                   
                    PMGSYSession.Current.AppletErrMessage = "Certificate details from client to server are null or empty.";
                    return Json(new { message = "Certificate details from client to server are null or empty." });
                }

                RegisterDSCModel regModel = new RegisterDSCModel();
                regModel = objBAL.GetDetailsToRegisterDSC();

                 // regModel.NameAsPerCertificate = model.PdfKey.Split('$')[2];   // uncomment

              //  regModel.NameAsPerCertificate = "VAISHALI ARUNKUMAR JAISWAL";

                  using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                  {
                      sw.WriteLine("1 :" );
                      sw.WriteLine("Name :" + regModel.NameAsPerCertificate);
                   
                      sw.Close();
                  }



                string verificationOfRegCertResult = objBAL.VerifyRegisteredCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);


                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                {
                    sw.WriteLine("2 :");
                    sw.WriteLine("verificationOfRegCertResult :" + verificationOfRegCertResult);
                    sw.Close();
                }


                if (!verificationOfRegCertResult.Equals(string.Empty))
                {

                   
                    PMGSYSession.Current.AppletErrMessage = verificationOfRegCertResult;
                    return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }

           
                // verify Certificate, if suucessful, then only store it in DB else flag error message
                string verificationResult = objBAL.VerifyCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NameAsPerCertificate);

                if (verificationResult.Equals(string.Empty))
                {

                    

                    Int32 AdminNdCode = Convert.ToInt32(model.PdfKey.Split('$')[0]); // uncomment
                    //Int32 AdminNdCode = 65;

                    string fileName = string.Empty;
                    string Path = string.Empty;
                    String xmlString = String.Empty;

                    string strFlag = "1";
                    string passwords = string.Empty;



                    if (strFlag.Equals("1"))
                    {
                        //Path = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;

                        ReatDAL pfmsDal = new ReatDAL();

                         xmlString = pfmsDal.GenerateDSCXml(AdminNdCode, out fileName, model.PdfKey.Split('$')[1], regModel.NameAsPerCertificate); // uncomment
                     //   xmlString = pfmsDal.GenerateDSCXml(AdminNdCode, out fileName, "D", "VAISHALI ARUNKUMAR JAISWAL");

                        TempData["FileName"] = fileName;
                        //Path = @"C:\AuthPdfFile\newpayrequest.xml";
                    }
                    else
                    {
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["REATDigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
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
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["REATDigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
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
                
                return Json(new { status = "error", message = "Error occurred while accessing pdf file.", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveDSCXml()
        {
            //using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["REATDigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
            //{
            //    sw.WriteLine(" in SaveDSCXml :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
            //    sw.Close();
            //}

            ReatDAL reatDal = new ReatDAL();
            DigSignBAL objBAL = new DigSignBAL();
            RegistrationData model = new RegistrationData();
            model.PdfKey = Request.Params["pdfKey"];
            String optype = model.PdfKey.Split('$')[1].ToString();
            ReatDAL pfmsdal = new ReatDAL();
            
            Boolean isXmlFlagSet = true;
            PFMSDAL1 pmfs = new PFMSDAL1();
            try
            {
                String StateShortCode = pmfs.GetStateShortName(PMGSYSession.Current.StateCode);
                String XmlFilePath = ConfigurationManager.AppSettings["REATDSCFilePath"].ToString() + StateShortCode + "\\";
                // <add key="XmlFilePath" value="C:\AuthXmlFile\" />
               // String XmlFilePath = ConfigurationManager.AppSettings["REATDSCFilePath"].ToString();
                if (!Directory.Exists(XmlFilePath))
                    Directory.CreateDirectory(XmlFilePath);

                string fileName1 = (String)TempData["FileName"];
                Request.Files[0].SaveAs(Path.Combine(XmlFilePath, fileName1));


                string opdb = reatDal.SaveDSCREAT(fileName1, PMGSYSession.Current.AdminNdCode, optype);


                if (isXmlFlagSet && opdb.Equals("1"))
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
                objDAL = new ReatDAL();

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
                ErrorLog.LogError(ex, "REAT.SaveDSCAcknowledgementXmlData()");
                return Json(new { success = false, message = "Error occured while mapping DSC acknowledgement data" });
            }
        }
        #endregion

        #region Sample Payment Sign XML for REAT

        public ActionResult TestSignXmlREAT()
        {
            string xml = string.Empty;
            string xmlFileName = string.Empty;

            string[] arrParams = null;
            try
            {
                ReatDAL objDAL = new ReatDAL();
                //xml = objDAL.GeneratePayXmlDAL();
                xml = objDAL.GeneratePayXmlDALTEST (out xml);

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
                ErrorLog.LogError(ex, "REAT.TestSignXmlREAT()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult SignEpaymentXmlTestREAT()
        {
            try
            {

                //DSCPFMSModel model = pfmsdal.ValidateDscPFMSDetails();
                return View();
            }
            catch (Exception ex)
            {
             //   ErrorLog.LogError(ex, "REAT.SignEpaymentXmlTestREAT()");
                return Json(string.Empty);
            }
        }


        [HttpPost]
        public ActionResult GetXmlTempREAT()
        {
         
            Int64 bill_ID;
            DigSignBAL objBAL = new DigSignBAL();
            try
            {
                RegistrationData model = new RegistrationData();
                
                //string verificationResult = string.Empty;
                // model.PdfKey = "905";

                  model.PdfKey = Request.Params["pdfKey"];
                  model.CertificateBase64 = string.IsNullOrEmpty(Request.Params["certificate"]) ? string.Empty : Request.Params["certificate"].Trim();
                  model.CertificateChainBase64 = string.IsNullOrEmpty(Request.Params["certificateChain"]) ? string.Empty : Request.Params["certificateChain"].Trim();

                  if (string.IsNullOrEmpty(model.CertificateBase64) || string.IsNullOrEmpty(model.CertificateChainBase64))
                  {
                      using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["REATDigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                      {
                          sw.WriteLine("Certificate details from client to server are null or empty." + DateTime.Now.ToString() + " UserName -" +
                              PMGSYSession.Current.UserName);
                          sw.Close();
                      }
                      return Json(new { message = "Certificate details from client to server are null or empty." });
                  }

                  RegisterDSCModel regModel = new RegisterDSCModel();
                  regModel = objBAL.GetDetailsToRegisterDSCREAT();

                  //verify certificate equality with Authorized Signatory's registered Certificate
                  string verificationOfRegCertResult = objBAL.VerifyRegisteredCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);

                  if (!verificationOfRegCertResult.Equals(string.Empty))
                  {
                      using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["REATDigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                      {
                          sw.WriteLine("verificationOfRegCertResult is NULL or empty" + DateTime.Now.ToString() + DateTime.Now.ToString() + " UserName -" +
                              PMGSYSession.Current.UserName);
                          sw.Close();
                      }

                      return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                          JsonRequestBehavior.AllowGet);
                  }

                 //  verify Certificate, if suucessful, then only store it in DB else flag error message
                  string verificationResult = objBAL.VerifyCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NameAsPerCertificate);
                
                 
                if (verificationResult.Equals(string.Empty))
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["REATDigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    {
                        sw.WriteLine("verificationResult success " + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                        sw.Close();
                    }

                    Int32 AdminNdCode = Convert.ToInt32(model.PdfKey);
                    string fileName = string.Empty;
                    string Path = string.Empty;
                    String xmlString = String.Empty;

                    //string strFlag = "1";//commemnted on 29-oct-2021
                    string passwords = string.Empty;

                    //if (strFlag.Equals("1"))
                    //{
                        //Path = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;
                        ReatDAL pfmsDal = new ReatDAL();

                        //xmlString = pfmsDal.GenerateDSCXml(AdminNdCode, out fileName);
                        xmlString = pfmsDal.GeneratePayXmlDALTEST(out fileName);
                        TempData["FileName"] = fileName;
                        //Path = @"C:\AuthPdfFile\newpayrequest.xml";
                    //}
                    //else
                    //{
                        
                    //    return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty }, JsonRequestBehavior.AllowGet);
                    //}
                    //if (System.IO.File.Exists(Path))
                    if (!String.IsNullOrEmpty(xmlString))
                    {
                        //var fileByteArr = System.IO.File.ReadAllBytes(Path);
                        var fileByteArr = System.Text.Encoding.ASCII.GetBytes(xmlString);
                        string fileBase64Str = Convert.ToBase64String(fileByteArr);

                        passwords = "abc";
                        string encParam = PMGSY.Common.Cryptography.AESEncrypt(passwords);
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["REATDigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
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
                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["REATDigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + "GetXmlTempREAT()");
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
        public ActionResult SaveXmlTempREAT()
        {
           

            DigSignBAL objBAL = new DigSignBAL();
            RegistrationData model = new RegistrationData();
            model.PdfKey = Request.Params["pdfKey"];

            ReatDAL pfmsdal = new ReatDAL();
           
            try
            {
                // <add key="XmlFilePath" value="C:\AuthXmlFile\" />
                String XmlFilePath = ConfigurationManager.AppSettings["REATXmlSignFilePath"].ToString();
                if (!Directory.Exists(XmlFilePath))
                    Directory.CreateDirectory(XmlFilePath);

               
                string fileName1 = (String)TempData["FileName"];
                Request.Files[0].SaveAs(Path.Combine(XmlFilePath, fileName1 + ".xml"));
                return Json(new { status = "success", message = "Document signed successfully." });

             
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["REATDigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + "SaveXmlTempREAT()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Json(new { message = "Error occurred while saving signed Document." });
            }
        }
        #endregion

        #region Payment Acknowledgement REAT
        [HttpGet]
        public ViewResult GetPaymentAckLayout()  //Added by Aditi Shree 30 March 2020
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetPaymentAckLayout()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SavePaymentAcknowledgementXmlData() //Added by Aditi Shree 1 April 2020
        {
            CommonFunctions comm = new CommonFunctions();
            string filePath = string.Empty;
            bool isRecordExists = false;
            HttpPostedFileBase file = null;
            bool status = false;
            try
            {
                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "Please select a xml file" });
                }
                //filePath = ConfigurationManager.AppSettings["PFMS_VALIDATE_XML"];
                objDAL = new ReatDAL();

                #region Multiple File
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        file = Request.Files[i];

                        XElement doc = XElement.Load(file.InputStream);
                        XNamespace ns = doc.GetDefaultNamespace(); //http://cpsms.nic.in/Beneficiary/ContractorPaymentStatus

                        status = objDAL.SavePaymentAcknowlegementDetails(doc, file.FileName, out isRecordExists);
                        if (status == false && isRecordExists == false)
                        {
                            break;
                        }
                    }
                    if (status == false && isRecordExists == false)
                    {
                        return Json(new { success = false, message = "Error in REAT mapping Payment acknowledgement data for file : " + file.FileName });
                    }
                }
                #endregion
                return Json(new { success = true, message = "Payment acknowledgement data mapped successfully" });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.SavePaymentAcknowledgementXmlData()");
                return Json(new { success = false, message = "Error occured while mapping Payment acknowledgement data" });
            }
        }
     
        
        
        
        #endregion

        #region Bank Acknowledgement for REAT
        [HttpGet]
        public ViewResult GetBankStatusLayout()  //Added by Aditi Shree on 15 April 2020
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetBankStatusLayout()");
                return null;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveBankPaymentStatusXmlData() //Added by Aditi Shree 17 April 2020
        {
            CommonFunctions comm = new CommonFunctions();
            string filePath = string.Empty;
            bool isRecordExists = false;
            HttpPostedFileBase file = null;
            bool status = false;
            try
            {
                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "Please select a xml file" });
                }
                objDAL = new ReatDAL();

                #region Multiple File
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        file = Request.Files[i];

                        XElement doc = XElement.Load(file.InputStream);
                        XNamespace ns = doc.GetDefaultNamespace(); //http://cpsms.nic.in/Beneficiary/ContractorPaymentStatus

                        status = objDAL.SaveBankAcknowlegementDetails(doc, file.FileName, out isRecordExists);

                        if (status == false && isRecordExists == false)
                        {
                            break;
                        }
                    }
                    if (status == false && isRecordExists == false)
                    {
                        return Json(new { success = false, message = "Error in mapping Bank response for file : " + file.FileName });
                    }
                }
                #endregion
                return Json(new { success = true, message = " Bank acknowledgement data mapped successfully" });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.SaveBankPaymentStatusXmlData()");
                return Json(new { success = false, message = "Error occured while mapping Bank acknowledgement data" });
            }
        }
        #endregion

        #region Payment Sign XML

        #region VIKKY
        [HttpPost]
        [Audit]
        public ActionResult SignSecondlevelSuccessEPaymentREATXml()
        {
            int MastConId = 0, ConAccountId = 0;
            try
            {
                ReatDAL pfmsdal = new ReatDAL();
                DSCREATModel model = pfmsdal.ValidateDscREATDetailsForSnaToHolding();

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

                model.IsValidContractor = pfmsdal.ValidateREATContractor(model.mastConId, model.conAccountId);

                // comment below line for deployment
                model.IsValidContractor = true;
                /*model.IsAccountNumberAvailable = true; model.IsIFSCAvailable = true; 
                model.IsInitPartyAvailable = true; model.IsEmailAvailable = true; model.IsValidContractor = true;*/

                return View("SignSecondlevelSuccessEPaymentREATXml", model);
            }
            catch (Exception ex)
            {
                return Json(string.Empty);
            }
        }

        [HttpPost]
        public ActionResult GetSecondlevelSuccessXml()
        {

            ReatDAL pfmsDal = new ReatDAL();
            Int64 bill_ID;
            DigSignBAL objBAL = new DigSignBAL();
            try
            {

                RegistrationData model = new RegistrationData();

                //model.PdfKey = 905 + "$" + 33516509.ToString() + "$" + 19020.ToString() + "$" + 13149.ToString();
                model.PdfKey = Request.Params["pdfKey"];
                bill_ID = Convert.ToInt64(model.PdfKey.Split('$')[1]);

                #region commented on 03-02-2023
                #region Payment Validation

                if (pfmsDal.IsSecondlevelSuccessPaymentExists(bill_ID))
                {
                    PMGSYSession.Current.AppletErrMessage = "Cannot make duplicate payment";
                    return Json(new { status = "error", message = "Cannot make duplicate payment", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                   JsonRequestBehavior.AllowGet);
                }
                #endregion
                #endregion

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
                regModel = objBAL.GetDetailsToRegisterDSCREAT();

                //verify certificate equality with Authorized Signatory's registered Certificate
                string verificationOfRegCertResult = objBAL.VerifyRegisteredCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);

                if (!verificationOfRegCertResult.Equals(string.Empty))
                {
                    PMGSYSession.Current.AppletErrMessage = verificationOfRegCertResult;
                    return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }

                // verify Certificate, if suucessful, then only store it in DB else flag error message
                string verificationResult = objBAL.VerifyCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NameAsPerCertificate);



                // string verificationResult = string.Empty;
                if (verificationResult.Equals(string.Empty))
                {
                    Int32 AdminNdCode = Convert.ToInt32(model.PdfKey.Split('$')[0]);
                    string fileName = string.Empty;
                    int runningCnt = 0;
                    string Path = string.Empty;
                    String xmlString = String.Empty;

                    //string strFlag = "1";
                    string passwords = string.Empty;


                    //if (strFlag.Equals("1"))
                    //{
                    //Path = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;

                    //xmlString = pfmsDal.GenerateDSCXml(AdminNdCode, out fileName);

                    PFMSDownloadPaymentXMLViewModel payModel = new PFMSDownloadPaymentXMLViewModel();

                    payModel.stateCode = PMGSYSession.Current.StateCode;
                    //payModel.agencyCode = PMGSYSession.Current.AdminNdCode;
                    payModel.generationDate = DateTime.Now.ToString();
                    payModel.FileType = "H";
                    payModel.billId = Convert.ToInt64(model.PdfKey.Split('$')[1]);
                    xmlString = pfmsDal.GenerateSecondLevelSuccessPaymentXMLDAL(payModel, out fileName, out runningCnt);
                    TempData["FileName"] = fileName;
                    TempData["RunningCount"] = runningCnt;

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
                    sw.WriteLine("Method : " + "GetSecondlevelSuccessXml()");
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
        public ActionResult SaveAccountHoldingPaymentXml()
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
            {
                sw.WriteLine(" in Reat Module SavePdf :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                sw.Close();
            }

            PaymentBAL objPaymentBAL = new PaymentBAL();

            DigSignBAL objBAL = new DigSignBAL();
            RegistrationData model = new RegistrationData();
            model.PdfKey = Request.Params["pdfKey"];

            PFMSDAL1 pfmsdal = new PFMSDAL1();

            try
            {

                String XmlFilePath = ConfigurationManager.AppSettings["REATHoldingAccountXmlSignFilePath"].ToString();
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


                //Request.Files[0].SaveAs(Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName));
                #endregion
                string fileName1 = (String)TempData["FileName"];
                int runningCount = Convert.ToInt32(TempData["RunningCount"]);
                string result = string.Empty;
                if (!string.IsNullOrEmpty(fileName1) && runningCount > 0)
                {
                    result = pfmsdal.InsertHoldingAccountEpaymentMailDetailsPFMS(Convert.ToInt64(model.PdfKey.Split('$')[1]), fileName1 + ".xml", Convert.ToInt32(model.PdfKey.Split('$')[2]), Convert.ToInt32(model.PdfKey.Split('$')[3]), runningCount);
                }
                if (result.Equals("1"))
                {

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



        [HttpPost]
        [Audit]
        public ActionResult SignEPaymentREATXml()
        {
            int MastConId = 0, ConAccountId = 0;
            try
            {
                ReatDAL pfmsdal = new ReatDAL();
                DSCREATModel model = pfmsdal.ValidateDscREATDetails();

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
                model.IsValidContractor = pfmsdal.ValidateREATContractor(model.mastConId, model.conAccountId);

             // comment below line for deployment 
                model.IsAccountNumberAvailable = true; model.IsIFSCAvailable = true; model.IsInitPartyAvailable = true; model.IsEmailAvailable = true; model.IsValidContractor = true;


                return View("SignEPaymentREATXml", model);
            }
            catch (Exception ex)
            {
                return Json(string.Empty);
            }
        }

        // [HttpPost]
        //public ActionResult GetXml()
        //{
          
        //    ReatDAL pfmsDal = new ReatDAL();
        //    Int64 bill_ID;
        //    DigSignBAL objBAL = new DigSignBAL();
        //    try
        //    {

        //        RegistrationData model = new RegistrationData();
        //        model.PdfKey = Request.Params["pdfKey"];
               
        //        model.CertificateBase64 = string.IsNullOrEmpty(Request.Params["certificate"]) ? string.Empty : Request.Params["certificate"].Trim();
        //        model.CertificateChainBase64 = string.IsNullOrEmpty(Request.Params["certificateChain"]) ? string.Empty : Request.Params["certificateChain"].Trim();

        //        #region Payment Validation
        //        bill_ID = Convert.ToInt64(model.PdfKey.Split('$')[1]);
        //        if (pfmsDal.IsPaymentExists(bill_ID))
        //        {
        //            PMGSYSession.Current.AppletErrMessage = "Cannot make duplicate payment";
        //            return Json(new { status = "error", message = "Cannot make duplicate payment", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
        //           JsonRequestBehavior.AllowGet);
        //        }
        //        #endregion

        //        if (string.IsNullOrEmpty(model.CertificateBase64) || string.IsNullOrEmpty(model.CertificateChainBase64))
        //        {
        //            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
        //            {
        //                sw.WriteLine("Certificate details from client to server are null or empty." + DateTime.Now.ToString() + " UserName -" +
        //                    PMGSYSession.Current.UserName);
        //                sw.Close();
        //            }
        //            PMGSYSession.Current.AppletErrMessage = "Certificate details from client to server are null or empty.";
        //            return Json(new { message = "Certificate details from client to server are null or empty." });
        //        }

        //        RegisterDSCModel regModel = new RegisterDSCModel();
        //        regModel = objBAL.GetDetailsToRegisterDSCREAT();

        //        //verify certificate equality with Authorized Signatory's registered Certificate
        //        string verificationOfRegCertResult = objBAL.VerifyRegisteredCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);

        //        if (!verificationOfRegCertResult.Equals(string.Empty))
        //        {

        //            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
        //            {
        //                sw.WriteLine("verificationOfRegCertResult is NULL or empty" + DateTime.Now.ToString() + DateTime.Now.ToString() + " UserName -" +
        //                    PMGSYSession.Current.UserName);
        //                sw.Close();
        //            }
        //            PMGSYSession.Current.AppletErrMessage = verificationOfRegCertResult;
        //            return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
        //                JsonRequestBehavior.AllowGet);
        //        }

        //        // verify Certificate, if suucessful, then only store it in DB else flag error message
        //        string verificationResult = objBAL.VerifyCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NameAsPerCertificate);

        //        if (verificationResult.Equals(string.Empty))
        //        {

        //            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
        //            {
        //                sw.WriteLine("verificationResult success " + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
        //                sw.Close();
        //            }

        //            Int32 AdminNdCode = Convert.ToInt32(model.PdfKey.Split('$')[0]);
        //            string fileName = string.Empty;
        //            int runningCnt = 0;
        //            string Path = string.Empty;
        //            String xmlString = String.Empty;

        //            string strFlag = "1";
        //            string passwords = string.Empty;



        //            if (strFlag.Equals("1"))
        //            {
        //                //Path = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;

        //                //xmlString = pfmsDal.GenerateDSCXml(AdminNdCode, out fileName);

        //                PFMSDownloadPaymentXMLViewModel payModel = new PFMSDownloadPaymentXMLViewModel();

        //                payModel.stateCode = PMGSYSession.Current.StateCode;
        //                //payModel.agencyCode = PMGSYSession.Current.AdminNdCode;
        //                payModel.generationDate = DateTime.Now.ToString();
        //                payModel.FileType = "N";
        //                payModel.billId = Convert.ToInt64(model.PdfKey.Split('$')[1]);

        //                xmlString = pfmsDal.GeneratePaymentXMLDAL(payModel, out fileName, out runningCnt);
        //                TempData["FileName"] = fileName;
        //                TempData["RunningCount"] = runningCnt;
        //                //Path = @"C:\AuthPdfFile\newpayrequest.xml";
        //            }
        //            else
        //            {
        //                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
        //                {
        //                    sw.WriteLine("verificationResult is  NULL or empty" + DateTime.Now.ToString() + " UserName -" +
        //                    PMGSYSession.Current.UserName);
        //                    sw.Close();
        //                }
        //                PMGSYSession.Current.AppletErrMessage = verificationResult;
        //                return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
        //                    JsonRequestBehavior.AllowGet);
        //            }
        //            //if (System.IO.File.Exists(Path))
        //            if (!String.IsNullOrEmpty(xmlString))
        //            {
        //                //var fileByteArr = System.IO.File.ReadAllBytes(Path);
        //                var fileByteArr = System.Text.Encoding.ASCII.GetBytes(xmlString);
        //                string fileBase64Str = Convert.ToBase64String(fileByteArr);

        //                passwords = "abc";
        //                string encParam = PMGSY.Common.Cryptography.AESEncrypt(passwords);
        //                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
        //                {
        //                    sw.WriteLine("encParam" + encParam + "  " + DateTime.Now.ToString() + " UserName -" +
        //                   PMGSYSession.Current.UserName);
        //                    sw.Close();
        //                }

        //                return Json(new
        //                {
        //                    status = "success",
        //                    password = encParam,
        //                    message = string.Empty,
        //                    fileBase64String = fileBase64Str,
        //                    encHashOfBase64String =
        //                        string.Empty
        //                }, JsonRequestBehavior.AllowGet);

        //                // return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
        //                //JsonRequestBehavior.AllowGet);

        //            }
        //            else
        //            {
        //                PMGSYSession.Current.AppletErrMessage = verificationResult;
        //                return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
        //                    JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else
        //        {
        //            PMGSYSession.Current.AppletErrMessage = verificationResult;
        //            return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
        //                JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
        //        {
        //            sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
        //            sw.WriteLine("Method : " + "Get Xml()");
        //            sw.WriteLine("Exception Message : " + ex.Message);
        //            sw.WriteLine("Exception : " + ex.StackTrace);
        //            sw.WriteLine("____________________________________________________");
        //            sw.Close();
        //        }
        //        return Json(new { status = "error", message = "Error occurred while accessing pdf file.", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
        //            JsonRequestBehavior.AllowGet);

        //    }

        //}


        [HttpPost]
        public ActionResult GetXml()
        {

            ReatDAL pfmsDal = new ReatDAL();
            Int64 bill_ID;
            DigSignBAL objBAL = new DigSignBAL();
            try
            {

                RegistrationData model = new RegistrationData();

                //model.PdfKey = 905 + "$" + 33516509.ToString() + "$" + 19020.ToString() + "$" + 13149.ToString();
               
               
                model.PdfKey = Request.Params["pdfKey"];
                bill_ID = Convert.ToInt64(model.PdfKey.Split('$')[1]);

                #region Payment Validation

                if (pfmsDal.IsPaymentExists(bill_ID))
                {
                    PMGSYSession.Current.AppletErrMessage = "Cannot make duplicate payment";
                    return Json(new { status = "error", message = "Cannot make duplicate payment", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                   JsonRequestBehavior.AllowGet);
                }
                #endregion

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
                regModel = objBAL.GetDetailsToRegisterDSCREAT();

                //verify certificate equality with Authorized Signatory's registered Certificate
                string verificationOfRegCertResult = objBAL.VerifyRegisteredCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);

                if (!verificationOfRegCertResult.Equals(string.Empty))
                {

                   
                    PMGSYSession.Current.AppletErrMessage = verificationOfRegCertResult;
                    return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }

                // verify Certificate, if suucessful, then only store it in DB else flag error message
                string verificationResult = objBAL.VerifyCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NameAsPerCertificate);
                


               // string verificationResult = string.Empty;


                if (verificationResult.Equals(string.Empty))
                {

                  

                    Int32 AdminNdCode = Convert.ToInt32(model.PdfKey.Split('$')[0]);
                    string fileName = string.Empty;
                    int runningCnt = 0;
                    string Path = string.Empty;
                    String xmlString = String.Empty;

                    //string strFlag = "1";
                    string passwords = string.Empty;



                    //if (strFlag.Equals("1"))
                    //{
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
                        //SaveXml1(xmlString);
                    //}
                    //else
                    //{
                    //    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                    //    {
                    //        sw.WriteLine("verificationResult is  NULL or empty" + DateTime.Now.ToString() + " UserName -" +
                    //        PMGSYSession.Current.UserName);
                    //        sw.Close();
                    //    }
                    //    PMGSYSession.Current.AppletErrMessage = verificationResult;
                    //    return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                    //        JsonRequestBehavior.AllowGet);
                    //}
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



        public void SaveXml1(string xmlString)
        {
           


            PaymentBAL objPaymentBAL = new PaymentBAL();

            DigSignBAL objBAL = new DigSignBAL();
            RegistrationData model = new RegistrationData();
            //model.PdfKey = Request.Params["pdfKey"];

            model.PdfKey = 905 + "$" + 33516509.ToString() + "$" + 1024.ToString() + "$" + 11963.ToString();

            PFMSDAL1 pfmsdal = new PFMSDAL1();
          
            try
            {

                String XmlFilePath = ConfigurationManager.AppSettings["REATXmlSignFilePath"].ToString();
                
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
               

                //Request.Files[0].SaveAs(Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName));
                #endregion
                string fileName1 = (String)TempData["FileName"];
                int runningCount = Convert.ToInt32(TempData["RunningCount"]);

                
                string result = string.Empty;
                if (!string.IsNullOrEmpty(fileName1) && runningCount > 0)
                {
                    result = pfmsdal.InsertEpaymentMailDetailsPFMS(Convert.ToInt64(model.PdfKey.Split('$')[1]), fileName1 + ".xml", Convert.ToInt32(model.PdfKey.Split('$')[2]), Convert.ToInt32(model.PdfKey.Split('$')[3]), runningCount);
                }
                if (result.Equals("1"))
                {
                   
                  //  Request.Files[0].SaveAs(Path.Combine(finalPath, fileName1 + ".xml"));

                    System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
                    xdoc.LoadXml(xmlString);
                    xdoc.Save(Path.Combine(finalPath, fileName1 + ".xml"));
                    
                }
                else if (result.Equals("-1"))
                {
                   
                }
                else
                {
                    //System.IO.File.Delete(Path.Combine(finalPath, fileName1 + ".xml"));
                    
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
              
            }
        }

        [HttpPost]
        public ActionResult SaveXml()
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
            {
                sw.WriteLine(" in Reat Module SavePdf :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                sw.Close();
            }

            PaymentBAL objPaymentBAL = new PaymentBAL();

            DigSignBAL objBAL = new DigSignBAL();
            RegistrationData model = new RegistrationData();
            model.PdfKey = Request.Params["pdfKey"];

            PFMSDAL1 pfmsdal = new PFMSDAL1();
          
            try
            {

                String XmlFilePath = ConfigurationManager.AppSettings["REATXmlSignFilePath"].ToString();
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
               

                //Request.Files[0].SaveAs(Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName));
                #endregion
                string fileName1 = (String)TempData["FileName"];
               // string fileName1 = "0037EATPAYREQ040520203";
                int runningCount = Convert.ToInt32(TempData["RunningCount"]);

                
                string result = string.Empty;
                if (!string.IsNullOrEmpty(fileName1) && runningCount > 0)
                {
                    result = pfmsdal.InsertEpaymentMailDetailsPFMS(Convert.ToInt64(model.PdfKey.Split('$')[1]), fileName1 + ".xml", Convert.ToInt32(model.PdfKey.Split('$')[2]), Convert.ToInt32(model.PdfKey.Split('$')[3]), runningCount);
                }
                if (result.Equals("1"))
                {
                   
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


        #region REAT Opening Balance
        [HttpGet]
        public ViewResult GetOpeningBalanceLayout()  //Added by Aditi Shree on 22 April 2020
        {
            REATOpeningBalanceViewModel model = new REATOpeningBalanceViewModel();
            try
            {
                objDAL = new ReatDAL();
                model = objDAL.GetAccBankDetails();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetOpeningBalanceLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GenerateXMLForOpeningBalance() //Added by Aditi Shree on 27 April 2020
        {
            string xmlString = string.Empty;
            string fileName = string.Empty;
            string message = string.Empty;
            double OpBalance = 0.0;
            DateTime OpDate = DateTime.Now;

            try
            {
                ReatDAL reatDAL = new ReatDAL();
                OpBalance = Convert.ToDouble(Request.Params["Opening Balance"]);
                OpDate = Convert.ToDateTime(Request.Params["Opening Date"]);
                xmlString = reatDAL.GenerateXMLOpeningBalanceDAL(OpBalance, OpDate, out fileName);
                TempData["FileName"] = fileName;
                if ((xmlString == "" || TempData["FileName"] == ""))
                {
                    return Json(new { message = "Error occured while saving Opening Balance details." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { message = "Details are saved successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GenerateXMLForOpeningBalance");
                return Json(new { message = "Error occured while saving Opening Balance details." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region Approve DSC
        [HttpGet]
        public ActionResult GetApproveDSCLayout()  //Added by Priyanka on 14 MAY 2020
        {
            ApproveDSCViewModel model = new ApproveDSCViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {

                model.lstState = comm.PopulateStates(true);
                model.lstAgency = new List<SelectListItem>();
                model.lstAgency.Insert(0, new SelectListItem { Text = "Select Agency", Value = "-1" });
                model.lstPIU = new List<SelectListItem>();
                model.lstPIU.Insert(0, new SelectListItem { Text = "Select PIU", Value = "-1" });

                //Added by Ajinkya 
                model.SRRDA_LIST = new List<SelectListItem>();
                model.SRRDA_LIST.Insert(0, new SelectListItem { Text = "Select SRRDA", Value = "-1" });



                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetApproveDSCLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult PopulatePIUbystatecode(int stateCode, int agencyCode)
        {
            try
            {

                int statecode = Convert.ToInt32(Request.Params["stateCode"]);
                int agencycode = Convert.ToInt32(Request.Params["agencyCode"]);
                ReatDAL reatDALobj = new ReatDAL();
                int? mast_parent_code = reatDALobj.PopulatePIUbystatecodeDAL(statecode, agencycode);
                CommonFunctions objCommonFunctions = new CommonFunctions();
                if (mast_parent_code != null)
                {
                    List<SelectListItem> lstPIU = objCommonFunctions.PopulateDPIUOfSRRDA((int)mast_parent_code);
                    return Json(lstPIU);
                }
                else
                {
                    throw new Exception("Mast_parent_ID not found");

                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.PopulatePIUbystatecode()");
                return null;
            }

        }

        //Ajinkya
        [HttpPost]
        public JsonResult PopulateSRRDA(int stateCode, int agencyCode)
        {
            ApproveDSCViewModel model = new ApproveDSCViewModel();
            List<SelectListItem> getsrrda = new List<SelectListItem>();
            try
            {


                //int statecode = Convert.ToInt32(Request.Params["stateCode"]);
                //int agencycode = Convert.ToInt32(Request.Params["agencyCode"]);
                ReatDAL reatDALobj = new ReatDAL();
                getsrrda = reatDALobj.PopulateSRRDA(stateCode, agencyCode);

                return Json(getsrrda);



            }
            catch (Exception)
            {
                return null;
            }


        }


        [HttpPost]
        public ActionResult GetAuthoriseSignatoryDetails(int AdminNDCode)
        {
            try
            {
                ReatDAL reatDALobj = new ReatDAL();
                int Admin_ND_Code = Convert.ToInt32(Request.Params["AdminNDCode"]);
                ApproveDSCViewModel approvedscmodel = reatDALobj.GetAuthoriseSignatoryDetailsDAL(Admin_ND_Code);


                if (approvedscmodel != null)
                {
                    return Json(approvedscmodel);
                }
                return null;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetAuthoriseSignatoryDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetApproveDSC(int AdminOfficerCode, int ADMINNDCODE, long FileID)
        {
            try
            {
                ReatDAL reatDALobj = new ReatDAL();

                string sucessmsg = reatDALobj.ApproveDSCDAL(AdminOfficerCode, ADMINNDCODE, FileID);

                if (sucessmsg != null)
                {
                    return Json(sucessmsg);
                }
                return null;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetApproveDSC()");
                return null;
            }
        }
        #endregion


        #region REAT OB Acknowledgement
        [HttpGet]
        public ViewResult GetOpeningBalanceAckLayout()  //Added by Aditi Shree on 8 May 2020
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetBankStatusLayout()");
                return null;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveOpeningBalanceXmlData() //Added by Aditi Shree 6 May 2020
        {
            CommonFunctions comm = new CommonFunctions();
            string filePath = string.Empty;
            bool isRecordExists = false;
            HttpPostedFileBase file = null;
            bool status = false;
            try
            {
                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "Please select a xml file" });
                }
                objDAL = new ReatDAL();

                #region Multiple File
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        file = Request.Files[i];

                        XElement doc = XElement.Load(file.InputStream);
                        XNamespace ns = doc.GetDefaultNamespace(); //http://cpsms.nic.in/EATOpeningBalance

                        status = objDAL.SaveOBAcknowlegement(doc, file.FileName, out isRecordExists);

                        if (status == false && isRecordExists == false)
                        {
                            break;
                        }
                    }
                    if (status == false && isRecordExists == false)
                    {
                        return Json(new { success = false, message = "Error in mapping OB response for file : " + file.FileName });
                    }
                }
                #endregion
                return Json(new { success = true, message = " Acknowledgement data mapped successfully" });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.SaveOpeningBalanceXmlData()");
                return Json(new { success = false, message = "Error occured while mapping  acknowledgement data" });
            }
        }
        #endregion



        #region Fund Receipt Acknowledgement

        [HttpGet]
        public ViewResult GetFundReceiptAckLayout()  //Added by Aditi Shree on 26 June 2020
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetFundReceiptAckLayout()");
                return null;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveFundReceiptAck() //Added by Aditi Shree 25 June 2020
        {
            CommonFunctions comm = new CommonFunctions();
            string filePath = string.Empty;
            bool isRecordExists = false;
            HttpPostedFileBase file = null;
            bool status = false;
            try
            {
                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "Please select a xml file" });
                }
                objDAL = new ReatDAL();

                #region Multiple File
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        file = Request.Files[i];

                        XElement doc = XElement.Load(file.InputStream);
                        XNamespace ns = doc.GetDefaultNamespace(); //http://cpsms.nic.in/EATPaymentAck

                        status = objDAL.SaveFRAcknowlegement(doc, file.FileName, out isRecordExists);

                        if (status == false && isRecordExists == false)
                        {
                            break;
                        }
                    }
                    if (status == false && isRecordExists == false)
                    {
                        return Json(new { success = false, message = "Error in mapping FR response for file : " + file.FileName });
                    }
                }
                #endregion
                return Json(new { success = true, message = " Acknowledgement data mapped successfully" });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.SaveFundReceiptAck()");
                return Json(new { success = false, message = "Error occured while mapping  acknowledgement data" });
            }
        }
        #endregion


        #region Fund Receipt
        [HttpGet]
        public ViewResult GetFundReceiptLayout()
        {
            CommonFunctions comObj = new CommonFunctions();
            try
            {
                Int32 Month = DateTime.Now.Month;
                Int32 Year = DateTime.Now.Year;
                if (Request.Params["Month"] != null)
                {
                    Month = Convert.ToInt32(Request.Params["Month"]);
                    Year = Convert.ToInt32(Request.Params["Year"]);
                }
                else if (PMGSYSession.Current.AccMonth != 0)
                {
                    Month = Convert.ToInt32(PMGSYSession.Current.AccMonth);
                    Year = Convert.ToInt32(PMGSYSession.Current.AccYear);
                }
                else
                {
                    PMGSYSession.Current.AccMonth = Convert.ToInt16(DateTime.Now.Month);
                    PMGSYSession.Current.AccYear = Convert.ToInt16(DateTime.Now.Year);
                }

                ViewBag.ddlMonth = comObj.PopulateMonths(Month);
                ViewBag.ddlYear = comObj.PopulateYears(Year);

                return View("GetFundReceiptLayout");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetFundReceiptLayout()");
                return null;
            }
        }//Added by Aditi Shree on 16 June 2020

        [HttpPost]
        [Audit]
        public ActionResult GetFundReceiptList(FormCollection homeFormCollection)
        {
            FundReceiptModel objModel = new FundReceiptModel();
            ReatDAL receiptDAL = new ReatDAL();
            long totalRecords;
            bool isReceiptEnable = false;

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            objModel.Month = Convert.ToInt16(Request.Params["month"]);
            objModel.Year = Convert.ToInt16(Request.Params["year"]);
            objModel.AdminNdCode = PMGSYSession.Current.AdminNdCode;

            PMGSYEntities dbContext = new PMGSYEntities();
            REAT_FUND_RECEIPT_CONFIGURATION config = new REAT_FUND_RECEIPT_CONFIGURATION();
            config = dbContext.REAT_FUND_RECEIPT_CONFIGURATION.Where(m => m.ADMIN_ND_CODE >= objModel.AdminNdCode).FirstOrDefault();

            if (config != null)
            {
                if (config.MONTH + (config.YEAR * 12) <= objModel.Month + (objModel.Year * 12))
                {
                    objModel.TransId = String.IsNullOrEmpty(Request.Params["transType"]) ? (short)0 : Convert.ToInt16(Request.Params["transType"].Split('$')[0]);
                    //objModel.page = Convert.ToInt32(homeFormCollection["page"]) - 1;
                    //objModel.rows = Convert.ToInt32(homeFormCollection["rows"]);
                    //objModel.sidx = homeFormCollection["sidx"].ToString();
                    //objModel.sord = homeFormCollection["sord"].ToString();
                    objModel.FundType = PMGSYSession.Current.FundType;
                    objModel.LevelId = PMGSYSession.Current.LevelId;

                    var jsonData = new
                    {
                        rows = receiptDAL.FundReceiptList(objModel, Convert.ToInt32(homeFormCollection["page"]) - 1, Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], out totalRecords),
                        total = totalRecords <= Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(homeFormCollection["rows"]) + 1,
                        page = Convert.ToInt32(homeFormCollection["page"]),
                        records = totalRecords,
                        isReceiptEnable = true
                    };
                    return Json(jsonData);

                }
                else
                {
                    return Json(new { isReceiptEnable = false, message = "Fund receipts are only enabled for " + System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(config.MONTH) + ", " + config.YEAR + " and greater  date. Previous date receipt cannot be sent to PFMS" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { message = "No record found" }, JsonRequestBehavior.AllowGet);
            }

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public JsonResult GenerateXMLForFundReceipt(String parameter, String hash, String key)
        {
            string xmlString = string.Empty;
            string fileName = string.Empty;
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                ReatDAL reatDAL = new ReatDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int billId = Convert.ToInt32(decryptedParameters["BillID"]);

                if (decryptedParameters.Count() > 0)
                {
                    xmlString = reatDAL.GenerateXMLFundReceiptDAL(billId, out fileName);
                }
                TempData["FileName"] = fileName;
                if ((xmlString == "" || TempData["FileName"] == ""))
                {
                    return Json(new { success = false, message = "Error occured while saving Fund Receipt details." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = "Details are Sent to PFMS" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GenerateXMLForFundReceipt");
                return Json(new { message = "Error occured while saving  Fund Receipt details." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region REAT Beneficiary Updation

        [HttpGet]
        public ActionResult VendorUpdationLayout()
        {
            REATDownloadXMLViewModel model = new REATDownloadXMLViewModel();
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
                ErrorLog.LogError(ex, "Reat.VendorUpdationLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetBeneficiaryUpdation(FormCollection formCollection)
        {
            int stateCode = 0, districtCode = 0, agencyCode = 0;
            try
            {
                objDAL = new ReatDAL();

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
                    rows = objDAL.GetBeneficiaryUpdationDAL(stateCode, districtCode, agencyCode, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetBeneficiaryDetails()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult EditContractor(string id)
        {

            try
            {
                CommonFunctions com = new CommonFunctions();
                PMGSYEntities dbContext = new PMGSYEntities();

                int ConId = Convert.ToInt32(id.Split(',')[0].Trim());
                int DetailId = Convert.ToInt32(id.Split(',')[1].Trim());
                int StateCode = Convert.ToInt32(id.Split(',')[2].Trim());
                int AgencyCode = Convert.ToInt32(id.Split(',')[3].Trim());

                MASTER_CONTRACTOR contractorDetails = dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_ID == ConId).FirstOrDefault();
                string ContractorName = contractorDetails.MAST_CON_FNAME + " " + contractorDetails.MAST_CON_MNAME + " " + contractorDetails.MAST_CON_LNAME;
                REAT_CONTRACTOR_DETAILS reatContractor = dbContext.REAT_CONTRACTOR_DETAILS.Where(a => a.MAST_CON_ID == ConId && a.DETAIL_ID == DetailId && a.reat_STATUS == "A").FirstOrDefault();

                ContractorUpdate model = new ContractorUpdate();

                model.MAST_BANK_NAME = reatContractor.MAST_BANK_NAME; // Added on 15 Feb 2021
                model.lstBankNames = com.PopulatePFMSBankNames(); //   // Added on 15 Feb 2021


                model.StateCode = StateCode;
                model.AgencyCode = AgencyCode;
                model.AccountID = reatContractor.MAST_ACCOUNT_ID;
                model.UserID = PMGSYSession.Current.UserId;
                model.IPAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                model.DETAIL_ID = reatContractor.DETAIL_ID;
                model.BankAccountNum = reatContractor.MAST_ACCOUNT_NUMBER;
                model.ContractorCompanyName = contractorDetails.MAST_CON_COMPANY_NAME;
                model.ContractorID = ConId;
                model.ContractorName = ContractorName;
                model.PAN = contractorDetails.MAST_CON_PAN;
                model.reatContractorName = reatContractor.reat_CON_NAME; // This Field is fetched in edit mode to update
                model.reatIFSC = reatContractor.reat_IFSC_CODE;  // This Field is fetched in edit mode to update
                return PartialView("EditContractor", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT().REAT().EditContractor");
                ModelState.AddModelError(String.Empty, "Details not exist.");
                return PartialView("EditContractor", new ContractorUpdate());
            }
        }

        public ActionResult GenerateXMLForBeneficiaryUpdation(string[] Contractors)
        {
            string xml = string.Empty;
            string xmlFileName = string.Empty;
            string message = string.Empty;
            int recordCount = 0;
            string stateShortCode = string.Empty;

            REATDownloadBeneficiaryUpdateXML model = null;
            try
            {

                model = new REATDownloadBeneficiaryUpdateXML();

                model.StateCode = string.IsNullOrEmpty(Request.Params["StateCode"]) ? 0 : Convert.ToInt32(Request.Params["StateCode"]);
                model.AgencyCode = string.IsNullOrEmpty(Request.Params["AgencyCode"]) ? 0 : Convert.ToInt32(Request.Params["AgencyCode"]);
                model.ContractorID = string.IsNullOrEmpty(Request.Params["ContractorID"]) ? 0 : Convert.ToInt32(Request.Params["ContractorID"]);


                model.AccountID = string.IsNullOrEmpty(Request.Params["AccountID"]) ? 0 : Convert.ToInt32(Request.Params["AccountID"]);
                model.reatIFSC = string.IsNullOrEmpty(Request.Params["reatIFSC"]) ? "NA" : Request.Params["reatIFSC"];
                // model.reatIFSC = model.reatIFSC.Split(',')[1].Trim(); // Updated IFSC Code from Textbox is taken
                model.reatContractorName = string.IsNullOrEmpty(Request.Params["reatContractorName"]) ? "NA" : Request.Params["reatContractorName"];


                model.UserID = string.IsNullOrEmpty(Request.Params["UserID"]) ? 0 : Convert.ToInt32(Request.Params["UserID"]);
                model.IPAddress = string.IsNullOrEmpty(Request.Params["IPAddress"]) ? "NA" : Request.Params["IPAddress"];
                model.DETAIL_ID = string.IsNullOrEmpty(Request.Params["DETAIL_ID"]) ? 0 : Convert.ToInt32(Request.Params["DETAIL_ID"]);
                model.BankName = string.IsNullOrEmpty(Request.Params["MAST_BANK_NAME"]) ? "NA" : Request.Params["MAST_BANK_NAME"];


                objDAL = new ReatDAL();
                bool result = objDAL.GenerateXMLForBeneficiaryUpdationDAL(model, ref message);

                return Json(new { message = message.Trim() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GenerateXMLForBeneficiaryUpdation");
                return Json(new { message = "Error occured while Reat XML generation" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region DSC File Verification
        [HttpGet]
        public ViewResult DSCFileVerifyLayout()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.DSCFileVerifyLayout()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DSCTemperVerification()
        {
            CommonFunctions comm = new CommonFunctions();
            string filePath = string.Empty;
            bool isFileTempered = false;
            HttpPostedFileBase file = null;
            bool status = false;
            bool isSignatureAvailable = false;
            try
            {
                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "Please select a xml file" });
                }
                // Generate a signing key.
                RSA Key = RSA.Create();
                #region Multiple File
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        file = Request.Files[i];
                        XmlDocument doc = new XmlDocument();
                        doc.Load(file.InputStream);
                        XNamespace ns = "http://cpsms.nic.in/EATPaymentAck"; //doc.GetDefaultNamespace();

                        //status = objDAL.SaveFRAcknowlegement(doc, file.FileName, out isRecordExists);
                        // Sign the XML that was just created and save it in a new file.
                        //SignXmlFile(doc,file.FileName, "signedExample.xml", Key);

                        // Verify the signature of the signed XML.
                        //isFileTempered = VerifyXmlFile("SignedExample.xml", Key);
                        isFileTempered = VerifyXmlFile(doc, file.FileName, Key, ref isSignatureAvailable);
                        if (isFileTempered == false && isSignatureAvailable == true)
                        {
                            break;
                        }
                        if (isFileTempered == false && isSignatureAvailable == false)
                        {
                            return Json(new { success = true, message = "Signature not found in XML." });
                        }
                    }
                    if (isFileTempered == true && isSignatureAvailable == true)
                    {
                        return Json(new { success = false, message = "Uploaded file : " + file.FileName + " is tempered." });
                    }
                }
                #endregion
                return Json(new { success = true, message = "No Tempering found in XML file." });
            }
            catch (FormatException ex1)
            {
                ErrorLog.LogError(ex1, "REAT.DSCTemperVerification()");
                return Json(new { success = false, message = "Uploaded file : " + file.FileName + " is tempered." });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.DSCTemperVerification()");
                return Json(new { success = false, message = "Error occured while verifying data" });
            }
        }
        /* <summary>
         * Sign an XML file and save the signature in a new file. This method does not 
         * save the public key within the XML file.  This file cannot be verified unless  
          *the verifying code has the key with which it was signed.
         </summary>*/
        public static void SignXmlFile(XmlDocument doc, string FileName, string SignedFileName, RSA Key)
        {
            // Create a new XML document.
            //XmlDocument doc = new XmlDocument();

            // Load the passed XML file using its name.
            //doc.Load(new XmlTextReader(FileName));

            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(doc);

            // Add the key to the SignedXml document. 
            signedXml.SigningKey = Key;

            // Create a reference to be signed.
            Reference reference = new Reference();
            reference.Uri = "";

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            if (doc.FirstChild is XmlDeclaration)
            {
                doc.RemoveChild(doc.FirstChild);
            }

            // Save the signed XML document to a file specified
            // using the passed string.
            XmlTextWriter xmltw = new XmlTextWriter(SignedFileName, new UTF8Encoding(false));
            doc.WriteTo(xmltw);
            xmltw.Close();
        }

        /*<summary>
         * Verify the signature of an XML file against an asymmetric  algorithm and return the result
         </summary>.*/
        public static Boolean VerifyXmlFile(XmlDocument doc, String Name, RSA Key, ref bool isSignatureAvailable)
        {
            // Create a new XML document.
            //XmlDocument doc = new XmlDocument();

            // Load the passed XML file into the document. 
            //doc.Load(Name);

            // Create a new SignedXml object and pass it the XML document class.
            SignedXml signedXml = new SignedXml(doc);

            // Find the "Signature" node and create a new XmlNodeList object.
            XmlNodeList nodeList = doc.GetElementsByTagName("Signature");

            if (nodeList.Count > 0)
            {
                isSignatureAvailable = true;
                // Load the signature node.
                signedXml.LoadXml((XmlElement)nodeList[0]);
            }
            else
            {
                isSignatureAvailable = false;
                return false;
            }
            // Check the signature and return the result.
            return signedXml.CheckSignature(Key);
        }

        #endregion

        #region Add IFSC code
        [HttpPost]
        public ActionResult PopulateDistinctPFMSBankNames(string search)
        {
            PMGSYEntities dbContext = null;
            try
            {

                dbContext = new PMGSYEntities();

                List<AddIfscCodeModel> lstBank = dbContext.PFMS_BANK_MASTER.Where(x => x.PFMS_BANK_NAME.Contains(search)).Select(x => new AddIfscCodeModel
                {

                    BankName = x.PFMS_BANK_NAME
                }).ToList();

                return new JsonResult { Data = lstBank, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateDistinctPFMSBankNames()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }




        [HttpGet]
        public ActionResult GetIFSCCodeLayout()
        {
            AddIfscCodeModel model = new AddIfscCodeModel();

            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.lstState = comm.PopulateStates(true);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetIFSCCodeLayout()");
                return null;
            }
        }




        [HttpPost]
        public ActionResult AddIFSCCode(AddIfscCodeModel mIfsc)
        {
            try
            {
                String oprnType = "I";
                AddIfscCodeModel custModel = new AddIfscCodeModel();
                if (ModelState.IsValid || mIfsc != null)
                {

                    ReatDAL objDAL = new ReatDAL();
                    custModel = objDAL.AddIFSCcode(mIfsc, ref oprnType);

                }

                return Json(new { custModel = custModel, oprnType = oprnType }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(ex.Data);
            }


        }


        #endregion



    }
}
