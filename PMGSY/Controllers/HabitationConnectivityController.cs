#region FileHeader
/*---------------------------------------------------------------------------------------------
Poject Name :PMGSY-II
File Name: StateLoginController.cs
Path: PMGSY/Controller/StateLoginController
Created By: Ashish Markande
Ceation Date:04/07/2013
Purpose: To show Habitation list and to change the status of habitation.
-----------------------------------------------------------------------------------------------
*/
#endregion FileHeader

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.DAL.StateLogin;
using PMGSY.BAL.StateLogin;
using PMGSY.Common;
using PMGSY.Extensions;

namespace PMGSY.Controllers
{
    #region StateLoginController
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class HabitationConnectivityController : Controller
    {
       

        IHabitationConnectivityDAL objStateLoginDAL = new HabitationConnectivityDAL();
        IHabitationConnectivityBAL objStateLoginBAL = new HabitationConnectivityBAL();
        string message = string.Empty;

        /// <summary>
        /// Method is used to load the habitation list view.
        /// </summary>
        /// <returns></returns>
      [Audit]
        public ActionResult HabitationList()
        {
           
           //ViewData["State"] = objStateLoginDAL.GetAllStates();
            ViewData["District"] = new SelectList(objStateLoginDAL.GetAllDistricts(), "Value", "Text");
       
            ViewData["Blocks"] = new SelectList(objStateLoginDAL.GetAllBlocks(0), "Value", "Text");

            return View("HabitationList");
         }

        //[HttpPost]
        //public JsonResult GetAllDistrictsByState(string stateCode)
        //{
        //    try
        //    {
        //        return Json(new SelectList(objStateLoginDAL.GetAllDistricts(), "Value", "Text"), JsonRequestBehavior.AllowGet);
        //    }
        //    catch
        //    {
        //        return Json(false);
        //    }
        //}




        /// <summary>
        /// Method is used to get all blocks by district code.
        /// </summary>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        [HttpPost]
      [Audit]
      public JsonResult GetAllBlocksByDistrict(string districtCode)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            try
            {
                return Json(new SelectList(objStateLoginDAL.GetAllBlocks(Convert.ToInt32(districtCode)),"Value","Text"),JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        /// <summary>
        /// Method is used to load the habitation grid.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetHabitationList(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            String searchParameters = String.Empty;
            long totalRecords;
           // string streamType = String.Empty;
            int districtCode = 0;
            int blockCode = 0;
            objStateLoginBAL = new HabitationConnectivityBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["Districts"]))
            {
                districtCode = Convert.ToInt32(Request.Params["Districts"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["Blocks"]))
            {
                blockCode = Convert.ToInt32(Request.Params["Blocks"]);
            }
            var jsonData = new
            {
                rows = objStateLoginBAL.GetHabitationDetails(districtCode,blockCode,Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method is used to change the status of multiple habitations.
        /// </summary>
        /// <param name="HabCode"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult ChangeStatusOfHabitation(string HabCode)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            string encryptedHabCode = string.Empty;
            string habStatus = string.Empty;
            bool status = false;

            try
            {
           
                habStatus = Request.Params["Status"];

                if (objStateLoginDAL.ChangeStatusOfHabitation(HabCode, habStatus))
                {
                    message = "Habitation Status changed successfully.";
                    status = true;

                }
                else
                {
                    message = "Error occured while processing,Habitation status not changed.";
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Error occured while processing,Habitation status not changed.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method is used to change the status of multiple habitations.
        /// </summary>
        /// <param name="HabCode"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult ChangeStatusAsPerCensusYearOfHabitation(string HabCode)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            string encryptedHabCode = string.Empty;
            string habStatus = string.Empty;
            bool status = false;

            try
            {

                habStatus = Request.Params["Status"];

                if (objStateLoginDAL.ChangeStatusAsPerCensusYearOfHabitation(HabCode, habStatus))
                {
                    message = "Habitation Status changed successfully.";
                    status = true;

                }
                else
                {
                    message = "Error occured while processing,Habitation status not changed.";
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Error occured while processing,Habitation status not changed.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
     
        /// <summary>
        /// Method is used to get total habitation details by block code.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetHabitationDetails(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            String searchParameters = String.Empty;
            long totalRecords;
         
            int blockCode = 0;
            objStateLoginBAL = new HabitationConnectivityBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
           
            if (!string.IsNullOrEmpty(Request.Params["Blocks"]))
            {
                blockCode = Convert.ToInt32(Request.Params["Blocks"]);
            }
            var jsonData = new
            {
                rows = objStateLoginBAL.GetAllDetails_ByBlockCode(blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to get total habitation details.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetTotalHabsDetails(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            String searchParameters = String.Empty;
            long totalRecords;
          
            string Code = string.Empty;
            int blockCode = 0;
            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            objStateLoginBAL = new HabitationConnectivityBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["Blocks"]))
            {
                 Code = Request.Params["Blocks"];
            }
            encryptedParameters = Code.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
            
            var jsonData = new
            {
                rows = objStateLoginBAL.GetTotalHabsDetails(blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Audit]
        public ActionResult GetHabsDetailsByStatus(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            String searchParameters = String.Empty;
            long totalRecords;

            string Code = string.Empty;
            int blockCode = 0;
            string habStatus = string.Empty;
            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            objStateLoginBAL = new HabitationConnectivityBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["Blocks"]))
            {
                Code = Request.Params["Blocks"];
            }
            encryptedParameters = Code.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
            habStatus = Request.Params["HabStatus"];
            var jsonData = new
            {
                rows = objStateLoginBAL.GetHabsDetailsByStatusBAL(habStatus,blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditStatus(String parameter, String hash, String key)
        {
            bool status = false;
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int habCode = Convert.ToInt32(decryptedParameters["HabCode"].ToString());
                objStateLoginBAL = new HabitationConnectivityBAL();
                if (objStateLoginBAL.EditHabStatus(habCode, ref message))
                {
                    status = true;
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Error occured", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Error occured", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method is used to get connected habitation details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetConnectedHabsDetails(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            String searchParameters = String.Empty;
            long totalRecords;
         
            int blockCode = 0;
            string Code = string.Empty;
            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            objStateLoginBAL = new HabitationConnectivityBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["Blocks"]))
            {
                Code = Request.Params["Blocks"];
            }

            encryptedParameters = Code.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
            var jsonData = new
            {
                rows = objStateLoginBAL.GetConnectedHabsDetails(blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to get not connected habitation details.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetNotConnectedHabsDetails(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            String searchParameters = String.Empty;
            long totalRecords;
           
            int blockCode = 0;
            string Code = string.Empty;
            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            objStateLoginBAL = new HabitationConnectivityBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["Blocks"]))
            {
                Code = Request.Params["Blocks"];
            }

            encryptedParameters = Code.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
            var jsonData = new
            {
                rows = objStateLoginBAL.GetNotConnectedHabsDetails(blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method is used to get not feasible habitation details.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetNotFeasibleHabsDetails(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            String searchParameters = String.Empty;
            long totalRecords;
           
            int blockCode = 0;
            string Code = string.Empty;
            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            objStateLoginBAL = new HabitationConnectivityBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["Blocks"]))
            {
                Code = Request.Params["Blocks"];
            }

            encryptedParameters = Code.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
            var jsonData = new
            {
                rows = objStateLoginBAL.GetNotFeasibleHabsDetails(blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method is used to get state connected habitation details.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetStateConnectedHabsDetails(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            String searchParameters = String.Empty;
            long totalRecords;
           
            int blockCode = 0;
            string Code = string.Empty;
            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            objStateLoginBAL = new HabitationConnectivityBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["Blocks"]))
            {
                Code = Request.Params["Blocks"];
            }

            encryptedParameters = Code.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
            var jsonData = new
            {
                rows = objStateLoginBAL.GetStateConnectedHabsDetails(blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to get benifited habitation details.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetBenifitedHabsDetails(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            String searchParameters = String.Empty;
            long totalRecords;
         
            int blockCode = 0;
            string Code = string.Empty;
            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            objStateLoginBAL = new HabitationConnectivityBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["Blocks"]))
            {
                Code = Request.Params["Blocks"];
            }

            encryptedParameters = Code.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
            var jsonData = new
            {
                rows = objStateLoginBAL.GetBenifitedHabsDetails(blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



    }
    #endregion StateLoginController
}
