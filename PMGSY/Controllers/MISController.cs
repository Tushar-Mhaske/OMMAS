#region HEADER
/*
* Project Id:

 * Project Name:OMMAS-II

 * File Name: MISController.cs

 * Author : Avinash Gawali

 * Creation Date :29/June/2019

 * Desc : This class is used as controller  to Get Data for Displaying Charts in case of Payment,DSC,Beneficiary
 */
#endregion

#region References
using Newtonsoft.Json;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.MIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
#endregion

namespace PMGSY.Controllers
{
    public class MISController : Controller
    {
        #region Methods
        public ActionResult Index()
        {
            return View();
        }

        #region Payment-Charts Layout
        /// <summary>
        ///This Method Returns HighCharts for PFMS MIS Payment
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult MISLayout()
        {
            MISDetailModel model = new MISDetailModel();
            PMGSYEntities dbContext = null;
            int decYear = 0;

            //Chart 1
            List<string> LocalStateShortNamelst1 = new List<string>();
            List<string> LocalStateChequeAmountlst1 = new List<string>();
            List<string> LocalTotalPaymentMadelst1 = new List<string>();

            //Chart 2
            List<string> LocalFinancialYearlst2 = new List<string>();
            List<string> LocalStateChequeAmountlst2 = new List<string>();
            List<string> LocalTotalPaymentMadelst2 = new List<string>();

            //Chart 3
            List<string> LocalMonthlst3 = new List<string>();
            List<string> LocalTotalPaymentMadelst3 = new List<string>();

            //Chart 4
            List<string> LocalMonthlst4 = new List<string>();
            List<string> LocalStateChequeAmountlst4 = new List<string>();

            //Chart DSC
            List<string> LocalStateShortNameDSC = new List<string>();
            List<string> LocalDSCFinalizedlst = new List<string>();
            List<string> LocalDSCVerifiedlst = new List<string>();

            //Chart Beneficiary
            List<string> LocalStateShortNameBeneficiary = new List<string>();
            List<string> LocalBeneficiaryFinalizedlst = new List<string>();
            List<string> LocalBeneficiaryVerifiedlst = new List<string>();
            List<string> LocalBeneficiaryTotallst = new List<string>();

            try
            {
                dbContext = new PMGSYEntities();

                string FundType = PMGSYSession.Current.FundType;
                int CurrentMonth = DateTime.Now.Month;
                int CurrentYear = DateTime.Now.Year;
                List<USP_RPT_MIS_PAYMENTS_Result> itemList = new List<USP_RPT_MIS_PAYMENTS_Result>();
                int seconds = DateTime.Now.Second;
                int minute = DateTime.Now.Minute;
                String sec = Convert.ToString(seconds);
                String min = Convert.ToString(minute);
                String combine = sec + "-" + min;

                itemList = dbContext.USP_RPT_MIS_PAYMENTS(FundType, CurrentMonth, CurrentYear, 0).ToList<USP_RPT_MIS_PAYMENTS_Result>();

                int seconds1 = DateTime.Now.Second;
                int minute1 = DateTime.Now.Minute;
                String sec1 = Convert.ToString(seconds1);
                String min1 = Convert.ToString(minute1);
                String combine1 = sec1 + "-" + min1;
                if (CurrentMonth <= 3)
                {
                    decYear = CurrentYear - 1;
                }
                else
                {
                    decYear = CurrentYear;
                }

                model.StateNameArr1 = new string[itemList.Count];
                model.TotalPaymentMadeArr1 = new string[itemList.Count];
                model.ChequeAmountArr1 = new string[itemList.Count];

                model.FinancialYearArr2 = new string[itemList.Count];
                model.ChequeAmountArr2 = new string[itemList.Count];
                model.TotalPaymentMadeArr2 = new string[itemList.Count];


                model.TotalPaymentMadeArr3 = new string[itemList.Count];

                model.MonthArr4 = new string[itemList.Count];
                model.ChequeAmountArr4 = new string[itemList.Count];


                #region Chart 1
                //Fin Year

                List<USP_RPT_MIS_PAYMENTS_Result> YearwiseFilter = new List<USP_RPT_MIS_PAYMENTS_Result>();
                YearwiseFilter = itemList.Where(x => x.FINANCIAL_YEAR == decYear).ToList();
                var StateChequePaymentMade = (from l in YearwiseFilter
                                              group l by l.MAST_STATE_SHORT_CODE into g
                                              select new
                                              {
                                                  FinYear = g.First().FINANCIAL_YEAR,
                                                  MAST_STATE_SHORT_CODE = g.First().MAST_STATE_SHORT_CODE,
                                                  chequeAmount = g.Sum(_ => _.CHQ_AMOUNT).ToString(),
                                                  TotalPaymentmade = g.Sum(x => x.Total_Payments_Made).ToString()

                                              }).ToList();
                StateChequePaymentMade = StateChequePaymentMade.Where(x => x.FinYear == decYear).ToList();


                decimal sumChqAmount1 = 0;
                Int32 sumTotalPaymentMade1 = 0;

                //First Chart
                foreach (var item in StateChequePaymentMade)
                {
                    //State Name
                    LocalStateShortNamelst1.Add(item.MAST_STATE_SHORT_CODE);
                    model.StateNameArr1 = LocalStateShortNamelst1.ToArray();
                    model.StateNamestr1 = string.Join(", ", model.StateNameArr1);

                    //Cheque Amount
                    LocalStateChequeAmountlst1.Add(item.chequeAmount.ToString());
                    model.ChequeAmountArr1 = LocalStateChequeAmountlst1.ToArray();
                    model.ChequeAmountstr1 = string.Join(", ", model.ChequeAmountArr1);

                    //Sum To Show
                    decimal chqAmount1 = Convert.ToDecimal(item.chequeAmount.ToString());
                    sumChqAmount1 = sumChqAmount1 + chqAmount1;


                    //Total Payment made
                    LocalTotalPaymentMadelst1.Add(item.TotalPaymentmade.ToString());
                    model.TotalPaymentMadeArr1 = LocalTotalPaymentMadelst1.ToArray();
                    model.TotalPaymentMadestr1 = string.Join(", ", model.TotalPaymentMadeArr1);

                    //Sum to Show
                    Int32 Paymade1 = Convert.ToInt32(item.TotalPaymentmade);
                    sumTotalPaymentMade1 = sumTotalPaymentMade1 + Paymade1;


                }

                model.sumChqAmount1 = String.Format("{0:0.00}", sumChqAmount1);
                model.sumPaymentMade1 = Convert.ToString(sumTotalPaymentMade1);


                #endregion

                #region Chart 2
                //Second Chart
                var FinancialYearChequeAmount = (from l in itemList
                                                 group l by l.FINANCIAL_YEAR into g
                                                 select new
                                                 {
                                                     FINANCIAL_YEAR = g.First().FINANCIAL_YEAR,
                                                     chequeAmount = g.Sum(_ => _.CHQ_AMOUNT).ToString(),
                                                     TotalPaymentmade = g.Sum(x => x.Total_Payments_Made).ToString()

                                                 }).ToList();

                decimal sumChqAmount2 = 0;
                Int32 sumTotalPaymentMade2 = 0;

                //Second Chart
                foreach (var item in FinancialYearChequeAmount)
                {
                    Int32 FinYear = Convert.ToInt32(item.FINANCIAL_YEAR);
                    Int32 IncFinYear = FinYear + 1;
                    string strINCFinYear = Convert.ToString(IncFinYear);
                    string combineFinYear = item.FINANCIAL_YEAR + "-" + strINCFinYear;


                    LocalFinancialYearlst2.Add(combineFinYear);
                    model.FinancialYearArr2 = LocalFinancialYearlst2.ToArray();
                    model.FinancialYearstr2 = string.Join(", ", model.FinancialYearArr2);

                    LocalStateChequeAmountlst2.Add(item.chequeAmount.ToString());
                    model.ChequeAmountArr2 = LocalStateChequeAmountlst2.ToArray();
                    model.ChequeAmountstr2 = string.Join(", ", model.ChequeAmountArr2);

                    //Sum To Show
                    decimal chqAmount2 = Convert.ToDecimal(item.chequeAmount.ToString());
                    sumChqAmount2 = sumChqAmount2 + chqAmount2;



                    LocalTotalPaymentMadelst2.Add(item.TotalPaymentmade.ToString());
                    model.TotalPaymentMadeArr2 = LocalTotalPaymentMadelst2.ToArray();
                    model.TotalPaymentMadestr2 = string.Join(", ", model.TotalPaymentMadeArr2);

                    //Sum to Show
                    Int32 Paymade2 = Convert.ToInt32(item.TotalPaymentmade);
                    sumTotalPaymentMade2 = sumTotalPaymentMade2 + Paymade2;

                }
                model.sumChqAmount2 = String.Format("{0:0.00}", sumChqAmount2);
                model.sumPaymentMade2 = Convert.ToString(sumTotalPaymentMade2);



                #endregion

                #region Chart 3

                List<USP_RPT_MIS_PAYMENTS_Result> yearFilterLst1 = new List<USP_RPT_MIS_PAYMENTS_Result>();
                yearFilterLst1 = itemList.Where(x => x.FINANCIAL_YEAR == decYear).ToList();


                //Third Chart
                var monthPaymentMade = (from l in yearFilterLst1
                                        group l by l.BILL_MONTH into g
                                        select new
                                        {
                                            FinYear = g.First().FINANCIAL_YEAR,
                                            m = g.First().BILL_MONTH,
                                            Months = g.First().BILL_MONTH.ToString(),
                                            TotalPaymentmade = g.Where(x => x.Total_Payments_Made > 0).Average(x => (int?)x.Total_Payments_Made).ToString()

                                        }).OrderBy(x => x.m).ToList();

                monthPaymentMade = monthPaymentMade.Where(x => x.FinYear == decYear).ToList();

                Int32 sumTotalPaymentMade3 = 0;

                //Third Chart
                foreach (var item in monthPaymentMade)
                {
                    Int32 month = Convert.ToInt32(item.Months);
                    string MonthName = dbContext.MASTER_MONTH.Where(x => x.MAST_MONTH_CODE == month).Select(x => x.MAST_MONTH_SHORT_NAME).FirstOrDefault();


                    LocalMonthlst3.Add(MonthName);
                    model.MonthArr3 = LocalMonthlst3.ToArray();
                    model.Monthstr3 = string.Join(", ", model.MonthArr3);

                    string str = String.Empty;
                    if (string.IsNullOrEmpty(item.TotalPaymentmade))
                    {
                        str = "0";

                    }
                    else
                    {
                        decimal paymentMade = Convert.ToDecimal(item.TotalPaymentmade);
                        decimal truncatedpaymentMade = Math.Truncate(paymentMade);
                        str = Convert.ToString(truncatedpaymentMade);
                    }

                    

                    LocalTotalPaymentMadelst3.Add(str);
                    model.TotalPaymentMadeArr3 = LocalTotalPaymentMadelst3.ToArray();
                    model.TotalPaymentMadestr3 = string.Join(", ", model.TotalPaymentMadeArr3);

                    sumTotalPaymentMade3 = sumTotalPaymentMade3 + Convert.ToInt32(str);
                    
                }
                model.sumPaymentMade3 = Convert.ToString(sumTotalPaymentMade3);

                #endregion

                #region Chart 4

                List<USP_RPT_MIS_PAYMENTS_Result> yearFilterLst2 = new List<USP_RPT_MIS_PAYMENTS_Result>();
                yearFilterLst2 = itemList.Where(x => x.FINANCIAL_YEAR == decYear).ToList();


                //Fourth Chart
                //var chequeAmountMonth = (from l in yearFilterLst2
                //                        group l by l.BILL_MONTH into g
                //                        select new
                //                        {
                //                            FinYear=g.First().FINANCIAL_YEAR,
                //                            m = g.First().BILL_MONTH,
                //                            Months = g.First().BILL_MONTH.ToString(),
                //                            chequeAmount = g.Where(x => x.CHQ_AMOUNT > 0).Average(x =>x.CHQ_AMOUNT).ToString()
                //                        }).OrderBy(x => x.m).ToList();


                var chequeAmountMonth = (from l in yearFilterLst2
                                         group l by l.BILL_MONTH into g
                                         select new
                                         {
                                             FinYear = g.First().FINANCIAL_YEAR,
                                             m = g.First().BILL_MONTH,
                                             Months = g.First().BILL_MONTH.ToString(),
                                             chequeAmount = g.Where(x => x.CHQ_AMOUNT > 0).Average(x => x.CHQ_AMOUNT).ToString()
                                         }).ToList();


                chequeAmountMonth = chequeAmountMonth.Where(x => x.FinYear == decYear).ToList();

                decimal sumChqAmount4 = 0;

                //Fourth Chart
                foreach (var item in chequeAmountMonth)
                {
                    Int32 month = Convert.ToInt32(item.Months);
                    string MonthName = dbContext.MASTER_MONTH.Where(x => x.MAST_MONTH_CODE == month).Select(x => x.MAST_MONTH_SHORT_NAME).FirstOrDefault();

                    LocalMonthlst4.Add(MonthName);
                    model.MonthArr4 = LocalMonthlst4.ToArray();
                    model.Monthstr4 = string.Join(", ", model.MonthArr4);

                    string str = String.Empty;
                    if (string.IsNullOrEmpty(item.chequeAmount))
                    {
                        str = "0";
                    }
                    else
                    {
                        decimal dec = Convert.ToDecimal(item.chequeAmount);
                        string str1 = String.Format("{0:0.00}", dec);
                        str = str1;

                    }
                    LocalStateChequeAmountlst4.Add(str);
                    model.ChequeAmountArr4 = LocalStateChequeAmountlst4.ToArray();
                    model.ChequeAmountstr4 = string.Join(", ", model.ChequeAmountArr4);

                    sumChqAmount4 = sumChqAmount4 + Convert.ToDecimal(str);

                }

                model.sumChqAmount4 = Convert.ToString(sumChqAmount4);
                


                #endregion

                #region DSC
                List<USP_RPT_MIS_DSC_Result> itemListDSC = new List<USP_RPT_MIS_DSC_Result>();
                itemListDSC = dbContext.USP_RPT_MIS_DSC().ToList<USP_RPT_MIS_DSC_Result>();
                var DscList = (from l in itemListDSC
                               group l by l.MAST_STATE_SHORT_CODE into g
                               select new
                               {
                                   StateCode = g.First().MAST_STATE_CODE,
                                   State_Short_Name = g.First().MAST_STATE_SHORT_CODE,
                                   DscFinalized = g.First().DSC_FINALIZED.ToString(),
                                   Dsc_Verified = g.First().DSC_VERIFIED.ToString(),
                               }).ToList();

                Int32 dscFinalized = 0;
                Int32 dscVerified = 0;

                foreach (var item in DscList)
                {
                    LocalStateShortNameDSC.Add(item.State_Short_Name);
                    model.DSCStateNameArr = LocalStateShortNameDSC.ToArray();
                    model.DSCStateNamestr = string.Join(", ", model.DSCStateNameArr);

                    LocalDSCFinalizedlst.Add(item.DscFinalized);
                    model.DSCFinalizedArr = LocalDSCFinalizedlst.ToArray();
                    model.DSCFinalizedstr = string.Join(", ", model.DSCFinalizedArr);

                    dscFinalized = dscFinalized + Convert.ToInt32(item.DscFinalized);

                    LocalDSCVerifiedlst.Add(item.Dsc_Verified);
                    model.DSCVerifiedArr = LocalDSCVerifiedlst.ToArray();
                    model.DSCVerifiedstr = string.Join(", ", model.DSCVerifiedArr);

                    dscVerified = dscVerified + Convert.ToInt32(item.Dsc_Verified);

                }

                model.sumDSCFinalized = Convert.ToString(dscFinalized);
                model.sumDSCVerified = Convert.ToString(dscVerified);


                #endregion

                #region Beneficiary
                List<USP_RPT_MIS_CONTRACTORS_Result> itemListBeneficiary = new List<USP_RPT_MIS_CONTRACTORS_Result>();
                itemListBeneficiary = dbContext.USP_RPT_MIS_CONTRACTORS().ToList<USP_RPT_MIS_CONTRACTORS_Result>();
                var BeneficiaryList = (from l in itemListBeneficiary
                                       group l by l.MAST_STATE_SHORT_CODE into g
                                       select new
                                       {
                                           StateCode = g.First().MAST_STATE_CODE,
                                           State_Short_Name = g.First().MAST_STATE_SHORT_CODE,
                                           TotalBeneficiary = g.First().TotalBeneficiaries.ToString(),
                                           BeneficiaryFinalized = g.First().Finalized.ToString(),
                                           BeneficiaryVerified = g.First().VERIFIED.ToString(),
                                       }).ToList();

                Int32 sumTotalBeneficiary = 0;
                Int32 sumBeneficiaryFinalized = 0;
                Int32 sumBeneficiaryVerified = 0;
                foreach (var item in BeneficiaryList)
                {
                    LocalStateShortNameBeneficiary.Add(item.State_Short_Name);
                    model.BeneficiaryStateNameArr = LocalStateShortNameBeneficiary.ToArray();
                    model.BeneficiaryStateNamestr = string.Join(", ", model.BeneficiaryStateNameArr);

                    LocalBeneficiaryTotallst.Add(item.TotalBeneficiary);
                    model.BeneficiaryTotalArr = LocalBeneficiaryTotallst.ToArray();
                    model.BeneficiaryTotalstr = string.Join(", ", model.BeneficiaryTotalArr);

                    sumTotalBeneficiary = sumTotalBeneficiary + Convert.ToInt32(item.TotalBeneficiary);


                    LocalBeneficiaryFinalizedlst.Add(item.BeneficiaryFinalized);
                    model.BeneficiaryFinalizedArr = LocalBeneficiaryFinalizedlst.ToArray();
                    model.BeneficiaryFinalizedstr = string.Join(", ", model.BeneficiaryFinalizedArr);
                    sumBeneficiaryFinalized = sumBeneficiaryFinalized + Convert.ToInt32(item.BeneficiaryFinalized);


                    LocalBeneficiaryVerifiedlst.Add(item.BeneficiaryVerified);
                    model.BeneficiaryVerifiedArr = LocalBeneficiaryVerifiedlst.ToArray();
                    model.BeneficiaryVerifiedstr = string.Join(", ", model.BeneficiaryVerifiedArr);
                    sumBeneficiaryVerified = sumBeneficiaryVerified + Convert.ToInt32(item.BeneficiaryVerified);

                }

                model.sumTotalBeneficiary = Convert.ToString(sumTotalBeneficiary);
                model.sumBeneficiaryFinalized = Convert.ToString(sumBeneficiaryFinalized);
                model.sumBeneficiaryVerified = Convert.ToString(sumBeneficiaryVerified);
                
                #endregion

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MISController.MISLayout()");
                return null;
            }

        }
        #endregion

        #region DSC/Beneficiary-Charts Layout

        /// <summary>
        ///This Method Returns HighCharts for PFMS MIS DSC/Beneficiary
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult MISDSCBeneficiaryLayout()
        {
            MISDetailModel model = new MISDetailModel();
            PMGSYEntities dbContext = null;

            //Chart DSC
            List<string> LocalStateShortNameDSC = new List<string>();
            List<string> LocalDSCFinalizedlst = new List<string>();
            List<string> LocalDSCVerifiedlst = new List<string>();

            //Chart Beneficiary
            List<string> LocalStateShortNameBeneficiary = new List<string>();
            List<string> LocalBeneficiaryFinalizedlst = new List<string>();
            List<string> LocalBeneficiaryVerifiedlst = new List<string>();
            List<string> LocalBeneficiaryTotallst = new List<string>();
            try
            {
                dbContext = new PMGSYEntities();

                #region DSC
                List<USP_RPT_MIS_DSC_Result> itemListDSC = new List<USP_RPT_MIS_DSC_Result>();
                itemListDSC = dbContext.USP_RPT_MIS_DSC().ToList<USP_RPT_MIS_DSC_Result>();
                var DscList = (from l in itemListDSC
                               group l by l.MAST_STATE_SHORT_CODE into g
                               select new
                               {
                                   StateCode = g.First().MAST_STATE_CODE,
                                   State_Short_Name = g.First().MAST_STATE_SHORT_CODE,
                                   DscFinalized = g.First().DSC_FINALIZED.ToString(),
                                   Dsc_Verified = g.First().DSC_VERIFIED.ToString(),
                               }).ToList();

                Int32 dscFinalized = 0;
                Int32 dscVerified = 0;
                
                foreach (var item in DscList)
                {
                    LocalStateShortNameDSC.Add(item.State_Short_Name);
                    model.DSCStateNameArr = LocalStateShortNameDSC.ToArray();
                    model.DSCStateNamestr = string.Join(", ", model.DSCStateNameArr);

                    LocalDSCFinalizedlst.Add(item.DscFinalized);
                    model.DSCFinalizedArr = LocalDSCFinalizedlst.ToArray();
                    model.DSCFinalizedstr = string.Join(", ", model.DSCFinalizedArr);

                    dscFinalized = dscFinalized + Convert.ToInt32(item.DscFinalized);

                    LocalDSCVerifiedlst.Add(item.Dsc_Verified);
                    model.DSCVerifiedArr = LocalDSCVerifiedlst.ToArray();
                    model.DSCVerifiedstr = string.Join(", ", model.DSCVerifiedArr);

                    dscVerified = dscVerified + Convert.ToInt32(item.Dsc_Verified);

                }

                model.sumDSCFinalized = Convert.ToString(dscFinalized);
                model.sumDSCVerified = Convert.ToString(dscVerified);


                #endregion

                #region Beneficiary
                List<USP_RPT_MIS_CONTRACTORS_Result> itemListBeneficiary = new List<USP_RPT_MIS_CONTRACTORS_Result>();
                itemListBeneficiary = dbContext.USP_RPT_MIS_CONTRACTORS().ToList<USP_RPT_MIS_CONTRACTORS_Result>();
                var BeneficiaryList = (from l in itemListBeneficiary
                                       group l by l.MAST_STATE_SHORT_CODE into g
                                       select new
                                       {
                                           StateCode = g.First().MAST_STATE_CODE,
                                           State_Short_Name = g.First().MAST_STATE_SHORT_CODE,
                                           TotalBeneficiary = g.First().TotalBeneficiaries.ToString(),
                                           BeneficiaryFinalized = g.First().Finalized.ToString(),
                                           BeneficiaryVerified = g.First().VERIFIED.ToString(),
                                       }).ToList();

                Int32 sumTotalBeneficiary = 0;
                Int32 sumBeneficiaryFinalized = 0;
                Int32 sumBeneficiaryVerified= 0;
                foreach (var item in BeneficiaryList)
                {
                    LocalStateShortNameBeneficiary.Add(item.State_Short_Name);
                    model.BeneficiaryStateNameArr = LocalStateShortNameBeneficiary.ToArray();
                    model.BeneficiaryStateNamestr = string.Join(", ", model.BeneficiaryStateNameArr);

                    LocalBeneficiaryTotallst.Add(item.TotalBeneficiary);
                    model.BeneficiaryTotalArr = LocalBeneficiaryTotallst.ToArray();
                    model.BeneficiaryTotalstr = string.Join(", ", model.BeneficiaryTotalArr);

                    sumTotalBeneficiary = sumTotalBeneficiary + Convert.ToInt32(item.TotalBeneficiary);


                    LocalBeneficiaryFinalizedlst.Add(item.BeneficiaryFinalized);
                    model.BeneficiaryFinalizedArr = LocalBeneficiaryFinalizedlst.ToArray();
                    model.BeneficiaryFinalizedstr = string.Join(", ", model.BeneficiaryFinalizedArr);
                    sumBeneficiaryFinalized = sumBeneficiaryFinalized + Convert.ToInt32(item.BeneficiaryFinalized);


                    LocalBeneficiaryVerifiedlst.Add(item.BeneficiaryVerified);
                    model.BeneficiaryVerifiedArr = LocalBeneficiaryVerifiedlst.ToArray();
                    model.BeneficiaryVerifiedstr = string.Join(", ", model.BeneficiaryVerifiedArr);
                    sumBeneficiaryVerified = sumBeneficiaryVerified + Convert.ToInt32(item.BeneficiaryVerified);

                }

                model.sumTotalBeneficiary = Convert.ToString(sumTotalBeneficiary);
                model.sumBeneficiaryFinalized = Convert.ToString(sumBeneficiaryFinalized);
                model.sumBeneficiaryVerified = Convert.ToString(sumBeneficiaryVerified);

                #endregion



            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MISController.MISDSCBeneficiaryLayout()");
                return null;
            }



            return View(model);
        }
        #endregion

        #region State-wise Data

        /// <summary>
        ///This Method Returns HighCharts for PFMS MIS Payment based on State 
        /// </summary>
        /// <param name="StateShortName"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult ReteriveStateWiseDataForMISPayment(string StateShortName)
        {
            MISDetailModel model = new MISDetailModel();
            PMGSYEntities dbContext = null;

            //Chart 2
            List<string> LocalFinancialYearlst2 = new List<string>();
            List<string> LocalStateChequeAmountlst2 = new List<string>();
            List<string> LocalTotalPaymentMadelst2 = new List<string>();

            //Chart 3
            List<string> LocalMonthlst3 = new List<string>();
            List<string> LocalTotalPaymentMadelst3 = new List<string>();

            //Chart 4
            List<string> LocalMonthlst4 = new List<string>();
            List<string> LocalStateChequeAmountlst4 = new List<string>();

            int decYear = 0;



            try
            {


                dbContext = new PMGSYEntities();

                string FundType = PMGSYSession.Current.FundType;
                int CurrentMonth = DateTime.Now.Month;
                int CurrentYear = DateTime.Now.Year;
                List<USP_RPT_MIS_PAYMENTS_Result> itemList = new List<USP_RPT_MIS_PAYMENTS_Result>();
                int seconds1 = DateTime.Now.Second;
                int minute1 = DateTime.Now.Minute;
                String sec1 = Convert.ToString(seconds1);
                String min1 = Convert.ToString(minute1);
                String combine1 = sec1 + "-" + min1;

                itemList = dbContext.USP_RPT_MIS_PAYMENTS(FundType, CurrentMonth, CurrentYear, 0).ToList<USP_RPT_MIS_PAYMENTS_Result>();
                int seconds12 = DateTime.Now.Second;
                int minute12 = DateTime.Now.Minute;
                String sec12 = Convert.ToString(seconds12);
                String min12 = Convert.ToString(minute12);
                String combine12 = sec12 + "-" + min12;



                if (CurrentMonth <= 3)
                {
                    decYear = CurrentYear - 1;
                }
                else
                {
                    decYear = CurrentYear;
                }



                model.FinancialYearArr2 = new string[itemList.Count];
                model.ChequeAmountArr2 = new string[itemList.Count];
                model.TotalPaymentMadeArr2 = new string[itemList.Count];

                model.TotalPaymentMadeArr3 = new string[itemList.Count];

                model.MonthArr4 = new string[itemList.Count];
                model.ChequeAmountArr4 = new string[itemList.Count];

                #region Chart 2
                //Chart 2 Reload

                List<USP_RPT_MIS_PAYMENTS_Result> FinancialYearChequeAmountlst = new List<USP_RPT_MIS_PAYMENTS_Result>();
                FinancialYearChequeAmountlst = itemList.Where(x => x.MAST_STATE_SHORT_CODE == StateShortName.Trim()).ToList();

                if (FinancialYearChequeAmountlst.Count == 0)
                {
                    return Json(new { Success = false });
                }

                var FinancialYearChequeAmount = (from l in FinancialYearChequeAmountlst
                                                 group l by l.FINANCIAL_YEAR into g
                                                 select new
                                                 {
                                                     StateShortName = g.First().MAST_STATE_SHORT_CODE,
                                                     FINANCIAL_YEAR = g.First().FINANCIAL_YEAR,
                                                     chequeAmount = g.Sum(_ => _.CHQ_AMOUNT).ToString(),
                                                     TotalPaymentmade = g.Sum(x => x.Total_Payments_Made).ToString()

                                                 }).ToList();
                decimal sumChqAmount2 = 0;
                Int32 sumTotalPaymentMade2 = 0;


                //Second Chart
                foreach (var item in FinancialYearChequeAmount)
                {
                    Int32 FinYear = Convert.ToInt32(item.FINANCIAL_YEAR);
                    Int32 IncFinYear = FinYear + 1;
                    string strINCFinYear = Convert.ToString(IncFinYear);
                    string combineFinYear = item.FINANCIAL_YEAR + "-" + strINCFinYear;


                    LocalFinancialYearlst2.Add(combineFinYear);
                    model.FinancialYearArr2 = LocalFinancialYearlst2.ToArray();
                    model.FinancialYearstr2 = string.Join(", ", model.FinancialYearArr2);

                    LocalStateChequeAmountlst2.Add(item.chequeAmount.ToString());
                    model.ChequeAmountArr2 = LocalStateChequeAmountlst2.ToArray();
                    model.ChequeAmountstr2 = string.Join(", ", model.ChequeAmountArr2);

                    //Sum To Show
                    decimal chqAmount2 = Convert.ToDecimal(item.chequeAmount.ToString());
                    sumChqAmount2 = sumChqAmount2 + chqAmount2;



                    LocalTotalPaymentMadelst2.Add(item.TotalPaymentmade.ToString());
                    model.TotalPaymentMadeArr2 = LocalTotalPaymentMadelst2.ToArray();
                    model.TotalPaymentMadestr2 = string.Join(", ", model.TotalPaymentMadeArr2);

                    //Sum to Show
                    Int32 Paymade2 = Convert.ToInt32(item.TotalPaymentmade);
                    sumTotalPaymentMade2 = sumTotalPaymentMade2 + Paymade2;

                }

                model.sumChqAmount2 = String.Format("{0:0.00}", sumChqAmount2);
                model.sumPaymentMade2 = Convert.ToString(sumTotalPaymentMade2);



                #endregion

                #region Chart 3
                List<USP_RPT_MIS_PAYMENTS_Result> YearfilterLst = new List<USP_RPT_MIS_PAYMENTS_Result>();
                YearfilterLst = itemList.Where(x => x.FINANCIAL_YEAR == decYear).ToList();

                List<USP_RPT_MIS_PAYMENTS_Result> monthPaymentMadelst = new List<USP_RPT_MIS_PAYMENTS_Result>();
                monthPaymentMadelst = YearfilterLst.Where(x => x.MAST_STATE_SHORT_CODE == StateShortName.Trim()).ToList();

                if (monthPaymentMadelst.Count == 0)
                {
                    return Json(new { Success = false });
                }


                //Third Chart
                var monthPaymentMade = (from l in monthPaymentMadelst
                                        group l by l.BILL_MONTH into g
                                        select new
                                        {
                                            FinYear = g.First().FINANCIAL_YEAR,
                                            m = g.First().BILL_MONTH,
                                            Months = g.First().BILL_MONTH.ToString(),
                                            TotalPaymentmade = g.Where(x => x.Total_Payments_Made > 0).Average(x => (int?)x.Total_Payments_Made).ToString()

                                        }).OrderBy(x => x.m).ToList();
                monthPaymentMade = monthPaymentMade.Where(x => x.FinYear == decYear).ToList();

                Int32 sumTotalPaymentMade3 = 0;

                //Third Chart

                foreach (var item in monthPaymentMade)
                {
                    Int32 month = Convert.ToInt32(item.Months);
                    string MonthName = dbContext.MASTER_MONTH.Where(x => x.MAST_MONTH_CODE == month).Select(x => x.MAST_MONTH_SHORT_NAME).FirstOrDefault();

                    LocalMonthlst3.Add(MonthName);
                    model.MonthArr3 = LocalMonthlst3.ToArray();
                    model.Monthstr3 = string.Join(", ", model.MonthArr3);

                    string str = String.Empty;
                    if (string.IsNullOrEmpty(item.TotalPaymentmade))
                    {
                        str = "0";
                    }
                    else
                    {
                        decimal paymentMade = Convert.ToDecimal(item.TotalPaymentmade);
                        decimal truncatedpaymentMade = Math.Truncate(paymentMade);
                        str = Convert.ToString(truncatedpaymentMade);
                    }

                    LocalTotalPaymentMadelst3.Add(str);
                    model.TotalPaymentMadeArr3 = LocalTotalPaymentMadelst3.ToArray();
                    model.TotalPaymentMadestr3 = string.Join(", ", model.TotalPaymentMadeArr3);
                    sumTotalPaymentMade3 = sumTotalPaymentMade3 + Convert.ToInt32(str);

                }
                #endregion
                model.sumPaymentMade3 = Convert.ToString(sumTotalPaymentMade3);


                #region Chart 4
                //Fin Year
                //State

                List<USP_RPT_MIS_PAYMENTS_Result> YearwiseFilter = new List<USP_RPT_MIS_PAYMENTS_Result>();
                YearwiseFilter = itemList.Where(x => x.FINANCIAL_YEAR == decYear).ToList();

                List<USP_RPT_MIS_PAYMENTS_Result> MonthChequeAmountlst = new List<USP_RPT_MIS_PAYMENTS_Result>();
                MonthChequeAmountlst = YearwiseFilter.Where(x => x.MAST_STATE_SHORT_CODE == StateShortName.Trim()).ToList();

                if (MonthChequeAmountlst.Count == 0)
                {
                    return Json(new { Success = false });
                }

                var chequeAmountMonth = (from l in MonthChequeAmountlst
                                         group l by l.BILL_MONTH into g
                                         select new
                                         {
                                             FinYear = g.First().FINANCIAL_YEAR,
                                             m = g.First().BILL_MONTH,
                                             Months = g.First().BILL_MONTH.ToString(),
                                             chequeAmount = g.Where(x => x.CHQ_AMOUNT > 0).Average(x => x.CHQ_AMOUNT).ToString()
                                         }).ToList();


                chequeAmountMonth = chequeAmountMonth.Where(x => x.FinYear == decYear).ToList();


                decimal sumChqAmount4 = 0;


                //Fourth Chart
                foreach (var item in chequeAmountMonth)
                {
                    Int32 month = Convert.ToInt32(item.Months);
                    string MonthName = dbContext.MASTER_MONTH.Where(x => x.MAST_MONTH_CODE == month).Select(x => x.MAST_MONTH_SHORT_NAME).FirstOrDefault();

                    LocalMonthlst4.Add(MonthName);
                    model.MonthArr4 = LocalMonthlst4.ToArray();
                    model.Monthstr4 = string.Join(", ", model.MonthArr4);

                    string str = String.Empty;
                    if (string.IsNullOrEmpty(item.chequeAmount))
                    {
                        str = "0";
                    }
                    else
                    {
                        decimal dec = Convert.ToDecimal(item.chequeAmount);
                        string str1 = String.Format("{0:0.00}", dec);
                        str = str1;

                    }
                    LocalStateChequeAmountlst4.Add(str);
                    model.ChequeAmountArr4 = LocalStateChequeAmountlst4.ToArray();
                    model.ChequeAmountstr4 = string.Join(", ", model.ChequeAmountArr4);

                    sumChqAmount4 = sumChqAmount4 + Convert.ToDecimal(str);

                }
                model.sumChqAmount4 = Convert.ToString(sumChqAmount4);

                #endregion


                return Json(new { Success = true, FinancialYearstr2 = model.FinancialYearstr2, ChequeAmountstr2 = model.ChequeAmountstr2, TotalPaymentMadestr2 = model.TotalPaymentMadestr2, sumChqAmount2 = model.sumChqAmount2, sumPaymentMade2 = model.sumPaymentMade2, Monthstr3 = model.Monthstr3, TotalPaymentMadestr3 = model.TotalPaymentMadestr3, sumPaymentMade3 = model.sumPaymentMade3, Monthstr4 = model.Monthstr4, ChequeAmountstr4 = model.ChequeAmountstr4, sumChqAmount4 = model.sumChqAmount4 });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MISController.ReteriveStateWiseDataForMISPayment()");
                return Json(new { Success = false });
            }

        }
        #endregion

        #region Year-wise Data

        /// <summary>
        ///This Method Returns HighCharts for PFMS MIS Payment based on Year/State  
        /// </summary>
        /// <param name="StateShortName"></param>
        /// <returns></returns>
        /// 

        [HttpPost]
        public ActionResult ReteriveYearWiseDataForMISPayment(string Year, string State)
        {
            MISDetailModel model = new MISDetailModel();
            PMGSYEntities dbContext = null;

            //Chart 3
            List<string> LocalMonthlst3 = new List<string>();
            List<string> LocalTotalPaymentMadelst3 = new List<string>();

            //Chart 4
            List<string> LocalMonthlst4 = new List<string>();
            List<string> LocalStateChequeAmountlst4 = new List<string>();
            try
            {
                dbContext = new PMGSYEntities();

                string FundType = PMGSYSession.Current.FundType;
                int CurrentMonth = DateTime.Now.Month;
                int CurrentYear = DateTime.Now.Year;
                List<USP_RPT_MIS_PAYMENTS_Result> itemList = new List<USP_RPT_MIS_PAYMENTS_Result>();
                itemList = dbContext.USP_RPT_MIS_PAYMENTS(FundType, CurrentMonth, CurrentYear, 0).ToList<USP_RPT_MIS_PAYMENTS_Result>();


                string[] splitArr = Year.Split('-');
                string FinancialYear=splitArr[0];
                int finYear = Convert.ToInt32(FinancialYear);

                model.TotalPaymentMadeArr3 = new string[itemList.Count];

                model.MonthArr4 = new string[itemList.Count];
                model.ChequeAmountArr4 = new string[itemList.Count];


                #region Chart 3

                //Filter :Fin Year wise

                List<USP_RPT_MIS_PAYMENTS_Result> yearFilterLst = new List<USP_RPT_MIS_PAYMENTS_Result>();


                //First Time
                if (State.Trim() == "s")
                {
                    yearFilterLst = itemList.Where(x => x.FINANCIAL_YEAR == finYear).ToList();
                }
                else
                {
                    string shortCode = Convert.ToString(State);
                    yearFilterLst = itemList.Where(x => x.FINANCIAL_YEAR == finYear && x.MAST_STATE_SHORT_CODE == shortCode.Trim()).ToList();
                }

                if (yearFilterLst.Count == 0)
                {
                    return Json(new { Success = false });
                }

                //Third Chart
                var monthPaymentMade = (from l in yearFilterLst
                                        group l by l.BILL_MONTH into g
                                        select new
                                        {
                                            FinYear = g.First().FINANCIAL_YEAR,
                                            m = g.First().BILL_MONTH,
                                            Months = g.First().BILL_MONTH.ToString(),
                                            TotalPaymentmade = g.Where(x => x.Total_Payments_Made > 0).Average(x => (int?)x.Total_Payments_Made).ToString()

                                        }).OrderBy(x => x.m).ToList();
                monthPaymentMade = monthPaymentMade.Where(x => x.FinYear == finYear).ToList();


                Int32 sumTotalPaymentMade3 = 0;


                //Third Chart

                foreach (var item in monthPaymentMade)
                {
                    Int32 month = Convert.ToInt32(item.Months);
                    string MonthName = dbContext.MASTER_MONTH.Where(x => x.MAST_MONTH_CODE == month).Select(x => x.MAST_MONTH_SHORT_NAME).FirstOrDefault();

                    LocalMonthlst3.Add(MonthName);
                    model.MonthArr3 = LocalMonthlst3.ToArray();
                    model.Monthstr3 = string.Join(", ", model.MonthArr3);

                    string str = String.Empty;
                    if (string.IsNullOrEmpty(item.TotalPaymentmade))
                    {
                        str = "0";
                    }
                    else
                    {
                        decimal paymentMade = Convert.ToDecimal(item.TotalPaymentmade);
                        decimal truncatedpaymentMade = Math.Truncate(paymentMade);
                        str = Convert.ToString(truncatedpaymentMade);
                    }

                    LocalTotalPaymentMadelst3.Add(str);
                    model.TotalPaymentMadeArr3 = LocalTotalPaymentMadelst3.ToArray();
                    model.TotalPaymentMadestr3 = string.Join(", ", model.TotalPaymentMadeArr3);
                    sumTotalPaymentMade3 = sumTotalPaymentMade3 + Convert.ToInt32(str);

                }
                model.sumPaymentMade3 = Convert.ToString(sumTotalPaymentMade3);

                #endregion

                #region Chart 4

                //Fourth Chart
                List<USP_RPT_MIS_PAYMENTS_Result> MonthChequeAmountlst = new List<USP_RPT_MIS_PAYMENTS_Result>();
                MonthChequeAmountlst = itemList;   // itemList.Where(x => x.MAST_STATE_SHORT_CODE == StateShortName.Trim()).ToList();

                List<USP_RPT_MIS_PAYMENTS_Result> yearFilterLst1 = new List<USP_RPT_MIS_PAYMENTS_Result>();

                //First Time
                if (State.Trim() == "s")
                {
                    yearFilterLst1 = itemList.Where(x => x.FINANCIAL_YEAR == finYear).ToList();
                }
                else
                {
                    string shortCode1 = Convert.ToString(State);
                    yearFilterLst1 = itemList.Where(x => x.FINANCIAL_YEAR == finYear && x.MAST_STATE_SHORT_CODE == shortCode1.Trim()).ToList();
                }

                if (yearFilterLst1.Count == 0)
                {
                    return Json(new { Success = false });
                }

                

                var chequeAmountMonth = (from l in yearFilterLst1
                                         group l by l.BILL_MONTH into g
                                         select new
                                         {
                                             FinYear = g.First().FINANCIAL_YEAR,
                                             m = g.First().BILL_MONTH,
                                             Months = g.First().BILL_MONTH.ToString(),
                                             chequeAmount = g.Where(x => x.CHQ_AMOUNT > 0).Average(x => x.CHQ_AMOUNT).ToString()
                                         }).ToList();


                chequeAmountMonth = chequeAmountMonth.Where(x => x.FinYear == finYear).ToList();

                decimal sumChqAmount4 = 0;

                //Fourth Chart
                foreach (var item in chequeAmountMonth)
                {
                    Int32 month = Convert.ToInt32(item.Months);
                    string MonthName = dbContext.MASTER_MONTH.Where(x => x.MAST_MONTH_CODE == month).Select(x => x.MAST_MONTH_SHORT_NAME).FirstOrDefault();

                    LocalMonthlst4.Add(MonthName);
                    model.MonthArr4 = LocalMonthlst4.ToArray();
                    model.Monthstr4 = string.Join(", ", model.MonthArr4);

                    string str = String.Empty;
                    if (string.IsNullOrEmpty(item.chequeAmount))
                    {
                        str = "0";
                    }
                    else
                    {
                        decimal dec = Convert.ToDecimal(item.chequeAmount);
                        string str1 = String.Format("{0:0.00}", dec);
                        str = str1;

                    }
                    LocalStateChequeAmountlst4.Add(str);
                    model.ChequeAmountArr4 = LocalStateChequeAmountlst4.ToArray();
                    model.ChequeAmountstr4 = string.Join(", ", model.ChequeAmountArr4);

                    sumChqAmount4 = sumChqAmount4 + Convert.ToDecimal(str);

                }

                model.sumChqAmount4 = Convert.ToString(sumChqAmount4);

                #endregion



                return Json(new { Success = true, Monthstr3 = model.Monthstr3, TotalPaymentMadestr3 = model.TotalPaymentMadestr3, sumPaymentMade3 = model.sumPaymentMade3, Monthstr4 = model.Monthstr4, ChequeAmountstr4 = model.ChequeAmountstr4, sumChqAmount4 = model.sumChqAmount4 });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MISController.ReteriveYearWiseDataForMISPayment()");
                return Json(new { Success = false });
            }


        }
        #endregion

        #endregion
    }
}
