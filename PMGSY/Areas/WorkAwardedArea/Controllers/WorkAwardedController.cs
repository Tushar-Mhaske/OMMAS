using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Areas.WorkAwardedArea.Models;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Controllers;
namespace PMGSY.Areas.WorkAwardedArea.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class WorkAwardedController : Controller
    {
        //
        // GET: /WorkAwardedArea/WorkAwarded/

        #region Tendering Agreement Details REPORT

        public ActionResult TendAgrDetailsLayout()
        {
            WorkAwardedModel waModel = new WorkAwardedModel();
            CommonFunctions commonFunctions = new CommonFunctions();
      
            waModel.YearList = commonFunctions.PopulateYears(System.DateTime.Now.Year);
            waModel.YearList.RemoveAt(0);
            waModel.YearList.Insert(0, (new SelectListItem { Text = "All Years", Value = "0", Selected = true }));
           
            waModel.CollabList = commonFunctions.PopulateFundingAgency(true);
            waModel.CollabList.RemoveAt(0);
            waModel.CollabList.Insert(0, (new SelectListItem { Text = "All Funding Agency", Value = "0", Selected = true }));

            #region Filtering Logic
            waModel.StateName = PMGSY.Extensions.PMGSYSession.Current.StateCode == 0 ? "0" : PMGSY.Extensions.PMGSYSession.Current.StateName.Trim();
            waModel.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();
            waModel.BlockName = "0";
            waModel.Mast_State_Code =PMGSYSession.Current.StateCode;
            waModel.Mast_District_Code = PMGSYSession.Current.DistrictCode;
            waModel.Mast_Block_Code = 0;
            waModel.LevelCode = 0 > 0 ? 3 : PMGSYSession.Current.DistrictCode> 0 ? 2 : 1;
            waModel.StateList = commonFunctions.PopulateStates(true);
            waModel.StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            waModel.StateList.Find(x => x.Value == waModel.StateCode.ToString()).Selected = true;

            waModel.DistrictList = new List<SelectListItem>();
            if (waModel.StateCode == 0)
            {
                waModel.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                waModel.DistrictList = commonFunctions.PopulateDistrict(waModel.StateCode, true);
                waModel.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ?-1 : PMGSYSession.Current.DistrictCode;
                waModel.DistrictList.Find(x => x.Value == waModel.DistrictCode.ToString()).Selected = true;

            }
            waModel.BlockList = new List<SelectListItem>();
            if (waModel.DistrictCode == 0)
            {
                waModel.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                waModel.BlockList = commonFunctions.PopulateBlocks(waModel.DistrictCode, true);
                waModel.BlockList.Find(m => m.Value == "-1").Value = "0";
                waModel.BlockCode = 0 == 0 ? 0 : 0;
                waModel.BlockList.Find(x => x.Value == waModel.BlockCode.ToString()).Selected = true;
            }
            #endregion

            return View(waModel);
        }

        public ActionResult TendAgrDetailsReport(WorkAwardedModel waModel)
        {
          
            try
            {
                if (ModelState.IsValid)
                {
                    waModel.LevelCode = waModel.StateCode > 0 ? 2 : 1;
                    waModel.Mast_State_Code = waModel.StateCode > 0 ? waModel.StateCode : waModel.Mast_State_Code;
                    waModel.Mast_District_Code = waModel.DistrictCode > 0 ? waModel.DistrictCode : waModel.Mast_District_Code;

                    waModel.LevelCode = waModel.RoadWise == true ? 4 : waModel.BlockCode > 0 ? 3 : waModel.DistrictCode > 0 ? 2 : 1;

                    waModel.StateName = waModel.StateCode == 0 ? "0" : waModel.StateName.Trim();
                    waModel.DistName = waModel.DistrictCode == 0 ? "0" : waModel.DistName.Trim();
                   
                    return View(waModel);
                }
                else
                {
                    string message = "";
                    //    bool flag = false;
                    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return View("TendAgrDetailsLayout", waModel);
                }
            }
            catch
            {
                return View(waModel);
            }
            //return View();
        }
        #endregion

        #region Common function

        public ActionResult DistrictDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
            // list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BlockDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), false);
            // list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AllDistrictDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
            // list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AllBlockDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
            // list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion




      

    }
}
