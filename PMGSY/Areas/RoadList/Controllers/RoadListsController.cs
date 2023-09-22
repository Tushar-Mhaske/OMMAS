using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Areas.RoadList.Models;
using PMGSY.Models;
using PMGSY.Controllers;
namespace PMGSY.Areas.RoadList.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class RoadListsController : Controller
    {
        //
        // GET: /RoadList/RoadLists/

        public ActionResult Index()
        {
            return View();
        }

        #region QM Quality Profile SSRS Report

        public ActionResult MPRoadListLayout()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            RoadListsModel qmphase = new RoadListsModel();
            if (PMGSYSession.Current.RoleCode == 2)
            {
                qmphase.lstStates = new List<SelectListItem>();
                qmphase.lstStates.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
                qmphase.lstStates.Insert(1, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
            }
            else
            {
                qmphase.lstStates = commonFunctions.PopulateStates(true);
               // qmphase.lstStates.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
            }
            qmphase.lstMPConst = PopulateMPConstituencyList(0);

            return View(qmphase);
        }

        [HttpPost]
        public ActionResult MPRoadListReport(RoadListsModel qmphaseProgress)
        {
            try
            {
                if (qmphaseProgress.StateCode == 0 || qmphaseProgress.StateCode > 0)
                {
                    //  qmphaseProgress.LevelID = 1;
                }
                else
                {
                    //   qmphaseProgress.LevelID = 2;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(qmphaseProgress);

        }

        public JsonResult PopulateMPConstituency(int id)
        {
            //int 

            List<SelectListItem> lstMPConst = PopulateMPConstituencyList(id);
            return Json(new SelectList(lstMPConst, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }


        public List<SelectListItem> PopulateMPConstituencyList(int StateCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                List<SelectListItem> lstMPConst = new List<SelectListItem>();
                lstMPConst =new SelectList(dbContext.MASTER_MP_CONSTITUENCY.Where(m => m.MAST_STATE_CODE == StateCode && m.MAST_MP_CONST_ACTIVE == "Y"), "MAST_MP_CONST_CODE", "MAST_MP_CONST_NAME").ToList();
                lstMPConst.Insert(0, new SelectListItem { Text="Select",Value="0"});
                return lstMPConst;
            }
            catch 
            {                
                throw;
            }
        
        }

        #endregion




    }
}
