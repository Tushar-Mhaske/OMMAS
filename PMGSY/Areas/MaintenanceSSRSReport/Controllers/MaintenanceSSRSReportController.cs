using PMGSY.Areas.MaintenanceSSRSReport.Models;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.MaintenanceSSRSReport.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class MaintenanceSSRSReportController : Controller
    {
        //
        // GET: /MaintenanceSSRSReport/MaintenanceSSRSReport/

        public ActionResult Index()
        {
            return View();
        }

        #region 1 PackageWise Maintenance -- Done
        [HttpGet]
        public ActionResult PackageWiseMaintenanceReportLayout()
        {
            PackageWiseMaintenanceModel objPackageWiseMaintenanceModel = new PackageWiseMaintenanceModel();
            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult PackageWiseMaintenanceReport(PackageWiseMaintenanceModel objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objPackageWiseMaintenanceModel.LevelCode = objPackageWiseMaintenanceModel.RoadWise == true ? 4 : objPackageWiseMaintenanceModel.BlockCode > 0 ? 3 : objPackageWiseMaintenanceModel.DistrictCode > 0 ? 2 : 1;
                    objPackageWiseMaintenanceModel.Mast_State_Code = objPackageWiseMaintenanceModel.StateCode > 0 ? objPackageWiseMaintenanceModel.StateCode : objPackageWiseMaintenanceModel.Mast_State_Code;
                    objPackageWiseMaintenanceModel.Mast_District_Code = objPackageWiseMaintenanceModel.DistrictCode > 0 ? objPackageWiseMaintenanceModel.DistrictCode : objPackageWiseMaintenanceModel.Mast_District_Code;
                    objPackageWiseMaintenanceModel.Mast_Block_Code = objPackageWiseMaintenanceModel.BlockCode > 0 ? objPackageWiseMaintenanceModel.BlockCode : objPackageWiseMaintenanceModel.Mast_Block_Code;

                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    return View("PackageWiseMaintenanceReportLayout", objPackageWiseMaintenanceModel);
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }
        #endregion

        #region 2 Maintenance Commitment -- Done
        [HttpGet]
        public ActionResult CommitmentMaintenanceReportLayout()
        {
            MaintenanceCommitmentModel objMaintenanceCommitmentModel = new MaintenanceCommitmentModel();
            return View(objMaintenanceCommitmentModel);
        }

        [HttpPost]
        public ActionResult CommitmentMaintenanceReport(MaintenanceCommitmentModel objMaintenanceCommitmentModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objMaintenanceCommitmentModel.LevelCode = objMaintenanceCommitmentModel.RoadWise == true ? 4 : objMaintenanceCommitmentModel.BlockCode > 0 ? 3 : objMaintenanceCommitmentModel.DistrictCode > 0 ? 2 : 1;
                    objMaintenanceCommitmentModel.Mast_State_Code = objMaintenanceCommitmentModel.StateCode > 0 ? objMaintenanceCommitmentModel.StateCode : objMaintenanceCommitmentModel.Mast_State_Code;
                    objMaintenanceCommitmentModel.Mast_District_Code = objMaintenanceCommitmentModel.DistrictCode > 0 ? objMaintenanceCommitmentModel.DistrictCode : objMaintenanceCommitmentModel.Mast_District_Code;
                    objMaintenanceCommitmentModel.Mast_Block_Code = objMaintenanceCommitmentModel.BlockCode > 0 ? objMaintenanceCommitmentModel.BlockCode : objMaintenanceCommitmentModel.Mast_Block_Code;

                    return View(objMaintenanceCommitmentModel);
                }
                else
                {
                    return View("PackageWiseMaintenanceReportLayout", objMaintenanceCommitmentModel);
                }
            }
            catch
            {
                return View(objMaintenanceCommitmentModel);
            }

        }
        #endregion

        #region 3 Maintenance Inspection -- Done
        [HttpGet]
        public ActionResult MaintenanceInspectionReportLayout()
        {
            MaintenanceInspectionModel objMaintenanceInspectionModel = new MaintenanceInspectionModel();
            return View(objMaintenanceInspectionModel);
        }

        [HttpPost]
        public ActionResult MaintenanceInspectionReport(MaintenanceInspectionModel objMaintenanceInspectionModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objMaintenanceInspectionModel.LevelCode = objMaintenanceInspectionModel.RoadWise == true ? 4 : objMaintenanceInspectionModel.BlockCode > 0 ? 3 : objMaintenanceInspectionModel.DistrictCode > 0 ? 2 : 1;
                    objMaintenanceInspectionModel.Mast_State_Code = objMaintenanceInspectionModel.StateCode > 0 ? objMaintenanceInspectionModel.StateCode : objMaintenanceInspectionModel.Mast_State_Code;
                    objMaintenanceInspectionModel.Mast_District_Code = objMaintenanceInspectionModel.DistrictCode > 0 ? objMaintenanceInspectionModel.DistrictCode : objMaintenanceInspectionModel.Mast_District_Code;
                    objMaintenanceInspectionModel.Mast_Block_Code = objMaintenanceInspectionModel.BlockCode > 0 ? objMaintenanceInspectionModel.BlockCode : objMaintenanceInspectionModel.Mast_Block_Code;

                    return View(objMaintenanceInspectionModel);
                }
                else
                {
                    return View("MaintenanceInspectionReportLayout", objMaintenanceInspectionModel);
                }
            }
            catch
            {
                return View(objMaintenanceInspectionModel);
            }

        }

        #endregion

        #region 9 Routine Maintenance Priority List -- Done
        [HttpGet]
        public ActionResult RoutineMaintPriorityReportLayout()
        {
            RoutineMaintPriorityModel objRoutineMaintPriorityModel = new RoutineMaintPriorityModel();
            return View(objRoutineMaintPriorityModel);
        }

        [HttpPost]
        public ActionResult RoutineMaintPriorityReport(RoutineMaintPriorityModel objRoutineMaintPriorityModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objRoutineMaintPriorityModel.LevelCode = objRoutineMaintPriorityModel.RoadWise == true ? 4 : objRoutineMaintPriorityModel.BlockCode > 0 ? 3 : objRoutineMaintPriorityModel.DistrictCode > 0 ? 2 : 1;
                    objRoutineMaintPriorityModel.Mast_State_Code = objRoutineMaintPriorityModel.StateCode > 0 ? objRoutineMaintPriorityModel.StateCode : objRoutineMaintPriorityModel.Mast_State_Code;
                    objRoutineMaintPriorityModel.Mast_District_Code = objRoutineMaintPriorityModel.DistrictCode > 0 ? objRoutineMaintPriorityModel.DistrictCode : objRoutineMaintPriorityModel.Mast_District_Code;
                    objRoutineMaintPriorityModel.Mast_Block_Code = objRoutineMaintPriorityModel.BlockCode > 0 ? objRoutineMaintPriorityModel.BlockCode : objRoutineMaintPriorityModel.Mast_Block_Code;

                    return View(objRoutineMaintPriorityModel);
                }
                else
                {
                    return View("RoutineMaintPriorityReportLayout", objRoutineMaintPriorityModel);
                }
            }
            catch
            {
                return View(objRoutineMaintPriorityModel);
            }

        }

        #endregion

        #region 10 Road PCI for OverDue -- Done
        [HttpGet]
        public ActionResult RoadPCIOverdueMaintReportLayout()
        {
            RoadPCIOverdueModel objRoadPCIOverdueModel = new RoadPCIOverdueModel();
            return View(objRoadPCIOverdueModel);
        }

        [HttpPost]
        public ActionResult RoadPCIOverdueMaintReport(RoadPCIOverdueModel objRoadPCIOverdueModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objRoadPCIOverdueModel.LevelCode = objRoadPCIOverdueModel.RoadWise == true ? 4 : objRoadPCIOverdueModel.BlockCode > 0 ? 3 : objRoadPCIOverdueModel.DistrictCode > 0 ? 2 : 1;
                    objRoadPCIOverdueModel.Mast_State_Code = objRoadPCIOverdueModel.StateCode > 0 ? objRoadPCIOverdueModel.StateCode : objRoadPCIOverdueModel.Mast_State_Code;
                    objRoadPCIOverdueModel.Mast_District_Code = objRoadPCIOverdueModel.DistrictCode > 0 ? objRoadPCIOverdueModel.DistrictCode : objRoadPCIOverdueModel.Mast_District_Code;
                    objRoadPCIOverdueModel.Mast_Block_Code = objRoadPCIOverdueModel.BlockCode > 0 ? objRoadPCIOverdueModel.BlockCode : objRoadPCIOverdueModel.Mast_Block_Code;

                    return View(objRoadPCIOverdueModel);
                }
                else
                {
                    return View("RoadPCIOverdueMaintReportLayout", objRoadPCIOverdueModel);
                }
            }
            catch
            {
                return View(objRoadPCIOverdueModel);
            }

        }

        #endregion

        #region 11 Estimated Maintenance -- Done
        [HttpGet]
        public ActionResult EstimatedMaintenanceReportLayout()
        {
            EstimatedMaintenanceModel objEstimatedMaintenanceModel = new EstimatedMaintenanceModel();
            return View(objEstimatedMaintenanceModel);
        }

        [HttpPost]
        public ActionResult EstimatedMaintenanceReport(EstimatedMaintenanceModel objEstimatedMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objEstimatedMaintenanceModel.LevelCode = objEstimatedMaintenanceModel.RoadWise == true ? 4 : objEstimatedMaintenanceModel.BlockCode > 0 ? 3 : objEstimatedMaintenanceModel.DistrictCode > 0 ? 2 : 1;
                    objEstimatedMaintenanceModel.Mast_State_Code = objEstimatedMaintenanceModel.StateCode > 0 ? objEstimatedMaintenanceModel.StateCode : objEstimatedMaintenanceModel.Mast_State_Code;
                    objEstimatedMaintenanceModel.Mast_District_Code = objEstimatedMaintenanceModel.DistrictCode > 0 ? objEstimatedMaintenanceModel.DistrictCode : objEstimatedMaintenanceModel.Mast_District_Code;
                    objEstimatedMaintenanceModel.Mast_Block_Code = objEstimatedMaintenanceModel.BlockCode > 0 ? objEstimatedMaintenanceModel.BlockCode : objEstimatedMaintenanceModel.Mast_Block_Code;

                    return View(objEstimatedMaintenanceModel);
                }
                else
                {
                    return View("EstimatedMaintenanceReportLayout", objEstimatedMaintenanceModel);
                }
            }
            catch
            {
                return View(objEstimatedMaintenanceModel);
            }

        }

        #endregion

        #region 13- Asset Value Maintenance
        [HttpGet]
        public ActionResult MaintenanceAssetValueLayout()
        {
            AssetValueModel assetValueModel = new AssetValueModel();
            assetValueModel.PMGSY = PMGSY.Extensions.PMGSYSession.Current.PMGSYScheme;
            return View(assetValueModel);
        }
        [HttpPost]
        public ActionResult MaintenanceAssetValueReport(AssetValueModel assetValueModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    assetValueModel.PMGSY = PMGSY.Extensions.PMGSYSession.Current.PMGSYScheme;
                    return View(assetValueModel);
                }
                else
                {
                    return View("MaintenanceAssetValueLayout", assetValueModel);
                }
            }
            catch { return View(assetValueModel); }

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

        #region Phase Profile Report
        [HttpGet]
        public ActionResult PhaseProfileReportLayout()
        {
            PhaseProfileViewModel php = new PhaseProfileViewModel();
            CommonFunctions comm = new CommonFunctions();


            php.StateList = comm.PopulateStates(true);
            php.StateList.RemoveAt(0);
            php.StateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });
            //if (PMGSYSession.Current.StateCode > 0)
            //{
            //    EC.StateCode = PMGSYSession.Current.StateCode;
            //    EC.StateName = PMGSYSession.Current.StateName;
            //    EC.State_Name = PMGSYSession.Current.StateName;
            //    //EC.DistrictList = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();
            //    //EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
            //    EC.DistrictList = comm.PopulateDistrict(EC.StateCode, true);
            //    EC.DistrictList.RemoveAt(0);
            //    EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

            //    EC.AgencyCode = PMGSYSession.Current.AdminNdCode;
            //    EC.AgencyName = PMGSYSession.Current.DepartmentName.Trim();
            //    //EC.AgencyList = comm.PopulateAgencies(EC.StateCode, true);
            //    //EC.AgencyCode = Convert.ToInt32(EC.AgencyList.Where(m => m.Selected == true).Select(m => m.Value).LastOrDefault());
            //    //EC.AgencyList.RemoveAt(0);

            //    //EC.CollaborationCode = 2;
            //    EC.CollaborationList = PopulateCollaborationsStateWise(EC.StateCode, true);
            //    //EC.CollaborationList.RemoveAt(0);
            //}
            //else
            //{
            List<SelectListItem> lstDistricts = new List<SelectListItem>();
            lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });
            php.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

            List<SelectListItem> lstBlock = new List<SelectListItem>();
            lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
            php.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();

            List<SelectListItem> lstCollab = new List<SelectListItem>();
            lstCollab.Insert(0, new SelectListItem { Value = "-1", Text = "Select Collaboration" });
            php.CollabList = new SelectList(lstCollab, "Value", "Text").ToList();

            List<SelectListItem> lstAgency = new List<SelectListItem>();
            lstAgency.Insert(0, new SelectListItem { Value = "-1", Text = "Select Agency" });
            php.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
            //}

            php.MonthList = comm.PopulateMonths(true);

            php.YearList = comm.PopulateFinancialYear(true, false).ToList();
            //php.YearList.RemoveAt(0);
            //EC.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });


            return View(php);
        }

        [HttpPost]
        public ActionResult PhaseProfileReport(PhaseProfileViewModel php)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //php.LevelCode = php.RoadWise == true ? 4 : objPackageWiseMaintenanceModel.BlockCode > 0 ? 3 : objPackageWiseMaintenanceModel.DistrictCode > 0 ? 2 : 1;
                    php.Level = 1;
                    //php.StateCode = php.StateCode > 0 ? php.StateCode : php.StateCode;
                    //php.DistrictCode = php.DistrictCode > 0 ? php.DistrictCode : php.DistrictCode;
                    //php.BlockCode = php.BlockCode > 0 ? php.BlockCode : php.BlockCode;

                    return View(php);
                }
                else
                {
                    return View("PhaseProfileReportLayout", php);
                }
            }
            catch
            {
                return View(php);
            }

        }


        public ActionResult PopulateDistricts(string param)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(param), true);
            //list.Find(x => x.Value == "-1").Value = "0";
            list.RemoveAt(0);
            list.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PopulateBlocks(string param)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(param), true);
            list.RemoveAt(0);
            list.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PopulateAgencies(string param)
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                int stateCode = Convert.ToInt32(param);

                return Json(comm.PopulateAgencies(stateCode, true));
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }


        [HttpPost]
        public JsonResult PopulateCollaborations(string param)
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int stateCode = Convert.ToInt32(param.Trim());
                List<SelectListItem> list = PopulateCollaborationsStateWise(stateCode, true);
                return Json(list);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }
        public List<SelectListItem> PopulateCollaborationsStateWise(int stateCode, bool isAllSelected = false)
        {
            List<SelectListItem> lstCollaborations = new List<SelectListItem>();
            SelectListItem item;
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                //int stateCode = Convert.ToInt32(param.Trim());

                if (isAllSelected == false)
                {
                    lstCollaborations.Insert(0, (new SelectListItem { Text = "Select Collaboration", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstCollaborations.Insert(0, (new SelectListItem { Text = "All Collaborations", Value = "0", Selected = true }));
                }

                var query = (from ms in dbContext.MASTER_FUNDING_AGENCY
                             join isp in dbContext.IMS_SANCTIONED_PROJECTS on ms.MAST_FUNDING_AGENCY_CODE equals isp.IMS_COLLABORATION
                             where isp.MAST_STATE_CODE == stateCode
                             select new
                             {
                                 Text = ms.MAST_FUNDING_AGENCY_NAME,
                                 Value = ms.MAST_FUNDING_AGENCY_CODE
                             }).Distinct().OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstCollaborations.Add(item);
                }



                return lstCollaborations;
            }
            catch
            {
                return lstCollaborations;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Maintenance Incentive Length Expenditure Report
        [HttpGet]
        public ActionResult MaintenanceIncentiveLengthExpenditureLayout()
        {
            MaintenanceIncentiveLenExpViewModel model = new MaintenanceIncentiveLenExpViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSY.Extensions.PMGSYSession.Current.StateCode == 0)
                {
                    model.StateList = comm.PopulateStates(true);
                    model.StateList.Find(x => x.Value == "0").Text = "All States";
                }
                else if (PMGSYSession.Current.StateCode > 0)
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceSSRS/MaintenanceSSRS/MaintenanceIncentiveLengthExpenditureLayout");
                return null;
            }
        }

        [HttpPost]
        public ActionResult MaintenanceIncentiveLengthExpenditureReport(MaintenanceIncentiveLenExpViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Level = 1;

                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceSSRS/MaintenanceSSRS/MaintenanceIncentiveLengthExpenditureReport");
                return null;
            }

        }
        #endregion

        #region Maintenance statewise , districtwise  30 Jan  2020

        [HttpGet]
        public ActionResult MaintennaceStatewiselayout()
        {
            MaintenanceStatewise objPackageWiseMaintenanceModel = new MaintenanceStatewise();
            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult MaintennaceStatewiseReport(MaintenanceStatewise objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objPackageWiseMaintenanceModel.LevelCode = objPackageWiseMaintenanceModel.StateCode > 0 ? 1 : 1;
                    objPackageWiseMaintenanceModel.Mast_State_Code = objPackageWiseMaintenanceModel.StateCode > 0 ? objPackageWiseMaintenanceModel.StateCode : objPackageWiseMaintenanceModel.Mast_State_Code;

                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    return View("MaintennaceStatewiselayout", objPackageWiseMaintenanceModel);
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }


        #endregion

        #region Maintenance Incentive Report
        [HttpGet]
        public ActionResult MaintenanceIncentiveLayout()
        {
            MaintenanceIncentiveViewModel model = new MaintenanceIncentiveViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSY.Extensions.PMGSYSession.Current.StateCode == 0)
                {
                    model.StateList = comm.PopulateStates(true);
                    model.StateList.Find(x => x.Value == "0").Text = "All States";
                }
                else if (PMGSYSession.Current.StateCode > 0)
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                }
                model.YearList = comm.PopulateYears(true);
                model.YearList.Find(x => x.Value == "0").Value = "-1";
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceSSRS/MaintenanceSSRS/MaintenanceIncentiveLayout");
                return null;
            }
        }

        [HttpPost]
        public ActionResult MaintenanceIncentiveReport(MaintenanceIncentiveViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceSSRS/MaintenanceSSRS/MaintenanceIncentiveReport");
                return null;
            }
        }
        #endregion

        #region Maintenance Report 28 Feb 2020
        [HttpGet]
        public ActionResult MaintenanceExpenditureLayout()
        {
            MaintenanceExpenditureModel objPackageWiseMaintenanceModel = new MaintenanceExpenditureModel();

            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult MaintenanceExpenditureReport(MaintenanceExpenditureModel objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //objPackageWiseMaintenanceModel.LevelCode = objPackageWiseMaintenanceModel.StateCode == 0 ? 1 : (objPackageWiseMaintenanceModel.DistrictCode==);
                    objPackageWiseMaintenanceModel.Mast_State_Code = objPackageWiseMaintenanceModel.StateCode > 0 ? objPackageWiseMaintenanceModel.StateCode : objPackageWiseMaintenanceModel.Mast_State_Code;
                    objPackageWiseMaintenanceModel.Mast_District_Code = objPackageWiseMaintenanceModel.DistrictCode > 0 ? objPackageWiseMaintenanceModel.DistrictCode : objPackageWiseMaintenanceModel.Mast_District_Code;
                    objPackageWiseMaintenanceModel.Mast_Block_Code = objPackageWiseMaintenanceModel.BlockCode > 0 ? objPackageWiseMaintenanceModel.BlockCode : objPackageWiseMaintenanceModel.Mast_Block_Code;

                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    return View("MaintenanceExpenditureLayout", objPackageWiseMaintenanceModel);
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }

        public ActionResult BlockDetailsForExp(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DistrictSelectDetailsForExp(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Award Details 
        [HttpGet]
        public ActionResult AwradwiseZeroPaymentReportLayout()
        {
            AwardwisePayment objPackageWiseMaintenanceModel = new AwardwisePayment();
            objPackageWiseMaintenanceModel.schemeList = new List<SelectListItem>();
            objPackageWiseMaintenanceModel.schemeList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            objPackageWiseMaintenanceModel.schemeList.Insert(1, new SelectListItem { Text = "PMGSY 1", Value = "1" });
            objPackageWiseMaintenanceModel.schemeList.Insert(2, new SelectListItem { Text = "PMGSY 2", Value = "2" });
            objPackageWiseMaintenanceModel.schemeList.Insert(3, new SelectListItem { Text = "RCPLWE", Value = "3" });


            objPackageWiseMaintenanceModel.AwardCodeList = new List<SelectListItem>();
            objPackageWiseMaintenanceModel.AwardCodeList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            objPackageWiseMaintenanceModel.AwardCodeList.Insert(1, new SelectListItem { Text = "AWARDED", Value = "1" });
            objPackageWiseMaintenanceModel.AwardCodeList.Insert(2, new SelectListItem { Text = "UNAWARDED", Value = "2" });


            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult AwardwisezeroPaymentReport(AwardwisePayment objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("AwradwiseZeroPaymentReportLayout", objPackageWiseMaintenanceModel);
                    return null;
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }
        #endregion

        #region Award Status Report 21 April 2020
        [HttpGet]
        public ActionResult PmgsyAwardStatusLayout()
        {
            AwardStatus php = new AwardStatus();
            CommonFunctions comm = new CommonFunctions();

            //php.StateList = comm.PopulateStates(true);
            //php.StateList.RemoveAt(0);
            //php.StateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });

            php.schemeList = new List<SelectListItem>();
            php.schemeList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            php.schemeList.Insert(1, new SelectListItem { Text = "PMGSY 1", Value = "1" });
            php.schemeList.Insert(2, new SelectListItem { Text = "PMGSY 2", Value = "2" });
            php.schemeList.Insert(3, new SelectListItem { Text = "RCPLWE", Value = "3" });
            php.schemeList.Insert(4, new SelectListItem { Text = "PMGSY 3", Value = "4" });

            //List<SelectListItem> lstDistricts = new List<SelectListItem>();
            //lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });
            //php.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

            //List<SelectListItem> lstBlock = new List<SelectListItem>();
            //lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
            //php.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();

            //List<SelectListItem> lstCollab = new List<SelectListItem>();
            //lstCollab.Insert(0, new SelectListItem { Value = "-1", Text = "Select Collaboration" });
            //php.CollabList = new SelectList(lstCollab, "Value", "Text").ToList();

            //List<SelectListItem> lstAgency = new List<SelectListItem>();
            //lstAgency.Insert(0, new SelectListItem { Value = "-1", Text = "Select Agency" });
            //php.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
         

            //php.MonthList = comm.PopulateMonths(true);

            //php.YearList = comm.PopulateFinancialYear(true, false).ToList();
           
            return View(php);
        }

        [HttpPost]
        public ActionResult PmgsyAwardStatusReport(AwardStatus php)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    return View(php);
                }
                else
                {
                    return View("PmgsyAwardStatusReport", php);
                }
            }
            catch
            {
                return View(php);
            }

        }
        #endregion




        #region Maintenance Report 27 April 2020
        [HttpGet]
        public ActionResult MaintenanceWorksLayout()
        {
            MaintenanceExpenditureModel objPackageWiseMaintenanceModel = new MaintenanceExpenditureModel();

            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult MaintenanceWorksReport(MaintenanceExpenditureModel objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //objPackageWiseMaintenanceModel.LevelCode = objPackageWiseMaintenanceModel.StateCode == 0 ? 1 : (objPackageWiseMaintenanceModel.DistrictCode==);
                    objPackageWiseMaintenanceModel.Mast_State_Code = objPackageWiseMaintenanceModel.StateCode > 0 ? objPackageWiseMaintenanceModel.StateCode : objPackageWiseMaintenanceModel.Mast_State_Code;
                    objPackageWiseMaintenanceModel.Mast_District_Code = objPackageWiseMaintenanceModel.DistrictCode > 0 ? objPackageWiseMaintenanceModel.DistrictCode : objPackageWiseMaintenanceModel.Mast_District_Code;
                    objPackageWiseMaintenanceModel.Mast_Block_Code = objPackageWiseMaintenanceModel.BlockCode > 0 ? objPackageWiseMaintenanceModel.BlockCode : objPackageWiseMaintenanceModel.Mast_Block_Code;

                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    return View("MaintenanceWorksLayout", objPackageWiseMaintenanceModel);
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }

      

        #endregion


        #region Aging Report
        [HttpGet]
        public ActionResult AgingLayout()
        {
            Aging objPackageWiseMaintenanceModel = new Aging();



            objPackageWiseMaintenanceModel.DelayGroupList = new List<SelectListItem>();
            objPackageWiseMaintenanceModel.DelayGroupList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            objPackageWiseMaintenanceModel.DelayGroupList.Insert(1, new SelectListItem { Text = "Less Than 1", Value = "1" });
            objPackageWiseMaintenanceModel.DelayGroupList.Insert(2, new SelectListItem { Text = "Between 1 and 2", Value = "2" });
            objPackageWiseMaintenanceModel.DelayGroupList.Insert(3, new SelectListItem { Text = "Between 2 and 3", Value = "3" });
            objPackageWiseMaintenanceModel.DelayGroupList.Insert(4, new SelectListItem { Text = "Between 3 and 4", Value = "4" });
            objPackageWiseMaintenanceModel.DelayGroupList.Insert(5, new SelectListItem { Text = "Between 4 and 5", Value = "5" });
            objPackageWiseMaintenanceModel.DelayGroupList.Insert(6, new SelectListItem { Text = "Between 5 and 6", Value = "6" });
            objPackageWiseMaintenanceModel.DelayGroupList.Insert(7, new SelectListItem { Text = "Between 6 and 7", Value = "7" });
            objPackageWiseMaintenanceModel.DelayGroupList.Insert(8, new SelectListItem { Text = "Between 7 and 8", Value = "8" });
            objPackageWiseMaintenanceModel.DelayGroupList.Insert(9, new SelectListItem { Text = "Between 8 and 9", Value = "9" });
            objPackageWiseMaintenanceModel.DelayGroupList.Insert(10, new SelectListItem { Text = "Between 9 and 10", Value = "10" });
            objPackageWiseMaintenanceModel.DelayGroupList.Insert(11, new SelectListItem { Text = "More Than 11", Value = "11" });


            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult AgingReport(Aging objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //objPackageWiseMaintenanceModel.LevelCode = objPackageWiseMaintenanceModel.StateCode == 0 ? 1 : (objPackageWiseMaintenanceModel.DistrictCode==);
                    objPackageWiseMaintenanceModel.Mast_State_Code = objPackageWiseMaintenanceModel.StateCode > 0 ? objPackageWiseMaintenanceModel.StateCode : objPackageWiseMaintenanceModel.Mast_State_Code;
                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    return View("AgingLayout", objPackageWiseMaintenanceModel);
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }
        #endregion

        #region RCPLWE BALANCE REPORT
        [HttpGet]
        public ActionResult RcplweBalanceLayout()
        {
            AwardwisePayment objPackageWiseMaintenanceModel = new AwardwisePayment();

            objPackageWiseMaintenanceModel.PhaseYearList.RemoveAt(0);
            objPackageWiseMaintenanceModel.MonthList.RemoveAt(0);

            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult RcplweBalanceReport(AwardwisePayment objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("AwradwiseZeroPaymentReportLayout", objPackageWiseMaintenanceModel);
                    return null;
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }
        #endregion

        #region Monthly Review Report
        [HttpGet]
        public ActionResult MonthlyReviewLayout()
        {
            AwardwisePayment objPackageWiseMaintenanceModel = new AwardwisePayment();

            objPackageWiseMaintenanceModel.PhaseYearList.RemoveAt(0);
            objPackageWiseMaintenanceModel.MonthList.RemoveAt(0);

            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult MonthlyReviewReport(AwardwisePayment objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("AwradwiseZeroPaymentReportLayout", objPackageWiseMaintenanceModel);
                    return null;
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }
        #endregion

        #region Monthly Action Plan
        [HttpGet]
        public ActionResult MonthlyActionLayout()
        {
            AwardwisePayment objPackageWiseMaintenanceModel = new AwardwisePayment();

            objPackageWiseMaintenanceModel.PhaseYearList.RemoveAt(0);
            objPackageWiseMaintenanceModel.MonthList.RemoveAt(0);

            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult MonthlyActionReport(AwardwisePayment objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("AwradwiseZeroPaymentReportLayout", objPackageWiseMaintenanceModel);
                    return null;
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }
        #endregion

        #region PMGSY III BALANCE REPORT
        [HttpGet]
        public ActionResult PmgsyBalanceLayout()
        {
            AwardwisePayment objPackageWiseMaintenanceModel = new AwardwisePayment();

            objPackageWiseMaintenanceModel.PhaseYearList.RemoveAt(0);
            objPackageWiseMaintenanceModel.MonthList.RemoveAt(0);

            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult PmgsyBalanceReport(AwardwisePayment objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("AwradwiseZeroPaymentReportLayout", objPackageWiseMaintenanceModel);
                    return null;
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }
        #endregion


        #region DLP 3 RDL  BALANCE REPORT
        [HttpGet]
        public ActionResult DLPExpLayout()
        {

            CommonFunctions commonFunctions = new CommonFunctions();
            DLPExpenditureModel objPackageWiseMaintenanceModel = new DLPExpenditureModel();


            objPackageWiseMaintenanceModel.schemeList = commonFunctions.PopulateScheme();

            // objPackageWiseMaintenanceModel.StateList = commonFunctions.PopulateStates();


            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult DLPExpReport(DLPExpenditureModel objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("AwradwiseZeroPaymentReportLayout", objPackageWiseMaintenanceModel);
                    return null;
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }
        #endregion

        #region Renewal Report
        [HttpGet]
        public ActionResult RenewalLayout()
        {

            CommonFunctions commonFunctions = new CommonFunctions();
            NewRenewaldetails objPackageWiseMaintenanceModel = new NewRenewaldetails();

            if (PMGSYSession.Current.StateCode > 0)
            {
                objPackageWiseMaintenanceModel.StateList = new List<SelectListItem>();
                objPackageWiseMaintenanceModel.StateList.Clear();
                objPackageWiseMaintenanceModel.StateList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
            }


            objPackageWiseMaintenanceModel.schemeList = commonFunctions.PopulateScheme();

            // objPackageWiseMaintenanceModel.StateList = commonFunctions.PopulateStates();


            return View(objPackageWiseMaintenanceModel);
        }


        [HttpPost]
        public ActionResult RenewalReport(NewRenewaldetails objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("AwradwiseZeroPaymentReportLayout", objPackageWiseMaintenanceModel);
                    return null;
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }
        #endregion

        #region Lab Report
        [HttpGet]
        public ActionResult LabLayout()
        {

            CommonFunctions commonFunctions = new CommonFunctions();
            DLPExpenditureModel objPackageWiseMaintenanceModel = new DLPExpenditureModel();


            objPackageWiseMaintenanceModel.schemeList = commonFunctions.PopulateScheme();

            // objPackageWiseMaintenanceModel.StateList = commonFunctions.PopulateStates();


            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult LabReport(DLPExpenditureModel objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("AwradwiseZeroPaymentReportLayout", objPackageWiseMaintenanceModel);
                    return null;
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }
        #endregion


        #region State Matrix Report
        [HttpGet]
        public ActionResult StateMatrixLayout()
        {
            AwardwisePayment objPackageWiseMaintenanceModel = new AwardwisePayment();

            objPackageWiseMaintenanceModel.PhaseYearList.RemoveAt(0);
            objPackageWiseMaintenanceModel.MonthList.RemoveAt(0);

            return View(objPackageWiseMaintenanceModel);
        }

        [HttpPost]
        public ActionResult StateatrixReport(AwardwisePayment objPackageWiseMaintenanceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    return View(objPackageWiseMaintenanceModel);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("AwradwiseZeroPaymentReportLayout", objPackageWiseMaintenanceModel);
                    return null;
                }
            }
            catch
            {
                return View(objPackageWiseMaintenanceModel);
            }

        }
        #endregion




    }
}
