using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Areas.LocationSSRSReports.Models;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Controllers;

namespace PMGSY.Areas.LocationSSRSReports.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class LocationSSRSReportsController : Controller
    {
        CommonFunctions comm = new CommonFunctions();
        //
        // GET: /LocationSSRSReports/LocationSSRSReport/

        public ActionResult Index()
        {
            return View();
        }

        #region Habitation Cluster -- Done
        [HttpGet]
        public ActionResult HabitationClusterReportLayout()
        {
            HabitationClusterModel objHabitationClusterModel = new HabitationClusterModel();
            return View(objHabitationClusterModel);
        }

        [HttpPost]
        public ActionResult HabitationClusterReport(HabitationClusterModel objHabitationClusterModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objHabitationClusterModel.Mast_State_Code = objHabitationClusterModel.StateCode > 0 ? objHabitationClusterModel.StateCode : objHabitationClusterModel.Mast_State_Code;
                    objHabitationClusterModel.Mast_District_Code = objHabitationClusterModel.DistrictCode > 0 ? objHabitationClusterModel.DistrictCode : objHabitationClusterModel.Mast_District_Code;
                    objHabitationClusterModel.Mast_Block_Code = objHabitationClusterModel.BlockCode > 0 ? objHabitationClusterModel.BlockCode : objHabitationClusterModel.Mast_Block_Code;

                    return View(objHabitationClusterModel);
                }
                else
                {
                    return View("HabitationClusterReportLayout", objHabitationClusterModel);
                }
            }
            catch
            {
                return View(objHabitationClusterModel);
            }

        }

        #endregion

        #region Score Habitation -- Done
        [HttpGet]
        public ActionResult HabitationScoreReportLayout()
        {
            HabitationScoreModel objHabitationScoreModel = new HabitationScoreModel();
            return View(objHabitationScoreModel);
        }

        [HttpPost]
        public ActionResult HabitationScoreReport(HabitationScoreModel objHabitationScoreModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objHabitationScoreModel.Mast_State_Code = objHabitationScoreModel.StateCode > 0 ? objHabitationScoreModel.StateCode : objHabitationScoreModel.Mast_State_Code;
                    objHabitationScoreModel.Mast_District_Code = objHabitationScoreModel.DistrictCode > 0 ? objHabitationScoreModel.DistrictCode : objHabitationScoreModel.Mast_District_Code;
                    objHabitationScoreModel.Mast_Block_Code = objHabitationScoreModel.BlockCode > 0 ? objHabitationScoreModel.BlockCode : objHabitationScoreModel.Mast_Block_Code;

                    return View(objHabitationScoreModel);
                }
                else
                {
                    return View("HabitationScoreReportLayout", objHabitationScoreModel);
                }
            }
            catch
            {
                return View(objHabitationScoreModel);
            }

        }

        #endregion

        #region State Report
        [HttpGet]
        public ActionResult StateLayout()
        {
            LocationMasterReports objLoc = new LocationMasterReports();

            try
            {
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                SelectListItem state = new SelectListItem
                {
                    Text = "State",
                    Value = "S"
                };

                SelectListItem unionTerritory = new SelectListItem
                {
                    Text = "Union Territory",
                    Value = "U"
                };

                List<SelectListItem> stateOrUnion = new List<SelectListItem>();
                stateOrUnion.Add(all);
                stateOrUnion.Add(state);
                stateOrUnion.Add(unionTerritory);

                //ViewData["STATE_UNION"] = stateOrUnion;

                SelectListItem regular = new SelectListItem
                {
                    Text = "Regular",
                    Value = "R"
                };

                SelectListItem island = new SelectListItem
                {
                    Text = "Island",
                    Value = "I"
                };

                SelectListItem northEast = new SelectListItem
                {
                    Text = "North East",
                    Value = "N"
                };

                SelectListItem hilly = new SelectListItem
                {
                    Text = "Hilly",
                    Value = "H"
                };

                SelectListItem northEastAndHilly = new SelectListItem
                {
                    Text = "North East and Hilly",
                    Value = "X"
                };

                List<SelectListItem> stateType = new List<SelectListItem>();
                stateType.Add(all);
                stateType.Add(regular);
                stateType.Add(island);
                stateType.Add(northEast);
                stateType.Add(hilly);
                stateType.Add(northEastAndHilly);
                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);

                objLoc.TerritoryTypeList = new SelectList(stateOrUnion, "Value", "Text").ToList();
                objLoc.ActiveFlagList = new SelectList(activeType, "Value", "Text").ToList();
                objLoc.StateTypeList = new SelectList(stateType, "Value", "Text").ToList();
                //ViewData["Active_TYPE"] = activeType;
                //ViewData["STATE_TYPE"] = stateType;

                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult StateReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 1;
                    objLoc.Type1 = objLoc.Territory;
                    objLoc.Type2 = objLoc.StateType;
                    objLoc.Type3 = "%";
                    objLoc.Type4 = "%";
                    objLoc.Type5 = "%";

                    objLoc.District = 0;
                    objLoc.Block = 0;
                    objLoc.Village = 0;
                    objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    objLoc.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region District
        [HttpGet]
        public ActionResult DistrictLayout(FormCollection frmCollection)
        {
            LocationMasterReports objLoc = new LocationMasterReports();
            try
            {
                objLoc.State = PMGSYSession.Current.StateCode;
                objLoc.District = PMGSYSession.Current.DistrictCode;

                int stateCode = PMGSYSession.Current.StateCode;
                // State Drop Down
                List<SelectListItem> stateDd = comm.PopulateStates(false);
                stateDd.Find(x => x.Value == "1").Selected = true;

                // stateDd.Find(x => x.Value == "0").Text = "All States";
                // stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                if (stateCode > 0)  //if state login
                {
                    stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                //ViewData["STATE"] = stateDd;
                // Other Filters           
                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                List<SelectListItem> iapDistrict = new List<SelectListItem>();
                iapDistrict.Add(all);
                iapDistrict.Add(yes);
                iapDistrict.Add(no);

                List<SelectListItem> pmgsyIncludedDd = new List<SelectListItem>();
                pmgsyIncludedDd.Add(all);
                pmgsyIncludedDd.Add(yes);
                pmgsyIncludedDd.Add(no);
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);

                objLoc.StateList = new SelectList(stateDd, "Value", "Text").ToList();
                objLoc.ActiveFlagList = new SelectList(activeType, "Value", "Text").ToList();
                objLoc.iapDistrictList = new SelectList(iapDistrict, "Value", "Text").ToList();
                objLoc.pmgsyIncludedList = new SelectList(pmgsyIncludedDd, "Value", "Text").ToList();
                //ViewData["Active_TYPE"] = activeType;
                //ViewData["IAP_DISTRICT"] = iapDistrict;
                //ViewData["PMGSY_INCLUDED"] = pmgsyIncludedDd;
                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        [HttpPost]
        public ActionResult DistrictReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 2;
                    objLoc.Type1 = objLoc.iapDistrict;
                    objLoc.Type2 = objLoc.pmgsyIncluded;
                    objLoc.Type3 = "%";
                    objLoc.Type4 = "%";
                    objLoc.Type5 = "%";

                    objLoc.District = 0;
                    objLoc.Block = 0;
                    objLoc.Village = 0;
                    objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        objLoc.State = PMGSYSession.Current.StateCode;
                    }
                    objLoc.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;
                    objLoc.DistName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Block
        [HttpGet]
        public ActionResult BlockLayout(FormCollection frmCollection)
        {
            LocationMasterReports objLoc = new LocationMasterReports();
            try
            {
                objLoc.State = PMGSYSession.Current.StateCode;
                objLoc.District = PMGSYSession.Current.DistrictCode;

                objLoc.Mast_State_Code = PMGSYSession.Current.StateCode;
                objLoc.Mast_District_Code = PMGSYSession.Current.DistrictCode;

                int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
                int districtCode = PMGSYSession.Current.DistrictCode;

                List<SelectListItem> stateDd = comm.PopulateStates(false);
                stateDd.Find(x => x.Value == "1").Selected = true;

                //stateDd.Find(x => x.Value == "0").Text = "All States";
                List<SelectListItem> districtDd = comm.PopulateDistrict(stateCode);
                districtDd.Find(x => x.Value == "0").Text = "All District";

                if (districtCode > 0)
                {
                    districtDd.Find(x => x.Value == districtCode.ToString()).Selected = true;
                }

                if (stateCode > 0)  //if state login
                {
                    stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                if (Convert.ToInt32(frmCollection["StateCode"]) > 0)    // if mord login
                {
                    // return Json(common.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]),true), JsonRequestBehavior.AllowGet);

                    List<SelectListItem> list = comm.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
                    list.Find(x => x.Value == "-1").Value = "0";
                    if (districtCode > 0)
                    {
                        list.Find(x => x.Value == districtCode.ToString()).Selected = true;
                    }
                    return Json(list, JsonRequestBehavior.AllowGet);
                }

                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                List<SelectListItem> isDesert = new List<SelectListItem>();
                isDesert.Add(all);
                isDesert.Add(yes);
                isDesert.Add(no);

                List<SelectListItem> pmgsyIncludedDd = new List<SelectListItem>();
                pmgsyIncludedDd.Add(all);
                pmgsyIncludedDd.Add(yes);
                pmgsyIncludedDd.Add(no);

                List<SelectListItem> schedule5 = new List<SelectListItem>();
                schedule5.Add(all);
                schedule5.Add(yes);
                schedule5.Add(no);

                List<SelectListItem> isTribal = new List<SelectListItem>();
                isTribal.Add(all);
                isTribal.Add(yes);
                isTribal.Add(no);
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);

                objLoc.StateList = new SelectList(stateDd, "Value", "Text").ToList();
                objLoc.DistrictList = new SelectList(districtDd, "Value", "Text").ToList();
                objLoc.pmgsyIncludedList = new SelectList(pmgsyIncludedDd, "Value", "Text").ToList();
                objLoc.ActiveFlagList = new SelectList(activeType, "Value", "Text").ToList();
                objLoc.DesertList = new SelectList(isDesert, "Value", "Text").ToList();
                objLoc.ScheduleList = new SelectList(schedule5, "Value", "Text").ToList();
                objLoc.TribalList = new SelectList(isTribal, "Value", "Text").ToList();
                //ViewData["Active_TYPE"] = activeType;
                //ViewData["IS_DESERT"] = isDesert;
                //ViewData["PMGSY_INCLUDED"] = pmgsyIncludedDd;
                //ViewData["IS_SCHEDULE5"] = schedule5;
                //ViewData["IS_TRIBAL"] = isTribal;
                //ViewData["DISTRICT"] = districtDd;
                //ViewData["STATE"] = stateDd;
                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult BlockReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 3;
                    objLoc.Type1 = objLoc.Desert;
                    objLoc.Type2 = objLoc.pmgsyIncluded;
                    objLoc.Type3 = objLoc.Schedule;
                    objLoc.Type4 = objLoc.Tribal;
                    objLoc.Type5 = "%";

                    //objLoc.District = 0;
                    objLoc.Block = 0;
                    objLoc.Village = 0;
                    objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    objLoc.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objLoc.State;
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        objLoc.District = PMGSYSession.Current.DistrictCode;
                    }
                    objLoc.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;
                    objLoc.DistName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                    objLoc.BlockName = "All Blocks";

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Village Master
        //[HttpGet]
        public ActionResult VillageLayout(FormCollection frmCollection)
        {
            LocationMasterReports objLoc = new LocationMasterReports();
            //MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            try
            {
                objLoc.Mast_State_Code = PMGSYSession.Current.StateCode;
                objLoc.Mast_District_Code = PMGSYSession.Current.DistrictCode;

                //int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;

                int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(frmCollection["DistrictCode"]) : PMGSYSession.Current.DistrictCode;

                List<SelectListItem> stateDd = comm.PopulateStates(false); //Change 11/12/2013
                stateDd.Find(x => x.Value == "1").Selected = true;

                // stateDd.Find(x => x.Value == "0").Text = "All States"; //Change 11/12/2013
                //List<SelectListItem> districtDd = comm.PopulateDistrict(stateCode);
                //districtDd.Find(x => x.Value == "0").Text = "All Districts";
                //districtDd.RemoveAt(0);

                int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(stateDd.ElementAt(0).Value) : PMGSYSession.Current.StateCode;

                objLoc.DistrictList = comm.PopulateDistrict(stateCode);
                objLoc.DistrictList.Find(x => x.Value == "0").Text = "All Districts";
                if (stateCode > 0)
                {
                    objLoc.DistrictList.RemoveAt(0);
                }
                List<SelectListItem> blockDd = comm.PopulateBlocks(districtCode);
                blockDd.Find(x => x.Value == "0").Text = "All Blocks";
                if (districtCode > 0)
                {
                    objLoc.District = districtCode;
                    //districtDd.Find(x => x.Value == districtCode.ToString()).Selected = true;
                    objLoc.DistrictList.Find(x => x.Value == districtCode.ToString()).Selected = true;
                }

                if (stateCode > 0)  //if state login
                {
                    objLoc.State = stateCode;
                    stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                if (Convert.ToInt32(frmCollection["DistrictCode"]) > 0)
                {
                    List<SelectListItem> list = comm.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
                    list.Find(x => x.Value == "-1").Value = "0";
                    return Json(list, JsonRequestBehavior.AllowGet);
                }

                if (Convert.ToInt32(frmCollection["StateCode"]) > 0)    // if mord login
                {
                    List<SelectListItem> list = comm.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
                    //list.Find(x => x.Value == "-1").Value = "0";
                    list.RemoveAt(0);
                    if (districtCode > 0)
                    {
                        list.Find(x => x.Value == districtCode.ToString()).Selected = true;
                    }
                    return Json(list, JsonRequestBehavior.AllowGet);
                }

                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                List<SelectListItem> schedule5 = new List<SelectListItem>();
                schedule5.Add(all);
                schedule5.Add(yes);
                schedule5.Add(no);
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);

                objLoc.StateList = new SelectList(stateDd, "Value", "Text").ToList();
                objLoc.DistrictList = new SelectList(objLoc.DistrictList, "Value", "Text").ToList();
                objLoc.BlockList = new SelectList(blockDd, "Value", "Text").ToList();
                objLoc.ActiveFlagList = new SelectList(activeType, "Value", "Text").ToList();
                objLoc.ScheduleList = new SelectList(schedule5, "Value", "Text").ToList();
                objLoc.CensusYearList = new PMGSY.DAL.MasterReports.MasterReportsDAL().PopulateCensus();
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    objLoc.District = Convert.ToInt32(objLoc.DistrictList.ElementAt(0).Value);
                }
                //ViewData["Active_TYPE"] = activeType;
                //ViewData["STATE"] = stateDd;
                //ViewData["DISTRICT"] = districtDd;
                //ViewData["BLOCK"] = blockDd;
                //ViewData["IS_SCHEDULE5"] = schedule5;
                //ViewData["CENSUS_YEAR"] = new PMGSY.DAL.MasterReports.MasterReportsDAL().PopulateCensus();
                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult VillageReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 4;
                    objLoc.Type1 = objLoc.Schedule;
                    objLoc.Type2 = "%";
                    objLoc.Type3 = "%";
                    objLoc.Type4 = "%";
                    objLoc.Type5 = "%";

                    //objLoc.District = 0;
                    //objLoc.Block = 0;
                    objLoc.Village = 0;
                    objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    objLoc.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objLoc.State;
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        objLoc.District = PMGSYSession.Current.DistrictCode;
                    }
                    if (PMGSYSession.Current.StateName != null)
                    {
                        objLoc.StateName = objLoc.State == 0 ? "All States" : PMGSYSession.Current.StateName;
                    }
                    if (PMGSYSession.Current.DistrictName != null)
                    {
                        objLoc.DistName = objLoc.District == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                    }
                    objLoc.BlockName = objLoc.Block == 0 ? "All Blocks" : objLoc.BlockName;
                    objLoc.VillageName = "All Villages";

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Habitation Master
        public ActionResult HabitationLayout(FormCollection frmCollection)
        {
            //MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            LocationMasterReports objLoc = new LocationMasterReports();

            try
            {
                objLoc.Mast_State_Code = PMGSYSession.Current.StateCode;
                objLoc.Mast_District_Code = PMGSYSession.Current.DistrictCode;

                int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
                int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(frmCollection["DistrictCode"]) : PMGSYSession.Current.DistrictCode;
                int blockCode = Convert.ToInt32(frmCollection["BlockCode"]);

                List<SelectListItem> stateDd = comm.PopulateStates(true);
                stateDd.Find(x => x.Value == "0").Text = "Select State";
                stateDd.Find(x => x.Value == "0").Value = "-1";

                objLoc.State = PMGSYSession.Current.StateCode;
                objLoc.District = PMGSYSession.Current.DistrictCode;
                //int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(stateDd.ElementAt(0).Value) : PMGSYSession.Current.StateCode;

                List<SelectListItem> districtDd = comm.PopulateDistrict(stateCode);
                districtDd.Find(x => x.Value == "0").Text = "Select District";
                districtDd.Find(x => x.Value == "0").Value = "-1";
                List<SelectListItem> blockDd = comm.PopulateBlocks(districtCode);
                blockDd.Find(x => x.Value == "0").Text = "Select Block";
                blockDd.Find(x => x.Value == "0").Value = "-1";
                List<SelectListItem> villageDd = new List<SelectListItem>();
                var villageList = new PMGSY.DAL.MasterReports.MasterReportsDAL().PopulateVillage(blockCode);
                villageDd.Add(new SelectListItem
                {
                    Text = "All Villages",
                    Value = "0",
                    Selected = true
                });
                foreach (SelectListItem village in villageList)
                    villageDd.Add(village);

                if (stateCode > 0)
                {
                    stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                if (districtCode > 0)
                {
                    districtDd.Find(x => x.Value == districtCode.ToString()).Selected = true;
                }

                if (blockCode > 0)
                {
                    return Json(villageDd, JsonRequestBehavior.AllowGet);
                }

                if (Convert.ToInt32(frmCollection["BlockCode"]) > 0)
                {
                    return Json(villageDd, JsonRequestBehavior.AllowGet);
                }

                if (Convert.ToInt32(frmCollection["DistrictCode"]) > 0)
                {
                    return Json(comm.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"])), JsonRequestBehavior.AllowGet);
                }
                if (Convert.ToInt32(frmCollection["StateCode"]) > 0)
                {
                    // return Json(common.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"])), JsonRequestBehavior.AllowGet);
                    List<SelectListItem> list = comm.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
                    list.Find(x => x.Value == "-1").Value = "0";
                    if (districtCode > 0)
                    {
                        list.Find(x => x.Value == districtCode.ToString()).Selected = true;
                    }
                    return Json(list, JsonRequestBehavior.AllowGet);
                }

                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };
                SelectListItem Unconnected = new SelectListItem
                {
                    Text = "Unconnected",
                    Value = "U"
                };
                SelectListItem Connected = new SelectListItem
                {
                    Text = "Connected",
                    Value = "C"
                };
                SelectListItem StateConnected = new SelectListItem
                {
                    Text = "State Connected",
                    Value = "S"
                };
                SelectListItem PMGSYConnected = new SelectListItem
                {
                    Text = "PMGSY Connected",
                    Value = "P"
                };
                SelectListItem NotFeasible = new SelectListItem
                {
                    Text = "Not Feasible",
                    Value = "F"
                };

                List<SelectListItem> schedule5 = new List<SelectListItem>();
                schedule5.Add(all);
                schedule5.Add(yes);
                schedule5.Add(no);

                List<SelectListItem> habitationStatus = new List<SelectListItem>();
                habitationStatus.Add(all);
                habitationStatus.Add(Unconnected);
                habitationStatus.Add(Connected);
                habitationStatus.Add(StateConnected);
                habitationStatus.Add(PMGSYConnected);
                habitationStatus.Add(NotFeasible);
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);

                objLoc.StateList = new SelectList(stateDd, "Value", "Text").ToList();
                objLoc.DistrictList = new SelectList(districtDd, "Value", "Text").ToList();
                objLoc.BlockList = new SelectList(blockDd, "Value", "Text").ToList();
                objLoc.ActiveFlagList = new SelectList(activeType, "Value", "Text").ToList();
                objLoc.ScheduleList = new SelectList(schedule5, "Value", "Text").ToList();
                objLoc.VillageList = new SelectList(villageDd, "Value", "Text").ToList();
                objLoc.HabitationStatusList = new SelectList(habitationStatus, "Value", "Text").ToList();
                objLoc.CensusYearList = new PMGSY.DAL.MasterReports.MasterReportsDAL().PopulateCensus();
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    objLoc.District = Convert.ToInt32(objLoc.DistrictList.ElementAt(0).Value);
                }

                //ViewData["Active_TYPE"] = activeType;
                //ViewData["STATE"] = stateDd;
                //ViewData["DISTRICT"] = districtDd;
                //ViewData["BLOCK"] = blockDd;
                //ViewData["VILLAGE"] = villageDd;
                //ViewData["IS_SCHEDULE5"] = schedule5;
                //ViewData["HAB_STATUS"] = habitationStatus;
                //ViewData["CENSUS_YEAR"] = new PMGSY.DAL.MasterReports.MasterReportsDAL().PopulateCensus();
                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult HabitationReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 5;
                    objLoc.Type1 = objLoc.HabitationStatus;
                    objLoc.Type2 = objLoc.Schedule;
                    objLoc.Type3 = "%";
                    objLoc.Type4 = "%";
                    objLoc.Type5 = "%";

                    //objLoc.District = 0;
                    //objLoc.Block = 0;
                    //objLoc.Village = 0;
                    objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    objLoc.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objLoc.State;
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        objLoc.District = PMGSYSession.Current.DistrictCode;
                    }
                    //if (PMGSYSession.Current.StateName != null)
                    //{
                    //    objLoc.StateName = objLoc.State == 0 ? "All States" : PMGSYSession.Current.StateName;
                    //}
                    //if (PMGSYSession.Current.DistrictName != null)
                    //{
                    //    objLoc.DistName = objLoc.District == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                    //}
                    //objLoc.BlockName = objLoc.Block == 0 ? "All Blocks" : objLoc.BlockName;
                    //objLoc.VillageName = "All Villages";

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Panchayat Master
        public ActionResult PanchayatLayout(FormCollection frmCollection)
        {
            //MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            LocationMasterReports objLoc = new LocationMasterReports();
            try
            {
                objLoc.Mast_State_Code = PMGSYSession.Current.StateCode;
                objLoc.Mast_District_Code = PMGSYSession.Current.DistrictCode;

                //int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
                int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(frmCollection["DistrictCode"]) : PMGSYSession.Current.DistrictCode;
                List<SelectListItem> stateDd = comm.PopulateStates(false);
                stateDd.Find(x => x.Value == "1").Selected = true;

                int stateCode = 0;
                if (frmCollection["StateCode"] == null)
                {
                    stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(stateDd.ElementAt(0).Value) : PMGSYSession.Current.StateCode;
                }
                else
                {
                    stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
                }
                //  stateDd.Find(x => x.Value == "0").Text = "All States";
                List<SelectListItem> districtDd = comm.PopulateDistrict(stateCode);
                districtDd.Find(x => x.Value == "0").Text = "All Districts";
                List<SelectListItem> blockDd = comm.PopulateBlocks(districtCode);
                blockDd.Find(x => x.Value == "0").Text = "All Blocks";

                objLoc.State = PMGSYSession.Current.StateCode;
                objLoc.District = PMGSYSession.Current.DistrictCode;

                if (districtCode > 0)
                {
                    districtDd.Find(x => x.Value == districtCode.ToString()).Selected = true;
                }

                if (stateCode > 0)  //if state login
                {
                    stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                if (Convert.ToInt32(frmCollection["DistrictCode"]) > 0)
                {
                    List<SelectListItem> list = comm.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
                    list.Find(x => x.Value == "-1").Value = "0";
                    return Json(list, JsonRequestBehavior.AllowGet);
                }

                if (Convert.ToInt32(frmCollection["StateCode"]) > 0)    // if mord login
                {
                    List<SelectListItem> list = comm.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]));
                    //list.Find(x => x.Value == "-1").Value = "0";
                    list.Find(x => x.Value == "0").Text = "All Districts";
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                SelectListItem all = new SelectListItem
                {
                    Text = "All",
                    Value = "%"
                };
                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);

                objLoc.StateList = new SelectList(stateDd, "Value", "Text").ToList();
                objLoc.DistrictList = new SelectList(districtDd, "Value", "Text").ToList();
                objLoc.BlockList = new SelectList(blockDd, "Value", "Text").ToList();
                objLoc.ActiveFlagList = new SelectList(activeType, "Value", "Text").ToList();

                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    objLoc.District = Convert.ToInt32(objLoc.DistrictList.ElementAt(0).Value);
                }
                //ViewData["Active_TYPE"] = activeType;
                //ViewData["STATE"] = stateDd;
                //ViewData["DISTRICT"] = districtDd;
                //ViewData["BLOCK"] = blockDd;

                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PanchayatReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 10;
                    objLoc.Type1 = "%";
                    objLoc.Type2 = "%";
                    objLoc.Type3 = "%";
                    objLoc.Type4 = "%";
                    objLoc.Type5 = "%";

                    //objLoc.District = 0;
                    //objLoc.Block = 0;
                    //objLoc.Village = 0;
                    objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    objLoc.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objLoc.State;
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        objLoc.District = PMGSYSession.Current.DistrictCode;
                    }
                    if (PMGSYSession.Current.StateName != null)
                    {
                        objLoc.StateName = objLoc.State == 0 ? "All States" : PMGSYSession.Current.StateName;
                    }
                    if (PMGSYSession.Current.DistrictName != null)
                    {
                        objLoc.DistName = objLoc.District == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                    }
                    objLoc.BlockName = objLoc.Block == 0 ? "All Blocks" : objLoc.BlockName;
                    objLoc.VillageName = "All Villages";

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region MLA Constituency
        public ActionResult MLAConstituencyLayout()
        {
            //MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            LocationMasterReports objLoc = new LocationMasterReports();
            try
            {
                objLoc.State = PMGSYSession.Current.StateCode;
                objLoc.District = PMGSYSession.Current.DistrictCode;

                objLoc.Mast_State_Code = PMGSYSession.Current.StateCode;
                objLoc.Mast_District_Code = PMGSYSession.Current.DistrictCode;
                List<SelectListItem> stateDd = comm.PopulateStates(false);
                stateDd.Find(x => x.Value == "1").Selected = true;

                int stateCode = PMGSYSession.Current.StateCode;
                // stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true; //change 11/12/2013

                if (stateCode > 0)  //if state login
                {
                    stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }
                SelectListItem all = new SelectListItem
                {
                    Text = "All",
                    Value = "%"
                };
                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);

                objLoc.StateList = new SelectList(stateDd, "Value", "Text").ToList();
                objLoc.ActiveFlagList = new SelectList(activeType, "Value", "Text").ToList();
                //ViewData["Active_TYPE"] = activeType;
                //ViewData["STATE"] = stateDd;
                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MLAConstituencyReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 7;
                    objLoc.Type1 = "%";
                    objLoc.Type2 = "%";
                    objLoc.Type3 = "%";
                    objLoc.Type4 = "%";
                    objLoc.Type5 = "%";

                    //objLoc.District = 0;
                    //objLoc.Block = 0;
                    //objLoc.Village = 0;
                    objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    objLoc.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objLoc.State;
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        objLoc.District = PMGSYSession.Current.DistrictCode;
                    }
                    if (PMGSYSession.Current.StateName != null)
                    {
                        objLoc.StateName = objLoc.State == 0 ? "All States" : PMGSYSession.Current.StateName;
                    }
                    if (PMGSYSession.Current.DistrictName != null)
                    {
                        objLoc.DistName = objLoc.District == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                    }
                    objLoc.BlockName = objLoc.Block == 0 ? "All Blocks" : objLoc.BlockName;
                    objLoc.VillageName = "All Villages";

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region
        public ActionResult MPConstituencyLayout()
        {
            LocationMasterReports objLoc = new LocationMasterReports();
            try
            {
                objLoc.State = PMGSYSession.Current.StateCode;
                objLoc.District = PMGSYSession.Current.DistrictCode;

                objLoc.Mast_State_Code = PMGSYSession.Current.StateCode;
                objLoc.Mast_District_Code = PMGSYSession.Current.DistrictCode;
                List<SelectListItem> stateDd = comm.PopulateStates(false);
                stateDd.Find(x => x.Value == "1").Selected = true;

                int stateCode = PMGSYSession.Current.StateCode;
                // stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true; //change 11/12/2013

                if (stateCode > 0)  //if state login
                {
                    stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }
                SelectListItem all = new SelectListItem
                {
                    Text = "All",
                    Value = "%"
                };
                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);

                objLoc.StateList = new SelectList(stateDd, "Value", "Text").ToList();
                objLoc.ActiveFlagList = new SelectList(activeType, "Value", "Text").ToList();
                //ViewData["Active_TYPE"] = activeType;
                //ViewData["STATE"] = stateDd;
                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MPConstituencyReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 6;
                    objLoc.Type1 = "%";
                    objLoc.Type2 = "%";
                    objLoc.Type3 = "%";
                    objLoc.Type4 = "%";
                    objLoc.Type5 = "%";

                    //objLoc.District = 0;
                    //objLoc.Block = 0;
                    //objLoc.Village = 0;
                    objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    objLoc.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objLoc.State;
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        objLoc.District = PMGSYSession.Current.DistrictCode;
                    }
                    if (PMGSYSession.Current.StateName != null)
                    {
                        objLoc.StateName = objLoc.State == 0 ? "All States" : PMGSYSession.Current.StateName;
                    }
                    if (PMGSYSession.Current.DistrictName != null)
                    {
                        objLoc.DistName = objLoc.District == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                    }
                    objLoc.BlockName = objLoc.Block == 0 ? "All Blocks" : objLoc.BlockName;
                    objLoc.VillageName = "All Villages";

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region MP Block
        public ActionResult MPBlockLayout(FormCollection frmCollection)
        {
            //MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            LocationMasterReports objLoc = new LocationMasterReports();
            try
            {
                objLoc.Mast_State_Code = PMGSYSession.Current.StateCode;
                objLoc.Mast_District_Code = PMGSYSession.Current.DistrictCode;

                objLoc.State = PMGSYSession.Current.StateCode;
                objLoc.District = PMGSYSession.Current.DistrictCode;

                List<SelectListItem> stateDd = comm.PopulateStates(false);
                stateDd.Find(x => x.Value == "1").Selected = true;

                int stateCode = PMGSYSession.Current.StateCode;
                // stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;

                //ViewData["STATE"] = stateDd;
                objLoc.StateList = stateDd;

                if (Convert.ToInt32(frmCollection["StateCode"]) > 0)
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem
                    {
                        Text = "All Constituencies",
                        Value = "0"
                    });
                    foreach (SelectListItem i in new PMGSY.DAL.MasterReports.MasterReportsDAL().PopulateMPConstituency(Convert.ToInt32(frmCollection["StateCode"])))
                    {
                        list.Add(i);
                    }
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                if (stateCode > 0)
                {
                    stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem
                    {
                        Text = "All Constituencies",
                        Value = "0"
                    });
                    foreach (SelectListItem i in new PMGSY.DAL.MasterReports.MasterReportsDAL().PopulateMPConstituency(stateCode))
                    {
                        list.Add(i);
                    }
                    //ViewData["MP_CONSTITUENCY"] = list;
                    objLoc.MPConstituencyList = list;
                }
                if (stateCode == 0)
                {
                    List<SelectListItem> allConstituency = new List<SelectListItem>();
                    allConstituency.Add(new SelectListItem
                    {
                        Text = "All Constituencies",
                        Value = "0"
                    });

                    //ViewData["MP_CONSTITUENCY"] = allConstituency;
                    objLoc.MPConstituencyList = allConstituency;
                }
                SelectListItem all = new SelectListItem
                {
                    Text = "All",
                    Value = "%"
                };
                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);
                //ViewData["Active_TYPE"] = activeType;
                objLoc.ActiveFlagList = activeType;
                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MPBlockReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 8;
                    objLoc.Type1 = "%";
                    objLoc.Type2 = "%";
                    objLoc.Type3 = "%";
                    objLoc.Type4 = "%";
                    objLoc.Type5 = "%";

                    //objLoc.District = 0;
                    //objLoc.Block = 0;
                    //objLoc.Village = 0;
                    objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    objLoc.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objLoc.State;
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        objLoc.District = PMGSYSession.Current.DistrictCode;
                    }
                    if (PMGSYSession.Current.StateName != null)
                    {
                        objLoc.StateName = objLoc.State == 0 ? "All States" : PMGSYSession.Current.StateName;
                    }
                    if (PMGSYSession.Current.DistrictName != null)
                    {
                        objLoc.DistName = objLoc.District == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                    }
                    objLoc.BlockName = objLoc.Block == 0 ? "All Blocks" : objLoc.BlockName;
                    objLoc.VillageName = "All Villages";

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region MLA Block
        public ActionResult MLABlockLayout(FormCollection frmCollection)
        {
            //MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            LocationMasterReports objLoc = new LocationMasterReports();
            try
            {
                objLoc.Mast_State_Code = PMGSYSession.Current.StateCode;
                objLoc.Mast_District_Code = PMGSYSession.Current.DistrictCode;

                objLoc.State = PMGSYSession.Current.StateCode;
                objLoc.District = PMGSYSession.Current.DistrictCode;

                List<SelectListItem> stateDd = comm.PopulateStates(false);
                stateDd.Find(x => x.Value == "1").Selected = true;

                int stateCode = PMGSYSession.Current.StateCode;
                // stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                //ViewData["STATE"] = stateDd;
                objLoc.StateList = stateDd;
                if (Convert.ToInt32(frmCollection["StateCode"]) > 0)
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem
                    {
                        Text = "All Constituencies",
                        Value = "0"
                    });
                    foreach (SelectListItem i in new PMGSY.DAL.MasterReports.MasterReportsDAL().PopulateMLAConstituency(Convert.ToInt32(frmCollection["StateCode"])))
                    {
                        list.Add(i);
                    }
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                if (stateCode > 0)
                {
                    stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem
                    {
                        Text = "All Constituencies",
                        Value = "0"
                    });
                    foreach (SelectListItem i in new PMGSY.DAL.MasterReports.MasterReportsDAL().PopulateMLAConstituency(stateCode))
                    {
                        list.Add(i);
                    }
                    //ViewData["MLA_CONSTITUENCY"] = list;
                    objLoc.MLAConstituencyList = list;
                }
                if (stateCode == 0)
                {
                    List<SelectListItem> allConstituency = new List<SelectListItem>();
                    allConstituency.Add(new SelectListItem
                    {
                        Text = "All Constituencies",
                        Value = "0"
                    });
                    //ViewData["MLA_CONSTITUENCY"] = allConstituency;
                    objLoc.MLAConstituencyList = allConstituency;
                }
                SelectListItem all = new SelectListItem
                {
                    Text = "All",
                    Value = "%"
                };
                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);
                //ViewData["Active_TYPE"] = activeType;
                objLoc.ActiveFlagList = activeType;
                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MLABlockReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 9;
                    objLoc.Type1 = "%";
                    objLoc.Type2 = "%";
                    objLoc.Type3 = "%";
                    objLoc.Type4 = "%";
                    objLoc.Type5 = "%";

                    //objLoc.District = 0;
                    //objLoc.Block = 0;
                    //objLoc.Village = 0;
                    //objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    objLoc.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objLoc.State;
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        objLoc.District = PMGSYSession.Current.DistrictCode;
                    }
                    if (PMGSYSession.Current.StateName != null)
                    {
                        objLoc.StateName = objLoc.State == 0 ? "All States" : PMGSYSession.Current.StateName;
                    }
                    if (PMGSYSession.Current.DistrictName != null)
                    {
                        objLoc.DistName = objLoc.District == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                    }
                    objLoc.BlockName = objLoc.Block == 0 ? "All Blocks" : objLoc.BlockName;
                    objLoc.VillageName = "All Villages";

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Panchayat Habitations
        public ActionResult PanchayatHabitationLayout(FormCollection frmCollection)
        {
            //MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            LocationMasterReports objLoc = new LocationMasterReports();
            try
            {
                objLoc.Mast_State_Code = PMGSYSession.Current.StateCode;
                objLoc.Mast_District_Code = PMGSYSession.Current.DistrictCode;

                objLoc.State = PMGSYSession.Current.StateCode;
                objLoc.District = PMGSYSession.Current.DistrictCode;

                int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
                int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(frmCollection["DistrictCode"]) : PMGSYSession.Current.DistrictCode;
                List<SelectListItem> stateDd = comm.PopulateStates(false);
                stateDd.Find(x => x.Value == "1").Selected = true;

                // stateDd.Find(x => x.Value == "0").Text = "Select State";
                List<SelectListItem> districtDd = comm.PopulateDistrict(stateCode, true);
                districtDd.Find(x => x.Value == "-1").Text = "Select District";
                List<SelectListItem> blockDd = comm.PopulateBlocks(districtCode, true);
                blockDd.Find(x => x.Value == "-1").Text = "Select Block";
                List<SelectListItem> panchayatDd = new List<SelectListItem>();
                panchayatDd.Add(new SelectListItem
                {
                    Text = "All Panchayats",
                    Value = "0",
                    Selected = true
                });
                //panchayatDd.Concat(new MasterReportsDAL().PopulatePanchayat(Convert.ToInt32(frmCollection["BlockCode"]),districtCode,stateCode));

                if (districtCode > 0)
                {
                    districtDd.Find(x => x.Value == districtCode.ToString()).Selected = true;
                }

                if (stateCode > 0)  //if state login
                {
                    stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                if (Convert.ToInt32(frmCollection["BlockCode"]) > 0)
                {

                    List<SelectListItem> list = new PMGSY.DAL.MasterReports.MasterReportsDAL().PopulatePanchayat(Convert.ToInt32(frmCollection["BlockCode"]), Convert.ToInt32(frmCollection["DistrictCode"]),
                        Convert.ToInt32(frmCollection["StateCode"]));

                    list.Insert(0, new SelectListItem
                    {
                        Text = "All Panchayats",
                        Value = "0",
                        Selected = true
                    });
                    return Json(list, JsonRequestBehavior.AllowGet);
                }

                if (Convert.ToInt32(frmCollection["DistrictCode"]) > 0)
                {
                    return Json(comm.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"])), JsonRequestBehavior.AllowGet);
                }

                if (Convert.ToInt32(frmCollection["StateCode"]) > 0)    // if mord login
                {
                    return Json(comm.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"])), JsonRequestBehavior.AllowGet);
                }
                SelectListItem all = new SelectListItem
                {
                    Text = "All",
                    Value = "%"
                };
                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);

                objLoc.StateList = new SelectList(stateDd, "Value", "Text").ToList();
                objLoc.DistrictList = new SelectList(districtDd, "Value", "Text").ToList();
                objLoc.BlockList = new SelectList(blockDd, "Value", "Text").ToList();
                objLoc.PanchayatList = new SelectList(panchayatDd, "Value", "Text").ToList();
                objLoc.ActiveFlagList = new SelectList(activeType, "Value", "Text").ToList();
                //ViewData["Active_TYPE"] = activeType;

                //ViewData["STATE"] = stateDd;
                //ViewData["DISTRICT"] = districtDd;
                //ViewData["BLOCK"] = blockDd;
                //ViewData["PANCHAYAT"] = panchayatDd;
                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PanchayatHabitationReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 11;
                    objLoc.Type1 = "%";
                    objLoc.Type2 = "%";
                    objLoc.Type3 = "%";
                    objLoc.Type4 = "%";
                    objLoc.Type5 = "%";

                    //objLoc.District = 0;
                    //objLoc.Block = 0;
                    //objLoc.Village = 0;
                    //objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    objLoc.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objLoc.State;
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        objLoc.District = PMGSYSession.Current.DistrictCode;
                    }
                    if (PMGSYSession.Current.StateName != null)
                    {
                        objLoc.StateName = objLoc.State == 0 ? "All States" : PMGSYSession.Current.StateName;
                    }
                    if (PMGSYSession.Current.DistrictName != null)
                    {
                        objLoc.DistName = objLoc.District == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                    }
                    objLoc.BlockName = objLoc.Block == 0 ? "All Blocks" : objLoc.BlockName;
                    objLoc.VillageName = "All Villages";

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Region Master
        public ActionResult RegionLayout(FormCollection frmCollection)
        {
            //MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            LocationMasterReports objLoc = new LocationMasterReports();
            try
            {
                objLoc.Mast_State_Code = PMGSYSession.Current.StateCode;
                objLoc.Mast_District_Code = PMGSYSession.Current.DistrictCode;

                objLoc.State = PMGSYSession.Current.StateCode;
                objLoc.District = PMGSYSession.Current.DistrictCode;

                List<SelectListItem> stateDd = comm.PopulateStates(false);
                //if (PMGSYSession.Current.StateCode > 0)
                //{
                //    stateDd.Find(x => x.Value == PMGSYSession.Current.StateCode.ToString()).Selected = true;
                //}
                //else
                //{
                stateDd.Find(x => x.Value == "1").Selected = true;
                //}
                int stateCode = PMGSYSession.Current.StateCode;
                if (stateCode > 0)
                {
                    stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }
                //ViewData["STATE"] = stateDd;
                objLoc.StateList = new SelectList(stateDd, "Value", "Text").ToList();

                SelectListItem all = new SelectListItem
                {
                    Text = "All",
                    Value = "%"
                };
                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);
                //ViewData["Active_TYPE"] = activeType;
                objLoc.ActiveFlagList = new SelectList(activeType, "Value", "Text").ToList();

                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult RegionReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 12;
                    objLoc.Type1 = "%";
                    objLoc.Type2 = "%";
                    objLoc.Type3 = "%";
                    objLoc.Type4 = "%";
                    objLoc.Type5 = "%";

                    //objLoc.District = 0;
                    objLoc.Block = 0;
                    objLoc.Village = 0;
                    objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    objLoc.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objLoc.State;
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        objLoc.District = PMGSYSession.Current.DistrictCode;
                    }
                    if (PMGSYSession.Current.StateName != null)
                    {
                        objLoc.StateName = objLoc.State == 0 ? "All States" : PMGSYSession.Current.StateName;
                    }
                    if (PMGSYSession.Current.DistrictName != null)
                    {
                        objLoc.DistName = objLoc.District == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                    }
                    objLoc.BlockName = objLoc.Block == 0 ? "All Blocks" : objLoc.BlockName;
                    objLoc.VillageName = "All Villages";

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Region District
        public ActionResult RegionDistrictLayout(FormCollection frmCollection)
        {
            //MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            LocationMasterReports objLoc = new LocationMasterReports();
            try
            {
                objLoc.Mast_State_Code = PMGSYSession.Current.StateCode;
                objLoc.Mast_District_Code = PMGSYSession.Current.DistrictCode;

                objLoc.State = PMGSYSession.Current.StateCode;
                objLoc.District = PMGSYSession.Current.DistrictCode;

                List<SelectListItem> stateDd = comm.PopulateStates(false);
                stateDd.Find(x => x.Value == "1").Selected = true;

                int stateCode = PMGSYSession.Current.StateCode;
                if (stateCode > 0)
                {
                    stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }
                //ViewData["STATE"] = stateDd;
                objLoc.StateList = new SelectList(stateDd, "Value", "Text").ToList();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem
                {
                    Text = "All Regions",
                    Value = "0"
                });

                if (Convert.ToInt32(frmCollection["StateCode"]) > 0)
                {
                    foreach (SelectListItem i in new PMGSY.DAL.MasterReports.MasterReportsDAL().PopulateRegion(Convert.ToInt32(frmCollection["StateCode"])))
                    {
                        list.Add(i);
                    }
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                if (stateCode > 0)
                {
                    foreach (SelectListItem i in new PMGSY.DAL.MasterReports.MasterReportsDAL().PopulateRegion(stateCode))
                    {
                        list.Add(i);
                    }
                    //ViewData["REGION"] = list;
                    objLoc.RegionList = new SelectList(list, "Value", "Text").ToList();
                }
                if (stateCode == 0)
                {
                    //ViewData["REGION"] = list;
                    objLoc.RegionList = new SelectList(list, "Value", "Text").ToList();
                }
                SelectListItem all = new SelectListItem
                {
                    Text = "All",
                    Value = "%"
                };
                SelectListItem yes = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "No",
                    Value = "N"
                };
                List<SelectListItem> activeType = new List<SelectListItem>();
                activeType.Add(all);
                activeType.Add(yes);
                activeType.Add(no);
                //ViewData["Active_TYPE"] = activeType;
                objLoc.ActiveFlagList = new SelectList(activeType, "Value", "Text").ToList();

                return View(objLoc);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult RegionDistrictReport(LocationMasterReports objLoc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objLoc.RptNo = 13;
                    objLoc.Type1 = "%";
                    objLoc.Type2 = "%";
                    objLoc.Type3 = "%";
                    objLoc.Type4 = "%";
                    objLoc.Type5 = "%";

                    //objLoc.District = 0;
                    objLoc.Block = 0;
                    objLoc.Village = 0;
                    objLoc.Panchayat = 0;
                    objLoc.Year = 0;

                    objLoc.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objLoc.State;
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        objLoc.District = PMGSYSession.Current.DistrictCode;
                    }
                    if (PMGSYSession.Current.StateName != null)
                    {
                        objLoc.StateName = objLoc.State == 0 ? "All States" : PMGSYSession.Current.StateName;
                    }
                    if (PMGSYSession.Current.DistrictName != null)
                    {
                        objLoc.DistName = objLoc.District == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                    }
                    objLoc.BlockName = objLoc.Block == 0 ? "All Blocks" : objLoc.BlockName;
                    objLoc.VillageName = "All Villages";

                    return View(objLoc);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("StateLayout", objLoc);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
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
            list.Find(x => x.Value == "0").Value = "-1";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockSelectDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), false);
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult SelectDistrict(FormCollection frmCollection)
        //{
        //    LocationMasterReports objLoc = new LocationMasterReports();
        //    CommonFunctions objCommonFunctions = new CommonFunctions();
        //    //List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
        //    // list.Find(x => x.Value == "-1").Value = "0";
        //    //list.RemoveAt(0);
        //     objLoc.DistrictList = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
        //     objLoc.DistrictList.RemoveAt(0);
        //     return Json(objLoc.DistrictList, JsonRequestBehavior.AllowGet);
        //}
        #endregion

        #region Ommas System Codes
        [HttpGet]
        public ActionResult OmmasSystemCodesLayout()
        {
            OMMSSystemCodesViewModel model = new OMMSSystemCodesViewModel();
            try
            {
                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.StateList = comm.PopulateStates(true);
                    model.StateList.Find(x => x.Value == "0").Value = "-1";

                    model.DistrictList = new List<SelectListItem>();
                    model.DistrictList.Insert(0, new SelectListItem { Text = "All Districts", Value = "0" });
                }
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode) });

                    //model.DistrictList = new List<SelectListItem>();
                    //model.DistrictList.Insert(0, new SelectListItem { Text = "All Districts", Value = "0" });
                    model.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode);
                    model.DistrictList.Find(x => x.Value == "0").Text = "All Districts";
                }
                if (PMGSYSession.Current.DistrictCode > 0)
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode) });

                    model.DistrictList = new List<SelectListItem>();
                    model.DistrictList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.DistrictName, Value = Convert.ToString(PMGSYSession.Current.DistrictCode) });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationSSRSReports.OmmasSystemCodesLayout");
                return View(model);
            }
            finally
            {

            }
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult OmmasSystemCodesReport(OMMSSystemCodesViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.DistrictName = model.DistrictCode == 0 ? "All Districts" : model.DistrictName;
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationSSRSReports.OmmasSystemCodesReport");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region MP/MLA Constitution Codes
        public ActionResult MPMLAConCodesLayout()
        {
            MPMLAConCodesViewModel model = new MPMLAConCodesViewModel();
            try
            {
                model.ConstituencyTypeList = new List<SelectListItem>();
                model.ConstituencyTypeList.Insert(0, new SelectListItem { Text = "MP", Value = "P" });
                model.ConstituencyTypeList.Insert(1, new SelectListItem { Text = "MLA", Value = "L" });

                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.StateList = comm.PopulateStates(true);
                    model.StateList.Find(x => x.Value == "0").Value = "-1";
                }
                else
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode).Trim() });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
            finally
            {

            }
        }

        [HttpPost]
        public ActionResult MPMLAConCodesReport(MPMLAConCodesViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.StateName = model.StateCode == 0 ? "All States" : model.StateName;
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Excel Reports
        public ActionResult SystemCodesExcelLayout()
        {
            SystemCodesExcelViewModel model = new SystemCodesExcelViewModel();
            try
            {
                model.ReportList = new List<SelectListItem>();
                model.ReportList.Insert(0, new SelectListItem { Text = "Location Master", Value = "1" });
                model.ReportList.Insert(1, new SelectListItem { Text = "Village Master", Value = "2" });
                model.ReportList.Insert(2, new SelectListItem { Text = "Habitation Master", Value = "3" });
                model.ReportList.Insert(3, new SelectListItem { Text = "MP Constituency", Value = "4" });
                model.ReportList.Insert(4, new SelectListItem { Text = "MLA Constituency", Value = "5" });
                model.ReportList.Insert(5, new SelectListItem { Text = "Existing Roads", Value = "6" });
                model.ReportList.Insert(6, new SelectListItem { Text = "Core Network", Value = "7" });

                //model.ReportList.Insert(0, new SelectListItem { Text = "Location Master", Value = "1" });
                //model.ReportList.Insert(1, new SelectListItem { Text = "MP Constituency", Value = "2" });
                //model.ReportList.Insert(2, new SelectListItem { Text = "MLA Constituency", Value = "3" });
                //model.ReportList.Insert(3, new SelectListItem { Text = "Existing Roads", Value = "4" });
                //model.ReportList.Insert(4, new SelectListItem { Text = "Core Network", Value = "5" });

                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.StateList = comm.PopulateStates(true);
                    model.StateList.Find(x => x.Value == "0").Value = "-1";

                    model.DistrictList = new List<SelectListItem>();
                    model.DistrictList.Insert(0, new SelectListItem { Text = "Select District", Value = "-1" });
                }
                else
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode).Trim() });

                    model.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode);
                    model.DistrictList.Find(x => x.Value == "0").Text = "All Districts";
                }
                if (PMGSYSession.Current.DistrictCode > 0)
                {
                    model.DistrictList = new List<SelectListItem>();
                    model.DistrictList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.DistrictName, Value = Convert.ToString(PMGSYSession.Current.DistrictCode) });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationSSRSReports.SystemCodesExcelLayout");
                return View(model);
            }
            finally
            {

            }
        }

        //[Audit]
        //[HttpPost]
        public ActionResult SystemCodesExcelReport(string rpt)
        {
            #region
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            SystemCodesExcelViewModel model = new SystemCodesExcelViewModel();
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
                switch (param[0].Trim())
                {
                    case "1":
                        rview.ServerReport.ReportPath = "/PMGSYCitizen/Ommas_System_Codes_Report";
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("STATE", param[1].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DISTRICT", param[3].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", param[2].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DistrictName", param[4].Trim()));
                        fName = "LOC_Codes_" + param[1].Trim() + "_" + DateTime.Now.ToShortDateString();
                        break;
                    case "2":
                        rview.ServerReport.ReportPath = "/PMGSYCitizen/Master_Village_Report";
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("STATE", param[1].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DISTRICT", param[3].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("BLOCK", "0"));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PMGSY", PMGSYSession.Current.PMGSYScheme.ToString()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", param[2].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DistrictName", param[4].Trim()));
                        fName = "VIL_Codes_" + param[1].Trim() + "_" + DateTime.Now.ToShortDateString();
                        break;
                    case "3":
                        rview.ServerReport.ReportPath = "/PMGSYCitizen/Master_Habitation_Report";
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("STATE", param[1].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DISTRICT", param[3].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("BLOCK", "0"));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PMGSY", PMGSYSession.Current.PMGSYScheme.ToString()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", param[2].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DistrictName", param[4].Trim()));
                        fName = "HAB_Codes_" + param[1].Trim() + "_" + DateTime.Now.ToShortDateString();
                        break;
                    case "4":
                        rview.ServerReport.ReportPath = "/PMGSYCitizen/MP_MLA_CON_Codes_Report";
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("CONTYPE", "P"));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("STATE", param[1].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", param[2].Trim()));
                        fName = "MP_Codes_" + param[1].Trim() + "_" + DateTime.Now.ToShortDateString();
                        break;
                    case "5":
                        rview.ServerReport.ReportPath = "/PMGSYCitizen/MP_MLA_CON_Codes_Report";
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("CONTYPE", "L"));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("STATE", param[1].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", param[2].Trim()));
                        fName = "MLA_Codes_" + param[1].Trim() + "_" + DateTime.Now.ToShortDateString();
                        break;
                    case "6":
                        rview.ServerReport.ReportPath = "/PMGSYCitizen/Master_DRRP_Report";
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("STATE", param[1].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DISTRICT", param[3].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("BLOCK", "0"));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PMGSY", PMGSYSession.Current.PMGSYScheme.ToString()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("RD_TYPE", "E"));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", param[2].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DistrictName", param[4].Trim()));
                        fName = "ER_Codes_" + param[1].Trim() + "_" + DateTime.Now.ToShortDateString();
                        break;
                    case "7":
                        rview.ServerReport.ReportPath = "/PMGSYCitizen/Master_CoreNetwork_Report";
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("STATE", param[1].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DISTRICT", param[3].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("BLOCK", "0"));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PMGSY", PMGSYSession.Current.PMGSYScheme.ToString()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("RD_TYPE", "C"));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", param[2].Trim()));
                        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DistrictName", param[4].Trim()));
                        fName = "CN_Codes_" + param[1].Trim() + "_" + DateTime.Now.ToShortDateString();
                        break;
                }
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

        #region Forth Nightly Report
        [HttpGet]
        public ActionResult ForthNightelyReportLayout()
        {
            ForthNightelyReportViewModel model = new ForthNightelyReportViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.FromYear = DateTime.Now.Year;
                model.ToYear = DateTime.Now.Year;

                model.FromMonth = DateTime.Now.Month - 2;
                model.ToMonth = DateTime.Now.Month - 1;

                model.FromYearList = comm.PopulateYears(false);
                model.ToYearList = comm.PopulateYears(false);
                model.FromMonthList = comm.PopulateMonths(false);
                model.ToMonthList = comm.PopulateMonths(false);

                model.StreamList = comm.PopulateFundingAgency();
                model.StreamList.Find(x => x.Value == "0").Text = "All Streams";

                model.pmgsySchemeList = new List<SelectListItem>();
                model.pmgsySchemeList.Insert(0, new SelectListItem { Text = "Both", Value = "0" });
                model.pmgsySchemeList.Insert(1, new SelectListItem { Text = "PMGSY1", Value = "1" });
                model.pmgsySchemeList.Insert(1, new SelectListItem { Text = "PMGSY2", Value = "2" });

                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
            finally
            {

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForthNightelyReport(ForthNightelyReportViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //model.DistrictName = model.DistrictCode == 0 ? "All Districts" : model.DistrictName;
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Habitation Facility Report
        [HttpGet]
        public ActionResult HabitationFaclityLayout()
        {
            HabitationFaclityViewModel model = new HabitationFaclityViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.StateList = comm.PopulateStates(false);
                    model.DistrictList = new List<SelectListItem>();
                    model.DistrictList.Insert(0, new SelectListItem { Text = "All Districts", Value = "0" });

                    model.BlockList = new List<SelectListItem>();
                    model.BlockList.Insert(0, new SelectListItem { Text = "All Blocks", Value = "0" });
                }
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                    model.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.DistrictList.Find(x => x.Value == "-1").Text = "Select District";

                    model.BlockList = new List<SelectListItem>();
                    model.BlockList.Insert(0, new SelectListItem { Text = "Select Block", Value = "-1" });

                    model.DistrictCode = (PMGSYSession.Current.DistrictCode > 0) ? PMGSYSession.Current.DistrictCode : 0;
                }
                if (PMGSYSession.Current.DistrictCode > 0)
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });

                    model.DistrictList = new List<SelectListItem>();
                    model.DistrictList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString() });

                    model.BlockList = comm.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    model.BlockList.Find(x => x.Value == "-1").Value = "0";

                    //model.BlockCode = (PMGSYSession.Current.DistrictCode > 0) ? PMGSYSession.Current.DistrictCode : 0;
                }

                model.YearList = new List<SelectListItem>();
                model.YearList.Insert(0, new SelectListItem { Text = "2001", Value = "2001" });
                //model.YearList.Insert(1, new SelectListItem { Text = "2011", Value = "2011" });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationSSRSReports.HabitationFaclityLayout()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult HabitationFaclityReport(HabitationFaclityViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationSSRSReports.HabitationFaclityReport()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
