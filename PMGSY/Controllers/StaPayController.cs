﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models;
using PMGSY.Models.STAPayment;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.BAL.STAPayment;
using PMGSY.Models.Proposal;
using System.Globalization;
using System.Configuration;
using System.IO;
using System.Data.Entity.Validation;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class StaPayController : Controller
    {
        STAPaymentBAL objStaPaymentBAL;
        private PMGSYEntities db = new PMGSYEntities();
        [Audit]
        public ActionResult ListStaPayment()
        {
            ProposalFilterViewModel filterViewModel = new ProposalFilterViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            List<SelectListItem> lstProposalTypes = new List<SelectListItem>();
            lstProposalTypes.Insert(0, new SelectListItem { Value = "0", Text = "Both" });
            lstProposalTypes.Insert(1, new SelectListItem { Value = "P", Text = "Road" });
            lstProposalTypes.Insert(2, new SelectListItem { Value = "L", Text = "Bridge" });
            filterViewModel.STATES = objCommonFuntion.PopulateStates();
            filterViewModel.BATCHS = objCommonFuntion.PopulateBatch();
            //filterViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
            filterViewModel.PROPOSAL_TYPES = lstProposalTypes;
            filterViewModel.IMS_YEAR = DateTime.Now.Year;
            filterViewModel.Years = PopulateYear(DateTime.Now.Year, true, false);
            filterViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency();
            return View("ListStaPayment", filterViewModel);
        }

        [Audit]
        public ActionResult StaPaymentList(string id)
        {
            STAPaymentViewModel staPayment = new STAPaymentViewModel();

            string[] arrParameters = id.Split('$');
            var ListStaPayment = db.IMS_STA_GET_PAYMENT(Convert.ToInt32(arrParameters[0]), Convert.ToInt32(arrParameters[1]), Convert.ToInt32(arrParameters[2]), Convert.ToInt32(arrParameters[3]), arrParameters[4], Convert.ToInt32(arrParameters[5])).ToList();
            if (ListStaPayment.Any())
            {
                staPayment.TOTAL_SCRUTNIZED_AMOUNT = ListStaPayment.Sum(a => a.SANCTION_AMOUNT);
                staPayment.MAST_STATE_NAME = ListStaPayment.Select(a => a.MAST_STATE_NAME).First();
                //staPayment.TOT_HON_OF_SCRUTINY = Math.Round(CalculateTotalHonorariumOfScrutiny(Convert.ToDecimal(staPayment.TOTAL_SCRUTNIZED_AMOUNT))/100, 2);
                staPayment.TOT_HON_OF_SCRUTINY = CalculateTotalHonorariumOfScrutiny(Convert.ToDecimal(staPayment.TOTAL_SCRUTNIZED_AMOUNT)) / 100;
                staPayment.TOTAL_HON_AMOUNT_IN_RUPEES = Math.Round(CalculateTotalHonorariumOfScrutiny(Convert.ToDecimal(staPayment.TOTAL_SCRUTNIZED_AMOUNT)) * 10, 2);
                staPayment.TOT_HON_MIN = CalculateTotalHonorariumMinimum(Convert.ToDecimal(staPayment.TOTAL_SCRUTNIZED_AMOUNT), Convert.ToDecimal(staPayment.TOT_HON_OF_SCRUTINY));
            }
            return View("StaPayment", staPayment);
        }

        public decimal CalculateTotalHonorariumOfScrutiny(decimal SanctionAmountinLakhs)
        {
            ///Added by SAMMED A. PATIL on 16MAR2018 for TOTAL_SCRUTNIZED_AMOUNT < 500 Jharkhand 2015 issue
            if (SanctionAmountinLakhs < 500)
            {
                return SanctionAmountinLakhs = 30;
            }
            else if (SanctionAmountinLakhs >= 500 && SanctionAmountinLakhs <= 5000)
            {
                return SanctionAmountinLakhs = SanctionAmountinLakhs * Convert.ToDecimal(0.03);
            }
            else if (SanctionAmountinLakhs > 5000 && SanctionAmountinLakhs <= 20000)
            {
                return SanctionAmountinLakhs = SanctionAmountinLakhs * Convert.ToDecimal(0.0225);
            }
            else if (SanctionAmountinLakhs > 20000)
            {
                return SanctionAmountinLakhs = SanctionAmountinLakhs * Convert.ToDecimal(0.015);
            }
            return Convert.ToDecimal(Math.Round(SanctionAmountinLakhs, 2));
        }

        public decimal CalculateTotalHonorariumMinimum(decimal TOTAL_SCRUTNIZED_AMOUNT, decimal TOT_HON_OF_SCRUTINY)
        {
            ///Added by SAMMED A. PATIL on 16MAR2018 for TOTAL_SCRUTNIZED_AMOUNT < 500 Jharkhand 2015 issue
            if (TOTAL_SCRUTNIZED_AMOUNT < 500)
            {
                return 30000;
            }
            else if (TOTAL_SCRUTNIZED_AMOUNT >= 500 && TOTAL_SCRUTNIZED_AMOUNT <= 5000)
            {
                TOT_HON_OF_SCRUTINY = 30000;
                if (TOT_HON_OF_SCRUTINY >= 30000 && TOT_HON_OF_SCRUTINY < 150000)
                {
                    return TOT_HON_OF_SCRUTINY;

                }
                else
                {
                    return 30000;
                }
            }
            else if (TOTAL_SCRUTNIZED_AMOUNT > 5000 && TOTAL_SCRUTNIZED_AMOUNT <= 20000)
            {
                TOT_HON_OF_SCRUTINY = 150000;
                if (TOT_HON_OF_SCRUTINY >= 150000 && TOT_HON_OF_SCRUTINY < 450000)
                {
                    return TOT_HON_OF_SCRUTINY;
                }
                else
                {
                    return 150000;
                }
            }
            else if (TOTAL_SCRUTNIZED_AMOUNT > 20000)
            {
                TOT_HON_OF_SCRUTINY = 450000;
                if (TOT_HON_OF_SCRUTINY >= 450000)
                {
                    return TOT_HON_OF_SCRUTINY;
                }
                else
                {
                    return 450000;
                }
            }
            return TOT_HON_OF_SCRUTINY;
        }

        [HttpPost]
        [Audit]
        public ActionResult ListStaPaymentDetails(FormCollection formCollection)
        {
            objStaPaymentBAL = new STAPaymentBAL();
            STAPaymentTotalModel model = new STAPaymentTotalModel();
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            //Added By Abhishek kamble 30-Apr-2014 Start
            using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 30-Apr-2014 end

            int MAST_STATE_CODE = Convert.ToInt32(Request.Params["MAST_STATE_ID"]);
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            decimal TOTAL_HON_AMOUNT_IN_RUPEES = Convert.ToDecimal(Request.Params["TOTAL_HON_AMOUNT_IN_RUPEES"]) / 100;
            decimal TOT_HON_MIN = Convert.ToDecimal(Request.Params["TOT_HON_MIN"]);
            decimal TOT_HON_OF_SCRUTINY = Convert.ToDecimal(Request.Params["TOT_HON_OF_SCRUTINY"]) * 100000;
            decimal maxValue = (TOT_HON_OF_SCRUTINY > TOT_HON_MIN) ? TOT_HON_OF_SCRUTINY : TOT_HON_MIN;
            int PMGSY_SCHEME = Convert.ToInt32(Request.Params["PMGSY_SCHEME"]);
            maxValue = maxValue / 100;
            int totalRecords;

            var jsonData = new
            {
                rows = objStaPaymentBAL.GetSTAPaymentListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, maxValue, PMGSY_SCHEME, out model),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
                TotalModel = model
            };
            return Json(jsonData);
        }

        [HttpPost]
        [Audit]
        public ActionResult ListGeneratedInvoice(FormCollection formCollection)
        {
            objStaPaymentBAL = new STAPaymentBAL();
            STAPaymentTotalViewModel model = new STAPaymentTotalViewModel();
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            //Added By Abhishek kamble 30-Apr-2014 Start
            using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 30-Apr-2014 end
            int MAST_STATE_CODE = Convert.ToInt32(Request.Params["MAST_STATE_ID"]);
            Int16 IMS_YEAR = Convert.ToInt16(Request.Params["IMS_YEAR"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            string STA_SANCTIONED_BY = Request.Params["STA_SANCTIONED_BY"];
            string STA_INSTITUTE_NAME = Request.Params["STA_INSTITUTE_NAME"];
            decimal HON_AMOUNT = Convert.ToDecimal(Request.Params["HON_AMOUNT"]);
            int PMGSY_SCHEME = Convert.ToInt32(Request.Params["PMGSY_SCHEME"]);
            int totalRecords;
            var jsonData = new
            {
                rows = objStaPaymentBAL.GetSTAInvoiceListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, STA_SANCTIONED_BY, STA_INSTITUTE_NAME, HON_AMOUNT, PMGSY_SCHEME, out model),
                total = 0 <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = 0,
                TotalModel = model
            };
            return Json(jsonData);
        }

        [HttpGet]
        [Audit]
        public ActionResult AddStaInvoiceDetails()
        {
            int MAST_STATE_CODE = Convert.ToInt32(Request.Params["MAST_STATE_ID"]);
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            string STA_SANCTIONED_BY = Request.Params["STA_SANCTIONED_BY"];
            string STA_INSTITUTE_NAME = Request.Params["STA_INSTITUTE_NAME"];
            decimal HON_AMOUNT = Convert.ToDecimal(Request.Params["HON_AMOUNT"]);
            int PMGSY_SCHEME = Convert.ToInt32(Request.Params["PMGSY_SCHEME"]);
            decimal honAmountTillDate = 0;

            STAInvoiceViewModel staInvoiceViewModel = new STAInvoiceViewModel();
            staInvoiceViewModel.MAST_STATE_CODE = MAST_STATE_CODE;
            staInvoiceViewModel.IMS_YEAR = Convert.ToInt16(IMS_YEAR);
            staInvoiceViewModel.IMS_BATCH = IMS_BATCH;
            staInvoiceViewModel.IMS_STREAM = IMS_STREAMS;
            staInvoiceViewModel.IMS_PROPOSAL_TYPE = IMS_PROPOSAL_TYPE;
            staInvoiceViewModel.STA_SANCTIONED_BY = STA_SANCTIONED_BY;
            staInvoiceViewModel.SAS_ABBREVATION = STA_INSTITUTE_NAME;
            staInvoiceViewModel.PMGSY_SCHEME = PMGSY_SCHEME;

            if (
                db.IMS_GENERATED_INVOICE.Where(
                                        c => c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                        c.IMS_YEAR == IMS_YEAR &&
                                        c.IMS_STREAM == IMS_STREAMS &&
                                        c.IMS_BATCH == IMS_BATCH &&
                                        c.IMS_PROPOSAL_TYPE == IMS_PROPOSAL_TYPE &&
                                        c.STA_SANCTIONED_BY == STA_SANCTIONED_BY &&
                                        c.MAST_PMGSY_SCHEME == PMGSY_SCHEME
                                        ).Any())
            {
                honAmountTillDate = db.IMS_GENERATED_INVOICE.Where(
                                            c => c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                            c.IMS_YEAR == IMS_YEAR &&
                                            c.IMS_STREAM == IMS_STREAMS &&
                                            c.IMS_BATCH == IMS_BATCH &&
                                            c.IMS_PROPOSAL_TYPE == IMS_PROPOSAL_TYPE &&
                                            c.STA_SANCTIONED_BY == STA_SANCTIONED_BY &&
                                            c.MAST_PMGSY_SCHEME == PMGSY_SCHEME
                                            ).Sum(c => c.HONORARIUM_AMOUNT);

            }

            staInvoiceViewModel.HONORARIUM_AMOUNT = HON_AMOUNT;
            staInvoiceViewModel.Balance_Amount = HON_AMOUNT - Convert.ToDecimal(honAmountTillDate);

            var Master_tax = (from c in db.MASTER_TAXES
                              where c.MAST_TDS_ID == (db.MASTER_TAXES.Select(a => a.MAST_TDS_ID).Max())
                              select c);
            MASTER_TAXES master_taxes = new MASTER_TAXES();
            foreach (var item in Master_tax)
            {
                master_taxes.MAST_TDS_ID = item.MAST_TDS_ID;
                master_taxes.MAST_TDS_SC = item.MAST_TDS_SC;
                master_taxes.MAST_TDS = item.MAST_TDS;
                master_taxes.SERVICE_TAX = item.SERVICE_TAX;
            }
            staInvoiceViewModel.Per_Sc = master_taxes.MAST_TDS_SC;
            staInvoiceViewModel.Per_Tds = master_taxes.MAST_TDS;
            staInvoiceViewModel.MAST_TDS_ID = master_taxes.MAST_TDS_ID;
            //staInvoiceViewModel.Per_Service_Tax = db.ADMIN_TECHNICAL_AGENCY.Any(m=>m.ADMIN_TA_NAME == STA_INSTITUTE_NAME && m.ADMIN_TA_SERVICE_TAX == null) ? 0 : Convert.ToDecimal(master_taxes.SERVICE_TAX) ;
            //new change done by vikram as suggested by Srinivasa sir on 17-Aug-2015
            staInvoiceViewModel.Per_Service_Tax = Convert.ToDecimal(master_taxes.SERVICE_TAX);
            //  staInvoiceViewModel.ServiceTaxNo = db.ADMIN_TECHNICAL_AGENCY.Where(m => m.ADMIN_TA_NAME == STA_INSTITUTE_NAME).Select(m => m.ADMIN_TA_SERVICE_TAX).FirstOrDefault() == null ? "-" : db.ADMIN_TECHNICAL_AGENCY.Where(m => m.ADMIN_TA_NAME == STA_INSTITUTE_NAME).Select(m => m.ADMIN_TA_SERVICE_TAX).FirstOrDefault();
            //  staInvoiceViewModel.ServiceTaxNo =db.ADMIN_TECHNICAL_AGENCY.Any(m=>m.ADMIN_TA_NAME == STA_INSTITUTE_NAME && m.ADMIN_TA_SERVICE_TAX == null) ? "0" : Convert.ToDecimal(master_taxes.SERVICE_TAX).ToString() ;
            staInvoiceViewModel.ServiceTaxNo = db.ADMIN_TECHNICAL_AGENCY.Any(m => m.ADMIN_TA_NAME == STA_INSTITUTE_NAME) ? db.ADMIN_TECHNICAL_AGENCY.Where(m => m.ADMIN_TA_NAME == STA_INSTITUTE_NAME).Select(s => s.ADMIN_TA_SERVICE_TAX).FirstOrDefault() : "0";

            return View(staInvoiceViewModel);
        }

        [Audit]
        [HttpPost]
        public JsonResult AddStaInvoiceDetails(STAInvoiceViewModel staInvoiceViewModel)
        {
            if (ModelState.IsValid)
            {
                objStaPaymentBAL = new STAPaymentBAL();
                string Status = objStaPaymentBAL.AddStaInvoiceDetailsBAL(staInvoiceViewModel);

                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
            }
        }

        /// <summary>
        /// populate the year 
        /// </summary>
        /// <returns></returns>

        public List<SelectListItem> PopulateYear(int SelectedYear = 0, bool populateFirstItem = true, bool isAllYearsSelected = false)
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            if (populateFirstItem && isAllYearsSelected == false)
            {
                item.Text = "Select Year";
                item.Value = "0";
                item.Selected = true;
                lstYears.Add(item);
            }
            if (populateFirstItem && isAllYearsSelected)
            {
                item.Text = "All Years";
                item.Value = "-1";
                item.Selected = true;
                lstYears.Add(item);
            }
            for (int i = 2000; i < DateTime.Now.Year + 1; i++)
            {
                item = new SelectListItem();
                item.Text = i + " - " + (i + 1);
                item.Value = i.ToString();
                //if (i == DateTime.Now.Year && SelectedYear == 0)
                //{
                //    //item.Selected = true;
                //}
                //if (i == SelectedYear)
                //{
                //   // item.Selected = true;
                //}
                lstYears.Add(item);
            }

            return lstYears;
        }

        public ActionResult PreviewSTAPayment(string id)
        {
            try
            {
                int state = Convert.ToInt32(id.Split('$')[0]);
                int year = Convert.ToInt32(id.Split('$')[1]);
                int batch = Convert.ToInt32(id.Split('$')[2]);
                int collaboration = Convert.ToInt32(id.Split('$')[3]);
                string proposalType = id.Split('$')[4];
                decimal? TOTAL_HON_MIN = Convert.ToDecimal(id.Split('$')[5]);
                int scheme = Convert.ToInt32(id.Split('$')[6]);
                decimal? ScrHon = Convert.ToDecimal(id.Split('$')[7]);
                decimal? ScrAmount = Convert.ToDecimal(id.Split('$')[8]);
                string CollabName = db.MASTER_FUNDING_AGENCY.Where(m => m.MAST_FUNDING_AGENCY_CODE == collaboration).Select(m => m.MAST_FUNDING_AGENCY_NAME).FirstOrDefault();
                string StateName = db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == state).Select(m => m.MAST_STATE_NAME).FirstOrDefault();
                decimal? MinHonValue = Convert.ToDecimal(id.Split('$')[5]) * 100000;
                decimal? MinHon = Convert.ToDecimal(id.Split('$')[9]);


                Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
                rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);

                System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("vMASTSTATECODE", state.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("vIMSYEAR", year == null ? "0" : year.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("vIMSBATCH", batch == null ? "0" : batch.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("vIMSCOLLABORATION", collaboration == null ? "0" : collaboration.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("vIMSPROPOSALTYPE", proposalType == null ? "%" : proposalType));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("vIMSPMGSYSCHEME", scheme == null ? "1" : scheme.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("MinHon", MinHon == null ? "0.00" : MinHon.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("ScrHon", ScrHon == null ? "0.00" : ScrHon.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("ScrAmount", ScrAmount == null ? "0.00" : ScrAmount.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("CollabName", CollabName == null ? "NA" : CollabName.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", StateName == null ? "NA" : StateName.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("MinHonValue", MinHonValue == null ? "0.00" : MinHonValue.ToString()));


                //Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(@"ndcad\ommasadmin", "Ndcsp@123");
                // Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(@"Administrator", "Admin@321");               
                Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Username"], System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Password"]);
                rview.ServerReport.ReportServerCredentials = irsc;
                rview.ServerReport.ReportPath = "/PMGSYCitizen/STA_Payment_Abstract_Report";
                // rview.ServerReport.ReportPath = "/PMGSYReports/STA_PAYMENT_REPORTS";
                rview.ServerReport.SetParameters(paramList);
                string mimeType, encoding, extension, deviceInfo;
                string[] streamids;
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string format = "PDF"; //Desired format goes here (PDF, Excel, or Image)

                deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>True</SimplePageHeaders>" + "</DeviceInfo>";
                //var fileName = "STA_Payment.pdf";
                var fileName = db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == state).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault() + "_" + year + "_Batch" + batch + ".pdf";
                byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var fileName = (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.Year + "-" + (model.Year + 1)) + "_BATCH" + model.Batch + "_" + (model.CollaborationName) + "_SCHEME" + model.PMGSYScheme;
                //string filePath = model.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"].ToString() + fileName + ".pdf" : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"].ToString() + fileName + ".pdf";
                //System.IO.File.WriteAllBytes(filePath, bytes);
                //BinaryWriter binaryWriter = new BinaryWriter(System.IO.File.Open( ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"] + fileName + ".pdf", FileMode.Create, FileAccess.ReadWrite));
                //binaryWriter.Write(bytes);
                Response.Clear();
                //if (format == "PDF")
                //{
                //    Response.ContentType = "application/pdf";
                //    Response.AddHeader("Content-disposition", "filename=" + fileName + ".pdf");

                //}

                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = fileName,

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,

                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(bytes, "application/pdf");
            }
            catch (Exception)
            {
                return null;
            }
        }


        [Audit]
        public ActionResult StaPaymentReport(string id)
        {
            objStaPaymentBAL = new STAPaymentBAL();
            STAInvoiceViewModel staInvoiceModel = new STAInvoiceViewModel();
            try
            {
                staInvoiceModel = objStaPaymentBAL.StaPaymentReportBAL(Convert.ToInt32(id));
                return View(staInvoiceModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }
        }

        [Audit]
        //[HttpPost]
        public ActionResult StaPaymentSSRSReport(string id)
        {
            objStaPaymentBAL = new STAPaymentBAL();
            STAInvoiceViewModel staInvoiceModel = new STAInvoiceViewModel();
            try
            {

                staInvoiceModel = objStaPaymentBAL.StaPaymentReportBAL(Convert.ToInt32(id));
                //return View(staInvoiceModel); 
                //GenerateReport(staInvoiceModel);
                staInvoiceModel.IMS_INVOICE_ID = Convert.ToInt32(id);
                Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
                rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);

                System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("INVOICE_ID", staInvoiceModel.IMS_INVOICE_ID.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("RupeesInWord", staInvoiceModel.TOTAL_AMOUNT_WORDS == null ? "Rupees zero" : staInvoiceModel.TOTAL_AMOUNT_WORDS.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("SAS_ABBREVATION", staInvoiceModel.SAS_ABBREVATION == null ? "NA" : staInvoiceModel.SAS_ABBREVATION.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("HONORARIUM_AMOUNT", staInvoiceModel.HONORARIUM_AMOUNT_IND_FORMAT == null ? "0.00" : staInvoiceModel.HONORARIUM_AMOUNT_IND_FORMAT.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PENALTY_AMOUNT", staInvoiceModel.PENALTY_AMOUNT_IND_FORMAT == null ? "0.00" : staInvoiceModel.PENALTY_AMOUNT_IND_FORMAT.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("TDS_AMOUNT", staInvoiceModel.TDS_AMOUNT_IND_FORMAT == null ? "0.00" : staInvoiceModel.TDS_AMOUNT_IND_FORMAT.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("SC_AMOUNT", staInvoiceModel.SC_AMOUNT_IND_FORMAT == null ? "0.00" : staInvoiceModel.SC_AMOUNT_IND_FORMAT.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("INVOICE_GEN_DATE", staInvoiceModel.INVOICE_GEN_DATE == null ? "NA" : staInvoiceModel.INVOICE_GEN_DATE.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("TotalAmount", staInvoiceModel.TOTAL_AMOUNT_IND_FORMAT.ToString()));

                //Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(@"ndcad\ommasadmin", "Ndcsp@123");
                // Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(@"Administrator", "Admin@321");               
                Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Username"], System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Password"]);
                rview.ServerReport.ReportServerCredentials = irsc;
                rview.ServerReport.ReportPath = "/PMGSYCitizen/STA_PAYMENT_REPORTS";
                // rview.ServerReport.ReportPath = "/PMGSYReports/STA_PAYMENT_REPORTS";
                rview.ServerReport.SetParameters(paramList);
                string mimeType, encoding, extension, deviceInfo;
                string[] streamids;
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string format = "PDF"; //Desired format goes here (PDF, Excel, or Image)

                deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>True</SimplePageHeaders>" + "</DeviceInfo>";
                //var fileName = "STA_Payment.pdf";
                var fileName = staInvoiceModel.IMS_INVOICE_ID + ".pdf";
                byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var fileName = (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.Year + "-" + (model.Year + 1)) + "_BATCH" + model.Batch + "_" + (model.CollaborationName) + "_SCHEME" + model.PMGSYScheme;
                //string filePath = model.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"].ToString() + fileName + ".pdf" : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"].ToString() + fileName + ".pdf";
                //System.IO.File.WriteAllBytes(filePath, bytes);
                //BinaryWriter binaryWriter = new BinaryWriter(System.IO.File.Open( ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"] + fileName + ".pdf", FileMode.Create, FileAccess.ReadWrite));
                //binaryWriter.Write(bytes);
                Response.Clear();
                //if (format == "PDF")
                //{
                //    Response.ContentType = "application/pdf";
                //    Response.AddHeader("Content-disposition", "filename=" + fileName + ".pdf");

                //}

                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = fileName,

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,

                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(bytes, "application/pdf");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }
        }

        //Not used following function to generate report
        public bool GenerateReport(STAInvoiceViewModel model)
        {
            try
            {
                Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
                rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);

                System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("INVOICE_ID", model.IMS_INVOICE_ID.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("RupeesInWord", model.TOTAL_AMOUNT_WORDS == null ? "zero" : model.TOTAL_AMOUNT_WORDS.ToString()));
                // Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(@"ndcad\ommasadmin", "Ndcsp@123");
                Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(@"Administrator", "Admin@321");
                rview.ServerReport.ReportServerCredentials = irsc;
                rview.ServerReport.ReportPath = "/PMGSYReports/STA_PAYEMENT_REPORT";
                //rview.ServerReport.ReportPath = "/SampleLocalizedReport/SanctionedProposalList";
                rview.ServerReport.SetParameters(paramList);
                string mimeType, encoding, extension, deviceInfo;
                string[] streamids;
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string format = "PDF"; //Desired format goes here (PDF, Excel, or Image)

                deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>True</SimplePageHeaders>" + "</DeviceInfo>";
                var fileName = "SampleSTA";
                byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var fileName = (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.Year + "-" + (model.Year + 1)) + "_BATCH" + model.Batch + "_" + (model.CollaborationName) + "_SCHEME" + model.PMGSYScheme;
                //string filePath = model.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"].ToString() + fileName + ".pdf" : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"].ToString() + fileName + ".pdf";
                //System.IO.File.WriteAllBytes(filePath, bytes);
                //BinaryWriter binaryWriter = new BinaryWriter(System.IO.File.Open(model.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"] + fileName + ".pdf" : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"] + fileName + ".pdf", FileMode.Create, FileAccess.ReadWrite));
                //binaryWriter.Write(bytes);
                Response.Clear();
                if (format == "PDF")
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-disposition", "filename=" + fileName + ".pdf");

                }

                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = fileName,

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,

                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                //Response.OutputStream.Write(bytes, 0, bytes.Length);

                //Response.OutputStream.Flush();

                //Response.OutputStream.Close();

                //Response.Flush();

                //Response.Close();

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return false;
            }
        }

        #region STA Payment
        [Audit]
        public ActionResult ListStaPaymentInovice()
        {
            ProposalFilterViewModel filterViewModel = new ProposalFilterViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            List<SelectListItem> lstProposalTypes = new List<SelectListItem>();
            lstProposalTypes.Insert(0, new SelectListItem { Value = "0", Text = "Both" });
            lstProposalTypes.Insert(1, new SelectListItem { Value = "P", Text = "Road" });
            lstProposalTypes.Insert(2, new SelectListItem { Value = "L", Text = "Bridge" });
            filterViewModel.STATES = objCommonFuntion.PopulateStates();
            filterViewModel.BATCHS = objCommonFuntion.PopulateBatch();
            //filterViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
            filterViewModel.PROPOSAL_TYPES = lstProposalTypes;
            filterViewModel.IMS_YEAR = DateTime.Now.Year;
            filterViewModel.Years = PopulateYear(DateTime.Now.Year, true, false);
            filterViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency();
            return View("ListStaPaymentInovice", filterViewModel);
        }


        [HttpPost]
        [Audit]
        public ActionResult ListStaPaymentInoviceDetails(FormCollection formCollection)
        {
            objStaPaymentBAL = new STAPaymentBAL();
            STAPaymentTotalModel model = new STAPaymentTotalModel();
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            //Added By Abhishek kamble 30-Apr-2014 Start
            using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 30-Apr-2014 end

            int MAST_STATE_CODE = Convert.ToInt32(Request.Params["MAST_STATE_ID"]);
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            int PMGSY_SCHEME = Convert.ToInt32(Request.Params["PMGSY_SCHEME"]);
            int totalRecords;

            var jsonData = new
            {
                rows = objStaPaymentBAL.GetSTAPaymenInoviceListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, PMGSY_SCHEME),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
                TotalModel = model
            };
            return Json(jsonData);
        }


        public ActionResult StaPaymentDetail(string id)
        {

            objStaPaymentBAL = new STAPaymentBAL();
            Dictionary<string, string> decryptedParameters = null;
            string[] encryptedParams = id.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
            int invoiceCode = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"]);
            //STAInvoiceViewModel invoiceModel = objStaPaymentBAL.StaPaymentReportBAL(invoiceCode);
            //model.Invoice_Generate_DATE = invoiceModel.Invoice_Generate_DATE;
            //model.IMS_INVOICE_CODE = invoiceCode;
            //model.EncryptedIMS_Invoice_Code = id;
            ViewData["IMS_INVOICE_CODE"] = invoiceCode;
            ViewData["EncryptedIMS_Invoice_Code"] = URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + invoiceCode.ToString().Trim() });
            return View();
        }

        public ActionResult StaPaymentAdd(string id)
        {
            STAPayemntInvoiceModel model = new STAPayemntInvoiceModel();
            objStaPaymentBAL = new STAPaymentBAL();
            Dictionary<string, string> decryptedParameters = null;
            string[] encryptedParams = id.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
            int invoiceCode = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"]);
            STAInvoiceViewModel invoiceModel = objStaPaymentBAL.StaPaymentReportBAL(invoiceCode);
            model.Invoice_Generate_DATE = invoiceModel.Invoice_Generate_DATE;
            model.IMS_INVOICE_CODE = invoiceCode;
            model.EncryptedIMS_Invoice_Code = id;
            return View(model);
        }

        [Audit]
        public ActionResult GetSTAPaymentList(FormCollection formCollection)
        {

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            long totalRecords = 0;
            int IMS_INVOICE_Code = 0;
            objStaPaymentBAL = new STAPaymentBAL();

            try
            {
                //Dictionary<string, string> decryptedParameters = null;
                //string[] encryptedParams = formCollection["IMS_INVOICE_CODE"].Split('/');
                //decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                //IMS_INVOICE_Code = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"]);

                if (!string.IsNullOrEmpty(formCollection["IMS_INVOICE_CODE"]))
                {
                    IMS_INVOICE_Code = Convert.ToInt32(formCollection["IMS_INVOICE_CODE"]);
                }


                var jsonData = new
                {
                    rows = objStaPaymentBAL.GetSTAPaymentListBAL(IMS_INVOICE_Code, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Add Test result details
        /// </summary>
        /// <param name="sTAPayemntInvoiceViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddSTAPaymentDetails(STAPayemntInvoiceModel sTAPayemntInvoiceViewModel)
        {
            string message = string.Empty;
            objStaPaymentBAL = new STAPaymentBAL();

            try
            {
                if (ModelState.IsValid)
                {

                    if (objStaPaymentBAL.AddSTAPaymentDetailsBAL(sTAPayemntInvoiceViewModel, ref message))
                    {
                        message = message == string.Empty ? "STA Payment details saved successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        message = message == string.Empty ? "STA Payment details not saved." : message;
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {

                    string messages = string.Join("; ", ModelState.Values
                                           .SelectMany(x => x.Errors)
                                           .Select(x => x.ErrorMessage));

                    return PartialView("StaPaymentAdd", sTAPayemntInvoiceViewModel);
                }
            }
            catch (DbEntityValidationException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "STA Payment details not saved because " + ex.Message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get Test result Details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditSTAPaymentDetails(String parameter, String hash, String key)
        {
            try
            {
                objStaPaymentBAL = new STAPaymentBAL();

                Dictionary<string, string> decryptedParameters = null;

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int paymentCode = Convert.ToInt32(decryptedParameters["IMS_PAYMENT_ID"].ToString());
                    int imsInvoiceCode = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"].ToString());

                    STAPayemntInvoiceModel proposalAdditionalCostModel = objStaPaymentBAL.EditSTAPaymentDetailsBAL(paymentCode, imsInvoiceCode);
                    proposalAdditionalCostModel.IMS_INVOICE_CODE = imsInvoiceCode;

                    if (proposalAdditionalCostModel == null)
                    {
                        ModelState.AddModelError("", "STA Payment Details not exist.");
                        return PartialView("StaPaymentAdd", new TestResultViewModel());
                    }


                    return PartialView("StaPaymentAdd", proposalAdditionalCostModel);
                }
                return PartialView("StaPaymentAdd", new ProposalAdditionalCostModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError("", "STA Payment details not exist.");
                return PartialView("StaPaymentAdd", new ProposalAdditionalCostModel());
            }
        }


        /// <summary>
        /// Update Test Result details.
        /// </summary>
        /// <param name="proposalAdditionalCostModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult UpdateSTAPaymentDetails(STAPayemntInvoiceModel staPaymentViewModel)
        {
            string message = string.Empty;
            try
            {

                if (ModelState.IsValid)
                {
                    objStaPaymentBAL = new STAPaymentBAL();

                    if (objStaPaymentBAL.UpdateSTAPaymentDetailsBAL(staPaymentViewModel, ref message))
                    {
                        message = message == string.Empty ? "STA Payment details Updated successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        message = message == string.Empty ? "STA Payment details not updated." : message;
                        return Json(new { success = false, message = message });
                    }

                }
                else
                {
                    message = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                        );

                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (DbEntityValidationException ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Additional Cost details not saved because " + ex.Message;
                return Json(new { succes = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete Test Result details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteSTAPaymentDetails(string parameter, string hash, string key)
        {
            string message = string.Empty;

            try
            {
                Dictionary<string, string> decryptedParameters = null;

                objStaPaymentBAL = new STAPaymentBAL();

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int paymentCode = Convert.ToInt32(decryptedParameters["IMS_PAYMENT_ID"].ToString());
                    int imsInvoiceCode = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"].ToString());

                    if (objStaPaymentBAL.DeleteSTAPaymentDetailsBAL(paymentCode, imsInvoiceCode, ref message))
                    {
                        message = message == string.Empty ? "STA Payment details deleted successfully." : message;

                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "STA Payment details not deleted." : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                message = "An error occured while processing you request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = "An error occured while proccessing your request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult FinalizeSTAPaymentDetails(string parameter, string hash, string key)
        {
            string message = string.Empty;

            try
            {
                Dictionary<string, string> decryptedParameters = null;

                objStaPaymentBAL = new STAPaymentBAL();

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int paymentCode = Convert.ToInt32(decryptedParameters["IMS_PAYMENT_ID"].ToString());
                    int imsInvoiceCode = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"].ToString());

                    if (objStaPaymentBAL.FinalizeSTAPaymentDetailsBAL(paymentCode, imsInvoiceCode, ref message))
                    {
                        message = message == string.Empty ? "STA Payment details finalized successfully." : message;

                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "STA Payment details not finalized." : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                message = "An error occured while processing you request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = "An error occured while proccessing your request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// method to delete the generated invoice
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteGeneratedInvoice(int invoiceID)
        {
            try
            {
                if (db.IMS_STA_PAYMENTS.Any(m => m.IMS_INVOICE_ID == invoiceID))
                {
                    return Json(new { Success = false, ErrorMessage = "Please delete payment details before deleting invoice details." });
                }
                else
                {
                    IMS_GENERATED_INVOICE generatedInvoice = db.IMS_GENERATED_INVOICE.Find(invoiceID);
                    if (generatedInvoice != null)
                    {
                        generatedInvoice.USERID = PMGSYSession.Current.UserId;
                        generatedInvoice.IPADD = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                        db.Entry(generatedInvoice).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        db.IMS_GENERATED_INVOICE.Remove(generatedInvoice);
                        db.SaveChanges();
                    }

                    return Json(new { Success = true });
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false, ErrorMessage = "Error occurred while processing your request." });
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeFinalizeSTAPaymentDetails(string parameter, string hash, string key)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int paymentCode = Convert.ToInt32(decryptedParameters["IMS_PAYMENT_ID"].ToString());
                    int imsInvoiceCode = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"].ToString());

                    if (db.IMS_STA_PAYMENTS.Any(m => m.IMS_INVOICE_ID == imsInvoiceCode && m.IMS_PAYMENT_ID == paymentCode))
                    {
                        IMS_STA_PAYMENTS staPayment = db.IMS_STA_PAYMENTS.Where(m => m.IMS_PAYMENT_ID == paymentCode && m.IMS_INVOICE_ID == imsInvoiceCode).FirstOrDefault();
                        staPayment.IMS_PAYMENT_FINALIZE = "N";
                        staPayment.USERID = PMGSYSession.Current.UserId;
                        staPayment.IPADD = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                        db.Entry(staPayment).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Success = false, message = "Payment details not found" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Success = false, message = "Payment details not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false, message = "Error occurred while processing your request." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

    }
}
