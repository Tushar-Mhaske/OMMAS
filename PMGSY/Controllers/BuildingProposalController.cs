#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   BuildingProposalController.cs        
        * Description   :   Action methods for Creating , Editing, Deleting Building Propsoals and All the Data Related to Building Proposal                            
        * Author        :   Anand Singh  
        * Creation Date :   05/August/2015
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
using PMGSY.Models.BuildingProposal;
using PMGSY.Models.Proposal;
using System.Data.Entity.Validation;
using PMGSY.BAL.BuildingProposal;
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


using PMGSY.BAL.Master;
using PMGSY.DAL.Master;
using PMGSY.Models.Master;
using System.Transactions;


namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class BuildingProposalController : Controller
    {
        //
        // GET: /BuildingProposal/
        public BuildingProposalController()
        {
            PMGSYSession.Current.ModuleName = "Proposal";
        }

        #region Variable Declaration
        private PMGSYEntities db = new PMGSYEntities();
        private IBuildingProposalBAL buildingProposalBAL = new BuildingProposalBAL();
        Dictionary<string, string> decryptedParameters = null;
        string message = String.Empty;
        int outParam = 0;
        public MasterDAL objDAL = new MasterDAL();
        public IMasterBAL objBAL = new MasterBAL();

        #endregion

        /// <summary>
        ///  Screen : Listing Page of the Proposal
        /// Get the Proposals
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetProposals(FormCollection formCollection,int syear, int block, int batch, int stream,string proptype, string propstatus,string propconnect)
        {
            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
           
            int totalRecords;
            ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
            var jsonData = new
            {
                rows = buildingProposalBAL.GetBuildingProposalsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, syear, block, batch, stream, proptype, PMGSYSession.Current.AdminNdCode, propstatus,propconnect,PMGSYSession.Current.PMGSYScheme,PMGSYSession.Current.RoleCode ,formCollection["filters"]),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        [HttpPost]
        [Audit]
        public ActionResult GetMoRDProposals(FormCollection formCollection, int state, int district, int syear, int batch, int stream, string proptype, string propstatus, string propconnect, int eacode)
        {
            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            int totalRecords;
            ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
            var jsonData = new
            {
                rows = buildingProposalBAL.GetMoRDBuildingProposalsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, state, district, syear, 0, batch, stream, proptype, eacode, propstatus, propconnect, PMGSYSession.Current.PMGSYScheme,  formCollection["filters"]),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Get For Create Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult BuildingCreate(string id)
        {
           //BuildingProposalViewModel buildingViewModel = new BuildingProposalViewModel();
            BuildingProposalViewModel proposalViewModel = new BuildingProposalViewModel();
            //ProposalViewModel proposalViewModel = new ProposalViewModel();
            
            ViewBag.operation = "C";

            #region Default values

            proposalViewModel.StateName = PMGSYSession.Current.StateName;
            proposalViewModel.DistrictName = PMGSYSession.Current.DistrictName;
            proposalViewModel.stateType = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_TYPE).FirstOrDefault();
            //following property added by Vikram for providing the staged details to the districts which are IAP_DISTRICT
            proposalViewModel.DistrictType = (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_IAP_DISTRICT).FirstOrDefault();
            proposalViewModel.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_SHORT_CODE).FirstOrDefault() +
                                               (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_DISTRICT_ID).FirstOrDefault();


            proposalViewModel.IMS_UPGRADE_CONNECT = PMGSYSession.Current.PMGSYScheme == 1 ? "N" : "U";  // for Scheme 2 only upgradation is allowed

            proposalViewModel.IMS_EXISTING_PACKAGE = "N";

            proposalViewModel.IMS_IS_STAGED = "C";
            
            proposalViewModel.IMS_YEAR = DateTime.Now.Year;
            // Amounts 
            proposalViewModel.IMS_PAV_EST_COST = 0;
            
            proposalViewModel.IMS_SANCTIONED_MAN_AMT1 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT2 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT3 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT4 = 0;
            proposalViewModel.IMS_SANCTIONED_MAN_AMT5 = 0;

            
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

            proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency();
            proposalViewModel.IMS_COLLABORATION = 1;
            //proposalViewModel.STREAMS = objCommonFuntion.PopulateStreams("P");

            proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode);
            
            ViewBag.EXISTING_IMS_PACKAGE_ID = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

            ///Changes by SAMMED A. PATIL on 22FEB2018 orsambalpur issue
            proposalViewModel.UPGRADE_CONNECT = proposalViewModel.IMS_UPGRADE_CONNECT;
            
            return View(proposalViewModel);
        }


        
        [HttpPost]
        [Audit]
        public ActionResult BuildingCreate(PMGSY.Models.BuildingProposal.BuildingProposalViewModel buildingProposalViewModel)
        {
            try
            {
                ViewBag.operation = "C";
                if (ModelState.IsValid)
                {
                    string Status = buildingProposalBAL.CreateBuilding(buildingProposalViewModel);
                    if (Status == string.Empty)
                        return Json(new { Success = true, ErrorMessage = "Building Proposal Save." });
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
                throw ex;
            }
        }


        /// <summary>
        /// Get For Create Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult BuildingEdit(int id)
        {
            BuildingProposalViewModel proposalViewModel = buildingProposalBAL.GetBuildingProposal(id);
            

            ViewBag.operation = "U";

            proposalViewModel.StateName = PMGSYSession.Current.StateName;
            proposalViewModel.DistrictName = PMGSYSession.Current.DistrictName;
            proposalViewModel.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_SHORT_CODE).FirstOrDefault() +
                                               (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_DISTRICT_ID).FirstOrDefault();

            proposalViewModel.Years = PopulateYear();

            CommonFunctions objCommonFuntion = new CommonFunctions();
            proposalViewModel.BATCHS = objCommonFuntion.PopulateUnFreezedBatch(PMGSYSession.Current.StateCode, proposalViewModel.IMS_YEAR, false, PMGSYSession.Current.PMGSYScheme == 1 ? true : false);

            proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency();
           

            proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode);
            TransactionParams objParam = new TransactionParams { 
            DISTRICT_CODE=proposalViewModel.MAST_DISTRICT_CODE,
            SANC_YEAR=(short) proposalViewModel.IMS_YEAR
            };

            //added by shyam for PACKAGE_PREFIX as STATE_SHORT_CODE + MAST_DISTRICT_ID
            proposalViewModel.PACKAGE_PREFIX = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_SHORT_CODE).FirstOrDefault() +
                                               (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_DISTRICT_ID).FirstOrDefault();
            proposalViewModel.IMS_EXISTING_PACKAGE = "E";
            //proposalViewModel.EXISTING_IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
            //objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
           
            proposalViewModel.EXISTING_PACKAGES = GetStagedPackageID(proposalViewModel.IMS_YEAR, proposalViewModel.IMS_BATCH);

           // ViewBag.EXISTING_IMS_PACKAGE_ID = objCommonFuntion.PopulatePackage(objParam);//new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

            ///Changes by SAMMED A. PATIL on 22FEB2018 orsambalpur issue
            proposalViewModel.UPGRADE_CONNECT = proposalViewModel.IMS_UPGRADE_CONNECT;

            return View("BuildingCreate",proposalViewModel);
        }



        [HttpPost]
        [Audit]
        public ActionResult BuildingEdit(PMGSY.Models.BuildingProposal.BuildingProposalViewModel buildingProposalViewModel)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    ViewBag.operation = "U";
                    string message = string.Empty;
                    
                    if (buildingProposalBAL.BuildingProposalUpdate(buildingProposalViewModel, ref message))
                        return Json(new { Success = true,ErrorMessage=message });
                    else
                        return Json(new { Success = false, ErrorMessage = message });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult PIUFinalizedBuilding(int id)
        {
            try
            {

                
                    if (buildingProposalBAL.PIUFinalizedBuildingProposal(id))
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false});
             
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }
        /// <summary>
        /// Get method for Details of Building Proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult BuildingDetails(int id)
        {

            BuildingProposalViewModel buildingDetails;
           
            //IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(id);
            buildingDetails = buildingProposalBAL.GetBuildingProposal(id);


            return View(buildingDetails);
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
        public JsonResult BuildingProposalDelete(int id)
        {
            
            try
            {
                string message=string.Empty;
                if (buildingProposalBAL.BuildingProposalDelete(id,ref message))
                {
                    return Json(new { success = true, errorMessage = message });
                }
                else
                {
                    return Json(new { success = false, errorMessage = message });
                }
            }
            catch
            {
                return Json(new { success = false, errorMessage = "Processing Error." });
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
            catch
            {
                return null;
            }
        }

        [Audit]
        public ActionResult BuildingMordSanctionDetail(int id)
        {
           
            return View(buildingProposalBAL.GetSanctionBuildingProposal(id));
        }
        

            
        [HttpPost]
        [Audit]
        public ActionResult BuildingMordSanctionDetail(BuildingSanctionViewModel buildingMoRDSacntionViewModel)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                   
                    string message = string.Empty;
                    if (buildingProposalBAL.BuildingProposalMoRDSacntionUpdate(buildingMoRDSacntionViewModel, ref message))
                        return Json(new { Success = true,ErrorMessage=message });
                    else
                        return Json(new { Success = false, ErrorMessage = message });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }
    }
}
