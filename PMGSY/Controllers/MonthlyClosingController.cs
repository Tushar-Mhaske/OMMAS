using PMGSY.BAL.MonthlyClosing;
using PMGSY.Common;
using PMGSY.DAL.MonthlyClosing;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Models.MonthlyClosing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class MonthlyClosingController : Controller
    {
        CommonFunctions common = null;

        IMonthlyClosingBAL objMonthClosingBAL = null;

        [HttpGet]
        [Audit]
        public ActionResult GetMonthlyClosingPage()
        {
            common = new CommonFunctions();
            objMonthClosingBAL = new MonthlyClosingBAL();
            MonthlyClosingModel model = new MonthlyClosingModel();
            TransactionParams objParam = new TransactionParams();
            objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            objParam.LVL_ID = PMGSYSession.Current.LevelId;
            objParam.FUND_TYPE = PMGSYSession.Current.FundType;
            List<SelectListItem> monthList = common.PopulateMonths(DateTime.Now.Month);
            //get the account started year
            //change done by Vikram on 11-Dec-2013
            String _minMonthYear = objMonthClosingBAL.GetAccountStartMonthandYear(objParam);
            int _minYear = 0;
            if (_minMonthYear != "-1")
            {
                String[] param =  _minMonthYear.Split('$');
                _minYear = Convert.ToInt32(param[1]);
            }

            model.FROM_MONTH_LIST = monthList;

            List<SelectListItem> yearList = common.PopulateYears(DateTime.Now.Year);
            //change done by Vikram 
            yearList = yearList.Where(m => Convert.ToInt32(m.Value) >= _minYear).ToList<SelectListItem>();
            //end of change
            model.FROM_YEAR_LIST = yearList;

            model.TO_MONTH_LIST = new List<SelectListItem>(model.FROM_MONTH_LIST);

            model.TO_YEAR_LIST = new List<SelectListItem>(model.FROM_YEAR_LIST);

            ViewBag.levelId = PMGSYSession.Current.LevelId;

            //Added Abhishek kamble for monthly cloasing of DPIU at SRRDA level 28-Aug-2014
            if (PMGSYSession.Current.LevelId == 4)
            {
                model.DPIU_LIST = common.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
            }

            return View("MonthlyClosingPage",model);
        }


        /// <summary>
        /// action to get the closed month and year of agency
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult GetClosedMonthandYear()
        {
            try 
            {
                string result = string.Empty;                           
                TransactionParams objparams = new TransactionParams();  
                objMonthClosingBAL = new MonthlyClosingBAL();

                //if added by abhishek kamble 28-Aug-2014 to show month close of DPIU at srrda level Start 
                if (!(String.IsNullOrEmpty(Request.Params["DPIU_CODE"])) && !(String.IsNullOrEmpty(Request.Params["OWN_DPIU"])))
                {
                    if (Request.Params["OWN_DPIU"] == "D")
                    {
                        objparams.ADMIN_ND_CODE = Convert.ToInt32(Request.Params["DPIU_CODE"]);
                        objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                        objparams.LVL_ID = 5;
                    }
                    else
                    {
                        objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                        objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                        objparams.LVL_ID = PMGSYSession.Current.LevelId;
                    }
                }   //end 
                else
                {                       
                    objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                    objparams.LVL_ID = PMGSYSession.Current.LevelId;
                }
                result = objMonthClosingBAL.GetClosedMonthAndYear(objparams);

                if (result.Equals("-1"))
                {
                    return Json(new
                    {
                        monthClosed = false

                    });

                }
                else {

                    return Json(new
                    {
                        monthClosed = true,
                        month = result.Split('$')[0],
                        year = result.Split('$')[1]
                    });
                }
            
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
       }

        /// <summary>
        /// Action to get the account starting details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult GetAccountStartMonthandYear()
        {
            try
            {
                string result = string.Empty;

                TransactionParams objparams = new TransactionParams();  
                objMonthClosingBAL = new MonthlyClosingBAL();


                if (!(String.IsNullOrEmpty(Request.Params["DPIU_CODE"])) && !(String.IsNullOrEmpty(Request.Params["OWN_DPIU"])))
                {
                    if (Request.Params["OWN_DPIU"] == "D")
                    {
                        objparams.ADMIN_ND_CODE = Convert.ToInt32(Request.Params["DPIU_CODE"]);
                        objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                        objparams.LVL_ID = 5;
                    }
                    else
                    {
                        objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                        objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                        objparams.LVL_ID = PMGSYSession.Current.LevelId;
                    }
                }
                else
                {
                    objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                    objparams.LVL_ID = PMGSYSession.Current.LevelId;
                }
                result = objMonthClosingBAL.GetAccountStartMonthandYear(objparams);

                if (result.Equals("-1"))
                {
                    return Json(new
                    {
                        accountStarted= false

                    });

                }
                else
                {

                    return Json(new
                    {
                        accountStarted = true,
                        month = result.Split('$')[0],
                        year = result.Split('$')[1]
                    });
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }


        }




        /// <summary>
        /// action to get the list of PIU who has not closed the month
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetDpiuNotClosedMonth(int? page, int? rows, string sidx, string sord)
        {

            PaymentFilterModel objFilter = new PaymentFilterModel();
            objMonthClosingBAL = new MonthlyClosingBAL();
            try
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
               
                long totalRecords;

                objFilter.page = Convert.ToInt32(page) - 1;
                objFilter.rows = Convert.ToInt32(rows);
                objFilter.sidx = sidx.ToString();
                objFilter.sord = sord.ToString();
              
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.LevelId = PMGSYSession.Current.LevelId;
               
                objFilter.Month = Convert.ToInt16(Request.Params["fromMonths"]);
                objFilter.Year = Convert.ToInt16(Request.Params["fromYear"]);
                short toMonth = Convert.ToInt16(Request.Params["toMonth"]);
                short toYear = Convert.ToInt16(Request.Params["toYear"]);

               
                var jsonData = new
                {
                    rows = objMonthClosingBAL.ListDPIUMonthNotClosed(objFilter, toMonth, toYear, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = page,
                    records = totalRecords
                };

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }

        /// <summary>
        /// action to Close the month
        /// </summary>
        /// <returns> status of the operation </returns>
        [Audit]
        public ActionResult CloseMonth(MonthlyClosingModel model)
        {

            
            common = new CommonFunctions();
            string result = string.Empty;
            try
            {

               /* 
                short fromMonth = Convert.ToInt16(Request.Params["fromMonth"]);
                short fromYear = Convert.ToInt16(Request.Params["fromYear"]);
                short toMonth = Convert.ToInt16(Request.Params["toMonth"]);
                short toYear = Convert.ToInt16(Request.Params["toYear"]);
                */

                short fromMonth = 0;
                 short fromYear = 0;
                    short toMonth = 0;
                    short toYear = 0;

                if (ModelState.IsValid)
                {

                    if (model.FROM_YEAR > model.CURRENT_YEAR)
                    {
                        ModelState.AddModelError("FROM_YEAR", "From Year should be less than equal to current year");
                    }
                    else  if (model.TO_YEAR > model.CURRENT_YEAR)
                    {
                        ModelState.AddModelError("TO_YEAR", "TO Year should be less than equal to current year");
                    }

                    if (model.FROM_YEAR == model.CURRENT_YEAR && model.FROM_MONTH > model.CURRENT_MONTH)
                    {
                        ModelState.AddModelError("FROM_MONTH", " FROM month should be less than equal to current month");
                    }

                    if (model.TO_YEAR == model.CURRENT_YEAR && model.TO_MONTH > model.CURRENT_MONTH)
                    {
                        ModelState.AddModelError("TO_MONTH", " TO month should be less than equal to current month");
                    }

                    if (model.CLOSE_MONTH_TYPE == "A")
                    {

                        if (model.TO_YEAR < model.FROM_YEAR)
                        {
                            ModelState.AddModelError("TO_YEAR", " TO year should be greater than equal to From year");
                        }

                        if (model.TO_YEAR == model.FROM_YEAR && model.TO_MONTH < model.FROM_MONTH)
                        {
                            ModelState.AddModelError("TO_MONTH", " TO month should be greater than equal to From month");
                        }


                        if (!CheckFinanacialYearForMonthAndYear(model.FROM_MONTH, model.FROM_YEAR, model.TO_MONTH, model.TO_YEAR))
                        {

                            ModelState.AddModelError("TO_MONTH", "From Month & Year and To month & year should be in same financial year");
                        }

                    }

                    fromMonth = Convert.ToInt16(model.FROM_MONTH);
                    fromYear = Convert.ToInt16(model.FROM_YEAR);
                    toMonth = Convert.ToInt16(model.TO_MONTH);
                    toYear = Convert.ToInt16(model.TO_YEAR);

                    //check for validations
                    string monthlyClosingStatus = String.Empty;

                    objMonthClosingBAL = new MonthlyClosingBAL();


                    //Added By Abhishek kamble 28-Aug-2014 for Monthly Closing of DPIU At SRRDA level start
                    int Admin_Nd_Code = 0;
                    short level_ID = 0;
                    if ((PMGSYSession.Current.LevelId == 4) && model.OwnDPIUFlag == "D")
                    {
                        Admin_Nd_Code = model.DPIU_CODE;
                        level_ID = 5;
                    }
                    else
                    {
                        Admin_Nd_Code = PMGSYSession.Current.AdminNdCode;
                        level_ID = PMGSYSession.Current.LevelId;
                    }
                    //Added By Abhishek kamble 28-Aug-2014 for Monthly Closing of DPIU At SRRDA level start


                    //single month closing
                    if (toMonth == 0)
                    {
                       //check own
                                    
                        //Parameter NdCode and lvlId modified by Abhishek for Monthly Closing of DPIU At SRRDA level
                        //old
                        //monthlyClosingStatus = common.MonthlyClosingStatus(PMGSYSession.Current.AdminNdCode, fromMonth, fromYear, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId,false);
                        monthlyClosingStatus = common.MonthlyClosingStatus(Admin_Nd_Code, fromMonth, fromYear, PMGSYSession.Current.FundType, level_ID,false);
                        
                        //check for all PIU under it 

                        //Parameter if Condition modified by Abhishek for Monthly Closing of DPIU At SRRDA level                         
                        //old
                        //if (PMGSYSession.Current.LevelId == 4)
                        if (level_ID == 4)
                        {
                            //if monthly closing status for SRRDA is  not closed then return the status no need to check for lower levels
                            if(monthlyClosingStatus.Split('$')[0] != "-222")
                            {
                                 return Json(new { monthlyClosingStatus });
                            }

                            //check for all PIU under it 

                            string dpiuMonthClosingStatus = objMonthClosingBAL.CheckAllPiuForMonthlyClosing(PMGSYSession.Current.AdminNdCode, fromMonth, fromYear, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId);


                            //if all DPIU has closed their month
                            if (dpiuMonthClosingStatus.Split('$')[0] == "-666")
                            {
                                
                                //Validation added By Abhishek kamble 24-July-2014 for SRRDA level to check DPIU Cheque Ack Status start
                                if (PMGSYSession.Current.LevelId == 4)
                                {
                                    ListPIUNames PIUNamesModel = new ListPIUNames();
                                    PIUNamesModel.USP_ACC_VERIFY_PIUS_CHEQUEACK_Model= objMonthClosingBAL.CheckAllPiuForChequeAck(PMGSYSession.Current.FundType, PMGSYSession.Current.AdminNdCode, fromMonth, fromYear, toMonth, toYear, "S");

                                    if (PIUNamesModel.USP_ACC_VERIFY_PIUS_CHEQUEACK_Model != null && PIUNamesModel.USP_ACC_VERIFY_PIUS_CHEQUEACK_Model.Count > 0)
                                    {
                                        return PartialView("PIUChequeAckStatus", PIUNamesModel);                                                                               
                                    }

                                }
                                //Validation added By Abhishek kamble 24-July-2014 for SRRDA level to check DPIU Cheque Ack Status end

                               //close the month of SRRDA
                                result = objMonthClosingBAL.CloseMonth(fromMonth, fromYear, toMonth, toYear, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, level_ID);

                            }
                            else
                            {
                                return Json(new { monthlyClosingStatus = dpiuMonthClosingStatus });
                            }

                        }
                        else
                        {


                            //if month is not closed 
                            if (monthlyClosingStatus.Split('$')[0] == "-222")
                            {
                                //close the month
                                //Parameter modified by Abhishek for Monthly Closing of DPIU At SRRDA level     28-Aug-2014                    
                                //old
                                //result = objMonthClosingBAL.CloseMonth(fromMonth, fromYear, toMonth, toYear, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType);
                                result = objMonthClosingBAL.CloseMonth(fromMonth, fromYear, toMonth, toYear, Admin_Nd_Code, PMGSYSession.Current.FundType,level_ID);

                            }
                            else
                            {
                                return Json(new { monthlyClosingStatus });
                            }

                        }

                    }
                    else
                    {
                        //multiple months

                        //get all months in between

                        List<int> monthList = new List<int>();
                        List<int> yearList = new List<int>();

                        short fromMonth1 = fromMonth;
                        short fromYear1 = fromYear;


                        if (toYear == fromYear1)
                        {
                            var monthsList = Enumerable.Range(fromMonth1, ((toMonth + 1) - fromMonth1));

                            monthList = monthsList.Select(x => x).ToList();

                            foreach (int item in monthList)
                            {

                                yearList.Add(toYear);

                            }
                        }
                        else
                        {                                  
                            //while (fromMonth != toMonth && fromYear != toYear)
                            while (fromMonth != toMonth && fromYear <= toYear)//Modified By Abhishek to close multiple months 17-July-2014
                            {
                                monthList.Add(fromMonth); fromMonth++;
                                yearList.Add(fromYear);

                                if (fromMonth == 13)
                                {
                                    fromMonth = 1;
                                    fromYear = Convert.ToInt16(fromYear + 1);
                                }
                            }

                            monthList.Add(toMonth);
                            yearList.Add(toYear);

                        }

                        //check for each month

                      

                        for (int i = 0; i < monthList.Count(); i++)
                        {

                            if (i == 0)
                            {
                                //Parameter NdCode and lvlId modified by Abhishek for Monthly Closing of DPIU At SRRDA level                      
                                //old
                                //monthlyClosingStatus = common.MonthlyClosingStatus(PMGSYSession.Current.AdminNdCode, Convert.ToInt16(monthList[i]), Convert.ToInt16(yearList[i]), PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, false);
                                monthlyClosingStatus = common.MonthlyClosingStatus(Admin_Nd_Code, Convert.ToInt16(monthList[i]), Convert.ToInt16(yearList[i]), PMGSYSession.Current.FundType, level_ID, false);

                                if (monthlyClosingStatus.Split('$')[0] != "-222")
                                {

                                    return Json(new { monthlyClosingStatus });

                                }
                            }

                            //check for all PIU under it 
                            //If condition modified by Abhishek for Monthly Closing of DPIU At SRRDA level                      
                            //Old
                            //if (PMGSYSession.Current.LevelId == 4)
                            if (level_ID== 4)
                            {
                                //check for all PIU under it 

                                string dpiuMonthClosingStatus = objMonthClosingBAL.CheckAllPiuForMonthlyClosing(PMGSYSession.Current.AdminNdCode, Convert.ToInt16(monthList[i]), Convert.ToInt16(yearList[i]), PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId);


                                //if all DPIU has closed their month
                                if (dpiuMonthClosingStatus.Split('$')[0] == "-666")
                                {
                                    //close the month of SRRDA
                                     //Commented By Abhishek kamble 17-July-2014 dont need to close month here to avoid multiple times single month close.
                                    //result = objMonthClosingBAL.CloseMonth(fromMonth, fromYear, toMonth, toYear, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType);

                                }
                                else
                                {
                                    return Json(new { monthlyClosingStatus = dpiuMonthClosingStatus });
                                }
                            }

                        }

                        //Validation added By Abhishek kamble 24-July-2014 for SRRDA level to check DPIU Cheque Ack Status start
                        //If condition modified by Abhishek for Monthly Closing of DPIU At SRRDA level                      
                        //Old
                        //if (PMGSYSession.Current.LevelId == 4)
                        if (level_ID == 4)
                        {
                            ListPIUNames PIUNamesModel = new ListPIUNames();
                            PIUNamesModel.USP_ACC_VERIFY_PIUS_CHEQUEACK_Model = objMonthClosingBAL.CheckAllPiuForChequeAck(PMGSYSession.Current.FundType, PMGSYSession.Current.AdminNdCode, fromMonth1, fromYear1, toMonth, toYear, "M");

                            if (PIUNamesModel.USP_ACC_VERIFY_PIUS_CHEQUEACK_Model != null && PIUNamesModel.USP_ACC_VERIFY_PIUS_CHEQUEACK_Model.Count > 0)
                            {
                                return PartialView("PIUChequeAckStatus", PIUNamesModel);
                            }

                        }
                        //Validation added By Abhishek kamble 24-July-2014 for SRRDA level to check DPIU Cheque Ack Status end


                        //Modified By Abhishek kamble 17-July-2014 fromMonth1 (fromMonth) and fromYear1 (fromYear)

                        //Parameter modified by Abhishek for Monthly Closing of DPIU At SRRDA level     28-Aug-2014                    
                         //old
                        //result = objMonthClosingBAL.CloseMonth(fromMonth1, fromYear1, toMonth, toYear, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType);
                        result = objMonthClosingBAL.CloseMonth(fromMonth1, fromYear1, toMonth, toYear, Admin_Nd_Code, PMGSYSession.Current.FundType,level_ID);                        
                      
                    }

                    return Json(new { result });
                   
                }
                else {

                    List<SelectListItem> monthList = common.PopulateMonths(model.FROM_MONTH);
                    model.FROM_MONTH_LIST = monthList;

                    List<SelectListItem> yearList = common.PopulateYears(model.FROM_YEAR);
                    model.FROM_YEAR_LIST = yearList;

                    model.TO_MONTH_LIST = new List<SelectListItem>(model.FROM_MONTH_LIST);

                    model.TO_YEAR_LIST = new List<SelectListItem>(model.FROM_YEAR_LIST);

                    ViewBag.levelId = PMGSYSession.Current.LevelId;

                    return View("MonthlyClosingPage", model);

                }
                
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

              
        }

        public bool CheckFinanacialYearForMonthAndYear( int fromMonth,int fromYear,int toMonth, int toYear)
        {

                

                //from year should be less than equal to to year
                if (fromYear > toYear)
                {
                    return false;
                }

                //difference of years should be atmost 1
                if (toYear - fromYear == 1) {

                    if (fromMonth >= 4)
                    {

                        if (fromYear != toYear && (toMonth <= 3 && toYear == fromYear + 1))
                        {
                            return true;//same financial year
                        } else if (fromYear == toYear && fromMonth > toMonth) 
                        {
                            return true;
                        }
                        else return false;


                    } else if (toMonth < fromMonth && (fromYear == toYear))
                    {
                        return false;
                    }
                    else return false;
                }
                else if (toYear == fromYear)
                {
                    if (toMonth < fromMonth) {

                        return false;
                    }
                    else
                    {
                        if (fromMonth >= 3 && toMonth >= 3) {
                            return true;
                        }
                        else if (fromMonth <= 3 && toMonth <= 3) {
                            return true;
                        }
                        else return false;
                    }
                }
                else
                {
                    return false;
                }
            }

        //Validation Added By Abhishek kamble to Check PIU's Check Ack Status 18-July-2014
        //[HttpPost]
        //public ActionResult CheckAllPiuForChequeAck()
        //{ 
        //    objMonthClosingBAL = new MonthlyClosingBAL();

        //    int FromMonth = Convert.ToInt32(Request.Params["FromMonth"]);
        //    int FromYear = Convert.ToInt32(Request.Params["FromYear"]);
        //    int ToMonth = Convert.ToInt32(Request.Params["ToMonth"]);
        //    int ToYear = Convert.ToInt32(Request.Params["ToYear"]);
        //    string MonthCloseSingleOrMultiple = Request.Params["MonthCloseSingleOrMultiple"];
        //    string status = "false";                           
        
        //    ListPIUNames lstPIUNames = new ListPIUNames();
        //    lstPIUNames.lstPiuNames = new List<PIUNames>();                

        //    //For Multiple Month Check start              


        //    //multiple months

        //    //get all months in between   
        //    List<int> monthList = new List<int>();
        //    List<int> yearList = new List<int>();
        //    List<String> chkAckDetails = new List<String>();


        //    //For Single Month                  
        //    if (MonthCloseSingleOrMultiple == "S")
        //    {
        //        //List PIU for Perticular Month
        //        chkAckDetails = objMonthClosingBAL.CheckAllPiuForChequeAck(PMGSYSession.Current.FundType, PMGSYSession.Current.AdminNdCode, FromMonth, FromYear);
        //        status = chkAckDetails.Count == 0 ? "false" : "true";
        //        foreach (String PiuName in chkAckDetails)
        //        {
        //            PIUNames piuNames = new PIUNames();
        //            piuNames.ADMIN_ND_NAME = PiuName;
        //            //piuNames.Month = FromMonth.ToString();     
        //            piuNames.Month = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(FromMonth);
        //            piuNames.Year = FromYear.ToString();
        //            lstPIUNames.lstPiuNames.Add(piuNames);
        //        }
        //    }
        //    else   //For Multiple Months
        //    {
        //        if (ToYear == FromYear)
        //        {
        //            var monthsList = Enumerable.Range(FromMonth, ((ToMonth + 1) - FromMonth));

        //            monthList = monthsList.Select(x => x).ToList();

        //            foreach (int item in monthList)
        //            {
        //                yearList.Add(ToYear);
        //            }
        //        }
        //        else
        //        {
        //            while (FromMonth != ToMonth && FromYear <= ToYear)
        //            {
        //                monthList.Add(FromMonth); FromMonth++;
        //                yearList.Add(FromYear);

        //                if (FromMonth == 13)
        //                {
        //                    FromMonth = 1;
        //                    FromYear = Convert.ToInt16(FromYear + 1);
        //                }
        //            }
        //            monthList.Add(ToMonth);
        //            yearList.Add(ToYear);                       
        //        }

        //        //Multiple month piu list
        //        for (int i = 0; i < monthList.Count(); i++)
        //        {
        //            //List PIU for Perticular Month
        //            chkAckDetails = objMonthClosingBAL.CheckAllPiuForChequeAck(PMGSYSession.Current.FundType, PMGSYSession.Current.AdminNdCode, monthList[i], yearList[i]);

        //            if (chkAckDetails.Count != 0)
        //            {
        //                status = "true";
        //            }

        //            foreach (String PiuName in chkAckDetails)
        //            {
        //                PIUNames piuNames = new PIUNames();
        //                piuNames.ADMIN_ND_NAME = PiuName;
        //                //piuNames.Month = FromMonth.ToString();     
        //                piuNames.Month = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthList[i]);
        //                piuNames.Year = yearList[i].ToString();
        //                lstPIUNames.lstPiuNames.Add(piuNames);
        //            }
        //        }
        //    }



        //    //For Multiple Month check end
            
        //    if (status == "true")
        //    {
        //        return PartialView("PIUChequeAckStatus",lstPIUNames);
        //    }
        //    else
        //    {
        //        return Json(new {status = status });
        //    }
        //}

        #region
        [Audit]
        [HttpGet]
        public JsonResult GetClosedMonthYear()
        {
            MonthlyClosingDAL objDAL = new MonthlyClosingDAL();
            try
            {
                string result = string.Empty;
                TransactionParams objparams = new TransactionParams();
                objMonthClosingBAL = new MonthlyClosingBAL();

                //if added by abhishek kamble 28-Aug-2014 to show month close of DPIU at srrda level Start 
                if (!(String.IsNullOrEmpty(Request.Params["DPIU_CODE"])) && !(String.IsNullOrEmpty(Request.Params["OWN_DPIU"])))
                {
                    if (Request.Params["OWN_DPIU"] == "D")
                    {
                        objparams.ADMIN_ND_CODE = Convert.ToInt32(Request.Params["DPIU_CODE"]);
                        objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                        objparams.LVL_ID = 5;
                    }
                    else
                    {
                        objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                        objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                        objparams.LVL_ID = PMGSYSession.Current.LevelId;
                    }
                }   //end 
                else
                {
                    objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                    objparams.LVL_ID = PMGSYSession.Current.LevelId;
                }
                result = objDAL.GetClosedMonthAndYearDAL(objparams);

                if (result.Equals("-1"))
                {
                    return Json(new
                    {
                        monthClosed = false
                    },JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        monthClosed = true,
                        month = result.Split('$')[0],
                        year = result.Split('$')[1]
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MonthlyClosing.GetClosedMonthYear");
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
