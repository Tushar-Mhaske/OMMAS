using PMGSY.Areas.GeneralReport.DAL;
using PMGSY.Areas.OtherReports.Models;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.OtherReports.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class OtherReportsController : Controller
    {
        //
        // GET: /OtherReports/OtherReports/
        #region Test Report

        public ActionResult TestReport()
        {
            try
            {
                TestModel ViewModel = new TestModel();
                ViewModel.StateCode = PMGSYSession.Current.StateCode;
                ViewModel.DistrictCode = PMGSYSession.Current.DistrictCode;

                int stateCode = PMGSYSession.Current.StateCode;
                int districtCode = PMGSYSession.Current.DistrictCode;

                ORDAL ORDal = new ORDAL();

                List<SelectListItem> stateList = ORDal.PopulateStates(true);
                //stateList.Find(x => x.Value == "1").Selected = true;
                stateList.Find(x => x.Value == "0").Value = "-1";

                List<SelectListItem> districtList = new List<SelectListItem>();
                List<SelectListItem> blockList = new List<SelectListItem>();
                List<SelectListItem> YearList = ORDal.PopulateYears();
                if (stateCode == 0)
                {
                    //foreach (SelectListItem i in ORDal.PopulateDistrict(1))
                    //{
                    //    districtList.Add(i);
                    //}
                    //districtList.Find(x => x.Value == "0").Text = "All District";


                    //blockList = ORDal.PopulateBlocks(districtCode);
                    //blockList.Find(x => x.Value == "0").Text = "All Blocks";

                    districtList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
                    blockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                }

                if (stateCode > 0 && districtCode > 0) //if piu Login
                {
                    SelectListItem item = stateList.Find(x => x.Value == stateCode.ToString());
                    stateList.Clear();
                    stateList.Add(item);

                    districtList = ORDal.PopulateDistrict(stateCode);
                    SelectListItem DistrictItem = districtList.Find(x => x.Value == districtCode.ToString());
                    districtList.Clear();
                    districtList.Add(DistrictItem);
                    blockList.Clear();
                    blockList = ORDal.PopulateBlocks(districtCode, true);
                }

                if (stateCode > 0 && districtCode == 0)  //if state login
                {
                    SelectListItem item = stateList.Find(x => x.Value == stateCode.ToString());
                    stateList.Clear();
                    stateList.Add(item);
                    districtList.Clear();
                    districtList = ORDal.PopulateDistrict(stateCode);
                    districtList.Find(x => x.Value == "0").Text = "All District";
                    blockList.Clear();
                    blockList = ORDal.PopulateBlocks(districtCode);
                    blockList.Find(x => x.Value == "0").Text = "All Blocks";
                }

                  ViewModel.StateList= stateList;
                  ViewModel.DistrictList = districtList;
                  ViewModel.BlockList = blockList;
                  ViewModel.YearList = YearList;
                return View(ViewModel);
            }
            catch {
                return null;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TestReportListing(TestModel model)
        {
            try
            {
                model.Level = PMGSYSession.Current.RoleCode == 25 ? 1 : PMGSYSession.Current.RoleCode == 2 ? 2 : 3;
                return View(model);
            }
            catch {
                return null;
            }
        }

        [HttpPost]
        public JsonResult PopulateDistrictList(int StateCode)
        {
            ORDAL LMDal = new ORDAL();
            List<SelectListItem> districtList = new List<SelectListItem>();
            districtList = LMDal.PopulateDistrict(StateCode, true);
            return Json(districtList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PopulateblockList(int districtCode)
        {
            ORDAL LMDal = new ORDAL();
            List<SelectListItem> BlockList = new List<SelectListItem>();
            BlockList = LMDal.PopulateBlocks(districtCode, true);
            return Json(BlockList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Rnd Target and Achievement
        [HttpGet]
        public ActionResult TechnologyCompletedLayout()
        {

            try
            {
                TechnologyModel RnDachievementModel = new TechnologyModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            ORDAL ORDal = new ORDAL();

            RnDachievementModel.BlockList = new List<SelectListItem>();
            RnDachievementModel.DistrictList = new List<SelectListItem>();

            RnDachievementModel.StateList = commonFunctions.PopulateStates(true);
            RnDachievementModel.StateList.Where(s => s.Value == "0").FirstOrDefault().Text = "All States";
            RnDachievementModel.YearList = commonFunctions.PopulateFinancialYear(true,true);

            int StateCode = PMGSYSession.Current.StateCode;
            int DistrictCode = PMGSYSession.Current.DistrictCode;

            int role = PMGSYSession.Current.RoleCode;
            int level = PMGSYSession.Current.LevelId;
                //StateCode == 0 && PMGSYSession.Current.LevelId==1
            if (role==25)  //mord login
            {
                RnDachievementModel.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" ,Selected=true});
                RnDachievementModel.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                
            }
                //StateCode > 0 && DistrictCode > 0
            if (role==22) //if piu Login
            {
                SelectListItem item = RnDachievementModel.StateList.Find(x => x.Value == StateCode.ToString());
                RnDachievementModel.StateList.Clear();
                RnDachievementModel.StateList.Add(item);

                RnDachievementModel.DistrictList = ORDal.PopulateDistrict(StateCode);
                SelectListItem DistrictItem = RnDachievementModel.DistrictList.Find(x => x.Value == DistrictCode.ToString());
                RnDachievementModel.DistrictList.Clear();
                RnDachievementModel.DistrictList.Add(DistrictItem);

                RnDachievementModel.BlockList.Clear();
                RnDachievementModel.BlockList = ORDal.PopulateBlocks(DistrictCode, true);
            }

            if (role==2)  //if state login
            {
                SelectListItem item = RnDachievementModel.StateList.Find(x => x.Value == StateCode.ToString());
                RnDachievementModel.StateList.Clear();
                RnDachievementModel.StateList.Add(item);

                RnDachievementModel.DistrictList.Clear();
                RnDachievementModel.DistrictList = ORDal.PopulateDistrict(StateCode);
                RnDachievementModel.DistrictList.Find(x => x.Value == "0").Text = "All District";

                RnDachievementModel.BlockList.Clear();
                RnDachievementModel.BlockList = ORDal.PopulateBlocks(DistrictCode);
                RnDachievementModel.BlockList.Find(x => x.Value == "0").Text = "All Blocks";
            }

            RnDachievementModel.BatchList = commonFunctions.PopulateBatch(true);
            RnDachievementModel.FundingAgencyList = commonFunctions.PopulateFundingAgency(true);
            RnDachievementModel.FundingAgencyList.Where(s => s.Value == "-1").First().Value = "0";
 
            RnDachievementModel.ConnectivityCode = "%";
            RnDachievementModel.ConnectivitysList = new List<SelectListItem>();
            RnDachievementModel.ConnectivitysList.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));
            RnDachievementModel.ConnectivitysList.Insert(1, (new SelectListItem { Text = "New Connectivity", Value = "N" }));
            RnDachievementModel.ConnectivitysList.Insert(2, (new SelectListItem { Text = "Upgradation", Value = "U" }));

            RnDachievementModel.RoadStatus = "%";
            RnDachievementModel.RoadStatusList = new List<SelectListItem>();
            RnDachievementModel.RoadStatusList.Insert(0, (new SelectListItem { Text = "All", Value = "%" }));
            RnDachievementModel.RoadStatusList.Insert(1, (new SelectListItem { Text = "Completed", Value = "C" }));
            RnDachievementModel.RoadStatusList.Insert(2, (new SelectListItem { Text = "In-Progress", Value = "P" }));

            return View(RnDachievementModel);
            }
            catch (Exception Ex)
            {
               string exception= Ex.Message;
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RndTargestAchievementListing(TechnologyModel model)
       {
           try
           {
               if (model.StateCode > 0 && model.DistrictCode==0 && model.BlockCode==0)
               {
                   model.LevelCode = 2;
               }
               else if (model.StateCode > 0 && model.DistrictCode > 0 && model.BlockCode == 0)
               {
                   model.LevelCode = 3;
               }
               else if (model.StateCode > 0 && model.DistrictCode > 0 && model.BlockCode > 0)
               {
                   model.LevelCode = 4;
               }
               else
               {
               model.LevelCode = PMGSY.Extensions.PMGSYSession.Current.RoleCode == 25 ? 1 : PMGSY.Extensions.PMGSYSession.Current.RoleCode == 2 ? 2 : PMGSY.Extensions.PMGSYSession.Current.RoleCode == 22 ? 3 : 4;
               }
               if (ModelState.IsValid)
               {
                   return View(model);
               }
               else {
                  
                   ModelState.Values.ToString();
                   return null;
                }
           }
           catch (Exception ex)
           {
               ex.Message.ToString();
               return null; 
           }
       }

        
        [HttpPost]
        public JsonResult PopulateCollaborations()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                return Json(objCommonFunctions.PopulateCollaborationsStateWise(stateCode, true));
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }
        #endregion

        #region Phase Profile
        [HttpGet]
        public ActionResult PhaseProfileLayout()
        {
            try
            {
                PhaseProfileModel PhaseModel = new PhaseProfileModel();
                CommonFunctions commonFunctions = new CommonFunctions();
                ORDAL ORDal = new ORDAL();

                PhaseModel.AgencyList = new List<SelectListItem>();
                PhaseModel.ColaborationList = new List<SelectListItem>();

                PhaseModel.StateList = commonFunctions.PopulateStates(true);

                int StateCode = PMGSYSession.Current.StateCode;
                int DistrictCode = PMGSYSession.Current.DistrictCode;

                int role = PMGSYSession.Current.RoleCode;

                if (role == 2)  //if state login
                {
                    SelectListItem item = PhaseModel.StateList.Find(x => x.Value == StateCode.ToString());
                    PhaseModel.StateList.Clear();
                    PhaseModel.StateList.Add(item);
                    // PhaseModel.AgencyList.Insert(0, (new SelectListItem { Text = "All ", Value = "0", Selected = true }));
                    PhaseModel.AgencyList = commonFunctions.PopulateAgencies(StateCode, true);
                    //PhaseModel.ColaborationList.Insert(0, (new SelectListItem { Text = "All Collaboration", Value = "0" }));
                    PhaseModel.ColaborationList = commonFunctions.PopulateCollaborationsStateWise(StateCode, true);
                }

                //PhaseModel.PMGSYList = new List<SelectListItem>();
                ////PhaseModel.PMGSYList.Insert(0, (new SelectListItem { Text = "Select PMGSY", Value = "" }));
                //PhaseModel.PMGSYList.Insert(0, (new SelectListItem { Text = "PMGSY-I", Value = "1" }));
                //PhaseModel.PMGSYList.Insert(1, (new SelectListItem { Text = "PMGSY-II", Value = "2" }));

                return View(PhaseModel);
            }
            catch (Exception Ex)
            {
                string exception = Ex.Message;
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoadPhaseProfileListing(PhaseProfileModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            return null;
        }
        #endregion

        #region Phase Sanctioned Report
        [HttpGet]
        public ViewResult PhaseSactionReportLayout() 
        {
            try
            {
                CommonFunctions common = new CommonFunctions();
                PhaseSanctionModel model = new PhaseSanctionModel();
                model.StateList = common.PopulateStates(false);
                model.StateList.Insert(0, new SelectListItem { Text = "All State", Value = "0", Selected = true });

                model.YearList = common.PopulateFinancialYear(true, true).ToList<SelectListItem>();
                model.BatchList = common.PopulateBatch(true);

                SelectListItem all = new SelectListItem { Text = "All", Value = "0", Selected = true };
                SelectListItem pmgsy1 = new SelectListItem { Text = "PMGSY-1", Value = "1", Selected = true };
                SelectListItem pmgsy2 = new SelectListItem { Text = "PMGSY-2", Value = "2", Selected = true };
                SelectListItem pmgsy3 = new SelectListItem { Text = "RCPLWE", Value = "3", Selected = true };
                SelectListItem pmgsy4 = new SelectListItem { Text = "PMGSY-3", Value = "4", Selected = true };

                model.PMGSYList = new List<SelectListItem>();
                model.PMGSYList.Add(all);
                model.PMGSYList.Add(pmgsy1);
                model.PMGSYList.Add(pmgsy2);
                model.PMGSYList.Add(pmgsy3);
                model.PMGSYList.Add(pmgsy4);

                return View(model);
            }
            catch
            {
                return null;
            }   
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult PhaseSactionReport(PhaseSanctionModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// Populate Agencies based on selected state
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Authorize]
        public JsonResult PopulateAgencies()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);

                return Json(objCommonFunctions.PopulateAgencies(stateCode, true));
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        //Added by abhinav pathak on 20-02-2019
        #region Corrigendum Report
        [HttpGet]
        public ActionResult CorrigendumReportLayout()
        {
            try
            {
                CorrigendumReportModel viewModel = new CorrigendumReportModel();
                CommonFunctions comm = new CommonFunctions();
                PMGSY.Areas.ECBriefReport.Controllers.ECBriefReportController ecModel = new PMGSY.Areas.ECBriefReport.Controllers.ECBriefReportController();

                if (PMGSYSession.Current.StateCode > 0)
                {
                    viewModel.StateCode = PMGSYSession.Current.StateCode;
                    viewModel.StateName = PMGSYSession.Current.StateName;
                    viewModel.State_Name = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(viewModel.StateCode), Text = Convert.ToString(viewModel.StateName) });
                    viewModel.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        viewModel.DistrictCode = PMGSYSession.Current.DistrictCode;
                        viewModel.DistName = PMGSYSession.Current.DistrictName;

                        List<SelectListItem> lstDist = new List<SelectListItem>();
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(viewModel.DistrictCode), Text = Convert.ToString(viewModel.DistName) });
                        viewModel.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                    }
                    else
                    {
                        viewModel.DistrictList = comm.PopulateDistrict(viewModel.StateCode, true);
                        viewModel.DistrictList.RemoveAt(0);
                        viewModel.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                    }


                }
                else
                {
                    viewModel.StateList = comm.PopulateStates(true);
                    viewModel.StateList.RemoveAt(0);
                    viewModel.StateList.Insert(0, new SelectListItem { Value = "0", Text = "All States" });

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
                    viewModel.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                }

                viewModel.YearList = comm.PopulateYears().ToList();
                viewModel.YearList.RemoveAt(0);
                viewModel.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
                viewModel.SchemeList = comm.PopulateScheme();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "otherReport.CorrigendumReportLayout()");
                return null;
            }
        }


        #region Corrigendun Report post method added by abhinav pathak on 20-02-2019
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CorrigendumReportLayoutPost(CorrigendumReportModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        model.StateName = PMGSYSession.Current.StateName;
                    }
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        model.DistName = PMGSYSession.Current.DistrictName;
                    }
                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OtherReports.CorrigendumReportLayoutPost");
                return null;
            }

        }

        #endregion
        #endregion

        #region Gepnic Tender Details Report added by abhinav pathak on 20-02-2019

        #region GEt Method
        [HttpGet]
        public ActionResult GepnicTenderReportLayout()
        {
            try
            {
                GepnicTenderReportModel viewModel = new GepnicTenderReportModel();
                CommonFunctions comm = new CommonFunctions();

                if (PMGSYSession.Current.StateCode > 0)
                {
                    viewModel.StateCode = PMGSYSession.Current.StateCode;
                    viewModel.StateName = PMGSYSession.Current.StateName;
                    viewModel.State_Name = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(viewModel.StateCode), Text = Convert.ToString(viewModel.StateName) });
                    viewModel.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        viewModel.DistrictCode = PMGSYSession.Current.DistrictCode;
                        viewModel.DistName = PMGSYSession.Current.DistrictName;

                        List<SelectListItem> lstDist = new List<SelectListItem>();
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(viewModel.DistrictCode), Text = Convert.ToString(viewModel.DistName) });
                        viewModel.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                    }
                    else
                    {
                        viewModel.DistrictList = comm.PopulateDistrict(viewModel.StateCode, true);
                        viewModel.DistrictList.RemoveAt(0);
                        viewModel.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                    }


                }
                else
                {
                    viewModel.StateList = comm.PopulateStates(true);
                    viewModel.StateList.RemoveAt(0);
                    viewModel.StateList.Insert(0, new SelectListItem { Value = "0", Text = "All States" });

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
                    viewModel.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                }

                viewModel.YearList = comm.PopulateYears().ToList();
                viewModel.YearList.RemoveAt(0);
                viewModel.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
                viewModel.SchemeList = comm.PopulateScheme();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OtherReports.GepnicTenderReportLayout()");
                return null;
            }
        }
        #endregion

        #region Gepnic Tender Report post method added by abhinav pathak on 20-02-2019
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GepnicTenderReportLayoutPost(GepnicTenderReportModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string useragent = Request.UserAgent;
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        model.StateName = PMGSYSession.Current.StateName;
                    }
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        model.DistName = PMGSYSession.Current.DistrictName;

                    }

                    return View(model);
                }
                else
                {
                    return null;
                }
            }


            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OtherReports.GepnicTenderReportLayoutPost");
                return null;
            }

        }

        #endregion
        #endregion

        #region Works without main AGR
        public ActionResult GetWorksWithoutAGRView()
        {
            WorksWithoutAgrModel viewModel = new WorksWithoutAgrModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.StateCode > 0)
                {
                    viewModel.StateCode = PMGSYSession.Current.StateCode;
                    viewModel.StateName = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(viewModel.StateCode), Text = Convert.ToString(viewModel.StateName) });
                    viewModel.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        viewModel.DistrictCode = PMGSYSession.Current.DistrictCode;
                        viewModel.DistName = PMGSYSession.Current.DistrictName;

                        List<SelectListItem> lstDist = new List<SelectListItem>();
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(viewModel.DistrictCode), Text = Convert.ToString(viewModel.DistName) });
                        viewModel.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();
                    }
                    else
                    {
                        viewModel.DistrictList = comm.PopulateDistrict(viewModel.StateCode, true);
                        viewModel.DistrictList.RemoveAt(0);
                        viewModel.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                    }
                }
                else
                {
                    viewModel.StateList = comm.PopulateStates(true);
                    viewModel.StateList.RemoveAt(0);
                    viewModel.StateList.Insert(0, new SelectListItem { Value = "0", Text = "All States" });

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
                    viewModel.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OtherReports.GetWorksWithoutAGRView");
                return null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetWorksWithoutAGRPost(WorksWithoutAgrModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        model.StateName = PMGSYSession.Current.StateName;
                    }
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        model.DistName = PMGSYSession.Current.DistrictName;
                    }
                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OtherReports.GetWorksWithoutAGRPost");
                return null;
            }
        }

        #endregion

        #region Cost Component Report
        // report by sachin 
        [HttpGet]
        public ActionResult ComponentCostReport()
        {
            try
            {
                ComponentModel ViewModel = new ComponentModel();
                ViewModel.StateCode = PMGSYSession.Current.StateCode;
                ViewModel.DistrictCode = PMGSYSession.Current.DistrictCode;
                CommonFunctions fun = new CommonFunctions();
                int stateCode = PMGSYSession.Current.StateCode;
                int districtCode = PMGSYSession.Current.DistrictCode;

                ORDAL ORDal = new ORDAL();
                ViewModel.BatchList = PopulateBatchDetails(true);//fun.PopulateBatch(true);
                //ViewModel.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "ALL Batch" });

                ViewModel.schemeCode = 4;

                ViewModel.StateList = fun.PopulateStates(true);
                //ViewModel.DistrictList = fun.PopulateDistrict(stateCode, true);
                //ViewModel.BlockList = fun.PopulateBlocks(districtCode, true);

                //ViewModel.YearList = fun.PopulateAllYears(DateTime.Now.Year);
                ViewModel.YearList = fun.PopulateFinancialYears(false, false).ToList();
                if (stateCode == 0)
                {
                    //ViewModel.StateList.Insert(0, new SelectListItem { Value = "0", Text = "All States" });

                    ViewModel.DistrictList = new List<SelectListItem>();
                    ViewModel.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                    ViewModel.BlockList = new List<SelectListItem>();
                    ViewModel.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                }
                if (stateCode > 0 && districtCode > 0) //if piu Login
                {
                    SelectListItem item = ViewModel.StateList.Find(x => x.Value == stateCode.ToString());
                    ViewModel.StateList.Clear();
                    ViewModel.StateList.Add(item);

                    ViewModel.DistrictList = ORDal.PopulateDistrict(stateCode);
                    SelectListItem DistrictItem = ViewModel.DistrictList.Find(x => x.Value == districtCode.ToString());
                    ViewModel.DistrictList.Clear();
                    ViewModel.DistrictList.Add(DistrictItem);
                    //ViewModel.BlockList.Clear();
                    ViewModel.BlockList = ORDal.PopulateBlocks(districtCode, true);
                }

                if (stateCode > 0 && districtCode == 0)  //if state login
                {
                    SelectListItem item = ViewModel.StateList.Find(x => x.Value == stateCode.ToString());
                    ViewModel.StateList.Clear();
                    ViewModel.StateList.Add(item);

                    //ViewModel.DistrictList.Clear();
                    ViewModel.DistrictList = ORDal.PopulateDistrict(stateCode);
                    ViewModel.DistrictList.Find(x => x.Value == "0").Text = "All District";

                    //ViewModel.BlockList.Clear();
                    ViewModel.BlockList = ORDal.PopulateBlocks(districtCode);
                    ViewModel.BlockList.Find(x => x.Value == "0").Text = "All Blocks";
                }

                ViewModel.lstSTAStatus = new List<SelectListItem>();
                ViewModel.lstSTAStatus.Insert(0, new SelectListItem() { Text = "All", Value = "0" });
                ViewModel.lstSTAStatus.Insert(1, new SelectListItem() { Text = "Yes", Value = "Y" });
                ViewModel.lstSTAStatus.Insert(2, new SelectListItem() { Text = "No", Value = "N" });

                return View(ViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.OtherReports.ComponentCostReport()");
                return null;
            }
        }

        //added by sachin 
        [HttpPost]
        public ActionResult ComponentCostReportList(ComponentModel ViewModel)
        {
            try
            {
                return View(ViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.OtherReports.ComponentCostReport()");
                return null;
            }
        }

        public List<SelectListItem> PopulateBatchDetails(bool isAllSelected = false)
        {
            List<SelectListItem> batchList = new List<SelectListItem>();
            SelectListItem item;

            try
            {
                PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
                var query = (from c in dbContext.MASTER_BATCH
                             select new
                             {
                                 Text = c.MAST_BATCH_NAME,
                                 Value = c.MAST_BATCH_CODE
                             }).ToList().Distinct();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    batchList.Add(item);
                }

                //if (isAllSelected == false)
                //{
                //    batchList.Insert(0, (new SelectListItem { Text = "Select Batch", Value = "-1", Selected = true }));
                //}
                //else if (isAllSelected == true)
                //{
                //    batchList.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                //}

                return batchList;
            }
            catch
            {
                return batchList;
            }
            finally
            {
               // dbContext.Dispose();
            }

        }
        #endregion

        #region Road Category Details
        // added by sachin for category wise report 
        [HttpGet]
        public ActionResult RoadCategory()
        {
            try
            {
                RoadCategoryModel ViewModel = new RoadCategoryModel();
                CommonFunctions commonFunctions = new CommonFunctions();
                ViewModel.StateCode = PMGSYSession.Current.StateCode;
                ViewModel.DistrictCode = PMGSYSession.Current.DistrictCode;
                CommonFunctions fun = new CommonFunctions();
                int stateCode = PMGSYSession.Current.StateCode;
                int districtCode = PMGSYSession.Current.DistrictCode;


                ViewModel.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

                ViewModel.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();

                ORDAL ORDal = new ORDAL();

                ViewModel.StateList = commonFunctions.PopulateStates(true);
                ViewModel.DistrictList = commonFunctions.PopulateDistrictForSRRDA(stateCode, false);

                //   ViewModel.DistrictList.Insert(0, new SelectListItem { Value="-1", Text="select district" });

                // ViewModel.schemeCode = 4;


                //ViewModel.DistrictList = fun.PopulateDistrict(stateCode, true);
                //ViewModel.BlockList = fun.PopulateBlocks(districtCode, true);


                if (stateCode > 0 && districtCode > 0) //if piu Login
                {
                    SelectListItem item = ViewModel.StateList.Find(x => x.Value == stateCode.ToString());
                    ViewModel.StateList.Clear();
                    ViewModel.StateList.Add(item);

                    ViewModel.DistrictList = ORDal.PopulateDistrict(stateCode);
                    SelectListItem DistrictItem = ViewModel.DistrictList.Find(x => x.Value == districtCode.ToString());
                    ViewModel.DistrictList.Clear();
                    ViewModel.DistrictList.Add(DistrictItem);
                    //ViewModel.BlockList.Clear();

                }

                if (stateCode > 0 && districtCode == 0)  //if state login
                {
                    SelectListItem item = ViewModel.StateList.Find(x => x.Value == stateCode.ToString());
                    ViewModel.StateList.Clear();
                    ViewModel.StateList.Add(item);

                    //ViewModel.DistrictList.Clear();
                    ViewModel.DistrictList = ORDal.PopulateDistrict(stateCode);
                    //   ViewModel.DistrictList.Find(x => x.Value == "0").Text = "All District";

                    //ViewModel.BlockList.Clear();

                }
                return View(ViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.OtherReports.RoadCategory()");
                return null;
            }
        }


        [HttpPost]
        public ActionResult RoadCategoryReport(RoadCategoryModel ViewModel)
        {
            try
            {
                return View(ViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.OtherReports.RoadCategoryReport()");
                return null;
            }
        }
        #endregion

        #region Benefitted Habs Facility
        [HttpGet]
        public ActionResult BenefittedHabsFacilityLayout()
        {
            CommonFunctions comm = new CommonFunctions();
            BenefittedHabsFacilityViewModel model = new BenefittedHabsFacilityViewModel();
            try
            {
                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.lstState = comm.PopulateStates(true);
                    model.lstState.Find(x => x.Value == "0").Value = "-1";
                }
                else
                {
                    model.lstState = new List<SelectListItem>();
                    model.lstState.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode).Trim() });
                }

                model.lstYear = comm.PopulateYears(true);
                int year = DateTime.Now.Month > 3 ? DateTime.Now.Year : DateTime.Now.Year - 1;
                int count = model.lstYear.IndexOf(model.lstYear.Find(c => c.Value == "2018"));
                model.lstYear.RemoveRange(model.lstYear.IndexOf(model.lstYear.Find(c => c.Value == "2018")), model.lstYear.Count - count);
                if (DateTime.Now.Month <= 3)
                {
                    model.lstYear.RemoveAt(model.lstYear.IndexOf(model.lstYear.Find(c => c.Value == DateTime.Now.Year.ToString())));
                }

                model.lstBatch = comm.PopulateBatch(false);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OtherReports.BenefittedHabsFacilityLayout");
                return View(model);
            }
            finally
            {

            }
        }

        public ActionResult BenefittedHabsFacilityExcelReport(string rpt)
        {
            #region
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            string[] param = null;
            string fName = string.Empty;
            try
            {
                param = rpt.Split('$');

                Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
                rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);

                System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();

                Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new PMGSY.Controllers.CustomReportCredentials(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Username"], System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Password"]);
                rview.ServerReport.ReportServerCredentials = irsc;

                rview.ServerReport.ReportPath = "/PMGSYCitizen/Benefitted_Habs_Facility_Report";
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("State", param[0].Trim()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Year", param[1].Trim()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Batch", param[2].Trim()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", param[3].Trim()));
                fName = "BenefittedHabsFacility__" + param[1].Trim() + "_" + DateTime.Now.ToShortDateString();

                rview.ServerReport.SetParameters(paramList);
                string mimeType, encoding, extension, deviceInfo;
                string[] streamids;
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string format = "Excel"; //Desired format goes here (PDF, Excel, or Image)

                deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>False</SimplePageHeaders>" + "</DeviceInfo>";
                //var fileName = "STA_Payment.pdf";
                var fileName = fName.Trim() + ".xls";
                byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

                Response.Clear();


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = fileName,

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,

                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(bytes, "application/ms-excel");
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "LocationSSRSReports.OmmasSystemCodesReport");
                return null;
            }
            #endregion
            return null;
        }
        #endregion

        #region CUPL Data Report
        [HttpGet]
        public ActionResult CUPLDataLayout()
        {
            CommonFunctions comm = new CommonFunctions();
            CUPLDataViewModel model = new CUPLDataViewModel();
            try
            {
                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.lstState = comm.PopulateStates(true);
                    model.lstState.Find(x => x.Value == "0").Value = "-1";
                }
                else
                {
                    model.lstState = new List<SelectListItem>();
                    model.lstState.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode).Trim() });
                }

                model.lstYear = comm.PopulateYears(true);
                int year = DateTime.Now.Month > 3 ? DateTime.Now.Year : DateTime.Now.Year - 1;
                int count = model.lstYear.IndexOf(model.lstYear.Find(c => c.Value == "2018"));
                model.lstYear.RemoveRange(model.lstYear.IndexOf(model.lstYear.Find(c => c.Value == "2018")), model.lstYear.Count - count);
                if (DateTime.Now.Month <= 3)
                {
                    model.lstYear.RemoveAt(model.lstYear.IndexOf(model.lstYear.Find(c => c.Value == DateTime.Now.Year.ToString())));
                }

                model.lstBatch = comm.PopulateBatch(false);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OtherReports.CUPLDataLayout");
                return View(model);
            }
            finally
            {

            }
        }

        //[HttpPost]
        public ActionResult CUPLDataExcelReport(string rpt)
        {
            #region
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            string[] param = null;
            string fName = string.Empty;
            try
            {
                param = rpt.Split('$');

                Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
                rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);

                System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();

                Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new PMGSY.Controllers.CustomReportCredentials(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Username"], System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Password"]);
                rview.ServerReport.ReportServerCredentials = irsc;

                rview.ServerReport.ReportPath = "/PMGSYCitizen/CUPL_Data_Report";
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("State", param[0].Trim()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Year", param[1].Trim()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Batch", param[2].Trim()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", param[3].Trim()));
                fName = "CUPLData__" + param[1].Trim() + "_" + DateTime.Now.ToShortDateString();

                rview.ServerReport.SetParameters(paramList);
                string mimeType, encoding, extension, deviceInfo;
                string[] streamids;
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string format = "Excel"; //Desired format goes here (PDF, Excel, or Image)

                deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>False</SimplePageHeaders>" + "</DeviceInfo>";
                //var fileName = "STA_Payment.pdf";
                var fileName = fName.Trim() + ".xls";
                byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

                Response.Clear();

                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = fileName,

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,

                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(bytes, "application/ms-excel");
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "LocationSSRSReports.CUPLDataExcelReport");
                return null;
            }
            #endregion
            return null;
        }
        #endregion
    }
}
