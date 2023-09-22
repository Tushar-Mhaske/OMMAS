using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Areas.StateProfileReports.Models;
using PMGSY.Extensions;
using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Controllers;
using System.IO;
using System.Configuration;

namespace PMGSY.Areas.StateProfileReports.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class StateProfileReportsController : Controller
    {
        //
        // GET: /StateProfileReports/StateProfileReports/

        [RequiredAuthentication]
        [RequiredAuthorization]
       

        [HttpGet]
        public ActionResult LoadFilters()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            SPFilters spFiltersModel = new SPFilters();
            try
            {
                //spFiltersModel.StateId = Convert.ToInt32(Request.Cookies["globalStateCode"] == null ? "-1" : Request.Cookies["globalStateCode"].Value);
                ////spFiltersModel.DistrictId = Convert.ToInt32(Request.Cookies["globalDistrictCode"] == null ? "-1" : Request.Cookies["globalDistrictCode"].Value);
                //spFiltersModel.StateList = objCommonFunctions.PopulateStates(false);

                if (PMGSYSession.Current.RoleCode == 2)
                {
                    spFiltersModel.StateList = new List<SelectListItem>();


                    spFiltersModel.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
                }
                else
                {
                    spFiltersModel.StateList = objCommonFunctions.PopulateStates(false);
                    spFiltersModel.StateList.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
                }
                //spFiltersModel.StateId > 0
                if (PMGSYSession.Current.RoleCode == 2)
                {
                    spFiltersModel.AgencyList = objCommonFunctions.PopulateAgencies(spFiltersModel.StateId, false);

                    spFiltersModel.Agency = Convert.ToInt32(spFiltersModel.AgencyList.Where(m => m.Selected == true).Select(m => m.Value).LastOrDefault());
                    spFiltersModel.AgencyList.RemoveAt(0);
                }
                else
                {
                    spFiltersModel.AgencyList = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();
                }
                spFiltersModel.DPIUList = new SelectList(PopulateDPIUs(0, 0, true), "Value", "Text").ToList();
                spFiltersModel.LevelCode = spFiltersModel.DPIUCode > 0 ? 2 : 1;

               // waModel.LevelCode = waModel.RoadWise == true ? 4 : waModel.BlockCode > 0 ? 3 : waModel.DistrictCode > 0 ? 2 : 1;
                return View(spFiltersModel);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Populate Agencies based on selected state
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PopulateAgencies()
        {
            try
            {
               // CommonFunctions objCommonFunctions = new CommonFunctions();
                

                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);

                return Json(PopulateAgencies(stateCode, false));
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        public List<SelectListItem> PopulateAgencies(int stateCode, bool isAllSelected = false)
        {
            List<SelectListItem> lstAgencies = new List<SelectListItem>();
            SelectListItem item;
            PMGSYEntities dbContext = null;

        //    select ma.MAST_AGENCY_CODE, ma.MAST_AGENCY_NAME from omms.MASTER_AGENCY ma
        //where ma.MAST_AGENCY_TYPE <>'G' and MAST_AGENCY_NAME in 
        //(select ad.ADMIN_ND_REMARKS  from omms.ADMIN_DEPARTMENT ad where MAST_STATE_CODE=16)

            try
            {
                dbContext = new PMGSYEntities();


                //var query = (from ma in dbContext.MASTER_AGENCY
                //             join md in dbContext.ADMIN_DEPARTMENT on ma.MAST_AGENCY_CODE equals md.MAST_AGENCY_CODE
                //             where md.MAST_STATE_CODE == stateCode &&
                //             md.MAST_ND_TYPE == "S" && ma.MAST_AGENCY_TYPE !="G"
                //             select new
                //             {
                //                 Text = md.ADMIN_ND_NAME,//ma.MAST_AGENCY_NAME,
                //                 Value = ma.MAST_AGENCY_CODE,
                //                 Selected = (ma.MAST_AGENCY_TYPE != "G" ? true : false)
                //             }).OrderBy(c => c.Text).ToList().Distinct();

                List<String> AgencyNames=dbContext.ADMIN_DEPARTMENT.Where(m=>m.MAST_STATE_CODE==stateCode).Select(s=>s.ADMIN_ND_REMARKS).ToList<String>();

                var query = (from ma in dbContext.MASTER_AGENCY
                             where ma.MAST_AGENCY_TYPE != "G"
                             && AgencyNames.Contains(ma.MAST_AGENCY_NAME)
                             select new
                             {
                                 Value=ma.MAST_AGENCY_CODE,
                                 Text=ma.MAST_AGENCY_NAME                                 
                             }).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    //item.Selected = data.Selected;
                    lstAgencies.Add(item);
                }

                if (isAllSelected == false)
                {
                    lstAgencies.Insert(0, (new SelectListItem { Text = "Select Agency", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstAgencies.Insert(0, (new SelectListItem { Text = "All Agencies", Value = "0", Selected = true }));
                }

                return lstAgencies;
            }
            catch
            {
                return lstAgencies;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ActionResult StateProfilePhaseSummaryReport(SPFilters spFiltersModel)
        {
            CommonFunctions commonFunctions = new CommonFunctions();
          
            return View(spFiltersModel);
        
        }

        [HttpPost]
        public JsonResult PopulateDPIU(int stateCode, int agencyCode)
        {
            try
            {
                return Json(PopulateDPIUs(stateCode, agencyCode, true), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        public List<SelectListItem> PopulateDPIUs(int stateCode, int agencyCode, bool isAllSelected = true)
        {
            List<SelectListItem> lstAgencies = new List<SelectListItem>();
            SelectListItem item;
            PMGSYEntities dbContext = null;
            try
            {
                dbContext = new PMGSYEntities();
                String AgencyName = dbContext.MASTER_AGENCY.Where(m => m.MAST_AGENCY_CODE == agencyCode).Select(m => m.MAST_AGENCY_NAME).FirstOrDefault();

                var query = (from ad in dbContext.ADMIN_DEPARTMENT
                             //join md in dbContext.ADMIN_DEPARTMENT on ma.MAST_AGENCY_CODE equals md.MAST_AGENCY_CODE
                             where ad.MAST_STATE_CODE == stateCode &&
                             ad.MAST_ND_TYPE=="D" &&
                             ad.ADMIN_ND_NAME.Contains(AgencyName)                            
                             select new
                             {
                                 Text = ad.ADMIN_ND_NAME,//ma.MAST_AGENCY_NAME,
                                 Value = ad.ADMIN_ND_CODE,
                                
                             }).OrderBy(c => c.Text).ToList().Distinct();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                   // item.Selected = data.Selected;
                    lstAgencies.Add(item);
                }

                if (isAllSelected == false)
                {
                    //lstAgencies.Insert(0, (new SelectListItem { Text = "Select Agency", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstAgencies.Insert(0, (new SelectListItem { Text = "All Agencies", Value = "0", Selected = true }));
                }

                return lstAgencies;
            }
            catch
            {
                return lstAgencies;
            }
            finally
            {
                dbContext.Dispose();
            }

        
        }


    }
}
