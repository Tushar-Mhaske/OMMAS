/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: AgreementDAL.cs

 * Author : Koustubh Nakate

 * Creation Date :18/June/2013

 * Desc : This class is used as data access layer to perform Save,Edit,Delete and listing of agreement screens.  
 * ---------------------------------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Common;
using PMGSY.Models.Agreement;
using System.Data.Entity;
using System.Transactions;
using System.Web.Mvc;
using PMGSY.Extensions;
using System.Configuration;
//using System.Data.Entity.Core.SqlClient;
using System.Data.SqlClient;
using System.Data.Entity.Core;
using System.Data.Entity.SqlServer;

namespace PMGSY.DAL.Agreement
{

    public enum AgreementModules
    {
        AgreementMaster = 1,
        AgreementDetails


    };

    public class ProposalTypes
    {
        public string TypeID { get; set; }
        public string ProposalType { get; set; }

        public static readonly Dictionary<string, string> lstProposalTypes = new Dictionary<string, string>() { { "0", "All" }, { "L", "LSB" }, { "P", "Road" }, { "B", "Building" } };
    }

    public class AgreementStatus_Search
    {
        public string StatusID { get; set; }
        public string Status { get; set; }

        public static readonly Dictionary<string, string> lstStatus = new Dictionary<string, string>() { { "0", "All" }, { "C", "Completed" }, { "W", "InCompleted" }, { "P", "InProgress" } };
    }

    public class AgreementDAL : IAgreementDAL
    {

        private readonly Dictionary<String, String> AgreementTypes = new Dictionary<string, string>() { { "C", "Contractor" }, { "S", "Supplier" }, { "D", "DPR" }, { "O", "Other Road" }, { "R", "Special Agreement" } };
        private readonly Dictionary<String, String> AgreementStatus = new Dictionary<string, string>() { { "P", "In Progress" }, { "W", "Agreement Terminated" }, { "M", "Maintenance Phase" }, { "C", "Agreement Completed" }, { "Y", "In Progress" }, { "N", "Agreement Terminated" } };

        private readonly Dictionary<String, String> WorkStatus = new Dictionary<string, string>() { { "P", "In Progress" }, { "W", "Work Terminated" }, { "M", "Maintenance Phase" }, { "C", "Work Completed" }, { "Y", "In Progress" }, { "N", "Work Terminated" } };

        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;


        #region AGREEMENT_DATA_ENTRY

        /// <summary>
        /// This function is used to calculated max code
        /// </summary>
        /// <param name="module"> MasterDataEntryModules object</param>
        /// <returns> MaxCode</returns>

        private Int64 GetMaxCode(AgreementModules module)
        {
            Int64? maxCode = 0;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                switch (module)
                {
                    case AgreementModules.AgreementMaster:
                        maxCode = (from agreementMaster in dbContext.TEND_AGREEMENT_MASTER select (Int64?)agreementMaster.TEND_AGREEMENT_CODE).Max();
                        break;

                    case AgreementModules.AgreementDetails:
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



        public Array GetProposedRoadListDAL(bool isSplitWork, int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, string proposalType, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (packageID.Contains("All"))
                {
                    packageID = "All Packages";
                }
                bool isAgreementExist = false;

                var query = from imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
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
                                ///Changes for RCPLWE
                            ((PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56) ? 0 : imsSanctionedProjectDetails.MAST_DPIU_CODE) == ((PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56) ? 0 : adminNDCode) &&
                            imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
                            imsSanctionedProjectDetails.IMS_DPR_STATUS == "N" &&
                            (sanctionedYear == 0 ? 1 : imsSanctionedProjectDetails.IMS_YEAR) == (sanctionedYear == 0 ? 1 : sanctionedYear) &&
                            (blockCode <= 0 ? 1 : imsSanctionedProjectDetails.MAST_BLOCK_CODE) == (blockCode <= 0 ? 1 : blockCode) &&
                            (packageID == "All Packages" ? "%" : imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper()) == (packageID == "All Packages" ? "%" : packageID.ToUpper()) &&
                            (proposalType == "0" ? "%" : imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE.ToUpper()) == (proposalType == "0" ? "%" : proposalType.ToUpper()) &&
                                //new filters added by Vikram 
                            (batch == 0 ? 1 : imsSanctionedProjectDetails.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
                            (collaboration <= 0 ? 1 : imsSanctionedProjectDetails.IMS_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                            (upgradationType == "0" ? "%" : imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT) == (upgradationType == "0" ? "%" : upgradationType) &&
                                //end of change
                            imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme // to list the details according to the Scheme in Session
                            // && imsSanctionedProjectDetails.IMS_SANCTIONED_DATE <= new DateTime(2020, 03, 31)
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

                                imsSanctionedProjectDetails.IMS_PUCCA_SIDE_DRAINS,  // Added on 14 Sept 2020

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
                                imsSanctionedProjectDetails.IMS_BRIDGE_NAME,
                                imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH,
                                imsSanctionedProjectDetails.IMS_SANCTIONED_DATE,
                                imsSanctionedProjectDetails.IMS_BATCH,
                                imsSanctionedProjectDetails.MASTER_BLOCK.MAST_BLOCK_NAME

                            };

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
                            case "WorkType":
                                query = query.OrderBy(x => x.IMS_PROPOSAL_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "Batch":
                                query = query.OrderBy(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Block":
                                query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_PROPOSAL_TYPE).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "WorkType":
                                query = query.OrderByDescending(x => x.IMS_PROPOSAL_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "Batch":
                                query = query.OrderByDescending(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Block":
                                query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_PROPOSAL_TYPE).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                    imsSanctionedProjectDetails.IMS_BRIDGE_NAME,
                    imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_DATE,
                    imsSanctionedProjectDetails.IMS_BATCH,
                    imsSanctionedProjectDetails.MAST_BLOCK_NAME
                }).ToArray();


                return result.Select(imsSanctionedProjectDetails => new
                {
                    //isAgreementExist = dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == imsSanctionedProjectDetails.IMS_PR_ROAD_CODE && ad.TEND_AGREEMENT_STATUS == "P"),
                    cell = new[] {                                                                               
                                    imsSanctionedProjectDetails.MAST_BLOCK_NAME == null?"-":imsSanctionedProjectDetails.MAST_BLOCK_NAME.ToString(),
                                    imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString() ,
                                    imsSanctionedProjectDetails.IMS_BATCH == null?"NA":"Batch "+ imsSanctionedProjectDetails.IMS_BATCH.ToString(),
                                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,                
                                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"?imsSanctionedProjectDetails.IMS_ROAD_NAME.ToString():(imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "L" ? (imsSanctionedProjectDetails.IMS_BRIDGE_NAME==null?"NA":imsSanctionedProjectDetails.IMS_BRIDGE_NAME.ToString()) : (imsSanctionedProjectDetails.IMS_ROAD_NAME==null?"NA":imsSanctionedProjectDetails.IMS_ROAD_NAME.ToString())),
                                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"?"Road":(imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "L" ? "Bridge" : "Building"),
                                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"?imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString():(imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "L" ? (imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH==null?"NA":imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH.ToString()) : (imsSanctionedProjectDetails.IMS_PAV_LENGTH==null?"NA":imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString())),//imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString(),
                                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME==null?"NA":imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME.Trim(),
                                    
                                    //imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? 
                                    //(imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT+
                                    //imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT).ToString() : (imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "L" ? (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT).ToString() : (imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT).ToString()),    

                                    ///Change made by SAMMED PATIL on 29MAR2016 
                                    (PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ? 
                                                      ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT)).ToString()
                                                    : ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT)+ (imsSanctionedProjectDetails.IMS_PUCCA_SIDE_DRAINS == null ? 0 : imsSanctionedProjectDetails.IMS_PUCCA_SIDE_DRAINS)).ToString(),
                                    
                                    dbContext.TEND_AGREEMENT_DETAIL.Any(m=>m.IMS_PR_ROAD_CODE == imsSanctionedProjectDetails.IMS_PR_ROAD_CODE) ? dbContext.TEND_AGREEMENT_DETAIL.Where(m=>m.IMS_PR_ROAD_CODE == imsSanctionedProjectDetails.IMS_PR_ROAD_CODE && (m.TEND_AGREEMENT_STATUS == "C" || m.TEND_AGREEMENT_STATUS == "P")).Select(m=>m.TEND_AGREEMENT_AMOUNT).FirstOrDefault().ToString() : "-",

                                    (imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5).ToString(),                                    
                                    //isSplitWork==false?(URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + (imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"?imsSanctionedProjectDetails.IMS_ROAD_NAME:(imsSanctionedProjectDetails.IMS_BRIDGE_NAME==null?"NA":imsSanctionedProjectDetails.IMS_BRIDGE_NAME.ToString())),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID, "ProposalType="+ imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE, "RoadLength="+(imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--"):(imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH==null?"NA": imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH.ToString().Replace(".","--"))), "SanctionedDate=" + (imsSanctionedProjectDetails.IMS_SANCTIONED_DATE==null? string.Empty: Convert.ToDateTime(imsSanctionedProjectDetails.IMS_SANCTIONED_DATE).ToString("dd/MM/yyyy").Replace("/","--"))   })  )   : ( URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + imsSanctionedProjectDetails.IMS_ROAD_NAME.ToString(),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID, "ProposalType="+ imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE, "RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")   }) )        //"RoadLength="+imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString()
                                    
                                   ///Change made by SAMMED PATIL on 21JUNE2017 for 
                                    isSplitWork==false 
                                    ? (URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()                                                               +"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),/*"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID,*/ "ProposalType="+ imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,                                                                           "RoadLength="+(imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--"):(imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH==null?"NA":                                                       imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH.ToString().Replace(".","--"))), "SanctionedDate=" + (imsSanctionedProjectDetails.IMS_SANCTIONED_DATE==null? string.Empty: Convert.ToDateTime                                                                  (imsSanctionedProjectDetails.IMS_SANCTIONED_DATE).ToString("dd/MM/yyyy").Replace("/","--"))   })  )   
                                    : (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == imsSanctionedProjectDetails.IMS_PR_ROAD_CODE && ad.TEND_AGREEMENT_STATUS == "P") 
                                        ? ("<a href='#' title='View Split Work' class='ui-icon ui-icon-zoomin ui-align-center' onClick=SplitWork('" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" +                                                                                  imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString                                                                         (),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID, "ProposalType="+ imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE, "RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace                                                      (".","--")   }) +"'); return false;'>View Split Work Details</a>")
                                        ///if Agreement entered against road then skip split works 19JUN2019
                                        : ((dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == imsSanctionedProjectDetails.IMS_PR_ROAD_CODE))
                                           ? "-" 
                                           : ("<a href='#' title='Split Work' onClick=SplitWork('" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"SanctionedYear =" +                                                         imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID, "ProposalType="+                                                                             imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE, "RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")   }) +"'); return false;'>Split</a>")))
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

        public Array GetProposedRoadITNOListDAL(bool isSplitWork, int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, string proposalType, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (packageID.Contains("All"))
                {
                    packageID = "All Packages";
                }

                var query = from imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
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
                            ((PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47) ? 0 : imsSanctionedProjectDetails.MAST_DPIU_CODE) == ((PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47) ? 0 : adminNDCode) &&
                            imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
                            imsSanctionedProjectDetails.IMS_DPR_STATUS == "N" &&
                            (sanctionedYear == 0 ? 1 : imsSanctionedProjectDetails.IMS_YEAR) == (sanctionedYear == 0 ? 1 : sanctionedYear) &&
                            (blockCode <= 0 ? 1 : imsSanctionedProjectDetails.MAST_BLOCK_CODE) == (blockCode <= 0 ? 1 : blockCode) &&
                            (packageID == "All Packages" ? "%" : imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper()) == (packageID == "All Packages" ? "%" : packageID.ToUpper()) &&
                            (proposalType == "0" ? "%" : imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE.ToUpper()) == (proposalType == "0" ? "%" : proposalType.ToUpper()) &&
                                //new filters added by Vikram 
                            (batch == 0 ? 1 : imsSanctionedProjectDetails.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
                            (collaboration <= 0 ? 1 : imsSanctionedProjectDetails.IMS_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                            (upgradationType == "0" ? "%" : imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT) == (upgradationType == "0" ? "%" : upgradationType) &&
                                //end of change
                            imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme // to list the details according to the Scheme in Session
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
                                imsSanctionedProjectDetails.IMS_BRIDGE_NAME,
                                imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH,
                                imsSanctionedProjectDetails.IMS_SANCTIONED_DATE,
                                imsSanctionedProjectDetails.IMS_BATCH,
                                imsSanctionedProjectDetails.MASTER_BLOCK.MAST_BLOCK_NAME

                            };

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
                            case "WorkType":
                                query = query.OrderBy(x => x.IMS_PROPOSAL_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "Batch":
                                query = query.OrderBy(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Block":
                                query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_PROPOSAL_TYPE).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "WorkType":
                                query = query.OrderByDescending(x => x.IMS_PROPOSAL_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "Batch":
                                query = query.OrderByDescending(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Block":
                                query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_PROPOSAL_TYPE).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                    imsSanctionedProjectDetails.IMS_BRIDGE_NAME,
                    imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_DATE,
                    imsSanctionedProjectDetails.IMS_BATCH,
                    imsSanctionedProjectDetails.MAST_BLOCK_NAME
                }).ToArray();


                return result.Select(imsSanctionedProjectDetails => new
                {

                    cell = new[] {                                                                               
                                    imsSanctionedProjectDetails.MAST_BLOCK_NAME == null?"-":imsSanctionedProjectDetails.MAST_BLOCK_NAME.ToString(),
                                    imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString() ,
                                    imsSanctionedProjectDetails.IMS_BATCH == null?"NA":"Batch "+ imsSanctionedProjectDetails.IMS_BATCH.ToString(),
                                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,                
                                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"?imsSanctionedProjectDetails.IMS_ROAD_NAME.ToString():(imsSanctionedProjectDetails.IMS_BRIDGE_NAME==null?"NA":imsSanctionedProjectDetails.IMS_BRIDGE_NAME.ToString()),
                                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"?"Road":"Bridge",
                                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"?imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString():(imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH==null?"NA":imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH.ToString()),//imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString(),
                                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME==null?"NA":imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME.Trim(),                                                              
                                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? 
                                    (imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT+
                                    imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT).ToString() : (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT).ToString(),    
                                    dbContext.TEND_AGREEMENT_DETAIL.Any(m=>m.IMS_PR_ROAD_CODE == imsSanctionedProjectDetails.IMS_PR_ROAD_CODE) ? dbContext.TEND_AGREEMENT_DETAIL.Where(m=>m.IMS_PR_ROAD_CODE == imsSanctionedProjectDetails.IMS_PR_ROAD_CODE && (m.TEND_AGREEMENT_STATUS == "C" || m.TEND_AGREEMENT_STATUS == "P")).Select(m=>m.TEND_AGREEMENT_AMOUNT).FirstOrDefault().ToString() : "-",
                                    (imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5).ToString(),                                    
                                    //isSplitWork==false?(URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + (imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"?imsSanctionedProjectDetails.IMS_ROAD_NAME:(imsSanctionedProjectDetails.IMS_BRIDGE_NAME==null?"NA":imsSanctionedProjectDetails.IMS_BRIDGE_NAME.ToString())),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID, "ProposalType="+ imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE, "RoadLength="+(imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--"):(imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH==null?"NA": imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH.ToString().Replace(".","--"))), "SanctionedDate=" + (imsSanctionedProjectDetails.IMS_SANCTIONED_DATE==null? string.Empty: Convert.ToDateTime(imsSanctionedProjectDetails.IMS_SANCTIONED_DATE).ToString("dd/MM/yyyy").Replace("/","--"))   })  )   : ( URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + imsSanctionedProjectDetails.IMS_ROAD_NAME.ToString(),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID, "ProposalType="+ imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE, "RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")   }) )        //"RoadLength="+imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString()
                                    isSplitWork==false?(URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID, "ProposalType="+ imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE, "RoadLength="+(imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--"):(imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH==null?"NA": imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH.ToString().Replace(".","--"))), "SanctionedDate=" + (imsSanctionedProjectDetails.IMS_SANCTIONED_DATE==null? string.Empty: Convert.ToDateTime(imsSanctionedProjectDetails.IMS_SANCTIONED_DATE).ToString("dd/MM/yyyy").Replace("/","--"))   })  )   : ( URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID, "ProposalType="+ imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE, "RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")   }) )        
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


        public List<MASTER_CONTRACTOR> GetAllContractor(int stateCode, string conSupFlag, bool isSearch)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<MASTER_CONTRACTOR> lstContractorWithPAN = new List<MASTER_CONTRACTOR>();

                conSupFlag = conSupFlag == "S" ? "S" : "C";
                List<MASTER_CONTRACTOR> lstContractor =
                                                        PMGSYSession.Current.RoleCode == 36 ?
                                                         (
                                                            from contractor in dbContext.MASTER_CONTRACTOR
                                                            select contractor
                                                         ).ToList<MASTER_CONTRACTOR>()
                                                         :
                                                         (from contractor in dbContext.MASTER_CONTRACTOR
                                                          join contractorReg in dbContext.MASTER_CONTRACTOR_REGISTRATION
                                                          on contractor.MAST_CON_ID equals contractorReg.MAST_CON_ID
                                                          where
                                                          contractorReg.MAST_REG_STATE == stateCode &&
                                                          contractorReg.MAST_REG_STATUS == "A" &&
                                                          contractor.MAST_CON_STATUS == "A"
                                                          // && contractor.MAST_CON_SUP_FLAG == conSupFlag
                                                          select contractor).ToList<MASTER_CONTRACTOR>()
                                                         ;
                lstContractor = lstContractor.Where(c => c.MAST_CON_COMPANY_NAME != null).ToList<MASTER_CONTRACTOR>();


                foreach (MASTER_CONTRACTOR contractor in lstContractor)
                {
                    lstContractorWithPAN.Add(new MASTER_CONTRACTOR() { MAST_CON_ID = contractor.MAST_CON_ID, MAST_CON_COMPANY_NAME = (contractor.MAST_CON_COMPANY_NAME + "(" + (string.IsNullOrWhiteSpace(contractor.MAST_CON_PAN) == true ? "" : contractor.MAST_CON_PAN) + (string.IsNullOrWhiteSpace(contractor.MAST_CON_PAN) == true ? "" : ",") + contractor.MAST_CON_ID + ")") });
                }

                if (!isSearch)
                {
                    if (conSupFlag.Equals("S"))
                    {
                        //lstContractor.Insert(0, new MASTER_CONTRACTOR() { MAST_CON_ID = 0, MAST_CON_COMPANY_NAME = "Select Supplier" });
                        lstContractorWithPAN.Insert(0, new MASTER_CONTRACTOR() { MAST_CON_ID = 0, MAST_CON_COMPANY_NAME = "Select Supplier" });
                    }
                    else
                    {
                        //lstContractor.Insert(0, new MASTER_CONTRACTOR() { MAST_CON_ID = 0, MAST_CON_COMPANY_NAME = "Select Contractor" });
                        lstContractorWithPAN.Insert(0, new MASTER_CONTRACTOR() { MAST_CON_ID = 0, MAST_CON_COMPANY_NAME = "Select Contractor" });
                    }

                }

                //var contractors = from list in lstContractor 
                //        select new {
                //            MAST_CON_ID=list.MAST_CON_ID,
                //            MAST_CON_COMPANY_NAME = list.MAST_CON_COMPANY_NAME + " " + list.MAST_CON_PAN
                //        };



                return lstContractorWithPAN;
                //return lstContractor;
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


        public Array GetAgreementDetailsListDAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                //var data = context.Database.ExecuteSqlCommand("Yourprocedure @param, @param1",
                //              param1, param2);


                //var query = from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                //            join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                //            on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                //            join imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                //            on tendAgreementDetails.IMS_PR_ROAD_CODE equals imsSanctionedProjectDetails.IMS_PR_ROAD_CODE                           
                //            join contractorDetails in dbContext.MASTER_CONTRACTOR
                //            on tendAgreementMaster.MAST_CON_ID equals contractorDetails.MAST_CON_ID into contractors
                //            from contractorDetails in contractors.DefaultIfEmpty()                    
                //            where
                //            tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode                            
                //            select new
                //            {
                //                tendAgreementDetails.TEND_AGREEMENT_ID,
                //                tendAgreementMaster.TEND_AGREEMENT_CODE,
                //                imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                //                imsSanctionedProjectDetails.IMS_ROAD_NAME,
                //                contractorDetails.MAST_CON_COMPANY_NAME,
                //                tendAgreementMaster.TEND_AGREEMENT_TYPE,
                //                tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                //                tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                //                //tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                //                tendAgreementDetails.TEND_AGREEMENT_AMOUNT,
                //                tendAgreementDetails.TEND_AMOUNT_YEAR1,
                //                tendAgreementDetails.TEND_AMOUNT_YEAR2,
                //                tendAgreementDetails.TEND_AMOUNT_YEAR3,
                //                tendAgreementDetails.TEND_AMOUNT_YEAR4,
                //                tendAgreementDetails.TEND_AMOUNT_YEAR5,
                //                tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                //                tendAgreementMaster.TEND_LOCK_STATUS,
                //                tendAgreementDetails.TEND_AGREEMENT_STATUS
                //            };

                //var agreementCodes = (from tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                //                      where
                //                      tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                //                      tendAgreementDetails.TEND_AGREEMENT_STATUS != "W"
                //                      select new { tendAgreementDetails.TEND_AGREEMENT_CODE }).Distinct();

                var query = (from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                             join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                             on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                             join imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                             on tendAgreementDetails.IMS_PR_ROAD_CODE equals imsSanctionedProjectDetails.IMS_PR_ROAD_CODE
                             join contractorDetails in dbContext.MASTER_CONTRACTOR
                             on tendAgreementMaster.MAST_CON_ID equals contractorDetails.MAST_CON_ID into contractors
                             from contractorDetails in contractors.DefaultIfEmpty()
                             where
                             tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                 //tendAgreementMaster.TEND_AGREEMENT_STATUS != "W" &&
                                 //tendAgreementDetails.TEND_AGREEMENT_STATUS != "W" &&
                             (agreementType == string.Empty ? "%" : tendAgreementMaster.TEND_AGREEMENT_TYPE) == (agreementType == string.Empty ? "%" : agreementType)
                                 //  group tendAgreementMaster.TEND_AGREEMENT_CODE by tendAgreementDetails.TEND_AGREEMENT_CODE into agreementDetails
                             &&
                             tendAgreementMaster.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme // to list the details according to the Scheme in Session
                             select new
                             {
                                 tendAgreementDetails.TEND_AGREEMENT_ID,
                                 tendAgreementMaster.TEND_AGREEMENT_CODE,
                                 imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                                 imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                 MAST_CON_COMPANY_NAME = contractorDetails.MAST_CON_COMPANY_NAME + " - (" + SqlFunctions.StringConvert((decimal)contractorDetails.MAST_CON_ID).Trim() + "),  " + (contractorDetails.MAST_CON_PAN != "" ? " (" + contractorDetails.MAST_CON_PAN + ")" : ""),
                                 tendAgreementMaster.TEND_AGREEMENT_TYPE,
                                 tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                                 tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                                 //tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                                 //tendAgreementDetails.TEND_AGREEMENT_AMOUNT,
                                 //tendAgreementDetails.TEND_AMOUNT_YEAR1,
                                 //tendAgreementDetails.TEND_AMOUNT_YEAR2,
                                 //tendAgreementDetails.TEND_AMOUNT_YEAR3,
                                 //tendAgreementDetails.TEND_AMOUNT_YEAR4,
                                 //tendAgreementDetails.TEND_AMOUNT_YEAR5,
                                 tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR1,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR2,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR3,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR4,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR5,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR6,
                                 tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                                 tendAgreementMaster.TEND_LOCK_STATUS,
                                 tendAgreementMaster.TEND_AGREEMENT_STATUS,
                                 tendAgreementMaster.MAST_PMGSY_SCHEME, //new change done by Vikram on 10 Feb 2014

                                  //adedd on 15-07-2022
                                 tendAgreementDetails.GST_AMT_MAINT_AGREEMENT_DLP,
                             });


                query = query.GroupBy(tm => tm.TEND_AGREEMENT_CODE).Select(tm => tm.FirstOrDefault());

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
                            case "AgreementType":
                                query = query.OrderBy(x => x.TEND_AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderBy(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "AgreementType":
                                query = query.OrderByDescending(x => x.TEND_AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderByDescending(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(tendAgreementMaster => new
                {
                    tendAgreementMaster.TEND_AGREEMENT_ID,
                    tendAgreementMaster.TEND_AGREEMENT_CODE,
                    tendAgreementMaster.IMS_PR_ROAD_CODE,
                    tendAgreementMaster.IMS_ROAD_NAME,
                    tendAgreementMaster.MAST_CON_COMPANY_NAME,
                    tendAgreementMaster.TEND_AGREEMENT_TYPE,
                    tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                    tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                    tendAgreementMaster.TEND_AMOUNT_YEAR1,
                    tendAgreementMaster.TEND_AMOUNT_YEAR2,
                    tendAgreementMaster.TEND_AMOUNT_YEAR3,
                    tendAgreementMaster.TEND_AMOUNT_YEAR4,
                    tendAgreementMaster.TEND_AMOUNT_YEAR5,
                    tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                    tendAgreementMaster.TEND_LOCK_STATUS,
                    tendAgreementMaster.TEND_AGREEMENT_STATUS,
                    tendAgreementMaster.TEND_AMOUNT_YEAR6,

                    //added on 15-07-2022
                    tendAgreementMaster.GST_AMT_MAINT_AGREEMENT_DLP
                }).ToArray();


                return result.Select(tendAgreementMaster => new
                {
                    id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {                                                                               
                                    // tendAgreementDetails.IMS_ROAD_NAME,
                                   // tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    tendAgreementMaster.TEND_AGREEMENT_NUMBER.ToString(),
                                    tendAgreementMaster.MAST_CON_COMPANY_NAME==null?"NA":tendAgreementMaster.MAST_CON_COMPANY_NAME.ToString().Trim(),
                                    AgreementTypes[tendAgreementMaster.TEND_AGREEMENT_TYPE].ToString(), 
                                    Convert.ToDateTime(tendAgreementMaster.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),
                                   // Agreement Amount with GST Amt on 15-07-2022
                                    (tendAgreementMaster.TEND_AGREEMENT_AMOUNT + tendAgreementMaster.GST_AMT_MAINT_AGREEMENT_DLP).ToString(),

                                    ((tendAgreementMaster.TEND_AMOUNT_YEAR1==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR1)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR2==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR2)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR3==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR3)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR4==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR4)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR5==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR5) + (PMGSYSession.Current.PMGSYScheme == 2?(tendAgreementMaster.TEND_AMOUNT_YEAR6==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR6):0)
                                    ).ToString(),
                                     AgreementStatus[tendAgreementMaster.TEND_AGREEMENT_STATUS].ToString(),
                                    // (tendAgreementMaster.TEND_AGREEMENT_STATUS =="W")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }),
                                     (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="N" && tendAgreementMaster.TEND_LOCK_STATUS=="N" && tendAgreementMaster.TEND_AGREEMENT_STATUS!="W") ?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='FinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Finalized'></span></td></tr></table></center>",
                                     
                                      URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }),

                                     (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47) ? URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }) : (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementMaster.TEND_LOCK_STATUS=="Y" || tendAgreementMaster.TEND_AGREEMENT_STATUS=="W")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }),
                                    (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementMaster.TEND_LOCK_STATUS=="Y" || tendAgreementMaster.TEND_AGREEMENT_STATUS=="W")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() })
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

        public Array GetAgreementDetailsListITNODAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                //var data = context.Database.ExecuteSqlCommand("Yourprocedure @param, @param1",
                //              param1, param2);


                //var query = from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                //            join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                //            on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                //            join imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                //            on tendAgreementDetails.IMS_PR_ROAD_CODE equals imsSanctionedProjectDetails.IMS_PR_ROAD_CODE                           
                //            join contractorDetails in dbContext.MASTER_CONTRACTOR
                //            on tendAgreementMaster.MAST_CON_ID equals contractorDetails.MAST_CON_ID into contractors
                //            from contractorDetails in contractors.DefaultIfEmpty()                    
                //            where
                //            tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode                            
                //            select new
                //            {
                //                tendAgreementDetails.TEND_AGREEMENT_ID,
                //                tendAgreementMaster.TEND_AGREEMENT_CODE,
                //                imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                //                imsSanctionedProjectDetails.IMS_ROAD_NAME,
                //                contractorDetails.MAST_CON_COMPANY_NAME,
                //                tendAgreementMaster.TEND_AGREEMENT_TYPE,
                //                tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                //                tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                //                //tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                //                tendAgreementDetails.TEND_AGREEMENT_AMOUNT,
                //                tendAgreementDetails.TEND_AMOUNT_YEAR1,
                //                tendAgreementDetails.TEND_AMOUNT_YEAR2,
                //                tendAgreementDetails.TEND_AMOUNT_YEAR3,
                //                tendAgreementDetails.TEND_AMOUNT_YEAR4,
                //                tendAgreementDetails.TEND_AMOUNT_YEAR5,
                //                tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                //                tendAgreementMaster.TEND_LOCK_STATUS,
                //                tendAgreementDetails.TEND_AGREEMENT_STATUS
                //            };

                //var agreementCodes = (from tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                //                      where
                //                      tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                //                      tendAgreementDetails.TEND_AGREEMENT_STATUS != "W"
                //                      select new { tendAgreementDetails.TEND_AGREEMENT_CODE }).Distinct();

                var query = (from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                             join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                             on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                             join imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                             on tendAgreementDetails.IMS_PR_ROAD_CODE equals imsSanctionedProjectDetails.IMS_PR_ROAD_CODE
                             join contractorDetails in dbContext.MASTER_CONTRACTOR
                             on tendAgreementMaster.MAST_CON_ID equals contractorDetails.MAST_CON_ID into contractors
                             from contractorDetails in contractors.DefaultIfEmpty()
                             where
                             tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                 //tendAgreementMaster.TEND_AGREEMENT_STATUS != "W" &&
                                 //tendAgreementDetails.TEND_AGREEMENT_STATUS != "W" &&
                             (agreementType == string.Empty ? "%" : tendAgreementMaster.TEND_AGREEMENT_TYPE) == (agreementType == string.Empty ? "%" : agreementType)
                                 //  group tendAgreementMaster.TEND_AGREEMENT_CODE by tendAgreementDetails.TEND_AGREEMENT_CODE into agreementDetails
                             &&
                             tendAgreementMaster.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme // to list the details according to the Scheme in Session
                             select new
                             {
                                 tendAgreementDetails.TEND_AGREEMENT_ID,
                                 tendAgreementMaster.TEND_AGREEMENT_CODE,
                                 imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                                 imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                 contractorDetails.MAST_CON_COMPANY_NAME,
                                 tendAgreementMaster.TEND_AGREEMENT_TYPE,
                                 tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                                 tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                                 //tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                                 //tendAgreementDetails.TEND_AGREEMENT_AMOUNT,
                                 //tendAgreementDetails.TEND_AMOUNT_YEAR1,
                                 //tendAgreementDetails.TEND_AMOUNT_YEAR2,
                                 //tendAgreementDetails.TEND_AMOUNT_YEAR3,
                                 //tendAgreementDetails.TEND_AMOUNT_YEAR4,
                                 //tendAgreementDetails.TEND_AMOUNT_YEAR5,
                                 tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR1,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR2,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR3,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR4,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR5,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR6,
                                 tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                                 tendAgreementMaster.TEND_LOCK_STATUS,
                                 tendAgreementMaster.TEND_AGREEMENT_STATUS,
                                 tendAgreementDetails.TEND_INCLUDE_ROAD_AMT,
                                 tendAgreementMaster.MAST_PMGSY_SCHEME //new change done by Vikram on 10 Feb 2014
                             });


                query = query.GroupBy(tm => tm.TEND_AGREEMENT_CODE).Select(tm => tm.FirstOrDefault());

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
                            case "AgreementType":
                                query = query.OrderBy(x => x.TEND_AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderBy(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "AgreementType":
                                query = query.OrderByDescending(x => x.TEND_AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderByDescending(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(tendAgreementMaster => new
                {
                    tendAgreementMaster.TEND_AGREEMENT_ID,
                    tendAgreementMaster.TEND_AGREEMENT_CODE,
                    tendAgreementMaster.IMS_PR_ROAD_CODE,
                    tendAgreementMaster.IMS_ROAD_NAME,
                    tendAgreementMaster.MAST_CON_COMPANY_NAME,
                    tendAgreementMaster.TEND_AGREEMENT_TYPE,
                    tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                    tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                    tendAgreementMaster.TEND_AMOUNT_YEAR1,
                    tendAgreementMaster.TEND_AMOUNT_YEAR2,
                    tendAgreementMaster.TEND_AMOUNT_YEAR3,
                    tendAgreementMaster.TEND_AMOUNT_YEAR4,
                    tendAgreementMaster.TEND_AMOUNT_YEAR5,
                    tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                    tendAgreementMaster.TEND_LOCK_STATUS,
                    tendAgreementMaster.TEND_AGREEMENT_STATUS,
                    tendAgreementMaster.TEND_AMOUNT_YEAR6,
                    tendAgreementMaster.TEND_INCLUDE_ROAD_AMT
                }).ToArray();

                return result.Select(tendAgreementMaster => new
                {
                    id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {                                                                               
                                    // tendAgreementDetails.IMS_ROAD_NAME,
                                   // tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    tendAgreementMaster.TEND_AGREEMENT_NUMBER.ToString(),
                                    tendAgreementMaster.MAST_CON_COMPANY_NAME==null?"NA":tendAgreementMaster.MAST_CON_COMPANY_NAME.ToString().Trim(),
                                    AgreementTypes[tendAgreementMaster.TEND_AGREEMENT_TYPE].ToString(), 
                                    Convert.ToDateTime(tendAgreementMaster.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),
                                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT.ToString(),      
                                    ((tendAgreementMaster.TEND_AMOUNT_YEAR1==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR1)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR2==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR2)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR3==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR3)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR4==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR4)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR5==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR5) + (PMGSYSession.Current.PMGSYScheme == 2?(tendAgreementMaster.TEND_AMOUNT_YEAR6==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR6):0)
                                    ).ToString(),
                                     //AgreementStatus[tendAgreementMaster.TEND_AGREEMENT_STATUS].ToString(),
                                     (checkAllAgreementsInProgress(tendAgreementMaster.TEND_AGREEMENT_CODE) == true && tendAgreementMaster.TEND_AGREEMENT_STATUS.Trim() != "P") 
                                     ? "<a href='#' title='Click here to change Agreement Status to In Progress' onClick=ChangeTerminatedAgreementMasterStatus('"+ URLEncrypt.EncryptParameters1(new string[] { tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() + "$" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }) + "'); return false;>"+ AgreementStatus[tendAgreementMaster.TEND_AGREEMENT_STATUS].ToString() +"</a>"
                                     : AgreementStatus[tendAgreementMaster.TEND_AGREEMENT_STATUS].ToString(),
                                    // (tendAgreementMaster.TEND_AGREEMENT_STATUS =="W")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }),
                                     (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="N" && tendAgreementMaster.TEND_LOCK_STATUS=="N" && tendAgreementMaster.TEND_AGREEMENT_STATUS!="W") ?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='FinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Finalized'></span></td></tr></table></center>",
                                     
                                      URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }),
                                      ///Changes for RCPLWE
                                      //Below Code is Modified on 27-12-2021 
                                    //((PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47|| PMGSYSession.Current.RoleCode == 56)) ? URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }) : (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementMaster.TEND_LOCK_STATUS=="Y" || tendAgreementMaster.TEND_AGREEMENT_STATUS=="W")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }),
                                    //Below Code is Modified on 27-12-2021 
                                    PMGSYSession.Current.RoleCode == 36 ? PMGSYSession.Current.RoleCode.ToString():((PMGSYSession.Current.RoleCode == 47|| PMGSYSession.Current.RoleCode == 56)) ? URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }) : (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementMaster.TEND_LOCK_STATUS=="Y" || tendAgreementMaster.TEND_AGREEMENT_STATUS=="W")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }),
                                    (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementMaster.TEND_LOCK_STATUS=="Y" || tendAgreementMaster.TEND_AGREEMENT_STATUS=="W")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() })
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

        public bool checkAllAgreementsInProgress(int agreementCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool isAllAgreementsInProgress = false;
            try
            {
                var query1 = from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                             join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                             on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                             join imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                             on tendAgreementDetails.IMS_PR_ROAD_CODE equals imsSanctionedProjectDetails.IMS_PR_ROAD_CODE
                             join proposalWorks in dbContext.IMS_PROPOSAL_WORK
                             on tendAgreementDetails.IMS_WORK_CODE equals proposalWorks.IMS_WORK_CODE into works
                             from proposalWorks in works.DefaultIfEmpty()

                             where
                             tendAgreementDetails.TEND_AGREEMENT_CODE == agreementCode
                             && tendAgreementMaster.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                             && imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme // new change done by Vikram as roads should be populated according to the Scheme
                             // tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                             //&&  tendAgreementDetails.TEND_AGREEMENT_STATUS != "W"
                             select new
                             {
                                 tendAgreementDetails.TEND_AGREEMENT_ID,
                                 tendAgreementDetails.TEND_AGREEMENT_CODE,
                                 tendAgreementDetails.IMS_PR_ROAD_CODE,
                                 tendAgreementDetails.IMS_WORK_CODE,
                                 imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                 tendAgreementDetails.TEND_AGREEMENT_AMOUNT,
                                 tendAgreementDetails.TEND_AMOUNT_YEAR1,
                                 tendAgreementDetails.TEND_AMOUNT_YEAR2,
                                 tendAgreementDetails.TEND_AMOUNT_YEAR3,
                                 tendAgreementDetails.TEND_AMOUNT_YEAR4,
                                 tendAgreementDetails.TEND_AMOUNT_YEAR5,
                                 tendAgreementDetails.TEND_PART_AGREEMENT,
                                 tendAgreementDetails.TEND_START_CHAINAGE,
                                 tendAgreementDetails.TEND_END_CHAINAGE,
                                 tendAgreementDetails.TEND_AGREEMENT_STATUS,
                                 tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                                 tendAgreementMaster.TEND_LOCK_STATUS,
                                 proposalWorks.IMS_WORK_DESC,
                                 tendAgreementMaster.TEND_AGREEMENT_TYPE,
                                 imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                                 imsSanctionedProjectDetails.IMS_BRIDGE_NAME,
                                 tendAgreementDetails.TEND_INCOMPLETE_REASON,
                                 tendAgreementDetails.TEND_VALUE_WORK_DONE,
                                 tendAgreementDetails.TEND_AMOUNT_YEAR6,
                                 imsSanctionedProjectDetails.IMS_YEAR,
                                 imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                                 tendAgreementDetails.TEND_INCLUDE_ROAD_AMT

                             };
                if (query1.Count() > 0)
                {
                    isAllAgreementsInProgress = query1.Where(x => x.TEND_AGREEMENT_STATUS == "W").Any() ? false : true;
                }

                return isAllAgreementsInProgress;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {

            }
        }

        public bool SaveAgreementDetailsDAL(AgreementDetails details_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int IMSPRRoadCode = 0;
                string roadName = string.Empty;
                Decimal? roadLength = 0;
                //Decimal? chainageDifference = 0;
                string agreementType = string.Empty;
                TEND_AGREEMENT_MASTER agreementMaster = null;
                TEND_AGREEMENT_DETAIL agreementDetails = null;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                CommonFunctions commonFunction = new CommonFunctions();

                encryptedParameters = details_agreement.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                //  roadName = decryptedParameters["IMSRoadName"].ToString().Trim();

                string proposalType = decryptedParameters["ProposalType"].ToString();

                if (proposalType.Equals("P"))
                {
                    roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_ROAD_NAME).FirstOrDefault();
                }
                else
                {
                    roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_BRIDGE_NAME).FirstOrDefault();
                }


                encryptedParameters = null;
                encryptedParameters = details_agreement.EncryptedAgreementType_Add.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                agreementType = decryptedParameters["AgreementType"].ToString().Trim();


                int count = 0;
                var agreementList = from agreementDetailsList in dbContext.TEND_AGREEMENT_DETAIL
                                    join agreementMasterList in dbContext.TEND_AGREEMENT_MASTER
                                    on agreementDetailsList.TEND_AGREEMENT_CODE equals agreementMasterList.TEND_AGREEMENT_CODE
                                    where
                                        // agreementDetailsList.TEND_AGREEMENT_STATUS == "P" &&
                                    agreementDetailsList.TEND_AGREEMENT_STATUS != "W" &&
                                    agreementDetailsList.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                    agreementMasterList.TEND_AGREEMENT_TYPE.ToUpper() == agreementType.ToUpper() &&
                                    agreementMasterList.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                                    select new
                                    {
                                        agreementDetailsList.TEND_AGREEMENT_ID,
                                        agreementDetailsList.TEND_AGREEMENT_CODE,
                                        agreementDetailsList.IMS_PR_ROAD_CODE,
                                        agreementDetailsList.IMS_WORK_CODE,
                                        agreementMasterList.TEND_AGREEMENT_TYPE,
                                        agreementMasterList.TEND_AGREEMENT_STATUS
                                    };



                var sanctionProposal = dbContext.IMS_SANCTIONED_PROJECTS.Find(IMSPRRoadCode);
                //decimal sanctionCost = sanctionProposal.IMS_PROPOSAL_TYPE == "P" ? (PMGSYSession.Current.PMGSYScheme == 1 ? (sanctionProposal.IMS_SANCTIONED_PAV_AMT + sanctionProposal.IMS_SANCTIONED_PW_AMT + sanctionProposal.IMS_SANCTIONED_OW_AMT + sanctionProposal.IMS_SANCTIONED_CD_AMT + sanctionProposal.IMS_SANCTIONED_RS_AMT) : (sanctionProposal.IMS_SANCTIONED_PAV_AMT + sanctionProposal.IMS_SANCTIONED_PW_AMT + sanctionProposal.IMS_SANCTIONED_OW_AMT + sanctionProposal.IMS_SANCTIONED_CD_AMT + sanctionProposal.IMS_SANCTIONED_RS_AMT + (sanctionProposal.IMS_SANCTIONED_HS_AMT.HasValue ? sanctionProposal.IMS_SANCTIONED_HS_AMT.Value : 0) + (sanctionProposal.IMS_SANCTIONED_FC_AMT.HasValue ? sanctionProposal.IMS_SANCTIONED_FC_AMT.Value : 0))) : (sanctionProposal.IMS_PROPOSAL_TYPE == "L" ? (PMGSYSession.Current.PMGSYScheme == 1 ? sanctionProposal.IMS_SANCTIONED_BW_AMT + sanctionProposal.IMS_SANCTIONED_BS_AMT : sanctionProposal.IMS_SANCTIONED_BW_AMT + sanctionProposal.IMS_SANCTIONED_BS_AMT + (sanctionProposal.IMS_SANCTIONED_HS_AMT.HasValue ? sanctionProposal.IMS_SANCTIONED_HS_AMT.Value : 0)) : sanctionProposal.IMS_SANCTIONED_PAV_AMT);

                //decimal sanctionCost = PMGSYSession.Current.PMGSYScheme == 1 ?
                //                                      (decimal)((sanctionProposal.IMS_SANCTIONED_PAV_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PAV_AMT) + (sanctionProposal.IMS_SANCTIONED_PW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PW_AMT) + (sanctionProposal.IMS_SANCTIONED_OW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_OW_AMT) + (sanctionProposal.IMS_SANCTIONED_CD_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_CD_AMT) + (sanctionProposal.IMS_SANCTIONED_BW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BW_AMT) + (sanctionProposal.IMS_SANCTIONED_BS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BS_AMT) + (sanctionProposal.IMS_SANCTIONED_RS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_RS_AMT))
                //                                    : (decimal)((sanctionProposal.IMS_SANCTIONED_PAV_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PAV_AMT) + (sanctionProposal.IMS_SANCTIONED_PW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PW_AMT) + (sanctionProposal.IMS_SANCTIONED_OW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_OW_AMT) + (sanctionProposal.IMS_SANCTIONED_CD_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_CD_AMT) + (sanctionProposal.IMS_SANCTIONED_FC_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_FC_AMT) + (sanctionProposal.IMS_SANCTIONED_HS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_HS_AMT) + (sanctionProposal.IMS_SANCTIONED_BW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BW_AMT) + (sanctionProposal.IMS_SANCTIONED_BS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BS_AMT));

                //decimal sanctionCost = PMGSYSession.Current.PMGSYScheme == 1 ?
                //                                      (decimal)((sanctionProposal.IMS_SANCTIONED_PAV_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PAV_AMT) + (sanctionProposal.IMS_SANCTIONED_PW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PW_AMT) + (sanctionProposal.IMS_SANCTIONED_OW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_OW_AMT) + (sanctionProposal.IMS_SANCTIONED_CD_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_CD_AMT) + (sanctionProposal.IMS_SANCTIONED_BW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BW_AMT) + (sanctionProposal.IMS_SANCTIONED_BS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BS_AMT) + (sanctionProposal.IMS_SANCTIONED_RS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_RS_AMT))
                //                                      : PMGSYSession.Current.PMGSYScheme == 4 ?
                //                                      (decimal)((sanctionProposal.IMS_SANCTIONED_PAV_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PAV_AMT) + (sanctionProposal.IMS_SANCTIONED_PW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PW_AMT) + (sanctionProposal.IMS_SANCTIONED_OW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_OW_AMT) + (sanctionProposal.IMS_SANCTIONED_CD_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_CD_AMT) + (sanctionProposal.IMS_SANCTIONED_FC_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_FC_AMT) + (sanctionProposal.IMS_SANCTIONED_HS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_HS_AMT) + (sanctionProposal.IMS_SANCTIONED_BW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BW_AMT) + (sanctionProposal.IMS_SANCTIONED_BS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BS_AMT) + (sanctionProposal.IMS_SANCTIONED_RS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_RS_AMT))
                //                                      : (decimal)((sanctionProposal.IMS_SANCTIONED_PAV_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PAV_AMT) + (sanctionProposal.IMS_SANCTIONED_PW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PW_AMT) + (sanctionProposal.IMS_SANCTIONED_OW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_OW_AMT) + (sanctionProposal.IMS_SANCTIONED_CD_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_CD_AMT) + (sanctionProposal.IMS_SANCTIONED_FC_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_FC_AMT) + (sanctionProposal.IMS_SANCTIONED_HS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_HS_AMT) + (sanctionProposal.IMS_SANCTIONED_BW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BW_AMT) + (sanctionProposal.IMS_SANCTIONED_BS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BS_AMT));

                decimal sanctionCost = (PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ? (decimal)((sanctionProposal.IMS_SANCTIONED_PAV_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PAV_AMT) + (sanctionProposal.IMS_SANCTIONED_PW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PW_AMT) + (sanctionProposal.IMS_SANCTIONED_OW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_OW_AMT) + (sanctionProposal.IMS_SANCTIONED_CD_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_CD_AMT) + (sanctionProposal.IMS_SANCTIONED_BW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BW_AMT) + (sanctionProposal.IMS_SANCTIONED_BS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BS_AMT) + (sanctionProposal.IMS_SANCTIONED_RS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_RS_AMT))
                    : (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 4) ? (decimal)((sanctionProposal.IMS_SANCTIONED_PAV_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PAV_AMT) + (sanctionProposal.IMS_SANCTIONED_PW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_PW_AMT) + (sanctionProposal.IMS_SANCTIONED_OW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_OW_AMT) + (sanctionProposal.IMS_SANCTIONED_CD_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_CD_AMT) + (sanctionProposal.IMS_SANCTIONED_BW_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BW_AMT) + (sanctionProposal.IMS_SANCTIONED_BS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_BS_AMT) + (sanctionProposal.IMS_SANCTIONED_FC_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_FC_AMT) + (sanctionProposal.IMS_SANCTIONED_HS_AMT == null ? 0 : sanctionProposal.IMS_SANCTIONED_HS_AMT) + (sanctionProposal.IMS_PUCCA_SIDE_DRAINS == null ? 0 : sanctionProposal.IMS_PUCCA_SIDE_DRAINS))
                    : 0;

                if (dbContext.IMS_PROPOSAL_COST_ADD.Any(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                {
                    decimal? costToAdd = dbContext.IMS_PROPOSAL_COST_ADD.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(m => m.IMS_TRANSACTION_CODE).Select(m => m.IMS_STATE_AMOUNT).FirstOrDefault() + dbContext.IMS_PROPOSAL_COST_ADD.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(m => m.IMS_TRANSACTION_CODE).Select(m => m.IMS_MORD_AMOUNT).FirstOrDefault();
                    sanctionCost = sanctionCost + (costToAdd.HasValue ? costToAdd.Value : 0);
                }

                //if (details_agreement.TEND_AGREEMENT_AMOUNT > (sanctionCost + (sanctionCost * (decimal)0.2)))
                //{
                //    message = "Awarded cost more than 20% of GOI cost.Additional Cost Entry can be made only by SRRDA.";
                //    return false;
                //}
                if (stateCode == 27)
                {
                    if (details_agreement.TEND_AGREEMENT_AMOUNT > (sanctionCost + (sanctionCost * (decimal)0.5)))
                    {
                        message = "Awarded cost more than 50% of GOI cost.Additional Cost Entry can be made only by SRRDA.";
                        return false;
                    }
                }
                else
                {
                    if (details_agreement.TEND_AGREEMENT_AMOUNT > (sanctionCost + (sanctionCost * (decimal)0.2)))
                    {
                        message = "Awarded cost more than 20% of GOI cost.Additional Cost Entry can be made only by SRRDA.";
                        return false;
                    }
                }


                //----------------------------------------------------- added on 15-07-2022 
                if (dbContext.TEND_AGREEMENT_DETAIL.Any(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                {
                    var getAllCost = (from ims in dbContext.IMS_SANCTIONED_PROJECTS
                                      join tenddetail in dbContext.TEND_AGREEMENT_DETAIL
                                      on ims.IMS_PR_ROAD_CODE equals tenddetail.IMS_PR_ROAD_CODE
                                      where tenddetail.IMS_PR_ROAD_CODE == IMSPRRoadCode

                                      select new
                                      {
                                          ims.IMS_SANCTIONED_PAV_AMT,
                                          ims.IMS_SANCTIONED_PW_AMT,
                                          ims.IMS_SANCTIONED_OW_AMT,
                                          ims.IMS_SANCTIONED_CD_AMT,
                                          ims.IMS_SANCTIONED_FC_AMT,
                                          ims.IMS_SANCTIONED_HS_AMT,
                                          ims.IMS_SANCTIONED_BW_AMT,
                                          ims.IMS_SANCTIONED_BS_AMT,
                                          ims.IMS_SANCTIONED_RS_AMT,
                                          ims.IMS_PUCCA_SIDE_DRAINS
                                      }).FirstOrDefault();


                    var getSchemeSanctionedCost = (PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ?
                        ((getAllCost.IMS_SANCTIONED_PAV_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_PAV_AMT) + (getAllCost.IMS_SANCTIONED_PW_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_PW_AMT) + (getAllCost.IMS_SANCTIONED_OW_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_OW_AMT) + (getAllCost.IMS_SANCTIONED_CD_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_CD_AMT) + (getAllCost.IMS_SANCTIONED_BW_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_BW_AMT) + (getAllCost.IMS_SANCTIONED_BS_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_BS_AMT) + (getAllCost.IMS_SANCTIONED_RS_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_RS_AMT))
                    : ((getAllCost.IMS_SANCTIONED_PAV_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_PAV_AMT) + (getAllCost.IMS_SANCTIONED_PW_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_PW_AMT) + (getAllCost.IMS_SANCTIONED_OW_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_OW_AMT) + (getAllCost.IMS_SANCTIONED_CD_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_CD_AMT) + (getAllCost.IMS_SANCTIONED_FC_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_FC_AMT) + (getAllCost.IMS_SANCTIONED_HS_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_HS_AMT) + (getAllCost.IMS_SANCTIONED_BW_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_BW_AMT) + (getAllCost.IMS_SANCTIONED_BS_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_BS_AMT) + (getAllCost.IMS_PUCCA_SIDE_DRAINS == null ? 0 : getAllCost.IMS_PUCCA_SIDE_DRAINS));


                    if (details_agreement.TEND_TENDER_AMOUNT > (getSchemeSanctionedCost * 10) || details_agreement.GST_AMT_MAINTAINANCE_AGREEMENT > (getSchemeSanctionedCost * 10) || details_agreement.TEND_AGREEMENT_AMOUNT > (getSchemeSanctionedCost * 10) || details_agreement.GST_AMT_MAINTAINANCE_AGREEMENT_DLP > (getSchemeSanctionedCost * 10) || details_agreement.APS_COLLECTED_AMOUNT > (getSchemeSanctionedCost * 10)
                        || details_agreement.TEND_AMOUNT_YEAR1 > (getSchemeSanctionedCost * 10) || details_agreement.TEND_AMOUNT_YEAR2 > (getSchemeSanctionedCost * 10) || details_agreement.TEND_AMOUNT_YEAR3 > (getSchemeSanctionedCost * 10) || details_agreement.TEND_AMOUNT_YEAR4 > (getSchemeSanctionedCost * 10) || details_agreement.TEND_AMOUNT_YEAR5 > (getSchemeSanctionedCost * 10)
                        || details_agreement.TEND_AMOUNT_YEAR6 > (getSchemeSanctionedCost * 10))
                    {
                        message = "Enter Amount in Lakhs ( Tender Amount / Agreement Amount / Year wise maintenance  Amount ) ";
                        return false;
                    }
                }

                //----------------------------------------------------- Changes END --------------------------------------------


                //Change is made as per suggestion by Srinivasa Sir. Details can be added only if Agreement Status is completed. 20 July 2020

                if (agreementList.Count() > 0)
                {
                    foreach (var agreementDetail in agreementList)
                    {
                        if (agreementDetail.TEND_AGREEMENT_STATUS != "C")
                        {
                            //message = "Agreement Status against road '" + roadName + "' is not completed. Hence these further Agreement Details can not be added.";
                            //return false;

                        }
                    }
                }

                if (false)  //proposalType != "B" added by pradip patil as per suggestion by srinivas sir on  15/12/2017 [if bulding then escape already agreement check nad allow new agreement]
                {

                    if (agreementList.Count() > 0)
                    {
                        foreach (var agreementDetail in agreementList)
                        {
                            ///Changed by SAMMED A. PATIL on 20JUNE2017 to allow add agreement details after agreement status is completed
                            if (agreementDetail.TEND_AGREEMENT_STATUS != "C" && agreementDetail.TEND_AGREEMENT_STATUS != "W")
                            {
                                if (agreementDetail.IMS_WORK_CODE == null)
                                {
                                    message = "Agreement Details against road '" + roadName + "' is already exist.";
                                    return false;
                                }
                                else
                                {
                                    count = dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(ps => ps.IMS_TOTAL_SPLIT).FirstOrDefault();

                                    if (agreementDetail.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE)
                                    {
                                        message = "Agreement Details against road '" + roadName + "' and selected work is already exist.";
                                        return false;
                                    }

                                    if (count == agreementList.Count())
                                    {
                                        message = "Agreement Details against road '" + roadName + "' is already exist.";
                                        return false;
                                    }

                                }
                            }
                        }
                    }
                }

                if (agreementType.Equals("C"))
                {
                    if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_PART_AGREEMENT == "Y" && ad.TEND_AGREEMENT_STATUS != "W"))
                    {
                        roadLength = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_AGREEMENT_STATUS != "W").Sum(td => (Decimal?)(td.TEND_END_CHAINAGE - td.TEND_START_CHAINAGE));
                        roadLength = (roadLength == null ? 0 : roadLength) + ((details_agreement.TEND_END_CHAINAGE == null ? 0 : details_agreement.TEND_END_CHAINAGE) - (details_agreement.TEND_START_CHAINAGE == null ? 0 : details_agreement.TEND_START_CHAINAGE));
                        if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                        {
                            message = "Chainage exceeds its road length.";
                            return false;
                        }
                    }
                }

                if (agreementType.Equals("C"))
                {
                    if (details_agreement.IMS_WORK_CODE > 0)
                    {

                        if (details_agreement.TEND_START_CHAINAGE != null && details_agreement.TEND_END_CHAINAGE != null)
                        {
                            if ((details_agreement.TEND_END_CHAINAGE == null ? 0 : details_agreement.TEND_END_CHAINAGE) > (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(pw => (Decimal?)pw.IMS_END_CHAINAGE).FirstOrDefault()) || (details_agreement.TEND_START_CHAINAGE == null ? 0 : details_agreement.TEND_START_CHAINAGE) < (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(pw => (Decimal?)pw.IMS_START_CHAINAGE).FirstOrDefault()))
                            {
                                message = "Chainage must be between work chainage.";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        // if (details_agreement.IsPartAgreement)
                        //  {
                        roadLength += ((details_agreement.TEND_END_CHAINAGE == null ? 0 : details_agreement.TEND_END_CHAINAGE) - (details_agreement.TEND_START_CHAINAGE == null ? 0 : details_agreement.TEND_START_CHAINAGE));

                        if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                        {
                            message = "Chainage exceeds its road length.";
                            return false;
                        }

                        //}

                    }
                }


                if (dbContext.TEND_AGREEMENT_MASTER.Any(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.TEND_AGREEMENT_NUMBER.ToUpper() == details_agreement.TEND_AGREEMENT_NUMBER.ToUpper() && am.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme))
                {
                    message = "Agreement Number is already exist.";
                    return false;
                }

                //Check agreement number present in Maintenance Agreement

                var IMSContractList = (from IMSContracts in dbContext.MANE_IMS_CONTRACT
                                       join IMSSanctioned in dbContext.IMS_SANCTIONED_PROJECTS
                                       on IMSContracts.IMS_PR_ROAD_CODE equals IMSSanctioned.IMS_PR_ROAD_CODE
                                       where
                                       IMSSanctioned.MAST_STATE_CODE == stateCode &&
                                       IMSSanctioned.MAST_DISTRICT_CODE == districtCode &&
                                       IMSContracts.MANE_AGREEMENT_NUMBER.ToUpper() == details_agreement.TEND_AGREEMENT_NUMBER.ToUpper()
                                       select new
                                       {

                                           IMSContracts.MANE_AGREEMENT_NUMBER,
                                           IMSContracts.MAST_CON_ID,
                                           IMSContracts.IMS_PR_ROAD_CODE,
                                           IMSSanctioned.MAST_STATE_CODE,
                                           IMSSanctioned.MAST_DISTRICT_CODE

                                       });


                if (IMSContractList.Count() > 0)
                {
                    message = "Agreement Number is already exist for maintenance agreement.";
                    return false;
                }

                //  Int32 recordCount = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.MAST_CON_ID == details_agreement.MAST_CON_ID && am.TEND_AGREEMENT_NUMBER.ToUpper() == details_agreement.TEND_AGREEMENT_NUMBER.ToUpper()).Count();
                //else if (dbContext.TEND_AGREEMENT_MASTER.Any(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.MAST_CON_ID == details_agreement.MAST_CON_ID && am.TEND_AGREEMENT_NUMBER.ToUpper() == details_agreement.TEND_AGREEMENT_NUMBER.ToUpper()))
                //{
                //    message = "Agreement Number for selected contractor is already exist.";
                //    return false;
                //}


                using (var scope = new TransactionScope())
                {
                    agreementMaster = new TEND_AGREEMENT_MASTER();


                    #region Added on 06 May 2020
                    DateTime? SanctionedDate =dbContext.IMS_SANCTIONED_PROJECTS.Where(m=>m.IMS_PR_ROAD_CODE==IMSPRRoadCode).Select(m=>m.IMS_SANCTIONED_DATE).FirstOrDefault();

                    if (SanctionedDate!=null)
                    {
                        agreementMaster.TEND_DATE_OF_AGREEMENT = commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_AGREEMENT);
                        if (agreementMaster.TEND_DATE_OF_AGREEMENT < SanctionedDate)
                        {
                            message = "Agreement Date must be greater than or equal to Sanctioned Date.";
                            return false;
                        }
                    }
                    #endregion


                    agreementMaster.TEND_AGREEMENT_CODE = (Int32)GetMaxCode(AgreementModules.AgreementMaster);
                    agreementMaster.MAST_STATE_CODE = stateCode;
                    agreementMaster.MAST_DISTRICT_CODE = districtCode;
                    agreementMaster.MAST_CON_ID = details_agreement.MAST_CON_ID == 0 ? null : (Int32?)details_agreement.MAST_CON_ID;
                    agreementMaster.TEND_TENDER_AMOUNT = details_agreement.TEND_TENDER_AMOUNT == null ? null : details_agreement.TEND_TENDER_AMOUNT;
                    agreementMaster.TEND_AGREEMENT_NUMBER = details_agreement.TEND_AGREEMENT_NUMBER;
                    agreementMaster.TEND_DATE_OF_AGREEMENT = commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_AGREEMENT);
                    agreementMaster.TEND_AGREEMENT_START_DATE = commonFunction.GetStringToDateTime(details_agreement.TEND_AGREEMENT_START_DATE);
                    agreementMaster.TEND_AGREEMENT_END_DATE = commonFunction.GetStringToDateTime(details_agreement.TEND_AGREEMENT_END_DATE);
                    agreementMaster.TEND_AGREEMENT_AMOUNT = (Decimal)details_agreement.TEND_AGREEMENT_AMOUNT;
                    agreementMaster.TEND_IS_AGREEMENT_FINALIZED = "N";
                    agreementMaster.TEND_AGREEMENT_TYPE = agreementType;//details_agreement.AgreementType == true ? "C" : "O";
                    agreementMaster.TEND_DATE_OF_AWARD_WORK = details_agreement.TEND_DATE_OF_AWARD_WORK == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_AWARD_WORK);
                    agreementMaster.TEND_DATE_OF_WORK_ORDER = details_agreement.TEND_DATE_OF_WORK_ORDER == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_WORK_ORDER);
                    agreementMaster.TEND_DATE_OF_COMMENCEMENT = details_agreement.TEND_DATE_OF_COMMENCEMENT == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_COMMENCEMENT);
                    agreementMaster.TEND_DATE_OF_COMPLETION = details_agreement.TEND_DATE_OF_COMPLETION == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_COMPLETION);

                    agreementMaster.TEND_AMOUNT_YEAR1 = details_agreement.TEND_AMOUNT_YEAR1 == null ? null : details_agreement.TEND_AMOUNT_YEAR1;
                    agreementMaster.TEND_AMOUNT_YEAR2 = details_agreement.TEND_AMOUNT_YEAR2 == null ? null : details_agreement.TEND_AMOUNT_YEAR2;
                    agreementMaster.TEND_AMOUNT_YEAR3 = details_agreement.TEND_AMOUNT_YEAR3 == null ? null : details_agreement.TEND_AMOUNT_YEAR3;
                    agreementMaster.TEND_AMOUNT_YEAR4 = details_agreement.TEND_AMOUNT_YEAR4 == null ? null : details_agreement.TEND_AMOUNT_YEAR4;
                    agreementMaster.TEND_AMOUNT_YEAR5 = details_agreement.TEND_AMOUNT_YEAR5 == null ? null : details_agreement.TEND_AMOUNT_YEAR5;
                    agreementMaster.TEND_AMOUNT_YEAR6 = details_agreement.TEND_AMOUNT_YEAR6 == null ? null : details_agreement.TEND_AMOUNT_YEAR6;

                    // Added By Rohit Borse on 01-07-2022
                    agreementMaster.GST_AMT_MAINT_AGREEMENT = details_agreement.GST_AMT_MAINTAINANCE_AGREEMENT;
                    agreementMaster.APS_COLLECTED = details_agreement.APS_COLLECTED;
                    agreementMaster.APS_COLLECTED_AMOUNT = details_agreement.APS_COLLECTED_AMOUNT;

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementMaster.TEND_HIGHER_SPEC_AMT = details_agreement.TEND_HIGHER_SPEC_AMT == null ? null : details_agreement.TEND_HIGHER_SPEC_AMT;
                        agreementMaster.TEND_STATE_SHARE = details_agreement.TEND_HIGHER_SPEC_AMT == null ? null : details_agreement.TEND_STATE_SHARE;
                        agreementMaster.TEND_MORD_SHARE = details_agreement.TEND_HIGHER_SPEC_AMT == null ? null : details_agreement.TEND_MORD_SHARE;
                    }

                    agreementMaster.TEND_AGREEMENT_REMARKS = details_agreement.TEND_AGREEMENT_REMARKS == null ? null : details_agreement.TEND_AGREEMENT_REMARKS.Trim();

                    agreementMaster.TEND_LOCK_STATUS = "N";
                    agreementMaster.TEND_AGREEMENT_STATUS = "P";
                    agreementMaster.USERID = PMGSYSession.Current.UserId;
                    agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementMaster.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme; //new change done by Vikram on 10 Feb 2014
                    dbContext.TEND_AGREEMENT_MASTER.Add(agreementMaster);
                    // dbContext.SaveChanges();

                    agreementDetails = new TEND_AGREEMENT_DETAIL();
                    agreementDetails.TEND_AGREEMENT_ID = (Int32)GetMaxCode(AgreementModules.AgreementDetails);
                    agreementDetails.TEND_AGREEMENT_CODE = agreementMaster.TEND_AGREEMENT_CODE;
                    agreementDetails.IMS_PR_ROAD_CODE = IMSPRRoadCode;
                    agreementDetails.TEND_AGREEMENT_AMOUNT = (Decimal)details_agreement.TEND_AGREEMENT_AMOUNT;

                    agreementDetails.TEND_AMOUNT_YEAR1 = details_agreement.TEND_AMOUNT_YEAR1 == null ? null : details_agreement.TEND_AMOUNT_YEAR1;
                    agreementDetails.TEND_AMOUNT_YEAR2 = details_agreement.TEND_AMOUNT_YEAR2 == null ? null : details_agreement.TEND_AMOUNT_YEAR2;
                    agreementDetails.TEND_AMOUNT_YEAR3 = details_agreement.TEND_AMOUNT_YEAR3 == null ? null : details_agreement.TEND_AMOUNT_YEAR3;
                    agreementDetails.TEND_AMOUNT_YEAR4 = details_agreement.TEND_AMOUNT_YEAR4 == null ? null : details_agreement.TEND_AMOUNT_YEAR4;
                    agreementDetails.TEND_AMOUNT_YEAR5 = details_agreement.TEND_AMOUNT_YEAR5 == null ? null : details_agreement.TEND_AMOUNT_YEAR5;
                    agreementDetails.TEND_AMOUNT_YEAR6 = details_agreement.TEND_AMOUNT_YEAR6 == null ? null : details_agreement.TEND_AMOUNT_YEAR6;
                    agreementDetails.TEND_START_CHAINAGE = details_agreement.TEND_START_CHAINAGE == null ? null : (details_agreement.TEND_START_CHAINAGE);
                    agreementDetails.TEND_END_CHAINAGE = details_agreement.TEND_END_CHAINAGE == null ? null : (details_agreement.TEND_END_CHAINAGE);

                    // Added By Rohit Borse on 01-07-2022
                    agreementDetails.GST_AMT_MAINT_AGREEMENT_DLP = details_agreement.GST_AMT_MAINTAINANCE_AGREEMENT_DLP;

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementDetails.TEND_HIGHER_SPEC_AMT = details_agreement.TEND_HIGHER_SPEC_AMT == null ? null : details_agreement.TEND_HIGHER_SPEC_AMT;
                        agreementDetails.TEND_STATE_SHARE = details_agreement.TEND_HIGHER_SPEC_AMT == null ? null : details_agreement.TEND_STATE_SHARE;
                        agreementDetails.TEND_MORD_SHARE = details_agreement.TEND_HIGHER_SPEC_AMT == null ? null : details_agreement.TEND_MORD_SHARE;
                    }
                    // agreementDetails.TEND_PART_AGREEMENT = "N";

                    //commented for requirement has changed road part taken from IMS_Proposal_Work
                    //if (details_agreement.IsPartAgreement)
                    //{
                    //    agreementDetails.TEND_PART_AGREEMENT = "Y";
                    //    agreementDetails.TEND_START_CHAINAGE = (Decimal)details_agreement.TEND_START_CHAINAGE;
                    //    agreementDetails.TEND_END_CHAINAGE = (Decimal)details_agreement.TEND_END_CHAINAGE;
                    //}
                    //else
                    //{
                    //    agreementDetails.TEND_PART_AGREEMENT = "N";
                    //}
                    if (details_agreement.IMS_WORK_CODE > 0)
                    {
                        agreementDetails.TEND_PART_AGREEMENT = "Y";
                        agreementDetails.IMS_WORK_CODE = details_agreement.IMS_WORK_CODE;

                        if (dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE).Any())
                        {
                            agreementDetails.TEND_FIRST_AGREEMENT = "N";
                        }
                        else
                        {
                            agreementDetails.TEND_FIRST_AGREEMENT = "Y";
                        }
                    }
                    else
                    {
                        agreementDetails.TEND_PART_AGREEMENT = "N";

                        if (dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode).Any())
                        {
                            agreementDetails.TEND_FIRST_AGREEMENT = "N";
                        }
                        else
                        {
                            agreementDetails.TEND_FIRST_AGREEMENT = "Y";
                        }

                    }
                    agreementDetails.TEND_AGREEMENT_STATUS = "P";

                    //added by Koustubh Nakate 01/11/2013 for differntiate first agreement

                    agreementDetails.TEND_INCLUDE_ROAD_AMT = "Y";

                    agreementDetails.USERID = PMGSYSession.Current.UserId;
                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.TEND_AGREEMENT_DETAIL.Add(agreementDetails);
                    dbContext.SaveChanges();

                    //if (dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode).Count() == 1)
                    //{
                    //    dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).ToList().ForEach(IMS => IMS.IMS_ISCOMPLETED = "G"); //&& IMS.IMS_ISCOMPLETED == "W"
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



        public List<TEND_AGREEMENT_MASTER> GetAgreementNumbers(int contractorID, string agreementType, bool isSearch)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                List<TEND_AGREEMENT_MASTER> agreementNumbersList = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.MAST_CON_ID == contractorID && am.TEND_AGREEMENT_TYPE == agreementType && am.TEND_AGREEMENT_STATUS != "W" && am.TEND_IS_AGREEMENT_FINALIZED == "N" && am.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(am => am.TEND_AGREEMENT_NUMBER).ToList<TEND_AGREEMENT_MASTER>();

                if (isSearch)
                {
                    agreementNumbersList.Insert(0, new TEND_AGREEMENT_MASTER() { TEND_AGREEMENT_CODE = 0, TEND_AGREEMENT_NUMBER = "All Agreements" });
                }
                else
                {
                    agreementNumbersList.Insert(0, new TEND_AGREEMENT_MASTER() { TEND_AGREEMENT_CODE = 0, TEND_AGREEMENT_NUMBER = "Select Agreement" });
                }

                return agreementNumbersList;

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


        public ExistingAgreementDetails GetExistingAgreementDetailsDAL(int contractorID, int agreementCode)
        {
            ExistingAgreementDetails existingAgreementDetails = new ExistingAgreementDetails();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                TEND_AGREEMENT_MASTER master_agreement = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.MAST_CON_ID == contractorID && am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                // Added By Rohit Borse on 01-07-2022
                TEND_AGREEMENT_DETAIL detail_agreement = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == master_agreement.TEND_AGREEMENT_CODE).FirstOrDefault();

                existingAgreementDetails.EncryptedTendAgreementCode_Existing = URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + master_agreement.TEND_AGREEMENT_CODE.ToString() });
                existingAgreementDetails.TEND_DATE_OF_AGREEMENT = master_agreement.TEND_DATE_OF_AGREEMENT == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_AGREEMENT_START_DATE = master_agreement.TEND_AGREEMENT_START_DATE == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_AGREEMENT_START_DATE).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_AGREEMENT_END_DATE = master_agreement.TEND_AGREEMENT_END_DATE == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_AGREEMENT_END_DATE).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_TENDER_AMOUNT = master_agreement.TEND_TENDER_AMOUNT == null ? 0 : master_agreement.TEND_TENDER_AMOUNT;
                existingAgreementDetails.TEND_AGREEMENT_AMOUNT_Existing = master_agreement.TEND_AGREEMENT_AMOUNT;
                existingAgreementDetails.TEND_DATE_OF_AWARD_WORK = master_agreement.TEND_DATE_OF_AWARD_WORK == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_AWARD_WORK).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_DATE_OF_WORK_ORDER = master_agreement.TEND_DATE_OF_WORK_ORDER == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_WORK_ORDER).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_DATE_OF_COMMENCEMENT = master_agreement.TEND_DATE_OF_COMMENCEMENT == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_COMMENCEMENT).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_DATE_OF_COMPLETION = master_agreement.TEND_DATE_OF_COMPLETION == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_COMPLETION).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_AMOUNT_YEAR1_Existing = master_agreement.TEND_AMOUNT_YEAR1 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR1;
                existingAgreementDetails.TEND_AMOUNT_YEAR2_Existing = master_agreement.TEND_AMOUNT_YEAR2 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR2;
                existingAgreementDetails.TEND_AMOUNT_YEAR3_Existing = master_agreement.TEND_AMOUNT_YEAR3 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR3;
                existingAgreementDetails.TEND_AMOUNT_YEAR4_Existing = master_agreement.TEND_AMOUNT_YEAR4 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR4;
                existingAgreementDetails.TEND_AMOUNT_YEAR5_Existing = master_agreement.TEND_AMOUNT_YEAR5 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR5;

                // Added By Rohit Borse on 01-07-2022
                existingAgreementDetails.GST_AMT_MAINTAINANCE_AGREEMENT = master_agreement.GST_AMT_MAINT_AGREEMENT;
                existingAgreementDetails.APS_COLLECTED = master_agreement.APS_COLLECTED == null ? null : (master_agreement.APS_COLLECTED.Equals("Y") ? "Yes" : "No");
                existingAgreementDetails.APS_COLLECTED_AMOUNT = master_agreement.APS_COLLECTED_AMOUNT;
                existingAgreementDetails.GST_AMT_MAINTAINANCE_AGREEMENT_DLP_EXISTING = detail_agreement.GST_AMT_MAINT_AGREEMENT_DLP;

                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    existingAgreementDetails.TEND_AMOUNT_YEAR6_Existing = master_agreement.TEND_AMOUNT_YEAR6 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR6;
                    existingAgreementDetails.TEND_MORD_SHARE_Existing = master_agreement.TEND_MORD_SHARE == null ? 0 : master_agreement.TEND_MORD_SHARE;
                    existingAgreementDetails.TEND_STATE_SHARE_Existing = master_agreement.TEND_STATE_SHARE == null ? 0 : master_agreement.TEND_STATE_SHARE;
                    existingAgreementDetails.TEND_HIGHER_SPEC_AMT_Existing = master_agreement.TEND_HIGHER_SPEC_AMT == null ? 0 : master_agreement.TEND_HIGHER_SPEC_AMT;
                }


                return existingAgreementDetails;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return existingAgreementDetails;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public bool SaveExistingAgreementDetailsDAL(ExistingAgreementDetails details_agreement_existing, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int IMSPRRoadCode = 0;
                int agreementCode = 0;
                string roadName = string.Empty;
                Decimal? roadLength = 0;
                TEND_AGREEMENT_DETAIL agreementDetails = null;
                TEND_AGREEMENT_MASTER agreementMaster = null;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                CommonFunctions commonFunction = new CommonFunctions();

                encryptedParameters = details_agreement_existing.EncryptedTendAgreementCode_Existing.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString());

                encryptedParameters = details_agreement_existing.EncryptedIMSPRRoadCode_Existing.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                // roadName = decryptedParameters["IMSRoadName"].ToString().Trim();

                string proposalType = decryptedParameters["ProposalType"].ToString();

                if (proposalType.Equals("P"))
                {
                    roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_ROAD_NAME).FirstOrDefault();
                }
                else
                {
                    roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_BRIDGE_NAME).FirstOrDefault();
                }


                /* if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRCodeCode && ad.TEND_PART_AGREEMENT == "N" && ad.TEND_AGREEMENT_STATUS != "W"))
                 {
                     message = "Agreement Details against road '" + roadName + "' is already exist.";
                     return false;
                 }
                 else if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRCodeCode && ad.TEND_PART_AGREEMENT == "Y"))
                 {
                     roadLength = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRCodeCode && ad.TEND_AGREEMENT_STATUS != "W").Sum(td => (Decimal?)(td.TEND_END_CHAINAGE - td.TEND_START_CHAINAGE));

                     if (roadLength != null && details_agreement_existing.IsPartAgreement_Existing)
                     {
                         roadLength += (details_agreement_existing.TEND_END_CHAINAGE_Existing - details_agreement_existing.TEND_START_CHAINAGE_Existing);

                         if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRCodeCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                         {
                             message = "Chainage exceeds its road length.";
                             return false;
                         }
                     }
                     else if (roadLength != null && !details_agreement_existing.IsPartAgreement_Existing)
                     {
                         message = "Partly agreement details against road '" + roadName + "' is already exist.";
                         return false;
                     }
                 }
                 else
                 {

                     if (details_agreement_existing.IsPartAgreement_Existing)
                     {

                         roadLength += (details_agreement_existing.TEND_END_CHAINAGE_Existing - details_agreement_existing.TEND_START_CHAINAGE_Existing);

                         if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRCodeCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                         {
                             message = "Chainage exceeds its road length.";
                             return false;
                         }

                     }

                 }*/

                string[] agreementStatus = { "C", "W" };
                int count = 0;
                string agreementType = "C"; //no existing functionality for Other Road
                var agreementList = from agreementDetailsList in dbContext.TEND_AGREEMENT_DETAIL
                                    join agreementMasterList in dbContext.TEND_AGREEMENT_MASTER
                                    on agreementDetailsList.TEND_AGREEMENT_CODE equals agreementMasterList.TEND_AGREEMENT_CODE
                                    where
                                        //agreementDetailsList.TEND_AGREEMENT_STATUS == "P" &&
                                        //agreementDetailsList.TEND_AGREEMENT_STATUS != "W" &&
                                    (!agreementStatus.Contains(agreementDetailsList.TEND_AGREEMENT_STATUS)) &&
                                    agreementDetailsList.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                    agreementMasterList.TEND_AGREEMENT_TYPE.ToUpper() == agreementType.ToUpper()
                                    select new
                                    {
                                        agreementDetailsList.TEND_AGREEMENT_ID,
                                        agreementDetailsList.TEND_AGREEMENT_CODE,
                                        agreementDetailsList.IMS_PR_ROAD_CODE,
                                        agreementDetailsList.IMS_WORK_CODE,
                                        agreementMasterList.TEND_AGREEMENT_TYPE

                                    };

                agreementList = agreementList.GroupBy(al => al.TEND_AGREEMENT_CODE).Select(al => al.FirstOrDefault());


                if (agreementList.Count() > 0)
                {
                    foreach (var agreementDetail in agreementList)
                    {

                        if (agreementDetail.IMS_WORK_CODE == null)
                        {
                            message = "Agreement Details against road '" + roadName + "' is already exist.";
                            return false;
                        }
                        else
                        {
                            count = dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(ps => ps.IMS_TOTAL_SPLIT).FirstOrDefault();

                            if (agreementDetail.IMS_WORK_CODE == details_agreement_existing.IMS_WORK_CODE)
                            {
                                message = "Agreement Details against road '" + roadName + "' and selected work is already exist.";
                                return false;
                            }

                            if (count == agreementList.Count())
                            {
                                message = "Agreement Details against road '" + roadName + "' is already exist.";
                                return false;
                            }
                        }

                    }
                }
                //if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_PART_AGREEMENT == "N" && ad.TEND_AGREEMENT_STATUS != "W"))
                //{
                //    message = "Agreement Details against road '" + roadName + "' is already exist.";
                //    return false;
                //}
                else if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_PART_AGREEMENT == "Y" && ad.TEND_AGREEMENT_STATUS != "W"))
                {

                    //if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_WORK_CODE == details_agreement_existing.IMS_WORK_CODE))
                    //{
                    //    message = "Agreement Details against road '" + roadName + "' and selected work is already exist.";
                    //    return false;
                    //}
                    //else if (details_agreement_existing.IMS_WORK_CODE == 0)
                    //{
                    //    message = "Partly Agreement Details against road '" + roadName + "' is already exist.";
                    //    return false;
                    //}
                    if (details_agreement_existing.IMS_WORK_CODE > 0)
                    {
                        if (details_agreement_existing.TEND_START_CHAINAGE_Existing != null && details_agreement_existing.TEND_END_CHAINAGE_Existing != null)
                        {
                            if ((details_agreement_existing.TEND_END_CHAINAGE_Existing == null ? 0 : details_agreement_existing.TEND_END_CHAINAGE_Existing) > (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_WORK_CODE == details_agreement_existing.IMS_WORK_CODE && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(pw => (Decimal?)pw.IMS_END_CHAINAGE).FirstOrDefault()) || (details_agreement_existing.TEND_START_CHAINAGE_Existing == null ? 0 : details_agreement_existing.TEND_START_CHAINAGE_Existing) < (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_WORK_CODE == details_agreement_existing.IMS_WORK_CODE && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(pw => (Decimal?)pw.IMS_START_CHAINAGE).FirstOrDefault()))
                            {
                                message = "Chainage exceeds its road length.";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        roadLength = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_AGREEMENT_STATUS != "W").Sum(td => (Decimal?)(td.TEND_END_CHAINAGE - td.TEND_START_CHAINAGE));
                        roadLength = (roadLength == null ? 0 : roadLength) + ((details_agreement_existing.TEND_END_CHAINAGE_Existing == null ? 0 : details_agreement_existing.TEND_END_CHAINAGE_Existing) - (details_agreement_existing.TEND_START_CHAINAGE_Existing == null ? 0 : details_agreement_existing.TEND_START_CHAINAGE_Existing));
                        if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                        {
                            message = "Chainage exceeds its road length.";
                            return false;
                        }
                    }

                }
                else
                {
                    // if (details_agreement.IsPartAgreement)
                    //  {
                    roadLength += ((details_agreement_existing.TEND_END_CHAINAGE_Existing == null ? 0 : details_agreement_existing.TEND_END_CHAINAGE_Existing) - (details_agreement_existing.TEND_START_CHAINAGE_Existing == null ? 0 : details_agreement_existing.TEND_START_CHAINAGE_Existing));

                    if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                    {
                        message = "Chainage exceeds its road length.";
                        return false;
                    }

                    //}

                }


                //Int32 recordCount = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.TEND_AGREEMENT_NUMBER.ToUpper() == details_agreement.TEND_AGREEMENT_NUMBER.ToUpper()).Count();
                //if (recordCount > 0)
                //{
                //    message = "Agreement Number under selected state and district is already exist.";
                //    return false;
                //}


                using (var scope = new TransactionScope())
                {

                    agreementDetails = new TEND_AGREEMENT_DETAIL();
                    agreementDetails.TEND_AGREEMENT_ID = (Int32)GetMaxCode(AgreementModules.AgreementDetails);
                    agreementDetails.TEND_AGREEMENT_CODE = agreementCode;
                    agreementDetails.IMS_PR_ROAD_CODE = IMSPRRoadCode;

                    agreementDetails.TEND_AGREEMENT_AMOUNT = (Decimal)details_agreement_existing.TEND_AGREEMENT_AMOUNT_NEW;
                    agreementDetails.TEND_AMOUNT_YEAR1 = details_agreement_existing.TEND_AMOUNT_YEAR1 == null ? null : details_agreement_existing.TEND_AMOUNT_YEAR1;
                    agreementDetails.TEND_AMOUNT_YEAR2 = details_agreement_existing.TEND_AMOUNT_YEAR2 == null ? null : details_agreement_existing.TEND_AMOUNT_YEAR2;
                    agreementDetails.TEND_AMOUNT_YEAR3 = details_agreement_existing.TEND_AMOUNT_YEAR3 == null ? null : details_agreement_existing.TEND_AMOUNT_YEAR3;
                    agreementDetails.TEND_AMOUNT_YEAR4 = details_agreement_existing.TEND_AMOUNT_YEAR4 == null ? null : details_agreement_existing.TEND_AMOUNT_YEAR4;
                    agreementDetails.TEND_AMOUNT_YEAR5 = details_agreement_existing.TEND_AMOUNT_YEAR5 == null ? null : details_agreement_existing.TEND_AMOUNT_YEAR5;
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementDetails.TEND_AMOUNT_YEAR6 = details_agreement_existing.TEND_AMOUNT_YEAR6_NEW == null ? null : details_agreement_existing.TEND_AMOUNT_YEAR6_NEW;
                        agreementDetails.TEND_STATE_SHARE = details_agreement_existing.TEND_STATE_SHARE_NEW == null ? null : details_agreement_existing.TEND_STATE_SHARE_NEW;
                        agreementDetails.TEND_MORD_SHARE = details_agreement_existing.TEND_MORD_SHARE_NEW == null ? null : details_agreement_existing.TEND_MORD_SHARE_NEW;
                        agreementDetails.TEND_HIGHER_SPEC_AMT = details_agreement_existing.TEND_HIGHER_SPEC_AMT_NEW == null ? null : details_agreement_existing.TEND_HIGHER_SPEC_AMT_NEW;
                    }
                    agreementDetails.TEND_START_CHAINAGE = details_agreement_existing.TEND_START_CHAINAGE_Existing == null ? null : (details_agreement_existing.TEND_START_CHAINAGE_Existing);
                    agreementDetails.TEND_END_CHAINAGE = details_agreement_existing.TEND_END_CHAINAGE_Existing == null ? null : (details_agreement_existing.TEND_END_CHAINAGE_Existing);
                    // agreementDetails.TEND_PART_AGREEMENT = "N";

                    //if (details_agreement_existing.IsPartAgreement_Existing)
                    //{
                    //    agreementDetails.TEND_PART_AGREEMENT = "Y";
                    //    agreementDetails.TEND_START_CHAINAGE = (Decimal)details_agreement_existing.TEND_START_CHAINAGE_Existing;
                    //    agreementDetails.TEND_END_CHAINAGE = (Decimal)details_agreement_existing.TEND_END_CHAINAGE_Existing;
                    //}
                    //else
                    //{
                    //    agreementDetails.TEND_PART_AGREEMENT = "N";
                    //}

                    if (details_agreement_existing.IMS_WORK_CODE > 0)
                    {
                        agreementDetails.TEND_PART_AGREEMENT = "Y";
                        agreementDetails.IMS_WORK_CODE = details_agreement_existing.IMS_WORK_CODE;

                        if (dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_WORK_CODE == details_agreement_existing.IMS_WORK_CODE).Any())
                        {
                            agreementDetails.TEND_FIRST_AGREEMENT = "N";
                        }
                        else
                        {
                            agreementDetails.TEND_FIRST_AGREEMENT = "Y";
                        }

                    }
                    else
                    {
                        agreementDetails.TEND_PART_AGREEMENT = "N";

                        if (dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode).Any())
                        {
                            agreementDetails.TEND_FIRST_AGREEMENT = "N";
                        }
                        else
                        {
                            agreementDetails.TEND_FIRST_AGREEMENT = "Y";
                        }
                    }
                    agreementDetails.TEND_AGREEMENT_STATUS = "P";

                    //added by Koustubh Nakate 01/11/2013 for differntiate first agreement

                    agreementDetails.TEND_INCLUDE_ROAD_AMT = "Y";
                    agreementDetails.USERID = PMGSYSession.Current.UserId;
                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.TEND_AGREEMENT_DETAIL.Add(agreementDetails);

                    agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                    if (agreementMaster == null)
                    {
                        return false;
                    }

                    agreementMaster.TEND_AGREEMENT_AMOUNT += (Decimal)details_agreement_existing.TEND_AGREEMENT_AMOUNT_NEW;
                    agreementMaster.TEND_AMOUNT_YEAR1 = (agreementMaster.TEND_AMOUNT_YEAR1 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR1) + (details_agreement_existing.TEND_AMOUNT_YEAR1 == null ? 0 : details_agreement_existing.TEND_AMOUNT_YEAR1);
                    agreementMaster.TEND_AMOUNT_YEAR2 = (agreementMaster.TEND_AMOUNT_YEAR2 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR2) + (details_agreement_existing.TEND_AMOUNT_YEAR2 == null ? 0 : details_agreement_existing.TEND_AMOUNT_YEAR2);
                    agreementMaster.TEND_AMOUNT_YEAR3 = (agreementMaster.TEND_AMOUNT_YEAR3 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR3) + (details_agreement_existing.TEND_AMOUNT_YEAR3 == null ? 0 : details_agreement_existing.TEND_AMOUNT_YEAR3);
                    agreementMaster.TEND_AMOUNT_YEAR4 = (agreementMaster.TEND_AMOUNT_YEAR4 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR4) + (details_agreement_existing.TEND_AMOUNT_YEAR4 == null ? 0 : details_agreement_existing.TEND_AMOUNT_YEAR4);
                    agreementMaster.TEND_AMOUNT_YEAR5 = (agreementMaster.TEND_AMOUNT_YEAR5 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR5) + (details_agreement_existing.TEND_AMOUNT_YEAR5 == null ? 0 : details_agreement_existing.TEND_AMOUNT_YEAR5);
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementMaster.TEND_AMOUNT_YEAR6 = (agreementMaster.TEND_AMOUNT_YEAR6 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR6) + (details_agreement_existing.TEND_AMOUNT_YEAR6_NEW == null ? 0 : details_agreement_existing.TEND_AMOUNT_YEAR6_NEW);
                        agreementMaster.TEND_STATE_SHARE = (agreementMaster.TEND_STATE_SHARE == null ? null : agreementMaster.TEND_STATE_SHARE) + (details_agreement_existing.TEND_STATE_SHARE_NEW == null ? 0 : details_agreement_existing.TEND_STATE_SHARE_NEW);
                        agreementMaster.TEND_MORD_SHARE = (agreementMaster.TEND_MORD_SHARE == null ? null : agreementMaster.TEND_MORD_SHARE) + (details_agreement_existing.TEND_MORD_SHARE_NEW == null ? 0 : details_agreement_existing.TEND_MORD_SHARE_NEW);
                        agreementMaster.TEND_HIGHER_SPEC_AMT = (agreementMaster.TEND_HIGHER_SPEC_AMT == null ? null : agreementMaster.TEND_HIGHER_SPEC_AMT) + (details_agreement_existing.TEND_HIGHER_SPEC_AMT_NEW == null ? 0 : details_agreement_existing.TEND_HIGHER_SPEC_AMT_NEW);
                    }
                    agreementMaster.USERID = PMGSYSession.Current.UserId;
                    agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;

                    dbContext.SaveChanges();

                    //if (dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode).Count() == 1)
                    //{
                    //    dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).ToList().ForEach(IMS => IMS.IMS_ISCOMPLETED = "G");//&& IMS.IMS_ISCOMPLETED == "W"
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


        public Array GetAgreementDetailsListDAL_ByAgreementCode(int agreementCode, int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int agreementID = 0;
            long records = 0;
            bool isRegularAgreement = true;
            try
            {

                var query = from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                            join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                            on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                            join imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                            on tendAgreementDetails.IMS_PR_ROAD_CODE equals imsSanctionedProjectDetails.IMS_PR_ROAD_CODE
                            join proposalWorks in dbContext.IMS_PROPOSAL_WORK
                            on tendAgreementDetails.IMS_WORK_CODE equals proposalWorks.IMS_WORK_CODE into works
                            from proposalWorks in works.DefaultIfEmpty()

                            where
                            tendAgreementDetails.TEND_AGREEMENT_CODE == agreementCode
                            && tendAgreementMaster.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                            && imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme // new change done by Vikram as roads should be populated according to the Scheme
                            // tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                            //&&  tendAgreementDetails.TEND_AGREEMENT_STATUS != "W"
                            select new
                            {
                                tendAgreementDetails.TEND_AGREEMENT_ID,
                                tendAgreementDetails.TEND_AGREEMENT_CODE,
                                tendAgreementDetails.IMS_PR_ROAD_CODE,
                                tendAgreementDetails.IMS_WORK_CODE,
                                imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                tendAgreementDetails.TEND_AGREEMENT_AMOUNT,
                                //added on 15-07-2022
                                tendAgreementDetails.GST_AMT_MAINT_AGREEMENT_DLP,

                                tendAgreementDetails.TEND_AMOUNT_YEAR1,
                                tendAgreementDetails.TEND_AMOUNT_YEAR2,
                                tendAgreementDetails.TEND_AMOUNT_YEAR3,
                                tendAgreementDetails.TEND_AMOUNT_YEAR4,
                                tendAgreementDetails.TEND_AMOUNT_YEAR5,
                                tendAgreementDetails.TEND_PART_AGREEMENT,
                                tendAgreementDetails.TEND_START_CHAINAGE,
                                tendAgreementDetails.TEND_END_CHAINAGE,
                                tendAgreementDetails.TEND_AGREEMENT_STATUS,
                                tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                                tendAgreementMaster.TEND_LOCK_STATUS,
                                proposalWorks.IMS_WORK_DESC,
                                tendAgreementMaster.TEND_AGREEMENT_TYPE,
                                imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                                imsSanctionedProjectDetails.IMS_BRIDGE_NAME,
                                tendAgreementDetails.TEND_INCOMPLETE_REASON,
                                tendAgreementDetails.TEND_VALUE_WORK_DONE,
                                tendAgreementDetails.TEND_AMOUNT_YEAR6,
                                imsSanctionedProjectDetails.IMS_YEAR,
                                imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                                IsPhysicalCompleted = (imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "P" ? (dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(z => z.IMS_PR_ROAD_CODE == imsSanctionedProjectDetails.IMS_PR_ROAD_CODE && z.EXEC_ISCOMPLETED ==                                                           "C" && z.EXEC_COMPLETION_DATE != null).Any()) : (dbContext.EXEC_LSB_MONTHLY_STATUS.Where(z => z.IMS_PR_ROAD_CODE == imsSanctionedProjectDetails.IMS_PR_ROAD_CODE && z.EXEC_ISCOMPLETED == "C" &&                                                            z.EXEC_COMPLETION_DATE != null).Any()))
                            };


                totalRecords = query == null ? 0 : query.Count();

                if (query.Any(q => q.TEND_AGREEMENT_TYPE == "O"))
                {
                    isRegularAgreement = false;
                }

                var existingAgreementDetails = (from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                                join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                                                on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                                                where
                                                tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                                tendAgreementMaster.TEND_AGREEMENT_TYPE == "O"
                                                // &&  tendAgreementDetails.IMS_WORK_CODE == query.Select(q => q.IMS_WORK_CODE).FirstOrDefault()
                                                select tendAgreementDetails
                           );

                if (existingAgreementDetails.Count() > 0)
                {
                    agreementID = existingAgreementDetails.Max(ad => ad.TEND_AGREEMENT_ID);
                    records = existingAgreementDetails.Count();

                    if (records > 1 && agreementID == query.Select(q => q.TEND_AGREEMENT_ID).FirstOrDefault())
                    {
                        records = 0;
                    }
                }

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "PartAgreement":
                                query = query.OrderBy(x => x.TEND_PART_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                            default:
                                query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "PartAgreement":
                                query = query.OrderByDescending(x => x.TEND_PART_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(tendAgreementDetails => new
                {
                    tendAgreementDetails.TEND_AGREEMENT_ID,
                    tendAgreementDetails.TEND_AGREEMENT_CODE,
                    tendAgreementDetails.IMS_PR_ROAD_CODE,
                    tendAgreementDetails.IMS_ROAD_NAME,
                    tendAgreementDetails.TEND_AGREEMENT_AMOUNT,
                    //added on 15-07-2022
                    tendAgreementDetails.GST_AMT_MAINT_AGREEMENT_DLP,

                    tendAgreementDetails.TEND_AMOUNT_YEAR1,
                    tendAgreementDetails.TEND_AMOUNT_YEAR2,
                    tendAgreementDetails.TEND_AMOUNT_YEAR3,
                    tendAgreementDetails.TEND_AMOUNT_YEAR4,
                    tendAgreementDetails.TEND_AMOUNT_YEAR5,
                    tendAgreementDetails.TEND_AGREEMENT_STATUS,
                    tendAgreementDetails.TEND_PART_AGREEMENT,
                    tendAgreementDetails.TEND_START_CHAINAGE,
                    tendAgreementDetails.TEND_END_CHAINAGE,
                    tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED,
                    tendAgreementDetails.TEND_LOCK_STATUS,
                    tendAgreementDetails.IMS_WORK_DESC,
                    tendAgreementDetails.IMS_PROPOSAL_TYPE,
                    tendAgreementDetails.IMS_BRIDGE_NAME,
                    tendAgreementDetails.TEND_INCOMPLETE_REASON,
                    tendAgreementDetails.TEND_VALUE_WORK_DONE,
                    tendAgreementDetails.TEND_AMOUNT_YEAR6,
                    tendAgreementDetails.IMS_PACKAGE_ID,
                    tendAgreementDetails.IMS_YEAR,
                    tendAgreementDetails.IsPhysicalCompleted
                }).ToArray();


                return result.Select(tendAgreementDetails => new
                {
                    cell = new[] {                                   
                                    tendAgreementDetails.IMS_YEAR == null?"-":tendAgreementDetails.IMS_YEAR + "-" + (tendAgreementDetails.IMS_YEAR + 1),
                                    tendAgreementDetails.IMS_PACKAGE_ID == null?"-":tendAgreementDetails.IMS_PACKAGE_ID.ToString(),
                                    tendAgreementDetails.IMS_PROPOSAL_TYPE=="P"?tendAgreementDetails.IMS_ROAD_NAME.ToString():(tendAgreementDetails.IMS_PROPOSAL_TYPE == "L" ? (tendAgreementDetails.IMS_BRIDGE_NAME==null?"NA":tendAgreementDetails.IMS_BRIDGE_NAME.ToString()) : (tendAgreementDetails.IMS_ROAD_NAME == null ? "" : tendAgreementDetails.IMS_ROAD_NAME.ToString())),                    
                                    //tendAgreementDetails.IMS_ROAD_NAME.ToString().Trim(),    
                                    tendAgreementDetails.IMS_WORK_DESC==null?"NA": tendAgreementDetails.IMS_WORK_DESC.ToString().Trim(),             
                                    tendAgreementDetails.TEND_PART_AGREEMENT.ToString().Trim()=="Y"?"Yes":"No",                                                 
                                    tendAgreementDetails.TEND_START_CHAINAGE==null?"NA":(tendAgreementDetails.TEND_START_CHAINAGE.ToString().Trim()),
                                    tendAgreementDetails.TEND_START_CHAINAGE==null?"NA":(tendAgreementDetails.TEND_END_CHAINAGE.ToString().Trim()),
                                    // change on 15-07-2022
                                    //tendAgreementDetails.TEND_AGREEMENT_AMOUNT.ToString(),                                   
                                    (tendAgreementDetails.TEND_AGREEMENT_AMOUNT + tendAgreementDetails.GST_AMT_MAINT_AGREEMENT_DLP).ToString(),

                                    ((tendAgreementDetails.TEND_AMOUNT_YEAR1==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR1)+
                                       (tendAgreementDetails.TEND_AMOUNT_YEAR2==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR2)+
                                       (tendAgreementDetails.TEND_AMOUNT_YEAR3==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR3)+
                                       (tendAgreementDetails.TEND_AMOUNT_YEAR4==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR4)+
                                       (tendAgreementDetails.TEND_AMOUNT_YEAR5==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR5) +(PMGSYSession.Current.PMGSYScheme == 2 ?(tendAgreementDetails.TEND_AMOUNT_YEAR6==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR6):0)
                                    ).ToString(),
                                    WorkStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString(),//AgreementStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString(),

                                      tendAgreementDetails.TEND_VALUE_WORK_DONE==null?"NA":(tendAgreementDetails.TEND_VALUE_WORK_DONE.ToString().Trim()),
                                      tendAgreementDetails.TEND_INCOMPLETE_REASON==null?"NA":(tendAgreementDetails.TEND_INCOMPLETE_REASON.ToString().Trim()),

                                    (tendAgreementDetails.TEND_AGREEMENT_STATUS =="C" || tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="N"||tendAgreementDetails.TEND_LOCK_STATUS=="Y" ||  records>1 )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString() }),

                                    (isRegularAgreement==true?(tendAgreementDetails.TEND_AGREEMENT_STATUS =="W" || tendAgreementDetails.IsPhysicalCompleted == true /*|| tendAgreementDetails.TEND_AGREEMENT_STATUS =="C" || tendAgreementDetails.TEND_AGREEMENT_STATUS =="M"*/ || tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="N") : (tendAgreementDetails.TEND_AGREEMENT_STATUS =="W" || tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="N") ) ?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString() }),

                                    (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47) ? ( tendAgreementDetails.TEND_AGREEMENT_STATUS == "I" ? string.Empty : URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + string.Empty,"ProposalType="+tendAgreementDetails.IMS_PROPOSAL_TYPE })) : (tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementDetails.TEND_LOCK_STATUS=="Y" || tendAgreementDetails.TEND_AGREEMENT_STATUS=="W" )?string.Empty: URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + string.Empty,"ProposalType="+tendAgreementDetails.IMS_PROPOSAL_TYPE }),

                                    (tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementDetails.TEND_LOCK_STATUS=="Y"|| tendAgreementDetails.TEND_AGREEMENT_STATUS=="W" )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + string.Empty,"ProposalType="+tendAgreementDetails.IMS_PROPOSAL_TYPE  })

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

        public bool ChangeTerminatedAgreementStatusDAL(int agreementCode, int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            TEND_AGREEMENT_DETAIL agreementDetails = null;
            string agreementType = string.Empty;

            try
            {
                using (var scope = new TransactionScope())
                {
                    agreementDetails = dbContext.TEND_AGREEMENT_DETAIL.Where(am => am.TEND_AGREEMENT_CODE == agreementCode && am.IMS_PR_ROAD_CODE == roadCode/* && am.TEND_AGREEMENT_STATUS != "C"*/).FirstOrDefault();

                    agreementDetails.TEND_INCOMPLETE_REASON = null;
                    agreementDetails.TEND_VALUE_WORK_DONE = null;
                    agreementDetails.TEND_AGREEMENT_STATUS = "P";

                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementDetails.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(agreementDetails).State = System.Data.Entity.EntityState.Modified;
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

        public bool ChangeTerminatedAgreementMasterStatusDAL(int agreementCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            TEND_AGREEMENT_MASTER agreementMaster = null;
            string agreementType = string.Empty;

            try
            {
                using (var scope = new TransactionScope())
                {
                    agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                    //agreementMaster.TEND_INCOMPLETE_REASON = null;
                    //agreementMaster.TEND_VALUE_WORK_DONE = null;
                    agreementMaster.TEND_AGREEMENT_STATUS = "P";

                    agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementMaster.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
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

        public Array GetAgreementDetailsListITNODAL_ByAgreementCode(int agreementCode, int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int agreementID = 0;
            long records = 0;
            bool isRegularAgreement = true;
            bool includeRoadAmt = false;

            if (dbContext.TEND_AGREEMENT_DETAIL.Any(m => m.TEND_AGREEMENT_CODE == agreementCode && m.TEND_INCLUDE_ROAD_AMT == "Y"))
            {
                includeRoadAmt = true;
            }

            try
            {

                var query = from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                            join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                            on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                            join imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                            on tendAgreementDetails.IMS_PR_ROAD_CODE equals imsSanctionedProjectDetails.IMS_PR_ROAD_CODE
                            join proposalWorks in dbContext.IMS_PROPOSAL_WORK
                            on tendAgreementDetails.IMS_WORK_CODE equals proposalWorks.IMS_WORK_CODE into works
                            from proposalWorks in works.DefaultIfEmpty()

                            where
                            tendAgreementDetails.TEND_AGREEMENT_CODE == agreementCode
                            && tendAgreementMaster.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                            && imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme // new change done by Vikram as roads should be populated according to the Scheme
                            // tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                            //&&  tendAgreementDetails.TEND_AGREEMENT_STATUS != "W"
                            select new
                            {
                                tendAgreementDetails.TEND_AGREEMENT_ID,
                                tendAgreementDetails.TEND_AGREEMENT_CODE,
                                tendAgreementDetails.IMS_PR_ROAD_CODE,
                                tendAgreementDetails.IMS_WORK_CODE,
                                imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                tendAgreementDetails.TEND_AGREEMENT_AMOUNT,
                                tendAgreementDetails.TEND_AMOUNT_YEAR1,
                                tendAgreementDetails.TEND_AMOUNT_YEAR2,
                                tendAgreementDetails.TEND_AMOUNT_YEAR3,
                                tendAgreementDetails.TEND_AMOUNT_YEAR4,
                                tendAgreementDetails.TEND_AMOUNT_YEAR5,
                                tendAgreementDetails.TEND_PART_AGREEMENT,
                                tendAgreementDetails.TEND_START_CHAINAGE,
                                tendAgreementDetails.TEND_END_CHAINAGE,
                                tendAgreementDetails.TEND_AGREEMENT_STATUS,
                                tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                                tendAgreementMaster.TEND_LOCK_STATUS,
                                proposalWorks.IMS_WORK_DESC,
                                tendAgreementMaster.TEND_AGREEMENT_TYPE,
                                imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                                imsSanctionedProjectDetails.IMS_BRIDGE_NAME,
                                tendAgreementDetails.TEND_INCOMPLETE_REASON,
                                tendAgreementDetails.TEND_VALUE_WORK_DONE,
                                tendAgreementDetails.TEND_AMOUNT_YEAR6,
                                imsSanctionedProjectDetails.IMS_YEAR,
                                imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                                tendAgreementDetails.TEND_INCLUDE_ROAD_AMT

                            };


                totalRecords = query == null ? 0 : query.Count();

                if (query.Any(q => q.TEND_AGREEMENT_TYPE == "O"))
                {
                    isRegularAgreement = false;
                }

                var existingAgreementDetails = (from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                                join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                                                on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                                                where
                                                tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                                tendAgreementMaster.TEND_AGREEMENT_TYPE == "O"
                                                // &&  tendAgreementDetails.IMS_WORK_CODE == query.Select(q => q.IMS_WORK_CODE).FirstOrDefault()
                                                select tendAgreementDetails
                           );

                if (existingAgreementDetails.Count() > 0)
                {
                    agreementID = existingAgreementDetails.Max(ad => ad.TEND_AGREEMENT_ID);
                    records = existingAgreementDetails.Count();

                    if (records > 1 && agreementID == query.Select(q => q.TEND_AGREEMENT_ID).FirstOrDefault())
                    {
                        records = 0;
                    }
                }

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "PartAgreement":
                                query = query.OrderBy(x => x.TEND_PART_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                            default:
                                query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "PartAgreement":
                                query = query.OrderByDescending(x => x.TEND_PART_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(tendAgreementDetails => new
                {
                    tendAgreementDetails.TEND_AGREEMENT_ID,
                    tendAgreementDetails.TEND_AGREEMENT_CODE,
                    tendAgreementDetails.IMS_PR_ROAD_CODE,
                    tendAgreementDetails.IMS_ROAD_NAME,
                    tendAgreementDetails.TEND_AGREEMENT_AMOUNT,
                    tendAgreementDetails.TEND_AMOUNT_YEAR1,
                    tendAgreementDetails.TEND_AMOUNT_YEAR2,
                    tendAgreementDetails.TEND_AMOUNT_YEAR3,
                    tendAgreementDetails.TEND_AMOUNT_YEAR4,
                    tendAgreementDetails.TEND_AMOUNT_YEAR5,
                    tendAgreementDetails.TEND_AGREEMENT_STATUS,
                    tendAgreementDetails.TEND_PART_AGREEMENT,
                    tendAgreementDetails.TEND_START_CHAINAGE,
                    tendAgreementDetails.TEND_END_CHAINAGE,
                    tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED,
                    tendAgreementDetails.TEND_LOCK_STATUS,
                    tendAgreementDetails.IMS_WORK_DESC,
                    tendAgreementDetails.IMS_PROPOSAL_TYPE,
                    tendAgreementDetails.IMS_BRIDGE_NAME,
                    tendAgreementDetails.TEND_INCOMPLETE_REASON,
                    tendAgreementDetails.TEND_VALUE_WORK_DONE,
                    tendAgreementDetails.TEND_AMOUNT_YEAR6,
                    tendAgreementDetails.IMS_PACKAGE_ID,
                    tendAgreementDetails.IMS_YEAR,
                    tendAgreementDetails.TEND_INCLUDE_ROAD_AMT
                }).ToArray();


                return result.Select(tendAgreementDetails => new
                {
                    cell = new[] {                                   
                                    tendAgreementDetails.IMS_YEAR == null?"-":tendAgreementDetails.IMS_YEAR + "-" + (tendAgreementDetails.IMS_YEAR + 1),
                                    tendAgreementDetails.IMS_PACKAGE_ID == null?"-":tendAgreementDetails.IMS_PACKAGE_ID.ToString(),
                                    tendAgreementDetails.IMS_PROPOSAL_TYPE=="P"?tendAgreementDetails.IMS_ROAD_NAME.ToString():(tendAgreementDetails.IMS_BRIDGE_NAME==null?"NA":tendAgreementDetails.IMS_BRIDGE_NAME.ToString()),                    
                                    //tendAgreementDetails.IMS_ROAD_NAME.ToString().Trim(),    
                                    tendAgreementDetails.IMS_WORK_DESC==null?"NA": tendAgreementDetails.IMS_WORK_DESC.ToString().Trim(),             
                                    tendAgreementDetails.TEND_PART_AGREEMENT.ToString().Trim()=="Y"?"Yes":"No",                                                 
                                    tendAgreementDetails.TEND_START_CHAINAGE==null?"NA":(tendAgreementDetails.TEND_START_CHAINAGE.ToString().Trim()),
                                    tendAgreementDetails.TEND_START_CHAINAGE==null?"NA":(tendAgreementDetails.TEND_END_CHAINAGE.ToString().Trim()),
                                    includeRoadAmt == true ? "NA" : tendAgreementDetails.TEND_AGREEMENT_AMOUNT.ToString(),      
                                    includeRoadAmt == true ? "NA" :((tendAgreementDetails.TEND_AMOUNT_YEAR1==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR1)+
                                       (tendAgreementDetails.TEND_AMOUNT_YEAR2==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR2)+
                                       (tendAgreementDetails.TEND_AMOUNT_YEAR3==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR3)+
                                       (tendAgreementDetails.TEND_AMOUNT_YEAR4==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR4)+
                                       (tendAgreementDetails.TEND_AMOUNT_YEAR5==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR5) +(PMGSYSession.Current.PMGSYScheme == 2 ?(tendAgreementDetails.TEND_AMOUNT_YEAR6==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR6):0)
                                    ).ToString(),
                                    //WorkStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString(),//AgreementStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString(),
                                    
                                    //WorkStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString() != "Work Terminated" ? WorkStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString() : "<a href='#' title='Agreement Status' onClick=ChangeTerminatedAgreementStatus('"+ URLEncrypt.EncryptParameters1(new string[] { tendAgreementDetails.TEND_AGREEMENT_CODE.ToString() + "$" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString() }) + "'); return false;>"+ WorkStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString() +"</a>",
                                    /// Changed by SAMMED A. PATIL on 19MAR2018 for ITNOTRIPURA issue, provision to change status of Completed work at ITNO
                                    ((WorkStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString() != "Work Terminated") && (WorkStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString() != "Work Completed")) ? WorkStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString() : "<a href='#' title='Agreement Status' onClick=ChangeTerminatedAgreementStatus('"+ URLEncrypt.EncryptParameters1(new string[] { tendAgreementDetails.TEND_AGREEMENT_CODE.ToString() + "$" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString() }) + "'); return false;>"+ WorkStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString() +"</a>",

                                    //URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString() }),

                                      tendAgreementDetails.TEND_VALUE_WORK_DONE==null?"NA":(tendAgreementDetails.TEND_VALUE_WORK_DONE.ToString().Trim()),
                                      tendAgreementDetails.TEND_INCOMPLETE_REASON==null?"NA":(tendAgreementDetails.TEND_INCOMPLETE_REASON.ToString().Trim()),

                                    (tendAgreementDetails.TEND_AGREEMENT_STATUS =="C" || tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="N"||tendAgreementDetails.TEND_LOCK_STATUS=="Y" ||  records>1 )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString() }),
                                    (isRegularAgreement==true?(tendAgreementDetails.TEND_AGREEMENT_STATUS =="W" || tendAgreementDetails.TEND_AGREEMENT_STATUS =="C" || tendAgreementDetails.TEND_AGREEMENT_STATUS =="M" || tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="N") : (tendAgreementDetails.TEND_AGREEMENT_STATUS =="W" || tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="N") ) ?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString() }),
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + string.Empty,"ProposalType="+tendAgreementDetails.IMS_PROPOSAL_TYPE }),//includeRoadAmt == true ? string.Empty : 
                                    string.Empty 
                                    //? 
                                    //(
                                    //    (tendAgreementDetails.TEND_AGREEMENT_STATUS == "I" 
                                    //    ? 
                                    //    string.Empty 
                                    //    : URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + string.Empty,"ProposalType="+tendAgreementDetails.IMS_PROPOSAL_TYPE }))
                                    //) 
                                    //    : 
                                    //    (
                                    //        (tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementDetails.TEND_LOCK_STATUS=="Y" || tendAgreementDetails.TEND_AGREEMENT_STATUS=="W" )
                                    //        ?
                                    //        string.Empty
                                    //        : 
                                    //        URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + string.Empty,"ProposalType="+tendAgreementDetails.IMS_PROPOSAL_TYPE })
                                    //    ),
                                    //    (
                                    //    tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementDetails.TEND_LOCK_STATUS=="Y"|| tendAgreementDetails.TEND_AGREEMENT_STATUS=="W" 
                                    //    )
                                    //    ?
                                    //    string.Empty
                                    //    :
                                    //    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + string.Empty,"ProposalType="+tendAgreementDetails.IMS_PROPOSAL_TYPE  })

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



        public AgreementDetails GetAgreementMasterDetailsDAL_ByAgreementCode(int agreementCode, bool isView = false)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;
                CommonFunctions commonFunction = new CommonFunctions();
                TEND_AGREEMENT_MASTER master_agreement = null;

                TEND_AGREEMENT_DETAIL detail_agreement = null;


                if (!isView)
                {
                    if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56)
                    {
                        master_agreement = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                        // Added By Rohit Borse on 01-07-2022
                        detail_agreement = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == master_agreement.TEND_AGREEMENT_CODE).FirstOrDefault();
                    }
                    else
                    {
                        master_agreement = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode && am.TEND_IS_AGREEMENT_FINALIZED == "N" && am.TEND_LOCK_STATUS == "N").FirstOrDefault();

                        // Added By Rohit Borse on 01-07-2022
                        detail_agreement = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == master_agreement.TEND_AGREEMENT_CODE).FirstOrDefault();
                    }
                }
                else
                {
                    master_agreement = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                    // Added By Rohit Borse on 01-07-2022
                    detail_agreement = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == master_agreement.TEND_AGREEMENT_CODE).FirstOrDefault();
                }

                AgreementDetails agreementDetails = null;

                if (master_agreement != null)
                {


                    agreementDetails = new AgreementDetails()
                    {

                        EncryptedTendAgreementCode = URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + master_agreement.TEND_AGREEMENT_CODE.ToString() }),

                        Mast_Con_Sup_Flag = master_agreement.TEND_AGREEMENT_TYPE == "S" ? "S" : "C",
                        MAST_CON_ID = master_agreement.MAST_CON_ID == null ? 0 : (Int32)master_agreement.MAST_CON_ID,
                        TEND_AGREEMENT_NUMBER = master_agreement.TEND_AGREEMENT_NUMBER,
                        TEND_DATE_OF_AGREEMENT = Convert.ToDateTime(master_agreement.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),
                        TEND_AGREEMENT_START_DATE = Convert.ToDateTime(master_agreement.TEND_AGREEMENT_START_DATE).ToString("dd/MM/yyyy"),
                        TEND_AGREEMENT_END_DATE = Convert.ToDateTime(master_agreement.TEND_AGREEMENT_END_DATE).ToString("dd/MM/yyyy"),
                        TEND_DATE_OF_AWARD_WORK = master_agreement.TEND_DATE_OF_AWARD_WORK == null ? string.Empty : Convert.ToDateTime(master_agreement.TEND_DATE_OF_AWARD_WORK).ToString("dd/MM/yyyy"),
                        TEND_DATE_OF_WORK_ORDER = master_agreement.TEND_DATE_OF_WORK_ORDER == null ? string.Empty : Convert.ToDateTime(master_agreement.TEND_DATE_OF_WORK_ORDER).ToString("dd/MM/yyyy"),
                        TEND_DATE_OF_COMMENCEMENT = master_agreement.TEND_DATE_OF_COMMENCEMENT == null ? string.Empty : Convert.ToDateTime(master_agreement.TEND_DATE_OF_COMMENCEMENT).ToString("dd/MM/yyyy"),
                        TEND_DATE_OF_COMPLETION = master_agreement.TEND_DATE_OF_COMPLETION == null ? string.Empty : Convert.ToDateTime(master_agreement.TEND_DATE_OF_COMPLETION).ToString("dd/MM/yyyy"),
                        TEND_AGREEMENT_REMARKS = master_agreement.TEND_AGREEMENT_REMARKS,
                        TEND_TENDER_AMOUNT = master_agreement.TEND_TENDER_AMOUNT == null ? 0 : master_agreement.TEND_TENDER_AMOUNT,
                        TEND_AGREEMENT_AMOUNT = master_agreement.TEND_AGREEMENT_AMOUNT,
                        TEND_AMOUNT_YEAR1 = master_agreement.TEND_AMOUNT_YEAR1 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR1,
                        TEND_AMOUNT_YEAR2 = master_agreement.TEND_AMOUNT_YEAR2 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR2,
                        TEND_AMOUNT_YEAR3 = master_agreement.TEND_AMOUNT_YEAR3 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR3,
                        TEND_AMOUNT_YEAR4 = master_agreement.TEND_AMOUNT_YEAR4 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR4,
                        TEND_AMOUNT_YEAR5 = master_agreement.TEND_AMOUNT_YEAR5 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR5,
                        IncludeRoadAmount = dbContext.TEND_AGREEMENT_DETAIL.Any(m => m.TEND_AGREEMENT_CODE == agreementCode && m.TEND_INCLUDE_ROAD_AMT == "Y") ? true : false,

                        // Added By Rohit Borse on 01-07-2022
                        GST_AMT_MAINTAINANCE_AGREEMENT = master_agreement.GST_AMT_MAINT_AGREEMENT,
                        GST_AMT_MAINTAINANCE_AGREEMENT_DLP = detail_agreement.GST_AMT_MAINT_AGREEMENT_DLP,
                        APS_COLLECTED = master_agreement.APS_COLLECTED == null ? "N" : (master_agreement.APS_COLLECTED.Equals("Y") ? "Y" : "N"),
                        APS_COLLECTED_AMOUNT = master_agreement.APS_COLLECTED_AMOUNT
                    };

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementDetails.TEND_HIGHER_SPEC_AMT = master_agreement.TEND_HIGHER_SPEC_AMT;
                        agreementDetails.TEND_MORD_SHARE = master_agreement.TEND_MORD_SHARE;
                        agreementDetails.TEND_STATE_SHARE = master_agreement.TEND_STATE_SHARE;
                        agreementDetails.TEND_AMOUNT_YEAR6 = master_agreement.TEND_AMOUNT_YEAR6 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR6;
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


        public bool UpdateAgreementMasterDetailsDAL(AgreementDetails master_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                int agreementCode = 0;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;
                CommonFunctions commonFunction = new CommonFunctions();
                encryptedParameters = master_agreement.EncryptedTendAgreementCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString());

                if (dbContext.TEND_AGREEMENT_MASTER.Any(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.TEND_AGREEMENT_NUMBER.ToUpper() == master_agreement.TEND_AGREEMENT_NUMBER.ToUpper() && am.TEND_AGREEMENT_CODE != agreementCode))
                {
                    message = "Agreement Number is already exist.";
                    return false;
                }

                if (dbContext.TEND_AGREEMENT_DETAIL.Any(m => m.TEND_AGREEMENT_CODE == agreementCode))
                {
                    //-------------------------------------- commented and changed on 15-07-2022

                    //var totalRoadAmount = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.TEND_AGREEMENT_CODE == agreementCode).Sum(m => m.TEND_AGREEMENT_AMOUNT);
                    //if (totalRoadAmount != master_agreement.TEND_AGREEMENT_AMOUNT)
                    //{
                    //    message = "Agreement amount must be equal to the sum of amount for the roads.";
                    //    return false;
                    //}

                    var totalRoadAmount = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.TEND_AGREEMENT_CODE == agreementCode).Sum(m => m.TEND_AGREEMENT_AMOUNT + m.GST_AMT_MAINT_AGREEMENT_DLP);

                    if (totalRoadAmount != master_agreement.TEND_AGREEMENT_AMOUNT + master_agreement.GST_AMT_MAINTAINANCE_AGREEMENT_DLP)
                    {
                        message = "Agreement amount must be equal to the sum of amount for the roads.";
                        return false;
                    }
                    //-------------------------------------- Changes  END
                }

                //----------------------------------------------------- added on 15-07-2022 
                if (dbContext.TEND_AGREEMENT_DETAIL.Any(m => m.TEND_AGREEMENT_CODE == agreementCode))
                {
                    var getAllCost = (from ims in dbContext.IMS_SANCTIONED_PROJECTS
                                      join tenddetail in dbContext.TEND_AGREEMENT_DETAIL
                                      on ims.IMS_PR_ROAD_CODE equals tenddetail.IMS_PR_ROAD_CODE
                                      where tenddetail.TEND_AGREEMENT_CODE == agreementCode

                                      select new
                                      {
                                          ims.IMS_SANCTIONED_PAV_AMT,
                                          ims.IMS_SANCTIONED_PW_AMT,
                                          ims.IMS_SANCTIONED_OW_AMT,
                                          ims.IMS_SANCTIONED_CD_AMT,
                                          ims.IMS_SANCTIONED_FC_AMT,
                                          ims.IMS_SANCTIONED_HS_AMT,
                                          ims.IMS_SANCTIONED_BW_AMT,
                                          ims.IMS_SANCTIONED_BS_AMT,
                                          ims.IMS_SANCTIONED_RS_AMT,
                                          ims.IMS_PUCCA_SIDE_DRAINS
                                      }).FirstOrDefault();


                    var getSchemeSanctionedCost = (PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ?
                        ((getAllCost.IMS_SANCTIONED_PAV_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_PAV_AMT) + (getAllCost.IMS_SANCTIONED_PW_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_PW_AMT) + (getAllCost.IMS_SANCTIONED_OW_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_OW_AMT) + (getAllCost.IMS_SANCTIONED_CD_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_CD_AMT) + (getAllCost.IMS_SANCTIONED_BW_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_BW_AMT) + (getAllCost.IMS_SANCTIONED_BS_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_BS_AMT) + (getAllCost.IMS_SANCTIONED_RS_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_RS_AMT))
                    : ((getAllCost.IMS_SANCTIONED_PAV_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_PAV_AMT) + (getAllCost.IMS_SANCTIONED_PW_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_PW_AMT) + (getAllCost.IMS_SANCTIONED_OW_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_OW_AMT) + (getAllCost.IMS_SANCTIONED_CD_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_CD_AMT) + (getAllCost.IMS_SANCTIONED_FC_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_FC_AMT) + (getAllCost.IMS_SANCTIONED_HS_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_HS_AMT) + (getAllCost.IMS_SANCTIONED_BW_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_BW_AMT) + (getAllCost.IMS_SANCTIONED_BS_AMT == null ? 0 : getAllCost.IMS_SANCTIONED_BS_AMT) + (getAllCost.IMS_PUCCA_SIDE_DRAINS == null ? 0 : getAllCost.IMS_PUCCA_SIDE_DRAINS));


                    if (master_agreement.TEND_TENDER_AMOUNT > (getSchemeSanctionedCost * 10) || master_agreement.GST_AMT_MAINTAINANCE_AGREEMENT > (getSchemeSanctionedCost * 10) || master_agreement.TEND_AGREEMENT_AMOUNT > (getSchemeSanctionedCost * 10) || master_agreement.GST_AMT_MAINTAINANCE_AGREEMENT_DLP > (getSchemeSanctionedCost * 10) || master_agreement.APS_COLLECTED_AMOUNT > (getSchemeSanctionedCost * 10)
                        || master_agreement.TEND_AMOUNT_YEAR1 > (getSchemeSanctionedCost * 10) || master_agreement.TEND_AMOUNT_YEAR2 > (getSchemeSanctionedCost * 10) || master_agreement.TEND_AMOUNT_YEAR3 > (getSchemeSanctionedCost * 10) || master_agreement.TEND_AMOUNT_YEAR4 > (getSchemeSanctionedCost * 10) || master_agreement.TEND_AMOUNT_YEAR5 > (getSchemeSanctionedCost * 10)
                        || master_agreement.TEND_AMOUNT_YEAR6 > (getSchemeSanctionedCost * 10))
                    {
                        message = "Enter Amount in Lakhs ( Tender Amount / Agreement Amount / Year wise maintenance  Amount ) ";
                        return false;
                    }

                    if (stateCode == 27)
                    {
                        if (master_agreement.TEND_AGREEMENT_AMOUNT > (getSchemeSanctionedCost + (getSchemeSanctionedCost * (decimal)0.5)))
                        {
                            message = "Awarded cost more than 50% of GOI cost.Additional Cost Entry can be made only by SRRDA.";
                            return false;
                        }
                    }
                    else
                    {
                        if (master_agreement.TEND_AGREEMENT_AMOUNT > (getSchemeSanctionedCost + (getSchemeSanctionedCost * (decimal)0.2)))
                        {
                            message = "Awarded cost more than 20% of GOI cost.Additional Cost Entry can be made only by SRRDA.";
                            return false;
                        }
                    }
                }

                //----------------------------------------------------- Changes END --------------------------------------------

                //Check agreement number present in Maintenance Agreement
                var IMSContractList = (from IMSContracts in dbContext.MANE_IMS_CONTRACT
                                       join IMSSanctioned in dbContext.IMS_SANCTIONED_PROJECTS
                                       on IMSContracts.IMS_PR_ROAD_CODE equals IMSSanctioned.IMS_PR_ROAD_CODE
                                       where
                                       IMSSanctioned.MAST_STATE_CODE == stateCode &&
                                       IMSSanctioned.MAST_DISTRICT_CODE == districtCode &&
                                       IMSContracts.MANE_AGREEMENT_NUMBER.ToUpper() == master_agreement.TEND_AGREEMENT_NUMBER.ToUpper()
                                       select new
                                       {

                                           IMSContracts.MANE_AGREEMENT_NUMBER,
                                           IMSContracts.MAST_CON_ID,
                                           IMSContracts.IMS_PR_ROAD_CODE,
                                           IMSSanctioned.MAST_STATE_CODE,
                                           IMSSanctioned.MAST_DISTRICT_CODE

                                       });


                if (IMSContractList.Count() > 0)
                {
                    message = "Agreement Number is already exist for maintenance agreement.";
                    return false;
                }

                //else if (dbContext.TEND_AGREEMENT_MASTER.Any(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.MAST_CON_ID == master_agreement.MAST_CON_ID && am.TEND_AGREEMENT_NUMBER.ToUpper() == master_agreement.TEND_AGREEMENT_NUMBER.ToUpper() && am.TEND_AGREEMENT_CODE != agreementCode))
                //{
                //    message = "Agreement Number for selected contractor is already exist.";
                //    return false;
                //}

                TEND_AGREEMENT_MASTER agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                // added on 15-07-2022
                TEND_AGREEMENT_DETAIL agreementDetail = dbContext.TEND_AGREEMENT_DETAIL.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                if (agreementMaster == null)
                {
                    return false;
                }

                agreementMaster.TEND_TENDER_AMOUNT = master_agreement.TEND_TENDER_AMOUNT == null ? null : master_agreement.TEND_TENDER_AMOUNT;
                agreementMaster.TEND_AGREEMENT_NUMBER = master_agreement.TEND_AGREEMENT_NUMBER;
                agreementMaster.TEND_DATE_OF_AGREEMENT = commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_AGREEMENT);
                agreementMaster.TEND_AGREEMENT_START_DATE = commonFunction.GetStringToDateTime(master_agreement.TEND_AGREEMENT_START_DATE);
                agreementMaster.TEND_AGREEMENT_END_DATE = commonFunction.GetStringToDateTime(master_agreement.TEND_AGREEMENT_END_DATE);
                //new code added by Vikram as AGREEMENT_AMOUNT was not updating in edit mode.
                agreementMaster.TEND_AGREEMENT_AMOUNT = master_agreement.TEND_AGREEMENT_AMOUNT == null ? 0 : master_agreement.TEND_AGREEMENT_AMOUNT.Value;
                //end of change
                agreementMaster.TEND_DATE_OF_AWARD_WORK = master_agreement.TEND_DATE_OF_AWARD_WORK == null ? null : (DateTime?)commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_AWARD_WORK);
                agreementMaster.TEND_DATE_OF_WORK_ORDER = master_agreement.TEND_DATE_OF_WORK_ORDER == null ? null : (DateTime?)commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_WORK_ORDER);
                agreementMaster.TEND_DATE_OF_COMMENCEMENT = master_agreement.TEND_DATE_OF_COMMENCEMENT == null ? null : (DateTime?)commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_COMMENCEMENT);
                agreementMaster.TEND_DATE_OF_COMPLETION = master_agreement.TEND_DATE_OF_COMPLETION == null ? null : (DateTime?)commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_COMPLETION);

                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    // added on 15-07-2022
                    agreementMaster.TEND_AMOUNT_YEAR6 = (agreementMaster.TEND_AMOUNT_YEAR6 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR6) + ((master_agreement.TEND_AMOUNT_YEAR6 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR6) - (agreementDetail.TEND_AMOUNT_YEAR6 == null ? 0 : agreementDetail.TEND_AMOUNT_YEAR6));
                    agreementMaster.TEND_HIGHER_SPEC_AMT = master_agreement.TEND_HIGHER_SPEC_AMT;
                    agreementMaster.TEND_MORD_SHARE = master_agreement.TEND_MORD_SHARE;
                    agreementMaster.TEND_STATE_SHARE = master_agreement.TEND_STATE_SHARE;
                }


                agreementMaster.TEND_AMOUNT_YEAR1 = master_agreement.TEND_AMOUNT_YEAR1 == null ? null : master_agreement.TEND_AMOUNT_YEAR1;
                agreementMaster.TEND_AMOUNT_YEAR2 = master_agreement.TEND_AMOUNT_YEAR2 == null ? null : master_agreement.TEND_AMOUNT_YEAR2;
                agreementMaster.TEND_AMOUNT_YEAR3 = master_agreement.TEND_AMOUNT_YEAR3 == null ? null : master_agreement.TEND_AMOUNT_YEAR3;
                agreementMaster.TEND_AMOUNT_YEAR4 = master_agreement.TEND_AMOUNT_YEAR4 == null ? null : master_agreement.TEND_AMOUNT_YEAR4;
                agreementMaster.TEND_AMOUNT_YEAR5 = master_agreement.TEND_AMOUNT_YEAR5 == null ? null : master_agreement.TEND_AMOUNT_YEAR5;
                agreementMaster.TEND_AGREEMENT_REMARKS = master_agreement.TEND_AGREEMENT_REMARKS == null ? null : master_agreement.TEND_AGREEMENT_REMARKS.Trim();
                agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                agreementMaster.USERID = PMGSYSession.Current.UserId;
                agreementMaster.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme; //new change done by Vikram on 10 Feb 2014

                // added on 15-07-2022
                agreementMaster.GST_AMT_MAINT_AGREEMENT = master_agreement.GST_AMT_MAINTAINANCE_AGREEMENT;
                agreementMaster.APS_COLLECTED = master_agreement.APS_COLLECTED;
                agreementMaster.APS_COLLECTED_AMOUNT = master_agreement.APS_COLLECTED_AMOUNT;

                dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                // added on 15-07-2022
                agreementDetail.TEND_AMOUNT_YEAR1 = master_agreement.TEND_AMOUNT_YEAR1 == null ? null : master_agreement.TEND_AMOUNT_YEAR1;
                agreementDetail.TEND_AMOUNT_YEAR2 = master_agreement.TEND_AMOUNT_YEAR2 == null ? null : master_agreement.TEND_AMOUNT_YEAR2;
                agreementDetail.TEND_AMOUNT_YEAR3 = master_agreement.TEND_AMOUNT_YEAR3 == null ? null : master_agreement.TEND_AMOUNT_YEAR3;
                agreementDetail.TEND_AMOUNT_YEAR4 = master_agreement.TEND_AMOUNT_YEAR4 == null ? null : master_agreement.TEND_AMOUNT_YEAR4;
                agreementDetail.TEND_AMOUNT_YEAR5 = master_agreement.TEND_AMOUNT_YEAR5 == null ? null : master_agreement.TEND_AMOUNT_YEAR5;
                agreementDetail.GST_AMT_MAINT_AGREEMENT_DLP = master_agreement.GST_AMT_MAINTAINANCE_AGREEMENT_DLP;
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {

                    agreementDetail.TEND_AMOUNT_YEAR6 = master_agreement.TEND_AMOUNT_YEAR6;
                    agreementDetail.TEND_HIGHER_SPEC_AMT = master_agreement.TEND_HIGHER_SPEC_AMT;
                    agreementDetail.TEND_MORD_SHARE = master_agreement.TEND_MORD_SHARE;
                    agreementDetail.TEND_STATE_SHARE = master_agreement.TEND_STATE_SHARE;
                }
                
                dbContext.Entry(agreementDetail).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                //------------------- added end
                
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

        public bool UpdateAgreementMasterDetailsITNODAL(AgreementDetails master_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {


                    int agreementCode = 0;
                    int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                    int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;
                    CommonFunctions commonFunction = new CommonFunctions();
                    encryptedParameters = master_agreement.EncryptedTendAgreementCode.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString());


                    //Check agreement number present in Maintenance Agreement
                    var IMSContractList = (from IMSContracts in dbContext.MANE_IMS_CONTRACT
                                           join IMSSanctioned in dbContext.IMS_SANCTIONED_PROJECTS
                                           on IMSContracts.IMS_PR_ROAD_CODE equals IMSSanctioned.IMS_PR_ROAD_CODE
                                           where
                                           IMSSanctioned.MAST_STATE_CODE == stateCode &&
                                           IMSSanctioned.MAST_DISTRICT_CODE == districtCode &&
                                           IMSContracts.MANE_AGREEMENT_NUMBER.ToUpper() == master_agreement.TEND_AGREEMENT_NUMBER.ToUpper()
                                           select new
                                           {

                                               IMSContracts.MANE_AGREEMENT_NUMBER,
                                               IMSContracts.MAST_CON_ID,
                                               IMSContracts.IMS_PR_ROAD_CODE,
                                               IMSSanctioned.MAST_STATE_CODE,
                                               IMSSanctioned.MAST_DISTRICT_CODE

                                           });


                    if (IMSContractList.Count() > 0)
                    {
                        message = "Agreement Number is already exist for maintenance agreement.";
                        return false;
                    }

                    if (dbContext.TEND_AGREEMENT_DETAIL.Any(m => m.TEND_AGREEMENT_CODE == agreementCode && m.TEND_INCLUDE_ROAD_AMT == "Y"))
                    {
                        var agreementDetails = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.TEND_AGREEMENT_CODE == agreementCode).ToList();

                        agreementDetails.ForEach(m => { m.TEND_AGREEMENT_AMOUNT = (master_agreement.TEND_AGREEMENT_AMOUNT.HasValue ? master_agreement.TEND_AGREEMENT_AMOUNT.Value : m.TEND_AGREEMENT_AMOUNT); });

                        dbContext.SaveChanges();
                    }


                    TEND_AGREEMENT_MASTER agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                    if (agreementMaster == null)
                    {
                        return false;
                    }

                    #region Added on 06 May 2020 On Edit

                    Int32 ProposalCode = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.TEND_AGREEMENT_CODE == agreementCode).Select(m => m.IMS_PR_ROAD_CODE).FirstOrDefault();
                    DateTime? SanctionedDate = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == ProposalCode).Select(m => m.IMS_SANCTIONED_DATE).FirstOrDefault();

                    if (SanctionedDate != null)
                    {
                        agreementMaster.TEND_DATE_OF_AGREEMENT = commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_AGREEMENT);
                        if (agreementMaster.TEND_DATE_OF_AGREEMENT < SanctionedDate)
                        {
                            message = "Agreement Date must be greater than or equal to Sanctioned Date.";
                            return false;
                        }
                    }
                    #endregion

                    agreementMaster.TEND_TENDER_AMOUNT = master_agreement.TEND_TENDER_AMOUNT == null ? null : master_agreement.TEND_TENDER_AMOUNT;
                    agreementMaster.TEND_AGREEMENT_NUMBER = master_agreement.TEND_AGREEMENT_NUMBER;
                    agreementMaster.TEND_DATE_OF_AGREEMENT = commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_AGREEMENT);
                    agreementMaster.TEND_AGREEMENT_START_DATE = commonFunction.GetStringToDateTime(master_agreement.TEND_AGREEMENT_START_DATE);
                    agreementMaster.TEND_AGREEMENT_END_DATE = commonFunction.GetStringToDateTime(master_agreement.TEND_AGREEMENT_END_DATE);
                    //new code added by Vikram as AGREEMENT_AMOUNT was not updating in edit mode.
                    agreementMaster.TEND_AGREEMENT_AMOUNT = master_agreement.TEND_AGREEMENT_AMOUNT == null ? 0 : master_agreement.TEND_AGREEMENT_AMOUNT.Value;
                    //end of change
                    agreementMaster.TEND_DATE_OF_AWARD_WORK = master_agreement.TEND_DATE_OF_AWARD_WORK == null ? null : (DateTime?)commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_AWARD_WORK);
                    agreementMaster.TEND_DATE_OF_WORK_ORDER = master_agreement.TEND_DATE_OF_WORK_ORDER == null ? null : (DateTime?)commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_WORK_ORDER);
                    agreementMaster.TEND_DATE_OF_COMMENCEMENT = master_agreement.TEND_DATE_OF_COMMENCEMENT == null ? null : (DateTime?)commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_COMMENCEMENT);
                    agreementMaster.TEND_DATE_OF_COMPLETION = master_agreement.TEND_DATE_OF_COMPLETION == null ? null : (DateTime?)commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_COMPLETION);

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementMaster.TEND_HIGHER_SPEC_AMT = master_agreement.TEND_HIGHER_SPEC_AMT;
                        agreementMaster.TEND_MORD_SHARE = master_agreement.TEND_MORD_SHARE;
                        agreementMaster.TEND_STATE_SHARE = master_agreement.TEND_STATE_SHARE;
                    }

                    agreementMaster.TEND_AMOUNT_YEAR1 = master_agreement.TEND_AMOUNT_YEAR1 == null ? null : master_agreement.TEND_AMOUNT_YEAR1;
                    agreementMaster.TEND_AMOUNT_YEAR2 = master_agreement.TEND_AMOUNT_YEAR2 == null ? null : master_agreement.TEND_AMOUNT_YEAR2;
                    agreementMaster.TEND_AMOUNT_YEAR3 = master_agreement.TEND_AMOUNT_YEAR3 == null ? null : master_agreement.TEND_AMOUNT_YEAR3;
                    agreementMaster.TEND_AMOUNT_YEAR4 = master_agreement.TEND_AMOUNT_YEAR4 == null ? null : master_agreement.TEND_AMOUNT_YEAR4;
                    agreementMaster.TEND_AMOUNT_YEAR5 = master_agreement.TEND_AMOUNT_YEAR5 == null ? null : master_agreement.TEND_AMOUNT_YEAR5;

                    agreementMaster.TEND_AGREEMENT_REMARKS = master_agreement.TEND_AGREEMENT_REMARKS == null ? null : master_agreement.TEND_AGREEMENT_REMARKS.Trim();
                    agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementMaster.USERID = PMGSYSession.Current.UserId;
                    agreementMaster.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme; //new change done by Vikram on 10 Feb 2014
                    dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    ts.Complete();
                }
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


        public bool DeleteAgreementMasterDetailsDAL_ByAgreementCode(int agreementCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                TEND_AGREEMENT_MASTER master_agreement = dbContext.TEND_AGREEMENT_MASTER.Find(agreementCode);

                //return false if agreement is used in accounts module (payment is done on agreement)
                if (dbContext.ACC_BILL_DETAILS.Any(m => m.IMS_AGREEMENT_CODE == agreementCode))
                {
                    return false;
                }

                if (master_agreement == null)
                {
                    return false;

                }

                master_agreement.USERID = PMGSYSession.Current.UserId;
                master_agreement.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(master_agreement).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.TEND_AGREEMENT_MASTER.Remove(master_agreement);
                dbContext.SaveChanges();
                return true;

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


        public ExistingAgreementDetails GetAgreementDetailsDAL_ByAgreementID(int agreementCode, int agreementID)
        {
            ExistingAgreementDetails existingAgreementDetails = new ExistingAgreementDetails();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                TEND_AGREEMENT_MASTER master_agreement = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();
                TEND_AGREEMENT_DETAIL details_agreement = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_ID == agreementID).FirstOrDefault();
                IMS_SANCTIONED_PROJECTS projects_sanctioned = dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == details_agreement.IMS_PR_ROAD_CODE).FirstOrDefault();


                if (master_agreement == null || details_agreement == null || projects_sanctioned == null)
                {
                    return existingAgreementDetails;
                }

                existingAgreementDetails.EncryptedIMSPRRoadCode_Existing = URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + projects_sanctioned.IMS_PR_ROAD_CODE.ToString(), "ProposalType=" + projects_sanctioned.IMS_PROPOSAL_TYPE });

                existingAgreementDetails.EncryptedTendAgreementCode_Existing = URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + master_agreement.TEND_AGREEMENT_CODE.ToString() });
                existingAgreementDetails.TEND_DATE_OF_AGREEMENT = master_agreement.TEND_DATE_OF_AGREEMENT == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_AGREEMENT_START_DATE = master_agreement.TEND_AGREEMENT_START_DATE == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_AGREEMENT_START_DATE).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_AGREEMENT_END_DATE = master_agreement.TEND_AGREEMENT_END_DATE == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_AGREEMENT_END_DATE).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_TENDER_AMOUNT = master_agreement.TEND_TENDER_AMOUNT == null ? 0 : master_agreement.TEND_TENDER_AMOUNT;
                existingAgreementDetails.TEND_AGREEMENT_AMOUNT_Existing = master_agreement.TEND_AGREEMENT_AMOUNT;
                existingAgreementDetails.TEND_DATE_OF_AWARD_WORK = master_agreement.TEND_DATE_OF_AWARD_WORK == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_AWARD_WORK).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_DATE_OF_WORK_ORDER = master_agreement.TEND_DATE_OF_WORK_ORDER == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_WORK_ORDER).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_DATE_OF_COMMENCEMENT = master_agreement.TEND_DATE_OF_COMMENCEMENT == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_COMMENCEMENT).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_DATE_OF_COMPLETION = master_agreement.TEND_DATE_OF_COMPLETION == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_COMPLETION).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_AMOUNT_YEAR1_Existing = master_agreement.TEND_AMOUNT_YEAR1 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR1;
                existingAgreementDetails.TEND_AMOUNT_YEAR2_Existing = master_agreement.TEND_AMOUNT_YEAR2 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR2;
                existingAgreementDetails.TEND_AMOUNT_YEAR3_Existing = master_agreement.TEND_AMOUNT_YEAR3 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR3;
                existingAgreementDetails.TEND_AMOUNT_YEAR4_Existing = master_agreement.TEND_AMOUNT_YEAR4 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR4;
                existingAgreementDetails.TEND_AMOUNT_YEAR5_Existing = master_agreement.TEND_AMOUNT_YEAR5 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR5;

                // added by rohit borse on 01-07-2022
                existingAgreementDetails.GST_AMT_MAINTAINANCE_AGREEMENT_DLP_NEW = details_agreement.GST_AMT_MAINT_AGREEMENT_DLP;

                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    existingAgreementDetails.TEND_MORD_SHARE_NEW = details_agreement.TEND_MORD_SHARE == null ? 0 : details_agreement.TEND_MORD_SHARE;
                    existingAgreementDetails.TEND_STATE_SHARE_NEW = details_agreement.TEND_STATE_SHARE == null ? 0 : details_agreement.TEND_STATE_SHARE.Value;
                    existingAgreementDetails.TEND_HIGHER_SPEC_AMT_NEW = details_agreement.TEND_HIGHER_SPEC_AMT == null ? 0 : details_agreement.TEND_HIGHER_SPEC_AMT.Value;
                    existingAgreementDetails.TEND_AMOUNT_YEAR6_Existing = master_agreement.TEND_AMOUNT_YEAR6 == null ? 0 : master_agreement.TEND_AMOUNT_YEAR6;
                    existingAgreementDetails.TEND_AMOUNT_YEAR6_NEW = details_agreement.TEND_AMOUNT_YEAR6 == null ? null : details_agreement.TEND_AMOUNT_YEAR6;
                    if (projects_sanctioned.IMS_SHARE_PERCENT == 1)
                    {
                        existingAgreementDetails.ProposalStateShare = "10";
                        existingAgreementDetails.ProposalMordShare = "90";
                    }
                    else if (projects_sanctioned.IMS_SHARE_PERCENT == 2)
                    {
                        existingAgreementDetails.ProposalStateShare = "25";
                        existingAgreementDetails.ProposalMordShare = "75";
                    }

                    if (projects_sanctioned.IMS_PROPOSAL_TYPE == "P")
                    {
                        existingAgreementDetails.ProposalStateCost = projects_sanctioned.IMS_SANCTIONED_RS_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_RS_AMT;
                        existingAgreementDetails.ProposalMordCost = ((projects_sanctioned.IMS_SANCTIONED_PAV_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_PAV_AMT) + (projects_sanctioned.IMS_SANCTIONED_PW_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_PW_AMT) + (projects_sanctioned.IMS_SANCTIONED_OW_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_OW_AMT) + (projects_sanctioned.IMS_SANCTIONED_CD_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_CD_AMT) + (projects_sanctioned.IMS_SANCTIONED_FC_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_FC_AMT) - (projects_sanctioned.IMS_SANCTIONED_RS_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_RS_AMT)).Value;
                    }
                    else if (projects_sanctioned.IMS_PROPOSAL_TYPE == "L")
                    {
                        existingAgreementDetails.ProposalStateCost = projects_sanctioned.IMS_SANCTIONED_BS_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_BS_AMT;
                        existingAgreementDetails.ProposalMordCost = (projects_sanctioned.IMS_SANCTIONED_BW_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_BW_AMT);
                    }
                }
                //working but removed road name
                //existingAgreementDetails.EncryptedTendAgreementID_Existing = URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + master_agreement.TEND_AGREEMENT_CODE.ToString(), "TendAgreementID =" + details_agreement.TEND_AGREEMENT_ID.ToString(), "IMSPRRoadCode =" + projects_sanctioned.IMS_PR_ROAD_CODE.ToString(), "IMSRoadName =" + projects_sanctioned.IMS_ROAD_NAME.ToString() });

                existingAgreementDetails.EncryptedTendAgreementID_Existing = URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + master_agreement.TEND_AGREEMENT_CODE.ToString(), "TendAgreementID =" + details_agreement.TEND_AGREEMENT_ID.ToString(), "IMSPRRoadCode =" + projects_sanctioned.IMS_PR_ROAD_CODE.ToString(), "IMSRoadName =" + string.Empty, "ProposalType=" + projects_sanctioned.IMS_PROPOSAL_TYPE });
                existingAgreementDetails.TEND_AGREEMENT_AMOUNT_NEW = details_agreement.TEND_AGREEMENT_AMOUNT;
                existingAgreementDetails.TEND_AMOUNT_YEAR1 = details_agreement.TEND_AMOUNT_YEAR1 == null ? null : details_agreement.TEND_AMOUNT_YEAR1;
                existingAgreementDetails.TEND_AMOUNT_YEAR2 = details_agreement.TEND_AMOUNT_YEAR2 == null ? null : details_agreement.TEND_AMOUNT_YEAR2;
                existingAgreementDetails.TEND_AMOUNT_YEAR3 = details_agreement.TEND_AMOUNT_YEAR3 == null ? null : details_agreement.TEND_AMOUNT_YEAR3;
                existingAgreementDetails.TEND_AMOUNT_YEAR4 = details_agreement.TEND_AMOUNT_YEAR4 == null ? null : details_agreement.TEND_AMOUNT_YEAR4;
                existingAgreementDetails.TEND_AMOUNT_YEAR5 = details_agreement.TEND_AMOUNT_YEAR5 == null ? null : details_agreement.TEND_AMOUNT_YEAR5;
                existingAgreementDetails.TEND_AMOUNT_YEAR6_NEW = details_agreement.TEND_AMOUNT_YEAR6 == null ? null : details_agreement.TEND_AMOUNT_YEAR6;
                existingAgreementDetails.IsPartAgreement_Existing = details_agreement.TEND_PART_AGREEMENT == "Y" ? true : false;
                existingAgreementDetails.TEND_START_CHAINAGE_Existing = details_agreement.TEND_START_CHAINAGE == null ? null : details_agreement.TEND_START_CHAINAGE;
                existingAgreementDetails.TEND_END_CHAINAGE_Existing = details_agreement.TEND_END_CHAINAGE == null ? null : details_agreement.TEND_END_CHAINAGE;

                existingAgreementDetails.IMS_WORK_DESC = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == details_agreement.IMS_PR_ROAD_CODE && pw.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE).Select(pw => pw.IMS_WORK_DESC).FirstOrDefault();

                existingAgreementDetails.IMS_WORK_DESC = existingAgreementDetails.IMS_WORK_DESC == null ? "NA" : existingAgreementDetails.IMS_WORK_DESC;

                return existingAgreementDetails;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return existingAgreementDetails;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public bool UpdateAgreementDetailsDAL(ExistingAgreementDetails details_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int IMSPRRoadCode = 0;
                string roadName = string.Empty;
                int agreementID = 0;
                int agreementCode = 0;
                Decimal? roadLength = 0;
                TEND_AGREEMENT_DETAIL agreementDetails = null;
                TEND_AGREEMENT_MASTER agreementMaster = null;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                CommonFunctions commonFunction = new CommonFunctions();

                encryptedParameters = details_agreement.EncryptedTendAgreementID_Existing.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString());
                agreementID = Convert.ToInt32(decryptedParameters["TendAgreementID"].ToString());
                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                // roadName = decryptedParameters["IMSRoadName"].ToString().Trim();

                string proposalType = decryptedParameters["ProposalType"].ToString();

                if (proposalType.Equals("P"))
                {
                    roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_ROAD_NAME).FirstOrDefault();
                }
                else
                {
                    roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_BRIDGE_NAME).FirstOrDefault();
                }

                string agreementType = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).Select(am => (string)am.TEND_AGREEMENT_TYPE).FirstOrDefault();


                if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_PART_AGREEMENT == "Y" && ad.TEND_AGREEMENT_STATUS != "W" && ad.TEND_AGREEMENT_ID != agreementID))
                {
                    Int32? imsWorkCode = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_ID == agreementID).Select(ad => (Int32?)ad.IMS_WORK_CODE).FirstOrDefault();

                    if (imsWorkCode > 0)
                    {
                        if (details_agreement.TEND_START_CHAINAGE_Existing != null && details_agreement.TEND_END_CHAINAGE_Existing != null)
                        {
                            if ((details_agreement.TEND_END_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_END_CHAINAGE_Existing) > (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_WORK_CODE == imsWorkCode && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(pw => (Decimal?)pw.IMS_END_CHAINAGE).FirstOrDefault()) || (details_agreement.TEND_START_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_START_CHAINAGE_Existing) < (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_WORK_CODE == imsWorkCode && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(pw => (Decimal?)pw.IMS_START_CHAINAGE).FirstOrDefault()))
                            {
                                message = "Chainage exceeds its road length.";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        roadLength = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_AGREEMENT_STATUS != "W" && ad.TEND_AGREEMENT_ID != agreementID).Sum(td => (Decimal?)(td.TEND_END_CHAINAGE - td.TEND_START_CHAINAGE));
                        roadLength = (roadLength == null ? 0 : roadLength) + ((details_agreement.TEND_END_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_END_CHAINAGE_Existing) - (details_agreement.TEND_START_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_START_CHAINAGE_Existing));
                        if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                        {
                            message = "Chainage exceeds its road length.";
                            return false;
                        }
                    }
                }
                else
                {
                    roadLength += ((details_agreement.TEND_END_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_END_CHAINAGE_Existing) - (details_agreement.TEND_START_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_START_CHAINAGE_Existing));

                    if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                    {
                        message = "Chainage exceeds its road length.";
                        return false;
                    }
                }

                var sanctionProposal = dbContext.IMS_SANCTIONED_PROJECTS.Find(IMSPRRoadCode);
                decimal sanctionCost = sanctionProposal.IMS_PROPOSAL_TYPE == "P" ? (PMGSYSession.Current.PMGSYScheme == 1 ? (sanctionProposal.IMS_SANCTIONED_PAV_AMT + sanctionProposal.IMS_SANCTIONED_PW_AMT + sanctionProposal.IMS_SANCTIONED_OW_AMT + sanctionProposal.IMS_SANCTIONED_CD_AMT + sanctionProposal.IMS_SANCTIONED_RS_AMT) : (sanctionProposal.IMS_SANCTIONED_PAV_AMT + sanctionProposal.IMS_SANCTIONED_PW_AMT + sanctionProposal.IMS_SANCTIONED_OW_AMT + sanctionProposal.IMS_SANCTIONED_CD_AMT + sanctionProposal.IMS_SANCTIONED_RS_AMT + (sanctionProposal.IMS_SANCTIONED_HS_AMT.HasValue ? sanctionProposal.IMS_SANCTIONED_HS_AMT.Value : 0) + (sanctionProposal.IMS_SANCTIONED_FC_AMT.HasValue ? sanctionProposal.IMS_SANCTIONED_FC_AMT.Value : 0))) : (sanctionProposal.IMS_PROPOSAL_TYPE == "L" ? (sanctionProposal.IMS_SANCTIONED_BW_AMT + sanctionProposal.IMS_SANCTIONED_BS_AMT) : (sanctionProposal.IMS_SANCTIONED_PAV_AMT));

                if (dbContext.IMS_PROPOSAL_COST_ADD.Any(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                {
                    decimal? costToAdd = dbContext.IMS_PROPOSAL_COST_ADD.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(m => m.IMS_TRANSACTION_CODE).Select(m => m.IMS_STATE_AMOUNT).FirstOrDefault() + dbContext.IMS_PROPOSAL_COST_ADD.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(m => m.IMS_TRANSACTION_CODE).Select(m => m.IMS_MORD_AMOUNT).FirstOrDefault();
                    sanctionCost = sanctionCost + (costToAdd.HasValue ? costToAdd.Value : 0);
                }

                if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47)
                {
                    if (dbContext.EXEC_PROGRESS.Any(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                    {
                        var finProgress = dbContext.EXEC_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(m => m.EXEC_PROG_YEAR).ThenByDescending(m => m.EXEC_PROG_MONTH).FirstOrDefault();
                        decimal? finAmount = (finProgress.EXEC_PAYMENT_LASTMONTH == null ? 0 : finProgress.EXEC_PAYMENT_LASTMONTH) + (finProgress.EXEC_PAYMENT_THISMONTH == null ? 0 : finProgress.EXEC_PAYMENT_THISMONTH);
                        if (finAmount > details_agreement.TEND_AGREEMENT_AMOUNT_NEW)
                        {
                            message = "Agreement amount is less than the amount entered in financial progress.";
                            return false;
                        }
                    }
                }
                else if (details_agreement.TEND_AGREEMENT_AMOUNT_NEW > (sanctionCost + (sanctionCost * (decimal)0.2)))
                {
                    message = "Awarded cost more than 20% of GOI cost.Additional Cost Entry can be made only by SRRDA.";
                    return false;
                }

                using (var scope = new TransactionScope())
                {

                    agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();
                    agreementDetails = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_ID == agreementID).FirstOrDefault();

                    if (agreementMaster == null || agreementDetails == null)
                    {
                        return false;
                    }


                    agreementMaster.TEND_AGREEMENT_AMOUNT = agreementMaster.TEND_AGREEMENT_AMOUNT + (Decimal)(details_agreement.TEND_AGREEMENT_AMOUNT_NEW - agreementDetails.TEND_AGREEMENT_AMOUNT);
                    agreementMaster.TEND_AMOUNT_YEAR1 = (agreementMaster.TEND_AMOUNT_YEAR1 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR1) + ((details_agreement.TEND_AMOUNT_YEAR1 == null ? 0 : details_agreement.TEND_AMOUNT_YEAR1) - (agreementDetails.TEND_AMOUNT_YEAR1 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR1));
                    agreementMaster.TEND_AMOUNT_YEAR2 = (agreementMaster.TEND_AMOUNT_YEAR2 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR2) + ((details_agreement.TEND_AMOUNT_YEAR2 == null ? 0 : details_agreement.TEND_AMOUNT_YEAR2) - (agreementDetails.TEND_AMOUNT_YEAR2 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR2));
                    agreementMaster.TEND_AMOUNT_YEAR3 = (agreementMaster.TEND_AMOUNT_YEAR3 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR3) + ((details_agreement.TEND_AMOUNT_YEAR3 == null ? 0 : details_agreement.TEND_AMOUNT_YEAR3) - (agreementDetails.TEND_AMOUNT_YEAR3 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR3));
                    agreementMaster.TEND_AMOUNT_YEAR4 = (agreementMaster.TEND_AMOUNT_YEAR4 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR4) + ((details_agreement.TEND_AMOUNT_YEAR4 == null ? 0 : details_agreement.TEND_AMOUNT_YEAR4) - (agreementDetails.TEND_AMOUNT_YEAR4 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR4));
                    agreementMaster.TEND_AMOUNT_YEAR5 = (agreementMaster.TEND_AMOUNT_YEAR5 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR5) + ((details_agreement.TEND_AMOUNT_YEAR5 == null ? 0 : details_agreement.TEND_AMOUNT_YEAR5) - (agreementDetails.TEND_AMOUNT_YEAR5 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR5));
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementMaster.TEND_AMOUNT_YEAR6 = (agreementMaster.TEND_AMOUNT_YEAR6 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR6) + ((details_agreement.TEND_AMOUNT_YEAR6_NEW == null ? 0 : details_agreement.TEND_AMOUNT_YEAR6_NEW) - (agreementDetails.TEND_AMOUNT_YEAR6 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR6));
                        agreementMaster.TEND_STATE_SHARE = (agreementMaster.TEND_STATE_SHARE == null ? null : agreementMaster.TEND_STATE_SHARE) + (details_agreement.TEND_STATE_SHARE_NEW == null ? 0 : details_agreement.TEND_STATE_SHARE_NEW) - (agreementDetails.TEND_STATE_SHARE == null ? 0 : agreementDetails.TEND_STATE_SHARE);
                        agreementMaster.TEND_MORD_SHARE = (agreementMaster.TEND_MORD_SHARE == null ? null : agreementMaster.TEND_MORD_SHARE) + (details_agreement.TEND_MORD_SHARE_NEW == null ? 0 : details_agreement.TEND_MORD_SHARE_NEW) - (agreementDetails.TEND_MORD_SHARE == null ? 0 : agreementDetails.TEND_MORD_SHARE);
                        agreementMaster.TEND_HIGHER_SPEC_AMT = (agreementMaster.TEND_HIGHER_SPEC_AMT == null ? null : agreementMaster.TEND_HIGHER_SPEC_AMT) + (details_agreement.TEND_HIGHER_SPEC_AMT_NEW == null ? 0 : details_agreement.TEND_HIGHER_SPEC_AMT_NEW) - (agreementDetails.TEND_HIGHER_SPEC_AMT == null ? 0 : agreementDetails.TEND_HIGHER_SPEC_AMT);
                    }
                    agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementMaster.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;



                    agreementDetails.TEND_AGREEMENT_AMOUNT = (Decimal)details_agreement.TEND_AGREEMENT_AMOUNT_NEW;
                    agreementDetails.TEND_AMOUNT_YEAR1 = details_agreement.TEND_AMOUNT_YEAR1 == null ? null : details_agreement.TEND_AMOUNT_YEAR1;
                    agreementDetails.TEND_AMOUNT_YEAR2 = details_agreement.TEND_AMOUNT_YEAR2 == null ? null : details_agreement.TEND_AMOUNT_YEAR2;
                    agreementDetails.TEND_AMOUNT_YEAR3 = details_agreement.TEND_AMOUNT_YEAR3 == null ? null : details_agreement.TEND_AMOUNT_YEAR3;
                    agreementDetails.TEND_AMOUNT_YEAR4 = details_agreement.TEND_AMOUNT_YEAR4 == null ? null : details_agreement.TEND_AMOUNT_YEAR4;
                    agreementDetails.TEND_AMOUNT_YEAR5 = details_agreement.TEND_AMOUNT_YEAR5 == null ? null : details_agreement.TEND_AMOUNT_YEAR5;

                    //Added by rohit borse on 01-07-2022
                    agreementDetails.GST_AMT_MAINT_AGREEMENT_DLP = details_agreement.GST_AMT_MAINTAINANCE_AGREEMENT_DLP_NEW;

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementDetails.TEND_AMOUNT_YEAR6 = details_agreement.TEND_AMOUNT_YEAR6_NEW == null ? null : details_agreement.TEND_AMOUNT_YEAR6_NEW;
                        //new change done by Vikram as in scheme 2 when road details are updated no cost were updating
                        agreementDetails.TEND_STATE_SHARE = details_agreement.TEND_STATE_SHARE_NEW == null ? null : details_agreement.TEND_STATE_SHARE_NEW;
                        agreementDetails.TEND_MORD_SHARE = details_agreement.TEND_MORD_SHARE_NEW == null ? null : details_agreement.TEND_MORD_SHARE_NEW;
                        //end of change
                    }
                    agreementDetails.TEND_START_CHAINAGE = details_agreement.TEND_START_CHAINAGE_Existing == null ? null : (details_agreement.TEND_START_CHAINAGE_Existing);
                    agreementDetails.TEND_END_CHAINAGE = details_agreement.TEND_END_CHAINAGE_Existing == null ? null : (details_agreement.TEND_END_CHAINAGE_Existing);

                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementDetails.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(agreementDetails).State = System.Data.Entity.EntityState.Modified;

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

        public bool UpdateAgreementDetailsITNODAL(ExistingAgreementDetails details_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int IMSPRRoadCode = 0;
                string roadName = string.Empty;
                int agreementID = 0;
                int agreementCode = 0;
                Decimal? roadLength = 0;
                TEND_AGREEMENT_DETAIL agreementDetails = null;
                TEND_AGREEMENT_MASTER agreementMaster = null;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                CommonFunctions commonFunction = new CommonFunctions();

                encryptedParameters = details_agreement.EncryptedTendAgreementID_Existing.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString());
                agreementID = Convert.ToInt32(decryptedParameters["TendAgreementID"].ToString());
                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                // roadName = decryptedParameters["IMSRoadName"].ToString().Trim();

                string proposalType = decryptedParameters["ProposalType"].ToString();

                if (proposalType.Equals("P"))
                {
                    roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_ROAD_NAME).FirstOrDefault();
                }
                else
                {
                    roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_BRIDGE_NAME).FirstOrDefault();
                }

                string agreementType = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).Select(am => (string)am.TEND_AGREEMENT_TYPE).FirstOrDefault();


                if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_PART_AGREEMENT == "Y" && ad.TEND_AGREEMENT_STATUS != "W" && ad.TEND_AGREEMENT_ID != agreementID))
                {
                    Int32? imsWorkCode = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_ID == agreementID).Select(ad => (Int32?)ad.IMS_WORK_CODE).FirstOrDefault();

                    if (imsWorkCode > 0)
                    {
                        if (details_agreement.TEND_START_CHAINAGE_Existing != null && details_agreement.TEND_END_CHAINAGE_Existing != null)
                        {
                            if ((details_agreement.TEND_END_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_END_CHAINAGE_Existing) > (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_WORK_CODE == imsWorkCode && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(pw => (Decimal?)pw.IMS_END_CHAINAGE).FirstOrDefault()) || (details_agreement.TEND_START_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_START_CHAINAGE_Existing) < (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_WORK_CODE == imsWorkCode && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(pw => (Decimal?)pw.IMS_START_CHAINAGE).FirstOrDefault()))
                            {
                                message = "Chainage exceeds its road length.";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        roadLength = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_AGREEMENT_STATUS != "W" && ad.TEND_AGREEMENT_ID != agreementID).Sum(td => (Decimal?)(td.TEND_END_CHAINAGE - td.TEND_START_CHAINAGE));
                        roadLength = (roadLength == null ? 0 : roadLength) + ((details_agreement.TEND_END_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_END_CHAINAGE_Existing) - (details_agreement.TEND_START_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_START_CHAINAGE_Existing));
                        if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                        {
                            message = "Chainage exceeds its road length.";
                            return false;
                        }
                    }
                }
                else
                {
                    roadLength += ((details_agreement.TEND_END_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_END_CHAINAGE_Existing) - (details_agreement.TEND_START_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_START_CHAINAGE_Existing));

                    if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                    {
                        message = "Chainage exceeds its road length.";
                        return false;
                    }
                }

                var sanctionProposal = dbContext.IMS_SANCTIONED_PROJECTS.Find(IMSPRRoadCode);
                decimal sanctionCost = sanctionProposal.IMS_PROPOSAL_TYPE == "P" ? (PMGSYSession.Current.PMGSYScheme == 1 ? (sanctionProposal.IMS_SANCTIONED_PAV_AMT + sanctionProposal.IMS_SANCTIONED_PW_AMT + sanctionProposal.IMS_SANCTIONED_OW_AMT + sanctionProposal.IMS_SANCTIONED_CD_AMT + sanctionProposal.IMS_SANCTIONED_RS_AMT) : (sanctionProposal.IMS_SANCTIONED_PAV_AMT + sanctionProposal.IMS_SANCTIONED_PW_AMT + sanctionProposal.IMS_SANCTIONED_OW_AMT + sanctionProposal.IMS_SANCTIONED_CD_AMT + sanctionProposal.IMS_SANCTIONED_RS_AMT + (sanctionProposal.IMS_SANCTIONED_HS_AMT.HasValue ? sanctionProposal.IMS_SANCTIONED_HS_AMT.Value : 0) + (sanctionProposal.IMS_SANCTIONED_FC_AMT.HasValue ? sanctionProposal.IMS_SANCTIONED_FC_AMT.Value : 0))) : (sanctionProposal.IMS_SANCTIONED_BW_AMT + sanctionProposal.IMS_SANCTIONED_BS_AMT);

                if (dbContext.IMS_PROPOSAL_COST_ADD.Any(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                {
                    decimal? costToAdd = dbContext.IMS_PROPOSAL_COST_ADD.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(m => m.IMS_TRANSACTION_CODE).Select(m => m.IMS_STATE_AMOUNT).FirstOrDefault() + dbContext.IMS_PROPOSAL_COST_ADD.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(m => m.IMS_TRANSACTION_CODE).Select(m => m.IMS_MORD_AMOUNT).FirstOrDefault();
                    sanctionCost = sanctionCost + (costToAdd.HasValue ? costToAdd.Value : 0);
                }

                if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47)
                {
                    //if (dbContext.EXEC_PROGRESS.Any(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                    //{
                    //    var finProgress = dbContext.EXEC_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(m => m.EXEC_PROG_YEAR).ThenByDescending(m => m.EXEC_PROG_MONTH).FirstOrDefault();
                    //    decimal? finAmount = (finProgress.EXEC_PAYMENT_LASTMONTH == null ? 0 : finProgress.EXEC_PAYMENT_LASTMONTH) + (finProgress.EXEC_PAYMENT_THISMONTH == null ? 0 : finProgress.EXEC_PAYMENT_THISMONTH);
                    //    if (finAmount > details_agreement.TEND_AGREEMENT_AMOUNT_NEW)
                    //    {
                    //        message = "Agreement amount is less than the amount entered in financial progress.";
                    //        return false;
                    //    }
                    //}
                }
                else if (details_agreement.TEND_AGREEMENT_AMOUNT_NEW > (sanctionCost + (sanctionCost * (decimal)0.2)))
                {
                    message = "Agreement amount should not exceed sanction cost + 20% of sanction cost.";
                    return false;
                }

                using (var scope = new TransactionScope())
                {

                    agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();
                    agreementDetails = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_ID == agreementID).FirstOrDefault();

                    if (agreementMaster == null || agreementDetails == null)
                    {
                        return false;
                    }




                    agreementDetails.TEND_AGREEMENT_AMOUNT = (Decimal)details_agreement.TEND_AGREEMENT_AMOUNT_NEW;
                    agreementDetails.TEND_AMOUNT_YEAR1 = details_agreement.TEND_AMOUNT_YEAR1 == null ? null : details_agreement.TEND_AMOUNT_YEAR1;
                    agreementDetails.TEND_AMOUNT_YEAR2 = details_agreement.TEND_AMOUNT_YEAR2 == null ? null : details_agreement.TEND_AMOUNT_YEAR2;
                    agreementDetails.TEND_AMOUNT_YEAR3 = details_agreement.TEND_AMOUNT_YEAR3 == null ? null : details_agreement.TEND_AMOUNT_YEAR3;
                    agreementDetails.TEND_AMOUNT_YEAR4 = details_agreement.TEND_AMOUNT_YEAR4 == null ? null : details_agreement.TEND_AMOUNT_YEAR4;
                    agreementDetails.TEND_AMOUNT_YEAR5 = details_agreement.TEND_AMOUNT_YEAR5 == null ? null : details_agreement.TEND_AMOUNT_YEAR5;
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementDetails.TEND_AMOUNT_YEAR6 = details_agreement.TEND_AMOUNT_YEAR6_NEW == null ? null : details_agreement.TEND_AMOUNT_YEAR6_NEW;
                        //new change done by Vikram as in scheme 2 when road details are updated no cost were updating
                        agreementDetails.TEND_STATE_SHARE = details_agreement.TEND_STATE_SHARE_NEW == null ? null : details_agreement.TEND_STATE_SHARE_NEW;
                        agreementDetails.TEND_MORD_SHARE = details_agreement.TEND_MORD_SHARE_NEW == null ? null : details_agreement.TEND_MORD_SHARE_NEW;
                        //end of change
                    }
                    agreementDetails.TEND_START_CHAINAGE = details_agreement.TEND_START_CHAINAGE_Existing == null ? null : (details_agreement.TEND_START_CHAINAGE_Existing);
                    agreementDetails.TEND_END_CHAINAGE = details_agreement.TEND_END_CHAINAGE_Existing == null ? null : (details_agreement.TEND_END_CHAINAGE_Existing);

                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementDetails.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(agreementDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    agreementMaster.TEND_AGREEMENT_AMOUNT = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.TEND_AGREEMENT_CODE == agreementCode).Sum(m => m.TEND_AGREEMENT_AMOUNT);
                    agreementMaster.TEND_AMOUNT_YEAR1 = (agreementMaster.TEND_AMOUNT_YEAR1 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR1) + ((details_agreement.TEND_AMOUNT_YEAR1 == null ? 0 : details_agreement.TEND_AMOUNT_YEAR1) - (agreementDetails.TEND_AMOUNT_YEAR1 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR1));
                    agreementMaster.TEND_AMOUNT_YEAR2 = (agreementMaster.TEND_AMOUNT_YEAR2 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR2) + ((details_agreement.TEND_AMOUNT_YEAR2 == null ? 0 : details_agreement.TEND_AMOUNT_YEAR2) - (agreementDetails.TEND_AMOUNT_YEAR2 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR2));
                    agreementMaster.TEND_AMOUNT_YEAR3 = (agreementMaster.TEND_AMOUNT_YEAR3 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR3) + ((details_agreement.TEND_AMOUNT_YEAR3 == null ? 0 : details_agreement.TEND_AMOUNT_YEAR3) - (agreementDetails.TEND_AMOUNT_YEAR3 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR3));
                    agreementMaster.TEND_AMOUNT_YEAR4 = (agreementMaster.TEND_AMOUNT_YEAR4 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR4) + ((details_agreement.TEND_AMOUNT_YEAR4 == null ? 0 : details_agreement.TEND_AMOUNT_YEAR4) - (agreementDetails.TEND_AMOUNT_YEAR4 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR4));
                    agreementMaster.TEND_AMOUNT_YEAR5 = (agreementMaster.TEND_AMOUNT_YEAR5 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR5) + ((details_agreement.TEND_AMOUNT_YEAR5 == null ? 0 : details_agreement.TEND_AMOUNT_YEAR5) - (agreementDetails.TEND_AMOUNT_YEAR5 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR5));
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementMaster.TEND_AMOUNT_YEAR6 = (agreementMaster.TEND_AMOUNT_YEAR6 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR6) + ((details_agreement.TEND_AMOUNT_YEAR6_NEW == null ? 0 : details_agreement.TEND_AMOUNT_YEAR6_NEW) - (agreementDetails.TEND_AMOUNT_YEAR6 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR6));
                        agreementMaster.TEND_STATE_SHARE = (agreementMaster.TEND_STATE_SHARE == null ? null : agreementMaster.TEND_STATE_SHARE) + (details_agreement.TEND_STATE_SHARE_NEW == null ? 0 : details_agreement.TEND_STATE_SHARE_NEW) - (agreementDetails.TEND_STATE_SHARE == null ? 0 : agreementDetails.TEND_STATE_SHARE);
                        agreementMaster.TEND_MORD_SHARE = (agreementMaster.TEND_MORD_SHARE == null ? null : agreementMaster.TEND_MORD_SHARE) + (details_agreement.TEND_MORD_SHARE_NEW == null ? 0 : details_agreement.TEND_MORD_SHARE_NEW) - (agreementDetails.TEND_MORD_SHARE == null ? 0 : agreementDetails.TEND_MORD_SHARE);
                        agreementMaster.TEND_HIGHER_SPEC_AMT = (agreementMaster.TEND_HIGHER_SPEC_AMT == null ? null : agreementMaster.TEND_HIGHER_SPEC_AMT) + (details_agreement.TEND_HIGHER_SPEC_AMT_NEW == null ? 0 : details_agreement.TEND_HIGHER_SPEC_AMT_NEW) - (agreementDetails.TEND_HIGHER_SPEC_AMT == null ? 0 : agreementDetails.TEND_HIGHER_SPEC_AMT);
                    }
                    agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementMaster.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;



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


        public bool DeleteAgreementDetailsDAL_ByAgreementID(int agreementID, int agreementCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                TEND_AGREEMENT_DETAIL agreementDetails = dbContext.TEND_AGREEMENT_DETAIL.Find(agreementID);
                TEND_AGREEMENT_MASTER agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Find(agreementCode);
                if (agreementDetails == null || agreementMaster == null)
                {
                    return false;
                }
                /// Added by SAMMED A. PATIL on 30JUNE2017 to restrict deletion of agreement if entry found in other tables
                if (dbContext.TEND_AGREEMENT_BG_RENEWAL.Any(x => x.TEND_AGREEMENT_CODE == agreementCode))
                {
                    message = "Bank Gurantee Details have been added against this agreement hence cannot Delete agreement";
                    return false;
                }
                if (dbContext.QUALITY_QM_LAB_MASTER.Any(x => x.TEND_AGREEMENT_CODE == agreementCode))
                {
                    message = "Lab Details have been added against this agreement hence cannot Delete agreement";
                    return false;
                }


                using (var scope = new TransactionScope())
                {

                    agreementMaster.TEND_AGREEMENT_AMOUNT -= (Decimal)agreementDetails.TEND_AGREEMENT_AMOUNT;
                    agreementMaster.TEND_AMOUNT_YEAR1 = (agreementMaster.TEND_AMOUNT_YEAR1 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR1) - (agreementDetails.TEND_AMOUNT_YEAR1 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR1);
                    agreementMaster.TEND_AMOUNT_YEAR2 = (agreementMaster.TEND_AMOUNT_YEAR2 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR2) - (agreementDetails.TEND_AMOUNT_YEAR2 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR2);
                    agreementMaster.TEND_AMOUNT_YEAR3 = (agreementMaster.TEND_AMOUNT_YEAR3 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR3) - (agreementDetails.TEND_AMOUNT_YEAR3 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR3);
                    agreementMaster.TEND_AMOUNT_YEAR4 = (agreementMaster.TEND_AMOUNT_YEAR4 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR4) - (agreementDetails.TEND_AMOUNT_YEAR4 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR4);
                    agreementMaster.TEND_AMOUNT_YEAR5 = (agreementMaster.TEND_AMOUNT_YEAR5 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR5) - (agreementDetails.TEND_AMOUNT_YEAR5 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR5);
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementMaster.TEND_AMOUNT_YEAR5 = (agreementMaster.TEND_AMOUNT_YEAR6 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR6) - (agreementDetails.TEND_AMOUNT_YEAR6 == null ? 0 : agreementDetails.TEND_AMOUNT_YEAR6);
                        agreementMaster.TEND_STATE_SHARE = (agreementMaster.TEND_STATE_SHARE == null ? 0 : agreementMaster.TEND_STATE_SHARE) - (agreementDetails.TEND_STATE_SHARE == null ? 0 : agreementDetails.TEND_STATE_SHARE);
                        agreementMaster.TEND_MORD_SHARE = (agreementMaster.TEND_MORD_SHARE == null ? 0 : agreementMaster.TEND_MORD_SHARE) - (agreementDetails.TEND_MORD_SHARE == null ? 0 : agreementDetails.TEND_MORD_SHARE);
                        agreementMaster.TEND_HIGHER_SPEC_AMT = (agreementMaster.TEND_HIGHER_SPEC_AMT == null ? 0 : agreementMaster.TEND_HIGHER_SPEC_AMT) - (agreementDetails.TEND_HIGHER_SPEC_AMT == null ? 0 : agreementDetails.TEND_HIGHER_SPEC_AMT);
                    }
                    agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementMaster.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;

                    agreementDetails.USERID = PMGSYSession.Current.UserId;
                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(agreementDetails).State = System.Data.Entity.EntityState.Modified;


                    dbContext.TEND_AGREEMENT_DETAIL.Remove(agreementDetails);
                    dbContext.SaveChanges();

                    if (!dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.TEND_AGREEMENT_CODE == agreementCode))
                    {
                        agreementMaster.USERID = PMGSYSession.Current.UserId;
                        agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();


                        dbContext.TEND_AGREEMENT_MASTER.Remove(agreementMaster);
                        dbContext.SaveChanges();
                    }

                    scope.Complete();

                    return true;
                }




            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DeleteAgreementDetailsDAL_ByAgreementIDDAL().DbUpdateException");
                message = "You can not delete this agreement details.";
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DeleteAgreementDetailsDAL_ByAgreementIDDAL()");
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
        /// Added by SAMMED A. PATIL on 30JUNE2017 to restrict finalization of agreement if entry found in other tables
        /// </summary>
        /// <param name="agreementCode"></param>
        /// <returns></returns>
        public string ValidateFinalizeAgreementDAL(int agreementCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            string message = string.Empty;
            try
            {
                if (dbContext.TEND_AGREEMENT_BG_RENEWAL.Any(x => x.TEND_AGREEMENT_CODE == agreementCode))
                {
                    message = "Bank Gurantee Details have been added against this agreement hence cannot UnFinalize agreement";
                }
                if (dbContext.QUALITY_QM_LAB_MASTER.Any(x => x.TEND_AGREEMENT_CODE == agreementCode))
                {
                    message = "Lab Details have been added against this agreement hence cannot UnFinalize agreement";
                }
                return message;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ValidateFinalizeAgreementDAL()");
                return "Error occured while validating finalize agreement";
            }
            finally
            {
                if (dbContext == null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool FinalizeAgreementDAL(int agreementCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            TEND_AGREEMENT_MASTER agreementMaster = null;
            try
            {
                using (var scope = new TransactionScope())
                {
                    agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode && am.TEND_IS_AGREEMENT_FINALIZED == "N" && am.TEND_LOCK_STATUS == "N").FirstOrDefault();

                    if (agreementMaster == null)
                    {
                        return false;
                    }

                    agreementMaster.TEND_IS_AGREEMENT_FINALIZED = "Y";
                    agreementMaster.USERID = PMGSYSession.Current.UserId;
                    agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    var roadList = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_STATUS == "P").Select(ad => ad.IMS_PR_ROAD_CODE);

                    foreach (var IMSPRRoadCode in roadList)
                    {
                        if (dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode).Count() == 1)
                        {
                            if (dbContext.IMS_SANCTIONED_PROJECTS.Any(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode && (IMS.IMS_ISCOMPLETED == "M")))
                            {
                                dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).ToList().ForEach(IMS => { IMS.IMS_ISCOMPLETED = "G"; IMS.USERID = PMGSYSession.Current.UserId; IMS.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; }); //&& IMS.IMS_ISCOMPLETED == "W"
                            }
                            dbContext.SaveChanges();
                        }
                    }

                    scope.Complete();
                }

                return true;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeAgreementDAL()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        public bool ChangeAgreementStatusToInCompleteDAL(IncompleteReason incompleteReason, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                TEND_AGREEMENT_MASTER agreementMaster = null;
                CommonFunctions commonFunction = new CommonFunctions();
                int agreementCode = 0;
                int agreementID = 0;
                string agreementType = string.Empty;
                decimal agreementAmount = 0;

                encryptedParameters = incompleteReason.EncryptedTendAgreementCode_IncompleteReason.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString());

                try
                {
                    agreementID = Convert.ToInt32(decryptedParameters["TendAgreementID"].ToString());
                }
                catch
                {
                    agreementID = 0;
                }


                agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();//&& am.TEND_AGREEMENT_STATUS != "W"

                if (agreementMaster == null)
                {
                    return false;
                }

                if (agreementID > 0)
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["EncryptedAgreementType"].ToString()))
                    {
                        encryptedParameters = HttpContext.Current.Session["EncryptedAgreementType"].ToString().Split('/');

                        if (encryptedParameters.Length == 3)
                        {
                            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                            agreementType = decryptedParameters["AgreementType"].ToString();
                        }

                    }

                    agreementAmount = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_ID == agreementID).Select(ad => ad.TEND_AGREEMENT_AMOUNT).FirstOrDefault();

                    //Commented as per directions from Pankaj sir on 19DEC2018
                    //if (incompleteReason.TEND_VALUE_WORK_DONE >= agreementAmount)
                    //{
                    //    message = "Value of Work Done should be less than agreement amount.";
                    //    return false;
                    //}

                    using (var scope = new TransactionScope())
                    {
                        //dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode).ToList().ForEach(ad => ad.TEND_AGREEMENT_STATUS = "W");// && ad.IMS_PR_ROAD_CODE == IMSPRRoadCode

                        //running
                        // dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_ID == agreementID).ToList().ForEach(ad => { ad.TEND_AGREEMENT_STATUS = "W"; ad.TEND_INCOMPLETE_REASON = (incompleteReason.TEND_INCOMPLETE_REASON == null ? null : incompleteReason.TEND_INCOMPLETE_REASON.Trim()); });// && ad.IMS_PR_ROAD_CODE == IMSPRRoadCode

                        //if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_STATUS == "M"))
                        //{
                        //    message = "You can not change work status to Incomplete, because one of the work from agreement in maintenance phase.";
                        //    return false;
                        //}

                        //change for not change status of agreement when one of the work is incomplete
                        //dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_STATUS == "P").ToList().ForEach(ad => { ad.TEND_AGREEMENT_STATUS = "W"; ad.TEND_INCOMPLETE_REASON = (incompleteReason.TEND_INCOMPLETE_REASON == null ? null : incompleteReason.TEND_INCOMPLETE_REASON.Trim()); });


                        //  agreementMaster.TEND_AGREEMENT_STATUS = "W";
                        //  agreementMaster.TEND_INCOMPLETE_REASON = incompleteReason.TEND_INCOMPLETE_REASON == null ? null : incompleteReason.TEND_INCOMPLETE_REASON.Trim();
                        //dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;

                        if (agreementType.Equals("O"))
                        {
                            dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_ID == agreementID).ToList().ForEach(ad => { ad.TEND_AGREEMENT_STATUS = "W"; ad.TEND_INCOMPLETE_REASON = (incompleteReason.TEND_INCOMPLETE_REASON == null ? null : incompleteReason.TEND_INCOMPLETE_REASON.Trim()); ad.TEND_VALUE_WORK_DONE = incompleteReason.TEND_VALUE_WORK_DONE; });
                            agreementMaster.TEND_AGREEMENT_STATUS = "W";
                            agreementMaster.TEND_INCOMPLETE_REASON = incompleteReason.TEND_INCOMPLETE_REASON == null ? null : incompleteReason.TEND_INCOMPLETE_REASON.Trim();
                            agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            agreementMaster.USERID = PMGSYSession.Current.UserId;
                            dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_ID == agreementID /*&& ad.TEND_AGREEMENT_STATUS == "P"*/).ToList().ForEach(ad => { ad.TEND_AGREEMENT_STATUS = "W"; ad.TEND_INCOMPLETE_REASON = (incompleteReason.TEND_INCOMPLETE_REASON == null ? null : incompleteReason.TEND_INCOMPLETE_REASON.Trim()); ad.TEND_VALUE_WORK_DONE = incompleteReason.TEND_VALUE_WORK_DONE; ad.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; ad.USERID = PMGSYSession.Current.UserId; });
                            dbContext.SaveChanges();

                            //addition on 13/08/2013 when one work incomplete then agreement incomplete

                            // working code
                            //if (!dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_STATUS == "P"))
                            //{
                            //    agreementMaster.TEND_AGREEMENT_STATUS = "W";
                            //    //agreementMaster.TEND_INCOMPLETE_REASON = incompleteReason.TEND_INCOMPLETE_REASON == null ? null : incompleteReason.TEND_INCOMPLETE_REASON.Trim();
                            //    dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
                            //}
                            //end working code

                            //written common function to update agreement status so previous code has been commented
                            commonFunction.UpdateAgreementStatus(agreementCode);

                        }

                        dbContext.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                }
                else
                {
                    agreementMaster.TEND_AGREEMENT_STATUS = "W";
                    agreementMaster.TEND_INCOMPLETE_REASON = incompleteReason.TEND_INCOMPLETE_REASON == null ? null : incompleteReason.TEND_INCOMPLETE_REASON.Trim();
                    agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementMaster.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
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

        public bool CheckSplitWorkFinalizedDAL(int IMSPRRoadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                IMS_PROPOSAL_SPLIT proposalSplit = dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();
                //19JUN2019
                var query = (from am in dbContext.TEND_AGREEMENT_MASTER
                             join ad in dbContext.TEND_AGREEMENT_DETAIL on am.TEND_AGREEMENT_CODE equals ad.TEND_AGREEMENT_CODE
                             where ad.IMS_PR_ROAD_CODE == IMSPRRoadCode
                             select ad.IMS_PR_ROAD_CODE).ToList();
                if (query.Count > 0)
                {
                    return true;
                }

                if (proposalSplit != null && proposalSplit.IMS_SPLIT_STATUS.ToUpper().Equals("N"))
                {
                    return false;
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
        public bool CheckforActiveAgreementDAL(int IMSPRRoadCode, string agreementType, ref bool isAgreementAvailable)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            //bool isAgreementAvailable = true;
            try
            {
                int count = 0;
                //var agreementList = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_STATUS == "P" && ad.IMS_PR_ROAD_CODE == IMSPRRoadCode).Distinct();

                var agreementList = from agreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                    join agreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                                    on agreementMaster.TEND_AGREEMENT_CODE equals agreementDetails.TEND_AGREEMENT_CODE
                                    where
                                        //    agreementDetails.TEND_AGREEMENT_STATUS == "P" && //changed condition for when once agreementy completed we can not make another agreement
                                        //agreementDetails.TEND_AGREEMENT_STATUS != "W" && 
                                        ///Changed by SAMMED A. PATIL on 22JAN2018 for cgsurajpur issue unable to add agreement after termination
                                        //   agreementDetails.TEND_AGREEMENT_STATUS != "P" &&
                                    agreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                    agreementMaster.TEND_AGREEMENT_TYPE.ToUpper() == agreementType.ToUpper() &&
                                    agreementMaster.MAST_STATE_CODE == PMGSYSession.Current.StateCode &&
                                    agreementMaster.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                                    orderby agreementDetails.TEND_AGREEMENT_ID descending
                                    select new
                                    {
                                        agreementDetails.TEND_AGREEMENT_ID,
                                        agreementDetails.TEND_AGREEMENT_CODE,
                                        agreementDetails.IMS_PR_ROAD_CODE,
                                        agreementDetails.IMS_WORK_CODE,
                                        agreementMaster.TEND_AGREEMENT_TYPE,
                                        //agreementMaster.TEND_AGREEMENT_STATUS
                                        ///Changed by SAMMED A. PATIL on 08FEB2018 for brpatna issue unable to add agreement on agreement terminate
                                        agreementDetails.TEND_AGREEMENT_STATUS
                                    };

                // agreementList = agreementList.GroupBy(al => al.TEND_AGREEMENT_CODE).Select(al => al.FirstOrDefault());


                //    if (agreementList.Count() > 0)
                //   {// Following change is made as per suggestion of Srinivasa Sir on 11 Aug 2020.

                DateTime? sanctionedDate = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_SANCTIONED_DATE).FirstOrDefault();
                string SanctionedDate = sanctionedDate == null ? null : Convert.ToDateTime(sanctionedDate).ToString("dd/MM/yyyy");


               //  Not awarded  Or Terminated  or un-freezed -> Allow   agreement

                // Else  Not allow agreemnt.


                //-------------------------------------------------------------------------------




                //var IMS_FREEZE_STATUS = (from s in dbContext.IMS_SANCTIONED_PROJECTS
                //                        where s.IMS_PR_ROAD_CODE == IMSPRRoadCode
                //                        select new
                //                        {
                //                            s.IMS_FREEZE_STATUS
                //                        }).FirstOrDefault();


                string FREEZE_STATUS = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(c => c.IMS_FREEZE_STATUS).FirstOrDefault();
                      

               // string FREEZE_STATUS= IMS_FREEZE_STATUS.ToString();

                


                 if (PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 2 )
                 {
                     foreach (var item1 in agreementList)
                     {
                         if (item1.TEND_AGREEMENT_ID > 0)
                         {
                             return true;
                         }

                     }

                     // added by saurabh on 01-01-2022
                     var RoadFinaneYear = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode && (IMS.MAST_PMGSY_SCHEME == 1 ||
                                         IMS.MAST_PMGSY_SCHEME == 2)).Select(IMS => IMS.IMS_YEAR).FirstOrDefault();
                     var Road_Award_Status = dbContext.TEND_AGREEMENT_DETAIL.Where(s => s.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(s => s.TEND_AGREEMENT_ID).FirstOrDefault();
                     var Road_Stat_Award = Road_Award_Status;
                     if (RoadFinaneYear >= 2020 && Road_Stat_Award > 0)
                     {
                         Road_Stat_Award = 1;
                     }
                     else
                     {
                         Road_Stat_Award = 0;
                     }

                     //if (String.Compare(FREEZE_STATUS, "U") == 0)
                     if (FREEZE_STATUS.Equals("")|| FREEZE_STATUS.Equals("U"))
                     {
                         return true;
                     }
                     else if (Road_Stat_Award == 0)
                     {
                         return false;
                     }
                     else
                     {
                         return true;
                     }
                     
                 }
                 else 
                 {
                     return true;

                 }





                //if (agreementList.Count() > 0)
                //{
                //    if (dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).Any())
                //    {
                //        count = dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(ps => ps.IMS_TOTAL_SPLIT).FirstOrDefault();

                //        if (count > 0)
                //        {
                //            return false;
                //        }

                //    }


                //    foreach (var item1 in agreementList)
                //    {// Hide Add Agreement Details button ONLY in case when TEND_AGREEMENT_STATUS="P"
                //        if (item1.TEND_AGREEMENT_STATUS == "P")
                //        {
                //            return true;
                //        }
                //    }

                //    foreach (var item1 in agreementList)
                //    {
                //        if (item1.TEND_AGREEMENT_STATUS == "C" || (item1.TEND_AGREEMENT_STATUS == "W" && PMGSYSession.Current.PMGSYScheme == 4))
                //        {
                //            return false;
                //        }
                //    }
                //    foreach (var item1 in agreementList)
                //    {
                //        if (item1.TEND_AGREEMENT_STATUS == "W" && sanctionedDate <= new DateTime(2020, 03, 31) && PMGSYSession.Current.PMGSYScheme != 4)
                //        {
                //            return true;
                //        }
                //    }

                //    foreach (var item1 in agreementList)
                //    {
                //        if (item1.IMS_WORK_CODE == null)
                //        {
                //            return false;
                //        }
                //    }

                //    count = dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(ps => ps.IMS_TOTAL_SPLIT).FirstOrDefault();

                //    if (count == agreementList.Count())
                //    {
                //        return false;
                //    }
                //}
                //else if (agreementList.Count() == 0 && PMGSYSession.Current.PMGSYScheme != 4 && (sanctionedDate == null || sanctionedDate <= new DateTime(2020, 03, 31)))
                //{
                //    Hide Add Agreement Details button when no agreement exists and date is either null or less than 31 March 2020.Changes instructed by Srinivasa Sir on 25-05-2021
                //    isAgreementAvailable = false;
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                //return false;


                //----------------------------------------------


                //foreach (var agreementDetails in agreementList)
                //{
                //    ///Changed by SAMMED A. PATIL on 20JUNE2017 to allow add agreement details after agreement status is completed
                //    if (agreementDetails.TEND_AGREEMENT_STATUS == "C" || agreementDetails.TEND_AGREEMENT_STATUS == "W")
                //    {
                //        return false;
                //    }
                //    if (agreementDetails.IMS_WORK_CODE == null)
                //    {
                //        return true;
                //    }
                //    else
                //    {
                //        count = dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(ps => ps.IMS_TOTAL_SPLIT).FirstOrDefault();

                //        if (count == agreementList.Count())
                //        {
                //            return true;
                //        }

                //    }

                //}
                // }


                //    return true;

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


        public List<SelectListItem> PopulateContractorsByPan(int stateCode, string panSearch, string conSupFlag)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> lstContDetails = new List<SelectListItem>();
            try
            {
                List<SelectListItem> lstContractorWithPAN = new List<SelectListItem>();

                conSupFlag = (conSupFlag == "S" ? "S" : "C");

                var lstContractor = (from contractor in dbContext.MASTER_CONTRACTOR
                                     join contractorReg in dbContext.MASTER_CONTRACTOR_REGISTRATION
                                     on contractor.MAST_CON_ID equals contractorReg.MAST_CON_ID
                                     where
                                     ((stateCode == 0 ? 1 : contractorReg.MAST_REG_STATE) == (stateCode == 0 ? 1 : stateCode)) &&
                                     contractorReg.MAST_REG_STATUS == "A" &&
                                     contractor.MAST_CON_STATUS == "A"
                                     // &&  contractor.MAST_CON_SUP_FLAG == conSupFlag
                                     && ((panSearch == "" ? "%" : contractor.MAST_CON_PAN).Contains(panSearch == "" ? "%" : panSearch))
                                     select new
                                     {
                                         MAST_CON_NAME = (contractor.MAST_CON_FNAME == null ? string.Empty : contractor.MAST_CON_FNAME) + " " + (contractor.MAST_CON_MNAME == null ? string.Empty : contractor.MAST_CON_MNAME) + " " + (contractor.MAST_CON_LNAME == null ? string.Empty : contractor.MAST_CON_LNAME) + " (Pan-" + (contractor.MAST_CON_PAN == null ? string.Empty : contractor.MAST_CON_PAN) + ")",//+"(ID-"+item.MAST_CON_ID+")",
                                         MAST_CON_ID = contractor.MAST_CON_ID
                                     }).ToList();

                lstContractorWithPAN = new SelectList(lstContractor.ToList(), "MAST_CON_ID", "MAST_CON_NAME").ToList();

                lstContractorWithPAN.Insert(0, new SelectListItem { Text = "Select Contractor/Supplier", Value = "0" });

                return lstContractorWithPAN;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region Agreement without Road
        public Array GetAgreementDetailsListDAL_WithoutRoad(int agreementYear, string status, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                var query = from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                            join contractorDetails in dbContext.MASTER_CONTRACTOR
                            on tendAgreementMaster.MAST_CON_ID equals contractorDetails.MAST_CON_ID into contractors
                            from contractorDetails in contractors.DefaultIfEmpty()
                            where
                            tendAgreementMaster.MAST_STATE_CODE == stateCode &&
                            tendAgreementMaster.MAST_DISTRICT_CODE == districtCode &&
                            (agreementYear == 0 ? 1 : tendAgreementMaster.TEND_DATE_OF_AGREEMENT.Year) == (agreementYear == 0 ? 1 : agreementYear) &&
                            (status == "0" ? "%" : tendAgreementMaster.TEND_AGREEMENT_STATUS.ToUpper()) == (status == "0" ? "%" : status.ToUpper()) &&
                            (agreementType == string.Empty ? "%" : tendAgreementMaster.TEND_AGREEMENT_TYPE) == (agreementType == string.Empty ? "%" : agreementType)
                                // && tendAgreementMaster.TEND_AGREEMENT_STATUS != "W"
                            && tendAgreementMaster.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme // new change done by Vikram in order to list the details depending upon the scheme
                            select new
                            {

                                tendAgreementMaster.TEND_AGREEMENT_CODE,
                                contractorDetails.MAST_CON_COMPANY_NAME,
                                tendAgreementMaster.TEND_AGREEMENT_TYPE,
                                tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                                tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                                tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                                tendAgreementMaster.TEND_AMOUNT_YEAR1,
                                tendAgreementMaster.TEND_AMOUNT_YEAR2,
                                tendAgreementMaster.TEND_AMOUNT_YEAR3,
                                tendAgreementMaster.TEND_AMOUNT_YEAR4,
                                tendAgreementMaster.TEND_AMOUNT_YEAR5,
                                tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                                tendAgreementMaster.TEND_LOCK_STATUS,
                                tendAgreementMaster.TEND_AGREEMENT_STATUS
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
                            case "AgreementType":
                                query = query.OrderBy(x => x.TEND_AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderBy(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "AgreementType":
                                query = query.OrderByDescending(x => x.TEND_AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderByDescending(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(tendAgreementMaster => new
                {

                    tendAgreementMaster.TEND_AGREEMENT_CODE,
                    tendAgreementMaster.MAST_CON_COMPANY_NAME,
                    tendAgreementMaster.TEND_AGREEMENT_TYPE,
                    tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                    tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                    tendAgreementMaster.TEND_AMOUNT_YEAR1,
                    tendAgreementMaster.TEND_AMOUNT_YEAR2,
                    tendAgreementMaster.TEND_AMOUNT_YEAR3,
                    tendAgreementMaster.TEND_AMOUNT_YEAR4,
                    tendAgreementMaster.TEND_AMOUNT_YEAR5,
                    tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                    tendAgreementMaster.TEND_LOCK_STATUS,
                    tendAgreementMaster.TEND_AGREEMENT_STATUS

                }).ToArray();


                return result.Select(tendAgreementMaster => new
                {
                    //id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {                                                                               
                                                                  
                                    tendAgreementMaster.TEND_AGREEMENT_NUMBER.ToString(),
                                    tendAgreementMaster.MAST_CON_COMPANY_NAME==null?"NA":tendAgreementMaster.MAST_CON_COMPANY_NAME.ToString().Trim(),
                                    AgreementTypes[tendAgreementMaster.TEND_AGREEMENT_TYPE].ToString(), 
                                    Convert.ToDateTime(tendAgreementMaster.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),
                                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT.ToString(),      
                                    ((tendAgreementMaster.TEND_AMOUNT_YEAR1==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR1)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR2==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR2)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR3==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR3)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR4==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR4)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR5==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR5)
                                    ).ToString(),
                                    AgreementStatus[tendAgreementMaster.TEND_AGREEMENT_STATUS].ToString(),
                                    (tendAgreementMaster.TEND_AGREEMENT_STATUS =="C" ||  tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="N" || tendAgreementMaster.TEND_LOCK_STATUS=="Y" )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    (tendAgreementMaster.TEND_AGREEMENT_STATUS =="W")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="N" && tendAgreementMaster.TEND_LOCK_STATUS=="N" && tendAgreementMaster.TEND_AGREEMENT_STATUS !="W") ?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='FinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Finalized'></span></td></tr></table></center>",
                                    (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y" && !dbContext.ACC_BILL_DETAILS.Any(m=>m.IMS_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE)) ?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='DeFinalize Agreement' onClick ='DeFinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='DeFinalized'></span></td></tr></table></center>",
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementMaster.TEND_LOCK_STATUS=="Y" || tendAgreementMaster.TEND_AGREEMENT_STATUS =="W" || tendAgreementMaster.TEND_AGREEMENT_STATUS =="C")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementMaster.TEND_LOCK_STATUS=="Y" || tendAgreementMaster.TEND_AGREEMENT_STATUS =="W" || tendAgreementMaster.TEND_AGREEMENT_STATUS =="C")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()})
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


        public bool SaveAgreementDetailsDAL_WithoutRoad(AgreementDetails details_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string agreementType = string.Empty;
                TEND_AGREEMENT_MASTER agreementMaster = null;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                CommonFunctions commonFunction = new CommonFunctions();

                encryptedParameters = details_agreement.EncryptedAgreementType_Add.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                agreementType = decryptedParameters["AgreementType"].ToString().Trim();


                if (dbContext.TEND_AGREEMENT_MASTER.Any(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.TEND_AGREEMENT_NUMBER.ToUpper() == details_agreement.TEND_AGREEMENT_NUMBER.ToUpper()))
                {
                    message = "Agreement Number is already exist.";
                    return false;
                }
                //  Int32 recordCount = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.MAST_CON_ID == details_agreement.MAST_CON_ID && am.TEND_AGREEMENT_NUMBER.ToUpper() == details_agreement.TEND_AGREEMENT_NUMBER.ToUpper()).Count();
                //else if (dbContext.TEND_AGREEMENT_MASTER.Any(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.MAST_CON_ID == details_agreement.MAST_CON_ID && am.TEND_AGREEMENT_NUMBER.ToUpper() == details_agreement.TEND_AGREEMENT_NUMBER.ToUpper()))
                //{
                //    message = "Agreement Number for selected contractor is already exist.";
                //    return false;
                //}



                agreementMaster = new TEND_AGREEMENT_MASTER();
                agreementMaster.TEND_AGREEMENT_CODE = (Int32)GetMaxCode(AgreementModules.AgreementMaster);
                agreementMaster.MAST_STATE_CODE = stateCode;
                agreementMaster.MAST_DISTRICT_CODE = districtCode;
                agreementMaster.MAST_CON_ID = details_agreement.MAST_CON_ID == 0 ? null : (Int32?)details_agreement.MAST_CON_ID;
                // agreementMaster.TEND_TENDER_AMOUNT = details_agreement.TEND_TENDER_AMOUNT == null ? null : details_agreement.TEND_TENDER_AMOUNT;
                agreementMaster.TEND_AGREEMENT_NUMBER = details_agreement.TEND_AGREEMENT_NUMBER;
                agreementMaster.TEND_DATE_OF_AGREEMENT = commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_AGREEMENT);
                agreementMaster.TEND_AGREEMENT_START_DATE = commonFunction.GetStringToDateTime(details_agreement.TEND_AGREEMENT_START_DATE);
                agreementMaster.TEND_AGREEMENT_END_DATE = commonFunction.GetStringToDateTime(details_agreement.TEND_AGREEMENT_END_DATE);
                agreementMaster.TEND_AGREEMENT_AMOUNT = (Decimal)details_agreement.TEND_AGREEMENT_AMOUNT;
                agreementMaster.TEND_IS_AGREEMENT_FINALIZED = "N";
                agreementMaster.TEND_AGREEMENT_TYPE = agreementType;//details_agreement.AgreementType == true ? "C" : "O";
                agreementMaster.TEND_DATE_OF_AWARD_WORK = details_agreement.TEND_DATE_OF_AWARD_WORK == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_AWARD_WORK);
                agreementMaster.TEND_DATE_OF_WORK_ORDER = details_agreement.TEND_DATE_OF_WORK_ORDER == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_WORK_ORDER);
                agreementMaster.TEND_DATE_OF_COMMENCEMENT = details_agreement.TEND_DATE_OF_COMMENCEMENT == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_COMMENCEMENT);
                agreementMaster.TEND_DATE_OF_COMPLETION = details_agreement.TEND_DATE_OF_COMPLETION == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_COMPLETION);

                //agreementMaster.TEND_AMOUNT_YEAR1 = details_agreement.TEND_AMOUNT_YEAR1 == null ? null : details_agreement.TEND_AMOUNT_YEAR1;
                //agreementMaster.TEND_AMOUNT_YEAR2 = details_agreement.TEND_AMOUNT_YEAR2 == null ? null : details_agreement.TEND_AMOUNT_YEAR2;
                //agreementMaster.TEND_AMOUNT_YEAR3 = details_agreement.TEND_AMOUNT_YEAR3 == null ? null : details_agreement.TEND_AMOUNT_YEAR3;
                //agreementMaster.TEND_AMOUNT_YEAR4 = details_agreement.TEND_AMOUNT_YEAR4 == null ? null : details_agreement.TEND_AMOUNT_YEAR4;
                //agreementMaster.TEND_AMOUNT_YEAR5 = details_agreement.TEND_AMOUNT_YEAR5 == null ? null : details_agreement.TEND_AMOUNT_YEAR5;



                agreementMaster.TEND_AGREEMENT_REMARKS = details_agreement.TEND_AGREEMENT_REMARKS == null ? null : details_agreement.TEND_AGREEMENT_REMARKS.Trim();

                agreementMaster.TEND_AGREEMENT_STATUS = "P";
                agreementMaster.TEND_LOCK_STATUS = "N";
                agreementMaster.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme; // new change done by Vikram on 10 Feb 2014
                agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                agreementMaster.USERID = PMGSYSession.Current.UserId;
                dbContext.TEND_AGREEMENT_MASTER.Add(agreementMaster);
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


        public bool UpdateAgreementMasterDetailsDAL_WithoutRoad(AgreementDetails details_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int agreementCode = 0;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                CommonFunctions commonFunction = new CommonFunctions();

                encryptedParameters = details_agreement.EncryptedTendAgreementCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString());


                if (dbContext.TEND_AGREEMENT_MASTER.Any(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.TEND_AGREEMENT_NUMBER.ToUpper() == details_agreement.TEND_AGREEMENT_NUMBER.ToUpper() && am.TEND_AGREEMENT_CODE != agreementCode))
                {
                    message = "Agreement Number is already exist.";
                    return false;
                }


                TEND_AGREEMENT_MASTER agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                if (agreementMaster == null)
                {
                    return false;
                }

                // agreementMaster.TEND_TENDER_AMOUNT = details_agreement.TEND_TENDER_AMOUNT == null ? null : details_agreement.TEND_TENDER_AMOUNT;
                agreementMaster.TEND_AGREEMENT_NUMBER = details_agreement.TEND_AGREEMENT_NUMBER;
                agreementMaster.TEND_DATE_OF_AGREEMENT = commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_AGREEMENT);
                agreementMaster.TEND_AGREEMENT_START_DATE = commonFunction.GetStringToDateTime(details_agreement.TEND_AGREEMENT_START_DATE);
                agreementMaster.TEND_AGREEMENT_END_DATE = commonFunction.GetStringToDateTime(details_agreement.TEND_AGREEMENT_END_DATE);
                agreementMaster.TEND_AGREEMENT_AMOUNT = (Decimal)details_agreement.TEND_AGREEMENT_AMOUNT;
                agreementMaster.TEND_DATE_OF_AWARD_WORK = details_agreement.TEND_DATE_OF_AWARD_WORK == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_AWARD_WORK);
                agreementMaster.TEND_DATE_OF_WORK_ORDER = details_agreement.TEND_DATE_OF_WORK_ORDER == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_WORK_ORDER);
                agreementMaster.TEND_DATE_OF_COMMENCEMENT = details_agreement.TEND_DATE_OF_COMMENCEMENT == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_COMMENCEMENT);
                agreementMaster.TEND_DATE_OF_COMPLETION = details_agreement.TEND_DATE_OF_COMPLETION == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_COMPLETION);

                //agreementMaster.TEND_AMOUNT_YEAR1 = details_agreement.TEND_AMOUNT_YEAR1 == null ? null : details_agreement.TEND_AMOUNT_YEAR1;
                //agreementMaster.TEND_AMOUNT_YEAR2 = details_agreement.TEND_AMOUNT_YEAR2 == null ? null : details_agreement.TEND_AMOUNT_YEAR2;
                //agreementMaster.TEND_AMOUNT_YEAR3 = details_agreement.TEND_AMOUNT_YEAR3 == null ? null : details_agreement.TEND_AMOUNT_YEAR3;
                //agreementMaster.TEND_AMOUNT_YEAR4 = details_agreement.TEND_AMOUNT_YEAR4 == null ? null : details_agreement.TEND_AMOUNT_YEAR4;
                //agreementMaster.TEND_AMOUNT_YEAR5 = details_agreement.TEND_AMOUNT_YEAR5 == null ? null : details_agreement.TEND_AMOUNT_YEAR5;
                agreementMaster.TEND_AGREEMENT_REMARKS = details_agreement.TEND_AGREEMENT_REMARKS == null ? null : details_agreement.TEND_AGREEMENT_REMARKS.Trim();
                agreementMaster.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme; //new change done by Vikram on 10 Feb 2014
                agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                agreementMaster.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
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

        public List<IMS_PROPOSAL_WORK> GetProposalWorks(string IMSPRRoadCode, string encryptedAgreementType, bool isSearch, bool isRegularAgreement, bool isEdit = false)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int imsPRRoadCode = 0;
            string agreementType = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(IMSPRRoadCode))
                {
                    encryptedParameters = IMSPRRoadCode.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        imsPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                    }

                }
                if (!string.IsNullOrEmpty(encryptedAgreementType))
                {
                    if (encryptedAgreementType.Equals("C"))
                    {
                        agreementType = encryptedAgreementType;
                    }
                    else
                    {
                        encryptedParameters = null;
                        encryptedParameters = encryptedAgreementType.Split('/');

                        if (encryptedParameters.Length == 3)
                        {
                            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                            agreementType = decryptedParameters["AgreementType"].ToString();
                        }
                    }

                }


                List<IMS_PROPOSAL_WORK> lstProposalWork = (from proposalWork in dbContext.IMS_PROPOSAL_WORK
                                                           join imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                                                         on proposalWork.IMS_PR_ROAD_CODE equals imsSanctionedProjectDetails.IMS_PR_ROAD_CODE
                                                           where
                                                           imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
                                                           imsSanctionedProjectDetails.IMS_DPR_STATUS == "N" &&
                                                           proposalWork.IMS_PR_ROAD_CODE == imsPRRoadCode
                                                           select proposalWork).ToList<IMS_PROPOSAL_WORK>();

                if (isRegularAgreement)
                {
                    //var existingWorkCodes = (from agreementDetails in dbContext.TEND_AGREEMENT_DETAIL where agreementDetails.IMS_PR_ROAD_CODE == imsPRRoadCode && agreementDetails.TEND_AGREEMENT_STATUS != "W" select new { agreementDetails.IMS_WORK_CODE }).Distinct();

                    var existingWorkCodes = (from agreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                                             join agreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                             on agreementDetails.TEND_AGREEMENT_CODE equals agreementMaster.TEND_AGREEMENT_CODE
                                             where
                                             agreementDetails.IMS_PR_ROAD_CODE == imsPRRoadCode &&
                                             agreementDetails.TEND_AGREEMENT_STATUS != "W" &&
                                             agreementMaster.TEND_AGREEMENT_TYPE.ToUpper() == agreementType.ToUpper()
                                             select new { agreementDetails.IMS_WORK_CODE }).Distinct();

                    lstProposalWork = (from workList in lstProposalWork
                                       where !existingWorkCodes.Any(workCode => workCode.IMS_WORK_CODE == workList.IMS_WORK_CODE)
                                       select workList).ToList<IMS_PROPOSAL_WORK>();
                }
                else
                {
                    if (!isEdit)
                    {
                        var existingWorkCodes = (from IMSContract in dbContext.MANE_IMS_CONTRACT where IMSContract.IMS_PR_ROAD_CODE == imsPRRoadCode && IMSContract.MANE_CONTRACT_STATUS == "P" select new { IMSContract.IMS_WORK_CODE }).Distinct();

                        lstProposalWork = (from workList in lstProposalWork
                                           where !existingWorkCodes.Any(workCode => workCode.IMS_WORK_CODE == workList.IMS_WORK_CODE)
                                           select workList).ToList<IMS_PROPOSAL_WORK>();
                    }
                }

                if (!isSearch)
                {
                    lstProposalWork.Insert(0, new IMS_PROPOSAL_WORK() { IMS_WORK_CODE = 0, IMS_WORK_DESC = "Select Work" });
                }

                return lstProposalWork;
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


        public SelectList GetProposalTypes()
        {
            List<ProposalTypes> proposalTypeList = new List<ProposalTypes>();

            foreach (var item in ProposalTypes.lstProposalTypes)
            {
                proposalTypeList.Add(new ProposalTypes() { TypeID = item.Key, ProposalType = item.Value });
            }

            return new SelectList(proposalTypeList, "TypeID", "ProposalType");
        }

        public SelectList GetAgreementStatusList()
        {
            List<AgreementStatus_Search> statusList = new List<AgreementStatus_Search>();

            foreach (var item in AgreementStatus_Search.lstStatus)
            {
                statusList.Add(new AgreementStatus_Search() { StatusID = item.Key, Status = item.Value });
            }

            return new SelectList(statusList, "StatusID", "Status");
        }

        public bool ChangeAgreementStatusToCompleteDAL(int agreementCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            TEND_AGREEMENT_MASTER agreementMaster = null;
            string agreementType = string.Empty;

            try
            {
                using (var scope = new TransactionScope())
                {
                    agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode && am.TEND_AGREEMENT_STATUS != "C" && am.TEND_LOCK_STATUS == "N").FirstOrDefault();

                    if (agreementMaster == null)
                    {
                        return false;
                    }

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["EncryptedAgreementType"].ToString()))
                    {
                        encryptedParameters = HttpContext.Current.Session["EncryptedAgreementType"].ToString().Split('/');

                        if (encryptedParameters.Length == 3)
                        {
                            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                            agreementType = decryptedParameters["AgreementType"].ToString();
                        }

                    }

                    ///Commented by SAMMED A. PATIL on 03 OCTOBER 2017 to display column changestatustocomplete for all agreement types
                    //if (agreementType.Equals("O"))
                    {
                        dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode).ToList().ForEach(ad => { ad.TEND_AGREEMENT_STATUS = "C"; ad.TEND_VALUE_WORK_DONE = null; ad.TEND_INCOMPLETE_REASON = null; });
                    }

                    agreementMaster.TEND_AGREEMENT_STATUS = "C";
                    agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementMaster.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
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

        #endregion Agreement without Road

        #region PROPOSAL_RELATED_DETAILS

        public Array GetProposalAgreementListDAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var query = (from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                             join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                             on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                             join imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                             on tendAgreementDetails.IMS_PR_ROAD_CODE equals imsSanctionedProjectDetails.IMS_PR_ROAD_CODE
                             join splitWork in dbContext.IMS_PROPOSAL_WORK
                             on imsSanctionedProjectDetails.IMS_PR_ROAD_CODE equals splitWork.IMS_PR_ROAD_CODE into split
                             from splitDetails in split.DefaultIfEmpty()
                             join contractorDetails in dbContext.MASTER_CONTRACTOR
                             on tendAgreementMaster.MAST_CON_ID equals contractorDetails.MAST_CON_ID into contractors
                             from contractorDetails in contractors.DefaultIfEmpty()

                             where
                             tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                             (agreementType == string.Empty ? "%" : tendAgreementMaster.TEND_AGREEMENT_TYPE) == (agreementType == string.Empty ? "%" : agreementType)
                             select new
                             {
                                 tendAgreementDetails.TEND_AGREEMENT_ID,
                                 tendAgreementMaster.TEND_AGREEMENT_CODE,
                                 imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                                 imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                 contractorDetails.MAST_CON_COMPANY_NAME,
                                 tendAgreementMaster.TEND_AGREEMENT_TYPE,
                                 tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                                 tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                                 tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR1,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR2,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR3,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR4,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR5,
                                 tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                                 tendAgreementMaster.TEND_LOCK_STATUS,
                                 tendAgreementMaster.TEND_AGREEMENT_STATUS,
                                 splitDetails.IMS_WORK_DESC
                             });


                query = query.GroupBy(tm => tm.TEND_AGREEMENT_CODE).Select(tm => tm.FirstOrDefault());

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
                            case "AgreementType":
                                query = query.OrderBy(x => x.TEND_AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderBy(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "AgreementType":
                                query = query.OrderByDescending(x => x.TEND_AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderByDescending(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(tendAgreementMaster => new
                {
                    tendAgreementMaster.TEND_AGREEMENT_ID,
                    tendAgreementMaster.TEND_AGREEMENT_CODE,
                    tendAgreementMaster.IMS_PR_ROAD_CODE,
                    tendAgreementMaster.IMS_ROAD_NAME,
                    tendAgreementMaster.MAST_CON_COMPANY_NAME,
                    tendAgreementMaster.TEND_AGREEMENT_TYPE,
                    tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                    tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                    tendAgreementMaster.TEND_AMOUNT_YEAR1,
                    tendAgreementMaster.TEND_AMOUNT_YEAR2,
                    tendAgreementMaster.TEND_AMOUNT_YEAR3,
                    tendAgreementMaster.TEND_AMOUNT_YEAR4,
                    tendAgreementMaster.TEND_AMOUNT_YEAR5,
                    tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                    tendAgreementMaster.TEND_LOCK_STATUS,
                    tendAgreementMaster.TEND_AGREEMENT_STATUS,
                    tendAgreementMaster.IMS_WORK_DESC
                }).ToArray();


                return result.Select(tendAgreementMaster => new
                {
                    id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {                                                                               
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    tendAgreementMaster.TEND_AGREEMENT_NUMBER.ToString(),
                                    tendAgreementMaster.IMS_WORK_DESC == null?"-":tendAgreementMaster.IMS_WORK_DESC.ToString(),
                                    tendAgreementMaster.MAST_CON_COMPANY_NAME==null?"NA":tendAgreementMaster.MAST_CON_COMPANY_NAME.ToString().Trim(),
                                    AgreementTypes[tendAgreementMaster.TEND_AGREEMENT_TYPE].ToString(), 
                                    Convert.ToDateTime(tendAgreementMaster.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),
                                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT.ToString(),      
                                    ((tendAgreementMaster.TEND_AMOUNT_YEAR1==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR1)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR2==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR2)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR3==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR3)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR4==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR4)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR5==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR5)
                                    ).ToString(),
                                     AgreementStatus[tendAgreementMaster.TEND_AGREEMENT_STATUS].ToString(),
                                    // (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="N" && tendAgreementMaster.TEND_LOCK_STATUS=="N" && tendAgreementMaster.TEND_AGREEMENT_STATUS!="W") ?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='FinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Finalized'></span></td></tr></table></center>",
                                     
                                    //  URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }),

                                    // (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementMaster.TEND_LOCK_STATUS=="Y" || tendAgreementMaster.TEND_AGREEMENT_STATUS=="W")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }),
                                    //(tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementMaster.TEND_LOCK_STATUS=="Y" || tendAgreementMaster.TEND_AGREEMENT_STATUS=="W")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() })
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

        public short GetSharePercent(string encryptedCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                String[] parameters = encryptedCode.Split('/');
                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameters[0], parameters[1], parameters[2] });
                int IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                short sharePercent = Convert.ToInt16(dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(m => m.IMS_SHARE_PERCENT).FirstOrDefault());
                return sharePercent;
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #region FINALIZE_AGREEMENT

        public Array GetAgreementListDAL(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 0;

                var lstAgreements = dbContext.USP_GET_AGREEMENTS(PMGSYSession.Current.DistrictCode, (blockCode <= 0 ? 0 : blockCode), (yearCode <= 0 ? 0 : yearCode), (package == "0" ? "%" : (package == "All" ? "%" : package)), PMGSYSession.Current.AdminNdCode, (agreementStatus == "0" ? "%" : agreementStatus), (finalize == "0" ? "%" : finalize), (proposalType == "0" ? "%" : (proposalType == "All" ? "%" : proposalType)), agreementType, PMGSYSession.Current.PMGSYScheme).ToList();

                totalRecords = lstAgreements.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                lstAgreements = lstAgreements.OrderBy(x => x.CONTRACTOR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementType":
                                lstAgreements = lstAgreements.OrderBy(x => x.AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementNumber":
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementDate":
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_AGREEMENT_NUMBER).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.CONTRACTOR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementType":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementNumber":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementDate":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }

                    }
                }
                else
                {
                    lstAgreements = lstAgreements.OrderBy(x => x.CONTRACTOR_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstAgreements.Select(tendAgreementMaster => new
                {
                    tendAgreementMaster.TEND_AGREEMENT_CODE,
                    tendAgreementMaster.CONTRACTOR_NAME,
                    tendAgreementMaster.AGREEMENT_TYPE,
                    tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                    tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                    tendAgreementMaster.MAINTENANCE_AMOUNT,
                    tendAgreementMaster.AGREEMENT_STATUS,
                    tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                    tendAgreementMaster.TEND_LOCK_STATUS,
                    tendAgreementMaster.TEND_AGREEMENT_STATUS,
                    tendAgreementMaster.TEND_AGREEMENT_TYPE,
                }).ToArray();

                return result.Select(tendAgreementMaster => new
                {
                    id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {                                                                               
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    tendAgreementMaster.TEND_AGREEMENT_NUMBER.ToString(),
                                    tendAgreementMaster.CONTRACTOR_NAME==null?"NA":tendAgreementMaster.CONTRACTOR_NAME.ToString().Trim(),
                                    tendAgreementMaster.AGREEMENT_TYPE.ToString(), 
                                    Convert.ToDateTime(tendAgreementMaster.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),
                                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT.ToString(),      
                                    tendAgreementMaster.MAINTENANCE_AMOUNT.ToString(),
                                    tendAgreementMaster.AGREEMENT_STATUS.ToString(),
                                    (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="N" && tendAgreementMaster.TEND_LOCK_STATUS=="N" && tendAgreementMaster.TEND_AGREEMENT_STATUS!="W") ?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='FinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Finalized'></span></td></tr></table></center>",
                                    (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y" ? 
                                    (tendAgreementMaster.TEND_AGREEMENT_TYPE == "C" ? 
                                    ((dbContext.EXEC_LSB_MONTHLY_STATUS.Where(m=> dbContext.TEND_AGREEMENT_DETAIL.Where(l=>l.TEND_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE).Select(l=>l.IMS_PR_ROAD_CODE).Contains(m.IMS_PR_ROAD_CODE)).Any() 
                                    || dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(m=> dbContext.TEND_AGREEMENT_DETAIL.Where(r=>r.TEND_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE).Select(r=>r.IMS_PR_ROAD_CODE).Contains(m.IMS_PR_ROAD_CODE)).Any() 
                                    || dbContext.EXEC_WORK_PROGRAM.Where(m=> dbContext.TEND_AGREEMENT_DETAIL.Where(w=>w.TEND_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE).Select(w=>w.IMS_PR_ROAD_CODE).Contains(m.IMS_PR_ROAD_CODE)).Any() 
                                    || dbContext.EXEC_PAYMENT_SCHEDULE.Where(m=> dbContext.TEND_AGREEMENT_DETAIL.Where(p=>p.TEND_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE).Select(p=>p.IMS_PR_ROAD_CODE).Contains(m.IMS_PR_ROAD_CODE)).Any() 
                                    || dbContext.ACC_BILL_DETAILS.Any(m=>m.IMS_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE && m.ACC_BILL_MASTER.FUND_TYPE == "P") || dbContext.EXEC_OFFICER_DETAILS.Where(m=> dbContext.TEND_AGREEMENT_DETAIL.Where(w=>w.TEND_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE).Select(w=>w.IMS_PR_ROAD_CODE).Contains(m.IMS_PR_ROAD_CODE)).Any() 
                                    || dbContext.EXEC_FILES.Where(m=> dbContext.TEND_AGREEMENT_DETAIL.Where(w=>w.TEND_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE).Select(w=>w.IMS_PR_ROAD_CODE).Contains(m.IMS_PR_ROAD_CODE)).Any()
                                    || dbContext.EXEC_CDWORKS.Where(m=> dbContext.TEND_AGREEMENT_DETAIL.Where(w=>w.TEND_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE).Select(w=>w.IMS_PR_ROAD_CODE).Contains(m.IMS_PR_ROAD_CODE)).Any()
                                    || dbContext.EXEC_PROGRESS.Where(m=> dbContext.TEND_AGREEMENT_DETAIL.Where(w=>w.TEND_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE).Select(w=>w.IMS_PR_ROAD_CODE).Contains(m.IMS_PR_ROAD_CODE)).Any()
                                    || dbContext.QUALITY_QM_LAB_MASTER.Any(m=>m.TEND_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE)
                                    ) ? "<span>-</span>" : (dbContext.ACC_BILL_DETAILS.Any(m=>m.IMS_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE && m.ACC_BILL_MASTER.FUND_TYPE == "P") || dbContext.EXEC_OFFICER_DETAILS.Where(m=> dbContext.TEND_AGREEMENT_DETAIL.Where(w=>w.TEND_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE).Select(w=>w.IMS_PR_ROAD_CODE).Contains(m.IMS_PR_ROAD_CODE)).Any() ?  "<span>-</span>" : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='DeFinalize Agreement' onClick ='DeFinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) + "\");' ></span></td></tr></table></center>") ): "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='DeFinalize Agreement' onClick ='DeFinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) + "\");' ></span></td></tr></table></center>") : "<span>-</span>"),
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =0","TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =0"  }),
                                    //tendAgreementMaster.TEND_AGREEMENT_STATUS == "W" ? "<a href='#' title='Click here to view request details' class='ui-icon ui-icon-locked ui-align-center' onClick=ChangeAgreementStatus('" + URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) +"'); return false;'>Change</a>" : "<span class='ui-icon ui-icon-unlocked ui-align-center'></span>",
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


        public bool DeFinalizeAgreementDAL(int agreementCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            TEND_AGREEMENT_MASTER agreementMaster = null;
            try
            {
                using (var scope = new TransactionScope())
                {
                    agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                    if (agreementMaster == null)
                    {
                        return false;
                    }

                    agreementMaster.TEND_IS_AGREEMENT_FINALIZED = "N";

                    dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    // discuss with agreement details flag
                    var roadList = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_STATUS == "P").Select(ad => ad.IMS_PR_ROAD_CODE);

                    foreach (var IMSPRRoadCode in roadList)
                    {
                        if (dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode).Count() == 1)
                        {
                            if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode && m.IMS_ISCOMPLETED == "G"))
                            {
                                dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).ToList().ForEach(IMS => { IMS.IMS_ISCOMPLETED = "M"; IMS.USERID = PMGSYSession.Current.UserId; IMS.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; }); //&& IMS.IMS_ISCOMPLETED == "W"
                            }
                            dbContext.SaveChanges();
                        }
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


        #endregion

        #region SPECIAL_AGREEMENT

        /// <summary>
        /// returns the list of proposals for adding the special agreement
        /// </summary>
        /// <param name="isSplitWork"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="blockCode"></param>
        /// <param name="sanctionedYear"></param>
        /// <param name="packageID"></param>
        /// <param name="proposalType"></param>
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
        public Array GetSpecialAgreementProposedRoadListDAL(bool isSplitWork, int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, string proposalType, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (packageID.Contains("All"))
                {
                    packageID = "All Packages";
                }

                var query = from imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
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
                            ((PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47) ? 0 : imsSanctionedProjectDetails.MAST_DPIU_CODE) == ((PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47) ? 0 : adminNDCode) &&
                            imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
                            imsSanctionedProjectDetails.IMS_DPR_STATUS == "N" &&
                            (sanctionedYear == 0 ? 1 : imsSanctionedProjectDetails.IMS_YEAR) == (sanctionedYear == 0 ? 1 : sanctionedYear) &&
                            (blockCode <= 0 ? 1 : imsSanctionedProjectDetails.MAST_BLOCK_CODE) == (blockCode <= 0 ? 1 : blockCode) &&
                            (packageID == "All Packages" ? "%" : imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper()) == (packageID == "All Packages" ? "%" : packageID.ToUpper()) &&
                            (proposalType == "0" ? "%" : imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE.ToUpper()) == (proposalType == "0" ? "%" : proposalType.ToUpper()) &&
                            (batch == 0 ? 1 : imsSanctionedProjectDetails.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
                            (collaboration <= 0 ? 1 : imsSanctionedProjectDetails.IMS_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                            (upgradationType == "0" ? "%" : imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT) == (upgradationType == "0" ? "%" : upgradationType) &&
                            imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme // to list the details according to the Scheme in Session
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
                                imsSanctionedProjectDetails.IMS_BRIDGE_NAME,
                                imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH,
                                imsSanctionedProjectDetails.IMS_SANCTIONED_DATE,
                                imsSanctionedProjectDetails.IMS_BATCH,
                                imsSanctionedProjectDetails.MASTER_BLOCK.MAST_BLOCK_NAME

                            };

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
                            case "WorkType":
                                query = query.OrderBy(x => x.IMS_PROPOSAL_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "Batch":
                                query = query.OrderBy(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Block":
                                query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_PROPOSAL_TYPE).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "WorkType":
                                query = query.OrderByDescending(x => x.IMS_PROPOSAL_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "Batch":
                                query = query.OrderByDescending(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Block":
                                query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_PROPOSAL_TYPE).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                    imsSanctionedProjectDetails.IMS_BRIDGE_NAME,
                    imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_DATE,
                    imsSanctionedProjectDetails.IMS_BATCH,
                    imsSanctionedProjectDetails.MAST_BLOCK_NAME
                }).ToArray();


                return result.Select(imsSanctionedProjectDetails => new
                {

                    cell = new[] {                                                                               
                                    imsSanctionedProjectDetails.MAST_BLOCK_NAME == null?"-":imsSanctionedProjectDetails.MAST_BLOCK_NAME.ToString(),
                                    imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString() ,
                                    imsSanctionedProjectDetails.IMS_BATCH == null?"NA":"Batch "+ imsSanctionedProjectDetails.IMS_BATCH.ToString(),
                                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,                
                                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"?imsSanctionedProjectDetails.IMS_ROAD_NAME.ToString():(imsSanctionedProjectDetails.IMS_BRIDGE_NAME==null?"NA":imsSanctionedProjectDetails.IMS_BRIDGE_NAME.ToString()),
                                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"?"Road":"Bridge",
                                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"?imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString():(imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH==null?"NA":imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH.ToString()),//imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString(),
                                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME==null?"NA":imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME.Trim(),                                                              
                                    
                                    //imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? 
                                    //(imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT+
                                    //imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT).ToString() : (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT).ToString(),    

                                    ///Change made by SAMMED PATIL on 29MAR2016 
                                    PMGSYSession.Current.PMGSYScheme == 1 ? 
                                                      ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT)).ToString()
                                                    : ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT)).ToString(),

                                    //dbContext.TEND_AGREEMENT_MASTER.Any(a=>a.TEND_AGREEMENT_CODE == dbContext.TEND_AGREEMENT_DETAIL.Where(m=>m.IMS_PR_ROAD_CODE == imsSanctionedProjectDetails.IMS_PR_ROAD_CODE).Select(m=>m.TEND_AGREEMENT_CODE).FirstOrDefault() && a.TEND_AGREEMENT_TYPE == "R") ? dbContext.TEND_AGREEMENT_MASTER.Where(a=>a.TEND_AGREEMENT_CODE == dbContext.TEND_AGREEMENT_DETAIL.Where(m=>m.IMS_PR_ROAD_CODE == imsSanctionedProjectDetails.IMS_PR_ROAD_CODE).Select(m=>m.TEND_AGREEMENT_CODE).FirstOrDefault() && a.TEND_AGREEMENT_TYPE == "R").Select(a=>a.TEND_AGREEMENT_AMOUNT).FirstOrDefault().ToString() : "-",
                                    (imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5).ToString(),                                    
                                    isSplitWork==false?(URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID, "ProposalType="+ imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE, "RoadLength="+(imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--"):(imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH==null?"NA": imsSanctionedProjectDetails.IMS_BRIDGE_LENGTH.ToString().Replace(".","--"))), "SanctionedDate=" + (imsSanctionedProjectDetails.IMS_SANCTIONED_DATE==null? string.Empty: Convert.ToDateTime(imsSanctionedProjectDetails.IMS_SANCTIONED_DATE).ToString("dd/MM/yyyy").Replace("/","--"))   })  )   : ( URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID, "ProposalType="+ imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE, "RoadLength="+ imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString().Replace(".","--")   }) )        
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
        /// saves the special agreement details
        /// </summary>
        /// <param name="details_agreement"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SaveSpecialAgreementDetailsDAL(SpecialAgreementDetails details_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int IMSPRRoadCode = 0;
                string roadName = string.Empty;
                Decimal? roadLength = 0;
                string agreementType = string.Empty;
                TEND_AGREEMENT_MASTER agreementMaster = null;
                TEND_AGREEMENT_DETAIL agreementDetails = null;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                CommonFunctions commonFunction = new CommonFunctions();

                encryptedParameters = details_agreement.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());

                string proposalType = decryptedParameters["ProposalType"].ToString();

                if (proposalType.Equals("P"))
                {
                    roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_ROAD_NAME).FirstOrDefault();
                }
                else
                {
                    roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_BRIDGE_NAME).FirstOrDefault();
                }


                encryptedParameters = null;
                encryptedParameters = details_agreement.EncryptedAgreementType_Add.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                agreementType = decryptedParameters["AgreementType"].ToString().Trim();


                int count = 0;
                var agreementList = from agreementDetailsList in dbContext.TEND_AGREEMENT_DETAIL
                                    join agreementMasterList in dbContext.TEND_AGREEMENT_MASTER
                                    on agreementDetailsList.TEND_AGREEMENT_CODE equals agreementMasterList.TEND_AGREEMENT_CODE
                                    where
                                    agreementDetailsList.TEND_AGREEMENT_STATUS != "W" &&
                                    agreementDetailsList.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                    agreementMasterList.TEND_AGREEMENT_TYPE.ToUpper() == agreementType.ToUpper() &&
                                    agreementMasterList.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                                    select new
                                    {
                                        agreementDetailsList.TEND_AGREEMENT_ID,
                                        agreementDetailsList.TEND_AGREEMENT_CODE,
                                        agreementDetailsList.IMS_PR_ROAD_CODE,
                                        agreementDetailsList.IMS_WORK_CODE,
                                        agreementMasterList.TEND_AGREEMENT_TYPE

                                    };

                //if (agreementList.Count() > 0)
                //{
                //    foreach (var agreementDetail in agreementList)
                //    {

                //        if (agreementDetail.IMS_WORK_CODE == null)
                //        {
                //            message = "Agreement Details against road '" + roadName + "' is already exist.";
                //            return false;
                //        }
                //        else
                //        {
                //            count = dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(ps => ps.IMS_TOTAL_SPLIT).FirstOrDefault();

                //            if (agreementDetail.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE)
                //            {
                //                message = "Agreement Details against road '" + roadName + "' and selected work is already exist.";
                //                return false;
                //            }

                //            if (count == agreementList.Count())
                //            {
                //                message = "Agreement Details against road '" + roadName + "' is already exist.";
                //                return false;
                //            }

                //        }

                //    }
                //}


                if (agreementType.Equals("C"))
                {
                    if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_PART_AGREEMENT == "Y" && ad.TEND_AGREEMENT_STATUS != "W"))
                    {
                        roadLength = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_AGREEMENT_STATUS != "W").Sum(td => (Decimal?)(td.TEND_END_CHAINAGE - td.TEND_START_CHAINAGE));
                        roadLength = (roadLength == null ? 0 : roadLength) + ((details_agreement.TEND_END_CHAINAGE == null ? 0 : details_agreement.TEND_END_CHAINAGE) - (details_agreement.TEND_START_CHAINAGE == null ? 0 : details_agreement.TEND_START_CHAINAGE));
                        if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                        {
                            message = "Chainage exceeds its road length.";
                            return false;
                        }
                    }
                }

                if (agreementType.Equals("C"))
                {
                    if (details_agreement.IMS_WORK_CODE > 0)
                    {

                        if (details_agreement.TEND_START_CHAINAGE != null && details_agreement.TEND_END_CHAINAGE != null)
                        {
                            if ((details_agreement.TEND_END_CHAINAGE == null ? 0 : details_agreement.TEND_END_CHAINAGE) > (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(pw => (Decimal?)pw.IMS_END_CHAINAGE).FirstOrDefault()) || (details_agreement.TEND_START_CHAINAGE == null ? 0 : details_agreement.TEND_START_CHAINAGE) < (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(pw => (Decimal?)pw.IMS_START_CHAINAGE).FirstOrDefault()))
                            {
                                message = "Chainage must be between work chainage.";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        // if (details_agreement.IsPartAgreement)
                        //  {
                        roadLength += ((details_agreement.TEND_END_CHAINAGE == null ? 0 : details_agreement.TEND_END_CHAINAGE) - (details_agreement.TEND_START_CHAINAGE == null ? 0 : details_agreement.TEND_START_CHAINAGE));

                        if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                        {
                            message = "Chainage exceeds its road length.";
                            return false;
                        }

                        //}

                    }
                }


                //if (dbContext.TEND_AGREEMENT_MASTER.Any(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.TEND_AGREEMENT_NUMBER.ToUpper() == details_agreement.TEND_AGREEMENT_NUMBER.ToUpper() && am.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme))
                //{
                //    message = "Agreement Number is already exist.";
                //    return false;
                //}

                //Check agreement number present in Maintenance Agreement

                var IMSContractList = (from IMSContracts in dbContext.MANE_IMS_CONTRACT
                                       join IMSSanctioned in dbContext.IMS_SANCTIONED_PROJECTS
                                       on IMSContracts.IMS_PR_ROAD_CODE equals IMSSanctioned.IMS_PR_ROAD_CODE
                                       where
                                       IMSSanctioned.MAST_STATE_CODE == stateCode &&
                                       IMSSanctioned.MAST_DISTRICT_CODE == districtCode &&
                                       IMSContracts.MANE_AGREEMENT_NUMBER.ToUpper() == details_agreement.TEND_AGREEMENT_NUMBER.ToUpper()
                                       select new
                                       {

                                           IMSContracts.MANE_AGREEMENT_NUMBER,
                                           IMSContracts.MAST_CON_ID,
                                           IMSContracts.IMS_PR_ROAD_CODE,
                                           IMSSanctioned.MAST_STATE_CODE,
                                           IMSSanctioned.MAST_DISTRICT_CODE

                                       });


                //if (IMSContractList.Count() > 0)
                //{
                //    message = "Agreement Number is already exist for maintenance agreement.";
                //    return false;
                //}

                using (var scope = new TransactionScope())
                {
                    agreementMaster = new TEND_AGREEMENT_MASTER();

                    agreementMaster.TEND_AGREEMENT_CODE = (Int32)GetMaxCode(AgreementModules.AgreementMaster);
                    agreementMaster.MAST_STATE_CODE = stateCode;
                    agreementMaster.MAST_DISTRICT_CODE = districtCode;
                    agreementMaster.MAST_CON_ID = details_agreement.MAST_CON_ID == 0 ? null : (Int32?)details_agreement.MAST_CON_ID;
                    agreementMaster.TEND_TENDER_AMOUNT = details_agreement.TEND_TENDER_AMOUNT == null ? null : details_agreement.TEND_TENDER_AMOUNT;
                    agreementMaster.TEND_AGREEMENT_NUMBER = details_agreement.TEND_AGREEMENT_NUMBER;
                    agreementMaster.TEND_DATE_OF_AGREEMENT = commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_AGREEMENT);
                    agreementMaster.TEND_AGREEMENT_START_DATE = commonFunction.GetStringToDateTime(details_agreement.TEND_AGREEMENT_START_DATE);
                    agreementMaster.TEND_AGREEMENT_END_DATE = commonFunction.GetStringToDateTime(details_agreement.TEND_AGREEMENT_END_DATE);
                    agreementMaster.TEND_AGREEMENT_AMOUNT = (Decimal)details_agreement.TEND_AGREEMENT_AMOUNT;
                    agreementMaster.TEND_IS_AGREEMENT_FINALIZED = "N";
                    agreementMaster.TEND_AGREEMENT_TYPE = agreementType;
                    agreementMaster.TEND_DATE_OF_AWARD_WORK = details_agreement.TEND_DATE_OF_AWARD_WORK == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_AWARD_WORK);
                    agreementMaster.TEND_DATE_OF_WORK_ORDER = details_agreement.TEND_DATE_OF_WORK_ORDER == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_WORK_ORDER);
                    agreementMaster.TEND_DATE_OF_COMMENCEMENT = details_agreement.TEND_DATE_OF_COMMENCEMENT == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_COMMENCEMENT);
                    agreementMaster.TEND_DATE_OF_COMPLETION = details_agreement.TEND_DATE_OF_COMPLETION == null ? null : (DateTime?)commonFunction.GetStringToDateTime(details_agreement.TEND_DATE_OF_COMPLETION);

                    // Added By Rohit Borse on 01-07-2022
                    agreementMaster.GST_AMT_MAINT_AGREEMENT = details_agreement.GST_AMT_MAINTAINANCE_AGREEMENT;
                    agreementMaster.APS_COLLECTED = details_agreement.APS_COLLECTED;
                    agreementMaster.APS_COLLECTED_AMOUNT = details_agreement.APS_COLLECTED_AMOUNT;

                    agreementMaster.TEND_AMOUNT_YEAR1 = null;
                    agreementMaster.TEND_AMOUNT_YEAR2 = null;
                    agreementMaster.TEND_AMOUNT_YEAR3 = null;
                    agreementMaster.TEND_AMOUNT_YEAR4 = null;
                    agreementMaster.TEND_AMOUNT_YEAR5 = null;
                    agreementMaster.TEND_AMOUNT_YEAR6 = null;

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementMaster.TEND_HIGHER_SPEC_AMT = details_agreement.TEND_HIGHER_SPEC_AMT == null ? null : details_agreement.TEND_HIGHER_SPEC_AMT;
                        agreementMaster.TEND_STATE_SHARE = details_agreement.TEND_HIGHER_SPEC_AMT == null ? null : details_agreement.TEND_STATE_SHARE;
                        agreementMaster.TEND_MORD_SHARE = details_agreement.TEND_HIGHER_SPEC_AMT == null ? null : details_agreement.TEND_MORD_SHARE;
                    }

                    agreementMaster.TEND_AGREEMENT_REMARKS = details_agreement.TEND_AGREEMENT_REMARKS == null ? null : details_agreement.TEND_AGREEMENT_REMARKS.Trim();

                    agreementMaster.TEND_LOCK_STATUS = "N";
                    agreementMaster.TEND_AGREEMENT_STATUS = "P";
                    agreementMaster.USERID = PMGSYSession.Current.UserId;
                    agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementMaster.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme; //new change done by Vikram on 10 Feb 2014
                    dbContext.TEND_AGREEMENT_MASTER.Add(agreementMaster);
                    // dbContext.SaveChanges();

                    agreementDetails = new TEND_AGREEMENT_DETAIL();
                    agreementDetails.TEND_AGREEMENT_ID = (Int32)GetMaxCode(AgreementModules.AgreementDetails);
                    agreementDetails.TEND_AGREEMENT_CODE = agreementMaster.TEND_AGREEMENT_CODE;
                    agreementDetails.IMS_PR_ROAD_CODE = IMSPRRoadCode;
                    agreementDetails.TEND_AGREEMENT_AMOUNT = (Decimal)details_agreement.TEND_AGREEMENT_AMOUNT;

                    // Added By Rohit Borse on 01-07-2022
                    agreementDetails.GST_AMT_MAINT_AGREEMENT_DLP = details_agreement.GST_AMT_MAINTAINANCE_AGREEMENT_DLP;

                    agreementDetails.TEND_AMOUNT_YEAR1 = null;
                    agreementDetails.TEND_AMOUNT_YEAR2 = null;
                    agreementDetails.TEND_AMOUNT_YEAR3 = null;
                    agreementDetails.TEND_AMOUNT_YEAR4 = null;
                    agreementDetails.TEND_AMOUNT_YEAR5 = null;
                    agreementDetails.TEND_AMOUNT_YEAR6 = null;
                    agreementDetails.TEND_START_CHAINAGE = details_agreement.TEND_START_CHAINAGE == null ? null : (details_agreement.TEND_START_CHAINAGE);
                    agreementDetails.TEND_END_CHAINAGE = details_agreement.TEND_END_CHAINAGE == null ? null : (details_agreement.TEND_END_CHAINAGE);
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementDetails.TEND_HIGHER_SPEC_AMT = details_agreement.TEND_HIGHER_SPEC_AMT == null ? null : details_agreement.TEND_HIGHER_SPEC_AMT;
                        agreementDetails.TEND_STATE_SHARE = details_agreement.TEND_HIGHER_SPEC_AMT == null ? null : details_agreement.TEND_STATE_SHARE;
                        agreementDetails.TEND_MORD_SHARE = details_agreement.TEND_HIGHER_SPEC_AMT == null ? null : details_agreement.TEND_MORD_SHARE;
                    }

                    if (details_agreement.IMS_WORK_CODE > 0)
                    {
                        agreementDetails.TEND_PART_AGREEMENT = "Y";
                        agreementDetails.IMS_WORK_CODE = details_agreement.IMS_WORK_CODE;

                        if (dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE).Any())
                        {
                            agreementDetails.TEND_FIRST_AGREEMENT = "N";
                        }
                        else
                        {
                            agreementDetails.TEND_FIRST_AGREEMENT = "Y";
                        }
                    }
                    else
                    {
                        agreementDetails.TEND_PART_AGREEMENT = "N";

                        if (dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode).Any())
                        {
                            agreementDetails.TEND_FIRST_AGREEMENT = "N";
                        }
                        else
                        {
                            agreementDetails.TEND_FIRST_AGREEMENT = "Y";
                        }

                    }
                    agreementDetails.TEND_AGREEMENT_STATUS = "P";
                    agreementDetails.TEND_INCLUDE_ROAD_AMT = "Y";
                    agreementDetails.USERID = PMGSYSession.Current.UserId;
                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.TEND_AGREEMENT_DETAIL.Add(agreementDetails);
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

        /// <summary>
        /// updates the details of Special agreement details
        /// </summary>
        /// <param name="master_agreement"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UpdateSpecialAgreementMasterDetailsDAL(SpecialAgreementDetails master_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                int agreementCode = 0;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;
                CommonFunctions commonFunction = new CommonFunctions();
                encryptedParameters = master_agreement.EncryptedTendAgreementCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString());

                //if (dbContext.TEND_AGREEMENT_MASTER.Any(am => am.MAST_STATE_CODE == stateCode && am.MAST_DISTRICT_CODE == districtCode && am.TEND_AGREEMENT_NUMBER.ToUpper() == master_agreement.TEND_AGREEMENT_NUMBER.ToUpper() && am.TEND_AGREEMENT_CODE != agreementCode))
                //{
                //    message = "Agreement Number is already exist.";
                //    return false;
                //}

                var IMSContractList = (from IMSContracts in dbContext.MANE_IMS_CONTRACT
                                       join IMSSanctioned in dbContext.IMS_SANCTIONED_PROJECTS
                                       on IMSContracts.IMS_PR_ROAD_CODE equals IMSSanctioned.IMS_PR_ROAD_CODE
                                       where
                                       IMSSanctioned.MAST_STATE_CODE == stateCode &&
                                       IMSSanctioned.MAST_DISTRICT_CODE == districtCode &&
                                       IMSContracts.MANE_AGREEMENT_NUMBER.ToUpper() == master_agreement.TEND_AGREEMENT_NUMBER.ToUpper()
                                       select new
                                       {

                                           IMSContracts.MANE_AGREEMENT_NUMBER,
                                           IMSContracts.MAST_CON_ID,
                                           IMSContracts.IMS_PR_ROAD_CODE,
                                           IMSSanctioned.MAST_STATE_CODE,
                                           IMSSanctioned.MAST_DISTRICT_CODE

                                       });


                //if (IMSContractList.Count() > 0)
                //{
                //    message = "Agreement Number is already exist for maintenance agreement.";
                //    return false;
                //}

                TEND_AGREEMENT_MASTER agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();
                
                TEND_AGREEMENT_DETAIL agreementDetail = dbContext.TEND_AGREEMENT_DETAIL.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                if (agreementMaster == null)
                {
                    return false;
                }

                agreementMaster.TEND_TENDER_AMOUNT = master_agreement.TEND_TENDER_AMOUNT == null ? null : master_agreement.TEND_TENDER_AMOUNT;
                agreementMaster.TEND_AGREEMENT_NUMBER = master_agreement.TEND_AGREEMENT_NUMBER;
                agreementMaster.TEND_DATE_OF_AGREEMENT = commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_AGREEMENT);
                agreementMaster.TEND_AGREEMENT_START_DATE = commonFunction.GetStringToDateTime(master_agreement.TEND_AGREEMENT_START_DATE);
                agreementMaster.TEND_AGREEMENT_END_DATE = commonFunction.GetStringToDateTime(master_agreement.TEND_AGREEMENT_END_DATE);
                agreementMaster.TEND_AGREEMENT_AMOUNT = master_agreement.TEND_AGREEMENT_AMOUNT == null ? 0 : master_agreement.TEND_AGREEMENT_AMOUNT.Value;
                agreementMaster.TEND_DATE_OF_AWARD_WORK = master_agreement.TEND_DATE_OF_AWARD_WORK == null ? null : (DateTime?)commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_AWARD_WORK);
                agreementMaster.TEND_DATE_OF_WORK_ORDER = master_agreement.TEND_DATE_OF_WORK_ORDER == null ? null : (DateTime?)commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_WORK_ORDER);
                agreementMaster.TEND_DATE_OF_COMMENCEMENT = master_agreement.TEND_DATE_OF_COMMENCEMENT == null ? null : (DateTime?)commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_COMMENCEMENT);
                agreementMaster.TEND_DATE_OF_COMPLETION = master_agreement.TEND_DATE_OF_COMPLETION == null ? null : (DateTime?)commonFunction.GetStringToDateTime(master_agreement.TEND_DATE_OF_COMPLETION);

                // added by rohit  borse on 01-07-2022
                agreementMaster.APS_COLLECTED = master_agreement.APS_COLLECTED;
                agreementMaster.APS_COLLECTED_AMOUNT = master_agreement.APS_COLLECTED_AMOUNT;
                agreementMaster.GST_AMT_MAINT_AGREEMENT = master_agreement.GST_AMT_MAINTAINANCE_AGREEMENT;

                

                if (PMGSYSession.Current.PMGSYScheme == 2)
                {                                      
                    agreementMaster.TEND_HIGHER_SPEC_AMT = master_agreement.TEND_HIGHER_SPEC_AMT;
                    agreementMaster.TEND_MORD_SHARE = master_agreement.TEND_MORD_SHARE;
                    agreementMaster.TEND_STATE_SHARE = master_agreement.TEND_STATE_SHARE;
                }

                agreementMaster.TEND_AGREEMENT_REMARKS = master_agreement.TEND_AGREEMENT_REMARKS == null ? null : master_agreement.TEND_AGREEMENT_REMARKS.Trim();
                agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                agreementMaster.USERID = PMGSYSession.Current.UserId;
                agreementMaster.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;
                dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                // added by rohit  borse on 01-07-2022
                agreementDetail.TEND_AGREEMENT_AMOUNT = Convert.ToDecimal(master_agreement.TEND_AGREEMENT_AMOUNT);
                agreementDetail.GST_AMT_MAINT_AGREEMENT_DLP = master_agreement.GST_AMT_MAINTAINANCE_AGREEMENT_DLP;
                                
                dbContext.Entry(agreementDetail).State = System.Data.Entity.EntityState.Modified;
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

        /// <summary>
        /// returns the list of special agreement list for the particular proposal
        /// </summary>
        /// <param name="IMSPRRoadCode"></param>
        /// <param name="agreementType"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetSpecialAgreementDetailsListDAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var query = (from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                             join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                             on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                             join imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                             on tendAgreementDetails.IMS_PR_ROAD_CODE equals imsSanctionedProjectDetails.IMS_PR_ROAD_CODE
                             join contractorDetails in dbContext.MASTER_CONTRACTOR
                             on tendAgreementMaster.MAST_CON_ID equals contractorDetails.MAST_CON_ID into contractors
                             from contractorDetails in contractors.DefaultIfEmpty()
                             where
                             tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                             (agreementType == string.Empty ? "%" : tendAgreementMaster.TEND_AGREEMENT_TYPE) == (agreementType == string.Empty ? "%" : agreementType)
                             &&
                             tendAgreementMaster.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme // to list the details according to the Scheme in Session
                             select new
                             {
                                 tendAgreementDetails.TEND_AGREEMENT_ID,
                                 tendAgreementMaster.TEND_AGREEMENT_CODE,
                                 imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                                 imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                 MAST_CON_COMPANY_NAME = contractorDetails.MAST_CON_COMPANY_NAME + (contractorDetails.MAST_CON_PAN != "" ? " (" + contractorDetails.MAST_CON_PAN + ")" : ""),
                                 tendAgreementMaster.TEND_AGREEMENT_TYPE,
                                 tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                                 tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                                 tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR1,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR2,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR3,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR4,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR5,
                                 tendAgreementMaster.TEND_AMOUNT_YEAR6,
                                 tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                                 tendAgreementMaster.TEND_LOCK_STATUS,
                                 tendAgreementMaster.TEND_AGREEMENT_STATUS,
                                 tendAgreementMaster.MAST_PMGSY_SCHEME,

                                 //adedd on 15-07-2022
                                 tendAgreementDetails.GST_AMT_MAINT_AGREEMENT_DLP,
                             });


                query = query.GroupBy(tm => tm.TEND_AGREEMENT_CODE).Select(tm => tm.FirstOrDefault());

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
                            case "AgreementType":
                                query = query.OrderBy(x => x.TEND_AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderBy(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "AgreementType":
                                query = query.OrderByDescending(x => x.TEND_AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderByDescending(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(tendAgreementMaster => new
                {
                    tendAgreementMaster.TEND_AGREEMENT_ID,
                    tendAgreementMaster.TEND_AGREEMENT_CODE,
                    tendAgreementMaster.IMS_PR_ROAD_CODE,
                    tendAgreementMaster.IMS_ROAD_NAME,
                    tendAgreementMaster.MAST_CON_COMPANY_NAME,
                    tendAgreementMaster.TEND_AGREEMENT_TYPE,
                    tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                    tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                    tendAgreementMaster.TEND_AMOUNT_YEAR1,
                    tendAgreementMaster.TEND_AMOUNT_YEAR2,
                    tendAgreementMaster.TEND_AMOUNT_YEAR3,
                    tendAgreementMaster.TEND_AMOUNT_YEAR4,
                    tendAgreementMaster.TEND_AMOUNT_YEAR5,
                    tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                    tendAgreementMaster.TEND_LOCK_STATUS,
                    tendAgreementMaster.TEND_AGREEMENT_STATUS,
                    tendAgreementMaster.TEND_AMOUNT_YEAR6,

                    // added on 15-07-2022
                    tendAgreementMaster.GST_AMT_MAINT_AGREEMENT_DLP
                }).ToArray();


                return result.Select(tendAgreementMaster => new
                {
                    id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {                                                                               
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    tendAgreementMaster.TEND_AGREEMENT_NUMBER.ToString(),
                                    tendAgreementMaster.MAST_CON_COMPANY_NAME==null?"NA":tendAgreementMaster.MAST_CON_COMPANY_NAME.ToString().Trim(),
                                    AgreementTypes[tendAgreementMaster.TEND_AGREEMENT_TYPE].ToString(), 
                                    Convert.ToDateTime(tendAgreementMaster.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),
                                     // Agreement Amount with GST Amt on 15-07-2022
                                    (tendAgreementMaster.TEND_AGREEMENT_AMOUNT + tendAgreementMaster.GST_AMT_MAINT_AGREEMENT_DLP).ToString(),

                                    ((tendAgreementMaster.TEND_AMOUNT_YEAR1==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR1)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR2==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR2)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR3==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR3)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR4==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR4)+
                                       (tendAgreementMaster.TEND_AMOUNT_YEAR5==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR5) + (PMGSYSession.Current.PMGSYScheme == 2?(tendAgreementMaster.TEND_AMOUNT_YEAR6==null?0:tendAgreementMaster.TEND_AMOUNT_YEAR6):0)
                                    ).ToString(),
                                     AgreementStatus[tendAgreementMaster.TEND_AGREEMENT_STATUS].ToString(),
                                     (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="N" && tendAgreementMaster.TEND_LOCK_STATUS=="N" && tendAgreementMaster.TEND_AGREEMENT_STATUS!="W") ?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='FinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Finalized'></span></td></tr></table></center>",
                                      URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }),
                                     (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementMaster.TEND_LOCK_STATUS=="Y" || tendAgreementMaster.TEND_AGREEMENT_STATUS=="W")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() }),
                                    (tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementMaster.TEND_LOCK_STATUS=="Y" || tendAgreementMaster.TEND_AGREEMENT_STATUS=="W")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementMaster.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementMaster.IMS_PR_ROAD_CODE.ToString() })
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
        /// returns the list of agreement details according to the particular agreement number
        /// </summary>
        /// <param name="agreementCode"></param>
        /// <param name="IMSPRRoadCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetSpecialAgreementDetailsListDAL_ByAgreementCode(int agreementCode, int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int agreementID = 0;
            long records = 0;
            bool isRegularAgreement = true;
            try
            {

                var query = from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                            join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                            on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                            join imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                            on tendAgreementDetails.IMS_PR_ROAD_CODE equals imsSanctionedProjectDetails.IMS_PR_ROAD_CODE
                            join proposalWorks in dbContext.IMS_PROPOSAL_WORK
                            on tendAgreementDetails.IMS_WORK_CODE equals proposalWorks.IMS_WORK_CODE into works
                            from proposalWorks in works.DefaultIfEmpty()

                            where
                            tendAgreementDetails.TEND_AGREEMENT_CODE == agreementCode
                            && tendAgreementMaster.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                            && imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                            select new
                            {
                                tendAgreementDetails.TEND_AGREEMENT_ID,
                                tendAgreementDetails.TEND_AGREEMENT_CODE,
                                tendAgreementDetails.IMS_PR_ROAD_CODE,
                                tendAgreementDetails.IMS_WORK_CODE,
                                imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                tendAgreementDetails.TEND_AGREEMENT_AMOUNT,
                                tendAgreementDetails.TEND_AMOUNT_YEAR1,
                                tendAgreementDetails.TEND_AMOUNT_YEAR2,
                                tendAgreementDetails.TEND_AMOUNT_YEAR3,
                                tendAgreementDetails.TEND_AMOUNT_YEAR4,
                                tendAgreementDetails.TEND_AMOUNT_YEAR5,
                                tendAgreementDetails.TEND_PART_AGREEMENT,
                                tendAgreementDetails.TEND_START_CHAINAGE,
                                tendAgreementDetails.TEND_END_CHAINAGE,
                                tendAgreementDetails.TEND_AGREEMENT_STATUS,
                                tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                                tendAgreementMaster.TEND_LOCK_STATUS,
                                proposalWorks.IMS_WORK_DESC,
                                tendAgreementMaster.TEND_AGREEMENT_TYPE,
                                imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                                imsSanctionedProjectDetails.IMS_BRIDGE_NAME,
                                tendAgreementDetails.TEND_INCOMPLETE_REASON,
                                tendAgreementDetails.TEND_VALUE_WORK_DONE,
                                tendAgreementDetails.TEND_AMOUNT_YEAR6,
                                imsSanctionedProjectDetails.IMS_YEAR,
                                imsSanctionedProjectDetails.IMS_PACKAGE_ID

                            };


                totalRecords = query == null ? 0 : query.Count();

                if (query.Any(q => q.TEND_AGREEMENT_TYPE == "O"))
                {
                    isRegularAgreement = false;
                }

                var existingAgreementDetails = (from tendAgreementMaster in dbContext.TEND_AGREEMENT_MASTER
                                                join tendAgreementDetails in dbContext.TEND_AGREEMENT_DETAIL
                                                on tendAgreementMaster.TEND_AGREEMENT_CODE equals tendAgreementDetails.TEND_AGREEMENT_CODE
                                                where
                                                tendAgreementDetails.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                                                tendAgreementMaster.TEND_AGREEMENT_TYPE == "O"
                                                select tendAgreementDetails
                           );

                if (existingAgreementDetails.Count() > 0)
                {
                    agreementID = existingAgreementDetails.Max(ad => ad.TEND_AGREEMENT_ID);
                    records = existingAgreementDetails.Count();

                    if (records > 1 && agreementID == query.Select(q => q.TEND_AGREEMENT_ID).FirstOrDefault())
                    {
                        records = 0;
                    }
                }

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "PartAgreement":
                                query = query.OrderBy(x => x.TEND_PART_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                            default:
                                query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "PartAgreement":
                                query = query.OrderByDescending(x => x.TEND_PART_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(tendAgreementDetails => new
                {
                    tendAgreementDetails.TEND_AGREEMENT_ID,
                    tendAgreementDetails.TEND_AGREEMENT_CODE,
                    tendAgreementDetails.IMS_PR_ROAD_CODE,
                    tendAgreementDetails.IMS_ROAD_NAME,
                    tendAgreementDetails.TEND_AGREEMENT_AMOUNT,
                    tendAgreementDetails.TEND_AMOUNT_YEAR1,
                    tendAgreementDetails.TEND_AMOUNT_YEAR2,
                    tendAgreementDetails.TEND_AMOUNT_YEAR3,
                    tendAgreementDetails.TEND_AMOUNT_YEAR4,
                    tendAgreementDetails.TEND_AMOUNT_YEAR5,
                    tendAgreementDetails.TEND_AGREEMENT_STATUS,
                    tendAgreementDetails.TEND_PART_AGREEMENT,
                    tendAgreementDetails.TEND_START_CHAINAGE,
                    tendAgreementDetails.TEND_END_CHAINAGE,
                    tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED,
                    tendAgreementDetails.TEND_LOCK_STATUS,
                    tendAgreementDetails.IMS_WORK_DESC,
                    tendAgreementDetails.IMS_PROPOSAL_TYPE,
                    tendAgreementDetails.IMS_BRIDGE_NAME,
                    tendAgreementDetails.TEND_INCOMPLETE_REASON,
                    tendAgreementDetails.TEND_VALUE_WORK_DONE,
                    tendAgreementDetails.TEND_AMOUNT_YEAR6,
                    tendAgreementDetails.IMS_PACKAGE_ID,
                    tendAgreementDetails.IMS_YEAR
                }).ToArray();


                return result.Select(tendAgreementDetails => new
                {
                    cell = new[] {                                   
                                    tendAgreementDetails.IMS_YEAR == null?"-":tendAgreementDetails.IMS_YEAR + "-" + (tendAgreementDetails.IMS_YEAR + 1),
                                    tendAgreementDetails.IMS_PACKAGE_ID == null?"-":tendAgreementDetails.IMS_PACKAGE_ID.ToString(),
                                    tendAgreementDetails.IMS_PROPOSAL_TYPE=="P"?tendAgreementDetails.IMS_ROAD_NAME.ToString():(tendAgreementDetails.IMS_BRIDGE_NAME==null?"NA":tendAgreementDetails.IMS_BRIDGE_NAME.ToString()),                    
                                    tendAgreementDetails.IMS_WORK_DESC==null?"NA": tendAgreementDetails.IMS_WORK_DESC.ToString().Trim(),             
                                    tendAgreementDetails.TEND_PART_AGREEMENT.ToString().Trim()=="Y"?"Yes":"No",                                                 
                                    tendAgreementDetails.TEND_START_CHAINAGE==null?"NA":(tendAgreementDetails.TEND_START_CHAINAGE.ToString().Trim()),
                                    tendAgreementDetails.TEND_START_CHAINAGE==null?"NA":(tendAgreementDetails.TEND_END_CHAINAGE.ToString().Trim()),
                                    tendAgreementDetails.TEND_AGREEMENT_AMOUNT.ToString(),      
                                    ((tendAgreementDetails.TEND_AMOUNT_YEAR1==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR1)+
                                       (tendAgreementDetails.TEND_AMOUNT_YEAR2==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR2)+
                                       (tendAgreementDetails.TEND_AMOUNT_YEAR3==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR3)+
                                       (tendAgreementDetails.TEND_AMOUNT_YEAR4==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR4)+
                                       (tendAgreementDetails.TEND_AMOUNT_YEAR5==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR5) +(PMGSYSession.Current.PMGSYScheme == 2 ?(tendAgreementDetails.TEND_AMOUNT_YEAR6==null?0:tendAgreementDetails.TEND_AMOUNT_YEAR6):0)
                                    ).ToString(),
                                    WorkStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString(),//AgreementStatus[tendAgreementDetails.TEND_AGREEMENT_STATUS].ToString(),

                                      tendAgreementDetails.TEND_VALUE_WORK_DONE==null?"NA":(tendAgreementDetails.TEND_VALUE_WORK_DONE.ToString().Trim()),
                                      tendAgreementDetails.TEND_INCOMPLETE_REASON==null?"NA":(tendAgreementDetails.TEND_INCOMPLETE_REASON.ToString().Trim()),

                                    (tendAgreementDetails.TEND_AGREEMENT_STATUS =="C" || tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="N"||tendAgreementDetails.TEND_LOCK_STATUS=="Y" ||  records>1 )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString() }),
                                    (isRegularAgreement==true?(tendAgreementDetails.TEND_AGREEMENT_STATUS =="W" || tendAgreementDetails.TEND_AGREEMENT_STATUS =="C" || tendAgreementDetails.TEND_AGREEMENT_STATUS =="M" || tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="N") : (tendAgreementDetails.TEND_AGREEMENT_STATUS =="W" || tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="N") ) ?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString() }),
                                    (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47) ? ( tendAgreementDetails.TEND_AGREEMENT_STATUS == "I" ? string.Empty : URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + string.Empty,"ProposalType="+tendAgreementDetails.IMS_PROPOSAL_TYPE })) : (tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementDetails.TEND_LOCK_STATUS=="Y" || tendAgreementDetails.TEND_AGREEMENT_STATUS=="W" )?string.Empty: URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + string.Empty,"ProposalType="+tendAgreementDetails.IMS_PROPOSAL_TYPE }),
                                    (tendAgreementDetails.TEND_IS_AGREEMENT_FINALIZED=="Y"||tendAgreementDetails.TEND_LOCK_STATUS=="Y"|| tendAgreementDetails.TEND_AGREEMENT_STATUS=="W" )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendAgreementID =" + tendAgreementDetails.TEND_AGREEMENT_ID.ToString(),"TendAgreementCode =" + tendAgreementDetails.TEND_AGREEMENT_CODE.ToString(),"IMSPRRoadCode =" + tendAgreementDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + string.Empty,"ProposalType="+tendAgreementDetails.IMS_PROPOSAL_TYPE  })

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
        /// returns the special agreement details
        /// </summary>
        /// <param name="agreementCode"></param>
        /// <param name="isView"></param>
        /// <returns></returns>
        public SpecialAgreementDetails GetSpecialAgreementMasterDetailsDAL_ByAgreementCode(int agreementCode, bool isView = false)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;
                CommonFunctions commonFunction = new CommonFunctions();
                TEND_AGREEMENT_MASTER master_agreement = null;

                // added by rohit borse on 01-07-2022
                TEND_AGREEMENT_DETAIL details_agreement = null;

                if (!isView)
                {
                    master_agreement = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode && am.TEND_IS_AGREEMENT_FINALIZED == "N" && am.TEND_LOCK_STATUS == "N").FirstOrDefault();

                    //added by rohit borse on 01-07-2022
                    details_agreement = dbContext.TEND_AGREEMENT_DETAIL.Where(am => am.TEND_AGREEMENT_CODE == master_agreement.TEND_AGREEMENT_CODE).FirstOrDefault();
                }
                else
                {
                    master_agreement = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();

                    //added by rohit borse on 01-07-2022
                    details_agreement = dbContext.TEND_AGREEMENT_DETAIL.Where(am => am.TEND_AGREEMENT_CODE == master_agreement.TEND_AGREEMENT_CODE).FirstOrDefault();
                }

                SpecialAgreementDetails agreementDetails = null;


                if (master_agreement != null)
                {


                    agreementDetails = new SpecialAgreementDetails()
                    {

                        EncryptedTendAgreementCode = URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + master_agreement.TEND_AGREEMENT_CODE.ToString() }),

                        Mast_Con_Sup_Flag = master_agreement.TEND_AGREEMENT_TYPE == "S" ? "S" : "C",
                        MAST_CON_ID = master_agreement.MAST_CON_ID == null ? 0 : (Int32)master_agreement.MAST_CON_ID,
                        TEND_AGREEMENT_NUMBER = master_agreement.TEND_AGREEMENT_NUMBER,
                        TEND_DATE_OF_AGREEMENT = Convert.ToDateTime(master_agreement.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),
                        TEND_AGREEMENT_START_DATE = Convert.ToDateTime(master_agreement.TEND_AGREEMENT_START_DATE).ToString("dd/MM/yyyy"),
                        TEND_AGREEMENT_END_DATE = Convert.ToDateTime(master_agreement.TEND_AGREEMENT_END_DATE).ToString("dd/MM/yyyy"),
                        TEND_DATE_OF_AWARD_WORK = master_agreement.TEND_DATE_OF_AWARD_WORK == null ? string.Empty : Convert.ToDateTime(master_agreement.TEND_DATE_OF_AWARD_WORK).ToString("dd/MM/yyyy"),
                        TEND_DATE_OF_WORK_ORDER = master_agreement.TEND_DATE_OF_WORK_ORDER == null ? string.Empty : Convert.ToDateTime(master_agreement.TEND_DATE_OF_WORK_ORDER).ToString("dd/MM/yyyy"),
                        TEND_DATE_OF_COMMENCEMENT = master_agreement.TEND_DATE_OF_COMMENCEMENT == null ? string.Empty : Convert.ToDateTime(master_agreement.TEND_DATE_OF_COMMENCEMENT).ToString("dd/MM/yyyy"),
                        TEND_DATE_OF_COMPLETION = master_agreement.TEND_DATE_OF_COMPLETION == null ? string.Empty : Convert.ToDateTime(master_agreement.TEND_DATE_OF_COMPLETION).ToString("dd/MM/yyyy"),
                        TEND_AGREEMENT_REMARKS = master_agreement.TEND_AGREEMENT_REMARKS,
                        TEND_TENDER_AMOUNT = master_agreement.TEND_TENDER_AMOUNT == null ? 0 : master_agreement.TEND_TENDER_AMOUNT,
                        TEND_AGREEMENT_AMOUNT = master_agreement.TEND_AGREEMENT_AMOUNT,

                        // added by rohit borse on 01-07-2022
                        APS_COLLECTED = master_agreement.APS_COLLECTED,
                        APS_COLLECTED_AMOUNT = master_agreement.APS_COLLECTED_AMOUNT,
                        GST_AMT_MAINTAINANCE_AGREEMENT = master_agreement.GST_AMT_MAINT_AGREEMENT,
                        GST_AMT_MAINTAINANCE_AGREEMENT_DLP = details_agreement.GST_AMT_MAINT_AGREEMENT_DLP,
                    };

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementDetails.TEND_HIGHER_SPEC_AMT = master_agreement.TEND_HIGHER_SPEC_AMT;
                        agreementDetails.TEND_MORD_SHARE = master_agreement.TEND_MORD_SHARE;
                        agreementDetails.TEND_STATE_SHARE = master_agreement.TEND_STATE_SHARE;
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
        /// updates the details of special agreement details
        /// </summary>
        /// <param name="details_agreement"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UpdateSpecialAgreementDetailsDAL(ExistingSpecialAgreementDetails details_agreement, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int IMSPRRoadCode = 0;
                string roadName = string.Empty;
                int agreementID = 0;
                int agreementCode = 0;
                Decimal? roadLength = 0;
                TEND_AGREEMENT_DETAIL agreementDetails = null;
                TEND_AGREEMENT_MASTER agreementMaster = null;
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                CommonFunctions commonFunction = new CommonFunctions();

                encryptedParameters = details_agreement.EncryptedTendAgreementID_Existing.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString());
                agreementID = Convert.ToInt32(decryptedParameters["TendAgreementID"].ToString());
                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());

                string proposalType = decryptedParameters["ProposalType"].ToString();

                if (proposalType.Equals("P"))
                {
                    roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_ROAD_NAME).FirstOrDefault();
                }
                else
                {
                    roadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_BRIDGE_NAME).FirstOrDefault();
                }

                string agreementType = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).Select(am => (string)am.TEND_AGREEMENT_TYPE).FirstOrDefault();


                if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_PART_AGREEMENT == "Y" && ad.TEND_AGREEMENT_STATUS != "W" && ad.TEND_AGREEMENT_ID != agreementID))
                {
                    Int32? imsWorkCode = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_ID == agreementID).Select(ad => (Int32?)ad.IMS_WORK_CODE).FirstOrDefault();

                    if (imsWorkCode > 0)
                    {
                        if (details_agreement.TEND_START_CHAINAGE_Existing != null && details_agreement.TEND_END_CHAINAGE_Existing != null)
                        {
                            if ((details_agreement.TEND_END_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_END_CHAINAGE_Existing) > (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_WORK_CODE == imsWorkCode && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(pw => (Decimal?)pw.IMS_END_CHAINAGE).FirstOrDefault()) || (details_agreement.TEND_START_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_START_CHAINAGE_Existing) < (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_WORK_CODE == imsWorkCode && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(pw => (Decimal?)pw.IMS_START_CHAINAGE).FirstOrDefault()))
                            {
                                message = "Chainage exceeds its road length.";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        roadLength = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_AGREEMENT_STATUS != "W" && ad.TEND_AGREEMENT_ID != agreementID).Sum(td => (Decimal?)(td.TEND_END_CHAINAGE - td.TEND_START_CHAINAGE));
                        roadLength = (roadLength == null ? 0 : roadLength) + ((details_agreement.TEND_END_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_END_CHAINAGE_Existing) - (details_agreement.TEND_START_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_START_CHAINAGE_Existing));
                        if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                        {
                            message = "Chainage exceeds its road length.";
                            return false;
                        }
                    }
                }
                else
                {
                    roadLength += ((details_agreement.TEND_END_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_END_CHAINAGE_Existing) - (details_agreement.TEND_START_CHAINAGE_Existing == null ? 0 : details_agreement.TEND_START_CHAINAGE_Existing));

                    if (roadLength > dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(sp => (Decimal?)sp.IMS_PAV_LENGTH).FirstOrDefault())
                    {
                        message = "Chainage exceeds its road length.";
                        return false;
                    }
                }

                var sanctionProposal = dbContext.IMS_SANCTIONED_PROJECTS.Find(IMSPRRoadCode);
                decimal sanctionCost = sanctionProposal.IMS_PROPOSAL_TYPE == "P" ? (PMGSYSession.Current.PMGSYScheme == 1 ? (sanctionProposal.IMS_SANCTIONED_PAV_AMT + sanctionProposal.IMS_SANCTIONED_PW_AMT + sanctionProposal.IMS_SANCTIONED_OW_AMT + sanctionProposal.IMS_SANCTIONED_CD_AMT + sanctionProposal.IMS_SANCTIONED_RS_AMT) : (sanctionProposal.IMS_SANCTIONED_PAV_AMT + sanctionProposal.IMS_SANCTIONED_PW_AMT + sanctionProposal.IMS_SANCTIONED_OW_AMT + sanctionProposal.IMS_SANCTIONED_CD_AMT + sanctionProposal.IMS_SANCTIONED_RS_AMT + (sanctionProposal.IMS_SANCTIONED_HS_AMT.HasValue ? sanctionProposal.IMS_SANCTIONED_HS_AMT.Value : 0) + (sanctionProposal.IMS_SANCTIONED_FC_AMT.HasValue ? sanctionProposal.IMS_SANCTIONED_FC_AMT.Value : 0))) : (sanctionProposal.IMS_SANCTIONED_BW_AMT + sanctionProposal.IMS_SANCTIONED_BS_AMT);

                if (dbContext.IMS_PROPOSAL_COST_ADD.Any(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                {
                    decimal? costToAdd = dbContext.IMS_PROPOSAL_COST_ADD.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(m => m.IMS_TRANSACTION_CODE).Select(m => m.IMS_STATE_AMOUNT).FirstOrDefault() + dbContext.IMS_PROPOSAL_COST_ADD.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).OrderByDescending(m => m.IMS_TRANSACTION_CODE).Select(m => m.IMS_MORD_AMOUNT).FirstOrDefault();
                    sanctionCost = sanctionCost + (costToAdd.HasValue ? costToAdd.Value : 0);
                }

               

                using (var scope = new TransactionScope())
                {

                    agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();
                    agreementDetails = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_ID == agreementID).FirstOrDefault();

                    if (agreementMaster == null || agreementDetails == null)
                    {
                        return false;
                    }

                    agreementMaster.TEND_AGREEMENT_AMOUNT = agreementMaster.TEND_AGREEMENT_AMOUNT + (Decimal)(details_agreement.TEND_AGREEMENT_AMOUNT_NEW - agreementDetails.TEND_AGREEMENT_AMOUNT);
                    agreementMaster.TEND_AMOUNT_YEAR1 = (agreementMaster.TEND_AMOUNT_YEAR1 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR1) + 0;
                    agreementMaster.TEND_AMOUNT_YEAR2 = (agreementMaster.TEND_AMOUNT_YEAR2 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR2) + 0;
                    agreementMaster.TEND_AMOUNT_YEAR3 = (agreementMaster.TEND_AMOUNT_YEAR3 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR3) + 0;
                    agreementMaster.TEND_AMOUNT_YEAR4 = (agreementMaster.TEND_AMOUNT_YEAR4 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR4) + 0;
                    agreementMaster.TEND_AMOUNT_YEAR5 = (agreementMaster.TEND_AMOUNT_YEAR5 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR5) + 0;


                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementMaster.TEND_AMOUNT_YEAR6 = (agreementMaster.TEND_AMOUNT_YEAR6 == null ? 0 : agreementMaster.TEND_AMOUNT_YEAR6) + 0;
                        agreementMaster.TEND_STATE_SHARE = (agreementMaster.TEND_STATE_SHARE == null ? null : agreementMaster.TEND_STATE_SHARE) + (details_agreement.TEND_STATE_SHARE_NEW == null ? 0 : details_agreement.TEND_STATE_SHARE_NEW) - (agreementDetails.TEND_STATE_SHARE == null ? 0 : agreementDetails.TEND_STATE_SHARE);
                        agreementMaster.TEND_MORD_SHARE = (agreementMaster.TEND_MORD_SHARE == null ? null : agreementMaster.TEND_MORD_SHARE) + (details_agreement.TEND_MORD_SHARE_NEW == null ? 0 : details_agreement.TEND_MORD_SHARE_NEW) - (agreementDetails.TEND_MORD_SHARE == null ? 0 : agreementDetails.TEND_MORD_SHARE);
                        agreementMaster.TEND_HIGHER_SPEC_AMT = (agreementMaster.TEND_HIGHER_SPEC_AMT == null ? null : agreementMaster.TEND_HIGHER_SPEC_AMT) + (details_agreement.TEND_HIGHER_SPEC_AMT_NEW == null ? 0 : details_agreement.TEND_HIGHER_SPEC_AMT_NEW) - (agreementDetails.TEND_HIGHER_SPEC_AMT == null ? 0 : agreementDetails.TEND_HIGHER_SPEC_AMT);
                    }
                    agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementMaster.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;



                    agreementDetails.TEND_AGREEMENT_AMOUNT = (Decimal)details_agreement.TEND_AGREEMENT_AMOUNT_NEW;
                    agreementDetails.TEND_AMOUNT_YEAR1 = 0;
                    agreementDetails.TEND_AMOUNT_YEAR2 = 0;
                    agreementDetails.TEND_AMOUNT_YEAR3 = 0;
                    agreementDetails.TEND_AMOUNT_YEAR4 = 0;
                    agreementDetails.TEND_AMOUNT_YEAR5 = 0;

                    

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        agreementDetails.TEND_AMOUNT_YEAR6 = 0;
                        agreementDetails.TEND_STATE_SHARE = details_agreement.TEND_STATE_SHARE_NEW == null ? null : details_agreement.TEND_STATE_SHARE_NEW;
                        agreementDetails.TEND_MORD_SHARE = details_agreement.TEND_MORD_SHARE_NEW == null ? null : details_agreement.TEND_MORD_SHARE_NEW;
                    }
                    agreementDetails.TEND_START_CHAINAGE = details_agreement.TEND_START_CHAINAGE_Existing == null ? null : (details_agreement.TEND_START_CHAINAGE_Existing);
                    agreementDetails.TEND_END_CHAINAGE = details_agreement.TEND_END_CHAINAGE_Existing == null ? null : (details_agreement.TEND_END_CHAINAGE_Existing);

                    // added on 15-07-2022
                    agreementDetails.GST_AMT_MAINT_AGREEMENT_DLP = details_agreement.GST_AMT_MAINTAINANCE_AGREEMENT_DLP;

                    agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    agreementDetails.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(agreementDetails).State = System.Data.Entity.EntityState.Modified;

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

        /// <summary>
        /// get the details of special agreement details
        /// </summary>
        /// <param name="agreementCode"></param>
        /// <param name="agreementID"></param>
        /// <returns></returns>
        public ExistingSpecialAgreementDetails GetSpecialAgreementDetailsDAL_ByAgreementID(int agreementCode, int agreementID)
        {
            ExistingSpecialAgreementDetails existingAgreementDetails = new ExistingSpecialAgreementDetails();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                TEND_AGREEMENT_MASTER master_agreement = dbContext.TEND_AGREEMENT_MASTER.Where(am => am.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();
                TEND_AGREEMENT_DETAIL details_agreement = dbContext.TEND_AGREEMENT_DETAIL.Where(ad => ad.TEND_AGREEMENT_CODE == agreementCode && ad.TEND_AGREEMENT_ID == agreementID).FirstOrDefault();
                IMS_SANCTIONED_PROJECTS projects_sanctioned = dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == details_agreement.IMS_PR_ROAD_CODE).FirstOrDefault();


                if (master_agreement == null || details_agreement == null || projects_sanctioned == null)
                {
                    return existingAgreementDetails;
                }

                existingAgreementDetails.EncryptedIMSPRRoadCode_Existing = URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + projects_sanctioned.IMS_PR_ROAD_CODE.ToString(), "ProposalType=" + projects_sanctioned.IMS_PROPOSAL_TYPE });

                existingAgreementDetails.EncryptedTendAgreementCode_Existing = URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + master_agreement.TEND_AGREEMENT_CODE.ToString() });
                existingAgreementDetails.TEND_DATE_OF_AGREEMENT = master_agreement.TEND_DATE_OF_AGREEMENT == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_AGREEMENT_START_DATE = master_agreement.TEND_AGREEMENT_START_DATE == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_AGREEMENT_START_DATE).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_AGREEMENT_END_DATE = master_agreement.TEND_AGREEMENT_END_DATE == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_AGREEMENT_END_DATE).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_TENDER_AMOUNT = master_agreement.TEND_TENDER_AMOUNT == null ? 0 : master_agreement.TEND_TENDER_AMOUNT;
                existingAgreementDetails.TEND_AGREEMENT_AMOUNT_Existing = master_agreement.TEND_AGREEMENT_AMOUNT;
                existingAgreementDetails.TEND_DATE_OF_AWARD_WORK = master_agreement.TEND_DATE_OF_AWARD_WORK == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_AWARD_WORK).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_DATE_OF_WORK_ORDER = master_agreement.TEND_DATE_OF_WORK_ORDER == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_WORK_ORDER).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_DATE_OF_COMMENCEMENT = master_agreement.TEND_DATE_OF_COMMENCEMENT == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_COMMENCEMENT).ToString("dd/MM/yyyy");
                existingAgreementDetails.TEND_DATE_OF_COMPLETION = master_agreement.TEND_DATE_OF_COMPLETION == null ? "NA" : Convert.ToDateTime(master_agreement.TEND_DATE_OF_COMPLETION).ToString("dd/MM/yyyy");

                // added on 15-07-2022
                existingAgreementDetails.GST_AMT_MAINTAINANCE_AGREEMENT_DLP = details_agreement.GST_AMT_MAINT_AGREEMENT_DLP;

                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    existingAgreementDetails.TEND_MORD_SHARE_NEW = details_agreement.TEND_MORD_SHARE == null ? 0 : details_agreement.TEND_MORD_SHARE;
                    existingAgreementDetails.TEND_STATE_SHARE_NEW = details_agreement.TEND_STATE_SHARE == null ? 0 : details_agreement.TEND_STATE_SHARE.Value;
                    existingAgreementDetails.TEND_HIGHER_SPEC_AMT_NEW = details_agreement.TEND_HIGHER_SPEC_AMT == null ? 0 : details_agreement.TEND_HIGHER_SPEC_AMT.Value;
                    if (projects_sanctioned.IMS_SHARE_PERCENT == 1)
                    {
                        existingAgreementDetails.ProposalStateShare = "10";
                        existingAgreementDetails.ProposalMordShare = "90";
                    }
                    else if (projects_sanctioned.IMS_SHARE_PERCENT == 2)
                    {
                        existingAgreementDetails.ProposalStateShare = "25";
                        existingAgreementDetails.ProposalMordShare = "75";
                    }

                    if (projects_sanctioned.IMS_PROPOSAL_TYPE == "P")
                    {
                        existingAgreementDetails.ProposalStateCost = projects_sanctioned.IMS_SANCTIONED_RS_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_RS_AMT;
                        existingAgreementDetails.ProposalMordCost = ((projects_sanctioned.IMS_SANCTIONED_PAV_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_PAV_AMT) + (projects_sanctioned.IMS_SANCTIONED_PW_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_PW_AMT) + (projects_sanctioned.IMS_SANCTIONED_OW_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_OW_AMT) + (projects_sanctioned.IMS_SANCTIONED_CD_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_CD_AMT) + (projects_sanctioned.IMS_SANCTIONED_FC_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_FC_AMT) - (projects_sanctioned.IMS_SANCTIONED_RS_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_RS_AMT)).Value;
                    }
                    else if (projects_sanctioned.IMS_PROPOSAL_TYPE == "L")
                    {
                        existingAgreementDetails.ProposalStateCost = projects_sanctioned.IMS_SANCTIONED_BS_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_BS_AMT;
                        existingAgreementDetails.ProposalMordCost = (projects_sanctioned.IMS_SANCTIONED_BW_AMT == null ? 0 : projects_sanctioned.IMS_SANCTIONED_BW_AMT);
                    }
                }
                //working but removed road name
                //existingAgreementDetails.EncryptedTendAgreementID_Existing = URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + master_agreement.TEND_AGREEMENT_CODE.ToString(), "TendAgreementID =" + details_agreement.TEND_AGREEMENT_ID.ToString(), "IMSPRRoadCode =" + projects_sanctioned.IMS_PR_ROAD_CODE.ToString(), "IMSRoadName =" + projects_sanctioned.IMS_ROAD_NAME.ToString() });

                existingAgreementDetails.EncryptedTendAgreementID_Existing = URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + master_agreement.TEND_AGREEMENT_CODE.ToString(), "TendAgreementID =" + details_agreement.TEND_AGREEMENT_ID.ToString(), "IMSPRRoadCode =" + projects_sanctioned.IMS_PR_ROAD_CODE.ToString(), "IMSRoadName =" + string.Empty, "ProposalType=" + projects_sanctioned.IMS_PROPOSAL_TYPE });
                existingAgreementDetails.TEND_AGREEMENT_AMOUNT_NEW = details_agreement.TEND_AGREEMENT_AMOUNT;

                existingAgreementDetails.IsPartAgreement_Existing = details_agreement.TEND_PART_AGREEMENT == "Y" ? true : false;
                existingAgreementDetails.TEND_START_CHAINAGE_Existing = details_agreement.TEND_START_CHAINAGE == null ? null : details_agreement.TEND_START_CHAINAGE;
                existingAgreementDetails.TEND_END_CHAINAGE_Existing = details_agreement.TEND_END_CHAINAGE == null ? null : details_agreement.TEND_END_CHAINAGE;

                existingAgreementDetails.IMS_WORK_DESC = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == details_agreement.IMS_PR_ROAD_CODE && pw.IMS_WORK_CODE == details_agreement.IMS_WORK_CODE).Select(pw => pw.IMS_WORK_DESC).FirstOrDefault();

                existingAgreementDetails.IMS_WORK_DESC = existingAgreementDetails.IMS_WORK_DESC == null ? "NA" : existingAgreementDetails.IMS_WORK_DESC;

                return existingAgreementDetails;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return existingAgreementDetails;
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

        #region AGREEMENT_UPDATION

        /// <summary>
        /// returns the list of agreements which are incomplete
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="yearCode"></param>
        /// <param name="blockCode"></param>
        /// <param name="package"></param>
        /// <param name="proposalType"></param>
        /// <param name="agreementStatus"></param>
        /// <param name="finalize"></param>
        /// <param name="agreementType"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetAgreementListForUpdationDAL(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, int state, int district, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstAgreements = dbContext.USP_GET_AGREEMENTS_FOR_UPDATION(state, (district <= 0 ? 0 : district), (blockCode <= 0 ? 0 : blockCode), (yearCode <= 0 ? 0 : yearCode), (package == "0" ? "%" : (package == "All" ? "%" : package)), PMGSYSession.Current.AdminNdCode, "W", (finalize == "0" ? "%" : finalize), (proposalType == "0" ? "%" : (proposalType == "All" ? "%" : proposalType)), agreementType, PMGSYSession.Current.PMGSYScheme).ToList();

                totalRecords = lstAgreements.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                lstAgreements = lstAgreements.OrderBy(x => x.CONTRACTOR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementType":
                                lstAgreements = lstAgreements.OrderBy(x => x.AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementNumber":
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            //case "AgreementDate":
                            //    lstAgreements = lstAgreements.OrderBy(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                            //    break;
                            default:
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_AGREEMENT_NUMBER).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.CONTRACTOR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementType":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementNumber":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            //case "AgreementDate":
                            //    lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                            //    break;
                            default:
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }

                    }
                }
                else
                {
                    lstAgreements = lstAgreements.OrderBy(x => x.CONTRACTOR_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstAgreements.Select(tendAgreementMaster => new
                {
                    tendAgreementMaster.TEND_AGREEMENT_CODE,
                    tendAgreementMaster.CONTRACTOR_NAME,
                    tendAgreementMaster.AGREEMENT_TYPE,
                    tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                    //tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                    tendAgreementMaster.MAINTENANCE_AMOUNT,
                    tendAgreementMaster.AGREEMENT_STATUS,
                    tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                    tendAgreementMaster.TEND_LOCK_STATUS,
                    tendAgreementMaster.TEND_AGREEMENT_STATUS,
                    tendAgreementMaster.TEND_AGREEMENT_TYPE,
                    tendAgreementMaster.PROPOSAL_NAME,
                    tendAgreementMaster.PROPOSAL_LENGTH,
                    tendAgreementMaster.IMS_YEAR,
                    tendAgreementMaster.IMS_BATCH,
                    tendAgreementMaster.IMS_PACKAGE_ID,
                    tendAgreementMaster.TEND_AGREEMENT_ID
                }).ToArray();

                return result.Select(tendAgreementMaster => new
                {
                    id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {                                                                               
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    tendAgreementMaster.TEND_AGREEMENT_NUMBER.ToString(),
                                    tendAgreementMaster.CONTRACTOR_NAME==null?"NA":tendAgreementMaster.CONTRACTOR_NAME.ToString().Trim(),
                                    tendAgreementMaster.AGREEMENT_TYPE.ToString(), 
                                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT.ToString(),      
                                    tendAgreementMaster.PROPOSAL_NAME.ToString(),
                                    tendAgreementMaster.PROPOSAL_LENGTH.ToString(),
                                    "Batch - " + tendAgreementMaster.IMS_BATCH.ToString(),
                                    tendAgreementMaster.IMS_YEAR + " - " + (tendAgreementMaster.IMS_YEAR + 1),
                                    tendAgreementMaster.IMS_PACKAGE_ID.ToString(),
                                    tendAgreementMaster.MAINTENANCE_AMOUNT.ToString(),
                                    tendAgreementMaster.AGREEMENT_STATUS.ToString(),
                                    tendAgreementMaster.TEND_AGREEMENT_STATUS == "W" ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil ui-align-center' title='DeFinalize Agreement' onClick ='ChangeAgreementStatus(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementId="+tendAgreementMaster.TEND_AGREEMENT_ID.ToString()}) + "\");' ></span></td></tr></table></center>" : "<span class='ui-icon ui-icon-unlocked ui-align-center'></span>"
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
        /// updates the status of agreement to inprogress
        /// </summary>
        /// <param name="agreementId"></param>
        /// <returns></returns>
        public bool ChangeAgreementStatusDAL(int agreementId)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    TEND_AGREEMENT_DETAIL agreementDetails = dbContext.TEND_AGREEMENT_DETAIL.Find(agreementId);

                    if (agreementDetails != null)
                    {
                        TEND_AGREEMENT_MASTER agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Find(agreementDetails.TEND_AGREEMENT_CODE);

                        agreementDetails.TEND_AGREEMENT_STATUS = "P";
                        agreementDetails.USERID = PMGSYSession.Current.UserId;
                        agreementDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(agreementDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        agreementMaster.TEND_AGREEMENT_STATUS = "P";
                        agreementMaster.USERID = PMGSYSession.Current.UserId;
                        agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(agreementMaster).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        ts.Complete();
                    }
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
        /// populates the packages by year and block
        /// </summary>
        /// <param name="year"></param>
        /// <param name="block"></param>
        /// <param name="isSearch"></param>
        /// <returns></returns>
        public List<IMS_SANCTIONED_PROJECTS> GetPackages(int year, int block, bool isSearch)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                List<IMS_SANCTIONED_PROJECTS> packageList = (from sanctionProjects in dbContext.IMS_SANCTIONED_PROJECTS
                                                             where
                                                             (year == 0 ? 1 : sanctionProjects.IMS_YEAR) == (year == 0 ? 1 : year) &&
                                                                 //sanctionProjects.IMS_YEAR == year &&
                                                             (block <= 0 ? 1 : sanctionProjects.MAST_BLOCK_CODE) == (block <= 0 ? 1 : block) &&
                                                             sanctionProjects.IMS_SANCTIONED == "Y" &&
                                                             sanctionProjects.IMS_DPR_STATUS == "N" &&
                                                             sanctionProjects.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                                                             select sanctionProjects).Distinct().ToList<IMS_SANCTIONED_PROJECTS>();



                packageList = packageList.GroupBy(pl => pl.IMS_PACKAGE_ID).Select(pl => pl.FirstOrDefault()).ToList<IMS_SANCTIONED_PROJECTS>();

                if (isSearch)
                {
                    packageList.Insert(0, new IMS_SANCTIONED_PROJECTS() { IMS_ROAD_NAME = "0", IMS_PACKAGE_ID = "All Packages" });
                }
                else
                {
                    packageList.Insert(0, new IMS_SANCTIONED_PROJECTS() { IMS_ROAD_NAME = "0", IMS_PACKAGE_ID = "Select Package" });

                }

                return packageList.OrderBy(o => o.IMS_PACKAGE_ID).ToList();

            }
            catch (Exception)
            {
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

        #region 01 June 2016

        public MASTER_CONTRACTOR_BANK GetContratorBankAccNoAndIFSCcode(int contractorId)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int lgdStateCode = 0;
            try
            {
               
                MASTER_CONTRACTOR_BANK contratorBankDetails = new MASTER_CONTRACTOR_BANK();

                if (PMGSYSession.Current.FundType != "A")
                {
                   
                    lgdStateCode = dbContext.OMMAS_LDG_STATE_MAPPING.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(z => z.MAST_STATE_LDG_CODE).FirstOrDefault();


                    // Below is commented as per suggestion by Srinivasa Sir on 30 Sept 2020 
                    //var pfmsBankDetails = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(c => c.MAST_CON_ID == contractorId && c.MAST_LGD_STATE_CODE == lgdStateCode && c.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode && c.STATUS == "A"
                    //                        && c.PFMS_CON_ID != null
                    //                        && c.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Where(z => z.MAST_CON_ID == contractorId && z.MAST_ACCOUNT_ID == c.MAST_ACCOUNT_ID && z.MAST_LOCK_STATUS == "Y").Select(a => a.MAST_ACCOUNT_STATUS).FirstOrDefault() == "A").FirstOrDefault();





                    // Below is added on 30 Sept 2020 as per suggestion by Srinivasa Sir
                    var pfmsBankDetails = dbContext.REAT_CONTRACTOR_DETAILS.Where(c => c.MAST_CON_ID == contractorId && c.MAST_LGD_STATE_CODE == lgdStateCode && c.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode && c.ommas_STATUS == "A"
                                            && c.REAT_CON_ID != null).FirstOrDefault();
                                        
                                            
     
                    
                    if (pfmsBankDetails != null)
                    {
                        contratorBankDetails = new MASTER_CONTRACTOR_BANK();
                        contratorBankDetails.MAST_CON_ID = contractorId;


                        contratorBankDetails.MAST_ACCOUNT_ID = pfmsBankDetails.MAST_ACCOUNT_ID;
                        contratorBankDetails.MAST_ACCOUNT_NUMBER = pfmsBankDetails.MAST_ACCOUNT_NUMBER;
                        contratorBankDetails.MAST_IFSC_CODE = pfmsBankDetails.MAST_IFSC_CODE;
                        contratorBankDetails.MAST_BANK_NAME = pfmsBankDetails.MAST_BANK_NAME;

                       
                    }
                }
                return contratorBankDetails;

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
        #endregion

        #region BANK guarantee

        public Array GetAgreementListBALForBG(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, out long totalRecords)
        {

            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstAgreements = dbContext.USP_GET_AGREEMENTS(PMGSYSession.Current.DistrictCode, (blockCode <= 0 ? 0 : blockCode), (yearCode <= 0 ? 0 : yearCode), (package == "0" ? "%" : (package == "All" ? "%" : package)), PMGSYSession.Current.AdminNdCode, (agreementStatus == "0" ? "%" : agreementStatus), "Y", (proposalType == "0" ? "%" : (proposalType == "All" ? "%" : proposalType)), "C", PMGSYSession.Current.PMGSYScheme).ToList();
                var lstAgreementsR = dbContext.USP_GET_AGREEMENTS(PMGSYSession.Current.DistrictCode, (blockCode <= 0 ? 0 : blockCode), (yearCode <= 0 ? 0 : yearCode), (package == "0" ? "%" : (package == "All" ? "%" : package)), PMGSYSession.Current.AdminNdCode, (agreementStatus == "0" ? "%" : agreementStatus), "Y", (proposalType == "0" ? "%" : (proposalType == "All" ? "%" : proposalType)), "R", PMGSYSession.Current.PMGSYScheme).ToList();
                lstAgreements.AddRange(lstAgreementsR);
                totalRecords = lstAgreements.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                lstAgreements = lstAgreements.OrderBy(x => x.CONTRACTOR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementType":
                                lstAgreements = lstAgreements.OrderBy(x => x.AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementNumber":
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementDate":
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_AGREEMENT_NUMBER).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.CONTRACTOR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementType":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementNumber":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementDate":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }

                    }
                }
                else
                {
                    lstAgreements = lstAgreements.OrderBy(x => x.CONTRACTOR_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstAgreements.Select(tendAgreementMaster => new
                {
                    tendAgreementMaster.TEND_AGREEMENT_CODE,
                    tendAgreementMaster.CONTRACTOR_NAME,
                    tendAgreementMaster.AGREEMENT_TYPE,
                    tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                    tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                    tendAgreementMaster.MAINTENANCE_AMOUNT,
                    tendAgreementMaster.AGREEMENT_STATUS,
                    tendAgreementMaster.TEND_IS_AGREEMENT_FINALIZED,
                    tendAgreementMaster.TEND_LOCK_STATUS,
                    tendAgreementMaster.TEND_AGREEMENT_STATUS,
                    tendAgreementMaster.TEND_AGREEMENT_TYPE,
                    tendAgreementMaster.TEND_AGREEMENT_END_DATE
                }).ToArray();


                return result.Select(tendAgreementMaster => new
                {

                    id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {                                                                               
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    tendAgreementMaster.TEND_AGREEMENT_NUMBER.ToString(),
                                    tendAgreementMaster.CONTRACTOR_NAME==null?"NA":tendAgreementMaster.CONTRACTOR_NAME.ToString().Trim(),
                                   // tendAgreementMaster.AGREEMENT_TYPE.ToString(), 
                                    Convert.ToDateTime(tendAgreementMaster.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),
                                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT.ToString(),      
                                    tendAgreementMaster.MAINTENANCE_AMOUNT.ToString(),
                                    tendAgreementMaster.AGREEMENT_STATUS.ToString(),
                                    //((dbContext.TEND_AGREEMENT_BG_RENEWAL.Where(s=>s.TEND_AGREEMENT_CODE==tendAgreementMaster.TEND_AGREEMENT_CODE).FirstOrDefault()==null)||(DateTime.Compare(dbContext.TEND_AGREEMENT_BG_RENEWAL.AsEnumerable().Where(t=>t.TEND_AGREEMENT_CODE==tendAgreementMaster.TEND_AGREEMENT_CODE).LastOrDefault().TEND_BG_EXPIRY_DATE,DateTime.Today)< 0)? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plus' title='Enter Bank details' onClick ='AddBankGuarantee(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil '  title='Edit Bank details' onClick ='EditBankGuarantee(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) + "\");' ></span></td></tr></table></center>" ),
                                     "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plus' title='Enter Bank Guarantee Details' onClick ='AddBankGuarantee(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) + "\");' ></span></td></tr></table></center>",
                                     (dbContext.TEND_AGREEMENT_BG_RENEWAL.Where(s=>s.TEND_AGREEMENT_CODE==tendAgreementMaster.TEND_AGREEMENT_CODE).FirstOrDefault()!=null? URLEncrypt.EncryptParameters1(new string[] {"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }):"")
                                    //tendAgreementMaster.TEND_AGREEMENT_STATUS == "W" ? "<a href='#' title='Click here to view request details' class='ui-icon ui-icon-locked ui-align-center' onClick=ChangeAgreementStatus('" + URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) +"'); return false;'>Change</a>" : "<span class='ui-icon ui-icon-unlocked ui-align-center'></span>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetAgreementListBALForBGDAL()");
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


        public bool AddBankGuaranteeDetails(BankGuaranteeDetailsModel model, out String IsValid)
        {
            int status = 0;
            int AgreementCode = 0;
            decimal Agrmeentsum = 0;
            try
            {
                using (var scope = new TransactionScope())
                {
                    var dbContext = new PMGSYEntities();
                    TEND_AGREEMENT_BG_RENEWAL AgrModel = new TEND_AGREEMENT_BG_RENEWAL();
                    AgreementCode = Convert.ToInt32(model.AGREEMENT_CODE);
                    AgrModel.TEND_AGREEMENT_CODE = Convert.ToInt32(model.AGREEMENT_CODE);

                    //DateTime AgreementDate = dbContext.TEND_AGREEMENT_MASTER.Where(t => t.TEND_AGREEMENT_CODE == AgreementCode).SingleOrDefault().TEND_DATE_OF_AGREEMENT;
                    //if (!((AgreementDate  < Convert.ToDateTime(model.BG_ISSUE_DATE)) &&(AgreementDate < Convert.ToDateTime(model.BG_EXPIRY_DATE)) ))
                    //{
                    //    IsValid = "Issue Date and Expiry Date should be greater than Agreement Date";
                    //    return false;                        
                    //}

                    decimal AgreementAmt = dbContext.TEND_AGREEMENT_MASTER.Where(t => t.TEND_AGREEMENT_CODE == AgreementCode).FirstOrDefault().TEND_AGREEMENT_AMOUNT;
                    Agrmeentsum = dbContext.TEND_AGREEMENT_BG_RENEWAL.Where(b => b.TEND_AGREEMENT_CODE == AgreementCode).Any() ? dbContext.TEND_AGREEMENT_BG_RENEWAL.Where(b => b.TEND_AGREEMENT_CODE == AgreementCode).Sum(s => s.TEND_BG_AMOUNT) : 0;
                    if (AgreementAmt < (Agrmeentsum + model.BG_AMOUNT))
                    {
                        IsValid = "Bank guarantee/FDR amount exceeding the Agreement amount";// by  ₹ " + ((Agrmeentsum + model.BG_AMOUNT) - AgreementAmt).ToString("0.000") + " Lakhs";
                        return false;
                    }
                    AgrModel.TEND_BG_AMOUNT = model.BG_AMOUNT;
                    AgrModel.TEND_BG_BANK_NAME = model.BG_BANK_NAME;
                    AgrModel.TEND_BG_EXPIRY_DATE = Convert.ToDateTime(model.BG_EXPIRY_DATE);
                    AgrModel.TEND_BG_ISSUE_DATE = Convert.ToDateTime(model.BG_ISSUE_DATE);
                    if (DateTime.Compare(Convert.ToDateTime(model.BG_VERIFICATION_DATE), Convert.ToDateTime(model.BG_ISSUE_DATE)) >= 0 && DateTime.Compare(Convert.ToDateTime(model.BG_VERIFICATION_DATE), Convert.ToDateTime(model.BG_EXPIRY_DATE)) < 0)
                    {
                        AgrModel.TEND_BG_VERIFIED_DATE = Convert.ToDateTime(model.BG_VERIFICATION_DATE);
                    }
                    else
                    {
                        IsValid = "Verification date should be less than Expiry date and greater than or equal to Issue date.";
                        return false;
                    }
                    AgrModel.TEND_BG_VERIFIED_BY = model.VERIFIEDBY;
                    AgrModel.TEND_BG_CODE = dbContext.TEND_AGREEMENT_BG_RENEWAL.Any() ? dbContext.TEND_AGREEMENT_BG_RENEWAL.Max(s => s.TEND_BG_CODE) + 1 : 1;
                    AgrModel.TEND_BG_STATUS = "A";
                    AgrModel.USERID = PMGSYSession.Current.UserId;
                    AgrModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    AgrModel.TEND_BG_FILE_NAME = "BG_" + AgrModel.TEND_AGREEMENT_CODE + "_" + AgrModel.TEND_BG_CODE + ".pdf";
                    dbContext.TEND_AGREEMENT_BG_RENEWAL.Add(AgrModel);

                    //Save the Bank Guarantee file 

                    String Path = ConfigurationManager.AppSettings["BANK_GAURANTEE_MAIN"].ToString();

                    String FileName = "BG_" + AgrModel.TEND_AGREEMENT_CODE + "_" + AgrModel.TEND_BG_CODE + ".pdf";
                    model.BGFile.SaveAs(System.IO.Path.Combine(Path, FileName));

                    status = dbContext.SaveChanges();
                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Bank guarantee/FDR details added successfully.";
                    return true;
                }
                else
                {
                    IsValid = "Bank guarantee/FDR  details not added.";
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddBankGuaranteeDetails");
                IsValid = "Bank guarantee  details not added.";
                return false;

            }

        }

        public BankGuaranteeDetailsModel GetBankGuaranteeObj(string agrtCode)
        {
            try
            {

                string[] agrBgCode = agrtCode.Split(',');
                int agreementCode = Convert.ToInt32(agrBgCode[0]);
                int tendBankcode = Convert.ToInt32(agrBgCode[1]);

                using (var scope = new TransactionScope())
                {
                    using (var dbContext = new PMGSYEntities())
                    {
                        TEND_AGREEMENT_BG_RENEWAL BankObj = dbContext.TEND_AGREEMENT_BG_RENEWAL.AsEnumerable().Where(T => T.TEND_AGREEMENT_CODE == agreementCode && T.TEND_BG_CODE == tendBankcode).LastOrDefault();
                        BankGuaranteeDetailsModel model = new BankGuaranteeDetailsModel();
                        model.TendBgCode = tendBankcode;
                        model.AGREEMENT_CODE = URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode=" + BankObj.TEND_AGREEMENT_CODE.ToString() });
                        model.BG_AMOUNT = Convert.ToDecimal(BankObj.TEND_BG_AMOUNT.ToString("0.00").TrimEnd('0').TrimEnd('.'));
                        model.BG_ISSUE_DATE = BankObj.TEND_BG_ISSUE_DATE.ToString("dd/MM/yyyy");
                        model.BG_EXPIRY_DATE = BankObj.TEND_BG_EXPIRY_DATE.ToString("dd/MM/yyyy");
                        model.BG_VERIFICATION_DATE = BankObj.TEND_BG_VERIFIED_DATE.HasValue ? BankObj.TEND_BG_VERIFIED_DATE.Value.ToString("dd/MM/yyyy") : string.Empty;

                        model.VERIFIEDBY = BankObj.TEND_BG_VERIFIED_BY;
                        model.BG_BANK_NAME = BankObj.TEND_BG_BANK_NAME;
                        model.TEND_BG_STATUS = BankObj.TEND_BG_STATUS;
                        model.Operation = "U";
                        model.BGfileName = BankObj.TEND_BG_FILE_NAME;
                        scope.Complete();
                        return model;
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBankGuaranteeObj");
                return null;
            }


        }

        public bool EditBankGuaranteeDetails(BankGuaranteeDetailsModel model, out String isValidMsg)
        {
            try
            {
                int status = 0;
                using (var scope = new TransactionScope())
                {
                    using (var dbContext = new PMGSYEntities())
                    {
                        int aggrementCode = Convert.ToInt32(model.AGREEMENT_CODE);
                        TEND_AGREEMENT_BG_RENEWAL AgrModel = dbContext.TEND_AGREEMENT_BG_RENEWAL.Where(T => T.TEND_AGREEMENT_CODE == aggrementCode && T.TEND_BG_CODE == model.TendBgCode).FirstOrDefault();

                        int AgreementCode = Convert.ToInt32(model.AGREEMENT_CODE);
                        AgrModel.TEND_AGREEMENT_CODE = Convert.ToInt32(model.AGREEMENT_CODE);

                        //DateTime AgreementDate = dbContext.TEND_AGREEMENT_MASTER.Where(t => t.TEND_AGREEMENT_CODE == AgreementCode).SingleOrDefault().TEND_DATE_OF_AGREEMENT;
                        //if (!((AgreementDate < Convert.ToDateTime(model.BG_ISSUE_DATE)) && (AgreementDate < Convert.ToDateTime(model.BG_EXPIRY_DATE))))
                        //{
                        //    isValidMsg = "Issue Date and Expiry Date should be greater than Agreement Date";
                        //    return false;
                        //}

                        decimal AgreementAmt = dbContext.TEND_AGREEMENT_MASTER.Where(t => t.TEND_AGREEMENT_CODE == AgreementCode).FirstOrDefault().TEND_AGREEMENT_AMOUNT;

                        decimal Agrmeentsum = dbContext.TEND_AGREEMENT_BG_RENEWAL.Where(b => b.TEND_AGREEMENT_CODE == AgreementCode).Any() ? dbContext.TEND_AGREEMENT_BG_RENEWAL.Where(b => b.TEND_AGREEMENT_CODE == AgreementCode).Sum(s => s.TEND_BG_AMOUNT) - AgrModel.TEND_BG_AMOUNT : 0;

                        if (AgreementAmt < (Agrmeentsum + model.BG_AMOUNT))
                        {
                            isValidMsg = "Bank guarantee/FDR amount exceeding the Agreement amount";// by  ₹ " + ((Agrmeentsum + model.BG_AMOUNT) - AgreementAmt).ToString("0.000") + " Lakhs";
                            return false;
                        }

                        AgrModel.TEND_AGREEMENT_CODE = Convert.ToInt32(model.AGREEMENT_CODE);
                        AgrModel.TEND_BG_AMOUNT = model.BG_AMOUNT;
                        AgrModel.TEND_BG_BANK_NAME = model.BG_BANK_NAME;
                        AgrModel.TEND_BG_EXPIRY_DATE = Convert.ToDateTime(model.BG_EXPIRY_DATE);
                        AgrModel.TEND_BG_ISSUE_DATE = Convert.ToDateTime(model.BG_ISSUE_DATE);
                        // AgrModel.TEND_BG_CODE = dbContext.TEND_AGREEMENT_BG_RENEWAL.Any() ? dbContext.TEND_AGREEMENT_BG_RENEWAL.Max(s => s.TEND_BG_CODE) + 1 : 1;

                        if (DateTime.Compare(Convert.ToDateTime(model.BG_VERIFICATION_DATE), Convert.ToDateTime(model.BG_ISSUE_DATE)) >= 0 && DateTime.Compare(Convert.ToDateTime(model.BG_VERIFICATION_DATE), Convert.ToDateTime(model.BG_EXPIRY_DATE)) < 0)
                        {
                            AgrModel.TEND_BG_VERIFIED_DATE = Convert.ToDateTime(model.BG_VERIFICATION_DATE);
                        }
                        else
                        {
                            isValidMsg = "Verification date should be less than Expiry date and greater than or equal to Issue date.";
                            return false;
                        }

                        AgrModel.TEND_BG_VERIFIED_BY = model.VERIFIEDBY;
                        AgrModel.TEND_BG_STATUS = (model.TEND_BG_STATUS == null) || (model.TEND_BG_STATUS == String.Empty) ? "A" : model.TEND_BG_STATUS;
                        AgrModel.USERID = PMGSYSession.Current.UserId;
                        AgrModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        // No need to update the file Name in the existing obj as [logic of fileName is Same]
                        if (model.BGFile != null)
                        {
                            String Path = ConfigurationManager.AppSettings["BANK_GAURANTEE_MAIN"].ToString();

                            String FileName = "BG_" + AgrModel.TEND_AGREEMENT_CODE + "_" + AgrModel.TEND_BG_CODE + ".pdf";

                            System.IO.File.Delete(System.IO.Path.Combine(Path, FileName));  //remove the old file
                            model.BGFile.SaveAs(System.IO.Path.Combine(Path, FileName)); //Save new File
                        }

                        dbContext.Entry(AgrModel).State = System.Data.Entity.EntityState.Modified;
                        status = dbContext.SaveChanges();
                        scope.Complete();

                    }
                    if (status > 0)
                    {
                        isValidMsg = "Bank guarantee/FDR details updated successfully.";
                        return true;
                    }
                    else
                    {
                        isValidMsg = "Bank guarantee/FDR details not updated.";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditBankGuaranteeDetails");
                isValidMsg = "Bank guarantee/FDR details not updated successfully.";
                return false;
            }

        }

        public Array GetExpBankGuaranteeList(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, int districtCode, String ActiveStatus, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {

                //var lstallAgreements
                var lstAgreements = dbContext.USP_GET_AGREEMENTS_BG(PMGSYSession.Current.StateCode, districtCode, (blockCode <= 0 ? 0 : blockCode), (yearCode <= 0 ? 0 : yearCode), (package == "0" ? "%" : (package == "All" ? "%" : package)), 0, (agreementStatus == "0" ? "%" : agreementStatus), "Y", (proposalType == "0" ? "%" : (proposalType == "All" ? "%" : proposalType)), "C", PMGSYSession.Current.PMGSYScheme).ToList();

                DateTime firstday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime lastday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));


                //var lstAgreements = (from agreement in lstallAgreements
                //                     join bank in dbContext.TEND_AGREEMENT_BG_RENEWAL on agreement.TEND_AGREEMENT_CODE equals bank.TEND_AGREEMENT_CODE
                //                     where (bank.TEND_BG_EXPIRY_DATE >= firstday
                //                     && bank.TEND_BG_EXPIRY_DATE <= lastday)
                //                     select new { agreement, bank }).ToList();

                //var lstAgreements = (from agreement in lstallAgreements
                //                     join bank in dbContext.TEND_AGREEMENT_BG_RENEWAL on agreement.TEND_AGREEMENT_CODE equals bank.TEND_AGREEMENT_CODE
                //                     select new { agreement, bank }).ToList();

                if (ActiveStatus == "E")
                {
                    lstAgreements = lstAgreements.Where(s => s.TEND_BG_EXPIRY_DATE >= firstday && s.TEND_BG_EXPIRY_DATE <= lastday).ToList();
                }
                else if (ActiveStatus == "T")
                {
                    lstAgreements = lstAgreements.Where(s => s.TEND_BG_EXPIRY_DATE < DateTime.Today).ToList();
                }
                else if (ActiveStatus == "A")
                {
                    lstAgreements = lstAgreements.Where(s => s.TEND_BG_EXPIRY_DATE >= DateTime.Today).ToList();
                }
                //var test = lstallAgreements.Join(
                //                                dbContext.TEND_AGREEMENT_BG_RENEWAL,
                //                                agreement => agreement.TEND_AGREEMENT_CODE,
                //                                bank => bank.TEND_AGREEMENT_CODE,
                //                                 (agreement, bank) => new { agreement, bank }).Where(a => a.bank.TEND_BG_EXPIRY_DATE >= firstday && a.bank.TEND_BG_EXPIRY_DATE < lastday).ToList();

                totalRecords = lstAgreements.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                lstAgreements = lstAgreements.OrderBy(x => x.CONTRACTOR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementType":
                                lstAgreements = lstAgreements.OrderBy(x => x.AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementNumber":
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementDate":
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAgreements = lstAgreements.OrderBy(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.CONTRACTOR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementType":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementNumber":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementDate":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAgreements = lstAgreements.OrderByDescending(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }

                    }
                }
                else
                {
                    lstAgreements = lstAgreements.OrderBy(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstAgreements.Select(tendAgreementMaster => new
                {
                    tendAgreementMaster.TEND_AGREEMENT_CODE,
                    tendAgreementMaster.CONTRACTOR_NAME,
                    tendAgreementMaster.MAST_DISTRICT_NAME,
                    tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                    tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                    tendAgreementMaster.TEND_BG_EXPIRY_DATE,
                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                    tendAgreementMaster.TEND_AGREEMENT_END_DATE,
                    tendAgreementMaster.TEND_BG_BANK_NAME,
                    tendAgreementMaster.TEND_BG_CODE

                }).ToArray();

                return result.Select(tendAgreementMaster => new
                {

                    id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {                                                                               
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    tendAgreementMaster.MAST_DISTRICT_NAME==null?"NA":tendAgreementMaster.MAST_DISTRICT_NAME,
                                    tendAgreementMaster.TEND_AGREEMENT_NUMBER.ToString(),
                                    tendAgreementMaster.CONTRACTOR_NAME==null?"NA":tendAgreementMaster.CONTRACTOR_NAME.ToString().Trim(),
                                    dbContext.MASTER_CONTRACTOR.Where(x=> x.MAST_CON_ID ==dbContext.TEND_AGREEMENT_MASTER.Where(s=>s.TEND_AGREEMENT_CODE == tendAgreementMaster.TEND_AGREEMENT_CODE).FirstOrDefault().MAST_CON_ID).FirstOrDefault().MAST_CON_PAN,
                                 // tendAgreementMaster.AGREEMENT_TYPE.ToString(), 
                                    Convert.ToDateTime(tendAgreementMaster.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),
                                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT.ToString(),      
                                    Convert.ToDateTime(tendAgreementMaster.TEND_BG_EXPIRY_DATE).ToString("dd/MM/yyyy"),
                                    tendAgreementMaster.TEND_BG_BANK_NAME.ToString(),
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString(),"TendBGCode="+tendAgreementMaster.TEND_BG_CODE }),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExpBankGuaranteeListDAL()");
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

        public int GetExpiredBankGuaranteeCount()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                // var lstallAgreements = dbContext.USP_GET_AGREEMENTS(PMGSYSession.Current.DistrictCode,0 ,  0,  "%" , PMGSYSession.Current.AdminNdCode,  "%", "Y",  "%", "C", PMGSYSession.Current.PMGSYScheme).ToList();

                var lstallAgreements = dbContext.USP_GET_AGREEMENTS_BG(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, 0, 0, "%", 0, "%", "Y", "%", "C", PMGSYSession.Current.PMGSYScheme).ToList();

                DateTime firstday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime lastday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));


                //var lstAgreements = (from agreement in lstallAgreements
                //                     join bank in dbContext.TEND_AGREEMENT_BG_RENEWAL on agreement.TEND_AGREEMENT_CODE equals bank.TEND_AGREEMENT_CODE
                //                     select new { agreement, bank }).ToList();

                var lstAgreements = lstallAgreements.Where(s => s.TEND_BG_EXPIRY_DATE >= firstday && s.TEND_BG_EXPIRY_DATE <= lastday).ToList();

                return lstAgreements.Count;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExpiredBankGuaranteeCount()");
                return 0;
            }

        }

        public Array GetAgreementDetailsListForContractor(String parameter, String hash, String key, int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, out long totalRecords)
        {

            PMGSYEntities dbContext = new PMGSYEntities();
            int TendAgreementCode = 0;
            int TendBgCode = 0;
            var decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
            if (decryptParameters.Count > 0)
            {
                TendAgreementCode = Convert.ToInt32(decryptParameters["TendAgreementCode"].ToString());
            }
            if (decryptParameters.Count > 1)
            {
                TendBgCode = Convert.ToInt32(decryptParameters["TendBGCode"].ToString());
            }
            try
            {
                var lstAgreements = (from agreement in dbContext.TEND_AGREEMENT_MASTER
                                     join bg in dbContext.TEND_AGREEMENT_BG_RENEWAL on agreement.TEND_AGREEMENT_CODE equals bg.TEND_AGREEMENT_CODE
                                     join contractor in dbContext.MASTER_CONTRACTOR on agreement.MAST_CON_ID equals contractor.MAST_CON_ID

                                     select new
                                     {
                                         TEND_AGREEMENT_CODE = agreement.TEND_AGREEMENT_CODE,
                                         TEND_BG_CODE = bg.TEND_BG_CODE,
                                         CONTRACTOR_NAME = contractor.MAST_CON_FNAME + " " + contractor.MAST_CON_MNAME + " " + contractor.MAST_CON_LNAME,
                                         AGREEMENT_TYPE = agreement.TEND_AGREEMENT_TYPE,
                                         TEND_AGREEMENT_NUMBER = agreement.TEND_AGREEMENT_NUMBER,
                                         TEND_DATE_OF_AGREEMENT = agreement.TEND_DATE_OF_AGREEMENT,
                                         TEND_AGREEMENT_AMOUNT = agreement.TEND_AGREEMENT_AMOUNT,
                                         TEND_BG_STATUS = bg.TEND_BG_STATUS,
                                         TEND_BG_AMOUNT = bg.TEND_BG_AMOUNT,
                                         MAINTENANCE_AMOUNT = (agreement.TEND_AMOUNT_YEAR1 + agreement.TEND_AMOUNT_YEAR2 + agreement.TEND_AMOUNT_YEAR3 + agreement.TEND_AMOUNT_YEAR4 + agreement.TEND_AMOUNT_YEAR5),
                                         AGREEMENT_STATUS = agreement.TEND_AGREEMENT_STATUS == "P" ? "In Progress" : (agreement.TEND_AGREEMENT_STATUS == "C" ? "Completed" : (agreement.TEND_AGREEMENT_STATUS == "W" ? "Incomplete" : "-")),
                                     }).ToList();
                // where   TendAgreementCode ==agreement.TEND_AGREEMENT_CODE

                if (decryptParameters.Count > 1 && TendBgCode > 0)
                {
                    lstAgreements = lstAgreements.Where(x => x.TEND_AGREEMENT_CODE == TendAgreementCode && x.TEND_BG_CODE == TendBgCode).ToList();
                }
                else
                {
                    lstAgreements = lstAgreements.Where(x => x.TEND_AGREEMENT_CODE == TendAgreementCode).ToList();
                }
                totalRecords = lstAgreements.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                lstAgreements = lstAgreements.OrderBy(x => x.CONTRACTOR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementType":
                                lstAgreements = lstAgreements.OrderBy(x => x.AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementNumber":
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementDate":
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAgreements = lstAgreements.OrderBy(x => x.TEND_AGREEMENT_NUMBER).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.CONTRACTOR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementType":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementNumber":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "AgreementDate":
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAgreements = lstAgreements.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }

                    }
                }
                else
                {
                    lstAgreements = lstAgreements.OrderBy(x => x.CONTRACTOR_NAME).ThenBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstAgreements.Select(tendAgreementMaster => new
                {
                    tendAgreementMaster.TEND_AGREEMENT_CODE,
                    tendAgreementMaster.TEND_BG_CODE,
                    tendAgreementMaster.CONTRACTOR_NAME,
                    tendAgreementMaster.AGREEMENT_TYPE,
                    tendAgreementMaster.TEND_AGREEMENT_NUMBER,
                    tendAgreementMaster.TEND_DATE_OF_AGREEMENT,
                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT,
                    tendAgreementMaster.MAINTENANCE_AMOUNT,
                    tendAgreementMaster.AGREEMENT_STATUS,
                    tendAgreementMaster.TEND_BG_AMOUNT,
                    tendAgreementMaster.TEND_BG_STATUS
                }).ToArray();


                return result.Select(tendAgreementMaster => new
                {

                    id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {                                                                               
                                    URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString() }),
                                    tendAgreementMaster.TEND_AGREEMENT_NUMBER.ToString(),
                                    tendAgreementMaster.CONTRACTOR_NAME==null?"NA":tendAgreementMaster.CONTRACTOR_NAME.ToString().Trim(),
                                   // tendAgreementMaster.AGREEMENT_TYPE.ToString(), 
                                    Convert.ToDateTime(tendAgreementMaster.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),
                                    tendAgreementMaster.TEND_AGREEMENT_AMOUNT.ToString(),      
                                    tendAgreementMaster.MAINTENANCE_AMOUNT.ToString(),
                                    tendAgreementMaster.AGREEMENT_STATUS.ToString(),
                                    "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil '  title='Edit Bank Guarantee Details' onClick ='EditBankGuarantee(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendAgreementCode="+tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()+","+tendAgreementMaster.TEND_BG_CODE}) + "\");' ></span></td></tr></table></center>" ,
                                    tendAgreementMaster.TEND_BG_AMOUNT.ToString("0.00"),
                                    tendAgreementMaster.TEND_BG_STATUS.Equals("A") ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-check '  title='Active' onClick ='' ></span></td></tr></table></center>" :"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-cancel'  title='Expired' onClick ='' ></span></td></tr></table></center>",
                                    (dbContext.TEND_AGREEMENT_BG_RENEWAL.Where(s=>s.TEND_AGREEMENT_CODE==tendAgreementMaster.TEND_AGREEMENT_CODE).FirstOrDefault()!=null? URLEncrypt.EncryptParameters1(new string[] {"TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()+","+tendAgreementMaster.TEND_BG_CODE }):""),
                                     (dbContext.TEND_AGREEMENT_BG_RENEWAL.Where(s=>s.TEND_AGREEMENT_CODE==tendAgreementMaster.TEND_AGREEMENT_CODE).FirstOrDefault()==null ? "-": "<a href='/Agreement/GetBankGuarantee?id="+URLEncrypt.EncryptParameters1(new String[]{"BGFile ="+dbContext.TEND_AGREEMENT_BG_RENEWAL.Where(s=>s.TEND_AGREEMENT_CODE==tendAgreementMaster.TEND_AGREEMENT_CODE && s.TEND_BG_CODE==tendAgreementMaster.TEND_BG_CODE).FirstOrDefault().TEND_BG_FILE_NAME})+"' title='Click here to view Bank Guearantee Details' class='ui-icon ui-icon-arrowthickstop-1-s  ui-align-center' target=_blank></a>")
                                    //tendAgreementMaster.TEND_AGREEMENT_STATUS == "W" ? "<a href='#' title='Click here to view request details' class='ui-icon ui-icon-locked ui-align-center' onClick=ChangeAgreementStatus('" + URLEncrypt.EncryptParameters1(new string[] { "TendAgreementCode =" + tendAgreementMaster.TEND_AGREEMENT_CODE.ToString()}) +"'); return false;'>Change</a>" : "<span class='ui-icon ui-icon-unlocked ui-align-center'></span>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetAgreementListBALForBGDAL()");
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


    }
    public interface IAgreementDAL
    {
        bool ChangeTerminatedAgreementMasterStatusDAL(int agreementCode);

        bool ChangeTerminatedAgreementStatusDAL(int agreementCode, int roadCode);

        Array GetProposedRoadListDAL(bool isSplitWork, int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, string proposalType, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetAgreementDetailsListDAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetAgreementDetailsListITNODAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetProposedRoadITNOListDAL(bool isSplitWork, int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, string proposalType, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveAgreementDetailsDAL(AgreementDetails details_agreement, ref string message);

        ExistingAgreementDetails GetExistingAgreementDetailsDAL(int contractorID, int agreementCode);

        bool SaveExistingAgreementDetailsDAL(ExistingAgreementDetails details_agreement_existing, ref string message);

        Array GetAgreementDetailsListDAL_ByAgreementCode(int agreementCode, int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetAgreementDetailsListITNODAL_ByAgreementCode(int agreementCode, int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);

        AgreementDetails GetAgreementMasterDetailsDAL_ByAgreementCode(int agreementCode, bool isView = false);

        bool UpdateAgreementMasterDetailsDAL(AgreementDetails master_agreement, ref string message);

        bool UpdateAgreementMasterDetailsITNODAL(AgreementDetails master_agreement, ref string message);

        bool DeleteAgreementMasterDetailsDAL_ByAgreementCode(int agreementCode, ref string message);

        ExistingAgreementDetails GetAgreementDetailsDAL_ByAgreementID(int agreementCode, int agreementID);

        bool UpdateAgreementDetailsDAL(ExistingAgreementDetails details_agreement, ref string message);

        bool DeleteAgreementDetailsDAL_ByAgreementID(int agreementID, int agreementCode, ref string message);

        Array GetAgreementDetailsListDAL_WithoutRoad(int agreementYear, string status, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveAgreementDetailsDAL_WithoutRoad(AgreementDetails details_agreement, ref string message);

        bool UpdateAgreementMasterDetailsDAL_WithoutRoad(AgreementDetails master_agreement, ref string message);

        bool FinalizeAgreementDAL(int agreementCode);

        bool ChangeAgreementStatusToInCompleteDAL(IncompleteReason incompleteReason, ref string message);

        bool CheckSplitWorkFinalizedDAL(int IMSPRRoadCode);

        bool CheckforActiveAgreementDAL(int IMSPRRoadCode, string agreementType, ref bool isAgreementAvailable);


        bool ChangeAgreementStatusToCompleteDAL(int agreementCode);

        bool UpdateAgreementDetailsITNODAL(ExistingAgreementDetails details_agreement, ref string message);

        #region PROPOSAL_RELATED_DETAILS

        Array GetProposalAgreementListDAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion

        #region FINALIZE_AGREEMENT

        Array GetAgreementListDAL(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, out long totalRecords);
        bool DeFinalizeAgreementDAL(int agreementCode);
        #endregion

        #region SPECIAL_AGREEMENT

        Array GetSpecialAgreementProposedRoadListDAL(bool isSplitWork, int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, string proposalType, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);
        bool SaveSpecialAgreementDetailsDAL(SpecialAgreementDetails details_agreement, ref string message);
        bool UpdateSpecialAgreementMasterDetailsDAL(SpecialAgreementDetails details_agreement, ref string message);
        Array GetSpecialAgreementDetailsListDAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetSpecialAgreementDetailsListDAL_ByAgreementCode(int agreementCode, int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        SpecialAgreementDetails GetSpecialAgreementMasterDetailsDAL_ByAgreementCode(int agreementCode, bool isView = false);
        bool UpdateSpecialAgreementDetailsDAL(ExistingSpecialAgreementDetails details_agreement, ref string message);
        ExistingSpecialAgreementDetails GetSpecialAgreementDetailsDAL_ByAgreementID(int agreementCode, int agreementID);

        #endregion

        #region AGREEMENT_UPDATION

        Array GetAgreementListForUpdationDAL(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, int state, int district, out long totalRecords);
        bool ChangeAgreementStatusDAL(int agreementId);

        #endregion

        #region 01 June 2016
        MASTER_CONTRACTOR_BANK GetContratorBankAccNoAndIFSCcode(int contractorId);
        #endregion

        #region Bank Guarantee

        Array GetAgreementListBALForBG(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, out long totalRecords);
        Array GetAgreementDetailsListForContractor(String parameter, String hash, String key, int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, out long totalRecords);
        bool AddBankGuaranteeDetails(BankGuaranteeDetailsModel model, out String IsValid);
        bool EditBankGuaranteeDetails(BankGuaranteeDetailsModel model, out String isValidMsg);
        BankGuaranteeDetailsModel GetBankGuaranteeObj(string agreementCode);
        Array GetExpBankGuaranteeList(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, int districtCode, String ActiveStatus, out long totalRecords);
        int GetExpiredBankGuaranteeCount();

        #endregion
    }

}