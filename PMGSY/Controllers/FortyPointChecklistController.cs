/*----------------------------------------------------------------------------------------
 * Project Id:

 * Project Name:OMMAS II

 * File Name: FortyPointChecklistController.cs

 * Author : Abhishek Kamble.

 * Creation Date :20/Nov/2013

 * Desc : This class is used as controller to perform Save,Edit,Update,Delete and listing of Forty point check list Module.  
 *
 * ---------------------------------------------------------------------------------------*/

using PMGSY.Common;
using PMGSY.BAL.FortyPointChecklist;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.FortyPointCheckList;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class FortyPointChecklistController : Controller
    {
        //
        // GET: /FortyPointChecklist/

        private PMGSYEntities dbContext = new PMGSYEntities();
        CommonFunctions commonFunction = new CommonFunctions();
        FortyPointChecklistBAL fortyPointChecklistBALContext = new FortyPointChecklistBAL();


        #region Forty Point Checklist Details

            public ActionResult FortyPointCheckListDetails()
            {
                return View("FortyPointCheckListDetails");
            }

            public ActionResult ListFortyPointChecklistDetails(FormCollection formCollection)
            {
                try
                {
                    using (CommonFunctions commonFunction = new CommonFunctions())
                    {
                        if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                        {
                            return null;
                        }
                    }

                    int totalRecords;
                    int stateCode = PMGSYSession.Current.StateCode;
                    int adminNdCode = PMGSYSession.Current.AdminNdCode;

                    var jsonData = new
                    {
                        rows = fortyPointChecklistBALContext.ListFortyPointCheckListDetails(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, stateCode, adminNdCode),
                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }   
                catch (Exception)
                {
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
                }
            }

            public ActionResult ListConstructionLabEquipmentDetails(FormCollection formCollection)
            {
                try
                {
                    int totalRecords;
                    int stateCode = PMGSYSession.Current.StateCode;
                    int adminCode = PMGSYSession.Current.AdminNdCode;
                    
                    using (CommonFunctions commonFunction = new CommonFunctions())
                    {
                        if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                        {
                            return null;
                        }
                    }

                    var jsonData = new
                    {
                        rows = fortyPointChecklistBALContext.ListConstructionLabEquipmentDetails(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, stateCode, adminCode),

                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords,
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
                }
            }

            /// <summary>
            /// Add/Edit Forty Point Checklist View Model
            /// </summary>
            /// <param name="formcollection"></param>
            /// <returns></returns>
            public JsonResult AddEditFortyPointChecklistDetails(FormCollection formcollection)
            {
                string id=formcollection["id"];       //check before convert
                int checkListPointId;
                string actionReply = formcollection["MAST_ACTION_TAKEN"];
                string message=string.Empty;

                //validation for ID
                if(Regex.IsMatch(id,"^([0-9]+)$"))
                {                       
                    checkListPointId=Convert.ToInt32(id);

                    if((checkListPointId<0) || (checkListPointId>Int32.MaxValue))
                    {                                                                
                        message="Invalid Chekck List Point Id";
                        return Json(message, JsonRequestBehavior.AllowGet);                              
                    }   
                }
                else{
                    message="Invalid Chekck List Point Id";
                    return Json(message, JsonRequestBehavior.AllowGet);                              
                }

                //Validation For Reply                 
                if (!(Regex.IsMatch(actionReply, "[A-Za-z0-9- .()]{1,2000}")))
                {
                    message = "Only alphanumeric characters are allowed.";
                    return Json(message, JsonRequestBehavior.AllowGet);
                }
                
                try
                {
                        FortyPointCheckListViewModel checkListViewModel = new FortyPointCheckListViewModel();
                        checkListViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                        checkListViewModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                        checkListViewModel.MAST_CHECKLIST_POINTID =checkListPointId;
                        checkListViewModel.MAST_ACTION_TAKEN = formcollection["MAST_ACTION_TAKEN"];

                        fortyPointChecklistBALContext.AddEditFortyPointCheckList(checkListViewModel,ref message);

                        return Json(message, JsonRequestBehavior.AllowGet);                              
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message += eve.ValidationErrors.ToString();
                    }
                    return Json(message, JsonRequestBehavior.AllowGet);                              
                }
                catch (Exception ex)
                {
                    message = "An error occured while proccessing your request because " + ex.Message;
                    return Json(message, JsonRequestBehavior.AllowGet);                              
                }
            }

            [HttpPost]
            public ActionResult DeleteFortyPointChecklistDetails(string parameter, string hash, string key)
            {
                string message = string.Empty;
                //int checkListPointId;
                try
                {
                    //validation for ID
                    //if (Regex.IsMatch(id, "^([0-9]+)$"))
                    //{
                    //    checkListPointId = Convert.ToInt32(id);

                    //    if ((checkListPointId < 0) || (checkListPointId > Int32.MaxValue))
                    //    {
                    //        message = "Invalid Chekck List Point Id";
                    //        return Json(new { success = false, message = message });
                    //    }
                    //}
                    //else
                    //{
                    //    message = "Invalid Chekck List Point Id";
                    //    return Json(new { success = false, message = message });
                    //}        

                    Dictionary<string, string> decryptedParameters = null;

                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                    if (decryptedParameters.Count > 0)
                    {
                        int checkListPointId = Convert.ToInt32(decryptedParameters["checkListId"].ToString());

                        if (fortyPointChecklistBALContext.DeleteFortyPointCheckListDetails(checkListPointId, ref message))
                        {
                            //message = "Employment Information details deleted successfully.";
                            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                          //  message = "Employment Information details not deleted.";
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

        #endregion Forty Point Checklist Details

        #region Employment Information Details


            /// <summary>
            /// Returns Add Employment Details List
            /// </summary>
            /// <returns></returns>
            
            public ActionResult EmploymentInformationDetails()
            {

                try
                {
                    MASTER_CHECKLIST_POINTS masterCheckListModel = dbContext.MASTER_CHECKLIST_POINTS.Where(m => m.MAST_CHECKLIST_POINTID == 18).FirstOrDefault();
                    if (masterCheckListModel != null)
                    {
                        ViewBag.CheckListId = 18;
                        ViewBag.CheckListIssue = masterCheckListModel.MAST_CHECKLIST_ISSUES;
                    }

                    return View("EmploymentInformationDetails");
                }
                catch (Exception ex)
                {
                    return View("EmploymentInformationDetails");
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
            public ActionResult GetEmploymentInformation(FormCollection formCollection)
            {             
                try
                {
                    int totalRecords;
                    int stateCode = PMGSYSession.Current.StateCode;
                    int adminCode = PMGSYSession.Current.AdminNdCode;

                    using (CommonFunctions commonFunction = new CommonFunctions())
                    {
                        if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"],formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                        {
                            return null;
                        }
                    }

                    var jsonData = new
                    {
                        rows = fortyPointChecklistBALContext.EmploymentInformationDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, stateCode, adminCode),

                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords,

                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                catch(Exception)
                {
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
                }
            }

            /// <summary>
            /// Returns Add Employment Detais Form
            /// </summary>
            /// <returns></returns>
            public ActionResult AddEmploymentInformation()
            {
                return PartialView("AddEmploymentInformation",new EmploymentInformationViewModel());
            }

            /// <summary>
            /// Save Employment Information Details.
            /// </summary>
            /// <param name="employmentInformationViewModel"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult AddEmploymentInformationDetails(EmploymentInformationViewModel employmentInformationViewModel)
            {                                                                                                    
                string message=string.Empty;
                try 
	            {	                       		
                    if(ModelState.IsValid)
                    {
                        if (fortyPointChecklistBALContext.AddEmploymentDetails(employmentInformationViewModel, ref message))
                        {
                            message = message == string.Empty ? "Employment Information Details saved successfully." : message;
                            return Json(new { success = true, message = message });
                        }
                        else {
                            message = message == string.Empty ? "Employment Information Details not saved." : message;
                            return Json(new { success = false, message = message });
                        }
                    }   
                    else{
                        return PartialView("AddEmploymentInformation", employmentInformationViewModel);
                    }

	            }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message += eve.ValidationErrors.ToString();
                    }
                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    message = "Employment Information details not saved because " + ex.Message;
                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }
            }

            /// <summary>
            /// Get Employment Information Details.
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="hash"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            [HttpGet]
            public ActionResult EditEmploymentInformationDetails(String parameter,String hash,String key)
            {
                try
                {
                    Dictionary<string, string> decryptedParameters = null;

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                    if (decryptedParameters.Count > 0)
                    {
                        int employmentId = Convert.ToInt32(decryptedParameters["employmentId"].ToString());

                        EmploymentInformationViewModel employmentInformationViewModel = fortyPointChecklistBALContext.GetEmploymentInformationDetails(employmentId);

                        if (employmentInformationViewModel == null)
                        {
                            ModelState.AddModelError("", "Employment Information Details Not Exist");
                            return PartialView("AddEmploymentInformation", new EmploymentInformationViewModel());                        
                        }
                        return PartialView("AddEmploymentInformation", employmentInformationViewModel);

                    }
                    return PartialView("AddEmploymentInformation", new EmploymentInformationViewModel());
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Employment Information Details Not Exist");
                    return PartialView("AddEmploymentInformation", new EmploymentInformationViewModel());                        
                }            
            }

            /// <summary>
            /// Update Employment Information Details.
            /// </summary>
            /// <param name="employmentInformationViewModel"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult UpdateEmploymentDetails(EmploymentInformationViewModel employmentInformationViewModel)
            {
                string message = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (fortyPointChecklistBALContext.EditEmploymentDetails(employmentInformationViewModel,ref message))
                        {
                            message = message == string.Empty ? "Employment information Details Updated successfully." : message;
                            return Json(new { success = true, message = message });
                        }
                        else
                        {
                            message = message == string.Empty ? "Employment information Details not updated." : message;
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

                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message += eve.ValidationErrors.ToString();
                    }
                    return Json(new { success = false, message = message });
                }
                catch (Exception ex)
                {
                    message = "Employment information Details not saved because " + ex.Message;
                    return Json(new { succes = false, message = message }, JsonRequestBehavior.AllowGet);
                }
            }

            /// <summary>
            /// Delete Employment Information Details.
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="hash"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult DeleteEmploymentInformationDetails(string parameter, string hash, string key)
            {
                string message = string.Empty;

                try
                {
                    Dictionary<string, string> decryptedParameters = null;

                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                    if (decryptedParameters.Count > 0)
                    {
                        int employmentId= Convert.ToInt32(decryptedParameters["employmentId"].ToString());
                    

                        if (fortyPointChecklistBALContext.DeleteEmploymentInformationDetails(employmentId,ref message))
                        {
                            message = "Employment Information details deleted successfully.";
                            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            message = "Employment Information details not deleted.";
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

        #endregion Employment Information Details

        #region Tender Cost Information Details

            /// <summary>
            /// Returns Add Employment Details List
            /// </summary>
            /// <returns></returns>    
            public ActionResult TenderCostInformationDetails()
            {
                
                try
                {
                    MASTER_CHECKLIST_POINTS masterCheckListModel = dbContext.MASTER_CHECKLIST_POINTS.Where(m=>m.MAST_CHECKLIST_POINTID==11).FirstOrDefault();
                    if (masterCheckListModel != null)
                    {
                        ViewBag.CheckListId= 11;
                        ViewBag.CheckListIssue = masterCheckListModel.MAST_CHECKLIST_ISSUES;
                    }

                    return View("TenderCostInformationDetails", masterCheckListModel);
                }
                catch (Exception ex)
                {
                    return View("TenderCostInformationDetails", new TenderCostInformationViewModel());
                }
                finally {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }                
                }                   
            }

            /// <summary>
            /// Returns Json data to Populate Grid
            /// </summary>
            /// <param name="formCollection"></param>
            /// <returns></returns>
            public ActionResult GetTenderCostInformation(FormCollection formCollection)
            {
                try
                {
                    int totalRecords;
                    int stateCode = PMGSYSession.Current.StateCode;
                    int adminCode = PMGSYSession.Current.AdminNdCode;

                    using (CommonFunctions commonFunction = new CommonFunctions())
                    {
                        if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                        {
                            return null;
                        }
                    }

                    var jsonData = new
                    {
                        rows = fortyPointChecklistBALContext.TenderCostInformationDetailsDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, stateCode, adminCode),

                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords,

                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
                }
            }

            /// <summary>
            /// Display Tender Cost Information Form
            /// </summary>
            /// <returns></returns>
            public ActionResult AddEditTenderCostInformationDetails()
            {
                return PartialView("AddEditTenderCostInformationDetails", new TenderCostInformationViewModel());
            }

            /// <summary>
            /// Save Tender Cost Information Details.
            /// </summary>
            /// <param name="tenderCostInformationViewModel"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult AddTenderCostInformationDetails(TenderCostInformationViewModel tenderCostInformationViewModel)
            {
                string message = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (fortyPointChecklistBALContext.AddTenderCostInformationDetails(tenderCostInformationViewModel, ref message))
                        {
                            message = message == string.Empty ? "Tender Cost Details saved successfully." : message;
                            return Json(new { success = true, message = message });
                        }
                        else
                        {
                            message = message == string.Empty ? "Tender Cost Details not saved." : message;
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return PartialView("AddEditTenderCostInformationDetails", tenderCostInformationViewModel);
                    }

                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message += eve.ValidationErrors.ToString();
                    }
                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    message = "Tender Cost details not saved because " + ex.Message;
                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }
            }

            /// <summary>
            /// Get Tender Cost Information Details.
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="hash"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            [HttpGet]
            public ActionResult EditTenderCostInformationDetails(String parameter, String hash, String key)
            {
                try
                {
                    Dictionary<string, string> decryptedParameters = null;

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                    if (decryptedParameters.Count > 0)
                    {
                        int priceId = Convert.ToInt32(decryptedParameters["priceId"].ToString());

                        TenderCostInformationViewModel tenderCostInformationViewModel = fortyPointChecklistBALContext.GetTenderCostInformationDetails(priceId);

                        if (tenderCostInformationViewModel == null)
                        {
                            ModelState.AddModelError("", "Tender Cost Details Not Exist");
                            return PartialView("AddEditTenderCostInformationDetails", new TenderCostInformationViewModel());
                        }
                        return PartialView("AddEditTenderCostInformationDetails", tenderCostInformationViewModel);

                    }
                    return PartialView("AddEditTenderCostInformationDetails", new TenderCostInformationViewModel());
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Tender Cost Details Not Exist");
                    return PartialView("AddEditTenderCostInformationDetails", new TenderCostInformationViewModel());
                }
            }

            /// <summary>
            /// Update Tender Cost Information Details.
            /// </summary>
            /// <param name="tenderCostViewModel"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult UpdateTenderCostDetails(TenderCostInformationViewModel tenderCostViewModel)
            {
                string message = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (fortyPointChecklistBALContext.EditCostInformationDetails(tenderCostViewModel, ref message))
                        {
                            message = message == string.Empty ? "Tender Cost Details Updated successfully." : message;
                            return Json(new { success = true, message = message });
                        }
                        else
                        {
                            message = message == string.Empty ? "Tender Cost Details not updated." : message;
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

                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message += eve.ValidationErrors.ToString();
                    }
                    return Json(new { success = false, message = message });
                }
                catch (Exception ex)
                {
                    message = "Tender Cost information Details not saved because " + ex.Message;
                    return Json(new { succes = false, message = message }, JsonRequestBehavior.AllowGet);
                }
            }

            /// <summary>
            /// Delete Tender Cost Information Details.
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="hash"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult DeleteTenderCostInformationDetails(string parameter, string hash, string key)
            {
                string message = string.Empty;

                try
                {
                    Dictionary<string, string> decryptedParameters = null;

                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                    if (decryptedParameters.Count > 0)
                    {
                        int priceId = Convert.ToInt32(decryptedParameters["priceId"].ToString());


                        if (fortyPointChecklistBALContext.DeleteTenderCostInformationDetails(priceId, ref message))
                        {
                            message = "Tender Cost details deleted successfully.";
                            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            message = "Tender Cost details not deleted.";
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


        #endregion Tender Cost Information Details

        #region Tender Equipment


            /// <summary>
            /// Display Equipment Details List
            /// </summary>
            /// <returns></returns>    
            public ActionResult TenderEquipmentDetails()
            {
                try
                {
                    MASTER_CHECKLIST_POINTS masterCheckListModel = dbContext.MASTER_CHECKLIST_POINTS.Where(m => m.MAST_CHECKLIST_POINTID == 17).FirstOrDefault();
                    if (masterCheckListModel != null)
                    {
                        ViewBag.CheckListId = 17;
                        ViewBag.CheckListIssue = masterCheckListModel.MAST_CHECKLIST_ISSUES;
                    }

                    return View("TenderEquipmentDetails");
                }
                catch (Exception ex)
                {
                    return View("TenderEquipmentDetails");
                }
                finally
                {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }              
            }

            /// <summary>
            /// Returns Json data to Populate Equipment Details Grid
            /// </summary>
            /// <param name="formCollection"></param>
            /// <returns></returns>
            public ActionResult ListTenderEquipmentDetails(FormCollection formCollection)
            {
                try
                {
                    int totalRecords;
                    int stateCode = PMGSYSession.Current.StateCode;
                    int adminCode = PMGSYSession.Current.AdminNdCode;
                    
                    //string sidx = formCollection["sidx"];
                    //string []sortIndex=sidx.Split(',');

                    using (CommonFunctions commonFunction = new CommonFunctions())
                    {
                        if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                        {
                            return null;
                        }
                    }

                    var jsonData = new
                    {
                        rows = fortyPointChecklistBALContext.ListTenderEquipmentDetails(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, stateCode, adminCode),

                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords,
                    };
                        return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
                }
            }

            /// <summary>
            /// Display Tender Equipment Form
            /// </summary>
            /// <param name="equipmentFlag"></param>
            /// <returns></returns>
            public ActionResult AddEditTenderEquipmentDetails(string equipmentFlag)
            {
                TenderEquipmentViewModel tenderEquipmentViewModel = new TenderEquipmentViewModel();
                
                return PartialView("AddEditTenderEquipmentDetails", tenderEquipmentViewModel);
            }

            /// <summary>
            /// Save Tender Equipment Details.
            /// </summary>
            /// <param name="tenderEquipmentViewModel"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult AddTenderEquipmentDetails(TenderEquipmentViewModel tenderEquipmentViewModel)
            {
                string message = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (fortyPointChecklistBALContext.AddTenderEquipmentDetails(tenderEquipmentViewModel, ref message))
                        {
                            message = message == string.Empty ? "Equipment Details saved successfully." : message;
                            return Json(new { success = true, message = message });
                        }
                        else
                        {
                            message = message == string.Empty ? "Equipment Details not saved." : message;
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return PartialView("AddEditTenderEquipmentDetails", tenderEquipmentViewModel);
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message += eve.ValidationErrors.ToString();
                    }
                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    message = "Equipment details not saved because " + ex.Message;
                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }
            }

            /// <summary>
            /// Get Tender Equipment Details.
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="hash"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            [HttpGet]
            public ActionResult EditTenderEquipmentDetails(String parameter, String hash, String key)
            {
                try
                {
                    Dictionary<string, string> decryptedParameters = null;

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                    if (decryptedParameters.Count > 0)
                    {
                        int equipmentId = Convert.ToInt32(decryptedParameters["equipmentId"].ToString());

                        TenderEquipmentViewModel tenderEquipmentViewModel = fortyPointChecklistBALContext.GetTenderEquipmentDetails(equipmentId);

                        if (tenderEquipmentViewModel== null)
                        {
                            ModelState.AddModelError("", "Tender Equipment Details Not Exist");
                            return PartialView("AddEditTenderEquipmentDetails", new TenderEquipmentViewModel());
                        }
                        return PartialView("AddEditTenderEquipmentDetails", tenderEquipmentViewModel);

                    }
                    return PartialView("AddEditTenderEquipmentDetails", new TenderEquipmentViewModel());
                }
                catch (Exception)
                {
                    ViewBag.ScreenName = "";
                    ModelState.AddModelError("", "Tender Equipment Details Not Exist");
                    return PartialView("AddEditTenderEquipmentDetails", new TenderEquipmentViewModel());
                }
            }

            /// <summary>
            /// Update Tender Equipment Details.
            /// </summary>
            /// <param name="tenderEquipmentViewModel"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult UpdateTenderEquipmentDetails(TenderEquipmentViewModel tenderEquipmentViewModel)
            {
                string message = string.Empty;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (fortyPointChecklistBALContext.EditTenderEquipmentDetails(tenderEquipmentViewModel, ref message))
                        {
                            message = message == string.Empty ? "Equipment Details Updated successfully." : message;
                            return Json(new { success = true, message = message });
                        }
                        else
                        {
                            message = message == string.Empty ? "Equipment Details not updated." : message;
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

                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message += eve.ValidationErrors.ToString();
                    }
                    return Json(new { success = false, message = message });
                }
                catch (Exception ex)
                {
                    message = "Equipment Details not saved because " + ex.Message;
                    return Json(new { succes = false, message = message }, JsonRequestBehavior.AllowGet);
                }
            }

            /// <summary>
            /// Delete Tender Cost Information Details.
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="hash"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult DeleteTenderEquipmentDetails(string parameter, string hash, string key)
            {
                string message = string.Empty;

                try
                {
                    Dictionary<string, string> decryptedParameters = null;

                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                    if (decryptedParameters.Count > 0)
                    {
                        int equipmentId = Convert.ToInt32(decryptedParameters["equipmentId"].ToString());

                        if (fortyPointChecklistBALContext.DeleteTenderEquipmentDetails(equipmentId, ref message))
                        {
                            message = "Equipment details deleted successfully.";
                            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            message = "Equipment details not deleted.";
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
        
        #endregion Tender Equipment
        
        #region commonFunction
            protected override void Dispose(bool disposing)
        {
            dbContext.Dispose();
            base.Dispose(disposing);
        }
        #endregion commonFunction
    }
}
