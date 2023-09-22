/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: LocationMasterDataEntryController.cs

 * Author : Koustubh Nakate

 * Creation Date :05/Apr/2013

 * Desc : This controller is used as get the request and send response as view for location master data entry screens.  
 * ---------------------------------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.MasterDataEntry;
using PMGSY.BAL;
using PMGSY.Common;
using PMGSY.Extensions;
using System.Configuration;
using System.IO;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    [Audit]
    public class LocationMasterDataEntryController : Controller
    {
        public LocationMasterDataEntryController()
        {
            PMGSYSession.Current.ModuleName = "Master Data Entry";
        }

        private PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();

        IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();
        PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;

        string message = string.Empty;
        int outParam = 0;
        //
        // GET: /LocationMasterDataEntry/

        #region GenericMasterDataEntry


        public ActionResult MasterDataEntry()
        {
            return View();
        }

        #endregion GenericMasterDataEntry

        #region StateDataEntry


        public ActionResult StateDetails()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetStateDetailsList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateUT = 0;
            int stateType = 0;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                //var jsonData = new
                //{
                //    rows = masterDataEntryBAL.GetStateDetailsListBAL(false, Convert.ToInt32(homeFormCollection["page"]) - 1, Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], out totalRecords), 
                //    total = totalRecords <= Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(homeFormCollection["rows"]) + 1,
                //    page = Convert.ToInt32(homeFormCollection["page"]),
                //    records = totalRecords
                //};
                if (!string.IsNullOrEmpty(Request.Params["StateUT"]))
                {
                    stateUT = Convert.ToInt32(Request.Params["StateUT"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["StateType"]))
                {
                    stateType = Convert.ToInt32(Request.Params["StateType"]);
                }
                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetStateDetailsListBAL(false, (Int32)page - 1, (Int32)rows, sidx, sord, out totalRecords, stateUT, stateType),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }

        }




        public ActionResult CreateState()
        {
            return PartialView("CreateState", new MASTER_STATE());
        }

        [HttpGet]
        public ActionResult SearchState()
        {
            try
            {
                MASTER_STATE model = new MASTER_STATE();
                //List<SelectListItem> lstst = new List<SelectListItem>();
                //lstst = new SelectList(model.StateUTs, "Value", "Text").ToList();
                //lstst.RemoveAt(0);
                return PartialView("SearchState", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["AgencyType"] = null;

            }
            return PartialView("SearchAgencyType");
        }

        //
        // POST: /LocationMasterDataEntry/Create


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MASTER_STATE master_state)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.SaveStateDetailsBAL(master_state, ref message))
                    {
                        //ModelState.AddModelError(string.Empty, "State Details added successfully."); 
                        message = message == string.Empty ? "State/UT details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        //ModelState.AddModelError(string.Empty, message == string.Empty ? "State Details not added successfully." : message);                       
                        message = message == string.Empty ? "State/UT details not saved." : message;
                    }

                }
                else
                {
                    return PartialView("CreateState", master_state);
                }

                //return View(master_state);
                // return PartialView("CreateState",master_state);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                /*ModelState.AddModelError(string.Empty, "State Details not added successfully.");
                     return PartialView("CreateState", master_state);*/
                message = "State/UT details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /LocationMasterDataEntry/Edit/5

        public ActionResult Edit(String parameter, String hash, String key)
        {
            // Dictionary<string,string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    MASTER_STATE master_state = masterDataEntryBAL.GetStateDetailsBAL_ByStateCode(Convert.ToInt32(decryptedParameters["StateCode"].ToString()));

                    if (master_state == null)
                    {
                        //return HttpNotFound();
                        ModelState.AddModelError(string.Empty, "State/UT details not exist.");
                        // return View();
                        return PartialView("CreateState", new MASTER_STATE());
                    }
                    //return View(master_state);
                    return PartialView("CreateState", master_state);

                }
                //return View();
                return PartialView("CreateState", new MASTER_STATE());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "State/UT details not exist.");
                //return View();
                return PartialView("CreateState", new MASTER_STATE());
            }
        }

        //
        // POST: /LocationMasterDataEntry/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MASTER_STATE master_state)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.UpdateStateDetailsBAL(master_state, ref message))
                    {
                        // ModelState.AddModelError(string.Empty, "State details updated successfully.");                    
                        message = message == string.Empty ? "State/UT details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        // ModelState.AddModelError(string.Empty, message == string.Empty ? "State details not updated successfully." : message);    
                        message = message == string.Empty ? "State/UT details not updated." : message;
                    }

                }
                else
                {
                    return PartialView("CreateState", master_state);
                }

                //return PartialView("CreateState", master_state);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                /* ModelState.AddModelError(string.Empty, "State details not updated successfully.");
                      return PartialView("CreateState", master_state);*/
                message = message == string.Empty ? "State/UT details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /LocationMasterDataEntry/Delete/5


        public ActionResult DeleteStateDetails(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    //IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.DeleteStateDetailsBAL_ByStateCode(Convert.ToInt32(decryptedParameters["StateCode"].ToString()), ref message))
                    {
                        message = "State/UT details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "State/UT details not deleted." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "State details/UT not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "State/UT details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult FinalizeState(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    bool status = masterDataEntryBAL.FinalizeStateBAL(Convert.ToInt32(decryptedParameters["StateCode"]));
                    if (status == true)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                return Json(new { success = false });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult FinalizeDistrict(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    bool status = masterDataEntryBAL.FinalizeDistrictBAL(Convert.ToInt32(decryptedParameters["DistrictCode"]));
                    if (status == true)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                return Json(new { success = false });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult FinalizeBlock(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    bool status = masterDataEntryBAL.FinalizeBlockBAL(Convert.ToInt32(decryptedParameters["BlockCode"]));
                    if (status == true)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                return Json(new { success = false });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }


        #endregion StateDataEntry

        #region DistrictDataEntry


        public ActionResult DistrictDetails()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetDistrictDetailsList(int? page, int? rows, string sidx, string sord)//FormCollection homeFormCollection
        {
            string[] agency = null;
            int agencyCode = 0;
            int regionCode = 0;
            int adminNdCode = 0;
            long totalRecords;
            int stateCode = 0;
            try
            {
                /*if (!string.IsNullOrEmpty(homeFormCollection["searchField"]))
                {
                    searchParameters = HttpUtility.HtmlDecode(homeFormCollection["searchField"]);

                    searchParameters = searchParameters.Replace("%2F", "/");
                    string[] str = (searchParameters.ToString().Split('&'));
                    for (int i = 0; i < str.Length; ++i)
                    {
                        string[] splitParameter = str[i].Split('=');
                        parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                    }

                    stateCode = Convert.ToInt16(parameters["ddlSearchStates"]);

                }*/
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }
                else
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["AgencyCode"]))
                {
                    agency = Convert.ToString(Request.Params["AgencyCode"]).Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { agency[0], agency[1], agency[2] });
                    agencyCode = Convert.ToInt32(decryptedParameters["TACode"]);
                }

                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetDistrictDetailsListBAL(agencyCode, regionCode, adminNdCode, false, DAL.MappingType.DistrictDetails, stateCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }





        public ActionResult CreateDistrict()
        {

            MASTER_DISTRICT master_district = new MASTER_DISTRICT();
            if (PMGSYSession.Current.RoleCode == 36)
            {
                master_district.MAST_STATE_CODE = Convert.ToInt32(PMGSYSession.Current.StateCode);
            }
            master_district.IsPMGSYIncluded = true;
            master_district.IsIAPDistrict = false;
            return PartialView("CreateDistrict", master_district);
        }



        public ActionResult SearchDistrict()
        {
            try
            {
                if (PMGSYSession.Current.RoleCode == 36)
                {
                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME", PMGSYSession.Current.StateCode);//masterDataEntryDAL.GetAllStates(true);
                }
                else
                {
                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME");//masterDataEntryDAL.GetAllStates(true);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["StateList"] = null;
            }

            return PartialView("SearchDistrict");
        }


        //
        // POST: /DistrictMasterDataEntry/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDistrict(MASTER_DISTRICT master_district)
        {
            bool status = false;
            try
            {
                ModelState["Max_Mast_District_Id"].Errors.Clear();
                if (ModelState.IsValid)
                {
                    //IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.SaveDistrictDetailsBAL(master_district, ref message))
                    {
                        // ModelState.AddModelError("success", "District details added successfully.");
                        message = message == string.Empty ? "District details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        //ModelState.AddModelError("error", message == string.Empty ? "District details not added successfully." : message);
                        message = message == string.Empty ? "District details not saved." : message;

                    }

                }
                else
                {
                    return PartialView("CreateDistrict", master_district);
                }

                // return PartialView("CreateDistrict", master_district);   
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                //ModelState.AddModelError("error", "District details not added successfully.");
                //return PartialView("CreateDistrict", master_district);
                //partialView = RenderPartialViewToString("CreateDistrict", master_district);
                message = "District details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        //
        // GET: /DistrictMasterDataEntry/Edit/5


        public ActionResult EditDistrict(String parameter, String hash, String key)
        {
            //Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    MASTER_DISTRICT master_district = masterDataEntryBAL.GetDistrictDetailsBAL_ByDistrictCode(Convert.ToInt32(decryptedParameters["DistrictCode"].ToString()));

                    if (master_district == null)
                    {
                        ModelState.AddModelError(string.Empty, "District details not exist.");
                        return PartialView("CreateDistrict", new MASTER_DISTRICT());
                    }
                    // return View(master_district);
                    return PartialView("CreateDistrict", master_district);
                }
                return PartialView("CreateDistrict", new MASTER_DISTRICT());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "District details not exist.");
                return PartialView("CreateDistrict", new MASTER_DISTRICT());
            }
        }

        //
        // POST: /DistrictMasterDataEntry/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDistrict(MASTER_DISTRICT master_district)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.UpdateDistrictDetailsBAL(master_district, ref message))
                    {
                        // ModelState.AddModelError("success", "District details updated successfully.");
                        message = message == string.Empty ? "District details updated successfully." : message;
                        status = true;

                    }
                    else
                    {
                        //  ModelState.AddModelError("error", message == string.Empty ? "District details not updated successfully." : message);
                        message = message == string.Empty ? "District details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("CreateDistrict", master_district);
                }



                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                //return PartialView("CreateDistrict", master_district);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                //ModelState.AddModelError("error", "District details not updated successfully.");             
                // return PartialView("CreateDistrict", master_district);
                message = message == string.Empty ? "District details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }



        public ActionResult DeleteDistrictDetails(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (masterDataEntryBAL.DeleteDistrictDetailsBAL_ByDistrictCode(Convert.ToInt32(decryptedParameters["DistrictCode"].ToString()), ref message))
                    {
                        message = "District details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "District details not deleted." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "District details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "District details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion DistrictDataEntry

        #region BlockDataEntry


        public ActionResult BlockDetails()
        {
            return View();
        }


        [HttpPost]
        public JsonResult GetDistrictsByStateCode(string stateCode)
        {
            try
            {
                if (!int.TryParse(stateCode, out outParam))
                {
                    return Json(false);
                }

                List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();

                districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(Convert.ToInt32(stateCode.Trim()), false);

                return Json(new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetDistrictsByStateCode


        [HttpPost]
        public JsonResult GetDistrictsByStateCode_Search(string stateCode)
        {
            try
            {
                if (!int.TryParse(stateCode, out outParam))
                {
                    return Json(false);
                }

                List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();

                districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(Convert.ToInt32(stateCode.Trim()), true);

                return Json(new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetDistrictsByStateCode


        [HttpPost]
        public ActionResult GetBlockDetailsList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = 0;
            int districtCode = 0;



            try
            {
                /*if (!string.IsNullOrEmpty(homeFormCollection["searchField"]))
                {
                    searchParameters = HttpUtility.HtmlDecode(homeFormCollection["searchField"]);

                    searchParameters = searchParameters.Replace("%2F", "/");
                    string[] str = (searchParameters.ToString().Split('&'));
                    for (int i = 0; i < str.Length; ++i)
                    {
                        string[] splitParameter = str[i].Split('=');
                        parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                    }

                    stateCode = Convert.ToInt16(parameters["ddlSearchStates"]);
                    districtCode = Convert.ToInt16(parameters["ddlSearchDistrict"]);
                }*/

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }
                else
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }
                else
                {
                    return null;
                }

                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetBlockDetailsListBAL(false, false, stateCode, districtCode, 0, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "LocationMasterDataEntry.GetBlockDetailsList()");
                return null;
            }

        }



        public ActionResult CreateBlock()
        {
            BlockMaster master_block = new BlockMaster();
            try
            {
                if (PMGSYSession.Current.RoleCode == 36)
                {
                    master_block.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                }
                master_block.IsPMGSYIncluded = true;
                master_block.IsDESERT = false;
                master_block.IsBADB = false;
                master_block.IsTRIBAL = false;
                master_block.IsSchedule5 = false;
                return PartialView("CreateBlock", master_block);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntry.CreateBlockGet()");
                return null;
            }
        }



        public ActionResult SearchBlock()
        {
            try
            {
                //BlockMaster master_block = new BlockMaster();

                //List<Models.MASTER_STATE> stateList = new List<Models.MASTER_STATE>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                //stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All" });
                if (PMGSYSession.Current.RoleCode == 36)
                {
                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME", PMGSYSession.Current.StateCode);
                }
                else
                {
                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME", 1);
                }


                //ViewData["StateList"] = master_block.States;
                //ViewData["DistrictList"] = master_block.Districts;

                List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                if (PMGSYSession.Current.RoleCode == 36)
                {
                    districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(PMGSYSession.Current.StateCode, true);
                }
                else
                {
                    districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(1, true);
                }

                ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME"); //master_block.Districts;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["StateList"] = null;
                ViewData["DistrictList"] = null;
            }

            return PartialView("SearchBlock");
        }

        public ActionResult IsDistrictIAP(string districtCode)
        {
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();

                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    outParam = Convert.ToInt32(Request.Params["districtCode"]);

                    return Json(new { isIAP = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == outParam).Select(s => s.MAST_IAP_DISTRICT).FirstOrDefault() });
                }

                return Json(false);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        //
        // POST: /BlockMasterDataEntry/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBlock(BlockMaster master_block)
        {
            bool status = false;
            try
            {
                ModelState["Max_Mast_Block_Id"].Errors.Clear();
                if (ModelState.IsValid)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.SaveBlockDetailsBAL(master_block, ref message))
                    {
                        //ModelState.AddModelError(string.Empty, "Block details added successfully.");
                        message = message == string.Empty ? "Block details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        //ModelState.AddModelError(string.Empty, message == string.Empty ? "Block details not added successfully." : message);
                        message = message == string.Empty ? "Block details not saved." : message;
                    }

                }
                else
                {
                    return PartialView("CreateBlock", master_block);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                /* ModelState.AddModelError(string.Empty, "Block details not added successfully.");
                      return View(master_block);*/
                ErrorLog.LogError(ex, "LocationMasterDataEntry.CreateBlockPost()");
                message = "Block details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /BlockMasterDataEntry/Edit/5


        public ActionResult EditBlock(String parameter, String hash, String key)
        {

            //Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    BlockMaster master_block = masterDataEntryBAL.GetBlockDetailsBAL_ByBlockCode(Convert.ToInt32(decryptedParameters["BlockCode"].ToString()));


                    if (master_block == null)
                    {
                        ModelState.AddModelError(string.Empty, "Block details not exist.");
                        return PartialView("CreateBlock", new BlockMaster());
                    }
                    // return View(master_block);
                    return PartialView("CreateBlock", master_block);
                }
                return PartialView("CreateBlock", new BlockMaster());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "LocationMasterDataEntry.EditBlockGet()");
                ModelState.AddModelError(string.Empty, "Block details not exist.");
                //return View();
                return PartialView("CreateBlock", new BlockMaster());
            }
        }

        //
        // POST: /BlockMasterDataEntry/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBlock(BlockMaster master_block)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.UpdateBlockDetailsBAL(master_block, ref message))
                    {
                        // ModelState.AddModelError(string.Empty, "Block details updated successfully.");
                        message = message == string.Empty ? "Block details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        // ModelState.AddModelError(string.Empty, message == string.Empty ? "Block details not updated successfully." : message);
                        message = message == string.Empty ? "Block details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("CreateBlock", master_block);
                }

                // return View(master_block);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                /* ModelState.AddModelError(string.Empty, "Block details not updated successfully.");
                       return View(master_block);*/
                ErrorLog.LogError(ex, "LocationMasterDataEntry.EditBlockPost()");
                message = message == string.Empty ? "Block details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }


        }



        public ActionResult DeleteBlockDetails(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (masterDataEntryBAL.DeleteBlockDetailsBAL_ByBlockCode(Convert.ToInt32(decryptedParameters["BlockCode"].ToString()), ref message))
                    {
                        message = "Block details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Block details not deleted." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Block details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "LocationMasterDataEntry.DeleteBlockDetailsPost()");
                message = "Block details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion BlockDataEntry

        #region VillageDataEntry



        public ActionResult VillageDetails()
        {

            try
            {
                ViewBag.MAST_LOCK_STATUS = "L";
                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                if (PMGSYSession.Current.StateCode > 0)
                {
                    //ViewBag.MAST_LOCK_STATUS = (dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, 0, 0, 0, 0, 0, 0, "VM", PMGSYSession.Current.PMGSYScheme, (short)PMGSYSession.Current.RoleCode).Select(m => m.UNLOCK_COUNT).FirstOrDefault() > 0 ? "U" : "L");
                    ViewBag.MAST_LOCK_STATUS = (dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, 0, 0, 0, 0, 0, 0, "VM", PMGSYSession.Current.PMGSYScheme, roleCode).Select(m => m.UNLOCK_COUNT).FirstOrDefault() > 0 ? "U" : "L");
                }

                return View();
            }
            catch
            {
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

            return View();
        }


        [HttpPost]
        public JsonResult GetBlocksByDistrictCode(string districtCode)
        {
            try
            {
                if (!int.TryParse(districtCode, out outParam))
                {
                    return Json(false);
                }

                List<Models.MASTER_BLOCK> blockList = new List<Models.MASTER_BLOCK>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(Convert.ToInt32(districtCode.Trim()), false);

                return Json(new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetBlocksByDistrictCode

        [HttpPost]
        public JsonResult GetBlocksByDistrictCode_Search(string districtCode)
        {
            try
            {
                if (!int.TryParse(districtCode, out outParam))
                {
                    return Json(false);
                }
                List<Models.MASTER_BLOCK> blockList = new List<Models.MASTER_BLOCK>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(Convert.ToInt32(districtCode.Trim()), true);

                return Json(new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetBlocksByDistrictCode

        [HttpPost]
        public ActionResult GetIsShedule5()
        {
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    outParam = Convert.ToInt32(Request.Params["blockCode"]);

                    return Json(new { schedule5 = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == outParam).Select(s => s.MAST_SCHEDULE5).FirstOrDefault() });
                }
                if (!string.IsNullOrEmpty(Request.Params["villageCode"]))
                {
                    outParam = Convert.ToInt32(Request.Params["villageCode"]);

                    return Json(new { schedule5 = dbContext.MASTER_VILLAGE.Where(m => m.MAST_VILLAGE_CODE == outParam).Select(s => s.MAST_SCHEDULE5).FirstOrDefault() });
                }



                return Json(false);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetBlocksByDistrictCode


        [HttpPost]
        public ActionResult GetVillageDetailsList(int? page, int? rows, string sidx, string sord)
        {


            long totalRecords;
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;


            try
            {
                /*if (!string.IsNullOrEmpty(homeFormCollection["searchField"]))
                {
                    searchParameters = HttpUtility.HtmlDecode(homeFormCollection["searchField"]);

                    searchParameters = searchParameters.Replace("%2F", "/");
                    string[] str = (searchParameters.ToString().Split('&'));
                    for (int i = 0; i < str.Length; ++i)
                    {
                        string[] splitParameter = str[i].Split('=');
                        parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                    }

                    stateCode = Convert.ToInt16(parameters["ddlSearchStates"]);
                    districtCode = Convert.ToInt16(parameters["ddlSearchDistrict"]);
                    blockCode = Convert.ToInt16(parameters["ddlSearchBlocks"]);
                }*/
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }
                else
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }
                else
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);

                }
                else
                {
                    return null;
                }

                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetVillageDetailsListBAL(stateCode, districtCode, blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }


        public ActionResult CreateVillage()
        {
            return PartialView("CreateVillage", new VillageMaster());
        }


        public ActionResult SearchVillage()
        {
            try
            {

                // PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                //Modified By Abhishek kamble 4-Apr-2014
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 36) //PIU or ITNO
                {
                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME", PMGSYSession.Current.StateCode);
                }
                else
                {
                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME", 1);
                }
                List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                if (PMGSYSession.Current.RoleCode == 36)
                {
                    districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(PMGSYSession.Current.StateCode, true);
                }
                else
                {
                    districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(1, true);
                }
                //districtList.RemoveAt(0);                

                ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");

                ViewData["BlockList"] = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(0, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["StateList"] = null;
                ViewData["DistrictList"] = null;
                ViewData["BlockList"] = null;
            }

            return PartialView("SearchVillage");
        }


        //
        // POST: /VillageMasterDataEntry/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVillage(VillageMaster master_village)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.SaveVillageDetailsBAL(master_village, ref message))
                    {
                        //ModelState.AddModelError(string.Empty, "Village details added successfully.");

                        message = message == string.Empty ? "Village details saved successfully." : message;
                        status = true;

                    }
                    else
                    {
                        // ModelState.AddModelError(string.Empty, message == string.Empty ? "Village details not added successfully." : message);
                        message = message == string.Empty ? "Village details not saved." : message;
                    }

                }
                else
                {
                    return PartialView("CreateVillage", master_village);
                }

                //return View(master_village);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                /*ModelState.AddModelError(string.Empty, "Village details not added successfully.");
                     return View(master_village);*/
                message = "Village details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /VillageMasterDataEntry/Edit/5

        public ActionResult EditVillage(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    VillageMaster master_village = masterDataEntryBAL.GetVillageDetailsBAL_ByVillageCode(Convert.ToInt32(decryptedParameters["VillageCode"].ToString()));

                    if (master_village == null)
                    {
                        ModelState.AddModelError(string.Empty, "Village details not exist.");
                        return PartialView("CreateVillage", new VillageMaster());
                    }
                    //return View(master_village);
                    return PartialView("CreateVillage", master_village);
                }
                //return View();
                return PartialView("CreateVillage", new VillageMaster());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Village details not exist.");
                // return View();
                return PartialView("CreateVillage", new VillageMaster());
            }
        }

        //
        // POST: /VillageMasterDataEntry/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVillage(VillageMaster master_village)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    //IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.UpdateVillageDetailsBAL(master_village, ref message))
                    {
                        // ModelState.AddModelError(string.Empty, "Village details updated successfully.");
                        message = message == string.Empty ? "Village details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        // ModelState.AddModelError(string.Empty, message == string.Empty ? "Village details not updated successfully." : message);
                        message = message == string.Empty ? "Village details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("CreateVillage", master_village);
                }

                // return View(master_village);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                /*ModelState.AddModelError(string.Empty, "Village details not updated successfully.");
                     return View(master_village);*/
                message = message == string.Empty ? "Village details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult DeleteVillageDetails(String parameter, String hash, String key)
        {
            //Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (masterDataEntryBAL.DeleteVillageDetailsBAL_ByVillageCode(Convert.ToInt32(decryptedParameters["VillageCode"].ToString()), ref message))
                    {
                        message = "Village details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Village details not deleted." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Village details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Village details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult FinalizeVillage(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    bool status = masterDataEntryBAL.FinalizeVillageBAL(Convert.ToInt32(decryptedParameters["VillageCode"]));
                    if (status == true)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                return Json(new { success = false });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }


        #endregion VillageDataEntry

        #region PanchayatDataEntry



        public ActionResult PanchayatDetails()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetPanchayatDetailsList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;


            try
            {
                /*if (!string.IsNullOrEmpty(homeFormCollection["searchField"]))
                {
                    searchParameters = HttpUtility.HtmlDecode(homeFormCollection["searchField"]);

                    searchParameters = searchParameters.Replace("%2F", "/");
                    string[] str = (searchParameters.ToString().Split('&'));
                    for (int i = 0; i < str.Length; ++i)
                    {
                        string[] splitParameter = str[i].Split('=');
                        parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                    }

                    stateCode = Convert.ToInt16(parameters["ddlSearchStates"]);
                    districtCode = Convert.ToInt16(parameters["ddlSearchDistrict"]);
                    blockCode = Convert.ToInt16(parameters["ddlSearchBlocks"]);
                }*/

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }
                else
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }
                else
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }
                else
                {
                    return null;
                }

                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetPanchayatDetailsListBAL(stateCode, districtCode, blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }


        public ActionResult CreatePanchayat()
        {
            return PartialView("CreatePanchayat", new PanchayatMaster());
        }



        public ActionResult SearchPanchayat()
        {
            try
            {

                //PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();
                ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME", 1);

                List<Models.MASTER_DISTRICT> districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(1, true);
                //districtList.RemoveAt(0);
                ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");

                // List<Models.MASTER_BLOCK> blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(0);

                ViewData["BlockList"] = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(0, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["StateList"] = null;
                ViewData["DistrictList"] = null;
                ViewData["BlockList"] = null;
            }

            return PartialView("SearchPanchayat");
        }

        //
        // POST: /PanchayatMasterDataEntry/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePanchayat(PanchayatMaster master_panchayat)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.SavePanchayatDetailsBAL(master_panchayat, ref message))
                    {
                        //ModelState.AddModelError(string.Empty, "Panchayat details added successfully.");
                        message = message == string.Empty ? "Panchayat details saved successfully." : message;
                        status = true;

                    }
                    else
                    {
                        //ModelState.AddModelError(string.Empty, message == string.Empty ? "Panchayat details not added successfully." : message);
                        message = message == string.Empty ? "Panchayat details not saved." : message;
                    }

                }
                else
                {
                    return PartialView("CreatePanchayat", master_panchayat);
                }

                // return View(master_panchayat);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                /* ModelState.AddModelError(string.Empty, "Panchayat details not added successfully.");
                      return View(master_panchayat);*/
                message = "Panchayat details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /PanchayatMasterDataEntry/Edit/5


        public ActionResult EditPanchayat(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    //  IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    PanchayatMaster master_panchayat = masterDataEntryBAL.GetPanchayatDetailsBAL_ByPanchayatCode(Convert.ToInt32(decryptedParameters["PanchayatCode"].ToString()));

                    if (master_panchayat == null)
                    {
                        ModelState.AddModelError(string.Empty, "Panchayat details not exist.");
                        return PartialView("CreatePanchayat", new PanchayatMaster());
                    }
                    //return View(master_panchayat);
                    return PartialView("CreatePanchayat", master_panchayat);
                }
                // return View();
                return PartialView("CreatePanchayat", new PanchayatMaster());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Panchayat details not exist.");
                // return View();
                return PartialView("CreatePanchayat", new PanchayatMaster());
            }
        }

        //
        // POST: /PanchayatMasterDataEntry/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPanchayat(PanchayatMaster master_panchayat)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    //IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.UpdatePanchayatDetailsBAL(master_panchayat, ref message))
                    {
                        // ModelState.AddModelError(string.Empty, "Panchayat details updated successfully.");
                        message = message == string.Empty ? "Panchayat details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        // ModelState.AddModelError(string.Empty, message == string.Empty ? "Panchayat details not updated successfully." : message);
                        message = message == string.Empty ? "Panchayat details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("CreatePanchayat", master_panchayat);
                }

                //return View(master_panchayat);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                /* ModelState.AddModelError(string.Empty, "Panchayat details not updated successfully.");
                      return View(master_panchayat);*/
                message = message == string.Empty ? "Panchayat details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult DeletePanchayatDetails(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (masterDataEntryBAL.DeletePanchayatDetailsBAL_ByPanchayatCode(Convert.ToInt32(decryptedParameters["PanchayatCode"].ToString()), ref message))
                    {
                        message = "Panchayat details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Panchayat details not deleted." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Panchayat details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Panchayat details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult FinalizePanchayat(String parameter, String hash, String key)
        {

            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (masterDataEntryBAL.FinalizePanchayatBAL(Convert.ToInt32(decryptedParameters["PanchayatCode"].ToString()), ref message))
                    {
                        message = "Panchayat details finalized.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Panchayat details not finalized." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Panchayat details not finalized.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Panchayat details not finalized.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion PanchayatDataEntry

        #region HabitationDataEntry


        public ActionResult HabitationDetails()
        {
            try
            {
                ViewBag.MAST_LOCK_STATUS = "L";
                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                if (PMGSYSession.Current.StateCode > 0)
                {
                    //ViewBag.MAST_LOCK_STATUS = (dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, 0, 0, 0, 0, 0, 0, "HM", PMGSYSession.Current.PMGSYScheme, (short)PMGSYSession.Current.RoleCode).Select(m => m.UNLOCK_COUNT).FirstOrDefault() > 0 ? "U" : "L");
                    ViewBag.MAST_LOCK_STATUS = (dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, 0, 0, 0, 0, 0, 0, "HM", PMGSYSession.Current.PMGSYScheme, roleCode).Select(m => m.UNLOCK_COUNT).FirstOrDefault() > 0 ? "U" : "L");
                }

                return View();
            }
            catch
            {
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

            return View();
        }


        [HttpPost]
        public JsonResult GetVillagesByBlockCode(string blockCode)
        {
            try
            {
                if (!int.TryParse(blockCode, out outParam))
                {
                    return Json(false);
                }
                List<Models.MASTER_VILLAGE> villageList = new List<Models.MASTER_VILLAGE>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                villageList = masterDataEntryDAL.GetAllVillagesByBlockCode(Convert.ToInt32(blockCode.Trim()), false);

                return Json(new SelectList(villageList, "MAST_VILLAGE_CODE", "MAST_VILLAGE_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetVillagesByBlockCode

        [HttpPost]
        public JsonResult GetVillagesByBlockCode_Search(string blockCode)
        {
            try
            {
                if (!int.TryParse(blockCode, out outParam))
                {
                    return Json(false);
                }
                List<Models.MASTER_VILLAGE> villageList = new List<Models.MASTER_VILLAGE>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                villageList = masterDataEntryDAL.GetAllVillagesByBlockCode(Convert.ToInt32(blockCode.Trim()), true);

                return Json(new SelectList(villageList, "MAST_VILLAGE_CODE", "MAST_VILLAGE_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetVillagesByBlockCode

        [HttpPost]
        public JsonResult GetMPContituencyByBlockCode(string blockCode)
        {
            try
            {
                if (!int.TryParse(blockCode, out outParam))
                {
                    return Json(false);
                }

                List<Models.MASTER_MP_CONSTITUENCY> mpContituencyList = new List<Models.MASTER_MP_CONSTITUENCY>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                mpContituencyList = masterDataEntryDAL.GetAllMPContituencyByBlockCode(Convert.ToInt32(blockCode.Trim()));

                return Json(new SelectList(mpContituencyList, "MAST_MP_CONST_CODE", "MAST_MP_CONST_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetMPContituencyByBlockCode

        [HttpPost]
        public JsonResult GetMLAContituencyByBlockCode(string blockCode)
        {
            try
            {
                //if (string.IsNullOrEmpty(blockCode))
                //{
                //    return Json(false);
                //}
                if (!int.TryParse(blockCode, out outParam))
                {
                    return Json(false);
                }

                List<Models.MASTER_MLA_CONSTITUENCY> mlaContituencyList = new List<Models.MASTER_MLA_CONSTITUENCY>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                mlaContituencyList = masterDataEntryDAL.GetAllMLAContituencyByBlockCode(Convert.ToInt32(blockCode.Trim()));

                return Json(new SelectList(mlaContituencyList, "MAST_MLA_CONST_CODE", "MAST_MLA_CONST_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetMLAContituencyByBlockCode

        [HttpPost]
        public ActionResult GetHabitationDetailsList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            string villageName = string.Empty;
            string habitationName = string.Empty;


            try
            {
                /*if (!string.IsNullOrEmpty(homeFormCollection["searchField"]))
                {
                    searchParameters = HttpUtility.HtmlDecode(homeFormCollection["searchField"]);

                    searchParameters = searchParameters.Replace("%2F", "/");
                    string[] str = (searchParameters.ToString().Split('&'));
                    for (int i = 0; i < str.Length; ++i)
                    {
                        string[] splitParameter = str[i].Split('=');
                        parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                    }

                    stateCode = Convert.ToInt16(parameters["ddlSearchStates"]);
                    districtCode = Convert.ToInt16(parameters["ddlSearchDistrict"]);
                    blockCode = Convert.ToInt16(parameters["ddlSearchBlocks"]);
                    villageName = parameters["txtSearchVillage"].ToString().Replace('+', ' ');
                }*/

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }
                else
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }
                else
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }
                else
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["villageName"]))
                {
                    villageName = Request.Params["villageName"].Replace('+', ' ');
                }
                else
                {
                    villageName = string.Empty;
                }


                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetHabitationDetailsListBAL(false, stateCode, districtCode, blockCode, villageName, habitationName, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }


        }


        public ActionResult CreateHabitation()
        {
            return PartialView("CreateHabitation", new HabitationMaster());
        }



        public ActionResult SearchHabitation()
        {
            try
            {

                // PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                //Modified By Abhishek kamble 4-Apr-2014
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 36)//PIU or ITNO
                {
                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME", PMGSYSession.Current.StateCode);
                }
                else
                {
                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME", 1);
                }



                List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                if (PMGSYSession.Current.RoleCode == 36)
                {
                    districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(PMGSYSession.Current.StateCode, true);
                }
                else
                {
                    districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(1, true);
                }
                //districtList.RemoveAt(0);
                ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");

                ViewData["BlockList"] = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(0, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["StateList"] = null;
                ViewData["DistrictList"] = null;
                ViewData["BlockList"] = null;
            }

            return PartialView("SearchHabitation");
        }

        //
        // POST: /HabitationMasterDataEntry/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHabitation(HabitationMaster master_habitations)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.SaveHabitationDetailsBAL(master_habitations, ref message))
                    {
                        // ModelState.AddModelError(string.Empty, "Habitation details added successfully.");
                        message = message == string.Empty ? "Habitation details saved successfully." : message;
                        status = true;

                    }
                    else
                    {
                        // ModelState.AddModelError(string.Empty, message == string.Empty ? "Habitation details not added successfully." : message);
                        message = message == string.Empty ? "Habitation details not saved." : message;
                    }

                }
                else
                {
                    return PartialView("CreateHabitation", master_habitations);
                }

                // return View(master_habitations);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                /* ModelState.AddModelError(string.Empty, "Habitation details not added successfully.");
                      return View(master_habitations);*/
                message = "Habitation details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /HabitationMasterDataEntry/Edit/5
        public ActionResult EditHabitation(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    HabitationMaster master_habitations = masterDataEntryBAL.GetHabitationDetailsBAL_ByHabitationCode(Convert.ToInt32(decryptedParameters["HabitationCode"].ToString()));

                    if (master_habitations == null)
                    {
                        ModelState.AddModelError(string.Empty, "Habitation details not exist.");
                        return PartialView("CreateHabitation", new HabitationMaster());

                    }
                    //return View(master_habitations);
                    return PartialView("CreateHabitation", master_habitations);

                }
                //return View();
                return PartialView("CreateHabitation", new HabitationMaster());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Habitation details not exist.");
                //return View();
                return PartialView("CreateHabitation", new HabitationMaster());
            }
        }

        //
        // POST: /HabitationMasterDataEntry/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditHabitation(HabitationMaster master_habitations)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.UpdateHabitationDetailsBAL(master_habitations, ref message))
                    {
                        //ModelState.AddModelError(string.Empty, "Habitation details updated successfully.");
                        message = message == string.Empty ? "Habitation details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        //ModelState.AddModelError(string.Empty, message == string.Empty ? "Habitation details not updated successfully." : message);
                        message = message == string.Empty ? "Habitation details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("CreateHabitation", master_habitations);
                }

                // return View(master_habitations);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                /*  ModelState.AddModelError(string.Empty, "Habitation details not updated successfully.");
                      return View(master_habitations);*/
                message = message == string.Empty ? "Habitation details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        [Audit]
        public ActionResult DeleteHabitationDetails(String parameter, String hash, String key)
        {
            //  Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (masterDataEntryBAL.DeleteHabitationDetailsBAL_ByHabitationCode(Convert.ToInt32(decryptedParameters["HabitationCode"].ToString()), ref message))
                    {
                        message = "Habitation details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Habitation details not deleted." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Habitation details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Habitation details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion HabitationDataEntry

        #region MLAConstituencyDataEntry

        public ActionResult MLAConstituencyDetails()
        {
            return PartialView();
        }


        [HttpPost]
        public ActionResult GetMLAConstituencyDetailsList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = 0;

            try
            {
                /*if (!string.IsNullOrEmpty(homeFormCollection["searchField"]))
                {
                    searchParameters = HttpUtility.HtmlDecode(homeFormCollection["searchField"]);

                    searchParameters = searchParameters.Replace("%2F", "/");
                    string[] str = (searchParameters.ToString().Split('&'));
                    for (int i = 0; i < str.Length; ++i)
                    {
                        string[] splitParameter = str[i].Split('=');
                        parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                    }

                    stateCode = Convert.ToInt16(parameters["ddlSearchStates"]);

                }*/
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }
                else
                {
                    return null;
                }



                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetMLAConstituencyDetailsListBAL(stateCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }

        }





        public ActionResult CreateMLAConstituency()
        {
            return PartialView("CreateMLAConstituency", new MLAConstituency());
        }



        public ActionResult SearchMLAConstituency()
        {
            try
            {
                if (PMGSYSession.Current.StateCode > 0)
                {
                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Add(new SelectListItem { Value = PMGSYSession.Current.StateCode.ToString(), Text = PMGSYSession.Current.StateName.Trim() });

                    ViewData["StateList"] = lstState;
                }
                else
                {
                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME"); //GetAllStates(true);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["StateList"] = null;
            }

            return PartialView("SearchMLAConstituency");
        }


        //
        // POST: /DistrictMasterDataEntry/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMLAConstituency(MLAConstituency master_mlaconstituency)
        {
            bool status = false;
            try
            {

                if (ModelState.IsValid)
                {
                    IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.SaveMLAConstituencyDetailsBAL(master_mlaconstituency, ref message))
                    {
                        message = message == string.Empty ? "MLA Constituency details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "MLA Constituency details not saved." : message;

                    }

                }
                else
                {
                    return PartialView("CreateMLAConstituency", master_mlaconstituency);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "MLA Constituency details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        //
        // GET: /DistrictMasterDataEntry/Edit/5


        public ActionResult EditMLAConstituency(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    MLAConstituency master_mlaconstituency = masterDataEntryBAL.GetMLAConstituencyDetailsBAL_ByMLAConstituencyCode(Convert.ToInt32(decryptedParameters["MLAConstituencyCode"].ToString()));

                    if (master_mlaconstituency == null)
                    {
                        ModelState.AddModelError(string.Empty, "MLA Constituency details not exist.");
                        return PartialView("CreateMLAConstituency", new MLAConstituency());
                    }
                    // return View(master_district);
                    return PartialView("CreateMLAConstituency", master_mlaconstituency);
                }
                return PartialView("CreateMLAConstituency", new MLAConstituency());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "MLA Constituency details not exist.");
                return PartialView("CreateMLAConstituency", new MLAConstituency());
            }
        }

        //
        // POST: /DistrictMasterDataEntry/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMLAConstituency(MLAConstituency master_mlaconstituency)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.UpdateMLAConstituencyDetailsBAL(master_mlaconstituency, ref message))
                    {
                        message = message == string.Empty ? "MLA Constituency details updated successfully." : message;
                        status = true;

                    }
                    else
                    {

                        message = message == string.Empty ? "MLA Constituency details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("CreateMLAConstituency", master_mlaconstituency);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "MLA Constituency details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult DeleteMLAConstituencyDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (masterDataEntryBAL.DeleteMLAConstituencyDetailsBAL_ByMLAConstituencyCode(Convert.ToInt32(decryptedParameters["MLAConstituencyCode"].ToString()), ref message))
                    {
                        message = "MLA Constituency details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "MLA Constituency details not deleted successfully." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "MLA Constituency details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "MLA Constituency details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion MLAConstituencyDataEntry


        #region MPConstituencyDataEntry


        public ActionResult MPConstituencyDetails()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult GetMPConstituencyDetailsList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = 0;



            try
            {
                /* if (!string.IsNullOrEmpty(homeFormCollection["searchField"]))
                 {
                     searchParameters = HttpUtility.HtmlDecode(homeFormCollection["searchField"]);

                     searchParameters = searchParameters.Replace("%2F", "/");
                     string[] str = (searchParameters.ToString().Split('&'));
                     for (int i = 0; i < str.Length; ++i)
                     {
                         string[] splitParameter = str[i].Split('=');
                         parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                     }

                     stateCode = Convert.ToInt16(parameters["ddlSearchStates"]);

                 }*/
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }
                else
                {
                    return null;
                }
                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetMPConstituencyDetailsListBAL(stateCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords), //RegistredDocumentsDetails.GetSearchResultList(partyName, districtCode, villageCode, fromDate, toDate, Convert.ToInt32(homeFormCollection["page"]) - 1, Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }


        public ActionResult CreateMPConstituency()
        {
            return PartialView("CreateMPConstituency", new MPConstituency());
        }

        public ActionResult SearchMPConstituency()
        {
            try
            {
                if (PMGSYSession.Current.StateCode > 0)
                {
                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Add(new SelectListItem { Value = PMGSYSession.Current.StateCode.ToString(), Text = PMGSYSession.Current.StateName.Trim() });

                    ViewData["StateList"] = lstState;
                }
                else
                {
                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME");//GetAllStates(true);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["StateList"] = null;
            }

            return PartialView("SearchMPConstituency");
        }


        //
        // POST: /DistrictMasterDataEntry/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMPConstituency(MPConstituency master_mpconstituency)
        {
            bool status = false;
            try
            {

                if (ModelState.IsValid)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.SaveMPConstituencyDetailsBAL(master_mpconstituency, ref message))
                    {
                        message = message == string.Empty ? "MP Constituency details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "MP Constituency details not saved." : message;

                    }

                }
                else
                {
                    return PartialView("CreateMPConstituency", master_mpconstituency);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "MP Constituency details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        //
        // GET: /DistrictMasterDataEntry/Edit/5

        public ActionResult EditMPConstituency(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    MPConstituency master_mpconstituency = masterDataEntryBAL.GetMPConstituencyDetailsBAL_ByMPConstituencyCode(Convert.ToInt32(decryptedParameters["MPConstituencyCode"].ToString()));

                    if (master_mpconstituency == null)
                    {
                        ModelState.AddModelError(string.Empty, "MP Constituency details not exist.");
                        return PartialView("CreateMPConstituency", new MPConstituency());
                    }
                    // return View(master_district);
                    return PartialView("CreateMPConstituency", master_mpconstituency);
                }
                return PartialView("CreateMPConstituency", new MPConstituency());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "MP Constituency details not exist.");
                return PartialView("CreateMPConstituency", new MPConstituency());
            }
        }

        //
        // POST: /DistrictMasterDataEntry/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMPConstituency(MPConstituency master_mpconstituency)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    //  IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.UpdateMPConstituencyDetailsBAL(master_mpconstituency, ref message))
                    {
                        message = message == string.Empty ? "MP Constituency details updated successfully." : message;
                        status = true;

                    }
                    else
                    {

                        message = message == string.Empty ? "MP Constituency details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("CreateMPConstituency", master_mpconstituency);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "MP Constituency details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult DeleteMPConstituencyDetails(String parameter, String hash, String key)
        {
            //  Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (masterDataEntryBAL.DeleteMPConstituencyDetailsBAL_ByMPConstituencyCode(Convert.ToInt32(decryptedParameters["MPConstituencyCode"].ToString()), ref message))
                    {
                        message = "MP Constituency details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "MP Constituency details not deleted." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "MP Constituency details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "MP Constituency details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion MPConstituencyDataEntry

        #region OtherHabitationDetailsDataEntry


        public ActionResult OtherHabitationDetails(String id)
        {
            // Dictionary<string, string> decryptedParameters = null;
            // String[] encryptedParameters = null;
            HabitationDetails habitationDetails = new HabitationDetails();

            try
            {
                if (id != string.Empty)
                {
                    encryptedParameters = id.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        ViewBag.HabitationName = decryptedParameters["HabitationName"].ToString();

                        //Added By Abhishek kamble 24-Feb-2014 for Village Population     start
                        int totalRemainingPopulation = 0;
                        int totalRemainingSCSTPopulation = 0;

                        int totalVillagePopulation = 0;
                        Int64 totalVillagePopulation20Per = 0;
                        int totalVillageSCSTPopulation = 0;


                        if (masterDataEntryDAL.CheckRemainingPopulation(true, dbContext, Convert.ToInt32(decryptedParameters["HabitationCode"]), ref totalRemainingPopulation, ref totalRemainingSCSTPopulation, ref totalVillagePopulation, ref totalVillagePopulation20Per, ref totalVillageSCSTPopulation, ref message))
                        {
                            habitationDetails.totalRemainingPopulation = totalRemainingPopulation;
                            habitationDetails.totalRemainingSCSTPopulation = totalRemainingSCSTPopulation;

                            habitationDetails.totalVillagePopulation = totalVillagePopulation;
                            habitationDetails.totalVillageSCSTPopulation = totalVillageSCSTPopulation;
                            habitationDetails.totalVillagePopulation20Per = totalVillagePopulation20Per;

                            habitationDetails.IsVillagePopulationDetailsExist = true;
                        }
                        else
                        {
                            habitationDetails.IsVillagePopulationDetailsExist = false;
                            habitationDetails.ErrMessageForIsVillagePopulationExist = message;
                        }
                        //Added By Abhishek kamble 24-Feb-2014 for Village Population end

                    }

                    habitationDetails.EncryptedHabitationCode_OtherDetails = id;
                    return PartialView("OtherHabitationDetails", habitationDetails);
                }
                return PartialView("OtherHabitationDetails", habitationDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("OtherHabitationDetails", habitationDetails);
            }
        }


        public ActionResult AddOtherHabitationDetails()
        {

            HabitationDetails habitationDetails = new HabitationDetails();
            if (PMGSYSession.Current.PMGSYScheme == 1)
            {
                return PartialView("AddOtherHabitationDetails", habitationDetails);
            }
            else
            {
                return PartialView("AddOtherHabitationDetailsPMGSYII", habitationDetails);
            }

        }


        [HttpPost]
        public ActionResult GetOtherHabitationDetailsList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            int habitationCode = 0;
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;


            string isLocked = string.Empty;
            string lockStatus = string.Empty;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                encryptedParameters = Request.Params["HabCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    habitationCode = Convert.ToInt32(decryptedParameters["HabitationCode"].ToString());
                    lockStatus = decryptedParameters["LockStatus"].ToString();
                }

                //isLocked = dbContext.MASTER_HABITATIONS.Where(mh => mh.MAST_HAB_CODE == habitationCode).Select(mh => mh.MAST_LOCK_STATUS).FirstOrDefault();

                //if (isLocked.Equals("Y"))
                //{
                //    lockStatus = isLocked;
                //}

                PMGSY.Models.MASTER_HABITATIONS masterHabitation = dbContext.MASTER_HABITATIONS.Where(mh => mh.MAST_HAB_CODE == habitationCode).FirstOrDefault();

                blockCode = dbContext.MASTER_VILLAGE.Where(mv => mv.MAST_VILLAGE_CODE == masterHabitation.MAST_VILLAGE_CODE).Select(mv => mv.MAST_BLOCK_CODE).FirstOrDefault();

                districtCode = dbContext.MASTER_BLOCK.Where(mb => mb.MAST_BLOCK_CODE == blockCode).Select(mb => mb.MAST_DISTRICT_CODE).FirstOrDefault();

                stateCode = dbContext.MASTER_DISTRICT.Where(md => md.MAST_DISTRICT_CODE == districtCode).Select(md => md.MAST_STATE_CODE).FirstOrDefault();

                byte scheme = (dbContext.MASTER_HABITATIONS_DETAILS.Where(m => m.MAST_HAB_CODE == habitationCode).Select(m => m.MAST_YEAR).FirstOrDefault() == 2001 ? Convert.ToByte(1) : Convert.ToByte(2));
                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                //Int32? count = (from status in dbContext.UDF_IMS_UNLOCK_STATUS(stateCode, districtCode, blockCode, masterHabitation.MAST_VILLAGE_CODE, habitationCode, 0, "HM", scheme) select (Int32?)status.UNLOCK_COUNT).FirstOrDefault();
                //Int32? count = (from status in dbContext.UDF_IMS_UNLOCK_STATUS(stateCode, districtCode, blockCode, masterHabitation.MAST_VILLAGE_CODE, habitationCode, 0, 0, 0, "HM", PMGSYSession.Current.PMGSYScheme, (short)PMGSYSession.Current.RoleCode) select (Int32?)status.UNLOCK_COUNT).FirstOrDefault();
                Int32? count = (from status in dbContext.UDF_IMS_UNLOCK_STATUS(stateCode, districtCode, blockCode, masterHabitation.MAST_VILLAGE_CODE, habitationCode, 0, 0, 0, "HM", PMGSYSession.Current.PMGSYScheme, roleCode) select (Int32?)status.UNLOCK_COUNT).FirstOrDefault();


                if (count != null && count > 0)
                {
                    isLocked = "M";
                }
                else
                {
                    isLocked = masterHabitation.MAST_LOCK_STATUS;
                }




                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetOtherHabitationDetailsListBAL(habitationCode, isLocked, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    userdata = new { lockStatus = isLocked }
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetOtherHabitationDetailsList()");
                return null;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOtherHabitationDetails(HabitationDetails details_habitations)
        {
            bool status = false;
            try
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);

                if (ModelState.IsValid)
                {

                    if (masterDataEntryBAL.SaveOtherHabitationDetailsBAL(details_habitations, ref message))
                    {

                        message = message == string.Empty ? "Habitation details saved successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "Habitation details not saved." : message;
                    }

                }
                else
                {
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        return PartialView("AddOtherHabitationDetails", details_habitations);
                    }
                    else
                    {
                        return PartialView("AddOtherHabitationDetailsPMGSYII", details_habitations);
                    }
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddOtherHabitationDetails(HabitationDetails details_habitations)");
                message = "Habitation details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult EditOtherHabitationDetails(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    HabitationDetails details_habitations = masterDataEntryBAL.GetOtherHabitationDetailsBAL_ByHabitationCodeandYear(Convert.ToInt32(decryptedParameters["HabitationCode"].ToString()), Convert.ToInt16(decryptedParameters["Year"].ToString()));

                    if (details_habitations == null)
                    {
                        ModelState.AddModelError(string.Empty, "Habitation details not exist.");
                        if (PMGSYSession.Current.PMGSYScheme == 1)
                        {
                            return PartialView("AddOtherHabitationDetails", new HabitationDetails());
                        }
                        else
                        {
                            return PartialView("AddOtherHabitationDetailsPMGSYII", new HabitationDetails());
                        }

                    }
                    ViewBag.HabitationName = decryptedParameters["HabitationName"].ToString();

                    //Added By Abhishek kamble 24-Feb-2014 for Village Population start
                    int totalRemainingPopulation = 0;
                    int totalRemainingSCSTPopulation = 0;

                    int totalVillagePopulation = 0;
                    Int64 totalVillagePopulation20Per = 0;
                    int totalVillageSCSTPopulation = 0;

                    if (masterDataEntryDAL.CheckRemainingPopulation(false, dbContext, Convert.ToInt32(decryptedParameters["HabitationCode"]), ref totalRemainingPopulation, ref totalRemainingSCSTPopulation, ref totalVillagePopulation, ref totalVillagePopulation20Per, ref totalVillageSCSTPopulation, ref message))
                    {
                        details_habitations.totalRemainingPopulation = totalRemainingPopulation;
                        details_habitations.totalRemainingSCSTPopulation = totalRemainingSCSTPopulation;

                        details_habitations.totalVillagePopulation = totalVillagePopulation;
                        details_habitations.totalVillageSCSTPopulation = totalVillageSCSTPopulation;
                        details_habitations.totalVillagePopulation20Per = totalVillagePopulation20Per;
                        //details_habitations.IsVillagePopulationDetailsExist = true;
                    }
                    //Added By Abhishek kamble 24-Feb-2014 for Village Population end


                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        return PartialView("AddOtherHabitationDetails", details_habitations);
                    }
                    else
                    {
                        return PartialView("AddOtherHabitationDetailsPMGSYII", details_habitations);
                    }



                }
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    return PartialView("AddOtherHabitationDetails", new HabitationDetails());
                }
                else
                {
                    return PartialView("AddOtherHabitationDetailsPMGSYII", new HabitationDetails());
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditOtherHabitationDetails(String parameter, String hash, String key)");
                ModelState.AddModelError(string.Empty, "Habitation details not exist.");
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    return PartialView("AddOtherHabitationDetails", new HabitationDetails());
                }
                else
                {
                    return PartialView("AddOtherHabitationDetailsPMGSYII", new HabitationDetails());
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOtherHabitationDetails(HabitationDetails details_habitations)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if (masterDataEntryBAL.UpdateOtherHabitationDetailsBAL(details_habitations, ref message))
                    {
                        message = message == string.Empty ? "Habitation details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Habitation details not updated." : message;
                    }
                }
                else
                {
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        return PartialView("AddOtherHabitationDetails", details_habitations);
                    }
                    else
                    {
                        return PartialView("AddOtherHabitationDetailsPMGSYII", details_habitations);
                    }
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditOtherHabitationDetails(HabitationDetails details_habitations)");
                message = message == string.Empty ? "Habitation details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }



        public ActionResult DeleteHabitationOtherDetails(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (masterDataEntryBAL.DeleteHabitationOtherDetailsBAL_ByHabCodeandYear(Convert.ToInt32(decryptedParameters["HabitationCode"].ToString()), Convert.ToInt16(decryptedParameters["Year"].ToString()), ref message))
                    {
                        message = "Habitation Other details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Habitation Other details not deleted." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Habitation Other details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Habitation Other details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult FinalizeHabitation(String parameter, String hash, String key)
        {

            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (masterDataEntryBAL.FinalizeHabitationBAL(Convert.ToInt32(decryptedParameters["HabitationCode"].ToString()), ref message))
                    {
                        message = "Habitation details finalized.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Habitation details not finalized." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Habitation details not finalized.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Habitation details not finalized.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ViewOtherHabitationDetails(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    HabitationDetails details_habitations = masterDataEntryBAL.GetOtherHabitationDetailsBAL_ByHabitationCodeandYear(Convert.ToInt32(decryptedParameters["HabitationCode"].ToString()), Convert.ToInt16(decryptedParameters["Year"].ToString()));

                    if (details_habitations == null)
                    {
                        ModelState.AddModelError(string.Empty, "Habitation details not exist.");
                        if (PMGSYSession.Current.PMGSYScheme == 1)
                        {
                            return PartialView("ViewOtherHabitationDetail", new HabitationDetails());
                        }
                        else
                        {
                            return PartialView("ViewOtherHabitationDetailPMGSYII", new HabitationDetails());
                        }

                    }
                    ViewBag.HabitationName = decryptedParameters["HabitationName"].ToString();
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        return PartialView("ViewOtherHabitationDetail", details_habitations);
                    }
                    else
                    {
                        return PartialView("ViewOtherHabitationDetailPMGSYII", details_habitations);
                    }



                }
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    return PartialView("ViewOtherHabitationDetail", new HabitationDetails());
                }
                else
                {
                    return PartialView("ViewOtherHabitationDetailPMGSYII", new HabitationDetails());
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Habitation details not exist.");
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    return PartialView("AddOtherHabitationDetails", new HabitationDetails());
                }
                else
                {
                    return PartialView("AddOtherHabitationDetailsPMGSYII", new HabitationDetails());
                }
            }
        }

        #endregion OtherHabitationDetailsDataEntry

        #region MLAConstituencyBlockMapping



        public ActionResult MapMLAConstituencyBlocks(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    //PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();


                    List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                    districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(Convert.ToInt32(decryptedParameters["StateCode"].ToString()), true);
                    ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");

                    ViewBag.EncryptedStateCode = parameter + '/' + hash + '/' + key;

                    ViewBag.MLAConstituencyName = decryptedParameters["MLAConstituencyName"].ToString();
                    ViewBag.StateName = decryptedParameters["StateName"].ToString();

                    ViewBag.MLAConstituencyCode = decryptedParameters["MLAConstituencyCode"].ToString();

                    return PartialView("MapMLAConstituencyBlocks");
                }
                return PartialView("MapMLAConstituencyBlocks");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MapMLAConstituencyBlocks");
            }
        }

        [HttpPost]
        public ActionResult GetBlockDetailsList_Mapping_MLA(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = 0;
            int districtCode = 0;
            int MLAConstituencyCode = 0;

            try
            {
                //if (!string.IsNullOrEmpty(homeFormCollection["searchField"]))
                //{
                //    searchParameters = HttpUtility.HtmlDecode(homeFormCollection["searchField"]);

                //    searchParameters = searchParameters.Replace("%2F", "/");
                //    string[] str = (searchParameters.ToString().Split('&'));
                //    for (int i = 0; i < str.Length; ++i)
                //    {
                //        string[] splitParameter = str[i].Split('=');
                //        parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                //    }


                //    encryptedParameters = Request.Params["EncryptedStateCode"].Split('/'); //parameters["EncryptedStateCode"].Split('/');
                //    districtCode = Convert.ToInt16(parameters["ddlSearchDistrict"]);


                //    if (encryptedParameters.Length == 3)
                //    {
                //        parameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                //        stateCode = Convert.ToInt32(parameters["MLAStateCode"].ToString());
                //    }

                //}

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                encryptedParameters = Request.Params["MLAStateCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    stateCode = Convert.ToInt32(decryptedParameters["StateCode"].ToString());
                }

                districtCode = Convert.ToInt16(Request.Params["DistrictCode"]);

                MLAConstituencyCode = Convert.ToInt16(Request.Params["MLAConstituencyCode"]);

                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetBlockDetailsListBAL(true, true, stateCode, districtCode, MLAConstituencyCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = 0,//totalRecords <= Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }


        [HttpPost]
        public ActionResult GetBlockDetailsList_Mapping_MP(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = 0;
            int districtCode = 0;
            int MPConstituencyCode = 0;

            try
            {
                //if (!string.IsNullOrEmpty(homeFormCollection["searchField"]))
                //{
                //    searchParameters = HttpUtility.HtmlDecode(homeFormCollection["searchField"]);

                //    searchParameters = searchParameters.Replace("%2F", "/");
                //    string[] str = (searchParameters.ToString().Split('&'));
                //    for (int i = 0; i < str.Length; ++i)
                //    {
                //        string[] splitParameter = str[i].Split('=');
                //        parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                //    }


                //    encryptedParameters = Request.Params["EncryptedStateCode"].Split('/'); //parameters["EncryptedStateCode"].Split('/');
                //    districtCode = Convert.ToInt16(parameters["ddlSearchDistrict"]);


                //    if (encryptedParameters.Length == 3)
                //    {
                //        parameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                //        stateCode = Convert.ToInt32(parameters["MLAStateCode"].ToString());
                //    }

                //}

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                encryptedParameters = Request.Params["MLAStateCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    stateCode = Convert.ToInt32(decryptedParameters["StateCode"].ToString());
                }

                districtCode = Convert.ToInt16(Request.Params["DistrictCode"]);

                MPConstituencyCode = Convert.ToInt16(Request.Params["MPConstituencyCode"]);

                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetBlockDetailsListBAL(true, false, stateCode, districtCode, MPConstituencyCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = 0,//totalRecords <= Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }


        [HttpPost]
        public ActionResult MapMLAConstituencyBlocks(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedBlockCodes = string.Empty;
            string encryptedMLAConstituencyCode = string.Empty;
            try
            {

                if (string.IsNullOrEmpty(frmCollection["EncryptedStateCode"]) || string.IsNullOrEmpty(frmCollection["EncryptedBlockCodes"]))
                {
                    message = "MLA Constituency and Block Mapping not added.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                encryptedBlockCodes = frmCollection["EncryptedBlockCodes"];
                encryptedMLAConstituencyCode = frmCollection["EncryptedStateCode"];

                if (masterDataEntryBAL.MapMLAConstituencyBlocksBAL(encryptedMLAConstituencyCode, encryptedBlockCodes))
                {

                    message = "MLA Constituency and Block are mapped successfully.";
                    status = true;
                }
                else
                {
                    message = "MLA Constituency and Block are not mapped.";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "MLA Constituency and Block are not mapped.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult MappedMLAConstituencyBlocks(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    ViewBag.EncryptedMLAConstituencyCode_Mapped = parameter + '/' + hash + '/' + key;

                    ViewBag.MLAConstituencyName = decryptedParameters["MLAConstituencyName"].ToString();
                    ViewBag.StateName = decryptedParameters["StateName"].ToString();

                    return PartialView("MappedMLAConstituencyBlocks");
                }
                return PartialView("MappedMLAConstituencyBlocks");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MappedMLAConstituencyBlocks");
            }
        }


        [HttpPost]
        public ActionResult GetBlockDetailsList_Mapped_MLA(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int mlaConstituencyCode = 0;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                encryptedParameters = Request.Params["MLAConstituencyCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    mlaConstituencyCode = Convert.ToInt32(decryptedParameters["MLAConstituencyCode"].ToString());
                }

                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetMappedBlockDetailsListBAL_MLA(mlaConstituencyCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }


        public ActionResult DeleteMappedMLABlock(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int adminId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                adminId = Convert.ToInt32(decryptedParameters["BlockId"]);
                bool status = masterDataEntryBAL.DeleteMappedMLABlockBAL(adminId, ref message);
                if (status == true)
                {
                    return Json(new { success = true });
                }
                else
                {
                    message = message == string.Empty ? "An error occured while processing your request." : message;

                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false });
            }
        }


        #endregion MLAConstituencyBlockMapping


        #region MPConstituencyBlockMapping



        public ActionResult MapMPConstituencyBlocks(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    //  PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();


                    List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                    districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(Convert.ToInt32(decryptedParameters["StateCode"].ToString()), true);

                    ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");

                    ViewBag.EncryptedStateCode = parameter + '/' + hash + '/' + key;

                    ViewBag.MPConstituencyName = decryptedParameters["MPConstituencyName"].ToString();
                    ViewBag.StateName = decryptedParameters["StateName"].ToString();

                    ViewBag.MPConstituencyCode = decryptedParameters["MPConstituencyCode"].ToString();

                    return PartialView("MapMPConstituencyBlocks");
                }
                return PartialView("MapMPConstituencyBlocks");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MapMPConstituencyBlocks");
            }
        }

        [HttpPost]
        public ActionResult MapMPConstituencyBlocks(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedBlockCodes = string.Empty;
            string encryptedMPConstituencyCode = string.Empty;
            try
            {

                if (string.IsNullOrEmpty(frmCollection["EncryptedStateCode"]) || string.IsNullOrEmpty(frmCollection["EncryptedBlockCodes"]))
                {
                    message = "MP Constituency and Block are not mapped.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                encryptedBlockCodes = frmCollection["EncryptedBlockCodes"];
                encryptedMPConstituencyCode = frmCollection["EncryptedStateCode"];

                if (masterDataEntryBAL.MapMPConstituencyBlocksBAL(encryptedMPConstituencyCode, encryptedBlockCodes))
                {

                    message = "MP Constituency and Block are mapped successfully.";
                    status = true;
                }
                else
                {
                    message = "MP Constituency and Block are not mapped .";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "MP Constituency and Block are not mapped.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult MappedMPConstituencyBlocks(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    ViewBag.EncryptedMPConstituencyCode_Mapped = parameter + '/' + hash + '/' + key;

                    ViewBag.MPConstituencyName = decryptedParameters["MPConstituencyName"].ToString();
                    ViewBag.StateName = decryptedParameters["StateName"].ToString();

                    return PartialView("MappedMPConstituencyBlocks");
                }
                return PartialView("MappedMPConstituencyBlocks");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MappedMPConstituencyBlocks");
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetBlockDetailsList_Mapped_MP(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int mpConstituencyCode = 0;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                encryptedParameters = Request.Params["MPConstituencyCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    mpConstituencyCode = Convert.ToInt32(decryptedParameters["MPConstituencyCode"].ToString());
                }

                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetMappedBlockDetailsListBAL_MP(mpConstituencyCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }


        public ActionResult DeleteMappedMPBlock(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int adminId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                adminId = Convert.ToInt32(decryptedParameters["BlockId"]);
                bool status = masterDataEntryBAL.DeleteMappedMPBlockBAL(adminId);
                if (status == true)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false });
            }
        }

        #endregion MPConstituencyBlockMapping


        #region PanchayathabitationsMapping


        public ActionResult MapPanchayatHabitations(String parameter, String hash, String key)
        {
            //  Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    // PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();



                    ViewBag.EncryptedBlockCode = parameter + '/' + hash + '/' + key;

                    ViewBag.PanchayatName = decryptedParameters["PanchayatName"].ToString();
                    ViewBag.StateName = decryptedParameters["StateName"].ToString();
                    ViewBag.DistrictName = decryptedParameters["DistrictName"].ToString();
                    ViewBag.BlockName = decryptedParameters["BlockName"].ToString();

                    return PartialView("MapPanchayatHabitations");
                }
                return PartialView("MapPanchayatHabitations");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MapPanchayatHabitations");
            }
        }

        [HttpPost]
        public JsonResult GetVillagesForHabitationMapping(String parameter, String hash, String key)
        {
            //  Dictionary<string, string> decryptedParameters = null;
            //GetHabitationDetailsList
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    return GetVillagesByBlockCode_Search(decryptedParameters["BlockCode"].ToString());
                }
                return Json(false);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }

        }


        [HttpPost]
        public ActionResult GetHabitationDetailsList_Mapping(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            String villageName = string.Empty;
            string habitationName = string.Empty;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                encryptedParameters = Request.Params["BlockCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
                }

                villageName = Request.Params["VillageName"].ToString().Trim().Replace('+', ' ');
                habitationName = Request.Params["HabitationName"].ToString().Trim().Replace('+', ' ');
                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetHabitationDetailsListBAL(true, stateCode, districtCode, blockCode, villageName, habitationName, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = 0,//totalRecords <= Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }


        [HttpPost]
        public ActionResult GetHabitationNameByBlockCodeList(String parameter, String hash, String key)
        {

            int blockCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());

                    List<SelectListItem> habitationList = new List<SelectListItem>();

                    PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                    habitationList = masterDataEntryDAL.GetAllHabitationNameByBlockCode(blockCode, true);

                    return Json(habitationList);
                }
                return Json(false);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }


        [HttpPost]
        public ActionResult MapPanchayatHabitations(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedHabCodes = string.Empty;
            string encryptedPanchayatCode = string.Empty;
            try
            {

                if (string.IsNullOrEmpty(frmCollection["EncryptedBlockCode"]) || string.IsNullOrEmpty(frmCollection["EncryptedHabCodes"]))
                {
                    message = "MP Constituency and Block are not mapped.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }


                encryptedHabCodes = frmCollection["EncryptedHabCodes"];
                encryptedPanchayatCode = frmCollection["EncryptedBlockCode"];

                if (masterDataEntryBAL.MapPanchayatHabitationsBAL(encryptedPanchayatCode, encryptedHabCodes))
                {

                    message = "Panchayat and Habitation are mapped successfully.";
                    status = true;
                }
                else
                {
                    message = "Panchayat and Habitation are not mapped.";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Panchayat and Habitation are not mapped.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }




        public ActionResult MappedPanchayatHabitations(String parameter, String hash, String key)
        {
            // Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    ViewBag.EncryptedPanchayatCode_Mapped = parameter + '/' + hash + '/' + key;


                    ViewBag.PanchayatName = decryptedParameters["PanchayatName"].ToString();
                    ViewBag.StateName = decryptedParameters["StateName"].ToString();
                    ViewBag.DistrictName = decryptedParameters["DistrictName"].ToString();
                    ViewBag.BlockName = decryptedParameters["BlockName"].ToString();
                    return PartialView("MappedPanchayatHabitations");
                }
                return PartialView("MappedPanchayatHabitations");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MappedPanchayatHabitations");
            }
        }


        [HttpPost]
        public ActionResult GetHabitationDetailsList_Mapped_Panchayat(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int panchayatCode = 0;


            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                encryptedParameters = Request.Params["PanchayatCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    panchayatCode = Convert.ToInt32(decryptedParameters["PanchayatCode"].ToString());
                }

                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetMappedHabitationDetailsListBAL_Panchayat(panchayatCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        //new Action added by Vikram on 24-08-2013


        public ActionResult DeleteMappedHabitation(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int adminId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                adminId = Convert.ToInt32(decryptedParameters["HabCode"]);
                bool status = masterDataEntryBAL.DeleteMappedHabitationBAL(adminId);
                if (status == true)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false });
            }
        }



        #endregion PanchayathabitationsMapping

        #region ShiftDistrict


        public ActionResult ShiftDistrict(String id)
        {
            int stateCode = 0;
            int districtCode = 0;
            try
            {
                if (id != string.Empty)
                {
                    ViewBag.EncryptedDistrictCode = id;

                    encryptedParameters = id.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        stateCode = Convert.ToInt32(decryptedParameters["StateCode"].ToString());
                        districtCode = Convert.ToInt32(decryptedParameters["DistrictCode"].ToString());
                    }
                    List<Models.MASTER_STATE> stateList = masterDataEntryDAL.GetAllStates(false);

                    stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });

                    stateList = stateList.Where(s => s.MAST_STATE_CODE != stateCode).ToList<Models.MASTER_STATE>();

                    ViewData["StateList"] = new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");//masterDataEntryDAL.GetAllStates(false);

                    ViewBag.ExistingStateName = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == stateCode).Select(s => s.MAST_STATE_NAME).FirstOrDefault();
                    ViewBag.ExistingDistrictName = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_DISTRICT_NAME).FirstOrDefault();

                    return PartialView("ShiftDistrict");
                }
                return PartialView("ShiftDistrict");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ShiftDistrict");
            }
        }

        [HttpPost]
        public ActionResult ShiftDistrict(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedDistrictCodes = string.Empty;
            string newStateCode = string.Empty;
            try
            {

                if (string.IsNullOrEmpty(frmCollection["EncryptedDistrictCode"]) || string.IsNullOrEmpty(frmCollection["ddlSearchStates_Shift"]))
                {
                    message = "District is not shifted.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                encryptedDistrictCodes = frmCollection["EncryptedDistrictCode"];
                newStateCode = frmCollection["ddlSearchStates_Shift"];

                if (masterDataEntryBAL.ShiftDistrictBAL(encryptedDistrictCodes, newStateCode))
                {

                    message = "District shifted successfully.";
                    status = true;
                }
                else
                {
                    message = "District is not shifted.";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "District is not shifted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion ShiftDistrict

        #region ShiftBlock

        public ActionResult ShiftBlock(String id)
        {
            int districtCode = 0;
            int stateCode = 0;
            int blockCode = 0;
            try
            {
                if (id != string.Empty)
                {
                    //  PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                    ViewBag.EncryptedBlockCode = id;

                    encryptedParameters = id.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        districtCode = Convert.ToInt32(decryptedParameters["DistrictCode"].ToString());
                        blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
                    }

                    stateCode = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_STATE_CODE).FirstOrDefault();

                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode);//GetAllStates(false);

                    List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                    districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false);
                    //districtList.RemoveAt(0);

                    districtList = districtList.Where(d => d.MAST_DISTRICT_CODE != districtCode).ToList<Models.MASTER_DISTRICT>();

                    ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");

                    ViewBag.ExistingStateName = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == stateCode).Select(s => s.MAST_STATE_NAME).FirstOrDefault();
                    ViewBag.ExistingDistrictName = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_DISTRICT_NAME).FirstOrDefault();
                    ViewBag.ExistingBlockName = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();


                    return PartialView("ShiftBlock");
                }
                return PartialView("ShiftBlock");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ShiftBlock");
            }
        }


        [HttpPost]
        public ActionResult ShiftBlock(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedBlockCode = string.Empty;
            string newStateCode = string.Empty;
            string newDistictCode = string.Empty;
            try
            {

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftBlock()");
                    sw.WriteLine("EncryptedBlockCode : " + frmCollection["EncryptedBlockCode"]);
                    sw.WriteLine("ddlSearchDistricts_ShiftBlock : " + frmCollection["ddlSearchDistricts_ShiftBlock"]);
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                if (string.IsNullOrEmpty(frmCollection["EncryptedBlockCode"]) || string.IsNullOrEmpty(frmCollection["ddlSearchDistricts_ShiftBlock"]))
                {
                    message = "Invalid District or Block code.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                encryptedBlockCode = frmCollection["EncryptedBlockCode"];
                //newStateCode = frmCollection["ddlSearchStates_Shift"];
                newDistictCode = frmCollection["ddlSearchDistricts_ShiftBlock"];

                if (masterDataEntryBAL.ShiftBlockBAL(encryptedBlockCode, newDistictCode))
                {
                    message = "Block shifted successfully.";
                    status = true;
                }
                else
                {
                    message = "Error occured while Block shifting.";
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftBlock()");
                    sw.WriteLine("status : " + status.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "LocationMasterDataEntry.ShiftBlock");
                message = "Error occurred while processing request for block shift.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion ShiftBlock

        #region ShiftVillage


        public ActionResult ShiftVillage(String id)
        {
            int districtCode = 0;
            int stateCode = 0;
            int blockCode = 0;
            int villageCode = 0;
            try
            {
                if (id != string.Empty)
                {
                    // PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                    ViewBag.EncryptedVillageCode = id;


                    encryptedParameters = id.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
                        villageCode = Convert.ToInt32(decryptedParameters["VillageCode"].ToString());
                    }


                    districtCode = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_DISTRICT_CODE).FirstOrDefault();
                    stateCode = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_STATE_CODE).FirstOrDefault();

                    if (PMGSYSession.Current.StateCode == 0)
                    {
                        ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);

                        List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                        districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false);
                        //  districtList.RemoveAt(0);
                        ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                    }
                    else if (PMGSYSession.Current.StateCode > 0)
                    {
                        ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false).Where(z => z.MAST_STATE_CODE == PMGSYSession.Current.StateCode), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);
                        ViewData["DistrictList"] = new SelectList(masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false).Where(z=>z.MAST_DISTRICT_CODE == districtCode), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                    }
                    //Models.MASTER_DISTRICT master_district=districtList.ElementAt(0);
                    List<Models.MASTER_BLOCK> blockList = new List<Models.MASTER_BLOCK>();
                    blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(districtCode, false);
                    //blockList.RemoveAt(0);

                    blockList = blockList.Where(b => b.MAST_BLOCK_CODE != blockCode).ToList<Models.MASTER_BLOCK>();

                    ViewData["BlockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");



                    ViewBag.ExistingStateName = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == stateCode).Select(s => s.MAST_STATE_NAME).FirstOrDefault();
                    ViewBag.ExistingDistrictName = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_DISTRICT_NAME).FirstOrDefault();
                    ViewBag.ExistingBlockName = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
                    ViewBag.ExistingVillageName = dbContext.MASTER_VILLAGE.Where(v => v.MAST_VILLAGE_CODE == villageCode).Select(v => v.MAST_VILLAGE_NAME).FirstOrDefault();

                    return PartialView("ShiftVillage");
                }
                return PartialView("ShiftVillage");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ShiftVillage");
            }
        }

        [HttpPost]
        public ActionResult ShiftVillage(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedVillageCode = string.Empty;
            string newBlockCode = string.Empty;
            try
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftBlock()");
                    sw.WriteLine("EncryptedVillageCode : " + frmCollection["EncryptedVillageCode"]);
                    sw.WriteLine("ddlSearchBlocks_ShiftVillage : " + frmCollection["ddlSearchBlocks_ShiftVillage"]);
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                if (string.IsNullOrEmpty(frmCollection["EncryptedVillageCode"]) || string.IsNullOrEmpty(frmCollection["ddlSearchBlocks_ShiftVillage"]))
                {
                    message = "Village not shifted successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                encryptedVillageCode = frmCollection["EncryptedVillageCode"];

                newBlockCode = frmCollection["ddlSearchBlocks_ShiftVillage"];

                if (masterDataEntryBAL.ShiftVillageBAL(encryptedVillageCode, newBlockCode))
                {

                    message = "Village shifted successfully.";
                    status = true;
                }
                else
                {
                    message = "Village is not shifted.";
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftVillage()");
                    sw.WriteLine("status : " + status.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntry.ShiftVillage()");
                message = "Village is not shifted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion ShiftVillage


        #region Shift Village Block

        public ActionResult ShiftVillageBlock(String id)
        {
            int districtCode = 0;
            int stateCode = 0;
            int blockCode = 0;
            int villageCode = 0;
            try
            {
                if (id != string.Empty)
                {
                    ViewBag.EncryptedVillageCode = id;
                    encryptedParameters = id.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
                        villageCode = Convert.ToInt32(decryptedParameters["VillageCode"].ToString());
                    }


                    districtCode = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_DISTRICT_CODE).FirstOrDefault();
                    stateCode = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_STATE_CODE).FirstOrDefault();

                    if (PMGSYSession.Current.StateCode == 0)
                    {
                        ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);

                        List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                        districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false);
                        //  districtList.RemoveAt(0);
                        ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                    }
                    else if (PMGSYSession.Current.StateCode > 0)
                    {
                        ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false).Where(z => z.MAST_STATE_CODE == PMGSYSession.Current.StateCode), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);
                        ViewData["DistrictList"] = new SelectList(masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false).Where(z => z.MAST_DISTRICT_CODE == districtCode), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                    }
                    //Models.MASTER_DISTRICT master_district=districtList.ElementAt(0);
                    List<Models.MASTER_BLOCK> blockList = new List<Models.MASTER_BLOCK>();
                    blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(districtCode, false);
                    //blockList.RemoveAt(0);

                    blockList = blockList.Where(b => b.MAST_BLOCK_CODE != blockCode).ToList<Models.MASTER_BLOCK>();

                    ViewData["BlockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewBag.ExistingStateName = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == stateCode).Select(s => s.MAST_STATE_NAME).FirstOrDefault();
                    ViewBag.ExistingDistrictName = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_DISTRICT_NAME).FirstOrDefault();
                    ViewBag.ExistingBlockName = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
                    ViewBag.ExistingVillageName = dbContext.MASTER_VILLAGE.Where(v => v.MAST_VILLAGE_CODE == villageCode).Select(v => v.MAST_VILLAGE_NAME).FirstOrDefault();
                    return PartialView("ShiftVillageBlock");
                }
                return PartialView("ShiftVillageBlock");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ShiftVillageBlock");
            }
        }



        [HttpPost]
        public ActionResult ShiftVillageBlock(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedVillageCode = string.Empty;
            string newBlockCode = string.Empty;
            PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();
            try
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftVillageBlock()");
                    sw.Close();
                }

                if (string.IsNullOrEmpty(frmCollection["EncryptedVillageCode1"]) || string.IsNullOrEmpty(frmCollection["ddlSearchBlocks_ShiftVillage1"])) // ddlSearchBlocks_ShiftVillage
                {
                    message = "Village not shifted in new Block.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                encryptedVillageCode = frmCollection["EncryptedVillageCode1"];

                newBlockCode = frmCollection["ddlSearchBlocks_ShiftVillage1"];

                if (masterDataEntryDAL.ShiftVillageToNewBlock(encryptedVillageCode, newBlockCode))
                {

                    message = "Village shifted successfully.";
                    status = true;
                }
                else
                {
                    message = "Village is not shifted in New Block.";
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftVillageBlock()");
                    sw.WriteLine("status : " + status.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntry.ShiftVillageBlock()");
                message = "Village is not shifted in new Block";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }





        public ActionResult ShiftHabitationToNewVillage(String id)
        {
            int districtCode = 0;
            int stateCode = 0;
            int blockCode = 0;
            int villageCode = 0;
            int habCode = 0;
            try
            {
                if (id != string.Empty)
                {
                    ViewBag.EncryptedHabCode = id;
                    encryptedParameters = id.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        habCode = Convert.ToInt32(decryptedParameters["HabCode"].ToString());
                        //blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
                        //villageCode = Convert.ToInt32(decryptedParameters["VillageCode"].ToString());
                    }

                    villageCode = dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == habCode).Select(m => m.MAST_VILLAGE_CODE).FirstOrDefault();
                    blockCode = dbContext.MASTER_VILLAGE.Where(m => m.MAST_VILLAGE_CODE == villageCode).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();
                    
                    districtCode = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_DISTRICT_CODE).FirstOrDefault();
                    stateCode = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_STATE_CODE).FirstOrDefault();

                    if (PMGSYSession.Current.StateCode == 0)
                    {
                        ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);

                        List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                        districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false);
                        //  districtList.RemoveAt(0);
                        ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                    }
                    else if (PMGSYSession.Current.StateCode > 0)
                    {
                        ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false).Where(z => z.MAST_STATE_CODE == PMGSYSession.Current.StateCode), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);
                        ViewData["DistrictList"] = new SelectList(masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false).Where(z => z.MAST_DISTRICT_CODE == districtCode), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                    }
                    //Models.MASTER_DISTRICT master_district=districtList.ElementAt(0);
                    List<Models.MASTER_BLOCK> blockList = new List<Models.MASTER_BLOCK>();
                    blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(districtCode, false);
                    blockList = blockList.Where(b => b.MAST_BLOCK_CODE != blockCode).ToList<Models.MASTER_BLOCK>();

                    List<Models.MASTER_VILLAGE> villageList = new List<Models.MASTER_VILLAGE>();
                    villageList = masterDataEntryDAL.GetAllVillagesByBlockCode(blockCode, false);
                    villageList = villageList.Where(m => m.MAST_VILLAGE_CODE != villageCode).ToList<Models.MASTER_VILLAGE>();

                    ViewData["BlockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewData["VillageList"] = new SelectList(villageList, "MAST_VILLAGE_CODE", "MAST_VILLAGE_NAME");


                    ViewBag.ExistingStateName = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == stateCode).Select(s => s.MAST_STATE_NAME).FirstOrDefault();
                    ViewBag.ExistingDistrictName = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_DISTRICT_NAME).FirstOrDefault();
                    ViewBag.ExistingBlockName = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
                    ViewBag.ExistingVillageName = dbContext.MASTER_VILLAGE.Where(v => v.MAST_VILLAGE_CODE == villageCode).Select(v => v.MAST_VILLAGE_NAME).FirstOrDefault();
                    return PartialView("ShiftHabitationToNewVillage");
                }
                return PartialView("ShiftHabitationToNewVillage");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ShiftHabitationToNewVillage");
            }
        }


        [HttpPost]
        public ActionResult ShiftHabitationToNewVillage(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedHabCode = string.Empty;
            string newBlockCode = string.Empty;
            string newVillageCode = string.Empty;
            PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();
            try
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftHabitationToNewVillage()");
                    sw.Close();
                }

                if (string.IsNullOrEmpty(frmCollection["EncryptedHabCode"]) || string.IsNullOrEmpty(frmCollection["ddlSearchBlocks_ShiftVillage2"])) // ddlSearchBlocks_ShiftVillage
                {
                    message = "Habitation is not shifted to new Village...";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                encryptedHabCode = frmCollection["EncryptedHabCode"]; // Olde Village Code

                newBlockCode = frmCollection["ddlSearchBlocks_ShiftVillage2"];

                newVillageCode = frmCollection["ddlSearchVillage_ShiftVillage2"];

                if (masterDataEntryDAL.ShiftHabToNewVillageDAL(encryptedHabCode, newBlockCode, newVillageCode))
                {

                    message = "Habitation is shifted to new Village Successfully.";
                    status = true;
                }
                else
                {
                    message = "Habitation is not shifted to new Village..";
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftHabitationToNewVillage()");
                    sw.WriteLine("status : " + status.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntry.ShiftHabitationToNewVillage()");
                message = "Habitation is not shifted to new Village.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion




        #region ShiftPanchayat


        public ActionResult ShiftPanchayat(String id)
        {
            int districtCode = 0;
            int stateCode = 0;
            int blockCode = 0;
            int panchayatCode = 0;

            try
            {
                if (id != string.Empty)
                {

                    ViewBag.EncryptedPanchayatCode = id;

                    encryptedParameters = id.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
                        panchayatCode = Convert.ToInt32(decryptedParameters["PanchayatCode"].ToString());
                    }


                    districtCode = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_DISTRICT_CODE).FirstOrDefault();
                    stateCode = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_STATE_CODE).FirstOrDefault();


                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode);//GetAllStates(false);

                    List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                    districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false);
                    //districtList.RemoveAt(0);
                    ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);

                    // Models.MASTER_DISTRICT master_district = districtList.ElementAt(0);
                    List<Models.MASTER_BLOCK> blockList = new List<Models.MASTER_BLOCK>();
                    blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(districtCode, false);//master_district.MAST_DISTRICT_CODE
                    //blockList.RemoveAt(0);

                    blockList = blockList.Where(b => b.MAST_BLOCK_CODE != blockCode).ToList<Models.MASTER_BLOCK>();

                    ViewData["BlockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");

                    ViewBag.ExistingStateName = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == stateCode).Select(s => s.MAST_STATE_NAME).FirstOrDefault();
                    ViewBag.ExistingDistrictName = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_DISTRICT_NAME).FirstOrDefault();
                    ViewBag.ExistingBlockName = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
                    ViewBag.ExistingPanchayatName = dbContext.MASTER_PANCHAYAT.Where(p => p.MAST_PANCHAYAT_CODE == panchayatCode).Select(p => p.MAST_PANCHAYAT_NAME).FirstOrDefault();


                    return PartialView("ShiftPanchayat");
                }
                return PartialView("ShiftPanchayat");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ShiftPanchayat");
            }
        }

        [HttpPost]
        public ActionResult ShiftPanchayat(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedPanchayatCode = string.Empty;
            string newBlockCode = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(frmCollection["EncryptedPanchayatCode"]) || string.IsNullOrEmpty(frmCollection["ddlSearchBlocks_ShiftPanchayat"]))
                {
                    message = "Panchayat is not shifted.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                encryptedPanchayatCode = frmCollection["EncryptedPanchayatCode"];

                newBlockCode = frmCollection["ddlSearchBlocks_ShiftPanchayat"];

                if (masterDataEntryBAL.ShiftPanchayatBAL(encryptedPanchayatCode, newBlockCode))
                {

                    message = "Panchayat shifted successfully.";
                    status = true;
                }
                else
                {
                    message = "Panchayat is not shifted.";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Panchayat is not shifted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion ShiftPanchayat

        #region commonfunctions

        /* private SelectList GetAllStates(bool flag)
        {
            List<Models.MASTER_STATE> stateList = new List<Models.MASTER_STATE>();

            PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

            stateList = masterDataEntryDAL.GetAllStates();

            if (flag)
            {
                stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" }); 
            }

            return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");
        }*/

        protected override void Dispose(bool disposing)
        {
            dbContext.Dispose();
            base.Dispose(disposing);
        }
        #endregion commonfunctions

        #region Quality Monitor User Name Detail

        #endregion

        #region Addition of Facitlity
        /// <summary>
        /// added by abhinav pathak on 17-12-2018
        /// This region performs the CRUD operation related
        /// to the facilities available in a district.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FacilityDetails()
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
                FacilitySearch SearchFacility = new FacilitySearch();
                List<SelectListItem> facility = new List<SelectListItem>();
                List<SelectListItem> habitation = new List<SelectListItem>();
                List<SelectListItem> facilityNameLst = new List<SelectListItem>();
                List<PMGSY.Models.MASTER_FACILITY_CATEGORY> facilitylst = (from item in dbContext.MASTER_FACILITY_CATEGORY
                                                                           where item.MASTER_FACILITY_PARENT_ID == null
                                                                           select item).ToList<PMGSY.Models.MASTER_FACILITY_CATEGORY>();

                SearchFacility.RoleName = PMGSYSession.Current.RoleName;

                int i = 0;
                foreach (var item in facilitylst)
                {
                    facility.Insert(i, new SelectListItem { Value = item.MASTER_FACILITY_CATEGORT_ID.ToString(), Text = item.MASTER_FACILITY_CATEGORY_NAME });
                }

                facility = facility.OrderBy(x => x.Text).ToList();
                facility.Insert(0, new SelectListItem { Value = "0", Text = "All Category" });
                SearchFacility.facilityList = new SelectList(facility.ToList(), "Value", "Text");

                List<SelectListItem> dist = new List<SelectListItem>();
                dist.Insert(0, new SelectListItem { Value = PMGSYSession.Current.DistrictCode.ToString(), Text = PMGSYSession.Current.DistrictName, Selected = true });
                SearchFacility.DistrictList = new SelectList(dist.ToList(), "Value", "Text");

                SearchFacility.BlockList = comm.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                SearchFacility.BlockList.Remove(SearchFacility.BlockList.Find(x => x.Text == "Select Block"));
                SearchFacility.BlockList.Insert(0, new SelectListItem { Text = "All Blocks", Value = "0", Selected = true });

                habitation.Insert(0, new SelectListItem { Value = "0", Text = "All Habitation" });
                SearchFacility.habitationList = new SelectList(habitation.ToList(), "Value", "Text");

                facilityNameLst.Insert(0, new SelectListItem { Value = "0", Text = "All Types" });
                SearchFacility.facilityNameList = new SelectList(facilityNameLst.ToList(), "Value", "Text");

                SearchFacility.operation = "S";
                return View(SearchFacility);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.FacilityDetails()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult SearchFacilityForm()
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
                FacilitySearch SearchFacility = new FacilitySearch();
                List<SelectListItem> facility = new List<SelectListItem>();
                List<SelectListItem> habitation = new List<SelectListItem>();
                List<SelectListItem> facilityNameLst = new List<SelectListItem>();
                List<PMGSY.Models.MASTER_FACILITY_CATEGORY> facilitylst = (from item in dbContext.MASTER_FACILITY_CATEGORY
                                                                           where item.MASTER_FACILITY_PARENT_ID == null
                                                                           select item).ToList<PMGSY.Models.MASTER_FACILITY_CATEGORY>();

                int i = 0;
                foreach (var item in facilitylst)
                {
                    facility.Insert(i, new SelectListItem { Value = item.MASTER_FACILITY_CATEGORT_ID.ToString(), Text = item.MASTER_FACILITY_CATEGORY_NAME });
                }

                facility = facility.OrderBy(x => x.Text).ToList();
                facility.Insert(0, new SelectListItem { Value = "0", Text = "All Category" });
                SearchFacility.facilityList = new SelectList(facility.ToList(), "Value", "Text");

             

                List<SelectListItem> dist = new List<SelectListItem>();
                dist.Insert(0, new SelectListItem { Value = PMGSYSession.Current.DistrictCode.ToString(), Text = PMGSYSession.Current.DistrictName, Selected = true });
                SearchFacility.DistrictList = new SelectList(dist.ToList(), "Value", "Text");
              

                SearchFacility.BlockList = comm.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                SearchFacility.BlockList.Remove(SearchFacility.BlockList.Find(x => x.Text == "Select Block"));
                SearchFacility.BlockList.Insert(0, new SelectListItem { Text = "All Blocks", Value = "0", Selected = true });

                habitation.Insert(0, new SelectListItem { Value = "0", Text = "All Habitation" });
                SearchFacility.habitationList = new SelectList(habitation.ToList(), "Value", "Text");

                facilityNameLst.Insert(0, new SelectListItem { Value = "0", Text = "All Types" });
                SearchFacility.facilityNameList = new SelectList(facilityNameLst.ToList(), "Value", "Text");

                SearchFacility.operation = "S";
                return View(SearchFacility);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.FacilityDetails()");
                return null;
            }
        }




        [HttpGet]
        public ActionResult AddFacilityLayout()
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
                CreateFacility createFacility = new CreateFacility();
                List<SelectListItem> facility = new List<SelectListItem>();
                List<SelectListItem> habitation = new List<SelectListItem>();
                List<SelectListItem> facilityNameLst = new List<SelectListItem>();
                List<PMGSY.Models.MASTER_FACILITY_CATEGORY> facilitylst = (from item in dbContext.MASTER_FACILITY_CATEGORY
                                                                           where item.MASTER_FACILITY_PARENT_ID == null
                                                                           select item).ToList<PMGSY.Models.MASTER_FACILITY_CATEGORY>();

                int i = 0;
                foreach (var item in facilitylst)
                {
                    facility.Insert(i, new SelectListItem { Value = item.MASTER_FACILITY_CATEGORT_ID.ToString(), Text = item.MASTER_FACILITY_CATEGORY_NAME });
                }

                facility = facility.OrderBy(x => x.Text).ToList();
                facility.Insert(0, new SelectListItem { Value = "0", Text = "-Select-" });
                createFacility.facilityList = new SelectList(facility.ToList(), "Value", "Text");

                List<SelectListItem> dist = new List<SelectListItem>();
                dist.Insert(0, new SelectListItem { Value = PMGSYSession.Current.DistrictCode.ToString(), Text = PMGSYSession.Current.DistrictName, Selected = true });
                createFacility.DistrictList = new SelectList(dist.ToList(), "Value", "Text");

                //Avinash
                //createFacility.BlockList = new SelectList(comm.PopulateBlocks(PMGSYSession.Current.DistrictCode, false).ToList(), "Value", "Text").ToList();
                createFacility.BlockList = comm.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);

                habitation.Insert(0, new SelectListItem { Value = "0", Text = "-Select-" });
                createFacility.habitationList = new SelectList(habitation.ToList(), "Value", "Text");

                facilityNameLst.Insert(0, new SelectListItem { Value = "0", Text = "-Select-" });
                createFacility.facilityNameList = new SelectList(facilityNameLst.ToList(), "Value", "Text");

                createFacility.isPMGSY3Finalized = masterDataEntryDAL.IsPMGSY3FinalizedDAL(createFacility.MAST_BLOCK_CODE, PMGSYSession.Current.DistrictCode);

                return PartialView(createFacility);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.AddFacilityLayout()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult CheckLockStatusPMGSY3()
        {
            try
            {
                int blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                if (blockCode > 0)
                {
                    bool flag = masterDataEntryDAL.IsPMGSY3FinalizedDAL(blockCode, PMGSYSession.Current.DistrictCode);

                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "CheckUnlocked()");
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetHabitationBlockCode()
        {

            int blockCode = 0;
            try
            {

                blockCode = Convert.ToInt32(Request.Params["blockCode"]);

                List<SelectListItem> habitationList = new List<SelectListItem>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                habitationList = masterDataEntryDAL.GetAllHabitationNameByBlockCode(blockCode, true);
                habitationList = habitationList.OrderBy(x => x.Text).ToList();
                habitationList.Insert(0, new SelectListItem { Value = "0", Text = "-Select-" });
                return Json(habitationList, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.GetHabitationBlockCode()");

                return null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFacilityDetails(CreateFacility createFacility)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    createFacility.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                    if (masterDataEntryBAL.SaveFacilityDetailsBAL(createFacility, ref message))
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                        //message = "Facility Details could not be saved";
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    status = false;
                    message = "Facility Details could not be saved";
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.SaveFacilityDetails()");
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetFacilityDetailsList(FormCollection formdata)
        {
            try
            {
                var formData = Request.Params["formdata"];
                var FormArrayKeys = formdata[0].Split('&');
                List<string> ModelValues = new List<string>();
                foreach (var item in FormArrayKeys)
                {
                    ModelValues.Add(item.Split('=')[1]);
                }
                int districtcode = PMGSYSession.Current.DistrictCode;
                int facilitycode = Convert.ToInt32(ModelValues.ElementAt(3));
                int facilityType = Convert.ToInt32(ModelValues.ElementAt(4));
                int blockcode = Convert.ToInt32(ModelValues.ElementAt(5));
                int habcode = Convert.ToInt32(ModelValues.ElementAt(6));
                int totalRecords = 0;
                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetFacilityDetailsListBAL(Convert.ToInt32(formdata["page"]) - 1, Convert.ToInt32(formdata["rows"]), formdata["sidx"], formdata["sord"], out totalRecords, ModelValues),
                    total = totalRecords <= Convert.ToInt32(formdata["rows"]) ? 1 : totalRecords / Convert.ToInt32(formdata["rows"]) + 1,
                    page = Convert.ToInt32(formdata["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.GetFacilityDetailsList()");
                return null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFacilityDetails(String parameter, String hash, String key)
        {
            try
            {
                bool status = false;
                string message = string.Empty;

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int faclityid = Convert.ToInt32(decryptedParameters["FacilityCode"].ToString());

                if (dbContext.MASTER_FACILITY.Where(m => m.LATITUDE != null && m.MASTER_FACILITY_ID == faclityid).Any())
                {
                    message = "Latitude and Longitude details are present against this Facility. Hence Facility Details can not be deleted.";
                    status = false;

                }






                if (masterDataEntryBAL.DeleteFacilityBAL(faclityid))
                {
                    message = message == string.Empty ? "Facility Details Deleted Successfully" : message;
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
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.DeleteFacilityDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetFacilityNameByFacilityCode()
        {
            try
            {
                //PMGSYEntities dbContext = new PMGSYEntities();
                int facilityCode = Convert.ToInt32(Request.Params["facilityCode"]);
                List<SelectListItem> facilityNameList = new List<SelectListItem>();
                var facilityList = (from item in dbContext.MASTER_FACILITY_CATEGORY
                                    where item.MASTER_FACILITY_PARENT_ID == facilityCode
                                    select new
                                    {
                                        item.MASTER_FACILITY_CATEGORT_ID,
                                        item.MASTER_FACILITY_CATEGORY_NAME

                                    }).ToList();

                int i = 0;
                foreach (var item in facilityList)
                {
                    facilityNameList.Insert(i, new SelectListItem { Value = item.MASTER_FACILITY_CATEGORT_ID.ToString(), Text = item.MASTER_FACILITY_CATEGORY_NAME });
                    i = i + 1;
                }

                facilityNameList = facilityNameList.OrderBy(x => x.Text).ToList();
                facilityNameList.Insert(0, new SelectListItem { Value = "0", Text = "-Select-" });
                return Json(facilityNameList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.GetFacilityNameByFacilityCode()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult EditFacilityDetails(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int facilityID = Convert.ToInt32(decryptedParameters["FacilityCode"].ToString());
                // int facilityID = Convert.ToInt32(Request.Params["facilityid"]);
                List<PMGSY.Models.MASTER_FACILITY_CATEGORY> facilitylst = new List<PMGSY.Models.MASTER_FACILITY_CATEGORY>();
                //PMGSYEntities dbContext = new PMGSYEntities();
                PMGSY.Models.MASTER_FACILITY masterFact = new PMGSY.Models.MASTER_FACILITY();
                CommonFunctions comm = new CommonFunctions();
                CreateFacility createFacility = new CreateFacility();
                List<SelectListItem> facility = new List<SelectListItem>();
                List<SelectListItem> habitation = new List<SelectListItem>();
                List<SelectListItem> facilityNameLst = new List<SelectListItem>();
                PMGSY.Models.MASTER_HABITATIONS habitationName = new PMGSY.Models.MASTER_HABITATIONS();

                facilitylst = (from item in dbContext.MASTER_FACILITY_CATEGORY
                               where item.MASTER_FACILITY_PARENT_ID == null
                               select item).ToList<PMGSY.Models.MASTER_FACILITY_CATEGORY>();

                var masterdata = (from item in dbContext.MASTER_FACILITY
                                  join facilityName in dbContext.MASTER_FACILITY_CATEGORY on
                                  item.MASTER_FACILITY_CATEGORY_ID equals facilityName.MASTER_FACILITY_CATEGORT_ID

                                  where item.MASTER_FACILITY_ID == facilityID
                                  select new
                                  {
                                      item.ADDRESS,
                                      //item.MASTER_FACILITY_CATEGORY,
                                      item.MASTER_FACILITY_CATEGORY_ID,
                                      item.MASTER_FACILITY_ID,
                                      item.MASTER_FACILITY_SUB_CATEGORY_ID,
                                      facilityName.MASTER_FACILITY_CATEGORT_ID,
                                      //facilityName.MASTER_FACILITY_CATEGORT_ID,
                                      facilityName.MASTER_FACILITY_CATEGORY_NAME,
                                      facilityName.MASTER_FACILITY_PARENT_ID,
                                      item.MASTER_FACILITY_DESC,
                                      item.PINCODE
                                  }).ToList();

                int masterSubID = masterdata.ElementAt(0).MASTER_FACILITY_SUB_CATEGORY_ID;

                var masterFacility = (from item in dbContext.MASTER_FACILITY_CATEGORY
                                      where item.MASTER_FACILITY_CATEGORT_ID == masterSubID
                                      select new
                                      {
                                          item.MASTER_FACILITY_CATEGORT_ID,
                                          item.MASTER_FACILITY_CATEGORY_NAME,
                                      }).ToList();

                var habName = (from item in dbContext.FACILITY_HABITATION_MAPPING
                               join hab in dbContext.MASTER_HABITATIONS on item.MASTER_HAB_CODE equals hab.MAST_HAB_CODE
                               where item.MASTER_FACILITY_ID == facilityID
                               select new
                               {
                                   hab.MAST_HAB_CODE,
                                   hab.MAST_HAB_NAME,
                                   hab.MASTER_VILLAGE.MAST_VILLAGE_TOT_POP
                               }).ToList();


                var facilityList = (from item in dbContext.MASTER_FACILITY_CATEGORY
                                    where item.MASTER_FACILITY_PARENT_ID != null
                                    select new
                                    {
                                        item.MASTER_FACILITY_CATEGORT_ID,
                                        item.MASTER_FACILITY_CATEGORY_NAME

                                    }).ToList();
                int i = 0;
                foreach (var item in facilitylst)
                {
                    facility.Insert(i, new SelectListItem { Value = item.MASTER_FACILITY_CATEGORT_ID.ToString(), Text = item.MASTER_FACILITY_CATEGORY_NAME });
                }
                var removeItem = facility.Find(c => c.Value == masterdata.ElementAt(0).MASTER_FACILITY_CATEGORT_ID.ToString());
                facility.Remove(removeItem);
                facility.Insert(0, new SelectListItem { Value = masterdata.ElementAt(0).MASTER_FACILITY_CATEGORT_ID.ToString(), Text = masterdata.ElementAt(0).MASTER_FACILITY_CATEGORY_NAME.ToString(), Selected = true });
                createFacility.facilityList = new SelectList(facility.ToList(), "Value", "Text");
                List<SelectListItem> dist = new List<SelectListItem>();
                dist.Insert(0, new SelectListItem { Value = PMGSYSession.Current.DistrictCode.ToString(), Text = PMGSYSession.Current.DistrictName, Selected = true });
                createFacility.DistrictList = new SelectList(dist.ToList(), "Value", "Text");

                //Avinash
                //createFacility.BlockList = new SelectList(comm.PopulateBlocks(PMGSYSession.Current.DistrictCode, false).ToList(), "Value", "Text").ToList();
                createFacility.BlockList = comm.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                createFacility.MAST_BLOCK_CODE = Convert.ToInt32(dbContext.FACILITY_HABITATION_MAPPING.Where(x => x.MASTER_FACILITY_ID == facilityID).Select(x => x.MASTER_BLOCK_CODE).FirstOrDefault());


                //habitation.Insert(0, new SelectListItem { Value = habName.ElementAt(0).MAST_HAB_CODE.ToString(), Text = habName.ElementAt(0).MAST_HAB_NAME.ToString() + "(H.Code: " + habName.ElementAt(0).MAST_HAB_CODE.ToString() + " , Pop: " + habName.ElementAt(0).MAST_VILLAGE_TOT_POP.ToString() + ")", Selected = true });

                habitation = masterDataEntryDAL.GetAllHabitationNameByBlockCode(createFacility.MAST_BLOCK_CODE, true);

                createFacility.HabitationCode = habName.Select(m => m.MAST_HAB_CODE).FirstOrDefault(); //Convert.ToInt32(habitation.ElementAt(0).Value);
                createFacility.habitationList = new SelectList(habitation, "Value", "Text");
                facilityNameLst.Insert(0, new SelectListItem { Value = masterFacility.ElementAt(0).MASTER_FACILITY_CATEGORT_ID.ToString(), Text = masterFacility.ElementAt(0).MASTER_FACILITY_CATEGORY_NAME, Selected = true });
                createFacility.facilityNameList = new SelectList(facilityNameLst.ToList(), "Value", "Text",
                                                  new SelectListItem { Value = masterFacility.ElementAt(0).MASTER_FACILITY_CATEGORT_ID.ToString(), Text = masterFacility.ElementAt(0).MASTER_FACILITY_CATEGORY_NAME, Selected = true });

                createFacility.EncryptedFacilityCode = masterdata.ElementAt(0).MASTER_FACILITY_ID.ToString();
                createFacility.facilityCode = masterdata.ElementAt(0).MASTER_FACILITY_CATEGORT_ID;
                createFacility.FacilityName = masterFacility.ElementAt(0).MASTER_FACILITY_CATEGORT_ID;
                createFacility.address = masterdata.ElementAt(0).ADDRESS;
                createFacility.FacilityDescription = masterdata.ElementAt(0).MASTER_FACILITY_DESC;
                createFacility.pincode = Convert.ToString(masterdata.ElementAt(0).PINCODE);
                return PartialView("AddFacilityLayout", createFacility);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.EditFacilityDetails()");

                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFacilityPost(CreateFacility model, string id)
        {
            bool status = false;
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    int facilityID = Convert.ToInt32(id);
                    //PMGSYEntities dbContext = new PMGSYEntities();
                    PMGSY.Models.MASTER_FACILITY masterFact = new PMGSY.Models.MASTER_FACILITY();
                    PMGSY.Models.FACILITY_HABITATION_MAPPING facilityMapping = new PMGSY.Models.FACILITY_HABITATION_MAPPING();

                    //Avinash
                    //Description Validation
                    if (dbContext.FACILITY_HABITATION_MAPPING.Where(x => x.MASTER_DISTRICT_CODE == model.MAST_DISTRICT_CODE && x.MASTER_BLOCK_CODE == model.MAST_BLOCK_CODE && x.MASTER_HAB_CODE == model.HabitationCode).Any())
                    {

                        List<PMGSY.Models.FACILITY_HABITATION_MAPPING> lstMapping = new List<PMGSY.Models.FACILITY_HABITATION_MAPPING>();
                        PMGSY.Models.MASTER_FACILITY master = new PMGSY.Models.MASTER_FACILITY();
                        lstMapping = dbContext.FACILITY_HABITATION_MAPPING.Where(x => x.MASTER_DISTRICT_CODE == model.MAST_DISTRICT_CODE && x.MASTER_BLOCK_CODE == model.MAST_BLOCK_CODE && x.MASTER_HAB_CODE == model.HabitationCode).ToList();
                        foreach (var item in lstMapping)
                        {
                            master = dbContext.MASTER_FACILITY.Where(x => x.MASTER_FACILITY_ID == item.MASTER_FACILITY_ID && x.MASTER_FACILITY_CATEGORY_ID == model.facilityCode && x.MASTER_FACILITY_SUB_CATEGORY_ID == model.FacilityName).FirstOrDefault();

                            if (master != null)
                            {
                                if (master.MASTER_FACILITY_DESC == model.FacilityDescription.ToUpper() || master.MASTER_FACILITY_DESC == model.FacilityDescription.ToLower())
                                {
                                    message = "Facility Description For Selected Block And Habitation already Entered Please Enter Another Description";
                                    status = true;
                                    return Json(new { success = status, message = message });

                                }
                            }
                        }
                    }

                    if (model.FacilityDescription.Length > 200)
                    {
                        message = "Maximum 200 Characters allowed in Facility Description";
                        status = false;
                        return Json(new { success = status, message = message });
                    }


                    if (model.address.Length > 255)
                    {
                        message = "Maximum 255 Characters allowed in Address";
                        status = false;
                        return Json(new { success = status, message = message });
                    }


                    if (model.facilityCode == 0)
                    {
                        message = "Please Select Facility Category";
                        status = false;
                        return Json(new { success = status, message = message });
                    }

                    if (model.FacilityName == 0)
                    {
                        message = "Please Select Facility Type";
                        status = false;
                        return Json(new { success = status, message = message });
                    }

                    if (string.IsNullOrEmpty(model.FacilityDescription))
                    {
                        message = "Please Enter Facility Description";
                        status = false;
                        return Json(new { success = status, message = message });
                    }


                    if (string.IsNullOrEmpty(model.address))
                    {
                        message = "Please Enter Address";
                        status = false;
                        return Json(new { success = status, message = message });
                    }

                    if (string.IsNullOrEmpty(model.pincode))
                    {
                        message = "Please Enter PinCode";
                        status = false;
                        return Json(new { success = status, message = message });
                    }


                    if (model.MAST_BLOCK_CODE == 0)
                    {
                        message = "Please Select Block";
                        status = false;
                        return Json(new { success = status, message = message });
                    }


                    if (model.HabitationCode == 0)
                    {
                        message = "Please Select Habitation";
                        status = false;
                        return Json(new { success = status, message = message });
                    }


                    masterFact = (from item in dbContext.MASTER_FACILITY
                                  where item.MASTER_FACILITY_ID == facilityID
                                  select item).FirstOrDefault();

                    facilityMapping = (from item in dbContext.FACILITY_HABITATION_MAPPING
                                       where item.MASTER_FACILITY_ID == facilityID
                                       select item).FirstOrDefault();

                    masterFact.MASTER_FACILITY_CATEGORY_ID = model.facilityCode;
                    masterFact.MASTER_FACILITY_SUB_CATEGORY_ID = model.FacilityName;
                    masterFact.ADDRESS = model.address.Trim();
                    masterFact.PINCODE = Convert.ToInt32(model.pincode);
                    masterFact.MASTER_FACILITY_DESC = model.FacilityDescription.Trim();
                    facilityMapping.MASTER_HAB_CODE = model.HabitationCode;
                    //Avinash
                    facilityMapping.MASTER_BLOCK_CODE = model.MAST_BLOCK_CODE;
                    dbContext.SaveChanges();
                    message = "Facility Details Updated Successfully";
                    status = true;
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    status = false;
                    message = "Facility Details Could Not Be Updated";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.EditFacilityPost()");
                message = "Facility Details Could Not Be Updated";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult DisplayFacilityDetails(String parameter, String hash, String key)
        {
            try
            {
                CreateFacility model = new CreateFacility();
                // int facilityID = 0;


                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int facilityID = Convert.ToInt32(decryptedParameters["FacilityCode"].ToString());

                model = masterDataEntryBAL.DisplayfacilityDetailsBAL(facilityID);

                model.EncryptedCode = URLEncrypt.EncryptParameters1(new string[] { "FacilityCode =" + facilityID.ToString().Trim() });

                //added by abhinav pathak on 27-aug-2019
                if (!string.IsNullOrEmpty(model.FileName))
                    //model.button = "<input id='btnDeleteImage' type='button' value='Delete Image' class='jqueryButton' title='click here to delete image' onClick = DeleteImage('" + model.FacilityID + "') />";

                model.FileName = ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD_VIRTUAL_DIR_PATH"] + "/" + model.FileName;
                if (model != null)
                {
                    if (String.IsNullOrEmpty(model.FileName))
                    {
                        model.Errmessage = "Image not Present";
                    }
                    else
                    {
                        model.Errmessage = string.Empty;

                    }
                }



                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntry/DisplayFacilityDetails");
                return null;
            }
        }


        [HttpPost]
        public ActionResult GetFacilityNameSearch()
        {
            try
            {
                //PMGSYEntities dbContext = new PMGSYEntities();
                int facilityCode = Convert.ToInt32(Request.Params["facilityCode"]);
                List<SelectListItem> facilityNameList = new List<SelectListItem>();
                var facilityList = (from item in dbContext.MASTER_FACILITY_CATEGORY
                                    where item.MASTER_FACILITY_PARENT_ID == facilityCode
                                    select new
                                    {
                                        item.MASTER_FACILITY_CATEGORT_ID,
                                        item.MASTER_FACILITY_CATEGORY_NAME

                                    }).ToList();

                int i = 0;
                foreach (var item in facilityList)
                {
                    facilityNameList.Insert(i, new SelectListItem { Value = item.MASTER_FACILITY_CATEGORT_ID.ToString(), Text = item.MASTER_FACILITY_CATEGORY_NAME });
                    i = i + 1;
                }

                facilityNameList = facilityNameList.OrderBy(x => x.Text).ToList();
                facilityNameList.Insert(0, new SelectListItem { Value = "0", Text = "All Types" });
                return Json(facilityNameList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.GetFacilityNameByFacilityCode()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetHabitationBlockCodeSearch()
        {

            int blockCode = 0;
            try
            {

                blockCode = Convert.ToInt32(Request.Params["blockCode"]);

                List<SelectListItem> habitationList = new List<SelectListItem>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                habitationList = masterDataEntryDAL.GetAllHabitationNameByBlockCode(blockCode, true);
                habitationList = habitationList.OrderBy(x => x.Text).ToList();
                habitationList.Insert(0, new SelectListItem { Value = "0", Text = "All Habitation" });
                return Json(habitationList, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.GetHabitationBlockCode()");

                return null;
            }

        }

        [HttpPost]
        public ActionResult DeleteImageLatLong(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int facilityID = Convert.ToInt32(decryptedParameters["FacilityCode"].ToString());
                var remark = Request.Params["Remark"];

                Regex RemarkCheck = new Regex(@"^[0-9a-zA-Z''-'\s]{1,254}$");
                if (RemarkCheck.IsMatch(remark.Trim()))
                {

                    if (masterDataEntryBAL.DeleteImageLatLong(facilityID, remark))
                    {
                        return Json(new { success = true, message = "Image has been deleted successfully." }, "application/json", JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Image has not been deleted." }, "application/json", JsonRequestBehavior.DenyGet);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(remark) || string.IsNullOrWhiteSpace(remark))
                        return Json(new { success = false, message = "Enter Remark" }, "application/json", JsonRequestBehavior.DenyGet);
                    else
                        return Json(new { success = false, message = "Invalid Remark" }, "application/json", JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController/DeleteImageLatLong");
                return Json(new { success = false, message = "An error occured while processing the request" }, "application/json", JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public ActionResult UploadFacilityPhoto(String parameter, String hash, String key)
        {
            PhotoUploadModel viewmodel = new PhotoUploadModel();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int facilityID = Convert.ToInt32(decryptedParameters["FacilityCode"].ToString());
                var FacilityDetails = dbContext.MASTER_FACILITY.Where(x => x.MASTER_FACILITY_ID == facilityID).FirstOrDefault();
                viewmodel.FacilityID = URLEncrypt.EncryptParameters1(new string[] { "FacilityCode =" + facilityID });

                viewmodel.NumberofImages = 0;
                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntry/UploadFacilityPhoto");
                return null;
            }
        }

        public ActionResult SavePhotograph(PhotoUploadModel formmodel)
        {
            try
            {
                CommonFunctions objCommonFunc = new CommonFunctions();

                String parameter = formmodel.FacilityID.Split('/')[0];
                String hash = formmodel.FacilityID.Split('/')[1];
                String key = formmodel.FacilityID.Split('/')[2];
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int facilityid = Convert.ToInt32(decryptedParameters["FacilityCode"].ToString());

                foreach (string file in Request.Files)
                {
                    string status = ValidatePhoto(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        formmodel.ErrorMessage = status;
                        return Json(new { message = "Photograph format is not valid", success = false });
                    }
                }
                bool isFileSaved = false;
                HttpPostedFileBase FileBase = null;
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    FileBase = Request.Files[i];
                    var filename = FileBase.FileName;
                    isFileSaved = masterDataEntryDAL.SavePhotoGraphDAL(facilityid, filename, FileBase);
                }

                formmodel.NumberofImages = 0;

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                if (isFileSaved)
                    return Json(new { success = isFileSaved, message = "Photograph uploaded successfully" }, JsonRequestBehavior.DenyGet);
                else
                    return Json(new { success = isFileSaved, message = "Photograph not uploaded" }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntry/SavePhotograph");
                return null;
            }
        }

        public string ValidatePhoto(int FileSize, string FileExtension)
        {
            if (!(FileExtension.ToUpper().Equals(".JPEG") || FileExtension.ToUpper().Equals(".JPG")))
            {
                return "Photograph not in correct format";
            }
            if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }

        public ActionResult GetRemarkForm(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int facilityID = Convert.ToInt32(decryptedParameters["FacilityCode"].ToString());
                DeletionRemark viewmodel = new DeletionRemark();
                viewmodel.FacilityID = parameter + "/" + hash + "/" + key;

                var FacilityDetails = dbContext.MASTER_FACILITY.Where(x => x.MASTER_FACILITY_ID == facilityID).FirstOrDefault();
                var HabDetails = dbContext.FACILITY_HABITATION_MAPPING.Include("MASTER_BLOCK.MASTER_DISTRICT").Where(x => x.MASTER_FACILITY_ID == facilityID).FirstOrDefault();
                var hab = dbContext.FACILITY_HABITATION_MAPPING.Where(x => x.MASTER_FACILITY_ID == facilityID).FirstOrDefault();
                viewmodel.blockname = HabDetails.MASTER_BLOCK.MAST_BLOCK_NAME;
                viewmodel.HabName = HabDetails.MASTER_HABITATIONS.MAST_HAB_NAME;
                viewmodel.facilityname = FacilityDetails.MASTER_FACILITY_DESC;

                return View("RemarkForm", viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntry/GetRemarkForm");
                return null;
            }
        }


        #endregion

        #region SSRS Map
        public ActionResult ShowMap(string id)
        {
            try
            {
                ShowMap model = new ShowMap();
                int facilityID = 0;
                facilityID = Convert.ToInt32(id);

                model = ShowMapDetails(facilityID);
                model.FileNameDetails = ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD_VIRTUAL_DIR_PATH"] + "/" + model.FileNameDetails;
                if (model != null)
                {
                    if (String.IsNullOrEmpty(model.FileNameDetails))
                    {
                        model.Errmessage = "Image not Present";
                    }
                    else
                    {
                        model.Errmessage = string.Empty;

                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntry/DisplayFacilityDetails");
                return null;
            }
        }

        public ShowMap ShowMapDetails(int facilityID)
        {
            ShowMap model = new ShowMap();
            try
            {
                PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();

                var facilityInfo = (from item in dbContext.MASTER_FACILITY
                                    where item.MASTER_FACILITY_ID == facilityID
                                    select item).FirstOrDefault();

                var facilitycategory = (from item in dbContext.MASTER_FACILITY_CATEGORY
                                        where item.MASTER_FACILITY_CATEGORT_ID == facilityInfo.MASTER_FACILITY_SUB_CATEGORY_ID
                                        select item).FirstOrDefault();


                var facilityParentCategory = (from item in dbContext.MASTER_FACILITY_CATEGORY
                                              where item.MASTER_FACILITY_CATEGORT_ID == facilityInfo.MASTER_FACILITY_CATEGORY_ID
                                              select item).FirstOrDefault();

                var habitationDetails = (from item in dbContext.FACILITY_HABITATION_MAPPING
                                         where item.MASTER_FACILITY_ID == facilityID
                                         select item).FirstOrDefault();

                var districtname = (from item in dbContext.MASTER_DISTRICT
                                    where item.MAST_DISTRICT_CODE == habitationDetails.MASTER_DISTRICT_CODE
                                    select item).FirstOrDefault();

                var BlockName = (from item in dbContext.MASTER_BLOCK
                                 where item.MAST_BLOCK_CODE == habitationDetails.MASTER_BLOCK_CODE
                                 select item).FirstOrDefault();

                var HabName = (from item in dbContext.MASTER_HABITATIONS
                               where item.MAST_HAB_CODE == habitationDetails.MASTER_HAB_CODE
                               select item).FirstOrDefault();

                model.DistrictName = districtname.MAST_DISTRICT_NAME;
                model.blockName = BlockName.MAST_BLOCK_NAME;
                model.habName = HabName.MAST_HAB_NAME;
                model.FacilityCategory = facilitycategory.MASTER_FACILITY_CATEGORY_NAME;
                model.FacilityParentCategory = facilityParentCategory.MASTER_FACILITY_CATEGORY_NAME;
                model.DisplayAddress = facilityInfo.ADDRESS;
                model.DisplayPIN = Convert.ToString(facilityInfo.PINCODE);
                model.FacilityDesc = facilityInfo.MASTER_FACILITY_DESC;
                model.LatitudeDetails = Convert.ToDouble(facilityInfo.LATITUDE);
                model.LongitudeDetails = Convert.ToDouble(facilityInfo.LONGITUDE);
                model.UploadDateDetails = Convert.ToString(facilityInfo.FILE_UPLOAD_DATE);
                model.FacilityID = facilityID;
                model.FileNameDetails = facilityInfo.FILE_NAME;
                string FACILITY_PHYSICAL_LOCATION = ConfigurationManager.AppSettings["FACILITY_PHYSICAL_LOCATION"].ToString();
                string FACILITY_VIRTUAL_LOCATION = ConfigurationManager.AppSettings["FACILITY_VIRTUAL_LOCATION"].ToString();

                return model;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MasterDataEntryDAL/ShowMapDetails");
                return model;
            }

        }


        #endregion

        #region Finalize Facility and Definalize
        [HttpPost]
        [Audit]
        public ActionResult FinalizeFacility(String parameter, String hash, String key)
        {
            DAL.MasterDataEntryDAL dal = new DAL.MasterDataEntryDAL();


            try
            {
                // int facilityID = Convert.ToInt32(Request.Params["facilityCode"]);
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int facilityCode = Convert.ToInt32(decryptedParameters["FacilityCode"].ToString());

                string result = dal.FinalizeFacilityDAL(facilityCode);
                if (result.Equals(string.Empty))
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalController.FinalizeProposal()");

                return null;
            }

        }

        [HttpGet]
        public ActionResult SearchFacilityFormForDefinalization()
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
                FacilitySearch SearchFacility = new FacilitySearch();
                List<SelectListItem> facility = new List<SelectListItem>();
                List<SelectListItem> habitation = new List<SelectListItem>();
                List<SelectListItem> facilityNameLst = new List<SelectListItem>();
                List<PMGSY.Models.MASTER_FACILITY_CATEGORY> facilitylst = (from item in dbContext.MASTER_FACILITY_CATEGORY
                                                                           where item.MASTER_FACILITY_PARENT_ID == null
                                                                           select item).ToList<PMGSY.Models.MASTER_FACILITY_CATEGORY>();

                int i = 0;
                foreach (var item in facilitylst)
                {
                    facility.Insert(i, new SelectListItem { Value = item.MASTER_FACILITY_CATEGORT_ID.ToString(), Text = item.MASTER_FACILITY_CATEGORY_NAME });
                }

                facility = facility.OrderBy(x => x.Text).ToList();
                facility.Insert(0, new SelectListItem { Value = "0", Text = "All Category" });
                SearchFacility.facilityList = new SelectList(facility.ToList(), "Value", "Text");

                if (PMGSYSession.Current.RoleName.Equals("ITNO"))
                {
                    List<SelectListItem> dist = new List<SelectListItem>();
                    //  dist.Insert(0, new SelectListItem { Value = PMGSYSession.Current.DistrictCode.ToString(), Text = PMGSYSession.Current.DistrictName, Selected = true });
                    SearchFacility.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, false, 0, false, false);//new SelectList(dist.ToList(), "Value", "Text");
                    SearchFacility.RoleName = "ITNO";
                    SearchFacility.StateCode = PMGSYSession.Current.StateCode;
                }
                else
                {

                    List<SelectListItem> dist = new List<SelectListItem>();
                    dist.Insert(0, new SelectListItem { Value = PMGSYSession.Current.DistrictCode.ToString(), Text = PMGSYSession.Current.DistrictName, Selected = true });
                    SearchFacility.DistrictList = new SelectList(dist.ToList(), "Value", "Text");
                    SearchFacility.RoleName = "NA";
                    SearchFacility.StateCode = PMGSYSession.Current.StateCode;
                }

                SearchFacility.BlockList = comm.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                //SearchFacility.BlockList.Remove(SearchFacility.BlockList.Find(x => x.Text == "Select Block"));
                //SearchFacility.BlockList.Insert(0, new SelectListItem { Text = "All Blocks", Value = "0", Selected = true });

                habitation.Insert(0, new SelectListItem { Value = "-1", Text = "-Select Habitation-" });
                SearchFacility.habitationList = new SelectList(habitation.ToList(), "Value", "Text");

                facilityNameLst.Insert(0, new SelectListItem { Value = "0", Text = "All Types" });
                SearchFacility.facilityNameList = new SelectList(facilityNameLst.ToList(), "Value", "Text");

                SearchFacility.operation = "S";
                return View(SearchFacility);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.FacilityDetails()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeFinalizeFacility(String parameter, String hash, String key)
        {
            DAL.MasterDataEntryDAL dal = new DAL.MasterDataEntryDAL();


            try
            {
                // int facilityID = Convert.ToInt32(Request.Params["facilityCode"]);
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int facilityCode = Convert.ToInt32(decryptedParameters["FacilityCode"].ToString());

                string result = dal.DeFinalizeFacilityDAL(facilityCode);
                if (result.Equals(string.Empty))
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalController.FinalizeProposal()");

                return null;
            }

        }

        [HttpPost]
        public ActionResult DefinalizeFacilityForAll(List<String> values) // Definalize all on itno login added by priyanka 10-08-2020
        {
            DAL.MasterDataEntryDAL dal = new DAL.MasterDataEntryDAL();
            try
            {
                var facilityCodes = new List<int>();
                foreach (String item in values)
                {
                    var paramHashKey = item.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { paramHashKey[0], paramHashKey[1], paramHashKey[2] });
                    int facilityCode = Convert.ToInt32(decryptedParameters["FacilityCode"].ToString());
                    facilityCodes.Add(facilityCode);
                }
                string result = dal.DeFinalizeFacilityForAllDAL(facilityCodes);
                if (result.Equals(string.Empty))
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = result }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.DefinalizeFacilityForAll()");

                return null;
            }
        }


        [HttpPost]
        public ActionResult GetFacilityDetailsListForDefinalization(FormCollection formdata)
        {
            try
            {
                var formData = Request.Params["formdata"];
                var FormArrayKeys = formdata[0].Split('&');
                List<string> ModelValues = new List<string>();
                foreach (var item in FormArrayKeys)
                {
                    ModelValues.Add(item.Split('=')[1]);
                }
                int districtcode = PMGSYSession.Current.DistrictCode;

                //int facilitycode = Convert.ToInt32(ModelValues.ElementAt(3));
                //int facilityType = Convert.ToInt32(ModelValues.ElementAt(4));
                //int blockcode = Convert.ToInt32(ModelValues.ElementAt(5));
                //int habcode = Convert.ToInt32(ModelValues.ElementAt(6));

                int facilitycode = Convert.ToInt32(ModelValues.ElementAt(4));
                int facilityType = Convert.ToInt32(ModelValues.ElementAt(5));
                int blockcode = Convert.ToInt32(ModelValues.ElementAt(7));
                int habcode = Convert.ToInt32(ModelValues.ElementAt(8));

                int totalRecords = 0;
                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetFacilityDetailsListBALDefinalize(Convert.ToInt32(formdata["page"]) - 1, Convert.ToInt32(formdata["rows"]), formdata["sidx"], formdata["sord"], out totalRecords, ModelValues),
                    total = totalRecords <= Convert.ToInt32(formdata["rows"]) ? 1 : totalRecords / Convert.ToInt32(formdata["rows"]) + 1,
                    page = Convert.ToInt32(formdata["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.GetFacilityDetailsList()");
                return null;
            }

        }


        public ActionResult BlockDetailsForDefinalization(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), false);
            
            list.Find(x => x.Value == "0").Value = "-1";

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region by abhinav
        
        [HttpPost]
        public ActionResult GetHabitationBlockCodeSearchDefinalize()
        {

            int blockCode = 0;
            try
            {

                blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                bool isFinalized = false;

                var BlockfinalizeDetails = dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == blockCode).FirstOrDefault();

                if (BlockfinalizeDetails != null)
                {
                    isFinalized = BlockfinalizeDetails.IS_FINALIZED == "Y" ? true : false ;
                }

                List<SelectListItem> habitationList = new List<SelectListItem>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                habitationList = masterDataEntryDAL.GetAllHabitationNameByBlockCode(blockCode, true);
                habitationList = habitationList.OrderBy(x => x.Text).ToList();
                habitationList.Insert(0, new SelectListItem { Value = "0", Text = "All Habitation" });
                return Json(new { hablist = habitationList, isfinalzedval = isFinalized }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.GetHabitationBlockCode()");

                return null;
            }

        }

        //[HttpGet]
        //public ActionResult FacilityDetailsDefinalize()
        //{
        //    try
        //    {
        //        CommonFunctions comm = new CommonFunctions();
        //        PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
        //        FacilitySearch SearchFacility = new FacilitySearch();
        //        List<SelectListItem> facility = new List<SelectListItem>();
        //        List<SelectListItem> habitation = new List<SelectListItem>();
        //        List<SelectListItem> facilityNameLst = new List<SelectListItem>();
        //        List<PMGSY.Models.MASTER_FACILITY_CATEGORY> facilitylst = (from item in dbContext.MASTER_FACILITY_CATEGORY
        //                                                                   where item.MASTER_FACILITY_PARENT_ID == null
        //                                                                   select item).ToList<PMGSY.Models.MASTER_FACILITY_CATEGORY>();

        //        SearchFacility.RoleName = PMGSYSession.Current.RoleName;

        //        int i = 0;
        //        foreach (var item in facilitylst)
        //        {
        //            facility.Insert(i, new SelectListItem { Value = item.MASTER_FACILITY_CATEGORT_ID.ToString(), Text = item.MASTER_FACILITY_CATEGORY_NAME });
        //        }

        //        facility = facility.OrderBy(x => x.Text).ToList();
        //        facility.Insert(0, new SelectListItem { Value = "0", Text = "All Category" });
        //        SearchFacility.facilityList = new SelectList(facility.ToList(), "Value", "Text");

        //        List<SelectListItem> dist = new List<SelectListItem>();
        //        dist.Insert(0, new SelectListItem { Value = PMGSYSession.Current.DistrictCode.ToString(), Text = PMGSYSession.Current.DistrictName, Selected = true });
        //        SearchFacility.DistrictList = new SelectList(dist.ToList(), "Value", "Text");

        //        SearchFacility.BlockList = comm.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
        //        SearchFacility.BlockList.Remove(SearchFacility.BlockList.Find(x => x.Text == "Select Block"));
        //        SearchFacility.BlockList.Insert(0, new SelectListItem { Text = "All Blocks", Value = "0", Selected = true });

        //        habitation.Insert(0, new SelectListItem { Value = "0", Text = "All Habitation" });
        //        SearchFacility.habitationList = new SelectList(habitation.ToList(), "Value", "Text");

        //        facilityNameLst.Insert(0, new SelectListItem { Value = "0", Text = "All Types" });
        //        SearchFacility.facilityNameList = new SelectList(facilityNameLst.ToList(), "Value", "Text");

        //        SearchFacility.operation = "S";
        //        return View("FacilityDetails" , SearchFacility);

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "LocationMasterDataEntryController.FacilityDetails()");
        //        return null;
        //    }
        //}
        
        #endregion

        #endregion
    }
}