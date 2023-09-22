
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Areas.QMSSRSReports.Models;
using PMGSY.Extensions;
using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Controllers;
using System.IO;
using System.Configuration;
using PMGSY.DAL.Master;
using PMGSY.Areas.QMSSRSReports.DAL;

namespace PMGSY.Areas.QMSSRSReports.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class QMSSRSReportsController : Controller
    {

        //
        // GET: /QMSSRSReports/QMSSRSReports/

        //
        // GET: /QMSSRSReports/QMSSRSReports/



        #region Grading and ATR's Report
        [HttpGet]
        public ActionResult QMATRLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                QM.FromYear = DateTime.Now.Year - 1;
                QM.ToYear = DateTime.Now.Year;

                QM.FromMonth = 1;
                QM.ToMonth = DateTime.Now.Month;

                QM.FromYearList = comm.PopulateYears(false);
                QM.ToYearList = comm.PopulateYears(false);
                QM.FromMonthList = comm.PopulateMonths(false);
                QM.ToMonthList = comm.PopulateMonths(false);

                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult QMATRReport(QualityReportsViewModel QM)
        {
            try
            {
                QM.Month = 2;
                QM.MonthList = new List<SelectListItem>();
                QM.MonthList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 25)
                    {
                        QM.Level = 1;
                    }
                    else if (PMGSYSession.Current.RoleCode == 8)
                    {
                        QM.Level = 2;
                    }

                    QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.State = PMGSYSession.Current.StateCode;

                    //QM.State = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;    
                    //QM.DISTRICT_NAME = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;

                    //QM.BLOCK_NAME = "All Blocks";

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Comparison and Grading Report
        [HttpGet]
        public ActionResult ComparisonandGradingLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                QM.Year = DateTime.Now.Year;
                QM.State = PMGSYSession.Current.StateCode;
                QM.StateList = comm.PopulateStates(true);
                QM.StateList.RemoveAt(0);
                QM.StateList.Insert(0, (new SelectListItem { Text = "Select", Value = "-1" }));
                if (PMGSYSession.Current.RoleCode == 8) //SQC
                {
                    QM.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, false, 0);
                    QM.DistrictList.RemoveAt(0);
                }
                else
                {
                    QM.DistrictList = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();

                }
                QM.DistrictList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                QM.YearList = comm.PopulateYears(false);
                QM.MonthList = comm.PopulateMonths(false);
                QM.MonthList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult ComparisonandGradingReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;

                    QM.StateName = QM.State == 0 ? "All States" : QM.StateName;
                    QM.DistName = QM.District == 0 ? "All Districts" : QM.DistName;

                    //QM.BLOCK_NAME = "All Blocks";

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Monthwise Inspection Report
        //[Audit]
        //public ActionResult QMMonthwiseInspectionsLayout()
        //{
        //    QualityReportsViewModel QM = new QualityReportsViewModel();
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        QM.Year = DateTime.Now.Year;
        //        QM.State = PMGSYSession.Current.StateCode;
        //        QM.StateList = comm.PopulateStates(true);
        //        QM.StateList.RemoveAt(0);
        //        QM.StateList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
        //        QM.YearList = comm.PopulateYears(false);
        //        return View(QM);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return null;
        //    }
        //    finally
        //    {
        //        if (comm != null)
        //        {
        //            comm.Dispose();
        //        }
        //    }
        //}
        public ActionResult QMMonthwiseInspectionsLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                QM.Year = DateTime.Now.Year;
                QM.State = PMGSYSession.Current.StateCode;
                QM.StateList = comm.PopulateStates(true);
                QM.StateList.RemoveAt(0);
                QM.StateList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                QM.YearList = comm.PopulateYears(false);
                QM.qmTypeList = new List<SelectListItem>();
                QM.qmTypeList.Add(new SelectListItem { Text = "Select", Value = "0", Selected = true });
                QM.qmTypeList.Add(new SelectListItem { Text = "NQM", Value = "I" });
                QM.qmTypeList.Add(new SelectListItem { Text = "SQM", Value = "S" });
                QM.qmType = "I";
                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        //[HttpPost]
        //public ActionResult QMMonthwiseInspectionsReport(QualityReportsViewModel QM)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

        //            QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;

        //            if (PMGSYSession.Current.StateName != null)
        //            {
        //                QM.StateName = PMGSYSession.Current.StateName;
        //            }
        //            else
        //            {
        //                QM.StateName = QM.State == 0 ? "All States" : QM.StateName;
        //            }
        //            QM.DistName = QM.District == 0 ? "All Districts" : QM.DistName;


        //            //QM.BLOCK_NAME = "All Blocks";

        //            return View(QM);
        //        }
        //        else
        //        {
        //            string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
        //            return null;
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        [HttpPost]
        public ActionResult QMMonthwiseInspectionsReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.StateName != null)
                    {
                        QM.StateName = PMGSYSession.Current.StateName;
                    }
                    else
                    {
                        QM.StateName = QM.State == 0 ? "All States" : QM.StateName;
                    }
                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Itemwise Inspection Report

        public ActionResult QMItemwiseInspectionsLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            SelectListItem itm;

            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //QM.GradingItemList = new SelectList(dbContext.MASTER_QM_ITEM.Where(m => //m.MAST_SUB_ITEM_CODE == 0 &&
                //                                m.MAST_QM_TYPE == "N" 
                //                                //&& m.MAST_ITEM_STATUS != "M" 
                //                                && m.MAST_ITEM_STATUS != "O"
                //                                ), "MAST_ITEM_NO", "MAST_ITEM_NAME").ToList();

                //QM.GradingItemList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));



                QM.GradingItemList = new List<MASTER_QM_ITEM>();
                //QM.GradingItemList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                QM.GradingItemList = dbContext.MASTER_QM_ITEM.Where(x => x.MAST_QM_TYPE == "N" && x.MAST_ITEM_STATUS != "O").ToList();


                //foreach (var data in list)
                //{
                //    itm = new SelectListItem();
                //    itm.Text = data.item + "." + data.subitem + " " + data.Text;
                //    itm.Value = data.Value.ToString();
                //    QM.GradingItemList.Add(itm);
                //}

                QM.State = PMGSYSession.Current.StateCode;
                QM.StateList = comm.PopulateStates(true);
                QM.StateList.RemoveAt(0);
                QM.StateList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                QM.DistrictList = new List<SelectListItem>();
                QM.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));

                QM.FromYear = DateTime.Now.Year - 1;
                QM.ToYear = DateTime.Now.Year;

                QM.FromMonth = 1;
                QM.ToMonth = DateTime.Now.Month;

                QM.FromYearList = comm.PopulateYears(false);
                QM.ToYearList = comm.PopulateYears(false);
                QM.FromMonthList = comm.PopulateMonths(false);
                QM.ToMonthList = comm.PopulateMonths(false);

                QM.Grade = 2;
                return View(QM);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult QMItemwiseInspectionsReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;

                    QM.StateName = QM.State == 0 ? "All States" : QM.StateName;
                    QM.DistName = QM.District == 0 ? "All Districts" : QM.DistName;

                    //QM.BLOCK_NAME = "All Blocks";

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Common function
        public ActionResult DistrictDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            //list.Find(x => x.Value == "-1").Text = "Select District";
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Action Taken Report
        public ActionResult QMATRDetailsLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                QM.FromYear = DateTime.Now.Year - 1;
                QM.ToYear = DateTime.Now.Year;

                QM.FromMonth = 1;
                QM.ToMonth = DateTime.Now.Month;

                QM.FromYearList = comm.PopulateYears(false);
                QM.ToYearList = comm.PopulateYears(false);
                QM.FromMonthList = comm.PopulateMonths(false);
                QM.ToMonthList = comm.PopulateMonths(false);
                QM.ValueType = "S";

                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult QMATRDetailsReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 25)
                    //{
                    //    QM.Level = 1;
                    //}
                    //else if (PMGSYSession.Current.RoleCode == 8)
                    //{
                    //    QM.Level = 2;
                    //}
                    QM.Level = 1;
                    QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;

                    QM.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;
                    QM.DistName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;

                    //QM.BLOCK_NAME = "All Blocks";

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Districtwise Details
        public ActionResult DistwiseInspectionLayout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DistwiseInspectionReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 25)
                    {
                        QM.Level = 1;
                    }
                    else if (PMGSYSession.Current.RoleCode == 8)
                    {
                        QM.Level = 2;
                    }

                    QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;

                    QM.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;
                    QM.DistName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;

                    //QM.BLOCK_NAME = "All Blocks";

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Unsatisfactory Works
        public ActionResult QMUnsatisfactoryWorkLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                QM.qmType = "I";
                if (PMGSYSession.Current.StateCode > 0)
                {
                    QM.StateList = new List<SelectListItem>();
                    QM.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode), Selected = true }));
                }
                else
                {
                    QM.StateList = comm.PopulateStates(true);
                    QM.StateList.RemoveAt(0);
                    QM.StateList.Insert(0, (new SelectListItem { Text = "Select State", Value = "-1", Selected = true }));
                }
                QM.qmTypeList = comm.PopulateMonitorTypes();

                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult QMUnsatisfactoryWorkReport(QualityReportsViewModel QM)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                if (ModelState.IsValid)
                {
                    //QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;

                    QM.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;
                    QM.DistName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;


                    var stateOfficersDetails = (from sqc in dbContext.ADMIN_SQC
                                                join
                                                    ms in dbContext.MASTER_STATE on sqc.MAST_STATE_CODE equals ms.MAST_STATE_CODE
                                                join
                                                    ad in dbContext.ADMIN_DEPARTMENT on ms.MAST_STATE_CODE equals ad.MAST_STATE_CODE
                                                join
                                                    ano in dbContext.ADMIN_NODAL_OFFICERS on ad.ADMIN_ND_CODE equals ano.ADMIN_ND_CODE
                                                where
                                                     sqc.MAST_STATE_CODE == QM.State &&
                                                     ad.MAST_ND_TYPE == "S" &&
                                                     ano.ADMIN_NO_DESIGNATION == 30
                                                select new
                                                {
                                                    STATE_NAME = ms.MAST_STATE_NAME,
                                                    QC_NAME = sqc.ADMIN_QC_NAME,
                                                    QC_PHONE = sqc.ADMIN_QC_PHONE1 + ((sqc.ADMIN_QC_PHONE2 != null && !sqc.ADMIN_QC_PHONE2.Equals(string.Empty)) ? (", " + sqc.ADMIN_QC_PHONE2) : ""),
                                                    CEO_NAME = ano.ADMIN_NO_FNAME + (ano.ADMIN_NO_MNAME != null ? (" " + ano.ADMIN_NO_MNAME) : "") + (ano.ADMIN_NO_LNAME != null ? (" " + ano.ADMIN_NO_LNAME) : ""),
                                                    CEO_PHONE = ano.ADMIN_NO_OFFICE_PHONE
                                                }).ToList();

                    foreach (var item in stateOfficersDetails)
                    {
                        QM.StateName = item.STATE_NAME;
                        QM.QCName = item.QC_NAME;
                        QM.QCPhone = (item.QC_PHONE == "" || item.QC_PHONE == null) ? "-" : item.QC_PHONE;
                        QM.CEOName = item.CEO_NAME;
                        QM.CEOPhone = (item.CEO_PHONE == "" || item.CEO_PHONE == null) ? "-" : item.CEO_PHONE;
                    }
                    //QM.BLOCK_NAME = "All Blocks";

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Commenced Works
        public ActionResult CommencedWorksLayout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CommencedWorksReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    //QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;

                    //QM.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;
                    //QM.DistName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;

                    //QM.BLOCK_NAME = "All Blocks";

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Completed Works
        public ActionResult QMCompletedWorkLayout()
        {
            //QMCompletedWorkModel model = new QMCompletedWorkModel();
            QualityReportsViewModel QM = new QualityReportsViewModel();
            try
            {
                string firstdate = "01/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
                QM.FROM_DATE = Convert.ToDateTime(firstdate.Trim()).ToString("dd/MM/yyyy");
                QM.TO_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        public ActionResult CompletedWorksReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    //QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;

                    //QM.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;
                    //QM.DistName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;

                    //QM.BLOCK_NAME = "All Blocks";

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Inspection Count Report
        //[Audit]
        public ActionResult QMInspectionsCountLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                //QM.Year = DateTime.Now.Year;

                //QM.YearList = comm.PopulateYears(true);
                QM.YearList = comm.PopulateFinancialYear(true, true).ToList();
                //QM.YearList.Find(x => x.Value == "0").Text = "All Years";
                QM.YearList.Find(x => x.Value == "0").Selected = true;

                QM.qmTypeList = new List<SelectListItem>();
                QM.qmTypeList = comm.PopulateMonitorTypes();

                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult QMInspectionsCountReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.Level = PMGSYSession.Current.RoleCode == 8 ? 2 : 1;
                    QM.State = PMGSYSession.Current.StateCode;
                    QM.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;

                    QM.DistName = "All Districts";
                    QM.BlockName = "All Blocks";

                    QM.RoadStat = "C";
                    QM.InspCount = 1;
                    //QM.BLOCK_NAME = "All Blocks";

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region QM Ongoing Works Inspections
        [HttpGet]
        public ActionResult QMOngoingWorksInspections()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {

                QM.FromYear = DateTime.Now.Year - 1;
                QM.ToYear = DateTime.Now.Year;

                QM.FromMonth = 1;
                QM.ToMonth = DateTime.Now.Month;


                QM.Level = 1;
                QM.LevelList = new List<SelectListItem>();
                QM.LevelList.Insert(0, (new SelectListItem { Text = "< 10 Works", Value = "1" }));
                QM.LevelList.Insert(1, (new SelectListItem { Text = "10 to 20 Works", Value = "2" }));
                QM.LevelList.Insert(2, (new SelectListItem { Text = "> 20 Works", Value = "3" }));

                QM.FromYearList = comm.PopulateYears(false);
                QM.ToYearList = comm.PopulateYears(false);
                QM.FromMonthList = comm.PopulateMonths(false);
                QM.ToMonthList = comm.PopulateMonths(false);

                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        public ActionResult QMOngoingWorksInspectionsReport(QualityReportsViewModel QM)
        {
            try
            {

                if (ModelState.IsValid)
                {


                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }


        }

        #endregion

        [Audit]
        public ActionResult QMObservationDetails(string id)
        {
            PMGSY.BAL.QualityMonitoring.QualityMonitoringBAL qualityBAL = new PMGSY.BAL.QualityMonitoring.QualityMonitoringBAL();
            try
            {
                PMGSY.Models.QualityMonitoring.QMFillObservationModel fillObservationModel = qualityBAL.QMObservationDetailsBAL(Convert.ToInt32(id));
                fillObservationModel.QM_OBSERVATION_ID = Convert.ToInt32(id);
                return View(fillObservationModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #region Lab Report
        [HttpGet]
        public ActionResult LabLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                QM.State = PMGSYSession.Current.StateCode;
                QM.StateName = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateName : "All States";
                if (PMGSYSession.Current.StateCode > 0)
                {
                    QM.StateList = new List<SelectListItem>();
                    QM.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode), Selected = true }));
                }
                else
                {
                    QM.StateList = comm.PopulateStates(true);
                    QM.StateList.Find(x => x.Value == "0").Text = "All States";
                }
                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult LabReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.StateName = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateName : QM.StateName;

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Lab Report
        [HttpGet]
        public ActionResult DaysTakenLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                QM.State = PMGSYSession.Current.StateCode;
                QM.StateName = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateName : "All States";
                if (PMGSYSession.Current.StateCode > 0)
                {
                    QM.StateList = new List<SelectListItem>();
                    QM.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode), Selected = true }));
                }
                else
                {
                    QM.StateList = comm.PopulateStates(true);
                    QM.StateList.Find(x => x.Value == "0").Text = "All States";
                }
                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult DaysTakenReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.StateName = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateName : QM.StateName;

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Lab Report
        [HttpGet]
        public ActionResult LabStateDetailsLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            // Added By Sachin
            QM.AgencyList = comm.PopulateAgencies(PMGSYSession.Current.StateCode, true);
            try
            {
                QM.State = PMGSYSession.Current.StateCode;
                QM.StateName = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateName : "All States";
                QM.AgencyList = comm.PopulateAgencies(QM.State, true);
                if (PMGSYSession.Current.StateCode > 0)
                {
                    QM.StateList = new List<SelectListItem>();
                    QM.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode), Selected = true }));
                }
                else
                {
                    QM.StateList = comm.PopulateStates(true);
                    QM.StateList.Find(x => x.Value == "0").Text = "All States";
                }
                return View(QM);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QMSSRSReports.LabStateDetailsLayout()");
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }



        [HttpPost]
        public JsonResult PopulateAgencies()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);

                return Json(objCommonFunctions.PopulateAgencies(stateCode, true));
            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRSReports.PopulateAgencies()");
                return Json(new { string.Empty });
            }
        }





        [HttpPost]
        public ActionResult LabStateDetailsReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.StateName = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateName : QM.StateName;

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRSReports.LabStateDetailsReport()");
                return null;
            }
        }
        #endregion

        #region Regrade Action Taken Report
        [HttpGet]
        public ActionResult RegradeATRLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                string firstdate = "01/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
                QM.FROM_DATE = Convert.ToDateTime(firstdate.Trim()).ToString("dd/MM/yyyy");
                QM.TO_DATE = DateTime.Now.ToString("dd/MM/yyyy");

                List<SelectListItem> lstGrade = new List<SelectListItem>();
                lstGrade.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstGrade.Insert(1, new SelectListItem { Value = "2", Text = "SRI" });
                lstGrade.Insert(2, new SelectListItem { Value = "3", Text = "U" });

                QM.RegradeATRGradeList = lstGrade;

                QM.State = PMGSYSession.Current.StateCode;
                QM.StateName = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateName : "All States";
                if (PMGSYSession.Current.StateCode > 0)
                {
                    QM.StateList = new List<SelectListItem>();
                    QM.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode), Selected = true }));
                }
                else
                {
                    QM.StateList = comm.PopulateStates(true);
                    QM.StateList.Find(x => x.Value == "0").Text = "All States";
                }

                List<SelectListItem> lstRegrade = new List<SelectListItem>();
                lstRegrade.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstRegrade.Insert(1, new SelectListItem { Value = "A", Text = "Accepted" });
                lstRegrade.Insert(2, new SelectListItem { Value = "R", Text = "Rejected" });

                QM.RegradeList = lstRegrade;

                List<SelectListItem> lstRdStatus = new List<SelectListItem>();
                lstRdStatus.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstRdStatus.Insert(1, new SelectListItem { Value = "C", Text = "Completed" });
                lstRdStatus.Insert(2, new SelectListItem { Value = "P", Text = "In Progress" });

                QM.RoadStatusList = lstRdStatus;

                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult RegradeATRReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;
                    QM.StateName = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateName : QM.StateName;

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Commenced SQM Only Report
        public ActionResult CommencedSQMOnlyLayout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CommencedSQMOnlyReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";
                    QM.Level = 1;
                    QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;
                    QM.District = 0;
                    QM.Status = "I";
                    QM.qmType = "S";

                    //QM.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;
                    //QM.DistName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;

                    //QM.BLOCK_NAME = "All Blocks";

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Works Not Inspected SQM
        public ActionResult WorksNotInspectedSQMLayout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult WorksNotInspectedSQMReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;

                    QM.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;
                    //QM.DistName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;

                    //QM.BLOCK_NAME = "All Blocks";

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Monitor Schedule Report
        [HttpGet]
        public ActionResult MonitorScheduleLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                QM.State = PMGSYSession.Current.StateCode;
                QM.StateName = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateName : "All States";
                if (PMGSYSession.Current.StateCode > 0)
                {
                    QM.StateList = new List<SelectListItem>();
                    QM.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode), Selected = true }));
                }
                else
                {
                    QM.StateList = comm.PopulateStates(true);
                    QM.StateList.Find(x => x.Value == "0").Text = "All States";
                }

                QM.MonthList = comm.PopulateMonths(false);
                //QM.MonthList.Find(x => x.Value == "0").Text = "All Months";

                QM.YearList = comm.PopulateYears(false);
                //QM.YearList.Find(x => x.Value == "0").Text = "All Years";

                QM.qmTypeList = new List<SelectListItem>();
                QM.qmTypeList.Insert(0, new SelectListItem { Value = "I", Text = "NQM" });
                QM.qmTypeList.Insert(1, new SelectListItem { Value = "S", Text = "SQM" });

                QM.qmType = PMGSYSession.Current.StateCode > 0 ? "S" : "I";

                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult MonitorScheduleReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    QM.qmType = PMGSYSession.Current.RoleCode == 6 ? "I" : QM.qmType;

                    QM.Monitor = 0;
                    QM.StateName = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateName : QM.StateName;

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region QM Ongoing Works Inspections
        [HttpGet]
        public ActionResult QMWorkwiseNQMSQMLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                QM.hdnRole = PMGSYSession.Current.RoleCode;
                if (PMGSYSession.Current.RoleCode == 8) //SQC
                {
                    QM.StateList = new List<SelectListItem>();
                    QM.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode) }));

                    QM.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, false, 0);
                    QM.DistrictList.Find(x => x.Value == "0").Text = "All Districts";
                    //QM.DistrictList.RemoveAt(0);
                }
                else
                {
                    QM.StateList = comm.PopulateStates(false);
                    //QM.StateList.Find(x => x.Value == "0").Value = "-1";
                    QM.StateList.Insert(0, (new SelectListItem { Text = "Select", Value = "-1" }));

                    QM.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    QM.DistrictList.Find(x => x.Value == "-1").Value = "0";
                }

                QM.FromYear = DateTime.Now.Year;
                QM.ToYear = DateTime.Now.Year;

                QM.FromMonth = 1;
                QM.ToMonth = DateTime.Now.Month;

                QM.FromYearList = comm.PopulateYears(false);
                QM.ToYearList = comm.PopulateYears(false);
                QM.FromMonthList = comm.PopulateMonths(false);
                QM.ToMonthList = comm.PopulateMonths(false);

                QM.FromYearList.Insert(0, (new SelectListItem { Text = "Select Year", Value = "-1" }));
                QM.ToYearList.Insert(0, (new SelectListItem { Text = "Select Year", Value = "-1" }));
                QM.FromMonthList.Insert(0, (new SelectListItem { Text = "Select Month", Value = "-1" }));
                QM.ToMonthList.Insert(0, (new SelectListItem { Text = "Select Month", Value = "-1" }));

                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        public ActionResult QMWorkwiseNQMSQMReport(QualityReportsViewModel QM)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }


        }

        #endregion

        /// DownLoad Image OR Pdf File (ATR)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult DownloadFile(String id)
        {
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = ".pdf";
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //In case of if File With Name not Found then find with Id, This is case particularly for ATR
                FullFileLogicalPathId = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR_VIRTUAL_DIR_PATH"], id.ToString() + ".pdf");
                FullfilePhysicalPathId = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR"], id.ToString() + ".pdf");

                if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #region QM Quality Profile SSRS Report

        public ActionResult QMQualityProfileLayout()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            QMQualityProfileViewModel qmphase = new QMQualityProfileViewModel();
            if (PMGSYSession.Current.RoleCode == 8)
            {
                qmphase.lstStates = new List<SelectListItem>();
                qmphase.lstStates.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
            }
            else
            {
                qmphase.lstStates = commonFunctions.PopulateStates(false);
                qmphase.lstStates.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
            }

            return View(qmphase);
        }

        [HttpPost]
        public ActionResult QMQualityProfileReport(QMQualityProfileViewModel qmphaseProgress)
        {
            try
            {
                if (qmphaseProgress.StateCode == 0 || qmphaseProgress.StateCode > 0)
                {
                    qmphaseProgress.LevelID = 1;
                }
                else
                {
                    qmphaseProgress.LevelID = 2;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(qmphaseProgress);

        }


        #endregion


        #region QM Quality Profile HighCharts Graph
        public ActionResult QMQualityProgressReport(string id)
        {
            string[] paramArr = id.Split('$');

            QMQualityProgressHighChartsViewModel model = new QMQualityProgressHighChartsViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            //if (PMGSYSession.Current.RoleCode == 8)
            //{
            //    model.StateList = new List<SelectListItem>();
            //    model.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
            //}
            //else
            //{
            //    model.StateList = objCommonFunctions.PopulateStates(false);
            //    model.StateList.Insert(0, new SelectListItem { Text = "All States", Value = "0" });
            //}

            model.StateCode = Convert.ToInt32(paramArr[0]);
            model.Year = Convert.ToInt32(paramArr[1]);

            // model.Year = DateTime.Now.Year;
            //// model.YearList = objCommonFunctions.PopulateYears(false).ToList();
            // model.YearList = objCommonFunctions.PopulateFinancialYear(false).ToList();




            return View(model);

        }


        [HttpPost]
        [Audit]
        public ActionResult QMQualityProfileLineChart()
        {
            try
            {
                List<USP_QUALITY_PROFILE_GRAPH_Result> List = GetLineChartDataforQualityProfile(Convert.ToInt32(Request.Params["stateCode"]), Convert.ToInt32(Request.Params["year"]));
                //List<USP_QUALITY_PROFILE_GRAPH_Result> List = GetLineChartDataforQualityProfile(stateCode, year);

                return new JsonResult
                {
                    Data = List
                };
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return new JsonResult
                {
                    Data = string.Empty
                };
            }
        }



        public List<USP_QUALITY_PROFILE_GRAPH_Result> GetLineChartDataforQualityProfile(int state, int year)
        {
            PMGSYEntities dbContext = null;
            dbContext = new PMGSYEntities();
            try
            {
                if (PMGSYSession.Current.StateCode > 0)
                    state = PMGSYSession.Current.StateCode;

                List<USP_QUALITY_PROFILE_GRAPH_Result> itemList = new List<USP_QUALITY_PROFILE_GRAPH_Result>();
                itemList = dbContext.USP_QUALITY_PROFILE_GRAPH(2, state, year).ToList<USP_QUALITY_PROFILE_GRAPH_Result>();

                return itemList;
            }
            catch (Exception ex)
            {
                // Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }




        #endregion


        #region NQM Wise Action Taken Report

        [HttpGet]
        public ActionResult QMNQMWiseActionTakenReportLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {

                QM.FromYear = DateTime.Now.Year - 1;
                QM.ToYear = DateTime.Now.Year;

                QM.FromMonth = 1;
                QM.ToMonth = DateTime.Now.Month;

                QM.FromYearList = comm.PopulateYears(false);
                QM.ToYearList = comm.PopulateYears(false);
                QM.FromMonthList = comm.PopulateMonths(false);
                QM.ToMonthList = comm.PopulateMonths(false);

                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        public ActionResult QMNQMWiseActionTakenReport(QualityReportsViewModel QM)
        {
            try
            {

                if (ModelState.IsValid)
                {


                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }


        }



        #endregion



        //
        // GET: /QMSSRSReports/QMSSRSReports/
        #region Monitor Wise Inspection Report

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult QMMonitorWiseInspection()
        {

            CommonFunctions objCommonFunctions = new CommonFunctions();


            try
            {
                QMFilterViewModel qmFilterModel = new QMFilterViewModel();
                qmFilterModel.QM_TYPE_CODE = "0";
                qmFilterModel.MAST_STATE_CODE = 0;


                qmFilterModel.FROM_MONTH = DateTime.Now.Month;
                qmFilterModel.FROM_YEAR = DateTime.Now.Year;
                qmFilterModel.TO_MONTH = DateTime.Now.Month;
                qmFilterModel.TO_YEAR = DateTime.Now.Year;
                qmFilterModel.RoleID = PMGSYSession.Current.RoleCode;
                if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)  //CQC or CQCADMIN 
                {
                    qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                    qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));



                }
                else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 48)  //SQC
                {
                    qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    ViewBag.StateName = PMGSYSession.Current.StateName;

                }



                qmFilterModel.QM_TYPES = new List<SelectListItem>();
                qmFilterModel.QM_TYPES.Add(new SelectListItem { Text = "Select Type", Value = "0", Selected = true });
                qmFilterModel.QM_TYPES.Add(new SelectListItem { Text = "SQM", Value = "S" });
                qmFilterModel.QM_TYPES.Add(new SelectListItem { Text = "NQM", Value = "I" });


                qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();

                return View(qmFilterModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (objCommonFunctions != null)
                {
                    objCommonFunctions.Dispose();
                }
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QMMonitorWiseInspectionReport(QMFilterViewModel qmFilterModel)
        {

            try
            {
                return View(qmFilterModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }
        #endregion



        //
        // GET: /QMSSRSReports/QMSSRSReports/
        #region Analysis of uploading of inspection
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult QMInspectionUpload()
        {

            CommonFunctions objCommonFunctions = new CommonFunctions();
            PMGSYEntities entities = new PMGSYEntities();

            try
            {
                QMFilterViewModel qmFilterModel = new QMFilterViewModel();
                qmFilterModel.QM_TYPE_CODE = "0";
                qmFilterModel.MAST_STATE_CODE = 0;


                qmFilterModel.FROM_MONTH = DateTime.Now.Month;
                qmFilterModel.FROM_YEAR = DateTime.Now.Year;
                qmFilterModel.TO_MONTH = DateTime.Now.Month;
                qmFilterModel.TO_YEAR = DateTime.Now.Year;
                qmFilterModel.RoleID = PMGSYSession.Current.RoleCode;
                if (PMGSYSession.Current.RoleCode == 5)  //CQC
                {
                    qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                    qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "Select", Value = "0", Selected = true }));



                }
                else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 22)  //SQC
                {
                    qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    ViewBag.StateName = PMGSYSession.Current.StateName;

                }
                if (PMGSYSession.Current.RoleCode == 6)
                {
                    qmFilterModel.MAST_STATE_CODE = 0;
                    qmFilterModel.QM_TYPE_CODE = "I";
                    qmFilterModel.ADMIN_QM_CODE = entities.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_USER_ID == PMGSYSession.Current.UserId).FirstOrDefault().ADMIN_QM_CODE;
                }
                else if (PMGSYSession.Current.RoleCode == 7)
                {
                    qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    qmFilterModel.QM_TYPE_CODE = "S";
                    qmFilterModel.ADMIN_QM_CODE = entities.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_USER_ID == PMGSYSession.Current.UserId).FirstOrDefault().ADMIN_QM_CODE;
                }



                qmFilterModel.QM_TYPES = new List<SelectListItem>();
                qmFilterModel.QM_TYPES.Add(new SelectListItem { Text = "Select Type", Value = "0", Selected = true });
                qmFilterModel.QM_TYPES.Add(new SelectListItem { Text = "SQM", Value = "S" });
                qmFilterModel.QM_TYPES.Add(new SelectListItem { Text = "NQM", Value = "I" });


                qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();

                return View(qmFilterModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (objCommonFunctions != null)
                {
                    objCommonFunctions.Dispose();
                }
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QMInspectionUploadReport(QMFilterViewModel qmFilterModel)
        {

            try
            {
                return View(qmFilterModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }
        #endregion


        #region Google Map
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GoogleMap()
        {
            PMGSYEntities entities = new PMGSYEntities();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            BlockModel blockModel = new BlockModel();

            blockModel.StateList = objCommonFunctions.PopulateStates(true);
            blockModel.DistrictList = objCommonFunctions.PopulateDistrict(5, false);
            blockModel.BlockList = objCommonFunctions.PopulateBlocks(26, false);


            return View(blockModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GoogleMapShow(int state, int district, int block)
        {
            List<BlockRoad> blockRoadList = new List<BlockRoad>();
            BlockRoad blockRoad;
            CommonFunctions obj = new CommonFunctions();
            PMGSYEntities entities = new PMGSYEntities();
            var blockJson = entities.USP_QM_BLOCK_LAT_LONG(state, district, block).ToList<USP_QM_BLOCK_LAT_LONG_Result>();


            var random = new Random(); // Make sure this is out of the loop!


            foreach (var road in blockJson)
            {

                /*  var InsplatLong = (
                                        from qm in entities.QUALITY_QM_OBSERVATION_MASTER
                                        join latLong in entities.QUALITY_QM_INSPECTION_FILE on qm.QM_OBSERVATION_ID equals latLong.QM_OBSERVATION_ID
                                         where qm.IMS_PR_ROAD_CODE== road.ROAD_ID && latLong.QM_LATITUDE != null && latLong.QM_LONGITUDE != null
                                         select new RoadGeoPosition 
                                         {
                                             ObservationId=qm.QM_OBSERVATION_ID,
                                             monitor = qm.QUALITY_QM_SCHEDULE.ADMIN_QUALITY_MONITORS.ADMIN_QM_LNAME + qm.QUALITY_QM_SCHEDULE.ADMIN_QUALITY_MONITORS.ADMIN_QM_FNAME + qm.QUALITY_QM_SCHEDULE.ADMIN_QUALITY_MONITORS.ADMIN_QM_MNAME == null ? "" : qm.QUALITY_QM_SCHEDULE.ADMIN_QUALITY_MONITORS.ADMIN_QM_MNAME,
                                             Grade=qm.MASTER_GRADE_TYPE.MAST_GRADE_SHORT_NAME,
                                             InspDate=qm.QM_INSPECTION_DATE,//.GetDateTimeToString(qm.QM_INSPECTION_DATE),
                                             RoadStatus=qm.QM_ROAD_STATUS=="C"?"Completed":"Progress",
                                             Description = latLong.QM_FILE_DESCR, 
                                             Lattitude = latLong.QM_LATITUDE.Value, 
                                             Longitude = latLong.QM_LONGITUDE.Value ,
                                             PhotoURL="https://online.omms.nic.in/OMMAS/QM/NQM/"+latLong.QM_FILE_NAME
                                         }
                                     ).ToList<RoadGeoPosition>();
                 */
                var InsplatLongDB = (
                                      from qm in entities.QUALITY_QM_OBSERVATION_MASTER
                                      join latLong in entities.QUALITY_QM_INSPECTION_FILE on qm.QM_OBSERVATION_ID equals latLong.QM_OBSERVATION_ID
                                      join sch in entities.QUALITY_QM_SCHEDULE on qm.ADMIN_SCHEDULE_CODE equals sch.ADMIN_SCHEDULE_CODE
                                      join monitor in entities.ADMIN_QUALITY_MONITORS on sch.ADMIN_QM_CODE equals monitor.ADMIN_QM_CODE
                                      join gt in entities.MASTER_GRADE_TYPE on qm.QM_OVERALL_GRADE equals gt.MAST_GRADE_CODE
                                      where qm.IMS_PR_ROAD_CODE == road.ROAD_ID && latLong.QM_LATITUDE != null && latLong.QM_LONGITUDE != null
                                      select new
                                      {

                                          monitor = (monitor.ADMIN_QM_TYPE == "I" ? "NQM: " : "SQM: ") + (monitor.ADMIN_QM_LNAME == null ? "" : monitor.ADMIN_QM_LNAME) + " " + (monitor.ADMIN_QM_FNAME == null ? "" : monitor.ADMIN_QM_FNAME) + " " + (monitor.ADMIN_QM_MNAME == null ? "" : monitor.ADMIN_QM_MNAME),
                                          Grade = gt.MAST_GRADE_NAME,
                                          InspDate = qm.QM_INSPECTION_DATE,
                                          RoadStatus = qm.QM_ROAD_STATUS == "C" ? "Completed" : (qm.QM_ROAD_STATUS == "M" ? "Maintenance" : "Progress"),
                                          Description = latLong.QM_FILE_DESCR,
                                          Lattitude = latLong.QM_LATITUDE.Value,
                                          Longitude = latLong.QM_LONGITUDE.Value,
                                          PhotoURL = (monitor.ADMIN_QM_TYPE == "I" ? "https://online.omms.nic.in/OMMAS/QM/NQM/" : "https://online.omms.nic.in/OMMAS/QM/SQM/") + latLong.QM_FILE_NAME,
                                          PhotoURLThumb = (monitor.ADMIN_QM_TYPE == "I" ? "https://online.omms.nic.in/OMMAS/QM/NQM/thumbnails/" : "https://online.omms.nic.in/OMMAS/QM/SQM/thumbnails/") + latLong.QM_FILE_NAME
                                      }
                                   );

                var InsplatLong = (
                                     from qm in InsplatLongDB.AsEnumerable()
                                     select new RoadGeoPosition
                                     {
                                         monitor = qm.monitor,
                                         Grade = qm.Grade,
                                         InspDate = obj.GetDateTimeToString(qm.InspDate),
                                         RoadStatus = qm.RoadStatus,
                                         Description = qm.Description,
                                         Lattitude = qm.Lattitude,
                                         Longitude = qm.Longitude,
                                         PhotoURL = qm.PhotoURL,
                                         PhotoURLThumb = qm.PhotoURLThumb
                                     }
                                  ).ToList<RoadGeoPosition>();
                blockRoad = new BlockRoad
                {

                    RoadId = road.ROAD_ID.ToString(),
                    RoadName = road.WORK_Name,
                    Package = road.Package,
                    SYear = road.SYear,
                    RoadLength = road.RoadLength,
                    RoadStatus = road.RoadStatus,
                    StipulatedCompletedDate = road.STIPULATED_COMPLETED_DATE,
                    ColorCode = String.Format("{0:X6}", random.Next(0x1000000)),
                    GeoPosition = InsplatLong

                };
                blockRoadList.Add(blockRoad);
            }
            return Json(blockRoadList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DistrictBlockList(int state, int district)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            if (district == 0)
            {
                var districtList = objCommonFunctions.PopulateDistrict(state, false);
                return Json(districtList, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var blockList = objCommonFunctions.PopulateBlocks(district, false);
                return Json(blockList, JsonRequestBehavior.AllowGet);

            }

        }


        #endregion

        #region Frequency of Inspection done by Monitor in a day

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult InspectionFrequencyMonitor()
        {

            CommonFunctions objCommonFunctions = new CommonFunctions();
            PMGSYEntities entities = new PMGSYEntities();

            try
            {
                QMFilterViewModel qmFilterModel = new QMFilterViewModel();
                qmFilterModel.QM_TYPE_CODE = "0";
                qmFilterModel.MAST_STATE_CODE = 0;


                qmFilterModel.FROM_MONTH = DateTime.Now.Month;
                qmFilterModel.FROM_YEAR = DateTime.Now.Year;
                qmFilterModel.TO_MONTH = DateTime.Now.Month;
                qmFilterModel.TO_YEAR = DateTime.Now.Year;
                qmFilterModel.RoleID = PMGSYSession.Current.RoleCode;
                if (PMGSYSession.Current.RoleCode == 5)  //CQC
                {
                    qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                    qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "Select", Value = "0", Selected = true }));



                }
                else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 22)  //SQC
                {
                    qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    ViewBag.StateName = PMGSYSession.Current.StateName;

                }

                qmFilterModel.QM_TYPES = new List<SelectListItem>();
                qmFilterModel.QM_TYPES.Add(new SelectListItem { Text = "Select Type", Value = "0", Selected = true });
                qmFilterModel.QM_TYPES.Add(new SelectListItem { Text = "SQM", Value = "S" });
                qmFilterModel.QM_TYPES.Add(new SelectListItem { Text = "NQM", Value = "I" });
                qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();

                return View(qmFilterModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (objCommonFunctions != null)
                {
                    objCommonFunctions.Dispose();
                }
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InspectionFrequencyMonitorReport(QMFilterViewModel qmFilterModel)
        {

            try
            {
                return View(qmFilterModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        #endregion


        // 10 DEC 2015 Added By Aanad 
        #region Team Details
        [Audit]
        public ActionResult QualityTeam()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            if (DateTime.Now.Month == 12)
            {
                qmFilterModel.FROM_YEARS_LIST.Add(new SelectListItem { Text = "2015", Value = "2015" });
                qmFilterModel.FROM_YEARS_LIST = qmFilterModel.FROM_YEARS_LIST.OrderByDescending(c => c.Value).ToList();
            }
            return View(qmFilterModel);
        }


        public ActionResult QualityTeamReport(int smonth, int syear)
        {
            ViewBag.smonth = smonth;
            ViewBag.syear = syear;
            return View();
        }
        #endregion

        // 10 DEC 2015 Added By Aanad 
        #region Contractor Wise Inspection Report
        [Audit]
        public ActionResult ContractorInspection()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ContractorInspectionModel contractorViewModel = new ContractorInspectionModel
            {
                MAST_STATE_CODE = PMGSYSession.Current.StateCode,
                ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus(),
                //SANCTION_YEAR_LIST = objCommonFunctions.PopulateFinancialYear(true, false),
                SANCTION_YEAR_LIST = objCommonFunctions.PopulateFinancialYear(true, true),
                ROAD_STATUS = "A",
                SANCTION_YEAR = 0
            };
            contractorViewModel.SANCTION_YEAR_LIST.ElementAt(0).Text = "All";
            return View(contractorViewModel);
        }
        [Audit]
        public ActionResult ContractorInspectionReport(int syear, string status)
        {

            CommonFunctions objCommonFunctions = new CommonFunctions();
            ContractorInspectionModel contractorViewModel = new ContractorInspectionModel
            {
                MAST_STATE_CODE = PMGSYSession.Current.StateCode,
                ROAD_STATUS = status == "A" ? "0" : status,
                SANCTION_YEAR = syear

            };
            return View(contractorViewModel);
        }
        #endregion


        #region NQM Schedule Month Wise Report
        // [Audit]
        public ActionResult NQMScheduleMonth()
        {

            CommonFunctions objCommonFunctions = new CommonFunctions();
            ViewBag.YEARS_LIST = objCommonFunctions.PopulateYears(true).ToList();
            return View();
        }

        [Audit]
        public ActionResult NQMScheduleMonthReport(int syear)
        {
            ViewBag.SYear = syear;
            return View();
        }
        #endregion

        #region Added on 02/FEB/2016
        //GET: /QMSSRSReports/MonitorTeam/NQMTeamInspection
        //public ActionResult NQMTeamInspection()
        //{
        //    QMTeamFilterModel teamModel = null;
        //    CommonFunctions objCommonFunctions = new CommonFunctions();
        //    teamModel = new QMTeamFilterModel();

        //    teamModel.MonitorList = objCommonFunctions.PopulateMonitors("false", "I", 0);
        //    teamModel.StateList = objCommonFunctions.PopulateStates(true);
        //    teamModel.StateList.Add(new SelectListItem() { Text = "All State", Value = "0", Selected = true });
        //    teamModel.ScheduleYearList = objCommonFunctions.PopulateYears(false);
        //    teamModel.ScheduleYearList.Add(new SelectListItem() { Text = "All Year", Value = "0", Selected = true });
        //    teamModel.ScheduleMonthList = objCommonFunctions.PopulateMonths(false);
        //    teamModel.ScheduleMonthList.Add(new SelectListItem() { Text = "All Month", Value = "0", Selected = true });
        //    teamModel.RoleID = PMGSYSession.Current.RoleCode;
        //    return View(teamModel);
        //}

        ////GET: /QMSSRSReports/MonitorTeam/NQMTeamInspectionReport
        //public ActionResult NQMTeamInspectionReport(QMTeamFilterModel teamModel)
        //{

        //    return View(teamModel);
        //}


        #endregion


        #region NQM Grading Abstract REPORT
        public ActionResult NQMGradingAbstractLayout()
        {
            QMGradingAbstract qmModel = new QMGradingAbstract();
            CommonFunctions commonFunctions = new CommonFunctions();

            qmModel.FrmYearList = commonFunctions.PopulateYears(System.DateTime.Now.Year, false);
            qmModel.FrmMonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

            qmModel.ToYearList = commonFunctions.PopulateYears(System.DateTime.Now.Year, false);
            qmModel.ToMonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

            qmModel.FrmMonth = System.DateTime.Now.Month;
            qmModel.ToMonth = System.DateTime.Now.Month;
            qmModel.FrmYear = System.DateTime.Now.Year;
            qmModel.ToYear = System.DateTime.Now.Year;

            qmModel.schemeList = new List<SelectListItem> { 
                                                            new SelectListItem{ Text = "All Schemes", Value ="0" , Selected = true }, 
                                                            new SelectListItem{ Text = "PMGSY I", Value ="1" }, 
                                                            new SelectListItem{ Text = "PMGSY II", Value ="2" } ,
                                                            new SelectListItem{ Text = "RCPLWE", Value ="3" } 
                                                           
                                                            };
            qmModel.qmTypeList = new List<SelectListItem> {                                                          
                                                            new SelectListItem{ Text = "NQM", Value ="I" , Selected = true} , 
                                                            new SelectListItem{ Text = "SQM", Value ="S" } 
                                                            };

            qmModel.StateCode = PMGSYSession.Current.StateCode;//fetchCookie.StateCode;



            if (PMGSYSession.Current.Language.Contains('-'))
            {
                qmModel.localizedValue = PMGSYSession.Current.Language.Substring(0, PMGSYSession.Current.Language.IndexOf('-'));
            }
            else
            {
                qmModel.localizedValue = PMGSYSession.Current.Language;
            }

            return View(qmModel);
        }

        public ActionResult NQMGradingAbstractReport(QMGradingAbstract qmModel)
        {

            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {

                    return View(qmModel);
                }
                else
                {
                    string message = "";
                    //    bool flag = false;
                    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;

                    qmModel.FrmYearList = commonFunctions.PopulateYears(System.DateTime.Now.Year, false);
                    qmModel.FrmMonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

                    qmModel.ToYearList = commonFunctions.PopulateYears(System.DateTime.Now.Year, false);
                    qmModel.ToMonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);
                    return View("NQMGradingAbstractLayout", qmModel);
                }
            }
            catch
            {
                return View(qmModel);
            }
            //return View();
        }
        #endregion

        #region
        [Audit]
        public ActionResult sqmassignedroad()
        {

            CommonFunctions objCommonFunctions = new CommonFunctions();
            QualityReportsViewModel filterViewModel = new QualityReportsViewModel();
            filterViewModel = new QualityReportsViewModel
            {
                FromMonthList = objCommonFunctions.PopulateMonths(false),
                ToMonthList = objCommonFunctions.PopulateMonths(false),
                FromYearList = objCommonFunctions.PopulateYears(false),
                ToYearList = objCommonFunctions.PopulateYears(false)
            };
            return View(filterViewModel);
        }

        [Audit]
        [HttpPost]
        public ActionResult sqmassignedroadReport(QualityReportsViewModel filterViewModel)
        {

            return View(filterViewModel);
        }

        #endregion


        public ActionResult QmFinPerUnsatisfactory()
        {
            return View();
        }

        public ActionResult QMATRInspectionLab()
        {
            return View();
        }

        public ActionResult QmATRPending()
        {

            CommonFunctions comm = new CommonFunctions();
            var statelist = comm.PopulateStates(false);

            statelist.Insert(0, (new SelectListItem { Text = "All States", Value = "0", Selected = true }));
            ViewBag.StateList = statelist;
            // ViewBag.StateList = comm.PopulateStates(true);

            return View();
        }

        public ActionResult QmATRPendingReport(int state)
        {
            ViewBag.StateCode = state;
            return View();
        }


        public ActionResult StatewiseIssue()
        {
            return View();
        }

        #region Priority Schedule by Aanand integrated on 10 May 2016

        public ActionResult SchedulePriorityLayout()
        {
            return View();
        }
        public ActionResult StateSchedulePriority()
        {
            return View();
        }

        public ActionResult StateSchedulePriorityReport(SchedulePriorityViewModel schedule)
        {
            return View(schedule);
        }
        public ActionResult DistrictSchedulePriority(int state)
        {
            ViewBag.StateCode = state;

            return View();
        }
        public ActionResult DistrictSchedulePriorityReport(SchedulePriorityViewModel schedule)
        {

            return View(schedule);
        }
        public ActionResult RoadSchedulePriority(SchedulePriorityViewModel schedule)
        {
            return View(schedule);
        }
        #endregion Priority Schedule

        #region CC BT



        public ActionResult CCBTLayout()
        {

            CCBTViewModel stateListWiseRoadsReportModel = new CCBTViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();

            stateListWiseRoadsReportModel.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

            stateListWiseRoadsReportModel.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();



            stateListWiseRoadsReportModel.Mast_State_Code = PMGSYSession.Current.StateCode;


            stateListWiseRoadsReportModel.Mast_District_Code = PMGSYSession.Current.DistrictCode;

            stateListWiseRoadsReportModel.DistrictCode = PMGSYSession.Current.DistrictCode;



            stateListWiseRoadsReportModel.StateList = commonFunctions.PopulateStates(true);
            stateListWiseRoadsReportModel.StateList.RemoveAt(0);
            stateListWiseRoadsReportModel.StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0", Selected = true }));



            stateListWiseRoadsReportModel.StateList.Find(x => x.Value == stateListWiseRoadsReportModel.StateCode.ToString()).Selected = true;

            stateListWiseRoadsReportModel.DistrictList = new List<SelectListItem>();
            if (stateListWiseRoadsReportModel.StateCode == 0)
            {
                stateListWiseRoadsReportModel.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                stateListWiseRoadsReportModel.DistrictList = commonFunctions.PopulateDistrict(stateListWiseRoadsReportModel.StateCode, true);

                stateListWiseRoadsReportModel.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                stateListWiseRoadsReportModel.DistrictList.Find(x => x.Value == stateListWiseRoadsReportModel.DistrictCode.ToString()).Selected = true;

            }
            stateListWiseRoadsReportModel.BlockList = new List<SelectListItem>();
            if (stateListWiseRoadsReportModel.DistrictCode == 0)
            {
                stateListWiseRoadsReportModel.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                stateListWiseRoadsReportModel.BlockList = commonFunctions.PopulateBlocks(stateListWiseRoadsReportModel.DistrictCode, true);
                //stateListWiseRoadsReportModel.BlockCode = fetchCookie.BlockCode == 0 ? 0 : fetchCookie.BlockCode;
                stateListWiseRoadsReportModel.BlockList.Find(x => x.Value == "-1").Value = "0";
                stateListWiseRoadsReportModel.BlockList.Find(x => x.Value == stateListWiseRoadsReportModel.BlockCode.ToString()).Selected = true;
            }

            stateListWiseRoadsReportModel.Year = 0;
            stateListWiseRoadsReportModel.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();
            stateListWiseRoadsReportModel.BatchList = commonFunctions.PopulateBatch(true);
            stateListWiseRoadsReportModel.FundingAgencyList = commonFunctions.PopulateFundingAgency(true);
            stateListWiseRoadsReportModel.FundingAgencyList.Find(x => x.Value == "-1").Value = "0";


            stateListWiseRoadsReportModel.Status = "%";
            stateListWiseRoadsReportModel.StatusList = new List<SelectListItem>();
            stateListWiseRoadsReportModel.StatusList.Insert(0, (new SelectListItem { Text = "All Status", Value = "%", Selected = true }));
            stateListWiseRoadsReportModel.StatusList.Insert(1, (new SelectListItem { Text = "Pending Proposals ", Value = "N" }));
            stateListWiseRoadsReportModel.StatusList.Insert(2, (new SelectListItem { Text = "Sanctioned Proposals", Value = "Y" }));
            stateListWiseRoadsReportModel.StatusList.Insert(1, (new SelectListItem { Text = "Un-Sanctioned Proposals", Value = "U" }));
            stateListWiseRoadsReportModel.StatusList.Insert(2, (new SelectListItem { Text = "Recommended Proposals", Value = "R" }));
            stateListWiseRoadsReportModel.StatusList.Insert(2, (new SelectListItem { Text = "Dropped Proposals", Value = "D" }));

            return View(stateListWiseRoadsReportModel);

        }



        public ActionResult CCBTReport(CCBTViewModel stateListWiseRoadsModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    stateListWiseRoadsModel.LevelCode = stateListWiseRoadsModel.RoadWise == true ? 4 : stateListWiseRoadsModel.BlockCode > 0 ? 3 : stateListWiseRoadsModel.DistrictCode > 0 ? 2 : 1;
                    stateListWiseRoadsModel.Mast_State_Code = stateListWiseRoadsModel.StateCode > 0 ? stateListWiseRoadsModel.StateCode : stateListWiseRoadsModel.Mast_State_Code;
                    stateListWiseRoadsModel.Mast_District_Code = stateListWiseRoadsModel.DistrictCode > 0 ? stateListWiseRoadsModel.DistrictCode : stateListWiseRoadsModel.Mast_District_Code;
                    stateListWiseRoadsModel.Mast_Block_Code = stateListWiseRoadsModel.BlockCode > 0 ? stateListWiseRoadsModel.BlockCode : stateListWiseRoadsModel.Mast_Block_Code;


                    return View(stateListWiseRoadsModel);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                    //return View("StateListWiseRoadsLayout");
                }
            }
            catch
            {
                return View(stateListWiseRoadsModel);
            }

        }
        #endregion

        #region NQM Report --> NQM List

        //URL: /QMSSRSReports/QMSSRSReports/NQMList
        public ActionResult NQMList()
        {

            IMasterDAL objDAL = new MasterDAL();

            List<MASTER_STATE> stateList = objDAL.GetAllStateNames();
            stateList.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" });
            ViewBag.StateList = new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");

            //List<SelectListItem> empanelledList = new List<SelectListItem>();
            //empanelledList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            //empanelledList.Insert(0, (new SelectListItem { Text = "Yes", Value = "Y", Selected = true }));
            //empanelledList.Insert(1, (new SelectListItem { Text = "No", Value = "N" }));
            //ViewBag.EmpanelledList = empanelledList;
            return View();
        }


        //URL: /QMSSRSReports/QMSSRSReports/MonitorReport
        public ActionResult MonitorReport(int QMState, string QMEmp, string QMType)
        {
            ViewBag.StateCode = QMState;
            ViewBag.Empanelled = QMEmp;
            ViewBag.QMType = QMType;
            return View();
        }


        #endregion

        #region Joint Inspection
        //   URL: /QMSSRSReports/QMSSRSReports/QMJIReportLayout
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult QMJIReportLayout()
        {

            CommonFunctions objCommonFunctions = new CommonFunctions();
            PMGSYEntities entities = new PMGSYEntities();

            try
            {
                QMJIViewModel qmJiViewModel = new QMJIViewModel();
                qmJiViewModel.MAST_STATE_CODE = 0;

                qmJiViewModel.FROM_MONTH = DateTime.Now.Month;
                qmJiViewModel.FROM_YEAR = DateTime.Now.Year;
                qmJiViewModel.TO_MONTH = DateTime.Now.Month;
                qmJiViewModel.TO_YEAR = DateTime.Now.Year;
                qmJiViewModel.RoleID = PMGSYSession.Current.RoleCode;
                qmJiViewModel.PublicRepresentativeCode = "A";

                qmJiViewModel.StateList = objCommonFunctions.PopulateStates(false);
                qmJiViewModel.StateList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));


                qmJiViewModel.PublicRepresentativeList.Insert(0, (new SelectListItem { Text = "All", Value = "A", Selected = true }));
                qmJiViewModel.PublicRepresentativeList.Insert(0, (new SelectListItem { Text = "MP", Value = "P", Selected = true }));
                qmJiViewModel.PublicRepresentativeList.Insert(0, (new SelectListItem { Text = "MLA", Value = "L", Selected = true }));
                qmJiViewModel.PublicRepresentativeList.Insert(0, (new SelectListItem { Text = "GP Level", Value = "G", Selected = true }));
                qmJiViewModel.PublicRepresentativeList.Insert(0, (new SelectListItem { Text = "Other", Value = "O", Selected = true }));



                qmJiViewModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmJiViewModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                qmJiViewModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmJiViewModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();

                return View(qmJiViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (objCommonFunctions != null)
                {
                    objCommonFunctions.Dispose();
                }
            }
        }
        //   URL: /QMSSRSReports/QMSSRSReports/QMJIReport
        public ActionResult QMJIReport(QMJIViewModel qmJiViewModel)
        {

            try
            {
                return View(qmJiViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }
        #endregion

        #region Contractor wise report developed by Deen Dayal Sharma on 05-052017 instructed by srinivas sir
        [HttpGet]
        public ActionResult ContractorWiseReportLayout()
        {
            try
            {
                List<SelectListItem> yearList = new CommonFunctions().PopulateYears();
                ContractorWiseReportModel model = new ContractorWiseReportModel();
                model.YEAR_INSPECTION_LIST = yearList;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorWiseReportLayout()");
                return null;
            }
        }

        [ValidateAntiForgeryTokenAttribute]
        [HttpPost]
        public ActionResult LoadContractorwiseReport(ContractorWiseReportModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoadContractorwiseReport()");
                return null;
            }
        }





        #endregion

        #region Inspection of Roads having 5 years difference between Physical Completion date and Inspection Date
        //Added by Deendayal on 05/01/2017
        [HttpGet]
        public ActionResult MaintenanceInspectionsLayout()
        {
            MaintenanceInspModel model = new MaintenanceInspModel();
            CommonFunctions comf = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.StateList = comf.PopulateStates(true).Where(x => x.Value.Equals(PMGSYSession.Current.StateCode.ToString())).ToList();
                    model.DistrictList = comf.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.DistrictList.RemoveAt(0);
                    model.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts", Selected = true });
                    model.MonthList = comf.PopulateMonths(false);
                    model.YearList = comf.PopulateYears(false);
                }
                else
                {
                    model.StateList = comf.PopulateStates(true);
                    model.StateList.RemoveAt(0);
                    model.StateList.Insert(0, new SelectListItem { Value = "0", Text = "All States", Selected = true });

                    model.MonthList = comf.PopulateMonths(false);
                    model.YearList = comf.PopulateYears(false);
                    model.DistrictList = new List<SelectListItem>{
                                          new SelectListItem{ Value= "0" , Text="All Districts",Selected = true}                  
                                        };
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.QMSSRSReports.MaintenanceInspectionsLayout()");
                return null;
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult DistrictDetails(int stateCode)
        {
            try
            {
                CommonFunctions comf = new CommonFunctions();
                List<SelectListItem> districtList = comf.PopulateDistrict(stateCode);
                return Json(districtList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.QMSSRSReports.DistrictDetails(int stateCode)");
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult LoadMaintenanceInspReport(MaintenanceInspModel model)
        {
            if (ModelState.IsValid)
            {
                var dbContext = new PMGSYEntities();
                model.StateName = model.StateCode == 0 ? "All States" : dbContext.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.StateCode).SingleOrDefault().MAST_STATE_NAME;
                model.DistrictName = model.DistrictCode == 0 || model.StateCode == 0 ? "All Districts" : dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == model.DistrictCode).SingleOrDefault().MAST_DISTRICT_NAME;
                return View(model);
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region MP Visit Report
        [HttpGet]
        public ViewResult MPVisitReportLayout()
        {
            MPVisitModel model = new MPVisitModel();
            CommonFunctions commonObj = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.RoleCode == 2)
                {
                    model.StateList.Add(commonObj.PopulateStates(false).Where(s => s.Value == PMGSYSession.Current.StateCode + "").FirstOrDefault());
                    model.DistrictList = commonObj.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.DistrictList.FirstOrDefault(x => x.Value == "-1").Value = "0";

                }
                if (PMGSYSession.Current.RoleCode == 25)
                {
                    model.StateList = commonObj.PopulateStates(true);
                    model.StateList.FirstOrDefault(x => x.Value == "0").Text = "All State";
                    model.DistrictList = commonObj.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.DistrictList.FirstOrDefault(x => x.Value == "-1").Value = "0";
                }

                model.YearList = commonObj.PopulateYears(false);
                model.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All", Selected = true });
                model.VisitYearList = commonObj.PopulateYears(false);
                model.VisitYearList.Insert(0, new SelectListItem { Value = "0", Text = "All", Selected = true });


                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MPVisitReportLayout()");
                return null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetDistricts(String StateCode)
        {
            CommonFunctions commonObj = new CommonFunctions();
            try
            {
                List<SelectListItem> lstDistrict = commonObj.PopulateDistrict(Convert.ToInt32(StateCode), true);
                lstDistrict.FirstOrDefault(x => x.Value == "-1").Value = "0";
                return Json(lstDistrict, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetDistricts()");
                return null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult MPVisitReport(MPVisitModel model)
        {
            try
            {
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MPVisitReport()");
                return null;
            }

        }


        public ViewResult ShowImages(String id)
        {
            QMReportsDAL QMDAL = new QMReportsDAL();
            ViewData["ImageList"] = QMDAL.GetVisitImages(Convert.ToInt32(id));
            return View();
        }

        #endregion

        #region Regrade Grading Abstract
        [HttpGet]
        public ViewResult RegradeGradingAbstractLayout()
        {
            QMRegradeGradingAbstractViewModel model = new QMRegradeGradingAbstractViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.collabList = new List<SelectListItem>();
                model.collabList.Insert(0, new SelectListItem { Text = "All Collaborations", Value = "0" });

                model.qmTypeList = new List<SelectListItem>();
                model.qmTypeList.Insert(0, new SelectListItem { Text = "SQM", Value = "S" });
                model.qmTypeList.Insert(0, new SelectListItem { Text = "NQM", Value = "I" });

                model.qmType = "I";

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RegradeGradingAbstractLayout()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult RegradeGradingAbstractReport(QMRegradeGradingAbstractViewModel model)
        {
            try
            {
                model.collaboration = 0;
                model.agency = 0;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RegradeGradingAbstractReport()");
                return null;
            }
        }
        #endregion

        #region District wise road details report

        public ActionResult DistrictWiseWorksReport()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DistrictWiseWorksReport()");
                //throw;
                return null;
            }
        }

        #endregion


        #region  QM ATR Pending Status 13 July 2018 Added By Rohit Jadhav.

        [HttpGet]
        public ViewResult PendingATRStatusLayout()
        {
            QMRegradeGradingAbstractViewModel1 model = new QMRegradeGradingAbstractViewModel1();
            CommonFunctions comm = new CommonFunctions();
            try
            {

                model.lstFromMonth = comm.PopulateMonths(true);
                model.lstToMonth = comm.PopulateMonths(true);

                model.lstFromYear = comm.PopulateYears(true);
                model.lstToYear = comm.PopulateYears(true);

                model.fromYear = 2010;//DateTime.Now.Year;
                model.fromMonth = DateTime.Now.Month;
                model.toYear = DateTime.Now.Year;
                model.toMonth = DateTime.Now.Month;

                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode), Selected = true }));
                }
                else
                {
                    model.StateList = comm.PopulateStates(true);
                    model.StateList.RemoveAt(0);
                    model.StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0", Selected = true }));
                }



                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PendingATRStatusLayout()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PendingATRStatusReport(QMRegradeGradingAbstractViewModel1 model)
        {
            try
            {
                if (model.fromYear > model.toYear)
                {
                    return Json(new { Success = false, ErrorMessage = "From Year should be less than or equal to To Year" });
                }
                //model.collaboration = 0;
                //model.agency = 0;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PendingATRStatusReport()");
                return null;
            }

        }

        #endregion
        //Added by abhinav pathak on 28/09/2018
        #region InProgressWorkOfContractor Report

        public ActionResult QMInProgressWorkReport()
        {
            InProgressWorkViewModel QM = new InProgressWorkViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                QM.Year = DateTime.Now.Year;
                QM.State = PMGSYSession.Current.StateCode;
                QM.StateList = comm.PopulateStates(true);
                QM.StateList.RemoveAt(0);
                QM.StateList.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
                QM.YearList = new List<SelectListItem>();
                QM.YearList.Add(new SelectListItem { Text = "Select Year", Value = "0", Selected = true });

                QM.YearList.Add(new SelectListItem { Text = "2018", Value = "2018" });
                QM.YearList.Add(new SelectListItem { Text = "2017", Value = "2017" });

                QM.schemeTypeList = new List<SelectListItem>();
                QM.schemeTypeList.Add(new SelectListItem { Text = "Select Scheme", Value = "0", Selected = true });
                QM.schemeTypeList.Add(new SelectListItem { Text = "PMGSY 1", Value = "1" });
                QM.schemeTypeList.Add(new SelectListItem { Text = "PMGSY 2", Value = "2" });
                QM.schemeTypeList.Add(new SelectListItem { Text = "RCPLWE", Value = "3" });
                ViewBag.Year = QM.YearList;
                return View(QM);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRSReports.QMSSRSReports.ShowCashPaymentReport()");
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult QMInProgressWorkReportPost(InProgressWorkViewModel QM)
        {
            InProgressWorkViewModel QMNew = new InProgressWorkViewModel();
            try
            {
                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.StateName != null)
                    {
                        QM.StateName = PMGSYSession.Current.StateName;
                        QMNew.State = QM.State;
                        QMNew.Year = QM.Year;
                        QMNew.schemeType = QM.schemeType;

                    }
                    else
                    {
                        QM.StateName = QM.State == 0 ? "All States" : QM.StateName;
                        QM.State = QM.State;
                        QM.Year = QM.Year;
                        QM.schemeType = QM.schemeType;
                    }
                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region
        //DRRP report added by Abhinav Pathak on 09/10/2018
        public ActionResult BlockReport()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                PMGSYEntities entities = new PMGSYEntities();

                DRRPReportModel blockModel = new DRRPReportModel();
                if (PMGSYSession.Current.StateCode > 0)
                {
                    blockModel.States = PMGSYSession.Current.StateCode;
                    //model.mastStateCode = PMGSYSession.Current.StateCode;
                    //model.stateName = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    List<SelectListItem> lstDist = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(blockModel.States), Text = Convert.ToString(PMGSYSession.Current.StateName) });
                    blockModel.StatesList = new SelectList(lstState, "Value", "Text").ToList();
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(PMGSYSession.Current.DistrictCode), Text = Convert.ToString(PMGSYSession.Current.DistrictName) });
                        blockModel.DistrictsList = new SelectList(lstDist, "Value", "Text").ToList();
                        blockModel.BlocksList = objCommonFunctions.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                        blockModel.BlocksList.Find(x => x.Value == "-1").Value = "0";
                    }
                    else
                    {
                        blockModel.DistrictsList = objCommonFunctions.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                        blockModel.BlocksList = objCommonFunctions.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                        blockModel.DistrictsList.Find(x => x.Value == "-1").Value = "0";
                        blockModel.BlocksList = objCommonFunctions.PopulateBlocks(31, false);
                        blockModel.BlocksList = objCommonFunctions.PopulateBlocks(PMGSYSession.Current.StateCode, true);
                        blockModel.BlocksList.Find(x => x.Value == "-1").Value = "0";
                    }

                }

                else
                {
                    blockModel.StatesList = objCommonFunctions.PopulateStates(true);
                    blockModel.DistrictsList = objCommonFunctions.PopulateDistrict(0, false);
                    blockModel.BlocksList = objCommonFunctions.PopulateBlocks(0, false);
                    //ViewBag.Dis = blockModel.DistrictsList;
                }
                return View(blockModel);
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRSReports.QMSSRSReports.BlockReport()");
                return null;
            }
            finally
            {
                objCommonFunctions.Dispose();
            }


        }


        [HttpPost]
        public ActionResult BlockReportPost(DRRPReportModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {
                    //model.mastStateCode = model.stateCode > 0 ? model.stateCode : model.mastStateCode;


                    //model.mastDistrictCode = model.districtCode > 0 ? model.districtCode : model.mastDistrictCode;
                    //model.districtName = model.districtCode == 0 ? "All" : model.districtName.Trim();
                    //model.StateName = PMGSYSession.Current.StateName;
                    if (model.StateName != null)
                    {
                        model.StateName = model.StateName;
                    }
                    else
                    {
                        model.StateName = PMGSYSession.Current.StateName;
                    }

                    model.StateName = model.States == 0 ? "All" : model.StateName.Trim();
                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            finally
            {

            }
        }
        #endregion

        #region DRRP Surface_Wise Report

        //DRRP SurfaceWise report added by Abhinav Pathak on 10/10/2018

        public ActionResult SurfaceWiseReport()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                PMGSYEntities entities = new PMGSYEntities();

                DRRPSurfaceWiseReportModel blockModel = new DRRPSurfaceWiseReportModel();
                if (PMGSYSession.Current.StateCode > 0)
                {
                    blockModel.States = PMGSYSession.Current.StateCode;
                    //model.mastStateCode = PMGSYSession.Current.StateCode;
                    //model.stateName = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    List<SelectListItem> lstDist = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(blockModel.States), Text = Convert.ToString(PMGSYSession.Current.StateName) });
                    blockModel.StatesList = new SelectList(lstState, "Value", "Text").ToList();
                    blockModel.StateName = PMGSYSession.Current.StateName;
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(PMGSYSession.Current.DistrictCode), Text = Convert.ToString(PMGSYSession.Current.DistrictName) });
                        blockModel.DistrictsList = new SelectList(lstDist, "Value", "Text").ToList();
                        blockModel.BlocksList = objCommonFunctions.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                        blockModel.BlocksList.Find(x => x.Value == "-1").Value = "0";
                    }
                    else
                    {
                        blockModel.DistrictsList = objCommonFunctions.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                        blockModel.DistrictsList.Find(x => x.Value == "-1").Value = "0";
                        blockModel.BlocksList = objCommonFunctions.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                        blockModel.PMGSY = PMGSYSession.Current.PMGSYScheme;
                    }

                }

                else
                {
                    blockModel.StatesList = objCommonFunctions.PopulateStates(true);
                    blockModel.DistrictsList = objCommonFunctions.PopulateDistrict(0, false);
                    //ViewBag.Dis = blockModel.DistrictsList;
                }
                return View(blockModel);
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRSReports.QMSSRSReports.SurfaceWiseReport()");
                return null;
            }
            finally
            {
                objCommonFunctions.Dispose();
            }


        }


        [HttpPost]
        public ActionResult SurfaceWiseReportPost(DRRPSurfaceWiseReportModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {
                    //model.mastStateCode = model.stateCode > 0 ? model.stateCode : model.mastStateCode;
                    //model.stateName = model.stateCode == 0 ? "All" : model.stateName.Trim();

                    //model.mastDistrictCode = model.districtCode > 0 ? model.districtCode : model.mastDistrictCode;
                    //model.districtName = model.districtCode == 0 ? "All" : model.districtName.Trim();
                    if (model.StateName != null)
                    {
                        model.StateName = model.StateName;
                    }
                    else
                    {
                        model.StateName = PMGSYSession.Current.StateName;
                    }

                    model.PMGSY = PMGSYSession.Current.PMGSYScheme;
                    //model.DistName = PMGSYSession.Current.DistrictName;
                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            finally
            {

            }
        }
        #endregion

        #region Work Not Inspected By NQM report
        //Work Not Inspected By NQM report added by Abhinav Pathak on 24/10/2018

        public ActionResult ProgressWorkByNQMReport()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                PMGSYEntities entities = new PMGSYEntities();

                WorkNotInspectedByNQMModel nqmModel = new WorkNotInspectedByNQMModel();
                if (PMGSYSession.Current.StateCode > 0)
                {
                    nqmModel.States = PMGSYSession.Current.StateCode;
                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(nqmModel.States), Text = Convert.ToString(PMGSYSession.Current.StateName) });
                    nqmModel.StatesList = new SelectList(lstState, "Value", "Text").ToList();
                }

                else
                {
                    nqmModel.StatesList = objCommonFunctions.PopulateStates(false);
                }

                return View(nqmModel);
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRSReports.QMSSRSReports.BlockReport()");
                return null;
            }
            finally
            {
                objCommonFunctions.Dispose();
            }

        }

        [HttpPost]
        public ActionResult ProgressWorkByNQMReportPost(WorkNotInspectedByNQMModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {

                    if (model.StateName != null)
                    {
                        model.StateName = model.StateName;
                    }
                    else
                    {
                        model.StateName = PMGSYSession.Current.StateName;
                    }

                    model.StateName = model.States == 0 ? "All" : model.StateName.Trim();
                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            finally
            {

            }
        }
        #endregion

        #region Contractor Works Not Inspected
        [HttpGet]
        public ActionResult ContractorWorksNotInspectedLayout()
        {
            ContractorWorksNotInspectedViewModel model = new ContractorWorksNotInspectedViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {

                model.lstState = comm.PopulateStates(true);
                model.lstFromYear = comm.PopulateYears(true);
                model.lstToYear = comm.PopulateYears(true);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRS.ContractorWorksNotInspectedLayout()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ContractorWorksNotInspectedReport(ContractorWorksNotInspectedViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRS.ContractorWorksNotInspectedReport()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Contractor Inspections Report
        [HttpGet]
        public ActionResult ContractorInspectionLayout()
        {
            ContractorInspectionViewModel model = new ContractorInspectionViewModel();
            CommonFunctions comm = new CommonFunctions();
            
            try
            {
                model.lstFromYear = comm.PopulateYears(true);
                model.lstToYear = comm.PopulateYears(true);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRSReports.ContractorInspectionLayout()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ContractorInspectionModReport(ContractorInspectionViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRSReports.ContractorInspectionReport()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Auto Schedule
        [HttpGet]
        public ActionResult DistrictAssignedNQMLayout()
        {
            PMGSY.Models.QualityMonitoring.AllocateDistrictsToNQMViewModel model = new PMGSY.Models.QualityMonitoring.AllocateDistrictsToNQMViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.lstState = comm.PopulateStates(false);
                model.MonthList = comm.PopulateMonths(false);
                model.YearList = comm.PopulateYears(false);

                if (System.DateTime.Now.Month == 12)
                {
                    model.YearList.Insert(0, (new SelectListItem { Text = (System.DateTime.Now.Year + 1).ToString(), Value = (System.DateTime.Now.Year + 1).ToString() }));
                }
                //model.YearList.Insert(0, new SelectListItem { Text = (DateTime.Now.Year + 1).ToString(), Value = (DateTime.Now.Year + 1).ToString() });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRSReports.QMSSRSReports.DistrictAssignedNQMLayout()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DistrictAssignedNQMReport(PMGSY.Models.QualityMonitoring.AllocateDistrictsToNQMViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRSReports.QMSSRSReports.DistrictAssignedNQMReport()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult NQMAutoScheduledWorksLayout()
        {
            PMGSY.Models.QualityMonitoring.AllocateDistrictsToNQMViewModel model = new PMGSY.Models.QualityMonitoring.AllocateDistrictsToNQMViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.lstState = comm.PopulateStates(false);
                model.MonthList = comm.PopulateMonths(false);
                model.YearList = comm.PopulateYears(false);
                //model.YearList.Insert(0, new SelectListItem { Text = (DateTime.Now.Year + 1).ToString(), Value = (DateTime.Now.Year + 1).ToString() });
                if (System.DateTime.Now.Month == 12)
                {
                    model.YearList.Insert(1, (new SelectListItem { Text = (System.DateTime.Now.Year + 1).ToString(), Value = (System.DateTime.Now.Year + 1).ToString() }));
                }


                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRSReports.QMSSRSReports.DistrictAssignedNQMLayout()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult NQMAutoScheduledWorksReport(PMGSY.Models.QualityMonitoring.AllocateDistrictsToNQMViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMSSRSReports.QMSSRSReports.DistrictAssignedNQMReport()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region Queek Response Sheet

        public ActionResult ResponseLayout()
        {
            ResponseSheet QM = new ResponseSheet();
            CommonFunctions comm = new CommonFunctions();
            SelectListItem itm;

            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                QM.MonitorList = new List<SelectListItem>();

                if (PMGSYSession.Current.RoleName.Equals("NQM"))
                {
                    QM.MonitorList = PopulateSingleMonitor("true", "I", 0);
                }
                else
                {
                    QM.MonitorList = comm.PopulateAllMonitors("false", "I", 0);
                }




                QM.SchemeList = new List<SelectListItem>();
                QM.SchemeList = PopulateSchemeForQueekResponse();


                QM.State = PMGSYSession.Current.StateCode;
                QM.StateList = comm.PopulateStates(true);
                QM.StateList.RemoveAt(0);
                QM.StateList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                QM.DistrictList = new List<SelectListItem>();
                QM.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));

                //QM.FromYear = DateTime.Now.Year - 1;
                //QM.FromMonth = 1;

                QM.FromYear = DateTime.Now.Year;
                QM.FromMonth = DateTime.Now.Month;

                QM.FromYearList = comm.PopulateYears(false);
                QM.FromMonthList = comm.PopulateMonths(false);

                return View(QM);
            }
            catch (Exception ex)
            {

                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        public List<SelectListItem> PopulateSingleMonitor(string isPopulateFirstSelect, string qmType, int stateCode)
        {
            List<SelectListItem> lstProfileNames = new List<SelectListItem>();
            PMGSYEntities dbContext = new PMGSYEntities();

            int Admin_QM_Code = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(m => m.ADMIN_QM_CODE).FirstOrDefault();
            try
            {
                SelectListItem item = new SelectListItem();

                if (qmType.Equals("0"))
                {
                    return lstProfileNames;
                }

                if (stateCode == 0)
                {
                    var query = (from c in dbContext.ADMIN_QUALITY_MONITORS
                                 where c.ADMIN_QM_TYPE == qmType
                                 && c.ADMIN_QM_CODE == Admin_QM_Code
                                 select new
                                 {
                                     Value = c.ADMIN_QM_CODE,
                                     Text = (c.ADMIN_QM_FNAME.Equals(null) ? "" : c.ADMIN_QM_FNAME) + " " + (c.ADMIN_QM_MNAME.Equals(null) ? "" : c.ADMIN_QM_MNAME) + " " + (c.ADMIN_QM_LNAME.Equals(null) ? "" : c.ADMIN_QM_LNAME)
                                 }).OrderBy(c => c.Text).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                }
                else
                {
                    //var query = dbContext.qm_statewise_inspection_monitosr_list(stateCode, qmType).ToList();
                    var query = dbContext.qm_statewise_inspection_monitosr_list(stateCode, qmType).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.MONITOR_NAME;
                        item.Value = data.ADMIN_QM_CODE.ToString();
                        lstProfileNames.Add(item);
                    }
                }



                return lstProfileNames;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return null;
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ResponseReport(ResponseSheet QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;
                    QM.StateName = QM.State == 0 ? "All States" : QM.StateName;
                    QM.DistName = QM.District == 0 ? "All Districts" : QM.DistName;
                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }


        public List<SelectListItem> PopulateSchemeForQueekResponse()
        {
            List<SelectListItem> populateScheme = new List<SelectListItem>();
            try
            {
                populateScheme.Insert(0, (new SelectListItem { Text = "All Schemes", Value = "0", Selected = true }));
                populateScheme.Insert(1, (new SelectListItem { Text = "PMGSY 1", Value = "1" }));
                populateScheme.Insert(2, (new SelectListItem { Text = "PMGSY 2", Value = "2" }));
                populateScheme.Insert(3, (new SelectListItem { Text = "RCPLWE", Value = "3" }));
                populateScheme.Insert(4, (new SelectListItem { Text = "PMGSY 3", Value = "4" }));
                return populateScheme;
            }
            catch
            {
                return populateScheme;
            }
        }

        public ActionResult DistrictDetailsQR(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            //list.Find(x => x.Value == "-1").Text = "Select District";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Comparison and Grading Report of NQM SQM By Sachin on 19 May 2020
        [HttpGet]
        public ActionResult CompGradingLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                QM.Year = DateTime.Now.Year;
                QM.State = PMGSYSession.Current.StateCode;
                QM.StateList = comm.PopulateStates(false);
                // QM.StateList.RemoveAt(0);
                // QM.StateList.Insert(0, (new SelectListItem { Text = "Select", Value = "-1" }));
                if (PMGSYSession.Current.RoleCode == 8) //SQC
                {
                    QM.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, false, 0);
                    QM.DistrictList.RemoveAt(0);
                }
                else
                {
                    QM.DistrictList = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();

                }
                QM.DistrictList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                QM.YearList = comm.PopulateYears(false);
                QM.MonthList = comm.PopulateMonths(false);
                QM.MonthList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult CompGradingReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;

                    QM.StateName = QM.State == 0 ? "All States" : QM.StateName;
                    QM.DistName = QM.District == 0 ? "All Districts" : QM.DistName;

                    //QM.BLOCK_NAME = "All Blocks";

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion


        #region NQM SQM GRADE COMPARISION
        public ActionResult NqmSqmGradeLayout()
        {
            grading QM = new grading();
            CommonFunctions comm = new CommonFunctions();


            QM.nqmRoadList = new List<SelectListItem> { 
                                                            new SelectListItem{ Text = "In Progress", Value ="P" , Selected = true }, 
                                                            new SelectListItem{ Text = "Completed", Value ="C" }, 
                                                               new SelectListItem{ Text = "Maintenance", Value ="M" } ,
                                                            new SelectListItem{ Text = "All", Value ="A" } 
                                                         
                                                           
                                                            };

            QM.sqmRoadList = new List<SelectListItem> { 
                                                             new SelectListItem{ Text = "In Progress", Value ="P" , Selected = true }, 
                                                            new SelectListItem{ Text = "Completed", Value ="C" }, 
                                                               new SelectListItem{ Text = "Maintenance", Value ="M" } ,
                                                            new SelectListItem{ Text = "All", Value ="A" } 
                                                            };


            QM.nqmoverallList = new List<SelectListItem> { 
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true }, 
                                                            new SelectListItem{ Text = "Satisfactory", Value ="1" }, 
                                                            new SelectListItem{ Text = "Unsatisfactory", Value ="3" } ,
                                                            new SelectListItem{ Text = "Satisfactory Required improvement", Value ="2" } 
                                                           
                                                            };


            QM.sqmoverallList = new List<SelectListItem> { 
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true }, 
                                                            new SelectListItem{ Text = "Satisfactory", Value ="1" }, 
                                                            new SelectListItem{ Text = "Unsatisfactory", Value ="3" } ,
                                                            new SelectListItem{ Text = "Satisfactory Required improvement", Value ="2" } 
                                                           
                                                            };






            SelectListItem itm;

            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                QM.MonitorList = new List<SelectListItem>();

                if (PMGSYSession.Current.RoleName.Equals("NQM"))
                {
                    QM.MonitorList = PopulateSingleMonitor("true", "I", 0);
                }
                else
                {
                    QM.MonitorList = comm.PopulateAllMonitors("false", "I", 0);
                }




                QM.SchemeList = new List<SelectListItem>();
                QM.SchemeList = PopulateSchemeForQueekResponse();


                QM.State = PMGSYSession.Current.StateCode;
                QM.StateList = comm.PopulateStates(true);
                QM.StateList.RemoveAt(0);
                QM.StateList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                //QM.FromYear = DateTime.Now.Year - 1;
                //QM.FromMonth = 1;

                QM.FromYear = DateTime.Now.Year;
                QM.FromMonth = DateTime.Now.Month;


                QM.FromYearList = comm.PopulateYears(false);
                QM.FromMonthList = comm.PopulateMonths(false);


                QM.ToYear = DateTime.Now.Year;
                QM.ToMonth = DateTime.Now.Month;


                QM.ToYearList = comm.PopulateYears(false);
                QM.ToMonthList = comm.PopulateMonths(false);


                return View(QM);
            }
            catch (Exception ex)
            {

                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult NQMSQMReport(grading QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    QM.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : QM.State;
                    QM.StateName = QM.State == 0 ? "All States" : QM.StateName;
                    QM.DistName = QM.District == 0 ? "All Districts" : QM.DistName;
                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion



        #region SSRS REPORT PDF UPLOADED STATUS OF NQM/SQM
        [Audit]
        public ActionResult InspectionReportStatus()
        {

            CommonFunctions objCommonFunctions = new CommonFunctions();
            QualityReportsViewModel filterViewModel = new QualityReportsViewModel();

            filterViewModel.FromYear = DateTime.Now.Year;
            filterViewModel.ToYear = DateTime.Now.Year;

            filterViewModel.FromMonth = DateTime.Now.Month - 1;
            filterViewModel.ToMonth = DateTime.Now.Month - 1;

            filterViewModel.qmTypeList = new List<SelectListItem>();
            filterViewModel.StateList = new List<SelectListItem>();
            filterViewModel.qmTypeList.Insert(0, (new SelectListItem { Text = "NQM", Value = "I", Selected = true }));
            filterViewModel.qmTypeList.Insert(0, (new SelectListItem { Text = "SQM", Value = "S", Selected = true }));
            filterViewModel.StateList = objCommonFunctions.PopulateStates(false);
            //filterViewModel = new QualityReportsViewModel
            // {
            filterViewModel.FromMonthList = new List<SelectListItem>();
            filterViewModel.FromMonthList = objCommonFunctions.PopulateMonths(false);


            filterViewModel.ToMonthList = new List<SelectListItem>();
            filterViewModel.ToMonthList = objCommonFunctions.PopulateMonths(false);

            filterViewModel.FromYearList = new List<SelectListItem>();
            filterViewModel.FromYearList = objCommonFunctions.PopulateYears(false);

            filterViewModel.ToYearList = new List<SelectListItem>();
            filterViewModel.ToYearList = objCommonFunctions.PopulateYears(false);
            //sachin

            // };
            return View(filterViewModel);
        }

        [Audit]
        [HttpPost]
        public ActionResult InspectionReportStatusReport(QualityReportsViewModel filterViewModel)
        {

            return View(filterViewModel);
        }

        #endregion



        #region NQM Report --> Tour Details
        public ActionResult TourDetailsLayout()
        {
            try
            {
                CommonFunctions com = new CommonFunctions();
                QualityReportsViewModel qmModel = new QualityReportsViewModel
                {
                    MonthList = com.PopulateMonths(true),
                    YearList = com.PopulateYears(true).ToList(),
                };

                return View(qmModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.TourDetailsLayout()");
                return View();
            }
        }

        public ActionResult TourDetailsReport(QualityReportsViewModel model)
        {
            try
            {
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.TourDetailsReport()");
                return View();
            }
        }
        #endregion



        #region ATR Report 
        //Ajinkya
        [HttpGet]
        public ViewResult ATRReportLayout()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.MAST_STATE_CODE = 0;
            qmFilterModel.ADMIN_QM_CODE = 0;
            qmFilterModel.ATR_STATUS = "0";
            qmFilterModel.ROAD_STATUS = "A";

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.TO_MONTH = DateTime.Now.Month;
            qmFilterModel.TO_YEAR = DateTime.Now.Year;

            //if (PMGSYSession.Current.RoleCode == 5)  //CQC
            if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9 || PMGSYSession.Current.RoleCode == 25)
            {
                qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                //qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                //qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "I", 0); //Purposely taken String "false" as argument
                qmFilterModel.MONITORS = objCommonFunctions.PopulateAllMonitors("false", "I", 0).ToList(); //Purposely taken String "false" as argument //Edited on 13-12-2022 to display all monitors

            }
            else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)  //ATRDetails//SQC
            {
                qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                //qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "I", PMGSYSession.Current.StateCode); //Purposely taken String "false" as argument
                qmFilterModel.MONITORS = objCommonFunctions.PopulateAllMonitors("false", "I", PMGSYSession.Current.StateCode).ToList(); //Purposely taken String "false" as argument //Edited on 13-12-2022 to display all monitors

            }
            else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)  //PIU or PIURCPLWE
            {
                qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitorsDistrictWise("false", "I", PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode); //Purposely taken String "false" as argument
            }

            qmFilterModel.schemeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All Schemes", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "PMGSY I", Value ="1" },
                                                            new SelectListItem{ Text = "PMGSY II", Value ="2" } ,
                                                            new SelectListItem{ Text = "RCPLWE", Value ="3" },
                                                            new SelectListItem{ Text = "PMGSY III", Value ="4" }


                                                            };//ATR_Change

            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.ATR_STATUS_LIST = objCommonFunctions.QualityATRStatus();

            qmFilterModel.imsSanctionedList = new List<SelectListItem> {
                                                        new SelectListItem{ Text = "Sanctioned", Value ="Y" , Selected = true },
                                                        new SelectListItem{ Text = "Dropped", Value ="D" },
                                                       };


            qmFilterModel.ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus();
            return View(qmFilterModel);
        }


        //Ajinkya
        [HttpPost]

        public ActionResult ATRStatusReport(QMFilterViewModel model)
        {
            try
            {
                if (model.FROM_YEAR > model.TO_YEAR)
                {
                    return Json(new { Success = false, ErrorMessage = "From Year should be less than or equal to To Year" });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ATRStatusReport()");
                return null;
            }

        }

        #endregion
    }
}