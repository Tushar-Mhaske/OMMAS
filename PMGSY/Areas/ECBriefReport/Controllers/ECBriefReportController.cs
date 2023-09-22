using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Areas.ECBriefReport.Models;
using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Extensions;
using PMGSY.Controllers;

namespace PMGSY.Areas.ECBriefReport.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class ECBriefReportController : Controller
    {
        //
        // GET: /ECBriefReport/ECBriefReport/

        PMGSYEntities dbContext;
        CommonFunctions comm = new CommonFunctions();

        #region EC Brief Report
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
        public ActionResult ECCheckListLayout()
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

                    EC.DistrictList = comm.PopulateDistrict(EC.StateCode, true);
                    EC.DistrictList.RemoveAt(0);
                    EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                    //EC.AgencyCode = PMGSYSession.Current.AdminNdCode;
                    //EC.AgencyName = PMGSYSession.Current.DepartmentName.Trim();

                    EC.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(EC.StateCode, PMGSYSession.Current.AdminNdCode, false);
                    EC.AgencyList.RemoveAt(0);

                    EC.CollaborationList = PopulateCollaborationsStateWise(EC.StateCode, true);
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
                    lstAgency.Insert(0, new SelectListItem { Value = "-1", Text = "Select Agency" });
                    EC.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
                }

                List<SelectListItem> lstBlock = new List<SelectListItem>();
                lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();

                EC.YearList = comm.PopulateFinancialYear(true, true).ToList();
                EC.YearList.RemoveAt(0);

                EC.BatchList = comm.PopulateBatch();
                EC.BatchList.RemoveAt(0);
                EC.BatchList.Insert(0, new SelectListItem { Value = "-1", Text = "Select Batch" });
                return View(EC);
            }
            catch
            {
                return null;
            }
        }


        [HttpPost]
        public ActionResult ECCheckListReport(ECBriefReportViewModel EC)
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

                        EC.DistrictList = comm.PopulateDistrict(EC.StateCode, true);
                        EC.DistrictList.RemoveAt(0);
                        EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                        //EC.AgencyList = comm.PopulateAgencies(EC.StateCode, false);
                        //EC.AgencyCode = Convert.ToInt32(EC.AgencyList.Where(m => m.Selected == true).Select(m => m.Value).LastOrDefault());

                        EC.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(EC.StateCode, PMGSYSession.Current.AdminNdCode, false);
                        EC.AgencyList.RemoveAt(0);

                        EC.CollaborationList = PopulateCollaborationsStateWise(EC.StateCode, true);

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
                        lstAgency.Insert(0, new SelectListItem { Value = "-1", Text = "Select Agency" });
                        EC.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
                    }

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();

                    EC.YearList = comm.PopulateFinancialYear(true, true).ToList();

                    EC.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
                    EC.BatchList = comm.PopulateBatch();
                    EC.BatchList.RemoveAt(0);
                    EC.BatchList.Insert(0, new SelectListItem { Value = "-1", Text = "Select Batch" });

                    return View("ECCheckListLayout", EC);
                }
            }
            catch
            {
                return View(EC);
            }

        }


        [HttpPost]
        public JsonResult PopulateAgenciesChkList()
        {
            try
            {
                //CommonFunctions objCommonFunctions = new CommonFunctions();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);

                return Json(comm.PopulateAgencies(stateCode, false));
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        #region Funding Agency Phase Profile

        [HttpGet]
        public ActionResult FAPhaseProfileLayout()
        {
            try
            {
                FAPhaseProfileReportViewModel FA = new FAPhaseProfileReportViewModel();

                if (PMGSYSession.Current.StateCode > 0)
                {
                    FA.StateCode = PMGSYSession.Current.StateCode;
                    FA.StateName = PMGSYSession.Current.StateName;
                    //FA.State_Name = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(FA.StateCode), Text = Convert.ToString(FA.StateName) });
                    FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        FA.DistrictCode = PMGSYSession.Current.DistrictCode;
                        FA.DistName = PMGSYSession.Current.DistrictName;

                        List<SelectListItem> lstDist = new List<SelectListItem>();
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(FA.DistrictCode), Text = Convert.ToString(FA.DistName) });
                        FA.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        FA.BlockList = comm.PopulateBlocks(FA.DistrictCode, true);
                        FA.BlockList.RemoveAt(0);
                        FA.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    }
                    else
                    {
                        FA.DistrictList = comm.PopulateDistrict(FA.StateCode, true);
                        FA.DistrictList.RemoveAt(0);
                        FA.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                        FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    }

                    //FA.AgencyCode = PMGSYSession.Current.AdminNdCode;
                    //FA.AgencyName = PMGSYSession.Current.DepartmentName.Trim();

                    List<SelectListItem> lstAgency = new List<SelectListItem>();
                    //lstAgency.Insert(0, new SelectListItem { Value = Convert.ToString(FA.AgencyCode), Text = Convert.ToString(FA.AgencyName) });
                    //FA.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
                    FA.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(FA.StateCode, PMGSYSession.Current.AdminNdCode, false);
                    FA.AgencyList.RemoveAt(0);

                    FA.CollaborationList = PopulateCollaborationsStateWise(FA.StateCode, true);
                }
                #region
                else
                {
                    FA.StateList = comm.PopulateStates(true);
                    FA.StateList.RemoveAt(0);
                    FA.StateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });

                    //List<SelectListItem> lstState = new List<SelectListItem>();
                    //lstState.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });
                    //FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });
                    FA.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                    List<SelectListItem> lstCollab = new List<SelectListItem>();
                    lstCollab.Insert(0, new SelectListItem { Value = "-1", Text = "Select Collaboration" });
                    FA.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                    List<SelectListItem> lstAgency = new List<SelectListItem>();
                    lstAgency.Insert(0, new SelectListItem { Value = "-1", Text = "Select Agency" });
                    FA.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                }
                #endregion


                return View(FA);
            }
            catch
            {
                return null;
            }
        }


        [HttpPost]
        public ActionResult FAPhaseProfileReport(FAPhaseProfileReportViewModel FA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FA.LevelCode = 1;

                    return View(FA);
                }
                else
                {
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        FA.StateCode = PMGSYSession.Current.StateCode;
                        FA.StateName = PMGSYSession.Current.StateName;
                        //FA.State_Name = PMGSYSession.Current.StateName;

                        List<SelectListItem> lstState = new List<SelectListItem>();
                        lstState.Insert(0, new SelectListItem { Value = Convert.ToString(FA.StateCode), Text = Convert.ToString(FA.StateName) });
                        FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                        FA.DistrictList = comm.PopulateDistrict(FA.StateCode, true);
                        FA.DistrictList.RemoveAt(0);
                        FA.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                        List<SelectListItem> lstAgency = new List<SelectListItem>();
                        FA.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(FA.StateCode, PMGSYSession.Current.AdminNdCode, false);
                        FA.AgencyList.RemoveAt(0);

                        FA.CollaborationList = PopulateCollaborationsStateWise(FA.StateCode, true);
                    }
                    #region
                    else
                    {
                        List<SelectListItem> lstState = new List<SelectListItem>();
                        lstState.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });
                        FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                        List<SelectListItem> lstDistricts = new List<SelectListItem>();
                        lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                        FA.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                        List<SelectListItem> lstCollab = new List<SelectListItem>();
                        lstCollab.Insert(0, new SelectListItem { Value = "0", Text = "Select Collaboration" });
                        FA.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                        List<SelectListItem> lstAgency = new List<SelectListItem>();
                        lstAgency.Insert(0, new SelectListItem { Value = "-1", Text = "Select Agency" });
                        FA.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
                    }
                    #endregion
                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    return View("FAPhaseProfileLayout", FA);
                }
            }
            catch
            {
                return View(FA);
            }
        }
        #endregion

        #region State Profile Report
        [HttpGet]
        public ActionResult StateProfileLayout()
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                FAPhaseProfileReportViewModel FA = new FAPhaseProfileReportViewModel();

                if (PMGSYSession.Current.StateCode > 0)
                {
                    FA.StateCode = PMGSYSession.Current.StateCode;
                    FA.StateName = PMGSYSession.Current.StateName;
                    //FA.State_Name = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(FA.StateCode), Text = Convert.ToString(FA.StateName) });
                    FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        FA.DistrictCode = PMGSYSession.Current.DistrictCode;
                        FA.DistName = PMGSYSession.Current.DistrictName;

                        List<SelectListItem> lstDist = new List<SelectListItem>();
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(FA.DistrictCode), Text = Convert.ToString(FA.DistName) });
                        FA.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                        FA.BlockList = comm.PopulateBlocks(FA.DistrictCode, true);
                        FA.BlockList.RemoveAt(0);
                        FA.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    }
                    else
                    {
                        FA.DistrictList = comm.PopulateDistrict(FA.StateCode, true);
                        FA.DistrictList.RemoveAt(0);
                        FA.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                        FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    }

                    List<SelectListItem> lstAgency = new List<SelectListItem>();

                    FA.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(FA.StateCode, PMGSYSession.Current.AdminNdCode, false);
                    FA.AgencyList.RemoveAt(0);

                    FA.CollaborationList = PopulateCollaborationsStateWise(FA.StateCode, true);
                }
                #region
                else
                {
                    FA.StateList = comm.PopulateStates(true);
                    FA.StateList.RemoveAt(0);
                    FA.StateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });
                    FA.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                    List<SelectListItem> lstCollab = new List<SelectListItem>();
                    lstCollab.Insert(0, new SelectListItem { Value = "-1", Text = "Select Collaboration" });
                    FA.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                    List<SelectListItem> lstAgency = new List<SelectListItem>();
                    lstAgency.Insert(0, new SelectListItem { Value = "-1", Text = "Select Agency" });
                    FA.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                }
                #endregion

                return View(FA);
            }
            catch
            {
                return null;
            }
        }


        [HttpPost]
        public ActionResult StateProfileReport(FAPhaseProfileReportViewModel FA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FA.LevelCode = 1;

                    return View(FA);
                }
                else
                {
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        FA.StateCode = PMGSYSession.Current.StateCode;
                        FA.StateName = PMGSYSession.Current.StateName;
                        //FA.State_Name = PMGSYSession.Current.StateName;

                        List<SelectListItem> lstState = new List<SelectListItem>();
                        lstState.Insert(0, new SelectListItem { Value = Convert.ToString(FA.StateCode), Text = Convert.ToString(FA.StateName) });
                        FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                        if (PMGSYSession.Current.DistrictCode > 0)
                        {
                            FA.DistrictCode = PMGSYSession.Current.DistrictCode;
                            FA.DistName = PMGSYSession.Current.DistrictName;

                            List<SelectListItem> lstDist = new List<SelectListItem>();
                            lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(FA.DistrictCode), Text = Convert.ToString(FA.DistName) });
                            FA.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                            FA.BlockList = comm.PopulateBlocks(FA.DistrictCode, true);
                            FA.BlockList.RemoveAt(0);
                            FA.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                        }
                        else
                        {
                            FA.DistrictList = comm.PopulateDistrict(FA.StateCode, true);
                            FA.DistrictList.RemoveAt(0);
                            FA.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                            List<SelectListItem> lstBlock = new List<SelectListItem>();
                            lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                            FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                        }

                        List<SelectListItem> lstAgency = new List<SelectListItem>();
                        FA.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(FA.StateCode, PMGSYSession.Current.AdminNdCode, false);
                        FA.AgencyList.RemoveAt(0);

                        FA.CollaborationList = PopulateCollaborationsStateWise(FA.StateCode, true);
                    }
                    #region
                    else
                    {
                        List<SelectListItem> lstState = new List<SelectListItem>();
                        lstState.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });
                        FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                        List<SelectListItem> lstDistricts = new List<SelectListItem>();
                        lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                        FA.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                        List<SelectListItem> lstCollab = new List<SelectListItem>();
                        lstCollab.Insert(0, new SelectListItem { Value = "0", Text = "Select Collaboration" });
                        FA.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                        List<SelectListItem> lstAgency = new List<SelectListItem>();
                        lstAgency.Insert(0, new SelectListItem { Value = "-1", Text = "Select Agency" });
                        FA.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                        FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    }
                    #endregion

                    return View("StateProfileLayout", FA);
                }
            }
            catch
            {
                return View(FA);
            }

        }

        #endregion


        #region PMGSY Achievements

        [HttpGet]
        public ActionResult PMGSYAchievementsLayout()
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

                    //List<SelectListItem> lstAgency = new List<SelectListItem>();
                    //EC.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(EC.StateCode, PMGSYSession.Current.AdminNdCode, false);
                    //EC.AgencyList.RemoveAt(0);



                    EC.CollaborationList = PopulateCollaborationsStateWise(EC.StateCode, true);
                }
                else
                {
                    EC.StateList = comm.PopulateStates(true);
                    EC.StateList.RemoveAt(0);
                    EC.StateList.Insert(0, new SelectListItem { Value = "0", Text = "All States" });

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
                    EC.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                    List<SelectListItem> lstCollab = new List<SelectListItem>();
                    lstCollab.Insert(0, new SelectListItem { Value = "0", Text = "All Collaborations" });
                    EC.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                    //List<SelectListItem> lstAgency = new List<SelectListItem>();
                    //lstAgency.Insert(0, new SelectListItem { Value = "0", Text = "Select Agency" });
                    //EC.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                }

                EC.YearList = comm.PopulateFinancialYear(true, true).ToList();
                //EC.YearList.RemoveAt(0);

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
        public ActionResult PMGSYAchievementsReport(ECBriefReportViewModel EC)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //EC.LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 3 : PMGSYSession.Current.StateCode > 0 ? 2 : 1;
                    EC.LevelCode = EC.DistrictCode > 0 ? 3 : EC.StateCode > 0 ? 2 : 1;
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        EC.State_Name = PMGSYSession.Current.StateName;
                    }

                    //EC.LevelCode = EC.RoadWise == true ? 4 : EC.BlockCode > 0 ? 3 : EC.DistrictCode > 0 ? 2 : 1;

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

                        //EC.DistrictList = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();
                        //EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
                        EC.DistrictList = comm.PopulateDistrict(EC.StateCode, true);
                        EC.DistrictList.RemoveAt(0);
                        EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

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

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();

                    EC.YearList = comm.PopulateFinancialYear(true, true).ToList();
                    //EC.YearList.RemoveAt(0);
                    EC.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
                    EC.BatchList = comm.PopulateBatch();
                    EC.BatchList.RemoveAt(0);
                    EC.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });

                    return View("PMGSYAchievementsLayout", EC);
                }
            }
            catch
            {
                return View(EC);
            }

        }
        #endregion

        #region  StateWise List of Roads Report
        /// <summary>
        /// Layout for Completed Roads Report
        /// </summary>
        /// <returns></returns>
        public ActionResult SLRWorksLayout()
        {
            StateListWiseRoadsViewModel stateListWiseRoadsReportModel = new StateListWiseRoadsViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            //FetchCookieData fetchCookie = new FetchCookieData();
            stateListWiseRoadsReportModel.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

            stateListWiseRoadsReportModel.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();

            //stateListWiseRoadsReportModel.BlockName = PMGSYSession.Current.BlockCode == 0 ? "0" : PMGSYSession.Current.BlockName.Trim();

            stateListWiseRoadsReportModel.Mast_State_Code = PMGSYSession.Current.StateCode;

            stateListWiseRoadsReportModel.Mast_District_Code = PMGSYSession.Current.DistrictCode;

            stateListWiseRoadsReportModel.DistrictCode = PMGSYSession.Current.DistrictCode;

            //stateListWiseRoadsReportModel.Mast_Block_Code = PMGSYSession.Current.BlockCode;

            //stateListWiseRoadsReportModel.LevelCode = PMGSYSession.Current.BlockCode > 0 ? 3 : PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;

            stateListWiseRoadsReportModel.StateList = commonFunctions.PopulateStates(true);

            //stateListWiseRoadsReportModel.StateCode = PMGSYSession.Current.StateCode == 0 ? -1 : PMGSYSession.Current.StateCode;

            stateListWiseRoadsReportModel.StateList.Find(x => x.Value == stateListWiseRoadsReportModel.StateCode.ToString()).Selected = true;

            stateListWiseRoadsReportModel.DistrictList = new List<SelectListItem>();
            if (stateListWiseRoadsReportModel.StateCode == 0)
            {
                stateListWiseRoadsReportModel.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                stateListWiseRoadsReportModel.DistrictList = commonFunctions.PopulateDistrict(stateListWiseRoadsReportModel.StateCode, true);
                stateListWiseRoadsReportModel.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                stateListWiseRoadsReportModel.DistrictList.Find(x => x.Value == stateListWiseRoadsReportModel.DistrictCode.ToString()).Selected = true;

            }
            stateListWiseRoadsReportModel.BlockList = new List<SelectListItem>();
            if (stateListWiseRoadsReportModel.DistrictCode == 0)
            {
                stateListWiseRoadsReportModel.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                stateListWiseRoadsReportModel.BlockList = commonFunctions.PopulateBlocks(stateListWiseRoadsReportModel.DistrictCode, true);
                //stateListWiseRoadsReportModel.BlockCode = fetchCookie.BlockCode == 0 ? 0 : fetchCookie.BlockCode;
                stateListWiseRoadsReportModel.BlockList.Find(x => x.Value == "-1").Value = "0";
                stateListWiseRoadsReportModel.BlockList.Find(x => x.Value == stateListWiseRoadsReportModel.BlockCode.ToString()).Selected = true;
            }

            stateListWiseRoadsReportModel.Year = DateTime.Now.Year;
            stateListWiseRoadsReportModel.YearList = commonFunctions.PopulateFinancialYear(true).ToList();
            stateListWiseRoadsReportModel.BatchList = commonFunctions.PopulateBatch(true);
            stateListWiseRoadsReportModel.FundingAgencyList = commonFunctions.PopulateFundingAgency(true);
            stateListWiseRoadsReportModel.FundingAgencyList.Find(x => x.Value == "-1").Value = "0";
            //if (PMGSYSession.Current.Language.Contains('-'))
            //{
            //    stateListWiseRoadsReportModel.localizedValue = fetchCookie.Language.Substring(0, fetchCookie.Language.IndexOf('-'));
            //}
            //else
            //{
            //    stateListWiseRoadsReportModel.localizedValue = fetchCookie.Language;
            //}

            stateListWiseRoadsReportModel.Status = "%";
            stateListWiseRoadsReportModel.StatusList = new List<SelectListItem>();
            stateListWiseRoadsReportModel.StatusList.Insert(0, (new SelectListItem { Text = "All Status", Value = "%", Selected = true }));
            stateListWiseRoadsReportModel.StatusList.Insert(1, (new SelectListItem { Text = "In Progress", Value = "P" }));
            stateListWiseRoadsReportModel.StatusList.Insert(2, (new SelectListItem { Text = "Complete", Value = "C" }));
            return View(stateListWiseRoadsReportModel);

        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SLRWorksReport(StateListWiseRoadsViewModel stateListWiseRoadsModel)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    stateListWiseRoadsModel.LevelCode = stateListWiseRoadsModel.RoadWise == true ? 4 : stateListWiseRoadsModel.BlockCode > 0 ? 3 : stateListWiseRoadsModel.DistrictCode > 0 ? 2 : 1;
                    stateListWiseRoadsModel.Mast_State_Code = stateListWiseRoadsModel.StateCode > 0 ? stateListWiseRoadsModel.StateCode : stateListWiseRoadsModel.Mast_State_Code;
                    stateListWiseRoadsModel.Mast_District_Code = stateListWiseRoadsModel.DistrictCode > 0 ? stateListWiseRoadsModel.DistrictCode : stateListWiseRoadsModel.Mast_District_Code;
                    stateListWiseRoadsModel.Mast_Block_Code = stateListWiseRoadsModel.BlockCode > 0 ? stateListWiseRoadsModel.BlockCode : stateListWiseRoadsModel.Mast_Block_Code;


                    return View(stateListWiseRoadsModel);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                    //return View("StateListWiseRoadsLayout");
                }
            }
            catch
            {
                return View(stateListWiseRoadsModel);
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
        #endregion


        #region
        
        public ActionResult Schedule5Report()
        {
            try
            {
                return View();
            }
            catch
            {
                return null;
            }

        }
        #endregion


        #region CNCPL Roadwise Core Network Connectivity Priority List

        [HttpGet]
        public ActionResult CNCPLRoadwiseCoreNetworkLayout()
        {
            try
            {
                FAPhaseProfileReportViewModel FA = new FAPhaseProfileReportViewModel();

                if (PMGSYSession.Current.StateCode > 0)
                {
                    FA.StateCode = PMGSYSession.Current.StateCode;
                    FA.StateName = PMGSYSession.Current.StateName;
                   

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(FA.StateCode), Text = Convert.ToString(FA.StateName) });
                    FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        FA.DistrictCode = PMGSYSession.Current.DistrictCode;
                        FA.DistName = PMGSYSession.Current.DistrictName;

                        List<SelectListItem> lstDist = new List<SelectListItem>();
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(FA.DistrictCode), Text = Convert.ToString(FA.DistName) });
                        FA.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        FA.BlockList = comm.PopulateBlocks(FA.DistrictCode, true);
                        FA.BlockList.RemoveAt(0);
                        FA.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    }
                    else
                    {
                        FA.DistrictList = comm.PopulateDistrict(FA.StateCode, true);
                        FA.DistrictList.RemoveAt(0);
                        FA.DistrictList.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                        FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    }

                    

                    List<SelectListItem> lstAgency = new List<SelectListItem>();
                   
                    FA.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(FA.StateCode, PMGSYSession.Current.AdminNdCode, false);
                    FA.AgencyList.RemoveAt(0);

                    FA.CollaborationList = PopulateCollaborationsStateWise(FA.StateCode, true);
                }
                #region
                else
                {
                    FA.StateList = comm.PopulateStates(true);
                    FA.StateList.RemoveAt(0);
                    FA.StateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });


                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });
                    FA.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                }
                #endregion


                return View(FA);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult PopulateDistrictsForCNCPL()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                List<SelectListItem> lstDist = new List<SelectListItem>();
                lstDist = objCommonFunctions.PopulateDistrict(stateCode, true);
                lstDist.RemoveAt(0);
                lstDist.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });
                return Json(lstDist);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        [HttpPost]
        public ActionResult CNCPLRoadwiseCoreNetworkRport(FAPhaseProfileReportViewModel FA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FA.LevelCode = 1;

                    return View(FA);
                }
                else
                {
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        FA.StateCode = PMGSYSession.Current.StateCode;
                        FA.StateName = PMGSYSession.Current.StateName;
                        //FA.State_Name = PMGSYSession.Current.StateName;

                        List<SelectListItem> lstState = new List<SelectListItem>();
                        lstState.Insert(0, new SelectListItem { Value = Convert.ToString(FA.StateCode), Text = Convert.ToString(FA.StateName) });
                        FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                        FA.DistrictList = comm.PopulateDistrict(FA.StateCode, true);
                        FA.DistrictList.RemoveAt(0);
                        FA.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                        List<SelectListItem> lstAgency = new List<SelectListItem>();
                        FA.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(FA.StateCode, PMGSYSession.Current.AdminNdCode, false);
                        FA.AgencyList.RemoveAt(0);

                        FA.CollaborationList = PopulateCollaborationsStateWise(FA.StateCode, true);
                    }
                    #region
                    else
                    {
                        List<SelectListItem> lstState = new List<SelectListItem>();
                        lstState.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });
                        FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                        List<SelectListItem> lstDistricts = new List<SelectListItem>();
                        lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                        FA.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                        List<SelectListItem> lstCollab = new List<SelectListItem>();
                        lstCollab.Insert(0, new SelectListItem { Value = "0", Text = "Select Collaboration" });
                        FA.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                        List<SelectListItem> lstAgency = new List<SelectListItem>();
                        lstAgency.Insert(0, new SelectListItem { Value = "-1", Text = "Select Agency" });
                        FA.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
                    }
                    #endregion
                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    return View("FAPhaseProfileLayout", FA);
                }
            }
            catch
            {
                return View(FA);
            }
        }

        #endregion 


        #region 14 July Category

        [HttpGet]
        public ActionResult CategoryLayout()
        {
            Category objProp = new Category();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            PMGSY.DAL.ProposalReports.ProposalReportsDAL objDAL = new PMGSY.DAL.ProposalReports.ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, false).ToList();
                List<SelectListItem> ddFundingAgency = objDAL.GetFundingAgencyList();
                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                //SelectListItem all = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "All",
                //    Value = "0"
                //};
                //  ddYear.RemoveAt(0);
                // ddYear.Insert(0, all);
                // ddState.Insert(0, all);

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                // List<SelectListItem> ddProposalStatus = new List<SelectListItem>();

                List<SelectListItem> ddBatch = objCommonFunctions.PopulateMonths(true);
                //ddBatch.RemoveAt(0);
                // ddBatch.Insert(0, all);
                objProp.BatchList = new SelectList(ddBatch, "Value", "Text").ToList();
                objProp.BatchList.Find(x => x.Value == "0").Value = "-1";
                objProp.YearList = new SelectList(ddYear, "Value", "Text").ToList();
                objProp.YearList.Find(x => x.Value == "0").Value = "-1";
                //objProp.StatusList = new SelectList(ddProposalStatus, "Value", "Text").ToList();
                objProp.CollabList = new SelectList(ddFundingAgency, "Value", "Text").ToList();

                //ViewData["YEAR"] = ddYear;
                //ViewData["BATCH"] = ddBatch;
                //ViewData["AGENCY"] = ddFundingAgency;
                //ViewData["STATUS"] = ddProposalStatus;

                //ViewData["STATE"] = ddState;

                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;

                objProp.Level = PMGSYSession.Current.RoleCode == 25 ? 1 : PMGSYSession.Current.RoleCode == 2 ? 2 : 3;

                if (PMGSYSession.Current.StateCode == 0)
                {
                    //objProp.Level = 1;
                    objProp.StateName = "All States";
                }
                else
                {
                    //objProp.Level = 2;
                    objProp.StateName = PMGSYSession.Current.StateName;
                }

                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    //objProp.Level = 3;
                    objProp.DistrictName = "All Districts";
                }
                else
                {
                    objProp.DistrictName = PMGSYSession.Current.DistrictName;
                }

                //if (PMGSYSession.Current.blockCode == 0)
                //{
                objProp.BlockName = "All Blocks";
                //}
                //else
                //{
                //    objProp.DistrictName = PMGSYSession.Current.DistrictName;
                //}

                return View(objProp);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult CategoryReport(Category objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    //objProp.State_Code = objProp.StateCode > 0 ? objProp.StateCode : objProp.Mast_State_Code;
                    objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    return View(objProp);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return View("CategoryLayout", objProp);
                }
            }
            catch
            {
                return View(objProp);
            }

        }
        #endregion 


        
        #region Gepnic 3 Jan 2019

        [HttpGet]
        public ActionResult GepnicPushedLayout()
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

                    //List<SelectListItem> lstAgency = new List<SelectListItem>();
                    //EC.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(EC.StateCode, PMGSYSession.Current.AdminNdCode, false);
                    //EC.AgencyList.RemoveAt(0);



                    EC.CollaborationList = PopulateCollaborationsStateWise(EC.StateCode, true);
                }
                else
                {
                    EC.StateList = comm.PopulateStates(true);
                    EC.StateList.RemoveAt(0);
                    EC.StateList.Insert(0, new SelectListItem { Value = "0", Text = "All States" });

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
                    EC.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                    List<SelectListItem> lstCollab = new List<SelectListItem>();
                    lstCollab.Insert(0, new SelectListItem { Value = "0", Text = "All Collaborations" });
                    EC.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                    //List<SelectListItem> lstAgency = new List<SelectListItem>();
                    //lstAgency.Insert(0, new SelectListItem { Value = "0", Text = "Select Agency" });
                    //EC.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                }

                EC.YearList = comm.PopulateFinancialYear(true, true).ToList();
                //EC.YearList.RemoveAt(0);

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
        public ActionResult GepnicPushedReport(ECBriefReportViewModel EC)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //EC.LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 3 : PMGSYSession.Current.StateCode > 0 ? 2 : 1;
                    EC.LevelCode = EC.DistrictCode > 0 ? 3 : EC.StateCode > 0 ? 2 : 1;
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        EC.State_Name = PMGSYSession.Current.StateName;
                    }

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

                        //EC.DistrictList = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();
                        //EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
                        EC.DistrictList = comm.PopulateDistrict(EC.StateCode, true);
                        EC.DistrictList.RemoveAt(0);
                        EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

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

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();

                    EC.YearList = comm.PopulateFinancialYear(true, true).ToList();
                    //EC.YearList.RemoveAt(0);
                    EC.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
                    EC.BatchList = comm.PopulateBatch();
                    EC.BatchList.RemoveAt(0);
                    EC.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });

                    return View("GepnicPushedLayout", EC);
                }
            }
            catch
            {
                return View(EC);
            }

        }

        #endregion 


        #region Road Progress 6 Aug 2019

        [HttpGet]
        public ActionResult RoadProgressLayout()
        {
            try
            {
                RoadProgress FA = new RoadProgress();
                CommonFunctions commonFunctions = new CommonFunctions();
                if (PMGSYSession.Current.StateCode > 0)
                {
                    FA.StateCode = PMGSYSession.Current.StateCode;
                    FA.StateName = PMGSYSession.Current.StateName;
                    //FA.State_Name = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(FA.StateCode), Text = Convert.ToString(FA.StateName) });
                    FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        FA.DistrictCode = PMGSYSession.Current.DistrictCode;
                        FA.DistName = PMGSYSession.Current.DistrictName;

                        List<SelectListItem> lstDist = new List<SelectListItem>();
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(FA.DistrictCode), Text = Convert.ToString(FA.DistName) });
                        FA.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        FA.BlockList = comm.PopulateBlocks(FA.DistrictCode, true);
                        FA.BlockList.RemoveAt(0);
                        FA.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    }
                    else
                    {
                        FA.DistrictList = comm.PopulateDistrict(FA.StateCode, true);
                        FA.DistrictList.RemoveAt(0);
                        FA.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                        FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    }



                    //FA.AgencyCode = PMGSYSession.Current.AdminNdCode;
                    //FA.AgencyName = PMGSYSession.Current.DepartmentName.Trim();

                  //  List<SelectListItem> lstAgency = new List<SelectListItem>();
                    //lstAgency.Insert(0, new SelectListItem { Value = Convert.ToString(FA.AgencyCode), Text = Convert.ToString(FA.AgencyName) });
                    //FA.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
                 //   FA.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(FA.StateCode, PMGSYSession.Current.AdminNdCode, true);
                 //   FA.AgencyList.RemoveAt(0);

                    FA.CollaborationList = PopulateCollaborationsStateWise(FA.StateCode, true);
                }
                #region
                else
                {
                    FA.StateList = comm.PopulateStates(true);
                    FA.StateList.RemoveAt(0);
                    FA.StateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });

                    //List<SelectListItem> lstState = new List<SelectListItem>();
                    //lstState.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });
                    //FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });
                    FA.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                    List<SelectListItem> lstCollab = new List<SelectListItem>();
                    lstCollab.Insert(0, new SelectListItem { Value = "-1", Text = "Select Collaboration" });
                    FA.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                    //List<SelectListItem> lstAgency = new List<SelectListItem>();
                    //lstAgency.Insert(0, new SelectListItem { Value = "-1", Text = "Select Agency" });
                    //FA.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                }

                FA.YearList = commonFunctions.PopulateYears(System.DateTime.Now.Year, false);
                FA.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

                FA.YearList.RemoveAt(0);
                FA.MonthList.RemoveAt(0);

                FA.Month = System.DateTime.Now.Month;
                FA.Year = System.DateTime.Now.Year;
                FA.lstscheme = commonFunctions.PopulateScheme();


                List<SelectListItem> lstAgency = new List<SelectListItem>();
                FA.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(FA.StateCode, PMGSYSession.Current.AdminNdCode, true);
                #endregion


                return View(FA);
            }
            catch
            {
                return null;
            }
        }


        [HttpPost]
        public ActionResult RoadProgressReport(RoadProgress FA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FA.LevelCode = 1;

                    return View(FA);
                }
                else
                {
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        FA.StateCode = PMGSYSession.Current.StateCode;
                        FA.StateName = PMGSYSession.Current.StateName;
                        //FA.State_Name = PMGSYSession.Current.StateName;

                        List<SelectListItem> lstState = new List<SelectListItem>();
                        lstState.Insert(0, new SelectListItem { Value = Convert.ToString(FA.StateCode), Text = Convert.ToString(FA.StateName) });
                        FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                        FA.DistrictList = comm.PopulateDistrict(FA.StateCode, true);
                        FA.DistrictList.RemoveAt(0);
                        FA.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                        List<SelectListItem> lstAgency = new List<SelectListItem>();
                        FA.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(FA.StateCode, PMGSYSession.Current.AdminNdCode, true);
                        FA.AgencyList.RemoveAt(0);

                        FA.CollaborationList = PopulateCollaborationsStateWise(FA.StateCode, true);
                    }
                    #region
                    else
                    {
                        List<SelectListItem> lstState = new List<SelectListItem>();
                        lstState.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });
                        FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                        List<SelectListItem> lstDistricts = new List<SelectListItem>();
                        lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                        FA.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                        List<SelectListItem> lstCollab = new List<SelectListItem>();
                        lstCollab.Insert(0, new SelectListItem { Value = "0", Text = "Select Collaboration" });
                        FA.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                        List<SelectListItem> lstAgency = new List<SelectListItem>();
                        lstAgency.Insert(0, new SelectListItem { Value = "0", Text = "All Agency" });
                        FA.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
                    }
                    #endregion
                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    return View("FAPhaseProfileLayout", FA);
                }
            }
            catch
            {
                return View(FA);
            }
        }


        [HttpPost]
        public JsonResult PopulateAgenciesForRoadProgress()
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
        #endregion

        #region ROAD LIST UNDER DLP SACHIN 28 MAY 2020

        [HttpGet]
        public ActionResult RoadDLPLayout()
        {
            try
            {
                RoadProgress FA = new RoadProgress();
                CommonFunctions commonFunctions = new CommonFunctions();
                if (PMGSYSession.Current.StateCode > 0)
                {
                    FA.StateCode = PMGSYSession.Current.StateCode;
                    FA.StateName = PMGSYSession.Current.StateName;
                    //FA.State_Name = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(FA.StateCode), Text = Convert.ToString(FA.StateName) });
                    FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        FA.DistrictCode = PMGSYSession.Current.DistrictCode;
                        FA.DistName = PMGSYSession.Current.DistrictName;

                        List<SelectListItem> lstDist = new List<SelectListItem>();
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(FA.DistrictCode), Text = Convert.ToString(FA.DistName) });
                        FA.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        FA.BlockList = comm.PopulateBlocks(FA.DistrictCode, true);
                        FA.BlockList.RemoveAt(0);
                        FA.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    }
                    else
                    {
                        FA.DistrictList = comm.PopulateDistrict(FA.StateCode, true);
                        FA.DistrictList.RemoveAt(0);
                        FA.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                        FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    }



                    //FA.AgencyCode = PMGSYSession.Current.AdminNdCode;
                    //FA.AgencyName = PMGSYSession.Current.DepartmentName.Trim();

                    //  List<SelectListItem> lstAgency = new List<SelectListItem>();
                    //lstAgency.Insert(0, new SelectListItem { Value = Convert.ToString(FA.AgencyCode), Text = Convert.ToString(FA.AgencyName) });
                    //FA.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
                    //   FA.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(FA.StateCode, PMGSYSession.Current.AdminNdCode, true);
                    //   FA.AgencyList.RemoveAt(0);

                    FA.CollaborationList = PopulateCollaborationsStateWise(FA.StateCode, true);
                }
                #region
                else
                {
                    FA.StateList = comm.PopulateStates(true);
                    FA.StateList.RemoveAt(0);
                    FA.StateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });

                    //List<SelectListItem> lstState = new List<SelectListItem>();
                    //lstState.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });
                    //FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });
                    FA.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                    List<SelectListItem> lstCollab = new List<SelectListItem>();
                    lstCollab.Insert(0, new SelectListItem { Value = "-1", Text = "Select Collaboration" });
                    FA.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                    //List<SelectListItem> lstAgency = new List<SelectListItem>();
                    //lstAgency.Insert(0, new SelectListItem { Value = "-1", Text = "Select Agency" });
                    //FA.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                }

                FA.YearList = commonFunctions.PopulateYears(System.DateTime.Now.Year, false);
                FA.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

                FA.YearList.RemoveAt(0);
                FA.MonthList.RemoveAt(0);

                FA.Month = System.DateTime.Now.Month;
                FA.Year = System.DateTime.Now.Year;
                FA.lstscheme = commonFunctions.PopulateScheme();


                List<SelectListItem> lstAgency = new List<SelectListItem>();
                FA.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(FA.StateCode, PMGSYSession.Current.AdminNdCode, true);
                #endregion


                return View(FA);
            }
            catch
            {
                return null;
            }
        }


        [HttpPost]
        public ActionResult RoadDLPLayoutReport(RoadProgress FA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FA.LevelCode = 1;

                    return View(FA);
                }
                else
                {
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        FA.StateCode = PMGSYSession.Current.StateCode;
                        FA.StateName = PMGSYSession.Current.StateName;
                        //FA.State_Name = PMGSYSession.Current.StateName;

                        List<SelectListItem> lstState = new List<SelectListItem>();
                        lstState.Insert(0, new SelectListItem { Value = Convert.ToString(FA.StateCode), Text = Convert.ToString(FA.StateName) });
                        FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                        FA.DistrictList = comm.PopulateDistrict(FA.StateCode, true);
                        FA.DistrictList.RemoveAt(0);
                        FA.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                        List<SelectListItem> lstAgency = new List<SelectListItem>();
                        FA.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(FA.StateCode, PMGSYSession.Current.AdminNdCode, true);
                        FA.AgencyList.RemoveAt(0);

                        FA.CollaborationList = PopulateCollaborationsStateWise(FA.StateCode, true);
                    }
                    #region
                    else
                    {
                        List<SelectListItem> lstState = new List<SelectListItem>();
                        lstState.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });
                        FA.StateList = new SelectList(lstState, "Value", "Text").ToList();

                        List<SelectListItem> lstDistricts = new List<SelectListItem>();
                        lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                        FA.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                        List<SelectListItem> lstCollab = new List<SelectListItem>();
                        lstCollab.Insert(0, new SelectListItem { Value = "0", Text = "Select Collaboration" });
                        FA.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                        List<SelectListItem> lstAgency = new List<SelectListItem>();
                        lstAgency.Insert(0, new SelectListItem { Value = "0", Text = "All Agency" });
                        FA.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();
                    }
                    #endregion
                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    FA.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    return View("FAPhaseProfileLayout", FA);
                }
            }
            catch
            {
                return View(FA);
            }
        }




        #endregion

    }


}
