#region File Header...
/* 
     *  Name : MPMLAProposalController.cs
     *  Path : ~\PMGSY\Controllers\MPMLAProposalController.cs
     *  Description : MPMLAProposalController Class inherits from Controller Class and used to List, MP/MLA Proposal Status Details and add,Edit,Delete MP/MLA Proposal Status
     *  Author : Abhishek Kamlble(PE, e-gov)
     *  Company : C-DAC,E-GOV
     *  Dates of modification : 05/Jul/2013           
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.DAL.MPMLAProposal;
using PMGSY.Common;
using PMGSY.BAL.MPMLAProposal;
using PMGSY.Models.MPMLAProposal;
using System.Data.Entity.Validation;
using PMGSY.Extensions;
namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class MPMLAProposalController : Controller
    {

        public MPMLAProposalController()
        {
            PMGSYSession.Current.ModuleName = "MP/MLA Proposal";
        }

        public ActionResult Index()
        {
            return View();
        }
        
        #region MP Proposal

        /// <summary>
        /// Display MP Proposed Road List
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListMPProposedRoads()
        {
            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }

                IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                ViewData["yearList"] = new SelectList(objDAL.PopulateYear(0, true, true), "Value", "Text");
                ViewData["constituencyList"] = new SelectList(objDAL.PopulateMPConstatuency(), "Value", "Text");
                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
               ViewData["yearList"] = null;
                ViewData["constituencyList"] = null;
                return View();
            }
        }
    
        /// <summary>
        /// GetMPProposedRoadList() this actions displays MP Proposed Road List
        /// </summary>
        /// <returns>return json data to display on grid</returns>
        [HttpPost]
        [Audit]
        public ActionResult GetMPProposedRoadList(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current.UserId == 0)
            {
                Response.Redirect("/Login/SessionExpire");           
            }

            long totalRecords;            
            int yearCode= 0;
            int constituencyCode = 0;

            IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["Year"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["Year"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["Constituency"]))
                {
                    constituencyCode = Convert.ToInt32(Request.Params["Constituency"]);
                }

                var jsonData = new
                {
                    rows = objBAL.ListMPProposedRoadList(yearCode,constituencyCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
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

        /// <summary>
        /// AddEditMPProposedRoadDetails() Actions shows MP Proposed Road Details Data Entry Form
        /// </summary>
        /// <returns> Returns partial view Of MP Proposed Road Data Entry Form </returns>
        [Audit]
        public ActionResult AddEditMPProposedRoadDetails()
        {
            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }

                IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();                   
                return PartialView("AddEditMPProposedRoadDetails", new MPProposalViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        /// <summary>
        /// AddWorkProgramDetails() Action is used to Save Work program Data into database
        /// </summary>
        /// <param name="WorkProgramViewModel"> AddWorkProgramDetails Contains All Entered Work program Form Data</param>
        /// <returns>If data is successfully Saved then return success message else return error message </returns>
        [HttpPost]
        [Audit]
        public ActionResult AddMPProposedRoadDetails(MPProposalViewModel mpProposalViewModel)
        {
            String message = String.Empty;
            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }

                if (ModelState.IsValid)
                {
                    IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();

                    if (objBAL.AddMPProposedRoadDetails(mpProposalViewModel, ref message))
                    {   
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {   
                    return PartialView("AddEditMPProposedRoadDetails", mpProposalViewModel);
                }
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.ApplicationInstance.Context);
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message = message + eve.ValidationErrors.ToString();
                    }
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "An Error Occured While proccessing your request";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditWorkProgramDetails() is used to Display Work Program Data entry Form
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditMPProposedRoadDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int mpConstituencyCode = Convert.ToInt32(decryptedParameters["MPConstCode"]);
                int imsYear = Convert.ToInt32(decryptedParameters["ImsYear"]);
                int ImsRoadId = Convert.ToInt32(decryptedParameters["ImsRoadId"]);
                MPProposalViewModel mpProposalViewModel = null;

                if (decryptedParameters.Count() > 0)
                {
                    IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();

                    mpProposalViewModel = objBAL.GetMPProposedRoadDetails(imsYear, mpConstituencyCode, ImsRoadId);
                    if (mpProposalViewModel == null)
                    {
                        ModelState.AddModelError(String.Empty, "MP Proposed Road details not exist");
                        return PartialView("AddEditMPProposedRoadDetails", new MPProposalViewModel());
                    }
                    return PartialView("AddEditMPProposedRoadDetails", mpProposalViewModel);
                }
                return PartialView("AddEditMPProposedRoadDetails", mpProposalViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "MP Proposed Road details not exist");
                return PartialView("AddEditMPProposedRoadDetails", new MPProposalViewModel());
            }   
        }


        /// <summary>
        /// EditWorkProgramDetails() Action is used to Update Work program Data into database
        /// </summary>
        /// <param name="WorkProgramViewModel"> workProgramViewModel Contains All Entered Work program Form Data</param>
        /// <returns>If data is successfully Saved then return success message else return error message </returns>
        [HttpPost]
        [Audit]
        public ActionResult EditMPProposedRoadDetails(MPProposalViewModel mpProposalViewModel)
        {
            String message = String.Empty;
                
            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }
                if (ModelState.IsValid)
                {
                    IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();
                    if (objBAL.EditMPProposedRoadDetails(mpProposalViewModel, ref message))
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string messages = string.Join(";</br>", ModelState.Values
                                                .SelectMany(x => x.Errors)
                                                .Select(x => x.ErrorMessage));

                    return Json(new { success = false, message = messages }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.ApplicationInstance.Context);
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message = message + eve.ValidationErrors.ToString();
                    }
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "An Error Occured While proccessing your request";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        ///DeleteWorkProgramDetails() this Action is used to delete Perticular Work Program Details From database  
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>this function return success message if Work Program Details successfully Deleted else shows error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteMPProposedRoadDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            string message = String.Empty;
            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int mpConstituencyCode = Convert.ToInt32(decryptedParameters["MPConstCode"]);
                int imsYear = Convert.ToInt32(decryptedParameters["ImsYear"]);
                int ImsRoadId = Convert.ToInt32(decryptedParameters["ImsRoadId"]);

                if (decryptedParameters.Count() > 0)
                {
                    IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();

                    if (objBAL.DeleteMPProposedRoadDetails(imsYear,mpConstituencyCode,ImsRoadId,ref message))
                    {
                        return Json(new { success = true, message = "MP Proposal Details details deleted successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "MP Proposal Details details can not be deleted because other details for mp proposal are entered." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message = "There is an error occured while processing your request." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "There is an error occured while processing your request." }, JsonRequestBehavior.AllowGet);
            }
        }
        
        
        #endregion

        #region MP Proposal Inclusion

        /// <summary>
        /// AddEditMPProposalInclusionDetails() Action is used to display MP Proposal Road Inclusion Details Data Entry form
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddEditMPProposalInclusionDetails(String parameter, String hash, String key)
        {
            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }

                Dictionary<string, string> decryptedParameters = null;
                
                MPRoadProposalInclusionViewModel mpProposalInclusionViewModel = null;

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int mpConstituencyCode = Convert.ToInt32(decryptedParameters["MPConstCode"]);
                int imsYear = Convert.ToInt32(decryptedParameters["ImsYear"]);
                int ImsRoadId = Convert.ToInt32(decryptedParameters["ImsRoadId"]);

                if (decryptedParameters.Count() > 0)
                {
                    IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();
                    mpProposalInclusionViewModel = objBAL.GetMPProposalRoadInclusionDetails(imsYear, mpConstituencyCode, ImsRoadId);
                    if (mpProposalInclusionViewModel == null)
                    {
                        ModelState.AddModelError(String.Empty, "MP Proposal Road Inclusion details not exist");
                        return PartialView("AddEditMPProposalInclusionDetails", new MPProposalViewModel());
                    }
                    return PartialView("AddEditMPProposalInclusionDetails", mpProposalInclusionViewModel);
                }
                return PartialView("AddEditMPProposalInclusionDetails", new MPRoadProposalInclusionViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "MP Proposal Road inclusion details not exist");
                return PartialView("AddEditMPProposalInclusionDetails", new MPRoadProposalInclusionViewModel());
            }
        }
       
        /// <summary>
        /// AddMPProposalRoadInclusionDetails() Action is used to Save MP Proposal Road Inclusion Details into database
        /// </summary>
        /// <param name="mpProposalInclusionViewModel"> mpProposalInclusionViewModel Contains All Entered MP Proposal Status data</param>
        /// <returns>If data is successfully Saved then return success message else return error message </returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddMPProposalRoadInclusionDetails(MPRoadProposalInclusionViewModel mpProposalInclusionViewModel)
        {
            String message = String.Empty;
            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }
                if (ModelState.IsValid)
                {
                    IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();

                    if (objBAL.AddMPProposalRoadInclusionDetails(mpProposalInclusionViewModel, ref message))
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return View("AddEditMPProposalInclusionDetails", mpProposalInclusionViewModel);
                }
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.ApplicationInstance.Context);
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message = message + eve.ValidationErrors.ToString();
                    }
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "An Error Occured While proccessing your request";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region MLA Proposal

        /// <summary>
        /// Display MLA Proposed Road List
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListMLAProposedRoads()
        {
            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }
                IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                ViewData["yearList"] = new SelectList(objDAL.PopulateYear(0, true, true), "Value", "Text");
                ViewData["constituencyList"] = new SelectList(objDAL.PopulateMLAConstatuency(), "Value", "Text");
                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["yearList"] = null;
                ViewData["constituencyList"] = null;
                return View();
            }
        }

        /// <summary>
        /// GetMLAProposedRoadList() this actions displays MLA Proposed Road List
        /// </summary>
        /// <returns>return json data to display on grid</returns>
        [HttpPost]
        [Audit]
        public ActionResult GetMLAProposedRoadList(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current.UserId == 0)
            {
                Response.Redirect("/Login/SessionExpire");           
            }
            long totalRecords;
            int yearCode = 0;
            int constituencyCode = 0;

            IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["Year"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["Year"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["Constituency"]))
                {
                    constituencyCode = Convert.ToInt32(Request.Params["Constituency"]);
                }

                var jsonData = new
                {
                    rows = objBAL.ListMLAProposedRoadList(yearCode, constituencyCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
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

        /// <summary>
        /// AddEditMLAProposedRoadDetails() Actions shows MLA Proposed Road Details Data Entry Form
        /// </summary>
        /// <returns> Returns partial view Of MLA Proposed Road Data Entry Form </returns>
        [Audit]
        public ActionResult AddEditMLAProposedRoadDetails()
        {
            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }
                IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                return PartialView("AddEditMLAProposedRoadDetails", new MLAProposalViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        /// <summary>
        /// AddMPProposedRoadDetails() Action is used to Save MLA details into database
        /// </summary>
        /// <param name="WorkProgramViewModel"> mpProposalViewModel Contains All Entered MLA Form Data</param>
        /// <returns>If data is successfully Saved then return success message else return error message </returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddMLAProposedRoadDetails(MLAProposalViewModel mlaProposalViewModel)
        {
            String message = String.Empty;
            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }
                if (ModelState.IsValid)
                {
                    IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();

                    if (objBAL.AddMLAProposedRoadDetails(mlaProposalViewModel, ref message))
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return PartialView("AddEditMLAProposedRoadDetails", mlaProposalViewModel);
                }
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.ApplicationInstance.Context);
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message = message + eve.ValidationErrors.ToString();
                    }
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "An Error Occured While proccessing your request";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditMPProposedRoadDetails() is used to Display MLA Proposal Data entry Form
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditMLAProposedRoadDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int mlaConstituencyCode = Convert.ToInt32(decryptedParameters["MLAConstCode"]);
                int imsYear = Convert.ToInt32(decryptedParameters["ImsYear"]);
                int ImsRoadId = Convert.ToInt32(decryptedParameters["ImsRoadId"]);
                MLAProposalViewModel mlaProposalViewModel = null;

                if (decryptedParameters.Count() > 0)
                {
                    IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();

                    mlaProposalViewModel = objBAL.GetMLAProposedRoadDetails(imsYear, mlaConstituencyCode, ImsRoadId);
                    if (mlaProposalViewModel == null)
                    {
                        ModelState.AddModelError(String.Empty, "MLA Proposed Road details not exist");
                        return PartialView("AddEditMLAProposedRoadDetails", new MLAProposalViewModel());
                    }
                    return PartialView("AddEditMLAProposedRoadDetails", mlaProposalViewModel);
                }
                return PartialView("AddEditMLAProposedRoadDetails", mlaProposalViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "MLA Proposed Road details not exist");
                return PartialView("AddEditMLAProposedRoadDetails", new MLAProposalViewModel());
            }
        }


        /// <summary>
        /// EditMLAProposedRoadDetails() Action is used to Update MLA Proposal Data into database
        /// </summary>
        /// <param name="mlaProposalViewModel"> mlaProposalViewModel Contains All Entered MLA Form Data</param>
        /// <returns>If data is successfully Saved then return success message else return error message </returns>
        [HttpPost]
        [Audit]
        public ActionResult EditMLAProposedRoadDetails(MLAProposalViewModel mlaProposalViewModel)
        {
            String message = String.Empty;

            try
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }
                if (ModelState.IsValid)
                {
                    IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();
                    if (objBAL.EditMLAProposedRoadDetails(mlaProposalViewModel, ref message))
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string messages = string.Join(";</br>", ModelState.Values
                                              .SelectMany(x => x.Errors)
                                              .Select(x => x.ErrorMessage));

                    return Json(new { success = false, message = messages }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.ApplicationInstance.Context);
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message = message + eve.ValidationErrors.ToString();
                    }
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "An Error Occured While proccessing your request";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        ///DeleteMLAProposedRoadDetails() this Action is used to delete MLA Proposal Details From database  
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>this function return success message if Work Program Details successfully Deleted else shows error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteMLAProposedRoadDetails(String parameter, String hash, String key)
        {
            if (PMGSYSession.Current.UserId == 0)
            {
                Response.Redirect("/Login/SessionExpire");           
            }
            Dictionary<string, string> decryptedParameters = null;
            string message = String.Empty;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int mlaConstituencyCode = Convert.ToInt32(decryptedParameters["MLAConstCode"]);
                int imsYear = Convert.ToInt32(decryptedParameters["ImsYear"]);
                int ImsRoadId = Convert.ToInt32(decryptedParameters["ImsRoadId"]);

                if (decryptedParameters.Count() > 0)
                {
                    IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();

                    if (objBAL.DeleteMLAProposedRoadDetails(imsYear, mlaConstituencyCode, ImsRoadId, ref message))
                    {
                        return Json(new { success = true, message = "MLA Proposal Details details deleted successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "MP Proposal Details details can not be deleted because other details for mla proposal are entered." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message = "There is an error occured while processing your request." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "There is an error occured while processing your request." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region MLA Proposal Inclusion

        /// <summary>
        /// AddEditMLAProposalInclusionDetails() Action is used to display MLA Proposal Road Inclusion Details Data Entry form
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddEditMLAProposalInclusionDetails(String parameter, String hash, String key)
            {
                try
                {
                    if (PMGSYSession.Current.UserId == 0)
                    {
                        Response.Redirect("/Login/SessionExpire");           
                    }
                    Dictionary<string, string> decryptedParameters = null;

                    MLARoadProposalInclusionViewModel mlaProposalInclusionViewModel = null;

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                    int mlaConstituencyCode = Convert.ToInt32(decryptedParameters["MLAConstCode"]);
                    int imsYear = Convert.ToInt32(decryptedParameters["ImsYear"]);
                    int ImsRoadId = Convert.ToInt32(decryptedParameters["ImsRoadId"]);

                    if (decryptedParameters.Count() > 0)
                    {
                        IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();
                        mlaProposalInclusionViewModel = objBAL.GetMLAProposalRoadInclusionDetails(imsYear, mlaConstituencyCode, ImsRoadId);
                        if (mlaProposalInclusionViewModel == null)
                        {
                            ModelState.AddModelError(String.Empty, "MLA Proposal Road Inclusion details not exist");
                            return PartialView("AddEditMLAProposalInclusionDetails", new MLAProposalViewModel());
                        }
                        return PartialView("AddEditMLAProposalInclusionDetails", mlaProposalInclusionViewModel);
                    }
                    return PartialView("AddEditMLAProposalInclusionDetails", new MLARoadProposalInclusionViewModel());
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    ModelState.AddModelError(String.Empty, "MLA Proposal Road inclusion details not exist");
                    return PartialView("AddEditMLAProposalInclusionDetails", new MLARoadProposalInclusionViewModel());
                }
            }

        /// <summary>
        /// AddMLAProposalRoadInclusionDetails() Action is used to Save MLA Proposal Road Inclusion Details into database
        /// </summary>
        /// <param name="mlaProposalInclusionViewModel"> mlaProposalInclusionViewModel Contains All Entered Mla Proposal Status data</param>
        /// <returns>If data is successfully Saved then return success message else return error message </returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddMLAProposalRoadInclusionDetails(MLARoadProposalInclusionViewModel mlaProposalInclusionViewModel)
            {
                String message = String.Empty;
                try
                {
                    if (PMGSYSession.Current.UserId == 0)
                    {
                        Response.Redirect("/Login/SessionExpire");           
                    }
                    if (ModelState.IsValid)
                    {
                        IMPMLAProposalBAL objBAL = new MPMLAProposalBAL();

                        if (objBAL.AddMLAProposalRoadInclusionDetails(mlaProposalInclusionViewModel, ref message))
                        {
                            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return View("AddEditMLAProposalInclusionDetails", mlaProposalInclusionViewModel);
                    }
                }
                catch (DbEntityValidationException e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.ApplicationInstance.Context);
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ModelState.AddModelError("", eve.ValidationErrors.ToString());
                            message = message + eve.ValidationErrors.ToString();
                        }
                    }
                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    message = "An Error Occured While proccessing your request";
                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }
            }

        #endregion

        #region Pupulate Dropdown

        /// <summary>
        /// PopulateImsRoads() is used to Populate Ims Road Names For MP/MLA
        /// </summary>
        /// <returns></returns>
            [HttpPost]
        [Audit]
        public JsonResult PopulateImsRoads()
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }
                int outParam = 0;

                string blockCode = Request.Params["BLOCK_CODE"];
                string yearCode = Request.Params["YEAR"];

                try
                {
                    if (!int.TryParse(blockCode, out outParam))
                    {
                        return Json(false);
                    }

                    if (!int.TryParse(yearCode, out outParam))
                    {
                        return Json(false);
                    }

                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return Json(new SelectList(objDAL.PopulateImsSanctionedRoads(Convert.ToInt32(blockCode.Trim()), Convert.ToInt32(yearCode.Trim())), "Value", "Text"));
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    return Json(false);
                }
            }//end function
        /// <summary>
        /// PopulateDrrpRoads() is used to Populate Core Network Road Names For MP/MLA
        /// </summary>
        /// <returns></returns>
            [HttpPost]
            [Audit]
            public JsonResult PopulateDrrpRoads()
            {
                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");           
                }
                string blockCode = Request.Params["MAST_BLOCK_CODE"];

                int outParam = 0;
                try
                {
                    if (!int.TryParse(blockCode, out outParam))
                    {
                        return Json(false);
                    }
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return Json(new SelectList(objDAL.PopulateDrrpRoads(Convert.ToInt32(blockCode.Trim())), "Value", "Text"));
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    return Json(false);
                }
            }//end function
        
        #endregion
    }
}
