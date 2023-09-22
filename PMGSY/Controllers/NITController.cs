/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMASII

 * File Name: NITController.cs

 * Author : Koustubh Nakate

 * Creation Date :05/July/2013

 * Desc : This controller is used as get the request and send response as view,list for NIT i.e Notice Inviting Tenders screens.  
 
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models;
using PMGSY.Extensions;
using PMGSY.Common;
using PMGSY.BAL.NIT;
using PMGSY.DAL.NIT;
using PMGSY.Models.NIT;
using PMGSY.Models.Common;
namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class NITController : Controller
    {
        private PMGSYEntities dbContext = new PMGSYEntities();
        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;
        string message = string.Empty;

        INITBAL objNITBAL = new NITBAL();
        NITDAL objNITDAL = new NITDAL();
        int outParam = 0;
        public ActionResult NITDetails()
        {        
            //try
            //{
                          
            //}
            //catch
            //{
               
            //}

            return View("NITDetails");
        }


        [HttpPost]
        [Audit]
        public ActionResult GetNITDetailsList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            //int sanctionedYear = 0;
            //int blockCode = 0;
            //string packageID = string.Empty;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                //if (!string.IsNullOrEmpty(Request.Params["sanctionedYear"]))
                //{
                //    sanctionedYear = Convert.ToInt32(Request.Params["sanctionedYear"]);
                //}
                //if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                //{
                //    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                //}
                //if (!string.IsNullOrEmpty(Request.Params["packageID"]))
                //{
                //    packageID = Request.Params["packageID"].Trim();
                //}
                var jsonData = new
                {
                    rows = objNITBAL.GetNITDetailsBAL(stateCode, districtCode, adminNDCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1), //totalRecords / Convert.ToInt32(rows) + 1,
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

        [Audit]
        public ActionResult AddNITDetails()
        {
            NITDetails objNITDetails = new NITDetails();
            try
            {
                objNITDetails.TendItemRate = true;
                return PartialView("AddNITDetails", objNITDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("AddNITDetails", objNITDetails);
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult AddNITDetails(NITDetails objNITDetails)
        {
            bool status = false;
            try
            {
                //ModelState.AddModelError(string.Empty, "as");
                //return PartialView("AddNITDetails", objNITDetails);

                if (ModelState.IsValid)
                {

                    if (objNITBAL.SaveNITDetailsBAL(objNITDetails, ref message))
                    {
                        message = message == string.Empty ? "NIT details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "NIT details not saved successfully." : message;
                    }

                }
                else
                {
                    return PartialView("AddNITDetails", objNITDetails);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "NIT details not saved successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [Audit]
        public ActionResult EditNITDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    NITDetails objNITDetails = objNITBAL.GetNITDetailsBAL(Convert.ToInt32(decryptParameters["TendNITCode"].ToString()));

                    if (objNITDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "NIT details not exist.");
                        return PartialView("AddNITDetails", new NITDetails());
                    }

                    return PartialView("AddNITDetails", objNITDetails);
                }
                return PartialView("AddNITDetails", new NITDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "NIT details not exist.");
                return PartialView("AddNITDetails", new NITDetails());
            }
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditNITDetails(NITDetails objNITDetails)
        {
            bool status = false;
            try
            {
                //ModelState.AddModelError(string.Empty, "as");
                //return PartialView("AddNITDetails", objNITDetails);

                if (ModelState.IsValid)
                {
                    if (objNITBAL.UpdateNITDetailsBAL(objNITDetails, ref message))
                    {
                        message = message == string.Empty ? "NIT details updated successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "NIT details not updated successfully." : message;
                    }
                }
                else
                {
                    return PartialView("AddNITDetails", objNITDetails);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "NIT details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        [Audit]
        public ActionResult DeleteNITDetails(String parameter, String hash, String key)
        {

            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (objNITBAL.DeleteNITDetailsBAL(Convert.ToInt32(decryptedParameters["TendNITCode"].ToString()), ref message))
                    {
                        message = "NIT details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "NIT details not deleted successfully." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "NIT details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "NIT details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult PublishNIT(String parameter, String hash, String key)
        {
            int tendNITCode = 0;
            
            bool status = false;
            message = "NIT not published successfully.";
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    tendNITCode = Convert.ToInt32(decryptedParameters["TendNITCode"]);

                    if (objNITBAL.PublishNITBAL(tendNITCode))
                    {
                        message = "NIT published successfully.";
                        status = true;
                    }
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }//end function PublishNIT


        [Audit]
        public ActionResult NITRoadDetails(String parameter, String hash, String key)
        {
            NITRoadDetails objNITRoadDetails = new NITRoadDetails();
            objNITRoadDetails.EncryptedTendNITCode = parameter + "/" + hash + "/" + key;
            objNITRoadDetails.isEdit = false;

            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

            if (decryptedParameters.Count > 0)
            {
                int NITCode = Convert.ToInt32(decryptedParameters["TendNITCode"].ToString());
                DateTime? formIssueDate = dbContext.TEND_NIT_MASTER.Where(NIT => NIT.TEND_NIT_CODE == NITCode).Select(NIT => NIT.TEND_ISSUE_START_DATE).FirstOrDefault();
                objNITRoadDetails.TenderIssueStartDate = formIssueDate == null ? null : Convert.ToDateTime(formIssueDate).ToString("dd/MM/yyyy");
            }

            return PartialView("NITRoadDetails", objNITRoadDetails);
        }

        [Audit]
        public ActionResult AddNITRoadDetails(String parameter, String hash, String key)
        {
            NITRoadDetails objNITRoadDetails = new NITRoadDetails();
            try
            {

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {

                    objNITRoadDetails.EncryptedTendNITCode = parameter + '/' + hash + '/' + key;

                    ViewBag.NITNumber = decryptedParameters["NITNumber"].Replace("--", "/").ToString();
                    ViewBag.FundingAgency = decryptedParameters["FundingAgency"].ToString();

                    ViewBag.TenderIssueStartDate = decryptedParameters["TenderIssueStartDate"].ToString().Replace("--", "/");

                    objNITRoadDetails.TenderIssueStartDate = ViewBag.TenderIssueStartDate;

                    objNITRoadDetails.isEdit = false;
                    return PartialView("AddNITRoadDetails", objNITRoadDetails);
                }
                return PartialView("AddNITRoadDetails", objNITRoadDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("AddNITRoadDetails", objNITRoadDetails);
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult GetNITRoadDetailsList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int TendNITCode = 0;
            try
            { 
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                encryptedParameters = Request.Params["TendNITCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    TendNITCode = Convert.ToInt32(decryptedParameters["TendNITCode"].ToString());
                }

                var jsonData = new
                {
                    rows = objNITBAL.GetNITRoadDetailsListBAL(TendNITCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),
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
        [Audit]
        public JsonResult GetPackagesByYear(string sanctionYear)
        {

            try
            {
                CommonFunctions commonFunction = new CommonFunctions();
               // TransactionParams transactionParams = new TransactionParams();

                if (!int.TryParse(sanctionYear, out outParam))
                {
                    return Json(false);
                }

                //transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                //transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                //transactionParams.ISSearch = false;
                //transactionParams.SANC_YEAR = Convert.ToInt16(sanctionYear.Trim());

                return Json(new SelectList(commonFunction.GetPackages(Convert.ToInt32(sanctionYear), 0, false), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetPackagesByYear

        [HttpPost]
        [Audit]
        public JsonResult GetRoadsByPackag(string sanctionYear, string packageID)
        {

            try
            {
                if (!int.TryParse(sanctionYear, out outParam))
                {
                    return Json(false);
                }
                if(string.IsNullOrEmpty(packageID))
                {
                     return Json(false);
                }

                CommonFunctions commonFunction = new CommonFunctions();

                return Json(new SelectList(commonFunction.GetRoads(Convert.ToInt32(sanctionYear), packageID, false, true), "IMS_PR_ROAD_CODE", "IMS_ROAD_NAME"));
 
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetRoadsByPackag

        [HttpPost]
        [Audit]
        public JsonResult GetWorksByRoad(string roadCode)
        {

            try
            {
                if (!int.TryParse(roadCode, out outParam))
                {
                    return Json(false);
                }
              
                CommonFunctions commonFunction = new CommonFunctions();

                return Json(new SelectList(commonFunction.GetWorks(Convert.ToInt32(roadCode), false), "IMS_WORK_CODE", "IMS_WORK_DESC"));

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetWorksByRoad


        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddNITRoadDetails(NITRoadDetails objNITRoadDetails)
        {
            bool status = false;
            try
            {
                if (objNITRoadDetails.Works.Count() == 1)
                {
                    ModelState.Remove("IMS_WORK_CODE");
                }
             
                if (ModelState.IsValid)
                {

                    if (objNITBAL.SaveNITRoadDetailsBAL(objNITRoadDetails, ref message))
                    {
                        message = message == string.Empty ? "Road details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Road details not saved successfully." : message;
                    }

                }
                else
                {
                    return PartialView("NITRoadDetails", objNITRoadDetails);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = "Road details not saved successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [Audit]
        public ActionResult EditNITRoadDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    NITRoadDetails objNITRoadDetails = objNITBAL.GetNITRoadDetailsBAL(Convert.ToInt32(decryptParameters["TendNITID"].ToString()),Convert.ToInt32(decryptParameters["TendNITCode"].ToString()));

                   
                    objNITRoadDetails.isEdit = true;
                    if (objNITRoadDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Road details not exist.");
                        return PartialView("NITRoadDetails", new NITRoadDetails());
                    }

                    return PartialView("NITRoadDetails", objNITRoadDetails);
                }
                return PartialView("NITRoadDetails", new NITRoadDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Road details not exist.");
                return PartialView("NITRoadDetails", new NITRoadDetails());
            }
        }


        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditNITRoadDetails(NITRoadDetails objNITRoadDetails)
        {
            bool status = false;
            try
            {
                //ModelState.AddModelError(string.Empty, "as");
                //return PartialView("NITRoadDetails", objNITRoadDetails);
                
                if (objNITRoadDetails.Works.Count() == 1)
                {
                    ModelState.Remove("IMS_WORK_CODE");
                }

                if (ModelState.IsValid)
                {
                    if (objNITBAL.UpdateNITRoadDetailsBAL(objNITRoadDetails, ref message))
                    {
                        message = message == string.Empty ? "Road details updated successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "Road details not updated successfully." : message;
                    }
                }
                else
                {
                    return PartialView("NITRoadDetails", objNITRoadDetails);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Road details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [Audit]
        public ActionResult DeleteNITRoadDetails(String parameter, String hash, String key)
        {

            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (objNITBAL.DeleteNITRoadDetailsBAL(Convert.ToInt32(decryptedParameters["TendNITID"].ToString()),Convert.ToInt32(decryptedParameters["TendNITCode"].ToString()), ref message))
                    {
                        message = "Road details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Road details not deleted successfully." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Road details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Road details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        [Audit]
        public JsonResult GetEstimatedCostMaintenanceCost(string roadCode, string workCode)
        {
            string totalEstimatedCost = string.Empty;
            string totalMaintenanceCost = string.Empty;

            try
            {
                if (!int.TryParse(roadCode, out outParam))
                {
                    return Json(new { totalEstimatedCost = totalEstimatedCost, totalMaintenanceCost = totalMaintenanceCost }, JsonRequestBehavior.AllowGet);
                }
                if (!int.TryParse(workCode, out outParam))
                {
                    return Json(new { totalEstimatedCost = totalEstimatedCost, totalMaintenanceCost = totalMaintenanceCost }, JsonRequestBehavior.AllowGet);
                }

                objNITBAL.GetEstimatedCostMaintenanceCostBAL(Convert.ToInt32(roadCode), Convert.ToInt32(workCode), ref totalEstimatedCost, ref totalMaintenanceCost);

                return Json(new { totalEstimatedCost = totalEstimatedCost, totalMaintenanceCost=totalMaintenanceCost }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { totalEstimatedCost = totalEstimatedCost, totalMaintenanceCost = totalMaintenanceCost }, JsonRequestBehavior.AllowGet);
            }
        }//end function GetEstimatedCostMaintenanceCost

        [Audit]
        public ActionResult ViewNITRoadDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    NITRoadDetails objNITRoadDetails = objNITBAL.GetNITRoadDetailsBAL(Convert.ToInt32(decryptParameters["TendNITID"].ToString()), Convert.ToInt32(decryptParameters["TendNITCode"].ToString()), true);

                    IMS_SANCTIONED_PROJECTS IMSSanctioned = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == objNITRoadDetails.IMS_PR_ROAD_CODE).FirstOrDefault();
                    ViewBag.RoadName = IMSSanctioned.IMS_PROPOSAL_TYPE == "L" ? IMSSanctioned.IMS_BRIDGE_NAME : IMSSanctioned.IMS_ROAD_NAME;
                    ViewBag.WorkName = dbContext.IMS_PROPOSAL_WORK.Where(c => c.IMS_WORK_CODE == objNITRoadDetails.IMS_WORK_CODE).Select(c => c.IMS_WORK_DESC).FirstOrDefault();

                    objNITRoadDetails.isEdit = true;
                    if (objNITRoadDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Road details not exist.");
                        return PartialView("ViewNITRoadDetails", new NITRoadDetails());
                    }

                    return PartialView("ViewNITRoadDetails", objNITRoadDetails);
                }
                return PartialView("ViewNITRoadDetails", new NITRoadDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Road details not exist.");
                return PartialView("ViewNITRoadDetails", new NITRoadDetails());
            }
        }

        #region commonFunction
        protected override void Dispose(bool disposing)
        {
            dbContext.Dispose();
            base.Dispose(disposing);
        }
        #endregion commonFunction
    }
}
