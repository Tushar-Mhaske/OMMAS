#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ReportsLayoutController.cs        
        * Description   :   Main layout for DataEntry Reports
        * Author        :   Shyam Yadav 
        * Creation Date :   26/August/2013    
 **/
#endregion

using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.ReportsLayout;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class ReportsLayoutController : Controller
    {
        
        Models.PMGSYEntities dbContext;
        
        #region Layout
        [Audit]
        public ActionResult ReportsLayout()
        {
            return View();
        }

        /// <summary>
        /// Render Common Filters for all reports
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ReportsFilters()
        {
            ReportsFiltersViewModel reportsViewModel = new ReportsFiltersViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();


            reportsViewModel.STATES = objCommonFuntion.PopulateStates();
            reportsViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode != 0 ? PMGSYSession.Current.StateCode : 0);
            reportsViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode != 0 ? PMGSYSession.Current.DistrictCode : 0, false);

            reportsViewModel.Years = objCommonFuntion.PopulateFinancialYear(false, true).ToList();
            reportsViewModel.Months = objCommonFuntion.PopulateMonths(true);

            reportsViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
            reportsViewModel.BATCHS = objCommonFuntion.PopulateBatch();
            reportsViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();

            reportsViewModel.YEAR = DateTime.Now.Year;
            reportsViewModel.MONTH = DateTime.Now.Month;

            return View(reportsViewModel);
        }


        /// <summary>
        /// Render Reports Menu from Database table
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ReportsMenu()
        {
            dbContext = new PMGSYEntities();
            
            ReportMenuListModel rptMenuListModel = new ReportMenuListModel();
            try
            {
                List<UM_Rpt_Menu_Master> lst = (from rpt in dbContext.UM_Rpt_Menu_Master
                                               where rpt.RoleId == PMGSYSession.Current.RoleCode
                                               select rpt).OrderBy(c => c.ParentId).OrderBy(c => c.VerticalLevel).OrderBy(c => c.Sequence).ToList<UM_Rpt_Menu_Master>();

                List<ReportMenuMasterModel> reportParentMenuList = new List<ReportMenuMasterModel>();
                List<ReportMenuMasterModel> reportChildMenuList = new List<ReportMenuMasterModel>();
                
                foreach (var item in lst)
                {
                    ReportMenuMasterModel rptMenuModel = new ReportMenuMasterModel();
                    rptMenuModel.RoleId = item.RoleId;
                    rptMenuModel.MenuId = item.MenuId;
                    rptMenuModel.MenuName = item.MenuName;
                    rptMenuModel.ParentId = item.ParentId;
                    rptMenuModel.VerticalLevel = item.VerticalLevel;
                    rptMenuModel.Sequence = item.Sequence;
                    
                    rptMenuModel.Controller = item.Controller==null?"":item.Controller;
                    rptMenuModel.Action = item.Action == null ? "" : item.Action;
                    
                    if (rptMenuModel.ParentId == 0)
                    {
                        reportParentMenuList.Add(rptMenuModel);
                    }
                    else
                    {
                        reportChildMenuList.Add(rptMenuModel);
                    }
                }

                rptMenuListModel.ReportParentMenusList = reportParentMenuList;
                rptMenuListModel.ReportChildMenusList = reportChildMenuList;
                
                return View(rptMenuListModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;

            }


            
        }

        #endregion


        #region Populate Filters

        /// <summary>
        /// Populate Districts according to selected State
        /// </summary>
        /// <param name="selectedState"></param>
        /// <param name="isAllSelected"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult PopulateDistricts(int selectedValue, bool isAllSelected)
        {
            try
            {
                return Json(new CommonFunctions().PopulateDistrict(selectedValue, isAllSelected));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Populate Blocks according to selected Districts
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <param name="isAllSelected"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult PopulateBlocks(int selectedValue, bool isAllSelected)
        {
            try
            {
                return Json(new CommonFunctions().PopulateBlocks(selectedValue, isAllSelected));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

    }
}
