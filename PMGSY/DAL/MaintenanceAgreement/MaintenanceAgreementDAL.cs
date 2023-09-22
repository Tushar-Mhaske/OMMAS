/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: MaintenanceAgreementDAL.cs

 * Author : Koustubh Nakate

 * Creation Date :21/June/2013

 * Desc : This class is used as data access layer to perform Save,Edit,Delete and listing of maintenance agreement screens.  
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Common;
using PMGSY.Models.MaintenanceAgreement;
using System.Transactions;
using System.Data.Entity;
using System.Web.Mvc;
using System.Text;
using PMGSY.Extensions;
//using System.Data.Entity.Core.SqlClient;
using System.Data.SqlClient;
using PMGSY.Models.Proposal;
using PMGSY.Models.Common;
using System.Text.RegularExpressions;
using System.Data.Entity.Core;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Reflection;
using System.Data.Entity.Validation;
using System.Configuration;

namespace PMGSY.DAL.MaintenanceAgreement
{
    public enum MaintenanceAgreementModules
    {
        IMSContract = 1,
        CNContract
    };

    public class MaintenanceAgreementDAL : IMaintenanceAgreementDAL
    {
        private readonly Dictionary<String, String> AgreementStatus = new Dictionary<string, string>() { { "P", "In Progress" }, { "I", "Terminate" }, { "C", "Agreement Completed" } };
        string errorLogPath = ConfigurationSettings.AppSettings["OMMASErrorLogPath"];
        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;

        #region MAINTENANCE_AGREEMENT


        /// <summary>
        /// This function is used to calculated max code
        /// </summary>
        /// <param name="module"> MasterDataEntryModules object</param>
        /// <returns> MaxCode</returns>

        private Int64 GetMaxCode(MaintenanceAgreementModules module, int IMSPRRoadCode)
        {
            Int64? maxCode = 0;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                switch (module)
                {
                    case MaintenanceAgreementModules.IMSContract:
                        //working code commenetd by Koustubh Nakate on 31/10/2013  
                        //  maxCode = (from IMSContract in dbContext.MANE_IMS_CONTRACT where IMSContract.IMS_PR_ROAD_CODE == IMSPRRoadCode select (Int64?)IMSContract.MANE_PR_CONTRACT_CODE).Max();

                        maxCode = (from IMSContract in dbContext.MANE_IMS_CONTRACT select (Int64?)IMSContract.MANE_PR_CONTRACT_CODE).Max();
                        break;

                    case MaintenanceAgreementModules.CNContract:
                        maxCode = (from agreementDetail in dbContext.TEND_AGREEMENT_DETAIL select (Int64?)agreementDetail.TEND_AGREEMENT_ID).Max();
                        break;
                }

                if (maxCode == null)
                {
                    maxCode = 1;
                }
                else
                {
                    maxCode = maxCode + 1;
                }

                return (Int64)maxCode;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return 0;
            }
            //finally
            //{
            //    if (dbContext != null)
            //    {
            //        dbContext.Dispose();
            //    }
            //}

        }


        //public Array GetCompletedRoadListDAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        //{
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    try
        //    {

        //        if (packageID.Contains("All"))
        //        {
        //            packageID = "All Packages";
        //        }

        //        var query = (from imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
        //                     join execRoads in dbContext.EXEC_ROADS_MONTHLY_STATUS
        //                     on imsSanctionedProjectDetails.IMS_PR_ROAD_CODE equals execRoads.IMS_PR_ROAD_CODE
        //                     join blockDetails in dbContext.MASTER_BLOCK
        //                     on imsSanctionedProjectDetails.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
        //                     join districtDetails in dbContext.MASTER_DISTRICT
        //                     on imsSanctionedProjectDetails.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
        //                     join stateDetails in dbContext.MASTER_STATE
        //                     on imsSanctionedProjectDetails.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
        //                     join adminDetails in dbContext.ADMIN_DEPARTMENT
        //                     on imsSanctionedProjectDetails.MAST_DPIU_CODE equals adminDetails.ADMIN_ND_CODE
        //                     join fundingAgency in dbContext.MASTER_FUNDING_AGENCY
        //                     on imsSanctionedProjectDetails.IMS_COLLABORATION equals fundingAgency.MAST_FUNDING_AGENCY_CODE into agencies
        //                     from fundingAgency in agencies.DefaultIfEmpty()
        //                     where
        //                     imsSanctionedProjectDetails.MAST_STATE_CODE == stateCode &&
        //                     imsSanctionedProjectDetails.MAST_DISTRICT_CODE == districtCode &&
        //                     imsSanctionedProjectDetails.MAST_DPIU_CODE == adminNDCode &&
        //                     imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
        //                     (imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "P" || imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "L") &&
        //                     execRoads.EXEC_ISCOMPLETED == "C" &&
        //                     (sanctionedYear == 0 ? 1 : imsSanctionedProjectDetails.IMS_YEAR) == (sanctionedYear == 0 ? 1 : sanctionedYear) &&
        //                     (blockCode == 0 ? 1 : imsSanctionedProjectDetails.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
        //                     (packageID == "All Packages" ? "%" : imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper()) == (packageID == "All Packages" ? "%" : packageID.ToUpper()) &&
        //                         //new filters added by Vikram 
        //                     (batch == 0 ? 1 : imsSanctionedProjectDetails.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
        //                     (collaboration <= 0 ? 1 : imsSanctionedProjectDetails.IMS_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
        //                     (upgradationType == "0" ? "%" : imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT) == (upgradationType == "0" ? "%" : upgradationType) &&
        //                         //end of change
        //                     imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //new change done by Vikram on 10 Feb 2014
        //                     && imsSanctionedProjectDetails.IMS_DPR_STATUS == "N" //new change done by Vikram
        //                     select new
        //                     {
        //                         imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
        //                         imsSanctionedProjectDetails.IMS_ROAD_NAME,
        //                         imsSanctionedProjectDetails.IMS_ROAD_FROM,
        //                         imsSanctionedProjectDetails.IMS_ROAD_TO,
        //                         imsSanctionedProjectDetails.IMS_YEAR,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
        //                         imsSanctionedProjectDetails.IMS_LOCK_STATUS,
        //                         imsSanctionedProjectDetails.IMS_SANCTIONED,
        //                         imsSanctionedProjectDetails.IMS_PACKAGE_ID,
        //                         imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
        //                         imsSanctionedProjectDetails.IMS_PAV_LENGTH,
        //                         fundingAgency.MAST_FUNDING_AGENCY_NAME,
        //                         imsSanctionedProjectDetails.IMS_BATCH,
        //                         imsSanctionedProjectDetails.MASTER_BLOCK.MAST_BLOCK_NAME

        //                     }).Union(from imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
        //                              join execRoads in dbContext.EXEC_LSB_MONTHLY_STATUS
        //                              on imsSanctionedProjectDetails.IMS_PR_ROAD_CODE equals execRoads.IMS_PR_ROAD_CODE
        //                              join blockDetails in dbContext.MASTER_BLOCK
        //                              on imsSanctionedProjectDetails.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
        //                              join districtDetails in dbContext.MASTER_DISTRICT
        //                              on imsSanctionedProjectDetails.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
        //                              join stateDetails in dbContext.MASTER_STATE
        //                              on imsSanctionedProjectDetails.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
        //                              join adminDetails in dbContext.ADMIN_DEPARTMENT
        //                              on imsSanctionedProjectDetails.MAST_DPIU_CODE equals adminDetails.ADMIN_ND_CODE
        //                              join fundingAgency in dbContext.MASTER_FUNDING_AGENCY
        //                              on imsSanctionedProjectDetails.IMS_COLLABORATION equals fundingAgency.MAST_FUNDING_AGENCY_CODE into agencies
        //                              from fundingAgency in agencies.DefaultIfEmpty()
        //                              where
        //                              imsSanctionedProjectDetails.MAST_STATE_CODE == stateCode &&
        //                              imsSanctionedProjectDetails.MAST_DISTRICT_CODE == districtCode &&
        //                              imsSanctionedProjectDetails.MAST_DPIU_CODE == adminNDCode &&
        //                              imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
        //                              (imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "P" || imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "L") &&
        //                              execRoads.EXEC_ISCOMPLETED == "C" &&
        //                              (sanctionedYear == 0 ? 1 : imsSanctionedProjectDetails.IMS_YEAR) == (sanctionedYear == 0 ? 1 : sanctionedYear) &&
        //                              (blockCode == 0 ? 1 : imsSanctionedProjectDetails.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
        //                              (packageID == "All Packages" ? "%" : imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper()) == (packageID == "All Packages" ? "%" : packageID.ToUpper()) &&
        //                                  //new filters added by Vikram 
        //                              (batch == 0 ? 1 : imsSanctionedProjectDetails.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
        //                              (collaboration <= 0 ? 1 : imsSanctionedProjectDetails.IMS_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
        //                              (upgradationType == "0" ? "%" : imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT) == (upgradationType == "0" ? "%" : upgradationType) &&
        //                                  //end of change
        //                              imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //new change done by Vikram on 10 Feb 2014
        //                              && imsSanctionedProjectDetails.IMS_DPR_STATUS == "N" //new change done by Vikram
        //                              select new
        //                              {
        //                                  imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
        //                                  imsSanctionedProjectDetails.IMS_ROAD_NAME,
        //                                  imsSanctionedProjectDetails.IMS_ROAD_FROM,
        //                                  imsSanctionedProjectDetails.IMS_ROAD_TO,
        //                                  imsSanctionedProjectDetails.IMS_YEAR,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
        //                                  imsSanctionedProjectDetails.IMS_LOCK_STATUS,
        //                                  imsSanctionedProjectDetails.IMS_SANCTIONED,
        //                                  imsSanctionedProjectDetails.IMS_PACKAGE_ID,
        //                                  imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
        //                                  imsSanctionedProjectDetails.IMS_PAV_LENGTH,
        //                                  fundingAgency.MAST_FUNDING_AGENCY_NAME,
        //                                  imsSanctionedProjectDetails.IMS_BATCH,
        //                                  imsSanctionedProjectDetails.MASTER_BLOCK.MAST_BLOCK_NAME

        //                              });



        //        totalRecords = query == null ? 0 : query.Count();


        //        if (sidx.Trim() != string.Empty)
        //        {
        //            if (sord.ToString() == "asc")
        //            {
        //                switch (sidx)
        //                {
        //                    case "RoadName":
        //                        query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                        break;
        //                    case "SanctionedYear":
        //                        query = query.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                        break;
        //                    case "RoadLength":
        //                        query = query.OrderBy(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                        break;
        //                    case "Collaboration":
        //                        query = query.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                        break;
        //                    case "Package":
        //                        query = query.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                        break;
        //                    default:
        //                        query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                        break;

        //                }


        //            }
        //            else
        //            {
        //                switch (sidx)
        //                {
        //                    case "RoadName":
        //                        query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                        break;
        //                    case "SanctionedYear":
        //                        query = query.OrderByDescending(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                        break;
        //                    case "RoadLength":
        //                        query = query.OrderByDescending(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                        break;
        //                    case "Collaboration":
        //                        query = query.OrderByDescending(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                        break;
        //                    case "Package":
        //                        query = query.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                        break;
        //                    default:
        //                        query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                        break;
        //                }

        //            }
        //        }
        //        else
        //        {
        //            query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //        }

        //        var result = query.Select(imsSanctionedProjectDetails => new
        //        {
        //            imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
        //            imsSanctionedProjectDetails.IMS_ROAD_NAME,
        //            imsSanctionedProjectDetails.IMS_ROAD_FROM,
        //            imsSanctionedProjectDetails.IMS_ROAD_TO,
        //            imsSanctionedProjectDetails.IMS_YEAR,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

        //            imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

        //            imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
        //            imsSanctionedProjectDetails.IMS_LOCK_STATUS,
        //            imsSanctionedProjectDetails.IMS_SANCTIONED,
        //            imsSanctionedProjectDetails.IMS_PACKAGE_ID,
        //            imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
        //            imsSanctionedProjectDetails.IMS_PAV_LENGTH,
        //            imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME,
        //            imsSanctionedProjectDetails.IMS_BATCH,
        //            imsSanctionedProjectDetails.MAST_BLOCK_NAME
        //        }).ToArray();


        //        return result.Select(imsSanctionedProjectDetails => new
        //        {

        //            cell = new[] {                                            
        //                            imsSanctionedProjectDetails.MAST_BLOCK_NAME == null ? "-" : imsSanctionedProjectDetails.MAST_BLOCK_NAME.ToString(),           
        //                            imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString() ,
        //                            imsSanctionedProjectDetails.IMS_BATCH == null ? "-" : ("Batch -" + imsSanctionedProjectDetails.IMS_BATCH).ToString(),
        //                            imsSanctionedProjectDetails.IMS_PACKAGE_ID,    
        //                            imsSanctionedProjectDetails.IMS_ROAD_NAME,
        //                            imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString(),
        //                            imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME==null?"NA":imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME.Trim(),                                                              
        //                            //imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? 
        //                            //(imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT+
        //                            //imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT).ToString() : (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT).ToString(),    

        //                            ///Change made by SAMMED PATIL on 29MAR2016 
        //                            PMGSYSession.Current.PMGSYScheme == 1 ? 
        //                                              ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT)).ToString()
        //                                            : ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT)).ToString(),

        //                            (imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5).ToString(),                                    
        //                            URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),/*"IMSRoadName =" + imsSanctionedProjectDetails.IMS_ROAD_NAME.ToString().Replace('/','_'),*/"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID,"RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")})//"RoadLength="+imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString()
        //            }
        //        }).ToArray();

        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        totalRecords = 0;
        //        return null;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}



        public Array GetCompletedRoadListDAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                if (packageID.Contains("All"))
                {
                    packageID = "All Packages";
                }

                var query = (from imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                             join execRoads in dbContext.EXEC_ROADS_MONTHLY_STATUS
                             on imsSanctionedProjectDetails.IMS_PR_ROAD_CODE equals execRoads.IMS_PR_ROAD_CODE
                             join blockDetails in dbContext.MASTER_BLOCK
                             on imsSanctionedProjectDetails.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                             join districtDetails in dbContext.MASTER_DISTRICT
                             on imsSanctionedProjectDetails.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                             join stateDetails in dbContext.MASTER_STATE
                             on imsSanctionedProjectDetails.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                             join adminDetails in dbContext.ADMIN_DEPARTMENT
                             on imsSanctionedProjectDetails.MAST_DPIU_CODE equals adminDetails.ADMIN_ND_CODE
                             join fundingAgency in dbContext.MASTER_FUNDING_AGENCY
                             on imsSanctionedProjectDetails.IMS_COLLABORATION equals fundingAgency.MAST_FUNDING_AGENCY_CODE into agencies
                             from fundingAgency in agencies.DefaultIfEmpty()
                             where
                             imsSanctionedProjectDetails.MAST_STATE_CODE == stateCode &&
                             imsSanctionedProjectDetails.MAST_DISTRICT_CODE == districtCode &&
                             imsSanctionedProjectDetails.MAST_DPIU_CODE == adminNDCode &&
                             imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
                             (imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "P" || imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "L") &&
                             execRoads.EXEC_ISCOMPLETED == "C" &&

                             imsSanctionedProjectDetails.IMS_YEAR == ((sanctionedYear == 0 || sanctionedYear == -1) ? imsSanctionedProjectDetails.IMS_YEAR : sanctionedYear) &&
                             imsSanctionedProjectDetails.MAST_BLOCK_CODE == ((blockCode == 0 || blockCode == -1) ? imsSanctionedProjectDetails.MAST_BLOCK_CODE : blockCode) &&
                             imsSanctionedProjectDetails.IMS_BATCH == ((batch == 0 || batch == -1) ? imsSanctionedProjectDetails.IMS_BATCH : batch) &&
                             imsSanctionedProjectDetails.IMS_COLLABORATION == ((collaboration == 0 || collaboration == -1) ? imsSanctionedProjectDetails.IMS_COLLABORATION : collaboration) &&
                             imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT == ((upgradationType == "0" || upgradationType == "%") ? imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT : upgradationType) &&
                             imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper() == ((packageID == "All Packages" || packageID == "%" || packageID == "All") ? imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper() : packageID.ToUpper()) &&
                             imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&
                             imsSanctionedProjectDetails.IMS_DPR_STATUS == "N"

                             //(sanctionedYear == 0 ? 1 : imsSanctionedProjectDetails.IMS_YEAR) == (sanctionedYear == 0 ? 1 : sanctionedYear) &&
                             //(blockCode == 0 ? 1 : imsSanctionedProjectDetails.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                             //(packageID == "All Packages" ? "%" : imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper()) == (packageID == "All Packages" ? "%" : packageID.ToUpper()) &&

                             //(batch == 0 ? 1 : imsSanctionedProjectDetails.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
                             //(collaboration <= 0 ? 1 : imsSanctionedProjectDetails.IMS_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                             //(upgradationType == "0" ? "%" : imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT) == (upgradationType == "0" ? "%" : upgradationType) &&




                             select new
                             {
                                 imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                                 imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                 imsSanctionedProjectDetails.IMS_ROAD_FROM,
                                 imsSanctionedProjectDetails.IMS_ROAD_TO,
                                 imsSanctionedProjectDetails.IMS_YEAR,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

                                 imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,
                                 imsSanctionedProjectDetails.IMS_PUCCA_SIDE_DRAINS, // Added on 14 Sept 20202

                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
                                 imsSanctionedProjectDetails.IMS_LOCK_STATUS,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED,
                                 imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                                 imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                                 imsSanctionedProjectDetails.IMS_PAV_LENGTH,
                                 fundingAgency.MAST_FUNDING_AGENCY_NAME,
                                 imsSanctionedProjectDetails.IMS_BATCH,
                                 imsSanctionedProjectDetails.MASTER_BLOCK.MAST_BLOCK_NAME

                             }).Union(from imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                                      join execRoads in dbContext.EXEC_LSB_MONTHLY_STATUS
                                      on imsSanctionedProjectDetails.IMS_PR_ROAD_CODE equals execRoads.IMS_PR_ROAD_CODE
                                      join blockDetails in dbContext.MASTER_BLOCK
                                      on imsSanctionedProjectDetails.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                                      join districtDetails in dbContext.MASTER_DISTRICT
                                      on imsSanctionedProjectDetails.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                                      join stateDetails in dbContext.MASTER_STATE
                                      on imsSanctionedProjectDetails.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                                      join adminDetails in dbContext.ADMIN_DEPARTMENT
                                      on imsSanctionedProjectDetails.MAST_DPIU_CODE equals adminDetails.ADMIN_ND_CODE
                                      join fundingAgency in dbContext.MASTER_FUNDING_AGENCY
                                      on imsSanctionedProjectDetails.IMS_COLLABORATION equals fundingAgency.MAST_FUNDING_AGENCY_CODE into agencies
                                      from fundingAgency in agencies.DefaultIfEmpty()
                                      where
                                      imsSanctionedProjectDetails.MAST_STATE_CODE == stateCode &&
                                      imsSanctionedProjectDetails.MAST_DISTRICT_CODE == districtCode &&
                                      imsSanctionedProjectDetails.MAST_DPIU_CODE == adminNDCode &&
                                      imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
                                      (imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "P" || imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "L") &&
                                      execRoads.EXEC_ISCOMPLETED == "C" &&

                                       imsSanctionedProjectDetails.IMS_YEAR == ((sanctionedYear == 0 || sanctionedYear == -1) ? imsSanctionedProjectDetails.IMS_YEAR : sanctionedYear) &&
                                       imsSanctionedProjectDetails.MAST_BLOCK_CODE == ((blockCode == 0 || blockCode == -1) ? imsSanctionedProjectDetails.MAST_BLOCK_CODE : blockCode) &&
                                       imsSanctionedProjectDetails.IMS_BATCH == ((batch == 0 || batch == -1) ? imsSanctionedProjectDetails.IMS_BATCH : batch) &&
                                       imsSanctionedProjectDetails.IMS_COLLABORATION == ((collaboration == 0 || collaboration == -1) ? imsSanctionedProjectDetails.IMS_COLLABORATION : collaboration) &&
                                       imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT == ((upgradationType == "0" || upgradationType == "%") ? imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT : upgradationType) &&
                                       imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper() == ((packageID == "All Packages" || packageID == "%" || packageID == "All") ? imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper() : packageID.ToUpper()) &&
                                       imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&
                                       imsSanctionedProjectDetails.IMS_DPR_STATUS == "N"




                                      //(sanctionedYear == 0 ? 1 : imsSanctionedProjectDetails.IMS_YEAR) == (sanctionedYear == 0 ? 1 : sanctionedYear) &&
                                      //(blockCode == 0 ? 1 : imsSanctionedProjectDetails.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                                      //(packageID == "All Packages" ? "%" : imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper()) == (packageID == "All Packages" ? "%" : packageID.ToUpper()) &&

                                      //(batch == 0 ? 1 : imsSanctionedProjectDetails.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
                                      //(collaboration <= 0 ? 1 : imsSanctionedProjectDetails.IMS_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                                      //(upgradationType == "0" ? "%" : imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT) == (upgradationType == "0" ? "%" : upgradationType) &&

                                      //imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme 
                                      //&& imsSanctionedProjectDetails.IMS_DPR_STATUS == "N"




                                      select new
                                      {
                                          imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                                          imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                          imsSanctionedProjectDetails.IMS_ROAD_FROM,
                                          imsSanctionedProjectDetails.IMS_ROAD_TO,
                                          imsSanctionedProjectDetails.IMS_YEAR,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

                                          imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

                                          imsSanctionedProjectDetails.IMS_PUCCA_SIDE_DRAINS, // Added on 14 Sept 2020

                                          imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
                                          imsSanctionedProjectDetails.IMS_LOCK_STATUS,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED,
                                          imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                                          imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                                          imsSanctionedProjectDetails.IMS_PAV_LENGTH,
                                          fundingAgency.MAST_FUNDING_AGENCY_NAME,
                                          imsSanctionedProjectDetails.IMS_BATCH,
                                          imsSanctionedProjectDetails.MASTER_BLOCK.MAST_BLOCK_NAME

                                      });



                totalRecords = query == null ? 0 : query.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "SanctionedYear":
                                query = query.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "RoadLength":
                                query = query.OrderBy(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Collaboration":
                                query = query.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Package":
                                query = query.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }


                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "SanctionedYear":
                                query = query.OrderByDescending(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "RoadLength":
                                query = query.OrderByDescending(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Collaboration":
                                query = query.OrderByDescending(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Package":
                                query = query.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(imsSanctionedProjectDetails => new
                {
                    imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                    imsSanctionedProjectDetails.IMS_ROAD_NAME,
                    imsSanctionedProjectDetails.IMS_ROAD_FROM,
                    imsSanctionedProjectDetails.IMS_ROAD_TO,
                    imsSanctionedProjectDetails.IMS_YEAR,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

                    imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,
                    imsSanctionedProjectDetails.IMS_PUCCA_SIDE_DRAINS, // Added on 14 Sept 2020
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
                    imsSanctionedProjectDetails.IMS_LOCK_STATUS,
                    imsSanctionedProjectDetails.IMS_SANCTIONED,
                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                    imsSanctionedProjectDetails.IMS_PAV_LENGTH,
                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME,
                    imsSanctionedProjectDetails.IMS_BATCH,
                    imsSanctionedProjectDetails.MAST_BLOCK_NAME
                }).ToArray();


                return result.Select(imsSanctionedProjectDetails => new
                {

                    cell = new[] {                                            
                                    imsSanctionedProjectDetails.MAST_BLOCK_NAME == null ? "-" : imsSanctionedProjectDetails.MAST_BLOCK_NAME.ToString(),           
                                    imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString() ,
                                    imsSanctionedProjectDetails.IMS_BATCH == null ? "-" : ("Batch -" + imsSanctionedProjectDetails.IMS_BATCH).ToString(),
                                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,    
                                    imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                    imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString(),
                                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME==null?"NA":imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME.Trim(),                                                              
                                    //imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? 
                                    //(imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT+
                                    //imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT).ToString() : (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT).ToString(),    

                                    ///Change made by SAMMED PATIL on 29MAR2016 
                                    PMGSYSession.Current.PMGSYScheme == 1 ? 
                                                      ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT)).ToString()
                                                    : ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT) + (imsSanctionedProjectDetails.IMS_PUCCA_SIDE_DRAINS == null ? 0 : imsSanctionedProjectDetails.IMS_PUCCA_SIDE_DRAINS)).ToString(),

                                    (imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5).ToString(),                                    
                                   
                                    
                                    
                                    URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),/*"IMSRoadName =" + imsSanctionedProjectDetails.IMS_ROAD_NAME.ToString().Replace('/','_'),*/"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToString().Replace("/","--"),"RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")})//"RoadLength="+imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString()
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public Array GetAgreementDetailsListDAL_Proposal(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;
                int? maxContractCode = 0;

                var query = from IMSContract in dbContext.MANE_IMS_CONTRACT
                            join workDetails in dbContext.IMS_PROPOSAL_WORK
                            on IMSContract.IMS_WORK_CODE equals workDetails.IMS_WORK_CODE into works
                            from workDetails in works.DefaultIfEmpty()
                            join contractorDetails in dbContext.MASTER_CONTRACTOR
                            on IMSContract.MAST_CON_ID equals contractorDetails.MAST_CON_ID //into contractors
                            //from contractorDetails in contractors.DefaultIfEmpty()
                            where
                            IMSContract.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                            IMSContract.MANE_AGREEMENT_TYPE == "R"
                            //&& IMSContract.MANE_CONTRACT_STATUS != "I"
                            select new
                            {

                                IMSContract.IMS_PR_ROAD_CODE,
                                IMSContract.MANE_PR_CONTRACT_CODE,
                                MAST_CON_COMPANY_NAME = contractorDetails.MAST_CON_COMPANY_NAME + " - (" + SqlFunctions.StringConvert((decimal)contractorDetails.MAST_CON_ID).Trim() + "),  " + (contractorDetails.MAST_CON_PAN != "" ? " (" + contractorDetails.MAST_CON_PAN + ")" : ""),
                                IMSContract.MANE_AGREEMENT_NUMBER,
                                IMSContract.MANE_AGREEMENT_DATE,
                                IMSContract.MANE_MAINTENANCE_START_DATE,
                                IMSContract.MANE_YEAR1_AMOUNT,
                                IMSContract.MANE_YEAR2_AMOUNT,
                                IMSContract.MANE_YEAR3_AMOUNT,
                                IMSContract.MANE_YEAR4_AMOUNT,
                                IMSContract.MANE_YEAR5_AMOUNT,
                                IMSContract.MANE_CONTRACT_FINALIZED,
                                IMSContract.MANE_LOCK_STATUS,
                                IMSContract.MANE_CONTRACT_STATUS,
                                IMSContract.MANE_CONTRACT_NUMBER,
                                workDetails.IMS_WORK_DESC,
                                IMSContract.IMS_WORK_CODE,
                                IsLatest = true,
                                IMSContract.MANE_CONTRACT_ID

                            };




                totalRecords = query == null ? 0 : query.Count();

                //maxContractCode = query.Max(q => (Int32?)q.MANE_CONTRACT_NUMBER);
                //maxContractCode = maxContractCode == null ? 0 : maxContractCode;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Work":
                                query = query.OrderBy(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "MaintenanceDate":
                                query = query.OrderBy(x => x.MANE_MAINTENANCE_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderBy(x => x.MANE_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }


                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                query = query.OrderByDescending(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Work":
                                query = query.OrderByDescending(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "MaintenanceDate":
                                query = query.OrderByDescending(x => x.MANE_MAINTENANCE_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderByDescending(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderByDescending(x => x.MANE_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(IMSContract => new
                {

                    IMSContract.IMS_PR_ROAD_CODE,
                    IMSContract.MANE_PR_CONTRACT_CODE,
                    IMSContract.MAST_CON_COMPANY_NAME,
                    IMSContract.MANE_AGREEMENT_NUMBER,
                    IMSContract.MANE_AGREEMENT_DATE,
                    IMSContract.MANE_MAINTENANCE_START_DATE,
                    IMSContract.MANE_YEAR1_AMOUNT,
                    IMSContract.MANE_YEAR2_AMOUNT,
                    IMSContract.MANE_YEAR3_AMOUNT,
                    IMSContract.MANE_YEAR4_AMOUNT,
                    IMSContract.MANE_YEAR5_AMOUNT,
                    IMSContract.MANE_CONTRACT_FINALIZED,
                    IMSContract.MANE_LOCK_STATUS,
                    IMSContract.MANE_CONTRACT_STATUS,
                    IMSContract.MANE_CONTRACT_NUMBER,
                    IMSContract.IMS_WORK_DESC,
                    IMSContract.IMS_WORK_CODE,
                    IMSContract.MANE_CONTRACT_ID

                }).ToArray();



                return result.Select(IMSContract => new
                {
                    //id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {


                                    IMSContract.MANE_AGREEMENT_NUMBER.ToString(),
                                    IMSContract.IMS_WORK_DESC==null?"NA":IMSContract.IMS_WORK_DESC.ToString().Trim(),
                                    IMSContract.MAST_CON_COMPANY_NAME==null?"NA":IMSContract.MAST_CON_COMPANY_NAME.ToString().Trim(),
                                    Convert.ToDateTime(IMSContract.MANE_AGREEMENT_DATE).ToString("dd/MM/yyyy"),
                                    Convert.ToDateTime(IMSContract.MANE_MAINTENANCE_START_DATE).ToString("dd/MM/yyyy"),

                                    ((IMSContract.MANE_YEAR1_AMOUNT)+
                                       (IMSContract.MANE_YEAR2_AMOUNT)+
                                       (IMSContract.MANE_YEAR3_AMOUNT)+
                                       (IMSContract.MANE_YEAR4_AMOUNT)+
                                       (IMSContract.MANE_YEAR5_AMOUNT)
                                    ).ToString(),

                                    dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Any(x => x.MANE_PR_CONTRACT_CODE == IMSContract.MANE_PR_CONTRACT_CODE && x.IMS_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE) ? "--" : AgreementStatus[IMSContract.MANE_CONTRACT_STATUS].ToString(),
                                    dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Any(x => x.MANE_PR_CONTRACT_CODE == IMSContract.MANE_PR_CONTRACT_CODE && x.IMS_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE) ? string.Empty : (dbContext.MANE_IMS_PROGRESS.Any(m=>m.IMS_PR_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE && m.MANE_MAINTENANCE_NUMBER == IMSContract.MANE_CONTRACT_NUMBER) ? (((CheckIsLatest(IMSContract.IMS_PR_ROAD_CODE, IMSContract.MANE_PR_CONTRACT_CODE, IMSContract.IMS_WORK_CODE, IMSContract.MANE_CONTRACT_NUMBER, IMSContract.MANE_CONTRACT_STATUS )==true) ?(IMSContract.MANE_CONTRACT_STATUS =="C" || IMSContract.MANE_CONTRACT_FINALIZED=="N") :(IMSContract.MANE_CONTRACT_STATUS =="C" || IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_FINALIZED=="N"))?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()})):string.Empty),
                                    dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Any(x => x.MANE_PR_CONTRACT_CODE == IMSContract.MANE_PR_CONTRACT_CODE && x.IMS_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE) ? string.Empty : (((CheckIsLatest(IMSContract.IMS_PR_ROAD_CODE, IMSContract.MANE_PR_CONTRACT_CODE, IMSContract.IMS_WORK_CODE, IMSContract.MANE_CONTRACT_NUMBER, IMSContract.MANE_CONTRACT_STATUS)==true)?(IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_FINALIZED=="N" ): (IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_STATUS=="C" ) )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString() })),
                                    dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Any(x => x.MANE_PR_CONTRACT_CODE == IMSContract.MANE_PR_CONTRACT_CODE && x.IMS_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE) ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Finalized'></span></td></tr></table></center>" : ((IMSContract.MANE_CONTRACT_FINALIZED=="N" && IMSContract.MANE_LOCK_STATUS=="N" && IMSContract.MANE_CONTRACT_STATUS=="P" ) ?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='FinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID  }) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Finalized'></span></td></tr></table></center>"),
                                    dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Any(x => x.MANE_PR_CONTRACT_CODE == IMSContract.MANE_PR_CONTRACT_CODE && x.IMS_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE) ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='DeFinalized'></span></td></tr></table></center>" : ((dbContext.MANE_IMS_INSPECTION.Any(m=>m.IMS_PR_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE) || dbContext.MANE_IMS_PROGRESS.Any(m=>m.IMS_PR_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE) || dbContext.ACC_BILL_DETAILS.Any(m=>m.IMS_PR_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE && m.IMS_AGREEMENT_CODE == IMSContract.MANE_PR_CONTRACT_CODE) || IMSContract.MANE_CONTRACT_FINALIZED == "N") ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='DeFinalized'></span></td></tr></table></center>" :"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='DeFinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() ,"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString() }) + "\");' ></span></td></tr></table></center>"),
                                    URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()  }),
                                    dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Any(x => x.MANE_PR_CONTRACT_CODE == IMSContract.MANE_PR_CONTRACT_CODE && x.IMS_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE) ? string.Empty : ((IMSContract.MANE_CONTRACT_FINALIZED=="Y"||IMSContract.MANE_LOCK_STATUS=="Y" || IMSContract.MANE_CONTRACT_STATUS=="C" || IMSContract.MANE_CONTRACT_STATUS=="I" )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()  })),
                                    dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Any(x => x.MANE_PR_CONTRACT_CODE == IMSContract.MANE_PR_CONTRACT_CODE && x.IMS_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE) ? string.Empty : ((IMSContract.MANE_CONTRACT_FINALIZED=="Y"||IMSContract.MANE_LOCK_STATUS=="Y"|| IMSContract.MANE_CONTRACT_STATUS=="C" || IMSContract.MANE_CONTRACT_STATUS=="I")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()})),
                                    dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Any(x => x.MANE_PR_CONTRACT_CODE == IMSContract.MANE_PR_CONTRACT_CODE && x.IMS_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE) ? "-" : ((IMSContract.MANE_CONTRACT_FINALIZED=="Y"?"<a href='#'  class='ui-icon ui-icon-plusthick ui-align-center' onClick='AddTechnologyDetails(" + IMSContract.IMS_PR_ROAD_CODE + ","+ IMSContract.MANE_PR_CONTRACT_CODE+"); return false;'>Add Technology Details</a>":"-"))
                                   
                                   // ( (IMSContract.MANE_CONTRACT_NUMBER==maxContractCode && (IMSContract.MANE_CONTRACT_STATUS=="P"||IMSContract.MANE_CONTRACT_STATUS=="I")) ?(IMSContract.MANE_CONTRACT_STATUS =="C" || IMSContract.MANE_CONTRACT_FINALIZED=="N") :(IMSContract.MANE_CONTRACT_STATUS =="C" || IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_FINALIZED=="N"))?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() }),
                                   // ( (IMSContract.MANE_CONTRACT_NUMBER==maxContractCode && (IMSContract.MANE_CONTRACT_STATUS=="P" || IMSContract.MANE_CONTRACT_STATUS=="C"))?(IMSContract.MANE_CONTRACT_STATUS =="I" ): (IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_STATUS=="C" ) )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() }),                                                                         
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

                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }



        private bool CheckIsLatest(int IMSPRRoadCode, int PRContractCode, int? IMSWorkCode, int contractNumber, string contractStatus)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int maxContractNumber = 0;
            MANE_IMS_CONTRACT IMSContract = null;
            try
            {

                if (IMSWorkCode == null)
                {
                    var contractList = dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode);

                    if (contractList.Count() > 1)
                    {
                        IMSContract = contractList.OrderByDescending(IMS => IMS.MANE_PR_CONTRACT_CODE).FirstOrDefault();
                    }
                    else
                    {
                        IMSContract = contractList.FirstOrDefault();
                    }

                    if (IMSContract != null && IMSContract.MANE_PR_CONTRACT_CODE != PRContractCode)
                    {
                        return false;
                    }
                    maxContractNumber = IMSContract.MANE_CONTRACT_NUMBER;
                }
                else
                {
                    //maxContractNumber = dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode && IMS.IMS_WORK_CODE == IMSWorkCode).Max(IMS => IMS.MANE_CONTRACT_NUMBER);

                    var contractList = dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode && IMS.IMS_WORK_CODE == IMSWorkCode);

                    if (contractList.Count() > 1)
                    {
                        IMSContract = contractList.OrderByDescending(IMS => IMS.MANE_PR_CONTRACT_CODE).FirstOrDefault();
                    }
                    else
                    {
                        IMSContract = contractList.FirstOrDefault();
                    }

                    if (IMSContract != null && IMSContract.MANE_PR_CONTRACT_CODE != PRContractCode)
                    {
                        return false;
                    }
                    maxContractNumber = IMSContract.MANE_CONTRACT_NUMBER;
                }

                if (maxContractNumber == contractNumber)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {



            }

        }



        public bool SaveAgreementDetailsDAL_Proposal(MaintenanceAgreementDetails details_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string proposalType = string.Empty;
                MANE_IMS_CONTRACT agreementDetails = null;
                EXEC_ROADS_MONTHLY_STATUS roadMonthlyStatus = null;
                EXEC_LSB_MONTHLY_STATUS lsbMonthlyStatus = null;
                int IMSPRRoadCode = 0;
                int? maxContractorNumber = 1;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                CommonFunctions commonFunction = new CommonFunctions();

                encryptedParameters = details_agreement.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString().Trim());

                // Added on 9 April 2020
                string packageID = dbContext.IMS_SANCTIONED_PROJECTS.Where(z => z.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(z => z.IMS_PACKAGE_ID).FirstOrDefault();
                if (dbContext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Where(m => m.ROAD_CODE == IMSPRRoadCode).Any() || dbContext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Where(m => m.PACKAGE_NO == packageID).Any())
                {

                    if (dbContext.EMARG_ROAD_DETAILS.Where(m => m.ROAD_CODE == IMSPRRoadCode && (m.OMMAS_REPUSHING_STATUS != "1" || m.OMMAS_REPUSHING_STATUS == null)).Any())  //(m.IS_DEACTIVATED == "Y" || m.IS_DEACTIVATED != null)
                    {
                        // Allow to modify
                    }
                    else
                    {
                        message = "This Package or Any Road in this Package is sent to Emarg. Hence Agreement Details can not be modified now.";
                        return false;
                    }
                }


                if (details_agreement.IMS_WORK_CODE == 0)
                {
                    if (dbContext.MANE_IMS_CONTRACT.Any(pc => pc.IMS_PR_ROAD_CODE == IMSPRRoadCode && pc.MANE_CONTRACT_STATUS == "P" && pc.MANE_AGREEMENT_TYPE == "R"))
                    {
                        message = "Agreement details with 'InProgress' status is already exist against selected road.";
                        return false;
                    }
                    //  else
                    //  {
                    /* working code but commented for removing restriction from user to continue or new contractor*/

                    //var agreementMasterDetails = from tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                    //                             join tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                    //                             on tendAgreementDetails.TEND_AGREEMENT_CODE equals tendAgreementMaster.TEND_AGREEMENT_CODE
                    //                             where
                    //                             tendAgreementDetails.TEND_AGREEMENT_CODE == (from td in dbContext.TEND_AGREEMENT_DETAIL where td.IMS_PR_ROAD_CODE == IMSPRRoadCode select td.TEND_AGREEMENT_CODE).FirstOrDefault()
                    //                             && (tendAgreementDetails.TEND_AGREEMENT_STATUS == "C" || tendAgreementDetails.TEND_AGREEMENT_STATUS == "M")
                    //                             select new
                    //                             {
                    //                                 tendAgreementMaster.TEND_AGREEMENT_CODE,
                    //                                 tendAgreementMaster.MAST_CON_ID,
                    //                                 tendAgreementDetails.IMS_WORK_CODE,
                    //                                 tendAgreementDetails.IMS_PR_ROAD_CODE,
                    //                                 tendAgreementDetails.TEND_AGREEMENT_STATUS
                    //                             };


                    // if (agreementMasterDetails.Any(ad => ad.TEND_AGREEMENT_STATUS == "M"))
                    //   {
                    /* working code but commented for removing restriction from user to continue or new contractor*/
                    //if (agreementMasterDetails.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(ad => (Int32)ad.MAST_CON_ID).FirstOrDefault() == details_agreement.MAST_CON_ID)
                    //{
                    //    message = "You can not make maintenance agreement with same contractor.";// because work from agreement    Number is already exist.";
                    //    return false;
                    //}

                    //  }
                    // else
                    //   {

                    //var imsContracts = from ad in agreementMasterDetails
                    //                   join imsContract in dbContext.MANE_IMS_CONTRACT
                    //                   on ad.IMS_WORK_CODE equals imsContract.IMS_WORK_CODE into works
                    //                   from imsContract in works.DefaultIfEmpty()
                    //                   where imsContract.MANE_CONTRACT_STATUS == "P"
                    //                   select new
                    //                   {
                    //                       imsContract.IMS_PR_ROAD_CODE,
                    //                       imsContract.IMS_WORK_CODE,
                    //                       imsContract.MAST_CON_ID
                    //                   };


                    /* working code but commented for removing restriction from user to continue or new contractor*/

                    //var agreementMasterDetailsWithoutWork = agreementMasterDetails.Where(am => am.IMS_WORK_CODE == null);

                    //if ((from aMDW in agreementMasterDetailsWithoutWork join IMSC in dbContext.MANE_IMS_CONTRACT on aMDW.IMS_PR_ROAD_CODE equals IMSC.IMS_PR_ROAD_CODE where IMSC.MANE_CONTRACT_STATUS=="P" select IMSC).Count() > 0)
                    //{
                    //    if (agreementMasterDetails.Where(am => am.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(am => (Int32)am.MAST_CON_ID).FirstOrDefault() != details_agreement.MAST_CON_ID)
                    //    {
                    //        message = "You can not make maintenance agreement with different contractor.";
                    //        return false;
                    //    }
                    //}

                    //var agreementMasterDetailsWithWork = agreementMasterDetails.Where(am => am.IMS_WORK_CODE != null);

                    //if ((from aMDW in agreementMasterDetailsWithWork join IMSC in dbContext.MANE_IMS_CONTRACT on aMDW.IMS_PR_ROAD_CODE equals IMSC.IMS_PR_ROAD_CODE where aMDW.IMS_WORK_CODE == IMSC.IMS_WORK_CODE &&  IMSC.MANE_CONTRACT_STATUS=="P" select IMSC).Count() > 0)
                    //{
                    //    if (agreementMasterDetails.Where(am => am.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(am => (Int32)am.MAST_CON_ID).FirstOrDefault() != details_agreement.MAST_CON_ID)
                    //    {
                    //        message = "You can not make maintenance agreement with different contractor.";
                    //        return false;
                    //    }
                    //}

                    /*end  working code but commented for removing restriction from user to continue or new contractor*/



                    //if (imsContracts.Count() > 0 && imsContracts.FirstOrDefault().MAST_CON_ID != details_agreement.MAST_CON_ID)
                    //{
                    //    message = "You can not make maintenance agreement with different contractor.";
                    //    return false;
                    //}

                    //   }


                    // }
                }
                else if (details_agreement.IMS_WORK_CODE > 0)
                {
                    if (dbContext.MANE_IMS_CONTRACT.Any(pc => pc.IMS_PR_ROAD_CODE == IMSPRRoadCode && pc.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE && pc.MANE_CONTRACT_STATUS == "P"))
                    {
                        message = "Agreement details with 'InProgress' status is already exist against selected work.";
                        return false;
                    }
                    /*else
                    {
                        var agreementMasterDetails = from tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                                                     join tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                                     on tendAgreementDetails.TEND_AGREEMENT_CODE equals tendAgreementMaster.TEND_AGREEMENT_CODE
                                                     where
                                                     tendAgreementDetails.TEND_AGREEMENT_CODE == (from td in dbContext.TEND_AGREEMENT_DETAIL where td.IMS_PR_ROAD_CODE == IMSPRRoadCode && td.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE select td.TEND_AGREEMENT_CODE).FirstOrDefault()
                                                     //tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                                     //tendAgreementDetails.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE &&
                                                      && (tendAgreementDetails.TEND_AGREEMENT_STATUS == "C" || tendAgreementDetails.TEND_AGREEMENT_STATUS == "M")
                                                     select new
                                                     {
                                                         tendAgreementMaster.TEND_AGREEMENT_CODE,
                                                         tendAgreementMaster.MAST_CON_ID,
                                                         tendAgreementDetails.IMS_WORK_CODE,
                                                         tendAgreementDetails.IMS_PR_ROAD_CODE,
                                                         tendAgreementDetails.TEND_AGREEMENT_STATUS
                                                     };


                        if (agreementMasterDetails.Any(ad => ad.TEND_AGREEMENT_STATUS == "M"))
                        {
                            if (agreementMasterDetails.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE).Select(ad => (Int32)ad.MAST_CON_ID).FirstOrDefault() == details_agreement.MAST_CON_ID)
                            {
                                message = "You can not make maintenance agreement with same contractor.";// because work from agreement    Number is already exist.";
                                return false;
                            }

                        }
                        else
                        {
                            //var imsContracts = from ad in agreementMasterDetails
                            //                   join imsContract in dbContext.MANE_IMS_CONTRACT
                            //                   on new { ad.IMS_PR_ROAD_CODE, ad.IMS_WORK_CODE } equals new { imsContract.IMS_PR_ROAD_CODE, imsContract.IMS_WORK_CODE } into masterDetails
                            //                   from imsContract in masterDetails.DefaultIfEmpty()
                            //                   select new
                            //                   {
                            //                       imsContract.IMS_PR_ROAD_CODE,
                            //                       imsContract.IMS_WORK_CODE,
                            //                       imsContract.MAST_CON_ID
                            //                   };

                            var agreementMasterDetailsWithoutWork = agreementMasterDetails.Where(am => am.IMS_WORK_CODE == null);

                            if ((from aMDW in agreementMasterDetailsWithoutWork join IMSC in dbContext.MANE_IMS_CONTRACT on aMDW.IMS_PR_ROAD_CODE equals IMSC.IMS_PR_ROAD_CODE where IMSC.MANE_CONTRACT_STATUS == "P" select IMSC).Count() > 0)
                            {
                                if (agreementMasterDetails.Where(am => am.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(am => (Int32)am.MAST_CON_ID).FirstOrDefault() != details_agreement.MAST_CON_ID)
                                {
                                    message = "You can not make maintenance agreement with different contractor.";
                                    return false;
                                }
                            }

                            var agreementMasterDetailsWithWork = agreementMasterDetails.Where(am => am.IMS_WORK_CODE != null);

                            if ((from aMDW in agreementMasterDetailsWithWork join IMSC in dbContext.MANE_IMS_CONTRACT on aMDW.IMS_PR_ROAD_CODE equals IMSC.IMS_PR_ROAD_CODE where aMDW.IMS_WORK_CODE==IMSC.IMS_WORK_CODE  && IMSC.MANE_CONTRACT_STATUS=="P" select IMSC).Count() > 0)
                            {
                                if (agreementMasterDetails.Where(am => am.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(am => (Int32)am.MAST_CON_ID).FirstOrDefault() != details_agreement.MAST_CON_ID)
                                {
                                    message = "You can not make maintenance agreement with different contractor.";
                                    return false;
                                }
                            }

 
                        }*/


                    //}//end else 
                }//end   else if (details_agreement.IMS_WORK_CODE > 0)

                if (details_agreement.IMS_WORK_CODE == 0)
                {
                    if (dbContext.MANE_IMS_CONTRACT.Any(pc => pc.IMS_PR_ROAD_CODE == IMSPRRoadCode && pc.MANE_AGREEMENT_NUMBER.ToUpper() == details_agreement.MANE_AGREEMENT_NUMBER.ToUpper()))
                    {
                        message = "Agreement Number is already exist for selected road.";
                        return false;
                    }
                }

                if (!details_agreement.IsNewContractor)
                {
                    if (dbContext.TEND_AGREEMENT_MASTER.Any(am => am.TEND_AGREEMENT_NUMBER.ToUpper() == details_agreement.MANE_AGREEMENT_NUMBER.ToUpper() && am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode))
                    {
                        message = "Agreement Number is already exist for regular agreement.";
                        return false;
                    }
                }

                // MANE_IMS_CONTRACT IMSContract = dbContext.MANE_IMS_CONTRACT.Where(pc => pc.MANE_AGREEMENT_NUMBER.ToUpper() == details_agreement.MANE_AGREEMENT_NUMBER.ToUpper()).FirstOrDefault(); //pc.IMS_PR_ROAD_CODE == IMSPRRoadCode &&


                var IMSContractList = (from IMSContracts in dbContext.MANE_IMS_CONTRACT
                                       join IMSSanctioned in dbContext.IMS_SANCTIONED_PROJECTS
                                       on IMSContracts.IMS_PR_ROAD_CODE equals IMSSanctioned.IMS_PR_ROAD_CODE
                                       where
                                       IMSSanctioned.MAST_STATE_CODE == stateCode &&
                                       IMSSanctioned.MAST_DISTRICT_CODE == districtCode &&
                                       IMSContracts.MANE_AGREEMENT_NUMBER.ToUpper() == details_agreement.MANE_AGREEMENT_NUMBER.ToUpper()
                                       && IMSSanctioned.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                                       && IMSContracts.MANE_AGREEMENT_TYPE == "R"
                                       select new
                                       {

                                           IMSContracts.MANE_AGREEMENT_NUMBER,
                                           IMSContracts.MAST_CON_ID,
                                           IMSContracts.IMS_PR_ROAD_CODE,
                                           IMSSanctioned.MAST_STATE_CODE,
                                           IMSSanctioned.MAST_DISTRICT_CODE

                                       }).FirstOrDefault();

                if (IMSContractList != null)
                {

                    if (IMSContractList.MAST_CON_ID != details_agreement.MAST_CON_ID)
                    {

                        message = "Contractor should be same for same agreement number/Agreement Number already exists.";
                        return false;
                    }

                    //IMS_SANCTIONED_PROJECTS sanctionedProjects = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE).FirstOrDefault();

                    //if (PMGSY.Extensions.PMGSYSession.Current.DistrictCode == sanctionedProjects.MAST_DISTRICT_CODE)
                    //{
                    //    if (IMSContract.MAST_CON_ID != details_agreement.MAST_CON_ID)
                    //    {
                    //        //message = "Agreement Number is already exist.";
                    //        message = "Contractor should be same for same agreement number.";
                    //        return false;
                    //    }
                    //}
                    //else
                    //{

                    //    message = "Agreement Number is already exist.";
                    //    return false;
                    //}
                }
                ///Changed by SAMMED PATIL for LSB and Road proposals  28/06/2016
                ///

                proposalType = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(x => x.IMS_PROPOSAL_TYPE).FirstOrDefault();

                if (proposalType == "P")
                {

                    //if (commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_START_DATE) < commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE))
                    //{
                    //    message = "Maintenance start date must be greater than or equal to construction completion date.";
                    //    return false;
                    //}

                    if (details_agreement.MANE_HANDOVER_DATE != null)
                    {
                        if (commonFunction.GetStringToDateTime(details_agreement.MANE_HANDOVER_DATE) < commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_END_DATE))
                        {
                            message = "Maintenance handover date must be greater than or equal to maintenance end date.";
                            return false;
                        }
                    }

                    roadMonthlyStatus = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(rms => rms.IMS_PR_ROAD_CODE == IMSPRRoadCode && rms.EXEC_ISCOMPLETED == "C").FirstOrDefault();
                    var ExecutionDay = roadMonthlyStatus.EXEC_COMPLETION_DATE.ToString().Split(' ')[0];
                    if (roadMonthlyStatus != null)
                    {
                        if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE) < commonFunction.GetStringToDateTime(ExecutionDay) || commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE) > commonFunction.GetStringToDateTime(ExecutionDay))
                        {
                            message = "Construction completion date should be equal to physical progress completion date.";
                            return false;
                        }
                        #region

                        //if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year > roadMonthlyStatus.EXEC_PROG_YEAR || commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year < roadMonthlyStatus.EXEC_PROG_YEAR)
                        //{
                        //    message = "Construction completion date (Year) should be equal to physical progress completion date.";
                        //    return false;
                        //}
                        //else if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year == roadMonthlyStatus.EXEC_PROG_YEAR && (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Month > roadMonthlyStatus.EXEC_PROG_MONTH || commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Month < roadMonthlyStatus.EXEC_PROG_MONTH))
                        //{
                        //    message = "Construction completion date (Month) should be equal to physical progress completion date.";
                        //    return false;
                        //} // below block added for comparing day in date for physical progress.
                        //else if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year == roadMonthlyStatus.EXEC_PROG_YEAR && commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Month == roadMonthlyStatus.EXEC_PROG_MONTH && (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Day < ExecDay ))
                        //{
                        //    message = "Construction completion date (Day) should be equal to physical progress completion date.";
                        //    return false;
                        //}
                        #endregion
                    }
                }
                else
                {

                    //if (commonFunction.GetStringToDateTime(details_agreement.MANE_HANDOVER_DATE) < commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_END_DATE))
                    //{
                    //    message = "Maintenance handover date must be greater than or equal to maintenance end date.";
                    //    return false;
                    //}

                    if (details_agreement.MANE_HANDOVER_DATE != null)
                    {
                        if (commonFunction.GetStringToDateTime(details_agreement.MANE_HANDOVER_DATE) < commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_END_DATE))
                        {
                            message = "Maintenance handover date must be greater than or equal to maintenance end date.";
                            return false;
                        }
                    }

                    //if (commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_START_DATE) < commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE))
                    //{
                    //    message = "Maintenance start date must be greater than or equal to construction completion date.";
                    //    return false;
                    //}

                    lsbMonthlyStatus = dbContext.EXEC_LSB_MONTHLY_STATUS.Where(rms => rms.IMS_PR_ROAD_CODE == IMSPRRoadCode && rms.EXEC_ISCOMPLETED == "C").FirstOrDefault();
                    var ExecutionDay = lsbMonthlyStatus.EXEC_COMPLETION_DATE.ToString().Split(' ')[0];

                    if (lsbMonthlyStatus != null)
                    {
                        if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE) < commonFunction.GetStringToDateTime(ExecutionDay) || commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE) > commonFunction.GetStringToDateTime(ExecutionDay))
                        {
                            message = "Construction completion date should be equal to physical progress completion date.";
                            return false;
                        }
                        #region
                        //if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year > lsbMonthlyStatus.EXEC_PROG_YEAR || commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year < lsbMonthlyStatus.EXEC_PROG_YEAR)
                        //{
                        //    message = "Construction completion date should be equal to physical progress completion date.";
                        //    return false;
                        //}
                        //else if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year == lsbMonthlyStatus.EXEC_PROG_YEAR && (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Month > lsbMonthlyStatus.EXEC_PROG_MONTH || commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Month < lsbMonthlyStatus.EXEC_PROG_MONTH))
                        //{
                        //    message = "Construction completion date should be equal to physical progress completion date.";
                        //    return false;
                        //} // below block added for comparing day in date for physical progress.
                        //else if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year == lsbMonthlyStatus.EXEC_PROG_YEAR && commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Month == lsbMonthlyStatus.EXEC_PROG_MONTH && (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Day < ExecDay ))
                        //{
                        //    message = "Construction completion date should be equal to physical progress completion date.";
                        //    return false;
                        //}
                        #endregion
                    }
                }
                using (var scope = new TransactionScope())
                {
                    agreementDetails = new MANE_IMS_CONTRACT();
                    agreementDetails.IMS_PR_ROAD_CODE = IMSPRRoadCode;
                    agreementDetails.MANE_PR_CONTRACT_CODE = (Int32)GetMaxCode(MaintenanceAgreementModules.IMSContract, IMSPRRoadCode);
                    agreementDetails.MANE_CONTRACT_ID = dbContext.MANE_IMS_CONTRACT.Max(cp => (Int32?)cp.MANE_CONTRACT_ID) == null ? 1 : (Int32)dbContext.MANE_IMS_CONTRACT.Max(cp => (Int32?)cp.MANE_CONTRACT_ID) + 1;
                    agreementDetails.MAST_CON_ID = details_agreement.MAST_CON_ID;
                    agreementDetails.MANE_AGREEMENT_NUMBER = details_agreement.MANE_AGREEMENT_NUMBER;
                    agreementDetails.MANE_AGREEMENT_DATE = commonFunction.GetStringToDateTime(details_agreement.MANE_AGREEMENT_DATE);
                    agreementDetails.MANE_CONSTR_COMP_DATE = commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE);
                    agreementDetails.MANE_MAINTENANCE_START_DATE = commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_START_DATE);

                    //added by ahishek kamble 21-nov-2013
                    if (details_agreement.MANE_MAINTENANCE_END_DATE != null)
                    {
                        agreementDetails.MANE_MAINTENANCE_END_DATE = commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_END_DATE);
                    }
                    else
                    {
                        agreementDetails.MANE_MAINTENANCE_END_DATE = null;
                    }

                    agreementDetails.MANE_HANDOVER_DATE = details_agreement.MANE_HANDOVER_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.MANE_HANDOVER_DATE);
                    agreementDetails.MANE_YEAR1_AMOUNT = (Decimal)details_agreement.MANE_YEAR1_AMOUNT;
                    agreementDetails.MANE_YEAR2_AMOUNT = (Decimal)details_agreement.MANE_YEAR2_AMOUNT;
                    agreementDetails.MANE_YEAR3_AMOUNT = (Decimal)details_agreement.MANE_YEAR3_AMOUNT;
                    agreementDetails.MANE_YEAR4_AMOUNT = (Decimal)details_agreement.MANE_YEAR4_AMOUNT;
                    agreementDetails.MANE_YEAR5_AMOUNT = (Decimal)details_agreement.MANE_YEAR5_AMOUNT;
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementDetails.MANE_YEAR6_AMOUNT = (Decimal)(details_agreement.MANE_YEAR6_AMOUNT == null ? 0 : details_agreement.MANE_YEAR6_AMOUNT);
                    }
                    agreementDetails.MANE_HANDOVER_TO = details_agreement.MANE_HANDOVER_TO == null ? null : details_agreement.MANE_HANDOVER_TO.Trim();
                    agreementDetails.MANE_CONTRACT_STATUS = "P";
                    agreementDetails.MANE_CONTRACT_FINALIZED = "N";
                    agreementDetails.MANE_LOCK_STATUS = "N";
                    agreementDetails.MANE_AGREEMENT_TYPE = "R";
                    if (details_agreement.IMS_WORK_CODE > 0)
                    {
                        agreementDetails.IMS_WORK_CODE = details_agreement.IMS_WORK_CODE;
                        agreementDetails.MANE_PART_AGREEMENT = "Y";


                        if (dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE).Any())
                        {
                            agreementDetails.MANE_FIRST_AGREEMENT = "N";
                        }
                        else
                        {
                            agreementDetails.MANE_FIRST_AGREEMENT = "Y";
                        }

                    }
                    else
                    {
                        agreementDetails.MANE_PART_AGREEMENT = "N";

                        if (dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Any())
                        {
                            agreementDetails.MANE_FIRST_AGREEMENT = "N";
                        }
                        else
                        {
                            agreementDetails.MANE_FIRST_AGREEMENT = "Y";
                        }

                    }


                    //for add contractor number count 
                    if (dbContext.MANE_IMS_CONTRACT.Any(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                    {
                        MANE_IMS_CONTRACT IMSContracts = null;

                        if (details_agreement.IMS_WORK_CODE > 0)
                        {
                            IMSContracts = dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode && IMS.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE).OrderByDescending(IMS => IMS.MANE_PR_CONTRACT_CODE).FirstOrDefault();
                        }
                        else if (details_agreement.IMS_WORK_CODE == 0)
                        {
                            IMSContracts = dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(IMS => IMS.MANE_PR_CONTRACT_CODE).FirstOrDefault();
                        }

                        if (IMSContracts != null)
                        {
                            if (IMSContracts.MANE_CONTRACT_STATUS == "I")
                            {
                                maxContractorNumber = IMSContracts.MANE_CONTRACT_NUMBER;
                            }
                            else
                            {
                                maxContractorNumber = IMSContracts.MANE_CONTRACT_NUMBER + 1;
                            }
                        }

                    }

                    agreementDetails.MANE_CONTRACT_NUMBER = (int)maxContractorNumber;
                    agreementDetails.USERID = PMGSYSession.Current.UserId;
                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.MANE_IMS_CONTRACT.Add(agreementDetails);

                    //update tender status to M when contractor not continue with same contractor
                    if (!details_agreement.IsNewContractor)
                    {
                        TEND_AGREEMENT_DETAIL tendAgreementDetails = null;
                        if (details_agreement.IMS_WORK_CODE > 0)
                        {
                            //tendAgreementDetails = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE && ad.TEND_AGREEMENT_STATUS == "C").FirstOrDefault();

                            tendAgreementDetails = (from agreementDetail in dbContext.TEND_AGREEMENT_DETAIL
                                                    join agreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                                    on agreementDetail.TEND_AGREEMENT_CODE equals agreementMaster.TEND_AGREEMENT_CODE
                                                    where
                                                    agreementDetail.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                                    agreementDetail.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE &&
                                                    agreementDetail.TEND_AGREEMENT_STATUS == "C" &&
                                                    agreementMaster.TEND_AGREEMENT_TYPE == "C"
                                                    select agreementDetail
                                                       ).FirstOrDefault();
                        }
                        else if (details_agreement.IMS_WORK_CODE == 0)
                        {
                            // tendAgreementDetails = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_AGREEMENT_STATUS == "C").FirstOrDefault();

                            tendAgreementDetails = (from agreementDetail in dbContext.TEND_AGREEMENT_DETAIL
                                                    join agreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                                    on agreementDetail.TEND_AGREEMENT_CODE equals agreementMaster.TEND_AGREEMENT_CODE
                                                    where
                                                    agreementDetail.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                                    agreementDetail.TEND_AGREEMENT_STATUS == "C" &&
                                                    agreementMaster.TEND_AGREEMENT_TYPE == "C"
                                                    select agreementDetail
                                                      ).FirstOrDefault();
                        }

                        if (tendAgreementDetails != null)
                        {
                            tendAgreementDetails.TEND_AGREEMENT_STATUS = "M";
                            tendAgreementDetails.USERID = PMGSYSession.Current.UserId;
                            tendAgreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(tendAgreementDetails).State = System.Data.Entity.EntityState.Modified;
                        }
                    }



                    dbContext.SaveChanges();

                    //if (dbContext.MANE_IMS_CONTRACT.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode).Count() == 1)
                    //{
                    //    dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).ToList().ForEach(IMS => IMS.IMS_ISCOMPLETED = "X");//&& IMS.IMS_ISCOMPLETED == "W"
                    //    dbContext.SaveChanges();
                    //}

                    scope.Complete();
                    return true;
                }

            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool GetExistingAgreementDetailsDAL(int IMSPRRoadCode, int IMSWorkCode, ref int contractorID, ref string agreementNumber, ref  string agreementDate, ref decimal? year1, ref decimal? year2, ref decimal? year3, ref decimal? year4, ref decimal? year5, ref string message)
        {
            StringBuilder existingAgreementDetails = new StringBuilder();
            //List<string> lstContractor = new List<string>();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var agreementMasterDetails = from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                             join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                                             on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                                             join contractorDetails in dbContext.MASTER_CONTRACTOR
                                             on tendAgreementMaster.MAST_CON_ID equals contractorDetails.MAST_CON_ID into contractors
                                             from contractorDetails in contractors.DefaultIfEmpty()
                                             where
                                             tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                             (IMSWorkCode == 0 ? 1 : tendAgreementDetails.IMS_WORK_CODE) == (IMSWorkCode == 0 ? 1 : IMSWorkCode) &&
                                             (tendAgreementDetails.TEND_AGREEMENT_STATUS == "C" || tendAgreementDetails.TEND_AGREEMENT_STATUS == "P" || tendAgreementDetails.TEND_AGREEMENT_STATUS == "W" || tendAgreementDetails.TEND_AGREEMENT_STATUS == "M") &&
                                             (tendAgreementMaster.TEND_AGREEMENT_TYPE == "C") &&
                                                 ///Changed by SAMMED A. PATIL on 11JAN2017 for jhjamtara issue unable to add maintenance agreement with same contractor
                                             (tendAgreementDetails.IMS_SANCTIONED_PROJECTS.IMS_PROPOSAL_TYPE == "P"
                                             ? (dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode && m.EXEC_ISCOMPLETED == "C"))
                                             : (tendAgreementDetails.IMS_SANCTIONED_PROJECTS.IMS_PROPOSAL_TYPE == "L"
                                               ? (dbContext.EXEC_LSB_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode && m.EXEC_ISCOMPLETED == "C"))
                                               : ("%" == "%")
                                               )
                                             )

                                             select new
                                             {
                                                 tendAgreementMaster.TEND_AGREEMENT_CODE,
                                                 tendAgreementMaster.MAST_CON_ID,
                                                 contractorDetails.MAST_CON_COMPANY_NAME,
                                                 tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                                                 tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                                                 tendAgreementDetails.TEND_AMOUNT_YEAR1,
                                                 tendAgreementDetails.TEND_AMOUNT_YEAR2,
                                                 tendAgreementDetails.TEND_AMOUNT_YEAR3,
                                                 tendAgreementDetails.TEND_AMOUNT_YEAR4,
                                                 tendAgreementDetails.TEND_AMOUNT_YEAR5,
                                                 tendAgreementDetails.TEND_AGREEMENT_STATUS

                                             };



                if (agreementMasterDetails.Count() > 0)
                {
                    //int i = 0;
                    // existingAgreementDetails.Append("<table width='100%' style='border:1px solid #808080;' class='rowstyle ui-corner-all' cellspacing=1 ><tr><th>Sr.No </th><th>Contractor Name </th><th> Agrement Number </th> <th> Agreement Date </th></tr>");

                    //not in use due to requirement changes

                    /*existingAgreementDetails.Append("<table width='100%'  class='rowstyle ui-corner-all' cellspacing=0 border='1px solid #808080' ><tr><th>Sr.No </th><th>Contractor Name </th><th> Agrement Number </th> <th> Agreement Date </th></tr>");
                    foreach (var item in agreementMasterDetails)
                    {
                        existingAgreementDetails.Append("<tr><td style='text-align:center'>" + (++i).ToString() + " </td><td>" + item.MAST_CON_COMPANY_NAME.ToString() + " </td><td>" + item.TEND_AGREEMENT_NUMBER.ToString() + " </td><td>" + Convert.ToDateTime(item.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy") + " </td></tr>");

                        year1 = year1 + (item.TEND_AMOUNT_YEAR1 == null ? 0 : item.TEND_AMOUNT_YEAR1);
                        year2 = year2 + (item.TEND_AMOUNT_YEAR2 == null ? 0 : item.TEND_AMOUNT_YEAR2);
                        year3 = year3 + (item.TEND_AMOUNT_YEAR3 == null ? 0 : item.TEND_AMOUNT_YEAR3);
                        year4 = year4 + (item.TEND_AMOUNT_YEAR4 == null ? 0 : item.TEND_AMOUNT_YEAR4);
                        year5 = year5 + (item.TEND_AMOUNT_YEAR5 == null ? 0 : item.TEND_AMOUNT_YEAR5);
                        contractorList.Add(item.MAST_CON_ID.ToString());     
                    }
                    existingAgreementDetails.Append("</table>");
                    agreementDetails = existingAgreementDetails.ToString();*/

                    //int? agreementCode = agreementMasterDetails.OrderByDescending(x => x.TEND_AGREEMENT_CODE).Select(am => (Int32?)am.TEND_AGREEMENT_CODE).FirstOrDefault();
                    ///Changed by SAMMED A. PATIL on 09JUNE2017 to order agreement by descending order of Agreement Date
                    int? agreementCode = agreementMasterDetails.OrderByDescending(x => x.TEND_DATE_OF_AGREEMENT).Select(am => (Int32?)am.TEND_AGREEMENT_CODE).FirstOrDefault();

                    //agreementCode = agreementCode == null ? 0 : agreementCode;

                    if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_STATUS == "W" && ad.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                    {
                        message = "Due to agreement has been terminated,you can not make maintenance agreement with same contractor.";
                        return false;
                    }

                    ///Changed by SAMMED A. PATIL on 09JUNE2017 to order agreement by descending order of Agreement Date
                    agreementMasterDetails = agreementMasterDetails.Where(am => am.TEND_AGREEMENT_STATUS == "C" || am.TEND_AGREEMENT_STATUS == "M" || am.TEND_AGREEMENT_STATUS == "P").OrderBy(x => x.TEND_DATE_OF_AGREEMENT);

                    if (agreementMasterDetails.Count() > 0)
                    {
                        foreach (var item in agreementMasterDetails)
                        {
                            year1 = year1 + (item.TEND_AMOUNT_YEAR1 == null ? 0 : item.TEND_AMOUNT_YEAR1);
                            year2 = year2 + (item.TEND_AMOUNT_YEAR2 == null ? 0 : item.TEND_AMOUNT_YEAR2);
                            year3 = year3 + (item.TEND_AMOUNT_YEAR3 == null ? 0 : item.TEND_AMOUNT_YEAR3);
                            year4 = year4 + (item.TEND_AMOUNT_YEAR4 == null ? 0 : item.TEND_AMOUNT_YEAR4);
                            year5 = year5 + (item.TEND_AMOUNT_YEAR5 == null ? 0 : item.TEND_AMOUNT_YEAR5);

                            contractorID = (Int32)item.MAST_CON_ID;
                            agreementNumber = item.TEND_AGREEMENT_NUMBER;
                            agreementDate = Convert.ToDateTime(item.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy");
                        }

                        return true;
                    }

                }

                //else
                //{
                //   // existingAgreementDetails.Append("<table width='100%' style='border:1px solid #808080;' class='rowstyle ui-corner-all' ><tr><th>Contractor Name </th><th> Agrement Number </th> <th> Agreement Date </th></tr>");
                //    existingAgreementDetails.Append("<table width='100%' style='border:1px solid #808080;' class='rowstyle ui-corner-all' >");
                //    existingAgreementDetails.Append("<tr><td colspan=3 style='text-align:left'>Agreement does not exist against selected road. </td><td>");
                //    existingAgreementDetails.Append("</table>");
                //    agreementDetails = existingAgreementDetails.ToString();
                //}
                message = "Agreement details does not exist.";
                return false;


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }




        public List<MASTER_CONTRACTOR> GetExistingContractor(int IMSPRRoadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<MASTER_CONTRACTOR> lstContractor = new List<MASTER_CONTRACTOR>();
            try
            {

                var contractorsList = from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                      join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                                      on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                                      join contractorDetails in dbContext.MASTER_CONTRACTOR
                                      on tendAgreementMaster.MAST_CON_ID equals contractorDetails.MAST_CON_ID into contractors
                                      from contractorDetails in contractors.DefaultIfEmpty()
                                      where
                                      tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                      tendAgreementDetails.TEND_AGREEMENT_STATUS == "C" &&
                                      tendAgreementMaster.TEND_AGREEMENT_TYPE == "C"
                                      select new
                                      {
                                          tendAgreementMaster.MAST_CON_ID,
                                          contractorDetails.MAST_CON_COMPANY_NAME,

                                      };

                contractorsList = contractorsList.Where(c => c.MAST_CON_COMPANY_NAME != null).Distinct();

                foreach (var contractor in contractorsList)
                {
                    lstContractor.Add(new MASTER_CONTRACTOR() { MAST_CON_ID = (Int32)contractor.MAST_CON_ID, MAST_CON_COMPANY_NAME = contractor.MAST_CON_COMPANY_NAME });
                }
                lstContractor.Insert(0, new MASTER_CONTRACTOR() { MAST_CON_ID = 0, MAST_CON_COMPANY_NAME = "Select Contractor" });

                return lstContractor;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public MaintenanceAgreementDetails GetMaintenanceAgreementDetailsDAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId, bool isView = false)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CommonFunctions commonFunction = new CommonFunctions();
                MANE_IMS_CONTRACT IMSContract = null;

                if (!isView)
                {
                    IMSContract = dbContext.MANE_IMS_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.MANE_PR_CONTRACT_CODE == PRContractCode && c.MANE_LOCK_STATUS == "N" && c.MANE_CONTRACT_STATUS == "P").FirstOrDefault();
                }
                else
                {
                    IMSContract = dbContext.MANE_IMS_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.MANE_PR_CONTRACT_CODE == PRContractCode).FirstOrDefault();
                }

                MaintenanceAgreementDetails agreementDetails = null;


                if (IMSContract != null)
                {


                    agreementDetails = new MaintenanceAgreementDetails()
                    {

                        EncryptedIMSPRRoadCode = URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(), "PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() }),
                        EncryptedPRContractCode = URLEncrypt.EncryptParameters1(new string[] { "PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() }),
                        MAST_CON_ID = IMSContract.MAST_CON_ID,
                        MANE_CONTRACT_ID = IMSContract.MANE_CONTRACT_ID,
                        MANE_AGREEMENT_NUMBER = IMSContract.MANE_AGREEMENT_NUMBER,
                        MANE_CONSTR_COMP_DATE = Convert.ToDateTime(IMSContract.MANE_CONSTR_COMP_DATE).ToString("dd/MM/yyyy"),
                        MANE_AGREEMENT_DATE = Convert.ToDateTime(IMSContract.MANE_AGREEMENT_DATE).ToString("dd/MM/yyyy"),
                        MANE_MAINTENANCE_START_DATE = Convert.ToDateTime(IMSContract.MANE_MAINTENANCE_START_DATE).ToString("dd/MM/yyyy"),
                        //added by abhishek kamble 21-nov-2013
                        MANE_MAINTENANCE_END_DATE = IMSContract.MANE_MAINTENANCE_END_DATE == null ? string.Empty : Convert.ToDateTime(IMSContract.MANE_MAINTENANCE_END_DATE).ToString("dd/MM/yyyy"),
                        MANE_HANDOVER_DATE = IMSContract.MANE_HANDOVER_DATE == null ? string.Empty : Convert.ToDateTime(IMSContract.MANE_HANDOVER_DATE).ToString("dd/MM/yyyy"),
                        MANE_HANDOVER_TO = IMSContract.MANE_HANDOVER_TO,
                        MANE_YEAR1_AMOUNT = IMSContract.MANE_YEAR1_AMOUNT,
                        MANE_YEAR2_AMOUNT = IMSContract.MANE_YEAR2_AMOUNT,
                        MANE_YEAR3_AMOUNT = IMSContract.MANE_YEAR3_AMOUNT,
                        MANE_YEAR4_AMOUNT = IMSContract.MANE_YEAR4_AMOUNT,
                        MANE_YEAR5_AMOUNT = IMSContract.MANE_YEAR5_AMOUNT,
                        IMS_WORK_CODE = IMSContract.IMS_WORK_CODE == null ? 0 : (Int32)IMSContract.IMS_WORK_CODE,
                        IncompleteReason = IMSContract.MANE_INCOMPLETE_REASON,
                        ValueOfWorkDone = IMSContract.MANE_VALUE_WORK_DONE,
                        IsEdit = true
                    };

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementDetails.MANE_YEAR6_AMOUNT = (Decimal)(IMSContract.MANE_YEAR6_AMOUNT == null ? 0 : IMSContract.MANE_YEAR6_AMOUNT);
                    }

                    //Note
                    if (dbContext.MANE_IMS_INSPECTION.Any(x => x.IMS_PR_ROAD_CODE == IMSContract.MANE_PR_CONTRACT_CODE))
                    {
                        if (agreementDetails.lstNote == null)
                        {
                            agreementDetails.lstNote = new List<string>();
                        }
                        agreementDetails.lstNote.Insert(0, "Details entered in Maintenance Inspection");
                    }
                    if (dbContext.MANE_IMS_PROGRESS.Any(x => x.IMS_PR_ROAD_CODE == IMSContract.MANE_PR_CONTRACT_CODE))
                    {
                        if (agreementDetails.lstNote == null)
                        {
                            agreementDetails.lstNote = new List<string>();
                            agreementDetails.lstNote.Insert(0, "Details entered in Maintenance Progress");
                        }
                        else
                        {
                            agreementDetails.lstNote.Insert(1, "Details entered in Maintenance Progress");
                        }
                    }
                    if (dbContext.ACC_BILL_DETAILS.Any(x => x.IMS_AGREEMENT_CODE == IMSContract.MANE_PR_CONTRACT_CODE && x.IMS_PR_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE))
                    {
                        if (agreementDetails.lstNote == null)
                        {
                            agreementDetails.lstNote = new List<string>();
                            agreementDetails.lstNote.Insert(0, "Payment details are entered against agreement");
                        }
                        else
                        {
                            agreementDetails.lstNote.Insert(1, "Payment details are entered against agreement");
                        }
                    }
                }

                return agreementDetails;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }//end GetMaintenanceAgreementDetailsDAL




        public bool UpdateMaintenanceAgreementDetailsDAL_Proposal(MaintenanceAgreementDetails details_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string proposalType = string.Empty;
                int IMSPRRoadCode = 0;
                int PRContractCode = 0;
                EXEC_ROADS_MONTHLY_STATUS roadMonthlyStatus = null;
                EXEC_LSB_MONTHLY_STATUS lsbMonthlyStatus = null;
                CommonFunctions commonFunction = new CommonFunctions();
                encryptedParameters = details_agreement.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                PRContractCode = Convert.ToInt32(decryptedParameters["PRContractCode"].ToString());

                //if (dbContext.MANE_IMS_CONTRACT.Any(pc => pc.IMS_PR_ROAD_CODE == IMSPRRoadCode && pc.MANE_AGREEMENT_NUMBER.ToUpper() == details_agreement.MANE_AGREEMENT_NUMBER.ToUpper() && pc.MANE_PR_CONTRACT_CODE != PRContractCode && pc.MAST_CON_ID != details_agreement.MAST_CON_ID))
                //{
                //    message = "Agreement Number is already exist.";
                //    return false;
                //}

                ///Changes by SAMMED PATIL for LSB and Road Proposals  28/06/2016
                proposalType = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(x => x.IMS_PROPOSAL_TYPE).FirstOrDefault();

                // below block added for Maintenance start date must be greater than or equal to construction completion date .           
                if (proposalType == "P")
                {
                    if (commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_START_DATE) < commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE))
                    {
                        message = "Maintenance start date must be greater than or equal to construction completion date.";
                        return false;
                    }
                    roadMonthlyStatus = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(rms => rms.IMS_PR_ROAD_CODE == IMSPRRoadCode && rms.EXEC_ISCOMPLETED == "C").FirstOrDefault();
                    var ExecutionDay = roadMonthlyStatus.EXEC_COMPLETION_DATE.ToString().Split(' ')[0];  // below block added for comparing day in date for physical progress.
                 
                    if (roadMonthlyStatus != null)
                    {
                        if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE) < commonFunction.GetStringToDateTime(ExecutionDay) || commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE) > commonFunction.GetStringToDateTime(ExecutionDay))
                        {
                            message = "Construction completion date should be equal to physical progress completion date.";
                            return false;
                        }
                        #region
                        //if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year > roadMonthlyStatus.EXEC_PROG_YEAR)
                        //{
                        //    message = "Construction completion date should be less than physical progress completion date.";
                        //    return false;
                        //}
                        //else if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year == roadMonthlyStatus.EXEC_PROG_YEAR && commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Month > roadMonthlyStatus.EXEC_PROG_MONTH)
                        //{
                        //    message = "Construction completion date should be less than physical progress completion date.";
                        //    return false;
                        //}  // below block added for comparing day in date for physical progress.
                        //else if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year == roadMonthlyStatus.EXEC_PROG_YEAR && commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Month == roadMonthlyStatus.EXEC_PROG_MONTH && (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Day < ExecDay || commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Day > ExecDay))
                        //{
                        //    message = "Construction completion date should be equal to physical progress completion date.";
                        //    return false;
                        //}
                        #endregion
                    }
                }
                else
                {
                    if (commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_START_DATE) < commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE))
                    {
                        message = "Maintenance start date must be greater than or equal to construction completion date.";
                        return false;
                    }
                    lsbMonthlyStatus = dbContext.EXEC_LSB_MONTHLY_STATUS.Where(rms => rms.IMS_PR_ROAD_CODE == IMSPRRoadCode && rms.EXEC_ISCOMPLETED == "C").FirstOrDefault();
                    var ExecutionDay = lsbMonthlyStatus.EXEC_COMPLETION_DATE.ToString().Split(' ')[0];                 
                    if (lsbMonthlyStatus != null)
                    {
                        if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE) < commonFunction.GetStringToDateTime(ExecutionDay) || commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE) > commonFunction.GetStringToDateTime(ExecutionDay))
                        {
                            message = "Construction completion date should be equal to physical progress completion date.";
                            return false;
                        }
                        #region
                        //if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year > lsbMonthlyStatus.EXEC_PROG_YEAR)
                        //{
                        //    message = "Construction completion date should be less than physical progress completion date.";
                        //    return false;
                        //}
                        //else if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year == lsbMonthlyStatus.EXEC_PROG_YEAR && commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Month > lsbMonthlyStatus.EXEC_PROG_MONTH)
                        //{
                        //    message = "Construction completion date should be less than physical progress completion date.";
                        //    return false;
                        //}  // below block added for comparing day in date for physical progress.
                        //else if (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Year == lsbMonthlyStatus.EXEC_PROG_YEAR && commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Month == lsbMonthlyStatus.EXEC_PROG_MONTH && (commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Day < ExecDay || commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE).Day > ExecDay))
                        //{
                        //    message = "Construction completion date should be equal to physical progress completion date.";
                        //    return false;
                        //}
                        #endregion
                    }
                }

                MANE_IMS_CONTRACT IMSContract = dbContext.MANE_IMS_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.MANE_PR_CONTRACT_CODE == PRContractCode && c.MANE_CONTRACT_STATUS == "P").FirstOrDefault();

                if (IMSContract == null)
                {
                    return false;
                }

                // IMSContract.MANE_AGREEMENT_NUMBER = details_agreement.MANE_AGREEMENT_NUMBER;
                IMSContract.MANE_AGREEMENT_DATE = commonFunction.GetStringToDateTime(details_agreement.MANE_AGREEMENT_DATE);
                IMSContract.MANE_CONSTR_COMP_DATE = commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE);
                IMSContract.MANE_MAINTENANCE_START_DATE = commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_START_DATE);

                //added by ahishek kamble 21-nov-2013
                if (details_agreement.MANE_MAINTENANCE_END_DATE != null)
                {
                    IMSContract.MANE_MAINTENANCE_END_DATE = commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_END_DATE);
                }
                else
                {
                    IMSContract.MANE_MAINTENANCE_END_DATE = null;
                }


                IMSContract.MANE_HANDOVER_DATE = details_agreement.MANE_HANDOVER_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.MANE_HANDOVER_DATE);
                IMSContract.MANE_YEAR1_AMOUNT = (Decimal)details_agreement.MANE_YEAR1_AMOUNT;
                IMSContract.MANE_YEAR2_AMOUNT = (Decimal)details_agreement.MANE_YEAR2_AMOUNT;
                IMSContract.MANE_YEAR3_AMOUNT = (Decimal)details_agreement.MANE_YEAR3_AMOUNT;
                IMSContract.MANE_YEAR4_AMOUNT = (Decimal)details_agreement.MANE_YEAR4_AMOUNT;
                IMSContract.MANE_YEAR5_AMOUNT = (Decimal)details_agreement.MANE_YEAR5_AMOUNT;
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    IMSContract.MANE_YEAR6_AMOUNT = (Decimal)(details_agreement.MANE_YEAR6_AMOUNT == null ? 0 : details_agreement.MANE_YEAR6_AMOUNT);
                }
                IMSContract.MANE_HANDOVER_TO = details_agreement.MANE_HANDOVER_TO == null ? null : details_agreement.MANE_HANDOVER_TO.Trim();
                IMSContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                IMSContract.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public bool FinalizeAgreementDAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            MANE_IMS_CONTRACT IMSContract = null;
            try
            {
                using (var scope = new TransactionScope())
                {
                    IMSContract = dbContext.MANE_IMS_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.MANE_PR_CONTRACT_CODE == PRContractCode && c.MANE_CONTRACT_FINALIZED == "N" && c.MANE_LOCK_STATUS == "N").FirstOrDefault();

                    if (IMSContract == null)
                    {
                        return false;
                    }

                    IMSContract.MANE_CONTRACT_FINALIZED = "Y";
                    IMSContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    IMSContract.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    if (dbContext.MANE_IMS_CONTRACT.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode).Count() == 1)
                    {
                        if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode && m.IMS_ISCOMPLETED == "C"))
                        {
                            dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).ToList().ForEach(IMS => IMS.IMS_ISCOMPLETED = "X");//&& IMS.IMS_ISCOMPLETED == "W"
                        }
                        dbContext.SaveChanges();
                    }
                    scope.Complete();
                }

                return true;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// definalization of Maintenance Agreement
        /// </summary>
        /// <param name="IMSPRRoadCode"></param>
        /// <param name="PRContractCode"></param>
        /// <returns></returns>
        public bool DeFinalizeAgreementDAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            MANE_IMS_CONTRACT IMSContract = null;
            try
            {
                using (var scope = new TransactionScope())
                {
                    IMSContract = dbContext.MANE_IMS_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.MANE_PR_CONTRACT_CODE == PRContractCode && c.MANE_CONTRACT_FINALIZED == "Y").FirstOrDefault();

                    if (IMSContract == null)
                    {
                        return false;
                    }

                    IMSContract.MANE_CONTRACT_FINALIZED = "N";
                    IMSContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    IMSContract.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    if (dbContext.MANE_IMS_CONTRACT.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode).Count() == 1)
                    {
                        if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode && m.IMS_ISCOMPLETED == "X"))
                        {
                            dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).ToList().ForEach(IMS => IMS.IMS_ISCOMPLETED = "C");//&& IMS.IMS_ISCOMPLETED == "W"
                        }
                        dbContext.SaveChanges();
                    }
                    scope.Complete();
                }

                return true;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public bool ChangeAgreementStatusToInCompleteDAL(Models.Agreement.IncompleteReason incompleteReason, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                MANE_IMS_CONTRACT IMSContract = null;
                int PRContractCode = 0;
                int IMSPRRoadCode = 0;
                decimal totalMaintenanceCost = 0;

                encryptedParameters = incompleteReason.EncryptedTendAgreementCode_IncompleteReason.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                PRContractCode = Convert.ToInt32(decryptedParameters["PRContractCode"].ToString());

                IMSContract = dbContext.MANE_IMS_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.MANE_PR_CONTRACT_CODE == PRContractCode && c.MANE_CONTRACT_STATUS != "I").FirstOrDefault();

                if (IMSContract == null)
                {
                    return false;
                }

                totalMaintenanceCost = (IMSContract.MANE_YEAR1_AMOUNT + IMSContract.MANE_YEAR2_AMOUNT + IMSContract.MANE_YEAR3_AMOUNT + IMSContract.MANE_YEAR4_AMOUNT + IMSContract.MANE_YEAR5_AMOUNT);

                if (incompleteReason.TEND_VALUE_WORK_DONE != 0 && totalMaintenanceCost != 0)
                {
                    if (incompleteReason.TEND_VALUE_WORK_DONE >= totalMaintenanceCost)
                    {
                        message = "Value of Work Done should be less than total maintenance cost.";
                        return false;
                    }
                }

                IMSContract.MANE_CONTRACT_STATUS = "I";
                IMSContract.MANE_INCOMPLETE_REASON = incompleteReason.TEND_INCOMPLETE_REASON == null ? null : incompleteReason.TEND_INCOMPLETE_REASON.Trim();
                IMSContract.MANE_VALUE_WORK_DONE = incompleteReason.TEND_VALUE_WORK_DONE;
                IMSContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                IMSContract.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;

            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }




        public bool DeleteMaintenanceAgreementDetailsDAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                TEND_AGREEMENT_DETAIL agreementDetails = null;

                //check whether the agreement is used in accounts module
                if (dbContext.ACC_BILL_DETAILS.Any(m => m.IMS_AGREEMENT_CODE == PRContractCode && m.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                {
                    return false;
                }


                MANE_IMS_CONTRACT IMSContract = dbContext.MANE_IMS_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.MANE_PR_CONTRACT_CODE == PRContractCode && c.MANE_CONTRACT_FINALIZED != "Y").FirstOrDefault();
                if (IMSContract.IMS_WORK_CODE == null)
                {
                    agreementDetails = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE).FirstOrDefault();

                }
                else
                {
                    agreementDetails = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE && ad.IMS_WORK_CODE == IMSContract.IMS_WORK_CODE).FirstOrDefault();
                }

                if (agreementDetails == null || IMSContract == null)
                {
                    return false;
                }

                using (var scope = new TransactionScope())
                {

                    IMSContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    IMSContract.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.MANE_IMS_CONTRACT.Remove(IMSContract);

                    agreementDetails.TEND_AGREEMENT_STATUS = "C";

                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementDetails.USERID = PMGSYSession.Current.UserId;

                    dbContext.Entry(agreementDetails).State = System.Data.Entity.EntityState.Modified;

                    dbContext.SaveChanges();
                    scope.Complete();
                    return true;
                }

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this agreement details.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }




        public bool CheckForExistingorNewContractorDAL(int IMSPRRoadCode, int IMSWorkCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                var agreementMasterDetails = from tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                                             join tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                             on tendAgreementDetails.TEND_AGREEMENT_CODE equals tendAgreementMaster.TEND_AGREEMENT_CODE
                                             where
                                             tendAgreementDetails.TEND_AGREEMENT_CODE == (from td in dbContext.TEND_AGREEMENT_DETAIL where td.IMS_PR_ROAD_CODE == IMSPRRoadCode select td.TEND_AGREEMENT_CODE).FirstOrDefault()
                                             && (tendAgreementDetails.TEND_AGREEMENT_STATUS == "C" || tendAgreementDetails.TEND_AGREEMENT_STATUS == "M")
                                             select new
                                             {
                                                 tendAgreementMaster.TEND_AGREEMENT_CODE,
                                                 tendAgreementMaster.MAST_CON_ID,
                                                 tendAgreementDetails.IMS_WORK_CODE,
                                                 tendAgreementDetails.IMS_PR_ROAD_CODE,
                                                 tendAgreementDetails.TEND_AGREEMENT_STATUS
                                             };


                if (agreementMasterDetails.All(ad => ad.TEND_AGREEMENT_STATUS == "C"))
                {
                    var imsContracts = from ad in agreementMasterDetails
                                       join imsContract in dbContext.MANE_IMS_CONTRACT
                                       on ad.IMS_PR_ROAD_CODE equals imsContract.IMS_PR_ROAD_CODE
                                       where imsContract.MANE_CONTRACT_STATUS == "P"
                                       select new
                                       {
                                           imsContract.IMS_PR_ROAD_CODE,
                                           imsContract.IMS_WORK_CODE,
                                           imsContract.MAST_CON_ID
                                       };
                    imsContracts = imsContracts.Where(c => c.IMS_PR_ROAD_CODE != IMSPRRoadCode);

                    if (imsContracts.Count() > 0)
                    {
                        return true;
                    }

                }

                return false;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public bool ChangeAgreementStatusToCompleteDAL(int IMSPRRoadCode, int PRContractCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            MANE_IMS_CONTRACT IMSContract = null;

            try
            {
                using (var scope = new TransactionScope())
                {
                    IMSContract = dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode && IMS.MANE_PR_CONTRACT_CODE == PRContractCode && IMS.MANE_CONTRACT_STATUS != "C" && IMS.MANE_LOCK_STATUS == "N").FirstOrDefault();

                    if (IMSContract == null)
                    {
                        return false;
                    }


                    IMSContract.MANE_CONTRACT_STATUS = "C";
                    IMSContract.MANE_INCOMPLETE_REASON = null;
                    IMSContract.MANE_VALUE_WORK_DONE = null;

                    dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    scope.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion

        #region PROPOSAL_RELATED_DETAILS

        public Array GetProposalAgreementListDAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;
                int? maxContractCode = 0;

                var query = from IMSContract in dbContext.MANE_IMS_CONTRACT
                            join workDetails in dbContext.IMS_PROPOSAL_WORK
                            on IMSContract.IMS_WORK_CODE equals workDetails.IMS_WORK_CODE into works
                            from workDetails in works.DefaultIfEmpty()
                            join contractorDetails in dbContext.MASTER_CONTRACTOR
                            on IMSContract.MAST_CON_ID equals contractorDetails.MAST_CON_ID //into contractors
                            //from contractorDetails in contractors.DefaultIfEmpty()
                            where
                            IMSContract.IMS_PR_ROAD_CODE == IMSPRRoadCode
                            //&& IMSContract.MANE_CONTRACT_STATUS != "I"
                            select new
                            {

                                IMSContract.IMS_PR_ROAD_CODE,
                                IMSContract.MANE_PR_CONTRACT_CODE,
                                contractorDetails.MAST_CON_COMPANY_NAME,
                                IMSContract.MANE_AGREEMENT_NUMBER,
                                IMSContract.MANE_AGREEMENT_DATE,
                                IMSContract.MANE_MAINTENANCE_START_DATE,
                                IMSContract.MANE_YEAR1_AMOUNT,
                                IMSContract.MANE_YEAR2_AMOUNT,
                                IMSContract.MANE_YEAR3_AMOUNT,
                                IMSContract.MANE_YEAR4_AMOUNT,
                                IMSContract.MANE_YEAR5_AMOUNT,
                                IMSContract.MANE_CONTRACT_FINALIZED,
                                IMSContract.MANE_LOCK_STATUS,
                                IMSContract.MANE_CONTRACT_STATUS,
                                IMSContract.MANE_CONTRACT_NUMBER,
                                workDetails.IMS_WORK_DESC,
                                IMSContract.IMS_WORK_CODE,
                                IsLatest = true

                            };




                totalRecords = query == null ? 0 : query.Count();

                //maxContractCode = query.Max(q => (Int32?)q.MANE_CONTRACT_NUMBER);
                //maxContractCode = maxContractCode == null ? 0 : maxContractCode;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Work":
                                query = query.OrderBy(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "MaintenanceDate":
                                query = query.OrderBy(x => x.MANE_MAINTENANCE_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderBy(x => x.MANE_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }


                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                query = query.OrderByDescending(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Work":
                                query = query.OrderByDescending(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "MaintenanceDate":
                                query = query.OrderByDescending(x => x.MANE_MAINTENANCE_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderByDescending(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderByDescending(x => x.MANE_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(IMSContract => new
                {

                    IMSContract.IMS_PR_ROAD_CODE,
                    IMSContract.MANE_PR_CONTRACT_CODE,
                    IMSContract.MAST_CON_COMPANY_NAME,
                    IMSContract.MANE_AGREEMENT_NUMBER,
                    IMSContract.MANE_AGREEMENT_DATE,
                    IMSContract.MANE_MAINTENANCE_START_DATE,
                    IMSContract.MANE_YEAR1_AMOUNT,
                    IMSContract.MANE_YEAR2_AMOUNT,
                    IMSContract.MANE_YEAR3_AMOUNT,
                    IMSContract.MANE_YEAR4_AMOUNT,
                    IMSContract.MANE_YEAR5_AMOUNT,
                    IMSContract.MANE_CONTRACT_FINALIZED,
                    IMSContract.MANE_LOCK_STATUS,
                    IMSContract.MANE_CONTRACT_STATUS,
                    IMSContract.MANE_CONTRACT_NUMBER,
                    IMSContract.IMS_WORK_DESC,
                    IMSContract.IMS_WORK_CODE

                }).ToArray();


                return result.Select(IMSContract => new
                {
                    cell = new[] {                                                                               
                                    IMSContract.MANE_AGREEMENT_NUMBER.ToString(),
                                    IMSContract.IMS_WORK_DESC==null?"NA":IMSContract.IMS_WORK_DESC.ToString().Trim(),
                                    IMSContract.MAST_CON_COMPANY_NAME==null?"NA":IMSContract.MAST_CON_COMPANY_NAME.ToString().Trim(),                            
                                    Convert.ToDateTime(IMSContract.MANE_AGREEMENT_DATE).ToString("dd/MM/yyyy"),
                                    Convert.ToDateTime(IMSContract.MANE_MAINTENANCE_START_DATE).ToString("dd/MM/yyyy"),
                                       
                                    ((IMSContract.MANE_YEAR1_AMOUNT)+
                                       (IMSContract.MANE_YEAR2_AMOUNT)+
                                       (IMSContract.MANE_YEAR3_AMOUNT)+
                                       (IMSContract.MANE_YEAR4_AMOUNT)+
                                       (IMSContract.MANE_YEAR5_AMOUNT)
                                    ).ToString(),
                                    AgreementStatus[IMSContract.MANE_CONTRACT_STATUS].ToString(),
                                   // ( (IMSContract.MANE_CONTRACT_NUMBER==maxContractCode && (IMSContract.MANE_CONTRACT_STATUS=="P"||IMSContract.MANE_CONTRACT_STATUS=="I")) ?(IMSContract.MANE_CONTRACT_STATUS =="C" || IMSContract.MANE_CONTRACT_FINALIZED=="N") :(IMSContract.MANE_CONTRACT_STATUS =="C" || IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_FINALIZED=="N"))?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() }),
                                   // ( (IMSContract.MANE_CONTRACT_NUMBER==maxContractCode && (IMSContract.MANE_CONTRACT_STATUS=="P" || IMSContract.MANE_CONTRACT_STATUS=="C"))?(IMSContract.MANE_CONTRACT_STATUS =="I" ): (IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_STATUS=="C" ) )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() }),
                                    
                                    //((CheckIsLatest(IMSContract.IMS_PR_ROAD_CODE, IMSContract.MANE_PR_CONTRACT_CODE, IMSContract.IMS_WORK_CODE, IMSContract.MANE_CONTRACT_NUMBER, IMSContract.MANE_CONTRACT_STATUS )==true) ?(IMSContract.MANE_CONTRACT_STATUS =="C" || IMSContract.MANE_CONTRACT_FINALIZED=="N") :(IMSContract.MANE_CONTRACT_STATUS =="C" || IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_FINALIZED=="N"))?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() }),
                                    //((CheckIsLatest(IMSContract.IMS_PR_ROAD_CODE, IMSContract.MANE_PR_CONTRACT_CODE, IMSContract.IMS_WORK_CODE, IMSContract.MANE_CONTRACT_NUMBER, IMSContract.MANE_CONTRACT_STATUS)==true)?(IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_FINALIZED=="N" ): (IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_STATUS=="C" ) )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() }),
                                    //(IMSContract.MANE_CONTRACT_FINALIZED=="N" && IMSContract.MANE_LOCK_STATUS=="N" && IMSContract.MANE_CONTRACT_STATUS=="P" ) ?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='FinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString()  }) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Finalized'></span></td></tr></table></center>",
                                    //URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString()  }),
                                    //(IMSContract.MANE_CONTRACT_FINALIZED=="Y"||IMSContract.MANE_LOCK_STATUS=="Y" || IMSContract.MANE_CONTRACT_STATUS=="C" || IMSContract.MANE_CONTRACT_STATUS=="I" )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString()  }),
                                    //(IMSContract.MANE_CONTRACT_FINALIZED=="Y"||IMSContract.MANE_LOCK_STATUS=="Y"|| IMSContract.MANE_CONTRACT_STATUS=="C" || IMSContract.MANE_CONTRACT_STATUS=="I")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString()})
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetProposalFinancialListDAL(int proposalCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int? monthCode = 0;
                int? yearCode = 0;
                //if ((dbContext.MANE_IMS_PROGRESS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode)))
                //{
                //    yearCode = (dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).Max(y => (Int32?)y.MANE_PROG_YEAR));
                //    monthCode = (dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_PROG_YEAR == yearCode && m.MANE_MAINTENANCE_NUMBER == contractCode).Max(s => (Int32?)s.MANE_PROG_MONTH));
                //}

                var lstFinancialProgress = (from item in dbContext.MANE_IMS_PROGRESS
                                            where item.IMS_PR_ROAD_CODE == proposalCode //&&
                                            //(contractCode == 0 ? 1 : item.MANE_MAINTENANCE_NUMBER) == (contractCode == 0 ? 1 : contractCode)
                                            select new
                                            {
                                                item.MANE_FINAL_PAYMENT_DATE,
                                                item.MANE_FINAL_PAYMENT_FLAG,
                                                item.MANE_PAYMENT_LASTMONTH,
                                                item.MANE_PAYMENT_THISMONTH,
                                                item.MANE_PROG_MONTH,
                                                item.MANE_PROG_YEAR,
                                                //item.EXEC_PROGRESS_TYPE,
                                                item.MANE_VALUEOFWORK_LASTMONTH,
                                                item.MANE_VALUEOFWORK_THISMONTH,
                                                item.IMS_PR_ROAD_CODE,
                                                item.MANE_MAINTENANCE_NUMBER
                                            });

                totalRecords = lstFinancialProgress.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "EXEC_FINAL_PAYMENT_DATE":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_FINAL_PAYMENT_DATE).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_FINAL_PAYMENT_FLAG":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_FINAL_PAYMENT_FLAG).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PAYMENT_LASTMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_PAYMENT_LASTMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PAYMENT_THISMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_PAYMENT_THISMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PROG_MONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_PROG_MONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PROGRAM_YEAR":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_PROG_YEAR).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "EXEC_PROGRESS_TYPE":
                            //    lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.EXEC_PROGRESS_TYPE).ThenBy(m => m.EXEC_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "EXEC_VALUEOFWORK_LASTMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_VALUEOFWORK_LASTMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_VALUEOFWORK_THISMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_VALUEOFWORK_THISMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "EXEC_FINAL_PAYMENT_DATE":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_FINAL_PAYMENT_DATE).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_FINAL_PAYMENT_FLAG":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_FINAL_PAYMENT_FLAG).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PAYMENT_LASTMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PAYMENT_LASTMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PAYMENT_THISMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PAYMENT_THISMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PROG_MONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PROG_MONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PROGRAM_YEAR":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PROG_YEAR).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "EXEC_PROGRESS_TYPE":
                            //    lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.EXEC_PROGRESS_TYPE).ThenBy(m => m.EXEC_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "EXEC_VALUEOFWORK_LASTMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_VALUEOFWORK_LASTMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_VALUEOFWORK_THISMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_VALUEOFWORK_THISMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }


                        //lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.EXEC_PROG_YEAR).ThenBy(m => m.EXEC_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var gridData = lstFinancialProgress.Select(progressDetails => new
                {
                    progressDetails.MANE_FINAL_PAYMENT_DATE,
                    progressDetails.MANE_FINAL_PAYMENT_FLAG,
                    progressDetails.MANE_PAYMENT_LASTMONTH,
                    progressDetails.MANE_PAYMENT_THISMONTH,
                    progressDetails.MANE_PROG_MONTH,
                    progressDetails.MANE_PROG_YEAR,
                    //progressDetails.EXEC_PROGRESS_TYPE,
                    progressDetails.MANE_VALUEOFWORK_LASTMONTH,
                    progressDetails.MANE_VALUEOFWORK_THISMONTH,
                    progressDetails.IMS_PR_ROAD_CODE,
                    progressDetails.MANE_MAINTENANCE_NUMBER
                }).ToArray();

                return gridData.Select(m => new
                {
                    id = m.IMS_PR_ROAD_CODE.ToString(),
                    cell = new[]
                    {
                        m.MANE_PROG_YEAR.ToString(),
                        m.MANE_PROG_MONTH == 1?"January":(m.MANE_PROG_MONTH == 2?"February":(m.MANE_PROG_MONTH == 3?"March":(m.MANE_PROG_MONTH == 4?"April":(m.MANE_PROG_MONTH == 5?"May":(m.MANE_PROG_MONTH == 6?"June":(m.MANE_PROG_MONTH == 7?"July":m.MANE_PROG_MONTH == 8?"August":(m.MANE_PROG_MONTH == 9?"September":(m.MANE_PROG_MONTH == 10?"October":(m.MANE_PROG_MONTH == 11?"November":"December"))))))))),
                        m.MANE_VALUEOFWORK_LASTMONTH.ToString(),
                        m.MANE_VALUEOFWORK_THISMONTH.ToString(),
                        (m.MANE_VALUEOFWORK_LASTMONTH + m.MANE_VALUEOFWORK_THISMONTH).ToString(),
                        m.MANE_PAYMENT_LASTMONTH.ToString(),
                        m.MANE_PAYMENT_THISMONTH.ToString(),
                        (m.MANE_PAYMENT_LASTMONTH + m.MANE_PAYMENT_THISMONTH).ToString(),
                        m.MANE_FINAL_PAYMENT_FLAG=="Y"?"Yes":"No",
                        m.MANE_FINAL_PAYMENT_DATE==null?"-":Convert.ToDateTime(m.MANE_FINAL_PAYMENT_DATE).ToString("dd/MM/yyyy"),
                        //(m.MANE_PROG_MONTH==monthCode && m.MANE_PROG_YEAR==yearCode)?"<a href='#' title='Click here to edit Financial Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditFinancialProgress('" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+m.IMS_PR_ROAD_CODE.ToString().Trim(),"Month="+m.MANE_PROG_MONTH.ToString().Trim(),"Year="+m.MANE_PROG_YEAR.ToString().Trim(),"ContractCode="+m.MANE_MAINTENANCE_NUMBER.ToString().Trim()}) +"'); return false;'>Add Remarks</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                        //(m.MANE_PROG_MONTH==monthCode && m.MANE_PROG_YEAR==yearCode)?"<a href='#' title='Click here to delete Financial Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFinancialProgress('" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+m.IMS_PR_ROAD_CODE.ToString().Trim(),"Month="+m.MANE_PROG_MONTH.ToString().Trim(),"Year="+m.MANE_PROG_YEAR.ToString().Trim(),"ContractCode="+m.MANE_MAINTENANCE_NUMBER.ToString().Trim()}) +"'); return false;'>Add Remarks</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        #endregion

        #region SPECIAL_MAINTENANCE_AGREEMENT

        /// <summary>
        /// returns the list of proposals for adding the special agreements
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="blockCode"></param>
        /// <param name="sanctionedYear"></param>
        /// <param name="packageID"></param>
        /// <param name="adminNDCode"></param>
        /// <param name="batch"></param>
        /// <param name="collaboration"></param>
        /// <param name="upgradationType"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetCompletedRoadForSpecialAgreementsListDAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                if (packageID.Contains("All"))
                {
                    packageID = "All Packages";
                }

                var query = (from imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                             join execRoads in dbContext.EXEC_ROADS_MONTHLY_STATUS
                             on imsSanctionedProjectDetails.IMS_PR_ROAD_CODE equals execRoads.IMS_PR_ROAD_CODE
                             join blockDetails in dbContext.MASTER_BLOCK
                             on imsSanctionedProjectDetails.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                             join districtDetails in dbContext.MASTER_DISTRICT
                             on imsSanctionedProjectDetails.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                             join stateDetails in dbContext.MASTER_STATE
                             on imsSanctionedProjectDetails.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                             join adminDetails in dbContext.ADMIN_DEPARTMENT
                             on imsSanctionedProjectDetails.MAST_DPIU_CODE equals adminDetails.ADMIN_ND_CODE
                             join fundingAgency in dbContext.MASTER_FUNDING_AGENCY
                             on imsSanctionedProjectDetails.IMS_COLLABORATION equals fundingAgency.MAST_FUNDING_AGENCY_CODE into agencies
                             from fundingAgency in agencies.DefaultIfEmpty()
                             where
                             imsSanctionedProjectDetails.MAST_STATE_CODE == stateCode &&
                             imsSanctionedProjectDetails.MAST_DISTRICT_CODE == districtCode &&
                             imsSanctionedProjectDetails.MAST_DPIU_CODE == adminNDCode &&
                             imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
                             (imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "P" || imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "L") &&
                             (execRoads.EXEC_ISCOMPLETED == "C" || execRoads.EXEC_ISCOMPLETED == "P") &&
                             (sanctionedYear == 0 ? 1 : imsSanctionedProjectDetails.IMS_YEAR) == (sanctionedYear == 0 ? 1 : sanctionedYear) &&
                             (blockCode == 0 ? 1 : imsSanctionedProjectDetails.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                             (packageID == "All Packages" ? "%" : imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper()) == (packageID == "All Packages" ? "%" : packageID.ToUpper()) &&
                             (batch == 0 ? 1 : imsSanctionedProjectDetails.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
                             (collaboration <= 0 ? 1 : imsSanctionedProjectDetails.IMS_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                             (upgradationType == "0" ? "%" : imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT) == (upgradationType == "0" ? "%" : upgradationType) &&
                             imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                             && imsSanctionedProjectDetails.IMS_DPR_STATUS == "N"
                             select new
                             {
                                 imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                                 imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                 imsSanctionedProjectDetails.IMS_ROAD_FROM,
                                 imsSanctionedProjectDetails.IMS_ROAD_TO,
                                 imsSanctionedProjectDetails.IMS_YEAR,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

                                 imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
                                 imsSanctionedProjectDetails.IMS_LOCK_STATUS,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED,
                                 imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                                 imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                                 imsSanctionedProjectDetails.IMS_PAV_LENGTH,
                                 fundingAgency.MAST_FUNDING_AGENCY_NAME,
                                 imsSanctionedProjectDetails.IMS_BATCH,
                                 imsSanctionedProjectDetails.MASTER_BLOCK.MAST_BLOCK_NAME

                             }).Distinct();



                totalRecords = query == null ? 0 : query.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "SanctionedYear":
                                query = query.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "RoadLength":
                                query = query.OrderBy(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Collaboration":
                                query = query.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Package":
                                query = query.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }


                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "SanctionedYear":
                                query = query.OrderByDescending(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "RoadLength":
                                query = query.OrderByDescending(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Collaboration":
                                query = query.OrderByDescending(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Package":
                                query = query.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(imsSanctionedProjectDetails => new
                {
                    imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                    imsSanctionedProjectDetails.IMS_ROAD_NAME,
                    imsSanctionedProjectDetails.IMS_ROAD_FROM,
                    imsSanctionedProjectDetails.IMS_ROAD_TO,
                    imsSanctionedProjectDetails.IMS_YEAR,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

                    imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
                    imsSanctionedProjectDetails.IMS_LOCK_STATUS,
                    imsSanctionedProjectDetails.IMS_SANCTIONED,
                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                    imsSanctionedProjectDetails.IMS_PAV_LENGTH,
                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME,
                    imsSanctionedProjectDetails.IMS_BATCH,
                    imsSanctionedProjectDetails.MAST_BLOCK_NAME
                }).ToArray();


                return result.Select(imsSanctionedProjectDetails => new
                {

                    cell = new[] {                                            
                                    imsSanctionedProjectDetails.MAST_BLOCK_NAME == null ? "-" : imsSanctionedProjectDetails.MAST_BLOCK_NAME.ToString(),           
                                    imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString() ,
                                    imsSanctionedProjectDetails.IMS_BATCH == null ? "-" : ("Batch -" + imsSanctionedProjectDetails.IMS_BATCH).ToString(),
                                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,    
                                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "P" ? "Road" :imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "L" ? "Bridge" : "Building",
                                    imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                    imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString(),
                                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME==null?"NA":imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME.Trim(),                                                              
                                    
                                    //imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? 
                                    //(imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT+
                                    //imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT).ToString() : (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT).ToString(),    

                                    ///Change made by SAMMED PATIL on 29MAR2016 
                                    PMGSYSession.Current.PMGSYScheme == 1 ? 
                                                      ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT)).ToString()
                                                    : ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT)).ToString(),
                                    
                                    (imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5).ToString(),
                                    "<a href='#' class='ui-icon ui-icon-zoomin ui-align-center' onclick='ViewSpecialMaintenanceAgreement(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString() /*, "IMSRoadName =" + imsSanctionedProjectDetails.IMS_ROAD_NAME.ToString().Replace('/', '_'), "SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID,"RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")*/}) + "\"); return false;'></a>"   // Parameters commented by Shreyas on 22-07-2022 to reduce URL size as suggested by Pankaj Sir
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// returns the list of special maintenance agreements
        /// </summary>
        /// <param name="IMSPRRoadCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetSpecialAgreementDetailsListDAL(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                var query = from IMSContract in dbContext.MANE_IMS_CONTRACT
                            join workDetails in dbContext.IMS_PROPOSAL_WORK
                            on IMSContract.IMS_WORK_CODE equals workDetails.IMS_WORK_CODE into works
                            from workDetails in works.DefaultIfEmpty()
                            join contractorDetails in dbContext.MASTER_CONTRACTOR
                            on IMSContract.MAST_CON_ID equals contractorDetails.MAST_CON_ID
                            where
                            IMSContract.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                            IMSContract.MANE_AGREEMENT_TYPE == "S"
                            select new
                            {

                                IMSContract.IMS_PR_ROAD_CODE,
                                IMSContract.MANE_PR_CONTRACT_CODE,
                                MAST_CON_COMPANY_NAME = contractorDetails.MAST_CON_COMPANY_NAME + (contractorDetails.MAST_CON_PAN != "" ? " (" + contractorDetails.MAST_CON_PAN + ")" : ""),
                                IMSContract.MANE_AGREEMENT_NUMBER,
                                IMSContract.MANE_AGREEMENT_DATE,
                                IMSContract.MANE_MAINTENANCE_START_DATE,
                                IMSContract.MANE_YEAR1_AMOUNT,
                                IMSContract.MANE_YEAR2_AMOUNT,
                                IMSContract.MANE_YEAR3_AMOUNT,
                                IMSContract.MANE_YEAR4_AMOUNT,
                                IMSContract.MANE_YEAR5_AMOUNT,
                                IMSContract.MANE_CONTRACT_FINALIZED,
                                IMSContract.MANE_LOCK_STATUS,
                                IMSContract.MANE_CONTRACT_STATUS,
                                IMSContract.MANE_CONTRACT_NUMBER,
                                workDetails.IMS_WORK_DESC,
                                IMSContract.IMS_WORK_CODE,
                                IsLatest = true,
                                IMSContract.MANE_CONTRACT_ID

                            };

                totalRecords = query == null ? 0 : query.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Work":
                                query = query.OrderBy(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "MaintenanceDate":
                                query = query.OrderBy(x => x.MANE_MAINTENANCE_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderBy(x => x.MANE_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                query = query.OrderByDescending(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Work":
                                query = query.OrderByDescending(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "MaintenanceDate":
                                query = query.OrderByDescending(x => x.MANE_MAINTENANCE_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderByDescending(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderByDescending(x => x.MANE_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(IMSContract => new
                {
                    IMSContract.IMS_PR_ROAD_CODE,
                    IMSContract.MANE_PR_CONTRACT_CODE,
                    IMSContract.MAST_CON_COMPANY_NAME,
                    IMSContract.MANE_AGREEMENT_NUMBER,
                    IMSContract.MANE_AGREEMENT_DATE,
                    IMSContract.MANE_MAINTENANCE_START_DATE,
                    IMSContract.MANE_YEAR1_AMOUNT,
                    IMSContract.MANE_YEAR2_AMOUNT,
                    IMSContract.MANE_YEAR3_AMOUNT,
                    IMSContract.MANE_YEAR4_AMOUNT,
                    IMSContract.MANE_YEAR5_AMOUNT,
                    IMSContract.MANE_CONTRACT_FINALIZED,
                    IMSContract.MANE_LOCK_STATUS,
                    IMSContract.MANE_CONTRACT_STATUS,
                    IMSContract.MANE_CONTRACT_NUMBER,
                    IMSContract.IMS_WORK_DESC,
                    IMSContract.IMS_WORK_CODE,
                    IMSContract.MANE_CONTRACT_ID

                }).ToArray();


                return result.Select(IMSContract => new
                {
                    cell = new[] {                                                                               
                                    
                                       
                                    IMSContract.MANE_AGREEMENT_NUMBER.ToString(),
                                    IMSContract.IMS_WORK_DESC==null?"NA":IMSContract.IMS_WORK_DESC.ToString().Trim(),
                                    IMSContract.MAST_CON_COMPANY_NAME==null?"NA":IMSContract.MAST_CON_COMPANY_NAME.ToString().Trim(),                            
                                    Convert.ToDateTime(IMSContract.MANE_AGREEMENT_DATE).ToString("dd/MM/yyyy"),
                                    Convert.ToDateTime(IMSContract.MANE_MAINTENANCE_START_DATE).ToString("dd/MM/yyyy"),
                                    ((IMSContract.MANE_YEAR1_AMOUNT)+
                                       (IMSContract.MANE_YEAR2_AMOUNT)+
                                       (IMSContract.MANE_YEAR3_AMOUNT)+
                                       (IMSContract.MANE_YEAR4_AMOUNT)+
                                       (IMSContract.MANE_YEAR5_AMOUNT)
                                    ).ToString(),
                                    AgreementStatus[IMSContract.MANE_CONTRACT_STATUS].ToString(),
                                    (IMSContract.MANE_CONTRACT_STATUS == "I" || IMSContract.MANE_CONTRACT_FINALIZED=="N") ? string.Empty : (((CheckIsLatest(IMSContract.IMS_PR_ROAD_CODE, IMSContract.MANE_PR_CONTRACT_CODE, IMSContract.IMS_WORK_CODE, IMSContract.MANE_CONTRACT_NUMBER, IMSContract.MANE_CONTRACT_STATUS )==true) ?(IMSContract.MANE_CONTRACT_STATUS =="C" || IMSContract.MANE_CONTRACT_FINALIZED=="N") :(IMSContract.MANE_CONTRACT_STATUS =="C" || IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_FINALIZED=="N"))?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()})),
                                    (IMSContract.MANE_CONTRACT_STATUS == "I" || IMSContract.MANE_CONTRACT_FINALIZED=="N" || IMSContract.MANE_CONTRACT_STATUS == "C") ? string.Empty :((CheckIsLatest(IMSContract.IMS_PR_ROAD_CODE, IMSContract.MANE_PR_CONTRACT_CODE, IMSContract.IMS_WORK_CODE, IMSContract.MANE_CONTRACT_NUMBER, IMSContract.MANE_CONTRACT_STATUS)==true)?(IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_FINALIZED=="N" ): (IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_STATUS=="C" ) )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString() }),
                                    (IMSContract.MANE_CONTRACT_FINALIZED=="N" && IMSContract.MANE_LOCK_STATUS=="N" && IMSContract.MANE_CONTRACT_STATUS=="P" ) ?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='FinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID  }) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Finalized'></span></td></tr></table></center>",
                                    IMSContract.MANE_CONTRACT_STATUS=="C" ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='DeFinalized'></span></td></tr></table></center>" :(dbContext.ACC_BILL_DETAILS.Any(m=>m.IMS_PR_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE && m.IMS_AGREEMENT_CODE == IMSContract.MANE_PR_CONTRACT_CODE) || IMSContract.MANE_CONTRACT_FINALIZED == "N" || IMSContract.MANE_CONTRACT_STATUS == "I") ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='DeFinalized'></span></td></tr></table></center>" :"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='DeFinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() ,"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()}) + "\");' ></span></td></tr></table></center>",
                                    URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()  }),
                                    (IMSContract.MANE_CONTRACT_FINALIZED=="Y"||IMSContract.MANE_LOCK_STATUS=="Y" || IMSContract.MANE_CONTRACT_STATUS=="C" || IMSContract.MANE_CONTRACT_STATUS=="I" )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()  }),
                                    (IMSContract.MANE_CONTRACT_FINALIZED=="Y"||IMSContract.MANE_LOCK_STATUS=="Y"|| IMSContract.MANE_CONTRACT_STATUS=="C" || IMSContract.MANE_CONTRACT_STATUS=="I")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()})
                    }
                }).ToArray();
            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// saves the details of maintenance agreement details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveSpecialAgreementDetailsDAL(MaintenanceAgreementDetails model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string proposalType = string.Empty;
                EXEC_LSB_MONTHLY_STATUS lsbMonthlyStatus = null;
                MANE_IMS_CONTRACT agreementDetails = null;
                EXEC_ROADS_MONTHLY_STATUS roadMonthlyStatus = null;
                int IMSPRRoadCode = 0;
                int? maxContractorNumber = 1;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                CommonFunctions commonFunction = new CommonFunctions();

                encryptedParameters = model.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString().Trim());

                var IMSContractList = (from IMSContracts in dbContext.MANE_IMS_CONTRACT
                                       join IMSSanctioned in dbContext.IMS_SANCTIONED_PROJECTS
                                       on IMSContracts.IMS_PR_ROAD_CODE equals IMSSanctioned.IMS_PR_ROAD_CODE
                                       where
                                       IMSSanctioned.MAST_STATE_CODE == stateCode &&
                                       IMSSanctioned.MAST_DISTRICT_CODE == districtCode &&
                                       IMSContracts.MANE_AGREEMENT_NUMBER.ToUpper() == model.MANE_AGREEMENT_NUMBER.ToUpper()
                                       select new
                                       {

                                           IMSContracts.MANE_AGREEMENT_NUMBER,
                                           IMSContracts.MAST_CON_ID,
                                           IMSContracts.IMS_PR_ROAD_CODE,
                                           IMSSanctioned.MAST_STATE_CODE,
                                           IMSSanctioned.MAST_DISTRICT_CODE

                                       }).FirstOrDefault();
                /// Changed by SAMMED PATIL for LSB and Road Proposal  28/06/2016
                proposalType = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(x => x.IMS_PROPOSAL_TYPE).FirstOrDefault();
                if (proposalType == "P")
                {
                    roadMonthlyStatus = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(rms => rms.IMS_PR_ROAD_CODE == IMSPRRoadCode && rms.EXEC_ISCOMPLETED == "C").FirstOrDefault();

                    if (roadMonthlyStatus != null)
                    {
                        if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year > roadMonthlyStatus.EXEC_PROG_YEAR || commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year < roadMonthlyStatus.EXEC_PROG_YEAR)
                        {
                            message = "Construction completion date should be equal to physical progress completion date.";
                            return false;
                        }
                        else if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year == roadMonthlyStatus.EXEC_PROG_YEAR && (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Month > roadMonthlyStatus.EXEC_PROG_MONTH || commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Month < roadMonthlyStatus.EXEC_PROG_MONTH))
                        {
                            message = "Construction completion date should be equal to physical progress completion date.";
                            return false;
                        }
                    }
                }
                else
                {
                    lsbMonthlyStatus = dbContext.EXEC_LSB_MONTHLY_STATUS.Where(rms => rms.IMS_PR_ROAD_CODE == IMSPRRoadCode && rms.EXEC_ISCOMPLETED == "C").FirstOrDefault();

                    if (lsbMonthlyStatus != null)
                    {
                        if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year > lsbMonthlyStatus.EXEC_PROG_YEAR || commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year < lsbMonthlyStatus.EXEC_PROG_YEAR)
                        {
                            message = "Construction completion date should be equal to physical progress completion date.";
                            return false;
                        }
                        else if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year == lsbMonthlyStatus.EXEC_PROG_YEAR && (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Month > lsbMonthlyStatus.EXEC_PROG_MONTH || commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Month < lsbMonthlyStatus.EXEC_PROG_MONTH))
                        {
                            message = "Construction completion date should be equal to physical progress completion date.";
                            return false;
                        }
                    }
                }
                using (var scope = new TransactionScope())
                {
                    agreementDetails = new MANE_IMS_CONTRACT();
                    agreementDetails.IMS_PR_ROAD_CODE = IMSPRRoadCode;
                    agreementDetails.MANE_PR_CONTRACT_CODE = (Int32)GetMaxCode(MaintenanceAgreementModules.IMSContract, IMSPRRoadCode);
                    agreementDetails.MANE_CONTRACT_ID = dbContext.MANE_IMS_CONTRACT.Max(cp => (Int32?)cp.MANE_CONTRACT_ID) == null ? 1 : (Int32)dbContext.MANE_IMS_CONTRACT.Max(cp => (Int32?)cp.MANE_CONTRACT_ID) + 1;
                    agreementDetails.MAST_CON_ID = model.MAST_CON_ID;
                    agreementDetails.MANE_AGREEMENT_NUMBER = model.MANE_AGREEMENT_NUMBER;
                    agreementDetails.MANE_AGREEMENT_DATE = commonFunction.GetStringToDateTime(model.MANE_AGREEMENT_DATE);
                    agreementDetails.MANE_CONSTR_COMP_DATE = commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE);
                    agreementDetails.MANE_MAINTENANCE_START_DATE = commonFunction.GetStringToDateTime(model.MANE_MAINTENANCE_START_DATE);

                    //added by ahishek kamble 21-nov-2013
                    if (model.MANE_MAINTENANCE_END_DATE != null)
                    {
                        agreementDetails.MANE_MAINTENANCE_END_DATE = commonFunction.GetStringToDateTime(model.MANE_MAINTENANCE_END_DATE);
                    }
                    else
                    {
                        agreementDetails.MANE_MAINTENANCE_END_DATE = null;
                    }

                    agreementDetails.MANE_HANDOVER_DATE = model.MANE_HANDOVER_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(model.MANE_HANDOVER_DATE);
                    agreementDetails.MANE_YEAR1_AMOUNT = (Decimal)model.MANE_YEAR1_AMOUNT;
                    agreementDetails.MANE_YEAR2_AMOUNT = 0;
                    agreementDetails.MANE_YEAR3_AMOUNT = 0;
                    agreementDetails.MANE_YEAR4_AMOUNT = 0;
                    agreementDetails.MANE_YEAR5_AMOUNT = 0;
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementDetails.MANE_YEAR6_AMOUNT = (Decimal)(model.MANE_YEAR6_AMOUNT == null ? 0 : model.MANE_YEAR6_AMOUNT);
                    }
                    agreementDetails.MANE_HANDOVER_TO = model.MANE_HANDOVER_TO == null ? null : model.MANE_HANDOVER_TO.Trim();
                    agreementDetails.MANE_CONTRACT_STATUS = "P";
                    agreementDetails.MANE_CONTRACT_FINALIZED = "N";
                    agreementDetails.MANE_LOCK_STATUS = "N";
                    agreementDetails.MANE_AGREEMENT_TYPE = "S";
                    if (model.IMS_WORK_CODE > 0)
                    {
                        agreementDetails.IMS_WORK_CODE = model.IMS_WORK_CODE;
                        agreementDetails.MANE_PART_AGREEMENT = "Y";


                        if (dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_WORK_CODE == model.IMS_WORK_CODE).Any())
                        {
                            agreementDetails.MANE_FIRST_AGREEMENT = "N";
                        }
                        else
                        {
                            agreementDetails.MANE_FIRST_AGREEMENT = "Y";
                        }

                    }
                    else
                    {
                        agreementDetails.MANE_PART_AGREEMENT = "N";

                        if (dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Any())
                        {
                            agreementDetails.MANE_FIRST_AGREEMENT = "N";
                        }
                        else
                        {
                            agreementDetails.MANE_FIRST_AGREEMENT = "Y";
                        }

                    }


                    //for add contractor number count 
                    if (dbContext.MANE_IMS_CONTRACT.Any(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                    {
                        MANE_IMS_CONTRACT IMSContracts = null;

                        if (model.IMS_WORK_CODE > 0)
                        {
                            IMSContracts = dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode && IMS.IMS_WORK_CODE == model.IMS_WORK_CODE).OrderByDescending(IMS => IMS.MANE_PR_CONTRACT_CODE).FirstOrDefault();
                        }
                        else if (model.IMS_WORK_CODE == 0)
                        {
                            IMSContracts = dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(IMS => IMS.MANE_PR_CONTRACT_CODE).FirstOrDefault();
                        }

                        if (IMSContracts != null)
                        {
                            if (IMSContracts.MANE_CONTRACT_STATUS == "I")
                            {
                                maxContractorNumber = IMSContracts.MANE_CONTRACT_NUMBER;
                            }
                            else
                            {
                                maxContractorNumber = IMSContracts.MANE_CONTRACT_NUMBER + 1;
                            }
                        }

                    }

                    agreementDetails.MANE_CONTRACT_NUMBER = (int)maxContractorNumber;
                    agreementDetails.USERID = PMGSYSession.Current.UserId;
                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.MANE_IMS_CONTRACT.Add(agreementDetails);

                    //update tender status to M when contractor not continue with same contractor
                    if (!model.IsNewContractor)
                    {
                        TEND_AGREEMENT_DETAIL tendAgreementDetails = null;
                        if (model.IMS_WORK_CODE > 0)
                        {
                            tendAgreementDetails = (from agreementDetail in dbContext.TEND_AGREEMENT_DETAIL
                                                    join agreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                                    on agreementDetail.TEND_AGREEMENT_CODE equals agreementMaster.TEND_AGREEMENT_CODE
                                                    where
                                                    agreementDetail.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                                    agreementDetail.IMS_WORK_CODE == model.IMS_WORK_CODE &&
                                                    agreementDetail.TEND_AGREEMENT_STATUS == "C" &&
                                                    agreementMaster.TEND_AGREEMENT_TYPE == "C"
                                                    select agreementDetail
                                                       ).FirstOrDefault();
                        }
                        else if (model.IMS_WORK_CODE == 0)
                        {
                            tendAgreementDetails = (from agreementDetail in dbContext.TEND_AGREEMENT_DETAIL
                                                    join agreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                                    on agreementDetail.TEND_AGREEMENT_CODE equals agreementMaster.TEND_AGREEMENT_CODE
                                                    where
                                                    agreementDetail.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                                    agreementDetail.TEND_AGREEMENT_STATUS == "C" &&
                                                    agreementMaster.TEND_AGREEMENT_TYPE == "C"
                                                    select agreementDetail
                                                      ).FirstOrDefault();
                        }

                        if (tendAgreementDetails != null)
                        {
                            tendAgreementDetails.TEND_AGREEMENT_STATUS = "M";
                            tendAgreementDetails.USERID = PMGSYSession.Current.UserId;
                            tendAgreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(tendAgreementDetails).State = System.Data.Entity.EntityState.Modified;
                        }
                    }

                    dbContext.SaveChanges();
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        /// <summary>
        /// updates the details of special agreement
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UpdateSpecialAgreementDetailsDAL(MaintenanceAgreementDetails model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string proposaType = string.Empty;
                int IMSPRRoadCode = 0;
                int PRContractCode = 0;
                EXEC_ROADS_MONTHLY_STATUS roadMonthlyStatus = null;
                EXEC_LSB_MONTHLY_STATUS lsbMonthlyStatus = null;
                CommonFunctions commonFunction = new CommonFunctions();
                encryptedParameters = model.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                PRContractCode = Convert.ToInt32(decryptedParameters["PRContractCode"].ToString());

                ///Changes by SAMMED PATIL for LSB and Road proposals 28/06/2016
                proposaType = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(x => x.IMS_PROPOSAL_TYPE).FirstOrDefault();
                if (proposaType == "P")
                {
                    roadMonthlyStatus = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(rms => rms.IMS_PR_ROAD_CODE == IMSPRRoadCode && rms.EXEC_ISCOMPLETED == "C").FirstOrDefault();

                    if (roadMonthlyStatus != null)
                    {
                        if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year > roadMonthlyStatus.EXEC_PROG_YEAR)
                        {
                            message = "Construction completion date should be less than physical progress completion date.";
                            return false;
                        }
                        else if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year == roadMonthlyStatus.EXEC_PROG_YEAR && commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Month > roadMonthlyStatus.EXEC_PROG_MONTH)
                        {
                            message = "Construction completion date should be less than physical progress completion date.";
                            return false;
                        }
                    }
                }
                else
                {
                    lsbMonthlyStatus = dbContext.EXEC_LSB_MONTHLY_STATUS.Where(rms => rms.IMS_PR_ROAD_CODE == IMSPRRoadCode && rms.EXEC_ISCOMPLETED == "C").FirstOrDefault();

                    if (lsbMonthlyStatus != null)
                    {
                        if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year > lsbMonthlyStatus.EXEC_PROG_YEAR)
                        {
                            message = "Construction completion date should be less than physical progress completion date.";
                            return false;
                        }
                        else if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year == lsbMonthlyStatus.EXEC_PROG_YEAR && commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Month > lsbMonthlyStatus.EXEC_PROG_MONTH)
                        {
                            message = "Construction completion date should be less than physical progress completion date.";
                            return false;
                        }
                    }
                }

                MANE_IMS_CONTRACT IMSContract = dbContext.MANE_IMS_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.MANE_PR_CONTRACT_CODE == PRContractCode && c.MANE_CONTRACT_STATUS == "P").FirstOrDefault();

                if (IMSContract == null)
                {
                    return false;
                }

                IMSContract.MANE_AGREEMENT_DATE = commonFunction.GetStringToDateTime(model.MANE_AGREEMENT_DATE);
                IMSContract.MANE_CONSTR_COMP_DATE = commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE);
                IMSContract.MANE_MAINTENANCE_START_DATE = commonFunction.GetStringToDateTime(model.MANE_MAINTENANCE_START_DATE);

                if (model.MANE_MAINTENANCE_END_DATE != null)
                {
                    IMSContract.MANE_MAINTENANCE_END_DATE = commonFunction.GetStringToDateTime(model.MANE_MAINTENANCE_END_DATE);
                }
                else
                {
                    IMSContract.MANE_MAINTENANCE_END_DATE = null;
                }


                IMSContract.MANE_HANDOVER_DATE = model.MANE_HANDOVER_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(model.MANE_HANDOVER_DATE);
                IMSContract.MANE_YEAR1_AMOUNT = (Decimal)model.MANE_YEAR1_AMOUNT;
                IMSContract.MANE_YEAR2_AMOUNT = 0;
                IMSContract.MANE_YEAR3_AMOUNT = 0;
                IMSContract.MANE_YEAR4_AMOUNT = 0;
                IMSContract.MANE_YEAR5_AMOUNT = 0;
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    IMSContract.MANE_YEAR6_AMOUNT = (Decimal)(model.MANE_YEAR6_AMOUNT == null ? 0 : model.MANE_YEAR6_AMOUNT);
                }
                IMSContract.MANE_HANDOVER_TO = model.MANE_HANDOVER_TO == null ? null : model.MANE_HANDOVER_TO.Trim();
                IMSContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                IMSContract.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }



        #endregion

        #region TECHNOLOGY

        /// <summary>
        /// saves the Technology details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddTechnologyDetailsDAL(MaintenanceTechnologyDetailsViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            dbContext = new PMGSYEntities();
            try
            {
                IMS_SANCTIONED_PROJECTS sancMaster = dbContext.IMS_SANCTIONED_PROJECTS.Find(model.IMS_PR_ROAD_CODE);
                decimal pavmentLength = sancMaster.IMS_PAV_LENGTH;
                decimal totalPavementLengthEntered = 0;


                // if (dbContext.IMS_PROPOSAL_TECH.Any(m => m.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE && m.IMS_START_CHAINAGE == model.IMS_START_CHAINAGE && m.IMS_END_CHAINAGE == model.IMS_END_CHAINAGE && m.MAST_TECH_CODE == model.MAST_TECH_CODE && m.MAST_LAYER_CODE == model.MAST_LAYER_CODE))
                if (dbContext.MANE_IMS_TECH.Any(m => m.MANE_PR_CONTRACT_CODE == model.MANE_CONTRACT_CODE && m.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE && m.IMS_START_CHAINAGE == model.IMS_START_CHAINAGE && m.IMS_END_CHAINAGE == model.IMS_END_CHAINAGE && m.MAST_TECH_CODE == model.MAST_TECH_CODE && m.MAST_LAYER_CODE == model.MAST_LAYER_CODE))
                {
                    message = "Technology details already entered.";
                    return false;
                }

                MANE_IMS_TECH techMaster = new MANE_IMS_TECH();
                if (dbContext.MANE_IMS_TECH.Any(m => m.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE && m.MANE_PR_CONTRACT_CODE == model.MANE_CONTRACT_CODE))
                {
                    techMaster.IMS_SEGMENT_NO = dbContext.MANE_IMS_TECH.Where(m => m.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE && m.MANE_PR_CONTRACT_CODE == model.MANE_CONTRACT_CODE).Max(m => m.IMS_SEGMENT_NO) + 1;
                }
                else
                {
                    techMaster.IMS_SEGMENT_NO = 1;
                }
                techMaster.IMS_END_CHAINAGE = model.IMS_END_CHAINAGE;
                techMaster.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                techMaster.MANE_PR_CONTRACT_CODE = model.MANE_CONTRACT_CODE;
                techMaster.IMS_START_CHAINAGE = model.IMS_START_CHAINAGE;
                techMaster.MAST_LAYER_CODE = model.MAST_LAYER_CODE;
                techMaster.MAST_TECH_CODE = model.MAST_TECH_CODE;
                techMaster.IMS_TECH_COST = model.IMS_TECH_COST;
                techMaster.IMS_LAYER_COST = model.IMS_LAYER_COST;
                techMaster.USERID = PMGSYSession.Current.UserId;
                techMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.MANE_IMS_TECH.Add(techMaster);
                dbContext.SaveChanges();
                message = "Technology details added successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns the technology details for updation
        /// </summary>
        /// <param name="proposalCode"></param>
        /// <param name="segmentCode"></param>
        /// <returns></returns>
        public MaintenanceTechnologyDetailsViewModel GetTechnologyDetails(int proposalCode, int contractCode, int segmentCode)
        {
            var dbContext = new PMGSYEntities();
            MaintenanceTechnologyDetailsViewModel model = new MaintenanceTechnologyDetailsViewModel();
            try
            {
                MANE_IMS_TECH techMaster = dbContext.MANE_IMS_TECH.Find(proposalCode, contractCode, segmentCode);
                model.EncryptedProposalSegmentCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + techMaster.IMS_PR_ROAD_CODE.ToString().Trim(), "ContractCode=" + techMaster.MANE_PR_CONTRACT_CODE.ToString().Trim(), "SegmentCode=" + techMaster.IMS_SEGMENT_NO.ToString().Trim() });
                model.IMS_END_CHAINAGE = techMaster.IMS_END_CHAINAGE;
                model.IMS_START_CHAINAGE = techMaster.IMS_START_CHAINAGE;
                model.MAST_LAYER_CODE = techMaster.MAST_LAYER_CODE;
                model.MAST_TECH_CODE = techMaster.MAST_TECH_CODE;
                model.IMS_PR_ROAD_CODE = techMaster.IMS_PR_ROAD_CODE;
                model.IMS_TECH_COST = techMaster.IMS_TECH_COST;
                model.IMS_LAYER_COST = techMaster.IMS_LAYER_COST;
                model.MANE_CONTRACT_CODE = techMaster.MANE_PR_CONTRACT_CODE;
                model.Operation = "E";
                return model;
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
        /// <summary>
        /// updates the technology details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>

        public bool EditTechnologyDetailsDAL(MaintenanceTechnologyDetailsViewModel model, ref string message)
        {
            var dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            String[] urlParams = null;
            try
            {
                IMS_SANCTIONED_PROJECTS sancMaster = dbContext.IMS_SANCTIONED_PROJECTS.Find(model.IMS_PR_ROAD_CODE);
                decimal pavmentLength = sancMaster.IMS_PAV_LENGTH;
                decimal totalPavementLengthEntered = 0;
                urlParams = model.EncryptedProposalSegmentCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlParams[0], urlParams[1], urlParams[2] });
                if (dbContext.MANE_IMS_TECH.Any(m => m.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE && m.MANE_PR_CONTRACT_CODE == model.MANE_CONTRACT_CODE))
                {
                    var lstSegments = dbContext.MANE_IMS_TECH.Where(m => m.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE && m.MANE_PR_CONTRACT_CODE == model.MANE_CONTRACT_CODE).ToList();
                    foreach (var item in lstSegments)
                    {
                        totalPavementLengthEntered = totalPavementLengthEntered + (item.IMS_END_CHAINAGE - item.IMS_START_CHAINAGE);
                    }
                }

                int segmentNo = Convert.ToInt32(decryptedParameters["SegmentCode"]);
                MANE_IMS_TECH master = dbContext.MANE_IMS_TECH.Where(m => m.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE && m.MANE_PR_CONTRACT_CODE == model.MANE_CONTRACT_CODE && m.IMS_SEGMENT_NO == segmentNo).FirstOrDefault();
                totalPavementLengthEntered = totalPavementLengthEntered - (master.IMS_END_CHAINAGE - master.IMS_START_CHAINAGE);
                totalPavementLengthEntered = totalPavementLengthEntered + (model.IMS_END_CHAINAGE - model.IMS_START_CHAINAGE);

                //if (totalPavementLengthEntered > pavmentLength)
                //{
                //    message = "Total chainage entered is greater than total pavement length of road.";
                //    return false;
                //}

                MANE_IMS_TECH techMaster = new MANE_IMS_TECH();
                techMaster = dbContext.MANE_IMS_TECH.Find(Convert.ToInt32(decryptedParameters["ProposalCode"]), Convert.ToInt32(decryptedParameters["ContractCode"]), Convert.ToInt32(decryptedParameters["SegmentCode"]));
                techMaster.IMS_END_CHAINAGE = model.IMS_END_CHAINAGE;
                techMaster.IMS_START_CHAINAGE = model.IMS_START_CHAINAGE;
                techMaster.MAST_LAYER_CODE = model.MAST_LAYER_CODE;
                techMaster.MAST_TECH_CODE = model.MAST_TECH_CODE;
                techMaster.IMS_TECH_COST = model.IMS_TECH_COST;
                techMaster.IMS_LAYER_COST = model.IMS_LAYER_COST;
                techMaster.USERID = PMGSYSession.Current.UserId;
                techMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(techMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                message = "Technology details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// deletes the technology details
        /// </summary>
        /// <param name="proposalCode"></param>
        /// <param name="segmentCode"></param>
        /// <returns></returns>
        public bool DeleteTechnologyDetails(int proposalCode, int segmentCode, int contractCode)
        {
            var dbContext = new PMGSYEntities();
            try
            {
                MANE_IMS_TECH techMaster = dbContext.MANE_IMS_TECH.Find(proposalCode, contractCode, segmentCode);
                techMaster.USERID = PMGSYSession.Current.UserId;
                techMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.MANE_IMS_TECH.Remove(techMaster);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        /// <summary>
        /// returns the list of technology details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="proposalCode"></param>
        /// <returns></returns>
        public Array GetTechnologyDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode, int contractCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                //var lstTechnologyDetails = dbContext.IMS_PROPOSAL_TECH.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.IMS_SEGMENT_NO).ToList();
                int maxTechCode = 0;
                var lstTechnologyDetails = (from item in dbContext.MANE_IMS_TECH
                                            where item.IMS_PR_ROAD_CODE == proposalCode
                                            select new
                                            {
                                                item.IMS_PR_ROAD_CODE,
                                                item.IMS_END_CHAINAGE,
                                                item.IMS_SEGMENT_NO,
                                                item.IMS_START_CHAINAGE,
                                                item.MASTER_EXECUTION_ITEM.MAST_HEAD_DESC,
                                                item.MASTER_TECHNOLOGY.MAST_TECH_NAME,
                                                item.IMS_TECH_COST,
                                                item.IMS_LAYER_COST,
                                                item.MASTER_TECHNOLOGY.MAST_TECH_TYPE,
                                                item.MANE_PR_CONTRACT_CODE
                                            }).OrderByDescending(m => m.IMS_SEGMENT_NO).Distinct();

                totalRecords = lstTechnologyDetails.Count();

                if (dbContext.MANE_IMS_TECH.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_PR_CONTRACT_CODE == contractCode))
                {
                    maxTechCode = dbContext.MANE_IMS_TECH.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.IMS_SEGMENT_NO).ThenByDescending(m => m.MAST_TECH_CODE).ThenByDescending(m => m.MAST_LAYER_CODE).Select(m => m.IMS_SEGMENT_NO).FirstOrDefault();
                }

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "IMS_SEGMENT_NO":
                                lstTechnologyDetails = lstTechnologyDetails.OrderBy(m => m.IMS_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "O_CDWORKS_COST":
                                lstTechnologyDetails = lstTechnologyDetails.OrderBy(m => m.IMS_START_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_END_CHAINAGE":
                                lstTechnologyDetails = lstTechnologyDetails.OrderBy(m => m.IMS_END_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_HEAD_DESC":
                                lstTechnologyDetails = lstTechnologyDetails.OrderBy(m => m.MAST_HEAD_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_TECH_NAME":
                                lstTechnologyDetails = lstTechnologyDetails.OrderBy(m => m.MAST_TECH_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_TECH_COST":
                                lstTechnologyDetails = lstTechnologyDetails.OrderBy(m => m.IMS_TECH_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_LAYER_COST":
                                lstTechnologyDetails = lstTechnologyDetails.OrderBy(m => m.IMS_LAYER_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstTechnologyDetails = lstTechnologyDetails.OrderBy(m => m.IMS_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "IMS_SEGMENT_NO":
                                lstTechnologyDetails = lstTechnologyDetails.OrderByDescending(m => m.IMS_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "O_CDWORKS_COST":
                                lstTechnologyDetails = lstTechnologyDetails.OrderByDescending(m => m.IMS_START_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_END_CHAINAGE":
                                lstTechnologyDetails = lstTechnologyDetails.OrderByDescending(m => m.IMS_END_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_HEAD_DESC":
                                lstTechnologyDetails = lstTechnologyDetails.OrderByDescending(m => m.MAST_HEAD_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_TECH_NAME":
                                lstTechnologyDetails = lstTechnologyDetails.OrderByDescending(m => m.MAST_TECH_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_TECH_COST":
                                lstTechnologyDetails = lstTechnologyDetails.OrderByDescending(m => m.IMS_TECH_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_LAYER_COST":
                                lstTechnologyDetails = lstTechnologyDetails.OrderByDescending(m => m.IMS_LAYER_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstTechnologyDetails = lstTechnologyDetails.OrderByDescending(m => m.IMS_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstTechnologyDetails = lstTechnologyDetails.OrderBy(m => m.IMS_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstTechnologyDetails.Select(m => new
                {
                    m.IMS_PR_ROAD_CODE,
                    m.IMS_SEGMENT_NO,
                    m.IMS_END_CHAINAGE,
                    m.IMS_START_CHAINAGE,
                    m.MAST_HEAD_DESC,
                    m.MAST_TECH_NAME,
                    m.IMS_TECH_COST,
                    m.IMS_LAYER_COST,
                    m.MAST_TECH_TYPE,
                    m.MANE_PR_CONTRACT_CODE
                }).ToArray();

                return result.Select(m => new
                {
                    cell = new[] 
                    {
                        m.IMS_SEGMENT_NO == null?string.Empty:m.IMS_SEGMENT_NO.ToString(),
                        m.IMS_START_CHAINAGE == null?string.Empty:m.IMS_START_CHAINAGE.ToString(),
                        m.IMS_END_CHAINAGE == null?string.Empty:m.IMS_END_CHAINAGE.ToString(),
                        m.IMS_TECH_COST==null?string.Empty:m.IMS_TECH_COST.ToString(),
                        m.IMS_LAYER_COST==null?string.Empty:m.IMS_LAYER_COST.ToString(),
                        m.MAST_HEAD_DESC == null?string.Empty:m.MAST_HEAD_DESC.ToString(),
                        m.MAST_TECH_NAME == null?string.Empty:m.MAST_TECH_NAME.ToString(),
                        m.MAST_TECH_TYPE == "A" ? "IRC Agreegated" : "IRC Non Agreegated",
                        //m.IMS_SEGMENT_NO == maxTechCode?
                        "<a href='#' title='Click here to edit Technology Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditTechnologyDetails('" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+m.IMS_PR_ROAD_CODE.ToString().Trim() ,"SegmentCode = "+m.IMS_SEGMENT_NO.ToString()  ,"ContractCode = "+m.MANE_PR_CONTRACT_CODE.ToString()}) + "'); return false;'>Edit Technology Details</a>",
                        //m.IMS_SEGMENT_NO == maxTechCode?
                        "<a href='#' title='Click here to delete Technology Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteTechnologyDetails('" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+m.IMS_PR_ROAD_CODE.ToString().Trim() ,"SegmentCode = "+m.IMS_SEGMENT_NO.ToString()  ,"ContractCode = "+m.MANE_PR_CONTRACT_CODE.ToString()}) + "'); return false;'>Delete Technology Details</a>"
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

        /// <summary>
        /// returns the start chainage associated with the latest technology details
        /// </summary>
        /// <param name="proposalCode"></param>
        /// <returns></returns>
        public decimal? GetTechnologyStartChainage(int proposalCode, int techCode, int layerCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.MANE_IMS_TECH.Any(m => m.IMS_PR_ROAD_CODE == proposalCode))
                {
                    MANE_IMS_TECH techMaster = dbContext.MANE_IMS_TECH.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MAST_TECH_CODE == techCode && m.MAST_LAYER_CODE == layerCode).OrderByDescending(m => m.IMS_SEGMENT_NO).FirstOrDefault();
                    return techMaster.IMS_END_CHAINAGE;
                }
                else
                {
                    return null;
                }
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

        #endregion

        #region Periodic Maintenance [25-04-2017]

        public Array GetPeriodicCompletedRoadListDAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                if (packageID.Contains("All"))
                {
                    packageID = "All Packages";
                }

                var query = (from imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                             join execRoads in dbContext.EXEC_ROADS_MONTHLY_STATUS
                             on imsSanctionedProjectDetails.IMS_PR_ROAD_CODE equals execRoads.IMS_PR_ROAD_CODE
                             join blockDetails in dbContext.MASTER_BLOCK
                             on imsSanctionedProjectDetails.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                             join districtDetails in dbContext.MASTER_DISTRICT
                             on imsSanctionedProjectDetails.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                             join stateDetails in dbContext.MASTER_STATE
                             on imsSanctionedProjectDetails.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                             join adminDetails in dbContext.ADMIN_DEPARTMENT
                             on imsSanctionedProjectDetails.MAST_DPIU_CODE equals adminDetails.ADMIN_ND_CODE
                             join fundingAgency in dbContext.MASTER_FUNDING_AGENCY
                             on imsSanctionedProjectDetails.IMS_COLLABORATION equals fundingAgency.MAST_FUNDING_AGENCY_CODE into agencies
                             from fundingAgency in agencies.DefaultIfEmpty()
                             where
                             imsSanctionedProjectDetails.MAST_STATE_CODE == stateCode &&
                             imsSanctionedProjectDetails.MAST_DISTRICT_CODE == districtCode &&
                             imsSanctionedProjectDetails.MAST_DPIU_CODE == adminNDCode &&
                             imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
                             (imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "P" || imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "L") &&
                             execRoads.EXEC_ISCOMPLETED == "C" &&
                             (sanctionedYear == 0 ? 1 : imsSanctionedProjectDetails.IMS_YEAR) == (sanctionedYear == 0 ? 1 : sanctionedYear) &&
                             (blockCode == 0 ? 1 : imsSanctionedProjectDetails.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                             (packageID == "All Packages" ? "%" : imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper()) == (packageID == "All Packages" ? "%" : packageID.ToUpper()) &&
                                 //new filters added by Vikram 
                             (batch == 0 ? 1 : imsSanctionedProjectDetails.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
                             (collaboration <= 0 ? 1 : imsSanctionedProjectDetails.IMS_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                             (upgradationType == "0" ? "%" : imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT) == (upgradationType == "0" ? "%" : upgradationType) &&
                                 //end of change
                             imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //new change done by Vikram on 10 Feb 2014
                             && imsSanctionedProjectDetails.IMS_DPR_STATUS == "N" //new change done by Vikram
                             select new
                             {
                                 imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                                 imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                 imsSanctionedProjectDetails.IMS_ROAD_FROM,
                                 imsSanctionedProjectDetails.IMS_ROAD_TO,
                                 imsSanctionedProjectDetails.IMS_YEAR,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

                                 imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
                                 imsSanctionedProjectDetails.IMS_LOCK_STATUS,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED,
                                 imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                                 imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                                 imsSanctionedProjectDetails.IMS_PAV_LENGTH,
                                 fundingAgency.MAST_FUNDING_AGENCY_NAME,
                                 imsSanctionedProjectDetails.IMS_BATCH,
                                 imsSanctionedProjectDetails.MASTER_BLOCK.MAST_BLOCK_NAME

                             }).Union(from imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                                      join execRoads in dbContext.EXEC_LSB_MONTHLY_STATUS
                                      on imsSanctionedProjectDetails.IMS_PR_ROAD_CODE equals execRoads.IMS_PR_ROAD_CODE
                                      join blockDetails in dbContext.MASTER_BLOCK
                                      on imsSanctionedProjectDetails.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                                      join districtDetails in dbContext.MASTER_DISTRICT
                                      on imsSanctionedProjectDetails.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                                      join stateDetails in dbContext.MASTER_STATE
                                      on imsSanctionedProjectDetails.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                                      join adminDetails in dbContext.ADMIN_DEPARTMENT
                                      on imsSanctionedProjectDetails.MAST_DPIU_CODE equals adminDetails.ADMIN_ND_CODE
                                      join fundingAgency in dbContext.MASTER_FUNDING_AGENCY
                                      on imsSanctionedProjectDetails.IMS_COLLABORATION equals fundingAgency.MAST_FUNDING_AGENCY_CODE into agencies
                                      from fundingAgency in agencies.DefaultIfEmpty()
                                      where
                                      imsSanctionedProjectDetails.MAST_STATE_CODE == stateCode &&
                                      imsSanctionedProjectDetails.MAST_DISTRICT_CODE == districtCode &&
                                      imsSanctionedProjectDetails.MAST_DPIU_CODE == adminNDCode &&
                                      imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
                                      (imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "P" || imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "L") &&
                                      execRoads.EXEC_ISCOMPLETED == "C" &&
                                      (sanctionedYear == 0 ? 1 : imsSanctionedProjectDetails.IMS_YEAR) == (sanctionedYear == 0 ? 1 : sanctionedYear) &&
                                      (blockCode == 0 ? 1 : imsSanctionedProjectDetails.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                                      (packageID == "All Packages" ? "%" : imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper()) == (packageID == "All Packages" ? "%" : packageID.ToUpper()) &&
                                          //new filters added by Vikram 
                                      (batch == 0 ? 1 : imsSanctionedProjectDetails.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
                                      (collaboration <= 0 ? 1 : imsSanctionedProjectDetails.IMS_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                                      (upgradationType == "0" ? "%" : imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT) == (upgradationType == "0" ? "%" : upgradationType) &&
                                          //end of change
                                      imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //new change done by Vikram on 10 Feb 2014
                                      && imsSanctionedProjectDetails.IMS_DPR_STATUS == "N" //new change done by Vikram
                                      select new
                                      {
                                          imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                                          imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                          imsSanctionedProjectDetails.IMS_ROAD_FROM,
                                          imsSanctionedProjectDetails.IMS_ROAD_TO,
                                          imsSanctionedProjectDetails.IMS_YEAR,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

                                          imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

                                          imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
                                          imsSanctionedProjectDetails.IMS_LOCK_STATUS,
                                          imsSanctionedProjectDetails.IMS_SANCTIONED,
                                          imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                                          imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                                          imsSanctionedProjectDetails.IMS_PAV_LENGTH,
                                          fundingAgency.MAST_FUNDING_AGENCY_NAME,
                                          imsSanctionedProjectDetails.IMS_BATCH,
                                          imsSanctionedProjectDetails.MASTER_BLOCK.MAST_BLOCK_NAME

                                      });



                totalRecords = query == null ? 0 : query.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "SanctionedYear":
                                query = query.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "RoadLength":
                                query = query.OrderBy(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Collaboration":
                                query = query.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Package":
                                query = query.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }


                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "SanctionedYear":
                                query = query.OrderByDescending(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "RoadLength":
                                query = query.OrderByDescending(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Collaboration":
                                query = query.OrderByDescending(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Package":
                                query = query.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(imsSanctionedProjectDetails => new
                {
                    imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                    imsSanctionedProjectDetails.IMS_ROAD_NAME,
                    imsSanctionedProjectDetails.IMS_ROAD_FROM,
                    imsSanctionedProjectDetails.IMS_ROAD_TO,
                    imsSanctionedProjectDetails.IMS_YEAR,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

                    imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
                    imsSanctionedProjectDetails.IMS_LOCK_STATUS,
                    imsSanctionedProjectDetails.IMS_SANCTIONED,
                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                    imsSanctionedProjectDetails.IMS_PAV_LENGTH,
                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME,
                    imsSanctionedProjectDetails.IMS_BATCH,
                    imsSanctionedProjectDetails.MAST_BLOCK_NAME
                }).ToArray();


                return result.Select(imsSanctionedProjectDetails => new
                {

                    cell = new[] {                                            
                                    imsSanctionedProjectDetails.MAST_BLOCK_NAME == null ? "-" : imsSanctionedProjectDetails.MAST_BLOCK_NAME.ToString(),           
                                    imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString() ,
                                    imsSanctionedProjectDetails.IMS_BATCH == null ? "-" : ("Batch -" + imsSanctionedProjectDetails.IMS_BATCH).ToString(),
                                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,    
                                    imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                    imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString(),
                                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME==null?"NA":imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME.Trim(),                                                              
                                    //imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? 
                                    //(imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT+
                                    //imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT).ToString() : (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT).ToString(),    

                                    ///Change made by SAMMED PATIL on 29MAR2016 
                                    PMGSYSession.Current.PMGSYScheme == 1 ? 
                                                      ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT)).ToString()
                                                    : ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT)).ToString(),
                                    (imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5).ToString(),                                    
                                    /* ((dbContext.MANE_MAINTENANCE_DETAILS.Any(
                                                                              s=>s.IMS_PR_ROAD_CODE==imsSanctionedProjectDetails.IMS_PR_ROAD_CODE)==false))
                                                                             ? "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-plusthick  ui-align-center' title='Add First Periodic Maintenance details' onClick ='AddPeriodicMaintenanceDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID,"RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")})  + "\");'></span></td> </tr></table></center>"
                                                                             :(((dbContext.MANE_MAINTENANCE_DETAILS.Where(m=>m.IMS_PR_ROAD_CODE==imsSanctionedProjectDetails.IMS_PR_ROAD_CODE).AsEnumerable().LastOrDefault().MANE_MAIN_TYPE =="F")&& dbContext.MANE_MAINTENANCE_DETAILS.Where(d=>d.IMS_PR_ROAD_CODE ==imsSanctionedProjectDetails.IMS_PR_ROAD_CODE).AsEnumerable().LastOrDefault().MANE_RENEWAL_COMPLETION_DATE==null))
                                                                             ?"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-pencil  ui-align-center' title='Edit First Maintenance details' onClick ='EditPeriodicMaintenanceDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID,"RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")})  + "\");'></span></td> </tr></table></center>"
                                                                             :((dbContext.MANE_MAINTENANCE_DETAILS.Where(m=>m.IMS_PR_ROAD_CODE==imsSanctionedProjectDetails.IMS_PR_ROAD_CODE).AsEnumerable().LastOrDefault().MANE_MAIN_TYPE =="F")&&(dbContext.MANE_MAINTENANCE_DETAILS.Where(d=>d.IMS_PR_ROAD_CODE ==imsSanctionedProjectDetails.IMS_PR_ROAD_CODE).FirstOrDefault().MANE_RENEWAL_COMPLETION_DATE!=null))
                                                                             ?"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-plusthick  ui-align-center' title='Add Second Periodic Maintenance details' onClick ='AddPeriodicMaintenanceDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID,"RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--"),"mainType=Y"})  + "\");'></span></td> </tr></table></center>" 
                                                                             :(((dbContext.MANE_MAINTENANCE_DETAILS.Where(m=>m.IMS_PR_ROAD_CODE==imsSanctionedProjectDetails.IMS_PR_ROAD_CODE).AsEnumerable().LastOrDefault().MANE_MAIN_TYPE =="S")&& dbContext.MANE_MAINTENANCE_DETAILS.Where(d=>d.IMS_PR_ROAD_CODE ==imsSanctionedProjectDetails.IMS_PR_ROAD_CODE).AsEnumerable().LastOrDefault().MANE_RENEWAL_COMPLETION_DATE==null))
                                                                             ? "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-pencil  ui-align-center' title='Edit Second Periodic Maintenance details' onClick ='EditPeriodicMaintenanceDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID,"RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--"),"mainType=Y"})  + "\");'></span></td> </tr></table></center>"
                                                                             :"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-locked  ui-align-center' title='' onClick =''></span></td> </tr></table></center>",*/
                                    //dbContext.MANE_MAINTENANCE_DETAILS.Where(s=>s.IMS_PR_ROAD_CODE==imsSanctionedProjectDetails.IMS_PR_ROAD_CODE).FirstOrDefault() != null
                                    //                                         ?"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-zoomin  ui-align-center' title='View Maintenance Agreement' onClick ='ViewPeriodicMaintenanceDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")}) + "\");'></span></td> </tr></table></center>"
                                    //                                         :"-",
                                    "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-zoomin  ui-align-center' title='View Maintenance Agreement' onClick ='ViewPeriodicMaintenanceDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")}) + "\");'></span></td> </tr></table></center>",
                                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetPeriodicCompletedRoadListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public List<SelectListItem> GetRenewalTypeList()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> RenewalTypeList = new SelectList(dbContext.MASTER_RENEWAL_TYPE, "MAST_RENEWAL_TYPE_CODE", "MAST_RENEWAL_TYPE_NAME").ToList<SelectListItem>();

                return RenewalTypeList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetRenewalTypeList()");
                return null;
            }

        }



        public List<SelectListItem> GetYearList(Int32 roadCode, Boolean all)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                DateTime? complDate = null;
                int ExecComplYear;
                List<SelectListItem> CompletionTypeList = new List<SelectListItem>();
                var complDateList = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(s => s.IMS_PR_ROAD_CODE == roadCode).Select(s => s.EXEC_COMPLETION_DATE).ToList();
                for (int i = 0; i < complDateList.Count; i++)
                {
                    if (complDateList[i] == null)
                    {
                        continue;
                    }
                    else
                    {
                        complDate = complDateList[i];
                        break;
                    }

                }

                if (complDate != null)
                {
                    String CompletionYear = Convert.ToDateTime(complDate).ToString("dd/MM/yyyy").Split('/')[2];
                    ExecComplYear = Convert.ToInt32(CompletionYear) + 5;

                    for (int i = ExecComplYear; i <= DateTime.Now.Year; i++)
                    {
                        CompletionTypeList.Add(new SelectListItem { Text = i + "-" + (i + 1), Value = i + "" });
                    }

                }

                if (all)
                {
                    CompletionTypeList.Insert(0, new SelectListItem { Text = "All Year", Value = "0", Selected = true });
                }
                else
                {
                    CompletionTypeList.Insert(0, new SelectListItem { Text = "Select Year", Value = "0", Selected = true });
                }
                return CompletionTypeList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetRenewalTypeList()");
                return null;
            }

        }

        public Boolean AddPeriodicMaintenanceDAL(AddPeriodicMaintenanceModel model, out String message)
        {
            PMGSYEntities dbContex = new PMGSYEntities();
            try
            {
                if (dbContex.MANE_MAINTENANCE_DETAILS.Any(s => s.IMS_PR_ROAD_CODE == model.ImdRoadCode))
                {
                    MANE_MAINTENANCE_DETAILS lastObj = dbContex.MANE_MAINTENANCE_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == model.ImdRoadCode).AsEnumerable().LastOrDefault();
                    if (lastObj.MANE_MAIN_YEAR > model.MANE_MAIN_YEAR)
                    {
                        message = "Periodic maintenance details should be later than the previous entry.";
                        return false;

                    }
                    else if (lastObj.MANE_MAIN_YEAR == model.MANE_MAIN_YEAR)
                    {
                        if (lastObj.MANE_MAIN_MONTH > model.MANE_MAIN_MONTH)
                        {
                            message = "Periodic maintenance details should be later than the previous entry.";
                            return false;
                        }
                        //else if (lastObj.MANE_MAIN_MONTH == model.MANE_MAIN_MONTH)
                        //{
                        //    message = "Periodic maintenance details for this period is already added.";
                        //    return false;
                        //}
                    }

                }
                if (model.MaintenanceCompleteDate != null)
                {
                    DateTime CompletionDate = Convert.ToDateTime(model.MaintenanceCompleteDate);

                    if (!(CompletionDate >= new DateTime(model.MANE_MAIN_YEAR, model.MANE_MAIN_MONTH, 1) && (CompletionDate <= DateTime.Today)))
                    {
                        message = "Maintenance completion date should be between selected period.";
                        return false;
                    }
                }

                MANE_MAINTENANCE_DETAILS maintenanceModel = new MANE_MAINTENANCE_DETAILS();
                maintenanceModel.MANE_MAIN_ID = dbContex.MANE_MAINTENANCE_DETAILS.Any() == false ? 1 : dbContex.MANE_MAINTENANCE_DETAILS.Max(m => m.MANE_MAIN_ID) + 1;
                maintenanceModel.IMS_PR_ROAD_CODE = model.ImdRoadCode;

                maintenanceModel.MANE_MAIN_YEAR = model.MANE_MAIN_YEAR;
                maintenanceModel.MANE_PROFILE_COST = model.ProfileCorrectionCost;
                maintenanceModel.MANE_RENEWAL_TYPE = model.RenewalType;
                maintenanceModel.MANE_TECH_CODE = model.Technology;
                maintenanceModel.MANE_PERIODIC_COST = model.MaintanenaceCost;
                maintenanceModel.MANE_OTHER_ITEM_COSE = model.OtherCost;
                maintenanceModel.MANE_TOTAL_COST = model.TotalMaintenanceCost;

                if (dbContex.MANE_MAINTENANCE_DETAILS.Any(s => s.IMS_PR_ROAD_CODE == model.ImdRoadCode))
                {
                    maintenanceModel.MANE_MAIN_TYPE = "S";
                    //maintenanceModel.MANE_RENEWAL_COMPLETION_DATE = Convert.ToDateTime(model.MaintenanceCompleteDate); // only for edit
                }
                else
                {
                    maintenanceModel.MANE_MAIN_TYPE = "F";

                    // maintenanceModel.MANE_RENEWAL_COMPLETION_DATE = Convert.ToDateTime(model.MaintenanceCompleteDate); // only for edit
                }

                maintenanceModel.MANE_IS_PER_INCENTIVE = model.IsPerformaceIncentive;
                maintenanceModel.MANE_PER_INCENTIVE_YEAR = model.IsPerformaceIncentive == "Y" ? model.PerformanceIntensiveYear : 0;
                if (model.Iscompleted == "Y")
                    maintenanceModel.MANE_RENEWAL_COMPLETION_DATE = Convert.ToDateTime(model.MaintenanceCompleteDate);

                /*
                maintenanceModel.MANE_YEAR1_COST = model.MANE_YEAR1_AMOUNT;
                maintenanceModel.MANE_YEAR2_COST = model.MANE_YEAR2_AMOUNT;
                maintenanceModel.MANE_YEAR3_COST = model.MANE_YEAR3_AMOUNT;
                maintenanceModel.MANE_YEAR4_COST = model.MANE_YEAR4_AMOUNT;
                maintenanceModel.MANE_YEAR5_COST = model.MANE_YEAR5_AMOUNT;
                maintenanceModel.MANE_TOTAL_YEAR_COST = model.MANE_TOTAL_AMOUNT;
                 */
                maintenanceModel.MANE_MAIN_MONTH = model.MANE_MAIN_MONTH;
                maintenanceModel.MANE_START_CHAINAGE = model.StartChainage;
                maintenanceModel.MANE_END_CHAINAGE = model.EndChainage;

                maintenanceModel.USERID = PMGSYSession.Current.UserId;
                maintenanceModel.IPADD = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',').Last().Trim();
                dbContex.MANE_MAINTENANCE_DETAILS.Add(maintenanceModel);
                dbContex.SaveChanges();
                message = "Maintenance details added successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddPeriodicMaintenanceDAL()");
                message = "Error occured while processing your request.";
                return false;
            }

        }

        public AddPeriodicMaintenanceModel GetPeriodicMentainanceModelDAL(Int32 imsRoadCode)
        {
            PMGSYEntities dbContex = new PMGSYEntities();
            try
            {
                MANE_MAINTENANCE_DETAILS maneModel = dbContex.MANE_MAINTENANCE_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == imsRoadCode).AsEnumerable().LastOrDefault();



                AddPeriodicMaintenanceModel model = new AddPeriodicMaintenanceModel();
                model.ImdRoadCode = maneModel.IMS_PR_ROAD_CODE;
                model.MANE_MAIN_YEAR = maneModel.MANE_MAIN_YEAR;
                model.ProfileCorrectionCost = Convert.ToDecimal(maneModel.MANE_PROFILE_COST.ToString("0.00"));
                model.RenewalType = maneModel.MANE_RENEWAL_TYPE;
                model.Technology = maneModel.MANE_TECH_CODE;
                model.MaintanenaceCost = Convert.ToDecimal(maneModel.MANE_PERIODIC_COST.ToString("0.00"));
                model.OtherCost = Convert.ToDecimal(maneModel.MANE_OTHER_ITEM_COSE.ToString("0.00"));
                model.TotalMaintenanceCost = Convert.ToDecimal(maneModel.MANE_TOTAL_COST.ToString("0.00"));
                //if (maneModel.MANE_RENEWAL_COMPLETION_DATE !=null)
                // model.MaintenanceCompleteDate = Convert.ToDateTime(maneModel.MANE_RENEWAL_COMPLETION_DATE).ToString("dd/MM/yyyy");
                model.IsPerformaceIncentive = maneModel.MANE_IS_PER_INCENTIVE;
                model.PerformanceIntensiveYear = maneModel.MANE_PER_INCENTIVE_YEAR ?? 0;

                model.IsSecondPeriodic = maneModel.MANE_MAIN_TYPE == "F" ? "N" : "Y";
                model.MANE_MAIN_MONTH = maneModel.MANE_MAIN_MONTH ?? 0;
                model.StartChainage = maneModel.MANE_START_CHAINAGE ?? 0;
                model.EndChainage = maneModel.MANE_END_CHAINAGE ?? 0;

                model.Iscompleted = maneModel.MANE_RENEWAL_COMPLETION_DATE == null ? "N" : "Y";  // for completion date calender
                if (model.Iscompleted == "Y")
                    model.MaintenanceCompleteDate = Convert.ToDateTime(maneModel.MANE_RENEWAL_COMPLETION_DATE).ToString("dd/MM/yyyy");
                //model.MANE_YEAR1_AMOUNT = Convert.ToDecimal(maneModel.MANE_YEAR1_COST.ToString("0.00"));
                //model.MANE_YEAR2_AMOUNT = Convert.ToDecimal(maneModel.MANE_YEAR2_COST.ToString("0.00"));
                //model.MANE_YEAR3_AMOUNT = Convert.ToDecimal(maneModel.MANE_YEAR3_COST.ToString("0.00"));
                //model.MANE_YEAR4_AMOUNT = Convert.ToDecimal(maneModel.MANE_YEAR4_COST.ToString("0.00"));
                //model.MANE_YEAR5_AMOUNT = Convert.ToDecimal(maneModel.MANE_YEAR5_COST.ToString("0.00"));
                //model.MANE_TOTAL_AMOUNT = Convert.ToDecimal(maneModel.MANE_TOTAL_YEAR_COST.ToString("0.00"));
                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetPeriodicMentainanceModelDAL()");
                return null;
            }

        }

        //public Boolean EditPeriodicMaintenanceDAL(AddPeriodicMaintenanceModel model, out String message)   //old
        //{
        //    PMGSYEntities dbContex = new PMGSYEntities();
        //    try
        //    {
        //        MANE_MAINTENANCE_DETAILS lastObj = dbContex.MANE_MAINTENANCE_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == model.ImdRoadCode).AsEnumerable().LastOrDefault();

        //        dbContex.MANE_MAINTENANCE_DETAILS.Remove(lastObj);
        //        dbContex.SaveChanges();


        //        if (dbContex.MANE_MAINTENANCE_DETAILS.Any(s => s.IMS_PR_ROAD_CODE == model.ImdRoadCode))
        //        {
        //            MANE_MAINTENANCE_DETAILS lastObjnew = dbContex.MANE_MAINTENANCE_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == model.ImdRoadCode).AsEnumerable().LastOrDefault();
        //            if (lastObjnew.MANE_MAIN_YEAR > lastObj.MANE_MAIN_YEAR)
        //            {
        //                message = "Periodic maintenance details should be later than the previous entry.";
        //                return false;

        //            }
        //            else if (lastObjnew.MANE_MAIN_YEAR == lastObj.MANE_MAIN_YEAR)
        //            {
        //                if (lastObjnew.MANE_MAIN_MONTH > lastObj.MANE_MAIN_MONTH)
        //                {
        //                    message = "Periodic maintenance details should be later than the previous entry.";
        //                    return false;
        //                }
        //                else if (lastObjnew.MANE_MAIN_MONTH == lastObj.MANE_MAIN_MONTH)
        //                {
        //                    message = "Periodic maintenance details for this period is already added.";
        //                    return false;
        //                }
        //            }

        //        }
        //        if (model.MaintenanceCompleteDate != null)
        //        {
        //            DateTime CompletionDate = Convert.ToDateTime(model.MaintenanceCompleteDate);

        //            if (!(CompletionDate >= new DateTime(model.MANE_MAIN_YEAR, model.MANE_MAIN_MONTH, 1) && (CompletionDate <= DateTime.Today)))
        //            {
        //                message = "Maintenance completion date should be between selected period.";
        //                return false;
        //            }
        //        }

        //        MANE_MAINTENANCE_DETAILS maintenanceModel = new MANE_MAINTENANCE_DETAILS();
        //        maintenanceModel.MANE_MAIN_ID = dbContex.MANE_MAINTENANCE_DETAILS.Any() == false ? 1 : dbContex.MANE_MAINTENANCE_DETAILS.Max(m => m.MANE_MAIN_ID) + 1;
        //        maintenanceModel.IMS_PR_ROAD_CODE = model.ImdRoadCode;

        //        maintenanceModel.MANE_MAIN_YEAR = model.MANE_MAIN_YEAR;
        //        maintenanceModel.MANE_PROFILE_COST = model.ProfileCorrectionCost;
        //        maintenanceModel.MANE_RENEWAL_TYPE = model.RenewalType;
        //        maintenanceModel.MANE_TECH_CODE = model.Technology;
        //        maintenanceModel.MANE_PERIODIC_COST = model.MaintanenaceCost;
        //        maintenanceModel.MANE_OTHER_ITEM_COSE = model.OtherCost;
        //        maintenanceModel.MANE_TOTAL_COST = model.TotalMaintenanceCost;

        //        if (dbContex.MANE_MAINTENANCE_DETAILS.Any(s => s.IMS_PR_ROAD_CODE == model.ImdRoadCode))
        //        {
        //            maintenanceModel.MANE_MAIN_TYPE = "S";
        //            //maintenanceModel.MANE_RENEWAL_COMPLETION_DATE = Convert.ToDateTime(model.MaintenanceCompleteDate); // only for edit
        //        }
        //        else
        //        {
        //            maintenanceModel.MANE_MAIN_TYPE = "F";

        //            // maintenanceModel.MANE_RENEWAL_COMPLETION_DATE = Convert.ToDateTime(model.MaintenanceCompleteDate); // only for edit
        //        }

        //        maintenanceModel.MANE_IS_PER_INCENTIVE = model.IsPerformaceIncentive;
        //        maintenanceModel.MANE_PER_INCENTIVE_YEAR = model.IsPerformaceIncentive == "Y" ? model.PerformanceIntensiveYear : 0;
        //        if (model.Iscompleted == "Y")
        //            maintenanceModel.MANE_RENEWAL_COMPLETION_DATE = Convert.ToDateTime(model.MaintenanceCompleteDate);

        //        /*
        //        maintenanceModel.MANE_YEAR1_COST = model.MANE_YEAR1_AMOUNT;
        //        maintenanceModel.MANE_YEAR2_COST = model.MANE_YEAR2_AMOUNT;
        //        maintenanceModel.MANE_YEAR3_COST = model.MANE_YEAR3_AMOUNT;
        //        maintenanceModel.MANE_YEAR4_COST = model.MANE_YEAR4_AMOUNT;
        //        maintenanceModel.MANE_YEAR5_COST = model.MANE_YEAR5_AMOUNT;
        //        maintenanceModel.MANE_TOTAL_YEAR_COST = model.MANE_TOTAL_AMOUNT;
        //         */
        //        maintenanceModel.MANE_MAIN_MONTH = model.MANE_MAIN_MONTH;
        //        maintenanceModel.MANE_START_CHAINAGE = model.StartChainage;
        //        maintenanceModel.MANE_END_CHAINAGE = model.EndChainage;

        //        maintenanceModel.USERID = PMGSYSession.Current.UserId;
        //        maintenanceModel.IPADD = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',').Last().Trim();
        //        dbContex.MANE_MAINTENANCE_DETAILS.Add(maintenanceModel);
        //        dbContex.SaveChanges();

        //        //lastObj.IMS_PR_ROAD_CODE = model.ImdRoadCode;

        //        //lastObj.MANE_MAIN_YEAR = model.MANE_MAIN_YEAR;
        //        //lastObj.MANE_PROFILE_COST = model.ProfileCorrectionCost;
        //        //lastObj.MANE_RENEWAL_TYPE = model.RenewalType;
        //        //lastObj.MANE_TECH_CODE = model.Technology;
        //        //lastObj.MANE_PERIODIC_COST = model.MaintanenaceCost;
        //        //lastObj.MANE_OTHER_ITEM_COSE = model.OtherCost;
        //        //lastObj.MANE_TOTAL_COST = model.TotalMaintenanceCost;
        //        ////maintenanceModel.MANE_MAIN_TYPE = maintenanceModel.MANE_MAIN_TYPE=="F"?"S":"S";
        //        //if(model.Iscompleted=="Y")
        //        //   lastObj.MANE_RENEWAL_COMPLETION_DATE = Convert.ToDateTime(model.MaintenanceCompleteDate);// only for edit
        //        //else
        //        //    lastObj.MANE_RENEWAL_COMPLETION_DATE = null;  //if not "Yes then NULL"

        //        //lastObj.MANE_IS_PER_INCENTIVE = model.IsPerformaceIncentive;
        //        //lastObj.MANE_PER_INCENTIVE_YEAR = model.IsPerformaceIncentive == "Y" ? model.PerformanceIntensiveYear : 0;

        //        ///* lastObj.MANE_YEAR1_COST = model.MANE_YEAR1_AMOUNT;
        //        // lastObj.MANE_YEAR2_COST = model.MANE_YEAR2_AMOUNT;
        //        // lastObj.MANE_YEAR3_COST = model.MANE_YEAR3_AMOUNT;
        //        // lastObj.MANE_YEAR4_COST = model.MANE_YEAR4_AMOUNT;
        //        // lastObj.MANE_YEAR5_COST = model.MANE_YEAR5_AMOUNT;
        //        // lastObj.MANE_TOTAL_YEAR_COST = model.MANE_TOTAL_AMOUNT;
        //        // */
        //        //lastObj.MANE_MAIN_MONTH = model.MANE_MAIN_MONTH;
        //        //lastObj.MANE_START_CHAINAGE = model.StartChainage;
        //        //lastObj.MANE_END_CHAINAGE = model.EndChainage;

        //        //lastObj.USERID = PMGSYSession.Current.UserId;
        //        //lastObj.IPADD = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',').Last().Trim();
        //        //dbContex.Entry(lastObj).State = System.Data.Entity.EntityState.Modified;
        //        //dbContex.SaveChanges();
        //        message = "Maintenance details updated successfully.";
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "AddPeriodicMaintenanceDAL()");
        //        message = "Error occured while processing your request.";
        //        return false;
        //    }

        //}

        public Boolean EditPeriodicMaintenanceDAL(AddPeriodicMaintenanceModel model, out String message)
        {
            PMGSYEntities dbContex = new PMGSYEntities();
            try
            {
                MANE_MAINTENANCE_DETAILS lastObj = dbContex.MANE_MAINTENANCE_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == model.ImdRoadCode).AsEnumerable().LastOrDefault();

                //if (dbContex.MANE_MAINTENANCE_DETAILS.Any(s => s.IMS_PR_ROAD_CODE == model.ImdRoadCode))
                //{
                //    // MANE_MAINTENANCE_DETAILS lastObj = dbContex.MANE_MAINTENANCE_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == model.ImdRoadCode).AsEnumerable().LastOrDefault();
                //    if (lastObj.MANE_MAIN_YEAR > model.MANE_MAIN_YEAR)
                //    {
                //        message = "Periodic maintenance details should be later than the previous entry.";
                //        return false;

                //    }
                //    else if (lastObj.MANE_MAIN_YEAR == model.MANE_MAIN_YEAR)
                //    {
                //        if (lastObj.MANE_MAIN_MONTH > model.MANE_MAIN_MONTH)
                //        {
                //            message = "Periodic maintenance details should be later than the previous entry.";
                //            return false;
                //        }
                //        else if (lastObj.MANE_MAIN_MONTH == model.MANE_MAIN_MONTH)
                //        {
                //            message = "Periodic maintenance details for this period is already added.";
                //            return false;
                //        }
                //    }

                //}

                lastObj.IMS_PR_ROAD_CODE = model.ImdRoadCode;

                //lastObj.MANE_MAIN_YEAR = model.MANE_MAIN_YEAR;
                //lastObj.MANE_MAIN_MONTH = model.MANE_MAIN_MONTH;
                lastObj.MANE_PROFILE_COST = model.ProfileCorrectionCost;
                lastObj.MANE_RENEWAL_TYPE = model.RenewalType;
                lastObj.MANE_TECH_CODE = model.Technology;
                lastObj.MANE_PERIODIC_COST = model.MaintanenaceCost;
                lastObj.MANE_OTHER_ITEM_COSE = model.OtherCost;
                lastObj.MANE_TOTAL_COST = model.TotalMaintenanceCost;
                //maintenanceModel.MANE_MAIN_TYPE = maintenanceModel.MANE_MAIN_TYPE=="F"?"S":"S";
                if (model.Iscompleted == "Y")
                    lastObj.MANE_RENEWAL_COMPLETION_DATE = Convert.ToDateTime(model.MaintenanceCompleteDate);// only for edit
                else
                    lastObj.MANE_RENEWAL_COMPLETION_DATE = null;  //if not "Yes then NULL"

                lastObj.MANE_IS_PER_INCENTIVE = model.IsPerformaceIncentive;
                lastObj.MANE_PER_INCENTIVE_YEAR = model.IsPerformaceIncentive == "Y" ? model.PerformanceIntensiveYear : 0;

                /* lastObj.MANE_YEAR1_COST = model.MANE_YEAR1_AMOUNT;
                 lastObj.MANE_YEAR2_COST = model.MANE_YEAR2_AMOUNT;
                 lastObj.MANE_YEAR3_COST = model.MANE_YEAR3_AMOUNT;
                 lastObj.MANE_YEAR4_COST = model.MANE_YEAR4_AMOUNT;
                 lastObj.MANE_YEAR5_COST = model.MANE_YEAR5_AMOUNT;
                 lastObj.MANE_TOTAL_YEAR_COST = model.MANE_TOTAL_AMOUNT;
                 */

                lastObj.MANE_START_CHAINAGE = model.StartChainage;
                lastObj.MANE_END_CHAINAGE = model.EndChainage;

                lastObj.USERID = PMGSYSession.Current.UserId;
                lastObj.IPADD = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',').Last().Trim();
                dbContex.Entry(lastObj).State = System.Data.Entity.EntityState.Modified;
                dbContex.SaveChanges();
                message = "Maintenance details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddPeriodicMaintenanceDAL()");
                message = "Error occured while processing your request.";
                return false;
            }

        }

        public List<AddPeriodicMaintenanceModel> GetPariodicMaintenceViewListDAL(Int32 imsRoadCode, double RoadLength)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<AddPeriodicMaintenanceModel> lstPeriodicMainLst = new List<AddPeriodicMaintenanceModel>();
                List<MANE_MAINTENANCE_DETAILS> lstMaintenance = dbContext.MANE_MAINTENANCE_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == imsRoadCode).ToList();

                foreach (var maneModel in lstMaintenance)
                {
                    AddPeriodicMaintenanceModel model = new AddPeriodicMaintenanceModel();
                    model.MANE_MAIN_YEAR = maneModel.MANE_MAIN_YEAR;
                    model.Length = Convert.ToDecimal(RoadLength);
                    model.ProfileCorrectionCost = Convert.ToDecimal(maneModel.MANE_PROFILE_COST.ToString("0.00"));
                    model.RenewalType = maneModel.MANE_RENEWAL_TYPE;
                    model.RenewalName = dbContext.MASTER_RENEWAL_TYPE.Where(s => s.MAST_RENEWAL_TYPE_CODE == maneModel.MANE_RENEWAL_TYPE).SingleOrDefault().MAST_RENEWAL_TYPE_NAME;
                    model.Technology = maneModel.MANE_TECH_CODE;
                    model.technologyName = dbContext.MASTER_TECHNOLOGY.Where(s => s.MAST_TECH_CODE == maneModel.MANE_TECH_CODE).FirstOrDefault().MAST_TECH_NAME;
                    model.MaintanenaceCost = Convert.ToDecimal(maneModel.MANE_PERIODIC_COST.ToString("0.00"));
                    model.OtherCost = Convert.ToDecimal(maneModel.MANE_OTHER_ITEM_COSE.ToString("0.00"));
                    model.TotalMaintenanceCost = Convert.ToDecimal(maneModel.MANE_TOTAL_COST.ToString("0.00"));
                    //if (maneModel.MANE_RENEWAL_COMPLETION_DATE !=null)
                    model.MaintenanceCompleteDate = Convert.ToDateTime(maneModel.MANE_RENEWAL_COMPLETION_DATE).ToString("dd/MM/yyyy");
                    model.IsPerformaceIncentive = maneModel.MANE_IS_PER_INCENTIVE;
                    model.PerformanceIntensiveYear = maneModel.MANE_PER_INCENTIVE_YEAR ?? 0;
                    model.IsSecondPeriodic = maneModel.MANE_MAIN_TYPE == "F" ? "N" : "Y";

                    model.StartChainage = Convert.ToDecimal(maneModel.MANE_START_CHAINAGE);
                    model.EndChainage = Convert.ToDecimal(maneModel.MANE_END_CHAINAGE);
                    model.MANE_MAIN_MONTH = (Int32)maneModel.MANE_MAIN_MONTH;
                    String month = maneModel.MANE_MAIN_MONTH.ToString();
                    model.MonthName = getMonthList().Where(s => s.Value == month).FirstOrDefault().Text;
                    //model.MANE_YEAR1_AMOUNT = Convert.ToDecimal(maneModel.MANE_YEAR1_COST.ToString("0.00"));
                    //model.MANE_YEAR2_AMOUNT = Convert.ToDecimal(maneModel.MANE_YEAR2_COST.ToString("0.00"));
                    //model.MANE_YEAR3_AMOUNT = Convert.ToDecimal(maneModel.MANE_YEAR3_COST.ToString("0.00"));
                    //model.MANE_YEAR4_AMOUNT = Convert.ToDecimal(maneModel.MANE_YEAR4_COST.ToString("0.00"));
                    //model.MANE_YEAR5_AMOUNT = Convert.ToDecimal(maneModel.MANE_YEAR5_COST.ToString("0.00"));
                    //model.MANE_TOTAL_AMOUNT = Convert.ToDecimal(maneModel.MANE_TOTAL_YEAR_COST.ToString("0.00"));

                    lstPeriodicMainLst.Add(model);
                }
                return lstPeriodicMainLst;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetPariodicMaintenceViewListDAL()");
                return null;
            }
        }

        public Tuple<String, String, String, String> GetCurrentRoadInfo(Int32 imsRoadCode)
        {
            try
            {
                var dbContext = new PMGSYEntities();
                IMS_SANCTIONED_PROJECTS imsModel = dbContext.IMS_SANCTIONED_PROJECTS.Where(s => s.IMS_PR_ROAD_CODE == imsRoadCode).FirstOrDefault();

                string CompletionDate = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(s => s.IMS_PR_ROAD_CODE == imsRoadCode).FirstOrDefault().EXEC_COMPLETION_DATE == null ? "-" : Convert.ToDateTime(dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(s => s.IMS_PR_ROAD_CODE == imsRoadCode).FirstOrDefault().EXEC_COMPLETION_DATE).ToString("dd/MM/yyyy");

                Tuple<String, String, String, String> CurrentImsTuple = Tuple.Create(imsModel.IMS_YEAR + "-" + (imsModel.IMS_YEAR + 1), imsModel.IMS_PACKAGE_ID, imsModel.IMS_ROAD_NAME, CompletionDate);
                return CurrentImsTuple;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCurrentRoadInfo()");
                return null;
            }
        }

        public List<SelectListItem> getMonthList()
        {
            List<SelectListItem> monthList = new List<SelectListItem>();
            monthList.Add(new SelectListItem { Text = "January", Value = "1" });
            monthList.Add(new SelectListItem { Text = "February", Value = "2" });
            monthList.Add(new SelectListItem { Text = "March", Value = "3" });
            monthList.Add(new SelectListItem { Text = "April", Value = "4" });
            monthList.Add(new SelectListItem { Text = "May", Value = "5" });
            monthList.Add(new SelectListItem { Text = "June", Value = "6" });
            monthList.Add(new SelectListItem { Text = "July", Value = "7" });
            monthList.Add(new SelectListItem { Text = "August", Value = "8" });
            monthList.Add(new SelectListItem { Text = "September", Value = "9" });
            monthList.Add(new SelectListItem { Text = "October", Value = "10" });
            monthList.Add(new SelectListItem { Text = "November", Value = "11" });
            monthList.Add(new SelectListItem { Text = "December", Value = "12" });
            monthList.Insert(0, new SelectListItem { Text = "Select month", Value = "0", Selected = true });
            return monthList;
        }

        public Array ViewPeriodicmaintenanceListDAL(int RodeCode, int page, int rows, string sidx, string sord, out long totalRecords, decimal RoadLength, out String AddButton)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<MANE_MAINTENANCE_DETAILS> maintenanceList = dbContext.MANE_MAINTENANCE_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RodeCode).ToList();
                totalRecords = maintenanceList.Count;
                MANE_MAINTENANCE_DETAILS lastObj = maintenanceList.LastOrDefault();

                if (sidx.Trim() != String.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sord)
                        {

                            case "Year":
                                maintenanceList = maintenanceList.OrderBy(s => s.MANE_MAIN_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Month":
                                maintenanceList = maintenanceList.OrderBy(s => s.MANE_MAIN_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                maintenanceList = maintenanceList.OrderBy(s => s.MANE_MAIN_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                    else
                    {
                        switch (sord)
                        {

                            case "Year":
                                maintenanceList = maintenanceList.OrderByDescending(s => s.MANE_MAIN_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Month":
                                maintenanceList = maintenanceList.OrderByDescending(s => s.MANE_MAIN_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                maintenanceList = maintenanceList.OrderByDescending(s => s.MANE_MAIN_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }

                    }

                }

                AddButton = "<input type='button' style='margin-left:40px' id='btnSubmit' title='Add periodic maintenance' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'AddPeriodicMaintenanceDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode=" + RodeCode, "RoadLength=" + dbContext.IMS_SANCTIONED_PROJECTS.Where(s => s.IMS_PR_ROAD_CODE == RodeCode).FirstOrDefault().IMS_PAV_LENGTH.ToString() }) + "\");return false;' value='Add Periodic Maintenance'/>";

                return maintenanceList.Select(item => new
                {
                    cell = new[] {
                    
                      item.MANE_MAIN_YEAR==0?"-":item.MANE_MAIN_YEAR +"-"+(item.MANE_MAIN_YEAR+1),
                     getMonthList().Where(s=>s.Value== item.MANE_MAIN_MONTH+"").FirstOrDefault().Text,
                     item.MANE_IS_PER_INCENTIVE=="Y"?"Yes":"No",
                     item.MANE_PER_INCENTIVE_YEAR==0?"-":item.MANE_PER_INCENTIVE_YEAR+"-"+(item.MANE_PER_INCENTIVE_YEAR+1),
                     RoadLength.ToString("0.000"),
                     //dbContext.MASTER_TECHNOLOGY.Where(s=>s.MAST_TECH_CODE==item.MANE_TECH_CODE).FirstOrDefault().MAST_TECH_NAME,
                     (item.MANE_TECH_CODE > 0) ? dbContext.MASTER_TECHNOLOGY.Where(s=>s.MAST_TECH_CODE==item.MANE_TECH_CODE).FirstOrDefault().MAST_TECH_NAME : "-",
                     item.MANE_START_CHAINAGE.ToString(),
                     item.MANE_END_CHAINAGE.ToString(),
                     item.MASTER_RENEWAL_TYPE.MAST_RENEWAL_TYPE_NAME,
                     item.MANE_PROFILE_COST.ToString("0.00"),
                     item.MANE_PERIODIC_COST.ToString("0.00"), //maintenance cost
                     item.MANE_OTHER_ITEM_COSE.ToString("0.00"),
                     item.MANE_TOTAL_COST.ToString("0.00"),
                     item.MANE_RENEWAL_COMPLETION_DATE==null?"-":Convert.ToDateTime(item.MANE_RENEWAL_COMPLETION_DATE).ToString("dd/MM/yyyy"),
                     item.Equals(lastObj)?"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-pencil  ui-align-center' title='Edit Maintenance details' onClick ='EditPeriodicMaintenanceDetails(\"" + URLEncrypt.EncryptParameters1(new string[] {"IMSPRRoadCode="+item.IMS_PR_ROAD_CODE, "RoadLength="+ dbContext.IMS_SANCTIONED_PROJECTS.Where(s=>s.IMS_PR_ROAD_CODE==item.IMS_PR_ROAD_CODE).FirstOrDefault().IMS_PAV_LENGTH.ToString()})  + "\");'></span></td> </tr></table></center>":"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-locked  ui-align-center' title='' onClick =''></span></td> </tr></table></center>",
                     item.Equals(lastObj)?"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-trash  ui-align-center' title='Delete Maintenance details' onClick ='DeletePeriodicMaintenanceDetails(\"" + URLEncrypt.EncryptParameters1(new string[] {"IMSPRRoadCode="+item.IMS_PR_ROAD_CODE, "ManeId="+ item.MANE_MAIN_ID })  + "\");'></span></td> </tr></table></center>":"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-locked  ui-align-center' title='' onClick =''></span></td> </tr></table></center>",
                     
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewPeriodicmaintenanceListDAL()");
                totalRecords = 0;
                AddButton = String.Empty;
                return null;
            }
        }

        public Boolean DeletePeriodicMantenanceDetails(int ImsRoadCode, int ManeId, out String message)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                MANE_MAINTENANCE_DETAILS obj = dbContext.MANE_MAINTENANCE_DETAILS.Where(s => s.MANE_MAIN_ID == ManeId && s.IMS_PR_ROAD_CODE == ImsRoadCode).FirstOrDefault();
                dbContext.MANE_MAINTENANCE_DETAILS.Remove(obj);
                dbContext.SaveChanges();
                message = "Periodic maintenance details deleted successfully";
                return true;
            }
            catch (Exception ex)
            {
                message = "Error Occured while processing your request";
                ErrorLog.LogError(ex, "DeletePeriodicMantenanceDetails();");
                return false;
            }
        }
        #endregion

        #region Renewal Agreement Added by SAMMED A. PATIL on 29JAN2018

        /// <summary>
        /// returns the list of proposals for adding the special agreements
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="blockCode"></param>
        /// <param name="sanctionedYear"></param>
        /// <param name="packageID"></param>
        /// <param name="adminNDCode"></param>
        /// <param name="batch"></param>
        /// <param name="collaboration"></param>
        /// <param name="upgradationType"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetCompletedRoadForRenewalAgreementsListDAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (packageID.Contains("All"))
                {
                    packageID = "All Packages";
                }
                var query = (from imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                             //join execRoads in dbContext.EXEC_ROADS_MONTHLY_STATUS
                             //on imsSanctionedProjectDetails.IMS_PR_ROAD_CODE equals execRoads.IMS_PR_ROAD_CODE
                             join maint in dbContext.MAINE_REPACKAGE_DETAILS
                             on imsSanctionedProjectDetails.IMS_PR_ROAD_CODE equals maint.IMS_PR_ROAD_CODE    //added by PP [suggested by Pankaj Sir] 
                             join blockDetails in dbContext.MASTER_BLOCK
                             on imsSanctionedProjectDetails.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                             join districtDetails in dbContext.MASTER_DISTRICT
                             on imsSanctionedProjectDetails.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                             join stateDetails in dbContext.MASTER_STATE
                             on imsSanctionedProjectDetails.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                             join adminDetails in dbContext.ADMIN_DEPARTMENT
                             on imsSanctionedProjectDetails.MAST_DPIU_CODE equals adminDetails.ADMIN_ND_CODE
                             join fundingAgency in dbContext.MASTER_FUNDING_AGENCY
                             on imsSanctionedProjectDetails.IMS_COLLABORATION equals fundingAgency.MAST_FUNDING_AGENCY_CODE into agencies
                             from fundingAgency in agencies.DefaultIfEmpty()
                             where
                             imsSanctionedProjectDetails.MAST_STATE_CODE == stateCode &&
                             imsSanctionedProjectDetails.MAST_DISTRICT_CODE == districtCode &&
                             imsSanctionedProjectDetails.MAST_DPIU_CODE == adminNDCode &&
                             imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
                             (imsSanctionedProjectDetails.IMS_ISCOMPLETED == "C" || imsSanctionedProjectDetails.IMS_ISCOMPLETED == "X") &&
                             imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "P" &&
                                 //(execRoads.EXEC_ISCOMPLETED == "C") &&
                                 //(SqlFunctions.DateDiff("day", execRoads.EXEC_COMPLETION_DATE, DateTime.Now) >= 1825) &&
                             (sanctionedYear == 0 ? 1 : imsSanctionedProjectDetails.IMS_YEAR) == (sanctionedYear == 0 ? 1 : sanctionedYear) &&
                             (blockCode == 0 ? 1 : imsSanctionedProjectDetails.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                             (packageID == "All Packages" ? "%" : imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper()) == (packageID == "All Packages" ? "%" : packageID.ToUpper()) &&
                             (batch == 0 ? 1 : imsSanctionedProjectDetails.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
                             (collaboration <= 0 ? 1 : imsSanctionedProjectDetails.IMS_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                             (upgradationType == "0" ? "%" : imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT) == (upgradationType == "0" ? "%" : upgradationType) &&
                             imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                             //&& imsSanctionedProjectDetails.IMS_DPR_STATUS == "N"
                             select new
                             {
                                 imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                                 imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                 imsSanctionedProjectDetails.IMS_ROAD_FROM,
                                 imsSanctionedProjectDetails.IMS_ROAD_TO,
                                 imsSanctionedProjectDetails.IMS_YEAR,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

                                 imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
                                 imsSanctionedProjectDetails.IMS_LOCK_STATUS,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED,
                                 imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                                 imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                                 imsSanctionedProjectDetails.IMS_PAV_LENGTH,
                                 fundingAgency.MAST_FUNDING_AGENCY_NAME,
                                 imsSanctionedProjectDetails.IMS_BATCH,
                                 imsSanctionedProjectDetails.MASTER_BLOCK.MAST_BLOCK_NAME,
                                 maint.IMS_MAINT_REPACKAGE_ID   //added by PP
                             }).Distinct();
                totalRecords = query == null ? 0 : query.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "SanctionedYear":
                                query = query.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "RoadLength":
                                query = query.OrderBy(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Collaboration":
                                query = query.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Package":
                                query = query.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "SanctionedYear":
                                query = query.OrderByDescending(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "RoadLength":
                                query = query.OrderByDescending(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Collaboration":
                                query = query.OrderByDescending(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Package":
                                query = query.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(imsSanctionedProjectDetails => new
                {
                    imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                    imsSanctionedProjectDetails.IMS_ROAD_NAME,
                    imsSanctionedProjectDetails.IMS_ROAD_FROM,
                    imsSanctionedProjectDetails.IMS_ROAD_TO,
                    imsSanctionedProjectDetails.IMS_YEAR,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

                    imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
                    imsSanctionedProjectDetails.IMS_LOCK_STATUS,
                    imsSanctionedProjectDetails.IMS_SANCTIONED,
                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                    imsSanctionedProjectDetails.IMS_PAV_LENGTH,
                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME,
                    imsSanctionedProjectDetails.IMS_BATCH,
                    imsSanctionedProjectDetails.MAST_BLOCK_NAME,
                    imsSanctionedProjectDetails.IMS_MAINT_REPACKAGE_ID //added by PP
                }).ToArray();

                return result.Select(imsSanctionedProjectDetails => new
                {
                    cell = new[] {                                            
                                    imsSanctionedProjectDetails.MAST_BLOCK_NAME == null ? "-" : imsSanctionedProjectDetails.MAST_BLOCK_NAME.ToString(),           
                                    imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString() ,
                                    imsSanctionedProjectDetails.IMS_BATCH == null ? "-" : ("Batch -" + imsSanctionedProjectDetails.IMS_BATCH).ToString(),
                                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,    
                                    //changed by PP [Suggested by Pankaj Sir (04-06-2018) ==new column IMS_MAINT_REPACKAGE_ID]
                                    imsSanctionedProjectDetails.IMS_MAINT_REPACKAGE_ID==null?"-":imsSanctionedProjectDetails.IMS_MAINT_REPACKAGE_ID,
                                    imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                    imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString(),
                                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME==null?"NA":imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME.Trim(),                                                              
                                    
                                    //imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? 
                                    //(imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT+
                                    //imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT).ToString() : (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT).ToString(),    

                                    ///Change made by SAMMED PATIL on 29MAR2016 
                                    PMGSYSession.Current.PMGSYScheme == 1 ? 
                                                      ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT)).ToString()
                                                    : ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT)).ToString(),

                                    (imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5).ToString(),                                    
                                   // "<a href='#' class='ui-icon ui-icon-zoomin ui-align-center' onclick='ViewRenewalMaintenanceAgreement(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + imsSanctionedProjectDetails.IMS_ROAD_NAME.ToString().Replace('/','_'),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID,"RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")}) + "\"); return false;'></a>" 
                                  //Changed by PP [suggested by Pankaj Sir] 
                                   "<a href='#' class='ui-icon ui-icon-zoomin ui-align-center' onclick='ViewRenewalMaintenanceAgreement(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" +                                                                                          imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),
                                       /*"IMSRoadName =" + imsSanctionedProjectDetails.IMS_ROAD_NAME.ToString().Replace('/','_'),*/
                                       "SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_MAINT_REPACKAGE_ID,"RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")}) + "\"); return false;'></a>"  //chaged by PP[suggested by Pankaj Sir]
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL.GetCompletedRoadForRenewalAgreementsListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// returns the list of special maintenance agreements
        /// </summary>
        /// <param name="IMSPRRoadCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetRenewalAgreementDetailsListDAL(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            //     dbContext.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                var query = from IMSContract in dbContext.MANE_IMS_CONTRACT
                            join workDetails in dbContext.IMS_PROPOSAL_WORK
                            on IMSContract.IMS_WORK_CODE equals workDetails.IMS_WORK_CODE into works
                            from workDetails in works.DefaultIfEmpty()
                            join contractorDetails in dbContext.MASTER_CONTRACTOR
                            on IMSContract.MAST_CON_ID equals contractorDetails.MAST_CON_ID
                            where
                            IMSContract.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                            IMSContract.MANE_AGREEMENT_TYPE == "T"
                            select new
                            {

                                IMSContract.IMS_PR_ROAD_CODE,
                                IMSContract.MANE_PR_CONTRACT_CODE,
                                MAST_CON_COMPANY_NAME = contractorDetails.MAST_CON_COMPANY_NAME + (contractorDetails.MAST_CON_PAN != "" ? " (" + contractorDetails.MAST_CON_PAN + ")" : ""),
                                IMSContract.MANE_AGREEMENT_NUMBER,
                                IMSContract.MANE_AGREEMENT_DATE,
                                IMSContract.MANE_MAINTENANCE_START_DATE,
                                IMSContract.MANE_YEAR1_AMOUNT,
                                IMSContract.MANE_YEAR2_AMOUNT,
                                IMSContract.MANE_YEAR3_AMOUNT,
                                IMSContract.MANE_YEAR4_AMOUNT,
                                IMSContract.MANE_YEAR5_AMOUNT,
                                IMSContract.MANE_CONTRACT_FINALIZED,
                                IMSContract.MANE_LOCK_STATUS,
                                IMSContract.MANE_CONTRACT_STATUS,
                                IMSContract.MANE_CONTRACT_NUMBER,
                                workDetails.IMS_WORK_DESC,
                                IMSContract.IMS_WORK_CODE,
                                IsLatest = true,
                                IMSContract.MANE_CONTRACT_ID

                            };

                totalRecords = query == null ? 0 : query.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Work":
                                query = query.OrderBy(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "MaintenanceDate":
                                query = query.OrderBy(x => x.MANE_MAINTENANCE_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderBy(x => x.MANE_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                query = query.OrderByDescending(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Work":
                                query = query.OrderByDescending(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "MaintenanceDate":
                                query = query.OrderByDescending(x => x.MANE_MAINTENANCE_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderByDescending(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderByDescending(x => x.MANE_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(IMSContract => new
                {
                    IMSContract.IMS_PR_ROAD_CODE,
                    IMSContract.MANE_PR_CONTRACT_CODE,
                    IMSContract.MAST_CON_COMPANY_NAME,
                    IMSContract.MANE_AGREEMENT_NUMBER,
                    IMSContract.MANE_AGREEMENT_DATE,
                    IMSContract.MANE_MAINTENANCE_START_DATE,
                    IMSContract.MANE_YEAR1_AMOUNT,
                    IMSContract.MANE_YEAR2_AMOUNT,
                    IMSContract.MANE_YEAR3_AMOUNT,
                    IMSContract.MANE_YEAR4_AMOUNT,
                    IMSContract.MANE_YEAR5_AMOUNT,
                    IMSContract.MANE_CONTRACT_FINALIZED,
                    IMSContract.MANE_LOCK_STATUS,
                    IMSContract.MANE_CONTRACT_STATUS,
                    IMSContract.MANE_CONTRACT_NUMBER,
                    IMSContract.IMS_WORK_DESC,
                    IMSContract.IMS_WORK_CODE,
                    IMSContract.MANE_CONTRACT_ID

                }).ToArray();


                return result.Select(IMSContract => new
                {
                    cell = new[] {     
                        
                            
                                       
                                    IMSContract.MANE_AGREEMENT_NUMBER.ToString(),
                                    IMSContract.IMS_WORK_DESC==null?"NA":IMSContract.IMS_WORK_DESC.ToString().Trim(),
                                    IMSContract.MAST_CON_COMPANY_NAME==null?"NA":IMSContract.MAST_CON_COMPANY_NAME.ToString().Trim(),                            
                                    Convert.ToDateTime(IMSContract.MANE_AGREEMENT_DATE).ToString("dd/MM/yyyy"),
                                    Convert.ToDateTime(IMSContract.MANE_MAINTENANCE_START_DATE).ToString("dd/MM/yyyy"),
                                    ((IMSContract.MANE_YEAR1_AMOUNT)/*+
                                       (IMSContract.MANE_YEAR2_AMOUNT)+
                                       (IMSContract.MANE_YEAR3_AMOUNT)+
                                       (IMSContract.MANE_YEAR4_AMOUNT)+
                                       (IMSContract.MANE_YEAR5_AMOUNT)*/
                                    ).ToString(),

                                    AgreementStatus[IMSContract.MANE_CONTRACT_STATUS].ToString(),


                                    (IMSContract.MANE_CONTRACT_STATUS == "I" || IMSContract.MANE_CONTRACT_FINALIZED=="N") ? string.Empty : (((CheckIsLatest(IMSContract.IMS_PR_ROAD_CODE, IMSContract.MANE_PR_CONTRACT_CODE, IMSContract.IMS_WORK_CODE, IMSContract.MANE_CONTRACT_NUMBER, IMSContract.MANE_CONTRACT_STATUS )==true) ?(IMSContract.MANE_CONTRACT_STATUS =="C" || IMSContract.MANE_CONTRACT_FINALIZED=="N") :(IMSContract.MANE_CONTRACT_STATUS =="C" || IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_FINALIZED=="N"))?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()})),
                                    (IMSContract.MANE_CONTRACT_STATUS == "I" || IMSContract.MANE_CONTRACT_FINALIZED=="N" || IMSContract.MANE_CONTRACT_STATUS == "C") ? string.Empty :((CheckIsLatest(IMSContract.IMS_PR_ROAD_CODE, IMSContract.MANE_PR_CONTRACT_CODE, IMSContract.IMS_WORK_CODE, IMSContract.MANE_CONTRACT_NUMBER, IMSContract.MANE_CONTRACT_STATUS)==true)?(IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_FINALIZED=="N" ): (IMSContract.MANE_CONTRACT_STATUS =="I" || IMSContract.MANE_CONTRACT_STATUS=="C" ) )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString() }),
                                    
                                    (IMSContract.MANE_CONTRACT_FINALIZED=="N" && IMSContract.MANE_LOCK_STATUS=="N" && IMSContract.MANE_CONTRACT_STATUS=="P" ) ?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='FinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID  }) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Finalized'></span></td></tr></table></center>",
                                 //   IMSContract.MANE_CONTRACT_STATUS=="C" ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='DeFinalized'></span></td></tr></table></center>" :(dbContext.ACC_BILL_DETAILS.Any(m=>m.IMS_PR_ROAD_CODE == IMSContract.IMS_PR_ROAD_CODE && m.IMS_AGREEMENT_CODE == IMSContract.MANE_PR_CONTRACT_CODE) || IMSContract.MANE_CONTRACT_FINALIZED == "N" || IMSContract.MANE_CONTRACT_STATUS == "I") ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='DeFinalized'></span></td></tr></table></center>" :"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='DeFinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() ,"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()}) + "\");' ></span></td></tr></table></center>",
                                   IMSContract.MANE_CONTRACT_STATUS=="C" ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='DeFinalized'></span></td></tr></table></center>" :(IMSContract.MANE_CONTRACT_FINALIZED == "N" || IMSContract.MANE_CONTRACT_STATUS == "I") ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='DeFinalized'></span></td></tr></table></center>" :"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='DeFinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() ,"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()}) + "\");' ></span></td></tr></table></center>",  
                                    
                                    URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()  }),
                                    (IMSContract.MANE_CONTRACT_FINALIZED=="Y"||IMSContract.MANE_LOCK_STATUS=="Y" || IMSContract.MANE_CONTRACT_STATUS=="C" || IMSContract.MANE_CONTRACT_STATUS=="I" )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()  }),
                                    (IMSContract.MANE_CONTRACT_FINALIZED=="Y"||IMSContract.MANE_LOCK_STATUS=="Y"|| IMSContract.MANE_CONTRACT_STATUS=="C" || IMSContract.MANE_CONTRACT_STATUS=="I")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString(),"ManeContractId="+IMSContract.MANE_CONTRACT_ID.ToString()})
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL.GetRenewalAgreementDetailsListDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                //          dbContext.Configuration.AutoDetectChangesEnabled = true;
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public string GetRoadName(int prRoadCode)
        {
            string roadName = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == prRoadCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                return roadName;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL.GetRoadName");
                return string.Empty;
            }
        }

        /// <summary>
        /// saves the details of maintenance agreement details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveRenewalAgreementDetailsDAL(RenewalMaintenanceViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string proposalType = string.Empty;
                EXEC_LSB_MONTHLY_STATUS lsbMonthlyStatus = null;
                MANE_IMS_CONTRACT agreementDetails = null;
                EXEC_ROADS_MONTHLY_STATUS roadMonthlyStatus = null;
                int IMSPRRoadCode = 0;
                int? maxContractorNumber = 1;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                CommonFunctions commonFunction = new CommonFunctions();

                encryptedParameters = model.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString().Trim());

                var IMSContractList = (from IMSContracts in dbContext.MANE_IMS_CONTRACT
                                       join IMSSanctioned in dbContext.IMS_SANCTIONED_PROJECTS
                                       on IMSContracts.IMS_PR_ROAD_CODE equals IMSSanctioned.IMS_PR_ROAD_CODE
                                       where
                                       IMSSanctioned.MAST_STATE_CODE == stateCode &&
                                       IMSSanctioned.MAST_DISTRICT_CODE == districtCode &&
                                       IMSContracts.MANE_AGREEMENT_NUMBER.ToUpper() == model.MANE_AGREEMENT_NUMBER.ToUpper()
                                       select new
                                       {

                                           IMSContracts.MANE_AGREEMENT_NUMBER,
                                           IMSContracts.MAST_CON_ID,
                                           IMSContracts.IMS_PR_ROAD_CODE,
                                           IMSSanctioned.MAST_STATE_CODE,
                                           IMSSanctioned.MAST_DISTRICT_CODE

                                       }).FirstOrDefault();
                /// Changed by SAMMED PATIL for LSB and Road Proposal  28/06/2016
                proposalType = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(x => x.IMS_PROPOSAL_TYPE).FirstOrDefault();
                if (proposalType == "P")
                {
                    roadMonthlyStatus = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(rms => rms.IMS_PR_ROAD_CODE == IMSPRRoadCode && rms.EXEC_ISCOMPLETED == "C").FirstOrDefault();

                    if (roadMonthlyStatus != null)
                    {
                        if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year > roadMonthlyStatus.EXEC_PROG_YEAR || commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year < roadMonthlyStatus.EXEC_PROG_YEAR)
                        {
                            message = "Construction completion date should be equal to physical progress completion date.";
                            return false;
                        }
                        else if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year == roadMonthlyStatus.EXEC_PROG_YEAR && (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Month > roadMonthlyStatus.EXEC_PROG_MONTH || commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Month < roadMonthlyStatus.EXEC_PROG_MONTH))
                        {
                            message = "Construction completion date should be equal to physical progress completion date.";
                            return false;
                        }
                    }
                }
                else
                {
                    lsbMonthlyStatus = dbContext.EXEC_LSB_MONTHLY_STATUS.Where(rms => rms.IMS_PR_ROAD_CODE == IMSPRRoadCode && rms.EXEC_ISCOMPLETED == "C").FirstOrDefault();

                    if (lsbMonthlyStatus != null)
                    {
                        if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year > lsbMonthlyStatus.EXEC_PROG_YEAR || commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year < lsbMonthlyStatus.EXEC_PROG_YEAR)
                        {
                            message = "Construction completion date should be equal to physical progress completion date.";
                            return false;
                        }
                        else if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year == lsbMonthlyStatus.EXEC_PROG_YEAR && (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Month > lsbMonthlyStatus.EXEC_PROG_MONTH || commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Month < lsbMonthlyStatus.EXEC_PROG_MONTH))
                        {
                            message = "Construction completion date should be equal to physical progress completion date.";
                            return false;
                        }
                    }
                }
                using (var scope = new TransactionScope())
                {
                    agreementDetails = new MANE_IMS_CONTRACT();
                    agreementDetails.IMS_PR_ROAD_CODE = IMSPRRoadCode;
                    agreementDetails.MANE_PR_CONTRACT_CODE = (Int32)GetMaxCode(MaintenanceAgreementModules.IMSContract, IMSPRRoadCode);
                    agreementDetails.MANE_CONTRACT_ID = dbContext.MANE_IMS_CONTRACT.Max(cp => (Int32?)cp.MANE_CONTRACT_ID) == null ? 1 : (Int32)dbContext.MANE_IMS_CONTRACT.Max(cp => (Int32?)cp.MANE_CONTRACT_ID) + 1;
                    agreementDetails.MAST_CON_ID = model.MAST_CON_ID;
                    agreementDetails.MANE_AGREEMENT_NUMBER = model.MANE_AGREEMENT_NUMBER;
                    agreementDetails.MANE_AGREEMENT_DATE = commonFunction.GetStringToDateTime(model.MANE_AGREEMENT_DATE);
                    agreementDetails.MANE_CONSTR_COMP_DATE = commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE);
                    agreementDetails.MANE_MAINTENANCE_START_DATE = commonFunction.GetStringToDateTime(model.MANE_MAINTENANCE_START_DATE);

                    //added by ahishek kamble 21-nov-2013
                    if (model.MANE_MAINTENANCE_END_DATE != null)
                    {
                        agreementDetails.MANE_MAINTENANCE_END_DATE = commonFunction.GetStringToDateTime(model.MANE_MAINTENANCE_END_DATE);
                    }
                    else
                    {
                        agreementDetails.MANE_MAINTENANCE_END_DATE = null;
                    }

                    agreementDetails.MANE_HANDOVER_DATE = model.MANE_HANDOVER_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(model.MANE_HANDOVER_DATE);
                    agreementDetails.MANE_YEAR1_AMOUNT = (Decimal)model.MANE_RENEWAL_AMOUNT;
                    agreementDetails.MANE_YEAR2_AMOUNT = (Decimal)model.MANE_YEAR1_AMOUNT;//0;
                    agreementDetails.MANE_YEAR3_AMOUNT = (Decimal)model.MANE_YEAR2_AMOUNT;//0;
                    agreementDetails.MANE_YEAR4_AMOUNT = (Decimal)model.MANE_YEAR3_AMOUNT;//0;
                    agreementDetails.MANE_YEAR5_AMOUNT = (Decimal)model.MANE_YEAR4_AMOUNT;//0;
                    agreementDetails.MANE_YEAR6_AMOUNT = (Decimal)(model.MANE_YEAR5_AMOUNT == null ? 0 : model.MANE_YEAR5_AMOUNT);
                    //if (PMGSYSession.Current.PMGSYScheme == 2)
                    //{
                    //    agreementDetails.MANE_YEAR6_AMOUNT = (Decimal)(model.MANE_YEAR6_AMOUNT == null ? 0 : model.MANE_YEAR6_AMOUNT);
                    //}
                    agreementDetails.MANE_HANDOVER_TO = model.MANE_HANDOVER_TO == null ? null : model.MANE_HANDOVER_TO.Trim();
                    agreementDetails.MANE_CONTRACT_STATUS = "P";
                    agreementDetails.MANE_CONTRACT_FINALIZED = "N";
                    agreementDetails.MANE_LOCK_STATUS = "N";
                    agreementDetails.MANE_AGREEMENT_TYPE = "T";
                    if (model.IMS_WORK_CODE > 0)
                    {
                        agreementDetails.IMS_WORK_CODE = model.IMS_WORK_CODE;
                        agreementDetails.MANE_PART_AGREEMENT = "Y";


                        if (dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_WORK_CODE == model.IMS_WORK_CODE).Any())
                        {
                            agreementDetails.MANE_FIRST_AGREEMENT = "N";
                        }
                        else
                        {
                            agreementDetails.MANE_FIRST_AGREEMENT = "Y";
                        }
                    }
                    else
                    {
                        agreementDetails.MANE_PART_AGREEMENT = "N";

                        if (dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Any())
                        {
                            agreementDetails.MANE_FIRST_AGREEMENT = "N";
                        }
                        else
                        {
                            agreementDetails.MANE_FIRST_AGREEMENT = "Y";
                        }
                    }

                    //for add contractor number count 
                    if (dbContext.MANE_IMS_CONTRACT.Any(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode && IMS.MANE_AGREEMENT_TYPE == "T"))
                    {
                        MANE_IMS_CONTRACT IMSContracts = null;

                        if (model.IMS_WORK_CODE > 0)
                        {
                            IMSContracts = dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode && IMS.IMS_WORK_CODE == model.IMS_WORK_CODE).OrderByDescending(IMS => IMS.MANE_PR_CONTRACT_CODE).FirstOrDefault();
                        }
                        else if (model.IMS_WORK_CODE == 0)
                        {
                            IMSContracts = dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(IMS => IMS.MANE_PR_CONTRACT_CODE).FirstOrDefault();
                        }

                        if (IMSContracts != null)
                        {
                            if (IMSContracts.MANE_CONTRACT_STATUS == "I")
                            {
                                maxContractorNumber = IMSContracts.MANE_CONTRACT_NUMBER;
                            }
                            else
                            {
                                maxContractorNumber = IMSContracts.MANE_CONTRACT_NUMBER + 1;
                            }
                        }
                    }

                    agreementDetails.MANE_CONTRACT_NUMBER = (int)maxContractorNumber;
                    agreementDetails.USERID = PMGSYSession.Current.UserId;
                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.MANE_IMS_CONTRACT.Add(agreementDetails);

                    //update tender status to M when contractor not continue with same contractor
                    if (!model.IsNewContractor)
                    {
                        TEND_AGREEMENT_DETAIL tendAgreementDetails = null;
                        if (model.IMS_WORK_CODE > 0)
                        {
                            tendAgreementDetails = (from agreementDetail in dbContext.TEND_AGREEMENT_DETAIL
                                                    join agreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                                    on agreementDetail.TEND_AGREEMENT_CODE equals agreementMaster.TEND_AGREEMENT_CODE
                                                    where
                                                    agreementDetail.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                                    agreementDetail.IMS_WORK_CODE == model.IMS_WORK_CODE &&
                                                    agreementDetail.TEND_AGREEMENT_STATUS == "C" &&
                                                    agreementMaster.TEND_AGREEMENT_TYPE == "C"
                                                    select agreementDetail
                                                       ).FirstOrDefault();
                        }
                        else if (model.IMS_WORK_CODE == 0)
                        {
                            tendAgreementDetails = (from agreementDetail in dbContext.TEND_AGREEMENT_DETAIL
                                                    join agreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                                    on agreementDetail.TEND_AGREEMENT_CODE equals agreementMaster.TEND_AGREEMENT_CODE
                                                    where
                                                    agreementDetail.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                                    agreementDetail.TEND_AGREEMENT_STATUS == "C" &&
                                                    agreementMaster.TEND_AGREEMENT_TYPE == "C"
                                                    select agreementDetail
                                                      ).FirstOrDefault();
                        }

                        if (tendAgreementDetails != null)
                        {
                            tendAgreementDetails.TEND_AGREEMENT_STATUS = "M";
                            tendAgreementDetails.USERID = PMGSYSession.Current.UserId;
                            tendAgreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(tendAgreementDetails).State = System.Data.Entity.EntityState.Modified;
                        }
                    }

                    dbContext.SaveChanges();
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveRenewalAgreementDetailsDAL()");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public RenewalMaintenanceViewModel GetRenewalAgreementDetailsDAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId, bool isView = false)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CommonFunctions commonFunction = new CommonFunctions();
                MANE_IMS_CONTRACT IMSContract = null;

                if (!isView)
                {
                    IMSContract = dbContext.MANE_IMS_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.MANE_PR_CONTRACT_CODE == PRContractCode && c.MANE_LOCK_STATUS == "N" && c.MANE_CONTRACT_STATUS == "P").FirstOrDefault();
                }
                else
                {
                    IMSContract = dbContext.MANE_IMS_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.MANE_PR_CONTRACT_CODE == PRContractCode).FirstOrDefault();
                }

                RenewalMaintenanceViewModel agreementDetails = null;


                if (IMSContract != null)
                {


                    agreementDetails = new RenewalMaintenanceViewModel()
                    {

                        EncryptedIMSPRRoadCode = URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(), "PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() }),
                        EncryptedPRContractCode = URLEncrypt.EncryptParameters1(new string[] { "PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() }),
                        MAST_CON_ID = IMSContract.MAST_CON_ID,
                        MANE_CONTRACT_ID = IMSContract.MANE_CONTRACT_ID,
                        MANE_AGREEMENT_NUMBER = IMSContract.MANE_AGREEMENT_NUMBER,
                        MANE_CONSTR_COMP_DATE = Convert.ToDateTime(IMSContract.MANE_CONSTR_COMP_DATE).ToString("dd/MM/yyyy"),
                        MANE_AGREEMENT_DATE = Convert.ToDateTime(IMSContract.MANE_AGREEMENT_DATE).ToString("dd/MM/yyyy"),
                        MANE_MAINTENANCE_START_DATE = Convert.ToDateTime(IMSContract.MANE_MAINTENANCE_START_DATE).ToString("dd/MM/yyyy"),
                        //added by abhishek kamble 21-nov-2013
                        MANE_MAINTENANCE_END_DATE = IMSContract.MANE_MAINTENANCE_END_DATE == null ? string.Empty : Convert.ToDateTime(IMSContract.MANE_MAINTENANCE_END_DATE).ToString("dd/MM/yyyy"),
                        MANE_HANDOVER_DATE = IMSContract.MANE_HANDOVER_DATE == null ? string.Empty : Convert.ToDateTime(IMSContract.MANE_HANDOVER_DATE).ToString("dd/MM/yyyy"),
                        MANE_HANDOVER_TO = IMSContract.MANE_HANDOVER_TO,

                        MANE_RENEWAL_AMOUNT = IMSContract.MANE_YEAR1_AMOUNT,
                        MANE_YEAR1_AMOUNT = IMSContract.MANE_YEAR2_AMOUNT,
                        MANE_YEAR2_AMOUNT = IMSContract.MANE_YEAR3_AMOUNT,
                        MANE_YEAR3_AMOUNT = IMSContract.MANE_YEAR4_AMOUNT,
                        MANE_YEAR4_AMOUNT = IMSContract.MANE_YEAR5_AMOUNT,
                        MANE_YEAR5_AMOUNT = IMSContract.MANE_YEAR6_AMOUNT,

                        IMS_WORK_CODE = IMSContract.IMS_WORK_CODE == null ? 0 : (Int32)IMSContract.IMS_WORK_CODE,
                        IncompleteReason = IMSContract.MANE_INCOMPLETE_REASON,
                        ValueOfWorkDone = IMSContract.MANE_VALUE_WORK_DONE,
                        IsEdit = true
                    };

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementDetails.MANE_YEAR6_AMOUNT = (Decimal)(IMSContract.MANE_YEAR6_AMOUNT == null ? 0 : IMSContract.MANE_YEAR6_AMOUNT);
                    }
                }

                return agreementDetails;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// updates the details of special agreement
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UpdateRenewalAgreementDetailsDAL(RenewalMaintenanceViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string proposaType = string.Empty;
                int IMSPRRoadCode = 0;
                int PRContractCode = 0;
                EXEC_ROADS_MONTHLY_STATUS roadMonthlyStatus = null;
                EXEC_LSB_MONTHLY_STATUS lsbMonthlyStatus = null;
                CommonFunctions commonFunction = new CommonFunctions();
                encryptedParameters = model.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                PRContractCode = Convert.ToInt32(decryptedParameters["PRContractCode"].ToString());

                ///Changes by SAMMED PATIL for LSB and Road proposals 28/06/2016
                proposaType = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(x => x.IMS_PROPOSAL_TYPE).FirstOrDefault();
                if (proposaType == "P")
                {
                    roadMonthlyStatus = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(rms => rms.IMS_PR_ROAD_CODE == IMSPRRoadCode && rms.EXEC_ISCOMPLETED == "C").FirstOrDefault();

                    if (roadMonthlyStatus != null)
                    {
                        if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year > roadMonthlyStatus.EXEC_PROG_YEAR)
                        {
                            message = "Construction completion date should be less than physical progress completion date.";
                            return false;
                        }
                        else if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year == roadMonthlyStatus.EXEC_PROG_YEAR && commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Month > roadMonthlyStatus.EXEC_PROG_MONTH)
                        {
                            message = "Construction completion date should be less than physical progress completion date.";
                            return false;
                        }
                    }
                }
                else
                {
                    lsbMonthlyStatus = dbContext.EXEC_LSB_MONTHLY_STATUS.Where(rms => rms.IMS_PR_ROAD_CODE == IMSPRRoadCode && rms.EXEC_ISCOMPLETED == "C").FirstOrDefault();

                    if (lsbMonthlyStatus != null)
                    {
                        if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year > lsbMonthlyStatus.EXEC_PROG_YEAR)
                        {
                            message = "Construction completion date should be less than physical progress completion date.";
                            return false;
                        }
                        else if (commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Year == lsbMonthlyStatus.EXEC_PROG_YEAR && commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE).Month > lsbMonthlyStatus.EXEC_PROG_MONTH)
                        {
                            message = "Construction completion date should be less than physical progress completion date.";
                            return false;
                        }
                    }
                }

                MANE_IMS_CONTRACT IMSContract = dbContext.MANE_IMS_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.MANE_PR_CONTRACT_CODE == PRContractCode && c.MANE_CONTRACT_STATUS == "P").FirstOrDefault();

                if (IMSContract == null)
                {
                    return false;
                }

                IMSContract.MANE_AGREEMENT_DATE = commonFunction.GetStringToDateTime(model.MANE_AGREEMENT_DATE);
                IMSContract.MANE_CONSTR_COMP_DATE = commonFunction.GetStringToDateTime(model.MANE_CONSTR_COMP_DATE);
                IMSContract.MANE_MAINTENANCE_START_DATE = commonFunction.GetStringToDateTime(model.MANE_MAINTENANCE_START_DATE);

                if (model.MANE_MAINTENANCE_END_DATE != null)
                {
                    IMSContract.MANE_MAINTENANCE_END_DATE = commonFunction.GetStringToDateTime(model.MANE_MAINTENANCE_END_DATE);
                }
                else
                {
                    IMSContract.MANE_MAINTENANCE_END_DATE = null;
                }


                IMSContract.MANE_HANDOVER_DATE = model.MANE_HANDOVER_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(model.MANE_HANDOVER_DATE);

                IMSContract.MANE_YEAR1_AMOUNT = (Decimal)model.MANE_RENEWAL_AMOUNT;
                IMSContract.MANE_YEAR2_AMOUNT = (Decimal)model.MANE_YEAR1_AMOUNT;
                IMSContract.MANE_YEAR3_AMOUNT = (Decimal)model.MANE_YEAR2_AMOUNT;
                IMSContract.MANE_YEAR4_AMOUNT = (Decimal)model.MANE_YEAR3_AMOUNT;
                IMSContract.MANE_YEAR5_AMOUNT = (Decimal)model.MANE_YEAR4_AMOUNT;
                IMSContract.MANE_YEAR6_AMOUNT = (Decimal)model.MANE_YEAR5_AMOUNT;

                //if (PMGSYSession.Current.PMGSYScheme == 2)
                //{
                //    IMSContract.MANE_YEAR6_AMOUNT = (Decimal)(model.MANE_YEAR5_AMOUNT == null ? 0 : model.MANE_YEAR6_AMOUNT);
                //}
                IMSContract.MANE_HANDOVER_TO = model.MANE_HANDOVER_TO == null ? null : model.MANE_HANDOVER_TO.Trim();
                IMSContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                IMSContract.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL.UpdateRenewalAgreementDetailsDAL().OptimisticConcurrencyException");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL.UpdateRenewalAgreementDetailsDAL().UpdateException");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL.UpdateRenewalAgreementDetailsDAL()");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region Maintenance Work Repackaging

        /// <summary>
        /// returns the list of proposal for repackaging
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="year"></param>
        /// <param name="batch"></param>
        /// <param name="block"></param>
        /// <param name="package"></param>
        /// <param name="collaboration"></param>
        /// <param name="proposalType"></param>
        /// <param name="upgradationType"></param>
        /// <returns></returns>
        public Array GetMaintenanceProposalsForRepackaging(int? page, int? rows, string sidx, string sord, out long totalRecords, int year, int batch, int block, string package, int collaboration, string proposalType, string upgradationType)
        {
            var dbContext = new PMGSYEntities();
            try
            {
                //var lstPropsoals = dbContext.USP_GET_MAINTENANCE_PROPOSALS_REPACKAGING(PMGSYSession.Current.DistrictCode, (block <= 0 ? 0 : block), (batch <= 0 ? 0 : batch), (year <= 0 ? 0 : year), package, (collaboration <= 0 ? 0 : collaboration), proposalType, upgradationType, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.PMGSYScheme).ToList();
                var lstPropsoals = dbContext.USP_GET_MAINTENANCE_PROPOSALS_REPACKAGING(PMGSYSession.Current.DistrictCode, (block <= 0 ? 0 : block), PMGSYSession.Current.AdminNdCode, (year <= 0 ? 0 : year), package, (batch <= 0 ? 0 : batch), (collaboration <= 0 ? 0 : collaboration), upgradationType, proposalType, PMGSYSession.Current.PMGSYScheme).ToList();
                totalRecords = lstPropsoals.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstPropsoals = lstPropsoals.OrderBy(m => m.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "ROAD_NAME":
                                lstPropsoals = lstPropsoals.OrderBy(m => m.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "IMS_YEAR":
                                lstPropsoals = lstPropsoals.OrderBy(m => m.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "IMS_BATCH":
                                lstPropsoals = lstPropsoals.OrderBy(m => m.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID_OLD":
                                lstPropsoals = lstPropsoals.OrderBy(m => m.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstPropsoals = lstPropsoals.OrderBy(m => m.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstPropsoals = lstPropsoals.OrderBy(m => m.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "ROAD_NAME":
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "IMS_YEAR":
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "IMS_BATCH":
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID_OLD":
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstPropsoals = lstPropsoals.OrderBy(m => m.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }



                var result = lstPropsoals.Select(m => new
                {
                    m.IMS_PR_ROAD_CODE,
                    m.IMS_PACKAGE_ID,
                    IMS_PACKAGE_ID_OLD = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == m.IMS_PR_ROAD_CODE).SingleOrDefault().IMS_PACKAGE_ID.ToString(),
                    m.IMS_ROAD_NAME,
                    m.MAST_BLOCK_NAME,
                    m.ROAD_LENGTH,
                    m.IMS_BATCH,
                    m.IMS_YEAR,
                }).ToArray();


                return result.Select(m => new
                {
                    cell = new[] 
                    {
                        
                        m.MAST_BLOCK_NAME == null?"":m.MAST_BLOCK_NAME.ToString(),
                        m.IMS_BATCH == null?"-":"Batch "+m.IMS_BATCH.ToString(),
                        m.IMS_YEAR == null?"-":(m.IMS_YEAR + "-" + (m.IMS_YEAR + 1)).ToString(),
                        m.IMS_PACKAGE_ID_OLD == null ? "-" : m.IMS_PACKAGE_ID_OLD.ToString(),
                        m.IMS_PACKAGE_ID == null?"-":m.IMS_PACKAGE_ID.ToString(),
                        m.IMS_ROAD_NAME == null?"-":m.IMS_ROAD_NAME.ToString(),
                        m.ROAD_LENGTH == null?"-":m.ROAD_LENGTH.ToString(),
                        "<a href='#' title='Click here to add Repackaging Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=AddRepackagingDetails('" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+m.IMS_PR_ROAD_CODE.ToString().Trim() }) +"'); return false;'>Add Repackaging Details</a>"
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetProposalsForRepackaging().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        /// <summary>
        /// returns the list of packages for repackaging
        /// </summary>
        /// <param name="block"></param>
        /// <param name="year"></param>
        /// <param name="batch"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateMaintenancePackagesForRepackaging(int block, int year, int batch)
        {
            var dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> PackageList = new List<SelectListItem>();
                var lstPackages = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                   join Maint in dbContext.MAINE_REPACKAGE_DETAILS on item.IMS_PR_ROAD_CODE equals Maint.IMS_PR_ROAD_CODE into MRD
                                   from p in MRD.DefaultIfEmpty()
                                   where
                                    item.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode &&
                                   (block < 0 ? 1 : item.MAST_BLOCK_CODE) == (block < 0 ? 1 : block) &&
                                   (year == 0 ? 1 : item.IMS_YEAR) == (year == 0 ? 1 : year) &&
                                   (batch == 0 ? 1 : item.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
                                   (item.IMS_ISCOMPLETED.Equals("C") || item.IMS_ISCOMPLETED.Equals("X"))

                                   select new
                                   {
                                       PACKAGE_CODE = p.IMS_MAINT_REPACKAGE_ID == null ? item.IMS_PACKAGE_ID : p.IMS_MAINT_REPACKAGE_ID,
                                       PACKAGE_NAME = p.IMS_MAINT_REPACKAGE_ID == null ? item.IMS_PACKAGE_ID : p.IMS_MAINT_REPACKAGE_ID
                                   }).Distinct().ToList();

                PackageList = new SelectList(lstPackages, "PACKAGE_CODE", "PACKAGE_NAME").ToList();
                PackageList.Insert(0, new SelectListItem { Value = "0", Text = "All Packages" });
                return PackageList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulatePackagesForRepackaging().DAL");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        /// <summary>
        /// updates the details of proposal after repackaging
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddMaintenanceRepackagingDetails(PMGSY.Models.MaintenanceAgreement.RepackagingDetailsViewModel model)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                string[] encryptedParameters = model.EncProposalCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                IMS_SANCTIONED_PROJECTS ims_master = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).FirstOrDefault();

                if (dbContext.MAINE_REPACKAGE_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode))
                {
                    MAINE_REPACKAGE_DETAILS repackgeDetails = dbContext.MAINE_REPACKAGE_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).FirstOrDefault();
                    if (model.NewOldPackage == "Y")
                    {
                        repackgeDetails.IMS_MAINT_REPACKAGE_ID = /*model.OLD_PACKAGE_ID_PREFIX +*/ model.NEW_PACKAGE_ID;
                    }
                    else
                    {
                        repackgeDetails.IMS_MAINT_REPACKAGE_ID = model.ExistingPackage;
                    }
                    repackgeDetails.ENTRY_BY = PMGSYSession.Current.RoleName;
                    repackgeDetails.ENTRY_DATE = DateTime.Now;
                    repackgeDetails.USERID = PMGSYSession.Current.UserId;
                    repackgeDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    ims_master.IMS_OLD_PACKAGE_ID = model.OLD_PACKAGE_ID;
                    dbContext.Entry(repackgeDetails).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    MAINE_REPACKAGE_DETAILS repackgeDetails = new MAINE_REPACKAGE_DETAILS();
                    if (model.NewOldPackage == "Y")
                    {
                        // repackgeDetails.IMS_MAINT_REPACKAGE_ID = model.ShortStateCode + model.NEW_PACKAGE_ID;
                        repackgeDetails.IMS_MAINT_REPACKAGE_ID = /*model.OLD_PACKAGE_ID_PREFIX +*/ model.NEW_PACKAGE_ID;
                    }
                    else
                    {
                        repackgeDetails.IMS_MAINT_REPACKAGE_ID = model.ExistingPackage;
                    }
                    repackgeDetails.IMS_PR_ROAD_CODE = ims_master.IMS_PR_ROAD_CODE;
                    //  repackgeDetails.START_CHAINAGE = model.StartChainage;  ///commented as per suggstion by Pankaj Sir
                    //  repackgeDetails.END_CHAINAGE = model.EndChainage;
                    repackgeDetails.ENTRY_BY = PMGSYSession.Current.RoleName;
                    repackgeDetails.ENTRY_DATE = DateTime.Now;
                    repackgeDetails.USERID = PMGSYSession.Current.UserId;
                    repackgeDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    ims_master.IMS_OLD_PACKAGE_ID = model.OLD_PACKAGE_ID;
                    dbContext.MAINE_REPACKAGE_DETAILS.Add(repackgeDetails);
                }
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddRepackagingDetails().DAL");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        /// <summary>
        /// populates the packages according to the block
        /// </summary>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateMaintenancePackageBlockWise(int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> lstPackage = new List<SelectListItem>();
            try
            {
                var pkgColl = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                               join Maint in dbContext.MAINE_REPACKAGE_DETAILS on item.IMS_PR_ROAD_CODE equals Maint.IMS_PR_ROAD_CODE into PV
                               from PS in PV.DefaultIfEmpty()
                               where item.MAST_BLOCK_CODE == blockCode
                               select new
                               {
                                   PACKAGE_CODE = PS.IMS_MAINT_REPACKAGE_ID == null ? item.IMS_PACKAGE_ID : PS.IMS_MAINT_REPACKAGE_ID,
                                   PACKAGE_NAME = PS.IMS_MAINT_REPACKAGE_ID == null ? item.IMS_PACKAGE_ID : PS.IMS_MAINT_REPACKAGE_ID
                               }).Distinct().ToList();
                if (pkgColl == null || pkgColl.Count() == 0)
                {
                    lstPackage.Insert(0, (new SelectListItem { Text = "All Packages", Value = "0", Selected = true }));
                    return lstPackage;
                }
                else
                {
                    foreach (var item in pkgColl)
                    {
                        lstPackage.Insert(0, (new SelectListItem { Text = item.PACKAGE_NAME.ToString(), Value = item.PACKAGE_CODE.ToString() }));
                    }
                    lstPackage.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0" }));
                }
                return lstPackage;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateMaintenancePackageBlockWise().DAL");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// get the details of proposal
        /// </summary>
        /// <param name="proposalCode"></param>
        /// <returns></returns>
        public PMGSY.Models.MaintenanceAgreement.RepackagingDetailsViewModel GetMaintenanceRepackagingDetails(int proposalCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                TransactionParams objParam = new TransactionParams();
                CommonFunctions objCommon = new CommonFunctions();
                PMGSY.Models.MaintenanceAgreement.RepackagingDetailsViewModel model = new PMGSY.Models.MaintenanceAgreement.RepackagingDetailsViewModel();
                IMS_SANCTIONED_PROJECTS ims_master = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).FirstOrDefault();
                objParam.BlockCode = ims_master.MAST_BLOCK_CODE;
                model.OLD_PACKAGE_ID = ims_master.IMS_PACKAGE_ID;
                model.OLD_PACKAGE_ID_PREFIX = ims_master.IMS_PACKAGE_ID + "M"; //chages done as per suggestion from Pankaj Sir [04/06/2018]
                model.roadLength = ims_master.IMS_PAV_LENGTH;
                model.ShortStateCode = ims_master.MASTER_STATE.MAST_STATE_SHORT_CODE + (ims_master.MASTER_DISTRICT.MAST_DISTRICT_ID < 10 ? "0" + ims_master.MASTER_DISTRICT.MAST_DISTRICT_ID.ToString() : ims_master.MASTER_DISTRICT.MAST_DISTRICT_ID.ToString());
                model.lstPackages = PopulateMaintenancePackageBlockWise(objParam.BlockCode);

                MAINE_REPACKAGE_DETAILS repackgeDetails = dbContext.MAINE_REPACKAGE_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).FirstOrDefault();
                if (repackgeDetails != null)
                {
                    model.NEW_PACKAGE_ID = repackgeDetails.IMS_MAINT_REPACKAGE_ID;
                }
                else
                {
                    model.NEW_PACKAGE_ID = model.OLD_PACKAGE_ID_PREFIX;
                }
                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetRepackagingDetails().DAL");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region EMARG CORRECTION

        public Array EmargDAL(int? page, int? rows, string sidx, string sord, out int totalRecords, int piuCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (PMGSYSession.Current.LevelId == 5 || PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
                {
                    // var curSes = PMGSYSession.Current.RoleName;
                    //var code = piuCode;
                    //var dcode = PMGSYSession.Current.DistrictCode;
                    //var scode = PMGSYSession.Current.StateCode;

                    //get districtCode if it's awailable in session use that it will cover for first user else for rest of use find district code from DPIU code using ternary op

                    //find district code using DPIUcode returned on select
                    var calculatedDcodeFromDPIU = (from u in dbContext.IMS_SANCTIONED_PROJECTS
                                                   where u.MAST_DPIU_CODE == piuCode
                                                   select u.MAST_DISTRICT_CODE).FirstOrDefault();

                    //statecode req form mord user

                    var newDCode = ((PMGSYSession.Current.DistrictCode != 0) ? (PMGSYSession.Current.DistrictCode) : (calculatedDcodeFromDPIU)); //getting district code

                    var lstTechnologyDetails1 = (from erd in dbContext.EMARG_ROAD_DETAILS
                                                 join isp in dbContext.IMS_SANCTIONED_PROJECTS on erd.ROAD_CODE equals isp.IMS_PR_ROAD_CODE
                                                 join emarg in dbContext.MANE_EMARG_CONTRACT on

                                                 new { C1 = isp.IMS_PR_ROAD_CODE, C2 = erd.EMARG_ID } equals
                                                 new { C1 = emarg.IMS_PR_ROAD_CODE, C2 = emarg.EMARG_ID }



                                                  into details
                                                 //  join emarg in dbContext.MANE_EMARG_CONTRACT on erd.ROAD_CODE equals emarg.IMS_PR_ROAD_CODE into details
                                                 from detailrecords in details.DefaultIfEmpty()
                                                 where isp.MAST_DISTRICT_CODE == newDCode && isp.IMS_SANCTIONED == "Y"
                                                       && ((PMGSYSession.Current.LevelId == 5 || PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6) ? (isp.MAST_DPIU_CODE == isp.MAST_DPIU_CODE) : (isp.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode))
                                                 select new
                                                 {
                                                     isp.IMS_PR_ROAD_CODE,
                                                     erd.MAST_STATE_NAME,
                                                     erd.MAST_DISTRICT_NAME,
                                                     isp.IMS_ROAD_NAME,
                                                     isp.IMS_PACKAGE_ID,
                                                     isp.MAST_STATE_CODE,
                                                     isp.MAST_DISTRICT_CODE,
                                                     isp.IMS_PAV_LENGTH,
                                                     detailrecords.MANE_IMS_ROAD_FINALIZE,
                                                     detailrecords.MANE_IMS_PACKAGE_FINALIZE,
                                                     detailrecords.IS_PUSHED_TO_EMARG,
                                                     erd.REJECTION_REASON,
                                                     erd.EMARG_ID,
                                                     erd.IS_DEACTIVATED
                                                 }).OrderBy(m => m.IMS_PACKAGE_ID).Distinct();

                    var lstTechnologyDetails = lstTechnologyDetails1.Where(m => m.IS_DEACTIVATED != "Y" || m.IS_DEACTIVATED == null).ToList();
                    totalRecords = lstTechnologyDetails.Count();

                    lstTechnologyDetails.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));

                    var result = lstTechnologyDetails.Select(m => new
                    {
                        m.IMS_PR_ROAD_CODE,
                        m.MAST_STATE_NAME,
                        m.MAST_DISTRICT_NAME,
                        m.IMS_ROAD_NAME,
                        m.IMS_PACKAGE_ID,
                        m.IMS_PAV_LENGTH,
                        m.MANE_IMS_ROAD_FINALIZE,
                        m.MANE_IMS_PACKAGE_FINALIZE,
                        m.IS_PUSHED_TO_EMARG,
                        m.REJECTION_REASON,
                        m.EMARG_ID
                    }).OrderBy(m => m.IMS_PACKAGE_ID).ToArray();

                    return result.Select(m => new
                    {
                        id = m.IMS_PR_ROAD_CODE.ToString(),
                        cell = new[]
                    {
                        m.IMS_PACKAGE_ID==null?string.Empty:m.IMS_PACKAGE_ID.ToString().Trim(),
                        m.IMS_ROAD_NAME == null?string.Empty:m.IMS_ROAD_NAME.ToString() +("(Emarg ID : "+m.EMARG_ID+")"),
                        m.MAST_STATE_NAME == null?string.Empty:m.MAST_STATE_NAME.ToString(),
                        m.MAST_DISTRICT_NAME == null?string.Empty:m.MAST_DISTRICT_NAME.ToString(),
                        m.IMS_PAV_LENGTH == null?string.Empty:m.IMS_PAV_LENGTH.ToString(),

                       // m.EMARG_ID==null?"Edit":"No Edit",
                       ((PMGSYSession.Current.LevelId == 5)?
                        ((m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":   "<center><table><tr><td disabled='disabled'  style='border-color:white'><span disabled='disabled' class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>"):"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>")
                        : ((m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":   "<center><table><tr><td disabled='disabled'  style='border-color:white'><span disabled='disabled' class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='' ></span></td></tr></table></center>"):"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='' ></span></td></tr></table></center>")),
		                    
	                    
                        //(m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":   "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='' ></span></td></tr></table></center>"):"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='' ></span></td></tr></table></center>",
                       
                       

                     //   (m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString().Trim()})):URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString().Trim()}),

                        PMGSYSession.Current.LevelId == 5 ? ((m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("N")?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Road Details' onClick ='FinalizeEmargCorrectionRoad(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"):"-") : ((m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("N")?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Road Details' onClick ='' ></span></td></tr></table></center>":"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"):"-"),

                        (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))?((m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N"))?"<span style='color:green;'>Package is Finalized</span>":"<span style='color:green;'>Package Pushed to EMARG</span>"):(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<span style='color:red;'>Road is Finalized</span>":"-")):"-",
 
                        // Road Deletion or Definalization before package finalization // m.MANE_IMS_ROAD_FINALIZE.Equals("Y") &&
                        
                        PMGSYSession.Current.LevelId == 5 ? (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_PACKAGE_FINALIZE!="Y" &&(m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N")))?"<a href='#' title='Click here to definalize Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteNonPackageFinalizedRoad('" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString().Trim()}) + "'); return false;'>Delete</a>":"-"):"-"  : (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_PACKAGE_FINALIZE!="Y" &&(m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N")))?"<a href='#' title='Click here to definalize Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick=''; return false;'>Delete</a>":"-"):"-",


                        (m.REJECTION_REASON!=null)?(m.REJECTION_REASON.Equals("1")?"Rejected By System":"PIU Correction Request"):"-",


                        //((PMGSYSession.Current.RoleName.Equals("PIU"))?
                        //((m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Road Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"-")):"-") 
                        //:((m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Road Details' onClick ='' ></span></td></tr></table></center>":(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Details' onClick ='' ></span></td></tr></table></center>":"-")):"-")),

                        (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Road Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"-")):"-",

                        "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Click here to Push Package details to Emarg' onClick ='PushToEmarg(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.IMS_PR_ROAD_CODE.ToString()  }) + "\");' ></span></td></tr></table></center>"
                         //(m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? (m.IS_PUSHED_TO_EMARG.Equals("N")?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='Click here to Push Package details to Emarg' onClick ='PushToEmarg(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString()  }) + "\");' ></span></td></tr></table></center>":"Details Pushed To Emarg"):"-"
                    }
                    }).ToArray();

                }
                else
                {
                    var lstTechnologyDetails1 = (from erd in dbContext.EMARG_ROAD_DETAILS
                                                 join isp in dbContext.IMS_SANCTIONED_PROJECTS on erd.ROAD_CODE equals isp.IMS_PR_ROAD_CODE
                                                 join emarg in dbContext.MANE_EMARG_CONTRACT on

                                                 new { C1 = isp.IMS_PR_ROAD_CODE, C2 = erd.EMARG_ID } equals
                                                 new { C1 = emarg.IMS_PR_ROAD_CODE, C2 = emarg.EMARG_ID }



                                                  into details
                                                 //  join emarg in dbContext.MANE_EMARG_CONTRACT on erd.ROAD_CODE equals emarg.IMS_PR_ROAD_CODE into details
                                                 from detailrecords in details.DefaultIfEmpty()
                                                 where isp.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode && isp.IMS_SANCTIONED == "Y"
                                                           && ((PMGSYSession.Current.RoleName.Equals("PIUOA") || PMGSYSession.Current.RoleName.Equals("PIURCPLWE")) ? (isp.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode) : (isp.MAST_DPIU_CODE == isp.MAST_DPIU_CODE))
                                                 select new
                                                 {
                                                     isp.IMS_PR_ROAD_CODE,
                                                     erd.MAST_STATE_NAME,
                                                     erd.MAST_DISTRICT_NAME,
                                                     isp.IMS_ROAD_NAME,
                                                     isp.IMS_PACKAGE_ID,
                                                     isp.MAST_STATE_CODE,
                                                     isp.MAST_DISTRICT_CODE,
                                                     isp.IMS_PAV_LENGTH,
                                                     detailrecords.MANE_IMS_ROAD_FINALIZE,
                                                     detailrecords.MANE_IMS_PACKAGE_FINALIZE,
                                                     detailrecords.IS_PUSHED_TO_EMARG,
                                                     erd.REJECTION_REASON,
                                                     erd.EMARG_ID,
                                                     erd.IS_DEACTIVATED
                                                 }).OrderBy(m => m.IMS_PACKAGE_ID).Distinct();

                    var lstTechnologyDetails = lstTechnologyDetails1.Where(m => m.IS_DEACTIVATED != "Y" || m.IS_DEACTIVATED == null).ToList();
                    totalRecords = lstTechnologyDetails.Count();

                    lstTechnologyDetails.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));

                    var result = lstTechnologyDetails.Select(m => new
                    {
                        m.IMS_PR_ROAD_CODE,
                        m.MAST_STATE_NAME,
                        m.MAST_DISTRICT_NAME,
                        m.IMS_ROAD_NAME,
                        m.IMS_PACKAGE_ID,
                        m.IMS_PAV_LENGTH,
                        m.MANE_IMS_ROAD_FINALIZE,
                        m.MANE_IMS_PACKAGE_FINALIZE,
                        m.IS_PUSHED_TO_EMARG,
                        m.REJECTION_REASON,
                        m.EMARG_ID
                    }).OrderBy(m => m.IMS_PACKAGE_ID).ToArray();

                    return result.Select(m => new
                    {
                        id = m.IMS_PR_ROAD_CODE.ToString(),
                        cell = new[]
                    {
                        m.IMS_PACKAGE_ID==null?string.Empty:m.IMS_PACKAGE_ID.ToString().Trim(),
                        m.IMS_ROAD_NAME == null?string.Empty:m.IMS_ROAD_NAME.ToString() +("(Emarg ID : "+m.EMARG_ID+")"),
                        m.MAST_STATE_NAME == null?string.Empty:m.MAST_STATE_NAME.ToString(),
                        m.MAST_DISTRICT_NAME == null?string.Empty:m.MAST_DISTRICT_NAME.ToString(),
                        m.IMS_PAV_LENGTH == null?string.Empty:m.IMS_PAV_LENGTH.ToString(),

                       // m.EMARG_ID==null?"Edit":"No Edit",

                        (m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":   "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>"):"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>",

                     //   (m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString().Trim()})):URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString().Trim()}),

                        (m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("N")?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Road Details' onClick ='FinalizeEmargCorrectionRoad(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"):"-",

                        (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))?((m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N"))?"<span style='color:green;'>Package is Finalized</span>":"<span style='color:green;'>Package Pushed to EMARG</span>"):(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<span style='color:red;'>Road is Finalized</span>":"-")):"-",

                        // Road Deletion or Definalization before package finalization // m.MANE_IMS_ROAD_FINALIZE.Equals("Y") &&
                        (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_PACKAGE_FINALIZE!="Y" &&(m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N")))?"<a href='#' title='Click here to definalize Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteNonPackageFinalizedRoad('" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString().Trim()}) + "'); return false;'>Delete</a>":"-"):"-",

                        (m.REJECTION_REASON!=null)?(m.REJECTION_REASON.Equals("1")?"Rejected By System":"PIU Correction Request"):"-",

                        (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Road Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"-")):"-",

                        "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Click here to Push Package details to Emarg' onClick ='PushToEmarg(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.IMS_PR_ROAD_CODE.ToString()  }) + "\");' ></span></td></tr></table></center>"
                         //(m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? (m.IS_PUSHED_TO_EMARG.Equals("N")?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='Click here to Push Package details to Emarg' onClick ='PushToEmarg(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString()  }) + "\");' ></span></td></tr></table></center>":"Details Pushed To Emarg"):"-"
                    }
                    }).ToArray();
                }


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().EmargDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }




        //public Array EmargDAL(int? page, int? rows, string sidx, string sord, out int totalRecords)
        //{
        //    PMGSYEntities dbContext = new PMGSYEntities();

        //    try
        //    {

        //        var lstTechnologyDetails1 = (from erd in dbContext.EMARG_ROAD_DETAILS
        //                                     join isp in dbContext.IMS_SANCTIONED_PROJECTS on erd.ROAD_CODE equals isp.IMS_PR_ROAD_CODE
        //                                     join emarg in dbContext.MANE_EMARG_CONTRACT on

        //                                     new { C1 = isp.IMS_PR_ROAD_CODE, C2 = erd.EMARG_ID } equals
        //                                     new { C1 = emarg.IMS_PR_ROAD_CODE, C2 = emarg.EMARG_ID }



        //                                      into details
        //                                     //  join emarg in dbContext.MANE_EMARG_CONTRACT on erd.ROAD_CODE equals emarg.IMS_PR_ROAD_CODE into details
        //                                     from detailrecords in details.DefaultIfEmpty()
        //                                     where isp.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode && isp.IMS_SANCTIONED == "Y"
        //                                               && ((PMGSYSession.Current.RoleName.Equals("PIUOA") || PMGSYSession.Current.RoleName.Equals("PIURCPLWE")) ? (isp.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode) : (isp.MAST_DPIU_CODE == isp.MAST_DPIU_CODE))
        //                                     select new
        //                                     {
        //                                         isp.IMS_PR_ROAD_CODE,
        //                                         erd.MAST_STATE_NAME,
        //                                         erd.MAST_DISTRICT_NAME,
        //                                         isp.IMS_ROAD_NAME,
        //                                         isp.IMS_PACKAGE_ID,
        //                                         isp.MAST_STATE_CODE,
        //                                         isp.MAST_DISTRICT_CODE,
        //                                         isp.IMS_PAV_LENGTH,
        //                                         detailrecords.MANE_IMS_ROAD_FINALIZE,
        //                                         detailrecords.MANE_IMS_PACKAGE_FINALIZE,
        //                                         detailrecords.IS_PUSHED_TO_EMARG,
        //                                         erd.REJECTION_REASON,
        //                                         erd.EMARG_ID,
        //                                         erd.IS_DEACTIVATED
        //                                     }).OrderBy(m => m.IMS_PACKAGE_ID).Distinct();

        //        var lstTechnologyDetails = lstTechnologyDetails1.Where(m => m.IS_DEACTIVATED != "Y" || m.IS_DEACTIVATED == null).ToList();
        //        totalRecords = lstTechnologyDetails.Count();

        //        lstTechnologyDetails.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));

        //        var result = lstTechnologyDetails.Select(m => new
        //        {
        //            m.IMS_PR_ROAD_CODE,
        //            m.MAST_STATE_NAME,
        //            m.MAST_DISTRICT_NAME,
        //            m.IMS_ROAD_NAME,
        //            m.IMS_PACKAGE_ID,
        //            m.IMS_PAV_LENGTH,
        //            m.MANE_IMS_ROAD_FINALIZE,
        //            m.MANE_IMS_PACKAGE_FINALIZE,
        //            m.IS_PUSHED_TO_EMARG,
        //            m.REJECTION_REASON,
        //            m.EMARG_ID
        //        }).OrderBy(m => m.IMS_PACKAGE_ID).ToArray();

        //        return result.Select(m => new
        //        {
        //            id = m.IMS_PR_ROAD_CODE.ToString(),
        //            cell = new[] 
        //            {   
        //                m.IMS_PACKAGE_ID==null?string.Empty:m.IMS_PACKAGE_ID.ToString().Trim(),
        //                m.IMS_ROAD_NAME == null?string.Empty:m.IMS_ROAD_NAME.ToString() +("(Emarg ID : "+m.EMARG_ID+")"),
        //                m.MAST_STATE_NAME == null?string.Empty:m.MAST_STATE_NAME.ToString(),
        //                m.MAST_DISTRICT_NAME == null?string.Empty:m.MAST_DISTRICT_NAME.ToString(),
        //                m.IMS_PAV_LENGTH == null?string.Empty:m.IMS_PAV_LENGTH.ToString(),

        //               // m.EMARG_ID==null?"Edit":"No Edit",

        //                (m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":   "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>"):"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>",

        //             //   (m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString().Trim()})):URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString().Trim()}),

        //                (m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("N")?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Road Details' onClick ='FinalizeEmargCorrectionRoad(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"):"-",

        //                (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))?((m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N"))?"<span style='color:green;'>Package is Finalized</span>":"<span style='color:green;'>Package Pushed to EMARG</span>"):(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<span style='color:red;'>Road is Finalized</span>":"-")):"-",

        //                // Road Deletion or Definalization before package finalization // m.MANE_IMS_ROAD_FINALIZE.Equals("Y") &&
        //                (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_PACKAGE_FINALIZE!="Y" &&(m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N")))?"<a href='#' title='Click here to definalize Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteNonPackageFinalizedRoad('" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString().Trim()}) + "'); return false;'>Delete</a>":"-"):"-",

        //                (m.REJECTION_REASON!=null)?(m.REJECTION_REASON.Equals("1")?"Rejected By System":"PIU Correction Request"):"-",

        //                (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Road Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"-")):"-",

        //                "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Click here to Push Package details to Emarg' onClick ='PushToEmarg(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.IMS_PR_ROAD_CODE.ToString()  }) + "\");' ></span></td></tr></table></center>"
        //                 //(m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? (m.IS_PUSHED_TO_EMARG.Equals("N")?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='Click here to Push Package details to Emarg' onClick ='PushToEmarg(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString()  }) + "\");' ></span></td></tr></table></center>":"Details Pushed To Emarg"):"-"
        //            }
        //        }).ToArray();

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "MaintenanceAgreementDAL().EmargDAL");
        //        totalRecords = 0;
        //        return null;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        public EmargCorrectionRoadDetails GetEmargDetailsAgainstRoadCode(int EmargID)
        {
            try
            {
                PMGSYEntities dbContext = new Models.PMGSYEntities();
                // EmargID
                //RoadCode

                int RoadCode = dbContext.EMARG_ROAD_DETAILS.Where(m => m.EMARG_ID == EmargID).Select(m => m.ROAD_CODE).FirstOrDefault();


                MANE_EMARG_CONTRACT dbModel = new MANE_EMARG_CONTRACT();

                if (dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.EMARG_ID != null && m.EMARG_ID == EmargID).Any())
                {

                    MANE_IMS_CONTRACT master = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y" && m.MANE_CONTRACT_STATUS == "P").FirstOrDefault();

                    EMARG_ROAD_DETAILS emargRoadDetails = dbContext.EMARG_ROAD_DETAILS.Where(m => m.ROAD_CODE == RoadCode && m.EMARG_ID == EmargID).FirstOrDefault();

                    // IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.PLAN_CN_ROAD_CODE != null).FirstOrDefault();
                    IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode).FirstOrDefault();


                    EXEC_ROADS_MONTHLY_STATUS exec = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.EXEC_ISCOMPLETED == "C").FirstOrDefault();

                    MANE_EMARG_CONTRACT emargContractTable = dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.EMARG_ID != null && m.EMARG_ID == EmargID).FirstOrDefault();

                    MASTER_CONTRACTOR contractor = dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_ID == emargContractTable.MAST_CON_ID).FirstOrDefault();

                    if (master == null || emargRoadDetails == null || isp == null || contractor == null || exec == null || emargContractTable == null)
                    {
                        return null;
                    }

                    if (contractor.MAST_CON_PAN == null || contractor.MAST_CON_PAN == string.Empty)
                    {
                        return null;
                    }


                    EmargCorrectionRoadDetails model = null;
                    if (master != null)
                    {
                        model = new EmargCorrectionRoadDetails()
                        {
                            Emarg_ID = EmargID,
                            // Dont Display But Take Values of Following Fields
                            EncryptedRoadCode = URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode=" + master.IMS_PR_ROAD_CODE.ToString().Trim() }),
                            IMS_PR_ROAD_CODE = master.IMS_PR_ROAD_CODE,
                            MANE_PR_CONTRACT_CODE = master.MANE_PR_CONTRACT_CODE,
                            MAST_CON_ID = master.MAST_CON_ID,
                            MANE_CONTRACT_NUMBER = master.MANE_CONTRACT_NUMBER,
                            MANE_CONTRACT_ID = master.MANE_CONTRACT_ID,
                            MANE_CONTRACT_STATUS = master.MANE_CONTRACT_STATUS,
                            MANE_CONTRACT_FINALIZED = master.MANE_CONTRACT_FINALIZED,
                            MANE_LOCK_STATUS = master.MANE_LOCK_STATUS,
                            MANE_AGREEMENT_TYPE = master.MANE_AGREEMENT_TYPE,


                            // Display Text
                            StateName = emargRoadDetails.MAST_STATE_NAME,
                            DistrictName = emargRoadDetails.MAST_DISTRICT_NAME,
                            BlockName = (dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == isp.MAST_BLOCK_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault()),
                            PackageName = isp.IMS_PACKAGE_ID,
                            RoadName = isp.IMS_ROAD_NAME,

                            ContractorName = ((string.IsNullOrEmpty(contractor.MAST_CON_FNAME) ? "" : contractor.MAST_CON_FNAME.Trim()) + " " + (string.IsNullOrEmpty(contractor.MAST_CON_MNAME) ? "" : contractor.MAST_CON_MNAME.Trim()) + " " + (string.IsNullOrEmpty(contractor.MAST_CON_LNAME) ? "" : contractor.MAST_CON_LNAME.Trim())) + ", Contractor ID : " + emargContractTable.MAST_CON_ID + ", Road Code : " + master.IMS_PR_ROAD_CODE.ToString(),
                            //  ContractorName = contractor.MAST_CON_FNAME.Trim() + " " + contractor.MAST_CON_MNAME.Trim() + " " + contractor.MAST_CON_LNAME.Trim(),
                            MANE_AGREEMENT_NUMBER = string.IsNullOrEmpty(master.MANE_AGREEMENT_NUMBER) ? "NA" : master.MANE_AGREEMENT_NUMBER.Trim(),
                            PanNumberText = (string.IsNullOrEmpty(contractor.MAST_CON_PAN) ? "NA" : contractor.MAST_CON_PAN.Trim()),
                            MANE_AGREEMENT_DATE = Convert.ToString(master.MANE_AGREEMENT_DATE).Substring(0, 10),


                            // Following Fields can be Changed  // Take Fields From already added database Table i.e. from MANE_EMARG_CONTRACT
                            MANE_CONSTR_COMP_DATE = Convert.ToString(emargContractTable.MANE_CONSTR_COMP_DATE).Substring(0, 10),
                            MANE_MAINTENANCE_START_DATE = Convert.ToString(emargContractTable.MANE_MAINTENANCE_START_DATE).Substring(0, 10),
                            MANE_MAINTENANCE_END_DATE = Convert.ToString(emargContractTable.MANE_MAINTENANCE_END_DATE).Substring(0, 10),
                            MANE_YEAR1_AMOUNT = emargContractTable.MANE_YEAR1_AMOUNT,
                            MANE_YEAR2_AMOUNT = emargContractTable.MANE_YEAR2_AMOUNT,
                            MANE_YEAR3_AMOUNT = emargContractTable.MANE_YEAR3_AMOUNT,
                            MANE_YEAR4_AMOUNT = emargContractTable.MANE_YEAR4_AMOUNT,
                            MANE_YEAR5_AMOUNT = emargContractTable.MANE_YEAR5_AMOUNT,
                            MANE_YEAR6_AMOUNT = emargContractTable.MANE_YEAR6_AMOUNT,
                            MANE_VALUE_WORK_DONE = emargContractTable.MANE_VALUE_WORK_DONE,
                            MANE_COMPLETED_LENGTH = emargContractTable.MANE_COMPLETED_LENGTH,
                            TrafficeTypeCode = emargContractTable.MANE_TRAFFIC_CATEGORY,
                            CarriageWidthCode = emargContractTable.MANE_CARRIAGE_WAY,
                            SaveOrUpdate = "Update"

                        };
                    }
                    return model;
                }
                else
                {


                    MANE_IMS_CONTRACT master = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y" && m.MANE_CONTRACT_STATUS == "P").FirstOrDefault();
                    EMARG_ROAD_DETAILS emargRoadDetails = dbContext.EMARG_ROAD_DETAILS.Where(m => m.ROAD_CODE == RoadCode && m.EMARG_ID == EmargID).FirstOrDefault();

                    //IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.PLAN_CN_ROAD_CODE != null).FirstOrDefault();
                    IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode).FirstOrDefault();

                    MASTER_CONTRACTOR contractor = dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_ID == master.MAST_CON_ID).FirstOrDefault();
                    EXEC_ROADS_MONTHLY_STATUS exec = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.EXEC_ISCOMPLETED == "C").FirstOrDefault();

                    if (master == null || emargRoadDetails == null || isp == null || contractor == null || exec == null)
                    {
                        return null;
                    }
                    EmargCorrectionRoadDetails model = null;
                    if (master != null)
                    {
                        model = new EmargCorrectionRoadDetails()
                        {
                            // Dont Display But Take Values of Following Fields
                            EncryptedRoadCode = URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode=" + master.IMS_PR_ROAD_CODE.ToString().Trim() }),
                            IMS_PR_ROAD_CODE = master.IMS_PR_ROAD_CODE,
                            MANE_PR_CONTRACT_CODE = master.MANE_PR_CONTRACT_CODE,
                            MAST_CON_ID = master.MAST_CON_ID,
                            MANE_CONTRACT_NUMBER = master.MANE_CONTRACT_NUMBER,
                            MANE_CONTRACT_ID = master.MANE_CONTRACT_ID,
                            MANE_CONTRACT_STATUS = master.MANE_CONTRACT_STATUS,
                            MANE_CONTRACT_FINALIZED = master.MANE_CONTRACT_FINALIZED,
                            MANE_LOCK_STATUS = master.MANE_LOCK_STATUS,
                            MANE_AGREEMENT_TYPE = master.MANE_AGREEMENT_TYPE,


                            // Display Text
                            StateName = emargRoadDetails.MAST_STATE_NAME,
                            DistrictName = emargRoadDetails.MAST_DISTRICT_NAME,
                            BlockName = (dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == isp.MAST_BLOCK_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault()),
                            PackageName = isp.IMS_PACKAGE_ID,
                            RoadName = isp.IMS_ROAD_NAME,

                            ContractorName = (string.IsNullOrEmpty(contractor.MAST_CON_FNAME) ? "" : contractor.MAST_CON_FNAME.Trim()) + " " + (string.IsNullOrEmpty(contractor.MAST_CON_MNAME) ? "" : contractor.MAST_CON_MNAME.Trim()) + " " + (string.IsNullOrEmpty(contractor.MAST_CON_LNAME) ? "" : contractor.MAST_CON_LNAME.Trim()),
                            //  ContractorName = contractor.MAST_CON_FNAME.Trim() + " " + contractor.MAST_CON_MNAME.Trim() + " " + contractor.MAST_CON_LNAME.Trim(),
                            MANE_AGREEMENT_NUMBER = string.IsNullOrEmpty(master.MANE_AGREEMENT_NUMBER) ? "NA" : master.MANE_AGREEMENT_NUMBER.Trim(),
                            PanNumberText = string.IsNullOrEmpty(contractor.MAST_CON_PAN) ? "NA" : contractor.MAST_CON_PAN.Trim(),
                            MANE_AGREEMENT_DATE = Convert.ToString(master.MANE_AGREEMENT_DATE).Substring(0, 10),


                            // Following Fields can be Changed
                            MANE_CONSTR_COMP_DATE = Convert.ToString(master.MANE_CONSTR_COMP_DATE).Substring(0, 10),
                            MANE_MAINTENANCE_START_DATE = Convert.ToString(master.MANE_MAINTENANCE_START_DATE).Substring(0, 10),
                            MANE_MAINTENANCE_END_DATE = (master.MANE_MAINTENANCE_END_DATE == null ? Convert.ToString(master.MANE_MAINTENANCE_START_DATE).Substring(0, 10) : Convert.ToString(master.MANE_MAINTENANCE_END_DATE).Substring(0, 10)),
                            // MANE_MAINTENANCE_END_DATE = Convert.ToString(master.MANE_MAINTENANCE_END_DATE).Substring(0, 10),
                            MANE_YEAR1_AMOUNT = master.MANE_YEAR1_AMOUNT,
                            MANE_YEAR2_AMOUNT = master.MANE_YEAR2_AMOUNT,
                            MANE_YEAR3_AMOUNT = master.MANE_YEAR3_AMOUNT,
                            MANE_YEAR4_AMOUNT = master.MANE_YEAR4_AMOUNT,
                            MANE_YEAR5_AMOUNT = master.MANE_YEAR5_AMOUNT,
                            MANE_YEAR6_AMOUNT = master.MANE_YEAR6_AMOUNT,
                            MANE_VALUE_WORK_DONE = master.MANE_VALUE_WORK_DONE,
                            MANE_COMPLETED_LENGTH = exec.EXEC_COMPLETED,
                            Emarg_ID = emargRoadDetails.EMARG_ID, // newwly added
                            SaveOrUpdate = "Save"

                        };
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().GetEmargDetailsAgainstRoadCode");
                return null;
            }
            finally
            {
                //if (dbContext != null)
                //{
                //    dbContext.Dispose();
                //}
            }
        }

        public bool UpdateEmargDAL(EmargCorrectionRoadDetails emargModel, ref string message)
        {
            try
            {

                PMGSYEntities dbContext = new PMGSYEntities();
                MANE_IMS_CONTRACT maneImsContract = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == emargModel.IMS_PR_ROAD_CODE && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y" && m.MANE_CONTRACT_STATUS == "P").FirstOrDefault();
                if (maneImsContract == null)
                {
                    message = "Agreement Status is not Finalized against this Road. Details can not be saved / updated until Agreement Status is Finalized.";
                    return false;
                }

                if (dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == emargModel.IMS_PR_ROAD_CODE && m.EMARG_ID == emargModel.Emarg_ID).Any())
                { // Update Code


                    MANE_EMARG_CONTRACT modelForUpdate = dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == emargModel.IMS_PR_ROAD_CODE && m.EMARG_ID != null && m.EMARG_ID == emargModel.Emarg_ID).FirstOrDefault();
                    if (modelForUpdate != null)
                    {
                        modelForUpdate.MANE_CONSTR_COMP_DATE = Convert.ToDateTime(emargModel.MANE_CONSTR_COMP_DATE);
                        modelForUpdate.MANE_MAINTENANCE_START_DATE = Convert.ToDateTime(emargModel.MANE_MAINTENANCE_START_DATE);
                        modelForUpdate.MANE_MAINTENANCE_END_DATE = Convert.ToDateTime(emargModel.MANE_MAINTENANCE_END_DATE);

                        modelForUpdate.MANE_YEAR1_AMOUNT = emargModel.MANE_YEAR1_AMOUNT;
                        modelForUpdate.MANE_YEAR2_AMOUNT = emargModel.MANE_YEAR2_AMOUNT;
                        modelForUpdate.MANE_YEAR3_AMOUNT = emargModel.MANE_YEAR3_AMOUNT;
                        modelForUpdate.MANE_YEAR4_AMOUNT = emargModel.MANE_YEAR4_AMOUNT;
                        modelForUpdate.MANE_YEAR5_AMOUNT = emargModel.MANE_YEAR5_AMOUNT;
                        modelForUpdate.MANE_YEAR6_AMOUNT = emargModel.MANE_YEAR6_AMOUNT;

                        modelForUpdate.MANE_VALUE_WORK_DONE = emargModel.MANE_VALUE_WORK_DONE;
                        modelForUpdate.MANE_COMPLETED_LENGTH = Convert.ToDecimal(emargModel.MANE_COMPLETED_LENGTH);

                        modelForUpdate.MANE_TRAFFIC_CATEGORY = emargModel.TrafficeTypeCode;
                        modelForUpdate.MANE_CARRIAGE_WAY = emargModel.CarriageWidthCode;


                        modelForUpdate.USERID = PMGSYSession.Current.UserId;
                        modelForUpdate.IPADD = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',').Last().Trim();

                        dbContext.Entry(modelForUpdate).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        message = "Details Updated Successfully.";
                        return true;
                    }
                    else
                    {
                        message = "Details are not Updated due to some Error.";
                        return false;
                    }

                }
                else
                { // Save Code



                    #region Agreement Number and Agreement Date Validations

                    string PackageNumber = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == emargModel.IMS_PR_ROAD_CODE).Select(m => m.IMS_PACKAGE_ID).FirstOrDefault();



                    var lstTechnologyDetails = (from erd in dbContext.EMARG_ROAD_DETAILS
                                                join isp in dbContext.IMS_SANCTIONED_PROJECTS on erd.ROAD_CODE equals isp.IMS_PR_ROAD_CODE
                                                join emarg in dbContext.MANE_EMARG_CONTRACT on

                                                new { C1 = isp.IMS_PR_ROAD_CODE, C2 = erd.EMARG_ID } equals
                                                new { C1 = emarg.IMS_PR_ROAD_CODE, C2 = emarg.EMARG_ID } into details

                                                from detailrecords in details.DefaultIfEmpty()
                                                where isp.IMS_PACKAGE_ID == PackageNumber && isp.IMS_SANCTIONED == "Y" && (erd.IS_DEACTIVATED != "Y" || erd.IS_DEACTIVATED == null)
                                                select new
                                                {
                                                    isp.IMS_PR_ROAD_CODE,

                                                    isp.IMS_PACKAGE_ID

                                                }).OrderBy(m => m.IMS_PACKAGE_ID).Distinct();







                    // Check Agreement Number and Agreement Date in single package must be same
                    int RoadCodeID = emargModel.IMS_PR_ROAD_CODE;
                    string AgreementNumber = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCodeID && m.MANE_CONTRACT_STATUS == "P" && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y").Select(m => m.MANE_AGREEMENT_NUMBER).FirstOrDefault();
                    DateTime AgreementDate = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCodeID && m.MANE_CONTRACT_STATUS == "P" && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y").Select(m => m.MANE_AGREEMENT_DATE).FirstOrDefault();
                    // Check Agreement Number and Agreement Date in single package must be same

                    if (string.IsNullOrEmpty(AgreementNumber))
                    {
                        message = "Agreement Number for any of the road in this package is empty. Hence This Road Details can not be saved.";
                        return false;
                    }

                    if (AgreementDate == null)
                    {
                        message = "Agreement Date for any of the road in this package is empty. Hence This Road Details can not be saved.";
                        return false;
                    }

                    foreach (var item in lstTechnologyDetails)
                    {
                        string CurrentAgreementNumber = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && m.MANE_CONTRACT_STATUS == "P" && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y").Select(m => m.MANE_AGREEMENT_NUMBER).FirstOrDefault();

                        DateTime CurrentAgreementDate = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && m.MANE_CONTRACT_STATUS == "P" && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y").Select(m => m.MANE_AGREEMENT_DATE).FirstOrDefault();

                        if (string.IsNullOrEmpty(CurrentAgreementNumber))
                        {
                            message = "Agreement Number for any of the road in this package is empty. Hence This Road Details can not be saved.";
                            return false;
                        }

                        if (CurrentAgreementDate == null)
                        {
                            message = "Agreement Date for any of the road in this package is empty. Hence This Road Details can not be saved.";
                            return false;
                        }



                        if (CurrentAgreementNumber.Equals(AgreementNumber))
                        {
                            // Agreement Numbers for All Roads in a Single Package must be same.
                        }
                        else
                        {
                            message = "Agreement Number for all Roads in this Package is not same. Hence This Road Details can not be saved.";
                            return false;
                        }

                        if (CurrentAgreementDate == AgreementDate)
                        {
                            // Agreement Date for All Roads in a Single Package must be same.
                        }
                        else
                        {
                            message = "Agreement Date for all Roads in this Package is not same. Hence This Road details can not be saved.";
                            return false;
                        }
                    }
                    #endregion




                    MANE_EMARG_CONTRACT dbModel = new MANE_EMARG_CONTRACT();


                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Method : " + "MaintenanceAgreementDAL().UpdateEmargDAL()");

                        sw.WriteLine("dbModel.EMARG_ID :" + emargModel.Emarg_ID);
                        sw.WriteLine("dbModel.IMS_PR_ROAD_CODE :" + emargModel.IMS_PR_ROAD_CODE);
                        sw.WriteLine("dbModel.MANE_PR_CONTRACT_CODE :" + emargModel.MANE_PR_CONTRACT_CODE);
                        sw.WriteLine("dbModel.MAST_CON_ID :" + emargModel.MAST_CON_ID);

                        sw.WriteLine("dbModel.MANE_AGREEMENT_NUMBER :" + emargModel.MANE_AGREEMENT_NUMBER);
                        sw.WriteLine("dbModel.MANE_CONTRACT_NUMBER :" + emargModel.MANE_CONTRACT_NUMBER);
                        sw.WriteLine("dbModel.MANE_CONTRACT_ID :" + emargModel.MANE_CONTRACT_ID);

                        sw.WriteLine("dbModel.MANE_AGREEMENT_DATE  :" + emargModel.MANE_AGREEMENT_DATE);
                        sw.WriteLine("dbModel.MANE_CONSTR_COMP_DATE :" + emargModel.MANE_CONSTR_COMP_DATE);
                        sw.WriteLine("dbModel.MANE_MAINTENANCE_START_DATE :" + emargModel.MANE_MAINTENANCE_START_DATE);
                        sw.WriteLine("dbModel.MANE_MAINTENANCE_END_DATE :" + emargModel.MANE_MAINTENANCE_END_DATE);

                        sw.WriteLine("dbModel.MANE_YEAR1_AMOUNT  :" + emargModel.MANE_YEAR1_AMOUNT);
                        sw.WriteLine("dbModel.MANE_YEAR2_AMOUNT :" + emargModel.MANE_YEAR2_AMOUNT);
                        sw.WriteLine("dbModel.MANE_YEAR3_AMOUNT :" + emargModel.MANE_YEAR3_AMOUNT);
                        sw.WriteLine("dbModel.MANE_YEAR4_AMOUNT :" + emargModel.MANE_YEAR4_AMOUNT);
                        sw.WriteLine("dbModel.MANE_YEAR5_AMOUNT :" + emargModel.MANE_YEAR5_AMOUNT);
                        sw.WriteLine("dbModel.MANE_YEAR6_AMOUNT :" + emargModel.MANE_YEAR6_AMOUNT);

                        sw.WriteLine("dbModel.MANE_CONTRACT_STATUS  :" + emargModel.MANE_CONTRACT_STATUS);
                        sw.WriteLine("dbModel.MANE_VALUE_WORK_DONE :" + emargModel.MANE_VALUE_WORK_DONE);
                        sw.WriteLine("dbModel.MANE_CONTRACT_FINALIZED :" + emargModel.MANE_CONTRACT_FINALIZED);
                        sw.WriteLine("dbModel.MANE_LOCK_STATUS :" + emargModel.MANE_LOCK_STATUS);
                        sw.WriteLine("dbModel.MANE_AGREEMENT_TYPE :" + emargModel.MANE_AGREEMENT_TYPE);
                        sw.WriteLine("dbModel.MANE_COMPLETED_LENGTH  :" + emargModel.MANE_COMPLETED_LENGTH);
                        sw.WriteLine("dbModel.MANE_TRAFFIC_CATEGORY :" + emargModel.TrafficeTypeCode);
                        sw.WriteLine("dbModel.MANE_CARRIAGE_WAY :" + emargModel.CarriageWidthCode);
                        sw.WriteLine("PMGSYSession.Current.UserId :" + PMGSYSession.Current.UserId);


                        sw.WriteLine("-------------------------------------Save Method--------MaintenanceAgreementDAL().UpdateEmargDAL------------------------------------------");
                        sw.Close();
                    }


                    dbModel.EMARG_ID = emargModel.Emarg_ID;
                    dbModel.IMS_PR_ROAD_CODE = emargModel.IMS_PR_ROAD_CODE;
                    dbModel.MANE_PR_CONTRACT_CODE = emargModel.MANE_PR_CONTRACT_CODE;
                    dbModel.MAST_CON_ID = emargModel.MAST_CON_ID;

                    dbModel.MANE_AGREEMENT_NUMBER = emargModel.MANE_AGREEMENT_NUMBER;
                    dbModel.MANE_CONTRACT_NUMBER = emargModel.MANE_CONTRACT_NUMBER;
                    dbModel.MANE_CONTRACT_ID = emargModel.MANE_CONTRACT_ID;

                    dbModel.MANE_AGREEMENT_DATE = Convert.ToDateTime(emargModel.MANE_AGREEMENT_DATE);
                    dbModel.MANE_CONSTR_COMP_DATE = Convert.ToDateTime(emargModel.MANE_CONSTR_COMP_DATE);
                    dbModel.MANE_MAINTENANCE_START_DATE = Convert.ToDateTime(emargModel.MANE_MAINTENANCE_START_DATE);
                    dbModel.MANE_MAINTENANCE_END_DATE = Convert.ToDateTime(emargModel.MANE_MAINTENANCE_END_DATE);

                    dbModel.MANE_YEAR1_AMOUNT = emargModel.MANE_YEAR1_AMOUNT;
                    dbModel.MANE_YEAR2_AMOUNT = emargModel.MANE_YEAR2_AMOUNT;
                    dbModel.MANE_YEAR3_AMOUNT = emargModel.MANE_YEAR3_AMOUNT;
                    dbModel.MANE_YEAR4_AMOUNT = emargModel.MANE_YEAR4_AMOUNT;
                    dbModel.MANE_YEAR5_AMOUNT = emargModel.MANE_YEAR5_AMOUNT;
                    dbModel.MANE_YEAR6_AMOUNT = emargModel.MANE_YEAR6_AMOUNT;


                    dbModel.MANE_CONTRACT_STATUS = emargModel.MANE_CONTRACT_STATUS;
                    dbModel.MANE_INCOMPLETE_REASON = null;


                    dbModel.MANE_VALUE_WORK_DONE = emargModel.MANE_VALUE_WORK_DONE;
                    dbModel.MANE_CONTRACT_FINALIZED = emargModel.MANE_CONTRACT_FINALIZED;
                    dbModel.MANE_LOCK_STATUS = emargModel.MANE_LOCK_STATUS;
                    dbModel.MANE_AGREEMENT_TYPE = emargModel.MANE_AGREEMENT_TYPE;
                    dbModel.MANE_COMPLETED_LENGTH = Convert.ToDecimal(emargModel.MANE_COMPLETED_LENGTH);

                    dbModel.MANE_TRAFFIC_CATEGORY = emargModel.TrafficeTypeCode;
                    dbModel.MANE_CARRIAGE_WAY = emargModel.CarriageWidthCode;

                    dbModel.MANE_IMS_PACKAGE_FINALIZE = "N";
                    dbModel.MANE_IMS_ROAD_FINALIZE = "N";
                    dbModel.IS_PUSHED_TO_EMARG = "N";
                    dbModel.IS_DEACTIVATED = null;

                    dbModel.USERID = PMGSYSession.Current.UserId;
                    dbModel.IPADD = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',').Last().Trim();
                    dbContext.MANE_EMARG_CONTRACT.Add(dbModel);

                    dbContext.SaveChanges();
                    message = "Details Saved Successfully.";
                    return true;
                }



            }
            catch (Exception ex)
            {

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "MaintenanceAgreementDAL().UpdateEmargDAL()");

                    sw.WriteLine("dbModel.EMARG_ID :" + emargModel.Emarg_ID);
                    sw.WriteLine("dbModel.IMS_PR_ROAD_CODE :" + emargModel.IMS_PR_ROAD_CODE);
                    sw.WriteLine("dbModel.MANE_PR_CONTRACT_CODE :" + emargModel.MANE_PR_CONTRACT_CODE);
                    sw.WriteLine("dbModel.MAST_CON_ID :" + emargModel.MAST_CON_ID);

                    sw.WriteLine("dbModel.MANE_AGREEMENT_NUMBER :" + emargModel.MANE_AGREEMENT_NUMBER);
                    sw.WriteLine("dbModel.MANE_CONTRACT_NUMBER :" + emargModel.MANE_CONTRACT_NUMBER);
                    sw.WriteLine("dbModel.MANE_CONTRACT_ID :" + emargModel.MANE_CONTRACT_ID);

                    sw.WriteLine("dbModel.MANE_AGREEMENT_DATE  :" + emargModel.MANE_AGREEMENT_DATE);
                    sw.WriteLine("dbModel.MANE_CONSTR_COMP_DATE :" + emargModel.MANE_CONSTR_COMP_DATE);
                    sw.WriteLine("dbModel.MANE_MAINTENANCE_START_DATE :" + emargModel.MANE_MAINTENANCE_START_DATE);
                    sw.WriteLine("dbModel.MANE_MAINTENANCE_END_DATE :" + emargModel.MANE_MAINTENANCE_END_DATE);

                    sw.WriteLine("dbModel.MANE_YEAR1_AMOUNT  :" + emargModel.MANE_YEAR1_AMOUNT);
                    sw.WriteLine("dbModel.MANE_YEAR2_AMOUNT :" + emargModel.MANE_YEAR2_AMOUNT);
                    sw.WriteLine("dbModel.MANE_YEAR3_AMOUNT :" + emargModel.MANE_YEAR3_AMOUNT);
                    sw.WriteLine("dbModel.MANE_YEAR4_AMOUNT :" + emargModel.MANE_YEAR4_AMOUNT);
                    sw.WriteLine("dbModel.MANE_YEAR5_AMOUNT :" + emargModel.MANE_YEAR5_AMOUNT);
                    sw.WriteLine("dbModel.MANE_YEAR6_AMOUNT :" + emargModel.MANE_YEAR6_AMOUNT);

                    sw.WriteLine("dbModel.MANE_CONTRACT_STATUS  :" + emargModel.MANE_CONTRACT_STATUS);
                    sw.WriteLine("dbModel.MANE_VALUE_WORK_DONE :" + emargModel.MANE_VALUE_WORK_DONE);
                    sw.WriteLine("dbModel.MANE_CONTRACT_FINALIZED :" + emargModel.MANE_CONTRACT_FINALIZED);
                    sw.WriteLine("dbModel.MANE_LOCK_STATUS :" + emargModel.MANE_LOCK_STATUS);
                    sw.WriteLine("dbModel.MANE_AGREEMENT_TYPE :" + emargModel.MANE_AGREEMENT_TYPE);
                    sw.WriteLine("dbModel.MANE_COMPLETED_LENGTH  :" + emargModel.MANE_COMPLETED_LENGTH);
                    sw.WriteLine("dbModel.MANE_TRAFFIC_CATEGORY :" + emargModel.TrafficeTypeCode);
                    sw.WriteLine("dbModel.MANE_CARRIAGE_WAY :" + emargModel.CarriageWidthCode);
                    sw.WriteLine("PMGSYSession.Current.UserId :" + PMGSYSession.Current.UserId);


                    sw.WriteLine("-------------------------------------In Catch Exception Block Method--------MaintenanceAgreementDAL().UpdateEmargDAL------------------------------------------");
                    sw.Close();
                }
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().UpdateEmargDAL");
                return false;
            }
            finally
            {

            }
        }

        #region PostDLP

        public Array EmargDLPDAL(int? page, int? rows, string sidx, string sord, out int totalRecords, int piuCode, int MaintTypeCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (PMGSYSession.Current.LevelId == 5 || PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
                {
                    // var curSes = PMGSYSession.Current.RoleName;
                    //var code = piuCode;
                    //var dcode = PMGSYSession.Current.DistrictCode;
                    //var scode = PMGSYSession.Current.StateCode;

                    //get districtCode if it's awailable in session use that it will cover for first user else for rest of use find district code from DPIU code using ternary op

                    //find district code using DPIUcode returned on select
                    var calculatedDcodeFromDPIU = (from u in dbContext.IMS_SANCTIONED_PROJECTS
                                                   where u.MAST_DPIU_CODE == piuCode
                                                   select u.MAST_DISTRICT_CODE).FirstOrDefault();

                    //statecode req form mord user

                    var newDCode = ((PMGSYSession.Current.DistrictCode != 0) ? (PMGSYSession.Current.DistrictCode) : (calculatedDcodeFromDPIU)); //getting district code

                    var lstTechnologyDetails1 = (from erd in dbContext.EMARG_ROAD_DETAILS
                                                 join isp in dbContext.IMS_SANCTIONED_PROJECTS on erd.ROAD_CODE equals isp.IMS_PR_ROAD_CODE
                                                 join emarg in dbContext.MANE_EMARG_CONTRACT on

                                                 new { C1 = isp.IMS_PR_ROAD_CODE, C2 = erd.EMARG_ID } equals
                                                 new { C1 = emarg.IMS_PR_ROAD_CODE, C2 = emarg.EMARG_ID }



                                                  into details
                                                 //  join emarg in dbContext.MANE_EMARG_CONTRACT on erd.ROAD_CODE equals emarg.IMS_PR_ROAD_CODE into details
                                                 from detailrecords in details.DefaultIfEmpty()
                                                 where isp.MAST_DISTRICT_CODE == newDCode && isp.IMS_SANCTIONED == "Y" && erd.DLP_TYPE == MaintTypeCode
                                                       && ((PMGSYSession.Current.LevelId == 5 || PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6) ? (isp.MAST_DPIU_CODE == isp.MAST_DPIU_CODE) : (isp.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode))
                                                 select new
                                                 {
                                                     isp.IMS_PR_ROAD_CODE,
                                                     erd.MAST_STATE_NAME,
                                                     erd.MAST_DISTRICT_NAME,
                                                     isp.IMS_ROAD_NAME,
                                                     isp.IMS_PACKAGE_ID,
                                                     isp.MAST_STATE_CODE,
                                                     isp.MAST_DISTRICT_CODE,
                                                     isp.IMS_PAV_LENGTH,
                                                     detailrecords.MANE_IMS_ROAD_FINALIZE,
                                                     detailrecords.MANE_IMS_PACKAGE_FINALIZE,
                                                     detailrecords.IS_PUSHED_TO_EMARG,
                                                     erd.REJECTION_REASON,
                                                     erd.EMARG_ID,
                                                     erd.IS_DEACTIVATED
                                                 }).OrderBy(m => m.IMS_PACKAGE_ID).Distinct();

                    var lstTechnologyDetails = lstTechnologyDetails1.Where(m => m.IS_DEACTIVATED != "Y" || m.IS_DEACTIVATED == null).ToList();
                    totalRecords = lstTechnologyDetails.Count();

                    lstTechnologyDetails.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));

                    var result = lstTechnologyDetails.Select(m => new
                    {
                        m.IMS_PR_ROAD_CODE,
                        m.MAST_STATE_NAME,
                        m.MAST_DISTRICT_NAME,
                        m.IMS_ROAD_NAME,
                        m.IMS_PACKAGE_ID,
                        m.IMS_PAV_LENGTH,
                        m.MANE_IMS_ROAD_FINALIZE,
                        m.MANE_IMS_PACKAGE_FINALIZE,
                        m.IS_PUSHED_TO_EMARG,
                        m.REJECTION_REASON,
                        m.EMARG_ID
                    }).OrderBy(m => m.IMS_PACKAGE_ID).ToArray();

                    return result.Select(m => new
                    {
                        id = m.IMS_PR_ROAD_CODE.ToString(),
                        cell = new[]
                    {
                        m.IMS_PACKAGE_ID==null?string.Empty:m.IMS_PACKAGE_ID.ToString().Trim(),
                        m.IMS_ROAD_NAME == null?string.Empty:m.IMS_ROAD_NAME.ToString() +("(Emarg ID : "+m.EMARG_ID+")"),
                        m.MAST_STATE_NAME == null?string.Empty:m.MAST_STATE_NAME.ToString(),
                        m.MAST_DISTRICT_NAME == null?string.Empty:m.MAST_DISTRICT_NAME.ToString(),
                        m.IMS_PAV_LENGTH == null?string.Empty:m.IMS_PAV_LENGTH.ToString(),

                       // m.EMARG_ID==null?"Edit":"No Edit",

                        //Commented on 26-07-2022
                       //((PMGSYSession.Current.LevelId == 5)?
                       // ((m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":   "<center><table><tr><td disabled='disabled'  style='border-color:white'><span disabled='disabled' class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>"):"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>")

                       // : 
                        
                       // ((m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":   "<center><table><tr><td disabled='disabled'  style='border-color:white'><span disabled='disabled' class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='' ></span></td></tr></table></center>"):"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='' ></span></td></tr></table></center>")),

                        //Modified on 26-07-2022

                       ((PMGSYSession.Current.LevelId == 5)?
                        ((m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>": (MaintTypeCode==2 ? "<center><table><tr><td disabled='disabled'  style='border-color:white'><span disabled='disabled' class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargPostDLPCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>" :  "<center><table><tr><td disabled='disabled'  style='border-color:white'><span disabled='disabled' class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>") ): ( MaintTypeCode==2 ? "<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargPostDLPCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>"))

                        :

                        ((m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":   "<center><table><tr><td disabled='disabled'  style='border-color:white'><span disabled='disabled' class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='' ></span></td></tr></table></center>"):"<center><table><tr><td style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='' ></span></td></tr></table></center>")),
		                    
	                    
                        //(m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":   "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='' ></span></td></tr></table></center>"):"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='' ></span></td></tr></table></center>",
                       
                       

                     //   (m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString().Trim()})):URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString().Trim()}),

                        PMGSYSession.Current.LevelId == 5 ? ((m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("N")?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Road Details' onClick ='FinalizeEmargCorrectionRoad(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"):"-") : ((m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("N")?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Road Details' onClick ='' ></span></td></tr></table></center>":"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"):"-"),

                        //(m.MANE_IMS_ROAD_FINALIZE!=null)? ((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))?((m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N"))?"<span style='color:green;'>Package is Finalized</span>":"<span style='color:green;'>Package Pushed to EMARG</span>"):(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<span style='color:red;'>Road is Finalized</span>":"-")):"-",
                        (m.MANE_IMS_ROAD_FINALIZE!=null)
                        ?
                            MaintTypeCode==2   ?
                            (m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?((m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N"))?"<span style='color:green;'>Road is Finalized</span>":"<span style='color:green;'>Road Pushed to EMARG</span>"):(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<span style='color:red;'>Road is Finalized</span>":"-"))
                            :
                            ((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))?((m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N"))?"<span style='color:green;'>Package is Finalized</span>":"<span style='color:green;'>Package Pushed to EMARG</span>"):(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<span style='color:red;'>Road is Finalized</span>":"-"))
                        :
                        "-",
                        // Road Deletion or Definalization before package finalization // m.MANE_IMS_ROAD_FINALIZE.Equals("Y") &&
                        
                        PMGSYSession.Current.LevelId == 5 ? (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_PACKAGE_FINALIZE!="Y" &&(m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N")))?"<a href='#' title='Click here to definalize Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteNonPackageFinalizedRoad('" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString().Trim()}) + "'); return false;'>Delete</a>":"-"):"-"  : (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_PACKAGE_FINALIZE!="Y" &&(m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N")))?"<a href='#' title='Click here to definalize Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick=''; return false;'>Delete</a>":"-"):"-",


                        //Added on 28-07-2022
                        //MaintTypeCode==2   ? PMGSYSession.Current.LevelId == 6 ? "<input type='button' disabled='disabled' title='After finalizing  , click here to push details to Emarg.' value='Push To Emarg' onclick='PushToEmargPostDLP("+m.EMARG_ID+")'>" : "<input type='button' title='After finalizing  , click here to push details to Emarg.' value='Push To Emarg' onclick='PushToEmargPostDLP("+m.EMARG_ID+")'>":"",

                        //Modified on 23-08-2022
                        (m.MANE_IMS_ROAD_FINALIZE!=null)
                        ?   // cond 1
                        MaintTypeCode==2
                        ?   // cond 2
                        m.MANE_IMS_ROAD_FINALIZE.Equals("Y")
                        ?   // cond 3
                        (m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N"))
                        ?   // cond 4
                        (PMGSYSession.Current.LevelId == 6 ? "<input type='button' disabled='disabled' title='After finalizing  , click here to push details to Emarg.' value='Push To Emarg' onclick='PushToEmargPostDLP("+m.EMARG_ID+")'>" : "<input type='button' title='After finalizing  , click here to push details to Emarg.' value='Push To Emarg' onclick='PushToEmargPostDLP("+m.EMARG_ID+")'>")
                        :"-" // cond 4
                        :"-" // cond 3
                        :"-" // cond 2
                        :"-",// cond 1

                        (m.REJECTION_REASON!=null)?(m.REJECTION_REASON.Equals("1")?"Rejected By System":"PIU Correction Request"):"-",



                        //((PMGSYSession.Current.RoleName.Equals("PIU"))?
                        //((m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Road Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"-")):"-") 
                        //:((m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Road Details' onClick ='' ></span></td></tr></table></center>":(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Details' onClick ='' ></span></td></tr></table></center>":"-")):"-")),

                        (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Road Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"-")):"-",

                        "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Click here to Push Package details to Emarg' onClick ='PushToEmarg(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.IMS_PR_ROAD_CODE.ToString()  }) + "\");' ></span></td></tr></table></center>"
                         //(m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? (m.IS_PUSHED_TO_EMARG.Equals("N")?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='Click here to Push Package details to Emarg' onClick ='PushToEmarg(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString()  }) + "\");' ></span></td></tr></table></center>":"Details Pushed To Emarg"):"-"
                    }
                    }).ToArray();

                }
                else
                {
                    var lstTechnologyDetails1 = (from erd in dbContext.EMARG_ROAD_DETAILS
                                                 join isp in dbContext.IMS_SANCTIONED_PROJECTS on erd.ROAD_CODE equals isp.IMS_PR_ROAD_CODE
                                                 join emarg in dbContext.MANE_EMARG_CONTRACT on

                                                 new { C1 = isp.IMS_PR_ROAD_CODE, C2 = erd.EMARG_ID } equals
                                                 new { C1 = emarg.IMS_PR_ROAD_CODE, C2 = emarg.EMARG_ID }



                                                  into details
                                                 //  join emarg in dbContext.MANE_EMARG_CONTRACT on erd.ROAD_CODE equals emarg.IMS_PR_ROAD_CODE into details
                                                 from detailrecords in details.DefaultIfEmpty()
                                                 where isp.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode && isp.IMS_SANCTIONED == "Y" && erd.DLP_TYPE == MaintTypeCode
                                                           && ((PMGSYSession.Current.RoleName.Equals("PIUOA") || PMGSYSession.Current.RoleName.Equals("PIURCPLWE")) ? (isp.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode) : (isp.MAST_DPIU_CODE == isp.MAST_DPIU_CODE))
                                                 select new
                                                 {
                                                     isp.IMS_PR_ROAD_CODE,
                                                     erd.MAST_STATE_NAME,
                                                     erd.MAST_DISTRICT_NAME,
                                                     isp.IMS_ROAD_NAME,
                                                     isp.IMS_PACKAGE_ID,
                                                     isp.MAST_STATE_CODE,
                                                     isp.MAST_DISTRICT_CODE,
                                                     isp.IMS_PAV_LENGTH,
                                                     detailrecords.MANE_IMS_ROAD_FINALIZE,
                                                     detailrecords.MANE_IMS_PACKAGE_FINALIZE,
                                                     detailrecords.IS_PUSHED_TO_EMARG,
                                                     erd.REJECTION_REASON,
                                                     erd.EMARG_ID,
                                                     erd.IS_DEACTIVATED
                                                 }).OrderBy(m => m.IMS_PACKAGE_ID).Distinct();

                    var lstTechnologyDetails = lstTechnologyDetails1.Where(m => m.IS_DEACTIVATED != "Y" || m.IS_DEACTIVATED == null).ToList();
                    totalRecords = lstTechnologyDetails.Count();

                    lstTechnologyDetails.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));

                    var result = lstTechnologyDetails.Select(m => new
                    {
                        m.IMS_PR_ROAD_CODE,
                        m.MAST_STATE_NAME,
                        m.MAST_DISTRICT_NAME,
                        m.IMS_ROAD_NAME,
                        m.IMS_PACKAGE_ID,
                        m.IMS_PAV_LENGTH,
                        m.MANE_IMS_ROAD_FINALIZE,
                        m.MANE_IMS_PACKAGE_FINALIZE,
                        m.IS_PUSHED_TO_EMARG,
                        m.REJECTION_REASON,
                        m.EMARG_ID
                    }).OrderBy(m => m.IMS_PACKAGE_ID).ToArray();

                    return result.Select(m => new
                    {
                        id = m.IMS_PR_ROAD_CODE.ToString(),
                        cell = new[]
                    {
                        m.IMS_PACKAGE_ID==null?string.Empty:m.IMS_PACKAGE_ID.ToString().Trim(),
                        m.IMS_ROAD_NAME == null?string.Empty:m.IMS_ROAD_NAME.ToString() +("(Emarg ID : "+m.EMARG_ID+")"),
                        m.MAST_STATE_NAME == null?string.Empty:m.MAST_STATE_NAME.ToString(),
                        m.MAST_DISTRICT_NAME == null?string.Empty:m.MAST_DISTRICT_NAME.ToString(),
                        m.IMS_PAV_LENGTH == null?string.Empty:m.IMS_PAV_LENGTH.ToString(),

                       // m.EMARG_ID==null?"Edit":"No Edit",

                        (m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":   "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>"):"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Add / Edit Road Details' onClick ='AddorEditEmargCorrectionDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>",

                     //   (m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString().Trim()})):URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString().Trim()}),

                        (m.MANE_IMS_ROAD_FINALIZE!=null)?(m.MANE_IMS_ROAD_FINALIZE.Equals("N")?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Road Details' onClick ='FinalizeEmargCorrectionRoad(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"):"-",

                        (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))?((m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N"))?"<span style='color:green;'>Package is Finalized</span>":"<span style='color:green;'>Package Pushed to EMARG</span>"):(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")?"<span style='color:red;'>Road is Finalized</span>":"-")):"-",

                        // Road Deletion or Definalization before package finalization // m.MANE_IMS_ROAD_FINALIZE.Equals("Y") &&
                        (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_PACKAGE_FINALIZE!="Y" &&(m.IS_PUSHED_TO_EMARG==null||m.IS_PUSHED_TO_EMARG.Equals("N")))?"<a href='#' title='Click here to definalize Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteNonPackageFinalizedRoad('" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString().Trim()}) + "'); return false;'>Delete</a>":"-"):"-",

                        (m.REJECTION_REASON!=null)?(m.REJECTION_REASON.Equals("1")?"Rejected By System":"PIU Correction Request"):"-",

                        (m.MANE_IMS_ROAD_FINALIZE!=null)?((m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Road Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":(m.MANE_IMS_ROAD_FINALIZE.Equals("Y")? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='View Details' onClick ='ViewDetailsAfterCorrection(\"" + URLEncrypt.EncryptParameters1(new string[]{"EmargID="+m.EMARG_ID.ToString()  }) + "\");' ></span></td></tr></table></center>":"-")):"-",

                        "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plusthick ui-align-center' title='Click here to Push Package details to Emarg' onClick ='PushToEmarg(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.IMS_PR_ROAD_CODE.ToString()  }) + "\");' ></span></td></tr></table></center>"
                         //(m.MANE_IMS_ROAD_FINALIZE.Equals("Y") && m.MANE_IMS_PACKAGE_FINALIZE.Equals("Y"))? (m.IS_PUSHED_TO_EMARG.Equals("N")?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin ui-align-center' title='Click here to Push Package details to Emarg' onClick ='PushToEmarg(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+m.ROAD_CODE.ToString()  }) + "\");' ></span></td></tr></table></center>":"Details Pushed To Emarg"):"-"
                    }
                    }).ToArray();
                }


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().EmargDLPDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public EmargCorrectionRoadDetailsPostDLP GetEmargDetailsAgainstRoadCodePostDLP(int EmargID)
        {
            try
            {
                PMGSYEntities dbContext = new Models.PMGSYEntities();
                // EmargID
                //RoadCode

                int RoadCode = dbContext.EMARG_ROAD_DETAILS.Where(m => m.EMARG_ID == EmargID).Select(m => m.ROAD_CODE).FirstOrDefault();


                MANE_EMARG_CONTRACT dbModel = new MANE_EMARG_CONTRACT();


                if (dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.EMARG_ID != null && m.EMARG_ID == EmargID).Any())
                {

                    MANE_IMS_CONTRACT master = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y" && m.MANE_CONTRACT_STATUS == "P").FirstOrDefault();

                    MANE_EMARG_CONTRACT emargMaster = dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.EMARG_ID != null && m.EMARG_ID == EmargID).FirstOrDefault();

                    //EMARG_CORRECTION_ROAD_DETAILS_Result EmargRoadDetails = dbContext.EMARG_CORRECTION_ROAD_DETAILS(EmargID).ToList<EMARG_CORRECTION_ROAD_DETAILS_Result>().FirstOrDefault();
                    USP_INSERT_POST_DLP_RECORDS_Result EmargRoadDetails = dbContext.USP_INSERT_POST_DLP_RECORDS(EmargID).ToList<USP_INSERT_POST_DLP_RECORDS_Result>().FirstOrDefault();

                    EmargCorrectionRoadDetailsPostDLP model = null;
                    if (master != null)
                    {
                        model = new EmargCorrectionRoadDetailsPostDLP()
                        {
                            Emarg_ID = EmargID,
                            // Dont Display But Take Values of Following Fields
                            EncryptedRoadCode = URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode=" + master.IMS_PR_ROAD_CODE.ToString().Trim() }),
                            IMS_PR_ROAD_CODE = master.IMS_PR_ROAD_CODE,
                            MANE_PR_CONTRACT_CODE = master.MANE_PR_CONTRACT_CODE,
                            MAST_CON_ID = master.MAST_CON_ID,
                            MANE_CONTRACT_NUMBER = master.MANE_CONTRACT_NUMBER,
                            MANE_CONTRACT_ID = master.MANE_CONTRACT_ID,
                            MANE_CONTRACT_STATUS = master.MANE_CONTRACT_STATUS,
                            MANE_CONTRACT_FINALIZED = master.MANE_CONTRACT_FINALIZED,
                            MANE_LOCK_STATUS = master.MANE_LOCK_STATUS,
                            MANE_AGREEMENT_TYPE = master.MANE_AGREEMENT_TYPE,

                            // Display Text
                            StateName = EmargRoadDetails.MAST_STATE_NAME,
                            DistrictName = EmargRoadDetails.MAST_DISTRICT_NAME,
                            BlockName = (dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == EmargRoadDetails.MAST_BLOCK_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault()),
                            PackageName = EmargRoadDetails.IMS_PACKAGE_ID,
                            RoadName = EmargRoadDetails.IMS_ROAD_NAME,

                            // ContractorName = ((string.IsNullOrEmpty(contractor.MAST_CON_FNAME) ? "" : contractor.MAST_CON_FNAME.Trim()) + " " + (string.IsNullOrEmpty(contractor.MAST_CON_MNAME) ? "" : contractor.MAST_CON_MNAME.Trim()) + " " + (string.IsNullOrEmpty(contractor.MAST_CON_LNAME) ? "" : contractor.MAST_CON_LNAME.Trim())) + ", Contractor ID : " + emargContractTable.MAST_CON_ID + ", Road Code : " + master.IMS_PR_ROAD_CODE.ToString(),
                            // //  ContractorName = contractor.MAST_CON_FNAME.Trim() + " " + contractor.MAST_CON_MNAME.Trim() + " " + contractor.MAST_CON_LNAME.Trim(),
                            MANE_AGREEMENT_NUMBER = string.IsNullOrEmpty(master.MANE_AGREEMENT_NUMBER) ? "NA" : master.MANE_AGREEMENT_NUMBER.Trim(),
                            // PanNumberText = (string.IsNullOrEmpty(contractor.MAST_CON_PAN) ? "NA" : contractor.MAST_CON_PAN.Trim()),

                            // Following Fields can be Changed  // Take Fields From already added database Table i.e. from MANE_EMARG_CONTRACT

                            MANE_YEAR1_AMOUNT = emargMaster.MANE_YEAR1_AMOUNT,
                            MANE_YEAR2_AMOUNT = emargMaster.MANE_YEAR2_AMOUNT,
                            MANE_YEAR3_AMOUNT = emargMaster.MANE_YEAR3_AMOUNT,
                            MANE_YEAR4_AMOUNT = emargMaster.MANE_YEAR4_AMOUNT,
                            MANE_YEAR5_AMOUNT = emargMaster.MANE_YEAR5_AMOUNT,
                            MANE_YEAR6_AMOUNT = emargMaster.MANE_YEAR6_AMOUNT,
                            MANE_VALUE_WORK_DONE = emargMaster.MANE_VALUE_WORK_DONE,
                            MANE_COMPLETED_LENGTH = emargMaster.MANE_COMPLETED_LENGTH,
                            TrafficeTypeCode = emargMaster.MANE_TRAFFIC_CATEGORY,
                            CarriageWidthCode = emargMaster.MANE_CARRIAGE_WAY,
                            SaveOrUpdate = "Update"

                        };
                    }
                    return model;
                }
                else
                {


                    MANE_IMS_CONTRACT master = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y" && m.MANE_CONTRACT_STATUS == "P").FirstOrDefault();
                    EMARG_ROAD_DETAILS emargRoadDetails = dbContext.EMARG_ROAD_DETAILS.Where(m => m.ROAD_CODE == RoadCode && m.EMARG_ID == EmargID).FirstOrDefault();

                    IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode).FirstOrDefault();

                    EXEC_ROADS_MONTHLY_STATUS exec = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode && m.EXEC_ISCOMPLETED == "C").FirstOrDefault();


                    //EMARG_CORRECTION_ROAD_DETAILS_Result EmargRoadDetails = dbContext.EMARG_CORRECTION_ROAD_DETAILS(EmargID).ToList<EMARG_CORRECTION_ROAD_DETAILS_Result>().FirstOrDefault();
                    //USP_INSERT_POST_DLP_RECORDS_Result EmargRoadDetails = dbContext.USP_INSERT_POST_DLP_RECORDS(EmargID).ToList<USP_INSERT_POST_DLP_RECORDS_Result>().FirstOrDefault();

                    EmargCorrectionRoadDetailsPostDLP model = null;

                    if (master == null || emargRoadDetails == null || isp == null || exec == null)
                    {
                        return null;
                    }

                    if (master != null)
                    {
                        model = new EmargCorrectionRoadDetailsPostDLP()
                        {
                            // Dont Display But Take Values of Following Fields
                            EncryptedRoadCode = URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode=" + master.IMS_PR_ROAD_CODE.ToString().Trim() }),
                            IMS_PR_ROAD_CODE = master.IMS_PR_ROAD_CODE,
                            MANE_PR_CONTRACT_CODE = master.MANE_PR_CONTRACT_CODE,
                            MAST_CON_ID = master.MAST_CON_ID,
                            MANE_CONTRACT_NUMBER = master.MANE_CONTRACT_NUMBER,
                            MANE_CONTRACT_ID = master.MANE_CONTRACT_ID,
                            MANE_CONTRACT_STATUS = master.MANE_CONTRACT_STATUS,
                            MANE_CONTRACT_FINALIZED = master.MANE_CONTRACT_FINALIZED,
                            MANE_LOCK_STATUS = master.MANE_LOCK_STATUS,
                            MANE_AGREEMENT_TYPE = master.MANE_AGREEMENT_TYPE,

                            // Display Text
                            StateName = emargRoadDetails.MAST_STATE_NAME,
                            DistrictName = emargRoadDetails.MAST_DISTRICT_NAME,
                            BlockName = (dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == isp.MAST_BLOCK_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault()),
                            PackageName = isp.IMS_PACKAGE_ID,
                            RoadName = isp.IMS_ROAD_NAME,

                            //ContractorName = (string.IsNullOrEmpty(contractor.MAST_CON_FNAME) ? "" : contractor.MAST_CON_FNAME.Trim()) + " " + (string.IsNullOrEmpty(contractor.MAST_CON_MNAME) ? "" : contractor.MAST_CON_MNAME.Trim()) + " " + (string.IsNullOrEmpty(contractor.MAST_CON_LNAME) ? "" : contractor.MAST_CON_LNAME.Trim()),
                            // //  ContractorName = contractor.MAST_CON_FNAME.Trim() + " " + contractor.MAST_CON_MNAME.Trim() + " " + contractor.MAST_CON_LNAME.Trim(),
                            MANE_AGREEMENT_NUMBER = string.IsNullOrEmpty(master.MANE_AGREEMENT_NUMBER) ? "NA" : master.MANE_AGREEMENT_NUMBER.Trim(),
                            // PanNumberText = string.IsNullOrEmpty(contractor.MAST_CON_PAN) ? "NA" : contractor.MAST_CON_PAN.Trim(),

                            // MANE_MAINTENANCE_END_DATE = Convert.ToString(master.MANE_MAINTENANCE_END_DATE).Substring(0, 10),
                            MANE_YEAR1_AMOUNT = master.MANE_YEAR1_AMOUNT,
                            MANE_YEAR2_AMOUNT = master.MANE_YEAR1_AMOUNT,
                            MANE_YEAR3_AMOUNT = master.MANE_YEAR1_AMOUNT,
                            MANE_YEAR4_AMOUNT = master.MANE_YEAR1_AMOUNT,
                            MANE_YEAR5_AMOUNT = master.MANE_YEAR1_AMOUNT,
                            MANE_YEAR6_AMOUNT = master.MANE_YEAR1_AMOUNT,
                            //MANE_VALUE_WORK_DONE = EmargRoadDetails.MANE_VALUE_WORK_DONE,
                            TrafficeTypeCode = isp.IMS_TRAFFIC_TYPE == null ? null : isp.IMS_TRAFFIC_TYPE,
                            CarriageWidthCode = isp.IMS_CARRIAGED_WIDTH == null ? null : isp.IMS_CARRIAGED_WIDTH,
                            MANE_COMPLETED_LENGTH = exec.EXEC_COMPLETED,
                            MANE_CONSTR_COMP_DATE = Convert.ToString(exec.EXEC_COMPLETION_DATE).Split(' ')[0],
                            Emarg_ID = emargRoadDetails.EMARG_ID, // newwly added
                            SaveOrUpdate = "Save"

                        };
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().GetEmargDetailsAgainstRoadCodePostDLP");
                return null;
            }
            finally
            {
                //if (dbContext != null)
                //{
                //    dbContext.Dispose();
                //}
            }
        }

        public bool UpdateEmargPostDLPDAL(EmargCorrectionRoadDetailsPostDLP emargModel, ref string message)
        {
            try
            {

                PMGSYEntities dbContext = new PMGSYEntities();
                //MANE_IMS_CONTRACT maneImsContract = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == emargModel.IMS_PR_ROAD_CODE && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y" && m.MANE_CONTRACT_STATUS == "P").FirstOrDefault();


                if (dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == emargModel.IMS_PR_ROAD_CODE && m.EMARG_ID == emargModel.Emarg_ID).Any())
                { // Update Code


                    MANE_EMARG_CONTRACT modelForUpdate = dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == emargModel.IMS_PR_ROAD_CODE && m.EMARG_ID != null && m.EMARG_ID == emargModel.Emarg_ID).FirstOrDefault();
                    if (modelForUpdate != null)
                    {
                        //modelForUpdate.MANE_CONSTR_COMP_DATE = Convert.ToDateTime(emargModel.MANE_CONSTR_COMP_DATE);
                        if (emargModel.MANE_CONSTR_COMP_DATE == null)
                        {
                            modelForUpdate.MANE_CONSTR_COMP_DATE = null;
                        }
                        else
                        {
                            modelForUpdate.MANE_CONSTR_COMP_DATE = Convert.ToDateTime(emargModel.MANE_CONSTR_COMP_DATE);

                        }
                        modelForUpdate.MANE_MAINTENANCE_START_DATE = null;
                        modelForUpdate.MANE_MAINTENANCE_END_DATE = null;

                        modelForUpdate.MANE_YEAR1_AMOUNT = null;
                        modelForUpdate.MANE_YEAR2_AMOUNT = null;
                        modelForUpdate.MANE_YEAR3_AMOUNT = null;
                        modelForUpdate.MANE_YEAR4_AMOUNT = null;
                        modelForUpdate.MANE_YEAR5_AMOUNT = null;
                        modelForUpdate.MANE_YEAR6_AMOUNT = null;

                        modelForUpdate.MANE_VALUE_WORK_DONE = null;
                        modelForUpdate.MANE_COMPLETED_LENGTH = Convert.ToDecimal(emargModel.MANE_COMPLETED_LENGTH);

                        modelForUpdate.MANE_TRAFFIC_CATEGORY = emargModel.TrafficeTypeCode.Value;
                        modelForUpdate.MANE_CARRIAGE_WAY = emargModel.CarriageWidthCode.Value;


                        modelForUpdate.USERID = PMGSYSession.Current.UserId;
                        modelForUpdate.IPADD = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',').Last().Trim();

                        dbContext.Entry(modelForUpdate).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        message = "Details Updated Successfully.";
                        return true;
                    }
                    else
                    {
                        message = "Details are not Updated due to some Error.";
                        return false;
                    }

                }
                else
                { // Save Code



                    MANE_EMARG_CONTRACT dbModel = new MANE_EMARG_CONTRACT();



                    dbModel.EMARG_ID = emargModel.Emarg_ID;
                    dbModel.IMS_PR_ROAD_CODE = emargModel.IMS_PR_ROAD_CODE;
                    dbModel.MANE_PR_CONTRACT_CODE = emargModel.MANE_PR_CONTRACT_CODE;
                    dbModel.MAST_CON_ID = emargModel.MAST_CON_ID;

                    dbModel.MANE_AGREEMENT_NUMBER = null;
                    dbModel.MANE_CONTRACT_NUMBER = null;
                    dbModel.MANE_CONTRACT_ID = null;

                    dbModel.MANE_AGREEMENT_DATE = null;
                    if (emargModel.MANE_CONSTR_COMP_DATE == null)
                    {
                        dbModel.MANE_CONSTR_COMP_DATE = null;
                    }
                    else
                    {
                        dbModel.MANE_CONSTR_COMP_DATE = Convert.ToDateTime(emargModel.MANE_CONSTR_COMP_DATE);
                    }
                    dbModel.MANE_MAINTENANCE_START_DATE = null;
                    dbModel.MANE_MAINTENANCE_END_DATE = null;

                    dbModel.MANE_YEAR1_AMOUNT = null;
                    dbModel.MANE_YEAR2_AMOUNT = null;
                    dbModel.MANE_YEAR3_AMOUNT = null;
                    dbModel.MANE_YEAR4_AMOUNT = null;
                    dbModel.MANE_YEAR5_AMOUNT = null;
                    dbModel.MANE_YEAR6_AMOUNT = null;


                    dbModel.MANE_CONTRACT_STATUS = null;
                    dbModel.MANE_INCOMPLETE_REASON = null;


                    dbModel.MANE_VALUE_WORK_DONE = null;
                    dbModel.MANE_CONTRACT_FINALIZED = null;
                    dbModel.MANE_LOCK_STATUS = null;
                    dbModel.MANE_AGREEMENT_TYPE = null;
                    dbModel.MANE_COMPLETED_LENGTH = Convert.ToDecimal(emargModel.MANE_COMPLETED_LENGTH);

                    dbModel.MANE_TRAFFIC_CATEGORY = emargModel.TrafficeTypeCode.Value;
                    dbModel.MANE_CARRIAGE_WAY = emargModel.CarriageWidthCode.Value;

                    dbModel.MANE_IMS_PACKAGE_FINALIZE = "N";
                    dbModel.MANE_IMS_ROAD_FINALIZE = "N";
                    dbModel.IS_PUSHED_TO_EMARG = "N";
                    dbModel.IS_DEACTIVATED = null;

                    dbModel.USERID = PMGSYSession.Current.UserId;
                    dbModel.IPADD = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',').Last().Trim();
                    dbContext.MANE_EMARG_CONTRACT.Add(dbModel);

                    dbContext.SaveChanges();
                    message = "Details Saved Successfully.";
                    return true;
                }



            }
            catch (Exception ex)
            {

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "MaintenanceAgreementDAL().UpdateEmargPostDLPDAL()");


                    sw.WriteLine("-------------------------------------In Catch Exception Block Method--------MaintenanceAgreementDAL().UpdateEmargPostDLPDAL------------------------------------------");
                    sw.Close();
                }
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().UpdateEmargPostDLPDAL");
                return false;
            }
            finally
            {

            }
        }

        public bool FinalizeEmargRoadPostDLPDAL(int EmargID, out string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            MANE_EMARG_CONTRACT IMSContract = null;
            // EmargID

            // IMSPRRoadCode
            int IMSPRRoadCode = dbContext.EMARG_ROAD_DETAILS.Where(m => m.EMARG_ID == EmargID).Select(m => m.ROAD_CODE).FirstOrDefault();
            try
            {



                #region Sanctioned Length Validations

                // Check Sanctioned Length is not null or not 0
                // IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode && m.PLAN_CN_ROAD_CODE != null).FirstOrDefault();

                IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();


                decimal SanctinedLength = isp.IMS_PAV_LENGTH;
                if (SanctinedLength == 0 || SanctinedLength == null)
                {
                    message = "Snactioned Length can not be 0 or Empty. Hence Road Details can not be finalized.";
                    return false;
                }
                #endregion

                #region Finalize Code

                using (var scope = new TransactionScope())
                {
                    IMSContract = dbContext.MANE_EMARG_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.EMARG_ID != null && c.EMARG_ID == EmargID).FirstOrDefault();

                    if (IMSContract == null)
                    {
                        message = "Road Details can not be finalized...";
                        return false;
                    }

                    IMSContract.MANE_IMS_ROAD_FINALIZE = "Y";
                    IMSContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    IMSContract.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    scope.Complete();
                }
                message = "Road Details finalized successfully.";
                return true;

                #endregion Finalize Code

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().FinalizeEmargRoadPostDLPDAL");
                message = "Error occured while finalizing Road Details";
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool PushPackageToEmargPostDLP(int EmargId, out string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            MANE_EMARG_CONTRACT IMSContract = null;

            try
            {
                using (var scope = new TransactionScope())
                {
                    EMARG_COMPLETED_WORK_DETAILS_SERVICE tableObject = new EMARG_COMPLETED_WORK_DETAILS_SERVICE();
                    int DistrictCode = PMGSYSession.Current.DistrictCode;
                    int StateCode = PMGSYSession.Current.StateCode;

                    USP_INSERT_POST_DLP_RECORDS_Result emargStats = dbContext.USP_INSERT_POST_DLP_RECORDS(EmargId).ToList<USP_INSERT_POST_DLP_RECORDS_Result>().FirstOrDefault();

                    //// To Compare Purpose Only
                    //var eStatsToCompare = dbContext.USP_CORRECTED_ROAD_DETAIL_FOR_MAINTENANCE(StateCode, DistrictCode).ToList();
                    //var emargStatsToCompare = eStatsToCompare.Where(m => m.EMARG_ID == EmargId).ToList().FirstOrDefault();



                    //var eStats = dbContext.USP_CORRECTED_ROAD_DETAIL_FOR_MAINTENANCE(StateCode, 0).ToList(); // To Push all roads in a single package across different district at once 
                    ////var emargStats = eStats.Where(m => m.PACKAGE_NO == PackageID).ToList();
                    //var emargStats = eStats.Where(m => m.EMARG_ID == EmargId).ToList().FirstOrDefault();


                    if (!dbContext.MANE_EMARG_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == emargStats.IMS_PR_ROAD_CODE && c.EMARG_ID == EmargId && c.MANE_IMS_ROAD_FINALIZE == "Y" && (c.IS_DEACTIVATED != "Y" || c.IS_DEACTIVATED == null)).Any())
                    {
                        message = "Road must be finalized to push into Emarg";
                        return false;
                    }

                    #region Duplicate Check
                    // To avoide duplicate records(duplicate Emarg ID) saving in EMARG_COMPLETED_WORK_DETAILS_SERVICE

                    if (dbContext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Where(m => m.EMARG_ID == emargStats.EMARG_ID).Any())
                    {
                        message = "This Road is already pushed to Emarg";
                        return false;
                    }


                    #endregion


                    IMSContract = dbContext.MANE_EMARG_CONTRACT.Where(c => c.EMARG_ID == emargStats.EMARG_ID).FirstOrDefault();
                    IMSContract.IS_PUSHED_TO_EMARG = "Y";
                    IMSContract.PUSHED_DATE_TO_EMARG = System.DateTime.Now;
                    IMSContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    IMSContract.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();



                    EMARG_ROAD_DETAILS update = dbContext.EMARG_ROAD_DETAILS.Where(m => m.EMARG_ID == emargStats.EMARG_ID).FirstOrDefault();

                    update.DATE_OF_REPUSHING = System.DateTime.Now;
                    update.OMMAS_REPUSHING_STATUS = "1";
                    update.EMARG_STATUS = "Y";
                    dbContext.Entry(update).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();





                    if (dbContext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Where(m => m.EMARG_ID == emargStats.EMARG_ID).Any())
                    {

                        //DO NOT INSERT SAME EMARG ID RECORD

                    }
                    else
                    {
                        // tableObject.EID = (dbContext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Any() ? (dbContext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Max(m=>m.EID)+1) : 1);//temValue.STATE_NAME;
                        tableObject.EMARG_ID = emargStats.EMARG_ID;
                        tableObject.MAST_STATE_CODE = emargStats.MAST_STATE_CODE;
                        tableObject.MAST_STATE_NAME = emargStats.MAST_STATE_NAME;
                        tableObject.PMGSY_SCHEME = emargStats.MAST_PMGSY_SCHEME;

                        tableObject.MAST_DISTRICT_CODE = emargStats.MAST_DISTRICT_CODE;
                        tableObject.MAST_DISTRICT_NAME = emargStats.MAST_DISTRICT_NAME;
                        tableObject.MAST_BLOCK_CODE = emargStats.MAST_BLOCK_CODE;
                        tableObject.MAST_BLOCK_NAME = (emargStats.MAST_BLOCK_CODE != null || emargStats.MAST_BLOCK_CODE != 0) ? dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == emargStats.MAST_BLOCK_CODE).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault().ToString() : null;
                        tableObject.PIU_CODE = emargStats.MAST_DPIU_CODE;
                        tableObject.PIU_NAME = emargStats.MAST_DPIU_CODE != null ? dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == emargStats.MAST_DPIU_CODE).Select(x => x.ADMIN_ND_NAME).FirstOrDefault().ToString() : null;
                        tableObject.ROAD_CODE = emargStats.IMS_PR_ROAD_CODE;
                        tableObject.SANCTION_YEAR = emargStats.IMS_YEAR.ToString() + "-" + (emargStats.IMS_YEAR + 1).ToString();
                        tableObject.SANCTION_BATCH = emargStats.IMS_BATCH;
                        tableObject.PACKAGE_NO = emargStats.IMS_PACKAGE_ID;


                        tableObject.ROAD_NAME = emargStats.IMS_ROAD_NAME;
                        tableObject.SANCTION_LENGTH = emargStats.IMS_PAV_LENGTH;
                        tableObject.COMPLETED_LENGTH = emargStats.MANE_COMPLETED_LENGTH;
                        tableObject.CC_LENGTH = null;
                        tableObject.BT_LENGTH = null;
                        tableObject.CDWorks = null;
                        tableObject.CN_CODE = emargStats.PLAN_CN_ROAD_CODE != null ? dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == emargStats.PLAN_CN_ROAD_CODE).Select(x => x.PLAN_CN_ROAD_NUMBER).FirstOrDefault().ToString() : null;
                        tableObject.TRAFFIC_CATEGORY = emargStats.MAST_TRAFFIC_NAME;


                        tableObject.CARRIAGE_WAY_WIDTH = emargStats.MAST_CARRIAGE_WIDTH;
                        tableObject.STAGE = emargStats.IMS_STAGE_PHASE;
                        tableObject.COMPLETION_DATE = emargStats.MANE_CONSTR_COMP_DATE;//itemValue.COMPLETION_DATE;

                        tableObject.WORK_ORDER_NO = null;
                        tableObject.WORK_ORDER_DATE = null;

                        tableObject.CONTRACTOR_NAME = null;
                        tableObject.CONTRACTOR_PAN = null;
                        tableObject.CONTRACTOR_ID = null;

                        tableObject.IS_PUSHED_TO_EMARG = "Y";
                        tableObject.PUSHED_DATE_TO_EMARG = System.DateTime.Now;
                        tableObject.USERID = PMGSYSession.Current.UserId;
                        tableObject.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Add(tableObject);
                        dbContext.SaveChanges();
                    }

                    dbContext.SaveChanges();
                    scope.Complete();

                }
                message = "This package is pushed to Emarg Successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().PushPackageToEmargPostDLP()");
                message = "This package is NOT pushed to Emarg.";
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        public bool FinalizeEmargRoadDAL(int EmargID, out string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            MANE_EMARG_CONTRACT IMSContract = null;
            // EmargID

            // IMSPRRoadCode
            int IMSPRRoadCode = dbContext.EMARG_ROAD_DETAILS.Where(m => m.EMARG_ID == EmargID).Select(m => m.ROAD_CODE).FirstOrDefault();
            try
            {
                #region PAN Validations
                //Check PAN Validations 
                MANE_IMS_CONTRACT master = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y" && m.MANE_CONTRACT_STATUS == "P").FirstOrDefault();
                MASTER_CONTRACTOR contractor = dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_ID == master.MAST_CON_ID).FirstOrDefault();
                if (string.IsNullOrEmpty(contractor.MAST_CON_PAN))
                {
                    message = "PAN Number is not available. Hence Road Details can not be finalized.";
                    return false;
                }
                else if (contractor.MAST_CON_PAN.Trim().Length != 10)
                {
                    message = contractor.MAST_CON_PAN.Trim() + " This PAN Number is not in valid Format. Valid PAN Length is 10 characters. Hence Road Details can not be finalized.";
                    return false;
                }
                else
                {
                    string ValidPanID = contractor.MAST_CON_PAN.Trim();

                    Regex regex = new Regex("([A-Z]){5}([0-9]){4}([A-Z]){1}$");
                    if (!regex.IsMatch(ValidPanID))
                    {
                        message = ValidPanID + " This PAN Number is not in correct format. Hence Road Details can not be finalized.";
                        return false;
                    }


                    //MANE_IMS_CONTRACT flag MANE_CONTRACT_STATUS
                    //I = Incompleted (i.e. Terminated)
                    //C= Completed
                    //P=In Progress
                    string packageID = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(m => m.IMS_PACKAGE_ID).FirstOrDefault();
                    List<Int32> RoadCodeList = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PACKAGE_ID == packageID && m.IMS_SANCTIONED == "Y" && m.IMS_PROPOSAL_TYPE == "P").Select(m => m.IMS_PR_ROAD_CODE).ToList<Int32>();

                    List<Int32> ContractorIDList = dbContext.MANE_IMS_CONTRACT.Where(m => RoadCodeList.Contains(m.IMS_PR_ROAD_CODE) && m.MANE_CONTRACT_STATUS == "P" && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y").Select(m => m.MAST_CON_ID).Distinct().ToList<Int32>();

                    //var contractor1 = dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_PAN == ValidPanID).ToList();
                    //if (contractor1.Count > 1)
                    //{// 
                    //    message = ValidPanID + " This PAN Number is mapped to more than one Contractor. Hence Road Details can not be finalized.";
                    //    return false;
                    //}


                    if (ContractorIDList.Count > 1)
                    {// 
                        message = "Within a Package, multiple Contractors can not be allowed. Hence This work can not be finalized.";
                        return false;
                    }
                }



                #endregion PAN Validations

                #region Agreement Validations
                MANE_IMS_CONTRACT master1 = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y" && m.MANE_CONTRACT_STATUS == "P").FirstOrDefault();
                string AgreementNumber = master.MANE_AGREEMENT_NUMBER.Trim();
                string AgreementDate = Convert.ToString(master.MANE_AGREEMENT_DATE).Substring(0, 10);
                if (string.IsNullOrEmpty(AgreementNumber))
                {
                    message = "Agreement Number is not available. Hence Road Details can not be finalized.";
                    return false;
                }
                else if (string.IsNullOrEmpty(AgreementDate))
                {
                    message = "Agreement Date is not available. Hence Road Details can not be finalized.";
                    return false;
                }
                #endregion Agreement Validations

                #region Sanctioned Length Validations

                // Check Sanctioned Length is not null or not 0
                // IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode && m.PLAN_CN_ROAD_CODE != null).FirstOrDefault();

                IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();


                decimal SanctinedLength = isp.IMS_PAV_LENGTH;
                if (SanctinedLength == 0 || SanctinedLength == null)
                {
                    message = "Snactioned Length can not be 0 or Empty. Hence Road Details can not be finalized.";
                    return false;
                }
                #endregion

                #region Finalize Code

                using (var scope = new TransactionScope())
                {
                    IMSContract = dbContext.MANE_EMARG_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == IMSPRRoadCode && c.EMARG_ID != null && c.EMARG_ID == EmargID).FirstOrDefault();

                    if (IMSContract == null)
                    {
                        message = "Road Details can not be finalized...";
                        return false;
                    }

                    IMSContract.MANE_IMS_ROAD_FINALIZE = "Y";
                    IMSContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    IMSContract.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    scope.Complete();
                }
                message = "Road Details finalized successfully.";
                return true;

                #endregion Finalize Code

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().FinalizeEmargRoadDAL");
                message = "Error occured while finalizing Road Details";
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool PackageFinalizeEmargRoadDAL(string PackageID, out string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            MANE_EMARG_CONTRACT IMSContract = null;
            try
            {
                using (var scope = new TransactionScope())
                {


                    var lstTechnologyDetails = (from erd in dbContext.EMARG_ROAD_DETAILS
                                                join isp in dbContext.IMS_SANCTIONED_PROJECTS on erd.ROAD_CODE equals isp.IMS_PR_ROAD_CODE
                                                join emarg in dbContext.MANE_EMARG_CONTRACT on

                                                new { C1 = isp.IMS_PR_ROAD_CODE, C2 = erd.EMARG_ID } equals
                                                new { C1 = emarg.IMS_PR_ROAD_CODE, C2 = emarg.EMARG_ID } into details

                                                from detailrecords in details.DefaultIfEmpty()
                                                where isp.IMS_PACKAGE_ID == PackageID && isp.IMS_SANCTIONED == "Y" && (erd.IS_DEACTIVATED != "Y" || erd.IS_DEACTIVATED == null) && erd.DLP_TYPE == 1
                                                select new
                                                {
                                                    isp.IMS_PR_ROAD_CODE,

                                                    isp.IMS_PACKAGE_ID

                                                }).OrderBy(m => m.IMS_PACKAGE_ID).Distinct();






                    var lstRoadCountInPackage = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PACKAGE_ID == PackageID && m.IMS_SANCTIONED == "Y" && m.IMS_PROPOSAL_TYPE == "P" && (m.IMS_STAGE_PHASE != "S1" || m.IMS_STAGE_PHASE == null)).ToList();

                    if (lstTechnologyDetails.Count() == lstRoadCountInPackage.Count())
                    {
                        // Allow to proceed
                    }
                    else
                    {
                        message = "All Roads in this Package are not available for correction. Hence this Package can not be finalized here.";
                        return false;
                    }

                    #region Agreement Validations

                    // Check Agreement Number and Agreement Date in single package must be same
                    int RoadCodeID = lstTechnologyDetails.Select(m => m.IMS_PR_ROAD_CODE).FirstOrDefault();//dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PACKAGE_ID == PackageID).Select(m => m.IMS_PR_ROAD_CODE).FirstOrDefault();
                    string AgreementNumber = dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCodeID && (m.IS_DEACTIVATED != "Y" || m.IS_DEACTIVATED == null)).Select(m => m.MANE_AGREEMENT_NUMBER).FirstOrDefault();
                    Nullable<System.DateTime> AgreementDate = dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCodeID && (m.IS_DEACTIVATED != "Y" || m.IS_DEACTIVATED == null)).Select(m => m.MANE_AGREEMENT_DATE).FirstOrDefault();
                    // Check Agreement Number and Agreement Date in single package must be same

                    foreach (var item in lstTechnologyDetails)
                    {

                        // Agreement Validations
                        if (dbContext.MANE_EMARG_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && c.MANE_IMS_ROAD_FINALIZE == "Y" && c.MANE_IMS_PACKAGE_FINALIZE == "N" && c.EMARG_ID != null && (c.IS_DEACTIVATED != "Y" || c.IS_DEACTIVATED == null)).Any())
                        {
                            IMSContract = dbContext.MANE_EMARG_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && c.MANE_IMS_ROAD_FINALIZE == "Y" && c.MANE_IMS_PACKAGE_FINALIZE == "N" && c.EMARG_ID != null && (c.IS_DEACTIVATED != "Y" || c.IS_DEACTIVATED == null)).FirstOrDefault();
                            if (IMSContract.MANE_AGREEMENT_NUMBER.Equals(AgreementNumber))
                            {
                                // Agreement Numbers for All Roads in a Single Package must be same.
                            }
                            else
                            {
                                message = "Agreement Number for all Roads in this Package is not same. Hence This package can not be finalized.";
                                return false;
                            }

                            if (IMSContract.MANE_AGREEMENT_DATE == AgreementDate)
                            {
                                // Agreement Date for All Roads in a Single Package must be same.
                            }
                            else
                            {
                                message = "Agreement Date for all Roads in this Package is not same. Hence This package can not be finalized.";
                                return false;
                            }
                        }
                        else
                        {
                            if (dbContext.MANE_EMARG_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && c.MANE_IMS_ROAD_FINALIZE == "Y" && c.MANE_IMS_PACKAGE_FINALIZE == "Y" && c.EMARG_ID != null && (c.IS_DEACTIVATED != "Y" || c.IS_DEACTIVATED == null)).Any())
                            {
                                message = "This Package is already Finalized.";
                                return false;
                            }
                            else
                            {
                                message = "This package may be available for other DPIU also.Finalize all the Roads in the Package .";
                                return false;
                            }

                        }
                        // Agreement Validations
                        #endregion

                        #region PAN Validations

                        //  PAN Validations
                        MANE_EMARG_CONTRACT Panvalidation = dbContext.MANE_EMARG_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && c.MANE_IMS_ROAD_FINALIZE == "Y" && c.MANE_IMS_PACKAGE_FINALIZE == "N" && (c.IS_DEACTIVATED != "Y" || c.IS_DEACTIVATED == null)).FirstOrDefault();

                        MASTER_CONTRACTOR objectPAN = dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_ID == Panvalidation.MAST_CON_ID).FirstOrDefault();



                        List<Int32> RoadCodeList = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PACKAGE_ID == PackageID && m.IMS_SANCTIONED == "Y" && m.IMS_PROPOSAL_TYPE == "P").Select(m => m.IMS_PR_ROAD_CODE).ToList<Int32>();

                        //  List<Int32> ContractorIDList=dbContext.MANE_IMS_CONTRACT.Where(m=>RoadCodeList.Contains(m.IMS_PR_ROAD_CODE) && m.MANE_CONTRACT_STATUS!="I").Select(m=>m.MAST_CON_ID).Distinct().ToList<Int32>();
                        List<Int32> ContractorIDList = dbContext.MANE_IMS_CONTRACT.Where(m => RoadCodeList.Contains(m.IMS_PR_ROAD_CODE) && m.MANE_CONTRACT_STATUS == "P" && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_FINALIZED == "Y").Select(m => m.MAST_CON_ID).Distinct().ToList<Int32>();
                        if (string.IsNullOrEmpty(objectPAN.MAST_CON_PAN))
                        {
                            message = "PAN Number is not available for any of the Road in this Package. Hence This package can not be finalized.";
                            return false;
                        }
                        else if (objectPAN.MAST_CON_PAN.Length != 10)
                        {
                            message = "PAN Number is not valid for any of the Road in this Package. Hence This package can not be finalized.";
                            return false;
                        }
                        else
                        {

                            Regex regex = new Regex("([A-Z]){5}([0-9]){4}([A-Z]){1}$");
                            if (!regex.IsMatch(objectPAN.MAST_CON_PAN))
                            {
                                message = objectPAN.MAST_CON_PAN + " This PAN Number is not in correct format. Hence This package can not be finalized.";
                                return false;
                            }

                            if (ContractorIDList.Count > 1)
                            {
                                message = "Within a Package, multiple Contractors can not be allowed. Hence This package can not be finalized.";
                                return false;
                            }

                            //if (contractor1.Count > 1)
                            //{
                            //    message = "Same PAN should not exist for more than one contractor. Hence This package can not be finalized.";
                            //    return false;
                            //}
                        }
                        #endregion

                        #region Finalize Code
                        var emargDetailsToUpdate = dbContext.EMARG_ROAD_DETAILS.Where(m => m.ROAD_CODE == item.IMS_PR_ROAD_CODE && m.EMARG_STATUS == "N" && (m.IS_DEACTIVATED != "Y" || m.IS_DEACTIVATED == null)).Select(m => m.ROAD_CODE);
                        foreach (int RoadCode in emargDetailsToUpdate)
                        {
                            EMARG_ROAD_DETAILS update = dbContext.EMARG_ROAD_DETAILS.Where(m => m.ROAD_CODE == RoadCode && (m.IS_DEACTIVATED != "Y" || m.IS_DEACTIVATED == null)).FirstOrDefault();
                            update.EMARG_STATUS = "Y";
                            dbContext.Entry(update).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        if (dbContext.MANE_EMARG_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && c.MANE_IMS_ROAD_FINALIZE == "Y" && c.MANE_IMS_PACKAGE_FINALIZE == "N" && c.EMARG_ID != null && (c.IS_DEACTIVATED != "Y" || c.IS_DEACTIVATED == null)).Any())
                        {
                            IMSContract = dbContext.MANE_EMARG_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && (c.IS_DEACTIVATED != "Y" || c.IS_DEACTIVATED == null)).FirstOrDefault();
                            IMSContract.MANE_IMS_PACKAGE_FINALIZE = "Y";
                            IMSContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            IMSContract.USERID = PMGSYSession.Current.UserId;
                            dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            message = "All Roads in this Package are not Finalized. Hence This package can not be Finalized.";
                            return false;
                        }

                    }
                    scope.Complete();
                }

                message = "Package Finalized Successfully.";
                return true;
                #endregion

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().PackageFinalizeEmargRoadDAL");
                message = "Package NOT Finalized.";
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool PushPackageToEmarg(string PackageID, out string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            MANE_EMARG_CONTRACT IMSContract = null;

            try
            {
                using (var scope = new TransactionScope())
                {
                    EMARG_COMPLETED_WORK_DETAILS_SERVICE tableObject = new EMARG_COMPLETED_WORK_DETAILS_SERVICE();
                    int DistrictCode = PMGSYSession.Current.DistrictCode;
                    int StateCode = PMGSYSession.Current.StateCode;


                    // To Compare Purpose Only
                    var eStatsToCompare = dbContext.USP_CORRECTED_ROAD_DETAIL_FOR_MAINTENANCE(StateCode, DistrictCode).ToList();
                    var emargStatsToCompare = eStatsToCompare.Where(m => m.PACKAGE_NO == PackageID).ToList();



                    var eStats = dbContext.USP_CORRECTED_ROAD_DETAIL_FOR_MAINTENANCE(StateCode, 0).ToList(); // To Push all roads in a single package across different district at once 
                    var emargStats = eStats.Where(m => m.PACKAGE_NO == PackageID).ToList();


                    #region If List have 0 Records then
                    if (emargStats.Count() == 0)
                    {
                        message = "All roads in this package must be finalized to push this package to Emarg";
                        return false;
                    }
                    #endregion


                    var result = emargStats.Select(o => o.CONTRACTOR_ID).Distinct().ToList();
                    if (result.Count() > 1)
                    {
                        message = "Within a Package, multiple Contractors can not be allowed. Hence This Package can not be pushed to Emarg.";
                        return false;
                    }


                    #region PAN Validations

                    // PAN Number Against Every Road Must not be null or empty. It Should be only of 10 digits.
                    foreach (var iValue in emargStats)
                    {
                        if (string.IsNullOrEmpty(iValue.CONTRACTOR_PAN))
                        {
                            message = "Contractor PAN is not Available for any of the road in this Package. Hence This Package can not be pushed to Emarg.";
                            return false;
                        }
                        if (iValue.CONTRACTOR_PAN.Count() != 10)
                        {
                            message = "Contractor PAN is not in Valid format for any of the road in this Package. Hence This Package can not be pushed to Emarg.";
                            return false;
                        }

                        Regex regex = new Regex("([A-Z]){5}([0-9]){4}([A-Z]){1}$");
                        if (!regex.IsMatch(iValue.CONTRACTOR_PAN))
                        {
                            message = iValue.CONTRACTOR_PAN + " This PAN Number is not in correct format. Hence This Package can not be pushed to Emarg.";
                            return false;
                        }


                        //var contractor1 = dbContext.MASTER_CONTRACTOR.Where(m => m.MAST_CON_PAN == iValue.CONTRACTOR_PAN).ToList();
                        //if (contractor1.Count > 1)
                        //{
                        //    message = "Same PAN should not exist for more than one contractor. Hence This package can not be pushed to Emarg.";
                        //    return false;
                        //}
                    }
                    #endregion PAN Validations

                    #region Agreement Validations

                    // Check Agreement Number and Agreement Date in single package must be same
                    int RoadCodeID = emargStats.Select(m => m.ROAD_CODE).FirstOrDefault();//dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PACKAGE_ID == PackageID).Select(m => m.IMS_PR_ROAD_CODE).FirstOrDefault();

                    string AgreementNumber = dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCodeID && (m.IS_DEACTIVATED != "Y" || m.IS_DEACTIVATED == null)).Select(m => m.MANE_AGREEMENT_NUMBER).FirstOrDefault();
                    Nullable<System.DateTime> AgreementDate = dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == RoadCodeID && (m.IS_DEACTIVATED != "Y" || m.IS_DEACTIVATED == null)).Select(m => m.MANE_AGREEMENT_DATE).FirstOrDefault();

                    foreach (var iValue in emargStats)
                    {
                        // Agreement Validations
                        if (dbContext.MANE_EMARG_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == iValue.ROAD_CODE && c.MANE_IMS_ROAD_FINALIZE == "Y" && c.MANE_IMS_PACKAGE_FINALIZE == "Y" && (c.IS_DEACTIVATED != "Y" || c.IS_DEACTIVATED == null)).Any())
                        {
                            IMSContract = dbContext.MANE_EMARG_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == iValue.ROAD_CODE && c.MANE_IMS_ROAD_FINALIZE == "Y" && c.MANE_IMS_PACKAGE_FINALIZE == "Y" && (c.IS_DEACTIVATED != "Y" || c.IS_DEACTIVATED == null)).FirstOrDefault();
                            if (IMSContract.MANE_AGREEMENT_NUMBER.Equals(AgreementNumber))
                            {
                                // Agreement Numbers for All Roads in a Single Package must be same.
                            }
                            else
                            {
                                message = "Agreement Number for all Roads in this Package is not same. Hence This package can not pushed to Emarg.";
                                return false;
                            }

                            if (IMSContract.MANE_AGREEMENT_DATE == AgreementDate)
                            {
                                // Agreement Date for All Roads in a Single Package must be same.
                            }
                            else
                            {
                                message = "Agreement Date for all Roads in this Package is not same. Hence This package can not pushed to Emarg.";
                                return false;
                            }
                        }
                        else
                        {
                            message = "Package is not finalized. Hence This package can not pushed to Emarg.";
                            return false;
                        }

                    }
                    #endregion


                    DateTime? maxCompletionDate = emargStats.Select(m => m.COMPLETION_DATE).Max();

                    #region Duplicate Check
                    // To avoide duplicate records(duplicate Emarg ID) saving in EMARG_COMPLETED_WORK_DETAILS_SERVICE

                    if (emargStatsToCompare.Count() == emargStats.Count())
                    {
                        foreach (var iValue in emargStats)
                        {
                            if (dbContext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Where(m => m.EMARG_ID == iValue.EMARG_ID).Any())
                            {
                                message = "This package is already pushed to Emarg";
                                return false;
                            }
                        }
                    }
                    #endregion




                    try
                    {
                        foreach (var iValue in emargStats)
                        {
                            IMSContract = dbContext.MANE_EMARG_CONTRACT.Where(c => c.EMARG_ID == iValue.EMARG_ID).FirstOrDefault();
                            IMSContract.IS_PUSHED_TO_EMARG = "Y";
                            IMSContract.PUSHED_DATE_TO_EMARG = System.DateTime.Now;
                            IMSContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            IMSContract.USERID = PMGSYSession.Current.UserId;
                            dbContext.Entry(IMSContract).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                        }
                    }
                    catch (DbEntityValidationException e)
                    {
                        if (!Directory.Exists(errorLogPath))
                        {
                            Directory.CreateDirectory(errorLogPath);
                        }
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                sw.WriteLine("Entity of type \"{0}\" in state \"{1}\"MANE_EMARG_CONTRACT has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                        message = "This package is NOT pushed to Emarg.";
                        return false;
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "MaintenanceAgreementDAL().PushPackageToEmarg().MANE_EMARG_CONTRACT");
                        message = "This package is NOT pushed to Emarg.";
                        return false;
                        throw;
                    }


                    try
                    {
                        foreach (var iValue in emargStats)
                        {
                            EMARG_ROAD_DETAILS update = dbContext.EMARG_ROAD_DETAILS.Where(m => m.EMARG_ID == iValue.EMARG_ID).FirstOrDefault();

                            update.DATE_OF_REPUSHING = System.DateTime.Now;
                            update.OMMAS_REPUSHING_STATUS = "1";
                            dbContext.Entry(update).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                        }
                    }
                    catch (DbEntityValidationException e)
                    {
                        if (!Directory.Exists(errorLogPath))
                        {
                            Directory.CreateDirectory(errorLogPath);
                        }
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                sw.WriteLine("Entity of type \"{0}\" in state \"{1}\"EMARG_ROAD_DETAILS has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                        message = "This package is NOT pushed to Emarg.";
                        return false;
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "MaintenanceAgreementDAL().PushPackageToEmarg().EMARG_ROAD_DETAILS");
                        message = "This package is NOT pushed to Emarg.";
                        return false;
                        throw;

                    }



                    try
                    {
                        foreach (var itemValue in emargStats)
                        {
                            if (dbContext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Where(m => m.EMARG_ID == itemValue.EMARG_ID).Any())
                            {

                                //DO NOT INSERT SAME EMARG ID RECORD

                            }
                            else
                            {
                                // tableObject.EID = (dbContext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Any() ? (dbContext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Max(m=>m.EID)+1) : 1);//temValue.STATE_NAME;
                                tableObject.EMARG_ID = itemValue.EMARG_ID;
                                tableObject.MAST_STATE_CODE = itemValue.MAST_STATE_CODE;
                                tableObject.MAST_STATE_NAME = itemValue.MAST_STATE_NAME;
                                tableObject.PMGSY_SCHEME = itemValue.PMGSY_SCHEME;

                                tableObject.MAST_DISTRICT_CODE = itemValue.MAST_DISTRICT_CODE;
                                tableObject.MAST_DISTRICT_NAME = itemValue.MAST_DISTRICT_NAME;
                                tableObject.MAST_BLOCK_CODE = itemValue.MAST_BLOCK_CODE;
                                tableObject.MAST_BLOCK_NAME = itemValue.MAST_BLOCK_NAME;
                                tableObject.PIU_CODE = itemValue.PIU_CODE;
                                tableObject.PIU_NAME = itemValue.PIU_NAME;
                                tableObject.ROAD_CODE = itemValue.ROAD_CODE;
                                tableObject.SANCTION_YEAR = itemValue.SANCTION_YEAR;
                                tableObject.SANCTION_BATCH = itemValue.SANCTION_BATCH;
                                tableObject.PACKAGE_NO = itemValue.PACKAGE_NO;


                                tableObject.ROAD_NAME = itemValue.ROAD_NAME.ToString();
                                tableObject.SANCTION_LENGTH = itemValue.SANCTION_LENGTH;
                                tableObject.COMPLETED_LENGTH = itemValue.COMPLETED_LENGTH;
                                tableObject.CC_LENGTH = itemValue.CC_LENGTH;
                                tableObject.BT_LENGTH = itemValue.BT_LENGTH;
                                tableObject.CDWorks = itemValue.SANCTION_CD_WORK;
                                tableObject.CN_CODE = itemValue.CORE_NETWORK_CODE;
                                tableObject.TRAFFIC_CATEGORY = itemValue.TRAFFIC_CATEGORY;


                                tableObject.CARRIAGE_WAY_WIDTH = itemValue.CARRIAGE_WAY_WIDTH;
                                tableObject.STAGE = itemValue.STAGE;
                                tableObject.COMPLETION_DATE = maxCompletionDate;//itemValue.COMPLETION_DATE;

                                tableObject.WORK_ORDER_NO = itemValue.WORK_ORDER_NO;
                                tableObject.WORK_ORDER_DATE = itemValue.WORK_ORDER_DATE;

                                tableObject.CONTRACTOR_NAME = itemValue.CONTRACTOR_NAME;
                                tableObject.CONTRACTOR_PAN = itemValue.CONTRACTOR_PAN;
                                tableObject.CONTRACTOR_ID = itemValue.CONTRACTOR_ID;

                                tableObject.IS_PUSHED_TO_EMARG = "Y";
                                tableObject.PUSHED_DATE_TO_EMARG = System.DateTime.Now;
                                tableObject.USERID = PMGSYSession.Current.UserId;
                                tableObject.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Add(tableObject);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                    catch (DbEntityValidationException e)
                    {                 
                        if (!Directory.Exists(errorLogPath))
                        {
                            Directory.CreateDirectory(errorLogPath);
                        }
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                sw.WriteLine("Entity of type \"{0}\" in state \"{1}\"EMARG_COMPLETED_WORK_DETAILS_SERVICE has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                        message = "This package is NOT pushed to Emarg.";
                        return false;
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "MaintenanceAgreementDAL().PushPackageToEmarg().EMARG_COMPLETED_WORK_DETAILS_SERVICE");
                        message = "This package is NOT pushed to Emarg.";
                        return false;
                        throw;
                    }
                    
                    dbContext.SaveChanges();
                    scope.Complete();

                }
                message = "This package is pushed to Emarg Successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().PushPackageToEmarg()");
                message = "This package is NOT pushed to Emarg.";
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool DeleteRoadDetailsBeforePackageFinalizationDAL(int EmargID, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MANE_EMARG_CONTRACT master = dbContext.MANE_EMARG_CONTRACT.Where(m => m.EMARG_ID == EmargID).FirstOrDefault();
                if (master != null)
                {
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    master.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.MANE_EMARG_CONTRACT.Remove(master);
                    dbContext.SaveChanges();
                    message = "Road details deleted successfully.";
                    return true;
                }
                else
                {
                    message = "Road details can not be deleted.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().DeleteRoadDetailsBeforePackageFinalizationDAL() Emarg ID =" + EmargID);
                message = "Error Occurred while processing your request.";
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion

        #region Emarg Repackage

        public PMGSY.Models.MaintenanceAgreement.EmargRepackage GetEmargMaintenanceRepackagingDetails(int proposalCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                CommonFunctions objCommon = new CommonFunctions();

                EmargRepackage model = new EmargRepackage();

                IMS_SANCTIONED_PROJECTS ims_master = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).FirstOrDefault();


                model.OLD_PACKAGE_ID = ims_master.IMS_PACKAGE_ID;
                model.NewOldPackage = ims_master.IMS_PACKAGE_ID;


                model.roadLength = ims_master.IMS_PAV_LENGTH;


                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetEmargMaintenanceRepackagingDetails().DAL");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // MORD Login Comes here
        public Array GetEmargMaintenanceProposalsForRepackaging(int? page, int? rows, string sidx, string sord, out long totalRecords, int StateCode, int batch, int block, string package, int collaboration, string proposalType, string upgradationType)
        {
            var dbContext = new PMGSYEntities();
            try
            {
                Int32 DistCode = (block == -1 ? 0 : block); // Here block variable have Dirtict Code in it.


                if (PMGSYSession.Current.RoleCode == 22)
                {// PIU
                    StateCode = PMGSYSession.Current.StateCode;
                    DistCode = PMGSYSession.Current.DistrictCode;
                }


                // MORD Login : Allow Complted, Maintenance and Progress Proposals.
                string[] Status = { "C", "X", "P" };

                var lstPropsoals1 = dbContext.USP_GET_EMARG_PACKAGES_FOR_REPACKAGING(StateCode, DistCode).ToList();

                var lstPropsoals = lstPropsoals1.Where(m => Status.Contains(m.IMS_ISCOMPLETED)).ToList();


                totalRecords = lstPropsoals.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                lstPropsoals = lstPropsoals.OrderBy(m => m.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstPropsoals = lstPropsoals.OrderBy(m => m.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstPropsoals = lstPropsoals.OrderBy(m => m.PACKAGE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                            default:
                                lstPropsoals = lstPropsoals.OrderBy(m => m.PACKAGE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.PACKAGE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                            default:
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.PACKAGE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstPropsoals = lstPropsoals.OrderBy(m => m.PACKAGE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }



                var result = lstPropsoals.Select(m => new
                {
                    m.MAST_STATE_NAME,
                    m.MAST_DISTRICT_NAME,
                    m.MAST_BLOCK_NAME, //= dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == m.IMS_PR_ROAD_CODE).SingleOrDefault().IMS_PACKAGE_ID.ToString(),
                    m.OLD_PACKAGE_NO,
                    m.PACKAGE_NO,
                    m.ROAD_NAME,
                    m.ROAD_CODE
                    // m.PACKAGE_COMPLETION_DATE


                }).ToArray();



                return result.Select(m => new
                {
                    cell = new[] 
                    {
                        
                        m.MAST_STATE_NAME == null?"":m.MAST_STATE_NAME.ToString(),
                        m.MAST_DISTRICT_NAME == null?"":m.MAST_DISTRICT_NAME.ToString(),

                        m.MAST_BLOCK_NAME == null ? "-" : m.MAST_BLOCK_NAME.ToString(),
                        m.OLD_PACKAGE_NO == null?"-":m.OLD_PACKAGE_NO.ToString(),
                        m.PACKAGE_NO==null?"-":m.PACKAGE_NO.ToString(),// Used as Current Package ID
                        m.ROAD_NAME == null?"-":m.ROAD_NAME.ToString(),

                       dbContext.EMARG_REPACKAGE_DETAILS.Any(x=>x.CURRENT_PACKAGE_ID==m.PACKAGE_NO && x.IS_PACKAGE_FINALIZED=="Y")?"-":(dbContext.EMARG_REPACKAGE_DETAILS.Any(x=>x.IMS_PR_ROAD_CODE==m.ROAD_CODE && x.IS_PACKAGE_FINALIZED=="Y")?"<span style='color:green;'>Package is Finalized</span>":(dbContext.EMARG_REPACKAGE_DETAILS.Any(x=>x.IMS_PR_ROAD_CODE==m.ROAD_CODE && x.REPACKAGING_STATUS=="Y")?"<span style='color:red;'>Road is Finalized</span>": "<a href='#' title='Click here to Add Repackaging Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=AddEmargRepackagingDetails('" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+m.ROAD_CODE.ToString().Trim() }) +"'); return false;'>Add Repackaging Details</a>")),
                      

                       dbContext.EMARG_REPACKAGE_DETAILS.Any(x=>x.IMS_PR_ROAD_CODE==m.ROAD_CODE && x.IS_PACKAGE_FINALIZED=="Y")?"<span style='color:green;'>Package is Finalized</span>":(dbContext.EMARG_REPACKAGE_DETAILS.Any(x=>x.IMS_PR_ROAD_CODE==m.ROAD_CODE)?(dbContext.EMARG_REPACKAGE_DETAILS.Any(x=>x.IMS_PR_ROAD_CODE==m.ROAD_CODE && x.REPACKAGING_STATUS=="Y")?"<span style='color:red;'>Road is Finalized</span>":"<a href='#' title='Click here to Finalize Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick=FinalizeEmargRepackagingDetails('" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+m.ROAD_CODE.ToString().Trim() }) +"'); return false;'>Finalize</a>"):"-")

                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetEmargMaintenanceProposalsForRepackaging().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool AddEmargMaintenanceRepackagingDetails(PMGSY.Models.MaintenanceAgreement.EmargRepackage model, out string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                string[] encryptedParameters = model.EncProposalCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                using (var scope = new TransactionScope())
                {
                    int proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    IMS_SANCTIONED_PROJECTS ims_master = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).FirstOrDefault();

                    String OldPakageID = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_PACKAGE_ID).FirstOrDefault();



                    //if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C"))
                    //{
                    //    DateTime? CompletionDate = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C").Select(m=>m.EXEC_COMPLETION_DATE).FirstOrDefault();
                    //    DateTime dt = Convert.ToDateTime("01-04-2010");
                    //    if (CompletionDate != null)
                    //    {
                    //        if (CompletionDate < dt)
                    //        {
                    //            message = "Completion Date for " + ims_master.IMS_ROAD_NAME + " This Road in " + ims_master.IMS_PACKAGE_ID + " Package ID is before 1 April 2010. Hence this Road can not be added in  " + model.NEW_PACKAGE_ID.Trim();
                    //            return false;
                    //        }
                    //    }

                    //}
                    //else 
                    //{
                    //    message = ims_master.IMS_ROAD_NAME + " This Road in " + ims_master.IMS_PACKAGE_ID+ " Package ID is not Completed. Hence this Road can not be added in  "+ model.NEW_PACKAGE_ID.Trim();
                    //    return false;
                    //}




                    if (dbContext.EMARG_REPACKAGE_DETAILS.Any(m => m.CURRENT_PACKAGE_ID == model.NEW_PACKAGE_ID.Trim() && m.IS_PACKAGE_FINALIZED == "Y"))
                    {
                        message = "This Package " + model.NEW_PACKAGE_ID.Trim() + " is already finalized after Repackaging. Hence this Road can not be added in " + model.NEW_PACKAGE_ID.Trim();
                        return false;
                    }

                    // ims_master.IMS_PACKAGE_ID
                    var modelMasterDetails1 = dbContext.USP_VALIDATE_EMARG_ROAD_FOR_REPACKAGING(ims_master.IMS_PACKAGE_ID, 1).ToList();
                    var modelMasterDetails2 = dbContext.USP_VALIDATE_EMARG_ROAD_FOR_REPACKAGING(ims_master.IMS_PACKAGE_ID, 2).ToList();


                    // Here level one and two can only be checked if there is data in table omms.emarg_road_details
                    if (dbContext.EMARG_ROAD_DETAILS.Any(m => m.PACKAGE_NO == ims_master.IMS_PACKAGE_ID))
                    {

                        if (modelMasterDetails1.Count != 0)
                        {
                            foreach (var itemValue1 in modelMasterDetails1)
                            {
                                if (itemValue1.ELIGIBLE_REPACKAGE == 1)
                                {
                                    foreach (var itemValue2 in modelMasterDetails2)
                                    {
                                        if (itemValue2.ELIGIBLE_REPACKAGE == 1)
                                        {
                                        }
                                        else
                                        { // Dont Allow
                                            message = "Package has been already sent to Emarg / PIU is in process of sending it to Emarg. Once it is received from Emarg in OMMAS for correction, then only it can be repackaged.";
                                            return false;
                                        }

                                    }
                                }
                                else
                                { // Dont Allow
                                    message = "Package has been already sent to Emarg / PIU is in process of sending it to Emarg. Once it is received from Emarg in OMMAS for correction, only then it can be repackaged.";
                                    return false;
                                }
                            }
                        }
                    }

                    // Check here new package ID must be Completed. (All Roads in that New package ID must be Completed.)
                    if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => m.IMS_PACKAGE_ID == model.NEW_PACKAGE_ID.Trim() && m.IMS_SANCTIONED == "Y"))
                    {
                        List<IMS_SANCTIONED_PROJECTS> lstIMs = new List<IMS_SANCTIONED_PROJECTS>();
                        lstIMs = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PACKAGE_ID == model.NEW_PACKAGE_ID.Trim() && m.IMS_SANCTIONED == "Y").ToList();
                        foreach (var item in lstIMs)
                        {
                            if (PMGSYSession.Current.RoleCode == 36)
                            {// ITNO Role : Allow only Completed Packages . i.e. C and X Flags only

                                if (item.IMS_ISCOMPLETED == "C" || item.IMS_ISCOMPLETED == "X")
                                {// Allow
                                }
                                else
                                {
                                    message = model.NEW_PACKAGE_ID.Trim() + " Only Completed package details can be repackaged by ITNO.";
                                    return false;
                                }
                            }
                            else
                            { // MOR Role : Allow Completed and Inprogress packages .i.e. C and X and P

                                if (item.IMS_ISCOMPLETED == "C" || item.IMS_ISCOMPLETED == "X" || item.IMS_ISCOMPLETED == "P" || item.IMS_ISCOMPLETED != null)
                                {// Allow
                                }
                                else
                                {
                                    message = model.NEW_PACKAGE_ID.Trim() + " Only Completed and Inprogress package details can be repackaged by MORD.";
                                    return false;
                                }

                            }

                            //if (item.IMS_ISCOMPLETED != "C" || item.IMS_ISCOMPLETED != "X")
                            //{
                            //    message = model.NEW_PACKAGE_ID.Trim() + " This Package is not completed. Repackaging can not be done.";
                            //    return false;
                            //}
                        }
                    }

                    // 1. Modification in Package Number in omms.IMS_SANCTIONED_PROJECTS
                    if (ims_master != null)
                    {
                        ims_master.IMS_OLD_PACKAGE_ID = OldPakageID.Trim();//ims_master.IMS_PACKAGE_ID.Trim();
                        ims_master.IMS_PACKAGE_ID = model.NEW_PACKAGE_ID.Trim();
                        dbContext.Entry(ims_master).State = System.Data.Entity.EntityState.Modified;
                    }


                    // 2.  To Capture Oldest Package ID
                    EMARG_REPACKAGE_DETAILS emargPackage = new EMARG_REPACKAGE_DETAILS();
                    if (!dbContext.EMARG_REPACKAGE_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode))
                    {
                        emargPackage.REPACKAGE_ID = (dbContext.EMARG_REPACKAGE_DETAILS.Any() ? dbContext.EMARG_REPACKAGE_DETAILS.Max(m => m.REPACKAGE_ID) + 1 : 1);
                        emargPackage.IMS_PR_ROAD_CODE = proposalCode;
                        emargPackage.REPACKAGING_STATUS = "N";
                        emargPackage.IS_PACKAGE_FINALIZED = "N";
                        emargPackage.OLDEST_PACKAGE_ID = OldPakageID.Trim();//ims_master.IMS_PACKAGE_ID.Trim();
                        dbContext.EMARG_REPACKAGE_DETAILS.Add(emargPackage);

                    }

                    //3.  Mark Maintenance Agreement as Terminated



                    List<MANE_IMS_CONTRACT> mic = new List<MANE_IMS_CONTRACT>();

                    mic = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_AGREEMENT_TYPE == "R" && m.MANE_CONTRACT_STATUS == "P").ToList();

                    if (mic == null)
                    {
                        message = "Maintenance Agreement Details are not available. This package can not be repackaged.";
                        return false;
                    }

                    if (mic != null)
                    {
                        foreach (var item3 in mic)
                        {
                            if (item3.MANE_CONTRACT_FINALIZED != "Y")
                            {
                                message = "Maintenance Agreement Details are not finalized. This package can not be repackaged.";
                                return false;
                            }
                        }
                    }

                    if (mic != null)
                    {
                        foreach (var item2 in mic)
                        {
                            item2.MANE_CONTRACT_STATUS = "I"; // Mark Terminated
                            dbContext.Entry(ims_master).State = System.Data.Entity.EntityState.Modified;
                        }
                    }


                    dbContext.SaveChanges();
                    scope.Complete();
                }
                message = "Repackaging Details are added Successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddEmargMaintenanceRepackagingDetails().DAL");
                message = "Repackaging Details are not added.";
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool FinalizeEmargRepackageRoadDAL(Int32 ProposalCode, out string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {


                using (var scope = new TransactionScope())
                {
                    IMS_SANCTIONED_PROJECTS isp = new IMS_SANCTIONED_PROJECTS();
                    isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == ProposalCode).FirstOrDefault();

                    if (!dbContext.EMARG_REPACKAGE_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == ProposalCode))
                    {
                        message = "Road details are not finalized as Repackaging for this Road is not done.";
                        return false;
                    }

                    if (dbContext.EMARG_REPACKAGE_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == ProposalCode))
                    {
                        EMARG_REPACKAGE_DETAILS emargPackage = new EMARG_REPACKAGE_DETAILS();
                        emargPackage = dbContext.EMARG_REPACKAGE_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == ProposalCode).FirstOrDefault();

                        emargPackage.OLD_PACKAGE_ID = isp.IMS_OLD_PACKAGE_ID;
                        emargPackage.CURRENT_PACKAGE_ID = isp.IMS_PACKAGE_ID;
                        emargPackage.ENTRY_DATE = System.DateTime.Now;
                        emargPackage.IS_ACTIVE = "Y";
                        emargPackage.REPACKAGING_STATUS = "Y";
                        emargPackage.USERID = PMGSYSession.Current.UserId;
                        emargPackage.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(emargPackage).State = System.Data.Entity.EntityState.Modified;
                    }

                    dbContext.SaveChanges();
                    scope.Complete();
                }
                message = "Repackaged Road details are Finalized Successfully.";
                return true;
            }


            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().FinalizeEmargRepackageRoadDAL");
                message = "Package NOT Finalized.";
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public List<SelectListItem> PopulateEmargMaintenancePackagesForRepackagingDAL(int block, int year, int batch)
        {
            var dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> PackageList = new List<SelectListItem>();
                var lstPackages = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                   join Maint in dbContext.MAINE_REPACKAGE_DETAILS on item.IMS_PR_ROAD_CODE equals Maint.IMS_PR_ROAD_CODE into MRD
                                   from p in MRD.DefaultIfEmpty()
                                   where
                                    item.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode &&
                                   (block < 0 ? 1 : item.MAST_BLOCK_CODE) == (block < 0 ? 1 : block) &&
                                   (year == 0 ? 1 : item.IMS_YEAR) == (year == 0 ? 1 : year) &&
                                   (batch == 0 ? 1 : item.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
                                   (item.IMS_ISCOMPLETED.Equals("C") || item.IMS_ISCOMPLETED.Equals("X"))

                                   select new
                                   {
                                       PACKAGE_CODE = p.IMS_MAINT_REPACKAGE_ID == null ? item.IMS_PACKAGE_ID : p.IMS_MAINT_REPACKAGE_ID,
                                       PACKAGE_NAME = p.IMS_MAINT_REPACKAGE_ID == null ? item.IMS_PACKAGE_ID : p.IMS_MAINT_REPACKAGE_ID
                                   }).Distinct().ToList();

                PackageList = new SelectList(lstPackages, "PACKAGE_CODE", "PACKAGE_NAME").ToList();
                PackageList.Insert(0, new SelectListItem { Value = "0", Text = "All Packages" });
                return PackageList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateEmargMaintenancePackagesForRepackagingDAL().DAL");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public bool PackageFinalizeEmargRepackageRoadDAL(string PackageID, out string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            MANE_EMARG_CONTRACT IMSContract = null;
            try
            {
                using (var scope = new TransactionScope())
                {
                    // Get Individual Road Codes to be deactivated.
                    List<EMARG_REPACKAGE_DETAILS> repackage = dbContext.EMARG_REPACKAGE_DETAILS.Where(m => m.CURRENT_PACKAGE_ID == PackageID).ToList();


                    List<IMS_SANCTIONED_PROJECTS> repackageEmarg = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PACKAGE_ID == PackageID).ToList();

                    string oldestPackageID = dbContext.EMARG_REPACKAGE_DETAILS.Where(m => m.CURRENT_PACKAGE_ID == PackageID).Select(m => m.OLDEST_PACKAGE_ID).FirstOrDefault();

                    if (!dbContext.EMARG_REPACKAGE_DETAILS.Any(m => m.CURRENT_PACKAGE_ID == PackageID))
                    {
                        message = "No roads are Repackaged in this Package";
                        return false;
                    }

                    if (repackageEmarg.Count == 0)
                    {
                        message = "All repackaged roads in " + PackageID + " Package are not finalized. Hence Package can not be Finalized.";
                        return false;
                    }

                    foreach (var item01 in repackageEmarg)
                    {

                        if (dbContext.EMARG_REPACKAGE_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == item01.IMS_PR_ROAD_CODE && m.IS_PACKAGE_FINALIZED == "Y").Any())
                        {
                            message = "Package is already finalized after Repackaging.";
                            return false;
                        }
                    }



                    foreach (var item02 in repackageEmarg)
                    {
                        if (dbContext.EMARG_REPACKAGE_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == item02.IMS_PR_ROAD_CODE && m.REPACKAGING_STATUS == "N").Any())
                        {
                            message = "All Repackaged Roads are not finalized in this Package. Hence Package can not be Finalized.";
                            return false;
                        }
                    }

                    foreach (var repackageItem in repackage)
                    {
                        EMARG_REPACKAGE_DETAILS rPackage = new EMARG_REPACKAGE_DETAILS();
                        rPackage = dbContext.EMARG_REPACKAGE_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == repackageItem.IMS_PR_ROAD_CODE).FirstOrDefault();
                        rPackage.IS_PACKAGE_FINALIZED = "Y";
                        dbContext.Entry(rPackage).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                    }

                    // Deactivate Individual Road Codes from omms.EMARG_ROAD_DETAILS
                    foreach (var repackageItem in repackage)
                    {
                        List<EMARG_ROAD_DETAILS> erd = dbContext.EMARG_ROAD_DETAILS.Where(m => m.ROAD_CODE == repackageItem.IMS_PR_ROAD_CODE).ToList();
                        if (erd.Count != 0)
                        {
                            foreach (var erdItme in erd)
                            {
                                erdItme.IS_DEACTIVATED = "Y";
                                dbContext.Entry(erdItme).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                    }

                    // Deactivate Individual Road Codes from omms.MANE_EMARG_CONTRACT
                    foreach (var repackageItem in repackage)
                    {
                        List<MANE_EMARG_CONTRACT> mec = dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == repackageItem.IMS_PR_ROAD_CODE).ToList();
                        if (mec.Count != 0)
                        {
                            foreach (var mecItem in mec)
                            {
                                mecItem.IS_DEACTIVATED = "Y";
                                dbContext.Entry(mecItem).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                    }


                    // Deactivate This WHOLE New Package ID details if it exist in omms.EMARG_ROAD_DETAILS
                    List<EMARG_ROAD_DETAILS> emargRoadDetails = dbContext.EMARG_ROAD_DETAILS.Where(m => m.PACKAGE_NO == PackageID).ToList();
                    foreach (var emargRoadDetailsItem in emargRoadDetails)
                    {
                        emargRoadDetailsItem.IS_DEACTIVATED = "Y";
                        dbContext.Entry(emargRoadDetailsItem).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                    }

                    // Deactivate This WHOLE New Package ID details if it exist in omms.MANE_EMARG_CONTRACT
                    foreach (var emargRoadDetailsItem in emargRoadDetails)
                    {
                        List<MANE_EMARG_CONTRACT> maneEmargContract = dbContext.MANE_EMARG_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == emargRoadDetailsItem.ROAD_CODE).ToList();
                        foreach (var maneEmargContractItem in maneEmargContract)
                        {
                            maneEmargContractItem.IS_DEACTIVATED = "Y";
                            dbContext.Entry(maneEmargContractItem).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                    }


                    // Insert above New package roads in omms.Emarg_Road_Details.

                    int Result = dbContext.USP_INSERT_INTO_EMARG_ROAD_DETAILS_ON_REPACKAGING(PackageID);

                    scope.Complete();
                }
                message = "Package finalized Successfully.";
                return true;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementDAL().PackageFinalizeEmargRepackageRoadDAL");
                message = "Package NOT Finalized.";
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region Emarg Repackage at ITNO

        // ITNO Login Comes Here
        public Array MordEmargDAL(string multiDistList, int? page, int? rows, string sidx, string sord, out int totalRecords)
        {
            var dbContext = new PMGSYEntities();
            try
            {

                string[] arrParam = multiDistList.Split(',');

                int[] myInts = Array.ConvertAll(arrParam, s => int.Parse(s));
                string[] Status = { "C", "X" };


                int StateCode = PMGSYSession.Current.StateCode;
                var lstPropsoalsOne = dbContext.USP_GET_EMARG_PACKAGES_FOR_REPACKAGING(StateCode, 0).ToList();// 

                // Take only Completed proposals. where IMS_ISCOMPLETED can only be C or X
                var lstPropsoalsTwo = lstPropsoalsOne.Where(m => Status.Contains(m.IMS_ISCOMPLETED)).ToList();

                // Take on selected district records.
                var lstPropsoals = lstPropsoalsTwo.Where(m => myInts.Contains(m.MAST_DISTRICT_CODE)).ToList();



                totalRecords = lstPropsoals.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                lstPropsoals = lstPropsoals.OrderBy(m => m.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstPropsoals = lstPropsoals.OrderBy(m => m.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstPropsoals = lstPropsoals.OrderBy(m => m.PACKAGE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                            default:
                                lstPropsoals = lstPropsoals.OrderBy(m => m.PACKAGE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.PACKAGE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                            default:
                                lstPropsoals = lstPropsoals.OrderByDescending(m => m.PACKAGE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstPropsoals = lstPropsoals.OrderBy(m => m.PACKAGE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }



                var result = lstPropsoals.Select(m => new
                {
                    m.MAST_STATE_NAME,
                    m.MAST_DISTRICT_NAME,
                    m.MAST_BLOCK_NAME, //= dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == m.IMS_PR_ROAD_CODE).SingleOrDefault().IMS_PACKAGE_ID.ToString(),
                    m.OLD_PACKAGE_NO,
                    m.PACKAGE_NO,
                    m.ROAD_NAME,
                    m.ROAD_CODE
                    // m.PACKAGE_COMPLETION_DATE


                }).ToArray();



                return result.Select(m => new
                {
                    cell = new[] 
                    {
                        
                        m.MAST_STATE_NAME == null?"":m.MAST_STATE_NAME.ToString(),
                        m.MAST_DISTRICT_NAME == null?"":m.MAST_DISTRICT_NAME.ToString(),

                        m.MAST_BLOCK_NAME == null ? "-" : m.MAST_BLOCK_NAME.ToString(),
                        m.OLD_PACKAGE_NO == null?"-":m.OLD_PACKAGE_NO.ToString(),
                        m.PACKAGE_NO==null?"-":m.PACKAGE_NO.ToString(),// Used as Current Package ID
                        m.ROAD_NAME == null?"-":m.ROAD_NAME.ToString(),

                       dbContext.EMARG_REPACKAGE_DETAILS.Any(x=>x.CURRENT_PACKAGE_ID==m.PACKAGE_NO && x.IS_PACKAGE_FINALIZED=="Y")?"-":(dbContext.EMARG_REPACKAGE_DETAILS.Any(x=>x.IMS_PR_ROAD_CODE==m.ROAD_CODE && x.IS_PACKAGE_FINALIZED=="Y")?"<span style='color:green;'>Package is Finalized</span>":(dbContext.EMARG_REPACKAGE_DETAILS.Any(x=>x.IMS_PR_ROAD_CODE==m.ROAD_CODE && x.REPACKAGING_STATUS=="Y")?"<span style='color:red;'>Road is Finalized</span>": "<a href='#' title='Click here to Add Repackaging Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=AddEmargRepackagingDetails('" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+m.ROAD_CODE.ToString().Trim() }) +"'); return false;'>Add Repackaging Details</a>")),
                      

                       dbContext.EMARG_REPACKAGE_DETAILS.Any(x=>x.IMS_PR_ROAD_CODE==m.ROAD_CODE && x.IS_PACKAGE_FINALIZED=="Y")?"<span style='color:green;'>Package is Finalized</span>":(dbContext.EMARG_REPACKAGE_DETAILS.Any(x=>x.IMS_PR_ROAD_CODE==m.ROAD_CODE)?(dbContext.EMARG_REPACKAGE_DETAILS.Any(x=>x.IMS_PR_ROAD_CODE==m.ROAD_CODE && x.REPACKAGING_STATUS=="Y")?"<span style='color:red;'>Road is Finalized</span>":"<a href='#' title='Click here to Finalize Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick=FinalizeEmargRepackagingDetails('" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+m.ROAD_CODE.ToString().Trim() }) +"'); return false;'>Finalize</a>"):"-")

                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetEmargMaintenanceProposalsForRepackaging().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion

        #region Terminated Package Agreement

        public Array GetTerminatedPackageListDAL(int stateCode, int yearCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                var lstExecution = (from item in dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS
                                    join sanctionRoad in dbContext.IMS_SANCTIONED_PROJECTS on item.ROAD_CODE equals sanctionRoad.IMS_PR_ROAD_CODE
                                    join district in dbContext.MASTER_DISTRICT on item.DISTRICT_CODE equals district.MAST_DISTRICT_CODE
                                    join year in dbContext.MASTER_YEAR on sanctionRoad.IMS_YEAR equals year.MAST_YEAR_CODE
                                    join fundingAgency in dbContext.MASTER_FUNDING_AGENCY on sanctionRoad.IMS_COLLABORATION equals fundingAgency.MAST_FUNDING_AGENCY_CODE
                                    where
                                    item.STATE_CODE == stateCode
                                    && item.DISTRICT_CODE == districtCode
                                    && item.BLOCK_CODE == blockCode
                                    && (yearCode == -1 ? 1 : sanctionRoad.IMS_YEAR) == (yearCode == -1 ? 1 : yearCode)
                                    select new
                                    {
                                        district.MAST_DISTRICT_NAME,
                                        year.MAST_YEAR_TEXT,
                                        sanctionRoad.IMS_YEAR,
                                        sanctionRoad.IMS_BATCH,
                                        sanctionRoad.IMS_PACKAGE_ID,
                                        item.EMARG_PACKAGE_NO,
                                        sanctionRoad.IMS_ROAD_NAME,
                                        sanctionRoad.IMS_PAV_LENGTH,
                                        fundingAgency.MAST_FUNDING_AGENCY_NAME,
                                        sanctionRoad.IMS_PR_ROAD_CODE,
                                        item.CONTRACTOR_PAN,
                                        item.DATA_FLAG,
                                        item.AGREEMENT_NO,
                                        item.RECORD_ID,
                                        item.MANE_PR_CONTRACT_CODE,
                                        item.IMS_ROAD_CODE,
                                    }).Distinct();


                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_DISTRICT_NAME":
                                lstExecution = lstExecution.OrderBy(m => m.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_PACKAGE_ID":
                                lstExecution = lstExecution.OrderBy(m => m.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_ROAD_NAME":
                                lstExecution = lstExecution.OrderBy(m => m.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_DISTRICT_NAME":
                                lstExecution = lstExecution.OrderBy(m => m.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_PACKAGE_ID":
                                lstExecution = lstExecution.OrderBy(m => m.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_ROAD_NAME":
                                lstExecution = lstExecution.OrderBy(m => m.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.MAST_DISTRICT_NAME,
                    executionDetails.IMS_YEAR,
                    executionDetails.MAST_YEAR_TEXT,
                    executionDetails.IMS_BATCH,
                    executionDetails.IMS_PACKAGE_ID,
                    executionDetails.EMARG_PACKAGE_NO,
                    executionDetails.IMS_ROAD_NAME,
                    executionDetails.IMS_PAV_LENGTH,
                    executionDetails.MAST_FUNDING_AGENCY_NAME,
                    executionDetails.IMS_PR_ROAD_CODE,
                    executionDetails.CONTRACTOR_PAN,
                    executionDetails.DATA_FLAG,
                    executionDetails.AGREEMENT_NO,
                    executionDetails.RECORD_ID,
                    executionDetails.MANE_PR_CONTRACT_CODE,
                    executionDetails.IMS_ROAD_CODE,
                }).ToArray();

                return result.Select(m => new
                {

                    id = m.IMS_PR_ROAD_CODE.ToString(),
                    cell = new[]
                    {
                            m.MAST_DISTRICT_NAME,
                            m.MAST_YEAR_TEXT,
                            m.IMS_BATCH.ToString(),
                            m.IMS_PACKAGE_ID == null || m.IMS_PACKAGE_ID == string.Empty ? "-" : m.IMS_PACKAGE_ID,
                            m.EMARG_PACKAGE_NO,
                            m.IMS_ROAD_NAME == null || m.IMS_ROAD_NAME == string.Empty ? "-" : m.IMS_ROAD_NAME,
                            m.IMS_PAV_LENGTH.ToString(),
                            m.MAST_FUNDING_AGENCY_NAME,
                            (m.MANE_PR_CONTRACT_CODE == null || m.IMS_ROAD_CODE == null) ? "--" :
                            (dbContext.MANE_IMS_CONTRACT.Where(x => m.IMS_ROAD_CODE == x.IMS_PR_ROAD_CODE && m.MANE_PR_CONTRACT_CODE == x.MANE_PR_CONTRACT_CODE).Select(x => x.MANE_YEAR1_AMOUNT + x.MANE_YEAR2_AMOUNT + x.MANE_YEAR3_AMOUNT + x.MANE_YEAR4_AMOUNT + x.MANE_YEAR5_AMOUNT)).FirstOrDefault().ToString(),
                            m.MANE_PR_CONTRACT_CODE != null ? "N"
                            : URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + m.IMS_PR_ROAD_CODE.ToString(),"SanctionedYear =" +  m.IMS_YEAR.ToString()+"-"+(m.IMS_YEAR+1).ToString(),"Package="+m.IMS_PACKAGE_ID.ToString().Replace("/","--"),"RoadLength="+ m.IMS_PAV_LENGTH.ToString().Replace(".","--")}),
                            m.MANE_PR_CONTRACT_CODE != null && m.IMS_ROAD_CODE != null ?    //m.DATA_FLAG == "Y" ? 
                            "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-search' title='View Details' onClick ='ViewContractorDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"imsRoadID="+m.IMS_PR_ROAD_CODE.ToString(), "AggNo="+ m.AGREEMENT_NO, "PanNumber="+ m.CONTRACTOR_PAN, "SanctionedYear =" +  m.IMS_YEAR.ToString()+"-"+(m.IMS_YEAR+1).ToString(),"Package="+m.IMS_PACKAGE_ID.ToString().Replace("/","--"),"RoadLength="+ m.IMS_PAV_LENGTH.ToString().Replace(".","--")}) + "\");'></span></td></tr></table></center>"
                            : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Details not added'></span></td></tr></table></center>",
                            (m.MANE_PR_CONTRACT_CODE != null && m.IMS_ROAD_CODE != null) && m.DATA_FLAG != "Y" ?
                            "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-trash' title='Delete Details' onClick ='DeleteContractorDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode=" + m.IMS_PR_ROAD_CODE, "ManePrContractCode=" + m.MANE_PR_CONTRACT_CODE}) + "\");'></span></td></tr></table></center>"
                            : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Cannot delete, details are Finalized'></span></td></tr></table></center>",
                            (m.MANE_PR_CONTRACT_CODE != null && m.IMS_ROAD_CODE != null) && m.DATA_FLAG != "Y" ?
                            "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Details' onClick ='FinalizeDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{ "recordId=" + m.RECORD_ID.ToString()}) + "\");'></span></td></tr></table></center>"
                            : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Details Finalized'></span></td></tr></table></center>"
                        }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetTerminatedPackageListDAL().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool SaveContractorDetailsDAL(TerminatedAgreementDetails details_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string proposalType = string.Empty;
                MANE_IMS_CONTRACT agreementDetails = null;
                int IMSPRRoadCode = 0;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                CommonFunctions commonFunction = new CommonFunctions();

                encryptedParameters = details_agreement.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString().Trim());

                string packageID = dbContext.IMS_SANCTIONED_PROJECTS.Where(z => z.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(z => z.IMS_PACKAGE_ID).FirstOrDefault();

                using (var scope = new TransactionScope())
                {
                    agreementDetails = new MANE_IMS_CONTRACT();

                    agreementDetails.IMS_PR_ROAD_CODE = IMSPRRoadCode;
                    agreementDetails.MANE_PR_CONTRACT_CODE = (Int32)GetMaxCode(MaintenanceAgreementModules.IMSContract, IMSPRRoadCode);
                    agreementDetails.MANE_CONTRACT_ID = dbContext.MANE_IMS_CONTRACT.Max(cp => (Int32?)cp.MANE_CONTRACT_ID) == null ? 1 : (Int32)dbContext.MANE_IMS_CONTRACT.Max(cp => (Int32?)cp.MANE_CONTRACT_ID) + 1;
                    agreementDetails.MANE_AGREEMENT_NUMBER = details_agreement.MANE_AGREEMENT_NUMBER;
                    agreementDetails.MANE_AGREEMENT_DATE = commonFunction.GetStringToDateTime(details_agreement.MANE_AGREEMENT_DATE);
                    agreementDetails.MANE_CONSTR_COMP_DATE = commonFunction.GetStringToDateTime(details_agreement.MANE_CONSTR_COMP_DATE);
                    agreementDetails.MANE_MAINTENANCE_START_DATE = commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_START_DATE);

                    if (details_agreement.MANE_MAINTENANCE_END_DATE != null)
                    {
                        agreementDetails.MANE_MAINTENANCE_END_DATE = commonFunction.GetStringToDateTime(details_agreement.MANE_MAINTENANCE_END_DATE);
                    }
                    else
                    {
                        agreementDetails.MANE_MAINTENANCE_END_DATE = null;
                    }

                    agreementDetails.MANE_HANDOVER_DATE = details_agreement.MANE_HANDOVER_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.MANE_HANDOVER_DATE);
                    agreementDetails.MANE_YEAR1_AMOUNT = (Decimal)details_agreement.MANE_YEAR1_AMOUNT;
                    agreementDetails.MANE_YEAR2_AMOUNT = (Decimal)details_agreement.MANE_YEAR2_AMOUNT;
                    agreementDetails.MANE_YEAR3_AMOUNT = (Decimal)details_agreement.MANE_YEAR3_AMOUNT;
                    agreementDetails.MANE_YEAR4_AMOUNT = (Decimal)details_agreement.MANE_YEAR4_AMOUNT;
                    agreementDetails.MANE_YEAR5_AMOUNT = (Decimal)details_agreement.MANE_YEAR5_AMOUNT;
                    agreementDetails.MANE_YEAR6_AMOUNT = (Decimal)(details_agreement.MANE_YEAR6_AMOUNT == null ? 0 : details_agreement.MANE_YEAR6_AMOUNT);

                    agreementDetails.MANE_HANDOVER_TO = details_agreement.MANE_HANDOVER_TO == null ? null : details_agreement.MANE_HANDOVER_TO.Trim();
                    agreementDetails.MANE_CONTRACT_STATUS = "P";
                    agreementDetails.MANE_CONTRACT_FINALIZED = "N";
                    agreementDetails.MANE_LOCK_STATUS = "N";
                    agreementDetails.MANE_AGREEMENT_TYPE = "R";
                    agreementDetails.MANE_FIRST_AGREEMENT = "N";
                    agreementDetails.MANE_PART_AGREEMENT = "Y";

                    agreementDetails.MAST_CON_ID = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_PAN == details_agreement.ContractorPAN).Select(y => y.MAST_CON_ID).FirstOrDefault();
                    agreementDetails.MANE_CONTRACT_NUMBER = dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault() == null ? 1 : dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Max(x => x.MANE_CONTRACT_NUMBER) + 1;
                    agreementDetails.USERID = PMGSYSession.Current.UserId;
                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.MANE_IMS_CONTRACT.Add(agreementDetails);

                    dbContext.SaveChanges();

                    List<MANE_IMS_CONTRACT> contractList = new List<MANE_IMS_CONTRACT>();

                    contractList = dbContext.MANE_IMS_CONTRACT.Where(x => x.IMS_PR_ROAD_CODE == agreementDetails.IMS_PR_ROAD_CODE && x.MANE_PR_CONTRACT_CODE != agreementDetails.MANE_PR_CONTRACT_CODE).ToList();

                    foreach (var item in contractList)
                    {
                        if (item.MANE_AGREEMENT_TYPE == "R")
                            item.MANE_CONTRACT_STATUS = "C";

                        dbContext.Entry(item).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS emargField = dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Where(x => x.RECORD_ID == details_agreement.recordId).FirstOrDefault();
                    emargField.DATA_FLAG = "N";
                    emargField.IMS_ROAD_CODE = agreementDetails.IMS_PR_ROAD_CODE;
                    emargField.MANE_PR_CONTRACT_CODE = agreementDetails.MANE_PR_CONTRACT_CODE;
                    dbContext.Entry(emargField).State = EntityState.Modified;
                    dbContext.SaveChanges();

                    scope.Complete();
                    return true;
                }

            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion
    }

    public interface IMaintenanceAgreementDAL
    {

        #region MAINTENANCE_AGREEMENT

        Array GetCompletedRoadListDAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetAgreementDetailsListDAL_Proposal(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveAgreementDetailsDAL_Proposal(MaintenanceAgreementDetails details_agreement, ref string message);

        bool GetExistingAgreementDetailsDAL(int IMSPRRoadCoderef, int IMSWorkCode, ref int contractorID, ref string agreementNumber, ref  string agreementDate, ref decimal? year1, ref decimal? year2, ref decimal? year3, ref decimal? year4, ref decimal? year5, ref string message);

        MaintenanceAgreementDetails GetMaintenanceAgreementDetailsDAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId, bool isView = false);

        bool UpdateMaintenanceAgreementDetailsDAL_Proposal(MaintenanceAgreementDetails details_agreement, ref string message);

        bool FinalizeAgreementDAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId);

        bool DeFinalizeAgreementDAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId);

        bool ChangeAgreementStatusToInCompleteDAL(Models.Agreement.IncompleteReason incompleteReason, ref string message);

        bool DeleteMaintenanceAgreementDetailsDAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId, ref string message);

        bool CheckForExistingorNewContractorDAL(int IMSPRRoadCode, int IMSWorkCode);

        bool ChangeAgreementStatusToCompleteDAL(int IMSPRRoadCode, int PRContractCode);

        #endregion

        #region SPECIAL_MAINTENANCE_AGREEMENT

        Array GetCompletedRoadForSpecialAgreementsListDAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetSpecialAgreementDetailsListDAL(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool SaveSpecialAgreementDetailsDAL(MaintenanceAgreementDetails model, ref string message);
        bool UpdateSpecialAgreementDetailsDAL(MaintenanceAgreementDetails model, ref string message);

        #endregion

        #region PROPOSAL_RELATED_DETAILS

        Array GetProposalAgreementListDAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetProposalFinancialListDAL(int proposalCode, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion

        #region Periodic Maintenance
        Array GetPeriodicCompletedRoadListDAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);

        Boolean AddPeriodicMaintenanceDAL(AddPeriodicMaintenanceModel model, out String message);
        Boolean EditPeriodicMaintenanceDAL(AddPeriodicMaintenanceModel model, out String message);

        AddPeriodicMaintenanceModel GetPeriodicMentainanceModelDAL(Int32 imsRoadCode);
        List<AddPeriodicMaintenanceModel> GetPariodicMaintenceViewListDAL(Int32 imsRoadCode, double RoadLength);
        Array ViewPeriodicmaintenanceListDAL(int RodeCode, int page, int rows, string sidx, string sord, out long totalRecords, decimal RoadLength, out String AddButton);
        #endregion

        #region Maintenance Work Repackaging

        Array GetMaintenanceProposalsForRepackaging(int? page, int? rows, string sidx, string sord, out long totalRecords, int year, int batch, int block, string package, int collaboration, string proposalType, string upgradationType);

        bool AddMaintenanceRepackagingDetails(PMGSY.Models.MaintenanceAgreement.RepackagingDetailsViewModel model);

        List<SelectListItem> PopulateMaintenancePackageBlockWise(int blockCode);

        #endregion

        #region Terminated Package Agreement

        Array GetTerminatedPackageListDAL(int stateCode, int yearCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool SaveContractorDetailsDAL(TerminatedAgreementDetails details_agreement, ref string message);

        #endregion

    }
}