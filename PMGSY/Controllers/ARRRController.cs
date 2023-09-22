using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.ARRR;
using PMGSY.Extensions;
using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Controllers;
using System.IO;
using System.Configuration;
using PMGSY.BAL.ARRR;
using System.Transactions;
using PMGSY.DAL.ARRR;
using System.Text.RegularExpressions;

namespace PMGSY.Controllers
{
    public class ARRRController : Controller
    {
        private IARRRDAL objDAL = null;
        private IARRRBAL objBAL = null;
        //
        // GET: /ARRR/

        PMGSYEntities dbContext;
        Dictionary<string, string> decryptedParameters = null;

        #region Chapter Items

        public ActionResult ItemDetails(FormCollection frmCollection)
        {
            //CommonFunctions objCommonFunctions = new CommonFunctions();
            IARRRBAL objIARRRBAL = new ARRRBAL();
            List<SelectListItem> list = objIARRRBAL.PopulateItemsListBAL(false, Convert.ToInt32(frmCollection["headCode"]));
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MajorItemDetails(FormCollection frmCollection)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();
            //List<SelectListItem> list = objIARRRBAL.PopulateMajorItemsListBAL(false, Convert.ToInt32(frmCollection["ItemCode"]));
            List<SelectListItem> list = objIARRRBAL.PopulateMajorItemsListItemwiseBAL(false, Convert.ToInt32(frmCollection["ItemCode"]));
            //List<SelectListItem> list = objIARRRBAL.PopulateMajorItemsListBAL(false, Convert.ToInt32(frmCollection["headCode"]));
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadChapterItems(string parameter, string hash, string key)
        {
            ChapterItemsViewModel model = new ChapterItemsViewModel();
            return View(model);
        }

        //[HttpGet]
        public ActionResult ChapterItemsLayout(string parameter, string hash, string key)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();

            ChapterItemsViewModel model = new ChapterItemsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.ItemActiveList = new List<SelectListItem>();
                model.ItemActiveList.Insert(0, (new SelectListItem { Text = "Yes", Value = "Y" }));
                model.ItemActiveList.Insert(0, (new SelectListItem { Text = "No", Value = "N" }));

                if (Request.Params["User_Action"] == null)
                {
                    model.ItemType = "I";
                    model.Item = 1;
                    model.ChapterList = objIARRRBAL.PopulateChapterListBAL(false);

                    model.ItemList = new List<SelectListItem>();
                    model.ItemList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                    model.MajorItemList = new List<SelectListItem>();
                    model.MajorItemList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                    model.MinorItemList = new List<SelectListItem>();
                    model.MinorItemList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                    model.ItemActive = "Y";
                    model.Parent = 0;
                }
                else
                {
                    model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                    if (parameter != null && hash != null && key != null)
                    {
                        model.EncryptedItemCode = parameter.Trim() + "/" + hash.Trim() + "/" + key.Trim();
                        decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    }

                    if (decryptedParameters != null)
                    {
                        dbContext = new PMGSYEntities();

                        model.ItemCode = Convert.ToInt32(decryptedParameters["ItemCode"]);
                        model.Parent = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MAST_ITEM_PARENT).FirstOrDefault();
                        model.Item = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MAST_ITEM).FirstOrDefault();

                        model.ItemName = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MAST_ITEM_NAME).FirstOrDefault();
                        model.ItemActive = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MAST_ITEM_ACTIVE).FirstOrDefault();

                        model.MinorItem = (Int32)(dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MAST_MINOR_SUBITEM_CODE).FirstOrDefault());
                        model.MajorItem = (Int32)(dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MAST_MAJOR_SUBITEM_CODE).FirstOrDefault());

                        model.ItemType = model.Parent == 0 ? "I" : (model.MajorItem > 0 && model.MinorItem == 0) ? "M" : (model.MajorItem > 0 && model.MinorItem > 0) ? "N" : "";
                        model.hdnItemType = model.ItemType;

                        model.ItemActive = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MAST_ITEM_ACTIVE).FirstOrDefault();

                        model.ChapterList = objIARRRBAL.PopulateChapterListBAL(false);
                        model.Chapter = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MAST_HEAD_CODE).FirstOrDefault();

                        if (model.hdnItemType == "I")
                        {
                            model.ItemList = new List<SelectListItem>();
                            model.ItemList.Insert(0, (new SelectListItem { Text = "All", Value = Convert.ToString(model.Parent) }));

                            model.MajorItemList = new List<SelectListItem>();
                            model.MajorItemList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
                        }
                        else
                        {
                            model.ItemList = objIARRRBAL.PopulateItemsListBAL(false, model.Chapter);


                            if (model.hdnItemType == "N")
                            {
                                model.MajorItemList = objIARRRBAL.PopulateMajorItemsListBAL(false, model.MajorItem);
                                //model.MajorItem = model.Parent;

                                model.Item = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.MajorItem).Select(m => m.MAST_ITEM_PARENT).FirstOrDefault(); ;//model.Chapter;
                            }
                            else if (model.hdnItemType == "M")
                            {
                                model.MajorItemList = new List<SelectListItem>();
                                model.MajorItemList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                                model.Item = model.Parent;
                            }
                        }
                        model.ItemDesc = Convert.ToString(dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MAST_ITEM_DESC).FirstOrDefault());
                        model.hdnMajorItem = model.MajorItem;

                        model.itemUserCode = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MAST_ITEM_USER_CODE).FirstOrDefault();
                        model.mordRef = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MAST_MORD_REF).FirstOrDefault();
                    }
                }
                return View(model);
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
        public ActionResult GetChapterItemsList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                IARRRBAL objBAL = new ARRRBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListChapterItemsBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        //[HttpPost]
        public ActionResult AddEditChapterItemDetails(ChapterItemsViewModel model)
        {
            bool status = false;
            string message = "";

            IARRRBAL objBAL = new ARRRBAL();
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                if (ModelState.IsValid)
                {
                    if (model.User_Action == "A")
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.addARRRChaptersBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Item details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Item details not saved" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Item details not saved" : message });
                            }
                        }
                    }
                    else
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.editARRRChaptersBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "ARRR Master details edited successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "ARRR Master details not edit" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "ARRR Master details not edit" : message });
                            }
                        }
                    }
                    //return Json(new { success = status, message = message == string.Empty ? "ARRR Master details not saved" : message });
                }
                else
                {
                    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {

            }
        }

        [HttpPost]
        public ActionResult changeChapterstatus(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int ItemCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    ItemCode = Convert.ToInt32(decryptedParameters["ItemCode"]);

                    if (objBAL.changeChapterstatusBAL(ItemCode))
                    {

                        status = true;

                        message = message == string.Empty ? "Chapter status changed successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "Chapter status not changed" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not change Chapter status.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not change Chapter status.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not change Chapter status.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult delChapterDetails(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int ItemCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    ItemCode = Convert.ToInt32(decryptedParameters["ItemCode"]);

                    if (objBAL.deleteARRRChaptersBAL(ItemCode))
                    {

                        status = true;

                        message = message == string.Empty ? "ARRR Master details deleted successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "ARRR Master details not deleted" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not delete this ARRR Master  details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not delete this ARRR Master  details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this ARRR Master details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Machinery Master
        public ActionResult LoadMachineryMaster(string parameter, string hash, string key)
        {
            MachineryMasterViewModel model = new MachineryMasterViewModel();
            return View(model);
        }

        //[HttpGet]
        public ActionResult MachineryMasterLayout(string parameter, string hash, string key)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();

            MachineryMasterViewModel model = new MachineryMasterViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.CategoryList = objIARRRBAL.PopulateCategoryBAL(false, 2);
                if (Request.Params["User_Action"] == null)
                {
                    model.flag = "Y";

                    model.lmmType = 2;
                    model.lmmActyCode = 0;

                    model.OutputUnitList = objIARRRBAL.PopulateOutputUnitBAL();
                    model.UsageUnitList = objIARRRBAL.PopulateUsageUnitBAL();

                }
                else
                {
                    model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                    if (parameter != null && hash != null && key != null)
                    {
                        model.EncryptedlmmCode = parameter.Trim() + "/" + hash.Trim() + "/" + key.Trim();
                        decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    }

                    if (decryptedParameters != null)
                    {
                        dbContext = new PMGSYEntities();

                        model.lmmCode = Convert.ToInt32(decryptedParameters["lmmCode"]);

                        model.flag = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == model.lmmCode).Select(m => m.MAST_LMM_ACTIVE).FirstOrDefault();

                        model.lmmType = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == model.lmmCode).Select(m => m.MAST_LMM_TYPE).FirstOrDefault();
                        model.lmmActyCode = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == model.lmmCode).Select(m => m.MAST_LMM_ACTY_CODE).FirstOrDefault();

                        model.OutputUnitList = objIARRRBAL.PopulateOutputUnitBAL();
                        model.UsageUnitList = objIARRRBAL.PopulateUsageUnitBAL();

                        model.OutputUnit = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == model.lmmCode).Select(m => m.MAST_LMM_OUTPUT_UNIT).FirstOrDefault();
                        model.UsageUnit = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == model.lmmCode).Select(m => m.MAST_LMM_USAGE_UNIT).FirstOrDefault();

                        model.Activity = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == model.lmmCode).Select(m => m.MAST_LMM_ACTIVITY).FirstOrDefault();
                        model.Description = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == model.lmmCode).Select(m => m.MAST_LMM_DESC).FirstOrDefault();
                        model.OutputRate = (decimal)dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == model.lmmCode).Select(m => m.MAST_LMM_OUTPUT_RATE).FirstOrDefault();

                        model.lCode = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == model.lmmCode).Select(m => m.MAST_LMM_CODE).FirstOrDefault();

                        model.Category = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == model.lmmCode).Select(m => m.MAST_LMM_CAT_CODE).FirstOrDefault();
                        //string ctg = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == model.lmmCode).Select(m => m.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_CATEGORY).FirstOrDefault();
                    }
                }
                return View(model);
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
        public ActionResult GetMachineryList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                IARRRBAL objBAL = new ARRRBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListMachineryMasterBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        public ActionResult AddEditMachineryMasterDetails(MachineryMasterViewModel model)
        {
            bool status = false;
            string message = "";

            IARRRBAL objBAL = new ARRRBAL();
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                if (ModelState.IsValid)
                {
                    if (model.User_Action == "A")
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.addARRRMachineryBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Machinery Master details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Machinery Master details not saved" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Machinery Master details not saved" : message });
                            }
                        }
                    }
                    else
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.editARRRMachineryBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Machinery Master details edited successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Machinery Master details not edit" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Machinery Master details not edit" : message });
                            }
                        }
                    }
                    //return Json(new { success = status, message = message == string.Empty ? "ARRR Master details not saved" : message });
                }
                else
                {
                    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {

            }
        }

        [HttpPost]
        public ActionResult changeMachineryMasterstatus(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int lmmCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    lmmCode = Convert.ToInt32(decryptedParameters["lmmCode"]);

                    if (objBAL.changeMachineryMasterstatusBAL(lmmCode))
                    {

                        status = true;

                        message = message == string.Empty ? "Machinery Master status changed successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "Machinery Master status not changed" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not change Machinery Master status.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not change Machinery Master status.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not change Chapter details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult delMaterialMasterDetails(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int lmmCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    lmmCode = Convert.ToInt32(decryptedParameters["lmmCode"]);

                    if (objBAL.deleteARRRMachineryBAL(lmmCode))
                    {

                        status = true;

                        message = message == string.Empty ? "Machinery Master details deleted successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "Machinery Master details not deleted" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not delete this Machinery Master details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not delete this Master  details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Master details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Material Rates
        public ActionResult MaterialRateDetails(FormCollection frmCollection)
        {
            //CommonFunctions objCommonFunctions = new CommonFunctions();
            IARRRBAL objIARRRBAL = new ARRRBAL();
            List<SelectListItem> list = objIARRRBAL.PopulateLMMItemsListBAL(false, 3, Convert.ToInt32(frmCollection["category"]));
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public ActionResult LoadMaterialRate()
        {
            LMMRateViewModel model = new LMMRateViewModel();
            ProposalController pc = new ProposalController();
            model.yearList = pc.PopulateYear(0, true, false);
            model.yearList.Find(x => x.Value == "0").Value = "-1";
            return View(model);
        }

        //[HttpGet]
        public ActionResult MaterialRateLayout(string parameter, string hash, string key)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();

            LMMRateViewModel model = new LMMRateViewModel();
            CommonFunctions comm = new CommonFunctions();
            ProposalController pc = new ProposalController();
            try
            {
                dbContext = new PMGSYEntities();

                model.rateType = 3;
                //model.ItemList = objIARRRBAL.PopulateLMMItemsListBAL(false, model.rateType);

                model.ItemList = new List<SelectListItem>();
                model.ItemList.Insert(0, new SelectListItem { Text = "Select", Value = "-1" });

                model.CategoryList = objIARRRBAL.PopulateCategoryBAL(false, 3);
                model.yearList = pc.PopulateYear(0, true, false);
                //model.yearList = comm.PopulateYears(true);
                model.yearList.Find(x => x.Value == "0").Value = "-1";

                if (Request.Params["User_Action"] == null)
                {
                    model.flag = "Y";
                    model.stateCode = PMGSYSession.Current.StateCode;

                    model.status = "N";
                }
                else
                {
                    model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                    if (parameter != null && hash != null && key != null)
                    {
                        model.EncryptedRateCode = parameter.Trim() + "/" + hash.Trim() + "/" + key.Trim();
                        decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    }

                    if (decryptedParameters != null)
                    {


                        model.rateCode = Convert.ToInt32(decryptedParameters["rateCode"]);

                        model.Item = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_LMM_TYPE_CODE).FirstOrDefault();
                        model.hdnItem = model.Item;
                        //string itmName = "";
                        //model.ItemList = new List<SelectListItem>();

                        //itmName = dbContext.MASTER_ARR_LMM_TYPES.Where(m => m.MAST_LMM_ACTY_CODE == model.Item).Select(m => m.MAST_LMM_DESC).FirstOrDefault();
                        //model.ItemList.Insert(0, new SelectListItem { Text = itmName.Trim(), Value = model.Item.ToString() });

                        model.flag = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.ACTIVE_FLAG).FirstOrDefault();

                        model.rateType = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_LMM_TYPE).FirstOrDefault();
                        model.status = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_RATE_IS_FINAL).FirstOrDefault();
                        model.stateCode = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                        //model.OutputUnitList = objIARRRBAL.PopulateOutputUnitBAL();

                        //model.OutputUnit = dbContext.MASTER_ARR_LMM_TYPES.Where(m => m.MAST_LMM_CODE == model.rateCode).Select(m => m.MAST_LMM_OUTPUT_UNIT).FirstOrDefault();

                        //model.Activity = dbContext.MASTER_ARR_LMM_TYPES.Where(m => m.MAST_LMM_CODE == model.rateCode).Select(m => m.MAST_LMM_ACTIVITY).FirstOrDefault();
                        //model.Description = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_LMM_DESC).FirstOrDefault();
                        model.Rate = (decimal)dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_RATE).FirstOrDefault();
                        //model.Date = Convert.ToDateTime(dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_EFFECTIVE_FROM).FirstOrDefault()).ToString("dd/MM/yyyy");
                        //model.TillDate = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_EFFECTIVE_TILL).FirstOrDefault() == null ? "" : Convert.ToDateTime(dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_EFFECTIVE_TILL).FirstOrDefault()).ToString("dd/MM/yyyy");
                        //model.Category = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MASTER_ARRR_LMM_TYPES.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_CAT_CODE).FirstOrDefault();
                        model.Year = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_YEAR).FirstOrDefault();

                        model.Category = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MASTER_ARRR_LMM_TYPES.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_CAT_CODE).FirstOrDefault();
                    }
                }
                return View(model);
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
        public ActionResult GetMaterialRateList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                IARRRBAL objBAL = new ARRRBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                int Year = String.IsNullOrEmpty(Request.Params["year"]) ? 0 : Convert.ToInt32(Request.Params["year"]);
                var jsonData = new
                {
                    rows = objBAL.ListMaterialRatesBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, Year, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }






        public ActionResult AddEditMaterialRateDetails(LMMRateViewModel model)
        {
            bool status = false;
            string message = "";

            IARRRBAL objBAL = new ARRRBAL();
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                if (ModelState.IsValid)
                {
                    if (model.User_Action == "A")
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.addARRRMaterialRatesBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Material Rate details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Material Rate details not saved" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Material Rate details not saved" : message });
                            }
                        }
                    }
                    else
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.editARRRMaterialRatesBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Material Rate details edited successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Material Rate details not edit" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Material Rate details not edit" : message });
                            }
                        }
                    }
                    //return Json(new { success = status, message = message == string.Empty ? "ARRR Master details not saved" : message });
                }
                else
                {
                    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {

            }
        }

        [HttpPost]
        public ActionResult changeMaterialRatestatus(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    rateCode = Convert.ToInt32(decryptedParameters["rateCode"]);

                    if (objBAL.changeLMMRatestatusBAL(rateCode))
                    {

                        status = true;

                        message = message == string.Empty ? "Material Rates status changed successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "Material Rates status not changed" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not change Material Rates status.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not change Material Rates status.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not change Material Rates status.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult delMaterialRateDetails(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            //MASTER_ARR_ITEMS_MASTER model = new MASTER_ARR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    rateCode = Convert.ToInt32(decryptedParameters["rateCode"]);

                    if (objBAL.deleteARRRMaterialRatesBAL(rateCode))
                    {

                        status = true;

                        message = message == string.Empty ? "ARRR Material Rate details deleted successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "ARRR Material Rate details not deleted" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not delete this ARRR Material Rate details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not delete this ARRR Material Rate details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this ARRR Material Rate details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult MaterialRateFinalization(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    rateCode = Convert.ToInt32(decryptedParameters["rateCode"]);

                    if (objBAL.MaterialRateFinalizeBAL(rateCode))
                    {
                        status = true;

                        message = message == string.Empty ? "ARRR Material Rate details finalized successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "ARRR Material Rate details not finalized" : message });
                    }
                    else
                    {
                        message = "You can not finalize this ARRR Material Rate details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not finalize this ARRR Material Rate details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not finalize this ARRR Material Rate details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GetMaterialFormList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                IARRRBAL objBAL = new ARRRBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListMaterialFormBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        [HttpPost]
        public JsonResult AddMultipleMaterialData(IEnumerable<LabourDetails> materialData)
        {
            bool status = false;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            try
            {
                //ModelState["Category"].Errors.Clear();
                //if (ModelState.IsValidField("Rate"))
                //{
                using (TransactionScope objScope = new TransactionScope())
                {
                    if (objBAL.addBulkARRRMaterialRatesBAL(materialData, ref message))
                    {
                        objScope.Complete();
                        status = true;
                        return Json(new { success = status, message = message == string.Empty ? "All Material details added successfully." : message });
                    }
                    else
                    {
                        return Json(new { success = status, message = message == string.Empty ? "Material Rate details not saved" : message });
                    }
                }
                //}
                //else
                //{
                //    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                //    return Json(new { success = status, message = msg == string.Empty ? "Labour Rate details not saved" : msg});
                //}
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {
            }
        }

        [HttpPost]
        public ActionResult FinalizeAllMaterialRates(string id)
        {
            dbContext = new PMGSYEntities();
            int year = 0;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            try
            {
                year = Convert.ToInt32(id.Trim());
                if (year > 0)
                {
                    if (objBAL.FinalizeAllMaterialRatesBAL(year))
                    {
                        status = true;

                        message = message == string.Empty ? "ARRR Material Rate details finalized successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "ARRR Material Rate details not finalized" : message });
                    }
                    else
                    {
                        message = "No records found to Finalize";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "Invalid Year ! ARRR Material Rate details not finalized.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Can not finalize ARRR Material Rate details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveMaterialPDFfile()
        {
            CommonFunctions comm = new CommonFunctions();
            string filePath = string.Empty;
            bool isFileSaved = false;
            HttpPostedFileBase FileBase = null;
            bool status = false;
            try
            {
                if (Request.Files.Count <= 0)
                {
                    return Json(new { success = false, message = "Please select a pdf file" });
                }
                IARRRDAL objDAL = new ARRRDAL();

                #region Multiple File
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        FileBase = Request.Files[i];
                        var filename = FileBase.FileName;

                        status = objDAL.SaveARRRMaterialFile(filename, FileBase, out isFileSaved);

                        if (status == false && isFileSaved == false)
                        {
                            break;
                        }
                    }
                    if (status == false && isFileSaved == false)
                    {
                        return Json(new { success = false, message = "Error in Saving uploaded file : " + FileBase.FileName });
                    }
                }
                #endregion
                return Json(new { success = true, message = "Upload Successful" });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.SaveMaterialPDFfile()");
                return Json(new { success = false, message = "Error occured while uploading file" });
            }
        }

        [HttpGet]
        public ActionResult DownloadMaterialPdfFile(String parameter, String hash, String key)
        {
            try
            {
                dbContext = new PMGSYEntities();
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

                var record = (from arrr in dbContext.MASTER_ARRR_STATES_RATES
                              where arrr.MAST_ARRR_RATE_CODE == obsId
                              select arrr).First();

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                if (!String.IsNullOrEmpty(record.MAST_ARRR_FILENAME))
                {
                    VirtualDirectoryUrl = ConfigurationManager.AppSettings["MaterialRateFilePath_Virtual_Dir_Path"];
                    PhysicalPath = ConfigurationManager.AppSettings["MaterialRateFilePath"];
                }
                else
                {
                    VirtualDirectoryUrl = null;
                    PhysicalPath = null;
                }

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
        }
        #endregion

        #region Labour Rates
        public ActionResult LabourRateDetails(FormCollection frmCollection)
        {
            //CommonFunctions objCommonFunctions = new CommonFunctions();
            IARRRBAL objIARRRBAL = new ARRRBAL();
            List<SelectListItem> list = objIARRRBAL.PopulateLMMItemsListBAL(false, 1, Convert.ToInt32(frmCollection["category"]));
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }



        public ActionResult LoadLabourRate()
        {
            LMMRateViewModel model = new LMMRateViewModel();
            CommonFunctions comm = new CommonFunctions();
            ProposalController pc = new ProposalController();
            model.yearList = pc.PopulateYear(0, true, false);
            //model.yearList = comm.PopulateYears(true);
            model.yearList.Find(x => x.Value == "0").Value = "-1";
            return View(model);
        }

        //[HttpGet]
        public ActionResult LabourRateLayout(string parameter, string hash, string key)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();

            LMMRateViewModel model = new LMMRateViewModel();
            CommonFunctions comm = new CommonFunctions();
            ProposalController pc = new ProposalController();
            try
            {
                model.rateType = 1;
                //model.ItemList = objIARRRBAL.PopulateLMMItemsListBAL(false, model.rateType);
                model.ItemList = new List<SelectListItem>();
                model.ItemList.Insert(0, new SelectListItem { Text = "Select", Value = "-1" });

                model.CategoryList = objIARRRBAL.PopulateCategoryBAL(false, 1);

                // model.yearList = comm.PopulateYears(true);               
                model.yearList = pc.PopulateYear(0, true, false);
                model.yearList.Find(x => x.Value == "0").Value = "-1";

                if (Request.Params["User_Action"] == null)
                {
                    model.flag = "Y";
                    //model.ItemList.RemoveAt(0);
                    model.stateCode = PMGSYSession.Current.StateCode;

                    model.status = "N";
                }
                else
                {
                    model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                    if (parameter != null && hash != null && key != null)
                    {
                        model.EncryptedRateCode = parameter.Trim() + "/" + hash.Trim() + "/" + key.Trim();
                        decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    }

                    if (decryptedParameters != null)
                    {
                        dbContext = new PMGSYEntities();

                        model.rateCode = Convert.ToInt32(decryptedParameters["rateCode"]);

                        model.Item = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_LMM_TYPE_CODE).FirstOrDefault();
                        model.hdnItem = model.Item;
                        //string itmName = "";
                        //model.ItemList = new List<SelectListItem>();
                        //itmName = dbContext.MASTER_ARR_LMM_TYPES.Where(m => m.MAST_LMM_ACTY_CODE == model.Item).Select(m => m.MAST_LMM_DESC).FirstOrDefault();
                        //model.ItemList.Insert(0, new SelectListItem { Text = itmName.Trim(), Value = model.Item.ToString() });

                        model.Category = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MASTER_ARRR_LMM_TYPES.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_CAT_CODE).FirstOrDefault();
                        model.hdnCategory = model.Category;

                        model.flag = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.ACTIVE_FLAG).FirstOrDefault();

                        model.rateType = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_LMM_TYPE).FirstOrDefault();
                        model.status = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_RATE_IS_FINAL).FirstOrDefault();
                        model.stateCode = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();

                        model.Rate = (decimal)dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_RATE).FirstOrDefault();
                        //model.Date = Convert.ToDateTime(dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_EFFECTIVE_FROM).FirstOrDefault()).ToString("dd/MM/yyyy");
                        //model.TillDate = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_EFFECTIVE_TILL).FirstOrDefault() == null ? "" : Convert.ToDateTime(dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_EFFECTIVE_TILL).FirstOrDefault()).ToString("dd/MM/yyyy");
                        model.Year = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_YEAR).FirstOrDefault();
                    }
                }
                return View(model);
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
        public ActionResult GetLabourRateList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                IARRRBAL objBAL = new ARRRBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                int Year = String.IsNullOrEmpty(Request.Params["year"]) ? 0 : Convert.ToInt32(Request.Params["year"]);
                var jsonData = new
                {
                    rows = objBAL.ListLabourRatesBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, Year, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }


        public ActionResult AddEditLabourRateDetails(LMMRateViewModel model)
        {
            bool status = false;
            string message = "";

            IARRRBAL objBAL = new ARRRBAL();
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                if (ModelState.IsValid)
                {
                    if (model.User_Action == "A")
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.addARRRLabourRatesBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Labour Rate details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Labour Rate details not saved" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Labour Rate details not saved" : message });
                            }
                        }
                    }
                    else
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.editARRRLabourRatesBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Labour Rate details edited successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Labour Rate details not edit" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Labour Rate details not edit" : message });
                            }
                        }
                    }
                    //return Json(new { success = status, message = message == string.Empty ? "ARRR Master details not saved" : message });
                }
                else
                {
                    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { success = status, message = message == string.Empty ? "Labour Rate details not edit" : message });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {

            }
        }

        [HttpPost]
        public ActionResult changeLabourRatestatus(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    rateCode = Convert.ToInt32(decryptedParameters["rateCode"]);

                    if (objBAL.changeLMMRatestatusBAL(rateCode))
                    {

                        status = true;

                        message = message == string.Empty ? "Labour Rates status changed successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "Labour Rates status not changed" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not change Labour Rates status.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not change Labour Rates status.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not change Labour Rates status.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult delLabourRateDetails(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            //MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    rateCode = Convert.ToInt32(decryptedParameters["rateCode"]);

                    if (objBAL.deleteARRRLabourRatesBAL(rateCode))
                    {

                        status = true;

                        message = message == string.Empty ? "ARRR Labour Rate details deleted successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "ARRR Labour Rate details not deleted" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not delete this ARRR Labour Rate details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not delete this ARRR Labour Rate details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this ARRR Labour Rate details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult LabourRateFinalization(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    rateCode = Convert.ToInt32(decryptedParameters["rateCode"]);

                    if (objBAL.LabourRateFinalizeBAL(rateCode))
                    {
                        status = true;

                        message = message == string.Empty ? "ARRR Labour Rate details finalized successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "ARRR Labour Rate details not finalized" : message });
                    }
                    else
                    {
                        message = "You can not finalize this ARRR Labour Rate details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not finalize this ARRR Labour Rate details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not finalize this ARRR Labour Rate details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost] // Aditi 1
        public ActionResult GetLabourFormList(int? page, int? rows, string sidx, string sord) //Added by Aditi on 21 Aug 2020
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                IARRRBAL objBAL = new ARRRBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListLabourFormBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        [HttpPost] // Aditi 2
        //[Audit]
        public JsonResult AddMultipleLabour(IEnumerable<LabourDetails> labourData)  //Added by Aditi on 24 Aug 2020
        {
            bool status = false;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            try
            {
                //ModelState["Category"].Errors.Clear();
                //if (ModelState.IsValidField("Rate"))
                //{
                using (TransactionScope objScope = new TransactionScope())
                {
                    if (objBAL.addBulkARRRLabourRatesBAL(labourData, ref message))
                    {
                        objScope.Complete();
                        status = true;
                        return Json(new { success = status, message = message == string.Empty ? "All Labour details added successfully." : message });
                    }
                    else
                    {
                        return Json(new { success = status, message = message == string.Empty ? "Labour Rate details not saved" : message });
                    }
                }
                //}
                //else
                //{
                //    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                //    return Json(new { success = status, message = msg == string.Empty ? "Labour Rate details not saved" : msg});
                //}
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {
            }
        }


        [HttpPost]  // Aditi 3
        public ActionResult FinalizeAllLabourRates(string id) //Added by Aditi on 1 Sept 2020
        {
            dbContext = new PMGSYEntities();
            int year = 0;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            try
            {
                year = Convert.ToInt32(id.Trim());
                if (year > 0)
                {
                    if (objBAL.FinalizeAllLabourRatesBAL(year))
                    {
                        status = true;

                        message = message == string.Empty ? "ARRR Labour Rate details finalized successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "ARRR Labour Rate details not finalized" : message });
                    }
                    else
                    {
                        message = "No records found to Finalize";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "Invalid Year ! ARRR Labour Rate details not finalized.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Can not finalize ARRR Labour Rate details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost] // Aditi 4
        // [ValidateAntiForgeryToken]
        public JsonResult SavePDFfile() //Added by Aditi Shree 4 Sept 2020
        {
            CommonFunctions comm = new CommonFunctions();
            string filePath = string.Empty;
            bool isFileSaved = false;
            HttpPostedFileBase FileBase = null;
            bool status = false;
            try
            {
                if (Request.Files.Count <= 0)
                {
                    return Json(new { success = false, message = "Please select a pdf file" });
                }
                IARRRDAL objDAL = new ARRRDAL();

                #region Multiple File
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        FileBase = Request.Files[i];
                        var filename = FileBase.FileName;

                        status = objDAL.SaveLabourCommisionFile(filename, FileBase, out isFileSaved);

                        if (status == false && isFileSaved == false)
                        {
                            break;
                        }
                    }
                    if (status == false && isFileSaved == false)
                    {
                        return Json(new { success = false, message = "Error in Saving uploaded file : " + FileBase.FileName });
                    }
                }
                #endregion
                return Json(new { success = true, message = "Upload Successful" });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.SavePDFfile()");
                return Json(new { success = false, message = "Error occured while uploading file" });
            }
        }

        [HttpGet]   // Aditi 5
        public ActionResult DownloadARRRPdfFile(String parameter, String hash, String key)//Added by Aditi Shree 9 Sept 2020
        {
            try
            {
                dbContext = new PMGSYEntities();
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

                var record = (from arrr in dbContext.MASTER_ARRR_STATES_RATES
                              where arrr.MAST_ARRR_RATE_CODE == obsId
                              select arrr).First();

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                if (!String.IsNullOrEmpty(record.MAST_ARRR_FILENAME))
                {
                    VirtualDirectoryUrl = ConfigurationManager.AppSettings["LabourRateFilePath_Virtual_Dir_Path"];
                    PhysicalPath = ConfigurationManager.AppSettings["LabourRateFilePath"];
                }
                else
                {
                    VirtualDirectoryUrl = null;
                    PhysicalPath = null;
                }

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
        }
        #endregion

        #region Machinery Rates
        public ActionResult MachineryRateDetails(FormCollection frmCollection)
        {
            //CommonFunctions objCommonFunctions = new CommonFunctions();
            IARRRBAL objIARRRBAL = new ARRRBAL();
            List<SelectListItem> list = objIARRRBAL.PopulateLMMItemsListBAL(false, 2, Convert.ToInt32(frmCollection["category"]));
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public ActionResult LoadMachineryRate()
        {
            LMMRateViewModel model = new LMMRateViewModel();
            ProposalController pc = new ProposalController();
            model.yearList = pc.PopulateYear(0, true, false);
            model.yearList.Find(x => x.Value == "0").Value = "-1";
            return View(model);
        }

        //[HttpGet]
        public ActionResult MachineryRateLayout(string parameter, string hash, string key)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();

            LMMRateViewModel model = new LMMRateViewModel();
            CommonFunctions comm = new CommonFunctions();
            ProposalController pc = new ProposalController();
            try
            {
                model.rateType = 2;
                //model.ItemList = objIARRRBAL.PopulateLMMItemsListBAL(false, model.rateType);
                model.ItemList = new List<SelectListItem>();
                model.ItemList.Insert(0, new SelectListItem { Text = "Select", Value = "-1" });

                model.CategoryList = objIARRRBAL.PopulateCategoryBAL(false, 2);

                //model.yearList = comm.PopulateYears(true);
                model.yearList = pc.PopulateYear(0, true, false);
                model.yearList.Find(x => x.Value == "0").Value = "-1";

                if (Request.Params["User_Action"] == null)
                {
                    model.flag = "Y";
                    //model.ItemList.RemoveAt(0);
                    model.stateCode = PMGSYSession.Current.StateCode;

                    model.status = "N";
                }
                else
                {
                    model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                    if (parameter != null && hash != null && key != null)
                    {
                        model.EncryptedRateCode = parameter.Trim() + "/" + hash.Trim() + "/" + key.Trim();
                        decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    }

                    if (decryptedParameters != null)
                    {
                        dbContext = new PMGSYEntities();

                        model.rateCode = Convert.ToInt32(decryptedParameters["rateCode"]);

                        model.Item = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_LMM_TYPE_CODE).FirstOrDefault();
                        model.hdnItem = model.Item;
                        //string itmName = "";
                        //model.ItemList = new List<SelectListItem>();
                        //itmName = dbContext.MASTER_ARR_LMM_TYPES.Where(m => m.MAST_LMM_ACTY_CODE == model.Item).Select(m => m.MAST_LMM_DESC).FirstOrDefault();
                        //model.ItemList.Insert(0, new SelectListItem { Text = itmName.Trim(), Value = model.Item.ToString() });

                        model.flag = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.ACTIVE_FLAG).FirstOrDefault();

                        model.rateType = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_LMM_TYPE).FirstOrDefault();
                        model.status = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_RATE_IS_FINAL).FirstOrDefault();
                        model.stateCode = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();

                        model.Rate = (decimal)dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_RATE).FirstOrDefault();
                        //model.Date = Convert.ToDateTime(dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_EFFECTIVE_FROM).FirstOrDefault()).ToString("dd/MM/yyyy");
                        //model.TillDate = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_EFFECTIVE_TILL).FirstOrDefault() == null ? "" : Convert.ToDateTime(dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_EFFECTIVE_TILL).FirstOrDefault()).ToString("dd/MM/yyyy");
                        model.Category = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MASTER_ARRR_LMM_TYPES.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_CAT_CODE).FirstOrDefault();
                        model.Year = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).Select(m => m.MAST_ARRR_YEAR).FirstOrDefault();
                    }
                }
                return View(model);
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
        public ActionResult GetMachineryRateList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                IARRRBAL objBAL = new ARRRBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                int Year = String.IsNullOrEmpty(Request.Params["year"]) ? 0 : Convert.ToInt32(Request.Params["year"]);
                var jsonData = new
                {
                    rows = objBAL.ListMachineryRatesBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, Year, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }





        public ActionResult AddEditMachineryRateDetails(LMMRateViewModel model)
        {
            bool status = false;
            string message = "";

            IARRRBAL objBAL = new ARRRBAL();
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                if (ModelState.IsValid)
                {
                    if (model.User_Action == "A")
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.addARRRMachineryRatesBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "ARRR Machinery Rate details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "ARRR Machinery Rate details not saved" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "ARRR Machinery Rate details not saved" : message });
                            }
                        }
                    }
                    else
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.editARRRMachineryRatesBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "ARRR Machinery Rate details edited successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "ARRR Machinery Rate details not edit" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "ARRR Machinery Rate details not edit" : message });
                            }
                        }
                    }
                    //return Json(new { success = status, message = message == string.Empty ? "ARRR Master details not saved" : message });
                }
                else
                {
                    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {

            }
        }

        [HttpPost]
        public ActionResult changeMachineryRatestatus(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    rateCode = Convert.ToInt32(decryptedParameters["rateCode"]);

                    if (objBAL.changeLMMRatestatusBAL(rateCode))
                    {

                        status = true;

                        message = message == string.Empty ? "Machinery Rates status changed successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "Machinery Rates status not changed" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not change Machinery Rates status.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not change Machinery Rates status.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not change Machinery Rates status.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult delMachineryRateDetails(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            //MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    rateCode = Convert.ToInt32(decryptedParameters["rateCode"]);

                    if (objBAL.deleteARRRMachineryRatesBAL(rateCode))
                    {

                        status = true;

                        message = message == string.Empty ? "ARRR Machinery Rate details deleted successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "ARRR Machinery Rate details not deleted" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not delete this ARRR Machinery Rate details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not delete this ARRR Machinery Rate details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this ARRR Machinery Rate details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult MachineryRateFinalization(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    rateCode = Convert.ToInt32(decryptedParameters["rateCode"]);

                    if (objBAL.MachineryRateFinalizeBAL(rateCode))
                    {
                        status = true;

                        message = message == string.Empty ? "ARRR Machinery Rate details finalized successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "ARRR Machinery Rate details not finalized" : message });
                    }
                    else
                    {
                        message = "You can not finalize this ARRR Machinery Rate details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not finalize this ARRR Machinery Rate details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not finalize this ARRR Machinery Rate details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GetMachineryFormList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                IARRRBAL objBAL = new ARRRBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListMachineryFormBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        [HttpPost]
        public JsonResult AddMultipleMachineryData(IEnumerable<LabourDetails> machineryData)
        {
            bool status = false;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            try
            {
                //ModelState["Category"].Errors.Clear();
                //if (ModelState.IsValidField("Rate"))
                //{
                using (TransactionScope objScope = new TransactionScope())
                {
                    if (objBAL.addBulkARRRMachineryRatesBAL(machineryData, ref message))
                    {
                        objScope.Complete();
                        status = true;
                        return Json(new { success = status, message = message == string.Empty ? "All Machinery details added successfully." : message });
                    }
                    else
                    {
                        return Json(new { success = status, message = message == string.Empty ? "Machinery Rate details not saved" : message });
                    }
                }
                //}
                //else
                //{
                //    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                //    return Json(new { success = status, message = msg == string.Empty ? "Labour Rate details not saved" : msg});
                //}
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {
            }
        }

        [HttpPost]
        public ActionResult FinalizeAllMachineryRates(string id)
        {
            dbContext = new PMGSYEntities();
            int year = 0;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            try
            {
                year = Convert.ToInt32(id.Trim());
                if (year > 0)
                {
                    if (objBAL.FinalizeAllMachineryRatesBAL(year))
                    {
                        status = true;

                        message = message == string.Empty ? "ARRR Machinery Rate details finalized successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "ARRR Machinery Rate details not finalized" : message });
                    }
                    else
                    {
                        message = "No records found to Finalize";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "Invalid Year ! ARRR Machinery Rate details not finalized.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Can not finalize ARRR Machinery Rate details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveMachineryPDFfile()
        {
            CommonFunctions comm = new CommonFunctions();
            string filePath = string.Empty;
            bool isFileSaved = false;
            HttpPostedFileBase FileBase = null;
            bool status = false;
            try
            {
                if (Request.Files.Count <= 0)
                {
                    return Json(new { success = false, message = "Please select a pdf file" });
                }
                IARRRDAL objDAL = new ARRRDAL();

                #region Multiple File
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        FileBase = Request.Files[i];
                        var filename = FileBase.FileName;

                        status = objDAL.SaveARRRMachineryFile(filename, FileBase, out isFileSaved);

                        if (status == false && isFileSaved == false)
                        {
                            break;
                        }
                    }
                    if (status == false && isFileSaved == false)
                    {
                        return Json(new { success = false, message = "Error in Saving uploaded file : " + FileBase.FileName });
                    }
                }
                #endregion
                return Json(new { success = true, message = "Upload Successful" });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.SaveMachineryPDFfile()");
                return Json(new { success = false, message = "Error occured while uploading file" });
            }
        }

        [HttpGet]
        public ActionResult DownloadMachineryPdfFile(String parameter, String hash, String key)
        {
            try
            {
                dbContext = new PMGSYEntities();
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

                var record = (from arrr in dbContext.MASTER_ARRR_STATES_RATES
                              where arrr.MAST_ARRR_RATE_CODE == obsId
                              select arrr).First();

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                if (!String.IsNullOrEmpty(record.MAST_ARRR_FILENAME))
                {
                    VirtualDirectoryUrl = ConfigurationManager.AppSettings["MachineryRateFilePath_Virtual_Dir_Path"];
                    PhysicalPath = ConfigurationManager.AppSettings["MachineryRateFilePath"];
                }
                else
                {
                    VirtualDirectoryUrl = null;
                    PhysicalPath = null;
                }

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
        }
        #endregion

        #region Analysis of Rates
        /*

        public ActionResult PopoulateAnalysisYear(FormCollection frmCollection)
        {
            //CommonFunctions objCommonFunctions = new CommonFunctions();
            IARRRBAL objIARRRBAL = new ARRRBAL();
            List<SelectListItem> list = objIARRRBAL.PopulateAnalysisYearsBAL(false);
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MinorItemDetails(FormCollection frmCollection)
        {
            //CommonFunctions objCommonFunctions = new CommonFunctions();
            IARRRBAL objIARRRBAL = new ARRRBAL();
            List<SelectListItem> list = objIARRRBAL.PopulateMinorItemsListBAL(false, Convert.ToInt32(frmCollection["ItemCode"]));
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult lmmTypeDetails(FormCollection frmCollection)
        {
            //CommonFunctions objCommonFunctions = new CommonFunctions();
            IARRRBAL objIARRRBAL = new ARRRBAL();
            List<SelectListItem> list = objIARRRBAL.PopulateLMMItemsListBAL(false, Convert.ToInt32(frmCollection["lmmType"]));
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRateDetails(FormCollection frmCollection)
        {
            int rateCode = 0;
            string rate;
            int typeCode = 0;
            //CommonFunctions objCommonFunctions = new CommonFunctions();
            //IARRRBAL objIARRRBAL = new ARRRBAL();
            //List<SelectListItem> list = objIARRRBAL.PopulateLMMItemsListBAL(false, Convert.ToInt32(frmCollection["lmmType"]));
            //list.Find(x => x.Value == "-1").Value = "0";
            dbContext = new PMGSYEntities();
            try
            {
                typeCode = Convert.ToInt32(frmCollection["typeCode"]);
                rate = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_LMM_TYPE_CODE == typeCode && m.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_ARRR_RATE).FirstOrDefault().ToString();
                rateCode = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_LMM_TYPE_CODE == typeCode && m.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_ARRR_RATE_CODE).FirstOrDefault();
                return Json(new { success = true, rate = rate.Trim(), Code = rateCode.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
                return null;
            }
        }

        //public ActionResult AnalysisRatesLayout()
        //{
        //    CommonFunctions comm = new CommonFunctions();
        //    AnalysisRatesViewModel model = new AnalysisRatesViewModel();
        //    try
        //    {


        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return null;        
        //    }
        //}

        public ActionResult LoadAnalysisRates()
        {
            CommonFunctions comm = new CommonFunctions();
            AnalysisRatesViewModel model = new AnalysisRatesViewModel();

            try
            {
                model.YearList = comm.PopulateFinancialYear(true).ToList();
                model.Year = System.DateTime.Now.Year;

                model.copyYearList = new List<SelectListItem>();
                model.copyYearList.Insert(0, new SelectListItem { Text = "All", Value = "0" });

                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        //[HttpGet]
        public ActionResult AnalysisRatesLayout(string parameter, string hash, string key)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();

            AnalysisRatesViewModel model = new AnalysisRatesViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.ChapterList = objIARRRBAL.PopulateChapterListBAL(false);

                model.ItemList = new List<SelectListItem>();
                model.ItemList.Insert(0, new SelectListItem { Text = "All", Value = "0" });

                model.MajorItemList = new List<SelectListItem>();
                model.MajorItemList.Insert(0, new SelectListItem { Text = "All", Value = "0" });

                model.MinorItemList = new List<SelectListItem>();
                model.MinorItemList.Insert(0, new SelectListItem { Text = "All", Value = "0" });

                model.LabourList = new List<SelectListItem>();
                model.LabourList.Insert(0, new SelectListItem { Text = "All", Value = "0" });

                model.MachineryList = new List<SelectListItem>();
                model.MachineryList.Insert(0, new SelectListItem { Text = "All", Value = "0" });

                model.MaterialList = new List<SelectListItem>();
                model.MaterialList.Insert(0, new SelectListItem { Text = "All", Value = "0" });

                model.lmmType = "1";
                model.lmmTypeList = objIARRRBAL.PopulateLMMItemsListBAL(false, 1);

                if (Request.Params["User_Action"] == null)
                {
                    model.Year = Convert.ToInt32(Request.Params["year"]);
                    model.User_Action = "A";
                    model.stateCode = PMGSYSession.Current.StateCode;

                    model.status = "N";
                }
                else
                {
                    model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                    if (parameter != null && hash != null && key != null)
                    {
                        model.EncryptedCode = parameter.Trim() + "/" + hash.Trim() + "/" + key.Trim();
                        decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    }

                    if (decryptedParameters != null)
                    {
                        dbContext = new PMGSYEntities();

                        model.analysisCode = Convert.ToInt32(decryptedParameters["analysisCode"]);

                        //model.stateCode = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                        model.ItemCode = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).Select(m => m.MAST_ITEM_CODE).FirstOrDefault();

                        int parent = 0;
                        int majItem = 0;
                        int minItem = 0;

                        parent = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).Select(m => m.MASTER_ARRR_ITEMS_MASTER.MAST_ITEM_PARENT).FirstOrDefault();
                        majItem = (int)dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).Select(m => m.MASTER_ARRR_ITEMS_MASTER.MAST_MAJOR_SUBITEM_CODE).FirstOrDefault();
                        minItem = (int)dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).Select(m => m.MASTER_ARRR_ITEMS_MASTER.MAST_MINOR_SUBITEM_CODE).FirstOrDefault();

                        model.ItemType = parent == 0 ? "I" : (majItem > 0 && minItem == 0) ? "M" : (majItem > 0 && minItem == 0) ? "N" : "";

                        string itemName;
                        itemName = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).Select(m => m.MASTER_ARRR_ITEMS_MASTER.MAST_ITEM_NAME).FirstOrDefault();//model.ItemCode.ToString();
                        model.ItemList = new List<SelectListItem>();
                        model.ItemList.Insert(0, new SelectListItem { Text = itemName, Value = model.ItemCode.ToString() });

                        model.ChapterList = objIARRRBAL.PopulateChapterListBAL(false);
                        //model.Chapter = dbContext.MASTER_ARR_ITEM_HEAD.Where(m => m.MAST_HEAD_CODE == model.ItemCode).Select(m => m.MAST_HEAD_CODE).FirstOrDefault();
                        model.Chapter = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).Select(m => m.MASTER_ARRR_ITEMS_MASTER.MAST_HEAD_CODE).FirstOrDefault();

                        model.lmmType = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).Select(m => m.MASTER_ARRR_LMM_TYPES.MAST_LMM_TYPE).FirstOrDefault().ToString();//model.ItemCode.ToString();
                        model.hdnlmmType = model.lmmType;
                        //model.lmmTypeList = objIARRRBAL.PopulateLMMItemsListBAL(false, Convert.ToInt32(model.lmmType));

                        model.Labour = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).Select(m => m.MASTER_ARRR_LMM_TYPES.MAST_LMM_TYPE_CODE).FirstOrDefault();//model.ItemCode.ToString();

                        string labourName = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).Select(m => m.MASTER_ARRR_LMM_TYPES.MAST_LMM_DESC).FirstOrDefault();//model.ItemCode.ToString();
                        model.lmmTypeList = new List<SelectListItem>();
                        model.lmmTypeList.Insert(0, new SelectListItem { Text = labourName, Value = model.Labour.ToString() });



                        model.status = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.analysisCode).Select(m => m.MAST_ARRR_RATE_IS_FINAL).FirstOrDefault();

                        //model.Amount = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).Select(m => m.MAST_ARRR_AMOUNT).FirstOrDefault();
                        model.quantity = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).Select(m => m.MAST_ARRR_QTY).FirstOrDefault();
                        model.Rate = (decimal)dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.analysisCode).Select(m => m.MAST_ARRR_RATE).FirstOrDefault();
                        //model.Date = Convert.ToDateTime(dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.analysisCode).Select(m => m.MAST_ARRR_EFFECTIVE_FROM).FirstOrDefault()).ToString("dd/MM/yyyy");
                    }
                }
                return View(model);
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
        public ActionResult GetAnalysisRatesList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                int year = Convert.ToInt32(Request.Params["year"]);

                IARRRBAL objBAL = new ARRRBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.ListAnalysisRatesBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, year),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords

                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        [HttpPost]
        //[Audit]
        public JsonResult UpdateAnalysisRatesDetails(FormCollection formCollection)
        {
            bool status = false;
            string message = "";
            string[] arrKey = formCollection["id"].Split('$');
            AnalysisRatesViewModel model = new AnalysisRatesViewModel();
            model.analysisCode = Convert.ToInt32(arrKey[0]);
            //newsfileupload.NEWS_FILE_ID = Convert.ToInt32(arrKey[1]);

            Regex regex = new Regex(@"^[0-9.]+$");
            if (regex.IsMatch(formCollection["Quantity"]) && formCollection["Quantity"].Trim().Length != 0)
            {
                model.quantity = Convert.ToDecimal(formCollection["Quantity"]);
            }
            else
            {
                return Json("Invalid Quantity, Only decimal mumbers are allowed");
            }

            if (regex.IsMatch(formCollection["Amount"]) && formCollection["Amount"].Trim().Length != 0)
            {
                model.Amount = Convert.ToDecimal(formCollection["Amount"]);
            }
            else
            {
                return Json("Invalid Amount, Only decimal mumbers are allowed");
            }

            //string status = newsDAL.UpdatePDFDetailsDAL(news_files);
            //if (status == string.Empty)
            //    return Json(true);
            //else
            //    return Json("There is an error occured while processing your request.");
            objBAL = new ARRRBAL();
            if (objBAL.editARRRAnalysisRatesBAL(model, ref message))
            {
                //objScope.Complete();
                message = message == string.Empty ? "Analysis Rates details edited successfully." : message;
                status = true;
                //return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not edit" : message });
                return Json(status);
            }
            else
            {
                //return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not edit" : message });
                return Json("Analysis Rates details not edited");
            }
        }

        public ActionResult AddEditAnalysisRatesDetails(AnalysisRatesViewModel model)
        {
            bool status = false;
            string message = "";

            IARRRBAL objBAL = new ARRRBAL();
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                if (ModelState.IsValid)
                {
                    if (model.User_Action == "A")
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.addARRRAnalysisRatesBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Analysis Rates details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not saved" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not saved" : message });
                            }
                        }
                    }
                    else
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.editARRRAnalysisRatesBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Analysis Rates details edited successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not edit" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not edit" : message });
                            }
                        }
                    }
                    //return Json(new { success = status, message = message == string.Empty ? "ARRR Master details not saved" : message });
                }
                else
                {
                    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {

            }
        }

        [HttpPost]
        public ActionResult delAnalysisRatesDetails(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            //MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    rateCode = Convert.ToInt32(decryptedParameters["analysisCode"]);

                    if (objBAL.deleteARRRAnalysisRatesBAL(rateCode))
                    {

                        status = true;

                        message = message == string.Empty ? "Analysis Rates details deleted successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not deleted" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not delete this Analysis Rates details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not delete this Analysis Rates details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Analysis Rates details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AnalysisRatesFinalization(string id)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            int year = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                year = Convert.ToInt32(id.Trim());
                //decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                //if (decryptedParameters.Count > 0)
                if (year > 0)
                {
                    //rateCode = Convert.ToInt32(decryptedParameters["analysisCode"]);

                    if (objBAL.AnalysisRateFinalizeBAL(year))
                    {
                        status = true;

                        message = message == string.Empty ? "Analysis Rates details finalized successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not finalized" : message });
                    }
                    else
                    {
                        message = "You can not finalize this Analysis Rates details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not finalize this Analysis Rates details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not finalize this Analysis Rates details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AnalysisRatesApproval(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    rateCode = Convert.ToInt32(decryptedParameters["analysisCode"]);

                    if (objBAL.AnalysisRateApproveBAL(rateCode))
                    {
                        status = true;

                        message = message == string.Empty ? "Analysis Rate details approved successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "Analysis Rate details not approved" : message });
                    }
                    else
                    {
                        message = "You can not approve this Analysis Rate details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not approve this Analysis Rate details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not approve this Analysis Rate details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult copyAnalysisRatesDetails(string id)
        {
            int frmyear = 0;
            int toyear = 0;
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            //MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            p = id.Split('$');
            try
            {
                frmyear = Convert.ToInt32(p[0].Trim());
                toyear = Convert.ToInt32(p[1].Trim());
                //decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                //if (decryptedParameters.Count > 0)
                if (frmyear > 0)
                {
                    //rateCode = Convert.ToInt32(decryptedParameters["analysisCode"]);

                    if (objBAL.copyARRRAnalysisRatesBAL(frmyear,toyear, ref message))
                    {

                        status = true;

                        message = message == string.Empty ? "Analysis Rates details copied successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not copied" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not copy this Analysis Rates details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not copy this Analysis Rates details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not copy this Analysis Rates details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        */
        #endregion

        #region Schedule of Rates
        //[HttpGet]
        public ActionResult LoadScheduleRatesLayout(/*string parameter, string hash, string key*/)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();

            ScheduleRatesViewModel model = new ScheduleRatesViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.ChapterList = objIARRRBAL.PopulateChapterListBAL(false);
                return View(model);
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
        public ActionResult GetScheduleChapterItemsList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                IARRRBAL objBAL = new ARRRBAL();
                int chapter = Convert.ToInt32(Request.Params["chapter"]);
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                var jsonData = new
                {
                    rows = objBAL.ListScheduleChapterItemsBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, chapter),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords

                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        public ActionResult LoadScheduleItemsLayout(string parameter, string hash, string key)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();
            ScheduleRatesViewModel model = new ScheduleRatesViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (parameter != null && hash != null && key != null)
                {
                    model.EncryptedCode = parameter.Trim() + "/" + hash.Trim() + "/" + key.Trim();
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                    if (decryptedParameters != null)
                    {
                        dbContext = new PMGSYEntities();

                        model.ItemCode = Convert.ToInt32(decryptedParameters["itemCode"]);
                        model.chapterName = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MASTER_ARRR_ITEM_HEAD.MAST_HEAD_NAME).FirstOrDefault();
                        model.itemName = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MAST_ITEM_NAME).FirstOrDefault();

                        //Added on 6 April 2021 --Modification Start
                        model.Chapter = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).Select(m => m.MASTER_ARRR_ITEM_HEAD.MAST_HEAD_CODE).FirstOrDefault();
                        model.ItemList = objIARRRBAL.PopulateItemsListBAL(false, model.Chapter);
                        model.MajorItemList = objIARRRBAL.PopulateMajorItemsListItemwiseBAL(false, Convert.ToInt32(decryptedParameters["itemCode"]));

                        //Modification End

                        //model.MajorItemList = objIARRRBAL.PopulateScheduleMajorItemsListBAL(false, model.ItemCode);
                        model.MinorItemList = objIARRRBAL.PopulateMinorItemsListBAL(false, model.ItemCode);

                        model.ItemTypeCode = model.MinorItem > 0 ? model.MinorItem : model.MajorItem > 0 ? model.MinorItem : model.ItemCode;
                    }
                }
                return View(model);
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
        public ActionResult GetScheduleLMMList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                int chapter = Convert.ToInt32(Request.Params["chapter"]);
                int itemCode = Convert.ToInt32(Request.Params["itemCode"]);

                IARRRBAL objBAL = new ARRRBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                var jsonData = new
                {
                    rows = objBAL.ListScheduleLMMBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, chapter, itemCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        [HttpPost]
        //[Audit]
        public JsonResult UpdateScheduleLMM(FormCollection formCollection)
        {
            bool status = false;
            string message = "";
            string[] arrKey = formCollection["id"].Split('$');
            ScheduleRatesViewModel model = new ScheduleRatesViewModel();
            model.scheduleCode = Convert.ToInt32(arrKey[0]);

            Regex regex = new Regex(@"^[0-9.]+$");
            if (regex.IsMatch(formCollection["Quantity"]) && formCollection["Quantity"].Trim().Length != 0)
            {
                model.quantity = Convert.ToDecimal(formCollection["Quantity"]);
            }
            else
            {
                return Json("Invalid Quantity, Only decimal mumbers are allowed");
            }

            objBAL = new ARRRBAL();
            if (objBAL.editScheduleLMMBAL(model, ref message))
            {
                //objScope.Complete();
                message = message == string.Empty ? "Schedule details edited successfully." : message;
                status = true;
                return Json(status);
            }
            else
            {
                //return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not edit" : message });
                return Json("Schedule details not edited");
            }
        }

        [HttpPost]
        public ActionResult ScheduleFinalization(string id)
        {
            dbContext = new PMGSYEntities();
            int rateCode = 0;
            int chapter = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;
            MASTER_ARRR_ITEMS_MASTER model = new MASTER_ARRR_ITEMS_MASTER();

            try
            {
                chapter = Convert.ToInt32(id.Trim());
                //decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                //if (decryptedParameters.Count > 0)
                if (chapter > 0)
                {
                    //rateCode = Convert.ToInt32(decryptedParameters["analysisCode"]);

                    if (objBAL.ScheduleFinalizeBAL(chapter))
                    {
                        status = true;

                        message = message == string.Empty ? "Analysis Rates details finalized successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not finalized" : message });
                    }
                    else
                    {
                        message = "You can not finalize this Analysis Rates details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not finalize this Analysis Rates details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not finalize this Analysis Rates details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult delScheduleLMM(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int scheduleCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    scheduleCode = Convert.ToInt32(decryptedParameters["scheduleCode"]);
                    if (objBAL.deleteScheduleLMMBAL(scheduleCode))
                    {
                        status = true;
                        message = message == string.Empty ? "Schedule details deleted successfully." : message;
                        return Json(new { success = status, message = message == string.Empty ? "Schedule details not deleted" : message });
                    }
                    else
                    {
                        message = "You can not delete this Schedule details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not delete this Schedule details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Schedule details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LabourScheduleLayout(string id)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();
            ScheduleRatesViewModel model = new ScheduleRatesViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (Request.Params["typeCode"] != null)
                {
                    model.ItemTypeCode = Convert.ToInt32(Request.Params["typeCode"]);
                }
                model.ItemCode = Convert.ToInt32(id.Trim());
                if (model.ItemCode > 0)
                {
                    dbContext = new PMGSYEntities();
                    model.CategoryList = objIARRRBAL.PopulateCategoryBAL(false, 1);
                    model.lmmTypeList = new List<SelectListItem>();
                    model.lmmTypeList.Insert(0, new SelectListItem { Text = "Select", Value = "-1" });
                    //model.lmmTypeList = objIARRRBAL.PopulateLMMItemsListBAL(false, 1, 0);
                    model.hdnItemTypeCode = model.ItemTypeCode;
                }
                return View(model);
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


        public ActionResult AddScheduleLabour(ScheduleRatesViewModel model)
        {
            bool status = false;
            string message = "";

            IARRRBAL objBAL = new ARRRBAL();
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                if (ModelState.IsValid)
                {
                    if (model.User_Action == "A" || model.User_Action == null)
                    {
                        model.hdlmmType = 1;
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.addScheduleLMMBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Labour Schedule details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Labour Schedule details not saved" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Labour Schedule details not saved" : message });
                            }
                        }
                    }
                    return Json(new { success = status, message = message == string.Empty ? "ARRR Master details not saved" : message });
                }
                else
                {
                    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {

            }
        }

        #region
        //[HttpPost]
        //public ActionResult GetScheduleLabourList(int? page, int? rows, string sidx, string sord)
        //{
        //    try
        //    {
        //        String searchParameters = String.Empty;
        //        long totalRecords;

        //        Dictionary<string, string> parameters = new Dictionary<string, string>();

        //        //int chapter = Convert.ToInt32(Request.Params["chapter"]);
        //        int itemCode = Convert.ToInt32(Request.Params["itemCode"]);

        //        IARRRBAL objBAL = new ARRRBAL();
        //        using (CommonFunctions commonFunction = new CommonFunctions())
        //        {
        //            if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
        //            {
        //                return null;
        //            }
        //        }
        //        var jsonData = new
        //        {
        //            rows = objBAL.ListScheduleLMMBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, 1, itemCode),
        //            total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
        //            page = Convert.ToInt32(page),
        //            records = totalRecords
        //        };
        //        return Json(jsonData, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return Json(string.Empty);
        //    }
        //}

        //[HttpPost]
        ////[Audit]
        //public JsonResult UpdateScheduleLabour(FormCollection formCollection)
        //{
        //    bool status = false;
        //    string message = "";
        //    string[] arrKey = formCollection["id"].Split('$');
        //    ScheduleRatesViewModel model = new ScheduleRatesViewModel();
        //    model.scheduleCode = Convert.ToInt32(arrKey[0]);
        //    //newsfileupload.NEWS_FILE_ID = Convert.ToInt32(arrKey[1]);

        //    Regex regex = new Regex(@"^[0-9.]+$");
        //    if (regex.IsMatch(formCollection["Quantity"]) && formCollection["Quantity"].Trim().Length != 0)
        //    {
        //        model.quantity = Convert.ToDecimal(formCollection["Quantity"]);
        //    }
        //    else
        //    {
        //        return Json("Invalid Quantity, Only decimal mumbers are allowed");
        //    }

        //    objBAL = new ARRRBAL();
        //    if (objBAL.editScheduleLMMBAL(model, ref message))
        //    {
        //        //objScope.Complete();
        //        message = message == string.Empty ? "Labour Schedule details edited successfully." : message;
        //        status = true;
        //        //return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not edit" : message });
        //        return Json(status);
        //    }
        //    else
        //    {
        //        //return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not edit" : message });
        //        return Json("Labour Schedule details not edited");
        //    }
        //}

        //[HttpPost]
        //public ActionResult delScheduleLabour(String parameter, String hash, String key)
        //{
        //    dbContext = new PMGSYEntities();
        //    int scheduleCode = 0;
        //    string[] p;
        //    string message = "";
        //    IARRRBAL objBAL = new ARRRBAL();
        //    bool status = false;

        //    try
        //    {
        //        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
        //        if (decryptedParameters.Count > 0)
        //        {
        //            scheduleCode = Convert.ToInt32(decryptedParameters["scheduleCode"]);
        //            if (objBAL.deleteScheduleLMMBAL(scheduleCode))
        //            {
        //                status = true;
        //                message = message == string.Empty ? "Labour Schedule details deleted successfully." : message;
        //                return Json(new { success = status, message = message == string.Empty ? "Labour Schedule details not deleted" : message });
        //            }
        //            else
        //            {
        //                message = "You can not delete this Labour Schedule details.";
        //                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else
        //        {
        //            message = "You can not delete this Labour Schedule details.";
        //            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        message = "You can not delete this Labour Schedule details.";
        //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        #endregion

        public ActionResult LMMScheduleDetails(FormCollection frmCollection)
        {
            //CommonFunctions objCommonFunctions = new CommonFunctions();
            IARRRBAL objIARRRBAL = new ARRRBAL();
            List<SelectListItem> list = objIARRRBAL.PopulateLMMItemsListBAL(false, Convert.ToInt32(frmCollection["lmmCode"]), Convert.ToInt32(frmCollection["category"]));
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MachineryScheduleLayout(string id)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();
            ScheduleRatesViewModel model = new ScheduleRatesViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (Request.Params["typeCode"] != null)
                {
                    model.ItemTypeCode = Convert.ToInt32(Request.Params["typeCode"]);
                }
                model.ItemCode = Convert.ToInt32(id.Trim());
                if (model.ItemCode > 0)
                {
                    dbContext = new PMGSYEntities();
                    model.CategoryList = objIARRRBAL.PopulateCategoryBAL(false, 2);
                    model.lmmTypeList = new List<SelectListItem>();
                    model.lmmTypeList.Insert(0, new SelectListItem { Text = "Select", Value = "-1" });
                    //model.lmmTypeList = objIARRRBAL.PopulateLMMItemsListBAL(false, 2, 0);
                    model.hdnItemTypeCode = model.ItemTypeCode;
                }
                return View(model);
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

        public ActionResult AddScheduleMachineryDetails(ScheduleRatesViewModel model)
        {
            bool status = false;
            string message = "";

            IARRRBAL objBAL = new ARRRBAL();
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                if (ModelState.IsValid)
                {
                    if (model.User_Action == "A" || model.User_Action == string.Empty)
                    {
                        model.hdlmmType = 2;
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.addScheduleLMMBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Machinery Schedule details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Machinery Schedule details not saved" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Machinery Schedule details not saved" : message });
                            }
                        }
                    }
                    return Json(new { success = status, message = message == string.Empty ? "Machinery Schedule details not saved" : message });
                }
                else
                {
                    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {

            }
        }

        #region
        //[HttpPost]
        //public ActionResult GetScheduleMachineryList(int? page, int? rows, string sidx, string sord)
        //{
        //    try
        //    {
        //        String searchParameters = String.Empty;
        //        long totalRecords;

        //        Dictionary<string, string> parameters = new Dictionary<string, string>();

        //        //int chapter = Convert.ToInt32(Request.Params["chapter"]);
        //        int itemCode = Convert.ToInt32(Request.Params["itemCode"]);

        //        IARRRBAL objBAL = new ARRRBAL();
        //        using (CommonFunctions commonFunction = new CommonFunctions())
        //        {
        //            if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
        //            {
        //                return null;
        //            }
        //        }
        //        var jsonData = new
        //        {
        //            rows = objBAL.ListScheduleLMMBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, 2, itemCode),
        //            total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
        //            page = Convert.ToInt32(page),
        //            records = totalRecords
        //        };
        //        return Json(jsonData, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return Json(string.Empty);
        //    }
        //}

        //[HttpPost]
        ////[Audit]
        //public JsonResult UpdateScheduleMachinery(FormCollection formCollection)
        //{
        //    bool status = false;
        //    string message = "";
        //    string[] arrKey = formCollection["id"].Split('$');
        //    ScheduleRatesViewModel model = new ScheduleRatesViewModel();
        //    model.scheduleCode = Convert.ToInt32(arrKey[0]);
        //    //newsfileupload.NEWS_FILE_ID = Convert.ToInt32(arrKey[1]);

        //    Regex regex = new Regex(@"^[0-9.]+$");
        //    if (regex.IsMatch(formCollection["Quantity"]) && formCollection["Quantity"].Trim().Length != 0)
        //    {
        //        model.quantity = Convert.ToDecimal(formCollection["Quantity"]);
        //    }
        //    else
        //    {
        //        return Json("Invalid Quantity, Only decimal mumbers are allowed");
        //    }

        //    objBAL = new ARRRBAL();
        //    if (objBAL.editScheduleLMMBAL(model, ref message))
        //    {
        //        //objScope.Complete();
        //        message = message == string.Empty ? "Machinery Schedule details edited successfully." : message;
        //        status = true;
        //        //return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not edit" : message });
        //        return Json(status);
        //    }
        //    else
        //    {
        //        //return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not edit" : message });
        //        return Json("Machinery Schedule details not edited");
        //    }
        //}

        //[HttpPost]
        //public ActionResult delScheduleMachinery(String parameter, String hash, String key)
        //{
        //    dbContext = new PMGSYEntities();
        //    int scheduleCode = 0;
        //    string[] p;
        //    string message = "";
        //    IARRRBAL objBAL = new ARRRBAL();
        //    bool status = false;

        //    try
        //    {
        //        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
        //        if (decryptedParameters.Count > 0)
        //        {
        //            scheduleCode = Convert.ToInt32(decryptedParameters["scheduleCode"]);
        //            if (objBAL.deleteScheduleLMMBAL(scheduleCode))
        //            {
        //                status = true;
        //                message = message == string.Empty ? "Machinery Schedule details deleted successfully." : message;
        //                return Json(new { success = status, message = message == string.Empty ? "Machinery Schedule details not deleted" : message });
        //            }
        //            else
        //            {
        //                message = "You can not delete this Machinery Schedule details.";
        //                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else
        //        {
        //            message = "You can not delete this Machinery Schedule details.";
        //            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        message = "You can not delete this Machinery Schedule details.";
        //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        #endregion

        public ActionResult MaterialScheduleLayout(string id)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();
            ScheduleRatesViewModel model = new ScheduleRatesViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (Request.Params["typeCode"] != null)
                {
                    model.ItemTypeCode = Convert.ToInt32(Request.Params["typeCode"]);
                }
                model.ItemCode = Convert.ToInt32(id.Trim());
                if (model.ItemCode > 0)
                {
                    dbContext = new PMGSYEntities();
                    model.CategoryList = objIARRRBAL.PopulateCategoryBAL(false, 3);
                    model.lmmTypeList = new List<SelectListItem>();
                    model.lmmTypeList.Insert(0, new SelectListItem { Text = "Select", Value = "-1" });
                    //model.lmmTypeList = objIARRRBAL.PopulateLMMItemsListBAL(false, 3, 0);
                    model.hdnItemTypeCode = model.ItemTypeCode;
                }
                return View(model);
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

        public ActionResult AddScheduleMaterialDetails(ScheduleRatesViewModel model)
        {
            bool status = false;
            string message = "";

            IARRRBAL objBAL = new ARRRBAL();
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                if (ModelState.IsValid)
                {
                    if (model.User_Action == "A" || model.User_Action == string.Empty)
                    {
                        model.hdlmmType = 3;
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.addScheduleLMMBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Material Schedule details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Material Schedule details not saved" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Material Schedule details not saved" : message });
                            }
                        }
                    }
                    return Json(new { success = status, message = message == string.Empty ? "Material Schedule details not saved" : message });
                }
                else
                {
                    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {

            }
        }

        public ActionResult TaxScheduleLayout(string id)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();
            TaxScheduleViewModel model = new TaxScheduleViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (Request.Params["itemCode"] != null)
                {
                    model.itemCode = Convert.ToInt32(Request.Params["typeCode"]);
                }
                model.itemCode = Convert.ToInt32(id.Trim());
                if (model.itemCode > 0)
                {
                    dbContext = new PMGSYEntities();
                    model.taxList = objIARRRBAL.PopulateTaxListBAL(false);
                    model.taxList.RemoveAt(1);
                    //model.taxList.Insert(0, new SelectListItem { Text = "Select", Value = "0" });
                    model.hdnitemCode = model.itemCode;
                }
                return View(model);
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
        public ActionResult GetScheduleTaxList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                //int chapter = Convert.ToInt32(Request.Params["chapter"]);
                int itemCode = Convert.ToInt32(Request.Params["itemCode"]);

                IARRRBAL objBAL = new ARRRBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                var jsonData = new
                {
                    rows = objBAL.ListScheduleTaxBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, itemCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        public ActionResult AddScheduleTaxDetails(TaxScheduleViewModel model)
        {
            bool status = false;
            string message = "";

            IARRRBAL objBAL = new ARRRBAL();
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                if (ModelState.IsValid)
                {
                    model.Rate = model.RatePer > 0 ? model.RatePer : model.Ratelmsm;

                    if ((model.User_Action == "A" || model.User_Action == string.Empty) && (model.Rate > 0))
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.addScheduleTaxBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Tax Schedule details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Tax Schedule details not saved" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Tax Schedule details not saved" : message });
                            }
                        }
                    }
                    return Json(new { success = status, message = message == string.Empty ? "Tax Schedule details not saved" : message });
                }
                else
                {
                    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {

            }
        }

        [HttpPost]
        //[Audit]
        public JsonResult UpdateScheduleTax(FormCollection formCollection)
        {
            bool status = false;
            string message = "";
            string[] arrKey = formCollection["id"].Split('$');
            TaxScheduleViewModel model = new TaxScheduleViewModel();
            model.taxCode = Convert.ToInt32(arrKey[0]);

            Regex regex = new Regex(@"^[0-9.]+$");
            if (regex.IsMatch(formCollection["Rate"]) && formCollection["Rate"].Trim().Length != 0)
            {
                model.Rate = Convert.ToDecimal(formCollection["Rate"]);
            }
            else
            {
                return Json("Invalid Rate, Only decimal mumbers are allowed");
            }

            objBAL = new ARRRBAL();
            if (objBAL.editScheduleTaxBAL(model, ref message))
            {
                //objScope.Complete();
                message = message == string.Empty ? "Tax Schedule details edited successfully." : message;
                status = true;
                return Json(status);
            }
            else
            {
                //return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not edit" : message });
                return Json("Tax Schedule details not edited");
            }
        }

        [HttpPost]
        public ActionResult delScheduleTax(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int taxCode = 0;
            string[] p;
            string message = "";
            IARRRBAL objBAL = new ARRRBAL();
            bool status = false;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    taxCode = Convert.ToInt32(decryptedParameters["taxCode"]);
                    if (objBAL.deleteScheduleTaxBAL(taxCode))
                    {
                        status = true;
                        message = message == string.Empty ? "Tax Schedule details deleted successfully." : message;
                        return Json(new { success = status, message = message == string.Empty ? "Tax Schedule details not deleted" : message });
                    }
                    else
                    {
                        message = "You can not delete this Tax Schedule details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "You can not delete this Tax Schedule details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Tax Schedule details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AnalysisofRatesReportLayout()
        {

            AnalysisOfRates model = new AnalysisOfRates();
            IARRRBAL objIARRRBAL = new ARRRBAL();
            CommonFunctions comm = new CommonFunctions();

            if (PMGSYSession.Current.StateCode == 0)
            {
                model.StateList = new List<SelectListItem>();
                model.StateList = comm.PopulateStates(false);
                //model.StateList.Find(x => x.Value == model.StateCode.ToString()).Selected = true;
            }

            model.PhaseYearList = new SelectList(comm.PopulateFinancialYear(true, true), "Value", "Text").ToList();
            model.ChapterList = objIARRRBAL.PopulateChapterListBAL(true);

            return View(model);

        }

        public ActionResult AnalysisofRatesReport(AnalysisOfRates model)
        {
            try
            {
                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.StateCode = model.StateCode > 0 ? model.StateCode : 0;
                }
                else
                {
                    model.StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
                }
                model.PhaseYear = model.PhaseYear > 0 ? model.PhaseYear : 0;
                model.Chapter = model.Chapter > 0 ? model.Chapter : 0;
                return View(model);
            }
            catch
            {
                return View();
            }
        }

        #region
        //[HttpPost]
        //public ActionResult GetScheduleMaterialList(int? page, int? rows, string sidx, string sord)
        //{
        //    try
        //    {
        //        String searchParameters = String.Empty;
        //        long totalRecords;

        //        Dictionary<string, string> parameters = new Dictionary<string, string>();

        //        //int chapter = Convert.ToInt32(Request.Params["chapter"]);
        //        int itemCode = Convert.ToInt32(Request.Params["itemCode"]);

        //        IARRRBAL objBAL = new ARRRBAL();
        //        using (CommonFunctions commonFunction = new CommonFunctions())
        //        {
        //            if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
        //            {
        //                return null;
        //            }
        //        }
        //        var jsonData = new
        //        {
        //            rows = objBAL.ListScheduleLMMBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, 3, itemCode),
        //            total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
        //            page = Convert.ToInt32(page),
        //            records = totalRecords
        //        };
        //        return Json(jsonData, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return Json(string.Empty);
        //    }
        //}

        //[HttpPost]
        ////[Audit]
        //public JsonResult UpdateScheduleMaterial(FormCollection formCollection)
        //{
        //    bool status = false;
        //    string message = "";
        //    string[] arrKey = formCollection["id"].Split('$');
        //    ScheduleRatesViewModel model = new ScheduleRatesViewModel();
        //    model.scheduleCode = Convert.ToInt32(arrKey[0]);
        //    //newsfileupload.NEWS_FILE_ID = Convert.ToInt32(arrKey[1]);

        //    Regex regex = new Regex(@"^[0-9.]+$");
        //    if (regex.IsMatch(formCollection["Quantity"]) && formCollection["Quantity"].Trim().Length != 0)
        //    {
        //        model.quantity = Convert.ToDecimal(formCollection["Quantity"]);
        //    }
        //    else
        //    {
        //        return Json("Invalid Quantity, Only decimal mumbers are allowed");
        //    }

        //    objBAL = new ARRRBAL();
        //    if (objBAL.editScheduleLMMBAL(model, ref message))
        //    {
        //        //objScope.Complete();
        //        message = message == string.Empty ? "Material Schedule details edited successfully." : message;
        //        status = true;
        //        //return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not edit" : message });
        //        return Json(status);
        //    }
        //    else
        //    {
        //        //return Json(new { success = status, message = message == string.Empty ? "Analysis Rates details not edit" : message });
        //        return Json("Material Schedule details not edited");
        //    }
        //}


        //[HttpPost]
        //public ActionResult delScheduleMaterial(String parameter, String hash, String key)
        //{
        //    dbContext = new PMGSYEntities();
        //    int scheduleCode = 0;
        //    string[] p;
        //    string message = "";
        //    IARRRBAL objBAL = new ARRRBAL();
        //    bool status = false;

        //    try
        //    {
        //        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
        //        if (decryptedParameters.Count > 0)
        //        {
        //            scheduleCode = Convert.ToInt32(decryptedParameters["scheduleCode"]);
        //            if (objBAL.deleteScheduleLMMBAL(scheduleCode))
        //            {
        //                status = true;
        //                message = message == string.Empty ? "Material Schedule details deleted successfully." : message;
        //                return Json(new { success = status, message = message == string.Empty ? "Material Schedule details not deleted" : message });
        //            }
        //            else
        //            {
        //                message = "You can not delete this Material Schedule details.";
        //                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else
        //        {
        //            message = "You can not delete this Material Schedule details.";
        //            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        message = "You can not delete this Material Schedule details.";
        //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        #endregion

        public ActionResult PopulateMinorItemsList(FormCollection frmCollection)
        {

            IARRRBAL objIARRRBAL = new ARRRBAL();
            List<SelectListItem> list = objIARRRBAL.PopulateMinorItemsListBAL(false, Convert.ToInt32(frmCollection["ItemCode"]));

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OutputScheduleLayout(string id)
        {
            IARRRBAL objIARRRBAL = new ARRRBAL();
            TaxScheduleViewModel model = new TaxScheduleViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (Request.Params["itemCode"] != null)
                {
                    model.itemCode = Convert.ToInt32(Request.Params["typeCode"]);
                }
                model.itemCode = Convert.ToInt32(id.Trim());
                if (model.itemCode > 0)
                {
                    dbContext = new PMGSYEntities();
                    model.taxList = objIARRRBAL.PopulateTaxListBAL(false);
                    model.hdnitemCode = model.itemCode;
                }
                return View(model);
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

        public ActionResult AddScheduleOutputDetails(TaxScheduleViewModel model)
        {
            bool status = false;
            string message = "";

            IARRRBAL objBAL = new ARRRBAL();
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();
                if (ModelState.IsValid)
                {
                    model.Rate = model.Ratelmsm;
                    model.tax = 0;
                    model.taxType = "F";

                    if ((model.User_Action == "A" || model.User_Action == string.Empty) && (model.Rate > 0))
                    {
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.addScheduleTaxBAL(model, ref message))
                            {
                                objScope.Complete();
                                message = message == string.Empty ? "Output Schedule details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Output Schedule details not saved" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Output Schedule details not saved" : message });
                            }
                        }
                    }
                    return Json(new { success = status, message = message == string.Empty ? "Output Schedule details not saved" : message });
                }
                else
                {
                    string msg = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {

            }
        }
        #endregion

        #region Master Screens Code

        #region Chapter Details


        //Get View1
        [HttpGet]
        public ActionResult GetChapterView()
        {
            Chapter ch = new Chapter();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                return View(ch);
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

        //Get View2
        public ActionResult AddEditChapterDetails()
        {
            Chapter cha = new Chapter();
            cha.Operation = "A";
            return PartialView("AddEditChapterDetails", cha);

        }


        //Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddChapterDetails(Chapter ch)
        {
            bool status = false;
            String message = String.Empty;
            try
            {
                objBAL = new ARRRBAL();

                if (ModelState.IsValid)
                {
                    if (objBAL.AddChapterDetailsBAL(ch, ref message))
                    {
                        message = message == string.Empty ? "Chapter details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditChapterDetails", ch);
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

        //Get List
        [HttpPost]
        public ActionResult GetChapterList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            objBAL = new ARRRBAL();
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
                    rows = objBAL.GetChapterDetailsListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        // Edit
        public ActionResult EditChapterDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            Chapter ch = new Chapter();
            objBAL = new ARRRBAL();
            //int taxCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                ch = objBAL.GetChapterDetailsBAL(Convert.ToInt32(decryptedParameters["ChapterCode"]));
                return PartialView("AddEditChapterDetails", ch);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        //Update 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditChapterDetails(Chapter taxModel)
        {
            bool status = false;
            String message = String.Empty;
            try
            {
                objBAL = new ARRRBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditChapterDetailsBAL(taxModel, ref message))
                    {
                        message = message == string.Empty ? "Chapter details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditChapterDetails", taxModel);
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

        //Delete
        public ActionResult DeleteChapterDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int chapterCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                chapterCode = Convert.ToInt32(decryptedParameters["ChapterCode"]);
                objBAL = new ARRRBAL();
                bool status = objBAL.DeleteChapterDetailsBAL(chapterCode);
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

        #endregion

        #region Material Master

        public ActionResult GetMaterialView()
        {
            Material ch = new Material();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                return View(ch);
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


        public ActionResult AddEditMaterialDetails()
        {
            Material cha = new Material();
            cha.Operation = "A";
            return PartialView("AddEditMaterialDetails", cha);

        }

        //Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMaterialDetails(Material ch)
        {
            bool status = false;
            String message = String.Empty;
            try
            {
                objBAL = new ARRRBAL();

                if (ModelState.IsValid)
                {
                    if (objBAL.AddMaterialDetailsBAL(ch, ref message))
                    {
                        message = message == string.Empty ? "Material details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMaterialDetails", ch);
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

        //List
        [HttpPost]
        public ActionResult GetMaterialList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            objBAL = new ARRRBAL();
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
                    rows = objBAL.GetMaterialDetailsListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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


        //Edit
        public ActionResult EditMaterialDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            Material ch = new Material();
            objBAL = new ARRRBAL();
            //int taxCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                ch = objBAL.GetMaterialDetailsBAL(Convert.ToInt32(decryptedParameters["MaterialCode"]));
                return PartialView("AddEditMaterialDetails", ch);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        //Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMaterialDetails(Material taxModel)
        {
            bool status = false;
            String message = String.Empty;
            try
            {
                objBAL = new ARRRBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditMaterialDetailsBAL(taxModel, ref message))
                    {
                        message = message == string.Empty ? "Material details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditMaterialDetails", taxModel);
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

        //Delete
        public ActionResult DeleteMaterialDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int materialCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                materialCode = Convert.ToInt32(decryptedParameters["MaterialCode"]);
                objBAL = new ARRRBAL();
                bool status = objBAL.DeleteMaterialDetailsBAL(materialCode);
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

        #endregion

        #region Labour Master


        public ActionResult GetLabourDetails()
        {
            Labour ch = new Labour();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                return View(ch);
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


        public ActionResult AddEditLabourDetails()
        {
            Labour cha = new Labour();
            cha.Operation = "A";
            return PartialView("AddEditLabourDetails", cha);

        }

        //Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLabourDetails(Labour ch)
        {
            bool status = false;
            String message = String.Empty;
            try
            {
                objBAL = new ARRRBAL();

                if (ModelState.IsValid)
                {
                    if (objBAL.ADDLabourBAL(ch, ref message))
                    {
                        message = message == string.Empty ? "Labour details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditLabourDetails", ch);
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

        //List
        [HttpPost]
        public ActionResult GetLabourList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            objBAL = new ARRRBAL();
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
                    rows = objBAL.GetLabourListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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


        //Edit
        public ActionResult EditLabourDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            Labour ch = new Labour();
            objBAL = new ARRRBAL();
            //int taxCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                ch = objBAL.GetLabourDetailsBAL(Convert.ToInt32(decryptedParameters["MaterialCode"]));
                return PartialView("AddEditLabourDetails", ch);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        //Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLabDetails(Labour taxModel)
        {
            bool status = false;
            String message = String.Empty;
            try
            {
                objBAL = new ARRRBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditLabourDetailsBAL(taxModel, ref message))
                    {
                        message = message == string.Empty ? "Labour details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditLabourDetails", taxModel);
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

        //Delete
        public ActionResult DeleteLabDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int materialCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                materialCode = Convert.ToInt32(decryptedParameters["MaterialCode"]);
                objBAL = new ARRRBAL();
                bool status = objBAL.DeleteLabDetailsBAL(materialCode);
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


        #endregion

        #region  Category Master

        public ActionResult GetCategoryView()
        {
            CategoryViewModel ch = new CategoryViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                return View(ch);
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

        public ActionResult AddEditCategoryDetails()
        {
            CategoryViewModel cha = new CategoryViewModel();
            cha.Operation = "A";
            return PartialView("AddEditCategoryDetails", cha);

        }

        //Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategoryDetails(CategoryViewModel ch)
        {
            bool status = false;
            String message = String.Empty;
            try
            {
                objBAL = new ARRRBAL();

                if (ModelState.IsValid)
                {
                    if (objBAL.AddCategoryDetailsBAL(ch, ref message))
                    {
                        message = message == string.Empty ? "Category details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditCategoryDetails", ch);
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

        //List
        [HttpPost]
        public ActionResult GetCategoryList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            objBAL = new ARRRBAL();
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
                    rows = objBAL.GetCategoryDetailsListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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


        //Edit
        public ActionResult EditCategoryDetails1(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            CategoryViewModel ch = new CategoryViewModel();
            objBAL = new ARRRBAL();
            //int taxCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                ch = objBAL.GetCategoryDetailsBAL(Convert.ToInt32(decryptedParameters["CategoryCode"]));
                return PartialView("AddEditCategoryDetails", ch);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        //Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategoryDetails(CategoryViewModel taxModel)
        {
            bool status = false;
            String message = String.Empty;
            try
            {
                objBAL = new ARRRBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditCategoryDetailsBAL(taxModel, ref message))
                    {
                        message = message == string.Empty ? "Category details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditCategoryDetails", taxModel);
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

        //Delete
        public ActionResult DeleteCategoryDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int materialCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                materialCode = Convert.ToInt32(decryptedParameters["CategoryCode"]);
                objBAL = new ARRRBAL();
                bool status = objBAL.DeleteCategoryDetailsBAL(materialCode);
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


        #endregion

        #region TAX Master

        //Get View1
        [HttpGet]
        public ActionResult GetTaxView()
        {
            TaxViewModel ch = new TaxViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                return View(ch);
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

        //Get View2
        public ActionResult AddEditTaxDetails()
        {
            TaxViewModel cha = new TaxViewModel();
            cha.Operation = "A";
            return PartialView("AddEditTaxDetails", cha);

        }


        //Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTaxDetails(TaxViewModel ch)
        {
            bool status = false;
            String message = String.Empty;
            try
            {
                objBAL = new ARRRBAL();

                if (ModelState.IsValid)
                {
                    if (objBAL.AddTaxDetailsBAL(ch, ref message))
                    {
                        message = message == string.Empty ? "Other Charges details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditTaxDetails", ch);
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

        //Get List
        [HttpPost]
        public ActionResult GetTaxList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            objBAL = new ARRRBAL();
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

        // Edit
        public ActionResult EditTaxDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            TaxViewModel ch = new TaxViewModel();
            objBAL = new ARRRBAL();
            //int taxCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                ch = objBAL.GetTaxDetailsBAL(Convert.ToInt32(decryptedParameters["TaxCode"]));
                return PartialView("AddEditTaxDetails", ch);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        //Update 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTaxDetails1(TaxViewModel taxModel)
        {
            bool status = false;
            String message = String.Empty;
            try
            {
                objBAL = new ARRRBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.EditTaxDetailsBAL(taxModel, ref message))
                    {
                        message = message == string.Empty ? "Other Charges details updated successfully." : message;
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

        //Delete
        public ActionResult DeleteTaxDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int chapterCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                chapterCode = Convert.ToInt32(decryptedParameters["TaxCode"]);
                objBAL = new ARRRBAL();
                bool status = objBAL.DeleteTaxDetailsBAL(chapterCode);
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

        #endregion

        #endregion

        #region ARRR Reports



        public ActionResult LabourReport()
        {
            try
            {
                ARRRReports objPackageWiseMaintenanceModel = new ARRRReports();

                return View(objPackageWiseMaintenanceModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //   return View();
        }

        public ActionResult LabourReportDetails(ARRRReports packageAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;

                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);

                }
            }
            catch
            {
                return View(packageAgreement);
            }
        }



        public ActionResult MaterialReport()
        {
            try
            {
                ARRRReports objPackageWiseMaintenanceModel = new ARRRReports();

                return View(objPackageWiseMaintenanceModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult MaterialReportDetails(ARRRReports packageAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;

                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);

                }
            }
            catch
            {
                return View(packageAgreement);
            }
        }




        public ActionResult MachineryReport()
        {
            try
            {
                ARRRReports objPackageWiseMaintenanceModel = new ARRRReports();

                return View(objPackageWiseMaintenanceModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult MachineryReportDetails(ARRRReports packageAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;

                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);

                }
            }
            catch
            {
                return View(packageAgreement);
            }
        }


        //public ActionResult PopulateYearsForARRRDetails(FormCollection frmCollection)
        //{
        //    CommonFunctions objCommonFunctions = new CommonFunctions();
        //    List<SelectListItem> list = PopulateYearsForARRR(Convert.ToInt32(frmCollection["StateCode"]), true);//objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
        //    list.Find(x => x.Value == "-1").Value = "0";
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}



        [HttpPost]
        public JsonResult PopulateFinancialYearsByStateForARRRMaterial()
        {
            try
            {

                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                return Json(PopulateFinancialYearsByStateForARRRDetailsMaterial(stateCode, false));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateFinancialYearsByStateForARRRMaterial()");
                return null;
            }
        }

        public List<SelectListItem> PopulateFinancialYearsByStateForARRRDetailsMaterial(int stateCode, bool isAllSelected = false)
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            SelectListItem item;

            try
            {
                dbContext = new PMGSYEntities();

                var query = (from ISP in dbContext.MASTER_ARRR_STATES_RATES
                             where ISP.MAST_STATE_CODE == stateCode && ISP.MAST_LMM_TYPE == 3//ISP.MAST_LMM_TYPE_CODE == 3


                             select new
                             {
                                 Text = ISP.MAST_ARRR_YEAR,
                                 Value = ISP.MAST_ARRR_YEAR,

                             }).OrderByDescending(c => c.Text).ToList().Distinct();


                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text + "-" + (data.Text + 1);
                    item.Value = data.Value.ToString();

                    lstYears.Add(item);
                }

                if (isAllSelected == false)
                {
                    lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstYears.Insert(0, (new SelectListItem { Text = "All Years", Value = "0", Selected = true }));
                }

                return lstYears;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateFinancialYearsByStateForARRRDetailsMaterial()");
                return lstYears;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        [HttpPost]
        public JsonResult PopulateFinancialYearsByStateForARRRMachine()
        {
            try
            {

                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                return Json(PopulateFinancialYearsByStateForARRRDetailsMachine(stateCode, false));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateFinancialYearsByStateForARRRMachine()");
                return null;
            }
        }

        public List<SelectListItem> PopulateFinancialYearsByStateForARRRDetailsMachine(int stateCode, bool isAllSelected = false)
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            SelectListItem item;

            try
            {
                dbContext = new PMGSYEntities();

                var query = (from ISP in dbContext.MASTER_ARRR_STATES_RATES
                             where ISP.MAST_STATE_CODE == stateCode && ISP.MAST_LMM_TYPE == 2//ISP.MAST_LMM_TYPE_CODE==2


                             select new
                             {
                                 Text = ISP.MAST_ARRR_YEAR,
                                 Value = ISP.MAST_ARRR_YEAR,

                             }).OrderByDescending(c => c.Text).ToList().Distinct();


                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text + "-" + (data.Text + 1);
                    item.Value = data.Value.ToString();

                    lstYears.Add(item);
                }

                if (isAllSelected == false)
                {
                    lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstYears.Insert(0, (new SelectListItem { Text = "All Years", Value = "0", Selected = true }));
                }

                return lstYears;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateFinancialYearsByStateForARRRDetailsMachine()");
                return lstYears;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        [HttpPost]
        public JsonResult PopulateFinancialYearsByStateForARRRLabour()
        {
            try
            {

                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                return Json(PopulateFinancialYearsByStateForARRRDetailsLabour(stateCode, false));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateFinancialYearsByStateForARRRLabour()");
                return null;
            }
        }

        public List<SelectListItem> PopulateFinancialYearsByStateForARRRDetailsLabour(int stateCode, bool isAllSelected = false)
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            SelectListItem item;

            try
            {
                dbContext = new PMGSYEntities();

                var query = (from ISP in dbContext.MASTER_ARRR_STATES_RATES
                             where ISP.MAST_STATE_CODE == stateCode && ISP.MAST_LMM_TYPE == 1 // ISP.MAST_LMM_TYPE_CODE == 1


                             select new
                             {
                                 Text = ISP.MAST_ARRR_YEAR,
                                 Value = ISP.MAST_ARRR_YEAR,

                             }).OrderByDescending(c => c.Text).ToList().Distinct();


                // Below is commented as per suggestion of Srinivasa sir to populate all  years 14 Jan 2021

                //var query = (from ISP in dbContext.IMS_SANCTIONED_PROJECTS
                //             where ISP.MAST_STATE_CODE == stateCode &&
                //             ISP.STA_SANCTIONED == "Y" &&
                //             ISP.IMS_SANCTIONED == "N" &&
                //             ISP.IMS_ISCOMPLETED == "S"
                //             select new
                //             {
                //                 Text = ISP.IMS_YEAR,
                //                 Value = ISP.IMS_YEAR,

                //             }).OrderByDescending(c => c.Text).ToList().Distinct();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text + "-" + (data.Text + 1);
                    item.Value = data.Value.ToString();

                    lstYears.Add(item);
                }

                if (isAllSelected == false)
                {
                    lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstYears.Insert(0, (new SelectListItem { Text = "All Years", Value = "0", Selected = true }));
                }

                return lstYears;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateFinancialYearsByStateForARRRDetailsLabour()");
                return lstYears;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion
    }
}
