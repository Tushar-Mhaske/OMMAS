using PMGSY.Areas.RCTRC.DAL;
using PMGSY.Areas.RCTRC.Models;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.RCTRC.Controllers
{
    public class RCTRCController : Controller
    {
        PMGSYEntities dbContext = null;
        string message = String.Empty;
        RctrcDAL objDAL = new RctrcDAL();

        public ActionResult RCTRCLayout()
        {
            try
            {
                CommonFunctions objCommonFuntion = new CommonFunctions();
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRCController.RCTRCLayout()");
                return null;
            }

        }

        #region Registration

        public ActionResult RCTRCRegistration()
        {
            RCTRCRegistrationModel model = new RCTRCRegistrationModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();

            try
            {
                model.StateList = PopulateStatesForRCTRC(true);
                model.StateList.RemoveAt(0);
                model.StateList.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
                model.StateList.Find(x => x.Value == model.StateCode.ToString()).Selected = true;


                model.DistrictList = new List<SelectListItem>();
                if (model.StateCode == 0)
                {
                    model.DistrictList.Insert(0, (new SelectListItem { Text = "Select District", Value = "0", Selected = true }));
                }
                else
                {
                    model.DistrictList = objCommonFuntion.PopulateDistrict(model.StateCode, true);
                    model.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                    model.DistrictList.Find(x => x.Value == model.DistrictCode.ToString()).Selected = true;
                }





               // model.DesignationList = objCommonFuntion.PopulateEducation("D"); // Change it later
                model.GraduationList = objCommonFuntion.PopulateEducation("D");
                model.PostGraduationList = objCommonFuntion.PopulateEducation("P");

                model.ComputerAtHomeCodeList = new List<SelectListItem>{ new SelectListItem{ Text="Yes",Value="Y",Selected=true},
                                                                         new SelectListItem{ Text="No",Value="N"}};


                model.ComputerAtOfficeList = new List<SelectListItem>{ new SelectListItem{ Text="Yes",Value="Y",Selected=true},
                                                                         new SelectListItem{ Text="No",Value="N"}};


                model.YearList = objCommonFuntion.PopulateFinancialYear(true, true).ToList();
                model.YearList.Find(x => x.Value == "0").Selected = true;

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.RCTRCRegistration");
                return null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCdRCTRCRegistration(RCTRCRegistrationModel model)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objDAL = new RctrcDAL();
                    if (objDAL.CdRCTRCRegistrationDAL(model, ref message))
                    {
                        message = message == string.Empty ? "TNA Registration details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "TNA Registration details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("RCTRCRegistration", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.AddCdRCTRCRegistration");
                message = message == string.Empty ? "TNA Registration details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GetRCTRCRegistrationList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objDAL = new RctrcDAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {
                rows = objDAL.ListRCTRCRegistrationDetals(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRegdetails(String parameter, String hash, String key)
        {
            objDAL = new RctrcDAL();
            Dictionary<string, string> decryptedParameters = null;

            var en = parameter + "/" + hash + "/" + key;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int RCTRC_Code = Convert.ToInt32(decryptedParameters["RCTRCCode"].ToString());

                string result = objDAL.DeleteRegistrationDAL(RCTRC_Code); //FinalizeRSAATRBAL
                if (result.Equals(string.Empty))
                {
                    return Json(new { Success = true, Code = en }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = result, Code = en, encryptedURLID = en }, JsonRequestBehavior.DenyGet);
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution().DeleteByAuditor()");
                return null;
            }

        }

        #endregion

        #region Training

        public ActionResult RCTRCTraining()
        {
            RCTRCTraining model = new RCTRCTraining();
            CommonFunctions objCommonFuntion = new CommonFunctions();

            try
            {
                model.ContactPerson_List = objCommonFuntion.PopulateContactPerson();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.RCTRCTraining");
                return null;
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRCTRCTraining(RCTRCTraining model)
        {
            bool status = false;

            try
            {
                ModelState.Remove("ContactPersonIDSearch");
                if (ModelState.IsValid)
                {
                    objDAL = new RctrcDAL();
                    if (objDAL.AddTrainingDAL(model, ref message))
                    {
                        message = message == string.Empty ? "TNA Training details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "TNA Training details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("RCTRCTraining", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.AddRCTRCTraining");
                message = message == string.Empty ? "TNA Training details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public ActionResult GetRCTRCTrainingList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objDAL = new RctrcDAL();
            int PersonCode=0;
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!(string.IsNullOrEmpty(Request.Params["postdataPersonID"])))
            {
                PersonCode = Convert.ToInt32(Request.Params["postdataPersonID"]);

            }

            var jsonData = new
            {
                rows = objDAL.GetRCTRCTrainingListDAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, PersonCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Master Applications

        public ActionResult MasterApplication()
        {
            RCTRCApplications model = new RCTRCApplications();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.ContactPerson_List = objCommonFuntion.PopulateContactPerson();
                model.MASTER_APPLICATIONS_LIST = dbContext.USP_GET_RCTRC_MASTER_APPLICATIONS().ToList<USP_GET_RCTRC_MASTER_APPLICATIONS_Result>();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.MasterApplication");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddApplications(RCTRCApplications model, FormCollection formCollection)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objDAL = new RctrcDAL();
                    if (objDAL.AddApplicationDAL(model, ref message, formCollection))
                    {
                        message = message == string.Empty ? "TNA Application details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "TNA Application details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("MasterApplication", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.AddApplications");
                message = message == string.Empty ? "TNA Application details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetRCTRCApplicationList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int PersonCode = 0;
            objDAL = new RctrcDAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!(string.IsNullOrEmpty(Request.Params["postdataPersonID"])))
            {
                PersonCode = Convert.ToInt32(Request.Params["postdataPersonID"]);

            }

            var jsonData = new
            {
                rows = objDAL.GetRCTRCApplicationListDAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, PersonCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Key Areas of Work

        public ActionResult KeyAreasOfWork()
        {
            RCTRCKeyAreasOfWork model = new RCTRCKeyAreasOfWork();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.ContactPerson_List = objCommonFuntion.PopulateContactPerson();
                model.KeyAreaList = dbContext.RCTRC_MASTER_WORK_AREA.Where(m=>m.RCTRC_WORK_AREA_ACTIVE=="Y").ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.KeyAreasOfWork");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddKeyArea(RCTRCKeyAreasOfWork model, FormCollection formCollection)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objDAL = new RctrcDAL();
                    if (objDAL.AddKeyAreaDAL(model, ref message, formCollection))
                    {
                        message = message == string.Empty ? "TNA Key Areas details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "TNA Key Areas details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("KeyAreasOfWork", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.AddKeyArea");
                message = message == string.Empty ? "TNA Key Areas details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetRCTRCKeyAreaList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int PersonCode = 0;
            objDAL = new RctrcDAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!(string.IsNullOrEmpty(Request.Params["postdataPersonID"])))
            {
                PersonCode = Convert.ToInt32(Request.Params["postdataPersonID"]);

            }
            var jsonData = new
            {
                rows = objDAL.GetRCTRCKeyAreaListDAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, PersonCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        #endregion
        
        #region Subdetails

        public ActionResult Subdetails()
        {
            RCTRCSubdetails model = new RCTRCSubdetails();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
               model.masterWorkAreaList = objCommonFuntion.GetMasterWorkArea();
               return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.Subdetails");
                return null;
            }
            finally 
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }


        public ActionResult SubdetailsPerMaster(int id)
        {
            RCTRCSubdetailsPerMaster model = new RCTRCSubdetailsPerMaster();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.masterAreaName = dbContext.RCTRC_MASTER_WORK_AREA.Where(m => m.RCTRC_WORK_AREA_ID == id).Select(m=>m.RCTRC_WORK_AREA_NAME).FirstOrDefault().ToString();
                model.ContactPerson_List = objCommonFuntion.PopulateContactPerson();
                model.KeyAreaList = dbContext.RCTRC_MASTER_KEY_AREA.Where(m => m.RCTRC_KEY_AREA_PARENT_ID == id).ToList();
                model.masterWorkAreaCode = id;


                // GetMasterWorkArea

                //model.KeyAreaList = dbContext.RCTRC_MASTER_KEY_AREA.Where(m=>m.RCTRC_KEY_AREA_PARENT_ID==1).ToList();
                //model.DesignList = dbContext.RCTRC_MASTER_KEY_AREA.Where(m => m.RCTRC_KEY_AREA_PARENT_ID == 2).ToList();
                //model.ConstructionActivitiesList = dbContext.RCTRC_MASTER_KEY_AREA.Where(m => m.RCTRC_KEY_AREA_PARENT_ID == 3).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.Subdetails");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }
        //AddSubdetails

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSubdetails(RCTRCSubdetailsPerMaster model, FormCollection formCollection)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objDAL = new RctrcDAL();
                    if (objDAL.AddSubdetailsDAL(model, ref message, formCollection))
                    {
                        message = message == string.Empty ? "Key Areas of Work (Subdetails) saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Key Areas of Work (Subdetails) not saved." : message;
                    }
                }
                else
                {
                    return PartialView("SubdetailsPerMaster", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.AddSubdetails");
                message = message == string.Empty ? "Key Areas of Work (Subdetails) not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GetSubdetailsList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int PersonCode = 0;
            objDAL = new RctrcDAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!(string.IsNullOrEmpty(Request.Params["postdataPersonID"])))
            {
                PersonCode = Convert.ToInt32(Request.Params["postdataPersonID"]);

            }
            var jsonData = new
            {
                rows = objDAL.GetSubdetailsListDAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, PersonCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Performance 

        public ActionResult MasterPerformance()
        {
            RCTRCPerformance model = new RCTRCPerformance();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.ContactPerson_List = objCommonFuntion.PopulateContactPerson();
                model.MASTER_PERFORMANCE_LIST = dbContext.RCTRC_MASTER_PERFORMANCE.Where(m=>m.RCTRC_PERF_ACTIVE=="Y").ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.MasterPerformance");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPerformance(RCTRCPerformance model, FormCollection formCollection)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objDAL = new RctrcDAL();
                    if (objDAL.AddPerformanceDAL(model, ref message, formCollection))
                    {
                        message = message == string.Empty ? "TNA Performance details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "TNA Performance details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("MasterPerformance", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.AddPerformance");
                message = message == string.Empty ? "TNA Performance details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetPerformanceList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int PersonCode = 0;
            objDAL = new RctrcDAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!(string.IsNullOrEmpty(Request.Params["postdataPersonID"])))
            {
                PersonCode = Convert.ToInt32(Request.Params["postdataPersonID"]);

            }
            var jsonData = new
            {
                rows = objDAL.GetPerformanceListDAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, PersonCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Suggestion


        public ActionResult Suggestion()
        {
            RCTRCSuggestion model = new RCTRCSuggestion();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.ContactPerson_List = objCommonFuntion.PopulateContactPerson();
                model.KeyAreaList = dbContext.RCTRC_MASTER_SUGGESTION.Where(m=>m.RCTRC_SUGG_ACTIVE=="Y").ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.Suggestion");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSuggestion(RCTRCSuggestion model, FormCollection formCollection)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objDAL = new RctrcDAL();
                    if (objDAL.AddAddSuggestionDAL(model, ref message, formCollection))
                    {
                        message = message == string.Empty ? "Suggestions saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Suggestion details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("Suggestion", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.AddSuggestion");
                message = message == string.Empty ? "Suggestion details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GetRCTRCSuggestionList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int PersonCode = 0;
            objDAL = new RctrcDAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!(string.IsNullOrEmpty(Request.Params["postdataPersonID"])))
            {
                PersonCode = Convert.ToInt32(Request.Params["postdataPersonID"]);

            }
            var jsonData = new
            {
                rows = objDAL.GetRCTRCSuggestionListDAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, PersonCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Training Required
        public ActionResult TrainingRequired()
        {
            RCTRCTrainingRequired model = new RCTRCTrainingRequired();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.ContactPerson_List = objCommonFuntion.PopulateContactPerson();
                model.KeyAreaList = dbContext.RCTRC_MASTER_TRAINING.Where(m=>m.RCTRC_TRAINGING_ACTIVE=="Y").ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.TrainingRequired");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTrainingRequired(RCTRCTrainingRequired model, FormCollection formCollection)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objDAL = new RctrcDAL();
                    if (objDAL.AddTrainingRequiredDAL(model, ref message, formCollection))
                    {
                        message = message == string.Empty ? "Required Training details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Required Training details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("TrainingRequired", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.AddTrainingRequired");
                message = message == string.Empty ? "Required Training details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GetRCTRCTrainingRequiredList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int PersonCode = 0;
            objDAL = new RctrcDAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!(string.IsNullOrEmpty(Request.Params["postdataPersonID"])))
            {
                PersonCode = Convert.ToInt32(Request.Params["postdataPersonID"]);

            }
            var jsonData = new
            {
                rows = objDAL.GetRCTRCTrainingRequiredListDAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, PersonCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Monitors

        [HttpGet]
        public ActionResult MasterQualityMonitor()
        {
            try
            {
                return View();
             }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRCController.AddEditMasterQualityMonitor [HttpGet]");
                return null;
            }
        }

        public ActionResult SearchQualityMonitor()
        {
            try
            {
                //IMasterDAL objDAL = new MasterDAL();
                IRctrcDAL objDAL = new RctrcDAL();
                ViewData["QmTypeList"] = new SelectList(objDAL.GetQmTypes(), "Value", "Text");

                List<MASTER_STATE> stateList = objDAL.GetAllStateNames();
                stateList.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" });

                ViewData["StateList"] = new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");
                stateList.Insert(stateList.Count, new MASTER_STATE { MAST_STATE_CODE = 90, MAST_STATE_NAME = "Central Agency" });
                stateList.Insert(stateList.Count, new MASTER_STATE { MAST_STATE_CODE = 91, MAST_STATE_NAME = "Central Goverment" });

                List<MASTER_DISTRICT> districtList = objDAL.GetAllDistrictByStateCode(0);
                districtList.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "All Districts" });
                ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");

                List<SelectListItem> empanelledList = new List<SelectListItem>();
                empanelledList.Insert(0, (new SelectListItem { Text = "Yes", Value = "Y", Selected = true }));
                empanelledList.Insert(1, (new SelectListItem { Text = "No", Value = "N" }));
                ViewData["EmpanelledList"] = empanelledList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRCController.SearchQualityMonitor");
                ViewData["QmTypeList"] = null;
                ViewData["StateList"] = null;
                ViewData["DistrictList"] = null;
                ViewData["EmpanelledList"] = null;
            }
            return PartialView("SearchQualityMonitor");
        }

        [HttpGet]
        public ActionResult AddOfficialDetails()
        {
            try
            {
                MasterAdminQualityMonitorViewModel model = new MasterAdminQualityMonitorViewModel();
                var deptList = new PMGSYEntities().ADMIN_DEPARTMENT.Where(x => x.MAST_ND_TYPE == "S" && x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).ToList();
                List<SelectListItem> l = new List<SelectListItem>();
                model.AdminDepartmentList = new List<SelectListItem>();
                foreach (var item in deptList)
                {
                    SelectListItem tempitem = new SelectListItem();
                    tempitem.Value = Convert.ToString(item.ADMIN_ND_CODE);
                    tempitem.Text = item.ADMIN_ND_NAME;
                    model.AdminDepartmentList.Add(tempitem);
                }
                model.AdminDepartmentList.Insert(0, new SelectListItem { Value = "0", Text = "Select Department", Selected = true });
                return PartialView("AddOfficialDetails", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRCController.AddEditMasterQualityMonitor [HttpGet]");
                return null;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOfficialDetailsPost(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel)
        {
            bool status = false;
            try
            {
                if (masterQualityMonitorViewModel.ADMIN_QM_TYPE.Equals("S"))
                {
                }

               // ModelState.Remove("ADMIN_QM_PAN");
             //   ModelState.Remove("ADMIN_SERVICE_TYPE");
                ModelState.Remove("ADMIN_QM_TYPE");
               // ModelState.Remove("DeEmpanelledRemark");

                if (ModelState.IsValid)
                {
                    RctrcDAL objDAL = new RctrcDAL();
                    if (objDAL.AddMasterQualityMonitor(masterQualityMonitorViewModel, ref message))
                    {
                        message = message == string.Empty ? "Details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddOfficialDetails", masterQualityMonitorViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRCController.AddOfficialDetailsPost [HttpPost]");
                message = "Details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMasterQualityMonitorList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            long totalRecords;
            int stateCode = 0;
            int districtCode = 0;
            string isEmpanelled = String.Empty;
            string firstName = String.Empty;
            string qmTypeName = String.Empty;

            try
            {
                RctrcDAL objDAL = new RctrcDAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["QmTypeName"]))
                {
                    qmTypeName = Request.Params["QmTypeName"].Replace('+', ' ').Trim();
                }

                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["isEmpanelled"]))
                {
                    isEmpanelled = Request.Params["isEmpanelled"];
                }


                var jsonData = new
                {
                    rows = objDAL.ListQualityMonitor(qmTypeName, stateCode, districtCode, isEmpanelled, Request.Params["filters"], Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        // Set User Name 
        [HttpGet]
        public ActionResult AddSQMUserLoginQualityMonitorDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                RctrcDAL objDAL = new RctrcDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int qualityMonitorCode = Convert.ToInt32(decryptedParameters["QmCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.AddSQMUserLoginQualityMonitorDAL(qualityMonitorCode, ref message))
                    {
                        message = message == string.Empty ? "TNA Official's username added successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Username not added." : message;
                    }

                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRCController.AddSQMUserLoginQualityMonitorDetails()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Common Methods
        public ActionResult DistrictDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]),false);
           // list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public List<SelectListItem> PopulateStatesForRCTRC(bool isPopulateFirstItem = true)
        {
            List<SelectListItem> StatesList = new List<SelectListItem>();
            SelectListItem item;

            if (isPopulateFirstItem)
            {
                item = new SelectListItem();
                item.Text = "Select State";
                item.Value = "0";
                item.Selected = true;
                StatesList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All States";
                item.Value = "0";
                item.Selected = true;
                StatesList.Add(item);

            }

            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_STATE
                             join map in dbContext.RCTRC_USERID_STATE_MAPPING on c.MAST_STATE_CODE equals map.STATE_CODE
                             where c.MAST_STATE_ACTIVE == "Y" && map.USER_ID == PMGSYSession.Current.UserId
                             select new
                             {
                                 Text = c.MAST_STATE_NAME,
                                 Value = c.MAST_STATE_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    StatesList.Add(item);
                }
                return StatesList;
            }
            catch
            {
                return StatesList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion


        #region  Monitor State Mapping


        public ActionResult RCTRCStateUserMapping()
        {
            RCTRCStateUserMapping model = new RCTRCStateUserMapping();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            dbContext = new PMGSYEntities();

            try
            {
                model.StateList = PopulateStatesForMapping(0);
                
                model.UserList = objCommonFuntion.GetRCTRCUsersForStateMapping();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.RCTRCStateUserMapping");
                return null;
            }

        }

        public ActionResult PopulateStateForUser(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = getStateCodeTobeExcluded(Convert.ToInt32(frmCollection["userID"]));
         
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> getStateCodeTobeExcluded(int userID)
        {
            List<SelectListItem> StatesList = new List<SelectListItem>();
            SelectListItem item;


            try
            {
                dbContext = new PMGSYEntities();

                var RemoveTheseStateCodes = (from map in dbContext.RCTRC_USERID_STATE_MAPPING
                                             where map.USER_ID == userID
                                             select new
                                             {
                                                 Text = map.MASTER_STATE.MAST_STATE_NAME,
                                                 Value = map.STATE_CODE
                                             }).OrderBy(c => c.Text).ToList();



                foreach (var data in RemoveTheseStateCodes)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    StatesList.Add(item);
                }
                return StatesList;
            }
            catch
            {
                return StatesList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        public List<SelectListItem> PopulateStatesForMapping(int userID)
        {
            List<SelectListItem> StatesList = new List<SelectListItem>();
            SelectListItem item;


            try
            {
                dbContext = new PMGSYEntities();

                var RemoveTheseStateCodes = (from map in dbContext.RCTRC_USERID_STATE_MAPPING
                                             where map.USER_ID == userID //(userID == 0 ? map.USER_ID : userID)
                                             select new
                                             {
                                                 Text = map.MASTER_STATE.MAST_STATE_NAME,
                                                 Value = map.STATE_CODE
                                             }).OrderBy(c => c.Text).ToList();


                var query = (from c in dbContext.MASTER_STATE
                             where c.MAST_STATE_ACTIVE == "Y"
                             select new
                             {
                                 Text = c.MAST_STATE_NAME,
                                 Value = c.MAST_STATE_CODE
                             }).OrderBy(c => c.Text).ToList();


                var FinalQuery = query.Except(RemoveTheseStateCodes);

                foreach (var data in FinalQuery)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    StatesList.Add(item);
                }
                return StatesList;
            }
            catch
            {
                return StatesList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

       


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUserStateMapingDetails(RCTRCStateUserMapping model, FormCollection formCollection)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objDAL = new RctrcDAL();
                    if (objDAL.StateUserMapDAL(model, ref message, formCollection))
                    {
                        message = message == string.Empty ? "TNA User and State mapped successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "TNA User and State not mapped." : message;
                    }
                }
                else
                {
                    return PartialView("RCTRCStateUserMapping", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC.AddUserStateMapingDetails");
                message = message == string.Empty ? "TNA User and State not mapped." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GetUserStateMappingList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objDAL = new RctrcDAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {
                rows = objDAL.GetMappedStateUserListDAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion 
    }
}
