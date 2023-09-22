#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   DashboardBAL.cs        
        * Description   :   Business Logic for Financial Report, Status Monitoring, General Information, 
        *                   Balancesheet, Annual account, Cummulative Expenditure, Implementation Summary .
        * Author        :   Shyam Yadav 
        * Creation Date :   20/Sep/2013
 **/
#endregion


using PMGSY.DAL.Dashboard;
using PMGSY.Models;
using PMGSY.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.Dashboard
{
    public class DashboardBAL : IDashboardBAL
    {
        private IDashboardDAL dashboardDAL;

        #region Financial Details

        /// <summary>
        /// Fund Vs Expenditure
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="fundType"></param>
        /// <returns></returns>
        public Array FundVsExpenditureBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string fundType)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.FundVsExpenditureDAL(page, rows, sidx, sord, out totalRecords, fundType);
        }


        /// <summary>
        /// Expenditure Summary
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="fundType"></param>
        /// <param name="stateNDCode"></param>
        /// <param name="piuNDCode"></param>
        /// <param name="fundingAgency"></param>
        /// <returns></returns>
        public Array ExpenditureSummaryBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string fundType, int stateNDCode, int piuNDCode, int fundingAgency)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.ExpenditureSummaryDAL(page, rows, sidx, sord, out totalRecords, fundType, stateNDCode, piuNDCode, fundingAgency);
        }


        /// <summary>
        /// Fund Wise Status Monitoring Details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateNDCode"></param>
        /// <param name="year"></param>
        /// <param name="durationType"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public Array StatusMonitoringReportBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string fundType)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.StatusMonitoringReportDAL(page, rows, sidx, sord, out totalRecords, fundType);
        }


        /// <summary>
        /// PIU wise Authorization Received & Expenditure
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateNDCode"></param>
        /// <param name="fundType"></param>
        /// <returns></returns>
        public Array StatusMonitoringPIUReportBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateNDCode, string fundType)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.StatusMonitoringPIUReportDAL(page, rows, sidx, sord, out totalRecords, stateNDCode, fundType);
        }


        /// <summary>
        /// Yearwise & Cumulative Expenditure Details
        /// </summary>
        /// <param name="stateNDCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="piuCode"></param>
        /// <param name="collaboration"></param>
        /// <param name="fundType"></param>
        /// <returns></returns>
        public List<USP_ACC_DB_Expn_Trend_Result> ExpenditureTrendBAL(string fundType, int stateNDCode, int piuCode, int collaboration)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.ExpenditureTrendDAL(fundType, stateNDCode, piuCode, collaboration);
        }


        #endregion


        #region Physical(Technical) Details

        /// <summary>
        /// All States Technical Details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="fundingAgency"></param>
        /// <returns></returns>
        public Array AllStatesTechnicalDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int fundingAgency)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.AllStatesTechnicalDetailsDAL(page, rows, sidx, sord, out totalRecords, fundingAgency);
        }

        
        /// <summary>
        /// Technical Details of all Districts under particular State
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="fundingAgency"></param>
        /// <returns></returns>
        public Array DistrictwiseTechnicalDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int fundingAgency)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.DistrictwiseTechnicalDetailsDAL(page, rows, sidx, sord, out totalRecords, stateCode, fundingAgency);
        }


        /// <summary>
        /// Technical Details of all Blocks under particular District
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="fundingAgency"></param>
        /// <returns></returns>
        public Array BlockwiseTechnicalDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int fundingAgency)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.BlockwiseTechnicalDetailsDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, fundingAgency);
        }


        ///// <summary>
        ///// Sanction Vs Awarded vs Completed vs Progress Works Details
        ///// </summary>
        ///// <param name="fundingAgency"></param>
        ///// <returns></returns>
        //public List<USP_TECH_DASH_R1_Result> WorksColumnChartBAL(int stateCode, int districtCode, int fundingAgency)
        //{
        //    dashboardDAL = new DashboardDAL();
        //    return dashboardDAL.WorksColumnChartDAL(stateCode, districtCode, fundingAgency);
        //}


        ///// <summary>
        ///// Sanction Vs Awarded vs Completed vs Progress Length Details
        ///// </summary>
        ///// <param name="fundingAgency"></param>
        ///// <returns></returns>
        //public List<LengthColumnChartStoredProcModel> LengthColumnChartBAL(int stateCode, int districtCode, int fundingAgency)
        //{
        //    dashboardDAL = new DashboardDAL();
        //    return dashboardDAL.LengthColumnChartDAL(stateCode, districtCode, fundingAgency);
        //}


        ///// <summary>
        ///// Column chart for Sanction Cost, Awarded Cost, Maintenance Expenditure
        ///// </summary>
        ///// <param name="stateCode"></param>
        ///// <param name="districtCode"></param>
        ///// <param name="fundingAgency"></param>
        ///// <returns></returns>
        //public List<CostColumnChartStoredProcModel> CostColumnChartBAL(int stateCode, int districtCode, int fundingAgency)
        //{
        //    dashboardDAL = new DashboardDAL();
        //    return dashboardDAL.CostColumnChartDAL(stateCode, districtCode, fundingAgency);
        //}

        public Array WorkLenghtExpYearWiseStateWiseGrid(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int fundingAgency, int year, bool isYearWise)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.WorkLenghtExpYearWiseStateWiseGrid(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, fundingAgency,year,isYearWise);
        }

        /// <summary>
        /// Added by Abhishek kamble 12Mar2015
        /// Column chart for work,Lenght and Expenditure.
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="fundingAgency"></param>
        /// <returns></returns>
        public List<USP_DSS_ACHIEVEMENT_REPORT_Result> WorksLengthExpYearWiseStateWiseColumnChartDAL(int stateCode, int districtCode, int fundingAgency,int year, bool isYearWise)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.WorksLengthExpYearWiseStateWiseColumnChartDAL(stateCode, districtCode, fundingAgency, year,isYearWise);
        }

        #endregion


        #region Quality Details

        public Array AllStatesInspectionDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.AllStatesInspectionDetailsDAL(page, rows, sidx, sord, out totalRecords);
        }

        public Array DistrictwiseInspectionDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.DistrictwiseInspectionDetailsDAL(page, rows, sidx, sord, out totalRecords, stateCode);
        }

        public Array MonitorwiseComplAndProgressDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string qmType)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.MonitorwiseComplAndProgressDetailsDAL(page, rows, sidx, sord, out totalRecords, stateCode,qmType);
        }

        public List<AllStatesQualityPieChartModel> AllStatesGradingPieChartBAL(int stateCode, string qmType)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.AllStatesGradingPieChartDAL(stateCode, qmType);
        }

        public List<USP_QM_STATE_DISTRICT_DASH_S1_Result> YearlyGradingLineChartBAL(int stateCode)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.YearlyGradingLineChartDAL(stateCode);
        }


        public List<USP_QM_STATE_DISTRICT_DASH_S3_Result> MonitorsGradingColumnChartBAL(int stateCode, int monitorCode, string qmType)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.MonitorsGradingColumnChartDAL(stateCode, monitorCode, qmType);
        }
        
        #endregion


        #region NRRDA_DASHBOARDS

        public List<USP_TECH_TRENDS_CHART_PPT_Result> GetProposalTrendsDetailsBAL(string stateList, int year, int reportType, int agency, int collaboration)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.GetProposalTrendsDetailsDAL(stateList,year,reportType,agency,collaboration);
        }

        public List<USP_PROP_HAB_PPT_LIST_Result> GetHabitationDetailsMPRBAL(string stateList, int year, int currentMonth, int agency, int collaboration)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.GetHabitationDetailsMPRDAL(stateList, year, currentMonth,agency,collaboration);
        }

        public List<USP_PROP_MAINT_PPT_LIST_Result> GetMaintenanceDetailsMPRBAL(string stateList, int year, int currentMonth, int agency, int collaboration)
        {
            dashboardDAL = new DashboardDAL();
            return dashboardDAL.GetMaintenanceDetailsMPRDAL(stateList, year, currentMonth,agency,collaboration);
        }

        #endregion

    }



    public interface IDashboardBAL
    {

        #region Financial Details
        
        Array FundVsExpenditureBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string fundType);
        Array ExpenditureSummaryBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string fundType, int stateNDCode, int piuNDCode, int fundingAgency);
        Array StatusMonitoringReportBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string fundType);
        Array StatusMonitoringPIUReportBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateNDCode, string fundType);
        List<USP_ACC_DB_Expn_Trend_Result> ExpenditureTrendBAL(string fundType, int stateNDCode, int piuCode, int collaboration);

        #endregion


        #region Physical(Technical) Details

        Array AllStatesTechnicalDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int fundingAgency);
        Array DistrictwiseTechnicalDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int fundingAgency);
        Array BlockwiseTechnicalDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int fundingAgency);
        //List<USP_TECH_DASH_R1_Result> WorksColumnChartBAL(int stateCode, int districtCode, int fundingAgency);
        //List<LengthColumnChartStoredProcModel> LengthColumnChartBAL(int stateCode, int districtCode, int fundingAgency);
        //List<CostColumnChartStoredProcModel> CostColumnChartBAL(int stateCode, int districtCode, int fundingAgency);
        Array WorkLenghtExpYearWiseStateWiseGrid(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int fundingAgency, int year, bool isYearWise);
        List<USP_DSS_ACHIEVEMENT_REPORT_Result> WorksLengthExpYearWiseStateWiseColumnChartDAL(int stateCode, int districtCode, int fundingAgency,int year, bool isYearWise);
        #endregion


        #region Quality Details

        Array AllStatesInspectionDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array DistrictwiseInspectionDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode);
        Array MonitorwiseComplAndProgressDetailsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string qmType);
        List<AllStatesQualityPieChartModel> AllStatesGradingPieChartBAL(int stateCode, string qmType);
        List<USP_QM_STATE_DISTRICT_DASH_S1_Result> YearlyGradingLineChartBAL(int stateCode);
        List<USP_QM_STATE_DISTRICT_DASH_S3_Result> MonitorsGradingColumnChartBAL(int stateCode, int monitorCode, string qmType);
        #endregion


        #region NRRDA_DASHBOARDS

        List<USP_TECH_TRENDS_CHART_PPT_Result> GetProposalTrendsDetailsBAL(string stateList,int year,int reportType,int agency,int collaboration);
        List<USP_PROP_HAB_PPT_LIST_Result> GetHabitationDetailsMPRBAL(string stateList, int year, int currentMonth, int agency, int collaboration);
        List<USP_PROP_MAINT_PPT_LIST_Result> GetMaintenanceDetailsMPRBAL(string stateList, int year, int currentMonth, int agency, int collaboration);

        #endregion
    }
}