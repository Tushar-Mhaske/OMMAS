#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   FormReportsController.cs        
        * Description   :   Listing of Records for all Form Reports
        * Author        :   Shyam Yadav 
        * Creation Date :   28/August/2013
 **/
#endregion

using PMGSY.BAL.FormReports;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models.FormReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    [Audit]
    public class FormReportsController : Controller
    {

        private IFormReportsBAL formReportsBAL;

        #region Form1

        /// <summary>
        /// Renders View for Form1 Report
        /// </summary>
        /// <returns></returns>      
        public ActionResult Form1()
        {
            FormReportsViewModel formReportsViewModel = new FormReportsViewModel();
            try
            {
                formReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                formReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                return View(formReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// List the Form1 Details for all States
        /// Details - No Of Districts, No Of Blocks, No Of Villages & Respective Population 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form1StateLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form1StateLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Form1 Records for districts in Particular State
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]        
        public ActionResult Form1DistrictLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form1DistrictLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, Convert.ToInt32(formCollection["stateCode"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Form1 Records for Blocks in Particular District
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form1BlockLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form1BlockLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["districtCode"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Form1 Records for Villages in Particular Blocks
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]        
        public ActionResult Form1VillageLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form1VillageLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]),
                                                            Convert.ToInt32(formCollection["districtCode"]),
                                                            Convert.ToInt32(formCollection["blockCode"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }



        #endregion



        #region Form2


        /// <summary>
        /// Renders View for Form2 Report
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult Form2()
        {
            FormReportsViewModel formReportsViewModel = new FormReportsViewModel();
            try
            {
                formReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                formReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                formReportsViewModel.MAST_CONSTITUENCY_TYPE = "L";
                List<SelectListItem> lstConstTypes = new List<SelectListItem>();
                lstConstTypes.Insert(0, (new SelectListItem { Text = "MLA", Value = "L", Selected = true }));
                lstConstTypes.Insert(0, (new SelectListItem { Text = "MP", Value = "P" }));
                formReportsViewModel.CONSTITUENCY_TYPES = lstConstTypes;

                return View(formReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// List the Form2 Details for all States
        /// Statewise Total MLA/MP Constituency
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form2StateLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            int constCode = 0;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form2StateLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["constType"], constCode),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        /// <summary>
        /// Lists Constituency wise Details of Total Districts & Total Blocks under particular State
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]      
        public ActionResult Form2DistrictLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            int constCode = Convert.ToInt32(formCollection["constCode"]);
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form2DistrictLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), formCollection["constType"], constCode),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Lists Districts & Blocks under particular Constituency
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]     
        public ActionResult Form2ConstituencyLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form2ConstituencyLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["constCode"]), formCollection["constType"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        #endregion



        #region Form3

        /// <summary>
        /// Renders View for Form3 Report
        /// </summary>
        /// <returns></returns>
        
        public ActionResult Form3()
        {
            FormReportsViewModel formReportsViewModel = new FormReportsViewModel();
            try
            {
                formReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                formReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                return View(formReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// List the Form1 Details for all States
        /// Statewise Total / Unconnected / Benefitted / Balance population
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form3StateLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form3StateLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Form3 Records for districts in Particular State
        /// Districtwise Total / Unconnected / Benefitted / Balance population
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form3DistrictLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form3DistrictLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, Convert.ToInt32(formCollection["stateCode"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Form3 Records for Blocks in Particular District
        /// Blockwise Total / Unconnected / Benefitted / Balance population
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]     
        public ActionResult Form3BlockLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form3BlockLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["districtCode"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }




        #endregion



        #region Form4

        /// <summary>
        /// Renders View for Form4 Report
        /// </summary>
        /// <returns></returns>
        
        public ActionResult Form4()
        {
            FormReportsViewModel formReportsViewModel = new FormReportsViewModel();
            try
            {
                formReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                formReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

               
                List<SelectListItem> lstProposalType = new List<SelectListItem>();
                lstProposalType.Add(new SelectListItem { Text = "All Proposals", Value = "%", Selected = true });
                lstProposalType.Add(new SelectListItem { Text = "Road", Value = "P" });
                lstProposalType.Add(new SelectListItem { Text = "Bridges", Value = "L" });
                formReportsViewModel.PROPOSAL_TYPES = lstProposalType;
                formReportsViewModel.MAST_PROPOSAL_TYPE = "%";


                formReportsViewModel.MAST_YEAR = DateTime.Now.Year;
                formReportsViewModel.YEARS = new CommonFunctions().PopulateFinancialYear(true, true).ToList();
               
                //formReportsViewModel.YEARS.Insert(0, (new SelectListItem { Text = "All Years", Value = "0", Selected = true }));

                formReportsViewModel.IMS_BATCH = 0;
                formReportsViewModel.BATCHS = new CommonFunctions().PopulateBatch();
                formReportsViewModel.BATCHS.RemoveAt(0);
                formReportsViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                formReportsViewModel.IMS_COLLABORATION = 0;
                List<SelectListItem> ddcollaboration = new CommonFunctions().PopulateFundingAgency(true);
                ddcollaboration.RemoveAt(0);
                ddcollaboration.Insert(0, (new SelectListItem { Text = "All Funding Agency", Value = "0", Selected = true }));
                formReportsViewModel.COLLABORATIONS = ddcollaboration;


                return View(formReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// List the Form4 Details for all States
        /// Statewise Proposal Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]      
        public ActionResult Form4StateLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int blockCode = 0;
            string proposalType = formCollection["proposalType"];
            int year = Convert.ToInt32(formCollection["year"]);
            int batch = Convert.ToInt32(formCollection["batch"]);
            int collaboration = Convert.ToInt32(formCollection["collaboration"]);
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form4StateLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, stateCode, districtCode, blockCode, year, batch, collaboration, proposalType),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Form4 Records for districts in Particular State
        /// Districtwise Proposal Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]      
        public ActionResult Form4DistrictLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(formCollection["stateCode"]) : PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int blockCode = 0;
            string proposalType = formCollection["proposalType"];
            int year = Convert.ToInt32(formCollection["year"]);
            int batch = Convert.ToInt32(formCollection["batch"]);
            int collaboration = Convert.ToInt32(formCollection["collaboration"]);

            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form4DistrictLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                             stateCode, districtCode, blockCode, year, batch, collaboration, proposalType),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Form4 Records for Blocks in Particular District
        /// Blockwise Proposal Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form4BlockLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(formCollection["stateCode"]) : PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(formCollection["districtCode"]) : PMGSYSession.Current.DistrictCode;
            int blockCode = 0;
            string proposalType = formCollection["proposalType"];
            int year = Convert.ToInt32(formCollection["year"]);
            int batch = Convert.ToInt32(formCollection["batch"]);
            int collaboration = Convert.ToInt32(formCollection["collaboration"]);

            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form4BlockLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            stateCode, districtCode, blockCode, year, batch, collaboration, proposalType),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        /// <summary>
        /// Listing of Form4 Records for Blocks in Particular District
        /// Blockwise Proposal Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form4FinalLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(formCollection["stateCode"]) : PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(formCollection["districtCode"]) : PMGSYSession.Current.DistrictCode;
            int blockCode = Convert.ToInt32(formCollection["blockCode"]);
            string proposalType = formCollection["proposalType"];
            int year = Convert.ToInt32(formCollection["year"]);
            int batch = Convert.ToInt32(formCollection["batch"]);
            int collaboration = Convert.ToInt32(formCollection["collaboration"]);

            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form4FinalLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            stateCode, districtCode, blockCode, year, batch, collaboration, proposalType),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }



        #endregion



        #region Form5/6

        //Form 5 & 6 are in same report i.e. in Form5

        /// <summary>
        /// Renders View for Form5 Report
        /// </summary>
        /// <returns></returns>      
        public ActionResult Form5()
        {
            FormReportsViewModel formReportsViewModel = new FormReportsViewModel();
            try
            {
                formReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                formReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                formReportsViewModel.MAST_CONSTITUENCY_TYPE = "L";
                List<SelectListItem> lstConstTypes = new List<SelectListItem>();
                lstConstTypes.Insert(0, (new SelectListItem { Text = "MLA", Value = "L", Selected = true }));
                lstConstTypes.Insert(0, (new SelectListItem { Text = "MP", Value = "P" }));
                formReportsViewModel.CONSTITUENCY_TYPES = lstConstTypes;


                formReportsViewModel.MAST_YEAR = DateTime.Now.Year;
                formReportsViewModel.YEARS = new CommonFunctions().PopulateFinancialYear(true, true).ToList();
               
                //formReportsViewModel.YEARS.Insert(0, (new SelectListItem { Text = "All Years", Value = "0", Selected = true }));

                return View(formReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// List the Form5 Details for all States
        /// Statewise MLA / MP Corenetworks & Proposed Road Counts
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]      
        public ActionResult Form5StateLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            int year = Convert.ToInt32(formCollection["year"]);
            int constCode = 0;
            string constType = formCollection["constType"];

            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form5StateLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                           year, constType, constCode),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Form5 Records for districts in Particular State
        /// Districtwise MLA / MP Corenetworks & Proposed Road Counts
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form5DistrictLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            int constCode = Convert.ToInt32(formCollection["constCode"]);
            string constType = formCollection["constType"];
            int statecode = Convert.ToInt32(formCollection["stateCode"]);
            int year = Convert.ToInt32(formCollection["year"]);
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form5DistrictLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            year, statecode, constType, constCode),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Form4 Records for Blocks in Particular District
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form5ConstituencyLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            int constCode = Convert.ToInt32(formCollection["constCode"]);
            string constType = formCollection["constType"];
            int statecode = Convert.ToInt32(formCollection["stateCode"]);
            int year = Convert.ToInt32(formCollection["year"]);

            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form5ConstituencyLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            year, statecode,
                                                           constCode, constType),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(String.Empty);
            }
        }




        #endregion



        #region Form7

        /// <summary>
        /// Renders View for Form7 Report
        /// </summary>
        /// <returns></returns>       
        public ActionResult Form7()
        {
            FormReportsViewModel formReportsViewModel = new FormReportsViewModel();
            try
            {
                formReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                formReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

               
                List<SelectListItem> lstProposalType = new List<SelectListItem>();
                lstProposalType.Add(new SelectListItem { Text = "All Proposals", Value = "%", Selected = true });
                lstProposalType.Add(new SelectListItem { Text = "Road", Value = "P" });
                lstProposalType.Add(new SelectListItem { Text = "Bridges", Value = "L" });
                formReportsViewModel.PROPOSAL_TYPES = lstProposalType;
                formReportsViewModel.MAST_PROPOSAL_TYPE = "%";

                formReportsViewModel.MAST_YEAR = DateTime.Now.Year; 
                formReportsViewModel.YEARS = new CommonFunctions().PopulateFinancialYear(true, true).ToList();
              
                //formReportsViewModel.YEARS.Insert(0, (new SelectListItem { Text = "All Years", Value = "0", Selected = true }));

                formReportsViewModel.IMS_BATCH = 0;
                formReportsViewModel.BATCHS = new CommonFunctions().PopulateBatch();
                formReportsViewModel.BATCHS.RemoveAt(0);
                formReportsViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                formReportsViewModel.IMS_COLLABORATION = 0;
                List<SelectListItem> ddcollaboration = new CommonFunctions().PopulateFundingAgency(true);
                ddcollaboration.RemoveAt(0);
                ddcollaboration.Insert(0, (new SelectListItem { Text = "All Funding Agency", Value = "0", Selected = true }));
                formReportsViewModel.COLLABORATIONS = ddcollaboration;


                return View(formReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }
        }


        /// <summary>
        /// Listing of Statewise Proposals, Cost, Awards & Awarded amount
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form7StateLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form7StateLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["proposalType"], Convert.ToInt32(formCollection["batch"]),
                                                            Convert.ToInt32(formCollection["year"]), Convert.ToInt32(formCollection["collaboration"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Districtwise Proposals, Cost, Awards & Awarded amount
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]      
        public ActionResult Form7DistrictLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form7DistrictLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), formCollection["proposalType"], Convert.ToInt32(formCollection["batch"]),
                                                            Convert.ToInt32(formCollection["year"]), Convert.ToInt32(formCollection["collaboration"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Blockwise Proposals, Award & Agreement Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form7BlockLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form7BlockLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["districtCode"]),
                                                            formCollection["proposalType"], Convert.ToInt32(formCollection["batch"]),
                                                            Convert.ToInt32(formCollection["year"]), Convert.ToInt32(formCollection["collaboration"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        /// <summary>
        /// Listing of Roadwise Proposals, Award & Agreement Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]      
        public ActionResult Form7FinalLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form7FinalLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["districtCode"]),
                                                            Convert.ToInt32(formCollection["blockCode"]), formCollection["proposalType"], Convert.ToInt32(formCollection["batch"]),
                                                            Convert.ToInt32(formCollection["year"]), Convert.ToInt32(formCollection["collaboration"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        #endregion



        #region Form8

        /// <summary>
        /// Renders View for Form8 Report
        /// </summary>
        /// <returns></returns>
       
        public ActionResult Form8()
        {
            FormReportsViewModel formReportsViewModel = new FormReportsViewModel();
            try
            {
                formReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                formReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

               
                List<SelectListItem> lstProposalType = new List<SelectListItem>();
                lstProposalType.Add(new SelectListItem { Text = "All Proposals", Value = "%", Selected = true });
                lstProposalType.Add(new SelectListItem { Text = "Road", Value = "P" });
                lstProposalType.Add(new SelectListItem { Text = "Bridges", Value = "L" });
                formReportsViewModel.PROPOSAL_TYPES = lstProposalType;
                formReportsViewModel.MAST_PROPOSAL_TYPE = "%";

                formReportsViewModel.MAST_YEAR = DateTime.Now.Year;
                formReportsViewModel.YEARS = new CommonFunctions().PopulateFinancialYear(true, true).ToList();               
                //formReportsViewModel.YEARS.Insert(0, (new SelectListItem { Text = "All Years", Value = "0", Selected = true }));

                formReportsViewModel.IMS_BATCH = 0;
                formReportsViewModel.BATCHS = new CommonFunctions().PopulateBatch();
                formReportsViewModel.BATCHS.RemoveAt(0);
                formReportsViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                formReportsViewModel.IMS_COLLABORATION = 0;
                formReportsViewModel.COLLABORATIONS = new CommonFunctions().PopulateFundingAgency(true);
                formReportsViewModel.COLLABORATIONS.RemoveAt(0);
                formReportsViewModel.COLLABORATIONS.Insert(0, (new SelectListItem { Text = "All Funding Agency", Value = "0", Selected = true }));
                return View(formReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Listing of Statewise Totals of Proposals, Awards, Length Completed, Proposal Payment, Physical Length etc
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form8StateLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form8StateLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["proposalType"], Convert.ToInt32(formCollection["batch"]),
                                                            Convert.ToInt32(formCollection["year"]), Convert.ToInt32(formCollection["collaboration"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Districtwise Totals of Proposals, Awards, Length Completed, Proposal Payment, Physical Length etc
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form8DistrictLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form8DistrictLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), formCollection["proposalType"], Convert.ToInt32(formCollection["batch"]),
                                                            Convert.ToInt32(formCollection["year"]), Convert.ToInt32(formCollection["collaboration"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        ///  Listing of Blockwise Details of Proposals & Execution of works
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Form8BlockLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form8BlockLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["districtCode"]),
                                                            formCollection["proposalType"], Convert.ToInt32(formCollection["batch"]),
                                                            Convert.ToInt32(formCollection["year"]), Convert.ToInt32(formCollection["collaboration"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
             
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        /// <summary>
        /// Listing of Roadwise Totals of Proposals, Awards, Length Completed, Proposal Payment, Physical Length etc
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]      
        public ActionResult Form8FinalLevelListing(FormCollection formCollection)
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
            formReportsBAL = new FormReportsBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = formReportsBAL.Form8FinalLevelListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["districtCode"]),
                                                            Convert.ToInt32(formCollection["blockCode"]), formCollection["proposalType"], Convert.ToInt32(formCollection["batch"]),
                                                            Convert.ToInt32(formCollection["year"]), Convert.ToInt32(formCollection["collaboration"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        #endregion


    }
}
