#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   LSBProposalController.cs        
        * Description   :   Action methods for Creating , Editing, Deleting LSB Propsoals and All the Data Related to Proposal
                            Including Other Parameters Like Habitation Details, CBR Details , Traffic Intensity and File Upload
        * Author        :   Shyam Yadav
        * Modified By   :   Shivkumar Deshmukh        
        * Creation Date :   20-05-2013
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
using PMGSY.DAL.Proposal;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    [Audit]
    public class LSBProposalController : Controller
    {
        public LSBProposalController()
        {
            PMGSYSession.Current.ModuleName = "Proposal";
        }
        private PMGSYEntities db = new PMGSYEntities();
        private LSBProposalBAL objLSBBAL = new LSBProposalBAL();

        #region LSB Creation Details

        /// <summary>
        ///  Screen : Listing Page of the Proposal
        /// Get the Proposals
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetLSBProposals(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
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
                rows = objLSBBAL.GetLSBProposalBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, out colTotal),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                //total = totalRecords,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
                TotalColumn = colTotal
            };
            return Json(jsonData);
        }


        /// <summary>
        /// LSB Proposals for State
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetLSBProposalsForSRRDA(FormCollection formCollection)
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
                rows = objLSBBAL.GetLSBProposalsForSRRDABAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, out colTotal),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
                TotalColumn = colTotal
            };
            return Json(jsonData);
        }


        /// <summary>
        /// Create Proposal for LSB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult CreateLSB(string id)
        {
            PMGSY.Models.Proposal.LSBViewModel proposalViewModel = new LSBViewModel();
            ViewBag.operation = "C";
            proposalViewModel.StateCodeForComparision = PMGSYSession.Current.StateCode;
            //proposalViewModel.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_SHORT_CODE).FirstOrDefault();

            //package prefix is MAST_STATE_SHORT_CODE + MAST_DISTRICT_ID
            proposalViewModel.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_SHORT_CODE).FirstOrDefault() +
                                               (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_DISTRICT_ID).FirstOrDefault().ToString("D2");

            proposalViewModel.IMS_EXISTING_PACKAGE = "N";
            proposalViewModel.isExistingRoad = "U";
            proposalViewModel.IMS_YEAR = DateTime.Now.Year;

            proposalViewModel.IMS_SANCTIONED_MAN_AMT1 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT2 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT3 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT4 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT5 = 0;

            //For PMGSY Scheme-2
            if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 4)//PMGSY3
            {
                proposalViewModel.IMS_IS_HIGHER_SPECIFICATION = "N";
                proposalViewModel.IMS_SHARE_PERCENT = 2;
                proposalViewModel.IMS_HIGHER_SPECIFICATION_COST = 0;
                proposalViewModel.IMS_RENEWAL_COST = 0;
            }




            if (id != "" && id != null)
            {
                string[] defaultValues = id.Split('$');
                if (defaultValues[0] != "" && defaultValues[0] != null)
                {
                    proposalViewModel.IMS_YEAR = Convert.ToInt32(defaultValues[0]);
                }

                //Purposely commented, because core network population depends on change of block
                // if it is set then nneds to populate respective Core Network
                //if (defaultValues[1] != "" && defaultValues[1] != null)
                //{
                //    proposalViewModel.MAST_BLOCK_CODE = Convert.ToInt32(defaultValues[1]);
                //}

                if (defaultValues[2] != "" && defaultValues[2] != null)
                {
                    proposalViewModel.IMS_BATCH = Convert.ToInt32(defaultValues[2]);
                }

                if (defaultValues[3] != "" && defaultValues[3] != null)
                {
                    proposalViewModel.IMS_COLLABORATION = Convert.ToInt32(defaultValues[3]);
                }

            }


            proposalViewModel.Years = new ProposalController().PopulateYear();

            CommonFunctions objCommonFuntion = new CommonFunctions();
            proposalViewModel.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, proposalViewModel.IMS_YEAR, false, PMGSYSession.Current.PMGSYScheme == 1 ? true : false);

            //proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency();

            ///Changed by SAMMED A. PATIL for RCPLWE
            proposalViewModel.COLLABORATIONS = new List<SelectListItem>();//objCommonFuntion.PopulateFundingAgency();
            proposalViewModel.COLLABORATIONS.Insert(0, new SelectListItem { Text = "Select Funding Agency", Value = "-1", Selected = true });

            proposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("P");
            proposalViewModel.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";

            proposalViewModel.CN_ROADS = new ProposalController().PopulateLinkThrough(0, "", "L");

            proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode);
            proposalViewModel.Existing_Roads_LSB = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();
            ViewBag.Stage_Year = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            ViewBag.Stage_Package = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            //GetPackageId(2013, 0);
            //ViewBag.EXISTING_IMS_PACKAGE_ID = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            proposalViewModel.PACKAGES = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();

            #region CODE_ADDED_BY_VIKRAM_FOR_CHANGES_IN_PMGSY_SCHEME_1_COST

            proposalViewModel.IsProposalFinanciallyClosed = false;
            int shareCode = db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault();
            ViewBag.shareCode = shareCode;
            proposalViewModel.IMS_SHARE_PERCENT_2015 = shareCode == 0 ? (byte)3 : (byte)shareCode;

            #endregion

            return View(proposalViewModel);
        }

        /// <summary>
        /// Post to create LSB Proposal
        /// </summary>
        /// <param name="ims_sanctioned_projects"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult CreateLSB(PMGSY.Models.Proposal.LSBViewModel ims_sanctioned_projects)
        {
            ProposalDAL objprDAL = new ProposalDAL();
            ViewBag.operation = "C";
            try
            {

                if (ModelState.IsValid)
                {
                    string route = objprDAL.getRoadRoute(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE));
                    if (route == "N" && ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "U" && PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        return Json(new { Success = false, ErrorMessage = "Please select New Connectivity for Missing Link Route" });
                    }
                    string Status = objLSBBAL.SaveLSBProposalBAL(ims_sanctioned_projects);
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
                ErrorLog.LogError(ex, "LSBProposal.CreateLSB");
                return Json(new { Success = false, ErrorMessage = "Error occured while Adding LSB Proposal" });
            }
        }

        /// <summary>
        /// ROad list of Proposed sanctioned roads for LSB
        /// </summary>
        /// <param name="BlockID"></param>
        /// <param name="Year"></param>
        /// <param name="PackageID"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetStagedRoadList(int BlockID, int StageYear, string PackageID, int Year)
        {
            if (BlockID == 0)
                return Json(String.Empty);
            else
                return Json(GetStagedRoads(BlockID, StageYear, PackageID, Year));
        }

        /// <summary>
        /// Get List<SelectListIem> for Existing Roads
        /// </summary>
        /// <param name="BlockID"></param>
        /// <param name="Year"></param>
        /// <param name="PackageID"></param>
        /// <returns></returns>
        [Audit]
        public List<SelectListItem> GetStagedRoads(int BlockID, int StageYear, string PackageID, int Year)
        {
            List<SelectListItem> lstRoads = new List<SelectListItem>();
           // DateTime dt = new CommonFunctions().GetStringToDateTime("28/04/2011");
            DateTime dt = new CommonFunctions().GetStringToDateTime("01/01/2001"); // 02-17-2021 02:46 PM  Srinivasa Sir mail dated  Re: Request to facil​itate proposal entry​ in OMMAS for PMGSY ​II​

            try
            {
                SelectListItem item = new SelectListItem();
                if (BlockID == 0)
                {
                    return lstRoads;
                }
                else
                {
                    if (PMGSYSession.Current.StateCode == 22 || PMGSYSession.Current.StateCode == 30)
                    {
                         var query = (from c in db.IMS_SANCTIONED_PROJECTS
                                     where c.IMS_YEAR == StageYear &&
                                         c.IMS_PACKAGE_ID == PackageID &&
                                         c.IMS_DPR_STATUS == "N" 
                                         //&&


                                        // (

                                        // (StageYear == Year)

                                        // ? ("%" == "%")

                                        // : c.IMS_SANCTIONED == "Y"

                                        // && 
                                        //// (
                                        // (c.MAST_STATE_CODE == 22 || c.MAST_STATE_CODE == 30 )
                                        //// ? (1 == 1)
                                        // //: (c.IMS_SANCTIONED_DATE <= dt || c.IMS_SANCTIONED_DATE == null)
                                        // //)

                                        // &&
                                        // (c.IMS_STAGE_PHASE == "S2" || c.IMS_IS_STAGED == "C" || c.IMS_IS_STAGED == null) // Srinivasa Sir told : Conside rIMS_IS_STAGED NULL as Completed 
                                        // )

                                         
                                         && c.IMS_PROPOSAL_TYPE == "P"

                                         && c.MAST_BLOCK_CODE == BlockID
                                        
                                         && c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme

                                         && (c.IMS_SANCTIONED_DATE >= dt || c.IMS_SANCTIONED_DATE == null)  //c.IMS_YEAR>2011 // Srinivasa Sir to asked to For Sikkim and Manipur , take Sanction Year greater than 2011
                                     select new
                                     {
                                         Value = c.IMS_PR_ROAD_CODE,
                                         Text = c.IMS_ROAD_NAME
                                     }).ToList();
                         foreach (var data in query)
                         {
                             item = new SelectListItem();
                             item.Text = data.Text;
                             item.Value = data.Value.ToString();


                             lstRoads.Add(item);
                         }
                         return lstRoads;
                    }
                    else
                    {

                        var query = (from c in db.IMS_SANCTIONED_PROJECTS
                                     where c.IMS_YEAR == StageYear &&
                                         c.IMS_PACKAGE_ID == PackageID &&
                                         c.IMS_DPR_STATUS == "N"
                                         //&&


                                         //(

                                         //(StageYear == Year)

                                         //? ("%" == "%")

                                         //: c.IMS_SANCTIONED == "Y"

                                         //&& (
                                         //(c.MAST_STATE_CODE == 2 || c.MAST_STATE_CODE == 30 || c.MAST_STATE_CODE == 17 || c.MAST_STATE_CODE == 20 || c.MAST_STATE_CODE == 25 || c.MAST_STATE_CODE == 14 || c.MAST_STATE_CODE == 12 || c.MAST_STATE_CODE == 35 || c.MAST_STATE_CODE == 34 || c.MAST_STATE_CODE == 5)
                                         //? (1 == 1)
                                         //: (c.IMS_SANCTIONED_DATE >= dt || c.IMS_SANCTIONED_DATE == null)  // Srinivasa Sir told : IMS_SANCTIONED_DATE can be greater than dt
                                         //)

                                         //&&
                                         //(c.IMS_STAGE_PHASE == "S2" || c.IMS_IS_STAGED == "C" || c.IMS_IS_STAGED == null) // Srinivasa Sir told : Conside rIMS_IS_STAGED NULL as Completed 
                                         //)

                                         &&
                                         c.IMS_PROPOSAL_TYPE == "P" &&
                                         c.MAST_BLOCK_CODE == BlockID
                                         && c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                                     select new
                                     {
                                         Value = c.IMS_PR_ROAD_CODE,
                                         Text = c.IMS_ROAD_NAME
                                     }).ToList();

                        foreach (var data in query)
                        {
                            item = new SelectListItem();
                            item.Text = data.Text;
                            item.Value = data.Value.ToString();


                            lstRoads.Add(item);
                        }
                        return lstRoads;
                    }



                   
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Year List for LSB
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult GetStagedYearList()
        {
            try
            {
                SelectListItem item = new SelectListItem();
                return Json(new ProposalController().PopulateYear(0, false, false));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Populate Packages for LSB
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="BatchID"></param>
        /// <returns></returns>
        [Audit]
        public List<SelectListItem> GetPackageIDLSB(int Year, int BatchID)
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
                    //Populate only packages that are staged and had stage 2 completed.
                    var query = (from c in db.IMS_SANCTIONED_PROJECTS
                                 where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode
                                 && c.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode
                                 && c.IMS_YEAR == Year
                                     //&& ((c.IMS_IS_STAGED.ToUpper().Equals("S") && c.IMS_STAGE_PHASE.ToUpper().Equals("S2")) || (c.IMS_IS_STAGED.ToUpper().Equals("C")))
                                     //above code commented by Vikram as suggested by Shyam sir.
                                     //&& (c.IMS_IS_STAGED.ToUpper().Equals("S"))
                                 && c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                                     ///Added by SAMMED A. PATIL for RCPLWE
                                 && c.IMS_COLLABORATION != 5
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
                        //if (selectedPackage.Equals(item.Value))
                        //{
                        //    item.Selected = true;
                        //}
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
                                     c.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode &&   //condition added by shyam on 26 Sep 2013
                                     c.IMS_YEAR == Year &&
                                     c.IMS_BATCH == BatchID &&
                                     c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Json to populate Package IDs for LSB
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="BatchID"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetPackageIDForLSB(int Year, int BatchID)
        {
            if (Year == 0)
                return Json(String.Empty);
            else
                return Json(GetPackageIDLSB(Year, BatchID));
        }


        #region RCPLWE
        /// <summary>
        /// Json to populate Package IDs for LSB
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="BatchID"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetPackageIDForLSBRCPLWE(int Year, int BatchID)
        {
            if (Year == 0)
                return Json(String.Empty);
            else
                return Json(GetPackageIDLSBRCPLWE(Year, BatchID));
        }

        /// <summary>
        /// Populate Packages for LSB
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="BatchID"></param>
        /// <returns></returns>
        [Audit]
        public List<SelectListItem> GetPackageIDLSBRCPLWE(int Year, int BatchID)
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
                    //Populate only packages that are staged and had stage 2 completed.
                    var query = (from c in db.IMS_SANCTIONED_PROJECTS
                                 where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode
                                 && c.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode
                                 && c.IMS_YEAR == Year
                                     //&& ((c.IMS_IS_STAGED.ToUpper().Equals("S") && c.IMS_STAGE_PHASE.ToUpper().Equals("S2")) || (c.IMS_IS_STAGED.ToUpper().Equals("C")))
                                     //above code commented by Vikram as suggested by Shyam sir.
                                     //&& (c.IMS_IS_STAGED.ToUpper().Equals("S"))
                                 && c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme//(PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme)///Changes for RCPLWE
                                     ///Added by SAMMED A. PATIL for RCPLWE
                                 && c.IMS_COLLABORATION == 5
                                 && c.IMS_PROPOSAL_TYPE == "P"
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
                        //if (selectedPackage.Equals(item.Value))
                        //{
                        //    item.Selected = true;
                        //}
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
                                     c.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode &&   //condition added by shyam on 26 Sep 2013
                                     c.IMS_YEAR == Year &&
                                     c.IMS_BATCH == BatchID &&
                                     c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion
        /// <summary>
        /// Details of LSB Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult LSBDetails(string id)
        {
            PMGSY.Models.Proposal.LSBViewModel objProposal = new LSBViewModel();
            try
            {

                String[] urlSplitParams = id.Split('$');
                Int32 IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);

                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);
                if (ims_sanctioned_projects == null)
                {
                    return HttpNotFound();
                }

                // Lock Status is depend on Is the proposal mord sanctioned or not?
                // In Case of Mord Unlocked Status IMS_LOCK_STATUS = "M"
                // Else IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS
                // Here if it is splitted from parameter - id, that means it is passed from GetLSBProposalsDAL() function
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
                        if (db.UDF_IMS_UNLOCK_STATUS(ims_sanctioned_projects.MAST_STATE_CODE, ims_sanctioned_projects.MAST_DISTRICT_CODE, ims_sanctioned_projects.MAST_BLOCK_CODE, 0, 0, ims_sanctioned_projects.IMS_PR_ROAD_CODE, ims_sanctioned_projects.IMS_BATCH, ims_sanctioned_projects.IMS_YEAR, "PR", ims_sanctioned_projects.MAST_PMGSY_SCHEME, (short)PMGSYSession.Current.RoleCode).Select(c => c.UNLOCK_COUNT).FirstOrDefault() > 0)
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





                objProposal.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
                objProposal.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objProposal.MAST_BLOCK_NAME = ims_sanctioned_projects.MASTER_BLOCK.MAST_BLOCK_NAME;
                objProposal.MAST_STREAM_NAME = ims_sanctioned_projects.MASTER_STREAMS.MAST_STREAM_NAME;
                objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;
                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
                objProposal.MAST_FUNDING_AGENCY_NAME = ims_sanctioned_projects.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_NAME;
                objProposal.MAST_STREAM_NAME = ims_sanctioned_projects.MASTER_STREAMS.MAST_STREAM_NAME;
                objProposal.IMS_STAGED_YEAR = ims_sanctioned_projects.IMS_STAGED_YEAR;
                objProposal.IMS_STAGED_PACKAGE_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;
                objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;        //Road Name
                objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;
                objProposal.CoreNetworkNumber = ims_sanctioned_projects.PLAN_CN_ROAD_CODE == null ? "-" : ims_sanctioned_projects.PLAN_ROAD.PLAN_CN_ROAD_NUMBER;
                if (objProposal.IMS_UPGRADE_CONNECT.Equals("U"))
                {
                    objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                }
                else if (objProposal.IMS_UPGRADE_CONNECT.Equals("N"))
                {
                    objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.PLAN_CN_ROAD_CODE == null ? "-" : ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME;
                }


                objProposal.IMS_BRIDGE_NAME = ims_sanctioned_projects.IMS_BRIDGE_NAME;
                objProposal.IMS_BRIDGE_LENGTH = ims_sanctioned_projects.IMS_BRIDGE_LENGTH;
                objProposal.IMS_BRIDGE_EST_COST_STATE = Convert.ToDecimal(ims_sanctioned_projects.IMS_BRIDGE_EST_COST_STATE);
                objProposal.IMS_BRIDGE_WORKS_EST_COST = Convert.ToDecimal(ims_sanctioned_projects.IMS_BRIDGE_WORKS_EST_COST);

                objProposal.IMS_SANCTIONED_BS_AMT = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT);
                objProposal.IMS_SANCTIONED_BW_AMT = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT);

                //For PMGSY Scheme-2
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
                    objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                    objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT;
                    objProposal.TotalCostWithHigherSpecCost = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) +
                                                              Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT) +
                                                              Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT);
                }


                // Added on 17 Feb 2020
                if (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5) // Added by Srishti
                {
                    objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT;
                }



                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS == null ? "NA" : ims_sanctioned_projects.IMS_REMARKS;

                objProposal.IMS_ISCOMPLETED = ims_sanctioned_projects.IMS_ISCOMPLETED;
                if ((db.IMS_LSB_BRIDGE_COMPONENT_DETAIL.Where(c => c.IMS_PR_ROAD_CODE == objProposal.IMS_PR_ROAD_CODE).Count() == 0)
                    || (db.IMS_LSB_BRIDGE_DETAIL.Where(c => c.IMS_PR_ROAD_CODE == objProposal.IMS_PR_ROAD_CODE).Count() == 0))
                {
                    objProposal.isAllDetailsEntered = false;        //either of data is not entered
                }
                else
                {
                    objProposal.isAllDetailsEntered = true;
                }
                //STA Scrutinized Details //----------------------------------------------------
                objProposal.STA_SANCTIONED = ims_sanctioned_projects.STA_SANCTIONED;

                objProposal.STA_SANCTIONED_BY = (ims_sanctioned_projects.STA_SANCTIONED_BY != null && ims_sanctioned_projects.STA_SANCTIONED_BY != "") ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == db.UM_User_Master.Where(a => a.UserName == ims_sanctioned_projects.STA_SANCTIONED_BY).Select(a => a.UserID).FirstOrDefault()).Any() ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == db.UM_User_Master.Where(a => a.UserName == ims_sanctioned_projects.STA_SANCTIONED_BY).Select(a => a.UserID).FirstOrDefault()).Select(b => b.ADMIN_TA_NAME).First() : "" : "";

                if (ims_sanctioned_projects.STA_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.STA_SANCTIONED_DATE).Year != 0)
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_projects.STA_SANCTIONED_DATE);
                    objProposal.STA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                }
                if (ims_sanctioned_projects.IMS_STA_REMARKS != null)
                    objProposal.MS_STA_REMARKS = ims_sanctioned_projects.IMS_STA_REMARKS.Trim();
                //-------------------------------------------------------------------------------


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

                //In Case of Mord Unlocked Status
                objProposal.IMS_LOCK_STATUS = IMS_LOCK_STATUS;


                objProposal.IMS_EXISTING_PACKAGE = ims_sanctioned_projects.IMS_EXISTING_PACKAGE;

                objProposal.TotalEstimatedCost = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT);


                // Added on 17 Feb 2020
                if (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5)  // Added by Srishti
                {

                    if (ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT != null)
                    {
                        objProposal.TotalEstimatedCost = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT);
                    }

                }

                if (ims_sanctioned_projects.IMS_STATE_SHARE_2015 == null)
                {
                    ims_sanctioned_projects.IMS_STATE_SHARE_2015 = 0;
                }
                if (ims_sanctioned_projects.IMS_MORD_SHARE_2015 == null)
                {
                    ims_sanctioned_projects.IMS_MORD_SHARE_2015 = 0;
                }

                if (ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT == null)
                {
                    ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT = 0;
                }
                if (ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT == null)
                {
                    ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT = 0;
                }

                if (ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT == null)
                {
                    ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT = 0;
                }

                //Total Cost(Excluding Higher Specification Rs. in Lakhs)
                objProposal.PMGSYIII_TotalCost_ExcludingHigherSpecificationRsinLakhs = ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT + ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT;


                //Total State Share Cost (Rs. in Lakhs) -- Same columns are used in SP TOTAL_STATE_SHARE=(ISNULL(IMS_SANCTIONED_HS_AMT,0)+ ISNULL(sp.IMS_STATE_SHARE_2015,0)) 
                objProposal.PMGSYIII_TotalStateShareCost = ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT + ims_sanctioned_projects.IMS_STATE_SHARE_2015;



                objProposal.IMS_REASON = ims_sanctioned_projects.IMS_REASON;
                if (ims_sanctioned_projects.IMS_REASON != null)
                {
                    objProposal.Reason = db.MASTER_REASON.Where(a => a.MAST_REASON_CODE == ims_sanctioned_projects.IMS_REASON).Select(a => a.MAST_REASON_NAME).First();
                }

                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_MORD_SHARE_2015.Value : 0;
                objProposal.IMS_TOTAL_COST_2015 = objProposal.IMS_STATE_SHARE_2015 + objProposal.IMS_MORD_SHARE_2015;
                //objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null ? 4 : ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                objProposal.IMS_TOTAL_STATE_SHARE_2015 = PMGSYSession.Current.PMGSYScheme == 2 ? objProposal.IMS_STATE_SHARE_2015 + Convert.ToDecimal(objProposal.IMS_HIGHER_SPECIFICATION_COST) : objProposal.IMS_STATE_SHARE_2015 + Convert.ToDecimal(objProposal.IMS_BRIDGE_EST_COST_STATE);
                //objProposal.IMS_SHARE_PERCENT_2015 = 4;

                return View(objProposal);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }

        /// <summary>
        /// Get to Update LSB Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditLSB(int id = 0)
        {
            ViewBag.operation = "U";
            PMGSY.DAL.Proposal.ProposalDAL objProposalDAL = new DAL.Proposal.ProposalDAL();

            IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(id);
            try
            {
                if (ims_sanctioned_projects == null)
                {
                    return HttpNotFound();
                }

                PMGSY.Models.Proposal.LSBViewModel objProposal = new LSBViewModel();
                objProposal.StateCodeForComparision = PMGSYSession.Current.StateCode;
                objProposal.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
                objProposal.MAST_STATE_CODE = ims_sanctioned_projects.MAST_STATE_CODE;
                objProposal.MAST_DISTRICT_CODE = ims_sanctioned_projects.MAST_DISTRICT_CODE;
                objProposal.MAST_DPIU_CODE = ims_sanctioned_projects.MAST_DPIU_CODE;
                objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;
                objProposal.isExistingRoad = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;
                objProposal.IMS_EXISTING_PACKAGE = "E";

                //For PMGSY Scheme-2
                if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5)//PMGSY3
                {
                    objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
                    objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                    objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST == null ? 0 : ims_sanctioned_projects.IMS_RENEWAL_COST.Value;
                    objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT;
                    objProposal.TotalCostWithHigherSpecCost = Convert.ToDecimal(ims_sanctioned_projects.IMS_BRIDGE_EST_COST_STATE) +
                                                              Convert.ToDecimal(ims_sanctioned_projects.IMS_BRIDGE_WORKS_EST_COST) +
                                                              Convert.ToDecimal(ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST == null ? 0 : ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST);
                }

                //package prefix is MAST_STATE_SHORT_CODE + MAST_DISTRICT_ID
                objProposal.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_SHORT_CODE).FirstOrDefault() +
                                                   (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_DISTRICT_ID).FirstOrDefault();

                CommonFunctions objCommonFuntion = new CommonFunctions();
                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.Years = new ProposalController().PopulateYear(Convert.ToInt32(ims_sanctioned_projects.IMS_YEAR), true, false);

                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                //objProposal.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, objProposal.IMS_YEAR, false, PMGSYSession.Current.PMGSYScheme == 1 ? true : false);

                objProposal.BATCHS = new List<SelectListItem>();
                objProposal.BATCHS.Insert(0, new SelectListItem() { Text = "Batch " + Convert.ToString(objProposal.IMS_BATCH), Value = Convert.ToString(objProposal.IMS_BATCH) });

                objProposal.IMS_COLLABORATION = ims_sanctioned_projects.IMS_COLLABORATION;
                objProposal.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency();

                // objProposal.isPaymentDone = checkIsPayment(ims_sanctioned_projects.IMS_PR_ROAD_CODE);
                //if (!objProposal.isPaymentDone)
                //{
                //    objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                //    objProposal.STREAMS = objCommonFuntion.PopulateStreams("P");
                //    objProposal.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
                //}

                objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                objProposal.STREAMS = objCommonFuntion.PopulateStreams("P");
                objProposal.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";






                objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;
                objProposal.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode);

                objProposal.IMS_STAGED_YEAR = ims_sanctioned_projects.IMS_STAGED_YEAR;
                objProposal.IMS_STAGED_PACKAGE_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;
                objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;
                objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;
                objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;

                objProposal.IMS_BRIDGE_NAME = ims_sanctioned_projects.IMS_BRIDGE_NAME;
                objProposal.IMS_BRIDGE_LENGTH = ims_sanctioned_projects.IMS_BRIDGE_LENGTH;
                objProposal.IMS_BRIDGE_EST_COST_STATE = Convert.ToDecimal(ims_sanctioned_projects.IMS_BRIDGE_EST_COST_STATE);
                objProposal.IMS_BRIDGE_WORKS_EST_COST = Convert.ToDecimal(ims_sanctioned_projects.IMS_BRIDGE_WORKS_EST_COST);
                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS == null ? "" : ims_sanctioned_projects.IMS_REMARKS;

                objProposal.EXISTING_IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;

                objProposal.PACKAGES = GetPackageIDLSB(ims_sanctioned_projects.IMS_YEAR, ims_sanctioned_projects.IMS_BATCH);
                //objProposal.PACKAGES = GetPackageIDLSB(ims_sanctioned_projects.IMS_YEAR, ims_sanctioned_projects.IMS_BATCH, ims_sanctioned_projects.IMS_COLLABORATION.Value);

                objProposal.CN_ROADS = new ProposalController().PopulateLinkThrough(ims_sanctioned_projects.MAST_BLOCK_CODE, ims_sanctioned_projects.IMS_UPGRADE_CONNECT, "L");
                //objProposal.CN_ROADS = GetStagedRoads(ims_sanctioned_projects.MAST_BLOCK_CODE, objProposal.IMS_STAGED_YEAR.Value, objProposal.IMS_STAGED_PACKAGE_ID, objProposal.IMS_YEAR);
                ViewBag.Stage_Year = new ProposalController().PopulateYear(Convert.ToInt32(ims_sanctioned_projects.IMS_STAGED_YEAR), true, false);

                if (objProposal.IMS_COLLABORATION == 5)
                {
                    ViewBag.Stage_Package = GetPackageIDLSBRCPLWE(Convert.ToInt32(ims_sanctioned_projects.IMS_STAGED_YEAR), 0);
                }
                else
                {
                    ViewBag.Stage_Package = GetPackageIDLSB(Convert.ToInt32(ims_sanctioned_projects.IMS_STAGED_YEAR), 0);
                }



                objProposal.Existing_Roads_LSB = GetStagedRoads(ims_sanctioned_projects.MAST_BLOCK_CODE, Convert.ToInt32(ims_sanctioned_projects.IMS_STAGED_YEAR), ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID, ims_sanctioned_projects.IMS_YEAR);    // change done by Vikram as suggested by Dev sir -- if proposal year and stage year is same then skip IMS_SANCTIONED = Y
                if (objProposal.Existing_Roads_LSB == null)
                {
                    objProposal.Existing_Roads_LSB = new List<SelectListItem>();
                    objProposal.Existing_Roads_LSB.Insert(0, new SelectListItem { Text = "Select Road", Value = "" });
                }
                objProposal.TotalEstimatedCost = objProposal.IMS_BRIDGE_WORKS_EST_COST + objProposal.IMS_BRIDGE_EST_COST_STATE;

                objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                objProposal.TotalMaintenanceCost = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2 +
                                                   ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4 +
                                                   ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5 +    //In case of PMGSY Scheme-II include IMS_SANCTIONED_RENEWAL_AMT
                                                   ((PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 4)//PMGSY3
                                                    ? Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT)
                                                    : 0.0M);


                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_MORD_SHARE_2015.Value : 0;
                objProposal.IMS_TOTAL_COST_2015 = objProposal.IMS_STATE_SHARE_2015 + objProposal.IMS_MORD_SHARE_2015;
                //objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null ? 4 : ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;

                //objProposal.IMS_TOTAL_STATE_SHARE_2015 = objProposal.IMS_STATE_SHARE_2015 +objProposal.IMS_BRIDGE_EST_COST_STATE;
                objProposal.IMS_TOTAL_STATE_SHARE_2015 = PMGSYSession.Current.PMGSYScheme == 1 ? (decimal)(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT + (ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0))
                                                                                           : (decimal)(/*ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT +*/ Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE_2015));
                if (objProposalDAL.IsProposalFinanciallyClosed(ims_sanctioned_projects.IMS_PR_ROAD_CODE) && ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null)
                {
                    objProposal.IMS_SHARE_PERCENT_2015 = 4;
                }
                else
                {
                    objProposal.IMS_SHARE_PERCENT_2015 = (ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null || ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == 0) ? ((byte)(db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault() == 0 ? 3 : db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault())) : ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                }

                ViewBag.shareCode = objProposal.IMS_SHARE_PERCENT_2015;

                return View("CreateLSB", objProposal);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }

        /// <summary>
        /// Post to update LSB details
        /// </summary>
        /// <param name="ims_sanctioned_projects"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult EditLSB(PMGSY.Models.Proposal.LSBViewModel ims_sanctioned_projects)
        {
            ProposalDAL objprDAL = new ProposalDAL();
            ViewBag.Operation = "U";
            try
            {
                if (ims_sanctioned_projects.isPaymentDone)
                {
                    ModelState.Remove("IMS_STREAMS");
                }
                // Added by Srishti
                if (PMGSYSession.Current.PMGSYScheme == 5)
                {
                    ModelState.Remove("PLAN_CN_ROAD_CODE");
                }
                if (ModelState.IsValid)
                {
                    string route = objprDAL.getRoadRoute(Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE));
                    if (route == "N" && ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "U" && PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        return Json(new { Success = false, ErrorMessage = "Please select New Connectivity for Missing Link Route" });
                    }
                    string Status = objLSBBAL.UpdateLSBProposalBAL(ims_sanctioned_projects);

                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    StringBuilder errorMessages = new StringBuilder();
                    foreach (var modelStateValue in ModelState.Values)
                    {
                        foreach (var error in modelStateValue.Errors)
                        {
                            errorMessages.Append(error.ErrorMessage);
                        }
                    }
                    return Json(new { Success = false, ErrorMessage = errorMessages.ToString() });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Post for Delete LSB Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult DeleteLSBConfirmed(int id)
        {
            string status = string.Empty;
            try
            {
                status = objLSBBAL.DeleteLSBProposalBAL(id);
                if (status == string.Empty)
                {
                    return Json(new { success = "true" });
                }
                else
                {
                    return Json(new { success = "false", errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("IMS_PR_ROAD_CODE :" + id);
                    sw.WriteLine("======================================================================");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DeleteLSBConfirmed()");
                    if (ex != null)
                        sw.WriteLine("Exception : " + ex.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                    if (ex.InnerException.InnerException != null)
                        sw.WriteLine("ex.InnerException.InnerException : " + ex.InnerException.InnerException.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return Json(new { success = "false", errorMessage = status });
            }
        }

        #endregion

        #region LSB Other Details

        /// <summary>
        /// Screen : Add Estimated Cost details to the Proposal
        /// Get Method to Add Cost Details to the Proposal 
        /// </summary>
        /// <param name="id1"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult LSBOtherDetails(String id)
        {
            LSBOthDetailsModel lsbOthDetails = new LSBOthDetailsModel();
            try
            {
                //Check is bridge details already entered?
                IMS_LSB_BRIDGE_DETAIL lsb_bridge_detail = db.IMS_LSB_BRIDGE_DETAIL.Find(Convert.ToInt32(id));

                //Edit Mode
                if (lsb_bridge_detail != null)
                {
                    lsbOthDetails.OPERATION = "U";
                    lsbOthDetails.IS_UPDATED = true;
                    CommonFunctions objCommonFuntion = new CommonFunctions();
                    lsbOthDetails.IMS_PR_ROAD_CODE = lsb_bridge_detail.IMS_PR_ROAD_CODE;

                    //Assign all properties
                    lsbOthDetails.IMS_ROAD_TYPE_LEVEL = lsb_bridge_detail.IMS_ROAD_TYPE_LEVEL;
                    lsbOthDetails.IMS_AVERAGE_GROUND_LEVEL = lsb_bridge_detail.IMS_AVERAGE_GROUND_LEVEL;
                    lsbOthDetails.IMS_NALA_BED_LEVEL = lsb_bridge_detail.IMS_NALA_BED_LEVEL;
                    lsbOthDetails.IMS_HIGHEST_FLOOD_LEVEL = lsb_bridge_detail.IMS_HIGHEST_FLOOD_LEVEL;
                    lsbOthDetails.IMS_ORDINARY_FLOOD_LEVEL = lsb_bridge_detail.IMS_ORDINARY_FLOOD_LEVEL;
                    lsbOthDetails.IMS_FOUNDATION_LEVEL = lsb_bridge_detail.IMS_FOUNDATION_LEVEL;
                    lsbOthDetails.IMS_HGT_BIRDGE_NBL = lsb_bridge_detail.IMS_HGT_BIRDGE_NBL;
                    lsbOthDetails.IMS_HGT_BRIDGE_FL = lsb_bridge_detail.IMS_HGT_BRIDGE_FL;
                    lsbOthDetails.IMS_BRG_SUBMERSIBLE = Convert.ToBoolean(lsb_bridge_detail.IMS_BRG_SUBMERSIBLE);
                    lsbOthDetails.IMS_BRG_BOX_CULVERT = Convert.ToBoolean(lsb_bridge_detail.IMS_BRG_BOX_CULVERT);
                    lsbOthDetails.IMS_BRG_RCC_ABUMENT = Convert.ToBoolean(lsb_bridge_detail.IMS_BRG_RCC_ABUMENT);
                    lsbOthDetails.IMS_BRG_HLB = Convert.ToBoolean(lsb_bridge_detail.IMS_BRG_HLB);
                    lsbOthDetails.IMS_SC_FD_CODE = lsb_bridge_detail.IMS_SC_FD_CODE;
                    lsbOthDetails.IMS_BEARING_CAPACITY = lsb_bridge_detail.IMS_BEARING_CAPACITY;
                    lsbOthDetails.IMS_ARG_TOT_SPANS = lsb_bridge_detail.IMS_ARG_TOT_SPANS;
                    lsbOthDetails.IMS_NO_OF_VENTS = lsb_bridge_detail.IMS_NO_OF_VENTS;
                    lsbOthDetails.IMS_SPAN_VENT = lsb_bridge_detail.IMS_SPAN_VENT;
                    lsbOthDetails.IMS_SCOUR_DEPTH = lsb_bridge_detail.IMS_SCOUR_DEPTH;
                    lsbOthDetails.IMS_WIDTH_OF_BRIDGE = lsb_bridge_detail.IMS_WIDTH_OF_BRIDGE;
                    lsbOthDetails.IMS_APPROACH_COST = lsb_bridge_detail.IMS_APPROACH_COST;
                    lsbOthDetails.IMS_BRGD_STRUCTURE_COST = lsb_bridge_detail.IMS_BRGD_STRUCTURE_COST;
                    lsbOthDetails.IMS_STRUCTURE_COST = lsb_bridge_detail.IMS_STRUCTURE_COST;
                    lsbOthDetails.IMS_BRGD_OTHER_COST = lsb_bridge_detail.IMS_BRGD_OTHER_COST;
                    lsbOthDetails.IMS_APPROACH_PER_MTR = lsb_bridge_detail.IMS_APPROACH_PER_MTR;
                    lsbOthDetails.IMS_BRGD_STRUCTURE_PER_MTR = lsb_bridge_detail.IMS_BRGD_STRUCTURE_PER_MTR;
                    lsbOthDetails.IMS_STRUCTURE_PER_MTR = lsb_bridge_detail.IMS_STRUCTURE_PER_MTR;
                    lsbOthDetails.IMS_BRGD_OTHER_PER_MTR = lsb_bridge_detail.IMS_BRGD_OTHER_PER_MTR;

                    lsbOthDetails.TotalEstimatedCost = lsbOthDetails.IMS_APPROACH_COST + lsbOthDetails.IMS_BRGD_STRUCTURE_COST +
                                                        lsbOthDetails.IMS_STRUCTURE_COST + lsbOthDetails.IMS_BRGD_OTHER_COST;



                    //Populate Lists
                    lsbOthDetails.FOUNDATION_CODE = objCommonFuntion.PopulateScourTypeFoundation(true);
                    lsbOthDetails.SCOUR_DEPTH = objCommonFuntion.PopulateScourTypeFoundation(false);
                    lsbOthDetails.WIDTH_OF_BRIDGE = objCommonFuntion.PopulateWidthOfBridge();
                }
                else
                {   //Add Mode
                    lsbOthDetails.OPERATION = "C";
                    IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(Convert.ToInt32(id));

                    CommonFunctions objCommonFuntion = new CommonFunctions();
                    lsbOthDetails.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
                    lsbOthDetails.IMS_SC_FD_CODE = 0;
                    lsbOthDetails.FOUNDATION_CODE = objCommonFuntion.PopulateScourTypeFoundation(true);
                    lsbOthDetails.SCOUR_DEPTH = objCommonFuntion.PopulateScourTypeFoundation(false);
                    lsbOthDetails.WIDTH_OF_BRIDGE = objCommonFuntion.PopulateWidthOfBridge();
                }

                return View(lsbOthDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
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
        public ActionResult LSBOtherDetails(LSBOthDetailsModel lsbCostModel)
        {
            if (ModelState.IsValid)
            {
                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = new IMS_SANCTIONED_PROJECTS();
                string status = objLSBBAL.LSBOtherDetailsBAL(lsbCostModel);

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
                    return Json(new { success = false, ErrorMessage = status });
                }
            }
            return Json(
            new
            {
                success = false
            });
        }


        /// <summary>
        /// Post for Delete Other Details for LSB Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult DeleteLSBOthDetails(int id)
        {
            string status = string.Empty;
            try
            {
                status = objLSBBAL.DeleteLSBOthDetailsBAL(id);
                if (status == string.Empty)
                {
                    return Json(new { success = "true" });
                }
                else
                {
                    return Json(new { success = "false", errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = "false", errorMessage = status });
            }
        }


        /// <summary>
        /// Display Other Details for LSB Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult LSBDisplayOthDetails(String id)
        {
            LSBOthDetailsModel lsbOthDetails = new LSBOthDetailsModel();
            try
            {
                //Check is bridge details already entered?
                IMS_LSB_BRIDGE_DETAIL lsb_bridge_detail = db.IMS_LSB_BRIDGE_DETAIL.Find(Convert.ToInt32(id));

                //Edit Mode
                if (lsb_bridge_detail != null)
                {
                    lsbOthDetails.OPERATION = "U";
                    lsbOthDetails.IS_UPDATED = true;
                    CommonFunctions objCommonFuntion = new CommonFunctions();
                    lsbOthDetails.IMS_PR_ROAD_CODE = lsb_bridge_detail.IMS_PR_ROAD_CODE;

                    //Assign all properties
                    lsbOthDetails.IMS_ROAD_TYPE_LEVEL = lsb_bridge_detail.IMS_ROAD_TYPE_LEVEL;
                    lsbOthDetails.IMS_AVERAGE_GROUND_LEVEL = lsb_bridge_detail.IMS_AVERAGE_GROUND_LEVEL;
                    lsbOthDetails.IMS_NALA_BED_LEVEL = lsb_bridge_detail.IMS_NALA_BED_LEVEL;
                    lsbOthDetails.IMS_HIGHEST_FLOOD_LEVEL = lsb_bridge_detail.IMS_HIGHEST_FLOOD_LEVEL;
                    lsbOthDetails.IMS_ORDINARY_FLOOD_LEVEL = lsb_bridge_detail.IMS_ORDINARY_FLOOD_LEVEL;
                    lsbOthDetails.IMS_FOUNDATION_LEVEL = lsb_bridge_detail.IMS_FOUNDATION_LEVEL;
                    lsbOthDetails.IMS_HGT_BIRDGE_NBL = lsb_bridge_detail.IMS_HGT_BIRDGE_NBL;
                    lsbOthDetails.IMS_HGT_BRIDGE_FL = lsb_bridge_detail.IMS_HGT_BRIDGE_FL;
                    lsbOthDetails.IMS_BRG_SUBMERSIBLE = Convert.ToBoolean(lsb_bridge_detail.IMS_BRG_SUBMERSIBLE);
                    lsbOthDetails.IMS_BRG_BOX_CULVERT = Convert.ToBoolean(lsb_bridge_detail.IMS_BRG_BOX_CULVERT);
                    lsbOthDetails.IMS_BRG_RCC_ABUMENT = Convert.ToBoolean(lsb_bridge_detail.IMS_BRG_RCC_ABUMENT);
                    lsbOthDetails.IMS_BRG_HLB = Convert.ToBoolean(lsb_bridge_detail.IMS_BRG_HLB);

                    lsbOthDetails.IMS_SC_FD_CODE = lsb_bridge_detail.IMS_SC_FD_CODE;

                    lsbOthDetails.IMS_BEARING_CAPACITY = lsb_bridge_detail.IMS_BEARING_CAPACITY;
                    lsbOthDetails.IMS_ARG_TOT_SPANS = lsb_bridge_detail.IMS_ARG_TOT_SPANS;
                    lsbOthDetails.IMS_NO_OF_VENTS = lsb_bridge_detail.IMS_NO_OF_VENTS;
                    lsbOthDetails.IMS_SPAN_VENT = lsb_bridge_detail.IMS_SPAN_VENT;

                    lsbOthDetails.IMS_SCOUR_DEPTH = lsb_bridge_detail.IMS_SCOUR_DEPTH;

                    lsbOthDetails.IMS_WIDTH_OF_BRIDGE = lsb_bridge_detail.IMS_WIDTH_OF_BRIDGE;
                    lsbOthDetails.IMS_APPROACH_COST = lsb_bridge_detail.IMS_APPROACH_COST;
                    lsbOthDetails.IMS_BRGD_STRUCTURE_COST = lsb_bridge_detail.IMS_BRGD_STRUCTURE_COST;
                    lsbOthDetails.IMS_STRUCTURE_COST = lsb_bridge_detail.IMS_STRUCTURE_COST;
                    lsbOthDetails.IMS_BRGD_OTHER_COST = lsb_bridge_detail.IMS_BRGD_OTHER_COST;
                    lsbOthDetails.IMS_APPROACH_PER_MTR = lsb_bridge_detail.IMS_APPROACH_PER_MTR;
                    lsbOthDetails.IMS_BRGD_STRUCTURE_PER_MTR = lsb_bridge_detail.IMS_BRGD_STRUCTURE_PER_MTR;
                    lsbOthDetails.IMS_STRUCTURE_PER_MTR = lsb_bridge_detail.IMS_STRUCTURE_PER_MTR;
                    lsbOthDetails.IMS_BRGD_OTHER_PER_MTR = lsb_bridge_detail.IMS_BRGD_OTHER_PER_MTR;

                    lsbOthDetails.TotalEstimatedCost = lsbOthDetails.IMS_APPROACH_COST + lsbOthDetails.IMS_BRGD_STRUCTURE_COST +
                                                        lsbOthDetails.IMS_STRUCTURE_COST + lsbOthDetails.IMS_BRGD_OTHER_COST;


                    lsbOthDetails.FOUNDATION_TYPE_TEXT = db.MASTER_SCOUR_FOUNDATION_TYPE.Where(c => c.IMS_SC_FD_CODE == lsbOthDetails.IMS_SC_FD_CODE).Select(c => c.IMS_SC_FD_NAME).First();
                    lsbOthDetails.SCOUR_DEPTH_TEXT = db.MASTER_SCOUR_FOUNDATION_TYPE.Where(c => c.IMS_SC_FD_CODE == lsbOthDetails.IMS_SCOUR_DEPTH).Select(c => c.IMS_SC_FD_NAME).First();
                }

                return View(lsbOthDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region LSB Component Details

        /// <summary>
        /// Show Listing of assigned components in grid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult ShowLSBComponentList(string id)
        {
            PMGSY.Models.Proposal.LSBComponentModel lsbComponentModel = new LSBComponentModel();
            try
            {
                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(Convert.ToInt32(id));
                lsbComponentModel.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;

                lsbComponentModel.OPERATION = "C";
                return View("LSBComponentList", lsbComponentModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }


        /// <summary>
        /// Listing of Component Details for LSB
        /// </summary>
        /// <param name="roadId"></param>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult LSBComponentList(int roadId, FormCollection homeFormCollection)
        {
            long totalRecords;
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                var jsonData = new
                {
                    rows = objLSBBAL.LSBComponentList(roadId, Convert.ToInt32(homeFormCollection["page"]) - 1,
                                            Convert.ToInt32(homeFormCollection["rows"]),
                                            homeFormCollection["sidx"],
                                            homeFormCollection["sord"], out totalRecords),
                    total = totalRecords <=
                    Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords /
                    Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
                    records = totalRecords
                };

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }



        /// <summary>
        /// Get for LSB Component Details (Add new form)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult AddLSBComponentDetails(string id)
        {
            PMGSY.Models.Proposal.LSBComponentModel lsbComponentModel = new LSBComponentModel();
            try
            {
                lsbComponentModel.OPERATION = "C";
                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(Convert.ToInt32(id));
                CommonFunctions objCommonFuntion = new CommonFunctions();
                lsbComponentModel.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
                lsbComponentModel.COMPONENT_TYPE = objCommonFuntion.PopulateLSBComponentType(lsbComponentModel.IMS_PR_ROAD_CODE);

                return View(lsbComponentModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }


        /// <summary>
        /// Save LSB Component Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddLSBComponentDetails(LSBComponentModel lsbComponentModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string status = objLSBBAL.LSBComponentDetailsBAL(lsbComponentModel);
                    if (status == string.Empty)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }

                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }



        /// <summary>
        /// Edit each component for LSB
        /// </summary>
        /// <param name="pr_road_code"></param>
        /// <param name="componentCode"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditLSBComponentDetails(string roadCode, string componentCode)
        {
            PMGSY.Models.Proposal.LSBComponentModel lsbComponentModel = new LSBComponentModel();
            try
            {


                int pr_road_code = Convert.ToInt32(roadCode);
                int ims_component_code = Convert.ToInt32(componentCode);

                List<IMS_LSB_BRIDGE_COMPONENT_DETAIL> ims_component_detail = (from bcd in db.IMS_LSB_BRIDGE_COMPONENT_DETAIL
                                                                              where bcd.IMS_PR_ROAD_CODE == pr_road_code &&
                                                                                  bcd.IMS_COMPONENT_CODE == ims_component_code
                                                                              select bcd).ToList<IMS_LSB_BRIDGE_COMPONENT_DETAIL>();
                //Edit Mode
                lsbComponentModel.OPERATION = "U";
                CommonFunctions objCommonFuntion = new CommonFunctions();
                foreach (var item in ims_component_detail)
                {
                    lsbComponentModel.IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                    lsbComponentModel.IMS_COMPONENT_CODE = item.IMS_COMPONENT_CODE;
                    lsbComponentModel.IMS_QUANTITY = item.IMS_QUANTITY;
                    lsbComponentModel.IMS_TOTAL_COST = item.IMS_TOTAL_COST;
                    lsbComponentModel.IMS_GRADE_CONCRETE = item.IMS_GRADE_CONCRETE;
                }

                //Add as selected item
                List<SelectListItem> ComponentTypeList = new List<SelectListItem>();
                SelectListItem selectItem;
                var componentMaster = (from c in db.MASTER_COMPONENT_TYPE
                                       where c.MAST_COMPONENT_CODE == lsbComponentModel.IMS_COMPONENT_CODE
                                       select new
                                       {
                                           Text = c.MAST_COMPONENT_NAME,
                                           Value = c.MAST_COMPONENT_CODE
                                       }).ToList();

                foreach (var data in componentMaster)
                {
                    selectItem = new SelectListItem();
                    selectItem.Text = data.Text;
                    selectItem.Value = data.Value.ToString();
                    ComponentTypeList.Add(selectItem);
                }

                lsbComponentModel.COMPONENT_TYPE = ComponentTypeList;

                return View("AddLSBComponentDetails", lsbComponentModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }

        }


        /// <summary>
        /// Post for Delete LSB Component Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult DeleteLSBComponent(string roadCode, string componentCode)
        {
            string status = string.Empty;
            try
            {
                int pr_road_code = Convert.ToInt32(roadCode);
                int ims_component_code = Convert.ToInt32(componentCode);

                status = objLSBBAL.DeleteLSBComponentBAL(pr_road_code, ims_component_code);

                if (status == string.Empty)
                {
                    return Json(new { success = "true" });
                }
                else
                {
                    return Json(new { success = "false", errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = "false", errorMessage = status });
            }
        }

        #endregion

        #region STA Mord

        /// <summary>
        /// Finalize LSB Proposal at DPIU Level
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult DPIUFinalizeProposal()
        {
            try
            {
                int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);
                string status = objLSBBAL.DPIUFinalizeProposalBAL(IMS_PR_ROAD_CODE);
                if (status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = status });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw;
            }
        }



        /// <summary>
        /// Populate List of LSB Proposals for STA 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetSTALSBProposals(FormCollection formCollection)
        {
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
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                int IMS_State = Convert.ToInt32(Request.Params["IMS_STATE"]); //change on 23 june 2014 by deepak

                int totalRecords;

                var jsonData = new
                {
                    rows = objLSBBAL.GetSTALSBProposalsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_State, IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw;
            }
        }


        /// <summary>
        /// Populate List of LSB Proposals for MoRD 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetMORDLSBProposals(FormCollection formCollection)
        {
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
                    rows = objLSBBAL.GetMordLSBProposalsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_AGENCY, IMS_UPGRADE_CONNECT, out colModel),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                    TotalColumn = colModel
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw;
            }
        }


        /// <summary>
        /// Finalization (i.e. Scrutinize) of LSB Proposal at STA Level
        /// </summary>
        /// <param name="staSanctionViewModel"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult STAFinalizeLSBProposal(StaLSBSanctionViewModel staSanctionViewModel)
        {
            try
            {
                string status = objLSBBAL.StaFinalizeLSBProposalBAL(staSanctionViewModel, "Y");

                if (status == string.Empty)
                {
                    return Json(new { Success = "true" });
                }
                {
                    return Json(new { Success = "false", errorMessage = "There is an error occured while processing your request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                throw;
            }

        }


        /// <summary>
        /// UnScrutinize LSB Proposal at STA Level
        /// </summary>
        /// <param name="staSanctionViewModel"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult STAUnFinalizeLSBProposal(StaLSBSanctionViewModel staSanctionViewModel)
        {
            try
            {
                string status = objLSBBAL.StaFinalizeLSBProposalBAL(staSanctionViewModel, "U");

                if (status == string.Empty)
                {
                    return Json(new { Success = "true" });
                }
                {
                    return Json(new { Success = "false", errorMessage = "There is an error occured while processing your request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw;
            }

        }


        /// <summary>
        /// Populate Details of particular Scrutinized Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetStaLSBScrutiny(string id)
        {
            try
            {
                int IMS_PR_ROAD_CODE = Convert.ToInt32(id);
                IMS_SANCTIONED_PROJECTS ims_sanctioned_project = db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).First();

                StaLSBSanctionViewModel staSanctionViewModel = new StaLSBSanctionViewModel();
                staSanctionViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
                staSanctionViewModel.STA_SANCTIONED = ims_sanctioned_project.STA_SANCTIONED;
                staSanctionViewModel.IMS_SANCTIONED = ims_sanctioned_project.IMS_SANCTIONED;
                //staSanctionViewModel.STA_SANCTIONED_BY = db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_NAME).First();

                if (PMGSYSession.Current.RoleCode == 3)
                {
                    staSanctionViewModel.STA_SANCTIONED_BY = db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Any() ? db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_NAME).First() : "NA";
                }
                else
                {
                    staSanctionViewModel.STA_SANCTIONED_BY = db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == db.UM_User_Master.Where(a => a.UserName == ims_sanctioned_project.STA_SANCTIONED_BY).Select(a => a.UserID).FirstOrDefault()).Any() ? db.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == db.UM_User_Master.Where(a => a.UserName == ims_sanctioned_project.STA_SANCTIONED_BY).Select(a => a.UserID).FirstOrDefault()).Select(b => b.ADMIN_TA_NAME).First() : "NA";
                }



                staSanctionViewModel.IMS_SANCTIONED = ims_sanctioned_project.IMS_SANCTIONED;
                staSanctionViewModel.IMS_ISCOMPLETED = ims_sanctioned_project.IMS_ISCOMPLETED;

                if (ims_sanctioned_project.STA_SANCTIONED_DATE == null)
                {
                    staSanctionViewModel.STA_SANCTIONED_DATE = DateTime.Now.ToString("dd-MMM-yyyy");
                    staSanctionViewModel.STA_UNSCRUTINY_DATE = DateTime.Now.ToString("dd-MMM-yyyy");
                }
                else
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_project.STA_SANCTIONED_DATE);
                    staSanctionViewModel.STA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                    staSanctionViewModel.STA_UNSCRUTINY_DATE = dateTime.ToString("dd-MMM-yyyy");
                }
                if (ims_sanctioned_project.IMS_STA_REMARKS != null)
                    staSanctionViewModel.MS_STA_REMARKS = ims_sanctioned_project.IMS_STA_REMARKS.Trim();

                return View("StaLSBSanctionProposal", staSanctionViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw;
            }
        }

        /// <summary>
        /// Populate Details of particular MORD Sanctioned LSB Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetMordLSBSanctionDetails(string id)
        {
            try
            {
                int IMS_PR_ROAD_CODE = Convert.ToInt32(id);
                IMS_SANCTIONED_PROJECTS ims_sanctioned_project = db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).First();

                MordLSBSanctionViewModel mordSanctionViewModel = new MordLSBSanctionViewModel();

                mordSanctionViewModel.IMS_PR_ROAD_CODE = ims_sanctioned_project.IMS_PR_ROAD_CODE;
                mordSanctionViewModel.IMS_ISCOMPLETED = ims_sanctioned_project.IMS_ISCOMPLETED;
                //mordSanctionViewModel.IMS_SANCTIONED_BY = ims_sanctioned_project.IMS_SANCTIONED_BY;
                mordSanctionViewModel.IMS_SANCTIONED_BY = PMGSYSession.Current.UserName;  //change by Deepak 20 Sept 2014
                mordSanctionViewModel.IMS_SANCTIONED_BY_TEXT = ims_sanctioned_project.IMS_SANCTIONED_BY;  //change by Deepak 20 Sept 2014 
                mordSanctionViewModel.IMS_SANCTIONED = ims_sanctioned_project.IMS_SANCTIONED;
                mordSanctionViewModel.IMS_SANCTIONED_BW_AMT = ims_sanctioned_project.IMS_SANCTIONED_BW_AMT;
                mordSanctionViewModel.IMS_SANCTIONED_BS_AMT = ims_sanctioned_project.IMS_SANCTIONED_BS_AMT;

                //PMGSY Scheme2
                mordSanctionViewModel.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_project.IMS_IS_HIGHER_SPECIFICATION;
                mordSanctionViewModel.IMS_SANCTIONED_HS_AMT = ims_sanctioned_project.IMS_SANCTIONED_HS_AMT;

                mordSanctionViewModel.TotalEstimatedCost = ims_sanctioned_project.IMS_SANCTIONED_BW_AMT + ims_sanctioned_project.IMS_SANCTIONED_BS_AMT;


                if (ims_sanctioned_project.IMS_SANCTIONED_DATE == null)
                {
                    mordSanctionViewModel.IMS_SANCTIONED_DATE = DateTime.Now.ToString("dd-MMM-yyyy");
                }
                else
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_project.IMS_SANCTIONED_DATE);
                    mordSanctionViewModel.IMS_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                }

                if (ims_sanctioned_project.IMS_PROG_REMARKS != null)
                    mordSanctionViewModel.IMS_PROG_REMARKS = ims_sanctioned_project.IMS_PROG_REMARKS.Trim();

                mordSanctionViewModel.IMS_REASON = ims_sanctioned_project.IMS_REASON;
                if (mordSanctionViewModel.IMS_REASON != null)
                {
                    mordSanctionViewModel.Reason = db.MASTER_REASON.Where(a => a.MAST_REASON_CODE == mordSanctionViewModel.IMS_REASON).Select(a => a.MAST_REASON_NAME).First();
                }

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

                List<IMS_GET_ACTIONS_FOR_MORD_Result> list_IMS_GET_ACTIONS_FOR_MORD_Result = new ProposalBAL().GetMordActions(ims_sanctioned_project.IMS_PR_ROAD_CODE);
                mordSanctionViewModel.IS_SANCTIONABLE = Convert.ToBoolean(list_IMS_GET_ACTIONS_FOR_MORD_Result[0].SANCTIONABLE);
                mordSanctionViewModel.IS_UNSANCTIONABLE = Convert.ToBoolean(list_IMS_GET_ACTIONS_FOR_MORD_Result[0].UNSANCTIONABLE);
                mordSanctionViewModel.IS_RECONSIDERABLE = Convert.ToBoolean(list_IMS_GET_ACTIONS_FOR_MORD_Result[0].RECONSIDERABLE);
                mordSanctionViewModel.IS_DROPPABLE = Convert.ToBoolean(list_IMS_GET_ACTIONS_FOR_MORD_Result[0].DROPPABLE);
                mordSanctionViewModel.IS_EXECUTION_STARTED = list_IMS_GET_ACTIONS_FOR_MORD_Result[0].IS_EXECUTION_STARTED;
                mordSanctionViewModel.IS_AGREEMENT_FINALIZED = list_IMS_GET_ACTIONS_FOR_MORD_Result[0].IS_AGREEMENT_FINALIZED;

                return View("MordLSBSanctionProposal", mordSanctionViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw;
            }
        }

        /// <summary>
        /// Update Sanction cost details for particular LSB Prposal at MoRD Level
        /// </summary>
        /// <param name="mordSanctionViewModel"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult UpdateMordLSBSanctionDetails(MordLSBSanctionViewModel mordSanctionViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string status = objLSBBAL.UpdateMordLSBSanctionDetailsBAL(mordSanctionViewModel);
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
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw;
            }
        }


        /// <summary>
        /// Get the Bulk Sanction UI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult BulkLSBDetails(string id)
        {
            MordLSBSanctionViewModel bulkMordDetails = objLSBBAL.GetBulkMordDetailBAL(id);
            bulkMordDetails.IMS_PR_ROAD_CODES = id;
            bulkMordDetails.IMS_SANCTIONED_DATE = DateTime.Now.ToString("dd-MMM-yyyy");
            bulkMordDetails.IMS_SANCTIONED_BY = PMGSYSession.Current.UserName;
            return View("LSBBulkDetails", bulkMordDetails);
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

        #region PTA

        //Chnage by Ujjwal Saket on 1-11-2013 for PTA
        /// <summary>
        /// Populate List of LSB Proposals for PTA 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetPTALSBProposals(FormCollection formCollection)
        {
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

                int totalRecords;

                var jsonData = new
                {
                    rows = objLSBBAL.GetSTALSBProposalsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, MAST_STATE_ID, IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw;
            }
        } //Finish Changes


        /// <summary>
        /// Finalization (i.e. Scrutinize) of LSB Proposal at PTA Level
        /// </summary>
        /// <param name="ptaSanctionViewModel"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult PTAFinalizeLSBProposal(PtaLSBSanctionViewModel ptaSanctionViewModel)
        {
            try
            {
                string status = objLSBBAL.PtaFinalizeLSBProposalBAL(ptaSanctionViewModel, "Y");

                if (status == string.Empty)
                {
                    return Json(new { Success = "true" });
                }
                {
                    return Json(new { Success = "false", errorMessage = "There is an error occured while processing your request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw;
            }

        }


        /// <summary>
        /// UnScrutinize LSB Proposal at PTA Level
        /// </summary>
        /// <param name="staSanctionViewModel"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult PTAUnFinalizeLSBProposal(PtaLSBSanctionViewModel ptaSanctionViewModel)
        {
            try
            {
                string status = objLSBBAL.PtaFinalizeLSBProposalBAL(ptaSanctionViewModel, "U");

                if (status == string.Empty)
                {
                    return Json(new { Success = "true" });
                }
                {
                    return Json(new { Success = "false", errorMessage = "There is an error occured while processing your request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw;
            }

        }


        /// <summary>
        /// Populate Details of particular Scrutinized Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetPtaLSBScrutiny(string id)
        {
            try
            {
                int IMS_PR_ROAD_CODE = Convert.ToInt32(id);
                IMS_SANCTIONED_PROJECTS ims_sanctioned_project = db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).First();

                PtaLSBSanctionViewModel ptaSanctionViewModel = new PtaLSBSanctionViewModel();
                ptaSanctionViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
                ptaSanctionViewModel.PTA_SANCTIONED = ims_sanctioned_project.PTA_SANCTIONED;

                ptaSanctionViewModel.NAME_OF_PTA = ims_sanctioned_project.PTA_SANCTIONED_BY == null
                                                    ? db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_NAME).FirstOrDefault()
                                                    : db.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_TA_CODE == ims_sanctioned_project.PTA_SANCTIONED_BY).Select(a => a.ADMIN_TA_NAME).FirstOrDefault();
                if (ims_sanctioned_project.PTA_SANCTIONED_DATE == null)
                {
                    ptaSanctionViewModel.PTA_SANCTIONED_DATE = DateTime.Now.ToString("dd-MMM-yyyy");
                    ptaSanctionViewModel.PTA_UNSCRUTINY_DATE = DateTime.Now.ToString("dd-MMM-yyyy");
                }
                else
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_project.PTA_SANCTIONED_DATE);
                    ptaSanctionViewModel.PTA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                    ptaSanctionViewModel.PTA_UNSCRUTINY_DATE = dateTime.ToString("dd-MMM-yyyy");
                }
                if (ims_sanctioned_project.IMS_PTA_REMARKS != null)
                    ptaSanctionViewModel.MS_PTA_REMARKS = ims_sanctioned_project.IMS_PTA_REMARKS.Trim();

                return View("PtaLSBSanctionProposal", ptaSanctionViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw;
            }
        }

        #endregion

        #region UnlockLsb
        /// <summary>
        /// Edit the Unlocked LSB Propsoal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditUnlockedLSB(int id = 0)
        {
            ViewBag.operation = "U";
            IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(id);
            PMGSY.Models.Proposal.UnlockLSBViewModel objProposal = new UnlockLSBViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            try
            {
                objProposal.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;
                objProposal.MAST_BLOCK_NAME = ims_sanctioned_projects.MASTER_BLOCK.MAST_BLOCK_NAME;
                objProposal.MAST_STREAM_NAME = ims_sanctioned_projects.MASTER_STREAMS.MAST_STREAM_NAME;
                objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;
                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                //objProposal.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, objProposal.IMS_YEAR, false, true);

                objProposal.BATCHS = new List<SelectListItem>();
                objProposal.BATCHS.Insert(0, new SelectListItem() { Text = "Batch " + Convert.ToString(objProposal.IMS_BATCH), Value = Convert.ToString(objProposal.IMS_BATCH) });

                //objCommonFuntion.PopulateUnFreezedBatch(ims_sanctioned_projects.IMS_BATCH, objProposal.IMS_YEAR);

                //For PMGSY Scheme-2
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
                    objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                    objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST == null ? 0 : ims_sanctioned_projects.IMS_RENEWAL_COST.Value;
                    objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT;
                    objProposal.TotalCostWithHigherSpecCost = Convert.ToDecimal(ims_sanctioned_projects.IMS_BRIDGE_EST_COST_STATE) +
                                                              Convert.ToDecimal(ims_sanctioned_projects.IMS_BRIDGE_WORKS_EST_COST) +
                                                              Convert.ToDecimal(ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST == null ? 0 : ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST);
                }

                objProposal.IMS_COLLABORATION = ims_sanctioned_projects.IMS_COLLABORATION;
                objProposal.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency();

                objProposal.isPaymentDone = checkIsPayment(ims_sanctioned_projects.IMS_PR_ROAD_CODE);
                if (!objProposal.isPaymentDone)
                {
                    objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                    objProposal.STREAMS = objCommonFuntion.PopulateStreams("P");
                    objProposal.STREAMS.Find(x => x.Text == "Select Technology Proposed").Text = "Select Stream Proposed";
                }

                objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;

                objProposal.IMS_STAGED_YEAR = ims_sanctioned_projects.IMS_STAGED_YEAR;
                objProposal.IMS_STAGED_PACKAGE_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;
                objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;        //Road Name
                objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;

                objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;
                objProposal.TotalMaintenanceCost = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5 + (PMGSYSession.Current.PMGSYScheme == 2 ? (ims_sanctioned_projects.IMS_RENEWAL_COST == null ? 0 : ims_sanctioned_projects.IMS_RENEWAL_COST.Value) : 0);
                objProposal.IMS_RENEWAL_COST = (ims_sanctioned_projects.IMS_RENEWAL_COST == null ? 0 : ims_sanctioned_projects.IMS_RENEWAL_COST.Value);


                if (objProposal.IMS_UPGRADE_CONNECT.Equals("U"))
                {
                    objProposal.IMS_ROAD_NAME = (ims_sanctioned_projects.IMS_ROAD_NAME == null ? "-" : ims_sanctioned_projects.IMS_ROAD_NAME); //change by deepak on 4-09-2014
                }
                else if (objProposal.IMS_UPGRADE_CONNECT.Equals("N"))
                {
                    //objProposal.IMS_ROAD_NAME = (ims_sanctioned_projects.PLAN_CN_ROAD_CODE == null ? (ims_sanctioned_projects.IMS_ROAD_NAME == null ? "-" : ims_sanctioned_projects.IMS_ROAD_NAME) : ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME);//change by deepak on 4-09-2014
                    objProposal.IMS_ROAD_NAME = (ims_sanctioned_projects.PLAN_CN_ROAD_CODE == null ? (ims_sanctioned_projects.IMS_ROAD_NAME == null ? "-" : ims_sanctioned_projects.IMS_ROAD_NAME) : ims_sanctioned_projects.IMS_ROAD_NAME);//change by SAMMED PATIL on 03-03-2016
                }

                objProposal.IMS_BRIDGE_NAME = ims_sanctioned_projects.IMS_BRIDGE_NAME;
                objProposal.IMS_BRIDGE_LENGTH = ims_sanctioned_projects.IMS_BRIDGE_LENGTH;

                /// State Share
                objProposal.IMS_SANCTIONED_BS_AMT = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT);
                /// Bridge Work
                objProposal.IMS_SANCTIONED_BW_AMT = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT);

                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS == null ? "NA" : ims_sanctioned_projects.IMS_REMARKS;

                if ((db.IMS_LSB_BRIDGE_COMPONENT_DETAIL.Where(c => c.IMS_PR_ROAD_CODE == objProposal.IMS_PR_ROAD_CODE).Count() == 0)
                    || (db.IMS_LSB_BRIDGE_DETAIL.Where(c => c.IMS_PR_ROAD_CODE == objProposal.IMS_PR_ROAD_CODE).Count() == 0))
                {
                    objProposal.isAllDetailsEntered = false;        //either of data is not entered
                }
                else
                {
                    objProposal.isAllDetailsEntered = true;
                }

                //STA Scrutinized Details //----------------------------------------------------
                objProposal.STA_SANCTIONED = ims_sanctioned_projects.STA_SANCTIONED;
                objProposal.STA_SANCTIONED_BY = ims_sanctioned_projects.STA_SANCTIONED_BY;
                if (ims_sanctioned_projects.STA_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.STA_SANCTIONED_DATE).Year != 0)
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_projects.STA_SANCTIONED_DATE);
                    objProposal.STA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                }
                if (ims_sanctioned_projects.IMS_STA_REMARKS != null)
                    objProposal.MS_STA_REMARKS = ims_sanctioned_projects.IMS_STA_REMARKS.Trim();
                //-------------------------------------------------------------------------------

                objProposal.IMS_SANCTIONED = ims_sanctioned_projects.IMS_SANCTIONED;
                objProposal.IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS;
                objProposal.IMS_EXISTING_PACKAGE = ims_sanctioned_projects.IMS_EXISTING_PACKAGE;
                objProposal.TotalEstimatedCost = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT);
                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_MORD_SHARE_2015.Value : 0;
                objProposal.IMS_TOTAL_COST_2015 = objProposal.IMS_STATE_SHARE_2015 + objProposal.IMS_MORD_SHARE_2015;
                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null ? ((byte)(db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault() == 0 ? 3 : db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault())) : ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                ProposalDAL objProposalDAL = new ProposalDAL();
                if (objProposalDAL.IsProposalFinanciallyClosed(ims_sanctioned_projects.IMS_PR_ROAD_CODE) && ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null)
                {
                    objProposal.IMS_SHARE_PERCENT_2015 = 4;
                }
                else
                {
                    objProposal.IMS_SHARE_PERCENT_2015 = (ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null || ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == 0) ? ((byte)(db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault() == 0 ? 3 : db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault())) : ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                }


                return View("LSBUnlockedProposal", objProposal);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }

        public bool checkIsPayment(int prRoadCode)
        {
            ProposalBAL objProposalBAL = new ProposalBAL();
            try
            {
                bool status = objProposalBAL.checkIsPaymentBAL(prRoadCode);
                return status;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {

            }
        }

        /// <summary>
        /// Update the Unlocked Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult UpdateUnlockedLSB(PMGSY.Models.Proposal.UnlockLSBViewModel ims_sanctioned_projects)
        {
            try
            {
                ModelState.Remove("IMS_SANCTIONED_BW_AMT");
                if (ims_sanctioned_projects.isPaymentDone)
                {
                    ModelState.Remove("IMS_STREAMS");
                }
               
                if (ModelState.IsValid)
                {
                    string Status = objLSBBAL.UpdateUnlockedProposalBAL(ims_sanctioned_projects);

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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = ex.Message });
            }
        }

        #endregion
    }
}
