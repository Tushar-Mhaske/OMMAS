using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Areas.StateProfilePrintReports.Models;
using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Extensions;
using PMGSY.Controllers;

namespace PMGSY.Areas.StateProfilePrintReports.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class StateProfilePrintController : Controller
    {
        //
        // GET: /StateProfilePrintReports/StateProfilePrint/

        PMGSYEntities dbContext;
        CommonFunctions comm = new CommonFunctions();

        #region State Profile Report 

        [HttpGet]
        public ActionResult ECBriefReportLayout()
        {
            try
            {
                ECBriefReportViewModel EC = new ECBriefReportViewModel();

                if (PMGSYSession.Current.StateCode > 0)
                {
                    EC.StateCode = PMGSYSession.Current.StateCode;
                    EC.StateName = PMGSYSession.Current.StateName;
                    EC.State_Name = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(EC.StateCode), Text = Convert.ToString(EC.StateName) });
                    EC.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        EC.DistrictCode = PMGSYSession.Current.DistrictCode;
                        EC.DistName = PMGSYSession.Current.DistrictName;

                        List<SelectListItem> lstDist = new List<SelectListItem>();
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(EC.DistrictCode), Text = Convert.ToString(EC.DistName) });
                        EC.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        EC.BlockList = comm.PopulateBlocks(EC.DistrictCode, true);
                        EC.BlockList.RemoveAt(0);
                        EC.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    }
                    else
                    {
                        EC.DistrictList = comm.PopulateDistrict(EC.StateCode, true);
                        EC.DistrictList.RemoveAt(0);
                        EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                        EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    }

                    //EC.AgencyCode = PMGSYSession.Current.AdminNdCode;
                    //EC.AgencyName = PMGSYSession.Current.DepartmentName.Trim();

                    List<SelectListItem> lstAgency = new List<SelectListItem>();
                    EC.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(EC.StateCode, PMGSYSession.Current.AdminNdCode, false);
                    EC.AgencyList.RemoveAt(0);

                    //EC.AgencyList = comm.PopulateAgencies(EC.StateCode, true);
                    //EC.AgencyCode = Convert.ToInt32(EC.AgencyList.Where(m => m.Selected == true).Select(m => m.Value).LastOrDefault());
                    //EC.AgencyList.RemoveAt(0);

                    //EC.CollaborationCode = 2;
                    EC.CollaborationList = PopulateCollaborationsStateWise(EC.StateCode, true);
                    //EC.CollaborationList.RemoveAt(0);
                }
                else
                {
                    EC.StateList = comm.PopulateStates(true);
                    EC.StateList.RemoveAt(0);
                    EC.StateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                    EC.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                    List<SelectListItem> lstCollab = new List<SelectListItem>();
                    lstCollab.Insert(0, new SelectListItem { Value = "0", Text = "Select Collaboration" });
                    EC.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                    List<SelectListItem> lstAgency = new List<SelectListItem>();
                    lstAgency.Insert(0, new SelectListItem { Value = "0", Text = "Select Agency" });
                    EC.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                }

                EC.YearList = comm.PopulateFinancialYear(true, true).ToList();
                EC.YearList.RemoveAt(0);
                //EC.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
                EC.BatchList = comm.PopulateBatch();
                EC.BatchList.RemoveAt(0);
                EC.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });
                return View(EC);
            }
            catch
            {
                return null;
            }
        }


        [HttpPost]
        public ActionResult ECBriefReport(ECBriefReportViewModel EC)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EC.LevelCode = 1;
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        EC.State_Name = PMGSYSession.Current.StateName;
                    }

                    //objAnalysisProposalModel.LevelCode = objAnalysisProposalModel.RoadWise == true ? 4 : objAnalysisProposalModel.BlockCode > 0 ? 3 : objAnalysisProposalModel.DistrictCode > 0 ? 2 : 1;
                    //objAnalysisProposalModel.Mast_State_Code = objAnalysisProposalModel.StateCode > 0 ? objAnalysisProposalModel.StateCode : objAnalysisProposalModel.Mast_State_Code;
                    //objAnalysisProposalModel.Mast_District_Code = objAnalysisProposalModel.DistrictCode > 0 ? objAnalysisProposalModel.DistrictCode : objAnalysisProposalModel.Mast_District_Code;
                    //objAnalysisProposalModel.Mast_Block_Code = objAnalysisProposalModel.BlockCode > 0 ? objAnalysisProposalModel.BlockCode : objAnalysisProposalModel.Mast_Block_Code;

                    return View(EC);
                }
                else
                {

                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        EC.StateCode = PMGSYSession.Current.StateCode;
                        EC.StateName = PMGSYSession.Current.StateName;
                        EC.State_Name = PMGSYSession.Current.StateName;

                        List<SelectListItem> lstState = new List<SelectListItem>();
                        lstState.Insert(0, new SelectListItem { Value = Convert.ToString(EC.StateCode), Text = Convert.ToString(EC.StateName) });
                        EC.StateList = new SelectList(lstState, "Value", "Text").ToList();

                        if (PMGSYSession.Current.DistrictCode > 0)
                        {
                            EC.DistrictCode = PMGSYSession.Current.DistrictCode;
                            EC.DistName = PMGSYSession.Current.DistrictName;

                            List<SelectListItem> lstDist = new List<SelectListItem>();
                            lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(EC.DistrictCode), Text = Convert.ToString(EC.DistName) });
                            EC.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                            List<SelectListItem> lstBlock = new List<SelectListItem>();
                            EC.BlockList = comm.PopulateBlocks(EC.DistrictCode, true);
                            EC.BlockList.RemoveAt(0);
                            EC.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                        }
                        else
                        {
                            EC.DistrictList = comm.PopulateDistrict(EC.StateCode, true);
                            EC.DistrictList.RemoveAt(0);
                            EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                            List<SelectListItem> lstBlock = new List<SelectListItem>();
                            lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                            EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                        }

                        EC.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(EC.StateCode, PMGSYSession.Current.AdminNdCode, false);
                        EC.AgencyList.RemoveAt(0);

                        //EC.AgencyList = comm.PopulateAgencies(EC.StateCode, true);
                        //EC.AgencyCode = Convert.ToInt32(EC.AgencyList.Where(m => m.Selected == true).Select(m => m.Value).LastOrDefault());
                        //EC.AgencyList.RemoveAt(0);

                        //EC.CollaborationCode = 2;
                        EC.CollaborationList = PopulateCollaborationsStateWise(EC.StateCode, true);
                        //EC.CollaborationList.RemoveAt(0);
                    }
                    else
                    {
                        EC.StateList = comm.PopulateStates(true);
                        EC.StateList.RemoveAt(0);
                        EC.StateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });

                        List<SelectListItem> lstDistricts = new List<SelectListItem>();
                        lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                        EC.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                        List<SelectListItem> lstCollab = new List<SelectListItem>();
                        lstCollab.Insert(0, new SelectListItem { Value = "0", Text = "Select Collaboration" });
                        EC.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                        List<SelectListItem> lstAgency = new List<SelectListItem>();
                        lstAgency.Insert(0, new SelectListItem { Value = "0", Text = "Select Agency" });
                        EC.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
                    }

                    //List<SelectListItem> lstBlock = new List<SelectListItem>();
                    //lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    //EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();

                    EC.YearList = comm.PopulateFinancialYear(true, true).ToList();
                    //EC.YearList.RemoveAt(0);
                    EC.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
                    EC.BatchList = comm.PopulateBatch();
                    EC.BatchList.RemoveAt(0);
                    EC.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });

                    return View("ECBriefReportLayout", EC);
                }
            }
            catch
            {
                return View(EC);
            }

        }



        /// <summary>
        /// State Specific Collaborations
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PopulateCollaborations()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);

                return Json(PopulateCollaborationsStateWise(stateCode, true));
            }
            catch
            {
                return Json(new { string.Empty });
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
                //CommonFunctions objCommonFunctions = new CommonFunctions();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);

                return Json(comm.PopulateAgencies(stateCode, true));
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        /// <summary>
        /// Populate district based on selected state
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PopulateBlocks()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int DistCode = Convert.ToInt32(Request.Params["DistrictCode"]);

                List<SelectListItem> lstBlock = new List<SelectListItem>();
                lstBlock = objCommonFunctions.PopulateBlocks(DistCode, true);
                lstBlock.RemoveAt(0);
                lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });

                return Json(lstBlock);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        /// <summary>
        /// Populate district based on selected state
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PopulateDistricts()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                List<SelectListItem> lstDist = new List<SelectListItem>();
                lstDist = objCommonFunctions.PopulateDistrict(stateCode, true);
                lstDist.RemoveAt(0);
                lstDist.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
                return Json(lstDist);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        /// <summary>
        /// State Specific Collaborations
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="isAllSelected"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateCollaborationsStateWise(int stateCode, bool isAllSelected = false)
        {
            List<SelectListItem> lstCollaborations = new List<SelectListItem>();
            SelectListItem item;

            try
            {
                dbContext = new PMGSYEntities();

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

        [HttpGet]
        public ActionResult StateProfileClusterLayout()
        {
            try
            {
                ECBriefReportViewModel EC = new ECBriefReportViewModel();

                if (PMGSYSession.Current.StateCode > 0)
                {
                    EC.StateCode = PMGSYSession.Current.StateCode;
                    EC.StateName = PMGSYSession.Current.StateName;
                    EC.State_Name = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(EC.StateCode), Text = Convert.ToString(EC.StateName) });
                    EC.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        EC.DistrictCode = PMGSYSession.Current.DistrictCode;
                        EC.DistName = PMGSYSession.Current.DistrictName;

                        List<SelectListItem> lstDist = new List<SelectListItem>();
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(EC.DistrictCode), Text = Convert.ToString(EC.DistName) });
                        EC.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        EC.BlockList = comm.PopulateBlocks(EC.DistrictCode, true);
                        EC.BlockList.RemoveAt(0);
                        EC.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    }
                    else
                    {
                        EC.DistrictList = comm.PopulateDistrict(EC.StateCode, true);
                        EC.DistrictList.RemoveAt(0);
                        EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                        EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    }

                    //EC.AgencyCode = PMGSYSession.Current.AdminNdCode;
                    //EC.AgencyName = PMGSYSession.Current.DepartmentName.Trim();

                    List<SelectListItem> lstAgency = new List<SelectListItem>();
                    EC.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(EC.StateCode, PMGSYSession.Current.AdminNdCode, false);
                    EC.AgencyList.RemoveAt(0);

                    //EC.AgencyList = comm.PopulateAgencies(EC.StateCode, true);
                    //EC.AgencyCode = Convert.ToInt32(EC.AgencyList.Where(m => m.Selected == true).Select(m => m.Value).LastOrDefault());
                    //EC.AgencyList.RemoveAt(0);

                    //EC.CollaborationCode = 2;
                    EC.CollaborationList = PopulateCollaborationsStateWise(EC.StateCode, true);
                    //EC.CollaborationList.RemoveAt(0);
                }
                else
                {
                    EC.StateList = comm.PopulateStates(true);
                    EC.StateList.RemoveAt(0);
                    EC.StateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                    EC.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                    List<SelectListItem> lstCollab = new List<SelectListItem>();
                    lstCollab.Insert(0, new SelectListItem { Value = "0", Text = "Select Collaboration" });
                    EC.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                    List<SelectListItem> lstAgency = new List<SelectListItem>();
                    lstAgency.Insert(0, new SelectListItem { Value = "0", Text = "Select Agency" });
                    EC.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                }

                EC.YearList = comm.PopulateFinancialYear(true, true).ToList();
                EC.YearList.RemoveAt(0);
                //EC.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
                EC.BatchList = comm.PopulateBatch();
                EC.BatchList.RemoveAt(0);
                EC.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });
                return View(EC);
            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex, "StateProfileClusterLayout");
                return null;
            }
        }

        [HttpPost]
        public ActionResult StateProfileClusterReport(ECBriefReportViewModel EC)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EC.LevelCode = 1;
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        EC.State_Name = PMGSYSession.Current.StateName;
                    }

                    //objAnalysisProposalModel.LevelCode = objAnalysisProposalModel.RoadWise == true ? 4 : objAnalysisProposalModel.BlockCode > 0 ? 3 : objAnalysisProposalModel.DistrictCode > 0 ? 2 : 1;
                    //objAnalysisProposalModel.Mast_State_Code = objAnalysisProposalModel.StateCode > 0 ? objAnalysisProposalModel.StateCode : objAnalysisProposalModel.Mast_State_Code;
                    //objAnalysisProposalModel.Mast_District_Code = objAnalysisProposalModel.DistrictCode > 0 ? objAnalysisProposalModel.DistrictCode : objAnalysisProposalModel.Mast_District_Code;
                    //objAnalysisProposalModel.Mast_Block_Code = objAnalysisProposalModel.BlockCode > 0 ? objAnalysisProposalModel.BlockCode : objAnalysisProposalModel.Mast_Block_Code;

                    return View(EC);
                }
                else
                {

                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        EC.StateCode = PMGSYSession.Current.StateCode;
                        EC.StateName = PMGSYSession.Current.StateName;
                        EC.State_Name = PMGSYSession.Current.StateName;

                        List<SelectListItem> lstState = new List<SelectListItem>();
                        lstState.Insert(0, new SelectListItem { Value = Convert.ToString(EC.StateCode), Text = Convert.ToString(EC.StateName) });
                        EC.StateList = new SelectList(lstState, "Value", "Text").ToList();

                        if (PMGSYSession.Current.DistrictCode > 0)
                        {
                            EC.DistrictCode = PMGSYSession.Current.DistrictCode;
                            EC.DistName = PMGSYSession.Current.DistrictName;

                            List<SelectListItem> lstDist = new List<SelectListItem>();
                            lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(EC.DistrictCode), Text = Convert.ToString(EC.DistName) });
                            EC.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                            List<SelectListItem> lstBlock = new List<SelectListItem>();
                            EC.BlockList = comm.PopulateBlocks(EC.DistrictCode, true);
                            EC.BlockList.RemoveAt(0);
                            EC.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                        }
                        else
                        {
                            EC.DistrictList = comm.PopulateDistrict(EC.StateCode, true);
                            EC.DistrictList.RemoveAt(0);
                            EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                            List<SelectListItem> lstBlock = new List<SelectListItem>();
                            lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                            EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                        }

                        EC.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(EC.StateCode, PMGSYSession.Current.AdminNdCode, false);
                        EC.AgencyList.RemoveAt(0);

                        //EC.AgencyList = comm.PopulateAgencies(EC.StateCode, true);
                        //EC.AgencyCode = Convert.ToInt32(EC.AgencyList.Where(m => m.Selected == true).Select(m => m.Value).LastOrDefault());
                        //EC.AgencyList.RemoveAt(0);

                        //EC.CollaborationCode = 2;
                        EC.CollaborationList = PopulateCollaborationsStateWise(EC.StateCode, true);
                        //EC.CollaborationList.RemoveAt(0);
                    }
                    else
                    {
                        EC.StateList = comm.PopulateStates(true);
                        EC.StateList.RemoveAt(0);
                        EC.StateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });

                        List<SelectListItem> lstDistricts = new List<SelectListItem>();
                        lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                        EC.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                        List<SelectListItem> lstCollab = new List<SelectListItem>();
                        lstCollab.Insert(0, new SelectListItem { Value = "0", Text = "Select Collaboration" });
                        EC.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                        List<SelectListItem> lstAgency = new List<SelectListItem>();
                        lstAgency.Insert(0, new SelectListItem { Value = "0", Text = "Select Agency" });
                        EC.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
                    }

                    //List<SelectListItem> lstBlock = new List<SelectListItem>();
                    //lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    //EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();

                    EC.YearList = comm.PopulateFinancialYear(true, true).ToList();
                    //EC.YearList.RemoveAt(0);
                    EC.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
                    EC.BatchList = comm.PopulateBatch();
                    EC.BatchList.RemoveAt(0);
                    EC.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });

                    return View("StateProfileClusterLayout", EC);
                }
            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex, "StateProfileClusterReport");
                return View(EC);
            }
        }
    }
}
