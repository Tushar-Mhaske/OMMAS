/*----------------------------------------------------------------------------------------
 * Project Id:

 * Project Name:OMMAS

 * File Name: MasterController.cs

 * Author : Vikram Nandanvar, Rohit Jadhav , Ashish Markande, Abhishek Kamble.

 * Creation Date :06/Apr/2013

 * Desc : This class is used as controller  to perform Save,Edit,Update,Delete and listing of master data  screens.  
 * ---------------------------------------------------------------------------------------*/
using PMGSY.BAL.Master;
using PMGSY.Models;
using PMGSY.Models.Master;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Common;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;
using PMGSY.Extensions;
using PMGSY.DAL.Master;
using PMGSY.DAL;
using PMGSY.BAL;
using System.Configuration;
using System.IO;
using System.Web.UI;
using PMGSY.Models.QualityMonitoring;
using System.Xml.Linq;



namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    [Audit]

    public class MasterController : Controller
    {
        private PMGSYEntities db = null;
        string message = String.Empty;
        private IMasterDAL objDAL = null;
        private IMasterBAL objBAL = null;
        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;
        int outParam = 0;
        CommonFunctions common = new CommonFunctions();
        #region MASTER_CDWORKS_TYPE

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Method for list of CDWorks 
        /// </summary>
        /// <param name="cdWorksCollection"></param>
        /// <returns>Json for populating the list</returns>

        [HttpPost]
        public ActionResult GetCDWorksList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {
                rows = objBAL.ListCdWorks(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords), //RegistredDocumentsDetails.GetSearchResultList(partyName, districtCode, villageCode, fromDate, toDate, Convert.ToInt32(homeFormCollection["page"]) - 1, Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// method for returning view of CDWorks type
        /// </summary>
        /// <returns>Partial view to the List page of CDWorks</returns>
        [HttpGet]
        public ActionResult AddCdWorksType()
        {
            return PartialView("AddCdWorksType", new CDWorksViewModel());
        }

        /// <summary>
        /// adding the CDWorks into the list. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Json containing success and message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCdWorksType(CDWorksViewModel model)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.AddCdWorks(model, ref message))
                    {
                        message = message == string.Empty ? "CD Works Length details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CD Works Length not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddCdWorksType", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "CD Works Length not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// method for returning the edit view with the particular data
        /// </summary>
        /// <param name="parameter">encrypted parameter of id</param>
        /// <param name="hash">encrypted hash of id</param>
        /// <param name="key">encrypted key of id</param>
        /// <returns></returns>
        [HttpGet]

        public ActionResult EditCdWorksType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    objBAL = new MasterBAL();
                    CDWorksViewModel model = objBAL.GetCdWorksDetails(Convert.ToInt32(decryptedParameters["CdWorksCode"].ToString()));

                    if (model == null)
                    {
                        ModelState.AddModelError(string.Empty, "CdWorks Length details not Exist.");
                        return PartialView("AddCdWorksType", new CDWorksViewModel());
                    }
                    return PartialView("AddCdWorksType", model);
                }
                return PartialView("AddCdWorksType", new CDWorksViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "CdWorks Length Details not Exist.");
                return PartialView("AddCdWorksType", new CDWorksViewModel());
            }
        }

        /// <summary>
        /// post action for updating the Contractor Data.
        /// </summary>
        /// <param name="model">contains the updated data.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCdWorksType(CDWorksViewModel model)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.EditCdWorks(model, ref message))
                    {
                        message = message == string.Empty ? "Cd Works Length details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Cd Works Length details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddCdWorksType", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Cd Works Length details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListWorksType()
        {
            return View();
        }

        /// <summary>
        /// delete contractor with the particular encrypted id
        /// </summary>
        /// <param name="parameter">encrypted parameter of id</param>
        /// <param name="hash">encrypted hash of id</param>
        /// <param name="key">encrypted key of id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteCdWorksType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    objBAL = new MasterBAL();
                    if (!objBAL.DeleteCdWorks(Convert.ToInt32(decryptedParameters["CdWorksCode"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "Cd Works Length details not deleted.");
                        return Json(new { success = false, message = "You can not delete this CD Works details." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Cd Work Length Details deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this CD Works Length details." }, JsonRequestBehavior.AllowGet);
            }
        }

        //required?

        public CDWorksViewModel CloneObject(MASTER_CDWORKS_TYPE master)
        {
            CDWorksViewModel model = new CDWorksViewModel();
            model.MAST_CDWORKS_CODE = master.MAST_CDWORKS_CODE;
            model.MAST_CDWORKS_NAME = master.MAST_CDWORKS_NAME;
            return model;
        }
        //required?

        public MASTER_CDWORKS_TYPE CloneModel(CDWorksViewModel model)
        {
            MASTER_CDWORKS_TYPE master = new MASTER_CDWORKS_TYPE();
            master.MAST_CDWORKS_CODE = model.MAST_CDWORKS_CODE;
            master.MAST_CDWORKS_NAME = model.MAST_CDWORKS_NAME;
            return master;
        }

        #endregion

        #region MASTER_ROAD_CATEGORY

        [HttpGet]
        public ActionResult ListRoadCategory()
        {
            return View();
        }

        /// <summary>
        /// sets data for populating the list of Road Category
        /// </summary>
        /// <param name="roadFormCollection">information about the form</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRoadCategoryList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {
                rows = objBAL.ListRoadCategory(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords), //RegistredDocumentsDetails.GetSearchResultList(partyName, districtCode, villageCode, fromDate, toDate, Convert.ToInt32(homeFormCollection["page"]) - 1, Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddRoadCategory()
        {
            return PartialView("AddRoadCategory", new RoadCategoryViewModel());
        }

        /// <summary>
        /// adds the data of Road Category
        /// </summary>
        /// <param name="model">data of Road Category</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRoadCategory(RoadCategoryViewModel model)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.AddRoadCategory(model, ref message))
                    {
                        message = message == string.Empty ? "Road Category details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Road Category details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddRoadCategory", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Road Category details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// sets the Raod category data for updation with particular encrypted id
        /// </summary>
        /// <param name="parameter">encrypted parameter for Road Category Id</param>
        /// <param name="hash">encrypted hash for Road Category Id</param>
        /// <param name="key">encrypted key for Road Category Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditRoadCategory(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    objBAL = new MasterBAL();
                    RoadCategoryViewModel model = objBAL.GetRoadDetails(Convert.ToInt32(decryptedParameters["RoadCode"].ToString()));

                    if (model == null)
                    {
                        ModelState.AddModelError(string.Empty, "Road Category details not exist.");
                        return PartialView("AddRoadCategory", new RoadCategoryViewModel());
                    }
                    return PartialView("AddRoadCategory", model);
                }
                return PartialView("AddRoadCategory", new RoadCategoryViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Road Category details not exist.");
                return PartialView("AddRoadCategory", new RoadCategoryViewModel());
            }
        }

        /// <summary>
        /// sets the updated data of Road Category
        /// </summary>
        /// <param name="model">updated data of Road Category</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditRoadCategory(RoadCategoryViewModel model)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.EditRoadCategory(model, ref message))
                    {
                        message = message == string.Empty ? "Road Category details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Road Category details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddRoadCategory", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Road Category details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// deletes the particular Road Category
        /// </summary>
        /// <param name="parameter">id</param>
        /// <param name="hash">id</param>
        /// <param name="key">id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteRoadCategory(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    objBAL = new MasterBAL();
                    if (!objBAL.DeleteRoadCategory(Convert.ToInt32(decryptedParameters["RoadCode"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "You can not delete this Road Category  details.");
                        return Json(new { success = false, message = "You can not delete this Road Category  details." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Road Category  details deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Road Category  details." }, JsonRequestBehavior.AllowGet);
            }
        }

        //required?

        public MASTER_ROAD_CATEGORY CloneModel(RoadCategoryViewModel model)
        {
            MASTER_ROAD_CATEGORY master = new MASTER_ROAD_CATEGORY();
            master.MAST_ROAD_CAT_CODE = model.MAST_ROAD_CAT_CODE;
            master.MAST_ROAD_CAT_NAME = model.MAST_ROAD_CAT_NAME;
            master.MAST_ROAD_SHORT_DESC = model.MAST_ROAD_SHORT_DESC;
            return master;
        }
        //required?

        public RoadCategoryViewModel CloneObject(MASTER_ROAD_CATEGORY master)
        {
            RoadCategoryViewModel model = new RoadCategoryViewModel();
            model.MAST_ROAD_CAT_CODE = master.MAST_ROAD_CAT_CODE;
            model.MAST_ROAD_CAT_NAME = master.MAST_ROAD_CAT_NAME;
            model.MAST_ROAD_SHORT_DESC = master.MAST_ROAD_SHORT_DESC;
            return model;
        }

        #endregion

        #region MASTER_SOIL_TYPE

        [HttpGet]
        public ActionResult ListSoilType()
        {
            return View();
        }

        /// <summary>
        /// sets the list of SoilType
        /// </summary>
        /// <param name="soilFormCollection">form containing the information of list</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSoilTypeList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {
                rows = objBAL.ListSoilType(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords), //RegistredDocumentsDetails.GetSearchResultList(partyName, districtCode, villageCode, fromDate, toDate, Convert.ToInt32(homeFormCollection["page"]) - 1, Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddSoilType()
        {
            return PartialView("AddSoilType", new SoilTypeViewModel());
        }


        /// <summary>
        /// returns data for updation with help of encrypted id
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditSoilType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int count = decryptParameters.Count();
                if (count > 0)
                {
                    SoilTypeViewModel model = objBAL.GetSoilDetails(Convert.ToInt32(decryptParameters["SoilCode"].ToString()));
                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Soil Type details not exist.");
                        return PartialView("AddSoilType", new SoilTypeViewModel());
                    }
                    return PartialView("AddSoilType", model);
                }
                return PartialView("AddSoilType", new SoilTypeViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Soil Type details not exist.");
                return PartialView("AddSoilType", new SoilTypeViewModel());
            }
        }


        /// <summary>
        /// adds the data into the list
        /// </summary>
        /// <param name="model">contains the inserted data</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSoilType(SoilTypeViewModel model)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.AddSoilType(model, ref message))
                    {
                        message = message == string.Empty ? "Soil Type details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Soil Type details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddSoilType", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Soil Type details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// udpates the data of particular id
        /// </summary>
        /// <param name="model">contains the updated data</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSoilType(SoilTypeViewModel model)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.EditSoilType(model, ref message))
                    {
                        message = message == string.Empty ? "Soil Type details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Soil Type details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddSoilType", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Soil Type details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// deletes the SoilType data with particular encrypted id
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteSoilType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteSoilType(Convert.ToInt32(decryptedParameters["SoilCode"].ToString())))
                    {
                        ModelState.AddModelError(String.Empty, "You can not delete this Soil Type details.");
                        return Json(new { success = false, message = "You can not delete this  Soil Type details." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Soil Type details deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this  Soil Type details." }, JsonRequestBehavior.AllowGet);
            }
        }

        //required?
        public MASTER_SOIL_TYPE CloneModel(SoilTypeViewModel model)
        {
            MASTER_SOIL_TYPE master = new MASTER_SOIL_TYPE();
            master.MAST_SOIL_TYPE_CODE = model.MAST_SOIL_TYPE_CODE;
            master.MAST_SOIL_TYPE_NAME = model.MAST_SOIL_TYPE_NAME;
            return master;
        }
        //requred?
        public SoilTypeViewModel CloneObject(MASTER_SOIL_TYPE master)
        {
            SoilTypeViewModel model = new SoilTypeViewModel();
            model.MAST_SOIL_TYPE_CODE = master.MAST_SOIL_TYPE_CODE;
            model.MAST_SOIL_TYPE_NAME = master.MAST_SOIL_TYPE_NAME;
            return model;
        }

        #endregion

        #region MASTER_FUNDING_AGENCY

        //-----------MASTER_FUNDING_AGENCY-------------//
        [HttpGet]
        public ActionResult ListFundingAgency()
        {
            return View();
        }

        /// <summary>
        /// sets the data for listing the Funding Agency
        /// </summary>
        /// <param name="fundCollection">form containing info abourt the list</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetFundingAgencyList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {
                rows = objBAL.ListFundingAgency(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddFundingAgency()
        {
            return View("AddFundingAgency", new FundingAgencyViewModel());
        }

        /// <summary>
        /// returns the data of particular encrypted id for updation
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditFundingAgency(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            objBAL = new MasterBAL();
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int count = decryptParameters.Count();
                if (count > 0)
                {
                    FundingAgencyViewModel model = objBAL.GetFundingAgencyDetails(Convert.ToInt32(decryptParameters["FundCode"].ToString()));
                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Funding Agency details not exist.");
                        return PartialView("AddFundingAgency", new FundingAgencyViewModel());
                    }
                    return PartialView("AddFundingAgency", model);
                }
                return PartialView("AddFundingAgency", new FundingAgencyViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Funding Agency details not exist.");
                return PartialView("AddFundingAgency", new FundingAgencyViewModel());
            }
        }

        /// <summary>
        /// adds the Funding Agency data into list
        /// </summary>
        /// <param name="model">contains the inserted data</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFundingAgency(FundingAgencyViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddFundingAgency(model, ref message))
                    {
                        message = message == string.Empty ? "Funding Agency details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Funding Agency details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddFundingAgency", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Funding Agency details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// sets the updated data into list
        /// </summary>
        /// <param name="model">contains the updated data</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFundingAgency(FundingAgencyViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditFundingAgency(model, ref message))
                    {
                        message = message == string.Empty ? "Funding Agency details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Funding Agency details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddFundingAgency", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Funding Agency details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// deletes the Funding agency details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteFundingAgency(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteFundingAgency(Convert.ToInt32(decryptedParameters["FundCode"].ToString())))
                    {
                        ModelState.AddModelError(String.Empty, "You can not delete this  Funding Agency details.");
                        return Json(new { success = false, message = "You can not delete this Funding Agency  details." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Funding Agency  details deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Funding Agency details." }, JsonRequestBehavior.AllowGet);
            }
        }

        public MASTER_FUNDING_AGENCY CloneModel(FundingAgencyViewModel model)
        {
            MASTER_FUNDING_AGENCY master = new MASTER_FUNDING_AGENCY();
            master.MAST_FUNDING_AGENCY_CODE = model.MAST_FUNDING_AGENCY_CODE;
            master.MAST_FUNDING_AGENCY_NAME = model.MAST_FUNDING_AGENCY_NAME;
            return master;
        }

        public FundingAgencyViewModel CloneObject(MASTER_FUNDING_AGENCY master)
        {
            FundingAgencyViewModel model = new FundingAgencyViewModel();
            model.MAST_FUNDING_AGENCY_CODE = master.MAST_FUNDING_AGENCY_CODE;
            model.MAST_FUNDING_AGENCY_NAME = master.MAST_FUNDING_AGENCY_NAME;
            return model;
        }

        #endregion

        #region MASTER_TRAFFIC_TYPE

        //-------------MASTER_TRAFFIC_TYPE----------------//

        [HttpGet]
        public ActionResult ListTrafficType()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchTrafficDetails()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> StatusList = new List<SelectListItem>();
                StatusList.Add(new SelectListItem { Text = "All", Value = "%" });
                StatusList.Add(new SelectListItem { Text = "Active", Value = "Y", Selected = true });
                StatusList.Add(new SelectListItem { Text = "Inactive", Value = "N" });
                ViewData["Status"] = StatusList;
                return PartialView("SearchTrafficDetails");

            }
            catch (Exception)
            {

                throw;
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
        /// sets the data for listing the Traffic Type
        /// </summary>
        /// <param name="trafficFormCollection">contains info about of list</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTrafficTypeList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            var jsonData = new
            {
                rows = objBAL.ListTrafficType(Request.Params["Status"], Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddTrafficType()
        {
            return PartialView("AddTrafficType", new TrafficTypeViewModel());
        }

        /// <summary>
        /// sets the data for updation of Traffic Type
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditTrafficType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    TrafficTypeViewModel model = objBAL.GetTrafficDetails(Convert.ToInt32(decryptedParameters["TrafficCode"].ToString()));

                    if (model == null)
                    {
                        ModelState.AddModelError(string.Empty, "Traffic details not exist.");
                        return PartialView("AddTrafficType", new TrafficTypeViewModel());
                    }
                    return PartialView("AddTrafficType", model);
                }
                return PartialView("AddTrafficType", new TrafficTypeViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Traffic details not exist.");
                return PartialView("AddTrafficType", new TrafficTypeViewModel());
            }
        }


        /// <summary>
        /// sets the updated Traffic Type data
        /// </summary>
        /// <param name="model">contains modified data</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTrafficType(TrafficTypeViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditTrafficType(model, ref message))
                    {
                        message = message == string.Empty ? "Traffic details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Traffic details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddTrafficType", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Traffic details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// adds the traffic type data
        /// </summary>
        /// <param name="model">contains the inserted traffic type data</param>
        /// <returns></returns>
        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult AddTrafficType(TrafficTypeViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddTrafficType(model, ref message))
                    {
                        message = message == string.Empty ? "Traffic details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Traffic details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddTrafficType", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Traffic details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// deletes the particular Traffic Type data
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteTrafficType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteTrafficType(Convert.ToInt32(decryptedParameters["TrafficCode"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "You can not delete this  Traffic  details.");
                        return Json(new { success = false, message = "You can not delete this  Traffic details." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Traffic  details deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Traffic details." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ChangeTrafficType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.ChangeTrafficType(Convert.ToInt32(decryptedParameters["TrafficCode"].ToString())))
                    {
                        ModelState.AddModelError(String.Empty, "Traffic Type Status not changed.");
                        return Json(new { success = false, message = "Traffic Type status not changed." });
                    }
                }
                //return View("ListTrafficType");
                return Json(new { success = true, message = "Traffic Type status changed successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                //return View("ListTrafficType");
                return Json(new { success = false, message = "Traffic Type status not changed." }, JsonRequestBehavior.AllowGet);
            }
        }

        public MASTER_TRAFFIC_TYPE CloneModel(TrafficTypeViewModel model)
        {
            MASTER_TRAFFIC_TYPE master = new MASTER_TRAFFIC_TYPE();
            master.MAST_TRAFFIC_CODE = model.MAST_TRAFFIC_CODE;
            master.MAST_TRAFFIC_NAME = model.MAST_TRAFFIC_NAME;
            master.MAST_TRAFFIC_STATUS = model.MAST_TRAFFIC_STATUS;
            return master;
        }

        public TrafficTypeViewModel CloneObject(MASTER_TRAFFIC_TYPE master)
        {
            TrafficTypeViewModel model = new TrafficTypeViewModel();
            master.MAST_TRAFFIC_CODE = model.MAST_TRAFFIC_CODE;
            master.MAST_TRAFFIC_NAME = model.MAST_TRAFFIC_NAME;
            master.MAST_TRAFFIC_STATUS = model.MAST_TRAFFIC_STATUS;
            return model;
        }

        #endregion

        #region MASTER_CDWORKS_TYPE_CONSTRUCTION

        //------------MASTER_CDWORKS_TYPE_CONSTRUCTION-----------------//

        /// <summary>
        /// Method to load list view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListCDWorksTypeConstruction()
        {
            return View();
        }

        /// <summary>
        /// Method to populate grid.
        /// </summary>
        /// <param name="constructionCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetConstructionTypeList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {
                rows = objBAL.ListConstructionType(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to load Add form.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddConstructionType()
        {
            return PartialView("AddConstructionType", new CDWorksConstructionViewModel());
        }

        /// <summary>
        /// Method to save CD works type.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddConstructionType(CDWorksConstructionViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddConstructionType(model, ref message))
                    {
                        message = message == string.Empty ? "CD Works Type details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CD Works Type details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddConstructionType", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "CD Works Type details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to set existing record into the page.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditConstructionType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    CDWorksConstructionViewModel model = objBAL.GetConstructionTypeDetails(Convert.ToInt32(decryptParameters["ConstructionCode"]));
                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "CD Works Type details not exist.");
                        return PartialView("AddConstructionType", new CDWorksConstructionViewModel());
                    }
                    return PartialView("AddConstructionType", model);
                }
                return PartialView("AddConstructionType", new CDWorksConstructionViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "CD Works Type details exist.");
                return PartialView("AddConstructionType", new CDWorksConstructionViewModel());
            }
        }

        /// <summary>
        /// Method to update the record.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConstructionType(CDWorksConstructionViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditConstructionType(model, ref message))
                    {
                        message = message == string.Empty ? "CD Works Type details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CD Works Type details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddConstructionType", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "CD Works Type details not updated.." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete the record.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteConstructionType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteConstructionType(Convert.ToInt32(decryptedParameters["ConstructionCode"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "You can not delete this CD Works Type details .");
                        return Json(new { success = false, message = "You can not delete this  CD Works Type details." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "CD Works Type details deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this CD Works Type details." }, JsonRequestBehavior.AllowGet);
            }
        }

        public MASTER_CDWORKS_TYPE_CONSTRUCTION CloneModel(CDWorksConstructionViewModel model)
        {
            MASTER_CDWORKS_TYPE_CONSTRUCTION master = new MASTER_CDWORKS_TYPE_CONSTRUCTION();
            master.MAST_CDWORKS_CODE = model.MAST_CDWORKS_CODE;
            master.MAST_CDWORKS_NAME = model.MAST_CDWORKS_NAME;
            return master;
        }

        public CDWorksConstructionViewModel CloneObject(MASTER_CDWORKS_TYPE_CONSTRUCTION master)
        {
            CDWorksConstructionViewModel model = new CDWorksConstructionViewModel();
            model.MAST_CDWORKS_CODE = master.MAST_CDWORKS_CODE;
            model.MAST_CDWORKS_NAME = master.MAST_CDWORKS_NAME;
            return model;
        }

        #endregion

        #region MASTER_CONTRACTOR

        //----------------MASTER_CONTRACTOR------------------------------//

        /// <summary>
        /// Method to load Add view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddContractor()
        {
            MasterContractorViewModel model = new MasterContractorViewModel();
            ViewBag.State = model.States;
            ViewBag.District = model.Districts;
            model.MAST_CON_STATUS = "A";
            return View(model);
        }

        /// <summary>
        /// Method to set existing record into the form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]

        public ActionResult EditContractor(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                PMGSYEntities dbContext = new PMGSYEntities();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MasterContractorViewModel model = objBAL.EditContractor(Convert.ToInt32(decryptParameters["ContractorCode"]));

                    //Code added by vikky[07/01/2022]]
                    Int32 conID = Convert.ToInt32(decryptParameters["ContractorCode"]);

                    //Below flags are added on 29-09-2022
                    bool flag_P = false;
                    bool flag_OD = false;

                    model.xmlRelaxationStatus = false;

                    AddXmlPanFile AddPan;
                    List<AddXmlPanFile> PanVal = new List<AddXmlPanFile>();
                    Int32 state_code_current = PMGSYSession.Current.StateCode;
                    if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/ContractorPANValidation.xml")))
                    {
                        try
                        {
                            XDocument doc = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/ContractorPANValidation.xml"));
                            foreach (XElement element in doc.Descendants("PANValidationExclusion").Descendants("Exclusion"))
                            {

                                AddPan = new AddXmlPanFile();

                                AddPan.state_Code = Convert.ToInt32(element.Element("StateCode").Value);
                                if (AddPan.state_Code == state_code_current)
                                {
                                    AddPan.Date_From = element.Element("DateFrom").Value;
                                    AddPan.Date_To = element.Element("DateTo").Value;
                                    AddPan.ModRequest = element.Element("ModRequest").Value;//Added on 29-09-2022
                                    PanVal.Add(AddPan);
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            if ((dbContext.REAT_CONTRACTOR_DETAILS.Any(m => m.MAST_CON_ID == conID && m.reat_STATUS == "A")) && model.xmlRelaxationStatus == false)
                            {
                                ModelState.AddModelError(String.Empty, "Contractor/Supplier details are used in REAT. Hence can not be modified.");
                                return View("AddContractor", new MasterContractorViewModel());
                            }
                        }
                    }

                    DateTime currentDate = DateTime.Now;
                    foreach (var item in PanVal)
                    {
                        DateTime datefrom = DateTime.ParseExact(item.Date_From, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime dateto = DateTime.ParseExact(item.Date_To, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        if ((currentDate >= datefrom && currentDate <= dateto) && state_code_current == item.state_Code)
                        {
                            model.xmlRelaxationStatus = true;
                            flag_OD = item.ModRequest == "P" ? true : false;//if Modification reuest for pan then keep other details disabled
                            flag_P = item.ModRequest == "OD" ? true : false;//if Modification reuest for other details then keep Pan disabled
                        }

                    }

                    if ((dbContext.REAT_CONTRACTOR_DETAILS.Any(m => m.MAST_CON_ID == conID && m.reat_STATUS == "A")) && model.xmlRelaxationStatus == false)
                    {
                        ModelState.AddModelError(String.Empty, "Contractor/Supplier details are used in REAT. Hence can not be modified.");
                        return View("AddContractor", new MasterContractorViewModel());
                    }

                    //code end here by vikky

                    //if (dbContext.REAT_CONTRACTOR_DETAILS.Any(m => m.MAST_CON_ID == conID && m.reat_STATUS != null && m.reat_STATUS == "A"))
                    //{
                    //    ModelState.AddModelError(String.Empty, "Contractor/Supplier details are used in REAT. Hence can not be modified.");
                    //    return View("AddContractor", new MasterContractorViewModel());
                    //}

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Contractor/Supplier details not exist.");
                        return View("AddContractor", new MasterContractorViewModel());
                    }
                    ViewBag.State = model.States;

                    ViewBag.District = model.Districts;

                    //Below lines are added on 29-09-2022
                    ViewBag.flag_P = flag_P;
                    ViewBag.flag_OD = flag_OD;

                    return View("AddContractor", model);
                }
                return View("AddContractor", new MasterContractorViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Contractor/Supplier details  not exist.");
                return View("AddContractor", new MasterContractorViewModel());
            }
        }

        /// <summary>
        /// Method to delete contractor details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteContractor(String parameter, String hash, String key)
        {
            //bool status = false;
            //string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            MasterContractorViewModel model = new MasterContractorViewModel();
            PMGSYEntities dbContext = new PMGSYEntities();
            ViewBag.District = model.Districts;
            ViewBag.State = model.States;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    Int32 ContractorID = Convert.ToInt32(decryptedParameters["ContractorCode"].ToString());
                    if (dbContext.REAT_CONTRACTOR_DETAILS.Any(m => m.MAST_CON_ID == ContractorID && m.reat_STATUS != null && m.reat_STATUS == "A"))
                    {
                        return Json(new { success = "false", message = "You can not delete this Contractor/Supplier details. These details are used in REAT." }, JsonRequestBehavior.AllowGet);
                    }


                    if (!objBAL.DeleteContractor(Convert.ToInt32(decryptedParameters["ContractorCode"].ToString())))
                    {
                        //ModelState.AddModelError(string.Empty, "Contractor Details not Deleted.");
                        return Json(new { success = "false", message = "You can not delete this Contractor/Supplier details." }, JsonRequestBehavior.AllowGet);
                    }
                }
                //return View("ListContractor");
                //status = true;
                return Json(new { success = "true", message = "Contractor/Supplier details deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                //return View("ListContractor");
                return Json(new { success = "false", message = "You can not delete this Contractor/Supplier details." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to load grid.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListContractor()
        {
            int stateCode;
            int districtCode;
            MasterContractorViewModel model = new MasterContractorViewModel();
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "",

            };
            SelectListItem contractor = new SelectListItem
            {
                Text = "Contractor",
                Value = "C",
                Selected = true
            };
            SelectListItem supplier = new SelectListItem
            {
                Text = "Supplier",
                Value = "S"
            };
            List<SelectListItem> contracSuppType = new List<SelectListItem>();
            contracSuppType.Add(all);
            contracSuppType.Add(contractor);
            contracSuppType.Add(supplier);
            // model.Status.Items.Add();
            List<SelectListItem> listStatus = new List<SelectListItem>();
            //listStatus.Insert(0, new SelectListItem { Value = "", Text = "All" });
            listStatus.Insert(0, new SelectListItem { Value = "A", Text = "Active", Selected = true });
            listStatus.Insert(1, new SelectListItem { Value = "I", Text = "Inactive" });
            listStatus.Insert(2, new SelectListItem { Value = "E", Text = "Expired" });
            listStatus.Insert(3, new SelectListItem { Value = "B", Text = "Blacklisted" });

            ViewData["ContractorSupplier"] = contracSuppType;
            objDAL = new MasterDAL();
            List<MASTER_STATE> lstState = objDAL.GetStates();
            lstState.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--All States--" });
            if (TempData["State"] != null)
            {
                stateCode = Convert.ToInt32(TempData["State"]);
                districtCode = Convert.ToInt32(TempData["District"]);
                model.MAST_STATE_CODE_ADDR = stateCode;
                model.MAST_DISTRICT_CODE_ADDR = districtCode;
                ViewBag.State = new SelectList(lstState, "MAST_STATE_CODE", "MAST_STATE_NAME", 1);
                ViewBag.District = model.Districts;
                ViewBag.Status = listStatus;
                return View();
            }
            else
            {

                ViewData["State"] = new SelectList(lstState, "MAST_STATE_CODE", "MAST_STATE_NAME", 1);

                //List<MASTER_DISTRICT> lstDistrict = objDAL.GetAllDistrictByStateCode(1); //new List<MASTER_DISTRICT>();
                //lstDistrict.Insert(0, new MASTER_DISTRICT { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "All Districts" });
                //ViewData["District"] = new SelectList(lstDistrict, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");



                //ViewBag.State = model.States;
                //ViewBag.District = model.Districts;
                //model.Status.Select(x => x.Value =="A");
                ViewBag.Status = listStatus;
                //List<SelectListItem> list = new List<SelectListItem>(); //= new SelectList(db.MASTER_CONTRACTOR.Where(m => m.MAST_STATE_CODE_ADDR == 21).ToList(), "MAST_CON_ID", "MAST_CON_PAN").ToList();
                //list.Insert(0, new SelectListItem { Value = "0", Text = "---Select---", Selected = true });
                return View();
            }
        }

        /// <summary>
        /// Method to get all districts.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult getDistricts(string id)
        {
            try
            {
                db = new PMGSYEntities();
                if (!int.TryParse(id, out outParam))
                {
                    return Json(false);
                }
                List<MASTER_DISTRICT> list = db.MASTER_DISTRICT.ToList();
                List<SelectListItem> list_dist = new List<SelectListItem>();
                list_dist.Insert(0, new SelectListItem() { Value = "0", Text = "All Districts", Selected = true });
                foreach (var item in list)
                {
                    if (item.MAST_STATE_CODE == (Convert.ToInt32(id)))
                    {
                        list_dist.Add(new SelectListItem { Value = item.MAST_DISTRICT_CODE.ToString(), Text = item.MAST_DISTRICT_NAME.ToString() });
                    }
                }
                return Json(list_dist);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }

        [HttpPost]
        public JsonResult GetList1(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            long totalRecords;
            int stateCode = 0;
            int districtCode = 0;
            string contractorName = string.Empty;
            string status = string.Empty;
            string panno = string.Empty;
            string contrSuppType = string.Empty;
            objBAL = new MasterBAL();
            try
            {


                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["searchField"]))
                {
                    searchParameters = HttpUtility.HtmlDecode(Request.Params["searchField"]);
                    searchParameters = searchParameters.Replace("%2F", "/");
                    string[] str = (searchParameters.ToString().Split('&'));
                    for (int i = 0; i < str.Length; ++i) //str.Length
                    {
                        string[] splitParameter = str[i].Split('=');
                        parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                    }

                    stateCode = Convert.ToInt16(parameters["State"]);
                    //   districtCode = Convert.ToInt16(parameters["District"]);
                    districtCode = 0;
                    //panCode = Convert.ToInt16(parameters["ddlPan"]);
                    status = parameters["Status"];
                    contractorName = parameters["Contractor"];
                    contrSuppType = parameters["ContractorSupplier"];
                    panno = parameters["PAN"];
                }


                var jsonData = new
                {
                    rows = objBAL.GetContractorList(stateCode.ToString(), districtCode.ToString(), contractorName, status, contrSuppType, panno, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, Request.Params["filters"]),
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

        /// <summary>
        /// Method to get pan numbers.
        /// </summary>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPanNumbers(string id1, string id2)
        {
            try
            {
                db = new PMGSYEntities();
                if (id2 == "0")
                {
                    int cont_id = Convert.ToInt32(id1);
                    List<SelectListItem> list = new SelectList(db.MASTER_CONTRACTOR.Where(m => m.MAST_STATE_CODE_ADDR == cont_id).ToList(), "MAST_CON_ID", "MAST_CON_PAN").ToList();
                    return Json(list);
                }
                else
                {
                    int cont_id = Convert.ToInt32(id2);
                    List<SelectListItem> list = new SelectList(db.MASTER_CONTRACTOR.Where(m => m.MAST_DISTRICT_CODE_ADDR == cont_id).ToList(), "MAST_CON_ID", "MAST_CON_PAN").ToList();
                    return Json(list);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }

        [HttpPost]
        public ActionResult GetStatePanNumbers(string id)
        {
            try
            {
                db = new PMGSYEntities();

                int cont_id = Convert.ToInt32(id);
                List<SelectListItem> list = new SelectList(db.MASTER_CONTRACTOR.Where(m => m.MAST_STATE_CODE_ADDR == cont_id).ToList(), "MAST_CON_ID", "MAST_CON_PAN").ToList();
                return Json(list);


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }


        /// <summary>
        /// Method to get registration details.
        /// </summary>
        /// <param name="regCollection"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DetailsRegistration(FormCollection regCollection, String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            long totalRecords;
            objBAL = new MasterBAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    int id = Convert.ToInt32(decryptedParameters["ContractorCode"]);
                    var jsonData = new
                    {
                        rows = objBAL.GetContractorRegistrationList(id, Convert.ToInt32(regCollection["page"]) - 1, Convert.ToInt32(regCollection["rows"]), regCollection["sidx"], regCollection["sord"], out totalRecords),
                        total = totalRecords <= Convert.ToInt32(regCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(regCollection["rows"]) + 1,
                        page = Convert.ToInt32(regCollection["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData);
                }
                return null;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        /// <summary>
        /// Method to add contractor details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddContractor(MasterContractorViewModel model)
        {
            ViewBag.District = model.Districts;
            ViewBag.State = model.States;
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (model.MAST_DISTRICT_CODE_ADDR == 0 || model.MAST_STATE_CODE_ADDR == 0)
                    {
                        message = "Please select district and state.";
                        status = false;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    if (objBAL.AddContractor(model, ref message))
                    {
                        message = message == string.Empty ? "Contractor/Supplier details saved successfully." : message;
                        status = true;
                        TempData["State"] = model.MAST_STATE_CODE_ADDR;
                        TempData["District"] = model.MAST_DISTRICT_CODE_ADDR;
                        TempData["Status"] = model.MAST_CON_STATUS;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Contractor/Supplier details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddContractor", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Contractor/Supplier details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to update contractor details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditContractor(MasterContractorViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditContractor(model, ref message))
                    {
                        message = message == string.Empty ? "Contractor/Supplier details updated successfully." : message;
                        status = true;
                        TempData["State"] = model.MAST_STATE_CODE_ADDR;
                        TempData["District"] = model.MAST_DISTRICT_CODE_ADDR;
                    }
                    else
                    {
                        message = message == string.Empty ? "Contractor/Supplier details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddContractor", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Contractor/Supplier details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// Method to search contractor.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchContractor()
        {
            return PartialView("SearchContractor");
        }

        /// <summary>
        /// Method to set existing record into the View form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewContractor(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MasterContractorViewModel model = objBAL.EditContractor(Convert.ToInt32(decryptParameters["ContractorCode"]));
                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Contractor/Supplier details not exist.");
                        return View("ViewContractor", new MasterContractorViewModel());
                    }
                    ViewBag.State = model.States;
                    ViewBag.District = model.Districts;
                    return View("ViewContractor", model);
                }
                return View("ViewContractor", new MasterContractorViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Contractor/Supplier details  not exist.");
                return View("ViewContractor", new MasterContractorViewModel());
            }
        }


        [HttpPost]
        public ActionResult SearchPanNumeberOfContractor(String strPannumber)
        {

            string message = string.Empty;
            try
            {
                objBAL = new MasterBAL();
                if (objBAL.PanNumberSearchExistBAL(strPannumber, ref message))
                {
                    return Json(new { success = "true", message = message == string.Empty ? "You can not search PAN this Contractor/Supplier details." : message }, JsonRequestBehavior.AllowGet);
                }


                return Json(new { success = "false", message = "You can not search PAN this successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = "false", message = "You can not Search Pan Number this Contractor/Supplier details." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region MASTER_DESIGNATION
        /// <summary>
        /// Method for to load designation list.
        /// </summary>
        /// <param name="desigCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDesignationList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            string desigCode = string.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["desigCode"]))
            {
                desigCode = Request.Params["desigCode"].ToString();
            }

            if (!string.IsNullOrEmpty(Request.Params["searchField"]))
            {
                searchParameters = HttpUtility.HtmlDecode(Request.Params["searchField"]);

                searchParameters = searchParameters.Replace("%2F", "/");
                string[] str = (searchParameters.ToString().Split('&'));
                for (int i = 0; i < str.Length; ++i) //str.Length
                {
                    string[] splitParameter = str[i].Split('=');
                    parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                }

                desigCode = parameters["MAST_DESIG_TYPE"];
            }

            var jsonData = new
            {
                rows = objBAL.GetDesignationList(desigCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to load list view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListMasterDesignation()
        {
            MasterDesignationViewModel model = new MasterDesignationViewModel();
            ViewBag.Designation = model.DesigType;
            return View();
        }

        /// <summary>
        /// Method to load Add view page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddDesignation()
        {
            return PartialView("AddDesignation", new MasterDesignationViewModel());
        }

        /// <summary>
        /// Method for search designation.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchDesignation()
        {
            MasterDesignationViewModel model = new MasterDesignationViewModel();
            ViewBag.Designation = model.DesigType;
            return PartialView("SearchDesignation");
        }

        /// <summary>
        /// Method to save designation details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDesignation(MasterDesignationViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddDesignation(model, ref message))
                    {
                        message = message == string.Empty ? "Designation details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Designation details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddDesignation", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Designation details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to set existing record into the page.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditDesignation(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MasterDesignationViewModel model = objBAL.GetDesignationDetails(Convert.ToInt32(decryptParameters["DesigCode"]).ToString());
                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Designation details not exist.");
                        return PartialView("AddDesignation", new MasterDesignationViewModel());
                    }
                    return PartialView("AddDesignation", model);
                }
                return PartialView("AddDesignation", new MasterDesignationViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Designation details not exist.");
                return PartialView("AddDesignation", new MasterDesignationViewModel());
            }
        }

        /// <summary>
        /// Method to update designation details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDesignation(MasterDesignationViewModel model)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.EditDesignation(model, ref message))
                    {
                        message = message == string.Empty ? "Designation details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Designation details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddDesignation", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Designation details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete designation details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult DeleteDesignation(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            string message = string.Empty;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteDesignation(Convert.ToInt32(decryptedParameters["DesigCode"])))
                    {
                        ModelState.AddModelError(string.Empty, "You can not delete this Designation details.");
                        return Json(new { success = false, message = "You can not delete this Designation details." }, JsonRequestBehavior.AllowGet);
                    }
                    status = true;
                    message = "Designation details deleted successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "You can not delete this Designation  details." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Designation  details." }, JsonRequestBehavior.AllowGet);
            }
        }



        #endregion

        #region MASTER_LOKSABHA_TERM

        /// <summary>
        /// Method to load list view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListLokSabhaTerm()
        {
            return View();
        }

        /// <summary>
        /// Method to load ADD view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddLokSabhaTerm()
        {
            db = new PMGSYEntities();
            MasterLokSabhaTermViewModel model = new MasterLokSabhaTermViewModel();


            model.MAST_LS_TERM = (db.MASTER_LOK_SABHA_TERM.Any() ? (from item in db.MASTER_LOK_SABHA_TERM select item.MAST_LS_TERM).Max() + 1 : 1);

            return PartialView("AddLokSabhaTerm", model);
        }

        /// <summary>
        /// Method to save loksabha term.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLokSabhaTerm(MasterLokSabhaTermViewModel model)
        {
            string date = Request.Params["MAST_LS_END_DATE"];
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddLokSabhaTerm(model, ref message))
                    {
                        message = message == string.Empty ? "Lok Sabha Term details saved successfully." : message;
                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Lok Sabha Term details not saved." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return PartialView("AddLokSabhaTerm", model);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Lok Sabha Term details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to set existing record into the form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditLokSabhaTerm(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {

                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int count = decryptParameters.Count();
                if (count > 0)
                {
                    MasterLokSabhaTermViewModel model = objBAL.GetLokSabhaTermDetails(Convert.ToInt32(decryptParameters["LokSabhaCode"].ToString()));

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Lok Sabha Term details not exist.");
                        return PartialView("AddLokSabhaTerm", new MasterLokSabhaTermViewModel());
                    }

                    return PartialView("AddLokSabhaTerm", model);
                }
                return PartialView("AddLokSabhaTerm", new MasterLokSabhaTermViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Lok Sabha Term details not exist.");
                return PartialView("AddLokSabhaTerm", new MasterLokSabhaTermViewModel());
            }
        }

        /// <summary>
        /// Method to update loksabha term.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLokSabhaTerm(MasterLokSabhaTermViewModel model)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.EditLokSabhaTerm(model, ref message))
                    {
                        message = message == string.Empty ? "Lok Sabha Term details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Lok Sabha Term details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddLokSabhaTerm", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Lok Sabha Term details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete loksabha term.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteLokSabhaTerm(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteLokSabhaTerm(Convert.ToInt32(decryptedParameters["LokSabhaCode"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "You can not delete this Lok Sabha Term details.");
                        return Json(new { success = false, message = "You can not delete this Lok Sabha Term details." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Lok Sabha Term details deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Lok Sabha Term details." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// mrthod to populate grid.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLokSabhaTermList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {

                rows = objBAL.GetLokSabhaTermList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region MASTER_MLA_MEMBERS

        /// <summary>
        /// Method to load list view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListMembers()
        {
            return View();
        }

        /// <summary>
        /// Method to load add view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMLAMembers()
        {
            return PartialView("AddEditMLAMembers", new MasterMLAMembersViewModel());
        }

        /// <summary>
        /// Method to populate the grid.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMemberList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int stateCode = 0;
            int constCode = 0;
            string memberName = string.Empty;
            int term = 0;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            objBAL = new MasterBAL();

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

            if (!string.IsNullOrEmpty(Request.Params["constituency"]))
            {
                constCode = Convert.ToInt32(Request.Params["constituency"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["memberName"]))
            {
                memberName = Request.Params["memberName"];
            }
            if (!string.IsNullOrEmpty(Request.Params["term"]))
            {
                term = Convert.ToInt32(Request.Params["term"]);
            }

            var jsonData = new
            {
                rows = objBAL.ListMLAMembers(stateCode, term, constCode, memberName, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to set existing record into the form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMLAMembers(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MasterMLAMembersViewModel model = objBAL.GetMemberDetails(Convert.ToInt32(decryptParameters["MemberCode"]));
                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "MLA Member details not exist.");
                        return PartialView("AddEditMLAMembers", new MasterMLAMembersViewModel());
                    }
                    ViewBag.Constituency = model.MAST_MLA_CONST_CODE;
                    ViewBag.VidhanSabhaTerm = model.MAST_VS_TERM;
                    return PartialView("AddEditMLAMembers", model);
                }
                return PartialView("AddEditMLAMembers", new MasterMLAMembersViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "MLA Member details not exist.");
                return PartialView("AddEditMLAMembers", new MasterMLAMembersViewModel());
            }
        }

        /// <summary>
        /// Method to save mla member details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditMLAMembers(MasterMLAMembersViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMLAMembers(model, ref message))
                    {
                        message = message == string.Empty ? "MLA Member details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "MLA Member details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMLAMembers", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "MLA Member details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to update mla member details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMLAMembers(MasterMLAMembersViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMLAMembers(model, ref message))
                    {
                        message = message == string.Empty ? "MLA Member details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "MLA Member details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMLAMembers", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "MLA Member details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete mla member details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMLAMember(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteMLAMembers(Convert.ToInt32(decryptedParameters["MemberCode"].ToString())))
                    {
                        ModelState.AddModelError(String.Empty, "You can not delete this MLA Member details.");
                        return Json(new { success = false, message = "You can not delete this MLA Member details." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "MLA Member details deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this MLA Member details." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to get term list by state code.
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetTermList_ByStateCode(string stateCode)
        {
            try
            {
                db = new PMGSYEntities();
                if (!int.TryParse(stateCode, out outParam))
                {
                    return Json(false);
                }
                int code = Convert.ToInt32(stateCode);
                var list = (from vs in db.MASTER_VIDHAN_SABHA_TERM
                            where
                            (code == 0 ? 1 : vs.MAST_STATE_CODE) == (code == 0 ? 1 : code)
                            select new
                            {
                                MAST_VS_TERM = vs.MAST_VS_TERM,
                                MAST_VS_NAME = vs.MAST_VS_TERM
                            }).Distinct().OrderByDescending(m => m.MAST_VS_TERM).ToList();




                return Json(new SelectList(list, "MAST_VS_TERM", "MAST_VS_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }

        }

        /// <summary>
        /// method to get constituency list by state code.
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetConstituencyList_ByStateCode(string stateCode)
        {
            db = new PMGSYEntities();
            int code = Convert.ToInt32(stateCode);
            List<MASTER_MLA_CONSTITUENCY> list = db.MASTER_MLA_CONSTITUENCY.Where(m => m.MAST_STATE_CODE == code && m.MAST_MLA_CONST_ACTIVE == "Y").ToList<MASTER_MLA_CONSTITUENCY>();
            list.Insert(0, new MASTER_MLA_CONSTITUENCY { MAST_MLA_CONST_CODE = 0, MAST_MLA_CONST_NAME = "All Constituencies" });
            return Json(new SelectList(list, "MAST_MLA_CONST_CODE", "MAST_MLA_CONST_NAME"));
        }

        /// <summary>
        /// method to get term list
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetTermList(string stateCode)
        {
            db = new PMGSYEntities();
            if (!int.TryParse(stateCode, out outParam))
            {
                return Json(false);
            }
            int code = Convert.ToInt32(stateCode);
            List<MASTER_VIDHAN_SABHA_TERM> list = db.MASTER_VIDHAN_SABHA_TERM.Where(m => m.MAST_STATE_CODE == code).ToList<MASTER_VIDHAN_SABHA_TERM>();
            return Json(new SelectList(list, "MAST_VS_TERM", "MAST_VS_TERM"));

        }

        /// <summary>
        /// Method to get constituency list.
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetConstituencyList(string stateCode)
        {
            db = new PMGSYEntities();
            if (!int.TryParse(stateCode, out outParam))
            {
                return Json(false);
            }
            int code = Convert.ToInt32(stateCode);
            List<MASTER_MLA_CONSTITUENCY> list = db.MASTER_MLA_CONSTITUENCY.Where(m => m.MAST_STATE_CODE == code).ToList<MASTER_MLA_CONSTITUENCY>();
            list.Insert(0, new MASTER_MLA_CONSTITUENCY { MAST_MLA_CONST_CODE = 0, MAST_MLA_CONST_NAME = "--Select--" });
            return Json(new SelectList(list, "MAST_MLA_CONST_CODE", "MAST_MLA_CONST_NAME"));
        }

        /// <summary>
        /// Method to search mla member.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchMLAMember()
        {
            objDAL = new MasterDAL();
            db = new PMGSYEntities();
            List<MASTER_STATE> lstState = objDAL.GetAllStates();
            lstState.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" });
            ViewData["State"] = new SelectList(lstState, "MAST_STATE_CODE", "MAST_STATE_NAME");

            List<MASTER_MLA_CONSTITUENCY> lstConstituency = new List<MASTER_MLA_CONSTITUENCY>();
            if (PMGSYSession.Current.StateCode > 0)
            {
                lstConstituency = db.MASTER_MLA_CONSTITUENCY.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode).ToList<MASTER_MLA_CONSTITUENCY>();
            }
            lstConstituency.Insert(0, new MASTER_MLA_CONSTITUENCY { MAST_MLA_CONST_CODE = 0, MAST_MLA_CONST_NAME = "All Constituencies" });
            ViewData["Constituency"] = new SelectList(lstConstituency.Where(z => z.MAST_MLA_CONST_ACTIVE == "Y").ToList(), "MAST_MLA_CONST_CODE", "MAST_MLA_CONST_NAME");

            //List<MASTER_MLA_MEMBERS> lstTerm = new List<MASTER_MLA_MEMBERS>();
            //lstTerm.Insert(0, new MASTER_MLA_MEMBERS { MAST_VS_TERM = 0, MAST_VS_TERMs = "All " });
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem
            {
                Text = "All Term",
                Value = "0"
            });
            ViewData["Term"] = list;

            return PartialView("SearchMLAMember");
        }

        /// <summary>
        /// Method to get member by state code.
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMembersByStateCode(string stateCode)
        {
            try
            {
                if (!int.TryParse(stateCode, out outParam))
                {
                    return Json(false);
                }
                List<Models.MASTER_MLA_MEMBERS> memberList = new List<Models.MASTER_MLA_MEMBERS>();

                objDAL = new MasterDAL();

                memberList = objDAL.GetMembersByStateCode(Convert.ToInt32(stateCode.Trim()));

                return Json(new SelectList(memberList, "MAST_MEMBER_ID", "MAST_MEMBER"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        #endregion

        #region ADMIN_DEPARTMENT

        /// <summary>
        /// Method to load list view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListAdminDepartment()
        {
            return View();
        }

        /// <summary>
        /// Method to serach admin departments.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchAdminDepartment()
        {
            objDAL = new MasterDAL();
            AdminDepartmentViewModel model = new AdminDepartmentViewModel();
            CommonFunctions objCommon = new CommonFunctions();
            List<MASTER_STATE> lstState = objDAL.GetAllStates();
            lstState.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" });
            ViewData["State"] = new SelectList(lstState, "MAST_STATE_CODE", "MAST_STATE_NAME");

            if (PMGSYSession.Current.RoleCode != 47 && PMGSYSession.Current.RoleCode != 36) //ITNO=36 and ITNOOA=47
            {
                List<MASTER_AGENCY> lstAgency = objDAL.GetAgencyNames();
                lstAgency.Insert(0, new MASTER_AGENCY { MAST_AGENCY_CODE = 0, MAST_AGENCY_NAME = "All Agencies" });
                ViewData["Agency"] = new SelectList(lstAgency, "MAST_AGENCY_CODE", "MAST_AGENCY_NAME");
            }
            else
            {
                List<SelectListItem> lstAgencyDeptwise = new List<SelectListItem>();
                lstAgencyDeptwise = objCommon.PopulateAgenciesByStateAndDepartmentwise(PMGSYSession.Current.StateCode, PMGSYSession.Current.AdminNdCode, true);
                lstAgencyDeptwise.RemoveAt(0);
                ViewData["Agency"] = new SelectList(lstAgencyDeptwise, "Value", "Text");
            }

            return PartialView("SearchAdminDepartment");
        }

        /// <summary>
        /// Method to get department list.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDepartmentList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int stateCode = 0;
            // int districtCode = 0;
            int agencyCode = 0;
            string adminName = string.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            objBAL = new MasterBAL();
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

            if (!string.IsNullOrEmpty(Request.Params["agency"]))
            {
                agencyCode = Convert.ToInt32(Request.Params["agency"]);
            }

            var jsonData = new
            {
                rows = objBAL.ListAdminDepartmentList(stateCode, agencyCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                //districtCode, departmentName, changes by Koustubh Nakate on 16-05-2013
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to load add view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditAdminDepartment()
        {
            objBAL = new MasterBAL();
            AdminDepartmentViewModel model = objBAL.AddStateAdmin();
            return PartialView("AddEditAdminDepartment", model);
        }

        /// <summary>
        /// Method to set existing record into  the page.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditAdminDepartment(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    AdminDepartmentViewModel model = objBAL.GetAdminDetails(Convert.ToInt32(decryptParameters["AdminCode"]));

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "SRRDA/DPIU details not exist.");
                        return PartialView("AddEditAdminDepartment", new AdminDepartmentViewModel());
                    }

                    model.ADMIN_ND_CODE = Convert.ToInt32(decryptParameters["AdminCode"]);      //change by Ujjwal Saket on25-10-2013 for populating mapped districts of Other Agency
                    return PartialView("AddEditAdminDepartment", model);
                }
                return PartialView("AddEditAdminDepartment", new AdminDepartmentViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "SRRDA/DPIU details not exist.");
                return PartialView("AddEditAdminDepartment", new AdminDepartmentViewModel());
            }
        }

        /// <summary>
        /// method to save admin department details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditAdminDepartment(AdminDepartmentViewModel model)
        {
            bool status = false;
            try
            {
                /*if (model.MAST_ND_TYPE == "S")
                {
                    ModelState.Remove("ADMIN_BA_ENABLE_DATE");
                    ModelState.Remove("ADMIN_EPAY_ENABLE_DATE");
                    ModelState.Remove("ADMIN_EREMIT_ENABLED_DATE");
                }*/
                if (model.MAST_ND_TYPE == "S")
                {
                    ModelState["MAST_PARENT_ND_CODE"].Errors.Clear();
                }
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddAdminDepartment(model, ref message))
                    {
                        if (model.MAST_ND_TYPE.Equals("S"))
                        {
                            message = message == string.Empty ? "SRRDA details saved successfully." : message;
                            status = true;
                        }
                        else if (model.MAST_ND_TYPE.Equals("D"))
                        {
                            message = message == string.Empty ? "DPIU details saved successfully." : message;
                            status = true;

                        }

                    }
                    else
                    {
                        message = message == string.Empty ? "SRRDA/DPIU details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditAdminDepartment", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "SRRDA/DPIU details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to update admin department details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAdminDepartment(AdminDepartmentViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditAdminDepartment(model, ref message))
                    {

                        if (model.MAST_ND_TYPE.Equals("S"))
                        {
                            message = message == string.Empty ? "SRRDA details updated successfully." : message;
                            status = true;
                        }
                        else if (model.MAST_ND_TYPE.Equals("D"))
                        {
                            message = message == string.Empty ? "DPIU details updated successfully." : message;
                            status = true;

                        }

                    }
                    else
                    {
                        message = message == string.Empty ? "SRRDA/DPIU details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditAdminDepartment", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "SRRDA/DPIU details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete admin department details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteAdminDepartment(String parameter, String hash, String key)
        {
            objBAL = new MasterBAL();
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteAdminDepartment(Convert.ToInt32(decryptedParameters["AdminCode"].ToString()), ref message))
                    {
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this SRRDA/DPIU details." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this SRRDA/DPIU details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this SRRDA/DPIU details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public ActionResult AddDistrictUnitDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            AdminDepartmentViewModel adminDepartmentViewModel = new AdminDepartmentViewModel();
            db = new PMGSYEntities();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int parentCode = Convert.ToInt32(decryptedParameters["AdminCode"]);
                ADMIN_DEPARTMENT admin = db.ADMIN_DEPARTMENT.Find(parentCode);

                adminDepartmentViewModel.MAST_ND_TYPE = "D";
                // model.MAST_PARENT_ND_CODE = admin.MAST_PARENT_ND_CODE;
                adminDepartmentViewModel.MAST_PARENT_ND_CODE = admin.ADMIN_ND_CODE;
                adminDepartmentViewModel.ADMIN_ND_CODE = parentCode;        //change by ujjwal saket on25-10-2013 for populating districts
                adminDepartmentViewModel.MAST_STATE_CODE = admin.MAST_STATE_CODE;
                adminDepartmentViewModel.MAST_AGENCY_CODE = admin.MAST_AGENCY_CODE;
                return PartialView("AddEditAdminDepartment", adminDepartmentViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("AddEditAdminDepartment", adminDepartmentViewModel);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }

        }

        /// <summary>
        /// Method to get districts by state code.
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
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

                MasterDAL masterDataEntryDAL = new MasterDAL();

                districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(Convert.ToInt32(stateCode.Trim()));

                districtList.Insert(0, new MASTER_DISTRICT { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "All Districts" });

                return Json(new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetDistrictsByStateCode

        /// <summary>
        /// Method to get district list.
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDistrictsList(string stateCode)
        {
            try
            {
                List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();

                MasterDAL masterDataEntryDAL = new MasterDAL();

                districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(Convert.ToInt32(stateCode.Trim()));

                districtList.Insert(0, new MASTER_DISTRICT { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });

                return Json(new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }


        /// <summary>
        /// Method to set existing record into  the View page.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewAdminDepartment(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    AdminDepartmentViewModel model = objBAL.GetAdminDetails(Convert.ToInt32(decryptParameters["AdminCode"]));

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "SRRDA/DPIU details not exist.");
                        return PartialView("ViewAdminDepartment", new AdminDepartmentViewModel());
                    }

                    model.ADMIN_ND_CODE = Convert.ToInt32(decryptParameters["AdminCode"]);      //change by Ujjwal Saket on25-10-2013 for populating mapped districts of Other Agency
                    return PartialView("ViewAdminDepartment", model);
                }
                return PartialView("ViewAdminDepartment", new AdminDepartmentViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "SRRDA/DPIU details not exist.");
                return PartialView("ViewAdminDepartment", new AdminDepartmentViewModel());
            }
        }

        #endregion

        #region PIU Departement
        [HttpGet]
        public ActionResult ListPIUDepartment()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchPIUDepartment()
        {
            objDAL = new MasterDAL();
            MasterDAL masterDataEntryDAL = new MasterDAL();
            AdminDepartmentViewModel model = new AdminDepartmentViewModel();
            int NDCode = 0;
            List<MASTER_STATE> lstState = objDAL.GetAllStates();
            // lstState.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" });           
            ViewData["State"] = new SelectList(lstState, "MAST_STATE_CODE", "MAST_STATE_NAME", PMGSYSession.Current.StateCode == 0 ? 1 : PMGSYSession.Current.StateCode);
            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList.Add(new SelectListItem { Text = "All", Value = "%" });
            StatusList.Add(new SelectListItem { Text = "Yes", Value = "Y", Selected = true });
            StatusList.Add(new SelectListItem { Text = "No", Value = "N" });
            ViewData["ddlActive"] = StatusList;
            List<SelectListItem> lstAgency = new List<SelectListItem>();
            List<SelectListItem> SSRDAList = new List<SelectListItem>();

            PMGSYEntities dbContext = new PMGSYEntities();
            if (PMGSYSession.Current.StateCode > 0)
            {
                var list = from admin in dbContext.ADMIN_DEPARTMENT
                           join state in dbContext.MASTER_STATE on admin.MAST_STATE_CODE equals state.MAST_STATE_CODE
                           //join district in dbContext.MASTER_DISTRICT on admin.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE into newDistrict
                           //from district in newDistrict.DefaultIfEmpty()
                           join agency in dbContext.MASTER_AGENCY on admin.MAST_AGENCY_CODE equals agency.MAST_AGENCY_CODE
                           where
                            admin.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode &&
                            admin.MAST_STATE_CODE == PMGSYSession.Current.StateCode
                           select new
                           {
                               admin.ADMIN_ND_CODE,
                               state.MAST_STATE_NAME,
                               agency.MAST_AGENCY_NAME,
                               admin.MAST_PARENT_ND_CODE,
                               admin.MAST_ND_TYPE,
                               admin.ADMIN_ND_NAME,
                               admin.MAST_STATE_CODE,
                               admin.MAST_AGENCY_CODE
                           };
                int agencyCode = list.Select(a => a.MAST_AGENCY_CODE).FirstOrDefault();
                lstAgency = masterDataEntryDAL.PopulateAgencies(PMGSYSession.Current.StateCode, true);
                lstAgency.Find(x => x.Value == agencyCode.ToString()).Selected = true;
                NDCode = list.Select(a => a.ADMIN_ND_CODE).FirstOrDefault();
                SSRDAList = new SelectList(masterDataEntryDAL.GetSSRDAByState(PMGSYSession.Current.StateCode).ToList(), "ADMIN_ND_CODE", "ADMIN_ND_NAME").ToList();
                SSRDAList.RemoveAt(0);
            }
            else
            {
                lstAgency.Insert(0, (new SelectListItem { Text = "All Agencies", Value = "0", Selected = true }));
                SSRDAList = new SelectList(masterDataEntryDAL.GetSSRDAByState(PMGSYSession.Current.StateCode == 0 ? 1 : PMGSYSession.Current.StateCode).ToList(), "ADMIN_ND_CODE", "ADMIN_ND_NAME").ToList();
                SSRDAList.RemoveAt(0);

            }
            ViewData["Agency"] = lstAgency;
            ViewData["ddlSSRDA"] = new SelectList(SSRDAList, "Value", "Text", NDCode == 0 ? 1 : NDCode);

            return PartialView("SearchPIUDepartment");
        }


        [HttpPost]
        public ActionResult GetPIUDepartmentList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int stateCode = 0;
            // int districtCode = 0;
            int agencyCode = 0;
            int adminNDCode = 0;
            string activeflag = "Y";
            string adminName = string.Empty;

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            objBAL = new MasterBAL();
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

            if (!string.IsNullOrEmpty(Request.Params["agency"]))
            {
                agencyCode = Convert.ToInt32(Request.Params["agency"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["adminNDCode"]))
            {
                adminNDCode = Convert.ToInt32(Request.Params["adminNDCode"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["active"]))
            {
                activeflag = Request.Params["active"];
            }
            var jsonData = new
            {
                rows = objBAL.GetDPIUListBAL(stateCode, agencyCode, adminNDCode, activeflag, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                //districtCode, departmentName, changes by Koustubh Nakate on 16-05-2013
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Method to load add view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditPIUDepartment()
        {
            objBAL = new MasterBAL();
            AdminDepartmentViewModel model = new AdminDepartmentViewModel();
            model.MAST_ND_TYPE = "D";
            model.MAST_PARENT_ND_CODE_List = new List<SelectListItem>();
            model.MAST_PARENT_ND_CODE_List.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
            MasterDAL masterDataEntryDAL = new MasterDAL();

            if (PMGSYSession.Current.StateCode > 0)
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                var list = from admin in dbContext.ADMIN_DEPARTMENT
                           join state in dbContext.MASTER_STATE on admin.MAST_STATE_CODE equals state.MAST_STATE_CODE
                           //join district in dbContext.MASTER_DISTRICT on admin.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE into newDistrict
                           //from district in newDistrict.DefaultIfEmpty()
                           join agency in dbContext.MASTER_AGENCY on admin.MAST_AGENCY_CODE equals agency.MAST_AGENCY_CODE
                           where
                            admin.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode &&
                            admin.MAST_STATE_CODE == PMGSYSession.Current.StateCode
                           select new
                           {
                               admin.ADMIN_ND_CODE,
                               state.MAST_STATE_NAME,
                               agency.MAST_AGENCY_NAME,
                               admin.MAST_PARENT_ND_CODE,
                               admin.MAST_ND_TYPE,
                               admin.ADMIN_ND_NAME,
                               admin.MAST_STATE_CODE,
                               admin.MAST_AGENCY_CODE
                           };
                model.Mast_Parent_ND_Name = list.Select(a => a.ADMIN_ND_NAME).FirstOrDefault();
                model.STATE_NAME = list.Select(a => a.MAST_STATE_NAME).FirstOrDefault();
                model.Agency_Name = list.Select(a => a.MAST_AGENCY_NAME).FirstOrDefault();
                model.MAST_PARENT_ND_CODE = list.Select(a => a.ADMIN_ND_CODE).FirstOrDefault();
                model.MAST_STATE_CODE = list.Select(a => a.MAST_STATE_CODE).FirstOrDefault();
                model.MAST_AGENCY_CODE = list.Select(a => a.MAST_AGENCY_CODE).FirstOrDefault();


                //List<SelectListItem> agencyList = new List<SelectListItem>();
                //ViewData["Agency"] = agencyList = masterDataEntryDAL.PopulateAgencies(model.MAST_STATE_CODE, false);


            }
            else
            {
                //model.MAST_PARENT_ND_CODE_List.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
            }

            return PartialView("AddEditPIUDepartment", model);
        }

        /// <summary>
        /// method to save admin department details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditPIUDepartment(AdminDepartmentViewModel model)
        {
            bool status = false;
            try
            {

                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddAdminDepartment(model, ref message))
                    {
                        message = message == string.Empty ? "DPIU details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "DPIU details not saved." : message;
                    }
                }
                else
                {
                    //model.MAST_PARENT_ND_CODE_List = new List<SelectListItem>();
                    //model.MAST_PARENT_ND_CODE_List.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
                    //return PartialView("AddEditPIUDepartment", model);
                    return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "DPIU details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to Get PIU department details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditPIUDepartment(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                MasterDAL masterDataEntryDAL = new MasterDAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    AdminDepartmentViewModel model = objBAL.GetAdminDetails(Convert.ToInt32(decryptParameters["AdminCode"]));

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "DPIU details not exist.");
                        return PartialView("AddEditPIUDepartment", new AdminDepartmentViewModel());
                    }

                    model.ADMIN_ND_CODE = Convert.ToInt32(decryptParameters["AdminCode"]);      //change by Ujjwal Saket on25-10-2013 for populating mapped districts of Other Agency
                    model.MAST_PARENT_ND_CODE_List = new List<SelectListItem>();
                    model.MAST_PARENT_ND_CODE_List = new SelectList(masterDataEntryDAL.GetSSRDAByStateAgencyCode(model.MAST_STATE_CODE, model.MAST_AGENCY_CODE, false).ToList(), "ADMIN_ND_CODE", "ADMIN_ND_NAME").ToList();

                    return PartialView("AddEditPIUDepartment", model);
                }

                return PartialView("AddEditPIUDepartment", new AdminDepartmentViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "DPIU details not exist.");
                return PartialView("AddEditPIUDepartment", new AdminDepartmentViewModel());
            }
        }

        /// <summary>
        /// Method to update PIU department details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPIUDepartment(AdminDepartmentViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditAdminDepartment(model, ref message))
                    {
                        message = message == string.Empty ? "DPIU details updated successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "DPIU details not updated." : message;
                    }
                }
                else
                {
                    //model.MAST_PARENT_ND_CODE_List = new List<SelectListItem>();
                    // model.MAST_PARENT_ND_CODE_List.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));

                    // return PartialView("AddEditPIUDepartment", model);               
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "DPIU details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete admin department details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePIUDepartment(String parameter, String hash, String key)
        {
            objBAL = new MasterBAL();
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteAdminDepartment(Convert.ToInt32(decryptedParameters["AdminCode"].ToString()), ref message))
                    {
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this DPIU details." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this DPIU details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this DPIU details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }



        [HttpPost]
        public JsonResult GetAgencyListByState(string stateCode, bool IsAllSelected)
        {
            try
            {
                List<SelectListItem> agencyList = new List<SelectListItem>();
                MasterDAL masterDataEntryDAL = new MasterDAL();
                agencyList = masterDataEntryDAL.PopulateAgencies(Convert.ToInt32(stateCode.Trim()), IsAllSelected);

                return Json(new SelectList(agencyList, "Value", "Text"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        /// <summary>
        ///  used Method in Add Edit Scrren to get SRRDA Dropdown list.
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSSRDAList(string stateCode, string agencyCode)
        {
            try
            {
                List<Models.ADMIN_DEPARTMENT> ssrdaList = new List<Models.ADMIN_DEPARTMENT>();

                MasterDAL masterDataEntryDAL = new MasterDAL();

                ssrdaList = masterDataEntryDAL.GetSSRDAByStateAgencyCode(Convert.ToInt32(stateCode.Trim()), Convert.ToInt32(agencyCode.Trim()), false);

                //ssrdaList.Insert(0, new ADMIN_DEPARTMENT { ADMIN_ND_CODE = 0, ADMIN_ND_NAME = "--Select--" });

                return Json(new SelectList(ssrdaList, "ADMIN_ND_CODE", "ADMIN_ND_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        /// <summary>
        ///use  Method in Search screen to get SRRDA Dropdown list.
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSSRDAListByState(string stateCode, bool IsAllSelected)
        {
            try
            {
                List<Models.ADMIN_DEPARTMENT> ssrdaList = new List<Models.ADMIN_DEPARTMENT>();

                MasterDAL masterDataEntryDAL = new MasterDAL();

                ssrdaList = masterDataEntryDAL.GetSSRDAByState(Convert.ToInt32(stateCode.Trim()), false);
                ssrdaList.RemoveAt(0);
                //ssrdaList.Insert(0, new ADMIN_DEPARTMENT { ADMIN_ND_CODE = 0, ADMIN_ND_NAME = "--Select--" });

                return Json(new SelectList(ssrdaList, "ADMIN_ND_CODE", "ADMIN_ND_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }
        [HttpPost]
        public ActionResult GetDPIUList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            int adminNDCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                adminNDCode = Convert.ToInt32(Request.Params["AdminNDCode"]);
                objBAL = new MasterBAL();
                var jsonData = new
                {
                    rows = objBAL.GetDPIUListBAL_ByAdminNDCode(adminNDCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        /// <summary>
        /// Method to get district list.
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDistrictsListByAdminNDCode(string stateCode, string AdminNdCode)
        {
            try
            {
                //List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();

                //MasterDAL masterDataEntryDAL = new MasterDAL();

                //districtList = masterDataEntryDAL.GetAllDistrictsByAdminNDCode(Convert.ToInt32(stateCode.Trim()), Convert.ToInt32(AdminNdCode.Trim()));

                //districtList.Insert(0, new MASTER_DISTRICT { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });

                //return Json(new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME"));
                CommonFunctions objCommonFuntion = new CommonFunctions();
                List<SelectListItem> districtList = new List<SelectListItem>();
                districtList = objCommonFuntion.GetAllDistrictsByAdminNDCode(Convert.ToInt32(stateCode.Trim()), Convert.ToInt32(AdminNdCode.Trim()));
                districtList.Insert(0, new SelectListItem { Value = "0", Text = "--Select District--" });
                return Json(new SelectList(districtList, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        #endregion

        #region MASTER_QUALIFICATION

        /// <summary>
        /// List view method.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterQual()
        {
            return View();
        }

        /// <summary>
        /// Method for add view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddMasterQual()
        {
            return PartialView("AddMasterQual", new MasterQualificationViewModel());
        }

        /// <summary>
        /// Method for adding qualification details.
        /// </summary>
        /// <param name="masterQualViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterQual(MasterQualificationViewModel masterQualViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterQualification(masterQualViewModel, ref message))
                    {
                        message = message == string.Empty ? "Qualification details saved successfully." : message;
                        status = true;
                    }
                    else
                    {

                        message = message == string.Empty ? "Qualification details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddMasterQual", masterQualViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Qualification not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method for setting existing record into the form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterQual(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MasterQualificationViewModel masterQualViewModel = objBAL.GetQualificationDetails_ByQualCode(Convert.ToInt32(decryptedParameters["QualId"].ToString()));
                    if (masterQualViewModel == null)
                    {
                        ModelState.AddModelError(string.Empty, "Qualification details not exist.");
                        return PartialView("AddMasterQual", new MasterQualificationViewModel());
                    }
                    return PartialView("AddMasterQual", masterQualViewModel);

                }
                return PartialView("AddMasterQual", new MasterQualificationViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Qualification details not exist.");

                return PartialView("AddMasterQual", new MasterQualificationViewModel());
            }
        }

        /// <summary>
        /// Method for updating the qualification details.
        /// </summary>
        /// <param name="masterQualViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterQual(MasterQualificationViewModel masterQualViewModel)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterQualification(masterQualViewModel, ref message))
                    {
                        message = message == string.Empty ? "Qualification details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Qualification details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddMasterQual", masterQualViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Qualification details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method for deleting the qualification details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterQual(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteMasterQualification(Convert.ToInt32(decryptedParameters["QualId"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "Qualification details not deleted.");
                        return Json(new { success = false, message = "You can not delete this Qualification details." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = "Qualification details deleted successfully." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message = "You can not delete this Qualification details." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Qualification details." }, JsonRequestBehavior.AllowGet);
            }

        }


        /// <summary>
        /// Method for listing qualification details.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>

        public ActionResult GetMasterQualDetails(int? page, int? rows, string sidx, string sord)
        {

            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {
                rows = objBAL.ListMasterQualification(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //end of master qualification

        #endregion qualificationActions

        #region Master_Execution

        /// <summary>
        /// Method for list view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListMasterExecution()
        {
            return View("ListMasterExecution");
        }

        /// <summary>
        /// Method for Add view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddMasterExecution()
        {
            return PartialView("AddMasterExecution", new MasterExecutionItemViewModel());
        }

        /// <summary>
        /// Method for adding the execution details.
        /// </summary>
        /// <param name="masterExecutionView"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterExecution(MasterExecutionItemViewModel masterExecutionView)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterExecution(masterExecutionView, ref message))
                    {
                        message = message == string.Empty ? "Execution details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Execution details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddMasterExecution", masterExecutionView);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Execution details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }


        }

        /// <summary>
        /// Method to set existing record into the form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterExecution(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameter = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameter = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameter.Count() > 0)
                {
                    MasterExecutionItemViewModel masterExecutionView = objBAL.GetExecutionDetails_ByExecutionCode(Convert.ToInt32(decryptedParameter["ExecutionId"].ToString()));
                    if (masterExecutionView == null)
                    {
                        ModelState.AddModelError(string.Empty, "Execution details not exist.");
                        return PartialView("AddMasterExecution", new MasterExecutionItemViewModel());
                    }
                    return PartialView("AddMasterExecution", masterExecutionView);

                }
                return PartialView("AddMasterExecution", new MasterExecutionItemViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Execution details not exist.");

                return PartialView("AddMasterExecution", new MasterExecutionItemViewModel());
            }


        }

        /// <summary>
        /// Method for updating the execution details.
        /// </summary>
        /// <param name="masterExecutionView"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditMasterExecution(MasterExecutionItemViewModel masterExecutionView)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {


                    if (objBAL.EditMasterExecution(masterExecutionView, ref message))
                    {
                        message = message == string.Empty ? "Execution details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Execution details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddMasterExecution", masterExecutionView);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Execution details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method for deleting execution details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterExecution(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteMasterExecution(Convert.ToInt32(decryptedParameters["ExecutionId"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "Execution details not deleted.");
                        return Json(new { success = false, message = "You can not delete this Execution details." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = "Execution details deleted successfully." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message = "You can not delete this Execution details." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Execution details." }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Method for loading the list.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetMasterExecutionDetails(int? page, int? rows, string sidx, string sord)
        {

            String searchParameters = String.Empty;
            long totalRecords;
            string ItemType = String.Empty;
            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["typeCode"]))
            {
                ItemType = Request.Params["typeCode"];
            }

            //if (!string.IsNullOrEmpty(homeFormCollection["typeCode"]))
            //{
            //    ItemType = homeFormCollection["typeCode"].Replace('+', ' ').Trim();
            //}

            var jsonData = new
            {
                rows = objBAL.ListMasterExecution(ItemType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// method for searching execution details.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchHeadType()
        {
            try
            {
                objDAL = new MasterDAL();

                ViewData["Type"] = new SelectList(objDAL.getAllHeadTypes(), "Value", "Text");

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["Type"] = null;

            }
            return PartialView("SearchHeadType");
        }

        #endregion Master_Execution

        //added by abhinav on 29-NOV-2018
        #region Admin_Sqc

        /// <summary>
        /// Method to search quality controller details.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchSQC()
        {
            AdminSqcViewModel adminSqcModel = new AdminSqcViewModel();
            try
            {
                objDAL = new MasterDAL();

                List<SelectListItem> listDeptName = new List<SelectListItem>();

                // listDeptName = objBAL.GetDepartListBAL(Convert.ToInt32(id));

                listDeptName.Add(new SelectListItem() { Selected = true, Value = "0", Text = "-Select-" });


                adminSqcModel.depatmentList = new SelectList(listDeptName, "value", "text").ToList();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME");

                ViewData["StatusList"] = objDAL.GetAllStatus();


            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "Master.SearchSQC");
                return null;
            }

            return PartialView("SearchSQC", adminSqcModel);
        }

        /// <summary>
        /// Method to load the SQC list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListAdminSqc()
        {
            return View();
        }

        /// <summary>
        /// Method to load ADD view of SQC.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddAdminSqc()
        {
            AdminSqcViewModel adminSqcModel = new AdminSqcViewModel();
            List<SelectListItem> listDeptName = new List<SelectListItem>();
            objBAL = new MasterBAL();
            //listDeptName = objBAL.GetDepartListBAL();
            listDeptName.Add(new SelectListItem() { Selected = true, Text = "-Select-", Value = "0" });
            adminSqcModel.depatmentList = new SelectList(listDeptName, "value", "text").ToList();
            //adminSqcModel.depatmentList.Add(new SelectListItem() { Value = "0", Text = "Select Department", Selected = true });
            return PartialView("AddAdminSqc", adminSqcModel);
        }

        [HttpGet]
        public ActionResult GetDepartmentSearch(string id)
        {
            try
            {
                objBAL = new MasterBAL();
                List<SelectListItem> dptlist = new List<SelectListItem>();


                if (id == "0")
                {
                    dptlist.Add(new SelectListItem() { Selected = true, Text = "All Department", Value = "0" });
                }
                else
                {
                    dptlist = objBAL.GetDeptOfStates(Convert.ToInt32(id));
                    dptlist.Insert(0, new SelectListItem() { Selected = true, Text = "All Department", Value = "0" });
                }

                return Json(new SelectList(dptlist, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.GetDepartmentSearch");
                return null;
            }

        }


        [HttpGet]
        public ActionResult GetDepartmentOfStates(string id)
        {
            try
            {
                objBAL = new MasterBAL();
                List<SelectListItem> dptlist = new List<SelectListItem>();
                dptlist = objBAL.GetDeptOfStates(Convert.ToInt32(id));
                dptlist.Insert(0, new SelectListItem() { Selected = true, Text = "-Select Department-", Value = "0" });
                return Json(new SelectList(dptlist, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.GetDepartmentOfStates");
                return null;
            }

        }

        /// <summary>
        /// Method for adding SQC details.
        /// </summary>
        /// <param name="adminSqcView"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdminSqc(AdminSqcViewModel adminSqcView)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddAdminSqc(adminSqcView, ref message))
                    {
                        message = message == string.Empty ? "Quality Controller details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Quality Controller details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddAdminSqc", adminSqcView);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.AddAdminSqc");
                message = message == string.Empty ? "Quality Controller details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// Method to set existing record into form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditAdminSqc(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameter = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameter = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameter.Count() > 0)
                {
                    AdminSqcViewModel adminSqcView = objBAL.GetAdminQc_ByQcCode(Convert.ToInt32(decryptedParameter["QcCode"].ToString()));
                    List<SelectListItem> listDeptName = new List<SelectListItem>();
                    listDeptName = objBAL.GetDeptOfStates(adminSqcView.MAST_STATE_CODE);
                    adminSqcView.depatmentList = new SelectList(listDeptName, "value", "text").ToList();
                    SelectListItem repeatedObj = adminSqcView.depatmentList.Find(Itm => Itm.Text == adminSqcView.ADMIN_ND_NAME && Itm.Value == adminSqcView.ADMIN_ND_CODE.ToString() && Itm.Selected == false);
                    adminSqcView.depatmentList.Remove(repeatedObj);
                    adminSqcView.depatmentList.Insert(0, new SelectListItem() { Selected = true, Text = adminSqcView.ADMIN_ND_NAME, Value = adminSqcView.ADMIN_ND_CODE.ToString() });

                    if (adminSqcView == null)
                    {
                        ModelState.AddModelError(string.Empty, "Quality Controller details not exist.");
                        return PartialView("AddAdminSqc", new AdminSqcViewModel());

                    }
                    if (adminSqcView.ADMIN_ACTIVE_STATUS.Equals("Y"))
                    {
                        adminSqcView.IsActive = true;
                    }
                    else
                    {
                        adminSqcView.IsActive = false;
                    }

                    return PartialView("AddAdminSqc", adminSqcView);
                }
                return PartialView("AddAdminSqc", new AdminSqcViewModel());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.EditAdminSqc");
                ModelState.AddModelError(string.Empty, "Quality Controller details not exist.");

                return PartialView("AddAdminSqc", new AdminSqcViewModel());
            }
        }

        /// <summary>
        /// Method to update SQC details.
        /// </summary>
        /// <param name="adminSqcView"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAdminSqc(AdminSqcViewModel adminSqcView)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditAdminSqc(adminSqcView, ref message))
                    {
                        message = message == string.Empty ? "Quality Controller details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Quality Controller details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddAdminSqc", adminSqcView);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.EditAdminSqc");
                message = message == string.Empty ? "Quality Controller details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
        }
        /// <summary>
        /// Method to delete SQC details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAdminSqc(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteAdminSqc(Convert.ToInt32(decryptedParameters["QcCode"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "Quality controller details not deleted.");
                        return Json(new { success = false, message = "You can not delete this Quality Controller details." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = "Quality Controller details deleted successfully." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message = "You can not delete this Quality Controller details." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "Master.DeleteAdminSqc");
                return Json(new { success = false, message = "You can not delete this Quality Controller details." }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Method to load SQC list.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>

        [HttpGet]
        public ActionResult GetAdminSqcDetails(int? page, int? rows, string sidx, string sord)
        {

            try
            {
                String searchParameters = String.Empty;
                long totalRecords;
                int stateCode = 0;
                int adminNdCode = 0;
                string status = string.Empty;
                objBAL = new MasterBAL();
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
                    adminNdCode = Convert.ToInt32(Request.Params["adminNdCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    status = Request.Params["status"].Trim();
                }


                var jsonData = new
                {
                    rows = objBAL.ListadminQc(stateCode, adminNdCode, status, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.GetAdminSqcDetails");
                return null;
            }
        }

        /// <summary>
        /// Method to get all districts.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult getDistrictsName(string id)
        {
            try
            {
                db = new PMGSYEntities();
                if (!int.TryParse(id, out outParam))
                {
                    return Json(false);
                }
                List<MASTER_DISTRICT> list = db.MASTER_DISTRICT.ToList();
                List<SelectListItem> list_dist = new List<SelectListItem>();
                list_dist.Insert(0, new SelectListItem() { Value = "0", Text = "--Select--", Selected = true });
                foreach (var item in list)
                {
                    if (item.MAST_STATE_CODE == (Convert.ToInt32(id)))
                    {
                        list_dist.Add(new SelectListItem { Value = item.MAST_DISTRICT_CODE.ToString(), Text = item.MAST_DISTRICT_NAME.ToString() });
                    }
                }
                return Json(list_dist);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.getDistrictsName");
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }

        #endregion Admin_Sqc

        #region masterstreams

        /// <summary>
        /// Method to load streams list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterListStreams()
        {
            return View();
        }

        /// <summary>
        /// Method to load ADD form of Streams.
        /// </summary>
        /// <returns></returns>

        public ActionResult AddtMasterStreams()
        {
            return PartialView("AddtMasterStreams", new MasterStreamsViewModel());
        }

        /// <summary>
        /// Method to add streams details.
        /// </summary>
        /// <param name="masterStreamViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterStreams(MasterStreamsViewModel masterStreamViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterStreams(masterStreamViewModel, ref message))
                    {
                        message = message == string.Empty ? "Stream details saved successfully." : message;
                        status = true;
                    }
                    else
                    {

                        message = message == string.Empty ? "Stream details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddtMasterStreams", masterStreamViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Stream details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        /// Method to set existing record into the form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterStream(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MasterStreamsViewModel masterStreamViewModel = objBAL.GetStreamsDetails_ByStream(Convert.ToInt32(decryptedParameters["StreamsId"].ToString()));
                    if (masterStreamViewModel == null)
                    {
                        ModelState.AddModelError(string.Empty, "Stream details not exist.");
                        return PartialView("AddtMasterStreams", new MasterQualificationViewModel());
                    }
                    return PartialView("AddtMasterStreams", masterStreamViewModel);

                }
                return PartialView("AddtMasterStreams", new MasterQualificationViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Stream details not exist.");
                //return View();
                return PartialView("AddtMasterStreams", new MasterQualificationViewModel());
            }
        }

        /// <summary>
        /// Method to update the streams details.
        /// </summary>
        /// <param name="masterStreamsViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterStreams(MasterStreamsViewModel masterStreamsViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.EditMasterStreams(masterStreamsViewModel, ref message))
                    {
                        message = message == string.Empty ? "Stream details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Stream details are not updated." : message;
                    }

                }
                else
                {
                    return PartialView("AddtMasterStreams", masterStreamsViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Stream details are not updated.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Method to delete the streams details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterStreams(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteMasterStreams(Convert.ToInt32(decryptedParameters["StreamsId"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "You can not delete this Stream details.");
                        return Json(new { success = false, message = "You can not delete this stream details." }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { success = true, message = "Stream details deleted successfully." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message = "You can not delete this Stream details." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Stream details." }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Method to load streams list.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetMasterStreamsList(int? page, int? rows, string sidx, string sord)
        {

            String searchParameters = String.Empty;
            long totalRecords;
            string streamType = String.Empty;
            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["StreamType"]))
            {
                streamType = Request.Params["StreamType"].Replace('+', ' ').Trim();
            }
            var jsonData = new
            {
                rows = objBAL.ListMasterStreams(streamType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to search streams details.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchStreamType()
        {
            try
            {
                MasterStreamsViewModel model = new MasterStreamsViewModel();
                ViewBag.StreamType = model.StreamsType;
                return PartialView("SearchStreamType");

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["StreamType"] = null;

            }
            return PartialView("SearchStreamType");
        }

        #endregion

        #region Checklist Points Actions

        /// <summary>
        /// Method to load CheckPoint list 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterChecklist()
        {
            return View();
        }

        /// <summary>
        /// Method to ADD form of Checklist Point
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterChecklist()
        {
            return PartialView("AddEditMasterChecklist", new MasterChecklistPointsViewModel());
        }

        /// <summary>
        /// Method to add Checklist Details 
        /// </summary>
        /// <param name="masterChecklistViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterChecklist(MasterChecklistPointsViewModel masterChecklistViewModel)
        {
            bool status = false;
            try
            {

                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterChecklist(masterChecklistViewModel, ref message))
                    {
                        message = message == string.Empty ? "Checklist Point details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Checklist Point details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterChecklist", masterChecklistViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Checklist Point  details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///  Method to set existing record into the form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterChecklist(String parameter, String hash, String key)
        {

            Dictionary<string, string> decryptedParameters = null;

            try
            {

                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MasterChecklistPointsViewModel masterChecklistViewMode = objBAL.GetChecklistDetails_ByChecklistCode(Convert.ToInt32(decryptedParameters["ChecklistID"].ToString()));
                    if (masterChecklistViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Checklist Point details not exist.");
                        return PartialView("AddEditMasterChecklist", new MasterChecklistPointsViewModel());
                    }
                    return PartialView("AddEditMasterChecklist", masterChecklistViewMode);

                }
                return PartialView("AddEditMasterChecklist", new MasterChecklistPointsViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Checklist Point details not exist.");

                return PartialView("AddEditMasterChecklist", new MasterChecklistPointsViewModel());
            }
        }

        /// <summary>
        ///  Method to update the Checklist Point details.
        /// </summary>
        /// <param name="masterChecklistViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterChecklist(MasterChecklistPointsViewModel masterChecklistViewModel)
        {
            bool status = false;

            try
            {

                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterChecklist(masterChecklistViewModel, ref message))
                    {
                        message = message == string.Empty ? "Checklist Point details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Checklist Point details not updated." : message;
                    }

                }
                else
                {
                    return PartialView("AddEditMasterChecklist", masterChecklistViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Checklist Point details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete Checklist Point Details 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterChecklist(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {

                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteMasterChecklist(Convert.ToInt32(decryptedParameters["ChecklistID"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "You can not delete this Checklist Point details.");


                        return Json(new { success = false, message = "You can not delete this Checklist Point details." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Checklist Point details deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Checklist Point details." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method for Loading the Checklist Point List . 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetMasterChecklistList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {

                rows = objBAL.ListMasterChecklist(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion Checklist Points Actions

        #region Master Agency Actions

        /// <summary>
        ///     Method to load Agency List.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterAgency()
        {
            return View();
        }

        /// <summary>
        /// Method to load Agency List .
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetMasterAgencyList(int? page, int? rows, string sidx, string sord)
        {

            String searchParameters = String.Empty;
            long totalRecords;
            string agencyType = String.Empty;
            objBAL = new MasterBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["AgencyType"]))
            {
                agencyType = Request.Params["AgencyType"].Replace('+', ' ').Trim();
            }



            var jsonData = new
            {
                rows = objBAL.ListMasterAgency(agencyType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method To load ADD view of the Agenncy Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterAgency()
        {
            objBAL = new MasterBAL();
            List<SelectListItem> list = objBAL.GetAgencyCode();
            ViewBag.AgencyType = list;
            return PartialView("AddEditMasterAgency", new MasterAgencyViewModel());
        }

        /// <summary>
        /// Method to add the Agency Details
        /// </summary>
        /// <param name="masterAgencyViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterAgency(MasterAgencyViewModel masterAgencyViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterAgency(masterAgencyViewModel, ref message))
                    {
                        message = message == string.Empty ? "Agency details saved successfully." : message;
                        status = true;
                    }
                    else
                    {

                        message = message == string.Empty ? "Agency details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterAgency", masterAgencyViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Agency details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to set existing records in the Agency Details view.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterAgency(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MasterAgencyViewModel masterAgencyViewModel = objBAL.GetAgencyDetails_ByAgencyCode(Convert.ToInt32(decryptedParameters["AgencyID"].ToString()));
                    if (masterAgencyViewModel == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agency  details not exist.");
                        return PartialView("AddEditMasterAgency", new MasterAgencyViewModel());
                    }
                    return PartialView("AddEditMasterAgency", masterAgencyViewModel);

                }
                return PartialView("AddEditMasterAgency", new MasterAgencyViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Agency details not exist.");

                return PartialView("AddEditMasterAgency", new MasterAgencyViewModel());
            }
        }

        /// <summary>
        /// Method to update Agency Details
        /// </summary>
        /// <param name="masterAgencyViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterAgency(MasterAgencyViewModel masterAgencyViewModel)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterAgency(masterAgencyViewModel, ref message))
                    {
                        message = message == string.Empty ? "Agency details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Agency details not updated." : message;
                    }

                }
                else
                {
                    return PartialView("AddEditMasterAgency", masterAgencyViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Agency details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete a perticular record of Agency 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterAgency(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;


            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteMasterAgency(Convert.ToInt32(decryptedParameters["AgencyID"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "You can not delete this Agency details.");

                        return Json(new { success = false, message = "You can not delete this Agency details." }, JsonRequestBehavior.AllowGet);

                    }
                }
                return Json(new { success = true, message = "Agency details deleted successfully." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Agency details." }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Method to search Agency Records.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchAgencyType()
        {
            try
            {
                MasterAgencyViewModel model = new MasterAgencyViewModel();
                ViewBag.AgencyType = model.AgencyType;
                return PartialView("SearchAgencyType");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["AgencyType"] = null;

            }
            return PartialView("SearchAgencyType");
        }


        #endregion  Master Agency Actions

        #region Actions of Reason

        /// <summary>
        /// Method For Loading the Reason List 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterReason()
        {
            return View();
        }

        /// <summary>
        /// Method For Loading the Reason List 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetMasterReasonList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            string reasonType = String.Empty;
            objBAL = new MasterBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["ReasonType"]))
            {
                reasonType = Request.Params["ReasonType"].Replace('+', ' ').Trim();
            }
            var jsonData = new
            {
                rows = objBAL.ListMasterReason(reasonType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method for getting the Reason List 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterReason()
        {
            return PartialView("AddEditMasterReason", new MasterReasonViewModel());
        }

        /// <summary>
        /// Method For adding the Reason Details 
        /// </summary>
        /// <param name="masterReasonViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterReason(MasterReasonViewModel masterReasonViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterReason(masterReasonViewModel, ref message))
                    {
                        message = message == string.Empty ? "Reason details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Reason details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterReason", masterReasonViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Reason details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method for setting the existing Reason details in the Reason Details view.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterReason(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MasterReasonViewModel masterReasonViewMode = objBAL.GetReasonDetails_ByReasonCode(Convert.ToInt32(decryptedParameters["ReasonID"].ToString()));
                    if (masterReasonViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Reason  details not exist.");
                        return PartialView("AddEditMasterReason", new MasterReasonViewModel());
                    }
                    return PartialView("AddEditMasterReason", masterReasonViewMode);

                }
                return PartialView("AddEditMasterReason", new MasterReasonViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Reason  details not exist.");

                return PartialView("AddEditMasterReason", new MasterReasonViewModel());
            }
        }

        /// <summary>
        /// Method for Updating the Reason Details.
        /// </summary>
        /// <param name="masterReasonViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterReason(MasterReasonViewModel masterReasonViewModel)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterReason(masterReasonViewModel, ref message))
                    {
                        message = message == string.Empty ? "Reason details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Reason details not updated." : message;
                    }

                }
                else
                {
                    return PartialView("AddEditMasterReason", masterReasonViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Reason details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method for deleting the perticular records Reason.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterReason(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteMasterReason(Convert.ToInt32(decryptedParameters["ReasonID"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "You can not delete this Reason details.");

                        return Json(new { success = false, message = "You can not delete this Reason details." }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { success = true, message = "Reason details deleted successfully." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Reason details." }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Method for Searching the Reason.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchReasonType()
        {
            try
            {
                MasterReasonViewModel model = new MasterReasonViewModel();
                ViewBag.ReasonType = model.ReasonType;
                return PartialView("SearchReasonType");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["ReasonType"] = null;
            }
            return PartialView("SearchReasonType");
        }

        #endregion Actions of Reason

        #region Technical Agency

        /// <summary>
        /// method to load technical Agency list view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListAdminTechnicalAgency()
        {
            return View();
        }

        /// <summary>
        /// Added by Sammed Patil 29 May 2014
        /// method to load technical Agency list view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewAdminTechnicalAgencyMapping()
        {
            PMGSY.Models.Master.ViewAdminTechnicalAgencyMapping viewTAMap = new ViewAdminTechnicalAgencyMapping();
            List<SelectListItem> DistrictList = new List<SelectListItem>();
            List<PMGSY.Models.MASTER_STATE> stateList = new List<PMGSY.Models.MASTER_STATE>();
            SelectList dist;
            try
            {
                IMasterDAL objDAL = new MasterDAL();

                viewTAMap.AgencyList = objDAL.GetTATypes();
                //viewTAMap.AgencyList.Insert(0, new PMGSY.Models.MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "All Districts" });
                //viewTAMap.AgencyList.Add(new SelectListItem()
                //{
                //    Text = "All Agencies",
                //    Value = "0",
                //    Selected = true
                //});
                //viewTAMap.AgencyList.Sort();

                //ViewData["StateList"] = new SelectList(objDAL.GetAllStates(), "MAST_STATE_CODE", "MAST_STATE_NAME");

                stateList = objDAL.GetAllStates();
                stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" });
                ViewData["StateList"] = new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");
                if (PMGSY.Extensions.PMGSYSession.Current.RoleCode == 36)
                {
                    viewTAMap.State = PMGSY.Extensions.PMGSYSession.Current.StateCode;

                    List<PMGSY.Models.MASTER_DISTRICT> districtList = new List<PMGSY.Models.MASTER_DISTRICT>();

                    //districtList = objDAL.GetAllDistricts(Convert.ToInt32(stateCode.Trim()));
                    districtList = objDAL.GetAllDistrictByStateCode(viewTAMap.State);

                    //if (districtList.Count == 0)
                    //{
                    districtList.Insert(0, new PMGSY.Models.MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "All Districts" });
                    ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");
                }
                else
                {
                    dist = new SelectList(objDAL.GetAllDistrictByStateCode(0), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");

                    if (dist.Count() == 0)
                    {
                        //dist.Insert(0, new PMGSY.Models.MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "All Districts" });
                        List<SelectListItem> sl = new List<SelectListItem>();
                        sl.Add(new SelectListItem() { Text = "All Districts", Value = "0" });
                        ViewData["DistrictList"] = sl;
                    }
                    else
                    {
                        ViewData["DistrictList"] = dist;
                    }
                }

                //ViewData["AgencyType"] = new SelectList(objDAL.GetTATypes(), "Value", "Text");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                //ViewData["AgencyType"] = null;
            }
            return PartialView("ViewAdminTechnicalAgencyMapping", viewTAMap);
        }

        /// Added by Sammed Patil 29 May 2014
        public JsonResult GetDistrictbyState(string stateCode)
        {
            objDAL = new MasterDAL();
            try
            {
                if (!int.TryParse(stateCode, out outParam))
                {
                    return Json(false);
                }

                List<PMGSY.Models.MASTER_DISTRICT> districtList = new List<PMGSY.Models.MASTER_DISTRICT>();

                //districtList = objDAL.GetAllDistricts(Convert.ToInt32(stateCode.Trim()));
                districtList = objDAL.GetAllDistrictByStateCode(Convert.ToInt32(stateCode));

                //if (districtList.Count == 0)
                //{
                districtList.Insert(0, new PMGSY.Models.MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "All Districts" });
                //}

                return Json(new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(false);
            }
        }//end function GetDistrictsByStateCode

        /// Added by Sammed Patil 29 May 2014
        public ActionResult GetDistrictTechnicalAgencyDetails(FormCollection formCollection)
        {
            long totalRecords;
            objBAL = new MasterBAL();
            try
            {
                using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                var jsonData = new
                {
                    rows = objBAL.ListdistrictTechnicalAgency(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToString(formCollection["Agency"]), Convert.ToInt32(formCollection["StateCode"]),
                                                            Convert.ToInt32(formCollection["DistrictCode"])
                                                            ),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }


            //var jsonData = new
            //{
            //    rows = objBAL.ListadminTechnicalAgency(taName, taType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
            //    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
            //    page = Convert.ToInt32(page),
            //    records = totalRecords
            //};

            //return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to load the technical agency details view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddAdminTechnicalAgency()
        {
            return PartialView("AddAdminTechnicalAgency", new AdminTechnicalAgencyViewModel());
        }

        /// <summary>
        /// Method for adding the Technical Agency details.
        /// </summary>
        /// <param name="adminTechnicalAgencyView"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdminTechnicalAgency(AdminTechnicalAgencyViewModel adminTechnicalAgencyView)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddAdminTechnicalAgency(adminTechnicalAgencyView, ref message))
                    {
                        message = message == string.Empty ? "Technical Agency details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Technical Agency details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddAdminTechnicalAgency", adminTechnicalAgencyView);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Technical Agency details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// Method for setting the existing records in the Technical Details view.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditAdminTechnicalAgency(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameter = null;

            try
            {
                objBAL = new MasterBAL();

                decryptedParameter = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameter.Count() > 0)
                {
                    AdminTechnicalAgencyViewModel adminTechnicalAgencyView = objBAL.GetAdminTA_ByTACode(Convert.ToInt32(decryptedParameter["TACode"].ToString()));
                    if (adminTechnicalAgencyView == null)
                    {
                        ModelState.AddModelError(string.Empty, "Technical Agency details not exist.");
                        return PartialView("AddAdminTechnicalAgency", new AdminTechnicalAgencyViewModel());
                    }
                    return PartialView("AddAdminTechnicalAgency", adminTechnicalAgencyView);

                }
                return PartialView("AddAdminTechnicalAgency", new AdminTechnicalAgencyViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Technical Agency details not exist.");

                return PartialView("AddAdminTechnicalAgency", new AdminTechnicalAgencyViewModel());
            }
        }

        /// <summary>
        /// Method for updating the Technical Agency records
        /// </summary>
        /// <param name="adminTechnicalAgencyView"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAdminTechnicalAgency(AdminTechnicalAgencyViewModel adminTechnicalAgencyView)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditAdminTechnicalAgency(adminTechnicalAgencyView, ref message))
                    {
                        message = message == string.Empty ? "Technical Agency details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Technical Agency details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddAdminTechnicalAgency", adminTechnicalAgencyView);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Technical Agency details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// Method for delating the record of technical Agency
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        public ActionResult DeleteAdminTechnicalAgency(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteAdminTechnicalAgency(Convert.ToInt32(decryptedParameters["TACode"].ToString())))
                    {
                        return Json(new { success = false, message = "You can not delete this Technical Agency details." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Technical Agency details deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Technical Agency details." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to  Load the Technical Agency List 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetAdminTechnicalAgencyDetails(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            string taName = String.Empty;
            string taType = String.Empty;
            objBAL = new MasterBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["AgencyName"]))
            {
                taName = Request.Params["AgencyName"].Replace('+', ' ').Trim();
            }

            if (!string.IsNullOrEmpty(Request.Params["AgencyType"]))
            {
                taType = Request.Params["AgencyType"];
            }

            var jsonData = new
            {
                rows = objBAL.ListadminTechnicalAgency(taName, taType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method for loading the Districts.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult getDistrictsNameTA(string id)
        {
            try
            {
                db = new PMGSYEntities();
                if (!int.TryParse(id, out outParam))
                {
                    return Json(false);
                }
                List<MASTER_DISTRICT> list = db.MASTER_DISTRICT.ToList();
                List<SelectListItem> list_dist = new List<SelectListItem>();
                list_dist.Insert(0, new SelectListItem() { Value = "0", Text = "---Select---", Selected = true });
                foreach (var item in list)
                {
                    if (item.MAST_STATE_CODE == (Convert.ToInt32(id)))
                    {
                        list_dist.Add(new SelectListItem { Value = item.MAST_DISTRICT_CODE.ToString(), Text = item.MAST_DISTRICT_NAME.ToString() });
                    }
                }
                return Json(list_dist);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }

        /// <summary>
        /// Method for searching the Technical Agency  Details 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchTechnicalAgency()
        {

            try
            {
                IMasterDAL objDAL = new MasterDAL();

                ViewData["AgencyType"] = new SelectList(objDAL.GetTATypes(), "Value", "Text");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["AgencyType"] = null;

            }
            return PartialView("SearchTechnicalAgency");
        }



        /// <summary>
        /// Method for setting the existing records in the Technical Details view.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewAdminTechnicalAgency(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameter = null;

            try
            {
                objBAL = new MasterBAL();

                decryptedParameter = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameter.Count() > 0)
                {
                    AdminTechnicalAgencyViewModel adminTechnicalAgencyView = objBAL.GetAdminTA_ByTACode(Convert.ToInt32(decryptedParameter["TACode"].ToString()));
                    if (adminTechnicalAgencyView == null)
                    {
                        ModelState.AddModelError(string.Empty, "Technical Agency details not exist.");
                        return PartialView("ViewAdminTechnicalAgency", new AdminTechnicalAgencyViewModel());
                    }
                    return PartialView("ViewAdminTechnicalAgency", adminTechnicalAgencyView);

                }
                return PartialView("ViewAdminTechnicalAgency", new AdminTechnicalAgencyViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Technical Agency details not exist.");

                return PartialView("ViewAdminTechnicalAgency", new AdminTechnicalAgencyViewModel());
            }
        }



        #endregion Technical Agency

        #region Surface Actions

        /// <summary>
        /// MasterSurface() action is used to display Surface Add/Edit From and Surface List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterSurface()
        {
            return View();
        }

        /// <summary>
        /// AddEditMasterSurface() action Display Add/Edit Surface Data Entry Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterSurface()
        {
            return PartialView("AddEditMasterSurface", new MasterSurfaceViewModel());
        }

        /// <summary>
        /// AddMasterSurface () save Surface Details into data base
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterSurface(MasterSurfaceViewModel masterSurfaceViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterSurface(masterSurfaceViewModel, ref message))
                    {
                        message = message == string.Empty ? "Surface details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Surface details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterSurface", masterSurfaceViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Surface details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditMasterSurface() action is used to display Surface Data Entry form In Edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterSurface(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MasterSurfaceViewModel masterSurfaceViewMode = objBAL.GetSurfaceDetails_BySurfaceCode(Convert.ToInt32(decryptedParameters["SurfaceID"].ToString()));
                    if (masterSurfaceViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Surface details no exist.");
                        return PartialView("AddEditMasterSurface", new MasterSurfaceViewModel());
                    }
                    return PartialView("AddEditMasterSurface", masterSurfaceViewMode);

                }
                return PartialView("AddEditMasterSurface", new MasterSurfaceViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Surface details not exist.");
                //return View();
                return PartialView("AddEditMasterSurface", new MasterSurfaceViewModel());
            }
        }

        /// <summary>
        /// EditMasterSurface() action is used to Update Surface details
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterSurface(MasterSurfaceViewModel masterSurfaceViewModel)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterSurface(masterSurfaceViewModel, ref message))
                    {
                        message = message == string.Empty ? "Surface details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Surface details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterSurface", masterSurfaceViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Surface details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// DeleteMasterSurface() action is used to delete surface Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult DeleteMasterSurface(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteMasterSurface(Convert.ToInt32(decryptedParameters["SurfaceID"].ToString()), ref message))
                    {
                        message = "Surface details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Surface details." : message;
                    }
                    return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "You can not delete this Surface details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Surface details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GetMasterSurfaceList() action display Surface List on Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetMasterSurfaceList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            var jsonData = new
            {
                rows = objBAL.ListMasterSurface(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion Surface Actions

        #region Component Type Actions

        /// <summary>
        /// MasterComponentType() action is used to display Component Type Add/Edit From and Component Type List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterComponentType()
        {
            return View();
        }

        /// <summary>
        /// GetMasterComponentTypeList() action display Component Type List on Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetMasterComponentTypeList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            var jsonData = new
            {
                rows = objBAL.ListMasterComponentType(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AddEditMasterComponentType() action Display Add/Edit Surface Data Entry Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterComponentType()
        {
            return PartialView("AddEditMasterComponentType", new MasterComponentTypeViewModel());
        }

        /// <summary>
        /// AddMasterComponentType() save Component Type Details into data base
        /// </summary>
        /// <param name="masterContractorClassTypeViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterComponentType(MasterComponentTypeViewModel masterComponentTypeViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterComponentType(masterComponentTypeViewModel, ref message))
                    {
                        message = message == string.Empty ? "Component details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Component details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterComponentType", masterComponentTypeViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Component details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditMasterComponentType() action is used to Display Component Type Details Form in Edit Mode
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns> 
        [HttpGet]
        public ActionResult EditMasterComponentType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MasterComponentTypeViewModel masterComponetTypeViewMode = objBAL.GetComponentDetails_ByComponentCode(Convert.ToInt32(decryptedParameters["CompID"].ToString()));
                    if (masterComponetTypeViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Component details not exist.");
                        return PartialView("AddEditMasterComponentType", new MasterComponentTypeViewModel());
                    }
                    return PartialView("AddEditMasterComponentType", masterComponetTypeViewMode);

                }
                return PartialView("AddEditMasterComponentType", new MasterComponentTypeViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Component details not exist.");
                return PartialView("AddEditMasterComponentType", new MasterComponentTypeViewModel());
            }
        }

        /// <summary>
        /// EditMasterComponentType() action is used to Update Component Type details
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterComponentType(MasterComponentTypeViewModel masterComponentTypeViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterComponentType(masterComponentTypeViewModel, ref message))
                    {
                        message = message == string.Empty ? "Component details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Component details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterComponentType", masterComponentTypeViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Component details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditMasterComponentType() action is used to Delete Component Type details
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterComponentType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteMasterComponentType(Convert.ToInt32(decryptedParameters["CompID"].ToString()), ref message))
                    {
                        message = "Component details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Component details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                message = "You can not delete this Component details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Component details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Component Type Actions

        #region Grade DAL defination

        /// <summary>
        /// MasterGradeType() action is used to display Grade Type Add/Edit Form and Grade Type List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterGradeType()
        {
            return View();
        }

        /// <summary>
        /// GetMasterGradeTypeList() action display Grade Type List on Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetMasterGradeTypeList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {
                rows = objBAL.ListMasterGradeType(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AddEditMasterGradeType() action Display Add/Edit Grade Type Data Entry Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterGradeType()
        {
            return PartialView("AddEditMasterGradeType", new MasterGradeTypeViewModel());
        }

        /// <summary>
        /// AddMasterGradeType() save Grade Type Details into data base
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterGradeType(MasterGradeTypeViewModel masterGradeTypeViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterGradeType(masterGradeTypeViewModel, ref message))
                    {
                        message = message == string.Empty ? "Grade details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Grade details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterGradeType", masterGradeTypeViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Grade details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditMasterGradeType() action is used to Display Grade Type Form in Edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterGradeType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MasterGradeTypeViewModel masterGradeTypeViewMode = objBAL.GetGradeTypeDetails_ByGradeCode(Convert.ToInt32(decryptedParameters["GradeID"].ToString()));
                    if (masterGradeTypeViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Grade details not exist.");
                        return PartialView("AddEditMasterGradeType", new MasterGradeTypeViewModel());
                    }
                    return PartialView("AddEditMasterGradeType", masterGradeTypeViewMode);

                }
                return PartialView("AddEditMasterGradeType", new MasterGradeTypeViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Grade details not exist.");
                return PartialView("AddEditMasterGradeType", new MasterGradeTypeViewModel());
            }
        }

        /// <summary>
        /// EditMasterGradeType() action is used to Update Grade Type Details
        /// </summary>
        /// <param name="masterGradeTypeViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterGradeType(MasterGradeTypeViewModel masterGradeTypeViewModel)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterGradeType(masterGradeTypeViewModel, ref message))
                    {
                        message = message == string.Empty ? "Grade details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Grade details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterGradeType", masterGradeTypeViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Grade details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// DeleteMasterGradeType() action is used to delete Grade Type Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterGradeType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteMasterGradeType(Convert.ToInt32(decryptedParameters["GradeID"].ToString()), ref message))
                    {
                        message = "Grade details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Grade details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "You can not delete this Grade details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Grade details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Grade DAL defination

        #region Terrain Actions

        /// <summary>
        /// MasterTerrainType() action is used to Display Terrain Type Add/Edit Details Form and Terrain List In Grid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterTerrainType()
        {
            return View();
        }

        /// <summary>
        /// GetMasterTerrainTypeList() action is used to display Terrrain Type Grid 
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>

        public ActionResult GetMasterTerrainTypeList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            objBAL = new MasterBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            var jsonData = new
            {
                rows = objBAL.ListMasterTerrainType(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AddEditMasterTerrainType() action is used to Display Add Terrain Type Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterTerrainType()
        {
            return PartialView("AddEditMasterTerrainType", new MasterTerrainTypeViewModel());
        }

        /// <summary>
        /// AddMasterTerrainType() action is to save Terrain type details
        /// </summary>
        /// <param name="masterTerrainTypeViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterTerrainType(MasterTerrainTypeViewModel masterTerrainTypeViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterTerrainType(masterTerrainTypeViewModel, ref message))
                    {
                        message = message == string.Empty ? "Terrain details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Terrain details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterTerrainType", masterTerrainTypeViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Terrain details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditMasterTerrainType() action is used to Display Terrain Type Form In edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterTerrainType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MasterTerrainTypeViewModel masterTerrainTypeViewMode = objBAL.GetTerrainTypeDetails_ByTerrainCode(Convert.ToInt32(decryptedParameters["TerrainID"].ToString()));
                    if (masterTerrainTypeViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Terrain details not exist.");
                        return PartialView("AddEditMasterTerrainType", new MasterTerrainTypeViewModel());
                    }
                    return PartialView("AddEditMasterTerrainType", masterTerrainTypeViewMode);
                }
                return PartialView("AddEditMasterTerrainType", new MasterTerrainTypeViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Terrain details not exist.");
                return PartialView("AddEditMasterTerrainType", new MasterTerrainTypeViewModel());
            }
        }

        /// <summary>
        /// EditMasterTerrainType() action is used to Update Terrain Type Details
        /// </summary>
        /// <param name="masterTerrainTypeViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterTerrainType(MasterTerrainTypeViewModel masterTerrainTypeViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterTerrainType(masterTerrainTypeViewModel, ref message))
                    {
                        message = message == string.Empty ? "Terrain details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Terrain details not updated." : message;
                    }

                }
                else
                {
                    return PartialView("AddEditMasterTerrainType", masterTerrainTypeViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Terrain details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// DeleteMasterTerrainType() action is used to Delete Terrain Type Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterTerrainType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteMasterTerrainType(Convert.ToInt32(decryptedParameters["TerrainID"].ToString()), ref message))
                    {
                        message = "Terrain details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Terrain details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                message = "You can not delete this Terrain details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Terrain details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Terrain Actions

        #region Unit Actions

        /// <summary>
        /// MasterUnit() action is used to Display Add/Edit Unit details Form and Unit Grid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterUnit()
        {
            return View();
        }

        /// <summary>
        /// GetMasterUnitList() action is used to Display Unit List on Grid
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>

        public ActionResult GetMasterUnitList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            objBAL = new MasterBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            var jsonData = new
            {
                rows = objBAL.ListMasterUnit(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AddEditMasterUnit() action is used to Display Add/Edit Form And Grid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterUnit()
        {
            return PartialView("AddEditMasterUnit", new MasterUnitsTypeViewModel());
        }

        /// <summary>
        /// AddMasterUnit() action is used to Save Unit Details
        /// </summary>
        /// <param name="frmCollect"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterUnit(FormCollection frmCollect)
        {
            bool status = false;
            MasterUnitsTypeViewModel masterUnitViewModel = new MasterUnitsTypeViewModel();
            objBAL = new MasterBAL();
            masterUnitViewModel.MAST_UNIT_NAME = frmCollect["MAST_UNIT_NAME"];
            masterUnitViewModel.MAST_UNIT_SHORT_NAME = frmCollect["MAST_UNIT_SHORT_NAME"];

            if (frmCollect["MAST_UNIT_DIMENSION"] != null && frmCollect["MAST_UNIT_DIMENSION"] != "")
            {
                masterUnitViewModel.MAST_UNIT_DIMENSION = Convert.ToInt32(frmCollect["MAST_UNIT_DIMENSION"]);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterUnit(masterUnitViewModel, ref message))
                    {
                        message = message == string.Empty ? "Unit details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Unit details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterUnit", masterUnitViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Unit details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditMasterUnit() action is used to Dispaly Unit Data entry form in Edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterUnit(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MasterUnitsTypeViewModel masterUnitViewMode = objBAL.GetUnitDetails_ByUnitCode(Convert.ToInt32(decryptedParameters["UnitID"].ToString()));
                    if (masterUnitViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unit details not exist.");
                        return PartialView("AddEditMasterUnit", new MasterUnitsTypeViewModel());
                    }
                    return PartialView("AddEditMasterUnit", masterUnitViewMode);

                }
                return PartialView("AddEditMasterUnit", new MasterUnitsTypeViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Unit details not exist.");
                return PartialView("AddEditMasterUnit", new MasterUnitsTypeViewModel());
            }
        }

        /// <summary>
        /// EditMasterUnit() action used to Update Unit Details
        /// </summary>
        /// <param name="frmCollect"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterUnit(FormCollection frmCollect)
        {
            bool status = false;
            MasterUnitsTypeViewModel masterUnitViewModel = new MasterUnitsTypeViewModel();

            objBAL = new MasterBAL();

            masterUnitViewModel.EncryptedUnitCode = frmCollect["EncryptedUnitCode"];
            masterUnitViewModel.MAST_UNIT_CODE = Convert.ToInt32(frmCollect["MAST_UNIT_CODE"]);
            masterUnitViewModel.MAST_UNIT_NAME = frmCollect["MAST_UNIT_NAME"];
            masterUnitViewModel.MAST_UNIT_SHORT_NAME = frmCollect["MAST_UNIT_SHORT_NAME"];

            if (frmCollect["MAST_UNIT_DIMENSION"] != null && frmCollect["MAST_UNIT_DIMENSION"] != "")
            {
                masterUnitViewModel.MAST_UNIT_DIMENSION = Convert.ToInt32(frmCollect["MAST_UNIT_DIMENSION"]);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterUnit(masterUnitViewModel, ref message))
                    {
                        message = message == string.Empty ? "Unit details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Unit details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterUnit", masterUnitViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Unit details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// DeleteMasterUnit() action is used to Delete Unit Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterUnit(String parameter, String hash, String key)
        {
            objBAL = new MasterBAL();
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteMasterUnit(Convert.ToInt32(decryptedParameters["UnitID"].ToString()), ref message))
                    {
                        message = "Unit details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Unit details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                message = "You can not delete this Unit details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Unit details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Unit Actions

        #region Scour Foundation Actions

        /// <summary>
        /// MasterScourFoundationType() action is used to display Scour Foundation Type Add/Edit
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterScourFoundationType()
        {
            return View();
        }

        /// <summary>
        /// Display Scour Foundation Type List
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>


        public ActionResult GetMasterScourFoundationTypeList(int? page, int? rows, string sidx, string sord)
        {

            String searchParameters = String.Empty;
            string SfType = String.Empty;
            long totalRecords;

            try
            {
                objBAL = new MasterBAL();

                if (!string.IsNullOrEmpty(Request.Params["SfTypeCode"]))
                {
                    SfType = Request.Params["SfTypeCode"].Replace('+', ' ').Trim();
                }

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListMasterScourFoundationType(SfType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        /// <summary>
        /// Display Scour Foundation Type Add Data Entry form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterScourFoundationType()
        {
            return PartialView("AddEditMasterScourFoundationType", new MasterScourFoundationTypeViewModel());
        }

        /// <summary>
        /// Save Scour Foundation Type Details
        /// </summary>
        /// <param name="masterScourFoundationTypeViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterScourFoundationType(MasterScourFoundationTypeViewModel masterScourFoundationTypeViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterScourFoundationType(masterScourFoundationTypeViewModel, ref message))
                    {
                        message = message == string.Empty ? "Scour/Foundation details saved successfully." : message;
                        status = true;
                    }
                    else
                    {

                        message = message == string.Empty ? "Scour/Foundation details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterScourFoundationType", masterScourFoundationTypeViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Scour/Foundation details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Display Scour Foundation Type Details in Edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterScourFoundationType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MasterScourFoundationTypeViewModel masterScourFoundationTypeViewMode = objBAL.GetScourFoundationDetails_ByScourFoundationCode(Convert.ToInt32(decryptedParameters["ScourID"].ToString()));
                    if (masterScourFoundationTypeViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Scour/Foundation details not exist.");
                        return PartialView("AddEditMasterScourFoundationType", new MasterScourFoundationTypeViewModel());
                    }
                    return PartialView("AddEditMasterScourFoundationType", masterScourFoundationTypeViewMode);

                }
                return PartialView("AddEditMasterScourFoundationType", new MasterScourFoundationTypeViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Scour/Foundation  details not exist.");
                return PartialView("AddEditMasterScourFoundationType", new MasterScourFoundationTypeViewModel());
            }
        }

        /// <summary>
        /// EditMasterScourFoundationType action is used to Update Scour Foundation Type Details
        /// </summary>
        /// <param name="masterScourFoundationTypeViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterScourFoundationType(MasterScourFoundationTypeViewModel masterScourFoundationTypeViewModel)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterScourFoundationType(masterScourFoundationTypeViewModel, ref message))
                    {
                        message = message == string.Empty ? "Scour/Foundation details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Scour/Foundation details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterScourFoundationType", masterScourFoundationTypeViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Scour/Foundation details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Delete Scour Foundation Type Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterScourFoundationType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                objBAL = new MasterBAL();

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteMasterScourFoundationType(Convert.ToInt32(decryptedParameters["ScourID"].ToString()), ref message))
                    {
                        message = "Scour/Foundation details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Scour/Foundation details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                message = "You can not delete this Scour/Foundation details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Scour/Foundation details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// SearchScourFoundation() action is used to Search Scour Foundation details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchScourFoundation()
        {
            try
            {
                IMasterDAL objDAL = new MasterDAL();

                List<SelectListItem> scourFoundationList = objDAL.GetScourFoundation();

                scourFoundationList.Insert(0, new SelectListItem() { Text = "All", Value = "" });

                ViewData["SfType"] = new SelectList(scourFoundationList, "Value", "Text");

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["SfType"] = null;
            }
            return PartialView("SearchScourFoundation");
        }


        #endregion Scour Foundation Actions

        #region Contractor Class Type  Actions

        /// <summary>
        /// MasterContractorClassType() action is used to display Add/Edit Contractor class type And contractor List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterContractorClassType()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchContractorClassType()
        {
            int stateCode = PMGSYSession.Current.StateCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            ViewData["STATE"] = stateDd;
            return View();
        }




        /// <summary>
        /// GetMasterContractorClassTypeList() action display Contractor class Type List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetMasterContractorClassTypeList(int? page, int? rows, string sidx, string sord)
        {
            objBAL = new MasterBAL();
            //int contStateCode = PMGSYSession.Current.StateCode;
            int contStateCode = 0;

            long totalRecords;

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["StateCode"]))
            {
                contStateCode = Convert.ToInt32(Request.Params["StateCode"]);
            }
            var jsonData = new
            {
                rows = objBAL.ListMasterContractorClassType(contStateCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AddEditMasterContractorClassType() Display Add/Edit Constrator Class Type Data Entry Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterContractorClassType()
        {
            CommonFunctions objCommonFunction = new CommonFunctions();
            List<SelectListItem> stateList = objCommonFunction.PopulateStates(true);
            stateList.RemoveAt(0);
            stateList.Insert(0, new SelectListItem() { Value = "0", Text = "--Select--" });
            ViewData["State"] = new SelectList(stateList as IEnumerable<SelectListItem>, "Value", "Text");

            MasterContractorClassTypeViewModel masterContractorClassTypeViewModel = new MasterContractorClassTypeViewModel();

            masterContractorClassTypeViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            return PartialView("AddEditMasterContractorClassType", masterContractorClassTypeViewModel);
        }

        /// <summary>
        /// AddMasterContractorClassType () save Contractor Class Type Form Data into data base
        /// </summary>
        /// <param name="masterContractorClassTypeViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterContractorClassType(MasterContractorClassTypeViewModel masterContractorClassTypeViewModel)
        {
            bool status = false;
            try
            {
                IMasterBAL objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    //int contStateCode = PMGSYSession.Current.StateCode;

                    if (objBAL.AddMasterContractorClassType(masterContractorClassTypeViewModel, ref message))
                    {
                        message = message == string.Empty ? "Contractor Class details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Contractor Class details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterContractorClassType", masterContractorClassTypeViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Contractor Class details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditMasterContractorClassType() action display Contractor Class type form in edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterContractorClassType(String parameter, String hash, String key)
        {
            IMasterBAL objBAL = new MasterBAL();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                CommonFunctions objCommonFunction = new CommonFunctions();
                ViewData["State"] = new SelectList(objCommonFunction.PopulateStates(true) as IEnumerable<SelectListItem>, "Value", "Text");

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MasterContractorClassTypeViewModel masterContractorClassTypeViewMode = objBAL.GetContractorClassDetails_ByClassCode(Convert.ToInt32(decryptedParameters["ContClassID"].ToString()));
                    if (masterContractorClassTypeViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Contractor Class details not exist.");
                        return PartialView("AddEditMasterContractorClassType", new MasterContractorClassTypeViewModel());
                    }
                    return PartialView("AddEditMasterContractorClassType", masterContractorClassTypeViewMode);

                }
                return PartialView("AddEditMasterContractorClassType", new MasterContractorClassTypeViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Contractor Class details not exist.");
                return PartialView("AddEditMasterContractorClassType", new MasterContractorClassTypeViewModel());
            }
        }

        /// <summary>
        /// EditMasterContractorClassType() action is used to Update Contractor Class Type Details
        /// </summary>
        /// <param name="masterContractorClassTypeViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterContractorClassType(MasterContractorClassTypeViewModel masterContractorClassTypeViewModel)
        {
            IMasterBAL objBAL = new MasterBAL();
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    //int contStateCode = PMGSYSession.Current.StateCode;

                    if (objBAL.EditMasterContractorClassType(masterContractorClassTypeViewModel, ref message))
                    {
                        message = message == string.Empty ? "Contractor Class details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Contractor Class details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterContractorClassType", masterContractorClassTypeViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Contractor Class details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// DeleteMasterContractorClassType() action is used to delete Contractor class Type Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterContractorClassType(String parameter, String hash, String key)
        {
            IMasterBAL objBAL = new MasterBAL();
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteMasterContractorClassType(Convert.ToInt32(decryptedParameters["ContClassID"].ToString()), ref message))
                    {
                        message = "Contractor Class  details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Contractor Class details." : message;

                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "You can not delete this Contractor Class  details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Contractor Class  details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion Contractor Class Type  Actions

        #region Contractor Registration Actions

        //new actions added by Vikram

        /// <summary>
        /// returns the list of registered contractors
        /// </summary>
        /// <returns></returns>

        public ActionResult ListContractorRegistration()
        {
            return View("ListContractorRegistration");
        }


        public ActionResult SearchContractorRegistration()
        {
            CommonFunctions objCommon = new CommonFunctions();
            objDAL = new MasterDAL();
            //Modified By Abhishek kamble 20-Feb-2014 start
            //ViewData["State"] = objCommon.PopulateStates(true);                 
            List<SelectListItem> lstStates = new List<SelectListItem>();
            lstStates = objCommon.PopulateStates(true);
            lstStates.RemoveAt(0);
            lstStates.Insert(0, new SelectListItem { Value = "0", Text = "All State" });
            ViewData["State"] = lstStates;
            //Modified By Abhishek kamble 20-Feb-2014 end

            // ViewData["District"] = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode);
            List<SelectListItem> list = new List<SelectListItem>();
            list.Insert(0, new SelectListItem { Value = "A", Text = "Active", Selected = true });
            list.Insert(1, new SelectListItem { Value = "I", Text = "Inactive" });
            //list.Insert(2, new SelectListItem { Value = "I", Text = "Expired" });
            List<SelectListItem> lstContractorStatus = new List<SelectListItem>();
            lstContractorStatus.Insert(0, new SelectListItem { Value = "A", Text = "Active", Selected = true });
            lstContractorStatus.Insert(1, new SelectListItem { Value = "I", Text = "Inactive" });
            lstContractorStatus.Insert(2, new SelectListItem { Value = "B", Text = "Blacklisted" });
            lstContractorStatus.Insert(3, new SelectListItem { Value = "E", Text = "Expired" });
            ViewData["Status"] = list;
            ViewData["ContractorStatus"] = lstContractorStatus;
            ViewData["ClassType"] = objDAL.PopulateClassTypes();
            return PartialView("SearchContractorRegistration");
        }


        public ActionResult ViewRegistrationDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            objBAL = new MasterBAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int contractorId = Convert.ToInt32(decryptedParameters["ContRegID"]);
                int registrationCode = Convert.ToInt32(decryptedParameters["ContRegCode"]);
                MasterContractorRegistrationViewModel registrationModel = objBAL.GetContRegDetails_ByConId_RegCode(contractorId, registrationCode);
                registrationModel.EncryptedContractorCode = URLEncrypt.EncryptParameters1(new string[] { "ContractorCode =" + registrationModel.MAST_CON_ID.ToString().Trim() });
                return PartialView("ViewRegistrationDetails", registrationModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ViewRegistrationDetails", new MasterContractorRegistrationViewModel());
            }
        }


        public ActionResult ViewContractorDetails(string id)
        {
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = id.Split('/');
            objBAL = new MasterBAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int contractorId = Convert.ToInt32(decryptedParameters["ContRegID"]);
                MasterContractorViewModel conModel = objBAL.EditContractor(contractorId);
                return PartialView("ViewContractorDetails", conModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ViewContractorDetails", new MasterContractorViewModel());
            }
        }

        [HttpPost]

        public JsonResult GetContractorsByStateCode(string stateCode)
        {
            try
            {
                List<SelectListItem> lstContractors = new List<SelectListItem>();
                IMasterDAL objDAL = new MasterDAL();
                lstContractors = objDAL.PopulateContractors(Convert.ToInt32(stateCode.Trim()));
                //classTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
                //return Json(new SelectList(lstContractors, "Value", "Text"));
                var jsonResult = Json(new SelectList(lstContractors, "Value", "Text"));
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        public ActionResult GetContractorRegList(int? page, int? rows, string sidx, string sord)
        {
            objBAL = new MasterBAL();
            int stateCode = 0;
            int districtCode = 0;
            string status = string.Empty;
            string contractorName = string.Empty;
            string conStatus = string.Empty;
            string panNumber = string.Empty;
            int classType = 0;
            string regNo = string.Empty;
            string companyName = string.Empty;
            int contractorId = 0;
            long totalRecords;


            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!(string.IsNullOrEmpty(Request.Params["stateCode"])))
            {
                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }

            //if (!(string.IsNullOrEmpty(Request.Params["districtCode"])))
            //{
            //    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
            //}

            if (!(string.IsNullOrEmpty(Request.Params["status"])))
            {
                status = Request.Params["status"];
            }

            if (!(string.IsNullOrEmpty(Request.Params["contractorName"])))
            {
                contractorName = Request.Params["contractorName"];
            }

            if (!(string.IsNullOrEmpty(Request.Params["conStatus"])))
            {
                conStatus = Request.Params["conStatus"];
            }

            if (!(string.IsNullOrEmpty(Request.Params["panNo"])))
            {
                panNumber = Request.Params["panNo"];
            }

            if (!(string.IsNullOrEmpty(Request.Params["classType"])))
            {
                classType = Convert.ToInt32(Request.Params["classType"]);
            }

            if (!(string.IsNullOrEmpty(Request.Params["regNo"])))
            {
                regNo = Request.Params["regNo"];
            }

            if (!(string.IsNullOrEmpty(Request.Params["companyName"])))
            {
                companyName = Request.Params["companyName"];
            }

            districtCode = (PMGSYSession.Current.DistrictCode > 0) ? PMGSYSession.Current.DistrictCode : 0;

            var jsonData = new
            {
                rows = objBAL.ListMasterContractorReg(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, stateCode, districtCode, status, contractorName, conStatus, panNumber, classType, regNo, companyName, contractorId),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetViewContractorRegistrationDetails(int? page, int? rows, string sidx, string sord)
        {


            objBAL = new MasterBAL();

            int ConId = 0;
            int RegId = 0;
            long totalRecords;
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!(string.IsNullOrEmpty(Request.Params["ConId"])))
            {
                ConId = Convert.ToInt32(Request.Params["ConId"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["RegId"])))
            {
                RegId = Convert.ToInt32(Request.Params["RegId"]);
            }

            var jsonData = new
            {
                rows = objBAL.GetViewContractorRegistrationListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, ConId, RegId),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// MasterContractorReg() action is used to display Contractor Registration Form and List Page
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterContractorReg(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                //required
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int ContCode = Convert.ToInt32(decryptedParameters["ContractorCode"]);

                PMGSYSession.Current.MastConCode = ContCode; //required

                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return View();
            }
        }

        /// <summary>
        /// GetMasterContractorRegList() action is used to get Contractor Registration Grid Data
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>

        //public ActionResult GetMasterContractorRegList(int? page, int? rows, string sidx, string sord)
        //{
        //    objBAL = new MasterBAL();

        //    int contStateCode = PMGSYSession.Current.StateCode;
        //    int contId = PMGSYSession.Current.MastConCode;

        //    long totalRecords;


        //    using (CommonFunctions commonFunction = new CommonFunctions())
        //    {
        //        if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
        //        {
        //            return null;
        //        }
        //    }

        //    var jsonData = new
        //    {
        //        rows = objBAL.ListMasterContractorReg(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords,stateCode, districtCode,status,contractorName),
        //        total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
        //        page = Convert.ToInt32(page),
        //        records = totalRecords
        //    };
        //    return Json(jsonData, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// AddEditMasterContractorReg() action is used to display Contractor Registration Add Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]

        public ActionResult AddEditMasterContractorReg()
        {
            // new change by vikram
            MasterContractorRegistrationViewModel model = new MasterContractorRegistrationViewModel();
            objDAL = new MasterDAL();
            model.MAST_REG_STATE = PMGSYSession.Current.StateCode;
            ViewData["ClassType"] = objDAL.GetAllContClassByRegState(PMGSYSession.Current.StateCode);
            ViewData["Contractor"] = objDAL.PopulateContractors(PMGSYSession.Current.StateCode);

            return PartialView("AddEditMasterContractorReg", model);
        }
        /// <summary>
        /// AddMasterContractorReg() action is used to Save Contractor Registration Details 
        /// </summary>
        /// <param name="masterContractorRegViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterContractorReg(MasterContractorRegistrationViewModel masterContractorRegViewModel)
        {
            bool status = false;
            objBAL = new MasterBAL();
            try
            {
                if (ModelState.IsValid)
                {
                    if (masterContractorRegViewModel.FUND_TYPE == "P" && (masterContractorRegViewModel.MAST_CON_REG_NO == null || masterContractorRegViewModel.MAST_CON_REG_NO == ""))
                    {
                        return Json(new { success = status, message = "Registration Number is Required" });
                    }
                    if (objBAL.AddMasterContractorReg(masterContractorRegViewModel, ref message))
                    {
                        message = message == string.Empty ? "Contractor/Supplier Registration details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Contractor/Supplier Registration details not saved." : message;
                    }
                }
                else
                {
                    // return PartialView("AddEditMasterContractorReg", masterContractorRegViewModel);
                    return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Contractor/Supplier Registration details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditMasterContractorReg() action is used to display Contractor registration Form in edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterContractorReg(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            objBAL = new MasterBAL();
            objDAL = new MasterDAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int contRegId = Convert.ToInt32(decryptedParameters["ContRegID"]);
                int contRegCode = Convert.ToInt32(decryptedParameters["ContRegCode"]);



                if (decryptedParameters.Count() > 0)
                {
                    MasterContractorRegistrationViewModel masterContractorRegViewMode = objBAL.GetContRegDetails_ByConId_RegCode(contRegId, contRegCode);
                    if (masterContractorRegViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Contractor/Supplier Registration details not exist.");
                        return PartialView("AddEditMasterContractorReg", new MasterContractorRegistrationViewModel());
                    }
                    if (PMGSYSession.Current.RoleCode == 23 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56)///Changes for RCPLWEITNO
                    {
                        // ViewData["Contractor"] = objDAL.PopulateContractorsForEdit(PMGSYSession.Current.StateCode);
                        ViewData["Contractor"] = objDAL.PopulateContractors(masterContractorRegViewMode.MAST_REG_STATE);
                        ViewData["ClassType"] = objDAL.GetAllContClassByRegState(masterContractorRegViewMode.MAST_REG_STATE);
                    }
                    else
                    {
                        ViewData["Contractor"] = objDAL.PopulateContractorsForEdit(PMGSYSession.Current.StateCode);
                        //ViewData["Contractor"] = objDAL.PopulateContractors(PMGSYSession.Current.StateCode);
                        ViewData["ClassType"] = objDAL.GetAllContClassByRegState(PMGSYSession.Current.StateCode);
                    }

                    return PartialView("AddEditMasterContractorReg", masterContractorRegViewMode);

                }
                return PartialView("AddEditMasterContractorReg", new MasterContractorRegistrationViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Contractor/Supplier Registration details not exist.");
                return PartialView("AddEditMasterContractorReg", new MasterContractorRegistrationViewModel());
            }
        }

        /// <summary>
        /// EditMasterContractorReg() action is used to Update Constrator Registration Details
        /// </summary>
        /// <param name="masterContractorRegViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterContractorReg(MasterContractorRegistrationViewModel masterContractorRegViewModel)
        {
            objBAL = new MasterBAL();
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterContractorReg(masterContractorRegViewModel, ref message))
                    {
                        message = message == string.Empty ? "Contractor/Supplier Registration details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Contractor/Supplier Registration details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterContractorReg", masterContractorRegViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Contractor/Supplier Registration details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Added on 25-01-2022 by Srishti Tyagi
        [HttpGet]
        public ActionResult EditMasterContractorRegFundType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            objBAL = new MasterBAL();
            objDAL = new MasterDAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int contRegId = Convert.ToInt32(decryptedParameters["ContRegID"]);
                int contRegCode = Convert.ToInt32(decryptedParameters["ContRegCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    MasterContractorRegistrationViewModel masterContractorRegViewMode = objBAL.GetContRegDetails_ByConId_RegCode(contRegId, contRegCode);
                    if (masterContractorRegViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Contractor/Supplier Registration details not exist.");
                        return PartialView("EditMasterContractorRegFundType", new MasterContractorRegistrationViewModel());
                    }
                    if (PMGSYSession.Current.RoleCode == 23 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56)///Changes for RCPLWEITNO
                    {
                        ViewData["Contractor"] = objDAL.PopulateContractors(masterContractorRegViewMode.MAST_REG_STATE);
                        ViewData["ClassType"] = objDAL.GetAllContClassByRegState(masterContractorRegViewMode.MAST_REG_STATE);
                    }
                    else
                    {
                        ViewData["Contractor"] = objDAL.PopulateContractorsForEdit(PMGSYSession.Current.StateCode);
                        ViewData["ClassType"] = objDAL.GetAllContClassByRegState(PMGSYSession.Current.StateCode);
                    }

                    if (masterContractorRegViewMode.FUND_TYPE == null)
                    {
                        masterContractorRegViewMode.FUND_TYPE = "P";
                    }

                    return PartialView("EditMasterContractorRegFundType", masterContractorRegViewMode);

                }
                return PartialView("EditMasterContractorRegFundType", new MasterContractorRegistrationViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Contractor/Supplier Registration details not exist.");
                return PartialView("AddEditMasterContractorReg", new MasterContractorRegistrationViewModel());
            }
        }

        // Added on 25-01-2022 by Srishti Tyagi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterContractorRegFundType(MasterContractorRegistrationViewModel masterContractorRegViewModel)
        {
            objBAL = new MasterBAL();
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterContractorRegFundType(masterContractorRegViewModel, ref message))
                    {
                        message = message == string.Empty ? "Contractor/Supplier Registration details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Contractor/Supplier Registration details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("EditMasterContractorRegFundType", masterContractorRegViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Contractor/Supplier Registration details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// DeleteMasterContractorReg() action is used to Delete Contractor Registration Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>Returns success message if Details successfully Deleted else</returns>
        [HttpPost]
        public ActionResult DeleteMasterContractorReg(String parameter, String hash, String key)
        {
            objBAL = new MasterBAL();
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int contRegId = Convert.ToInt32(decryptedParameters["ContRegID"]);
                int contRegCode = Convert.ToInt32(decryptedParameters["ContRegCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteMasterContractorReg(contRegId, contRegCode, ref message))
                    {
                        message = "Contractor/Supplier Registration details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Contractor/Supplier Registration details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                message = "You can not delete this Contractor/Supplier Registration details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Contractor/Supplier Registration details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetContractorsByPan(string stateCode)
        {
            try
            {
                int state = Convert.ToInt32(stateCode.Split('$')[0]);
                string panSearch = stateCode.Split('$')[1];
                List<SelectListItem> lstContractors = new List<SelectListItem>();
                IMasterDAL objDAL = new MasterDAL();
                lstContractors = objDAL.PopulateContractorsByPan(state, panSearch);
                return Json(new SelectList(lstContractors, "Value", "Text"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        public ActionResult SearchContractorByPan()
        {
            try
            {
                return PartialView();
            }
            catch (Exception)
            {
                return null;
            }
        }


        #endregion Contractor Registration Actions

        #region Vidhan sabha Term Actions

        /// <summary>
        /// MasterVidhanSabhaTerm() is used to Display Vidhan Sabha Term Add/Edit Form and Grid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterVidhanSabhaTerm()
        {
            return View();
        }

        /// <summary>
        /// GetMasterVidhanSabhaTermList display Vidhan Sabha Term List on Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetMasterVidhanSabhaTermList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            int stateCode = 0;

            try
            {
                objBAL = new MasterBAL();
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

                var jsonData = new
                {
                    rows = objBAL.ListVidhanSabhaTerm(stateCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        /// <summary>
        /// AddEditMasterVidhanSabhaTerm() action is used to Dispaly Vidhan Sabha Term Data entry Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterVidhanSabhaTerm()
        {
            return PartialView("AddEditMasterVidhanSabhaTerm", new MasterVidhanSabhaTermViewModel());
        }

        /// <summary>
        /// AddMasterMasterVidhanSabhaTerm() action is used to Save Vidhan Sabha Details
        /// </summary>
        /// <param name="masterVidhanSabhaTermViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterMasterVidhanSabhaTerm(MasterVidhanSabhaTermViewModel masterVidhanSabhaTermViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();

                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterVidhanSabhaTerm(masterVidhanSabhaTermViewModel, ref message))
                    {
                        message = message == string.Empty ? "Vidhan Sabha Term details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Vidhan Sabha Term details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterVidhanSabhaTerm", masterVidhanSabhaTermViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Vidhan Sabha Term details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditMasterVidhanSabhaTerm() action is used to Display Vidhan Sabha Form in Edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterVidhanSabhaTerm(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int stateCode = Convert.ToInt32(decryptedParameters["StateID"]);
                int VidhanSabhaId = Convert.ToInt32(decryptedParameters["VidhanSabhaId"]);

                if (decryptedParameters.Count() > 0)
                {
                    MasterVidhanSabhaTermViewModel masterVidhanSabhaViewMode = objBAL.GetVidhanSabhaTerm_ByStateCode_TermId(stateCode, VidhanSabhaId);
                    if (masterVidhanSabhaViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Vidhan Sabha Term details not exist.");
                        return PartialView("AddEditMasterVidhanSabhaTerm", new MasterVidhanSabhaTermViewModel());
                    }
                    return PartialView("AddEditMasterVidhanSabhaTerm", masterVidhanSabhaViewMode);
                }
                return PartialView("AddEditMasterVidhanSabhaTerm", new MasterVidhanSabhaTermViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Vidhan Sabha Term details not exist.");
                return PartialView("AddEditMasterVidhanSabhaTerm", new MasterVidhanSabhaTermViewModel());
            }
        }


        /// <summary>
        /// Update Vidhan Sabha Term Details
        /// </summary>
        /// <param name="masterVidhanSabhaTermViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterVidhanSabhaTerm(MasterVidhanSabhaTermViewModel masterVidhanSabhaTermViewModel)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.EditMasterVidhanSabhaTerm(masterVidhanSabhaTermViewModel, ref message))
                    {
                        message = message == string.Empty ? "Vidhan Sabha Term details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Vidhan Sabha Term details not updated." : message;
                    }

                }
                else
                {
                    return PartialView("AddEditMasterVidhanSabhaTerm", masterVidhanSabhaTermViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Vidhan Sabha Term details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// DeleteMasterVidhanSabhaTerm() action is used to Delete Vidhan Sabha Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterVidhanSabhaTerm(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int stateCode = Convert.ToInt32(decryptedParameters["StateID"]);
                int VidhanSabhaId = Convert.ToInt32(decryptedParameters["VidhanSabhaId"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteVidhanSabhaTerm(stateCode, VidhanSabhaId, ref message))
                    {
                        message = "Vidhan Sabha Term details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Vidhan Sabha Term details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                message = "You can not delete this Vidhan Sabha Term details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Vidhan Sabha Term details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// SearchVidhanSabhaTerm() action is used to Search Vidhan Sabha Term Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchVidhanSabhaTerm()
        {
            try
            {
                IMasterDAL objDAL = new MasterDAL();
                List<MASTER_STATE> stateList = objDAL.GetAllStateNames();
                stateList.Insert(0, new MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" });
                ViewData["StateList"] = new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["StateList"] = null;
            }
            return PartialView("SearchVidhanSabhaTerm");
        }

        #endregion Vidhan sabha Term Actions

        #region Region Actions

        /// <summary>
        /// Display Region Add/Edit Form And Grid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterRegion()
        {
            return View();
        }


        //changes by Koustubh Nakate on 10-05-2013 to maintain state 
        /// <summary>
        /// Diaplay Get Region List For Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetMasterRegionList(int? page, int? rows, string sidx, string sord)
        {
            objBAL = new MasterBAL();
            String searchParameters = string.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();
            long totalRecords;
            int stateCode = 0;
            try
            {
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
                var jsonData = new
                {
                    rows = objBAL.ListMasterRegion(stateCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        /// <summary>
        /// AddEditMasterRegion() display Region Form for Add/Edit and Grid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterRegion()
        {
            return PartialView("AddEditMasterRegion", new MasterRegionViewModel());
        }

        /// <summary>
        /// Save Region Details
        /// </summary>
        /// <param name="masterRegionViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterRegion(MasterRegionViewModel masterRegionViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.AddMasterRegion(masterRegionViewModel, ref message))
                    {
                        message = message == string.Empty ? "Region details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Region details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterRegion", masterRegionViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Region details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Dispaly Region Form in Edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterRegion(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int regionCode = Convert.ToInt32(decryptedParameters["RegionId"]);

                if (decryptedParameters.Count() > 0)
                {
                    MasterRegionViewModel masterRegionViewMode = objBAL.GetMasterRegion_ByRegionCode(regionCode);
                    if (masterRegionViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Region details not exist.");
                        return PartialView("AddEditMasterRegion", new MasterRegionViewModel());
                    }
                    return PartialView("AddEditMasterRegion", masterRegionViewMode);

                }
                return PartialView("AddEditMasterRegion", new MasterRegionViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Region details not exist.");
                return PartialView("AddEditMasterRegion", new MasterRegionViewModel());
            }
        }

        /// <summary>
        /// Update Region Details
        /// </summary>
        /// <param name="masterRegionViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterRegion(MasterRegionViewModel masterRegionViewModel)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.EditMasterRegion(masterRegionViewModel, ref message))
                    {
                        message = message == string.Empty ? "Region details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Region details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterRegion", masterRegionViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Region details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //change by Koustubh Nakate on 10-05-2013  
        /// <summary>
        /// Delete Region Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeleteMasterRegion(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.DeleteMasterRegion(Convert.ToInt32(decryptedParameters["RegionId"].ToString()), ref message))
                    {
                        message = "Region details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Region details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this Region details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Region details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion Region Actions

        #region Autonomous Body Actions

        /// <summary>
        /// Display Autonomous Body Add/Edit Form and Grid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterAdminAutonomousBody()
        {
            return View();
        }

        /// <summary>
        /// Get AutonomousBody Grid Data to display
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetMasterAdminAutonomousBodyList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            objBAL = new MasterBAL();
            int stateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;

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


            var jsonData = new
            {
                rows = objBAL.ListMasterAdminAutonomousBody(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, stateCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1), //totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display AdminAutonomousBody Data entry Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterAdminAutonomousBody()
        {
            MasterAdminAutonomousBodyViewModel masterAdminAutonomousBodyViewModel = new MasterAdminAutonomousBodyViewModel();

            string rolename = PMGSYSession.Current.RoleName;
            masterAdminAutonomousBodyViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;

            List<MASTER_STATE> stateList = new List<MASTER_STATE>();
            IMasterDAL objDAL = new MasterDAL();
            stateList = objDAL.GetAllStateNames();

            //
            db = new PMGSYEntities();
            List<int> stateCodes = db.ADMIN_AUTONOMOUS_BODY.Select(s => s.MAST_STATE_CODE).ToList();

            stateList = (from data in stateList
                         where !stateCodes.Contains(data.MAST_STATE_CODE)
                         select data
                          ).ToList<MASTER_STATE>();

            masterAdminAutonomousBodyViewModel.lstState = new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");


            return PartialView("AddEditMasterAdminAutonomousBody", masterAdminAutonomousBodyViewModel);
        }

        /// <summary>
        /// Save AdminAutonomousBody Details
        /// </summary>
        /// <param name="masterAdminAutonomousBodyViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterAdminAutonomousBody(MasterAdminAutonomousBodyViewModel masterAdminAutonomousBodyViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();

                    if (objBAL.AddMasterAdminAutonomousBody(masterAdminAutonomousBodyViewModel, ref message))
                    {
                        message = message == string.Empty ? "Autonomous Body details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Autonomous Body details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterAdminAutonomousBody", masterAdminAutonomousBodyViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Autonomous Body details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Display AutonomousBody details in Edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterAdminAutonomousBody(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int stateCode = Convert.ToInt32(decryptedParameters["AutoBodyCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    objBAL = new MasterBAL();
                    MasterAdminAutonomousBodyViewModel masterAutonomousBodyViewMode = objBAL.GetMasterAdminAutonomousBodyViewModel_ByStateCode(stateCode);
                    if (masterAutonomousBodyViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Autonomous Body details not exist.");
                        return PartialView("AddEditMasterAdminAutonomousBody", new MasterRegionViewModel());
                    }
                    return PartialView("AddEditMasterAdminAutonomousBody", masterAutonomousBodyViewMode);

                }
                return PartialView("AddEditMasterAdminAutonomousBody", new MasterRegionViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Autonomous Body details not exist.");
                return PartialView("AddEditMasterAdminAutonomousBody", new MasterRegionViewModel());
            }
        }



        /// <summary>
        /// Upadate Autonomous Body Details
        /// </summary>
        /// <param name="masterAutonomousBodyViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterAdminAutonomousBody(MasterAdminAutonomousBodyViewModel masterAutonomousBodyViewModel)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.EditMasterAdminAutonomousBody(masterAutonomousBodyViewModel, ref message))
                    {
                        message = message == string.Empty ? "Autonomous Body details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Autonomous Body details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterAdminAutonomousBody", masterAutonomousBodyViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Autonomous Body details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete Autonomous Body Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterAutonomousBody(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int stateCode = Convert.ToInt32(decryptedParameters["AutoBodyCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.DeleteMasterAdminAutonomousBody(stateCode, ref message))
                    {
                        message = "Autonomous Body details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Autonomous Body details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this Autonomous Body details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Autonomous Body details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to search Admin Autonomous Body.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchAdminAutonomousBody()
        {
            try
            {
                objDAL = new MasterDAL();
                List<MASTER_STATE> lstState = objDAL.GetAllStates();
                lstState.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" });
                ViewData["StateList"] = new SelectList(lstState, "MAST_STATE_CODE", "MAST_STATE_NAME", PMGSYSession.Current.StateCode);

                return PartialView("SearchAdminAutonomousBody");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["StateList"] = null;

            }
            return PartialView("SearchAdminAutonomousBody");
        }


        #endregion Autonomous Body Actions

        #region  Mp Members Actions

        /// <summary>
        /// Display MP Member Add Form and Grid
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterMpMember(String parameter, String hash, String key)
        {
            return View();
        }

        /// <summary>
        /// Get Mp Member List to display on grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>

        public ActionResult GetMasterMpMemberList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            long totalRecords;
            int termCode = 0;
            int constituencyCode = 0;
            int stateCode = 0;
            string memberName = String.Empty;
            objBAL = new MasterBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["termCode"]))
                {
                    termCode = Convert.ToInt32(Request.Params["termCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["constituencyCode"]))
                {
                    constituencyCode = Convert.ToInt32(Request.Params["constituencyCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["memberName"]))
                {
                    memberName = Request.Params["memberName"].Replace('+', ' ').Trim();
                }

                var jsonData = new
                {
                    rows = objBAL.ListMpMember(termCode, stateCode, constituencyCode, memberName, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        /// <summary>
        /// Dispaly MP Member data Entry Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterMpMember()
        {
            return PartialView("AddEditMasterMpMember", new MasterMpMembersViewModel());
        }

        /// <summary>
        /// Save MP Member details
        /// </summary>
        /// <param name="masterMpMemberViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterMpMember(MasterMpMembersViewModel masterMpMemberViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();

                    if (objBAL.AddMasterMpMember(masterMpMemberViewModel, ref message))
                    {
                        message = message == string.Empty ? "MP Member details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "MP Member details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterMpMember", masterMpMemberViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "MP Member details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Display MP Member Form in Edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterMpMember(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int termCode = Convert.ToInt32(decryptedParameters["TermCode"]);
                int constCode = Convert.ToInt32(decryptedParameters["ConstCode"]);
                int memberCode = Convert.ToInt32(decryptedParameters["MemberCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    objBAL = new MasterBAL();
                    MasterMpMembersViewModel masterMpMemberViewMode = objBAL.GetMpMember_ByTerm_ConstCode_MemberId(termCode, constCode, memberCode);
                    if (masterMpMemberViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "MP Member details not exist.");
                        return PartialView("AddEditMasterMpMember", new MasterMpMembersViewModel());
                    }
                    return PartialView("AddEditMasterMpMember", masterMpMemberViewMode);
                }
                return PartialView("AddEditMasterMpMember", new MasterMpMembersViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "MP Member details not exist.");
                return PartialView("AddEditMasterMpMember", new MasterMpMembersViewModel());
            }
        }

        /// <summary>
        /// Update MP Member Details
        /// </summary>
        /// <param name="masterMpMemberViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterMpMember(MasterMpMembersViewModel masterMpMemberViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.EditMasterMpMember(masterMpMemberViewModel, ref message))
                    {
                        message = message == string.Empty ? "MP Member details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "MP Member details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterMpMember", masterMpMemberViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "MP Member details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete MP Member Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterMpMember(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int termCode = Convert.ToInt32(decryptedParameters["TermCode"]);
                int constCode = Convert.ToInt32(decryptedParameters["ConstCode"]);
                int memberCode = Convert.ToInt32(decryptedParameters["MemberCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.DeleteMpMember(termCode, constCode, memberCode, ref message))
                    {
                        message = "MP Member details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this MP Member details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "You can not delete this MP Member details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this MP Member details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Search MP Members Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchMpMembers()
        {
            try
            {
                IMasterDAL objDAL = new MasterDAL();
                db = new PMGSYEntities();
                int stateCode = PMGSYSession.Current.StateCode;
                List<MASTER_STATE> lstState = objDAL.GetAllStates();

                lstState.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" });

                ViewData["State"] = new SelectList(lstState, "MAST_STATE_CODE", "MAST_STATE_NAME");

                List<MASTER_LOK_SABHA_TERM> lockSabhaTermList = new List<MASTER_LOK_SABHA_TERM>();
                lockSabhaTermList = objDAL.GetAllLockSabhaTerms();
                lockSabhaTermList = lockSabhaTermList.OrderByDescending(a => a.MAST_LS_TERM).ToList();
                ViewData["Term"] = new SelectList(lockSabhaTermList, "MAST_LS_TERM", "MAST_LS_TERM");

                /// ViewData["ConstituencyNames"] = new SelectList(objDAL.GetAllMpConstituencyNames(), "MAST_MP_CONST_CODE", "MAST_MP_CONST_NAME");
                List<MASTER_MP_CONSTITUENCY> lstConstituency = new List<MASTER_MP_CONSTITUENCY>();
                //if (PMGSYSession.Current.StateCode > 0)
                //{
                //    lstConstituency = db.MASTER_MP_CONSTITUENCY.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode).ToList<MASTER_MP_CONSTITUENCY>();
                //}
                lstConstituency.Insert(0, new MASTER_MP_CONSTITUENCY { MAST_MP_CONST_CODE = 0, MAST_MP_CONST_NAME = "All Constituencies" });
                ViewData["ConstituencyNames"] = new SelectList(lstConstituency.Where(z => z.MAST_MP_CONST_ACTIVE == "Y").ToList(), "MAST_MP_CONST_CODE", "MAST_MP_CONST_NAME");

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["Term"] = null;
                ViewData["ConstituencyNames"] = null;
            }
            return PartialView("SearchMpMembers");
        }

        /// <summary>
        /// Method to get MP constituency list.
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMPConstituencyList(string stateCode)
        {
            db = new PMGSYEntities();
            if (!int.TryParse(stateCode, out outParam))
            {
                return Json(false);
            }
            int code = Convert.ToInt32(stateCode);
            List<MASTER_MP_CONSTITUENCY> list = db.MASTER_MP_CONSTITUENCY.Where(m => m.MAST_STATE_CODE == code && m.MAST_MP_CONST_ACTIVE == "Y").OrderBy(x => x.MAST_MP_CONST_NAME).ToList<MASTER_MP_CONSTITUENCY>();
            //list.Insert(0, new MASTER_MP_CONSTITUENCY { MAST_MP_CONST_CODE = 0, MAST_MP_CONST_NAME = "--All Constituencies--" });
            return Json(new SelectList(list, "MAST_MP_CONST_CODE", "MAST_MP_CONST_NAME"));
        }

        /// <summary>
        /// Search MP Member By Term Code
        /// </summary>
        /// <param name="termCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMemberNameByTermCode_Search(string termCode)
        {
            try
            {
                if (!int.TryParse(termCode, out outParam))
                {
                    return Json(false);
                }
                List<MASTER_MP_MEMBERS> nameList = new List<MASTER_MP_MEMBERS>();
                IMasterDAL objDAL = new MasterDAL();
                if (termCode == String.Empty)
                {
                    nameList = objDAL.GetAllMemberNamesByTermCode(0, true);
                    return Json(new SelectList(nameList, "MAST_LS_TERM", "MAST_MEMBER"));
                }
                nameList = objDAL.GetAllMemberNamesByTermCode(Convert.ToInt32(termCode.Trim()), true);
                return Json(new SelectList(nameList, "MAST_LS_TERM", "MAST_MEMBER"));

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        [HttpPost]
        public JsonResult GetStateCodeByMPConstiuencyCode()
        {
            db = new PMGSYEntities();
            int statCode = 0;
            try
            {
                int mpConstCode = 0;
                if (!string.IsNullOrEmpty(Request.Params["MpConstCode"]))
                {
                    mpConstCode = Convert.ToInt32(Request.Params["MpConstCode"]);
                }
                statCode = db.MASTER_MP_CONSTITUENCY.Where(a => a.MAST_MP_CONST_CODE == mpConstCode).Select(a => a.MAST_STATE_CODE).FirstOrDefault();
                return Json(new { statCode = statCode }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                // return Json(false);
                return Json(new { statCode = statCode }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }

        #endregion Mp Members Actions

        #region Quality Monitors  Actions

        ///<summary>
        /// MasterQualityMonitor() Action is used to Display Quality Monitor Add Form And Grid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MasterQualityMonitor()
        {
            return View();
        }

        /// <summary>
        /// Get QualityMonitor List to display on Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        //public ActionResult GetMasterQualityMonitorList(int? page, int? rows, string sidx, string sord)
        //{
        //    String searchParameters = String.Empty;
        //    Dictionary<string, string> parameters = new Dictionary<string, string>();
        //    long totalRecords;
        //    int stateCode = 0;
        //    int districtCode = 0;
        //    string isEmpanelled = String.Empty;
        //    string firstName = String.Empty;
        //    string qmTypeName = String.Empty;

        //    try
        //    {
        //        objBAL = new MasterBAL();
        //        using (CommonFunctions commonFunction = new CommonFunctions())
        //        {
        //            if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
        //            {
        //                return null;
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(Request.Params["QmTypeName"]))
        //        {
        //            qmTypeName = Request.Params["QmTypeName"].Replace('+', ' ').Trim();
        //        }

        //        if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
        //        {
        //            stateCode = Convert.ToInt32(Request.Params["stateCode"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
        //        {
        //            districtCode = Convert.ToInt32(Request.Params["districtCode"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["isEmpanelled"]))
        //        {
        //            isEmpanelled = Request.Params["isEmpanelled"];
        //        }


        //        var jsonData = new
        //        {
        //            rows = objBAL.ListQualityMonitor(qmTypeName, stateCode, districtCode, isEmpanelled, Request.Params["filters"], Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
        //            total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
        //            page = Convert.ToInt32(page),
        //            records = totalRecords
        //        };
        //        return Json(jsonData, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return null;
        //    }
        //}
        //By Anand
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
                objBAL = new MasterBAL();
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
                    rows = objBAL.ListQualityMonitor(qmTypeName, stateCode, districtCode, isEmpanelled, Request.Params["filters"], Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        //Get Cadre states on the selection of QM Type
        [HttpPost]
        public JsonResult GetCadreStates(String QMType)
        {
            try
            {
                IMasterDAL objDAL = new MasterDAL();
                if (QMType.Equals("S"))
                {
                    //  IMasterDAL objDAL = new MasterDAL();
                    List<MASTER_STATE> stateList = objDAL.GetAllStateNames();
                    stateList.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" });
                    return Json(new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME"));
                }
                else
                {
                    List<MASTER_STATE> stateList = objDAL.GetAllStateNames();
                    stateList.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States And Services" });
                    stateList.Insert(stateList.Count, new MASTER_STATE { MAST_STATE_CODE = 90, MAST_STATE_NAME = "Central Agency" });
                    stateList.Insert(stateList.Count, new MASTER_STATE { MAST_STATE_CODE = 91, MAST_STATE_NAME = "Central Goverment" });
                    return Json(new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME"));
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCadreStates");
                return null;

            }
        }



        /// <summary>
        /// Display Quality Monitor Add Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterQualityMonitor()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            short statecode = PMGSYSession.Current.StateCode;
            try
            {
                //added by abhinav pathak on 22-july-2019
                MasterAdminQualityMonitorViewModel model = new MasterAdminQualityMonitorViewModel();
                // Added By Rohit Borse on 03 March 2022
                model.EMPANELLED_REMOVE_LIST = new List<SelectListItem> {
                                                        //new SelectListItem{ Text = "Due to Misconduct", Value ="M"}, 
                                                        new SelectListItem{ Text = "Due to Age", Value ="A" },
                                                        new SelectListItem{ Text = "Due to Administrative Reasons", Value ="AR" },
                                                        new SelectListItem{ Text = "Due to Performance", Value ="P" },
                                                        new SelectListItem{ Text = "Others", Value ="O" },
                                                       };

                model.EMPANELLED_REMOVE_LIST.Insert(0, new SelectListItem { Value = "0", Text = "-- Select Reason --", Selected = true });
                //=====================
                var deptList = dbContext.ADMIN_DEPARTMENT.Where(x => x.MAST_ND_TYPE == "S" && x.MAST_STATE_CODE == statecode).ToList();

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
                return PartialView("AddEditMasterQualityMonitor", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Error occured while processing your request.");
                return PartialView("AddEditMasterQualityMonitor", new MasterAdminQualityMonitorViewModel());
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        /// <summary>
        /// Save Quality Monitor Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
         //public ActionResult AddMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMasterQualityMonitor(FormCollection formCollection)  //Changed model object to FormCollection by Shreyas on 06-01-2023
        {
            MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel = new MasterAdminQualityMonitorViewModel(); //added Shreyas-------
            bool status = false;



            #region Added by Shreyas to integrate PAN file uploding
            //masterQualityMonitorViewModel.ADMIN_QM_AADHAR_NO = formCollection["ADMIN_QM_AADHAR_NO"];//---
            masterQualityMonitorViewModel.ADMIN_QM_AADHAR_NO = formCollection["ADMIN_QM_AADHAR_NO"] == "" ? null : formCollection["ADMIN_QM_AADHAR_NO"];
            masterQualityMonitorViewModel.ADMIN_QM_ADDRESS1 = formCollection["ADMIN_QM_ADDRESS1"];//---
            masterQualityMonitorViewModel.ADMIN_QM_ADDRESS2 = formCollection["ADMIN_QM_ADDRESS2"];//---
            masterQualityMonitorViewModel.ADMIN_QM_BIRTH_DATE = formCollection["ADMIN_QM_BIRTH_DATE"];//---
            //masterQualityMonitorViewModel.ADMIN_QM_CODE= Int32.Parse(formCollection["ADMIN_QM_CODE"]);
            masterQualityMonitorViewModel.ADMIN_QM_DEG = formCollection["ADMIN_QM_DEG"];
            masterQualityMonitorViewModel.ADMIN_QM_DEMPANELLED_DATE = formCollection["ADMIN_QM_DEMPANELLED_DATE"];//---
            masterQualityMonitorViewModel.ADMIN_QM_DESG = (formCollection["ADMIN_QM_DESG"] == null ? 0 : Convert.ToInt32(formCollection["ADMIN_QM_DESG"]));//---
            masterQualityMonitorViewModel.ADMIN_QM_DOCPATH = formCollection["ADMIN_QM_DOCPATH"];
            masterQualityMonitorViewModel.ADMIN_QM_EMAIL = formCollection["ADMIN_QM_EMAIL"];//---
            masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED = formCollection["ADMIN_QM_EMPANELLED"];//---
            masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED_MONTH = (formCollection["ADMIN_QM_EMPANELLED_MONTH"] == null ? 0 : Convert.ToInt32(formCollection["ADMIN_QM_EMPANELLED_MONTH"]));//---
            masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED_REASON = formCollection["ADMIN_QM_EMPANELLED_REASON"];
            masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED_YEAR = (formCollection["ADMIN_QM_EMPANELLED_YEAR"] == null ? 0 : Convert.ToInt32(formCollection["ADMIN_QM_EMPANELLED_YEAR"]));//---
            masterQualityMonitorViewModel.ADMIN_QM_FAX = formCollection["ADMIN_QM_FAX"];//---
            masterQualityMonitorViewModel.ADMIN_QM_FNAME = formCollection["ADMIN_QM_FNAME"];//---
            masterQualityMonitorViewModel.ADMIN_QM_IMAGE = formCollection["ADMIN_QM_DEMPANELLED_DATE"];
            masterQualityMonitorViewModel.ADMIN_QM_LNAME = formCollection["ADMIN_QM_LNAME"];//---
            masterQualityMonitorViewModel.ADMIN_QM_MNAME = formCollection["ADMIN_QM_MNAME"];//---
            masterQualityMonitorViewModel.ADMIN_QM_MOBILE1 = formCollection["ADMIN_QM_MOBILE1"];//---
            masterQualityMonitorViewModel.ADMIN_QM_MOBILE2 = formCollection["ADMIN_QM_MOBILE2"];//---
            masterQualityMonitorViewModel.ADMIN_QM_PAN = formCollection["ADMIN_QM_PAN"];//---
            masterQualityMonitorViewModel.ADMIN_QM_PHONE1 = formCollection["ADMIN_QM_PHONE1"];//---
            masterQualityMonitorViewModel.ADMIN_QM_PHONE2 = formCollection["ADMIN_QM_PHONE2"];//---
            masterQualityMonitorViewModel.ADMIN_QM_PIN = formCollection["ADMIN_QM_PIN"];//---
            masterQualityMonitorViewModel.ADMIN_QM_REMARKS = formCollection["ADMIN_QM_REMARKS"];//---
            masterQualityMonitorViewModel.ADMIN_QM_STD1 = formCollection["ADMIN_QM_STD1"];//---
            masterQualityMonitorViewModel.ADMIN_QM_STD2 = formCollection["ADMIN_QM_STD2"];//---
            masterQualityMonitorViewModel.ADMIN_QM_STD_FAX = formCollection["ADMIN_QM_STD_FAX"];//---
            masterQualityMonitorViewModel.ADMIN_QM_TYPE = formCollection["ADMIN_QM_TYPE"];//---
            masterQualityMonitorViewModel.ADMIN_SERVICE_TYPE = PMGSYSession.Current.RoleCode == 5 ? "D" : formCollection["ADMIN_SERVICE_TYPE"];//---  //Added condition for rolecode 5 on 27-03-2023
            //masterQualityMonitorViewModel.CadreStates = (formCollection["CadreStates"] == null ? 0 : Convert.ToInt32(formCollection["CadreStates"]));//
            masterQualityMonitorViewModel.DeEmpanelledRemark = formCollection["DeEmpanelledRemark"];//
            masterQualityMonitorViewModel.DESIGNATION_NAME = formCollection["DESIGNATION_NAME"];
            //masterQualityMonitorViewModel.Districts = formCollection["ADMIN_QM_DEMPANELLED_DATE"];
            masterQualityMonitorViewModel.DISTRICT_NAME = formCollection["DISTRICT_NAME"];
            masterQualityMonitorViewModel.empanelledRemove = formCollection["empanelledRemove"];//---
            masterQualityMonitorViewModel.EMPANELLED_REMOVE_LIST = null;
            masterQualityMonitorViewModel.EncryptedQmCode = formCollection["EncryptedQmCode"];//---
            masterQualityMonitorViewModel.isOpEdit = formCollection["isOpEdit"];//---
            masterQualityMonitorViewModel.MASTER_DESIGNATION = null;
            masterQualityMonitorViewModel.MASTER_DISTRICT = null;
            masterQualityMonitorViewModel.MASTER_STATE = null;
            masterQualityMonitorViewModel.MASTER_STATE1 = null;
            masterQualityMonitorViewModel.MAST_CADRE_STATE_CODE = null;//
            masterQualityMonitorViewModel.MAST_DISTRICT_CODE = (formCollection["MAST_DISTRICT_CODE"] == null ? 0 : Convert.ToInt32(formCollection["MAST_DISTRICT_CODE"]));//---
            masterQualityMonitorViewModel.MAST_STATE_CODE = (formCollection["MAST_STATE_CODE"] == null ? 0 : Convert.ToInt32(formCollection["MAST_STATE_CODE"]));//----
            masterQualityMonitorViewModel.MAST_STATE_CODE_ADDR = (formCollection["MAST_STATE_CODE_ADDR"] == null ? 0 : Convert.ToInt32(formCollection["MAST_STATE_CODE_ADDR"]));//---

            objBAL = new MasterBAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions objCommonFunc = new CommonFunctions();

            foreach (string file in Request.Files)
            {
                if (Request.Files[0].FileName == "" || Request.Files[0].FileName == null)
                {
                    ModelState.AddModelError("File", @"Pdf file is Required");
                }
                string BALstatus = objBAL.ValidatePDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                if (BALstatus != string.Empty)
                {
                    //message = "Upload PDF file..";
                    status = false;
                    return Json(new { success = status, message = BALstatus }, JsonRequestBehavior.AllowGet);

                }
            }

            masterQualityMonitorViewModel.FILE_NAME = Request.Files[0].FileName;
            masterQualityMonitorViewModel.File = Request.Files[0];
            #endregion
            //HttpPostedFileBase postedBgFile = Request.Files[0];

            try
            {
                if (masterQualityMonitorViewModel.ADMIN_QM_TYPE.Equals("S"))
                {
                    ModelState.Remove("ADMIN_SERVICE_TYPE");
                }

                //added by abhinav pathak on 22-08-2019
                ModelState.Remove("DeEmpanelledRemark");

                //----------- changes made by Rohit Borse on 16-03-2022
                // added for De-Empanelled date is required for edit operation only

                if (ModelState.ContainsKey("ADMIN_QM_DEMPANELLED_DATE"))
                {
                    ModelState.Remove("ADMIN_QM_DEMPANELLED_DATE");
                }
                if (ModelState.ContainsKey("empanelledRemove"))             //Added by Shreyas on 15-11-2022
                {
                    ModelState.Remove("empanelledRemove");
                }

                masterQualityMonitorViewModel.EMPANELLED_REMOVE_LIST = new List<SelectListItem> {
                                                        //new SelectListItem{ Text = "Due to Misconduct", Value ="M"},
                                                        new SelectListItem{ Text = "Due to Age", Value ="A" },
                                                        new SelectListItem{ Text = "Due to Administrative Reasons", Value ="AR" },
                                                        new SelectListItem{ Text = "Due to Performance", Value ="P" },
                                                        new SelectListItem{ Text = "Others", Value ="O" },
                                                       };
                masterQualityMonitorViewModel.EMPANELLED_REMOVE_LIST.Insert(0, new SelectListItem { Value = "0", Text = "-- Select Reason --", Selected = true });


                if (ModelState.IsValid)
                {
                    int AdminQMCode = 0;
                    int qmCode = 0;
                    objBAL = new MasterBAL();
                    if (objBAL.AddMasterQualityMonitor(masterQualityMonitorViewModel, ref message, ref AdminQMCode))
                    {
                        //#region Add by Shreyas
                        //try
                        //{
                        //    var fileData = new List<QualityMonitorFileUploadViewModel>();
                        //    if (AdminQMCode != 0)
                        //    {
                        //        string PhysicalPath = ConfigurationManager.AppSettings["QUALITY_MONITOR_PAN_FILE"];
                        //        if (!(objCommonFunc.ValidateIsPdf(PhysicalPath, Request)))
                        //        {
                        //            message = "File Type is Not Allowed.";
                        //            status = false;
                        //            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                        //        }
                        //        foreach (string file in Request.Files)
                        //        {
                        //            UploadPANFile(Request, fileData, qmCode);
                        //        }


                        //        var serializer = new JavaScriptSerializer();
                        //        serializer.MaxJsonLength = Int32.MaxValue;

                        //        var result = new ContentResult
                        //        {
                        //            Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                        //        };
                        //    }

                        //}
                        //catch (Exception ex)
                        //{

                        //    throw;
                        //}

                        //#endregion




                        message = message == string.Empty ? "Quality Monitor details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Quality Monitor details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterQualityMonitor", masterQualityMonitorViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Quality Monitor details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }//By Aanand
        //public ActionResult AddMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel)
        //{
        //    bool status = false;
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            objBAL = new MasterBAL();
        //            if (objBAL.AddMasterQualityMonitor(masterQualityMonitorViewModel, ref message))
        //            {
        //                message = message == string.Empty ? "Quality Monitor details saved successfully." : message;
        //                status = true;
        //            }
        //            else
        //            {
        //                message = message == string.Empty ? "Quality Monitor details not saved." : message;
        //            }
        //        }
        //        else
        //        {
        //            return PartialView("AddEditMasterQualityMonitor", masterQualityMonitorViewModel);
        //        }
        //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        message = "Quality Monitor details not saved.";
        //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// Display QualityMonitor details in edit mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterQualityMonitor(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int qualityMonitorCode = Convert.ToInt32(decryptedParameters["QmCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    MasterAdminQualityMonitorViewModel masterQualityMonitorViewMode = objBAL.GetQualityMonitor_ByQualityMonitorCode(qualityMonitorCode);

                    masterQualityMonitorViewMode.EMPANELLED_REMOVE_LIST = new List<SelectListItem> {
                                                        //new SelectListItem{ Text = "Due to Misconduct", Value ="M"}, 
                                                        new SelectListItem{ Text = "Due to Age", Value ="A" },
                                                        new SelectListItem{ Text = "Due to Administrative Reasons", Value ="AR" },
                                                        new SelectListItem{ Text = "Due to Performance", Value ="P" },
                                                        new SelectListItem{ Text = "Others", Value ="O" },
                                                       };

                    masterQualityMonitorViewMode.EMPANELLED_REMOVE_LIST.Insert(0, new SelectListItem { Value = "0", Text = "-- Select Reason --", Selected = true });
                    //----------

                    masterQualityMonitorViewMode.ADMIN_QM_DEMPANELLED_DATE = DateTime.Now.ToString("dd/MM/yyyy");

                    //added by abhinav pathak on 22-july-2019
                    #region//added by abhinav pathak

                    masterQualityMonitorViewMode.isOpEdit = "E";
                    var deptList = new PMGSYEntities().ADMIN_DEPARTMENT.Where(x => x.MAST_ND_TYPE == "S" && x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).ToList();
                    masterQualityMonitorViewMode.AdminDepartmentList = new List<SelectListItem>();
                    foreach (var item in deptList)
                    {
                        SelectListItem tempitem = new SelectListItem();
                        tempitem.Value = Convert.ToString(item.ADMIN_ND_CODE);
                        tempitem.Text = item.ADMIN_ND_NAME;
                        masterQualityMonitorViewMode.AdminDepartmentList.Add(tempitem);

                    }
                    masterQualityMonitorViewMode.AdminDepartmentList.Insert(0, new SelectListItem { Value = "0", Text = "Select Department", Selected = true });//added by abhinav pathak

                    #endregion
                    if (masterQualityMonitorViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Quality Monitor details not exist.");
                        return PartialView("AddEditMasterQualityMonitor", new MasterAdminQualityMonitorViewModel());
                    }
                    string cadreStatesMapp = string.Empty, cadreState = string.Empty;
                    if (masterQualityMonitorViewMode.ADMIN_SERVICE_TYPE == "A")
                    {

                        foreach (int cadre in masterQualityMonitorViewMode.MAST_CADRE_STATE_CODE)
                        {
                            cadreState += cadre + "$";
                        }
                        cadreStatesMapp = cadreState.Substring(0, cadreState.Length - 1);
                    }
                    ViewBag.cadreStatesMapp = cadreStatesMapp;
                    return PartialView("AddEditMasterQualityMonitor", masterQualityMonitorViewMode);

                }
                return PartialView("AddEditMasterQualityMonitor", new MasterAdminQualityMonitorViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Quality Monitor details not exist.");
                return PartialView("AddEditMasterQualityMonitor", new MasterAdminQualityMonitorViewModel());
            }
        }//By Aanad

        #region Quality Monitor Profile Information to NQM / SQM as Present in CQC / SQC
        //Added by Hrishikesh to provide Quality Monitor Profile Information to NQM/SQM as Present in CQC/SQC --start
        [HttpGet]
        [Audit]
        public ActionResult GetQMProfileInformation()
        {
            var userid = PMGSYSession.Current.UserId;   //get the currently loged in users userID
            objBAL = new MasterBAL();

            //AdminQualityMonitorsModel adminmonitordetails = new AdminQualityMonitorsModel();
            MasterAdminQualityMonitorViewModel adminmonitordetails = new MasterAdminQualityMonitorViewModel();
            try
            {
                var profiledata = objBAL.GetQMProfileInformationBAL(userid);
                if (profiledata != null)
                {
                    adminmonitordetails = profiledata;
                }
                else
                {
                    //ModelState.AddModelError(string.Empty, "Quality Monitor details not exist.");
                    return View(profiledata);
                }
            }
            catch (Exception ex)
            {
                //ErrorLog.LogError(ex, "GetQMProfileInformation");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Quality Monitor details not exist.");
                return View(new MasterAdminQualityMonitorViewModel());
            }
            return View(adminmonitordetails);
        }//end GetQMProfileInformation()
        #endregion



        //public ActionResult EditMasterQualityMonitor(String parameter, String hash, String key)
        //{
        //    Dictionary<string, string> decryptedParameters = null;

        //    try
        //    {
        //        objBAL = new MasterBAL();
        //        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
        //        int qualityMonitorCode = Convert.ToInt32(decryptedParameters["QmCode"]);

        //        if (decryptedParameters.Count() > 0)
        //        {
        //            MasterAdminQualityMonitorViewModel masterQualityMonitorViewMode = objBAL.GetQualityMonitor_ByQualityMonitorCode(qualityMonitorCode);
        //            if (masterQualityMonitorViewMode == null)
        //            {
        //                ModelState.AddModelError(string.Empty, "Quality Monitor details not exist.");
        //                return PartialView("AddEditMasterQualityMonitor", new MasterAdminQualityMonitorViewModel());
        //            }
        //            return PartialView("AddEditMasterQualityMonitor", masterQualityMonitorViewMode);

        //        }
        //        return PartialView("AddEditMasterQualityMonitor", new MasterAdminQualityMonitorViewModel());

        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        ModelState.AddModelError(string.Empty, "Quality Monitor details not exist.");
        //        return PartialView("AddEditMasterQualityMonitor", new MasterAdminQualityMonitorViewModel());
        //    }
        //}

        /// <summary>
        /// Update Quality Monitor Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
       //public ActionResult EditMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterQualityMonitor(FormCollection formCollection)
        {
            bool status = false;
            PMGSYEntities dbcontext = new PMGSYEntities();
            MasterBAL bal = new MasterBAL(); //Changed by deendayal on 28/7/2017

            MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel = new MasterAdminQualityMonitorViewModel();
            #region Added by Shreyas to integrate PAN file uploding

            masterQualityMonitorViewModel.ADMIN_QM_AADHAR_NO = formCollection["ADMIN_QM_AADHAR_NO"] == "" ? null : formCollection["ADMIN_QM_AADHAR_NO"];//---
            masterQualityMonitorViewModel.ADMIN_QM_ADDRESS1 = formCollection["ADMIN_QM_ADDRESS1"];//---
            masterQualityMonitorViewModel.ADMIN_QM_ADDRESS2 = formCollection["ADMIN_QM_ADDRESS2"];//---
            masterQualityMonitorViewModel.ADMIN_QM_BIRTH_DATE = formCollection["ADMIN_QM_BIRTH_DATE"];//---
            //masterQualityMonitorViewModel.ADMIN_QM_CODE= Int32.Parse(formCollection["ADMIN_QM_CODE"]);
            masterQualityMonitorViewModel.ADMIN_QM_DEG = formCollection["ADMIN_QM_DEG"];
            masterQualityMonitorViewModel.ADMIN_QM_DEMPANELLED_DATE = formCollection["ADMIN_QM_DEMPANELLED_DATE"];//---
            masterQualityMonitorViewModel.ADMIN_QM_DESG = (formCollection["ADMIN_QM_DESG"] == null ? 0 : Convert.ToInt32(formCollection["ADMIN_QM_DESG"]));//---
            masterQualityMonitorViewModel.ADMIN_QM_DOCPATH = formCollection["ADMIN_QM_DOCPATH"];
            masterQualityMonitorViewModel.ADMIN_QM_EMAIL = formCollection["ADMIN_QM_EMAIL"];//---
            masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED = formCollection["ADMIN_QM_EMPANELLED"];//---
            masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED_MONTH = (formCollection["ADMIN_QM_EMPANELLED_MONTH"] == null ? 0 : Convert.ToInt32(formCollection["ADMIN_QM_EMPANELLED_MONTH"]));//---
            masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED_REASON = formCollection["ADMIN_QM_EMPANELLED_REASON"];
            masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED_YEAR = (formCollection["ADMIN_QM_EMPANELLED_YEAR"] == null ? 0 : Convert.ToInt32(formCollection["ADMIN_QM_EMPANELLED_YEAR"]));//---
            masterQualityMonitorViewModel.ADMIN_QM_FAX = formCollection["ADMIN_QM_FAX"];//---
            masterQualityMonitorViewModel.ADMIN_QM_FNAME = formCollection["ADMIN_QM_FNAME"];//---
            masterQualityMonitorViewModel.ADMIN_QM_IMAGE = formCollection["ADMIN_QM_DEMPANELLED_DATE"];
            masterQualityMonitorViewModel.ADMIN_QM_LNAME = formCollection["ADMIN_QM_LNAME"];//---
            //masterQualityMonitorViewModel.ADMIN_QM_MNAME = formCollection["ADMIN_QM_MNAME"];//---
            masterQualityMonitorViewModel.ADMIN_QM_MNAME = formCollection["ADMIN_QM_MNAME"] == "" ? null : formCollection["ADMIN_QM_MNAME"];
            masterQualityMonitorViewModel.ADMIN_QM_MOBILE1 = formCollection["ADMIN_QM_MOBILE1"];//---
            masterQualityMonitorViewModel.ADMIN_QM_MOBILE2 = formCollection["ADMIN_QM_MOBILE2"];//---
            masterQualityMonitorViewModel.ADMIN_QM_PAN = formCollection["ADMIN_QM_PAN"];//---
            masterQualityMonitorViewModel.ADMIN_QM_PHONE1 = formCollection["ADMIN_QM_PHONE1"];//---
            masterQualityMonitorViewModel.ADMIN_QM_PHONE2 = formCollection["ADMIN_QM_PHONE2"];//---
            masterQualityMonitorViewModel.ADMIN_QM_PIN = formCollection["ADMIN_QM_PIN"];//---
            masterQualityMonitorViewModel.ADMIN_QM_REMARKS = formCollection["ADMIN_QM_REMARKS"];//---
            masterQualityMonitorViewModel.ADMIN_QM_STD1 = formCollection["ADMIN_QM_STD1"];//---
            masterQualityMonitorViewModel.ADMIN_QM_STD2 = formCollection["ADMIN_QM_STD2"];//---
            masterQualityMonitorViewModel.ADMIN_QM_STD_FAX = formCollection["ADMIN_QM_STD_FAX"];//---
            masterQualityMonitorViewModel.ADMIN_QM_TYPE = formCollection["ADMIN_QM_TYPE"];//---
            masterQualityMonitorViewModel.ADMIN_SERVICE_TYPE = formCollection["ADMIN_SERVICE_TYPE"] == null ? "0" : formCollection["ADMIN_SERVICE_TYPE"];//---
            //masterQualityMonitorViewModel.CadreStates = (formCollection["CadreStates"] == null ? 0 : Convert.ToInt32(formCollection["CadreStates"]));//
            masterQualityMonitorViewModel.DeEmpanelledRemark = formCollection["DeEmpanelledRemark"];//
            masterQualityMonitorViewModel.DESIGNATION_NAME = formCollection["DESIGNATION_NAME"];
            //masterQualityMonitorViewModel.Districts = formCollection["ADMIN_QM_DEMPANELLED_DATE"];
            masterQualityMonitorViewModel.DISTRICT_NAME = formCollection["DISTRICT_NAME"];
            masterQualityMonitorViewModel.empanelledRemove = formCollection["empanelledRemove"];//---
            masterQualityMonitorViewModel.EMPANELLED_REMOVE_LIST = null;
            masterQualityMonitorViewModel.EncryptedQmCode = formCollection["EncryptedQmCode"];//---
            masterQualityMonitorViewModel.isOpEdit = formCollection["isOpEdit"];//---
            masterQualityMonitorViewModel.MASTER_DESIGNATION = null;
            masterQualityMonitorViewModel.MASTER_DISTRICT = null;
            masterQualityMonitorViewModel.MASTER_STATE = null;
            masterQualityMonitorViewModel.MASTER_STATE1 = null;
            masterQualityMonitorViewModel.MAST_CADRE_STATE_CODE = null;//
            masterQualityMonitorViewModel.MAST_DISTRICT_CODE = (formCollection["MAST_DISTRICT_CODE"] == null ? 0 : Convert.ToInt32(formCollection["MAST_DISTRICT_CODE"]));//---
            masterQualityMonitorViewModel.MAST_STATE_CODE = (formCollection["MAST_STATE_CODE"] == null ? 0 : Convert.ToInt32(formCollection["MAST_STATE_CODE"]));//----
            masterQualityMonitorViewModel.MAST_STATE_CODE_ADDR = (formCollection["MAST_STATE_CODE_ADDR"] == null ? 0 : Convert.ToInt32(formCollection["MAST_STATE_CODE_ADDR"]));//---

            objBAL = new MasterBAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions objCommonFunc = new CommonFunctions();




            if (!(formCollection["FILE_NAME"] == "" || formCollection["FILE_NAME"] == null))
            {
                var filename = formCollection["FILE_NAME"];
                masterQualityMonitorViewModel.FILE_NAME = formCollection["FILE_NAME"];
            }
            else
            {
                if (masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED != "N")  //For deEmpanellmet PAN document is not mendetory..   Add by Shreyas on 13-04-2023
                {
                    foreach (string file in Request.Files)
                    {
                        string BALstatus = objBAL.ValidatePDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                        if (BALstatus != string.Empty)
                        {
                            message = BALstatus;
                            status = false;
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                        }
                    }
                }

                masterQualityMonitorViewModel.FILE_NAME = Request.Files[0].FileName;
                masterQualityMonitorViewModel.File = Request.Files[0];
            }

            #endregion
            try
            {
                if (string.IsNullOrEmpty(masterQualityMonitorViewModel.EncryptedQmCode))
                {

                    message = "Quality Monitor details not updated.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                if (PMGSYSession.Current.RoleCode == 8)
                {
                    if (ModelState.ContainsKey("ADMIN_SERVICE_TYPE"))
                        ModelState["ADMIN_SERVICE_TYPE"].Errors.Clear();

                    //added by DEENDAYAL on 20JUN2017 to authorize sqc to modify sqm details
                    string[] encryptedParameters = masterQualityMonitorViewModel.EncryptedQmCode.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        message = "Monitor Details can not be updated.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    int QualityMonitorCode = Convert.ToInt32(decryptedParameters["QmCode"].ToString());

                    //ADMIN_QUALITY_MONITORS qm = dbcontext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == QualityMonitorCode).SingleOrDefault();

                    //if (!qm.ADMIN_QM_FNAME.Equals(masterQualityMonitorViewModel.ADMIN_QM_FNAME) || !qm.ADMIN_QM_MNAME.Equals(masterQualityMonitorViewModel.ADMIN_QM_MNAME) || !qm.ADMIN_QM_LNAME.Equals(masterQualityMonitorViewModel.ADMIN_QM_LNAME) || !qm.ADMIN_QM_BIRTH_DATE.ToString().Split(' ')[0].ToString().Equals(masterQualityMonitorViewModel.ADMIN_QM_BIRTH_DATE) || !qm.ADMIN_QM_PAN.Equals(masterQualityMonitorViewModel.ADMIN_QM_PAN))
                    //{
                    //    message = "These changes are not authorized.";
                    //    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    //}


                    if (!bal.CheckIdentityInformation(masterQualityMonitorViewModel, QualityMonitorCode))
                    {
                        message = "These changes are not authorized.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }

                }
                ///Added by SAMMED A. PATIL on 24 OCTOBER 2016 as per instructions from Srinivasa Sir
                if (masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED == "N")
                {
                    ModelState.Remove("ADMIN_QM_PAN");
                    ModelState.Remove("ADMIN_QM_BIRTH_DATE");
                    ModelState.Remove("ADMIN_QM_EMPANELLED_MONTH");//Added by Deendayal to on 21/06/2017
                }

                //condition added by abhinav pathak on 19-july-2019.
                if (masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED == "Y")
                {
                    ModelState.Remove("empanelledRemove");
                    ModelState.Remove("DeEmpanelledRemark");
                    ModelState.Remove("ADMIN_QM_DEMPANELLED_DATE");
                }

                //added by abhinav to check error list in model.(for debugging)
                #region added by abhinav to check error list in model.(for debugging)
                var errorvalues = ModelState.Values;
                ModelErrorCollection modelerror = null;
                foreach (var item in errorvalues)
                {
                    modelerror = item.Errors;
                }
                #endregion
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.EditMasterQualityMonitor(masterQualityMonitorViewModel, ref message))
                    {

                        message = message == string.Empty ? "Quality Monitor details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Quality Monitor details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterQualityMonitor", masterQualityMonitorViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Quality Monitor details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }//By Aanand


        //public ActionResult EditMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel)
        //{
        //    bool status = false;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(masterQualityMonitorViewModel.EncryptedQmCode))
        //        {
        //            message = "Quality Monitor details not updated.";
        //            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            objBAL = new MasterBAL();
        //            if (objBAL.EditMasterQualityMonitor(masterQualityMonitorViewModel, ref message))
        //            {
        //                message = message == string.Empty ? "Quality Monitor details updated successfully." : message;
        //                status = true;
        //            }
        //            else
        //            {
        //                message = message == string.Empty ? "Quality Monitor details not updated." : message;
        //            }
        //        }
        //        else
        //        {
        //            return PartialView("AddEditMasterQualityMonitor", masterQualityMonitorViewModel);
        //        }
        //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        message = message == string.Empty ? "Quality Monitor details not updated." : message;
        //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// Delete Quality Monitor Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterQualityMonitor(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int qualityMonitorCode = Convert.ToInt32(decryptedParameters["QmCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.DeleteQualityMonitor(qualityMonitorCode, ref message))
                    {
                        message = "Quality Monitor details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Quality Monitor details." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                message = "You can not delete this Quality Monitor details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Quality Monitor details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Populate Districts By State Code
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDistrictByStateCode(string stateCode)
        {
            try
            {
                if (!int.TryParse(stateCode, out outParam))
                {
                    return Json(false);
                }
                List<MASTER_DISTRICT> districtList = new List<MASTER_DISTRICT>();
                IMasterDAL objDAL = new MasterDAL();
                districtList = objDAL.GetAllDistrictByStateCode(Convert.ToInt32(stateCode));
                return Json(new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//By Anand

        //public JsonResult GetDistrictByStateCode(string stateCode)
        //{
        //    try
        //    {
        //        if (!int.TryParse(stateCode, out outParam))
        //        {
        //            return Json(false);
        //        }
        //        List<MASTER_DISTRICT> districtList = new List<MASTER_DISTRICT>();
        //        IMasterDAL objDAL = new MasterDAL();
        //        districtList = objDAL.GetAllDistrictByStateCode(Convert.ToInt32(stateCode));
        //        return Json(new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME"));
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return Json(false);
        //    }
        //}




        /// <summary>
        /// Search QM Details
        /// </summary>
        /// <returns></returns>

        public ActionResult SearchQualityMonitor()
        {
            try
            {
                IMasterDAL objDAL = new MasterDAL();
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["QmTypeList"] = null;
                ViewData["StateList"] = null;
                ViewData["DistrictList"] = null;
                ViewData["EmpanelledList"] = null;
            }
            return PartialView("SearchQualityMonitor");
        }

        /// <summary>
        /// Display QualityMonitor details in edit mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddSQMUserLoginQualityMonitorDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int qualityMonitorCode = Convert.ToInt32(decryptedParameters["QmCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.AddSQMUserLoginQualityMonitorBAL(qualityMonitorCode, ref message))
                    {
                        message = message == string.Empty ? "Quality Monitor username added successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Quality Monitor username not added." : message;
                    }

                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.AddSQMUserLoginQualityMonitorDetails()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #region FileUpload

        //list Uploaded Images        
        public JsonResult ListFiles(FormCollection formCollection)
        {
            objBAL = new MasterBAL();

            int ADMIN_QM_CODE = Convert.ToInt32(Request["ADMIN_QM_CODE"]);
            int totalRecords;
            var jsonData = new
            {
                rows = objBAL.GetFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, ADMIN_QM_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// List Pdf Files
        /// </summary>
        /// <param name="formCollection"> Parameters to Enlist PDf files in JqGrid</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ListPDFFiles(FormCollection formCollection)
        {
            int ADMIN_QM_CODE = Convert.ToInt32(Request["ADMIN_QM_CODE"]);
            int totalRecords;
            objBAL = new MasterBAL();

            var jsonData = new
            {
                rows = objBAL.GetPDFFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, ADMIN_QM_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        //return Image/Doocument Upload Option Menu
        //[HttpGet]

        public ActionResult QualityMonitorFileUpload(string id)
        {
            QualityMonitorFileUploadViewModel QMfileUploadViewModel = new QualityMonitorFileUploadViewModel();
            MasterAdminQualityMonitorViewModel QualityMonitorViewModel = new MasterAdminQualityMonitorViewModel();

            try
            {
                db = new PMGSYEntities();

                IMasterBAL objMasterBAL = new MasterBAL();
                QualityMonitorViewModel = objMasterBAL.GetQualityMonitor_ByQualityMonitorCode(Convert.ToInt32(id));

                QMfileUploadViewModel.ADMIN_QualityMonitor_CODE = Convert.ToInt32(id);
                QMfileUploadViewModel.QMName = QualityMonitorViewModel.ADMIN_QM_FNAME + " " + QualityMonitorViewModel.ADMIN_QM_MNAME + " " + QualityMonitorViewModel.ADMIN_QM_LNAME;
                QMfileUploadViewModel.QMType = QualityMonitorViewModel.ADMIN_QM_TYPE == "I" ? "NQM" : "SQM";
                QMfileUploadViewModel.QMDesignation = db.MASTER_DESIGNATION.Where(m => m.MAST_DESIG_CODE == QualityMonitorViewModel.ADMIN_QM_DESG).Select(s => s.MAST_DESIG_NAME).FirstOrDefault();
                QMfileUploadViewModel.QMState = QualityMonitorViewModel.MAST_STATE_CODE == null ? "-" : db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == QualityMonitorViewModel.MAST_STATE_CODE).Select(s => s.MAST_STATE_NAME).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return View("QualityMonitorFileUpload", QMfileUploadViewModel);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
            return View("QualityMonitorFileUpload", QMfileUploadViewModel);
        }

        //Returns Image Upload View
        // [HttpGet]

        public ActionResult QualityMonitorImageUpload(string id)
        {
            db = new PMGSYEntities();
            QualityMonitorFileUploadViewModel fileUploadViewModel = new QualityMonitorFileUploadViewModel();
            fileUploadViewModel.ADMIN_QualityMonitor_CODE = Convert.ToInt32(id);
            if (db.ADMIN_QUALITY_MONITORS.Where(a => a.ADMIN_QM_CODE == fileUploadViewModel.ADMIN_QualityMonitor_CODE && a.ADMIN_QM_IMAGE != null).Any())
            {
                fileUploadViewModel.NumberofFiles = 1;
            }
            else
            {
                fileUploadViewModel.NumberofFiles = 0;
            }
            return View("QualityMonitorImageUpload", fileUploadViewModel);
        }

        //Returns Document Upload View
        // [HttpGet]

        public ActionResult QualityMonitorPdfFileUpload(string id)
        {
            QualityMonitorFileUploadViewModel fileUploadViewModel = new QualityMonitorFileUploadViewModel();
            fileUploadViewModel.ADMIN_QualityMonitor_CODE = Convert.ToInt32(id);
            db = new PMGSYEntities();

            if (db.ADMIN_QUALITY_MONITORS.Where(a => a.ADMIN_QM_CODE == fileUploadViewModel.ADMIN_QualityMonitor_CODE && a.ADMIN_QM_DOCPATH != null).Any())
            {
                fileUploadViewModel.NumberofFiles = 1;
            }
            else
            {
                fileUploadViewModel.NumberofFiles = 0;
            }
            return View("QualityMonitorPdfFileUpload", fileUploadViewModel);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult Uploads(QualityMonitorFileUploadViewModel fileUploadViewModel)
        {
            //Added By Abhishek kamble 26-Apr-2014 to validate File Type
            CommonFunctions objCommonFunc = new CommonFunctions();
            //Array of File Types to Validate             
            String[] fileTypes = new String[] { "jpeg", "png", "gif" };
            if (!(objCommonFunc.IsValidImageFile(ConfigurationManager.AppSettings["QUALITY_MONITOR_FILE_UPLOAD"], Request, fileTypes)))
            {
                fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                return View("ImageUpload", fileUploadViewModel.ErrorMessage);
            }
            //var fileData = new List<QualityMonitorFileUploadViewModel>();
            int ADMIN_QM_CODE = 0;

            if (fileUploadViewModel.ADMIN_QualityMonitor_CODE != 0)
            {
                ADMIN_QM_CODE = fileUploadViewModel.ADMIN_QualityMonitor_CODE;
            }
            else
            {
                try
                {
                    ADMIN_QM_CODE = Convert.ToInt32(Request["ADMIN_QM_CODE"]);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                    if (Request["ADMIN_QM_CODE"].Contains(','))
                    {
                        ADMIN_QM_CODE = Convert.ToInt32(Request["ADMIN_QM_CODE"].Split(',')[0]);
                    }
                }
            }

            UploadImageFile(Request, fileUploadViewModel, ADMIN_QM_CODE);

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileUploadViewModel) + "}",
            };
            return result;
        }

        public void UploadImageFile(HttpRequestBase request, QualityMonitorFileUploadViewModel fileUploadViewModel, int ADMIN_QM_CODE)
        {
            try
            {
                String StorageRoot = ConfigurationManager.AppSettings["QUALITY_MONITOR_FILE_UPLOAD"];
                objBAL = new MasterBAL();

                HttpPostedFileBase file = request.Files[0];

                var fileId = ADMIN_QM_CODE;

                var fileName = ADMIN_QM_CODE + "-" + 1 + Path.GetExtension(request.Files[0].FileName).ToString();
                var fullPath = Path.Combine(StorageRoot, fileName);
                var ThumbnailPath = Path.Combine(StorageRoot, "thumbnails");
                var FullThumbnailPath = Path.Combine(ThumbnailPath, fileName);

                fileUploadViewModel.url = fullPath;
                fileUploadViewModel.thumbnail_url = fullPath;
                fileUploadViewModel.name = fileName;
                fileUploadViewModel.type = file.ContentType;
                fileUploadViewModel.size = file.ContentLength;
                fileUploadViewModel.delete_url = "";
                fileUploadViewModel.delete_type = "DELETE";
                fileUploadViewModel.ADMIN_QM_CODE = ADMIN_QM_CODE;

                fileUploadViewModel.file_type = "I";

                string status = objBAL.AddFileUploadDetailsBAL(fileUploadViewModel);
                if (status == string.Empty)
                {
                    objBAL.CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                }
                else
                {
                    // show an error over here
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                throw ex;
            }
        }

        [HttpPost]

        public ActionResult PdfFileUpload(QualityMonitorFileUploadViewModel fileUploadViewModel)
        {

            int ADMIN_QM_CODE = 0;
            if (fileUploadViewModel.ADMIN_QualityMonitor_CODE != 0)
            {
                ADMIN_QM_CODE = fileUploadViewModel.ADMIN_QualityMonitor_CODE;
            }
            else
            {
                try
                {
                    ADMIN_QM_CODE = Convert.ToInt32(Request["ADMIN_QM_CODE"]);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                    if (Request["ADMIN_QM_CODE"].Contains(','))
                    {
                        ADMIN_QM_CODE = Convert.ToInt32(Request["ADMIN_QM_CODE"].Split(',')[0]);
                    }
                }
            }

            UploadPDFFile(Request, fileUploadViewModel, ADMIN_QM_CODE);

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileUploadViewModel) + "}",
            };
            return result;
        }


        public void UploadPDFFile(HttpRequestBase request, QualityMonitorFileUploadViewModel fileUploadViewModel, int ADMIN_QM_CODE)
        {
            try
            {
                String StorageRoot = ConfigurationManager.AppSettings["QUALITY_MONITOR_FILE_UPLOAD"];
                objBAL = new MasterBAL();

                HttpPostedFileBase file = request.Files[0];

                var fileId = ADMIN_QM_CODE;

                var fileName = ADMIN_QM_CODE + "-" + 1 + Path.GetExtension(request.Files[0].FileName).ToString();
                var fullPath = Path.Combine(StorageRoot, fileName);
                var ThumbnailPath = Path.Combine(StorageRoot, "thumbnails");
                var FullThumbnailPath = Path.Combine(ThumbnailPath, fileName);

                fileUploadViewModel.url = fullPath;
                fileUploadViewModel.thumbnail_url = fullPath;
                fileUploadViewModel.name = fileName;
                fileUploadViewModel.type = file.ContentType;
                fileUploadViewModel.size = file.ContentLength;
                fileUploadViewModel.delete_url = "";
                fileUploadViewModel.delete_type = "DELETE";
                fileUploadViewModel.ADMIN_QM_CODE = ADMIN_QM_CODE;

                fileUploadViewModel.file_type = "D";

                string status = objBAL.AddFileUploadDetailsBAL(fileUploadViewModel);

                if (status == string.Empty)
                {
                    file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["QUALITY_MONITOR_PDF_FILE_UPLOAD"], fileName));
                }
                else
                {
                    // show an error over here
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                throw ex;
            }
        }

        // [HttpGet]

        public ActionResult DownloadFile(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPath = string.Empty;
            string FullfilePhysicalPath = string.Empty;
            string FileExtension = string.Empty;

            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    FileName = (urlSplitParams[0]);
                }
            }
            FileExtension = Path.GetExtension(FileName).ToLower();

            if (FileExtension == ".pdf")
            {
                FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_MONITOR_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_MONITOR_PDF_FILE_UPLOAD"], FileName);
            }
            else if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
            {
                FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_MONITOR_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_MONITOR_FILE_UPLOAD"], FileName);
            }

            string name = Path.GetFileName(FileName);
            string ext = Path.GetExtension(FileName);

            string type = string.Empty;

            if (ext != null)
            {
                switch (ext.ToLower())
                {
                    case ".pdf":
                        type = "Application/pdf";
                        break;
                    case ".doc":
                    case ".docx":
                        type = "Application/msword";
                        break;
                    case ".jpg":
                    case ".bmp":
                    case ".tiff":
                    case ".png":
                    case ".gif":
                    case ".jpeg":
                        type = "image/png";
                        break;
                    default:
                        type = "Application";
                        break;
                }
            }

            if (System.IO.File.Exists(FullfilePhysicalPath))
            {
                return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
            }
            else
            {
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Changed by deendayal on 05/19/2017
        [HttpPost]
        public JsonResult DeleteFileDetails(string id)
        {
            objBAL = new MasterBAL();
            db = new PMGSYEntities();
            String PhysicalPath = string.Empty;
            string status = "Error";
            if (Request.Params["FILE_TYPE"].ToUpper() == "I")
            {
                PhysicalPath = ConfigurationManager.AppSettings["QUALITY_MONITOR_FILE_UPLOAD"];
            }
            else if (Request.Params["FILE_TYPE"].ToUpper() == "D")
            {
                PhysicalPath = ConfigurationManager.AppSettings["QUALITY_MONITOR_PDF_FILE_UPLOAD"];
            }

            string[] arrParam = Request.Params["ADMIN_QM_CODE"].Split('$');

            int ADMIN_QM_CODE = Convert.ToInt32(arrParam[0]);

            string FILE_NAME = Request.Params["FILE_NAME"];

            PhysicalPath = Path.Combine(PhysicalPath, FILE_NAME);

            if (!System.IO.File.Exists(PhysicalPath))
            {
                ADMIN_QUALITY_MONITORS monitors = db.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_CODE == ADMIN_QM_CODE).SingleOrDefault();
                monitors.ADMIN_QM_IMAGE = null;
                db.ADMIN_QUALITY_MONITORS.Attach(monitors);
                db.Entry(monitors).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                status = "";
                return Json(new { Success = true, ErrorMessage = "File Deleted sucessfully." });
            }


            if (Request.Params["FILE_TYPE"].ToUpper() == "I")
            {
                status = objBAL.DeleteFileDetails(ADMIN_QM_CODE, FILE_NAME);
            }

            if (Request.Params["FILE_TYPE"].ToUpper() == "D")
            {
                status = objBAL.DeletePdfFileDetails(ADMIN_QM_CODE, FILE_NAME);
            }


            if (status == string.Empty)
            {
                try
                {
                    System.IO.File.Delete(PhysicalPath);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                    return Json(new { Success = true, ErrorMessage = ex.Message });
                }
                return Json(new { Success = true, ErrorMessage = "There is an error while processing your request." });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = status });
            }
        }
        #endregion

        #endregion


        #region PdfUpload for PAN number

        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult ListPANFiles(FormCollection formCollection)
        {
            objBAL = new MasterBAL();
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            int qmCode = Convert.ToInt32(Request["ADMIN_QM_CODE"]);
            int totalRecords;
            var jsonData = new
            {
                rows = objBAL.GetPANFileListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, qmCode),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }


        /// <summary>
        /// Get the PDF File Upload View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult PANFileUpload(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                QualityMonitorFileUploadViewModel fileUploadViewModel = new QualityMonitorFileUploadViewModel();
                fileUploadViewModel.ADMIN_QM_CODE = Convert.ToInt32(id);
                fileUploadViewModel.ErrorMessage = string.Empty;
                if (dbContext.ADMIN_QUALITY_MONITORS.Where(a => a.ADMIN_QM_CODE == fileUploadViewModel.ADMIN_QM_CODE && a.ADMIN_QM_PAN_FILE != null).Any())
                {
                    fileUploadViewModel.NumberofFiles = 1;
                }
                else
                {
                    fileUploadViewModel.NumberofFiles = 0;
                }
                return View(fileUploadViewModel);
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


        /// <summary>
        /// Post Method for Uploading PDF File
        /// </summary>
        /// <param name="fileUploadViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult PANFileUpload(QualityMonitorFileUploadViewModel fileUploadViewModel)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            objBAL = new MasterBAL();
            CommonFunctions objCommonFunc = new CommonFunctions();
            try
            {
                foreach (string file in Request.Files)
                {
                    string status = objBAL.ValidatePDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        fileUploadViewModel.ErrorMessage = status;
                        return View("PANFileUpload", fileUploadViewModel.ErrorMessage);
                    }
                }

                var fileData = new List<QualityMonitorFileUploadViewModel>();

                int qmCode = 0;
                if (fileUploadViewModel.ADMIN_QM_CODE != 0)
                {
                    qmCode = fileUploadViewModel.ADMIN_QM_CODE;
                }
                else
                {
                    try
                    {
                        qmCode = Convert.ToInt32(Request["ADMIN_QM_CODE"]);
                    }
                    catch
                    {
                        if (Request["ADMIN_QM_CODE"].Contains(','))
                        {
                            qmCode = Convert.ToInt32(Request["ADMIN_QM_CODE"].Split(',')[0]);
                        }
                    }
                }

                //string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = ConfigurationManager.AppSettings["QUALITY_MONITOR_PAN_FILE"];

                if (!(objCommonFunc.ValidateIsPdf(PhysicalPath, Request)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("PANFileUpload", fileUploadViewModel.ErrorMessage);
                }

                foreach (string file in Request.Files)
                {
                    UploadPANFile(Request, fileData, qmCode);
                }

                if (dbContext.ADMIN_QUALITY_MONITORS.Where(a => a.ADMIN_QM_CODE == fileUploadViewModel.ADMIN_QM_CODE && a.ADMIN_QM_PAN_FILE != null).Any())
                {
                    fileUploadViewModel.NumberofFiles = 1;
                }
                else
                {
                    fileUploadViewModel.NumberofFiles = 0;
                }

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception)
            {
                fileUploadViewModel.ErrorMessage = "An Error Occurred While Processing Your Request.";
                return View("PANFileUpload", fileUploadViewModel.ErrorMessage);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// This Method Uploads PDF File
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        [Audit]
        public void UploadPANFile(HttpRequestBase request, List<QualityMonitorFileUploadViewModel> statuses, int qmCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            objBAL = new MasterBAL();
            try
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];

                    var fileName = qmCode + Path.GetExtension(request.Files[i].FileName).ToString();

                    string PhysicalPath = ConfigurationManager.AppSettings["QUALITY_MONITOR_PAN_FILE"];

                    var fullPath = Path.Combine(PhysicalPath, fileName);

                    statuses.Add(new QualityMonitorFileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",

                        ADMIN_QM_CODE = qmCode
                    });

                    string status = objBAL.AddPANUploadDetailsBAL(statuses);
                    if (status == string.Empty)
                    {
                        //file.SaveAs(fullPath);
                        file.SaveAs(Path.Combine(PhysicalPath, fileName));
                    }
                    else
                    {
                        // show an error over here
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Downloads the File
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult DownloadPANFile(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string FileName = string.Empty;
                int obsId = 0;
                string FullFileLogicalPath = string.Empty;
                string FullfilePhysicalPath = string.Empty;
                string FileExtension = string.Empty;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                        obsId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                string VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_MONITOR_PAN_FILE_VIRTUAL_PATH"];
                string PhysicalPath = ConfigurationManager.AppSettings["QUALITY_MONITOR_PAN_FILE"];

                FullFileLogicalPath = Path.Combine(VirtualDirectoryUrl, FileName);
                FullfilePhysicalPath = Path.Combine(PhysicalPath, FileName);

                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, "Application/pdf", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        /// <summary>
        /// Delete File and File Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult DeletePANFileDetails(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            objBAL = new MasterBAL();
            try
            {
                int ADMIN_QM_CODE = Convert.ToInt32(Request.Params["ADMIN_QM_CODE"]);
                string PhysicalPath = ConfigurationManager.AppSettings["QUALITY_MONITOR_PAN_FILE"];
                string FILE_NAME = ADMIN_QM_CODE + ".pdf";

                PhysicalPath = Path.Combine(PhysicalPath, FILE_NAME);
                string status = objBAL.DeletePANFileDetailsBAL(ADMIN_QM_CODE);

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "There is an error while processing your request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch
            {
                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
        }

        #endregion

        #endregion Quality Monitors Actions


        //added by koustubh Nakate on 04-05-2013 for Region-District Mapping

        #region Region-District-Mapping


        public ActionResult MapRegionDistricts(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    ViewBag.EncryptedStateCode = parameter + '/' + hash + '/' + key;
                    ViewBag.RegionName = decryptedParameters["RegionName"].ToString();
                    ViewBag.StateName = decryptedParameters["StateName"].ToString();

                    return PartialView("MapRegionDistricts");
                }
                return PartialView("MapRegionDistricts");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MapRegionDistricts");
            }
        }

        /// <summary>
        /// Mapp Region Districts
        /// </summary>
        /// <param name="frmCollection"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult MapRegionDistricts(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedDistrictCodes = string.Empty;
            string encryptedRegionCode = string.Empty;
            objBAL = new MasterBAL();
            try
            {
                encryptedDistrictCodes = frmCollection["EncryptedDistrictCodes"];
                encryptedRegionCode = frmCollection["EncryptedStateCode"];

                if (objBAL.MapRegionDistrictsBAL(encryptedRegionCode, encryptedDistrictCodes))
                {

                    message = "Region and District are mapped successfully.";
                    status = true;
                }
                else
                {
                    message = "Region and District are not mapped.";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Region and District are not mapped.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get District Details List for Mapping
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult GetDistrictDetailsList_Mapping(int? page, int? rows, string sidx, string sord)
        {
            string[] agency = null;
            int agencyCode = 0;
            int regionCode = 0;
            int adminNdCode = 0;
            String searchParameters = string.Empty;
            Dictionary<string, string> decryptedParameters = new Dictionary<string, string>();
            IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();
            long totalRecords;
            int stateCode = 0;
            String[] encryptedParameters = null;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                encryptedParameters = Request.Params["StateCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    stateCode = Convert.ToInt32(decryptedParameters["StateCode"].ToString());
                    regionCode = Convert.ToInt32(decryptedParameters["RegionCode"].ToString());

                }

                if (!string.IsNullOrEmpty(Request.Params["AgencyCode"]))
                {
                    agency = Convert.ToString(Request.Params["AgencyCode"]).Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { agency[0], agency[1], agency[2] });
                    agencyCode = Convert.ToInt32(decryptedParameters["TACode"]);
                }

                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetDistrictDetailsListBAL(agencyCode, regionCode, adminNdCode, true, DAL.MappingType.RegionDistrict, stateCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = 0,
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

        /// <summary>
        /// Populate States
        /// </summary>
        /// <returns></returns>

        public ActionResult SearchRegion()
        {
            try
            {
                ViewData["StateList"] = new SelectList(new MasterDataEntryDAL().GetAllStates(true), "MAST_STATE_CODE", "MAST_STATE_NAME");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["StateList"] = null;
            }

            return PartialView("SearchRegion");
        }

        /// <summary>
        /// Mapped Region Districts List
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        public ActionResult MappedRegionDistricts(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    ViewBag.EncryptedRegionCode_Mapped = parameter + '/' + hash + '/' + key;

                    ViewBag.RegionName = decryptedParameters["RegionName"].ToString();
                    ViewBag.StateName = decryptedParameters["StateName"].ToString();

                    return PartialView("MappedRegionDistricts");
                }
                return PartialView("MappedRegionDistricts");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MappedRegionDistricts");
            }
        }

        /// <summary>
        /// Get District Details Mapped List 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDistrictDetailsList_Mapped(int? page, int? rows, string sidx, string sord)
        {
            Dictionary<string, string> decryptedParameters = new Dictionary<string, string>();
            long totalRecords;
            int regionCode = 0;
            String[] encryptedParameters = null;
            objBAL = new MasterBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            try
            {
                encryptedParameters = Request.Params["RegionCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    regionCode = Convert.ToInt32(decryptedParameters["RegionCode"].ToString());
                }

                var jsonData = new
                {
                    rows = objBAL.GetMappedDistrictDetailsListBAL_Region(regionCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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


        public ActionResult DeleteMappedDistrictRegionDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int regionId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                regionId = Convert.ToInt32(decryptedParameters["RegionId"]);
                objBAL = new MasterBAL();
                bool status = objBAL.DeleteMappedRegionDistrictBAL(regionId);
                if (status == true)
                {
                    return Json(new { success = true, message = "Mapped District details deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Mapped District details not deleted" });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Mapped District details not deleted." });
            }
        }


        #endregion Region-District-Mapping


        //added by Koustubh Nakate on 16/05/2013 for admin department 
        #region DPIU LIST BY SRDA

        [HttpPost]
        public ActionResult GetDPIUList_BySRDACode(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            int adminNDCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                adminNDCode = Convert.ToInt32(Request.Params["AdminNDCode"]);
                objBAL = new MasterBAL();
                var jsonData = new
                {
                    rows = objBAL.GetDPIUListBAL_ByAdminNDCode(adminNDCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        #endregion  DPIU LIST BY SRDA


        //added by Koustubh Nakate on 28-05-2013 for get all classes from state
        //added by Vikram Nandanwar on 23-08-2013 for get all classes from state
        [HttpPost]
        public JsonResult GetClassessByStateCode(string stateCode)
        {
            try
            {
                List<SelectListItem> classTypeList = new List<SelectListItem>();
                IMasterDAL objDAL = new MasterDAL();
                classTypeList = objDAL.GetAllContClassByRegState(Convert.ToInt32(stateCode.Trim()));

                classTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });

                return Json(new SelectList(classTypeList, "Value", "Text"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }


        #region BankDetails
        /// <summary>
        /// This method is for loading the list of contractor bank details.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns view of list</returns>
        [HttpGet]
        public ActionResult ListBankDetails(String id)
        {
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            MasterContractorBankDetails contractorBankDetails = new MasterContractorBankDetails();
            int ContractorId = 0;
            int RegState = 0;
            try
            {
                if (id != string.Empty)
                {
                    encryptedParameters = id.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        ViewBag.ContractorId = decryptedParameters["ContRegID"].ToString();
                        ViewData["RegStateCode"] = decryptedParameters["RegState"].ToString();
                        ContractorId = Convert.ToInt32(decryptedParameters["ContRegID"]);
                        //RegState = Convert.ToInt32(decryptedParameters["RegState"]);


                    }
                    contractorBankDetails.EncryptedContractorId = id;
                    contractorBankDetails.MAST_CON_ID = ContractorId;


                    return PartialView("ListBankDetails", contractorBankDetails);
                }
                return PartialView("ListBankDetails", contractorBankDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ListBankDetails", contractorBankDetails);
            }
        }
        /// <summary>
        /// This method is for loading the Add functionality form.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View of Add screen</returns>
        [HttpGet]
        public ActionResult AddBankDetails(string id)
        {
            objDAL = new MasterDAL();
            MasterContractorBankDetails model = new MasterContractorBankDetails();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                string[] strsplit = id.Split('$');
                ViewBag.ContractorId = strsplit[0].ToString();
                List<MASTER_DISTRICT> lstDistricts = null;
                List<MASTER_STATE> lstState = null;
                int stateCode = 0;

                #region
                //lstState = objDAL.GetStates();
                //lstState.Insert(0, new MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });
                //stateCode = Convert.ToInt32(strsplit[1]);
                //lstDistricts = objDAL.GetAllDistrictsByStateCode(stateCode);
                //lstDistricts.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });


                //if (PMGSY.Extensions.PMGSYSession.Current.StateCode == 0)
                //{
                //    //lstDistricts = objDAL.getContractorDistricts(Convert.ToInt32(strsplit[0])); ///Coomented by deepak

                //    stateCode = Convert.ToInt32(strsplit[1]);
                //    lstDistricts = objDAL.GetAllDistrictsByStateCode(stateCode);
                //}
                //else
                //{
                //    lstDistricts = objDAL.GetAllDistrictsByStateCode(PMGSY.Extensions.PMGSYSession.Current.StateCode);
                //    lstDistricts.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });
                //    stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                //}
                #endregion

                // if (PMGSY.Extensions.PMGSYSession.Current.StateCode == 0) {
                stateCode = Convert.ToInt32(strsplit[1]);
                lstState = objDAL.GetStates();

                lstDistricts = objDAL.GetAllDistrictsByStateCode(stateCode);
                //}
                if (PMGSY.Extensions.PMGSYSession.Current.StateCode > 0)
                {
                    stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                    lstState.Insert(0, new MASTER_STATE() { MAST_STATE_CODE = stateCode, MAST_STATE_NAME = PMGSY.Extensions.PMGSYSession.Current.StateName });
                }
                /*else
                {
                    stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                    lstState = new List<MASTER_STATE>();
                    lstState.Insert(0, new MASTER_STATE() { MAST_STATE_CODE = stateCode, MAST_STATE_NAME = PMGSY.Extensions.PMGSYSession.Current.StateName });

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        lstDistricts = new List<MASTER_DISTRICT>();
                        lstDistricts.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode, MAST_DISTRICT_NAME = PMGSYSession.Current.DistrictName });
                    }
                    else
                    {
                        lstDistricts = objDAL.GetAllDistrictsByStateCode(PMGSY.Extensions.PMGSYSession.Current.StateCode);
                        lstDistricts.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });
                    }
                }*/

                model.Mast_State_Code = stateCode;
                ViewData["Districts"] = new SelectList(lstDistricts, "MAST_DISTRICT_CODE", " MAST_DISTRICT_NAME");
                ViewData["State"] = new SelectList(lstState, "MAST_STATE_CODE", " MAST_STATE_NAME", stateCode);

                model.lstBankNames = comm.PopulatePFMSBankNames();
                model.lstIfscCodes = new List<SelectListItem>();
                model.lstIfscCodes.Insert(0, new SelectListItem() { Text = "Select Ifsc Code", Value = "" });

                return PartialView("AddBankDetails", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.AddBankDetails().GET");
                return null;
            }
        }

        [HttpGet]
        public ActionResult PopulateIfscByBankName(string bankName)
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                List<SelectListItem> list = objCommonFunctions.PopulatePFMSIfscByBank(bankName);
                //list.Find(x => x.Value == "-1").Value = "0";
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.PopulateIfscByBankName()");
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetBankList()
        {
            try
            {
                List<SelectListItem> bankList = new List<SelectListItem>();

                MasterDAL objmasterDAL = new MasterDAL();

                bankList = objmasterDAL.GetBankListDAL();

                return Json(bankList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBankList()");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }//end function GetBankList

        /// <summary>
        /// This method is for Adding the bank details
        /// </summary>
        /// <param name="contractorBankDetails"></param>
        /// <returns>View for Add form </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBankDetails(MasterContractorBankDetails contractorBankDetails)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                MasterDAL masterDAL = new MasterDAL();
                if (ModelState.IsValid)
                {
                    if (!masterDAL.ValidatePFMSBankDetailsDAL(contractorBankDetails.MAST_BANK_NAME, contractorBankDetails.MAST_IFSC_CODE))
                    {
                        return Json(new { success = false, message = "Invalid Ifsc Code entered." }, JsonRequestBehavior.AllowGet);
                    }
                    if (objBAL.AddContractorBankDetails(contractorBankDetails, ref message))
                    {
                        message = message == string.Empty ? "Bank details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Bank details not saved. " : message;
                    }
                }

                else
                {
                    return PartialView("AddBankDetails", contractorBankDetails);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Bank details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is for checking existing bank details.
        /// </summary>
        /// <param name="contractorBankDetails"></param>
        /// <returns>Returns true if  bank details already exists.</returns>
        [HttpPost]
        public ActionResult CheckExistRecord(MasterContractorBankDetails contractorBankDetails)
        {
            bool status = true;
            bool alreadyExists = false;
            bool isBankDetailsExists = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objDAL = new MasterDAL();

                    //Added By Abhishke kamble 20-feb-2014 
                    if (objDAL.IsBankDetailsExists(contractorBankDetails, ref message))
                    {
                        status = true;
                        return Json(new { success = status, isBankDetailsExists = true, message = message });
                    }

                    if (objDAL.checkAlreadyExists(contractorBankDetails, ref message))
                    {
                        status = true;
                        alreadyExists = true;
                    }
                }

                return Json(new { success = status, alreadyExists = alreadyExists, isBankDetailsExists = isBankDetailsExists }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Bank details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is for loading existing record into the form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditBankDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            MasterDAL masterDAL = new MasterDAL();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                objDAL = new MasterDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int AccountCode = Convert.ToInt32(decryptedParameters["AccountCode"]);
                int ContractorCode = Convert.ToInt32(decryptedParameters["ContractorId"]);
                ViewBag.ContractorId = ContractorCode.ToString();
                // List<MASTER_DISTRICT> lstDistricts = objDAL.getContractorDistricts(ContractorCode);
                List<MASTER_DISTRICT> lstDistricts = null;
                List<MASTER_STATE> lstState = null;
                lstState = objDAL.GetStates();
                lstState.Insert(0, new MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });



                if (decryptedParameters.Count() > 0)
                {
                    objBAL = new MasterBAL();

                    MasterContractorBankDetails contractorBankDetails = objBAL.getContractorBankDetails_ByBankCode(AccountCode, ContractorCode);
                    db = new PMGSYEntities();
                    var stateCode = db.MASTER_DISTRICT.Where(a => a.MAST_DISTRICT_CODE == contractorBankDetails.MAST_DISTRICT_CODE).Select(a => a.MAST_STATE_CODE).FirstOrDefault();

                    contractorBankDetails.Mast_State_Code = Convert.ToInt32(stateCode.ToString());
                    //if (PMGSY.Extensions.PMGSYSession.Current.StateCode == 0)
                    //{
                    //   // lstDistricts = objDAL.getContractorDistricts(ContractorCode);
                    //    lstDistricts = objDAL.GetAllDistrictsByStateCode(contractorBankDetails.Mast_State_Code);
                    //}
                    //else
                    //{
                    //    lstDistricts = objDAL.GetAllDistrictsByStateCode(PMGSY.Extensions.PMGSYSession.Current.StateCode);
                    //    lstDistricts.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });
                    //}
                    lstDistricts = objDAL.GetAllDistrictsByStateCode(contractorBankDetails.Mast_State_Code);
                    lstDistricts.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });
                    ViewData["Districts"] = new SelectList(lstDistricts, "MAST_DISTRICT_CODE", " MAST_DISTRICT_NAME", contractorBankDetails.MAST_DISTRICT_CODE);

                    ViewData["State"] = new SelectList(lstState, "MAST_STATE_CODE", " MAST_STATE_NAME", contractorBankDetails.Mast_State_Code);
                    if (contractorBankDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Bank details not exist.");
                        return PartialView("AddBankDetails", new MasterContractorBankDetails());
                    }

                    contractorBankDetails.lstBankNames = comm.PopulatePFMSBankNames();
                    contractorBankDetails.lstIfscCodes = comm.PopulatePFMSIfscByBank(contractorBankDetails.MAST_BANK_NAME);

                    if (!masterDAL.ValidatePFMSBankDetailsDAL(contractorBankDetails.MAST_BANK_NAME, contractorBankDetails.MAST_IFSC_CODE))
                    {
                        //contractorBankDetails.pfmsErrorMessage = "Either Bank Name or Ifsc Code or both details entered does not match PFMS details, please select Bank Name & Ifsc Code as per PFMS";
                        contractorBankDetails.pfmsErrorMessage = "Bank/IFSC Code is not entered as per Master Data of Bank/IFSC Code, please select correct Bank/IFSC Code from given dropdowns.";
                    }

                    return PartialView("AddBankDetails", contractorBankDetails);
                }
                return PartialView("AddBankDetails", new MasterContractorBankDetails());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Bank details not exist.");
                return PartialView("AddBankDetails", new MasterContractorBankDetails());
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }

        }

        /// <summary>
        /// This method is to update the record
        /// </summary>
        /// <param name="contractorBankDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditBankDetails(MasterContractorBankDetails contractorBankDetails)
        {
            bool status = false;
            MasterDAL masterDAL = new MasterDAL();
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (!masterDAL.ValidatePFMSBankDetailsDAL(contractorBankDetails.MAST_BANK_NAME, contractorBankDetails.MAST_IFSC_CODE))
                    {
                        return Json(new { success = false, message = "Invalid Ifsc Code entered." }, JsonRequestBehavior.AllowGet);
                    }
                    if (objBAL.EditContractorBankDetails(contractorBankDetails, ref message))
                    {
                        message = message == string.Empty ? "Bank details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Bank details not updated." : message;
                    }
                }
                //else
                //{
                //    return PartialView("AddBankDetails", contractorBankDetails);
                //}
                else
                {
                    string messages = string.Join("; ", ModelState.Values
                                           .SelectMany(x => x.Errors)
                                           .Select(x => x.ErrorMessage));

                    return Json(new { success = status, message = messages }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Bank details not updated.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// This method is to delete the bank details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteBankDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int AccountCode = Convert.ToInt32(decryptedParameters["AccountCode"]);
                int ContractorCode = Convert.ToInt32(decryptedParameters["ContractorId"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteContractorBankDetails(AccountCode, ContractorCode, ref message))
                    {
                        message = "Bank details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Bank details." : message;
                    }
                    return Json(new { success = status, message = "Bank details deleted successfully." }, JsonRequestBehavior.AllowGet);
                }
                message = "You can not delete this Bank details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Bank details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is loading the grid for bank details.
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>


        public ActionResult GetContractorBankDetails(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;

            int ContractorCode = 0;
            int regState = 0;
            try
            {
                //Added By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Added By Abhishek kamble 29-Apr-2014 end

                objBAL = new MasterBAL();
                encryptedParameters = Request.Params["ContractorCode"].Split('/');
                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    ContractorCode = Convert.ToInt32(decryptedParameters["ContRegID"].ToString());
                    regState = ((PMGSYSession.Current.RoleCode == 23) ? 0 : (Convert.ToInt32(decryptedParameters["RegState"].ToString())));
                }

                var jsonData = new
                {
                    rows = objBAL.ListContractorBankDetails(ContractorCode, regState, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        public ActionResult GetContractorBankDetailsView(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            MasterDAL masterDAL = new MasterDAL();
            int ContractorCode = 0;
            int regState = 0;
            try
            {
                //Added By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Added By Abhishek kamble 29-Apr-2014 end

                objBAL = new MasterBAL();
                encryptedParameters = Request.Params["ContractorCode"].Split('/');
                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    ContractorCode = Convert.ToInt32(decryptedParameters["ContRegID"].ToString());
                    //regState = ((PMGSYSession.Current.RoleCode == 23) ? 0 : (Convert.ToInt32(decryptedParameters["RegState"].ToString())));
                    regState = 0;
                }

                var jsonData = new
                {
                    rows = masterDAL.ListContractorBankDetailsView(ContractorCode, regState, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        ///added by PP [05-05-2018]
        /// <summary>
        /// This method is used to finalize the bank details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FinalizeBankDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            MasterDAL masterDAL = new MasterDAL();
            CommonFunctions comm = new CommonFunctions();
            string[] arrBankDetails = new string[3];
            try
            {
                objDAL = new MasterDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int AccountCode = Convert.ToInt32(decryptedParameters["AccountCode"]);
                int ContractorCode = Convert.ToInt32(decryptedParameters["ContractorId"]);

                if (decryptedParameters.Count() > 0)
                {
                    #region
                    arrBankDetails = masterDAL.GetdetailsforPFMSValidation(AccountCode, ContractorCode);
                    if (arrBankDetails[2].Trim() == "I")
                    {
                        return Json(new { success = false, message = "Bank Details are inactive, cannot finalize the Bank Details." }, JsonRequestBehavior.AllowGet);
                    }
                    if (!masterDAL.ValidatePFMSBankDetailsDAL(arrBankDetails[0], arrBankDetails[1]))
                    {
                        return Json(new { success = false, message = "Bank Name and IFSC Code are not entered as per RBI, cannot finalize the Bank Details." }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                    objBAL = new MasterBAL();
                    Boolean Result = objBAL.FinalizeBankDetails(AccountCode, ContractorCode);
                    if (Result)
                        return Json(new { success = true, message = "Account details finalized successfully." });
                    else
                        return Json(new { success = false, message = "Error occure whilw processing your request" });
                }
                return Json(new { success = false, message = "Error occure whilw processing your request" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occure whilw processing your request" });
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }

        }


        [HttpPost]
        public ActionResult ActivateBankAccountStatus(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            MasterDAL masterDAL = new MasterDAL();
            CommonFunctions comm = new CommonFunctions();
            string[] arrBankDetails = new string[3];
            try
            {
                objDAL = new MasterDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int AccountCode = Convert.ToInt32(decryptedParameters["AccountCode"]);
                int ContractorCode = Convert.ToInt32(decryptedParameters["ContractorId"]);

                if (decryptedParameters.Count() > 0)
                {
                    //  objBAL = new MasterBAL();
                    Boolean Result = objDAL.ActivateBankAccountStatusDAL(AccountCode, ContractorCode);
                    if (Result)
                        return Json(new { success = true, message = "Account Status activated successfully." });
                    else
                        return Json(new { success = false, message = "Error occured while processing your request" });
                }
                return Json(new { success = false, message = "Error occured while processing your request" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occured while processing your request" });
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }

        }
        #endregion BankDetails

        #region NodalOfficer

        /// <summary>
        /// Method to load Nodal Officer list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListNodalOfficer()
        {
            return View("ListNodalOfficer");
        }

        /// <summary>
        /// Method to Load ADD form.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddNodalOfficer()
        {
            List<SelectListItem> lstDistrict = new List<SelectListItem>();
            lstDistrict.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
            ViewBag.District = lstDistrict;
            return PartialView("AddNodalOfficer", new AdminNodalOfficerViewModel());
        }

        /// <summary>
        /// Method to add nodal officer details.
        /// </summary>
        /// <param name="nodalOfficerView"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNodalOfficer(AdminNodalOfficerViewModel nodalOfficerView)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddNodalOfficer(nodalOfficerView, ref message))
                    {
                        message = "Nodal Officer details saved successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Nodal Officer details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddNodalOfficer", nodalOfficerView);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Nodal Officer details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadProductImage(Guid ProductId)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    if (Request.Files[0].ContentLength > 0)
                    {
                        HttpPostedFileBase postedFile = Request.Files[0];
                        string filename = System.IO.Path.GetFileName(Request.Files[0].FileName);
                        string strLocation = HttpContext.Server.MapPath("~/images/Thumb");
                        Request.Files[0].SaveAs(strLocation + @"\" + filename.Replace('+', '_'));
                    }
                }
                catch (FormatException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    return Content(ex.Message);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    throw ex;
                }
            }
            return PartialView("AddEditMasterQualityMonitor", new MasterAdminQualityMonitorViewModel());
        }

        /// <summary>
        /// Method to set existing record into the form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditNodalOfficer(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameter = null;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameter = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameter.Count() > 0)
                {
                    AdminNodalOfficerViewModel nodalOfficerView = objBAL.GetAdminNodalOfficer_ByOfficerCode(Convert.ToInt32(decryptedParameter["NodalOfficerCode"].ToString()));
                    // ViewBag.District = nodalOfficerView.MAST_DISTRICT_CODE.ToString();
                    List<SelectListItem> lstDistrict = new List<SelectListItem>();
                    lstDistrict = objBAL.PopulateDistrict(nodalOfficerView.ADMIN_ND_CODE.ToString());
                    if (nodalOfficerView.MAST_DISTRICT_CODE != null)
                    {
                        if (lstDistrict.Any(m => m.Value == nodalOfficerView.MAST_DISTRICT_CODE.ToString()))
                        {

                            var selected = lstDistrict.Where(x => x.Value == nodalOfficerView.MAST_DISTRICT_CODE.ToString()).First();
                            selected.Selected = true;
                        }
                    }
                    else
                    {
                        lstDistrict.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                    }

                    ViewBag.District = lstDistrict;
                    if (nodalOfficerView == null)
                    {
                        ModelState.AddModelError(string.Empty, "Nodal Officer details not exist.");
                        return PartialView("AddNodalOfficer", new AdminNodalOfficerViewModel());

                    }
                    return PartialView("AddNodalOfficer", nodalOfficerView);
                }
                return PartialView("AddNodalOfficer", new AdminNodalOfficerViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Nodal Officer details not exist.");

                return PartialView("AddNodalOfficer", new AdminNodalOfficerViewModel());

            }
        }

        /// <summary>
        /// Method to Update nodal officer details.
        /// </summary>
        /// <param name="nodalOfficerView"></param>
        /// <returns></returns>
        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult EditNodalOfficer(AdminNodalOfficerViewModel nodalOfficerView)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditNodalOfficer(nodalOfficerView, ref message))
                    {
                        message = "Nodal Officer details updated successfully.";
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "Nodal Officer details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddNodalOfficer", nodalOfficerView);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Nodal Officer details not updated.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete nodal officer details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteNodalOfficer(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteNodalOfficer(Convert.ToInt32(decryptedParameters["NodalOfficerCode"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "You can not delete this Nodal Officer details.");
                        return Json(new { success = false, message = "You can not delete this nodal Officer details." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = "Nodal Officer details deleted successfully." }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { success = false, message = "You can not delete this Nodal Officer details." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this Nodal Officer details." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to set existing record into the form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewNodalOfficer(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameter = null;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameter = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameter.Count() > 0)
                {
                    AdminNodalOfficerViewModel nodalOfficerView = objBAL.GetAdminNodalOfficer_ByOfficerCode(Convert.ToInt32(decryptedParameter["NodalOfficerCode"].ToString()));
                    if (nodalOfficerView == null)
                    {
                        ModelState.AddModelError(string.Empty, "Nodal Officer details not exist.");
                        return PartialView("ViewNodalOfficer", new AdminNodalOfficerViewModel());

                    }
                    return PartialView("ViewNodalOfficer", nodalOfficerView);
                }
                return PartialView("ViewNodalOfficer", new AdminNodalOfficerViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Nodal Officer details not exist.");

                return PartialView("ViewNodalOfficer", new AdminNodalOfficerViewModel());

            }
        }


        /// <summary>
        /// Method to load nodal officer list.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>


        public ActionResult GetNodalOfficerDetails(int? page, int? rows, string sidx, string sord)
        {

            String searchParameters = String.Empty;
            long totalRecords;
            int officeCode = 0;
            int designationCode = 0;
            int stateCode = 0;
            int noTypeCode = 0;
            string moduleType = "P";
            string activflag = "Y";
            objBAL = new MasterBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["officeCode"]))
            {
                officeCode = Convert.ToInt32(Request.Params["officeCode"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["designationCode"]))
            {
                designationCode = Convert.ToInt32(Request.Params["designationCode"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["StateCode"]))
            {
                //stateCode = Convert.ToInt32(Request.Params["StateCode"]);
                stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(Request.Params["StateCode"]) : PMGSYSession.Current.StateCode;

            }
            if (!string.IsNullOrEmpty(Request.Params["NoTypeCode"]))
            {
                noTypeCode = Convert.ToInt32(Request.Params["NoTypeCode"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["ModuleType"]))
            {
                moduleType = Request.Params["ModuleType"];
            }
            if (!string.IsNullOrEmpty(Request.Params["Active"]))
            {
                activflag = Request.Params["Active"];
            }
            var jsonData = new
            {
                rows = objBAL.ListNodalOfficer(stateCode, officeCode, designationCode, noTypeCode, moduleType, activflag, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method for search functionality.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchNodalOfficer()
        {
            try
            {
                objDAL = new MasterDAL();
                //List<ADMIN_DEPARTMENT> ndNameList = objDAL.GetAdminNdCode();

                //switch (PMGSYSession.Current.RoleCode)
                //{
                //    case 22:
                //        break;
                //    case 2:
                //        break;
                //    case 37:
                //        break;
                //    case 38:
                //        break;
                //    case 36:
                //        break;
                //    default:
                //        ndNameList.Insert(0, new ADMIN_DEPARTMENT { ADMIN_ND_CODE = 0, ADMIN_ND_NAME = "All Offices" });
                //        break;
                //}
                List<ADMIN_DEPARTMENT> ndNameList = new List<ADMIN_DEPARTMENT>();
                ndNameList.Insert(0, new ADMIN_DEPARTMENT { ADMIN_ND_CODE = 0, ADMIN_ND_NAME = "All Offices" });
                objDAL = new MasterDAL();
                List<MASTER_STATE> lstState = objDAL.GetAllStates();
                lstState.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" });
                List<MASTER_PROFILE> lstNoType = new List<MASTER_PROFILE>();
                lstNoType = objDAL.GetAdminNoType();
                if (PMGSYSession.Current.RoleCode != 47) //ITNOOA=47 RoleCode
                {
                    lstNoType.Insert(0, new MASTER_PROFILE { MAST_PROFILE_CODE = 0, MAST_PROFILE_NAME = "All Type" });
                }

                //List<SelectListItem> stateDd = common.PopulateStates(false);
                //int stateCode = PMGSYSession.Current.StateCode;
                //if (stateCode > 0)  //if state login
                //{
                //    lstState.Find(x => x.MAST_STATE_CODE == stateCode).Selected = true;
                //}

                List<SelectListItem> lstModuleType = new List<SelectListItem>();
                lstModuleType.Insert(0, (new SelectListItem { Text = "P", Value = "P", Selected = true }));
                // lstModuleType.Insert(1, (new SelectListItem { Text = "A", Value = "A"}));
                lstModuleType.Insert(1, (new SelectListItem { Text = "M", Value = "M" }));


                ViewData["STATE"] = new SelectList(lstState, "MAST_STATE_CODE", "MAST_STATE_NAME", PMGSYSession.Current.StateCode);

                ViewData["NdNameList"] = new SelectList(ndNameList, "ADMIN_ND_CODE", "ADMIN_ND_NAME");
                List<MASTER_DESIGNATION> designationList = objDAL.GetNodalDesignation();
                designationList.Insert(0, new MASTER_DESIGNATION() { MAST_DESIG_CODE = 0, MAST_DESIG_NAME = "All designations" });
                ViewData["DesignationList"] = new SelectList(designationList, "MAST_DESIG_CODE", "MAST_DESIG_NAME");
                ViewData["NOType"] = new SelectList(lstNoType, "MAST_PROFILE_CODE", "MAST_PROFILE_NAME");
                ViewData["ModuleType"] = lstModuleType;
                List<SelectListItem> StatusList = new List<SelectListItem>();
                StatusList.Add(new SelectListItem { Text = "All", Value = "%" });
                StatusList.Add(new SelectListItem { Text = "Yes", Value = "Y", Selected = true });
                StatusList.Add(new SelectListItem { Text = "No", Value = "N" });
                ViewData["Active"] = StatusList;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["NdNameList"] = null;
                ViewData["DistrictList"] = null;
                ViewData["DesignationList"] = null;
                ViewData["ModuleType"] = null;
                ViewData["NOType"] = null;
            }
            return PartialView("SearchNodalOfficer");
        }

        [HttpPost]
        public JsonResult PopulateDistrict(string id)
        {
            //int adminNdCode = Convert.ToInt32(id);
            objBAL = new MasterBAL();
            List<SelectListItem> lstDistrict = objBAL.PopulateDistrict(id);
            lstDistrict.Insert(0, new SelectListItem { Text = "Select District", Value = "0" });
            return Json(lstDistrict, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult PopulateAdminNd_ByStateCode()
        {
            int stateCode = 0;
            if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
            {
                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }
            objDAL = new MasterDAL();
            List<ADMIN_DEPARTMENT> ndNameList = objDAL.GetAdminNdCode_ByStateCode(stateCode);
            ndNameList.Insert(0, new ADMIN_DEPARTMENT { ADMIN_ND_CODE = 0, ADMIN_ND_NAME = "All Offices" });
            return Json(new SelectList(ndNameList, "ADMIN_ND_CODE", "ADMIN_ND_NAME", PMGSY.Extensions.PMGSYSession.Current.AdminNdCode), JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult GetStateCodeByAdminNdCode()
        {
            db = new PMGSYEntities();
            int statCode = 0;
            try
            {
                int adminNdCode = 0;
                if (!string.IsNullOrEmpty(Request.Params["AdminNdCode"]))
                {
                    adminNdCode = Convert.ToInt32(Request.Params["AdminNdCode"]);
                }
                statCode = db.ADMIN_DEPARTMENT.Where(a => a.ADMIN_ND_CODE == adminNdCode).Select(a => a.MAST_STATE_CODE).FirstOrDefault();
                return Json(new { statCode = statCode }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                // return Json(false);
                return Json(new { statCode = statCode }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }
        #endregion NodalOfficer

        #region BankDetails NO
        /// <summary>
        /// This method is for loading the list of contractor bank details.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns view of list</returns>
        [HttpGet]
        public ActionResult ListBankDetailsNO(String id)
        {
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            MasterContractorBankDetails contractorBankDetails = new MasterContractorBankDetails();
            int ContractorId = 0;
            int RegState = 0;
            try
            {
                if (id != string.Empty)
                {
                    encryptedParameters = id.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        //ViewBag.ContractorId = decryptedParameters["ContRegID"].ToString();
                        //ViewData["RegStateCode"] = decryptedParameters["RegState"].ToString();
                        //ContractorId = Convert.ToInt32(decryptedParameters["ContRegID"]);
                        //RegState = Convert.ToInt32(decryptedParameters["RegState"]);

                        contractorBankDetails.encrNodalOfficerCode = id;
                        contractorBankDetails.NodalOfficerCode = Convert.ToInt32(decryptedParameters["NodalOfficerCode"]);
                    }



                    return PartialView("ListBankDetailsNO", contractorBankDetails);
                }
                return PartialView("ListBankDetailsNO", contractorBankDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ListBankDetailsNO", contractorBankDetails);
            }
        }
        /// <summary>
        /// This method is for loading the Add functionality form.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View of Add screen</returns>
        [HttpGet]
        public ActionResult AddBankDetailsNO(string id)
        {
            objDAL = new MasterDAL();
            MasterContractorBankDetails model = new MasterContractorBankDetails();
            string[] strsplit = id.Split('$');
            ViewBag.NodalOfficerCode = strsplit[0].ToString();
            List<MASTER_DISTRICT> lstDistricts = null;
            List<MASTER_STATE> lstState = null;
            lstState = objDAL.GetStates();
            lstState.Insert(0, new MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });
            int stateCode = 0;


            //if (PMGSY.Extensions.PMGSYSession.Current.StateCode == 0)
            //{
            //    //lstDistricts = objDAL.getContractorDistricts(Convert.ToInt32(strsplit[0])); ///Coomented by deepak

            //    stateCode = Convert.ToInt32(strsplit[1]);
            //    lstDistricts = objDAL.GetAllDistrictsByStateCode(stateCode);
            //}
            //else
            //{
            //    lstDistricts = objDAL.GetAllDistrictsByStateCode(PMGSY.Extensions.PMGSYSession.Current.StateCode);
            //    lstDistricts.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });
            //    stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
            //}
            stateCode = PMGSYSession.Current.StateCode;//Convert.ToInt32(strsplit[1]);
            lstDistricts = objDAL.GetAllDistrictsByStateCode(stateCode);
            lstDistricts.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });

            model.Mast_State_Code = stateCode;
            ViewData["Districts"] = new SelectList(lstDistricts, "MAST_DISTRICT_CODE", " MAST_DISTRICT_NAME");
            ViewData["State"] = new SelectList(lstState, "MAST_STATE_CODE", " MAST_STATE_NAME", stateCode);
            return PartialView("AddBankDetailsNO", model);
        }

        /// <summary>
        /// This method is for Adding the bank details
        /// </summary>
        /// <param name="contractorBankDetails"></param>
        /// <returns>View for Add form </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBankDetailsNO(MasterContractorBankDetails contractorBankDetails)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddBankDetailsNO(contractorBankDetails, ref message))
                    {
                        message = message == string.Empty ? "Bank details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Bank details not saved. " : message;
                    }
                }
                else
                {
                    return PartialView("AddBankDetailsNO", contractorBankDetails);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Bank details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is for checking existing bank details.
        /// </summary>
        /// <param name="contractorBankDetails"></param>
        /// <returns>Returns true if  bank details already exists.</returns>
        [HttpPost]
        public ActionResult CheckExistRecordNO(MasterContractorBankDetails contractorBankDetails)
        {
            bool status = true;
            bool alreadyExists = false;
            bool isBankDetailsExists = false;

            try
            {
                if (ModelState.IsValid)
                {
                    objDAL = new MasterDAL();

                    //Added By Abhishke kamble 20-feb-2014 
                    if (objDAL.IsBankDetailsExistsNO(contractorBankDetails, ref message))
                    {
                        status = true;
                        return Json(new { success = status, isBankDetailsExists = true, message = message });
                    }

                    if (objDAL.checkAlreadyExistsNO(contractorBankDetails, ref message))
                    {
                        status = true;
                        alreadyExists = true;
                    }
                }

                return Json(new { success = status, alreadyExists = alreadyExists, isBankDetailsExists = isBankDetailsExists }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Bank details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is for loading existing record into the form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditBankDetailsNO(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objDAL = new MasterDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int AccountCode = Convert.ToInt32(decryptedParameters["AccountCode"]);
                int NodalOfficerCode = Convert.ToInt32(decryptedParameters["NodalOfficerCode"]);
                ViewBag.NodalOfficerCode = NodalOfficerCode.ToString();
                // List<MASTER_DISTRICT> lstDistricts = objDAL.getContractorDistricts(ContractorCode);
                List<MASTER_DISTRICT> lstDistricts = null;
                List<MASTER_STATE> lstState = null;
                lstState = objDAL.GetStates();
                lstState.Insert(0, new MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });



                if (decryptedParameters.Count() > 0)
                {
                    objBAL = new MasterBAL();

                    MasterContractorBankDetails contractorBankDetails = objBAL.getBankDetailsNO_ByBankCode(AccountCode, NodalOfficerCode);
                    db = new PMGSYEntities();
                    var stateCode = db.MASTER_DISTRICT.Where(a => a.MAST_DISTRICT_CODE == contractorBankDetails.MAST_DISTRICT_CODE).Select(a => a.MAST_STATE_CODE).FirstOrDefault();

                    contractorBankDetails.Mast_State_Code = Convert.ToInt32(stateCode.ToString());

                    lstDistricts = objDAL.GetAllDistrictsByStateCode(contractorBankDetails.Mast_State_Code);
                    lstDistricts.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });
                    ViewData["Districts"] = new SelectList(lstDistricts, "MAST_DISTRICT_CODE", " MAST_DISTRICT_NAME", contractorBankDetails.MAST_DISTRICT_CODE);

                    ViewData["State"] = new SelectList(lstState, "MAST_STATE_CODE", " MAST_STATE_NAME", contractorBankDetails.Mast_State_Code);
                    if (contractorBankDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Bank details not exist.");
                        return PartialView("AddBankDetailsNO", new MasterContractorBankDetails());
                    }
                    return PartialView("AddBankDetailsNO", contractorBankDetails);
                }
                return PartialView("AddBankDetailsNO", new MasterContractorBankDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Bank details not exist.");
                return PartialView("AddBankDetailsNO", new MasterContractorBankDetails());
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }

        }

        /// <summary>
        /// This method is to update the record
        /// </summary>
        /// <param name="contractorBankDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditBankDetailsNO(MasterContractorBankDetails contractorBankDetails)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditBankDetailsNO(contractorBankDetails, ref message))
                    {
                        message = message == string.Empty ? "Bank details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Bank details not updated." : message;
                    }
                }
                //else
                //{
                //    return PartialView("AddBankDetails", contractorBankDetails);
                //}
                else
                {
                    string messages = string.Join("; ", ModelState.Values
                                           .SelectMany(x => x.Errors)
                                           .Select(x => x.ErrorMessage));

                    return Json(new { success = status, message = messages }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Bank details not updated.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// This method is to delete the bank details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteBankDetailsNO(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int AccountCode = Convert.ToInt32(decryptedParameters["AccountCode"]);
                int ContractorCode = Convert.ToInt32(decryptedParameters["NodalOfficerCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteBankDetailsNO(AccountCode, ContractorCode, ref message))
                    {
                        message = "Bank details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Bank details." : message;
                    }
                    return Json(new { success = status, message = "Bank details deleted successfully." }, JsonRequestBehavior.AllowGet);
                }
                message = "You can not delete this Bank details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Bank details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is loading the grid for bank details.
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>


        public ActionResult GetContractorBankDetailsNO(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;

            int NodalOfficerCode = 0;

            try
            {
                //Added By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Added By Abhishek kamble 29-Apr-2014 end

                objBAL = new MasterBAL();
                encryptedParameters = Request.Params["NodalOfcCode"].Split('/');
                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    NodalOfficerCode = Convert.ToInt32(decryptedParameters["NodalOfficerCode"].ToString());
                }

                var jsonData = new
                {
                    rows = objBAL.ListBankDetailsNO(NodalOfficerCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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


        #endregion BankDetails

        #region Agency-State-District-Mapping

        /// <summary>
        /// Map Agency to States
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MapAgencyStates(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    ViewBag.EncryptedAgencyCode = parameter + '/' + hash + '/' + key;

                    ViewBag.AgencyName = decryptedParameters["TAName"].ToString();

                    return PartialView("MapAgencyStates");
                }
                return PartialView("MapAgencyStates");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MapAgencyStates");
            }
        }

        /// <summary>
        /// Map Agency to States
        /// </summary>
        /// <param name="frmCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MapAgencyStates(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedStateCodes = string.Empty;
            string encryptedAgencyCode = string.Empty;
            string startDate = string.Empty;
            objBAL = new MasterBAL();

            try
            {
                if (string.IsNullOrEmpty(frmCollection["EncryptedStateCodes"]) || string.IsNullOrEmpty(frmCollection["EncryptedAgencyCode"]))
                {
                    message = "Agency and District are not map.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(frmCollection["StartDateState"]))
                {
                    message = "Start Date is not Selected";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                encryptedStateCodes = frmCollection["EncryptedStateCodes"];
                encryptedAgencyCode = frmCollection["EncryptedAgencyCode"];
                startDate = frmCollection["StartDateState"];             //change by Ujjwal Saket on 28-10-2013

                if (objBAL.MapAgencyStatesBAL(encryptedAgencyCode, encryptedStateCodes, startDate))
                {

                    message = "Agency and State are mapped successfully.";
                    status = true;
                }
                else
                {
                    message = "Agency and State are not mapped.";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Agency and State are not mapped.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get State Details List Mapping
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStateDetailsList_Mapping(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = string.Empty;
            Dictionary<string, string> decryptedParameters = new Dictionary<string, string>();
            IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();
            long totalRecords;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetStateDetailsListBAL(true, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, 0, 0),
                    total = 0,
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

        /// <summary>
        /// Map Agency to Districts
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>


        public ActionResult MapAgencyDistricts(String parameter, String hash, String key)
        {
            try
            {

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                //added by Ujjwal Saket on 6/1/2014
                Int32 adminTaCode = 0;
                string adminTaType = string.Empty;
                Int32? stateCode = 0;
                if (decryptedParameters.Count > 0)
                {

                    List<Models.MASTER_STATE> stateList = new List<Models.MASTER_STATE>();
                    stateList = masterDataEntryDAL.GetAllStates(true);
                    adminTaCode = Convert.ToInt32(decryptedParameters["TACode"].Trim());
                    using (db = new PMGSYEntities())
                    {
                        adminTaType = db.ADMIN_TECHNICAL_AGENCY.Where(x => x.ADMIN_TA_CODE == adminTaCode).Select(x => x.ADMIN_TA_TYPE).FirstOrDefault();
                        if (adminTaType.Equals("S"))
                        {
                            stateCode = db.ADMIN_TECHNICAL_AGENCY.Where(x => x.ADMIN_TA_CODE == adminTaCode).Select(x => x.MAST_STATE_CODE).FirstOrDefault();
                        }
                    }
                    if (stateCode != null)
                    {
                        ViewData["StateList"] = new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode);
                    }
                    else
                    {
                        ViewData["StateList"] = new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");
                    }

                    ViewBag.EncryptedAgencyCode = parameter + '/' + hash + '/' + key;

                    ViewBag.AgencyName = decryptedParameters["TAName"].ToString();

                    return PartialView("MapAgencyDistricts");
                }
                return PartialView("MapAgencyDistricts");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MapAgencyDistricts");
            }
        }

        /// <summary>
        /// Get District Details List of Mapping Agency
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDistrictDetailsList_Mapping_Agency(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = string.Empty;
            String[] agency = null;
            int agencyCode;
            int regionCode = 0;
            int adminNdCode = 0;
            Dictionary<string, string> decryptedParameters = new Dictionary<string, string>();
            IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();
            long totalRecords;
            int stateCode = 0;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                stateCode = Convert.ToInt32(Request.Params["StateCode"].ToString());

                agency = Convert.ToString(Request.Params["AgencyCode"]).Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { agency[0], agency[1], agency[2] });
                agencyCode = Convert.ToInt32(decryptedParameters["TACode"]);
                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetDistrictDetailsListBAL(agencyCode, regionCode, adminNdCode, true, DAL.MappingType.AgencyDistrict, stateCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = 0,
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

        /// <summary>
        /// Map Agency to Districts
        /// </summary>
        /// <param name="frmCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MapAgencyDistricts(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedDistrictCodes = string.Empty;
            string encryptedAgencyCode = string.Empty;
            string startDate = string.Empty;
            objBAL = new MasterBAL();
            try
            {
                if (string.IsNullOrEmpty(frmCollection["EncryptedDistrictCodes"]) || string.IsNullOrEmpty(frmCollection["EncryptedAgencyCode"]))
                {
                    message = "Agency and District are not map.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(frmCollection["StartDate"]))
                {
                    message = "Start Date is not Selected";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                encryptedDistrictCodes = frmCollection["EncryptedDistrictCodes"];
                encryptedAgencyCode = frmCollection["EncryptedAgencyCode"];
                startDate = frmCollection["StartDate"];     //change by Ujjwal Saket on 28-10-2013

                if (objBAL.MapAgencyDistrictsBAL(encryptedAgencyCode, encryptedDistrictCodes, startDate))
                {

                    message = "Agency and District are mapped successfully.";
                    status = true;
                }
                else
                {
                    message = "Agency and District are not mapped.";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Agency and District are not mapped.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// MappedA gency State and Districts List
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MappedAgencyStateandDistricts(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                CommonFunctions comFunction = new CommonFunctions();
                List<SelectListItem> lstStates = new List<SelectListItem>();
                lstStates = comFunction.PopulateStates(true);
                lstStates.RemoveAt(0);
                lstStates.Insert(0, new SelectListItem { Value = "0", Text = "All State" });

                ViewBag.StateList = lstStates;
                //ViewBag.StateList = comFunction.PopulateStates(true);               

                if (decryptedParameters.Count > 0)
                {
                    ViewBag.EncryptedAgencyCode_Mapped = parameter + '/' + hash + '/' + key;

                    ViewBag.AgencyName = decryptedParameters["TAName"].ToString();

                    ViewBag.AgencyType = decryptedParameters["TAType"].ToString();

                    return PartialView("MappedAgencyStateandDistricts");
                }


                return PartialView("MappedAgencyStateandDistricts");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MappedAgencyStateandDistricts");
            }
        }

        //added by Ujjwal Saket on 29-10-2013 for Finalizing the Mapped State
        /// <summary>
        /// Finalizing Mapped States
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        public ActionResult FinalizeMappedStateAgency(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int adminId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                adminId = Convert.ToInt32(decryptedParameters["AdminId"]);
                objBAL = new MasterBAL();
                bool status = objBAL.FinalizeMappedStateAgencyBAL(adminId);
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
        }//finish addition


        //Added by Ujjwal Saket on 8/1/2014 to return View for adding the end dates for PTA login
        /// <summary>
        /// Method to return View for adding the end dates for PTA login
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEndDateState(String parameter, String hash, String key)
        {

            Int32 adminTaId = 0;
            objBAL = new MasterBAL();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                adminTaId = Convert.ToInt32(decryptedParameters["AdminId"].Trim());


                if (decryptedParameters.Count() > 0 && adminTaId != 0)
                {
                    EndDateStateViewModel endDateStateViewModel = new EndDateStateViewModel();
                    //endDateDistrictViewModel.EncryptedAdminId = URLEncrypt.EncryptParameters1(new string[] { "AdminId=" + adminTaId.ToString() });
                    endDateStateViewModel = objBAL.AddEndDateStateBAL(endDateStateViewModel, adminTaId);

                    return View("AddEndDateState", endDateStateViewModel);
                }

                return View("MappedAgencyStateandDistricts");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return View("MappedAgencyStateandDistricts");
            }


        }



        //added by Ujjwal Saket on 30-10-2013 for Updating the End Dates of States
        /// <summary>
        /// Updating the End Dates of States
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateStateEndDatePTA(FormCollection formCollection)
        {
            string encryptedAdminId = string.Empty;
            string endDate = string.Empty;
            string message = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(formCollection["EndDateState"].ToString()))
                {
                    message = "End Date must not be empty.";
                    return Json(new { success = false, message = message });
                }
                encryptedAdminId = formCollection["EncryptedAdminId"].ToString();
                endDate = formCollection["EndDateState"].ToString();

                objBAL = new MasterBAL();
                bool status = objBAL.UpdateStateEndDatePTA_BAL(endDate, encryptedAdminId, ref message);
                if (status == true)
                {
                    return Json(new { success = true });
                }
                else if (status == false && !string.IsNullOrEmpty(message))
                {
                    return Json(new { success = false, message = message });
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


        }//finish addition


        /// <summary>
        /// Get Mapped State Details List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStateDetailsList_Mapped(int? page, int? rows, string sidx, string sord)
        {
            Dictionary<string, string> decryptedParameters = new Dictionary<string, string>();
            long totalRecords;
            int agencyCode = 0;
            String[] encryptedParameters = null;
            objBAL = new MasterBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                encryptedParameters = Request.Params["AgencyCode"].Split('/');

                //Added By Abhishek kamble 3-Apr-2014
                int StateCode = Convert.ToInt32(Request.Params["StateCode"]);


                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    agencyCode = Convert.ToInt32(decryptedParameters["TACode"].ToString());
                }

                var jsonData = new
                {
                    rows = objBAL.GetMappedStateDetailsListBAL_Agency(StateCode, agencyCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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


        //added by Ujjwal Saket on 29-10-2013 for Finalizing the Mapped District
        /// <summary>
        /// Finalizing Mapped District
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        public ActionResult FinalizeMappedDistrictAgency(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int adminId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                adminId = Convert.ToInt32(decryptedParameters["AdminId"]);
                objBAL = new MasterBAL();
                bool status = objBAL.FinalizeMappedDistrictAgencyBAL(adminId);
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
        }//finish addition


        //Added by Ujjwal Saket on 7/1/2014 to return View for adding the end dates for STA login
        /// <summary>
        /// Method to return View for adding the end dates for STA login
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEndDateDistrict(String parameter, String hash, String key)
        {

            Int32 adminTaId = 0;
            objBAL = new MasterBAL();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                adminTaId = Convert.ToInt32(decryptedParameters["AdminId"].Trim());


                if (decryptedParameters.Count() > 0 && adminTaId != 0)
                {
                    EndDateDistrictViewModel endDateDistrictViewModel = new EndDateDistrictViewModel();
                    //endDateDistrictViewModel.EncryptedAdminId = URLEncrypt.EncryptParameters1(new string[] { "AdminId=" + adminTaId.ToString() });
                    endDateDistrictViewModel = objBAL.AddEndDateDistrictBAL(endDateDistrictViewModel, adminTaId);
                    return View("AddEndDateDistrict", endDateDistrictViewModel);
                }

                return View("MappedAgencyStateandDistricts");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return View("MappedAgencyStateandDistricts");
            }


        }


        //added by Ujjwal Saket on 30-10-2013 for Updating the End Dates of District
        /// <summary>
        /// Updating the End Dates of District
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateDistrictEndDateSTA(FormCollection formCollection)
        {

            string encryptedAdminId = string.Empty;
            string endDate = string.Empty;
            string message = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(formCollection["EndDateDistrict"].ToString()))
                {
                    return Json(new { success = false });
                }
                encryptedAdminId = formCollection["EncryptedAdminId"].ToString();
                endDate = formCollection["EndDateDistrict"].ToString();

                objBAL = new MasterBAL();
                bool status = objBAL.UpdateDistrictEndDateSTA_BAL(endDate, encryptedAdminId, ref message);
                if (status == true)
                {
                    return Json(new { success = true });
                }
                else if (status == false && !string.IsNullOrEmpty(message))
                {
                    return Json(new { success = false, message = message });
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
        }//finish addition





        /// <summary>
        /// Get Mapped Agency District Details List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDistrictDetailsList_Mapped_Agency(int? page, int? rows, string sidx, string sord)
        {
            Dictionary<string, string> decryptedParameters = new Dictionary<string, string>();
            long totalRecords;
            int agencyCode = 0;
            String[] encryptedParameters = null;
            objBAL = new MasterBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                encryptedParameters = Request.Params["AgencyCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    agencyCode = Convert.ToInt32(decryptedParameters["TACode"].ToString());
                }

                var jsonData = new
                {
                    rows = objBAL.GetMappedDistrictDetailsListBAL_Agency(agencyCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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


        // new change by Vikram on 24-08-2013



        public ActionResult DeleteMappedState(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int adminId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                adminId = Convert.ToInt32(decryptedParameters["AdminId"]);
                objBAL = new MasterBAL();
                bool status = objBAL.DeleteMappedStateAgencyBAL(adminId);
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


        public ActionResult DeleteMappedDistrict(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int adminId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                adminId = Convert.ToInt32(decryptedParameters["AdminId"]);
                objBAL = new MasterBAL();
                bool status = objBAL.DeleteMappedDistrictAgencyBAL(adminId);
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


        #endregion Agency-State-District-Mapping

        #region SRRDA-District-Mapping

        /// <summary>
        /// Map SRRDA Districts
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        public ActionResult MapSRRDADistricts(String parameter, String hash, String key)
        {
            AdminDepartmentViewModel objDept = new AdminDepartmentViewModel();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    objDept.EncryptedMapAdminCode = parameter + '/' + hash + '/' + key;

                    ViewBag.SRRDAName = decryptedParameters["AdminName"].ToString();
                    ViewBag.StateName = decryptedParameters["StateName"].ToString();

                    return PartialView("MapSRRDADistricts", objDept);
                }
                return PartialView("MapSRRDADistricts");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MapSRRDADistricts");
            }
        }

        /// <summary>
        /// Map SRRDA Districts
        /// </summary>
        /// <param name="frmCollection"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult MapSRRDADistricts(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedDistrictCodes = string.Empty;
            string encryptedAdminCode = string.Empty;
            objBAL = new MasterBAL();
            try
            {
                encryptedDistrictCodes = frmCollection["EncryptedDistrictCodes"];
                //encryptedAdminCode = frmCollection["EncryptedAdminCode"];
                encryptedAdminCode = frmCollection["EncryptedMapAdminCode"];    //edited by Ujjwal Saket on 25-10-2013 for mapping the districts and SRRDA

                if (objBAL.MapSRRDADistrictsBAL(encryptedAdminCode, encryptedDistrictCodes))
                {
                    message = "SRRDA and District are mapped successfully.";
                    status = true;
                }
                else
                {
                    message = "SRRDA and District are not mapped.";
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "SRRDA and District are not mapped.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Get District Details List for Mapping_SRRDA
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult GetDistrictDetailsList_Mapping_SRRDA(FormCollection homeFormCollection)
        {
            string[] agency = null;
            int agencyCode = 0;
            int regionCode = 0;
            int adminNdCode = 0;
            String searchParameters = string.Empty;
            Dictionary<string, string> decryptedParameters = new Dictionary<string, string>();
            IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();
            long totalRecords;
            int stateCode = 0;
            String[] encryptedParameters = null;

            /*  if (Session["user"] == null)
              {
                  throw new Exception("Please login again!!");
              }
              if (!ModelState.IsValid)
              {
                  return Json(false);
              }*/

            //if (!ValidateGridParameters(homeFormCollection))
            //    {
            //        return Json(false);
            //    }

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["AgencyCode"]))
                {
                    agency = Convert.ToString(Request.Params["AgencyCode"]).Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { agency[0], agency[1], agency[2] });
                    agencyCode = Convert.ToInt32(decryptedParameters["TACode"]);
                }

                encryptedParameters = homeFormCollection["AdminCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    stateCode = Convert.ToInt32(decryptedParameters["StateCode"].ToString());
                    adminNdCode = Convert.ToInt32(decryptedParameters["AdminCode"].ToString());
                }
                var jsonData = new
                {
                    rows = masterDataEntryBAL.GetDistrictDetailsListBAL(agencyCode, regionCode, adminNdCode, true, DAL.MappingType.SRRDADistrict, stateCode, Convert.ToInt32(homeFormCollection["page"]) - 1, Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], out totalRecords), //RegistredDocumentsDetails.GetSearchResultList(partyName, districtCode, villageCode, fromDate, toDate, Convert.ToInt32(homeFormCollection["page"]) - 1, Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], out totalRecords),
                    total = 0,//totalRecords <= Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
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

        /// <summary>
        /// Mapped SRRDA Districts
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        public ActionResult MappedSRRDADistricts(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    ViewBag.EncryptedAdminCode_Mapped = parameter + '/' + hash + '/' + key;

                    ViewBag.SRRDAName = decryptedParameters["AdminName"].ToString();
                    ViewBag.StateName = decryptedParameters["StateName"].ToString();

                    return PartialView("MappedSRRDADistricts");
                }
                return PartialView("MappedSRRDADistricts");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("MappedSRRDADistricts");
            }
        }


        /// <summary>
        /// Get Mapped_SRRDA District Details List
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult GetDistrictDetailsList_Mapped_SRRDA(FormCollection homeFormCollection)
        {
            Dictionary<string, string> decryptedParameters = new Dictionary<string, string>();
            long totalRecords;
            int adminCode = 0;
            String[] encryptedParameters = null;
            objBAL = new MasterBAL();
            /*  if (Session["user"] == null)
              {
                  throw new Exception("Please login again!!");
              }
              if (!ModelState.IsValid)
              {
                  return Json(false);
              }*/

            //if (!ValidateGridParameters(homeFormCollection))
            //    {
            //        return Json(false);
            //    }

            try
            {

                encryptedParameters = homeFormCollection["AdminCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    adminCode = Convert.ToInt32(decryptedParameters["AdminCode"].ToString());
                }

                var jsonData = new
                {
                    rows = objBAL.GetMappedDistrictDetailsListBAL_SRRDA(adminCode, Convert.ToInt32(homeFormCollection["page"]) - 1, Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
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



        public ActionResult DeleteMappedSRRDADistrict(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int adminId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                adminId = Convert.ToInt32(decryptedParameters["AdminId"]);
                objBAL = new MasterBAL();
                bool status = objBAL.DeleteMappedSRRDADistrictBAL(adminId);
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

        #endregion SRRDA-District-Mapping


        /// <summary>
        /// Get Vidhan Sabha Term number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetVidhanSabhaTerm(string id)
        {
            int vidhanSabhaTerm = 0;
            try
            {
                objDAL = new MasterDAL();
                vidhanSabhaTerm = objDAL.GetVidhanSabhaTermDAL(Convert.ToInt32(id.Trim()));
                return Json(new { status = true, vidhanSabhaTerm = vidhanSabhaTerm }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { status = false, vidhanSabhaTerm = vidhanSabhaTerm }, JsonRequestBehavior.AllowGet);
            }
        }//end function GetVidhanSabhaTerm

        /// <summary>
        /// Get Lok  Sabh Term Dates
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLokSabhaTermDates(string id)
        {
            string lokSabhaStartDate = string.Empty;
            string lokSabhaEndDate = string.Empty;
            bool status = false;
            objDAL = new MasterDAL();
            try
            {
                if (objDAL.GetLokSabhaTermDatesDAL(id == null ? 0 : Convert.ToInt32(id.Trim()), ref lokSabhaStartDate, ref lokSabhaEndDate))
                {
                    status = true;
                }
                return Json(new { status = status, lokSabhaStartDate = lokSabhaStartDate, lokSabhaEndDate = lokSabhaEndDate }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { status = status, lokSabhaStartDate = lokSabhaStartDate, lokSabhaEndDate = lokSabhaEndDate }, JsonRequestBehavior.AllowGet);
            }
        }//end function GetLokSabhaTermDates

        /// <summary>
        /// Get Vidhan Sabha Term Dates
        /// </summary>
        /// <param name="frmCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetVidhanSabhaTermDates(FormCollection frmCollection)
        {
            string vidhanSabhaStartDate = string.Empty;
            string vidhanSabhaEndDate = string.Empty;
            int stateCode = 0;
            int vidhanSabhaTerm = 0;
            bool status = false;
            objDAL = new MasterDAL();
            try
            {
                stateCode = Convert.ToInt32(frmCollection["stateCode"].ToString());
                vidhanSabhaTerm = Convert.ToInt32(frmCollection["vidhanSabhaTerm"].ToString());

                if (objDAL.GetVidhanSabhaTermDatesDAL(stateCode, vidhanSabhaTerm, ref vidhanSabhaStartDate, ref vidhanSabhaEndDate))
                {
                    status = true;
                }
                return Json(new { status = status, vidhanSabhaStartDate = vidhanSabhaStartDate, vidhanSabhaEndDate = vidhanSabhaEndDate }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { status = status, vidhanSabhaStartDate = vidhanSabhaStartDate, vidhanSabhaEndDate = vidhanSabhaEndDate }, JsonRequestBehavior.AllowGet);
            }
        }//end function GetVidhanSabhaTermDates

        /// <summary>
        /// Contractor Registration Change Status to InActive
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ContractorRegChangeStatus_InActive(String parameter, String hash, String key)
        {
            int conID = 0;
            int ConRegCode = 0;
            bool status = false;
            objBAL = new MasterBAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    conID = Convert.ToInt32(decryptedParameters["ContRegID"]);
                    ConRegCode = Convert.ToInt32(decryptedParameters["ContRegCode"]);

                    if (objBAL.ContractorRegChangeStatusBAL(conID, ConRegCode, true))
                    {
                        message = "Contractor/Supplier Registration DeActivated successfully.";
                        status = true;
                    }
                    else
                    {
                        message = "Contractor/Supplier Registration status not changed.";
                    }
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Contractor/Supplier Registration status not changed.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }//end function ContractorRegStatusToInActive

        /// <summary>
        /// Contractor Registration Change Status to Active
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ContractorRegChangeStatus_Active(String parameter, String hash, String key)
        {
            int conID = 0;
            int ConRegCode = 0;
            bool status = false;
            objBAL = new MasterBAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    conID = Convert.ToInt32(decryptedParameters["ContRegID"]);
                    ConRegCode = Convert.ToInt32(decryptedParameters["ContRegCode"]);

                    if (objBAL.ContractorRegChangeStatusBAL(conID, ConRegCode, false))
                    {
                        message = "Contractor/Supplier Registration Activated successfully.";
                        status = true;
                    }
                    else
                    {
                        message = "Contractor/Supplier Registration status not changed.";
                    }

                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Contractor/Supplier Registration status not changed.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }//end function ContractorRegStatusToActive

        #region MASTER_TAXES


        public ActionResult ListTaxDetails()
        {
            return View("ListTaxDetails");
        }


        public ActionResult AddEditTaxDetails()
        {
            MasterTaxViewModel taxModel = new MasterTaxViewModel();
            taxModel.Operation = "A";
            return PartialView("AddEditTaxDetails", taxModel);
        }

        [HttpPost]
        public JsonResult GetTaxDetailsList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            objBAL = new MasterBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetTaxDetailsListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
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


        public ActionResult EditTaxDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            MasterTaxViewModel taxModel = new MasterTaxViewModel();
            objBAL = new MasterBAL();
            //int taxCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                taxModel = objBAL.GetTaxDetails(Convert.ToInt32(decryptedParameters["TaxCode"]));
                return PartialView("AddEditTaxDetails", taxModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }


        public ActionResult DeleteTaxDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int taxCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                taxCode = Convert.ToInt32(decryptedParameters["TaxCode"]);
                objBAL = new MasterBAL();
                bool status = objBAL.DeleteTaxDetailsBAL(taxCode);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTaxDetails(MasterTaxViewModel taxModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddTaxDetailsBAL(taxModel, ref message))
                    {
                        message = message == string.Empty ? "Tax details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditTaxDetails", taxModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTaxDetails(MasterTaxViewModel taxModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditTaxDetailsBAL(taxModel, ref message))
                    {
                        message = message == string.Empty ? "Tax details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditTaxDetails", taxModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        #region Technology

        /// <summary>
        /// Display Technology Details.
        /// </summary>
        /// <returns></returns>
        public ActionResult ListTechnologyDetails()
        {
            return View("ListTechnologyDetails");
        }
        [HttpGet]
        public ActionResult SearchTechnologyDetails()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> StatusList = new List<SelectListItem>();
                StatusList.Add(new SelectListItem { Text = "All", Value = "%" });
                StatusList.Add(new SelectListItem { Text = "Active", Value = "Y", Selected = true });
                StatusList.Add(new SelectListItem { Text = "Inactive", Value = "N" });
                ViewData["Status"] = StatusList;
                return PartialView("SearchTechnologyDetails");

            }
            catch (Exception)
            {

                throw;
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
        /// Display Technology Details form for data entry.
        /// </summary>
        /// <returns></returns>
        public ActionResult AddEditTechDetails()
        {
            CommonFunctions comm = new CommonFunctions();

            MasterTechnologyViewModel TechModel = new MasterTechnologyViewModel();
            TechModel.TechType = "A";
            TechModel.ListLayers = comm.PopulateRoadExecutionItems();
            return PartialView("AddEditTechDetails", TechModel);
        }


        /// <summary>
        /// List Technology Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult TechnologyDetailsList(FormCollection formCollection)
        {
            long totalRecords = 0;
            objBAL = new MasterBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListTechnologyDetails(formCollection["Status"], Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
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
        /// Save Technology Details.
        /// </summary>
        /// <param name="techViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddTechnologyDetails(MasterTechnologyViewModel techViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddTechnologyDetails(techViewModel, ref message))
                    {
                        message = message == string.Empty ? "Technology details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditTechDetails", techViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Update technology details.
        /// </summary>
        /// <param name="techViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditTechnologyDetails(MasterTechnologyViewModel techViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditTechnologyDetails(techViewModel, ref message))
                    {
                        message = message == string.Empty ? "Technology details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditTechDetails", techViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get Technology details for editing.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult GetTechnologyDetails(string parameter, string hash, string key)
        {
            Dictionary<string, string> decryptedParameters = null;
            MasterTechnologyViewModel techViewModel = new MasterTechnologyViewModel();
            objBAL = new MasterBAL();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                techViewModel = objBAL.GetTechnologyDetails(Convert.ToInt32(decryptedParameters["TechCode"]));
                return PartialView("AddEditTechDetails", techViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Delete technology details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteTechnologyDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int technologyCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                technologyCode = Convert.ToInt32(decryptedParameters["TechCode"]);
                objBAL = new MasterBAL();
                bool status = objBAL.DeleteTechnologyDetails(technologyCode);
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

        /// <summary>
        /// Change technology details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeTechnologyStatus(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int technologyCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                technologyCode = Convert.ToInt32(decryptedParameters["TechCode"]);
                objBAL = new MasterBAL();
                bool status = objBAL.ChangeTchnologyStatus(technologyCode);
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

        #endregion Technology

        #region Test

        /// <summary>
        /// List Test details.
        /// </summary>
        /// <returns></returns>
        public ActionResult ListTestDetails()
        {
            return View("ListTestDetails");
        }

        [HttpGet]
        public ActionResult SearchTestDetails()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> StatusList = new List<SelectListItem>();
                StatusList.Add(new SelectListItem { Text = "All", Value = "%" });
                StatusList.Add(new SelectListItem { Text = "Active", Value = "Y", Selected = true });
                StatusList.Add(new SelectListItem { Text = "Inactive", Value = "N" });
                ViewData["Status"] = StatusList;
                return PartialView("SearchTestDetails");

            }
            catch (Exception)
            {

                throw;
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
        /// Display add form to add test details.
        /// </summary>
        /// <returns></returns>
        public ActionResult AddEditTestDetails()
        {
            MasterTestViewModel TestModel = new MasterTestViewModel();
            return PartialView("AddEditTestDetails", TestModel);
        }

        /// <summary>
        /// List Test Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult TestDetailsList(FormCollection formCollection)
        {
            long totalRecords = 0;
            objBAL = new MasterBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListTestDetails(formCollection["Status"], Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
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
        /// Add Test details.
        /// </summary>
        /// <param name="testViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddTestDetails(MasterTestViewModel testViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddTestDetails(testViewModel, ref message))
                    {
                        message = message == string.Empty ? "Test details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditTestDetails", testViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Update test details
        /// </summary>
        /// <param name="testViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditTestDetails(MasterTestViewModel testViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditTestDetails(testViewModel, ref message))
                    {
                        message = message == string.Empty ? "Test details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditTestDetails", testViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get test details for editing.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult GetTestDetails(string parameter, string hash, string key)
        {
            Dictionary<string, string> decryptedParameters = null;
            MasterTestViewModel testViewModel = new MasterTestViewModel();
            objBAL = new MasterBAL();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                testViewModel = objBAL.GetTestDetails(Convert.ToInt32(decryptedParameters["TestCode"]));
                return PartialView("AddEditTestDetails", testViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Delete test details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteTestDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int testCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                testCode = Convert.ToInt32(decryptedParameters["TestCode"]);
                objBAL = new MasterBAL();
                bool status = objBAL.DeleteTestDetails(testCode);
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
        /// <summary>
        /// Change test details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeTestStatus(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int testCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                testCode = Convert.ToInt32(decryptedParameters["TestCode"]);
                objBAL = new MasterBAL();
                bool status = objBAL.ChangeTestStatus(testCode);
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

        #endregion Test

        #region Alerts

        /// <summary>
        /// List Alert Details
        /// </summary>
        /// <returns></returns>
        public ActionResult AlertDetails()
        {
            return View("AlertDetails");
        }
        /// <summary>
        /// Search alerts details
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchAlertDetails()
        {
            List<SelectListItem> lstStatus = new List<SelectListItem>();
            //SelectList lst = new SelectList();
            lstStatus.Add(new SelectListItem { Text = "All", Value = "%" });
            lstStatus.Add(new SelectListItem { Text = "Active", Value = "Y" });
            lstStatus.Add(new SelectListItem { Text = "Inactive", Value = "N" });
            ViewBag.Status = new SelectList(lstStatus, "Value", "Text");

            return PartialView("SearchAlertDetails");
        }

        /// <summary>
        /// Dispay alert details form
        /// </summary>
        /// <returns></returns>
        public ActionResult AddEditAlertDetails()
        {
            AdminAlertsViewModel AlertViewModel = new AdminAlertsViewModel();
            return PartialView("AddEditAlertDetails", AlertViewModel);
        }

        /// <summary>
        /// List alert details.
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult ListAlertsDetails(FormCollection formCollection)
        {
            long totalRecords = 0;
            objBAL = new MasterBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                String status = Request.Params["Status"];

                var jsonData = new
                {
                    rows = objBAL.ListAlertsDetails(status, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
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
        /// Add alert details.
        /// </summary>
        /// <param name="AlertViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddAlertDetails(AdminAlertsViewModel AlertViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddAlertDetails(AlertViewModel, ref message))
                    {
                        message = message == string.Empty ? "Alert details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditAlertDetails", AlertViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Update alert details.
        /// </summary>
        /// <param name="AlertViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditAlertDetails(AdminAlertsViewModel AlertViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objBAL = new MasterBAL();
                    if (objBAL.EditAlertDetails(AlertViewModel, ref message))
                    {
                        message = message == string.Empty ? "Alert details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditAlertDetails", AlertViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {

            }
        }

        public ActionResult ViewAlertDetails(String parameter, String hash, String key)
        {
            Dictionary<String, String> decryptedParameters = null;
            AdminAlertsViewModel AlertViewModel = new AdminAlertsViewModel();
            objBAL = new MasterBAL();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                AlertViewModel = objBAL.ViewAlertDetails(Convert.ToInt32(decryptedParameters["AlertId"]));
                return PartialView("AddEditAlertDetails", AlertViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Delete alert details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteAlertDetails(String parameter, String hash, String key)
        {
            Dictionary<String, String> decryptedParameters = null;
            int AlertId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                AlertId = Convert.ToInt32(decryptedParameters["AlertId"]);
                objBAL = new MasterBAL();
                bool status = objBAL.DeleteAlertDetails(AlertId);
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

        /// <summary>
        /// Change alert Status
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeAlertStatus(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int AlertId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                AlertId = Convert.ToInt32(decryptedParameters["AlertId"]);
                objBAL = new MasterBAL();
                bool status = objBAL.ChangeAlertStatus(AlertId);
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

        #endregion Alerts

        #region PMGSY2

        /// <summary>
        /// List PMGSY 2 details grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ListPMGSY2Details()
        {
            return View("ListPMGSY2Details");
        }


        /// <summary>
        /// List PMGSY2 Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult PMGSY2DetailsList(FormCollection formCollection)
        {
            long totalRecords = 0;
            objBAL = new MasterBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListPMGSYIIDetails(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
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
        /// Change PMGSY II staus.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePMGSY2Status(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int stateCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                stateCode = Convert.ToInt32(decryptedParameters["StateCode"]);
                objBAL = new MasterBAL();
                bool status = objBAL.ChangePMGSY2Status(stateCode);
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

        /// <summary>
        /// Get PMGSY II Status.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPMGSY2Status(String id)
        {
            int stateCode = Convert.ToInt32(id);
            try
            {
                objBAL = new MasterBAL();
                bool status = objBAL.IsPMGSY2Active(stateCode);
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



        #endregion Test

        #region Feedback Category

        public ActionResult ListFeedbackDetails()
        {
            return View("ListFeedbackDetails");
        }

        /// <summary>
        /// Display Feedback details form
        /// </summary>
        /// <returns></returns>
        public ActionResult AddEditFeedbackDetails()
        {
            FeedbackCategoryViewModel feedbackViewModel = new FeedbackCategoryViewModel();
            return PartialView("AddEditFeedbackDetails", feedbackViewModel);
        }

        /// <summary>
        /// List Feedback Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult FeedbackDetailsList(FormCollection formCollection)
        {
            long totalRecords = 0;
            objBAL = new MasterBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListFeedbackCategoryDetails(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
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
        /// Add feedback details
        /// </summary>
        /// <param name="feedbackViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddFeedbackDetails(FeedbackCategoryViewModel feedbackViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddFeedbackDetails(feedbackViewModel, ref message))
                    {
                        message = message == string.Empty ? "Feedback details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditFeedbackDetails", feedbackViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Update feedback details.
        /// </summary>
        /// <param name="feedbackViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditFeedbackDetails(FeedbackCategoryViewModel feedbackViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditFeedbackDetails(feedbackViewModel, ref message))
                    {
                        message = message == string.Empty ? "Feedback details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditFeedbackDetails", feedbackViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Dipslay feedback details for update purpose.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult GetFeedbackDetails(string parameter, string hash, string key)
        {
            Dictionary<string, string> decryptedParameters = null;
            FeedbackCategoryViewModel feedbackViewModel = new FeedbackCategoryViewModel();
            objBAL = new MasterBAL();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                feedbackViewModel = objBAL.GetFeedbackDetails(Convert.ToInt32(decryptedParameters["FeedbackId"]));
                return PartialView("AddEditFeedbackDetails", feedbackViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Delete feedback details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteFeedbackDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int feedbackId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                feedbackId = Convert.ToInt32(decryptedParameters["FeedbackId"]);
                objBAL = new MasterBAL();
                bool status = objBAL.DeleteFeedbackDetails(feedbackId);
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
        #endregion Test

        #region Carriage

        public ActionResult ListCarriageDetails()
        {
            return View("ListCarriageDetails");
        }

        [HttpGet]
        public ActionResult SearchCarriageDetail()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> StatusList = new List<SelectListItem>();
                StatusList.Add(new SelectListItem { Text = "All", Value = "%" });
                StatusList.Add(new SelectListItem { Text = "Active", Value = "Y", Selected = true });
                StatusList.Add(new SelectListItem { Text = "Inactive", Value = "N" });
                ViewData["Status"] = StatusList;
                return PartialView("SearchCarriageDetail");

            }
            catch (Exception)
            {

                throw;
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
        /// Display carriage details form
        /// </summary>
        /// <returns></returns>
        public ActionResult AddEditCarriageDetails()
        {
            MasterCarriageViewModel CarriageViewModel = new MasterCarriageViewModel();
            return PartialView("AddEditCarriageDetails", CarriageViewModel);
        }

        /// <summary>
        /// List Carriage Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult CarriageDetailsList(FormCollection formCollection)
        {
            long totalRecords = 0;
            objBAL = new MasterBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListCarriageDetails(formCollection["Status"], Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
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
        /// Add carriage details.
        /// </summary>
        /// <param name="carriageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddCarriageDetails(MasterCarriageViewModel carriageViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddCarriageDetails(carriageViewModel, ref message))
                    {
                        message = message == string.Empty ? "Carriage details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditCarriageDetails", carriageViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Update carriage details.
        /// </summary>
        /// <param name="carriageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCarriageDetails(MasterCarriageViewModel carriageViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditCarriageDetails(carriageViewModel, ref message))
                    {
                        message = message == string.Empty ? "Carriage details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditCarriageDetails", carriageViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get carriage details for update purpose
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult GetCarriageDetails(string parameter, string hash, string key)
        {
            Dictionary<string, string> decryptedParameters = null;
            MasterCarriageViewModel carriageViewModel = new MasterCarriageViewModel();
            objBAL = new MasterBAL();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                carriageViewModel = objBAL.GetCarriageDetails(Convert.ToInt32(decryptedParameters["CarriageCode"]));
                return PartialView("AddEditCarriageDetails", carriageViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Delete carriage details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteCarriageDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int carriageCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                carriageCode = Convert.ToInt32(decryptedParameters["CarriageCode"]);
                objBAL = new MasterBAL();
                bool status = objBAL.DeleteCarriageDetails(carriageCode);
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

        /// <summary>
        /// Change carriage status.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeCarriageStatus(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int CarriageCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                CarriageCode = Convert.ToInt32(decryptedParameters["CarriageCode"]);
                objBAL = new MasterBAL();
                bool status = objBAL.ChangeCarriageStatus(CarriageCode);
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

        #endregion Carriage

        #region Info

        /// <summary>
        /// List Information Details
        /// </summary>
        /// <returns></returns>
        public ActionResult ListInfoDetails(string id)
        {
            ViewBag.Type = id;
            return View("ListInfoDetails");
        }

        /// <summary>
        /// Dispaly search Information view.
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchInfoDetails()
        {
            return PartialView("SearchInfoDetails", new MasterInfoViewModel());
        }

        /// <summary>
        /// return Json Data to display information on grid
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult InfoDetailsList(FormCollection formCollection)
        {
            long totalRecords = 0;
            objBAL = new MasterBAL();
            string infoType = Request.Params["Type"];
            int infoStateCode = Convert.ToInt32(Request.Params["StateCode"]);

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListInfoDetails(infoType, infoStateCode, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
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
        /// Show Information Details Form
        /// </summary>
        /// <returns></returns>
        public ActionResult AddEditInfoDetails(string id)
        {
            //String[] Types = new String[] {"M","N","T","A"};           

            MasterInfoViewModel infoViewModel = new MasterInfoViewModel();
            infoViewModel.MAST_INFO_TYPE = id;
            infoViewModel.MAST_SORT_ORDER = 0;
            return PartialView("AddEditInfoDetails", infoViewModel);
        }

        /// <summary>
        /// Save Information Details
        /// </summary>
        /// <param name="infoViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddInfoDetails(MasterInfoViewModel infoViewModel)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddInfoDetails(infoViewModel, ref message))
                    {
                        message = message == string.Empty ? "Information details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditInfoDetails", infoViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Update Information Details
        /// </summary>
        /// <param name="infoViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditInfoDetails(MasterInfoViewModel infoViewModel)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditInfoDetails(infoViewModel, ref message))
                    {
                        message = message == string.Empty ? "Information details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditInfoDetails", infoViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Get Information Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult GetInfoDetails(string parameter, string hash, string key)
        {
            Dictionary<string, string> decryptedParameters = null;
            MasterInfoViewModel infoViewModel = new MasterInfoViewModel();
            objBAL = new MasterBAL();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                infoViewModel = objBAL.GetInfoDetails(Convert.ToInt32(decryptedParameters["InfoCode"]));
                return PartialView("AddEditInfoDetails", infoViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occured while processing your reqeust." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete Information Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteInfoDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int InfoCode = 0;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                InfoCode = Convert.ToInt32(decryptedParameters["InfoCode"]);
                objBAL = new MasterBAL();
                if (objBAL.DeleteInfoDetails(InfoCode))
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

        /// <summary>
        /// Change Information Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeInfoStatus(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int InfoCode = 0;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                InfoCode = Convert.ToInt32(decryptedParameters["InfoCode"]);
                objBAL = new MasterBAL();
                if (objBAL.ChangeInfoStatus(InfoCode))
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
        #endregion Info

        #region Contractor Detail
        [HttpGet]
        public ActionResult ListContractorPanSearchDetail()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetContractorByPanSearchList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int stateCode = 0;
            int districtCode = 0;
            string contractorName = string.Empty;
            string status = string.Empty;
            string panno = string.Empty;
            string contrSuppType = string.Empty;
            objBAL = new MasterBAL();
            try
            {


                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                panno = Request.Params["PAN"];

                var jsonData = new
                {
                    rows = objBAL.GetContractorList(stateCode.ToString(), districtCode.ToString(), contractorName, status, contrSuppType, panno, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, Request.Params["filters"]),
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

        /// <summary>
        /// Method to set existing record into the View form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewContractorByPanSearch(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MasterContractorViewModel model = objBAL.EditContractor(Convert.ToInt32(decryptParameters["ContractorCode"]));
                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Contractor/Supplier details not exist.");
                        return View("ViewContractorByPanSearch", new MasterContractorViewModel());
                    }
                    ViewBag.State = model.States;
                    ViewBag.District = model.Districts;
                    return View("ViewContractorByPanSearch", model);
                }
                return View("ViewContractorByPanSearch", new MasterContractorViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Contractor/Supplier details  not exist.");
                return View("ViewContractorByPanSearch", new MasterContractorViewModel());
            }
        }

        public ActionResult GetContractorByIdPanSearchBankDetails(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            MasterDAL masterDAL = new MasterDAL();
            int ContractorCode = 0;
            int regState = 0;
            try
            {
                //Added By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Added By Abhishek kamble 29-Apr-2014 end

                objBAL = new MasterBAL();
                encryptedParameters = Request.Params["ContractorCode"].Split('/');
                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    ContractorCode = Convert.ToInt32(decryptedParameters["ContractorCode"].ToString());
                    //regState = Convert.ToInt32(decryptedParameters["RegState"].ToString());
                    regState = 0;
                }

                var jsonData = new
                {
                    //rows = objBAL.ListContractorBankDetails(ContractorCode, regState, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    rows = masterDAL.ListContractorBankDetailsView(ContractorCode, regState, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        public ActionResult GetContractorByIdPanSearchRegList(int? page, int? rows, string sidx, string sord)
        {
            objBAL = new MasterBAL();
            try
            {
                int stateCode = 0;
                int districtCode = 0;
                string status = string.Empty;
                string contractorName = string.Empty;
                string conStatus = string.Empty;
                string panNumber = string.Empty;
                int classType = 0;
                string regNo = string.Empty;
                string companyName = string.Empty;
                int ContractorCode = 0;
                long totalRecords;


                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                encryptedParameters = Request.Params["ContractorCode"].Split('/');
                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    ContractorCode = Convert.ToInt32(decryptedParameters["ContractorCode"].ToString());
                }
                var jsonData = new
                {
                    rows = objBAL.ListMasterContractorReg(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, stateCode, districtCode, status, contractorName, conStatus, panNumber, classType, regNo, companyName, ContractorCode),
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

        public ActionResult GetContractorByIdPanSearchAgreementList(int? page, int? rows, string sidx, string sord)
        {
            objBAL = new MasterBAL();
            try
            {
                int stateCode = 0;
                int districtCode = 0;
                string status = string.Empty;
                string contractorName = string.Empty;
                string conStatus = string.Empty;
                string panNumber = string.Empty;
                int classType = 0;
                string regNo = string.Empty;
                string companyName = string.Empty;
                int ContractorCode = 0;
                long totalRecords;


                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                encryptedParameters = Request.Params["ContractorCode"].Split('/');
                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    ContractorCode = Convert.ToInt32(decryptedParameters["ContractorCode"].ToString());
                }
                var jsonData = new
                {
                    rows = objBAL.GetContractorAgreementListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, stateCode, districtCode, status, contractorName, conStatus, panNumber, classType, regNo, companyName, ContractorCode),
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

        public ActionResult GetContractorByIdPanSearchIMSMaintenanceList(int? page, int? rows, string sidx, string sord)
        {
            objBAL = new MasterBAL();
            try
            {
                int stateCode = 0;
                int districtCode = 0;
                string status = string.Empty;
                string contractorName = string.Empty;
                string conStatus = string.Empty;
                string panNumber = string.Empty;
                int classType = 0;
                string regNo = string.Empty;
                string companyName = string.Empty;
                int ContractorCode = 0;
                long totalRecords;


                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                encryptedParameters = Request.Params["ContractorCode"].Split('/');
                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    ContractorCode = Convert.ToInt32(decryptedParameters["ContractorCode"].ToString());
                }
                var jsonData = new
                {
                    rows = objBAL.GetContractorIMSMaintenanceListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, stateCode, districtCode, status, contractorName, conStatus, panNumber, classType, regNo, companyName, ContractorCode),
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

        public ActionResult GetContractorByIdPanSearchPaymentList(int? page, int? rows, string sidx, string sord)
        {
            objBAL = new MasterBAL();
            try
            {
                int stateCode = 0;
                int districtCode = 0;
                string status = string.Empty;
                string contractorName = string.Empty;
                string conStatus = string.Empty;
                string panNumber = string.Empty;
                int classType = 0;
                string regNo = string.Empty;
                string companyName = string.Empty;
                int ContractorCode = 0;
                long totalRecords;


                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                encryptedParameters = Request.Params["ContractorCode"].Split('/');
                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    ContractorCode = Convert.ToInt32(decryptedParameters["ContractorCode"].ToString());
                }
                var jsonData = new
                {
                    rows = objBAL.GetContractorPaymentListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, stateCode, districtCode, status, contractorName, conStatus, panNumber, classType, regNo, companyName, ContractorCode),
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

        #endregion

        #region Connectivity Status
        public ActionResult Form3ConnectivityStatusLayout()
        {
            PMGSYEntities dbContext;
            CommonFunctions comm = new CommonFunctions();
            ConnectivityStatusViewModel conn = new ConnectivityStatusViewModel();

            try
            {
                if (PMGSYSession.Current.StateCode == 0)
                {
                    conn.StateList = comm.PopulateStates(true);

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });
                    conn.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();
                }
                else
                {
                    conn.StateCode = PMGSYSession.Current.StateCode;
                    conn.StateName = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts = comm.PopulateDistrict(conn.StateCode, false);
                    conn.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
            }
            finally
            {

            }
            return View(conn);
        }

        public ActionResult Form3ConnectivityStatus(ConnectivityStatusViewModel conn)
        {
            PMGSYEntities dbContext;
            CommonFunctions comm = new CommonFunctions();

            try
            {
                dbContext = new PMGSYEntities();
                //if (ModelState.IsValid)
                //{
                var s = dbContext.IMS_CONNECTIVITY.Where(a => a.MAST_STATE_CODE == conn.StateCode && a.MAST_DISTRICT_CODE == conn.DistrictCode);
                if (s.Count() > 0)
                {
                    conn.hdnflag = "N";
                    conn.flag = "N";
                    foreach (var item in s)
                    {
                        conn.t11 = Convert.ToString(item.IMS_TNH_1000).Trim();
                        conn.t12 = Convert.ToString(item.IMS_TNH_500).Trim();

                        conn.t1El499 = Convert.ToString(item.IMS_TNH_EL_499).Trim();
                        conn.t1El249 = Convert.ToString(item.IMS_TNH_EL_249).Trim();

                        conn.t13 = Convert.ToString(item.IMS_TNH_250).Trim();
                        conn.t14 = Convert.ToString(item.IMS_TNH).Trim();
                        conn.t1Tot = Convert.ToString(item.IMS_TNH_1000 + item.IMS_TNH_500 + item.IMS_TNH_250 + item.IMS_TNH).Trim();

                        conn.t21 = Convert.ToString(item.IMS_TNCH_1000).Trim();
                        conn.t22 = Convert.ToString(item.IMS_TNCH_500).Trim();

                        conn.t2El499 = Convert.ToString(item.IMS_TNCH_EL_499).Trim();
                        conn.t2El249 = Convert.ToString(item.IMS_TNCH_EL_249).Trim();

                        conn.t23 = Convert.ToString(item.IMS_TNCH_250).Trim();
                        conn.t24 = Convert.ToString(item.IMS_TNCH).Trim();
                        conn.t2Tot = Convert.ToString(item.IMS_TNCH_1000 + item.IMS_TNCH_500 + item.IMS_TNCH_250 + item.IMS_TNCH).Trim();

                        conn.t31 = Convert.ToString(item.IMS_TNH_1000 - item.IMS_TNCH_1000).Trim();
                        conn.t32 = Convert.ToString(item.IMS_TNH_500 - item.IMS_TNCH_500).Trim();

                        conn.t3El499 = Convert.ToString(item.IMS_TNH_EL_499 - item.IMS_TNCH_EL_499).Trim();
                        conn.t3El249 = Convert.ToString(item.IMS_TNH_EL_249 - item.IMS_TNCH_EL_249).Trim();

                        conn.t33 = Convert.ToString(item.IMS_TNH_250 - item.IMS_TNCH_250).Trim();
                        conn.t34 = Convert.ToString(item.IMS_TNH - item.IMS_TNCH).Trim();
                        conn.t3Tot = Convert.ToString(Convert.ToInt32(conn.t31) + Convert.ToInt32(conn.t32) + Convert.ToInt32(conn.t33) + Convert.ToInt32(conn.t34)).Trim();

                        conn.t41 = Convert.ToString(item.IMS_H2000_1000).Trim();
                        conn.t42 = Convert.ToString(item.IMS_H2000_500).Trim();

                        conn.t4El499 = Convert.ToString(item.IMS_H2000_EL_499).Trim();
                        conn.t4El249 = Convert.ToString(item.IMS_H2000_EL_249).Trim();

                        conn.t43 = Convert.ToString(item.IMS_H2000_250).Trim();
                        conn.t44 = Convert.ToString(item.IMS_H2000).Trim();
                        conn.t4Tot = Convert.ToString(item.IMS_H2000_1000 + item.IMS_H2000_500 + item.IMS_H2000_250 + item.IMS_H2000).Trim();

                        conn.t51 = Convert.ToString(item.IMS_H2001_1000).Trim();
                        conn.t52 = Convert.ToString(item.IMS_H2001_500).Trim();

                        conn.t5El499 = Convert.ToString(item.IMS_H2001_EL_499).Trim();
                        conn.t5El249 = Convert.ToString(item.IMS_H2001_EL_249).Trim();

                        conn.t53 = Convert.ToString(item.IMS_H2001_250).Trim();
                        conn.t54 = Convert.ToString(item.IMS_H2001).Trim();
                        conn.t5Tot = Convert.ToString(item.IMS_H2001_1000 + item.IMS_H2001_500 + item.IMS_H2001_250 + item.IMS_H2001).Trim();


                    }
                }
                else
                {
                    conn.flag = "N";
                    conn.hdnflag = "Y";
                }
                //conn.hdnflag = conn.flag;
                conn.hdnStateCode = conn.StateCode;
                conn.hdnDistCode = conn.DistrictCode;
                return View("Form3ConnectivityStatus", conn);
                //}
                //else
                //{
                //    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                //    return Json(new { message = message.Trim() });
                //}
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        [HttpPost]
        public JsonResult PopulateDistricts(string param)
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                //int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                int stateCode = Convert.ToInt32(param.Trim());

                List<SelectListItem> lstDist = new List<SelectListItem>();
                lstDist = objCommonFunctions.PopulateDistrict(stateCode, false);
                //lstDist.RemoveAt(0);
                //lstDist.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
                //lstDist.Sort();
                return Json(lstDist);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { string.Empty });
            }
        }

        [HttpPost]
        public JsonResult SaveConnectivityStatus(ConnectivityStatusViewModel conn)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                CommonFunctions objCommonFunctions = new CommonFunctions();

                MasterDAL master = new MasterDAL();

                bool flag = master.AddEditConnectivityStatus(conn);

                return Json(new { status = flag });
                //}
                //else
                //{
                //    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                //    return Json(new { message = message.Trim() });
                //}
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { string.Empty });
            }
        }

        [HttpPost]
        public JsonResult DeleteConnectivityStatus(ConnectivityStatusViewModel conn)
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();

                MasterDAL master = new MasterDAL();

                bool flag = master.DelConnectivityStatus(conn);

                return Json(new { status = flag });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { string.Empty });
            }
        }



        #endregion

        #region Cluster Master
        [HttpGet]
        public ActionResult ListCluster()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchCluster()
        {
            MasterClusterSearchModel model = new MasterClusterSearchModel();

            return PartialView("SearchCluster", model);
        }


        [HttpPost]
        public ActionResult GetClusterList(FormCollection formCollection)
        {
            //int? page, int? rows, string sidx, string sord
            String searchParameters = String.Empty;
            long totalRecords;
            string activeStatus = "Y";
            // int districtCode = 0;
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int page = Convert.ToInt32(formCollection["page"]);
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            if (!string.IsNullOrEmpty(formCollection["Status"]))
            {
                activeStatus = formCollection["Status"];
            }


            Dictionary<string, string> parameters = new Dictionary<string, string>();

            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {
                rows = objBAL.ListClusterBAL(stateCode, districtCode, blockCode, activeStatus, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Method to load add view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditCluster()
        {
            objBAL = new MasterBAL();
            MasterClusterViewModel model = new MasterClusterViewModel();

            return PartialView("AddEditCluster", model);
        }

        [Audit]
        public ActionResult AddClusterHabitation(string HabCode)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            string encryptedHabCode = string.Empty;
            string habStatus = string.Empty;
            bool status = false;
            string habiatationName = string.Empty;
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            objBAL = new MasterBAL();
            try
            {
                stateCode = Convert.ToInt32(Request.Params["StateCode"]);
                districtCode = Convert.ToInt32(Request.Params["DistrictCode"]);
                blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
                habiatationName = Request.Params["HabName"];
                if (objBAL.AddClusterHabitationBAL(HabCode, habiatationName, blockCode))
                {
                    message = "Cluster details saved successfully.";
                    status = true;
                }
                else
                {
                    message = "Cluster details not saved.";
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Error occured while processing,Cluster details  Added.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetClusterHabiationList(FormCollection formCollection)
        {
            //int? page, int? rows, string sidx, string sord
            String searchParameters = String.Empty;
            long totalRecords;
            // int districtCode = 0;
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int page = Convert.ToInt32(formCollection["page"]);
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];



            Dictionary<string, string> parameters = new Dictionary<string, string>();

            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }


            var jsonData = new
            {
                rows = objBAL.GetHabitationListClusterBAL(stateCode, districtCode, blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Method to Get Cluster Name Edit details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditCluster(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                MasterDAL masterDataEntryDAL = new MasterDAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MasterClusterEditViewModel model = objBAL.GetClusterDetailsBAL(Convert.ToInt32(decryptParameters["ClusterCode"]));
                    return PartialView("EditCluster", model);
                }

                return PartialView("EditCluster", new MasterClusterEditViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Cluster details not exist.");
                return PartialView("EditCluster", new MasterClusterEditViewModel());
            }
        }

        /// <summary>
        /// Method to update Cluster Name details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCluster(MasterClusterEditViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditClusterNameHabiationBAL(model, ref message))
                    {
                        message = message == string.Empty ? "Cluster details updated successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "Cluster details not updated." : message;
                    }
                }
                else
                {
                    //model.MAST_PARENT_ND_CODE_List = new List<SelectListItem>();
                    // model.MAST_PARENT_ND_CODE_List.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));

                    // return PartialView("AddEditPIUDepartment", model);               
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Cluster details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete Cluster  details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteCluster(String parameter, String hash, String key)
        {
            objBAL = new MasterBAL();
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteClusterBAL(Convert.ToInt32(decryptedParameters["ClusterCode"].ToString()), ref message))
                    {
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Cluster details." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this Cluster details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Cluster details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Method to Get Cluster Name Edit details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewClusterHabitation(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                MasterDAL masterDataEntryDAL = new MasterDAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MasterClusterViewEditHabiationModel model = objBAL.GetClusterHabsDetailsBAL(Convert.ToInt32(decryptParameters["ClusterCode"]));
                    return PartialView("ViewClusterHabitation", model);
                }

                return PartialView("ViewClusterHabitation", new MasterClusterViewEditHabiationModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Cluster details not exist.");
                return PartialView("ViewClusterHabitation", new MasterClusterViewEditHabiationModel());
            }
        }

        [HttpPost]
        public ActionResult GetViewClusterHabitatonList()
        {
            //int? page, int? rows, string sidx, string sord
            String searchParameters = String.Empty;
            long totalRecords;
            // int districtCode = 0;
            int stateCode = Convert.ToInt32(Request.Params["StateCode"]);
            int districtCode = Convert.ToInt32(Request.Params["DistrictCode"]);
            int blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
            int page = Convert.ToInt32(Request.Params["page"]);
            int rows = Convert.ToInt32(Request.Params["rows"]);
            string sidx = Request.Params["sidx"];
            string sord = Request.Params["sord"];


            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            encryptedParameters = Request.Params["EncryptedClusterCode"].Split('/');

            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

            int ClusterCode = Convert.ToInt32(decryptedParameters["ClusterCode"].ToString());


            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }


            var jsonData = new
            {
                rows = objBAL.GetClusterHabitationListByClusterCodeBAL(ClusterCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult DeleteClusterHabitation(String parameter, String hash, String key)
        {
            objBAL = new MasterBAL();
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteClusterHabitationBAL(Convert.ToInt32(decryptedParameters["ClusterCode"].ToString()), Convert.ToInt32(decryptedParameters["HabCode"].ToString()), ref message))
                    {
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Cluster Habitation details." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this Cluster Habitation details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Cluster Habitation details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult GetViewAddHabitationListIntoCluster()
        {
            //int? page, int? rows, string sidx, string sord
            String searchParameters = String.Empty;
            long totalRecords;
            // int districtCode = 0;
            int stateCode = Convert.ToInt32(Request.Params["StateCode"]);
            int districtCode = Convert.ToInt32(Request.Params["DistrictCode"]);
            int blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
            int page = Convert.ToInt32(Request.Params["page"]);
            int rows = Convert.ToInt32(Request.Params["rows"]);
            string sidx = Request.Params["sidx"];
            string sord = Request.Params["sord"];


            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            encryptedParameters = Request.Params["EncryptedClusterCode"].Split('/');

            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

            int ClusterCode = Convert.ToInt32(decryptedParameters["ClusterCode"].ToString());


            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }


            var jsonData = new
            {
                rows = objBAL.GetAddHabitationListIntoClusterBAL(ClusterCode, stateCode, districtCode, blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Audit]
        public ActionResult UpdateClusterHabitation(string HabCode)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            string encryptedHabCode = string.Empty;
            string habStatus = string.Empty;
            bool status = false;
            string clusterName = string.Empty;
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            int clusterCode = 0;
            objBAL = new MasterBAL();
            try
            {
                stateCode = Convert.ToInt32(Request.Params["StateCode"]);
                districtCode = Convert.ToInt32(Request.Params["DistrictCode"]);
                blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
                blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
                string[] encryptedParameters = null;
                Dictionary<string, string> decryptedParameters = null;
                encryptedParameters = Request.Params["EncryptedClusterCode"].Split('/');

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                clusterCode = Convert.ToInt32(decryptedParameters["ClusterCode"].ToString());

                clusterName = Request.Params["ClusterName"];
                if (objBAL.UpdateClusterHabitationBAL(HabCode, clusterName, clusterCode))
                {
                    message = "Cluster details Habitation Updated successfully.";
                    status = true;
                }
                else
                {
                    message = "Cluster details  Habitation not Updated .";
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Error occured while processing,Cluster details  Added.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Method to update Cluster Name details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizeClusterHabitation(MasterClusterViewEditHabiationModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.FinalizeClusterHabitationBAL(model, ref message))
                    {
                        message = message == string.Empty ? "Cluster finalize successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "Cluster details not finalize." : message;
                    }
                }
                else
                {
                    //model.MAST_PARENT_ND_CODE_List = new List<SelectListItem>();
                    // model.MAST_PARENT_ND_CODE_List.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));

                    // return PartialView("AddEditPIUDepartment", model);               
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Cluster details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region  Cluster Core Network Master
        [HttpGet]
        public ActionResult ListClusterCN()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchClusterCN()
        {
            MasterClusterSearchModel model = new MasterClusterSearchModel();

            return PartialView("SearchClusterCN", model);
        }


        [HttpPost]
        public ActionResult GetClusterCNList(FormCollection formCollection)
        {
            //int? page, int? rows, string sidx, string sord
            String searchParameters = String.Empty;
            long totalRecords;
            string activeStatus = "Y";
            // int districtCode = 0;
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int page = Convert.ToInt32(formCollection["page"]);
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            if (!string.IsNullOrEmpty(formCollection["Status"]))
            {
                activeStatus = formCollection["Status"];
            }


            Dictionary<string, string> parameters = new Dictionary<string, string>();

            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {
                rows = objBAL.ListClusterCNBAL(stateCode, districtCode, blockCode, activeStatus, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Method to load add view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditClusterCN()
        {
            objBAL = new MasterBAL();
            MasterClusterViewModel model = new MasterClusterViewModel();
            return PartialView("AddEditClusterCN", model);
        }


        [Audit]
        public ActionResult AddClusterCNHabitation(string HabCode)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            string encryptedHabCode = string.Empty;
            string habStatus = string.Empty;
            bool status = false;
            string habiatationName = string.Empty;
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            objBAL = new MasterBAL();
            try
            {
                stateCode = Convert.ToInt32(Request.Params["StateCode"]);
                districtCode = Convert.ToInt32(Request.Params["DistrictCode"]);
                blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
                habiatationName = Request.Params["HabName"];
                if (objBAL.AddClusterHabitationBAL(HabCode, habiatationName, blockCode))
                {
                    message = "Cluster details saved successfully.";
                    status = true;
                }
                else
                {
                    message = "Cluster details not saved.";
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Error occured while processing,Cluster details  Added.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GetCoreNetworkClusterList(FormCollection formCollection)
        {
            //int? page, int? rows, string sidx, string sord
            String searchParameters = String.Empty;
            long totalRecords;
            // int districtCode = 0;
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int page = Convert.ToInt32(formCollection["page"]);
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }


            var jsonData = new
            {
                rows = objBAL.GetCoreNetworkListClusterCNBAL(stateCode, districtCode, blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetClusterCNHabiationList(String parameter, String hash, String key)
        {
            //int? page, int? rows, string sidx, string sord
            String searchParameters = String.Empty;
            long totalRecords;
            int blockCode = 0;
            int roadCode = 0;
            int page = Convert.ToInt32(Request.Params["page"]);
            int rows = Convert.ToInt32(Request.Params["rows"]);
            string sidx = Request.Params["sidx"];
            string sord = Request.Params["sord"];
            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            if (decryptedParameters.Count() > 0)
            {

                roadCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                objBAL = new MasterBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                var jsonData = new
                {
                    rows = objBAL.GetHabitationListClusterCNBAL(roadCode, blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Method to Get Cluster Name Edit details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditClusterCN(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                MasterDAL masterDataEntryDAL = new MasterDAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MasterClusterEditViewModel model = objBAL.GetClusterCNDetailsBAL(Convert.ToInt32(decryptParameters["ClusterCode"]));
                    return PartialView("EditClusterCN", model);
                }

                return PartialView("EditClusterCN", new MasterClusterEditViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Cluster details not exist.");
                return PartialView("EditClusterCN", new MasterClusterEditViewModel());
            }
        }

        /// <summary>
        /// Method to update Cluster Name details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClusterCN(MasterClusterEditViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditClusterCNNameHabiationBAL(model, ref message))
                    {
                        message = message == string.Empty ? "Cluster details updated successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "Cluster details not updated." : message;
                    }
                }
                else
                {
                    //model.MAST_PARENT_ND_CODE_List = new List<SelectListItem>();
                    // model.MAST_PARENT_ND_CODE_List.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));

                    // return PartialView("AddEditPIUDepartment", model);               
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Cluster details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete Cluster  details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteClusterCN(String parameter, String hash, String key)
        {
            objBAL = new MasterBAL();
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteClusterCNBAL(Convert.ToInt32(decryptedParameters["ClusterCode"].ToString()), ref message))
                    {
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Cluster details." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this Cluster details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Cluster details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Method to Get Cluster Name Edit details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewClusterCNHabitation(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                MasterDAL masterDataEntryDAL = new MasterDAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MasterClusterViewEditHabiationModel model = objBAL.GetClusterHabsDetailsBAL(Convert.ToInt32(decryptParameters["ClusterCode"]));
                    return PartialView("ViewClusterCNHabitation", model);
                }

                return PartialView("ViewClusterCNHabitation", new MasterClusterViewEditHabiationModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Cluster details not exist.");
                return PartialView("ViewClusterCNHabitation", new MasterClusterViewEditHabiationModel());
            }
        }

        [HttpPost]
        public ActionResult GetViewClusterCNHabitatonList()
        {
            //int? page, int? rows, string sidx, string sord
            String searchParameters = String.Empty;
            long totalRecords;
            // int districtCode = 0;
            int stateCode = Convert.ToInt32(Request.Params["StateCode"]);
            int districtCode = Convert.ToInt32(Request.Params["DistrictCode"]);
            int blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
            int page = Convert.ToInt32(Request.Params["page"]);
            int rows = Convert.ToInt32(Request.Params["rows"]);
            string sidx = Request.Params["sidx"];
            string sord = Request.Params["sord"];


            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            encryptedParameters = Request.Params["EncryptedClusterCode"].Split('/');

            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

            int ClusterCode = Convert.ToInt32(decryptedParameters["ClusterCode"].ToString());


            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }


            var jsonData = new
            {
                rows = objBAL.GetClusterCNHabitationListByClusterCodeBAL(ClusterCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult DeleteClusterCNHabitation(String parameter, String hash, String key)
        {
            objBAL = new MasterBAL();
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteClusterCNHabitationBAL(Convert.ToInt32(decryptedParameters["ClusterCode"].ToString()), Convert.ToInt32(decryptedParameters["HabCode"].ToString()), ref message))
                    {
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Cluster Habitation details." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this Cluster Habitation details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Cluster Habitation details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult GetViewAddCoreNetworkListByClusterCodeCN()
        {
            //int? page, int? rows, string sidx, string sord
            String searchParameters = String.Empty;
            long totalRecords;
            // int districtCode = 0;
            int stateCode = Convert.ToInt32(Request.Params["StateCode"]);
            int districtCode = Convert.ToInt32(Request.Params["DistrictCode"]);
            int blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
            int page = Convert.ToInt32(Request.Params["page"]);
            int rows = Convert.ToInt32(Request.Params["rows"]);
            string sidx = Request.Params["sidx"];
            string sord = Request.Params["sord"];


            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            encryptedParameters = Request.Params["EncryptedClusterCode"].Split('/');

            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

            int ClusterCode = Convert.ToInt32(decryptedParameters["ClusterCode"].ToString());


            objBAL = new MasterBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }


            var jsonData = new
            {
                rows = objBAL.GetAddCoreNetworkListByClusterCodeBAL(ClusterCode, stateCode, districtCode, blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetViewAddHabitationListIntoClusterCN(String parameter, String hash, String key)
        {
            //int? page, int? rows, string sidx, string sord
            String searchParameters = String.Empty;
            long totalRecords;
            // int districtCode = 0;
            int stateCode = Convert.ToInt32(Request.Params["StateCode"]);
            int districtCode = Convert.ToInt32(Request.Params["DistrictCode"]);
            int blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
            int page = Convert.ToInt32(Request.Params["page"]);
            int rows = Convert.ToInt32(Request.Params["rows"]);
            string sidx = Request.Params["sidx"];
            string sord = Request.Params["sord"];


            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            if (decryptedParameters.Count() > 0)
            {

                int clusterCode = Convert.ToInt32(decryptedParameters["ClusterCode"].ToString());
                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());


                objBAL = new MasterBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                var jsonData = new
                {
                    rows = objBAL.GetAddHabitationListIntoClusterCNBAL(clusterCode, roadCode, stateCode, districtCode, blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }

        [Audit]
        public ActionResult UpdateClusterCNHabitation(string HabCode)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            string encryptedHabCode = string.Empty;
            string habStatus = string.Empty;
            bool status = false;
            string clusterName = string.Empty;
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            int clusterCode = 0;
            objBAL = new MasterBAL();
            try
            {
                stateCode = Convert.ToInt32(Request.Params["StateCode"]);
                districtCode = Convert.ToInt32(Request.Params["DistrictCode"]);
                blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
                blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
                string[] encryptedParameters = null;
                Dictionary<string, string> decryptedParameters = null;
                encryptedParameters = Request.Params["EncryptedClusterCode"].Split('/');

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                clusterCode = Convert.ToInt32(decryptedParameters["ClusterCode"].ToString());

                clusterName = Request.Params["ClusterName"];
                if (objBAL.UpdateClusterCNHabitationBAL(HabCode, clusterName, clusterCode))
                {
                    message = "Cluster details Habitation Updated successfully.";
                    status = true;
                }
                else
                {
                    message = "Cluster details  Habitation not Updated .";
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Error occured while processing,Cluster details  Added.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Method to update Cluster Name details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizeClusterCNHabitation(MasterClusterViewEditHabiationModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.FinalizeClusterCNHabitationBAL(model, ref message))
                    {
                        message = message == string.Empty ? "Cluster finalize successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "Cluster details not finalize." : message;
                    }
                }
                else
                {
                    //model.MAST_PARENT_ND_CODE_List = new List<SelectListItem>();
                    // model.MAST_PARENT_ND_CODE_List.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));

                    // return PartialView("AddEditPIUDepartment", model);               
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Cluster details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region IMS EC Check List
        [HttpGet]
        public ActionResult ListImsEcCheck()
        {
            ViewData["IMS_EC_TYPE_HD"] = "F";
            return View();
        }
        [HttpGet]
        public ActionResult ListImsPreEcCheck()
        {
            ViewData["IMS_EC_TYPE_HD"] = "P";
            return View("ListImsEcCheck");
        }
        [HttpGet]
        public ActionResult SearchImsEcCheck()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                objDAL = new MasterDAL();
                MasterDAL masterDataEntryDAL = new MasterDAL();

                IMSECCheckListSearchViewModel model = new IMSECCheckListSearchViewModel();
                var query = (from ma in dbContext.MASTER_AGENCY
                             join md in dbContext.ADMIN_DEPARTMENT on ma.MAST_AGENCY_CODE equals md.MAST_AGENCY_CODE
                             where md.MAST_STATE_CODE == PMGSYSession.Current.StateCode &&
                             md.MAST_ND_TYPE == "S" &&
                             md.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode
                             select new
                             {
                                 ma.MAST_AGENCY_CODE
                             }).FirstOrDefault();
                model.Mast_Agency = query == null ? 0 : query.MAST_AGENCY_CODE;
                ViewData["Agency"] = model.Mast_Agency;
                return PartialView("SearchImsEcCheck", model);

            }
            catch (Exception)
            {

                throw;
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
        public ActionResult GetImsEcCheckList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int stateCode = 0;
            // int districtCode = 0;
            int agencyCode = 0;
            int year = 0;
            int batch = 0;
            string typeEc = string.Empty;

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            objBAL = new MasterBAL();
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
            if (!string.IsNullOrEmpty(Request.Params["EcType"]))
            {
                typeEc = Request.Params["EcType"];
            }
            if (!string.IsNullOrEmpty(Request.Params["agency"]))
            {
                agencyCode = Convert.ToInt32(Request.Params["agency"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["year"]))
            {
                year = Convert.ToInt32(Request.Params["year"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["batch"]))
            {
                batch = Convert.ToInt32(Request.Params["batch"]);
            }
            var jsonData = new
            {
                rows = objBAL.ListImsEcCheckListBAL(stateCode, year, batch, agencyCode, typeEc, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Method to load add view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditImsEcCheck()
        {
            objBAL = new MasterBAL();
            IMSECCheckListViewModel model = new IMSECCheckListViewModel();
            return PartialView("AddEditImsEcCheck", model);
        }

        /// <summary>
        /// method to save admin department details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditImsEcCheck(IMSECCheckListViewModel model)
        {
            bool status = false;
            try
            {

                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddImsEcCheckListBAL(model, ref message))
                    {
                        message = message == string.Empty ? "check list details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "check list details not saved." : message;
                    }
                }
                else
                {

                    return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "check list details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to Get PIU department details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditImsEcCheck(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    IMSECCheckListViewModel model = objBAL.GetImsEcCheckDetailsBAL(Convert.ToInt32(decryptParameters["EcCode"]));

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Check list details not exist.");
                        return PartialView("AddEditImsEcCheck", new IMSECCheckListViewModel());
                    }
                    return PartialView("AddEditImsEcCheck", model);
                }

                return PartialView("AddEditImsEcCheck", new IMSECCheckListViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, " Check list details not exist.");
                return PartialView("AddEditImsEcCheck", new IMSECCheckListViewModel());
            }
        }

        [HttpGet]
        public ActionResult ViewImsEcCheck(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    IMSECCheckListViewModel model = objBAL.GetImsEcCheckDetailsBAL(Convert.ToInt32(decryptParameters["EcCode"]));

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Check list details not exist.");
                        return PartialView("ViewImsEcCheck", new IMSECCheckListViewModel());
                    }
                    return PartialView("ViewImsEcCheck", model);
                }

                return PartialView("ViewImsEcCheck", new IMSECCheckListViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, " Check list details not exist.");
                return PartialView("ViewImsEcCheck", new IMSECCheckListViewModel());
            }
        }

        /// <summary>
        /// Method to update PIU department details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditImsEcCheck(IMSECCheckListViewModel model)
        {
            bool status = false;
            try
            {
                if (model.HdRoleTypeEntry == "S")
                {
                    if (ModelState.ContainsKey("IMS_NRRDA_REMARKS"))
                        ModelState["IMS_NRRDA_REMARKS"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_CN_READY_NRRDA"))
                        ModelState["IMS_CN_READY_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_CN_READY_NRRDA"))
                        ModelState["IMS_DRRP_OMMAS_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_DRRP_OMMAS_NRRDA"))
                        ModelState["IMS_DRRP_OMMAS_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_CNCUPL_READY_NRRDA"))
                        ModelState["IMS_CNCUPL_READY_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_DP_APPROVAL_NRRDA"))
                        ModelState["IMS_DP_APPROVAL_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_SLSC_PROCEEDING_NRRDA"))
                        ModelState["IMS_SLSC_PROCEEDING_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_DPR_SCRUTINY_NRRDA"))
                        ModelState["IMS_DPR_SCRUTINY_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_PCI_REGISTER_NRRDA"))
                        ModelState["IMS_PCI_REGISTER_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_UNSEALED_NRRDA"))
                        ModelState["IMS_UNSEALED_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_MP_DATA_NRRDA"))
                        ModelState["IMS_MP_DATA_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_MAINT_YEARWISE_NRRDA"))
                        ModelState["IMS_MAINT_YEARWISE_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_ESTIMATE_SSR_NRRDA"))
                        ModelState["IMS_ESTIMATE_SSR_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_DPR_STA_NRRDA"))
                        ModelState["IMS_DPR_STA_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_SSR_DATE_NRRDA"))
                        ModelState["IMS_SSR_DATE_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_NIT_UPLOADED_NRRDA"))
                        ModelState["IMS_NIT_UPLOADED_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_WORK_CAPACITY_NRRDA"))
                        ModelState["IMS_WORK_CAPACITY_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_IPAI_ACCOUNTS_NRRDA"))
                        ModelState["IMS_IPAI_ACCOUNTS_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_LWE_MHA_NRRDA"))
                        ModelState["IMS_LWE_MHA_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_WB_BATCH_SIZE_NRRDA"))
                        ModelState["IMS_WB_BATCH_SIZE_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_WB_ECOP_NRRDA"))
                        ModelState["IMS_WB_ECOP_NRRDA"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_WB_STA_CLEARED_NRRDA"))
                        ModelState["IMS_WB_STA_CLEARED_NRRDA"].Errors.Clear();
                }
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditImsEcCheckBAL(model, ref message))
                    {
                        message = message == string.Empty ? "Check list details updated successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "Check list details not updated." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Check list details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete admin department details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteImsEcCheck(String parameter, String hash, String key)
        {
            objBAL = new MasterBAL();
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteImsEcCheckBAL(Convert.ToInt32(decryptedParameters["EcCode"].ToString()), ref message))
                    {
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Check list details." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this Check list details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Check list details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult FinalizeECCheckList(String parameter, String hash, String key)
        {

            try
            {
                objBAL = new MasterBAL();
                bool status = false;
                IMSECCheckListViewModel modelImsEcCheck = new IMSECCheckListViewModel();
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    string EcType = decryptedParameters["ECType"].ToString();

                    if (objBAL.FinalizeECCheckListBAL(Convert.ToInt32(decryptedParameters["EcCode"]), ref message))
                    {
                        message = message == string.Empty ? "check list details finalize successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "check list details not finalize." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = status, message = "check list details not finalize." });
            }
            catch (Exception)
            {
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult DownloadViewECCheckList(String parameter, String hash, String key)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                objBAL = new MasterBAL();
                string FileName = string.Empty;
                string FullfilePhysicalPath = string.Empty;
                string FileExtension = ".pdf";
                string path = string.Empty;
                IMSECCheckListViewModel modelImsEcCheck = new IMSECCheckListViewModel();

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    string EcType = decryptedParameters["ECType"].ToString();
                    int ecId = Convert.ToInt32(decryptedParameters["EcCode"]);
                    PMGSY.Models.IMS_EC_PDF_GENERATED ecCheckListDetails = dbContext.IMS_EC_PDF_GENERATED.Where(m => m.MAST_EC_ID == ecId).FirstOrDefault();
                    if (ecCheckListDetails != null)
                    {
                        FullfilePhysicalPath = Path.Combine(ecCheckListDetails.IMS_FILE_PATH, ecCheckListDetails.IMS_FILE_NAME);

                        if (System.IO.File.Exists(FullfilePhysicalPath))
                        {
                            return File(FullfilePhysicalPath, "Application/pdf", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                        }
                        else
                        {
                            message = message == string.Empty ? "File not available." : message;
                            return Json(new { success = "false", message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        message = message == string.Empty ? "check list detail not available." : message;
                        return Json(new { success = "false", message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                message = message == string.Empty ? "check list detail not available." : message;
                return Json(new { success = "false", message = message, JsonRequestBehavior.AllowGet });
            }
            catch (Exception)
            {
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = "false", message = message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeFinalizeEC(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            objBAL = new MasterBAL();
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    Int32 EcCode = Convert.ToInt32(decryptParameters["EcCode"]);//Convert.ToInt32(Request.Params["EcCode"]);
                    if (EcCode != 0)
                    {
                        string Status = objBAL.DeFinalizeECBAL(EcCode);
                        if (Status == string.Empty)
                            return Json(new { Success = true });
                        else
                            return Json(new { Success = false, ErrorMessage = Status });
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }

        #endregion

        #region IMS EC  File Upload
        [HttpGet]
        public ActionResult ListImsEcFileUpload()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchImsEcFileUpload()
        {
            objDAL = new MasterDAL();
            IMSEcFileUploadSearchViewModel model = new IMSEcFileUploadSearchViewModel();
            return PartialView("SearchImsEcFileUpload", model);
        }


        [HttpPost]
        public ActionResult GetImsEcFileUploadList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int stateCode = 0;
            // int districtCode = 0;
            int agencyCode = 0;
            int year = 0;
            int batch = 0;
            string fileType = "%";
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            objBAL = new MasterBAL();
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

            if (!string.IsNullOrEmpty(Request.Params["agency"]))
            {
                agencyCode = Convert.ToInt32(Request.Params["agency"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["year"]))
            {
                year = Convert.ToInt32(Request.Params["year"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["batch"]))
            {
                batch = Convert.ToInt32(Request.Params["batch"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["fileType"]))
            {
                fileType = Request.Params["fileType"];
            }
            var jsonData = new
            {
                rows = objBAL.ListImsFileUploadBAL(stateCode, year, batch, agencyCode, fileType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Method to load add view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditImsEcFileUpload()
        {
            objBAL = new MasterBAL();
            IMSEcFileUploadViewModel model = new IMSEcFileUploadViewModel();
            return PartialView("AddEditImsEcFileUpload", model);
        }

        /// <summary>
        /// method to save admin department details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult AddEditImsEcFileUpload(IMSEcFileUploadViewModel model, HttpPostedFileBase file)
        public ActionResult AddEditImsEcFileUpload(FormCollection frmCollection)
        {
            bool status = false;
            db = new PMGSYEntities();
            IMSEcFileUploadViewModel model = new IMSEcFileUploadViewModel();
            CommonFunctions objCommonFunc = new CommonFunctions();
            try
            {
                HttpPostedFileBase file = Request.Files["file"];
                if (file != null)
                {
                    if (file.ContentLength > 0)
                    {

                        string fileTypes = ConfigurationManager.AppSettings["EC_FILE_UPLOAD_FORMAT"];
                        string[] arrfiletype = fileTypes.Split('$');
                        //string filename = string.Empty;
                        string path = string.Empty;
                        bool fileExt = false;
                        string fileSaveExt = string.Empty;
                        foreach (var item in arrfiletype)
                        {
                            if (item == file.FileName.Split('.')[1])
                            {
                                fileExt = true;
                                fileSaveExt = item;
                                break;
                            }
                        }
                        if (fileExt == true)
                        {
                            switch (fileSaveExt)
                            {
                                case "pdf":
                                    path = ConfigurationManager.AppSettings["EC_FILE_UPLOAD_PDF"];
                                    break;
                                case "jpg":
                                case "bmp":
                                case "tiff":
                                case "png":
                                case "gif":
                                case "jpeg":
                                    path = ConfigurationManager.AppSettings["EC_FILE_UPLOAD_Image"];
                                    break;
                                case "doc":
                                case "docx":
                                    path = ConfigurationManager.AppSettings["EC_FILE_UPLOAD_DOC"];
                                    break;

                            }

                            model.PhaseYear = Convert.ToInt32(frmCollection["PhaseYear"]);
                            model.Batch = Convert.ToInt32(frmCollection["Batch"]); ;
                            model.Mast_Agency = Convert.ToInt32(frmCollection["Mast_Agency"]); ;
                            model.ImsFileType = frmCollection["ImsFileType"];
                            model.Mast_State_Code = Convert.ToInt32(frmCollection["Mast_State_Code"]) == 0 ? Convert.ToInt32(frmCollection["hdStateCode"]) : Convert.ToInt32(frmCollection["Mast_State_Code"]);

                            if (ModelState.IsValid)
                            {
                                //filename = Path.GetFileName(Request.Files["file"].FileName);
                                var fileName = (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.Mast_State_Code).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.PhaseYear + "-" + (model.PhaseYear + 1)) + "_BATCH" + (model.Batch == null ? 0 : model.Batch) + "_" + (model.ImsFileType) + "_SCHEME" + PMGSYSession.Current.PMGSYScheme + "." + fileSaveExt;
                                model.ImsFileName = fileName;
                                model.ImsFilePath = path;
                                objBAL = new MasterBAL();
                                if (objBAL.AddImsEcFileUploadBAL(model, ref message))
                                {
                                    if (message == string.Empty)
                                    {
                                        Request.Files["file"].SaveAs(Path.Combine(path, fileName));
                                    }
                                    message = message == string.Empty ? "File Upload details saved successfully." : message;
                                    status = true;


                                    //if (message == string.Empty)
                                    //{
                                    //    TempData["notice"] = "Y";
                                    //    return RedirectToAction("ListImsEcFileUpload");
                                    //}
                                    //else
                                    //{
                                    //     return View("AddEditImsEcFileUpload", model);
                                    //}
                                    return Json(new { success = status, message = message == string.Empty ? "File Upload details not saved" : message });

                                }
                                else
                                {

                                    return Json(new { success = status, message = message == string.Empty ? "File Upload details not saved" : message });

                                }
                            }
                            else
                            {
                                return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                            }
                        }
                        else
                        {
                            message = "File type is not allowed.";

                            // return View("AddEditImsEcFileUpload", model);
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);


                        }
                    }
                    else
                    {
                        message = message == string.Empty ? "Please select file to upload." : message;
                        // return View("AddEditImsEcFileUpload", model);
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    message = message == string.Empty ? "Please select file to upload." : message;
                    // return View("AddEditImsEcFileUpload", model);
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                // return View("AddEditImsEcFileUpload", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "File Upload details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Method to Get PIU department details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditImsEcFileUpload(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    IMSEcFileUploadViewModel model = objBAL.GetImsEcFileUploadDetailsBAL(Convert.ToInt32(decryptParameters["FileCode"]));

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "File Upload details not exist.");
                        return PartialView("AddEditImsEcFileUpload", new IMSEcFileUploadViewModel());
                    }

                    return PartialView("AddEditImsEcFileUpload", model);
                }

                return PartialView("AddEditImsEcFileUpload", new IMSEcFileUploadViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, " File Upload details not exist.");
                return PartialView("AddEditImsEcFileUpload", new IMSEcFileUploadViewModel());
            }
        }

        /// <summary>
        /// Method to update PIU department details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditImsEcFileUpload(IMSEcFileUploadViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditImsFileUploadBAL(model, ref message))
                    {
                        message = message == string.Empty ? "File Upload details updated successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "File Upload details not updated." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "File Upload details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete admin department details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteImsEcFileUpload(String parameter, String hash, String key)
        {
            objBAL = new MasterBAL();
            bool status = false;
            string FileName = string.Empty;
            string FullfilePhysicalPath = string.Empty;
            string FileExtension = string.Empty;
            string path = string.Empty;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    IMSEcFileUploadViewModel model = objBAL.GetImsEcFileUploadDetailsBAL(Convert.ToInt32(decryptedParameters["FileCode"]));

                    if (model != null)
                    {
                        FileExtension = Path.GetExtension(model.ImsFileName).ToLower();

                        //switch (FileExtension)
                        //{
                        //    case ".pdf":
                        //        path = ConfigurationManager.AppSettings["EC_FILE_UPLOAD_PDF"];
                        //        break;
                        //    case ".jpg":
                        //    case ".bmp":
                        //    case ".tiff":
                        //    case ".png":
                        //    case ".gif":
                        //    case ".jpeg":
                        //        path = ConfigurationManager.AppSettings["EC_FILE_UPLOAD_Image"];
                        //        break;
                        //    case ".doc":
                        //    case ".docx":
                        //        path = ConfigurationManager.AppSettings["EC_FILE_UPLOAD_DOC"];
                        //        break;

                        //}


                        FullfilePhysicalPath = Path.Combine(model.ImsFilePath, model.ImsFileName);

                        if (objBAL.DeleteImsFileUploadBAL(Convert.ToInt32(decryptedParameters["FileCode"].ToString()), ref message))
                        {
                            status = true;
                            System.IO.File.Delete(FullfilePhysicalPath);
                        }
                        else
                        {
                            message = message == string.Empty ? "You can not delete this File Upload details." : message;
                        }
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this File Upload details." : message;

                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this File Upload details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this File Upload details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult DownloadECFile(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullfilePhysicalPath = string.Empty;
            string FileExtension = string.Empty;
            string path = string.Empty;

            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    FileName = (urlSplitParams[0]);
                }
            }
            FileExtension = Path.GetExtension(FileName).ToLower();
            switch (FileExtension)
            {
                case ".pdf":
                    path = ConfigurationManager.AppSettings["EC_FILE_UPLOAD_PDF"];
                    break;
                case ".jpg":
                case ".bmp":
                case ".tiff":
                case ".png":
                case ".gif":
                case ".jpeg":
                    path = ConfigurationManager.AppSettings["EC_FILE_UPLOAD_Image"];
                    break;
                case ".doc":
                case ".docx":
                    path = ConfigurationManager.AppSettings["EC_FILE_UPLOAD_DOC"];
                    break;

            }


            FullfilePhysicalPath = Path.Combine(path, FileName);

            string name = Path.GetFileName(FileName);
            string ext = Path.GetExtension(FileName);

            string type = string.Empty;

            if (ext != null)
            {
                switch (ext.ToLower())
                {
                    case ".pdf":
                        type = "Application/pdf";
                        break;
                    case ".doc":
                    case ".docx":
                        type = "Application/msword";
                        break;
                    case ".jpg":
                    case ".bmp":
                    case ".tiff":
                    case ".png":
                    case ".gif":
                    case ".jpeg":
                        type = "image/png";
                        break;
                    default:
                        type = "Application";
                        break;
                }
            }



            if (System.IO.File.Exists(FullfilePhysicalPath))
            {
                return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
            }
            else
            {
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult FileUploadIndex()
        {
            return View();
        }
        #endregion

        #region IMS EC Training
        [HttpGet]
        public ActionResult ListImsEcTraining()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchImsEcTraining()
        {
            objDAL = new MasterDAL();
            MasterDAL masterDataEntryDAL = new MasterDAL();
            IMSEcTrainingSearchViewModel model = new IMSEcTrainingSearchViewModel();
            return PartialView("SearchImsEcTraining", model);
        }


        [HttpPost]
        public ActionResult GetImsEcTrainingList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int stateCode = 0;
            int designationCode = 0;
            int year = 0;

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            objBAL = new MasterBAL();
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

            if (!string.IsNullOrEmpty(Request.Params["designation"]))
            {
                designationCode = Convert.ToInt32(Request.Params["designation"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["year"]))
            {
                year = Convert.ToInt32(Request.Params["year"]);
            }

            var jsonData = new
            {
                rows = objBAL.ListImsEcTrainingBAL(stateCode, year, designationCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Method to load add view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditImsEcTraining()
        {
            objBAL = new MasterBAL();
            IMSEcTrainingViewModel model = new IMSEcTrainingViewModel();
            return PartialView("AddEditImsEcTraining", model);
        }

        /// <summary>
        /// method to save admin department details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditImsEcTraining(IMSEcTrainingViewModel model)
        {
            bool status = false;
            try
            {

                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddImsEcTrainingBAL(model, ref message))
                    {
                        message = message == string.Empty ? "Training details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Training  details not saved." : message;
                    }
                }
                else
                {
                    return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Training details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to Get PIU department details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditImsEcTraining(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                MasterDAL masterDataEntryDAL = new MasterDAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    IMSEcTrainingViewModel model = objBAL.GetImsEcTrainingDetailsBAL(Convert.ToInt32(decryptParameters["TrainingCode"]));

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Training details not exist.");
                        return PartialView("AddEditImsEcTraining", new IMSEcTrainingViewModel());
                    }
                    return PartialView("AddEditImsEcTraining", model);
                }

                return PartialView("AddEditImsEcTraining", new IMSEcTrainingViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, " Training details not exist.");
                return PartialView("AddEditImsEcTraining", new IMSEcTrainingViewModel());
            }
        }

        /// <summary>
        /// Method to update PIU department details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditImsEcTraining(IMSEcTrainingViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditImsTrainingBAL(model, ref message))
                    {
                        message = message == string.Empty ? "Training details updated successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "Training details not updated." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Training  details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete admin department details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteImsEcTraining(String parameter, String hash, String key)
        {
            objBAL = new MasterBAL();
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteImsTrainingBAL(Convert.ToInt32(decryptedParameters["TrainingCode"].ToString()), ref message))
                    {
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Training  details." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this Training details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Training details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region Common function
        public ActionResult DistrictDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DistrictSelectDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
            // list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockSelectDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), false);
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Growth Score Actions

        /// <summary>
        /// MasterScourFoundationType() action is used to display Scour Foundation Type Add/Edit
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GrowthScoreLayout()
        {
            return View();
        }

        /// <summary>
        /// Display Scour Foundation Type List
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>


        public ActionResult GetMasterGrowthScoreList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            string SfType = String.Empty;
            long totalRecords;

            try
            {
                objBAL = new MasterBAL();

                if (!string.IsNullOrEmpty(Request.Params["SfTypeCode"]))
                {
                    SfType = Request.Params["SfTypeCode"].Replace('+', ' ').Trim();
                }

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListGrowthMasterScoreType(SfType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        /// <summary>
        /// Display Scour Foundation Type Add Data Entry form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMasterGrowthScoreType()
        {
            GrowthScoreViewModel obj = new GrowthScoreViewModel();

            return PartialView("AddEditMasterGrowthScoreType", obj);
        }

        /// <summary>
        /// Save Scour Foundation Type Details
        /// </summary>
        /// <param name="masterScourFoundationTypeViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AddMasterGrowthScoreType(GrowthScoreViewModel objScore)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMasterGrowthScore(objScore, ref message))
                    {
                        message = message == string.Empty ? "Score details saved successfully." : message;
                        status = true;
                    }
                    else
                    {

                        message = message == string.Empty ? "Score details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterGrowthScoreType", objScore);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Score details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Display Score Type Details in Edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMasterGrowthScoreType(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                objDAL = new MasterDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    GrowthScoreViewModel objScore = objDAL.GetGrowthScoreDetails_ByScoreID(Convert.ToInt32(decryptedParameters["ScoreID"].ToString()));
                    if (objScore == null)
                    {
                        ModelState.AddModelError(string.Empty, "Score details does not exist.");
                        return PartialView("AddEditMasterGrowthScoreType", new GrowthScoreViewModel());
                    }
                    return PartialView("AddEditMasterGrowthScoreType", objScore);

                }
                return PartialView("AddEditMasterGrowthScoreType", new GrowthScoreViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Score details does not exist.");
                return PartialView("AddEditMasterGrowthScoreType", new GrowthScoreViewModel());
            }
        }

        /// <summary>
        /// EditMasterScourFoundationType action is used to Update Scour Foundation Type Details
        /// </summary>
        /// <param name="masterScourFoundationTypeViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGrowthScoreType(GrowthScoreViewModel growthScoreViewModel)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMasterGrowthScore(growthScoreViewModel, ref message))
                    {
                        message = message == string.Empty ? "Score details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Score details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMasterGrowthScoreType", growthScoreViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Score details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Delete Score Type Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMasterGrowthScore(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                objBAL = new MasterBAL();

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteMasterGrowthScore(Convert.ToInt32(decryptedParameters["ScoreID"].ToString()), ref message))
                    {
                        message = "Score details and Sub Item details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Score details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                message = "You can not delete this Score details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Score details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult GrowthScoreSubItemLayout(String parameter, String hash, String key)
        {
            string scoreId = "";
            GrowthScoreSubItemViewModel scoreViewModel = new GrowthScoreSubItemViewModel();

            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            if (decryptedParameters.Count > 0)
            {
                scoreId = Convert.ToString(decryptedParameters["ParentId"]).Trim();
            }
            scoreViewModel.hdnScoreCode = scoreId;//parameter.Trim() + "/" + hash.Trim() + "/" + key.Trim();

            return View(scoreViewModel);
        }

        /// <summary>
        /// SearchScourFoundation() action is used to Search Scour Foundation details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GrowthScoreSubItemDetails(string scoreId)
        {
            GrowthScoreSubItemViewModel obj = new GrowthScoreSubItemViewModel();
            try
            {
                IMasterDAL objDAL = new MasterDAL();

                obj.hdnScoreCode = scoreId;

                return PartialView(obj);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

            }
            return PartialView(obj);
        }

        //[HttpGet]
        public ActionResult GrowthScoreSubItemInfo(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            GrowthScoreSubItemViewModel obj = new GrowthScoreSubItemViewModel();
            try
            {
                string scoreId = "";

                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    scoreId = Convert.ToString(decryptedParameters["ParentId"]).Trim();
                }

                obj.hdnScoreCode = scoreId;

                int id = Convert.ToInt32(scoreId);

                var s = dbContext.MASTER_GROWTH_SCORE.Where(a => a.MAST_SCORE_ID == id).ToList();

                foreach (var a in s)
                {
                    obj.Description = a.MAST_SCORE_NAME.Trim();
                    obj.Value = Convert.ToString(a.MAST_SCORE_VALUE).Trim();
                    obj.Type = a.MAST_SCORE_TYPE.Trim() == "H" ? "Highest" : "Cumulative";
                }

                return PartialView(obj);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
            return PartialView(obj);
        }

        public ActionResult GetMasterGrowthScoreSubItemList(int? page, int? rows, string sidx, string sord, int scoreId)
        {

            String searchParameters = String.Empty;
            string SfType = String.Empty;
            long totalRecords;

            try
            {
                objBAL = new MasterBAL();

                if (!string.IsNullOrEmpty(Request.Params["SfTypeCode"]))
                {
                    SfType = Request.Params["SfTypeCode"].Replace('+', ' ').Trim();
                }

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListScoreSubItems(SfType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, scoreId),
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

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AddGrowthScoreSubItem(GrowthScoreSubItemViewModel objScore)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddScoreSubItemDetails(objScore, ref message))
                    {
                        message = message == string.Empty ? "Score Sub Item details saved successfully." : message;
                        status = true;
                    }
                    else
                    {

                        message = message == string.Empty ? "Score Sub Item details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("GrowthScoreSubItemDetails", objScore);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Score Sub Item details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult EditScoreSubItem(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new MasterBAL();
                objDAL = new MasterDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    GrowthScoreSubItemViewModel objScore = objDAL.GetScoreSubItemDetails_ByScoreID(Convert.ToInt32(decryptedParameters["ScoreID"].ToString()));
                    if (objScore == null)
                    {
                        ModelState.AddModelError(string.Empty, "Score Sub Item details does not exist.");
                        return PartialView("GrowthScoreSubItemDetails", new GrowthScoreSubItemViewModel());
                    }
                    return PartialView("GrowthScoreSubItemDetails", objScore);

                }
                return PartialView("GrowthScoreSubItemDetails", new GrowthScoreSubItemViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Score Sub Item details does not exist.");
                return PartialView("GrowthScoreSubItemDetails", new GrowthScoreSubItemViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGrowthScoreSubItemDetails(GrowthScoreSubItemViewModel growthScoreViewModel)
        {
            bool status = false;

            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditGrowthScoreSubItemDetails(growthScoreViewModel, ref message))
                    {
                        message = message == string.Empty ? "Score Sub Item details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Score Sub Item details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("GrowthScoreSubItemDetails", growthScoreViewModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Score details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteScoreSubItemDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                objBAL = new MasterBAL();

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteScoreSubItem(Convert.ToInt32(decryptedParameters["ScoreID"].ToString()), ref message))
                    {
                        message = "Score Sub Item details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Score Sub Item details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                message = "You can not delete this Score Sub Item details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Score Sub Item details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Growth Score Actions

        #region Quality Inspection & ATR Deletions

        [Audit]
        public ActionResult QualityLayout(string id)
        {
            QMFilterViewModel model = new QMFilterViewModel();
            model.QM_TYPE_CODE = id;
            return View(model);
        }

        /// <summary>
        /// Common Filters for Quality Module
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QualityFilters(string id)
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.MAST_STATE_CODE = 0;
            qmFilterModel.ADMIN_QM_CODE = 0;

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.TO_MONTH = DateTime.Now.Month;
            qmFilterModel.TO_YEAR = DateTime.Now.Year;

            qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
            qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

            qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", id, 0); //Purposely taken String "false" as argument

            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.QM_TYPE_CODE = id;
            return View(qmFilterModel);
        }

        /// <summary>
        /// View Inspection Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMViewInspectionDetails(FormCollection formCollection)
        {
            objBAL = new MasterBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = objBAL.QMViewInspectionDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["monitorCode"]),
                                                            Convert.ToInt32(formCollection["fromInspMonth"]), Convert.ToInt32(formCollection["fromInspYear"]),
                                                            Convert.ToInt32(formCollection["toInspMonth"]), Convert.ToInt32(formCollection["toInspYear"]), formCollection["qmType"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        /// <summary>
        /// Filters for ATR Details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QualityATRFilters()
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

            qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
            qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

            qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "0", 0); //Purposely taken String "false" as argument

            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.ATR_STATUS_LIST = objCommonFunctions.QualityATRStatus();
            qmFilterModel.ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus();
            return View(qmFilterModel);
        }


        /// <summary>
        /// View for ATR Details in  HTML Report
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult QualityATRDetails(FormCollection formCollection)
        {
            objBAL = new MasterBAL();
            QMATRDetailsModel atrDetailsModel = new QMATRDetailsModel();
            atrDetailsModel.ATR_LIST = new List<QMATRModel>();
            atrDetailsModel.OBS_LIST = new List<QMObsATRModel>();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                atrDetailsModel.OBS_ATR_LIST = objBAL.ATRDetailsBAL(Convert.ToInt32(formCollection["MAST_STATE_CODE"]), Convert.ToInt32(formCollection["ADMIN_QM_CODE"]),
                                                                Convert.ToInt32(formCollection["FROM_MONTH"]), Convert.ToInt32(formCollection["FROM_YEAR"]),
                                                                Convert.ToInt32(formCollection["TO_MONTH"]), Convert.ToInt32(formCollection["TO_YEAR"]),
                                                                formCollection["ATR_STATUS"], formCollection["ROAD_STATUS"]);

                var distinctObsList = (from obs in atrDetailsModel.OBS_ATR_LIST
                                       select new
                                       {
                                           obs.QM_OBSERVATION_ID,
                                           obs.MONITOR_NAME,
                                           obs.STATE_NAME,
                                           obs.DISTRICT_NAME,
                                           obs.BLOCK_NAME,
                                           obs.IMS_PACKAGE_ID,
                                           obs.IMS_YEAR,
                                           obs.IMS_ROAD_NAME,
                                           obs.QM_INSPECTED_START_CHAINAGE,
                                           obs.QM_INSPECTED_END_CHAINAGE,
                                           obs.QM_INSPECTION_DATE,
                                           obs.IMS_ISCOMPLETED,
                                           obs.OVERALL_GRADE,
                                           obs.NO_OF_PHOTO_UPLOADED,
                                           obs.QM_ATR_STATUS,
                                           obs.PMGSY_SCHEME,
                                           obs.ADMIN_IS_ENQUIRY,
                                           obs.IMS_PROPOSAL_TYPE
                                       }).Distinct().ToList();

                foreach (var item in distinctObsList)
                {
                    QMObsATRModel obsModel = new QMObsATRModel();
                    obsModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
                    obsModel.MONITOR_NAME = item.MONITOR_NAME;
                    obsModel.STATE_NAME = item.STATE_NAME;
                    obsModel.DISTRICT_NAME = item.DISTRICT_NAME;
                    obsModel.BLOCK_NAME = item.BLOCK_NAME;
                    obsModel.IMS_PACKAGE_ID = item.IMS_PACKAGE_ID;
                    obsModel.IMS_YEAR = item.IMS_YEAR.ToString() + "-" + (item.IMS_YEAR + 1).ToString();
                    obsModel.IMS_ROAD_NAME = item.IMS_ROAD_NAME;
                    obsModel.QM_INSPECTED_START_CHAINAGE = item.QM_INSPECTED_START_CHAINAGE;
                    obsModel.QM_INSPECTED_END_CHAINAGE = item.QM_INSPECTED_END_CHAINAGE;
                    obsModel.QM_INSPECTION_DATE = item.QM_INSPECTION_DATE;
                    obsModel.IMS_ISCOMPLETED = item.IMS_ISCOMPLETED;
                    obsModel.PMGSY_SCHEME = item.PMGSY_SCHEME;
                    obsModel.OVERALL_GRADE = item.OVERALL_GRADE;
                    obsModel.NO_OF_PHOTO_UPLOADED = item.NO_OF_PHOTO_UPLOADED;
                    obsModel.QM_ATR_STATUS = item.QM_ATR_STATUS;
                    obsModel.ADMIN_IS_ENQUIRY = item.ADMIN_IS_ENQUIRY.Equals("Y") ? "Yes" : "No";
                    obsModel.IMS_PROPOSAL_TYPE = item.IMS_PROPOSAL_TYPE.Equals("P") ? "Road" : "LSB";
                    obsModel.SHOW_OBS_LINK = "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRObsDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>View</a>";

                    atrDetailsModel.OBS_LIST.Add(obsModel);
                }

                //int index = 0;
                foreach (var item in atrDetailsModel.OBS_ATR_LIST)
                {
                    // Populate & Add ATR Details for each Observation Id
                    QMATRModel atrModel = new QMATRModel();
                    atrModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
                    atrModel.QM_ATR_ID = item.QM_ATR_ID;
                    atrModel.ATR_ENTRY_DATE = item.ATR_ENTRY_DATE;
                    atrModel.ATR_REGRADE_STATUS = item.ATR_REGRADE_STATUS.Trim().Equals("N") ? "No" : "Yes";
                    atrModel.ATR_REGRADE_REMARKS = item.ATR_REGRADE_REMARKS;
                    atrModel.ATR_REGRADE_DATE = item.ATR_REGRADE_DATE;
                    atrModel.ATR_IS_DELETED = item.ATR_IS_DELETED;
                    atrModel.QM_ATR_STATUS = item.QM_ATR_STATUS;

                    atrModel.IS_SUBMITTED = item.QM_ATR_STATUS.Trim().Equals("N") ? "No" : "Yes";
                    atrModel.ATR_UPLOAD_VIEW_LINK =
                        PMGSYSession.Current.RoleCode == 22 //for PIU view only
                        ? (item.QM_ATR_ID != null)
                                            ? "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>"
                                            : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                        : (item.QM_ATR_STATUS.Trim().Equals("N")) // Upload/View
                                        ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Upload</a>"
                                        : (item.QM_ATR_ID != null)
                                            ? "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>"
                                            : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrModel.ATR_ACCEPTANCE_LINK =
                         PMGSYSession.Current.RoleCode == 22 //for PIU view only
                         ? item.ATR_REGRADE_STATUS.Trim().Equals("A")
                                        ? "Accepted"
                                        : item.ATR_REGRADE_STATUS.Trim().Equals("R")
                                            ? "Rejected"
                                            : item.ATR_REGRADE_STATUS.Equals("V")
                                                ? "Verified"
                                                : item.ATR_REGRADE_STATUS.Equals("D")
                                                    ? "Non Rectifible Deffect"
                                                    : ""
                         : item.ATR_REGRADE_STATUS.Trim().Equals("A")     // Acceptance
                                        ? "Accepted"
                                        : item.ATR_REGRADE_STATUS.Equals("V")
                                            ? "Verified"
                                            : item.ATR_REGRADE_STATUS.Equals("D")
                                                ? "Non Rectifible Deffect"
                                                : item.ATR_REGRADE_STATUS.Trim().Equals("R") // if any of the ATR against Obs Id is Accepted then dont provide link to upload.
                                                    ? item.QM_ATR_STATUS.Equals("A")
                                                        ? "Rejected"
                                                        : "Rejected" + //If Rejected atr is last against Observation, then append + sign to upload again
                                                          (atrModel.QM_ATR_ID == (dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max())
                                                             ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Upload</a>"
                                                             : "")
                                                    : "";

                    atrModel.ATR_REGRADE_LINK =
                        PMGSYSession.Current.RoleCode == 22 //for PIU view only
                        ? (item.ATR_REGRADE_STATUS.Trim().Equals("A"))
                            ? "<a href='#' title='Click here to see observation details against ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRGradingDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                            : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                        : ((item.ATR_REGRADE_STATUS.Trim().Equals("U") || item.ATR_REGRADE_STATUS.Trim().Equals("V")) && item.ATR_IS_DELETED.Equals("N")) // Regrade, for recent entry only
                            ? "<a href='#' title='Click here to regrade ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='RegradeATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                            : (item.ATR_REGRADE_STATUS.Trim().Equals("A"))
                                ? "<a href='#' title='Click here to see observation details against ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRGradingDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                                : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrModel.ATR_DELETE_LINK =
                         PMGSYSession.Current.RoleCode == 22 //for PIU view only
                         ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                         : (item.QM_ATR_ID != null && item.QM_ATR_STATUS.Trim() != "A" && item.QM_ATR_STATUS.Trim() != "N" && (item.QM_ATR_ID == dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max()))
                                ? "<a href='#' title='Click here to delete ATR details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteATR(\"" +
                                    item.QM_OBSERVATION_ID.ToString().Trim() + "\",\"" + dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID && a.ATR_IS_DELETED == "N").Select(a => a.QM_ATR_ID).FirstOrDefault()
                                    + "\"); return false;'>Delete</a>"
                                : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrDetailsModel.ATR_LIST.Add(atrModel);

                    //index++; // increment index for each record.
                }

                return View(atrDetailsModel);
            }
            catch
            {
                atrDetailsModel.ERROR = "Error occurred while processing your request";
                return View(atrDetailsModel); //return model as null
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region MAINTENANCE_POLICY_UPLOAD

        /// <summary>
        /// main view for listing the file upload details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListMaintenancePolicyFileUpload()
        {
            return View();
        }

        /// <summary>
        /// search view for maintenance policy file upload
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchMaintenancePolicy()
        {
            try
            {
                List<SelectListItem> lstSelectOption = new List<SelectListItem>();
                lstSelectOption.Insert(0, new SelectListItem { Value = "0", Text = "Select Agency" });
                MaintenancePolicyFilterViewModel model = new MaintenancePolicyFilterViewModel();
                model.lstStatesSearch = common.PopulateStates(true);
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.State = PMGSYSession.Current.StateCode;
                    model.lstAgenciesSearch = common.PopulateAgenciesByStateAndDepartmentwise(PMGSYSession.Current.StateCode, PMGSYSession.Current.AdminNdCode, true);
                }
                else
                {
                    model.lstAgenciesSearch = lstSelectOption;
                }
                return PartialView("SearchMaintenancePolicy", model);
            }
            catch (Exception)
            {
                return null;
            }

        }


        /// <summary>
        /// returns the list of maintenance policy file upload 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMaintenancePolicyFileUploadList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int stateCode = 0;
            int agencyCode = 0;
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            objBAL = new MasterBAL();
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

            if (!string.IsNullOrEmpty(Request.Params["agency"]))
            {
                agencyCode = Convert.ToInt32(Request.Params["agency"]);
            }

            var jsonData = new
            {
                rows = objBAL.ListMaintenancePolicyBAL(stateCode, agencyCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Method to load add view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEditMaintenancePolicy()
        {
            List<SelectListItem> lstSelectOption = new List<SelectListItem>();
            List<SelectListItem> lstFileTypes = new List<SelectListItem>();
            lstSelectOption.Insert(0, new SelectListItem { Value = "0", Text = "Select Agency" });
            MaintenancePolicyViewModel model = new MaintenancePolicyViewModel();

            lstFileTypes.Insert(0, new SelectListItem { Value = "0", Text = "Select File Type" });
            lstFileTypes.Insert(1, new SelectListItem { Value = "HW", Text = "Hindi/Regional Language - Word" });
            lstFileTypes.Insert(2, new SelectListItem { Value = "EW", Text = "English - Word" });
            lstFileTypes.Insert(3, new SelectListItem { Value = "HP", Text = "Hindi/Regional Language - PDF" });
            lstFileTypes.Insert(4, new SelectListItem { Value = "EP", Text = "English - PDF" });

            model.lstStates = common.PopulateStates(true);
            model.lstFileTypes = lstFileTypes;
            if (PMGSYSession.Current.StateCode > 0)
            {
                model.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                model.lstAgencies = common.PopulateAgenciesByStateAndDepartmentwise(PMGSYSession.Current.StateCode, PMGSYSession.Current.AdminNdCode, false);
            }
            else
            {
                model.lstAgencies = lstSelectOption;
            }
            return PartialView("AddEditMaintenancePolicy", model);
        }

        /// <summary>
        /// method to save admin department details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddEditMaintenancePolicy(FormCollection frmCollection)
        {
            bool status = false;
            db = new PMGSYEntities();
            MaintenancePolicyViewModel model = new MaintenancePolicyViewModel();
            CommonFunctions objCommonFunc = new CommonFunctions();
            try
            {
                HttpPostedFileBase file = Request.Files["file"];
                if (file != null)
                {
                    if (file.ContentLength > 0)
                    {

                        string fileTypes = ConfigurationManager.AppSettings["MAINTENANCE_POLICY_FILE_UPLOAD_FORMAT"];
                        string[] arrfiletype = fileTypes.Split('$');
                        string path = string.Empty;
                        bool fileExt = false;
                        string fileSaveExt = string.Empty;
                        foreach (var item in arrfiletype)
                        {
                            if (item == file.FileName.Split('.')[1])
                            {
                                fileExt = true;
                                fileSaveExt = item;
                                break;
                            }
                        }
                        if (fileExt == true)
                        {
                            switch (fileSaveExt)
                            {
                                case "pdf":
                                    path = ConfigurationManager.AppSettings["MAINTENANCE_POLICY_FILE_UPLOAD_PDF"];
                                    break;
                                case "doc":
                                    path = ConfigurationManager.AppSettings["MAINTENANCE_POLICY_FILE_UPLOAD_DOC"];
                                    break;
                                case "docx":
                                    path = ConfigurationManager.AppSettings["MAINTENANCE_POLICY_FILE_UPLOAD_DOC"];
                                    break;
                            }

                            switch (frmCollection["IMS_FILE_TYPE"])
                            {
                                case "HW":
                                    if (fileSaveExt == "pdf")
                                    {
                                        return Json(new { success = false, message = "File type selected is not valid." });
                                    }
                                    break;
                                case "EW":
                                    if (fileSaveExt == "pdf")
                                    {
                                        return Json(new { success = false, message = "File type selected is not valid." });
                                    }
                                    break;
                                case "HP":
                                    if (fileSaveExt == "doc" || fileSaveExt == "docx")
                                    {
                                        return Json(new { success = false, message = "File type selected is not valid." });
                                    }
                                    break;
                                case "EP":
                                    if (fileSaveExt == "doc" || fileSaveExt == "docx")
                                    {
                                        return Json(new { success = false, message = "File type selected is not valid." });
                                    }
                                    break;
                                default:
                                    break;
                            }



                            model.MAST_AGENCY_CODE = Convert.ToInt32(frmCollection["MAST_AGENCY_CODE"]); ;
                            model.IMS_FILE_TYPE = frmCollection["IMS_FILE_TYPE"];
                            model.MAST_STATE_CODE = Convert.ToInt32(frmCollection["MAST_STATE_CODE"]) == 0 ? Convert.ToInt32(frmCollection["hdStateCode"]) : Convert.ToInt32(frmCollection["MAST_STATE_CODE"]);
                            model.IMS_POLICY_DATE = frmCollection["IMS_POLICY_DATE"];
                            if (ModelState.IsValid)
                            {
                                //filename = Path.GetFileName(Request.Files["file"].FileName);
                                var fileName = (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.MAST_STATE_CODE).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.IMS_FILE_TYPE) + "_SCHEME" + PMGSYSession.Current.PMGSYScheme + "." + fileSaveExt;
                                model.IMS_FILE_NAME = fileName;
                                model.IMS_FILE_PATH = path;
                                objBAL = new MasterBAL();
                                if (objBAL.AddMaintenancePolicyBAL(model, ref message))
                                {
                                    if (message == string.Empty)
                                    {
                                        Request.Files["file"].SaveAs(Path.Combine(path, fileName));
                                    }
                                    message = message == string.Empty ? "File Upload details saved successfully." : message;
                                    status = true;
                                    return Json(new { success = status, message = message == string.Empty ? "File Upload details not saved" : message });
                                }
                                else
                                {
                                    return Json(new { success = status, message = message == string.Empty ? "File Upload details not saved" : message });
                                }
                            }
                            else
                            {
                                return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                            }
                        }
                        else
                        {
                            message = "File type is not allowed.";
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        message = message == string.Empty ? "Please select file to upload." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = message == string.Empty ? "Please select file to upload." : message;
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "File Upload details not saved." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Method to Get PIU department details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditMaintenancePolicy(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                objBAL = new MasterBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MaintenancePolicyViewModel model = objBAL.GetMaintenancePolicyDetailsBAL(Convert.ToInt32(decryptParameters["FileCode"]));

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "File Upload details not exist.");
                        return PartialView("AddEditMaintenancePolicy", new MaintenancePolicyViewModel());
                    }

                    return PartialView("AddEditMaintenancePolicy", model);
                }

                return PartialView("AddEditMaintenancePolicy", new MaintenancePolicyViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, " File Upload details not exist.");
                return PartialView("AddEditMaintenancePolicy", new MaintenancePolicyViewModel());
            }
        }

        /// <summary>
        /// Method to update PIU department details. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMaintenancePolicyUpload(MaintenancePolicyViewModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMaintenancePolicyBAL(model, ref message))
                    {
                        message = message == string.Empty ? "File Upload details updated successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "File Upload details not updated." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "File Upload details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete admin department details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMaintenancePolicyFile(String parameter, String hash, String key)
        {
            objBAL = new MasterBAL();
            bool status = false;
            string FileName = string.Empty;
            string FullfilePhysicalPath = string.Empty;
            string FileExtension = string.Empty;
            string path = string.Empty;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MaintenancePolicyViewModel model = objBAL.GetMaintenancePolicyDetailsBAL(Convert.ToInt32(decryptedParameters["FileCode"]));

                    if (model != null)
                    {
                        FileExtension = Path.GetExtension(model.IMS_FILE_NAME).ToLower();

                        FullfilePhysicalPath = Path.Combine(model.IMS_FILE_PATH, model.IMS_FILE_NAME);

                        if (objBAL.DeleteMaintenancePolicyBAL(Convert.ToInt32(decryptedParameters["FileCode"].ToString()), ref message))
                        {
                            status = true;
                            System.IO.File.Delete(FullfilePhysicalPath);
                        }
                        else
                        {
                            message = message == string.Empty ? "You can not delete this File Upload details." : message;
                        }
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this File Upload details." : message;

                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this File Upload details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this File Upload details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// download action for uploaded file
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult DownloadMaintenancePolicyFile(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullfilePhysicalPath = string.Empty;
            string FileExtension = string.Empty;
            string path = string.Empty;

            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    FileName = (urlSplitParams[0]);
                }
            }

            FileExtension = Path.GetExtension(FileName).ToLower();

            switch (FileExtension)
            {
                case ".pdf":
                    path = ConfigurationManager.AppSettings["MAINTENANCE_POLICY_FILE_UPLOAD_PDF"];
                    break;
                case ".doc":
                    path = ConfigurationManager.AppSettings["MAINTENANCE_POLICY_FILE_UPLOAD_DOC"];
                    break;
                case ".docx":
                    path = ConfigurationManager.AppSettings["MAINTENANCE_POLICY_FILE_UPLOAD_DOC"];
                    break;
            }


            FullfilePhysicalPath = Path.Combine(path, FileName);

            string name = Path.GetFileName(FileName);
            string ext = Path.GetExtension(FileName);

            string type = string.Empty;

            if (ext != null)
            {
                switch (ext.ToLower())
                {
                    case ".pdf":
                        type = "Application/pdf";
                        break;
                    case ".doc":
                        type = "Application/msword";
                        break;
                    case ".docx":
                        type = "Application/msword";
                        break;
                    default:
                        type = "Application";
                        break;
                }
            }

            if (System.IO.File.Exists(FullfilePhysicalPath))
            {
                return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
            }
            else
            {
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        #region BLOCKING AND LISTING FOR QUALITY MONITOR
        //by Pradip  29-12-2016  to block Quality Monitor and listing  Start here

        [HttpPost]
        public JsonResult BlockMasterQualityMonitor(String PAN)
        {
            string message = String.Empty;
            try
            {
                objBAL = new MasterBAL();
                bool status = objBAL.BlockeQualityMonitor(PAN);
                if (status == true)
                {
                    return Json(new { status = true, message = "Quality monitor is black listed." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Quality monitor is not black listed." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "Quality monitor is not black listed." }, JsonRequestBehavior.AllowGet);
            }


        }


        public ActionResult ShowBlockedQualityMonitirs(int? page, int? rows, string sidx, string sord)
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
                objBAL = new MasterBAL();
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
                    rows = objBAL.ListBlockedQualityMonitor(qmTypeName, stateCode, districtCode, isEmpanelled, Request.Params["filters"], Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Added by Pradip Patil end 29-12-2016 
        #endregion

        #region Matrix Parameters

        [HttpGet]
        public ActionResult MatrixParametersLayout()
        {
            return View();
        }

        /// <summary>
        /// List matix Param Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        ///
        [HttpPost]
        public ActionResult MatrixParamDetailsList(FormCollection formCollection)
        {
            long totalRecords = 0;
            objBAL = new MasterBAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListMatrixDetails(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
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


        public ActionResult AddEditMatrixDetails()
        {
            MatrixParamModel model = new MatrixParamModel();
            return PartialView("AddEditMatrixDetails", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMatrixParam(MatrixParamModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddMatrixParamDetails(model, ref message))
                    {
                        message = message == string.Empty ? "Parameters details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMatrixDetails", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult DeleteMatrixParamDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int matrixCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                matrixCode = Convert.ToInt32(decryptedParameters["MatrixCode"]);
                objBAL = new MasterBAL();
                bool status = objBAL.DeleteMatrixParamDetails(matrixCode);
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
                //  Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false });
            }
        }


        /// <summary>
        /// Get Matrix Parameters details for update purpose
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult GetMatrixParamDetails(string parameter, string hash, string key)
        {
            Dictionary<string, string> decryptedParameters = null;
            MatrixParamModel model = new MatrixParamModel();
            objBAL = new MasterBAL();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                model = objBAL.GetMatrixParamDetails(Convert.ToInt32(decryptedParameters["MatrixCode"]));
                return PartialView("AddEditMatrixDetails", model);
            }
            catch (Exception ex)
            {
                // Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }


        /// <summary>
        /// Update Parameters details.
        /// </summary>
        /// <param name="carriageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditMatrixDetails(MatrixParamModel model)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMatrixParamDetails(model, ref message))
                    {
                        message = message == string.Empty ? "Parameters details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMatrixDetails", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Changes for RCPLWE Map Scheme for STA

        /*Commented By Hrishikesh 26-07-2023 For Vibrant Village Mapp Scheme
        [HttpGet]
        public ActionResult MapPMGSYScheme(String parameter, String hash, String key)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            MasterDAL objmasterDAL = new MasterDAL();
            int mappedScheme = 0;
            try
            {
                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                Int32 adminTaId = 0;
                string adminTaType = string.Empty;
                Int32? stateCode = 0;
                if (decryptedParameters.Count > 0)
                {
                    adminTaId = Convert.ToInt32(decryptedParameters["AdminId"].Trim());

                    ViewBag.EncryptedAgencyId = parameter + '/' + hash + '/' + key;

                    //ViewBag.AgencyName = decryptedParameters["TAName"].ToString();

                    mappedScheme = objmasterDAL.GetMappedSchemeDAL(adminTaId);

                    switch (mappedScheme)
                    {
                        case 1://PMGSY1
                            ViewBag.MAST_PMGSY1 = true;
                            ViewBag.MAST_PMGSY2 = false;
                            ViewBag.RCPLWE = false;
                            ViewBag.MAST_PMGSY3 = false;
                            break;
                        case 2://PMGSY2
                            ViewBag.MAST_PMGSY1 = false;
                            ViewBag.MAST_PMGSY2 = true;
                            ViewBag.RCPLWE = false;
                            ViewBag.MAST_PMGSY3 = false;
                            break;
                        case 3://RCPLWE
                            ViewBag.MAST_PMGSY1 = false;
                            ViewBag.MAST_PMGSY2 = false;
                            ViewBag.RCPLWE = true;
                            ViewBag.MAST_PMGSY3 = false;
                            break;
                        case 4://PMGSY1 && PMGSY2
                            ViewBag.MAST_PMGSY1 = true;
                            ViewBag.MAST_PMGSY2 = true;
                            ViewBag.RCPLWE = false;
                            ViewBag.MAST_PMGSY3 = false;
                            break;
                        case 5://PMGSY2 && RCPLWE
                            ViewBag.MAST_PMGSY1 = false;
                            ViewBag.MAST_PMGSY2 = true;
                            ViewBag.RCPLWE = true;
                            ViewBag.MAST_PMGSY3 = false;
                            break;
                        case 6://PMGSY1 && RCPLWE
                            ViewBag.MAST_PMGSY1 = true;
                            ViewBag.MAST_PMGSY2 = false;
                            ViewBag.RCPLWE = true;
                            ViewBag.MAST_PMGSY3 = false;
                            break;
                        case 7://PMGSY1 && PMGSY2 && RCPLWE
                            ViewBag.MAST_PMGSY1 = true;
                            ViewBag.MAST_PMGSY2 = true;
                            ViewBag.RCPLWE = true;
                            ViewBag.MAST_PMGSY3 = false;
                            break;
                        case 8://PMGSY3
                            ViewBag.MAST_PMGSY1 = false;
                            ViewBag.MAST_PMGSY2 = false;
                            ViewBag.RCPLWE = false;
                            ViewBag.MAST_PMGSY3 = true;
                            break;
                        case 9://PMGSY1 && PMGSY3
                            ViewBag.MAST_PMGSY1 = true;
                            ViewBag.MAST_PMGSY2 = false;
                            ViewBag.RCPLWE = false;
                            ViewBag.MAST_PMGSY3 = true;
                            break;
                        case 10://PMGSY2 && PMGSY3
                            ViewBag.MAST_PMGSY1 = false;
                            ViewBag.MAST_PMGSY2 = true;
                            ViewBag.RCPLWE = false;
                            ViewBag.MAST_PMGSY3 = true;
                            break;
                        case 11://RCPLWE && PMGSY3
                            ViewBag.MAST_PMGSY1 = false;
                            ViewBag.MAST_PMGSY2 = false;
                            ViewBag.RCPLWE = true;
                            ViewBag.MAST_PMGSY3 = true;
                            break;
                        case 12://PMGSY1 && PMGSY2 && PMGSY3
                            ViewBag.MAST_PMGSY1 = true;
                            ViewBag.MAST_PMGSY2 = true;
                            ViewBag.RCPLWE = false;
                            ViewBag.MAST_PMGSY3 = true;
                            break;
                        case 13://PMGSY1 && RCPLWE && PMGSY3
                            ViewBag.MAST_PMGSY1 = true;
                            ViewBag.MAST_PMGSY2 = false;
                            ViewBag.RCPLWE = true;
                            ViewBag.MAST_PMGSY3 = true;
                            break;
                        case 14://PMGSY1 && PMGSY2 && RCPLWE && PMGSY3
                            ViewBag.MAST_PMGSY1 = true;
                            ViewBag.MAST_PMGSY2 = true;
                            ViewBag.RCPLWE = true;
                            ViewBag.MAST_PMGSY3 = true;
                            break;
                        // you can have any number of case statements 
                        default: // Optional 
                            ViewBag.MAST_PMGSY1 = false;
                            ViewBag.MAST_PMGSY2 = false;
                            ViewBag.RCPLWE = false;
                            ViewBag.MAST_PMGSY3 = false;
                            break;
                    }

                    return PartialView("MapPMGSYScheme");
                }
                return PartialView("MapPMGSYScheme");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MapPMGSYScheme");
                return PartialView("MapPMGSYScheme");
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdatePMGSYScheme(String parameter, String hash, String key)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            MasterDAL objmasterDAL = new MasterDAL();
            bool status = false;
            try
            {
                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                Int32 adminTaId = 0;
                int mastScheme = 0;
                if (decryptedParameters.Count > 0)
                {
                    adminTaId = Convert.ToInt32(decryptedParameters["AdminId"].Trim());
                    mastScheme = Convert.ToInt32(Request.Params["MAST_PMGSY"]);

                    status = objmasterDAL.UpdatePMGSYSchemeDAL(adminTaId, mastScheme);
                }
                return Json(new { success = status, message = "PMGSY Scheme mapped successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MapPMGSYScheme");
                return Json(new { success = status, message = "Error occured while mapping PMGSY Scheme" }, JsonRequestBehavior.AllowGet);
            }
        }*/

        //Added By Hrishikesh For Vibrant Village Mapp Scheme --26-07-2023-- original Method is Commented Above 
        [HttpGet]
        public ActionResult MapPMGSYScheme(String parameter, String hash, String key)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            MasterDAL objmasterDAL = new MasterDAL();
            /*int mappedScheme = 0;*/
            try
            {
                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                Int32 adminTaId = 0;
                string adminTaType = string.Empty;
                Int32? stateCode = 0;
                if (decryptedParameters.Count > 0)
                {
                    adminTaId = Convert.ToInt32(decryptedParameters["AdminId"].Trim());

                    ViewBag.EncryptedAgencyId = parameter + '/' + hash + '/' + key;

                    //ViewBag.AgencyName = decryptedParameters["TAName"].ToString();

                    string[] mappedScheme = objmasterDAL.GetMappedSchemeDAL(adminTaId);

                    bool pmgsy1 = false;
                    bool pmgsy2 = false;
                    bool rcplwe = false;
                    bool pmgsy3 = false;
                    bool vibrantVillage = false;

                    if (mappedScheme.Length > 0)
                    {
                        for (var i = 0; i < mappedScheme.Length; i++)
                        {
                            if (mappedScheme[i] == "1")
                            {
                                pmgsy1 = true;
                            }
                            if (mappedScheme[i] == "2")
                            {
                                pmgsy2 = true;
                            }
                            if (mappedScheme[i] == "3")
                            {
                                rcplwe = true;
                            }
                            if (mappedScheme[i] == "4")
                            {
                                pmgsy3 = true;
                            }
                            if (mappedScheme[i] == "5")
                            {
                                vibrantVillage = true;
                            }
                        }

                        ViewBag.MAST_PMGSY1 = pmgsy1;
                        ViewBag.MAST_PMGSY2 = pmgsy2;
                        ViewBag.RCPLWE = rcplwe;
                        ViewBag.MAST_PMGSY3 = pmgsy3;
                        ViewBag.VibrantV = vibrantVillage;
                    }
                    else
                    {
                        ViewBag.MAST_PMGSY1 = false;
                        ViewBag.MAST_PMGSY2 = false;
                        ViewBag.RCPLWE = false;
                        ViewBag.MAST_PMGSY3 = false;
                        ViewBag.VibrantV = false;
                    }


                    return PartialView("MapPMGSYScheme");
                }
                return PartialView("MapPMGSYScheme");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MapPMGSYScheme");
                return PartialView("MapPMGSYScheme");
            }
        }

        //Added By Hrishikesh For Vibrant Village Mapp Scheme --26-07-2023-- original Method is Commented Above 
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdatePMGSYScheme(String parameter, String hash, String key)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            MasterDAL objmasterDAL = new MasterDAL();
            bool status = false;
            try
            {
                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                Int32 adminTaId = 0;
                //int mastScheme = 0;
                if (decryptedParameters.Count > 0)
                {
                    adminTaId = Convert.ToInt32(decryptedParameters["AdminId"].Trim());
                    //mastScheme = Convert.ToInt32(Request.Params["MAST_PMGSY"]);
                    var strMastScheme = Request.Params["MAST_PMGSY"].ToString();


                    //status = objmasterDAL.UpdatePMGSYSchemeDAL(adminTaId, mastScheme);
                    status = objmasterDAL.UpdatePMGSYSchemeDAL(adminTaId, strMastScheme);

                }
                return Json(new { success = status, message = "PMGSY Scheme mapped successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdatePMGSYScheme");
                return Json(new { success = status, message = "Error occured while mapping PMGSY Scheme" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region PMGSY States

        [HttpGet]
        public ActionResult ListPMGSYStates()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.ListPMGSYStates");
                return null;
            }
        }

        [HttpGet]
        public ActionResult AddPmgsyStates()
        {
            try
            {
                PMGSYStatesViewModel model = new PMGSYStatesViewModel();
                CommonFunctions comm = new CommonFunctions();
                model.stateslist = comm.PopulateStateDetails();
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.AddPmgsyStates");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddState(PMGSYStatesViewModel masterAgencyViewModel)
        {
            bool status = false;
            try
            {
                objBAL = new MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddStateBAL(masterAgencyViewModel, ref message))
                    {
                        message = message == string.Empty ? "State details saved successfully." : message;
                        status = true;
                    }
                    else
                    {

                        message = message == string.Empty ? "State details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddPmgsyStates", masterAgencyViewModel);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.AddState");
                return null;
            }
        }


        [HttpGet]
        public ActionResult GetPmgsyStateDetails(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;
                string status = string.Empty;
                objBAL = new MasterBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListPmgsyStates(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.GetPmgsyStateDetails");
                return null;
            }
        }

        #endregion



        #region Financial Year Target
        /// <summary>
        /// ListFinancialYearTarget() Actions Shows Filter Bar and Financial Target List Grid
        /// </summary> 
        /// <returns></returns>
        [Audit]
        public ActionResult ListFinancialYearTarget()
        {
            try
            {
                //IExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                CommonFunctions objCommon = new CommonFunctions();
                ViewData["States"] = objCommon.PopulateStates(true);
                ViewData["Years"] = objCommon.PopulateFinancialYears(true, false);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "ListFinancialYearTarget()");
            }
            return View();
        }
        [Audit]
        public ActionResult ValidationOnAddForm(int state, int year)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                bool result = dbContext.TARGET_PMGSY_SCHEME.Where(x => x.MAST_STATE_CODE == state && x.TARGET_YEAR == year).Any();
                if (result)
                {
                    return Json(new { success = false, message = "Existing Financial Year Target for state and year is already exist." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = "Financial Year Target for state and year does not exist." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        /// <summary>
        /// AddEditFinancialYearTarget() Actions shows Existing Road Data entry form
        /// </summary>
        /// <returns> Returns partial view Of Financial Year Target Data Entry Form </returns>
        [Audit]
        public ActionResult AddEditFinancialYearTarget(string state, int year)
        {


            FinancialYearTargetViewModel FinancialYearTargetViewModel = new FinancialYearTargetViewModel();
            try
            {

                FinancialYearTargetViewModel.stateName = state; //> 0 ? stateCode : PMGSYSession.Current.StateCode;
                FinancialYearTargetViewModel.year = year; //stateCode > 0 ? districtCode : PMGSYSession.Current.DistrictCode;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                ModelState.AddModelError("", ex.Message);
                return PartialView("AddEditFinancialYearTarget", new FinancialYearTargetViewModel());
            }
            return PartialView("AddEditFinancialYearTarget", FinancialYearTargetViewModel);
        }

        /// <summary>
        /// AddFinancialTarget() this actions displays Existing Road
        /// </summary>
        /// <returns>return json data to display on grid</returns>
        /// 
        [HttpPost]
        [Audit]
        public ActionResult AddFinancialTarget(IEnumerable<FinancialYearTargetViewModel> data)
        {
            bool status = false;
            try
            {
                MasterDAL objERDAL = new MasterDAL();

                if (objERDAL.AddFinancialTargetdata(data, ref message))
                {
                    message = message == string.Empty ? "Financial Year Data details saved successfully." : message;
                    status = true;
                }
                else
                {
                    message = message == string.Empty ? "Financial Year Data details not saved successfully." : message;
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
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
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Financial Year Target details not saved successfully";
                message = message + ex.Message;
                return Json(new { success = status, message = "There is an error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [Audit]
        public ActionResult GetFinancialYearTargetList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords; int year = 0; int state = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["state"]))
                {
                    state = Convert.ToInt32(Request.Params["state"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["year"]))
                {
                    year = Convert.ToInt32(Request.Params["year"]);

                }
                // state = 3;
                MasterBAL objBAL = new MasterBAL();


                var jsonData = new
                {
                    rows = objBAL.ListFinancialYearTarget(state, year, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
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

        [HttpPost]
        [Audit]
        public ActionResult DeleteFinancialYearTarget(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            int pmgsyId = 0;
            MasterBAL objBAL = new MasterBAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                //int pmgsyId = Convert.ToInt32(Request.Params["id"]);

                if (decryptedParameters.Count > 0)
                    pmgsyId = Convert.ToInt32(decryptedParameters["pmgsyId"]);
                {
                    bool result = objBAL.DeleteFinancialYearTarget(pmgsyId, ref message);

                    if (result)
                    {
                        message = "Financial Year Target details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Financial Year Target details can not be deleted" : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion



        #region Interstate Monitor Mapping

        [HttpGet]
        public ActionResult MasterQualityMonitorForMapping()
        {
            DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                InterStateMapping model = new InterStateMapping();

                CommonFunctions commonFunctions = new CommonFunctions();
                List<SelectListItem> lstState;

                lstState = commonFunctions.PopulateStates(true);


                model.StateName = PMGSYSession.Current.StateName;
                model.MAST_STATE_CODE = PMGSYSession.Current.StateCode;

                string stCode = Convert.ToString(model.MAST_STATE_CODE);

                model.STATES = lstState.Where(m => m.Value != stCode).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MasterController().MasterQualityMonitorForMapping");

                return null;
            }

        }



        [HttpPost]
        [Audit]
        public ActionResult ApproveInterState(string id)
        {
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            int EmargID = 0;
            string progressType = string.Empty;
            try
            {
                int ADMIN_QM_CODE_MAP_ID = Convert.ToInt32(id);
                MasterDAL dal = new MasterDAL();

                if ((dal.ApproveInterStateDAL(ADMIN_QM_CODE_MAP_ID, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = true, message = "Details approved successfully." });
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeApproveInterState(string id)
        {
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            int EmargID = 0;
            string progressType = string.Empty;
            try
            {
                int ADMIN_QM_CODE_MAP_ID = Convert.ToInt32(id);
                MasterDAL dal = new MasterDAL();

                if ((dal.DeApproveInterStateDAL(ADMIN_QM_CODE_MAP_ID, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = true, message = "Details deapproved successfully." });
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }




        [HttpPost]
        [Audit]
        public ActionResult MapInterState()
        {
            bool status = false;
            try
            {
                MasterDAL objERDAL = new MasterDAL();

                //string monitorIdsFinal = formCollection["monitorIds"];
                //string stetCodeFinal = formCollection["stateCode"];

                var monitorIdsFinal = Convert.ToString(Request.Params["monitorIds"]);
                var stetCodeFinal = Convert.ToString(Request.Params["stateCode"]);

                if (objERDAL.MapInterStateDAL(monitorIdsFinal, stetCodeFinal, ref message))
                {
                    message = message == string.Empty ? "Monitor and State Mapping is done successfully." : message;
                    status = true;
                }
                else
                {
                    message = message == string.Empty ? "Monitor and State Mapping is not done." : message;
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
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
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Error : Monitor and State Mapping is not done.";
                message = message + ex.Message;
                return Json(new { success = status, message = "Error :: Monitor and State Mapping is not done." }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult FinalListAfterMapping(FormCollection formCollection)
        {
            var JsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            JsonSerializer.MaxJsonLength = Int32.MaxValue;
            try
            {
                if (PMGSYSession.Current == null)
                {
                    Response.Redirect("/Login/Login");
                }

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                //   Int32 StateCode = Convert.ToInt32(formCollection["IMS_STATE"]);
                //    string DistListFinal = formCollection["DistListID"];

                int totalRecords = 0;

                MasterDAL dal = new MasterDAL();
                var jsonData = new
                {
                    rows = dal.FinalListAfterMappingDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenananceAgreementController().GetEmargFinalList()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeleteMapInterStateDetails(string id)
        {
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                int ADMIN_QM_CODE_MAP_ID = Convert.ToInt32(id);
                MasterDAL dal = new MasterDAL();

                if ((dal.DeleteMapInterStateDetailsDAL(ADMIN_QM_CODE_MAP_ID, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = true, message = "Details deleted successfully." });
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }
        #endregion

    }

    public class SearchJson
    {
        public string groupOp { get; set; }
        public List<rules> rules { get; set; }
    }

    public class rules
    {
        public string field { get; set; }
        public string op { get; set; }
        public string data { get; set; }
    }
}

