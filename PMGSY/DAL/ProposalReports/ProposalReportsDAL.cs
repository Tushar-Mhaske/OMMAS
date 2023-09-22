using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.ProposalReports;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;

namespace PMGSY.DAL.ProposalReports
{
    public class ProposalReportsDAL : IProposalReportsDAL
    {
        PMGSY.Models.PMGSYEntities dbContext;

        #region
        public PMGSY.Models.ProposalReports.MRDProposalModel GetMRDProposalHabCovgDAL(PMGSY.Models.ProposalReports.MRDProposalModel mrd)
        {
            try
            {

                dbContext = new PMGSYEntities();

                List<USP_MRD_HAB_COVERAGE_Result> lstMRDProposal = new List<USP_MRD_HAB_COVERAGE_Result>();

                mrd.PackageCode = mrd.PackageCode == "0" ? "%" : mrd.PackageCode;
                mrd.STAStatusCode = mrd.STAStatusCode == "0" ? "%" : mrd.STAStatusCode;
                mrd.CollabCode = mrd.CollabCode == -1 ? 0 : mrd.CollabCode;
                mrd.CategoryCode = mrd.CategoryCode == "0" ? "%" : mrd.CategoryCode;

                mrd.Level = 1;
                //mrd.BlockCode = 0;
                //mrd.Agency = 0;
                mrd.PTAStatus = "%";
                mrd.MRDStatus = mrd.StatusCode == "A" ? "%" : mrd.StatusCode;
                mrd.PMGSY = PMGSY.Extensions.PMGSYSession.Current.PMGSYScheme;


                //set Agency Name And DPIU name
                mrd.lstMRDProposalHab = dbContext.Database.SqlQuery<USP_MRD_HAB_COVERAGE_Result>("EXEC [omms].[USP_MRD_HAB_COVERAGE] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Agency,@Package,@PTAStatus,@STAStatus,@MRDStatus,@Type,@Connect,@PMGSY",
                new SqlParameter("@Level", mrd.Level),
                new SqlParameter("@State", mrd.StateCode),
                new SqlParameter("@District", mrd.DistrictCode),
                new SqlParameter("@Block", mrd.BlockCode),
                new SqlParameter("@Year", mrd.Year),
                new SqlParameter("@Batch", mrd.BatchCode),
                new SqlParameter("@Collaboration", mrd.CollabCode),
                new SqlParameter("@Agency", mrd.Agency),
                new SqlParameter("@Package", mrd.PackageCode),
                new SqlParameter("@PTAStatus", mrd.PTAStatus),
                new SqlParameter("@STAStatus", mrd.STAStatusCode),
                new SqlParameter("@MRDStatus", mrd.MRDStatus),
                new SqlParameter("@Type", mrd.ProposalCode),
                new SqlParameter("@Connect", mrd.CategoryCode),
                new SqlParameter("@PMGSY", mrd.PMGSY)
            ).ToList<USP_MRD_HAB_COVERAGE_Result>();
                //.OrderBy(m => m.MAST_DISTRICT_CODE).ThenBy(m => m.MAST_BLOCK_CODE).ThenBy(m => m.IMS_PACKAGE_ID).ThenBy(m => m.ROAD_NAME).ThenBy(m => m.IMS_YEAR)
                //.ThenBy(m => m.IMS_UPGRADE_CONNECT).ThenBy(m => m.TOTAL_HABS).ThenBy(m => m.HAB_NAME).ThenBy(m => m.HAB_POP).ThenBy(m => m.SCST_POP).ThenBy(m => m.IMS_CC_LENGTH)
                //.ThenBy(m => m.IMS_BT_LENGTH).ThenBy(m => m.ROAD_LENGTH).ThenBy(m => m.PAV_COST).ThenBy(m => m.PW_AVG_KM).ThenBy(m => m.IMS_NO_OF_CDWORKS).ThenBy(m => m.CD_COST)
                //.ThenBy(m => m.AVG_CD_COST).ThenBy(m => m.PW_COST).ThenBy(m => m.OW_COST).ThenBy(m => m.ROAD_AMT).ThenBy(m => m.ROAD_STATE).ThenBy(m => m.TOTAL_COST)
                //.ThenBy(m => m.AVG_COST_KM).ThenBy(m => m.STAGE_CONST).ThenBy(m => m.MP_CONST).ThenBy(m => m.MLA_CONST).ThenBy(m => m.IMS_ZP_RESO_OBTAINED).ThenBy(m => m.IMS_BATCH)
                //.ThenBy(m => m.IMS_COLLABORATION).ThenBy(m => m.MAST_CARRIAGE_WIDTH).ThenBy(m => m.IMS_PROPOSED_SURFACE).ThenBy(m => m.IMS_TRAFFIC_TYPE).ThenBy(m => m.CN_ROAD).ThenBy(m => m.MAINT_AMT)
                //.ThenBy(m => m.TRAFFIC_YEAR).ThenBy(m => m.AADT).ThenBy(m => m.ESAL).ThenBy(m => m.CBR).ThenBy(m => m.CBR).ThenBy(m => m.STA_SCRUTINY)
                //.ThenBy(m => m.PTA_SCRUTINY).ThenBy(m => m.PROPOSAL_STATUS).ThenBy(m => m.IMS_SANCTIONED_DATE).ThenBy(m => m.IMS_STA_REMARKS).ThenBy(m => m.IMS_PTA_REMARKS).ThenBy(m => m.IMS_REMARKS).ToList<USP_MRD_PROPOSAL_REPORT_Result>();

                return mrd;
                //}
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        public List<PMGSY.Models.ProposalReports.RoadCBRListing> GetMRDProposalRoadCBRDetailsDAL(int roadCode)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                var itemList = (from s in dbContext.IMS_CBR_VALUE
                                where (s.IMS_PR_ROAD_CODE == roadCode)
                                orderby s.IMS_PR_ROAD_CODE
                                select new
                                {
                                    s.IMS_SEGMENT_NO,
                                    s.IMS_STR_CHAIN,
                                    s.IMS_END_CHAIN,
                                    s.IMS_CBR_VALUE1,
                                }).ToList();

                List<RoadCBRListing> lst = new List<RoadCBRListing>();
                foreach (var item in itemList)
                {
                    lst.Add(new RoadCBRListing() { IMS_SEGMENT_NO = Convert.ToString(item.IMS_SEGMENT_NO), IMS_STR_CHAIN = Convert.ToString(item.IMS_STR_CHAIN), IMS_END_CHAIN = Convert.ToString(item.IMS_END_CHAIN), SEGMENT_LENGTH = Convert.ToString(item.IMS_STR_CHAIN + item.IMS_END_CHAIN), IMS_CBR_VALUE = Convert.ToString(item.IMS_CBR_VALUE1) });
                }
                return lst;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public List<PMGSY.Models.ProposalReports.BridgeEstCostListing> GetMRDProposalBridgeEstCostDetailsDAL(int roadCode)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                var itemList = (from s in dbContext.IMS_LSB_BRIDGE_DETAIL
                                where (s.IMS_PR_ROAD_CODE == roadCode)
                                orderby s.IMS_PR_ROAD_CODE
                                select new
                                {
                                    s.IMS_BRGD_STRUCTURE_COST,
                                    s.IMS_STRUCTURE_COST,
                                    s.IMS_BRGD_OTHER_COST,
                                    s.IMS_APPROACH_PER_MTR,
                                    s.IMS_BRGD_STRUCTURE_PER_MTR,
                                    s.IMS_STRUCTURE_PER_MTR,
                                    s.IMS_BRGD_OTHER_PER_MTR
                                }).ToList();

                List<BridgeEstCostListing> lst = new List<BridgeEstCostListing>();
                foreach (var item in itemList)
                {
                    lst.Add(new BridgeEstCostListing() { IMS_BRGD_STRUCTURE_COST = Convert.ToString(item.IMS_BRGD_STRUCTURE_COST), IMS_STRUCTURE_COST = Convert.ToString(item.IMS_STRUCTURE_COST), IMS_BRGD_OTHER_COST = Convert.ToString(item.IMS_BRGD_OTHER_COST), IMS_APPROACH_PER_MTR = Convert.ToString(item.IMS_APPROACH_PER_MTR), IMS_BRGD_STRUCTURE_PER_MTR = Convert.ToString(item.IMS_BRGD_STRUCTURE_PER_MTR), IMS_STRUCTURE_PER_MTR = Convert.ToString(item.IMS_STRUCTURE_PER_MTR), IMS_BRGD_OTHER_PER_MTR = Convert.ToString(item.IMS_BRGD_OTHER_PER_MTR) });
                }
                return lst;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<PMGSY.Models.ProposalReports.BridgeCostListing> GetMRDProposalBridgeCostDetailsDAL(int roadCode)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                var itemList = (from s in dbContext.IMS_SANCTIONED_PROJECTS
                                where (s.IMS_PR_ROAD_CODE == roadCode)
                                orderby s.IMS_PR_ROAD_CODE
                                select new
                                {
                                    s.IMS_MAINTENANCE_YEAR1,
                                    s.IMS_MAINTENANCE_YEAR2,
                                    s.IMS_MAINTENANCE_YEAR3,
                                    s.IMS_MAINTENANCE_YEAR4,
                                    s.IMS_MAINTENANCE_YEAR5,
                                    s.IMS_RENEWAL_COST,
                                    s.IMS_SANCTIONED_RS_AMT
                                }).ToList();

                List<BridgeCostListing> lst = new List<BridgeCostListing>();
                foreach (var item in itemList)
                {
                    lst.Add(new BridgeCostListing() { Year1 = Convert.ToString(item.IMS_MAINTENANCE_YEAR1), Year2 = Convert.ToString(item.IMS_MAINTENANCE_YEAR2), Year3 = Convert.ToString(item.IMS_MAINTENANCE_YEAR3), Year4 = Convert.ToString(item.IMS_MAINTENANCE_YEAR4), Year5 = Convert.ToString(item.IMS_MAINTENANCE_YEAR5), IMS_RENEWAL_COST = Convert.ToString(item.IMS_RENEWAL_COST), TotalCost = Convert.ToString(item.IMS_SANCTIONED_RS_AMT) });
                }
                return lst;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //MRDProposal Report
        public List<PMGSY.Models.ProposalReports.BridgeListing> GetMRDProposalBridgeDetailsDAL(int roadCode)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                var itemList = (from s in dbContext.IMS_LSB_BRIDGE_DETAIL
                                where (s.IMS_PR_ROAD_CODE == roadCode)
                                orderby s.IMS_PR_ROAD_CODE
                                select new
                                {
                                    s.IMS_ROAD_TYPE_LEVEL,
                                    s.IMS_AVERAGE_GROUND_LEVEL,
                                    s.IMS_NALA_BED_LEVEL,
                                    s.IMS_HIGHEST_FLOOD_LEVEL,
                                    s.IMS_ORDINARY_FLOOD_LEVEL,
                                    s.IMS_FOUNDATION_LEVEL,
                                    s.IMS_HGT_BIRDGE_NBL,
                                    s.IMS_HGT_BRIDGE_FL,
                                    s.IMS_BRG_SUBMERSIBLE,
                                    s.IMS_BRG_BOX_CULVERT,
                                    s.IMS_BRG_RCC_ABUMENT,
                                    s.IMS_BRG_HLB,
                                    s.MASTER_SCOUR_FOUNDATION_TYPE.IMS_SC_FD_NAME,
                                    s.IMS_BEARING_CAPACITY,
                                    s.IMS_ARG_TOT_SPANS,
                                    s.IMS_NO_OF_VENTS,
                                    s.IMS_SPAN_VENT,
                                    s.IMS_SCOUR_DEPTH,
                                    s.IMS_WIDTH_OF_BRIDGE
                                }).ToList();

                List<BridgeListing> lst = new List<BridgeListing>();
                foreach (var item in itemList)
                {
                    //string fndType = "";
                    //switch (item.IMS_SC_FD_CODE)
                    //{
                    //    case 1:
                    //        fndType = "Open";
                    //        break;
                    //    case 2:
                    //        fndType = "Raft";
                    //        break;
                    //    case 3:
                    //        fndType = "Well";
                    //        break;
                    //    case 4:
                    //        fndType = "Pile";
                    //        break;
                    //    case 5:
                    //        fndType = "Any Other";
                    //        break;
                    //}
                    lst.Add(new BridgeListing() { IMS_ROAD_TYPE_LEVEL = Convert.ToString(item.IMS_ROAD_TYPE_LEVEL).Trim(), IMS_AVERAGE_GROUND_LEVEL = Convert.ToString(item.IMS_AVERAGE_GROUND_LEVEL), IMS_NALA_BED_LEVEL = Convert.ToString(item.IMS_NALA_BED_LEVEL).Trim(), IMS_HIGHEST_FLOOD_LEVEL = Convert.ToString(item.IMS_HIGHEST_FLOOD_LEVEL), IMS_ORDINARY_FLOOD_LEVEL = Convert.ToString(item.IMS_ORDINARY_FLOOD_LEVEL), IMS_FOUNDATION_LEVEL = Convert.ToString(item.IMS_FOUNDATION_LEVEL), IMS_HGT_BIRDGE_NBL = Convert.ToString(item.IMS_HGT_BIRDGE_NBL), IMS_HGT_BRIDGE_FL = Convert.ToString(item.IMS_HGT_BRIDGE_FL), IMS_BRG_SUBMERSIBLE = Convert.ToString(item.IMS_BRG_SUBMERSIBLE), IMS_BRG_BOX_CULVERT = Convert.ToString(item.IMS_BRG_BOX_CULVERT), IMS_BRG_RCC_ABUMENT = Convert.ToString(item.IMS_BRG_RCC_ABUMENT), IMS_BRG_HLB = Convert.ToString(item.IMS_BRG_HLB), IMS_SC_FD_CODE = Convert.ToString(item.IMS_SC_FD_NAME), IMS_BEARING_CAPACITY = Convert.ToString(item.IMS_BEARING_CAPACITY), IMS_ARG_TOT_SPANS = Convert.ToString(item.IMS_FOUNDATION_LEVEL), IMS_NO_OF_VENTS = Convert.ToString(item.IMS_NO_OF_VENTS), IMS_SPAN_VENT = Convert.ToString(item.IMS_SPAN_VENT), IMS_SCOUR_DEPTH = Convert.ToString(item.IMS_SCOUR_DEPTH), IMS_WIDTH_OF_BRIDGE = Convert.ToString(item.IMS_WIDTH_OF_BRIDGE) });
                }
                return lst;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //MRDProposal Report
        #region MRDProposal Report
        public PMGSY.Models.ProposalReports.MRDProposalModel GetMRDProposalDAL(PMGSY.Models.ProposalReports.MRDProposalModel mrd)
        {
            try
            {
                //using (var scope = new TransactionScope())
                //{
                dbContext = new PMGSYEntities();
                //PMGSY.Models.ProposalReports.MRDProposalModel mrd = new PMGSY.Models.ProposalReports.MRDProposalModel();
                List<USP_MRD_PROPOSAL_REPORT_Result> lstMRDProposal = new List<USP_MRD_PROPOSAL_REPORT_Result>();

                mrd.PackageCode = mrd.PackageCode == "0" ? "%" : mrd.PackageCode;
                mrd.STAStatusCode = mrd.STAStatusCode == "0" ? "%" : mrd.STAStatusCode;
                mrd.CollabCode = mrd.CollabCode == -1 ? 0 : mrd.CollabCode;
                mrd.CategoryCode = mrd.CategoryCode == "0" ? "%" : mrd.CategoryCode;

                mrd.Level = 1;
                //mrd.BlockCode = 0;
                //mrd.Agency = 0;
                mrd.PTAStatus = "%";
                mrd.MRDStatus = mrd.StatusCode == "A" ? "%" : mrd.StatusCode;
                mrd.PMGSY = PMGSY.Extensions.PMGSYSession.Current.PMGSYScheme;


                //short LevelId = 0;
                //if (objParam.Selection == "D" && objParam.Dpiu != 0)
                //{
                //    LevelId = 5;
                //}
                //else
                //{
                //    if (PMGSYSession.Current.LevelId == 6)
                //    {
                //        LevelId = 4;
                //    }
                //    else
                //    {
                //        LevelId = PMGSYSession.Current.LevelId;
                //    }
                //}

                //set Agency Name And DPIU name
                mrd.lstMRDProposal = dbContext.Database.SqlQuery<USP_MRD_PROPOSAL_REPORT_Result>("EXEC [omms].[USP_MRD_PROPOSAL_REPORT] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Agency,@Package,@PTAStatus,@STAStatus,@MRDStatus,@Type,@Connect,@PMGSY",
                new SqlParameter("@Level", mrd.Level),
                new SqlParameter("@State", mrd.StateCode),
                new SqlParameter("@District", mrd.DistrictCode),
                new SqlParameter("@Block", mrd.BlockCode),
                new SqlParameter("@Year", mrd.Year),
                new SqlParameter("@Batch", mrd.BatchCode),
                new SqlParameter("@Collaboration", mrd.CollabCode),
                new SqlParameter("@Agency", mrd.Agency),
                new SqlParameter("@Package", mrd.PackageCode),
                new SqlParameter("@PTAStatus", mrd.PTAStatus),
                new SqlParameter("@STAStatus", mrd.STAStatusCode),
                new SqlParameter("@MRDStatus", mrd.MRDStatus),
                new SqlParameter("@Type", mrd.ProposalCode),
                new SqlParameter("@Connect", mrd.CategoryCode),
                new SqlParameter("@PMGSY", mrd.PMGSY)
            ).ToList<USP_MRD_PROPOSAL_REPORT_Result>();
                //.OrderBy(m => m.MAST_DISTRICT_CODE).ThenBy(m => m.MAST_BLOCK_CODE).ThenBy(m => m.IMS_PACKAGE_ID).ThenBy(m => m.ROAD_NAME).ThenBy(m => m.IMS_YEAR)
                //.ThenBy(m => m.IMS_UPGRADE_CONNECT).ThenBy(m => m.TOTAL_HABS).ThenBy(m => m.HAB_NAME).ThenBy(m => m.HAB_POP).ThenBy(m => m.SCST_POP).ThenBy(m => m.IMS_CC_LENGTH)
                //.ThenBy(m => m.IMS_BT_LENGTH).ThenBy(m => m.ROAD_LENGTH).ThenBy(m => m.PAV_COST).ThenBy(m => m.PW_AVG_KM).ThenBy(m => m.IMS_NO_OF_CDWORKS).ThenBy(m => m.CD_COST)
                //.ThenBy(m => m.AVG_CD_COST).ThenBy(m => m.PW_COST).ThenBy(m => m.OW_COST).ThenBy(m => m.ROAD_AMT).ThenBy(m => m.ROAD_STATE).ThenBy(m => m.TOTAL_COST)
                //.ThenBy(m => m.AVG_COST_KM).ThenBy(m => m.STAGE_CONST).ThenBy(m => m.MP_CONST).ThenBy(m => m.MLA_CONST).ThenBy(m => m.IMS_ZP_RESO_OBTAINED).ThenBy(m => m.IMS_BATCH)
                //.ThenBy(m => m.IMS_COLLABORATION).ThenBy(m => m.MAST_CARRIAGE_WIDTH).ThenBy(m => m.IMS_PROPOSED_SURFACE).ThenBy(m => m.IMS_TRAFFIC_TYPE).ThenBy(m => m.CN_ROAD).ThenBy(m => m.MAINT_AMT)
                //.ThenBy(m => m.TRAFFIC_YEAR).ThenBy(m => m.AADT).ThenBy(m => m.ESAL).ThenBy(m => m.CBR).ThenBy(m => m.CBR).ThenBy(m => m.STA_SCRUTINY)
                //.ThenBy(m => m.PTA_SCRUTINY).ThenBy(m => m.PROPOSAL_STATUS).ThenBy(m => m.IMS_SANCTIONED_DATE).ThenBy(m => m.IMS_STA_REMARKS).ThenBy(m => m.IMS_PTA_REMARKS).ThenBy(m => m.IMS_REMARKS).ToList<USP_MRD_PROPOSAL_REPORT_Result>();

                //list of Payment
                //lstPaymentCB = dbContext.SP_ACC_CASHBOOK_PAYMENT_SIDE(objParam.FundType, objParam.AdminNdCode, objParam.Month, objParam.Year, LevelId).ToList<SP_ACC_CASHBOOK_PAYMENT_SIDE_Result>();

                //cbSingle.SingleCB.ListPaymentCB = lstPaymentCB;

                //cbSingle.TotalPayCash = cbSingle.SingleCB.ListPaymentCB.Sum(m => m.cash);
                //cbSingle.TotalPayBank = cbSingle.SingleCB.ListPaymentCB.Sum(m => m.bank_auth);

                //cbSingle.Month = objParam.Month;
                //cbSingle.Year = objParam.Year;

                ////Report Header Information
                //var rptHeader = dbContext.SP_ACC_Get_Report_Header_Information("Reports", "CashBook", PMGSYSession.Current.FundType, LevelId, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
                //if (rptHeader == null)
                //{
                //    cbSingle.ReportNumber = String.Empty;
                //    cbSingle.ReportName = String.Empty;
                //    cbSingle.ReportParaName = String.Empty;
                //    cbSingle.FundType = objCommonFunc.GetFundName(PMGSYSession.Current.FundType);
                //}
                //else
                //{
                //    cbSingle.ReportNumber = rptHeader.REPORT_FORM_NO;
                //    cbSingle.ReportName = rptHeader.REPORT_NAME;
                //    cbSingle.ReportParaName = rptHeader.REPORT_PARAGRAPH_NAME;
                //    cbSingle.FundType = objCommonFunc.GetFundName(PMGSYSession.Current.FundType);
                //}
                //scope.Complete();
                return mrd;
                //}
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        public Array MPR1StateListingDAL(int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int Month = month;
                int Year = year;
                int Collaboration = collaboration;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listMPR1Reports = dbContext.USP_MPR_REPORT1(Level, State, District, Block, Month, Year, Collaboration, PMGSY).ToList<USP_MPR_REPORT1_Result>();

                totalRecords = listMPR1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listMPR1Reports = listMPR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listMPR1Reports = listMPR1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listMPR1Reports = listMPR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listMPR1Reports.Select(x => new
                {

                    id = x.LOCATION_CODE.ToString().Trim() + "$" + x.LOCATION_NAME.ToString().Trim() + "$" + Year.ToString().Trim() + "$" + Month.ToString().Trim() + "$" + Collaboration.ToString().Trim(),
                    cell = new[]{
                       // "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='MPR1DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Year.ToString().Trim()+"\",\""+Month.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                       x.LOCATION_NAME.ToString(),
                       x.IMS_YEAR.ToString(),	
                        x.TN_PROPOSALS.ToString(),	
                        x.TN_LEN.ToString(),	
                        x.TN_AMT.ToString(),	
                        x.POP1000.ToString(),	
                        x.POP999.ToString(),	
                        x.POP499.ToString(),	
                        x.POP250.ToString(),	
                        x.TU_PROPOSALS.ToString(),	
                        x.TU_LEN.ToString(),	
                        x.TU_AMT.ToString(),	
                        x.TOT_PROP.ToString(),	
                        x.TOT_LEN.ToString(),	
                        x.TOT_AMT.ToString(),	
                        x.FUNDS_RELEASED.ToString(),	
                        x.CN_PROPOSALS.ToString(),	
                        x.CN_LEN.ToString(),	
                        x.CN_AMT.ToString(),	
                        x.CPOP1000.ToString(),	
                        x.CPOP999.ToString(),	
                        x.CPOP499.ToString(),	
                        x.CPOP250.ToString(),	
                        x.CU_PROPOSALS.ToString(),	
                        x.CU_LEN.ToString(),	
                        x.CU_AMT.ToString(),	
                        x.CTOT_PROP.ToString(),	
                        x.CTOT_LEN.ToString(),	
                        x.CTOT_AMT.ToString(),	
                        x.CCN_PROPOSALS.ToString(),	
                        x.CCN_LEN.ToString(),	
                        x.CCN_AMT.ToString(),	
                        x.CCPOP1000.ToString(),	
                        x.CCPOP999.ToString(),	
                        x.CCPOP499.ToString(),	
                        x.CCPOP250.ToString(),	
                        x.CCU_PROPOSALS.ToString(),	
                        x.CCU_LEN.ToString(),	
                        x.CCU_AMT.ToString(),	
                        x.CCTOT_PROP.ToString(),	
                        x.CCTOT_LEN.ToString(),	
                        x.CCTOT_AMT.ToString()
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array MPR1DistrictListingDAL(int stateCode, int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Month = month;
                int Year = year;
                int Collaboration = collaboration;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listMPR1Reports = dbContext.USP_MPR_REPORT1(Level, State, District, Block, Month, Year, Collaboration, PMGSY).ToList<USP_MPR_REPORT1_Result>();

                totalRecords = listMPR1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listMPR1Reports = listMPR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listMPR1Reports = listMPR1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listMPR1Reports = listMPR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listMPR1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE.ToString().Trim() + "$" + State.ToString().Trim() + "$" + x.LOCATION_NAME.ToString().Trim() + "$" + Year.ToString().Trim() + "$" + Month.ToString().Trim() + "$" + Collaboration.ToString().Trim(),
                    cell = new[]{
                        //"<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='MPR1BlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Year.ToString().Trim()+"\",\""+Month.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.LOCATION_NAME.ToString(),
                        x.IMS_YEAR.ToString(),	
                        x.TN_PROPOSALS.ToString(),	
                        x.TN_LEN.ToString(),	
                        x.TN_AMT.ToString(),	
                        x.POP1000.ToString(),	
                        x.POP999.ToString(),	
                        x.POP499.ToString(),	
                        x.POP250.ToString(),	
                        x.TU_PROPOSALS.ToString(),	
                        x.TU_LEN.ToString(),	
                        x.TU_AMT.ToString(),	
                        x.TOT_PROP.ToString(),	
                        x.TOT_LEN.ToString(),	
                        x.TOT_AMT.ToString(),	
                        x.FUNDS_RELEASED.ToString(),	
                        x.CN_PROPOSALS.ToString(),	
                        x.CN_LEN.ToString(),	
                        x.CN_AMT.ToString(),	
                        x.CPOP1000.ToString(),	
                        x.CPOP999.ToString(),	
                        x.CPOP499.ToString(),	
                        x.CPOP250.ToString(),	
                        x.CU_PROPOSALS.ToString(),	
                        x.CU_LEN.ToString(),	
                        x.CU_AMT.ToString(),	
                        x.CTOT_PROP.ToString(),	
                        x.CTOT_LEN.ToString(),	
                        x.CTOT_AMT.ToString(),	
                        x.CCN_PROPOSALS.ToString(),	
                        x.CCN_LEN.ToString(),	
                        x.CCN_AMT.ToString(),	
                        x.CCPOP1000.ToString(),	
                        x.CCPOP999.ToString(),	
                        x.CCPOP499.ToString(),	
                        x.CCPOP250.ToString(),	
                        x.CCU_PROPOSALS.ToString(),	
                        x.CCU_LEN.ToString(),	
                        x.CCU_AMT.ToString(),	
                        x.CCTOT_PROP.ToString(),	
                        x.CCTOT_LEN.ToString(),	
                        x.CCTOT_AMT.ToString()              
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array MPR1BlockListingDAL(int districtCode, int stateCode, int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Month = month;
                int Year = year;
                int Collaboration = collaboration;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listMPR1Reports = dbContext.USP_MPR_REPORT1(Level, State, District, Block, Month, Year, Collaboration, PMGSY).ToList<USP_MPR_REPORT1_Result>();

                totalRecords = listMPR1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listMPR1Reports = listMPR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listMPR1Reports = listMPR1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listMPR1Reports = listMPR1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listMPR1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE.ToString().Trim() + "$" + District.ToString().Trim() + "$" + State.ToString().Trim() + "S" + x.LOCATION_NAME.ToString().Trim() + "$" + Year.ToString().Trim() + "$" + Month.ToString().Trim() + "$" + Collaboration.ToString().Trim(),
                    cell = new[]{
                      //"<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='MPR1FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Year.ToString().Trim()+"\",\""+Month.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                       x.LOCATION_NAME.ToString(),
                       x.IMS_YEAR.ToString(),	
                        x.TN_PROPOSALS.ToString(),	
                        x.TN_LEN.ToString(),	
                        x.TN_AMT.ToString(),	
                        x.POP1000.ToString(),	
                        x.POP999.ToString(),	
                        x.POP499.ToString(),	
                        x.POP250.ToString(),	
                        x.TU_PROPOSALS.ToString(),	
                        x.TU_LEN.ToString(),	
                        x.TU_AMT.ToString(),	
                        x.TOT_PROP.ToString(),	
                        x.TOT_LEN.ToString(),	
                        x.TOT_AMT.ToString(),	
                        x.FUNDS_RELEASED.ToString(),	
                        x.CN_PROPOSALS.ToString(),	
                        x.CN_LEN.ToString(),	
                        x.CN_AMT.ToString(),	
                        x.CPOP1000.ToString(),	
                        x.CPOP999.ToString(),	
                        x.CPOP499.ToString(),	
                        x.CPOP250.ToString(),	
                        x.CU_PROPOSALS.ToString(),	
                        x.CU_LEN.ToString(),	
                        x.CU_AMT.ToString(),	
                        x.CTOT_PROP.ToString(),	
                        x.CTOT_LEN.ToString(),	
                        x.CTOT_AMT.ToString(),	
                        x.CCN_PROPOSALS.ToString(),	
                        x.CCN_LEN.ToString(),	
                        x.CCN_AMT.ToString(),	
                        x.CCPOP1000.ToString(),	
                        x.CCPOP999.ToString(),	
                        x.CCPOP499.ToString(),	
                        x.CCPOP250.ToString(),	
                        x.CCU_PROPOSALS.ToString(),	
                        x.CCU_LEN.ToString(),	
                        x.CCU_AMT.ToString(),	
                        x.CCTOT_PROP.ToString(),	
                        x.CCTOT_LEN.ToString(),	
                        x.CCTOT_AMT.ToString()                            
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array MPR1FinalListingDAL(int blockCode, int districtCode, int stateCode, int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_CN_MPR1_FINAL_REPORT> listMPR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 4;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                char Route = '%';
                int Tinyint = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                listMPR1Reports = dbContext.Database.SqlQuery<USP_CN_MPR1_FINAL_REPORT>("EXEC [omms].[USP_CUCPL_REPORT] @Level,@State,@District,@Block,@Route,@PMGSY",
                    new SqlParameter("@Level", Level),
                    new SqlParameter("@State", State),
                    new SqlParameter("@District", District),
                    new SqlParameter("@Block", Block),
                    new SqlParameter("@Route", Route),
                    new SqlParameter("@PMGSY", Tinyint)
                ).ToList<USP_CN_MPR1_FINAL_REPORT>();

                totalRecords = listMPR1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listMPR1Reports = listMPR1Reports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listMPR1Reports = listMPR1Reports.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listMPR1Reports = listMPR1Reports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listMPR1Reports.Select(x => new
                {
                    id = x.MAST_BLOCK_CODE,
                    cell = new[]{	
                        x.MAST_BLOCK_NAME,
                        x.IMS_YEAR.ToString(),	
                        x.IMS_BATCH.ToString(),	
                        x.IMS_PACKAGE_ID.ToString()	,
                        x.IMS_PROPOSAL_TYPE.ToString(),	
                        x.IMS_ROAD_NAME.ToString(),	
                        x.IMS_BRIDGE_NAME.ToString(),	
                        x.IMS_UPGRADE_CONNECT.ToString(),	
                        x.IMS_PAV_LENGTH.ToString(),	
                        x.IMS_BRIDGE_LENGTH.ToString(),	
                        x.IMS_COLLABORATION.ToString(),	
                        x.ROAD_AMT.ToString(),	
                        x.BRIDGE_AMT.ToString(),	
                        x.MAINT_AMT.ToString(),	
                        x.TOTAL_LENGTH_COMPLETED.ToString(),	
                        x.TOTAL_EXP.ToString(), 
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Array MPR2StateListingDAL(int month, int year, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int Month = month;
                int Year = year;
                int Collaboration = collaboration;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listMPR2Reports = dbContext.USP_MPR_REPORT2(Level, State, District, Block, Month, Year, Collaboration, PMGSY).ToList<USP_MPR_REPORT2_Result>();

                totalRecords = listMPR2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listMPR2Reports = listMPR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listMPR2Reports = listMPR2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listMPR2Reports = listMPR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listMPR2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='MPR2DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+month.ToString().Trim()+"\",\""+year.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TN_LEN.ToString(),	
                        x.TNM_LEN.ToString(),	
                        x.TNY_LEN.ToString(),	
                        x.TPOP1000.ToString(),	
                        x.TPOP999.ToString(),	
                        x.TPOP499.ToString(),	
                        x.TPOP250.ToString(),	
                        x.TOT_TPOP.ToString(),	
                        x.POP1000.ToString(),	
                        x.POP999.ToString(),	
                        x.POP499.ToString(),	
                        x.POP250.ToString(),	
                        x.TOT_POP.ToString(),	
                        x.YPOP1000.ToString(),	
                        x.YPOP999.ToString(),	
                        x.YPOP499.ToString(),	
                        x.YPOP250.ToString(),	
                        x.TOT_YPOP.ToString(),	
                        x.TU_LEN.ToString(),	
                        x.TUM_LEN.ToString(),	
                        x.TRM_LEN.ToString(),		
                        x.TUY_LEN.ToString(),		
                        x.TRY_LEN.ToString()
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array MPR2DistrictListingDAL(int month, int year, int collaboration, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Month = month;
                int Year = year;
                int Collaboration = collaboration;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listMPR2Reports = dbContext.USP_MPR_REPORT2(Level, State, District, Block, Month, Year, Collaboration, PMGSY).ToList<USP_MPR_REPORT2_Result>();


                totalRecords = listMPR2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listMPR2Reports = listMPR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listMPR2Reports = listMPR2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listMPR2Reports = listMPR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listMPR2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='MPR2BlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Month.ToString().Trim()+"\",\""+Year.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TN_LEN.ToString(),	
                        x.TNM_LEN.ToString(),	
                        x.TNY_LEN.ToString(),	
                        x.TPOP1000.ToString(),	
                        x.TPOP999.ToString(),	
                        x.TPOP499.ToString(),	
                        x.TPOP250.ToString(),	
                        x.TOT_TPOP.ToString(),	
                        x.POP1000.ToString(),	
                        x.POP999.ToString(),	
                        x.POP499.ToString(),	
                        x.POP250.ToString(),	
                        x.TOT_POP.ToString(),	
                        x.YPOP1000.ToString(),	
                        x.YPOP999.ToString(),	
                        x.YPOP499.ToString(),	
                        x.YPOP250.ToString(),	
                        x.TOT_YPOP.ToString(),	
                        x.TU_LEN.ToString(),	
                        x.TUM_LEN.ToString(),	
                        x.TRM_LEN.ToString(),		
                        x.TUY_LEN.ToString(),		
                        x.TRY_LEN.ToString()      
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array MPR2BlockListingDAL(int month, int year, int collaboration, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Month = month;
                int Year = year;
                int Collaboration = collaboration;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listMPR2Reports = dbContext.USP_MPR_REPORT2(Level, State, District, Block, Month, Year, Collaboration, PMGSY).ToList<USP_MPR_REPORT2_Result>();

                totalRecords = listMPR2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listMPR2Reports = listMPR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listMPR2Reports = listMPR2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listMPR2Reports = listMPR2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listMPR2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                    //  "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='MPR2FinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Month.ToString().Trim()+"\",\""+Year.ToString().Trim() +"\",\""+Collaboration.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                       x.LOCATION_NAME,                        
                       x.TN_LEN.ToString(),	
                        x.TNM_LEN.ToString(),	
                        x.TNY_LEN.ToString(),	
                        x.TPOP1000.ToString(),	
                        x.TPOP999.ToString(),	
                        x.TPOP499.ToString(),	
                        x.TPOP250.ToString(),	
                        x.TOT_TPOP.ToString(),	
                        x.POP1000.ToString(),	
                        x.POP999.ToString(),	
                        x.POP499.ToString(),	
                        x.POP250.ToString(),	
                        x.TOT_POP.ToString(),	
                        x.YPOP1000.ToString(),	
                        x.YPOP999.ToString(),	
                        x.YPOP499.ToString(),	
                        x.YPOP250.ToString(),	
                        x.TOT_YPOP.ToString(),	
                        x.TU_LEN.ToString(),	
                        x.TUM_LEN.ToString(),	
                        x.TRM_LEN.ToString(),		
                        x.TUY_LEN.ToString(),		
                        x.TRY_LEN.ToString()
                       
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array MPR2FinalListingDAL(int month, int year, int collaboration, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_MPR_REPORT2_FINAL_Result> listMPR2Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 4;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Month = month;
                int Year = year;
                int Collaboration = collaboration;
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                listMPR2Reports = dbContext.Database.SqlQuery<USP_MPR_REPORT2_FINAL_Result>("EXEC [omms].[USP_MPR_REPORT2] @Level,@State,@District,@Block,@Month,@Year,@Collaboration,@PMGSY",
                    new SqlParameter("@Level", Level),
                    new SqlParameter("@State", State),
                    new SqlParameter("@District", District),
                    new SqlParameter("@Block", Block),
                    new SqlParameter("@Month", Month),
                    new SqlParameter("@Year", Year),
                    new SqlParameter("@Collaboration", Collaboration),
                    new SqlParameter("@PMGSY", PMGSY)
                ).ToList<USP_MPR_REPORT2_FINAL_Result>();

                totalRecords = listMPR2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listMPR2Reports = listMPR2Reports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listMPR2Reports = listMPR2Reports.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listMPR2Reports = listMPR2Reports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listMPR2Reports.Select(x => new
                {
                    id = x.MAST_BLOCK_CODE,
                    cell = new[]{	
                        x.MAST_BLOCK_NAME,
                        x.IMS_YEAR.ToString(),	
                        x.IMS_BATCH.ToString(),	
                        x.IMS_PACKAGE_ID.ToString(),	
                        x.IMS_PR_ROAD_CODE.ToString(),	
                        x.IMS_PROPOSAL_TYPE.ToString(),	
                        x.IMS_ROAD_NAME.ToString(),	
                        x.IMS_BRIDGE_NAME.ToString(),	
                        x.IMS_UPGRADE_CONNECT.ToString(),	
                        x.IMS_PAV_LENGTH.ToString(),	
                        x.IMS_BRIDGE_LENGTH.ToString(),	
                        x.IMS_COLLABORATION.ToString(),	
                        x.ROAD_AMT.ToString(),	
                        x.BRIDGE_AMT.ToString(),	
                        x.MAINT_AMT.ToString(),	
                        x.TOTAL_LENGTH_COMPLETED.ToString(),	
                        x.TOTAL_EXP.ToString(), 
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Array HY1StateListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listHY1Reports = dbContext.USP_MPR_HY1(Level, State, District, PMGSY).ToList<USP_MPR_HY1_Result>();



                totalRecords = listHY1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listHY1Reports = listHY1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listHY1Reports = listHY1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listHY1Reports = listHY1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listHY1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE.ToString().Trim() + "$" + x.LOCATION_NAME.ToString().Trim(),
                    cell = new[]{
                       // "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='HY1DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.LOCATION_NAME.ToString(),
                        x.IMS_YEAR.ToString(),	
                        x.TOTAL_PROPOSALS.ToString(),	
                        x.TOTAL_LEN.ToString(),	
                        x.TOTAL_AMT.ToString(),	
                        x.TOTAL_PROPOSALS_3.ToString(),	
                        x.TOTAL_LEN_3.ToString(),	
                        x.TOTAL_AMT_3.ToString(),	
                        x.TOTAL_PROPOSALS_6.ToString(),	
                        x.TOTAL_LEN_6.ToString(),	
                        x.TOTAL_AMT_6.ToString(),	
                        x.TOTAL_PROPOSALS_9.ToString(),	
                        x.TOTAL_LEN_9.ToString(),	
                        x.TOTAL_AMT_9.ToString(),	
                        x.TOTAL_PROPOSAL_BAL.ToString(),	
                        x.TOTAL_LEN_BAL.ToString(),	
                        x.TOTAL_AMT_BAL.ToString(),	
                        x.MIN_MAX.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array HY1DistrictListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var listHY1Reports = dbContext.USP_MPR_HY1(Level, State, District, PMGSY).ToList<USP_MPR_HY1_Result>();

                totalRecords = listHY1Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listHY1Reports = listHY1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listHY1Reports = listHY1Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listHY1Reports = listHY1Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listHY1Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        x.LOCATION_NAME,
                        x.IMS_YEAR.ToString(),	
                        x.TOTAL_PROPOSALS.ToString(),	
                        x.TOTAL_LEN.ToString(),	
                        x.TOTAL_AMT.ToString(),	
                        x.TOTAL_PROPOSALS_3.ToString(),	
                        x.TOTAL_LEN_3.ToString(),	
                        x.TOTAL_AMT_3.ToString(),	
                        x.TOTAL_PROPOSALS_6.ToString(),	
                        x.TOTAL_LEN_6.ToString(),	
                        x.TOTAL_AMT_6.ToString(),	
                        x.TOTAL_PROPOSALS_9.ToString(),	
                        x.TOTAL_LEN_9.ToString(),	
                        x.TOTAL_AMT_9.ToString(),	
                        x.TOTAL_PROPOSAL_BAL.ToString(),	
                        x.TOTAL_LEN_BAL.ToString(),	
                        x.TOTAL_AMT_BAL.ToString(),	
                        x.MIN_MAX.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Array HY2StateListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listHY2Reports = dbContext.USP_MPR_HY2(Level, State, District, PMGSY).ToList<USP_MPR_HY2_Result>();

                totalRecords = listHY2Reports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listHY2Reports = listHY2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listHY2Reports = listHY2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listHY2Reports = listHY2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listHY2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE.ToString().Trim() + "$" + x.LOCATION_NAME.ToString().Trim(),
                    cell = new[]{
                       // "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='HY2DistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                       x.LOCATION_NAME.ToString(), 
                       x.IMS_YEAR.ToString(),	
                        x.TOTAL_PROPOSALS.ToString(),	
                        x.TOTAL_LEN.ToString(),	
                        x.TOTAL_AMT.ToString(),	
                        x.TOTAL_PROPOSALS_0.ToString(),	
                        x.TOTAL_LEN_0.ToString(),	
                        x.TOTAL_AMT_0.ToString(),	
                        x.TOTAL_PROPOSALS_6.ToString(),	
                        x.TOTAL_LEN_6.ToString(),
                        x.TOTAL_AMT_6.ToString(),	
                        x.TOTAL_PROPOSALS_9.ToString(),	
                        x.TOTAL_LEN_9.ToString(),	
                        x.TOTAL_AMT_9.ToString(),	
                        x.TOTAL_PROPOSALS_12.ToString(),	
                        x.TOTAL_LEN_12.ToString(),	
                        x.TOTAL_AMT_12.ToString(),	
                        x.TOTAL_PROPOSAL_BAL.ToString(),	
                        x.TOTAL_LEN_BAL.ToString(),	
                        x.TOTAL_AMT_BAL.ToString(),	
                        x.LEVIED.ToString(),	
                        x.RECOVERED.ToString(),	
                        x.REMARKS.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array HY2DistrictListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var listHY2Reports = dbContext.USP_MPR_HY2(Level, State, District, PMGSY).ToList<USP_MPR_HY2_Result>();
                totalRecords = listHY2Reports.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listHY2Reports = listHY2Reports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listHY2Reports = listHY2Reports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listHY2Reports = listHY2Reports.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listHY2Reports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        x.LOCATION_NAME,
                        x.IMS_YEAR.ToString(),	
                        x.TOTAL_PROPOSALS.ToString(),	
                        x.TOTAL_LEN.ToString(),	
                        x.TOTAL_AMT.ToString(),	
                        x.TOTAL_PROPOSALS_0.ToString(),	
                        x.TOTAL_LEN_0.ToString(),	
                        x.TOTAL_AMT_0.ToString(),	
                        x.TOTAL_PROPOSALS_6.ToString(),	
                        x.TOTAL_LEN_6.ToString(),	
                        x.TOTAL_AMT_6.ToString(),	
                        x.TOTAL_PROPOSALS_9.ToString(),	
                        x.TOTAL_LEN_9.ToString(),	
                        x.TOTAL_AMT_9.ToString(),	
                        x.TOTAL_PROPOSALS_12.ToString(),	
                        x.TOTAL_LEN_12.ToString(),	
                        x.TOTAL_AMT_12.ToString(),	
                        x.TOTAL_PROPOSAL_BAL.ToString(),	
                        x.TOTAL_LEN_BAL.ToString(),	
                        x.TOTAL_AMT_BAL.ToString(),	
                        x.LEVIED.ToString(),	
                        x.RECOVERED.ToString(),	
                        x.REMARKS.ToString()
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Array PropListStateListingDAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = PMGSYSession.Current.StateCode;
                int District = 0;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var PropListReports = dbContext.USP_PROP_LIST_REPORT(Level, State, District, Block, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_LIST_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropListDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Year.ToString().Trim()+"\",\""+Batch.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\",\""+Status.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TOTAL_PROPOSALS.ToString(),                    
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropListDistrictListingDAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var PropListReports = dbContext.USP_PROP_LIST_REPORT(Level, State, District, Block, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_LIST_REPORT_Result>();

                totalRecords = PropListReports.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropListBlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Year.ToString().Trim()+"\",\""+Batch.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\",\""+Status.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_PROPOSALS.ToString()	
                                  
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropListBlockListingDAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_MPR_REPORT1_Result> listMPR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var PropListReports = dbContext.USP_PROP_LIST_REPORT(Level, State, District, Block, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_LIST_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                      "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropListFinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Year.ToString().Trim()+"\",\""+Batch.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\",\""+Status.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                      // x.LOCATION_NAME.ToString(),
                       x.TOTAL_PROPOSALS.ToString()                                       
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropListFinalListingDAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_PropList_REPORT_FINAL_Result> PropListReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 4;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                PropListReports = dbContext.Database.SqlQuery<USP_PropList_REPORT_FINAL_Result>("EXEC [omms].[USP_PROP_LIST_REPORT] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Status,@PMGSY",
                    new SqlParameter("@Level", Level),
                    new SqlParameter("@State", State),
                    new SqlParameter("@District", District),
                    new SqlParameter("@Block", Block),
                    new SqlParameter("@Year", Year),
                    new SqlParameter("@Batch", Batch),
                    new SqlParameter("@Collaboration", Collaboration),
                    new SqlParameter("@Status", Status),
                   new SqlParameter("@PMGSY", PMGSY)
                ).ToList<USP_PropList_REPORT_FINAL_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.MAST_BLOCK_CODE,
                    cell = new[]{	
                        x.MAST_BLOCK_NAME,
                        x.IMS_YEAR.ToString(),	
                        x.IMS_BATCH.ToString(),	
                        x.IMS_PACKAGE_ID.ToString()	,
                        x.ROAD_NAME.ToString(),	
                        x.ROAD_LENGTH.ToString(),	
                        x.IMS_BT_LENGTH.ToString(),	
                        x.IMS_CC_LENGTH.ToString(),                       	
                        x.IMS_COLLABORATION.ToString(),	
                        x.ROAD_AMT.ToString(),	
                        x.BRIDGE_AMT.ToString(),	
                        x.MAINT_AMT.ToString(),	
                        x.MAST_HAB_NAME.ToString(),	
                        x.MAST_HAB_STATUS.ToString(),	
                        x.MAST_HAB_TOT_POP.ToString(),
                        x.PROPOSAL_STATUS.ToString()
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Array PropSanctionStateListingDAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = 0;
                int District = 0;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var PropListReports = dbContext.USP_PROP_LIST_REPORT(Level, State, District, Block, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_LIST_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropSanctionDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Year.ToString().Trim()+"\",\""+Batch.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\",\""+Status.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TOTAL_PROPOSALS.ToString(),                    
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropSanctionDistrictListingDAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var PropListReports = dbContext.USP_PROP_LIST_REPORT(Level, State, District, Block, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_LIST_REPORT_Result>();

                totalRecords = PropListReports.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropSanctionBlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Year.ToString().Trim()+"\",\""+Batch.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\",\""+Status.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_PROPOSALS.ToString()	
                                  
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropSanctionBlockListingDAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_MPR_REPORT1_Result> listMPR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var PropListReports = dbContext.USP_PROP_LIST_REPORT(Level, State, District, Block, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_LIST_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                      "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropSanctionFinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Year.ToString().Trim()+"\",\""+Batch.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\",\""+Status.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                      // x.LOCATION_NAME.ToString(),
                       x.TOTAL_PROPOSALS.ToString()                                       
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropSanctionFinalListingDAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_PropList_REPORT_FINAL_Result> PropListReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 4;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                PropListReports = dbContext.Database.SqlQuery<USP_PropList_REPORT_FINAL_Result>("EXEC [omms].[USP_PROP_LIST_REPORT] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Status,@PMGSY",
                    new SqlParameter("@Level", Level),
                    new SqlParameter("@State", State),
                    new SqlParameter("@District", District),
                    new SqlParameter("@Block", Block),
                    new SqlParameter("@Year", Year),
                    new SqlParameter("@Batch", Batch),
                    new SqlParameter("@Collaboration", Collaboration),
                    new SqlParameter("@Status", Status),
                   new SqlParameter("@PMGSY", PMGSY)
                ).ToList<USP_PropList_REPORT_FINAL_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.MAST_BLOCK_CODE,
                    cell = new[]{	
                        x.MAST_BLOCK_NAME,
                        x.IMS_YEAR.ToString(),	
                        x.IMS_BATCH.ToString(),	
                        x.IMS_PACKAGE_ID.ToString()	,
                        x.ROAD_NAME.ToString(),	
                        x.ROAD_LENGTH.ToString(),	
                        x.IMS_BT_LENGTH.ToString(),	
                        x.IMS_CC_LENGTH.ToString(),                       	
                        x.IMS_COLLABORATION.ToString(),	
                        x.ROAD_AMT.ToString(),	
                        x.BRIDGE_AMT.ToString(),	
                        x.MAINT_AMT.ToString(),	
                        x.MAST_HAB_NAME.ToString(),	
                        x.MAST_HAB_STATUS.ToString(),	
                        x.MAST_HAB_TOT_POP.ToString() 
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Array PropSanctionLengthStateListingDAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                //List<USP_Proposal_Length_REPORT_Result> PropListReports;
                int Level = 1;
                int State = PMGSYSession.Current.StateCode;
                int District = 0;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var PropListReports = dbContext.USP_PROP_LEN_REPORT(Level, State, District, Block, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_LEN_REPORT_Result>();
                //  PropListReports = dbContext.Database.SqlQuery<USP_Proposal_Length_REPORT_Result>("EXEC [omms].[USP_PROP_LEN_REPORT] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Status,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Year", Year),
                //    new SqlParameter("@Batch", Batch),
                //    new SqlParameter("@Collaboration", Collaboration),
                //    new SqlParameter("@Status", Status),
                //   new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_Proposal_Length_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropSanctionLengthDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Year.ToString().Trim()+"\",\""+Batch.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\",\""+Status.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TOTAL_PROPOSALS.ToString(),
                        x.ROAD_LEN.ToString(),
                        x.BT_LEN.ToString(),
                        x.CC_LEN.ToString(),
                        x.CC_PERC.ToString()
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropSanctionLengthDistrictListingDAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                //  List<USP_Proposal_Length_REPORT_Result> PropListReports;
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var PropListReports = dbContext.USP_PROP_LEN_REPORT(Level, State, District, Block, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_LEN_REPORT_Result>();
                //PropListReports = dbContext.Database.SqlQuery<USP_Proposal_Length_REPORT_Result>("EXEC [omms].[USP_PROP_LEN_REPORT] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Status,@PMGSY",
                //    new SqlParameter("@Level", Level),
                //    new SqlParameter("@State", State),
                //    new SqlParameter("@District", District),
                //    new SqlParameter("@Block", Block),
                //    new SqlParameter("@Year", Year),
                //    new SqlParameter("@Batch", Batch),
                //    new SqlParameter("@Collaboration", Collaboration),
                //    new SqlParameter("@Status", Status),
                //    new SqlParameter("@PMGSY", PMGSY)
                //    ).ToList<USP_Proposal_Length_REPORT_Result>();

                totalRecords = PropListReports.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropSanctionLengthBlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Year.ToString().Trim()+"\",\""+Batch.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\",\""+Status.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                        x.TOTAL_PROPOSALS.ToString(),
                        x.ROAD_LEN.ToString(),
                        x.BT_LEN.ToString(),
                        x.CC_LEN.ToString(),
                        x.CC_PERC.ToString()
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropSanctionLengthBlockListingDAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_MPR_REPORT1_Result> listMPR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // List<USP_Proposal_Length_REPORT_Result> PropListReports;
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var PropListReports = dbContext.USP_PROP_LEN_REPORT(Level, State, District, Block, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_LEN_REPORT_Result>();
                //PropListReports = dbContext.Database.SqlQuery<USP_Proposal_Length_REPORT_Result>("EXEC [omms].[USP_PROP_LEN_REPORT] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Status,@PMGSY",
                //new SqlParameter("@Level", Level),
                //new SqlParameter("@State", State),
                //new SqlParameter("@District", District),
                //new SqlParameter("@Block", Block),
                //new SqlParameter("@Year", Year),
                //new SqlParameter("@Batch", Batch),
                //new SqlParameter("@Collaboration", Collaboration),
                //new SqlParameter("@Status", Status),
                //new SqlParameter("@PMGSY", PMGSY)
                //).ToList<USP_Proposal_Length_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                      "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropSanctionLengthFinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Year.ToString().Trim()+"\",\""+Batch.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\",\""+Status.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                      // x.LOCATION_NAME.ToString(),
                        x.TOTAL_PROPOSALS.ToString(),
                        x.ROAD_LEN.ToString(),
                        x.BT_LEN.ToString(),
                        x.CC_LEN.ToString(),
                        x.CC_PERC.ToString()               
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropSanctionLengthFinalListingDAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_Prop_Length_REPORT_FINAL_Result> PropListReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 4;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                PropListReports = dbContext.Database.SqlQuery<USP_Prop_Length_REPORT_FINAL_Result>("EXEC [omms].[USP_PROP_LEN_REPORT] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Status,@PMGSY",
                    new SqlParameter("@Level", Level),
                    new SqlParameter("@State", State),
                    new SqlParameter("@District", District),
                    new SqlParameter("@Block", Block),
                    new SqlParameter("@Year", Year),
                    new SqlParameter("@Batch", Batch),
                    new SqlParameter("@Collaboration", Collaboration),
                    new SqlParameter("@Status", Status),
                   new SqlParameter("@PMGSY", PMGSY)
                ).ToList<USP_Prop_Length_REPORT_FINAL_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.MAST_BLOCK_CODE,
                    cell = new[]{	
                        x.MAST_BLOCK_NAME,
                        x.IMS_YEAR.ToString(),	
                        x.IMS_BATCH.ToString(),	
                        x.IMS_PACKAGE_ID.ToString()	,
                        x.ROAD_NAME.ToString(),	
                        x.IMS_UPGRADE_CONNECT.ToString(),
	                    x.IMS_PAV_LENGTH.ToString(),
                        x.IMS_BT_LENGTH.ToString(),  
                     	x.IMS_CC_LENGTH.ToString(),	
                        x.ROAD_AMT.ToString(),	
                        x.BRIDGE_AMT.ToString(),                       
                        x.MAINT_AMT.ToString(),	
                        x.TOTAL_AMT.ToString() 
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Array PropEMCStateListingDAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                int State = PMGSYSession.Current.StateCode;
                int District = 0;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 720;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var PropListReports = dbContext.USP_PROP_EMC_REPORT(Level, State, District, Block, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_EMC_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropEMCDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Year.ToString().Trim()+"\",\""+Batch.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\",\""+Status.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.TOTAL_PROPOSALS.ToString(),
                        x.ROAD_AMT.ToString(),
                        x.BRIDGE_AMT.ToString(),
                        x.MAINT1_AMT.ToString(),
                        x.MAINT2_AMT.ToString(),
                        x.MAINT3_AMT.ToString(),
                        x.MAINT4_AMT.ToString(),
                        x.MAINT5_AMT.ToString(),
                        x.MAINT_AMT.ToString(),
                       x.TOTAL_AMT.ToString()
                       
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropEMCDistrictListingDAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                int State = stateCode;
                int District = 0;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var PropListReports = dbContext.USP_PROP_EMC_REPORT(Level, State, District, Block, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_EMC_REPORT_Result>();

                totalRecords = PropListReports.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                        "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropEMCBlockReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+stateCode+"\",\""+x.LOCATION_NAME+"\",\""+Year.ToString().Trim()+"\",\""+Batch.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\",\""+Status.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                       x.TOTAL_PROPOSALS.ToString(),
                        x.ROAD_AMT.ToString(),
                        x.BRIDGE_AMT.ToString(),
                        x.MAINT1_AMT.ToString(),
                        x.MAINT2_AMT.ToString(),
                        x.MAINT3_AMT.ToString(),
                        x.MAINT4_AMT.ToString(),
                        x.MAINT5_AMT.ToString(),
                        x.MAINT_AMT.ToString(),
                       x.TOTAL_AMT.ToString() 
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropEMCBlockListingDAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            // List<USP_MPR_REPORT1_Result> listMPR1Reports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 3;
                int State = stateCode;
                int District = districtCode;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var PropListReports = dbContext.USP_PROP_EMC_REPORT(Level, State, District, Block, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_EMC_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE,
                    cell = new[]{
                      "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropEMCFinalReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+districtCode.ToString()+"\",\""+stateCode.ToString()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Year.ToString().Trim()+"\",\""+Batch.ToString().Trim()+"\",\""+Collaboration.ToString().Trim()+"\",\""+Status.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",
                      // x.LOCATION_NAME.ToString(),
                        x.TOTAL_PROPOSALS.ToString(),
                        x.ROAD_AMT.ToString(),
                        x.BRIDGE_AMT.ToString(),
                        x.MAINT1_AMT.ToString(),
                        x.MAINT2_AMT.ToString(),
                        x.MAINT3_AMT.ToString(),
                        x.MAINT4_AMT.ToString(),
                        x.MAINT5_AMT.ToString(),
                        x.MAINT_AMT.ToString(),
                        x.TOTAL_AMT.ToString()               
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropEMCFinalListingDAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_Prop_EMC_REPORT_FINAL_Result> PropListReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 4;
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                PropListReports = dbContext.Database.SqlQuery<USP_Prop_EMC_REPORT_FINAL_Result>("EXEC [omms].[USP_PROP_EMC_REPORT] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Status,@PMGSY",
                    new SqlParameter("@Level", Level),
                    new SqlParameter("@State", State),
                    new SqlParameter("@District", District),
                    new SqlParameter("@Block", Block),
                    new SqlParameter("@Year", Year),
                    new SqlParameter("@Batch", Batch),
                    new SqlParameter("@Collaboration", Collaboration),
                    new SqlParameter("@Status", Status),
                   new SqlParameter("@PMGSY", PMGSY)
                ).ToList<USP_Prop_EMC_REPORT_FINAL_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.MAST_BLOCK_CODE,
                    cell = new[]{	
                        x.MAST_BLOCK_NAME,
                        x.IMS_YEAR.ToString(),	
                        x.IMS_BATCH.ToString(),	
                        x.IMS_PACKAGE_ID.ToString()	,
                        x.ROAD_NAME.ToString(),	
                        x.IMS_UPGRADE_CONNECT.ToString(),                                             	
                        x.ROAD_AMT.ToString(),	
                        x.BRIDGE_AMT.ToString(),	
                        x.IMS_SANCTIONED_MAN_AMT1.ToString(),	
                        x.IMS_SANCTIONED_MAN_AMT2.ToString(),	
                        x.IMS_SANCTIONED_MAN_AMT3.ToString(),	
                        x.IMS_SANCTIONED_MAN_AMT4.ToString(),	
                        x.IMS_SANCTIONED_MAN_AMT5.ToString(), 
                        x.MAINT_AMT.ToString(),	
                        x.TOTAL_AMT.ToString()  
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropScrutinyListingDAL(int stateCode, string type, int agencyId, int year, int batch, int scheme, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_PROP_SCRUTINY_List_REPORT_Result> PropListReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int State = stateCode;
                string Type = type == "0" ? "%" : type;
                int District = PMGSYSession.Current.DistrictCode;
                int Block = 0;
                int Year = year;
                int Batch = batch;
                int Scheme = scheme;
                int TACode = agencyId;
                string TAName = "%";
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //var PropListReports = agencyId;
                // var PropListReports = dbContext.USP_PROP_SCRUTINY_REPORT(Type, State, District, Year, Batch, Scheme,TACode,TAName, PMGSY).ToList<USP_PROP_SCRUTINY_REPORT_Result>();
                PropListReports = dbContext.Database.SqlQuery<USP_PROP_SCRUTINY_List_REPORT_Result>("EXEC [omms].[USP_PROP_SCRUTINY_REPORT] @TYPE,@STATE,@DISTRICT,@Block,@YEAR,@BATCH,@SCHEME,@TACODE,@TANAME,@PMGSY",
                  new SqlParameter("@TYPE", Type),
                  new SqlParameter("@STATE", State),
                  new SqlParameter("@DISTRICT", District),
                  new SqlParameter("@Block", Block),
                  new SqlParameter("@YEAR", Year),
                  new SqlParameter("@BATCH", Batch),
                  new SqlParameter("@SCHEME", Scheme),
                  new SqlParameter("@TACODE", TACode),
                  new SqlParameter("@TANAME", TAName),
                 new SqlParameter("@PMGSY", PMGSY)
              ).ToList<USP_PROP_SCRUTINY_List_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.TA_CODE.ToString(),
                    cell = new[]{
                      // "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropEMCDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                      // "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PropEMCDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                        x.MAST_STATE_NAME.ToString(),
                        x.MAST_DISTRICT_NAME.ToString(),
                        x.IMS_YEAR.ToString(),
                        x.MAST_FUNDING_AGENCY_NAME.ToString(),
                        x.TA_NAME.ToString(),
                        x.TA_PROPOSALS==0?"0": "<a href='#' style='text-decoration:underline;color:#0000FF' title='Click here to view details' onClick='loadPropScrutinyTASDReportGrid(\""+Type.ToString().Trim()+"\",\""+x.MAST_STATE_CODE.ToString()+"\",\""+x.MAST_DISTRICT_CODE.ToString()+"\",\""+x.IMS_YEAR.ToString().Trim()+"\",\""+Batch+"\",\""+x.MAST_FUNDING_AGENCY_CODE.ToString()+"\",\""+x.TA_CODE.ToString()+"\",\""+x.TA_NAME.ToString().Trim()+"\",\""+x.TA_PROPOSALS.ToString().Trim()+"\"); return false;'>" + x.TA_PROPOSALS.ToString().Trim() + "</a>",                       
                        x.TA_PROPOSALS.ToString(),
                        (x.TA_ROAD_AMT+ x.TA_BRIDGE_AMT+x.TA_MAINT_AMT).ToString(),                       
                        x.MRD_PROPOSALS==0?"0": "<a href='#' style='text-decoration:underline;color:#0000FF' title='Click here to view details' onClick='loadPropScrutinyTASDReportGrid(\""+Type.ToString().Trim()+"\",\""+x.MAST_STATE_CODE.ToString()+"\",\""+x.MAST_DISTRICT_CODE.ToString()+"\",\""+x.IMS_YEAR.ToString().Trim()+"\",\""+Batch+"\",\""+x.MAST_FUNDING_AGENCY_CODE.ToString()+"\",\""+x.TA_CODE.ToString()+"\",\""+x.TA_NAME.ToString().Trim()+"\",\""+x.MRD_PROPOSALS.ToString().Trim()+"\"); return false;'>" + x.MRD_PROPOSALS.ToString().Trim() + "</a>",                       
                        x.MRD_PROPOSALS.ToString(),
                        (x.MRD_ROAD_AMT+ x.MRD_BRIDGE_AMT+x.MRD_MAINT_AMT).ToString()                                           
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropScrutinyTASDListingDAL(int stateCode, int districtCode, string type, int agencyId, int year, int batch, int scheme, string taName, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int State = stateCode;
                int District = districtCode;
                string Type = type == "0" ? "%" : type;
                int Year = year;
                int Batch = batch;
                int Scheme = scheme;
                int TACode = agencyId;
                string TAName = taName;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //var PropListReports = agencyId;
                var PropListReports = dbContext.USP_PROP_TASD_REPORT(Type, State, District, Year, Batch, Scheme, TACode, TAName, PMGSY).ToList<USP_PROP_TASD_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{
                        x.MAST_STATE_NAME.ToString(),
                        x.MAST_FUNDING_AGENCY_NAME.ToString(),
                        x.MAST_DISTRICT_NAME.ToString(),
                        x.MAST_BLOCK_NAME.ToString(),
                        x.IMS_PACKAGE_ID.ToString(),
                        x.IMS_ROAD_NAME.ToString(),
                        x.IMS_YEAR.ToString(),                       
                        x.IMS_UPGRADE_CONNECT.ToString(),            
                        x.PROP_LEN.ToString(),
                        x.IMS_SANCTIONED_PAV_AMT.ToString(),
                        x.IMS_NO_OF_CDWORKS.ToString(),
                        x.IMS_SANCTIONED_CD_AMT.ToString(),
                        x.IMS_SANCTIONED_OW_AMT.ToString(), 
                        x.IMS_SANCTIONED_PW_AMT.ToString(),          
                       (x.IMS_SANCTIONED_PAV_AMT+ x.IMS_SANCTIONED_CD_AMT + x.IMS_SANCTIONED_OW_AMT+x.IMS_SANCTIONED_PW_AMT).ToString()
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> GetTechAgencyName_ByAgencyType(int stateCode, string type)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstAgency = new List<SelectListItem>();

                var list = (from ag in dbContext.ADMIN_TECHNICAL_AGENCY
                            where
                                //   (type == string.Empty ? "%" : ag.ADMIN_TA_TYPE) == (type == string.Empty ? "%" : type)
                          (type == "0" ? "%" : ag.ADMIN_TA_TYPE) == (type == "0" ? "%" : type) &&
                          (stateCode == 0 ? 0 : ag.MAST_STATE_CODE) == (stateCode == 0 ? 0 : stateCode)
                            select new
                            {
                                Value = ag.ADMIN_TA_CODE,
                                Text = ag.ADMIN_TA_NAME
                            }).Distinct().OrderBy(ag => ag.Text).ToList();


                lstAgency = new SelectList(list.ToList(), "Value", "Text").ToList();
                lstAgency.Insert(0, new SelectListItem { Text = "All", Value = "0" });

                return lstAgency;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public List<SelectListItem> GetFundingAgencyList()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstAgency = new List<SelectListItem>();

                var list = (from ag in dbContext.MASTER_FUNDING_AGENCY
                            select new
                            {
                                MAST_FUNDING_AGENCY_CODE = ag.MAST_FUNDING_AGENCY_CODE,
                                MAST_FUNDING_AGENCY_NAME = ag.MAST_FUNDING_AGENCY_NAME
                            }).Distinct().OrderBy(m => m.MAST_FUNDING_AGENCY_NAME).ToList();


                lstAgency = new SelectList(list.ToList(), "MAST_FUNDING_AGENCY_CODE", "MAST_FUNDING_AGENCY_NAME").ToList();
                lstAgency.Insert(0, new SelectListItem { Text = "All", Value = "0" });
                return lstAgency;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public List<SelectListItem> GetAgencyList()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstAgency = new List<SelectListItem>();

                var list = (from ag in dbContext.MASTER_AGENCY

                            select new
                            {
                                MAST_AGENCY_CODE = ag.MAST_AGENCY_CODE,
                                MAST_AGENCY_NAME = ag.MAST_AGENCY_NAME
                            }).Distinct().OrderBy(m => m.MAST_AGENCY_NAME).ToList();


                lstAgency = new SelectList(list.ToList(), "MAST_AGENCY_CODE", "MAST_AGENCY_NAME").ToList();
                lstAgency.Insert(0, new SelectListItem { Text = "All", Value = "0" });
                return lstAgency;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> GetContractoreNameByState(int stateCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstContractor = new List<SelectListItem>();

                var list = (from mc in dbContext.MASTER_CONTRACTOR
                            join
                             mcr in dbContext.MASTER_CONTRACTOR_REGISTRATION
                             on mc.MAST_CON_ID equals mcr.MAST_CON_ID
                            where
                            (mc.MAST_CON_SUP_FLAG == "C") &&
                          (stateCode == 0 ? 0 : mcr.MAST_REG_STATE) == (stateCode == 0 ? 0 : stateCode)
                            select new
                            {
                                Value = mc.MAST_CON_ID,
                                Text = (mc.MAST_CON_FNAME != null ? mc.MAST_CON_FNAME.Trim() : string.Empty) + " " + (mc.MAST_CON_MNAME != null ? mc.MAST_CON_MNAME.Trim() : string.Empty) + " " + (mc.MAST_CON_LNAME != null ? mc.MAST_CON_LNAME.Trim() : string.Empty)
                            }).Distinct().OrderBy(ct => ct.Text).ToList();


                lstContractor = new SelectList(list.ToList(), "Value", "Text").ToList();
                lstContractor.Insert(0, new SelectListItem { Text = "All", Value = "0" });

                return lstContractor;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> GetPackageByStateDistrictBlock(int stateCode, int districtCode, int blockCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstPackage = new List<SelectListItem>();

                var list = (from ag in dbContext.IMS_SANCTIONED_PROJECTS
                            where
                                //   (type == string.Empty ? "%" : ag.ADMIN_TA_TYPE) == (type == string.Empty ? "%" : type)
                          (stateCode == 0 ? 0 : ag.MAST_STATE_CODE) == (stateCode == 0 ? 0 : stateCode) &&
                          (districtCode == 0 ? 0 : ag.MAST_DISTRICT_CODE) == (districtCode == 0 ? 0 : districtCode) &&
                          (blockCode == 0 ? 0 : ag.MAST_BLOCK_CODE) == (blockCode == 0 ? 0 : blockCode)


                            select new
                            {
                                Value = ag.IMS_PACKAGE_ID,
                                Text = ag.IMS_PACKAGE_ID
                            }).Distinct().OrderBy(ag => ag.Text).ToList();


                lstPackage = new SelectList(list.ToList(), "Value", "Text").ToList();
                lstPackage.Insert(0, new SelectListItem { Text = "All", Value = "%" });

                return lstPackage;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PendingWorksListingDAL(int stateCode, string reason, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int State = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                string Reason = reason;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PENDING_WORKS_REPORT(State, Reason, PMGSY).ToList<USP_PENDING_WORKS_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{
                        x.MAST_STATE_NAME.ToString(),
                        x.MAST_DISTRICT_NAME.ToString(),
                        x.MAST_BLOCK_NAME.ToString(),
                        x.IMS_PACKAGE_ID.ToString(),
                        x.PHASE.ToString(),
                        x.IMS_ROAD_NAME.ToString(),
                        x.IMS_PAV_LENGTH.ToString(),                       
                        x.PAV_COST.ToString(),            
                        x.CON_TYPE.ToString(),
                        x.PENDING_REASON.ToString()                     
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropAnalysisDataGapListingDAL(int stateCode, string type, string scrutinity, string sanction, string report, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int StateCode = stateCode;
                string Type = type;
                string Scrutinity = scrutinity;
                string Sanction = sanction;
                string Report = report;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPOSAL_ANALYSIS(StateCode, Type, Scrutinity, Sanction, PMGSY).ToList<USP_PROPOSAL_ANALYSIS_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.MAST_STATE_CODE.ToString().Trim() + "$" + x.IMS_YEAR.ToString() + "$" + x.IMS_BATCH.ToString() + "$" + Type.ToString().Trim() + "$" + Scrutinity.ToString().Trim() + "$" + Sanction.ToString().Trim() + "$" + Report.ToString().Trim() + "$" + x.ROAD_COUNT.ToString().Trim(),
                    cell = new[]{
                        x.PHASE.ToString(),
                        x.IMS_BATCH.ToString(),
                        //x.ROAD_COUNT==0?"0": "<a href='#' style='text-decoration:underline;color:#0000FF' title='Click here to view details' onClick='LoadPropAnalysisDetailGrid(\""+x.MAST_STATE_CODE.ToString().Trim()+"\",\""+x.IMS_YEAR.ToString()+"\",\""+x.IMS_BATCH.ToString()+"\",\""+Type.ToString().Trim()+"\",\""+Scrutinity.ToString().Trim()+"\",\""+Sanction.ToString().Trim()+"\",\""+Report.ToString().Trim()+"\",\""+x.ROAD_COUNT.ToString().Trim()+"\"); return false;'>" + x.ROAD_COUNT.ToString().Trim() + "</a>",                       
                        //x.MAST_STATE_CODE.ToString().Trim()+"$"+x.IMS_YEAR.ToString()+"$"+x.IMS_BATCH.ToString()+"$"+Type.ToString().Trim()+"$"+Scrutinity.ToString().Trim()+"$"+Sanction.ToString().Trim()+"$"+Report.ToString().Trim()+"$"+x.ROAD_COUNT.ToString().Trim(),
                        x.ROAD_COUNT.ToString().Trim(),
                        x.ROAD_COUNT.ToString(),
                        x.SINGLE_HAB.ToString(),
                        x.HAB_MAPPED_SINGLE.ToString(),
                        x.HAB_MAPPED_MANY.ToString(),
                        x.HAB_NOT_MAPPED.ToString(),                       
                        x.MAINT_COST.ToString(),            
                        x.CN_NOT_MAPPED.ToString(),
                        x.CARRIAGE_WIDTH.ToString(),   
                        x.TRAFFIC_INTENSITY.ToString(),
                        x.CBR_VALUE.ToString()                     
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropAnalysisDetailListingDAL(int stateCode, int year, int batch, string type, string scrutinity, string sanction, string report, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int StateCode = stateCode;
                int Year = year;
                int Batch = batch;
                string Type = type;
                string Scrutinity = scrutinity;
                string Sanction = sanction;
                string Report = report;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPOSAL_ANALYSIS_DET(StateCode, Year, Batch, Type, Scrutinity, Sanction, Report, PMGSY).ToList<USP_PROPOSAL_ANALYSIS_DET_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.IMS_PR_ROAD_CODE.ToString().Trim() + "$" + x.IMS_ROAD_NAME.ToString().Trim() + "$" + StateCode,
                    cell = new[]{

                        x.MAST_DISTRICT_NAME.ToString(),
                       // "<a href='#' style='text-decoration:underline;color:#0000FF' title='Click here to view details' onClick='LoadHabitationTrafficCBRGrid(\""+x.IMS_PR_ROAD_CODE.ToString().Trim()+"\",\""+x.IMS_ROAD_NAME.ToString().Trim()+"\"); return false;'>" + x.MAST_BLOCK_NAME.ToString().Trim() + "</a>",                       
                       x.MAST_BLOCK_NAME.ToString().Trim(), 
                       x.IMS_PACKAGE_ID.ToString(),
                        x.STREAM.ToString(),
                        x.CONN_TYPE.ToString(),
                        x.SCRUTINY.ToString(),                       
                        x.IMS_CN_ROAD_NUMBER.ToString(),            
                        x.IMS_ROAD_NAME.ToString(),
                        x.IMS_CARRIAGED_WIDTH.ToString(),   
                        x.IMS_PAV_LENGTH.ToString(),
                        x.CN_LENGTH.ToString(),   
                        x.PREV_CN_LENGTH.ToString(),  
                        x.EXTRA_LENGTH.ToString(),  
                        x.PER_EXTRA.ToString(),
                        x.PROP_COST.ToString(),
                        x.STATE_SHARE.ToString(), 
                        x.TOTAL_COST.ToString(),
                        x.MAINT_COST.ToString()  
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropAnalysisHabitationListingDAL(int roadCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int RoadCode = roadCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPOSAL_ANALYSIS_HAB(RoadCode, PMGSY).ToList<USP_PROPOSAL_ANALYSIS_HAB_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{
                        x.MAST_HAB_NAME.ToString(),
                        x.MAST_HAB_TOT_POP.ToString(),
                        x.IMS_IS_HAB_SCST.ToString(),
                        x.MAST_HAB_CONNECTED.ToString()
                                            
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropAnalysisTrafficListingDAL(int roadCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int RoadCode = roadCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPOSAL_ANALYSIS_TRAF(RoadCode).ToList<USP_PROPOSAL_ANALYSIS_TRAF_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.IMS_TI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.IMS_TI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.IMS_TI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{
                        x.IMS_TI_YEAR.ToString(),
                        x.IMS_TOTAL_TI.ToString(),
                        x.IMS_COMM_TI.ToString()                                                                  
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropAnalysisCBRListingDAL(int roadCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int RoadCode = roadCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPOSAL_ANALYSIS_CBR(RoadCode).ToList<USP_PROPOSAL_ANALYSIS_CBR_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.IMS_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.IMS_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.IMS_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{
                        x.IMS_SEGMENT_NO.ToString(),
                        x.IMS_STR_CHAIN.ToString(),
                        x.IMS_END_CHAIN.ToString(),
                        x.IMS_CBR_VALUE.ToString()                                            
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> GetCarriageWidthList()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstCarriageWidth = new List<SelectListItem>();

                var list = (from ag in dbContext.MASTER_CARRIAGE
                            select new
                            {
                                MAST_CARRIAGE_CODE = ag.MAST_CARRIAGE_CODE,
                                MAST_CARRIAGE_WIDTH = ag.MAST_CARRIAGE_WIDTH
                            }).Distinct().OrderBy(m => m.MAST_CARRIAGE_WIDTH).ToList();


                lstCarriageWidth = new SelectList(list.ToList(), "MAST_CARRIAGE_CODE", "MAST_CARRIAGE_WIDTH").ToList();
                lstCarriageWidth.Insert(0, new SelectListItem { Text = "All", Value = "0" });
                return lstCarriageWidth;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #region Proposal DataGap
        public Array PropNotMappedStateListingDAL(int stateCode, int districtCode,int blockCode, int yearCode,int batchCode,int collaborationCode,int agencyCode,string staStatusCode,string mrdStatusCode, string sanctionedCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {


            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int Level = 1;
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int Year = yearCode;
                int block = blockCode;
                int collaboration = collaborationCode;
                int batch = batchCode;
                int agency = agencyCode;
                string staStatus =staStatusCode;
                string mrdStatus =mrdStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                // var PropListReports = dbContext.USP_PROPDATA_CNNOTMAP(Level, StateCode, DistrictCode, Year, PMGSY).ToList<USP_PROPDATA_CNNOTMAP_Result>();
                var PropListReports = dbContext.USP_PROPDATA_CNNOTMAP(Level, StateCode, DistrictCode, block, yearCode, batch, collaboration, agency, staStatus, mrdStatus, PMGSY).ToList<USP_PROPDATA_CNNOTMAP_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE.ToString().Trim() + "$" + x.LOCATION_NAME.ToString().Trim(),

                    cell = new[]{
                    // "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='LoadPropNotMappedDistrictDetailGrid(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+x.IMS_YEAR.ToString().Trim()+"\",\""+sanctioned.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                    x.LOCATION_NAME==null?"":x.LOCATION_NAME.ToString(),                   
                     x.PROPOSALS==null?"":x.PROPOSALS.ToString()                                                       
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropNotMappedDistrictListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_PROPDATA_CNNOTMAP_District_Result> PropListReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int Level = 2;
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int Year = yearCode;
                int Block = blockCode;
                int Batch = batchCode;         
                int collaboration = collaborationCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                string mrdStatus = mrdStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //  var PropListReports = dbContext.USP_PROPDATA_CNNOTMAP(Level, StateCode, DistrictCode, Year, PMGSY).ToList<USP_PROPDATA_CNNOTMAP_Result>();
                PropListReports = dbContext.Database.SqlQuery<USP_PROPDATA_CNNOTMAP_District_Result>("EXEC [omms].[USP_PROPDATA_CNNOTMAP] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Agency,@STAStatus,@MRDStatus,@PMGSY",
                                new SqlParameter("@Level", Level),
                                new SqlParameter("@State", StateCode),
                                new SqlParameter("@District", DistrictCode),
                                new SqlParameter("@Block", Block),
                                new SqlParameter("@Year", Year),
                                new SqlParameter("@Batch", Batch),
                                new SqlParameter("@Collaboration", collaboration),
                                new SqlParameter("@Agency", agency),             
                                new SqlParameter("@STAStatus", staStatus),
                                new SqlParameter("@MRDStatus", mrdStatus),
                                new SqlParameter("@PMGSY", PMGSY)
                              ).ToList<USP_PROPDATA_CNNOTMAP_District_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE.ToString().Trim() + "$" + x.LOCATION_NAME.ToString().Trim() + "$" + x.IMS_YEAR.ToString().Trim() + "$" + x.PHASE.ToString().Trim(),

                    cell = new[]{
                       x.PHASE==null?"": x.PHASE.ToString(),
                     // "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='LoadPropNotMappedDetailGrid(\""+StateCode.ToString().Trim()+"\",\""+stateName.ToString().Trim()+"\",\""+x.PHASE.ToString().Trim()+"\",\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+x.IMS_YEAR.ToString().Trim()+"\",\""+sanctioned.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                    x.LOCATION_NAME==null?"": x.LOCATION_NAME.ToString(),
                    x.PROPOSALS.ToString()
                                                                
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropNotMappedDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int year = yearCode;
                int block = blockCode;
                int collaboration = collaborationCode;
                int batch = batchCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                string mrdStatus = mrdStatusCode;

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                // var PropListReports = dbContext.USP_PROPDATA_CNNOTMAP_DETAILS(StateCode, DistrictCode, Year, Sanctioned, PMGSY).ToList<USP_PROPDATA_CNNOTMAP_DETAILS_Result>();
                var PropListReports = dbContext.USP_PROPDATA_CNNOTMAP_DETAILS(StateCode, DistrictCode, block, year, batch, collaboration, agency, staStatus, mrdStatus, PMGSY).ToList<USP_PROPDATA_CNNOTMAP_DETAILS_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{
                        x.MAST_DISTRICT_NAME==null?"":x.MAST_DISTRICT_NAME.ToString(),
                         x.MAST_BLOCK_NAME==null?"":x.MAST_BLOCK_NAME.ToString(),
                         x.IMS_PACKAGE_ID==null?"":x.IMS_PACKAGE_ID.ToString(),
                        x.IMS_ROAD_NAME==null?"":x.IMS_ROAD_NAME.ToString(),
                        x.ConstructionType==null?"":x.ConstructionType.ToString(),
                        x.IMS_PAV_LENGTH==null?"":x.IMS_PAV_LENGTH.ToString(),
                        x.SANC_COST==null?"": x.SANC_COST.ToString(),
                        x.Status==null?"":x.Status.ToString()                                            
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropNotMappedPhaseViewListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {


                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int year = yearCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                int block = blockCode;
                int collaboration = collaborationCode;
                int batch = batchCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                string mrdStatus = mrdStatusCode;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                // var PropListReports = dbContext.USP_PROPDATA_CNNOTMAP_PHASE(stateCode,, PMGSY).ToList<USP_PROPDATA_CNNOTMAP_PHASE_Result>();
                var PropListReports = dbContext.USP_PROPDATA_CNNOTMAP_PHASE(StateCode, DistrictCode, block, year, batch, collaboration, agency, staStatus, mrdStatus, PMGSY).ToList<USP_PROPDATA_CNNOTMAP_PHASE_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.IMS_YEAR.ToString().Trim(),
                    cell = new[]{
                        x.PHASE==null?"":x.PHASE.ToString(),
                        x.PROPOSALS==null?"":x.PROPOSALS.ToString(),
                                                                
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropNumberBaseCNDistrictListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_PROPDATA_CNDUP_REPORT_Result> PropListReports;

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                int Level = 2;
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int Year = yearCode;
                int block = blockCode;
                int collaboration = collaborationCode;
                int batch = batchCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                string mrdStatus = mrdStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                // var PropListReports = dbContext.USP_PROPDATA_CNDUP(Level, StateCode, DistrictCode, PMGSY).ToList<USP_PROPDATA_CNDUP_Result>();
                PropListReports = dbContext.Database.SqlQuery<USP_PROPDATA_CNDUP_REPORT_Result>("EXEC [omms].[USP_PROPDATA_CNDUP] @Level,@State,@District,@PMGSY",
                                 new SqlParameter("@Level", Level),
                                 new SqlParameter("@State", StateCode),
                                 new SqlParameter("@District", DistrictCode),
                                 new SqlParameter("@PMGSY", PMGSY)
                              ).ToList<USP_PROPDATA_CNDUP_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.MAST_STATE_CODE.ToString().Trim() + "$" + x.MAST_STATE_NAME.ToString().Trim() + "$" + x.MAST_DISTRICT_CODE.ToString().Trim() + "$" + x.MAST_BLOCK_CODE.ToString().Trim() + "$" + x.MAST_DISTRICT_NAME.ToString() + "$" + x.PLAN_CN_ROAD_NUMBER.ToString().Trim(),
                    cell = new[]{
                        x.MAST_STATE_NAME==null?"":x.MAST_STATE_NAME.ToString(),
                        x.MAST_DISTRICT_NAME==null?"": x.MAST_DISTRICT_NAME.ToString(),
                        x.MAST_BLOCK_NAME==null?"":x.MAST_BLOCK_NAME.ToString(),
                        x.PLAN_CN_ROAD_NUMBER==null?"":x.PLAN_CN_ROAD_NUMBER.ToString(),
                        x.DUPLICATES==null?"":x.DUPLICATES.ToString()                                                       
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropNumberBaseCNRoadDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string road, string report, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_PROPDATA_CNDUP_DETAILS_Report_Result> PropListReports;

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                string Report = report;
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int Year = yearCode;
                int block = blockCode;
                int collaboration = collaborationCode;
                int batch = batchCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                string mrdStatus = mrdStatusCode;
                int BlockCode = blockCode;
                string Road = road;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                // var PropListReports = dbContext.USP_PROPDATA_CNDUP_DETAILS(Report, StateCode, Road, PMGSY).ToList<USP_PROPDATA_CNDUP_DETAILS_Report_Result>();
                PropListReports = dbContext.Database.SqlQuery<USP_PROPDATA_CNDUP_DETAILS_Report_Result>("EXEC [omms].[USP_PROPDATA_CNDUP_DETAILS] @Report,@State,@District,@Block,@Road,@PMGSY",
                                 new SqlParameter("@Report", Report),
                                 new SqlParameter("@State", StateCode),
                                 new SqlParameter("@District", DistrictCode),
                                 new SqlParameter("@Block", BlockCode),
                                 new SqlParameter("@Road", Road),
                                 new SqlParameter("@PMGSY", PMGSY)
                              ).ToList<USP_PROPDATA_CNDUP_DETAILS_Report_Result>();

                totalRecords = PropListReports.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    //id = x.LOCATION_CODE.ToString().Trim() + "$" + x.LOCATION_NAME.ToString().Trim(),
                    cell = new[]{
                         x.MAST_DISTRICT_NAME==null?"":x.MAST_DISTRICT_NAME.ToString(),
                        x.MAST_BLOCK_NAME==null?"":x.MAST_BLOCK_NAME.ToString(),
                         x.PLAN_CN_ROAD_NUMBER==null?"":x.PLAN_CN_ROAD_NUMBER.ToString(),
                         x.PLAN_RD_NAME==null?"":x.PLAN_RD_NAME.ToString(),
                         x.PLAN_RD_NAME==null?"":x.PLAN_RD_NAME.ToString() , //Require Column Road Category Type
                         x.PLAN_RD_LENGTH==null?"":x.PLAN_RD_LENGTH.ToString(),
                        x.PLAN_RD_FROM_CHAINAGE==null?"":x.PLAN_RD_FROM_CHAINAGE.ToString() ,
                        x.PLAN_RD_TO_CHAINAGE==null?"":x.PLAN_RD_TO_CHAINAGE.ToString() 
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropMultipleDistrictListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int district = districtCode;
                int block = blockCode;
                int Year = yearCode;
                int collaboration = collaborationCode;
                int batch = batchCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPDATA_CNMUL(Sanctioned, StateCode, district, block, Year, batch, collaboration, agency, staStatus, PMGSY).ToList<USP_PROPDATA_CNMUL_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.MAST_STATE_CODE.ToString().Trim() + "$" + x.MAST_STATE_NAME.ToString().Trim() + "$" + x.MAST_DISTRICT_CODE.ToString() + "$" + x.MAST_BLOCK_CODE.ToString() + "$" + x.PLAN_CN_ROAD_NUMBER.ToString().Trim() + "$" + Sanctioned.ToString().Trim() + "$" + x.MAST_DISTRICT_NAME.ToString().Trim() + "$" + x.MAST_BLOCK_NAME.ToString().Trim(),
                    cell = new[]{
                         x.MAST_DISTRICT_NAME==null?"":x.MAST_DISTRICT_NAME.ToString(),
                         x.MAST_BLOCK_NAME==null?"":x.MAST_BLOCK_NAME.ToString(),
                        x.PLAN_CN_ROAD_NUMBER==null?"": x.PLAN_CN_ROAD_NUMBER.ToString(),
                        x.DUPLICATES==null?"":x.DUPLICATES.ToString()                                                       
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropMultipleDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string roadNumber, string sanctioned, string report, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                string Report = report;
                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int Year = yearCode;
                int District = districtCode;
                int Block = blockCode;
                string RoadNumber = roadNumber;
                int collaboration = collaborationCode;
                int batch = batchCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPDATA_CNMUL_DETAILS(Report, Sanctioned, StateCode, District, Block, Year, batch, collaboration, agency, staStatus, RoadNumber, PMGSY).ToList<USP_PROPDATA_CNMUL_DETAILS_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    cell = new[]{
                        x.MAST_DISTRICT_NAME==null?"":x.MAST_DISTRICT_NAME.ToString(),
                        x.MAST_BLOCK_NAME==null?"":x.MAST_BLOCK_NAME.ToString(),
                        x.PLAN_CN_ROAD_NUMBER==null?"":x.PLAN_CN_ROAD_NUMBER.ToString(),
                        x.IMSYEAR==null?"":x.IMSYEAR.ToString() ,
                        x.IMS_PACKAGE_ID==null?"":x.IMS_PACKAGE_ID.ToString()  ,
                        x.IMS_ROAD_NAME==null?"":x.IMS_ROAD_NAME.ToString() , 
                        x.CONSTRUCTIONTYPE==null?"":x.CONSTRUCTIONTYPE.ToString() ,
                        x.IMS_PAV_LENGTH==null?"":x.IMS_PAV_LENGTH.ToString() ,
                        x.SANC_COST==null?"":x.SANC_COST.ToString() ,
                        x.MAST_FUNDING_AGENCY_NAME==null?"":x.MAST_FUNDING_AGENCY_NAME.ToString() , 
                         x.Status==null?"":x.Status.ToString() 
                      }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropSingleHabStateListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int population, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int Batch = batchCode;
                int Population = population;
                int Year = yearCode;
                int district = districtCode;
                int block = blockCode;
                int collaboration = collaborationCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPDATA_HAB(Level, StateCode, district, block, Year, Batch, collaboration, agency, Population, staStatus, Sanctioned, PMGSY).ToList<USP_PROPDATA_HAB_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE.ToString().Trim() + "$" + x.LOCATION_NAME.ToString().Trim(),
                    cell = new[]{
                         x.LOCATION_NAME==null?"":x.LOCATION_NAME.ToString(),
                        x.ROADS==null?"":x.ROADS.ToString()                                                                           
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropSingleHabDistrictListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int population, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_PROPDATA_HAB_REPORT_Result> PropListReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int Batch = batchCode;
                int Population = population;
                int Year = yearCode;
                int District = districtCode;
                int Block = blockCode;
                int collaboration = collaborationCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //  var PropListReports = dbContext.USP_PROPDATA_HAB(Level, StateCode, Year, Batch, Population, Sanctioned, PMGSY).ToList<USP_PROPDATA_HAB_Result>();
                PropListReports = dbContext.Database.SqlQuery<USP_PROPDATA_HAB_REPORT_Result>("EXEC [omms].[USP_PROPDATA_HAB] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Agency,@Pop,@STAStatus,@Sanction,@PMGSY",
                new SqlParameter("@Level", Level),
                new SqlParameter("@State", StateCode),
                new SqlParameter("@District", District),
                new SqlParameter("@Block", Block),
                new SqlParameter("@Year", Year),
                new SqlParameter("@Batch", Batch),
                new SqlParameter("@Collaboration", collaboration),
                new SqlParameter("@Agency", agency),
                new SqlParameter("@Pop", Population),
                new SqlParameter("@STAStatus", staStatus),
                new SqlParameter("@Sanction", Sanctioned),
               new SqlParameter("@PMGSY", PMGSY)
            ).ToList<USP_PROPDATA_HAB_REPORT_Result>();


                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = StateCode.ToString().Trim() + "$" + x.LOCATION_CODE.ToString().Trim() + "$" + x.LOCATION_NAME.ToString().Trim() + "$" + x.IMS_YEAR.ToString(),
                    cell = new[]{
                        x.PHASE==null?"":x.PHASE.ToString(),
                        x.LOCATION_NAME==null?"":x.LOCATION_NAME.ToString(),                        
                         x.ROADS==null?"":x.ROADS.ToString()                                                                           
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropSingleHabDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int population, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int DistrictCode = districtCode;
                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int Batch = batchCode;
                int Population = population;
                int Year = yearCode;
                int district = districtCode;
                int block = blockCode;
                int collaboration = collaborationCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPDATA_HAB_DETAILS(StateCode, DistrictCode, block, Year, Batch, collaboration, agency, Population, staStatus, Sanctioned, PMGSY).ToList<USP_PROPDATA_HAB_DETAILS_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    cell = new[]{
                         x.MAST_DISTRICT_NAME==null?"":x.MAST_DISTRICT_NAME.ToString(),
                        x.MAST_BLOCK_NAME==null?"": x.MAST_BLOCK_NAME.ToString(),                        
                        x.PHASE==null?"": x.PHASE.ToString(),                        
                        x.IMS_PACKAGE_ID==null?"":x.IMS_PACKAGE_ID.ToString(),                        
                        x.IMS_ROAD_NAME==null?"": x.IMS_ROAD_NAME.ToString(),                        
                        x.MAST_HAB_NAME==null?"":x.MAST_HAB_NAME.ToString(),                        
                        x.MAST_HAB_TOT_POP==null?"": x.MAST_HAB_TOT_POP.ToString(),                        
                        x.ConstructionType==null?"": x.ConstructionType.ToString(),                        
                        x.IMS_PAV_LENGTH==null?"":x.IMS_PAV_LENGTH.ToString(),                       
                        x.SANC_COST==null?"":x.SANC_COST.ToString(),                        
                        PMGSY==1?"PMGSY1":"PMGSY2"                                                                         
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Array PropZeroMaintStateListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string constructionType, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int Batch = batchCode;
                string ConstructionType = constructionType;
                int Year = yearCode;
                int district = districtCode;
                int block = blockCode;
                int collaboration = collaborationCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPDATA_MAINZ(Level, StateCode, DistrictCode, block, Year, Batch, collaboration, agency, ConstructionType, staStatus, Sanctioned, PMGSY).ToList<USP_PROPDATA_MAINZ_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.MAST_STATE_CODE.ToString().Trim() + "$" + x.MAST_STATE_NAME.ToString().Trim(),
                    cell = new[]{
                        x.MAST_STATE_NAME==null?"":x.MAST_STATE_NAME.ToString(),
                        x.PROPOSALS==null?"": x.PROPOSALS.ToString()                                                                           
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropZeroMaintDistrictListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string constructionType, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_PROPDATA_MAINZ_REPORT_Result> PropListReports;

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int Block = blockCode;
                int Batch = batchCode;
                string ConstructionType = constructionType;
                int Year = yearCode;
                int collaboration = collaborationCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;

                PropListReports = dbContext.Database.SqlQuery<USP_PROPDATA_MAINZ_REPORT_Result>("EXEC [omms].[USP_PROPDATA_MAINZ] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Agency,@Type,@STAStatus,@Sanction,@PMGSY",
                new SqlParameter("@Level", Level),
                new SqlParameter("@State", StateCode),
                new SqlParameter("@District", DistrictCode),
                new SqlParameter("@Block", Block),
                new SqlParameter("@Year", Year),
                new SqlParameter("@Batch", Batch),
                new SqlParameter("@Collaboration", collaboration),
                new SqlParameter("@Agency", agency),
                new SqlParameter("@Type", constructionType),
                new SqlParameter("@STAStatus", staStatus),
                new SqlParameter("@Sanction", Sanctioned),
               new SqlParameter("@PMGSY", PMGSY)
            ).ToList<USP_PROPDATA_MAINZ_REPORT_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = StateCode.ToString().Trim() + "$" + x.MAST_DISTRICT_CODE.ToString().Trim() + "$" + x.MAST_DISTRICT_NAME.ToString().Trim() + "$" + x.IMS_YEAR.ToString(),
                    cell = new[]{
                      x.PHASE==null?"": x.PHASE.ToString(),
                       x.MAST_DISTRICT_NAME==null?"": x.MAST_DISTRICT_NAME.ToString(),
                        x.PROPOSALS==null?"":x.PROPOSALS.ToString()                                                                           
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropZeroMaintDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string constructionType, string sanctioned, string report, int page, int rows, string sidx, string sord, out int totalRecords)
        {

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                string Report = report;
                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int Batch = batchCode;
                string ConstructionType = constructionType;
                int Year = yearCode;
                int district = districtCode;
                int block = blockCode;
                int collaboration = collaborationCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPDATA_MAINZ_DETAILS(Report, StateCode, DistrictCode, block, Year, Batch, collaboration, agency, ConstructionType, staStatus, Sanctioned, PMGSY).ToList<USP_PROPDATA_MAINZ_DETAILS_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    cell = new[]{
                           x.MAST_DISTRICT_NAME==null?"": x.MAST_DISTRICT_NAME.ToString(),                     
                           x.MAST_BLOCK_NAME==null?"":x.MAST_BLOCK_NAME.ToString(),
                           x.PHASE==null?"": x.PHASE.ToString(),                     
                           x.IMS_PACKAGE_ID==null?"": x.IMS_PACKAGE_ID.ToString(),                     
                           x.IMS_ROAD_NAME==null?"": x.IMS_ROAD_NAME.ToString(),                     
                           x.ConstructionType==null?"":x.ConstructionType.ToString(),                     
                           x.IMS_PAV_LENGTH==null?"0": x.IMS_PAV_LENGTH.ToString(),                     
                           x.SANC_COST==null?"":x.SANC_COST.ToString(), 
                           PMGSY==1?"PMGSY1":"PMGSY2",
                           x.Status==null?"": x.Status.ToString()                     
                                                                                                   
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropCarriageWidthStateListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int carriageWidth, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_PROPDATA_CARRIAGE_Report> PropListReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 1;
                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int Block = blockCode;
                int Batch = batchCode;
                int CarriageWidth = carriageWidth;
                int Year = yearCode;              
                int collaboration = collaborationCode;
                int agency = agencyCode;
                string staStatus =staStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //var PropListReports = dbContext.USP_PROPDATA_CARRIAGE(Level, StateCode, DistrictCode, Year, Batch, CarriageWidth, Sanctioned, PMGSY).ToList<USP_PROPDATA_CARRIAGE_Result>();
                PropListReports = dbContext.Database.SqlQuery<USP_PROPDATA_CARRIAGE_Report>("EXEC [omms].[USP_PROPDATA_CARRIAGE] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Agency,@Carriage,@STAStatus,@Sanction,@PMGSY",
                                 new SqlParameter("@Level", Level),
                                 new SqlParameter("@State", StateCode),
                                 new SqlParameter("@District", DistrictCode),
                                 new SqlParameter("@Block", Block),
                                 new SqlParameter("@Year", Year),
                                 new SqlParameter("@Batch", Batch),
                                 new SqlParameter("@Collaboration", collaboration),
                                 new SqlParameter("@Agency", agency),
                                 new SqlParameter("@Carriage", CarriageWidth),
                                 new SqlParameter("@STAStatus", staStatus),
                                 new SqlParameter("@Sanction", Sanctioned),
                                 new SqlParameter("@PMGSY", PMGSY)
                                 ).ToList<USP_PROPDATA_CARRIAGE_Report>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.LOCATION_CODE.ToString().Trim() + "$" + x.LOCATION_NAME.ToString().Trim() + "$" + x.MAST_CARRIAGE_CODE.ToString() + "$" + x.CarriageWidth.ToString(),
                    cell = new[]{
                       x.LOCATION_NAME==null?"": x.LOCATION_NAME.ToString(),
                        x.CarriageWidth==null?"": x.CarriageWidth.ToString(),
                        x.Proposals==null?"0": x.Proposals.ToString()                                                   
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropCarriageWidthDistrictListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int carriageWidth, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_PROPDATA_CARRIAGE_Report> PropListReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Level = 2;
                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int Block = blockCode;
                int Batch = batchCode;
                int CarriageWidth = carriageWidth;
                int Year = yearCode;              
                int collaboration = collaborationCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                //  var PropListReports = dbContext.USP_PROPDATA_CARRIAGE(Level, StateCode, DistrictCode, Year, Batch, CarriageWidth, Sanctioned, PMGSY).ToList<USP_PROPDATA_CARRIAGE_Result>();
                //PropListReports = dbContext.Database.SqlQuery<USP_PROPDATA_CARRIAGE_Report>("EXEC [omms].[USP_PROPDATA_CARRIAGE] @Level,@State,@District,@Year,@Batch,@Carriage,@Sanction,@PMGSY",
                //   new SqlParameter("@Level", Level),
                //   new SqlParameter("@State", StateCode),
                //   new SqlParameter("@District", DistrictCode),
                //   new SqlParameter("@Year", Year),
                //   new SqlParameter("@Batch", Batch),
                //   new SqlParameter("@Carriage", CarriageWidth),
                //   new SqlParameter("@Sanction", Sanctioned),
                //   new SqlParameter("@PMGSY", PMGSY)
                //  ).ToList<USP_PROPDATA_CARRIAGE_Report>();

                PropListReports = dbContext.Database.SqlQuery<USP_PROPDATA_CARRIAGE_Report>("EXEC [omms].[USP_PROPDATA_CARRIAGE] @Level,@State,@District,@Block,@Year,@Batch,@Collaboration,@Agency,@Carriage,@STAStatus,@Sanction,@PMGSY",
                                   new SqlParameter("@Level", Level),
                                   new SqlParameter("@State", StateCode),
                                   new SqlParameter("@District", DistrictCode),
                                   new SqlParameter("@Block", Block),
                                   new SqlParameter("@Year", Year),
                                   new SqlParameter("@Batch", Batch),
                                   new SqlParameter("@Collaboration", collaboration),
                                   new SqlParameter("@Agency", agency),
                                   new SqlParameter("@Carriage", CarriageWidth),
                                   new SqlParameter("@STAStatus", staStatus),
                                   new SqlParameter("@Sanction", Sanctioned),
                                   new SqlParameter("@PMGSY", PMGSY)
                                  ).ToList<USP_PROPDATA_CARRIAGE_Report>();


                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = StateCode.ToString().Trim() + "$" + x.LOCATION_CODE.ToString().Trim() + "$" + x.LOCATION_NAME.ToString().Trim() + "$" + x.MAST_CARRIAGE_CODE.ToString() + "$" + x.IMS_YEAR.ToString(),
                    cell = new[]{
                          x.PHASE==null?"":x.PHASE.ToString(),// Phase
                         x.LOCATION_NAME==null?"": x.LOCATION_NAME.ToString(), //DistrictName
                          x.Proposals==null?"0": x.Proposals.ToString(),  //Total Proposal
                         x.CarriageWidth==null?"": x.CarriageWidth.ToString(), //Total Pavment Length
                         x.CarriageWidth==null?"": x.CarriageWidth.ToString()    //Total Sanctioned Cost                                               
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public Array PropCarriageWidthDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int carriageWidth, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int Batch = batchCode;
                int CarriageWidth = carriageWidth;
                int Year = yearCode;
                int district = districtCode;
                int block = blockCode;
                int collaboration = collaborationCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPDATA_CARRIAGE_DETAILS(StateCode, DistrictCode, block, Year, Batch, collaboration, agency, CarriageWidth, staStatus, Sanctioned, PMGSY).ToList<USP_PROPDATA_CARRIAGE_DETAILS_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    cell = new[]{
                        x.MAST_DISTRICT_NAME==null?"": x.MAST_DISTRICT_NAME.ToString(),
                        x.MAST_BLOCK_NAME==null?"": x.MAST_BLOCK_NAME.ToString(),
                        x.PHASE==null?"": x.PHASE.ToString(),
                        x.IMS_PACKAGE_ID==null?"": x.IMS_PACKAGE_ID.ToString(),
                        x.IMS_ROAD_NAME==null?"": x.IMS_ROAD_NAME.ToString(),
                        x.ConstructionType==null?"": x.ConstructionType.ToString(),
                        x.IMS_PAV_LENGTH.ToString(),
                        x.SANC_COST==null?"": x.SANC_COST.ToString(),
                        PMGSY==1?"PMGSY1":"PMGSY2",
                         x.Status==null?"":x.Status.ToString()                                                                        
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropVariationLengthListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                string Sanctioned = sanctioned;
                int StateCode = stateCode;
                int Batch = batchCode;
                int Year = yearCode;
                int district = districtCode;
                int block = blockCode;
                int collaboration = collaborationCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPDATA_CNLEN(StateCode, district, block, Year, Batch, collaboration, agency, staStatus, Sanctioned, PMGSY).ToList<USP_PROPDATA_CNLEN_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    cell = new[]{
                        x.MAST_DISTRICT_NAME==null?"": x.MAST_DISTRICT_NAME.ToString(),
                       x.MAST_BLOCK_NAME==null?"": x.MAST_BLOCK_NAME.ToString(),
                       x.CONN_TYPE==null?"": x.CONN_TYPE.ToString(),
                        x.PLAN_CN_ROAD_NUMBER==null?"": x.PLAN_CN_ROAD_NUMBER.ToString(),
                       x.IMS_ROAD_NAME==null?"": x.IMS_ROAD_NAME.ToString(),
                       x.IMS_ROAD_FROM==null?"":  x.IMS_ROAD_FROM.ToString(),                                                   
                       x.IMS_ROAD_TO==null?"":  x.IMS_ROAD_TO.ToString(),                                                   
                       x.IMS_PAV_LENGTH==null?"0": x.IMS_PAV_LENGTH.ToString(),                                                   
                       x.CN_LENGTH==null?"0": x.CN_LENGTH.ToString(),                                                   
                       x.PREV_CN_LENGTH==null?"0": x.PREV_CN_LENGTH.ToString(),                                                   
                       x.EXTRA_LENGTH==null?"0": x.EXTRA_LENGTH.ToString(),                                                   
                        x.PER_EXTRA==null?"0": x.PER_EXTRA.ToString()                                       
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropMisclassificationListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                int StateCode = stateCode;
                int district = districtCode;
                int block = blockCode;
                int Year = yearCode;
                int collaboration = collaborationCode;
                int batch = batchCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                string mrdStatus = mrdStatusCode;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPDATA_MISCLASS(StateCode, district, block, Year, batch, collaboration, agency, staStatus, mrdStatus, PMGSY).ToList<USP_PROPDATA_MISCLASS_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {
                    id = x.MAST_STATE_CODE.ToString().Trim() + "$" + x.MAST_STATE_NAME.ToString().Trim() + "$" + x.IMS_YEAR.ToString().Trim() + "$" + x.PHASE.ToString().Trim(),
                    cell = new[]{
                       x.MAST_STATE_NAME==null?"": x.MAST_STATE_NAME.ToString(),
                        x.PHASE==null?"": x.PHASE.ToString(),
                       x.ROADS==null?"": x.ROADS.ToString(),
                        x.P_ROAD_LENGTH==null?"0": x.P_ROAD_LENGTH.ToString(),
                        x.P_BRIDGE_LENGTH==null?"0": x.P_BRIDGE_LENGTH.ToString(),                                                   
                         x.P_PAV_COST==null?"0":x.P_PAV_COST.ToString(),                                                   
                       x.P_BRIDGE_COST==null?"0":  x.P_BRIDGE_COST.ToString(),                                                   
                        x.TOTAL_ROAD==null?"0": x.TOTAL_ROAD.ToString(),                                                   
                        x.BRIDGES==null?"0": x.BRIDGES.ToString(),                                                   
                        x.B_ROAD_LENGTH==null?"0": x.B_ROAD_LENGTH.ToString(),                                                   
                       x.B_BRIDGE_LENGTH==null?"0":  x.B_BRIDGE_LENGTH.ToString(),  
                       x.B_BRIDGE_COST==null?"0":  x.B_BRIDGE_COST.ToString(),                                       
                        x.B_PAV_COST==null?"0": x.B_PAV_COST.ToString(),                                       
                        x.TOTAL_BRIDGE==null?"0": x.TOTAL_BRIDGE.ToString(),                                       
                        x.TOTAL_VALUE==null?"0": x.TOTAL_VALUE.ToString()                          
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropMisclassificationDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string type, string report, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                string Type = type;
                int StateCode = stateCode;
                int Year = yearCode;
                string Report = report;
                int district = districtCode;
                int block = blockCode;
                int collaboration = collaborationCode;
                int batch = batchCode;
                int agency = agencyCode;
                string staStatus = staStatusCode;
                string mrdStatus = mrdStatusCode;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROPDATA_MISCLASS_DETAILS(Report, StateCode, district, block, Year, batch, collaboration, agency, type, staStatus, mrdStatus, PMGSY).ToList<USP_PROPDATA_MISCLASS_DETAILS_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{
                        x.IMS_PACKAGE_ID==null?"": x.IMS_PACKAGE_ID.ToString(),
                       x.Stream==null?"": x.Stream.ToString(),
                       x.IMS_ROAD_NAME==null?"": x.IMS_ROAD_NAME.ToString(),
                       x.ConstructionType==null?"": x.ConstructionType.ToString(),
                       x.TotalLength==null?"0": x.TotalLength.ToString(),                                                   
                       x.PAV_COST==null?"0":  x.PAV_COST.ToString(),                                                   
                       x.BRIDGE_LENGTH==null?"0": x.BRIDGE_LENGTH.ToString(),                                                   
                       x.BRIDGE_COST==null?"0": x.BRIDGE_COST.ToString(),                                                   
                       x.Status==null?"":  x.Status.ToString()                         
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

#endregion
        public Array PCIAbstractAnalysisListingDAL(int stateCode, int districtCode, int blockCode, string flag, string routeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string RouteType = routeType;
                string Flage = flag;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;

                var listPCIAbstractReports = dbContext.USP_CN_PCI_ANALYSIS(State, District, Block, Flage, RouteType, PMGSY).ToList<USP_CN_PCI_ANALYSIS_Result>();

                totalRecords = listPCIAbstractReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listPCIAbstractReports = listPCIAbstractReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listPCIAbstractReports = listPCIAbstractReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listPCIAbstractReports = listPCIAbstractReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listPCIAbstractReports.Select(x => new
                {
                    id = x.LOCATION_CODE.ToString() + "$" + x.LOCATION_NAME.ToString() + "$" + x.PCI_LENGTH.ToString(),
                    cell = new[]{
                       // "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PCIAbstractDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                      x.LOCATION_NAME==null?"":  x.LOCATION_NAME.ToString(),
                      x.LINK_ROADS==null?"": x.LINK_ROADS.ToString(),
                       x.PLAN_RD_LENGTH==null?"": x.PLAN_RD_LENGTH.ToString(),
                        x.PCI_ROAD==null?"": x.PCI_ROAD.ToString(),
                      x.PCI_LENGTH==null?"0": x.PCI_LENGTH.ToString(),
                       x.LEN_ONE==null?"0": x.LEN_ONE.ToString(),
                       x.LEN_TWO==null?"0": x.LEN_TWO.ToString(),
                       x.LEN_THREE==null?"0": x.LEN_THREE.ToString(),
                        x.LEN_FOUR==null?"0": x.LEN_FOUR.ToString(),                    
                      x.LEN_FIVE==null?"0": x.LEN_FIVE.ToString(),
                        x.TOT_LENGTH_YEAR1==null?"":x.TOT_LENGTH_YEAR1.ToString(),
                       x.TOT_LENGTH_YEAR2==null?"": x.TOT_LENGTH_YEAR2.ToString(),
                       x.TOT_LENGTH_YEAR3==null?"": x.TOT_LENGTH_YEAR3.ToString()
                    
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<USP_CN_PCI_ANALYSIS_Result> GetPCIAnalysisChartDAL(int stateCode, int districtCode, int blockCode, string flag, string routeType)
        {

            try
            {
                dbContext = new PMGSYEntities();
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string RouteType = routeType;
                string Flage = flag;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;

                List<USP_CN_PCI_ANALYSIS_Result> resultlist = dbContext.USP_CN_PCI_ANALYSIS(State, District, Block, Flage, RouteType, PMGSY).ToList<USP_CN_PCI_ANALYSIS_Result>();

                return resultlist;


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PropAssetValueListingDAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {


                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROP_ASSET_VALUE(PMGSY).ToList<USP_PROP_ASSET_VALUE_Result>();

                totalRecords = PropListReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{
                        x.MAST_STATE_NAME.ToString(),
                        x.IMS_YEAR.ToString(),
                        x.NC_Value.ToString(),
                        x.NC_Length.ToString(),
                        x.NC_Avg.ToString(),
                        x.UP_Value.ToString(),
                        x.UP_Length.ToString(),
                        x.UP_Avg.ToString(),
                        x.NC_Avg_3Years.ToString(),
                        x.NC_Asset_Value.ToString(),
                        x.UG_Asset_Value.ToString(),
                        x.Total_Assest_Value.ToString(), //Current Replacement Column
                        x.Maint_Fund_Req.ToString(),
                        x.Maint_Fund_Recv.ToString(),
                        x.Maint_Fund_Avg_3Years.ToString(),
                        x.Per_Maint_Fund_Recv.ToString()                                                                  
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array ExecutionFinancialProgressListingDAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string type, string progress, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int BlockCode = blockCode;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Type = type; //'P'
                string Progress = progress; //R- Excution Financial Progress
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROP_FIN_REPORT(StateCode, DistrictCode, BlockCode, Year, Batch, Collaboration, Type, Progress, PMGSY).ToList<USP_PROP_FIN_REPORT_Result>();

                totalRecords = PropListReports.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{
                       x.MAST_DISTRICT_NAME==null?"NA": x.MAST_DISTRICT_NAME.ToString(),
                       x.MAST_BLOCK_NAME==null?"NA": x.MAST_BLOCK_NAME.ToString(),
                       x.IMS_YEAR==null?"NA":x.IMS_YEAR.ToString(), 
                       x.IMS_PACKAGE_ID==null?"NA": x.IMS_PACKAGE_ID.ToString(),
                       x.ROAD_NAME==null?"NA": x.ROAD_NAME.ToString(),
                       x.ROAD_LENGTH.ToString(),//Road/Bridge Length                  
                       (x.ROAD_AMT+x.ROAD_STATE+x.BRIDGE_AMT+x.BRIDGE_STATE).ToString(),//Sanction Cost
                       x.MAINT_AMT.ToString(),//Maintenance Cost
                       (x.EXEC_VALUEOFWORK_LASTMONTH+x.EXEC_VALUEOFWORK_THISMONTH).ToString(),//Value of Work
                       (x.EXEC_PAYMENT_THISMONTH+x.EXEC_PAYMENT_LASTMONTH).ToString(),//Payment Made as Expendditure
                       x.PROGRESS_PERIOD.ToString(),//Progress as on
                       x.EXEC_FINAL_PAYMENT_FLAG==null?"NA":x.EXEC_FINAL_PAYMENT_FLAG.ToString(), //is Final Payment Flag
                       x.EXEC_FINAL_PAYMENT_DATE==null?"NA":Convert.ToDateTime(x.EXEC_FINAL_PAYMENT_DATE).ToString("dd/MM/yyyy")   //is Final Payment Date                                             
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array MaintenanceFinancialProgressListingDAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string type, string progress, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            List<USP_PROPOSAL_MAINTENANCE_FIN_REPORT_Result> PropListReports;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int BlockCode = blockCode;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Type = type; //'P'
                string Progress = progress; //M- Maintenance Financial Progress
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                PropListReports = dbContext.Database.SqlQuery<USP_PROPOSAL_MAINTENANCE_FIN_REPORT_Result>("EXEC [omms].[USP_PROP_FIN_REPORT] @State,@District,@Block,@Year,@Batch,@Collaboration,@Type,@Progress,@PMGSY",
                                new SqlParameter("@State", StateCode),
                                new SqlParameter("@District", DistrictCode),
                                new SqlParameter("@Block", BlockCode),
                                new SqlParameter("@Year", Year),
                                new SqlParameter("@Batch", Batch),
                                new SqlParameter("@Collaboration", Collaboration),
                                new SqlParameter("@Type", Type),
                                new SqlParameter("@Progress", Progress),
                                new SqlParameter("@PMGSY", PMGSY)
                               ).ToList<USP_PROPOSAL_MAINTENANCE_FIN_REPORT_Result>();


                totalRecords = PropListReports.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{                      
                           x.MAST_DISTRICT_NAME==null?"NA": x.MAST_DISTRICT_NAME.ToString(),
                           x.MAST_BLOCK_NAME==null?"NA": x.MAST_BLOCK_NAME.ToString(),
                           x.IMS_YEAR==null?"NA":x.IMS_YEAR.ToString(), 
                           x.IMS_PACKAGE_ID==null?"NA": x.IMS_PACKAGE_ID.ToString(),
                           x.ROAD_NAME==null?"NA": x.ROAD_NAME.ToString(),
                           x.ROAD_LENGTH.ToString(),//Road/Bridge Length                  
                           (x.ROAD_AMT+x.ROAD_STATE+x.BRIDGE_AMT+x.BRIDGE_STATE).ToString(),//Sanction Cost
                           x.MAINT_AMT.ToString(),//Maintenance Cost
                           (x.MANE_VALUEOFWORK_LASTMONTH+x.MANE_PAYMENT_THISMONTH).ToString(),//Value of Work
                           (x.MANE_PAYMENT_THISMONTH+x.MANE_PAYMENT_LASTMONTH).ToString(),//Payment Made as Expendditure
                           x.PROGRESS_PERIOD.ToString(),//Progress as on
                           x.MANE_FINAL_PAYMENT_FLAG==null?"NA":x.MANE_FINAL_PAYMENT_FLAG.ToString(), //is Final Payment Flag
                           x.MANE_FINAL_PAYMENT_DATE==null?"NA":Convert.ToDateTime(x.MANE_FINAL_PAYMENT_DATE).ToString("dd/MM/yyyy"),   //is Final Payment Date             
                         }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array MaintenanceAgreementListingDAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int BlockCode = blockCode;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Status = status;

                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROP_MAINT_AGREEMENT_LIST(StateCode, DistrictCode, BlockCode, Year, Batch, Collaboration, Status, PMGSY).ToList<USP_PROP_MAINT_AGREEMENT_LIST_Result>();

                totalRecords = PropListReports.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{                      
                       //x.MAST_BLOCK_NAME==null?"NA": x.MAST_BLOCK_NAME.ToString(),
                       x.IMS_PACKAGE_ID==null?"NA": x.IMS_PACKAGE_ID.ToString(),
                       x.IMS_YEAR==null?"NA":x.IMS_YEAR.ToString(),
                       x.ROAD_NAME==null?"NA": x.ROAD_NAME.ToString(),                       
                       x.MANE_CONSTR_COMP_DATE==null?"NA":Convert.ToDateTime(x.MANE_CONSTR_COMP_DATE).ToString("dd/MM/yyyy"),
                       x.MANE_MAINTENANCE_START_DATE==null?"NA": Convert.ToDateTime(x.MANE_MAINTENANCE_START_DATE).ToString("dd/MM/yyyy"),
                       ((x.COMPANY_NAME==null?"":"Company Name -"+x.COMPANY_NAME.ToString())+(x.CONTRACTOR_NAME==null?"":", Contractor Name -"+x.CONTRACTOR_NAME.ToString())+(x.CONTRACTOR_PAN==null?"":", PAN No -"+x.CONTRACTOR_PAN.ToString())),
                       x.MAINT_AMT.ToString() //Maintanance Amount                                                                       
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array MaintenanceInspectionListingDAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string type, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int BlockCode = blockCode;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                string Type = type;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROP_INSP_REPORT(StateCode, DistrictCode, BlockCode, Year, Batch, Collaboration, Type, PMGSY).ToList<USP_PROP_INSP_REPORT_Result>();

                totalRecords = PropListReports.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{               
                       x.IMS_ROAD_NAME==null?"NA": x.IMS_ROAD_NAME.ToString(), 
                       x.IMS_PACKAGE_ID==null?"NA": x.IMS_PACKAGE_ID.ToString(),
                       x.IMS_YEAR==null?"NA":x.IMS_YEAR.ToString(),                                          
                       x.IMS_PAV_LENGTH.ToString(),//length of Road
                       x.MANE_INSP_DATE==null?"NA":Convert.ToDateTime( x.MANE_INSP_DATE).ToString("dd/MM/yyyy"),
                       x.MANE_RECTIFICATION_DATE==null?"":Convert.ToDateTime( x.MANE_RECTIFICATION_DATE).ToString("dd/MM/yyyy")                                                  
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array FundSanctionReleaseListingDAL(int stateCode, int year, int collaboration, int agency, string fund, string type, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int StateCode = stateCode;
                int Year = year;
                int Agency = agency;
                int Collaboration = collaboration;
                string Fund = fund;
                string Type = type;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_FUND_RELEASE_REPORT(StateCode, Year, Collaboration, Agency, Fund, Type).ToList<USP_FUND_RELEASE_REPORT_Result>();

                totalRecords = PropListReports.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.PHASE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{               
                       x.PHASE==null?"NA": x.PHASE.ToString(),  //PHASE
                       x.MAST_TRANSACTION_NO==null?"NA": x.MAST_TRANSACTION_NO.ToString(), //Installation No
                       x.RELEASE_YEAR==null?"NA":x.RELEASE_YEAR.ToString(),       //Financial Year                                    
                       x.MAST_AGENCY_NAME.ToString(), //Executing Agency
                       x.MAST_FUNDING_AGENCY_NAME==null?"NA":x.MAST_FUNDING_AGENCY_NAME.ToString(), //Collaboration
                       x.MAST_RELEASE_AMOUNT.ToString(),//Amount Released
                       x.RELEASE_DATE==null?"":Convert.ToDateTime( x.RELEASE_DATE).ToString("dd/MM/yyyy"),
                       x.RELEASE_ORDER.ToString() //Sanction No

                   }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array TeanderAgreementListingDAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, int conId, string package, string status, string agreement, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int StateCode = stateCode;
                int DistrictCode = districtCode;
                int BlockCode = blockCode;
                int Year = year;
                int Batch = batch;
                int Collaboration = collaboration;
                int ConId = conId;
                string Package = package;
                string Status = status;
                string Agreement = agreement;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var PropListReports = dbContext.USP_PROP_AGREEMENT_LIST(StateCode, DistrictCode, BlockCode, Year, Batch, Collaboration, ConId, Package, Status, Agreement, PMGSY).ToList<USP_PROP_AGREEMENT_LIST_Result>();

                totalRecords = PropListReports.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        PropListReports = PropListReports.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    PropListReports = PropListReports.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return PropListReports.Select(x => new
                {

                    cell = new[]{                      
                       x.MAST_DISTRICT_NAME==null?"NA": x.MAST_DISTRICT_NAME.ToString(),
                       x.IMS_YEAR==null?"NA":x.IMS_YEAR.ToString(),                       
                       x.IMS_PACKAGE_ID==null?"NA": x.IMS_PACKAGE_ID.ToString(),
                       x.CONTRACTOR_NAME==null?"NA":x.CONTRACTOR_NAME.ToString() +" ("+x.MAST_CON_ID+")",                      
                       x.TEND_DATE_OF_WORK_ORDER==null?"NA":Convert.ToDateTime(x.TEND_DATE_OF_WORK_ORDER).ToString("dd/MM/yyyy"), //Date of Work Order
                       x.TEND_DATE_OF_COMPLETION==null?"NA":Convert.ToDateTime(x.TEND_DATE_OF_COMPLETION).ToString("dd/MM/yyyy"), // Date of Completion
                       x.TEND_AGREEMENT_NUMBER==null?"NA":x.TEND_AGREEMENT_NUMBER.ToString(),     //Agreement Mo               
                       x.TEND_DATE_OF_AGREEMENT==null?"NA":Convert.ToDateTime(x.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),      //Date of Agreement   
                       x.TOTAL_AGREEMENT_COST.ToString()                                            
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


    }

    public interface IProposalReportsDAL
    {

        PMGSY.Models.ProposalReports.MRDProposalModel GetMRDProposalHabCovgDAL(PMGSY.Models.ProposalReports.MRDProposalModel objParam);

        List<PMGSY.Models.ProposalReports.RoadCBRListing> GetMRDProposalRoadCBRDetailsDAL(int roadCode);
        //MRDProposal Report
        PMGSY.Models.ProposalReports.MRDProposalModel GetMRDProposalDAL(PMGSY.Models.ProposalReports.MRDProposalModel objParam);
        //MRDProposal Bridge Details Report
        List<PMGSY.Models.ProposalReports.BridgeListing> GetMRDProposalBridgeDetailsDAL(int roadCode);

        List<PMGSY.Models.ProposalReports.BridgeCostListing> GetMRDProposalBridgeCostDetailsDAL(int roadCode);

        List<PMGSY.Models.ProposalReports.BridgeEstCostListing> GetMRDProposalBridgeEstCostDetailsDAL(int roadCode);

        // MPR 1
        Array MPR1StateListingDAL(int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPR1DistrictListingDAL(int stateCode, int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPR1BlockListingDAL(int districtCode, int stateCode, int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPR1FinalListingDAL(int blockCode, int districtCode, int stateCode, int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords);

        // MPR 2
        Array MPR2StateListingDAL(int month, int year, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPR2DistrictListingDAL(int month, int year, int collaboration, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPR2BlockListingDAL(int month, int year, int collaboration, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPR2FinalListingDAL(int month, int year, int collaboration, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        // HY 1
        Array HY1StateListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array HY1DistrictListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        // HY 2
        Array HY2StateListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array HY2DistrictListingDAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        // Prop List
        Array PropListStateListingDAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropListDistrictListingDAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropListBlockListingDAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropListFinalListingDAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);

        // Prop Sanction List
        Array PropSanctionStateListingDAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSanctionDistrictListingDAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSanctionBlockListingDAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSanctionFinalListingDAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);

        // Prop Sanction Length
        Array PropSanctionLengthStateListingDAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSanctionLengthDistrictListingDAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSanctionLengthBlockListingDAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSanctionLengthFinalListingDAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);

        // Prop Estimated Maintanance Cost
        Array PropEMCStateListingDAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropEMCDistrictListingDAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropEMCBlockListingDAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropEMCFinalListingDAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);


        // Prop Scrutiny 
        Array PropScrutinyListingDAL(int stateCode, string type, int agencyId, int year, int batch, int scheme, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropScrutinyTASDListingDAL(int stateCode, int districtCode, string type, int agencyId, int year, int batch, int scheme, string taName, int page, int rows, string sidx, string sord, out int totalRecords);
        // Pending Works Details 
        Array PendingWorksListingDAL(int stateCode, string reason, int page, int rows, string sidx, string sord, out int totalRecords);
        // Proposal Analysis Details 
        Array PropAnalysisDataGapListingDAL(int stateCode, string type, string scrutinity, string sanction, string report, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropAnalysisDetailListingDAL(int stateCode, int year, int batch, string type, string scrutinity, string sanction, string report, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropAnalysisHabitationListingDAL(int roadCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropAnalysisTrafficListingDAL(int roadCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropAnalysisCBRListingDAL(int roadCode, int page, int rows, string sidx, string sord, out int totalRecords);


        //Proposal DataGap Details
        #region Proposal Datagap Details
        Array PropNotMappedStateListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropNotMappedDistrictListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropNotMappedDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropNotMappedPhaseViewListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);

        Array PropNumberBaseCNDistrictListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropNumberBaseCNRoadDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string road, string report, int page, int rows, string sidx, string sord, out int totalRecords);

        Array PropMultipleDistrictListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropMultipleDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string roadNumber,string sanctioned, string report, int page, int rows, string sidx, string sord, out int totalRecords);

        Array PropSingleHabStateListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int population, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSingleHabDistrictListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int population, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSingleHabDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int population, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);

        Array PropZeroMaintStateListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string constructionType, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropZeroMaintDistrictListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string constructionType, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropZeroMaintDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string constructionType, string sanctioned, string report, int page, int rows, string sidx, string sord, out int totalRecords);

        Array PropCarriageWidthStateListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int carriageWidth, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropCarriageWidthDistrictListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int carriageWidth, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropCarriageWidthDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int carriageWidth, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);

        Array PropVariationLengthListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropMisclassificationListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropMisclassificationDetailsListingDAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string type, string report, int page, int rows, string sidx, string sord, out int totalRecords);
        #endregion

        //PCI Analysis
        Array PCIAbstractAnalysisListingDAL(int stateCode, int districtCode, int blockCode, string flag, string routeType, int page, int rows, string sidx, string sord, out int totalRecords);
        List<USP_CN_PCI_ANALYSIS_Result> GetPCIAnalysisChartDAL(int stateCode, int districtCode, int blockCode, string flag, string routeType);

        Array PropAssetValueListingDAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ExecutionFinancialProgressListingDAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string type, string progress, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MaintenanceFinancialProgressListingDAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string type, string progress, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MaintenanceAgreementListingDAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MaintenanceInspectionListingDAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string type, int page, int rows, string sidx, string sord, out int totalRecords);
        Array FundSanctionReleaseListingDAL(int stateCode, int year, int collaboration, int agency, string fund, string type, int page, int rows, string sidx, string sord, out int totalRecords);
        Array TeanderAgreementListingDAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, int conId, string package, string status, string agreement, int page, int rows, string sidx, string sord, out int totalRecords);

    }
}