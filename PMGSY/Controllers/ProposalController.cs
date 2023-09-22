#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ProposalController.cs        
        * Description   :   Action methods for Creating , Editing, Deleting Road Propsoals and All the Data Related to Proposal
                            Including Other Parameters Like Habitation Details, CBR Details , Traffic Intensity and File Upload
        * Author        :   Shivkumar Deshmukh  
        * Modified By   :   Shyam Yadav
        * Creation Date :   04/April/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models;
using PMGSY.Models.Proposal;
using System.Data.Entity.Validation;
using PMGSY.BAL.Proposal;
using PMGSY.Common;
using System.Text;
using PMGSY.Extensions;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.IO;
using System.Configuration;
using System.Net;
using PMGSY.DAL.Proposal;
using PMGSY.Models.Common;
using PMGSY.BAL.Execution;

using PMGSY.DAL.Proposal;
using PMGSY.BAL.Master;
using PMGSY.DAL.Master;
using PMGSY.Models.Master;
using System.Transactions;
using System.Data.Entity.Infrastructure;
//using System.Data.Entity.Core.SqlClient;
using System.Data.SqlClient;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class ProposalController : Controller
    {

        public ProposalController()
        {
            PMGSYSession.Current.ModuleName = "Proposal";
        }
        #region Variable Declaration
        private PMGSYEntities db = new PMGSYEntities();
        private ProposalBAL objProposalBAL = new ProposalBAL();
        Dictionary<string, string> decryptedParameters = null;
        string message = String.Empty;
        int outParam = 0;
        public MasterDAL objDAL = new MasterDAL();
        public IMasterBAL objBAL = new MasterBAL();

        #endregion

        #region Road Proposal Data Entry

        /// <summary>
        /// Get Method of Proposal Home Page
        /// Contains Filters for Listing Proposals
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListProposal()
        {
            ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            List<SelectListItem> lstTypes = new List<SelectListItem>();
            lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
            lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
            proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
            proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
            // DPIU
            if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
            {
                proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                //proposalViewModel.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, proposalViewModel.IMS_YEAR, true);
                proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                proposalViewModel.CONNECTIVITYLIST = lstTypes;
                proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                proposalViewModel.BATCHS.RemoveAt(0);
                proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

            }
            else if (PMGSYSession.Current.RoleCode == 15)          //PTA 
            {
                proposalViewModel.STATES = objCommonFuntion.PopulateStatesOfPTA();
                proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0);
                proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                proposalViewModel.BATCHS.RemoveAt(0);
                proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
            }
            else if (PMGSYSession.Current.RoleCode == 3)          //STA 
            {
                proposalViewModel.STATES = objCommonFuntion.PopulateStatesOfPTA();
                proposalViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrictsOfTA(PMGSYSession.Current.StateCode, true);
                proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                proposalViewModel.BATCHS.RemoveAt(0);
                proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
            }
            else if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 37 || PMGSYSession.Current.RoleCode == 55)          //SRRDA or SRRDAOA or SRRDARCPLWE
            {
                proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                proposalViewModel.CONNECTIVITYLIST = lstTypes;
                proposalViewModel.BATCHS.RemoveAt(0);
                proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                proposalViewModel.BLOCKS.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 65)  // Mord and Mord View
            {

                List<SelectListItem> lstRoadTypes = new List<SelectListItem>();
                lstRoadTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });

                proposalViewModel.STATES = objCommonFuntion.PopulateStates();
                proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0, true);
                proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                proposalViewModel.BATCHS.RemoveAt(0);
                proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                proposalViewModel.AGENCIES = lstRoadTypes;
                proposalViewModel.CONNECTIVITYLIST = lstTypes;
            }

            proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
            proposalViewModel.Years = PopulateYear(0, true, true);
            proposalViewModel.IMS_YEAR = DateTime.Now.Year;
            //proposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("", true);
            proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;

            return View("ListProposal", proposalViewModel);
        }

        /// <summary>
        ///  Screen : Listing Page of the Proposal
        /// Get the Proposals
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetProposals(FormCollection formCollection)
        {
            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 29-Apr-2014 end
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int MAST_BLOCK_ID = Convert.ToInt32(Request.Params["MAST_BLOCK_ID"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
            string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
            int totalRecords;
            ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
            var jsonData = new
            {
                rows = objProposalBAL.GetProposalsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, PMGSYSession.Current.AdminNdCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, formCollection["filters"], out colTotal),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
                TotalColumn = colTotal
            };
            return Json(jsonData);
        }



        /// <summary>
        /// Get proposals for SRRDA
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetProposalsForSRRDA(FormCollection formCollection)
        {
            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 29-Apr-2014 end
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
            string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
            int MAST_BLOCK_CODE = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
            int totalRecords;
            ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
            var jsonData = new
            {
                rows = objProposalBAL.GetProposalsForSRRDABAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, formCollection["filters"], out colTotal),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
                TotalColumn = colTotal
            };
            return Json(jsonData);
        }



        /// <summary>
        /// Get method for Details of Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult Details(string id)
        {

            String[] urlSplitParams = id.Split('$');
            Int32 IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);

            IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);


            // Lock Status is depend on Is the proposal mord sanctioned or not?
            // In Case of Mord Unlocked Status IMS_LOCK_STATUS = "M"
            // Else IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS
            // Here if it is splitted from parameter - id, that means it is passed from GetProposalsDAL() function
            // Else get it from logic in db function -- UDF_IMS_UNLOCK_STATUS
            string IMS_LOCK_STATUS = string.Empty;

            if (urlSplitParams.Length > 1)
            {
                IMS_LOCK_STATUS = urlSplitParams[1];
            }
            else
            {
                if (ims_sanctioned_projects.IMS_SANCTIONED == "Y")
                {
                    if (db.UDF_IMS_UNLOCK_STATUS(ims_sanctioned_projects.MAST_STATE_CODE, ims_sanctioned_projects.MAST_DISTRICT_CODE, ims_sanctioned_projects.MAST_BLOCK_CODE, 0, 0, ims_sanctioned_projects.IMS_PR_ROAD_CODE, 0, 0, "PR", ims_sanctioned_projects.MAST_PMGSY_SCHEME, (short)PMGSYSession.Current.RoleCode).Select(c => c.UNLOCK_COUNT).FirstOrDefault() > 0)
                    {
                        IMS_LOCK_STATUS = "M";
                    }
                    else
                    {
                        IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS;
                    }
                }
                else
                {
                    IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS;
                }
            }



            PMGSY.Models.Proposal.ProposalViewModel objProposal = new ProposalViewModel();
            if (ims_sanctioned_projects == null)
            {
                return HttpNotFound();
            }

            objProposal.StateName = db.MASTER_STATE.Where(a => a.MAST_STATE_CODE == ims_sanctioned_projects.MAST_STATE_CODE).Select(a => a.MAST_STATE_NAME).First();
            objProposal.DistrictName = db.MASTER_DISTRICT.Where(a => a.MAST_DISTRICT_CODE == ims_sanctioned_projects.MAST_DISTRICT_CODE).Select(a => a.MAST_DISTRICT_NAME).First();
            ViewBag.IsTechnologyExist = db.IMS_PROPOSAL_TECH.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE) ? "Y" : "N";
            objProposal.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
            objProposal.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            objProposal.MAST_BLOCK_NAME = ims_sanctioned_projects.MASTER_BLOCK.MAST_BLOCK_NAME;
            objProposal.MAST_STREAM_NAME = ims_sanctioned_projects.MASTER_STREAMS.MAST_STREAM_NAME;
            objProposal.IMS_STAGE_PHASE = ims_sanctioned_projects.IMS_STAGE_PHASE;
            objProposal.IMS_STAGED_YEAR = ims_sanctioned_projects.IMS_STAGED_YEAR;
            objProposal.IMS_STAGED_PACKAGE_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;

            if (ims_sanctioned_projects.PLAN_CN_ROAD_CODE != null)
            {
                //objProposal.PLAN_RD_NAME = ims_sanctioned_projects.PLAN_ROAD.PLAN_CN_ROAD_NUMBER + "-" + ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME;
                ///Changed by SAMMED A. PATIL for RCPLWE
                objProposal.PLAN_RD_NAME = ims_sanctioned_projects.PLAN_ROAD.PLAN_CN_ROAD_NUMBER == "O" ? ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME : (ims_sanctioned_projects.PLAN_ROAD.PLAN_CN_ROAD_NUMBER + "-" + ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME);
            }
            else
            {
                objProposal.PLAN_RD_NAME = "--";
            }

            objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;
            if (ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "U")
            {
                objProposal.MAST_EXISTING_SURFACE_NAME = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE == null ? "" : ims_sanctioned_projects.MASTER_SURFACE.MAST_SURFACE_NAME;
                objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;
                if (ims_sanctioned_projects.IMS_ISBENEFITTED_HABS == "N")
                {
                    objProposal.HABS_REASON_TEXT = db.MASTER_REASON.Where(a => a.MAST_REASON_CODE == ims_sanctioned_projects.IMS_HABS_REASON).Select(a => a.MAST_REASON_NAME).FirstOrDefault();
                }
            }
            objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
            objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
            objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;
            objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
            objProposal.MAST_FUNDING_AGENCY_NAME = ims_sanctioned_projects.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_NAME;
            objProposal.MAST_STREAM_NAME = ims_sanctioned_projects.MASTER_STREAMS.MAST_STREAM_NAME;

            objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
            objProposal.IMS_PARTIAL_LEN = ims_sanctioned_projects.IMS_PARTIAL_LEN.ToUpper() == "F" ? "Full Length" : "Partial Length";
            objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
            objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;
            objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
            objProposal.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;
            objProposal.IMS_NO_OF_CDWORKS = ims_sanctioned_projects.IMS_NO_OF_CDWORKS;

            // All Costs
            objProposal.IMS_SANCTIONED_PAV_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;
            objProposal.IMS_SANCTIONED_CD_AMT = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;
            objProposal.IMS_SANCTIONED_PW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;
            objProposal.IMS_SANCTIONED_OW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;
            objProposal.IMS_SANCTIONED_RS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT;

            //objProposal.IMS_TOTAL_COST = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT + ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT +
            //                             ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT + ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT +
            //                             ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT +
            //                             Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT) + "";

            objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
            //objProposal.IMS_TOTAL_COST = (ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT + ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT +
            //                              ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT + ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT +
            //                              Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT)).ToString();
            ///Changes made by SAMMED A. PATIL on 14AUG2017
            objProposal.IMS_TOTAL_COST = Convert.ToString((PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ?
                                                      ((ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT))
                                                    : (PMGSYSession.Current.PMGSYScheme == 5)
                                                        ? ((ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) + (ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS == null ? 0 : ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS))
                                                        : ((ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT)));
            objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;
            objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
            objProposal.IMS_FURNITURE_COST = ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT;
            objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT;
            objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT;
            objProposal.IMS_SANCTIONED_FC_AMT = ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT;
            objProposal.IMS_SANCTIONED_RENEWAL_AMT = ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT ?? 0;

            if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 1)
            {
                objProposal.IMS_SANCTIONED_AMOUNT = (Convert.ToDecimal(objProposal.IMS_TOTAL_COST) * 90) / 100;
            }
            else if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 2)
            {
                objProposal.IMS_SANCTIONED_AMOUNT = (Convert.ToDecimal(objProposal.IMS_TOTAL_COST) * 75) / 100;
            }

            objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
            objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
            objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
            objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
            objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;


            objProposal.TotalMaintenanceCost = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2 +
                                               ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4 +
                                               ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5 +  //In case of PMGSY Scheme-II include IMS_SANCTIONED_RENEWAL_AMT
                                               (PMGSYSession.Current.PMGSYScheme == 2
                                                ? Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT)
                                                : 0.0M);

            objProposal.IMS_ZP_RESO_OBTAINED = ims_sanctioned_projects.IMS_ZP_RESO_OBTAINED;
            objProposal.MAST_MP_CONST_NAME = ims_sanctioned_projects.MASTER_MP_CONSTITUENCY == null ? "NA" : ims_sanctioned_projects.MASTER_MP_CONSTITUENCY.MAST_MP_CONST_NAME;
            objProposal.MAST_MLA_CONST_NAME = ims_sanctioned_projects.MASTER_MLA_CONSTITUENCY == null ? "NA" : ims_sanctioned_projects.MASTER_MLA_CONSTITUENCY.MAST_MLA_CONST_NAME;
            objProposal.IMS_CARRIAGED_WIDTH = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH;
            objProposal.Display_Carriaged_Width = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH != null ? db.MASTER_CARRIAGE.Where(a => a.MAST_CARRIAGE_CODE == ims_sanctioned_projects.IMS_CARRIAGED_WIDTH).Select(a => a.MAST_CARRIAGE_WIDTH).First().ToString() : "NA";

            if (ims_sanctioned_projects.MASTER_TRAFFIC_TYPE != null)
            {
                objProposal.IMS_TRAFFIC_CATAGORY_NAME = ims_sanctioned_projects.MASTER_TRAFFIC_TYPE.MAST_TRAFFIC_NAME;
            }
            else
            {
                objProposal.IMS_TRAFFIC_CATAGORY_NAME = "NA";
            }

            objProposal.IMS_PROPOSED_SURFACE = ims_sanctioned_projects.IMS_PROPOSED_SURFACE.ToUpper() == "S" ? "Sealed" : "UnSealed";
            objProposal.IMS_CC_LENGTH = ims_sanctioned_projects.IMS_CC_LENGTH;
            objProposal.IMS_BT_LENGTH = ims_sanctioned_projects.IMS_BT_LENGTH;
            objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;

            objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS;
            objProposal.IMS_REASON = ims_sanctioned_projects.IMS_REASON;

            if (ims_sanctioned_projects.IMS_REASON != null)
            {
                objProposal.Reason = db.MASTER_REASON.Where(a => a.MAST_REASON_CODE == ims_sanctioned_projects.IMS_REASON).Select(a => a.MAST_REASON_NAME).First();
            }

            objProposal.IMS_IS_STAGED = ims_sanctioned_projects.IMS_IS_STAGED;
            objProposal.IMS_ISCOMPLETED = ims_sanctioned_projects.IMS_ISCOMPLETED;

            //STA Details
            objProposal.STA_SANCTIONED = ims_sanctioned_projects.STA_SANCTIONED;
            objProposal.STA_SANCTIONED_BY = (ims_sanctioned_projects.STA_SANCTIONED_BY != null && ims_sanctioned_projects.STA_SANCTIONED_BY != "") ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == db.UM_User_Master.Where(a => a.UserName == ims_sanctioned_projects.STA_SANCTIONED_BY).Select(a => a.UserID).FirstOrDefault()).Any() ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == db.UM_User_Master.Where(a => a.UserName == ims_sanctioned_projects.STA_SANCTIONED_BY).Select(a => a.UserID).FirstOrDefault()).Select(b => b.ADMIN_TA_NAME).First() : (ims_sanctioned_projects.STA_SANCTIONED_BY == null ? "NA" : ims_sanctioned_projects.STA_SANCTIONED_BY.ToString()) : (ims_sanctioned_projects.STA_SANCTIONED_BY == null ? "NA" : ims_sanctioned_projects.STA_SANCTIONED_BY.ToString()); //change done by Vikram on 26/05/2014 as if no data found in Admin Technical Agency then show the STA Name as it is from IMS_SANCTIONED_PROJECTS

            if (ims_sanctioned_projects.STA_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.STA_SANCTIONED_DATE).Year != 0)
            {
                DateTime dateTime = new DateTime();
                dateTime = Convert.ToDateTime(ims_sanctioned_projects.STA_SANCTIONED_DATE);
                objProposal.STA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
            }
            objProposal.MS_STA_REMARKS = ims_sanctioned_projects.IMS_STA_REMARKS;


            //PTA Details
            objProposal.PTA_SANCTIONED = ims_sanctioned_projects.PTA_SANCTIONED;
            objProposal.PTA_SANCTIONED_BY = ims_sanctioned_projects.PTA_SANCTIONED_BY == null ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_CODE).FirstOrDefault() : ims_sanctioned_projects.PTA_SANCTIONED_BY;
            objProposal.NAME_OF_PTA = ims_sanctioned_projects.PTA_SANCTIONED_BY == null
                                                    ? db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_NAME).FirstOrDefault()
                                                    : db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_TA_CODE == ims_sanctioned_projects.PTA_SANCTIONED_BY).Select(a => a.ADMIN_TA_NAME).FirstOrDefault();

            if (ims_sanctioned_projects.PTA_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.PTA_SANCTIONED_DATE).Year != 0)
            {
                DateTime dateTime = new DateTime();
                dateTime = Convert.ToDateTime(ims_sanctioned_projects.PTA_SANCTIONED_DATE);
                objProposal.PTA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
            }
            objProposal.MS_PTA_REMARKS = ims_sanctioned_projects.IMS_PTA_REMARKS;


            objProposal.IMS_SANCTIONED = ims_sanctioned_projects.IMS_SANCTIONED;
            objProposal.IMS_SANCTIONED_BY = ims_sanctioned_projects.IMS_SANCTIONED_BY;
            if (ims_sanctioned_projects.IMS_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.IMS_SANCTIONED_DATE).Year != 0)
            {
                DateTime dateTime = new DateTime();
                dateTime = Convert.ToDateTime(ims_sanctioned_projects.IMS_SANCTIONED_DATE);
                objProposal.IMS_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
            }
            objProposal.IMS_PROG_REMARKS = ims_sanctioned_projects.IMS_PROG_REMARKS;


            //objProposal.IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS;

            //In Case of Mord Unlocked Status
            objProposal.IMS_LOCK_STATUS = IMS_LOCK_STATUS;
            objProposal.IMS_DPR_STATUS = ims_sanctioned_projects.IMS_DPR_STATUS; //new change done by Vikram to hide the finalize button if the Proposal is DPR.
            objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
            objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0;
            objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_MORD_SHARE_2015.Value : 0;
            objProposal.IMS_TOTAL_COST_2015 = objProposal.IMS_STATE_SHARE_2015 + objProposal.IMS_MORD_SHARE_2015;
            //@PMGSY = 1  THEN (ISNULL(sp.IMS_STATE_SHARE,0) + ISNULL(sp.IMS_STATE_SHARE_2015,0) + ISNULL(sp.IMS_SANCTIONED_BS_AMT,0)) WHEN @PMGSY = 2 THEN (ISNULL(IMS_SANCTIONED_RS_AMT,0)+ISNULL(IMS_SANCTIONED_HS_AMT,0)+ ISNULL(sp.IMS_STATE_SHARE_2015,0)+ ISNULL(sp.IMS_SANCTIONED_BS_AMT,0)) ELSE '-' END AS TOTAL_STATE_SHARE,
            objProposal.IMS_TOTAL_STATE_SHARE_2015 = PMGSYSession.Current.PMGSYScheme == 1 ? (decimal)(ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT + (ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0))
                                                                                           : (decimal)(/*ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT +*/ Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE_2015) + ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT);
            //objProposal.IMS_SHARE_PERCENT_2015 = (byte)(db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault() == 0 ? 3 : db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault());
            //objProposal.IMS_SHARE_PERCENT_2015 = 4;

            objProposal.ImsPuccaSideDrains = ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS.HasValue ? ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS.Value : 0;

            return View(objProposal);
        }


        /// <summary>
        /// Get For Create Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult Create(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            ProposalDAL objProposalDAL = new ProposalDAL();
            PMGSY.Models.Proposal.ProposalViewModel proposalViewModel = new ProposalViewModel();
            ViewBag.operation = "C";

            #region Default values

            proposalViewModel.StateName = PMGSYSession.Current.StateName;
            proposalViewModel.DistrictName = PMGSYSession.Current.DistrictName;
            proposalViewModel.stateType = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_TYPE).FirstOrDefault();
            //following property added by Vikram for providing the staged details to the districts which are IAP_DISTRICT
            proposalViewModel.DistrictType = (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_IAP_DISTRICT).FirstOrDefault();
            proposalViewModel.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_SHORT_CODE).FirstOrDefault() +
                                               (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_DISTRICT_ID).FirstOrDefault().ToString("D2");


            proposalViewModel.IMS_UPGRADE_CONNECT = PMGSYSession.Current.PMGSYScheme == 1 ? "N" : "U";  // for Scheme 2 only upgradation is allowed

            proposalViewModel.IMS_EXISTING_PACKAGE = "N";

            proposalViewModel.IMS_IS_STAGED = "C";
            proposalViewModel.IMS_ISBENEFITTED_HABS = "Y";
            proposalViewModel.IMS_ISBENEFITTED_HABS = "Y";
            proposalViewModel.IMS_STATE_SHARE = 0;
            proposalViewModel.IMS_PROPOSED_SURFACE = "S";

            proposalViewModel.IMS_YEAR = DateTime.Now.Year;
            // Amounts 
            proposalViewModel.IMS_SANCTIONED_PAV_AMT = 0;
            proposalViewModel.IMS_SANCTIONED_CD_AMT = 0;
            proposalViewModel.IMS_SANCTIONED_PW_AMT = 0;
            proposalViewModel.IMS_SANCTIONED_OW_AMT = 0;

            proposalViewModel.IMS_SANCTIONED_MAN_AMT1 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT2 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT3 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT4 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT5 = 0;

            //For PMGSY Scheme-2
            if (PMGSYSession.Current.PMGSYScheme == 2)
            {
                proposalViewModel.IMS_IS_HIGHER_SPECIFICATION = "N";
                proposalViewModel.IMS_SANCTIONED_AMOUNT = 0;
                proposalViewModel.IMS_SHARE_PERCENT = 2;
                proposalViewModel.IMS_HIGHER_SPECIFICATION_COST = 0;
                proposalViewModel.IMS_FURNITURE_COST = 0;
                proposalViewModel.IMS_RENEWAL_COST = 0;
                proposalViewModel.IMS_SANCTIONED_HS_AMT = 0;
                proposalViewModel.IMS_SANCTIONED_FC_AMT = 0;
                proposalViewModel.IMS_SANCTIONED_RENEWAL_AMT = 0;
            }
            #endregion

            if (id != "" && id != null)
            {
                string[] defaultValues = id.Split('$');
                if (defaultValues[0] != "" && defaultValues[0] != null)
                {
                    proposalViewModel.IMS_YEAR = Convert.ToInt32(defaultValues[0]);
                }

                if (defaultValues[1] != "" && defaultValues[1] != null)
                {
                    proposalViewModel.MAST_BLOCK_CODE = Convert.ToInt32(defaultValues[1]);
                }

                if (defaultValues[2] != "" && defaultValues[2] != null)
                {
                    proposalViewModel.IMS_BATCH = Convert.ToInt32(defaultValues[2]);
                }

                if (defaultValues[3] != "" && defaultValues[3] != null)
                {
                    proposalViewModel.IMS_COLLABORATION = Convert.ToInt32(defaultValues[3]);
                }

            }

            proposalViewModel.Years = PopulateYear();

            CommonFunctions objCommonFuntion = new CommonFunctions();
            proposalViewModel.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, proposalViewModel.IMS_YEAR, false, PMGSYSession.Current.PMGSYScheme == 1 ? true : false);

            ///Changed by SAMMED A. PATIL for RCPLWE
            proposalViewModel.COLLABORATIONS = new List<SelectListItem>();//objCommonFuntion.PopulateFundingAgency();
            proposalViewModel.COLLABORATIONS.Insert(0, new SelectListItem { Text = "Select Funding Agency", Value = "-1", Selected = true });

            proposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("P");
            proposalViewModel.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
            if (proposalViewModel.MAST_BLOCK_CODE != 0)
            {
                //proposalViewModel.CN_ROADS = PopulateLinkThrough(proposalViewModel.MAST_BLOCK_CODE, ""); //commented by shyam, because default IMS_UPGRADE_CONNECT value will be 'N'
                proposalViewModel.CN_ROADS = PopulateLinkThrough(proposalViewModel.MAST_BLOCK_CODE, "N", "P");
            }
            else
            {
                //proposalViewModel.CN_ROADS = PopulateLinkThrough(0, "");
                proposalViewModel.CN_ROADS = PopulateLinkThrough(0, "N", "P");
            }

            proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode); //PMGSYSession.Current.PMGSYScheme == 3 ? objCommonFuntion.PopulateBlocksforRCPLWE(PMGSYSession.Current.DistrictCode) : objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode);
            proposalViewModel.MP_CONSTITUENCY = objCommonFuntion.PopulateMPConstituency(0);
            proposalViewModel.MLA_CONSTITUENCY = objCommonFuntion.PopulateMLAConstituency(0);
            proposalViewModel.CARRIAGED_WIDTH = objCommonFuntion.PopulateCarriageWidth();
            proposalViewModel.HABS_REASON = objCommonFuntion.PopulateReason("H");
            proposalViewModel.EXISTING_SURFACE = objCommonFuntion.PopulateSurfaceType(false);
            proposalViewModel.TRAFFIC_TYPE = objCommonFuntion.PopulateTrafficType();
            proposalViewModel.PROPOSED_SURFACE = objCommonFuntion.PopulateProposedSurface();

            ViewBag.EXISTING_IMS_PACKAGE_ID = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

            //List<SelectListItem> lstYear = new List<SelectListItem>();
            //lstYear = PopulateYear();
            //int year = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Max(m => m.IMS_YEAR);
            //year = year + 1;
            //int count = lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString()));
            //lstYear.RemoveRange(lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString())), lstYear.Count - count);
            //ViewBag.lstYear = lstYear;

            ViewBag.lstYear = proposalViewModel.Years;

            ViewBag.Stage_2_Year = new SelectList(PopulateYear(0, false).AsEnumerable<SelectListItem>(), "Value", "Text", DateTime.Now.Year);
            ViewBag.Stage_2_Package_ID = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

            proposalViewModel.STAGE1_PROPOSAL_ROADS = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();

            #region CODE_ADDED_BY_VIKRAM_FOR_CHANGES_IN_PMGSY_SCHEME_1_COST

            proposalViewModel.IsProposalFinanciallyClosed = false;
            int shareCode = db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault();
            ViewBag.shareCode = shareCode;
            proposalViewModel.IMS_SHARE_PERCENT_2015 = shareCode == 0 ? (byte)3 : (byte)shareCode;

            #endregion

            return View(proposalViewModel);
        }

        /// <summary>
        /// returns blocks according to the state
        /// </summary>
        /// <param name="stateCode">indicates the state id</param>
        /// <returns></returns>
        [Audit]
        public JsonResult PopulateFundingAgency(int blockCode)
        {
            ProposalDAL objDAL = new ProposalDAL();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                List<SelectListItem> fundingAgencyList = objDAL.PopulateFundingAgency(blockCode);
                return Json(fundingAgencyList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateFundingAgency()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// returns blocks according to the state
        /// </summary>
        /// <param name="stateCode">indicates the state id</param>
        /// <returns></returns>
        [Audit]
        public JsonResult PopulateLinkThroughListRCPLWE(int blockCode)
        {
            ProposalDAL objDAL = new ProposalDAL();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                List<SelectListItem> linkThroughListRCPLWEList = objDAL.PopulateLinkThroughListRCPLWE(blockCode);
                return Json(linkThroughListRCPLWEList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateLinkThroughListRCPLWE()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Save Proposal
        /// </summary>
        /// <param name="ims_sanctioned_projects"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult Create(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)
        {
            ProposalDAL objprDAL = new DAL.Proposal.ProposalDAL();
            try
            {

                ModelState.Remove("TotalPostDLPMaintenanceCost");
                ModelState.Remove("PUCCA_SIDE_DRAIN_LENGTH");
                ModelState.Remove("PROTECTION_LENGTH");
                ModelState.Remove("SURFACE_BRICK_SOLLING");


                ModelState.Remove("SURFACE_BT");
                ModelState.Remove("SURFACE_CC");
                ModelState.Remove("SURFACE_GRAVEL");
                ModelState.Remove("SURFACE_MOORUM");

                ModelState.Remove("SURFACE_TRACK");
                ModelState.Remove("SURFACE_WBM");

                // Added on 18 March 2021
                ModelState.Remove("EXISTING_CARRIAGEWAY_WIDTH");
                ModelState.Remove("EXISTING_CARRIAGEWAY_PUC");

                ViewBag.operation = "C";
                if (ModelState.IsValid)
                {
                    string route = objprDAL.getRoadRoute(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE));
                    if (route == "N" && ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "U" && PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        return Json(new { Success = false, ErrorMessage = "Please select New Connectivity for Missing Link Route" });
                    }
                    //string Status = objProposalBAL.SaveRoadProposalBAL(ims_sanctioned_projects);
                    // Changes by Saurabh start here
                    string Status = string.Empty;
                    if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                    {
                        Status = objProposalBAL.SaveRoadProposalBAL(ims_sanctioned_projects);
                    }
                    else
                    {
                        Status = "User role is Un-authorized.";
                        return Json(new { Success = false, ErrorMessage = Status });
                    }
                    // Changes by Saurabh End here
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "Create(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)");
                throw ex;
            }
        }

        /// <summary>
        /// Checks whether proposal can be Finalized 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetProposalChecks()
        {
            ProposalDAL objprDAL = new DAL.Proposal.ProposalDAL();
            bool flg = false;
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);

            //In Case of Mord Unlock proposal
            string IMS_LOCK_STATUS = Request.Params["IMS_LOCK_STATUS"];

            //string route = objprDAL.getRoadRoute(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE));
            //if (route == "N" && ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "U" && PMGSYSession.Current.PMGSYScheme == 1)
            //{
            //    return Json(new { Success = false, ErrorMessage = "Please select New Connectivity for Missing Link Route" });
            //}


            string status = objProposalBAL.GetProposalChecksBAL(IMS_PR_ROAD_CODE, IMS_LOCK_STATUS);
            if (status == string.Empty)
                return Json(new { Success = true });
            else
                return Json(new { Success = false, ErrorMessage = status }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Finalize the Proposal at DPIU Level
        /// </summary>
        /// <returns></returns>

        [Audit]
        public JsonResult DPIUFinalizeProposal()
        {
            //Temp check for RJ to restrict finalisation of Proposals
            //if (PMGSYSession.Current.StateCode == 29 && PMGSYSession.Current.DistrictCode != 556)
            //{
            //    //return "Proposal cannot be finalised, Please contact NRIDA.";
            //    return Json(new { Success = false, ErrorMessage = "Proposal cannot be finalised, Please contact NRIDA." });
            //}

            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);
            string status = objProposalBAL.DPIUFinalizeProposalBAL(IMS_PR_ROAD_CODE);
            if (status == string.Empty)
                return Json(new { Success = true });
            else
                return Json(new { Success = false, ErrorMessage = status });
        }

        /// <summary>
        /// Replace the First Search Term from the Text 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>

        public string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        /// <summary>
        /// Display Road Proposal in Edit Form
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult Edit(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            ProposalDAL objProposalDAL = new ProposalDAL();

            bool isIAP = false;

            CommonFunctions objCommonFuntion = new CommonFunctions();
            int IMS_PR_ROAD_CODE = 0;
            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                }
            }
            ViewBag.operation = "U";

            IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);


            if (ims_sanctioned_projects == null)
            {
                return Json(new { Success = false, ErrorMessage = "Proposal Not Found" });
            }

            PMGSY.Models.Proposal.ProposalViewModel objProposal = new ProposalViewModel();

            var adapter = (IObjectContextAdapter)db;
            var objectContext = adapter.ObjectContext;
            objectContext.CommandTimeout = 0;

            if (db.ACC_BILL_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
            {
                objProposal.IS_PAYMENT_MADE = "Y";
            }
            else
            {
                objProposal.IS_PAYMENT_MADE = "N";
            }

            objProposal.operation = "U";    //For Update operation

            objProposal.StateName = PMGSYSession.Current.StateName;
            objProposal.DistrictName = PMGSYSession.Current.DistrictName;

            objProposal.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
            objProposal.MAST_STATE_CODE = ims_sanctioned_projects.MAST_STATE_CODE;
            objProposal.stateType = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_TYPE).FirstOrDefault();
            objProposal.MAST_DISTRICT_CODE = ims_sanctioned_projects.MAST_DISTRICT_CODE;
            objProposal.MAST_DPIU_CODE = ims_sanctioned_projects.MAST_DPIU_CODE;
            objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;

            objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;
            objProposal.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode);

            objProposal.IMS_COLLABORATION = ims_sanctioned_projects.IMS_COLLABORATION;
            objProposal.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency();

            //isIAP = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == objProposal.MAST_BLOCK_CODE && x.MAST_IAP_BLOCK == "Y").Any();

            // Upgradation Proposal
            if (ims_sanctioned_projects.IMS_UPGRADE_CONNECT.ToUpper() == "U")
            {
                objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;

                objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;

                if (ims_sanctioned_projects.IMS_ISBENEFITTED_HABS == "N")
                {
                    objProposal.IMS_HABS_REASON = ims_sanctioned_projects.IMS_HABS_REASON;
                }
            }
            objProposal.HABS_REASON = objCommonFuntion.PopulateReason("H");

            objProposal.IMS_IS_STAGED = ims_sanctioned_projects.IMS_IS_STAGED;

            // For staged Propsal
            if (objProposal.IMS_IS_STAGED == "S")
            {
                objProposal.IMS_STAGE_PHASE = ims_sanctioned_projects.IMS_STAGE_PHASE;
                //Stage 1
                if (objProposal.IMS_STAGE_PHASE == "S1")
                {
                    objProposal.IMS_STAGE_PHASE = "1";
                    objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                    //objProposal.CN_ROADS = PopulateLinkThrough(ims_sanctioned_projects.MAST_BLOCK_CODE, ims_sanctioned_projects.IMS_UPGRADE_CONNECT);
                    objProposal.CN_ROADS = PopulateOnlyLinkThrough(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE), ims_sanctioned_projects.MAST_BLOCK_CODE, ims_sanctioned_projects.IMS_UPGRADE_CONNECT);
                }
                else if (objProposal.IMS_STAGE_PHASE == "S2")
                {
                    objProposal.IMS_STAGE_PHASE = "2";
                    objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                    objProposal.CN_ROADS = PopulateStagedLinkThrough(Convert.ToInt32(ims_sanctioned_projects.IMS_STAGED_YEAR), ims_sanctioned_projects.IMS_BATCH, ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID, "U", (ims_sanctioned_projects.IMS_STAGED_ROAD_ID.HasValue ? ims_sanctioned_projects.IMS_STAGED_ROAD_ID.Value : 0));
                }
            }
            else if (objProposal.IMS_IS_STAGED == "C")
            {
                objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                ///Added by SAMMED A. PATIL for RCPLWE
                if (objProposal.IMS_COLLABORATION == 5)
                {
                    objProposal.CN_ROADS = objProposalDAL.PopulateLinkThroughListRCPLWE(objProposal.MAST_BLOCK_CODE);
                }
                else
                {
                    objProposal.CN_ROADS = PopulateOnlyLinkThrough(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE), ims_sanctioned_projects.MAST_BLOCK_CODE, ims_sanctioned_projects.IMS_UPGRADE_CONNECT);
                }
            }

            // objProposal.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == ims_sanctioned_projects.MAST_STATE_CODE select c.MAST_STATE_SHORT_CODE).FirstOrDefault();

            //added by shyam for PACKAGE_PREFIX as STATE_SHORT_CODE + MAST_DISTRICT_ID
            objProposal.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_SHORT_CODE).FirstOrDefault() +
                                               (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_DISTRICT_ID).FirstOrDefault();
            objProposal.IMS_EXISTING_PACKAGE = "E";
            objProposal.EXISTING_IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
            //objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;

            objProposal.EXISTING_PACKAGES = GetStagedPackageID(ims_sanctioned_projects.IMS_YEAR, ims_sanctioned_projects.IMS_BATCH);
            objProposal.EXISTING_PACKAGES.Insert(0, new SelectListItem { Text = "Select Package", Value = "" });
            objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
            objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;

            objProposal.IMS_PARTIAL_LEN = ims_sanctioned_projects.IMS_PARTIAL_LEN;

            objProposal.IMS_CC_LENGTH = ims_sanctioned_projects.IMS_CC_LENGTH;
            objProposal.IMS_BT_LENGTH = ims_sanctioned_projects.IMS_BT_LENGTH;

            // Pavement Length Client Side Validation Skipped
            objProposal.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;

            // For Stage Two Directly Get the Length of Road
            if (objProposal.IMS_STAGE_PHASE == "2")
            {
                if (ims_sanctioned_projects.PLAN_CN_ROAD_CODE != null)
                {
                    //string IMS_PAV_LENGTH = objProposalBAL.GetRoadDetailsForStageTwoProposalBAL(objProposal.IMS_PR_ROAD_CODE, Convert.ToInt32(objProposal.PLAN_CN_ROAD_CODE));
                    string IMS_PAV_LENGTH = objProposalBAL.GetRoadDetailsForStageTwoProposalBAL(Convert.ToInt32(ims_sanctioned_projects.IMS_STAGED_ROAD_ID), Convert.ToInt32(objProposal.PLAN_CN_ROAD_CODE));
                    if (IMS_PAV_LENGTH != string.Empty)
                    {
                        objProposal.DUP_IMS_PAV_LENGTH = Convert.ToDecimal(IMS_PAV_LENGTH);
                    }
                }

                //Added by shyam to take Stage I road length for Stage II Proposal
                objProposal.IMS_STAGE1_PAV_LENGTH = db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_STAGED_ROAD_ID).Select(c => c.IMS_PAV_LENGTH).FirstOrDefault();
                objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;
                objProposal.STAGE1_PROPOSAL_ROADS = new SelectList(db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_STAGED_ROAD_ID), "IMS_PR_ROAD_CODE", "IMS_ROAD_NAME").ToList();
            }
            // Here We Calculate the Remaining Length id 
            else
            {
                if (ims_sanctioned_projects.PLAN_CN_ROAD_CODE != null)
                {
                    /// Get the Roads Remaining Length 
                    string IMS_PAV_LENGTH = objProposalBAL.GetRoadDetailsForUpdate(objProposal.IMS_PR_ROAD_CODE, Convert.ToInt32(objProposal.PLAN_CN_ROAD_CODE));
                    /// Assign Remaining Length to Dummy Length For Validation ( Validation :Pavement Length Should not be greator than Available Length)
                    if (IMS_PAV_LENGTH != string.Empty)
                    {
                        objProposal.DUP_IMS_PAV_LENGTH = Convert.ToDecimal(IMS_PAV_LENGTH);
                    }
                }

                objProposal.STAGE1_PROPOSAL_ROADS = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();
            }



            objProposal.IMS_NO_OF_CDWORKS = ims_sanctioned_projects.IMS_NO_OF_CDWORKS;

            objProposal.IMS_ZP_RESO_OBTAINED = ims_sanctioned_projects.IMS_ZP_RESO_OBTAINED;
            objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS == null ? ims_sanctioned_projects.IMS_REMARKS : ims_sanctioned_projects.IMS_REMARKS.Trim();

            objProposal.IMS_STATE_SHARE = ims_sanctioned_projects.IMS_STATE_SHARE;
            objProposal.IMS_SANCTIONED_PAV_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;
            objProposal.IMS_SANCTIONED_CD_AMT = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;
            objProposal.IMS_SANCTIONED_PW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;
            objProposal.IMS_SANCTIONED_OW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;

            //For PMGSY Scheme-2
            if (PMGSYSession.Current.PMGSYScheme == 2)
            {
                objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
                objProposal.TotalCost = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT + ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT +
                                                    ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT + ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT +
                                                    Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT);
                objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;
                objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                objProposal.IMS_FURNITURE_COST = ims_sanctioned_projects.IMS_FURNITURE_COST;
                objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST;
                objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT;
                objProposal.IMS_SANCTIONED_FC_AMT = ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT;
                objProposal.IMS_SANCTIONED_RENEWAL_AMT = ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT;

                if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 1)
                {
                    objProposal.IMS_SANCTIONED_AMOUNT = (objProposal.TotalCost * 90) / 100;
                    //objProposal.IMS_STATE_SHARE = (objProposal.TotalCost * 10) / 100;
                }
                else if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 2)
                {
                    objProposal.IMS_SANCTIONED_AMOUNT = (objProposal.TotalCost * 75) / 100;
                    //objProposal.IMS_STATE_SHARE = (objProposal.TotalCost * 25) / 100;
                }
            }


            objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
            objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
            objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
            objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
            objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

            if (PMGSYSession.Current.PMGSYScheme == 3)
            {
                objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST ?? 0;
            }

            objProposal.TotalMaintenanceCost = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2 +
                                               ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4 +
                                               ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5 +    //In case of PMGSY Scheme-II include IMS_SANCTIONED_RENEWAL_AMT
                                               (PMGSYSession.Current.PMGSYScheme == 2
                                                ? Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT)
                                                : 0.0M);

            objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
            objProposal.Years = PopulateYear();//PopulateYear(ims_sanctioned_projects.IMS_YEAR);

            objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;

            //objProposal.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, objProposal.IMS_YEAR, false, PMGSYSession.Current.PMGSYScheme == 1 ? true : false);

            objProposal.BATCHS = new List<SelectListItem>();
            objProposal.BATCHS.Insert(0, new SelectListItem() { Text = "Batch " + Convert.ToString(ims_sanctioned_projects.IMS_BATCH), Value = Convert.ToString(ims_sanctioned_projects.IMS_BATCH) });

            objProposal.isPaymentDone = checkIsPayment(ims_sanctioned_projects.IMS_PR_ROAD_CODE);

            if (PMGSYSession.Current.StateCode == 17)
            { // Karnataka State. All Proposal to be edited in case of Payment Is Made. // Suggested by Pankaj Sir on 08 Jan 2021
                objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                objProposal.STREAMS = objCommonFuntion.PopulateStreams("P");
                objProposal.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
            }

            if (!objProposal.isPaymentDone)
            {
                objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                objProposal.STREAMS = objCommonFuntion.PopulateStreams("P");
                objProposal.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
            }



            objProposal.MAST_MP_CONST_CODE = ims_sanctioned_projects.MAST_MP_CONST_CODE;
            objProposal.MP_CONSTITUENCY = objCommonFuntion.PopulateMPConstituency(ims_sanctioned_projects.MAST_BLOCK_CODE);

            objProposal.MAST_MLA_CONST_CODE = ims_sanctioned_projects.MAST_MLA_CONST_CODE;
            objProposal.MLA_CONSTITUENCY = objCommonFuntion.PopulateMLAConstituency(ims_sanctioned_projects.MAST_BLOCK_CODE);

            objProposal.IMS_CARRIAGED_WIDTH = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH;
            objProposal.CARRIAGED_WIDTH = objCommonFuntion.PopulateCarriageWidth();

            objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;
            objProposal.EXISTING_SURFACE = objCommonFuntion.PopulateSurfaceType();

            objProposal.IMS_TRAFFIC_TYPE = ims_sanctioned_projects.IMS_TRAFFIC_TYPE;
            objProposal.TRAFFIC_TYPE = objCommonFuntion.PopulateTrafficType();

            objProposal.IMS_PROPOSED_SURFACE = ims_sanctioned_projects.IMS_PROPOSED_SURFACE;
            objProposal.PROPOSED_SURFACE = objCommonFuntion.PopulateProposedSurface();

            objProposal.Stage_2_Year = ims_sanctioned_projects.IMS_STAGED_YEAR;
            objProposal.Stage_2_Package_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;
            objProposal.PACKAGES = GetStagedPackageID(Convert.ToInt32(ims_sanctioned_projects.IMS_STAGED_YEAR), 0);
            //ViewBag.Stage_2_Package_ID = new SelectList(GetStagedPackageID(ims_sanctioned_projects.IMS_YEAR, 0), "Value", "Text");
            objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
            objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0;
            objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_MORD_SHARE_2015.Value : 0;
            objProposal.IMS_TOTAL_COST_2015 = objProposal.IMS_STATE_SHARE_2015 + objProposal.IMS_MORD_SHARE_2015;
            //objProposal.IMS_SHARE_PERCENT_2015 = (byte)(db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault() == 0 ? 3 : db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault());
            objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null ? ((byte)(db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault() == 0 ? 3 : db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault())) : ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
            //if (!objProposalDAL.IsProposalFinanciallyClosed(ims_sanctioned_projects.IMS_PR_ROAD_CODE) && ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null)
            //{
            //    objProposal.IMS_SHARE_PERCENT_2015 = 4;
            //}
            //else
            //{

            //}

            List<SelectListItem> lstYear = new List<SelectListItem>();
            lstYear = PopulateYear();
            int year = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Max(m => m.IMS_YEAR);
            if (year != DateTime.Now.Year)
            {
                year = year + 1;
            }
            int count = lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString()));
            lstYear.RemoveRange(lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString())), lstYear.Count - count);
            ViewBag.lstYear = lstYear;

            ViewBag.shareCode = objProposal.IMS_SHARE_PERCENT_2015;
            return View("Create", objProposal);
        }


        /// <summary>
        /// Populate Link Through
        /// </summary>
        /// <param name="BlockID"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetOnlyLinkThrough()
        {
            ProposalDAL objDAL = new DAL.Proposal.ProposalDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int BlockID = Convert.ToInt32(Request.Params["BlockID"]);
                string ims_upgrade_connect = Request.Params["IMS_UPGRADE_CONNECT"].ToString();
                string proposalType = Request.Params["PROPOSAL_TYPE"].ToString();

                int prRoadCode = Convert.ToInt32(Request.Params["PrRoadCode"]);
                int cnRoadCode = objDAL.getCNRoadCode(prRoadCode);

                return Json(PopulateOnlyLinkThrough(cnRoadCode, BlockID, ims_upgrade_connect));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetOnlyLinkThrough()");
                return null;
            }
            finally
            {

            }
        }
        /// <summary>
        /// Updates Road Proposal
        /// </summary>
        /// <param name="ims_sanctioned_projects"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult Edit(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)
        {
            ProposalDAL objprDAL = new DAL.Proposal.ProposalDAL();
            try
            {


                ModelState.Remove("TotalPostDLPMaintenanceCost");
                ModelState.Remove("PUCCA_SIDE_DRAIN_LENGTH");
                ModelState.Remove("PROTECTION_LENGTH");
                ModelState.Remove("SURFACE_BRICK_SOLLING");


                ModelState.Remove("SURFACE_BT");
                ModelState.Remove("SURFACE_CC");
                ModelState.Remove("SURFACE_GRAVEL");
                ModelState.Remove("SURFACE_MOORUM");

                ModelState.Remove("SURFACE_TRACK");
                ModelState.Remove("SURFACE_WBM");

                //if (PMGSYSession.Current.PMGSYScheme == 3)
                //{
                ModelState.Remove("EXISTING_CARRIAGEWAY_PUC");
                ModelState.Remove("EXISTING_CARRIAGEWAY_WIDTH");
                //}


                ViewBag.Operation = "E";
                if (ims_sanctioned_projects.isPaymentDone)
                {
                    ModelState.Remove("IMS_STREAMS");
                }
                if (ModelState.IsValid)
                {
                    string route = objprDAL.getRoadRoute(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE));
                    if (route == "N" && ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "U" && PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        return Json(new { Success = false, ErrorMessage = "Please select New Connectivity for Missing Link Route" });
                    }
                   // string Status = objProposalBAL.UpdateRoadProposalBAL(ims_sanctioned_projects);
                    // Changes by Saurabh Start Here..
                    string Status = string.Empty;
                    if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                    {
                        Status = objProposalBAL.UpdateRoadProposalBAL(ims_sanctioned_projects);
                    }
                    else
                    {
                        Status = "User Role is not Authorized.";
                        return Json(new { Success = false, ErrorMessage = Status });
                    }
                    // Changes by Saurabh Ends Here..

                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "Edit(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)");
                throw ex;
            }
        }

        /// <summary>
        /// Check if Propsoal can be Deleted.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult IsProposalDeleted(String parameter, String hash, String key)
        {
            int IMS_PR_ROAD_CODE = 0;
            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                }
            }
            else
            {
                return Json(new { success = false, errorMessage = "Error occured while processing your request." });
            }
            string status = string.Empty;
            try
            {
                status = objProposalBAL.IsProposalDeletedBAL(IMS_PR_ROAD_CODE);
                if (status == string.Empty)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "IsProposalDeleted()");
                return Json(new { success = false, errorMessage = status });
            }

        }

        /// <summary>
        /// Delete Road Proposal Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult DeleteConfirmed(String parameter, String hash, String key)
        {
            int IMS_PR_ROAD_CODE = 0;
            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);

                }
            }
            else
            {
                return Json(new { success = false, errorMessage = "There is an error occured while processing your request." });
            }

            string status = string.Empty;
            try
            {
                status = objProposalBAL.DeleteRoadProposalBAL(IMS_PR_ROAD_CODE);
                if (status == string.Empty)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteConfirmed()");
                return Json(new { success = false, errorMessage = status });
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }

        /// <summary>
        /// Check if Proposal Can be Edited
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult IsProposalEdited()
        {
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);

            bool IMS_TRAFFIC_TYPE = Convert.ToBoolean(Request.Params["IMS_TRAFFIC_TYPE"]);

            string status = objProposalBAL.IsProposalEditedBAL(IMS_PR_ROAD_CODE, IMS_TRAFFIC_TYPE);

            if (status == String.Empty)
            {
                return Json(new { Success = true });
            }
            else if (status == "S2")
            {
                return Json(new { Success = true, message = "S2" });
            }
            else
            {
                return Json(new { Success = false, Errormessage = status });
            }
        }

        /// <summary>
        /// Used In Case of User Edit Upgradation Proposal
        /// Check if Habitations are benifitted against it
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult IsHabitationsBenifitted()
        {
            try
            {
                int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);

                string status = objProposalBAL.IsHabitationsBenifitted(IMS_PR_ROAD_CODE);

                if (status == String.Empty)
                {
                    return Json(new { Success = true });
                }
                else
                {
                    return Json(new { Success = false, Errormessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "IsHabitationsBenifitted()");
                return Json(new { Success = false, Errormessage = "Error Occured while Processing Your Request." });
            }
        }

        /// <summary>
        /// Get the Road Details e.g. Length of Road including Proposals made on this Road        
        /// </summary>
        /// <returns></returns>

        [Audit]
        public JsonResult GetRoadDetails()
        {
            try
            {

                string ims_proposal_length = string.Empty;
                int PLAN_CN_ROAD_CODE = 0;
                int IMS_STAGED_ROAD_ID = 0;
                if (Request.Params["PLAN_CN_ROAD_CODE"].Trim() != "" && Request.Params["PLAN_CN_ROAD_CODE"] != null)
                {
                    PLAN_CN_ROAD_CODE = Convert.ToInt32(Request.Params["PLAN_CN_ROAD_CODE"]);
                }

                bool isStageTwoProposal = Convert.ToBoolean(Request.Params["IMS_STAGE_PHASE"]);

                if (isStageTwoProposal == true && Request.Params["IMS_STAGED_ROAD_ID"].Trim() != "" && Request.Params["IMS_STAGED_ROAD_ID"] != null)
                {
                    IMS_STAGED_ROAD_ID = Convert.ToInt32(Request.Params["IMS_STAGED_ROAD_ID"]);
                }

                int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);

                string IMS_PAV_LENGTH = objProposalBAL.GetRoadDetailsBAL(PLAN_CN_ROAD_CODE, isStageTwoProposal, IMS_PR_ROAD_CODE, IMS_STAGED_ROAD_ID);
                string IMS_ROAD_FROM = string.Empty;
                string IMS_ROAD_TO = string.Empty;
                string IMS_PARTIAL_LEN = string.Empty;
                string IMS_STAGE1_PAV_LENGTH = string.Empty;

                // For Stage II Proposal
                if ((db.IMS_SANCTIONED_PROJECTS.Where(c => c.MAST_STATE_CODE == PMGSYSession.Current.StateCode && c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode && c.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode && c.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && c.IMS_PROPOSAL_TYPE == "P").Any()))
                {
                    //IMS_SANCTIONED_PROJECTS ims_Sanctioned_project = db.IMS_SANCTIONED_PROJECTS.Where(a => a.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && a.IMS_PROPOSAL_TYPE == "P").First();

                    IMS_SANCTIONED_PROJECTS sanctioned_project = db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == IMS_STAGED_ROAD_ID && a.IMS_PROPOSAL_TYPE == "P").FirstOrDefault();

                    //If No Entry for IMS_STAGED_ROAD_ID, then take details using PLAN_CN_ROAD_CODE 
                    if (sanctioned_project == null)
                    {
                        sanctioned_project = db.IMS_SANCTIONED_PROJECTS.Where(a => a.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && a.IMS_PROPOSAL_TYPE == "P").First();
                    }

                    ims_proposal_length = sanctioned_project.IMS_PARTIAL_LEN;
                    IMS_ROAD_FROM = sanctioned_project.IMS_ROAD_FROM;
                    IMS_ROAD_TO = sanctioned_project.IMS_ROAD_TO;

                    //Added by shyam to take Stage I road length for Stage II Proposal
                    IMS_STAGE1_PAV_LENGTH = IMS_PAV_LENGTH;

                }
                return Json(new { Success = true, IMS_PAV_LENGTH = IMS_PAV_LENGTH, IMS_PARTIAL_LEN = ims_proposal_length, IMS_ROAD_FROM = IMS_ROAD_FROM, IMS_ROAD_TO = IMS_ROAD_TO, IMS_STAGE1_PAV_LENGTH = IMS_STAGE1_PAV_LENGTH });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetRoadDetails()");
                return Json(new { Success = false, IMS_PAV_LENGTH = 0 });
            }
        }
        #endregion

        #region UnlockedProposal
        /// <summary>
        /// Display Locked 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditUnLockedProposal(string parameter, string hash, string key)
        {
            ProposalDAL objDAL = new DAL.Proposal.ProposalDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            int IMS_PR_ROAD_CODE = 0;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                    }
                }

                UnlockProposalViewModel ProposalViewModel = objProposalBAL.GetPropsoalDetailsBAL(IMS_PR_ROAD_CODE);
                CommonFunctions objCommonFunction = new CommonFunctions();


                ////new code added by Vikram for enabling change provision of core network in unlock mode

                //// For staged Propsal
                //if (ProposalViewModel.IMS_IS_STAGED == "S")
                //{
                //    //Stage 1
                //    if (ProposalViewModel.IMS_STAGE_PHASE == "S1")
                //    {
                //        //ProposalViewModel.IMS_STAGE_PHASE = "1";
                //        ProposalViewModel.CN_ROADS = PopulateOnlyLinkThrough(Convert.ToInt32(ProposalViewModel.PLAN_CN_ROAD_CODE), ProposalViewModel.MAST_BLOCK_CODE, ProposalViewModel.IMS_UPGRADE_CONNECT);
                //    }
                //    else if (ProposalViewModel.IMS_STAGE_PHASE == "S2")
                //    {
                //        //ProposalViewModel.IMS_STAGE_PHASE = "2";
                //        ProposalViewModel.CN_ROADS = PopulateStagedLinkThrough(Convert.ToInt32(ProposalViewModel.IMS_STAGED_YEAR), ProposalViewModel.IMS_BATCH, ProposalViewModel.IMS_STAGED_PACKAGE_ID, "N", (ProposalViewModel.IMS_STAGED_ROAD_ID.HasValue ? ProposalViewModel.IMS_STAGED_ROAD_ID.Value : 0));
                //    }
                //}
                //else if (ProposalViewModel.IMS_IS_STAGED == "C")
                //{
                //    ProposalViewModel.CN_ROADS = PopulateOnlyLinkThrough(Convert.ToInt32(ProposalViewModel.PLAN_CN_ROAD_CODE), ProposalViewModel.MAST_BLOCK_CODE, ProposalViewModel.IMS_UPGRADE_CONNECT);
                //}

                //if (ProposalViewModel.IMS_STAGE_PHASE == "S2")
                //{
                //    ProposalViewModel.STAGE1_PROPOSAL_ROADS = new SelectList(db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == ProposalViewModel.IMS_STAGED_ROAD_ID), "IMS_PR_ROAD_CODE", "IMS_ROAD_NAME").ToList();
                //}
                //ProposalViewModel.Years = PopulateYear();
                //ProposalViewModel.PACKAGES = GetStagedPackageID(Convert.ToInt32(ProposalViewModel.IMS_STAGED_YEAR), 0);
                //ViewBag.Operation = "U";
                ////end of change

                //ProposalViewModel.BATCHS = new CommonFunctions().PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, ProposalViewModel.IMS_YEAR, false, true);
                ProposalViewModel.BATCHS = new List<SelectListItem>();
                ProposalViewModel.BATCHS.Insert(0, new SelectListItem() { Text = "Batch " + Convert.ToString(ProposalViewModel.IMS_BATCH), Value = Convert.ToString(ProposalViewModel.IMS_BATCH) });
                ///Changed by SAMMED A. PATIL for 
                ProposalViewModel.COLLABORATIONS = objDAL.PopulateFundingAgency(ProposalViewModel.MAST_BLOCK_CODE);//objCommonFuntion.PopulateFundingAgency();

                ProposalViewModel.isPaymentDone = checkIsPayment(IMS_PR_ROAD_CODE);

                if (PMGSYSession.Current.StateCode == 17)
                { // Karnataka State. All Proposal to be edited in case of Payment Is Made. // Suggested by Pankaj Sir on 08 Jan 2021
                    ProposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("P");
                    ProposalViewModel.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
                }

                if (!ProposalViewModel.isPaymentDone)
                {
                    ProposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("P");
                    ProposalViewModel.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
                }
                ProposalViewModel.PROPOSED_SURFACE = objCommonFuntion.PopulateProposedSurface();
                ProposalViewModel.EXISTING_SURFACE = objCommonFuntion.PopulateSurfaceType(false);
                ProposalViewModel.MP_CONSTITUENCY = objCommonFuntion.PopulateMPConstituency(ProposalViewModel.MAST_BLOCK_CODE);
                ProposalViewModel.MLA_CONSTITUENCY = objCommonFuntion.PopulateMLAConstituency(ProposalViewModel.MAST_BLOCK_CODE);
                ProposalViewModel.TRAFFIC_TYPE = objCommonFuntion.PopulateTrafficType();
                ProposalViewModel.PMGSYScheme = PMGSYSession.Current.PMGSYScheme;

                ProposalViewModel.CARRIAGED_WIDTH = objCommonFuntion.PopulateCarriageWidth();

                ProposalViewModel.Years = PopulateYear();
                //ViewBag.Stage_2_Year = new SelectList(PopulateYear(0, false).AsEnumerable<SelectListItem>(), "Value", "Text", DateTime.Now.Year);

                List<SelectListItem> lstYear = new List<SelectListItem>();
                lstYear = PopulateYear();
                int year = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Max(m => m.IMS_YEAR);
                if (year != DateTime.Now.Year)
                {
                    year = year + 1;
                }
                int count = lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString()));
                lstYear.RemoveRange(lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString())), lstYear.Count - count);
                ViewBag.lstYear = lstYear;

                //ViewBag.Stage_2_Package_ID = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

                ProposalViewModel.PACKAGES = new List<SelectListItem>();
                ProposalViewModel.PACKAGES.Insert(0, new SelectListItem { Text = "Select Stage 2 Package", Value = "" });
                if (ProposalViewModel.Stage_2_Package_ID != null)
                {
                    ProposalViewModel.PACKAGES.Insert(1, new SelectListItem { Text = ProposalViewModel.Stage_2_Package_ID, Value = ProposalViewModel.Stage_2_Package_ID });
                }

                ProposalViewModel.hdnISSTAGED = ProposalViewModel.IMS_IS_STAGED;

                ViewBag.operation = "U";

                return View("UnlockedProposal", ProposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.EditUnLockedProposal()");
                return null;
            }
        }

        public bool checkIsPayment(int prRoadCode)
        {
            try
            {
                bool status = objProposalBAL.checkIsPaymentBAL(prRoadCode);
                return status;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "checkIsPayment()");
                return false;
            }
            finally
            {

            }
        }

        /// <summary>
        /// Save the Unlocked Proposal Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult SaveUnLockedProposal(UnlockProposalViewModel proposalViewModel)
        {
            ProposalDAL objprDAL = new DAL.Proposal.ProposalDAL();

            if (proposalViewModel.PLAN_CN_ROAD_CODE == null)
            {
                return Json(new { Success = false, ErrorMessage = "Please Map Core Network" });
            }
            ///Above condition ADDED By SAMMED PATIL on 02DEC2016 as per instructions from Srinivasa Sir

            if (proposalViewModel.IMS_UPGRADE_CONNECT == "U")
            {
                if (proposalViewModel.MAST_EXISTING_SURFACE_CODE == null || proposalViewModel.MAST_EXISTING_SURFACE_CODE <= 0)
                {
                    ModelState.AddModelError("MAST_EXISTING_SURFACE_CODE", "Please select Existing Surface");
                }
            }
            if (proposalViewModel.IMS_YEAR >= 2012 && proposalViewModel.IMS_PAV_LENGTH < (decimal)0.5)
            {
                return Json(new { Success = false, ErrorMessage = "Invalid Pavement Length,Must be greater than 0.5 km" });
            }
            if (proposalViewModel.isPaymentDone)
            {
                ModelState.Remove("IMS_STREAMS");
            }
            if (ModelState.IsValid)
            {
                proposalViewModel.PLAN_CN_ROAD_CODE = objprDAL.getCNRoadCode(proposalViewModel.IMS_PR_ROAD_CODE);
                string route = objprDAL.getRoadRoute(Convert.ToInt32(proposalViewModel.PLAN_CN_ROAD_CODE));
                if (route == "N" && proposalViewModel.IMS_UPGRADE_CONNECT == "U" && PMGSYSession.Current.PMGSYScheme == 1)
                {
                    return Json(new { Success = false, ErrorMessage = "Please select New Connectivity for Missing Link Route" });
                }
                string Status = objProposalBAL.UpdateUnlockedProposalBAL(proposalViewModel);
                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
            }
        }

        /// <summary>
        /// Finalize Method for Unlocked Proposals
        /// </summary>
        /// <param name="unlockedProposalViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult FinalizeUnlockedProposal(UnlockProposalViewModel unlockedProposalViewModel)
        {
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);

            string status = objProposalBAL.DPIUFinalizeUnlockedProposalBAL(IMS_PR_ROAD_CODE);

            if (status == string.Empty)
                return Json(new { Success = true });
            else
                return Json(new { Success = false, ErrorMessage = status });
        }

        #endregion

        #region STA

        /// <summary>
        /// Enlist the Road Proposals for STA Login    
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetSTARoadProposals(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);

            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
            int IMS_State = Convert.ToInt32(Request.Params["IMS_STATE"]); //change on 23 june 2014 by deepak
            int totalRecords;

            var jsonData = new
            {
                rows = objProposalBAL.GetSTAProposalsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_State, IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// STA Scrutinize the Road Proposal
        /// </summary>
        /// <param name="staSanctionViewModel"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult STAFinalizeRoadProposal(StaSanctionViewModel staSanctionViewModel)
        {
            string status = objProposalBAL.StaFinalizeProposalBAL(staSanctionViewModel, "Y");

            if (status == "The District is allocated only for scrutiny of Bridge Proposal.")
            {
                return Json(new { Success = "false", errorMessage = status });
            }

            if (status == string.Empty)
            {
                return Json(new { Success = "true" });
            }
            {
                return Json(new { Success = "false", errorMessage = "There is an error occured while processing your request." });
            }

        }

        /// <summary>
        /// STA Un-Scrutinize the Road Proposal
        /// </summary>
        /// <param name="staSanctionViewModel"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult STAUnFinalizeRoadProposal(StaSanctionViewModel staSanctionViewModel)
        {
            string status = objProposalBAL.StaFinalizeProposalBAL(staSanctionViewModel, "U");

            if (status == string.Empty)
            {
                return Json(new { Success = "true" });
            }
            {
                return Json(new { Success = "false", errorMessage = "There is an error occured while processing your request." });
            }

        }

        /// <summary>
        /// Get the STA Scrutiny Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetStaScritiny(string id)
        {
            int IMS_PR_ROAD_CODE = Convert.ToInt32(id);
            IMS_SANCTIONED_PROJECTS ims_sanctioned_project = db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).First();

            StaSanctionViewModel staSanctionViewModel = new StaSanctionViewModel();
            staSanctionViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
            staSanctionViewModel.STA_SANCTIONED = ims_sanctioned_project.STA_SANCTIONED;
            if (PMGSYSession.Current.RoleCode == 3)
            {
                staSanctionViewModel.STA_SANCTIONED_BY = db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Any() ? db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_NAME).First() : (ims_sanctioned_project == null ? "NA" : ims_sanctioned_project.STA_SANCTIONED_BY.ToString()); //change done by Vikram on 26/05/2014 as if no data found in Admin Technical Agency then show the STA Name as it is from IMS_SANCTIONED_PROJECTS
            }
            else
            {
                staSanctionViewModel.STA_SANCTIONED_BY = db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == db.UM_User_Master.Where(a => a.UserName == ims_sanctioned_project.STA_SANCTIONED_BY).Select(a => a.UserID).FirstOrDefault()).Any() ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == db.UM_User_Master.Where(a => a.UserName == ims_sanctioned_project.STA_SANCTIONED_BY).Select(a => a.UserID).FirstOrDefault()).Select(b => b.ADMIN_TA_NAME).First() : (ims_sanctioned_project == null ? "NA" : ims_sanctioned_project.STA_SANCTIONED_BY.ToString()); //change done by Vikram on 26/05/2014 as if no data found in Admin Technical Agency then show the STA Name as it is from IMS_SANCTIONED_PROJECTS
            }

            staSanctionViewModel.IMS_ISCOMPLETED = ims_sanctioned_project.IMS_ISCOMPLETED;

            DateTime dateTime = new DateTime();
            if (ims_sanctioned_project.STA_SANCTIONED_DATE == null)
            {
                dateTime = DateTime.Now;
                staSanctionViewModel.STA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                staSanctionViewModel.STA_UNSCRUTINY_DATE = dateTime.ToString("dd-MMM-yyyy");
            }
            else
            {
                dateTime = Convert.ToDateTime(ims_sanctioned_project.STA_SANCTIONED_DATE);
                staSanctionViewModel.STA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                staSanctionViewModel.STA_UNSCRUTINY_DATE = dateTime.ToString("dd-MMM-yyyy");
            }

            staSanctionViewModel.MS_STA_REMARKS = ims_sanctioned_project.IMS_STA_REMARKS == null ? "" : ims_sanctioned_project.IMS_STA_REMARKS.Trim();

            return View("StaSactionProposal", staSanctionViewModel);
        }

        #endregion

        #region PTA

        /// <summary>
        /// Enlist the Road Proposals for PTA Login    
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetPTARoadProposals(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int MAST_STATE_ID = Convert.ToInt32(Request.Params["MAST_STATE_ID"]);
            int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);

            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];

            int totalRecords;

            var jsonData = new
            {
                rows = objProposalBAL.GetSTAProposalsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, MAST_STATE_ID, IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// PTA Scrutinize the Road Proposal
        /// </summary>
        /// <param name="ptaSanctionViewModel"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult PTAFinalizeRoadProposal(PtaSanctionViewModel ptaSanctionViewModel)
        {
            string status = string.Empty;

            //Change done by Vikram on 20 March 2014 for validating STA_SANCTIONED_DATE with PTA_SANCTIONED_DATE
            if (ModelState["PTA_SANCTIONED_DATE"].Errors.Count() == 0)
            {
                status = objProposalBAL.PtaFinalizeProposalBAL(ptaSanctionViewModel, "Y");
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = "PTA Sanction Date must be greater than or equal to STA Sanction Date." });
            }


            if (status == string.Empty)
            {
                return Json(new { Success = "true" });
            }
            {
                return Json(new { Success = "false", errorMessage = "There is an error occured while processing your request." });
            }

        }

        /// <summary>
        /// PTA Un-Scrutinize the Road Proposal
        /// </summary>
        /// <param name="ptaSanctionViewModel"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult PTAUnFinalizeRoadProposal(PtaSanctionViewModel ptaSanctionViewModel)
        {
            string status = objProposalBAL.PtaFinalizeProposalBAL(ptaSanctionViewModel, "U");

            if (status == string.Empty)
            {
                return Json(new { Success = "true" });
            }
            {
                return Json(new { Success = "false", errorMessage = "There is an error occured while processing your request." });
            }

        }

        /// <summary>
        /// Get the PTA Scrutiny Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetPtaScrutiny(string id)
        {
            int IMS_PR_ROAD_CODE = Convert.ToInt32(id);
            CommonFunctions objCommon = new CommonFunctions();
            IMS_SANCTIONED_PROJECTS ims_sanctioned_project = db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).First();

            PtaSanctionViewModel ptaSanctionViewModel = new PtaSanctionViewModel();
            ptaSanctionViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
            ptaSanctionViewModel.PTA_SANCTIONED = ims_sanctioned_project.PTA_SANCTIONED;
            ptaSanctionViewModel.PTA_SANCTIONED_BY = ims_sanctioned_project.PTA_SANCTIONED_BY;
            ptaSanctionViewModel.IMS_SANCTIONED = ims_sanctioned_project.IMS_SANCTIONED;
            ViewBag.IsTechnologyExist = db.IMS_PROPOSAL_TECH.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE) ? "Y" : "N";
            //Change done by Vikram on 20 March 2014 (for validating STA Sanction Date with PTA Sanction Date
            if (ims_sanctioned_project.STA_SANCTIONED_DATE != null)
            {
                ptaSanctionViewModel.STA_SANCTIONED_DATE = objCommon.GetDateTimeToString(ims_sanctioned_project.STA_SANCTIONED_DATE.Value);
            }
            //end of change

            ptaSanctionViewModel.NAME_OF_PTA = ims_sanctioned_project.PTA_SANCTIONED_BY == null
                                                    ? db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_NAME).FirstOrDefault()
                                                    : db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_TA_CODE == ims_sanctioned_project.PTA_SANCTIONED_BY).Select(a => a.ADMIN_TA_NAME).FirstOrDefault();

            DateTime dateTime = new DateTime();
            if (ims_sanctioned_project.PTA_SANCTIONED_DATE == null)
            {
                dateTime = DateTime.Now;
                ptaSanctionViewModel.PTA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                ptaSanctionViewModel.PTA_UNSCRUTINY_DATE = dateTime.ToString("dd-MMM-yyyy");
            }
            else
            {
                dateTime = Convert.ToDateTime(ims_sanctioned_project.PTA_SANCTIONED_DATE);
                ptaSanctionViewModel.PTA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                ptaSanctionViewModel.PTA_UNSCRUTINY_DATE = dateTime.ToString("dd-MMM-yyyy");
            }

            ptaSanctionViewModel.MS_PTA_REMARKS = ims_sanctioned_project.IMS_PTA_REMARKS == null ? "" : ims_sanctioned_project.IMS_PTA_REMARKS.Trim();

            return View("PtaSactionProposal", ptaSanctionViewModel);
        }

        #endregion

        #region MoRD

        /// <summary>
        /// Get the MoRD Sanction Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetMordSanctionDetails(string id)
        {
            int IMS_PR_ROAD_CODE = Convert.ToInt32(id);
            IMS_SANCTIONED_PROJECTS ims_sanctioned_project = db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).First();

            MordSanctionViewModel mordSanctionViewModel = new MordSanctionViewModel();
            mordSanctionViewModel.OperationType = "D";
            mordSanctionViewModel.IMS_PR_ROAD_CODE = ims_sanctioned_project.IMS_PR_ROAD_CODE;
            mordSanctionViewModel.IMS_SANCTIONED_CD_AMT = ims_sanctioned_project.IMS_SANCTIONED_CD_AMT;
            mordSanctionViewModel.IMS_SANCTIONED_PAV_AMT = ims_sanctioned_project.IMS_SANCTIONED_PAV_AMT;
            mordSanctionViewModel.IMS_SANCTIONED_PW_AMT = ims_sanctioned_project.IMS_SANCTIONED_PW_AMT;
            mordSanctionViewModel.IMS_SANCTIONED_OW_AMT = ims_sanctioned_project.IMS_SANCTIONED_OW_AMT;
            mordSanctionViewModel.IMS_SANCTIONED_RS_AMT = ims_sanctioned_project.IMS_SANCTIONED_RS_AMT;

            //PMGSY Scheme2
            mordSanctionViewModel.IMS_SANCTIONED_FC_AMT = ims_sanctioned_project.IMS_SANCTIONED_FC_AMT;
            mordSanctionViewModel.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_project.IMS_IS_HIGHER_SPECIFICATION;
            mordSanctionViewModel.IMS_SANCTIONED_HS_AMT = ims_sanctioned_project.IMS_SANCTIONED_HS_AMT;

            mordSanctionViewModel.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_project.IMS_SANCTIONED_MAN_AMT1;
            mordSanctionViewModel.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_project.IMS_SANCTIONED_MAN_AMT2;
            mordSanctionViewModel.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_project.IMS_SANCTIONED_MAN_AMT3;
            mordSanctionViewModel.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_project.IMS_SANCTIONED_MAN_AMT4;
            mordSanctionViewModel.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_project.IMS_SANCTIONED_MAN_AMT5;

            //PMGSY Scheme2
            mordSanctionViewModel.IMS_SANCTIONED_RENEWAL_AMT = ims_sanctioned_project.IMS_SANCTIONED_RENEWAL_AMT;

            mordSanctionViewModel.TotalMaintenanceCost = ims_sanctioned_project.IMS_SANCTIONED_MAN_AMT1 + ims_sanctioned_project.IMS_SANCTIONED_MAN_AMT2 +
                                                         ims_sanctioned_project.IMS_SANCTIONED_MAN_AMT3 + ims_sanctioned_project.IMS_SANCTIONED_MAN_AMT4 +
                                                         ims_sanctioned_project.IMS_SANCTIONED_MAN_AMT5 +
                                                         (ims_sanctioned_project.IMS_SANCTIONED_RENEWAL_AMT == null ? 0.0M : Convert.ToDecimal(ims_sanctioned_project.IMS_SANCTIONED_RENEWAL_AMT));

            //mordSanctionViewModel.IMS_SANCTIONED_BY = ims_sanctioned_project.IMS_SANCTIONED_BY;   
            mordSanctionViewModel.IMS_SANCTIONED_BY = PMGSYSession.Current.UserName;
            mordSanctionViewModel.IMS_SANCTIONED_BY_TEXT = ims_sanctioned_project.IMS_SANCTIONED_BY == "NULL" ? "-" : ims_sanctioned_project.IMS_SANCTIONED_BY;
            if (ims_sanctioned_project.IMS_SANCTIONED_DATE == null)
                //mordSanctionViewModel.IMS_SANCTIONED_DATE = DateTime.Now.ToString("dd-MMM-yyyy");
                ///Changed by SAMMED A. PATIL on 11JULY2017
                mordSanctionViewModel.IMS_SANCTIONED_DATE = "-";
            else
                mordSanctionViewModel.IMS_SANCTIONED_DATE = Convert.ToDateTime(ims_sanctioned_project.IMS_SANCTIONED_DATE).ToString("dd-MMM-yyyy");

            mordSanctionViewModel.IMS_SANCTIONED = ims_sanctioned_project.IMS_SANCTIONED;
            mordSanctionViewModel.IMS_PROG_REMARKS = ims_sanctioned_project.IMS_PROG_REMARKS;
            mordSanctionViewModel.IMS_ISCOMPLETED = ims_sanctioned_project.IMS_ISCOMPLETED;
            mordSanctionViewModel.IMS_REASON = ims_sanctioned_project.IMS_REASON == null ? 0 : ims_sanctioned_project.IMS_REASON;
            if (mordSanctionViewModel.IMS_SANCTIONED == "R")
            {
                mordSanctionViewModel.REASONS = new CommonFunctions().PopulateReason("R");

            }
            else if (mordSanctionViewModel.IMS_SANCTIONED == "D")
            {
                mordSanctionViewModel.REASONS = new CommonFunctions().PopulateReason("D");
            }
            else
            {
                mordSanctionViewModel.REASONS = new CommonFunctions().PopulateReason("", false);
            }
            if (ims_sanctioned_project.IMS_REASON != null)
            {
                ViewBag.Reason = ims_sanctioned_project.MASTER_REASON1.MAST_REASON_NAME;
            }
            List<IMS_GET_ACTIONS_FOR_MORD_Result> list_IMS_GET_ACTIONS_FOR_MORD_Result = new ProposalBAL().GetMordActions(ims_sanctioned_project.IMS_PR_ROAD_CODE);
            mordSanctionViewModel.IS_SANCTIONABLE = Convert.ToBoolean(list_IMS_GET_ACTIONS_FOR_MORD_Result[0].SANCTIONABLE);
            mordSanctionViewModel.IS_UNSANCTIONABLE = Convert.ToBoolean(list_IMS_GET_ACTIONS_FOR_MORD_Result[0].UNSANCTIONABLE);
            mordSanctionViewModel.IS_RECONSIDERABLE = Convert.ToBoolean(list_IMS_GET_ACTIONS_FOR_MORD_Result[0].RECONSIDERABLE);
            mordSanctionViewModel.IS_DROPPABLE = Convert.ToBoolean(list_IMS_GET_ACTIONS_FOR_MORD_Result[0].DROPPABLE);
            mordSanctionViewModel.IS_EXECUTION_STARTED = list_IMS_GET_ACTIONS_FOR_MORD_Result[0].IS_EXECUTION_STARTED;
            mordSanctionViewModel.IS_AGREEMENT_FINALIZED = list_IMS_GET_ACTIONS_FOR_MORD_Result[0].IS_AGREEMENT_FINALIZED;

            return View("MordSanctionRoadProposal", mordSanctionViewModel);
        }

        /// <summary>
        /// Update the Mord Sanction Details
        /// </summary>
        /// <param name="mordSanctionViewModel"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult UpdateMordSanctionDetails(MordSanctionViewModel mordSanctionViewModel)
        {
            string status = objProposalBAL.UpdateMordSanctionDetailsBAL(mordSanctionViewModel);

            if (status == string.Empty)
            {
                string Operation = string.Empty;
                if (mordSanctionViewModel.IMS_SANCTIONED == "R")
                    Operation = "Recommended for Improvement";
                else if (mordSanctionViewModel.IMS_SANCTIONED == "D")
                    Operation = "Dropped";
                else if (mordSanctionViewModel.IMS_SANCTIONED == "Y")
                    Operation = "Sanctioned";
                else if (mordSanctionViewModel.IMS_SANCTIONED == "U")
                    Operation = "UnSanctioned";

                return Json(new { Success = true, Message = "Proposal " + Operation + " Successfully." });
            }
            else
                return Json(new { Success = false, ErrorMessage = status });
        }

        /// <summary>
        /// List the Road Proposals for MoRD Login
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetMORDRoadProposals(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int MAST_STATE_ID = Convert.ToInt32(Request.Params["MAST_STATE_ID"]);
            int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
            string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
            int IMS_AGENCY = Convert.ToInt32(Request.Params["IMS_AGENCY"]);

            int totalRecords;
            ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
            var jsonData = new
            {
                rows = objProposalBAL.GetMordProposalsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_AGENCY, IMS_UPGRADE_CONNECT, out colTotal),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
                TotalColumn = colTotal
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Populates Reasons 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateReasons()
        {
            if (Request.Params["IMS_SANCTIONED"] != string.Empty)
            {
                //return Json(new CommonFunctions().PopulateReason(Request.Params["IMS_SANCTIONED"]));

                // For any type of proposal i.e. Dropped, Sanctioned, Reconsidered - Populate reason of type S
                return Json(new CommonFunctions().PopulateReason("S"));
            }
            else
            {
                return Json(string.Empty);
            }
        }

        /// <summary>
        /// Get the Bulk Sanction UI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        [HttpPost]
        public ActionResult BulkDetails()
        {
            MordSanctionViewModel bulkMordDetails = objProposalBAL.GetBulkMordDetailBAL(Request.Params["Proposals[]"]);
            bulkMordDetails.IMS_PR_ROAD_CODES = Request.Params["Proposals[]"];
            bulkMordDetails.IMS_SANCTIONED_DATE = DateTime.Now.ToString("dd-MMM-yyyy");
            bulkMordDetails.IMS_SANCTIONED_BY = PMGSYSession.Current.UserName;
            return View("BulkDetails", bulkMordDetails);
        }

        /// <summary>
        /// Bulk Sanction the Road Proposals
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult BulkSanction(MordSanctionViewModel mordSanctionViewModel)
        {
            if (ModelState.IsValid)
            {
                string status = new ProposalBAL().BulkMordDetailBAL(mordSanctionViewModel);
                if (status == string.Empty)
                {
                    return Json(new { Success = true, Message = "Proposals Sanctioned Successfully." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
            }

        }

        #endregion

        #region Habitation Details

        /// <summary>
        /// Mapped Habitation List for Proposal
        /// Appears when user Clicks on Habitation link in Proposal Grid 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetHabitationList(FormCollection formCollection)
        {
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            var jsonData = new
            {
                rows = objProposalBAL.GetHabitationListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
                userdata = new { CreateCluster = "<input type='button' id='CreateCluster' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' value='Create Cluster'></input>", EditCluster = "<input type='button' id='EditCluster' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' value='Update Clusters'></input>" }
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Screen : Map Habitations to the Proposal
        /// Get Method to Add Habitation to the Proposal 
        /// </summary>
        /// <param name="id1"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult AddHabitation(String parameter, String hash, String key)
        {

            int IMS_PR_ROAD_CODE = 0;
            string IMS_LOCK_STATUS = string.Empty;
            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);

                    //Taken for is Mord Unlocked Status
                    IMS_LOCK_STATUS = urlSplitParams[1];
                }
            }

            HabitationViewModel habitationDetails = new HabitationViewModel();

            IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(Convert.ToInt32(IMS_PR_ROAD_CODE));

            //Taken for is Mord Unlocked Status
            habitationDetails.IMS_LOCK_STATUS = IMS_LOCK_STATUS;


            if (ims_sanctioned_projects.IMS_IS_STAGED == "S" && ims_sanctioned_projects.IMS_STAGE_PHASE == "S2")
            {
                habitationDetails.IsStageTwoProposal = true;
            }
            else
            {
                habitationDetails.IsStageTwoProposal = false;
            }

            habitationDetails.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
            habitationDetails.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
            habitationDetails.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
            //habitationDetails.PLAN_RD_NAME = (ims_sanctioned_projects.PLAN_ROAD != null) ? ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME : "NA";
            habitationDetails.PLAN_RD_NAME = ims_sanctioned_projects.IMS_ROAD_FROM + " " + ims_sanctioned_projects.IMS_ROAD_TO;
            habitationDetails.IMS_PAV_LENGTH = Convert.ToDecimal(ims_sanctioned_projects.IMS_PAV_LENGTH);

            habitationDetails.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
            habitationDetails.PLAN_CN_ROAD_CODE = (ims_sanctioned_projects.PLAN_CN_ROAD_CODE).ToString();

            habitationDetails.MAST_STATE_TYPE = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == ims_sanctioned_projects.MAST_STATE_CODE select c.MAST_STATE_TYPE).First().ToUpper();
            habitationDetails.MAST_IAP_DISTRICT = (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == ims_sanctioned_projects.MAST_DISTRICT_CODE select c.MAST_IAP_DISTRICT).First().ToUpper();
            MASTER_BLOCK blockMaster = db.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == ims_sanctioned_projects.MAST_BLOCK_CODE).FirstOrDefault();

            habitationDetails.MAST_IS_TRIBAL = (from c in db.MASTER_BLOCK where c.MAST_BLOCK_CODE == ims_sanctioned_projects.MAST_BLOCK_CODE select c.MAST_IS_TRIBAL).First().ToUpper();
            habitationDetails.MAST_BLOCK_IS_DESERT = blockMaster.MAST_IS_DESERT;
            habitationDetails.MAST_BLOCK_SCHEDULE5 = blockMaster.MAST_SCHEDULE5;
            habitationDetails.MAST_IAP_BLOCK = blockMaster.MAST_IAP_BLOCK;
            habitationDetails.IMS_ISCOMPLETED = ims_sanctioned_projects.IMS_ISCOMPLETED;

            habitationDetails.HAB_CODES_LIST = string.Empty;

            ViewBag.MASTER_HABITATION = new SelectList(PopulateHabitations(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE), IMS_PR_ROAD_CODE).AsEnumerable<SelectListItem>(), "Value", "Text");
            ViewBag.MAST_HAB_CODE = new SelectList(PopulateHabitations(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE), IMS_PR_ROAD_CODE).AsEnumerable<SelectListItem>(), "Value", "Text");
            return PartialView(habitationDetails);
        }

        /// <summary>
        /// Screen : Map Habitations to the Proposal
        /// Get Method to Add Habitation to the Proposal 
        /// </summary>
        /// <param name="id1"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult AddHabitationWithCluster(String parameter, String hash, String key)
        {

            int IMS_PR_ROAD_CODE = 0;
            string IMS_LOCK_STATUS = string.Empty;
            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);

                    //Taken for is Mord Unlocked Status
                    IMS_LOCK_STATUS = urlSplitParams[1];
                }
            }

            HabitationClusterViewModel habitationDetails = new HabitationClusterViewModel();

            IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(Convert.ToInt32(IMS_PR_ROAD_CODE));

            //Taken for is Mord Unlocked Status
            habitationDetails.IMS_LOCK_STATUS = IMS_LOCK_STATUS;


            if (ims_sanctioned_projects.IMS_IS_STAGED == "S" && ims_sanctioned_projects.IMS_STAGE_PHASE == "S2")
            {
                habitationDetails.IsStageTwoProposal = true;
            }
            else
            {
                habitationDetails.IsStageTwoProposal = false;
            }

            habitationDetails.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
            habitationDetails.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
            habitationDetails.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
            //habitationDetails.PLAN_RD_NAME = (ims_sanctioned_projects.PLAN_ROAD != null) ? ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME : "NA";
            habitationDetails.PLAN_RD_NAME = ims_sanctioned_projects.IMS_ROAD_FROM + " " + ims_sanctioned_projects.IMS_ROAD_TO;
            habitationDetails.IMS_PAV_LENGTH = Convert.ToDecimal(ims_sanctioned_projects.IMS_PAV_LENGTH);

            habitationDetails.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
            habitationDetails.PLAN_CN_ROAD_CODE = (ims_sanctioned_projects.PLAN_CN_ROAD_CODE).ToString();

            habitationDetails.MAST_STATE_TYPE = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == ims_sanctioned_projects.MAST_STATE_CODE select c.MAST_STATE_TYPE).First().ToUpper();
            habitationDetails.MAST_IAP_DISTRICT = (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == ims_sanctioned_projects.MAST_DISTRICT_CODE select c.MAST_IAP_DISTRICT).First().ToUpper();
            MASTER_BLOCK blockMaster = db.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == ims_sanctioned_projects.MAST_BLOCK_CODE).FirstOrDefault();

            habitationDetails.MAST_IS_TRIBAL = (from c in db.MASTER_BLOCK where c.MAST_BLOCK_CODE == ims_sanctioned_projects.MAST_BLOCK_CODE select c.MAST_IS_TRIBAL).First().ToUpper();
            habitationDetails.MAST_BLOCK_IS_DESERT = blockMaster.MAST_IS_DESERT;
            habitationDetails.MAST_BLOCK_SCHEDULE5 = blockMaster.MAST_SCHEDULE5;
            habitationDetails.MAST_IAP_BLOCK = blockMaster.MAST_IAP_BLOCK;
            habitationDetails.IMS_ISCOMPLETED = ims_sanctioned_projects.IMS_ISCOMPLETED;

            habitationDetails.HAB_CODES_LIST = string.Empty;

            ViewBag.MASTER_HABITATION = new SelectList(PopulateHabitations(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE), IMS_PR_ROAD_CODE).AsEnumerable<SelectListItem>(), "Value", "Text");
            ViewBag.MAST_HAB_CODE = new SelectList(PopulateHabitations(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE), IMS_PR_ROAD_CODE).AsEnumerable<SelectListItem>(), "Value", "Text");
            ViewBag.MAST_CLUSTER_CODE = new SelectList(PopulateClusters(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE), IMS_PR_ROAD_CODE).AsEnumerable<SelectListItem>(), "Value", "Text");
            return PartialView(habitationDetails);
        }


        /// <summary>
        /// Screen : Map Habitations to the Proposal
        /// Adding the Habitation to Proposal
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddHabitation(HabitationViewModel habModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string status = objProposalBAL.AddHabitationDetailBAL(habModel);

                    if (status == string.Empty)
                    {
                        return Json(new {success=true });
                    }
                    else
                    {
                        return Json(new { success=false, message=status});
                    }
                }
                return Json(
                new
                {
                    success = false
                });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalController().AddHabitation()  ::  public ActionResult AddHabitation(HabitationViewModel habModel)");
                return null;
            }
        }

        /// <summary>
        /// Screen : Map Habitations to the Proposal
        /// Adding the Habitation to Proposal
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddCluster(HabitationClusterViewModel habModel)
        {
            if (ModelState.IsValid)
            {
                string status = objProposalBAL.AddHabitationClusterDetailBAL(habModel);

                if (status == string.Empty)
                {
                    return Json(
                    new
                    {
                        success = true
                    });
                }
                else
                {
                    return Json(new { success = false, message = status });
                }
            }
            return Json(
            new
            {
                success = false
            });
        }

        /// <summary>
        /// Screen : Map Habitations to the Proposal
        /// Populate the Habitations 
        /// </summary>
        /// <param name="PLAN_CN_ROAD_CODE"></param>
        /// <returns></returns> 
        [Audit]
        public List<SelectListItem> PopulateHabitations(int PLAN_CN_ROAD_CODE, int IMS_PR_ROAD_CODE)
        {
            List<SelectListItem> HabList = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();

            IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(Convert.ToInt32(IMS_PR_ROAD_CODE));

            var habitatioms = (from c in db.MASTER_HABITATIONS
                               join o in db.PLAN_ROAD_HABITATION
                               on c.MAST_HAB_CODE equals o.MAST_HAB_CODE
                               join a in db.MASTER_HABITATIONS_DETAILS
                               on c.MAST_HAB_CODE equals a.MAST_HAB_CODE
                               where
                               o.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE
                               &&
                                   ((PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3)///Changes for RCPLWE
                                        ? (!db.IMS_BENEFITED_HABS.Any(p => p.MAST_HAB_CODE == c.MAST_HAB_CODE && p.HAB_INCLUDED == "Y"))// && p.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE
                                        : (!db.IMS_BENEFITED_HABS.Any(p => p.MAST_HAB_CODE == c.MAST_HAB_CODE && p.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                                   )
                               &&
                               a.MAST_YEAR == ((PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ? 2001 : 2011)  //For Scheme-1, Census year always 2001 & for Scheme-2, Census Year always 2011.

                               && !c.MAST_HAB_STATUS.Contains("F") && !c.MAST_HAB_STATUS.Contains("S")  //Populate only Habitations with Status U & C
                               && (ims_sanctioned_projects.IMS_UPGRADE_CONNECT.Equals("N") ? a.MAST_HAB_CONNECTED : "1") == (ims_sanctioned_projects.IMS_UPGRADE_CONNECT.Equals("N") ? "N" : "1")    //new change done by Vikram

                               select new
                               {
                                   Value = c.MAST_HAB_CODE,
                                   Text = c.MAST_HAB_NAME,
                                   Population = a.MAST_HAB_TOT_POP,
                                   Village = c.MASTER_VILLAGE.MAST_VILLAGE_NAME
                               }
                        ).OrderBy(c => c.Text);

            habitatioms =
                (PMGSYSession.Current.PMGSYScheme == 2)
                ? (from mh in db.MASTER_HABITATIONS
                   join mhd in db.MASTER_HABITATIONS_DETAILS
                   on mh.MAST_HAB_CODE equals mhd.MAST_HAB_CODE
                   join prh in db.PLAN_ROAD_HABITATION
                   on mh.MAST_HAB_CODE equals prh.MAST_HAB_CODE
                   where
                   !(
                     from ims in db.IMS_SANCTIONED_PROJECTS
                     join bh in db.IMS_BENEFITED_HABS
                     on ims.IMS_PR_ROAD_CODE equals bh.IMS_PR_ROAD_CODE
                     where ims.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                     select bh.MAST_HAB_CODE
                   ).Contains(prh.MAST_HAB_CODE)
                   && prh.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE
                   && mhd.MAST_YEAR == ((PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ? 2001 : 2011)///Changes for RCPLWE
                   select new
                   {
                       Value = mh.MAST_HAB_CODE,
                       Text = mh.MAST_HAB_NAME,
                       Population = mhd.MAST_HAB_TOT_POP,
                       Village = mh.MASTER_VILLAGE.MAST_VILLAGE_NAME
                   }).OrderBy(m => m.Text)
                          ///PMGSY 1 or 3
                          : (from mh in db.MASTER_HABITATIONS
                             join mhd in db.MASTER_HABITATIONS_DETAILS
                             on mh.MAST_HAB_CODE equals mhd.MAST_HAB_CODE
                             join prh in db.PLAN_ROAD_HABITATION
                             on mh.MAST_HAB_CODE equals prh.MAST_HAB_CODE
                             where
                             !(
                               from ims in db.IMS_SANCTIONED_PROJECTS
                               join bh in db.IMS_BENEFITED_HABS
                               on ims.IMS_PR_ROAD_CODE equals bh.IMS_PR_ROAD_CODE
                               where ims.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                               select bh.MAST_HAB_CODE
                             ).Contains(prh.MAST_HAB_CODE)
                             && prh.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE
                             && mhd.MAST_YEAR == ((PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ? 2001 : 2011)///Changes for RCPLWE
                             && mh.MAST_HAB_STATUS != "F" && mh.MAST_HAB_STATUS != "S"
                             && ((ims_sanctioned_projects.IMS_UPGRADE_CONNECT.Equals("N") || ims_sanctioned_projects.IMS_UPGRADE_CONNECT.Equals("U")) ? mhd.MAST_HAB_CONNECTED : "1") == (ims_sanctioned_projects.IMS_UPGRADE_CONNECT.Equals("N") ? "N" : ims_sanctioned_projects.IMS_UPGRADE_CONNECT.Equals("U") ? "Y" : "1")
                             //&& mhd.MAST_HAB_CONNECTED == ims_sanctioned_projects.IMS_UPGRADE_CONNECT
                             select new
                             {
                                 Value = mh.MAST_HAB_CODE,
                                 Text = mh.MAST_HAB_NAME,
                                 Population = mhd.MAST_HAB_TOT_POP,
                                 Village = mh.MASTER_VILLAGE.MAST_VILLAGE_NAME
                             }).OrderBy(m => m.Text);
            // .OrderBy(m => m.Text);

            // Added by Srishti
            if (PMGSYSession.Current.PMGSYScheme == 5)
            {
                habitatioms = (from mh in db.MASTER_HABITATIONS
                               join mhd in db.MASTER_HABITATIONS_DETAILS
                               on mh.MAST_HAB_CODE equals mhd.MAST_HAB_CODE
                               join prh in db.PLAN_ROAD_HABITATION
                               on mh.MAST_HAB_CODE equals prh.MAST_HAB_CODE
                               where
                               !(
                                 from ims in db.IMS_SANCTIONED_PROJECTS
                                 join bh in db.IMS_BENEFITED_HABS
                                 on ims.IMS_PR_ROAD_CODE equals bh.IMS_PR_ROAD_CODE
                                 where ims.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                                 where ims.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                                 select bh.MAST_HAB_CODE
                               ).Contains(prh.MAST_HAB_CODE)
                               && prh.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE
                               && mhd.MAST_YEAR == ((PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ? 2001 : 2011)///Changes for RCPLWE
                               //&& mh.MAST_HAB_STATUS != "F" && mh.MAST_HAB_STATUS != "S" // Change as per instructed by NRRIDA as on 04-09-2023  (Include both connected and unconnected habitations)
                               // && mhd.MAST_HAB_CONNECTED == "Y"
                               //&& mhd.MAST_HAB_CONNECTED == "N" // Change as per instructed by NRRIDA as on 04-09-2023 (Include both connected and unconnected habitations)
                               select new
                               {
                                   Value = mh.MAST_HAB_CODE,
                                   Text = mh.MAST_HAB_NAME,
                                   Population = mhd.MAST_HAB_TOT_POP,
                                   Village = mh.MASTER_VILLAGE.MAST_VILLAGE_NAME
                               }).OrderBy(m => m.Text);
            }

            foreach (var data in habitatioms)
            {
                item = new SelectListItem();
                item.Text = data.Text + "  ( Population :" + data.Population + " Village : " + data.Village + ")";
                item.Value = data.Value.ToString();
                HabList.Add(item);
            }

            return HabList;
        }

        /// <summary>
        /// Screen : Map Cluster to the Proposal
        /// Populate the Clusters 
        /// </summary>
        /// <param name="PLAN_CN_ROAD_CODE"></param>
        /// <returns></returns> 
        [Audit]
        public List<SelectListItem> PopulateClusters(int PLAN_CN_ROAD_CODE, int IMS_PR_ROAD_CODE)
        {
            List<SelectListItem> HabList = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();

            IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(Convert.ToInt32(IMS_PR_ROAD_CODE));

            var clusters = db.IMS_GET_CLUSTERS_FOR_PROPOSALS(ims_sanctioned_projects.MAST_BLOCK_CODE, PLAN_CN_ROAD_CODE, PMGSYSession.Current.PMGSYScheme);

            HabList = new SelectList(clusters.ToList(), "MAST_CLUSTER_CODE", "MAST_CLUSTER_NAME").ToList();

            return HabList;
        }


        /// <summary>
        /// Screen : Map Habitations to the Proposal
        /// Get The Habitations when mapping habitations to proposal
        /// Called When User Delete the habitaion , to repopulate the habitations
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetHabitations()
        {
            int PLAN_CN_ROAD_CODE = 0;
            if (Request.Params[""] == "")
            {
                return Json
                (
                    new { string.Empty }
                );
            }
            else
            {
                if (Request.Params["PLAN_CN_ROAD_CODE"].Trim() != "" && Request.Params["PLAN_CN_ROAD_CODE"] != null)
                {
                    PLAN_CN_ROAD_CODE = Convert.ToInt32(Request.Params["PLAN_CN_ROAD_CODE"]);
                }

                int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);
                ///PMGSY3 Changes
                //return Json(PopulateHabitations(PLAN_CN_ROAD_CODE, IMS_PR_ROAD_CODE));
                ProposalDAL objDAL = new DAL.Proposal.ProposalDAL();
                return Json(PMGSYSession.Current.PMGSYScheme == 4 ? objDAL.PopulateHabitationsPMGSY3DAL(PLAN_CN_ROAD_CODE, IMS_PR_ROAD_CODE) : PopulateHabitations(PLAN_CN_ROAD_CODE, IMS_PR_ROAD_CODE));

            }
        }

        /// <summary>
        /// Screen : Map Habitations to the Proposal
        /// Get The Habitations when mapping habitations to proposal
        /// Called When User Delete the habitaion , to repopulate the habitations
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetHabitationCluster()
        {
            int PLAN_CN_ROAD_CODE = 0;
            if (Request.Params[""] == "")
            {
                return Json
                (
                    new { string.Empty }
                );
            }
            else
            {
                if (Request.Params["PLAN_CN_ROAD_CODE"].Trim() != "" && Request.Params["PLAN_CN_ROAD_CODE"] != null)
                {
                    PLAN_CN_ROAD_CODE = Convert.ToInt32(Request.Params["PLAN_CN_ROAD_CODE"]);
                }

                int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);
                return Json(PopulateClusters(PLAN_CN_ROAD_CODE, IMS_PR_ROAD_CODE));

            }

        }


        /// <summary>
        /// Finalize the Habitation Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult FinalizeHabitaion(FormCollection formCollection)
        {
            HabitationViewModel habitationViewModel = new HabitationViewModel();

            habitationViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"]);

            habitationViewModel.MAST_STATE_TYPE = formCollection["MAST_STATE_TYPE"];
            habitationViewModel.MAST_IAP_DISTRICT = formCollection["MAST_IAP_DISTRICT"];
            habitationViewModel.MAST_IS_TRIBAL = formCollection["MAST_IS_TRIBAL"];

            //In Case of Mord Unlock Status IMS_LOCK_STATUS is Passed to Finalize Function
            habitationViewModel.IMS_LOCK_STATUS = formCollection["IMS_LOCK_STATUS"];
            habitationViewModel.MAST_BLOCK_IS_DESERT = formCollection["MAST_BLOCK_IS_DESERT"];
            habitationViewModel.MAST_BLOCK_SCHEDULE5 = formCollection["MAST_BLOCK_SCHEDULE5"];
            habitationViewModel.MAST_IAP_BLOCK = formCollection["MAST_IAP_BLOCK"];

            string status = objProposalBAL.FinalizeHabitationBAL(habitationViewModel);
            if (status == string.Empty)
            {
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false, errorMessage = status });
            }
        }

        /// <summary>
        /// Get The Habitations when mapping habitations to proposal
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetHabitationDetails()
        {
            if (Request.Params["selectedHabitation"] == "")
            {
                return Json(new { string.Empty });
            }
            int HabitationCode = Convert.ToInt32(Request.Params["selectedHabitation"]);

            if (HabitationCode == 0)
                return Json(new { string.Empty });

            var query = (from c in db.MASTER_HABITATIONS_DETAILS
                         where c.MAST_HAB_CODE == HabitationCode
                         select new
                         {
                             HabCode = c.MAST_HAB_CODE,
                             HabPopulation = c.MAST_HAB_TOT_POP
                         }).First();
            return Json(
            new
            {
                habcode = query.HabCode,
                habpopulation = query.HabPopulation
            });
        }

        /// <summary>
        /// Creates the Cluster of Habitation Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult CreateCluster()
        {
            string[] ClusterArray = Request.Params["CLUSTER_CODES"].Split('$');

            objProposalBAL.CreateClusterBAL(ClusterArray, Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]));

            return Json(
            new
            {
                Success = true
            });
        }

        /// <summary>
        /// Update the Cluster
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult UpdateCluster()
        {
            string[] HabitationArray = Request.Params["IMS_HAB_CODES"].Split(',');
            string[] ClusterArray = Request.Params["CLUSTER_CODES"].Split(',');

            string status = objProposalBAL.UpdateClusterBAL(HabitationArray, ClusterArray, Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]));

            if (status == string.Empty)
            {
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = status });
            }
        }

        /// <summary>
        /// UnMap the Habitation
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult UnMapHabitation()
        {
            int result = 0;
            try
            {
                result = objProposalBAL.UnMapHabitationBAL(Request.Params["IMS_PR_ROAD_CODE"], Request.Params["MAST_HAB_CODE"]);

                if (result == 0)
                {
                    return Json(new { Success = false, Message = "Habitations already connected, cannot be deleted, " });
                }
                return Json(new { Success = true, Message = "Habitation Deleted Successfully" });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UnMapHabitation()");
                return Json(new { Success = false, Message = "Error occured on Unmap Habitations" });
            }
            finally
            {

            }
        }

        /// <summary>
        /// UnMap the Habitation Cluster
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult UnMapHabitationCluster()
        {
            try
            {
                objProposalBAL.UnMapHabitationClusterBAL(Request.Params["IMS_PR_ROAD_CODE"], Request.Params["MAST_HAB_CODE"], Request.Params["MAST_CLUSTER_CODE"]);

                return Json(
                new
                {
                    Success = true
                });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UnMapHabitationCluster()");
                return Json(
                new
                {
                    Success = false
                });
            }
        }

        #endregion

        #region Traffic Intensity

        /// <summary>
        /// Populate the Applicable Years For Traffic Intensity
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        [Audit]
        public List<SelectListItem> PopulateTrafficIntensityYears(int IMS_PR_ROAD_CODE)
        {
            List<SelectListItem> AllYears = PopulateYear();

            var query = (from c in db.IMS_TRAFFIC_INTENSITY
                         where c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE
                         select new
                         {
                             Value = c.IMS_TI_YEAR
                         }).ToList();


            foreach (var data in query)
            {
                AllYears.Remove(AllYears.Where(c => c.Value == data.Value.ToString()).Single());
            }
            return AllYears;
        }

        /// <summary>
        /// Call the PopulateTrafficIntensityYears() from Traffic Intensity View
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateTrafficIntensityYears()
        {
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);
            return Json(PopulateTrafficIntensityYears(IMS_PR_ROAD_CODE));
        }

        /// <summary>
        /// Get Method for Traffic Intensity Form
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult TrafficIntensity(String parameter, String hash, String key)
        {
            int IMS_PR_ROAD_CODE = 0;
            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);

                }
            }


            ViewBag.ISM_TI_YEAR = new SelectList(PopulateTrafficIntensityYears(IMS_PR_ROAD_CODE).AsEnumerable<SelectListItem>(), "Value", "Text", DateTime.Now.Year.ToString());
            TrafficViewModel trafficViewModel = new TrafficViewModel();

            IMS_SANCTIONED_PROJECTS ims_santioned_project = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);
            trafficViewModel.Operation = "A";
            trafficViewModel.IMS_PR_ROAD_CODE = ims_santioned_project.IMS_PR_ROAD_CODE;
            trafficViewModel.DistrictName = ims_santioned_project.MASTER_DISTRICT.MAST_DISTRICT_NAME;
            trafficViewModel.BlockName = ims_santioned_project.MASTER_BLOCK.MAST_BLOCK_NAME;
            trafficViewModel.RoadCode = ims_santioned_project.PLAN_ROAD == null ? 0 : ims_santioned_project.PLAN_ROAD.PLAN_CN_ROAD_CODE;
            //trafficViewModel.RoadName = ims_santioned_project.PLAN_ROAD == null ? "NA" : ims_santioned_project.PLAN_ROAD.PLAN_RD_NAME;
            //trafficViewModel.RoadName = ims_santioned_project.IMS_ROAD_FROM + " "+  ims_santioned_project.IMS_ROAD_TO;
            trafficViewModel.RoadName = ims_santioned_project.IMS_ROAD_NAME;
            trafficViewModel.CurveType = ims_santioned_project.MASTER_TRAFFIC_TYPE.MAST_TRAFFIC_NAME;

            trafficViewModel.IMS_YEAR = ims_santioned_project.IMS_YEAR;
            trafficViewModel.IMS_BATCH = ims_santioned_project.IMS_BATCH;
            trafficViewModel.IMS_PACKAGE_ID = ims_santioned_project.IMS_PACKAGE_ID;
            trafficViewModel.IMS_ROAD_NAME = ims_santioned_project.IMS_ROAD_NAME;
            trafficViewModel.IMS_PAV_LENGTH = Convert.ToDecimal(ims_santioned_project.IMS_PAV_LENGTH);

            if (trafficViewModel.CurveType == "T1")
            {
                trafficViewModel.MinValue = 10000;
                trafficViewModel.MaxValue = 30000;
            }
            if (trafficViewModel.CurveType == "T2")
            {
                trafficViewModel.MinValue = 30000;
                trafficViewModel.MaxValue = 60000;
            }
            if (trafficViewModel.CurveType == "T3")
            {
                trafficViewModel.MinValue = 60000;
                trafficViewModel.MaxValue = 100000;
            }
            if (trafficViewModel.CurveType == "T4")
            {
                trafficViewModel.MinValue = 100000;
                trafficViewModel.MaxValue = 200000;
            }
            if (trafficViewModel.CurveType == "T5")
            {
                trafficViewModel.MinValue = 200000;
                trafficViewModel.MaxValue = 300000;
            }
            if (trafficViewModel.CurveType == "T6")
            {
                trafficViewModel.MinValue = 300000;
                trafficViewModel.MaxValue = 600000;
            }
            if (trafficViewModel.CurveType == "T7")
            {
                trafficViewModel.MinValue = 600000;
                trafficViewModel.MaxValue = 1000000;
            }
            if (trafficViewModel.CurveType == "IRC 37")
            {
                //trafficViewModel.MinValue = 1000001;
                trafficViewModel.MinValue = 2000001;
                trafficViewModel.MaxValue = 5000000;
            }

            ///ADDED BY SAMMED PATIL on 10NOV2016 as per directions from Srinivasa Sir
            if (trafficViewModel.CurveType == "T8")
            {
                trafficViewModel.MinValue = 1000000;
                trafficViewModel.MaxValue = 1500000;
            }
            if (trafficViewModel.CurveType == "T9")
            {
                trafficViewModel.MinValue = 1500000;
                trafficViewModel.MaxValue = 2000000;
            }

            trafficViewModel.IMS_TI_YEAR = DateTime.Now.Year;

            return PartialView(trafficViewModel);
        }

        /// <summary>
        /// Saves the Data of Traffic Intensity
        /// </summary>
        /// <param name="traffic_intensity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public JsonResult TrafficIntensity(TrafficViewModel traffic_intensity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objProposalBAL.SaveTrafficIntesityBAL(traffic_intensity);
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TrafficIntensity(TrafficViewModel traffic_intensity)");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }

        /// <summary>
        /// Populate the List of Traffic Intensity List 
        /// Jqgrid 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetTrafficIntensityList(FormCollection formCollection)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;

            var jsonData = new
            {
                rows = objProposalBAL.GetTrafficListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Update the Traffic Intensity Details
        /// </summary>
        /// <param name="trafficIntensity"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult UpdateTrafficIntensity(TrafficViewModel trafficIntensity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objProposalBAL.UpdateTrafficIntesityBAL(trafficIntensity);
                    return Json(new { Success = true });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateTrafficIntensity(TrafficViewModel trafficIntensity)");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }


        }

        /// <summary>
        /// Delete the Traffic Intensity Details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult DeleteTrafficIntensity()
        {
            TrafficViewModel trafficIntensity = new TrafficViewModel();
            trafficIntensity.IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);
            trafficIntensity.IMS_TI_YEAR = Convert.ToInt32(Request.Params["IMS_TI_YEAR"]);
            objProposalBAL.DeleteTrafficIntensityDetailsBAL(trafficIntensity);
            return Json(new { Success = true });
        }

        /// <summary>
        /// Edit the Traffic Intensity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditTrafficIntensity(string id)
        {
            string[] parameters = id.Split('$');

            int IMS_PR_ROAD_CODE = Convert.ToInt32(parameters[0]);
            int IMS_TI_YEAR = Convert.ToInt32(parameters[1]);

            IMS_TRAFFIC_INTENSITY ims_traffic_intensity = db.IMS_TRAFFIC_INTENSITY.Find(IMS_PR_ROAD_CODE, IMS_TI_YEAR);
            TrafficViewModel trafficViewModel = new TrafficViewModel();

            IMS_SANCTIONED_PROJECTS ims_santioned_project = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);
            trafficViewModel.Operation = "U";
            //trafficViewModel.DistrictName = ims_santioned_project.MASTER_DISTRICT.MAST_DISTRICT_NAME;
            //trafficViewModel.BlockName = ims_santioned_project.MASTER_BLOCK.MAST_BLOCK_NAME;
            //trafficViewModel.RoadCode = ims_santioned_project.PLAN_ROAD.PLAN_CN_ROAD_CODE;
            trafficViewModel.RoadName = ims_santioned_project.PLAN_ROAD.PLAN_RD_NAME;
            trafficViewModel.CurveType = ims_santioned_project.MASTER_TRAFFIC_TYPE.MAST_TRAFFIC_NAME;



            trafficViewModel.IMS_PR_ROAD_CODE = ims_traffic_intensity.IMS_PR_ROAD_CODE;
            trafficViewModel.IMS_TI_YEAR = ims_traffic_intensity.IMS_TI_YEAR;
            trafficViewModel.IMS_TOTAL_TI = ims_traffic_intensity.IMS_TOTAL_TI;
            trafficViewModel.IMS_COMM_TI = ims_traffic_intensity.IMS_COMM_TI;


            trafficViewModel.IMS_YEAR = ims_santioned_project.IMS_YEAR;
            trafficViewModel.IMS_BATCH = ims_santioned_project.IMS_BATCH;
            trafficViewModel.IMS_PACKAGE_ID = ims_santioned_project.IMS_PACKAGE_ID;
            trafficViewModel.IMS_ROAD_NAME = ims_santioned_project.IMS_ROAD_NAME;
            trafficViewModel.IMS_PAV_LENGTH = Convert.ToDecimal(ims_santioned_project.IMS_PAV_LENGTH);

            List<SelectListItem> SpecificYear = new List<SelectListItem>();
            SelectListItem year = new SelectListItem();
            year.Text = IMS_TI_YEAR.ToString() + " - " + (IMS_TI_YEAR + 1).ToString();
            year.Value = IMS_TI_YEAR.ToString();
            SpecificYear.Add(year);

            ViewBag.ISM_TI_YEAR = new SelectList(SpecificYear.AsEnumerable<SelectListItem>(), "Value", "Text", trafficViewModel.IMS_TI_YEAR);

            if (trafficViewModel.CurveType == "T1")
            {
                trafficViewModel.MinValue = 10000;
                trafficViewModel.MaxValue = 30000;
            }
            if (trafficViewModel.CurveType == "T2")
            {
                trafficViewModel.MinValue = 30000;
                trafficViewModel.MaxValue = 60000;
            }
            if (trafficViewModel.CurveType == "T3")
            {
                trafficViewModel.MinValue = 60000;
                trafficViewModel.MaxValue = 100000;
            }
            if (trafficViewModel.CurveType == "T4")
            {
                trafficViewModel.MinValue = 100000;
                trafficViewModel.MaxValue = 200000;
            }
            if (trafficViewModel.CurveType == "T5")
            {
                trafficViewModel.MinValue = 200000;
                trafficViewModel.MaxValue = 300000;
            }
            if (trafficViewModel.CurveType == "T6")
            {
                trafficViewModel.MinValue = 300000;
                trafficViewModel.MaxValue = 600000;
            }
            if (trafficViewModel.CurveType == "T7")
            {
                trafficViewModel.MinValue = 600000;
                trafficViewModel.MaxValue = 1000000;
            }
            if (trafficViewModel.CurveType == "IRC 37")
            {
                trafficViewModel.MinValue = 1000000;
                trafficViewModel.MaxValue = 5000000;
            }

            return PartialView("TrafficIntensity", trafficViewModel);
        }
        #endregion

        #region CBRValue
        /// <summary>
        /// Get the CBR Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult CBRValue(String parameter, String hash, String key)
        {
            int IMS_PR_ROAD_CODE = 0;
            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                }
            }


            IMS_SANCTIONED_PROJECTS ims_santioned_project = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);

            CBRViewModel cbr_view = new CBRViewModel();
            cbr_view.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;
            cbr_view.Operation = "A";
            cbr_view.PackageID = ims_santioned_project.IMS_PACKAGE_ID;
            cbr_view.DistrictName = ims_santioned_project.MASTER_DISTRICT.MAST_DISTRICT_NAME;
            cbr_view.BlockName = ims_santioned_project.MASTER_BLOCK.MAST_BLOCK_NAME;
            cbr_view.RoadID = ims_santioned_project.PLAN_ROAD == null ? "0" : ims_santioned_project.PLAN_ROAD.PLAN_CN_ROAD_NUMBER;
            cbr_view.RoadLength = ims_santioned_project.PLAN_ROAD != null ? Convert.ToDecimal(ims_santioned_project.PLAN_ROAD.PLAN_RD_LENGTH) : 0;

            cbr_view.RoadName = ims_santioned_project.PLAN_ROAD == null ? "NA" : ims_santioned_project.PLAN_ROAD.PLAN_RD_NAME;
            cbr_view.IMS_PAV_LENGTH = Convert.ToDecimal(ims_santioned_project.IMS_PAV_LENGTH);

            cbr_view.IMS_YEAR = ims_santioned_project.IMS_YEAR;
            cbr_view.IMS_BATCH = ims_santioned_project.IMS_BATCH;
            cbr_view.IMS_PACKAGE_ID = ims_santioned_project.IMS_PACKAGE_ID;
            cbr_view.IMS_ROAD_NAME = ims_santioned_project.IMS_ROAD_NAME;

            if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == ims_santioned_project.IMS_PR_ROAD_CODE).Any())
            {
                cbr_view.CBRLenghEntered = db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == ims_santioned_project.IMS_PR_ROAD_CODE).AsEnumerable().Sum(c => c.IMS_END_CHAIN - c.IMS_STR_CHAIN);
                cbr_view.IMS_STR_CHAIN = db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == ims_santioned_project.IMS_PR_ROAD_CODE).Max(c => c.IMS_END_CHAIN);
            }
            else
            {
                cbr_view.CBRLenghEntered = 0;
                cbr_view.IMS_STR_CHAIN = 0;
            }

            cbr_view.Remaining_Length = ims_santioned_project.IMS_PAV_LENGTH - cbr_view.CBRLenghEntered;

            return PartialView("CBRValue", cbr_view);
        }

        /// <summary>
        /// Save the CBR Value and Get the Remaining Length
        /// </summary>
        /// <param name="CBRModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult CBRValue(CBRViewModel CBRModel)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    string status = objProposalBAL.SaveCBRValueBAL(CBRModel);
                    if (status == string.Empty)
                    {
                        if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).Any())
                            CBRModel.CBRLenghEntered = db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).AsEnumerable().Sum(c => c.IMS_END_CHAIN - c.IMS_STR_CHAIN);

                        CBRModel.Remaining_Length = db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).Select(c => c.IMS_PAV_LENGTH).First() - CBRModel.CBRLenghEntered;

                        return Json(new { Success = true, RemainingLength = CBRModel.Remaining_Length, Start_Chainage = CBRModel.IMS_END_CHAIN });
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = status });
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CBRValue(CBRViewModel CBRModel)");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }

        /// <summary>
        /// List the CBR Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetCBRList(FormCollection formCollection)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;
            var jsonData = new
            {
                rows = objProposalBAL.GetCBRListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
                // userdata = new { SegmentLength = "<span sty  'jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' value='Update Clusters'></input>" }
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Edit the CBR Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditCBRValue(string id)
        {
            string[] parameters = id.Split('$');

            int IMS_PR_ROAD_CODE = Convert.ToInt32(parameters[0]);
            int IMS_SEGMENT_NO = Convert.ToInt32(parameters[1]);

            IMS_CBR_VALUE ims_cbr_value = db.IMS_CBR_VALUE.Find(IMS_PR_ROAD_CODE, IMS_SEGMENT_NO);
            CBRViewModel cbrModel = new CBRViewModel();
            cbrModel.Operation = "U";

            cbrModel.IMS_PR_ROAD_CODE = ims_cbr_value.IMS_PR_ROAD_CODE;
            cbrModel.IMS_SEGMENT_NO = ims_cbr_value.IMS_SEGMENT_NO;
            cbrModel.IMS_STR_CHAIN = ims_cbr_value.IMS_STR_CHAIN;
            cbrModel.IMS_END_CHAIN = ims_cbr_value.IMS_END_CHAIN;
            cbrModel.Segment_Length = ims_cbr_value.IMS_END_CHAIN - ims_cbr_value.IMS_STR_CHAIN;
            cbrModel.IMS_CBR_VALUE1 = ims_cbr_value.IMS_CBR_VALUE1;
            cbrModel.IMS_PAV_LENGTH = db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(c => c.IMS_PAV_LENGTH).First();

            IMS_SANCTIONED_PROJECTS ims_santioned_project = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);
            cbrModel.IMS_YEAR = ims_santioned_project.IMS_YEAR;
            cbrModel.IMS_BATCH = ims_santioned_project.IMS_BATCH;
            cbrModel.IMS_PACKAGE_ID = ims_santioned_project.IMS_PACKAGE_ID;
            cbrModel.IMS_ROAD_NAME = ims_santioned_project.IMS_ROAD_NAME;

            var query = db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(c => c.PLAN_ROAD.PLAN_RD_LENGTH).First();
            cbrModel.RoadLength = query != null ? Convert.ToDecimal(query) : 0;
            if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == ims_cbr_value.IMS_PR_ROAD_CODE).Any())
                cbrModel.CBRLenghEntered = db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == ims_cbr_value.IMS_PR_ROAD_CODE).AsEnumerable().Sum(c => c.IMS_END_CHAIN - c.IMS_STR_CHAIN);

            cbrModel.Remaining_Length = cbrModel.IMS_PAV_LENGTH - cbrModel.CBRLenghEntered;

            return PartialView("CBRValue", cbrModel);
        }

        /// <summary>
        /// Update the CBR Details
        /// </summary>
        /// <param name="CBRModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult UpdateCBRValue(CBRViewModel CBRModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string status = objProposalBAL.UpdateCBRValueBAL(CBRModel);

                    if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).Any())
                        CBRModel.CBRLenghEntered = db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).AsEnumerable().Sum(c => c.IMS_END_CHAIN - c.IMS_STR_CHAIN);

                    CBRModel.Remaining_Length = db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).Select(c => c.IMS_PAV_LENGTH).First() - CBRModel.CBRLenghEntered;

                    if (status == string.Empty)
                    {
                        return Json(new { Success = true, RemainingLength = CBRModel.Remaining_Length, Start_Chainage = CBRModel.IMS_END_CHAIN });
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = status });
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateCBRValue(CBRViewModel CBRModel)");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }

        /// <summary>
        /// Delete the CBR Details
        /// </summary>
        /// <param name="CBRModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult DeleteCBRValue(CBRViewModel CBRModel)
        {
            string status = objProposalBAL.DeleteCBRValueBAL(CBRModel);
            if (status == string.Empty)
            {
                if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).Any())
                    CBRModel.CBRLenghEntered = db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).AsEnumerable().Sum(c => c.IMS_END_CHAIN - c.IMS_STR_CHAIN);

                CBRModel.Remaining_Length = db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).Select(c => c.IMS_PAV_LENGTH).First() - CBRModel.CBRLenghEntered;
                CBRModel.IMS_STR_CHAIN = db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).Any() ? db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).Select(c => c.IMS_END_CHAIN).Max() : 0;

                return Json(new { Success = true, RemainingLength = CBRModel.Remaining_Length, IMS_STR_CHAIN = CBRModel.IMS_STR_CHAIN });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = status });
            }
        }

        #endregion

        #region FileUpload

        /// <summary>
        /// Lists the Files
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult ListFiles(FormCollection formCollection)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;
            var jsonData = new
            {
                rows = objProposalBAL.GetFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult ListPDFFiles(FormCollection formCollection)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;

            var jsonData = new
            {
                rows = objProposalBAL.GetPDFFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult ListSTASRRDAPDFFiles(FormCollection formCollection)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;

            var jsonData = new
            {
                rows = objProposalBAL.GetSTASRRDAPDFFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }


        /// <summary>
        /// Get Main File Upload View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult FileUpload(string id)
        {
            string[] arr = id.Split('$');
            if (arr.Length > 0)
            {
                FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
                fileUploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(arr[0]);

                IMS_SANCTIONED_PROJECTS ims_sanctioned_project = db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE).First();
                fileUploadViewModel.IMS_YEAR = ims_sanctioned_project.IMS_YEAR;
                fileUploadViewModel.IMS_BATCH = ims_sanctioned_project.IMS_BATCH;
                fileUploadViewModel.IMS_PACKAGE_ID = ims_sanctioned_project.IMS_PACKAGE_ID;
                fileUploadViewModel.IMS_ROAD_NAME = ims_sanctioned_project.IMS_ROAD_NAME;
                fileUploadViewModel.IMS_PAV_LENGTH = Convert.ToDecimal(ims_sanctioned_project.IMS_PAV_LENGTH);

                //if (arr.Length > 1)
                //{
                //    fileUploadViewModel.ISPFTYPE = arr[1];
                //}

                return View("FileUpload", fileUploadViewModel);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///  Get the Image Upload View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult ImageUpload(string id)
        {
            FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
            fileUploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
            if (db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "I").Any())
            {
                fileUploadViewModel.NumberofImages = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "I").Count();
            }
            else
            {
                fileUploadViewModel.NumberofImages = 0;
            }
            fileUploadViewModel.ErrorMessage = string.Empty;
            return View(fileUploadViewModel);
        }

        /// <summary>
        /// Get the PDF File Upload View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult PdfFileUpload(string id)
        {
            FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
            fileUploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
            fileUploadViewModel.ErrorMessage = string.Empty;
            if (db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "C").Any())
            {
                if (PMGSYSession.Current.RoleCode == 2)
                {
                    fileUploadViewModel.NumberofPdfs = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "C" && a.ISPF_UPLOAD_BY == "S").Count();
                }
                else if (PMGSYSession.Current.RoleCode == 3)
                {
                    fileUploadViewModel.NumberofPdfs = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "C" && a.ISPF_UPLOAD_BY == "T").Count();
                }
                else if (PMGSYSession.Current.RoleCode == 17)
                {
                    fileUploadViewModel.NumberofPdfs = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "C" && a.ISPF_UPLOAD_BY == "P").Count();
                }
                else
                {
                    fileUploadViewModel.NumberofPdfs = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "C" && a.ISPF_UPLOAD_BY == "D").Count();
                }
            }
            else
            {
                fileUploadViewModel.NumberofPdfs = 0;
            }
            return View(fileUploadViewModel);
        }

        /// <summary>
        /// Post Method for Uploading IMAGE File
        /// </summary>
        /// <param name="fileUploadViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [Audit]
        public ActionResult Uploads(FileUploadViewModel fileUploadViewModel)
        {
            CommonFunctions objCommonFunc = new CommonFunctions();
            //Array of File Types to Validate             
            String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
            if (!(objCommonFunc.IsValidImageFile(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYII"] : PMGSYSession.Current.PMGSYScheme == 5 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSY_VV"] : ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYIII"], Request, fileTypes)))
            {
                fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                return View("ImageUpload", fileUploadViewModel.ErrorMessage);
            }

            foreach (string file in Request.Files)
            {
                string status = objProposalBAL.ValidateImageFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                if (status != string.Empty)
                {
                    fileUploadViewModel.ErrorMessage = status;
                    return View("ImageUpload", fileUploadViewModel.ErrorMessage);
                }
            }

            var fileData = new List<FileUploadViewModel>();

            int IMS_PR_ROAD_CODE = 0;
            if (fileUploadViewModel.IMS_PR_ROAD_CODE != 0)
            {
                IMS_PR_ROAD_CODE = fileUploadViewModel.IMS_PR_ROAD_CODE;
            }
            else
            {
                try
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
                }
                catch
                {
                    if (Request["IMS_PR_ROAD_CODE"].Contains(','))
                    {
                        IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"].Split(',')[0]);
                    }
                }
            }
            foreach (string file in Request.Files)
            {
                UploadImageFile(Request, fileData, IMS_PR_ROAD_CODE);
            }
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        /// <summary>
        /// Uploads the Image File
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        public void UploadImageFile(HttpRequestBase request, List<FileUploadViewModel> statuses, int IMS_PR_ROAD_CODE)
        {
            String StorageRoot = PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYII"] : PMGSYSession.Current.PMGSYScheme == 5 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSY_VV"] : ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYIII"];
            int MaxCount = 0;
            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFileBase file = request.Files[i];
                var fileId = IMS_PR_ROAD_CODE;
                if (db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                {
                    //Commented Max Count Logic By Shyam, it fails when user deletes intermediate file.
                    //MaxCount = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count(); 

                    //Take Max File Id respective to IMS_PR_ROAD_CODE
                    MaxCount = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(a => a.IMS_FILE_ID).Max();
                }
                MaxCount++;

                var fileName = IMS_PR_ROAD_CODE + "-" + MaxCount + Path.GetExtension(request.Files[i].FileName).ToString();
                var fullPath = Path.Combine(StorageRoot, fileName);

                var ThumbnailPath = Path.Combine(StorageRoot, "thumbnails");
                var FullThumbnailPath = Path.Combine(ThumbnailPath, fileName);

                statuses.Add(new FileUploadViewModel()
                {
                    url = fullPath,
                    thumbnail_url = fullPath,
                    name = fileName,
                    type = file.ContentType,
                    size = file.ContentLength,
                    delete_url = "",
                    delete_type = "DELETE",

                    chainage = Convert.ToDecimal(request.Params["chainageValue[]"]),
                    Image_Description = request.Params["remark[]"],

                    IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE
                });

                string status = objProposalBAL.AddFileUploadDetailsBAL(statuses, "I");
                if (status == string.Empty)
                {
                    objProposalBAL.CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                    //file.SaveAs(fullPath);
                }
                else
                {
                    // show an error over here
                }
            }
        }


        /// <summary>
        /// Post Method for Uploading PDF File
        /// </summary>
        /// <param name="fileUploadViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult PdfFileUpload(FileUploadViewModel fileUploadViewModel)
        {
            CommonFunctions objCommonFunc = new CommonFunctions();
            if (!(objCommonFunc.ValidateIsPdf(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYII"] : PMGSYSession.Current.PMGSYScheme == 5 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSY_VV"] : ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYIII"], Request)))
            {
                fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                return View("PdfFileUpload", fileUploadViewModel.ErrorMessage);
            }


            foreach (string file in Request.Files)
            {
                string status = objProposalBAL.ValidatePDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                if (status != string.Empty)
                {
                    fileUploadViewModel.ErrorMessage = status;
                    return View("PdfFileUpload", fileUploadViewModel.ErrorMessage);
                }
            }

            var fileData = new List<FileUploadViewModel>();

            int IMS_PR_ROAD_CODE = 0;
            if (fileUploadViewModel.IMS_PR_ROAD_CODE != 0)
            {
                IMS_PR_ROAD_CODE = fileUploadViewModel.IMS_PR_ROAD_CODE;
            }
            else
            {
                try
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
                }
                catch
                {
                    if (Request["IMS_PR_ROAD_CODE"].Contains(','))
                    {
                        IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"].Split(',')[0]);
                    }
                }
            }
            foreach (string file in Request.Files)
            {
                UploadPDFFile(Request, fileData, IMS_PR_ROAD_CODE);
            }

            if (db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "C").Any())
            {
                fileUploadViewModel.NumberofPdfs = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "C").Count();
            }
            else
            {
                fileUploadViewModel.NumberofPdfs = 0;
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        /// <summary>
        /// This Method Uploads PDF File
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        [Audit]
        public void UploadPDFFile(HttpRequestBase request, List<FileUploadViewModel> statuses, int IMS_PR_ROAD_CODE)
        {
            String StorageRoot = PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYII"] : PMGSYSession.Current.PMGSYScheme == 5 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSY_VV"] : ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYIII"];
            int MaxCount = 0;

            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFileBase file = request.Files[i];
                var fileId = IMS_PR_ROAD_CODE;
                if (db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                {
                    MaxCount = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count();
                }
                MaxCount++;

                var fileName = IMS_PR_ROAD_CODE + "-" + MaxCount + Path.GetExtension(request.Files[i].FileName).ToString();
                var fullPath = Path.Combine(StorageRoot, fileName);

                statuses.Add(new FileUploadViewModel()
                {
                    url = fullPath,
                    thumbnail_url = fullPath,
                    name = fileName,
                    type = file.ContentType,
                    size = file.ContentLength,
                    delete_url = "",
                    delete_type = "DELETE",

                    PdfDescription = request.Params["PdfDescription[]"],

                    IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE
                });

                string status = objProposalBAL.AddFileUploadDetailsBAL(statuses, "C");
                if (status == string.Empty)
                {
                    //file.SaveAs(fullPath);
                    file.SaveAs(Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYII"] : PMGSYSession.Current.PMGSYScheme == 5 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSY_VV"] : ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYIII"], fileName));
                }
                else
                {
                    // show an error over here
                }
            }
        }

        #region Joint Inspection

        /// <summary>
        /// Get the PDF File Upload View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult JIFileUpload(string id)
        {
            FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
            fileUploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
            fileUploadViewModel.ErrorMessage = string.Empty;
            if (db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "J").Any())
            {
                //if (PMGSYSession.Current.RoleCode == 2)
                //{
                //    fileUploadViewModel.NumberofPdfs = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "C" && a.ISPF_UPLOAD_BY == "S").Count();
                //}
                //else if (PMGSYSession.Current.RoleCode == 3)
                //{
                //    fileUploadViewModel.NumberofPdfs = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "C" && a.ISPF_UPLOAD_BY == "T").Count();
                //}
                //else if (PMGSYSession.Current.RoleCode == 17)
                //{
                //    fileUploadViewModel.NumberofPdfs = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "C" && a.ISPF_UPLOAD_BY == "P").Count();
                //}
                //else
                //{
                //    fileUploadViewModel.NumberofPdfs = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "C" && a.ISPF_UPLOAD_BY == "D").Count();
                //}
                fileUploadViewModel.NumberofPdfs = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "J").Count();
            }
            else
            {
                fileUploadViewModel.NumberofPdfs = 0;
            }
            return View(fileUploadViewModel);
        }

        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult ListJIFiles(FormCollection formCollection)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;

            var jsonData = new
            {
                rows = objProposalBAL.GetJIFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Post Method for Uploading PDF File
        /// </summary>
        /// <param name="fileUploadViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult JIFileUpload(FileUploadViewModel fileUploadViewModel)
        {
            CommonFunctions objCommonFunc = new CommonFunctions();
            if (!(objCommonFunc.ValidateIsPdf(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_PMGSYII"] : ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_PMGSYIII"], Request)))
            {
                fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                return View("JIFileUpload", fileUploadViewModel.ErrorMessage);
            }


            foreach (string file in Request.Files)
            {
                string status = objProposalBAL.ValidatePDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                if (status != string.Empty)
                {
                    fileUploadViewModel.ErrorMessage = status;
                    return View("PdfFileUpload", fileUploadViewModel.ErrorMessage);
                }
            }

            var fileData = new List<FileUploadViewModel>();

            int IMS_PR_ROAD_CODE = 0;
            if (fileUploadViewModel.IMS_PR_ROAD_CODE != 0)
            {
                IMS_PR_ROAD_CODE = fileUploadViewModel.IMS_PR_ROAD_CODE;
            }
            else
            {
                try
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
                }
                catch
                {
                    if (Request["IMS_PR_ROAD_CODE"].Contains(','))
                    {
                        IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"].Split(',')[0]);
                    }
                }
            }
            foreach (string file in Request.Files)
            {
                UploadJIFile(Request, fileData, IMS_PR_ROAD_CODE);
            }

            if (db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "J").Any())
            {
                fileUploadViewModel.NumberofPdfs = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "J").Count();
            }
            else
            {
                fileUploadViewModel.NumberofPdfs = 0;
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        /// <summary>
        /// This Method Uploads PDF File
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        [Audit]
        public void UploadJIFile(HttpRequestBase request, List<FileUploadViewModel> statuses, int IMS_PR_ROAD_CODE)
        {
            String StorageRoot = PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_PMGSYII"] : ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_PMGSYIII"];
            int MaxCount = 0;

            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFileBase file = request.Files[i];
                var fileId = IMS_PR_ROAD_CODE;
                if (db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                {
                    MaxCount = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count();
                }
                MaxCount++;

                var fileName = IMS_PR_ROAD_CODE + "-" + MaxCount + Path.GetExtension(request.Files[i].FileName).ToString();
                var fullPath = Path.Combine(StorageRoot, fileName);

                statuses.Add(new FileUploadViewModel()
                {
                    url = fullPath,
                    thumbnail_url = fullPath,
                    name = fileName,
                    type = file.ContentType,
                    size = file.ContentLength,
                    delete_url = "",
                    delete_type = "DELETE",

                    PdfDescription = request.Params["PdfDescription[]"],

                    IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE
                });

                string status = objProposalBAL.AddFileUploadDetailsBAL(statuses, "J");
                if (status == string.Empty)
                {
                    //file.SaveAs(fullPath);
                    file.SaveAs(Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_PMGSYII"] : ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_PMGSYIII"], fileName));
                }
                else
                {
                    // show an error over here
                }
            }
        }
        #endregion

        /// <summary>
        /// Downloads the File
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult DownloadFile(String parameter, String hash, String key)
        {
            try
            {
                string FileName = string.Empty;
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

                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".pdf")
                {
                    ///PMGSY3 changes
                    FullFileLogicalPath = Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH_PMGSYII"] : PMGSYSession.Current.PMGSYScheme == 5 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH_PMGSY_VV"] : ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH_PMGSYIII"], FileName);
                    FullfilePhysicalPath = Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYII"] : PMGSYSession.Current.PMGSYScheme == 5 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSY_VV"] : ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYIII"], FileName);
                }
                else if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {
                    FullFileLogicalPath = Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_VIRTUAL_DIR_PATH_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_VIRTUAL_DIR_PATH_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_VIRTUAL_DIR_PATH_PMGSYII"] : PMGSYSession.Current.PMGSYScheme == 5 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_VIRTUAL_DIR_PATH_VV"] : ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_VIRTUAL_DIR_PATH_PMGSYIII"], FileName);
                    FullfilePhysicalPath = Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYII"] : PMGSYSession.Current.PMGSYScheme == 5 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSY_VV"] : ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYIII"], FileName);
                }

                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DownloadFile()");
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Update the Image File Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult UpdateImageDetails(FormCollection formCollection)
        {
            string[] arrKey = formCollection["id"].Split('$');
            FileUploadViewModel fileuploadViewModel = new FileUploadViewModel();
            fileuploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(arrKey[1]);
            fileuploadViewModel.IMS_FILE_ID = Convert.ToInt32(arrKey[0]);
            try
            {
                Regex regexChainage = new Regex((@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$"));
                if (regexChainage.IsMatch(formCollection["chainage"]) && formCollection["chainage"].Trim().Length != 0)
                {
                    fileuploadViewModel.chainage = Convert.ToDecimal(formCollection["chainage"]);
                }
                else
                {
                    return Json("Invalid chainage,Can only contains Numeric values and Total 7 Digits and 3 digits after decimal place");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateImageDetails()");
                return Json("Please Enter Valid Chainage.");
            }
            Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");

            if (regex.IsMatch(formCollection["Description"]) && formCollection["Description"].Trim().Length != 0)
            {
                fileuploadViewModel.Image_Description = formCollection["Description"];
            }
            else
            {
                return Json("Invalid Image Description, Only Alphabets, Numbers and  [,.()-] value are allowed");
            }

            string status = objProposalBAL.UpdateImageDetailsBAL(fileuploadViewModel);

            if (status == string.Empty)
                return Json(true);
            else
                return Json("There is an error occured while processing your request.");
        }

        /// <summary>
        /// Update the PDF File Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult UpdatePDFDetails(FormCollection formCollection)
        {
            string[] arrKey = formCollection["id"].Split('$');
            FileUploadViewModel fileuploadViewModel = new FileUploadViewModel();
            fileuploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(arrKey[1]);
            fileuploadViewModel.IMS_FILE_ID = Convert.ToInt32(arrKey[0]);

            Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");
            if (regex.IsMatch(formCollection["Description"]) && formCollection["Description"].Trim().Length != 0)
            {
                fileuploadViewModel.PdfDescription = formCollection["Description"];
            }
            else
            {
                return Json("Invalid PDF Description, Only Alphabets,Numbers and [,.()-] are allowed");
            }

            string status = objProposalBAL.UpdatePDFDetailsBAL(fileuploadViewModel);

            if (status == string.Empty)
                return Json(true);
            else
                return Json("There is an error occured while processing your request.");
        }

        /// <summary>
        /// Update the PDF File Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult UpdateJIDetails(FormCollection formCollection)
        {
            string[] arrKey = formCollection["id"].Split('$');
            FileUploadViewModel fileuploadViewModel = new FileUploadViewModel();
            fileuploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(arrKey[1]);
            fileuploadViewModel.IMS_FILE_ID = Convert.ToInt32(arrKey[0]);

            Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");
            if (regex.IsMatch(formCollection["Description"]) && formCollection["Description"].Trim().Length != 0)
            {
                fileuploadViewModel.PdfDescription = formCollection["Description"];
            }
            else
            {
                return Json("Invalid PDF Description, Only Alphabets,Numbers and [,.()-] are allowed");
            }

            string status = objProposalBAL.UpdateJIDetailsBAL(fileuploadViewModel);

            if (status == string.Empty)
                return Json(true);
            else
                return Json("There is an error occured while processing your request.");
        }

        /// <summary>
        /// Delete File and File Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult DeleteFileDetails(string id)
        {
            string[] arrParam = null;
            try
            {
                String PhysicalPath = string.Empty;
                String ThumbnailPath = string.Empty;
                string IMS_FILE_NAME = Request.Params["IMS_FILE_NAME"];

                //if (IMS_FILE_NAME.Contains('_'))
                //{
                //    IMS_FILE_NAME = IMS_FILE_NAME.Replace('_', ' ');
                //}

                if (Request.Params["ISPF_TYPE"].ToUpper() == "I")
                {
                    ///PMGSY3 changes
                    PhysicalPath = PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_RCPLWE"]
                        : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYII"] : ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYIII"];
                    ThumbnailPath = Path.Combine(Path.Combine(PhysicalPath, "thumbnails"), IMS_FILE_NAME);
                }
                else if (Request.Params["ISPF_TYPE"].ToUpper() == "C" || Request.Params["ISPF_TYPE"].ToUpper() == "P")//STA PMGSY3
                {
                    PhysicalPath = PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYII"] : ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_PMGSYIII"];
                }
                else if (Request.Params["ISPF_TYPE"].ToUpper() == "J")
                {
                    PhysicalPath = PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_PMGSYII"] : ConfigurationManager.AppSettings["PROPOSAL_JI_FILE_UPLOAD_PMGSYIII"];
                }

                arrParam = Request.Params["IMS_PR_ROAD_CODE"].Split('$');

                int IMS_FILE_ID = Convert.ToInt32(arrParam[0]);
                int IMS_PR_ROAD_CODE = Convert.ToInt32(arrParam[1]);

                PhysicalPath = Path.Combine(PhysicalPath, IMS_FILE_NAME);

                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    //return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }

                string status = objProposalBAL.DeleteFileDetails(IMS_FILE_ID, IMS_PR_ROAD_CODE, IMS_FILE_NAME, Request.Params["ISPF_TYPE"]);

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "There is an error while processing your request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                //Exception unhandledException = Server.GetLastError();
                //HttpException httpException = unhandledException as HttpException;
                //Exception innerException = unhandledException.InnerException;

                ErrorLog.LogError(ex, "DeleteFileDetails()");
                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
        }

        #endregion

        #region DropdownPopulation

        /// <summary>
        /// Used When User Edits The Proposal
        /// </summary>
        /// <param name="IMS_CN_ROAD_CODE"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateOnlyLinkThrough(int IMS_CN_ROAD_CODE, int BlockCode, String IMS_UPGRADE_CONNECT)
        {
            List<SelectListItem> lstLinkThrough = new List<SelectListItem>();

            List<SelectListItem> lstCoreNetwork = PopulateLinkThrough(BlockCode, IMS_UPGRADE_CONNECT, "P");

            SelectListItem item = new SelectListItem();

            foreach (SelectListItem listItem in lstCoreNetwork)
            {
                lstLinkThrough.Add(listItem);
            }


            var sp = (from c in db.PLAN_ROAD
                      where c.PLAN_CN_ROAD_CODE == IMS_CN_ROAD_CODE

                      select new
                      {
                          Value = c.PLAN_CN_ROAD_CODE,
                          Text = c.PLAN_CN_ROAD_NUMBER + "-" + c.PLAN_RD_NAME

                      }).ToList();

            foreach (var data in sp)
            {
                item = new SelectListItem();
                item.Text = data.Text;
                item.Value = data.Value.ToString();

                //if (!lstLinkThrough.Contains(item))
                if (!lstLinkThrough.Where(a => a.Value == item.Value).Any())
                {
                    lstLinkThrough.Add(item);
                }
            }

            return lstLinkThrough;
        }

        /// <summary>
        /// Populates the Status of Propsoal for Filtering
        /// </summary>
        /// <param name="RollID"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateProposalStatus(int RollID)
        {
            // STA & PTALogin Status
            if (RollID == 3 || RollID == 15)
            {
                List<SelectListItem> lstProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();
                item.Text = "Pending Proposals";
                item.Value = "N";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Scrutinized Proposals";
                item.Value = "Y";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Un-Scrutinized Proposals";
                item.Value = "U";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "A";
                lstProposalStatus.Add(item);

                return lstProposalStatus;
            }
            /// MoRD Login Status
            else if (RollID == 25 || RollID == 65)//Changes by SAMMED A. PATIL for Mord View
            {
                List<SelectListItem> lstProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Pending Proposals";
                item.Value = "N";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Sanctioned Proposals";
                item.Value = "Y";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Un-Sanctioned Proposals";
                item.Value = "U";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Recommended Proposals";
                item.Value = "R";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Dropped Propsoal";
                item.Value = "D";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA Pending";
                item.Value = "S";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "A";
                lstProposalStatus.Add(item);

                return lstProposalStatus;
            }
            else if (RollID == 2 || RollID == 37 || RollID == 55)
            {
                List<SelectListItem> lstProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "00";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PTA Scrutinized";
                item.Value = "PY";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PTA Pending";
                item.Value = "PN";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PTA UnScrutinized";
                item.Value = "PU";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA Scrutinized";
                item.Value = "SY";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA Pending";
                item.Value = "SN";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA UnScrutinized";
                item.Value = "SU";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "MoRD Pending";
                item.Value = "MN";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "MoRD Sanctioned Proposals";
                item.Value = "MY";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "MoRD Un-Sanctioned Proposals";
                item.Value = "MU";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Recommended Proposals";
                item.Value = "MR";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Dropped Proposals";
                item.Value = "MD";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PIU Pending";
                item.Value = "DE";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Habitation Finalized";
                item.Value = "DH";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PIU Finalize";
                item.Value = "DD";
                lstProposalStatus.Add(item);



                return lstProposalStatus;
            }
            else if (RollID == 22 || RollID == 38 || RollID == 54)
            {
                List<SelectListItem> lstProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "00";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PTA Srutinized";
                item.Value = "PY";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PTA Pending";
                item.Value = "PN";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PTA UnScrutinized";
                item.Value = "PU";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA Srutinized";
                item.Value = "SY";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA Pending";
                item.Value = "SN";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA UnScrutinized";
                item.Value = "SU";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "MoRD Pending";
                item.Value = "MN";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "MoRD Sanctioned Proposals";
                item.Value = "MY";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "MoRD Un-Sanctioned Proposals";
                item.Value = "MU";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Recommended Proposals";
                item.Value = "MR";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Dropped Proposals";
                item.Value = "MD";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PIU Entry";
                item.Value = "DE";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Habitation Finalized";
                item.Value = "DH";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PIU Finalized";
                item.Value = "DD";
                lstProposalStatus.Add(item);



                return lstProposalStatus;
            }
            else
            {
                List<SelectListItem> lstProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item.Text = "Select Status";
                item.Value = "0";
                item.Selected = true;
                lstProposalStatus.Add(item);

                return lstProposalStatus;
            }
        }

        /// <summary>
        /// populate the year 
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateYear(int SelectedYear = 0, bool populateFirstItem = true, bool isAllYearsSelected = false)
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            if (populateFirstItem && isAllYearsSelected == false)
            {
                item.Text = "Select Year";
                item.Value = "0";
                item.Selected = true;
                lstYears.Add(item);
            }
            if (populateFirstItem && isAllYearsSelected)
            {
                item.Text = "All Years";
                item.Value = "-1";
                item.Selected = true;
                lstYears.Add(item);
            }
            for (int i = 2000; i < DateTime.Now.Year + 1; i++)
            {
                item = new SelectListItem();
                item.Text = i + " - " + (i + 1);
                item.Value = i.ToString();
                //if (i == DateTime.Now.Year && SelectedYear == 0)
                //{
                //    //item.Selected = true;
                //}
                //if (i == SelectedYear)
                //{
                //   // item.Selected = true;
                //}
                lstYears.Add(item);
            }

            return lstYears;
        }

        /// <summary>
        /// Used in Edit Proposal Screen Link Through Population
        /// </summary>
        /// <param name="BlockID"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateLinkThrough(int BlockID, string IMS_UPGRADE_CONNECT, string IMS_PROPOSAL_TYPE)
        {
            List<SelectListItem> lstLinkThrough = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();

            item.Text = (PMGSYSession.Current.PMGSYScheme == 1) ? "Select Core Network Link/Through Route Number" : (PMGSYSession.Current.PMGSYScheme == 3) ? "Select RCPLWE Road Link/Through Route Number" : "Select Candidate Road Major Rural Link/Through Route Number";///Changes for RCPLWE
            item.Value = "";
            item.Selected = true;
            lstLinkThrough.Add(item);

            //if (BlockID == 0)
            ///Changes for RCPLWE Bridge Proposal entry
            if (BlockID <= 0 || (IMS_PROPOSAL_TYPE == "P" && PMGSYSession.Current.PMGSYScheme == 3))
            {
                return lstLinkThrough;
            }
            else
            {
                //var sp = (from c in db.IMS_GET_CORE_NETWORK(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, BlockID, IMS_PROPOSAL_TYPE, IMS_UPGRADE_CONNECT, PMGSYSession.Current.PMGSYScheme == 3 ? 1 : PMGSYSession.Current.PMGSYScheme)///Changes for RCPLWE
                var sp = (from c in db.IMS_GET_CORE_NETWORK(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, BlockID, IMS_PROPOSAL_TYPE, IMS_UPGRADE_CONNECT, PMGSYSession.Current.PMGSYScheme)///Changes for RCPLWE

                          select new
                          {
                              Value = c.PLAN_CN_ROAD_CODE,
                              Text = c.PLAN_RD_NAME
                          }).ToList();

                foreach (var data in sp)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstLinkThrough.Add(item);
                }
                return lstLinkThrough;
            }
        }

        /// <summary>
        /// Populate Link Through
        /// </summary>
        /// <param name="BlockID"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetLinkThroughList()
        {
            int BlockID = Convert.ToInt32(Request.Params["BlockID"]);
            string ims_upgrade_connect = Request.Params["IMS_UPGRADE_CONNECT"].ToString();
            string proposalType = Request.Params["PROPOSAL_TYPE"].ToString();
            return Json(PopulateLinkThrough(BlockID, ims_upgrade_connect, proposalType));
        }

        /// <summary>
        /// Populates MP Constituenct
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult PoulateMPConstituency()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int MAST_BLOCK_CODE = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);

                return Json(objCommonFunctions.PopulateMPConstituency(MAST_BLOCK_CODE));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PoulateMPConstituency()");
                return Json(new { string.Empty });
            }
        }

        /// <summary>
        /// Get the List of Districts
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetDistricts()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int MAST_STATE_CODE = Convert.ToInt32(Request.Params["MAST_STATE_CODE"]);
                if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 65)//Changes by SAMMED A. PATIL for Mord View 
                {
                    return Json(objCommonFunctions.PopulateDistrict(MAST_STATE_CODE, true));
                }
                else if (PMGSYSession.Current.RoleCode == 3)
                {
                    return Json(objCommonFunctions.PopulateDistrictsOfTA(MAST_STATE_CODE, true));
                }
                else
                {
                    return Json(objCommonFunctions.PopulateDistrict(MAST_STATE_CODE, false));
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetDistricts()");
                return Json(new { string.Empty });
            }
        }

        /// <summary>
        /// Populates UnFreezed Batches
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult PoulateUnFreezedBatches()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();

            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            try
            {
                return Json(objCommonFunctions.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, IMS_YEAR, false, true));
                //return Json(objCommonFunctions.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, IMS_YEAR, false, false));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PoulateUnFreezedBatches()");
                return Json(new { string.Empty });
            }
        }

        [HttpGet]
        public JsonResult GetShareCodeByStateCode()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                return Json(new { ShareCode = objCommonFunctions.GetShareCodeByStateCode(PMGSYSession.Current.StateCode) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetShareCodeByStateCode()");
                return Json(new { ShareCode = 0 });
            }
        }

        /// <summary>
        /// Populates MLA Constituency
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult PoulateMLAConstituency()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();

            int MAST_BLOCK_CODE = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);

            try
            {
                return Json(objCommonFunctions.PopulateMLAConstituency(MAST_BLOCK_CODE));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PoulateMLAConstituency()");
                return Json(new { string.Empty });
            }
        }

        /// <summary>
        /// Populate Stage 1 Proposals as year & pakage changes
        /// </summary>
        /// <param name="year"></param>
        /// <param name="packageID"></param>
        /// <returns></returns>
        [Audit]
        public List<SelectListItem> PopulateStagedProposalList(int year, string packageID)
        {
            List<SelectListItem> lstStage1Proposal = new List<SelectListItem>();
            try
            {
                var lstOfStage1Proposal = db.USP_PRO_LIST_STAGE_PROPOSALS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, PMGSYSession.Current.AdminNdCode, year, packageID).ToList();

                SelectListItem item;

                //item = new SelectListItem();
                //item.Text = "Select Road";
                //item.Value = "0";
                //item.Selected = true;
                //lstStage1Proposal.Add(item);

                foreach (var data in lstOfStage1Proposal)
                {
                    item = new SelectListItem();
                    item.Text = data.IMS_STAGED_ROAD;
                    item.Value = data.IMS_PR_ROAD_CODE.ToString();
                    lstStage1Proposal.Add(item);
                }

                return (lstStage1Proposal);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateStagedProposalList()");
                return lstStage1Proposal;
            }
        }

        /// <summary>
        /// Populate Stage 1 Proposals as year & pakage changes
        /// </summary>
        /// <param name="year"></param>
        /// <param name="packageID"></param>
        /// <returns></returns>
        [Audit]
        public List<SelectListItem> PopulateStagedProposalListSRRDA(int year, string packageID, int AdminCode, int DistrictCode)
        {
            List<SelectListItem> lstStage1Proposal = new List<SelectListItem>();
            try
            {
                var lstOfStage1Proposal = db.USP_PRO_LIST_STAGE_PROPOSALS(PMGSYSession.Current.StateCode, DistrictCode, AdminCode, year, packageID).ToList();

                SelectListItem item;

                //item = new SelectListItem();
                //item.Text = "Select Road";
                //item.Value = "0";
                //item.Selected = true;
                //lstStage1Proposal.Add(item);

                foreach (var data in lstOfStage1Proposal)
                {
                    item = new SelectListItem();
                    item.Text = data.IMS_STAGED_ROAD;
                    item.Value = data.IMS_PR_ROAD_CODE.ToString();
                    lstStage1Proposal.Add(item);
                }

                return (lstStage1Proposal);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateStagedProposalListSRRDA()");
                return lstStage1Proposal;
            }
        }

        /// <summary>
        /// Get the Staged Link Through
        /// Applicable For Stage 2 Road Proposals
        /// Called When User Click on Stage 2 RadioButton and/or when User changes the Staged Package ID
        /// Screen : Creation Of Proposal
        /// </summary>
        /// <param name="year"></param>
        /// <param name="batch"></param>
        /// <param name="packageID"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetStagedProposalList(int year, string packageID)
        {
            return Json(PopulateStagedProposalList(year, packageID));
        }

        /// <summary>
        /// Get the Staged Years
        /// Applicable For Stage 2 Road Proposals
        /// Called When User Click on Stage 2 RadioButton
        /// Screen : Creation Of Proposal
        /// </summary>
        /// <param name="year"></param>
        /// <param name="batch"></param>
        /// <param name="packageID"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetStagedYearsList(int year, int proposalCode)
        {
            List<SelectListItem> lstYear = new List<SelectListItem>();
            try
            {
                lstYear = PopulateYear();
                //year = year + 1;
                int count = lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString()));
                lstYear.RemoveRange(lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString())), lstYear.Count - count);
                return Json(lstYear, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetStagedYearsList()");
                return null;
            }
            finally
            {

            }
        }

        /// <summary>
        /// Get the Staged Link Through
        /// Applicable For Stage 2 Road Proposals
        /// Called When User Click on Stage 2 RadioButton and/or when User changes the Staged Package ID
        /// Screen : Creation Of Proposal
        /// </summary>
        /// <param name="year"></param>
        /// <param name="batch"></param>
        /// <param name="packageID"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetStagedProposalListSRRDA(int year, string packageID, int AdminCode, int DistrictCode)
        {
            return Json(PopulateStagedProposalListSRRDA(year, packageID, AdminCode, DistrictCode));
        }


        /// <summary>
        /// For Stage 2 Proposals
        /// Populate link/through routes
        /// </summary>
        /// <param name="year"></param>
        /// <param name="batch"></param>
        /// <param name="packageID"></param>
        /// <returns></returns>

        public List<SelectListItem> PopulateStagedLinkThrough(int year, int batch, string packageID, string OperationType, int stageRoadId)
        {
            List<SelectListItem> lstLinkThrough = new List<SelectListItem>();
            try
            {
                ///// Stage Two Proposals
                if (OperationType == "N")
                {
                    //var lstOfCN = db.USP_PRO_LIST_STAGE2_LINK_THROUGH(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, PMGSYSession.Current.AdminNdCode, year, packageID).ToList();
                    var lstOfCN = (from sp in db.IMS_SANCTIONED_PROJECTS
                                   where sp.IMS_PR_ROAD_CODE == stageRoadId
                                   select new
                                   {

                                       PLAN_CN_ROAD_CODE = sp.PLAN_ROAD.PLAN_CN_ROAD_CODE,
                                       PLAN_RD_NAME = (sp.PLAN_ROAD.PLAN_CN_ROAD_NUMBER + "-" + sp.PLAN_ROAD.PLAN_RD_NAME)

                                   });


                    SelectListItem item;
                    foreach (var data in lstOfCN)
                    {
                        item = new SelectListItem();
                        item.Text = data.PLAN_RD_NAME;
                        item.Value = data.PLAN_CN_ROAD_CODE.ToString();
                        lstLinkThrough.Add(item);
                    }
                }
                else
                {
                    /// Find the Core Network Road for that Package, proposal for the Core Network should be finalized.
                    var filter = (
                                      from q in db.IMS_SANCTIONED_PROJECTS
                                      where
                                          q.MAST_STATE_CODE == PMGSYSession.Current.StateCode &&
                                          q.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode &&
                                          q.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode &&
                                          q.IMS_IS_STAGED == "S" &&
                                          q.IMS_STAGE_PHASE == "S1" &&
                                          q.IMS_PROPOSAL_TYPE == "P" &&
                                          q.IMS_YEAR == year &&
                                          q.IMS_PACKAGE_ID == packageID &&
                                          (q.IMS_ISCOMPLETED != "E" && q.IMS_ISCOMPLETED != "H")
                                      //&& q.IMS_LOCK_STATUS == "Y"
                                      select
                                      q.PLAN_CN_ROAD_CODE).ToList();


                    var Query = (from c in db.PLAN_ROAD
                                 where
                                 filter.Contains(c.PLAN_CN_ROAD_CODE)
                                 select new
                                 {
                                     Value = c.PLAN_CN_ROAD_CODE,
                                     Text = c.PLAN_CN_ROAD_NUMBER + "-" + c.PLAN_RD_NAME
                                 }).ToList();

                    SelectListItem item;
                    foreach (var data in Query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstLinkThrough.Add(item);
                    }
                }

                return (lstLinkThrough);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateStagedLinkThrough()");
                return lstLinkThrough;
            }
        }

        /// <summary>
        /// Get the Staged Link Through
        /// Applicable For Stage 2 Road Proposals
        /// Called When User Click on Stage 2 RadioButton and/or when User changes the Staged Package ID
        /// Screen : Creation Of Proposal
        /// </summary>
        /// <param name="year"></param>
        /// <param name="batch"></param>
        /// <param name="packageID"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetStagedLinkThroughList(int year, int batch, string packageID, int stageRoad)
        {
            return Json(PopulateStagedLinkThrough(year, batch, packageID, "N", stageRoad));
        }

        /// <summary>
        /// Uses
        /// 1) on Selection of Existing Package ID Radio button , Get the List of Exising Packages 
        /// 2) Get the Stage I Package ID and Populate in   Staged Package Number Dropdown List
        /// 
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="BatchID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetStagedPackageID(int Year, int BatchID)
        {
            List<SelectListItem> lstPackage = new List<SelectListItem>();
            try
            {
                SelectListItem item = new SelectListItem();
                if (Year == 0 && BatchID == 0)
                {
                    return lstPackage;
                }
                if (Year != 0 && BatchID == 0)
                {

                    //Populate only packages that are staged and had stage 1 completed.
                    var query = (from c in db.IMS_SANCTIONED_PROJECTS
                                 where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode
                                    && c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode
                                    && c.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode     //condition added by shyam on 26 Sep 2013
                                    && c.IMS_YEAR == Year
                                    && c.IMS_IS_STAGED.ToUpper() == "S"
                                    && c.IMS_STAGE_PHASE.ToUpper() == "S1"
                                 select new
                                 {
                                     Value = c.IMS_PACKAGE_ID,
                                     Text = c.IMS_PACKAGE_ID
                                 }).ToList().Distinct();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text.ToString();
                        item.Value = data.Value.ToString();
                        lstPackage.Add(item);
                    }

                    return lstPackage;
                }
                else
                {
                    var query = (from c in db.IMS_SANCTIONED_PROJECTS
                                 where
                                     c.MAST_STATE_CODE == PMGSYSession.Current.StateCode &&
                                     c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode &&
                                     c.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode &&        //condition added by shyam on 26 Sep 2013
                                     c.IMS_YEAR == Year &&
                                     c.IMS_BATCH == BatchID
                                 select new
                                 {
                                     Value = c.IMS_PACKAGE_ID,
                                     Text = c.IMS_PACKAGE_ID
                                 }).ToList().Distinct();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text.ToString();
                        item.Value = data.Value.ToString();
                        lstPackage.Add(item);
                    }
                    return lstPackage;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetStagedPackageID()");
                return null;
            }
        }

        /// <summary>
        /// Uses
        /// 1) on Selection of Existing Package ID Radio button , Get the List of Exising Packages 
        /// 2) Get the Stage I Package ID and Populate in   Staged Package Number Dropdown List
        /// 
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="BatchID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetStagedPackageIDSRRDA(int Year, int BatchID, int adminCode)
        {
            List<SelectListItem> lstPackage = new List<SelectListItem>();
            try
            {
                SelectListItem item = new SelectListItem();
                if (Year == 0 && BatchID == 0)
                {
                    return lstPackage;
                }
                if (Year != 0 && BatchID == 0)
                {

                    //Populate only packages that are staged and had stage 1 completed.
                    var query = (from c in db.IMS_SANCTIONED_PROJECTS
                                 where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode
                                    && c.MAST_DPIU_CODE == adminCode     //condition added by shyam on 26 Sep 2013
                                    && c.IMS_YEAR == Year
                                    && c.IMS_IS_STAGED.ToUpper() == "S"
                                    && c.IMS_STAGE_PHASE.ToUpper() == "S1"
                                 select new
                                 {
                                     Value = c.IMS_PACKAGE_ID,
                                     Text = c.IMS_PACKAGE_ID
                                 }).ToList().Distinct();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text.ToString();
                        item.Value = data.Value.ToString();
                        lstPackage.Add(item);
                    }

                    return lstPackage;
                }
                else
                {
                    var query = (from c in db.IMS_SANCTIONED_PROJECTS
                                 where
                                     c.MAST_STATE_CODE == PMGSYSession.Current.StateCode &&
                                     c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode &&
                                     c.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode &&        //condition added by shyam on 26 Sep 2013
                                     c.IMS_YEAR == Year &&
                                     c.IMS_BATCH == BatchID
                                 select new
                                 {
                                     Value = c.IMS_PACKAGE_ID,
                                     Text = c.IMS_PACKAGE_ID
                                 }).ToList().Distinct();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text.ToString();
                        item.Value = data.Value.ToString();
                        lstPackage.Add(item);
                    }
                    return lstPackage;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetStagedPackageIDSRRDA()");
                return null;
            }
        }

        /// <summary>
        /// Populate Existing Package
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult GetExistingPackage()
        {
            try
            {
                return Json(objProposalBAL.PopulateExistingPackage(Convert.ToInt32(Request.Params["Year"]), Convert.ToInt32(Request.Params["BatchID"])));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExistingPackage()");
                return Json(new { Success = false, ErrorMessage = "There is an Error while processing your Request." });
            }
        }

        /// <summary>
        /// Get the Existing Packages
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="BatchID"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetPackageId(int Year, int BatchID)
        {
            List<SelectListItem> lstPackage = new List<SelectListItem>();
            try
            {
                SelectListItem item = new SelectListItem();
                if (Year == 0 && BatchID == 0)
                {
                    return Json(lstPackage);
                }
                if (Year != 0 && BatchID == 0)
                {

                    //Populate only packages that are staged and had stage 1 completed.
                    return Json(GetStagedPackageID(Year, 0));
                }
                else
                {
                    return Json(GetStagedPackageID(Year, BatchID));
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetPackageId()");
                return null;
            }
        }

        /// <summary>
        /// Get the Existing Packages
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="BatchID"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetPackageIdSRRDA(int Year, int BatchID, int AdminCode)
        {
            List<SelectListItem> lstPackage = new List<SelectListItem>();
            try
            {
                SelectListItem item = new SelectListItem();
                if (Year == 0 && BatchID == 0)
                {
                    return Json(lstPackage);
                }
                if (Year != 0 && BatchID == 0)
                {

                    //Populate only packages that are staged and had stage 1 completed.
                    return Json(GetStagedPackageIDSRRDA(Year, 0, AdminCode));
                }
                else
                {
                    return Json(GetStagedPackageIDSRRDA(Year, BatchID, AdminCode));
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetPackageIdSRRDA()");
                return null;
            }
        }

        /// <summary>
        /// returns the agency list for dropdown population
        /// </summary>
        /// <returns></returns>
        public JsonResult PopulateAgencies()
        {
            try
            {
                PMGSY.DAL.Proposal.ProposalDAL objDAL = new PMGSY.DAL.Proposal.ProposalDAL();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                return Json(objDAL.PopulateAgencies(stateCode, true));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateAgencies()");
                return null;
            }
        }

        #endregion

        #region REVISION

        /// <summary>
        /// returns view for add/edit of Revision of Cost or Length
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult RevisedCostLength()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "RevisedCostLength()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// provides view for adding revision details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddRevisionDetails(string id)
        {
            try
            {
                int proposalCode = 0;
                RevisedCostLengthViewModel objModel = new RevisedCostLengthViewModel();
                if ((int.TryParse(id, out proposalCode)))
                {
                    objModel.IMS_PR_ROAD_CODE = proposalCode;
                }
                objProposalBAL = new ProposalBAL();
                objModel = objProposalBAL.GetOldRevisedCostLengthBAL(proposalCode);
                return PartialView("AddRevisionDetails", objModel);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddRevisionDetails()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// List of revision of cost or length of road
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetRevisedCostLengthList(int? page, int? rows, string sidx, string sord)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            objProposalBAL = new ProposalBAL();
            long totalRecords = 0;
            int imsRoadCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    imsRoadCode = Convert.ToInt32(Request.Params["roadCode"]);
                }

                var jsonData = new
                {
                    rows = objProposalBAL.GetRevisedCostLengthListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetRevisedCostLengthList()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }

        }

        /// <summary>
        /// List of revision details of lsb
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetRevisionBridgeList(int? page, int? rows, string sidx, string sord)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            objProposalBAL = new ProposalBAL();
            long totalRecords = 0;
            int imsRoadCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    imsRoadCode = Convert.ToInt32(Request.Params["roadCode"]);
                }

                var jsonData = new
                {
                    rows = objProposalBAL.GetRevisionBridgeListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetRevisionBridgeList()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }

        }

        /// <summary>
        /// post method for adding the Revision details
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddRevisedCostLength(RevisedCostLengthViewModel objModel)
        {
            try
            {
                string message = string.Empty;
                if (ModelState.IsValid)
                {
                    bool Status = objProposalBAL.AddRevisedCostLengthBAL(objModel, ref message);
                    if (Status)
                        return Json(new { success = true, message = message });
                    else
                        return Json(new { success = false, message = message });
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddRevisedCostLength()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        /// <summary>
        /// updating the revision details
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditRevisionDetails(RevisedCostLengthViewModel objModel)
        {
            try
            {
                string message = string.Empty;
                if (ModelState.IsValid)
                {
                    bool Status = objProposalBAL.EditRevisionDetailsBAL(objModel, ref message);
                    if (Status)
                        return Json(new { success = true, message = message });
                    else
                        return Json(new { success = false, message = message });
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditRevisionDetails(RevisedCostLengthViewModel objModel)");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        /// <summary>
        /// returns the edit view of revision details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditRevisedDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            objProposalBAL = new ProposalBAL();
            int proposalCode = 0;
            int revisionCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                revisionCode = Convert.ToInt32(decryptedParameters["RevisionCode"]);
                RevisedCostLengthViewModel model = new RevisedCostLengthViewModel();
                model = objProposalBAL.GetRevisionDetailsBAL(proposalCode, revisionCode);
                return PartialView("AddRevisionDetails", model);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditRevisionDetails(String parameter, String hash, String key)");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Action to get proposal type (whether Road or Bridge)
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult GetProposalType(string id)
        {
            try
            {
                ProposalDAL objDAL = new ProposalDAL();
                int proposalCode = 0;
                string proposalType = string.Empty;
                if ((int.TryParse(id, out proposalCode)))
                {
                    proposalType = objDAL.GetProposalType(proposalCode);
                }
                return Json(new { data = proposalType });
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetProposalType()");
                return Json(new { data = "Error" });
            }
        }

        #endregion

        #region TECHNOLOGY

        /// <summary>
        /// returns the view for listing the technology details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult ListTechnologyDetails(string parameter, string hash, string key)
        {

            int IMS_PR_ROAD_CODE = 0;
            objProposalBAL = new ProposalBAL();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                    }
                }

                UnlockProposalViewModel model = objProposalBAL.GetPropsoalDetailsBAL(IMS_PR_ROAD_CODE);
                ViewBag.ProposalCode = IMS_PR_ROAD_CODE;
                ViewBag.RoadName = model.IMS_ROAD_NAME;
                ViewBag.Year = model.IMS_YEAR;
                ViewBag.Batch = model.IMS_BATCH;
                ViewBag.Package = model.IMS_PACKAGE_ID;
                ViewBag.PavLength = model.IMS_PAV_LENGTH;

                if (Request.Params["Convergence"] != null)
                {
                    ViewBag.Convergence = Request.Params["Convergence"];
                }
                else
                {
                    ViewBag.Convergence = "N";
                }

                return PartialView("ListTechnologyDetails");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListTechnologyDetails()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// get method for add view of Technology details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddTechnologyDetails(string id)
        {
            int proposalCode = 0;
            TechnologyDetailsViewModel model = new TechnologyDetailsViewModel();
            CommonFunctions objCommon = new CommonFunctions();
            objProposalBAL = new ProposalBAL();
            ProposalDAL objProposalDAL = new ProposalDAL();
            try
            {
                if (!int.TryParse(id, out proposalCode))
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
                else
                {
                    model.IMS_PR_ROAD_CODE = proposalCode;
                }

                //model.ListLayers = objCommon.PopulateRoadExecutionItems();

                model.ListTechnology = objCommon.PopulateTechnologyItems();
                //proposalModel.Districts.Find(x => x.Value == "-1").Value = "0";
                //proposalModel.Districts.Find(x => x.Value == proposalModel.MAST_District_CODE.ToString()).Selected = true;
                model.ListTechnology.RemoveAt(0);
                model.MAST_TECH_CODE = model.ListTechnology.First().MAST_TECH_CODE;
                model.ListLayers = objProposalDAL.PopulateRoadExecutionItemsTechnologywise(model.MAST_TECH_CODE);
                model.Operation = "A";

                if (Request.Params["Convergence"] != null)
                {
                    model.convergence = Request.Params["Convergence"];
                }
                else
                {
                    model.convergence = "N";
                }

                return PartialView("AddTechnologyDetails", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddTechnologyDetails() get method");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        public JsonResult PopulateLayersTechnologywise()
        {
            try
            {
                PMGSY.DAL.Proposal.ProposalDAL objDAL = new PMGSY.DAL.Proposal.ProposalDAL();
                int techCode = Convert.ToInt32(Request.Params["techCode"]);
                return Json(objDAL.PopulateRoadExecutionItemsTechnologywise(techCode));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateLayersTechnologywise()");
                return null;
            }
        }

        /// <summary>
        /// post method for adding the Technology details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddTechnologyDetails(TechnologyDetailsViewModel model)
        {
            try
            {
                string message = string.Empty;
                objProposalBAL = new ProposalBAL();
                if (ModelState.IsValid)
                {
                    bool status = objProposalBAL.AddTechnologyDetailsBAL(model, ref message);
                    if (status)
                        return Json(new { success = true, message = message });
                    else
                        return Json(new { success = false, message = message });
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddTechnologyDetails() post method");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// post method for updating the Technology details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditTechnologyDetails(TechnologyDetailsViewModel model)
        {
            try
            {
                string message = string.Empty;
                objProposalBAL = new ProposalBAL();
                if (ModelState.IsValid)
                {
                    bool status = objProposalBAL.EditTechnologyDetailsBAL(model, ref message);
                    if (status)
                        return Json(new { success = true, message = message });
                    else
                        return Json(new { success = false, message = message });
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditTechnologyDetails()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// deletes the technology details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult DeleteTechnologyDetails(String parameter, String hash, String key)
        {
            objProposalBAL = new ProposalBAL();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                bool status = objProposalBAL.DeleteTechnologyDetails(Convert.ToInt32(decryptedParameters["ProposalCode"]), Convert.ToInt32(decryptedParameters["SegmentCode"]));
                if (status == true)
                {
                    return Json(new { success = true, message = "Technology Details deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteTechnologyDetails()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns data of particular technology details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetTechnologyDetailsForEdit(String parameter, String hash, String key)
        {
            objProposalBAL = new ProposalBAL();
            Dictionary<string, string> decryptedParameters = null;
            TechnologyDetailsViewModel model = new TechnologyDetailsViewModel();
            CommonFunctions objCommom = new CommonFunctions();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                model = objProposalBAL.GetTechnologyDetails(Convert.ToInt32(decryptedParameters["ProposalCode"]), Convert.ToInt32(decryptedParameters["SegmentCode"]));
                model.ListLayers = objCommom.PopulateRoadExecutionItems();
                model.ListTechnology = objCommom.PopulateTechnologyItems();
                if (model != null)
                {
                    return PartialView("AddTechnologyDetails", model);
                }
                else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetTechnologyDetailsForEdit()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the list of Technology details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetTechnologyDetailsList(int? page, int? rows, string sidx, string sord)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            objProposalBAL = new ProposalBAL();
            long totalRecords = 0;
            int imsRoadCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["ProposalCode"])))
                {
                    imsRoadCode = Convert.ToInt32(Request.Params["ProposalCode"]);
                }

                var jsonData = new
                {
                    rows = objProposalBAL.GetTechnologyDetailsListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetTechnologyDetailsList()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }

        }

        public JsonResult GetStartChainage(String id)
        {
            int proposalCode = 0;
            int techCode = 0;
            int layerCode = 0;
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    if (id.Split('$').Count() == 3)
                    {
                        proposalCode = Convert.ToInt32(id.Split('$')[0]);
                        techCode = Convert.ToInt32(id.Split('$')[1]);
                        layerCode = Convert.ToInt32(id.Split('$')[2]);
                    }
                }

                decimal? endChainage = objProposalBAL.GetTechnologyStartChainage(proposalCode, techCode, layerCode);
                return Json(new { Success = true, StartChainage = endChainage });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetStartChainage()");
                return Json(null);
            }
        }

        #endregion

        #region REVISION_ITNO

        /// <summary>
        /// returns the view of list of revision details for itno
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListProposalRevisionDetails()
        {
            ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
            proposalViewModel.STATES = objCommonFuntion.PopulateStates();
            proposalViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            if (PMGSYSession.Current.RoleCode != 47)
            {
                proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode);
            }
            else
            {
                List<SelectListItem> districtList = new List<SelectListItem>();
                districtList = objCommonFuntion.GetAllDistrictsByAdminNDCode(PMGSYSession.Current.StateCode, PMGSYSession.Current.AdminNdCode);
                districtList.Insert(0, new SelectListItem { Value = "0", Text = "--Select District--" });
                proposalViewModel.DISTRICTS = districtList;
            }
            proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
            proposalViewModel.BATCHS.Find(x => x.Value == "0").Text = "All";
            proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
            proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
            proposalViewModel.Years = PopulateYear(0, true, true);
            proposalViewModel.IMS_YEAR = DateTime.Now.Year;
            proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;
            return View("ListProposalRevisionDetails", proposalViewModel);
        }

        #endregion

        #region Test Result Details

        /// <summary>
        /// Render View for test Result details.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult TestResultDetails(string id)
        {

            try
            {

                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = null;
                IProposalBAL objProposalBAL = new ProposalBAL();
                TestResultViewModel testResultViewModel = new TestResultViewModel();

                //get Road Details
                ims_sanctioned_projects = objProposalBAL.GetRoadDetails(Convert.ToInt32(id));
                //if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47) //ITNO=36 or OAITNO=47
                //{
                //    if ((ims_sanctioned_projects.IMS_SANCTIONED == "N") && (ims_sanctioned_projects.IMS_ISCOMPLETED == "D" || ims_sanctioned_projects.IMS_ISCOMPLETED == "S"))
                //    {
                //        testResultViewModel.RoadStatus = true;
                //    }
                //    else
                //    {
                //        testResultViewModel.RoadStatus = false;
                //    }
                //}

                //else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)//PIU=22 && PIUOA=38
                //{
                //    if ((ims_sanctioned_projects.IMS_SANCTIONED == "N") && (ims_sanctioned_projects.IMS_ISCOMPLETED == "E" || ims_sanctioned_projects.IMS_ISCOMPLETED == "H"))
                //    {
                //        testResultViewModel.RoadStatus = true;
                //    }
                //    else
                //    {
                //        testResultViewModel.RoadStatus = false;
                //    }
                //}

                testResultViewModel.RoadStatus = true;//Change on 23 Sept 2014

                testResultViewModel.hidden_ims_pr_road_code = Convert.ToInt32(id);

                //set Road Details                                                            
                testResultViewModel.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                testResultViewModel.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                testResultViewModel.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
                testResultViewModel.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                testResultViewModel.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;

                return View(testResultViewModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TestResultDetails()");
                return View(new TestResultViewModel());
            }
        }

        /// <summary>
        ///  Returns Details related to Test Result such as Chainage,Value,Sample,Test Names.
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetTestResultList(FormCollection formCollection)
        {

            long totalRecords = 0;
            int IMS_PR_ROAD_CODE = 0;

            IProposalBAL objProposalBAL = new ProposalBAL();


            try
            {
                using (CommonFunctions objCommonfunction = new CommonFunctions())
                {

                    if (!objCommonfunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(formCollection["_search"]), Convert.ToInt64(formCollection["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(formCollection["IMS_PR_ROAD_CODE"]))
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"]);
                }


                var jsonData = new
                {
                    rows = objProposalBAL.TestResultDetails(IMS_PR_ROAD_CODE, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetTestResultList()");
                return null;
            }
        }

        /// <summary>
        /// Returns Test Result Sample Details such as chainage,value,sample.
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetTestResultSampleList(FormCollection formCollection)
        {

            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            long totalRecords = 0;
            int IMS_PR_ROAD_CODE = 0;

            IProposalBAL objProposalBAL = new ProposalBAL();

            try
            {
                //using (CommonFunctions objCommonfunction = new CommonFunctions())
                //{

                //    if (!objCommonfunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(formCollection["_search"]), Convert.ToInt64(formCollection["nd"]))))
                //    {
                //        return null;
                //    }
                //}

                if (!string.IsNullOrEmpty(formCollection["IMS_PR_ROAD_CODE"]))
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"]);
                }


                var jsonData = new
                {
                    rows = objProposalBAL.TestResultSampleDetails(IMS_PR_ROAD_CODE, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetTestResultSampleList()");
                return null;
            }
        }

        /// <summary>
        /// Add Test result details
        /// </summary>
        /// <param name="testResultViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddTestResultDetails(TestResultViewModel testResultViewModel)
        {
            string message = string.Empty;
            IProposalBAL objProposalBAL = new ProposalBAL();

            try
            {
                if (ModelState.IsValid)
                {

                    if (objProposalBAL.AddTestResultDetails(testResultViewModel, ref message))
                    {
                        message = message == string.Empty ? "Test Result Details saved successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        message = message == string.Empty ? "Test Result Details not saved." : message;
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {

                    string messages = string.Join("; ", ModelState.Values
                                           .SelectMany(x => x.Errors)
                                           .Select(x => x.ErrorMessage));

                    return PartialView("TestResultDetails", testResultViewModel);
                }
            }
            catch (DbEntityValidationException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddTestResultDetails(DbEntityValidationException ex)");
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddTestResultDetails()");
                message = "Test result details not saved because " + ex.Message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get Test result Details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditTestResultDetails(String parameter, String hash, String key)
        {
            try
            {
                IProposalBAL objProposalBAL = new ProposalBAL();

                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = null;

                Dictionary<string, string> decryptedParameters = null;

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int resultCode = Convert.ToInt32(decryptedParameters["IMS_RESULT_CODE"].ToString());
                    int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"].ToString());

                    TestResultViewModel testResultViewModel = objProposalBAL.EditTestResultDetails(resultCode, imsPrRoadCode);
                    testResultViewModel.hidden_ims_pr_road_code = imsPrRoadCode;

                    if (testResultViewModel == null)
                    {
                        ModelState.AddModelError("", "Test Result Details not exist.");
                        return PartialView("TestResultDetails", new TestResultViewModel());
                    }

                    //get Road Details
                    ims_sanctioned_projects = objProposalBAL.GetRoadDetails(imsPrRoadCode);

                    //set Road Details                                                            
                    testResultViewModel.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                    testResultViewModel.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                    testResultViewModel.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
                    testResultViewModel.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                    testResultViewModel.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;
                    //if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47) //ITNO=36 or OAITNO=47
                    //{
                    //    if ((ims_sanctioned_projects.IMS_SANCTIONED == "N") && (ims_sanctioned_projects.IMS_ISCOMPLETED == "D" || ims_sanctioned_projects.IMS_ISCOMPLETED == "S"))
                    //    {
                    //        testResultViewModel.RoadStatus = true;
                    //    }
                    //    else
                    //    {
                    //        testResultViewModel.RoadStatus = false;
                    //    }
                    //}
                    //else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)//PIU=22 && PIUOA=38
                    //{
                    //    if ((ims_sanctioned_projects.IMS_SANCTIONED == "N") && (ims_sanctioned_projects.IMS_ISCOMPLETED == "E" || ims_sanctioned_projects.IMS_ISCOMPLETED == "H"))
                    //    {
                    //        testResultViewModel.RoadStatus = true;
                    //    }
                    //    else
                    //    {
                    //        testResultViewModel.RoadStatus = false;
                    //    }
                    //}
                    testResultViewModel.RoadStatus = true; //For Change on 23 Sept 2014
                    return PartialView("TestResultDetails", testResultViewModel);
                }
                return PartialView("TestResultDetails", new TestResultViewModel());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditTestResultDetails()");
                ModelState.AddModelError("", "Test Result Details not exist.");
                return PartialView("TestResultDetails", new TestResultViewModel());
            }
        }


        /// <summary>
        /// Update Test Result details.
        /// </summary>
        /// <param name="testResultViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult UpdateTestResultDetails(TestResultViewModel testResultViewModel)
        {
            string message = string.Empty;
            try
            {

                if (ModelState.IsValid)
                {
                    IProposalBAL objProposalBAL = new ProposalBAL();

                    if (objProposalBAL.UpdateTestResultDetails(testResultViewModel, ref message))
                    {
                        message = message == string.Empty ? "Test Result Details Updated successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        message = message == string.Empty ? "Test Result Details not updated." : message;
                        return Json(new { success = false, message = message });
                    }

                }
                else
                {
                    message = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                        );

                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (DbEntityValidationException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "UpdateTestResultDetails(DbEntityValidationException ex)");
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "UpdateTestResultDetails()");
                message = "Test Result Details not saved because " + ex.Message;
                return Json(new { succes = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete Test Result details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteTestDetails(string parameter, string hash, string key)
        {
            string message = string.Empty;

            try
            {
                Dictionary<string, string> decryptedParameters = null;

                IProposalBAL objProposalBAL = new ProposalBAL();

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int resultCode = Convert.ToInt32(decryptedParameters["IMS_RESULT_CODE"].ToString());
                    int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"].ToString());

                    if (objProposalBAL.DeleteTestResultDetails(resultCode, imsPrRoadCode, ref message))
                    {
                        message = "Test Result details deleted successfully.";
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "Test Result details not deleted.";
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                message = "An error occured while processing you request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteTestDetails()");
                message = "An error occured while proccessing your request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        //Role: ITNO 


        /// <summary>
        /// Get Method of Proposal Home Page
        /// Contains Filters for Listing Proposals 
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListProposalForTestResult()
        {
            ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();

            //proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;

            proposalViewModel.Years = PopulateYear(0, true, true);
            proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, true);
            proposalViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
            proposalViewModel.DISTRICTS.Find(x => x.Value == "-1").Value = "0";
            proposalViewModel.DISTRICTS.Find(x => x.Value == proposalViewModel.MAST_DISTRICT_CODE.ToString()).Selected = true;
            proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
            proposalViewModel.BATCHS.Find(x => x.Value == "0").Text = "All Batch";
            proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);

            proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);

            proposalViewModel.IMS_YEAR = DateTime.Now.Year;

            //proposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("", true);
            proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;

            return View("ListProposalForTestResult", proposalViewModel);
        }


        /// <summary>
        /// Enlist the Road Proposals for ITNO Login    
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetItnoRoadProposals(FormCollection formCollection)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);

            int totalRecords;

            var jsonData = new
            {
                rows = objProposalBAL.GetItnoProposalsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }



        #endregion

        #region SANCTION_ORDER_GENERATION

        /// <summary>
        /// returns the filter view for sanction order generation
        /// </summary>
        /// <returns></returns>
        public ActionResult ListProposalsForSanctionOrder()
        {
            SanctionOrderFilterModel filterModel = new SanctionOrderFilterModel();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                filterModel.StateList = objCommon.PopulateStates(true);
                filterModel.BatchList = objCommon.PopulateBatch();
                filterModel.StreamList = objCommon.PopulateFundingAgency(false);
                //filterModel.YearList = new SelectList(objCommon.PopulateFinancialYear(true, false).ToList(), "Value", "Text").ToList();
                filterModel.YearList = new List<SelectListItem>();
                filterModel.YearList.Insert(0, new SelectListItem() { Text = "Select Year", Value = "-1" });

                filterModel.AgencyList = new List<SelectListItem>();
                filterModel.AgencyList.Insert(0, new SelectListItem() { Text = "All Agencies", Value = "0" });

                return View(filterModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListProposalsForSanctionOrder()");
                return null;
            }
        }

        public JsonResult PopulateFinancialYearsByState()
        {
            try
            {
                PMGSY.DAL.Proposal.ProposalDAL objDAL = new PMGSY.DAL.Proposal.ProposalDAL();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                return Json(objDAL.PopulateFinancialYearsByStateDAL(stateCode, false));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateFinancialYearsByState()");
                return null;
            }
        }

        /// <summary>
        /// returns the view for sanction order addition
        /// </summary>
        /// <returns></returns>
        public ActionResult SanctionOrderView()
        {
            SanctionOrderViewModel model = new SanctionOrderViewModel();
            PMGSY.DAL.Proposal.ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();

            try
            {
                if (!String.IsNullOrEmpty(Request.Params["StateCode"]))
                {
                    model.StateCode = Convert.ToInt32(Request.Params["StateCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["YearCode"]))
                {
                    model.YearCode = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["StreamCode"]))
                {
                    model.StreamCode = Convert.ToInt32(Request.Params["StreamCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["BatchCode"]))
                {
                    model.BatchCode = Convert.ToInt32(Request.Params["BatchCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["SchemeCode"]))
                {
                    model.PMGSYScheme = Convert.ToInt32(Request.Params["SchemeCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Agency"]))
                {
                    model.Agency = Convert.ToInt32(Request.Params["Agency"]);
                }

                model.IsSOGenerated = objProposalDAL.IsSanctionOrderGenerated(model);

                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SanctionOrderView()");
                return null;
            }
        }

        /// <summary>
        /// returns the list of proposals according to batch for sanction order generations
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult GetProposalListByBatch(int? page, int? rows, string sidx, string sord)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            objProposalBAL = new ProposalBAL();
            long totalRecords = 0;
            int stateCode = 0;
            int year = 0;
            int streamCode = 0;
            int batch = 0;
            int scheme = 0;
            string proposalType = string.Empty;
            bool IsSOGenerated = false;
            int agency = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["StateCode"])))
                {
                    stateCode = Convert.ToInt32(Request.Params["StateCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["YearCode"])))
                {
                    year = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["StreamCode"])))
                {
                    streamCode = Convert.ToInt32(Request.Params["StreamCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BatchCode"])))
                {
                    batch = Convert.ToInt32(Request.Params["BatchCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["Scheme"])))
                {
                    scheme = Convert.ToInt32(Request.Params["Scheme"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["Type"])))
                {
                    proposalType = Request.Params["Type"];
                }

                if (!(string.IsNullOrEmpty(Request.Params["Agency"])))
                {
                    agency = Convert.ToInt32(Request.Params["Agency"]);
                }

                var jsonData = new
                {
                    rows = objProposalBAL.GetProposalsForSanctionOrder(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, stateCode, year, streamCode, agency, batch, scheme, proposalType, out IsSOGenerated),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    IsSOGenerated = IsSOGenerated,
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetProposalListByBatch()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// save the data for sanction order
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSanctionOrder(SanctionOrderViewModel model)
        {
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    bool Status = objProposalBAL.AddSanctionOrderBAL(model, ref message);
                    if (Status)
                        return Json(new { success = true, message = message });
                    else
                        return Json(new { success = false, message = message });
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddSanctionOrder(SanctionOrderViewModel model)");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the report for Sanction Order Generation
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewSanctionOrderReport()
        {
            SanctionOrderFilterModel model = new SanctionOrderFilterModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            ProposalDAL objDAL = new ProposalDAL();
            try
            {
                if (!(string.IsNullOrEmpty(Request.Params["StateCode"])))
                {
                    model.State = Convert.ToInt32(Request.Params["StateCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["YearCode"])))
                {
                    model.Year = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["StreamCode"])))
                {
                    model.Stream = Convert.ToInt32(Request.Params["StreamCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BatchCode"])))
                {
                    model.Batch = Convert.ToInt32(Request.Params["BatchCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["SchemeCode"]))
                {
                    model.PMGSYScheme = Convert.ToInt32(Request.Params["SchemeCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Agency"]))
                {
                    model.Agency = Convert.ToInt32(Request.Params["Agency"]);
                }

                IMS_SANCTIONED_PROJECTS_PDF sanctionModel = new IMS_SANCTIONED_PROJECTS_PDF();
                sanctionModel = objDAL.GetSanctionOrderMaster(model);
                if (sanctionModel != null)
                {
                    string FullFilePhysicalPath = Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"] : PMGSYSession.Current.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_RCPLWE"] : PMGSYSession.Current.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"] : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYIII"], sanctionModel.IMS_PDF_NAME);
                }
                if (sanctionModel != null)
                {
                    model.BatchName = "Batch : " + model.Batch;
                    model.StateName = sanctionModel.MASTER_STATE.MAST_STATE_NAME;
                    model.CollaborationName = sanctionModel.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_NAME;
                    model.SanctionOrderNo = sanctionModel.IMS_ORDER_NUMBER;
                    model.SanctionOrderDate = objCommonFuntion.GetDateTimeToString(sanctionModel.IMS_ORDER_DATE);
                }
                else
                {
                    model.BatchName = "Batch : " + model.Batch;
                    model.StateName = db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_NAME).FirstOrDefault();
                    model.CollaborationName = db.MASTER_FUNDING_AGENCY.Where(m => m.MAST_FUNDING_AGENCY_CODE == model.Stream).Select(m => m.MAST_FUNDING_AGENCY_NAME).FirstOrDefault();
                    model.SanctionOrderNo = "-";
                    model.SanctionOrderDate = "-";
                }

                GenerateReport(model, sanctionModel);

                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewSanctionOrderReport()");
                return null;
            }
        }

        /// <summary>
        /// returns the sanction order list
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSanctionOrderList(int? page, int? rows, string sidx, string sord)
        {
            objProposalBAL = new ProposalBAL();
            long totalRecords = 0;
            int stateCode = 0;
            int year = 0;
            int streamCode = 0;
            int batch = 0;
            int scheme = 0;
            string proposalType = string.Empty;
            int agency = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["StateCode"])))
                {
                    stateCode = Convert.ToInt32(Request.Params["StateCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["YearCode"])))
                {
                    year = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["StreamCode"])))
                {
                    streamCode = Convert.ToInt32(Request.Params["StreamCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BatchCode"])))
                {
                    batch = Convert.ToInt32(Request.Params["BatchCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["Scheme"])))
                {
                    scheme = Convert.ToInt32(Request.Params["Scheme"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["Type"])))
                {
                    proposalType = Request.Params["Type"];
                }

                if (!(string.IsNullOrEmpty(Request.Params["Agency"])))
                {
                    agency = Convert.ToInt32(Request.Params["Agency"]);
                }

                var jsonData = new
                {
                    rows = objProposalBAL.GetSanctionOrderListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, stateCode, year, streamCode, agency, batch, scheme, proposalType),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetSanctionOrderList()");
                return null;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// returns the sanction order in form of pdf
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetSanctionOrder(string id)
        {
            try
            {
                string[] parameters = id.Split('$');
                int state = Convert.ToInt32(parameters[0]);
                int batch = Convert.ToInt32(parameters[1]);
                int collaboration = Convert.ToInt32(parameters[2]);
                int year = Convert.ToInt32(parameters[3]);
                int scheme = Convert.ToInt32(parameters[4]);
                string filename = parameters[5];
                ///Changes by SAMMED A. PATIL for RCPLWE
                var filePath = (scheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"] : scheme == 3 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_RCPLWE"] : scheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"] : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYIII"]) + filename;
                if (System.IO.File.Exists(filePath))
                {
                    Byte[] file = System.IO.File.ReadAllBytes(filePath);

                    //Response.ContentType = "application/pdf";

                    //Response.AddHeader("Content-disposition", "filename=" + filename + ".pdf");

                    //Response.OutputStream.Write(file, 0, file.Length);

                    //Response.OutputStream.Flush();

                    //Response.OutputStream.Close();

                    //Response.Flush();

                    //Response.Close();

                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        // for example foo.bak
                        FileName = filename,

                        // always prompt the user for downloading, set to true if you want 
                        // the browser to try to show the file inline
                        Inline = false,

                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(file, "application/pdf");
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetSanctionOrder()");
                return null;
            }
        }

        /// <summary>
        /// returns the pdf file for view of sanction order
        /// </summary>
        /// <returns></returns>
        public ActionResult PreviewSanctionOrderReport()
        {
            SanctionOrderFilterModel model = new SanctionOrderFilterModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            try
            {
                if (!(string.IsNullOrEmpty(Request.Params["StateCode"])))
                {
                    model.State = Convert.ToInt32(Request.Params["StateCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["YearCode"])))
                {
                    model.Year = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["StreamCode"])))
                {
                    model.Stream = Convert.ToInt32(Request.Params["StreamCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BatchCode"])))
                {
                    model.Batch = Convert.ToInt32(Request.Params["BatchCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["SchemeCode"]))
                {
                    model.PMGSYScheme = Convert.ToInt32(Request.Params["SchemeCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["ProposalType"]))
                {
                    model.ProposalType = Request.Params["ProposalType"];
                }

                if (!(string.IsNullOrEmpty(Request.Params["Agency"])))
                {
                    model.Agency = Convert.ToInt32(Request.Params["Agency"]);
                }

                IMS_SANCTIONED_PROJECTS_PDF sanctionModel = new IMS_SANCTIONED_PROJECTS_PDF();
                sanctionModel = db.IMS_SANCTIONED_PROJECTS_PDF.Where(m => m.MAST_STATE_CODE == model.State && m.IMS_BATCH == model.Batch && m.IMS_COLLABORATION == model.Stream && m.IMS_YEAR == model.Year && m.MAST_PMGSY_SCHEME == model.PMGSYScheme && m.MAST_AGENCY_CODE == model.Agency).FirstOrDefault();
                //if (sanctionModel != null)
                //{
                //    string FullFilePhysicalPath = Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"] : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"], sanctionModel.IMS_PDF_NAME);
                //}
                if (sanctionModel != null)
                {
                    model.BatchName = "Batch : " + model.Batch;
                    model.StateName = sanctionModel.MASTER_STATE.MAST_STATE_NAME;
                    model.CollaborationName = sanctionModel.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_NAME;
                    model.SanctionOrderNo = sanctionModel.IMS_ORDER_NUMBER;
                    model.SanctionOrderDate = objCommonFuntion.GetDateTimeToString(sanctionModel.IMS_ORDER_DATE);
                }
                else
                {
                    model.BatchName = "Batch : " + model.Batch;
                    model.StateName = db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_NAME).FirstOrDefault();
                    model.CollaborationName = db.MASTER_FUNDING_AGENCY.Where(m => m.MAST_FUNDING_AGENCY_CODE == model.Stream).Select(m => m.MAST_FUNDING_AGENCY_NAME).FirstOrDefault();
                    model.SanctionOrderNo = "-";
                    model.SanctionOrderDate = "-";
                }



                Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
                rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);

                System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("State", model.State.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("District", "0"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Block", "0"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Year", model.Year.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Batch", model.Batch.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Agency", model.Agency.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Collaboration", model.Stream.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Status", model.ProposalType));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PMGSY", model.PMGSYScheme.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", model.StateName));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("BatchName", (model.Batch == null ? "-" : model.Batch.ToString())));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("CollaborationName", model.CollaborationName));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("SanctionLetterNo", (model.SanctionOrderNo == null ? "-" : model.SanctionOrderNo)));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("SanctionDate", (model.SanctionOrderDate == null ? "-" : model.SanctionOrderDate)));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("UserId", PMGSY.Extensions.PMGSYSession.Current.UserId.ToString()));
                //Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(@"ndcad\ommasadmin", "Ndcsp@123");
                Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Username"], System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Password"]);
                //Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(@"Administrator", "Admin@321");
                rview.ServerReport.ReportServerCredentials = irsc;
                rview.ServerReport.ReportPath = "/PMGSYCitizen/SanctionedProposalList";
                //rview.ServerReport.ReportPath = "/SampleLocalizedReport/SanctionedProposalList";
                rview.ServerReport.SetParameters(paramList);
                string mimeType, encoding, extension, deviceInfo;
                string[] streamids;
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string format = "PDF"; //Desired format goes here (PDF, Excel, or Image)

                deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>True</SimplePageHeaders>" + "</DeviceInfo>";
                byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileName = (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.Year + "-" + (model.Year + 1)) + "_BATCH" + model.Batch + "_" + (model.CollaborationName) + "_SCHEME" + model.PMGSYScheme + ".pdf";
                //string filePath = model.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"].ToString() + fileName + ".pdf" : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"].ToString() + fileName + ".pdf";
                string filePath = model.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"].ToString() + fileName + ".pdf" : model.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"].ToString() + fileName + ".pdf" : model.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_RCPLWE"].ToString() + fileName + ".pdf" : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYIII"].ToString() + fileName + ".pdf";
                Response.Clear();
                //if (format == "PDF")
                //{
                //    Response.ContentType = "application/pdf";
                //    Response.AddHeader("Content-disposition", "filename=" + fileName + ".pdf");

                //}

                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = fileName,

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,

                };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(bytes, "application/pdf");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PreviewSanctionOrderReport()");
                return null;
            }
        }

        /// <summary>
        /// returns the pdf file for district abstract report.
        /// </summary>
        /// <returns></returns>
        public ActionResult PreviewDistrictAbstractReport()
        {
            SanctionOrderFilterModel model = new SanctionOrderFilterModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            try
            {
                if (!(string.IsNullOrEmpty(Request.Params["StateCode"])))
                {
                    model.State = Convert.ToInt32(Request.Params["StateCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["YearCode"])))
                {
                    model.Year = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["StreamCode"])))
                {
                    model.Stream = Convert.ToInt32(Request.Params["StreamCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BatchCode"])))
                {
                    model.Batch = Convert.ToInt32(Request.Params["BatchCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["SchemeCode"]))
                {
                    model.PMGSYScheme = Convert.ToInt32(Request.Params["SchemeCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["ProposalType"]))
                {
                    model.ProposalType = Request.Params["ProposalType"];
                }

                if (!String.IsNullOrEmpty(Request.Params["Agency"]))
                {
                    model.Agency = Convert.ToInt32(Request.Params["Agency"]);
                }

                IMS_SANCTIONED_PROJECTS_PDF sanctionModel = new IMS_SANCTIONED_PROJECTS_PDF();
                sanctionModel = db.IMS_SANCTIONED_PROJECTS_PDF.Where(m => m.MAST_STATE_CODE == model.State && m.IMS_BATCH == model.Batch && m.IMS_COLLABORATION == model.Stream && m.IMS_YEAR == model.Year && m.MAST_PMGSY_SCHEME == model.PMGSYScheme && m.MAST_AGENCY_CODE == model.Agency).FirstOrDefault();
                if (sanctionModel != null)
                {
                    // string FullFilePhysicalPath = Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"] : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"], sanctionModel.IMS_PDF_NAME);
                }
                if (sanctionModel != null)
                {
                    model.BatchName = "Batch : " + model.Batch;
                    model.StateName = sanctionModel.MASTER_STATE.MAST_STATE_NAME;
                    model.CollaborationName = sanctionModel.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_NAME;
                    model.SanctionOrderNo = sanctionModel.IMS_ORDER_NUMBER;
                    model.SanctionOrderDate = objCommonFuntion.GetDateTimeToString(sanctionModel.IMS_ORDER_DATE);
                }
                else
                {
                    model.BatchName = "Batch : " + model.Batch;
                    model.StateName = db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_NAME).FirstOrDefault();
                    model.CollaborationName = db.MASTER_FUNDING_AGENCY.Where(m => m.MAST_FUNDING_AGENCY_CODE == model.Stream).Select(m => m.MAST_FUNDING_AGENCY_NAME).FirstOrDefault();
                    model.SanctionOrderNo = "-";
                    model.SanctionOrderDate = "-";
                }



                Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
                rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);

                System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("State", model.State.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("District", "0"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Block", "0"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Year", model.Year.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Batch", model.Batch.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Agency", model.Agency.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Collaboration", model.Stream.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Status", model.ProposalType));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PMGSY", model.PMGSYScheme.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", model.StateName));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("BlockName", "-"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DistName", "-"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("BatchName", (model.Batch == null ? "-" : model.Batch.ToString())));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("CollaborationName", model.CollaborationName));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StatusName", "-"));
                //Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(@"ndcad\ommasadmin", "Ndcsp@123");
                Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Username"], System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Password"]);
                //Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(@"Administrator", "Admin@321");
                rview.ServerReport.ReportServerCredentials = irsc;
                rview.ServerReport.ReportPath = "/PMGSYCitizen/SanctionAbstractProposalList";
                //rview.ServerReport.ReportPath = "/SampleLocalizedReport/SanctionedProposalList";
                rview.ServerReport.SetParameters(paramList);
                string mimeType, encoding, extension, deviceInfo;
                string[] streamids;
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string format = "PDF"; //Desired format goes here (PDF, Excel, or Image)

                deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>True</SimplePageHeaders>" + "</DeviceInfo>";
                byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileName = (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.Year + "-" + (model.Year + 1)) + "_BATCH" + model.Batch + "_" + (model.CollaborationName) + "_SCHEME" + model.PMGSYScheme;
                fileName = fileName + ".pdf";
                Response.Clear();
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = fileName,
                    Inline = false,

                };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(bytes, "application/pdf");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PreviewDistrictAbstractReport()");
                return null;
            }
        }

        /// <summary>
        /// code for generation of pdf file from the ssrs report.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sanctionModel"></param>
        /// <returns></returns>
        public bool GenerateReport(SanctionOrderFilterModel model, IMS_SANCTIONED_PROJECTS_PDF sanctionModel)
        {
            try
            {
                Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
                rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);

                System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("State", model.State.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("District", "0"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Block", "0"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Year", model.Year.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Batch", model.Batch.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Agency", model.Agency.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Collaboration", model.Stream.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Status", "%"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PMGSY", model.PMGSYScheme.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", model.StateName));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("BatchName", model.Batch == null ? "-" : model.Batch.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("CollaborationName", model.CollaborationName));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("SanctionLetterNo", (model.SanctionOrderNo == null ? "-" : model.SanctionOrderNo)));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("SanctionDate", (model.SanctionOrderDate == null ? "-" : model.SanctionOrderDate)));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("UserId", PMGSY.Extensions.PMGSYSession.Current.UserId.ToString()));
                Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(@"ndcad\ommasadmin", "Ndcsp@123");
                rview.ServerReport.ReportServerCredentials = irsc;
                rview.ServerReport.ReportPath = "/PMGSYCitizen/SanctionedProposalList";
                rview.ServerReport.SetParameters(paramList);
                string mimeType, encoding, extension, deviceInfo;
                string[] streamids;
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string format = "PDF"; //Desired format goes here (PDF, Excel, or Image)
                deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>True</SimplePageHeaders>" + "</DeviceInfo>";
                byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileName = (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.Year + "-" + (model.Year + 1)) + "_BATCH" + model.Batch + "_" + (model.CollaborationName) + "_SCHEME" + model.PMGSYScheme;
                //string filePath = model.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"].ToString() + fileName + ".pdf" : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"].ToString() + fileName + ".pdf";
                string filePath = model.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"].ToString() + fileName + ".pdf" : model.PMGSYScheme == 2 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"].ToString() + fileName + ".pdf" : model.PMGSYScheme == 3 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_RCPLWE"].ToString() + fileName + ".pdf" : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYIII"].ToString() + fileName + ".pdf";
                Response.Clear();
                if (format == "PDF")
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-disposition", "filename=" + fileName + ".pdf");

                }

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = fileName,
                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                return true;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GenerateReport()");
                return false;
            }
        }

        #endregion

        #region REPACKAGING

        /// <summary>
        /// view for listing the proposals for repackaging
        /// </summary>
        /// <returns></returns>
        public ActionResult ListProposalForRepackaging()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                RepackagingFilterViewModel model = new RepackagingFilterViewModel();
                model.lstBatchs = objCommon.PopulateBatch();
                model.lstYears = new SelectList(objCommon.PopulateFinancialYear(true, true).ToList(), "Value", "Text").ToList();
                model.lstBlocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                model.lstCollaborations = objCommon.PopulateFundingAgency(true);
                List<SelectListItem> lstPackage = new List<SelectListItem>();
                lstPackage.Insert(0, new SelectListItem { Value = "0", Text = "All Packages" });
                model.lstPackages = lstPackage;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListProposalForRepackaging()");
                return null;
            }
        }

        /// <summary>
        /// returns the json for listing proposal details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetProposalListForRepackaging(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 30-Apr-2014 end
                objProposalBAL = new ProposalBAL();
                long totalRecords = 0;
                int year = 0;
                int block = 0;
                int batch = 0;
                string package = string.Empty;
                int collaboration = 0;
                string proposalType = string.Empty;
                string upgradationType = string.Empty;

                if (!(string.IsNullOrEmpty(Request.Params["YearCode"])))
                {
                    year = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BatchCode"])))
                {
                    batch = Convert.ToInt32(Request.Params["BatchCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BlockCode"])))
                {
                    block = Convert.ToInt32(Request.Params["BlockCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["Package"])))
                {
                    package = Request.Params["Package"];
                }

                if (!(string.IsNullOrEmpty(Request.Params["Collaboration"])))
                {
                    collaboration = Convert.ToInt32(Request.Params["Collaboration"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["ProposalType"])))
                {
                    proposalType = Request.Params["ProposalType"];
                }

                if (!(string.IsNullOrEmpty(Request.Params["UpgradationType"])))
                {
                    upgradationType = Request.Params["UpgradationType"];
                }

                var jsonData = new
                {
                    rows = objProposalBAL.GetProposalsForRepackaging(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, year, batch, block, package, collaboration, proposalType, upgradationType),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                return jsonResult;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetProposalListForRepackaging()");
                return null;
            }
        }

        /// <summary>
        /// returns the view for Repackaging
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult AddRepackagingDetails()
        {
            Dictionary<string, string> decryptedParameters = null;
            CommonFunctions objCommon = new CommonFunctions();
            ProposalDAL objDAL = new ProposalDAL();
            try
            {
                string id = string.Empty;
                if (!String.IsNullOrEmpty(Request.Params["ProposalCode"]))
                {
                    id = Request.Params["ProposalCode"];
                }
                string[] encryptedParams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                int proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                RepackagingDetailsViewModel model = new RepackagingDetailsViewModel();
                model = objDAL.GetRepackagingDetails(proposalCode);
                model.EncProposalCode = URLEncrypt.EncryptParameters(new string[] { "ProposalCode=" + decryptedParameters["ProposalCode"] });
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddRepackagingDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult PopulatePackages()
        {
            ProposalDAL objDAL = new ProposalDAL();
            string[] encryptedParams = null;
            string id = null;
            int Year = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["Year"]))
                {
                    Year = Convert.ToInt32(Request.Params["Year"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["ProposalCode"]))
                {
                    encryptedParams = Request.Params["ProposalCode"].Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });

                    //string[] arrParam = id.Split('$');
                    return Json(objDAL.PopulatePackages(Convert.ToInt32(decryptedParameters["ProposalCode"]), Year));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulatePackagesForRepackaging()");
                return null;
            }
        }

        /// <summary>
        /// post method for saving the repackaging details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRepackagingDetails(RepackagingDetailsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool status = objProposalBAL.AddRepackagingDetails(model);
                    if (status == true)
                    {
                        return Json(new { success = true, message = "Repackaging details added successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddRepackagingDetails(RepackagingDetailsViewModel model)");
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// populates the packages based on block , year and batch
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PopulatePackagesForRepackaging(string id)
        {
            ProposalDAL objDAL = new ProposalDAL();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    string[] arrParam = id.Split('$');
                    return Json(objDAL.PopulatePackagesForRepackaging(Convert.ToInt32(arrParam[0]), Convert.ToInt32(arrParam[1]), Convert.ToInt32(arrParam[2])));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulatePackagesForRepackaging()");
                return null;
            }
        }

        /// <summary>
        /// function for populating blocks
        /// </summary>
        /// <returns></returns>
        public JsonResult PopulateBlocks()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                int districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                return Json(objCommon.PopulateBlocks(districtCode, true));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateBlocks()");
                return null;
            }
        }

        #endregion

        #region DPR_LIST

        /// <summary>
        /// view for listing the DPR Proposals
        /// </summary>
        /// <returns></returns>
        public ActionResult ListDPRProposals()
        {
            try
            {
                ProposalDPRFilterViewModel proposalViewModel = new ProposalDPRFilterViewModel();
                proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;
                return View("ListDPRProposals", proposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListDPRProposals()");
                return null;
            }
        }

        public ActionResult EditYearDPRProposalLayout(int roadCode)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                SelectList lstYears = comm.PopulateFinancialYear(true, false);
                lstYears.ToList<SelectListItem>().Find(x => x.Value == "0").Value = "-1";
                ViewBag.Years = lstYears;
                ViewBag.RoadCode = roadCode;//Request.Params["roadCode"];
                return View("EditYearDPRProposalLayout");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditYearDPRProposalLayout()");
                return null;
            }
        }

        /// <summary>
        /// deletes the DPR Proposal
        /// </summary>
        /// <param name="proposalCode"></param>
        /// <returns></returns>
        public ActionResult UpdateDPRProposal(int proposalCode, int sanctionYear)
        {
            ProposalDAL propDAL = new DAL.Proposal.ProposalDAL();
            try
            {
                if (proposalCode <= 0)
                {
                    return Json(new { success = false, message = "Please select Proposal" });
                }
                if (sanctionYear <= 0)
                {
                    return Json(new { success = false, message = "Please select a valid Year" });
                }
                bool status = propDAL.UpdateYearDPRDAL(proposalCode, sanctionYear);
                if (status == true)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "DPR Proposal not updated" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateDPRProposal()");
                return null;
            }
        }

        public ActionResult DistrictDetailsforDPR(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
            if (PMGSYSession.Current.RoleCode == 22)
            {
                list.Find(x => x.Value == "-1").Text = "Select District";
            }
            else
            {
                list.Find(x => x.Value == "-1").Value = "0";
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// function for returning the list of DPR Proposals 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult DPRProposalList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                int year = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int stateCode = Convert.ToInt32(Request.Params["MAST_STATE_ID"]);
                int districtCode = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int blockCode = Convert.ToInt32(Request.Params["MAST_BLOCK_ID"]);
                int batch = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int colloaboaration = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string proposalType = Request.Params["IMS_PROPOSAL_TYPE"];
                string proposalStatus = Request.Params["IMS_PROPOSAL_STATUS"];
                string packageId = Request.Params["Package_Id"];
                string connectivity = Request.Params["IMS_UPGRADE_COONECT"];
                var jsonData = new
                {
                    rows = objProposalBAL.GetDPRProposalListBAL(stateCode, districtCode, blockCode, year, batch, colloaboaration, proposalType, proposalStatus, packageId, connectivity, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DPRProposalList()");
                return null;
            }
        }

        /// <summary>
        /// deletes the DPR Proposal
        /// </summary>
        /// <param name="proposalCode"></param>
        /// <returns></returns>
        public ActionResult DeleteDPRProposal(int proposalCode)
        {
            try
            {
                bool status = objProposalBAL.DeleteDPRProposalBAL(proposalCode);
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
                ErrorLog.LogError(ex, "DeleteDPRProposal()");
                return null;
            }
        }

        #endregion

        #region OLD_DATA_PROPOSAL_UPDATE

        /// <summary>
        /// main view for updating the old proposal details
        /// </summary>
        /// <returns></returns>
        public ActionResult ListProposalsForUpdate()
        {
            try
            {
                if (PMGSYSession.Current.StateCode == 35)
                {
                    ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
                    CommonFunctions objCommonFuntion = new CommonFunctions();
                    List<SelectListItem> lstTypes = new List<SelectListItem>();
                    lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                    lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                    lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                    proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
                    proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                    proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
                    proposalViewModel.Years = PopulateYear(0, true, true);
                    proposalViewModel.IMS_YEAR = DateTime.Now.Year;
                    proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;
                    return View(proposalViewModel);
                }
                else
                {
                    return null;
                }
            }
            
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListProposalsForUpdate()");
                return null;
            }
        }

        /// <summary>
        /// lists the proposals which do not have core network mapped
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult GetProposalsForUpdate(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, Convert.ToInt32(rows), sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int MAST_BLOCK_ID = Convert.ToInt32(Request.Params["MAST_BLOCK_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                long totalRecords;

                var jsonData = new
                {
                    rows = objProposalBAL.GetProposalsForUpdateBAL(page.Value - 1, rows, sidx, sord, out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, PMGSYSession.Current.AdminNdCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetProposalsForUpdate()");
                return null;
            }
        }

        /// <summary>
        /// view for selection of new proposal
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult UpdateProposalDetails(String parameter, String hash, String key)
        {
            try
            {
                ProposalUpdateViewModel model = new ProposalUpdateViewModel();

                int proposalCode = 0;
                string[] encryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                proposalCode = Convert.ToInt32(encryptedParameters[0]);
                model = objProposalBAL.GetOldProposalDetailsBAL(proposalCode);
                model.EncryptedProposalCode = URLEncrypt.EncryptParameters(new string[] { proposalCode.ToString() });
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateProposalDetails()");
                return null;
            }
        }

        /// <summary>
        /// updates the details of proposals - new core network mapped
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProposalDetails(ProposalUpdateViewModel model)
        {
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    bool status = objProposalBAL.UpdateProposalDetailsBAL(model, out message);
                    if (status == true)
                    {
                        return Json(new { success = true, message = "Proposal Updated Successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState).ToString() });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateProposalDetails(ProposalUpdateViewModel model)");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// changes the status of proposal from complete to staged
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult ChangeCompleteProposalToStaged(String parameter, String hash, String key)
        {
            try
            {
                string message = string.Empty;
                string[] encryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                int proposalCode = Convert.ToInt32(encryptedParameters[0]);
                bool status = objProposalBAL.ChangeCompleteProposalToStagedBAL(proposalCode, out message);
                if (status == true)
                {
                    return Json(new { success = true, errorMessage = message });
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ChangeCompleteProposalToStaged()");
                return Json(new { success = false, errorMessage = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// complete the status of staged proposal to complete
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult ChangeStagedProposalToComplete(String parameter, String hash, String key)
        {
            try
            {
                string message = string.Empty;
                string[] encryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                int proposalCode = Convert.ToInt32(encryptedParameters[0]);
                bool status = objProposalBAL.ChangeStagedProposalToCompleteBAL(proposalCode, out message);
                if (status == true)
                {
                    return Json(new { success = true, errorMessage = message });
                }
                else
                {
                    return Json(new { success = false, errorMessage = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ChangeStagedProposalToComplete()");
                return Json(new { success = false, errorMessage = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// view for changing the status of status of stage1 proposal to stage 2
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult ChangeStage1ProposalToStage2(String parameter, String hash, String key)
        {
            try
            {
                db = new PMGSYEntities();
                CommonFunctions objCommon = new CommonFunctions();
                Stage1ToStage2ViewModel model = new Stage1ToStage2ViewModel();
                ProposalDAL objDAL = new ProposalDAL();
                string[] decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                model.ProposalCode = Convert.ToInt32(decryptedParameters[0]);
                model.Batch = 0;
                IMS_SANCTIONED_PROJECTS imsMaster = objDAL.GetSanctionMaster(model.ProposalCode);
                model.AdminCode = imsMaster.MAST_DPIU_CODE;
                model.DistrictCode = imsMaster.MAST_DISTRICT_CODE;
                model.lstStagedYears = new SelectList(objCommon.PopulateFinancialYear(false, false).ToList(), "Value", "Text").ToList();
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ChangeStage1ProposalToStage2()");
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// changes the status of complete proposal to stage2
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult ChangeCompleteProposalToStage2(String parameter, String hash, String key)
        {
            try
            {
                db = new PMGSYEntities();
                CommonFunctions objCommon = new CommonFunctions();
                Stage1ToStage2ViewModel model = new Stage1ToStage2ViewModel();
                ProposalDAL objDAL = new ProposalDAL();
                string[] decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                model.ProposalCode = Convert.ToInt32(decryptedParameters[0]);
                model.Batch = 0;
                IMS_SANCTIONED_PROJECTS imsMaster = objDAL.GetSanctionMaster(model.ProposalCode);
                model.AdminCode = imsMaster.MAST_DPIU_CODE;
                model.DistrictCode = imsMaster.MAST_DISTRICT_CODE;
                model.lstStagedYears = new SelectList(objCommon.PopulateFinancialYear(false, false).ToList(), "Value", "Text").ToList();
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ChangeCompleteProposalToStage2()");
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// main view for mapping the core networks to proposal
        /// </summary>
        /// <returns></returns>
        public ActionResult ListProposalsForCNMapping()
        {
            try
            {
                ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
                CommonFunctions objCommonFuntion = new CommonFunctions();
                List<SelectListItem> lstTypes = new List<SelectListItem>();
                lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
                proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
                proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                if (PMGSYSession.Current.RoleCode == 22)
                {
                    proposalViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                }
                proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                proposalViewModel.CONNECTIVITYLIST = lstTypes;
                proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                proposalViewModel.BATCHS.RemoveAt(0);
                proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
                proposalViewModel.Years = PopulateYear(0, true, true);
                proposalViewModel.IMS_YEAR = DateTime.Now.Year;
                proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;
                return View(proposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListProposalsForCNMapping()");
                return null;
            }
        }

        /// <summary>
        /// returns view for selection of core network according to particular block 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult MapCoreNetworkToProposals(String parameter, String hash, String key)
        {
            string[] decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                ProposalDAL objProposalDAL = new ProposalDAL();
                CoreNetworkMappingViewModel model = new CoreNetworkMappingViewModel();
                model.ProposalCode = Convert.ToInt32(decryptedParameters[0]);
                model = objProposalDAL.GetProposalDetails(model.ProposalCode);
                model.lstBlocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode);
                model.lstCoreNetworks = PopulateLinkThrough(model.Block, model.UpgradeConnect, model.ProposalType);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MapCoreNetworkToProposals()");
                return null;
            }
        }

        /// <summary>
        /// returns the list of proposals which do not have core network mapped.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult GetProposalsForCNMapping(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, Convert.ToInt32(rows), sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_BLOCK_ID = Convert.ToInt32(Request.Params["MAST_BLOCK_ID"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                long totalRecords;

                var jsonData = new
                {
                    rows = objProposalBAL.GetProposalsForCNMappingBAL(page.Value - 1, rows, sidx, sord, out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, PMGSYSession.Current.AdminNdCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetProposalsForCNMapping()");
                return null;
            }
        }

        /// <summary>
        /// updates the details of proposals --- new core network mapped.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MapCoreNetworkDetails(CoreNetworkMappingViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool status = objProposalBAL.MapCoreNetworkDetailsBAL(model);
                    if (status == true)
                    {
                        return Json(new { success = true, message = "Proposal Updated Successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MapCoreNetworkDetails()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// changes the status of proposal from stage1 to stage2
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStage1ProposalToStage2(Stage1ToStage2ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool status = objProposalBAL.ChangeStage1ProposalToStage2BAL(model);
                    if (status == true)
                    {
                        return Json(new { success = true, message = "Proposal Updated Successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ChangeStage1ProposalToStage2(Stage1ToStage2ViewModel model)");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// changes the status of proposal from complete to stage2
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeCompleteProposalsToStage2(Stage1ToStage2ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool status = objProposalBAL.ChangeCompleteProposalsToStage2BAL(model);
                    if (status == true)
                    {
                        return Json(new { success = true, message = "Proposal Updated Successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ChangeCompleteProposalsToStage2(Stage1ToStage2ViewModel model)");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the view for PIU updation
        /// </summary>
        /// <returns></returns>
        //public ActionResult ProposalPIUUpdate(string id, int district)
        //{
        //    ProposalPIUUpdateViewModel model = new ProposalPIUUpdateViewModel();
        //    ProposalDAL objDAL = new ProposalDAL();
        //    try
        //    {
        //        if (!String.IsNullOrEmpty(id))
        //        {
        //            model.ProposalArray = id;
        //            model.lstPIU = objDAL.PopulatePIUOfDistrict(district);
        //        }
        //        return PartialView(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "ProposalPIUUpdate()");
        //        return null;
        //    }
        //}

        //public ActionResult ProposalPIUUpdate(string id, int district)
        //{
        //    ProposalPIUUpdateViewModel_New model = new ProposalPIUUpdateViewModel_New();
        //    ProposalDAL objDAL = new ProposalDAL();
        //    try
        //    {
        //        if (!String.IsNullOrEmpty(id))
        //        {
        //            model.PackageId = id;
        //            model.lstPIU = objDAL.PopulatePIUOfDistrict(district);
        //        }
        //        return PartialView(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "ProposalPIUUpdate()");
        //        return null;
        //    }
        //}


        public ActionResult ProposalPIUUpdate(int district, string id)   // change 
        {
            ProposalPIUUpdateViewModel_New model = new ProposalPIUUpdateViewModel_New();
            ProposalDAL objDAL = new ProposalDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    if (district != 0)
                    {
                        int RoadCode = Convert.ToInt32(id);  // change 
                        model.DistrictCode = district;   // change 
                        model.PackageId = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();   // change
                        model.lstPIU = objDAL.PopulatePIUOfDistrict(district);
                    }
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalPIUUpdate()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult UpdateProposalPIUDetails(ProposalPIUUpdateViewModel model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (objProposalBAL.UpdateProposalPIUDetailsBAL(model))
        //            {
        //                return Json(new { success = true, message = "Proposal Details Updated Successfully." });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = "Error occurred while processing your request." });
        //            }
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Error occurred while processing your request." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "UpdateProposalPIUDetails()");
        //        return Json(new { success = false, message = "Error occurred while processing your request." });
        //    }
        //}

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UpdateProposalPIUDetails(ProposalPIUUpdateViewModel_New model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string result = objProposalBAL.UpdateProposalPIUDetailsBAL(model);

                    if (result == "success")
                    {
                        return Json(new { success = true, message = "Proposal Details Updated Successfully." });
                    }
                    else if (result == "fail")
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });

                    }
                    else if (result == "1")
                    {
                        return Json(new { success = false, message = "This Package found in Multiple Scheme" });

                    }
                    else if (result == "2")
                    {
                        return Json(new { success = false, message = "This Package found in Multiple PIU" });

                    }
                    else if (result == "3")
                    {
                        return Json(new { success = false, message = "This Package found in Multiple Years" });

                    }
                    else if (result == "4")
                    {
                        return Json(new { success = false, message = "This Package has been modified,already" });

                    }
                    else if (result == "5")
                    {
                        return Json(new { success = false, message = "This Package found in Multiple Districts" });

                    }  // change
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });

                    }
                }
                else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateProposalPIUDetails()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the view for PIU updation
        /// </summary>
        /// <returns></returns>
        public ActionResult ProposalUpdateBlock(string id, int district)
        {
            ProposalUpdateBlockViewModel model = new ProposalUpdateBlockViewModel();
            ProposalDAL objDAL = new ProposalDAL();
            string districtName = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    model.ProposalArray = id;
                    model.lstBlock = objDAL.PopulateBlockOfDistrict(district, out districtName);
                    model.DistrictName = districtName;
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalUpdateBlock()");
                return null;
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UpdateProposalBlockDetails(ProposalUpdateBlockViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (objProposalBAL.UpdateProposalBlockDetailsBAL(model))
                    {
                        return Json(new { success = true, message = "Proposal Details Updated Successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    string message = "";
                    bool flag = false;
                    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;

                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateProposalBlockDetails()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }
        #endregion

        #region CHANGING_CORE_NETWORKS

        /// <summary>
        /// form for updating the core network
        /// </summary>
        /// <returns></returns>
        public ActionResult ProposalCoreNetworkUpdate(String parameter, String hash, String key)
        {
            ProposalDAL objProposalDAL = new ProposalDAL();
            try
            {
                CoreNetworkUpdateViewModel model = new CoreNetworkUpdateViewModel();
                string[] parameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                CoreNetworkMappingViewModel cnMapModel = objProposalDAL.GetProposalDetails(Convert.ToInt32(parameters[0]));
                model.Block = cnMapModel.Block;
                model.lstCoreNetworks = PopulateLinkThrough(model.Block, cnMapModel.UpgradeConnect, cnMapModel.ProposalType);
                List<SelectListItem> lstPrevCorenetwork = PopulateOnlyLinkThrough(cnMapModel.CnCode, cnMapModel.Block, cnMapModel.UpgradeConnect);
                foreach (var item in lstPrevCorenetwork)
                {
                    if (model.lstCoreNetworks.Any(m => m.Value == item.Value))
                    {

                    }
                    else
                    {
                        model.lstCoreNetworks.Add(item);
                    }
                }
                model.CnCode = cnMapModel.CnCode;
                model.EncryptedProposalCode = parameter + "/" + hash + "/" + key;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalCoreNetworkUpdate()");
                return null;
            }
        }

        /// <summary>
        /// updates the core network details
        /// </summary>
        /// <param name="model">contains the updated form details</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCoreNetworkDetails(CoreNetworkUpdateViewModel model)
        {
            objProposalBAL = new ProposalBAL();
            try
            {
                if (ModelState.IsValid)
                {
                    if (objProposalBAL.UpdateCoreNetworkDetailsBAL(model))
                    {
                        return Json(new { success = true, message = "Core Network details updated successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateCoreNetworkDetails()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// partial view for updating the block details of proposal
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult ProposalBlockUpdate(String parameter, String hash, String key)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                ProposalBlockUpdateViewModel model = new ProposalBlockUpdateViewModel();
                string[] parameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                model.lstDistricts = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                model.lstBlocks.Insert(0, new SelectListItem { Value = "0", Text = "Select Block" });
                model.lstPIU.Insert(0, new SelectListItem { Value = "0", Text = "Select PIU" });
                model.ProposalCode = Convert.ToInt32(parameters[0]);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalBlockUpdate()");
                return null;
            }
        }

        /// <summary>
        /// updates the block details of proposal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateBlockDetails(ProposalBlockUpdateViewModel model)
        {
            objProposalBAL = new ProposalBAL();
            try
            {
                if (ModelState.IsValid)
                {
                    if (objProposalBAL.UpdateBlockDetailsBAL(model))
                    {
                        return Json(new { success = true, message = "Block details updated successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateBlockDetails()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the PIU of district
        /// </summary>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public ActionResult PopulatePIUOfDistrict(int districtCode)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                TransactionParams objParam = new TransactionParams();
                objParam.DISTRICT_CODE = districtCode;
                return Json(objCommon.PopulateDPIU(objParam));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulatePIUOfDistrict()");
                return null;
            }
        }

        #endregion

        #region Proposal Additional  Cost
        /// <summary>
        /// returns the list view of execution progress details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListProposalAdditionalCostDetail()
        {
            CommonFunctions objCommon = new CommonFunctions();
            ProposalAdditionalCostFilterViewModel proposalModel = new ProposalAdditionalCostFilterViewModel();
            List<SelectListItem> lstBatches = new List<SelectListItem>();
            TransactionParams transactionParams = new TransactionParams();
            transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
            transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            transactionParams.ISSearch = true;
            transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;
            lstBatches = objCommon.PopulateBatch();
            lstBatches.RemoveAt(0);
            lstBatches.Add(new SelectListItem { Value = "0", Text = "All Batches" });

            proposalModel.States = objCommon.PopulateStates(false);
            proposalModel.States.Insert(0, (new SelectListItem { Text = "All States", Value = "0" }));
            proposalModel.MAST_State_CODE = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            proposalModel.States.Find(x => x.Value == proposalModel.MAST_State_CODE.ToString()).Selected = true;

            proposalModel.Districts = new List<SelectListItem>();
            if (proposalModel.MAST_State_CODE == 0)
            {
                proposalModel.Districts.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                proposalModel.Districts = objCommon.PopulateDistrict(proposalModel.MAST_State_CODE, true);
                proposalModel.MAST_District_CODE = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                proposalModel.Districts.Find(x => x.Value == "-1").Value = "0";
                proposalModel.Districts.Find(x => x.Value == proposalModel.MAST_District_CODE.ToString()).Selected = true;

            }
            proposalModel.BLOCKS = new List<SelectListItem>();
            if (proposalModel.MAST_District_CODE == 0)
            {
                proposalModel.BLOCKS.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                proposalModel.BLOCKS = objCommon.PopulateBlocks(proposalModel.MAST_District_CODE, true);
                proposalModel.BLOCKS.Find(x => x.Value == "-1").Value = "0";
                //BlockCode = PMGSYSession.Current.BlockCode == 0 ? 0 : PMGSYSession.Current.BlockCode;
                //BlockList.Find(x => x.Value == BlockCode.ToString()).Selected = true;
            }
            // proposalModel.BATCHS = lstBatches;
            // proposalModel.BLOCKS = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
            proposalModel.Years = PopulateYear(0, true, true);
            proposalModel.Years.Find(x => x.Value == "-1").Value = "0";
            proposalModel.STREAMS = objCommon.PopulateStreams("", true);
            proposalModel.PROPOSAL_TYPES = objCommon.PopulateProposalTypes();

            //new filters added by Vikram 
            proposalModel.lstBatchs = objCommon.PopulateBatch(true);
            proposalModel.lstCollaborations = objCommon.PopulateFundingAgency(true);
            proposalModel.lstUpgradations = objCommon.PopulateNewUpgradeList(true);
            //end of change

            List<SelectListItem> lstPackages = objCommon.PopulatePackage(transactionParams);
            lstPackages.RemoveAt(0);
            lstPackages.Insert(0, new SelectListItem { Value = "All", Text = "All Packages" });
            proposalModel.PACKAGES = lstPackages;
            return View("ListProposalAdditionalCostDetail", proposalModel);
        }

        /// <summary>
        /// returns the list of execution progress details 
        /// </summary>
        /// <param name="executionCollection">contains the filters and grid parameters</param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetProposalAdditionalCostList(int? page, int? rows, string sidx, string sord)
        {
            int yearCode = 0;
            int blockCode = 0;
            int streamCollaborationCode = 0;
            int batchCode = 0;
            int stateCode = 0;
            int districtCode = 0;
            string proposalCode = string.Empty;
            string packageCode = string.Empty;
            long totalRecords = 0;
            string upgradationType = string.Empty;

            IProposalBAL objBAL = new ProposalBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
            {
                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
            {
                districtCode = Convert.ToInt32(Request.Params["districtCode"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
            {
                yearCode = Convert.ToInt32(Request.Params["yearCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
            {
                blockCode = Convert.ToInt32(Request.Params["blockCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
            {
                streamCollaborationCode = Convert.ToInt32(Request.Params["collaboration"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
            {
                batchCode = Convert.ToInt32(Request.Params["batchCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
            {
                proposalCode = Request.Params["proposalCode"];
            }

            if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
            {
                packageCode = Request.Params["packageCode"];
            }

            if (!(string.IsNullOrEmpty(Request.Params["UpgradationType"])))
            {
                upgradationType = Request.Params["UpgradationType"];
            }

            var jsonData = new
            {
                rows = objBAL.GetProposalAdditionalCostListBAL(stateCode, districtCode, blockCode, yearCode, packageCode, proposalCode, batchCode, streamCollaborationCode, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Audit]
        public ActionResult AdditionalCostDetails(string id)
        {

            try
            {
                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = null;
                IProposalBAL objProposalBAL = new ProposalBAL();

                ProposalAdditionalCostModel proposalAdditionalCostModel = new ProposalAdditionalCostModel();
                Dictionary<string, string> decryptedParameters = null;
                string[] encryptedParams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                int proposalCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"]);
                ims_sanctioned_projects = objProposalBAL.GetRoadDetails(proposalCode);
                proposalAdditionalCostModel.IMS_PR_ROAD_CODE = proposalCode;
                proposalAdditionalCostModel.EncryptedRoadCode = id;
                //set Road Details                                                            
                proposalAdditionalCostModel.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                proposalAdditionalCostModel.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                proposalAdditionalCostModel.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
                proposalAdditionalCostModel.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                proposalAdditionalCostModel.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;
                proposalAdditionalCostModel.IMS_PR_ROAD_CODE = proposalCode;
                proposalAdditionalCostModel.IMS_STATE_AMOUNT_TEXT = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT);
                proposalAdditionalCostModel.IMS_MORD_AMOUNT_TEXT = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT);
                return View(proposalAdditionalCostModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AdditionalCostDetails()");
                return View(new TestResultViewModel());
            }
        }

        [Audit]
        public ActionResult GetAdditionalCostList(FormCollection formCollection)
        {

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            long totalRecords = 0;
            int IMS_PR_ROAD_CODE = 0;

            IProposalBAL objProposalBAL = new ProposalBAL();

            try
            {


                if (!string.IsNullOrEmpty(formCollection["IMS_PR_ROAD_CODE"]))
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"]);
                }


                var jsonData = new
                {
                    rows = objProposalBAL.GetAdditionalCostListBAL(IMS_PR_ROAD_CODE, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetAdditionalCostList()");
                return null;
            }
        }

        /// <summary>
        /// Add Test result details
        /// </summary>
        /// <param name="proposalAdditionalCostModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdditionalCostDetails(FormCollection formCollection) //ProposalAdditionalCostModel proposalAdditionalCostModel
        {
            string message = string.Empty;
            IProposalBAL objProposalBAL = new ProposalBAL();

            try
            {
                HttpPostedFileBase ClearancePdfFile = Request.Files["fileLetter"];
                string fileTypes = string.Empty;
                string[] arrfiletype = new string[5];
                bool fileExt = false;
                string filename = string.Empty;
                string filePdfSaveExt = string.Empty;
                string filePathPdf = string.Empty;
                bool status = false;

                if (ClearancePdfFile != null)
                {
                    fileTypes = ConfigurationManager.AppSettings["ADDITIONAL_COST_PDF_FORMAT"];


                    //if (fileTypes == ClearancePdfFile.FileName.Split('.')[1])
                    if (fileTypes == Path.GetExtension(ClearancePdfFile.FileName.Trim()).Split('.')[1])
                    {
                        fileExt = true;
                        filePdfSaveExt = fileTypes;
                        filePathPdf = ConfigurationManager.AppSettings["ADDITIONAL_COST_PDF_UPLOAD"];
                    }

                    if (fileExt == false)
                    {
                        message = "Sanction Letter File type is not allowed.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "Please upload file.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                ProposalAdditionalCostModel proposalAdditionalCostModel = new ProposalAdditionalCostModel();
                proposalAdditionalCostModel.IMS_BATCH = Convert.ToInt32(formCollection["IMS_BATCH"]);
                proposalAdditionalCostModel.IMS_LETTER_NO = formCollection["IMS_LETTER_NO"];
                proposalAdditionalCostModel.IMS_MORD_AMOUNT = Convert.ToDecimal(formCollection["IMS_MORD_AMOUNT"]);
                proposalAdditionalCostModel.IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"]);
                proposalAdditionalCostModel.IMS_RELEASE_DATE = formCollection["IMS_RELEASE_DATE"];
                proposalAdditionalCostModel.IMS_STATE_AMOUNT = Convert.ToDecimal(formCollection["IMS_STATE_AMOUNT"]);
                proposalAdditionalCostModel.TOTAL_AMOUNT = Convert.ToDecimal(formCollection["TOTAL_AMOUNT"]);

                if (ClearancePdfFile != null)
                {
                    proposalAdditionalCostModel.IMS_FILE_NAME = ClearancePdfFile.FileName;
                    Request.Files["fileLetter"].SaveAs(Path.Combine(filePathPdf, ClearancePdfFile.FileName));
                }

                if (ModelState.IsValid)
                {

                    if (objProposalBAL.AddAdditionalCostDetailsBAL(proposalAdditionalCostModel, ref message))
                    {
                        message = message == string.Empty ? "Additional Cost Details saved successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        message = message == string.Empty ? "Additional Cost Details not saved." : message;
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {

                    string messages = string.Join("; ", ModelState.Values
                                           .SelectMany(x => x.Errors)
                                           .Select(x => x.ErrorMessage));

                    return PartialView("AdditionalCostDetails", proposalAdditionalCostModel);
                }
            }
            catch (DbEntityValidationException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddAdditionalCostDetails(DbEntityValidationException ex)");
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddAdditionalCostDetails()");
                message = "Additional Cost details not saved because " + ex.Message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get Test result Details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditAdditionalCostDetails(String parameter, String hash, String key)
        {
            try
            {
                IProposalBAL objProposalBAL = new ProposalBAL();

                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = null;

                Dictionary<string, string> decryptedParameters = null;

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int transationCode = Convert.ToInt32(decryptedParameters["TransactionCode"].ToString());
                    int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"].ToString());

                    ProposalAdditionalCostModel proposalAdditionalCostModel = objProposalBAL.EditAdditionalCostDetailsBAL(transationCode, imsPrRoadCode);
                    proposalAdditionalCostModel.IMS_PR_ROAD_CODE = imsPrRoadCode;

                    if (proposalAdditionalCostModel == null)
                    {
                        ModelState.AddModelError("", "Test Result Details not exist.");
                        return PartialView("AdditionalCostDetails", new TestResultViewModel());
                    }

                    //get Road Details
                    ims_sanctioned_projects = objProposalBAL.GetRoadDetails(imsPrRoadCode);

                    //set Road Details                                                            
                    proposalAdditionalCostModel.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                    proposalAdditionalCostModel.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                    proposalAdditionalCostModel.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
                    proposalAdditionalCostModel.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                    proposalAdditionalCostModel.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;

                    return PartialView("AdditionalCostDetails", proposalAdditionalCostModel);
                }
                return PartialView("AdditionalCostDetails", new ProposalAdditionalCostModel());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditAdditionalCostDetails()");
                ModelState.AddModelError("", "Additional Cost details not exist.");
                return PartialView("AdditionalCostDetails", new ProposalAdditionalCostModel());
            }
        }


        /// <summary>
        /// Update Test Result details.
        /// </summary>
        /// <param name="proposalAdditionalCostModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult UpdateAdditionalCostDetails(FormCollection formCollection)
        {
            string message = string.Empty;
            try
            {

                ProposalAdditionalCostModel proposalAdditionalCostModel = new ProposalAdditionalCostModel();
                proposalAdditionalCostModel.IMS_BATCH = Convert.ToInt32(formCollection["IMS_BATCH"]);
                proposalAdditionalCostModel.IMS_LETTER_NO = formCollection["IMS_LETTER_NO"];
                proposalAdditionalCostModel.IMS_MORD_AMOUNT = Convert.ToDecimal(formCollection["IMS_MORD_AMOUNT"]);
                proposalAdditionalCostModel.IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"]);
                proposalAdditionalCostModel.IMS_RELEASE_DATE = formCollection["IMS_RELEASE_DATE"];
                proposalAdditionalCostModel.IMS_STATE_AMOUNT = Convert.ToDecimal(formCollection["IMS_STATE_AMOUNT"]);
                proposalAdditionalCostModel.TOTAL_AMOUNT = Convert.ToDecimal(formCollection["TOTAL_AMOUNT"]);
                proposalAdditionalCostModel.EncryptedRoadCode = formCollection["EncryptedRoadCode"];
                proposalAdditionalCostModel.EncryptedTransactionRoadCode = formCollection["EncryptedTransactionRoadCode"];

                ModelState.Remove("IMS_FILE_NAME");

                if (ModelState.IsValid)
                {
                    IProposalBAL objProposalBAL = new ProposalBAL();

                    if (objProposalBAL.UpdateAdditionalCostDetailsBAL(proposalAdditionalCostModel, ref message))
                    {
                        message = message == string.Empty ? "Additional Cost details Updated successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        message = message == string.Empty ? "Additional Cost details not updated." : message;
                        return Json(new { success = false, message = message });
                    }

                }
                else
                {
                    message = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                        );

                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (DbEntityValidationException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "UpdateAdditionalCostDetails(DbEntityValidationException ex)");
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "UpdateAdditionalCostDetails()");
                message = "Additional Cost details not saved because " + ex.Message;
                return Json(new { succes = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete Test Result details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteAdditionalCostDetails(string parameter, string hash, string key)
        {
            string message = string.Empty;

            try
            {
                Dictionary<string, string> decryptedParameters = null;

                IProposalBAL objProposalBAL = new ProposalBAL();

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int transactionCode = Convert.ToInt32(decryptedParameters["TransactionCode"].ToString());
                    int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"].ToString());

                    if (objProposalBAL.DeleteAdditionalCostDetailsBAL(transactionCode, imsPrRoadCode, ref message))
                    {
                        message = message == string.Empty ? "Additional Cost details deleted successfully." : message;

                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Additional Cost details not deleted." : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                message = "An error occured while processing you request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteAdditionalCostDetails()");
                message = "An error occured while proccessing your request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Audit]
        public ActionResult DownloadAdditionalCostFile(string parameter, string hash, string key)
        {
            try
            {
                string FileName = string.Empty;
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
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".pdf")
                {
                    FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["ADDITIONAL_COST_PDF_UPLOAD"], FileName);
                }

                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DownloadAdditionalCostFile()");
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        #region STA Payment
        /// <summary>
        /// returns the list view of execution progress details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListSTAPAYMENTDetail()
        {
            CommonFunctions objCommon = new CommonFunctions();
            ProposalAdditionalCostFilterViewModel proposalModel = new ProposalAdditionalCostFilterViewModel();
            List<SelectListItem> lstBatches = new List<SelectListItem>();
            TransactionParams transactionParams = new TransactionParams();
            transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
            transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            transactionParams.ISSearch = true;
            transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;
            lstBatches = objCommon.PopulateBatch();
            lstBatches.RemoveAt(0);
            lstBatches.Add(new SelectListItem { Value = "0", Text = "All Batches" });

            proposalModel.States = objCommon.PopulateStates(false);
            proposalModel.States.Insert(0, (new SelectListItem { Text = "All States", Value = "0" }));
            proposalModel.MAST_State_CODE = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            proposalModel.States.Find(x => x.Value == proposalModel.MAST_State_CODE.ToString()).Selected = true;

            proposalModel.Districts = new List<SelectListItem>();
            if (proposalModel.MAST_State_CODE == 0)
            {
                proposalModel.Districts.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                proposalModel.Districts = objCommon.PopulateDistrict(proposalModel.MAST_State_CODE, true);
                proposalModel.MAST_District_CODE = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                proposalModel.Districts.Find(x => x.Value == "-1").Value = "0";
                proposalModel.Districts.Find(x => x.Value == proposalModel.MAST_District_CODE.ToString()).Selected = true;

            }
            proposalModel.BLOCKS = new List<SelectListItem>();
            if (proposalModel.MAST_District_CODE == 0)
            {
                proposalModel.BLOCKS.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                proposalModel.BLOCKS = objCommon.PopulateBlocks(proposalModel.MAST_District_CODE, true);
                proposalModel.BLOCKS.Find(x => x.Value == "-1").Value = "0";
                //BlockCode = PMGSYSession.Current.BlockCode == 0 ? 0 : PMGSYSession.Current.BlockCode;
                //BlockList.Find(x => x.Value == BlockCode.ToString()).Selected = true;
            }
            // proposalModel.BATCHS = lstBatches;
            // proposalModel.BLOCKS = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
            proposalModel.Years = PopulateYear(0, true, true);
            proposalModel.Years.Find(x => x.Value == "-1").Value = "0";
            proposalModel.STREAMS = objCommon.PopulateStreams("", true);
            proposalModel.PROPOSAL_TYPES = objCommon.PopulateProposalTypes();

            //new filters added by Vikram 
            proposalModel.lstBatchs = objCommon.PopulateBatch(true);
            proposalModel.lstCollaborations = objCommon.PopulateFundingAgency(true);
            proposalModel.lstUpgradations = objCommon.PopulateNewUpgradeList(true);
            //end of change

            List<SelectListItem> lstPackages = objCommon.PopulatePackage(transactionParams);
            lstPackages.RemoveAt(0);
            lstPackages.Insert(0, new SelectListItem { Value = "All", Text = "All Packages" });
            proposalModel.PACKAGES = lstPackages;
            return View("ListProposalAdditionalCostDetail", proposalModel);
        }

        /// <summary>
        /// returns the list of execution progress details 
        /// </summary>
        /// <param name="executionCollection">contains the filters and grid parameters</param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetSTAPaymentProposalList(int? page, int? rows, string sidx, string sord)
        {
            int yearCode = 0;
            int blockCode = 0;
            int streamCollaborationCode = 0;
            int batchCode = 0;
            int stateCode = 0;
            int districtCode = 0;
            string proposalCode = string.Empty;
            string packageCode = string.Empty;
            long totalRecords = 0;
            string upgradationType = string.Empty;

            IProposalBAL objBAL = new ProposalBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
            {
                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
            {
                districtCode = Convert.ToInt32(Request.Params["districtCode"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
            {
                yearCode = Convert.ToInt32(Request.Params["yearCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
            {
                blockCode = Convert.ToInt32(Request.Params["blockCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
            {
                streamCollaborationCode = Convert.ToInt32(Request.Params["collaboration"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
            {
                batchCode = Convert.ToInt32(Request.Params["batchCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
            {
                proposalCode = Request.Params["proposalCode"];
            }

            if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
            {
                packageCode = Request.Params["packageCode"];
            }

            if (!(string.IsNullOrEmpty(Request.Params["UpgradationType"])))
            {
                upgradationType = Request.Params["UpgradationType"];
            }

            var jsonData = new
            {
                rows = objBAL.GetProposalAdditionalCostListBAL(stateCode, districtCode, blockCode, yearCode, packageCode, proposalCode, batchCode, streamCollaborationCode, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Audit]
        public ActionResult STAPaymentDetails(string id)
        {

            try
            {
                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = null;
                IProposalBAL objProposalBAL = new ProposalBAL();

                ProposalAdditionalCostModel proposalAdditionalCostModel = new ProposalAdditionalCostModel();
                Dictionary<string, string> decryptedParameters = null;
                string[] encryptedParams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                int proposalCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"]);
                ims_sanctioned_projects = objProposalBAL.GetRoadDetails(proposalCode);
                proposalAdditionalCostModel.IMS_PR_ROAD_CODE = proposalCode;
                proposalAdditionalCostModel.EncryptedRoadCode = id;
                //set Road Details                                                            
                proposalAdditionalCostModel.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                proposalAdditionalCostModel.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                proposalAdditionalCostModel.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
                proposalAdditionalCostModel.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                proposalAdditionalCostModel.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;
                proposalAdditionalCostModel.IMS_PR_ROAD_CODE = proposalCode;
                proposalAdditionalCostModel.IMS_STATE_AMOUNT_TEXT = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT);
                proposalAdditionalCostModel.IMS_MORD_AMOUNT_TEXT = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT);
                return View(proposalAdditionalCostModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "STAPaymentDetails()");
                return View(new TestResultViewModel());
            }
        }

        [Audit]
        public ActionResult GetSTAPaymentList(FormCollection formCollection)
        {

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            long totalRecords = 0;
            int IMS_PR_ROAD_CODE = 0;

            IProposalBAL objProposalBAL = new ProposalBAL();

            try
            {


                if (!string.IsNullOrEmpty(formCollection["IMS_PR_ROAD_CODE"]))
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"]);
                }


                var jsonData = new
                {
                    rows = objProposalBAL.GetAdditionalCostListBAL(IMS_PR_ROAD_CODE, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetSTAPaymentList()");
                return null;
            }
        }

        /// <summary>
        /// Add Test result details
        /// </summary>
        /// <param name="proposalAdditionalCostModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddSTAPaymentDetails(ProposalAdditionalCostModel proposalAdditionalCostModel)
        {
            string message = string.Empty;
            IProposalBAL objProposalBAL = new ProposalBAL();

            try
            {
                if (ModelState.IsValid)
                {

                    if (objProposalBAL.AddAdditionalCostDetailsBAL(proposalAdditionalCostModel, ref message))
                    {
                        message = message == string.Empty ? "STA Payment details saved successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        message = message == string.Empty ? "STA Payment details not saved." : message;
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {

                    string messages = string.Join("; ", ModelState.Values
                                           .SelectMany(x => x.Errors)
                                           .Select(x => x.ErrorMessage));

                    return PartialView("AdditionalCostDetails", proposalAdditionalCostModel);
                }
            }
            catch (DbEntityValidationException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddSTAPaymentDetails(DbEntityValidationException ex)");
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddSTAPaymentDetails()");
                message = "STA Payment details not saved because " + ex.Message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get Test result Details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditSTAPaymentDetails(String parameter, String hash, String key)
        {
            try
            {
                IProposalBAL objProposalBAL = new ProposalBAL();

                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = null;

                Dictionary<string, string> decryptedParameters = null;

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int transationCode = Convert.ToInt32(decryptedParameters["TransactionCode"].ToString());
                    int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"].ToString());

                    ProposalAdditionalCostModel proposalAdditionalCostModel = objProposalBAL.EditAdditionalCostDetailsBAL(transationCode, imsPrRoadCode);
                    proposalAdditionalCostModel.IMS_PR_ROAD_CODE = imsPrRoadCode;

                    if (proposalAdditionalCostModel == null)
                    {
                        ModelState.AddModelError("", "STA Payment Details not exist.");
                        return PartialView("AdditionalCostDetails", new TestResultViewModel());
                    }

                    //get Road Details
                    ims_sanctioned_projects = objProposalBAL.GetRoadDetails(imsPrRoadCode);

                    //set Road Details                                                            
                    proposalAdditionalCostModel.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                    proposalAdditionalCostModel.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                    proposalAdditionalCostModel.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
                    proposalAdditionalCostModel.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                    proposalAdditionalCostModel.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;

                    return PartialView("AdditionalCostDetails", proposalAdditionalCostModel);
                }
                return PartialView("AdditionalCostDetails", new ProposalAdditionalCostModel());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditSTAPaymentDetails()");
                ModelState.AddModelError("", "STA Payment details not exist.");
                return PartialView("AdditionalCostDetails", new ProposalAdditionalCostModel());
            }
        }


        /// <summary>
        /// Update Test Result details.
        /// </summary>
        /// <param name="proposalAdditionalCostModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult UpdateSTAPaymentDetails(ProposalAdditionalCostModel proposalAdditionalCostModel)
        {
            string message = string.Empty;
            try
            {

                if (ModelState.IsValid)
                {
                    IProposalBAL objProposalBAL = new ProposalBAL();

                    if (objProposalBAL.UpdateAdditionalCostDetailsBAL(proposalAdditionalCostModel, ref message))
                    {
                        message = message == string.Empty ? "STA Payment details Updated successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        message = message == string.Empty ? "STA Payment details not updated." : message;
                        return Json(new { success = false, message = message });
                    }

                }
                else
                {
                    message = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                        );

                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (DbEntityValidationException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "UpdateSTAPaymentDetails(DbEntityValidationException ex)");
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "UpdateSTAPaymentDetails()");
                message = "Additional Cost details not saved because " + ex.Message;
                return Json(new { succes = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete Test Result details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteSTAPaymentDetails(string parameter, string hash, string key)
        {
            string message = string.Empty;

            try
            {
                Dictionary<string, string> decryptedParameters = null;

                IProposalBAL objProposalBAL = new ProposalBAL();

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int transactionCode = Convert.ToInt32(decryptedParameters["TransactionCode"].ToString());
                    int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"].ToString());

                    if (objProposalBAL.DeleteAdditionalCostDetailsBAL(transactionCode, imsPrRoadCode, ref message))
                    {
                        message = message == string.Empty ? "STA Payment details deleted successfully." : message;

                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "STA Payment details not deleted." : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                message = "An error occured while processing you request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteSTAPaymentDetails()");
                message = "An error occured while proccessing your request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region MRD CLEARANCE LETTERS Upload Sanction Order
        [HttpGet]
        public ActionResult ListMrdClearenceLetter()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchMrdClearenceLetter()
        {
            objDAL = new MasterDAL();
            MrdClearenceSearchViewModel model = new MrdClearenceSearchViewModel();
            return PartialView("SearchMrdClearenceLetter", model);
        }


        [HttpPost]
        public ActionResult GetMrdClearenceLetterList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            int stateCode = 0;
            // int districtCode = 0;
            int agencyCode = 0;
            int year = 0;
            int batch = 0;
            int collaboration = 0;
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IProposalBAL objBAL = new ProposalBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
            {
                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["agency"]))
            {
                agencyCode = Convert.ToInt32(Request.Params["agency"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["year"]))
            {
                year = Convert.ToInt32(Request.Params["year"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["batch"]))
            {
                batch = Convert.ToInt32(Request.Params["batch"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
            {
                collaboration = Convert.ToInt32(Request.Params["collaboration"]);
            }
            var jsonData = new
            {
                rows = objBAL.ListMrdClearanceBAL(stateCode, year, batch, agencyCode, collaboration, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult AddEditMrdClearenceLetter()
        {
            objBAL = new MasterBAL();
            MrdClearenceViewModel model = new MrdClearenceViewModel();
            model.User_Action = "A";
            return PartialView("AddEditMrdClearenceLetter", model);
        }


        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult AddEditMrdClearenceLetter(MrdClearenceViewModel model, HttpPostedFileBase file)
        public ActionResult AddEditMrdClearenceLetter(FormCollection frmCollection)
        {
            bool status = false;
            db = new PMGSYEntities();
            MrdClearenceViewModel model = new MrdClearenceViewModel();
            CommonFunctions objCommonFunc = new CommonFunctions();
            try
            {
                if (frmCollection["User_Action"] == "A") //For Save/Add Clearance Detail
                {
                    model.User_Action = frmCollection["User_Action"].ToString();
                    HttpPostedFileBase ClearancePdfFile = Request.Files["ClearancePdfFile"];
                    HttpPostedFileBase RoadPdfFile = Request.Files["RoadPdfFile"];
                    HttpPostedFileBase RoadExcelFile = Request.Files["RoadExcelFile"];
                    string fileTypes = string.Empty;
                    string[] arrfiletype = new string[5];
                    bool fileExt = false;
                    string filename = string.Empty;
                    string fileExcelSaveExt = string.Empty;
                    string filePdfSaveExt = string.Empty;
                    string filePathClearancePdfFile = string.Empty;
                    string filePathRoadPdfFile = string.Empty;
                    string filePathRoadExcelFile = string.Empty;
                    //if (ClearancePdfFile == null)
                    //{
                    //    message = message == string.Empty ? "Please select Clearance Letter Pdf file to upload." : message;
                    //    // return View("AddEditImsEcFileUpload", model);
                    //    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    //}
                    //else if (RoadPdfFile == null)
                    //{
                    //    message = message == string.Empty ? "Please select Road List  Pdf file to upload." : message;
                    //    // return View("AddEditImsEcFileUpload", model);
                    //    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    //}

                    if (ClearancePdfFile != null)
                    {
                        fileTypes = ConfigurationManager.AppSettings["Clearance_PDF_FORMAT"];


                        if (fileTypes == ClearancePdfFile.FileName.Split('.')[1])
                        {
                            fileExt = true;
                            filePdfSaveExt = fileTypes;
                            filePathClearancePdfFile = ConfigurationManager.AppSettings["Clearance_PDF_File_Upload"];
                        }

                        if (fileExt == false)
                        {
                            message = "Clearance Letter Pdf File type is not allowed.";
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                        }
                    }

                    if (RoadPdfFile != null)
                    {
                        fileTypes = ConfigurationManager.AppSettings["Clearance_PDF_FORMAT"];


                        if (fileTypes == RoadPdfFile.FileName.Split('.')[1])
                        {
                            fileExt = true;
                            filePdfSaveExt = fileTypes;
                            filePathRoadPdfFile = ConfigurationManager.AppSettings["Clearance_Road_PDF_File_Upload"];
                        }

                        if (fileExt == false)
                        {
                            message = "Road List Pdf File type is not allowed.";
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (RoadExcelFile != null)
                    {
                        fileTypes = ConfigurationManager.AppSettings["Clearance_Doc_FORMAT"];
                        arrfiletype = fileTypes.Split('$');
                        foreach (var item in arrfiletype)
                        {
                            if (item == RoadExcelFile.FileName.Split('.')[1])
                            {
                                fileExt = true;
                                fileExcelSaveExt = item;
                                filePathRoadExcelFile = ConfigurationManager.AppSettings["Clearance_Doc_File_Upload"];
                                break;
                            }
                        }
                        if (fileExt == false)
                        {
                            message = "Road List Excel File type is not allowed.";
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }


                    model.PhaseYear = Convert.ToInt32(frmCollection["PhaseYear"]);
                    model.Batch = Convert.ToInt32(frmCollection["Batch"]); ;
                    model.Mast_Agency = Convert.ToInt32(frmCollection["Mast_Agency"]); ;
                    model.IMS_COLLABORATION = Convert.ToInt32(frmCollection["IMS_COLLABORATION"]);
                    model.Mast_State_Code = Convert.ToInt32(frmCollection["Mast_State_Code"]) == 0 ? Convert.ToInt32(frmCollection["hdStateCode"]) : Convert.ToInt32(frmCollection["Mast_State_Code"]);
                    model.MRD_CLEARANCE_DATE = frmCollection["MRD_CLEARANCE_DATE"];
                    model.MRD_CLEARANCE_NUMBER = frmCollection["MRD_CLEARANCE_NUMBER"];
                    model.MRD_TOTAL_ROADS = Convert.ToInt32(frmCollection["MRD_TOTAL_ROADS"]);
                    model.MRD_TOTAL_LSB = Convert.ToInt32(frmCollection["MRD_TOTAL_LSB"]);
                    model.MRD_ROAD_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_MORD_SHARE_AMT"]);
                    model.MRD_ROAD_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_STATE_SHARE_AMT"]);
                    model.MRD_ROAD_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_TOTAL_AMT"]);
                    model.MRD_LSB_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_MORD_SHARE_AMT"]);
                    model.MRD_LSB_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_STATE_SHARE_AMT"]);
                    model.MRD_LSB_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_TOTAL_AMT"]);
                    model.MRD_TOTAL_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_MORD_SHARE_AMT"]);
                    model.MRD_TOTAL_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_STATE_SHARE_AMT"]);
                    model.MRD_TOTAL_SANCTIONED_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_SANCTIONED_AMT"]);
                    model.MRD_TOTAL_ROAD_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_ROAD_LENGTH"]);
                    model.MRD_TOTAL_LSB_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_LSB_LENGTH"]);
                    model.MRD_HAB_1000 = Convert.ToInt32(frmCollection["MRD_HAB_1000"]);
                    model.MRD_HAB_500 = Convert.ToInt32(frmCollection["MRD_HAB_500"]);
                    model.MRD_HAB_250_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_250_ELIGIBLE"]);
                    model.MRD_HAB_100_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_100_ELIGIBLE"]);
                    model.UPGRADE_CONNECT = frmCollection["UPGRADE_CONNECT"];
                    model.STAGE_COMPLETE = frmCollection["STAGE_COMPLETE"];
                    model.MRD_CLEARANCE_REMARKS = frmCollection["MRD_CLEARANCE_REMARKS"];

                    if (ModelState.IsValid)
                    {
                        //filename = Path.GetFileName(Request.Files["file"].FileName);
                        model.MRD_CLEARANCE_CODE = db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) == null ? 1 : (Int32)db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) + 1;
                        // var fileName = model.MRD_CLEARANCE_CODE + "_" + (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.Mast_State_Code).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.PhaseYear + "-" + (model.PhaseYear + 1)) + "_BATCH" + (model.Batch == null ? 0 : model.Batch) + "_" + (model.IMS_COLLABORATION) + "_SCHEME" + PMGSYSession.Current.PMGSYScheme;
                        if (ClearancePdfFile != null)
                        {
                            model.MRD_CLEARANCE_PDF_FILE = model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadPdfFile != null)
                        {
                            model.MRD_ROAD_PDF_FILE = model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadExcelFile != null)
                        {
                            model.MRD_ROAD_EXCEL_FILE = model.MRD_CLEARANCE_CODE + "." + fileExcelSaveExt;
                        }


                        IProposalBAL objBAL = new ProposalBAL();
                        using (TransactionScope objScope = new TransactionScope())
                        {

                            if (objBAL.AddMrdClearanceBAL(model, ref message))
                            {
                                if (message == string.Empty)
                                {
                                    if (ClearancePdfFile != null)
                                    {
                                        Request.Files["ClearancePdfFile"].SaveAs(Path.Combine(filePathClearancePdfFile, model.MRD_CLEARANCE_PDF_FILE));
                                    }
                                    if (RoadPdfFile != null)
                                    {
                                        Request.Files["RoadPdfFile"].SaveAs(Path.Combine(filePathRoadPdfFile, model.MRD_ROAD_PDF_FILE));
                                    }
                                    if (RoadExcelFile != null)
                                    {
                                        Request.Files["RoadExcelFile"].SaveAs(Path.Combine(filePathRoadExcelFile, model.MRD_ROAD_EXCEL_FILE));
                                    }
                                }
                                objScope.Complete();
                                message = message == string.Empty ? "Clearance  details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Clearance details not saved" : message });

                            }
                            else
                            {

                                return Json(new { success = status, message = message == string.Empty ? "Clearance details not saved" : message });

                            }
                        }
                    }

                    else
                    {
                        return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                    }

                }
                else if (frmCollection["User_Action"] == "E") //For Edit/Update Clearance Detail
                {
                    model.User_Action = frmCollection["User_Action"].ToString();
                    HttpPostedFileBase ClearancePdfFile = Request.Files["ClearancePdfFile"];
                    HttpPostedFileBase RoadPdfFile = Request.Files["RoadPdfFile"];
                    HttpPostedFileBase RoadExcelFile = Request.Files["RoadExcelFile"];
                    string fileTypes = string.Empty;
                    string[] arrfiletype = new string[5];
                    bool fileExt = false;
                    string filename = string.Empty;
                    string fileExcelSaveExt = "xslx";
                    string filePdfSaveExt = "pdf";
                    string filePathClearancePdfFile = string.Empty;
                    string filePathRoadPdfFile = string.Empty;
                    string filePathRoadExcelFile = string.Empty;
                    //if (frmCollection["Temp_MRD_CLEARANCE_PDF_FILE"] == "")
                    //{
                    //    if (ClearancePdfFile == null)
                    //    {
                    //        message = message == string.Empty ? "Please select Clearance Letter Pdf file to upload." : message;
                    //        // return View("AddEditImsEcFileUpload", model);
                    //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    //else if (frmCollection["Temp_MRD_ROAD_PDF_FILE"] == "")
                    //{
                    //    if (RoadPdfFile == null)
                    //    {
                    //        message = message == string.Empty ? "Please select Road List Pdf file to upload." : message;
                    //        // return View("AddEditImsEcFileUpload", model);
                    //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}

                    if (frmCollection["Temp_MRD_CLEARANCE_PDF_FILE"] == "")
                    {
                        if (ClearancePdfFile != null)
                        {
                            fileTypes = ConfigurationManager.AppSettings["Clearance_PDF_FORMAT"];


                            if (fileTypes == ClearancePdfFile.FileName.Split('.')[1])
                            {
                                fileExt = true;
                                filePdfSaveExt = fileTypes;
                                filePathClearancePdfFile = ConfigurationManager.AppSettings["Clearance_PDF_File_Upload"];
                            }

                            if (fileExt == false)
                            {
                                message = "Clearance Letter Pdf File type is not allowed.";
                                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                            }
                        }
                    }
                    if (frmCollection["Temp_MRD_ROAD_PDF_FILE"] == "")
                    {
                        if (RoadPdfFile != null)
                        {
                            fileTypes = ConfigurationManager.AppSettings["Clearance_PDF_FORMAT"];


                            if (fileTypes == RoadPdfFile.FileName.Split('.')[1])
                            {
                                fileExt = true;
                                filePdfSaveExt = fileTypes;
                                filePathRoadPdfFile = ConfigurationManager.AppSettings["Clearance_Road_PDF_File_Upload"];
                            }

                            if (fileExt == false)
                            {
                                message = "Road List Pdf File type is not allowed.";
                                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    if (frmCollection["Temp_MRD_ROAD_EXCEL_FILE"] == "")
                    {
                        if (RoadExcelFile != null)
                        {
                            fileTypes = ConfigurationManager.AppSettings["Clearance_Doc_FORMAT"];
                            arrfiletype = fileTypes.Split('$');
                            foreach (var item in arrfiletype)
                            {
                                if (item == RoadExcelFile.FileName.Split('.')[1])
                                {
                                    fileExt = true;
                                    fileExcelSaveExt = item;
                                    filePathRoadExcelFile = ConfigurationManager.AppSettings["Clearance_Doc_File_Upload"];
                                    break;
                                }
                            }
                            if (fileExt == false)
                            {
                                message = "Road List Excel File type is not allowed.";
                                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    model.PhaseYear = Convert.ToInt32(frmCollection["PhaseYear"]);
                    model.Batch = Convert.ToInt32(frmCollection["Batch"]); ;
                    model.Mast_Agency = Convert.ToInt32(frmCollection["Mast_Agency"]); ;
                    model.IMS_COLLABORATION = Convert.ToInt32(frmCollection["IMS_COLLABORATION"]);
                    model.Mast_State_Code = Convert.ToInt32(frmCollection["Mast_State_Code"]) == 0 ? Convert.ToInt32(frmCollection["hdStateCode"]) : Convert.ToInt32(frmCollection["Mast_State_Code"]);
                    model.MRD_CLEARANCE_DATE = frmCollection["MRD_CLEARANCE_DATE"];
                    model.MRD_CLEARANCE_NUMBER = frmCollection["MRD_CLEARANCE_NUMBER"];
                    model.MRD_TOTAL_ROADS = Convert.ToInt32(frmCollection["MRD_TOTAL_ROADS"]);
                    model.MRD_TOTAL_LSB = Convert.ToInt32(frmCollection["MRD_TOTAL_LSB"]);
                    model.MRD_ROAD_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_MORD_SHARE_AMT"]);
                    model.MRD_ROAD_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_STATE_SHARE_AMT"]);
                    model.MRD_ROAD_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_TOTAL_AMT"]);
                    model.MRD_LSB_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_MORD_SHARE_AMT"]);
                    model.MRD_LSB_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_STATE_SHARE_AMT"]);
                    model.MRD_LSB_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_TOTAL_AMT"]);
                    model.MRD_TOTAL_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_MORD_SHARE_AMT"]);
                    model.MRD_TOTAL_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_STATE_SHARE_AMT"]);
                    model.MRD_TOTAL_SANCTIONED_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_SANCTIONED_AMT"]);
                    model.MRD_TOTAL_ROAD_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_ROAD_LENGTH"]);
                    model.MRD_TOTAL_LSB_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_LSB_LENGTH"]);
                    model.MRD_HAB_1000 = Convert.ToInt32(frmCollection["MRD_HAB_1000"]);
                    model.MRD_HAB_500 = Convert.ToInt32(frmCollection["MRD_HAB_500"]);
                    model.MRD_HAB_250_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_250_ELIGIBLE"]);
                    model.MRD_HAB_100_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_100_ELIGIBLE"]);
                    model.EncryptedClearanceCode = frmCollection["EncryptedClearanceCode"];
                    model.UPGRADE_CONNECT = frmCollection["UPGRADE_CONNECT"];
                    model.STAGE_COMPLETE = frmCollection["STAGE_COMPLETE"];
                    model.MRD_CLEARANCE_REMARKS = frmCollection["MRD_CLEARANCE_REMARKS"];

                    if (ModelState.IsValid)
                    {
                        //filename = Path.GetFileName(Request.Files["file"].FileName);
                        // model.MRD_CLEARANCE_CODE = db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) == null ? 1 : (Int32)db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) + 1;
                        // var fileName = model.MRD_CLEARANCE_CODE + "_" + (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.Mast_State_Code).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.PhaseYear + "-" + (model.PhaseYear + 1)) + "_BATCH" + (model.Batch == null ? 0 : model.Batch) + "_" + (model.IMS_COLLABORATION) + "_SCHEME" + PMGSYSession.Current.PMGSYScheme;
                        string[] encryptedParameters = model.EncryptedClearanceCode.Split('/');
                        if (!(encryptedParameters.Length == 3))
                        {
                            return Json(new { success = status, message = message == string.Empty ? "Clearance details not updated" : message });
                        }

                        Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        model.MRD_CLEARANCE_CODE = Convert.ToInt32(decryptedParameters["ClearanceCode"].ToString());
                        //model.MRD_CLEARANCE_PDF_FILE = model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        //model.MRD_ROAD_PDF_FILE = model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        //if (RoadExcelFile != null)
                        //{
                        //    model.MRD_ROAD_EXCEL_FILE = model.MRD_CLEARANCE_CODE + "." + fileExcelSaveExt;
                        //}
                        //model.MRD_CLEARANCE_PDF_FILE = frmCollection["Temp_MRD_CLEARANCE_PDF_FILE"] != "" ? frmCollection["MRD_CLEARANCE_PDF_FILE"] : model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        // model.MRD_ROAD_PDF_FILE = frmCollection["Temp_MRD_ROAD_PDF_FILE"] != "" ? frmCollection["MRD_ROAD_PDF_FILE"] : model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        model.MRD_CLEARANCE_PDF_FILE = frmCollection["MRD_CLEARANCE_PDF_FILE"] != "" ? frmCollection["MRD_CLEARANCE_PDF_FILE"] : null;
                        model.MRD_ROAD_PDF_FILE = frmCollection["MRD_ROAD_PDF_FILE"] != "" ? frmCollection["MRD_ROAD_PDF_FILE"] : null;
                        model.MRD_ROAD_EXCEL_FILE = frmCollection["MRD_ROAD_EXCEL_FILE"] != "" ? frmCollection["MRD_ROAD_EXCEL_FILE"] : null;
                        if (ClearancePdfFile != null)
                        {
                            model.MRD_CLEARANCE_PDF_FILE = frmCollection["Temp_MRD_CLEARANCE_PDF_FILE"] != "" ? frmCollection["MRD_CLEARANCE_PDF_FILE"] : model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;

                        }
                        if (RoadPdfFile != null)
                        {
                            model.MRD_ROAD_PDF_FILE = frmCollection["Temp_MRD_ROAD_PDF_FILE"] != "" ? frmCollection["MRD_ROAD_PDF_FILE"] : model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadExcelFile != null)
                        {
                            model.MRD_ROAD_EXCEL_FILE = frmCollection["Temp_MRD_ROAD_EXCEL_FILE"] != "" ? frmCollection["MRD_ROAD_EXCEL_FILE"] : model.MRD_CLEARANCE_CODE + "." + fileExcelSaveExt;
                        }

                        IProposalBAL objBAL = new ProposalBAL();
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.EditMrdClearanceBAL(model, ref message)) //Edit / Update Detail
                            {
                                if (message == string.Empty)
                                {
                                    if (frmCollection["Temp_MRD_CLEARANCE_PDF_FILE"] == "")
                                    {
                                        if (ClearancePdfFile != null)
                                        {
                                            System.IO.File.Delete(Path.Combine(filePathClearancePdfFile, model.MRD_CLEARANCE_PDF_FILE));
                                            Request.Files["ClearancePdfFile"].SaveAs(Path.Combine(filePathClearancePdfFile, model.MRD_CLEARANCE_PDF_FILE));
                                        }
                                    }
                                    if (frmCollection["Temp_MRD_ROAD_PDF_FILE"] == "")
                                    {
                                        if (RoadPdfFile != null)
                                        {
                                            System.IO.File.Delete(Path.Combine(filePathRoadPdfFile, model.MRD_ROAD_PDF_FILE));
                                            Request.Files["RoadPdfFile"].SaveAs(Path.Combine(filePathRoadPdfFile, model.MRD_ROAD_PDF_FILE));
                                        }
                                    }
                                    if (frmCollection["Temp_MRD_ROAD_EXCEL_FILE"] == "")
                                    {

                                        if (RoadExcelFile != null)
                                        {
                                            System.IO.File.Delete(Path.Combine(filePathRoadExcelFile, model.MRD_ROAD_EXCEL_FILE));

                                            Request.Files["RoadExcelFile"].SaveAs(Path.Combine(filePathRoadExcelFile, model.MRD_ROAD_EXCEL_FILE));
                                        }
                                    }
                                }
                                objScope.Complete();
                                message = message == string.Empty ? "Clearance details updated successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Clearance details not updated" : message });

                            }
                            else
                            {

                                return Json(new { success = status, message = message == string.Empty ? "Clearance details not updated" : message });

                            }
                        }
                    }

                    else
                    {
                        return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                    }

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddEditMrdClearenceLetter()");
                message = message == string.Empty ? model.User_Action == "A" ? "Clearance details not saved." : model.User_Action == "E" ? "Clearance detail not updated" : message : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }



        [HttpGet]
        public ActionResult EditMrdClearenceLetter(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                IProposalBAL objBAL = new ProposalBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MrdClearenceViewModel model = objBAL.GetMrdClearanceDetailsBAL(Convert.ToInt32(decryptParameters["ClearanceCode"]));
                    model.User_Action = "E";
                    model.Temp_MRD_CLEARANCE_PDF_FILE = model.MRD_CLEARANCE_PDF_FILE == string.Empty ? "" : "file 1";
                    model.Temp_MRD_ROAD_PDF_FILE = model.MRD_ROAD_PDF_FILE == string.Empty ? "" : "file 2";
                    model.Temp_MRD_ROAD_EXCEL_FILE = model.MRD_ROAD_EXCEL_FILE == string.Empty ? "" : "file 3";
                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Clearance details not exist.");
                        return PartialView("AddEditMrdClearenceLetter", new MrdClearenceViewModel());
                    }

                    return PartialView("AddEditMrdClearenceLetter", model);
                }

                return PartialView("AddEditMrdClearenceLetter", new MrdClearenceViewModel());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditMrdClearenceLetter()");
                ModelState.AddModelError(String.Empty, " File Upload details not exist.");
                return PartialView("AddEditMrdClearenceLetter", new MrdClearenceViewModel());
            }
        }

        [Audit]
        [HttpGet]
        public ActionResult ViewMrdClearenceDetail(String parameter, String hash, String key)
        {

            try
            {
                IProposalBAL objProposalBAL = new ProposalBAL();
                Dictionary<string, string> decryptedParameters = null;
                // string[] encryptedParams = id.Split('/');
                // decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int clearanceCode = Convert.ToInt32(decryptedParameters["ClearanceCode"]);
                MrdClearenceViewModel model = objProposalBAL.GetMrdClearanceDetailsBAL(clearanceCode);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewMrdClearenceDetail()");
                return View(new MrdClearenceViewModel());
            }
        }


        [Audit]
        [HttpGet]
        public ActionResult ViewMrdClearenceRevisionDetail(String parameter, String hash, String key)
        {

            try
            {
                IProposalBAL objProposalBAL = new ProposalBAL();
                MrdClearenceRevisionViewModel modelMrdRevsion = new MrdClearenceRevisionViewModel();
                Dictionary<string, string> decryptedParameters = null;
                // string[] encryptedParams = id.Split('/');
                // decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int clearanceCode = Convert.ToInt32(decryptedParameters["ClearanceCode"]);
                modelMrdRevsion = objProposalBAL.GetMrdClearanceRevisionDetailsBAL(clearanceCode, "A");
                modelMrdRevsion.User_Action = "A";
                return View(modelMrdRevsion);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewMrdClearenceRevisionDetail()");
                return View(new MrdClearenceRevisionViewModel());
            }
        }


        [Audit]
        [HttpGet]
        public ActionResult AddEditMrdClearenceRevsionDetail(String parameter, String hash, String key)
        {

            try
            {
                IProposalBAL objProposalBAL = new ProposalBAL();
                MrdClearenceRevisionViewModel modelMrdRevsion = new MrdClearenceRevisionViewModel();
                Dictionary<string, string> decryptedParameters = null;
                // string[] encryptedParams = id.Split('/');
                // decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int clearanceCode = Convert.ToInt32(decryptedParameters["ClearanceCode"]);
                modelMrdRevsion = objProposalBAL.GetMrdClearanceRevisionDetailsBAL(clearanceCode, "A");
                modelMrdRevsion.User_Action = "A";
                return View(modelMrdRevsion);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddEditMrdClearenceRevsionDetail()");
                return View(new MrdClearenceRevisionViewModel());
            }
        }

        [HttpGet]
        public ActionResult EditMrdClearenceRevsionDetail(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                IProposalBAL objBAL = new ProposalBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MrdClearenceRevisionViewModel model = objBAL.GetMrdClearanceRevisionDetailsBAL(Convert.ToInt32(decryptParameters["ClearanceCode"]), "E");
                    model.User_Action = "E";
                    model.Temp_MRD_REVISED_CLEARANCE_PDF_FILE = model.MRD_CLEARANCE_REVISED_PDF_FILE == string.Empty ? "" : "file 1";
                    model.Temp_MRD_ROAD_REVISED_PDF_FILE = model.MRD_ROAD_REVISED_PDF_FILE == string.Empty ? "" : "file 2";
                    model.Temp_MRD_ROAD_REVISED_EXCEL_FILE = model.MRD_ROAD_REVISED_EXCEL_FILE == string.Empty ? "" : "file 3";
                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Clearance details not exist.");
                        return PartialView("AddEditMrdClearenceRevsionDetail", new MrdClearenceRevisionViewModel());
                    }

                    return PartialView("AddEditMrdClearenceRevsionDetail", model);
                }

                return PartialView("AddEditMrdClearenceRevsionDetail", new MrdClearenceRevisionViewModel());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditMrdClearenceRevsionDetail()");
                ModelState.AddModelError(String.Empty, " File Upload details not exist.");
                return PartialView("AddEditMrdClearenceRevsionDetail", new MrdClearenceRevisionViewModel());
            }
        }


        /// <summary>
        /// Method to Add Edit Clerance Revision Detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddEditMrdClerenceRevisionDetail(FormCollection frmCollection)
        {
            bool status = false;
            db = new PMGSYEntities();
            MrdClearenceRevisionViewModel model = new MrdClearenceRevisionViewModel();
            CommonFunctions objCommonFunc = new CommonFunctions();
            try
            {
                if (frmCollection["User_Action"] == "A") //For Save/Add Clearance Detail
                {
                    model.User_Action = frmCollection["User_Action"].ToString();
                    HttpPostedFileBase ClearanceRevisedPdfFile = Request.Files["ClearanceRevisedPdfFile"];
                    HttpPostedFileBase RoadRevisedPdfFile = Request.Files["RoadRevisedPdfFile"];
                    HttpPostedFileBase RoadRevsiedExcelFile = Request.Files["RoadRevisedExcelFile"];
                    string fileTypes = string.Empty;
                    string[] arrfiletype = new string[5];
                    bool fileExt = false;
                    string filename = string.Empty;
                    string fileExcelSaveExt = string.Empty;
                    string filePdfSaveExt = string.Empty;
                    string filePathClearanceRevsiedPdfFile = string.Empty;
                    string filePathRoadRevsiedPdfFile = string.Empty;
                    string filePathRoadRevisedExcelFile = string.Empty;
                    //if (ClearanceRevisedPdfFile == null)
                    //{
                    //    message = message == string.Empty ? "Please select Clearance Letter Revised Pdf file to upload." : message;
                    //    // return View("AddEditImsEcFileUpload", model);
                    //    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    //}
                    //else if (RoadRevisedPdfFile == null)
                    //{
                    //    message = message == string.Empty ? "Please select Road List  Revised  Pdf file to upload." : message;
                    //    // return View("AddEditImsEcFileUpload", model);
                    //    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    //}
                    if (ClearanceRevisedPdfFile != null)
                    {
                        fileTypes = ConfigurationManager.AppSettings["Clearance_PDF_FORMAT"];


                        if (fileTypes == ClearanceRevisedPdfFile.FileName.Split('.')[1])
                        {
                            fileExt = true;
                            filePdfSaveExt = fileTypes;
                            filePathClearanceRevsiedPdfFile = ConfigurationManager.AppSettings["Clearance_Revision_PDF_File_Upload"];
                        }

                        if (fileExt == false)
                        {
                            message = "Clearance Letter Revised Pdf File type is not allowed.";
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                        }
                    }

                    if (RoadRevisedPdfFile != null)
                    {
                        fileTypes = ConfigurationManager.AppSettings["Clearance_PDF_FORMAT"];


                        if (fileTypes == RoadRevisedPdfFile.FileName.Split('.')[1])
                        {
                            fileExt = true;
                            filePdfSaveExt = fileTypes;
                            filePathRoadRevsiedPdfFile = ConfigurationManager.AppSettings["Clearance_Road_Revision_PDF_File_Upload"];
                        }

                        if (fileExt == false)
                        {
                            message = "Road List Revised Pdf File type is not allowed.";
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (RoadRevsiedExcelFile != null)
                    {
                        fileTypes = ConfigurationManager.AppSettings["Clearance_Doc_FORMAT"];
                        arrfiletype = fileTypes.Split('$');
                        foreach (var item in arrfiletype)
                        {
                            if (item == RoadRevsiedExcelFile.FileName.Split('.')[1])
                            {
                                fileExt = true;
                                fileExcelSaveExt = item;
                                filePathRoadRevisedExcelFile = ConfigurationManager.AppSettings["Clearance_Revision_DOC_File_Upload"];
                                break;
                            }
                        }
                        if (fileExt == false)
                        {
                            message = "Road List Revised Excel File type is not allowed.";
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }


                    // model.PhaseYear = Convert.ToInt32(frmCollection["PhaseYear"]);
                    model.EncryptedClearanceCode = frmCollection["EncryptedClearanceCode"];
                    model.MRD_REVISION_DATE = frmCollection["MRD_REVISION_DATE"];
                    model.MRD_REVISION_NUMBER = frmCollection["MRD_REVISION_NUMBER"];
                    model.EncryptedClearanceRevisionCode = frmCollection["EncryptedClearanceRevisionCode"];
                    //filename = Path.GetFileName(Request.Files["file"].FileName);
                    // model.MRD_CLEARANCE_CODE = db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) == null ? 1 : (Int32)db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) + 1;
                    // var fileName = model.MRD_CLEARANCE_CODE + "_" + (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.Mast_State_Code).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.PhaseYear + "-" + (model.PhaseYear + 1)) + "_BATCH" + (model.Batch == null ? 0 : model.Batch) + "_" + (model.IMS_COLLABORATION) + "_SCHEME" + PMGSYSession.Current.PMGSYScheme;
                    model.PhaseYear = Convert.ToInt32(frmCollection["PhaseYear"]);
                    model.Batch = Convert.ToInt32(frmCollection["Batch"]); ;
                    model.Mast_Agency = Convert.ToInt32(frmCollection["Mast_Agency"]); ;
                    model.IMS_COLLABORATION = Convert.ToInt32(frmCollection["IMS_COLLABORATION"]);
                    model.Mast_State_Code = Convert.ToInt32(frmCollection["Mast_State_Code"]) == 0 ? Convert.ToInt32(frmCollection["hdStateCode"]) : Convert.ToInt32(frmCollection["Mast_State_Code"]);
                    model.MRD_CLEARANCE_DATE = frmCollection["MRD_CLEARANCE_DATE"];
                    model.MRD_CLEARANCE_NUMBER = frmCollection["MRD_CLEARANCE_NUMBER"];
                    model.MRD_TOTAL_ROADS = Convert.ToInt32(frmCollection["MRD_TOTAL_ROADS"]);
                    model.MRD_TOTAL_LSB = Convert.ToInt32(frmCollection["MRD_TOTAL_LSB"]);
                    model.MRD_ROAD_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_MORD_SHARE_AMT"]);
                    model.MRD_ROAD_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_STATE_SHARE_AMT"]);
                    model.MRD_ROAD_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_TOTAL_AMT"]);
                    model.MRD_LSB_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_MORD_SHARE_AMT"]);
                    model.MRD_LSB_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_STATE_SHARE_AMT"]);
                    model.MRD_LSB_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_TOTAL_AMT"]);
                    model.MRD_TOTAL_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_MORD_SHARE_AMT"]);
                    model.MRD_TOTAL_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_STATE_SHARE_AMT"]);
                    model.MRD_TOTAL_SANCTIONED_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_SANCTIONED_AMT"]);
                    model.MRD_TOTAL_ROAD_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_ROAD_LENGTH"]);
                    model.MRD_TOTAL_LSB_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_LSB_LENGTH"]);
                    model.MRD_HAB_1000 = Convert.ToInt32(frmCollection["MRD_HAB_1000"]);
                    model.MRD_HAB_500 = Convert.ToInt32(frmCollection["MRD_HAB_500"]);
                    model.MRD_HAB_250_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_250_ELIGIBLE"]);
                    model.MRD_HAB_100_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_100_ELIGIBLE"]);
                    model.STAGE_COMPLETE = frmCollection["STAGE_COMPLETE"];
                    model.UPGRADE_CONNECT = frmCollection["UPGRADE_CONNECT"];
                    model.MRD_CLEARANCE_REMARKS = frmCollection["MRD_CLEARANCE_REMARKS"];

                    if (ModelState.IsValid)
                    {
                        string[] encryptedParameters = model.EncryptedClearanceRevisionCode.Split('/');
                        if (!(encryptedParameters.Length == 3))
                        {
                            return Json(new { success = status, message = message == string.Empty ? "Clearance details not saved" : message });
                        }
                        model.MRD_CLEARANCE_CODE = db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) == null ? 1 : (Int32)db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) + 1;

                        Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        model.MRD_ORG_CLEARANCE_CODE = Convert.ToInt32(decryptedParameters["ClearanceRevisionCode"].ToString());
                        if (ClearanceRevisedPdfFile != null)
                        {
                            model.MRD_CLEARANCE_REVISED_PDF_FILE = model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadRevisedPdfFile != null)
                        {
                            model.MRD_ROAD_REVISED_PDF_FILE = model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadRevsiedExcelFile != null)
                        {
                            model.MRD_ROAD_REVISED_EXCEL_FILE = model.MRD_CLEARANCE_CODE + "." + fileExcelSaveExt;
                        }


                        IProposalBAL objBAL = new ProposalBAL();
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.AddMrdClearanceRevisionBAL(model, ref message))
                            {
                                if (message == string.Empty)
                                {
                                    if (ClearanceRevisedPdfFile != null)
                                    {
                                        Request.Files["ClearanceRevisedPdfFile"].SaveAs(Path.Combine(filePathClearanceRevsiedPdfFile, model.MRD_CLEARANCE_REVISED_PDF_FILE));
                                    }
                                    if (RoadRevisedPdfFile != null)
                                    {
                                        Request.Files["RoadRevisedPdfFile"].SaveAs(Path.Combine(filePathRoadRevsiedPdfFile, model.MRD_ROAD_REVISED_PDF_FILE));
                                    }
                                    if (RoadRevsiedExcelFile != null)
                                    {
                                        Request.Files["RoadRevisedExcelFile"].SaveAs(Path.Combine(filePathRoadRevisedExcelFile, model.MRD_ROAD_REVISED_EXCEL_FILE));
                                    }
                                }
                                objScope.Complete();
                                message = message == string.Empty ? "Clearance Revised details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Clearance Revised details not saved" : message });

                            }
                            else
                            {

                                return Json(new { success = status, message = message == string.Empty ? "Clearance Revised details not saved" : message });

                            }
                        }
                    }

                    else
                    {
                        return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                    }

                }
                else if (frmCollection["User_Action"] == "E") //For Edit/Update Clearance Detail
                {
                    model.User_Action = frmCollection["User_Action"].ToString();
                    HttpPostedFileBase ClearanceRevisedPdfFile = Request.Files["ClearanceRevisedPdfFile"];
                    HttpPostedFileBase RoadRevisedPdfFile = Request.Files["RoadRevisedPdfFile"];
                    HttpPostedFileBase RoadRevisedExcelFile = Request.Files["RoadRevisedExcelFile"];
                    string fileTypes = string.Empty;
                    string[] arrfiletype = new string[5];
                    bool fileExt = false;
                    string filename = string.Empty;
                    string fileExcelSaveExt = "xslx";
                    string filePdfSaveExt = "pdf";
                    string filePathClearancePdfFile = string.Empty;
                    string filePathRoadPdfFile = string.Empty;
                    string filePathRoadExcelFile = string.Empty;
                    //if (frmCollection["Temp_MRD_REVISED_CLEARANCE_PDF_FILE"] == "")
                    //{
                    //    if (ClearanceRevisedPdfFile == null)
                    //    {
                    //        message = message == string.Empty ? "Please select Clearance Letter Revised Pdf file to upload." : message;
                    //        // return View("AddEditImsEcFileUpload", model);
                    //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    //else if (frmCollection["Temp_MRD_ROAD_REVISED_PDF_FILE"] == "")
                    //{
                    //    if (RoadRevisedPdfFile == null)
                    //    {
                    //        message = message == string.Empty ? "Please select Road List Revised Pdf file to upload." : message;
                    //        // return View("AddEditImsEcFileUpload", model);
                    //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}

                    if (frmCollection["Temp_MRD_REVISED_CLEARANCE_PDF_FILE"] == "")
                    {
                        if (ClearanceRevisedPdfFile != null)
                        {
                            fileTypes = ConfigurationManager.AppSettings["Clearance_PDF_FORMAT"];


                            if (fileTypes == ClearanceRevisedPdfFile.FileName.Split('.')[1])
                            {
                                fileExt = true;
                                filePdfSaveExt = fileTypes;
                                filePathClearancePdfFile = ConfigurationManager.AppSettings["Clearance_Revision_PDF_File_Upload"];
                            }

                            if (fileExt == false)
                            {
                                message = "Clearance Letter Revised Pdf File type is not allowed.";
                                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                            }
                        }
                    }
                    if (frmCollection["Temp_MRD_ROAD_REVISED_PDF_FILE"] == "")
                    {
                        if (RoadRevisedPdfFile != null)
                        {
                            fileTypes = ConfigurationManager.AppSettings["Clearance_PDF_FORMAT"];


                            if (fileTypes == RoadRevisedPdfFile.FileName.Split('.')[1])
                            {
                                fileExt = true;
                                filePdfSaveExt = fileTypes;
                                filePathRoadPdfFile = ConfigurationManager.AppSettings["Clearance_Road_Revision_PDF_File_Upload"];
                            }

                            if (fileExt == false)
                            {
                                message = "Road List Revised Pdf File type is not allowed.";
                                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    if (frmCollection["Temp_MRD_ROAD_REVISED_EXCEL_FILE"] == "")
                    {
                        if (RoadRevisedExcelFile != null)
                        {
                            fileTypes = ConfigurationManager.AppSettings["Clearance_Doc_FORMAT"];
                            arrfiletype = fileTypes.Split('$');
                            foreach (var item in arrfiletype)
                            {
                                if (item == RoadRevisedExcelFile.FileName.Split('.')[1])
                                {
                                    fileExt = true;
                                    fileExcelSaveExt = item;
                                    filePathRoadExcelFile = ConfigurationManager.AppSettings["Clearance_Revision_DOC_File_Upload"];
                                    break;
                                }
                            }
                            if (fileExt == false)
                            {
                                message = "Road List Revised Excel File type is not allowed.";
                                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    model.EncryptedClearanceCode = frmCollection["EncryptedClearanceCode"];
                    model.EncryptedClearanceRevisionCode = frmCollection["EncryptedClearanceRevisionCode"];
                    model.MRD_REVISION_DATE = frmCollection["MRD_REVISION_DATE"];
                    model.MRD_ORG_CLEARANCE_CODE = Convert.ToInt32(frmCollection["MRD_ORG_CLEARANCE_CODE"]);
                    model.MRD_REVISION_NUMBER = frmCollection["MRD_REVISION_NUMBER"];
                    //filename = Path.GetFileName(Request.Files["file"].FileName);
                    // model.MRD_CLEARANCE_CODE = db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) == null ? 1 : (Int32)db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) + 1;
                    // var fileName = model.MRD_CLEARANCE_CODE + "_" + (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.Mast_State_Code).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.PhaseYear + "-" + (model.PhaseYear + 1)) + "_BATCH" + (model.Batch == null ? 0 : model.Batch) + "_" + (model.IMS_COLLABORATION) + "_SCHEME" + PMGSYSession.Current.PMGSYScheme;
                    model.PhaseYear = Convert.ToInt32(frmCollection["PhaseYear"]);
                    model.Batch = Convert.ToInt32(frmCollection["Batch"]); ;
                    model.Mast_Agency = Convert.ToInt32(frmCollection["Mast_Agency"]); ;
                    model.IMS_COLLABORATION = Convert.ToInt32(frmCollection["IMS_COLLABORATION"]);
                    model.Mast_State_Code = Convert.ToInt32(frmCollection["Mast_State_Code"]) == 0 ? Convert.ToInt32(frmCollection["hdStateCode"]) : Convert.ToInt32(frmCollection["Mast_State_Code"]);
                    model.MRD_CLEARANCE_DATE = frmCollection["MRD_CLEARANCE_DATE"];
                    model.MRD_CLEARANCE_NUMBER = frmCollection["MRD_CLEARANCE_NUMBER"];
                    model.MRD_TOTAL_ROADS = Convert.ToInt32(frmCollection["MRD_TOTAL_ROADS"]);
                    model.MRD_TOTAL_LSB = Convert.ToInt32(frmCollection["MRD_TOTAL_LSB"]);
                    model.MRD_ROAD_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_MORD_SHARE_AMT"]);
                    model.MRD_ROAD_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_STATE_SHARE_AMT"]);
                    model.MRD_ROAD_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_TOTAL_AMT"]);
                    model.MRD_LSB_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_MORD_SHARE_AMT"]);
                    model.MRD_LSB_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_STATE_SHARE_AMT"]);
                    model.MRD_LSB_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_TOTAL_AMT"]);
                    model.MRD_TOTAL_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_MORD_SHARE_AMT"]);
                    model.MRD_TOTAL_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_STATE_SHARE_AMT"]);
                    model.MRD_TOTAL_SANCTIONED_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_SANCTIONED_AMT"]);
                    model.MRD_TOTAL_ROAD_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_ROAD_LENGTH"]);
                    model.MRD_TOTAL_LSB_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_LSB_LENGTH"]);
                    model.MRD_HAB_1000 = Convert.ToInt32(frmCollection["MRD_HAB_1000"]);
                    model.MRD_HAB_500 = Convert.ToInt32(frmCollection["MRD_HAB_500"]);
                    model.MRD_HAB_250_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_250_ELIGIBLE"]);
                    model.MRD_HAB_100_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_100_ELIGIBLE"]);
                    model.STAGE_COMPLETE = frmCollection["STAGE_COMPLETE"];
                    model.UPGRADE_CONNECT = frmCollection["UPGRADE_CONNECT"];
                    model.MRD_CLEARANCE_REMARKS = frmCollection["MRD_CLEARANCE_REMARKS"];

                    if (ModelState.IsValid)
                    {
                        string[] encryptedParameters = model.EncryptedClearanceCode.Split('/');
                        if (!(encryptedParameters.Length == 3))
                        {
                            return Json(new { success = status, message = message == string.Empty ? "Clearance Revision details not updated" : message });
                        }

                        Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        model.MRD_CLEARANCE_CODE = Convert.ToInt32(decryptedParameters["ClearanceCode"].ToString());

                        //model.MRD_CLEARANCE_REVISED_PDF_FILE = (frmCollection["Temp_MRD_REVISED_CLEARANCE_PDF_FILE"] != "") ? frmCollection["MRD_CLEARANCE_REVISED_PDF_FILE"] : model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        model.MRD_CLEARANCE_REVISED_PDF_FILE = frmCollection["MRD_CLEARANCE_REVISED_PDF_FILE"] != "" ? frmCollection["MRD_CLEARANCE_REVISED_PDF_FILE"] : null;

                        model.MRD_ROAD_REVISED_PDF_FILE = frmCollection["MRD_ROAD_REVISED_PDF_FILE"] != "" ? frmCollection["MRD_ROAD_REVISED_PDF_FILE"] : null;
                        model.MRD_ROAD_REVISED_EXCEL_FILE = frmCollection["MRD_ROAD_REVISED_EXCEL_FILE"] != "" ? frmCollection["MRD_ROAD_REVISED_EXCEL_FILE"] : null;
                        if (ClearanceRevisedPdfFile != null)
                        {
                            model.MRD_CLEARANCE_REVISED_PDF_FILE = frmCollection["Temp_MRD_REVISED_CLEARANCE_PDF_FILE"] != "" ? frmCollection["MRD_CLEARANCE_REVISED_PDF_FILE"] : model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadRevisedPdfFile != null)
                        {
                            model.MRD_ROAD_REVISED_PDF_FILE = frmCollection["Temp_MRD_ROAD_REVISED_PDF_FILE"] != "" ? frmCollection["MRD_ROAD_REVISED_PDF_FILE"] : model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadRevisedExcelFile != null)
                        {
                            model.MRD_ROAD_REVISED_EXCEL_FILE = frmCollection["Temp_MRD_ROAD_REVISED_EXCEL_FILE"] != "" ? frmCollection["MRD_ROAD_REVISED_EXCEL_FILE"] : model.MRD_CLEARANCE_CODE + "." + fileExcelSaveExt;
                        }
                        IProposalBAL objBAL = new ProposalBAL();
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.EditMrdClearanceRevsionBAL(model, ref message)) //Edit / Update Detail
                            {
                                if (message == string.Empty)
                                {
                                    if (frmCollection["Temp_MRD_REVISED_CLEARANCE_PDF_FILE"] == "")
                                    {
                                        if (ClearanceRevisedPdfFile != null)
                                        {
                                            System.IO.File.Delete(Path.Combine(filePathClearancePdfFile, model.MRD_CLEARANCE_REVISED_PDF_FILE));
                                            Request.Files["ClearanceRevisedPdfFile"].SaveAs(Path.Combine(filePathClearancePdfFile, model.MRD_CLEARANCE_REVISED_PDF_FILE));
                                        }
                                    }
                                    if (frmCollection["Temp_MRD_ROAD_REVISED_PDF_FILE"] == "")
                                    {
                                        if (RoadRevisedPdfFile != null)
                                        {
                                            System.IO.File.Delete(Path.Combine(filePathRoadPdfFile, model.MRD_ROAD_REVISED_PDF_FILE));
                                            Request.Files["RoadRevisedPdfFile"].SaveAs(Path.Combine(filePathRoadPdfFile, model.MRD_ROAD_REVISED_PDF_FILE));
                                        }
                                    }
                                    if (frmCollection["Temp_MRD_ROAD_REVISED_EXCEL_FILE"] == "")
                                    {

                                        if (RoadRevisedExcelFile != null)
                                        {
                                            System.IO.File.Delete(Path.Combine(filePathRoadExcelFile, model.MRD_ROAD_REVISED_EXCEL_FILE));

                                            Request.Files["RoadRevisedExcelFile"].SaveAs(Path.Combine(filePathRoadExcelFile, model.MRD_ROAD_REVISED_EXCEL_FILE));
                                        }
                                    }
                                }
                                objScope.Complete();
                                message = message == string.Empty ? "Clearance Revised details updated successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Clearance Revised details not updated" : message });

                            }
                            else
                            {

                                return Json(new { success = status, message = message == string.Empty ? "Clearance Revised details not updated" : message });

                            }
                        }
                    }

                    else
                    {
                        return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                    }

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddEditMrdClerenceRevisionDetail()");
                message = message == string.Empty ? model.User_Action == "A" ? "Clearance Revised details not saved." : model.User_Action == "E" ? "Clearance Revised detail not updated" : message : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }

        /// <summary>
        /// Method to Add Edit Clerance Revision Detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddEditMrdClerenceRevisionDetailPartial(FormCollection frmCollection)
        {
            bool status = false;
            db = new PMGSYEntities();
            MrdClearenceRevisionViewModel model = new MrdClearenceRevisionViewModel();
            CommonFunctions objCommonFunc = new CommonFunctions();
            try
            {
                if (frmCollection["User_Action"] == "A") //For Save/Add Clearance Detail
                {
                    model.User_Action = frmCollection["User_Action"].ToString();
                    HttpPostedFileBase ClearanceRevisedPdfFile = Request.Files["ClearanceRevisedPdfFile"];
                    HttpPostedFileBase RoadRevisedPdfFile = Request.Files["RoadRevisedPdfFile"];
                    HttpPostedFileBase RoadRevsiedExcelFile = Request.Files["RoadRevisedExcelFile"];
                    string fileTypes = string.Empty;
                    string[] arrfiletype = new string[5];
                    bool fileExt = false;
                    string filename = string.Empty;
                    string fileExcelSaveExt = string.Empty;
                    string filePdfSaveExt = string.Empty;
                    string filePathClearanceRevsiedPdfFile = string.Empty;
                    string filePathRoadRevsiedPdfFile = string.Empty;
                    string filePathRoadRevisedExcelFile = string.Empty;

                    if (ClearanceRevisedPdfFile != null)
                    {
                        fileTypes = ConfigurationManager.AppSettings["Clearance_PDF_FORMAT"];


                        if (fileTypes == ClearanceRevisedPdfFile.FileName.Split('.')[1])
                        {
                            fileExt = true;
                            filePdfSaveExt = fileTypes;
                            filePathClearanceRevsiedPdfFile = ConfigurationManager.AppSettings["Clearance_Revision_PDF_File_Upload"];
                        }

                        if (fileExt == false)
                        {
                            message = "Clearance Letter Revised Pdf File type is not allowed.";
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                        }
                    }

                    if (RoadRevisedPdfFile != null)
                    {
                        fileTypes = ConfigurationManager.AppSettings["Clearance_PDF_FORMAT"];


                        if (fileTypes == RoadRevisedPdfFile.FileName.Split('.')[1])
                        {
                            fileExt = true;
                            filePdfSaveExt = fileTypes;
                            filePathRoadRevsiedPdfFile = ConfigurationManager.AppSettings["Clearance_Road_Revision_PDF_File_Upload"];
                        }

                        if (fileExt == false)
                        {
                            message = "Road List Revised Pdf File type is not allowed.";
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (RoadRevsiedExcelFile != null)
                    {
                        fileTypes = ConfigurationManager.AppSettings["Clearance_Doc_FORMAT"];
                        arrfiletype = fileTypes.Split('$');
                        foreach (var item in arrfiletype)
                        {
                            if (item == RoadRevsiedExcelFile.FileName.Split('.')[1])
                            {
                                fileExt = true;
                                fileExcelSaveExt = item;
                                filePathRoadRevisedExcelFile = ConfigurationManager.AppSettings["Clearance_Revision_DOC_File_Upload"];
                                break;
                            }
                        }
                        if (fileExt == false)
                        {
                            message = "Road List Revised Excel File type is not allowed.";
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }


                    // model.PhaseYear = Convert.ToInt32(frmCollection["PhaseYear"]);
                    model.EncryptedClearanceCode = frmCollection["EncryptedClearanceCode"];
                    model.MRD_REVISION_DATE = frmCollection["MRD_REVISION_DATE"];
                    model.MRD_REVISION_NUMBER = frmCollection["MRD_REVISION_NUMBER"];
                    model.EncryptedClearanceRevisionCode = frmCollection["EncryptedClearanceRevisionCode"];
                    //filename = Path.GetFileName(Request.Files["file"].FileName);
                    // model.MRD_CLEARANCE_CODE = db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) == null ? 1 : (Int32)db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) + 1;
                    // var fileName = model.MRD_CLEARANCE_CODE + "_" + (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.Mast_State_Code).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.PhaseYear + "-" + (model.PhaseYear + 1)) + "_BATCH" + (model.Batch == null ? 0 : model.Batch) + "_" + (model.IMS_COLLABORATION) + "_SCHEME" + PMGSYSession.Current.PMGSYScheme;
                    model.PhaseYear = Convert.ToInt32(frmCollection["PhaseYear"]);
                    model.Batch = Convert.ToInt32(frmCollection["Batch"]); ;
                    model.Mast_Agency = Convert.ToInt32(frmCollection["Mast_Agency"]); ;
                    model.IMS_COLLABORATION = Convert.ToInt32(frmCollection["IMS_COLLABORATION"]);
                    model.Mast_State_Code = Convert.ToInt32(frmCollection["Mast_State_Code"]) == 0 ? Convert.ToInt32(frmCollection["hdStateCode"]) : Convert.ToInt32(frmCollection["Mast_State_Code"]);
                    model.MRD_CLEARANCE_DATE = frmCollection["MRD_CLEARANCE_DATE"];
                    model.MRD_CLEARANCE_NUMBER = frmCollection["MRD_CLEARANCE_NUMBER"];
                    model.MRD_TOTAL_ROADS = Convert.ToInt32(frmCollection["MRD_TOTAL_ROADS"]);
                    model.MRD_TOTAL_LSB = Convert.ToInt32(frmCollection["MRD_TOTAL_LSB"]);
                    model.MRD_ROAD_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_MORD_SHARE_AMT"]);
                    model.MRD_ROAD_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_STATE_SHARE_AMT"]);
                    model.MRD_ROAD_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_TOTAL_AMT"]);
                    model.MRD_LSB_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_MORD_SHARE_AMT"]);
                    model.MRD_LSB_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_STATE_SHARE_AMT"]);
                    model.MRD_LSB_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_TOTAL_AMT"]);
                    model.MRD_TOTAL_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_MORD_SHARE_AMT"]);
                    model.MRD_TOTAL_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_STATE_SHARE_AMT"]);
                    model.MRD_TOTAL_SANCTIONED_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_SANCTIONED_AMT"]);
                    model.MRD_TOTAL_ROAD_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_ROAD_LENGTH"]);
                    model.MRD_TOTAL_LSB_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_LSB_LENGTH"]);
                    model.MRD_HAB_1000 = Convert.ToInt32(frmCollection["MRD_HAB_1000"]);
                    model.MRD_HAB_500 = Convert.ToInt32(frmCollection["MRD_HAB_500"]);
                    model.MRD_HAB_250_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_250_ELIGIBLE"]);
                    model.MRD_HAB_100_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_100_ELIGIBLE"]);
                    model.STAGE_COMPLETE = frmCollection["STAGE_COMPLETE"];
                    model.UPGRADE_CONNECT = frmCollection["UPGRADE_CONNECT"];
                    model.MRD_CLEARANCE_REMARKS = frmCollection["MRD_CLEARANCE_REMARKS"];


                    if (ModelState.IsValid)
                    {
                        string[] encryptedParameters = model.EncryptedClearanceRevisionCode.Split('/');
                        if (!(encryptedParameters.Length == 3))
                        {
                            return Json(new { success = status, message = message == string.Empty ? "Clearance details not saved" : message });
                        }
                        model.MRD_CLEARANCE_CODE = db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) == null ? 1 : (Int32)db.MRD_CLEARANCE_LETTERS.Max(cp => (Int32?)cp.MRD_CLEARANCE_CODE) + 1;

                        Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        model.MRD_ORG_CLEARANCE_CODE = Convert.ToInt32(decryptedParameters["ClearanceRevisionCode"].ToString());
                        if (ClearanceRevisedPdfFile != null)
                        {
                            model.MRD_CLEARANCE_REVISED_PDF_FILE = model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadRevisedPdfFile != null)
                        {
                            model.MRD_ROAD_REVISED_PDF_FILE = model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadRevsiedExcelFile != null)
                        {
                            model.MRD_ROAD_REVISED_EXCEL_FILE = model.MRD_CLEARANCE_CODE + "." + fileExcelSaveExt;
                        }


                        IProposalBAL objBAL = new ProposalBAL();
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.AddMrdClearanceRevisionBAL(model, ref message))
                            {
                                if (message == string.Empty)
                                {
                                    if (ClearanceRevisedPdfFile != null)
                                    {
                                        Request.Files["ClearanceRevisedPdfFile"].SaveAs(Path.Combine(filePathClearanceRevsiedPdfFile, model.MRD_CLEARANCE_REVISED_PDF_FILE));
                                    }
                                    if (RoadRevisedPdfFile != null)
                                    {
                                        Request.Files["RoadRevisedPdfFile"].SaveAs(Path.Combine(filePathRoadRevsiedPdfFile, model.MRD_ROAD_REVISED_PDF_FILE));
                                    }
                                    if (RoadRevsiedExcelFile != null)
                                    {
                                        Request.Files["RoadRevisedExcelFile"].SaveAs(Path.Combine(filePathRoadRevisedExcelFile, model.MRD_ROAD_REVISED_EXCEL_FILE));
                                    }
                                }
                                objScope.Complete();
                                message = message == string.Empty ? "Clearance Revised details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Clearance Revised details not saved" : message });

                            }
                            else
                            {

                                return Json(new { success = status, message = message == string.Empty ? "Clearance Revised details not saved" : message });

                            }
                        }
                    }

                    else
                    {
                        return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                    }

                }
                else if (frmCollection["User_Action"] == "E") //For Edit/Update Clearance Detail
                {
                    model.User_Action = frmCollection["User_Action"].ToString();
                    HttpPostedFileBase ClearanceRevisedPdfFile = Request.Files["ClearanceRevisedPdfFile"];
                    HttpPostedFileBase RoadRevisedPdfFile = Request.Files["RoadRevisedPdfFile"];
                    HttpPostedFileBase RoadRevisedExcelFile = Request.Files["RoadRevisedExcelFile"];
                    string fileTypes = string.Empty;
                    string[] arrfiletype = new string[5];
                    bool fileExt = false;
                    string filename = string.Empty;
                    string fileExcelSaveExt = "xslx";
                    string filePdfSaveExt = "pdf";
                    string filePathClearancePdfFile = string.Empty;
                    string filePathRoadPdfFile = string.Empty;
                    string filePathRoadExcelFile = string.Empty;

                    if (frmCollection["Temp_MRD_REVISED_CLEARANCE_PDF_FILE"] == "")
                    {
                        if (ClearanceRevisedPdfFile != null)
                        {
                            fileTypes = ConfigurationManager.AppSettings["Clearance_PDF_FORMAT"];


                            if (fileTypes == ClearanceRevisedPdfFile.FileName.Split('.')[1])
                            {
                                fileExt = true;
                                filePdfSaveExt = fileTypes;
                                filePathClearancePdfFile = ConfigurationManager.AppSettings["Clearance_Revision_PDF_File_Upload"];
                            }

                            if (fileExt == false)
                            {
                                message = "Clearance Letter Revised Pdf File type is not allowed.";
                                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                            }
                        }
                    }
                    if (frmCollection["Temp_MRD_ROAD_REVISED_PDF_FILE"] == "")
                    {
                        if (RoadRevisedPdfFile != null)
                        {
                            fileTypes = ConfigurationManager.AppSettings["Clearance_PDF_FORMAT"];


                            if (fileTypes == RoadRevisedPdfFile.FileName.Split('.')[1])
                            {
                                fileExt = true;
                                filePdfSaveExt = fileTypes;
                                filePathRoadPdfFile = ConfigurationManager.AppSettings["Clearance_Road_Revision_PDF_File_Upload"];
                            }

                            if (fileExt == false)
                            {
                                message = "Road List Revised Pdf File type is not allowed.";
                                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    if (frmCollection["Temp_MRD_ROAD_REVISED_EXCEL_FILE"] == "")
                    {
                        if (RoadRevisedExcelFile != null)
                        {
                            fileTypes = ConfigurationManager.AppSettings["Clearance_Doc_FORMAT"];
                            arrfiletype = fileTypes.Split('$');
                            foreach (var item in arrfiletype)
                            {
                                if (item == RoadRevisedExcelFile.FileName.Split('.')[1])
                                {
                                    fileExt = true;
                                    fileExcelSaveExt = item;
                                    filePathRoadExcelFile = ConfigurationManager.AppSettings["Clearance_Revision_DOC_File_Upload"];
                                    break;
                                }
                            }
                            if (fileExt == false)
                            {
                                message = "Road List Revised Excel File type is not allowed.";
                                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    model.EncryptedClearanceCode = frmCollection["EncryptedClearanceCode"];
                    model.EncryptedClearanceRevisionCode = frmCollection["EncryptedClearanceRevisionCode"];
                    model.MRD_REVISION_DATE = frmCollection["MRD_REVISION_DATE"];
                    model.MRD_ORG_CLEARANCE_CODE = Convert.ToInt32(frmCollection["MRD_ORG_CLEARANCE_CODE"]);
                    model.MRD_REVISION_NUMBER = frmCollection["MRD_REVISION_NUMBER"];
                    model.PhaseYear = Convert.ToInt32(frmCollection["PhaseYear"]);
                    model.Batch = Convert.ToInt32(frmCollection["Batch"]); ;
                    model.Mast_Agency = Convert.ToInt32(frmCollection["Mast_Agency"]); ;
                    model.IMS_COLLABORATION = Convert.ToInt32(frmCollection["IMS_COLLABORATION"]);
                    model.Mast_State_Code = Convert.ToInt32(frmCollection["Mast_State_Code"]) == 0 ? Convert.ToInt32(frmCollection["hdStateCode"]) : Convert.ToInt32(frmCollection["Mast_State_Code"]);
                    model.MRD_CLEARANCE_DATE = frmCollection["MRD_CLEARANCE_DATE"];
                    model.MRD_CLEARANCE_NUMBER = frmCollection["MRD_CLEARANCE_NUMBER"];
                    model.MRD_TOTAL_ROADS = Convert.ToInt32(frmCollection["MRD_TOTAL_ROADS"]);
                    model.MRD_TOTAL_LSB = Convert.ToInt32(frmCollection["MRD_TOTAL_LSB"]);
                    model.MRD_ROAD_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_MORD_SHARE_AMT"]);
                    model.MRD_ROAD_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_STATE_SHARE_AMT"]);
                    model.MRD_ROAD_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_TOTAL_AMT"]);
                    model.MRD_LSB_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_MORD_SHARE_AMT"]);
                    model.MRD_LSB_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_STATE_SHARE_AMT"]);
                    model.MRD_LSB_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_TOTAL_AMT"]);
                    model.MRD_TOTAL_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_MORD_SHARE_AMT"]);
                    model.MRD_TOTAL_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_STATE_SHARE_AMT"]);
                    model.MRD_TOTAL_SANCTIONED_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_SANCTIONED_AMT"]);
                    model.MRD_TOTAL_ROAD_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_ROAD_LENGTH"]);
                    model.MRD_TOTAL_LSB_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_LSB_LENGTH"]);
                    model.MRD_HAB_1000 = Convert.ToInt32(frmCollection["MRD_HAB_1000"]);
                    model.MRD_HAB_500 = Convert.ToInt32(frmCollection["MRD_HAB_500"]);
                    model.MRD_HAB_250_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_250_ELIGIBLE"]);
                    model.MRD_HAB_100_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_100_ELIGIBLE"]);
                    model.STAGE_COMPLETE = frmCollection["STAGE_COMPLETE"];
                    model.UPGRADE_CONNECT = frmCollection["UPGRADE_CONNECT"];
                    model.MRD_CLEARANCE_REMARKS = frmCollection["MRD_CLEARANCE_REMARKS"];

                    if (ModelState.IsValid)
                    {
                        string[] encryptedParameters = model.EncryptedClearanceCode.Split('/');
                        if (!(encryptedParameters.Length == 3))
                        {
                            return Json(new { success = status, message = message == string.Empty ? "Clearance Revision details not updated" : message });
                        }

                        Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        model.MRD_CLEARANCE_CODE = Convert.ToInt32(decryptedParameters["ClearanceCode"].ToString());

                        //model.MRD_CLEARANCE_REVISED_PDF_FILE = (frmCollection["Temp_MRD_REVISED_CLEARANCE_PDF_FILE"] != "") ? frmCollection["MRD_CLEARANCE_REVISED_PDF_FILE"] : model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        model.MRD_CLEARANCE_REVISED_PDF_FILE = frmCollection["MRD_CLEARANCE_REVISED_PDF_FILE"] != "" ? frmCollection["MRD_CLEARANCE_REVISED_PDF_FILE"] : null;

                        model.MRD_ROAD_REVISED_PDF_FILE = frmCollection["MRD_ROAD_REVISED_PDF_FILE"] != "" ? frmCollection["MRD_ROAD_REVISED_PDF_FILE"] : null;
                        model.MRD_ROAD_REVISED_EXCEL_FILE = frmCollection["MRD_ROAD_REVISED_EXCEL_FILE"] != "" ? frmCollection["MRD_ROAD_REVISED_EXCEL_FILE"] : null;
                        if (ClearanceRevisedPdfFile != null)
                        {
                            model.MRD_CLEARANCE_REVISED_PDF_FILE = frmCollection["Temp_MRD_REVISED_CLEARANCE_PDF_FILE"] != "" ? frmCollection["MRD_CLEARANCE_REVISED_PDF_FILE"] : model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadRevisedPdfFile != null)
                        {
                            model.MRD_ROAD_REVISED_PDF_FILE = frmCollection["Temp_MRD_ROAD_REVISED_PDF_FILE"] != "" ? frmCollection["MRD_ROAD_REVISED_PDF_FILE"] : model.MRD_CLEARANCE_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadRevisedExcelFile != null)
                        {
                            model.MRD_ROAD_REVISED_EXCEL_FILE = frmCollection["Temp_MRD_ROAD_REVISED_EXCEL_FILE"] != "" ? frmCollection["MRD_ROAD_REVISED_EXCEL_FILE"] : model.MRD_CLEARANCE_CODE + "." + fileExcelSaveExt;
                        }
                        IProposalBAL objBAL = new ProposalBAL();
                        using (TransactionScope objScope = new TransactionScope())
                        {
                            if (objBAL.EditMrdClearanceRevsionBAL(model, ref message)) //Edit / Update Detail
                            {
                                if (message == string.Empty)
                                {
                                    if (frmCollection["Temp_MRD_REVISED_CLEARANCE_PDF_FILE"] == "")
                                    {
                                        if (ClearanceRevisedPdfFile != null)
                                        {
                                            System.IO.File.Delete(Path.Combine(filePathClearancePdfFile, model.MRD_CLEARANCE_REVISED_PDF_FILE));
                                            Request.Files["ClearanceRevisedPdfFile"].SaveAs(Path.Combine(filePathClearancePdfFile, model.MRD_CLEARANCE_REVISED_PDF_FILE));
                                        }
                                    }
                                    if (frmCollection["Temp_MRD_ROAD_REVISED_PDF_FILE"] == "")
                                    {
                                        if (RoadRevisedPdfFile != null)
                                        {
                                            System.IO.File.Delete(Path.Combine(filePathRoadPdfFile, model.MRD_ROAD_REVISED_PDF_FILE));
                                            Request.Files["RoadRevisedPdfFile"].SaveAs(Path.Combine(filePathRoadPdfFile, model.MRD_ROAD_REVISED_PDF_FILE));
                                        }
                                    }
                                    if (frmCollection["Temp_MRD_ROAD_REVISED_EXCEL_FILE"] == "")
                                    {

                                        if (RoadRevisedExcelFile != null)
                                        {
                                            System.IO.File.Delete(Path.Combine(filePathRoadExcelFile, model.MRD_ROAD_REVISED_EXCEL_FILE));

                                            Request.Files["RoadRevisedExcelFile"].SaveAs(Path.Combine(filePathRoadExcelFile, model.MRD_ROAD_REVISED_EXCEL_FILE));
                                        }
                                    }
                                }
                                objScope.Complete();
                                message = message == string.Empty ? "Clearance Revised details updated successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Clearance Revised details not updated" : message });

                            }
                            else
                            {

                                return Json(new { success = status, message = message == string.Empty ? "Clearance Revised details not updated" : message });

                            }
                        }
                    }

                    else
                    {
                        return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                    }

                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddEditMrdClerenceRevisionDetailPartial()");
                message = message == string.Empty ? model.User_Action == "A" ? "Clearance Revised details not saved." : model.User_Action == "E" ? "Clearance Revised detail not updated" : message : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }


        /// <summary>
        /// Method to delete admin department details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMrdClearenceLetter(String parameter, String hash, String key)
        {
            IProposalBAL objBAL = new ProposalBAL();
            bool status = false;
            string filePathClearancePdfFile = ConfigurationManager.AppSettings["Clearance_PDF_File_Upload"];
            string filePathRoadPdfFile = ConfigurationManager.AppSettings["Clearance_Road_PDF_File_Upload"];
            string filePathRoadExcelFile = ConfigurationManager.AppSettings["Clearance_Doc_File_Upload"];
            string filePathClearanceRevsiedPdfFile = ConfigurationManager.AppSettings["Clearance_Revision_PDF_File_Upload"];
            string filePathRoadRevsiedPdfFile = ConfigurationManager.AppSettings["Clearance_Road_Revision_PDF_File_Upload"];
            string filePathRoadRevisedExcelFile = ConfigurationManager.AppSettings["Clearance_Revision_DOC_File_Upload"];
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MrdClearenceViewModel model = objBAL.GetMrdClearanceDetailsBAL(Convert.ToInt32(decryptedParameters["ClearanceCode"]));
                    string fileType = decryptedParameters["Type"].ToString().Trim();
                    if (model != null)
                    {

                        if (objBAL.DeleteMrdClearanceBAL(Convert.ToInt32(Convert.ToInt32(decryptedParameters["ClearanceCode"])), ref message))
                        {
                            status = true;
                            if (model.MRD_CLEARANCE_PDF_FILE != "")
                            {
                                System.IO.File.Delete(Path.Combine(filePathClearancePdfFile, model.MRD_CLEARANCE_PDF_FILE));
                            }
                            if (model.MRD_ROAD_PDF_FILE != "")
                            {
                                System.IO.File.Delete(Path.Combine(filePathRoadPdfFile, model.MRD_ROAD_PDF_FILE));
                            }
                            if (model.MRD_ROAD_EXCEL_FILE != "")
                            {
                                System.IO.File.Delete(Path.Combine(filePathRoadExcelFile, model.MRD_ROAD_EXCEL_FILE));
                            }

                        }
                        else
                        {
                            message = message == string.Empty ? "You can not delete this Clearance details." : message;
                        }
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Clearance details." : message;

                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this Clearance details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "DeleteMrdClearenceLetter()");
                message = "You can not delete this Clearance details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult DownloadMrdClearenceLetter(String parameter, String hash, String key)
        {
            IProposalBAL objBAL = new ProposalBAL();
            string fileName = string.Empty;
            string fullfilePhysicalPath = string.Empty;
            string fileExtension = string.Empty;
            string path = string.Empty;

            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

            if (decryptedParameters.Count() > 0)
            {
                MrdClearenceViewModel model = objBAL.GetMrdClearanceDetailsBAL(Convert.ToInt32(decryptedParameters["ClearanceCode"]));
                string fileType = decryptedParameters["Type"].ToString().Trim();
                switch (fileType)
                {
                    case "CP":
                        fileName = model.MRD_CLEARANCE_PDF_FILE;
                        path = ConfigurationManager.AppSettings["Clearance_PDF_File_Upload"];
                        break;
                    case "RP":
                        fileName = model.MRD_ROAD_PDF_FILE;
                        path = ConfigurationManager.AppSettings["Clearance_Road_PDF_File_Upload"];
                        break;
                    case "RE":
                        fileName = model.MRD_ROAD_EXCEL_FILE;
                        path = ConfigurationManager.AppSettings["Clearance_Doc_File_Upload"];
                        break;
                    case "CPR":
                        fileName = model.MRD_CLEARANCE_PDF_FILE;
                        path = ConfigurationManager.AppSettings["Clearance_Revision_PDF_File_Upload"];
                        break;
                    case "RPR":
                        fileName = model.MRD_ROAD_PDF_FILE;
                        path = ConfigurationManager.AppSettings["Clearance_Road_Revision_PDF_File_Upload"];
                        break;
                    case "RER":
                        fileName = model.MRD_ROAD_EXCEL_FILE;
                        path = ConfigurationManager.AppSettings["Clearance_Revision_DOC_File_Upload"];
                        break;
                }


                fileExtension = Path.GetExtension(fileName).ToLower();


                fullfilePhysicalPath = Path.Combine(path, fileName);

                string name = Path.GetFileName(fileName);
                string ext = Path.GetExtension(fileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                        case ".xls":
                        case ".xlsx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }



                if (System.IO.File.Exists(fullfilePhysicalPath))
                {
                    return File(fullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + fileExtension);
                }
                else
                {
                    return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete Particular File from path and set on database as string empty field
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditDeleteMrdClearenceFile(String parameter, String hash, String key)
        {
            IProposalBAL objBAL = new ProposalBAL();
            bool status = false;
            int cleranceCode = 0;
            string fileType = string.Empty;
            string fileName = string.Empty;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    //MrdClearenceViewModel model = objBAL.GetMrdClearanceDetailsBAL(Convert.ToInt32(decryptedParameters["ClearanceCode"]));
                    cleranceCode = Convert.ToInt32(decryptedParameters["ClearanceCode"]);
                    fileType = decryptedParameters["Type"].ToString().Trim();
                    fileName = decryptedParameters["File"].ToString().Trim();

                    if (objBAL.EditDeleteMrdClearanceFileBAL(cleranceCode, fileType, fileName, ref message))
                    {
                        status = true;
                        message = message == string.Empty ? "Clearance file deleted successfully." : message;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this file details." : message;
                    }


                }
                else
                {
                    message = message == string.Empty ? "You can not delete this file details." : message;

                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);


                message = "You can not delete this file details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditDeleteMrdClearenceFile()");
                message = "You can not delete this file details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult ListMrdClearenceFiles()
        {
            Dictionary<string, string> decryptParameters = null;
            ViewData["ClearanceCodeEncrypted"] = Request.Params["ClearanceCodeEncrypted"].ToString().Trim();
            string[] encryptedParams = Request.Params["ClearanceCodeEncrypted"].ToString().Trim().Split('/');
            decryptParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
            int clearanceCode = Convert.ToInt32(decryptParameters["ClearanceCode"]);
            ViewBag.CleranceCodeEncryptFile = URLEncrypt.EncryptParameters1(new string[] { "ClearanceCode=" + clearanceCode.ToString().Trim(), "Status=" + decryptParameters["Status"].ToString().Trim() });

            return View();
        }

        [HttpGet]
        public ActionResult ListMrdClearenceRevisionDetail()
        {
            Dictionary<string, string> decryptParameters = null;
            ViewData["hdClearanceCodeEncrypted"] = Request.Params["ClearanceCodeEncrypted"].ToString().Trim();
            string[] encryptedParams = Request.Params["ClearanceCodeEncrypted"].ToString().Trim().Split('/');
            decryptParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
            int clearanceCode = Convert.ToInt32(decryptParameters["ClearanceCode"]);
            ViewBag.CleranceCodeEncrypt = URLEncrypt.EncryptParameters1(new string[] { "ClearanceCode=" + clearanceCode.ToString().Trim() });
            return View();
        }
        [HttpPost]
        public ActionResult GetMrdClearenceFileList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                IProposalBAL objBAL = new ProposalBAL();
                string[] encryptedParameters = Request.Params["ClearanceCodeEncrypted"].Split('/');
                if (!(encryptedParameters.Length == 3))
                {
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
                }
                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                string searchParameters = String.Empty;
                long totalRecords;
                int clearanceCode = 0;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(decryptedParameters["ClearanceCode"].ToString()))
                {
                    clearanceCode = Convert.ToInt32(decryptedParameters["ClearanceCode"].ToString()); ;
                }
                string clearanceStatus = decryptedParameters["Status"].ToString();
                var jsonData = new
                {
                    rows = objBAL.ListMrdClearanceFileBAL(clearanceCode, clearanceStatus, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetMrdClearenceFileList()");
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetMrdClearenceRevisionList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                IProposalBAL objBAL = new ProposalBAL();
                string[] encryptedParameters = Request.Params["ClearanceCodeEncrypted"].Split('/');
                if (!(encryptedParameters.Length == 3))
                {
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
                }
                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                string searchParameters = String.Empty;
                long totalRecords;
                int clearanceCode = 0;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(decryptedParameters["ClearanceCode"].ToString()))
                {
                    clearanceCode = Convert.ToInt32(decryptedParameters["ClearanceCode"].ToString()); ;
                }

                var jsonData = new
                {
                    rows = objBAL.ListMrdClearanceRevisionBAL(clearanceCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetMrdClearenceRevisionList()");
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetOriginalMrdClearenceList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                IProposalBAL objBAL = new ProposalBAL();
                string[] encryptedParameters = Request.Params["ClearanceCodeEncrypted"].Split('/');
                if (!(encryptedParameters.Length == 3))
                {
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
                }
                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                string searchParameters = String.Empty;
                long totalRecords;
                int clearanceCode = 0;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(decryptedParameters["ClearanceCode"].ToString()))
                {
                    clearanceCode = Convert.ToInt32(decryptedParameters["ClearanceCode"].ToString()); ;
                }

                var jsonData = new
                {
                    rows = objBAL.ListOriginalMrdClearanceBAL(clearanceCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetOriginalMrdClearenceList()");
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteMrdClearenceRevisionLetter(String parameter, String hash, String key)
        {
            IProposalBAL objBAL = new ProposalBAL();
            bool status = false;
            string filePathClearancePdfFile = ConfigurationManager.AppSettings["Clearance_PDF_File_Upload"];
            string filePathRoadPdfFile = ConfigurationManager.AppSettings["Clearance_Road_PDF_File_Upload"];
            string filePathRoadExcelFile = ConfigurationManager.AppSettings["Clearance_Doc_File_Upload"];
            string filePathClearanceRevsiedPdfFile = ConfigurationManager.AppSettings["Clearance_Revision_PDF_File_Upload"];
            string filePathRoadRevsiedPdfFile = ConfigurationManager.AppSettings["Clearance_Road_Revision_PDF_File_Upload"];
            string filePathRoadRevisedExcelFile = ConfigurationManager.AppSettings["Clearance_Revision_DOC_File_Upload"];
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    MrdClearenceViewModel model = objBAL.GetMrdClearanceDetailsBAL(Convert.ToInt32(decryptedParameters["ClearanceCode"]));

                    if (model != null)
                    {

                        if (objBAL.DeleteMrdClearanceRevisionBAL(Convert.ToInt32(Convert.ToInt32(decryptedParameters["ClearanceCode"])), ref message))
                        {
                            status = true;

                            if (model.MRD_CLEARANCE_PDF_FILE != "")
                            {
                                System.IO.File.Delete(Path.Combine(filePathClearanceRevsiedPdfFile, model.MRD_CLEARANCE_PDF_FILE));
                            }
                            if (model.MRD_ROAD_PDF_FILE != "")
                            {
                                System.IO.File.Delete(Path.Combine(filePathRoadRevsiedPdfFile, model.MRD_ROAD_PDF_FILE));
                            }
                            if (model.MRD_ROAD_EXCEL_FILE != "")
                            {
                                System.IO.File.Delete(Path.Combine(filePathRoadRevisedExcelFile, model.MRD_ROAD_EXCEL_FILE));
                            }

                        }
                        else
                        {
                            message = message == string.Empty ? "You can not delete this Clearance Revision details." : message;
                        }
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Clearance Revision details." : message;

                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "You can not delete this Clearance Revision details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "DeleteMrdClearenceRevisionLetter()");
                message = "You can not delete this Clearance  Revision details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// returns the partial view for adding the clearance details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult AddClearanceRevisionDetailsPartial(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                IProposalBAL objBAL = new ProposalBAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MrdClearenceRevisionViewModel model = objBAL.GetMrdClearanceRevisionDetailsBAL(Convert.ToInt32(decryptParameters["ClearanceCode"]), "E");
                    model.User_Action = "E";
                    model.Temp_MRD_REVISED_CLEARANCE_PDF_FILE = model.MRD_CLEARANCE_REVISED_PDF_FILE == string.Empty ? "" : "file 1";
                    model.Temp_MRD_ROAD_REVISED_PDF_FILE = model.MRD_ROAD_REVISED_PDF_FILE == string.Empty ? "" : "file 2";
                    model.Temp_MRD_ROAD_REVISED_EXCEL_FILE = model.MRD_ROAD_REVISED_EXCEL_FILE == string.Empty ? "" : "file 3";
                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Clearance details not exist.");
                        return PartialView("AddClearanceRevisionDetailsPartial", new MrdClearenceRevisionViewModel());
                    }

                    return PartialView("AddClearanceRevisionDetailsPartial", model);
                }

                return PartialView("AddClearanceRevisionDetailsPartial", new MrdClearenceRevisionViewModel());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddClearanceRevisionDetailsPartial()");
                ModelState.AddModelError(String.Empty, " File Upload details not exist.");
                return PartialView("AddClearanceRevisionDetailsPartial", new MrdClearenceRevisionViewModel());
            }
        }




        #endregion

        #region Disposing DataBase Object
        /// <summary>
        /// This Disposes DataBase Object
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        #endregion

        #region OTHER Common Function

        public ActionResult PopulateAgenciesByStateAndDepartmentwise(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateAgenciesByStateAndDepartmentwise(Convert.ToInt32(frmCollection["StateCode"]), PMGSYSession.Current.AdminNdCode, Convert.ToBoolean(frmCollection["IsAllSelected"]));

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// returns districts according to the state
        /// </summary>
        /// <param name="stateCode">indicates the state id</param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetDistrictByState(int stateCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                return Json(new SelectList(objCommon.PopulateDistrict(stateCode, true), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetDistrictByState()");
                return null;
            }
        }

        /// <summary>
        /// returns the package code by year and district code
        /// </summary>
        /// <param name="yearCode">indicates the year code</param>
        /// <param name="districtCode">indicates the district code</param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetPackageByState(int yearCode, int districtCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                TransactionParams objParam = new TransactionParams();
                objParam.SANC_YEAR = Convert.ToInt16(yearCode);
                objParam.DISTRICT_CODE = districtCode;
                return Json(new SelectList(objCommon.PopulatePackage(objParam), "Value", "Text"), JsonRequestBehavior.AllowGet);
                //List<IMS_SANCTIONED_PROJECTS> lstProposal = db.IMS_SANCTIONED_PROJECTS.Where(m=>m.MAST_STATE_CODE==stateCode && m.MAST_DISTRICT_CODE == districtCode).ToList<IMS_SANCTIONED_PROJECTS>();
                //return Json(new SelectList(lstProposal, "IMS_PACKAGE_ID", "IMS_PACKAGE_ID"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetPackageByState()");
                return null;
            }
        }


        [HttpPost]
        [Audit]
        public JsonResult GetPackagesByYearandBlock(string sanctionYear, string blockCode)
        {
            try
            {
                TransactionParams transactionParams = new TransactionParams();

                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = Convert.ToInt16(sanctionYear.Trim());
                transactionParams.BlockCode = Convert.ToInt16(blockCode.Trim());
                List<SelectListItem> lstPackages = (new SelectList(new CommonFunctions().GetPackages(Convert.ToInt32(sanctionYear), Convert.ToInt32(blockCode), true).ToList(), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID")).ToList<SelectListItem>();
                return Json(lstPackages);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetPackagesByYearandBlock()");
                return Json(false);
            }
        }

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

        #region  Habitation Definalization

        public ActionResult ListProposalforHabitationDefinalization()
        {
            ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
            proposalViewModel.STATES = objCommonFuntion.PopulateStates();
            proposalViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            if (PMGSYSession.Current.RoleCode != 47)
            {
                proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode);
            }
            else
            {
                List<SelectListItem> districtList = new List<SelectListItem>();
                districtList = objCommonFuntion.GetAllDistrictsByAdminNDCode(PMGSYSession.Current.StateCode, PMGSYSession.Current.AdminNdCode);
                districtList.Insert(0, new SelectListItem { Value = "0", Text = "--Select District--" });
                proposalViewModel.DISTRICTS = districtList;
            }
            proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
            proposalViewModel.BATCHS.Find(x => x.Value == "0").Text = "All";
            proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
            proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
            proposalViewModel.Years = PopulateYear(0, true, true);
            proposalViewModel.IMS_YEAR = DateTime.Now.Year;
            proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;
            return View("ListProposalforHabitationDefinalization", proposalViewModel);
        }


        [HttpPost]
        [Audit]
        public ActionResult GetMordProposalsforHabitationDefinalization(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int MAST_STATE_ID = Convert.ToInt32(Request.Params["MAST_STATE_ID"]);
            int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
            string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
            int IMS_AGENCY = Convert.ToInt32(Request.Params["IMS_AGENCY"]);

            int totalRecords;
            var jsonData = new
            {
                rows = objProposalBAL.GetMordProposalsforHabFinalizationBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_AGENCY, IMS_UPGRADE_CONNECT),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
            };
            return Json(jsonData);
        }

        public ActionResult DefinalizeHabitation(string parameter, string hash, string key)
        {
            int IMS_PR_ROAD_CODE = 0;

            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                    }

                    if (objProposalBAL.DefinalizeHabitationBAL(IMS_PR_ROAD_CODE))
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {
                //ErrorLog.LogError(ex, "DefinalizeHabitation()");
                ErrorLog.LogError(ex, "DefinalizeHabitation()");
                return Json(new { success = false });
            }
        }

        #endregion

        #region GEPNIC_INTEGRATION

        public ActionResult SearchProposalsForGEPNIC()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                GepnicProposalSearch model = new GepnicProposalSearch();

                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.lstState = objCommon.PopulateStates(true);
                }
                else
                {
                    model.lstState = new List<SelectListItem>();
                    model.lstState.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                }

                model.lstDistricts = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                model.lstDistricts.RemoveAt(0);
                model.lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                model.lstProposalTypes = objCommon.PopulateProposalTypes();
                model.lstSanctionYears = new SelectList(objCommon.PopulateFinancialYears(true, false).ToList(), "Value", "Text").ToList();
                model.lstBlocks = new List<SelectListItem>();
                model.lstBlocks.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                model.lstPackages = new List<SelectListItem>();
                model.lstPackages.Insert(0, new SelectListItem { Value = "0", Text = "All Packages" });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SearchProposalsForGEPNIC()");
                return null;
            }
        }

        public JsonResult PopulateSanctionedYearOnChnageofDistrict()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                //  int districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                return Json(objCommon.PopulateFinancialYear(true, true));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateSanctionedYearOnChnageofDistrict()");
                return null;
            }
        }

        public JsonResult PopulateSanctionedYear()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                //  int districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                return Json(objCommon.PopulateFinancialYears(true, false));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateSanctionedYearOnChnageofDistrict()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetGepnicProposalList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, Convert.ToInt32(rows), sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int District = Convert.ToInt32(Request.Params["District"]);
                int Year = Convert.ToInt32(Request.Params["Year"]);
                int Block = Convert.ToInt32(Request.Params["Block"]);
                string Package = Request.Params["Package"];
                string ProposalType = Request.Params["ProposalType"];
                int State = Convert.ToInt32(Request.Params["State"]);// PMGSYSession.Current.StateCode;

                int totalRecords;

                var jsonData = new
                {
                    rows = objProposalBAL.GetGepnicProposals(page.Value - 1, rows.Value, sidx, sord, out totalRecords, State, District, Year, Block, ProposalType, Package),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetGepnicProposalList()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GepnicOrgansationsLayout()
        {
            ProposalDAL objDAL = new DAL.Proposal.ProposalDAL();
            try
            {
                string[] proposals = Request.Params["proposals[]"].Split(',');
                if (proposals.Length == 0 || (proposals.Length == 1 && proposals[0].Trim() == string.Empty))
                {
                    return Json(new { success = false, message = "Please select at least one proposal." });
                }
                int State = Convert.ToInt32(Request.Params["State"]);
                CommonFunctions objCommon = new CommonFunctions();
                GepnicOrganisationsViewModel model = new GepnicOrganisationsViewModel();
                model.lstOrganisation = objDAL.PopulateGepnicOrganisationsDAL(State);//objDAL.PopulateGepnicOrganisationsDAL(PMGSYSession.Current.StateCode);
                model.proposalsIds = String.Join(",", proposals);//(string[])proposals.Clone();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SearchProposalsForGEPNIC()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult InsertGepnicProposals(/*string[] proposals*/ GepnicOrganisationsViewModel model)
        {
            ProposalDAL objProposalDAL = new ProposalDAL();
            string message = "";
            try
            {

                //proposals = Request.Params["proposals[]"].Split(',');
                string[] proposals = model.proposalsIds.Split(',');

                if (proposals.Length == 0 || (proposals.Length == 1 && proposals[0].Trim() == string.Empty))
                {
                    return Json(new { success = false, message = "Please select at least one proposal." });
                }
                else
                {
                    if (objProposalDAL.InsertGepnicProposalDetails(/*proposals*/ model, ref message))
                    {
                        return Json(new { success = true, message = "Proposal details sent to Gepnic Successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "InsertGepnicProposals()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

          [HttpPost]
        //public bool EnableRepush(String id)
        public bool EnableRepush(List<SelectListItem> roadcode)
        {
            Dictionary<string, string> decryptedParameters = null;
            PMGSYEntities dbContext = null;
            OMMAS_GEPNIC_INTEGRATION master = null;
            GepnicProposalSearch model = new GepnicProposalSearch();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    dbContext = new PMGSYEntities();
                    model = new GepnicProposalSearch();
                    master = new OMMAS_GEPNIC_INTEGRATION();
                    List<SelectListItem> roadcodelist = new List<SelectListItem>();
                    roadcodelist = roadcode;
                    int arr = 0;


                    foreach (var item in roadcodelist)
                    {
                        arr = (int.Parse(item.Value));
                    }
                    int ProposalID = Convert.ToInt32(arr);

                    string packageId = dbContext.OMMAS_GEPNIC_INTEGRATION.Where(m => m.ROAD_CODE == ProposalID).Select(m => m.PACKAGE_NUMBER).FirstOrDefault();

                    var siteCodesToUpdate = dbContext.OMMAS_GEPNIC_INTEGRATION.Where(m => m.PACKAGE_NUMBER == packageId && m.GEPNIC_NREGA == "G").Select(m => m.SITE_CODE).ToList();

                    if (siteCodesToUpdate.Count() > 0)
                    {
                        foreach (var siteCode in siteCodesToUpdate)
                        {

                            master = dbContext.OMMAS_GEPNIC_INTEGRATION.Where(m => m.SITE_CODE == siteCode).FirstOrDefault();
                            master.GEPNIC_NREGA = "N";
                            dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    scope.Complete();

                }

                return true;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var validationError in eve.ValidationErrors)
                    {
                        Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            validationError.PropertyName, validationError.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                throw;
            }
            catch (DbUpdateException e)
            {
                //throw e;
                ErrorLog.LogError(e, "Proposal.EnableRepush.DbUpdateException");
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "Proposal.EnableRepush");
                ModelState.AddModelError(string.Empty, "Error accured while processing your request.");
                return false;
                // return PartialView("SearchProposalsForGEPNIC", new GepnicProposalSearch());
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }

            }
        }

        [HttpPost]
        public JsonResult RoadCodeCheck(string id)
        {
            Models.PMGSYEntities dbContext;
            GepnicProposalSearch model = new GepnicProposalSearch();
            ProposalDAL objProposalDAL = new ProposalDAL();
            try
            {
                List<SelectListItem> roadcode = new List<SelectListItem>();
                roadcode = objProposalDAL.RoadCodeCheck(id);

                return Json(new { roadcode });


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.RoadCodeCheck()");
                return null;
            }


        }



        #endregion

        #region PROPOSAL_PROGRESS_LENGTH

        /// <summary>
        /// returns the list view of execution progress details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListProposalAdditionalLengthDetail()
        {
            CommonFunctions objCommon = new CommonFunctions();
            ProposalAdditionalCostFilterViewModel proposalModel = new ProposalAdditionalCostFilterViewModel();
            List<SelectListItem> lstBatches = new List<SelectListItem>();
            TransactionParams transactionParams = new TransactionParams();
            transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
            transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            transactionParams.ISSearch = true;
            transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;
            lstBatches = objCommon.PopulateBatch();
            lstBatches.RemoveAt(0);
            lstBatches.Add(new SelectListItem { Value = "0", Text = "All Batches" });

            proposalModel.States = objCommon.PopulateStates(false);
            proposalModel.States.Insert(0, (new SelectListItem { Text = "All States", Value = "0" }));
            proposalModel.MAST_State_CODE = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            proposalModel.States.Find(x => x.Value == proposalModel.MAST_State_CODE.ToString()).Selected = true;

            proposalModel.Districts = new List<SelectListItem>();
            if (proposalModel.MAST_State_CODE == 0)
            {
                proposalModel.Districts.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                proposalModel.Districts = objCommon.PopulateDistrict(proposalModel.MAST_State_CODE, true);
                proposalModel.MAST_District_CODE = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                proposalModel.Districts.Find(x => x.Value == "-1").Value = "0";
                proposalModel.Districts.Find(x => x.Value == proposalModel.MAST_District_CODE.ToString()).Selected = true;

            }
            proposalModel.BLOCKS = new List<SelectListItem>();
            if (proposalModel.MAST_District_CODE == 0)
            {
                proposalModel.BLOCKS.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                proposalModel.BLOCKS = objCommon.PopulateBlocks(proposalModel.MAST_District_CODE, true);
                proposalModel.BLOCKS.Find(x => x.Value == "-1").Value = "0";
                //BlockCode = PMGSYSession.Current.BlockCode == 0 ? 0 : PMGSYSession.Current.BlockCode;
                //BlockList.Find(x => x.Value == BlockCode.ToString()).Selected = true;
            }
            // proposalModel.BATCHS = lstBatches;
            // proposalModel.BLOCKS = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
            proposalModel.Years = PopulateYear(0, true, true);
            proposalModel.Years.Find(x => x.Value == "-1").Value = "0";
            proposalModel.STREAMS = objCommon.PopulateStreams("", true);
            proposalModel.PROPOSAL_TYPES = objCommon.PopulateProposalTypes();

            //new filters added by Vikram 
            proposalModel.lstBatchs = objCommon.PopulateBatch(true);
            proposalModel.lstCollaborations = objCommon.PopulateFundingAgency(true);
            proposalModel.lstUpgradations = objCommon.PopulateNewUpgradeList(true);
            //end of change

            List<SelectListItem> lstPackages = objCommon.PopulatePackage(transactionParams);
            lstPackages.RemoveAt(0);
            lstPackages.Insert(0, new SelectListItem { Value = "All", Text = "All Packages" });
            proposalModel.PACKAGES = lstPackages;
            return View(proposalModel);
        }

        /// <summary>
        /// returns the list of execution progress details 
        /// </summary>
        /// <param name="executionCollection">contains the filters and grid parameters</param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetProposalAdditionalLengthList(int? page, int? rows, string sidx, string sord)
        {
            int yearCode = 0;
            int blockCode = 0;
            int streamCollaborationCode = 0;
            int batchCode = 0;
            int stateCode = 0;
            int districtCode = 0;
            string proposalCode = string.Empty;
            string packageCode = string.Empty;
            long totalRecords = 0;
            string upgradationType = string.Empty;
            ProposalDAL objProposalDAL = new ProposalDAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
            {
                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
            {
                districtCode = Convert.ToInt32(Request.Params["districtCode"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
            {
                yearCode = Convert.ToInt32(Request.Params["yearCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
            {
                blockCode = Convert.ToInt32(Request.Params["blockCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
            {
                streamCollaborationCode = Convert.ToInt32(Request.Params["collaboration"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
            {
                batchCode = Convert.ToInt32(Request.Params["batchCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
            {
                proposalCode = Request.Params["proposalCode"];
            }

            if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
            {
                packageCode = Request.Params["packageCode"];
            }

            if (!string.IsNullOrEmpty(Request.Params["upgradationType"]))
            {
                upgradationType = Request.Params["upgradationType"];
            }

            var jsonData = new
            {
                rows = objProposalDAL.GetProposalAdditionalLengthListDAL(stateCode, districtCode, blockCode, yearCode, packageCode, proposalCode, batchCode, streamCollaborationCode, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// returns the view for adding the additional progress length
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddAdditionLength(string id)
        {
            try
            {
                IProposalBAL objProposalBAL = new ProposalBAL();
                Dictionary<string, string> decryptedParameters = null;
                string[] encryptedParams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                int proposalCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"]);
                AdditionalLengthViewModel model = new AdditionalLengthViewModel();
                model.EncProposalCode = URLEncrypt.EncryptParameters1(new string[] { "IMS_PR_ROAD_CODE=" + proposalCode });
                model.IMS_PR_ROAD_CODE = proposalCode;
                IMS_SANCTIONED_PROJECTS sanctionDetails = objProposalBAL.GetRoadDetails(proposalCode);
                model.IMS_YEAR = sanctionDetails.IMS_YEAR;
                model.IMS_BATCH = sanctionDetails.IMS_BATCH;
                model.IMS_PACKAGE_ID = sanctionDetails.IMS_PACKAGE_ID;
                model.IMS_ROAD_NAME = sanctionDetails.IMS_ROAD_NAME;
                //LSB change 05NOV2019
                model.ProposalType = sanctionDetails.IMS_PROPOSAL_TYPE.Trim();
                model.IMS_PAV_LENGTH = sanctionDetails.IMS_PROPOSAL_TYPE == "P" ? sanctionDetails.IMS_PAV_LENGTH : sanctionDetails.IMS_BRIDGE_LENGTH;
                model.IMS_STATE_AMOUNT_TEXT = Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_BS_AMT) + Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_RS_AMT);
                model.IMS_MORD_AMOUNT_TEXT = Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_PAV_AMT) + Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_CD_AMT) + Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_OW_AMT) + Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_FC_AMT) + Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_BW_AMT);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddAdditionLength()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveAdditionalLengthDetails(AdditionalLengthViewModel model)
        {
            try
            {
                ProposalDAL objProposalDAL = new ProposalDAL();

                if (ModelState.IsValid)
                {
                    if (db.IMS_PROGRESS_LENGTH_COMPLETION.Any(m => m.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE))
                    {
                        return Json(new { success = false, message = "Only one change in length is allowed." });
                    }

                    if (objProposalDAL.SaveAdditionalLengthDetails(model))
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, errorMessage = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, errorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveAdditionalLengthDetails()");
                return Json(new { success = false, errorMessage = "Error occurred while processing your request." });
            }
        }

        [HttpPost]
        public ActionResult UpdateAdditionalLengthDetails(AdditionalLengthViewModel model)
        {
            try
            {
                ProposalDAL objProposalDAL = new ProposalDAL();

                if (ModelState.IsValid)
                {
                    if (objProposalDAL.UpdateAdditionalLengthDetails(model))
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, errorMessage = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, errorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateAdditionalLengthDetails()");
                return Json(new { success = false, errorMessage = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the view for updating the additional length details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditAdditionalLength(String parameter, String hash, String key)
        {
            try
            {
                ProposalDAL objProposalDAL = new ProposalDAL();

                Dictionary<string, string> decryptedParameters = null;

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int transationCode = Convert.ToInt32(decryptedParameters["TransactionCode"].ToString());
                    int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"].ToString());

                    AdditionalLengthViewModel model = objProposalDAL.GetAdditionalProgressLengthDetails(imsPrRoadCode, transationCode);
                    IMS_SANCTIONED_PROJECTS sanctionDetails = objProposalBAL.GetRoadDetails(imsPrRoadCode);
                    model.EncProposalCode = URLEncrypt.EncryptParameters1(new string[] { "IMS_PR_ROAD_CODE=" + imsPrRoadCode.ToString().Trim() });
                    model.IMS_YEAR = sanctionDetails.IMS_YEAR;
                    model.IMS_BATCH = sanctionDetails.IMS_BATCH;
                    model.IMS_PACKAGE_ID = sanctionDetails.IMS_PACKAGE_ID;
                    model.IMS_ROAD_NAME = sanctionDetails.IMS_ROAD_NAME;
                    model.IMS_PAV_LENGTH = sanctionDetails.IMS_PAV_LENGTH;
                    model.IMS_STATE_AMOUNT_TEXT = Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_BS_AMT) + Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_RS_AMT);
                    model.IMS_MORD_AMOUNT_TEXT = Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_PAV_AMT) + Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_CD_AMT) + Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_OW_AMT) + Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_FC_AMT) + Convert.ToDecimal(sanctionDetails.IMS_SANCTIONED_BW_AMT);

                    if (model != null)
                    {
                        return PartialView("AddAdditionLength", model);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditAdditionalLength()");
                return Json(new { success = false, errorMessage = "Error occurred while processing your request." });
            }
        }

        [Audit]
        public ActionResult GetAdditionalLengthList(FormCollection formCollection)
        {

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            long totalRecords = 0;
            int IMS_PR_ROAD_CODE = 0;

            ProposalDAL objProposalDAL = new ProposalDAL();

            try
            {


                if (!string.IsNullOrEmpty(formCollection["IMS_PR_ROAD_CODE"]))
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"]);
                }


                var jsonData = new
                {
                    rows = objProposalDAL.GetAdditionalLengthListDAL(IMS_PR_ROAD_CODE, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetAdditionalLengthList()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeleteAdditionalLengthDetails(string parameter, string hash, string key)
        {
            try
            {
                ProposalDAL objProposalDAL = new ProposalDAL();

                Dictionary<string, string> decryptedParameters = null;

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int transactionCode = Convert.ToInt32(decryptedParameters["TransactionCode"].ToString());
                    int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"].ToString());

                    if (objProposalDAL.DeleteAdditionLengthDetails(transactionCode, imsPrRoadCode))
                    {
                        message = message == string.Empty ? "Additional Cost details deleted successfully." : message;

                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Additional Cost details not deleted." : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                message = "An error occured while processing you request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteAdditionalLengthDetails()");
                throw;
            }
        }


        [HttpPost]
        public ActionResult ApproveRejectAdditionalLengthDetails(string parameter, string hash, string key)
        {
            try
            {
                ProposalDAL objProposalDAL = new ProposalDAL();

                Dictionary<string, string> decryptedParameters = null;

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int transactionCode = Convert.ToInt32(decryptedParameters["TransactionCode"].ToString());
                    int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"].ToString());

                    string approveReject = Request.Params["ApproveReject"];

                    if (objProposalDAL.ApproveRejectAdditionalLengthDetails(imsPrRoadCode, transactionCode, approveReject))
                    {
                        message = message == string.Empty ? "Additional Cost details deleted successfully." : message;

                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Additional Cost details not deleted." : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                message = "An error occured while processing you request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ApproveRejectAdditionalLengthDetails()");
                throw;
            }
        }

        #endregion

        #region Dropped Proposal[by Pradip Patil (10/04/2017)]


        [HttpGet]
        public ActionResult ListProposalForDropping()
        {
            ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            List<SelectListItem> lstTypes = new List<SelectListItem>();
            lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
            lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
            proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
            proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(false);

            proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, true);
            proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
            proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
            proposalViewModel.CONNECTIVITYLIST = lstTypes;
            proposalViewModel.BATCHS.RemoveAt(0);
            proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "Select Batch", Value = "0", Selected = true }));
            proposalViewModel.BLOCKS.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            ///Changes by SAMMED A. PATIL on 02JAN2018 for Bridge Proposal
            proposalViewModel.PROPOSAL_TYPES = new List<SelectListItem>();//objCommonFuntion.PopulateProposalTypes();
            proposalViewModel.PROPOSAL_TYPES.Insert(0, new SelectListItem { Text = "Road", Value = "P" });
            proposalViewModel.PROPOSAL_TYPES.Insert(1, new SelectListItem { Text = "Bridge", Value = "L" });

            proposalViewModel.Years = PopulateYear(0, true, true);
            proposalViewModel.IMS_YEAR = DateTime.Now.Year;
            //proposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("", true);
            proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;

            return View(proposalViewModel);
        }

        /// <summary>
        /// Get proposals for SRRDA for Dropping 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult GetDroppingProposalsForSRRDA(FormCollection formCollection)
        {
            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 29-Apr-2014 end
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
            string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
            int MAST_BLOCK_CODE = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
            int totalRecords;
            ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
            var jsonData = new
            {
                rows = objProposalBAL.GetDroppingProposalsForSRRDABAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, formCollection["filters"], out colTotal),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
                TotalColumn = colTotal
            };
            return Json(jsonData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PopulateFinancialYearsByStateForDropping()
        {
            try
            {
                PMGSY.DAL.Proposal.ProposalDAL objDAL = new PMGSY.DAL.Proposal.ProposalDAL();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                return Json(objDAL.PopulateFinancialYearsByStateForDroppingDAL(stateCode, false));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateFinancialYearsByStateForDropping()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddWorksforDropping(string roadCode)
        {
            AddDropOrderViewModel model = new AddDropOrderViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            try
            {
                model.encryptedRoadCode = roadCode;
                model.lstDropReason = new CommonFunctions().PopulateReason("S");
                model.expenditureIncurred = Convert.ToDecimal(Request.Params["expIncurred"]);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddWorksforDropping()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DropProposal(string[] droppedArray)  //, String [] filter
        {
            Dictionary<string, string> decryptedParam = new Dictionary<string, string>();
            String result = "Error occured while assigning proposal for dropping";
            Boolean status = false;
            try
            {
                //string[] encparamValues = droppedArray;//droppedArray.Split(',');
                List<int> imsRoadCodeList = new List<int>();

                for (int i = 0; i < droppedArray.Length; i++)
                {
                    string[] singleEncoded = droppedArray[i].Split('/');
                    decryptedParam = URLEncrypt.DecryptParameters1(new String[] { singleEncoded[0], singleEncoded[1], singleEncoded[2] });
                    int ImsRoadCode = Convert.ToInt32(decryptedParam["ImsRoadCode"]);
                    imsRoadCodeList.Add(ImsRoadCode);
                }
                status = objProposalBAL.DropProposal(imsRoadCodeList, out result);
                if (status)
                    return Json(new { success = status, message = result });
                return Json(new { success = status, message = result });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DropProposal(string[] droppedArray)");
                return Json(new { success = status, message = result });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddDropProposal(AddDropOrderViewModel model)  //, String [] filter
        {
            Dictionary<string, string> decryptedParam = new Dictionary<string, string>();
            String result = "Error occured while assigning proposal for dropping";
            Boolean status = false;
            string[] arrEncrCode = model.encryptedRoadCode.Split('/');
            try
            {
                if (model.expenditureIncurred > model.recoupAmt)
                {
                    return Json(new { success = status, message = "Please enter Recoup Amount greater than or equal to the Expenditure Incurred" });
                }
                decryptedParam = URLEncrypt.DecryptParameters1(new String[] { arrEncrCode[0], arrEncrCode[1], arrEncrCode[2] });
                model.imsPrRoadCode = Convert.ToInt32(decryptedParam["ImsRoadCode"]);
                status = objProposalBAL.AddDropProposalBAL(model, out result);
                if (status)
                    return Json(new { success = status, message = result });
                return Json(new { success = status, message = result });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddDropProposal(AddDropOrderViewModel model)");
                return Json(new { success = status, message = result });
            }
        }

        [HttpGet]
        public ActionResult DropLetterGenerationLayout()
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            bool flag = false;
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DropLetterGenerationLayout()");
                return null;
            }
        }

        /// <summary>
        /// List matix Param Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        ///
        [HttpPost]
        public ActionResult ListDropppingWorks(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            int stateCode = 0;
            long totalRecords = 0;
            objBAL = new MasterBAL();
            //objProposalBAL
            try
            {
                if (PMGSYSession.Current.StateCode > 0)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                }
                else if (!string.IsNullOrEmpty(formCollection["stateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["stateCode"]);
                }

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objProposalBAL.ListDropppingWorksBAL(stateCode, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "ListWorksForDroppping()");
                return null;
            }
        }

        /// <summary>
        /// List matix Param Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        ///
        [HttpPost]
        public ActionResult ListWorksForDroppping(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            int reqCode = 0;
            long totalRecords = 0;
            objBAL = new MasterBAL();
            //objProposalBAL
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["reqCode"]))
                {
                    reqCode = Convert.ToInt32(Request.Params["reqCode"]);
                }
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objProposalBAL.ListWorksForDropppingBAL(reqCode, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "ListWorksForDroppping()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DropLetterRequestLayout(string[] DropDetails)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            bool flag = false;
            try
            {
                TempData["arrDropDetails"] = DropDetails;
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DropLetterRequestLayout()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddDropRequestDetails(string letterNo)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            bool status = false;
            string result = "";
            try
            {
                if (string.IsNullOrEmpty(letterNo))
                {
                    return Json(new { success = status, message = "Please select works for dropping." });
                }
                Regex regex = new Regex(@"^[a-zA-Z0-9 -._/()]+$");
                if (!regex.IsMatch(letterNo))
                {
                    return Json(new { success = status, message = "Letter No. contains invalid characters, Letter No. should be alphanumeric" });
                }

                String[] DropDetails = (String[])TempData["arrDropDetails"];
                if (DropDetails.Length > 0)
                {
                    status = objProposalBAL.AddDropRequestDetailsBAL(DropDetails, letterNo, out result);
                    if (status)
                        return Json(new { success = status, message = result });
                }
                return Json(new { success = status, message = "Please select works for dropping." });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddDropProposal(string letterNo)");
                return Json(new { success = status, message = result });
            }
        }

        [HttpGet]
        public ActionResult GetDropRequestPDF(string id)
        {
            try
            {
                //string[] parameters = id.Split('$');
                //int scheme = Convert.ToInt32(parameters[0]);
                int scheme = PMGSYSession.Current.PMGSYScheme;
                string filename = id;//parameters[1];

                var filePath = (scheme == 1 ? ConfigurationManager.AppSettings["DROP_REQUEST_PDF_PMGSYI"] : scheme == 2 ? ConfigurationManager.AppSettings["DROP_REQUEST_PDF_PMGSYII"] : scheme == 3 ? ConfigurationManager.AppSettings["DROP_REQUEST_PDF_RCPLWE"] : ConfigurationManager.AppSettings["DROP_REQUEST_PDF_PMGSYIII"]) + filename;
                if (System.IO.File.Exists(filePath))
                {
                    Byte[] file = System.IO.File.ReadAllBytes(filePath);

                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        // for example foo.bak
                        FileName = filename,

                        // always prompt the user for downloading, set to true if you want 
                        // the browser to try to show the file inline
                        Inline = false,

                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(file, "application/pdf");
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetDropOrder()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult ListProposalsForDroppingOrderMRD()
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                ViewBag.StateList = objCommon.PopulateStates(true);
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListProposalsForDroppingOrder()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult ListProposalsForDroppingOrder()
        {
            SanctionOrderFilterModel filterModel = new SanctionOrderFilterModel();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                filterModel.StateList = objCommon.PopulateStates(true);
                filterModel.BatchList = objCommon.PopulateBatch();
                filterModel.BatchList.Where(s => s.Value == "0").FirstOrDefault().Text = "All";

                filterModel.StreamList = objCommon.PopulateFundingAgency(true);
                filterModel.StreamList.Where(x => x.Value == "-1").FirstOrDefault().Value = "0";
                //filterModel.YearList = new SelectList(objCommon.PopulateFinancialYear(true, false).ToList(), "Value", "Text").ToList();
                filterModel.YearList = new List<SelectListItem>();
                filterModel.YearList.Insert(0, new SelectListItem() { Text = "Select Year", Value = "-1" });

                return View(filterModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListProposalsForDroppingOrder()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDroppedProposalListByBatch(int? page, int? rows, string sidx, string sord, string reqCode)
        {

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            objProposalBAL = new ProposalBAL();
            long totalRecords = 0;
            int stateCode = 0;
            int year = 0;
            int streamCode = 0;
            int batch = 0;
            int scheme = 0;
            string proposalType = string.Empty;
            bool IsDOGenerated = false;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["StateCode"])))
                {
                    stateCode = Convert.ToInt32(Request.Params["StateCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["YearCode"])))
                {
                    year = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["StreamCode"])))
                {
                    streamCode = Convert.ToInt32(Request.Params["StreamCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BatchCode"])))
                {
                    batch = Convert.ToInt32(Request.Params["BatchCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["Scheme"])))
                {
                    scheme = Convert.ToInt32(Request.Params["Scheme"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["Type"])))
                {
                    proposalType = Request.Params["Type"];
                }

                var jsonData = new
                {
                    rows = objProposalBAL.GetProposalsForDroppedOrder(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, stateCode, year, streamCode, batch, scheme, proposalType, out IsDOGenerated, reqCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    IsDOGenerated = IsDOGenerated,
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetDroppedProposalListByBatch()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        [HttpGet]
        public ActionResult DropOrderView(string[] ApproveRoads)
        {
            DropOrderViewModel model = new DropOrderViewModel();
            PMGSY.DAL.Proposal.ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();
            decryptedParameters = new Dictionary<string, string>();
            try
            {
                List<int> imsRoadCodeList = new List<int>();

                for (int i = 0; i < ApproveRoads.Length; i++)
                {
                    string[] singleEncoded = ApproveRoads[i].Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { singleEncoded[0], singleEncoded[1], singleEncoded[2] });
                    int ImsRoadCode = Convert.ToInt32(decryptedParameters["ImsRoadCode"]);
                    imsRoadCodeList.Add(ImsRoadCode);
                }

                TempData["mrdSelectedRoads"] = imsRoadCodeList;

                if (!String.IsNullOrEmpty(Request.Params["StateCode"]))
                {
                    model.StateCode = Convert.ToInt32(Request.Params["StateCode"]);
                }

                //if (!String.IsNullOrEmpty(Request.Params["YearCode"]))
                //{
                //    model.YearCode = Convert.ToInt32(Request.Params["YearCode"]);
                //}

                //if (!String.IsNullOrEmpty(Request.Params["StreamCode"]))
                //{
                //    model.StreamCode = Convert.ToInt32(Request.Params["StreamCode"]);
                //}

                //if (!String.IsNullOrEmpty(Request.Params["BatchCode"]))
                //{
                //    model.BatchCode = Convert.ToInt32(Request.Params["BatchCode"]);
                //}

                //if (!String.IsNullOrEmpty(Request.Params["SchemeCode"]))
                //{
                //    model.PMGSYScheme = Convert.ToInt32(Request.Params["SchemeCode"]);
                //}
                model.PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
                if (!String.IsNullOrEmpty(Request.Params["RequestCode"]))
                {
                    model.RequestCode = Convert.ToInt32(Request.Params["RequestCode"]);
                }

                model.IMS_REQUEST_ORDER_DATE = objProposalDAL.getRequestLetterDateDAL(model.RequestCode);
                //model.IsDOGenerated = objProposalDAL.IsDropOrderGenerated(model);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DropOrderView()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDropOrder(DropOrderViewModel model)
        {
            string message = string.Empty;
            try
            {
                List<int> mrdselectedroadList = (List<int>)TempData["mrdSelectedRoads"];
                if (ModelState.IsValid)
                {
                    bool Status = objProposalBAL.AddDropOrderBAL(model, mrdselectedroadList, ref message);
                    if (Status)
                        return Json(new { success = true, message = message });
                    else
                        return Json(new { success = false, message = message });
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddDropOrder()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        //[HttpPost]
        //public ActionResult AddDropOrder(string [] dropApproveArray)
        //{
        //    string message = string.Empty;
        //    DropOrderViewModel model = new DropOrderViewModel();
        //    try
        //    {
        //        if (!String.IsNullOrEmpty(Request.Params["StateCode"]))
        //        {
        //            model.StateCode = Convert.ToInt32(Request.Params["StateCode"]);
        //        }

        //        if (!String.IsNullOrEmpty(Request.Params["YearCode"]))
        //        {
        //            model.YearCode = Convert.ToInt32(Request.Params["YearCode"]);
        //        }

        //        if (!String.IsNullOrEmpty(Request.Params["StreamCode"]))
        //        {
        //            model.StreamCode = Convert.ToInt32(Request.Params["StreamCode"]);
        //        }

        //        if (!String.IsNullOrEmpty(Request.Params["BatchCode"]))
        //        {
        //            model.BatchCode = Convert.ToInt32(Request.Params["BatchCode"]);
        //        }

        //        if (!String.IsNullOrEmpty(Request.Params["SchemeCode"]))
        //        {
        //            model.PMGSYScheme = Convert.ToInt32(Request.Params["SchemeCode"]);
        //        }

        //        bool Status = objProposalBAL.AddDropOrderBAL(model, dropApproveArray, ref message);
        //            if (Status)
        //                return Json(new { success = true, message = message });
        //            else
        //                return Json(new { success = false, message = message });


        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "AddDropOrder()");
        //        return Json(new { success = false, message = "Error occurred while processing your request." });
        //    }
        //}

        /// <summary>
        /// returns the pdf file for view of drop order
        /// </summary>
        /// <returns></returns>
        /// 

        //public ActionResult PreviewDropOrderReport()
        //{
        //    SanctionOrderFilterModel model = new SanctionOrderFilterModel();
        //    CommonFunctions objCommonFuntion = new CommonFunctions();
        //    try
        //    {
        //        if (!(string.IsNullOrEmpty(Request.Params["StateCode"])))
        //        {
        //            model.State = Convert.ToInt32(Request.Params["StateCode"]);
        //        }

        //        if (!(string.IsNullOrEmpty(Request.Params["YearCode"])))
        //        {
        //            model.Year = Convert.ToInt32(Request.Params["YearCode"]);
        //        }

        //        if (!(string.IsNullOrEmpty(Request.Params["StreamCode"])))
        //        {
        //            model.Stream = Convert.ToInt32(Request.Params["StreamCode"]);
        //        }

        //        if (!(string.IsNullOrEmpty(Request.Params["BatchCode"])))
        //        {
        //            model.Batch = Convert.ToInt32(Request.Params["BatchCode"]);
        //        }

        //        if (!String.IsNullOrEmpty(Request.Params["SchemeCode"]))
        //        {
        //            model.PMGSYScheme = Convert.ToInt32(Request.Params["SchemeCode"]);
        //        }

        //        if (!String.IsNullOrEmpty(Request.Params["ProposalType"]))
        //        {
        //            model.ProposalType = Request.Params["ProposalType"];
        //        }

        //        IMS_DROPPED_PROJECTS_PDF sanctionModel = new IMS_DROPPED_PROJECTS_PDF();
        //        sanctionModel = db.IMS_DROPPED_PROJECTS_PDF.Where(m => m.MAST_STATE_CODE == model.State && m.IMS_BATCH == model.Batch && m.IMS_COLLABORATION == model.Stream && m.IMS_YEAR == model.Year && m.MAST_PMGSY_SCHEME == model.PMGSYScheme).FirstOrDefault();

        //        if (sanctionModel != null)
        //        {
        //            model.BatchName = "Batch : " + model.Batch;
        //            model.StateName = sanctionModel.MASTER_STATE.MAST_STATE_NAME;
        //            model.CollaborationName = sanctionModel.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_NAME;
        //            model.DropOrderNo = sanctionModel.IMS_ORDER_NUMBER;
        //            model.DropOrderDate = objCommonFuntion.GetDateTimeToString(sanctionModel.IMS_ORDER_DATE);
        //        }
        //        else
        //        {
        //            model.BatchName = "Batch : " + model.Batch;
        //            model.StateName = db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_NAME).FirstOrDefault();
        //            model.CollaborationName = db.MASTER_FUNDING_AGENCY.Where(m => m.MAST_FUNDING_AGENCY_CODE == model.Stream).Select(m => m.MAST_FUNDING_AGENCY_NAME).FirstOrDefault();
        //            model.DropOrderNo = "-";
        //            model.DropOrderDate = "-";
        //        }

        //        Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
        //        rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);

        //        System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("State", model.State.ToString()));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("District", "0"));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Block", "0"));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Year", model.Year.ToString()));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Batch", model.Batch.ToString()));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Collaboration", model.Stream.ToString()));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Status", model.ProposalType));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PMGSY", model.PMGSYScheme.ToString()));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", model.StateName));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("BatchName", (model.Batch == null ? "-" : model.Batch.ToString())));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("CollaborationName", model.CollaborationName));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DropLetterNo", (model.DropOrderNo == null ? "-" : model.DropOrderNo)));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DropDate", (model.DropOrderDate == null ? "-" : model.DropOrderDate)));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("UserId", PMGSY.Extensions.PMGSYSession.Current.UserId.ToString()));

        //        Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Username"], System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Password"]);
        //        rview.ServerReport.ReportServerCredentials = irsc;
        //        rview.ServerReport.ReportPath = "/PMGSYCitizen/DropProposalList";
        //        rview.ServerReport.SetParameters(paramList);
        //        string mimeType, encoding, extension, deviceInfo;
        //        string[] streamids;
        //        Microsoft.Reporting.WebForms.Warning[] warnings;
        //        string format = "PDF"; //Desired format goes here (PDF, Excel, or Image)

        //        deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>True</SimplePageHeaders>" + "</DeviceInfo>";
        //        byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
        //        var fileName = (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.Year + "-" + (model.Year + 1)) + "_BATCH" + model.Batch + "_" + (model.CollaborationName) + "_SCHEME" + model.PMGSYScheme + ".pdf";

        //        String DirectoryPath = model.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["DROP_ORDER_PDF_PMGSYI"].ToString() : ConfigurationManager.AppSettings["DROP_ORDER_PDF_PMGSYII"].ToString();
        //        if (!Directory.Exists(DirectoryPath))
        //             Directory.CreateDirectory(DirectoryPath);

        //        string filePath = model.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"].ToString() + fileName + ".pdf" : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"].ToString() + fileName + ".pdf";
        //        Response.Clear();

        //        var cd = new System.Net.Mime.ContentDisposition
        //        {
        //            // for example foo.bak
        //            FileName = fileName,

        //            // always prompt the user for downloading, set to true if you want 
        //            // the browser to try to show the file inline
        //            Inline = false,

        //        };

        //        Response.AppendHeader("Content-Disposition", cd.ToString());

        //        return File(bytes, "application/pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "PreviewDropOrderReport()");
        //        return null;
        //    }
        //}


        //public ActionResult PreviewDistrictAbstractDropReport()
        //{
        //    SanctionOrderFilterModel model = new SanctionOrderFilterModel();
        //    CommonFunctions objCommonFuntion = new CommonFunctions();
        //    try
        //    {
        //        if (!(string.IsNullOrEmpty(Request.Params["StateCode"])))
        //        {
        //            model.State = Convert.ToInt32(Request.Params["StateCode"]);
        //        }

        //        if (!(string.IsNullOrEmpty(Request.Params["YearCode"])))
        //        {
        //            model.Year = Convert.ToInt32(Request.Params["YearCode"]);
        //        }

        //        if (!(string.IsNullOrEmpty(Request.Params["StreamCode"])))
        //        {
        //            model.Stream = Convert.ToInt32(Request.Params["StreamCode"]);
        //        }

        //        if (!(string.IsNullOrEmpty(Request.Params["BatchCode"])))
        //        {
        //            model.Batch = Convert.ToInt32(Request.Params["BatchCode"]);
        //        }

        //        if (!String.IsNullOrEmpty(Request.Params["SchemeCode"]))
        //        {
        //            model.PMGSYScheme = Convert.ToInt32(Request.Params["SchemeCode"]);
        //        }

        //        if (!String.IsNullOrEmpty(Request.Params["ProposalType"]))
        //        {
        //            model.ProposalType = Request.Params["ProposalType"];
        //        }

        //        IMS_SANCTIONED_PROJECTS_PDF sanctionModel = new IMS_SANCTIONED_PROJECTS_PDF();
        //        sanctionModel = db.IMS_SANCTIONED_PROJECTS_PDF.Where(m => m.MAST_STATE_CODE == model.State && m.IMS_BATCH == model.Batch && m.IMS_COLLABORATION == model.Stream && m.IMS_YEAR == model.Year && m.MAST_PMGSY_SCHEME == model.PMGSYScheme).FirstOrDefault();
        //        if (sanctionModel != null)
        //        {
        //            // string FullFilePhysicalPath = Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYI"] : ConfigurationManager.AppSettings["SANCTION_ORDER_PDF_PMGSYII"], sanctionModel.IMS_PDF_NAME);
        //        }
        //        if (sanctionModel != null)
        //        {
        //            model.BatchName = "Batch : " + model.Batch;
        //            model.StateName = sanctionModel.MASTER_STATE.MAST_STATE_NAME;
        //            model.CollaborationName = sanctionModel.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_NAME;
        //            model.SanctionOrderNo = sanctionModel.IMS_ORDER_NUMBER;
        //            model.SanctionOrderDate = objCommonFuntion.GetDateTimeToString(sanctionModel.IMS_ORDER_DATE);
        //        }
        //        else
        //        {
        //            model.BatchName = "Batch : " + model.Batch;
        //            model.StateName = db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_NAME).FirstOrDefault();
        //            model.CollaborationName = db.MASTER_FUNDING_AGENCY.Where(m => m.MAST_FUNDING_AGENCY_CODE == model.Stream).Select(m => m.MAST_FUNDING_AGENCY_NAME).FirstOrDefault();
        //            model.SanctionOrderNo = "-";
        //            model.SanctionOrderDate = "-";
        //        }



        //        Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
        //        rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);

        //        System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("State", model.State.ToString()));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("District", "0"));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Block", "0"));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Year", model.Year.ToString()));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Batch", model.Batch.ToString()));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Collaboration", model.Stream.ToString()));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Status", model.ProposalType));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PMGSY", model.PMGSYScheme.ToString()));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StateName", model.StateName));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("BlockName", "-"));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("DistName", "-"));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("BatchName", (model.Batch == null ? "-" : model.Batch.ToString())));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("CollaborationName", model.CollaborationName));
        //        paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("StatusName", "-"));
        //        Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Username"], System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Password"]);
        //        rview.ServerReport.ReportServerCredentials = irsc;
        //        rview.ServerReport.ReportPath = "/PMGSYCitizen/DroppedAbstractProposalList";
        //        rview.ServerReport.SetParameters(paramList);
        //        string mimeType, encoding, extension, deviceInfo;
        //        string[] streamids;
        //        Microsoft.Reporting.WebForms.Warning[] warnings;
        //        string format = "PDF"; //Desired format goes here (PDF, Excel, or Image)

        //        deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>True</SimplePageHeaders>" + "</DeviceInfo>";
        //        byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
        //        var fileName = (db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.State).Select(m => m.MAST_STATE_SHORT_CODE).FirstOrDefault()) + "_" + (model.Year + "-" + (model.Year + 1)) + "_BATCH" + model.Batch + "_" + (model.CollaborationName) + "_SCHEME" + model.PMGSYScheme;
        //        fileName = fileName + ".pdf";
        //        Response.Clear();
        //        var cd = new System.Net.Mime.ContentDisposition
        //        {
        //            FileName = fileName,
        //            Inline = false,

        //        };

        //        Response.AppendHeader("Content-Disposition", cd.ToString());

        //        return File(bytes, "application/pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "PreviewDistrictAbstractDropReport()");
        //        return null;
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDropOrderList(int? page, int? rows, string sidx, string sord)
        {
            objProposalBAL = new ProposalBAL();
            long totalRecords = 0;
            int stateCode = 0;
            int year = 0;
            int streamCode = 0;
            int batch = 0;
            int scheme = 0;
            String Status = String.Empty;
            string proposalType = string.Empty;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["StateCode"])))
                {
                    stateCode = Convert.ToInt32(Request.Params["StateCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["YearCode"])))
                {
                    year = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["StreamCode"])))
                {
                    streamCode = Convert.ToInt32(Request.Params["StreamCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BatchCode"])))
                {
                    batch = Convert.ToInt32(Request.Params["BatchCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["Scheme"])))
                {
                    scheme = Convert.ToInt32(Request.Params["Scheme"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["Type"])))
                {
                    proposalType = Request.Params["Type"];
                }

                if (!(String.IsNullOrEmpty(Request.Params["Status"])))
                {
                    Status = Request.Params["Status"];
                }
                var jsonData = new
                {
                    rows = objProposalBAL.GetDropOrderListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, stateCode, year, streamCode, batch, scheme, proposalType, Status),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetDropOrderList()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDetailDropOrderList(int? page, int? rows, String sidx, String sord)
        {
            int RequestCode = 0;
            objProposalBAL = new ProposalBAL();
            long totalRecords = 0;
            int scheme = 0;
            try
            {
                if (!(string.IsNullOrEmpty(Request.Params["RequestCode"])))
                {
                    RequestCode = Convert.ToInt32(Request.Params["RequestCode"]);
                }
                if (!(string.IsNullOrEmpty(Request.Params["Scheme"])))
                {
                    scheme = Convert.ToInt32(Request.Params["Scheme"]);
                }
                var jsonData = new
                {
                    rows = objProposalBAL.GetDetailDropOrderListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, RequestCode, scheme),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetDetailDropOrderList()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetDropOrder(string id)
        {
            try
            {
                string[] parameters = id.Split('$');
                //int state = Convert.ToInt32(parameters[0]);
                //int batch = Convert.ToInt32(parameters[1]);
                //int collaboration = Convert.ToInt32(parameters[2]);
                //int year = Convert.ToInt32(parameters[3]);
                //int scheme = Convert.ToInt32(parameters[0]);

                int scheme = PMGSYSession.Current.PMGSYScheme;
                string filename = parameters[1];

                var filePath = (scheme == 1 ? ConfigurationManager.AppSettings["DROP_ORDER_PDF_PMGSYI"] : scheme == 2 ? ConfigurationManager.AppSettings["DROP_ORDER_PDF_PMGSYII"] : scheme == 3 ? ConfigurationManager.AppSettings["DROP_ORDER_PDF_RCPLWE"] : ConfigurationManager.AppSettings["DROP_ORDER_PDF_PMGSYIII"]) + filename;
                if (System.IO.File.Exists(filePath))
                {
                    Byte[] file = System.IO.File.ReadAllBytes(filePath);

                    //Response.ContentType = "application/pdf";

                    //Response.AddHeader("Content-disposition", "filename=" + filename + ".pdf");

                    //Response.OutputStream.Write(file, 0, file.Length);

                    //Response.OutputStream.Flush();

                    //Response.OutputStream.Close();

                    //Response.Flush();

                    //Response.Close();

                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        // for example foo.bak
                        FileName = filename,

                        // always prompt the user for downloading, set to true if you want 
                        // the browser to try to show the file inline
                        Inline = false,

                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(file, "application/pdf");
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetDropOrder()");
                return null;
            }
        }
        #endregion

        #region Matrix Master
        [HttpGet]
        public ActionResult MatrixMasterLayout()
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            bool flag = false;
            try
            {
                ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();
                ViewBag.flag = objProposalDAL.checkIsStateEntered();
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MatrixMasterLayout()");
                return null;
            }
        }

        /// <summary>
        /// List matix Param Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        ///
        [HttpPost]
        public ActionResult GetMatrixParamDetailsList(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            long totalRecords = 0;
            objBAL = new MasterBAL();
            //objProposalBAL
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objProposalBAL.ListMatrixParametersDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetMatrixParamDetailsList()");
                return null;
            }
        }

        /// <summary>
        /// List matix Param Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        ///
        [HttpPost]
        public ActionResult GetMatrixParamWeightageDetailsList(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            long totalRecords = 0;
            objBAL = new MasterBAL();
            //objProposalBAL
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objProposalBAL.ListMatrixParametersWeightageDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetMatrixParamWeightageDetailsList()");
                return null;
            }
        }

        /// <summary>
        /// Screen : Add Matrix Master Details for PMGSY-II
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult AddMatrixMasterDetails(string[] MatrixParams)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            decimal parentValue = 0, childValue = 0;
            string prevClass = string.Empty;
            string[] item = null;
            bool flag = true;
            string message = string.Empty;
            string prevGrowthType = string.Empty;
            try
            {
                #region These Validations are to be removed
                /*if (MatrixParams != null)
                {
                    for (int i = 0; i < MatrixParams.Length; i++)
                    {
                        item = MatrixParams[i].Split(',');
                        if (item[1] == "parent")
                        {
                            if (childValue == 0)
                            {
                                prevClass = item[3];
                                parentValue = Convert.ToInt32(item[2]);
                            }
                            else
                            {
                                if (childValue > parentValue)
                                {
                                    flag = false;
                                    return Json(new { success = false, message = "Sum of child items in matrix should be less than or equal to parent item for : " + prevClass });
                                }
                                else
                                {
                                    flag = true;
                                    prevClass = item[3];
                                    childValue = 0;
                                    parentValue = Convert.ToInt32(item[2]);
                                }
                            }
                        }
                        else
                        {
                            childValue = childValue + Convert.ToInt32(item[2]);
                        }
                    }
                }*/
                #endregion

                if (MatrixParams != null)
                {
                    for (int i = 0; i < MatrixParams.Length; i++)
                    {
                        item = MatrixParams[i].Split(',');
                        if (item[1] == "parent")
                        {
                            if (childValue == 0)
                            {
                                prevClass = item[3];
                                parentValue = Convert.ToDecimal(item[2]);
                                prevGrowthType = item[4];
                            }
                            else
                            {
                                if (prevGrowthType == "Cumulative" && (childValue > parentValue))
                                {
                                    flag = false;
                                    //message = growthType == "C" ? "Sum of child items in matrix should be less than or equal to parent item for : " : "Value of child items in matrix should be less than or equal to parent item for : ";
                                    return Json(new { success = false, message = "Sum of child items in matrix should be less than or equal to parent item for : " + prevClass });
                                }
                                else
                                {
                                    flag = true;
                                    prevClass = item[3];
                                    childValue = 0;
                                    parentValue = Convert.ToDecimal(item[2]);
                                    prevGrowthType = item[4];
                                }
                            }
                        }
                        else
                        {
                            childValue = childValue + Convert.ToDecimal(item[2]);

                            if (prevGrowthType == "Highest")
                            {
                                if (Convert.ToDecimal(item[2]) > parentValue)
                                {
                                    return Json(new { success = false, message = "Value of child items in matrix should be less than or equal to parent item for : " + prevClass });
                                }
                            }
                        }
                    }
                }
                if (flag)
                {
                    bool status = objProposalBAL.AddMatrixDetailsBAL(MatrixParams, ref message);
                    if (status == true)
                    {
                        return Json(new { success = true, message = "Matrix Details added successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Matrix Details could not be added" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Invalid Matrix Details" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddMatrixMasterDetails()");
                if (ex.Message == "Input string was not in a correct format.")
                {
                    return Json(new { success = false, message = "Please enter only numbers" });
                }
                else
                {
                    return Json(new { success = false, message = "Error occured while adding Matrix Details" });
                }
            }
        }

        [HttpGet]
        public ActionResult DistrictMappingLayout()
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            PMGSY.DAL.Proposal.ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();
            try
            {
                DistrictMappingModel model = new DistrictMappingModel();
                model.DistrictList = objProposalDAL.PopulateDistrict(PMGSYSession.Current.StateCode);
                model.DistrictList.RemoveAt(0);

                model.isStateEntered = objProposalDAL.checkIsStateEntered();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DistrictMappingLayout()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult MapDistrict(DistrictMappingModel model)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            message = "District mapped successfully.";
            Boolean Status = false;
            try
            {
                Status = objProposalBAL.SaveDistrictMappinDetails(model, out message);
                return Json(new { success = Status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MapDistrict()");
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// List mapped District Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        ///
        [HttpPost]
        public ActionResult ListMappedDistrictDetails(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            long totalRecords = 0;
            objBAL = new MasterBAL();
            //objProposalBAL
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objProposalBAL.ListMappedDistrictDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListMappedDistrictDetails()");
                return null;
            }
        }

        /// <summary>
        /// DeleteMapDistricts() action is used to deleted mapped districts
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteMappedDistricts(String hash, String parameter, String key, string roadCode)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            Dictionary<string, string> decryptedParameters = null;
            string message = string.Empty;
            ProposalBAL objProposal = new ProposalBAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objProposal.DeleteMappedDistrictsBAL(Convert.ToInt32(decryptedParameters["DistrictMappingId"].ToString()), out message))
                    {
                        ModelState.AddModelError(String.Empty, "District could not be deleted.");
                        message = message == String.Empty ? "District could not be deleted" : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "District deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteMappedDistricts()");
                return Json(new { success = false, message = "District could not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region LWE Listing
        [Audit]
        public ActionResult ListProposalLWE()
        {
            ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            List<SelectListItem> lstTypes = new List<SelectListItem>();
            try
            {

                lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
                proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
                // DPIU
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                {
                    proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    //proposalViewModel.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, proposalViewModel.IMS_YEAR, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                }
                else if (PMGSYSession.Current.RoleCode == 15)          //PTA 
                {
                    proposalViewModel.STATES = objCommonFuntion.PopulateStatesOfPTA();
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 3)          //STA 
                {
                    proposalViewModel.STATES = objCommonFuntion.PopulateStatesOfPTA();
                    proposalViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrictsOfTA(PMGSYSession.Current.StateCode, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 37 || PMGSYSession.Current.RoleCode == 55)          //SRRDA or SRRDAOA or SRRDARCPLWE
                {
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                    proposalViewModel.BLOCKS.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 65)  // Mord and Mord View
                {

                    List<SelectListItem> lstRoadTypes = new List<SelectListItem>();
                    lstRoadTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });

                    proposalViewModel.STATES = objCommonFuntion.PopulateStates();
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0, true);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.AGENCIES = lstRoadTypes;
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                }

                proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
                proposalViewModel.Years = PopulateYear(0, true, true);
                proposalViewModel.IMS_YEAR = DateTime.Now.Year;
                //proposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("", true);
                proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;

                return View(proposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.ListProposalLWE");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetMORDRoadProposalsLWE(FormCollection formCollection)
        {
            ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();
            try
            {
                //Adde By Abhishek kamble 30-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 30-Apr-2014 end
                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_STATE_ID = Convert.ToInt32(Request.Params["MAST_STATE_ID"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                int IMS_AGENCY = Convert.ToInt32(Request.Params["IMS_AGENCY"]);

                int totalRecords;
                ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
                var jsonData = new
                {
                    rows = objProposalDAL.GetMordProposalsLWEDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_AGENCY, IMS_UPGRADE_CONNECT, out colTotal),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                    TotalColumn = colTotal
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.GetMORDRoadProposalsLWE()");
                return null;
            }
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
        }

        /// <summary>
        /// Populate List of LSB Proposals for MoRD 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetMORDLSBProposalsLWE(FormCollection formCollection)
        {
            ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_STATE_ID = Convert.ToInt32(Request.Params["MAST_STATE_ID"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                int IMS_AGENCY = Convert.ToInt32(Request.Params["IMS_AGENCY"]);

                int totalRecords;

                ProposalColumnsTotal colModel = new ProposalColumnsTotal();

                var jsonData = new
                {
                    rows = objProposalDAL.GetMordLSBProposalsLWEDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_AGENCY, IMS_UPGRADE_CONNECT, out colModel),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                    TotalColumn = colModel
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.GetMORDLSBProposalsLWE()");
                throw;
            }
        }

        /// <summary>
        /// Get proposals for SRRDA
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetProposalsForSRRDALWE(FormCollection formCollection)
        {
            ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                int MAST_BLOCK_CODE = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
                int totalRecords;
                ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
                var jsonData = new
                {
                    rows = objProposalDAL.GetProposalsForSRRDALWEDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, formCollection["filters"], out colTotal),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                    TotalColumn = colTotal
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.GetProposalsForSRRDALWE");
                return null;
            }
        }

        /// <summary>
        /// Get proposals for SRRDA
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetLSBProposalsForSRRDALWE(FormCollection formCollection)
        {
            ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                int MAST_BLOCK_CODE = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
                int totalRecords;
                ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
                var jsonData = new
                {
                    //rows = objProposalDAL.GetLSBProposalsForSRRDALWEDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, formCollection["filters"], out colTotal),

                    rows = objProposalDAL.GetLSBProposalsForSRRDALWEDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, out colTotal),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                    TotalColumn = colTotal
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.GetProposalsForSRRDALWE");
                return null;
            }
        }

        /// <summary>
        /// deletes the DPR Proposal
        /// </summary>
        /// <param name="proposalCode"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLWEProposal(String parameter, String hash, String key)
        {
            ProposalDAL propDAL = new DAL.Proposal.ProposalDAL();
            int proposalCode = 0;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        proposalCode = Convert.ToInt32(urlSplitParams[0]);
                    }
                }

                if (proposalCode <= 0)
                {
                    return Json(new { success = false, message = "Please select Proposal" });
                }
                bool status = propDAL.DeleteLWEDAL(proposalCode);
                if (status == true)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Proposal not deleted" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.DeleteLWEProposal()");
                return null;
            }
        }
        #endregion

        #region PMGSY3
        [Audit]
        [HttpGet]
        public ActionResult ListProposalPMGSY3()
        {
            ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            List<SelectListItem> lstTypes = new List<SelectListItem>();
            try
            {
                lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
                proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
                // DPIU
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                {
                    proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    //proposalViewModel.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, proposalViewModel.IMS_YEAR, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 15)          //PTA 
                {
                    proposalViewModel.STATES = objCommonFuntion.PopulateStatesOfPTA();
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 3)          //STA 
                {
                    proposalViewModel.STATES = objCommonFuntion.PopulateStatesOfPTA();
                    proposalViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrictsOfTA(PMGSYSession.Current.StateCode, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 37 || PMGSYSession.Current.RoleCode == 55)          //SRRDA or SRRDAOA or SRRDARCPLWE
                {
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                    proposalViewModel.BLOCKS.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 65)  // Mord and Mord View
                {

                    List<SelectListItem> lstRoadTypes = new List<SelectListItem>();
                    lstRoadTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });

                    proposalViewModel.STATES = objCommonFuntion.PopulateStates();
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0, true);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.AGENCIES = lstRoadTypes;
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                }

                proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
                proposalViewModel.Years = PopulateYear(0, true, true);
                proposalViewModel.IMS_YEAR = DateTime.Now.Year;

                SelectListItem itm = proposalViewModel.Years.Where(z => z.Value == DateTime.Now.Year.ToString().Trim()).FirstOrDefault();
                if (DateTime.Now.Month <= 3)
                {
                    proposalViewModel.Years.Remove(itm);
                    proposalViewModel.IMS_YEAR = DateTime.Now.Year - 1;
                }
                
                //proposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("", true);
                proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;

                return View("ListProposalPMGSY3", proposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.ListProposalPMGSY3()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult CreateProposalPMGSY3(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            ProposalDAL objProposalDAL = new ProposalDAL();
            PMGSY.Models.Proposal.ProposalViewModelPMGSY3 proposalViewModel = new ProposalViewModelPMGSY3();
            ViewBag.operation = "C";
            try
            {
                #region Default values
                proposalViewModel.StateName = PMGSYSession.Current.StateName;
                proposalViewModel.DistrictName = PMGSYSession.Current.DistrictName;
                proposalViewModel.stateType = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_TYPE).FirstOrDefault();
                //following property added by Vikram for providing the staged details to the districts which are IAP_DISTRICT
                proposalViewModel.DistrictType = (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_IAP_DISTRICT).FirstOrDefault();
                proposalViewModel.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_SHORT_CODE).FirstOrDefault() +
                                                   (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_DISTRICT_ID).FirstOrDefault().ToString("D2");

                proposalViewModel.IMS_UPGRADE_CONNECT = PMGSYSession.Current.PMGSYScheme == 1 ? "N" : "U";  // for Scheme 2 only upgradation is allowed

                proposalViewModel.IMS_EXISTING_PACKAGE = "N";

                proposalViewModel.IMS_IS_STAGED = "C";
                proposalViewModel.IMS_ISBENEFITTED_HABS = "Y";
                proposalViewModel.IMS_ISBENEFITTED_HABS = "Y";
                proposalViewModel.IMS_STATE_SHARE = 0;
                proposalViewModel.IMS_PROPOSED_SURFACE = "S";

                proposalViewModel.IMS_YEAR = DateTime.Now.Year;
                // Amounts 
                proposalViewModel.IMS_SANCTIONED_PAV_AMT = 0;
                proposalViewModel.IMS_SANCTIONED_CD_AMT = 0;
                proposalViewModel.IMS_SANCTIONED_PW_AMT = 0;
                proposalViewModel.IMS_SANCTIONED_OW_AMT = 0;

                proposalViewModel.IMS_SANCTIONED_MAN_AMT1 = 0;
                proposalViewModel.IMS_SANCTIONED_MAN_AMT2 = 0;
                proposalViewModel.IMS_SANCTIONED_MAN_AMT3 = 0;
                proposalViewModel.IMS_SANCTIONED_MAN_AMT4 = 0;
                proposalViewModel.IMS_SANCTIONED_MAN_AMT5 = 0;

                //For PMGSY Scheme-4
                if (PMGSYSession.Current.PMGSYScheme == 4)
                {
                    proposalViewModel.IMS_IS_HIGHER_SPECIFICATION = "N";
                    proposalViewModel.IMS_SANCTIONED_AMOUNT = 0;
                    proposalViewModel.IMS_SHARE_PERCENT = 2;
                    proposalViewModel.IMS_HIGHER_SPECIFICATION_COST = 0;
                    proposalViewModel.IMS_FURNITURE_COST = 0;
                    proposalViewModel.IMS_RENEWAL_COST = 0;
                    proposalViewModel.IMS_SANCTIONED_HS_AMT = 0;
                    proposalViewModel.IMS_SANCTIONED_FC_AMT = 0;
                    proposalViewModel.IMS_SANCTIONED_RENEWAL_AMT = 0;
                }
                #endregion

                if (id != "" && id != null)
                {
                    string[] defaultValues = id.Split('$');
                    if (defaultValues[0] != "" && defaultValues[0] != null)
                    {
                        proposalViewModel.IMS_YEAR = Convert.ToInt32(defaultValues[0]);
                    }

                    if (defaultValues[1] != "" && defaultValues[1] != null)
                    {
                        proposalViewModel.MAST_BLOCK_CODE = Convert.ToInt32(defaultValues[1]);
                    }

                    if (defaultValues[2] != "" && defaultValues[2] != null)
                    {
                        proposalViewModel.IMS_BATCH = Convert.ToInt32(defaultValues[2]);
                    }

                    if (defaultValues[3] != "" && defaultValues[3] != null)
                    {
                        proposalViewModel.IMS_COLLABORATION = Convert.ToInt32(defaultValues[3]);
                    }

                }

                proposalViewModel.Years = PopulateYear();

                CommonFunctions objCommonFuntion = new CommonFunctions();
                proposalViewModel.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, proposalViewModel.IMS_YEAR, false, PMGSYSession.Current.PMGSYScheme == 1 ? true : false);

                ///Changed by SAMMED A. PATIL for RCPLWE
                proposalViewModel.COLLABORATIONS = new List<SelectListItem>();//objCommonFuntion.PopulateFundingAgency();
                proposalViewModel.COLLABORATIONS.Insert(0, new SelectListItem { Text = "Select Funding Agency", Value = "-1", Selected = true });

                proposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("P");
                proposalViewModel.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
                if (proposalViewModel.MAST_BLOCK_CODE != 0)
                {
                    //proposalViewModel.CN_ROADS = PopulateLinkThrough(proposalViewModel.MAST_BLOCK_CODE, ""); //commented by shyam, because default IMS_UPGRADE_CONNECT value will be 'N'

                    //proposalViewModel.CN_ROADS = PopulateLinkThrough(proposalViewModel.MAST_BLOCK_CODE, "N", "P");
                    proposalViewModel.CN_ROADS = objProposalDAL.PopulateLinkThroughListPMGSY3DAL(proposalViewModel.MAST_BLOCK_CODE, proposalViewModel.IMS_BATCH);
                }
                else
                {
                    //proposalViewModel.CN_ROADS = PopulateLinkThrough(0, "");
                    //proposalViewModel.CN_ROADS = PopulateLinkThrough(0, "N", "P");
                    proposalViewModel.CN_ROADS = objProposalDAL.PopulateLinkThroughListPMGSY3DAL(proposalViewModel.MAST_BLOCK_CODE, proposalViewModel.IMS_BATCH);
                }

                proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode); //PMGSYSession.Current.PMGSYScheme == 3 ? objCommonFuntion.PopulateBlocksforRCPLWE(PMGSYSession.Current.DistrictCode) : objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode);
                proposalViewModel.MP_CONSTITUENCY = objCommonFuntion.PopulateMPConstituency(0);
                proposalViewModel.MLA_CONSTITUENCY = objCommonFuntion.PopulateMLAConstituency(0);
                proposalViewModel.CARRIAGED_WIDTH = objCommonFuntion.PopulateCarriageWidth();
                proposalViewModel.HABS_REASON = objCommonFuntion.PopulateReason("H");
                proposalViewModel.EXISTING_SURFACE = objCommonFuntion.PopulateSurfaceType(false);
                proposalViewModel.TRAFFIC_TYPE = objCommonFuntion.PopulateTrafficType();
                proposalViewModel.PROPOSED_SURFACE = objCommonFuntion.PopulateProposedSurface();

                ViewBag.EXISTING_IMS_PACKAGE_ID = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

                //List<SelectListItem> lstYear = new List<SelectListItem>();
                //lstYear = PopulateYear();
                //int year = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Max(m => m.IMS_YEAR);
                //year = year + 1;
                //int count = lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString()));
                //lstYear.RemoveRange(lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString())), lstYear.Count - count);
                //ViewBag.lstYear = lstYear;

                ViewBag.lstYear = proposalViewModel.Years;

                ViewBag.Stage_2_Year = new SelectList(PopulateYear(0, false).AsEnumerable<SelectListItem>(), "Value", "Text", DateTime.Now.Year);
                ViewBag.Stage_2_Package_ID = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

                proposalViewModel.STAGE1_PROPOSAL_ROADS = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();

                #region CODE_ADDED_BY_VIKRAM_FOR_CHANGES_IN_PMGSY_SCHEME_1_COST

                proposalViewModel.IsProposalFinanciallyClosed = false;
                int shareCode = db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault();
                ViewBag.shareCode = shareCode;
                proposalViewModel.IMS_SHARE_PERCENT_2015 = shareCode == 0 ? (byte)3 : (byte)shareCode;

                #endregion

                return View(proposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.CreateProposalPMGSY3()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult GetLinkThroughListPMGSY3()
        {
            try
            {
                int batch = Convert.ToInt32(Request.Params["Batch"]);
                int BlockID = Convert.ToInt32(Request.Params["BlockID"]);
                string ims_upgrade_connect = Request.Params["IMS_UPGRADE_CONNECT"].ToString();
                string proposalType = Request.Params["PROPOSAL_TYPE"].ToString();
                //return Json(PopulateLinkThroughListPMGSY3(BlockID, ims_upgrade_connect, proposalType));
                return Json(PopulateLinkThroughListPMGSY3(BlockID, batch));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.GetLinkThroughListPMGSY3");
                return null;
            }
        }

        //[HttpGet]
        public List<SelectListItem> PopulateLinkThroughListPMGSY3(int blockCode, int batch)
        {
            ProposalDAL objDAL = new ProposalDAL();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                List<SelectListItem> linkThroughListPMGSY3List = objDAL.PopulateLinkThroughListPMGSY3DAL(blockCode, batch);
                //return Json(linkThroughListPMGSY3List, JsonRequestBehavior.AllowGet);
                return linkThroughListPMGSY3List;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.PopulateLinkThroughListRCPLWE()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult CreateProposalPMGSY3(PMGSY.Models.Proposal.ProposalViewModelPMGSY3 ims_sanctioned_projects)
        {
            ProposalDAL objprDAL = new DAL.Proposal.ProposalDAL();
            try
            {
                ViewBag.operation = "C";

                if (PMGSYSession.Current.PMGSYScheme == 4)  //Added by Aditi on 18 August 2020
                {
                    decimal TotalSurfaceLength = Convert.ToDecimal(ims_sanctioned_projects.SURFACE_BRICK_SOLLING + ims_sanctioned_projects.SURFACE_BT + ims_sanctioned_projects.SURFACE_CC + ims_sanctioned_projects.SURFACE_GRAVEL + ims_sanctioned_projects.SURFACE_MOORUM + ims_sanctioned_projects.SURFACE_TRACK + ims_sanctioned_projects.SURFACE_WBM);

                    if (ims_sanctioned_projects.IMS_PAV_LENGTH != TotalSurfaceLength)
                    {
                        return Json(new { Success = false, ErrorMessage = "Sum of all the Existing Surface Lengths should be equal to the Pavement Length" });
                    }
                    if (ims_sanctioned_projects.SURFACE_BRICK_SOLLING >= 0 && ims_sanctioned_projects.SURFACE_BT >= 0 && ims_sanctioned_projects.SURFACE_CC >= 0 && ims_sanctioned_projects.SURFACE_GRAVEL >= 0 && ims_sanctioned_projects.SURFACE_MOORUM >= 0 && ims_sanctioned_projects.SURFACE_TRACK >= 0 && ims_sanctioned_projects.SURFACE_WBM >= 0)
                    {
                        ModelState.Remove("MAST_EXISTING_SURFACE_CODE");
                    }
                }

                // Can be removed
                //ModelState.Remove("EXISTING_CARRIAGEWAY_WIDTH");
                //ModelState.Remove("EXISTING_CARRIAGEWAY_PUC");

                if (ModelState.IsValid)
                {
                    string route = objprDAL.getRoadRoute(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE));
                    if (route == "N" && ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "U" && PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        return Json(new { Success = false, ErrorMessage = "Please select New Connectivity for Missing Link Route" });
                    }
                    //PMGSY3
                    //if (PMGSYSession.Current.PMGSYScheme == 4)
                    //{
                    //    if (ims_sanctioned_projects.ImsRidingQualityLength <= 0)
                    //    {
                    //        return Json(new { Success = false, ErrorMessage = "Please enter Riding Quality Length" });
                    //    }
                    //}
                   // string Status = objProposalBAL.SaveRoadProposalBALPMGSY3(ims_sanctioned_projects);

                    // cHANGES BY Saurabh start here
                    string Status = string.Empty;
                    if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                    {
                        Status = objProposalBAL.SaveRoadProposalBALPMGSY3(ims_sanctioned_projects);
                    }
                    else
                    {
                        Status = "User role is not authorized";
                        return Json(new { Success = false, ErrorMessage = Status });
                    }
                    // cHANGES BY Saurabh End here

                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "CreateProposalPMGSY3(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)");
                throw ex;
            }
        }


        [HttpGet]
        [Audit]
        public ActionResult EditProposalPMGSY3(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            ProposalDAL objProposalDAL = new ProposalDAL();
            bool isIAP = false;
            CommonFunctions objCommonFuntion = new CommonFunctions();
            int IMS_PR_ROAD_CODE = 0;

            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                    }
                }
                ViewBag.operation = "U";

                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);

                if (ims_sanctioned_projects == null)
                {
                    return Json(new { Success = false, ErrorMessage = "Proposal Not Found" });
                }

                PMGSY.Models.Proposal.ProposalViewModelPMGSY3 objProposal = new ProposalViewModelPMGSY3();

                var adapter = (IObjectContextAdapter)db;
                var objectContext = adapter.ObjectContext;
                objectContext.CommandTimeout = 0;


                //if (db.ACC_BILL_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                //{
                //    objProposal.IS_PAYMENT_MADE = "Y";
                //}
                //else
                //{
                //    objProposal.IS_PAYMENT_MADE = "N";
                //}
                objProposal.IS_PAYMENT_MADE = "N";
                // Below 2 Fields are  Added on 18 March 2021
                objProposal.EXISTING_CARRIAGEWAY_WIDTH = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_WIDTH;
                objProposal.EXISTING_CARRIAGEWAY_PUC = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_PUC;

                objProposal.operation = "U";    //For Update operation

                objProposal.StateName = PMGSYSession.Current.StateName;
                objProposal.DistrictName = PMGSYSession.Current.DistrictName;

                objProposal.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
                objProposal.MAST_STATE_CODE = ims_sanctioned_projects.MAST_STATE_CODE;
                objProposal.stateType = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_TYPE).FirstOrDefault();
                objProposal.MAST_DISTRICT_CODE = ims_sanctioned_projects.MAST_DISTRICT_CODE;
                objProposal.MAST_DPIU_CODE = ims_sanctioned_projects.MAST_DPIU_CODE;
                objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;

                objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;
                objProposal.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode);

                objProposal.IMS_COLLABORATION = ims_sanctioned_projects.IMS_COLLABORATION;
                objProposal.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency();

                //isIAP = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == objProposal.MAST_BLOCK_CODE && x.MAST_IAP_BLOCK == "Y").Any();

                // Upgradation Proposal
                if (ims_sanctioned_projects.IMS_UPGRADE_CONNECT.ToUpper() == "U")
                {
                    objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;

                    objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;

                    if (ims_sanctioned_projects.IMS_ISBENEFITTED_HABS == "N")
                    {
                        objProposal.IMS_HABS_REASON = ims_sanctioned_projects.IMS_HABS_REASON;
                    }
                }
                objProposal.HABS_REASON = objCommonFuntion.PopulateReason("H");

                objProposal.IMS_IS_STAGED = ims_sanctioned_projects.IMS_IS_STAGED;

                // For staged Propsal
                if (objProposal.IMS_IS_STAGED == "S")
                {
                    objProposal.IMS_STAGE_PHASE = ims_sanctioned_projects.IMS_STAGE_PHASE;
                    //Stage 1
                    if (objProposal.IMS_STAGE_PHASE == "S1")
                    {
                        objProposal.IMS_STAGE_PHASE = "1";
                        objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                        //objProposal.CN_ROADS = PopulateLinkThrough(ims_sanctioned_projects.MAST_BLOCK_CODE, ims_sanctioned_projects.IMS_UPGRADE_CONNECT);
                        //objProposal.CN_ROADS = PopulateOnlyLinkThrough(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE), ims_sanctioned_projects.MAST_BLOCK_CODE, ims_sanctioned_projects.IMS_UPGRADE_CONNECT);
                        //objProposal.CN_ROADS = PopulateLinkThroughListPMGSY3(objProposal.MAST_BLOCK_CODE, objProposal.IMS_BATCH);

                        objProposal.CN_ROADS = new List<SelectListItem>();
                        objProposal.CN_ROADS.Insert(0, new SelectListItem() { Text = ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME.Trim(), Value = ims_sanctioned_projects.PLAN_CN_ROAD_CODE.ToString() });
                    }
                    else if (objProposal.IMS_STAGE_PHASE == "S2")
                    {
                        objProposal.IMS_STAGE_PHASE = "2";
                        objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;

                        //objProposal.CN_ROADS = PopulateStagedLinkThrough(Convert.ToInt32(ims_sanctioned_projects.IMS_STAGED_YEAR), ims_sanctioned_projects.IMS_BATCH, ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID, "U", (ims_sanctioned_projects.IMS_STAGED_ROAD_ID.HasValue ? ims_sanctioned_projects.IMS_STAGED_ROAD_ID.Value : 0));

                        objProposal.CN_ROADS = new List<SelectListItem>();
                        objProposal.CN_ROADS.Insert(0, new SelectListItem() { Text = ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME.Trim(), Value = ims_sanctioned_projects.PLAN_CN_ROAD_CODE.ToString() });

                    }
                }
                else if (objProposal.IMS_IS_STAGED == "C")
                {
                    objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                    ///Added by SAMMED A. PATIL for RCPLWE
                    if (objProposal.IMS_COLLABORATION == 5)
                    {
                        //objProposal.CN_ROADS = objProposalDAL.PopulateLinkThroughListRCPLWE(objProposal.MAST_BLOCK_CODE);
                        objProposal.CN_ROADS = new List<SelectListItem>();
                        objProposal.CN_ROADS.Insert(0, new SelectListItem() { Text = ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME.Trim(), Value = ims_sanctioned_projects.PLAN_CN_ROAD_CODE.ToString() });
                    }
                    else
                    {
                        //objProposal.CN_ROADS = PopulateOnlyLinkThrough(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE), ims_sanctioned_projects.MAST_BLOCK_CODE, ims_sanctioned_projects.IMS_UPGRADE_CONNECT);

                        //objProposal.CN_ROADS = PopulateLinkThroughListPMGSY3(objProposal.MAST_BLOCK_CODE, objProposal.IMS_BATCH);
                        objProposal.CN_ROADS = new List<SelectListItem>();
                        objProposal.CN_ROADS.Insert(0, new SelectListItem() { Text = ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME.Trim(), Value = ims_sanctioned_projects.PLAN_CN_ROAD_CODE.ToString() });
                    }
                }

                // objProposal.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == ims_sanctioned_projects.MAST_STATE_CODE select c.MAST_STATE_SHORT_CODE).FirstOrDefault();

                //added by shyam for PACKAGE_PREFIX as STATE_SHORT_CODE + MAST_DISTRICT_ID
                objProposal.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_SHORT_CODE).FirstOrDefault() +
                                                   (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_DISTRICT_ID).FirstOrDefault();
                objProposal.IMS_EXISTING_PACKAGE = "E";
                objProposal.EXISTING_IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
                //objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;

                objProposal.EXISTING_PACKAGES = GetStagedPackageID(ims_sanctioned_projects.IMS_YEAR, ims_sanctioned_projects.IMS_BATCH);
                objProposal.EXISTING_PACKAGES.Insert(0, new SelectListItem { Text = "Select Package", Value = "" });
                objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;

                objProposal.IMS_PARTIAL_LEN = ims_sanctioned_projects.IMS_PARTIAL_LEN;

                objProposal.IMS_CC_LENGTH = ims_sanctioned_projects.IMS_CC_LENGTH;
                objProposal.IMS_BT_LENGTH = ims_sanctioned_projects.IMS_BT_LENGTH;

                // Pavement Length Client Side Validation Skipped
                objProposal.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;

                // For Stage Two Directly Get the Length of Road
                if (objProposal.IMS_STAGE_PHASE == "2")
                {
                    if (ims_sanctioned_projects.PLAN_CN_ROAD_CODE != null)
                    {
                        //string IMS_PAV_LENGTH = objProposalBAL.GetRoadDetailsForStageTwoProposalBAL(objProposal.IMS_PR_ROAD_CODE, Convert.ToInt32(objProposal.PLAN_CN_ROAD_CODE));
                        string IMS_PAV_LENGTH = objProposalBAL.GetRoadDetailsForStageTwoProposalBAL(Convert.ToInt32(ims_sanctioned_projects.IMS_STAGED_ROAD_ID), Convert.ToInt32(objProposal.PLAN_CN_ROAD_CODE));
                        if (IMS_PAV_LENGTH != string.Empty)
                        {
                            objProposal.DUP_IMS_PAV_LENGTH = Convert.ToDecimal(IMS_PAV_LENGTH);
                        }
                    }

                    //Added by shyam to take Stage I road length for Stage II Proposal
                    objProposal.IMS_STAGE1_PAV_LENGTH = db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_STAGED_ROAD_ID).Select(c => c.IMS_PAV_LENGTH).FirstOrDefault();
                    objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;
                    objProposal.STAGE1_PROPOSAL_ROADS = new SelectList(db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_STAGED_ROAD_ID), "IMS_PR_ROAD_CODE", "IMS_ROAD_NAME").ToList();
                }
                // Here We Calculate the Remaining Length id 
                else
                {
                    if (ims_sanctioned_projects.PLAN_CN_ROAD_CODE != null)
                    {
                        /// Get the Roads Remaining Length 
                        string IMS_PAV_LENGTH = objProposalBAL.GetRoadDetailsForUpdate(objProposal.IMS_PR_ROAD_CODE, Convert.ToInt32(objProposal.PLAN_CN_ROAD_CODE));
                        /// Assign Remaining Length to Dummy Length For Validation ( Validation :Pavement Length Should not be greator than Available Length)
                        if (IMS_PAV_LENGTH != string.Empty)
                        {
                            objProposal.DUP_IMS_PAV_LENGTH = Convert.ToDecimal(IMS_PAV_LENGTH);
                        }
                    }

                    objProposal.STAGE1_PROPOSAL_ROADS = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();
                }



                objProposal.IMS_NO_OF_CDWORKS = ims_sanctioned_projects.IMS_NO_OF_CDWORKS;

                objProposal.IMS_ZP_RESO_OBTAINED = ims_sanctioned_projects.IMS_ZP_RESO_OBTAINED;
                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS == null ? ims_sanctioned_projects.IMS_REMARKS : ims_sanctioned_projects.IMS_REMARKS.Trim();

                objProposal.IMS_STATE_SHARE = ims_sanctioned_projects.IMS_STATE_SHARE;
                objProposal.IMS_SANCTIONED_PAV_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;
                objProposal.IMS_SANCTIONED_CD_AMT = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;
                objProposal.IMS_SANCTIONED_PW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;
                objProposal.IMS_SANCTIONED_OW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;

                //For PMGSY Scheme-2
                if (PMGSYSession.Current.PMGSYScheme == 4)
                {
                    objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
                    objProposal.TotalCost = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT + ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT +
                                                        ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT + ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT +
                                                        Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT);
                    objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                    objProposal.IMS_FURNITURE_COST = ims_sanctioned_projects.IMS_FURNITURE_COST;
                    objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST;
                    objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT;
                    objProposal.IMS_SANCTIONED_FC_AMT = ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT;
                    objProposal.IMS_SANCTIONED_RENEWAL_AMT = ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT;

                    if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 1)
                    {
                        objProposal.IMS_SANCTIONED_AMOUNT = (objProposal.TotalCost * 90) / 100;
                        //objProposal.IMS_STATE_SHARE = (objProposal.TotalCost * 10) / 100;
                    }
                    else if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 2)
                    {
                        objProposal.IMS_SANCTIONED_AMOUNT = (objProposal.TotalCost * 75) / 100;
                        //objProposal.IMS_STATE_SHARE = (objProposal.TotalCost * 25) / 100;
                    }
                }


                objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                if (PMGSYSession.Current.PMGSYScheme == 3)
                {
                    objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST ?? 0;
                }

                objProposal.TotalMaintenanceCost = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2 +
                                                   ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4 +
                                                   ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5 +    //In case of PMGSY Scheme-II include IMS_SANCTIONED_RENEWAL_AMT
                                                   (PMGSYSession.Current.PMGSYScheme == 4
                                                    ? Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT)
                                                    : 0.0M);

                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.Years = PopulateYear();//PopulateYear(ims_sanctioned_projects.IMS_YEAR);

                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;

                // objProposal.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, objProposal.IMS_YEAR, false, PMGSYSession.Current.PMGSYScheme == 1 ? true : false);

                objProposal.BATCHS = new List<SelectListItem>();
                objProposal.BATCHS.Insert(0, new SelectListItem() { Text = "Batch " + Convert.ToString(ims_sanctioned_projects.IMS_BATCH), Value = Convert.ToString(ims_sanctioned_projects.IMS_BATCH) });

                objProposal.isPaymentDone = checkIsPayment(ims_sanctioned_projects.IMS_PR_ROAD_CODE);

                if (PMGSYSession.Current.StateCode == 17)
                { // Karnataka State. All Proposal to be edited in case of Payment Is Made. // Suggested by Pankaj Sir on 08 Jan 2021
                    objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                    objProposal.STREAMS = objCommonFuntion.PopulateStreams("P");
                    objProposal.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
                }

                if (!objProposal.isPaymentDone)
                {
                    objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                    objProposal.STREAMS = objCommonFuntion.PopulateStreams("P");
                    objProposal.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
                }



                objProposal.MAST_MP_CONST_CODE = ims_sanctioned_projects.MAST_MP_CONST_CODE;
                objProposal.MP_CONSTITUENCY = objCommonFuntion.PopulateMPConstituency(ims_sanctioned_projects.MAST_BLOCK_CODE);

                objProposal.MAST_MLA_CONST_CODE = ims_sanctioned_projects.MAST_MLA_CONST_CODE;
                objProposal.MLA_CONSTITUENCY = objCommonFuntion.PopulateMLAConstituency(ims_sanctioned_projects.MAST_BLOCK_CODE);

                objProposal.IMS_CARRIAGED_WIDTH = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH;
                objProposal.CARRIAGED_WIDTH = objCommonFuntion.PopulateCarriageWidth();

                objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;
                objProposal.EXISTING_SURFACE = objCommonFuntion.PopulateSurfaceType();

                objProposal.IMS_TRAFFIC_TYPE = ims_sanctioned_projects.IMS_TRAFFIC_TYPE;
                objProposal.TRAFFIC_TYPE = objCommonFuntion.PopulateTrafficType();

                objProposal.IMS_PROPOSED_SURFACE = ims_sanctioned_projects.IMS_PROPOSED_SURFACE;
                objProposal.PROPOSED_SURFACE = objCommonFuntion.PopulateProposedSurface();

                objProposal.Stage_2_Year = ims_sanctioned_projects.IMS_STAGED_YEAR;
                objProposal.Stage_2_Package_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;
                objProposal.PACKAGES = GetStagedPackageID(Convert.ToInt32(ims_sanctioned_projects.IMS_STAGED_YEAR), 0);
                //ViewBag.Stage_2_Package_ID = new SelectList(GetStagedPackageID(ims_sanctioned_projects.IMS_YEAR, 0), "Value", "Text");
                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_MORD_SHARE_2015.Value : 0;
                objProposal.IMS_TOTAL_COST_2015 = objProposal.IMS_STATE_SHARE_2015 + objProposal.IMS_MORD_SHARE_2015;
                //objProposal.IMS_SHARE_PERCENT_2015 = (byte)(db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault() == 0 ? 3 : db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault());
                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null ? ((byte)(db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault() == 0 ? 3 : db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault())) : ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                //if (!objProposalDAL.IsProposalFinanciallyClosed(ims_sanctioned_projects.IMS_PR_ROAD_CODE) && ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null)
                //{
                //    objProposal.IMS_SHARE_PERCENT_2015 = 4;
                //}
                //else
                //{

                //}

                List<SelectListItem> lstYear = new List<SelectListItem>();
                lstYear = PopulateYear();
                int year = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Max(m => m.IMS_YEAR);
                if (year != DateTime.Now.Year)
                {
                    year = year + 1;
                }
                int count = lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString()));
                lstYear.RemoveRange(lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString())), lstYear.Count - count);
                ViewBag.lstYear = lstYear;

                ViewBag.shareCode = objProposal.IMS_SHARE_PERCENT_2015;

                ///PMGSY3
                objProposal.ImsRidingQualityLength = ims_sanctioned_projects.IMS_RIDING_QUALITY_LENGTH.Value;
                objProposal.ImsPuccaSideDrains = ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS.Value;
                objProposal.ImsGSTCost = ims_sanctioned_projects.IMS_GST_COST.Value;

                IMS_COST_COMPONENT imsCostComponent = ims_sanctioned_projects.IMS_COST_COMPONENT.Where(z => z.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_PR_ROAD_CODE).FirstOrDefault();

                if (imsCostComponent != null)
                {
                    objProposal.imsComponentId = imsCostComponent.IMS_COMPONENT_ID;
                    objProposal.ImsClearing = imsCostComponent.IMS_CLEARING;
                    objProposal.ImsExcavation = imsCostComponent.IMS_EXCAVATION;
                    objProposal.ImsFilling = imsCostComponent.IMS_FILLING;
                    objProposal.ImsSubGrade = imsCostComponent.IMS_SUB_GRADE;
                    objProposal.ImsShoulder = imsCostComponent.IMS_SHOULDER;
                    objProposal.ImsGranularSubBase = imsCostComponent.IMS_GRANULAR_SUB_BASE;
                    objProposal.ImsSoilAggregate = imsCostComponent.IMS_SOIL_AGGREGATE;
                    objProposal.ImsWBMGradeII = imsCostComponent.IMS_WBM_GRADE_II;
                    objProposal.ImsWBMGradeIII = imsCostComponent.IMS_WBM_GRADE_III;
                    objProposal.ImsWMM = imsCostComponent.IMS_WMM;
                    objProposal.ImsPrimeCoat = imsCostComponent.IMS_PRIME_COAT;
                    objProposal.ImsTackCoat = imsCostComponent.IMS_TACK_COAT;
                    objProposal.ImsBMDBM = imsCostComponent.IMS_BM_DBM;
                    objProposal.ImsOGPC_SDBC_BC = imsCostComponent.IMS_OGPC_SDBC_BC;
                    objProposal.ImsSealCoat = imsCostComponent.IMS_SEAL_COAT;
                    objProposal.ImsSurfaceDressing = imsCostComponent.IMS_SURFACE_DRESSING;
                    objProposal.ImsDryLeanConcrete = imsCostComponent.IMS_DRY_LEAN_CONCRETE;
                    objProposal.ImsConcretePavement = imsCostComponent.IMS_CONCRETE_PAVEMENT;
                }
                //Added by Aditi on 6 August 2020
                if (PMGSYSession.Current.PMGSYScheme == 4)
                {
                    #region For Post DPL Maintenance Cost
                    objProposal.PUCCA_SIDE_DRAIN_LENGTH = ims_sanctioned_projects.PUCCA_SIDE_DRAIN_LENGTH;
                    objProposal.PROTECTION_LENGTH = ims_sanctioned_projects.PROTECTION_LENGTH;
                    objProposal.IMS_MAINTENANCE_YEAR6 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR6);
                    objProposal.IMS_MAINTENANCE_YEAR7 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR7);
                    objProposal.IMS_MAINTENANCE_YEAR8 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR8);
                    objProposal.IMS_MAINTENANCE_YEAR9 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR9);
                    objProposal.IMS_MAINTENANCE_YEAR10 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR10);

                    objProposal.TotalPostDLPMaintenanceCost = objProposal.IMS_MAINTENANCE_YEAR6 + objProposal.IMS_MAINTENANCE_YEAR7 + objProposal.IMS_MAINTENANCE_YEAR8 + objProposal.IMS_MAINTENANCE_YEAR9 + objProposal.IMS_MAINTENANCE_YEAR10;
                    #endregion

                    #region Existing Surface Details
                    objProposal.SURFACE_BRICK_SOLLING = ims_sanctioned_projects.SURFACE_BRICK_SOLLING;
                    objProposal.SURFACE_BT = ims_sanctioned_projects.SURFACE_BT;
                    objProposal.SURFACE_CC = ims_sanctioned_projects.SURFACE_CC;
                    objProposal.SURFACE_GRAVEL = ims_sanctioned_projects.SURFACE_GRAVEL;
                    objProposal.SURFACE_MOORUM = ims_sanctioned_projects.SURFACE_MOORUM;
                    objProposal.SURFACE_TRACK = ims_sanctioned_projects.SURFACE_TRACK;
                    objProposal.SURFACE_WBM = ims_sanctioned_projects.SURFACE_WBM;
                    #endregion
                }

                return View("CreateProposalPMGSY3", objProposal);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditProposalPMGSY3()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult EditProposalPMGSY3(PMGSY.Models.Proposal.ProposalViewModelPMGSY3 ims_sanctioned_projects)
        {
            ProposalDAL objprDAL = new DAL.Proposal.ProposalDAL();
            try
            {
                // Can be removed
                //ModelState.Remove("EXISTING_CARRIAGEWAY_WIDTH");
                //ModelState.Remove("EXISTING_CARRIAGEWAY_PUC");

                ViewBag.Operation = "E";
                if (ims_sanctioned_projects.isPaymentDone)
                {
                    ModelState.Remove("IMS_STREAMS");
                }
                if (PMGSYSession.Current.PMGSYScheme == 4)  //Added by Aditi on 11 August 2020
                {
                    decimal TotalSurfaceLength = Convert.ToDecimal(ims_sanctioned_projects.SURFACE_BRICK_SOLLING + ims_sanctioned_projects.SURFACE_BT + ims_sanctioned_projects.SURFACE_CC + ims_sanctioned_projects.SURFACE_GRAVEL + ims_sanctioned_projects.SURFACE_MOORUM + ims_sanctioned_projects.SURFACE_TRACK + ims_sanctioned_projects.SURFACE_WBM);

                    if (ims_sanctioned_projects.IMS_PAV_LENGTH != TotalSurfaceLength)
                    {
                        return Json(new { Success = false, ErrorMessage = "Sum of all the Existing Surface Lengths should be equal to the Pavement Length" });
                    }
                    if (ims_sanctioned_projects.SURFACE_BRICK_SOLLING >= 0 && ims_sanctioned_projects.SURFACE_BT >= 0 && ims_sanctioned_projects.SURFACE_CC >= 0 && ims_sanctioned_projects.SURFACE_GRAVEL >= 0 && ims_sanctioned_projects.SURFACE_MOORUM >= 0 && ims_sanctioned_projects.SURFACE_TRACK >= 0 && ims_sanctioned_projects.SURFACE_WBM >= 0)
                    {
                        ModelState.Remove("MAST_EXISTING_SURFACE_CODE");
                    }
                }

                if (ModelState.IsValid)
                {
                    string route = objprDAL.getRoadRoute(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE));
                    if (route == "N" && ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "U" && PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        return Json(new { Success = false, ErrorMessage = "Please select New Connectivity for Missing Link Route" });
                    }
                   // string Status = objprDAL.UpdateRoadProposalDALPMGSY3(ims_sanctioned_projects);

                    // Changes Start here by saurabh
                    string Status = string.Empty;
                    if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                    {
                        Status = objprDAL.UpdateRoadProposalDALPMGSY3(ims_sanctioned_projects);
                    }
                    else
                    {
                        Status = "User Role Un-authorized.";
                        return Json(new { Success = false, ErrorMessage = Status });
                    }
                    // Changes End here by saurabh

                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditProposalPMGSY3(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)");
                throw ex;
            }
        }
      

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public JsonResult DeletePMGSY3Proposal(String parameter, String hash, String key)
        {
            int IMS_PR_ROAD_CODE = 0;
            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                }
            }
            else
            {
                return Json(new { success = false, errorMessage = "Invalid Road Code." });
            }

            string status = string.Empty;
            try
            {
                status = objProposalBAL.DeleteRoadProposalPMGSY3BAL(IMS_PR_ROAD_CODE);
                if (status == string.Empty)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteConfirmed()");
                return Json(new { success = false, errorMessage = status });
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult DetailsPMGSY3Proposal(string id)
        {
            try
            {
                String[] urlSplitParams = id.Split('$');
                Int32 IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);

                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);


                // Lock Status is depend on Is the proposal mord sanctioned or not?
                // In Case of Mord Unlocked Status IMS_LOCK_STATUS = "M"
                // Else IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS
                // Here if it is splitted from parameter - id, that means it is passed from GetProposalsDAL() function
                // Else get it from logic in db function -- UDF_IMS_UNLOCK_STATUS
                string IMS_LOCK_STATUS = string.Empty;

                if (urlSplitParams.Length > 1)
                {
                    IMS_LOCK_STATUS = urlSplitParams[1];
                }
                else
                {
                    if (ims_sanctioned_projects.IMS_SANCTIONED == "Y")
                    {
                        if (db.UDF_IMS_UNLOCK_STATUS(ims_sanctioned_projects.MAST_STATE_CODE, ims_sanctioned_projects.MAST_DISTRICT_CODE, ims_sanctioned_projects.MAST_BLOCK_CODE, 0, 0, ims_sanctioned_projects.IMS_PR_ROAD_CODE, 0, 0, "PR", ims_sanctioned_projects.MAST_PMGSY_SCHEME, (short)PMGSYSession.Current.RoleCode).Select(c => c.UNLOCK_COUNT).FirstOrDefault() > 0)
                        {
                            IMS_LOCK_STATUS = "M";
                        }
                        else
                        {
                            IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS;
                        }
                    }
                    else
                    {
                        IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS;
                    }
                }



                PMGSY.Models.Proposal.ProposalViewModelPMGSY3 objProposal = new ProposalViewModelPMGSY3();
                if (ims_sanctioned_projects == null)
                {
                    return HttpNotFound();
                }

                //objProposal.EXISTING_CARRIAGEWAY_WIDTH = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_WIDTH==null?"-":Convert.ToString(ims_sanctioned_projects.EXISTING_CARRIAGEWAY_WIDTH);
                //objProposal.EXISTING_CARRIAGEWAY_PUC = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_PUC == null ? "-" : Convert.ToString(ims_sanctioned_projects.EXISTING_CARRIAGEWAY_PUC);

                objProposal.EXISTING_CARRIAGEWAY_WIDTH = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_WIDTH;
                objProposal.EXISTING_CARRIAGEWAY_PUC = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_PUC;


                objProposal.StateName = db.MASTER_STATE.Where(a => a.MAST_STATE_CODE == ims_sanctioned_projects.MAST_STATE_CODE).Select(a => a.MAST_STATE_NAME).First();
                objProposal.DistrictName = db.MASTER_DISTRICT.Where(a => a.MAST_DISTRICT_CODE == ims_sanctioned_projects.MAST_DISTRICT_CODE).Select(a => a.MAST_DISTRICT_NAME).First();
                ViewBag.IsTechnologyExist = db.IMS_PROPOSAL_TECH.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE) ? "Y" : "N";
                objProposal.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
                objProposal.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objProposal.MAST_BLOCK_NAME = ims_sanctioned_projects.MASTER_BLOCK.MAST_BLOCK_NAME;
                objProposal.MAST_STREAM_NAME = ims_sanctioned_projects.MASTER_STREAMS.MAST_STREAM_NAME;
                objProposal.IMS_STAGE_PHASE = ims_sanctioned_projects.IMS_STAGE_PHASE;
                objProposal.IMS_STAGED_YEAR = ims_sanctioned_projects.IMS_STAGED_YEAR;
                objProposal.IMS_STAGED_PACKAGE_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;

                if (ims_sanctioned_projects.PLAN_CN_ROAD_CODE != null)
                {
                    //objProposal.PLAN_RD_NAME = ims_sanctioned_projects.PLAN_ROAD.PLAN_CN_ROAD_NUMBER + "-" + ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME;
                    ///Changed by SAMMED A. PATIL for RCPLWE
                    objProposal.PLAN_RD_NAME = ims_sanctioned_projects.PLAN_ROAD.PLAN_CN_ROAD_NUMBER == "O" ? ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME : (ims_sanctioned_projects.PLAN_ROAD.PLAN_CN_ROAD_NUMBER + "-" + ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME);
                }
                else
                {
                    objProposal.PLAN_RD_NAME = "--";
                }

                objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;
                if (ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "U")
                {
                    objProposal.MAST_EXISTING_SURFACE_NAME = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE == null ? "" : ims_sanctioned_projects.MASTER_SURFACE.MAST_SURFACE_NAME;
                    objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;
                    if (ims_sanctioned_projects.IMS_ISBENEFITTED_HABS == "N")
                    {
                        objProposal.HABS_REASON_TEXT = db.MASTER_REASON.Where(a => a.MAST_REASON_CODE == ims_sanctioned_projects.IMS_HABS_REASON).Select(a => a.MAST_REASON_NAME).FirstOrDefault();
                    }
                }
                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;
                objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
                objProposal.MAST_FUNDING_AGENCY_NAME = ims_sanctioned_projects.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_NAME;
                objProposal.MAST_STREAM_NAME = ims_sanctioned_projects.MASTER_STREAMS.MAST_STREAM_NAME;

                objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                objProposal.IMS_PARTIAL_LEN = ims_sanctioned_projects.IMS_PARTIAL_LEN.ToUpper() == "F" ? "Full Length" : "Partial Length";
                objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;
                objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                objProposal.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;
                objProposal.IMS_NO_OF_CDWORKS = ims_sanctioned_projects.IMS_NO_OF_CDWORKS;

                // All Costs
                objProposal.IMS_SANCTIONED_PAV_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;
                objProposal.IMS_SANCTIONED_CD_AMT = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;
                objProposal.IMS_SANCTIONED_PW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;
                objProposal.IMS_SANCTIONED_OW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;
                objProposal.IMS_SANCTIONED_RS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT;

                //objProposal.IMS_TOTAL_COST = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT + ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT +
                //                             ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT + ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT +
                //                             ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT +
                //                             Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT) + "";

                objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
                //objProposal.IMS_TOTAL_COST = (ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT + ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT +
                //                              ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT + ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT +
                //                              Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT)).ToString();
                ///Changes made by SAMMED A. PATIL on 14AUG2017
                objProposal.IMS_TOTAL_COST = Convert.ToString((PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ?
                                                          ((ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT))
                                                        : ((ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) + (ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS == null ? 0 : ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS)
                                               //+ (ims_sanctioned_projects.IMS_GST_COST == null ? 0 : ims_sanctioned_projects.IMS_GST_COST) 
                                                        ));
                objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;
                objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                objProposal.IMS_FURNITURE_COST = ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT;
                objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT;
                objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT;
                objProposal.IMS_SANCTIONED_FC_AMT = ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT;
                objProposal.IMS_SANCTIONED_RENEWAL_AMT = ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT ?? 0;

                if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 1)
                {
                    objProposal.IMS_SANCTIONED_AMOUNT = (Convert.ToDecimal(objProposal.IMS_TOTAL_COST) * 90) / 100;
                }
                else if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 2)
                {
                    objProposal.IMS_SANCTIONED_AMOUNT = (Convert.ToDecimal(objProposal.IMS_TOTAL_COST) * 75) / 100;
                }

                objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;


                objProposal.TotalMaintenanceCost = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2 +
                                                   ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4 +
                                                   ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5 +  //In case of PMGSY Scheme-II include IMS_SANCTIONED_RENEWAL_AMT
                                                   (PMGSYSession.Current.PMGSYScheme == 4
                                                    ? Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT)
                                                    : 0.0M);

                objProposal.IMS_ZP_RESO_OBTAINED = ims_sanctioned_projects.IMS_ZP_RESO_OBTAINED;
                objProposal.MAST_MP_CONST_NAME = ims_sanctioned_projects.MASTER_MP_CONSTITUENCY == null ? "NA" : ims_sanctioned_projects.MASTER_MP_CONSTITUENCY.MAST_MP_CONST_NAME;
                objProposal.MAST_MLA_CONST_NAME = ims_sanctioned_projects.MASTER_MLA_CONSTITUENCY == null ? "NA" : ims_sanctioned_projects.MASTER_MLA_CONSTITUENCY.MAST_MLA_CONST_NAME;
                objProposal.IMS_CARRIAGED_WIDTH = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH;
                objProposal.Display_Carriaged_Width = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH != null ? db.MASTER_CARRIAGE.Where(a => a.MAST_CARRIAGE_CODE == ims_sanctioned_projects.IMS_CARRIAGED_WIDTH).Select(a => a.MAST_CARRIAGE_WIDTH).First().ToString() : "NA";

                if (ims_sanctioned_projects.MASTER_TRAFFIC_TYPE != null)
                {
                    objProposal.IMS_TRAFFIC_CATAGORY_NAME = ims_sanctioned_projects.MASTER_TRAFFIC_TYPE.MAST_TRAFFIC_NAME;
                }
                else
                {
                    objProposal.IMS_TRAFFIC_CATAGORY_NAME = "NA";
                }

                objProposal.IMS_PROPOSED_SURFACE = ims_sanctioned_projects.IMS_PROPOSED_SURFACE.ToUpper() == "S" ? "Sealed" : "UnSealed";
                objProposal.IMS_CC_LENGTH = ims_sanctioned_projects.IMS_CC_LENGTH;
                objProposal.IMS_BT_LENGTH = ims_sanctioned_projects.IMS_BT_LENGTH;
                objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;

                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS;
                objProposal.IMS_REASON = ims_sanctioned_projects.IMS_REASON;

                if (ims_sanctioned_projects.IMS_REASON != null)
                {
                    objProposal.Reason = db.MASTER_REASON.Where(a => a.MAST_REASON_CODE == ims_sanctioned_projects.IMS_REASON).Select(a => a.MAST_REASON_NAME).First();
                }

                objProposal.IMS_IS_STAGED = ims_sanctioned_projects.IMS_IS_STAGED;
                objProposal.IMS_ISCOMPLETED = ims_sanctioned_projects.IMS_ISCOMPLETED;

                //STA Details
                objProposal.STA_SANCTIONED = ims_sanctioned_projects.STA_SANCTIONED;
                objProposal.STA_SANCTIONED_BY = (ims_sanctioned_projects.STA_SANCTIONED_BY != null && ims_sanctioned_projects.STA_SANCTIONED_BY != "") ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == db.UM_User_Master.Where(a => a.UserName == ims_sanctioned_projects.STA_SANCTIONED_BY).Select(a => a.UserID).FirstOrDefault()).Any() ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == db.UM_User_Master.Where(a => a.UserName == ims_sanctioned_projects.STA_SANCTIONED_BY).Select(a => a.UserID).FirstOrDefault()).Select(b => b.ADMIN_TA_NAME).First() : (ims_sanctioned_projects.STA_SANCTIONED_BY == null ? "NA" : ims_sanctioned_projects.STA_SANCTIONED_BY.ToString()) : (ims_sanctioned_projects.STA_SANCTIONED_BY == null ? "NA" : ims_sanctioned_projects.STA_SANCTIONED_BY.ToString()); //change done by Vikram on 26/05/2014 as if no data found in Admin Technical Agency then show the STA Name as it is from IMS_SANCTIONED_PROJECTS

                if (ims_sanctioned_projects.STA_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.STA_SANCTIONED_DATE).Year != 0)
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_projects.STA_SANCTIONED_DATE);
                    objProposal.STA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                }
                objProposal.MS_STA_REMARKS = ims_sanctioned_projects.IMS_STA_REMARKS;


                //PTA Details
                objProposal.PTA_SANCTIONED = ims_sanctioned_projects.PTA_SANCTIONED;
                objProposal.PTA_SANCTIONED_BY = ims_sanctioned_projects.PTA_SANCTIONED_BY == null ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_CODE).FirstOrDefault() : ims_sanctioned_projects.PTA_SANCTIONED_BY;
                objProposal.NAME_OF_PTA = ims_sanctioned_projects.PTA_SANCTIONED_BY == null
                                                        ? db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_NAME).FirstOrDefault()
                                                        : db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_TA_CODE == ims_sanctioned_projects.PTA_SANCTIONED_BY).Select(a => a.ADMIN_TA_NAME).FirstOrDefault();

                if (ims_sanctioned_projects.PTA_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.PTA_SANCTIONED_DATE).Year != 0)
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_projects.PTA_SANCTIONED_DATE);
                    objProposal.PTA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                }
                objProposal.MS_PTA_REMARKS = ims_sanctioned_projects.IMS_PTA_REMARKS;


                objProposal.IMS_SANCTIONED = ims_sanctioned_projects.IMS_SANCTIONED;
                objProposal.IMS_SANCTIONED_BY = ims_sanctioned_projects.IMS_SANCTIONED_BY;
                if (ims_sanctioned_projects.IMS_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.IMS_SANCTIONED_DATE).Year != 0)
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_projects.IMS_SANCTIONED_DATE);
                    objProposal.IMS_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                }
                objProposal.IMS_PROG_REMARKS = ims_sanctioned_projects.IMS_PROG_REMARKS;


                //objProposal.IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS;

                //In Case of Mord Unlocked Status
                objProposal.IMS_LOCK_STATUS = IMS_LOCK_STATUS;
                objProposal.IMS_DPR_STATUS = ims_sanctioned_projects.IMS_DPR_STATUS; //new change done by Vikram to hide the finalize button if the Proposal is DPR.
                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_MORD_SHARE_2015.Value : 0;
                objProposal.IMS_TOTAL_COST_2015 = objProposal.IMS_STATE_SHARE_2015 + objProposal.IMS_MORD_SHARE_2015;
                //@PMGSY = 1  THEN (ISNULL(sp.IMS_STATE_SHARE,0) + ISNULL(sp.IMS_STATE_SHARE_2015,0) + ISNULL(sp.IMS_SANCTIONED_BS_AMT,0)) WHEN @PMGSY = 2 THEN (ISNULL(IMS_SANCTIONED_RS_AMT,0)+ISNULL(IMS_SANCTIONED_HS_AMT,0)+ ISNULL(sp.IMS_STATE_SHARE_2015,0)+ ISNULL(sp.IMS_SANCTIONED_BS_AMT,0)) ELSE '-' END AS TOTAL_STATE_SHARE,
                objProposal.IMS_TOTAL_STATE_SHARE_2015 = PMGSYSession.Current.PMGSYScheme == 1 ? (decimal)(ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT + (ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0))
                                                                                               : (decimal)(/*ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT +*/ Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE_2015) + ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT);
                //objProposal.IMS_SHARE_PERCENT_2015 = (byte)(db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault() == 0 ? 3 : db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault());
                //objProposal.IMS_SHARE_PERCENT_2015 = 4;

                objProposal.ImsRidingQualityLength = ims_sanctioned_projects.IMS_RIDING_QUALITY_LENGTH.HasValue ? ims_sanctioned_projects.IMS_RIDING_QUALITY_LENGTH.Value : 0;
                objProposal.ImsPuccaSideDrains = ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS.HasValue ? ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS.Value : 0;
                objProposal.ImsGSTCost = ims_sanctioned_projects.IMS_GST_COST.HasValue ? ims_sanctioned_projects.IMS_GST_COST.Value : 0;

                IMS_COST_COMPONENT imsCostComponent = ims_sanctioned_projects.IMS_COST_COMPONENT.Where(z => z.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_PR_ROAD_CODE).FirstOrDefault();

                if (imsCostComponent != null)
                {
                    objProposal.imsComponentId = imsCostComponent.IMS_COMPONENT_ID;
                    objProposal.ImsClearing = imsCostComponent.IMS_CLEARING;
                    objProposal.ImsExcavation = imsCostComponent.IMS_EXCAVATION;
                    objProposal.ImsFilling = imsCostComponent.IMS_FILLING;
                    objProposal.ImsSubGrade = imsCostComponent.IMS_SUB_GRADE;
                    objProposal.ImsShoulder = imsCostComponent.IMS_SHOULDER;
                    objProposal.ImsGranularSubBase = imsCostComponent.IMS_GRANULAR_SUB_BASE;
                    objProposal.ImsSoilAggregate = imsCostComponent.IMS_SOIL_AGGREGATE;
                    objProposal.ImsWBMGradeII = imsCostComponent.IMS_WBM_GRADE_II;
                    objProposal.ImsWBMGradeIII = imsCostComponent.IMS_WBM_GRADE_III;
                    objProposal.ImsWMM = imsCostComponent.IMS_WMM;
                    objProposal.ImsPrimeCoat = imsCostComponent.IMS_PRIME_COAT;
                    objProposal.ImsTackCoat = imsCostComponent.IMS_TACK_COAT;
                    objProposal.ImsBMDBM = imsCostComponent.IMS_BM_DBM;
                    objProposal.ImsOGPC_SDBC_BC = imsCostComponent.IMS_OGPC_SDBC_BC;
                    objProposal.ImsSealCoat = imsCostComponent.IMS_SEAL_COAT;
                    objProposal.ImsSurfaceDressing = imsCostComponent.IMS_SURFACE_DRESSING;
                    objProposal.ImsDryLeanConcrete = imsCostComponent.IMS_DRY_LEAN_CONCRETE;
                    objProposal.ImsConcretePavement = imsCostComponent.IMS_CONCRETE_PAVEMENT;

                }

                objProposal.encrProposalCode = URLEncrypt.EncryptParameters(new string[] { "IMS_PR_ROAD_CODE =" + objProposal.IMS_PR_ROAD_CODE.ToString() });
                return View(objProposal);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DetailsPMGSY3Proposal");
                return null;
            }
        }


        #region added by abhinav
        [HttpGet]
        public ActionResult PdfFileUploadView(string parameter, string hash, string key)
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();

                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int IMS_ROAD_CODE = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"].ToString());


                PDFFileUploadViewModel fileUploadViewModel = new PDFFileUploadViewModel();
                //fileUploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(IMS_ROAD_CODE);

                fileUploadViewModel.Enc_IMS_PR_ROAD_CODE = parameter + "/" + hash + "/" + key;

                fileUploadViewModel.ErrorMessage = string.Empty;

                //To check if file already uploaded.
                if (dbcontext.IMS_SANCTION_FOREST_CLEARANCE.Where(x => x.IMS_PR_ROAD_CODE == IMS_ROAD_CODE).Any())
                {
                    fileUploadViewModel.NumberofPdfs = 1;
                }

                else
                {
                    fileUploadViewModel.NumberofPdfs = 0;
                }

                return View(fileUploadViewModel);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult SavePDFFile(PDFSaveFormModel formmodel)
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                CommonFunctions objCommonFunc = new CommonFunctions();

                String parameter = formmodel.Enc_IMS_PR_ROAD_CODE.Split('/')[0];
                String hash = formmodel.Enc_IMS_PR_ROAD_CODE.Split('/')[1];
                String key = formmodel.Enc_IMS_PR_ROAD_CODE.Split('/')[2];

                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"].ToString());

                foreach (string file in Request.Files)
                {
                    string status = ValidatePhoto(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        formmodel.ErrorMessage = status;
                        return Json(new { message = "Photograph format is not valid", success = false });
                    }
                }
                bool isFileSaved = false;
                HttpPostedFileBase FileBase = null;

                ProposalDAL objDAL = new DAL.Proposal.ProposalDAL();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    FileBase = Request.Files[i];
                    var filename = FileBase.FileName;

                    //string remark = Request.Params["InspPdfDescription[]"];
                    //Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");
                    //if (!regex.IsMatch(remark))
                    //{
                    //    return Json(new { success = isFileSaved, message = "Only alphabets and numbers allowed in remark" }, JsonRequestBehavior.DenyGet);
                    //}

                    if (dbcontext.IMS_SANCTION_FOREST_CLEARANCE.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                        return Json(new { success = isFileSaved, message = "File already uploaded." }, JsonRequestBehavior.DenyGet);

                    isFileSaved = objDAL.SavePDFFileDAL(IMS_PR_ROAD_CODE, filename, FileBase/*, Request.Params["InspPdfDescription[]"]*/);
                }

                formmodel.NumberofImages = 0;

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                if (isFileSaved)
                    return Json(new { success = isFileSaved, message = "File uploaded successfully" }, JsonRequestBehavior.DenyGet);
                else
                    return Json(new { success = isFileSaved, message = "File not uploaded" }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal/SavePDFFile");
                return null;
            }
        }

        public string ValidatePhoto(int FileSize, string FileExtension)
        {
            try
            {
                if (!(FileExtension.ToUpper().Equals(".PDF") || FileExtension.ToUpper().Equals(".pdf")))
                {
                    return "File not in correct format";
                }
                if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_MAX_SIZE"]))
                {
                    return "File Size Exceed the Maximum File size Limit";
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/ValidatePhoto");
                return null;
            }
        }

        [HttpPost]
        public JsonResult PMGSYIIIListPDFFiles(FormCollection formCollection)
        {
            //using (CommonFunctions commonFunction = new CommonFunctions())
            //{
            //    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
            //    {
            //        return null;
            //    }
            //}

            string ENCR_IMS_PR_ROAD_CODE = Request.Params["IMS_PR_ROAD_CODE"];

            String parameter = ENCR_IMS_PR_ROAD_CODE.Split('/')[0];
            String hash = ENCR_IMS_PR_ROAD_CODE.Split('/')[1];
            String key = ENCR_IMS_PR_ROAD_CODE.Split('/')[2];

            Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            int IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"].ToString());

            int totalRecords;
            ProposalDAL objDAL = new DAL.Proposal.ProposalDAL();

            var jsonData = new
            {
                rows = objDAL.PMGSYIIIGetPDFFilesListDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        [HttpGet]
        public ActionResult DownloadPDFFilePMGSY3(String parameter, String hash, String key)
        {
            try
            {
                string FileName = string.Empty;
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

                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                FullFileLogicalPath = ConfigurationManager.AppSettings["PROPOSAL_PMGSY3_FILE_UPLOAD_FILE_UPLOAD_VIRTUAL_DIR_PATH"];
                FullfilePhysicalPath = ConfigurationManager.AppSettings["PROPOSAL_PMGSY3_FILE_UPLOAD"];


                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(Path.Combine(FullfilePhysicalPath, FileName)))
                {
                    return File(Path.Combine(FullfilePhysicalPath, FileName), "Application/pdf", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DownloadPDFFilePMGSY3()");
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeletePDFFileDetailsPMGSY3(string id)
        {
            string[] arrParam = null;
            try
            {
                String PhysicalPath = string.Empty;
                String ThumbnailPath = string.Empty;

                PhysicalPath = ConfigurationManager.AppSettings["PROPOSAL_PMGSY3_FILE_UPLOAD"];

                int IMS_FILE_ID = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"].Split('$')[0]);
                int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"].Split('$')[1]);
                string filename = Request.Params["IMS_PR_ROAD_CODE"].Split('$')[2];

                PhysicalPath = Path.Combine(PhysicalPath, "");
                ProposalDAL objDAL = new DAL.Proposal.ProposalDAL();
                string status = objDAL.DeletePDFFileDetailsDAL(IMS_FILE_ID, IMS_PR_ROAD_CODE, filename);

                if (status == string.Empty)
                {
                    try
                    {
                        if (System.IO.File.Exists(Path.Combine(PhysicalPath, filename)))
                            System.IO.File.Delete(Path.Combine(PhysicalPath, filename));
                        return Json(new { Success = true, ErrorMessage = "File deleted successfully." });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteFileDetails()");
                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
        }

        #endregion

        #endregion

        #region Proposal Shifting
        /// <summary>
        /// Get Filter and List of Proposals to be shifted
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult ListProposalForShifting()
        {
            try
            {
                ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
                CommonFunctions objCommonFuntion = new CommonFunctions();
                List<SelectListItem> lstTypes = new List<SelectListItem>();
                lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
                proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
                // DPIU
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                {
                    proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    //proposalViewModel.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, proposalViewModel.IMS_YEAR, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                }
                else if (PMGSYSession.Current.RoleCode == 15)          //PTA 
                {
                    proposalViewModel.STATES = objCommonFuntion.PopulateStatesOfPTA();
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 3)          //STA 
                {
                    proposalViewModel.STATES = objCommonFuntion.PopulateStatesOfPTA();
                    proposalViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrictsOfTA(PMGSYSession.Current.StateCode, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 37 || PMGSYSession.Current.RoleCode == 55 || PMGSYSession.Current.RoleCode == 36)    //  36 is for ITNO    //SRRDA or SRRDAOA or SRRDARCPLWE
                {
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                    proposalViewModel.BLOCKS.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 65)  // Mord and Mord View
                {

                    List<SelectListItem> lstRoadTypes = new List<SelectListItem>();
                    lstRoadTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });

                    proposalViewModel.STATES = objCommonFuntion.PopulateStates();
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0, true);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.AGENCIES = lstRoadTypes;
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                }

                proposalViewModel.PROPOSAL_TYPES = PopulateProposalTypesForShifting();// objCommonFuntion.PopulateProposalTypes();
                proposalViewModel.Years = PopulateYear(0, true, false);
                proposalViewModel.IMS_YEAR = DateTime.Now.Year;
                //proposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("", true);
                proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;

                return View("ListProposalForShifting", proposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.ListProposalForShifting()");
                return null;
            }
        }

        [HttpGet]
        public List<SelectListItem> PopulateProposalTypesForShifting()
        {
            try
            {
                List<SelectListItem> ProposalType = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();
                item.Text = "Road";
                item.Value = "P";
                item.Selected = true;

                ProposalType.Add(item);

                item = new SelectListItem();
                item.Text = "Bridges";
                item.Value = "L";
                ProposalType.Add(item);
                return ProposalType;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.PopulateProposalTypesForShifting()");
                return null;
            }
        }

        /// <summary>
        /// Get proposals for ITNO
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetProposalsForITNOForShifting(FormCollection formCollection)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                int MAST_BLOCK_CODE = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
                int totalRecords;
                ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
                var jsonData = new
                {
                    rows = objProposalBAL.GetProposalsForITNOForShiftingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, formCollection["filters"], out colTotal),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                    TotalColumn = colTotal
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.GetProposalsForITNOForShifting()");
                return null;
            }

        }

        [HttpGet]
        public ActionResult GetShiftDialogBox(String parameter, String hash, String key)
        {

            if (parameter == null || hash == null || key == null)
            {
                return null;
            }

            PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

            ProposalDAL objDAL = new ProposalDAL();
            GetShiftDialogBox modelObj = new GetShiftDialogBox();

            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;

            string message = string.Empty;

            int districtCode = 0;
            int stateCode = 0;
            int blockCode = 0;
            int villageCode = 0;
            var en = parameter + "/" + hash + "/" + key;
            try
            {
                if (en != string.Empty)
                {
                    // ViewBag.EncryptedVillageCode = en;
                    modelObj.EncryptedVillageCode = en;
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    int ProposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"].ToString());
                    //ViewBag.ProposalCode = ProposalCode;

                    modelObj.ProposalCode = ProposalCode;

                    ShiftProposalModel modelFromDAL = new ShiftProposalModel();

                    CommonFunctions objCommonFuntion = new CommonFunctions();
                    modelFromDAL = objDAL.GetPropDetailsDAL(ProposalCode);
                    blockCode = modelFromDAL.BlockCode;
                    districtCode = modelFromDAL.DistrictCode;
                    stateCode = modelFromDAL.StateCode;


                    //  ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);
                    modelObj.STATES = objCommonFuntion.PopulateStates(false);
                    modelObj.MAST_STATE_CODE = PMGSYSession.Current.StateCode;


                    List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                    districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false);
                    // ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                    modelObj.DISTRICTS = objCommonFuntion.PopulateDistrict(stateCode, false);
                    modelObj.MAST_DISTRICT_CODE = districtCode;

                    List<Models.MASTER_BLOCK> blockList = new List<Models.MASTER_BLOCK>();
                    blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(districtCode, false);
                    blockList = blockList.Where(b => b.MAST_BLOCK_CODE != blockCode).ToList<Models.MASTER_BLOCK>();
                    //ViewData["BlockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    modelObj.BLOCKS = objCommonFuntion.PopulateBlocks(districtCode, false);


                    ViewBag.ExistingStateName = modelFromDAL.StateName;
                    ViewBag.ExistingDistrictName = modelFromDAL.DistrictName;
                    ViewBag.ExistingBlockName = modelFromDAL.BlockName;
                    ViewBag.ExistingVillageName = "Test";
                    return PartialView("GetShiftDialogBox", modelObj);


                }
                return PartialView("GetShiftDialogBox", modelObj);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal().GetShiftDialogBox()");
                return PartialView("GetShiftDialogBox");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostShiftDialogBox(GetShiftDialogBox frmCollection)
        {
            bool status = false;
            string encryptedVillageCode = string.Empty;
            string newBlockCode = string.Empty;
            string newDistrictCode = string.Empty;
            string ProposalCode = string.Empty;
            ProposalDAL objDAL = new ProposalDAL();
            try
            {

                //if (string.IsNullOrEmpty(frmCollection["EncryptedVillageCode"]) || string.IsNullOrEmpty(frmCollection["ddlSearchBlocks_ShiftVillage"]))
                //{
                //    message = "Details not shifted successfully.";
                //    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                //}


                if (string.IsNullOrEmpty(frmCollection.EncryptedVillageCode) || string.IsNullOrEmpty(Convert.ToString(frmCollection.MAST_BLOCK_CODE)))
                {
                    message = "Details not shifted successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                encryptedVillageCode = frmCollection.EncryptedVillageCode;//frmCollection["EncryptedVillageCode"];

                newBlockCode = Convert.ToString(frmCollection.MAST_BLOCK_CODE); //frmCollection["ddlSearchBlocks_ShiftVillage"];

                newDistrictCode = Convert.ToString(frmCollection.MAST_DISTRICT_CODE); //frmCollection["ddlSearchDistricts_ShiftVillage"];


                ProposalCode = Convert.ToString(frmCollection.ProposalCode); //frmCollection["ProposalCode"];

                if (objDAL.ShiftProposalDAL(encryptedVillageCode, newBlockCode, newDistrictCode, ProposalCode))
                {

                    message = "Details shifted successfully.";
                    status = true;
                }
                else
                {
                    message = "Proposal Details not Shifted.";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal().PostShiftDialogBox()");
                message = "Details not shifted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult GetLSBDetails(FormCollection formCollection)
        {
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                int MAST_BLOCK_CODE = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
                int totalRecords;
                ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
                var jsonData = new
                {
                    rows = objProposalBAL.GetLSBBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, out colTotal),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                    TotalColumn = colTotal
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal().GetLSBDetails()");
                return null;
            }
        }

        #endregion

        #region Update Proposal PMGSY3
        public ActionResult ListProposalsForUpdatePMGSY3()
        {
            try
            {
                    ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
                    CommonFunctions objCommonFuntion = new CommonFunctions();
                    List<SelectListItem> lstTypes = new List<SelectListItem>();
                    lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                    lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                    lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                    proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
                    proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                    proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
                    proposalViewModel.Years = PopulateYear(0, true, true);
                    proposalViewModel.IMS_YEAR = DateTime.Now.Year;
                    proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;

                    proposalViewModel.STATES = objCommonFuntion.PopulateStates(true);

                    //Below Code commented on 28-03-20222  
                    //if (PMGSYSession.Current.RoleCode == 2)
                    //Below Condition Added on 28-03-20222
                    if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 55)
                    {// SRRDA Role
                        proposalViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                        proposalViewModel.StateName = PMGSYSession.Current.StateName;


                    }

                    return View(proposalViewModel);
                
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListProposalsForUpdatePMGSY3()");
                return null;
            }
        }

        public ActionResult GetProposalsForUpdatePMGSY3(int? page, int? rows, string sidx, string sord)
        {
            ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, Convert.ToInt32(rows), sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int MAST_BLOCK_ID = Convert.ToInt32(Request.Params["MAST_BLOCK_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                int StateCode = Convert.ToInt32(Request.Params["MAST_STATE_CODE"]);
                long totalRecords;

                var jsonData = new
                {
                    //rows = objProposalBAL.GetProposalsForUpdateBAL(page.Value - 1, rows, sidx, sord, out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, PMGSYSession.Current.AdminNdCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT),
                    rows = objProposalDAL.GetProposalsForUpdatePMGSY3DAL(page.Value - 1, rows, sidx, sord, out totalRecords, StateCode, MAST_DISTRICT_ID, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, PMGSYSession.Current.AdminNdCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.GetProposalsForUpdatePMGSY3()");
                return null;
            }
        }

        public ActionResult UpdateProposalDetailsPMGSY3(String parameter, String hash, String key)
        {
            ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();
            try
            {
                ProposalUpdateViewModelPMGSY3 model = new ProposalUpdateViewModelPMGSY3();

                int proposalCode = 0;
                string[] encryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                proposalCode = Convert.ToInt32(encryptedParameters[0]);
                model = objProposalDAL.GetOldProposalDetailsPMGSY3DAL(proposalCode);
                model.EncryptedProposalCode = URLEncrypt.EncryptParameters(new string[] { proposalCode.ToString() });
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateProposalDetailsPMGSY3()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProposalDetailsPMGSY3(ProposalUpdateViewModelPMGSY3 model)
        {
            ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    bool status = objProposalDAL.UpdateProposalDetailsPMGSY3DAL(model, out message);
                    if (status == true)
                    {
                        return Json(new { success = true, message = "Proposal Updated Successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState).ToString() });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateProposalDetailsPMGSY3(ProposalUpdateViewModelPMGSY3 model)");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        #endregion

        #region Delete Non Sanctined and Non Dropped Proposals By MORD
        [Audit]
        public ActionResult DeleteNonSanctionedAndNonDroppedProposal(String parameter, String hash, String key)
        {
            ProposalDAL objProposalDAL = new ProposalDAL();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                bool status = objProposalDAL.DeleteNonSanctionedAndNonDroppedProposalDAL(Convert.ToInt32(decryptedParameters["ProposalCode"]));
                if (status == true)
                {
                    return Json(new { success = true, message = "Proposal Details deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteNonSanctionedAndNonDroppedProposal()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        #endregion

        #region Proposal Listing PMGSY 3  Added on 20 May 2020
        [Audit]
        [HttpGet]
        public ActionResult ListProposalPMGSY3FreezUnfreez()
        {
            ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            List<SelectListItem> lstTypes = new List<SelectListItem>();
            try
            {
                lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
                proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
                // DPIU
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                {
                    proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    //proposalViewModel.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, proposalViewModel.IMS_YEAR, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 15)          //PTA 
                {
                    proposalViewModel.STATES = objCommonFuntion.PopulateStatesOfPTA();
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 3)          //STA 
                {
                    proposalViewModel.STATES = objCommonFuntion.PopulateStatesOfPTA();
                    proposalViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrictsOfTA(PMGSYSession.Current.StateCode, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 37 || PMGSYSession.Current.RoleCode == 55)          //SRRDA or SRRDAOA or SRRDARCPLWE
                {
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                    proposalViewModel.BLOCKS.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 65)  // Mord and Mord View
                {

                    List<SelectListItem> lstRoadTypes = new List<SelectListItem>();
                    lstRoadTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });

                    proposalViewModel.STATES = objCommonFuntion.PopulateStates(true);
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0, true);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.AGENCIES = lstRoadTypes;
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                }

                proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
                proposalViewModel.Years = PopulateYear(0, true, true);
                proposalViewModel.IMS_YEAR = DateTime.Now.Year;

                SelectListItem itm = proposalViewModel.Years.Where(z => z.Value == DateTime.Now.Year.ToString().Trim()).FirstOrDefault();
                if (DateTime.Now.Month <= 3)
                {
                    proposalViewModel.Years.Remove(itm);
                    proposalViewModel.IMS_YEAR = DateTime.Now.Year - 1;
                }

                //proposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("", true);
                proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;

                return View("ListProposalPMGSY3FreezUnfreez", proposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.ListProposalPMGSY3FreezUnfreez()");
                return null;
            }
        }
       
        [HttpPost]
        [Audit]
        public ActionResult GetProposalsForSRRDAFreezeUnfreeze(FormCollection formCollection)
        {
            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 29-Apr-2014 end
            int IMS_YEAR = Convert.ToInt32(Request.Params["YEAR"]);
            int IMS_STATE = Convert.ToInt32(Request.Params["STATE"]); // This Field is used as state Code
            int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
            string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
            int MAST_BLOCK_CODE = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
            int totalRecords;
            ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
            ProposalDAL objDAL = new ProposalDAL();
            var jsonData = new
            {
                rows = objDAL.GetProposalForSRRDAFreezeUnfreezeDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_STATE, MAST_DISTRICT_ID, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, formCollection["filters"], out colTotal),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
                TotalColumn = colTotal
            };
            return Json(jsonData);
        }
        // FreezeUnfreezeDetails
        [HttpPost]
        public ActionResult FreezeDetails()
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                var ProposalID = Convert.ToInt32(Request.Params["ProposalCode"]);



                var entry = dbcontext.IMS_SANCTIONED_PROJECTS.Where(obj => obj.IMS_PR_ROAD_CODE == ProposalID).FirstOrDefault();
                if (entry != null)
                {
                    entry.IMS_PROGRESS_STATUS_FREEZE = "Y";
                    entry.IMS_FREEZE_STATUS = "F";//Added on 13-10-2021
                    entry.IMS_PROGRESS_STATUS_DATE = System.DateTime.Now;

                    dbcontext.Entry(entry).State = System.Data.Entity.EntityState.Modified;
                    dbcontext.SaveChanges();
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
                }

                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalController/FreezeDetails");
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
        }
        [HttpPost]
        public ActionResult UnFreezeDetails()
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                var ProposalID = Convert.ToInt32(Request.Params["ProposalCode"]);



                var entry = dbcontext.IMS_SANCTIONED_PROJECTS.Where(obj => obj.IMS_PR_ROAD_CODE == ProposalID).FirstOrDefault();
                if (entry != null)
                {
                    entry.IMS_PROGRESS_STATUS_FREEZE = "N";
                    entry.IMS_FREEZE_STATUS = "U";//Added on 13-10-2021
                    entry.IMS_PROGRESS_STATUS_DATE = System.DateTime.Now;

                    dbcontext.Entry(entry).State = System.Data.Entity.EntityState.Modified;
                    dbcontext.SaveChanges();
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
                }

                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalController/UnFreezeDetails");
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
        }

        #endregion

		#region Freeze Unfreeze New for MoRD Added on 23 Dec 2021 by Srishti tyagi

        [Audit]
        [HttpGet]
        public ActionResult FreezeUnfreezeNew()
        {
            FreezeUnfreezeViewModel freezeUnfreezeModel = new FreezeUnfreezeViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
          
            try
            {
                if (PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 2)
                {
                    if (PMGSYSession.Current.RoleCode == 25)  // Mord and Mord View
                    {
                        freezeUnfreezeModel.STATES = objCommonFuntion.PopulateStates(true);
                        freezeUnfreezeModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0, true);
                    }
                }
                return View("FreezeUnfreezeNew", freezeUnfreezeModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.FreezeUnfreezeNew()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetFreezeUnfreezeList(FormCollection formCollection)
        {
            FreezeUnfreezeViewModel freezeunfreezeModel = new FreezeUnfreezeViewModel();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int IMS_STATE = Convert.ToInt32(Request.Params["STATE"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);

                int totalRecords;

                ProposalDAL objDAL = new ProposalDAL();
                List<int> SelectedIdList = new List<int>();

                var jsonData = new
                {
                    rows = objDAL.GetFreezeUnfreezeDAL(IMS_STATE, MAST_DISTRICT_ID, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, out SelectedIdList),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                    userdata = new { ids = SelectedIdList.ToArray<int>() }
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalDAL()GetFreezeUnfreezeList()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult FreezeMordDetails(int[] submitarray)
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();

                int[] arr = submitarray;

                foreach (int item in submitarray)
                {
                    var entry = dbcontext.IMS_SANCTIONED_PROJECTS.Where(obj => obj.IMS_PR_ROAD_CODE == item).FirstOrDefault();
                    if (entry != null)
                    {
                        entry.IMS_PROGRESS_STATUS_FREEZE = "Y";
                        entry.IMS_FREEZE_STATUS = "F";
                        entry.IMS_PROGRESS_STATUS_DATE = System.DateTime.Now;

                        dbcontext.Entry(entry).State = System.Data.Entity.EntityState.Modified;
                        ((IObjectContextAdapter)dbcontext).ObjectContext.CommandTimeout = 180;
                        dbcontext.SaveChanges();

                    }

                }               

                return Json(new { success = true }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalController/FreezeMordDetails");
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public ActionResult UnFreezeMordDetails(string id)
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                int roadId = Convert.ToInt32(Request.Params["SANCTION_CODE"]);

                var entry = dbcontext.IMS_SANCTIONED_PROJECTS.Where(obj => obj.IMS_PR_ROAD_CODE == roadId).FirstOrDefault();
                if (entry != null)
                {
                    entry.IMS_PROGRESS_STATUS_FREEZE = "N";
                    entry.IMS_FREEZE_STATUS = "U";
                    entry.IMS_PROGRESS_STATUS_DATE = System.DateTime.Now;

                    dbcontext.Entry(entry).State = System.Data.Entity.EntityState.Modified;
                    dbcontext.SaveChanges();
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
                }

                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalController/UnFreezeMordDetails");
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
        }


        #endregion

        #region Reject Drop Request

        [HttpGet]
        public ActionResult RejectDropOrderView(string[] ApproveRoads)
        {
            DropOrderViewModel model = new DropOrderViewModel();
            PMGSY.DAL.Proposal.ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();
            decryptedParameters = new Dictionary<string, string>();
            try
            {
                List<int> imsRoadCodeList = new List<int>();

                for (int i = 0; i < ApproveRoads.Length; i++)
                {
                    string[] singleEncoded = ApproveRoads[i].Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { singleEncoded[0], singleEncoded[1], singleEncoded[2] });
                    int ImsRoadCode = Convert.ToInt32(decryptedParameters["ImsRoadCode"]);
                    imsRoadCodeList.Add(ImsRoadCode);
                }

                TempData["mrdSelectedRoads"] = imsRoadCodeList;

                if (!String.IsNullOrEmpty(Request.Params["StateCode"]))
                {
                    model.StateCode = Convert.ToInt32(Request.Params["StateCode"]);
                }

                //if (!String.IsNullOrEmpty(Request.Params["YearCode"]))
                //{
                //    model.YearCode = Convert.ToInt32(Request.Params["YearCode"]);
                //}

                //if (!String.IsNullOrEmpty(Request.Params["StreamCode"]))
                //{
                //    model.StreamCode = Convert.ToInt32(Request.Params["StreamCode"]);
                //}

                //if (!String.IsNullOrEmpty(Request.Params["BatchCode"]))
                //{
                //    model.BatchCode = Convert.ToInt32(Request.Params["BatchCode"]);
                //}

                //if (!String.IsNullOrEmpty(Request.Params["SchemeCode"]))
                //{
                //    model.PMGSYScheme = Convert.ToInt32(Request.Params["SchemeCode"]);
                //}
                model.PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
                if (!String.IsNullOrEmpty(Request.Params["RequestCode"]))
                {
                    model.RequestCode = Convert.ToInt32(Request.Params["RequestCode"]);
                }

                model.IMS_REQUEST_ORDER_DATE = objProposalDAL.getRequestLetterDateDAL(model.RequestCode);
                //model.IsDOGenerated = objProposalDAL.IsDropOrderGenerated(model);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RejectDropOrderView()");
                return null;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRejectDropOrder(DropOrderViewModel model)
        {
            ModelState.Remove("IMS_DROP_ORDER_NUMBER");
            ModelState.Remove("IMS_DROP_ORDER_DATE");
            string message = string.Empty;
            ProposalDAL objproposalDAL = new ProposalDAL();
            try
            {
                List<int> mrdselectedroadList = (List<int>)TempData["mrdSelectedRoads"];
                if (ModelState.IsValid)
                {
                    bool Status = objproposalDAL.AddRejectDropOrderDAL(model, mrdselectedroadList, ref message);
                    if (Status)
                        return Json(new { success = true, message = message });
                    else
                        return Json(new { success = false, message = message });
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddRejectDropOrder()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        //Delete
        public ActionResult DeleteRequestDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            ProposalDAL objproposalDAL = new ProposalDAL();
            int reqCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                reqCode = Convert.ToInt32(decryptedParameters["DropReqID"]);
                //  objBAL = new ARRRBAL();
                bool status = objproposalDAL.DeleteReqDAL(reqCode);
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
                ErrorLog.LogError(ex, "DeleteRequestDetails()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }


        #endregion

        #region Proposals for PMGSY Scheme 5 (Vibrant Village) - Srishti Tyagi 27/06/2023

        [Audit]
        [HttpGet]
        //Changed By Hrishikesh For STA and PTA --start --12-07-2023
        public ActionResult ListProposalVibrantVillage()
        {
            ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            List<SelectListItem> lstTypes = new List<SelectListItem>();
            try
            {
                lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
                proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
                // DPIU
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                {
                    proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    //proposalViewModel.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, proposalViewModel.IMS_YEAR, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 15)          //PTA 
                {
                    proposalViewModel.STATES = objCommonFuntion.PopulateStatesOfPTA();
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 3)          //STA 
                {
                    proposalViewModel.STATES = objCommonFuntion.PopulateStatesOfPTA();
                    proposalViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrictsOfTA(PMGSYSession.Current.StateCode, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 37 || PMGSYSession.Current.RoleCode == 55)          //SRRDA or SRRDAOA or SRRDARCPLWE
                {
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                    proposalViewModel.BLOCKS.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 65)  // Mord and Mord View
                {

                    List<SelectListItem> lstRoadTypes = new List<SelectListItem>();
                    lstRoadTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });

                    proposalViewModel.STATES = objCommonFuntion.PopulateStates();
                    proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(0, true);
                    proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                    proposalViewModel.BATCHS.RemoveAt(0);
                    proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                    proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                    proposalViewModel.AGENCIES = lstRoadTypes;
                    proposalViewModel.CONNECTIVITYLIST = lstTypes;
                }

                proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
                proposalViewModel.Years = PopulateYear(0, true, true);
                proposalViewModel.IMS_YEAR = DateTime.Now.Year;

                SelectListItem itm = proposalViewModel.Years.Where(z => z.Value == DateTime.Now.Year.ToString().Trim()).FirstOrDefault();
                if (DateTime.Now.Month <= 3)
                {
                    proposalViewModel.Years.Remove(itm);
                    proposalViewModel.IMS_YEAR = DateTime.Now.Year - 1;
                }

                proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;

                return View(proposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Proposal.ListProposalVibrantVillage()");
                return null;
            }
        }
        //Changed By Hrishikesh For STA and PTA --End --12-07-2023


        [HttpPost]
        [Audit]
        public ActionResult GetProposalsVibrantVillage(FormCollection formCollection)
        {
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int MAST_BLOCK_ID = Convert.ToInt32(Request.Params["MAST_BLOCK_ID"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
            string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
            string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
            int totalRecords;
            ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
            var jsonData = new
            {
                rows = objProposalBAL.GetProposalsVibrantVillageBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, PMGSYSession.Current.AdminNdCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, formCollection["filters"], out colTotal),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
                TotalColumn = colTotal
            };
            return Json(jsonData);
        }

        [HttpGet]
        [Audit]
        public ActionResult CreateVibrantVillage(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            ProposalDAL objProposalDAL = new ProposalDAL();
            PMGSY.Models.Proposal.ProposalViewModel proposalViewModel = new ProposalViewModel();
            ViewBag.operation = "C";

            #region Default values

            proposalViewModel.StateName = PMGSYSession.Current.StateName;
            proposalViewModel.DistrictName = PMGSYSession.Current.DistrictName;
            proposalViewModel.stateType = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_TYPE).FirstOrDefault();
            proposalViewModel.DistrictType = (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_IAP_DISTRICT).FirstOrDefault();
            proposalViewModel.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_SHORT_CODE).FirstOrDefault() +
                                               (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_DISTRICT_ID).FirstOrDefault().ToString("D2");


            proposalViewModel.IMS_UPGRADE_CONNECT = "N";

            proposalViewModel.IMS_EXISTING_PACKAGE = "N";

            proposalViewModel.IMS_IS_STAGED = "C";
            proposalViewModel.IMS_ISBENEFITTED_HABS = "Y";
            proposalViewModel.IMS_STATE_SHARE = 0;
            proposalViewModel.IMS_PROPOSED_SURFACE = "S";

            proposalViewModel.IMS_YEAR = DateTime.Now.Year;
            // Amounts 
            proposalViewModel.IMS_SANCTIONED_PAV_AMT = 0;
            proposalViewModel.IMS_SANCTIONED_CD_AMT = 0;
            proposalViewModel.IMS_SANCTIONED_PW_AMT = 0;
            proposalViewModel.IMS_SANCTIONED_OW_AMT = 0;

            proposalViewModel.IMS_SANCTIONED_MAN_AMT1 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT2 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT3 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT4 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT5 = 0;

            proposalViewModel.IMS_FURNITURE_COST = 0;
            proposalViewModel.IMS_RENEWAL_COST = 0;

            //For PMGSY Scheme-2
            if (PMGSYSession.Current.PMGSYScheme == 2)
            {
                proposalViewModel.IMS_IS_HIGHER_SPECIFICATION = "N";
                proposalViewModel.IMS_SANCTIONED_AMOUNT = 0;
                proposalViewModel.IMS_SHARE_PERCENT = 2;
                proposalViewModel.IMS_HIGHER_SPECIFICATION_COST = 0;
                proposalViewModel.IMS_SANCTIONED_HS_AMT = 0;
                proposalViewModel.IMS_SANCTIONED_FC_AMT = 0;
                proposalViewModel.IMS_SANCTIONED_RENEWAL_AMT = 0;
            }
            #endregion

            if (id != "" && id != null)
            {
                string[] defaultValues = id.Split('$');
                if (defaultValues[0] != "" && defaultValues[0] != null)
                {
                    proposalViewModel.IMS_YEAR = Convert.ToInt32(defaultValues[0]);
                }

                if (defaultValues[1] != "" && defaultValues[1] != null)
                {
                    proposalViewModel.MAST_BLOCK_CODE = Convert.ToInt32(defaultValues[1]);
                }

                if (defaultValues[2] != "" && defaultValues[2] != null)
                {
                    proposalViewModel.IMS_BATCH = Convert.ToInt32(defaultValues[2]);
                }

                if (defaultValues[3] != "" && defaultValues[3] != null)
                {
                    proposalViewModel.IMS_COLLABORATION = Convert.ToInt32(defaultValues[3]);
                }

            }

            proposalViewModel.Years = PopulateYear();

            CommonFunctions objCommonFuntion = new CommonFunctions();
            proposalViewModel.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, proposalViewModel.IMS_YEAR, false, PMGSYSession.Current.PMGSYScheme == 1 ? true : false);

            proposalViewModel.COLLABORATIONS = new List<SelectListItem>();//objCommonFuntion.PopulateFundingAgency();
            proposalViewModel.COLLABORATIONS.Insert(0, new SelectListItem { Text = "Select Funding Agency", Value = "-1", Selected = true });

            proposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("P");
            proposalViewModel.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
            if (proposalViewModel.MAST_BLOCK_CODE != 0)
            {
                proposalViewModel.CN_ROADS = objProposalDAL.PopulateLinkThroughListPMGSY3DAL(proposalViewModel.MAST_BLOCK_CODE, proposalViewModel.IMS_BATCH);
            }
            else
            {
                proposalViewModel.CN_ROADS = PopulateLinkThrough(0, "N", "P");
            }

            proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode); //PMGSYSession.Current.PMGSYScheme == 3 ? objCommonFuntion.PopulateBlocksforRCPLWE(PMGSYSession.Current.DistrictCode) : objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode);
            proposalViewModel.MP_CONSTITUENCY = objCommonFuntion.PopulateMPConstituency(0);
            proposalViewModel.MLA_CONSTITUENCY = objCommonFuntion.PopulateMLAConstituency(0);
            proposalViewModel.CARRIAGED_WIDTH = objCommonFuntion.PopulateCarriageWidth();
            proposalViewModel.HABS_REASON = objCommonFuntion.PopulateReason("H");
            proposalViewModel.EXISTING_SURFACE = objCommonFuntion.PopulateSurfaceType(false);
            proposalViewModel.TRAFFIC_TYPE = objCommonFuntion.PopulateTrafficType();
            proposalViewModel.PROPOSED_SURFACE = objCommonFuntion.PopulateProposedSurface();

            ViewBag.EXISTING_IMS_PACKAGE_ID = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

            ViewBag.lstYear = proposalViewModel.Years;

            ViewBag.Stage_2_Year = new SelectList(PopulateYear(0, false).AsEnumerable<SelectListItem>(), "Value", "Text", DateTime.Now.Year);
            ViewBag.Stage_2_Package_ID = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

            proposalViewModel.STAGE1_PROPOSAL_ROADS = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();

            proposalViewModel.IsProposalFinanciallyClosed = false;
            int shareCode = db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault();
            ViewBag.shareCode = shareCode;
            proposalViewModel.IMS_SHARE_PERCENT_2015 = shareCode == 0 ? (byte)3 : (byte)shareCode;
            proposalViewModel.IMS_IS_HIGHER_SPECIFICATION = "N";

            return View(proposalViewModel);
        }

        [HttpPost]
        [Audit]
        public ActionResult CreateProposalPMGSY5(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)
        {
            ProposalDAL objprDAL = new DAL.Proposal.ProposalDAL();
            try
            {

                ModelState.Remove("TotalPostDLPMaintenanceCost");
                ModelState.Remove("PUCCA_SIDE_DRAIN_LENGTH");
                ModelState.Remove("PROTECTION_LENGTH");
                ModelState.Remove("SURFACE_BRICK_SOLLING");


                ModelState.Remove("SURFACE_BT");
                ModelState.Remove("SURFACE_CC");
                ModelState.Remove("SURFACE_GRAVEL");
                ModelState.Remove("SURFACE_MOORUM");

                ModelState.Remove("SURFACE_TRACK");
                ModelState.Remove("SURFACE_WBM");

                // Added on 18 March 2021
                ModelState.Remove("EXISTING_CARRIAGEWAY_WIDTH");
                ModelState.Remove("EXISTING_CARRIAGEWAY_PUC");

                ViewBag.operation = "C";
                if (ModelState.IsValid)
                {
                    string route = objprDAL.getRoadRoute(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE));
                    if (route == "N" && ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "U" && PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        return Json(new { Success = false, ErrorMessage = "Please select New Connectivity for Missing Link Route" });
                    }
                    string Status = objProposalBAL.SaveRoadProposalPMGSY5BAL(ims_sanctioned_projects);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "CreateProposalPMGSY5(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)");
                throw ex;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult EditProposalPMGSY5(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            ProposalDAL objProposalDAL = new ProposalDAL();
            bool isIAP = false;
            CommonFunctions objCommonFuntion = new CommonFunctions();
            int IMS_PR_ROAD_CODE = 0;

            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                    }
                }
                ViewBag.operation = "U";

                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);

                if (ims_sanctioned_projects == null)
                {
                    return Json(new { Success = false, ErrorMessage = "Proposal Not Found" });
                }

                PMGSY.Models.Proposal.ProposalViewModel objProposal = new ProposalViewModel();

                var adapter = (IObjectContextAdapter)db;
                var objectContext = adapter.ObjectContext;
                objectContext.CommandTimeout = 0;

                objProposal.IS_PAYMENT_MADE = "N";
                objProposal.EXISTING_CARRIAGEWAY_WIDTH = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_WIDTH;
                objProposal.EXISTING_CARRIAGEWAY_PUC = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_PUC;

                objProposal.operation = "U";    //For Update operation

                objProposal.StateName = PMGSYSession.Current.StateName;
                objProposal.DistrictName = PMGSYSession.Current.DistrictName;

                objProposal.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
                objProposal.MAST_STATE_CODE = ims_sanctioned_projects.MAST_STATE_CODE;
                objProposal.stateType = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_TYPE).FirstOrDefault();
                objProposal.MAST_DISTRICT_CODE = ims_sanctioned_projects.MAST_DISTRICT_CODE;
                objProposal.MAST_DPIU_CODE = ims_sanctioned_projects.MAST_DPIU_CODE;
                objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;

                objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;
                objProposal.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode);

                objProposal.IMS_COLLABORATION = ims_sanctioned_projects.IMS_COLLABORATION;
                objProposal.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency();

                // Upgradation Proposal
                //if (ims_sanctioned_projects.IMS_UPGRADE_CONNECT.ToUpper() == "U")
                //{
                objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;

                objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;

                //if (ims_sanctioned_projects.IMS_ISBENEFITTED_HABS == "N")
                //{
                objProposal.IMS_HABS_REASON = ims_sanctioned_projects.IMS_HABS_REASON;
                //    }
                //}
                objProposal.HABS_REASON = objCommonFuntion.PopulateReason("H");

                objProposal.IMS_IS_STAGED = ims_sanctioned_projects.IMS_IS_STAGED;

                // For staged Propsal
                if (objProposal.IMS_IS_STAGED == "S")
                {
                    objProposal.IMS_STAGE_PHASE = ims_sanctioned_projects.IMS_STAGE_PHASE;
                    //Stage 1
                    if (objProposal.IMS_STAGE_PHASE == "S1")
                    {
                        objProposal.IMS_STAGE_PHASE = "1";
                        objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;

                        objProposal.CN_ROADS = new List<SelectListItem>();
                        objProposal.CN_ROADS.Insert(0, new SelectListItem() { Text = ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME.Trim(), Value = ims_sanctioned_projects.PLAN_CN_ROAD_CODE.ToString() });
                    }
                    else if (objProposal.IMS_STAGE_PHASE == "S2")
                    {
                        objProposal.IMS_STAGE_PHASE = "2";
                        objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                        objProposal.CN_ROADS = new List<SelectListItem>();
                        objProposal.CN_ROADS.Insert(0, new SelectListItem() { Text = ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME.Trim(), Value = ims_sanctioned_projects.PLAN_CN_ROAD_CODE.ToString() });

                    }
                }
                else if (objProposal.IMS_IS_STAGED == "C")
                {
                    objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;

                    if (objProposal.IMS_COLLABORATION == 5)
                    {
                        objProposal.CN_ROADS = new List<SelectListItem>();
                        objProposal.CN_ROADS.Insert(0, new SelectListItem() { Text = ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME.Trim(), Value = ims_sanctioned_projects.PLAN_CN_ROAD_CODE.ToString() });
                    }
                    else
                    {
                        objProposal.CN_ROADS = new List<SelectListItem>();
                        objProposal.CN_ROADS.Insert(0, new SelectListItem() { Text = ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME.Trim(), Value = ims_sanctioned_projects.PLAN_CN_ROAD_CODE.ToString() });
                    }
                }

                objProposal.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_SHORT_CODE).FirstOrDefault() +
                                                   (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_DISTRICT_ID).FirstOrDefault();
                objProposal.IMS_EXISTING_PACKAGE = "E";
                objProposal.EXISTING_IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;

                objProposal.EXISTING_PACKAGES = GetStagedPackageID(ims_sanctioned_projects.IMS_YEAR, ims_sanctioned_projects.IMS_BATCH);
                objProposal.EXISTING_PACKAGES.Insert(0, new SelectListItem { Text = "Select Package", Value = "" });
                objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;

                objProposal.IMS_PARTIAL_LEN = ims_sanctioned_projects.IMS_PARTIAL_LEN;

                objProposal.IMS_CC_LENGTH = ims_sanctioned_projects.IMS_CC_LENGTH;
                objProposal.IMS_BT_LENGTH = ims_sanctioned_projects.IMS_BT_LENGTH;

                objProposal.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;

                // For Stage Two Directly Get the Length of Road
                if (objProposal.IMS_STAGE_PHASE == "2")
                {
                    if (ims_sanctioned_projects.PLAN_CN_ROAD_CODE != null)
                    {
                        string IMS_PAV_LENGTH = objProposalBAL.GetRoadDetailsForStageTwoProposalBAL(Convert.ToInt32(ims_sanctioned_projects.IMS_STAGED_ROAD_ID), Convert.ToInt32(objProposal.PLAN_CN_ROAD_CODE));
                        if (IMS_PAV_LENGTH != string.Empty)
                        {
                            objProposal.DUP_IMS_PAV_LENGTH = Convert.ToDecimal(IMS_PAV_LENGTH);
                        }
                    }

                    objProposal.IMS_STAGE1_PAV_LENGTH = db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_STAGED_ROAD_ID).Select(c => c.IMS_PAV_LENGTH).FirstOrDefault();
                    objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;
                    objProposal.STAGE1_PROPOSAL_ROADS = new SelectList(db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_STAGED_ROAD_ID), "IMS_PR_ROAD_CODE", "IMS_ROAD_NAME").ToList();
                }
                // Here We Calculate the Remaining Length id 
                else
                {
                    if (ims_sanctioned_projects.PLAN_CN_ROAD_CODE != null)
                    {
                        string IMS_PAV_LENGTH = objProposalBAL.GetRoadDetailsForUpdate(objProposal.IMS_PR_ROAD_CODE, Convert.ToInt32(objProposal.PLAN_CN_ROAD_CODE));
                        if (IMS_PAV_LENGTH != string.Empty)
                        {
                            objProposal.DUP_IMS_PAV_LENGTH = Convert.ToDecimal(IMS_PAV_LENGTH);
                        }
                    }

                    objProposal.STAGE1_PROPOSAL_ROADS = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();
                }



                objProposal.IMS_NO_OF_CDWORKS = ims_sanctioned_projects.IMS_NO_OF_CDWORKS;

                objProposal.IMS_ZP_RESO_OBTAINED = ims_sanctioned_projects.IMS_ZP_RESO_OBTAINED;
                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS == null ? ims_sanctioned_projects.IMS_REMARKS : ims_sanctioned_projects.IMS_REMARKS.Trim();

                objProposal.IMS_STATE_SHARE = ims_sanctioned_projects.IMS_STATE_SHARE;
                objProposal.IMS_SANCTIONED_PAV_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;
                objProposal.IMS_SANCTIONED_CD_AMT = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;
                objProposal.IMS_SANCTIONED_PW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;
                objProposal.IMS_SANCTIONED_OW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;

                //if (PMGSYSession.Current.PMGSYScheme == 4)
                //{
                objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
                objProposal.TotalCost = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT + ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT +
                                                    ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT + ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT +
                                                    Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT);
                objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;
                objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                objProposal.IMS_FURNITURE_COST = ims_sanctioned_projects.IMS_FURNITURE_COST;
                objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST;
                objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT;
                objProposal.IMS_SANCTIONED_FC_AMT = ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT;
                objProposal.IMS_SANCTIONED_RENEWAL_AMT = ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT;

                //if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 1)
                //{
                //    objProposal.IMS_SANCTIONED_AMOUNT = (objProposal.TotalCost * 90) / 100;
                //}
                //else if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 2)
                //{
                objProposal.IMS_SANCTIONED_AMOUNT = (objProposal.TotalCost * 75) / 100;
                //}
                //}


                objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                if (PMGSYSession.Current.PMGSYScheme == 3)
                {
                    objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST ?? 0;
                }

                objProposal.TotalMaintenanceCost = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2 +
                                                   ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4 +
                                                   ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5 +    //In case of PMGSY Scheme-II include IMS_SANCTIONED_RENEWAL_AMT
                                                   (PMGSYSession.Current.PMGSYScheme == 4
                                                    ? Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT)
                                                    : 0.0M);

                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.Years = PopulateYear();//PopulateYear(ims_sanctioned_projects.IMS_YEAR);

                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;

                objProposal.BATCHS = new List<SelectListItem>();
                objProposal.BATCHS.Insert(0, new SelectListItem() { Text = "Batch " + Convert.ToString(ims_sanctioned_projects.IMS_BATCH), Value = Convert.ToString(ims_sanctioned_projects.IMS_BATCH) });

                objProposal.isPaymentDone = checkIsPayment(ims_sanctioned_projects.IMS_PR_ROAD_CODE);

                if (PMGSYSession.Current.StateCode == 17)
                { // Karnataka State. All Proposal to be edited in case of Payment Is Made. // Suggested by Pankaj Sir on 08 Jan 2021
                    objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                    objProposal.STREAMS = objCommonFuntion.PopulateStreams("P");
                    objProposal.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
                }

                if (!objProposal.isPaymentDone)
                {
                    objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                    objProposal.STREAMS = objCommonFuntion.PopulateStreams("P");
                    objProposal.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
                }



                objProposal.MAST_MP_CONST_CODE = ims_sanctioned_projects.MAST_MP_CONST_CODE;
                objProposal.MP_CONSTITUENCY = objCommonFuntion.PopulateMPConstituency(ims_sanctioned_projects.MAST_BLOCK_CODE);

                objProposal.MAST_MLA_CONST_CODE = ims_sanctioned_projects.MAST_MLA_CONST_CODE;
                objProposal.MLA_CONSTITUENCY = objCommonFuntion.PopulateMLAConstituency(ims_sanctioned_projects.MAST_BLOCK_CODE);

                objProposal.IMS_CARRIAGED_WIDTH = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH;
                objProposal.CARRIAGED_WIDTH = objCommonFuntion.PopulateCarriageWidth();

                objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;
                objProposal.EXISTING_SURFACE = objCommonFuntion.PopulateSurfaceType();

                objProposal.IMS_TRAFFIC_TYPE = ims_sanctioned_projects.IMS_TRAFFIC_TYPE;
                objProposal.TRAFFIC_TYPE = objCommonFuntion.PopulateTrafficType();

                objProposal.IMS_PROPOSED_SURFACE = ims_sanctioned_projects.IMS_PROPOSED_SURFACE;
                objProposal.PROPOSED_SURFACE = objCommonFuntion.PopulateProposedSurface();

                objProposal.Stage_2_Year = ims_sanctioned_projects.IMS_STAGED_YEAR;
                objProposal.Stage_2_Package_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;
                objProposal.PACKAGES = GetStagedPackageID(Convert.ToInt32(ims_sanctioned_projects.IMS_STAGED_YEAR), 0);
                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_MORD_SHARE_2015.Value : 0;
                objProposal.IMS_TOTAL_COST_2015 = objProposal.IMS_STATE_SHARE_2015 + objProposal.IMS_MORD_SHARE_2015;
                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null ? ((byte)(db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault() == 0 ? 3 : db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault())) : ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;

                List<SelectListItem> lstYear = new List<SelectListItem>();
                lstYear = PopulateYear();
                int year = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Max(m => m.IMS_YEAR);
                if (year != DateTime.Now.Year)
                {
                    year = year + 1;
                }
                int count = lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString()));
                lstYear.RemoveRange(lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString())), lstYear.Count - count);
                ViewBag.lstYear = lstYear;

                ViewBag.shareCode = objProposal.IMS_SHARE_PERCENT_2015;

                objProposal.ImsRidingQualityLength = ims_sanctioned_projects.IMS_RIDING_QUALITY_LENGTH.Value;
                objProposal.ImsPuccaSideDrains = ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS.Value;
                objProposal.ImsGSTCost = ims_sanctioned_projects.IMS_GST_COST.Value;

                IMS_COST_COMPONENT imsCostComponent = ims_sanctioned_projects.IMS_COST_COMPONENT.Where(z => z.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_PR_ROAD_CODE).FirstOrDefault();

                if (imsCostComponent != null)
                {
                    objProposal.imsComponentId = imsCostComponent.IMS_COMPONENT_ID;
                    objProposal.ImsClearing = imsCostComponent.IMS_CLEARING;
                    objProposal.ImsExcavation = imsCostComponent.IMS_EXCAVATION;
                    objProposal.ImsFilling = imsCostComponent.IMS_FILLING;
                    objProposal.ImsSubGrade = imsCostComponent.IMS_SUB_GRADE;
                    objProposal.ImsShoulder = imsCostComponent.IMS_SHOULDER;
                    objProposal.ImsGranularSubBase = imsCostComponent.IMS_GRANULAR_SUB_BASE;
                    objProposal.ImsSoilAggregate = imsCostComponent.IMS_SOIL_AGGREGATE;
                    objProposal.ImsWBMGradeII = imsCostComponent.IMS_WBM_GRADE_II;
                    objProposal.ImsWBMGradeIII = imsCostComponent.IMS_WBM_GRADE_III;
                    objProposal.ImsWMM = imsCostComponent.IMS_WMM;
                    objProposal.ImsPrimeCoat = imsCostComponent.IMS_PRIME_COAT;
                    objProposal.ImsTackCoat = imsCostComponent.IMS_TACK_COAT;
                    objProposal.ImsBMDBM = imsCostComponent.IMS_BM_DBM;
                    objProposal.ImsOGPC_SDBC_BC = imsCostComponent.IMS_OGPC_SDBC_BC;
                    objProposal.ImsSealCoat = imsCostComponent.IMS_SEAL_COAT;
                    objProposal.ImsSurfaceDressing = imsCostComponent.IMS_SURFACE_DRESSING;
                    objProposal.ImsDryLeanConcrete = imsCostComponent.IMS_DRY_LEAN_CONCRETE;
                    objProposal.ImsConcretePavement = imsCostComponent.IMS_CONCRETE_PAVEMENT;
                }

                //if (PMGSYSession.Current.PMGSYScheme == 4)
                //{
                #region For Post DPL Maintenance Cost
                objProposal.PUCCA_SIDE_DRAIN_LENGTH = ims_sanctioned_projects.PUCCA_SIDE_DRAIN_LENGTH;
                objProposal.PROTECTION_LENGTH = ims_sanctioned_projects.PROTECTION_LENGTH;
                objProposal.IMS_MAINTENANCE_YEAR6 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR6);
                objProposal.IMS_MAINTENANCE_YEAR7 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR7);
                objProposal.IMS_MAINTENANCE_YEAR8 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR8);
                objProposal.IMS_MAINTENANCE_YEAR9 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR9);
                objProposal.IMS_MAINTENANCE_YEAR10 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR10);

                objProposal.TotalPostDLPMaintenanceCost = objProposal.IMS_MAINTENANCE_YEAR6 + objProposal.IMS_MAINTENANCE_YEAR7 + objProposal.IMS_MAINTENANCE_YEAR8 + objProposal.IMS_MAINTENANCE_YEAR9 + objProposal.IMS_MAINTENANCE_YEAR10;
                #endregion

                #region Existing Surface Details
                objProposal.SURFACE_BRICK_SOLLING = ims_sanctioned_projects.SURFACE_BRICK_SOLLING;
                objProposal.SURFACE_BT = ims_sanctioned_projects.SURFACE_BT;
                objProposal.SURFACE_CC = ims_sanctioned_projects.SURFACE_CC;
                objProposal.SURFACE_GRAVEL = ims_sanctioned_projects.SURFACE_GRAVEL;
                objProposal.SURFACE_MOORUM = ims_sanctioned_projects.SURFACE_MOORUM;
                objProposal.SURFACE_TRACK = ims_sanctioned_projects.SURFACE_TRACK;
                objProposal.SURFACE_WBM = ims_sanctioned_projects.SURFACE_WBM;
                #endregion
                //}

                return View("CreateVibrantVillage", objProposal);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditProposalPMGSY5()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult EditProposalVibrantVillages(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)
        {
            ProposalDAL objprDAL = new DAL.Proposal.ProposalDAL();
            try
            {
                ModelState.Remove("TotalPostDLPMaintenanceCost");
                ModelState.Remove("PUCCA_SIDE_DRAIN_LENGTH");
                ModelState.Remove("PROTECTION_LENGTH");
                ModelState.Remove("SURFACE_BRICK_SOLLING");


                ModelState.Remove("SURFACE_BT");
                ModelState.Remove("SURFACE_CC");
                ModelState.Remove("SURFACE_GRAVEL");
                ModelState.Remove("SURFACE_MOORUM");

                ModelState.Remove("SURFACE_TRACK");
                ModelState.Remove("SURFACE_WBM");

                ModelState.Remove("EXISTING_CARRIAGEWAY_WIDTH");
                ModelState.Remove("EXISTING_CARRIAGEWAY_PUC");

                ViewBag.Operation = "E";
                if (ims_sanctioned_projects.isPaymentDone)
                {
                    ModelState.Remove("IMS_STREAMS");
                }
                /*if (PMGSYSession.Current.PMGSYScheme == 4)
                {*/
                decimal TotalSurfaceLength = Convert.ToDecimal(ims_sanctioned_projects.SURFACE_BRICK_SOLLING + ims_sanctioned_projects.SURFACE_BT + ims_sanctioned_projects.SURFACE_CC + ims_sanctioned_projects.SURFACE_GRAVEL + ims_sanctioned_projects.SURFACE_MOORUM + ims_sanctioned_projects.SURFACE_TRACK + ims_sanctioned_projects.SURFACE_WBM);

                if (ims_sanctioned_projects.IMS_PAV_LENGTH != TotalSurfaceLength)
                {
                    return Json(new { Success = false, ErrorMessage = "Sum of all the Existing Surface Lengths should be equal to the Pavement Length" });
                }
                if (ims_sanctioned_projects.SURFACE_BRICK_SOLLING >= 0 && ims_sanctioned_projects.SURFACE_BT >= 0 && ims_sanctioned_projects.SURFACE_CC >= 0 && ims_sanctioned_projects.SURFACE_GRAVEL >= 0 && ims_sanctioned_projects.SURFACE_MOORUM >= 0 && ims_sanctioned_projects.SURFACE_TRACK >= 0 && ims_sanctioned_projects.SURFACE_WBM >= 0)
                {
                    ModelState.Remove("MAST_EXISTING_SURFACE_CODE");
                }
                //}

                if (ModelState.IsValid)
                {
                    string route = objprDAL.getRoadRoute(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE));
                    if (route == "N" && ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "U" && PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        return Json(new { Success = false, ErrorMessage = "Please select New Connectivity for Missing Link Route" });
                    }
                    string Status = objprDAL.UpdateRoadProposalDALPMGSY5(ims_sanctioned_projects);

                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditProposalPMGSY3(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)");
                throw ex;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult ViewDetailsPMGSY5Proposal(string id)
        {
            try
            {
                String[] urlSplitParams = id.Split('$');
                Int32 IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);

                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);

                // Lock Status is depend on Is the proposal mord sanctioned or not?
                // In Case of Mord Unlocked Status IMS_LOCK_STATUS = "M"
                // Else IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS
                // Here if it is splitted from parameter - id, that means it is passed from GetProposalsDAL() function
                // Else get it from logic in db function -- UDF_IMS_UNLOCK_STATUS
                string IMS_LOCK_STATUS = string.Empty;

                if (urlSplitParams.Length > 1)
                {
                    IMS_LOCK_STATUS = urlSplitParams[1];
                }
                else
                {
                    if (ims_sanctioned_projects.IMS_SANCTIONED == "Y")
                    {
                        if (db.UDF_IMS_UNLOCK_STATUS(ims_sanctioned_projects.MAST_STATE_CODE, ims_sanctioned_projects.MAST_DISTRICT_CODE, ims_sanctioned_projects.MAST_BLOCK_CODE, 0, 0, ims_sanctioned_projects.IMS_PR_ROAD_CODE, 0, 0, "PR", ims_sanctioned_projects.MAST_PMGSY_SCHEME, (short)PMGSYSession.Current.RoleCode).Select(c => c.UNLOCK_COUNT).FirstOrDefault() > 0)
                        {
                            IMS_LOCK_STATUS = "M";
                        }
                        else
                        {
                            IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS;
                        }
                    }
                    else
                    {
                        IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS;
                    }
                }

                PMGSY.Models.Proposal.ProposalViewModel objProposal = new ProposalViewModel();
                if (ims_sanctioned_projects == null)
                {
                    return HttpNotFound();
                }

                objProposal.EXISTING_CARRIAGEWAY_WIDTH = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_WIDTH;
                objProposal.EXISTING_CARRIAGEWAY_PUC = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_PUC;

                objProposal.StateName = db.MASTER_STATE.Where(a => a.MAST_STATE_CODE == ims_sanctioned_projects.MAST_STATE_CODE).Select(a => a.MAST_STATE_NAME).First();
                objProposal.DistrictName = db.MASTER_DISTRICT.Where(a => a.MAST_DISTRICT_CODE == ims_sanctioned_projects.MAST_DISTRICT_CODE).Select(a => a.MAST_DISTRICT_NAME).First();
                ViewBag.IsTechnologyExist = db.IMS_PROPOSAL_TECH.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE) ? "Y" : "N";
                objProposal.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
                objProposal.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objProposal.MAST_BLOCK_NAME = ims_sanctioned_projects.MASTER_BLOCK.MAST_BLOCK_NAME;
                objProposal.MAST_STREAM_NAME = ims_sanctioned_projects.MASTER_STREAMS.MAST_STREAM_NAME;
                objProposal.IMS_STAGE_PHASE = ims_sanctioned_projects.IMS_STAGE_PHASE;
                objProposal.IMS_STAGED_YEAR = ims_sanctioned_projects.IMS_STAGED_YEAR;
                objProposal.IMS_STAGED_PACKAGE_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;
                objProposal.PUCCA_SIDE_DRAIN_LENGTH = ims_sanctioned_projects.PUCCA_SIDE_DRAIN_LENGTH;
                objProposal.PROTECTION_LENGTH = ims_sanctioned_projects.PROTECTION_LENGTH;

                objProposal.IMS_MAINTENANCE_YEAR6 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR6);
                objProposal.IMS_MAINTENANCE_YEAR7 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR7);
                objProposal.IMS_MAINTENANCE_YEAR8 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR8);
                objProposal.IMS_MAINTENANCE_YEAR9 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR9);
                objProposal.IMS_MAINTENANCE_YEAR10 = Convert.ToDecimal(ims_sanctioned_projects.IMS_MAINTENANCE_YEAR10);
                objProposal.TotalPostDLPMaintenanceCost = objProposal.IMS_MAINTENANCE_YEAR6 + objProposal.IMS_MAINTENANCE_YEAR7 + objProposal.IMS_MAINTENANCE_YEAR8 + objProposal.IMS_MAINTENANCE_YEAR9 + objProposal.IMS_MAINTENANCE_YEAR10;

                objProposal.SURFACE_BRICK_SOLLING = ims_sanctioned_projects.SURFACE_BRICK_SOLLING;
                objProposal.SURFACE_BT = ims_sanctioned_projects.SURFACE_BT;
                objProposal.SURFACE_CC = ims_sanctioned_projects.SURFACE_CC;
                objProposal.SURFACE_GRAVEL = ims_sanctioned_projects.SURFACE_GRAVEL;
                objProposal.SURFACE_MOORUM = ims_sanctioned_projects.SURFACE_MOORUM;
                objProposal.SURFACE_TRACK = ims_sanctioned_projects.SURFACE_TRACK;
                objProposal.SURFACE_WBM = ims_sanctioned_projects.SURFACE_WBM;

                if (ims_sanctioned_projects.PLAN_CN_ROAD_CODE != null)
                {
                    objProposal.PLAN_RD_NAME = ims_sanctioned_projects.PLAN_ROAD.PLAN_CN_ROAD_NUMBER == "O" ? ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME : (ims_sanctioned_projects.PLAN_ROAD.PLAN_CN_ROAD_NUMBER + "-" + ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME);
                }
                else
                {
                    objProposal.PLAN_RD_NAME = "--";
                }

                objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;
                if (ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "N")
                {
                    objProposal.MAST_EXISTING_SURFACE_NAME = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE == null ? "" : ims_sanctioned_projects.MASTER_SURFACE.MAST_SURFACE_NAME;
                    objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;
                    if (ims_sanctioned_projects.IMS_ISBENEFITTED_HABS == "N")
                    {
                        objProposal.HABS_REASON_TEXT = db.MASTER_REASON.Where(a => a.MAST_REASON_CODE == ims_sanctioned_projects.IMS_HABS_REASON).Select(a => a.MAST_REASON_NAME).FirstOrDefault();
                    }
                }

                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;
                objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
                objProposal.MAST_FUNDING_AGENCY_NAME = ims_sanctioned_projects.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_NAME;
                objProposal.MAST_STREAM_NAME = ims_sanctioned_projects.MASTER_STREAMS.MAST_STREAM_NAME;

                objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                objProposal.IMS_PARTIAL_LEN = ims_sanctioned_projects.IMS_PARTIAL_LEN.ToUpper() == "F" ? "Full Length" : "Partial Length";
                objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;
                objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                objProposal.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;
                objProposal.IMS_NO_OF_CDWORKS = ims_sanctioned_projects.IMS_NO_OF_CDWORKS;

                // All Costs
                objProposal.IMS_SANCTIONED_PAV_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;
                objProposal.IMS_SANCTIONED_CD_AMT = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;
                objProposal.IMS_SANCTIONED_PW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;
                objProposal.IMS_SANCTIONED_OW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;
                objProposal.IMS_SANCTIONED_RS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT;

                objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;

                objProposal.IMS_TOTAL_COST = Convert.ToString((PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ?
                                                          ((ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT))
                                                        : ((ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT) + (ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) + (ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS == null ? 0 : ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS)
                                                        ));
                objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;
                objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                objProposal.IMS_FURNITURE_COST = ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT;
                objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT;
                objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT;
                objProposal.IMS_SANCTIONED_FC_AMT = ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT;
                objProposal.IMS_SANCTIONED_RENEWAL_AMT = ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT ?? 0;

                if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 1)
                {
                    objProposal.IMS_SANCTIONED_AMOUNT = (Convert.ToDecimal(objProposal.IMS_TOTAL_COST) * 90) / 100;
                }
                else if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 2)
                {
                    objProposal.IMS_SANCTIONED_AMOUNT = (Convert.ToDecimal(objProposal.IMS_TOTAL_COST) * 75) / 100;
                }

                objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                objProposal.TotalMaintenanceCost = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2 +
                                                   ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4 +
                                                   ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5 +
                                                   Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT);

                objProposal.IMS_ZP_RESO_OBTAINED = ims_sanctioned_projects.IMS_ZP_RESO_OBTAINED;
                objProposal.MAST_MP_CONST_NAME = ims_sanctioned_projects.MASTER_MP_CONSTITUENCY == null ? "NA" : ims_sanctioned_projects.MASTER_MP_CONSTITUENCY.MAST_MP_CONST_NAME;
                objProposal.MAST_MLA_CONST_NAME = ims_sanctioned_projects.MASTER_MLA_CONSTITUENCY == null ? "NA" : ims_sanctioned_projects.MASTER_MLA_CONSTITUENCY.MAST_MLA_CONST_NAME;
                objProposal.IMS_CARRIAGED_WIDTH = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH;
                objProposal.Display_Carriaged_Width = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH != null ? db.MASTER_CARRIAGE.Where(a => a.MAST_CARRIAGE_CODE == ims_sanctioned_projects.IMS_CARRIAGED_WIDTH).Select(a => a.MAST_CARRIAGE_WIDTH).First().ToString() : "NA";

                if (ims_sanctioned_projects.MASTER_TRAFFIC_TYPE != null)
                {
                    objProposal.IMS_TRAFFIC_CATAGORY_NAME = ims_sanctioned_projects.MASTER_TRAFFIC_TYPE.MAST_TRAFFIC_NAME;
                }
                else
                {
                    objProposal.IMS_TRAFFIC_CATAGORY_NAME = "NA";
                }

                objProposal.IMS_PROPOSED_SURFACE = ims_sanctioned_projects.IMS_PROPOSED_SURFACE.ToUpper() == "S" ? "Sealed" : "UnSealed";
                objProposal.IMS_CC_LENGTH = ims_sanctioned_projects.IMS_CC_LENGTH;
                objProposal.IMS_BT_LENGTH = ims_sanctioned_projects.IMS_BT_LENGTH;
                objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;

                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS;
                objProposal.IMS_REASON = ims_sanctioned_projects.IMS_REASON;

                if (ims_sanctioned_projects.IMS_REASON != null)
                {
                    objProposal.Reason = db.MASTER_REASON.Where(a => a.MAST_REASON_CODE == ims_sanctioned_projects.IMS_REASON).Select(a => a.MAST_REASON_NAME).First();
                }

                objProposal.IMS_IS_STAGED = ims_sanctioned_projects.IMS_IS_STAGED;
                objProposal.IMS_ISCOMPLETED = ims_sanctioned_projects.IMS_ISCOMPLETED;

                //STA Details
                objProposal.STA_SANCTIONED = ims_sanctioned_projects.STA_SANCTIONED;
                objProposal.STA_SANCTIONED_BY = (ims_sanctioned_projects.STA_SANCTIONED_BY != null && ims_sanctioned_projects.STA_SANCTIONED_BY != "") ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == db.UM_User_Master.Where(a => a.UserName == ims_sanctioned_projects.STA_SANCTIONED_BY).Select(a => a.UserID).FirstOrDefault()).Any() ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == db.UM_User_Master.Where(a => a.UserName == ims_sanctioned_projects.STA_SANCTIONED_BY).Select(a => a.UserID).FirstOrDefault()).Select(b => b.ADMIN_TA_NAME).First() : (ims_sanctioned_projects.STA_SANCTIONED_BY == null ? "NA" : ims_sanctioned_projects.STA_SANCTIONED_BY.ToString()) : (ims_sanctioned_projects.STA_SANCTIONED_BY == null ? "NA" : ims_sanctioned_projects.STA_SANCTIONED_BY.ToString()); //change done by Vikram on 26/05/2014 as if no data found in Admin Technical Agency then show the STA Name as it is from IMS_SANCTIONED_PROJECTS

                if (ims_sanctioned_projects.STA_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.STA_SANCTIONED_DATE).Year != 0)
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_projects.STA_SANCTIONED_DATE);
                    objProposal.STA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                }
                objProposal.MS_STA_REMARKS = ims_sanctioned_projects.IMS_STA_REMARKS;

                //PTA Details
                objProposal.PTA_SANCTIONED = ims_sanctioned_projects.PTA_SANCTIONED;
                objProposal.PTA_SANCTIONED_BY = ims_sanctioned_projects.PTA_SANCTIONED_BY == null ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_CODE).FirstOrDefault() : ims_sanctioned_projects.PTA_SANCTIONED_BY;
                objProposal.NAME_OF_PTA = ims_sanctioned_projects.PTA_SANCTIONED_BY == null
                                                        ? db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_NAME).FirstOrDefault()
                                                        : db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_TA_CODE == ims_sanctioned_projects.PTA_SANCTIONED_BY).Select(a => a.ADMIN_TA_NAME).FirstOrDefault();

                if (ims_sanctioned_projects.PTA_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.PTA_SANCTIONED_DATE).Year != 0)
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_projects.PTA_SANCTIONED_DATE);
                    objProposal.PTA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                }

                objProposal.MS_PTA_REMARKS = ims_sanctioned_projects.IMS_PTA_REMARKS;

                objProposal.IMS_SANCTIONED = ims_sanctioned_projects.IMS_SANCTIONED;
                objProposal.IMS_SANCTIONED_BY = ims_sanctioned_projects.IMS_SANCTIONED_BY;
                if (ims_sanctioned_projects.IMS_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.IMS_SANCTIONED_DATE).Year != 0)
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_projects.IMS_SANCTIONED_DATE);
                    objProposal.IMS_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                }
                objProposal.IMS_PROG_REMARKS = ims_sanctioned_projects.IMS_PROG_REMARKS;

                //In Case of Mord Unlocked Status
                objProposal.IMS_LOCK_STATUS = IMS_LOCK_STATUS;
                objProposal.IMS_DPR_STATUS = ims_sanctioned_projects.IMS_DPR_STATUS; //new change done by Vikram to hide the finalize button if the Proposal is DPR.
                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_MORD_SHARE_2015.Value : 0;
                objProposal.IMS_TOTAL_COST_2015 = objProposal.IMS_STATE_SHARE_2015 + objProposal.IMS_MORD_SHARE_2015;
                objProposal.IMS_TOTAL_STATE_SHARE_2015 = PMGSYSession.Current.PMGSYScheme == 1 ? (decimal)(ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT + (ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0))
                                                                                               : (decimal)(/*ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT +*/ Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE_2015) + ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT);
                objProposal.ImsRidingQualityLength = ims_sanctioned_projects.IMS_RIDING_QUALITY_LENGTH.HasValue ? ims_sanctioned_projects.IMS_RIDING_QUALITY_LENGTH.Value : 0;
                objProposal.ImsPuccaSideDrains = ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS.HasValue ? ims_sanctioned_projects.IMS_PUCCA_SIDE_DRAINS.Value : 0;
                objProposal.ImsGSTCost = ims_sanctioned_projects.IMS_GST_COST.HasValue ? ims_sanctioned_projects.IMS_GST_COST.Value : 0;

                IMS_COST_COMPONENT imsCostComponent = ims_sanctioned_projects.IMS_COST_COMPONENT.Where(z => z.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_PR_ROAD_CODE).FirstOrDefault();

                if (imsCostComponent != null)
                {
                    objProposal.imsComponentId = imsCostComponent.IMS_COMPONENT_ID;
                    objProposal.ImsClearing = imsCostComponent.IMS_CLEARING;
                    objProposal.ImsExcavation = imsCostComponent.IMS_EXCAVATION;
                    objProposal.ImsFilling = imsCostComponent.IMS_FILLING;
                    objProposal.ImsSubGrade = imsCostComponent.IMS_SUB_GRADE;
                    objProposal.ImsShoulder = imsCostComponent.IMS_SHOULDER;
                    objProposal.ImsGranularSubBase = imsCostComponent.IMS_GRANULAR_SUB_BASE;
                    objProposal.ImsSoilAggregate = imsCostComponent.IMS_SOIL_AGGREGATE;
                    objProposal.ImsWBMGradeII = imsCostComponent.IMS_WBM_GRADE_II;
                    objProposal.ImsWBMGradeIII = imsCostComponent.IMS_WBM_GRADE_III;
                    objProposal.ImsWMM = imsCostComponent.IMS_WMM;
                    objProposal.ImsPrimeCoat = imsCostComponent.IMS_PRIME_COAT;
                    objProposal.ImsTackCoat = imsCostComponent.IMS_TACK_COAT;
                    objProposal.ImsBMDBM = imsCostComponent.IMS_BM_DBM;
                    objProposal.ImsOGPC_SDBC_BC = imsCostComponent.IMS_OGPC_SDBC_BC;
                    objProposal.ImsSealCoat = imsCostComponent.IMS_SEAL_COAT;
                    objProposal.ImsSurfaceDressing = imsCostComponent.IMS_SURFACE_DRESSING;
                    objProposal.ImsDryLeanConcrete = imsCostComponent.IMS_DRY_LEAN_CONCRETE;
                    objProposal.ImsConcretePavement = imsCostComponent.IMS_CONCRETE_PAVEMENT;
                }

                objProposal.encrProposalCode = URLEncrypt.EncryptParameters(new string[] { "IMS_PR_ROAD_CODE =" + objProposal.IMS_PR_ROAD_CODE.ToString() });
                return View(objProposal);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewDetailsPMGSY5Proposal");
                return null;
            }
        }

        #endregion

    }

    /// <summary>
    /// class for setting the usename and password for the Report Server
    /// </summary>
    /// 
    public class CustomReportCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
    {

        // local variable for network credential.
        private string _UserName;
        private string _PassWord;
        //private string _DomainName;
        public CustomReportCredentials(string UserName, string PassWord)//, string DomainName)
        {
            _UserName = UserName;
            _PassWord = PassWord;
            //_DomainName = DomainName;
        }
        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;  // not use ImpersonationUser
            }
        }
        public ICredentials NetworkCredentials
        {
            get
            {

                // use NetworkCredentials
                return new NetworkCredential(_UserName, _PassWord);//, _DomainName);
            }
        }
        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {

            // not use FormsCredentials unless you have implements a custom autentication.
            authCookie = null;
            user = password = authority = null;
            return false;
        }

    }

}